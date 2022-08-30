using LearningWebSite.Areas.Admin.Controllers;
using LearningWebSite.Core.InfraStructure;
using LearningWebSite.Core.Senders;
using LearningWebSite.Core.Services;
using LearningWebSite.Core.ViewModel.Users;
using LearningWebSite.DataLayer.Entities.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TopLearn.Core.Convertors;

namespace LearningWebSite.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class AccountController : MainControllerBase
    {
        private readonly UserManager<CustomUser> _userManager;
        private readonly SignInManager<CustomUser> _signInManager;
        private readonly IViewRenderService _viewRenderService;
        private readonly IUserService _userService;
        public AccountController(UserManager<CustomUser> userManager, SignInManager<CustomUser> signInManager, IViewRenderService viewRenderService, IUserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _viewRenderService = viewRenderService;
            _userService = userService;
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid) return View(loginViewModel);
            var user = await _userManager.FindByEmailAsync(loginViewModel.Email.FixEmail());
            if (user == null)
            {
                ModelState.AddModelError(String.Empty, "کاربری با این مشخصات یافت نشد!");
                return View(loginViewModel);
            }
            var res = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, loginViewModel.RememberMe, false);
            if (res.Succeeded)
            {
                return RedirectToAction("Index", "Home", new { area = "User" });
            }
            ModelState.AddModelError(String.Empty, "نام کاربری یا کلمه عبور اشتباه می باشد!");
            return View(loginViewModel);
        }
        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(registerViewModel);
            }
            var user = _userService.SignUpUser(registerViewModel);
            await _userManager.CreateAsync(user, registerViewModel.Password);
            await _userManager.AddClaimAsync(user, new Claim("StudentType", "Student"));
            await _userManager.AddToRoleAsync(user,"Student");
            var body = _viewRenderService.RenderToStringAsync("_ActiveAccount", user);
            SendEmail.Send(user.Email, "فعال سازی", body);
            return RedirectToAction(nameof(ActiveAccount));
        }
        [HttpGet]
        [Route("ActiveAccount")]
        public IActionResult ActiveAccount()
        {
            return View();
        }
        [HttpPost]
        [Route("ActiveAccount")]
        public async Task<IActionResult> ActiveAccount(string code)
        {
            if (Url.IsLocalUrl(HttpContext.Request.Path))
            {
                var user = _userService.GetUserByActiveCode(code);
                if (user == null)
                {
                    ViewBag.Error = true;
                    return View("ActiveAccount");
                }
                if (user.EmailConfirmed)
                {
                    ViewBag.Error2 = true;
                    return View("ActiveAccount");
                }
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                await _userManager.ConfirmEmailAsync(user, token);
                _userService.changeActiveCode(user.Id);
                return Redirect("/");
            }
            return BadRequest();
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return Json(true);
            return Json("ایمیل وارد شده از قبل موجود است");
        }
        [HttpGet]
        [Route("ForgetPassword")]
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost("ForgetPassword")]
        public IActionResult ForgetPassword(string email)
        {
            var user = _userManager.FindByEmailAsync(email).Result;
            if (user == null)
            {
                return RedirectAndShowAlert(OperationResult.Error("کاربری با این ایمیل یافت نشد!"), View("ForgetPassword"));
            }
            var body = _viewRenderService.RenderToStringAsync("_resetPassword", user);
            SendEmail.Send(user.Email, "تغییر کلمه عبور", body);
            return RedirectAndShowAlert(OperationResult.Success("لطفا به ایمیل خود مراجعه کنید!"), View("ForgetPassword"));
        }
        [HttpGet]
        [Route("ResetPassword/{id}")]
        public IActionResult ResetPassword(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;
            if (user == null)
            {
                return NotFound();
            }
            ViewBag.UserId = id;
            return View();
        }
        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ChangePasswordByUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.Password);
            if (result.Succeeded)
            {
                return RedirectAndShowAlert(OperationResult.Success("کلمه عبور شما با موفقیت تغییر کرد!"), Redirect("/Account/Login"));

            }
            foreach (var item in result.Errors)
            {
                ModelState.AddModelError(string.Empty, item.Description);
            }
            ViewBag.UserId=model.UserId;
            return View(model);
        }
    }
}
