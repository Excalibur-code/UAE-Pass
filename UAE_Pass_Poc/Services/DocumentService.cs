using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using UAE_Pass_Poc.DBContext;
using UAE_Pass_Poc.Entities;
using UAE_Pass_Poc.Enums;
using UAE_Pass_Poc.Exceptions;
using UAE_Pass_Poc.Models.Request;
using UAE_Pass_Poc.Models.Response;
using UAE_Pass_Poc.Services.Interfaces;

namespace UAE_Pass_Poc.Services;

public class DocumentService : IDocumentService
{
    private readonly ILogger<DocumentService> _logger;
    private readonly HttpClient _httpClient;
    private readonly ICadesVerificationService _cadesVerificationService;
    private readonly IPresentationProcessingService _presentationProcessingService;
    private readonly IMapper _mapper;
    private readonly UaePassDbContext _dbContext;
    private readonly string _documentStoragePath;
    private readonly string _uaePassSecret;
    private readonly string _partnerId;
    private readonly string _baseUri;
    private readonly string _accessCode;

    public DocumentService(ILogger<DocumentService> logger, IConfiguration configuration, HttpClient httpClient,
    ICadesVerificationService cadesVerificationService, IPresentationProcessingService presentationProcessingService,
    IMapper mapper, UaePassDbContext dbContext)
    {
        _logger = logger;
        _httpClient = httpClient;
        _cadesVerificationService = cadesVerificationService;
        _presentationProcessingService = presentationProcessingService;
        _mapper = mapper;
        _dbContext = dbContext;
        _documentStoragePath = configuration["UAEPass:DocumentStoragePath"] ?? "uaepass_documents";
        _uaePassSecret = configuration["UAEPass:Secret"] ?? "7bbfde6064b01a3c8389bcb689a6ecae";
        _partnerId = configuration["UAEPass:PartnerId"] ?? "did:uae:eth:c76036545911b577d6383ad4b1f593ae8f7982a2";
        _baseUri = configuration["UAEPass:BaseUri"] ?? "https://papistage.dv.government.net.ae";
        _accessCode = "461c43f6-f7e2-3055-9c6f-dde216c3549c";
    }

    #region Presentation Request Status
    public async Task<PresentationRequestStatusResponse> GetPresentationRequestStatusAsync(string proofOfPresentationRequestId)
    {
        if (string.IsNullOrWhiteSpace(proofOfPresentationRequestId))
        {
            throw new ArgumentException("Proof Of Presentation request Id is required.");
        }

        var url = $"{_baseUri}/papi/v1.0/presentation/{proofOfPresentationRequestId}/presentation-status";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("Authorization", $"Bearer {GenerateUAEPassAccessToken(_accessCode)}");
        var response = await SendAsync<PresentationRequestStatusResponse>(request);
        if (!response.IsSuccess)
        {
            throw new UaePassRequestException($"Get Presentation Request Status API Failed With Message: {response.Message} and Code: {response.Code}");
        }
        return response.Data ?? new PresentationRequestStatusResponse();
    }
    #endregion

    #region Credential Status
    public async Task<CredentialStatusResponse> GetCredentialStatusAsync(CredentialStatusRequest request)
    {
        if (string.IsNullOrEmpty(request.ProofOfPresentationId) || string.IsNullOrEmpty(request.RequestId) || string.IsNullOrEmpty(request.ProofOfIssuanceId))
        {
            throw new ArgumentException("All fields in CredentialStatusRequest are required.");
        }

        var url = $"{_baseUri}/papi/v1.0/presentation/credential-status";
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
        httpRequest.Headers.Add("Authorization", $"Bearer {GenerateUAEPassAccessToken(_accessCode)}");
        httpRequest.Content = CreateJsonContent(request);

        var response = await SendAsync<CredentialStatusResponse>(httpRequest);
        if (!response.IsSuccess)
        {
            throw new UaePassRequestException($"Get Credential Status Failed with Message: {response.Message} and Code : {response.Code}");
        }

        return response.Data ?? new CredentialStatusResponse();
    }
    #endregion

