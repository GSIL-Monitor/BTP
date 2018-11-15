using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.JAP.MVC.Controller;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.UI.Filters;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public partial class CommodityOrderController : BaseController
    {
        #region 接单打印

        /// <summary>
        /// 接单打印首页
        /// </summary>
        /// <returns></returns>
        [CheckAppId]
        public ActionResult BPrintOrders()
        {
            return this.Index();
        }

        /// <summary>
        /// 获取某一订单信息输出到打印机
        /// </summary>
        /// <param name="orderId">订单Id</param>
        /// <returns></returns>
        [CheckAppId]
        public ActionResult CYOrderPrint(string orderId)
        {
            CommodityOrderFacade coFacade = new CommodityOrderFacade();
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
                JAP.Common.Loging.LogHelper.Error(string.Format("Exception。orderId：{0}，appId：{1}，userId：{2}", orderId, appId, this.ContextDTO.LoginUserID), ex);
            }

            CommodityOrderVM covm = coFacade.GetCommodityOrder(Guid.Parse(orderId), appId);
            ViewBag.orderFullInfo = covm;
            return View("~/Views/CommodityOrder/CYOrderPrint.cshtml");
        }

        /// <summary>
        /// 根据appId，支付时间判断是否有新订单
        /// </summary>
        /// <param name="appId">应用Id</param>
        /// <param name="cylastPayTicks">支付时间的Ticks</param>
        /// <returns></returns>
        public ActionResult HasNewCyCommodityOrder(Guid appId, long cylastPayTicks)
        {
            DateTime time = new DateTime();
            DateTime? startOrderTime = null;
            DateTime? endOrderTime = null;
            DateTime? lastPaymentTime = null;
            CommodityOrderFacade coFacade = new CommodityOrderFacade();
            DateTime lastPayTime = DateTime.MinValue.AddTicks(cylastPayTicks);
            var result = coFacade.GetNewCyUntreatedCount(appId, lastPayTime);

            if (result > 0)
            {
                int pageIndex = 1;
                if (!string.IsNullOrEmpty(Request.QueryString["currentPage"]))
                {
                    pageIndex = int.Parse(Request.QueryString["currentPage"]);
                }
                int pageSize = result;

                CommodityOrderSearchDTO search = new CommodityOrderSearchDTO()
                {
                    AppId = appId,
                    PageSize = pageSize,
                    PageIndex = pageIndex,
                    PriceLow = "",
                    PriceHight = "",
                    SeacrhContent = "",
                    DayCount = "",
                    State = "18",
                    Payment = "",
                    StartOrderTime = startOrderTime,
                    EndOrderTime = endOrderTime,
                    EsAppId = null,
                    LastPayTime = lastPayTime
                };
                var searchResult = coFacade.GetAllCommodityOrderByAppId(search);
                return Json(new { success = result > 0, data = searchResult.Data, count = searchResult.Count, orderTicks = searchResult.Data[0].PaymentTime.Value.Ticks, msg = "有新订单" });
            }
            else
            {
                return Json(new { success = result > 0, data = result, msg = "无新订单" });
            }
        }

        /// <summary>
        /// 接单打印部分页
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
        public PartialViewResult CYPartialIndex(string priceLow, string priceHight, string seacrhContent, string dayCount, string state, string payment, string startTime, string endTime, Guid? esAppId)
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
            state = "18";
            return this.PartialIndex(priceLow, priceHight, seacrhContent, dayCount, state, payment, startTime, endTime, esAppId,null);
        }

        #endregion
    }
}
