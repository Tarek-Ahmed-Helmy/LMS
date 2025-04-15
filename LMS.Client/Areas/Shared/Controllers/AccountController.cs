using LMS.Entities.Models;
using LMS.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Web.Areas.Shared.Controllers;

[Area("Shared")]
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _signInManager.PasswordSignInAsync(model.EmailOrUserName, model.Password, model.RememberMe, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            var user = await _userManager.FindByNameAsync(model.EmailOrUserName)
                        ?? await _userManager.FindByEmailAsync(model.EmailOrUserName);

            if (user == null)
            {
                await _signInManager.SignOutAsync();
                ModelState.AddModelError(string.Empty, "User not found.");
                return View(model);
            }

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("SuperAdmin"))
                return RedirectToAction("Index", "Administration", new { area = "Admin" });

            if (roles.Contains("Admin"))
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

            if (roles.Contains("Teacher"))
                return RedirectToAction("Index", "Dashboard", new { area = "Teacher" });

            if (roles.Contains("Student"))
                return RedirectToAction("Index", "Dashboard", new { area = "Student" });

            if (roles.Contains("Parent"))
                return RedirectToAction("Index", "Dashboard", new { area = "Parent" });

            // Default fallback
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account", new { area = "Shared" });
    }

    [AllowAnonymous]
    [HttpPost]
    [HttpGet]
    public async Task<IActionResult> IsEmailAvailable(string Email)
    {
        var user = await _userManager.FindByEmailAsync(Email);

        if (user == null)
        {
            return Json(true);
        }
        else
        {
            return Json($"Email {Email} is already in use.");
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }
}