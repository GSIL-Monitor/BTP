using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Net;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.Serialization.Json;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.BTP.UI.Models;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Common;

namespace Jinher.AMP.BTP.UI.Util
{
    public class WebUtil
    {
        #region 常用参数

        /// <summary>
        /// 电商平台应用id
        /// </summary>
        public const string EsAppId = "esappid";
        /// <summary>
        ///  顶级域
        /// </summary>
        public const string BaseDomain = "iuoooo.com";

        public const string CurrencyKeyPrex = "appcurrency:";
        #endregion


        /// <summary>
        /// 获取应用Id.
        /// </summary>
        public static Guid AppId
        {
            get
            {
                string strAppId = System.Web.HttpContext.Current.Request.QueryString["appId"];
                if (string.IsNullOrEmpty(strAppId) && System.Web.HttpContext.Current.Session["APPID"] != null)
                {
                    strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString();
                }
                Guid appId;
                Guid.TryParse(strAppId, out appId);
                return appId;
            }
        }

        #region 过滤url中的用户信息相关参数：userid\sessionid\changeOrgId;

        /// <summary>
        /// 清除当前请求中的用户信息。
        /// </summary>
        /// <param name="httpContext">当前请求上下文</param>
        /// <returns>新的请求url</returns>
        public static string ClearUInfoFromRequest(HttpContextBase httpContext)
        {
            HttpRequestBase request = httpContext.Request;

            bool existWxOpenId = false;
            if (httpContext.Session["wxOpenId"] != null)
            {
                existWxOpenId = string.IsNullOrWhiteSpace(httpContext.Session["wxOpenId"].ToString()) ? false : true;
            }
            string newQueryString = GetNewRequestUrlWithoutUinfo(request, existWxOpenId);
            return newQueryString;
        }
        private static string GetNewRequestUrlWithoutUinfo(HttpRequestBase request, bool existWxOpenId)
        {
            string source = request["source"];
            //没有source参数，或值为空，或从其他页面跳转到当前页，不需要处理用户单点信息。
            if (string.IsNullOrWhiteSpace(source)
                || source.ToLower().IndexOf("share") <= -1
                || (request.UrlReferrer != null && request.UrlReferrer.AbsoluteUri.Length > 0))
            {
                return "";
            }
            //除去用户信息后的参数串。
            string reqParams = GetNewRequestParamWithoutUinfo(request, existWxOpenId);
            if (string.IsNullOrWhiteSpace(reqParams))
            {
                return "";
            }
            string[] sps = request.Url.ToString().Split("?".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (sps.Length > 0)
            {
                string newQueryString = sps[0] + "?" + reqParams;
                return newQueryString;
            }
            return "";
        }

        /// <summary>
        /// 获取当前请求除去用户信息后的参数串。
        /// </summary>
        private static string GetNewRequestParamWithoutUinfo(HttpRequestBase request, bool existWxOpenId)
        {
            //key小写。
            List<string> listQueryKeys = new List<string>();
            listQueryKeys.Add("userid");
            listQueryKeys.Add("sessionid");
            listQueryKeys.Add("changeorg");
            listQueryKeys.Add("user");

            //如果url中有wxOpenId,session中没有,则表示是分享出去的,则去掉url中的wxOpenId.
            if (request.QueryString.AllKeys.Contains("wxOpenId")
               && (!existWxOpenId))
            {
                listQueryKeys.Add("wxopenid");
            }

            //是否需要过滤，如果不需要过滤，直接返回。
            if (!NeedFilter(request, listQueryKeys))
            {
                return "";
            }
            string queryParamStringNew = FilterQueryString(request, listQueryKeys);
            return queryParamStringNew;
        }




        /// <summary>
        /// 过滤QueryString中的参数。
        /// </summary>
        /// <param name="Request">当前请求</param>
        /// <param name="filterQueryKeys">过滤参数列表</param>
        /// <returns>过滤后的请求参数串</returns>
        private static string FilterQueryString(HttpRequestBase Request, List<string> filterQueryKeys)
        {
            StringBuilder queryParamStringNew = new StringBuilder();
            var keys = Request.QueryString.AllKeys;
            foreach (string key in keys)
            {
                if (string.IsNullOrWhiteSpace(key)
                    || filterQueryKeys.Contains(key.ToLower()))
                {
                    continue;
                }
                queryParamStringNew.AppendFormat("{0}={1}&", key, Request.QueryString[key]);
            }
            string qpstr = queryParamStringNew.ToString();
            qpstr = qpstr.Length > 0 ? queryParamStringNew.ToString().Substring(0, qpstr.Length - 1) : "";
            return qpstr;
        }

        /// <summary>
        /// 是否需要过滤
        /// </summary>
        /// <param name="Request">当前请求</param>
        /// <param name="filterQueryKeys">过滤参数列表</param>
        /// <returns></returns>
        private static bool NeedFilter(HttpRequestBase Request, List<string> filterQueryKeys)
        {
            var keys = Request.QueryString.AllKeys;
            bool nf = false;
            foreach (string key in keys)
            {
                if ((!string.IsNullOrWhiteSpace(key))
                    && filterQueryKeys.Contains(key.ToLower()))
                {
                    nf = true;
                    break;
                }
            }
            return nf;
        }

        #endregion

        #region json序列化
        /// <summary>
        /// 将object类型的对象序列化为json字符串。
        /// </summary>
        /// <typeparam name="T">要序列化的对象的类型</typeparam>
        /// <param name="t">要序列化的对象</param>
        /// <returns>序列化后的字符串</returns>
        public static string ToJson<T>(T t)
        {
            DataContractJsonSerializer ds = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();
            ds.WriteObject(ms, t);

            string strReturn = Encoding.UTF8.GetString(ms.ToArray());
            ms.Close();
            return strReturn;
        }

        /// <summary>
        /// 将json字符串反序列化为对象.
        /// </summary>
        /// <typeparam name="T">目标对象的类型</typeparam>
        /// <param name="strJson">要反序列化的json字符串.</param>
        /// <returns>反序列化后的对象</returns>
        public static T FromJson<T>(string strJson) where T : class
        {
            DataContractJsonSerializer ds = new DataContractJsonSerializer(typeof(T));
            //string s = Regex.Replace(strJson, @"\\{1,}'", @"\\'");
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(strJson));

            return ds.ReadObject(ms) as T;
        }
        #endregion

