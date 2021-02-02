using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StudentManagement_DataBase.EFModel.IdentityModel;
using StudentManagement_DataBase.EFModel.StudentModel;
using StudentManagement_DataBase.ModelExtensions;
using StudentManagement_Repository.Student;
using StudentManagement_Tools.MiddleWare;
using StudentManagement_Tools.Security;

namespace StudentManagement
{
	public class Startup
	{
		private readonly IConfiguration _configuration;

		public Startup(IConfiguration configuration)
		{
			_configuration = configuration;
		}


		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllersWithViews(config=> 
			{
				var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
												 .Build();

				config.Filters.Add(new AuthorizeFilter(policy));


			}).AddNewtonsoftJson();

			services.AddRazorPages();

			services.AddScoped<IStudentRepository, SqlServerStudentRepository>();

			services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler> ();

			services.AddDbContextPool<StudentDbContext>(options => options.UseSqlServer(connectionString: _configuration.GetConnectionString("StudentConnection")));

			services.AddIdentity<ApplicationUser, IdentityRole>()
				.AddErrorDescriber<CustomIdentityErrorDesc>()
				.AddEntityFrameworkStores<StudentDbContext>();

			services.Configure<IdentityOptions>(options=> 
			{
				options.Password.RequiredLength = 6;
				options.Password.RequireLowercase = true;
				options.Password.RequireUppercase = true;
				options.Password.RequireNonAlphanumeric = false;
			});

			services.AddHttpContextAccessor(); ;

			services.AddAuthorization(options => {
				options.AddPolicy(
					name: "DeleteRolePolicy",
					configurePolicy: policy => policy.RequireClaim(claimType: "Delete Role")
					);

				options.AddPolicy(
					name: "CreateRolePolicy",
					configurePolicy: policy => policy.RequireClaim(claimType: "Create Role"));

				options.AddPolicy(
					name: "EditRolePolicy",
					configurePolicy: policy => policy.AddRequirements(new ManageAdminInRolesAndClaimsRequirement()
					));


				options.AddPolicy(
					name: "AdminPolicy",
					configurePolicy: policy => policy.RequireRole(roles: "Admin"));
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}else if (env.IsStaging() || env.IsProduction()) 
			{
				app.UseExceptionHandler("/Error");
				app.UseStatusCodePagesWithReExecute(pathFormat: "/Error/{0}");
			}
			

			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();

			app.UseAuthorization();

			app.UseDataInitiaizer();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
