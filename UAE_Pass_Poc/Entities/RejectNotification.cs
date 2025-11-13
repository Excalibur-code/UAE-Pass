using UAE_Pass_Poc.Enums;

namespace UAE_Pass_Poc.Entities;

public class RejectNotification : Entity
{
    public string ProofOfPresentationRequestId { get; set; } = null!;
    public string RejectReason { get; set; } = null!;
    public Guid PresentationRejectId { get; set; }
    public Guid RequestPresentationId { get; set; }
}