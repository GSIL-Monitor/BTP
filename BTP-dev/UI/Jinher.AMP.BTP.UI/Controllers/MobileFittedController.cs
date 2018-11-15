using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.DSS.ISV.Facade;
using Jinher.AMP.CBC.IBP.Facade;
using Jinher.AMP.CBC.Deploy;
using Jinher.AMP.CBC.Deploy.CustomDTO;
using Alipay.Class;
using System.Text;
using System.Security.Cryptography;
using Jinher.AMP.BTP.Common;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using Jinher.AMP.BTP.UI.Commons;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.MVC.Cache;
using Jinher.JAP.Common.Loging;
using System.Web.Routing;
using Jinher.AMP.FSP.ISV.Facade;
using Jinher.AMP.App.Deploy.CustomDTO;
using Jinher.AMP.BTP.UI.Models;
using Jinher.AMP.FSP.Deploy.CustomDTO;
using Jinher.AMP.BTP.UI.Util;
using Jinher.AMP.BTP.UI.Filters;
using Jinher.AMP.Coupon.Deploy.CustomDTO;
using Jinher.AMP.Coupon.Deploy.Enum;
using System.Web.Script.Serialization;
using Jinher.AMP.BTP.ISV.Facade;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public class MobileFittedController : Controller
    {
        //
        // GET: /MobileFitted/

        /// <summary>
        /// 商品列表页
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [DealUInfoInShare]
        [ArgumentExceptionDeal]
        public ActionResult CommodityList(Guid appId, Guid? promotionId)
        {
            ViewBag.AppId = appId;
            ViewBag.UrlPrefix = GetUrlPrefix();
            ViewBag.PortalUrl = CustomConfig.PortalUrl;

            ViewBag.IsShowAddCart = false;
            if (appId != Guid.Empty)
            {
                ViewBag.IsShowAddCart = BACSV.Instance.IsAddShopCartInComList(appId);
            }
            return View();
        }

        public ActionResult GetCommodityListV2(CommodityListSearchDTO search)
        {
            Jinher.AMP.BTP.ISV.Facade.CommodityFacade facade = new ISV.Facade.CommodityFacade();
            var ret = facade.GetCommodityListV2(search);
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        private string GetUrlPrefix()
        {
            string h = Request.Url.Host.ToLower();

            if (h.Contains("testbtp") || h.Contains("localhost"))
            {
                return "http://test";

            }
            else if (h.Contains("devbtp"))
            {
                return "http://dev";

            }
            else if (h.Contains("prebtp"))
            {
                return "http://pre";

            }
            else
            {
                return "http://";

            }
        }

    }
}
