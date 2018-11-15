using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Security.Cryptography;
using System.Reflection;
using System.IO;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.Common
{
    /// <summary>
    /// 签名
    /// </summary>
    public class SignHelper
    {
        #region 公开方法

        /// <summary>
        /// 对url进行签名。
        /// </summary>
        /// <param name="fullUrl">完整get请求url.</param>
        /// <returns>签名以后的完整get请求url</returns>
        public static string SignUrl(string fullUrl)
        {
            string validData = "";
            int indw = fullUrl.IndexOf("?");
            //没带参数，直接返回。
            if (indw == fullUrl.Length - 1)
            {
                return fullUrl;
            }
            if (indw > -1)
            {
                //可能有多个?号，不用split.
                validData = fullUrl.Substring(indw + 1);
            }
            if (string.IsNullOrWhiteSpace(validData))
            {
                return fullUrl;
            }
            string baseUrl = fullUrl.Substring(0, indw);
            fullUrl = baseUrl + "?" + SignParams(validData);
            return fullUrl;
        }

        /// <summary>
        /// 对url参数串形式的参数进行签名。
        /// </summary>
        /// <param name="strParams"></param>
        /// <returns></returns>
        public static string SignParams(string strParams)
        {
            var dict = ConvertUrlParmToDictionary(strParams);
            strParams = SignDictionary(dict);
            return strParams;
        }
        /// <summary>
        /// 对参数字典进行签名
        /// </summary>
        /// <param name="dicArray">参数字典</param>
        /// <returns>url参数串.</returns>
        public static string SignDictionary(SortedDictionary<string, string> dicArray)
        {
            // modify by cxf 2015-2-4
            string partnerPrivKey = CustomConfig.PartnerPrivKey;
            //string partnerPrivKey = System.Configuration.ConfigurationManager.AppSettings["PartnerPrivKey"];

            string link = ConvertDictionaryToUrlParam(dicArray);
            //生成签名。
            string sign = GetSign(link, partnerPrivKey);
            string fullUrl = link + "&sign=" + sign;
            return fullUrl;
        }

        /// <summary>
        /// 验证请求参数的签名
        /// </summary>
        /// <returns></returns>
        public static bool ValidSignRequest(HttpRequestBase request)
        {
            bool isValid = false;
            string httpMethod = request.HttpMethod.ToUpper();
            if (httpMethod == "POST")
            {
                Stream stream = request.InputStream;
                byte[] byts = new byte[stream.Length];
                stream.Read(byts, 0, byts.Length);
                //和发送请求时使用的字符编码方式保持一致
                string reqParamString = request.ContentEncoding.GetString(byts);
                isValid = VerifySign(reqParamString);
            }
            else if (httpMethod == "GET")
            {
                string rawUrl = request.RawUrl;
                isValid = ValidSignUrl(rawUrl);
            }

            return isValid;
        }

        /// <summary>
        /// 验证参数
        /// </summary>
        /// <param name="fullUrl"></param>
        /// <returns></returns>
        public static bool ValidSignUrl(string fullUrl)
        {
            string validData = "";
            int indw = fullUrl.IndexOf("?");
            //没带参数，直接返回。
            if (indw == fullUrl.Length - 1)
            {
                return false;
            }
            if (indw > -1)
            {
                validData = fullUrl.Substring(indw + 1);
            }
            if (string.IsNullOrWhiteSpace(validData))
            {
                return false;
            }
            bool isValid = VerifySign(validData);
            return isValid;
        }


        /// <summary>
        /// 需要验证的字符串。
        /// </summary>
        /// <param name="validData">要验证的完整的字符串</param>
        /// <returns></returns>
        public static bool VerifySign(string validData)
        {
            string sign = GetSignParamValue(validData);
            //验证参数合法性。
            bool isValid = VerifySign(validData, sign);
            return isValid;
        }

        /// <summary>
        /// 需要验证的字符串。
        /// </summary>
        /// <param name="validData">要验证的完整的字符串</param>
        /// <returns></returns>
        public static bool VerifySign(string validData, string sign)
        {
            //为参数解码;sign一直是数字和字母的组合，不需要解码。
            validData = HttpUtility.UrlDecode(validData);
            // modify by cxf 2015-2-4
            string partnerPrivKey = CustomConfig.PartnerPrivKey;
            //string partnerPrivKey = System.Configuration.ConfigurationManager.AppSettings["PartnerPrivKey"];
            //为url的参数串部分进行排序，生成新的参数串。
            string newLink = GetNewLinkString(validData);
            //验证参数合法性。
            bool isValid = VerifySign(newLink, partnerPrivKey, sign);
            return isValid;
        }

        #endregion



        #region 私有方法。


        /// <summary>
        /// 获取url中的sign参数。
        /// </summary>
        /// <param name="strUrlParm">整个参数串</param>
        /// <returns></returns>
        private static string GetSignParamValue(string strUrlParm)
        {
            return GetParamValue(strUrlParm, "sign");
        }

        /// <summary>
        /// 获取url中的参数。
        /// </summary>
        /// <returns></returns>
        private static string GetParamValue(string strUrlParm, string signParamName)
        {
            if (string.IsNullOrWhiteSpace(strUrlParm))
            {
                throw new Exception("参数串不能为空!");
            }
            if (string.IsNullOrWhiteSpace(signParamName))
            {
                throw new Exception("签名参数参数名不能为空!");
            }
            string sign = "";
            //对参数进行排序处理。
            char[] spAnd = "&".ToCharArray();
            char[] spEqu = "=".ToCharArray();
            SortedDictionary<string, string> sortDict = new SortedDictionary<string, string>();
            string[] reqParams = strUrlParm.Split(spAnd, StringSplitOptions.RemoveEmptyEntries);
            foreach (string p in reqParams)
            {
                string[] kv = p.Split(spEqu, StringSplitOptions.RemoveEmptyEntries);
                if (kv[0].ToLower() == signParamName.ToLower() && kv.Length == 2)
                {
                    sign = kv[1];
                    break;
                }
            }

            return sign;
        }

        /// <summary>
        /// 将参数排序后组成新的参数串。
        /// </summary>
        /// <param name="strUrlParm">url参数串</param>
        /// <returns>按参数名排序以后新的url参数串</returns>
        private static string GetNewLinkString(string strUrlParm)
        {
            SortedDictionary<string, string> sortDict = ConvertUrlParmToDictionary(strUrlParm);
            sortDict.Remove("sign");
            //重新生成url参数。
            string linkUrl = ConvertDictionaryToUrlParam(sortDict);
            return linkUrl;
        }

        /// <summary>
        /// 将参数串转为集合。
        /// </summary>
        /// <param name="strUrlParm">参数串</param>
        /// <returns>参数集合</returns>
        private static SortedDictionary<string, string> ConvertUrlParmToDictionary(string strUrlParm)
        {
            if (string.IsNullOrWhiteSpace(strUrlParm))
            {
                throw new Exception("参数串不能为空!");
            }
            //对参数进行排序处理。
            char[] spAnd = "&".ToCharArray();
            char[] spEqu = "=".ToCharArray();
            SortedDictionary<string, string> sortDict = new SortedDictionary<string, string>();
            string[] reqParams = strUrlParm.Split(spAnd, StringSplitOptions.RemoveEmptyEntries);
            foreach (string p in reqParams)
            {
                string[] kv = p.Split(spEqu, StringSplitOptions.RemoveEmptyEntries);
                string value = kv.Length == 2 ? kv[1] : "";
                sortDict.Add(kv[0], value);
            }
            return sortDict;
        }

        /// <summary>
        /// 把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串
        /// </summary>
        /// <param name="sArray">需要拼接的数组</param>
        /// <returns>拼接完成以后的字符串</returns>
        public static string ConvertDictionaryToUrlParam(SortedDictionary<string, string> dicArray)
        {
            if (dicArray == null || dicArray.Count == 0)
            {
                return "";
            }
            StringBuilder prestr = new StringBuilder();
            foreach (KeyValuePair<string, string> temp in dicArray)
            {
                prestr.AppendFormat("{0}={1}&", temp.Key, temp.Value);
            }

            //去掉最后一个&字符
            int nLen = prestr.Length;
            prestr.Remove(prestr.Length - 1, 1);

            return prestr.ToString();
        }

        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="oriData">原始数据</param>
        /// <param name="sign">签名数据</param>
        /// <returns>是否通过验证</returns>
        private static bool VerifySign(string oriData, string priKey, string sign)
        {
            string signStr = GetSign(oriData, priKey);
            bool isValid = signStr == sign ? true : false;
            return isValid;
        }

        /// <summary>
        /// 签名（具体实现方法）
        /// </summary>
        /// <param name="data">过滤后升序的字符串</param>
        /// <param name="partnerPrivKey">私钥</param>
        /// <returns>签名结果</returns>
        public static string GetSign(string data, string partnerPrivKey)
        {
            data += partnerPrivKey;
            return GetSign(data);
        }

        /// <summary>
        /// 为字符串data生成签名。
        /// </summary>
        /// <param name="data">要生成签名的源数据</param>
        /// <returns>签名</returns>
        public static string GetSign(string data)
        {
            //Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("GetSign data:{0}", data));
            StringBuilder sb = new StringBuilder(32);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.UTF8.GetBytes(data));
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }
            //Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("GetSign sign:{0}", sb.ToString()));
            return sb.ToString();
        }

        #endregion

    }

    /// <summary>
    /// 标记当前属性需要验证签名。
    /// </summary>
    public class SignAttribute : Attribute
    {

    }
}
