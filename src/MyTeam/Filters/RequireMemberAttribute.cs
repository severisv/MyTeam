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
            var userPlayer = actionContext.HttpContext.Member();

            if (_allowIfSelf && CurrentPageIsAboutUser(actionContext, userPlayer)) return;

            if (!userPlayer.Exists || _roles != null && !userPlayer.Roles.ContainsAny(_roles))
            {
                actionContext.Result = new UnauthorizedResult(actionContext.HttpContext);
            }
        }

        private bool CurrentPageIsAboutUser(ActionExecutingContext actionContext, UserMember userPlayer)
        {
            object targetPlayerId;
            actionContext.ActionArguments.TryGetValue("playerId", out targetPlayerId);
            if (userPlayer.Id == (targetPlayerId as Guid?)) return true;
            return false;

        }


    }
}