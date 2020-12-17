using Microsoft.AspNetCore.Mvc;
using StudentManagement_Tools.CustomUtil;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.ViewModels.IdentityViewModel
{
	public class RegistryViewModel
	{
		[Required(ErrorMessage ="邮箱不得为空")]
		[Display(Name ="邮箱")]
		[EmailAddress(ErrorMessage ="邮箱地址不合法")]
		[DataType(DataType.EmailAddress)]
		[Remote(action: "IsEmailInUse", controller: "Account")]
		[ValidEmailDomain(allowdomain:"edu.com",ErrorMessage ="注册邮箱必须以edu.com为后缀")]
		public string Email { get; set; }

		[Required(ErrorMessage ="密码不得为空")]
		[Display(Name ="密码")]
		[DataType(DataType.Password)]
		public string PassWord { get; set; }

		[Required(ErrorMessage ="确认密码不得为空")]
		[DataType(DataType.Password)]
		[Display(Name ="确认密码")]
		[Compare(otherProperty:"PassWord",ErrorMessage ="两次密码不一致")]
		public string ConfirmPassWord { get; set; }

		[Display(Name ="城市")]
		[DataType(DataType.Text)]
		public string City { get; set; }
	}
}
