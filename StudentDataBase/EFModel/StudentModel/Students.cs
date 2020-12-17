using StudentManagement_DataBase.ModelExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement_DataBase.EFModel.StudentModel
{
	public class Students
	{
		public int ID { get; set; }

		[Required(ErrorMessage ="请输入姓名"),MaxLength(10,ErrorMessage ="姓名长度不得超过10个字符")]
		[Display(Name ="姓名")]
		[DataType(DataType.Text)]
		public string Name { get; set; }

		[Required(ErrorMessage ="请选择学生班级")]
		[Display(Name ="班级")]
		public ClassNameEnum? ClassName { get; set; }

		[Required(ErrorMessage ="请输入邮箱")]
		[Display(Name ="邮箱")]
		[EmailAddress(ErrorMessage ="邮箱地址非法")]
		[DataType(DataType.EmailAddress)]
		public string Email { get; set; }

		[Display(Name="头像")]
		[DataType(DataType.ImageUrl)]
		public string PhotoPath { get; set; }
	}
}
