using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Jinher.AMP.BTP.Common
{
    /// <summary>
    /// web请求帮助类
    /// </summary>
    public class WebRequestHelper
    {
        /// <summary>
        /// 构造HTTP的POST请求
        /// </summary>
        /// <param name="url">请求的url</param>
        /// <param name="strData">请求参数，和get请求？后用&分隔的内容一样</param>
        /// <returns></returns>
        public static string SendPostInfo(string url, string strData)
        {
            return SendWebRequest(url, strData, "POST", "application/x-www-form-urlencoded");
        }

        /// <summary>
        ///  构造HTTP的GET请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string SendGetRequest(string url)
        {
            return SendWebRequest(url, null, "GET", "application/x-www-form-urlencoded");
        }
        /// <summary>
        /// 发送WebRequest请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="strData"></param>
        /// <param name="reqMethod"></param>
        /// <param name="reqContentType"></param>
        /// <returns></returns>
        public static string SendWebRequest(string url, string strData, string reqMethod, string reqContentType)
        {
            HttpWebRequest request = null;
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = (HttpWebRequest)WebRequest.Create(url);
            }
            request.Method = reqMethod;
            request.ContentType = reqContentType;

            Stream newStream = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(strData))
                {
                    UTF8Encoding encoding = new UTF8Encoding();
                    byte[] data = encoding.GetBytes(strData);

                    request.ContentLength = data.Length;
                    // 发送数据
                    newStream = request.GetRequestStream();
                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                }
            }
            catch (Exception ex)
            {
                JAP.Common.Loging.LogHelper.Error("WebRequestHelper.SendWebRequest.newStream", ex);
            }
            finally
            {
                if (newStream != null)
                {
                    newStream.Close();
                }
            }

            // 获得返回数据
            string strResponse = string.Empty;
            WebResponse webResponse = null;
            StreamReader responseStream = null;
            try
            {
                webResponse = request.GetResponse();
                responseStream = new StreamReader(webResponse.GetResponseStream(), System.Text.Encoding.UTF8);
                strResponse = responseStream.ReadToEnd();
                webResponse.Close();
                responseStream.Close();
            }
            catch (Exception ex)
            {
                JAP.Common.Loging.LogHelper.Error("WebRequestHelper.SendWebRequest.GetResponse", ex);
                strResponse = ex.ToString();
            }
            finally
            {
                if (webResponse != null)
                {
                    webResponse.Close();
                }
                if (responseStream != null)
                {
                    responseStream.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return strResponse;
        }
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受  
        }
    }
}
