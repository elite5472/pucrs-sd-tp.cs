namespace Tutoring.WebApp.Migrations
{
	using Microsoft.AspNet.Identity;
	using Microsoft.AspNet.Identity.EntityFramework;
	using System;
	using System.Data.Entity;
	using System.Data.Entity.Migrations;
	using System.Linq;
	using Tutoring.WebApp.Models;

    internal class Configuration : DbMigrationsConfiguration<Tutoring.WebApp.Models.ApplicationDbContext>
    {
		public ApplicationUserManager Manager {get; set;}

        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Tutoring.WebApp.Models.ApplicationDbContext appcontext)
        {
			Manager = new ApplicationUserManager(new UserStore<ApplicationUser>(appcontext));
			using (var context = new TutoringContext())
			{
				CreateUser("1000", "Alfredo Sans", "alfredo.sans@pucrs.br", "Director");
				CreateUser("2394", "Maria Alcatraz", "maria.alcatraz@pucrs.br", "Staff");
				CreateUser("2925", "Rodrigo Vazquez", "rodrigo.vazquez@pucrs.br", "Staff");
				Person teacher1 = CreateUser("3460", "Adriano Turgh", "adriano.turgh@pucrs.br", "Professor");
				Person teacher2 = CreateUser("4932", "Katernie Schniezel", "katerine.schniezel@pucrs.br", "Professor");
				Person teacher3 = CreateUser("5440", "John Doe", "john.doe@pucrs.br", "Professor");
				CreateUser("9000", "Guillermo Borges", "guillermo.borges@acad.pucrs.br", "Student");
				CreateUser("9302", "Caio Borges", "caio.borges@acad.pucrs.br", "Student");
				CreateUser("9784", "Daniel Barcellos", "daniel.barcellos@acad.pucrs.br", "Student");
				CreateUser("10203", "Gabriela Pachecos", "gabriela.pachecos@acad.pucrs.br", "Student");
				CreateUser("10366", "Andres Covar", "andres.covar@acad.pucrs.br", "Student");
				CreateUser("10678", "Lairton Ballin", "lairton.ballin@acad.pucrs.br", "Student");

				Class c = new Class() { Code = "1572D-04", Faculty = "FACIN", ProfessorId = teacher1.PersonId, Name = "Artificial Intelligence", Professor = context.People.First(x => x.PucrsId == teacher1.PucrsId)};
				context.Classes.Add(c);

				c = new Class() { Code = "5429D-04", Faculty = "FACIN", ProfessorId = teacher2.PersonId, Name = "Peripherals Programming", Professor = context.People.First(x => x.PucrsId == teacher2.PucrsId) };
				context.Classes.Add(c);

				c = new Class() { Code = "4910D-03", Faculty = "FAMAT", ProfessorId = teacher3.PersonId, Name = "Statistics", Professor = context.People.First(x => x.PucrsId == teacher3.PucrsId) };
				context.Classes.Add(c);

				context.SaveChanges();	
			}
        }

		protected Person CreateUser(string pucrs_id, string name, string email, string type)
		{
			using (var context = new TutoringContext())
			{
				ApplicationUser user = new ApplicationUser()
				{
					UserName = pucrs_id,
					Email = email
				};

				Person person = new Person()
				{
					PucrsId = pucrs_id,
					Name = name,
					Email = email,
					UserType = type
				};

				Manager.Create(user, "@Password123");
				context.People.Add(person);
				context.SaveChanges();

				return context.People.First(x => x.PucrsId == pucrs_id);
			}
		}
    }
}
