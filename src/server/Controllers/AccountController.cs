using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyTeam.Models;
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
        private readonly ILogger _logger;
        private readonly IPlayerService _playerService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILoggerFactory loggerFactory,
            IPlayerService playerService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
