using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.CBC.IBP.Facade;
using Jinher.AMP.CBC.Deploy;
using System.Web.Script.Serialization;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.MVC.Controller;
using Jinher.AMP.Coupon.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.TPS.Helper;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public partial class MobileController : BaseController
    {
        public ActionResult PayFinish(Guid appId, Guid orderId, Guid shopId)
        {
            return Redirect(string.Format("/Mobile/PaySuccess?appId={0}&orderId={1}&shopId={2}", appId, orderId, shopId));
        }

        private void SetShopHome()
        {
            var appIdstr = Request.QueryString["appId"];
            //拿不到appid 赋值易捷北京appid
            if (string.IsNullOrEmpty(appIdstr))
            {
                appIdstr = Convert.ToString(YJB.Deploy.CustomDTO.YJBConsts.YJAppId);
            }
            ViewBag.ShopHome = string.Format(CustomConfig.H5HomePage, string.IsNullOrEmpty(appIdstr) ? "" : appIdstr);
        }

        public ActionResult PaySuccess()
        {
            var orderId = Guid.Empty;
            ViewBag.EsAppId = Request.QueryString["appId"];
            ViewBag.ShopId = Request.QueryString["shopId"];
            ViewBag.OrderId = Request.QueryString["orderId"];
            SetShopHome();
            var isMainOrder = false;
            string out_trade_no = Request.QueryString["out_trade_no"];
            if (out_trade_no != null && Guid.TryParse(out_trade_no.Substring(0, 36), out orderId) && orderId != Guid.Empty)
            {
                Jinher.AMP.BTP.IBP.Facade.CommodityOrderFacade bpFacade = new Jinher.AMP.BTP.IBP.Facade.CommodityOrderFacade();
                var order = bpFacade.GetCommodityOrderInfo(orderId);
                ViewBag.EsAppId = order.EsAppId;
                ViewBag.ShopId = order.AppId;
                ViewBag.OrderId = order.Id;
                Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade facade = new ISV.Facade.CommodityOrderFacade();
                isMainOrder = facade.CheckIsMainOrder(orderId);
            }
            else if (Guid.TryParse(Request.QueryString["orderId"], out orderId) && orderId != Guid.Empty)
            {
                Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade facade = new ISV.Facade.CommodityOrderFacade();
                isMainOrder = facade.CheckIsMainOrder(orderId);
            }
            //调一下京东确认预占
            PaymentNotifyController PaymentNotify = new PaymentNotifyController();
            JdOrderHelper.UpdateJdorder(orderId);
            //调一下苏宁确认预占
            SuningSV.suning_govbus_confirmorder_add(orderId, isMainOrder);
            //调一下方正确认预占
            //FangZhengSV.FangZheng_Order_Confirm(orderId, isMainOrder);
            ViewBag.MainOrder = isMainOrder ? 1 : 0;
            return View("~/Views/MobileFitted/PaySuccess.cshtml");
        }

        /// <summary>
        /// 获取可用优惠券模板列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetUsableCouponTemplateList()
        {
            ViewBag.LoginUserId = ContextDTO.LoginUserID;
            ViewBag.SessionId = ContextDTO.SessionID;
            ViewBag.AppId = Guid.Empty;
            try
            {
                int pageIndex = Request["pageIndex"] == null ? 0 : int.Parse(Request["pageIndex"]);
                int pageSize = Request["pageSize"] == null ? 0 : int.Parse(Request["pageSize"]);
                ViewBag.PageNumber = pageIndex;
                var param = new Jinher.AMP.ZPH.Deploy.CustomDTO.QueryPavilionAppParam
                {
                    Id = Guid.Parse(Request.Params["appId"]),
                    pageIndex = 1,
                    pageSize = int.MaxValue
                };

                var appIds = new List<Guid>() { param.Id };
                //if (ZPHSV.Instance.IsAppPavilion(param.Id)) //判断是否是电商馆，或者正品o2o的
                //{
                //    var returnInfo = ZPHSV.Instance.GetPavilionApp(param);
                //    appIds = returnInfo.Data.Select(t => t.appId).ToList();
                //}
                ViewBag.AppId = param.Id;
                ViewBag.Coupons = GetCoupons(pageIndex, pageSize, appIds, ContextDTO.LoginUserID);

            }
            catch (Exception ex)
            {
                ViewBag.PageNumber = -1;
                ViewBag.Coupons = null;
            }
            return PartialView("~/Views/MobileFitted/CanUseCouponTemplateList.cshtml");
        }

        /// <summary>
        /// 获取优惠券
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="appIds"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private List<CouponTemplatDetailDTO> GetCoupons(int pageIndex, int pageSize, List<Guid> appIds, Guid userId)
        {
            CouponTemplateUsableRequestDTO requestDto = new CouponTemplateUsableRequestDTO()
            {
                CurrentPage = (pageIndex - 1) * pageSize,
                PageSize = pageSize,
                AppList = appIds,
                UserId = userId,
                CommodityId = Guid.Empty,
                UseCenter=2
            };
            var re = CouponSV.Instance.GetUsableCouponsTemplateList(requestDto, false);
            if (re != null)
            {
                return re.RecordCollection.ToList();
            }
            return null;
        }

        public ActionResult BindCoupon(Guid couponTemplateId)
        {
            if (couponTemplateId == Guid.Empty)
            {
                return Json(new { success = false, msg = "参数错误" });
            }
            Guid userId = ContextDTO != null ? ContextDTO.LoginUserID : Guid.Empty;
            if (userId == Guid.Empty)
            {
                return Json(new { success = false, msg = "Unauthorized" });
            }
            return DoBindCoupon(userId, couponTemplateId);
        }

        /// <summary>
        /// 调用优惠券接口绑定
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="couponTemplateId"></param>
        /// <returns></returns>
        private JsonResult DoBindCoupon(Guid userId, Guid couponTemplateId)
        {
            CouponCreateRequestDTO requestDto = new CouponCreateRequestDTO { BindUserId = userId, ConponTemplateId = couponTemplateId };
            var result = CouponSV.Instance.BindCouponToUser(requestDto);
            return Json(new { success = result.IsSuccess, msg = result.Info });
        }

        /// <summary>
        /// 分享订单获取分享数据
        /// </summary>
        /// <param name="orderId">订单ID，如果是拆单是主订单Id</param>
        /// <returns></returns>
        public ActionResult GetShareOrderInfo(Guid orderId)
        {
            try
            {
                var shareInfo = new CommodityOrderFacade().GetShareOrderInfoByOrderId(orderId);
                if (shareInfo == null)
                    return Json(new { success = false, msg = "分享失败" });
                return Json(new { success = true, share = shareInfo, msg = "分享成功" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "分享失败" });
            }
        }

        /// <summary>
        /// 获取支付成功的配置
        /// </summary>
        /// <param name="appId">馆ID，或 shopId</param>
        /// <returns></returns>
        public ActionResult GetShopSetting(Guid appId)
        {
            try
            {
                var appConfig = ZPHSV.GetOrderPayedConfig(appId);
                return Json(new { success = true, appConfig = appConfig });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "获取数据失败" });
            }
        }


        /// <summary>
        /// 获取支付成功的配置
        /// </summary>
        /// <param name="appId">馆ID，或 shopId</param>
        /// <returns></returns>
        public ActionResult GetCreateOrderTip(Guid appId)
        {
            try
            {
                //var orderTip = ZPHSV.GetCreateOrderTip(appId);
                var orderTip = CacheHelper.ZPH.GetCreateOrderTip(appId);
                return Json(new { success = true, orderTip = orderTip });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "获取数据失败" });
            }
        }



        /// <summary>
        /// 分享页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ShareOrder()
        {
            ViewBag.AppId = Request.QueryString["appId"];
            ViewBag.ShopId = Request.QueryString["shopId"];
            ViewBag.OrderId = Request.QueryString["orderId"];
            ViewBag.ShopHome = string.Format(CustomConfig.H5HomePage, Request.QueryString["appId"]);
            Guid _appId = Guid.Empty;
            Guid.TryParse(Request.QueryString["appId"], out _appId);
            var resultApp = APPSV.GetAppName(_appId);
            ViewBag.ShareOrderTitle = "订单分享";
            if (resultApp != null)
            {
                ViewBag.ShareOrderTitle = resultApp;
            }
            return View("~/Views/MobileFitted/ShareOrder.cshtml");
        }

        /// <summary>
        /// 获取分享订单详情
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public ActionResult GetShareOrderDetail(Guid orderId)
        {
            Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade orderfacade = new ISV.Facade.CommodityOrderFacade();
            CommodityOrderShareDTO order = orderfacade.GetShareOrderItems(orderId);
            string nickName = "";
            string userImg = "";
            try
            {
                Jinher.AMP.BTP.ISV.Facade.BTPUserFacade userSV = new ISV.Facade.BTPUserFacade();
                Jinher.AMP.BTP.Deploy.CustomDTO.UserSDTO commodityuser = userSV.GetUser(order.UserId, Guid.Empty);
                nickName = commodityuser.UserName;
                userImg = commodityuser.PicUrl;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("GetShareOrderDetail调CBC这个GetUserBasicInfoNew服务异常。orderId：{0}，userId：{1}",
                                  orderId.ToString(), order.UserId.ToString()), ex);
            }

            return Json(new { data = order, nickname = nickName, userimg = userImg }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CopyShareOrderToShoppingCart(Guid orderId, Guid userId, Guid appId)
        {
            Jinher.AMP.BTP.ISV.Facade.ShoppingCartFacade facade = new ISV.Facade.ShoppingCartFacade();
            var result = facade.CopyShareOrderToShoppingCart(userId, orderId, appId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
