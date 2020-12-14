using Microsoft.EntityFrameworkCore;
using StudentManagement_DataBase.EFModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement_DataBase.ModelExtensions
{
	public static class ModelBuilderExtensions
	{
		public static void Seed(this ModelBuilder modelBuilder) 
		{
			modelBuilder.Entity<Students>().HasData(
				data: new Students
				{
					ID = 1,
					Name = "朱超",
					ClassName = ClassNameEnum.FourthGrade,
					Email = "MoshangPengyou@hotmail.com",
					PhotoPath=null
				}
				);
		}
	}
}
