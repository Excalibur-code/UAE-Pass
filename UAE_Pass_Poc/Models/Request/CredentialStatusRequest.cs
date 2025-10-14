using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace UAE_Pass_Poc.Models.Request;

public class CredentialStatusRequest
{
    [Required]
    [JsonProperty("proofOfPresentationId")]
    public string ProofOfPresentationId { get; set; } = string.Empty; //blockchain transaction reference id
    [Required]
    [JsonProperty("requestId")]
    public string RequestId { get; set; } = string.Empty; //required id of our own system
    [Required]
    [JsonProperty("proofOfIssuanceId")]
    public string ProofOfIssuanceId { get; set; } = string.Empty; //id of an already shared credential.(Check Appendix 8)
}