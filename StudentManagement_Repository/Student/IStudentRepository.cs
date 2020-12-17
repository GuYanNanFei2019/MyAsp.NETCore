using StudentManagement_DataBase.EFModel.StudentModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement_Repository.Student
{
	public interface IStudentRepository
	{
		/// <summary>
		/// 根据ID获取学生
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		Task<Students> GetStudent(int id);

		/// <summary>
		/// 获取学生列表
		/// </summary>
		/// <returns></returns>
		Task<IEnumerable<Students>> GetAllStudents();

		/// <summary>
		/// 根据EMAIL获取学生
		/// </summary>
		/// <param name="email"></param>
		/// <returns></returns>
		Task<Students> GetStudentsByEmail(string email);

		/// <summary>
		/// 新增学生
		/// </summary>
		/// <param name="students"></param>
		/// <returns></returns>
		Task<Students> AddStudent(Students students);

		/// <summary>
		/// 修改学生信息
		/// </summary>
		/// <param name="students"></param>
		/// <returns></returns>
		Task<Students> UpdateStudent(Students students);

		/// <summary>
		/// 删除学生
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		Task<Students> DeleteStudent(int id);

		/// <summary>
		/// 检查学生注册或更新信息时邮箱唯一性
		/// </summary>
		/// <param name="email"></param>
		/// <returns></returns>
		Task<bool> CheckEmailUnique(string email);
	}
}
