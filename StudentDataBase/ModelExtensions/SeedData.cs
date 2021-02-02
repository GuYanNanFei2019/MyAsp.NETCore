using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using StudentManagement_DataBase.EFModel.IdentityModel;
using StudentManagement_DataBase.EFModel.StudentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StudentManagement_DataBase.ModelExtensions
{
	public static class SeedData
	{
		public static IApplicationBuilder UseDataInitiaizer(this IApplicationBuilder builder) 
		{
			using (var scope=builder.ApplicationServices.CreateScope())
			{
				var dbcontext = scope.ServiceProvider.GetService<StudentDbContext>();
				var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();

				System.Console.WriteLine("开始迁移数据库....");

				dbcontext.Database.Migrate();

				System.Console.WriteLine("数据库迁移完成...");

				#region 迁移学生数据

				if (!dbcontext.Students.Any())
				{
					System.Console.WriteLine("开始创建学生种子数据....");
					dbcontext.Students.Add(new Students
					{
						ID = 1,
						Name = "朱超",
						ClassName = ClassNameEnum.FourthGrade,
						Email = "MoshangPengyou@hotmail.com",
						PhotoPath = null
					});
				}
				else
				{
					System.Console.WriteLine("无需创建学生种子数据");
				}
				#endregion

				#region 初始化管理用户
				//查询数据库中是否有管理用户
				var adminUser = dbcontext.Users.FirstOrDefault(u => u.UserName == "moshangpengyou@hotmail.com");

				if (adminUser==null)
				{
					//添加User信息
					var user = new ApplicationUser
					{
						UserName = "moshangpengyou@hotmail.com",
						Email = "moshangpengyou@hotmail.com",
						EmailConfirmed = true,
						City = "南京",

					};
					//创建USER
					var identityRes = userManager.CreateAsync(user: user, password: "O9wsNt@eo").GetAwaiter().GetResult();

					//添加角色信息
					var role = dbcontext.Roles.Add(new IdentityRole
					{
						Name = "Admin",
						NormalizedName="ADMIN"
					});

					dbcontext.SaveChanges();

					//添加角色和USER关系
					dbcontext.UserRoles.Add(new IdentityUserRole<string>
					{
						RoleId = role.Entity.Id,
						UserId = user.Id
					});

					var userClaims = new List<IdentityUserClaim<string>>();

					//添加USER声明
					userClaims.Add(new IdentityUserClaim<string>
					{
						UserId=user.Id,
						ClaimType="Create Role",
						ClaimValue= "Create Role"
					});

					userClaims.Add(new IdentityUserClaim<string>
					{
						UserId = user.Id,
						ClaimType = "Edit Role",
						ClaimValue = "Edit Role"
					});

					userClaims.Add(new IdentityUserClaim<string>
					{
						UserId = user.Id,
						ClaimType = "Delete Role",
						ClaimValue = "Delete Role"
					});

					userClaims.Add(new IdentityUserClaim<string>
					{
						UserId = user.Id,
						ClaimType = "EditStudent",
						ClaimValue = "EditStudent"
					});

					dbcontext.UserClaims.AddRange(userClaims);

					dbcontext.SaveChanges();
				}
				else
				{
					System.Console.WriteLine("无需创建种子数据....");
				}
				#endregion

				return builder;
			}
		}
	}
}
