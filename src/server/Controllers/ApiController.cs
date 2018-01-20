using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MyTeam.Filters;
using MyTeam.Models;
using MyTeam.Models.Enums;
using MyTeam.Models.Structs;
using MyTeam.Services.Application;
using MyTeam.Services.Domain;


namespace MyTeam.Controllers
{
  [RequireMember]
  public class ApiController : BaseController
  {

    private readonly ApplicationDbContext _dbContext;
    private readonly IEventService _eventService;
    private readonly IPlayerService _playerService;
    private readonly ICacheHelper _cacheHelper;

    public ApiController(ApplicationDbContext dbContext, IEventService eventService, IPlayerService playerService, ICacheHelper cacheHelper)
    {
      _playerService = playerService;
      _eventService = eventService;
      _dbContext = dbContext;
      _cacheHelper = cacheHelper;
    }


    [HttpPost]
    [RequireMember(Roles.Coach, Roles.Admin)]
    public JsonResult TogglePlayerRole(Guid id, string role)
    {
      if (ModelState.IsValid)
      {
        _playerService.TogglePlayerRole(id, role, Club.Id);
        _cacheHelper.ClearMemberCache(id);
        var reponse = new { Success = true };
        return new JsonResult(reponse);
      }

      var validationMessage = string.Join(" ,", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
      return new JsonResult(JsonResponse.ValidationFailed(validationMessage));
    }


    public JsonResult GetTeams()
    {
      var teams = _dbContext.Teams
          .OrderBy(c => c.SortOrder)
          .Where(c => c.ClubId == Club.Id).Select(t => new
          {
            Id = t.Id,
            ShortName = t.ShortName
          }
          );
      return new JsonResult(new { data = teams });
    }

    [HttpPost]
    [RequireMember(Roles.Coach, Roles.Admin)]
    public JsonResult TogglePlayerTeam(Guid teamId, Guid playerId)
    {
      if (ModelState.IsValid)
      {
        _playerService.TogglePlayerTeam(teamId, playerId, Club.Id);
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
