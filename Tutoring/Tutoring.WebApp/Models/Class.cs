using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tutoring.WebApp.Models
{
	public class Class
	{
		public int ClassId { get; set; }
		public int ProfessorId { get; set; }
		public Person Professor { get; set; }
		public string Name { get; set; }
		public string Code { get; set; }
		public string Faculty { get; set; }
	}
}