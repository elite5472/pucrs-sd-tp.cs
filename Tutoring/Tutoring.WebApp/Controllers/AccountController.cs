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
using Tutoring.WebApp.Models.Account;
using Tutoring.WebApp.BusinessRules;
using System.Security;

namespace Tutoring.WebApp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

		[AccessPolicy(AccessPermissions.ACCOUNT_SHOW)]
		public ActionResult Show(string id)
		{
			ShowViewModel model = new ShowViewModel();

			using(var context = new TutoringContext())
			{
				model.Person = context.People.FirstOrDefault(x => x.PucrsId == id);
				if (model.Person == null)
					return HttpNotFound();
			}

			return View(model);
		}

		[HttpGet]
		[AccessPolicy(AccessPermissions.ACCOUNT_MANAGE)]
		public ActionResult Manage()
		{
			ManageViewModel model = new ManageViewModel();
			string usr_handle = User.Identity.GetUserName();

			using(var context = new TutoringContext())
			{
				Person user = context.People.First(x => x.PucrsId == usr_handle);
				model.Users = context.People
					.ToList()
					.Where(x => UserTypeAccessLevel.GetAccessLevel(x.UserType) < UserTypeAccessLevel.GetAccessLevel(user.UserType))
					.OrderBy(x => x.UserType)
					.ThenBy(x => x.Name)
					.ToList();

				model.UserTypes = UserTypeAccessLevel.UserTypes
					.Where(x => x.Value < UserTypeAccessLevel.GetAccessLevel(user.UserType))
					.Select(x => x.Key)
					.ToList();
			}


			return View(model);
		}

		[HttpPost]
		[AccessPolicy(AccessPermissions.ACCOUNT_MANAGE)]
		public ActionResult Manage(ManageViewModel model)
		{
			using(var context = new TutoringContext())
			{
				string usr_handle = User.Identity.GetUserName();
				Person user = context.People.First(x => x.PucrsId == usr_handle);
				var usr_types = model.Users.ToDictionary(k => k.PucrsId, v => v.UserType);
				foreach (var usr_type in usr_types)
				{
					Person target = context.People.FirstOrDefault(x => x.PucrsId == usr_type.Key);
					if (target == null) continue;

					if (UserTypeAccessLevel.GetAccessLevel(target.UserType) >= UserTypeAccessLevel.GetAccessLevel(user.UserType))
						throw new SecurityException();

					if (usr_type.Value != target.UserType)
					{
						target.UserType = usr_type.Value;
					}
				}
				context.SaveChanges();
			}
			return Manage();
		}

		

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.PucrsId, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
			string msg = "Create a new " + UserTypeAccessLevel.DefaultRegisterType + " account.";
			int highest_access = UserTypeAccessLevel.UserTypes.Max(x => x.Value);
			string highest_type = UserTypeAccessLevel.UserTypes.Where(x => x.Value == highest_access).First().Key;
			using (var context = new ApplicationDbContext())
			{
				if (context.Users.Count() == 0)
				{
					msg = "Create a " + highest_type + " account to set up the application.";
				}
			}

			RegisterViewModel model = new RegisterViewModel() { Message = msg };
			
            return View(model);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
			string usr_type = UserTypeAccessLevel.DefaultRegisterType;
			using (var context = new ApplicationDbContext())
			{
				if (context.Users.Count() == 0)
				{
					int highest_access = UserTypeAccessLevel.UserTypes.Max(x => x.Value);
					string highest_type = UserTypeAccessLevel.UserTypes.Where(x => x.Value == highest_access).First().Key;
					usr_type = highest_type;
				}
			}

            if (ModelState.IsValid)
            {
				Person person = new Person
				{
					Name = model.Name,
					PucrsId = model.PucrsId,
					UserType = usr_type,
					Email = model.Email
				};
                var user = new ApplicationUser
				{ 
					UserName = model.PucrsId, 
					Email = model.Email,
				};

                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

					using(var context = new TutoringContext())
					{
						context.People.Add(person);
						context.SaveChanges();
					}

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}