﻿using System;
using System.Collections.Generic;
using System.Linq;
using DATA = TicketingSystem.Data;

namespace TicketingSystem.Services.Impl
{
	public class TicketService : ITicketService
	{
		private readonly DATA.TicketingSystemDbContext _context;

		public TicketService()
		{
			_context = new DATA.TicketingSystemDbContext();
		}

		public IEnumerable<Ticket> Get(int projectId, int? userId = null)
		{
			IQueryable<DATA.Ticket> tickets = _context.Tickets.Where(t => t.ProjectId == projectId);
			if (userId != null)
			{
				_context.Tickets.Where(t => t.Submitter.Id == userId.Value);
			}

			return tickets.ToList().Select(CreateTicket);
		}

		public IEnumerable<Ticket> GetAllTicketsForClient(int userId)
		{
			IQueryable<DATA.Ticket> tickets = _context.Tickets.Where(t => t.Submitter.Id == userId);

			return tickets.ToList().Select(CreateTicket);
		}

		public IEnumerable<Ticket> GetAllTicketsToShowForClient(int userId, int page = 1, int PageSize = 5)
		{
			List<Ticket> tickets =
				_context
				.Tickets
				.Where(t => t.Submitter.Id == userId)
				.Select(CreateTicket)
				.OrderByDescending(p => p.Id)
				.Skip((page - 1) * PageSize)
				.Take(PageSize)
				.ToList();

			return tickets;
		}

		
		public IEnumerable<Ticket> GetAllTicketsForAdminAndSupport()
		{
			List<Ticket> tickets = _context.Tickets.Select(CreateTicket).ToList();

			return tickets;
		}

		public IEnumerable<Ticket> GetAllTicketsToShowForAdminAndSupport(int page = 1, int PageSize = 5)
		{
			List<Ticket> tickets = 
				_context
				.Tickets
				.Select(CreateTicket)
				.OrderByDescending(p => p.Id)
				.Skip((page - 1) * PageSize)
				.Take(PageSize)
				.ToList();

			return tickets;
		}

		public int Create(CreateTicketModel model)
		{
			DATA.TicketType ticketType = (DATA.TicketType)Enum.Parse(typeof(DATA.TicketType), model.TicketType);
			if (!Enum.IsDefined(typeof(DATA.TicketType), ticketType))
			{
				throw new ServiceException("Invalid Ticket Type.");
			}

			DATA.TicketState ticketState = (DATA.TicketState)Enum.Parse(typeof(DATA.TicketState), model.TicketState);
			if (!Enum.IsDefined(typeof(DATA.TicketState), ticketState))
			{
				throw new ServiceException("Invalid Ticket State.");
			}

			if (string.IsNullOrEmpty(model.TicketTitle) || model.TicketTitle.Length < 5 || string.IsNullOrWhiteSpace(model.TicketTitle))
			{
				throw new ServiceException("The Ticket title should have no less than 5 characters.");
			}

			if (string.IsNullOrEmpty(model.TicketDescription) || model.TicketDescription.Length < 5)
			{
				throw new ServiceException("The description should have no less than 5 characters.");
			}

			if (_context.Tickets.Any(a => a.ProjectId == model.ProjectId && a.Title == model.TicketTitle))
			{
				throw new ServiceException($"This project already has ticket with title '{model.TicketTitle}'.");
			}

			DATA.Ticket ticket = new DATA.Ticket()
			{
				ProjectId = model.ProjectId,
				Description = model.TicketDescription,
				SubmissionDate = DateTime.Now,
				Title = model.TicketTitle,
				Type = ticketType,
				State = ticketState,
				SubmitterId = model.SubmitterId,
			};

			_context.Tickets.Add(ticket);

			if (!string.IsNullOrEmpty(model.FileName))
			{
				DATA.File file = new DATA.File
				{
					Name = model.FileName,
					Content = model.FileContent,
					TicketId = ticket.Id,
				};

				_context.Files.Add(file);
			}


			_context.SaveChanges();

			return ticket.Id;
		}

