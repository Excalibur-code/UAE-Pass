using UAE_Pass_Poc.Enums;
using UAE_Pass_Poc.Models.Request;
using UAE_Pass_Poc.Models.Response;

namespace UAE_Pass_Poc.Services.Interfaces;

public interface IDocumentService
{
    /// <summary>
    /// return respective presentation request’s status and error category code and corresponding message.
    ///  Supposed to invoke this API for any Presentation Request if it get expired but had 
    /// not been shared with them by UAEPASS user.
    /// </summary>
    /// <param name="ProofOfPresentationRequestId"></param>
    /// <returns></returns>
    Task<PresentationRequestStatusResponse> GetPresentationRequestStatusAsync(string proofOfPresentationRequestId);
    /// <summary>
    /// For verifying a Verifiable Credential separately, shared by a Citizen
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<CredentialStatusResponse> GetCredentialStatusAsync(CredentialStatusRequest request);
    /// <summary>
    /// Get Available Document Types from Uae Pass. - Self Issued or Issued by Government Entity
    /// </summary>
    /// <param name="issuer"></param>
    /// <returns></returns>
    Task<List<DocumentTypesResponse>> GetAvailableDocumentTypes(DocumentIssuer issuer);
    /// <summary>
    /// To generate jwt token used by uae pass APIs.
    /// </summary>
    /// <param name="accessCode"></param>
    /// <param name="expirationHours"></param>
    /// <returns></returns>
    string GenerateUAEPassAccessToken(string accessCode, int expirationHours = 1);
    /// <summary>
    /// Raise Request for Documents from Uae Pass.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    Task<RequestPresentationResponseModel> RequestPresentationAsync(RequestPresentationModel model);
    /// <summary>
    /// Receive Presentation or Credential From Uae Pass.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    Task<PresentationReceiveResponse> ReceivePresentationAsync(ReceivePresentationModel model);
    /// <summary>
    /// Receive Visualization or Evidence file for an already shared Credential from UAE Pass.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    Task<VisualizationReceivedResponse> ReceiveVisualizationAsync(ReceiveVisualizationModel model);
    /// <summary>
    /// User Rejected the Presentation Request. 
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    Task<RejectNotificationResponse> RejectNotificationAsync(RejectNotificationRequest model);
    /// <summary>
    /// Get List of Verified Attributes to be passed as Presentation Request from UAE Pass.
    /// </summary>
    /// <returns></returns>
    Task<List<VerifiedAttributesResponse>> GetListOfVerfiedAttributesAsync();
}
