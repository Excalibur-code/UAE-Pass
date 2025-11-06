namespace UAE_Pass_Poc.Entities;

public class ReceivePresentation : Entity
{
    public string ProofOfPresentationId { get; set; } = string.Empty;
    public string? ProofOfPresentationRequestId { get; set; } = null;
    public string? QrId { get; set; } = null;
    public List<string>? SignedPresentation { get; set; } = null;
    public string? CitizenSignature { get; set; } = null;
    public bool IsPresentationValid { get; set; } = false;
}