namespace UAE_Pass_Poc.Entities;

public class ReceivePresentationResponse : Entity
{
    public Guid RequestPresentationId { get; set; }
    public string ProofOfPresentationId { get; set; } = string.Empty;
    public Guid ReceivePresentationId { get; set; }
    public string PresentationReceiptId { get; set; } = string.Empty;
}