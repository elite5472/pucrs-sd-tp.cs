using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tutoring.WebApp.Models
{
	public class Tutorship
	{
		public int Id { get; set; }

		public int ClassId { get; set; }
		public Class Class { get; set; }

		public int? TutorId { get; set; }
		public Person Tutor { get; set; }

		public string About { get; set; }

		public bool Approved { get; set; }
		public bool Seeking { get; set; }

		public DateTime DateRequested { get; set; }

		public ICollection<Person> Applicants { get; set; }
		public ICollection<Person> Subscribers { get; set; }

		public Tutorship()
		{
			Applicants = new List<Person>();
			Subscribers = new List<Person>();
		}
	}
}