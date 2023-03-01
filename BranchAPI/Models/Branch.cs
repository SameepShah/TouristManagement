using Newtonsoft.Json;
using System.Net;

namespace BranchAPI.Models
{
    public class Branch
    {
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }

        [JsonProperty(PropertyName = "BranchCode")]
        public string? BranchCode { get; set; }

        [JsonProperty(PropertyName = "BranchName")]
        public string? BranchName { get; set; }

        [JsonProperty(PropertyName = "Website")]
        public string? Website { get; set; }

        [JsonProperty(PropertyName = "Contact")]
        public string? Contact { get; set; }

        [JsonProperty(PropertyName = "Email")]
        public string? Email { get; set; }

        [JsonProperty(PropertyName = "Place")]
        public Place? Place { get; set; }
        
        [JsonProperty(PropertyName = "Tariffs")]
        public List<Tariff>? Tariffs { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class BranchAddResponse
    {
        public string BranchId { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class BranchEditResponse
    {
        public string BranchId { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string ErrorMessage { get; set; }
    }


    public class UpdateBranch
    {
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }

        [JsonProperty(PropertyName = "BranchCode")]
        public string? BranchCode { get; set; }

        [JsonProperty(PropertyName = "Tariffs")]
        public List<Tariff>? Tariffs { get; set; }

    }

}
