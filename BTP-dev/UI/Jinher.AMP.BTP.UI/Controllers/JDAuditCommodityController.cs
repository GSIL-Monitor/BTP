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
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.TPS.Helper;


namespace Jinher.AMP.BTP.UI.Controllers
{
    public partial class JDAuditCommodityController : BaseController
    {
        #region  页面
        /// <summary>
        /// 京东审核页面
        /// </summary>
        /// <returns></returns>
        [CheckAppId]
        public ActionResult Index(Guid appId)
        {
            ViewBag.appId = appId;
            bool IsShow = false;
            if (ThirdECommerceHelper.IsJingDongDaKeHu(appId) || ThirdECommerceHelper.IsSuNingYiGou(appId))
            {
                IsShow = true;
            }
            ViewBag.IsShow = IsShow;
            bool IsYpk = false;
            if (ThirdECommerceHelper.IsYiPaiKe(appId))
            {
                IsYpk = true;
            }
            ViewBag.IsYpk = IsYpk;
            return View();
        }
        /// <summary>
        /// 京东现价审核页面
        /// </summary>
        /// <returns></returns>
        [CheckAppId]
        public ActionResult JDEditPriceIndex(Guid appId)
        {
            JDAuditComFacade JDAudit = new JDAuditComFacade();
            ViewBag.AuditPriceState = JDAudit.GetAuditMode(appId).PriceModeState;//获取京东现价审核方式
            ViewBag.appId = appId;
            return View();
        }
        /// <summary>
        /// 京东进货价审核页面
        /// </summary>
        /// <returns></returns>
        [CheckAppId]
        public ActionResult JDEditCostPriceIndex(Guid appId)
        {
            JDAuditComFacade JDAudit = new JDAuditComFacade();
            ViewBag.AuditCostState = JDAudit.GetAuditMode(appId).CostModeState;//获取京东进货价审核方式
            ViewBag.appId = appId;
            return View();
        }
        /// <summary>
        /// 京东下架或者售罄审核页面
        /// </summary>
        /// <returns></returns>
        [CheckAppId]
        public ActionResult JDAuditOffSaleIndex(Guid appId)
        {
            JDAuditComFacade JDAudit = new JDAuditComFacade();
            ViewBag.StockModeState = JDAudit.GetAuditMode(appId).StockModeState;//获取京东下架或者售罄审核方式
            ViewBag.appId = appId;
            return View();
        }
        #endregion
        /// <summary>
        /// 京东现价审核列表数据
        /// </summary>
        /// <returns></returns>        
        [GridAction]
        public ActionResult GetJDEditPriceList(string appId, string Name, string JdCode, int AuditState, decimal MinRate, decimal MaxRate, string EditStartime, string EditEndTime)
        {
            try
            {
                Guid AppId = new Guid(appId);
                JDAuditComFacade JDAudit = new JDAuditComFacade();
                int pageIndex = 1;
                int pageSize = 100;
                if (!string.IsNullOrEmpty(Request["page"]))
                {
                    pageIndex = int.Parse(Request["page"]);
                }
                //获取数据
                ResultDTO<List<CommodityAndCategoryDTO>> retInfo = JDAudit.GetEditPriceList(AppId, Name, JdCode, AuditState, MinRate, MaxRate, EditStartime, EditEndTime, Convert.ToInt32(OperateTypeEnum.京东修改现价), pageIndex, pageSize);
                List<string> JDEditPriceList = new List<string>
                {
                    "AuditId",
                    "SupplyName",
                    "AppName",
                    "PicturesPath",
                    "JDCode",
                    "ComAttribute",
                    "NewCostPrice",
                    "Price",
                    "JdPrice",
                    "NowPriceProfit",
                    "NewPriceProfit",
                    "ApplyTime",
                    "AuditState",
                    "AuditRemark",
                    "AuditUserName",
                    "AuditUserCode",
                    "AuditTime",
                    "Name"
                };
                int rowCount = retInfo != null ? retInfo.ResultCode : 0;
                var list = retInfo != null ? retInfo.Data : new List<CommodityAndCategoryDTO>();
                return View(new GridModel<CommodityAndCategoryDTO>(JDEditPriceList, list, rowCount, pageIndex, pageSize, string.Empty));
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("京东现价审核异常JDEditPriceIndex,"), ex);
                return null;
            }
        }

