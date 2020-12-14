using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.ViewModels.StudentsViewModel
{
	public class StudentEditViewModel:StudentCreateViewModel
	{
		public int id { get; set; }

		public string ExistingPhotoPath { get; set; }
	}
}
