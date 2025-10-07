namespace UAE_Pass_Poc.Models.Response;

public class CredentialStatusResponse
{
    public string ProofOfCredentialValidation { get; set; } = string.Empty; //blockchain transaction reference id
    public string Status { get; set; } = string.Empty; //active, revoked
    public DateTime LastStatusUpdateDate { get; set; }
}