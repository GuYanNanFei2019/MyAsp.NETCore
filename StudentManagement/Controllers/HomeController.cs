using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StudentManagement_DataBase.EFModel.StudentModel;
using StudentManagement_Repository.Student;
using System;
using System.IO;
using StudentManagement.ViewModels.StudentsViewModel;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace StudentManagement.Controllers
{
	[Route("[controller]/[action]")]
	[Authorize(Roles = "Admin,Custom")]
	public class HomeController : Controller
	{
		private readonly IStudentRepository _studentRepository;
		private readonly IWebHostEnvironment _environment;
		private readonly ILogger _logger;

		/// <summary>
		/// 构造函数注入
		/// </summary>
		/// <param name="studentRepository"></param>
		public HomeController(IStudentRepository studentRepository, IWebHostEnvironment environment, ILogger<HomeController> logger)
		{
			_studentRepository = studentRepository;
			_environment = environment;
			_logger = logger;
			_logger.LogDebug("NLog注入Home控制器");
		}

		#region 学生管理

		/// <summary>
		/// 学生列表
		/// </summary>
		/// <returns></returns>
		[Route("")]
		[Route("~/")]
		[Route("~/Home")]
		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> Index()
		{
			return View(await _studentRepository.GetAllStudents());
		}

		/// <summary>
		/// 学生详情
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[Route("{id?}")]
		[HttpGet]
		public async Task<IActionResult> Detail(int id)
		{
			Students students =await _studentRepository.GetStudent(id);

			if (students == null)
			{
				_logger.LogError($"Home控制器中Detail方法ID为{id}的学生不存在");
				Response.StatusCode = 404;
				return View(viewName: "~/Views/Error/StudentNotFound.cshtml", model: id);
			}

			HomeDetailViewModel homeDetailViewModel = new HomeDetailViewModel
			{
				Students =await _studentRepository.GetStudent(id),
				PageTitle = "学生详情信息"
			};

			return View(homeDetailViewModel);
		}

		/// <summary>
		/// 新增学生视图
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Authorize(Roles ="Admin")]
		public IActionResult Create()
		{
			return View();
		}

		/// <summary>
		/// 新增学生
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Create(StudentCreateViewModel viewModel)
		{
			if (ModelState.IsValid)
			{
				string uniqueName = null;
				if (viewModel.PhotoPath != null && viewModel.PhotoPath.Count > 0)
				{
					uniqueName = CreateStudentImage(viewModel);
				}

				Students students = new Students
				{
					Name = viewModel.Name,
					ClassName = viewModel.ClassName,
					Email = viewModel.Email,
					PhotoPath = uniqueName
				};

				await _studentRepository.AddStudent(students);

				return RedirectToAction(actionName: "Detail", routeValues: new { id = students.ID });
			}

			return View();
		}


		/// <summary>
		/// 编辑学生视图
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Edit(int id)
		{
			Students students =await _studentRepository.GetStudent(id);

			if (students != null)
			{
				var viewModel = new StudentEditViewModel
				{
					id = students.ID,
					Name = students.Name,
					Email = students.Email,
					ClassName = students.ClassName,
					ExistingPhotoPath = students.PhotoPath
				};
				return View(viewModel);
			}
			else
			{
				_logger.LogError($"Home控制器中Edit方法ID为{id}的学生不存在");
				Response.StatusCode = 404;
				return View(viewName: "~/Views/Error/StudentNotFound.cshtml", model: id);
			}
		}

		/// <summary>
		/// 编辑学生
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Edit(StudentEditViewModel viewModel)
		{
			if (ModelState.IsValid)
			{
				Students students =await _studentRepository.GetStudent(viewModel.id);
				students.Name = viewModel.Name;
				students.Email = viewModel.Email;
				students.ClassName = viewModel.ClassName;

				if (viewModel.PhotoPath != null
					&& viewModel.PhotoPath.Count > 0)
				{
					if (viewModel.ExistingPhotoPath != null)
					{
						var filePath = Path.Combine(path1: _environment.WebRootPath,
								  path2: "images",
								  path3: viewModel.ExistingPhotoPath);

						System.IO.File.Delete(filePath);

					}

					string uniqueName = CreateStudentImage(viewModel);
					students.PhotoPath = uniqueName;
				}

				await _studentRepository.UpdateStudent(students);

				return RedirectToAction(actionName: "Detail",
							routeValues: new { id = students.ID });
			}
			return View(viewModel);
		}

		/// <summary>
		/// 删除学生视图
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Delete(int id)
		{
			Students students = await _studentRepository.GetStudent(id);

			if (students == null)
			{
				_logger.LogError($"Home控制器中Delete方法ID为{id}的学生不存在");

				Response.StatusCode = 404;

				return View(
					viewName: "~/Views/Error/StudentNotFound.cshtml", model: id);
			}

			return View(students);
		}

		/// <summary>
		/// 删除学生
		/// </summary>
		/// <param name="students"></param>
		/// <returns></returns>
		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Delete(Students students)
		{
			if (students.PhotoPath != null)
			{
				var filePath = Path.Combine(path1: _environment.WebRootPath,
							  path2: "images",
							  path3: students.PhotoPath);

				System.IO.File.Delete(filePath);
			}

			await _studentRepository.DeleteStudent(students.ID);

			return RedirectToAction("Index");
		}

		#endregion

		/// <summary>
		/// 检查新增学生和编辑学生信息时邮箱唯一性
		/// </summary>
		/// <param name="email"></param>
		/// <returns></returns>
		[AcceptVerbs("Get","Post")]
		public async Task<IActionResult> CheckEmailUnique(string email) 
		{
			var res = await _studentRepository.CheckEmailUnique(email);
			if (res)
			{
				return Json(true);
			}
			else
			{
				return Json($"对不起，邮箱{email}已被使用，请更换邮箱");
			}
		}

		/// <summary>
		/// 将图片保存到指定路径并返回文件名
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		private string CreateStudentImage(StudentCreateViewModel viewModel)
		{
			var uniqueName = "";

			if (viewModel.PhotoPath.Count > 0)
			{
				foreach (var item in viewModel.PhotoPath)
				{
					string upLoadFolder = Path.Combine(path1: _environment.WebRootPath,
									 path2: "images");

					uniqueName = Guid.NewGuid().ToString() + "_" + item.FileName;

					string filePath = Path.Combine(path1: upLoadFolder,
								 path2: uniqueName);

					using (var fileStream = new FileStream(path: filePath, mode: FileMode.Create))
					{
						item.CopyTo(fileStream);
					}
				}
			}
			return uniqueName;
		}
	}
}
