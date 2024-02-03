using System.Windows;
using System.Windows.Input;

namespace ForgeBoard.Core.Views.Windows
{
    /// <summary>
    /// Logique d'interaction pour TopWindow.xaml
    /// </summary>
    public partial class TopWindow : Window
    {
        public TopWindow()
        {
            InitializeComponent();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void Border_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }
}
