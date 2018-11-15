using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.Common
{
    public class BTPLogHelper
    {
        private static readonly string HostName = IpUtils.GetLocalHostName();
        private static readonly string HostIp = IpUtils.GetLocalIpAddress();


        /// <summary>
        /// DEBUG信息
        /// </summary>
        /// <param name="message">信息内容</param>
        public static void Debug(string message)
        {
            Debug(message, false);
        }
        /// <summary>
        /// DEBUG信息
        /// </summary>
        /// <param name="message">信息内容</param>
        /// <param name="isEmail">是否发送Email通知，默认false</param>
        public static void Debug(string message, bool isEmail)
        {
            LogHelper.Debug(message);
            if (isEmail)
                SendEmail("Debug", message);
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="message">信息内容</param>
        public static void Error(string message)
        {
            Error(message, true);
        }
        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="message">信息内容</param>
        /// <param name="isEmail">是否发送Email通知，默认true</param>
        public static void Error(string message, bool isEmail)
        {
            LogHelper.Error(message);
            if (isEmail)
                SendEmail("Error", message);
        }

        /// <summary>
        /// 提示信息
        /// </summary>
        /// <param name="message">信息内容</param>
        public static void Info(string message)
        {
            Info(message, false);
        }
        /// <summary>
        /// 提示信息
        /// </summary>
        /// <param name="message">信息内容</param>
        /// <param name="isEmail">是否发送Email通知，默认false</param>
        public static void Info(string message, bool isEmail)
        {
            LogHelper.Info(message);
            if (isEmail)
                SendEmail("Info", message);
        }

        /// <summary>
        /// 警告信息
        /// </summary>
        /// <param name="message">信息内容</param>
        public static void Warn(string message)
        {
            Warn(message, false);
        }

        /// <summary>
        /// 警告信息
        /// </summary>
        /// <param name="message">信息内容</param>
        /// <param name="isEmail">是否发送Email通知，默认false</param>
        public static void Warn(string message, bool isEmail)
        {
            LogHelper.Warn(message);
            if (isEmail)
                SendEmail("Warn", message);
        }
        private static void SendEmail(string title, string message)
        {
            EmailUtil.getInstance().SendMail(false, title, CustomConfig.Environment + ":" + title + "(" + HostName + "," + HostIp + ")", message, CustomConfig.EmailSendModel.ToEmail, CustomConfig.EmailSendModel.ToCC);
        }
    }

    public class IpUtils
    {
        public static string GetLocalIpAddress()
        {
            System.Net.IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());


            for (int i = 0; i != ipEntry.AddressList.Length; i++)
            {
                if (!ipEntry.AddressList[i].IsIPv6LinkLocal && !ipEntry.AddressList[i].IsIPv6Multicast
                    && !ipEntry.AddressList[i].IsIPv6SiteLocal && !ipEntry.AddressList[i].IsIPv6Teredo)
                {
                    return ipEntry.AddressList[i].ToString();

                }
            }
            return string.Empty;
        }
        /// <summary>
        /// 主机名
        /// </summary>
        /// <returns></returns>
        public static string GetLocalHostName()
        {
            return System.Net.Dns.GetHostName();
        }

    }

}
