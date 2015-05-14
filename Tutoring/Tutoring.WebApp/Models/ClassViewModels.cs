using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tutoring.WebApp.Models.ClassViewModels
{
	public class IndexViewModel
	{
		public bool CanEdit { get; set; }
		public bool CanRequestTutorship { get; set; }
		public bool CanReviewTutorship { get; set; }
		public string Teacher { get; set; }
		public List<Class> Classes { get; set; }
	}
	public class EditViewModel
	{
		public string FormType { get; set; }

		public int Id { get; set; }

		[Required]
		[Display(Name = "Class Name")]
		public string Name { get; set; }
		[Required]
		[Display(Name = "Class Code")]
		public string Code { get; set; }
		[Required]
		[Display(Name = "Faculty")]
		public string Faculty { get; set; }
		[Required]
		[Display(Name = "Teacher PUCRS ID")]
		public string TeacherId { get; set; }
	}
}