		public void Delete(int ticketId)
		{
			DATA.Ticket ticket = _context.Tickets.First(t => t.Id == ticketId);
			IEnumerable<DATA.Message> messages = _context.Messages.Where(m => m.TicketId == ticket.Id);

			_context.Messages.RemoveRange(messages);
			_context.Tickets.Remove(ticket);
			_context.SaveChanges();
		}
		public void Edit(UpdateTicketModel model)
		{
			DATA.TicketType ticketType = (DATA.TicketType)Enum.Parse(typeof(DATA.TicketType), model.Type);
			if (!Enum.IsDefined(typeof(DATA.TicketType), ticketType))
			{
				throw new ServiceException("Invalid Ticket Type.");
			}

			DATA.TicketState ticketState = (DATA.TicketState)Enum.Parse(typeof(DATA.TicketState), model.State);
			if (!Enum.IsDefined(typeof(DATA.TicketState), ticketState))
			{
				throw new ServiceException("Invalid Ticket State.");
			}

			if (string.IsNullOrEmpty(model.Title) || model.Title.Length < 5 || string.IsNullOrWhiteSpace(model.Title))
			{
				throw new ServiceException("The Ticket title should have no less than 5 characters.");
			}

			if (string.IsNullOrEmpty(model.Description) || model.Description.Length < 5)
			{
				throw new ServiceException("The description should have no less than 5 characters.");
			}

			DATA.Ticket ticket = _context.Tickets.First(t => t.Id == model.Id);

			ticket.Description = model.Description;
			ticket.Title = model.Title;
			ticket.Type = ticketType;
			ticket.State = ticketState;
			
			_context.SaveChanges();
		}

		public Ticket GetByTicketId(int ticketId)
		{
			DATA.Ticket ticket = _context.Tickets.First(t => t.Id == ticketId);

			return CreateTicket(ticket);
		}

		public Ticket GetByTitle(string ticketTitle)
		{
			DATA.Ticket ticket = _context.Tickets.First(t => t.Title == ticketTitle);

			return CreateTicket(ticket);
		}

		public Ticket GetByProjectId(int projectId)
		{
			DATA.Ticket ticket = _context.Tickets.First(t => t.ProjectId == projectId);

			return CreateTicket(ticket);
		}

		public Ticket GetByProjectIdAndTicketTitle(int projectId, string ticketTitle)
		{
			DATA.Ticket ticket = _context.Tickets.First(t => t.ProjectId == projectId && t.Title == ticketTitle);

			return CreateTicket(ticket);
		}

		public void ChangeType(UpdateTicketModel model)
		{
			DATA.Ticket ticket = _context.Tickets.First(t => t.Id == model.Id);

			if (ticket == null)
			{
				throw new ServiceException("Ticket not found.");
			}

			if (!string.IsNullOrEmpty(model.Type))
			{
				DATA.TicketType type = (DATA.TicketType)Enum.Parse(typeof(DATA.TicketType), model.Type);
				ticket.Type = type;
			}

			_context.SaveChanges();
		}

		public void ChangeState(UpdateTicketModel model)
		{
			DATA.Ticket ticket = _context.Tickets.First(t => t.Id == model.Id);

			if (ticket == null)
			{
				throw new ServiceException("Ticket not found.");
			}

			if (!string.IsNullOrEmpty(model.State))
			{
				DATA.TicketState state = (DATA.TicketState)Enum.Parse(typeof(DATA.TicketState), model.Type);
				ticket.State = state;
			}

			_context.SaveChanges();
		}

		public static Ticket CreateTicket(DATA.Ticket ticket)
		{
			return new Ticket
			{
				Id = ticket.Id,
				ProjectId = ticket.ProjectId,
				SubmitterId = ticket.SubmitterId,
				Title = ticket.Title,
				State = ticket.State.ToString(),
				Type = ticket.Type.ToString(),
				Description = ticket.Description,
				SubmissionDate = ticket.SubmissionDate,
			};
		}
	}
}
