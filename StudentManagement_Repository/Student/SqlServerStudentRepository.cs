using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using StudentManagement_DataBase.EFModel.StudentModel;
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

		public async Task<Students> AddStudent(Students students)
		{
			await _dbcontext.AddAsync(students);
			await _dbcontext.SaveChangesAsync();

			return students;
		}

		public async Task<Students> DeleteStudent(int id)
		{
			if (await _dbcontext.Students.FindAsync(id) != null)
			{
				_dbcontext.Students.Remove(_dbcontext.Students.Find(id));
				await _dbcontext.SaveChangesAsync();
			}

			return await _dbcontext.Students.FindAsync(id);
		}

		public async Task<IEnumerable<Students>> GetAllStudents() => await _dbcontext.Students.ToListAsync();

		public async Task<Students> GetStudent(int id) => await _dbcontext.Students.FindAsync(id);

		public async Task<Students> GetStudentsByEmail(string email) => await _dbcontext.Students.Where(s => s.Email.Equals(email)).FirstOrDefaultAsync();

		public async Task<Students> UpdateStudent(Students students)
		{
			_dbcontext.Students.Attach(students).State = EntityState.Modified;

			await _dbcontext.SaveChangesAsync();

			return students;
		}

		/// <summary>
		/// 检查学生注册或更新信息时邮箱唯一性
		/// </summary>
		/// <param name="email"></param>
		/// <returns></returns>
		public async Task<bool> CheckEmailUnique(string email) 
		{
			if (!string.IsNullOrWhiteSpace(email))
			{
				if (await GetStudentsByEmail(email)!=null)
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
