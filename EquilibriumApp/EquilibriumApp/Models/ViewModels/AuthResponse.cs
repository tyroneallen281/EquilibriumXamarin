using Newtonsoft.Json;

namespace EquilibriumApp.Mobile.Models.AppModels
{
    public  class AuthResponse
    {
       [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
        public string RefreshToken { get; set; }

        [JsonProperty("as:client_id")]
        public string ClientId { get; set; }

        public string UserName { get; set; }

        [JsonProperty(".issued")]
        public string Issued { get; set; }

        [JsonProperty(".expires")]
        public string Expires { get; set; }
    }
}