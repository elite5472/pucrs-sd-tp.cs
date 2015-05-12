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
			var manager = new UserManager<ApplicationUser>(
				new UserStore<ApplicationUser>(
					new ApplicationDbContext()));

				var user = new ApplicationUser()
				{
					UserName = "111804993", Email = "guiborges@live.com", Name = "Guillermo Borges", PucrsId = "111804993", UserType = "Director"
				};
				manager.Create(user, "@Password123");
        }
    }
}
