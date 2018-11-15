using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.BTP.UI.Filters;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public class CustomMenuController : Controller
    {
        /// <summary>
        /// 自定义菜单-选择电商菜单页面。
        /// </summary>
        /// <returns></returns>
        [CheckAppId]
        public ActionResult Index()
        {
            List<string> functionCodes = new List<string>();
            functionCodes.Add("home");
            functionCodes.Add("CategoryManage");
            functionCodes.Add("ShoppingCart");
            functionCodes.Add("presale");
            functionCodes.Add("Seckill");
            functionCodes.Add("Coupon");
            functionCodes.Add("mypaper");
            Guid appId = (Guid)System.Web.HttpContext.Current.Session["APPID"];
            var menuData = BACSV.Instance.GetFunctionUsedInfo(appId, functionCodes);
            JavaScriptSerializer js = new JavaScriptSerializer();
            string mjson =  js.Serialize(menuData);
            mjson = HttpUtility.UrlEncode(mjson);
            ViewBag.MenuData = mjson;
            return View();
        }

    }
}
