using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using UAE_Pass_Poc.Models;
using UAE_Pass_Poc.Models.Request;
using UAE_Pass_Poc.Services.Interfaces;

namespace UAE_Pass_Poc.Services
{
    public class PresentationProcessingService : IPresentationProcessingService
    {
        private readonly ILogger<PresentationProcessingService> _logger;
        private readonly ICadesVerificationService _cadesVerificationService;

        public PresentationProcessingService(
            ILogger<PresentationProcessingService> logger,
            ICadesVerificationService cadesVerificationService)
        {
            _logger = logger;
            _cadesVerificationService = cadesVerificationService;
        }

        public async Task<List<DecodedPresentation>> ProcessSignedPresentation(List<string> signedPresentationBase64List)
        {
            var decodedPresentations = new List<DecodedPresentation>();

            foreach (var base64EncodedPresentation in signedPresentationBase64List)
            {
                byte[] decodedBytes;
                try
                {
                    decodedBytes = Convert.FromBase64String(base64EncodedPresentation);
                }
                catch (FormatException ex)
                {
                    _logger.LogError(ex, "Invalid BASE64 format for a signed presentation string.");
                    throw new FormatException("One of the signed presentation strings is not valid Base64.", ex);
                }

                string jsonString = Encoding.UTF8.GetString(decodedBytes);

                var decodedPresentation = JsonConvert.DeserializeObject<DecodedPresentation>(jsonString);
                if (decodedPresentation == null)
                {
                    _logger.LogError("Failed to deserialize decoded presentation JSON.");
                    throw new InvalidOperationException("Failed to deserialize decoded presentation.");
                }

                // 1. Verify 'id' (Hash of credentials)
                // if (decodedPresentation.Credentials != null && decodedPresentation.Id != null)
                // {
                //     // Serialize credentials back to JSON string to hash it
                //     // Ensure consistent serialization for hashing (e.g., no pretty printing, consistent property order)
                //     string credentialsJson = JsonConvert.SerializeObject(decodedPresentation.Credentials, Formatting.None);
                //     byte[] credentialsBytes = Encoding.UTF8.GetBytes(credentialsJson);

                //     byte[] hashBytes;
                //     using (SHA256 sha256Hash = SHA256.Create())
                //     {
                //         hashBytes = sha256Hash.ComputeHash(credentialsBytes);
                //     }
                //     string calculatedIdHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

                //     if (!string.Equals(decodedPresentation.Id, calculatedIdHash, StringComparison.OrdinalIgnoreCase))
                //     {
                //         _logger.LogWarning($"Credential hash mismatch for presentation ID: {decodedPresentation.Id}. Calculated: {calculatedIdHash}, Expected: {decodedPresentation.Id}");
                //         throw new SecurityException("Credential hash mismatch.");
                //     }
                //     _logger.LogDebug($"Credential hash verified for presentation ID: {decodedPresentation.Id}");
                // }

                decodedPresentations.Add(decodedPresentation);
            }
            return decodedPresentations;
        }

        // public async Task<bool> VerifyPresentationProof(DecodedPresentation decodedPresentation)
        // {
        //     if (decodedPresentation.Proof == null || string.IsNullOrEmpty(decodedPresentation.PresentationSubject) || decodedPresentation.Credentials == null)
        //     {
        //         _logger.LogWarning("Missing Proof, PresentationSubject, or Credentials for top-level Proof verification.");
        //         return false;
        //     }

        //     // 1. Resolve Citizen's DID to get their DID Document
        //     var didDocument = await _didResolutionService.ResolveDid(decodedPresentation.PresentationSubject);
        //     if (didDocument == null || didDocument.PublicKey == null || !didDocument.PublicKey.Any())
        //     {
        //         _logger.LogWarning($"Could not resolve DID document or find public keys for citizen: {decodedPresentation.PresentationSubject}");
        //         return false;
        //     }

        //     // 2. Find matching public key from DID document
        //     var matchingPublicKey = didDocument.PublicKey.FirstOrDefault(pk =>
        //         string.Equals(pk.PublicKeyBase58, decodedPresentation.Proof.PublicKeyBase58, StringComparison.OrdinalIgnoreCase) &&
        //         string.Equals(pk.Id, decodedPresentation.Proof.Creator, StringComparison.OrdinalIgnoreCase));

