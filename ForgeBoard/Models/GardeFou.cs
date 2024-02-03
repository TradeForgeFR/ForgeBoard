
using ForgeBoard.Core;
using NinjaTrader.Cbi;

namespace ForgeBoard.Models
{
    public class GardeFou
    {
        public void Init()
        {
            //TODO // remove the return bellow to activate it
            return;

            foreach(var account in Account.All)
            {
                account.OrderUpdate += Account_OrderUpdate;
            }
        }

        public void Dispose()
        {
            foreach (var account in Account.All)
            {
                account.OrderUpdate -= Account_OrderUpdate;
            }

            NinjaTraderInteractions.PrintToOutput("Disposing Garde Fou");
        }

        private void Account_OrderUpdate(object sender, OrderEventArgs e)
        {
            if (e.OrderState == OrderState.Submitted)
            {
                (sender as Account).Cancel(new[] { e.Order });
                NinjaTraderInteractions.PrintToOutput("Cancel by Garde Fou " + e.Order.ToString());
            }
        }
    }
}
