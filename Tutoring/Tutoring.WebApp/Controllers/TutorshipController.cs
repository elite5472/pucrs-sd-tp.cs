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
using Tutoring.WebApp.BusinessRules;
using Tutoring.WebApp.Models;
using Tutoring.WebApp.Models.TutorshipViewModels;

namespace Tutoring.WebApp.Controllers
{
    public class TutorshipController : Controller
    {
		[AccessPolicy(AccessPermissions.TUTORING_INDEX)]
        public ActionResult Index(int class_id = -1, string filter = null)
        {
			IndexViewModel model = new IndexViewModel();
			using(var context = new TutoringContext())
			{
				string usr_handle = User.Identity.GetUserName();
				Person user = context.People.First(x => x.PucrsId == usr_handle);

				var query = context.Tutorships
					.Include("Tutor")
					.Where(x => x.Seeking || x.Approved);
				if (class_id != -1)
				{
					Class c = context.Classes.FirstOrDefault(x => x.ClassId == class_id);
					if (c == null) return HttpNotFound();

					model.Class = c;
					query = query.Where(x => x.ClassId == class_id);
				}

				if (filter != null && filter.ToLower() == "approved")
				{
					model.Filter = "Approved";
					query = query.Where(x => x.Approved == true);
				}
				else if (filter != null && filter.ToLower() == "requested")
				{
					model.Filter = "Requested";
					query = query.Where(x => x.Seeking == true);
				}
				model.Tutorships = query.ToList();

				if (user.UserType == AccessPermissions.TUTORING_APPLY_TYPE) model.CanApply = true;
				if (user.UserType == AccessPermissions.TUTORING_SUBSCRIBE_TYPE) model.CanSubscribe = true;
				if (UserTypeAccessLevel.GetAccessLevel(user.UserType) >= AccessPermissions.TUTORING_PETITION) model.CanRequest = true;
				if (UserTypeAccessLevel.GetAccessLevel(user.UserType) >= AccessPermissions.TUTORING_APPROVE) model.CanApprove = true;
				if (UserTypeAccessLevel.GetAccessLevel(user.UserType) >= AccessPermissions.TUTORING_DENY) model.CanDeny = true;
			}
            return View(model);
        }

		[HttpGet]
		[AccessPolicy(AccessPermissions.TUTORING_PETITION)]
		public ActionResult Petition(int class_id)
		{
			PetitionViewModel model = new PetitionViewModel();
			using(var context = new TutoringContext())
			{
				model.Class = context.Classes.FirstOrDefault(x => x.ClassId == class_id);
				if (model.Class == null) return HttpNotFound();
			}
			return View(model);
		}

		[HttpPost]
		[AccessPolicy(AccessPermissions.TUTORING_PETITION)]
		public ActionResult Petition(PetitionViewModel model)
		{
			using(var context = new TutoringContext())
			{
				if (context.Tutorships.Any(x => x.ClassId == model.Class.ClassId && x.Seeking))
					ModelState.AddModelError("", "This class already has a tutorship request.");

				Tutorship t = new Tutorship();
				t.ClassId = model.Class.ClassId;
				t.About = model.About;
				t.Approved = false;
				t.Seeking = true;
				t.DateRequested = DateTime.Now;

				context.Tutorships.Add(t);
				context.SaveChanges();
			}
			return Index(model.Class.ClassId);
		}
    }
}