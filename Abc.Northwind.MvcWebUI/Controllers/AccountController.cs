using Abc.Northwind.MvcWebUI.Entities;
using Abc.Northwind.MvcWebUI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Abc.Northwind.MvcWebUI.Controllers
{
    public class AccountController : Controller
    {
        UserManager<CustomIdentityUser> _userManager;
        RoleManager<CustomIdentityRole> _roleManager;
        SignInManager<CustomIdentityUser> _signInManager;

        // identity kısmında geliştirme tamamlanınca; migration için npm console açılır ve 'add-migration identity' komutu çalıştırılır
        // ardından 'update-database' komutu çalıştırılır ve DB'de gerekli tabloların da oluşması sağlanır
        public AccountController(UserManager<CustomIdentityUser> userManager, RoleManager<CustomIdentityRole> roleManager, SignInManager<CustomIdentityUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel registerViewModel)
        {
            var defaultRole = "Admin";

            if (ModelState.IsValid)
            {
                CustomIdentityUser user = new CustomIdentityUser()
                {
                    UserName = registerViewModel.UserName,
                    Email = registerViewModel.Email
                };

                // kullanıcı oluşturur
                IdentityResult result =
                    _userManager.CreateAsync(user, registerViewModel.Password).Result;

                if (result.Succeeded)
                {
                    // sistemde rol var mı diye bakar
                    if (!_roleManager.RoleExistsAsync(defaultRole).Result)
                    {
                        CustomIdentityRole role = new CustomIdentityRole()
                        {
                            Name = defaultRole
                        };

                        IdentityResult roleResult = _roleManager.CreateAsync(role).Result;
                        // yoksa default rol'ü oluşturacaktır

                        if (!roleResult.Succeeded)
                        {
                            ModelState.AddModelError("", "We can't add the role!");
                            return View(registerViewModel);
                        }
                    }
                    // kullanıcı oluşturduk, rol yoksa onu da oluşturduk ve kullanıcıya rolünü veriyoruz
                    _userManager.AddToRoleAsync(user, defaultRole).Wait();

                    return RedirectToAction("Login", "Account");
                }

                var errors = string.Empty;

                foreach (var item in result.Errors)
                {
                    errors += " " + item.Description;
                }

                TempData.Add("message", errors);
            }
            return View(registerViewModel);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = _signInManager.PasswordSignInAsync(loginViewModel.UserName,
                    loginViewModel.Password, loginViewModel.RememberMe, false).Result;
                // false: login gerçekleşmezse hesabı kilitleyelim mi?  - hayır, denemeye devam edilebilsin

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Admin");
                }

                ModelState.AddModelError("", "Invalid login!");
                TempData.Add("message", "Invalid login!");
            }
            return View(loginViewModel);
        }
        public ActionResult LogOff()
        {
            _signInManager.SignOutAsync().Wait();

            return RedirectToAction("Login");
        }
    }
}
