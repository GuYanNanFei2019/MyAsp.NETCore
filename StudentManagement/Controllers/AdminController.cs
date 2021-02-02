using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudentManagement.ViewModels.IdentityViewModel;
using StudentManagement_DataBase.EFModel.IdentityModel;
using StudentManagement_DataBase.ModelExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentManagement.Controllers
{
	[Authorize(Roles = "Admin")]
	public class AdminController : Controller
	{
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ILogger _logger;

		public AdminController(RoleManager<IdentityRole> roleManager, ILogger<AdminController> logger, UserManager<ApplicationUser> userManager)
		{
			_roleManager = roleManager;
			_userManager = userManager;

			_logger = logger;
			_logger.LogInformation("NLOG注入Admin控制器");
		}

		#region 角色管理

		/// <summary>
		/// 创建角色(视图)
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Authorize(policy: "CreateRolePolicy")]
		public IActionResult CreateRole()
		{
			return View();
		}

		/// <summary>
		/// 创建角色
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		[HttpPost]
		[Authorize(policy: "CreateRolePolicy")]
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
					return RedirectToAction("ListRoles");
				}

				foreach (var item in res.Errors)
				{
					ModelState.AddModelError(string.Empty, item.Description);
				}
			}

			return View(viewModel);
		}

		/// <summary>
		/// 角色列表
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public IActionResult ListRoles()
		{
			var roles = _roleManager.Roles;
			return View(roles);
		}

		/// <summary>
		/// 编辑角色(视图)
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		[Authorize(policy: "EditRolePolicy")]
		public async Task<IActionResult> EditRole(string id)
		{
			var role = await _roleManager.FindByIdAsync(id);

			if (role == null)
			{
				ViewBag.ErrorMessage = $"ID为{id}的角色不存在，请重试";

				_logger.LogError($"Admin控制器中EditRole方法ID为{id}的角色不存在");

				return View("~/Views/Error/RouteNotFound.cshtml");
			}

			var model = new EditRoleViewModel
			{
				Id = role.Id,
				RoleName = role.Name
			};

			foreach (var item in _userManager.Users)
			{
				if (await _userManager.IsInRoleAsync(item, role.Name))
				{
					model.Users.Add(item.UserName);
				}
			}

			return View(model);
		}

		/// <summary>
		/// 编辑角色
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Authorize(policy: "EditRolePolicy")]
		public async Task<IActionResult> EditRole(EditRoleViewModel model)
		{
			var role = await _roleManager.FindByIdAsync(model.Id);

			if (role == null)
			{
				ViewBag.ErrorMessage = $"ID为{model.Id}的角色不存在，请重试";

				_logger.LogError($"Admin控制器中EditRole方法ID为{model.Id}的角色不存在");

				return View("~/Views/Error/RouteNotFound.cshtml");
			}
			else
			{
				role.Name = model.RoleName;

				var res = await _roleManager.UpdateAsync(role);

				if (res.Succeeded)
				{
					return RedirectToAction("ListRoles");
				}

				foreach (var item in res.Errors)
				{
					ModelState.AddModelError("", item.Description);
				}

				return View(model);
			}
		}

		/// <summary>
		/// 编辑角色中的用户(视图)
		/// </summary>
		/// <param name="roleId"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> EditUserInRole(string roleId)
		{
			ViewBag.roleId = roleId;

			var role = await _roleManager.FindByIdAsync(roleId);

			if (role == null)
			{
				ViewBag.ErrorMessage = $"ID为{roleId}的角色不存在，请重试";

				_logger.LogError($"Admin控制器中EditUserInRole方法ID为{roleId}的角色不存在");

				return View("~/Views/Error/RouteNotFound.cshtml");
			}

			var model = new List<UserRoleViewModel>();

			foreach (var item in _userManager.Users)
			{
				var userRoleViewModel = new UserRoleViewModel
				{
					UserId = item.Id,
					UserName = item.UserName
				};

				userRoleViewModel.IsSelected = await _userManager.IsInRoleAsync(item, role.Name);

				model.Add(userRoleViewModel);
			}

			return View(model);
		}

		/// <summary>
		/// 编辑角色中的用户
		/// </summary>
		/// <param name="model"></param>
		/// <param name="roleId"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> EditUserInRole(List<UserRoleViewModel> model, string roleId)
		{
			var role = await _roleManager.FindByIdAsync(roleId);

			if (role == null)
			{
				ViewBag.ErrorMessage = $"ID为{roleId}的角色不存在，请重试";

				_logger.LogError($"Admin控制器中EditUserInRole方法ID为{roleId}的角色不存在");

				return View("~/Views/Error/RouteNotFound.cshtml");
			}

			IdentityResult res = null;

			//判断当前用户是否已经属于该角色且被选中
			//不属于的话，添加到该角色中
			//没有选中要移除出来
			for (int i = 0; i < model.Count; i++)
			{
				var user = await _userManager.FindByIdAsync(model[i].UserId);

				//被选中且不属于该角色，添加到角色中
				if (model[i].IsSelected && !await _userManager.IsInRoleAsync(user, role.Name))
				{
					res = await _userManager.AddToRoleAsync(user, role.Name);
				}
				else if (!model[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
				{
					//没有被选中，属于该角色，从该角色中移除
					res = await _userManager.RemoveFromRoleAsync(user, role.Name);
				}
				else
				{
					continue;//被选中且已存在于该角色中，不发生改变。
				}

				if (res.Succeeded)
				{
					if (i < (model.Count - 1))
					{
						continue;
					}
					else
					{
						return RedirectToAction("EditRole", new { id = role.Id });
					}
				}
			}

			return RedirectToAction("EditRole", new { id = role.Id });
		}

		/// <summary>
		/// 删除角色
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[Authorize(policy: "DeleteRolePolicy")]
		[HttpPost]
		public async Task<IActionResult> DeleteRole(string id)
		{
			var role = await _roleManager.FindByIdAsync(id);

			if (role == null)
			{
				ViewBag.ErrorMessage = $"ID为{id}的角色不存在，请重试";
				_logger.LogError($"Admin控制器中DeleteRole方法ID为{id}的角色不存在");

				return View("~/Views/Error/RouteNotFound.cshtml");
			}

			try
			{
				var res = await _roleManager.DeleteAsync(role);

				if (res.Succeeded)
				{
					return RedirectToAction("ListRoles");
				}

				foreach (var item in res.Errors)
				{
					ModelState.AddModelError("", item.Description);
				}

				return RedirectToAction("ListRoles");
			}
			catch (DbUpdateException ex)
			{
				_logger.LogError($"Admin控制器中的DeleteRole方法发生异常：{ex}");

				ViewBag.ErrorTitle = $"角色{role.Name}正在被使用中，无法删除！";
				ViewBag.ErrorMessage = $"角色{role.Name}无法删除，因为该角色中拥有用户。如果您想删除该角色，请先删除该角色下的用户。";
				return View("~/Views/Error/Error.cshtml");
			}
		}
		#endregion

		#region 用户管理
		/// <summary>
		/// 用户列表
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public IActionResult ListUsers()
		{
			var users = _userManager.Users;
			return View(users);
		}

		/// <summary>
		/// 编辑用户(视图)
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		[Authorize(policy: "AdminPolicy")]
		public async Task<IActionResult> EditUser(string id)
		{
			var user = await _userManager.FindByIdAsync(id);

			if (user == null)
			{
				ViewBag.ErrorMessage = $"ID为{id}的用户不存在，请重试";
				_logger.LogError($"Admin控制器中EditUser方法ID为{id}的用户不存在");

				return View("~/Views/Error/RouteNotFound.cshtml");
			}

			var userClaims = await _userManager.GetClaimsAsync(user);

			var userRoles = await _userManager.GetRolesAsync(user);

			var model = new EditUserViewModel
			{
				ID = user.Id,
				Email = user.Email,
				UserName = user.UserName,
				City = user.City,
				Claims = userClaims,
				Roles = userRoles
			};
			return View(model);
		}

		/// <summary>
		/// 编辑用户
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Authorize(policy: "AdminPolicy")]
		public async Task<IActionResult> EditUser(EditUserViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByIdAsync(model.ID);

				if (user == null)
				{
					ViewBag.ErrorMessage = $"ID为{model.ID}的用户不存在，请重试";
					_logger.LogError($"Admin控制器中EditUser方法ID为{model.ID}的用户不存在");

					return View("~/Views/Error/RouteNotFound.cshtml");
				}

				user.Email = model.Email;
				user.UserName = model.UserName;
				user.City = model.City;

				var res = await _userManager.UpdateAsync(user);

				if (res.Succeeded)
				{
					return RedirectToAction("ListUsers");
				}

				foreach (var item in res.Errors)
				{
					ModelState.AddModelError("", item.Description);
				}
			}

			return View(model);
		}

		/// <summary>
		/// 删除用户
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpPost]
		[Authorize(policy: "AdminPolicy")]
		public async Task<IActionResult> DeleteUser(string id)
		{
			var user = await _userManager.FindByIdAsync(id);

			if (user == null)
			{
				ViewBag.ErrorMessage = $"ID为{id}的用户不存在，请重试";
				_logger.LogError($"Admin控制器中DeleteUser方法ID为{id}的用户不存在");

				return View("~/Views/Error/RouteNotFound.cshtml");
			}

			var res = await _userManager.DeleteAsync(user);

			if (res.Succeeded)
			{
				return RedirectToAction("ListUsers");
			}

			foreach (var item in res.Errors)
			{
				ModelState.AddModelError("", item.Description);
			}

			return RedirectToAction("ListUsers");
		}

		/// <summary>
		/// 管理用户的角色(视图)
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		[HttpGet]
		//[Authorize(policy: "AdminPolicy")]
		[Authorize(Policy = "EditRolePolicy")]
		public async Task<IActionResult> ManagerUserRole(string userId) 
		{
			ViewBag.userId = userId;

			var user =await _userManager.FindByIdAsync(userId);

			if (user==null)
			{
				ViewBag.ErrorMessage = $"ID为{userId}的用户不存在，请重试";
				_logger.LogError($"Admin控制器中ManagerUserRole方法ID为{userId}的用户不存在");

				return View("~/Views/Error/RouteNotFound.cshtml");
			}


			var model = new List<RolesInUserViewModel>();

			foreach (var item in _roleManager.Roles)
			{
				var rolesInViewModel = new RolesInUserViewModel
				{
					RoleId = item.Id,
					RoleName = item.Name
				};

				//判断当前用户是否已经在该角色下
				
				if (await _userManager.IsInRoleAsync(user, item.Name))
				{
					rolesInViewModel.IsSelected = true;
				}
				else
				{
					rolesInViewModel.IsSelected = false;
				}

				model.Add(rolesInViewModel);
			}

			return View(model);
		}

		/// <summary>
		/// 管理用户的角色
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> ManagerUserRole(List<RolesInUserViewModel> viewModel,string userId) 
		{
			var user = await _userManager.FindByIdAsync(userId);

			if (user == null)
			{
				ViewBag.ErrorMessage = $"ID为{userId}的用户不存在，请重试";
				_logger.LogError($"Admin控制器中ManagerUserRole方法ID为{userId}的用户不存在");

				return View("~/Views/Error/RouteNotFound.cshtml");
			}

			var roles = await _userManager.GetRolesAsync(user);

			//移除当前用户的所有角色
			var res = await _userManager.RemoveFromRolesAsync(user, roles);

			if (!res.Succeeded)
			{
				foreach (var item in res.Errors)
				{
					ModelState.AddModelError("", item.Description);
				}

				return View(viewModel);
			}

			//查询出模型列表中被选中的角色添加到用户
			res = await _userManager.AddToRolesAsync(user: user, roles: viewModel.Where(x => x.IsSelected).Select(y=>y.RoleName));
			if (!res.Succeeded)
			{
				foreach (var item in res.Errors)
				{
					ModelState.AddModelError("", item.Description);
				}

				return View(viewModel);
			}

			return RedirectToAction("EditUser",new { id=userId});
		}

		/// <summary>
		/// 管理用户声明(视图)
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		[HttpGet]
		[Authorize(policy: "AdminPolicy")]
		public async Task<IActionResult> ManagerUserClaims(string userId) 
		{
			ViewBag.userId = userId;

			var user = await _userManager.FindByIdAsync(userId);

			if (user == null)
			{
				ViewBag.ErrorMessage = $"ID为{userId}的用户不存在，请重试";
				_logger.LogError($"Admin控制器中ManagerUserClaims方法ID为{userId}的用户不存在");

				return View("~/Views/Error/RouteNotFound.cshtml");
			}

			//查找当前用户是否有声明
			var existingClaim = await _userManager.GetClaimsAsync(user);

			var model = new UserClaimViewModel
			{
				UserId = userId
			};

			//循环程序的所有声明
			foreach (var item in ClaimsStore.AllClaim)
			{
				UserClaim userClaim = new UserClaim
				{
					ClaimType = item.Type
				};
				if (existingClaim.Any(c => c.Type == item.Type))
				{
					userClaim.IsSelected = true;
				}
				model.UserClaims.Add(userClaim);
			}

			return View(model);
		}

		/// <summary>
		/// 管理用户声明
		/// </summary>
		/// <param name="model"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> ManagerUserClaims(UserClaimViewModel model,string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);

			if (user == null)
			{
				ViewBag.ErrorMessage = $"ID为{userId}的用户不存在，请重试";
				_logger.LogError($"Admin控制器中ManagerUserClaims方法ID为{userId}的用户不存在");

				return View("~/Views/Error/RouteNotFound.cshtml");
			}

			//查找用户现有的声明并清空
			var claims =await _userManager.GetClaimsAsync(user);
			var res = await _userManager.RemoveClaimsAsync(user, claims);

			if (!res.Succeeded)
			{
				foreach (var item in res.Errors)
				{
					ModelState.AddModelError("", item.Description);
				}

				return View(model);
			}

			//添加选中的声明到用户中
			res = await _userManager.AddClaimsAsync(user, model.UserClaims.Where(x => x.IsSelected)
																 .Select(y => new Claim(y.ClaimType, y.IsSelected ? "true" : "false")));

			return RedirectToAction("EditUser", new { id =model.UserId});
		}
		#endregion
	}
}