        //     if (matchingPublicKey == null)
        //     {
        //         _logger.LogWarning($"No matching public key found in DID document for creator: {decodedPresentation.Proof.Creator} for citizen: {decodedPresentation.PresentationSubject}");
        //         return false;
        //     }

        //     // 3. Perform non-CAdES signature verification using the public key, signature, nonce, and the signed content (credentials object).
        //     // The content that was signed is the JSON representation of `decodedPresentation.Credentials`.
        //     string signedContent = JsonConvert.SerializeObject(decodedPresentation.Credentials, Formatting.None); // Ensure consistent serialization
        //     byte[] signedContentBytes = Encoding.UTF8.GetBytes(signedContent);

        //     // This part is highly dependent on the 'signatureType' and the specific algorithm (EdDSA on secp256k1).
        //     // You'll need a cryptographic library that supports EdDSA and Base58 decoding for the public key.
        //     // Example (conceptual - you'll need to find a suitable library and implement this precisely):
        //     // bool isProofSignatureValid = YourEdDSALibrary.Verify(
        //     //     decodedPresentation.Proof.Signature,
        //     //     decodedPresentation.Proof.Nonce,
        //     //     signedContentBytes, // The content that was signed
        //     //     matchingPublicKey.PublicKeyBase58, // Base58 encoded public key
        //     //     decodedPresentation.Proof.SignatureType
        //     // );

        //     // For now, returning true conceptually. Replace with actual crypto verification.
        //     bool isProofSignatureValid = true; // Placeholder

        //     if (!isProofSignatureValid)
        //     {
        //         _logger.LogWarning($"Top-level Proof signature verification failed for citizen: {decodedPresentation.PresentationSubject}");
        //         return false;
        //     }

        //     _logger.LogInformation($"Top-level Proof signature successfully verified for citizen: {decodedPresentation.PresentationSubject} (conceptual).");
        //     return await Task.FromResult(true);
        // }

        public async Task<bool> VerifyCredentialIssuerSignature(Credential credential)
        {
            if (string.IsNullOrEmpty(credential.EncodedCredential) || string.IsNullOrEmpty(credential.IssuerSignature))
            {
                _logger.LogWarning($"Missing encodedCredential or issuerSignature for credential VC ID: {credential.VcId}");
                return false;
            }

            // 1. Base64 decode encodedCredential
            byte[] decodedEncodedCredentialBytes;
            try
            {
                decodedEncodedCredentialBytes = Convert.FromBase64String(credential.EncodedCredential);
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, $"Invalid BASE64 format for encodedCredential in VC ID: {credential.VcId}");
                return false;
            }

            // 2. Calculate SHA256 hash of the decoded encodedCredential
            byte[] hashOfEncodedCredential;
            using (SHA256 sha256Hash = SHA256.Create())
            {
                hashOfEncodedCredential = sha256Hash.ComputeHash(decodedEncodedCredentialBytes);
            }
            string hashHex = BitConverter.ToString(hashOfEncodedCredential).Replace("-", "").ToLowerInvariant();
            _logger.LogDebug($"Calculated SHA256 Hash of encodedCredential for VC ID {credential.VcId}: {hashHex}");

            // 3. Perform CAdES verification using issuerSignature and the hash
            bool isIssuerSignatureValid = _cadesVerificationService.ValidateCADESignature(
                credential.IssuerSignature,
                string.Empty
            );

            if (!isIssuerSignatureValid)
            {
                _logger.LogWarning($"Issuer CAdES signature verification failed for VC ID: {credential.VcId}");
            }
            else
            {
                _logger.LogInformation($"Issuer CAdES signature successfully verified for VC ID: {credential.VcId}");
            }
            return isIssuerSignatureValid;
        }

        // public async Task<bool> VerifyCredentialProof(Credential credential)
        // {
        //     if (credential.Proof == null || string.IsNullOrEmpty(credential.VcId))
        //     {
        //         _logger.LogWarning($"Missing Proof or VcId for credential proof verification.");
        //         return false;
        //     }

