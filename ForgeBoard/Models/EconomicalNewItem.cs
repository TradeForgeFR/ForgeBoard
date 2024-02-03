using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace ForgeBoard.Models
{
    public class EconomicalNewItem
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("economy")]
        public string Country { get; set; }

        [JsonProperty("impact")]
        public string Impact { get; set; }

        [JsonProperty("previous")]
        public string Previous { get; set; }

        [JsonProperty("actual")]
        public string Actual { get; set; }

        [JsonProperty("forecast")]
        public string Forecast { get; set; }

        [JsonProperty("data")]
        public string Date { get; set; }

        [DisplayName("Date")]
        public DateTime ConvertedTime { get; set; } 
    }
}
