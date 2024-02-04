using ForgeBoard.Core;
using ForgeBoard.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace ForgeBoard.Views.Widgets
{
    /// <summary>
    /// Logique d'interaction pour PriceWidget.xaml
    /// </summary>
    public partial class PriceWidget : UserControl
    {
        public PriceWidget()
        {
            InitializeComponent();

            MouseLeave += (o, e) => tradingPopup.StaysOpen = false;
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            (DataContext as InstrumentViewModel).SelectionBorderVisbility = Visibility.Visible;
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            var dataContext = new InstrumentViewModel();
            var widget = new PriceWidget()
            {
                DataContext = dataContext
            };

            widget.removeBTN.IsEnabled = false;
            widget.extractBTN.IsEnabled = false;
            dataContext.Instrument = _instrumentSelector.Instrument;
            widget.mainBorder.Margin = new Thickness(0);
            widget.selectionBorder.Margin = new Thickness(0);

            var wnd = ForgeBoardInteractions.ExtractToTopWindow(widget);
            wnd.Closing += (oo, ee) =>
            {
                this.Visibility = Visibility.Visible;
                dataContext.Dispose();
            };

            this.Visibility = Visibility.Collapsed;
        }

        private void mainBoder_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.RightButton == System.Windows.Input.MouseButtonState.Pressed)
                return;

            tradingPopup.StaysOpen = true;
            tradingPopup.IsOpen = true;
        }
    }
}
