using System;
using System.Linq;
using Microsoft.AspNet.Mvc;
using MyTeam.Controllers;
using MyTeam.Models.Dto;

namespace MyTeam.Filters
{
    public class RequireMemberAttribute : ActionFilterAttribute
    {
        private string[] _roles;
        public RequireMemberAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public RequireMemberAttribute()
        {
            
        }

        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            var controller = (BaseController)actionContext.Controller;

            var userPlayer = controller.CurrentPlayer;

            if (!UserIsMember(userPlayer) || _roles != null && !UserHasAnyOneRole(userPlayer))
            {
                actionContext.Result = new UnauthorizedResult(actionContext.HttpContext);
            }
        }

        private bool UserHasAnyOneRole(PlayerDto userPlayer)
        {
            return userPlayer.Roles.Any(ur => _roles.Any(r => r == ur));
        }

        private bool UserIsMember(PlayerDto userPlayer)
        {
            return userPlayer != null;
        }
    }
}