using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.BTP.UI.Filters;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public class SettlementManageController : Controller
    {
        [CheckAppId]
        public ActionResult Index()
        {
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return HttpNotFound();
            }
            ViewBag.appId = appId;
            ViewBag.userId = Request["userId"]; 
            if (ZPHSV.Instance.IsAppPavilion(appId))
            {
                ViewBag.Name = " 入驻管理-商城";
                return View("SettlementManageShop");
            }
            else
            {
                ViewBag.Name = "入驻管理-商家";
                return View("SettlementManageUser");
            }
        }

        public ActionResult SettlementManageUser()
        {   
            //传到前端的属于esAppId
            string appId = Request["appId"];
            string userId = Request["userId"];
            ViewBag.Name = "入驻管理-商家";
            ViewBag.appId = appId;
            ViewBag.userId = userId;
            return View();
        }


        public ActionResult SettlementManageShop()
        {
            string appId = Request["appId"];
            string userId = Request["userId"];
            ViewBag.Name = " 入驻管理-商城";
            ViewBag.appId = appId;
            ViewBag.userId = userId;
            return View();
        }

    }
}
