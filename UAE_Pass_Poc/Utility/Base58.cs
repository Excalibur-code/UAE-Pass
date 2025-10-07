using System;
using System.Linq;
using System.Text;

namespace UAE_Pass_Poc.Utility
{
    public static class Base58
    {
        private static readonly char[] ALPHABET = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz".ToCharArray();
        private static readonly int BASE_58 = ALPHABET.Length;
        private static readonly int BASE_256 = 256;

        private static readonly int[] INDEXES = new int[128];

        static Base58()
        {
            Array.Fill(INDEXES, -1);
            for (int i = 0; i < ALPHABET.Length; i++)
            {
                if (ALPHABET[i] < 128) // Ensure character is within ASCII range for INDEXES
                {
                    INDEXES[ALPHABET[i]] = i;
                }
            }
        }

        public static string Encode(byte[] input)
        {
            if (input.Length == 0)
            {
                return "";
            }

            input = CopyOfRange(input, 0, input.Length);

            int zeroCount = 0;
            while (zeroCount < input.Length && input[zeroCount] == 0)
            {
                ++zeroCount;
            }

            byte[] temp = new byte[input.Length * 2];
            int j = temp.Length;

            int startAt = zeroCount;
            while (startAt < input.Length)
            {
                byte mod = DivMod58(input, startAt);
                if (input[startAt] == 0)
                {
                    ++startAt;
                }

                temp[--j] = (byte)ALPHABET[mod];
            }

            while (j < temp.Length && temp[j] == ALPHABET[0])
            {
                ++j;
            }

            while (--zeroCount >= 0)
            {
                temp[--j] = (byte)ALPHABET[0];
            }

            byte[] output = CopyOfRange(temp, j, temp.Length);
            return Encoding.UTF8.GetString(output);
        }

        public static byte[] Decode(string input)
        {
            if (input.Length == 0)
            {
                return new byte[0];
            }

            byte[] input58 = new byte[input.Length];
            for (int i = 0; i < input.Length; ++i)
            {
                char c = input[i];
                int digit58 = -1;
                if (c >= 0 && c < 128)
                {
                    digit58 = INDEXES[c];
                }
                if (digit58 < 0)
                {
                    throw new ArgumentException("Not a Base58 input: " + input);
                }
                input58[i] = (byte)digit58;
            }

            int zeroCount = 0;
            while (zeroCount < input58.Length && input58[zeroCount] == 0)
            {
                ++zeroCount;
            }

            byte[] temp = new byte[input.Length];
            int j = temp.Length;

            int startAt = zeroCount;
            while (startAt < input58.Length)
            {
                byte mod = DivMod256(input58, startAt);
                if (input58[startAt] == 0)
                {
                    ++startAt;
                }
                temp[--j] = mod;
            }

            while (j < temp.Length && temp[j] == 0)
            {
                ++j;
            }

            int length = temp.Length - (j - zeroCount);
            byte[] result = new byte[length];
            Array.Copy(temp, j - zeroCount, result, 0, length);
            return result;
        }

        private static byte DivMod58(byte[] number, int startAt)
        {
            int remainder = 0;
            for (int i = startAt; i < number.Length; i++)
            {
                int digit256 = (int)number[i] & 0xFF;
                int temp = remainder * BASE_256 + digit256;

                number[i] = (byte)(temp / BASE_58);

                remainder = temp % BASE_58;
            }

            return (byte)remainder;
        }

        private static byte DivMod256(byte[] number58, int startAt)
        {
            int remainder = 0;
            for (int i = startAt; i < number58.Length; i++)
            {
                int digit58 = (int)number58[i] & 0xFF;
                int temp = remainder * BASE_58 + digit58;

                number58[i] = (byte)(temp / BASE_256);

                remainder = temp % BASE_256;
            }

            return (byte)remainder;
        }

        private static byte[] CopyOfRange(byte[] source, int from, int to)
        {
            byte[] range = new byte[to - from];
            Buffer.BlockCopy(source, from, range, 0, range.Length);
            return range;
        }
    }
}