using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tutoring.WebApp.Models.Account
{
	public class ShowViewModel
	{
		public Person Person { get; set; }
	}

	public class ManageViewModel
	{
		public List<Person> Users { get; set; }
		public List<string> UserTypes { get; set; }

		public ManageViewModel()
		{
			Users = new List<Person>();
			UserTypes = new List<string>();
		}
	}

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "PUCRS ID")]
        public string PucrsId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
		public string Message { get; set; }

		[Required]
		[Display(Name = "Name")]
		public string Name { get; set; }

		[Required]
		[Display(Name = "PUCRS ID")]
		public string PucrsId { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
