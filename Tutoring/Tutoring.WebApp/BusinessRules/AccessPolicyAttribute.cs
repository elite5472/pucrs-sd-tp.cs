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

namespace Tutoring.WebApp.BusinessRules
{
	[AttributeUsage(AttributeTargets.Method)]
	public class AccessPolicyAttribute : AuthorizeAttribute
	{
		private int level = 0;
		private string type = null;

		public AccessPolicyAttribute(int level)  
		{  
			this.level = level;  
		}

		public AccessPolicyAttribute(string type)
		{
			this.type = type;
		}  

		protected override bool AuthorizeCore(HttpContextBase httpContext)  
		{    
			using(var context = new TutoringContext())
			{
				string user_id = httpContext.User.Identity.GetUserName();
				Person user = context.People.Where(x => x.PucrsId == user_id).FirstOrDefault();
				if (user == null) return false;

				if (type != null)
					return user.UserType == type;
				else
					return UserTypeAccessLevel.GetAccessLevel(user.UserType) >= level;
			}  
		} 

		protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)  
		{  
			filterContext.Result = new HttpUnauthorizedResult();  
		}  
	}
}