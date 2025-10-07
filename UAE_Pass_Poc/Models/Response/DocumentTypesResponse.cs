namespace UAE_Pass_Poc.Models.Response;

public class DocumentTypesResponse
{
    public string Name { get; set; } = string.Empty; //Name/Description of Document
    public string DocMnemonic { get; set; } = string.Empty; //Document Type used by uae pass dv
    public string CredentialType { get; set; } = string.Empty; //VC Type | eg. ISSUED or Verified, SELF -> for self issued
    public string IssuingEntity { get; set; } = string.Empty; //Entity issuing the document; For Self signed - NA
    public string TypeOfEntity { get; set; } = string.Empty; //FGE, PSE, LGE
    //FGE - Federal Government Entity, PGE - Private Sector Entity, LGE - Local Government Entity
    public string EmiratesCode { get; set; } = string.Empty; //Emirates Code of Local Issuing Entity - AD, AJM, FUJ, SHJ, DXB, RAK, UAQ
    public string Language { get; set; } = string.Empty; //Language - EN, AR - ISO 639-1 language code
    public string? SupplementaryInfo { get; set; } = null; //Eidence or Visualization Or None
    public List<string>? Parameters { get; set; } = null; //Applicable to credential instance in which multiple instances are
    //applicable. this is used to identify a specific instance from the multiple present.
}