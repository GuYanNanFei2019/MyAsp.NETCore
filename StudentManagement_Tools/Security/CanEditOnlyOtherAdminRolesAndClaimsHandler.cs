using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement_Tools.Security
{
	/// <summary>
	/// 只能编辑其他Admin角色和声明的处理程序
	/// </summary>
	public class CanEditOnlyOtherAdminRolesAndClaimsHandler : AuthorizationHandler<ManageAdminInRolesAndClaimsRequirement>
	{
		private readonly IHttpContextAccessor _contextAccessor;

		public CanEditOnlyOtherAdminRolesAndClaimsHandler(IHttpContextAccessor contextAccessor)
		{
			_contextAccessor = contextAccessor;
		}

		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageAdminInRolesAndClaimsRequirement requirement)
		{
			//获取http上下文
			HttpContext httpContext = _contextAccessor.HttpContext;

			string loggedInAdminId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

			string adminIdBeingEdited = _contextAccessor.HttpContext.Request.Query["userId"];

			//判断用户是否是Admin角色，并且Edit Role声明为true
			if (context.User.IsInRole("Admin")
	   && context.User.HasClaim(claim => claim.Type == "Edit Role"
									  && claim.Value == "true"))
			{
				//如果当前Admin角色的UserId为空，则进入的是角色列表，无需判断当前用户ID
				if (string.IsNullOrWhiteSpace(adminIdBeingEdited))
				{
					context.Succeed(requirement);
				}else if(adminIdBeingEdited.ToLower() != loggedInAdminId.ToLower()) 
				{
					context.Succeed(requirement);
				}
			}
			return Task.CompletedTask;
		}
	}
}
