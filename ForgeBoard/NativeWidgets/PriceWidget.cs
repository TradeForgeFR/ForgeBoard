using ForgeBoard.Core;
using ForgeBoard.Views.Widgets;
using System.Windows.Controls;
using System.Windows.Media;

namespace ForgeBoard.NativeWidgets
{
    internal class PriceWidget : BarExtension
    {
        private ForgeBoard.Views.Widgets.PriceWidget _priceWidget;
        public PriceWidget()
        {
            Name = "Price Widget";
            CanBePinned = false;
            PathLogo = PathGeometry.CreateFromGeometry(Geometry.Parse("M20,18H4V6H20M20,4H4C2.89,4 2,4.89 2,6V18A2,2 0 0,0 4,20H20A2,2 0 0,0 22,18V6C22,4.89 21.1,4 20,4M11,17H13V16H14A1,1 0 0,0 15,15V12A1,1 0 0,0 14,11H11V10H15V8H13V7H11V8H10A1,1 0 0,0 9,9V12A1,1 0 0,0 10,13H13V14H9V16H11V17Z"));
            Description = "Displays live instrument's prices on the bar and provides abilities to trade";
        }

        public override void DeInit()
        {
           
        }

        public override void Init()
        {
             
        }

        public override void MenuClick()
        {
            // add the logic when the user click on the item from the main menu
            _priceWidget = new ForgeBoard.Views.Widgets.PriceWidget();
            ForgeBoardInteractions.AddWidgetToBar(_priceWidget, string.Empty, false);

            _priceWidget.removeBTN.Click += (o, e) =>
            {
                _priceWidget.Dispose();
                ForgeBoardInteractions.RemovedWidgetFromBar(_priceWidget);
            };
        }

        public override void PinnedButtonClick(Button targetButton)
        {
             // add the logic when the user click from the pinned button
        }
    }
}