        //     // The Proof is the Issuer's vault signature on vcId.
        //     // So, the content signed is the vcId itself.
        //     byte[] signedContentBytes = Encoding.UTF8.GetBytes(credential.VcId);

        //     // 1. Resolve Issuer's DID (from Proof.Creator)
        //     string issuerDid = credential.Proof.Creator;
        //     if (string.IsNullOrEmpty(issuerDid))
        //     {
        //         _logger.LogWarning($"Could not determine Issuer DID from Proof.Creator for credential VC ID: {credential.VcId}");
        //         return false;
        //     }

        //     var didDocument = await _didResolutionService.ResolveDid(issuerDid);
        //     if (didDocument == null || didDocument.PublicKey == null || !didDocument.PublicKey.Any())
        //     {
        //         _logger.LogWarning($"Could not resolve DID document or find public keys for issuer: {issuerDid} for credential VC ID: {credential.VcId}");
        //         return false;
        //     }

        //     // 2. Find matching public key from DID document
        //     var matchingPublicKey = didDocument.PublicKey.FirstOrDefault(pk =>
        //         string.Equals(pk.PublicKeyBase58, credential.Proof.PublicKeyBase58, StringComparison.OrdinalIgnoreCase) &&
        //         string.Equals(pk.Id, credential.Proof.Creator, StringComparison.OrdinalIgnoreCase));

        //     if (matchingPublicKey == null)
        //     {
        //         _logger.LogWarning($"No matching public key found in DID document for issuer creator: {credential.Proof.Creator} for credential VC ID: {credential.VcId}");
        //         return false;
        //     }

        //     // 3. Perform non-CAdES signature verification using the public key, signature, nonce, and the signed content (vcId).
        //     // Example (conceptual - you'll need to find a suitable library and implement this precisely):
        //     // bool isCredentialProofValid = YourCryptoLibrary.Verify(
        //     //     credential.Proof.Signature,
        //     //     credential.Proof.Nonce,
        //     //     signedContentBytes, // The content that was signed (vcId)
        //     //     matchingPublicKey.PublicKeyBase58, // Base58 encoded public key
        //     //     credential.Proof.SignatureType
        //     // );

        //     // For now, returning true conceptually. Replace with actual crypto verification.
        //     bool isCredentialProofValid = true; // Placeholder

        //     if (!isCredentialProofValid)
        //     {
        //         _logger.LogWarning($"Credential Proof signature verification failed for VC ID: {credential.VcId}");
        //         return false;
        //     }

        //     _logger.LogInformation($"Credential Proof signature successfully verified for VC ID: {credential.VcId} (conceptual).");
        //     return await Task.FromResult(true);
        // }

