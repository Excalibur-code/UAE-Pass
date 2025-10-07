using System.ComponentModel.DataAnnotations;
using UAE_Pass_Poc.CustomAttributes;
using UAE_Pass_Poc.Enums;

namespace UAE_Pass_Poc.Models.Request
{
    public class RequestPresentationModel
    {
        [SwaggerIgnore]
        public string PartnerId { get; set; } = string.Empty;
        [Required]
        public string PurposeEN { get; set; } = string.Empty;
        [Required]
        public string PurposeAR { get; set; } = string.Empty;
        [SwaggerIgnore]
        public string Request { get; set; } = string.Empty; //Customized Id - Should be set internally
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [Phone]
        public string Mobile { get; set; } = string.Empty; //Should be in E.164 format
        [Required]
        public DateTime ExpiryDate { get; set; }
        [Required]
        public RequestOrigin Origin { get; set; } = RequestOrigin.WEB;
        public List<string> RequestedVerifiedAttributes { get; set; } = new List<string>(); //Check GetVerifiedAttributes() API for response types
        [Required]
        public List<DocumentInfo> RequestedDocuments { get; set; } = new List<DocumentInfo>();
    }

    public class DocumentInfo
    {
        public DocumentType? DocumentType { get; set; } = null; 
        public string? CustomDocumentTypeEN { get; set; } = null; // if any other doc, which is not mentioned in document type list
        public string? CustomDocumentTypeAR { get; set; } = null;
        public bool Required { get; set; } = true;
        public bool? RequiredAttested { get; set; } = null; // if attestation is required for this document
        public bool? AllowExpired { get; set; } = null; // if expired document is allowed
        public bool? SelfSignedAccepted { get; set; } = null; // if self-signed document is allowed; true for custom document types
        public EmirateCode? Emirate { get; set; } = null; // two character code of emirate, e.g. "DX", "SH" where doc needs to be issued
        public bool? SingleInstanceRequested { get; set; } = null; // true - if only one instance of this document is requested;
        public List<DocInstance>? Instances { get; set; } = null; // if specific instances of this document type are requested, else all will be shown.
    }

    public class DocInstance
    {
        public string? Name { get; set; } = null;
        public string? Value { get; set; } = null; 
    }
}
