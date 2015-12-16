using System;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.AspNet.Mvc.Routing;
using MyTeam.Controllers;

namespace MyTeam.Filters
{
    public class RequireMemberAttribute : ActionFilterAttribute
    {
        private readonly string[] _roles;
        private readonly bool _allowIfSelf;
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


            if (!actionContext.HttpContext.Request.IsAjaxRequest() && userPlayer.Exists && !userPlayer.ProfileIsConfirmed)
            {
                var controller = actionContext.Controller as BaseController;
                if (controller != null)
                    actionContext.Result = controller.RedirectToAction("Edit", "Player", new {playerId = userPlayer.Id, filterRedirect = true });
            }


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