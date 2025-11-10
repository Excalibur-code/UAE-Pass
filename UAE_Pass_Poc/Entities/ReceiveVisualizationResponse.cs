using NBitcoin.RPC;

namespace UAE_Pass_Poc.Entities;

public class ReceiveVisualizationResponse : Entity
{
    public Guid ReceiveVisualizationId { get; set; }
    public string EvidenceVisualizationReceiptID { get; set; } = null!;
    public Guid ReceivePresentationId { get; set; }
    public string ProofOfPresentationId { get; set; } = null!;
}