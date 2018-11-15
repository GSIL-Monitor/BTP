using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.DSS.ISV.Facade;
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
using Jinher.AMP.Portal.Common;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.MVC.Cache;
using Jinher.JAP.Common.Loging;
using System.Web.Routing;
using Jinher.AMP.FSP.ISV.Facade;
using Jinher.AMP.App.Deploy.CustomDTO;
using Jinher.AMP.BTP.UI.Models;
using Jinher.AMP.FSP.Deploy.CustomDTO;
using Jinher.AMP.BTP.UI.Util;
using Jinher.AMP.BTP.UI.Filters;
using Jinher.AMP.Coupon.Deploy.CustomDTO;
using Jinher.AMP.Coupon.Deploy.Enum;
using System.Web.Script.Serialization;
using Jinher.AMP.BTP.ISV.Facade;
using Jinher.JAP.MVC.Controller;
using AppDownloadDTO = Jinher.AMP.BTP.Deploy.CustomDTO.AppDownloadDTO;


namespace Jinher.AMP.BTP.UI.Controllers
{
    /// <summary>
    /// MobileController 舌尖在线
    /// </summary>
    public partial class MobileController : BaseController
    {
        /// <summary>
        /// 点餐商品列表
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl(UrlNeedAppParams = UrlNeedAppParamsEnum.ShopId)]
        public ActionResult CYCommodityList()
        {
            ViewBag.NowTime = DateTime.Now.ToString();
            //return View("~/Views/CYMobile/CommodityList.cshtml");
            return View("~/Views/CYMobile/CyMain.cshtml");
        }

        public JsonResult CYGetCommodity(Guid appId, Guid userId)
        {
            return GetCateringCommodity(appId, userId);
        }

        public ActionResult CYCheckCommodity(Guid UserID, List<CommodityIdAndStockId> CommodityIdAndStockIds,
            Guid diyGroupId, int promotionType)
        {
            return CheckCommodity(UserID, CommodityIdAndStockIds, diyGroupId, promotionType, Guid.Empty);
        }
        /// <summary>
        /// 获取馆详情
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAppPavilionInfo(Jinher.AMP.ZPH.Deploy.CustomDTO.QueryAppPavilionParam param)
        {
            StoreFacade facade = new StoreFacade();
            return Json(new { ret = facade.GetAppPavilionInfo(param) });
        }