        /// <summary>
        /// 京东进货价审核页面
        /// </summary>
        /// <returns></returns>
        [CheckAppId]
        [GridAction]
        public ActionResult GetJDEditCostPriceList(string appId, string Name, string JdCode, int AuditState, decimal MinRate, decimal MaxRate, string EditStartime, string EditEndTime)
        {
            try
            {
                Guid AppId = new Guid(appId);
                JDAuditComFacade JDAudit = new JDAuditComFacade();
                int pageIndex = 1;
                int pageSize = 100;
                if (!string.IsNullOrEmpty(Request["page"]))
                {
                    pageIndex = int.Parse(Request["page"]);
                }
                //获取数据
                ResultDTO<List<CommodityAndCategoryDTO>> retInfo = JDAudit.GetEditPriceList(AppId, Name, JdCode, AuditState, MinRate, MaxRate, EditStartime, EditEndTime, Convert.ToInt32(OperateTypeEnum.京东修改进货价), pageIndex, pageSize);

                List<string> JDEditCostPriceList = new List<string>
                {
                    "AuditId",
                    "SupplyName",
                    "AppName",
                    "PicturesPath",
                    "JDCode",
                    "ComAttribute",
                    "NewPrice",
                    "CostPrice",
                    "JdCostPrice",
                    "JdPrice",
                    "NowCostPriceProfit",
                    "NewCostPriceProfit",
                    "ApplyTime",
                    "AuditState",
                    "AuditRemark",
                    "AuditUserName",
                    "AuditUserCode",
                    "AuditTime",
                    "Name"
                };
                int rowCount = retInfo != null ? retInfo.ResultCode : 0;
                var list = retInfo != null ? retInfo.Data : new List<CommodityAndCategoryDTO>();
                return View(new GridModel<CommodityAndCategoryDTO>(JDEditCostPriceList, list, rowCount, pageIndex, pageSize, string.Empty));
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("京东现价审核异常JDEditPriceIndex,"), ex);
                return null;
            }
        }
        /// <summary>
        /// 京东下架售罄审核列表数据
        /// </summary>
        /// <returns></returns>
        [CheckAppId]
        [GridAction]
        public ActionResult GetJDEditShelfAndSTOCKList(string appId, string Name, string JdCode, int AuditState, int JdStatus, string EditStartime, string EditEndTime)
        {
            try
            {
                Guid AppId = new Guid(appId);
                JDAuditComFacade JDAudit = new JDAuditComFacade();
                int pageIndex = 1;
                int pageSize = 100;
                if (!string.IsNullOrEmpty(Request["page"]))
                {
                    pageIndex = int.Parse(Request["page"]);
                }
                //获取数据
                ResultDTO<List<CommodityAndCategoryDTO>> retInfo = JDAudit.GetOffSaleAndNoStockList(AppId, Name, JdCode, AuditState, JdStatus, EditStartime, EditEndTime, Convert.ToInt32(OperateTypeEnum.下架无货商品审核), pageIndex, pageSize);
                List<string> JDEditList = new List<string>
                {
                    "AuditId",
                    "SupplyName",
                    "AppName",
                    "PicturesPath",
                    "JDCode",
                    "ComAttribute",
                    "CostPrice",
                    "Price",
                    "Stock",
                    "JdStatusName",
                    "AuditStateName",
                    "AuditUserName",
                    "AuditUserCode",
                    "AuditTime",
                    "Name",
                    "JdStatus",
                    "AuditState"
                };
                int rowCount = retInfo != null ? retInfo.ResultCode : 0;
                var list = retInfo != null ? retInfo.Data : new List<CommodityAndCategoryDTO>();
                return View(new GridModel<CommodityAndCategoryDTO>(JDEditList, list, rowCount, pageIndex, pageSize, string.Empty));
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("京东下架售罄审核列表数据GetJDEditShelfAndSTOCKList,"), ex);
                return null;
            }
        }
        /// <summary>
        /// 设置京东现价审核方式
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetPriceAuditMode(Guid AppId, int ModeStatus)
        {
            try
            {
                JDAuditComFacade JDAudit = new JDAuditComFacade();
                //调用批量审核接口
                ResultDTO result = JDAudit.SetEditPriceMode(AppId, ModeStatus);
                if (result.ResultCode == 0)
                {
                    return Json(new { Result = true, code = 0, Messages = "设置成功" });
                }
                else
                {
                    return Json(new { Result = false, code = 1, Messages = "设置失败" });
                }

            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format(" 京东现价设置审核方式SetPriceAuditMode,AppId{0},ModeStatus{1}", AppId, ModeStatus), ex);
                return Json(new { Success = false, Messages = ex.Message });
            }
        }

        /// <summary>
        /// 设置京东现价审核方式
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetCostAuditMode(Guid AppId, int ModeStatus)
        {
            try
            {
                JDAuditComFacade JDAudit = new JDAuditComFacade();
                //调用批量审核接口
                ResultDTO result = JDAudit.SetEditCostPriceMode(AppId, ModeStatus);
                if (result.ResultCode == 0)
                {
                    return Json(new { Result = true, code = 0, Messages = "设置成功" });
                }
                else
                {
                    return Json(new { Result = false, code = 1, Messages = "设置失败" });
                }

            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format(" 京东进货价设置审核方式SetCostAuditMode,AppId{0},ModeStatus{1}", AppId, ModeStatus), ex);
                return Json(new { Success = false, Messages = ex.Message });
            }
        }
        /// <summary>
        /// 设置下架无货商品审核方式
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetOffAndNoStockMode(Guid AppId, int ModeStatus)
        {
            try
            {
                JDAuditComFacade JDAudit = new JDAuditComFacade();
                //调用批量审核接口
                ResultDTO result = JDAudit.SetOffAndNoStockMode(AppId, ModeStatus);
                if (result.ResultCode == 0)
                {
                    return Json(new { Result = true, code = 0, Messages = "设置成功" });
                }
                else
                {
                    return Json(new { Result = false, code = 1, Messages = "设置失败" });
                }
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("设置下架无货商品审核方式SetOffAndNoStockMode,AppId{0},ModeStatus{1}", AppId, ModeStatus), ex);
                return Json(new { Success = false, Messages = ex.Message });
            }
        }
        /// <summary>
        /// 手动审核京东现价
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AuditJDEditPrice()
        {
            try
            {
                string idlist = Request["Ids"] ?? "";
                if (string.IsNullOrEmpty(idlist))
                {
                    return null;
                }
                JArray objson = JArray.Parse(idlist);
                List<Guid> ids = new List<Guid>();
                foreach (var item in objson)
                {
                    Guid id = Guid.Parse(item["Id"].ToString());
                    ids.Add(id);
                }
                int State = Convert.ToInt32(Request["State"]);
                string SetpPrice = Request["Price"] ?? "0";
                string SetPrice = Request["Price"] == "" ? "0" : Request["Price"];
                decimal Price = Convert.ToDecimal(SetPrice);
                string AuditRemark = Request["AuditRemark"];
                if (State == 2 && string.IsNullOrEmpty(AuditRemark))
                {
                    return Json(new { Result = false, code = 1, Messages = "审核不通过时审核意见必填" });
                }
                JDAuditComFacade JDAudit = new JDAuditComFacade();
                //调用批量审核接口
                ResultDTO result = JDAudit.AuditJDPrice(ids, State, Price, AuditRemark, 1);
                if (result.ResultCode == 0)
                {
                    return Json(new { Result = true, code = 0, Messages = "审核成功" });
                }
                else
                {
                    return Json(new { Result = false, code = 1, Messages = "审核失败" });
                }

            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("京东现价审核异常AuditJDEditPrice"), ex);
                return Json(new { Success = false, Messages = ex.Message });
            }
        }
        /// <summary>
        /// 手动审核京东进价
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AuditJDEditCostPrice()
        {
            try
            {
                string idlist = Request["Ids"] ?? "";
                if (string.IsNullOrEmpty(idlist))
                {
                    return null;
                }
                JArray objson = JArray.Parse(idlist);
                List<Guid> ids = new List<Guid>();
                foreach (var item in objson)
                {
                    Guid id = Guid.Parse(item["Id"].ToString());
                    ids.Add(id);
                }
                int State = Convert.ToInt32(Request["State"]);
                int Dispose = Convert.ToInt32(Request["Dispose"]);
                string AuditRemark = Request["AuditRemark"];
                if (State == 2 && string.IsNullOrEmpty(AuditRemark))
                {
                    return Json(new { Result = false, code = 1, Messages = "审核不通过时审核意见必填" });
                }
                JDAuditComFacade JDAudit = new JDAuditComFacade();
                //调用批量审核接口
                ResultDTO result = JDAudit.AuditJDCostPrice(ids, State, AuditRemark, Dispose, 1);
                if (result.ResultCode == 0)
                {
                    return Json(new { Result = true, code = 0, Messages = "审核成功" });
                }
                else
                {
                    return Json(new { Result = false, code = 1, Messages = "审核失败" });
                }

            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("京东现价审核异常AuditJDEditPrice"), ex);
                return Json(new { Success = false, Messages = ex.Message });
            }
        }
        /// <summary>
        ///置为上架
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AuditJDEditOnShelf()
        {
            try
            {
                string idlist = Request["Ids"] ?? "";
                if (string.IsNullOrEmpty(idlist))
                {
                    return null;
                }
                JArray objson = JArray.Parse(idlist);
                List<Guid> ids = new List<Guid>();
                foreach (var item in objson)
                {
                    Guid id = Guid.Parse(item["Id"].ToString());
                    ids.Add(id);
                }

                JDAuditComFacade JDAudit = new JDAuditComFacade();

                ResultDTO result = JDAudit.SetPutaway(ids, 1);
                if (result.ResultCode == 0)
                {
                    return Json(new { Result = true, code = 0, Messages = "上架成功" });
                }
                else
                {
                    return Json(new { Result = false, code = 1, Messages = "上架失败" });
                }

            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("置为上架异常"), ex);
                return Json(new { Success = false, Messages = ex.Message });
            }
        }
        /// <summary>
        ///置为下架
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AuditJDEditOffShelf()
        {
            try
            {
                string idlist = Request["Ids"] ?? "";
                if (string.IsNullOrEmpty(idlist))
                {
                    return null;
                }
                JArray objson = JArray.Parse(idlist);
                List<Guid> ids = new List<Guid>();
                foreach (var item in objson)
                {
                    Guid id = Guid.Parse(item["Id"].ToString());
                    ids.Add(id);
                }

                JDAuditComFacade JDAudit = new JDAuditComFacade();

                ResultDTO result = JDAudit.SetOffShelf(ids, 1);
                if (result.ResultCode == 0)
                {
                    return Json(new { Result = true, code = 0, Messages = "下架成功" });
                }
                else
                {
                    return Json(new { Result = false, code = 1, Messages = "下架失败" });
                }

            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("置为下架异常"), ex);
                return Json(new { Success = false, Messages = ex.Message });
            }
        }
        /// <summary>
        ///置为售罄
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AuditJDEditNoStock()
        {
            try
            {
                string idlist = Request["Ids"] ?? "";
                if (string.IsNullOrEmpty(idlist))
                {
                    return null;
                }
                JArray objson = JArray.Parse(idlist);
                List<Guid> ids = new List<Guid>();
                foreach (var item in objson)
                {
                    Guid id = Guid.Parse(item["Id"].ToString());
                    ids.Add(id);
                }

                JDAuditComFacade JDAudit = new JDAuditComFacade();

                ResultDTO result = JDAudit.SetNoStock(ids, 1);
                if (result.ResultCode == 0)
                {
                    return Json(new { Result = true, code = 0, Messages = "置为售罄成功" });
                }
                else
                {
                    return Json(new { Result = false, code = 1, Messages = "置为售罄失败" });
                }

            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("置为售罄异常"), ex);
                return Json(new { Success = false, Messages = ex.Message });
            }
        }
        /// <summary>
        ///置为有货
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AuditJDEditInStock()
        {
            try
            {
                string idlist = Request["Ids"] ?? "";
                if (string.IsNullOrEmpty(idlist))
                {
                    return null;
                }
                JArray objson = JArray.Parse(idlist);
                List<Guid> ids = new List<Guid>();
                foreach (var item in objson)
                {
                    Guid id = Guid.Parse(item["Id"].ToString());
                    ids.Add(id);
                }

                JDAuditComFacade JDAudit = new JDAuditComFacade();

                ResultDTO result = JDAudit.SetInStore(ids, 1);
                if (result.ResultCode == 0)
                {
                    return Json(new { Result = true, code = 0, Messages = "置为有货成功" });
                }
                else
                {
                    return Json(new { Result = false, code = 1, Messages = "置为有货失败" });
                }

            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("置为有货异常"), ex);
                return Json(new { Success = false, Messages = ex.Message });
            }
        }
        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ExportData(string appId, string Name, string JdCode, int AuditState, decimal MinRate, decimal MaxRate, string EditStartime, string EditEndTime, int Action)
        {
            Guid AppId = new Guid(appId);
            JDAuditComFacade JDAudit = new JDAuditComFacade();
            //获取数据
            ResultDTO<List<CommodityAndCategoryDTO>> retInfo = JDAudit.ExportPriceList(AppId, Name, JdCode, AuditState, MinRate, MaxRate, EditStartime, EditEndTime, Action);
            return Json(new { data = retInfo.ResultCode });
        }
        /// <summary>
        /// 京东售价审核导出(excel)
        /// </summary>
        /// <returns></returns>
        public FileResult ExportExcelData(string hidappId, string hidName, string hidJdCode, int hidAuditState, decimal hidMinRate, decimal hidMaxRate, string hidEditStartime, string hidEditEndTime, int hidAction)
        {
            Guid AppId = new Guid(hidappId);
            JDAuditComFacade JDAudit = new JDAuditComFacade();
            //获取数据
            ResultDTO<List<CommodityAndCategoryDTO>> retInfo = JDAudit.ExportPriceList(AppId, hidName, hidJdCode, hidAuditState, hidMinRate, hidMaxRate, hidEditStartime, hidEditEndTime, hidAction);

            StringBuilder sbHtml = new StringBuilder();
            sbHtml.Append("<meta http-equiv='Content-Type' content='text/html; charset=utf-8' />");
            sbHtml.Append("<table border='1' cellspacing='0' cellpadding='0'>");
            sbHtml.Append("<tr>");
            if (Convert.ToInt32(hidAction) == 9)
            {
                sbHtml.Append("<tr>");
                List<string> lstTitle = new List<string>();
                lstTitle = new List<string> { "供应商名称", "APP名称", "商品备注编码", "商品名称", "商品属性", "当前进价", "当前售价", "最新售价", "当前售价毛利率", "最新售价毛利率", "修改时间", "状态", "审核意见", "审核人昵称", "审核人用户名", "审核时间" };
                foreach (var item in lstTitle)
                {
                    sbHtml.Append("<td style='font-size: 14px;text-align:center;background-color: #DCE0E2; font-weight:bold;' height='25'>" + item + "</td>");
                }
                sbHtml.Append("</tr>");
                if (retInfo.ResultCode > 0)
                {
                    foreach (var item in retInfo.Data)
                    {
                        sbHtml.Append("<tr>");
                        sbHtml.Append("<td>" + item.SupplyName + "</td>");
                        sbHtml.Append("<td>" + item.AppName + "</td>");
                        sbHtml.Append("<td>" + item.JDCode + "</td>");
                        sbHtml.Append("<td>" + item.Name + "</td>");
                        sbHtml.Append("<td>" + item.ComAttribute + "</td>");
                        sbHtml.Append("<td>" + item.NewCostPrice + "</td>");
                        sbHtml.Append("<td>" + item.Price + "</td>");
                        sbHtml.Append("<td>" + item.JdPrice + "</td>");
                        sbHtml.Append("<td>" + item.NowPriceProfit.ToString() + "%" + "</td>");
                        sbHtml.Append("<td>" + item.NewPriceProfit.ToString() + "%" + "</td>");
                        sbHtml.Append("<td>" + item.ApplyTime + "</td>");
                        sbHtml.Append("<td>" + item.AuditStateName + "</td>");
                        sbHtml.Append("<td>" + item.AuditRemark + "</td>");
                        sbHtml.Append("<td>" + item.AuditUserName + "</td>");
                        sbHtml.Append("<td>" + item.AuditUserCode + "</td>");
                        sbHtml.Append("<td>" + item.AuditTime + "</td>");
                        sbHtml.Append("</tr>");
                    }
                }
            }
            if (Convert.ToInt32(hidAction) == 10)
            {
                sbHtml.Append("<tr>");
                List<string> lstTitle = new List<string>();
                lstTitle = new List<string> { "供应商名称", "APP名称", "商品备注编码", "商品名称", "商品属性", "当前售价", "当前进价", "最新进价", "京东售价", "当前进价毛利率", "最新进价毛利率", "修改时间", "状态", "审核意见", "审核人昵称", "审核人用户名", "审核时间" };
                foreach (var item in lstTitle)
                {
                    sbHtml.Append("<td style='font-size: 14px;text-align:center;background-color: #DCE0E2; font-weight:bold;' height='25'>" + item + "</td>");
                }
                sbHtml.Append("</tr>");
                if (retInfo.ResultCode > 0)
                {
                    foreach (var item in retInfo.Data)
                    {
                        sbHtml.Append("<tr>");
                        sbHtml.Append("<td>" + item.SupplyName + "</td>");
                        sbHtml.Append("<td>" + item.AppName + "</td>");
                        sbHtml.Append("<td>" + item.JDCode + "</td>");
                        sbHtml.Append("<td>" + item.Name + "</td>");
                        sbHtml.Append("<td>" + item.ComAttribute + "</td>");
                        sbHtml.Append("<td>" + item.Price + "</td>");
                        sbHtml.Append("<td>" + item.CostPrice + "</td>");
                        sbHtml.Append("<td>" + item.JdCostPrice + "</td>");
                        sbHtml.Append("<td>" + item.JdPrice + "</td>");
                        sbHtml.Append("<td>" + item.NowCostPriceProfit.ToString() + "%" + "</td>");
                        sbHtml.Append("<td>" + item.NewCostPriceProfit.ToString() + "%" + "</td>");
                        sbHtml.Append("<td>" + item.ApplyTime + "</td>");
                        sbHtml.Append("<td>" + item.AuditStateName + "</td>");
                        sbHtml.Append("<td>" + item.AuditRemark + "</td>");
                        sbHtml.Append("<td>" + item.AuditUserName + "</td>");
                        sbHtml.Append("<td>" + item.AuditUserCode + "</td>");
                        sbHtml.Append("<td>" + item.AuditTime + "</td>");
                        sbHtml.Append("</tr>");
                    }
                }
            }

            sbHtml.Append("</table>");
            return File(System.Text.Encoding.UTF8.GetBytes(sbHtml.ToString()), "application/vnd.ms-excel", string.Format("京东审核报表_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }
        [HttpPost]
        public ActionResult ExportOffAndNoStockData(string appId, string Name, string JdCode, int AuditState, int JdStatus, string EditStartime, string EditEndTime)
        {
            Guid AppId = new Guid(appId);
            JDAuditComFacade JDAudit = new JDAuditComFacade();
            //获取数据
            ResultDTO<List<CommodityAndCategoryDTO>> retInfo = JDAudit.ExportOffSaleAndNoStockData(AppId, Name, JdCode, AuditState, JdStatus, EditStartime, EditEndTime, Convert.ToInt32(OperateTypeEnum.下架无货商品审核));
            return Json(new { data = retInfo.ResultCode });
        }
        /// <summary>
        /// 京东下架无货商品审核导出(excel)
        /// </summary>
        /// <returns></returns>
        public FileResult ExportOffAndNoStockExcelData(string hidappId, string hidName, string hidJdCode, string hidAuditState, string hidJdStatus, string hidEditStartime, string hidEditEndTime)
        {
            Guid AppId = new Guid(hidappId);
            JDAuditComFacade JDAudit = new JDAuditComFacade();
            //获取数据
            ResultDTO<List<CommodityAndCategoryDTO>> retInfo = JDAudit.ExportOffSaleAndNoStockData(AppId, hidName, hidJdCode, Convert.ToInt32(hidAuditState), Convert.ToInt32(hidJdStatus), hidEditStartime, hidEditEndTime, Convert.ToInt32(OperateTypeEnum.下架无货商品审核));
            StringBuilder sbHtml = new StringBuilder();
            sbHtml.Append("<meta http-equiv='Content-Type' content='text/html; charset=utf-8' />");
            sbHtml.Append("<table border='1' cellspacing='0' cellpadding='0'>");
            sbHtml.Append("<tr>");

            sbHtml.Append("<tr>");
            List<string> lstTitle = new List<string>();
            lstTitle = new List<string> { "供应商名称", "APP名称", "商品备注编码", "商品名称", "商品属性", "当前进价", "当前售价", "当前库存", "商品状态", "处理方式", "审核人昵称", "审核人用户名", "审核时间" };
            foreach (var item in lstTitle)
            {
                sbHtml.Append("<td style='font-size: 14px;text-align:center;background-color: #DCE0E2; font-weight:bold;' height='25'>" + item + "</td>");
            }
            sbHtml.Append("</tr>");
            if (retInfo.ResultCode > 0)
            {
                foreach (var item in retInfo.Data)
                {
                    sbHtml.Append("<tr>");
                    sbHtml.Append("<td>" + item.SupplyName + "</td>");
                    sbHtml.Append("<td>" + item.AppName + "</td>");
                    sbHtml.Append("<td>" + item.JDCode + "</td>");
                    sbHtml.Append("<td>" + item.Name + "</td>");
                    sbHtml.Append("<td>" + item.ComAttribute + "</td>");
                    sbHtml.Append("<td>" + item.CostPrice + "</td>");
                    sbHtml.Append("<td>" + item.Price + "</td>");
                    sbHtml.Append("<td>" + item.Stock + "</td>");
                    sbHtml.Append("<td>" + item.JdStatusName + "</td>");
                    sbHtml.Append("<td>" + item.AuditStateName + "</td>");
                    sbHtml.Append("<td>" + item.AuditUserName + "</td>");
                    sbHtml.Append("<td>" + item.AuditUserCode + "</td>");
                    sbHtml.Append("<td>" + item.AuditTime + "</td>");
                    sbHtml.Append("</tr>");
                }
            }
            sbHtml.Append("</table>");
            return File(System.Text.Encoding.UTF8.GetBytes(sbHtml.ToString()), "application/vnd.ms-excel", string.Format("京东审核报表_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }
    }
}
