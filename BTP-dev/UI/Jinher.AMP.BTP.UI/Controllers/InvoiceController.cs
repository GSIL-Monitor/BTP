using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.UI.Filters;
using Jinher.JAP.MVC.Controller;
using System.IO;
using System.Data;
using Jinher.AMP.BTP.Common;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public class InvoiceController : BaseController
    {
        /// <summary>
        /// 发票信息
        /// </summary>
        /// <returns></returns>
        [CheckAppId]
        public ActionResult InvoiceManage()
        {
            Guid appId = new Guid(Request["appId"]);
            Jinher.AMP.BTP.IBP.Facade.InvoiceFacade facade = new InvoiceFacade();
            var appEx = facade.GetInvoiceCategory(appId);
            ViewBag.InvoiceDefault = appEx.Data.InvoiceDefault;
            ViewBag.InvoiceValues = appEx.Data.InvoiceValues;
            ViewBag.appid = appId.ToString();
            OrderFieldFacade ordersetfacde = new OrderFieldFacade();
            ViewBag.OrderSet = ordersetfacde.GetOrderSet(appId);
            return View();
        }

        /// <summary>
        /// 发票信息
        /// </summary>
        /// <returns></returns>
        public ActionResult InvoiceSearch(Jinher.AMP.BTP.Deploy.CustomDTO.InvoiceSearchDTO search)
        {
            Jinher.AMP.BTP.IBP.Facade.InvoiceFacade facade = new InvoiceFacade();
            var result = facade.GetInvoiceInfoList(search);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 发票列表
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public PartialViewResult PartialInvoiceList(Jinher.AMP.BTP.Deploy.CustomDTO.InvoiceSearchDTO search)
        {
            int pageIndex = 1;
            if (!string.IsNullOrWhiteSpace(Request.QueryString["currentPage"]))
            {
                pageIndex = int.Parse(Request.QueryString["currentPage"]);
            }
            search.PageIndex = pageIndex;
            Jinher.AMP.BTP.IBP.Facade.InvoiceFacade facade = new InvoiceFacade();
            var result = facade.GetInvoiceInfoList(search);
            ViewBag.InvoiceInfoList = result.Data;
            return PartialView();
        }
        /// <summary>
        /// 设置可用发票类型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult SetInvoiceCategory(Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO model)
        {
            Jinher.AMP.BTP.IBP.Facade.InvoiceFacade facade = new InvoiceFacade();
            var result = facade.SetInvoiceCategory(model);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 修改发票状态，或备注信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult UpdateInvoice(List<Jinher.AMP.BTP.Deploy.CustomDTO.InvoiceUpdateDTO> list)
        {
            Jinher.AMP.BTP.IBP.Facade.InvoiceFacade facade = new InvoiceFacade();
            var result = facade.UpdateInvoice(list);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VatInvoiceProofDetail(Guid userId)
        {
            Guid UserId = this.ContextDTO.LoginUserID;
            Jinher.AMP.BTP.IBP.Facade.InvoiceFacade invoiceFacade = new InvoiceFacade();
            var result = invoiceFacade.ShowVatInvoiceProof(userId);
            ViewBag.InvoiceInfoList = result.Data;
            return View();
        }

        /// <summary>
        /// 增票资质
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult VatInvoiceProof(Jinher.AMP.BTP.Deploy.CustomDTO.VatInvoiceProofInfoDTO vatInvoiceP)
        {
            Jinher.AMP.BTP.IBP.Facade.InvoiceFacade invoiceFacade = new InvoiceFacade();
            Guid UserId = this.ContextDTO.LoginUserID;
            vatInvoiceP.Id = UserId;
            var result = invoiceFacade.SaveVatInvoiceProof(vatInvoiceP);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 显示增值税发票信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult VatInvoiceProofDetailShow(Guid uid)
        {
            Jinher.AMP.BTP.IBP.Facade.InvoiceFacade invoiceFacade = new InvoiceFacade();
            var result = invoiceFacade.ShowVatInvoiceProof(uid);
            ViewBag.InvoiceInfoList = result.Data;
            return View();
        }

        /// <summary>
        /// 显示增值税发票信息 金采支付使用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult VatInvoiceProofDetailShowII(Guid jcActivityId)
        {
            Jinher.AMP.BTP.IBP.Facade.InvoiceFacade invoiceFacade = new InvoiceFacade();
            var result = invoiceFacade.ShowVatInvoiceProofII(jcActivityId);
            ViewBag.InvoiceInfoList = result.Data;
            return View();
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ExportData(Jinher.AMP.BTP.Deploy.CustomDTO.InvoiceExportDTO search)
        {
            Jinher.AMP.BTP.IBP.Facade.InvoiceFacade invoiceFacade = new InvoiceFacade();
            var result = invoiceFacade.GetInvoiceExport(search);
            return Json(new { data = result });
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <returns></returns>
        public FileResult DownExportData(string datas)
        {
            List<Jinher.AMP.BTP.Deploy.CustomDTO.InvoiceExportDTO> result = JsonHelper.JsonDeserialize<List<Jinher.AMP.BTP.Deploy.CustomDTO.InvoiceExportDTO>>(datas);
            StringBuilder sbHtml = new StringBuilder();
            sbHtml.Append("<meta http-equiv='Content-Type' content='text/html; charset=utf-8' />");
            sbHtml.Append("<table border='1' cellspacing='0' cellpadding='0'>");
            sbHtml.Append("<tr>");
            var lstTitle = new List<string> { "年份", "月份", "开票日期", "单位名称", "订单内容", "订单金额", "订单编号" };
            foreach (var item in lstTitle)
            {
                sbHtml.Append("<td style='font-size: 14px;text-align:center;background-color: #DCE0E2; font-weight:bold;' height='25'>" + item + "</td>");
            }
            sbHtml.Append("</tr>");
            if (result.Count() > 0)
            {
                foreach (var item in result)
                {
                    sbHtml.Append("<tr>");
                    sbHtml.Append("<td style='font-size: 12px;height:20px;'>" + item.Year.ToString() + "</td>");
                    sbHtml.Append("<td style='font-size: 12px;height:20px;'>" + item.Month.ToString() + "</td>");
                    sbHtml.Append("<td style='font-size: 12px;height:20px;'>" + item.SubTime.ToString("yyyy-MM-dd") + "</td>");
                    sbHtml.Append("<td style='font-size: 12px;height:20px;'>" + item.InvoiceTitle.ToString() + "</td>");
                    sbHtml.Append("<td style='font-size: 12px;height:20px;'>" + item.Content.ToString() + "</td>");
                    sbHtml.Append("<td style='font-size: 12px;height:20px;'>" + item.RealPrice.ToString() + "</td>");
                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Code.ToString() + "</td>");
                    sbHtml.Append("</tr>");
                }
            }
            sbHtml.Append("</table>");
            return File(System.Text.Encoding.UTF8.GetBytes(sbHtml.ToString()), "application/vnd.ms-excel", string.Format("发票信息_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }


        /// <summary>
        /// 导出电子发票详细
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ExportDataDetail(Jinher.AMP.BTP.Deploy.CustomDTO.InvoiceSearchDTO  search)
        {
            Jinher.AMP.BTP.IBP.Facade.InvoiceFacade invoiceFacade = new InvoiceFacade();
            var result = invoiceFacade.GetInvoiceExportDetail(search);
            return Json(new { data = result });
        }




        #region 导出电子发票副本 原先注释不能删

        //public FileResult DownExportDataDetail(string dataDetail)
        //{
        //    List<Jinher.AMP.BTP.Deploy.CustomDTO.ElectronicInvoiceDTO> result = JsonHelper.JsonDeserialize<List<Jinher.AMP.BTP.Deploy.CustomDTO.ElectronicInvoiceDTO>>(dataDetail);
        //    StringBuilder sbHtml = new StringBuilder();
        //    sbHtml.Append("<table border='1' cellspacing='0' cellpadding='0'>");
        //    sbHtml.Append("<tr>");
        //    var lstTitle = new List<string> { "发票流水号", "购货方邮箱", "购货方手机", "购货方名称", "购货方识别号", "购货方地址", "购货方固定电话", "购货方银行账号", "项目名称", "规格型号", "项目单位", "项目数量", "含税标志", "项目单价", "项目金额", "税率", "备注", "发票行性质", "商品编码", "自行编码", "优惠政策表示", "零税率标识", "增值税特殊管理" };
        //    foreach (var item in lstTitle)
        //    {
        //        sbHtml.Append("<td style='font-size: 14px;text-align:center;background-color: #DCE0E2; font-weight:bold;' height='25'>" + item + "</td>");
        //    }
        //    sbHtml.Append("</tr>");
        //    if (result.Count() > 0)
        //    {
        //        foreach (var item in result)
        //        {
        //            int count = item.SmallInvoice.Count();
        //            if (count == 1)
        //            {
        //                sbHtml.Append("<tr>");
        //                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Code + "</td>");
        //                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ReceiptEmail + "</td>");
        //                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ReceiptPhone + "</td>");
        //                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.InvoiceTitle + "</td>");
        //                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.BuyerCode + "</td>");
        //                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ReceiptAddress + "</td>");
        //                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.BuyerPhone + "</td>");
        //                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.BuyerBankNumber + "</td>");
        //                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.SmallInvoice[0].Name + "</td>");
        //                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Specifications + "</td>");
        //                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ProjectUnit + "</td>");
        //                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.SmallInvoice[0].Number + "</td>");
        //                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.TallageMark + "</td>");
        //                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.SmallInvoice[0].Price + "</td>");
        //                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + (item.SmallInvoice[0].Number * item.SmallInvoice[0].Price) + "</td>");
        //                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.TaxRate + "</td>");
        //                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Remark + "</td>");
        //                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.InvoicelineProperty + "</td>");
        //                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.SmallInvoice[0].TaxClassCode + "</td>");
        //                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.SmallInvoice[0].No_Code + "</td>");
        //                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.PolicyMark + "</td>");
        //                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ZeroTaxRateMark + "</td>");
        //                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.SpecialParticular + "</td>");
        //                sbHtml.Append("</tr>");

        //                if (item.Freight != 0)
        //                {
        //                    sbHtml.Append("<tr>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Code + "</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ReceiptEmail + "</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ReceiptPhone + "</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.InvoiceTitle + "</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.BuyerCode + "</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ReceiptAddress + "</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.BuyerPhone + "</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.BuyerBankNumber + "</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>派送服务</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Specifications + "</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ProjectUnit + "</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>1</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.TallageMark + "</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Freight + "</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Freight * 1 + "</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>0.06</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Remark + "</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.InvoicelineProperty + "</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>3040409030000000000</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>304040903</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.PolicyMark + "</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ZeroTaxRateMark + "</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.SpecialParticular + "</td>");
        //                    sbHtml.Append("</tr>");
        //                }

        //            }
        //            else
        //            {
        //                foreach (var _item in item.SmallInvoice)
        //                {
        //                    sbHtml.Append("<tr>");
        //                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Code + "</td>");
        //                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ReceiptEmail + "</td>");
        //                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ReceiptPhone + "</td>");
        //                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.InvoiceTitle + "</td>");
        //                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.BuyerCode + "</td>");
        //                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ReceiptAddress + "</td>");
        //                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.BuyerPhone + "</td>");
        //                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.BuyerBankNumber + "</td>");
        //                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + _item.Name + "</td>");
        //                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Specifications + "</td>");
        //                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ProjectUnit + "</td>");
        //                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + _item.Number + "</td>");
        //                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.TallageMark + "</td>");
        //                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + _item.Price + "</td>");
        //                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + (_item.Number * _item.Price) + "</td>");
        //                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.TaxRate + "</td>");
        //                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Remark + "</td>");
        //                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.InvoicelineProperty + "</td>");
        //                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + _item.TaxClassCode + "</td>");
        //                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + _item.No_Code + "</td>");
        //                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.PolicyMark + "</td>");
        //                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ZeroTaxRateMark + "</td>");
        //                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.SpecialParticular + "</td>");
        //                    sbHtml.Append("</tr>");
        //                }
        //                if (item.Freight != 0)
        //                {
        //                    sbHtml.Append("<tr>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Code + "</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ReceiptEmail + "</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ReceiptPhone + "</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.InvoiceTitle + "</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.BuyerCode + "</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ReceiptAddress + "</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.BuyerPhone + "</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.BuyerBankNumber + "</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>派送服务</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Specifications + "</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ProjectUnit + "</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>1</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.TallageMark + "</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Freight + "</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Freight * 1 + "</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>0.06</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Remark + "</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.InvoicelineProperty + "</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>3040409030000000000</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>304040903</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.PolicyMark + "</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ZeroTaxRateMark + "</td>");
        //                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.SpecialParticular + "</td>");
        //                    sbHtml.Append("</tr>");
        //                }

        //            }
        //        }
        //    }
        //    sbHtml.Append("</table>");
        //    return File(System.Text.Encoding.UTF8.GetBytes(sbHtml.ToString()), "application/vnd.ms-excel", string.Format("电子发票信息_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
        //}


        #endregion

        /// <summary>
        /// 导出电子发票详细
        /// </summary>
        /// <returns></returns>
        public FileResult DownExportDataDetail(string dataDetail)
        {
            StringBuilder sbHtml = null;
            try
            {
                sbHtml = new StringBuilder();
                Dictionary<Guid, string> dic = new Dictionary<Guid, string>();
                string Appids = CustomConfig.TdAppIds;
                if (!string.IsNullOrEmpty(Appids))
                {
                    JArray arr = (JArray)JsonConvert.DeserializeObject(Appids);
                    foreach (var item in arr)
                    {
                        dic.Add(Guid.Parse(item["Key"].ToString()), item["Value"].ToString());
                    }
                }
                List<Jinher.AMP.BTP.Deploy.CustomDTO.ElectronicInvoiceDTO> result = JsonHelper.JsonDeserialize<List<Jinher.AMP.BTP.Deploy.CustomDTO.ElectronicInvoiceDTO>>(dataDetail);
                sbHtml.Append("<table border='1' cellspacing='0' cellpadding='0'>");
                sbHtml.Append("<tr>");
                List<string> lstTitle = null;
                lstTitle = new List<string> { "发票流水号", "购货方邮箱", "购货方手机", "购货方名称", "购货方识别号", "购货方地址", "购货方固定电话", "购货方银行账号", "项目名称", "规格型号", "项目单位", "项目数量", "含税标志", "项目单价", "项目金额", "税率", "备注", "发票行性质", "商品编码", "自行编码", "优惠政策表示", "零税率标识", "增值税特殊管理" };
                foreach (var item in lstTitle)
                {
                    sbHtml.Append("<td style='font-size: 14px;text-align:center;background-color: #DCE0E2; font-weight:bold;' height='25'>" + item + "</td>");
                }
                sbHtml.Append("</tr>");
                if (result.Count() > 0)
                {
                    foreach (var item in result)
                    {
                        int count = item.SmallInvoice.Count();
                        if (count == 1)
                        {
                            if (dic.ContainsKey(item.AppId))
                            {
                                sbHtml.Append("<tr>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Code + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ReceiptEmail + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ReceiptPhone + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.InvoiceTitle + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.BuyerCode + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ReceiptAddress + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.BuyerPhone + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.BuyerBankNumber + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.SmallInvoice[0].Name + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Specifications + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ProjectUnit + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.SmallInvoice[0].Number + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.TallageMark + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.SmallInvoice[0].CostPrice + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + (item.SmallInvoice[0].CostPrice * item.SmallInvoice[0].Number) + "</td>");
                                if (item.SmallInvoice[0].CostPrice == 0)
                                {
                                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.SmallInvoice[0].TaxRate + "</td>");
                                }
                                else
                                {
                                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.TaxRate + "</td>");
                                }
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Remark + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.InvoicelineProperty + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.SmallInvoice[0].TaxClassCode + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.SmallInvoice[0].No_Code + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.PolicyMark + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ZeroTaxRateMark + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.SpecialParticular + "</td>");
                                sbHtml.Append("</tr>");
                            }
                            else
                            {
                                sbHtml.Append("<tr>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Code + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ReceiptEmail + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ReceiptPhone + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.InvoiceTitle + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.BuyerCode + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ReceiptAddress + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.BuyerPhone + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.BuyerBankNumber + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.SmallInvoice[0].Name + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Specifications + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ProjectUnit + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.SmallInvoice[0].Number + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.TallageMark + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + ((item.RealPrice - item.Freight) / item.SmallInvoice[0].Number) + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + (item.RealPrice - item.Freight) + "</td>");
                                if (item.SmallInvoice[0].CostPrice == 0)
                                {
                                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.SmallInvoice[0].TaxRate + "</td>");
                                }
                                else
                                {
                                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.TaxRate + "</td>");
                                }
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Remark + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.InvoicelineProperty + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.SmallInvoice[0].TaxClassCode + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.SmallInvoice[0].No_Code + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.PolicyMark + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ZeroTaxRateMark + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.SpecialParticular + "</td>");
                                sbHtml.Append("</tr>");
                            }
                            if (item.Freight != 0)
                            {
                                sbHtml.Append("<tr>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Code + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ReceiptEmail + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ReceiptPhone + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.InvoiceTitle + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.BuyerCode + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ReceiptAddress + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.BuyerPhone + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.BuyerBankNumber + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>派送服务</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Specifications + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ProjectUnit + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>1</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.TallageMark + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Freight + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Freight * 1 + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>0.06</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Remark + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.InvoicelineProperty + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>3040409030000000000</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>304040903</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.PolicyMark + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ZeroTaxRateMark + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.SpecialParticular + "</td>");
                                sbHtml.Append("</tr>");
                            }
                            if (dic.ContainsKey(item.AppId))
                            {
                                sbHtml.Append("<tr>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Code + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ReceiptEmail + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ReceiptPhone + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.InvoiceTitle + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.BuyerCode + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ReceiptAddress + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.BuyerPhone + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.BuyerBankNumber + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>通道费用</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Specifications + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ProjectUnit + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>1</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.TallageMark + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + (item.SmallInvoice[0].Price - item.SmallInvoice[0].CostPrice) + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + ((item.SmallInvoice[0].Price - item.SmallInvoice[0].CostPrice) * item.SmallInvoice[0].Number) + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>0.06</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Remark + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.InvoicelineProperty + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>3040409030000000000</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>304040903</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.PolicyMark + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ZeroTaxRateMark + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.SpecialParticular + "</td>");
                                sbHtml.Append("</tr>");
                            }
                        }
                        else
                        {
                            decimal? psprice = 0;
                            decimal? sum = 0;
                            decimal? Totalsum = 0;
                            if (dic.ContainsKey(item.AppId))
                            {
                                foreach (var _item in item.SmallInvoice)
                                {
                                    sbHtml.Append("<tr>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Code + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ReceiptEmail + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ReceiptPhone + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.InvoiceTitle + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.BuyerCode + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ReceiptAddress + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.BuyerPhone + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.BuyerBankNumber + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + _item.Name + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Specifications + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ProjectUnit + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + _item.Number + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.TallageMark + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + _item.CostPrice + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + (_item.Number * _item.CostPrice) + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.TaxRate + "</td>");
                                    if (_item.CostPrice == 0)
                                    {
                                        sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + _item.TaxRate + "</td>");
                                    }
                                    else
                                    {
                                        sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.TaxRate + "</td>");
                                    }
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Remark + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.InvoicelineProperty + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + _item.TaxClassCode + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + _item.No_Code + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.PolicyMark + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ZeroTaxRateMark + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.SpecialParticular + "</td>");
                                    sbHtml.Append("</tr>");
                                    psprice += (_item.Price - _item.CostPrice);
                                    sum += psprice;
                                    Totalsum += (psprice * _item.Number);
                                }
                            }
                            else
                            {
                                foreach (var _item in item.SmallInvoice)
                                {
                                    sbHtml.Append("<tr>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Code + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ReceiptEmail + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ReceiptPhone + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.InvoiceTitle + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.BuyerCode + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ReceiptAddress + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.BuyerPhone + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.BuyerBankNumber + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + _item.Name + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Specifications + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ProjectUnit + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + _item.Number + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.TallageMark + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + _item.Price + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + (_item.Number * _item.Price) + "</td>");
                                    if (_item.CostPrice == 0)
                                    {
                                        sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + _item.TaxRate + "</td>");
                                    }
                                    else
                                    {
                                        sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.TaxRate + "</td>");
                                    }
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Remark + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.InvoicelineProperty + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + _item.TaxClassCode + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + _item.No_Code + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.PolicyMark + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ZeroTaxRateMark + "</td>");
                                    sbHtml.Append("<td  style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.SpecialParticular + "</td>");
                                    sbHtml.Append("</tr>");
                                }
                            }

                            if (item.Freight != 0)
                            {
                                sbHtml.Append("<tr>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Code + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ReceiptEmail + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ReceiptPhone + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.InvoiceTitle + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.BuyerCode + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ReceiptAddress + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.BuyerPhone + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.BuyerBankNumber + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>派送服务</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Specifications + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ProjectUnit + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>1</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.TallageMark + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Freight + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Freight * 1 + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>0.06</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Remark + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.InvoicelineProperty + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>3040409030000000000</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>304040903</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.PolicyMark + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ZeroTaxRateMark + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.SpecialParticular + "</td>");
                                sbHtml.Append("</tr>");
                            }

                            if (dic.ContainsKey(item.AppId))
                            {
                                sbHtml.Append("<tr>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Code + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ReceiptEmail + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ReceiptPhone + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.InvoiceTitle + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.BuyerCode + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ReceiptAddress + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.BuyerPhone + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.BuyerBankNumber + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>通道费用</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Specifications + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ProjectUnit + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>1</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.TallageMark + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + sum + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + Totalsum + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>0.06</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Remark + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.InvoicelineProperty + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>3040409030000000000</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>304040903</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.PolicyMark + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ZeroTaxRateMark + "</td>");
                                sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.SpecialParticular + "</td>");
                                sbHtml.Append("</tr>");
                            }
                        }
                    }
                }
                sbHtml.Append("</table>");
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("DownExportDataDetail异常信息{0}", ex.Message), ex);
            }
            return File(System.Text.Encoding.UTF8.GetBytes(sbHtml.ToString()), "application/vnd.ms-excel", string.Format("电子发票信息_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }









    }
}
