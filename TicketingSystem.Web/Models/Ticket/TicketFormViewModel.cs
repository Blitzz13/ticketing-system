﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TicketingSystem.Web.Models.Ticket
{
	public class TicketFormViewModel
	{
		public List<string> ProjectNames { get; set; }

		[Display(Name = "Project Name")]
		public string ProjectName { get; set; }

		[Display(Name = "Ticket title")]
		public string TicketTitle { get; set; }

		[Display(Name = "Ticket types")]
		public string TicketType { get; set; }

		[Display(Name = "Ticket states")]
		public string TicketState { get; set; }

		[MinLength(5, ErrorMessage = "You should write atleast 5 characters")]
		public string Description { get; set; }

		[Display(Name = "File (optional)")]
		public string FilePath { get; set; }

		public string FileName { get; set; }

		public int ProjectId { get; set; }

		public int TicketId { get; set; }

		public int SubmitterId { get; set; }

		public string ErrorMessage { get; set; }
	}
}
