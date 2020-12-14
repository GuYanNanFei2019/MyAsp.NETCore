using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentManagement_DataBase.ModelExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement_DataBase.EFModel
{
	public class StudentDbContext : IdentityDbContext
	{
		public StudentDbContext(DbContextOptions<StudentDbContext> options) : base(options)
		{

		}

		public DbSet<Students> Students { get; set; }


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Seed();
		}
	}
}
