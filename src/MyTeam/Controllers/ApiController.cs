using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
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
     
        private readonly ApplicationDbContext _dbContext;
        private readonly IEventService _eventService;
        private readonly IPlayerService _playerService;

        public ApiController(ApplicationDbContext dbContext, IEventService eventService, IPlayerService playerService)
        {
            _playerService = playerService;
            _eventService = eventService;
            _dbContext = dbContext;
        }


        public JsonResult GetPlayers()
        {
            var players = _dbContext.Players.Where(p => p.ClubId == Club.Id)
                .Select(p =>
              new
              {
                  Id = p.Id,
                  FullName = p.Name,
                  FirstName = p.FirstName,
                  MiddleName = p.MiddleName,
                  LastName = p.LastName,
                  Status = p.Status.ToString(),
                  Roles = p.Roles,
              }).OrderBy(p => p.FullName).ToList();

            var playerIds = players.Select(p => p.Id);
            var memberTeams = _dbContext.MemberTeams.Where(mt => playerIds.Contains(mt.MemberId)).ToList();

            var result = new List<Object>();
            foreach (var p in players)
            {
                result.Add(new
                {
                    p.Id,
                    p.FullName,
                    p.FirstName,
                    p.MiddleName,
                    p.LastName,
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
            var ids = _playerService.GetFacebookIds();
            return new JsonResult(new { data = ids });
        }

        [HttpPost]
        [RequireMember(Roles.Coach, Roles.Admin)]
        public JsonResult SetPlayerStatus(Guid id, PlayerStatus status)
        {
            if (ModelState.IsValid)
            {
                _playerService.SetPlayerStatus(id, status, Club.ClubId);
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
                _playerService.TogglePlayerRole(id, role, Club.ClubId);
                var reponse = new {Success = true};
                return new JsonResult(reponse);
            }

            var validationMessage = string.Join(" ,", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return new JsonResult(JsonResponse.ValidationFailed(validationMessage));
        }


        public JsonResult GetTeams()
        {
            var teams = _dbContext.Teams
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
                _playerService.TogglePlayerTeam(teamId, playerId, Club.ClubId);
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
                _eventService.UpdateDescription(eventId, description);
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

    
        [RequireMember(Roles.Coach, Roles.Admin)]
        public IActionResult TestException()
        {
            throw new Exception("Boom");
        }
    }
}
