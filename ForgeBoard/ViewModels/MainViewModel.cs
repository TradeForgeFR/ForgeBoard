using ForgeBoard.Core;
using ForgeBoard.Core.ViewModels;
using ForgeBoard.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ForgeBoard.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region private 
        private NewsCalendar _newsCalendar = new NewsCalendar();
        private GardeFou _gardeFou = new GardeFou();
        #endregion
        public AccountsViewModel AccountsViewModel { get; } = new AccountsViewModel();
        public ObservableCollection<UserControl> Widgets { get; set; } = new ObservableCollection<UserControl>();
        public ObservableCollection<BarExtension> AvailableExtensions { get; private set; } = new ObservableCollection<BarExtension>();
        public void Init()
        {  
            // init the logic for the widgets and extensions
            ForgeBoardInteractions.AddWidgetToBarEvent += ForgeBoardInteractions_AddWidgetToBarEvent;
            ForgeBoardInteractions.RemoveWidgetFromBarEvent += ForgeBoardInteractions_RemoveWidgetFromBarEvent;

            var widget = new ForgeBoard.NativeWidgets.PriceWidget();
            AvailableExtensions.Add(widget);

            var news = new ForgeBoard.NativeWidgets.NewsWidget();
            AvailableExtensions.Add(news);

            // init the accounts objects
            AccountsViewModel.Init();

            Task.Run(async ()=>
            {
                var list = await _newsCalendar.GetNews();
                if(list != null)
                {
                    foreach(var item in list)
                    {
                        NinjaTraderInteractions.PrintToOutput(item.Name);
                    }
                }
            });

            _gardeFou.Init();
        }

        public void DeInit()
        {
            ForgeBoardInteractions.AddWidgetToBarEvent -= ForgeBoardInteractions_AddWidgetToBarEvent;
            ForgeBoardInteractions.RemoveWidgetFromBarEvent -= ForgeBoardInteractions_RemoveWidgetFromBarEvent;

            AccountsViewModel.Dispose();
            _gardeFou.Dispose();
        }
        private void ForgeBoardInteractions_RemoveWidgetFromBarEvent(UserControl control)
        {
            Widgets.Remove(control);
        }

        private void ForgeBoardInteractions_AddWidgetToBarEvent(UserControl control, string ProfileName, bool forceDisplay)
        {
            Widgets.Add(control);
        } 
    }
}
