using StudentManagement_DataBase.EFModel.StudentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.ViewModels.StudentsViewModel
{
	public class HomeDetailViewModel
	{
		public Students Students { get; set; }

		public string PageTitle { get; set; }
	}
}
