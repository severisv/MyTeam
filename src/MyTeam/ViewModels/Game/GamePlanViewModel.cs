using System;

namespace MyTeam.ViewModels.Game
{
    public class GamePlanViewModel
    {
        public Guid GameId { get; }
        public string GamePlan { get; }
        public string Team { get; }
        public string Opponent { get; }
        public bool IsPublished { get; }

        public GamePlanViewModel(Guid gameId, string team, string opponent, string gamePlan, bool? isPublished)
        {
            GameId = gameId;
            GamePlan = gamePlan;
            Team = team;
            Opponent = opponent;
            IsPublished = isPublished ?? false;
        }
    }
}