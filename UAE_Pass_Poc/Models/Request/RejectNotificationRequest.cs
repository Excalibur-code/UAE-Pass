using System.ComponentModel.DataAnnotations;

namespace UAE_Pass_Poc.Models.Request;

public class RejectNotificationRequest
{
    [Required]
    public string ProofOfPresentationRequestId { get; set; } = string.Empty;
    public string RejectReason { get; set; } = string.Empty; //(USER_REJECTED, USER_EXITED, UAEPASS_ERROR)
}