#define WhenTest
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.AMP.App.Deploy.CustomDTO;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.MVC.Cache;
using System.Text;
using Jinher.AMP.CBC.Deploy;
using Jinher.AMP.CBC.Deploy.CustomDTO;
using Jinher.AMP.BTP.UI.Filters;
using System.IO;
using Jinher.AMP.BTP.ISV.Facade;
using System.Text.RegularExpressions;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.UI.ModelLogic;
using Jinher.JAP.Common.Loging;
using CommodityDistributionDTO = Jinher.AMP.BTP.Deploy.CommodityDistributionDTO;


namespace Jinher.AMP.BTP.UI.Controllers
{
    /// <summary>
    /// 推广相关页面
    /// </summary>
    public partial class DistributeController : Jinher.JAP.MVC.Controller.BaseController
    {

        /// <summary>
        /// 三级分销全局设置功能
        /// </summary>
        /// <returns></returns>
        [CheckAppId]
        public ActionResult Manage()
        {
            Guid appId = (Guid)System.Web.HttpContext.Current.Session["APPID"];
            Jinher.AMP.BTP.IBP.Facade.DistributeFacade facade = new DistributeFacade();
            var resultCount = facade.ManageNc(new ManageVM { AppId = appId });
            ViewBag.ManageNc = resultCount;
            return View();
        }

