using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicTacToe.Models;
using TicTacToe.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Routing;

namespace TicTacToe.Controllers
{
    public class AccountController : Controller
    {
        private IUserService _userService;
        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Login(string returnUrl)
        {
            return await Task.Run(() =>
            {
                var loginModel = new LoginModel { ReturnUrl = returnUrl };
                return View(loginModel);
            });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.SignInUser(loginModel, HttpContext);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(loginModel.ReturnUrl))
                        return Redirect(loginModel.ReturnUrl);
                    else
                        return RedirectToAction("Index", "Home");
                }
                else if (result.RequiresTwoFactor)
                {
                    await SendEmailTwoFactor(loginModel.UserName);
                    return RedirectToAction("ValidateTwoFactor");
                }
                else
                    ModelState.AddModelError("", result.IsLockedOut ? "User is locked" : "User is not allowed");
            }

            return View();
        }

        public IActionResult Logout()
        {
            _userService.SignOutUser(HttpContext).Wait();
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public async Task<ActionResult> ExternalLogin(string provider, string ReturnUrl)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallBack), "Account", new { ReturnUrl = ReturnUrl }, Request.Scheme, Request.Host.ToString());
            var properties = await _userService.GetExternalAuthenticationProperties(provider, redirectUrl);
            ViewBag.ReturnUrl = redirectUrl;
            return Challenge(properties, provider);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallBack(string returnUrl, string remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                ViewBag.ReturnUrl = returnUrl;
                return View("Login");
            }

            var info = await _userService.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction("Login", new { ReturnUrl = returnUrl });
            }

            var result = await _userService.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl))
                    return Redirect(returnUrl);
                else
                    return RedirectToAction("Index", "Home");
            }
            if (result.IsLockedOut)
            {
                return View("Lockout");
            }
            else
            {
                return View("NotFound");
            }
        }

        private async Task SendEmailTwoFactor(string UserName)
        {
            var user = await _userService.GetUserByEmail(UserName);
            var urlAction = new UrlActionContext
            {
                Action = "ValidateTwoFactor",
                Controller = "Account",
                Values = new { email = UserName, code = await _userService.GetTwoFactorCode(user.UserName, "Email") },
                Protocol = Request.Scheme,
                Host = Request.Host.ToString()
            };

            var TwoFactorEmailModel = new TwoFactorEmailModel
            {
                DisplayName = $"{user.FirstName} {user.LastName}",
                Email = UserName,
                ActionUrl = Url.Action(urlAction)
            };
            var emailRenderService = HttpContext.RequestServices.GetService<IEmailTemplateRenderService>();
            var emailService = HttpContext.RequestServices.GetService<IEmailService>();
            var message = await emailRenderService.RenderTemplate("EmailTemplates/TwoFactorEmail", TwoFactorEmailModel, Request.Host.ToString());
            try
            {
                emailService.SendEmail(UserName, "Tic-Tac-Toe Two Factor Code", message).Wait();
            }
            catch
            {
            }
        }

        public async Task<IActionResult> ValidateTwoFactor(string email, string code)
        {
            return await Task.Run(() =>
            {
                return View(new ValidateTwoFactorModel { Code = code, UserName = email });
            });
        }

        [HttpPost]
        public async Task<IActionResult> ValidateTwoFactor(ValidateTwoFactorModel validateTwoFactorModel)
        {
            if (ModelState.IsValid)
            {
                await _userService.ValidateTwoFactor(validateTwoFactorModel.UserName, "Email", validateTwoFactorModel.Code, HttpContext);
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ForgotPassword()
        {
            return await Task.Run(() =>
            {
                return View();
            });
        }

        [HttpPost]
        public async Task<IActionResult> SendResetPassword(string UserName)
        {
            var user = await _userService.GetUserByEmail(UserName);
            var urlAction = new UrlActionContext
            {
                Action = "ResetPassword",
                Controller = "Account",
                Values = new { email = UserName, code = await _userService.GetResetPasswordCode(user) },
                Protocol = Request.Scheme,
                Host = Request.Host.ToString()
            };

            var resetPasswordEmailModel = new ResetPasswordEmailModel
            {
                DisplayName = $"{user.FirstName} {user.LastName}",
                Email = UserName,
                ActionUrl = Url.Action(urlAction)
            };

            var emailRenderService = HttpContext.RequestServices.GetService<IEmailTemplateRenderService>();
            var emailService = HttpContext.RequestServices.GetService<IEmailService>();
            var message = await emailRenderService.RenderTemplate("EmailTemplates/ResetPasswordEmail", resetPasswordEmailModel,
                Request.Host.ToString());

            try
            {
                emailService.SendEmail(UserName, "Tic-Tac-Toe Reset Password", message).Wait();
            }
            catch
            {

            }

            return View("ConfirmResetPasswordRequest", resetPasswordEmailModel);
        }

        public async Task<IActionResult> ResetPassword(string email, string code)
        {
            var user = await _userService.GetUserByEmail(email);
            ViewBag.Code = code;
            return View(new ResetPasswordModel { Token = code, UserName = email });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel reset)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.ResetPassword(reset.UserName, reset.Password, reset.Token);

                if (result.Succeeded)
                    return RedirectToAction("Login");
                else
                    ModelState.AddModelError("", "Cannot reset your password");
            }
            return View();
        }
    }
}
