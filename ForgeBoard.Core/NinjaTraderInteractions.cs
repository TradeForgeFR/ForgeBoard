using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Tools;
using NinjaTrader.NinjaScript;
using System.Windows;
namespace ForgeBoard.Core
{
    public enum NinjaDialog
    {
        Chart,
        News,
        DynamicDOM
    }

    public static class NinjaTraderInteractions
    {
        private static ControlCenter _controlCenter;

        public static void Init()
        {
            foreach (Window window in NinjaTrader.Core.Globals.AllWindows)
            {
                if (window is ControlCenter)
                {
                    _controlCenter = (ControlCenter)window;
                    return; 
                }
            }
        } 
        
        public static void OpenNinjaDialog(NinjaDialog dialog)
        { 
            if (_controlCenter == null)
                return;
            // TODO : Message error
            string itemName = string.Empty;

            switch (dialog)
            {
                case NinjaDialog.Chart:
                    itemName = "mnuChart";
                    break;
                case NinjaDialog.News:
                    itemName = "mnuNews";
                    break;
                case NinjaDialog.DynamicDOM:
                    itemName = "mnuDomDynamic";
                    break;
            }

            if (string.IsNullOrEmpty(itemName))
                return;
            // TODO : Message error
            _controlCenter.Dispatcher.Invoke(() =>
            {
                var chartMenu = _controlCenter.FindName(itemName) as NTMenuItem;
                if (chartMenu != null)
                    chartMenu.RaiseEvent(new RoutedEventArgs(NTMenuItem.ClickEvent));
            });
        }

        public static void PrintToOutput(string message)
        {
            NinjaTrader.Code.Output.Process(message, PrintTo.OutputTab1);
        }

        public static void SendMarketOrder(bool isBuyOrder, int size, Instrument instrument, Account account, AtmStrategy atm = null)
        {
            if (account != null && instrument != null && size >= 1)
            {
                var _mainOrder = account.CreateOrder(instrument,
                                 isBuyOrder ? OrderAction.Buy : OrderAction.Sell,
                                 OrderType.Market,
                                 OrderEntry.Manual,
                                 TimeInForce.Day,
                                 size,
                                 0,
                                 0,
                                 string.Empty,
                                 "Entry",
                                 NinjaTrader.Core.Globals.MaxDate,
                                 null);

                if (atm == null)
                    account.Submit(new[] { _mainOrder });
                else
                    AtmStrategy.StartAtmStrategy(atm, _mainOrder);
            }
        }
    }
}
