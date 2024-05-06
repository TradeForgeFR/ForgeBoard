using ForgeBoard.Core;
using ForgeBoard.Core.ViewModels;
using Newtonsoft.Json;
using NinjaTrader.Gui.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using static Shared1.OrderStatus.Types;

namespace ForgeBoard.Models
{
    public class SelectedCountry: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _checked;
        public bool Checked
        {
            get => _checked;
            set
            {
                _checked = value;
                OnPropertyChanged();
            }
        }

        //public bool Checked { get; set; }
        public string Label { get; set; }

        public SelectedCountry(bool Checked, string lbl)
        {
            this.Checked = Checked;
            this.Label = lbl;
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class NewsCalendar : ViewModelBase
    {
        public ObservableCollection<EconomicalNewItem> News { get; private set; } = new ObservableCollection<EconomicalNewItem>();
        public ObservableCollection<EconomicalNewItem> FilteredNews { get; private set; } = new ObservableCollection<EconomicalNewItem>();
        public ObservableCollection<SelectedCountry> NewsCountry { get; private set; } = new ObservableCollection<SelectedCountry>();

        private int _impact = 0;

        public async Task GetNews()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string apiUrl = "https://tradeforge.fr/news";

                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var list = JsonConvert.DeserializeObject<List<EconomicalNewItem>>(content);

                    foreach (var item in list)
                    {
                        if (item.Date.IsNullOrEmpty())
                            continue;

                        var newDate = DateTime.Parse(item.Date);

                        DateTime convertedTime = TimeZoneInfo.ConvertTimeFromUtc(newDate, TimeZoneInfo.Local);
                        item.ConvertedTime = convertedTime;
                    }

                    ForgeBoardInteractions.BarDispatcher.Invoke((Action)delegate
                    {
                        NewsCountry = new ObservableCollection<SelectedCountry>(list.Where(p => !string.IsNullOrEmpty(p.Country)).Select(n => new SelectedCountry(false, n.Country)).GroupBy(o => o.Label).Select(q => q.FirstOrDefault()).ToList().OrderBy(r => r.Label));
                        NewsCountry.Insert(0, new SelectedCountry(true, "ALL"));
                        this.OnPropertyChanged(nameof(NewsCountry));
                    });

                    // ne jamais oublier le dispatcher sur un objet de type Observable collection
                    ForgeBoardInteractions.BarDispatcher.Invoke((Action)delegate
                    {
                        News = new ObservableCollection<EconomicalNewItem>(list);
                        FilteredNews = FilteredData;
                        this.OnPropertyChanged(nameof(FilteredNews));
                    });
                    // juste pour la curiosité
                    NinjaTraderInteractions.PrintToOutput("News total = " + News.Count);
                }
                else
                {
                    NinjaTraderInteractions.PrintToOutput("Error occured while getting the news list, infos : " + response.ReasonPhrase);
                }
            }

        }

        public void setImpact(int i)
        {
            _impact = i;
            FilteredNews = FilteredData;
            //NinjaTraderInteractions.PrintToOutput("Filtered News total = " + FilteredNews.Count);
            this.OnPropertyChanged(nameof(FilteredNews));
        }
        public ObservableCollection<EconomicalNewItem> FilteredData
        {
            get
            {
                return new ObservableCollection<EconomicalNewItem>(News.Where(
                    i => MeetsFilterRequirements(i)));
            }
        }

        private bool MeetsFilterRequirements(EconomicalNewItem item)
        {
            IEnumerable<SelectedCountry> SelectedCountries = NewsCountry.Where(n => (n.Checked == true && n.Label == item.Country) || (n.Checked == true && n.Label == "ALL"));
            if (SelectedCountries.Count() == 0)
                return false;

            if (_impact == 1)
                return true;

            return (item.Impact >= _impact);
        }
    }
}
