using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tutoring.WebApp.Models;

namespace Tutoring.WebApp.BusinessRules
{
	public class TutorshipRules
	{
		public static string GetStatus(Tutorship t)
		{
			if (t.Approved)
				return "Approved";
			if (t.Seeking)
				return "Requested";
			else
				return "Denied";
		}

		public static string FormatAbout(string s)
		{
			return  "<p>" + s
				.Replace("\r\n", "</p><p>")
				.Replace("\n", "</p><p>")
				.Replace("\r", "</p><p>")
				.Replace("<p></p>", "<br />")
				+ "</p>";
		}

		public static string ShortAbout(string s)
		{
			s = s
				.Replace("\r\n", " ")
				.Replace("\n", " ")
				.Replace("\r", " ");
			string s1 = s.Substring(0, Math.Min(64, s.Length));
			if (s1.Length != s.Length)
				return s1 + "...";
			else
				return s1;
		}
	}
}