using Newtonsoft.Json;

namespace CRM_Homestay.App.Attributes
{
    /// <summary>
    /// JsonResponse
    /// </summary>
    public class JsonResponse
    {
        /// <summary>
        /// Message
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string? Message { get; set; }

        /// <summary>
        /// StatusCode
        /// </summary>
        [JsonProperty(PropertyName = "statusCode")]
        public int StatusCode { get; set; }

        /// <summary>
        /// MessageDevelopment
        /// </summary>
        [JsonProperty(PropertyName = "messageDevelopment")]
        public string? MessageDevelopment { get; set; }
    }
}
