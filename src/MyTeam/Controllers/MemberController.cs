using System.Linq;
using Microsoft.AspNet.Mvc;
using MyTeam.Models.Domain;
using MyTeam.Resources;
using MyTeam.Services.Repositories;
using MyTeam.ViewModels.Member;

namespace MyTeam.Controllers
{
    public class MemberController : BaseController
    {
        [FromServices]
        public IRepository<Player> PlayerRepository { get; set; }


        public IActionResult Index()
        {

            ViewBag.PageName = Res.SquadList;

            var players = PlayerRepository.Get().Where(p => p.Club.ClubId == Context.GetClub().ClubId).Select(p => new MemberInfoViewModel{
                Id = p.Id,
                Name = p.Fullname,
                Status = p.Status,
                ImageSmall = p.ImageSmall
            });

            return View("ListMembers", players);
        }

    
    }
}