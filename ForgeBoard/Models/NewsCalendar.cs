using ForgeBoard.Core;
using ForgeBoard.Core.ViewModels;
using Newtonsoft.Json;
using NinjaTrader.Gui.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ForgeBoard.Models
{
    public class NewsCalendar : ViewModelBase
    {
        public ObservableCollection<EconomicalNewItem> News { get; private set; } = new ObservableCollection<EconomicalNewItem>();
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

                    // ne jamais oublier le dispatcher sur un objet de type Observable collection
                    ForgeBoardInteractions.BarDispatcher.Invoke((Action)delegate 
                    {
                        News = new ObservableCollection<EconomicalNewItem>(list);
                        this.OnPropertyChanged(nameof(News));
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
    }
}
