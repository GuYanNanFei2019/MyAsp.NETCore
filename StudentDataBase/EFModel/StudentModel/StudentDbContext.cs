using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentManagement_DataBase.EFModel.IdentityModel;
using StudentManagement_DataBase.ModelExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement_DataBase.EFModel.StudentModel
{
	public class StudentDbContext : IdentityDbContext<ApplicationUser>
	{
		public StudentDbContext(DbContextOptions<StudentDbContext> options) : base(options)
		{

		}

		public DbSet<Students> Students { get; set; }


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			foreach (var item in modelBuilder.Model.GetEntityTypes().SelectMany
				(
				selector: s => s.GetForeignKeys()
				))
			{
				item.DeleteBehavior = DeleteBehavior.Restrict;
			}
		}
	}
}
