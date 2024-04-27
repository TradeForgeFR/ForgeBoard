using ForgeBoard.Core.Views.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ForgeBoard.Core
{
    public enum MessageType
    {
        Info,
        Danger,
        Error,
        Warning
    }
    public static  class ForgeBoardInteractions
    {
        public static event AddWidgetToBarHandler AddWidgetToBarEvent;
        public static event RemoveWidgetFromBarHandler RemoveWidgetFromBarEvent;
        public static event CreateMessagehandler CreateMessageEvent;

        public static void AddWidgetToBar(FrameworkElement control, string profileName, bool forceDisplay)
        {
            AddWidgetToBarEvent?.Invoke(control, profileName, forceDisplay);
        }

        public static void RemovedWidgetFromBar(FrameworkElement control)
        {
            RemoveWidgetFromBarEvent?.Invoke(control);
        }

        public static void CreateMessage(MessageType type, string message)
        {
            CreateMessageEvent?.Invoke(type, message);
        }

        public static TopWindow ExtractToTopWindow(UserControl control)
        {
            var wnd = new TopWindow();
            wnd.container.Content = control;
            wnd.Show();

            return wnd;
        }

        public static Dispatcher BarDispatcher { get;  set; }
    }

    public delegate void AddWidgetToBarHandler(FrameworkElement control, string ProfileName, bool forceDisplay);
    public delegate void RemoveWidgetFromBarHandler(FrameworkElement control);
    public delegate void CreateMessagehandler(MessageType type, string message);
}
