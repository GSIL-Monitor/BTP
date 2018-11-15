using System;
using System.Web.Mvc;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.TPS;
using Jinher.JAP.MVC.Controller;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using System.Collections.Generic;
using System.Text;
using Jinher.JAP.MVC.UIJquery.DataGrid;
using System.Net;
using Jinher.AMP.BTP.UI.Filters;

namespace Jinher.AMP.BTP.UI.Areas.WeChatQRCode.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class WeChatQrCodeManageController : BaseController
    {
        #region 添加带参二维码
        // GET: /WeChatQRCode/CreateWeChatQRCode/
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="cdto"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateWeChatQrCode(Deploy.CustomDTO.CateringDTO.WeChatQRCodeDTO cdto)
        {
            if (cdto == null)
                return Json(new Deploy.CustomDTO.ResultDTO { isSuccess = false, Message = "参数为空" });
            cdto.id = Guid.NewGuid();
            Deploy.CustomDTO.ResultDTO<string> ticket = GetWeChatTicket(cdto);
            cdto.WeChatTicket = ticket.isSuccess ? ticket.Data : "";
            Deploy.CustomDTO.ResultDTO ret = new Deploy.CustomDTO.ResultDTO { isSuccess = ticket.isSuccess, Message = "生成票据失败！", ResultCode = 0 };

            if (ticket.isSuccess)
            {
                IBP.Facade.WeChatQRCodeFacade facade = new IBP.Facade.WeChatQRCodeFacade();
                ret = facade.CreateWeChatQRCode(cdto);
            }
            var result = new { ret = ret, ticket = cdto.WeChatTicket };
            return Json(result);
        }
        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="cdto"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateWeChatQrCodeByCustomAppId(Deploy.CustomDTO.CateringDTO.WeChatQRCodeDTO cdto)
        {
            if (cdto == null)
                return Json(new Deploy.CustomDTO.ResultDTO { isSuccess = false, Message = "参数为空" });
            cdto.appId = CustomConfig.WeChatSpreader.AppId;
            return CreateWeChatQrCodeByAppId(cdto);


        }
        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="cdto"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateWeChatQrCodeByAppId(Deploy.CustomDTO.CateringDTO.WeChatQRCodeDTO cdto)
        {
            if (cdto == null)
                return Json(new Deploy.CustomDTO.ResultDTO { isSuccess = false, Message = "参数为空" });
            cdto.IsAppWeChatSetting = true;
            var searchResult = WCPSV.Instance.GetDeveloperInfo(cdto.appId);
            if (searchResult == null)
                return Json(new Deploy.CustomDTO.ResultDTO { isSuccess = false, Message = "应用未配置公众号信息" });
            cdto.WeChatAppId = searchResult.WAppId;
            cdto.weChatSecret = searchResult.WSecret;
            return CreateWeChatQrCode(cdto);
        }

        Deploy.CustomDTO.ResultDTO<string> GetWeChatTicket(Deploy.CustomDTO.CateringDTO.WeChatQRCodeDTO cdto)
        {
            ISV.Facade.WeChatQRCodeFacade facade = new ISV.Facade.WeChatQRCodeFacade();
            Deploy.CustomDTO.WeChat.ForeverQrcodeDTO param = new Deploy.CustomDTO.WeChat.ForeverQrcodeDTO
            {
                UseDeveloperId = true,
                AppId = cdto.WeChatAppId,
                JhAppId = cdto.appId,
                AppSecret = cdto.weChatSecret,
                SceneStr = string.Format("qrtype={0}", cdto.id),
                IsAppWeChatSetting = cdto.IsAppWeChatSetting
            };

            Deploy.CustomDTO.ResultDTO<string> ret = facade.CreateForeverQrcode(param);
            return ret;
        }
        /// <summary>
        /// 获取流水号
        /// </summary>
        /// <returns></returns>
        int GetWeChatQRNo()
        {
            IBP.Facade.WeChatQRCodeFacade facade = new IBP.Facade.WeChatQRCodeFacade();
            return facade.GetWeChatQRNo();
        }
        #endregion

        #region 添加微处菜单
        /// <summary>
        /// 添加微处菜单
        /// </summary>
        /// <returns></returns>
        public ActionResult AddWeChatMenu()
        {
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="strMenuJson"></param>
        /// <returns></returns>
        public ActionResult CustomWeChatMenu(Guid appId, string strMenuJson)
        {
            IBP.Facade.WeChatQRCodeFacade facade = new IBP.Facade.WeChatQRCodeFacade();
            bool isSuccess = facade.AddWeChatMenu(appId, strMenuJson);
            return Json(new { ret = isSuccess });
        }
        #endregion

        #region 推广主相关代码
        /// <summary>
        /// 添加微处菜单
        /// </summary>
        /// <returns></returns>
        public ActionResult SpreadManagerIndex()
        {
            return View();
        }
        #endregion

        /// <summary>
        /// 新二维码管理
        /// </summary>
        /// <returns></returns>
        [CheckAppId]
        public ActionResult Manager(Guid appId)
        {
            ViewBag.AppId = appId;
            WeChatQRCodeFacade facade = new WeChatQRCodeFacade();
            ViewBag.QRTypes = facade.GetQrCodeTypeList(new Deploy.CustomDTO.WeChatQRCodeSearchDTO { AppId = appId }).Data;
            return View();
        }

        /// <summary>
        /// 获取二维码列表
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Manager(WeChatQRCodeSearchDTO dto, int page)
        {
            dto.AppId = (Guid)Session["APPID"];
            dto.PageIndex = page;
            WeChatQRCodeFacade facade = new WeChatQRCodeFacade();
            var result = facade.GetWechatQrCodeList(dto);
            if (result.isSuccess)
            {
                return Json(new
                {
                    data = result.Data.List,
                    records = result.Data.Count,
                    page = dto.PageIndex
                });
            }
            return Json(result);
        }

        /// <summary>
        /// 下载二维码
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public ActionResult Download(string ticket, string fileName)
        {
            using (WebClient wc = new WebClient())
            {
                var data = wc.DownloadData("https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=" + ticket);
                return File(data, "image/jpg", Url.Encode(fileName));
            }
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ActionResult ExportExcel(WeChatQRCodeSearchDTO dto)
        {
            dto.AppId = (Guid)Session["APPID"];
            dto.PageIndex = 1;
            dto.PageSize = int.MaxValue;
            WeChatQRCodeFacade facade = new WeChatQRCodeFacade();
            var qrCodes = facade.GetWechatQrCodeList(dto);
            var sbHtml = new StringBuilder();
            sbHtml.Append("<table border='1' cellspacing='0' cellpadding='0'>");
            sbHtml.Append("<tr>");
            var lstTitle = new List<string> { "推广渠道", "微信公众号名称", "微信AppID", "微信AppSecret", "二维码链接", "绑定状态", "是否启用", "备注" };
            foreach (var item in lstTitle)
            {
                sbHtml.AppendFormat("<td style='font-size: 14px;text-align:center;background-color: #DCE0E2; font-weight:bold;' height='25'>{0}</td>", item);
            }
            sbHtml.Append("</tr>");
            foreach (var model in qrCodes.Data.List)
            {
                sbHtml.Append("<tr>");
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.QrTypeDesc);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.WeChatPublicCode);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.AppId);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.AppId);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'><a href='{0}'>{0}</a></td>", "https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=" + model.WeChatTicket);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.IsUse ? "已绑定" : "未绑定");
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.IsDel == 0 ? "是" : "否");
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.Description);
                sbHtml.Append("</tr>");
            }
            sbHtml.Append("</table>");
            return File(System.Text.Encoding.UTF8.GetBytes(sbHtml.ToString()), "application/ms-excel", string.Format("带参数二维码{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }

        /// <summary>
        /// 批量创建公众号二维码
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateQrCode(QrCodeCreateDTO dto)
        {
            WeChatQRCodeFacade facade = new WeChatQRCodeFacade();
            return Json(facade.CreateWeChatQrCodeBatch(dto));
        }

        /// <summary>
        /// 启用、停用二维码
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ActionResult UpdateState(WeChatQRCodeUpdateStateDTO dto)
        {
            WeChatQRCodeFacade facade = new WeChatQRCodeFacade();
            return Json(facade.UpdateState(dto));
        }
    }
}