        /// <summary>
        /// 门店列表
        /// </summary>
        /// <returns></returns>
        [Filters.CheckParamType(IsCheckGuid = true)]
        [DealMobileUrl]
        public ActionResult Store(Guid appId)
        {
            Guid esAppId = appId;
            return View("~/Views/CYMobile/Store.cshtml");
        }
        /// <summary>
        /// 点餐商品列表
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl(UrlNeedAppParams = UrlNeedAppParamsEnum.ShopId)]
        public ActionResult CYCommodityListTest()
        {
            ViewBag.NowTime = DateTime.Now.ToString();
            return View("~/Views/CYMobile/CommodityListTest.cshtml");
        }
        /// <summary>
        /// 按门店领取优惠券
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public JsonResult GetCouponByShop(Guid shopId)
        {
            try
            {
                Jinher.AMP.Coupon.ISV.Facade.CouponFacade coupon = new Coupon.ISV.Facade.CouponFacade();
                var result = coupon.GetAppCouponCountInfo(shopId);
                if (result.Code == 0)
                {
                    return Json(result.Data.AppCount + result.Data.CommodityCount);
                }
                return Json(0);
            }
            catch (Exception ex)
            {
                return Json(-1);
            }
        }

        /// <summary>
        /// 获取门店信息列表。
        /// </summary>
        /// <param name="slp"></param>
        /// <returns></returns>
        public JsonResult GetStoreByLocation(StoreLocationParam slp)
        {
            try
            {
                ISV.Facade.StoreFacade storeFacade = new ISV.Facade.StoreFacade();
                var stores = storeFacade.GetCateringPlatformStore(slp);
                if (stores != null && stores.Stroes != null && stores.Stroes.Count > 0)
                {
                    var storeIds = stores.Stroes.Select(r => r.Id).ToList();
                    IBP.Facade.CateringSettingFacade settingBP = new IBP.Facade.CateringSettingFacade();
                    var settings = settingBP.GetCateringSettingByStoreIds(storeIds);
                    var tempStores = (from se in stores.Stroes
                                      join st in settings on se.Id equals st.CateringSetting.StoreId
                                      select new
                                      {
                                          store = se,
                                          setting = st
                                      }).ToList();

                    return Json(new { result = true, stores = tempStores, proviences = stores.Proviences });
                }
                return Json(new { result = false });
            }
            catch (Exception ex)
            {
                LogHelper.Error("CY调用ISV.Facade.StoreFacade.GetStoreByLocation异常，异常信息：", ex);
            }
            return Json(new { result = false });
        }

        /// <summary>
        /// 订单列表视图
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl]
        [WeixinOAuthOpenId]
        public ActionResult CYMyOrderList()
        {
            ViewBag.PortalUrl = CustomConfig.PortalUrl;
            return View("~/Views/CYMobile/MyOrderList.cshtml");
        }

        //private string GetStorePhone(Guid appId)
        //{
        //    var stores= new Jinher.AMP.BTP.IBP.Facade.StoreFacade().GetAppStore(appId);
        //    if (stores != null && stores.Count > 0) return stores[0].Phone;
        //    return "";
        //}

        private void GetStorePhoneStr(List<PhoneSDTO> Phone)
        {
            if (Phone == null || Phone.Count == 0) return;
            string p = "";
            Phone.ForEach(r =>
            {
                p += r.PhoneNumber + ",";
            });

            ViewBag.StorePhoneStr = p.TrimEnd(',');
        }

        public ActionResult CYMyOrderDetailShow()
        {
            ViewBag.StorePhoneStr = "";
            string oidStr = Request.QueryString["orderId"];
            Guid orderId = Guid.Empty;
            if (Guid.TryParse(oidStr, out orderId))
            {
                var result = GetOrderDetailsVM(orderId);
                if (result == null || result.data == null)
                {
                    ViewBag.Title = "订单详情";
                    ViewBag.Message = "查询失败";
                    return View("~/Views/Mobile/MobileError.cshtml");
                }
                ViewBag.Batch = result.data.Batch;
                ViewBag.AppName = result.AppName;
                ISV.Facade.StoreFacade settingBP = new ISV.Facade.StoreFacade();
                var _store = settingBP.GetOnlyStoreInApp(result.data.AppId);
                if (_store.Data != null)
                {
                    ViewBag.StoreName = _store.Data.StoreName;
                    ViewBag.StorePhone = _store.Data.Phone;
                    GetStorePhoneStr(_store.Data.Phone);
                }
                else
                {
                    ViewBag.StoreName = "数据加载失败";
                    ViewBag.StorePhone = "数据加载失败";
                }
                JavaScriptSerializer js = new JavaScriptSerializer();
                ViewBag.odJsonString = HttpUtility.UrlEncode(js.Serialize(result));
                return View("~/Views/CYMobile/MyOrderDetailShow.cshtml");
            }
            else
            {
                ViewBag.Title = "订单详情";
                ViewBag.Message = "查询失败";
                return View("~/Views/Mobile/MobileError.cshtml");
            }
        }
        /// <summary>
        /// 支付回调url
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public ActionResult CYOrderPayBack(Guid appId)
        {
            return Redirect("/Mobile/CYMyOrderList?appId=" + appId);
        }

        /// <summary>
        /// 订单详情视图
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl]
        public ActionResult CYMyOrderDetail()
        {
            ViewBag.StorePhoneStr = "";
            Guid userId = getLoginUserId();
            ViewBag.appId = Guid.Empty;

            CommodityOrderFacade orderFacade = new CommodityOrderFacade();

            string oidStr = Request.QueryString["orderId"];
            Guid orderId = Guid.Empty;
            if (!Guid.TryParse(oidStr, out orderId))
            {
                ViewBag.Title = "订单详情";
                ViewBag.Message = "查询失败";
                return View("~/Views/Mobile/MobileError.cshtml");
            }

            var orderCheckResult = orderFacade.GetOrderCheckInfo(new OrderQueryParamDTO { OrderId = orderId });
            if (orderCheckResult == null || orderCheckResult.ResultCode != 0 || orderCheckResult.Data == null)
            {
                ViewBag.Title = "订单详情";
                ViewBag.Message = "查询失败";
                return View("~/Views/Mobile/MobileError.cshtml");
            }
            ViewBag.appId = orderCheckResult.Data.AppId;
            ViewBag.Batch = orderCheckResult.Data.Batch;

            ISV.Facade.StoreFacade settingBP = new ISV.Facade.StoreFacade();
            var _store = settingBP.GetOnlyStoreInApp(orderCheckResult.Data.AppId);
            if (_store.Data != null)
            {
                ViewBag.StoreName = _store.Data.StoreName;
                ViewBag.StorePhone = _store.Data.Phone;
                GetStorePhoneStr(_store.Data.Phone);
            }
            else
            {
                ViewBag.StoreName = "数据加载失败";
                ViewBag.StorePhone = "数据加载失败";
            }

            //var result = GetOrderDetailsVM();
            //下单用户访问订单详情。
            if (orderCheckResult.Data.UserId == userId)
            {
                ViewBag.Org = this.ContextDTO.LoginOrg;
                ViewBag.FSPUrl = CustomConfig.FSPUrl;
                ViewBag.PortalUrl = CustomConfig.PortalUrl;
                ViewBag.PromotionUrl = CustomConfig.PromotionUrl;

                if (APPBP.IsFittedApp(orderCheckResult.Data.AppId))
                {
                    // bool hasReviewFunction = BACBP.CheckCommodityReview(orderCheckResult.Data.AppId);
                    // ViewBag.hasReviewFunction = hasReviewFunction;

                    return View("~/Views/CYMobile/MyOrderDetail.cshtml");
                }
                else
                {
                    return View("~/Views/CYMobile/MyOrderDetail.cshtml");
                }
            }
            else
            {
                var result = GetOrderDetailsVM(orderId);
                ViewBag.AppName = result.AppName;
                //非下单用户访问订单详情，只可预览，不可操作。
                return View("~/Views/CYMobile/MyOrderDetail.cshtml", result.data);
            }
        }

        /// <summary>
        /// 订单页
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl(UrlNeedAppParams = UrlNeedAppParamsEnum.ShopId)]  //原生商品详情跳转到下订单页面需要shopId
        public ActionResult CYCreateOrder()
        {
            var esAppId = MobileCookies.AppId;

            ViewBag.FSPUrl = CustomConfig.FSPUrl;
            ViewBag.PromotionUrl = CustomConfig.PromotionUrl;
            ViewBag.PortalUrl = CustomConfig.PortalUrl;
            ViewBag.UploadFileCommodityList = CustomConfig.UploadFileCommodityList;
            ViewBag.Contract1CommodityList = CustomConfig.Contract1CommodityList;
            ViewBag.Contract2CommodityList = CustomConfig.Contract2CommodityList;
            //协议
            var contractList = CustomConfig.RentAgreement.ConfigCollection.GetAll();
            List<Jinher.AMP.BTP.Deploy.CustomDTO.RentAgreementConfigDTO> contractDTOList =
                new List<RentAgreementConfigDTO>();
            if (contractList != null && contractList.Count > 0)
            {
                foreach (var item in contractList)
                {
                    RentAgreementConfigDTO model = new RentAgreementConfigDTO();
                    model.Id = item.Id;
                    model.Name = item.Name;
                    contractDTOList.Add(model);
                }
            }
            ViewBag.RentAgreement = HttpUtility.UrlEncode(JsonHelper.JsonSerializer(contractDTOList));

            var appSelfTakeWay = 0;
            var resultBAC = BACBP.CheckAppSelfTake(esAppId);
            if (resultBAC)
            {
                var resultZPH = ZPHSV.Instance.GetAppSelfTakeWay(esAppId);
                appSelfTakeWay = resultZPH;
            }
            else
            {
                appSelfTakeWay = 0;
            }
            ViewBag.AppSelfTakeWay = appSelfTakeWay;
            //0 非平台型app; 1 平台型app
            ViewBag.IsPlatformApp = ZPHSV.Instance.IsAppPavilion(esAppId) ? 1 : 0;
            return View("~/Views/CYMobile/CyCreateOrder.cshtml");

        }
    }
}
