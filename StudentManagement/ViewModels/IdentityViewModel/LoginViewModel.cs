using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.ViewModels.IdentityViewModel
{
	public class LoginViewModel
	{
		[Required(ErrorMessage ="邮箱不得为空")]
		[Display(Name ="邮箱")]
		[DataType(DataType.EmailAddress)]
		[EmailAddress(ErrorMessage ="邮箱地址不合法")]
		public string Email { get; set; }

		[Required(ErrorMessage ="密码不得为空")]
		[Display(Name ="密码")]
		[DataType(DataType.Password)]
		public string PassWord { get; set; }

		[Display(Name ="记住我")]
		public bool RememberMe { get; set; }
	}
}
