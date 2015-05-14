using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tutoring.WebApp.Models
{
	public class Person
	{
		public int PersonId { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string PucrsId { get; set; }
		public string UserType { get; set; }
	}
}