    #region Get List Of Document Types
    public async Task<List<DocumentTypesResponse>> GetAvailableDocumentTypes(DocumentIssuer issuer)
    {
        var url = $"{_baseUri}/papi/v2/document-types?types={issuer.ToString()}&lang=en";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("Authorization", $"Bearer {GenerateUAEPassAccessToken(_accessCode)}");
        var response = await SendAsync<List<DocumentTypesResponse>>(request);
        if (!response.IsSuccess)
        {
            throw new UaePassRequestException($"Get Document Types Failed with Message: {response.Message} and Code : {response.Code}");
        }
        return response.Data ?? new List<DocumentTypesResponse>();
    }
    #endregion

    #region Request For Documents
    public async Task<RequestPresentationResponseModel> RequestPresentationAsync(RequestPresentationModel model)
    {
        //to be setup internally.
        model.PartnerId = _partnerId;
        model.RequestId = "WASL" + DateTime.Now.ToString("yy") + "AE" + DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        _logger.LogInformation("Initiating Request Presentation process for RequestId: {RequestId}", model.RequestId);

        //Step 1 - Validate the model.
        var (isValid, validationErrors) = ValidatePresentationRequest(model);
        if (!isValid && validationErrors != null && validationErrors.Any())
        {
            _logger.LogWarning("Request Presentation model validation failed: {Errors}", string.Join(", ", validationErrors));
            throw new ArgumentException("Invalid request model", string.Join(", ", validationErrors));
        }

        //Step 2 - Configure request
        var url = $"{_baseUri}/papi/v2/presentation-requests";
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("Authorization", $"Bearer {GenerateUAEPassAccessToken(_accessCode)}");
        request.Content = CreateJsonContent(model);

        //Step 3 - Send request
        var response = await SendAsync<RequestPresentationResponseModel>(request);
        if (!response.IsSuccess)
        {
            _logger.LogError("Request Presentation API call failed. Message: {Message}, Code: {Code}", response.Message, response.Code);
            throw new UaePassRequestException($"Request For Presentation Failed with Message: {response.Message} and Code : {response.Code}");
        }

        //Step 4 - Store payload & response in database.
        var requestPresentationEntity = _mapper.Map<Entities.RequestPresentation>(model);
        _dbContext.RequestPresentations.Add(requestPresentationEntity);

        var responseMappingEntity = _mapper.Map<RequestPresentationResponseMapping>(response.Data);
        responseMappingEntity.RequestPresentationId = requestPresentationEntity.Id;
        responseMappingEntity.RequestId = model.RequestId;
        _dbContext.RequestPresentationResponseMappings.Add(responseMappingEntity);

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Request Presentation process completed successfully for RequestId: {RequestId}", model.RequestId);
        return response.Data ?? new RequestPresentationResponseModel();
    }

    private static (bool isValid, List<string>? errors) ValidatePresentationRequest(RequestPresentationModel model)
    {
        var errors = new List<string>();
        if (model.RequestedDocuments.Any(x => x.DocumentType == null))
        {
            var customDocs = model.RequestedDocuments.Where(x => x.DocumentType == null).ToList();
            foreach (var doc in customDocs)
            {
                if (string.IsNullOrEmpty(doc.CustomDocumentTypeEN) || string.IsNullOrEmpty(doc.CustomDocumentTypeAR))
                {
                    errors.Add("Custom document types must have both English and Arabic names.");
                }
            }

            if (customDocs.GroupBy(d => d.CustomDocumentTypeEN).Any(g => g.Count() > 1))
            {
                errors.Add("Custom document types must have unique names.");
            }

            if (customDocs.GroupBy(d => d.CustomDocumentTypeAR).Any(g => g.Count() > 1))
            {
                errors.Add("Custom document types must have unique Arabic names.");
            }

            var nonSelfSignedCustomDocs = customDocs.Where(d => d.SelfSignedAccepted == false || d.SelfSignedAccepted == null).ToList();
            if (nonSelfSignedCustomDocs.Any())
            {
                errors.Add("Custom document types must accept self-signed documents.");
            }
        }

        if (model.RequestedDocuments.Any(x => x.SingleInstanceRequested == false))
        {
            var multiInstanceDocs = model.RequestedDocuments.Where(x => x.SingleInstanceRequested is not null).ToList();
            foreach (var doc in multiInstanceDocs)
            {
                if (doc.Instances == null || !doc.Instances.Any())
                {
                    errors.Add("Documents allowing single/multiple instances must specify at least one instance.");
                }

                var instances = doc.Instances;
                if (instances!.GroupBy(i => i.Name).Any(g => g.Count() > 1))
                {
                    errors.Add("Document instances must have unique names.");
                }

                if (instances!.Any(i => string.IsNullOrEmpty(i.Name) || string.IsNullOrEmpty(i.Value)))
                {
                    errors.Add("Each document instance must have both a name and a value.");
                }
            }
        }

        if(errors.Any())
        {
            return (false, errors);
        }

        return (true, null);
    }
    #endregion

