using System;
using System.Collections.Generic;


namespace MyTeam.ViewModels.Game
{
    public class SimpleGame
    {
        public Guid Id { get; set; }
        public string Team { get; set; }
        public string Opponent { get; set; }
        public string Name => $"{Team} vs {Opponent}";

        public SimpleGame(Guid id, string team, string opponent)
        {
            Id = id;
            Team = team;
            Opponent = opponent;
        }
    }
}