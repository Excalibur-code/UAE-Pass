namespace UAE_Pass_Poc.Entities;

public class VisualizationFile : Entity
{
    public string File { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string ProofOfPresentationId { get; set; } = null!;
    public Guid VisualizationId { get; set; } 
}