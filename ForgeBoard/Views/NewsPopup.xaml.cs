using ForgeBoard.Core;
using ForgeBoard.Models;
using ForgeBoard.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ForgeBoard.Views
{
    /// <summary>
    /// Logique d'interaction pour NewsPopup.xaml
    /// </summary>
    public partial class NewsPopup : UserControl
    {
        public NewsPopup()
        {
            InitializeComponent();
        }

        private void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateText();
            if (DataContext != null)
                (DataContext as NewsCalendar).setImpact(ImpactFilter.SelectedIndex + 1);
        }

        private void UpdateText()
        {
            
            if (DataContext != null)
            {
                int count = (DataContext as NewsCalendar).NewsCountry.Count(i => i.Checked == true);
                
                switch (count)
                {
                    case 0:
                        CountryFilter.Text = "<none>";
                        break;
                    case 1:
                        CountryFilter.Text = (DataContext as NewsCalendar).NewsCountry[0].Label;
                        break;
                    default:
                        CountryFilter.Text = "<multiple>";
                        break;
                }
            }
        }

        private void CountryFilter_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateText();
            if (DataContext != null)
                (DataContext as NewsCalendar).setImpact(ImpactFilter.SelectedIndex + 1);
        }
    }
}
