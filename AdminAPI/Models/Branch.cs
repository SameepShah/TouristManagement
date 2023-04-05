using Newtonsoft.Json;
using System.Net;
namespace AdminAPI.Models
{
    public class Branch
    {
        [JsonProperty(PropertyName = "id")]
        public string? id { get; set; }

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

        [JsonProperty(PropertyName = "Places")]
        public List<Place>? Places { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class SearchBranchResponse
    {
        public List<Branch> Branches { get; set; }
        public int TotalRecords { get; set; }
        public string Message { get; set; }
    }

    public class SearchBranch
    {
        [JsonProperty(PropertyName = "id")]
        public string? id { get; set; }

        [JsonProperty(PropertyName = "BranchCode")]
        public string? BranchCode { get; set; }

        [JsonProperty(PropertyName = "BranchName")]
        public string? BranchName { get; set; }

        [JsonProperty(PropertyName = "Place")]
        public string? Place { get; set; }

        [JsonProperty(PropertyName = "PaginationSorting")]
        public PaginationSortingModel PaginationSorting { get; set; }
    }

}
