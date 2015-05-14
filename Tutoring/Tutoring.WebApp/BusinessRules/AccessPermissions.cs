using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tutoring.WebApp.BusinessRules
{
	public class AccessPermissions
	{
		public const int TUTORING_INDEX = 1;
		public const string TUTORING_APPLY_TYPE = "Student";
		public const string TUTORING_SUBSCRIBE_TYPE = "Student";
		public const int TUTORING_PETITION = 2;
		public const int TUTORING_APPROVE = 4;
		public const int TUTORING_DENY = 4;
		public const int CLASS_INDEX = 1;
		public const int CLASS_EDIT = 3;
		public const int ACCOUNT_SHOW = 1;
		public const int ACCOUNT_MANAGE = 3;
	}
}