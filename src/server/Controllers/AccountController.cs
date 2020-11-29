using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using MyTeam.Models;
using MyTeam.Resources;
using Services.Utils;
using MyTeam.Services.Application;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.Account;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace MyTeam.Controllers
{
    [Authorize]
    [Route("konto")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly EmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly IPlayerService _playerService;
        private readonly ICacheHelper _cacheHelper;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            EmailSender emailSender,
            ILoggerFactory loggerFactory,
            IPlayerService playerService,
            ICacheHelper cacheHelper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _cacheHelper = cacheHelper;
            _playerService = playerService;
        }


        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("innlogging")]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null, bool local = false)
        {
            
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Local"] = local;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _cacheHelper.ClearCache(HttpContext.GetClub()?.Id, model.Email);
                    _logger.LogDebug(1, "User logged in.");
                    return RedirectToLocal(returnUrl);
                }
                                if (result.IsLockedOut)
                {
                    _logger.LogWarning(2, "User account locked out.");
                    return View("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Ugyldig innlogginsforsøk.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        [Route("ny")]
        public IActionResult Register(string returnUrl = null)
        {
            if (returnUrl?.Contains("returnUrl") == true) HttpContext.Abort();
            ViewData["ReturnUrl"] = returnUrl;
            var model = new RegisterViewModel();
            return View(model);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("ny")]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = "/")
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                    try
                    {
                        await _emailSender.SendEmailAsync(model.Email, "Bekreft din konto",
                        "Bekreft kontoen din ved å trykke <a href=\"" + callbackUrl + "\">her</a>");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Klarte ikke sende e-post ved oppretting av konto");
                    }
                    await _signInManager.SignInAsync(user, isPersistent: true);
                    _logger.LogInformation(3, "User created a new account with password.");
                    return Redirect(returnUrl);
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("utlogging")]
        public async Task<IActionResult> LogOff(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogDebug(4, "User logged out.");
            _cacheHelper.ClearCache(HttpContext.GetClub()?.Id, HttpContext.User.Identity.Name);
            return RedirectToLocal(returnUrl);
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("innlogging/ekstern/bekreftelse")]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
        {

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _userManager.AddClaimAsync(user,
                        new Claim("facebookFirstName",
                               info.Principal.Claims.First(c => c.Type == ClaimTypes.GivenName).Value));
                        await _userManager.AddClaimAsync(user,
                            new Claim("facebookLastName",
                                        info.Principal.Claims.First(c => c.Type == ClaimTypes.Surname).Value));
                        await _userManager.AddClaimAsync(user,
                            new Claim("facebookId",
                                        info.Principal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value));

                        await _signInManager.SignInAsync(user, isPersistent: true);

                        _logger.LogInformation(6, "User {Email} created an account using {Name} provider.", model.Email, info.LoginProvider);

                        _playerService.AddEmailToPlayer(model.FacebookId, model.Email);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("epost/bekreft")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("passord/glemt")]
        public IActionResult ForgotPassword()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("passord/glemt")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user == null) //|| !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // Send an email with this link
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code, email = user.Email }, protocol: HttpContext.Request.Scheme);
                await _emailSender.SendEmailAsync(model.Email, "Nullstill passord",
                   "Du kan nullstille passordet ditt ved å trykke <a href=\"" + callbackUrl + "\">her</a>");
                return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("passord/bekreft")]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("passord/nullstill")]
        public IActionResult ResetPassword(string code = null, string email = "")
        {
            var model = new ResetPasswordViewModel
            {
                Code = code,
                Email = email
            };
            return code == null ? View("Error") : View(model);
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("passord/nullstill")]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            AddErrors(result);
            return View();
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("passord/nullstill/bekreft")]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }       

        
        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }


        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return Redirect("/");
            }
        }

        #endregion
    }
   
}
