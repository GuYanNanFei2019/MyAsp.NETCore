using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using StudentManagement.ViewModels.IdentityViewModel;

using StudentManagement_DataBase.EFModel.IdentityModel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Controllers
{
	[AllowAnonymous]
	public class AccountController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;

		public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
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
				var user = new ApplicationUser
				{
					UserName = model.Email,
					Email = model.Email,
					City=model.City
				};

				var res = await _userManager.CreateAsync(user, model.PassWord);

				if (res.Succeeded)
				{
					if (_signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
					{
						return RedirectToAction("ListUsers", "Admin");
					}

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
		public IActionResult Login(string ReturnUrl)
		{
			LoginViewModel model = new LoginViewModel
			{
				ReturnUrl = ReturnUrl
			};

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel model, string ReturnUrl)
		{
			if (ModelState.IsValid)
			{
				var res = await _signInManager.PasswordSignInAsync(model.Email, model.PassWord, model.RememberMe, false);
				if (res.Succeeded)
				{
					if (!string.IsNullOrWhiteSpace(ReturnUrl))
					{
						if (Url.IsLocalUrl(ReturnUrl))
						{
							return Redirect(ReturnUrl);
						}
					}
					else
					{
						return RedirectToAction("Index", "Home");
					}
				}

				ModelState.AddModelError(string.Empty, "登录失败，请重试");
			}

			return View(model);
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult AccessDenied() 
		{
			return View();
		}

		/// <summary>
		/// 检测用户注册时邮箱是否重复
		/// </summary>
		/// <param name="email"></param>
		/// <returns></returns>
		[AcceptVerbs("Get","Post")]
		[AllowAnonymous]
		public async Task<IActionResult> IsEmailInUse(string email) 
		{
			var user = await _userManager.FindByEmailAsync(email);

			if (user==null)
			{
				return Json(true);
			}
			else
			{
				return Json($"对不起，邮箱{email}已被注册，请更换邮箱");
			}
		}
	}
}
