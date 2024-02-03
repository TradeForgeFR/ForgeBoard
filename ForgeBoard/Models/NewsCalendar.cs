using ForgeBoard.Core;
using Newtonsoft.Json;
using NinjaTrader.Gui.Tools;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ForgeBoard.Models
{
    public class NewsCalendar
    {
        public async Task<List<EconomicalNewItem>> GetNews()
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

                    return list;
                }
                else
                {
                    NinjaTraderInteractions.PrintToOutput("Error occured while getting the news list, infos : " + response.ReasonPhrase);
                    return null;
                }
            }

        }
    }
}
