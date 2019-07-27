using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using CcNet.Utils.Extensions;

namespace CcNet.Utils.Helpers
{
    /// <summary>
    /// AES工具类
    /// </summary>
    public class AesHelper
    {
        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <param name="error">错误信息</param>
        /// <param name="key">密钥</param>
        /// <param name="vector">向量值(传入该值时使用CBC模式加密,否则使用ECB模式加密)</param>
        /// <param name="encodingName">编码名称</param>
        /// <returns>密文</returns>
        public static string Encrypt(string plainText, out string error,
            string key = DEFAULT_KEY, byte[] vector = null, string encodingName = "UTF-8")
        {
            error = null;
            RijndaelManaged aes = null;

            try
            {
                var encoding = Encoding.GetEncoding(encodingName);
                var plainBytes = encoding.GetBytes(plainText);

                aes = new RijndaelManaged
                {
                    Padding = PaddingMode.PKCS7,
                    KeySize = 128,
                    Key = encoding.GetBytes(key)
                };

                if (vector.IsEmpty())
                {
                    aes.Mode = CipherMode.ECB;
                }
                else
                {
                    aes.Mode = CipherMode.CBC;
                    aes.IV = vector;
                }

                using (var mStream = new MemoryStream())
                using (var cryptoStream = new CryptoStream(mStream
                    , aes.CreateEncryptor()
                    , CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    return Convert.ToBase64String(mStream.ToArray());
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return string.Empty;
            }
            finally
            {
                aes?.Clear();
            }
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="encryptedText">密文</param> 
        /// <param name="error">错误信息</param>
        /// <param name="key">密钥</param>
        /// <param name="vector">向量值(传入该值时使用CBC模式解密,否则使用ECB模式解密)</param>
        /// <param name="encodingName">编码名称</param>
        /// <returns>明文</returns>
        public static string Decrypt(string encryptedText, out string error,
            string key = DEFAULT_KEY, byte[] vector = null, string encodingName = "UTF-8")
        {
            error = null;
            RijndaelManaged aes = null;

            try
            {
                var encoding = Encoding.GetEncoding(encodingName);
                var encryptedBytes = Convert.FromBase64String(encryptedText);

                aes = new RijndaelManaged
                {
                    Padding = PaddingMode.PKCS7,
                    KeySize = 128,
                    Key = encoding.GetBytes(key)
                };

                if (vector.IsEmpty())
                {
                    aes.Mode = CipherMode.ECB;
                }
                else
                {
                    aes.Mode = CipherMode.CBC;
                    aes.IV = vector;
                }

                using (var mStream = new MemoryStream(encryptedBytes))
                using (var cryptoStream = new CryptoStream(mStream
                    , aes.CreateDecryptor()
                    , CryptoStreamMode.Read))
                {
                    var tmp = new byte[encryptedBytes.Length + 32];
                    int len = cryptoStream.Read(tmp, 0, encryptedBytes.Length + 32);
                    var ret = new byte[len];
                    Array.Copy(tmp, 0, ret, 0, len);
                    return encoding.GetString(ret);
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return string.Empty;
            }
            finally
            {
                aes?.Clear();
            }
        }

        private const string DEFAULT_KEY = "GYUnz41w9UoQclmP";
    }
}
