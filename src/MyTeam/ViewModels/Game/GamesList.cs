using System;
using System.Collections.Generic;
using System.Linq;
using MyTeam.Models.Enums;

namespace MyTeam.ViewModels.Game
{
    public class GamesList
    {
        private readonly Guid _teamId;
        private readonly string _gamesString;
        private readonly string _teamName;
        private readonly GameType _gameType;
        public List<ParsedGame> Games {
            get
            {
                try
                {
                    return ParseGames(_gamesString);

                }
                catch (Exception e)
                {
                    throw new ArgumentException("Invalid string, could not create Games object", nameof(_gamesString), e);
                }
            }
        }


        public GamesList(Guid teamId, string teamName, string gamesString, GameType gameType)
        {
            _teamName = teamName;
            _gamesString = gamesString;
            _teamId = teamId;
            _gameType = gameType;
        }



        private List<ParsedGame> ParseGames(string tableString)
        {
            var table = tableString.Split('\n');

            return table.Select(line => new ParsedGame(_teamId, _teamName, _gameType, line)).Where(game => game.IsValid).ToList();
        }
    }
}