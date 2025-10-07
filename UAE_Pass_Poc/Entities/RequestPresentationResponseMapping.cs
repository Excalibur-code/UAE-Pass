namespace UAE_Pass_Poc.Entities;

public class RequestPresentationResponseMapping
{
    public Guid RequestPresentationId { get; set; }
    public string RequestId { get; set; } = string.Empty;
    public string ProofOfPresentationId { get; set; } = string.Empty; // response from request presentation API.
}