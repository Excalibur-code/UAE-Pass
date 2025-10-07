namespace UAE_Pass_Poc.Models.Response;

public class PresentationRequestStatusResponse
{
    public bool IsSuccess { get; set; } // true if request has been shared, else false
    public bool IsExpired { get; set; } // true if request has expired.
    public string ResponseCode { get; set; } = string.Empty; // DV specific key attribute for look up table corresponding to request status
    public string ResponseType { get; set; } = string.Empty; //Generic type of the response code
    public string ResponseMessage { get; set; } = string.Empty; // Specific type of response code.
    public string ErrorCode { get; set; } = string.Empty; //DV specific error code.
    public string EvidenceShareStatus { get; set; } = string.Empty; // if isSuccess = true; then populate whether evidence/visualization
    // corresponding to that presentation has been shared or not with SP. Possible values are 'SHARED', 'PENDING' 
}