using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Helper
{
    public class Seguranca
    {
        public enum EncodingType
        {
            //ASCII,//Não aceita na biblioteca mobile.
            //UTF7,//Não aceita na biblioteca mobile.
            UTF8
            //UTF32//Não aceita na biblioteca mobile.
        }

        private char[] Password1 = new char[] { 'C', 'o', 's', 'f', 'g', 'i', 'e', '0', '9', 's', 'd', '9', '3', '2', 'c', 'i', 'v', 'C', 'L', 'S', 'O', 's', 'v', '4', '7' };
        private char[] Password2 = new char[] { 'J', 'V', '9', 'd', 'g', '#', 'v', 'n', '0', 'B', 'Q', 'l', 'b', '0', 'b', 'n', 's', 'o', 's', 'j', 'p', 'j', 'k', 'v', 'U' };
        private EncodingType Type = EncodingType.UTF8;//Default: UTF-8

        public Seguranca()
        {

        }

        public Seguranca(EncodingType encodingType)
        {
            SetEncodingType(encodingType);
        }

        public EncodingType GetEncodingType()
        {
            return Type;
        }

        public void SetEncodingType(EncodingType encodingType)
        {
            Type = encodingType;
        }

        private byte[] ToByte(char[] s)
        {
            return ToByte(new string(s), GetEncodingType());
        }

        private byte[] GenerateKey(int keySize)
        {
            if (keySize >= 0)
            {
                byte[] ret = new byte[keySize];
                byte[] asciiPwd = new byte[Password1.Length + Password2.Length];
                byte[] pwd1 = ToByte(Password1);
                byte[] pwd2 = ToByte(Password2);

                for (int i = 0; i < pwd1.Length + pwd2.Length; i++)
                {
                    if (i < pwd1.Length)
                    {
                        asciiPwd[i] = pwd1[i];
                    }
                    else
                    {
                        asciiPwd[i] = pwd2[i - pwd1.Length];
                    }
                }

                for (int i = 0; i < keySize; i++)
                {
                    for (int j = 0; j < asciiPwd.Length; j++)
                    {
                        for (int l = 0; l <= i; l++)
                        {
                            if (j % 2 == 0)
                            {
                                ret[i] = (byte)(asciiPwd[j] * asciiPwd[(j + l) % asciiPwd.Length] - asciiPwd[i % asciiPwd.Length] + ret[l]);
                            }
                            else if (j % 3 == 0)
                            {
                                ret[i] = (byte)(asciiPwd[j] - asciiPwd[(j + l) % asciiPwd.Length] + asciiPwd[i % asciiPwd.Length] - ret[l]);
                            }
                            else if (j % 4 == 0)
                            {
                                ret[i] = (byte)(asciiPwd[j] + asciiPwd[(j + l) % asciiPwd.Length] * asciiPwd[i % asciiPwd.Length] * ret[l]);
                            }
                        }
                    }
                }

                return ret;
            }
            else
            {
                throw new InvalidOperationException("It is not possible to generate a lower zero key size.");
            }
        }

        private byte[] Shift(byte[] value, int indexes)
        {
            if (value != null && indexes != 0)
            {
                byte[] ret = new byte[value.Length];

                for (int i = 0; i < Math.Abs(indexes); i++)
                {
                    byte currentStream = 0, nextStream = 0;

                    for (int j = 0; j < value.Length; j++)
                    {
                        if (indexes > 0)
                        {
                            if (j == value.Length - 1)
                            {
                                nextStream = value[0];
                                currentStream = value[value.Length - 1];
                                ret[0] = currentStream;
                            }
                            else
                            {
                                nextStream = value[j + 1];
                                currentStream = value[j];
                                ret[j + 1] = currentStream;
                            }
                        }
                        else
                        {
                            if (j == 0)
                            {
                                nextStream = value[value.Length - 1];
                                currentStream = value[0];
                                ret[value.Length - 1] = currentStream;
                            }
                            else
                            {
                                nextStream = value[j - 1];
                                currentStream = value[j];
                                ret[j - 1] = currentStream;
                            }
                        }
                        currentStream = nextStream;
                    }
                }

                return ret;
            }
            else if (value == null)
            {
                throw new InvalidOperationException("It is not possible to shift a null value.");
            }
            else
            {
                throw new InvalidOperationException("It is not possible to shift zero indexes.");
            }
        }

        private byte[] Xor(byte[] value, byte[] key)
        {
            if (value != null && key != null && value.Length == key.Length)
            {
                byte[] ret = new byte[value.Length];

                for (int i = 0; i < value.Length; i++)
                {
                    ret[i] = (byte)(value[i] ^ key[i]);
                }

                return ret;
            }
            else if (value == null)
            {
                throw new InvalidOperationException("It is not possible to shift a null value.");
            }
            else if (key == null)
            {
                throw new InvalidOperationException("It is not possible to shift a with null key.");
            }
            else
            {
                throw new InvalidOperationException("It is not possible to shift a value diferently sized from key.");
            }
        }

        public byte[] Encrypt(string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                return Encrypt(ToByte(value, GetEncodingType()));
            }
            else if (value == null)
            {
                return null;
            }
            else
            {
                return new byte[0];
            }
        }

        public byte[] Encrypt(byte[] value)
        {
            if (value != null && value.Length > 0)
            {
                byte[] ret = new byte[value.Length];
                byte[] key = GenerateKey(value.Length);

                ret = value;
                ret = Shift(ret, 1);
                ret = Xor(ret, key);
                ret = Shift(ret, 3);
                ret = Xor(ret, key);
                ret = Shift(ret, 2);
                ret = Xor(ret, key);

                return ret;
            }
            else
            {
                return value;
            }
        }

        public byte[] Decrypt(byte[] encryptedValue)
        {
            if (encryptedValue != null && encryptedValue.Length > 0)
            {
                byte[] ret = new byte[encryptedValue.Length];
                byte[] key = GenerateKey(encryptedValue.Length);

                ret = encryptedValue;
                ret = Xor(ret, key);
                ret = Shift(ret, -2);
                ret = Xor(ret, key);
                ret = Shift(ret, -3);
                ret = Xor(ret, key);
                ret = Shift(ret, -1);

                return ret;
            }
            else
            {
                return encryptedValue;
            }
        }

        public string DecryptToString(byte[] encryptedValue)
        {
            byte[] bytes = Decrypt(encryptedValue);
            if (bytes.Length > 0)
            {
                return ToString(bytes, GetEncodingType());
            }
            else
            {
                return string.Empty;
            }
        }

        public static byte[] ToByte(string s, EncodingType type)
        {
            if (s != null)
            {
                return System.Text.Encoding.UTF8.GetBytes(s);
            }
            else
            {
                return null;
            }
        }

        public static string ToString(byte[] value, EncodingType type)
        {
            if (value != null)
            {
                return System.Text.Encoding.UTF8.GetString(value, 0, value.Length);
            }
            else
            {
                return null;
            }
        }
    }
}
