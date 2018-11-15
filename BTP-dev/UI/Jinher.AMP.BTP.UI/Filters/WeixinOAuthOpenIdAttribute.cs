using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.UI.Util;
using System.Web.Routing;

namespace Jinher.AMP.BTP.UI.Filters
{
    /// <summary>
    /// 为微信中打开的页面加上openId参数。
    /// </summary>
    public class WeixinOAuthOpenIdAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //在fsp中做微信认证，btp中的认证暂停
            return;
            HttpContextBase httpContext = filterContext.HttpContext;
            HttpRequestBase request = httpContext.Request;

            if (CustomConfig.WxSignFlag!="Y" || request.Url == null ||
                !request.Url.AbsoluteUri.ToLower().Contains(CustomConfig.WxDomain))
                return;

            //只有入口需要取openId.
            if (request.UrlReferrer != null && request.UrlReferrer.AbsoluteUri.Length > 0 &&
                !request.UrlReferrer.AbsoluteUri.ToLower().Contains("mp.weixin.qq.com"))
            {
                return;
            }
            string wxOpenIdQueryString = WebUtil.GetWxCodeServiceUrl(httpContext);
            if (string.IsNullOrWhiteSpace(wxOpenIdQueryString))
            {
                return;
            }

            filterContext.Result = new RedirectResult(wxOpenIdQueryString);
        }
    }
}