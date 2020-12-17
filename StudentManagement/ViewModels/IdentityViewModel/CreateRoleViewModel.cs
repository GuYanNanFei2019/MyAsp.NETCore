using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.ViewModels.IdentityViewModel
{
	public class CreateRoleViewModel
	{
		[Required(ErrorMessage ="角色名不能为空")]
		[Display(Name ="角色名")]
		public string RoleName { get; set; }
	}
}
