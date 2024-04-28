using ForgeBoard.Core.ViewModels;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Windows.Media;

namespace ForgeBoard.Models
{
    public class EconomicalNewItem 
    { 
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("economy")]
        public string Country { get; set; }

        [JsonProperty("impact")]
        public int Impact { get; set; } = 0;

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
        public string HourStr { get => $"{ConvertedTime.Hour} H"; }

        public Brush FirstImpactBrush { get => Impact >= 1 ? Brushes.Orange : Brushes.Transparent; }

        public Brush SecondImpactBrush { get => Impact >= 2 ? Brushes.Orange : Brushes.Transparent; }

        public Brush ThirdImpactBrush { get => Impact >= 3 ? Brushes.Orange : Brushes.Transparent; }
    }
}
