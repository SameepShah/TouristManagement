using Newtonsoft.Json;

namespace BranchAPI.Models
{
    public class Branch
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "BranchName")]
        public string BranchName { get; set; }

        [JsonProperty(PropertyName = "Website")]
        public string Website { get; set; }

        [JsonProperty(PropertyName = "Contact")]
        public string Contact { get; set; }

        [JsonProperty(PropertyName = "Email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "Place")]
        public Place Place { get; set; }
        
        [JsonProperty(PropertyName = "Tariffs")]
        public List<Tariff> Tariffs { get; set; }
    }

    public class Tariff
    {
        public string TariffName { get; set; }
        public decimal TariffAmount { get; set; }
    }
}
