using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Routing;
using MyTeam.Models.Domain;
using MyTeam.Models.Dto;

namespace MyTeam.Services.Application
{
    public interface IMemoryStore
    {
        PlayerDto GetPlayerFromUser(string name);
        ClubDto GetCurrentClub(string clubId);
    }
}