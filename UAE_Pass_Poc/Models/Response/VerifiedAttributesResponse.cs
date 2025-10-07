namespace UAE_Pass_Poc.Models.Response;

public class VerifiedAttributesResponse
{
    public string AttributeName { get; set; } = string.Empty; //Name of the UAEPASS Attribute which needs to be passed as part of Presentation Request to get this data
    public string Description { get; set; } = string.Empty;//Detail of attribute
    public string Language { get; set; } = string.Empty;//ISO 639-1 Language code. e.g. English = en and Arabic = ar
}