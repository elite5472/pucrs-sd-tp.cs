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
	public class HomeController : Controller
	{
		[AccessPolicy(1)]
		public ActionResult Index()
		{
			using (var context = new TutoringContext())
			{
				string user_token = User.Identity.GetUserName();
				Person user = context.People.First(x => x.PucrsId == user_token);

				if (user.UserType == "Student")
					return RedirectToAction("Index", "Class");

				if (user.UserType == "Professor")
					return RedirectToAction("Index", "Class", new { teacher_id = user.PucrsId });

				else return View();
			}
		}
	}
}