extern alias BCCryptography;

using System.Text;
using Nethereum.Signer;
using Nethereum.Hex.HexConvertors.Extensions;
using BigInteger = System.Numerics.BigInteger;
using BouncyBigInteger = BCCryptography::Org.BouncyCastle.Math.BigInteger;
using UAE_Pass_Poc.Exceptions;

namespace UAE_Pass_Poc.Services
{
    public class SignatureValidator : ISignatureValidator
    {
        private readonly ILogger<SignatureValidator> _logger;

        public SignatureValidator(ILogger<SignatureValidator> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Validates signature
        /// </summary>
        public bool ValidateSignature(string payload, string publicKey, string signature)
        {
            // First, let's decode the expected public key to see what it contains
            DecodeExpectedPublicKey(publicKey);

            byte[] r = signature.Substring(0, 64).HexToByteArray();
            byte[] s = signature.Substring(64, 64).HexToByteArray();
            byte v = signature.Substring(128).HexToByteArray()[0];

            bool isValid = ValidateSignature(payload, publicKey, v, r, s) ||
                           ValidateHashedSignature(payload, publicKey, v, r, s);  
            return isValid;
        }

        /// <summary>
        /// Validates signature treating payload as RAW MESSAGE (will hash it with Ethereum prefix)
        /// </summary>
        public static bool ValidateSignature(string payload, string publicKeyAsBase58, byte v, byte[] r, byte[] s)
        {
            try
            {
                var payloadBytes = payload.HexToByteArray();
                
                // Create EthECDSASignature using BouncyCastle BigIntegers
                var rBig = new BouncyBigInteger(1, r);
                var sBig = new BouncyBigInteger(1, s);
                var signature = new EthECDSASignature(rBig, sBig, new byte[] { v });

                // Hash the message WITH Ethereum prefix
                var signer = new EthereumMessageSigner();
                var hashedMessage = signer.HashPrefixedMessage(payloadBytes);
                
                // // Try both recovery IDs
                // for (int rid = 0; rid <= 1; rid++)
                // {
                //     try
                //     {
                //         var ecKey = EthECKey.RecoverFromSignature(signature, rid, hashedMessage);
                        
                //         if (ecKey != null)
                //         {
                //             var result = CheckPublicKeyMatch(ecKey, publicKeyAsBase58, rid);
                //             if (result) return true;
                //         }
                //     }
                //     catch { }
                // }
                
                // return false;

                int recoveryId = v >= 27 ? v - 27 : v;
                var ecKey = EthECKey.RecoverFromSignature(signature, recoveryId, payloadBytes);

                if (ecKey == null)
                    return false;

                var result = CheckPublicKeyMatch(ecKey, publicKeyAsBase58, recoveryId);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Validates signature treating payload as ALREADY HASHED
        /// </summary>
        public static bool ValidateHashedSignature(string payload, string publicKeyAsBase58, byte v, byte[] r, byte[] s)
        {
            try
            {
                var payloadBytes = payload.HexToByteArray();

                // Create EthECDSASignature using BouncyCastle BigIntegers
                var rBig = new BouncyBigInteger(1, r);
                var sBig = new BouncyBigInteger(1, s);
                var signature = new EthECDSASignature(rBig, sBig, new byte[] { v });

                // // Try both recovery IDs
                // for (int rid = 0; rid <= 1; rid++)
                // {
                //     try
                //     {
                //         var ecKey = EthECKey.RecoverFromSignature(signature, rid, payloadBytes);

                //         if (ecKey != null)
                //         {
                //             var result = CheckPublicKeyMatch(ecKey, publicKeyAsBase58, rid);
                //             if (result) return true;
                //         }
                //     }
                //     catch { }
                // }

                int recoveryId = v >= 27 ? v - 27 : v;
                var ecKey = EthECKey.RecoverFromSignature(signature, recoveryId, payloadBytes);

                if (ecKey == null)
                    return false;
                
                var result = CheckPublicKeyMatch(ecKey, publicKeyAsBase58, recoveryId);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Helper method to check if recovered public key matches expected
        /// </summary>
        private static bool CheckPublicKeyMatch(EthECKey ecKey, string publicKeyAsBase58, int recoveryId)
        {   
            // Get uncompressed public key
            var pubKeyUncompressed = ecKey.GetPubKey(false);
            
            // Skip the 0x04 prefix
            byte[] keyToUse = pubKeyUncompressed.Length == 65 && pubKeyUncompressed[0] == 0x04
                ? pubKeyUncompressed.Skip(1).ToArray()
                : pubKeyUncompressed;
            
            var pubKeyRecovered = new BigInteger(keyToUse, isUnsigned: true, isBigEndian: true);
            var recoveredBase58 = PublicKeyAsBase58(pubKeyRecovered);
            
            bool isValid = string.Equals(publicKeyAsBase58, recoveredBase58, StringComparison.OrdinalIgnoreCase);
            
            return isValid;
        }

        /// <summary>
        /// Converts public key to Base58 format
        /// </summary>
        public static string PublicKeyAsBase58(BigInteger publicKey)
        {
            var hexString = PublicKeyAsHexString(true, 128, publicKey);
            return Base58.Encode(Encoding.UTF8.GetBytes(hexString));
        }

        /// <summary>
        /// Converts public key to hex string
        /// </summary>
        public static string PublicKeyAsHexString(bool withPrefix, int size, BigInteger publicKey)
        {
            string publicKeyHexStr = publicKey.ToString("x");

            if (publicKeyHexStr.StartsWith("-"))
            {
                publicKeyHexStr = publicKeyHexStr.Substring(1);
            }

            if (publicKeyHexStr.Length < size)
            {
                publicKeyHexStr = publicKeyHexStr.PadLeft(size, '0');
            }
            else if (publicKeyHexStr.Length > size)
            {
                publicKeyHexStr = publicKeyHexStr.Substring(publicKeyHexStr.Length - size);
            }

            if (withPrefix)
            {
                publicKeyHexStr = "0x" + publicKeyHexStr;
            }

            return publicKeyHexStr;
        }
        
        #region Private Methods
        
        /// <summary>
        /// Decode the expected public key to understand its format
        /// </summary>
        private static void DecodeExpectedPublicKey(string publicKeyBase58)
        {
            try
            {
                // Decode from Base58
                byte[] decodedBytes = Base58.Decode(publicKeyBase58);
                
                // Try to interpret as UTF-8 string (since Java encodes hex string as UTF-8)
                string decodedString = Encoding.UTF8.GetString(decodedBytes);
                
                // If it starts with "0x", it's a hex string
                if (decodedString.StartsWith("0x"))
                {
                    string hexPart = decodedString.Substring(2);
                    
                    // Try to parse as BigInteger
                    try
                    {
                        BigInteger pubKeyBigInt = BigInteger.Parse("0" + hexPart, System.Globalization.NumberStyles.HexNumber);
                    }
                    catch (Exception ex)
                    {
                        throw new UaePassRequestException($"Could not parse as BigInteger: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new UaePassRequestException($"Error decoding public key: {ex.Message}");
            }
        }
        #endregion
    }
}