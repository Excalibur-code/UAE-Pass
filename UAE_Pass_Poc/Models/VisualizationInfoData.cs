using System;
using System.Xml;
using System.Xml.Serialization;


namespace UAE_Pass_Poc.Models;

[XmlRoot("Data")]
public class VisualizationInfoData
{
    [XmlElement("Context")]
    public string Context { get; set; } = string.Empty;

    [XmlElement("documentType")]
    public string DocumentType { get; set; } = string.Empty;

    [XmlElement("issuerId")]
    public string IssuerId { get; set; } = string.Empty;

    [XmlElement("credentialSubject")]
    public string CredentialSubject { get; set; } = string.Empty;

    [XmlElement("claim")]
    public Claim Claim { get; set; } = new();
    
    [XmlElement("visualization")]
    public Visualization Visualizations { get; set; } = new();

}


public class Visualization
{
    [XmlAnyElement]
    public XmlElement Content { get; set; } = null!;

    // Main tag helper
    [XmlIgnore]
    public string Kind => Content?.Name; // e.g., "passportImage" or "emiratesID"

    [XmlIgnore]
    public string InnerXml => Content?.InnerXml;

    [XmlIgnore]
    public string InnerText => Content?.InnerText;
}

public class Claim
{
    [XmlElement("arabicCardNumberLabel")]
    public string ArabicCardNumberLabel { get; set; } = string.Empty;

    [XmlElement("arabicDateOfBirthLabel")]
    public string ArabicDateOfBirthLabel { get; set; } = string.Empty;

    [XmlElement("arabicExpiryDateLabel")]
    public string ArabicExpiryDateLabel { get; set; } = string.Empty;

    [XmlElement("arabicGenderLabel")]
    public string ArabicGenderLabel { get; set; } = string.Empty;

    [XmlElement("arabicIDNumberLabel")]
    public string ArabicIDNumberLabel { get; set; } = string.Empty;

    [XmlElement("arabicNameLabel")]
    public string ArabicNameLabel { get; set; } = string.Empty;

    [XmlElement("arabicNameOfDocLabel")]
    public string ArabicNameOfDocLabel { get; set; } = string.Empty;

    [XmlElement("arabicNationalityLabel")]
    public string ArabicNationalityLabel { get; set; } = string.Empty;

    [XmlElement("arabicSignatureLabel")]
    public string ArabicSignatureLabel { get; set; } = string.Empty;

    [XmlElement("cardNumber")]
    public string CardNumber { get; set; } = string.Empty;

    [XmlElement("dateOfBirth")]
    public string DateOfBirthRaw { get; set; } = string.Empty;  // raw date is ISO-8601 (YYYY-MM-DD)

    [XmlElement("documentTypeCode")]
    public int? DocumentTypeCode { get; set; }

    [XmlElement("englishCardNumberLabel")]
    public string EnglishCardNumberLabel { get; set; } = string.Empty;

    [XmlElement("englishDateOfBirthLabel")]
    public string EnglishDateOfBirthLabel { get; set; } = string.Empty;

    [XmlElement("englishExpiryDateLabel")]
    public string EnglishExpiryDateLabel { get; set; } = string.Empty;

    [XmlElement("englishGenderLabel")]
    public string EnglishGenderLabel { get; set; } = string.Empty;

    [XmlElement("englishIDNumberLabel")]
    public string EnglishIDNumberLabel { get; set; } = string.Empty;

    [XmlElement("englishNameLabel")]
    public string EnglishNameLabel { get; set; } = string.Empty;

    [XmlElement("englishNameOfDocLabel")]
    public string EnglishNameOfDocLabel { get; set; } = string.Empty;

    [XmlElement("englishNationalityLabel")]
    public string EnglishNationalityLabel { get; set; } = string.Empty;

    [XmlElement("englishSignatureLabel")]
    public string EnglishSignatureLabel { get; set; } = string.Empty;

    [XmlElement("expiryDate")]
    public string ExpiryDateRaw { get; set; } = string.Empty;

    [XmlElement("genderAr")]
    public string GenderAr { get; set; } = string.Empty;

    [XmlElement("genderCode")]
    public int? GenderCode { get; set; }

    [XmlElement("genderEn")]
    public string GenderEn { get; set; } = string.Empty;

    [XmlElement("IDN")]
    public string IDN { get; set; } = string.Empty;

    [XmlElement("nameAr")]
    public string NameAr { get; set; } = string.Empty;

    [XmlElement("nameEn")]
    public string NameEn { get; set; } = string.Empty;

    [XmlElement("nameOfDocAr")]
    public string NameOfDocAr { get; set; } = string.Empty;

    [XmlElement("nameOfDocEn")]
    public string NameOfDocEn { get; set; } = string.Empty;

    [XmlElement("nationalityAr")]
    public string NationalityAr { get; set; } = string.Empty;

    [XmlElement("nationalityCode")]
    public string NationalityCode { get; set; } = string.Empty;

    [XmlElement("nationalityEn")]
    public string NationalityEn { get; set; } = string.Empty;

    [XmlIgnore]
    public DateTime? DateOfBirth =>
        TryParseDate(DateOfBirthRaw);

    [XmlIgnore]
    public DateTime? ExpiryDate =>
        TryParseDate(ExpiryDateRaw);

    private static DateTime? TryParseDate(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;
        if (DateTime.TryParse(s, out var dt)) return dt.Date;
        if (DateTime.TryParseExact(s, new[] { "yyyy-MM-dd", "yyyy-MM-ddTHH:mm:ssK" },
                                   System.Globalization.CultureInfo.InvariantCulture,
                                   System.Globalization.DateTimeStyles.AssumeUniversal, out dt))
            return dt.Date;
        return null;
    }
}