using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using System.Data;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public class RefundCouponController : Controller
    {
        //
        // GET: /RefundCoupon/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetData(RefundCouponSearchDTO dto)
        {
            var facade = new Jinher.AMP.BTP.IBP.Facade.CouponRefundFacade();
            var result = facade.GetCouponRefundList(dto);
            return Json(result);
        }

        /// <summary>
        /// 获取OrderId
        /// </summary>
        /// <param name="OrderCode"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetOrderId(string OrderCode)
        {
            string commodityid = "";
            if (!string.IsNullOrEmpty(OrderCode))
            {
                var id = BE.CommodityOrder.ObjectSet().Where(x => x.Code == OrderCode).Select(x => x.Id).FirstOrDefault();
                if (id != null && id != Guid.Empty)
                {
                    commodityid = id.ToString();
                }
            }
            return Json(commodityid);
        }

        [HttpPost]
        public ActionResult SaveRemark(string Id,string Remark)
        {
            var facade = new Jinher.AMP.BTP.IBP.Facade.CouponRefundFacade();
            var result = new ResultDTO();
            if (!string.IsNullOrEmpty(Id))
            {
                Guid GId = Guid.Parse(Id);
                result = facade.UpdateRemark(GId, Remark);
            }
            return Json(result.isSuccess);
        }

        /// <summary>
        /// 导出抵用券退款明细
        /// </summary>
        /// <param name="JdCode"></param>
        /// <param name="AppId"></param>
        /// <returns></returns>
        //[HttpGet]
        //public ActionResult ExportCouponRefund(SearchBase dto)
        //{
        //    var facade = new Jinher.AMP.BTP.IBP.Facade.CouponRefundFacade();
        //    var result = facade.GetCouponRefundList(dto);
        //    if (!result.isSuccess)
        //    {
        //        return Json(result);
        //    }
        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("退款时间", typeof(string));
        //    dt.Columns.Add("收款账号（登录账户）", typeof(string));
        //    dt.Columns.Add("收款人姓名/昵称", typeof(string));
        //    dt.Columns.Add("商品退款抵用券金额", typeof(string));
        //    dt.Columns.Add("运费退款抵用券金额", typeof(string));
        //    dt.Columns.Add("商品退款金额", typeof(string));
        //    dt.Columns.Add("运费退款金额", typeof(string));
        //    dt.Columns.Add("退款金额合计", typeof(string));
        //    dt.Columns.Add("店铺名称", typeof(string));
        //    dt.Columns.Add("订单编号", typeof(string));
        //    dt.Columns.Add("商品名称", typeof(string));
        //    dt.Columns.Add("收货人手机号", typeof(string));
        //    dt.Columns.Add("收货人姓名", typeof(string));
        //    dt.Columns.Add("备注", typeof(string));
        //    foreach (var d in result.Data.List)
        //    {
        //        //dt.Rows.Add(d.JDCode, d.CategoryName, d.VideoName, d.TaxClassCode, d.TaxRate, d.InputRax);
        //    }
        //    return File(Jinher.AMP.BTP.Common.ExcelHelper.Export(dt), "application/vnd.ms-excel", string.Format("export_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
        //}

    }
}
