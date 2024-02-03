using ForgeBoard.Core;
using ForgeBoard.Core.ViewModels;
using NinjaTrader.Cbi;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ForgeBoard.ViewModels
{
    public class AccountItemViewModel : ViewModelBase
    {
        private Account _account;
        private double _unrealizedPnL = 0, _realizedPnl = 0;

        public ObservableCollection<PositionViewModel> Positions { get; set; } = new ObservableCollection<PositionViewModel>();
        public BasicCommand FlatCommand { get; }
        public AccountItemViewModel(Account account)
        {
            _account = account;

            FlatCommand = new BasicCommand(() =>
            {
                var validation = MessageBox.Show(string.Format("Are you sure ou want to flat the account {0}", _account.DisplayName), "Flat account", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if(validation == MessageBoxResult.Yes)
                {
                    _account.Flatten(_account.Positions.Select(x => x.Instrument).ToArray());
                }
            });

            Init();
        }

        private void Init()
        {
            foreach(var position in _account.Positions)
            {
                Positions.Add(new PositionViewModel(position, _account));
            }

            UnrealizedPnL = _account.Positions.Select(x => x.GetUnrealizedProfitLoss(PerformanceUnit.Currency)).Sum();
            RealizedPnL   = _account.Get(AccountItem.RealizedProfitLoss, Currency.UsDollar);
            Visibility    = _account.Positions.Count >= 1 ? Visibility.Visible : Visibility.Collapsed;

            _account.AccountItemUpdate += _account_AccountItemUpdate;
            _account.PositionUpdate += _account_PositionUpdate;
        }

        private  void _account_PositionUpdate(object sender, PositionEventArgs e)
        {
            try
            { 
                if (e.Operation == Operation.Add)
                {
                    ForgeBoardInteractions.BarDispatcher.Invoke((Action)delegate
                    {
                        Positions.Add(new PositionViewModel(e.Position, _account));
                    });
                }
                else if (e.Operation == Operation.Remove)
                {
                    ForgeBoardInteractions.BarDispatcher.Invoke((Action)delegate
                    {
                        var toRemove = Positions.First(x => x.Instrument == e.Position.Instrument.FullName);
                        Positions.Remove(toRemove); 
                    });
                     
                } 
            }
            catch(Exception ex)
            {
                NinjaTraderInteractions.PrintToOutput(ex.Message);
            }
        }

        private void _account_AccountItemUpdate(object sender, AccountItemEventArgs e)
        {
            if(e.AccountItem == AccountItem.UnrealizedProfitLoss)
            {
                UnrealizedPnL = e.Value;
                RealizedPnL = _account.Get(AccountItem.RealizedProfitLoss, Currency.UsDollar);
            } 

            Visibility = _account.Positions.Count >= 1 ? Visibility.Visible : Visibility.Collapsed;
            OnPropertyChanged(nameof(Visibility));

            AccountUpdated?.Invoke();
        }

        public Account Account { get { return _account; } }

        public double UnrealizedPnL
        {
            get { return Math.Round(_unrealizedPnL, 2); }
            set 
            { 
                _unrealizedPnL = value;
                OnPropertyChanged(nameof(UnrealizedPnL));
            }
        }

        public double RealizedPnL
        {
            get { return Math.Round(_realizedPnl, 2); }
            set
            {
                _realizedPnl = value;
                OnPropertyChanged(nameof(RealizedPnL));
            }
        }

        public Visibility Visibility { get; set; } = Visibility.Collapsed;

        public void Dispose()
        {
            _account.AccountItemUpdate -= _account_AccountItemUpdate;
            _account.PositionUpdate    -= _account_PositionUpdate;

            foreach(var item in Positions)
            {
                item.Dispose(); 
            }
        } 

        public event AccountUpdatedHandler AccountUpdated;
    }

    public delegate void AccountUpdatedHandler();
}