        public async Task IntegratePresentationData(List<DecodedPresentation> verifiedPresentations, string? requestId)
        {
            foreach (var presentation in verifiedPresentations)
            {
                _logger.LogInformation($"Integrating data for presentation subject: {presentation.PresentationSubject}");

                if (presentation.Credentials != null)
                {
                    foreach (var credential in presentation.Credentials)
                    {
                        _logger.LogInformation($"Processing credential type: {credential.CredentialDocumentType}, VC ID: {credential.VcId}");

                        // 1. Verify Issuer Signature (CAdES)
                        //bool isIssuerSignatureValid = await VerifyCredentialIssuerSignature(credential);
                        string combinedSignedPresentationString = string.Join("", credential.EncodedCredential);
                        byte[] combinedSignedPresentationBytes = Encoding.UTF8.GetBytes(combinedSignedPresentationString);

                        byte[] hashOfCombinedSignedPresentation;
                        using (SHA256 sha256Hash = SHA256.Create())
                        {
                            hashOfCombinedSignedPresentation = sha256Hash.ComputeHash(combinedSignedPresentationBytes);
                        }
                        string hashHex = BitConverter.ToString(hashOfCombinedSignedPresentation).Replace("-", "").ToLowerInvariant();
                        bool isIssuerSignatureValid = _cadesVerificationService.ValidateCADESignature(credential.IssuerSignature!, hashHex);
                        if (!isIssuerSignatureValid)
                        {
                            _logger.LogError($"Skipping integration for VC ID {credential.VcId} due to invalid issuer signature.");
                            continue; // Or throw an exception, depending on your error handling policy
                        }

                        // 2. Verify Credential Proof (Issuer's vault signature on vcId)
                        //bool isCredentialProofValid = await VerifyCredentialProof(credential);
                        bool isCredentialProofValid = false; // Placeholder until implemented
                        if (!isCredentialProofValid)
                        {
                            _logger.LogError($"Skipping integration for VC ID {credential.VcId} due to invalid credential proof.");
                            continue;
                        }

                        // 3. Validate Credential Status via Blockchain (proofOfIssuanceId)
                        // This would involve interacting with the Blockchain using proofOfIssuanceId
                        // Example: bool isActive = await _blockchainService.CheckCredentialStatus(credential.ProofOfIssuanceId);
                        // if (!isActive) { /* handle revoked credential */ }
                        _logger.LogInformation($"Credential status check for VC ID {credential.VcId} (conceptual).");


                        // 4. Decode and Parse EncodedCredential (if present)
                        if (!string.IsNullOrEmpty(credential.EncodedCredential))
                        {
                            byte[] innerDecodedBytes;
                            try
                            {
                                innerDecodedBytes = Convert.FromBase64String(credential.EncodedCredential);
                            }
                            catch (FormatException ex)
                            {
                                _logger.LogError(ex, $"Invalid BASE64 format for inner encodedCredential in VC ID: {credential.VcId}");
                                continue;
                            }

                            string innerJsonString = Encoding.UTF8.GetString(innerDecodedBytes);
                            _logger.LogDebug($"Decoded inner credential JSON for VC ID {credential.VcId}: {innerJsonString}");

                            // Now, parse this innerJsonString based on Section 8.1.1.1 for specific document types
                            // You'll need a switch statement or a factory pattern here
                            switch (credential.CredentialDocumentType)
                            {
                                case "EmiratesId":
                                    // Example: var emiratesIdData = JsonConvert.DeserializeObject<EmiratesIdData>(innerJsonString);
                                    // _logger.LogDebug($"Decoded Emirates ID data: {emiratesIdData?.IdNumber}");
                                    // await _yourUserRepository.UpdateUserWithEmiratesId(emiratesIdData, presentation.PresentationSubject);
                                    _logger.LogInformation($"Integrating EmiratesId data for VC ID: {credential.VcId} (conceptual).");
                                    break;
                                case "Passport":
                                    // Example: var passportData = JsonConvert.DeserializeObject<PassportData>(innerJsonString);
                                    // _logger.LogDebug($"Decoded Passport data: {passportData?.PassportNumber}");
                                    _logger.LogInformation($"Integrating Passport data for VC ID: {credential.VcId} (conceptual).");
                                    break;
                                case "SalaryCertificate":
                                    // If salary certificate also has encodedCredential (the example showed it without)
                                    // Example: var salaryData = JsonConvert.DeserializeObject<SalaryData>(innerJsonString);
                                    _logger.LogInformation($"Integrating SalaryCertificate data for VC ID: {credential.VcId} (conceptual).");
                                    break;
                                default:
                                    _logger.LogWarning($"Unknown credentialDocumentType: {credential.CredentialDocumentType} for VC ID: {credential.VcId}. Skipping inner credential processing.");
                                    break;
                            }
                        }
                        else if (!string.IsNullOrEmpty(credential.DocumentName))
                        {
                            // This is likely a self-signed document without an encodedCredential
                            _logger.LogDebug($"Processing self-signed document: {credential.DocumentName} for VC ID: {credential.VcId}");
                            // You might retrieve evidence using urlToRetriveEvidence here
                            _logger.LogInformation($"Integrating self-signed document data for VC ID: {credential.VcId} (conceptual).");
                        }
                    }
                }

                if (presentation.VerifiedAttributes != null)
                {
                    _logger.LogDebug($"Processing verified mobile: {presentation.VerifiedAttributes.Mobile}, email: {presentation.VerifiedAttributes.Email}");
                    // Update user with verified attributes
                    _logger.LogInformation($"Integrating verified attributes for presentation subject: {presentation.PresentationSubject} (conceptual).");
                }
            }
            await Task.CompletedTask;
        }
    }
}