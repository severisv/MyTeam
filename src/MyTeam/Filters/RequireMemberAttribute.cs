﻿using System;
using Microsoft.AspNetCore.Mvc.Filters;
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
                actionContext.Result = new Extensions.Mvc.UnauthorizedResult(actionContext.HttpContext);
            }

            
        }

        private bool CurrentPageIsAboutUser(ActionExecutingContext actionContext, UserMember userPlayer)
        {
            var targetPlayerId = actionContext.HttpContext.Request.Query["playerId"];

            if (string.IsNullOrWhiteSpace(targetPlayerId))
                targetPlayerId = actionContext.HttpContext.Request.Form["PlayerId"];

            Guid playerId;
            Guid.TryParse(targetPlayerId, out playerId);

            if (playerId == userPlayer.Id) return true;
            
            return false;

        }


    }
}