using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Text;
using System.Net;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.BTP.UI.Models;

namespace Jinher.AMP.BTP.UI.Util
{
    /// <summary>
    /// 手机端公共参数
    /// </summary>
    public class MobileCookies
    {
        /// <summary>
        /// 公共参数
        /// </summary>
        private const string Bdata = "b_data";

        private const string BdataAppId = "appId";

        private const string BtpCommonDataKey = "btpcommon";

        /// <summary>
        /// 顶级域
        /// </summary>
        public const string CommonDomain = "iuoooo.com";

        private static Dictionary<string, object> getBData()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            var cookie = System.Web.HttpContext.Current.Request.Cookies[Bdata];
            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                var cookieValue = HttpUtility.UrlDecode(cookie.Value);
                if (cookieValue == null)
                    return result;
                var obj = js.DeserializeObject(cookieValue);
                if (obj != null && obj.GetType().Name == typeof(Dictionary<,>).Name)
                    return obj as Dictionary<string, object>;
            }
            return result;
        }
        private static void setBDataAppId(Guid appId)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            Dictionary<string, object> cookieData = new Dictionary<string, object>();
            cookieData.Add(BdataAppId, appId);
            var cookie = new HttpCookie(Bdata) { Value = HttpUtility.UrlEncode(js.Serialize(cookieData)), Domain = CommonDomain };
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        /// <summary>
        /// 平台id
        /// </summary>
        public static Guid AppId
        {
            get
            {
                var cookieData = getBData();

                Guid appId;
                if (cookieData.ContainsKey(BdataAppId) && Guid.TryParse(cookieData[BdataAppId].ToString(), out appId))
                {
                    return appId;
                }
                return Guid.Empty;
            }
            set
            {
                setBDataAppId(value);
            }
        }
        /// <summary>
        /// 获取公共cookie
        /// </summary>
        /// <returns></returns>
        private static CommonCookies getBtpCommonCookies()
        {
            CommonCookies result = new CommonCookies();
            var cookie = System.Web.HttpContext.Current.Request.Cookies[BtpCommonDataKey];
            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                var cookieValue = HttpUtility.UrlDecode(cookie.Value);
                if (cookieValue == null)
                    return result;
                var obj = js.Deserialize<CommonCookies>(cookieValue);
                if (obj != null)
                    result = obj;
            }
            return result;
        }
        /// <summary>
        /// 是否定制app
        /// </summary>
        /// <returns></returns>
        public static bool IsFittedApp()
        {
            var cookieData = getBtpCommonCookies();
            return cookieData.IsFitted;
        }
        /// <summary>
        /// 获取皮肤类型
        /// </summary>
        /// <returns></returns>
        public static int GetSkinType()
        {
            var cookieData = getBtpCommonCookies();
            return cookieData.SkinType;
        }
        /// <summary>
        /// 设置btp通用cookie
        /// </summary>
        public static void SetBtpCommonCookies(Guid esAppId)
        {
            var cookieData = getBtpCommonCookies();
            var currency = "￥";
            if (ZPHSV.Instance.GetMaskPicV(esAppId) != null)
            {
                currency = ZPHSV.Instance.GetMaskPicV(esAppId).currencySymbol;
            }
            if (cookieData.AppId != esAppId)
            {
                cookieData.AppId = esAppId;
                cookieData.IsFitted = APPBP.IsFittedApp(esAppId);
                cookieData.SkinType = APPSV.GetSkinType(esAppId);
                cookieData.LayoutCode = BACSV.Instance.GetAppLayoutCode(esAppId);
                cookieData.Currency = currency;

                JavaScriptSerializer js = new JavaScriptSerializer();
                var cookie = new HttpCookie(BtpCommonDataKey) { Value = HttpUtility.UrlEncode(js.Serialize(cookieData)) };
                HttpContext.Current.Response.Cookies.Add(cookie);
            }

            var currencyCookie = new HttpCookie("Currency") { Value = currency };
            HttpContext.Current.Response.Cookies.Add(currencyCookie);
        }
        /// <summary>
        /// 获取当前应用版式
        /// </summary>
        /// <returns></returns>
        public static string GetLayoutCode()
        {
            var cookieData = getBtpCommonCookies();
            return cookieData.LayoutCode;
        }

        /// <summary>
        /// 是否河套版式
        /// </summary>
        /// <returns></returns>
        public static bool IsHetaoLayout()
        {
            var cookieData = getBtpCommonCookies();
            return cookieData.LayoutCode == BACSV.HetaoLayoutKey;
        }
        /// <summary>
        /// 返回货币符号，默认返回“￥”
        /// </summary>
        /// <returns></returns>
        public static string GetCurrency()
        {
            var cookieData = getBtpCommonCookies();
            if (string.IsNullOrEmpty(cookieData.Currency))
            {
                return "￥";
            }
            return cookieData.Currency;
        }
    }
}
