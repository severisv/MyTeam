using System.Linq;
using Microsoft.AspNet.Mvc;
using MyTeam.Filters;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;
using MyTeam.Resources;
using MyTeam.Services.Repositories;
using MyTeam.ViewModels.Member;

namespace MyTeam.Controllers
{
    [RequireMember]
    public class MemberController : BaseController
    {
        [FromServices]
        public IRepository<Player> PlayerRepository { get; set; }


        public IActionResult Index(PlayerStatus status = PlayerStatus.Aktiv)
        {

            ViewBag.Title = Res.SquadList;

            var players = PlayerRepository.Get()
                .Where(p => p.Club.ClubId == Context.GetClub().ClubId)
                .Where(p => p.Status == status)
                .OrderBy(p => p.FirstName)
                .Select(p => new MemberInfoViewModel{
                Id = p.Id,
                Name = p.Name,
                Status = p.Status,
                ImageSmall = p.ImageSmall,
                Phone = p.Phone,
                Email = p.Email
            });
            var model = new MemberListViewModel(players, status);
            return View("ListMembers", model);
        }

    
    }
}