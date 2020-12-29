using StudentManagement_DataBase.EFModel.IdentityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.ViewModels.IdentityViewModel
{
    public class UserClaimViewModel
    {
		public UserClaimViewModel()
		{
			UserClaims = new List<UserClaim>();
		}

		public string UserId { get; set; }

		public List<UserClaim> UserClaims { get; set; }
	}
}
