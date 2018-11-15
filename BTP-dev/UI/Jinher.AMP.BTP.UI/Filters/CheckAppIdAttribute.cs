using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Jinher.AMP.BTP.UI.Filters
{
    /// <summary>
    /// 检查并更新APPId的过滤器
    /// </summary>
    public class CheckAppIdAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string strAppId = filterContext.HttpContext.Request.QueryString["appId"];
            if (string.IsNullOrEmpty(strAppId) && System.Web.HttpContext.Current.Session["APPID"] != null)
            {
                strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString();
            }
            Guid appId = Guid.Empty;
            if (!Guid.TryParse(strAppId, out appId))
            {
                filterContext.HttpContext.Response.StatusCode = 404;
                filterContext.HttpContext.Response.End();
                return;
            }
            if (appId != Guid.Empty)
            {
                System.Web.HttpContext.Current.Session["APPID"] = appId;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}