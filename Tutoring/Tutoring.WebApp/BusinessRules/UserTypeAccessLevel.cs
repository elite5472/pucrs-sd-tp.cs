using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tutoring.WebApp.BusinessRules
{
	public class UserTypeAccessLevel
	{
		public static int GetAccessLevel(string user_type)
		{
			switch(user_type)
			{
				case "Student": return 1;
				case "Professor": return 2;
				case "Staff": return 3;
				case "Director": return 4;
				default: return 0;
			}
		}

		public static bool CheckPromoteRights(string user_type, string target_type)
		{
			int user_level = GetAccessLevel(user_type);
			int target_level = GetAccessLevel(target_type);

			if (user_level < 4) return false;
			if (user_level <= target_level) return false;

			return true;
		}

		public static string DefaultRegisterType
		{
			get
			{
				return "Student";
			}
		}

		public static List<string> GetPromoteTypesForUser(string user_type)
		{
			List<string> result = new List<string>();
			int level = GetAccessLevel(user_type);
			if(level >= 4)
			{
				result.Add("Student");
				result.Add("Professor");
			}
			if(level >= 5)
			{
				result.Add("Staff");
				result.Add("Director");
			}
			return result;
		}
	}
}