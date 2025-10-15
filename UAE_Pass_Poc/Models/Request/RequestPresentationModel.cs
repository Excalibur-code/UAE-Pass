using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using UAE_Pass_Poc.Convertor;
using UAE_Pass_Poc.CustomAttributes;
using UAE_Pass_Poc.Enums;

namespace UAE_Pass_Poc.Models.Request
{   
    public class RequestPresentationModel
    {
        [JsonPropertyName("partnerId")]
        [SwaggerIgnore]
        public string? PartnerId { get; set; } = null;

        [JsonPropertyName("purposeEN")]
        public string PurposeEN { get; set; } = string.Empty;

        [JsonPropertyName("purposeAR")]
        public string PurposeAR { get; set; } = string.Empty;

        [JsonPropertyName("requestId")]
        [SwaggerIgnore]
        public string RequestId { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        [EmailAddress]
        public string? Email { get; set; } = null;

        [JsonPropertyName("mobile")]
        [Phone]
        public string? Mobile { get; set; } = null;

        [JsonPropertyName("expiryDate")]
        [JsonConverter(typeof(PlainDateTimeConverter))]
        public DateTime ExpiryDate { get; set; }

        [JsonPropertyName("origin")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RequestOrigin Origin { get; set; } = RequestOrigin.WEB;

        [JsonPropertyName("requestedVerifiedAttributes")]
        public List<string> RequestedVerifiedAttributes { get; set; } = new();

        [JsonPropertyName("requestedDocuments")]
        public List<DocumentInfo> RequestedDocuments { get; set; } = new();
    }

    public class DocumentInfo
    {
        [JsonPropertyName("documentType")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DocumentType? DocumentType { get; set; } = null;

        [JsonPropertyName("required")]
        public bool Required { get; set; } = true;

        [JsonPropertyName("requiredAttested")]
        public bool? RequiredAttested { get; set; } = null;

        [JsonPropertyName("allowExpired")]
        public bool? AllowExpired { get; set; } = null;

        [JsonPropertyName("selfSignedAccepted")]
        public bool? SelfSignedAccepted { get; set; } = null;

        [JsonPropertyName("emirate")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EmirateCode? Emirate { get; set; } = null;

        [JsonPropertyName("singleInstanceRequested")]
        public bool? SingleInstanceRequested { get; set; } = null;

        [JsonPropertyName("instances")]
        public List<DocInstance>? Instances { get; set; } = null;

        [JsonPropertyName("customDocumentTypeEN")]
        public string? CustomDocumentTypeEN { get; set; }

        [JsonPropertyName("customDocumentTypeAR")]
        public string? CustomDocumentTypeAR { get; set; }
    }

    public class DocInstance
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }
}
