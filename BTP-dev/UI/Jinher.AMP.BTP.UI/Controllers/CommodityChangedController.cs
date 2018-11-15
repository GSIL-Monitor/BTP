using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.AMP.BTP.UI.Filters;
using Jinher.JAP.MVC.Controller;
using Jinher.JAP.MVC.UIJquery.DataGrid;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.TPS;
using Newtonsoft.Json.Linq;
using System.Text;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public partial class CommodityChangedController : BaseController
    {
        /// <summary>
        /// 商品明细报表页
        /// </summary>
        /// <returns></returns>
        public ActionResult GoodsDetailReport(string appId)
        {
            ViewBag.appId = appId;
            return View();
        }
        /// <summary>
        /// 获取明细报表数据
        /// </summary>       
        /// <returns></returns>
        [GridAction]
        public ActionResult GetCommodityChangedList(string EsAppid, string SupplierCode, string AppId, string Name, string JDCode, string Barcode, string SubName, string ModName, string SubStarTime, string SubEndTime, string ModStarTime, string ModEndTime, string State)
        {
            try
            {
                CommodityChangeFacade ChangeFacade = new CommodityChangeFacade();
                #region 查询条件
                CommodityChangeDTO Search = new CommodityChangeDTO();
                //查询馆下所有的店铺及自己            
                List<Guid> appids = ChangeFacade.GetMallApplyList(new Guid(EsAppid)).Select(p => p.AppId).ToList();
                appids.Add(new Guid(EsAppid));
                Search.Appids = appids.Distinct().ToList();
                //供应商对应的appid
                List<Guid> AppidList = new List<Guid>();
                if (!string.IsNullOrEmpty(SupplierCode))
                {
                    AppidList = ChangeFacade.GetSupplierList(new Guid(EsAppid)).Where(p => p.SupplierCode == SupplierCode).Select(p => p.AppId).ToList();
                }
                Search.AppidsList = AppidList;
                Search.AppId = new Guid(AppId);
                Search.Name = Name;
                Search.JDCode = JDCode;
                Search.Barcode = Barcode;
                if (!string.IsNullOrEmpty(SubName))
                {
                    Search.SubId = CBCSV.GetUserIdByAccount(SubName); // SubName为发布人Account
                }
                else
                {
                    Search.SubId = Guid.Empty;
                }
                if (!string.IsNullOrEmpty(ModName))
                {
                    Search.ModifiedId = CBCSV.GetUserIdByAccount(ModName); //ModName为修改人Account
                }
                else
                {
                    Search.ModifiedId = Guid.Empty;
                }
                Search.SubStarTime = SubStarTime;
                Search.SubEndTime = SubEndTime;
                Search.ModStarTime = ModStarTime;
                Search.ModEndTime = ModEndTime;
                Search.State = State == "" ? -1 : Convert.ToInt32(State);

                int pageIndex = 1;
                int pageSize = 20;
                if (!string.IsNullOrEmpty(Request["page"]))
                {
                    pageIndex = int.Parse(Request["page"]);
                }
                #endregion

                Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO>> retInfo = ChangeFacade.GetCommodityChangeList(Search, pageIndex, pageSize);

                if (retInfo.Data.Count() > 0)
                {
                    #region 格式化商品属性
                    retInfo.Data.ForEach(p =>
                    {
                        if (p.Type == 0 || p.Type == null)
                        {
                            p.CommodityTypeName = "实体商品";
                        }
                        else if (p.Type == 1)
                        {
                            p.CommodityTypeName = "虚拟商品";
                        }
                        string str = null;
                        if (!string.IsNullOrEmpty(p.ComAttribute))
                        {
                            JArray objson = JArray.Parse(p.ComAttribute);
                            if (objson.ToString() != "[]")
                            {
                                foreach (var item in objson)
                                {
                                    JObject obj = JObject.Parse(item.ToString());
                                    str += obj["SecondAttribute"] + ",";
                                }
                                str = str.Remove(str.Length - 1, 1);
                                p.ComAttribute = str;
                            }
                            else
                            {
                                p.ComAttribute = str;
                            }
                        }
                        else
                        {
                            p.ComAttribute = str;
                        }
                    });
                    #endregion
                }

                List<string> NameList = new List<string>
                 {
                  "Id",
                  "SupplierName",
                  "SupplierTypeName",
                  "AppName",
                  "Barcode",
                  "CommodityTypeName",
                  "YJCouponActivityId",
                  "YJCouponType",
                  "No_Code",
                  "JDCode",
                  "Name",
                  "MarketPrice",
                  "Price",
                  "CostPrice",
                  "TaxClassCode",
                  "TaxRate",
                  "InputRax",
                  "ComAttribute",
                  "Unit",
                  "SubCode",
                  "SubOn",
                  "ModifiedCode",
                  "ModifiedOn",
                  "AuditState",
                  "SubName",
                  "ModifiedName",
                  "State",
                  "IsDel"
                 };
                int rowCount = retInfo != null ? retInfo.ResultCode : 0;
                var list = retInfo != null ? retInfo.Data : new List<CommodityChangeDTO>();
                return View(new GridModel<CommodityChangeDTO>(NameList, list, rowCount, pageIndex, pageSize, string.Empty));
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("控制器中根据搜索条件获取变动商品表异常。"), ex);
                return null;
            }

        }
        /// <summary>
        /// 统计数据信息
        /// </summary>        
        public JsonResult GetTotalNum(string EsAppid, string SupplierCode, string AppId, string Name, string JDCode, string Barcode, string SubName, string ModName, string SubStarTime, string SubEndTime, string ModStarTime, string ModEndTime, string State)
        {
            CommodityChangeFacade ChangeFacade = new CommodityChangeFacade();
            #region 查询条件
            CommodityChangeDTO Search = new CommodityChangeDTO();
            //查询馆下所有的店铺及自己            
            List<Guid> appids = ChangeFacade.GetMallApplyList(new Guid(EsAppid)).Select(p => p.AppId).ToList();
            appids.Add(new Guid(EsAppid));
            Search.Appids = appids.Distinct().ToList();
            //供应商对应的appid
            List<Guid> AppidList = new List<Guid>();
            if (!string.IsNullOrEmpty(SupplierCode))
            {
                AppidList = ChangeFacade.GetSupplierList(new Guid(EsAppid)).Where(p => p.SupplierCode == SupplierCode).Select(p => p.AppId).ToList();
            }
            Search.AppidsList = AppidList;
            Search.AppId = !string.IsNullOrEmpty(AppId) ? new Guid(AppId) : Guid.Empty;
            Search.Name = Name;
            Search.JDCode = JDCode;
            Search.Barcode = Barcode;
            if (!string.IsNullOrEmpty(SubName))
            {
                Search.SubId = CBCSV.GetUserIdByAccount(SubName); // SubName为发布人Account
            }
            else
            {
                Search.SubId = Guid.Empty;
            }
            if (!string.IsNullOrEmpty(ModName))
            {
                Search.ModifiedId = CBCSV.GetUserIdByAccount(ModName); //ModName为修改人Account
            }
            else
            {
                Search.ModifiedId = Guid.Empty;
            }
            Search.SubStarTime = SubStarTime;
            Search.SubEndTime = SubEndTime;
            Search.ModStarTime = ModStarTime;
            Search.ModEndTime = ModEndTime;
            Search.State = State == "" ? -1 : Convert.ToInt32(State);
            #endregion
            List<totalNum> list = ChangeFacade.GetTotalList(Search);
            return Json(new { data = list });
        }
        /// <summary>
        ///模糊匹配商家名称
        /// </summary>
        /// <param name="esAppId"></param>
        /// <returns></returns>
        public JsonResult GetMallAppInfo(Guid esAppId)
        {
            CommodityChangeFacade ChangeFacade = new CommodityChangeFacade();
            var list = ChangeFacade.GetMallApplyList(esAppId).Select(p => new BigAutocomplete
            {
                title = p.AppName,
                result = p.AppId.ToString()
            }).ToList();
            var keyword = Request["keyword"];
            if (!string.IsNullOrEmpty(keyword))
            {
                list = list.Where(p => p.title.Contains(keyword)).ToList();
            }
            return Json(new { data = list });
        }

        /// <summary>
        /// 模糊匹配供应商信息
        /// </summary>
        /// <param name="esAppId"></param>
        /// <returns></returns>
        public JsonResult GetSupplierInfo(Guid esAppId)
        {
            CommodityChangeFacade ChangeFacade = new CommodityChangeFacade();
            var list = ChangeFacade.GetSupplierList(esAppId).Select(p => new BigAutocomplete
            {
                title = p.SupplierName,
                result = p.AppId.ToString(),
                code = p.SupplierCode
            }).ToList();
            var keyword = Request["keyword"];
            if (!string.IsNullOrEmpty(keyword))
            {
                list = list.Where(p => p.title.Contains(keyword)).ToList();
            }
            return Json(new { data = list });
        }
        /// <summary>
        /// 模糊匹配供应商Code
        /// </summary>
        /// <param name="esAppId"></param>
        /// <returns></returns>
        public JsonResult GetSupplierCode(Guid esAppId)
        {
            CommodityChangeFacade ChangeFacade = new CommodityChangeFacade();
            var list = ChangeFacade.GetSupplierList(esAppId).Select(p => new BigAutocomplete
            {
                title = p.SupplierCode,
                result = p.AppId.ToString(),
                code = p.SupplierName
            }).ToList();
            var keyword = Request["keyword"];
            if (!string.IsNullOrEmpty(keyword))
            {
                list = list.Where(p => p.title.Contains(keyword)).ToList();
            }
            return Json(new { data = list });
        }
        /// <summary>
        /// 模糊查询用户信息
        /// </summary>
        /// <param name="esAppId"></param>
        /// <returns></returns>
        public List<Guid> GetUserInfoList(string name)
        {
            CommodityChangeFacade ChangeFacade = new CommodityChangeFacade();
            List<Guid> userlist = new List<Guid>();
            var list = ChangeFacade.GetUserList().Select(p => new BigAutocomplete
            {
                title = p.UserName,
                result = p.UserId.ToString()
            }).ToList();
            if (!string.IsNullOrEmpty(name))
            {
                list = list.Where(p => p.title.Contains(name)).ToList();
                foreach (var item in list)
                {
                    userlist.Add(new Guid(item.result));
                }
                return userlist;
            }
            else
            {
                return userlist;
            }

        }
        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ExportData(string EsAppid, string SupplierCode, string AppId, string Name, string JDCode, string Barcode, string SubName, string ModName, string SubStarTime, string SubEndTime, string ModStarTime, string ModEndTime, string State)
        {
            CommodityChangeFacade ChangeFacade = new CommodityChangeFacade();
            #region 查询条件
            CommodityChangeDTO Search = new CommodityChangeDTO();
            //查询馆下所有的店铺及自己            
            List<Guid> appids = ChangeFacade.GetMallApplyList(new Guid(EsAppid)).Select(p => p.AppId).ToList();
            appids.Add(new Guid(EsAppid));
            Search.Appids = appids.Distinct().ToList();
            //供应商对应的appid
            List<Guid> AppidList = new List<Guid>();
            if (!string.IsNullOrEmpty(SupplierCode))
            {
                AppidList = ChangeFacade.GetSupplierList(new Guid(EsAppid)).Where(p => p.SupplierCode == SupplierCode).Select(p => p.AppId).ToList();
            }
            Search.AppidsList = AppidList;
            Search.AppId = !string.IsNullOrEmpty(AppId) ? new Guid(AppId) : Guid.Empty;
            Search.Name = Name;
            Search.JDCode = JDCode;
            Search.Barcode = Barcode;
            if (!string.IsNullOrEmpty(SubName))
            {
                Search.SubId = CBCSV.GetUserIdByAccount(SubName); // SubName为发布人Account
            }
            else
            {
                Search.SubId = Guid.Empty;
            }
            if (!string.IsNullOrEmpty(ModName))
            {
                Search.ModifiedId = CBCSV.GetUserIdByAccount(ModName); //ModName为修改人Account
            }
            else
            {
                Search.ModifiedId = Guid.Empty;
            }
            Search.SubStarTime = SubStarTime;
            Search.SubEndTime = SubEndTime;
            Search.ModStarTime = ModStarTime;
            Search.ModEndTime = ModEndTime;
            Search.State = State == "" ? -1 : Convert.ToInt32(State);
            #endregion
            int result = ChangeFacade.GetCommodityChangeExport(Search).Count();
            return Json(new { data = result });
        }
        /// <summary>
        /// 导出(excel) 商品明细报表
        /// </summary>
        /// <returns></returns>
        public FileResult ExportExcelData(string hidEsAppid, string hidSupplierCode, string hidAppId, string hidName, string hidJDCode, string hidBarcode, string hidSubName, string hidModName, string hidSubStarTime, string hidSubEndTime, string hidModStarTime, string hidModEndTime, string hidState)
        {
            CommodityChangeFacade ChangeFacade = new CommodityChangeFacade();
            #region 查询条件
            CommodityChangeDTO Search = new CommodityChangeDTO();
            //查询馆下所有的店铺及自己            
            List<Guid> appids = ChangeFacade.GetMallApplyList(new Guid(hidEsAppid)).Select(p => p.AppId).ToList();
            appids.Add(new Guid(hidEsAppid));
            Search.Appids = appids.Distinct().ToList();
            //供应商对应的appid
            List<Guid> AppidList = new List<Guid>();
            if (!string.IsNullOrEmpty(hidSupplierCode))
            {
                AppidList = ChangeFacade.GetSupplierList(new Guid(hidEsAppid)).Where(p => p.SupplierCode == hidSupplierCode).Select(p => p.AppId).ToList();
            }
            Search.AppidsList = AppidList;
            Search.AppId = !string.IsNullOrEmpty(hidAppId) ? new Guid(hidAppId) : Guid.Empty;
            Search.Name = hidName;
            Search.JDCode = hidJDCode;
            Search.Barcode = hidBarcode;
            if (!string.IsNullOrEmpty(hidSubName))
            {
                Search.SubId = CBCSV.GetUserIdByAccount(hidSubName); // SubName为发布人Account
            }
            else
            {
                Search.SubId = Guid.Empty;
            }
            if (!string.IsNullOrEmpty(hidModName))
            {
                Search.ModifiedId = CBCSV.GetUserIdByAccount(hidModName); //ModName为修改人Account
            }
            else
            {
                Search.ModifiedId = Guid.Empty;
            }
            Search.SubStarTime = hidSubStarTime;
            Search.SubEndTime = hidSubEndTime;
            Search.ModStarTime = hidModStarTime;
            Search.ModEndTime = hidModEndTime;
            Search.State = hidState == "" ? -1 : Convert.ToInt32(hidState);
            #endregion
            List<CommodityChangeDTO> result = ChangeFacade.GetCommodityChangeExport(Search);
            if (result.Count() > 0)
            {
                #region 格式化商品属性
                result.ForEach(p =>
                {
                    if (p.Type == 0 || p.Type == null)
                    {
                        p.CommodityTypeName = "实体商品";
                    }
                    else if (p.Type == 1)
                    {
                        p.CommodityTypeName = "虚拟商品";
                    }
                    string str = null;
                    if (!string.IsNullOrEmpty(p.ComAttribute))
                    {
                        JArray objson = JArray.Parse(p.ComAttribute);
                        if (objson.ToString() != "[]")
                        {
                            foreach (var item in objson)
                            {
                                JObject obj = JObject.Parse(item.ToString());
                                str += obj["SecondAttribute"] + ",";
                            }
                            str = str.Remove(str.Length - 1, 1);
                            p.ComAttribute = str;
                        }
                        else
                        {
                            p.ComAttribute = str;
                        }
                    }
                    else
                    {
                        p.ComAttribute = str;
                    }
                });
                #endregion
            }
            StringBuilder sbHtml = new StringBuilder();
            sbHtml.Append("<meta http-equiv='Content-Type' content='text/html; charset=utf-8' />");
            sbHtml.Append("<table border='1' cellspacing='0' cellpadding='0'>");
            sbHtml.Append("<tr>");
            var lstTitle = new List<string> { "供应商名称", "APP名称", "商品条码", "商品类型", "活动编码", "类型编码", "商品编号", "备注编码", "商品名称", "市场价", "商品现价", "进货价", "税收编码", "销项税", "进项税", "商品属性", "计价单位", "发布人", "发布时间", "修改人", "修改时间", "商品状态" };
            foreach (var item in lstTitle)
            {
                sbHtml.Append("<td style='font-size: 14px;text-align:center;background-color: #DCE0E2; font-weight:bold;' height='25'>" + item + "</td>");
            }
            sbHtml.Append("</tr>");
            if (result.Count() > 0)
            {
                foreach (CommodityChangeDTO item in result)
                {
                    sbHtml.Append("<tr>");
                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.SupplierName + "</td>");
                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.AppName + "</td>");
                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Barcode + "</td>");
                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.CommodityTypeName + "</td>");
                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.YJCouponActivityId + "</td>");
                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.YJCouponType + "</td>");
                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.No_Code + "</td>");
                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.JDCode + "</td>");
                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Name + "</td>");
                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.MarketPrice + "</td>");
                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Price + "</td>");
                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.CostPrice + "</td>");
                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.TaxClassCode + "</td>");
                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.TaxRate + "</td>");
                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.InputRax + "</td>");
                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ComAttribute + "</td>");
                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.Unit + "</td>");
                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.SubCode + "&nbsp;&nbsp;&nbsp;" + item.SubName + "</td>");
                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.SubOn + "</td>");
                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ModifiedCode + "&nbsp;&nbsp;&nbsp;" + item.ModifiedName + "</td>");
                    if (item.SubOn.ToString().Substring(0, item.SubOn.ToString().Length - 2) == item.ModifiedOn.ToString().Substring(0, item.ModifiedOn.ToString().Length - 2))
                    {
                        sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'></td>");
                    }
                    else
                    {
                        sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.ModifiedOn + "</td>");
                    }
                    sbHtml.Append("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>" + item.StateName + "</td>");
                    sbHtml.Append("</tr>");
                }
            }
            sbHtml.Append("</table>");
            return File(System.Text.Encoding.UTF8.GetBytes(sbHtml.ToString()), "application/vnd.ms-excel", string.Format("商品明细报表_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));

        }
    }
}
