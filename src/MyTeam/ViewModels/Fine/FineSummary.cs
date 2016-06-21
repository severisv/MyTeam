using System;
using System.Linq;

namespace MyTeam.ViewModels.Fine
{
    public class FineSummary
    {
        private readonly IGrouping<Guid, FineViewModel> _playerFines;
        private FineViewModel _player => _playerFines.First();
        public string Name => _player.Name;
        public string FacebookId => _player.FacebookId;
        public string Image => _player.MemberImage;
        public Guid MemberId => _player.MemberId;

        public string PlayerImage => _playerFines.First().MemberImage;

        public double Total => _playerFines.Sum(p => p.Rate);

        public double Due => _playerFines.Where(f => !f.IsPaid).Sum(p => p.Rate);

        public FineSummary(IGrouping<Guid, FineViewModel> playerFines)
        {
            _playerFines = playerFines;
        }
    }
}