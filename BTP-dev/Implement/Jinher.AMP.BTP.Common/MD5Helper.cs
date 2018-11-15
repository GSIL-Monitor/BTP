using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Jinher.AMP.BTP.Common
{
    /// <summary>
    /// MD5帮助类
    /// </summary>
    public class MD5Helper
    {
        public static string GetMD5(byte[] buffer)
        {
            StringBuilder sb = new StringBuilder();
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = md5.ComputeHash(buffer);
            foreach (byte b in data)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        public static string GetMD5(Stream stream)
        {
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            return GetMD5(buffer);
        }

        public static string GetMD5(string text, Encoding code)
        {
            byte[] buffer = code.GetBytes(text);
            return GetMD5(buffer);
        }
    }
}
