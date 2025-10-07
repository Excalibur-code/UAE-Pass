using Newtonsoft.Json;

namespace UAE_Pass_Poc.Models.Request
{
    public class DecodedPresentation
    {
        [JsonProperty("@context")]
        public List<string>? Context { get; set; }

        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("type")]
        public List<string>? Type { get; set; }

        [JsonProperty("presentationSubject")]
        public string? PresentationSubject { get; set; }

        [JsonProperty("verifier")]
        public string? Verifier { get; set; }

        [JsonProperty("requestedAt")]
        public string? RequestedAt { get; set; }

        [JsonProperty("credentials")]
        public List<Credential>? Credentials { get; set; }

        [JsonProperty("verifiedAttributes")]
        public VerifiedAttributes? VerifiedAttributes { get; set; }

        [JsonProperty("proof")]
        public Proof? Proof { get; set; } // Non-CAdES UAEPASS Digital Vault signature
    }

    public class Proof
    {
        [JsonProperty("signatureType")]
        public string? SignatureType { get; set; }

        [JsonProperty("createdOn")]
        public string? CreatedOn { get; set; }

        [JsonProperty("creator")]
        public string? Creator { get; set; }

        [JsonProperty("publicKeyBase58")]
        public string? PublicKeyBase58 { get; set; }

        [JsonProperty("signature")]
        public string? Signature { get; set; }

        [JsonProperty("nonce")]
        public string? Nonce { get; set; }
    }

    public class VerifiedAttributes
    {
        [JsonProperty("mobile")]
        public string? Mobile { get; set; }

        [JsonProperty("email")]
        public string? Email { get; set; }
    }
}