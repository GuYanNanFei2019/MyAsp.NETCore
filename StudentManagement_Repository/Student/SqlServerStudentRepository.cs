using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using StudentManagement_DataBase.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement_Repository.Student
{
	public class SqlServerStudentRepository : IStudentRepository
	{
		private readonly StudentDbContext _dbcontext;
		private readonly ILogger _logger;

		public SqlServerStudentRepository(StudentDbContext dbContext,ILogger<SqlServerStudentRepository> logger)
		{
			_dbcontext = dbContext;
			_logger = logger;
		}

		public Students AddStudent(Students students)
		{
			_dbcontext.Add(students);
			_dbcontext.SaveChanges();

			return students;
		}

		public Students DeleteStudent(int id)
		{
			if (_dbcontext.Students.Find(id) != null)
			{
				_dbcontext.Students.Remove(_dbcontext.Students.Find(id));
				_dbcontext.SaveChanges();
			}

			return _dbcontext.Students.Find(id);
		}

		public IEnumerable<Students> GetAllStudents() => _dbcontext.Students.ToList();

		public Students GetStudent(int id) => _dbcontext.Students.Find(id);

		public Students GetStudentsByEmail(string email) => _dbcontext.Students.Where(s => s.Email.Equals(email)).FirstOrDefault();

		public Students UpdateStudent(Students students)
		{
			_dbcontext.Students.Attach(students).State = EntityState.Modified;

			_dbcontext.SaveChanges();

			return students;
		}

		/// <summary>
		/// 检查学生注册或更新信息时邮箱唯一性
		/// </summary>
		/// <param name="email"></param>
		/// <returns></returns>
		public bool CheckEmailUnique(string email) 
		{
			if (!string.IsNullOrWhiteSpace(email))
			{
				if (GetStudentsByEmail(email)!=null)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
			else
			{
				return false;
			}
		}
	}
}
