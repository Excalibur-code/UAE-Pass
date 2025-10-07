namespace UAE_Pass_Poc.Models
{
    public class DidDocument
    {
        public string? Id { get; set; }
        public List<PublicKey>? PublicKey { get; set; }
    }

    public class PublicKey
    {
        public string? Id { get; set; }
        public string? Type { get; set; }
        public string? Controller { get; set; }
        public string? PublicKeyBase58 { get; set; } // The Base58 encoded public key
    }
}