using ForgeBoard.Core;
using NinjaTrader.Cbi;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace ForgeBoard.Views.Widgets
{
    /// <summary>
    /// Logique d'interaction pour PriceWidget.xaml
    /// </summary>
    public partial class PriceWidget : UserControl
    {
        private double _last = double.NaN;
        private System.Windows.Media.Brush up, down, neutral;
        private bool _subscribed = false;
        public PriceWidget()
        {
            InitializeComponent();

            MouseLeave += (o, e) => tradingPopup.StaysOpen = false;
            selectionBorder.Visibility = Visibility.Visible;


            Color baseColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#60A917");
            // Create a Color with 50% opacity
            Color colorWithOpacity = Color.FromArgb(128, baseColor.R, baseColor.G, baseColor.B); // 128 for 50% opacity (ranges from 0 to 255)
            // Create a SolidColorBrush with the color and opacity
            up = new SolidColorBrush(colorWithOpacity);

            baseColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#B20000");
            colorWithOpacity = Color.FromArgb(128, baseColor.R, baseColor.G, baseColor.B); // 128 for 50% opacity (ranges from 0 to 255)
            down = new SolidColorBrush(colorWithOpacity);

            baseColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#0A191C"); 
            neutral = new SolidColorBrush(baseColor);

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ForgeBoardInteractions.RemovedWidgetFromBar(this);
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            selectionBorder.Visibility = Visibility.Visible;
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            var widget = new PriceWidget();
            widget.removeBTN.IsEnabled = false;
            widget.extractBTN.IsEnabled = false;
            widget.SelectedInstrument = _instrumentSelector.Instrument;
            widget.mainBorder.Margin = new Thickness(0);
            widget.selectionBorder.Margin = new Thickness(0); 

            var wnd = ForgeBoardInteractions.ExtractToTopWindow(widget);
            wnd.Closing += (oo, ee) =>
            {
                this.Visibility = Visibility.Visible;
                widget.Dispose();
            };

            this.Visibility = Visibility.Collapsed; 
        }

        private void _instrumentSelector_InstrumentChanged(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // unsuscribe the old instrument
            if (_subscribed)
                _instrumentSelector.Instrument.MarketDataUpdate -= Instrument_MarketDataUpdate;

            selectionBorder.Visibility = Visibility.Collapsed; 

            this.instrumentName.Text = _instrumentSelector.Instrument.FullName;

            bid.Text = _instrumentSelector.Instrument.MarketData?.Bid?.Price.ToString();
            ask.Text = _instrumentSelector.Instrument.MarketData?.Ask?.Price.ToString();

            _instrumentSelector.Instrument.MarketDataUpdate += Instrument_MarketDataUpdate;
            _subscribed = true;
        }

        private void Instrument_MarketDataUpdate(object sender, NinjaTrader.Data.MarketDataEventArgs e)
        {
            if(e.MarketDataType == NinjaTrader.Data.MarketDataType.Bid)
            {
                bid.Text = e.Instrument.MasterInstrument.FormatPrice(e.Bid);
            }
            else if(e.MarketDataType == NinjaTrader.Data.MarketDataType.Ask)
            {
                ask.Text = e.Instrument.MasterInstrument.FormatPrice(e.Ask);
            }
            else if( e.MarketDataType == NinjaTrader.Data.MarketDataType.Last)
            {
                if (e.Price > _last)
                    mainBorder.Background = up;
                else if (e.Price < _last)
                    mainBorder.Background = down;
                else
                    mainBorder.Background = neutral;

                _last = e.Price;
            }             
        }
          
        private void mainBoder_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.RightButton == System.Windows.Input.MouseButtonState.Pressed)
                return; 

            tradingPopup.StaysOpen = true;
            tradingPopup.IsOpen = true; 
        }

        public Instrument SelectedInstrument
        {
            get => _instrumentSelector.Instrument;
            set=> _instrumentSelector.Instrument = value;
        }

        public void Dispose()
        {
            if (_subscribed)
                _instrumentSelector.Instrument.MarketDataUpdate -= Instrument_MarketDataUpdate;
        }
    }
}
