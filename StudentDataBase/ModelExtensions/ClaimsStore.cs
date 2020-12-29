using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement_DataBase.ModelExtensions
{
    public class ClaimsStore
    {
        public static List<Claim> AllClaim = new List<Claim>()
        {
            new Claim("Create Role","Create Role"),
            new Claim("Edit Role","Edit Role"),
            new Claim("Delete Role","Delete Role"),
            new Claim("EditStudent","EditStudent"),
        };
    }
}
