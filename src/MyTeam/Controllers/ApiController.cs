using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using MyTeam.Filters;
using MyTeam.Models;
using MyTeam.Models.Enums;
using MyTeam.Models.Structs;
using MyTeam.Services.Domain;


namespace MyTeam.Controllers
{
    [RequireMember]
    public class ApiController : BaseController
    {
        [FromServices]
        public IPlayerService PlayerService { get; set; }
        [FromServices]
        public IEventService EventService { get; set; }
        [FromServices]
        public ApplicationDbContext DbContext { get; set; }
        
        public JsonResult GetPlayers()
        {
            var players = DbContext.Players.Where(p => p.Club.Id == Club.Id)
                .Select(p =>
              new
              {
                  Id = p.Id,
                  FullName = p.Name,
                  Status = p.Status.ToString(),
                  Roles = p.Roles,
              }).OrderBy(p => p.FullName).ToList();

            var playerIds = players.Select(p => p.Id);
            var memberTeams = DbContext.MemberTeams.Where(mt => playerIds.Contains(mt.MemberId)).ToList();

            var result = new List<Object>();
            foreach (var p in players)
            {
                result.Add(new
                {
                    p.Id,
                    p.FullName,
                    p.Status,
                    p.Roles,
                    TeamIds = memberTeams.Where(mt => mt.MemberId == p.Id).Select(mt => mt.TeamId).ToList()
            });
                    
            }

            return new JsonResult(result);
        }

        [RequireMember(Roles.Coach, Roles.Admin)]
        public JsonResult GetFacebookIds()
        {
            var ids = PlayerService.GetFacebookIds();
            return new JsonResult(new { data = ids });
        }

        [HttpPost]
        [RequireMember(Roles.Coach, Roles.Admin)]
        public JsonResult SetPlayerStatus(Guid id, PlayerStatus status)
        {
            if (ModelState.IsValid)
            {
                PlayerService.SetPlayerStatus(id, status, Club.ClubId);
                var reponse = new {Success = true};
                return new JsonResult(reponse);
            }

            var validationMessage = string.Join(" ,", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return new JsonResult(JsonResponse.ValidationFailed(validationMessage));
        }

        [HttpPost]
        [RequireMember(Roles.Coach, Roles.Admin)]
        public JsonResult TogglePlayerRole(Guid id, string role)
        {
            if (ModelState.IsValid)
            {
                PlayerService.TogglePlayerRole(id, role, Club.ClubId);
                var reponse = new {Success = true};
                return new JsonResult(reponse);
            }

            var validationMessage = string.Join(" ,", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return new JsonResult(JsonResponse.ValidationFailed(validationMessage));
        }


        public JsonResult GetTeams()
        {
            var teams = DbContext.Teams
                .OrderBy(c => c.SortOrder)
                .Where(c => c.ClubId == Club.Id).Select(t => new {
                Id = t.Id,
                ShortName = t.ShortName}
                );
            return new JsonResult(new { data = teams });
        }

        [HttpPost]
        [RequireMember(Roles.Coach, Roles.Admin)]
        public JsonResult TogglePlayerTeam(Guid teamId, Guid playerId)
        {
            if (ModelState.IsValid)
            {
                PlayerService.TogglePlayerTeam(teamId, playerId, Club.ClubId);
                var reponse = new { Success = true };
                return new JsonResult(reponse);
            }

            var validationMessage = string.Join(" ,", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return new JsonResult(JsonResponse.ValidationFailed(validationMessage));
        }

        [HttpPost]
        [RequireMember(Roles.Coach, Roles.Admin)]
        public JsonResult UpdateEventDescription(Guid eventId, string description)
        {
            if (ModelState.IsValid)
            {
                EventService.UpdateDescription(eventId, description);
                var reponse = new { Success = true };
                return new JsonResult(reponse);
            }

            var validationMessage = string.Join(" ,", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return new JsonResult(JsonResponse.ValidationFailed(validationMessage));
        }

        [RequireMember(Roles.Coach, Roles.Admin)]
        public IActionResult Now()
        {
                return Json(new
                {
                    DateTime.Now,
                    Date = DateTime.Now.ToShortDateString(),
                    Time = DateTime.Now.ToShortTimeString()
                });
        }

    }
}
