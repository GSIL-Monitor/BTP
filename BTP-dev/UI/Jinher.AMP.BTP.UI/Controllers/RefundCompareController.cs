using Jinher.AMP.BTP.IBP.Facade;
using Jinher.JAP.MVC.UIJquery.DataGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public class RefundCompareController : Controller
    {
        //
        // GET: /RefundCompare/

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [GridAction]
        public ActionResult GetRefundCompare(Deploy.CustomDTO.OrderRefundSearchDTO searchDTO)
        {
            OrderRefundFacade orderRefundFacade = new OrderRefundFacade();
            var result = orderRefundFacade.GetOrderRefund(searchDTO);
            IList<string> show = new List<string>();
            show.Add("Id");
            show.Add("OrderId");
            show.Add("SubTime");
            show.Add("OrderRefundMoneyAndCoupun");
            show.Add("RefundYJCouponMoney");
            show.Add("RefundFreightPrice");
            show.Add("RefundYJBMoney");
            show.Add("RefundMoney");
            var gridobj = new GridModel<Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundCompareDTO>(show, result.Data.List, result.ResultCode, searchDTO.PageIndex, searchDTO.PageSize, string.Empty);
            return View(gridobj);
        }

    }
}
