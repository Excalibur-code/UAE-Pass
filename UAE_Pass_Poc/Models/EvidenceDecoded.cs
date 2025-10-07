using Newtonsoft.Json;

namespace UAE_Pass_Poc.Models
{
    public class Evidence
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("credentialDocumentType")]
        public string? CredentialDocumentType { get; set; }
    }
}