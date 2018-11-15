using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.AMP.BTP.UI.Models;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.MVC.UIJquery.DataGrid;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.MVC.Controller;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public class OrderExceptionController : BaseController
    {

        public ActionResult Index()
        {
            string fullName = this.GetType() + ".Index";
            List<Jinher.AMP.BTP.Deploy.CustomDTO.AppSetAppDTO> appList = APPManageVM.GetAppSet(fullName);
            ViewBag.appList = appList;

            return View();
        }

        [GridAction]
        public ActionResult GetOrderExceptionGrid(CommodityOrderExceptionParamDTO orderExcParamDto)
        {
            int pNum = 0;
            int.TryParse(Request["page"], out pNum);

            int pSize = 0;
            int.TryParse(Request["rows"], out pSize);

            orderExcParamDto.PageNumber = pNum;
            orderExcParamDto.PageSize = pSize;

            var result = OrderExceptionVM.GetOrderExceptionByAppId(orderExcParamDto);

            List<string> showList = new List<string>();
            showList.Add("Id");
            showList.Add("ExceptionTime");
            showList.Add("ClearingPrice");
            showList.Add("OrderRealPrice");
            showList.Add("ExceptionReason");
            showList.Add("OrderCode");
            showList.Add("AppName");
            showList.Add("State");
            showList.Add("Note");
            showList.Add("SubId");

            int total = 0;
            int.TryParse(result.Message, out total);

            return View(new GridModel<Jinher.AMP.BTP.Deploy.CommodityOrderExceptionDTO>(showList, result.Data, total, orderExcParamDto.PageNumber, string.Empty));

        }

        public ActionResult UpdateOrderException(Jinher.AMP.BTP.Deploy.CommodityOrderExceptionDTO dto)
        {
            string invoker = this.GetType() + ".UpdateOrderException";
            ResultDTO result = OrderExceptionVM.UpdateOrderException(dto, invoker);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
