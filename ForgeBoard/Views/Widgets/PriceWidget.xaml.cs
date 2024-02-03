using ForgeBoard.Core;
using ForgeBoard.Core.Models;
using ForgeBoard.Core.Views;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.SuperDom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;
using Color = System.Windows.Media.Color;

namespace ForgeBoard.Views.Widgets
{
    /// <summary>
    /// Logique d'interaction pour PriceWidget.xaml
    /// </summary>
    public partial class PriceWidget : UserControl
    {
        private double _last = double.NaN;
        private System.Windows.Media.Brush up, down;
        private bool _subscribed = false;
        private Instrument _instrument = null;
        private List<SparkChartPoint> stockData;
        public PriceWidget()
        {
            InitializeComponent();

            MouseLeave += (o, e) => tradingPopup.StaysOpen = false;
            selectionBorder.Visibility = Visibility.Visible;


            Color baseColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#60A917");
            up = new SolidColorBrush(baseColor);

            baseColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#E51400");
            down = new SolidColorBrush(baseColor);
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
            try
            {
                if (_subscribed && _instrument != null)
                    _instrument.MarketDataUpdate -= Instrument_MarketDataUpdate;

                _instrument = _instrumentSelector.Instrument;
                selectionBorder.Visibility = Visibility.Collapsed;

                this.instrumentName.Text = _instrument.FullName;

                // bid.Text = _instrument.MasterInstrument.FormatPrice(_instrument.MarketData.Bid.Bid, false);
                // ask.Text = _instrument.MasterInstrument.FormatPrice(_instrument.MarketData.Ask.Ask, false);

                _instrument.MarketDataUpdate += Instrument_MarketDataUpdate;
                _subscribed = true;
                RequestBars();
            }
            catch (Exception ex)
            {
                NinjaTraderInteractions.PrintToOutput(ex.Message + Environment.NewLine + ex.Data + Environment.NewLine + ex.StackTrace + Environment.NewLine + ex.Source);
            }
        }

        private void Instrument_MarketDataUpdate(object sender, NinjaTrader.Data.MarketDataEventArgs e)
        {
            if (e.MarketDataType == NinjaTrader.Data.MarketDataType.Bid && e.Bid > 0)
            {
                bid.Text = e.Instrument.MasterInstrument.FormatPrice(e.Bid, false);
            }
            else if (e.MarketDataType == NinjaTrader.Data.MarketDataType.Ask && e.Ask > 0)
            {
                ask.Text = e.Instrument.MasterInstrument.FormatPrice(e.Ask, false);
            }
            else if (e.MarketDataType == NinjaTrader.Data.MarketDataType.Last)
            {
                if (e.Price >= e.Ask)
                {
                    askBorder.Background = up;
                    bidBorder.Background = up;
                    mainBoder.BorderBrush = up;
                }
                else if (e.Price <= e.Bid)
                {
                    askBorder.Background = down;
                    bidBorder.Background = down;
                    mainBoder.BorderBrush = down;
                }
            }
        }

        private void RequestBars()
        {
            BarsPeriod bp = new BarsPeriod
            {
                MarketDataType = MarketDataType.Last,
                BarsPeriodType = BarsPeriodType.Minute,
                Value = 60
            };
            var BarsRequest = new BarsRequest(_instrument, 24);
            BarsRequest.BarsPeriod = bp;

            BarsRequest.Request((request, errorCode, errorMessage) =>
            {
                if (errorCode == ErrorCode.NoError)
                {
                    sparkChart.StockDatas.Clear();
                    for (int i = 0; i < request.Bars.Count - 1; i++)
                    {
                        sparkChart.StockDatas.Add(new SparkChartPoint()
                        {
                            Date = request.Bars.BarsSeries.GetTime(i),
                            Price = request.Bars.BarsSeries.GetClose(i)
                        });
                    }

                    ForgeBoardInteractions.BarDispatcher.Invoke((Action)delegate
                    {
                        sparkChart.DrawStockAreaChart();
                    });

                    NinjaTraderInteractions.PrintToOutput(string.Format("Successfully got the historical data for {0}", _instrument.FullName));
                }
                else
                {
                    NinjaTraderInteractions.PrintToOutput(errorMessage);
                }
            });
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
            set => _instrumentSelector.Instrument = value;
        }

        public void Dispose()
        {
            if (_subscribed && _instrument != null)
                _instrument.MarketDataUpdate -= Instrument_MarketDataUpdate;
        }  
    }
}
