using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tutoring.WebApp.BusinessRules
{
	public class UserTypeAccessLevel
	{
		private static Dictionary<string, int> _UserTypes = new Dictionary<string, int>
		{
			{"Student", 1},
			{"Professor", 2},
			{"Staff", 3},
			{"Director", 4}
		};

		public static Dictionary<string, int> UserTypes
		{
			get
			{
				return _UserTypes;
			}
		}

		public static int GetAccessLevel(string user_type)
		{
			if (UserTypes.ContainsKey(user_type))
				return UserTypes[user_type];
			else
				return 0;

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