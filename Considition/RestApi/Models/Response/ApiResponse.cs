using Newtonsoft.Json;

namespace Considition.RestApi.Models.Response
{
    class ApiResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }
    }
}
