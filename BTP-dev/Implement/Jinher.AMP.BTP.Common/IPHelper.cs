using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Web;
using Jinher.JAP.Common.Loging;
using Newtonsoft.Json.Linq;

namespace Jinher.AMP.BTP.Common
{
    public static class IPHelper
    {
        public static string GetRegion(string ip)
        {
            if (string.IsNullOrEmpty(ip))
            {
                return string.Empty;
            }
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://ip.taobao.com//service/getIpInfo.php?ip=" + ip);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (Stream myResponseStream = response.GetResponseStream())
                using (StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8")))
                {
                    string retString = myStreamReader.ReadToEnd();
                    JObject json = JObject.Parse(retString);
                    if (json["data"].HasValues)
                    {
                        if (json["data"]["region_id"].HasValues)
                        {
                            return json["data"]["region_id"].ToString();
                        }
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                LogHelper.Error("IPHelper.GetRegion Exception, Ip=" + ip, ex);
                return null;
            }
        }

        /// <summary>
        /// 检查IP地址格式
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(string ip)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        /// <summary>
        /// 获取客户端IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetClientIp()
        {
            try
            {
                OperationContext context = OperationContext.Current;
                if (context == null) return string.Empty;
                MessageProperties properties = context.IncomingMessageProperties;
                RemoteEndpointMessageProperty endpoint = properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                if (endpoint != null)
                {
                    LogHelper.Debug("IPHelper.GetClientIp:", endpoint.Address);
                    return endpoint.Address;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("IPHelper.GetClientIp异常", ex);
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取客户端IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetWebIP()
        {
            //如果客户端使用了代理服务器，则利用HTTP_X_FORWARDED_FOR找到客户端IP地址
            string httpXAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            string userHostAddress = string.Empty;
            if (!string.IsNullOrEmpty(httpXAddress))
            {
                userHostAddress = httpXAddress.Split(',')[0].Trim();
                LogHelper.Debug(string.Format("HTTP_X_FORWARDED_FOR:{0}", userHostAddress));
            }
            //利用Request.Headers属性获取IP地址
            if (string.IsNullOrEmpty(userHostAddress))
            {
                userHostAddress = HttpContext.Current.Request.Headers["X_FORWARDED_FOR"];
                LogHelper.Debug(string.Format("X_FORWARDED_FOR:{0}", userHostAddress));
            }
            //读取REMOTE_ADDR获取客户端IP地址
            if (string.IsNullOrEmpty(userHostAddress))
            {
                userHostAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                LogHelper.Debug(string.Format("REMOTE_ADDR:{0}", userHostAddress));
            }
            //利用Request.UserHostAddress属性获取IP地址
            if (string.IsNullOrEmpty(userHostAddress))
            {
                userHostAddress = HttpContext.Current.Request.UserHostAddress;
                LogHelper.Debug(string.Format("UserHostAddress:{0}", userHostAddress));
            }
            //并检查IP地址的格式
            if (!string.IsNullOrEmpty(userHostAddress) && IsIP(userHostAddress))
            {
                return userHostAddress;
            }
            return string.Empty;
        }

        /// <summary>
        /// 校验ip地址
        /// </summary>
        /// <param name="ipList"></param>
        /// <returns></returns>
        public static bool CheckCurrentIP(List<string> ipList)
        {
            string currentIp = GetClientIp();
            if (string.IsNullOrEmpty(currentIp))
            {
                currentIp = GetWebIP();
            }
            return !string.IsNullOrEmpty(currentIp) && ipList.Contains(currentIp);
        }

        /// <summary>
        /// 根据域名获取外网ip
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static string GetIPByDomain(string domain)
        {
            LogHelper.Debug("IPHelper.GetIPByDomain,domain=" + domain);
            try
            {
                var ipArr = Dns.GetHostAddresses(domain);
                if (ipArr == null || ipArr.Length == 0 || ipArr[0] == null) return string.Empty;
                foreach (var ipAddress in ipArr)
                {
                    LogHelper.Debug("IPHelper.GetIPByDomain,ip=" + ipAddress);
                }
                return ipArr[0].ToString();
            }
            catch (Exception ex)
            {
                LogHelper.Error("IPHelper.GetIPByDomain异常", ex);
            }
            return string.Empty;
        }
    }
}
