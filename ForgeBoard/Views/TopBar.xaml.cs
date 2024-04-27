using ForgeBoard.Core;
using ForgeBoard.ViewModels;
using NinjaTrader.Gui.Tools;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using TWCAppBar.AppBar;

namespace ForgeBoard.Views
{
    /// <summary>
    /// Logique d'interaction pour TopBar.xaml
    /// </summary> 
    public partial class TopBar : Window
    {
        public WorkspaceOptions WorkspaceOptions { get; set; }
        private RightBar _rightBar;
        internal static event ExitBarHandler OnExitBar;
        public TopBar()
        {
            InitializeComponent();
            Loaded += TopBar_Loaded;
            Closing += TopBar_Closing;

            OnExitBar += TopBar_OnExitBar;
        }

        private void TopBar_OnExitBar()
        {
            Close();
        }

        internal static void Exit()
        {
            OnExitBar?.Invoke();
        }
        private void TopBar_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        { 
            OnExitBar -= TopBar_OnExitBar;
            AppBarFunctions.SetAppBar(this, ABEdge.None);

            if (_rightBar != null)
            {
                AppBarFunctions.SetAppBar(_rightBar, ABEdge.None);
                _rightBar.Close();
            }
        }

        private void TopBar_Loaded(object sender, RoutedEventArgs e)
        {
            AppBarFunctions.SetAppBar(this, ABEdge.Top);

            /*_rightBar = new RightBar()
            {
                Width = 100
            };
            _rightBar.Loaded += (ii, ee) =>
            {
                AppBarFunctions.SetAppBar(_rightBar, ABEdge.Right);

            };
            _rightBar.Show();
           */
            NinjaTraderInteractions.Init();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void mainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            mainMenuPopup.IsOpen = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NinjaTraderInteractions.OpenNinjaDialog(NinjaDialog.Chart);
        }

        public IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                        yield return (T)child;

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                        yield return childOfChild;
                }
            }
        }

        private void accountBTN_Click(object sender, RoutedEventArgs e)
        {
            this.accountPopup.IsOpen = true;
        }

        private void newsButton_Click(object sender, RoutedEventArgs e)
        {
            newsPopup.IsOpen = true;
        }
    }
    internal delegate void ExitBarHandler();
}
