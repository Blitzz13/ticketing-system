﻿using System;
using System.Collections.Generic;

namespace TicketingSystem.Data
{
	public class Ticket
	{
		public int Id { get; set; }

		public DateTime SubmissionDate { get; set; }

		public int SubmitterId { get; set; }

		public User Submitter { get; set; }

		public TicketType Type { get; set; }

		public TicketState State { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }

		public ICollection<File> Files { get; set; } = new List<File>();

		public int ProjectId { get; set; }

		public Project Project { get; set; }

		public ICollection<Message> Messages { get; set; }
	}
}
