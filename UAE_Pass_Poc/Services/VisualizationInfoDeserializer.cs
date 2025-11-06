using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UAE_Pass_Poc.Models;

namespace UAE_Pass_Poc.Services;

public static class VisualizationInfoDeserializer
{
    /// <summary>
    /// Deserialize from a Base64-encoded XML string.
    /// </summary>
    public static VisualizationInfoData FromBase64(string base64)
    {
        var xmlBytes = Convert.FromBase64String(base64);
        using var ms = new MemoryStream(xmlBytes);
        return FromStream(ms);
    }

    /// <summary>
    /// Deserialize from a raw XML string (UTF-8).
    /// </summary>
    public static VisualizationInfoData FromString(string xml)
    {
        var bytes = Encoding.UTF8.GetBytes(xml);
        using var ms = new MemoryStream(bytes);
        return FromStream(ms);
    }

    /// <summary>
    /// Deserialize from a stream with secure XmlReader settings.
    /// </summary>
    public static VisualizationInfoData FromStream(Stream xmlStream)
    {
        // Secure reader: prohibit DTD & external entities (protects against XXE)
        var settings = new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Prohibit,
            IgnoreComments = true,
            IgnoreWhitespace = true,
            XmlResolver = null // important: disallow external entity resolution
        };

        using var reader = XmlReader.Create(xmlStream, settings);
        var serializer = new XmlSerializer(typeof(VisualizationInfoData));
        var obj = (VisualizationInfoData?)serializer.Deserialize(reader);

        if (obj is null)
            throw new InvalidDataException("Failed to deserialize VisualizationInfoData from XML.");

        return obj;
    }
}