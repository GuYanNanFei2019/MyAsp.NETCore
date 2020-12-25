using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.ViewModels.IdentityViewModel
{
	public class EditUserViewModel
	{
		public EditUserViewModel()
		{
			Claims = new List<string>();
			Roles = new List<string>();
		}

		public string ID { get; set; }

		[Required(ErrorMessage ="用户名不得为空")]
		[Display(Name ="用户名")]
		[DataType(DataType.Text)]
		public string UserName { get; set; }

		[Required(ErrorMessage ="邮箱不得为空")]
		[EmailAddress(ErrorMessage ="邮箱地址不合法")]
		[DataType(DataType.EmailAddress)]
		[Display(Name ="邮箱")]
		public string Email { get; set; }

		[Display(Name ="角色")]
		public IList<string> Roles { get; set; }

		[Display(Name ="声明")]
		public List<string> Claims { get; set; }

		[Display(Name ="城市")]
		public string City { get; set; }
	}
}
