using Newtonsoft.Json;
using UAE_Pass_Poc.Models.Request;

namespace UAE_Pass_Poc.Models
{
    public class Credential
    {
        [JsonProperty("@context")]
        public List<string>? Context { get; set; }

        [JsonProperty("vcId")]
        public string? VcId { get; set; }

        [JsonProperty("credentialDocumentType")]
        public string? CredentialDocumentType { get; set; }

        [JsonProperty("documentName")]
        public string? DocumentName { get; set; } // Optional, For Self-Signed Documents

        [JsonProperty("encodedCredential")]
        public string? EncodedCredential { get; set; } // Mandatory for issued credentials, Base64 encoded

        [JsonProperty("issuerSignature")]
        public string? IssuerSignature { get; set; } // CAdES signature of Issuer on hash of encodedCredential

        [JsonProperty("credentialExpiryDate")]
        public string? CredentialExpiryDate { get; set; }

        [JsonProperty("credentialAssuranceLevel")]
        public string? CredentialAssuranceLevel { get; set; }

        [JsonProperty("proofOfIssuanceId")]
        public string? ProofOfIssuanceId { get; set; } // Smart contract DID for status validation

        [JsonProperty("urlToRetriveEvidence")]
        public string? UrlToRetriveEvidence { get; set; }

        [JsonProperty("urlToRetriveVisualization")]
        public string? UrlToRetriveVisualization { get; set; }

        [JsonProperty("proof")]
        public Proof? Proof { get; set; } // Issuer's vault signature on vcId
    }
}