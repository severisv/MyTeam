using System.Linq;
using Microsoft.AspNet.Mvc;
using MyTeam.Filters;
using MyTeam.Models;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;
using MyTeam.Resources;
using MyTeam.Services.Repositories;
using MyTeam.ViewModels.Member;

namespace MyTeam.Controllers
{
    [RequireMember]
    [Route("intern")]
    public class MemberController : BaseController
    {
        [FromServices]
        public ApplicationDbContext DbContext { get; set; }

        [Route("lagliste")]
        public IActionResult Index(PlayerStatus status = PlayerStatus.Aktiv)
        {

            ViewBag.Title = Res.SquadList;

            var players = DbContext.Players
                .Where(p => p.Club.ClubIdentifier == HttpContext.GetClub().ClubId)
                .Where(p => p.Status == status)
                .OrderBy(p => p.FirstName)
                .Select(p => new MemberInfoViewModel{
                    Id = p.Id,
                    Name = p.Name,
                    Status = p.Status,
                    Image = p.ImageFull,
                    Phone = p.Phone,
                    Email = p.Email,
                    FacebookId = p.FacebookId
                });
            var model = new MemberListViewModel(players, status);
            return View("ListMembers", model);
        }

    
    }
}