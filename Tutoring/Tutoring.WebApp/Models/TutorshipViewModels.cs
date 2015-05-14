using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tutoring.WebApp.Models.TutorshipViewModels
{
	public class IndexViewModel
	{
		public bool CanApply { get; set; }
		public bool CanSubscribe { get; set; }
		public bool CanRequest { get; set; }
		public bool CanApprove { get; set; }
		public bool CanDeny { get; set; }

		public Class Class { get; set; }
		public string Filter { get; set; }
		public List<Tutorship> Tutorships { get; set; }

		public string GetStatus(Tutorship t)
		{
			if (t.Approved)
				return "Approved";
			if (t.Seeking)
				return "Requested";
			else
				return "Denied";
		}
	}

	public class PetitionViewModel
	{
		public Class Class { get; set; }

		[Required]
		[Display(Name = "About")]
		public String About { get; set; }
	}
}