    #region Receive Presentation - Webhook
    public async Task<PresentationReceiveResponse> ReceivePresentationAsync(ReceivePresentationModel model)
    {
        if (string.IsNullOrEmpty(model.ProofOfPresentationId)
         && string.IsNullOrEmpty(model.ProofOfPresentationRequestId)
         && string.IsNullOrEmpty(model.QrId))
        {
            _logger.LogWarning("Missing required identifiers in ReceivePresentationModel.");
            throw new ArgumentException("One of ProofOfPresentationId, ProofOfPresentationRequestId, or QrId must be provided.");
        }

        if (model.SignedPresentation == null || !model.SignedPresentation.Any())
        {
            _logger.LogWarning("Missing SignedPresentation in ReceivePresentationModel.");
            throw new ArgumentException("SignedPresentation is mandatory.");
        }

        if (string.IsNullOrEmpty(model.CitizenSignature))
        {
            _logger.LogWarning("Missing CitizenSignature in ReceivePresentationModel.");
            throw new ArgumentException("CitizenSignature is mandatory.");
        }

        // --- 2. CAdES Verification of CitizenSignature (Outer Signature) ---
        // The CitizenSignature is on the SHA256 hash of the *entire* signedPresentation payload.
        // To get this, we need to concatenate the raw Base64 strings of SignedPresentation
        // and then hash that combined string.
        string combinedSignedPresentationString = string.Join("", model.SignedPresentation);
        byte[] combinedSignedPresentationBytes = Encoding.UTF8.GetBytes(combinedSignedPresentationString);

        byte[] hashOfCombinedSignedPresentation;
        using (SHA256 sha256Hash = SHA256.Create())
        {
            hashOfCombinedSignedPresentation = sha256Hash.ComputeHash(combinedSignedPresentationBytes);
        }
        string hashHex = BitConverter.ToString(hashOfCombinedSignedPresentation).Replace("-", "").ToLowerInvariant();
        _logger.LogDebug($"Calculated SHA256 Hash of combined SignedPresentation for CAdES: {hashHex}");

        bool isCadesSignatureValid = _cadesVerificationService.ValidateCADESignature(
            model.CitizenSignature,
            string.Empty
        );

        if (!isCadesSignatureValid)
        {
            _logger.LogWarning($"CAdES signature verification failed for RequestId: {model.ProofOfPresentationRequestId}");
            throw new UaePassRequestException("Invalid CitizenSignature.");
        }
        _logger.LogInformation("CAdES signature successfully verified.");

        // --- 3. Process Decoded Presentation Data (Inner Structure) ---
        List<DecodedPresentation> decodedPresentations = await _presentationProcessingService.ProcessSignedPresentation(model.SignedPresentation);

        // --- 4. Verify Proof Object (Non-CAdES Citizen Signature within each presentation) ---
        foreach (var presentation in decodedPresentations)
        {
            bool isPresentationProofValid = await _presentationProcessingService.VerifyPresentationProof(presentation);
            if (!isPresentationProofValid)
            {
                _logger.LogWarning($"Presentation Proof verification failed for presentation subject: {presentation.PresentationSubject}");
                throw new UaePassRequestException("Invalid Proof signature within presentation.");
            }
        }
        _logger.LogInformation("All internal Presentation Proof signatures successfully verified.");

        // --- 5. Integrate with Business Logic (now includes credential-level verification) ---
        await _presentationProcessingService.IntegratePresentationData(decodedPresentations, model.ProofOfPresentationRequestId);

        // --- 6. Generate PresentationReceiptID ---
        string presentationReceiptId = Guid.NewGuid().ToString(); // Or generate based on your internal logic

        _logger.LogInformation($"Successfully processed Presentation Request for RequestId: {model.ProofOfPresentationRequestId}. Generated Receipt ID: {presentationReceiptId}");

        return new PresentationReceiveResponse { PresentationReceiptID = presentationReceiptId };
    }
    #endregion

