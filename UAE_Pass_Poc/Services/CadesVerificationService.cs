using System.Text;
using UAE_Pass_Poc.Services.Interfaces;
using System.Security.Cryptography;
using SignedCms = System.Security.Cryptography.Pkcs.SignedCms;
using SignerInfo = System.Security.Cryptography.Pkcs.SignerInfo;

namespace UAE_Pass_Poc.Services
{
    public class CadesVerificationService : ICadesVerificationService
    {
        private readonly ILogger<CadesVerificationService> _logger;

        public CadesVerificationService(ILogger<CadesVerificationService> logger)
        {
            _logger = logger;
        }
        
        public bool ValidateCADESignature(string inputSignature, string inputData)
        {
            bool validated = false;
            
            try
            {
                // Step 1: Decode the Base64-encoded signature
                byte[] signatureBytes = Convert.FromBase64String(inputSignature);
                
                //Step 2: Create SignedCms object and decode the signature
                SignedCms signedCms = new SignedCms();
                signedCms.Decode(signatureBytes);
                
                //Step 3: Extract the data enveloped inside signature
                byte[] contentBytes = signedCms.ContentInfo.Content;
                string extractedData = Encoding.UTF8.GetString(contentBytes);
                
                if (string.Equals(inputData, extractedData, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (SignerInfo signerInfo in signedCms.SignerInfos)
                    {
                        try
                        {
                            signerInfo.CheckSignature(true);
                            validated = true;

                            _logger.LogInformation("Signature verified successfully.");
                        }
                        catch (CryptographicException ex)
                        {
                            _logger.LogWarning($"Signature verification failed: {ex.Message}");
                            validated = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error validating signature: {ex.Message}");
                validated = false;
            }
            
            return validated;
        }
    }
}