        #region 微信认证

        /// <summary>
        /// 判断是否在微信内置浏览器中
        /// </summary>
        /// <returns></returns>
        public static bool SideInWeixinBroswer()
        {
            return SideInWeixinBroswer(HttpContext.Current.Request.UserAgent);
        }

        /// <summary>
        /// 判断是否在微信内置浏览器中
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static bool SideInWeixinBroswer(string userAgent)
        {

            if (string.IsNullOrEmpty(userAgent)
                || (!userAgent.Contains("MicroMessenger") && !userAgent.Contains("Windows Phone")))
            {
                //在微信外部
                return false;
            }
            //在微信内部
            return true;
        }

        /// <summary>
        /// 获取微信code的服务地址
        /// </summary>
        /// <returns></returns>
        public static string GetWxCodeServiceUrl(HttpContextBase httpContext)
        {
            HttpRequestBase request = httpContext.Request;
            string userAgent = request.UserAgent;
            if (!WebUtil.SideInWeixinBroswer(userAgent))
            {
                //在微信外部没有code.
                return "";
            }
            //有openid,直接返回。
            if (!string.IsNullOrWhiteSpace(request.QueryString["wxOpenId"]))
            {
                return "";
            }
            string weixinCode = request.QueryString["code"];
            if (string.IsNullOrWhiteSpace(weixinCode))
            {
                //保存当前url参数，微信验证会丢失除第一参数以外的所有参数。
                string srcUrl = request.Url.ToString();
                httpContext.Session["AuthorizeSourceUrl"] = srcUrl;

                //没有code参数，加code参数。
                string weixinAppId = Jinher.AMP.BTP.Common.CustomConfig.WeixinAppId;

                string wxCodeUrl = WeixinOAuth2.GetAuthorizeUrl(weixinAppId, srcUrl, "", OAuthScope.snsapi_base, "code");
                return wxCodeUrl;
            }

            //已有code,直接获取OpenId.
            string wxOpenId = GetWeixinOpenIdByRequest(request);
            if (string.IsNullOrWhiteSpace(wxOpenId))
            {
                //没有获取到openId,不用再做跳转。
                return "";
            }
            //将wxOpenId存入session.
            httpContext.Session["wxOpenId"] = wxOpenId;
            string url = "";
            if (httpContext.Session["AuthorizeSourceUrl"] != null)
            {
                url = httpContext.Session["AuthorizeSourceUrl"].ToString();
            }
            url = string.IsNullOrWhiteSpace(url) ? request.Url.ToString() : url;
            if (!url.Contains("wxOpenId"))
            {
                url += url.Contains('?') ? "&" : "?";
                url += "wxOpenId=" + wxOpenId;
            }
            return url;
        }

