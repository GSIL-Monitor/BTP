using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.BTP.UI.Util;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.UI.Filters
{

    /// <summary>
    /// 手机端url校验过滤器
    /// </summary>
    public class DealMobileUrlAttribute : ActionFilterAttribute
    {
        private const string ParamAppId = "appId";
        private const string ParamEsAppId = "esAppId";
        private const string ParamShopId = "shopId";
        private const string ParamShare = "isshowsharebenefitbtn";
        private static string[] _shareUrls = { "/mobile/commoditydetaildiy", "/mobile/commoditydetail", "/mobile/myorderdetail" };
        private const string IsDealMobileUrl = "isdealmobileurl";

        private bool _isWxAutoLogin = true;
        private UrlNeedAppParamsEnum _urlNeedAppParams = UrlNeedAppParamsEnum.None;

        ///// <summary>
        ///// context
        ///// </summary>
        //private ActionExecutingContext _filterContext = new ActionExecutingContext();
        ///// <summary>
        ///// 是否需要重定向
        ///// </summary>
        //private bool _needRedirect = false;
        ///// <summary>
        ///// 重定向url
        ///// </summary>
        //private string _redirectUrl = null;

        /// <summary>
        /// Url中必传app参数
        /// </summary>
        public UrlNeedAppParamsEnum UrlNeedAppParams
        {
            get { return _urlNeedAppParams; }
            set { _urlNeedAppParams = value; }
        }

        /// <summary>
        /// 微信中是否自动登录、默认登录
        /// </summary>
        public bool IsWxAutoLogin
        {
            get { return _isWxAutoLogin; }
            set { _isWxAutoLogin = value; }
        }


        /// <summary>
        /// 拼接url
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private void addUrlParams(ActionExecutingContext _filterContext, ref bool _needRedirect, ref string _redirectUrl, string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                return;
            if (string.IsNullOrEmpty(_redirectUrl))
                _redirectUrl = _filterContext.HttpContext.Request.Url.ToString();
            //var newUrl = _redirectUrl.UrlAddParam(key, value);
            var newUrl = UrlAddParam(_redirectUrl, key, value);

            if (newUrl != _redirectUrl)
            {
                _redirectUrl = newUrl;
                _needRedirect = true;
            }
        }

        private void removeUrlParams(ActionExecutingContext _filterContext, ref bool _needRedirect, ref string _redirectUrl, string key)
        {
            if (string.IsNullOrEmpty(key))
                return;
            if (string.IsNullOrEmpty(_redirectUrl))
                _redirectUrl = _filterContext.HttpContext.Request.Url.ToString();
            //var newUrl = _redirectUrl.UrlDelParam(key);
            var newUrl = UrlDelParam(_redirectUrl, key);
            if (newUrl != _redirectUrl)
            {
                _redirectUrl = newUrl;
                _needRedirect = true;
            }
        }

        private void dealShareParams(ActionExecutingContext _filterContext, ref bool _needRedirect, ref string _redirectUrl)
        {
            var url = _filterContext.HttpContext.Request.Url.ToString().ToLower();
            if (!_shareUrls.Any(url.Contains))
            {
                return;
            }
            addUrlParams(_filterContext, ref _needRedirect, ref _redirectUrl, ParamShare, "1");
        }

        private void removeFirstPageUserInfo(ActionExecutingContext _filterContext, ref bool _needRedirect, ref string _redirectUrl)
        {
            if (WebUtil.IsJhWebView())
                return;
            if (_filterContext.HttpContext.Request.UrlReferrer != null || string.IsNullOrEmpty(_filterContext.HttpContext.Request["source"]) || !_filterContext.HttpContext.Request["source"].Contains("share"))
                return;
            removeUrlParams(_filterContext, ref _needRedirect, ref _redirectUrl, "userid");
            removeUrlParams(_filterContext, ref _needRedirect, ref _redirectUrl, "sessionid");
            removeUrlParams(_filterContext, ref _needRedirect, ref _redirectUrl, "changeorg");
            removeUrlParams(_filterContext, ref _needRedirect, ref _redirectUrl, "user");
            if (_filterContext.HttpContext.Session["wxOpenId"] != null && !string.IsNullOrWhiteSpace(_filterContext.HttpContext.Session["wxOpenId"].ToString()))
            {
                if (_filterContext.HttpContext.Request.QueryString.AllKeys.Contains("wxOpenId"))
                    removeUrlParams(_filterContext, ref _needRedirect, ref _redirectUrl, "wxopenid");
            }
        }

        /// <summary>
        /// 判断是否处理过的url，处理过增加标识
        /// </summary>
        /// <returns></returns>
        private bool isDealedUrl(ActionExecutingContext _filterContext, ref  bool _needRedirect, ref string _redirectUrl)
        {
            if (_filterContext.HttpContext.Request.Url.ToString().ToLower().Contains(IsDealMobileUrl))
                return true;
            return false;
        }

        /// <summary>
        /// 处理esappId,appId
        /// 1.原店铺appId在url参数名为appId，后改为shopId
        /// 2.原商城appId用esAppId参数来传递，后改为appId
        /// </summary>
        /// <returns>返回当前商城的Id</returns>
        private Guid dealOldAppParams(ActionExecutingContext _filterContext, ref bool _needRedirect, ref string _redirectUrl)
        {
            Guid result = MobileCookies.AppId;

            //如果是btp页面之间跳转，则不需要
            var urlReferrer = _filterContext.HttpContext.Request.UrlReferrer;
            if (urlReferrer != null)
            {
                var url = urlReferrer.ToString().ToLower();
                if (url.Contains("btp.iuoooo.com") || url.Contains("promotion.iuoooo.com/myvouchers/voucherlist") || url.Contains("fsp.iuoooo.com"))
                    return result;
            }

            //if (isDealedUrl())
            //    return result;

            Guid reqEsAppId, reqAppId, reqShopId;
            bool hasReqEsAppId = Guid.TryParse(_filterContext.HttpContext.Request.QueryString[ParamEsAppId], out reqEsAppId) && reqEsAppId != Guid.Empty;
            bool hasReqAppId = Guid.TryParse(_filterContext.HttpContext.Request.QueryString[ParamAppId], out reqAppId) && reqAppId != Guid.Empty;
            bool hasReqShopId = Guid.TryParse(_filterContext.HttpContext.Request.QueryString[ParamShopId], out reqShopId) && reqShopId != Guid.Empty;
            bool hascjzyParms = !string.IsNullOrEmpty(_filterContext.HttpContext.Request.QueryString["producttype"]) && _filterContext.HttpContext.Request.QueryString["producttype"].ToLower().Contains("cjzy");



            //url中包含esappid，证明是老版本，则 appId->shopId,  esappId->appId
            if (hasReqEsAppId)
            {
                if (hasReqAppId)
                    addUrlParams(_filterContext, ref _needRedirect, ref _redirectUrl, ParamShopId, reqAppId.ToString());
                addUrlParams(_filterContext, ref _needRedirect, ref _redirectUrl, ParamAppId, reqEsAppId.ToString());
                removeUrlParams(_filterContext, ref _needRedirect, ref _redirectUrl, ParamEsAppId);

                result = reqEsAppId;
            }
            //url中包含appId，但没有shopId，处理方法：根据url项判断是否需要shopId，如果需要则appId->shopId
            else if (hasReqAppId)
            {
                if (!hasReqShopId && (_urlNeedAppParams == UrlNeedAppParamsEnum.ShopId || _urlNeedAppParams == UrlNeedAppParamsEnum.Both))
                    addUrlParams(_filterContext, ref _needRedirect, ref _redirectUrl, ParamShopId, reqAppId.ToString());

                return reqAppId;
            }
            //url中包含shopId，证明已走新参数模式,处理方法：如果没有appId，shopId->appId,并写入cookie，避免重定向
            else if (hasReqShopId)
            {
                addUrlParams(_filterContext, ref _needRedirect, ref _redirectUrl, ParamAppId, reqShopId.ToString());
                return reqShopId;
            }
            //未做app分离的正品会：商城id为正品会appid，
            else if (hascjzyParms)
            {
                addUrlParams(_filterContext, ref _needRedirect, ref _redirectUrl, ParamAppId, CustomConfig.ZPHAppId.ToString());
                result = CustomConfig.ZPHAppId;
            }
            //esappid、appid、shopid都没哟,目前只有商品详情有处理方案
            else
            {
                var controller = _filterContext.RouteData.Values["controller"].ToString().ToLower();
                var action = _filterContext.RouteData.Values["action"].ToString().ToLower();
                var url = controller + "/" + action;

                switch (url)
                {
                    //商品详情，如果没有任何参数商品详情页的esappid认为是商品所在应用的appId
                    case "mobile/commoditydetail":
                    case "mobile/commoditydetaildiy":
                        Guid commodityId;
                        if (Guid.TryParse(_filterContext.HttpContext.Request.QueryString["commodityId"], out commodityId))
                        {
                            ISV.Facade.CommodityFacade cf = new ISV.Facade.CommodityFacade();

                            var comAppResult = cf.GetCommodityAppId(commodityId);
                            if (comAppResult.ResultCode == 0 && comAppResult.Data != Guid.Empty)
                            {
                                result = comAppResult.Data;
                            }
                        }
                        break;
                }
            }
            return result;
        }

        //校验是否定制应用
        private void dealFittedApp(Guid appId)
        {
            if (appId != MobileCookies.AppId)
                MobileCookies.AppId = appId;
            MobileCookies.SetBtpCommonCookies(appId);
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var orginalUrl = filterContext.HttpContext.Request.Url.ToString(); ;
            bool _needRedirect = false;
            string _redirectUrl = null;

            //处理分享url
            dealShareParams(filterContext, ref _needRedirect, ref _redirectUrl);
            //处理外部打开带用户信息的问题
            removeFirstPageUserInfo(filterContext, ref _needRedirect, ref _redirectUrl);
            //处理旧版本app参数问题
            var dealAppId = dealOldAppParams(filterContext, ref _needRedirect, ref _redirectUrl);

            //校验是否定制应用
            dealFittedApp(dealAppId);

            //在微信中且未登录，直接跳转到登录页面
            if (_isWxAutoLogin && WebUtil.SideInWeixinBroswer() && !MobileLoginCookie.HasLoginCookie())
            {
                gotoLoginPage(filterContext, ref _needRedirect, ref _redirectUrl, dealAppId);
            }

            if (_needRedirect)
            {
                //  addUrlParams(IsDealMobileUrl, "1");
                // LogHelper.Info("DealMobileUrlAttribute.Redirect, OrginalUrl: " + orginalUrl + "; RedirectUrl: " + _redirectUrl);
                var oldUserId = HttpUtility.ParseQueryString(new Uri(orginalUrl).Query).Get("userId");
                var newUserId = HttpUtility.ParseQueryString(new Uri(_redirectUrl).Query).Get("userId");
                if (!string.IsNullOrEmpty(newUserId) && oldUserId != newUserId)
                {
                    LogHelper.Error("DealMobileUrlAttribute.RedirectError, OrginalUrl: " + orginalUrl + "; RedirectUrl: " + _redirectUrl);
                }
                filterContext.Result = new RedirectResult(_redirectUrl);
            }
            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// 微信中自动跳转到登录页面
        /// </summary>
        /// <param name="esAppId"></param>
        private void gotoLoginPage(ActionExecutingContext _filterContext, ref bool _needRedirect, ref string _redirectUrl, Guid esAppId)
        {
            removeUrlParams(_filterContext, ref _needRedirect, ref _redirectUrl, "islogin");
            string loginUrl = CustomConfig.PipWxLogin;
            if (_filterContext.HttpContext.Request.Url.Scheme == "https")
            {
                loginUrl = loginUrl.Replace("http://", "https://");
            }
            _redirectUrl = string.Format(loginUrl, esAppId, HttpUtility.UrlEncode(_redirectUrl), new Random().Next(100000));

            //cf063155-e6e9-4019-ba12-6b44b704243f 买走美国 8b4d3317-6562-4d51-bef1-0c05694ac3a6 易捷北京 0b85c71d-a272-4f0d-a6ce-386415cf4c69 丹农优品
            _needRedirect = (esAppId != new Guid("cf063155-e6e9-4019-ba12-6b44b704243f") && esAppId != new Guid("8b4d3317-6562-4d51-bef1-0c05694ac3a6") && esAppId != new Guid("0b85c71d-a272-4f0d-a6ce-386415cf4c69"));
        }

        private string UrlAddParam(string url, string key, string value)
        {
            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(key))
            {
                return url;
            }
            string newParam = string.Format("{0}={1}", key, value);
            Regex reg = new Regex(string.Format("(?<=\\?|\\&){0}=[^\\&]*", key), RegexOptions.IgnoreCase);
            if (reg.IsMatch(url))
            {
                url = reg.Replace(url, newParam);
            }
            else
            {
                if (url.Contains("?"))
                {
                    url += "&" + newParam;
                }
                else
                {
                    url += "?" + newParam;
                }
            }
            return url;
        }

        private string UrlDelParam(string url, string key)
        {
            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(key))
            {
                return url;
            }
            Regex reg = new Regex(string.Format("((?<=\\?)|\\&){0}=[^\\&]*", key), RegexOptions.IgnoreCase);
            url = reg.Replace(url, "");
            url = url.TrimEnd('?');
            return url;
        }

    }
    /// <summary>
    /// url中需要app参数
    /// </summary>
    public enum UrlNeedAppParamsEnum
    {
        /// <summary>
        /// 不需要
        /// </summary>
        None = 0,
        /// <summary>
        /// 商城Id
        /// </summary>
        AppId,
        /// <summary>
        /// 店铺id
        /// </summary>
        ShopId,
        /// <summary>
        /// 所有
        /// </summary>
        Both

    }
}
