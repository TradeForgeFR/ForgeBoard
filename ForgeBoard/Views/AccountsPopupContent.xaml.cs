using ForgeBoard.ViewModels;
using System.Windows.Controls;

namespace ForgeBoard.Views
{
    /// <summary>
    /// Logique d'interaction pour AccountsPopupContent.xaml
    /// </summary>
    public partial class AccountsPopupContent : UserControl
    {
        public AccountsPopupContent()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            (DataContext as AccountsViewModel).KeepPopupOpen = !(DataContext as AccountsViewModel).KeepPopupOpen;
        }

        private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            (DataContext as AccountsViewModel).Extract();
        }
    }
}
