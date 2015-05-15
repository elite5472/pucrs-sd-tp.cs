using System;
using System.Data.Entity;
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
using System.Collections.Generic;

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
					Class c = context.Classes
						.Include("Professor")
						.FirstOrDefault(x => x.ClassId == class_id);

					if (c == null) return HttpNotFound();

					model.Class = c;
					query = query.Where(x => x.ClassId == class_id);

					if (UserTypeAccessLevel.GetAccessLevel(user.UserType) >= AccessPermissions.TUTORING_PETITION &&
						usr_handle == c.Professor.PucrsId)
					{
						model.CanRequest = true;
					}
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
			}
            return View(model);
        }

		[AccessPolicy(AccessPermissions.TUTORING_INDEX)]
		public ActionResult Show(int id)
		{
			ShowViewModel model = new ShowViewModel();

			using(var context = new TutoringContext())
			{
				string usr_handle = User.Identity.GetUserName();
				Person user = context.People.First(x => x.PucrsId == usr_handle);

				Tutorship t = context.Tutorships
					.Include(x => x.Tutor)
					.Include(x => x.Class)
					.Include(x => x.Class.Professor)
					.Include(x => x.Applicants)
					.Include(x => x.Subscribers)
					.FirstOrDefault(x => x.TutorshipId == id);

				if (t == null)
					return HttpNotFound();

				model.Tutorship = t;

				if (t.Seeking && t.Applicants.Count() > 0)
					model.ShowApplicants = true;
				else if (t.Approved && t.Subscribers.Count() > 0)
					model.ShowSubscribers = true;

				if
				(
					t.Seeking &&
					user.UserType == AccessPermissions.TUTORING_APPLY_TYPE &&
					!t.Applicants.Any(x => x.PucrsId == user.PucrsId)
				) { model.CanApply = true; }

				if
				(
					t.Approved &&
					user.UserType == AccessPermissions.TUTORING_SUBSCRIBE_TYPE &&
					!t.Subscribers.Any(x => x.PucrsId == user.PucrsId) &&
					t.Tutor.PucrsId != user.PucrsId
				) { model.CanSubscribe = true; }

				if (t.Seeking && UserTypeAccessLevel.GetAccessLevel(user.UserType) >= AccessPermissions.TUTORING_APPROVE) model.CanApprove = true;
				if ((t.Seeking || t.Approved) && UserTypeAccessLevel.GetAccessLevel(user.UserType) >= AccessPermissions.TUTORING_DENY) model.CanDeny = true;
			}

			return View(model);
		}

		[AccessPolicy(AccessPermissions.TUTORING_INDEX)]
		public ActionResult TutorHistoryPartial(string id)
		{
			using (var context = new TutoringContext())
			{
				string usr_handle = User.Identity.GetUserName();
				Person user = context.People.First(x => x.PucrsId == usr_handle);

				List<Tutorship> model = context.Tutorships
					.Include(x => x.Tutor)
					.Include(x => x.Class)
					.Include(x => x.Class.Professor)
					.Where(x => x.Tutor.PucrsId == id)
					.ToList();

				return PartialView(model);
			}
		}

		[HttpGet]
		[AccessPolicy(AccessPermissions.TUTORING_APPLY_TYPE)]
		public ActionResult Apply(int id)
		{
			using(var context = new TutoringContext())
			{
				string usr_handle = User.Identity.GetUserName();
				Person user = context.People.First(x => x.PucrsId == usr_handle);

				Tutorship t = context.Tutorships
					.Include(x => x.Applicants)
					.FirstOrDefault(x => x.TutorshipId == id);

				if (t == null || !t.Seeking)
					return HttpNotFound();

				t.Applicants.Add(user);
				context.SaveChanges();
			}

			return RedirectToAction("Show", new { id = id });
		}

		[HttpGet]
		[AccessPolicy(AccessPermissions.TUTORING_SUBSCRIBE_TYPE)]
		public ActionResult Subscribe(int id)
		{
			using (var context = new TutoringContext())
			{
				string usr_handle = User.Identity.GetUserName();
				Person user = context.People.First(x => x.PucrsId == usr_handle);

				Tutorship t = context.Tutorships
					.Include(x => x.Subscribers)
					.FirstOrDefault(x => x.TutorshipId == id);

				if (t == null || !t.Approved)
					return HttpNotFound();

				t.Subscribers.Add(user);
				context.SaveChanges();
			}

			return RedirectToAction("Show", new { id = id });
		}

		[HttpGet]
		[AccessPolicy(AccessPermissions.TUTORING_DENY)]
		public ActionResult Deny(int id)
		{
			using (var context = new TutoringContext())
			{
				Tutorship t = context.Tutorships
					.FirstOrDefault(x => x.TutorshipId == id);

				if (t == null)
					return HttpNotFound();

				t.Approved = false;
				t.Seeking = false;

				context.SaveChanges();
			}

			return RedirectToAction("Show", new { id = id });
		}

		[HttpGet]
		[AccessPolicy(AccessPermissions.TUTORING_APPROVE)]
		public ActionResult Approve(int tutorship_id, string user_id)
		{
			using (var context = new TutoringContext())
			{
				Tutorship t = context.Tutorships
					.Include(x => x.Tutor)
					.FirstOrDefault(x => x.TutorshipId == tutorship_id);

				Person p = context.People.FirstOrDefault(x => x.PucrsId == user_id);

				if (t == null || p == null || t.Approved)
					return HttpNotFound();

				t.Approved = true;
				t.Seeking = false;
				t.Tutor = p;

				context.SaveChanges();
			}

			return RedirectToAction("Show", new { id = tutorship_id });
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
				string usr_handle = User.Identity.GetUserName();
				Class c = context.Classes
					.Include("Professor")
					.FirstOrDefault(x => x.ClassId == model.Class.ClassId);

				if (c == null) return HttpNotFound();

				if (usr_handle != c.Professor.PucrsId)
				{
					ModelState.AddModelError("", "Only the professor of this class can submit requests.");
					return View(model);
				}

				if (context.Tutorships.Any(x => x.ClassId == model.Class.ClassId && x.Seeking))
				{
					ModelState.AddModelError("", "This class already has a tutorship request.");
					return View(model);
				}

				Tutorship t = new Tutorship();
				t.ClassId = model.Class.ClassId;
				t.About = model.About;
				t.Approved = false;
				t.Seeking = true;
				t.DateRequested = DateTime.Now;

				context.Tutorships.Add(t);
				context.SaveChanges();
			}
			return RedirectToAction("Index", new { class_id = model.Class.ClassId });
		}
    }
}