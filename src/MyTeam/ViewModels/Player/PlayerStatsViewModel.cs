using System;
using System.Collections.Generic;
using System.Linq;
using MyTeam.Models.Enums;
using MyTeam.ViewModels.Game;

namespace MyTeam.ViewModels.Player
{
    public class PlayerStatsViewModel
    {
        public Guid PlayerId { get; }
        public Guid TeamId { get; }
        public int Year { get; }

        public IEnumerable<GameEventViewModel> GameEvents { get; }

        public int GameCount { get; }
        public int GoalCount => GameEvents.Count(g => g.PlayerId == PlayerId && g.Type == GameEventType.Goal);
        public int AssistCount => GameEvents.Count(g => g.AssistedById == PlayerId);
        public int YellowCards => GameEvents.Count(g => g.PlayerId == PlayerId && g.Type == GameEventType.YellowCard);
        public int RedCards => GameEvents.Count(g => g.PlayerId == PlayerId && g.Type == GameEventType.RedCard);

        public PlayerStatsViewModel(Guid playerId, Guid teamId, IEnumerable<GameEventViewModel> gameEvents, int gameAttendances, int year)
        {
            PlayerId = playerId;
            GameEvents =
                gameEvents ??
                Enumerable.Empty<GameEventViewModel>();
            TeamId = teamId;
            GameCount = gameAttendances;
            Year = year;
        }

    }
}