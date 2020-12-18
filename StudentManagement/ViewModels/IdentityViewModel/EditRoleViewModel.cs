using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.ViewModels.IdentityViewModel
{
	public class EditRoleViewModel
	{
		public EditRoleViewModel()
		{
			Users =new List<string>();
		}

		[Display(Name ="角色ID")]
		public string Id { get; set; }

		[Required(ErrorMessage ="角色名称不得为空")]
		[Display(Name ="角色名称")]
		public string RoleName { get; set; }

		[Display(Name ="该角色中的用户")]
		public List<string> Users { get; set; }
	}
}