        /// <summary>
        /// 三级分销全局设置功能 ajax
        /// </summary>
        /// <returns></returns>
        [CheckAppId]
        public ActionResult ManagerList(Jinher.AMP.BTP.Deploy.CustomDTO.ManageVM manaDTO)
        {
            Guid appId = (Guid)System.Web.HttpContext.Current.Session["APPID"];
            Jinher.AMP.BTP.IBP.Facade.DistributeFacade facade = new DistributeFacade();
            var result = facade.ManageInfo(manaDTO);

            var countSaved = result.Count;
            var managerCountSaved = result.Manager.Count;

            //使用测试数据覆盖DSS的数据
            //上线时请确保注释掉此代码

            //#if WhenTest
            //    result = CreateTestManageDto(appId);
            //#endif

            //调用JudgeDistributorsHasIdentityExt更新每个分销商的HasIdentityInfo属性
            var distributorList = result.Manager.Select(x => x.Id).ToList();
            var applyIdentitys = facade.GetDistributorsIdentitys(distributorList);
            var distributorsRemarks = facade.GetDistributorsRemarks(distributorList);
            foreach (var d in result.Manager)
            {
                var ai = applyIdentitys.FirstOrDefault(x => x.DistributorId == d.Id);
                if (ai != null && ai.HasIdentity)
                {
                    d.HasIdentityInfo = true;
                    d.ApplyId = ai.ApplyId;
                    DistributionIdentityLogic.DecorateManagerSDto(d, ai.Identitys);
                    d.Pic = ai.PicturePath;
                }
                else
                {
                    d.HasIdentityInfo = false;
                    d.ApplyId = Guid.Empty;
                }

                d.Remarks = distributorsRemarks.ContainsKey(d.Id) ? distributorsRemarks[d.Id] : "";
            }

            if (result.Count != countSaved || result.Manager.Count != managerCountSaved)
            {
                LogHelper.Error(
                    string.Format(
                        "Distribute.ManagerList(AppId='{0}',ParentId='{1}')，从DSS返回：Count={2},Manager.Count={3}，最终返回：Count={4},Manager.Count={5}。",
                        manaDTO.AppId, manaDTO.ParentId, countSaved, managerCountSaved, result.Count,
                        result.Manager.Count));
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 备注分销商申请
        /// </summary>
        /// <param name="id"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [CheckAppId]
        public ActionResult RemarkDistributor(Guid id, string content)
        {
            var remarks = Uri.UnescapeDataString(content);//解码
            var facade = new IBP.Facade.DistributeFacade();

            var ret = facade.RemarkDistributor(id, remarks);
            return Json(new { Result = ret.isSuccess, Messages = ret.Message });
        }

        /// <summary>
        /// 导出订单(excel)
        /// </summary>
        /// <returns></returns>
        [CheckAppId]
        public void ExportManagerInfo(Jinher.AMP.BTP.Deploy.CustomDTO.ManageVM manaDTO)
        {
            Guid appId = (Guid)System.Web.HttpContext.Current.Session["APPID"];
            Jinher.AMP.BTP.IBP.Facade.DistributeFacade facade = new DistributeFacade();
            manaDTO.PageSize = int.MaxValue;
            manaDTO.PageIndex = 1;
            manaDTO.AppId = appId;
            var result = facade.ManageInfo(manaDTO);
            var sbHtml = new StringBuilder();
            sbHtml.Append("<table border='1' cellspacing='0' cellpadding='0'>");
            sbHtml.Append("<tr>");
            var lstTitle = new List<string> { "序号", "分销商", "销量(元)", "佣金(元)", "下级分销商数", "注册时间" };
            foreach (var item in lstTitle)
            {
                sbHtml.AppendFormat("<td style='font-size: 14px;text-align:center;background-color: #DCE0E2; font-weight:bold;' height='25'>{0}</td>", item);
            }
            sbHtml.Append("</tr>");
            if (result == null || result.Count < 1 || result.Manager == null)
            {

            }
            else
            {
                var resultData = result.Manager;
                int i = 1;
                foreach (ManagerSDTO model in resultData)
                {
                    sbHtml.Append("<tr>");
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", i);
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}{1}</td>", model.Name, model.UserCode);
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.OrderAmount);
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.CommissionAmount);
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.UnderlingCount);
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.UserSubTime);
                    sbHtml.Append("</tr>");
                    i++;
                }
            }
            sbHtml.Append("</table>");
            string filePath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Temp\\fenxiaoshang";
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
            string fileName = "fenxiaoshang" + Guid.NewGuid() + ".xls";

            using (FileStream fs = new FileStream(filePath + "\\" + fileName, FileMode.Create))
            {
                StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                sw.Write(sbHtml.ToString());
                fs.Flush();
                sw.Close();
            }

            Response.Charset = "UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.AppendHeader("Content-Disposition", "attachment;filename=dingdan" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            Response.WriteFile(filePath + "\\" + fileName);
            Response.End();

            System.IO.File.Delete(filePath + "\\" + fileName);

        }

        /// <summary>
        /// 分销商订单页
        /// </summary>
        /// <returns></returns> 
        public ActionResult DistributeOrderList()
        {

            return View();
        }

        /// <summary>
        /// 订单详情页
        /// </summary>
        /// <param name="commodityOrderId"></param>
        /// <returns></returns>
        public ActionResult CommodityOrderDetail(string commodityOrderId)
        {
            //Guid appId = (Guid)System.Web.HttpContext.Current.Session["APPID"];
            Jinher.AMP.BTP.IBP.Facade.CommodityOrderFacade cf = new Jinher.AMP.BTP.IBP.Facade.CommodityOrderFacade();
            CommodityOrderVM CommodityOrder = cf.GetCommodityOrder(new Guid(commodityOrderId), Guid.Empty);
            ViewBag.CommodityOrder = CommodityOrder;
            return View();
        }

        /// <summary>
        /// 获取订单列表信息
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ActionResult GetDistributeOrderList(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderDistributionSearchDTO search)
        {
            Jinher.AMP.BTP.IBP.Facade.CommodityOrderFacade facade = new IBP.Facade.CommodityOrderFacade();
            var result = facade.GetDistributeOrderList(search);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 导出订单(excel)
        /// </summary>
        /// <param name="search"></param>
        public void ExportDistributeOrderListData(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderDistributionSearchDTO search)
        {
            Jinher.AMP.BTP.IBP.Facade.CommodityOrderFacade facade = new IBP.Facade.CommodityOrderFacade();
            search.PageSize = int.MaxValue;
            search.PageIndex = 1;
            var result = facade.GetDistributeOrderList(search);
            var sbHtml = new StringBuilder();
            sbHtml.Append("<table border='1' cellspacing='0' cellpadding='0'>");
            sbHtml.Append("<tr>");
            var lstTitle = new List<string> { "序号", "订单编号", "金额", "佣金", "订单完成时间" };
            foreach (var item in lstTitle)
            {
                sbHtml.AppendFormat("<td style='font-size: 14px;text-align:center;background-color: #DCE0E2; font-weight:bold;' height='25'>{0}</td>", item);
            }
            sbHtml.Append("</tr>");
            if (result == null || result.Count < 1 || result.CommodityOrderDistributionInfoList == null)
            {

            }
            else
            {
                var resultData = result.CommodityOrderDistributionInfoList;
                int i = 1;
                foreach (Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderDistributionInfoDTO model in resultData)
                {
                    sbHtml.Append("<tr>");
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", i);
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.Code);
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.Price);
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.DistributeMoney);
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.EndTime);
                    sbHtml.Append("</tr>");
                    i++;
                }
            }
            sbHtml.Append("</table>");
            string filePath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Temp\\dingdan";
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
            string fileName = "dingdan" + Guid.NewGuid() + ".xls";

            using (FileStream fs = new FileStream(filePath + "\\" + fileName, FileMode.Create))
            {
                StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                sw.Write(sbHtml.ToString());
                fs.Flush();
                sw.Close();
            }

            Response.Charset = "UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.AppendHeader("Content-Disposition", "attachment;filename=dingdan" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            Response.WriteFile(filePath + "\\" + fileName);
            Response.End();

            System.IO.File.Delete(filePath + "\\" + fileName);
        }

        /// <summary>
        /// 初始加载
        /// </summary>
        /// <returns></returns>
        public ActionResult DistributionRule()
        {
            DistributRuleFullDTO distributRuleFullDto = new DistributRuleFullDTO();
            string id = Request["appId"];
            if (!string.IsNullOrEmpty(id))
            {
                distributRuleFullDto = GetDistributRuleFullDTOByAppId(id);
                if (distributRuleFullDto.DbIdentitySets == null)
                {
                    distributRuleFullDto.DbIdentitySets = new List<DistributionIdentitySetFullDTO>();
                }
            }
            return View(distributRuleFullDto);
        }

        /// <summary>
        /// 获取成为分销商设置
        /// </summary>
        /// <param name="appId">appid</param>
        /// <returns></returns>
        public DistributRuleFullDTO GetDistributRuleFullDTOByAppId(string appId)
        {
            DistributionSearchDTO distributionSearchDto = new DistributionSearchDTO
            {
                AppId = new Guid(appId),
                UserId = this.ContextDTO.LoginUserID
            };
            DistributeFacade distributeFacade = new DistributeFacade();
            DistributRuleFullDTO distributRuleFullDto = distributeFacade.GetDistributRuleFull(distributionSearchDto);
            if (distributRuleFullDto == null)
            {
                distributRuleFullDto = new DistributRuleFullDTO
                {
                    DbIdentitySets = new List<DistributionIdentitySetFullDTO>(),
                    Title = ""
                };
            }
            return distributRuleFullDto;
        }

        /// <summary>
        /// 前端页面使用
        /// </summary>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public ActionResult GetDistributRuleFullDTOByAppId1(string appId)
        {
            BTP.Deploy.CustomDTO.ResultDTO<DistributRuleFullDTO> ret = new ResultDTO<DistributRuleFullDTO>();
            DistributRuleFullDTO distributRuleFullDto = GetDistributRuleFullDTOByAppId(appId);
            ret.Data = distributRuleFullDto;
            ret.ResultCode = 0;

            return Json(new { Message = ret.Message, Code = ret.ResultCode, Data = ret.Data }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存分销商条件设置
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ModifyDistributRuleFull(DistributRuleFullDTO distributRuleFullDto, string strJson)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result = new Deploy.CustomDTO.ResultDTO();
            try
            {
                if (!string.IsNullOrEmpty(strJson) && strJson != "[]")
                {
                    System.Web.Script.Serialization.JavaScriptSerializer serial = new System.Web.Script.Serialization.JavaScriptSerializer();
                    distributRuleFullDto.DbIdentitySets = serial.Deserialize<List<DistributionIdentitySetFullDTO>>(strJson);
                }

                DistributeFacade distributeFacade = new DistributeFacade();
                result = distributeFacade.ModifyDistributRuleFull(distributRuleFullDto);
            }
            catch (Exception ex)
            {
                LogHelper.Error("ModifyDistributRuleFull异常，异常信息：", ex);
                result.ResultCode = -1;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 创建ManageDTO测试数据
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        private ManageDTO CreateTestManageDto(Guid appId)
        {
            var facade = new DistributeFacade();
            int rowCount;
            var distributors = facade.GetDistributors(appId, 10, 1, out rowCount);

            var managerSdtos =
                distributors.Select(
                    x =>
                        new ManagerSDTO
                        {
                            Esappid = x.EsAppId,
                            Id = x.Id,
                            Key = x.Key,
                            ParentId = x.ParentId,
                            Level = x.Level,
                            Name = x.Name,
                            UserCode = x.UserCode,
                            UserSubTime = x.UserSubTime
                        }).ToList();
            return new ManageDTO
            {
                Count = rowCount,
                ParentCode = "00000000-0000-0000-0000-000000000000",
                ParentName = "",
                Manager = managerSdtos
            };
        }
    }
}