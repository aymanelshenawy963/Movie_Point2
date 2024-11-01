using ETickets.Models;
using ETickets.Utiltiy;
using ETickets.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace ETickets.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        public readonly RoleManager<IdentityRole> roleManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }

        public async Task<IActionResult> Regsister()
        {
            if (roleManager.Roles.IsNullOrEmpty())
            {
                await roleManager.CreateAsync(new(SD.AdminRole));
                await roleManager.CreateAsync(new(SD.CompanyRole));
                await roleManager.CreateAsync(new(SD.CustomarRole));
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Regsister(ApplicationUserVM userVM)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser applicationUser = new()
                {
                    UserName = userVM.Name,
                    Email = userVM.Email,
                    Adderss = userVM.Address
                };

                var result = await userManager.CreateAsync(applicationUser, userVM.Password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(applicationUser,SD.CustomarRole);
                    await signInManager.SignInAsync(applicationUser, false);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("Password", "error - does not match the constrains");
            }

            return View(userVM);
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            var applicationUser = await userManager.FindByNameAsync(loginVM.UserName);
            if (applicationUser != null)
            {
                var result = await userManager.CheckPasswordAsync(applicationUser,loginVM.Password);

                if (result)
                {
                    await signInManager.SignInAsync(applicationUser, loginVM.RemeberMe);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("Password", "Invalide Password");

                }

            }
            else
            {
                ModelState.AddModelError("UserName", "Invalide User");
            }

            return View();
        }

        public IActionResult Logout()
        {
            signInManager.SignOutAsync();
            return RedirectToAction("Login","Account");
        }

    }
}
