using ForgeBoard.Core.ViewModels;
using NinjaTrader.Cbi;
using System;

namespace ForgeBoard.ViewModels
{
    public class PositionViewModel : ViewModelBase
    {
        private Position _position;
        private Account _account;
        private double _averagePrice =0, _size =0, _pnl=0;
        public PositionViewModel(Position position, Account account)
        {
            _position = position;
            _account = account;
            _account.AccountItemUpdate += _account_AccountItemUpdate;

            CloseCommand = new BasicCommand(() =>
            {
                position.Close();
            });

            ReverseCommand = new BasicCommand(() =>
            {
                position.Reverse(TimeInForce.Gtc, DateTime.MaxValue);
            });
        }

        private void _account_AccountItemUpdate(object sender, AccountItemEventArgs e)
        {
            OnPropertyChanged(nameof(Price));
            OnPropertyChanged(nameof(PnL));
            OnPropertyChanged(nameof(Size));
        }

        public string Instrument
        {
            get => _position.Instrument.FullName;
        }

        public double Price
        {
            get => _position.AveragePrice;
        }

        public double Size
        {
            get => _position.Quantity;
        }

        public double PnL
        {
            get => Math.Round(_position.GetUnrealizedProfitLoss(PerformanceUnit.Currency), 2);
        }

        public BasicCommand CloseCommand { get; }
        public BasicCommand ReverseCommand { get; }
        public void Dispose()
        {
            _account.AccountItemUpdate -= _account_AccountItemUpdate;
        }
    }
}
