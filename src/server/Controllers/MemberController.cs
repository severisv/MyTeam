using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MyTeam.Filters;
using MyTeam.Models;
using MyTeam.Models.Enums;
using MyTeam.Resources;
using MyTeam.ViewModels.Member;

namespace MyTeam.Controllers
{
    [RequireMember]
    [Route("intern")]
    public class MemberController : BaseController
    {
        private readonly ApplicationDbContext _dbContext;

        public MemberController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Route("lagliste")]
        public IActionResult Index(PlayerStatus status = PlayerStatus.Aktiv)
        {

            ViewBag.Title = Res.SquadList;

            var players = _dbContext.Players
                .Where(p => p.ClubId == HttpContext.GetClub().Id)
                .Where(p => p.Status == status)
                .OrderBy(p => p.FirstName)
                .Select(p => new MemberInfoViewModel{
                    Id = p.Id,
                    Name = p.Name,
                    Status = p.Status,
                    Image = p.ImageFull,
                    Phone = p.Phone,
                    Email = p.Email,
                    FacebookId = p.FacebookId,
                    BirthDate = p.BirthDate,
                    UrlName = p.UrlName
                });
            var model = new MemberListViewModel(players, status);
            return View("ListMembers", model);
        }

    
    }
}