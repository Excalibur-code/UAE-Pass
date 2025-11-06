using UAE_Pass_Poc.Models;
using UAE_Pass_Poc.Models.Request; // For DecodedPresentation and Credential

namespace UAE_Pass_Poc.Services.Interfaces
{
    public interface IPresentationProcessingService
    {
        /// <summary>
        /// Decodes, deserializes, and performs initial validation of the internal structure of signedPresentation.
        /// </summary>
        /// <param name="signedPresentationBase64List">List of Base64 encoded signed presentation strings.</param>
        /// <returns>A list of decoded and partially validated presentation objects.</returns>
        Task<List<DecodedPresentation>> ProcessSignedPresentation(List<string> signedPresentationBase64List);

        /// <summary>
        /// Verifies the top-level Proof object within a DecodedPresentation (Citizen's signature on credentials).
        /// </summary>
        /// <param name="decodedPresentation">The decoded presentation object.</param>
        /// <returns>True if the Proof is valid, false otherwise.</returns>
        //Task<bool> VerifyPresentationProof(DecodedPresentation decodedPresentation);

        /// <summary>
        /// Verifies the CAdES signature of the Issuer on the encodedCredential within a specific Credential.
        /// </summary>
        /// <param name="credential">The credential object containing the encodedCredential and issuerSignature.</param>
        /// <returns>True if the issuer's CAdES signature is valid, false otherwise.</returns>
        Task<bool> VerifyCredentialIssuerSignature(Credential credential);

        /// <summary>
        /// Verifies the Issuer's vault signature (Proof object) on the vcId within a specific Credential.
        /// </summary>
        /// <param name="credential">The credential object containing the vcId and Proof.</param>
        /// <returns>True if the credential's Proof is valid, false otherwise.</returns>
        //Task<bool> VerifyCredentialProof(Credential credential);

        /// <summary>
        /// Integrates the verified presentation data into the application's business logic.
        /// This includes decoding inner credentials, validating status, and updating user data.
        /// </summary>
        /// <param name="verifiedPresentations">A list of fully verified presentation objects.</param>
        /// <param name="requestId">The request ID associated with the presentation.</param>
        Task IntegratePresentationData(List<DecodedPresentation> verifiedPresentations, string? requestId, List<string> verifiableAttributes);
    }
}