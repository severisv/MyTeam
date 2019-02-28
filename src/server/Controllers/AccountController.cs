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
using Microsoft.Extensions.Options;

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
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        [Route("innlogging")]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Title"] = Res.Login;
            var model = new LoginViewModel();
            return View(model);
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
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
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
        [Route("innlogging/ekstern")]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("innlogging/ekstern/callback")]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true);
            if (result.Succeeded)
            {
                _logger.LogDebug(5, "User logged in with {Name} provider.", info.LoginProvider);

                var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

                await _signInManager.SignInAsync(user, isPersistent: true);
                if (!info.Principal.Claims.Any(c => c.Type == "facebookFirstName"))
                    await _userManager.AddClaimAsync(user,
                    new Claim("facebookFirstName",
                           info.Principal.Claims.First(c => c.Type == ClaimTypes.GivenName).Value));

                if (!info.Principal.Claims.Any(c => c.Type == "facebookLastName"))
                    await _userManager.AddClaimAsync(user,
                        new Claim("facebookLastName",
                                    info.Principal.Claims.First(c => c.Type == ClaimTypes.Surname).Value));

                if (!info.Principal.Claims.Any(c => c.Type == "facebookId"))
                    await _userManager.AddClaimAsync(user,
                        new Claim("facebookId",
                                    info.Principal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value));

                return RedirectToLocal(returnUrl);
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl });
            }
            if (result.IsLockedOut)
            {
                return View("Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var idClaim = info.Principal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier);
                ViewData["FacebookId"] = idClaim.Value;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = email });
            }
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


        [HttpGet]
        [AllowAnonymous]
        [Route("kode/send")]
        public async Task<ActionResult> SendCode(string returnUrl = null, bool rememberMe = false)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("kode/send")]
        public async Task<IActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            // Generate the token and send it
            var code = await _userManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
            if (string.IsNullOrWhiteSpace(code))
            {
                return View("Error");
            }

            var message = "Your security code is: " + code;
            if (model.SelectedProvider == "Email")
            {
                await _emailSender.SendEmailAsync(await _userManager.GetEmailAsync(user), "Security Code", message);
            }

            return RedirectToAction(nameof(VerifyCode), new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("kode/verifiser")]
        public async Task<IActionResult> VerifyCode(string provider, bool rememberMe, string returnUrl = null)
        {
            // Require that the user has already logged in via username/password or external login
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("kode/verifiser")]
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes.
            // If a user enters incorrect codes for a specified amount of time then the user account
            // will be locked out for a specified amount of time.
            var result = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser);
            if (result.Succeeded)
            {
                return RedirectToLocal(model.ReturnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning(7, "User account locked out.");
                return View("Lockout");
            }
            else
            {
                ModelState.AddModelError("", "Invalid code.");
                return View(model);
            }
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
