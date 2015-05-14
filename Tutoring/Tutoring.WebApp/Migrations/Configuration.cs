namespace Tutoring.WebApp.Migrations
{
	using Microsoft.AspNet.Identity;
	using Microsoft.AspNet.Identity.EntityFramework;
	using System;
	using System.Data.Entity;
	using System.Data.Entity.Migrations;
	using System.Linq;
	using Tutoring.WebApp.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<Tutoring.WebApp.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Tutoring.WebApp.Models.ApplicationDbContext context)
        {

        }
    }
}
