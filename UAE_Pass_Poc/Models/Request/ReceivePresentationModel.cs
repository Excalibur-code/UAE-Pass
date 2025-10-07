namespace UAE_Pass_Poc.Models.Request
{
    public class ReceivePresentationModel
    {
        public string? ProofOfPresentationId { get; set; } = null;
        public string? ProofOfPresentationRequestId { get; set; } = null;
        public string? QrId { get; set; } = null; //Mandatory if ProofOfPresentationRequestId is not provided

        //BASE64 Encoded Presentation structure from the presenter. CAdES signing has been done on the Hash of this Payload.
        //Internal Structure of the decoded presentation is explained in Appendix 8
        public List<string>? SignedPresentation { get; set; } = null; //Mandatory if ProofOfPresentationId is not provided

        //UAEPASS CAdES signature of Citizen on the SHA256 hash of signedPresentation.
        //This can be verified easily by taking SHA3 256 hash of signedPresentation and excute CAdES Verification process on top of it.
        //Check Appendix-VI for more information and sample code.
        public string? CitizenSignature { get; set; } = null; 
    }
}