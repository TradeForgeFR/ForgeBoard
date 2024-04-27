using ForgeBoard.Core;
using ForgeBoard.Core.ViewModels;
using ForgeBoard.Models;
using ForgeBoard.NativeWidgets;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ForgeBoard.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region private 
        private NewsCalendar _newsCalendar = new NewsCalendar(); 
        #endregion
        public AccountsViewModel AccountsViewModel { get; } = new AccountsViewModel();
        public ObservableCollection<FrameworkElement> Widgets { get; set; } = new ObservableCollection<FrameworkElement>();
        public ObservableCollection<BarExtension> AvailableExtensions { get; private set; }  
        public void Init()
        {  
            // init the logic for the widgets and extensions
            ForgeBoardInteractions.AddWidgetToBarEvent += ForgeBoardInteractions_AddWidgetToBarEvent;
            ForgeBoardInteractions.RemoveWidgetFromBarEvent += ForgeBoardInteractions_RemoveWidgetFromBarEvent;

            // Create the available list from the static
            AvailableExtensions = new ObservableCollection<BarExtension>()
            {
                { new PriceWidget() }, 
                { new HelloTPF() }
            };

            // init the accounts objects
            AccountsViewModel.Init();

            // téléchargement des news éco
            Task.Run(async ()=>
            {
               await _newsCalendar.GetNews(); 
            });
        }

        public void DeInit()
        {
            ForgeBoardInteractions.AddWidgetToBarEvent -= ForgeBoardInteractions_AddWidgetToBarEvent;
            ForgeBoardInteractions.RemoveWidgetFromBarEvent -= ForgeBoardInteractions_RemoveWidgetFromBarEvent;

            AccountsViewModel.Dispose(); 
        }

        // adding the widgets 
        private void ForgeBoardInteractions_RemoveWidgetFromBarEvent(FrameworkElement control)
        {
            Widgets.Remove(control);
        }

        private void ForgeBoardInteractions_AddWidgetToBarEvent(FrameworkElement control, string ProfileName, bool forceDisplay)
        {
            Widgets.Add(control);
        }

        // todo ajouter une région pour le spublic fields
        public NewsCalendar NewsCalendar
        {
            get => _newsCalendar;
        }
    }
}
