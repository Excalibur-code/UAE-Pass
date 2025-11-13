using System.Security.Policy;
using UAE_Pass_Poc.Enums;

namespace UAE_Pass_Poc.Entities;

public class RequestPresentation : Entity
{
    public string PurposeEN { get; set; } = string.Empty;
    public string PurposeAR { get; set; } = string.Empty;
    public string Request { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public RequestOrigin Origin { get; set; } = RequestOrigin.WEB;
    public string RequestedVerifiedAttributes { get; set; } = string.Empty;
    public RequestStatus Status { get; set; } = RequestStatus.PENDING;
    public string? Message { get; set; } = null;
    public virtual ICollection<Document> RequestedDocuments { get; set; } = null!;
}