namespace UAE_Pass_Poc.Services.Interfaces;

public interface ICadesVerificationService
{
    /// <summary>
    /// Verifies a CAdES signature against the SHA256 hash of the original payload.
    /// </summary>
    /// <param name="cadesSignature">The Base64 encoded CAdES signature string.</param>
    /// <param name="payloadHash">The SHA256 hash of the content that was signed.</param>

    /// <returns>True if the signature is valid, false otherwise.</returns>
    Task<bool> VerifyCadesSignature(string cadesSignature, byte[] payloadHash);
}