using System.ComponentModel.DataAnnotations;

namespace UAE_Pass_Poc.Models.Request;

public class ReceiveVisualizationModel
{
    [Required]
    public string RequestId { get; set; } = string.Empty; //request Id from UAE Pass Dv
    [Required]
    public string ProofOfPresentationId { get; set; } = string.Empty;//transaction reference id as proof of presentation.
    public string VcId { get; set; } = string.Empty; //present for issued documents; not present for self signed doc.
    [Required]
    public string Status { get; set; } = string.Empty; //ACTIVE, EXPIRED OR REVOKED
    public string VisualizationInfo { get; set; } = string.Empty;//required if status = active;
                                                                 //Base 64 encoded JSON which will contain the Visualization PDF along with the data attributes which have been used to ////generate Visualization
    [Required]
    public string EvidenceInfo { get; set; } = string.Empty;//Base 64 encoded JSON which will contain the Evidence PDF
    public string FileType { get; set; } = string.Empty;//required if status = active; PDF, JPEG or other file types
    public string IssuerSignature { get; set; } = string.Empty;//Signature by the Issuer of the Visual file (may be Issuer or Digital trust platform). This field will not be present if the document is a self signed document
}