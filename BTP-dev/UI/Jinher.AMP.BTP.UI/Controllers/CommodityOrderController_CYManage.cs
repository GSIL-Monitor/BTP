using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.UI.Filters;
using Jinher.JAP.MVC.Controller;
using Jinher.AMP.BTP.IBP.Facade;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public partial class CommodityOrderController : BaseController
    {
        #region 订单管理

        /// <summary>
        /// 订单管理首页
        /// </summary>
        /// <returns></returns>
        [CheckAppId]
        public ActionResult CYOrderManage()
        {
            Guid appId = Guid.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request["appId"]))
                {
                    ViewBag.AppId = Guid.TryParse(Request["appId"], out appId) ? appId : Guid.Empty;
                    Jinher.AMP.BTP.ISV.Facade.StoreFacade sf = new Jinher.AMP.BTP.ISV.Facade.StoreFacade();
                    var _store = sf.GetOnlyStoreInApp(appId);
                    if (_store.Data != null)
                    {
                        ViewBag.StoreName = _store.Data.StoreName;
                        ViewBag.StoreId = _store.Data.Id;
                    }
                }
                if (!string.IsNullOrEmpty(Request["userId"]))
                {
                    ViewBag.UserId = Guid.TryParse(Request["userId"], out appId) ? appId : Guid.Empty;
                }
            }
            catch (Exception ex)
            {
                JAP.Common.Loging.LogHelper.Error(string.Format("Exception。appId：{0}，userId：{1}", appId, this.ContextDTO.LoginUserID), ex);
            }
            return View();
        }

        /// <summary>
        /// 订单管理部分页，订单列表页
        /// </summary>
        /// <param name="priceLow"></param>
        /// <param name="priceHight"></param>
        /// <param name="seacrhContent"></param>
        /// <param name="dayCount"></param>
        /// <param name="state"></param>
        /// <param name="payment"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="esAppId"></param>
        /// <returns></returns>
        [CheckAppId]
        public PartialViewResult CYOrderManagePartialIndex(string priceLow, string priceHight, string seacrhContent, string dayCount, string state, string payment, string startTime, string endTime, Guid? esAppId)
        {
            Guid appId = Guid.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request["appId"]))
                {
                    appId = Guid.TryParse(Request["appId"], out appId) ? appId : Guid.Empty;
                    Jinher.AMP.BTP.ISV.Facade.StoreFacade sf = new Jinher.AMP.BTP.ISV.Facade.StoreFacade();
                    var _store = sf.GetOnlyStoreInApp(appId);
                    if (_store.Data != null)
                    {
                        ViewBag.StoreName = _store.Data.StoreName;
                    }
                }
            }
            catch (Exception ex)
            {
                JAP.Common.Loging.LogHelper.Error(string.Format("Exception。appId：{0}，userId：{1}", appId, this.ContextDTO.LoginUserID), ex);
            }
            state = state == null || state.Equals("-1") ? "4, 7, 18, 19, 20, 21" : state;

            return this.PartialIndex(priceLow, priceHight, seacrhContent, dayCount, state, payment, startTime, endTime, esAppId, null);
        }

        /// <summary>
        /// 订单详情
        /// </summary>
        /// <param name="commodityOrderId"></param>
        /// <returns></returns>
        [CheckAppId]
        public ActionResult CYOrderDetails(string commodityOrderId)
        {
            Guid orderId = Guid.TryParse(commodityOrderId, out orderId) ? orderId : Guid.Empty;
            Jinher.AMP.BTP.IBP.Facade.CommodityOrderFacade coFacade = new Jinher.AMP.BTP.IBP.Facade.CommodityOrderFacade();
            Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade coSvFacade = new Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade();
            Guid appId = Guid.Empty;
            Guid userId = Guid.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request["appId"]))
                {
                    appId = Guid.TryParse(Request["appId"], out appId) ? appId : Guid.Empty;
                    Jinher.AMP.BTP.ISV.Facade.StoreFacade sf = new Jinher.AMP.BTP.ISV.Facade.StoreFacade();
                    var _store = sf.GetOnlyStoreInApp(appId);
                    if (_store.Data != null)
                    {
                        ViewBag.StoreName = _store.Data.StoreName;
                    }
                }
                if (!string.IsNullOrEmpty(Request["userId"]))
                {
                    userId = Guid.TryParse(Request["userId"], out userId) ? userId : Guid.Empty;
                }
            }
            catch (Exception ex)
            {
                JAP.Common.Loging.LogHelper.Error(string.Format("Exception。orderId：{0}，appId：{1}，userId：{2}", orderId, appId, this.ContextDTO.LoginUserID), ex);
            }

            CommodityOrderVM covm = coFacade.GetCommodityOrder(orderId, appId);
            ViewBag.CommodityOrder = covm;
            CommodityOrderSDTO cosdto = coSvFacade.GetOrderItems(orderId, userId, appId);
            ViewBag.orderFullInfo = cosdto;
            return View();
        }

        /// <summary>
        /// 交班
        /// </summary>
        /// <returns></returns>
        [CheckAppId]
        public ActionResult ShiftChange()
        {
            Guid temp;
            DateTime result = DateTime.Now;
            ShiftManageFacade smf = new ShiftManageFacade();

            Deploy.CustomDTO.ShiftLogDTO sld = new Jinher.AMP.BTP.Deploy.CustomDTO.ShiftLogDTO();

            sld.id = Guid.NewGuid();

            if (string.IsNullOrEmpty(Request["userId"]))
            {
                return Json(new { success = false, data = string.Empty, code = -1, msg = "无交班人信息。" });
            }
            sld.userId = Guid.TryParse(Request["userId"], out temp) ? temp : Guid.Empty;
            sld.subId = sld.userId;

            if (string.IsNullOrEmpty(Request["shiftTime"]))
            {
                return Json(new { success = false, data = string.Empty, code = -1, msg = "无交班时间。" });
            }
            sld.shiftTime = DateTime.TryParse(Request["shiftTime"], out result) ? result : DateTime.Now;

            if (string.IsNullOrEmpty(Request["storeId"]))
            {
                return Json(new { success = false, data = string.Empty, code = -1, msg = "无交班人所在门店信息。" });
            }
            sld.storeId = Guid.TryParse(Request["storeId"], out temp) ? temp : Guid.Empty;

            if (string.IsNullOrEmpty(Request["appId"]))
            {
                return Json(new { success = false, data = string.Empty, code = -1, msg = "无交班应用标识信息。" });
            }
            sld.appId = Guid.TryParse(Request["appId"], out temp) ? temp : Guid.Empty;

            var data = smf.ShiftExchange(sld);

            if (data.isSuccess)
            {
                System.Web.Security.FormsAuthentication.SignOut();
            }
            return Json(new { success = data.isSuccess, data = data, code = data.ResultCode, msg = data.Message });
        }

        #region 餐饮订单退款

        /// <summary>
        /// 餐饮订单退款
        /// </summary>
        /// <param name="commodityOrderId">餐饮订单Id</param>
        /// <param name="state">订单状态</param>
        /// <param name="message">退款原因</param>
        /// <param name="userId">退款操作人</param>
        /// <param name="refundType">退款方式，整单、部分</param>
        /// <param name="orderItemIds">部分退款的商品ID列表，可空</param>
        /// <returns></returns>
        public ActionResult UpdateCYCommodityOrder(Guid commodityOrderId, int state, string message, Guid userId, int refundType, string orderItemIds, bool? hasFreight)
        {
            CommodityOrderFacade cf = new CommodityOrderFacade();

            UpdateCommodityOrderParamDTO dto = new UpdateCommodityOrderParamDTO();
            dto.orderId = commodityOrderId;
            dto.targetState = state;
            dto.remessage = message;
            dto.userId = userId;
            dto.CYRefundType = refundType;

            if (refundType == 1)
            {
                string[] orderItemIdArry = orderItemIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                Guid orderItemId;
                dto.OrderItemIds = new List<Guid>();
                foreach (string item in orderItemIdArry)
                {
                    dto.OrderItemIds.Add(Guid.TryParse(item, out orderItemId) ? orderItemId : Guid.Empty);
                }
            }

            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result = cf.CancelTheOrder(dto);
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "修改成功" });
            }
            return Json(new { Result = false, Messages = result.Message });
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult ShiftLogOff()
        {
            base.Session.Timeout = 0;
            return RedirectToAction("Index", "Login");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commodityOrderId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public ActionResult GetCommodityOrderById(Guid commodityOrderId, Guid appId)
        {
            CommodityOrderFacade cf = new CommodityOrderFacade();
            CommodityOrderVM commodityOrder = cf.GetCommodityOrder(commodityOrderId, appId);

            return Json(new { success = commodityOrder != null, data = commodityOrder, code = commodityOrder != null ? 0 : 1, msg = commodityOrder != null ? "success" : "error" });
        }

        #endregion
    }
}
