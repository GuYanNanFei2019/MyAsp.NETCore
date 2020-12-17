using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement_DataBase.EFModel.IdentityModel
{
	public class ApplicationUser:IdentityUser
	{
		public string City { get; set; }
	}
}
