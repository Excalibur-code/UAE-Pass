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
            // Initialize trusted certificates here if needed
            // Example: _uaePassRootCertificate = new X509Certificate2("path/to/uaepass_root.cer");
        }
        
        public bool ValidateCADESignature(string inputSignature, string inputData)
        {
            bool validated = false;
            
            try
            {
                // Base64 decode of input signature
                byte[] signatureBytes = Convert.FromBase64String(inputSignature);
                
                // Create SignedCms object and decode the signature
                SignedCms signedCms = new SignedCms();
                signedCms.Decode(signatureBytes);
                
                // Extract the data enveloped inside signature
                byte[] contentBytes = signedCms.ContentInfo.Content;
                string extractedData = Encoding.UTF8.GetString(contentBytes);
                
                // Deserialize if needed (equivalent to ObjectMapper)
                // If the data is JSON, you can deserialize it
                // string extractedData = JsonConvert.DeserializeObject<string>(rawExtractedData);
                
                // Is Input Data matching with the data retrieved from Signature?
                if (string.Equals(inputData, extractedData, StringComparison.OrdinalIgnoreCase))
                {
                    // Verify the Certificate and Signature
                    foreach (SignerInfo signerInfo in signedCms.SignerInfos)
                    {
                        try
                        {
                            // Verify the signature with the certificate
                            signerInfo.CheckSignature(true); // true = verify certificate chain
                            
                            // If we reach here, signature is verified
                            Console.WriteLine("Signature verified");
                            validated = true;
                        }
                        catch (CryptographicException ex)
                        {
                            Console.WriteLine($"Signature verification failed: {ex.Message}");
                            validated = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error validating signature: {ex.Message}");
                validated = false;
            }
            
            return validated;
        }
    }
}