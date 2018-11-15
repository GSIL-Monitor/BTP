using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.TPS
{
    /**
 *
 * 快递鸟物流轨迹即时查询接口
 *
 * @author: CQ
 * @qq: 1069712970
 * @see: http://www.kdniao.com/YundanChaxunAPI.aspx
 * @copyright: 深圳市快金数据技术服务有限公司
 *
 * DEMO中的电商ID与私钥仅限测试使用，正式环境请单独注册账号
 * 单日超过500单查询量，建议接入我方物流轨迹订阅推送接口
 * 
 * ID和Key请到官网申请：http://www.kdniao.com/ServiceApply.aspx
 * 测试ID和KEY已经关闭
 * ID:1237100
 * KEY:518a73d8-1f7f-441a-b644-33e77b49d846
 * KdApiSubscribeKit 原名 KdApiSubscribeDemo
 */
    //
    [BTPAopLog]
    public class KdApiSubscribeKit : ContextBoundObject
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static KdApiSubscribeKit Instance = new KdApiSubscribeKit();
        private KdApiSubscribeKit()
        {

        }

        /// <summary>
        /// Json方式  物流信息订阅
        /// </summary>
        /// <returns></returns>
        [BTPAopLogMethod]
        public KdSubscribeResultDTO orderTracesSubByJson(List<KdSubscribeDTO> kdSubList)
        {
            //string requestData = "{'Code': 'YTO','Item': [" +
            //                   "{'No': '710155965039','Bk': 'test'}" +
            //                   "]}";
            string requestData = JsonHelper.JsonSerializer(kdSubList);
            LogHelper.Debug(string.Format("KdApiSubscribeKit.orderTracesSubByJson.requestData:{0}", requestData));


            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("RequestData", HttpUtility.UrlEncode(requestData, Encoding.UTF8));
            param.Add("EBusinessID", BTP.Common.CustomConfig.KdniaoEBusinessID);
            param.Add("RequestType", "1005");
            string dataSign = encrypt(requestData, BTP.Common.CustomConfig.KdniaoAppKey, "UTF-8");
            param.Add("DataSign", HttpUtility.UrlEncode(dataSign, Encoding.UTF8));
            param.Add("DataType", "2");

            string result = sendPost(BTP.Common.CustomConfig.KdniaoSubscribeReqURL, param);
            LogHelper.Debug(string.Format("KdApiSubscribeKit.orderTracesSubByJson结果：{0}", result));
            //根据公司业务处理返回的信息......
            KdSubscribeResultDTO resultDto = JsonHelper.JsonDeserialize<KdSubscribeResultDTO>(result);
            return resultDto;
        }

        /// <summary>
        ///  XML方式  物流信息订阅
        /// </summary>
        /// <returns></returns>
        [BTPAopLogMethod]
        public string orderTracesSubByXml()
        {
            string requestData = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" +
                                "<Content>" +
                                "<Code>SF</Code>" +
                                "<Items>" +
                                "<Item>" +
                                "<No>909261024507</No>" +
                                "<Bk>test</Bk>" +
                                "</Item>" +
                                "<Item>" +
                                "<No>909261024507</No>" +
                                "<Bk>test</Bk>" +
                                "</Item>" +
                                "</Items>" +
                                "</Content>";

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("RequestData", HttpUtility.UrlEncode(requestData, Encoding.UTF8));
            param.Add("EBusinessID", BTP.Common.CustomConfig.KdniaoEBusinessID);
            param.Add("RequestType", "1005");
            string dataSign = encrypt(requestData, BTP.Common.CustomConfig.KdniaoAppKey, "UTF-8");
            param.Add("DataSign", HttpUtility.UrlEncode(dataSign, Encoding.UTF8));
            param.Add("DataType", "1");

            string result = sendPost(BTP.Common.CustomConfig.KdniaoSubscribeReqURL, param);

            //根据公司业务处理返回的信息......

            return result;
        }

        /// <summary>
        /// Post方式提交数据，返回网页的源代码
        /// </summary>
        /// <param name="url">发送请求的 URL</param>
        /// <param name="param">请求的参数集合</param>
        /// <returns>远程资源的响应结果</returns>
        private string sendPost(string url, Dictionary<string, string> param)
        {
            string result = "";
            StringBuilder postData = new StringBuilder();
            if (param != null && param.Count > 0)
            {
                foreach (var p in param)
                {
                    if (postData.Length > 0)
                    {
                        postData.Append("&");
                    }
                    postData.Append(p.Key);
                    postData.Append("=");
                    postData.Append(p.Value);
                }
            }
            LogHelper.Debug(string.Format("KdApiSubscribeKit.orderTracesSubByJson params:{0},KdniaoSubscribeReqURL:{1}", postData.ToString(), url));
            byte[] byteData = Encoding.GetEncoding("UTF-8").GetBytes(postData.ToString());
            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/x-www-form-urlencoded";
                request.Referer = url;
                request.Accept = "*/*";
                request.Timeout = 30 * 1000;
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729)";
                request.Method = "POST";
                request.ContentLength = byteData.Length;
                Stream stream = request.GetRequestStream();
                stream.Write(byteData, 0, byteData.Length);
                stream.Flush();
                stream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream backStream = response.GetResponseStream();
                StreamReader sr = new StreamReader(backStream, Encoding.GetEncoding("UTF-8"));
                result = sr.ReadToEnd();
                sr.Close();
                backStream.Close();
                response.Close();
                request.Abort();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("KdApiSubscribeKit.sendPost异常，异常信息：{0}", ex));
                result = ex.Message;
            }
            return result;
        }

        ///<summary>
        ///电商Sign签名
        ///</summary>
        ///<param name="content">内容</param>
        ///<param name="keyValue">Appkey</param>
        ///<param name="charset">URL编码 </param>
        ///<returns>DataSign签名</returns>
        private string encrypt(String content, String keyValue, String charset)
        {
            if (keyValue != null)
            {
                return base64(MD5(content + keyValue, charset), charset);
            }
            return base64(MD5(content, charset), charset);
        }

        ///<summary>
        /// 字符串MD5加密
        ///</summary>
        ///<param name="str">要加密的字符串</param>
        ///<param name="charset">编码方式</param>
        ///<returns>密文</returns>
        private string MD5(string str, string charset)
        {
            byte[] buffer = System.Text.Encoding.GetEncoding(charset).GetBytes(str);
            try
            {
                System.Security.Cryptography.MD5CryptoServiceProvider check;
                check = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] somme = check.ComputeHash(buffer);
                string ret = "";
                foreach (byte a in somme)
                {
                    if (a < 16)
                        ret += "0" + a.ToString("X");
                    else
                        ret += a.ToString("X");
                }
                return ret.ToLower();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// base64编码
        /// </summary>
        /// <param name="str">内容</param>
        /// <param name="charset">编码方式</param>
        /// <returns></returns>
        private string base64(String str, String charset)
        {
            return Convert.ToBase64String(System.Text.Encoding.GetEncoding(charset).GetBytes(str));
        }
    }
}
