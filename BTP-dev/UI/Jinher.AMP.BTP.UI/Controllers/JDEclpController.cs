using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.MVC.UIJquery.DataGrid;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public class JDEclpController : Controller
    {
        /// <summary>
        /// 进销存-京东实时库存同步日志视图
        /// </summary>
        /// <returns></returns>
        public ActionResult JDStockjournalList()
        {
            return View();
        }

        /// <summary>
        /// AJAX获取StockJournal日志
        /// </summary>
        /// <param name="dtBeginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [GridAction]
        public ActionResult GetStockJournalData(String dtBeginDate, String dtEndDate)
        {
            Jinher.AMP.BTP.IBP.Facade.JdEclpOrderFacade comFacade = new IBP.Facade.JdEclpOrderFacade();
            int pageIndex = Convert.ToInt32(Request["page"] ?? "1");
            int pageSize = Convert.ToInt32(Request["rows"] ?? "20");

            var searcharg = new JourneyDTO()
            {
                dtBegTime = string.IsNullOrEmpty(dtBeginDate) ? DateTime.MinValue : DateTime.Parse(dtBeginDate),
                dtEndTime = string.IsNullOrEmpty(dtEndDate) ? DateTime.MaxValue : DateTime.Parse(dtEndDate),
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            List<string> showList = new List<string>();
            showList.Add("Id");
            showList.Add("CommodityErQiCode");
            showList.Add("CommodityOldStock");
            showList.Add("CommodityNewStock");
            showList.Add("CommodityStockOldStock");
            showList.Add("Json");

            var resultData = comFacade.GetJDStockJourneyList(searcharg);
            return View(new GridModel<JDStockJournalDTO>(showList, resultData.Data, resultData.ResultCode, pageIndex, pageSize, string.Empty));
        }

        /// <summary>
        /// 进销存-京东订单日志
        /// </summary>
        /// <returns></returns>
        public ActionResult JDEclpOrderJournalList()
        {
            return View();
        }

        /// <summary>
        /// AJAX获取EclpOrderJournal日志
        /// </summary>
        /// <param name="dtBeginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [GridAction]
        public ActionResult GetEclpOrderJournalData(String dtBeginDate, String dtEndDate,String OrderCode,String EclpOrderNo, String OrderID)
        {
            Jinher.AMP.BTP.IBP.Facade.JdEclpOrderFacade comFacade = new IBP.Facade.JdEclpOrderFacade();
            int pageIndex = Convert.ToInt32(Request["page"] ?? "1");
            int pageSize = Convert.ToInt32(Request["rows"] ?? "20");

            var searcharg = new JourneyDTO()
            {
                dtBegTime = string.IsNullOrEmpty(dtBeginDate) ? DateTime.MinValue : DateTime.Parse(dtBeginDate),
                dtEndTime = string.IsNullOrEmpty(dtEndDate) ? DateTime.MaxValue : DateTime.Parse(dtEndDate),
                OrderCode = OrderCode,
                EclpOrderNo = EclpOrderNo,
                OrderID =  String.IsNullOrEmpty(OrderID)?Guid.Empty: Guid.Parse(OrderID),
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            List<string> showList = new List<string>();
            showList.Add("Id");
            showList.Add("EclpOrderNo");
            showList.Add("OrderId");
            showList.Add("AppName");
            showList.Add("OrderCode");
            showList.Add("Name");
            showList.Add("StateFrom");
            showList.Add("StateTo");
            showList.Add("Details");
            showList.Add("SubTime");
            showList.Add("Code");
            showList.Add("SupplierName");
            showList.Add("AppType");
            showList.Add("Json");
            
            var resultData = comFacade.GetJDEclpOrderJournalList(searcharg);
            return View(new GridModel<Deploy.CustomDTO.JdEclp.JDEclpJourneyExtendDTO>(showList, resultData.Data, resultData.ResultCode, pageIndex, pageSize, string.Empty));
        }

        /// <summary>
        /// 京东服务单日志列表
        /// </summary>
        /// <returns></returns>
        public ActionResult JDEclpOrderRefundAfterSalesJournalList()
        {
            return View();
        }

        /// <summary>
        /// AJAX获取京东服务单日志
        /// </summary>
        /// <param name="dtBeginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [GridAction]
        public ActionResult GetEclpOrderRefundAfterSalesJournalData(String dtBeginDate, String dtEndDate)
        {
            Jinher.AMP.BTP.IBP.Facade.JdEclpOrderFacade comFacade = new IBP.Facade.JdEclpOrderFacade();
            int pageIndex = Convert.ToInt32(Request["page"] ?? "1");
            int pageSize = Convert.ToInt32(Request["rows"] ?? "20");

            var searcharg = new JourneyDTO()
            {
                dtBegTime = string.IsNullOrEmpty(dtBeginDate) ? DateTime.MinValue : DateTime.Parse(dtBeginDate),
                dtEndTime = string.IsNullOrEmpty(dtEndDate) ? DateTime.MaxValue : DateTime.Parse(dtEndDate),
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            List<string> showList = new List<string>();
            showList.Add("Id");
            showList.Add("EclpOrderNo");
            showList.Add("EclpServicesNo");
            showList.Add("OrderId");
            showList.Add("Details");
            showList.Add("SubTime");

            var resultData = comFacade.GetJDEclpOrderRefundAfterSalesJournalList(searcharg);
            return View(new GridModel<JDEclpOrderRefundAfterSalesJournalDTO>(showList, resultData.Data, resultData.ResultCode, pageIndex, pageSize, string.Empty));
        }
        
        /// <summary>
        /// 重新创建京东订单
        /// </summary>
        /// <param name="orderID"></param>
        public void SetUnitityData(String orderID)
        {
            if (!String.IsNullOrEmpty(orderID))
            {
                new IBP.Facade.JdEclpOrderFacade().CreateOrder(Guid.Parse(orderID), string.Empty);
            }
        }
    }
}
