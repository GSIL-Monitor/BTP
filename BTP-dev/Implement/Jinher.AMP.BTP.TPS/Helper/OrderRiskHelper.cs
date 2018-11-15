using System;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.Common.Loging;
using Newtonsoft.Json.Linq;

namespace Jinher.AMP.BTP.TPS.Helper
{
    /// <summary>
    /// 订单帮助类
    /// </summary>
    public static class OrderRiskHelper
    {
        private static int RiskScore;

        static OrderRiskHelper()
        {
            var riskScore = ConfigurationManager.AppSettings["RiskScore"];
            if (string.IsNullOrEmpty(riskScore))
            {
                RiskScore = 85;
            }
            else
            {
                RiskScore = int.Parse(riskScore);
            }
        }

        /// <summary>
        /// 阿里云 营销风险识别-增强版
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool CheckCouponRisk(string phone, string ip)
        {
            try
            {
                var callerid = Guid.Parse("8533e0bd-3050-45b3-b3c8-517571a51783");
                long time = Time();
                var code = GetCode("qn6B6ujMNi", callerid, time);
                string urlpara = string.Format("?callerid={0}&time={1}&code={2}", callerid, time, code);
                string url = "http://testopenapi.iuoooo.com/api.svc/open.couponrisk" + urlpara;
                string paraJson = "{\"phone\":\"" + phone + "\",\"ip\": \"" + ip + "\"}";
                string data = WebRequestHelper.SendPostInfo("", paraJson);
                LogHelper.Info("OrderHelper.CheckCouponrisk，request" + phone + ":" + ip + ", response:" + data);
                JObject obj = JObject.Parse(data);
                if (obj["IsSuccess"].ToObject<bool>())
                {
                    var score = obj["Score"].ToObject<int>();
                    return score < RiskScore;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("OrderHelper.CheckCouponrisk 异常，request" + phone + ":" + ip, ex);
            }
            return true;
        }

        /// <summary>
        /// 阿里云 地址评分
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static bool CheckAddressscore(string address)
        {
            try
            {
                var callerid = Guid.Parse("8533e0bd-3050-45b3-b3c8-517571a51783");
                long time = Time();
                var code = GetCode("qn6B6ujMNi", callerid, time);
                string urlpara = string.Format("?callerid={0}&time={1}&code={2}", callerid, time, code);
                string url = "http://openapi.iuoooo.com/api.svc/open.addressscore" + urlpara;
                string paraJson = "{\"address\":\"" + address + "\"}";
                string data = WebRequestHelper.SendPostInfo("", paraJson);
                LogHelper.Info("OrderHelper.CheckCouponrisk，request" + address + ", response:" + data);
                JObject obj = JObject.Parse(data);
                if (obj["IsSuccess"].ToObject<bool>())
                {
                    var score = obj["Score"].ToObject<int>();
                    return score < RiskScore;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("OrderHelper.CheckAddressscore 异常，request" + address, ex);
            }
            return true;
        }

        private static long Time()
        {
            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            return (DateTime.Now.Ticks - startTime.Ticks) / 10000000;
        }

        private static string GetCode(string apikey, Guid callerid, long time)
        {
            var queryStr = string.Format("apikey={0}&callerid={1}&time={2}", apikey, callerid.ToString().ToLower(), time);
            return GetMd5(queryStr);
        }

        private static string GetMd5(string str)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            var bytValue = Encoding.UTF8.GetBytes(str);
            var bytHash = md5.ComputeHash(bytValue);
            md5.Clear();
            string sTemp = bytHash.Aggregate("", (current, t) => current + t.ToString("X").PadLeft(2, '0'));
            return sTemp.ToLower();
        }
    }
}
