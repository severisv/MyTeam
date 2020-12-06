using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyTeam.Models;
using Services.Utils;
using MyTeam.Services.Application;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.Account;
using ILogger = Microsoft.Extensions.Logging.ILogger;


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

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            EmailSender emailSender,
            ILoggerFactory loggerFactory,
            IPlayerService playerService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _playerService = playerService;
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
