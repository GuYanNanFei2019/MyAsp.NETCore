using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.ViewModels.IdentityViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Controllers
{
	[AllowAnonymous]
	public class AccountController : Controller
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly SignInManager<IdentityUser> _signInManager;

		public AccountController(UserManager<IdentityUser> userManager,SignInManager<IdentityUser> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}

		/// <summary>
		/// 注册
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public IActionResult Register() 
		{
			return View();
		}

		/// <summary>
		/// 注册用户
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> Register(RegistryViewModel model) 
		{
			if (ModelState.IsValid)
			{
				var user = new IdentityUser
				{
					UserName = model.Email,
					Email = model.Email
				};

				var res = await _userManager.CreateAsync(user, model.PassWord);

				if (res.Succeeded)
				{
					await _signInManager.SignInAsync(user, isPersistent: false);

					return RedirectToAction("Index", "Home");
				}

				foreach (var item in res.Errors)
				{
					ModelState.AddModelError(string.Empty, item.Description);
				}
			}

			return View(model);
		}

		/// <summary>
		/// 注销
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> LogOut() 
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		public IActionResult Login() 
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel model) 
		{
			if (ModelState.IsValid)
			{
				var res = await _signInManager.PasswordSignInAsync(model.Email, model.PassWord, model.RememberMe, false);
				if (res.Succeeded)
				{
					return RedirectToAction("Index", "Home");
				}

				ModelState.AddModelError(string.Empty, "登录失败，请重试");
			}

			return View(model);
		}
	}
}
