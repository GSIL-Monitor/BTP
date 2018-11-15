using System.Web;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Net;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Collections;
using System.Text.RegularExpressions;
using Jinher.AMP.BTP.Common;

namespace Jinher.AMP.BTP.UI.Util
{
    /// <summary>
    /// 类名：WeixinFunction
    /// 功能：接口公用函数类
    /// 详细：该类是请求、通知返回两个文件所调用的公用函数核心处理文件，不需要修改
    /// 版本：1.0
    /// 日期：2015-05-05
    /// </summary>
    public class WeixinFunction
    {
        /// <summary>
        /// 签名(进行升序排列)
        /// </summary>
        /// <param name="dicArrayPre">要签名的数组</param>
        /// <param name="key">安全校验码(key=value)</param>
        /// <param name="sign_type">签名类型</param>
        /// <param name="_input_charset">编码格式</param>
        /// <returns>签名结果字符串</returns>
        public static string BuildMysign(SortedDictionary<string, string> dicArrayPre, string keyValue, string sign_type, string _input_charset)
        {
            Dictionary<string, string> dicArray = FilterPara(dicArrayPre);
            string prestr = CreateLinkString(dicArray);
            prestr = prestr +"&"+ keyValue;
            string mysign = Sign(prestr, sign_type, _input_charset);
            return mysign;
        }

        /// <summary>
        /// 签名(不进行升序排列)
        /// </summary>
        /// <param name="dicArrayPre">要签名的数组</param>
        /// <param name="key">安全校验码(key=value)</param>
        /// <param name="sign_type">签名类型</param>
        /// <param name="_input_charset">编码格式</param>
        /// <returns>签名结果字符串</returns>
        public static string BuildMysign(Dictionary<string, string> dicArrayPre, string keyValue, string sign_type, string _input_charset)
        {
            string prestr = CreateLinkString(dicArrayPre);
            prestr = prestr + "&" + keyValue;
            string mysign = Sign(prestr, sign_type, _input_charset);
            return mysign;
        }

        /// <summary>
        /// 除去数组中的空值和签名参数并以字母a到z的顺序排序
        /// </summary>
        /// <param name="dicArrayPre">过滤前的参数组</param>
        /// <returns>过滤后的参数组</returns>
        public static Dictionary<string, string> FilterPara(SortedDictionary<string, string> dicArrayPre)
        {
            Dictionary<string, string> dicArray = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> temp in dicArrayPre)
            {
                if (temp.Key.ToLower() != "sign" && temp.Key.ToLower() != "key" && temp.Value != "" && temp.Value != null)
                {
                    dicArray.Add(temp.Key.ToLower(), temp.Value);
                }
            }

            return dicArray;
        }

        /// <summary>
        /// 把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串
        /// </summary>
        /// <param name="sArray">需要拼接的数组</param>
        /// <returns>拼接完成以后的字符串</returns>
        public static string CreateLinkString(Dictionary<string, string> dicArray)
        {
            StringBuilder prestr = new StringBuilder();
            foreach (KeyValuePair<string, string> temp in dicArray)
            {
                prestr.Append(temp.Key + "=" + temp.Value + "&");
            }

            //去掉最後一個&字符
            int nLen = prestr.Length;
            prestr.Remove(nLen - 1, 1);

            return prestr.ToString();
        }

        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="dicArrayPre">要签名的数组</param>
        /// <returns>签名结果字符串</returns>
        public static string BuildMysign(SortedDictionary<string, object> dicArrayPre)
        {
            string prestr = ToUrl(dicArrayPre);
            //prestr += "&key=" + CustomConfig.WxApiKey;
            string mysign = Sign(prestr,"MD5","utf-8");
            return mysign;
        }

