using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Tutoring.WebApp.Models;
using Tutoring.WebApp.BusinessRules;
using Tutoring.WebApp.Models.ClassViewModels;

namespace Tutoring.WebApp.Controllers
{
    public class ClassController : Controller
    {

		[AccessPolicy(AccessPermissions.CLASS_INDEX)]
        public ActionResult Index(string teacher_id = null)
        {
			using (var context = new TutoringContext())
			{
				string user_token = User.Identity.GetUserName();
				Person user = context.People.First(x => x.PucrsId == user_token);

				IndexViewModel model = new IndexViewModel();

				if (UserTypeAccessLevel.GetAccessLevel(user.UserType) >= AccessPermissions.CLASS_EDIT)
					model.CanEdit = true;

				Person teacher = context.People.FirstOrDefault(x => x.PucrsId == teacher_id);
				if(teacher != null)
				{
					model.Classes = context.Classes
						.Include("Professor")
						.Where(x => x.Professor.PucrsId == teacher_id)
						.OrderBy(x => x.Faculty)
						.ThenBy(x => x.Name)
						.ToList();

					model.Teacher = teacher.Name;
				}
				else
				{
					model.Classes = context.Classes
						.Include("Professor")
						.OrderBy(x => x.Faculty)
						.ThenBy(x => x.Name)
						.ToList();
				}

				return View(model);
			}
        }

		[AccessPolicy(AccessPermissions.CLASS_EDIT)]
		public ActionResult Edit(int id = -1)
		{
			EditViewModel model = new EditViewModel();
			model.FormType = "Add";
			model.Id = id;
			if (id != -1) using (var context = new TutoringContext())
			{
				Class c = context.Classes
					.Include("Professor")
					.FirstOrDefault(x => x.ClassId == id);
				if(c != null)
				{
					model.FormType = "Edit";
					model.Name = c.Name;
					model.Code = c.Code;
					model.Faculty = c.Faculty;
					model.TeacherId = c.Professor.PucrsId;
				}
			}

			return View(model);
		}

		[HttpPost]
		[AccessPolicy(AccessPermissions.CLASS_EDIT)]
		public ActionResult Edit(EditViewModel model)
		{
			using (var context = new TutoringContext())
			{
				Person teacher = context.People.FirstOrDefault(x => x.PucrsId == model.TeacherId);
				if(teacher == null || UserTypeAccessLevel.GetAccessLevel(teacher.UserType) < UserTypeAccessLevel.UserTypes["Professor"])
				{
					ModelState.AddModelError("", "The specified PUCRS ID does not belong to any registered teacher.");
					return View(model);
				}

				Class c = context.Classes.FirstOrDefault(x => x.ClassId == model.Id);
				if(c != null)
				{
					c.Name = model.Name;
					c.Faculty = model.Faculty;
					
					if (model.Code != c.Code && context.Classes.Any(x => x.Code == model.Code))
					{
						ModelState.AddModelError("", "The specified class code is already in use.");
						return View(model);
					}

					c.Code = model.Code;
					c.Professor = teacher;
					
				}
				else
				{
					c = new Class();
					c.Name = model.Name;
					c.Faculty = model.Faculty;

					if (context.Classes.Any(x => x.Code == model.Code))
					{
						ModelState.AddModelError("", "The specified class code is already in use.");
						return View(model);
					}

					c.Code = model.Code;
					c.Professor = teacher;
					context.Classes.Add(c);
				}

				context.SaveChanges();
			}

			return RedirectToAction("Index");
		}


    }
}