using System;
using System.Collections.Generic;
using System.Linq;
using MyTeam.ViewModels.Payment;

namespace MyTeam.ViewModels.Fine
{
    public class FineSummary
    {
        private readonly IGrouping<Guid, FineViewModel> _playerFines;
        private readonly IEnumerable<PaymentViewModel> _playerPayments;
        private FineViewModel _player => _playerFines.First();
        public string Name => _player.Name;
        public string FacebookId => _player.FacebookId;
        public string Image => _player.MemberImage;
        public Guid MemberId => _player.MemberId;

        public string PlayerImage => _playerFines.First().MemberImage;

        public double Total => _playerFines.Sum(p => p.Rate);

        public double Due => _playerFines.Sum(p => p.Rate) - _playerPayments.Sum(p => p.Amount);
            

        public FineSummary(IGrouping<Guid, FineViewModel> playerFines, IEnumerable<PaymentViewModel> playerPayments)
        {
            _playerFines = playerFines;
            _playerPayments = playerPayments;
        }
    }
}