    #region UAE Pass JWT Token
    public string GenerateUAEPassAccessToken(string accessCode, int expirationHours = 1)
    {
        _logger.LogInformation("Generating UAE Pass Access Token from access code {accessCode} with expiration {expirationHours} hours.", accessCode, expirationHours);
        if (string.IsNullOrEmpty(accessCode))
            return string.Empty;

        //setup
        var partnerId = _partnerId;
        var secret = _uaePassSecret;
        //setup

        // Current timestamp and expiration
        var currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var expirationTime = currentTime + (expirationHours * 3600);

        // JWT Header
        var header = new
        {
            alg = "HS512"
        };

        // JWT Payload
        var payload = new
        {
            sub = accessCode,
            iss = partnerId,
            iat = currentTime,
            exp = expirationTime
        };

        // Serialize to JSON
        var headerJson = JsonSerializer.Serialize(header);
        var payloadJson = JsonSerializer.Serialize(payload);

        // Base64 URL encode
        var headerBase64 = Base64UrlEncode(Encoding.UTF8.GetBytes(headerJson));
        var payloadBase64 = Base64UrlEncode(Encoding.UTF8.GetBytes(payloadJson));

        // Create message to sign
        var message = $"{headerBase64}.{payloadBase64}";

        // Generate HMAC-SHA512 signature
        var signature = GenerateHmacSha512Signature(message, secret);
        var signatureBase64 = Base64UrlEncode(signature);

        // Return complete JWT token
        _logger.LogInformation("UAE Pass Access Token generated successfully.");
        return $"{headerBase64}.{payloadBase64}.{signatureBase64}";
    }

