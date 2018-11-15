using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.PL;
using Jinher.AMP.BTP.IBP.Facade;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Jinher.AMP.BTP.UI.Filters;
using Jinher.AMP.BTP.UI.Util;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.MVC.Controller;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.MVC.UIJquery.DataGrid;
using System.Web.Script.Serialization;
using Jinher.AMP.BTP.TPS;


namespace Jinher.AMP.BTP.UI.Controllers
{
    /// <summary>
    /// 拼团
    /// </summary>
    public class DiyGroupController : BaseController
    {
        /// <summary>
        /// 拼团管理
        /// </summary>
        /// <returns></returns>
       [CheckAppId]
        public ActionResult DiyGroupManage()
        {
            return View();
        }
       [HttpPost]
       [CheckAppId]
       public PartialViewResult PartialIndex(string searchContent,string state)
        {
            Guid appId = (Guid)System.Web.HttpContext.Current.Session["APPID"];
            DiyGroupFacade df = new DiyGroupFacade();
            int pageIndex = 1;
            if (!string.IsNullOrEmpty(Request.QueryString["currentPage"]))
            {
                pageIndex = int.Parse(Request.QueryString["currentPage"]);
            }
            int pageSize = 20;
            if (!string.IsNullOrEmpty(Request["pageSize"]))
            {
                if (!int.TryParse(Request["pageSize"], out pageSize))
                {
                    pageSize = 20;
                }
                else
                {
                    if (pageSize > 200)
                    {
                        pageSize = 200;//最大200条每次
                    }
                }

            }
            DiyGroupSearchDTO search = new DiyGroupSearchDTO()
            {
                AppId = appId,
                PageSize = pageSize,
                PageIndex = pageIndex,
                ComNameSub=searchContent,
                State=state
            };
            var searchResult = df.GetDiyGroups(search);
            List<DiyGroupManageVM> diyOrderList = searchResult.Data.Data;
            ViewBag.Count = searchResult.Data.Count;
            ViewBag.diyOrderList = diyOrderList;
            return PartialView();
        }
        /// <summary>
        /// 退款
        /// </summary>
        /// <param name="diyId"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckAppId]
        public ActionResult DiyOrderRefund(Guid diyId)
       {
           Guid appId = (Guid)System.Web.HttpContext.Current.Session["APPID"];
           DiyGroupFacade df = new DiyGroupFacade();

            DiyGroupSearchDTO search = new DiyGroupSearchDTO()
                {
                    AppId = appId,
                    DiyGoupId = diyId
                };
            var tkResult = df.Refund(search);
            if (tkResult.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "退款成功" });
            }
            return Json(new { Result = false, Messages = "退款失败" });
       }
        /// <summary>
        /// 确认成团
        /// </summary>
        /// <param name="diyId"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckAppId]
        public ActionResult DiyOrderConfirm(Guid diyId)
        {
            Guid appId = (Guid)System.Web.HttpContext.Current.Session["APPID"];
            DiyGroupFacade df = new DiyGroupFacade();

            DiyGroupSearchDTO search = new DiyGroupSearchDTO()
            {
                AppId = appId,
                DiyGoupId = diyId
            };
            var tkResult = df.ConfirmDiyGroup(search);
            if (tkResult.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "确认成团成功" });
            }
            return Json(new { Result = false, Messages = "确认成团失败" });
        }
        /// <summary>
        /// 我的拼团
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl]
        public ActionResult MyDiyGroup(Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupSearchDTO search)
        {
            return View();
        }

        public ActionResult GetDiyOrderList(Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupSearchDTO search)
        {
            Jinher.AMP.BTP.ISV.Facade.DiyGroupFacade facade = new ISV.Facade.DiyGroupFacade();

            var result = facade.GetDiyGroupList(search);
            ViewBag.diyOrderList = result;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
