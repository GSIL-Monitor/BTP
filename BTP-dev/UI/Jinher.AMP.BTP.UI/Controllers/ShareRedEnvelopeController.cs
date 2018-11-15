using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.Portal.Common;
using Jinher.AMP.CBC.IBP.Facade;
using Jinher.AMP.CBC.Deploy;
using Jinher.AMP.CBC.Deploy.CustomDTO;
using Alipay.Class;
using System.Text;
using System.Security.Cryptography;
using Jinher.AMP.BTP.Common;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using Jinher.AMP.BTP.UI.Commons;
using Jinher.JAP.MVC.Cache;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.MVC.Controller;
using Jinher.JAP.MVC.UIJquery.DataGrid;
using Jinher.AMP.BTP.UI.Util;
using Jinher.AMP.BTP.UI.Filters;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public class ShareRedEnvelopeController : Jinher.JAP.MVC.Controller.BaseController
    {
        /// <summary>
        /// 红包列表 多个过滤器，先出现的后执行（先执行DealUInfoInShare，后执行CheckUserId）
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl]
        public ActionResult ShareRedEnvelopesList()
        {
            Guid userId = Guid.Empty;
            Guid.TryParse(Request["userId"], out userId);
            string appId = Request["appId"];
            string os = Request["os"];
            string sessionid = Request["sessionId"];
            int pageIndex = 1;
            int pageSize = 20;

            string source = Request.QueryString["source"];
            source = string.IsNullOrWhiteSpace(source) ? "" : source;

            if (userId == Guid.Empty || string.IsNullOrWhiteSpace(sessionid))
            {
                return View();
            }

            Jinher.AMP.BTP.ISV.Facade.ShareRedEnvelopeFacade srefacade = new ISV.Facade.ShareRedEnvelopeFacade();
            ViewBag.UserId = userId;
            ViewBag.ShareRedEnvelopesList = srefacade.GetMyRedEnvelope(new Guid(Request["userId"]), 0, pageIndex, pageSize);

            ViewBag.MobileType = os;
            ViewBag.AppId = appId;
            ViewBag.UserId = userId;
            ViewBag.sessionid = sessionid;
            ViewBag.source = source;
            return View();
        }
        /// <summary>
        /// ajax 请求页
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult GetShareRedEnvelopesList(Guid userId, int type, int pageIndex, int pageSize)
        {
            Jinher.AMP.BTP.ISV.Facade.ShareRedEnvelopeFacade srefacade = new ISV.Facade.ShareRedEnvelopeFacade();
            int spageIndex = pageIndex;
            int spageSize = pageSize;
            var shareredlist = srefacade.GetMyRedEnvelope(userId, type, pageIndex, pageSize);
            return this.Json(shareredlist);
        }

        /// <summary>
        /// ajax 请求页
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult GetOrgShareRedEnvelopesList(Guid userId, int type, int pageIndex, int pageSize)
        {
            Jinher.AMP.BTP.ISV.Facade.ShareRedEnvelopeFacade srefacade = new ISV.Facade.ShareRedEnvelopeFacade();
            int spageIndex = pageIndex;
            int spageSize = pageSize;
            var shareredlist = srefacade.GetMyOrgRedEnvelope(userId, type, pageIndex, pageSize);
            return this.Json(shareredlist);
        }

        /// <summary>
        /// 领取红包
        /// </summary>
        /// <param name="RedEnvelopesId">红包ID</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult ReceiveShareRedEnvelopes(Guid RedEnvelopesId)
        {
            Jinher.AMP.BTP.ISV.Facade.ShareRedEnvelopeFacade srefacade = new ISV.Facade.ShareRedEnvelopeFacade();
            var reslult = srefacade.DrawRedEnvelope(RedEnvelopesId);
            return this.Json(reslult);
        }


        /// <summary>
        /// 红包详细
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl]
        public ActionResult ShareRedEnvelopesDetail()
        {
            string msgId = Request["msgId"];
            string os = Request["os"];
            string isAnnon = Request["isannon"];

            string source = Request.QueryString["source"];
            source = string.IsNullOrWhiteSpace(source) ? "" : source;

            string _host = Request.Url.Scheme + "://" + Request.Url.Host;

            string url = Jinher.AMP.BTP.TPS.ShortUrlSV.Instance.GenShortUrl(_host + "/ShareRedEnvelope/ShareRedEnvelopesDetail?msgId=" + msgId + "&opentype=share&source=" + source);
            string shareImg = Request.Url.Scheme +"://fileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f965f/2014120912/d83a01e2-74e9-4f4f-bc1f-f64f2ab9d89a_2014120912421146568626.png";
            ViewBag.ShareUrl = url;
            ViewBag.ShareImg = shareImg;
            ViewBag.BTPAppresUrl = CustomConfig.BTPAppres;
            ViewBag.OS = os;
            ViewBag.IsAnnon = isAnnon;
            ViewBag.DividentDue = CustomConfig.SaleShare.DividentDue;
            ViewBag.DownAppId = CustomConfig.SaleShare.AppId;
            return View();
        }
        /// <summary>
        /// 红包详细
        /// </summary>
        /// <returns></returns>
        public ActionResult GetShareRedEnvelopes(Guid msgId)
        {
            Guid RedEnvelopesId = msgId;
            Jinher.AMP.BTP.ISV.Facade.ShareRedEnvelopeFacade srefacade = new ISV.Facade.ShareRedEnvelopeFacade();
            var result = srefacade.GetRedEnvelope(RedEnvelopesId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 规则说明
        /// </summary>
        /// <returns></returns>
        public ActionResult RuleDescription()
        {
            Guid gAppId = Guid.Empty;
            string appId = Request["appId"];
            if (!string.IsNullOrEmpty(appId))
            {
                gAppId = new Guid(appId);
            }
            Jinher.AMP.BTP.ISV.Facade.ShareRedEnvelopeFacade srefacade = new ISV.Facade.ShareRedEnvelopeFacade();
            RuleDescriptionDTO rdDto = new RuleDescriptionDTO();
            rdDto = srefacade.GetRuleDescription(gAppId);
            ViewBag.RuleDescription = rdDto;
            return View();
        }

        public ActionResult ShowRuleDescription()
        {
            Guid gAppId = Guid.Empty;
            string appId = Request["appId"];
            if (!string.IsNullOrEmpty(appId))
            {
                gAppId = new Guid(appId);
            }
            Jinher.AMP.BTP.ISV.Facade.ShareRedEnvelopeFacade srefacade = new ISV.Facade.ShareRedEnvelopeFacade();
            RuleDescriptionDTO rdDto = new RuleDescriptionDTO();
            rdDto = srefacade.GetRuleDescription(gAppId);
            ViewBag.RuleDescription = rdDto;
            return View();
        }

        public ActionResult ShowEditeRuleDescription()
        {
            Guid gAppId = Guid.Empty;
            string appId = Request["appId"];
            if (!string.IsNullOrEmpty(appId))
            {
                gAppId = new Guid(appId);
            }

            Jinher.AMP.BTP.ISV.Facade.ShareRedEnvelopeFacade srefacade = new ISV.Facade.ShareRedEnvelopeFacade();
            RuleDescriptionDTO rdDto = new RuleDescriptionDTO();
            rdDto = srefacade.GetRuleDescription(gAppId);
            ViewBag.RuleDescription = rdDto;
            return View();
        }


        public ActionResult AddRuleDescription(string Mess,Guid appId)
        {
            Jinher.AMP.BTP.ISV.Facade.ShareRedEnvelopeFacade srefacade = new ISV.Facade.ShareRedEnvelopeFacade();
            RuleDescriptionDTO rdDto = srefacade.GetRuleDescription(appId).Id == Guid.Empty ? new RuleDescriptionDTO { Id = Guid.NewGuid() } : srefacade.GetRuleDescription(appId);
            rdDto.Description = Mess;
            rdDto.appId = appId;
            ViewBag.RuleDescription = rdDto;
            var result = srefacade.UseRuleDescription(rdDto);
            return Json(new { Result = result.ResultCode, Messages = result.Message });

        }

        /// <summary>
        /// 众筹红包列表
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl]
        public ActionResult CrowdRedEnvelopesList()
        {
            Guid userId = new Guid(Request["userId"]);
            string appId = Request["appId"];
            string os = Request["os"];
            string sessionid = Request["sessionId"];
            int pageIndex = 1;
            int pageSize = 20;

            string source = Request.QueryString["source"];
            source = string.IsNullOrWhiteSpace(source) ? "" : source;

            Jinher.AMP.BTP.ISV.Facade.ShareRedEnvelopeFacade srefacade = new ISV.Facade.ShareRedEnvelopeFacade();
            ViewBag.UserId = userId;
            ViewBag.ShareRedEnvelopesList = srefacade.GetMyRedEnvelope(new Guid(Request["userId"]), 1, pageIndex, pageSize);

            ViewBag.MobileType = os;
            ViewBag.AppId = appId;
            ViewBag.UserId = userId;
            ViewBag.sessionid = sessionid;
            ViewBag.source = source;
            return View();
        }

        public ActionResult ShareList()
        {
            return View();
        }

        /// <summary>
        ///  获取众销列表
        /// </summary>
        /// <returns></returns>
        [GridAction]
        public ActionResult GetShareList(int pageSize, int pageIndex)
        {


            pageIndex = Request["page"] == null ? 0 : Convert.ToInt32(Request["page"]);
            pageSize = Request["rows"] == null ? 0 : Convert.ToInt32(Request["rows"]);
            Jinher.AMP.BTP.ISV.Facade.ShareRedEnvelopeFacade srefacade = new ISV.Facade.ShareRedEnvelopeFacade();

            var result = srefacade.GetShareList(pageSize, pageIndex);
            List<ShareItemDTO> list = new List<ShareItemDTO>();
            if (result == null)
                result = new ShareListResult();

            List<string> showList = new List<string>();
            showList.Add("ShareUserId");
            showList.Add("ShareUserCode");
            showList.Add("ShareUserName");
            showList.Add("ThirdPartName");
            showList.Add("TotalDividend");
            showList.Add("ShareDate");
            showList.Add("OrderCode");

            HttpCookie cookie = new HttpCookie("GridSum");
            cookie["SumUserCount"] = result.SumUserCount.ToString(CultureInfo.InvariantCulture);
            cookie["SumTotalDividend"] = result.SumTotalDividend.ToString(CultureInfo.InvariantCulture);

            this.Response.Cookies.Add(cookie);

            return View(new GridModel<ShareItemDTO>(showList, result.ShareItems, result.Count, pageIndex, string.Empty));

        }

    }
}
