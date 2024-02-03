#region Using declarations
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using ForgeBoard.Core;
using ForgeBoard.ViewModels;
using ForgeBoard.Views;
using NinjaTrader.Gui.Tools;
using NinjaTrader.NinjaScript;
#endregion

//This namespace holds GUI items and is required.
namespace NinjaTrader.Gui.NinjaScript
{
    // NT creates an instance of each class derived from "AddOnBase" and call OnWindowCreated/OnWindowDestroyed for every instance and every NTWindow which is created or destroyed...
    public class ForgeBoard : AddOnBase
	{
		private NTMenuItem addOnFrameworkMenuItem;
		private NTMenuItem existingMenuItemInControlCenter;
		private NTMenuItem chartMenuItem;
		private Separator separator;
		private bool _isTopBarCreated = false;
		// Same as other NS objects. However there's a difference: this event could be called in any thread

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description = "Example AddOn demonstrating some of the framework's capabilities";
				Name = "Forge Board";
			}
		}

		// Will be called as a new NTWindow is created. It will be called in the thread of that window
		protected override void OnWindowCreated(Window window)
		{
			AddMenuItem(window);
            AddPinButtons(window);
        }

		// Will be called as a new NTWindow is destroyed. It will be called in the thread of that window
		protected override void OnWindowDestroyed(Window window)
		{
			if (addOnFrameworkMenuItem != null && window is ControlCenter)
			{
				if (existingMenuItemInControlCenter != null && existingMenuItemInControlCenter.Items.Contains(addOnFrameworkMenuItem))
					existingMenuItemInControlCenter.Items.Remove(addOnFrameworkMenuItem);

				addOnFrameworkMenuItem.Click -= OnMenuItemClick;
				addOnFrameworkMenuItem = null;
			}
        }

		// Open our AddOn's window when the menu item is clicked on
		private void OnMenuItemClick(object sender, RoutedEventArgs e)
		{
			Core.Globals.RandomDispatcher.BeginInvoke(new Action(() =>
			{
				if (_isTopBarCreated)
					return;

				var model = new MainViewModel();
				model.Init();
                var bar = new TopBar();

				bar.Closed += (oo, ee) =>
				{
					model.DeInit();
					_isTopBarCreated = false;
				};

				bar.DataContext = model;
				_isTopBarCreated = true;
				bar.Show();

                ForgeBoardInteractions.BarDispatcher = bar.Dispatcher;
            }));
		}

        private IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
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
        private void AddMenuItem(Window window)
		{

            // We want to place our AddOn in the Control Center's menus
            ControlCenter cc = window as ControlCenter;
            if (cc == null)
                return;


            /* Determine we want to place our AddOn in the Control Center's "New" menu
            Other menus can be accessed via the control's "Automation ID". For example: toolsMenuItem, workspacesMenuItem, connectionsMenuItem, helpMenuItem. */
            existingMenuItemInControlCenter = cc.FindFirst("ControlCenterMenuItemNew") as NTMenuItem;
            if (existingMenuItemInControlCenter == null)
                return;

			// 'Header' sets the name of our AddOn seen in the menu structure
			separator = new Separator()
			{
				Foreground = System.Windows.Media.Brushes.CadetBlue
            };
            addOnFrameworkMenuItem = new NTMenuItem { Header = "Forge Board", Style = Application.Current.TryFindResource("MainMenuItem") as Style, Icon = "𓄀", Foreground= System.Windows.Media.Brushes.CadetBlue };
            addOnFrameworkMenuItem.Click += OnMenuItemClick;


            // Add our AddOn into the "New" menu
            existingMenuItemInControlCenter.Items.Add(separator);
            existingMenuItemInControlCenter.Items.Add(addOnFrameworkMenuItem);          
        }

        protected void AddPinButtons(Window window)
        {
            if (window is ControlCenter)
                return;

            if (window is NTWindow)
            {
                var closeButton = FindVisualChildren<Button>(window).Where(x => x.Name == "closeButton").First();

                if (closeButton != null)
                {
                    var stack = closeButton.Parent as StackPanel;
                    if (stack != null)
                    {
                        var foregroundBinding = new System.Windows.Data.Binding("Foreground");
                        foregroundBinding.Source = closeButton;

                        var content = new Path()
                        {
                            Data = PathGeometry.CreateFromGeometry(Geometry.Parse("M16,12V4H17V2H7V4H8V12L6,14V16H11.2V22H12.8V16H18V14L16,12Z")),
                            Height = 10,
                            Stretch = System.Windows.Media.Stretch.Uniform,
                            Margin = new Thickness(2, 0, 2, 0)
                        };

                        var btn = new Button()
                        {
                            Style = (Style)Application.Current.FindResource("WindowButtons"),
                            Name = "pinButton",
                            Content = content,
                        };

                        content.SetBinding(Path.FillProperty, foregroundBinding);
                        stack.Children.Insert(0, btn);

                        btn.Click += (o, e) =>
                        {
                            window.Topmost = !window.Topmost;
                        };
                    }
                }
            }
        }

    }   

	public static class Helpers
	{
		public static Dispatcher BarDispatcher { get; internal set; }
	}
}