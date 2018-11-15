using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.TPS;
using Jinher.JAP.MVC.Controller;
using Jinher.AMP.BTP.UI.Models;
using Jinher.AMP.App.Deploy.CustomDTO;
using Jinher.AMP.App.IBP.Facade;
using Jinher.AMP.App.Deploy.Enum;
using Jinher.AMP.App.Deploy;
using Jinher.AMP.BTP.Common;

namespace Jinher.AMP.BTP.UI.Controllers
{
    [Obsolete("最早的商品分享，已过期", false)]
    public class ShareController : BaseController
    {
        /// <summary>
        /// 标记是移动端访问还是PC端访问
        /// </summary>
        bool IsMobile;

        public ActionResult ShareList()
        {
            return View();
        }

        /// <summary>
        /// 分享商品
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        [Obsolete("最早的商品分享，已过期", false)]
        public ActionResult Commodity()
        {
            string id = Request.QueryString["id"];
            Guid commodityId;
            if (!Guid.TryParse(id, out commodityId))
            {
                return Content("您访问的链接已失效");
            }
            IsMobile = IsMoblie();
            ViewBag.AppSettUrl = CustomConfig.AppUrl;

            ShareQueryFacade comf = new ShareQueryFacade();
            CommodityDTO commodityDTO = comf.GetCommodity(commodityId);

            if (commodityDTO == null)
            {
                return Content("商品信息已删除");
            }

            //应用信息部分
            List<AppPackageDetailDTO> appPackageListDTO = GetApp(commodityDTO.AppId).AppPackageDetailList;
            AppPackageDetailDTO appPackageDTO = new AppPackageDetailDTO();

            if (appPackageListDTO != null)
            {
                appPackageDTO = appPackageListDTO.Where(c => c.HostType == HostTypeEnum.Android).FirstOrDefault();
                if (appPackageDTO != null)
                {
                    this.ViewBag.Icon = appPackageDTO.Icon;
                    this.ViewBag.AppName = appPackageDTO.AppPackageName;
                    this.ViewBag.CommentCount = appPackageDTO.DownloadCount;
                    this.ViewBag.QRCodeUrl = appPackageDTO.QRCodeUrl;
                    this.ViewBag.AppUrl = appPackageDTO.AppUrl;
                }
                else
                {
                    this.ViewBag.AppName = "应用信息未找到";
                    this.ViewBag.NoApp = true;
                }

                var IPhone = appPackageListDTO.Where(c => c.HostType == HostTypeEnum.Iphone).FirstOrDefault();
                if (IPhone != null && !string.IsNullOrEmpty(IPhone.AppUrl))
                {
                    var appUrlList = IPhone.AppUrl.Split(';');
                    if (appUrlList.Length > 0)
                    {
                        this.ViewBag.iPhoneUrl = IPhone.AppUrl.Split(';')[0];
                    }
                }
            }

            ViewBag.CommdityName = commodityDTO.Name;
            ViewBag.SalesNumber = commodityDTO.Salesvolume;
            ViewBag.CollectNumber = commodityDTO.TotalCollection;
            ViewBag.CommdityImage = commodityDTO.PicturesPath;
            ViewBag.AppId = commodityDTO.AppId;
            ViewBag.BTPAppresUrl = CustomConfig.BTPAppres;
            if (IsMobile == true)
            {
                return View("MobileView");
            }
            else
            {
                return View("PcView");
            }
        }

