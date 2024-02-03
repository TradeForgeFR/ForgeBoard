using ForgeBoard.Core.ViewModels;
using ForgeBoard.Views;
using NinjaTrader.Cbi;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ForgeBoard.ViewModels
{
    public class AccountsViewModel : ViewModelBase
    {
        private double _uunrealizedPnL, _realizedPnL= 0;
        private bool _keepPopupOpen = false;
        public ObservableCollection<AccountItemViewModel> Accounts { get; set; } = new ObservableCollection<AccountItemViewModel>();

        public void Init()
        {
            foreach (var accountItem in Account.All)
            {
                var accountViewModel = new AccountItemViewModel(accountItem);
                Accounts.Add(accountViewModel);
                accountViewModel.AccountUpdated += AccountViewModel_AccountUpdated;
            }

            UnrealizedPnL = Accounts.Select(x => x.UnrealizedPnL).Sum();
            RealizedPnL = Accounts.Select(x => x.RealizedPnL).Sum();
        }

        private void AccountViewModel_AccountUpdated()
        {
            lock (this.Accounts)
            {
                UnrealizedPnL = Accounts.Select(x => x.UnrealizedPnL).Sum();
                RealizedPnL = Accounts.Select(x => x.RealizedPnL).Sum();
            }
        }
        public double UnrealizedPnL
        {
            get => Math.Round(_uunrealizedPnL, 2);
            set
            {
                _uunrealizedPnL = value;
                this.OnPropertyChanged(nameof(Sum));
                this.OnPropertyChanged(nameof(UnrealizedPnL));
            }
        }

        public bool KeepPopupOpen
        {
            get { return _keepPopupOpen; }
            set
            {
                _keepPopupOpen = value;
                OnPropertyChanged(nameof(KeepPopupOpen));
            }
        }
        public double RealizedPnL
        {
            get => Math.Round(_realizedPnL, 2);
            set
            {
                _realizedPnL = value;
                this.OnPropertyChanged(nameof(Sum));
                this.OnPropertyChanged(nameof(RealizedPnL));
            }
        }

        public double Sum
        {
            get => Math.Round(RealizedPnL + UnrealizedPnL, 2);
        }

        public void Extract()
        {
            var content = new AccountsPopupContent()
            {
                DataContext = this,
            };

            content.actionButtons.Visibility = Visibility.Collapsed;
            content.closeWNDButton.Visibility = Visibility.Visible;
            var wnd = new Window()
            {
                WindowStyle = WindowStyle.None,
                ResizeMode = ResizeMode.NoResize,
                SizeToContent = SizeToContent.WidthAndHeight,
                Content = content,
                Topmost = true,
                ShowInTaskbar = false,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            content.topBorder.MouseDown += (o, e) =>
            {
                wnd.DragMove();
            };
            content.closeWNDButton.Click += (o, e) =>
            {
                wnd.Close();
            };

            wnd.Show();
        }

        public void Dispose()
        {
            foreach(var account in this.Accounts)
            {
                account.Dispose();
            }
        }
    }
}
