using Microsoft.AspNetCore.Mvc;
using UAE_Pass_Poc.Enums;
using UAE_Pass_Poc.Models.Request;
using UAE_Pass_Poc.ResponseHandlers;
using UAE_Pass_Poc.ResponseHandlers.Models;
using UAE_Pass_Poc.Services.Interfaces;

namespace UAE_Pass_Poc.Controllers
{
    [ApiController]
    [Route("api/document")]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpGet("jwt-token")]
        public ISuccessResult GenerateJwtToken([FromQuery] string accessCode)
        {
            var accessToken = _documentService.GenerateUAEPassAccessToken(accessCode, 1);
            return ResponseResult.Success(accessToken);
        }

        [HttpPost("request-presentation")]
        public async Task<ISuccessResult> RequestPresentation([FromBody] RequestPresentationModel request)
        {
            return ResponseResult.Success(await _documentService.RequestPresentationAsync(request));
        }

        [HttpPost("receive-presentation")]
        public async Task<ISuccessResult> ReceivePresentation([FromBody] ReceivePresentationModel request)
        {
            //log the entire request object for debugging purpose
            //log base64 data in a way to avoid data loss.
            return ResponseResult.Success(await _documentService.ReceivePresentationAsync(request));
        }

        [HttpPost("receive-visualization")]
        public async Task<ISuccessResult> ReceiveVisualization([FromBody] ReceiveVisualizationModel request)
        {
            //log the entire request object for debugging purpose
            //log base64 data in a way to avoid data loss.
            return ResponseResult.Success(await _documentService.ReceiveVisualizationAsync(request));
        }

        [HttpPost("reject-notification")]
        public async Task<ISuccessResult> RejectNotification([FromBody] RejectNotificationRequest request)
        {
            return ResponseResult.Success(await _documentService.RejectNotificationAsync(request));
        }

        [HttpGet("document-types")]
        public async Task<ISuccessResult> GetAvailableDocumentTypes([FromQuery] DocumentIssuer issuer)
        {
            return ResponseResult.Success(await _documentService.GetAvailableDocumentTypes(issuer));
        }

        [HttpPost("credential-status")]
        public async Task<ISuccessResult> GetCredentialStatus([FromBody] CredentialStatusRequest request)
        {
            return ResponseResult.Success(await _documentService.GetCredentialStatusAsync(request));
        }

        [HttpGet("presentation-request-status")]
        public async Task<ISuccessResult> GetPresentationRequestStatus([FromQuery] string proofOfPresentationRequestId)
        {
            return ResponseResult.Success(await _documentService.GetPresentationRequestStatusAsync(proofOfPresentationRequestId));
        }

        [HttpGet("getList-of-verified-attributes")]
        public async Task<ISuccessResult> GetListOfVerifiedAttributes()
        {
            return ResponseResult.Success(await _documentService.GetListOfVerfiedAttributesAsync());
        }
    }
}