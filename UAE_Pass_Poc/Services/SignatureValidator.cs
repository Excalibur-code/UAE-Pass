extern alias BCCrypto;
extern alias BCCryptography;

using Nethereum.Signer;
using Nethereum.Hex.HexConvertors.Extensions;
using SimpleBase;
using BigInteger = BCCryptography::Org.BouncyCastle.Math.BigInteger;
using System.Text;

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
        /// payload - (id or vcId as applicable)
        /// publicKey - (publicKeyBase58 - proof section)
        /// signature - (signature - proof section)
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="publicKey"></param>
        /// <param name="signature"></param>
        /// <returns></returns>
        public bool ValidateSignature(string payload, string publicKey, string signature)
        {
            if (string.IsNullOrWhiteSpace(payload) ||
                string.IsNullOrWhiteSpace(publicKey) ||
                string.IsNullOrWhiteSpace(signature))
            {
                _logger.LogWarning("Invalid input parameters for signature validation.");
                return false;
            }

            var r = HexStringToByteArray(signature.Substring(0, 64));
            var s = HexStringToByteArray(signature.Substring(64, 64));
            var v = HexStringToByteArray(signature.Substring(128))[0];

            bool flag = ValidateSignature(payload, publicKey, v, r, s) || ValidateHashedSignature(payload, publicKey, v, r, s);
            return flag;
        }

        public bool ValidateHashedSignature(string payLoad, string publicKeyAsBase58, byte v, byte[] r, byte[] s)
        {
            try
            {
                // Convert hex string to byte array
                byte[] messageHash = payLoad.HexToByteArray();

                // // Create signature from components - convert byte arrays to BigInteger
                var rBigInt = new BigInteger(r.Reverse().Concat(new byte[] { 0 }).ToArray());
                var sBigInt = new BigInteger(s.Reverse().Concat(new byte[] { 0 }).ToArray());
                var signature = new EthECDSASignature(rBigInt, sBigInt, new byte[] { v });

                // // For recovery, use the recovery ID (0 or 1)
                // // If v is 27 or 28 (Ethereum standard), subtract 27
                // // If v is already 0 or 1, use it directly
                int recoveryId = v >= 27 ? v - 27 : v;

                // // Recover public key from signature
                var ecKey = EthECKey.RecoverFromSignature(signature, recoveryId, messageHash);

                if (ecKey == null)
                {
                    throw new Exception("Failed to recover public key from signature");
                }

                // Get public key as BigInteger
                byte[] pubKeyBytes = ecKey.GetPubKey();

                // Remove the 0x04 prefix for uncompressed keys
                if (pubKeyBytes.Length == 65 && pubKeyBytes[0] == 0x04)
                {
                    pubKeyBytes = pubKeyBytes.Skip(1).ToArray();
                }

                BigInteger pubKeyRecovered = new BigInteger(pubKeyBytes.Reverse().Concat(new byte[] { 0 }).ToArray());

                var pubKeyRecoveredBase58 = PublicKeyAsBase58(pubKeyRecovered);
                _logger.LogInformation($"Recovered Public Key (Base58): {pubKeyRecoveredBase58}");
                return publicKeyAsBase58.Equals(pubKeyRecoveredBase58, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid signature");
            }
        }

        /// <summary>
        /// Converts a public key to Base58 encoding
        /// </summary>
        /// <param name="publicKey">The public key as BigInteger</param>
        /// <returns>Base58 encoded public key</returns>
        public static string PublicKeyAsBase58(BigInteger publicKey)
        {
            string hexString = PublicKeyAsHexString(true, 128, publicKey);
            byte[] bytes = Encoding.UTF8.GetBytes(hexString);
            return Base58.Bitcoin.Encode(bytes);
        }

        /// <summary>
        /// Converts a public key to hex string representation
        /// </summary>
        /// <param name="withPrefix">Whether to include "0x" prefix</param>
        /// <param name="size">The desired string length (padding with zeros)</param>
        /// <param name="publicKey">The public key as BigInteger</param>
        /// <returns>Hex string representation of public key</returns>
        public static string PublicKeyAsHexString(bool withPrefix, int size, BigInteger publicKey)
        {
            // Convert BigInteger to byte array and then to hex string
            byte[] bytes = publicKey.ToByteArray();

            // BigInteger.ToByteArray() returns little-endian with sign byte
            // Reverse to get big-endian and remove trailing zero if present
            if (bytes[bytes.Length - 1] == 0)
            {
                Array.Resize(ref bytes, bytes.Length - 1);
            }
            Array.Reverse(bytes);

            // Convert to hex string
            string publicKeyHexStr = BitConverter.ToString(bytes).Replace("-", "").ToLower();

            // Pad with leading zeros
            if (publicKeyHexStr.Length < size)
            {
                publicKeyHexStr = new string('0', size - publicKeyHexStr.Length) + publicKeyHexStr;
            }
            // Truncate if too long
            // else if (publicKeyHexStr.Length > size)
            // {
            //     publicKeyHexStr = publicKeyHexStr.Substring(publicKeyHexStr.Length - size);
            // }

            if (withPrefix)
            {
                publicKeyHexStr = "0x" + publicKeyHexStr;
            }

            return publicKeyHexStr;
        }

        private static byte[] HexStringToByteArray(string hex)
        {
            int length = hex.Length;
            byte[] bytes = new byte[length / 2];
            for (int i = 0; i < length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }


        /// <summary>
        /// Validates an Ethereum signature
        /// </summary>
        /// <param name="payload">The message payload as hex string</param>
        /// <param name="publicKeyAsBase58">The public key in Base58 format</param>
        /// <param name="v">The recovery id</param>
        /// <param name="r">The r component of the signature</param>
        /// <param name="s">The s component of the signature</param>
        /// <returns>True if signature is valid, false otherwise</returns>
        public bool ValidateSignature(string payload, string publicKeyAsBase58, byte v, byte[] r, byte[] s)
        {
            try
            {
                // Convert hex payload to bytes
                byte[] payloadBytes = payload.HexToByteArray();

                // Create EthECDSASignature from r and s
                // Ensure positive BigInteger by appending 0x00 if needed
                var rBigInt = new BigInteger(r.Reverse().Concat(new byte[] { 0 }).ToArray());
                var sBigInt = new BigInteger(s.Reverse().Concat(new byte[] { 0 }).ToArray());

                var signature = new EthECDSASignature(rBigInt, sBigInt, new byte[] { v });

                int recoveryId = v >= 27 ? v - 27 : v;
                //var ecKey = EthECKey.RecoverFromSignature(signature, payloadBytes);

                var ecKey = EthECKey.RecoverFromSignature(signature, recoveryId, payloadBytes);

                _logger.LogInformation($"Payload: {payload}");
                _logger.LogInformation($"Payload Bytes: {BitConverter.ToString(payloadBytes)}");
                _logger.LogInformation($"r: {rBigInt}");
                _logger.LogInformation($"s: {sBigInt}");
                _logger.LogInformation($"v: {v}, recoveryId: {recoveryId}");


                if (ecKey == null)
                    return false;

                // Get recovered public key and convert to Base58
                byte[] pubKeyBytes = ecKey.GetPubKey();
                BigInteger pubKeyRecovered = new BigInteger(pubKeyBytes.Reverse().Concat(new byte[] { 0 }).ToArray());
                string recoveredPublicKeyBase58 = PublicKeyAsBase58(pubKeyRecovered);

                // Compare with provided public key (case-insensitive)
                return string.Equals(publicKeyAsBase58, recoveredPublicKeyBase58, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid signature");
            }
        }

        private static string PublicKeyAsBase58Validate(BigInteger publicKey)
        {
            // Convert to byte array (little-endian by default in .NET)
            byte[] pubKeyBytes = publicKey.ToByteArray();

            // Remove padding zero byte if present (sign byte)
            if (pubKeyBytes[pubKeyBytes.Length - 1] == 0)
            {
                Array.Resize(ref pubKeyBytes, pubKeyBytes.Length - 1);
            }

            // Reverse to big-endian (Ethereum standard)
            Array.Reverse(pubKeyBytes);

            // Encode to Base58 using SimpleBase library
            return SimpleBase.Base58.Bitcoin.Encode(pubKeyBytes);
        }
    }
}