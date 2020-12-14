using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StudentManagement_DataBase.EFModel;
using StudentManagement_Repository.Student;
using System;
using System.IO;
using StudentManagement.ViewModels.StudentsViewModel;

namespace StudentManagement.Controllers
{
	[Route("[controller]/[action]")]
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

		/// <summary>
		/// 学生列表
		/// </summary>
		/// <returns></returns>
		[Route("")]
		[Route("~/")]
		[Route("~/Home")]
		[HttpGet]
		public IActionResult Index()
		{
			return View(_studentRepository.GetAllStudents());
		}

		/// <summary>
		/// 学生详情
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[Route("{id?}")]
		[HttpGet]
		public IActionResult Detail(int id)
		{
			Students students = _studentRepository.GetStudent(id);

			if (students == null)
			{
				_logger.LogError($"Home控制器中Detail方法ID为{id}的学生不存在");
				Response.StatusCode = 404;
				return View(viewName: "~/Views/Error/StudentNotFound.cshtml", model: id);
			}

			HomeDetailViewModel homeDetailViewModel = new HomeDetailViewModel
			{
				Students = _studentRepository.GetStudent(id),
				PageTitle = "学生详情信息"
			};

			return View(homeDetailViewModel);
		}

		/// <summary>
		/// 新增学生视图
		/// </summary>
		/// <returns></returns>
		[HttpGet]
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
		public IActionResult Create(StudentCreateViewModel viewModel)
		{
			if (ModelState.IsValid)
			{
				bool emailres = _studentRepository.CheckEmailUnique(viewModel.Email);

				if (!emailres)
				{
					ViewBag.EmailRepeat = "对不起，邮箱不得重复，请更换其他邮箱";
					return View();
				}

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

				_studentRepository.AddStudent(students);

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
		public IActionResult Edit(int id)
		{
			Students students = _studentRepository.GetStudent(id);

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
		public IActionResult Edit(StudentEditViewModel viewModel)
		{
			if (ModelState.IsValid)
			{
				bool emailres = _studentRepository.CheckEmailUnique(viewModel.Email);

				if (!emailres)
				{
					ViewBag.EmailRepeat = "对不起，邮箱不得重复，请更换其他邮箱";
					return View(viewModel);
				}

				Students students = _studentRepository.GetStudent(viewModel.id);
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

				_studentRepository.UpdateStudent(students);

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
		public IActionResult Delete(int id)
		{
			Students students = _studentRepository.GetStudent(id);

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
		public IActionResult Delete(Students students)
		{
			if (students.PhotoPath != null)
			{
				var filePath = Path.Combine(path1: _environment.WebRootPath,
							  path2: "images",
							  path3: students.PhotoPath);

				System.IO.File.Delete(filePath);
			}

			_studentRepository.DeleteStudent(students.ID);

			return RedirectToAction("Index");
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
