namespace UAE_Pass_Poc.Entities;

public class ReceiveVisualization : Entity
{
    public string RequestId { get; set; } = null!;
    public string ProofOfPresentationId { get; set; } = null!;
    public string VCId { get; set; } = null!;
    public string? VisualizationInfo { get; set; } = null;
    public string? EvidenceInfo { get; set; } = null;
    public string? IssuerSignature { get; set; } = null;
    public string FileType { get; set; } = null!;
    public string Status { get; set; } = null!;
}