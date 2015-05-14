using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Tutoring.WebApp.Models
{
	public class TutoringContext : DbContext
	{
		public TutoringContext() : base("TutoringConnection") { }
		public DbSet<Person> People { get; set; }
		public DbSet<Class> Classes { get; set; }
		public DbSet<Tutorship> Tutorships { get; set; }
	}
}