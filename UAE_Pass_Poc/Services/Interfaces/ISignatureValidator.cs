namespace UAE_Pass_Poc.Services
{
    public interface ISignatureValidator
    {
        bool ValidateSignature(string payload, string publicKey, string signature);
    }
}