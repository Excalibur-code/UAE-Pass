using UAE_Pass_Poc.Enums;

namespace UAE_Pass_Poc.Entities;

public class Document : Entity
{
    public DocumentType? DocumentType { get; set; } = null;
    public string? CustomDocumentTypeEN { get; set; } = null;
    public string? CustomDocumentTypeAR { get; set; } = null;
    public bool Required { get; set; } = true;
    public bool? RequiredAttested { get; set; } = null;
    public bool? AllowExpired { get; set; } = null;
    public bool? SelfSignedAccepted { get; set; } = null;
    public EmirateCode? Emirate { get; set; } = null;
    public bool? SingleInstanceRequested { get; set; } = null;
    public Guid RequestPresentationId { get; set; }
    public virtual IEnumerable<DocInstance>? DocumentInstances { get; set; } = null;
    public virtual RequestPresentation RequestPresentation { get; set; } = null!;
}