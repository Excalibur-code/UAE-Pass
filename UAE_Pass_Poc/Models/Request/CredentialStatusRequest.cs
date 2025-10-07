using System.ComponentModel.DataAnnotations;

namespace UAE_Pass_Poc.Models.Request;

public class CredentialStatusRequest
{
    [Required]
    public string ProofOfPresentationId { get; set; } = string.Empty; //blockchain transaction reference id
    [Required]
    public string RequestId { get; set; } = string.Empty; //required id of our own system
    [Required]
    public string ProofOfIssuanceId { get; set; } = string.Empty; //id of an already shared credential.(Check Appendix 8)
}