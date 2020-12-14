using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Controllers
{
	public class ErrorController : Controller
	{
		private readonly ILogger _logger;

		public ErrorController(ILogger<ErrorController> logger)
		{
			_logger = logger;
			_logger.LogDebug("注入Error控制器");
		}

		[Route(template: "Error/{statusCode}")]
		public IActionResult HttpStatusCodeHandler(int statusCode)
		{
			var statusCodeRes = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

			switch (statusCode)
			{
				case 404:
					ViewBag.ErrorMessage = "抱歉，您访问的页面不存在";
					_logger.LogWarning($"路径{statusCodeRes.OriginalPath}不存在");
					break;
			}

			return View(viewName: "RouteNotFound");
		}

		[AllowAnonymous]
		[Route("Error")]
		public IActionResult Error()
		{
			var exceptionHandler = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
			ViewBag.Message = exceptionHandler.Error.Message;

			_logger.LogError(message: $"错误路径：{exceptionHandler.Path}，发生了一个错误信息{exceptionHandler.Error.Message}。错误堆栈信息为：{exceptionHandler.Error.StackTrace}");

			return View();
		}
	}
}
