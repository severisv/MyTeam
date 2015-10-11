using System;
using MyTeam.Models.Domain;

namespace MyTeam.Services.Domain
{
    public interface IPlayerService
    {
        void Add(string clubId, string facebookId, string firstName, string lastName);
    }
}