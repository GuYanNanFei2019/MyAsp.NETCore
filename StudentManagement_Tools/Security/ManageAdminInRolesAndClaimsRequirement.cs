using Microsoft.AspNetCore.Authorization;

using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement_Tools.Security
{
	/// <summary>
	/// 管理Admin角色声明需求
	/// </summary>
	public class ManageAdminInRolesAndClaimsRequirement:IAuthorizationRequirement
	{
	}
}
