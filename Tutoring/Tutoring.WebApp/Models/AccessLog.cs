using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tutoring.WebApp.Models
{
	public class AccessLog
	{
		public int AccessLogId { get; set; }
		public int PersonId { get; set; }
		public Person Person { get; set; }
		public DateTime AccessDate { get; set; }
	}
}