using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.BTP.UI.Commons;
using Jinher.AMP.BTP.UI.Filters;
using Jinher.AMP.BTP.UI.ModelLogic;
using Jinher.AMP.BTP.UI.Util;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.MVC.Controller;

namespace Jinher.AMP.BTP.UI.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public partial class DistributionApplyAuditorController : BaseController//
    {
        /// <summary>
        /// 分销商申请 列表
        /// </summary>
        /// <returns></returns>
        [CheckAppId]
        public ActionResult Index()
        {
            var appId = WebUtil.AppId;
            var pageIndex = string.IsNullOrEmpty(Request.QueryString["currentPage"]) ? 1 : int.Parse(Request.QueryString["currentPage"]);
            var pageSize = 10;
            int rowCount;

            var list = SearchDistributionApplyList(appId, pageIndex, pageSize, out rowCount, "", -1);

            var stateEnums = new EnumHelper().GetEnumChineseDepicts<DistributeApplyStateEnum>(0, true);
            stateEnums.Remove(stateEnums.FirstOrDefault(x => x.Key == (int)DistributeApplyStateEnum.AuditAgain));
            ViewBag.ApplyStates = stateEnums;

            ViewBag.ApplyList = list;
            ViewBag.Count = rowCount;
            ViewBag.AppId = WebUtil.AppId;
            return View();
        }
        
        /// <summary>
        /// 分销商申请 部分页
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [CheckAppId]
        public PartialViewResult PartialIndex(string userName, int state)
        {
            Guid appId;
            if (!Guid.TryParse(System.Web.HttpContext.Current.Session["APPID"].ToString(), out appId))
            {
                Response.StatusCode = 404;
                return null;
            }

            var pageIndex = string.IsNullOrEmpty(Request.QueryString["currentPage"]) ? 1 : int.Parse(Request.QueryString["currentPage"]);
            var pageSize = 10;
            int rowCount;

            var list = SearchDistributionApplyList(appId, pageIndex, pageSize, out rowCount, userName, state);

            ViewBag.UserName = userName;
            ViewBag.State = state;
            ViewBag.ApplyList = list;
            ViewBag.Count = rowCount;
            ViewBag.lastIndex = (pageIndex - 1) * pageSize;

            return PartialView();
        }

        /// <summary>
        /// 导出订单(excel)
        /// </summary>
        /// <returns></returns>
        [CheckAppId]
        public void ExportApplyList(string exportUserName, int exportState)
        {
            Guid appId;
            if (!Guid.TryParse(System.Web.HttpContext.Current.Session["APPID"].ToString(), out appId))
            {
                Response.StatusCode = 404;
                return ;
            }

            var pageIndex = 1;
            var pageSize = int.MaxValue; 
            int rowCount;

            var list = SearchDistributionApplyList(appId, pageIndex, pageSize, out rowCount, exportUserName, exportState);
            

            var sbHtml = new StringBuilder();
            sbHtml.Append("<table border='1' cellspacing='0' cellpadding='0'>");
            sbHtml.Append("<tr>");
            var lstTitle = new List<string> { "序号", "申请人", "申请时间", "状态", "备注" };
            foreach (var item in lstTitle)
            {
                sbHtml.AppendFormat("<td style='font-size: 14px;text-align:center;background-color: #DCE0E2; font-weight:bold;' height='25'>{0}</td>", item);
            }
            sbHtml.Append("</tr>");
            if (list != null &&list.Any())
            {
                int i = 1;
                foreach (var model in list)
                {
                    sbHtml.Append("<tr>");
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", i);
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0} {1}</td>", model.UserName, model.UserCode);
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.SubTime);
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.StateName);
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.Remarks);
                    sbHtml.Append("</tr>");
                    i++;
                }
            }
            sbHtml.Append("</table>");
            string filePath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Temp\\FenXiaoShangShenQing";
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
            string fileName = "FenXiaoShangShenQing" + Guid.NewGuid() + ".xls";

            using (FileStream fs = new FileStream(filePath + "\\" + fileName, FileMode.Create))
            {
                StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                sw.Write(sbHtml.ToString());
                fs.Flush();
                sw.Close();
            }

            Response.Charset = "UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.AppendHeader("Content-Disposition", "attachment;filename=FenXiaoShangShenQing" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            Response.WriteFile(filePath + "\\" + fileName);
            Response.End();

            System.IO.File.Delete(filePath + "\\" + fileName);

        }

        /// <summary>
        /// 备注分销商申请
        /// </summary>
        /// <param name="id"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [CheckAppId]
        public ActionResult RemarkDistributionApply(Guid id, string content)
        {
            var remarks = Uri.UnescapeDataString(content);//解码
            var facade = new IBP.Facade.DistributeFacade();

            var ret = facade.RemarkDistributionApply(id, remarks);
            return Json(new { Result = ret.isSuccess, Messages = ret.Message });
        }
         /// <summary>
        /// 获取分销商申请的审批历史记录
        /// </summary>
        /// <param name="id"></param>
        [CheckAppId]
        public ActionResult GetApplyAuditList(Guid id)
        {
            var facade = new IBP.Facade.DistributeFacade();
            var applyId = id;

            var ret = facade.GetApplyAuditList(applyId);
            return Json(new { Result = 1, Data = ret });
        }

        /// <summary>
        /// 通过审核分销商申请
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [CheckAppId]
        public ActionResult PassAuditingDistributionApply(Guid id)
        {
            var facade = new IBP.Facade.DistributeFacade();

            var auditingRet = facade.AuditingDistributionApply(id, true, "");
            if (auditingRet.isSuccess)
            {
                var auditingRetData = auditingRet.Data;
                var qrCodeUrl = CreateQrCode(auditingRetData.MicroShopLogo, auditingRetData.MicroShopUrl);
                facade.UpdateMicroshopQrCode(new UpdateQrCodeRequestDTO
                {
                    MicroShopId = auditingRetData.MicroShopId.Value,
                    QRCodeUrl = qrCodeUrl
                });
            }

            return Json(new {Result = true, Messages = ""});
        }

        /// <summary>
        /// 不通过审核分销商申请
        /// </summary>
        /// <param name="id"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [CheckAppId]
        public ActionResult RefuseAuditingDistributionApply(Guid id, string content)
        {
            var refuseReason = Uri.UnescapeDataString(content);//解码
            var facade = new IBP.Facade.DistributeFacade();

            var auditingRet = facade.AuditingDistributionApply(id, false, refuseReason);
            return Json(new { Result = auditingRet.isSuccess, Messages = auditingRet.Message });
        }

        /// <summary>
        /// 展示分销商的身份条件信息
        /// </summary>
        /// <returns></returns>
        public ActionResult IdentityInfo(Guid applyId)
        {
            var facade = new IBP.Facade.DistributeFacade();

            var identityDtos = facade.GetDistributorApplyIdentityVals(applyId);
            if (!identityDtos.Any())
            {
                ViewBag.IdentityVals = new List<DistributionIdentityResultDTO>();
                return View();
            }

            //修改其中的 用户名、账号
            var apply = facade.GetDistributionApply(applyId);
           
            //判断是否必填
            var ruleId = apply.RuleId;
            var setDtos = facade.GetDistributionIdentitySets(ruleId);
            Predicate<DistributionIdentityDTO> judgeIsRequired = (x) =>
            {
                var set = setDtos.FirstOrDefault(k => k.Id == x.IdentitySetId); 
                return set != null && set.IsRequired;
            };

            var lstData = identityDtos.Select(
                x => new DistributionIdentityResultDTO(
                    x.Name, x.Value, (int) x.ValueType, x.NameCategory,
                    judgeIsRequired(x))).ToList();

            ViewBag.IdentityVals = lstData; 

            return View();
        }
        
        private static string CreateQrCode(string fileImg, string replaceUrl)
        {
            string qrCode = "";
            try
            {
                //网络图片读取
                WebClient mywebclient = new WebClient();
                var imgfile = mywebclient.DownloadData(fileImg);

                //本地图片读取
                //var imgfile = Jinher.AMP.EBC.Common.ImageHelper.ConvertFileToBinary(fileImg);

                var qRCodeWithIconDto = new Jinher.JAP.BaseApp.Tools.Deploy.CustomDTO.QRCodeWithIconDTO
                {
                    IconDate = imgfile,
                    Source = replaceUrl
                };

                //生成带图片的二维码
                var codepath = TPS.BaseAppToolsSV.Instance.GenQRCodeWithIcon(qRCodeWithIconDto);
                qrCode = CustomConfig.CommonFileServerUrl + codepath;
            }
            catch (Exception ex)
            {
                string errStack = ex.Message + ex.StackTrace;
                while (ex.InnerException != null)
                {
                    errStack += ex.InnerException.Message + ex.InnerException.StackTrace;
                    ex = ex.InnerException;
                }
                LogHelper.Error("CreateQrCode异常，异常信息：" + errStack, ex);
           }
           return qrCode;
        }

        private List<DistributionApplyResultDTO> SearchDistributionApplyList(Guid appId, int pageIndex,
            int pageSize, out int rowCount, string userName, int state)
        {
            var enumHelper = new EnumHelper();
            var facade = new IBP.Facade.DistributeFacade();

            var list = facade.GetDistributionApplyListByWhere(appId, pageSize, pageIndex, out rowCount,
                userName, state);
            list.ForEach(x => x.StateName = (x.State == DistributeApplyStateEnum.AuditAgain ? "待审核" : enumHelper.GetDepict(x.State)));

            list.ForEach(DistributionIdentityLogic.DecorateDistributionApplyDto);

            return list;
        }
    }
}
