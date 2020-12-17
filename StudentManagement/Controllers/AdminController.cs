using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using StudentManagement.ViewModels.IdentityViewModel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Controllers
{
	public class AdminController : Controller
	{
		private readonly RoleManager<IdentityRole> _roleManager;

		public AdminController(RoleManager<IdentityRole> roleManager) => _roleManager = roleManager;

		[HttpGet]
		public IActionResult CreateRole()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> CreateRole(CreateRoleViewModel viewModel)
		{
			if (ModelState.IsValid)
			{
				var role = new IdentityRole
				{
					Name = viewModel.RoleName
				};

				var res = await _roleManager.CreateAsync(role);
				if (res.Succeeded)
				{
					return RedirectToAction("Index", "Home");
				}

				foreach (var item in res.Errors)
				{
					ModelState.AddModelError(string.Empty, item.Description);
				}
			}

			return View(viewModel);
		}
	}
}
