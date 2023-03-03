using Newtonsoft.Json;

namespace BranchAPI.Models
{
    public class Place
    {
        [JsonProperty(PropertyName = "PlaceId")]
        public string PlaceId { get; set; }

        [JsonProperty(PropertyName = "PlaceName")]
        public string PlaceName { get; set; }

        [JsonProperty(PropertyName = "TariffAmount")]
        public decimal TariffAmount { get; set; }

    }
}