        /// <summary>
        /// 通过请求获取OpenId
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        private static string GetWeixinOpenIdByRequest(HttpRequestBase request)
        {
            string userAgent = request.UserAgent;
            if (!WebUtil.SideInWeixinBroswer(userAgent))
            {
                //在微信外部没有openid.
                return "";
            }
            //通过微信授权认证code获取OpenId
            string weixinCode = request.QueryString["code"];
            string openId = GetWeixinOpenId(weixinCode);
            return openId;
        }

        /// <summary>
        /// 通过微信授权认证code获取OpenId
        /// </summary>
        /// <param name="weixinCode"></param>
        /// <returns></returns>
        private static string GetWeixinOpenId(string weixinCode)
        {
            string emptyOpenId = "";
            if (string.IsNullOrEmpty(weixinCode))
            {
                return emptyOpenId;
            }
            string weixinAppId = Jinher.AMP.BTP.Common.CustomConfig.WeixinAppId;
            string weixinAppIdSecret = Jinher.AMP.BTP.Common.CustomConfig.WeixinAppIdSecret;
            //获取access_token 服务地址。
            string access_token_url = WeixinOAuth2.GetAccessTokenByCode(weixinAppId, weixinAppIdSecret, weixinCode);
            string responseText = WebRequestHelper.SendPostInfo(access_token_url, "");
            if (string.IsNullOrWhiteSpace(responseText))
            {
                return emptyOpenId;
            }
            OpenModel openModel = FromJson<OpenModel>(responseText);
            if (openModel == null)
            {
                return emptyOpenId;
            }
            return openModel.openid;
        }


        #endregion

        #region 定制应用判断

        /// <summary>
        /// 获取esappId
        /// </summary>
        /// <returns></returns>
        public static Guid GetEsAppId()
        {
            return MobileCookies.AppId;
        }
        #endregion

        /// <summary>
        /// 获取行记录相关js
        /// </summary>
        /// <returns></returns>
        public static string GetBehaviorRecordJs()
        {
            string result = string.Empty;
            string brUrlResult = string.Empty;

            try
            {
                string brUrl = RedisHelper.Get<string>(RedisKeyConst.BehaviorRecordUrl);
                //SessionCache.Current.GetCache("BehaviorRecordUrl");
                if (brUrl != null && !string.IsNullOrWhiteSpace(brUrl))
                {
                    brUrlResult = brUrl;
                }
                else
                {
                    string url = Jinher.AMP.BTP.TPS.DSSSV.Instance.GetBehaviorRecordUrl();
                    brUrlResult = url;
                    RedisHelper.Set(RedisKeyConst.BehaviorRecordUrl, url);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("WebUtil.GetBehaviorRecordJs异常", ex);
            }

            if (string.IsNullOrWhiteSpace(brUrlResult))
            {
                brUrlResult = "http://dss.iuoooo.com/Scripts/bury/mercury.js?ver=2";
            }

            result = string.Format("<script type=\"text/javascript\" src={0} id=\"maima\"></script>", brUrlResult);
            result += "<script src=\"/Content/Mobile/behaviorRecord.js\" type=\"text/javascript\"></script>";

            return result;
        }
        /// <summary>
        /// 判断是否金和webview
        /// </summary>
        /// <returns></returns>
        public static bool IsJhWebView()
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Current.Request.UserAgent) && HttpContext.Current.Request.UserAgent.ToLower().Contains("jhwebview"))
                return true;
            return false;
        }
        public static string GetAssumeULikeUrl()
        {
            if (HttpContext.Current.Request.Url.Scheme == "http")
            {
                return Jinher.AMP.BTP.Common.CustomConfig.AssumeULikeUrl;
            }
            else
            {
                return Jinher.AMP.BTP.Common.CustomConfig.AssumeULikeUrlHttps;
            }
        }
        /// <summary>
        /// 获取货币符号(portal使用)
        /// </summary>
        /// <returns></returns>
        public static string GetCurrency()
        {
            var appId = AppId;
            var sessionKey = (CurrencyKeyPrex + appId).ToLower();
            var currencySession = System.Web.HttpContext.Current.Session[sessionKey];
            string currency = "￥";
            if (currencySession == null || string.IsNullOrEmpty(currencySession.ToString()))
            {
                var zphResult = ZPHSV.Instance.GetMaskPicV(AppId);
                if (zphResult != null && !string.IsNullOrEmpty(zphResult.currencySymbol))
                {
                    currency = zphResult.currencySymbol;
                }
                System.Web.HttpContext.Current.Session[sessionKey] = currency;
            }
            else
            {
                currency = currencySession.ToString();
            }
            return currency;
        }
    }
}
