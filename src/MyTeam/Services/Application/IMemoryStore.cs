using MyTeam.Models.Domain;

namespace MyTeam.Services.Application
{
    public interface IMemoryStore
    {
        Player GetPlayerFromUser(string name);
    }
}