        public static string ToUrl(SortedDictionary<string, object> m_values)
        {
            string buff = "";
            foreach (KeyValuePair<string, object> pair in m_values)
            {
                if (pair.Value == null)
                {
                    Jinher.JAP.Common.Loging.LogHelper.Error(string.Format(@"微信签名ToUrl方法，值为null，key：{0})", pair.Key));
                }

                if ((!string.IsNullOrWhiteSpace(pair.Key)) 
                    && pair.Key != "sign" && pair.Value.ToString() != "")
                {
                    buff += pair.Key.ToLower() + "=" + pair.Value + "&";
                }
            }
            buff = buff.Trim('&');
            return buff;
        }


        /// <summary>
        /// 签名（具体实现方法）
        /// </summary>
        /// <param name="prestr">过滤后升序的字符串</param>
        /// <param name="sign_type">签名类型</param>
        /// <param name="_input_charset">编码格式</param>
        /// <returns>签名结果</returns>
        public static string Sign(string prestr, string sign_type, string _input_charset)
        {
            Jinher.JAP.Common.Loging.LogHelper.Error(string.Format(@"微信签名的字符串prestr:{0})",prestr));

            byte[] StrRes = Encoding.Default.GetBytes(prestr);
            HashAlgorithm iSHA = new SHA1CryptoServiceProvider();
            StrRes = iSHA.ComputeHash(StrRes);
            StringBuilder enText = new StringBuilder();
            foreach (byte iByte in StrRes)
            {
                enText.AppendFormat("{0:x2}", iByte);
            }
            Jinher.JAP.Common.Loging.LogHelper.Error(string.Format(@"微信签名:{0}", enText.ToString()));
            return enText.ToString();
        }

        /// <summary>
        /// 返回 XML字符串 节点value
        /// </summary>
        /// <param name="xmlDoc">XML格式 数据</param>
        /// <param name="xmlNode">节点</param>
        /// <returns>节点value</returns>
        public static string GetStrForXmlDoc(string xmlDoc, string xmlNode)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlDoc);
            XmlNode xn = xml.SelectSingleNode(xmlNode);
            return xn.InnerText;
        }

        /// <summary>
        /// 获取预支付 XML 参数组合
        /// </summary>
        /// <returns></returns>
        public static string ParseXML(SortedDictionary<string, string> parameters)
        {
            var sb = new StringBuilder();
            sb.Append("<xml>");
            var akeys = new ArrayList(parameters.Keys);
            foreach (string k in akeys)
            {
                var v = (string)parameters[k];
                if (Regex.IsMatch(v, @"^[0-9.]$"))
                {
                    sb.Append("<" + k + ">" + v + "</" + k + ">");
                }
                else
                {
                    sb.Append("<" + k + "><![CDATA[" + v + "]]></" + k + ">");
                }
            }
            sb.Append("</xml>");
            return sb.ToString();
        }

        /// <summary>
        /// 随机字符串
        /// </summary>
        /// <returns></returns>
        public static string GetNoncestr()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        /// <summary>
        /// 时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimestamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        /// <summary>
        /// 取时间戳生成随即数,替换交易单号中的后10位流水号
        /// </summary>
        /// <returns></returns>
        public static UInt32 UnixStamp()
        {
            TimeSpan ts = DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return Convert.ToUInt32(ts.TotalSeconds);
        }

        /// <summary>
        /// 取随机数
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string BuildRandomStr(int length)
        {
            Random rand = new Random();

            int num = rand.Next();

            string str = num.ToString();

            if (str.Length > length)
            {
                str = str.Substring(0, length);
            }
            else if (str.Length < length)
            {
                int n = length - str.Length;
                while (n > 0)
                {
                    str.Insert(0, "0");
                    n--;
                }
            }
            return str;
        }

        /// <summary>
        /// 判断是否在微信内置浏览器中
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static bool SideInWeixinBroswer(HttpContextBase httpContext)
        {
            var userAgent = httpContext.Request.UserAgent;
            if (string.IsNullOrEmpty(userAgent) || (!userAgent.Contains("MicroMessenger") && !userAgent.Contains("Windows Phone")))
            {
                //在微信外部
                return false;
            }
            //在微信内部
            return true;
        }

    }
}