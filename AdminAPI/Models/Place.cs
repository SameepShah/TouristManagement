using Newtonsoft.Json;

namespace AdminAPI.Models
{
    public class Place
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "PlaceId")]
        public string PlaceId { get; set; }

        [JsonProperty(PropertyName = "PlaceName")]
        public string PlaceName { get; set; }

    }
}
