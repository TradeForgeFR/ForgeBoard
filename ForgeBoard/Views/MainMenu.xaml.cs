using ForgeBoard.Core;
using System.Windows;
using System.Windows.Controls;

namespace ForgeBoard.Views
{
    /// <summary>
    /// Logique d'interaction pour MainMenu.xaml
    /// </summary>
    public partial class MainMenu : UserControl
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TopBar.Exit();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NinjaTraderInteractions.OpenNinjaDialog(NinjaDialog.Chart);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            NinjaTraderInteractions.OpenNinjaDialog(NinjaDialog.News);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            ((sender as Button).DataContext as BarExtension).MenuClick();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            NinjaTraderInteractions.OpenNinjaDialog(NinjaDialog.Chart);
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            NinjaTraderInteractions.OpenNinjaDialog(NinjaDialog.DynamicDOM);
        }
    }
}
