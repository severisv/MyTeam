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
        private bool _allowIfSelf;
        public RequireMemberAttribute(bool allowIfSelf, params string[] roles)
        {
            _roles = roles;
            _allowIfSelf = allowIfSelf;
        }
        public RequireMemberAttribute(params string[] roles) : this(false, roles){}

        public RequireMemberAttribute(){}

        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            var controller = (BaseController)actionContext.Controller;

            var userPlayer = controller.CurrentPlayer;

            if (_allowIfSelf && CurrentPageIsAboutUser(actionContext, userPlayer)) return;

            if (!UserIsMember(userPlayer) || _roles != null && !UserHasAnyOneRole(userPlayer))
            {
                actionContext.Result = new UnauthorizedResult(actionContext.HttpContext);
            }
        }

        private bool CurrentPageIsAboutUser(ActionExecutingContext actionContext, PlayerDto userPlayer)
        {
            if (userPlayer == null) return false;
            object targetPlayerId;
            actionContext.ActionArguments.TryGetValue("playerId", out targetPlayerId);
            if ((targetPlayerId as Guid?) != userPlayer.Id) return false;
            return true;

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