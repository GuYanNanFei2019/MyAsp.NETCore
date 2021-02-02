using Microsoft.AspNetCore.Authorization;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement_Tools.Security
{
	public class SuperAdmin_Handler : AuthorizationHandler<ManageAdminInRolesAndClaimsRequirement>
	{


		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageAdminInRolesAndClaimsRequirement requirement)
		{
			if (context.User.IsInRole("Super Admin"))
			{
				context.Succeed(requirement);
			}

			return Task.CompletedTask;
		}
	}
}
