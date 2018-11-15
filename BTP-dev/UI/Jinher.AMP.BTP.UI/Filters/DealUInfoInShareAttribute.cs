using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.AMP.BTP.UI.Util;
using System.Web.Routing;

namespace Jinher.AMP.BTP.UI.Filters
{
    /// <summary>
    /// 过滤掉分享出去的url中的用户信息参数。
    /// </summary>
    [Obsolete("已过期", true)]
    public class DealUInfoInShareAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContextBase httpContext = filterContext.HttpContext;
            if (!string.IsNullOrWhiteSpace(httpContext.Request.UserAgent) && httpContext.Request.UserAgent.ToLower().Contains("jhwebview"))
                return;
            string newQueryString = WebUtil.ClearUInfoFromRequest(httpContext);
            if (!string.IsNullOrWhiteSpace(newQueryString))
            {
                filterContext.Result = new RedirectResult(newQueryString);
            }
        }
    }
}