using LMS.Entities.Models;
using LMS.Utilities;
using LMS.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LMS.Web.Areas.AdminArea.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.SuperAdminRole)]
public class AdministrationController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }
    [HttpGet]
    public IActionResult Index()
    {
        ViewBag.TotalRoles = _roleManager.Roles.Count();
        ViewBag.TotalUsers = _userManager.Users.Count();
        return View();
    }

    #region Roles Options
    [HttpGet]
    public async Task<IActionResult> ListRoles()
    {
        List<IdentityRole> roles = await _roleManager.Roles.ToListAsync();
        return View(roles);
    }

    [HttpGet]
    public IActionResult CreateRole()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRole(CreateRoleViewModel roleModel)
    {
        if (ModelState.IsValid)
        {
            bool roleExists = await _roleManager.RoleExistsAsync(roleModel.RoleName);
            if (roleExists)
            {
                ModelState.AddModelError("", "Role Already Exists");
            }
            else
            {
                IdentityRole newRole = new IdentityRole
                {
                    Name = roleModel.RoleName
                };
                var result = await _roleManager.CreateAsync(newRole);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
        }
        return View(roleModel);
    }

    [HttpGet]
    public async Task<IActionResult> EditRole(string roleId)
    {
        IdentityRole role = await _roleManager.FindByIdAsync(roleId);
        if (role == null)
        {
            return View("Error");
        }

        var model = new EditRoleViewModel
        {
            Id = role.Id,
            RoleName = role.Name
        };

        model.Users = new List<string>();
        model.Claims = new List<string>();

        var roleClaims = await _roleManager.GetClaimsAsync(role);
        model.Claims = roleClaims.Select(c => c.Value).ToList();

        foreach (var user in await _userManager.Users.ToListAsync())
        {
            if (await _userManager.IsInRoleAsync(user, role.Name))
            {
                model.Users.Add(user.UserName);
            }
        }
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditRole(EditRoleViewModel model)
    {
        if (ModelState.IsValid)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            role.Name = model.RoleName;
            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction("ListRoles");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteRole(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null)
        {
            ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
            return View("NotFound");
        }

        try
        {
            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction("ListRoles");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View("ListRoles", await _roleManager.Roles.ToListAsync());
        }
        catch (DbUpdateException ex)
        {
            ViewBag.Error = ex.Message;
            ViewBag.ErrorTitle = $"{role.Name} Role is in Use";
            ViewBag.ErrorMessage = $"{role.Name} Role cannot be deleted as there are users in this role. If you want to delete this role, please remove the users from the role and then try to delete";
            return View("Error");
        }
    }

    [HttpGet]
    public async Task<IActionResult> ManageRoleClaims(string RoleId)
    {
        var role = await _roleManager.FindByIdAsync(RoleId);
        if (role == null)
        {
            ViewBag.ErrorMessage = $"Role with Id = {RoleId} cannot be found";
            return View("NotFound");
        }
        ViewBag.RoleName = role.Name;

        var model = new RoleClaimsViewModel
        {
            RoleId = RoleId
        };

        var existingRoleClaims = await _roleManager.GetClaimsAsync(role);

        foreach (Claim claim in ClaimsStore.GetAllClaims())
        {
            AppClaim roleClaim = new AppClaim
            {
                ClaimType = claim.Type,
                IsSelected = existingRoleClaims.Any(c => c.Type == claim.Type),
                Category = claim.Type.Split('.')[0]
            };
            model.Claims.Add(roleClaim);
        }
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ManageRoleClaims(RoleClaimsViewModel model)
    {
        var role = await _roleManager.FindByIdAsync(model.RoleId);
        if (role == null)
        {
            ViewBag.ErrorMessage = $"Role with Id = {model.RoleId} cannot be found";
            return View("NotFound");
        }

        var claims = await _roleManager.GetClaimsAsync(role);


        for (int i = 0; i < model.Claims.Count; i++)
        {
            Claim claim = new Claim(model.Claims[i].ClaimType, model.Claims[i].ClaimType);

            IdentityResult? result;

            if (model.Claims[i].IsSelected && !claims.Any(c => c.Type == claim.Type))
            {
                result = await _roleManager.AddClaimAsync(role, claim);
            }
            else if (!model.Claims[i].IsSelected && claims.Any(c => c.Type == claim.Type))
            {
                result = await _roleManager.RemoveClaimAsync(role, claim);
            }
            else
            {
                continue;
            }

            if (result.Succeeded)
            {
                if (i < model.Claims.Count - 1)
                    continue;
                else
                    return RedirectToAction("EditRole", new { roleId = model.RoleId });
            }
            else
            {
                ModelState.AddModelError("", "Cannot add or removed selected claims to role");
                return View(model);
            }
        }
        return RedirectToAction("EditRole", new { roleId = model.RoleId });
    }
    #endregion

    #region Users Options
    [HttpGet]
    public IActionResult ListUsers()
    {
        var users = _userManager.Users;
        return View(users);
    }

    [HttpGet]
    public IActionResult CreateUser()
    {
        var excludedRoles = new[]
        {
            SD.SuperAdminRole,
            SD.TeacherRole,
            SD.StudentRole,
            SD.ParentRole
        };

        var roles = _roleManager.Roles.Where(r => !excludedRoles.Contains(r.Name))
            .Select(r => new SelectListItem
            {
                Value = r.Name,
                Text = r.Name
            }).ToList();

        var viewModel = new CreateUserViewModel
        {
            Roles = roles
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateUser(CreateUserViewModel newUser)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = newUser.Email,
                Email = newUser.Email,
                FullName = newUser.FullName,
                Address = newUser.Address
            };

            var result = await _userManager.CreateAsync(user, newUser.Password);
            if (result.Succeeded)
            {
                if (newUser.SelectedRoles != null && newUser.SelectedRoles.Any())
                {
                    await _userManager.AddToRolesAsync(user, newUser.SelectedRoles);
                }
                return RedirectToAction("ListUsers", "Administration");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        newUser.Roles = _roleManager.Roles.Select(r => new SelectListItem
        {
            Value = r.Name,
            Text = r.Name
        }).ToList();
        return View(newUser);
    }

    [HttpGet]
    public async Task<IActionResult> EditUser(string UserId)
    {
        var user = await _userManager.FindByIdAsync(UserId);

        if (user == null)
        {
            ViewBag.ErrorMessage = $"User with Id = {UserId} cannot be found";
            return View("NotFound");
        }

        var model = new EditUserViewModel
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            FullName = user.FullName,
            Address = user.Address,
            Claims = (await _userManager.GetClaimsAsync(user)).Select(c => c.Value).ToList(),
            Roles = await _userManager.GetRolesAsync(user)
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUser(EditUserViewModel model)
    {
        var user = await _userManager.FindByIdAsync(model.Id);

        if (user == null)
        {
            ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
            return View("NotFound");
        }

        user.Email = model.Email;
        user.UserName = model.UserName;
        user.FullName = model.FullName;
        user.Address = model.Address;

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            return RedirectToAction("ListUsers");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(string UserId)
    {
        var user = await _userManager.FindByIdAsync(UserId);

        if (user == null)
        {
            ViewBag.ErrorMessage = $"User with Id = {UserId} cannot be found";
            return View("NotFound");
        }

        var result = await _userManager.DeleteAsync(user);

        if (result.Succeeded)
        {
            return RedirectToAction("ListUsers");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View("ListUsers");
    }

    [HttpGet]
    public async Task<IActionResult> ManageUserRoles(string UserId)
    {
        var user = await _userManager.FindByIdAsync(UserId);
        if (user == null)
        {
            ViewBag.ErrorMessage = $"User with Id = {UserId} cannot be found";
            return View("NotFound");
        }
        ViewBag.UserId = UserId;
        ViewBag.UserName = user.UserName;

        var model = new List<UserRolesViewModel>();
        foreach (var role in await _roleManager.Roles.ToListAsync())
        {
            var userRolesViewModel = new UserRolesViewModel
            {
                RoleId = role.Id,
                RoleName = role.Name,
                IsSelected = await _userManager.IsInRoleAsync(user, role.Name)
            };
            model.Add(userRolesViewModel);
        }
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ManageUserRoles(List<UserRolesViewModel> model, string UserId)
    {
        var user = await _userManager.FindByIdAsync(UserId);

        if (user == null)
        {
            ViewBag.ErrorMessage = $"User with Id = {UserId} cannot be found";
            return View("NotFound");
        }

        var roles = await _userManager.GetRolesAsync(user);

        var result = await _userManager.RemoveFromRolesAsync(user, roles);

        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Cannot remove user existing roles");
            return View(model);
        }

        List<string> RolesToBeAssigned = model.Where(x => x.IsSelected).Select(y => y.RoleName).ToList();

        if (RolesToBeAssigned.Any())
        {
            result = await _userManager.AddToRolesAsync(user, RolesToBeAssigned);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot Add Selected Roles to User");
                return View(model);
            }
        }

        return RedirectToAction("EditUser", new { UserId });
    }
    #endregion
}


