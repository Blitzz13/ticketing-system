﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TicketingSystem.Services;
using TicketingSystem.Services.Impl;
using TicketingSystem.Web.Models.Message;
using TicketingSystem.Web.Models.Ticket;

namespace TicketingSystem.Web.Controllers
{
	public class TicketController : Controller
	{
		public readonly ITicketService _ticketService = new TicketService();

		public readonly IProjectService _projectService = new ProjectService();

		public readonly IUserService _userService = new UserService();

		public readonly IMessageService _messageService = new MessageService();

		private const int PageSize = 5;

		public TicketController(ITicketService ticketService, IProjectService projectService,
			IUserService userService, IMessageService messageService)
		{
			_ticketService = ticketService;
			_projectService = projectService;
			_userService = userService;
			_messageService = messageService;
		}

		[HttpGet]
		[Authorize]
		public IActionResult Create()
		{
			TicketFormViewModel model = new TicketFormViewModel();

			GetProjectsName(model);

			return View(model);
		}

		[HttpPost]
		[Authorize]
		public IActionResult Create(TicketFormViewModel viewModel)
		{
			var model = new CreateTicketModel();

			viewModel.ProjectId = _projectService.GetByName(viewModel.ProjectName).Id;
			int userId = GetUserId();

			string path = viewModel.FilePath;
			if (!string.IsNullOrEmpty(path))
			{
				byte[] file = System.IO.File.ReadAllBytes(path);
				string fileName = Path.GetFileName(path);

				model = new CreateTicketModel
				{
					ProjectId = viewModel.ProjectId,
					TicketTitle = viewModel.TicketTitle,
					TicketDescription = viewModel.Description,
					TicketState = viewModel.TicketState.Replace(" ", ""),
					TicketType = viewModel.TicketType.Replace(" ", ""),
					FileContent = file,
					FileName = fileName,
					SubmitterId = userId
				};
			}

			model = new CreateTicketModel
			{
				ProjectId = viewModel.ProjectId,
				TicketTitle = viewModel.TicketTitle,
				TicketDescription = viewModel.Description,
				TicketState = viewModel.TicketState.Replace(" ", ""),
				TicketType = viewModel.TicketType.Replace(" ", ""),
				SubmitterId = userId
			};

			try
			{
				_ticketService.Create(model);
			}
			catch (System.Exception e)
			{
				viewModel.ErrorMessage = e.Message;
				GetProjectsName(viewModel);
				return View(nameof(Create), viewModel);
			}

			return RedirectToAction(nameof(HomeController.Index), "Home");
		}

		[HttpGet]
		[Authorize]
		public IActionResult ListTickets(int page = 1)
		{
			int userId = GetUserId();
			var ticketsToShow = new List<OverviewTicketViewModel>();
			if (User.IsInRole("Client"))
			{
				int ticketsCount = _ticketService.GetAllTicketsForClient(userId).Count();

				var tickets = _ticketService.GetAllTicketsToShowForClient(userId, page, PageSize);

				CreateOverviewTickets(ticketsToShow, tickets);

				return View(new TicketListingViewModel
				{
					Tickets = ticketsToShow,
					CurrentPage = page,
					TotalPages = (int)Math.Ceiling(ticketsCount / (double)PageSize)
				});
			}
			else
			{
				int ticketsCount = _ticketService.GetAllTicketsForAdminAndSupport().Count();

				var tickets = _ticketService.GetAllTicketsToShowForAdminAndSupport();

				CreateOverviewTickets(ticketsToShow, _ticketService.GetAllTicketsToShowForAdminAndSupport(page, PageSize));

				return View(new TicketListingViewModel
				{
					Tickets = ticketsToShow,
					CurrentPage = page,
					TotalPages = (int)Math.Ceiling(ticketsCount / (double)PageSize)
				});

			}
		}

		[HttpPost]
		[Authorize]
		public IActionResult DeleteTicket(int id)
		{
			_ticketService.Delete(id);

			return RedirectToAction(nameof(ListTickets));
		}

		[HttpGet]
		[Authorize]
		public IActionResult View(int id)
		{
			Ticket ticket = _ticketService.GetByTicketId(id);

			int currnetUserId = _userService.GetByUsername(User.Identity.Name).Id;

			if (User.IsInRole("Client") && currnetUserId != ticket.SubmitterId)
			{
				return NotFound();
			}

			var model = new ViewTicketViewModel();

			try
			{
				var messages = new List<Message>();

				messages.AddRange(_messageService.Get(id));

				var viewMessages = new List<MessageViewModel>();

				foreach (var message in messages)
				{

					var viewModel = new MessageViewModel
					{
						Id = message.Id,
						Content = message.Content,
						PublishingDate = message.PublishingDate,
						State = message.State,
						UserId = message.UserId,
						Username = message.Username
					};

					viewMessages.Add(viewModel);
				}

				model = new ViewTicketViewModel
				{
					TicketId = id,
					TicketTitle = ticket.Title,
					Description = ticket.Description,
					FileName = ticket.FileName,
					ProjectName = _projectService.GetById(ticket.ProjectId).Name,
					SubmitterId = ticket.SubmitterId,
					TicketState = ticket.State,
					TicketType = ticket.Type,
					Messages = viewMessages
				};

				return View(model);
			}
			catch
			{
				model = new ViewTicketViewModel
				{
					TicketId = id,
					TicketTitle = ticket.Title,
					Description = ticket.Description,
					FileName = ticket.FileName,
					ProjectName = _projectService.GetById(ticket.ProjectId).Name,
					SubmitterId = ticket.SubmitterId,
					TicketState = ticket.State,
					TicketType = ticket.Type
				};
				return View(model);
			}


		}

