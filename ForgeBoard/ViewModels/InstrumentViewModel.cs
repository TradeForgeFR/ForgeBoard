using ForgeBoard.Core;
using ForgeBoard.Core.Models;
using ForgeBoard.Core.ViewModels;
using ForgeBoard.Core.Views;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.NinjaScript.AtmStrategy;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ForgeBoard.ViewModels
{
    public class InstrumentViewModel : ViewModelBase
    {
        private double _lastPrice = double.NaN;
        private System.Windows.Media.Brush _upBrush, _donwBrush, _askBrush, _bidBrush, _lastBrush;
        private bool _subscribed = false;
        private Instrument _instrument = null;
        private List<SparkChartPoint> stockData;
        private string _bid = "Bid", _ask = "Ask", _open, _high, _low, _last;
        private Account _account;
        private int _lot = 1;
        private Visibility _selectionBorderVisibility = Visibility.Visible;
        private AtmStrategySelector _strategySelector;
        public InstrumentViewModel()
        {
            _strategySelector = new AtmStrategySelector();
            Color baseColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#60A917");
            _upBrush = new SolidColorBrush(baseColor);

            baseColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#E51400");
            _donwBrush = new SolidColorBrush(baseColor);

            BuyCommand = new BasicCommand(() =>
            {
                NinjaTraderInteractions.SendMarketOrder(true, _lot, _instrument, _account, _strategySelector.SelectedAtmStrategy);
            });

            SellCommand = new BasicCommand(() =>
            {
                NinjaTraderInteractions.SendMarketOrder(false, _lot, _instrument, _account, _strategySelector.SelectedAtmStrategy);
            });
        }

        private void UpdateInstrument(Instrument instrument)
        {
            // unsuscribe the old instrument
            try
            {
                if (_subscribed && _instrument != null)
                    _instrument.MarketDataUpdate -= Instrument_MarketDataUpdate;

                _instrument = instrument;
                SelectionBorderVisbility = Visibility.Collapsed;

                _instrument.MarketDataUpdate += Instrument_MarketDataUpdate;
                
                _subscribed = true;
                RequestBars();
            }
            catch (Exception ex)
            {
                NinjaTraderInteractions.PrintToOutput(ex.Message + Environment.NewLine + ex.Data + Environment.NewLine + ex.StackTrace + Environment.NewLine + ex.Source);
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
                    Chart.StockDatas.Clear();
                    for (int i = 0; i < request.Bars.Count - 1; i++)
                    {
                        Chart.StockDatas.Add(new SparkChartPoint()
                        {
                            Date = request.Bars.BarsSeries.GetTime(i),
                            Price = request.Bars.BarsSeries.GetClose(i)
                        });
                    }

                    ForgeBoardInteractions.BarDispatcher.Invoke((Action)delegate
                    {
                        Chart.DrawStockAreaChart();
                    });

                    NinjaTraderInteractions.PrintToOutput(string.Format("Successfully got the historical data for {0}", _instrument.FullName));
                }
                else
                {
                    NinjaTraderInteractions.PrintToOutput(errorMessage);
                }
            });
        }
        private void Instrument_MarketDataUpdate(object sender, NinjaTrader.Data.MarketDataEventArgs e)
        {
            if (e.MarketDataType == NinjaTrader.Data.MarketDataType.Bid && e.Bid > 0)
            {
                Bid = e.Instrument.MasterInstrument.FormatPrice(e.Bid, false);
            }
            else if (e.MarketDataType == NinjaTrader.Data.MarketDataType.Ask && e.Ask > 0)
            {
                Ask = e.Instrument.MasterInstrument.FormatPrice(e.Ask, false);
            }
            else if (e.MarketDataType == NinjaTrader.Data.MarketDataType.Last)
            {
                if (e.Price >= e.Ask)
                {
                   AskBrush= _upBrush;
                   BidBrush = _upBrush; 
                }
                else if (e.Price <= e.Bid)
                {
                    AskBrush = _donwBrush;
                    BidBrush = _donwBrush;
                }

                LastBrush = e.Price >= _lastPrice ? _upBrush : _donwBrush;
                _lastPrice = e.Price;
                Last = e.Instrument.MasterInstrument.FormatPrice(e.Price, false);
            }
            else if(e.MarketDataType == MarketDataType.Opening)
            {
                Open = e.Instrument.MasterInstrument.FormatPrice(e.Price, false);
            }
            else if(e.MarketDataType == MarketDataType.DailyHigh)
            {
                High = e.Instrument.MasterInstrument.FormatPrice(e.Price, false);
            }
            else if(e.MarketDataType == MarketDataType.DailyLow)
            {
                Low = e.Instrument.MasterInstrument.FormatPrice(e.Price, false);
            }
        }

        public void Dispose()
        {
            if (_subscribed && _instrument != null)
                _instrument.MarketDataUpdate -= Instrument_MarketDataUpdate;

            NinjaTraderInteractions.PrintToOutput(string.Format("Disposing price widget for {0}", _instrument.FullName));
        }  
        #region public fieals

        public Instrument Instrument
        {
            get
            {
                return _instrument;
            }
            set
            { 
                if(value != null)
                {
                    UpdateInstrument(value);
                } 
                OnPropertyChanged(nameof(Instrument));
            }
        }

        public SparkChart Chart { get; } = new SparkChart();
        public string Open
        {
            get { return _open; }
            set
            {
                _open = value;
                OnPropertyChanged(nameof(Open));
            }
        }

        public string High
        {
            get { return _high; }
            set
            {
                _high = value;
                OnPropertyChanged(nameof(High));
            }
        }

        public string Last
        {
            get { return _last; }
            set
            {
                _last = value;
                OnPropertyChanged(nameof(Last));
            }
        }

        public string Low
        {
            get { return _low; }
            set
            {
                _low = value;
                OnPropertyChanged(nameof(Low));
            }
        }

        public string Bid
        {
            get { return _bid; }
            set
            {
                _bid = value;
                OnPropertyChanged(nameof(Bid));
            }
        }

        public string Ask
        {
            get { return _ask; }
            set
            {
                _ask = value;
                OnPropertyChanged(nameof(Ask));
            }
        }

        public Visibility SelectionBorderVisbility
        {
            get
            {
                return _selectionBorderVisibility;
            }
            set
            {
                _selectionBorderVisibility = value;
                OnPropertyChanged(nameof(SelectionBorderVisbility));
            }
        }

        public Brush LastBrush
        {
            get
            {
                return _lastBrush;
            }
            set
            {
                _lastBrush = value;
                OnPropertyChanged(nameof(LastBrush));
            }
        }

        public Brush AskBrush
        {
            get
            {
                return _askBrush;
            }
            set
            {
                _askBrush = value;
                OnPropertyChanged(nameof(AskBrush));
            }
        }

        public Brush BidBrush
        {
            get
            {
                return _bidBrush;
            }
            set
            {
                _bidBrush = value;
                OnPropertyChanged(nameof(BidBrush));
            }
        }

        public Account Account
        {
            get
            {
                return _account;
            }
            set
            {
                _account = value;
                OnPropertyChanged(nameof(Account));
            }
        }

        public int Lot
        {
            get
            {
                return _lot;
            }
            set
            {
                _lot = value;
                OnPropertyChanged(nameof(Lot));
            }
        }

        public AtmStrategySelector AtmStrategy
        {
            get => _strategySelector;
        }
        public BasicCommand BuyCommand { get; private set; }
        public BasicCommand SellCommand { get; private set; }
        #endregion
    }
}