    /// <summary>
    /// Generates HMAC-SHA512 signature
    /// </summary>
    private byte[] GenerateHmacSha512Signature(string message, string secret)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secret);
        var messageBytes = Encoding.UTF8.GetBytes(message);

        using (var hmac = new HMACSHA512(keyBytes))
        {
            return hmac.ComputeHash(messageBytes);
        }
    }

    /// <summary>
    /// Base64 URL encoding (without padding)
    /// </summary>
    private string Base64UrlEncode(byte[] input)
    {
        var base64 = Convert.ToBase64String(input);
        // Convert to Base64 URL format
        return base64.Replace('+', '-')
                    .Replace('/', '_')
                    .TrimEnd('=');
    }

    /// <summary>
    /// Base64 URL decoding
    /// </summary>
    private byte[] Base64UrlDecode(string input)
    {
        var base64 = input.Replace('-', '+').Replace('_', '/');

        // Add padding if needed
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }

        return Convert.FromBase64String(base64);
    }
    #endregion

    #region Receive Visualization - Webhook
    public async Task<VisualizationReceivedResponse> ReceiveVisualizationAsync(ReceiveVisualizationModel model)
    {
        //NOTE - Visualization Info and Evidence Info must be persisted in system as an audit trail as well as for future disputes.
        //Issuer Signature field for audit trail and future disputes.

        // ----- FOR VISUALIZATION -----
        //Step 1 - Validate the CAdES Signature.
        //Step 1.1 - Take SHA256 hash of model.VisualizationInfo
        //Step 1.2 - Unwrap data inside model.IssuerSignature using open DSS library -> Match both hash. they should match.

        //Step 2 - Decode base64 of model.visualizationInfo to get actual visualization and data used to generate it.
        //Step 2.1 - Match the data received here with the corresponding data received in the receive-presentation API.

        // ----- FOR EVIDENCE -----
        // -- NEED SOME CLARITY ON THIS STEP.

        return new VisualizationReceivedResponse()
        {
            EvidenceVisualizationReceiptID = Guid.NewGuid().ToString()
        };
    }
    #endregion

    #region Reject Notification
    public async Task<RejectNotificationResponse> RejectNotificationAsync(RejectNotificationRequest model)
    {
        //Match Proof Of Presentation Request ID from the DB
        //If exists, then proceed.

        //1. USER_REJECTED –
        // User has intentionally rejected the sharing request. Service Provider has to mark the request complete and update as failure.
        // (Permanent failure)
        // 2. USER_EXITED – User is not proceed with the request. Even though user has an option to open UAEPASS application again and share the request if this is still active.
        // 3. UAEPASS_ERROR – There is an intermittent failure happened during the flow. User has an option to retry the sharing process and complete the journey.
        // (Intermittent failure)
        return new RejectNotificationResponse() { PresentationRejectID = Guid.NewGuid().ToString() };
    }
    #endregion

    #region Get List Of Verfied Attributes
    public async Task<List<VerifiedAttributesResponse>> GetListOfVerfiedAttributesAsync()
    {
        var url = $"{_baseUri}/papi/v1.0/verified-attributes?type=ALL";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("Authorization", $"Bearer {GenerateUAEPassAccessToken(_accessCode)}");
        var response = await SendAsync<List<VerifiedAttributesResponse>>(request);
        return response.Data ?? new List<VerifiedAttributesResponse>();
    }
    #endregion

    #region Request Helpers
    public static HttpContent CreateJsonContent<T>(T model)
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        var jsonContent = JsonSerializer.Serialize(model, options);
        return new StringContent(jsonContent, Encoding.UTF8, "application/json");
    }

    private async Task<RestResponseModel<T>> SendAsync<T>(HttpRequestMessage request)
    {
        try
        {
            _logger.LogDebug("Sending HTTP request to: {Url}", request.RequestUri);

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("Received response with status code {StatusCode}", response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("HTTP request failed with status code {StatusCode} and reason: {Reason}", response.StatusCode, response.ReasonPhrase);

                // Attempt to deserialize error response           
                var errorModel = JsonSerializer.Deserialize<RestResponseModel<T>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return new RestResponseModel<T>
                {
                    IsSuccess = false,
                    StatusCode = (int)response.StatusCode,
                    Data = default,
                    Token = errorModel?.Token,
                    Timestamp = errorModel?.Timestamp,
                    Code = errorModel?.Code,
                    Message = $"Error: {errorModel?.Message}"
                };
            }

            var data = JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (object.Equals(data, default(T)))
            {
                _logger.LogWarning("Deserialization returned null for type {Type}", typeof(T).Name);
                return new RestResponseModel<T>
                {
                    IsSuccess = false,
                    StatusCode = (int)response.StatusCode,
                    Message = "Deserialization returned null",
                    Data = default
                };
            }

            _logger.LogDebug("Deserialization successful for type {Type}", typeof(T).Name);

            return new RestResponseModel<T>
            {
                IsSuccess = true,
                StatusCode = (int)response.StatusCode,
                Message = "Success",
                Data = data
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while sending HTTP request.");
            return new RestResponseModel<T>
            {
                IsSuccess = false,
                StatusCode = 500,
                Message = $"Exception: {ex.Message}",
                Data = default
            };
        }
    }
    #endregion
}