		[HttpGet]
		[Authorize]
		public IActionResult Edit(int id)
		{
			Ticket ticket = _ticketService.GetByTicketId(id);

			int currnetUserId = _userService.GetByUsername(User.Identity.Name).Id;

			if (User.IsInRole("Client") && currnetUserId != ticket.SubmitterId)
			{
				return NotFound();
			}

			var model = new TicketFormViewModel
			{
				TicketId = id,
				TicketTitle = ticket.Title,
				Description = ticket.Description,
				FileName = ticket.FileName,
				ProjectName = _projectService.GetById(ticket.ProjectId).Name,
				SubmitterId = ticket.SubmitterId,
				TicketState = ticket.State,
				TicketType = ticket.Type,
			};

			return View(model);
		}

		[HttpGet]
		[Authorize]
		public IActionResult Delete(int id)
		{
			Ticket ticket = _ticketService.GetByTicketId(id);

			int currnetUserId = _userService.GetByUsername(User.Identity.Name).Id;

			if (User.IsInRole("Client") && currnetUserId != ticket.SubmitterId)
			{
				return NotFound();
			}

			var model = new TicketFormViewModel
			{
				TicketId = id,
				TicketTitle = ticket.Title,
				Description = ticket.Description,
				FileName = ticket.FileName,
				ProjectName = _projectService.GetById(ticket.ProjectId).Name,
				SubmitterId = ticket.SubmitterId,
				TicketState = ticket.State,
				TicketType = ticket.Type,
			};

			return View(model);
		}

		[HttpPost]
		[Authorize]
		public IActionResult Delete(int id, TicketFormViewModel viewTicket)
		{
			_ticketService.Delete(id);

			return RedirectToAction(nameof(ListTickets));
		}

		[HttpPost]
		[Authorize]
		public IActionResult Edit(int id, TicketFormViewModel viewModel)
		{
			int currnetUserId = _userService.GetByUsername(User.Identity.Name).Id;

			if (User.IsInRole("Client") && currnetUserId != viewModel.SubmitterId)
			{
				return NotFound();
			}

			var model = new UpdateTicketModel
			{
				Id = id,
				ProjectName = viewModel.ProjectName,
				Title = viewModel.TicketTitle,
				Description = viewModel.Description,
				State = viewModel.TicketState.Replace(" ", ""),
				Type = viewModel.TicketType.Replace(" ", ""),
			};

			try
			{
				_ticketService.Edit(model);
			}
			catch (Exception e)
			{
				viewModel.ErrorMessage = e.Message;
				viewModel.ProjectName = _projectService.GetById(_ticketService.GetByTicketId(id).ProjectId).Name;
				return View(viewModel);
			}

			return RedirectToAction(nameof(ListTickets));
		}

		private void CreateOverviewTickets(List<OverviewTicketViewModel> overviewTickets, IEnumerable<Ticket> tickets)
		{
			foreach (var ticket in tickets)
			{
				var viewModel = new OverviewTicketViewModel
				{
					Id = ticket.Id,
					Title = ticket.Title,
					ProjectName = _projectService.GetById(ticket.ProjectId).Name,
					SubmissionDate = ticket.SubmissionDate,
					SumitterName = _userService.GetByUserId(ticket.SubmitterId).Username,
					TicketState = ticket.State
				};
				overviewTickets.Add(viewModel);
			}
		}

		private static int GetUserId()
		{
			HttpContextAccessor accessor = new HttpContextAccessor();
			var claims = accessor.HttpContext.User.Claims;
			int userId = -1;

			foreach (var claim in claims)
			{
				string claimValue = claim.Value;
				int.TryParse(claimValue, out userId);
			}

			return userId;
		}

		private void GetProjectsName(TicketFormViewModel viewModel)
		{
			List<string> projectNames = _projectService.Get().Select(pr => pr.Name).ToList();

			viewModel.ProjectNames = new List<string>();

			foreach (var name in projectNames)
			{
				viewModel.ProjectNames.Add(name);
			}
		}
	}
}