        /// <summary>
        /// 分享订单
        /// </summary>
        /// <returns></returns>
        public ActionResult Order()
        {
            string id = Request.QueryString["id"];
            Guid orderId;
            if (!Guid.TryParse(id, out orderId))
            {
                return Content("您访问的链接已失效");
            }

            IsMobile = IsMoblie();
            ViewBag.AppSettUrl = CustomConfig.AppUrl;

            ShareQueryFacade comf = new ShareQueryFacade();
            OrderForShareDTO orderForShareDTO = comf.GetOrderCommoditys(orderId);

            if (orderForShareDTO == null)
            {
                return Content("您访问的链接已失效");
            }

            OrderInfo order = new OrderInfo();

            Guid appId = orderForShareDTO.AppId;
            order.AppId = appId;
            order.UserName = string.IsNullOrEmpty(orderForShareDTO.UserName) ? "" : orderForShareDTO.UserName;
            order.UserPhoto = string.IsNullOrEmpty(orderForShareDTO.UserPhoto) ? "/Content/images/default_avatar.png" : orderForShareDTO.UserPhoto;
            order.Count = orderForShareDTO.Count;

            order.Days = orderForShareDTO.Days;
            order.RealPrice = orderForShareDTO.RealPrice;
            //应用信息部分
            List<AppPackageDetailDTO> appPackageListDTO = GetApp(appId).AppPackageDetailList;
            AppPackageDetailDTO appPackageDTO = new AppPackageDetailDTO();

            if (appPackageListDTO != null)
            {
                appPackageDTO = appPackageListDTO.Where(c => c.HostType == HostTypeEnum.Android).FirstOrDefault();
                if (appPackageDTO != null)
                {
                    this.ViewBag.Icon = appPackageDTO.Icon;
                    this.ViewBag.AppName = appPackageDTO.AppPackageName;
                    this.ViewBag.CommentCount = appPackageDTO.DownloadCount;
                    this.ViewBag.QRCodeUrl = appPackageDTO.QRCodeUrl;
                    this.ViewBag.AppUrl = appPackageDTO.AppUrl;
                }
                else
                {
                    this.ViewBag.AppName = "应用信息未找到";
                    this.ViewBag.NoApp = true;
                }

                var IPhone = appPackageListDTO.Where(c => c.HostType == HostTypeEnum.Iphone).FirstOrDefault();
                if (IPhone != null)
                {
                    this.ViewBag.iPhoneUrl = IPhone.AppUrl.Split(';')[0];
                }
            }

            if (orderForShareDTO.CommodityDTO != null)
            {
                CommodityDTO commodityDTO = new CommodityDTO();
                commodityDTO = orderForShareDTO.CommodityDTO;
                order.CommdityName = commodityDTO.Name;
                order.SalesNumber = commodityDTO.Salesvolume;
                order.CollectNumber = commodityDTO.TotalCollection;
                order.CommdityImage = commodityDTO.PicturesPath;

            }

            ViewBag.Order = order;
            if (IsMobile == true)
            {
                return View("OrderMobileView");
            }
            else
            {
                return View("OrderPcView");
            }
        }

        /// <summary>
        /// 获取应用信息
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public AppPackageDetailListDTO GetApp(Guid appId)
        {
            return APPBP.Instance.GetAppPackageDetailsWithHostTypeByAppId(appId);
        }

        /// <summary>
        /// 判断是手机端浏览器还是PC端浏览器
        /// </summary>
        public bool IsMoblie()
        {
            string agent = (Request.UserAgent + "").ToLower().Trim();
            if (agent == "" ||
                agent.IndexOf("mobile") != -1 ||
                agent.IndexOf("mobi") != -1 ||
                agent.IndexOf("nokia") != -1 ||
                agent.IndexOf("samsung") != -1 ||
                agent.IndexOf("sonyericsson") != -1 ||
                agent.IndexOf("mot") != -1 ||
                agent.IndexOf("blackberry") != -1 ||
                agent.IndexOf("lg") != -1 ||
                agent.IndexOf("htc") != -1 ||
                agent.IndexOf("j2me") != -1 ||
                agent.IndexOf("ucweb") != -1 ||
                agent.IndexOf("opera mini") != -1 ||
                agent.IndexOf("mobi") != -1 ||
                agent.IndexOf("android") != -1 ||
                agent.IndexOf("iphone") != -1)
            {
                //终端可能是手机
                return true;
            }
            return false;
        }



    }
}
