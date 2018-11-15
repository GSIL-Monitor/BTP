using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.BTP.TPS.Helper;
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
using System.Web.UI.WebControls;
using Jinher.AMP.BTP.IBP.Facade;
using AppDownloadDTO = Jinher.AMP.BTP.Deploy.CustomDTO.AppDownloadDTO;
using AppExtensionFacade = Jinher.AMP.BTP.ISV.Facade.AppExtensionFacade;
using AppSelfTakeStationFacade = Jinher.AMP.BTP.ISV.Facade.AppSelfTakeStationFacade;
using CommodityFacade = Jinher.AMP.BTP.ISV.Facade.CommodityFacade;
using CommodityOrderFacade = Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade;
using InvoiceFacade = Jinher.AMP.BTP.ISV.Facade.InvoiceFacade;
using System.Configuration;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.TPS.Invoic;
using Jinher.AMP.BTP.UI.Commons;
using System.Text.RegularExpressions;
using Jinher.AMP.LBP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.CustomDTO.AfterSales;
using System.Web.Caching;
using Jinher.JAP.Cache;
using Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee;
using Jinher.AMP.YJB.Deploy;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public partial class MobileController : Jinher.JAP.MVC.Controller.BaseController
    {


        [DealMobileUrl(UrlNeedAppParams = UrlNeedAppParamsEnum.ShopId)]
        [ArgumentExceptionDeal(Title = "所有商品")]
        public ActionResult getShopType(string[] appId, Guid EsAppId)
        {
            List<string> li = new List<string>();
            foreach (var item in appId)
            {
                if (CacheHelper.MallApply.GetMallTypeListByEsAppId(EsAppId).Any(_ => _.Id.ToString() == item && _.Type != 1))//如果这个商品所在的店铺是自营的
                {
                    li.Add("self");//自营
                }
                else
                    li.Add("tp");//第三方

            }
            return Json(li, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 商品列表页
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        [DealMobileUrl(UrlNeedAppParams = UrlNeedAppParamsEnum.ShopId)]
        [ArgumentExceptionDeal(Title = "所有商品")]
        public ActionResult CommodityList(Guid shopId, Guid? promotionId, Guid? yjCouponId)
        {
            ViewBag.IsShowAddCart = false;

            if (yjCouponId.HasValue)
            {
                ViewBag.YJCouponId = yjCouponId.Value;
            }

            ViewBag.IsYJCoupon = yjCouponId.HasValue;//优惠券进来的 //抵用券进来的

            if (MobileCookies.IsFittedApp())
            {
                ViewBag.ComListSetting = BACSV.GetComListSetting(WebUtil.GetEsAppId());
                return View("~/Views/MobileFitted/CommodityList.cshtml");
            }
            else
            {
                //免费app
                if (shopId != Guid.Empty)
                {
                    var appExtFacade = new Jinher.AMP.BTP.ISV.Facade.AppExtensionFacade();
                    var result = appExtFacade.GetAppExtensionByAppId(shopId);
                    if (result.ResultCode == 0 && result.Data != null)
                    {
                        ViewBag.IsShowAddCart = result.Data.IsShowAddCart;
                    }
                }
                return View("~/Views/Mobile/CommodityList.cshtml");
            }
        }



        /// <summary>
        /// 商品列表页
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        [DealMobileUrl(UrlNeedAppParams = UrlNeedAppParamsEnum.ShopId)]
        [ArgumentExceptionDeal(Title = "所有商品")]
        public ActionResult CommodityListCoupon(Guid shopId, Guid? promotionId, Guid? yjCouponId)
        {
            ViewBag.IsShowAddCart = false;
            if (yjCouponId.HasValue)
            {
                ViewBag.IsYJCoupon = true;
                ViewBag.YJCouponId = yjCouponId.Value;
            }
            else
            {
                ViewBag.IsYJCoupon = false;
            }



            if (MobileCookies.IsFittedApp())
            {
                var search = new CommodityListSearchDTO()
                {
                    AppId = new Guid(Request["appId"].ToString()),
                    FieldSort = 0,
                    IsHasStock = false,
                    OrderState = 1,
                    PageIndex = 1,
                    PageSize = 1
                };

                var facade = new ISV.Facade.MallApplyFacade();
                var result = facade.GetCommodityListV3(search);

                if (result.appInfoList != null && result.appInfoList.Count() > 0)
                    ViewBag.appName = result.appInfoList[0].appName;

                var lbt = new List<string[]>();
                foreach (var item in result.LBList.List)
                {
                    lbt.Add(new string[] { item.LinkUrl, item.ImageUrl });
                }
                if (result.LiveActivity != null)
                {
                    ViewBag.LiveActivity_Pic = result.LiveActivity.AppIcon;
                    ViewBag.LiveActivity_LiveName = result.LiveActivity.LiveName;
                    ViewBag.LiveActivity_AppName = result.LiveActivity.AppName;
                }
                else
                {
                    ViewBag.LiveActivity_Pic = "";
                    ViewBag.LiveActivity_LiveName = "";
                    ViewBag.LiveActivity_AppName = "";
                }


                ViewBag.ComListSetting = BACSV.GetComListSetting(WebUtil.GetEsAppId());
                return View("~/Views/MobileFitted/CommodityList.cshtml", lbt);
            }
            else
            {
                //免费app
                if (shopId != Guid.Empty)
                {
                    var appExtFacade = new Jinher.AMP.BTP.ISV.Facade.AppExtensionFacade();
                    var result = appExtFacade.GetAppExtensionByAppId(shopId);
                    if (result.ResultCode == 0 && result.Data != null)
                    {
                        ViewBag.IsShowAddCart = result.Data.IsShowAddCart;
                    }
                }
                return View("~/Views/Mobile/CommodityList.cshtml");
            }
        }
        public ActionResult Zhiliquan()
        {
            return View();
        }

        /// <summary>
        /// 订制应用，进店逛逛 获取商品列表
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ActionResult GetCommodityListV2(CommodityListSearchDTO search)
        {
            Jinher.AMP.BTP.ISV.Facade.CommodityFacade facade = new ISV.Facade.CommodityFacade();
            var ret = facade.GetCommodityListV2(search);
            return Json(ret, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 订制应用，进店逛逛 获取商品列表 轮播图片 直播信息
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ActionResult GetCommodityListV3(CommodityListSearchDTO search)
        {
            Jinher.AMP.BTP.ISV.Facade.MallApplyFacade facade = new ISV.Facade.MallApplyFacade();
            var ret = facade.GetCommodityListV3(search);
            return Json(ret, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 订制应用，进店逛逛 获取商品列表
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ActionResult GetCommodityListV2ForYJCoupon(CommodityListSearchDTO search)
        {
            Jinher.AMP.BTP.ISV.Facade.CommodityFacade facade = new ISV.Facade.CommodityFacade();
            var ret = facade.GetCommodityListV2(search);
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 订制应用，进店逛逛 获取商品列表
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ActionResult GetCommodityListV2ForCoupon(CommodityListSearchDTO search)
        {
            Jinher.AMP.BTP.ISV.Facade.CommodityFacade facade = new ISV.Facade.CommodityFacade();
            search.PageSize = Int32.MaxValue;
            var ret = facade.GetCommodityListV2(search);
            //获取优惠券模板对应的商品id
            Jinher.AMP.BTP.TPS.CouponSVFacade couponSvFacade = new Jinher.AMP.BTP.TPS.CouponSVFacade();
            var comIds = couponSvFacade.GetCouponGoodsList(search.CouponTemplateId);
            List<CommodityListCDTO> commodityList = ret.comdtyList.Where(t => comIds.Contains(t.Id)).ToList();
            ret.comdtyList = commodityList;
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 跳转的订单取消页面
        /// </summary>
        /// <param name="type">显示方式：1 只显示 多文本框；2 只显示退款单选内容；3 显示可选择的单选内容；4 显示退款方式</param>
        /// <param name="shopId">店铺Id</param>
        /// <param name="orderId">订单id</param>
        /// <param name="pri">订单的价格</param>
        /// <param name="state">订单当前状态</param>
        /// <param name="userId">用户id</param>
        /// <param name="pay">支付方式</param>
        /// <param name="spendScoreMoney">消费积分金额</param>
        /// <param name="spendYJCardPrice">易捷卡金额</param>
        /// <param name="spendYJBMoney"></param>
        /// <param name="orderItemId">订单商品详情id</param>
        /// <returns></returns>
        [DealMobileUrl]
        [ArgumentExceptionDeal(Title = "退款/退货申请")]
        public ActionResult RefundOrder(string type, Guid shopId, string orderId, string pri, string state, string userId, string pay, string spendScoreMoney, string spendYJBMoney, string spendCouponMoney, string orderItemId, string spendYJCardPrice)
        {
            decimal CurrPic = Convert.ToDecimal(pri);
            decimal SpendScoreMoney = Convert.ToDecimal(spendScoreMoney);
            decimal SpendCouponMoney = Convert.ToDecimal(spendCouponMoney);
            decimal SpendYJBMoney = Convert.ToDecimal(spendYJBMoney);
            decimal SpendYJCardPrice = Convert.ToDecimal(spendYJCardPrice);
            decimal yjCouponPrice = 0;
            ViewBag.appId = shopId;
            ViewBag.orderId = orderId;
            bool canOnlyRefund = true;
            var orderItemList = new List<OrderListItemCDTO>();
            if (string.IsNullOrEmpty(orderItemId) || orderItemId == "00000000-0000-0000-0000-000000000000")
            {
                var useryjcouponlist = YJBSV.GetUserYJCouponByOrderId(new Guid(orderId));
                if (useryjcouponlist.Data != null)
                {
                    foreach (var item in useryjcouponlist.Data)
                    {
                        yjCouponPrice += item.UsePrice;
                    }
                }
                if (state == "1" || state == "13")
                {
                    ViewBag.pic = (CurrPic + yjCouponPrice + SpendYJBMoney).ToString(CultureInfo.InvariantCulture);
                    //ViewBag.pic = (CurrPic + SpendScoreMoney + SpendYJBMoney - SpendCouponMoney).ToString(CultureInfo.InvariantCulture);
                }
                else if (state == "2" || state == "3")
                {
                    canOnlyRefund = false;
                    var commodityOrderFacade = new Jinher.AMP.BTP.IBP.Facade.CommodityOrderFacade();
                    var order = commodityOrderFacade.GetCommodityOrderInfo(new Guid(orderId));
                    LogHelper.Info("CurrPic " + CurrPic + " - order.Freight" + order.Freight);
                    //ViewBag.pic = (CurrPic + SpendScoreMoney + SpendYJBMoney - SpendCouponMoney - order.Freight).ToString(CultureInfo.InvariantCulture);
                    ViewBag.pic = (CurrPic + SpendScoreMoney + SpendYJBMoney + yjCouponPrice + SpendYJCardPrice - order.Freight).ToString(CultureInfo.InvariantCulture);
                }
                var cc = new CommodityOrderFacade();
                var orderItems = cc.GetOrderItems(new Guid(orderId), new Guid(userId), shopId);
                orderItemList = orderItems.ShoppingCartItemSDTO;
            }
            else
            {
                //单品退款
                var orderItems = new CommodityOrderFacade().GetOrderItems(new Guid(orderId), new Guid(userId), shopId);
                var orderItem = orderItems.ShoppingCartItemSDTO.FirstOrDefault(t => t.Id == new Guid(orderItemId));
                if (orderItem != null)
                {
                    CurrPic = (orderItem.RealPrice * orderItem.CommodityNumber);
                    if (CurrPic == 0)
                    {
                        CurrPic = (orderItem.DiscountPrice * orderItem.CommodityNumber);
                    }

                    if (state == "1" || state == "13")
                    {
                        // 待发货时最高退款金额=该商品实际售价-该商品承担的优惠券金额+该商品的运费-该商品承担的改价运费金额-该商品承担的改价商品金额+关税-易捷抵用券
                        //ViewBag.pic = (CurrPic - orderItem.CouponPrice + orderItem.FreightPrice - orderItem.ChangeFreightPrice - orderItem.ChangeRealPrice + orderItem.Duty - orderItem.YJCouponPrice).ToString(CultureInfo.InvariantCulture);
                        ViewBag.pic = (CurrPic - orderItem.CouponPrice + orderItem.FreightPrice - orderItem.ChangeFreightPrice - orderItem.ChangeRealPrice + orderItem.Duty).ToString(CultureInfo.InvariantCulture);
                    }
                    else if (state == "2" || state == "3")
                    {
                        canOnlyRefund = false;
                        //已发货时默认退款金额=该商品实际售价-该商品承担的优惠券金额-该商品承担的改价商品金额—关税-易捷抵用券
                        //ViewBag.pic = (CurrPic - orderItem.CouponPrice - orderItem.ChangeRealPrice - orderItem.Duty - orderItem.YJCouponPrice).ToString(CultureInfo.InvariantCulture);
                        ViewBag.pic = (CurrPic - orderItem.CouponPrice - orderItem.ChangeRealPrice - orderItem.Duty).ToString(CultureInfo.InvariantCulture);
                    }
                    orderItemList = new List<OrderListItemCDTO> { orderItem };
                }
            }
            ViewBag.orderItemList = orderItemList;
            ViewBag.state = state;
            ViewBag.pay = pay;
            ViewBag.CanOnlyRefund = canOnlyRefund || !ThirdECommerceHelper.IsWangYiYanXuan(shopId);
            return View();
        }

        /// <summary>
        /// 退货退款页面
        /// </summary>
        /// <param name="type"></param>
        /// <param name="shopId"></param>
        /// <param name="orderId"></param>
        /// <param name="pri"></param>
        /// <param name="state"></param>
        /// <param name="userId"></param>
        /// <param name="pay"></param>
        /// <param name="spendScoreMoney"></param>
        /// <param name="spendYJBMoney"></param>
        /// <param name="orderItemId"></param>
        /// <returns></returns>
        [DealMobileUrl]
        [ArgumentExceptionDeal(Title = "退款/退货申请")]
        public ActionResult ReEchrfundOrder(string shopId, string orderId, string pri, string state, string userId, string pay, string spendScoreMoney, string spendYJBMoney, string orderItemId)
        {
            decimal CurrPic = Convert.ToDecimal(pri);
            decimal SpendScoreMoney = Convert.ToDecimal(spendScoreMoney);
            decimal SpendYJBMoney = Convert.ToDecimal(spendYJBMoney);
            ViewBag.appId = shopId;
            ViewBag.orderId = orderId;
            List<JdComponentExport> customerExpects = new List<JdComponentExport>();
            if (String.IsNullOrEmpty(orderItemId))
            {
                return Content("参数不正确！请正常操作！");
            }

            if (string.IsNullOrEmpty(orderItemId))
            {
                ViewBag.pic = (CurrPic + SpendScoreMoney + SpendYJBMoney).ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                //单品退款
                CommodityOrderFacade commodityOrderFacade = new CommodityOrderFacade();
                var orderItems = commodityOrderFacade.GetOrderItems(new Guid(orderId), new Guid(userId), new Guid(shopId));
                ViewBag.OrderInfo = orderItems;
                var orderItem = orderItems.ShoppingCartItemSDTO.FirstOrDefault(t => t.Id == new Guid(orderItemId));
                if (orderItem != null)
                {
                    CurrPic = (orderItem.RealPrice * orderItem.CommodityNumber);
                    if (CurrPic == 0)
                    {
                        CurrPic = (orderItem.DiscountPrice * orderItem.CommodityNumber);
                    }
                    if (state == "1")
                    {
                        // 待发货时最高退款金额=该商品实际售价-该商品承担的优惠券金额+该商品的运费-该商品承担的改价运费金额-该商品承担的改价商品金额+关税

                        //ViewBag.pic = (CurrPic - orderItem.CouponPrice + orderItem.FreightPrice - orderItem.ChangeFreightPrice - orderItem.ChangeRealPrice + orderItem.Duty - orderItem.YJCouponPrice).ToString(CultureInfo.InvariantCulture);
                        ViewBag.pic = (CurrPic - orderItem.CouponPrice + orderItem.FreightPrice - orderItem.ChangeFreightPrice - orderItem.ChangeRealPrice + orderItem.Duty).ToString(CultureInfo.InvariantCulture);
                    }
                    else if (state == "2" || state == "3")
                    {
                        //已发货时默认退款金额=该商品实际售价-该商品承担的优惠券金额-该商品承担的改价商品金额—关税
                        //ViewBag.pic = (CurrPic - orderItem.CouponPrice - orderItem.ChangeRealPrice - orderItem.Duty - orderItem.YJCouponPrice).ToString(CultureInfo.InvariantCulture);
                        ViewBag.pic = (CurrPic - orderItem.CouponPrice - orderItem.ChangeRealPrice - orderItem.Duty).ToString(CultureInfo.InvariantCulture);
                    }

                    #region 获取是京东的数据内容
                    //var jdorderitemfacade = new Jinher.AMP.BTP.IBP.Facade.JdOrderItemFacade();
                    //JdOrderItemDTO model = new JdOrderItemDTO();
                    //model.CommodityOrderId = orderItem.OrderId.ToString();
                    //model.TempId = orderItem.CommodityId;
                    //var jdorderitemlist = jdorderitemfacade.GetJdOrderItemList(model).ToList();
                    //if (jdorderitemlist.Count() > 0)
                    //{
                    //    var jdorderitem = jdorderitemlist[0];
                    //    customerExpects = JDSV.GetCustomerExpectComp(jdorderitem.JdOrderId, jdorderitem.CommoditySkuId);
                    //}
                    //else
                    //{
                    //    throw new Exception("非京东订单");
                    //}
                    #endregion
                }
            }
            ViewBag.state = state;
            ViewBag.pay = pay;
            ViewBag.CustomerExpects = customerExpects;
            return View();
        }

        /// <summary>
        /// 检查京东订单是否可以退款
        /// </summary>
        /// <returns></returns>
        public ActionResult CheckJdRefundIsAvailable(string jdorderId, string skuId)
        {
            var result = JDSV.GetAvailableNumberComp(jdorderId, skuId);
            if (result)
            {
                var customerExpects = JDSV.GetCustomerExpectComp(jdorderId, skuId);
                if (customerExpects.Any(_ => _.Code == "10"))
                {
                    return Json(new ResultDTO { isSuccess = true, Message = "可以退款" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new ResultDTO { isSuccess = false, Message = "不支持退货" + JsonConvert.SerializeObject(customerExpects) }, JsonRequestBehavior.AllowGet);
            }
            return Json(new ResultDTO { isSuccess = false, Message = "不可以退款" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 京东退款页面
        /// </summary>
        /// <param name="type">显示方式：1 只显示 多文本框；2 只显示退款单选内容；3 显示可选择的单选内容；4 显示退款方式</param>
        /// <param name="shopId">店铺Id</param>
        /// <param name="orderId">订单id</param>
        /// <param name="pri">订单的价格</param>
        /// <param name="state">订单当前状态</param>
        /// <param name="userId">用户id</param>
        /// <param name="pay">支付方式</param>
        /// <param name="spendScoreMoney">消费积分金额</param>
        /// <param name="spendYJBMoney"></param>
        /// <param name="orderItemId">订单商品详情id</param>
        /// <returns></returns>
        [DealMobileUrl]
        [ArgumentExceptionDeal(Title = "退款/退货申请")]
        public ActionResult RefundJdOrder(string type, string shopId, string orderId, string pri, string state, string userId, string pay, string spendScoreMoney, string spendYJBMoney, string orderItemId)
        {
            decimal CurrPic = Convert.ToDecimal(pri);
            decimal SpendScoreMoney = Convert.ToDecimal(spendScoreMoney);
            decimal SpendYJBMoney = Convert.ToDecimal(spendYJBMoney);
            ViewBag.appId = shopId;
            ViewBag.orderId = orderId;
            List<JdComponentExport> customerExpects = new List<JdComponentExport>();
            var orderItemList = new List<OrderListItemCDTO>();
            if (string.IsNullOrEmpty(orderItemId))
            {
                ViewBag.pic = (CurrPic + SpendScoreMoney + SpendYJBMoney).ToString(CultureInfo.InvariantCulture);
                var cc = new CommodityOrderFacade();
                var orderItems = cc.GetOrderItems(new Guid(orderId), new Guid(userId), new Guid(shopId));
                orderItemList = orderItems.ShoppingCartItemSDTO;
            }
            else
            {
                //单品退款
                CommodityOrderFacade commodityOrderFacade = new CommodityOrderFacade();
                var orderItems = commodityOrderFacade.GetOrderItems(new Guid(orderId), new Guid(userId), new Guid(shopId));
                ViewBag.OrderInfo = orderItems;
                var orderItem = orderItems.ShoppingCartItemSDTO.FirstOrDefault(t => t.Id == new Guid(orderItemId));
                if (orderItem != null)
                {
                    CurrPic = (orderItem.RealPrice * orderItem.CommodityNumber);
                    if (CurrPic == 0)
                    {
                        CurrPic = (orderItem.DiscountPrice * orderItem.CommodityNumber);
                    }
                    if (state == "1")
                    {
                        // 待发货时最高退款金额=该商品实际售价-该商品承担的优惠券金额+该商品的运费-该商品承担的改价运费金额-该商品承担的改价商品金额+关税

                        //ViewBag.pic = (CurrPic - orderItem.CouponPrice + orderItem.FreightPrice - orderItem.ChangeFreightPrice - orderItem.ChangeRealPrice + orderItem.Duty - orderItem.YJCouponPrice).ToString(CultureInfo.InvariantCulture);
                        ViewBag.pic = (CurrPic - orderItem.CouponPrice + orderItem.FreightPrice - orderItem.ChangeFreightPrice - orderItem.ChangeRealPrice + orderItem.Duty).ToString(CultureInfo.InvariantCulture);
                    }
                    else if (state == "2" || state == "3")
                    {
                        //已发货时默认退款金额=该商品实际售价-该商品承担的优惠券金额-该商品承担的改价商品金额—关税
                        //ViewBag.pic = (CurrPic - orderItem.CouponPrice - orderItem.ChangeRealPrice - orderItem.Duty - orderItem.YJCouponPrice).ToString(CultureInfo.InvariantCulture);
                        ViewBag.pic = (CurrPic - orderItem.CouponPrice - orderItem.ChangeRealPrice - orderItem.Duty).ToString(CultureInfo.InvariantCulture);
                    }

                    #region 获取是京东的数据内容
                    var jdorderitemfacade = new Jinher.AMP.BTP.IBP.Facade.JdOrderItemFacade();
                    JdOrderItemDTO model = new JdOrderItemDTO();
                    model.CommodityOrderId = orderItem.OrderId.ToString();
                    model.TempId = orderItem.CommodityId;
                    var jdorderitemlist = jdorderitemfacade.GetJdOrderItemList(model).ToList();
                    if (jdorderitemlist.Count() > 0)
                    {
                        var jdorderitem = jdorderitemlist[0];
                        customerExpects = JDSV.GetCustomerExpectComp(jdorderitem.JdOrderId, jdorderitem.CommoditySkuId);
                    }
                    else
                    {
                        throw new Exception("订单异常。");
                    }
                    #endregion

                    orderItemList = new List<OrderListItemCDTO> { orderItem };
                }
            }
            ViewBag.orderItemList = orderItemList;
            ViewBag.state = state;
            ViewBag.pay = pay;
            ViewBag.CustomerExpects = customerExpects;
            return View();
        }

        /// <summary>
        /// 京东退款页面
        /// </summary>
        /// <param name="type">显示方式：1 只显示 多文本框；2 只显示退款单选内容；3 显示可选择的单选内容；4 显示退款方式</param>
        /// <param name="shopId">店铺Id</param>
        /// <param name="orderId">订单id</param>
        /// <param name="pri">订单的价格</param>
        /// <param name="state">订单当前状态</param>
        /// <param name="userId">用户id</param>
        /// <param name="pay">支付方式</param>
        /// <param name="spendScoreMoney">消费积分金额</param>
        /// <param name="spendYJBMoney"></param>
        /// <param name="orderItemId">订单商品详情id</param>
        /// <returns></returns>
        [DealMobileUrl]
        [ArgumentExceptionDeal(Title = "退款/退货申请")]
        public ActionResult RefundJdOrder2(string type, string shopId, string orderId, string pri, string state, string userId, string pay, string spendScoreMoney, string spendYJBMoney, string orderItemId)
        {
            decimal CurrPic = Convert.ToDecimal(pri);
            decimal SpendScoreMoney = Convert.ToDecimal(spendScoreMoney);
            decimal SpendYJBMoney = Convert.ToDecimal(spendYJBMoney);
            ViewBag.appId = shopId;
            ViewBag.orderId = orderId;
            List<JdComponentExport> customerExpects = new List<JdComponentExport>();
            var orderItemList = new List<OrderListItemCDTO>();
            if (string.IsNullOrEmpty(orderItemId))
            {
                ViewBag.pic = (CurrPic + SpendScoreMoney + SpendYJBMoney).ToString(CultureInfo.InvariantCulture);
                var cc = new CommodityOrderFacade();
                var orderItems = cc.GetOrderItems(new Guid(orderId), new Guid(userId), new Guid(shopId));
                orderItemList = orderItems.ShoppingCartItemSDTO;
            }
            else
            {
                //单品退款
                CommodityOrderFacade commodityOrderFacade = new CommodityOrderFacade();
                var orderItems = commodityOrderFacade.GetOrderItems(new Guid(orderId), new Guid(userId), new Guid(shopId));
                ViewBag.OrderInfo = orderItems;
                var orderItem = orderItems.ShoppingCartItemSDTO.FirstOrDefault(t => t.Id == new Guid(orderItemId));
                if (orderItem != null)
                {
                    CurrPic = (orderItem.RealPrice * orderItem.CommodityNumber);
                    if (CurrPic == 0)
                    {
                        CurrPic = (orderItem.DiscountPrice * orderItem.CommodityNumber);
                    }
                    if (state == "1")
                    {
                        // 待发货时最高退款金额=该商品实际售价-该商品承担的优惠券金额+该商品的运费-该商品承担的改价运费金额-该商品承担的改价商品金额+关税

                        //ViewBag.pic = (CurrPic - orderItem.CouponPrice + orderItem.FreightPrice - orderItem.ChangeFreightPrice - orderItem.ChangeRealPrice + orderItem.Duty - orderItem.YJCouponPrice).ToString(CultureInfo.InvariantCulture);
                        ViewBag.pic = (CurrPic - orderItem.CouponPrice + orderItem.FreightPrice - orderItem.ChangeFreightPrice - orderItem.ChangeRealPrice + orderItem.Duty).ToString(CultureInfo.InvariantCulture);
                    }
                    else if (state == "2" || state == "3")
                    {
                        //已发货时默认退款金额=该商品实际售价-该商品承担的优惠券金额-该商品承担的改价商品金额—关税
                        //ViewBag.pic = (CurrPic - orderItem.CouponPrice - orderItem.ChangeRealPrice - orderItem.Duty - orderItem.YJCouponPrice).ToString(CultureInfo.InvariantCulture);
                        ViewBag.pic = (CurrPic - orderItem.CouponPrice - orderItem.ChangeRealPrice - orderItem.Duty).ToString(CultureInfo.InvariantCulture);
                    }

                    #region 获取是京东的数据内容
                    var jdorderitemfacade = new Jinher.AMP.BTP.IBP.Facade.JdOrderItemFacade();
                    JdOrderItemDTO model = new JdOrderItemDTO();
                    model.CommodityOrderId = orderItem.OrderId.ToString();
                    model.TempId = orderItem.CommodityId;
                    var jdorderitemlist = jdorderitemfacade.GetJdOrderItemList(model).ToList();
                    if (jdorderitemlist.Count() > 0)
                    {
                        var jdorderitem = jdorderitemlist[0];
                        customerExpects = JDSV.GetCustomerExpectComp(jdorderitem.JdOrderId, jdorderitem.CommoditySkuId);
                    }
                    else
                    {
                        throw new Exception("订单异常。");
                    }
                    #endregion

                    orderItemList = new List<OrderListItemCDTO> { orderItem };
                }
            }
            ViewBag.state = state;
            ViewBag.pay = pay;
            ViewBag.CustomerExpects = customerExpects;
            return View();
        }

        #region 退款页面---苏宁
        /// <summary>
        /// 京东退款页面
        /// </summary>
        /// <param name="type">显示方式：1 只显示 多文本框；2 只显示退款单选内容；3 显示可选择的单选内容；4 显示退款方式</param>
        /// <param name="shopId">店铺Id</param>
        /// <param name="orderId">订单id</param>
        /// <param name="pri">订单的价格</param>
        /// <param name="state">订单当前状态</param>
        /// <param name="userId">用户id</param>
        /// <param name="pay">支付方式</param>
        /// <param name="spendScoreMoney">消费积分金额</param>
        /// <param name="spendYJBMoney"></param>
        /// <param name="orderItemId">订单商品详情id</param>
        /// <returns></returns>
        [DealMobileUrl]
        [ArgumentExceptionDeal(Title = "退款/退货申请")]
        public ActionResult RefundSNOrder(string type, string shopId, string orderId, string pri, string state, string userId, string pay, string spendScoreMoney, string spendYJBMoney, string orderItemId)
        {
            // 取件方式(必填 1 上门取件-非厂送-自营    2快递寄回-厂送 )
            ViewBag.supplier = SNFactoryDeliveryEnum.NonFactoryDelivery.GetHashCode().ToString();
            decimal CurrPic = Convert.ToDecimal(pri);
            decimal SpendScoreMoney = Convert.ToDecimal(spendScoreMoney);
            decimal SpendYJBMoney = Convert.ToDecimal(spendYJBMoney);
            ViewBag.appId = shopId;
            ViewBag.orderId = orderId;
            List<SNComponentExport> customerExpects = new List<SNComponentExport>();
            var orderItemList = new List<OrderListItemCDTO>();
            //****测试***********88数据
            //customerExpects.Add(new SNComponentExport() { Code = "10", Name = "退款/退货" });

            if (string.IsNullOrEmpty(orderItemId))
            {
                ViewBag.pic = (CurrPic + SpendScoreMoney + SpendYJBMoney).ToString(CultureInfo.InvariantCulture);
                var cc = new CommodityOrderFacade();
                var orderItems = cc.GetOrderItems(new Guid(orderId), new Guid(userId), new Guid(shopId));
                orderItemList = orderItems.ShoppingCartItemSDTO;
            }
            else
            {
                //单品退款
                CommodityOrderFacade commodityOrderFacade = new CommodityOrderFacade();
                var orderItems = commodityOrderFacade.GetOrderItems(new Guid(orderId), new Guid(userId), new Guid(shopId));
                ViewBag.OrderInfo = orderItems;
                var orderItem = orderItems.ShoppingCartItemSDTO.FirstOrDefault(t => t.Id == new Guid(orderItemId));
                if (orderItem != null)
                {

                    CurrPic = (orderItem.RealPrice * orderItem.CommodityNumber);
                    if (CurrPic == 0)
                    {
                        CurrPic = (orderItem.DiscountPrice * orderItem.CommodityNumber);
                    }
                    if (state == "1")
                    {
                        // 待发货时最高退款金额=该商品实际售价-该商品承担的优惠券金额+该商品的运费-该商品承担的改价运费金额-该商品承担的改价商品金额+关税

                        //ViewBag.pic = (CurrPic - orderItem.CouponPrice + orderItem.FreightPrice - orderItem.ChangeFreightPrice - orderItem.ChangeRealPrice + orderItem.Duty).ToString(CultureInfo.InvariantCulture);
                        ViewBag.pic = (CurrPic + orderItem.FreightPrice - orderItem.ChangeFreightPrice - orderItem.ChangeRealPrice + orderItem.Duty).ToString(CultureInfo.InvariantCulture);
                    }
                    else if (state == "2" || state == "3")
                    {
                        //已发货时默认退款金额=该商品实际售价-该商品承担的优惠券金额-该商品承担的改价商品金额—关税
                        //ViewBag.pic = (CurrPic - orderItem.CouponPrice - orderItem.ChangeRealPrice - orderItem.Duty).ToString(CultureInfo.InvariantCulture);
                        ViewBag.pic = (CurrPic - orderItem.ChangeRealPrice - orderItem.Duty).ToString(CultureInfo.InvariantCulture);
                    }


                    #region 获取是苏宁的数据内容   增加苏宁退款状态判断

                    //根据订单的状态展示退款方式（仅退款or 退款退货）
                    var snOrderItemFacade = new Jinher.AMP.BTP.IBP.Facade.SNAfterSaleFacade();
                    SNOrderItemDTO snModel = new SNOrderItemDTO();
                    snModel.OrderId = orderItem.OrderId;
                    var snOrderItemList = snOrderItemFacade.GetSNOrderItemList(snModel).ToList();
                    if (snOrderItemList.Count() > 0)
                    {
                        var snOrderItem = snOrderItemList[0];
                        //查询苏宁支持的退货方式
                        SNOrderStatusDTO orderStatus = SuningSV.SNGetOrderStatus(snOrderItem.CustomOrderId);

                        if (orderStatus != null)
                        {
                            //待发货，仅退款
                            if (orderStatus.OrderStatus.Equals("2"))
                            {
                                // customerExpects.Add(new SNComponentExport() { Code = orderStatus.OrderStatus, Name = "仅退款" });
                                customerExpects.Add(new SNComponentExport() { Code = "10", Name = "仅退款" });
                            }
                            //待收货和已完成  退款退货
                            else if (orderStatus.OrderStatus.Equals("3") || orderStatus.OrderStatus.Equals("4"))
                            {
                                customerExpects.Add(new SNComponentExport() { Code = "10", Name = "退款/退货" });
                                //customerExpects.Add(new SNComponentExport() { Code = orderStatus.OrderStatus, Name = "退款/退货" });
                            }
                        }



                        #region //厂送 1  非厂送  2

                        //SNOrderAfterSalesHelper.SNJudgeIsFactoryDeliveryByOrderId(new Guid());
                        //取件方式(必填 1 上门取件-非厂送-自营    2快递寄回-厂送 )
                        List<SNApplyRejectedSkusDTO> list = new List<SNApplyRejectedSkusDTO>() { new SNApplyRejectedSkusDTO() { SkuId = snOrderItem.CustomSkuId } };
                        var factoryDeliveryType = SNOrderAfterSalesHelper.SNJudgeIsFactoryDelivery(orderItems.Province, orderItems.City, list);
                        if (factoryDeliveryType.Any())
                        {
                            if (factoryDeliveryType.FirstOrDefault().IsFactorySend)
                            {
                                ViewBag.supplier = SNFactoryDeliveryEnum.FactoryDelivery.GetHashCode().ToString();
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        LogHelper.Error(string.Format("MobileController.RefundSNOrder 【苏宁-售后】为查询到苏宁订单数据【OrderId={0}】", orderItem.OrderId));

                    }

                    #endregion

                    orderItemList = new List<OrderListItemCDTO> { orderItem };

                }
            }


            ViewBag.orderItemList = orderItemList;
            ViewBag.state = state;
            ViewBag.pay = pay;
            ViewBag.CustomerExpects = customerExpects;
            return View();
        }


        #endregion



        /// <summary>
        /// 保存用户退款退货退单等操作
        /// </summary>
        /// <param name="type">保存的方式： 1 多文本框内容；2 退款单选内容；3 可选择的单选内容；4 退款方式</param>
        /// <param name="RefundExpCo">type 为4时，传入选择或者输入的快递名称，其他状态可不传</param>
        /// <param name="RefundExpOrderNo">type 为4时，传入输入的快递单号，其他状态可不传</param>
        /// <param name="appId">appid</param>
        /// <param name="state">订单当前状态</param>
        /// <param name="orderId">订单编号</param>
        /// <param name="pic">type为 2或者3时 传入上传的凭证，暂时还没做，其他可不传</param>
        /// <param name="money">type为 2或者3时 传入退款退货的金额，其他可不传</param>
        /// <param name="dec">详细描述</param>
        /// <param name="refundReason">type为 2或者3时 传入选择原因</param>
        /// <param name="userId">用户id</param>
        /// <param name="pay">支付类型</param>
        /// <param name="refundType">1 :仅退款； 2 退款退货 (实际项目里面的逻辑是，0 仅退款；1 退款退货；代码里已经做了处理)</param>
        /// <param name="orderItemId">退款商品详情id</param>
        /// <returns></returns> 
        public ActionResult SaveRefundOrder(string type, string RefundExpCo = "", string RefundExpOrderNo = "",
            string appId = "", string state = "", string orderId = "", string pic = "",
            string money = "", string dec = "", string refundReason = "",
            string userId = "", string pay = "", string refundType = "", string orderItemId = "")
        {
            BTP.ISV.Facade.CommodityOrderFacade orderSV = new BTP.ISV.Facade.CommodityOrderFacade();


            ResultDTO result = new ResultDTO();
            if (type == "1")
            {
                result = orderSV.UpdateCommodityOrder(int.Parse(state), Guid.Parse(orderId), Guid.Parse(userId),
                    Guid.Parse(appId), int.Parse(pay), "", dec);
            }
            // 不清楚type是什么意思，暂定 type=2 为未发货退款 type=3 为已发货退款
            else if (type == "2" || type == "3")
            {
                SubmitOrderRefundDTO modelParam = new SubmitOrderRefundDTO();
                modelParam.commodityorderId = Guid.Parse(orderId);
                modelParam.Id = Guid.Parse(orderId);
                modelParam.RefundDesc = dec;
                modelParam.RefundExpCo = RefundExpCo;
                modelParam.RefundExpOrderNo = RefundExpOrderNo;
                modelParam.RefundMoney = decimal.Parse(money);
                modelParam.State = int.Parse(state);
                modelParam.RefundReason = refundReason;

                modelParam.RefundType = refundType == "1" ? 0 : 1;
                modelParam.OrderRefundImgs = pic;
                modelParam.OrderItemId = Guid.Parse(orderItemId);

                result = orderSV.SubmitOrderRefund(modelParam);
            }
            // 添加物流信息？
            else if (type == "4")
            {
                // { ResultCode = 0, Message = "Success" };
                result = null;//orderSV.AddOrderRefundExp(Guid.Parse(orderId), RefundExpCo, RefundExpOrderNo, Guid.Parse(orderItemId));
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private string UploadRefundFile(string Imgs)
        {
            string[] imgList = Imgs.Split(',');
            List<Jinher.JAP.BaseApp.FileServer.Deploy.CustomDTO.FileDTO> fileDTOs =
                new List<JAP.BaseApp.FileServer.Deploy.CustomDTO.FileDTO>();

            foreach (string imgUrl in imgList)
            {
                string path = Server.MapPath(imgUrl);
                using (FileStream stream = new FileStream(path, FileMode.Open))
                {
                    Jinher.JAP.BaseApp.FileServer.Deploy.CustomDTO.FileDTO fileDTO =
                        new JAP.BaseApp.FileServer.Deploy.CustomDTO.FileDTO();
                    fileDTO.UploadFileName = imgUrl.Substring(imgUrl.LastIndexOf('\\') + 1);
                    int fileLength = Convert.ToInt32(stream.Length);

                    byte[] fileData = new byte[fileLength];
                    stream.Read(fileData, 0, fileLength);
                    fileDTO.FileData = fileData;
                    fileDTO.FileSize = fileData.Length;
                    fileDTO.StartPosition = 0;
                    fileDTO.IsClient = false;
                    fileDTOs.Add(fileDTO);
                }
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }


            try
            {
                List<string> fileImgList = Jinher.AMP.BTP.TPS.BTPFileSV.Instance.UploadFileList(fileDTOs);
                fileImgList = fileImgList.Select(x => CustomConfig.FileServerUrl + x).ToList();
                return string.Join(",", fileImgList);
            }
            catch
            {
                return "error";
            }


        }

        /// <summary>
        /// 获取商品
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult GetCommodity(Guid? appId, int pageIndex, int pageSize, Guid? promotionId, string areaCode)
        {
            if ((promotionId == null || promotionId.Value == Guid.Empty) && appId != null && appId.Value != Guid.Empty)
            {
                Jinher.AMP.BTP.ISV.Facade.CommodityFacade facade = new ISV.Facade.CommodityFacade();
                var ret = facade.GetCommodity(appId.Value, pageIndex, pageSize, areaCode);
                return Json(ret, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Jinher.AMP.BTP.ISV.Facade.PromotionFacade facade = new ISV.Facade.PromotionFacade();
                var result = facade.GetPromotionItems(promotionId.Value, Guid.Empty, pageIndex, pageSize);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 根据搜索条件查询商品
        /// </summary>
        /// <param name="want"></param>
        /// <param name="appId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult GetWantCommodity(string want, Guid appId, int pageIndex, int pageSize)
        {
            Jinher.AMP.BTP.ISV.Facade.CommodityFacade facade = new ISV.Facade.CommodityFacade();
            var result = facade.GetWantCommodity(want, appId, pageIndex, pageSize);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据类目查询商品
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="appId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult GetCommodityByCategory(Guid categoryId, Guid appId, int pageIndex, int pageSize)
        {
            Jinher.AMP.BTP.ISV.Facade.CommodityFacade facade = new ISV.Facade.CommodityFacade();
            var result = facade.GetCommodityByCategory(categoryId, appId, pageIndex, pageSize);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据商品id获取评价
        /// </summary>
        /// <param name="commodityId"></param>
        /// <param name="appId"></param>
        /// <param name="lastReviewTime"></param>
        /// <returns></returns>
        public ActionResult GetReviewByCommodityId(Guid commodityId, Guid appId, string lastReviewTime)
        {
            DateTime realLastTime;
            if (string.IsNullOrEmpty(lastReviewTime) || !DateTime.TryParse(lastReviewTime, out realLastTime))
            {
                realLastTime = DateTime.Now.AddYears(10);
            }
            Jinher.AMP.BTP.ISV.Facade.ReviewFacade facade = new ISV.Facade.ReviewFacade();
            List<ReviewSDTO> result = facade.GetReviewByCommodityId(commodityId, appId, realLastTime);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取类目
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public ActionResult GetCategory(Guid appId)
        {
            Jinher.AMP.BTP.ISV.Facade.CategoryFacade facade = new ISV.Facade.CategoryFacade();
            var result = facade.GetCategory(appId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 商品详情
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl]
        [ArgumentExceptionDeal(Title = "商品详情")]
        public ActionResult CommodityDetail(Guid commodityId, Guid? shopId)
        {
            ViewBag.BTPAppresUrl = CustomConfig.BTPAppres;
            ViewBag.PortalUrl = CustomConfig.PortalUrl;
            ViewBag.ZPHUrl = CustomConfig.ZPHUrl;
            ViewBag.IsShowConnection = isComDetailShowConnection();
            ViewBag.AssumeULikeUrl = HttpUtility.UrlEncode(WebUtil.GetAssumeULikeUrl());
            CommodityChangeFacade com = new CommodityChangeFacade();
            var typeid = com.JudgeActivityType(commodityId);
            LogHelper.Debug("根据商品id差找该商品所属活动类型 ：商品id：" + commodityId + ";活动类型：" + typeid);
            //if (typeid == 2)//预约 
            //{
            //    return View("~/Views/MobileFitted/CommodityReservation.cshtml");
            //}
            //if (typeid == 5)// 预售 
            //{
            //    return View("~/Views/MobileFitted/CommodityPresale.cshtml");
            //}
            //if (typeid == 0 || typeid == 1)//普通商品或者秒杀详情
            //{
            //    return View("~/Views/MobileFitted/CommodityOrdinaryDetail.cshtml");
            //}
            if (MobileCookies.IsFittedApp())
            {
                //定制，非拼团。
                bool b = BACBP.CheckCommodityReview(WebUtil.GetEsAppId());
                ViewBag.hasReviewFunction = b;
                return View("~/Views/MobileFitted/CommodityDetail.cshtml");
            }

            return View("~/Views/Mobile/CommodityDetail.cshtml");
        }

        /// <summary>
        /// 提醒发货，发系统消息给商家
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public ActionResult ShipmentRemind(Guid orderId)
        {
            Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade co = new Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade();
            var result = co.ShipmentRemind(orderId);

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 商品详情是否显示联系商家、进店逛逛
        /// </summary>
        /// <returns></returns>
        private bool isComDetailShowConnection()
        {
            if (!WebUtil.SideInWeixinBroswer())
                return true;
            if (ZPHSV.Instance.IsAppPavilion(WebUtil.GetEsAppId()))
                return true;
            return false;
        }

        /// <summary>
        /// 拼团商品详情
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl]
        [ArgumentExceptionDeal(Title = "商品详情")]
        public ActionResult CommodityDetailDiy(Guid commodityId, Guid? shopId)
        {
            ViewBag.BTPAppresUrl = CustomConfig.BTPAppres;
            ViewBag.PortalUrl = CustomConfig.PortalUrl;
            ViewBag.ZPHUrl = CustomConfig.ZPHUrl;
            ViewBag.IsShowConnection = isComDetailShowConnection();
            ViewBag.hasReviewFunction = BACBP.CheckCommodityReview(WebUtil.GetEsAppId());
            ViewBag.AssumeULikeUrl = HttpUtility.UrlEncode(WebUtil.GetAssumeULikeUrl());
            return View("~/Views/MobileFitted/CommodityDetailDiyGroup.cshtml");
        }




        /// <summary>
        /// 获取 未完成的拼团列表
        /// </summary>
        /// <param name="inputDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UnfinishedDiyGrouplist(UnfinishedDiyGroupInputDTO inputDTO)
        {
            var facade = new DiyGroupFacade();
            var resultDTO = facade.UnfinishedDiyGrouplist(inputDTO);
            var data = resultDTO.Data;
            return Json(data);

        }




        /// <summary>
        /// 商品详情
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl]
        [ArgumentExceptionDeal(Title = "金和")]
        public ActionResult CommodityDetailFirst(Guid? commodityId, Guid? appId, Guid? promotionId, Guid? userId)
        {
            string json = "";
            if (commodityId.HasValue)
            {
                Guid UserId = Jinher.JAP.BF.BE.Deploy.Base.ContextDTO.Current.LoginUserID;
                if (UserId == Guid.Empty && userId.HasValue)
                {
                    UserId = userId.Value;
                }
                Jinher.AMP.BTP.ISV.Facade.CommodityFacade facade = new ISV.Facade.CommodityFacade();
                var result = facade.GetCommodityDetails(commodityId.Value, Guid.Empty, UserId);

                json = JsonHelper.JsonSerializer<CommoditySDTO>(result);

                if (promotionId.HasValue)
                {
                    var prizeResult = facade.GetUserPrizeRecord(promotionId.Value, commodityId.Value, UserId);
                    if (prizeResult.ResultCode == 0)
                    {
                        ViewBag.IsHYL = true;
                        ViewBag.Price = prizeResult.Price;
                        ViewBag.UserID = UserId;
                    }
                }
                ViewBag.AppId = result.AppId;
            }

            ViewBag.ZPHUrl = CustomConfig.ZPHUrl;
            ViewBag.BTPAppresUrl = CustomConfig.BTPAppres;
            ViewBag.BTPBacUrl = CustomConfig.BTPBac;
            ViewBag.Result = json;
            ViewBag.PortalUrl = CustomConfig.PortalUrl;
            return View();
        }

        /// <summary>
        /// 商品详情接口
        /// </summary>
        /// <param name="commodityId"></param>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult GetCommodityDetails(Guid commodityId, Guid? appId)
        {
            if (appId == null && Session["appId"] != null)
            {
                appId = (Guid)Session["appId"];
            }
            Guid UserId = Jinher.JAP.BF.BE.Deploy.Base.ContextDTO.Current.LoginUserID;
            Jinher.AMP.BTP.ISV.Facade.CommodityFacade facade = new ISV.Facade.CommodityFacade();
            var result = facade.GetCommodityDetails(commodityId, appId == null ? Guid.Empty : appId.Value, UserId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 商品详情接口
        /// </summary>
        /// <param name="commodityId"></param>
        /// <param name="appId"></param>
        /// <param name="freightTo"></param>
        /// <param name="outPromotionId">正品会活动Id</param>
        /// <param name="userId"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public ActionResult GetCommodityDetailsZPH(Guid commodityId, Guid? appId, string freightTo, Guid? outPromotionId, Guid? userId, string source)
        {
            string contactUrl = string.Empty;
            string appName = string.Empty;
            var contactObj = Jinher.AMP.ZPH.Deploy.Enum.ContactObj.ContactCurrentApp;
            if (appId == Guid.Empty)
            {
                return Json(new { ResultCode = 1, Messages = "传入参数为空" });
            }
            var resultUrl = ZPHSV.Instance.GetMaskPicII(appId ?? Guid.Empty);
            var resultApp = APPSV.GetAppName(appId ?? Guid.Empty);
            var resultIsVolume = ZPHSV.Instance.CheckIsShowSalesVolume(appId ?? Guid.Empty);
            if (resultUrl != null && resultApp != null)
            {
                contactUrl = resultUrl.ContactUrl;
                contactObj = resultUrl.contactObj;
                appName = resultApp;
            }

            Jinher.AMP.BTP.ISV.Facade.CommodityFacade facade = new ISV.Facade.CommodityFacade();
            var result = facade.GetCommodityDetailsZPHNewSku(commodityId, appId ?? Guid.Empty, userId ?? Guid.Empty, freightTo, outPromotionId);
            AppDownloadDTO appDownload = new AppDownloadDTO();
            //定制应用分享下载
            if (MobileCookies.IsFittedApp() && !string.IsNullOrEmpty(source) && source.Trim().ToLower() == "share")
            {
                var downloadAppResult = new AppExtensionFacade().GetAppDownLoadInfo(WebUtil.GetEsAppId());
                if (downloadAppResult.ResultCode == 0)
                    appDownload = downloadAppResult.Data;
            }
            return Json(new
            {
                ResultCode = result.ResultCode,
                CommodityInfo = result.Data,
                AppDownLoadInfo = appDownload,
                ContactUrl = contactUrl,
                EsAppName = appName,
                ContactObj = contactObj,
                ResultIsVolume = resultIsVolume
            },
                            JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 商品详情接口 （金采团购活动使用）
        /// </summary>
        /// <param name="commodityId"></param>
        /// <param name="appId"></param>
        /// <param name="freightTo"></param>
        /// <param name="jcActivityId">金采团购活动Id</param>
        /// <param name="userId"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public ActionResult GetCommodityDetailsZPHII(Guid commodityId, Guid? appId, string freightTo, Guid? jcActivityId,
            Guid? userId, string source)
        {
            string contactUrl = string.Empty;
            string appName = string.Empty;
            var contactObj = Jinher.AMP.ZPH.Deploy.Enum.ContactObj.ContactCurrentApp;
            if (appId == Guid.Empty)
            {
                return Json(new { ResultCode = 1, Messages = "传入参数为空" });
            }
            var resultUrl = ZPHSV.Instance.GetMaskPicII(appId ?? Guid.Empty);
            var resultApp = APPSV.GetAppName(appId ?? Guid.Empty);
            var resultIsVolume = ZPHSV.Instance.CheckIsShowSalesVolume(appId ?? Guid.Empty);
            if (resultUrl != null && resultApp != null)
            {
                contactUrl = resultUrl.ContactUrl;
                contactObj = resultUrl.contactObj;
                appName = resultApp;
            }

            Jinher.AMP.BTP.ISV.Facade.CommodityFacade facade = new ISV.Facade.CommodityFacade();
            var result = facade.GetCommodityDetailsZPHNewSkuII(commodityId, appId ?? Guid.Empty, userId ?? Guid.Empty, freightTo, jcActivityId);
            AppDownloadDTO appDownload = new AppDownloadDTO();
            //定制应用分享下载
            if (MobileCookies.IsFittedApp() && !string.IsNullOrEmpty(source) && source.Trim().ToLower() == "share")
            {
                AppExtensionFacade appFacade = new AppExtensionFacade();
                var downloadAppResult = appFacade.GetAppDownLoadInfo(WebUtil.GetEsAppId());
                if (downloadAppResult.ResultCode == 0)
                    appDownload = downloadAppResult.Data;
            }
            return
                Json(
                    new
                    {
                        ResultCode = result.ResultCode,
                        CommodityInfo = result.Data,
                        AppDownLoadInfo = appDownload,
                        ContactUrl = contactUrl,
                        EsAppName = appName,
                        ContactObj = contactObj,
                        ResultIsVolume = resultIsVolume
                    }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据商品ID获取商品参与的优惠套装
        /// </summary>
        /// <param name="commodityId"></param>
        /// <param name="appId"></param>
        /// <param name="isDetailPage"></param>
        /// <returns></returns>
        public ActionResult GetSetMealActivitysByCommodityId(Guid commodityId, Guid appId)
        {
            var result = ZPHSV.Instance.GetSetMealActivitysByCommodityId(commodityId, appId, false);
            return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查看商品
        /// </summary>
        /// <param name="appId">appid</param>
        /// <param name="commodityIds">商品id，以逗号分隔</param>
        /// <returns></returns>
        [DealMobileUrl(UrlNeedAppParams = UrlNeedAppParamsEnum.ShopId)]
        [ArgumentExceptionDeal(Title = "")]
        public ActionResult CommodityView(Guid appId, string commodityIds, Guid? orderId)
        {
            Session.Add("appId", appId);
            List<Guid> idList = new List<Guid>();
            //兼容订单分享
            if (orderId != null && orderId.Value != Guid.Empty)
            {
                IBP.Facade.CommodityOrderFacade orderSV = new IBP.Facade.CommodityOrderFacade();
                var commodityResult = orderSV.GetCommodityIdsByOrderId(orderId.Value);
                if (commodityResult != null)
                {
                    //commodityId=' + getQueryString('commodityIds') + '&appId=' + sessionStorage.appId + '&user=' + sessionStorage.userId + '&source=share';
                    //if (commodityResult.Count == 1)
                    //{
                    //    string userId = Request.Params["user"] == null ? "" : Request.Params["user"].ToString();
                    //    if(string.IsNullOrEmpty(userId))
                    //    {
                    //        userId = Request.Params["userId"] == null ? "" : Request.Params["userId"].ToString();
                    //    }
                    //    //string type = Request.Params["type"] == null ? "" : Request.Params["type"].ToString();
                    //    string source = Request.Params["source"] == null ? "" : Request.Params["source"].ToString();
                    //    return RedirectToAction("CommodityDetail", new { appId = appId, commodityId = commodityResult[0], user = userId, source = source });
                    //}
                    //else if (commodityResult.Count > 1)
                    {
                        StringBuilder strbCommIds = new StringBuilder(50);
                        foreach (Guid commId in commodityResult)
                        {
                            strbCommIds.Append(commId).Append(",");
                        }
                        strbCommIds.Remove(strbCommIds.Length - 1, 1);
                        commodityIds = strbCommIds.ToString();
                    }
                }
            }
            else
            {
                string[] list = commodityIds.Split(',');
                for (int i = 0; i < list.Length; i++)
                {
                    Guid guid;
                    if (Guid.TryParse(list[i], out guid))
                    {
                        idList.Add(guid);
                    }
                }

                //if (string.IsNullOrEmpty(Request["source"]) && (Request.Params["type"] == null || string.IsNullOrEmpty(Request.Params["type"].ToString())) && idList.Count == 1)
                //{
                //    return RedirectToAction("CommodityDetail", new { appId = appId, commodityId = idList[0] });
                //}
                if (idList.Count == 1)
                {
                    var url =
                        Request.Url.ToString()
                            .ToLower()
                            .Replace("commodityview", "commoditydetail")
                            .Replace("commodityids", "commodityid");
                    if (!url.Contains("isshowsharebenefitbtn"))
                    {
                        if (!url.Contains("?"))
                            url += "?";
                        else
                            url += "&";
                        url += "isshowsharebenefitbtn=1";
                    }
                    return Redirect(url);
                }
            }
            ViewBag.commodityIds = commodityIds;
            ViewBag.PortalUrl = CustomConfig.PortalUrl;

            return View();
        }

        /// <summary>
        /// 根据商品ids获取商品列表
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="commodityIds"></param>
        /// <param name="areaCode">地区编码</param>
        /// <returns></returns>
        public ActionResult GetCommodityByIds(Guid appId, string commodityIds, string areaCode)
        {
            List<Guid> idList = new List<Guid>();
            string[] list = commodityIds.Split(',');
            for (int i = 0; i < list.Length; i++)
            {
                Guid guid;
                if (Guid.TryParse(list[i], out guid))
                {
                    idList.Add(guid);
                }
            }
            ISV.Facade.CommodityFacade facade = new ISV.Facade.CommodityFacade();
            List<CommodityListCDTO> result =
                facade.GetCommodityByIdsNew(new CommoditySearchDTO { commodityIdList = idList, AreaCode = areaCode });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 检查拼团状态
        /// </summary>
        /// <param name="inputDTO"></param>
        /// <returns></returns>
        public ActionResult CheckDiyGroup(CheckDiyGroupInputDTO inputDTO)
        {
            var facade = new ISV.Facade.DiyGroupFacade();

            var outputDTO = facade.CheckDiyGroup(inputDTO);

            return Json(outputDTO, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 创建订单
        /// </summary>
        /// <returns></returns>
        public ActionResult SaveCommodityOrder(OrderSDTO orderSDTO, string sessionId, string isHYL)
        {
            if (isHYL == "1")
            {
                Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade facade = new ISV.Facade.CommodityOrderFacade();
                var result = facade.SavePrizeCommodityOrder(orderSDTO);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                int orderCount = orderSDTO.ShoppingCartItemSDTO.Select(e => e.AppId).Distinct().Count();
                if (orderCount > 1)
                {
                    Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade facade = new ISV.Facade.CommodityOrderFacade();
                    var result = facade.SaveSetCommodityOrder(orderSDTO);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade facade = new ISV.Facade.CommodityOrderFacade();
                    var result = facade.SaveCommodityOrder(orderSDTO);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

            }
        }

        /// <summary>
        /// 订单页
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl(UrlNeedAppParams = UrlNeedAppParamsEnum.ShopId)] //原生商品详情跳转到下订单页面需要shopId
        public ActionResult CreateOrder()
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
            ViewBag.IsWx = WebUtil.SideInWeixinBroswer() && MobileCookies.IsFittedApp();

            var key = "A_OrderSet:" + esAppId.ToString();
            var data = HttpRuntime.Cache.Get(key) as OrderFieldDTO;
            if (data == null)
            {
                OrderFieldFacade ordersetfacde = new OrderFieldFacade();
                data = ordersetfacde.GetOrderSet(esAppId);
                if (data == null)
                {
                    data = new OrderFieldDTO { Id = Jinher.AMP.BTP.TPS.Cache.Consts.NullGuid };
                }
                HttpRuntime.Cache.Add(key, data, null, DateTime.Now.AddHours(1), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Default, null);
            }
            if (data.Id == Jinher.AMP.BTP.TPS.Cache.Consts.NullGuid)
            {
                data = null;
            }
            ViewBag.OrderSet = data;

            ViewBag.IsYJBJ = YJB.Deploy.CustomDTO.YJBConsts.YJAppId == esAppId;
            return View();
        }

        /// <summary>
        /// 支付页
        /// </summary>
        /// <returns></returns>
        [ArgumentExceptionDeal(Title = "金和")]
        public ActionResult Payment(Guid? appId)
        {
            if (appId == null)
            {
                appId = (Guid)Session["appId"];
            }
            Jinher.AMP.BTP.ISV.Facade.PaymentsFacade facade = new ISV.Facade.PaymentsFacade();
            var result = facade.GetPayments(appId.Value);
            bool hasAlipay = false;
            bool hasPayDelivery = false;
            bool hasJinherAlipay = false;
            foreach (PaymentsSDTO payment in result)
            {
                if (payment.PaymentsName == "支付宝")
                {
                    hasAlipay = true;
                }
                else if (payment.PaymentsName == "货到付款")
                {
                    hasPayDelivery = true;
                }
                else if (payment.PaymentsName == "担保支付宝")
                {
                    hasJinherAlipay = true;
                }
            }
            ViewBag.hasAlipay = hasAlipay;
            ViewBag.hasPayDelivery = hasPayDelivery;
            ViewBag.hasJinherAlipay = hasJinherAlipay;

            //System.Runtime.Serialization.Json.DataContractJsonSerializer serializer =
            //                               new System.Runtime.Serialization.Json.DataContractJsonSerializer(this.ContextDTO.GetType());
            //MemoryStream ms = new MemoryStream();
            //serializer.WriteObject(ms, this.ContextDTO);
            //string retVal = Convert.ToBase64String(ms.ToArray());
            //ms.Close();
            //ViewBag.ContextDTO = retVal;
            ViewBag.SessionId = this.ContextDTO.SessionID;
            return View();
        }

        /// <summary>
        /// 好运来支付页
        /// </summary>
        /// <returns></returns>
        [ArgumentExceptionDeal(Title = "金和")]
        public ActionResult PaymentHYL(Guid? appId)
        {
            if (appId == null)
            {
                appId = (Guid)Session["appId"];
            }
            Jinher.AMP.BTP.ISV.Facade.PaymentsFacade facade = new ISV.Facade.PaymentsFacade();
            var result = facade.GetPayments(appId.Value);
            bool hasAlipay = false;
            bool hasJinherAlipay = false;
            foreach (PaymentsSDTO payment in result)
            {
                if (payment.PaymentsName == "支付宝")
                {
                    hasAlipay = true;
                }
                else if (payment.PaymentsName == "担保支付宝")
                {
                    hasJinherAlipay = true;
                }
            }
            ViewBag.hasAlipay = hasAlipay;
            ViewBag.hasJinherAlipay = hasJinherAlipay;
            return View();
        }

        /// <summary>
        /// 获取省
        /// </summary>
        /// <returns></returns>
        public ActionResult GetProvince()
        {
            List<Area> provinceDtos = CBCBP.Instance.GeProvinceByCountryCode();
            provinceDtos.Add(new Area() { Code = "", Name = "请选择" });
            return Json(provinceDtos, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取市列表
        /// </summary>
        /// <param name="provinceCode"></param>
        /// <returns></returns>
        public ActionResult PartialProvince(string provinceCode)
        {
            CityFacade tempFacade = new CityFacade();
            List<Area> cityList = new List<Area>();
            if (!string.IsNullOrWhiteSpace(provinceCode))
            {
                cityList = CBCBP.Instance.GetCityByProvinceCode(provinceCode);
            }
            cityList.Add(new Area() { Code = "", Name = "请选择" });
            return Json(cityList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取区列表
        /// </summary>
        /// <param name="cityCode"></param>
        /// <param name="selectCountyCode"></param>
        /// <returns></returns>
        public ActionResult PartialCity(string cityCode)
        {
            List<CountyDTO> countyDtos = new List<CountyDTO>();
            if (!string.IsNullOrWhiteSpace(cityCode))
            {
                var tmpAreas = ProvinceCityHelper.GetCityDistricts(cityCode);
                if (tmpAreas != null && tmpAreas.Any())
                {
                    foreach (var tmpArea in tmpAreas)
                    {
                        countyDtos.Add(new CountyDTO
                        {
                            Code = tmpArea.AreaCode,
                            AreaCode = tmpArea.AreaCode,
                            Name = tmpArea.Name
                        });
                    }
                }
            }
            countyDtos.Add(new CountyDTO() { Code = "", Name = "请选择" });
            return Json(countyDtos, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取金币余额
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public ActionResult GetGoldBalance(System.Guid userId, string sessionId)
        {
            var result = FSPSV.Instance.GetBalance(userId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取收货地址
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public ActionResult GetDeliveryAddress(System.Guid userId, System.Guid appId)
        {
            Jinher.AMP.BTP.ISV.Facade.DeliveryAddressFacade facade = new ISV.Facade.DeliveryAddressFacade();
            var result = facade.GetDeliveryAddress(userId, appId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 货到付款
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="userId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public ActionResult UpdateCommodityOrder(System.Guid orderId, System.Guid userId, System.Guid appId)
        {
            Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade facade = new ISV.Facade.CommodityOrderFacade();
            var result = facade.UpdateCommodityOrder(1, orderId, userId, appId, 1, "", "");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public ActionResult Login(string username, string password)
        {
            LoginInfoDTO dto = new LoginInfoDTO();
            dto.IuAccount = username;
            dto.IuPassword = password;
            var result = CBCSV.Instance.Login(dto);
            // 保存上下文信息
            Session.Add("contextDTO", result.ContextDTO);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 支付宝
        /// </summary>
        [Obsolete("支付宝直接到账采用fsp接口", false)]
        public ActionResult SendAlipay(System.Guid orderId, System.Guid userId, System.Guid appId)
        {
            // 保存userId
            Session.Add("userId", userId);
            Session.Add("appId", appId);
            //获取订单
            Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade orderfacade = new ISV.Facade.CommodityOrderFacade();
            CommodityOrderSDTO order = orderfacade.GetOrderItems(orderId, userId, appId);
            //获取商家支付宝信息 
            Jinher.AMP.BTP.ISV.Facade.PaymentsFacade facade = new ISV.Facade.PaymentsFacade();
            AlipayDTO alipay = facade.GetAlipayInfo(appId);
            Alipay.Class.Config.Key = alipay.AliPayPublicKey;

            // 支付宝外部交易单号
            string out_trade_no = order.CommodityOrderId.ToString();
            string strChargingCount = order.Price.ToString();

            // 支付宝商品名称
            string subject = string.Format("订单号:{0}", order.Code);

            // 支付宝授权接口请求id
            string req_id = DateTime.Now.Ticks.ToString();

            // 初始化支付宝服务
            Service ali = new Service();

            // 调用支付宝授权接口
            string alipayToken = ali.alipay_wap_trade_create_direct(
                Alipay.Class.Config.Req_url,
                subject,
                out_trade_no,
                strChargingCount,
                alipay.AliPaySeller,
                Alipay.Class.Config.Notify_url,
                Alipay.Class.Config.Out_user,
                Alipay.Class.Config.Merchant_url,
                Alipay.Class.Config.Call_back_url,
                Alipay.Class.Config.Service_Create,
                Alipay.Class.Config.Sec_id,
                alipay.AliPayPartnerId,
                req_id,
                Alipay.Class.Config.Format,
                Alipay.Class.Config.V,
                Alipay.Class.Config.Input_charset_UTF8,
                Alipay.Class.Config.Req_url,
                alipay.AliPayPublicKey,
                Alipay.Class.Config.Sec_id);

            // 创建支付宝交易接口URL
            string alipayUrl = ali.alipay_Wap_Auth_AuthAndExecute(
                Alipay.Class.Config.Req_url,
                Alipay.Class.Config.Sec_id,
                alipay.AliPayPartnerId,
                Alipay.Class.Config.Call_back_url,
                Alipay.Class.Config.Format,
                Alipay.Class.Config.V,
                Alipay.Class.Config.Service_Auth,
                alipayToken,
                Alipay.Class.Config.Input_charset_UTF8,
                Alipay.Class.Config.Req_url,
                alipay.AliPayPublicKey,
                Alipay.Class.Config.Sec_id);

            // 调用支付宝交易接口
            Response.Redirect(alipayUrl);
            return null;
        }

        /// <summary>
        /// 支付宝移动支付异步通知。（个信）(支付宝直接到账)
        /// </summary>
        /// <returns></returns>
        [ValidateInput(false)]
        [Obsolete("支付宝直接到账采用fsp接口", false)]
        public ActionResult Notify()
        {
            // Debug log
            JAP.Common.Loging.LogHelper.Info("Begin支付宝调用Notify接口");
            JAP.Common.Loging.LogHelper.Info(string.Format("Request.Form Count: {0}", Request.Form.Count));
            try
            {
                foreach (var key in Request.Form.AllKeys)
                {
                    JAP.Common.Loging.LogHelper.Info(string.Format("\t{0}={1}", key, Request.Form[key]));
                }
            }
            catch (Exception e)
            {
                JAP.Common.Loging.LogHelper.Error(string.Format("Exception", e));
            }

            //创建待签名数组，注意Notify这里数组不需要进行排序，请保持以下顺序
            Dictionary<string, string> sArrary = new Dictionary<string, string>();
            sArrary.Add("service", Request.Form["service"]);
            sArrary.Add("v", Request.Form["v"]);
            sArrary.Add("sec_id", Request.Form["sec_id"]);
            sArrary.Add("notify_data", Request.Form["notify_data"]);

            //生成签名，用于和post过来的签名进行对照
            string mysign = Function.BuildMysign(sArrary, Alipay.Class.Config.Key, Alipay.Class.Config.Sec_id,
                Alipay.Class.Config.Input_charset_UTF8);

            //支付宝post的签名
            string aliSign = Request.Form["sign"];

            // Debug log
            JAP.Common.Loging.LogHelper.Info(string.Format("mysign: {0}", mysign));

            if (!aliSign.Equals(mysign))
            {
                //签名验证失败
                JAP.Common.Loging.LogHelper.Error("Alipay支付验证失败！");
                Response.Write("Alipay支付验证失败！");
                return null;
            }

            //获取notify_data的值
            string notify_data = Request.Form["notify_data"];
            //获取 notify_data 参数中xml格式里面的 trade_status 值
            string trade_status = Alipay.Class.Function.GetStrForXmlDoc(notify_data, "notify/trade_status");

            //判断trade_status是否为TRADE_FINISHED
            if (!trade_status.Equals("TRADE_FINISHED"))
            {
                //交易未成功
                JAP.Common.Loging.LogHelper.Error("Alipay支付失败！");
                Response.Write("fail");
                return null;
            }

            // 获取订单
            string out_trade_no = Alipay.Class.Function.GetStrForXmlDoc(notify_data, "notify/out_trade_no");
            // Debug log
            JAP.Common.Loging.LogHelper.Debug(string.Format("out_trade_no: {0}", out_trade_no));
            Guid orderId;
            if (Guid.TryParse(out_trade_no, out orderId) == false)
            {
                JAP.Common.Loging.LogHelper.Error(string.Format("Alipay支付成功，但订单ID错误！（订单ID：{0}）", out_trade_no));
                Response.Write("fail");
            }

            // 支付订单
            // Debug log
            JAP.Common.Loging.LogHelper.Info(string.Format("orderId: {0}", orderId));

            Guid userId = (Guid)Session["userId"];
            Guid appId = (Guid)Session["appId"];
            Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade facade = new ISV.Facade.CommodityOrderFacade();
            var result = facade.PayUpdateCommodityOrder(orderId, userId, appId, 2, 0, 0, 0);

            if (result.ResultCode == 1)
            {
                JAP.Common.Loging.LogHelper.Error(string.Format("Alipay支付成功，但支付金币出错！（订单ID：{0}）", out_trade_no));
                Response.Write("fail");
            }

            // Debug log
            JAP.Common.Loging.LogHelper.Info("End支付宝调用Notify接口");

            Response.Write("success");
            Response.End();
            return null;
        }

        /// <summary>
        /// 支付宝移动支付成功后跳转页。（个信）
        /// </summary>
        /// <returns></returns>
        [Obsolete("支付宝直接到账采用fsp接口", false)]
        public ActionResult Callback()
        {
            //Response.Redirect("/StaticPages/Success.html");
            //return null;
            return View();
        }

        /// <summary>
        /// 订单列表视图
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl]
        [WeixinOAuthOpenId]
        public ActionResult MyOrderList()
        {
            SetShopHome();
            ViewBag.PortalUrl = CustomConfig.PortalUrl;

            if (Request.QueryString["orderState"] == "-1")
                return Redirect("~/Mobile/RefundList" + Request.Url.Query);
            //return View("~/Views/Mobile/RefundList.cshtml");
            return View();
        }

        /// <summary>
        /// 获取订单
        /// </summary>
        /// <param name="orderQueryParamDTO"></param>
        /// <returns></returns>
        public ActionResult GetOrder(OrderQueryParamDTO orderQueryParamDTO)
        {
            Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade facade = new ISV.Facade.CommodityOrderFacade();

            var result = facade.GetCommodityOrderByUserIDNew(orderQueryParamDTO);

            Jinher.AMP.BTP.IBP.Facade.CommodityFacade commodityfacade = new Jinher.AMP.BTP.IBP.Facade.CommodityFacade();
            Jinher.AMP.BTP.IBP.Facade.JdOrderItemFacade jdorderitemfacade = new Jinher.AMP.BTP.IBP.Facade.JdOrderItemFacade();
            foreach (var order in result)
            {
                #region 获取是京东的数据内容
                JdOrderItemDTO model = new JdOrderItemDTO();
                model.CommodityOrderId = order.CommodityOrderId.ToString();
                var jdorderitemlist = jdorderitemfacade.GetJdOrderItemList(model).ToList();
                //对JdOrderId进行去重处理
                jdorderitemlist = jdorderitemlist.GroupBy(p => p.JdOrderId).Select(p => p.OrderByDescending(t => t.SubTime).FirstOrDefault()).ToList();
                if (jdorderitemlist.Count() > 0)
                {
                    foreach (var item in jdorderitemlist)
                    {
                        //获取子订单信息
                        var selectJdOrder = JdHelper.selectJdOrder1(item.JdOrderId);
                        if (!string.IsNullOrEmpty(selectJdOrder))
                        {
                            JObject objwlgs = JObject.Parse(selectJdOrder);
                            foreach (var _item in order.ShoppingCartItemSDTO)
                            {
                                JArray objson = JArray.Parse(objwlgs["sku"].ToString());
                                foreach (var ZiJdOrder in objson)
                                {
                                    Jinher.AMP.BTP.Deploy.CommodityDTO commodity = commodityfacade.GetCommodityDetail(_item.CommodityId);
                                    if (commodity.JDCode == ZiJdOrder["skuId"].ToString())
                                    {
                                        _item.JdOrderid = item.JdOrderId;
                                        _item.JdOrderStatus = (Jinher.AMP.BTP.Deploy.Enum.JdEnum)item.State;
                                        _item.JdSkuId = item.CommoditySkuId;
                                    }
                                }
                            }
                        }


                    }
                }
                #endregion
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private OrderDetailVM GetOrderDetailsVM(Guid orderId)
        {

            Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade facade = new ISV.Facade.CommodityOrderFacade();
            var result = facade.GetOrderItems(orderId, Guid.Empty, Guid.Empty);
            Guid realAppId = Guid.Empty;
            if (result != null && result.AppId != Guid.Empty)
            {
                realAppId = result.AppId;
            }

            //对没有收货人或收货人电话的，需从用户信息中取出
            if (string.IsNullOrWhiteSpace(result.ReceiptUserName) || string.IsNullOrWhiteSpace(result.ReceiptPhone))
            {
                var invoker = this.GetType() + ".MyOrderDetail";
                var jsonr = UserModel.GetUserNameAndCode(result.UserId, invoker);
                if (string.IsNullOrWhiteSpace(result.ReceiptUserName))
                {
                    result.ReceiptUserName = jsonr.Item1;
                }
                if (string.IsNullOrWhiteSpace(result.ReceiptPhone))
                {
                    result.ReceiptPhone = jsonr.Item2;
                }
            }
            string appName = APPSV.GetAppName(realAppId);

            OrderDetailVM od = new OrderDetailVM { data = result, Msg = "", AppName = appName };
            return od;
        }

        /// <summary>
        /// 去除模拟登录干扰，获取当前登录用户Id
        /// </summary>
        /// <returns></returns>
        private Guid getLoginUserId()
        {
            var userId = ContextDTO.LoginUserID;
            if (!AuthorizeHelper.IsLogin())
            {
                Jinher.JAP.Common.Context.ApplicationContext.Current[Jinher.JAP.Common.Context.ApplicationContext.ContextKey] =
                    Jinher.JAP.BF.BE.Deploy.Base.ContextDTO.GetDefaultValue();
                SingleSignOn singleSignOn = new SingleSignOn();
                SSOResult res = singleSignOn.Do(Request);
                if (res != null && res.IsSuccess)
                {
                    userId = Jinher.JAP.BF.BE.Deploy.Base.ContextDTO.Current.LoginUserID;
                }
                else
                {
                    userId = Guid.Empty;
                }
            }
            return userId;
        }

        /// <summary>
        /// 订单详情视图
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl]
        public ActionResult MyOrderDetail()
        {
            Guid userId = getLoginUserId();
            ViewBag.appId = Guid.Empty;

            CommodityOrderFacade orderFacade = new CommodityOrderFacade();

            string oidStr = Request.QueryString["orderId"];
            Guid orderId = Guid.Empty;
            if (!Guid.TryParse(oidStr, out orderId))
            {
                return View();
            }

            ViewBag.IsEclpOrder = new JdEclpOrderFacade().ISEclpOrder(orderId);

            #region //********是否存在苏宁订单
            ViewBag.IsSuNingOrder = false;
            var snOrderItemList = new SNAfterSaleFacade().GetSNOrderItemList(new SNOrderItemDTO { OrderId = orderId });
            if (snOrderItemList.Any())
            {
                ViewBag.IsSuNingOrder = true;
            }
            #endregion

            var orderCheckResult = orderFacade.GetOrderCheckInfo(new OrderQueryParamDTO { OrderId = orderId });
            if (orderCheckResult == null || orderCheckResult.ResultCode != 0 || orderCheckResult.Data == null)
            {
                return View();
            }
            ViewBag.appId = orderCheckResult.Data.AppId;
            //var result = GetOrderDetailsVM();
            //下单用户访问订单详情。
            if (orderCheckResult.Data.UserId == userId)
            {
                ViewBag.Org = this.ContextDTO.LoginOrg;
                ViewBag.FSPUrl = CustomConfig.FSPUrl;
                ViewBag.PortalUrl = CustomConfig.PortalUrl;
                ViewBag.PromotionUrl = CustomConfig.PromotionUrl;

                //if (APPBP.IsFittedApp(orderCheckResult.Data.AppId))
                //{
                bool hasReviewFunction = BACBP.CheckCommodityReview(orderCheckResult.Data.AppId);
                ViewBag.hasReviewFunction = hasReviewFunction;

                return View("~/Views/MobileFitted/MyOrderDetail.cshtml");
                //}
                //else
                //{
                //    return View("~/Views/Mobile/MyOrderDetail.cshtml");
                //}
            }
            else
            {
                var result = GetOrderDetailsVM(orderId);
                ViewBag.AppName = result.AppName;
                //非下单用户访问订单详情，只可预览，不可操作。
                return View("~/Views/Mobile/SelfTakeOrderDetail.cshtml", result.data);
            }
        }

        /// <summary>
        /// 查看订单明细
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="orderId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult GetOrderDetails(Guid? appId, Guid orderId, Guid userId)
        {
            Jinher.AMP.BTP.IBP.Facade.CommodityFacade commodityfacade = new Jinher.AMP.BTP.IBP.Facade.CommodityFacade();
            Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade facade = new ISV.Facade.CommodityOrderFacade();
            var result = facade.GetOrderItems(orderId, userId, appId.HasValue ? appId.Value : Guid.Empty);
            //****测试*测试数据
            //result.StateAfterSales = 3;



            #region 获取是京东的数据内容
            Jinher.AMP.BTP.IBP.Facade.JdOrderItemFacade jdorderitemfacade = new Jinher.AMP.BTP.IBP.Facade.JdOrderItemFacade();
            JdOrderItemDTO model = new JdOrderItemDTO();
            model.CommodityOrderId = result.CommodityOrderId.ToString();
            var jdorderitemlist = jdorderitemfacade.GetJdOrderItemList(model).ToList();
            //对JdOrderId进行去重处理
            jdorderitemlist = jdorderitemlist.GroupBy(p => p.JdOrderId).Select(p => p.OrderByDescending(t => t.SubTime).FirstOrDefault()).ToList();
            if (jdorderitemlist.Count() > 0)
            {
                foreach (var item in jdorderitemlist)
                {
                    //获取子订单信息
                    var selectJdOrder = JdHelper.selectJdOrder1(item.JdOrderId);
                    if (!string.IsNullOrEmpty(selectJdOrder))
                    {
                        JObject objwlgs = JObject.Parse(selectJdOrder);
                        foreach (var _item in result.ShoppingCartItemSDTO)
                        {
                            JArray objson = JArray.Parse(objwlgs["sku"].ToString());
                            foreach (var ZiJdOrder in objson)
                            {
                                Jinher.AMP.BTP.Deploy.CommodityDTO commodity = commodityfacade.GetCommodityDetail(_item.CommodityId);
                                if (commodity.JDCode == ZiJdOrder["skuId"].ToString())
                                {
                                    _item.JdOrderid = item.JdOrderId;
                                    _item.JdOrderStatus = (Jinher.AMP.BTP.Deploy.Enum.JdEnum)item.State;
                                    _item.JdSkuId = item.CommoditySkuId;
                                }
                            }
                        }
                    }


                }
            }
            #endregion


            #region 获取是苏宁的数据内容//*******************

            //foreach (var _item in result.ShoppingCartItemSDTO)
            //{
            //        _item.SnOrderid = "111";
            //        _item.SnSkuId = "222";
            //        _item.Price = 12;
            //        _item.SnOrderItemId = "222";
            //    _item.SnExpressStatus = 1;

            //}


            Jinher.AMP.BTP.IBP.Facade.SNAfterSaleFacade snorderitemfacade = new Jinher.AMP.BTP.IBP.Facade.SNAfterSaleFacade();
            SNOrderItemDTO modelSn = new SNOrderItemDTO
            {
                OrderId = result.CommodityOrderId
            };
            var snOrderItemListAll = snorderitemfacade.GetSNOrderItemList(modelSn).ToList();
            //对JdOrderId进行去重处理
            var snOrderItemList = snOrderItemListAll.GroupBy(p => p.CustomOrderId).Select(p => p.OrderByDescending(t => t.SubTime).FirstOrDefault()).ToList();
            if (snOrderItemList.Count() > 0)
            {
                foreach (var item in snOrderItemList)
                {
                    //获取子订单信息
                    SNAfterOrderDetailDTO selectSNOrder = SuningSV.SNGetOrderDetailById(item.CustomOrderId);
                    if (selectSNOrder != null)
                    {
                        //获取订单状态
                        foreach (var _item in result.ShoppingCartItemSDTO)
                        {
                            foreach (SNAfterOrderDetailListDTO ziSNOrder in selectSNOrder.OrderItemList)
                            {
                                Jinher.AMP.BTP.Deploy.CommodityDTO commodity = commodityfacade.GetCommodityDetail(_item.CommodityId);
                                if (ziSNOrder.CommdtyCode.Equals(commodity.JDCode))
                                {
                                    _item.SnOrderid = item.CustomOrderId;
                                    _item.SnSkuId = snOrderItemListAll.Where(p => p.CustomOrderItemId.Equals(ziSNOrder.OrderItemId)).FirstOrDefault().CustomSkuId;
                                    _item.Price = decimal.Parse(ziSNOrder.UnitPrice);
                                    _item.SnOrderItemId = ziSNOrder.OrderItemId;
                                    _item.SnExpressStatus = snOrderItemListAll.Where(p => p.CustomOrderItemId.Equals(ziSNOrder.OrderItemId)).FirstOrDefault().ExpressStatus; //item.ExpressStatus;
                                    //_item.SNOrderStatus = SNGetOrderStatus(item.CustomOrderId, ziSNOrder.OrderItemId);
                                }
                            }
                        }
                    }


                }
            }


            #endregion

            Guid realAppId = Guid.Empty;
            if (result != null && result.AppId != Guid.Empty)
                realAppId = result.AppId;
            string phone = string.Empty;
            string appName = "";
            try
            {
                //****测试**暂时注释
                appName = APPSV.GetAppName(realAppId);

            }
            catch (Exception ex)
            {
                LogHelper.Error(
                    string.Format("手机端查看订单信息详情异常。appId：{0}，orderId：{1}，userId：{2}", appId, orderId, userId), ex);
            }
            //对没有收货人或收货人电话的，需从用户信息中取出
            if (string.IsNullOrWhiteSpace(result.ReceiptUserName) || string.IsNullOrWhiteSpace(result.ReceiptPhone))
            {
                var invoker = this.GetType() + ".GetOrderDetails";
                var jsonr = UserModel.GetUserNameAndCode(userId, invoker);
                if (string.IsNullOrWhiteSpace(result.ReceiptUserName))
                {
                    result.ReceiptUserName = jsonr.Item1;
                }
                if (string.IsNullOrWhiteSpace(result.ReceiptPhone))
                {
                    result.ReceiptPhone = jsonr.Item2;
                }
            }
            if (result.OrderType == 3 && result.State == 3)
            {
                var yjbjCardList = new YJBJCardFacade().Get(result.CommodityOrderId);
                result.ShoppingCartItemSDTO.ForEach(p =>
                {
                    p.YJBJCardList = yjbjCardList.Where(x => x.CommodityId == p.CommodityId && x.Status == 2).ToList();
                });
            }
            #region 进销存订单
            JDEclpOrderDTO jdEclpOrder = null;
            if (result.EsAppId == YJB.Deploy.CustomDTO.YJBConsts.YJAppId && result.AppType.HasValue && new List<short> { 2, 3 }.Contains(result.AppType.Value))
                jdEclpOrder = new IBP.Facade.JdEclpOrderFacade().GetOrderInfo(result.CommodityOrderId);
            #endregion
            #region 第三方电商数据
            var subExpressNos = string.Empty;
            var isThirdECommerceOrder = false;
            var orderItemExpress = ThirdECommerceHelper.GetOrderItemExpress(realAppId, result.CommodityOrderId, ref isThirdECommerceOrder);
            if (orderItemExpress != null && orderItemExpress.Count > 0)
            {
                isThirdECommerceOrder = true;
                result.ShoppingCartItemSDTO.ForEach(p =>
                {
                    p.ExpressNo = orderItemExpress.Where(x => x.OrderItemId == p.Id).Select(x => x.ExpressNo).FirstOrDefault();
                    p.SubExpressNos = orderItemExpress.Where(x => x.OrderItemId == p.Id).Select(x => x.SubExpressNos).FirstOrDefault();
                });
                result.ShoppingCartItemSDTO = result.ShoppingCartItemSDTO.OrderBy(p => p.ExpressNo).ToList();
            }
            else
            {
                result.ShoppingCartItemSDTO.ForEach(p =>
                {
                    p.ExpressNo = result.ExpOrderNo;
                });
            }
            #endregion
            return Json(new { data = result, Msg = phone, AppName = appName, JdEclpOrder = jdEclpOrder, IsThirdECommerce = isThirdECommerceOrder }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 金币确认支付
        /// </summary>
        /// <param name="commodityOrderId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public ActionResult ConfirmOrder(Guid commodityOrderId, string password)
        {
            Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade facade = new ISV.Facade.CommodityOrderFacade();
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(System.Text.Encoding.GetEncoding("UTF-8").GetBytes(password));
            string pwd = BitConverter.ToString(output).Replace("-", "");
            var result = facade.ConfirmOrder(commodityOrderId, pwd);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 确认收货
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="userId"></param>
        /// <param name="appId"></param>
        /// <param name="payment"></param>
        /// <returns></returns>
        public ActionResult UpdateCommodityOrderc(System.Guid orderId,
            System.Guid userId, System.Guid appId, int payment, string goldpwd)
        {
            List<int> stwog = PaySourceVM.GetSecTransWithoutGoldPayment();
            if (stwog.Contains(payment) && !string.IsNullOrEmpty(goldpwd))
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] output = md5.ComputeHash(System.Text.Encoding.GetEncoding("UTF-8").GetBytes(goldpwd));
                goldpwd = BitConverter.ToString(output).Replace("-", "");
            }

            Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade facade = new ISV.Facade.CommodityOrderFacade();
            var result = facade.UpdateCommodityOrder(3, orderId, userId, appId, payment, goldpwd, "");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 取消退款
        /// </summary>
        /// <param name="orderId">订单id</param>
        /// <param name="state">状态</param>
        /// <returns></returns>
        public ActionResult CancelOrderRefund(Guid orderId, int state, Guid orderItemId)
        {
            Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade facade = new ISV.Facade.CommodityOrderFacade();
            ResultDTO result = facade.CancelOrderRefund(orderId, state);
            if (orderItemId != Guid.Empty)
            {
                result = facade.CancelOrderItemRefund(orderId, state, orderItemId);
            }
            else
            {
                result = facade.CancelOrderRefund(orderId, state);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckGoldPwd(Guid userId, string pwd)
        {
            FSP.Deploy.CustomDTO.ReturnInfoDTO result = Jinher.AMP.BTP.TPS.FSPSV.Instance.HasPassword(userId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SetGoldPayPwd(Guid userId, string pwd)
        {
            FSP.Deploy.CustomDTO.ReturnInfoDTO result = Jinher.AMP.BTP.TPS.FSPSV.Instance.ChangePassword("", pwd, userId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckGoldPayPwdVal(Guid userId, string sessionId, string pwd)
        {
            FSP.Deploy.CustomDTO.ReturnInfoDTO result = FSPSV.Instance.CheckPassword(pwd, ContextDTO);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 优惠套装列表视图
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl]
        public ActionResult MealItemsDetail()
        {
            return View();
        }

        /// <summary>
        /// 我的购物车列表视图
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl]
        public ActionResult ShoppongCartList()
        {
            ViewBag.PortalUrl = CustomConfig.PortalUrl;
            if (MobileCookies.IsFittedApp())
            {
                return View("~/Views/MobileFitted/ShoppongCartList.cshtml");
            }
            return View();
        }

        /// <summary>
        /// 获取购物车
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="Appid"></param>
        /// <returns></returns>
        public ActionResult GetShoppongCartList(Guid userId, Guid Appid)
        {
            var facade = new Jinher.AMP.BTP.ISV.Facade.ShoppingCartFacade();
            List<Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySDTO> Lists = facade.GetShoppongCartItemsNew(userId, Appid);

            List<Jinher.AMP.BTP.Deploy.CustomDTO.UserOrderCarDTO> listUOC = null;

            if (Lists != null && Lists.Count > 0)
            {
                List<Guid> appIds = (from it in Lists select it.AppId).Distinct().ToList();

                Dictionary<Guid, string> listApps = APPSV.GetAppNameListByIds(appIds);
                if (listApps.Any())
                {
                    foreach (var item in Lists)
                    {
                        if (listApps.ContainsKey(item.AppId))
                        {
                            var listAppName = listApps[item.AppId];
                            if (!String.IsNullOrEmpty(listAppName))
                            {
                                item.AppName = listAppName;
                            }
                        }
                    }
                }

                Jinher.AMP.BTP.ISV.Facade.CrowdfundingFacade cfFacade = new ISV.Facade.CrowdfundingFacade();
                listUOC = cfFacade.GetUserCrowdfundingBuyer(appIds, userId);
            }

            return Json(new { CommodifyList = Lists, CrowdfundingList = listUOC }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetShoppongCartList2(Guid userId, Guid Appid)
        {
            List<Jinher.AMP.BTP.Deploy.CustomDTO.UserOrderCarDTO> listUOC = null;
            List<ShopCartOfShopDto> shopList = new List<ShopCartOfShopDto>();
            List<ShopCartCommoditySDTO> InvalidList = new List<ShopCartCommoditySDTO>();


            var shopCartRetrun = new Jinher.AMP.BTP.ISV.Facade.ShoppingCartFacade().GetShoppongCartItemsNew3(userId, Appid);

            if (shopCartRetrun != null && shopCartRetrun.isSuccess && shopCartRetrun.Data != null)
            {
                InvalidList = shopCartRetrun.Data.InvalidList ?? InvalidList;

                if (shopCartRetrun.Data.ShopList != null && shopCartRetrun.Data.ShopList.Count > 0)
                {
                    shopList = shopCartRetrun.Data.ShopList;
                    var appIds = shopCartRetrun.Data.ShopList.Select(a => a.AppId).ToList();

                    listUOC = new ISV.Facade.CrowdfundingFacade().GetUserCrowdfundingBuyer(appIds, userId);
                }
            }

            var result = new
            {
                CrowdfundingList = listUOC,
                ShopList = shopList,
                InvalidList = InvalidList
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 推荐商品
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult GetHotRed(string appId, string userId)
        {
            ReturnInfo<List<ZPH.Deploy.CustomDTO.CommodityListCDTO>> result = new ReturnInfo<List<ZPH.Deploy.CustomDTO.CommodityListCDTO>>()
            {
                Data = new List<ZPH.Deploy.CustomDTO.CommodityListCDTO>()
            };

            LogHelper.Info("GetHotRed  appId:" + appId + "    user:" + userId);

            try
            {
                var facade = new ZPH.ISV.Facade.ToRecommendFacade();
                var param = new Jinher.AMP.ZPH.Deploy.MobileCDTO.YJBJToRecommendParam();
                param.pageSize = 20;
                param.appid = appId.ToString();
                param.userId = userId.ToString();
                param.pageOpt = ZPH.Deploy.Enum.PageOperate.Upword;
                param.sceneId = "shopCartPage";
                var ret = facade.GetHotRed(param);
                if (ret.isSuccess && ret.Code == 0)
                {
                    result.Data = ret.Data;
                    result.IsSuccess = true;
                }
                else
                {
                    LogHelper.Error("GetHotRed  error:" + JsonHelper.JsSerializer(ret));
                    result.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("GetHotRed message:" + ex.Message + "   ex:" + JsonHelper.JsSerializer(ex.StackTrace));
            }

            LogHelper.Info("GetHotRed  result:" + JsonHelper.JsSerializer(result));

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 领取优惠券
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <param name="cId"></param>
        /// <returns></returns>
        public ActionResult CreateCoupon(Guid appId, Guid userId, Guid couponId)
        {
            ReturnInfo result = new ReturnInfo();

            try
            {
                var couponFacade = new Jinher.AMP.Coupon.ISV.Facade.CouponFacade();
                var param = new CouponCreateRequestDTO();
                param.BindUserId = userId;
                param.EsAppId = appId;
                param.ConponTemplateId = couponId;
                var ret = couponFacade.CreateCoupon(param);
                result.IsSuccess = ret.IsSuccess;
                result.Message = ret.Info;
                result.Code = ret.Code.ToString();
            }
            catch (Exception ex)
            {
                LogHelper.Error("CreateCoupon   error Message:" + ex.Message + "    StackTrace:" + ex.StackTrace);
                result.IsSuccess = false;
                result.Message = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult SaveShoppingCart(SaveShoppingCartParamDTO sscDto)
        {
            ResultDTO result = new ResultDTO { ResultCode = 1 };
            if (sscDto == null || sscDto.CommodityId == Guid.Empty)
            {
                result.Message = "参数错误，参数不能为空！";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            Jinher.AMP.BTP.ISV.Facade.ShoppingCartFacade facade = new ISV.Facade.ShoppingCartFacade();
            result = facade.SaveShoppingCartNew(sscDto);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加购物车
        /// </summary>
        /// <param name="userId">当前用户Id</param>
        /// <param name="appId">发布商品的应用Id</param>
        /// <param name="esAppId">电商馆id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveShoppingCartNew(SaveShoppingCartParamDTO sscDto)
        {
            Jinher.AMP.BTP.ISV.Facade.ShoppingCartFacade facade = new ISV.Facade.ShoppingCartFacade();
            var result = facade.SaveShoppingCartNew(sscDto);
            return Json(result, JsonRequestBehavior.AllowGet);
        }




        /// <summary>
        /// 编辑购物车
        /// </summary>
        /// <param name="shopCartCommodityUpdateDTOs">购物车编辑实体</param>
        /// <param name="userId">用户Id</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public ActionResult UpdateShoppingCart
            (System.Collections.Generic.List<ShopCartCommodityUpdateDTO> shopCartCommodityUpdateDTOs,
                System.Guid userId, System.Guid appId)
        {
            Jinher.AMP.BTP.ISV.Facade.ShoppingCartFacade facade = new ISV.Facade.ShoppingCartFacade();
            var result = facade.UpdateShoppingCart(shopCartCommodityUpdateDTOs, userId, appId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除购物车
        /// </summary>
        /// <param name="shopCartItemId">购物车Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public ActionResult DeleteShoppingCart
            (System.Guid shopCartItemId, System.Guid userId, System.Guid appId)
        {
            Jinher.AMP.BTP.ISV.Facade.ShoppingCartFacade facade = new ISV.Facade.ShoppingCartFacade();
            var result = facade.DeleteShoppingCart(shopCartItemId, userId, appId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 校验商品
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="CommodityIdAndStockIds"></param>
        /// <returns></returns>
        public ActionResult CheckCommodity(Guid UserID, List<CommodityIdAndStockId> CommodityIdAndStockIds,
            Guid diyGroupId, int promotionType, Guid? jcActivityId)
        {
            CheckCommodityParam ccp = new CheckCommodityParam();
            ccp.UserID = UserID;
            ccp.CommodityIdsList = CommodityIdAndStockIds;
            ccp.DiygId = diyGroupId;
            ccp.PromotionType = promotionType;
            if (jcActivityId == null)
            {
                ccp.JcActivityId = Guid.Empty;
            }
            else
            {
                ccp.JcActivityId = (Guid)jcActivityId;
            }

            Jinher.AMP.BTP.ISV.Facade.CommodityFacade facade = new ISV.Facade.CommodityFacade();
            var result = new List<CheckCommodityDTO>();
            if (ccp.JcActivityId == Guid.Empty)
            {
                result = facade.CheckCommodityV3(ccp);
            }
            else
            {
                result = facade.CheckCommodityV3II(ccp);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [ArgumentExceptionDeal]
        [DealMobileUrl(UrlNeedAppParams = UrlNeedAppParamsEnum.ShopId)]
        public ActionResult PromotionList(Guid shopId)
        {
            var appId = shopId;
            Jinher.AMP.BTP.ISV.Facade.PromotionFacade facade = new ISV.Facade.PromotionFacade();
            var result = facade.GetNewPromotion(appId);
            ViewBag.PortalUrl = CustomConfig.PortalUrl;
            ViewBag.AppId = appId;
            return View(result);
        }

        /// <summary>
        /// 商品排序接口
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="appId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="fieldSort"></param>
        /// <param name="order"></param>
        /// <param name="areaCode"></param>
        /// <returns></returns>
        public ActionResult GetOrByCommodity(System.Guid categoryId, System.Guid appId, int pageIndex, int pageSize,
            int fieldSort, int order, string areaCode)
        {
            try
            {
                Jinher.AMP.BTP.ISV.Facade.CommodityFacade facade = new ISV.Facade.CommodityFacade();
                var result = facade.GetOrByCommodity(categoryId, appId, pageIndex, pageSize, fieldSort, order, areaCode);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(
                    string.Format("商品排序接口。categoryId：{0}，appId：{1}，pageIndex：{2}，pageSize：{3}，fieldSort：{4}，order：{5}",
                        categoryId, appId, pageIndex, pageSize, fieldSort, order), ex);
                return Json("", JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetOwnerIdByAppId(Guid appId)
        {
            var result = APPSV.Instance.GetAppOwnerInfo(appId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取收货地址列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="appId">appId</param>
        /// <param name="IsDefault">是否默认收货地址</param>
        /// <returns></returns>
        public ActionResult GetDeliveryAddressList(System.Guid userId, System.Guid appId, int IsDefault)
        {
            Jinher.AMP.BTP.ISV.Facade.DeliveryAddressFacade facade = new ISV.Facade.DeliveryAddressFacade();
            var result = facade.GetDeliveryAddressList(userId, appId, IsDefault);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加或修改收货地址
        /// </summary>
        /// <param name="addressDTO">地址实体</param>
        /// <param name="appId">APPId</param>
        /// <returns></returns>
        public ActionResult SaveDeliveryAddress(Jinher.AMP.BTP.Deploy.CustomDTO.AddressSDTO addressDTO)
        {
            Jinher.AMP.BTP.ISV.Facade.DeliveryAddressFacade facade = new ISV.Facade.DeliveryAddressFacade();
            var result = facade.SaveDeliveryAddressNew(addressDTO);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除收货地址
        /// </summary>
        /// <param name="addressId">地址ID</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public ActionResult DeleteDeliveryAddress(System.Guid addressId, System.Guid appId)
        {
            Jinher.AMP.BTP.ISV.Facade.DeliveryAddressFacade facade = new ISV.Facade.DeliveryAddressFacade();
            var result = facade.DeleteDeliveryAddress(addressId, appId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 编辑收货地址 设置默认地址
        /// </summary>
        /// <param name="addressId">地址id</param>
        /// <returns></returns>
        public ActionResult UpdateDeliveryAddressIsDefault(System.Guid addressId)
        {
            Jinher.AMP.BTP.ISV.Facade.DeliveryAddressFacade facade = new ISV.Facade.DeliveryAddressFacade();
            var result = facade.UpdateDeliveryAddressIsDefault(addressId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 编辑收货地址(已废弃)
        /// </summary>
        /// <param name="addressDTO">地址实体</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public ActionResult UpdateDeliveryAddress(Jinher.AMP.BTP.Deploy.CustomDTO.AddressSDTO addressDTO,
            System.Guid appId)
        {
            Jinher.AMP.BTP.ISV.Facade.DeliveryAddressFacade facade = new ISV.Facade.DeliveryAddressFacade();
            var result = facade.UpdateDeliveryAddress(addressDTO, appId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 收货地址详情
        /// </summary>
        /// <param name="addressId">地址ID</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public ActionResult GetDeliveryAddressByAddressId(System.Guid addressId, System.Guid appId)
        {
            Jinher.AMP.BTP.ISV.Facade.DeliveryAddressFacade facade = new ISV.Facade.DeliveryAddressFacade();
            var result = facade.GetDeliveryAddressByAddressId(addressId, appId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 编辑收货地址视图
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl]
        public ActionResult AddDeliveryAddress()
        {
            string invoker = this.GetType() + ".AddDeliveryAddress";
            Guid addressId = Guid.Empty;
            Guid.TryParse(Request["addressid"], out addressId);
            AddressSDTO address = null;
            if (addressId != Guid.Empty)
            {
                address = DeliveryAddressModel.GetDeliveryAddressByAddressId(addressId, Guid.Empty, invoker);
            }
            if (address == null)
            {
                address = new AddressSDTO();
            }
            else
            {
                Dictionary<string, string> dic = getoldProvice();
                foreach (var item in dic)
                {
                    if (item.Key.Contains(address.Province.Trim()))
                    {
                        address.ProvinceCode = item.Value;
                    }
                }
            }
            JavaScriptSerializer js = new JavaScriptSerializer();
            string addressJsonString = js.Serialize(address);
            addressJsonString = HttpUtility.UrlEncode(addressJsonString);
            ViewBag.AddressInfo = addressJsonString;
            return View();
        }

        //原有省数据
        public Dictionary<string, string> getoldProvice()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("北京市", "1");
            dic.Add("上海市", "2");
            dic.Add("天津市", "3");
            dic.Add("重庆市", "4");
            dic.Add("河北省", "5");
            dic.Add("山西省", "6");
            dic.Add("河南省", "7");
            dic.Add("辽宁省", "8");
            dic.Add("吉林省", "9");
            dic.Add("黑龙江省", "10");
            dic.Add("内蒙古自治区", "11");
            dic.Add("江苏省", "12");
            dic.Add("山东省", "13");
            dic.Add("安徽省", "14");
            dic.Add("浙江省", "15");
            dic.Add("福建省", "16");
            dic.Add("湖北省", "17");
            dic.Add("湖南省", "18");
            dic.Add("广东省", "19");
            dic.Add("广西壮族自治区", "20");
            dic.Add("江西省", "21");
            dic.Add("四川省", "22");
            dic.Add("海南省", "23");
            dic.Add("贵州省", "24");
            dic.Add("云南省", "25");
            dic.Add("西藏自治区", "26");
            dic.Add("陕西省", "27");
            dic.Add("甘肃省", "28");
            dic.Add("青海省", "29");
            dic.Add("宁夏回族自治区", "30");
            dic.Add("新疆维吾尔自治区", "31");
            dic.Add("台湾省", "32");
            dic.Add("澳门特别行政区", "52993");
            dic.Add("香港特别行政区", "52993");
            return dic;
        }


        /// <summary>
        /// 获取收获地址视图 
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl]
        public ActionResult DeliveryAddressList()
        {
            ViewBag.PortalUrl = CustomConfig.PortalUrl;
            return View();
        }

        public ActionResult GenShortUrl(string longUrl)
        {
            string result = string.Empty;
            try
            {
                result = ShortUrlSV.Instance.GenShortUrl(longUrl);
                LogHelper.Info(string.Format("MobileController.GenShortUrl获取短地址，原地址:{0}，短地址:{1}", longUrl, result));
                if (string.IsNullOrEmpty(result))
                {
                    result = longUrl;
                }
            }
            catch (Exception ex)
            {
                result = longUrl;
                LogHelper.Error(string.Format("MobileController.GenShortUrl，获取短地址异常，原地址:{0}", longUrl), ex);
            }

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetAppName(Guid appId)
        {
            var result = APPSV.GetAppName(appId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 验证手机号 是否被注册
        /// </summary>
        /// <param name="LoginId"></param>
        /// <returns></returns>
        public ActionResult CheckMobileRegister(string LoginId)
        {
            Jinher.AMP.CBC.Deploy.CustomDTO.ReturnInfoDTO result =
                CBCSV.Instance.CheckAccountIsRegistered(DecodeBase64(LoginId));
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 给手机 发送验证码
        /// </summary>
        /// <param name="LoginId"></param>
        /// <returns></returns>
        public ActionResult SendMobileCode(string LoginId)
        {
            var result = CBCSV.Instance.CheckRegisteredGenAuthCode(DecodeBase64(LoginId));
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RegisterAndLogin(string LoginId, string Password, string Code, string validate)
        {
            try
            {
                var json = new object();
                var dicstr = CheckValidCode(validate);
                if (dicstr.Count != 0)
                {
                    json = new
                    {
                        Success = false,
                        Message = dicstr.Values.First()
                    };

                    return Json(json, JsonRequestBehavior.AllowGet);
                }


                string code = Code;
                string phone = DecodeBase64(LoginId);
                string password = DecodeBase64(Password);
                string sessId = string.Empty;
                UserDTO user = new UserDTO();
                user.AccountType = CBC.Deploy.Enum.AccountTypeEnum.Normal;
                user.Name = phone;
                user.Password = password;
                RegReturnInfoDTO returnInfo = CBCSV.Instance.RegisterWithAuthCode(user, code);


                LoginReturnInfoDTO LoginReturnInfo = new LoginReturnInfoDTO();
                //注册成功，更新应用subId
                if (returnInfo.IsSuccess)
                {
                    //登录
                    LoginInfoDTO loginInfoDTO = new LoginInfoDTO();
                    loginInfoDTO.IuAccount = phone;
                    loginInfoDTO.IuPassword = password;

                    LoginReturnInfo = CBCSV.Instance.Login(loginInfoDTO);
                    if (LoginReturnInfo.IsSuccess)
                    {
                        sessId = LoginReturnInfo.ContextDTO.SessionID;

                        bool isDistrib = false;
                        bool.TryParse(this.Request["isDistrib"], out isDistrib);
                        if (isDistrib)
                        {
                            Guid esAppId = Guid.Empty;
                            Guid.TryParse(this.Request["esAppId"], out esAppId);

                            Guid srcDistributorId = Guid.Empty;
                            Guid.TryParse(this.Request["srcDistributorId"], out srcDistributorId);


                            //绑定三级分销关系。
                            Jinher.AMP.BTP.Deploy.CustomDTO.DistributorUserRelationDTO distUR =
                                new DistributorUserRelationDTO();
                            distUR.DistributorId = srcDistributorId;
                            distUR.EsAppId = esAppId;
                            distUR.LoginAccount = phone;
                            distUR.UserId = LoginReturnInfo.ContextDTO.LoginUserID;

                            Jinher.AMP.BTP.ISV.Facade.DistributorFacade distFacade =
                                new Jinher.AMP.BTP.ISV.Facade.DistributorFacade();
                            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Guid> result =
                                distFacade.SaveDistributorRelation(distUR);
                            if (result.ResultCode != 0)
                            {
                                returnInfo.IsSuccess = false;
                                returnInfo.Message = result.Message;
                                returnInfo.StatusCode = result.ResultCode.ToString();
                            }
                        }
                    }
                }
                return
                    Json(
                        new
                        {
                            Success = true,
                            RegReturnInfo = returnInfo,
                            LoginReturnInfo = LoginReturnInfo,
                            SessionID = sessId
                        }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(
                    string.Format("注册并登录异常。LoginId：{0}，Password：{1}，Code：{2}，validate：{3}", LoginId, Password, Code,
                        validate), ex);
                return Json("", JsonRequestBehavior.AllowGet);
            }


        }

        public ActionResult MobileLogin(string LoginId, string Password, string validateCode)
        {
            Jinher.AMP.EBC.Deploy.CustomDTO.ReturnInfoDTO rt = new Jinher.AMP.EBC.Deploy.CustomDTO.ReturnInfoDTO();
            var dicstr = CheckValidCode(validateCode);
            if (dicstr.Count != 0)
            {
                rt.IsSuccess = false;
                rt.Message = dicstr.Values.First();
                return Json(new { ret = rt }, JsonRequestBehavior.AllowGet);
            }

            Guid subId = Guid.Empty;
            Guid orgId = Guid.Empty;

            string sessId = string.Empty;
            string loginCode = DecodeBase64(LoginId);
            string loginPassword = DecodeBase64(Password);
            //登录
            LoginInfoDTO loginInfoDTO = new LoginInfoDTO();
            loginInfoDTO.IuAccount = loginCode;
            loginInfoDTO.IuPassword = loginPassword;
            LoginReturnInfoDTO logReturnInfo = CBCSV.Instance.Login(loginInfoDTO);
            if (logReturnInfo.IsSuccess)
            {

                subId = logReturnInfo.ContextDTO.LoginUserID;
                orgId = logReturnInfo.ContextDTO.LoginOrg;
                sessId = logReturnInfo.ContextDTO.SessionID;
                Jinher.AMP.Portal.Common.SetSessionCache.SetPersonalContext(logReturnInfo.ContextDTO);
                bool isDistrib = false;
                bool.TryParse(this.Request["isDistrib"], out isDistrib);
                if (isDistrib)
                {
                    Guid esAppId = Guid.Empty;
                    Guid.TryParse(this.Request["esAppId"], out esAppId);

                    Guid srcDistributorId = Guid.Empty;
                    Guid.TryParse(this.Request["srcDistributorId"], out srcDistributorId);

                    //绑定三级分销关系。
                    Jinher.AMP.BTP.Deploy.CustomDTO.DistributorUserRelationDTO distUR = new DistributorUserRelationDTO();
                    distUR.DistributorId = srcDistributorId;
                    distUR.EsAppId = esAppId;
                    distUR.LoginAccount = loginCode;
                    distUR.UserId = logReturnInfo.ContextDTO.LoginUserID;

                    Jinher.AMP.BTP.ISV.Facade.DistributorFacade distFacade =
                        new Jinher.AMP.BTP.ISV.Facade.DistributorFacade();
                    Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Guid> result = distFacade.SaveDistributorRelation(distUR);
                    if (result.ResultCode == 0)
                    {
                        rt.IsSuccess = true;
                    }
                    else
                    {
                        rt.IsSuccess = false;
                        rt.Message = result.Message;
                        rt.StatusCode = result.ResultCode.ToString();
                    }
                }
                else
                {
                    rt.IsSuccess = true;
                }
            }
            else
            {
                rt.IsSuccess = false;
                rt.Message = logReturnInfo.Message;
                rt.StatusCode = logReturnInfo.StatusCode;
            }
            return Json(new { ret = rt, SubId = subId, OrgId = orgId, SessionID = sessId }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ContactAppOwner()
        {
            return View();
        }

        public ActionResult GetAppMenuChangeInfo(Guid appId)
        {
            var appMenuList = BACSV.Instance.GetAppMenuChangeInfo(appId);
            bool isShowSearchMenu = false;
            var searchMenuResult =
                new ISV.Facade.CategoryFacade().CheckIsShowSearchMenu(new CategorySearchDTO { AppId = appId });
            if (searchMenuResult != null && searchMenuResult.ResultCode == 0)
                isShowSearchMenu = searchMenuResult.Data;
            return Json(new { appMenuList = appMenuList, isShowSearchMenu = isShowSearchMenu },
                JsonRequestBehavior.AllowGet);

        }


        //获取验证码
        public ActionResult GetValidateCode()
        {
            ValidateCode vCode = new ValidateCode();
            string code = vCode.CreateRandomCode(4);
            SessionCache.Current.AddCache("ValidCode", code);
            SessionCache.Current.AddCache("ValidCodeGenerateTime", DateTime.Now);
            byte[] bytes = vCode.CreateValidateGraphic(code);
            return File(bytes, @"image/jpeg");

        }



        private Dictionary<bool, string> CheckValidCode(string validCode)
        {
            Dictionary<bool, string> dic = new Dictionary<bool, string>();
            if (string.IsNullOrEmpty(validCode))
            {
                dic.Add(false, "请输入验证码");
                return dic;
            }
            if (SessionCache.Current.GetCache("ValidCodeGenerateTime") != null &&
                !string.IsNullOrEmpty(SessionCache.Current.GetCache("ValidCodeGenerateTime").ToString()))
            {
                DateTime endTime = DateTime.Now;
                DateTime beginTime = DateTime.Parse(SessionCache.Current.GetCache("ValidCodeGenerateTime").ToString());
                long timeDiff = DateDiff(beginTime, endTime);
                if (timeDiff > EXPIRATION_TIME)
                {
                    dic.Add(false, "您输入的验证码有误");
                    return dic;
                }
            }
            if (SessionCache.Current.GetCache("ValidCode") != null &&
                validCode.ToLower() != SessionCache.Current.GetCache("ValidCode").ToString().ToLower())
            {
                dic.Add(false, "您输入的验证码有误");
                return dic;
            }
            return dic;
        }

        /// <summary>
        /// 验证码过期时间5分钟
        /// </summary>
        private static long EXPIRATION_TIME = 5 * 60;

        private long DateDiff(DateTime arg_StartDate, DateTime arg_EndDate)
        {
            long lngDateDiffValue = 0;
            System.TimeSpan objTimeSpan = new System.TimeSpan(arg_EndDate.Ticks - arg_StartDate.Ticks);

            lngDateDiffValue = (long)objTimeSpan.TotalSeconds;
            return (lngDateDiffValue);
        }

        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="codeName">解密采用的编码方式，注意和加密时采用的方式一致</param>
        /// <param name="result">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        public static string DecodeBase64(Encoding encode, string result)
        {
            string decode = "";
            byte[] bytes = Convert.FromBase64String(result);
            try
            {
                decode = encode.GetString(bytes);
            }
            catch
            {
                decode = result;
            }
            return decode;
        }

        /// <summary>
        /// Base64解密，采用utf8编码方式解密
        /// </summary>
        /// <param name="result">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        public static string DecodeBase64(string result)
        {
            return DecodeBase64(Encoding.UTF8, result);
        }

        /// <summary>
        /// 分享订单视图
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl]
        public ActionResult ShareMyOrderDetail()
        {
            ViewBag.PortalUrl = CustomConfig.PortalUrl;
            return View();
        }

        /// <summary>
        /// 获取分享订单详情
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public ActionResult GetShareMyOrderDetail(Guid orderId, Guid appId)
        {
            Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade orderfacade = new ISV.Facade.CommodityOrderFacade();
            CommodityOrderSDTO order = orderfacade.GetOrderItems(orderId,
                new Guid("00000000-0000-0000-0000-000000000000"), appId);
            //order.ReceiptPhone = "***" + order.ReceiptPhone.Substring(3, 5) + "***";
            if ((!string.IsNullOrEmpty(order.ReceiptPhone))
                && order.ReceiptPhone.Length >= 11)
            {
                order.ReceiptPhone = order.ReceiptPhone.Substring(0, 3) + "*****" + order.ReceiptPhone.Substring(8, 3);
            }

            order.City = "***";
            order.District = "***";
            string address = "";
            if ((!string.IsNullOrEmpty(order.ReceiptAddress))
                && order.ReceiptAddress.Length > 3)
            {
                for (int i = 0; i < order.ReceiptAddress.Length - 3; i++)
                {
                    address += "*";
                }
                order.ReceiptAddress = order.ReceiptAddress.Substring(0, 3) + address;
            }

            if ((!string.IsNullOrEmpty(order.ReceiptUserName))
                && order.ReceiptUserName.Length > 1)
            {
                order.ReceiptUserName = order.ReceiptUserName.Substring(0, 1) + "**";
            }
            if ((!string.IsNullOrEmpty(order.Code))
                && order.Code.Length > 1)
            {
                order.Code = order.Code.Substring(0, 4) + "**************";
            }
            order.Payment = -1;
            string nickName = "";
            string userImg = "";
            try
            {
                Jinher.AMP.BTP.ISV.Facade.BTPUserFacade userSV = new ISV.Facade.BTPUserFacade();
                Jinher.AMP.BTP.Deploy.CustomDTO.UserSDTO commodityuser = userSV.GetUser(order.UserId,
                    new Guid(
                        "00000000-0000-0000-0000-000000000000"));
                nickName = commodityuser.UserName;
                userImg = commodityuser.PicUrl;
            }
            catch (Exception ex)
            {
                LogHelper.Error(
                    string.Format("GetShareMyOrderDetail调CBC这个GetUserBasicInfoNew服务异常。orderId：{0}，userId：{1}",
                        orderId.ToString(), order.UserId.ToString()), ex);
            }

            return Json(new { data = order, nickname = nickName, userimg = userImg }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orderId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public ActionResult CopyOrderToShoppingCart(Guid userId, Guid orderId, Guid appId)
        {
            Jinher.AMP.BTP.ISV.Facade.ShoppingCartFacade facade = new ISV.Facade.ShoppingCartFacade();
            var result = facade.CopyOrderToShoppingCart(userId, orderId, appId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 众筹宣传语
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl]
        [ArgumentExceptionDeal]
        public ActionResult CrowdfundingSlogan(Guid shopId)
        {
            Jinher.AMP.BTP.ISV.Facade.CrowdfundingFacade srefacade = new ISV.Facade.CrowdfundingFacade();

            var result = srefacade.GetCrowdfundingSlogan(shopId);
            ViewBag.Slogan = result;
            return View();
        }

        /// <summary>
        /// 众筹详情
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl]
        [ArgumentExceptionDeal(Title = "金和IU")]
        public ActionResult CrowdfundingDesc(Guid appId)
        {
            Jinher.AMP.BTP.ISV.Facade.CrowdfundingFacade srefacade = new ISV.Facade.CrowdfundingFacade();

            var result = srefacade.GetCrowdfundingDesc(appId);
            ViewBag.CrowdDesc = result;
            return View();
        }

        /// <summary>
        /// 众筹分红首页
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl]
        public ActionResult Crowdfunding()
        {
            return View();
        }

        /// <summary>
        /// 众筹分红明细
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl]
        public ActionResult CrowdfundingDividend()
        {
            return View();
        }

        /// <summary>
        /// 持股详情
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl]
        public ActionResult CrowdfundingHolding()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult GetUserCrowdfundingBuy(Guid appId, Guid userId)
        {
            Jinher.AMP.BTP.ISV.Facade.CrowdfundingFacade facade = new ISV.Facade.CrowdfundingFacade();
            var result = facade.GetUserCrowdfundingBuy(appId, userId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public ActionResult GetCrowdfundingState(Guid appId)
        {
            Jinher.AMP.BTP.ISV.Facade.CrowdfundingFacade facade = new ISV.Facade.CrowdfundingFacade();
            var result = facade.GetCrowdfundingState(appId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 众筹宣传语
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCrowdfundingSlogan(Guid appId)
        {
            Jinher.AMP.BTP.ISV.Facade.CrowdfundingFacade srefacade = new ISV.Facade.CrowdfundingFacade();
            var result = srefacade.GetCrowdfundingSlogan(appId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 订单取消
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl]
        public ActionResult OrderCancelReason()
        {
            return View();
        }


        public ActionResult ClickOKCancelOrder(Guid orderId, Guid userId, Guid appId, string mess)
        {
            Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade co = new Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade();
            var result = co.UpdateCommodityOrder(5, orderId, userId, appId, 0, null, mess);

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 删除订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="isDel">1客户删除2商家删除</param>
        /// <returns></returns>
        public ActionResult DelOrder(Guid orderId, int isDel = 1)
        {
            Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade co = new Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade();
            var result = co.DelOrder(orderId, isDel);

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 获取中石化电子发票下载地址
        /// </summary>
        /// <param name="orderCode">订单code</param>
        public ActionResult Invoice(string orderCode)
        {
            string pdfContent = "";
            InvoicManage invoicManage = new InvoicManage();
            var analyzeInvoicInfo = invoicManage.AnalyzeInvoicInfo(orderCode, 0, out pdfContent);
            if (analyzeInvoicInfo == "")
            {
                analyzeInvoicInfo = invoicManage.AnalyzeInvoicInfo(orderCode, 2, out pdfContent);
            }
            ViewBag.InvoicePath = analyzeInvoicInfo;
            ViewBag.InvoiceInfo = pdfContent;
            return View();
        }

        /// <summary>
        /// 获取中石化电子发票下载地址
        /// </summary>
        /// <param name="orderCode">订单code</param>
        public ActionResult DownInvoice(string orderCode)
        {
            string agent = (Request.UserAgent + "").ToLower().Trim();
            if (agent.IndexOf("android", StringComparison.Ordinal) == -1)
            {
                string pdfContent = "";
                InvoicManage invoicManage = new InvoicManage();
                var analyzeInvoicInfo = invoicManage.AnalyzeInvoicInfo(orderCode, 0, out pdfContent);
                if (analyzeInvoicInfo == "")
                {
                    analyzeInvoicInfo = invoicManage.AnalyzeInvoicInfo(orderCode, 2, out pdfContent);
                }

                FileStream myFileStream = new FileStream(pdfContent.Replace("\\", "/"), FileMode.Open);
                FileStream fs = myFileStream;
                byte[] buffer = new byte[fs.Length];
                fs.Position = 0;
                fs.Read(buffer, 0, (int)fs.Length);
                Response.Clear();
                Response.AddHeader("Content-Length", fs.Length.ToString());
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", "inline;FileName=" + pdfContent.Replace("\\", "/") + "");
                fs.Close();
                Response.BinaryWrite(buffer);
                Response.OutputStream.Flush();
                Response.OutputStream.Close();
            }
            return View();
        }

        /// <summary>
        /// 安卓端下载pdf文件
        /// </summary>
        /// <param name="orderCode"></param>
        /// <returns></returns>
        public ActionResult GetFile(string orderCode)
        {
            string pdfContent = "";
            InvoicManage invoicManage = new InvoicManage();
            var analyzeInvoicInfo = invoicManage.AnalyzeInvoicInfo(orderCode, 0, out pdfContent);
            if (analyzeInvoicInfo == "")
            {
                analyzeInvoicInfo = invoicManage.AnalyzeInvoicInfo(orderCode, 2, out pdfContent);
            }
            LogHelper.Debug(string.Format("安卓端下载pdf文件：{0}", analyzeInvoicInfo));
            return Json(analyzeInvoicInfo, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DelayShip(Guid orderId)
        {
            Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade co = new Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade();
            var result = co.DelayConfirmTime(orderId);

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetFreightDetails(Guid commodityId)
        {
            Jinher.AMP.BTP.ISV.Facade.CommodityFacade facade = new ISV.Facade.CommodityFacade();
            var result = facade.GetFreightDetails(commodityId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 计算运费
        /// </summary>
        /// <param name="templateCounts"></param>
        /// <param name="freightTo"></param>
        /// <param name="isSelfTake"></param>
        /// <returns></returns>
        public ActionResult CalFreight(List<OptimalCouponParam> templateCounts, string freightTo, int isSelfTake)
        {
            Jinher.AMP.BTP.ISV.Facade.CommodityFacade facade = new ISV.Facade.CommodityFacade();
            string FreightTo = null;
            //Dictionary<string, string> dic = getoldProvice();
            //foreach (var item in dic)
            //{
            //    if (item.Key.Contains(freightTo.Trim()))
            //    {
            //        FreightTo = item.Value;
            //    }
            //}
            //放弃原有逻辑
            FreightTo = ProvinceCityHelper.GetProvinceCodeByName(freightTo);
            List<Jinher.AMP.BTP.Deploy.CustomDTO.TemplateCountDTO> tempCounts = (from tc in templateCounts
                                                                                 select
                                                                                     new TemplateCountDTO()
                                                                                     {
                                                                                         CommodityId = tc.CommodityId,
                                                                                         Count = tc.Count,
                                                                                         Price = tc.Price
                                                                                     }).ToList();
            var result = facade.CalFreightMultiApps(FreightTo, isSelfTake, tempCounts, null, null, null);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Obsolete("已废弃", false)]
        public ActionResult GetSecKillPromotion(Guid commodityId)
        {
            Jinher.AMP.BTP.ISV.Facade.PromotionFacade facade = new Jinher.AMP.BTP.ISV.Facade.PromotionFacade();
            var result = facade.GetSecKillPromotion(commodityId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckSecKillBuy(Guid commodityId)
        {
            Jinher.AMP.BTP.ISV.Facade.PromotionFacade facade = new Jinher.AMP.BTP.ISV.Facade.PromotionFacade();
            var result = facade.CheckSecKillBuy(commodityId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GoldPayCommodityOrder(GoldPayDTO goldDTO)
        {
            AppIdOwnerIdTypeDTO appDTO = null;

            ContextDTO contextDTO = ContextDTO;
            try
            {
                appDTO = APPSV.Instance.GetAppOwnerInfo(goldDTO.appId, contextDTO);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取应用所属者接口异常。goldDTO：{0}", JsonHelper.JsonSerializer(goldDTO)), ex);
                return Json(new ResultDTO { ResultCode = 1, Message = "获取应用所属者接口异常" }, JsonRequestBehavior.AllowGet);
            }
            if (appDTO == null || appDTO.OwnerId == Guid.Empty)
            {
                return Json(new ResultDTO { ResultCode = 1, Message = "获取应用所属者空异常" }, JsonRequestBehavior.AllowGet);
            }

            string payorComment = string.Format("电商{0}", goldDTO.comName);
            string notifyUrl = string.Format("{0}PaymentNotify/Goldpay", CustomConfig.BtpDomain);
            PayOrderGoldDTO payOrderDTO = new PayOrderGoldDTO();
            payOrderDTO.AppId = goldDTO.appId;
            payOrderDTO.BizId = goldDTO.orderId;
            payOrderDTO.CouponCodes = goldDTO.couponCodes;
            payOrderDTO.CouponCount = (double)goldDTO.couponCount;
            payOrderDTO.Gold = Convert.ToInt64(goldDTO.gold * 1000);
            payOrderDTO.TotalCount = Convert.ToInt64(goldDTO.realprice * 1000);
            payOrderDTO.PayeeComment = payOrderDTO.PayorComment = payorComment;
            payOrderDTO.Password = goldDTO.goldpwd;
            payOrderDTO.NotifyUrl = notifyUrl;
            payOrderDTO.PayeeId = appDTO.OwnerId;
            try
            {
                FSP.Deploy.CustomDTO.ReturnInfoDTO<long> goldResult =
                    Jinher.AMP.BTP.TPS.FSPSV.Instance.PayByPayeeIdBatch(payOrderDTO, contextDTO);
                //(goldDTO.orderId, appDTO.OwnerId, (ulong)(goldDTO.realprice * 1000), payorComment, payorComment, goldDTO.goldpwd, goldDTO.appId, notifyUrl);
                if (goldResult.Code == 0)
                {
                    return Json(new ResultDTO { ResultCode = 0, Message = "Success" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new ResultDTO { ResultCode = 1, Message = goldResult.Message },
                        JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("金币支付接口异常。goldDTO：{0}", JsonHelper.JsonSerializer(goldDTO)), ex);
                return Json(new ResultDTO { ResultCode = 1, Message = "金币支付接口异常" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 退款详情页面
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl]
        public ActionResult RefundInfo()
        {
            return View();
        }

        [DealMobileUrl]
        public ActionResult RefundType(Guid shopId, Guid orderId, Guid? orderItemId = null)
        {
            if (ThirdECommerceHelper.IsWangYiYanXuan(shopId))
            {
                ViewBag.RefundInfo = YXOrderRefundHelper.GetOrderRefundAfterSalesInfo(orderId);
            }
            return View();
        }

        [DealMobileUrl]
        public ActionResult RefundType2(Guid shopId, Guid orderId, Guid? orderItemId = null)
        {
            if (ThirdECommerceHelper.IsWangYiYanXuan(shopId))
            {
                ViewBag.RefundInfo = YXOrderRefundHelper.GetOrderRefundAfterSalesInfo(orderId);
            }
            return View();
        }

        [DealMobileUrl]
        public ActionResult RefundTypeInfo()
        {
            return View();
        }

        /// <summary>
        /// 获取退款详情信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="userId"></param>
        /// <param name="sessionId"></param>
        /// <param name="orderItemId"></param>
        /// <returns></returns>
        public ActionResult GetOrderRefund(Guid orderId, Guid userId, string sessionId, Guid orderItemId)
        {
            Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade orderSV =
                new Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade();
            //orderSV.ContextDTO = context;
            try
            {
                var orderRefund = orderSV.GetOrderRefund(orderId, orderItemId);
                orderRefund.IsJdEclpOrder = new IBP.Facade.JdEclpOrderFacade().ISEclpOrder(orderId);
                JdOrderHelper.GetJdRefundInfo(orderRefund);


                SNOrderAfterSalesHelper.GetSNRefundInfo(orderRefund, orderId, orderItemId);
                return Json(orderRefund, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new SubmitOrderRefundDTO(), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 保存退货物流信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="RefundExpCo"></param>
        /// <param name="RefundExpOrderNo"></param>
        /// <param name="userId"></param>
        /// <param name="sessionId"></param>
        /// <param name="orderItemId"></param>
        /// <returns></returns>
        public ActionResult SaveRefundType(Guid orderId, string RefundExpCo, string RefundExpOrderNo, Guid userId, string sessionId, Guid orderItemId)
        {
            BTP.ISV.Facade.CommodityOrderFacade orderSV = new BTP.ISV.Facade.CommodityOrderFacade();
            try
            {
                var orderRefund = orderSV.AddOrderRefundExp(orderId, RefundExpCo, RefundExpOrderNo, orderItemId);
                return Json(orderRefund, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new ResultDTO { ResultCode = 1, Message = "保存退货方式接口异常" }, JsonRequestBehavior.AllowGet);
            }
        }

        [DealMobileUrl]
        public ActionResult OrderReview()
        {
            return View();
        }

        public ActionResult SavaeOrderReview(Guid ordeItemId, Guid appId, Guid userId, string sessionId, string content,
            string giveUrl)
        {
            BTP.ISV.Facade.ReviewFacade orderSV = new BTP.ISV.Facade.ReviewFacade();

            ReviewSDTO reviewDTO = new ReviewSDTO();
            reviewDTO.AppId = appId;
            reviewDTO.Details = content;
            reviewDTO.OrderItemId = ordeItemId;
            reviewDTO.UserId = userId;
            reviewDTO.SourceUrl = giveUrl;
            try
            {
                if (userId != Guid.Empty)
                {
                    UserNameIconDTO userInfo = CBCSV.Instance.GetUserNameIconDTO(userId);

                    if (userInfo != null)
                    {
                        reviewDTO.Name = userInfo.Name;
                        reviewDTO.UserHead = userInfo.HeadIcon;
                    }
                }

                var resultDTO = orderSV.SaveReview(reviewDTO, appId);
                return Json(resultDTO, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new ResultDTO { ResultCode = 1, Message = "保存评价接口异常" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取行为记录js的url
        /// </summary>
        /// <returns></returns>
        public ActionResult GetBehaviorRecordUrl()
        {
            ResultDTO result = new ResultDTO();
            result.ResultCode = 0;


            string brUrl = RedisHelper.Get<string>(RedisKeyConst.BehaviorRecordUrl);
            //SessionCache.Current.GetCache("BehaviorRecordUrl");
            if (brUrl != null && !string.IsNullOrWhiteSpace(brUrl))
            {
                result.Message = brUrl;
            }
            else
            {
                string url = Jinher.AMP.BTP.TPS.DSSSV.Instance.GetBehaviorRecordUrl();
                result.Message = url;
                RedisHelper.Set(RedisKeyConst.BehaviorRecordUrl, url);
                //SessionCache.Current.AddCache("BehaviorRecordUrl", url);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Obsolete("已废弃", false)]
        public ActionResult GetGoldCouponCount(System.Guid userId)
        {
            var result = Jinher.AMP.BTP.TPS.PromotionSV.Instance.GetUsersVoucherCount(userId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 活动预约
        /// </summary>
        /// <param name="outPromotionId">外部活动Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="sessionId"></param>
        /// <param name="verifyCode"></param>s
        /// <returns></returns>
        public ActionResult SaveMyPresellComdtyZPH(Guid outPromotionId, Guid userId, string sessionId, string verifyCode, Guid esAppId, Guid commodityId, Guid commodityStockId)
        {
            ISV.Facade.CommodityFacade commodityFacade = new ISV.Facade.CommodityFacade();
            var result = commodityFacade.SaveMyPresellComdtyZPH(outPromotionId, userId, verifyCode, esAppId, commodityId, commodityStockId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 核查用户是否可以预约
        /// </summary>
        /// <param name="presellComdtyId">预约商品id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="commodityId"></param>
        /// <param name="commodityStockId"></param>
        /// <returns></returns>
        public ActionResult CheckMyPresellComdty(Guid userId, Guid presellComdtyId, Guid commodityId, Guid commodityStockId)
        {
            Jinher.AMP.ZPH.Deploy.CustomDTO.MyPresellComdtyCDTO myPresellComdtyCdto = new Jinher.AMP.ZPH.Deploy.CustomDTO.MyPresellComdtyCDTO
            {
                userId = userId,
                presellComdtyId = presellComdtyId,
                commodityId = commodityId,
                commodityStockId = commodityStockId
            };
            var result = ZPHSV.Instance.CheckMyPresellComdty(myPresellComdtyCdto);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查看是否可以货到付款
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId">用户Id</param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public ActionResult CheckPayDelivery(Guid appId, Guid userId, string sessionId)
        {
            var result = new ResultDTO() { ResultCode = 0, Message = "yes" };
            Jinher.AMP.BTP.ISV.Facade.PaymentsFacade facade = new ISV.Facade.PaymentsFacade();
            var payments = facade.GetPayments(appId);
            bool hasPayDelivery = false;
            if (payments != null)
            {
                hasPayDelivery = payments.Count(c => c.PaymentsName == "货到付款") > 0;
            }
            if (!hasPayDelivery)
                result = new ResultDTO() { ResultCode = 1, Message = "no" };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckPayPattern(Guid esAppId, Guid userId, string sessionId, Guid appId)
        {
            var result = new DirectArrivalDTO();
            //在线支付情况下判断是否直接到账
            int tradeType = Jinher.AMP.BTP.TPS.FSPSV.GetTradeSettingInfo(esAppId);
            result.TradeType = tradeType;
            if (tradeType == 0)
            {
                result.GoldBal = FSPSV.Instance.GetBalance(userId);

                result.IsAllAppInZPH = ZPHSV.Instance.CheckAllAppInZPH(new List<Guid> { appId });
                if (result.IsAllAppInZPH)
                {
                    string methodInfo = this.GetType() + ".GetGoldCouponCount";
                    result.GoldCouponCount = PayModel.GetGoldCouponCount(userId, methodInfo);
                }
            }

            Jinher.AMP.BTP.ISV.Facade.PaymentsFacade facade = new ISV.Facade.PaymentsFacade();
            var payments = facade.GetPayments(appId);
            if (payments != null)
            {
                var hasPayDelivery = payments.Count(c => c.PaymentsName == "货到付款");
                if (hasPayDelivery > 0)
                {
                    result.IsHdfk = true;
                    result.Pattern = 0;
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查看是否可以货到付款
        /// </summary>
        /// <returns></returns>
        public ActionResult GetVerifyCodeZPH()
        {
            ISV.Facade.CommodityFacade facade = new ISV.Facade.CommodityFacade();
            var result = facade.GetVerifyCodeZPH();
            return File(result.Data, @"image/jpeg");
        }

        /// <summary>
        /// 个人主页
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl]
        public ActionResult MyHome()
        {
            return View();
        }

        /// <summary>
        /// 我的位置
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl]
        public ActionResult MyLocation()
        {
            //GetWxConfigSign();
            //Guid userId = Guid.Empty;
            //Guid.TryParse(Request["userId"], out userId);
            //string sessionId = Request["sessionId"];


            //ViewBag.MyBespeakUrl = CustomConfig.MyBespeakUrl;
            //UserInfoCountDTO uinfo = GetUserInfoCountData(userId, sessionId);
            //ViewBag.uinfo = "我的位置";

            return View();
        }

        public ActionResult GetUserInfoCount()
        {
            Guid userId = Guid.Empty;
            Guid.TryParse(Request["userId"], out userId);
            string sessionId = Request["sessionId"];
            Guid esAppId = new Guid(Request["esAppId"]);

            UserInfoCountDTO uinfo = GetUserInfoCountData(userId, sessionId, esAppId);
            return Json(uinfo);
        }

        public UserInfoCountDTO GetUserInfoCountData(Guid userId, string sessionId, Guid esAppId)
        {
            UserInfoCountDTO uinfo = null;


            if (userId == Guid.Empty || string.IsNullOrWhiteSpace(sessionId))
            {
                uinfo = new UserInfoCountDTO();
                return uinfo;
            }

            Jinher.AMP.BTP.Deploy.CustomDTO.UserSDTO uDto = null;
            try
            {
                var uFacade = new Jinher.AMP.BTP.ISV.Facade.BTPUserFacade();
                uDto = uFacade.GetUser(userId, Guid.Empty);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取用户信息异常。userId：{0}，appId：{1}", userId, Guid.Empty), ex);
            }

            try
            {
                var asFacade = new Jinher.AMP.BTP.ISV.Facade.AppSetFacade();
                uinfo = asFacade.GetUserInfoCount(userId, esAppId);
                if (uinfo == null)
                {
                    uinfo = new UserInfoCountDTO();
                }
                if (uDto != null)
                {
                    uinfo.PicUrl = uDto.PicUrl;
                    uinfo.UserName = uDto.UserName;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(
                    string.Format(
                        "MobileController中调用Jinher.AMP.BTP.ISV.Facade.AppSetFacade.GetUserInfoCount异常。userId：{0},sessionId:{1}",
                        userId, sessionId), ex);
            }
            if (uinfo == null)
            {
                uinfo = new UserInfoCountDTO();
            }
            return uinfo;
        }


        /// <summary>
        /// 批量删除购物车中的商品
        /// </summary>
        /// <param name="shopCartItemId">购物车Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public ActionResult DeleteCommoditysFromShoppingCart
            (System.Guid userId, System.Guid appId)
        {
            var resultEmpty = new ResultDTO();
            resultEmpty.Message = "请选择要删除的商品！";
            resultEmpty.ResultCode = 1;

            string idStr = Request["shopCartItemIds"];
            string[] strs = idStr.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (strs.Length == 0)
            {
                return Json(resultEmpty, JsonRequestBehavior.AllowGet);
            }

            List<Guid> shopCartItemIds = new List<Guid>();
            foreach (string s in strs)
            {
                Guid siid = Guid.Empty;
                Guid.TryParse(s, out siid);
                if (siid == Guid.Empty)
                {
                    continue;
                }
                shopCartItemIds.Add(siid);
            }
            if (shopCartItemIds.Count == 0)
            {
                return Json(resultEmpty, JsonRequestBehavior.AllowGet);
            }
            Jinher.AMP.BTP.ISV.Facade.ShoppingCartFacade facade = new ISV.Facade.ShoppingCartFacade();
            var result = facade.DeleteCommoditysFromShoppingCart(shopCartItemIds, userId, appId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 秒杀提醒
        /// </summary>
        /// <param name="commodityId">商品id</param>
        /// <param name="outPromotionId">活动id</param>
        /// <param name="uId">用户id</param>
        /// <param name="sId">sessionid</param>
        /// <returns></returns>
        public ActionResult SendNotificationsZPH(Guid commodityId, Guid outPromotionId, Guid uId, string sId,
            Guid esAppId)
        {
            ISV.Facade.CommodityFacade cf = new ISV.Facade.CommodityFacade();
            BTP.Deploy.CustomDTO.ResultDTO ret = cf.SendNotificationsZPH(commodityId, outPromotionId, esAppId);

            return Json(new { Message = ret.Message, Code = ret.ResultCode }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取提醒状态
        /// </summary>
        /// <param name="uId">用户id</param>
        /// <param name="skId">秒杀商品id</param>
        /// <returns></returns>
        public ActionResult SetNotificationState(Guid uId, Guid skId)
        {
            ZPH.Deploy.CustomDTO.SeckillInfoCDTO seckillInfoCDTO = new ZPH.Deploy.CustomDTO.SeckillInfoCDTO();
            seckillInfoCDTO.seckillId = skId;
            seckillInfoCDTO.userId = uId;

            ZPH.Deploy.CustomDTO.SeckillComdtyInfoCDTO scInfoCDTO =
                Jinher.AMP.BTP.TPS.ZPHSV.Instance.GetSeckillInfoById(seckillInfoCDTO);
            if (scInfoCDTO == null)
            {
                scInfoCDTO = new ZPH.Deploy.CustomDTO.SeckillComdtyInfoCDTO();
            }

            bool notification = scInfoCDTO.isReminded;

            return Json(new { state = notification }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 校验手机号是否易捷员工
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <returns></returns>
        public ActionResult CheckInviterMobile(string phone)
        {
            YJEmployeeSearchDTO search = new YJEmployeeSearchDTO();
            if (phone == string.Empty)
            {
                return Json(new ResultDTO { ResultCode = 1, Message = "未获取到邀请人手机号" });
            }
            //if (ContextDTO.LoginUserID == Guid.Empty)
            //{
            //    return Json(new ResultDTO { ResultCode = 1, Message = "获取用户信息失败" });
            //}
            search.Phone = phone;
            var facade = new YJEmployeeFacade();
            if (facade.GetYJEmployeeListExtBySearch(search).Data.Count <= 0)
            {
                return Json(new ResultDTO { ResultCode = 1, Message = "手机号输入有误，请与邀请人联系！" });
            }
            return Json(new ResultDTO { ResultCode = 0 },JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 创建订单
        /// </summary>
        /// <returns></returns>
        public ActionResult SaveCommodityOrderNew(List<OrderSDTO> orderList, string sessionId, string isHYL, string storeCouponId, decimal StoreCouponCommdityPrice, decimal StoreCouponPrice, int StoreCouponCommdityCount)
        {
            if (orderList == null || !orderList.Any())
            {
                return Json(new OrderResultDTO { ResultCode = 1, Message = "提交错误" }, JsonRequestBehavior.AllowGet);
            }

            if (isHYL == "1" && orderList.Count > 1)
            {
                return Json(new OrderResultDTO { ResultCode = 1, Message = "好运来不支持合并支付" }, JsonRequestBehavior.AllowGet);
            }

            //// 风险检查
            //var receiptPhone = orderList.First().ReceiptPhone;
            //if (string.IsNullOrEmpty(receiptPhone))
            //{
            //    return Json(new OrderResultDTO { ResultCode = 1, Message = "提交错误，收货电话不能为空。" }, JsonRequestBehavior.AllowGet);
            //}
            //if (!OrderRiskHelper.CheckCouponRisk(receiptPhone, Request.UserHostAddress))
            //{
            //    return Json(new OrderResultDTO { ResultCode = 1, Message = "提交错误，用户信息异常。" }, JsonRequestBehavior.AllowGet);
            //}


            // 平台暂未实现
            //if (!OrderRiskHelper.CheckAddressscore(orderList.First().Province + orderList.First().City + orderList.First().District + orderList.First().Street + orderList.First().ReceiptAddress))
            //{
            //    return Json(new OrderResultDTO { ResultCode = 1, Message = "提交错误，用户信息异常。" }, JsonRequestBehavior.AllowGet);
            //}

            if (isHYL == "1")
            {
                Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade facade = new ISV.Facade.CommodityOrderFacade();
                var result = facade.SavePrizeCommodityOrder(orderList[0]);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                orderList.ForEach(c =>
                {
                    c.Scheme = Request.Url.Scheme;
                    c.StoreCouponId = new Guid(storeCouponId.Trim());
                    c.StoreCouponPrice = StoreCouponPrice;                //跨店优惠券的金额
                    c.StoreCouponCommdityCount = StoreCouponCommdityCount;//跨店的商品的数量，用以算最后一单
                    c.StoreCouponCommdityPrice = StoreCouponCommdityPrice;//跨店的商品的总金额
                });


                var facade = new ISV.Facade.CommodityOrderFacade();

                if (orderList.Count > 1)//多个订单
                {
                    var result = facade.SaveSetCommodityOrderNew(orderList);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else ////一个订单
                {
                    var order = orderList[0];
                    if(order.EsAppId == Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId)
                    {
                        if (order.YJCouponPrice > 0 && order.YJCouponIds != null && order.YJCouponIds.Count > 0
                            && CacheHelper.MallApply.GetMallTypeListByEsAppId(order.EsAppId).Any(_ => _.Id == orderList[0].AppId && _.Type == 1))
                        {
                            return Json(new OrderResultDTO { ResultCode = 1, Message = "请重新下单" }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    var result = facade.SaveCommodityOrder(order);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

            }
        }

        //将得到的订单id回写到JdOrderItem表中(订单id为单个时)
        public void UpdateJdorderitem(string MainOrderId, string CommodityOrderId)
        {
            Jinher.AMP.BTP.IBP.Facade.JdOrderItemFacade jdorderitemfacade = new Jinher.AMP.BTP.IBP.Facade.JdOrderItemFacade();
            List<JdCache> arr = JsonHelper.JsonDeserialize<List<JdCache>>(SessionHelper.Get("JdPorder"));
            if (arr != null)
            {
                if (arr.Count() > 0)
                {
                    foreach (var item in arr)
                    {
                        JdOrderItemDTO model = new JdOrderItemDTO();
                        model.JdPorderId = item.JdporderId;
                        model.State = 1;//预占状态的
                        var jdorderitem = jdorderitemfacade.GetJdOrderItemList(model).FirstOrDefault();
                        if (jdorderitem != null)
                        {
                            jdorderitem.MainOrderId = MainOrderId;
                            jdorderitem.CommodityOrderId = CommodityOrderId;
                            //主要的订单
                            jdorderitemfacade.UpdateJdOrderItem(jdorderitem);
                        }

                    }
                }
            }

        }


        //将得到的订单id回写到JdOrderItem表中(订单id为多个时)
        public void UpdateJdorderitems(string MainOrderId, string CommodityOrderId, string AppId)
        {
            List<JdCache> arr = JsonHelper.JsonDeserialize<List<JdCache>>(SessionHelper.Get("JdPorder"));
            Jinher.AMP.BTP.IBP.Facade.CommodityOrderFacade commodityorderfacade = new Jinher.AMP.BTP.IBP.Facade.CommodityOrderFacade();
            Jinher.AMP.BTP.IBP.Facade.JdOrderItemFacade jdorderitemfacade = new Jinher.AMP.BTP.IBP.Facade.JdOrderItemFacade();
            if (arr != null)
            {
                if (arr.Count() > 0)
                {
                    foreach (var item in arr)
                    {
                        if (item.AppId == AppId)
                        {
                            List<string> objlist = new List<string>();
                            objlist.Add(item.JdporderId);
                            var obj = jdorderitemfacade.GetList(objlist).FirstOrDefault();
                            obj.MainOrderId = MainOrderId;
                            obj.CommodityOrderId = CommodityOrderId;
                            //主要的订单
                            jdorderitemfacade.UpdateJdOrderItem(obj);
                        }
                    }

                }
            }

        }

        /// <summary>
        /// 创建京东订单
        /// </summary>
        /// <returns></returns>
        public JsonResult SaveJdCommodityOrder(List<OrderSDTO> orderList, string isHYL)
        {
            try
            {
                LogHelper.Info(string.Format("SaveJdCommodityOrder{0}________________{1}", orderList.Count(), isHYL));
                string Appids = CustomConfig.AppIds;
                LogHelper.Info(string.Format("Appids{0}", Appids));
                List<string> Appidlist = null;
                if (!string.IsNullOrEmpty(Appids))
                {
                    Appidlist = Appids.Split(new char[] { ',' }).ToList();
                }
                LogHelper.Info(string.Format("Appidlist的数量{0}", Appidlist.Count()));
                string orderPriceSnap = null;
                string sku = null;
                ResultDTO result = null;
                List<JdCache> objlist = new List<JdCache>();
                Jinher.AMP.BTP.IBP.Facade.CommodityFacade facade = new Jinher.AMP.BTP.IBP.Facade.CommodityFacade();
                Jinher.AMP.BTP.IBP.Facade.JdOrderItemFacade jdorderitemfacade = new Jinher.AMP.BTP.IBP.Facade.JdOrderItemFacade();
                Jinher.AMP.BTP.IBP.Facade.JdJournalFacade jdjournalfacade = new Jinher.AMP.BTP.IBP.Facade.JdJournalFacade();
                Jinher.AMP.BTP.IBP.Facade.JdlogsFacade jdllogsfacade = new Jinher.AMP.BTP.IBP.Facade.JdlogsFacade();
                LogHelper.Info(string.Format("orderList的数量{0}", orderList.Count()));
                if (orderList == null || !orderList.Any())
                {
                    return Json(new OrderResultDTO { ResultCode = 1, Message = "提交错误" }, JsonRequestBehavior.AllowGet);
                }
                if (isHYL == "1" && orderList.Count > 1)
                {
                    return Json(new OrderResultDTO { ResultCode = 1, Message = "好运来不支持合并支付" }, JsonRequestBehavior.AllowGet);
                }
                List<CommoditySummaryDTO> errorCommodities = new List<CommoditySummaryDTO>();
                if (Appidlist != null && Appidlist.Count > 0 && orderList.Count() > 0)
                {
                    LogHelper.Info(string.Format("Appidlist的数量{0},orderList的数量{1}", Appidlist.Count(), orderList.Count()));
                    foreach (var item in orderList)
                    {
                        if (item.ShoppingCartItemSDTO.Count() > 0)
                        {
                            orderPriceSnap = null;
                            sku = null;
                            foreach (var _item in item.ShoppingCartItemSDTO)
                            {

                                LogHelper.Info(string.Format("ShoppingCartItemSDTO的数据{0}", JsonHelper.JsonSerializer<List<ShoppingCartItemSDTO>>(item.ShoppingCartItemSDTO)));
                                Jinher.AMP.BTP.Deploy.CommodityDTO commodity = facade.GetCommodityDetail(_item.Id);
                                LogHelper.Info(string.Format("京东日志1:{0}:{1}", commodity.JDCode, commodity.AppId));
                                if (!string.IsNullOrWhiteSpace(commodity.JDCode) && Appidlist.Contains(commodity.AppId.ToString().ToUpper()))
                                {
                                    orderPriceSnap += "{'price':" + commodity.CostPrice + ",'skuId':" + commodity.JDCode + "},";
                                    sku += "{'skuId':" + commodity.JDCode + ", 'num':" + _item.CommodityNumber + ",'bNeedAnnex':true, 'bNeedGift':false},";
                                    LogHelper.Info(string.Format("京东日志2:{0}:{1}", orderPriceSnap, sku));
                                }
                                if (string.IsNullOrWhiteSpace(commodity.JDCode) && Appidlist.Contains(commodity.AppId.ToString().ToUpper()))
                                {
                                    Jinher.AMP.BTP.Deploy.JdlogsDTO model = new Jinher.AMP.BTP.Deploy.JdlogsDTO();
                                    model.Id = Guid.NewGuid();
                                    model.Content = (APPSV.GetAppName(commodity.AppId) + "App中" + commodity.Name + "商品的备注编码不存在，请尽快补充填写~");
                                    model.Remark = string.Empty;
                                    model.AppId = commodity.AppId;
                                    model.ModifiedOn = DateTime.Now;
                                    model.SubTime = DateTime.Now;
                                    model.Isdisable = false;
                                    jdllogsfacade.SaveJdlogs(model);
                                    bool falg = EmailHelper.SendEmail("京东错误日志", model.Content, "yijieds@126.com");

                                    var errorCommodity = new CommoditySummaryDTO();
                                    errorCommodity.Id = commodity.Id;
                                    errorCommodity.Name = commodity.Name;
                                    errorCommodity.PicturesPath = commodity.PicturesPath;
                                    errorCommodity.Price = _item.Price;
                                    errorCommodity.Sku = _item.SizeAndColorId;
                                    errorCommodity.ShopCartItemId = _item.ShopCartItemId;
                                    errorCommodities.Add(errorCommodity);

                                    return Json(new OrderResultDTO { ResultCode = 2, Message = "商品已售馨,请选择其他商品", ErrorCommodities = errorCommodities }, JsonRequestBehavior.AllowGet);
                                }
                            }

                            LogHelper.Info(string.Format("京东日志3:{0}:{1}", orderPriceSnap, sku));
                            if (!string.IsNullOrEmpty(orderPriceSnap) && !string.IsNullOrEmpty(sku))
                            {
                                orderPriceSnap = orderPriceSnap.Remove(orderPriceSnap.Length - 1, 1);
                                sku = sku.Remove(sku.Length - 1, 1);
                                orderPriceSnap = "[" + orderPriceSnap + "]";
                                sku = "[" + sku + "]";
                                string thirdOrder = Guid.NewGuid().ToString();
                                if (string.IsNullOrEmpty(item.StreetCode))
                                {
                                    item.StreetCode = "0";
                                }
                                //获取京东编号
                                string jdporderId = JdHelper.GetJDOrder(thirdOrder, orderPriceSnap, sku, item.ReceiptUserName, item.ReceiptAddress, item.ReceiptPhone, "yijieds@126.com", item.ProvinceCode, item.CityCode, item.DistrictCode, item.StreetCode);
                                LogHelper.Info(string.Format("京东日志4:{0}:{1}", orderPriceSnap, sku));
                                if (!string.IsNullOrWhiteSpace(jdporderId))
                                {
                                    #region 京东下单情况
                                    objlist.Add(new JdCache { JdporderId = jdporderId, AppId = item.AppId.ToString() });
                                    //记录页面级缓存
                                    JdOrderItemDTO jdorderitemdto = new JdOrderItemDTO()
                                    {
                                        Id = Guid.NewGuid(),
                                        JdPorderId = jdporderId,
                                        TempId = Guid.Parse(thirdOrder),
                                        JdOrderId = Guid.Empty.ToString(),
                                        MainOrderId = Guid.Empty.ToString(),
                                        CommodityOrderId = Guid.Empty.ToString(),
                                        State = Convert.ToInt32(JdEnum.YZ),
                                        StateContent = new EnumHelper().GetDescription(JdEnum.YZ),
                                        SubTime = DateTime.Now,
                                        ModifiedOn = DateTime.Now
                                    };
                                    result = jdorderitemfacade.SaveJdOrderItem(jdorderitemdto);
                                    if (result.isSuccess == true)
                                    {
                                        JdJournalDTO jdjournaldto = new JdJournalDTO()
                                        {
                                            Id = Guid.NewGuid(),
                                            JdPorderId = jdporderId,
                                            TempId = Guid.Parse(thirdOrder),
                                            JdOrderId = Guid.Empty.ToString(),
                                            MainOrderId = Guid.Empty.ToString(),
                                            CommodityOrderId = Guid.Empty.ToString(),
                                            Name = "京东统一下单接口",
                                            Details = "初始状态为" + Convert.ToInt32(JdEnum.YZ),
                                            SubTime = DateTime.Now
                                        };
                                        result = jdjournalfacade.SaveJdJournal(jdjournaldto);
                                        if (result.isSuccess != true)
                                        {
                                            return Json(new OrderResultDTO { ResultCode = 1, Message = result.Message }, JsonRequestBehavior.AllowGet);
                                        }

                                    }
                                    else
                                    {
                                        return Json(new OrderResultDTO { ResultCode = 1, Message = result.Message }, JsonRequestBehavior.AllowGet);
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region 记录京东日志
                                    string Jdlog = SessionHelper.Get("Jdlogs");
                                    string resultCode = SessionHelper.Get("resultCode");
                                    if (resultCode == "3017") //账户异常情况特殊
                                    {
                                        bool falg = EmailHelper.SendEmail("京东错误日志", "您的京东账户余额不足,请充值!", "yijieds@126.com");
                                        if (falg == true)
                                        {
                                            Jinher.AMP.BTP.Deploy.JdlogsDTO model = new Jinher.AMP.BTP.Deploy.JdlogsDTO();
                                            model.Id = Guid.NewGuid();
                                            model.Content = "您的京东账户余额不足,请充值!";
                                            model.Remark = string.Empty;
                                            model.AppId = Guid.Empty;
                                            model.ModifiedOn = DateTime.Now;
                                            model.SubTime = DateTime.Now;
                                            model.Isdisable = false;
                                            jdllogsfacade.SaveJdlogs(model);
                                        }

                                        var commonlist = item.ShoppingCartItemSDTO;
                                        if (commonlist != null && commonlist.Count() > 0)
                                        {
                                            foreach (var itemlog in commonlist)
                                            {
                                                var errorCommodity = new CommoditySummaryDTO();
                                                errorCommodity.Id = itemlog.Id;
                                                errorCommodity.Name = itemlog.Name;
                                                errorCommodity.PicturesPath = itemlog.Pic;
                                                errorCommodity.Price = itemlog.Price;
                                                errorCommodity.Sku = itemlog.SizeAndColorId;
                                                errorCommodity.ShopCartItemId = itemlog.ShopCartItemId;
                                                errorCommodities.Add(errorCommodity);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(Jdlog))
                                        {
                                            string num = null;
                                            var matches = System.Text.RegularExpressions.Regex.Matches(Jdlog, @"(\d+)");
                                            int count = 0;
                                            foreach (Match match in matches)
                                            {
                                                if (count == 0)
                                                {
                                                    num = match.Value;
                                                }
                                                count++;
                                            }
                                            var commonlist = item.ShoppingCartItemSDTO;
                                            if (commonlist != null && commonlist.Count() > 0)
                                            {
                                                foreach (var itemlog in commonlist)
                                                {
                                                    Jinher.AMP.BTP.Deploy.CommodityDTO commodity = facade.GetCommodityDetail(itemlog.Id);
                                                    if (commodity.JDCode == num.ToString() && Appidlist.Contains(commodity.AppId.ToString().ToUpper()))
                                                    {
                                                        var errorCommodity = new CommoditySummaryDTO();
                                                        errorCommodity.Id = commodity.Id;
                                                        errorCommodity.Name = commodity.Name;
                                                        errorCommodity.PicturesPath = commodity.PicturesPath;
                                                        errorCommodity.Price = itemlog.Price;
                                                        errorCommodity.Sku = itemlog.SizeAndColorId;
                                                        errorCommodity.ShopCartItemId = itemlog.ShopCartItemId;
                                                        errorCommodities.Add(errorCommodity);

                                                        string Content = null;
                                                        Content += (APPSV.GetAppName(commodity.AppId) + "App中" + itemlog.Name) + "商品[" + commodity.JDCode + "]";
                                                        if (resultCode == "2004")
                                                        {
                                                            Content += "京东商品池中不存在";
                                                        }
                                                        else if (resultCode == "3019")
                                                        {
                                                            string str = Jdlog;
                                                            if (!string.IsNullOrEmpty(str))
                                                            {
                                                                Content += "价格错误,";
                                                                int num1 = str.IndexOf('[');
                                                                int num2 = str.IndexOf(']');
                                                                string strjdprice = str.Substring(num1 + 1, (num2 - num1 - 1));
                                                                string[] arr = strjdprice.Split(new char[] { '=' });
                                                                Content += "京东价" + arr[1] + "元," + "易捷价" + commodity.CostPrice + "元";
                                                            }

                                                        }
                                                        else if (resultCode.ToString() == "3008")
                                                        {
                                                            Content += "已售馨";
                                                        }
                                                        else
                                                        {
                                                            Content += "异常信息:" + Jdlog;
                                                        }
                                                        bool falg = EmailHelper.SendEmail("京东错误日志", Content, "yijieds@126.com");
                                                        //if (falg == true)
                                                        //{
                                                        Jinher.AMP.BTP.Deploy.JdlogsDTO model = new Jinher.AMP.BTP.Deploy.JdlogsDTO();
                                                        model.Id = Guid.NewGuid();
                                                        model.Content = Content;
                                                        model.Remark = string.Empty;
                                                        model.AppId = itemlog.AppId;
                                                        model.ModifiedOn = DateTime.Now;
                                                        model.SubTime = DateTime.Now;
                                                        model.Isdisable = false;
                                                        var logResult = jdllogsfacade.SaveJdlogs(model);
                                                        //}
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    #endregion

                                    #region 获取京东订单单号失败的情况
                                    if (objlist.Count() > 0)
                                    {
                                        //删除JdOrderItem已操作的内容
                                        foreach (var jdorderid in objlist)
                                        {
                                            //京东确认取消订单
                                            bool flag = JdHelper.OrderCancel(jdorderid.JdporderId);
                                            if (flag == true)
                                            {
                                                List<string> jdorder = new List<string>();
                                                jdorder.Add(jdorderid.JdporderId);
                                                //删除京东对应订单
                                                var res = jdorderitemfacade.DeleteJdOrderItem(jdorder);
                                                if (res.isSuccess == true)
                                                {
                                                    JdJournalDTO jdjournaldto = new JdJournalDTO()
                                                    {
                                                        Id = Guid.NewGuid(),
                                                        JdPorderId = jdporderId,
                                                        TempId = Guid.Parse(thirdOrder),
                                                        JdOrderId = Guid.Empty.ToString(),
                                                        MainOrderId = Guid.Empty.ToString(),
                                                        CommodityOrderId = Guid.Empty.ToString(),
                                                        Name = "京东确认取消订单",
                                                        Details = "删除JdOrderItem表中相应的内容",
                                                        SubTime = DateTime.Now
                                                    };
                                                    jdjournalfacade.SaveJdJournal(jdjournaldto);
                                                }
                                            }
                                        }

                                    }

                                    #endregion

                                    LogHelper.Error("商品已售馨,请选择其他商品,Jdlogs：" + Jdlog + " resultCode:" + resultCode);
                                    return Json(new OrderResultDTO { ResultCode = 2, Message = "商品已售馨,请选择其他商品", ErrorCommodities = errorCommodities }, JsonRequestBehavior.AllowGet);
                                }

                            }

                        }
                    }
                }

                /*
                if (objlist.Count > 0)
                {

                    //记录页面级缓存
                    //先删除原有缓存
                    SessionHelper.Del("JdPorder");
                    string str = null;
                    foreach (var item in objlist)
                    {
                        str += "{\"JdporderId\":\"" + item.JdporderId + "\",\"AppId\":\"" + item.AppId + "\"},";
                    }
                    str = str.Remove(str.Length - 1, 1);
                    str = "[" + str + "]";
                    //记录现存的缓存数据
                    SessionHelper.Add("JdPorder", str);
                }
                else
                {
                    //没有京东产品要清空内容
                    SessionHelper.Del("JdPorder");
                }
                */
                if (objlist.Count > 0)
                {

                    //记录页面级缓存
                    //新dll
                    RedisHelperNew.Remove(RedisKeyConst.UserOrder_JdPOrderIdList, orderList.FirstOrDefault().UserId.ToString(), "BTPCache");
                    string str = null;
                    foreach (var item in objlist)
                    {
                        str += "{\"JdporderId\":\"" + item.JdporderId + "\",\"AppId\":\"" + item.AppId + "\"},";
                    }
                    str = str.Remove(str.Length - 1, 1);
                    str = "[" + str + "]";
                    //记录现存的缓存数据
                    //新dell
                    RedisHelperNew.RegionSet(RedisKeyConst.UserOrder_JdPOrderIdList, orderList.FirstOrDefault().UserId.ToString(), str, 1, "BTPCache");
                    //新dll
                    string json = RedisHelperNew.RegionGet<string>(RedisKeyConst.UserOrder_JdPOrderIdList, orderList.FirstOrDefault().UserId.ToString(), "BTPCache");
                    LogHelper.Info("【京东订单-获取Redis】userId--->[" + orderList.FirstOrDefault().UserId.ToString() + "],缓存Json值--->【" + json + "】");
                    //如果为空则说明缓存写入失败
                    if (string.IsNullOrEmpty(json))
                    {
                        return Json(new OrderResultDTO { ResultCode = 1, Message = "操作异常，请重试" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    //新dll
                    RedisHelperNew.Remove(RedisKeyConst.UserOrder_JdPOrderIdList, orderList.FirstOrDefault().UserId.ToString(), "BTPCache");
                }

            }
            catch (Exception ex)
            {
                LogHelper.Info(string.Format("京东异常日志:{0}", ex));
                return Json(new OrderResultDTO { ResultCode = 1, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new OrderResultDTO { ResultCode = 0, Message = null }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult PayUrl()
        {
            return View();
        }


        /// <summary>
        /// 创建在线支付 add by XJ
        /// </summary>
        /// <returns></returns>
        public JsonResult PayUrl(PayOrderToFspDTO model)
        {

            CommodityOrderFacade facade = new CommodityOrderFacade();
            if (model != null)
            {
                model.Scheme = Request.Url.Scheme;
            }
            var result = facade.GetPayUrl(model);
            return Json(new { data = result });
        }


        /// <summary>
        /// 多app运费计算
        /// </summary>
        /// <param name="freightTo">运送到</param>
        /// <param name="isSelfTake">是否自提</param>OptimalCouponParam
        /// <param name="templateCounts">模板数据集合</param>
        /// <param name="hasAddress">是否有收货地址</param>
        /// <param name="issc">是否显示优惠券</param>
        /// <param name="uid">当前用户Id</param>
        /// <param name="isScore">是否计算积分</param>
        /// <returns>运费计算结果</returns>
        public ActionResult CalFreightMultiApps(List<ComScoreCheckDTO> templateCounts, string freightTo,
            string isSelfTake, string uid, string issc, bool hasAddress, bool isScore, string setMealId, Dictionary<Guid, decimal> coupons, bool useYjb, List<Guid> yjCoupons, Guid? esAppId)
        {
            using (StopwatchLogHelper.BeginScope("Mobile.CalFreightMultiApps"))
            {
                List<OrderReduction> reductions = new List<OrderReduction>();
                var hasPromotion = templateCounts.Any(p => p.IsPromotion);

                var input = new YJB.Deploy.CustomDTO.OrderInsteadCashInputDTO();
                input.UserId = new Guid(uid);
                input.Commodities = templateCounts.Select(t => new Jinher.AMP.YJB.Deploy.CustomDTO.OrderInsteadCashInputCommodityDTO
                {
                    AppId = t.AppId,
                    Id = t.CommodityId,
                    Price = t.RealPrice,
                    Number = t.Num,
                }).ToList();
                var yjInfo = Jinher.AMP.BTP.TPS.Helper.YJBHelper.GetCommodityCashPercent(MobileCookies.AppId, input);

                // 易捷币信息
                var yjbInfo = new YJB.Deploy.CustomDTO.OrderInsteadCashDTO { Enabled = false };
                if (!hasPromotion)
                {
                    yjbInfo = yjInfo.YJBInfo;
                }

                // 抵现卷
                var yjCouponInfo = yjInfo.YJCouponInfo;

                // 优惠券
                ReturnInfo<List<CouponNewDTO>> couponResult = new ReturnInfo<List<CouponNewDTO>>();
                if (Convert.ToInt32(issc) != 1 && !hasPromotion)//好运来和促销活动不使用优惠券
                {
                    using (StopwatchLogHelper.BeginScope("GetOptimalCoupon"))
                    {
                        couponResult = GetOptimalCoupon(templateCounts, new Guid(uid));
                        if (couponResult.Data != null && couponResult.Data.Any())
                        {
                            couponResult.Data.ForEach(c => reductions.Add(new OrderReduction() { AppId = c.ShopId, Reduction = c.Cash }));
                        }
                    }
                }

                #region 修改为优惠券可以算包邮金额-20180622
                //if ((coupons == null || coupons.Count == 0))
                //{
                //    if (couponResult.Data != null && couponResult.Data.Count > 0)
                //    {
                //        coupons = couponResult.Data.Select(_ => new { AppId = _.ShopId, Price = _.Cash }).ToDictionary(_ => _.AppId, _ => _.Price);
                //    }
                //}
                //else
                //{
                //    foreach (var appId in coupons.Keys)
                //    {
                //        if (coupons[appId] == 0)
                //        {
                //            var tem = couponResult.Data.Where(_ => _.ShopId == appId).FirstOrDefault();
                //            couponResult.Data.Remove(tem);
                //        }
                //    }
                //}
                coupons = null;
                #endregion

                // 运费
                Jinher.AMP.BTP.Deploy.CustomDTO.FreighMultiAppResultDTO fmResult = new FreighMultiAppResultDTO();
                if (hasAddress)
                {

                    //Dictionary<string, string> dic = getoldProvice();
                    //foreach (var item in dic)
                    //{
                    //    if (item.Key.Contains(freightTo.Trim()))
                    //    {
                    //        freightTo = item.Value;
                    //    }
                    //}

                    using (StopwatchLogHelper.BeginScope("FreighMultiAppResult"))
                    {
                        freightTo = ProvinceCityHelper.GetProvinceCodeByName(freightTo);
                        List<Jinher.AMP.BTP.Deploy.CustomDTO.TemplateCountDTO> tempCounts =
                            (from tc in templateCounts
                             select new TemplateCountDTO()
                             {
                                 CommodityId = tc.CommodityId,
                                 Count = tc.Num,
                                 Price = tc.RealPrice
                             }).ToList();
                        Jinher.AMP.BTP.ISV.Facade.CommodityFacade facade = new ISV.Facade.CommodityFacade();
                        fmResult = facade.CalFreightMultiApps(freightTo, Convert.ToInt32(isSelfTake), tempCounts, coupons, useYjb ? yjbInfo : null, yjCoupons);
                    }
                }

                OrderScoreCheckResultDTO scoreResult = new OrderScoreCheckResultDTO();
                if (isScore && !(esAppId.HasValue && esAppId == YJB.Deploy.CustomDTO.YJBConsts.YJAppId))
                {
                    OrderScoreCheckDTO pdto = new OrderScoreCheckDTO();
                    pdto.UserId = new Guid(uid);
                    pdto.EsAppId = MobileCookies.AppId;
                    pdto.Coms = templateCounts;
                    pdto.Reductions = reductions;

                    Jinher.AMP.BTP.ISV.Facade.ScoreSettingFacade ssFacade =
                        new Jinher.AMP.BTP.ISV.Facade.ScoreSettingFacade();
                    ResultDTO<OrderScoreCheckResultDTO> usResult = ssFacade.OrderScoreCheck(pdto);
                    if (usResult != null && usResult.ResultCode == 0)
                    {
                        scoreResult = usResult.Data;
                    }
                }

                return Json(new
                {
                    FreightResult = fmResult,
                    CouponResult = couponResult,
                    ScoreResult = scoreResult,
                    YJBInfo = yjbInfo,
                    YJCouponInfo = yjCouponInfo,
                    hasPromotion//使用了 活动，不能使用任何优惠，现在用于跨店满减券的判断
                    //YJCouponInfo = new Jinher.AMP.YJB.Deploy.CustomDTO.YJCouponCanInsteadCashDTO
                    //{
                    //    CanCombinabled = true,
                    //    CanMultipuled = true,
                    //    YJCoupons = new List<AMP.YJB.Deploy.CustomDTO.MUserCouponDto> { 
                    //        new Jinher.AMP.YJB.Deploy.CustomDTO.MUserCouponDto
                    //        {
                    //            Description="test1", Scope=0, Price= 88, Name= "测试1", LimitAmount= -1, Id= Guid.NewGuid(), CommodityId= null,EndTime=DateTime.Now.AddDays(10)
                    //        },
                    //        new Jinher.AMP.YJB.Deploy.CustomDTO.MUserCouponDto
                    //        {
                    //            Description="test2", Scope=1, Price= 99, Name= "测试2", LimitAmount= 99, Id= Guid.NewGuid(), CommodityId= null,EndTime=DateTime.Now.AddDays(20)
                    //        },
                    //        new Jinher.AMP.YJB.Deploy.CustomDTO.MUserCouponDto
                    //        {
                    //            Description="test3", Scope=2, Price= 100, Name= "测试3", LimitAmount= -1, Id= Guid.NewGuid(), CommodityId= Guid.NewGuid(),EndTime=DateTime.Now.AddDays(30)
                    //        }
                    //    },
                    //    UnusableYJCoupons = new List<Jinher.AMP.YJB.Deploy.CustomDTO.MUserCouponDto> { 
                    //        new Jinher.AMP.YJB.Deploy.CustomDTO.MUserCouponDto
                    //        {
                    //            Description="test1", Scope=0, Price= 88, Name= "测试不可用1", LimitAmount= -1, Id= Guid.NewGuid(), CommodityId= null,EndTime=DateTime.Now.AddDays(10)
                    //        },
                    //        new Jinher.AMP.YJB.Deploy.CustomDTO.MUserCouponDto
                    //        {
                    //            Description="test2", Scope=1, Price= 99, Name= "测试不可用2", LimitAmount= 99, Id= Guid.NewGuid(), CommodityId= null,EndTime=DateTime.Now.AddDays(20)
                    //        },
                    //        new Jinher.AMP.YJB.Deploy.CustomDTO.MUserCouponDto
                    //        {
                    //            Description="test3", Scope=2, Price= 100, Name= "测试不可用3", LimitAmount= -1, Id= Guid.NewGuid(), CommodityId= Guid.NewGuid(),EndTime=DateTime.Now.AddDays(30)
                    //        }
                    //    }
                    //}
                }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 生成微信config签名
        /// </summary>
        /// <returns></returns>
        public void GetWxConfigSign()
        {
            ViewBag.wxConfigTimestamp = "";
            ViewBag.wxConfigNonceStr = "";
            ViewBag.wxConfigSign = "";

            string token = WeixinOAuth2.GetAccessToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                return;
            }
            string ticket = WeixinOAuth2.GetJsapiTicket(token);
            if (string.IsNullOrWhiteSpace(ticket))
            {
                return;
            }

            string url = Request.Url.ToString();
            string timestamp = WeixinFunction.GetTimestamp();
            string nonceStr = WeixinFunction.GetNoncestr();

            // 签名参数集合
            SortedDictionary<string, object> sdParams = new SortedDictionary<string, object>();
            sdParams.Add("url", url);
            sdParams.Add("jsapi_ticket", ticket);
            sdParams.Add("nonceStr", nonceStr);
            sdParams.Add("timeStamp", timestamp);
            // 生成微信参数签名
            string wxSign = WeixinFunction.BuildMysign(sdParams).ToUpper();
            ViewBag.wxConfigSign = wxSign;
            ViewBag.wxConfigTimestamp = timestamp;
            ViewBag.wxConfigNonceStr = nonceStr;
        }

        /// <summary>
        /// 生成微信config签名-异步调用
        /// </summary>
        /// <returns></returns>
        public JsonResult GetWxConfigSignAsyc()
        {
            string token = WeixinOAuth2.GetAccessToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }
            string ticket = WeixinOAuth2.GetJsapiTicket(token);
            if (string.IsNullOrWhiteSpace(ticket))
            {
                return null;
            }

            string url = Request["Url"];
            string timestamp = WeixinFunction.GetTimestamp();
            string nonceStr = WeixinFunction.GetNoncestr();

            // 签名参数集合
            SortedDictionary<string, object> sdParams = new SortedDictionary<string, object>();
            sdParams.Add("url", url);
            sdParams.Add("jsapi_ticket", ticket);
            sdParams.Add("nonceStr", nonceStr);
            sdParams.Add("timeStamp", timestamp);
            // 生成微信参数签名
            string wxSign = WeixinFunction.BuildMysign(sdParams).ToUpper();

            return Json(new { appId = CustomConfig.WeixinAppId, timestamp = timestamp, nonceStr = nonceStr, signature = wxSign });
        }

        /// <summary>
        /// 获取最优优惠券
        /// </summary>
        /// <param name="ocpList">订单中的店铺-商品信息</param>
        /// <param name="userId">当前用户Id</param>
        /// <returns></returns>
        public JsonResult GetOptimalCouponAjax(List<ComScoreCheckDTO> ocpList, Guid userId)
        {
            var result = GetOptimalCoupon(ocpList, userId);
            return Json(result);
        }

        /// <summary>
        /// 获取最优优惠券
        /// </summary>
        /// <param name="ocpList">订单中的店铺-商品信息</param>
        /// <param name="userId">当前用户Id</param>
        /// <returns></returns>
        public ReturnInfo<List<CouponNewDTO>> GetOptimalCoupon(List<ComScoreCheckDTO> ocpList, Guid userId)
        {
            ReturnInfo<List<CouponNewDTO>> result = new ReturnInfo<List<CouponNewDTO>>();
            if (ocpList == null)
            {
                result.Code = "-1";
                result.IsSuccess = false;
                result.Message = "参数错误";
                return result;
            }
            IEnumerable<Guid> shopIds = (from ocp in ocpList select ocp.AppId).Distinct();
            //调用优惠券系统接口.
            var couResult = GetUserCouponsByShopList(shopIds.ToList(), userId);
            if (couResult == null)
            {
                result.Code = "-2";
                result.IsSuccess = false;
                result.Message = "接口异常，请稍后重试！";
                return result;
            }
            if (couResult.Code != 0)
            {
                result.Code = couResult.Code.ToString();
                result.Message = couResult.Info;
                result.IsSuccess = couResult.IsSuccess;
                return result;
            }
            IList<CouponNewDTO> listCR = couResult.Data;
            if (listCR == null || listCR.Count == 0)
            {
                result.Code = "-3";
                result.Message = "没有找到优惠券信息！";
                result.IsSuccess = false;
                return result;
            }

            List<CouponNewDTO> listCBest = new List<CouponNewDTO>();
            //以店铺（AppId）为中心进行筛选。(每个店铺只能用一张优惠券)
            foreach (Guid shopId in shopIds)
            {

                //找出当前店铺可用优惠券
                var enumCR = from cr in listCR
                             where cr.ShopId == shopId

                             select cr;

                //当前店铺所有商品
                IEnumerable<ComScoreCheckDTO> ocpInShopList = from ocp in ocpList
                                                              where ocp.AppId == shopId
                                                              select ocp;
                //当前店铺总金额
                decimal totalPrice = (from ocp in ocpInShopList
                                      select ocp.RealPrice * ocp.Num).Sum();

                //找出作用在店铺上的最优优惠券（小于最接近totalPrice的优惠券）。
                var couponForShopList = (from cr in enumCR
                                         where cr.CouponType == CouponType.BeInCommon
                                               && cr.LimitCondition <= totalPrice
                                         orderby cr.Cash descending
                                         select cr).ToList();

                CouponNewDTO couponForShop = null;

                if (couponForShopList != null && couponForShopList.Count > 0)
                {
                    couponForShop = couponForShopList.FirstOrDefault();
                    if (couponForShopList.Count > 1)
                    {
                        var appFirst =
                            couponForShopList.Where(t => t.UseType == Coupon.Deploy.Enum.UseType.App)
                                .OrderByDescending(t => t.Cash)
                                .FirstOrDefault();
                        var jinHerFirst =
                            couponForShopList.Where(t => t.UseType == Coupon.Deploy.Enum.UseType.Jinher)
                                .OrderByDescending(t => t.Cash)
                                .FirstOrDefault();
                        if (appFirst != null && jinHerFirst != null)
                        {
                            if (appFirst.Cash == jinHerFirst.Cash)
                            {
                                couponForShop = appFirst;
                            }
                        }
                    }
                }

                //找出当前店铺作用在商品上的优惠券。
                var enumCRforCommodity = from cr in enumCR
                                         where cr.CouponType == CouponType.SpecifyGoods
                                         select cr;

                //找出作用在商品上的可用优惠券。
                var cExtList = new List<CouponNewDTOExtend>();
                foreach (var crc in enumCRforCommodity)
                {
                    var glist = crc.GoodList;
                    if (glist == null || glist.Count == 0)
                    {
                        continue;
                    }

                    //用户在当前店铺买了可以使用当前优惠券的商品总金额。
                    var tpc =
                        (from a in ocpInShopList where glist.Contains(a.CommodityId) select a.RealPrice * a.Num).Sum();
                    //不满足限制条件，优惠券不可用。
                    if (crc.LimitCondition > tpc)
                    {
                        continue;
                    }
                    //用户在当前店铺买了可以使用当前优惠券的商品。
                    var tpcids = from a in ocpInShopList where glist.Contains(a.CommodityId) select a.CommodityId;
                    if (tpcids == null || (!tpcids.Any()))
                    {
                        continue;
                    }
                    //将可用优惠券保存。
                    var cext = new CouponNewDTOExtend();
                    cext.FillWith(crc);
                    //cext.GoodId = tpcids;
                    cext.GoodList = tpcids.ToList();
                    cExtList.Add(cext);

                }

                //作用在商品上的最优优惠券。
                var cExtest = cExtList.OrderByDescending(cext => cext.Cash).FirstOrDefault();
                if (cExtList.Count > 1)
                {
                    var appFirstCom =
                        cExtList.Where(t => t.UseType == Coupon.Deploy.Enum.UseType.App)
                            .OrderByDescending(t => t.Cash)
                            .FirstOrDefault();
                    var jinHerFirstCom =
                        cExtList.Where(t => t.UseType == Coupon.Deploy.Enum.UseType.Jinher)
                            .OrderByDescending(t => t.Cash)
                            .FirstOrDefault();

                    if (appFirstCom != null && jinHerFirstCom != null)
                    {
                        if (appFirstCom.Cash == jinHerFirstCom.Cash)
                        {
                            cExtest = appFirstCom;
                        }
                    }
                }

                if (couponForShop == null && cExtest == null)
                {
                    continue;
                }
                //没有作用在店铺上的优惠券
                else if (couponForShop == null && cExtest != null)
                {
                    listCBest.Add(cExtest);
                }
                //没有作用在商品上的优惠券
                else if (couponForShop != null && cExtest == null)
                {
                    listCBest.Add(couponForShop);
                }
                //作用在店铺上的优惠券的优惠额度大于作用在商品上的.
                else if (couponForShop.Cash > cExtest.Cash)
                {
                    listCBest.Add(couponForShop);
                }
                else if (couponForShop.Cash == cExtest.Cash)
                {
                    //优惠额度相等，商品上的优先。
                    if (couponForShop.UseType == UseType.App && cExtest.UseType == UseType.Jinher)
                    {
                        listCBest.Add(couponForShop);
                    }
                    else
                    {
                        listCBest.Add(cExtest);
                    }
                }
                else
                {
                    listCBest.Add(cExtest);
                }
            }
            result.Data = listCBest;
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 调用接口获取当前用户在店铺中所有可用优惠券列表。
        /// </summary>
        /// <param name="shopIds">店铺列表</param>
        /// <param name="uid">当前用户</param>
        /// <returns>可用优惠券列表</returns>
        public Jinher.AMP.Coupon.Deploy.CustomDTO.ReturnInfoDTO<IList<CouponNewDTO>> GetUserCouponsByShopList(
            IList<Guid> shopIds, Guid uid)
        {
            try
            {
                ListCouponRequestDTO condition = new ListCouponRequestDTO();
                condition.ShopList = shopIds;
                condition.UserId = uid;
                condition.CouponState = CouponState.Bind;
                var result = Jinher.AMP.BTP.TPS.CouponSV.Instance.GetUserCouponsByShopList(condition);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(
                    string.Format(
                        "MobileController中调用Jinher.AMP.Coupon.ISV.Facade.CouponFacade.GetUserCouponsByShopList接口异常。shopIds：{0}，uid：{1}",
                        JsonHelper.JsonSerializer(shopIds), uid), ex);
            }
            return null;
        }

        /// <summary>
        /// 获取用户信息。
        /// </summary>
        /// <returns></returns>
        public ActionResult GetUserNameAndNickname(Guid uid)
        {
            string mn = this.GetType() + ".GetUserNameAndNickname";
            var jsonr = UserModel.GetUserNameAndCode(uid, mn);
            return Json(jsonr, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 校验app是否正品会app
        /// </summary>
        /// <returns></returns>
        public ActionResult CheckIsAppSet(Guid appId)
        {
            var jsonr = ZPHSV.Instance.CheckIsAppInZPH(appId);
            return Json(jsonr, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SelfTakeStationList()
        {
            return View();
        }

        /// <summary>
        /// 获取某个区域的自提点
        /// </summary>
        /// <param name="areaCode">区域（城市）编码</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns></returns>
        public ActionResult GetSelfTakeStationList(Guid appId, string searchContent, int pageIndex, int pageSize)
        {


            string methodInfo = this.GetType() + ".GetSelfTakeStationList";

            int rowCount = 0;

            SelfTakeStationSearchDTO stsSearch = new SelfTakeStationSearchDTO();
            stsSearch.appId = appId;
            stsSearch.pageIndex = pageIndex;
            stsSearch.pageSize = pageSize;
            stsSearch.searchContent = searchContent;
            //stsSearch.rowCount = 0;

            var result = SelfTakeStationVM.GetSelfTakeStation(stsSearch, out rowCount, methodInfo);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //自提订单列表页。
        public ActionResult SelfTakeOrderList()
        {
            Guid userId = Guid.Empty;
            Guid.TryParse(Request["userId"], out userId);
            return View();
        }

        //获取自提订单列表
        public ActionResult GetSelfTakeOrderList(Guid userId, int pageIndex, int pageSize)
        {
            try
            {
                //管理员用户Id --> 自提点 --> 自提点的订单。 
                Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade coFacade =
                    new Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade();
                List<OrderListCDTO> orderList = coFacade.GetOrderListByManagerId(userId, pageIndex, pageSize);
                return Json(orderList, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                LogHelper.Error("MobileController.GetSelfTakeOrderList异常", ex);
            }
            return Json(null, JsonRequestBehavior.AllowGet);

        }

        //获取自提订单售后列表
        public ActionResult GetSelfTakeOrderListAfterSales(Guid userId, int pageIndex, int pageSize, string state)
        {
            try
            {
                //管理员用户Id --> 自提点 --> 自提点的订单。 
                Jinher.AMP.BTP.ISV.Facade.CommodityOrderAfterSalesFacade coFacade =
                    new Jinher.AMP.BTP.ISV.Facade.CommodityOrderAfterSalesFacade();

                List<OrderListCDTO> orderList = coFacade.GetSelfTakeOrderListAfterSales(userId, pageIndex, pageSize,
                    state);
                return Json(orderList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Error("MobileController.GetSelfTakeOrderListAfterSales异常", ex);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 自提订单商品清单。
        /// </summary>
        /// <returns></returns>
        public ActionResult SelfTakeOrderCommodityList()
        {
            ResultDTO<CommodityOrderSDTO> result = null;
            try
            {
                Guid userId = Guid.Empty;
                Guid.TryParse(Request.QueryString["userId"], out userId);
                string pickUpCode = Request.QueryString["pickUpCode"];

                Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade coFacade =
                    new Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade();
                result = coFacade.GetOrderItemsByPickUpCode(userId, pickUpCode) as ResultDTO<CommodityOrderSDTO>;

                ////=======================测试使用==============================
                //Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade orderfacade = new ISV.Facade.CommodityOrderFacade();
                //CommodityOrderSDTO order = orderfacade.GetOrderItems(new Guid("1c19db92-20be-476b-b079-378674ffab4f"), userId, Guid.Empty);
                //result = new ResultDTO<CommodityOrderSDTO>();
                //result.Data = order;
                ////=============================================================

            }
            catch (Exception ex)
            {
                LogHelper.Error("MobileController.SelfTakeOrderCommodityList异常", ex);
            }
            if (result == null)
            {
                result = new ResultDTO<CommodityOrderSDTO>();
                result.Data = new CommodityOrderSDTO();
            }
            return View(result);
        }


        /// <summary>
        /// createOrder页面加载时需要加载的信息。
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCreateOrderAllInfo(CreateOrderActionParam ap)
        {
            using (StopwatchLogHelper.BeginScope("Mobile.GetCreateOrderAllInfo"))
            {
                CreateOrderPageLoadAction at = new CreateOrderPageLoadAction();

                if (ap.userId == Guid.Empty)
                {
                    at.ResultCode = -1;
                    at.Message = "出错了";
                    return Json(at, JsonRequestBehavior.AllowGet);
                }

                //是否直接到账
                int tradeType = Jinher.AMP.BTP.TPS.FSPSV.GetTradeSettingInfo(ap.esAppId);
                at.TradeType = tradeType;
                if (tradeType == -1)
                {
                    return Json(at, JsonRequestBehavior.AllowGet);
                }

                List<Tuple<Action<object>, object>> actionList = new List<Tuple<Action<object>, object>>();

                //tradeType:0:担保交易;1：非担保交易（直接到账）
                if (tradeType == 0)
                {

                    var balanceAction = new Tuple<Action<object>, object>(at.GetBalance, ap);
                    actionList.Add(balanceAction);

                    bool isAllAppInZPH = ZPHSV.CheckAllAppInZPH(ap.appIds);
                    at.IsAllAppInZPH = isAllAppInZPH;
                    if (isAllAppInZPH)
                    {
                        var couponAction = new Tuple<Action<object>, object>(at.GetGoldCouponCount, ap);
                        actionList.Add(couponAction);
                    }
                }

                var addressAction = new Tuple<Action<object>, object>(at.GetDeliveryAddressDefault, ap);
                actionList.Add(addressAction);

                // 易捷北京没有众筹?
                if (ap.coType != "gouwuche")
                {
                    var crowdfundingAction = new Tuple<Action<object>, object>(at.GetUserCrowdfundingBuy, ap);
                    actionList.Add(crowdfundingAction);
                }
                if (ap.appSelfTakeWay == 2 || ap.appSelfTakeWay == 3)
                {
                    var appselftakeAction = new Tuple<Action<object>, object>(at.GetAppSelfTakeStationDefault, ap);
                    actionList.Add(appselftakeAction);
                }

                //var usAction = new Tuple<Action<object>, object>(at.GetUserScore, ap);
                //actionList.Add(usAction);

                var cod = new Tuple<Action<object>, object>(at.GetIsAllAppSupportCOD, ap);
                actionList.Add(cod);

                var duty = new Tuple<Action<object>, object>(at.GetAllDuty, ap);
                actionList.Add(duty);

                // YJB 在CalFreightMultiApps中查询
                //var yjb = new Tuple<Action<object>, object>(at.GetAppYJB, ap);
                //actionList.Add(yjb);

                var youka = new Tuple<Action<object>, object>(at.GetAppYouKa, ap);
                actionList.Add(youka);

                var setMeal = new Tuple<Action<object>, object>(at.GetAppSetMeal, ap);
                actionList.Add(setMeal);

                var jcActivityYouKa = new Tuple<Action<object>, object>(at.GetAppJcActivity, ap);
                actionList.Add(jcActivityYouKa);

                //获取是否展示发票信息
                bool isInvoice = true;
                string[] appIdArr = ap.appIds.Split(";；,，".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                List<Guid> appIdList = new List<Guid>();
                if (appIdArr.Any())
                {
                    appIdList = appIdArr.ToList().ConvertAll(aid => new Guid(aid));
                }
                InvoiceSearchDTO invSearchDTO = new InvoiceSearchDTO();
                invSearchDTO.AppIds = appIdList;
                invSearchDTO.UserId = this.ContextDTO.LoginUserID;
                InvoiceFacade invFacade = new InvoiceFacade();
                ResultDTO<InvoiceSettingDTO> invResult = invFacade.GetInvoiceSetting(invSearchDTO);
                if (invResult.ResultCode == 0 && invResult.Data != null)
                {
                    if (!invResult.Data.IsElectronicInvoice && !invResult.Data.IsOrdinaryInvoice && !invResult.Data.IsVATInvoice)
                    {
                        isInvoice = false;
                    }
                }
                at.IsInvoice = isInvoice;


                #region 易捷卡
                ////获取可用易捷卡数量
                //var YJCardNum = new Tuple<Action<object>, object>(at.GetYjcNum, null);
                //actionList.Add(YJCardNum);
                ////获取易捷卡信息
                //if (!string.IsNullOrWhiteSpace(ap.YjcIds))
                //{
                //    var GetYjcInfos = new Tuple<Action<object>, object>(at.GetYjcInfos, ap);
                //    actionList.Add(GetYjcInfos);
                //}
                #endregion
                MultiWorkTask mwTask = new MultiWorkTask();
                mwTask.Start(actionList);

                return Json(at, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// 获取默认的自提点信息。
        /// </summary>
        /// <param name="areaCode">区域编码</param>
        /// <returns></returns>
        public ActionResult GetSelfTakeStationDefault(string areaCode)
        {
            string methodInfo = this.GetType() + ".GetSelfTakeStationDefault";

            int rowCount = 0;
            SelfTakeStationSearchResultDTO selfTakeStation = null;

            SelfTakeStationSearchDTO stsSearch = new SelfTakeStationSearchDTO();
            stsSearch.Code = areaCode;
            stsSearch.pageIndex = 1;
            stsSearch.pageSize = 10;
            stsSearch.rowCount = 10;

            var result = SelfTakeStationVM.GetSelfTakeStation(stsSearch, out rowCount, methodInfo);
            if (result != null && result.Count > 0)
            {
                selfTakeStation = result[0];
            }
            return Json(selfTakeStation, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 运营工具中订单详情
        /// </summary>
        /// <returns></returns>
        public ActionResult SelfTakeOrderDetail()
        {
            Guid orderId = Guid.Empty;
            Guid.TryParse(Request["orderId"], out orderId);

            Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade facade = new ISV.Facade.CommodityOrderFacade();
            var result = facade.GetOrderItems(orderId, Guid.Empty, Guid.Empty);
            Guid realAppId = Guid.Empty;
            if (result != null && result.AppId != Guid.Empty)
                realAppId = result.AppId;

            //对没有收货人或收货人电话的，需从用户信息中取出
            if (string.IsNullOrWhiteSpace(result.ReceiptUserName) || string.IsNullOrWhiteSpace(result.ReceiptPhone))
            {
                var invoker = this.GetType() + ".GetOrderDetails";
                var jsonr = UserModel.GetUserNameAndCode(result.UserId, invoker);
                if (string.IsNullOrWhiteSpace(result.ReceiptUserName))
                {
                    result.ReceiptUserName = jsonr.Item1;
                }
                if (string.IsNullOrWhiteSpace(result.ReceiptPhone))
                {
                    result.ReceiptPhone = jsonr.Item2;
                }
            }
            string appName = APPSV.GetAppName(realAppId);
            ViewBag.AppName = appName;
            return View(result);
        }

        /// <summary>
        /// 获取应用分享信息
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public ActionResult GetAppShareContent(Guid appId)
        {
            var result = APPSV.Instance.GetAppShareContent(appId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 售后保存用户退款退货退单等操作
        /// </summary>
        /// <param name="type">保存的方式： 1 多文本框内容；2 退款单选内容；3 可选择的单选内容；4 退款方式</param>
        /// <param name="RefundExpCo">type 为4时，传入选择或者输入的快递名称，其他状态可不传</param>
        /// <param name="RefundExpOrderNo">type 为4时，传入输入的快递单号，其他状态可不传</param>
        /// <param name="appId">appid</param>
        /// <param name="state">订单当前状态</param>
        /// <param name="orderId">订单编号</param>
        /// <param name="pic">type为 2或者3时 传入上传的凭证，暂时还没做，其他可不传</param>
        /// <param name="money">type为 2或者3时 传入退款退货的金额，其他可不传</param>
        /// <param name="dec">详细描述</param>
        /// <param name="refundReason">type为 2或者3时 传入选择原因</param>
        /// <param name="userId">用户id</param>
        /// <param name="pay">支付类型</param>
        /// <param name="refundType">1 :仅退款； 2 退款退货 (实际项目里面的逻辑是，0 仅退款；1 退款退货；代码里已经做了处理)</param>
        /// <returns></returns>
        public ActionResult SaveRefundOrderAfterSales(string type, string RefundExpCo = "", string RefundExpOrderNo = "",
            string appId = "", string state = "", string orderId = "",
            string pic = "", string money = "", string dec = "",
            string refundReason = "", string userId = "", string pay = "",
            string refundType = "", string orderItemId = "")
        {
            BTP.ISV.Facade.CommodityOrderAfterSalesFacade orderSV = new BTP.ISV.Facade.CommodityOrderAfterSalesFacade();


            ResultDTO result = new ResultDTO();

            SubmitOrderRefundDTO modelParam = new SubmitOrderRefundDTO();
            modelParam.commodityorderId = Guid.Parse(orderId);
            modelParam.Id = Guid.Parse(orderId);
            modelParam.RefundDesc = dec;
            modelParam.RefundExpCo = RefundExpCo;
            modelParam.RefundExpOrderNo = RefundExpOrderNo;
            modelParam.RefundMoney = decimal.Parse(money);
            modelParam.State = int.Parse(state);
            modelParam.RefundReason = refundReason;

            modelParam.RefundType = refundType == "1" ? 0 : 1;
            modelParam.OrderRefundImgs = pic;
            modelParam.OrderItemId = Guid.Parse(orderItemId);

            result = orderSV.SubmitOrderRefundAfterSales(modelParam);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 京东售后保存用户退款退货退单等操作
        /// </summary>
        public ActionResult SaveRefundJdOrderAfterSales(AddressInfo address, string type, string RefundExpCo = "", string RefundExpOrderNo = "",
            string appId = "", string state = "", string orderId = "",
            string pic = "", string money = "", string dec = "",
            string refundReason = "", string userId = "", string pay = "",
            string refundType = "", string orderItemId = "")
        {
            BTP.ISV.Facade.CommodityOrderAfterSalesFacade orderSV = new BTP.ISV.Facade.CommodityOrderAfterSalesFacade();


            ResultDTO result = new ResultDTO();

            SubmitOrderRefundDTO modelParam = new SubmitOrderRefundDTO();
            modelParam.commodityorderId = Guid.Parse(orderId);
            modelParam.Id = Guid.Parse(orderId);
            modelParam.RefundDesc = dec;
            modelParam.RefundExpCo = RefundExpCo;
            modelParam.RefundExpOrderNo = RefundExpOrderNo;
            modelParam.RefundMoney = decimal.Parse(money);
            modelParam.State = int.Parse(state);
            modelParam.RefundReason = refundReason;

            modelParam.RefundType = refundType == "1" ? 0 : 1;
            modelParam.OrderRefundImgs = pic;
            modelParam.OrderItemId = Guid.Parse(orderItemId);
            //增加字段
            modelParam.IsJDOrder = true;
            modelParam.Address = address;
            result = orderSV.SubmitOrderRefundAfterSales(modelParam);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 进销存订单申请单品售后
        /// </summary>
        /// <param name="address"></param>
        /// <param name="type"></param>
        /// <param name="RefundExpCo"></param>
        /// <param name="RefundExpOrderNo"></param>
        /// <param name="appId"></param>
        /// <param name="state"></param>
        /// <param name="orderId"></param>
        /// <param name="pic"></param>
        /// <param name="money"></param>
        /// <param name="dec"></param>
        /// <param name="refundReason"></param>
        /// <param name="userId"></param>
        /// <param name="pay"></param>
        /// <param name="refundType"></param>
        /// <param name="orderItemId"></param>
        /// <returns></returns>
        public ActionResult SaveJdeclpRefundOrderAfterSales(AddressInfo address, string type, string RefundExpCo = "", string RefundExpOrderNo = "",
            string appId = "", string state = "", string orderId = "",
            string pic = "", string money = "", string dec = "",
            string refundReason = "", string userId = "", string pay = "",
            string refundType = "", string orderItemId = "", string pickwareType = "")
        {
            BTP.ISV.Facade.CommodityOrderAfterSalesFacade orderSV = new BTP.ISV.Facade.CommodityOrderAfterSalesFacade();

            ResultDTO result = new ResultDTO();

            SubmitOrderRefundDTO modelParam = new SubmitOrderRefundDTO();
            modelParam.commodityorderId = Guid.Parse(orderId);
            modelParam.Id = Guid.Parse(orderId);
            modelParam.RefundDesc = dec;
            modelParam.RefundExpCo = RefundExpCo;
            modelParam.RefundExpOrderNo = RefundExpOrderNo;
            modelParam.RefundMoney = decimal.Parse(money);
            modelParam.State = int.Parse(state);
            modelParam.RefundReason = refundReason;

            modelParam.RefundType = refundType == "1" ? 0 : 1;
            modelParam.OrderRefundImgs = pic;
            modelParam.OrderItemId = Guid.Parse(orderItemId);

            modelParam.IsJdEclpOrder = true;
            if (modelParam.RefundType == 1)
            {
                modelParam.PickwareType = pickwareType == "1" ? 1 : 2;
                modelParam.Address = address;
            }

            result = orderSV.SubmitOrderRefundAfterSales(modelParam);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 售后取消退款
        /// </summary>
        /// <param name="orderId">订单id</param>
        /// <param name="state">状态</param>
        /// <returns></returns>
        public ActionResult CancelOrderRefundAfterSales(Guid orderId, int state, Guid orderItemId)
        {
            BTP.ISV.Facade.CommodityOrderAfterSalesFacade orderSV = new BTP.ISV.Facade.CommodityOrderAfterSalesFacade();
            CancelOrderRefundDTO model = new CancelOrderRefundDTO();
            model.CommodityOrderId = orderId;
            model.State = state.ToString();
            model.OrderItemId = orderItemId;
            ResultDTO result = orderSV.CancelOrderRefundAfterSales(model);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 售后退款/退货申请详情
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="userId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public ActionResult GetOrderRefundAfterSales(Guid orderId, Guid userId, string sessionId, Guid orderItemId)
        {
            BTP.ISV.Facade.CommodityOrderAfterSalesFacade orderSV = new BTP.ISV.Facade.CommodityOrderAfterSalesFacade();
            try
            {
                var orderRefund = orderSV.GetOrderRefundAfterSales(orderId, orderItemId);
                orderRefund.IsJdEclpOrder = new IBP.Facade.JdEclpOrderFacade().ISEclpOrder(orderId);
                JdOrderHelper.GetJdRefundInfo(orderRefund);

                SNOrderAfterSalesHelper.GetSNRefundInfo(orderRefund, orderId, orderItemId);
                return Json(orderRefund, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new SubmitOrderRefundDTO(), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 售后退货方式
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="RefundExpCo"></param>
        /// <param name="RefundExpOrderNo"></param>
        /// <param name="userId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public ActionResult SaveRefundTypeAfterSales(Guid orderId, string RefundExpCo, string RefundExpOrderNo, Guid userId, string sessionId, Guid orderItemId)
        {
            BTP.ISV.Facade.CommodityOrderAfterSalesFacade orderSV = new BTP.ISV.Facade.CommodityOrderAfterSalesFacade();
            try
            {
                AddOrderRefundExpDTO model = new AddOrderRefundExpDTO();
                model.CommodityOrderId = orderId;
                model.RefundExpCo = RefundExpCo;
                model.RefundExpOrderNo = RefundExpOrderNo;
                model.OrderItemId = orderItemId;
                var orderRefund = orderSV.AddOrderRefundExpAfterSales(model);
                return Json(orderRefund, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new ResultDTO { ResultCode = 1, Message = "保存退货方式接口异常" }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// 售后订单列表-运营工具使用
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl]
        public ActionResult AfterSaleOrderList()
        {
            return View();
        }

        /// <summary>
        /// 退款详情-运营工具使用
        /// </summary>
        /// <returns></returns>
        public ActionResult RefundInfoSeller()
        {
            return View();
        }

        /// <summary>
        /// 拒绝退款原因-运营工具使用
        /// </summary>
        /// <returns></returns>
        public ActionResult RefuseRefundReasonSeller()
        {
            return View();
        }

        #region 售后

        /// <summary>
        /// 同意退款/退款申请
        /// </summary>
        /// <param name="commodityOrderId"></param>
        /// <param name="state"></param>
        /// <param name="message"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult UpdateCommodityOrderAfterSales(Guid commodityOrderId, int state, string message, Guid userId)
        {
            Jinher.AMP.BTP.ISV.Facade.CommodityOrderAfterSalesFacade cf =
                new Jinher.AMP.BTP.ISV.Facade.CommodityOrderAfterSalesFacade();
            CancelTheOrderDTO model = new CancelTheOrderDTO();
            model.OrderId = commodityOrderId;
            model.State = state;
            model.Message = message;
            model.UserId = userId;
            ResultDTO result = cf.CancelTheOrderAfterSales(model);
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "售后订单修改成功" });
            }
            return Json(new { Result = false, Messages = result.Message });
        }

        /// <summary>
        /// 拒绝退款/退货申请 (运营工具中保存拒绝原因)
        /// </summary>
        /// <param name="commodityOrderId"></param>
        /// <param name="state"></param>
        /// <param name="message"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult RefuseRefundOrderAfterSales(Guid commodityOrderId, int state, string message, Guid userId,
            string refuseReason = "")
        {
            Jinher.AMP.BTP.ISV.Facade.CommodityOrderAfterSalesFacade cf =
                new Jinher.AMP.BTP.ISV.Facade.CommodityOrderAfterSalesFacade();
            CancelTheOrderDTO model = new CancelTheOrderDTO();
            model.OrderId = commodityOrderId;
            model.State = state;
            model.Message = message;
            model.UserId = userId;
            model.RefuseReason = refuseReason;
            ResultDTO result = cf.RefuseRefundOrderAfterSales(model);
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "拒绝申请成功" });
            }
            return Json(new { Result = false, Messages = result.Message });
        }

        #endregion

        /// <summary>
        /// 获取商品属性。
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCommodityAttrStocks(Guid commodityId)
        {
            CommoditySearchDTO csDto = new CommoditySearchDTO();
            csDto.CommodityId = commodityId;

            Jinher.AMP.BTP.ISV.Facade.CommodityFacade cf = new Jinher.AMP.BTP.ISV.Facade.CommodityFacade();
            var result = cf.CommodityAttrStocks(csDto);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 点餐商品列表
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl(UrlNeedAppParams = UrlNeedAppParamsEnum.ShopId)]
        public ActionResult CateringCommodityList()
        {
            return View("~/Views/MobileFitted/CateringCommodityList.cshtml");
        }

        /// <summary>
        /// 获取菜单下商品。
        /// </summary>
        /// <param name="appId">店铺appid</param>
        /// <returns></returns>
        public JsonResult GetCateringCommodity(Guid appId, Guid userId)
        {
            CateringCommodityDTO dto = new CateringCommodityDTO();
            //店铺中所有商品。
            Jinher.AMP.BTP.ISV.Facade.CommodityFacade commodityFacade = new ISV.Facade.CommodityFacade();
            var result =
                commodityFacade.GetCateringCommodity(new CommodityListSearchDTO()
                {
                    AppId = appId,
                    UserId = userId,
                    PageIndex = 1,
                    PageSize = int.MaxValue
                });
            if (result != null && result.ResultCode == 0 && result.Data != null)
            {
                dto = result.Data;
            }
            return Json(dto);
        }

        /// <summary>
        /// 金和APP平台租用协议（金和电商APP）
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl]
        public ActionResult JhRentAgreement()
        {
            ViewBag.Title = Request["title"];
            return View();
        }

        /// <summary>
        /// 金和APP平台租用协议（金和营销APP）
        /// </summary>
        /// <returns></returns>
        public ActionResult JhRentSemAgreement()
        {
            return View();
        }

        /// <summary>
        /// 获取联系地址
        /// </summary>
        /// <param name="esAppId"></param>
        /// <returns></returns>
        public ActionResult GetZPHContractUrl(Guid esAppId)
        {
            string contactUrl = string.Empty;
            string appName = string.Empty;
            if (esAppId == Guid.Empty)
            {
                return Json(new { Code = 1, Messages = "传入参数为空" });
            }

            var result = ZPHSV.Instance.GetMaskPicII(esAppId);
            var resultApp = APPSV.GetAppName(esAppId);
            if (result != null && resultApp != null)
            {
                contactUrl = result.ContactUrl;
                var contactObj = result.contactObj;
                appName = resultApp;
                return
                    Json(
                        new
                        {
                            Code = 0,
                            Messages = "Success",
                            Data = contactUrl,
                            DataApp = appName,
                            ContactObj = contactObj
                        });
            }
            return Json(new { Code = 2, Messages = "未获取到结果" });
        }

        /// <summary>
        /// 众销佣金（已入账与未入账）
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl]
        public ActionResult CommissionMoney()
        {
            var url = Request.Url.ToString();
            url = url.TrimEnd('/').ToLower();
            string param = url.Substring(url.IndexOf("?"));

            Guid appId = new Guid(Request["appId"]);
            Guid userId = this.ContextDTO.LoginUserID;
            int? payeeType = null;
            if (Request["payeeType"] != null)
            {
                payeeType = int.Parse(Request["payeeType"]);
            }

            if (!payeeType.HasValue || payeeType.Value == 3)
            {
                //0：没有启用分成推广功能；1：启用
                int shareType = 0;

                //校验app是否启用分成推广功能
                if (BACBP.CheckSharePromotion(appId))
                {
                    Jinher.AMP.BTP.ISV.Facade.ShareOrderFacade facade = new ISV.Facade.ShareOrderFacade();
                    Jinher.AMP.BTP.Deploy.CustomDTO.ShareOrderMoneySumSearchDTO search =
                        new Jinher.AMP.BTP.Deploy.CustomDTO.ShareOrderMoneySumSearchDTO();
                    search.UseId = userId;
                    search.AppId = appId;
                    search.PayeeType = 3;

                    var shareInfo = facade.GetShareOrderMoneySumInfo(search);
                    if (shareInfo == null)
                    {
                        shareType = 2;
                        ViewBag.ShareInfo = null;
                    }
                    else
                    {
                        shareType = 1;
                        ViewBag.ShareInfo = shareInfo;
                    }
                }
                else
                {
                    shareType = 0;
                }
                ViewBag.ShareType = shareType;
                if (shareType == 0)
                {
                    ViewBag.Title = "众销收入";
                    ViewBag.Message = "抱歉，暂不支持该功能";
                    ;
                    return View("~/Views/Mobile/MobileError.cshtml");
                }
                ViewBag.Title = "众销收入";

            }
            else if (payeeType.Value == 12)
            {
                //0：没有启用渠道推广功能；1：启用
                int shareType = 0;

                //校验app是否启用渠道推广功能
                if (BACBP.CheckChannel(appId))
                {
                    Jinher.AMP.BTP.ISV.Facade.ShareOrderFacade facade = new ISV.Facade.ShareOrderFacade();
                    Jinher.AMP.BTP.Deploy.CustomDTO.ShareOrderMoneySumSearchDTO search =
                        new Jinher.AMP.BTP.Deploy.CustomDTO.ShareOrderMoneySumSearchDTO();
                    search.UseId = userId;
                    search.AppId = appId;
                    search.PayeeType = 12;

                    var shareInfo = facade.GetShareOrderMoneySumInfo(search);
                    if (shareInfo == null)
                    {
                        shareType = 2;
                        ViewBag.ShareInfo = null;
                    }
                    else
                    {
                        shareType = 1;
                        ViewBag.ShareInfo = shareInfo;
                    }
                }
                else
                {
                    shareType = 0;
                }
                ViewBag.ShareType = shareType;
                if (shareType == 0)
                {
                    ViewBag.Title = "渠道佣金";
                    ViewBag.Message = "抱歉，暂不支持该功能";
                    ;
                    return View("~/Views/Mobile/MobileError.cshtml");
                }
                ViewBag.Title = "渠道佣金";
            }
            return View();
        }

        /// <summary>
        /// 获取佣金
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ActionResult GetShareOrderMoneyInfo(Jinher.AMP.BTP.Deploy.CustomDTO.ShareOrderMoneySearchDTO search)
        {
            Guid userId = this.ContextDTO.LoginUserID;
            search.UseId = userId;
            Jinher.AMP.BTP.ISV.Facade.ShareOrderFacade facade = new ISV.Facade.ShareOrderFacade();
            var result = facade.GetShareOrderMoneyInfo(search);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 拼团详情
        /// </summary>
        /// <param name="diyGroupId"></param>
        /// <param name="appId"></param>
        /// <param name="shareId"></param>
        /// <returns></returns>
        [DealMobileUrl]
        public ActionResult DiyGroupDetail(Guid diyGroupId, Guid appId, string shareId)
        {
            var esAppId = appId;
            if (!Request.Url.ToString().ToLower().Contains("/mobile/"))
            {
                var redirectUrl = string.Format("/Mobile/DiyGroupDetail?diyGroupId={0}&appId={1}", diyGroupId, esAppId);
                if (!string.IsNullOrWhiteSpace(shareId))
                {
                    redirectUrl += "&shareId=" + shareId;
                }
                return Redirect(redirectUrl);
            }
            return View();
        }

        /// <summary>
        /// 获取拼团详情数据
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ActionResult GetDiyGroupDetail(Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupDetailSearchDTO search)
        {
            Jinher.AMP.BTP.ISV.Facade.DiyGroupFacade facade = new ISV.Facade.DiyGroupFacade();
            var result = facade.GetDiyGroupDetail(search);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 活动规则
        /// </summary>
        /// <returns></returns>
        public ActionResult DiyGroupActivityRule()
        {
            return View();
        }

        /// <summary>
        /// 拼团下订单页
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateOrderDiyGroup()
        {
            ViewBag.FSPUrl = CustomConfig.FSPUrl;
            ViewBag.PromotionUrl = CustomConfig.PromotionUrl;
            ViewBag.PortalUrl = CustomConfig.PortalUrl;
            ViewBag.UploadFileCommodityList = CustomConfig.UploadFileCommodityList;
            ViewBag.Contract1CommodityList = CustomConfig.Contract1CommodityList;
            ViewBag.Contract2CommodityList = CustomConfig.Contract2CommodityList;
            return View();
        }

        /// <summary>
        /// 获取会员信息
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public ActionResult GetVipInfo(Guid appId)
        {
            Guid userId = this.ContextDTO.LoginUserID;
            if (userId == Guid.Empty)
            {
                return Json(new { ResultCode = 1, Message = "没有登录" }, JsonRequestBehavior.AllowGet);
            }
            Jinher.AMP.BTP.ISV.Facade.BTPUserFacade facade = new ISV.Facade.BTPUserFacade();
            var result = facade.GetVipInfo(userId, appId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取商品属性。
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCommodityAttribute(Guid commodityId, Guid userId)
        {
            Jinher.AMP.BTP.ISV.Facade.CommodityFacade cf = new Jinher.AMP.BTP.ISV.Facade.CommodityFacade();
            var result = cf.GetCommodityAttribute(commodityId, userId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 商品属性选择测试
        /// </summary>
        /// <returns></returns>
        public ActionResult MasTest()
        {
            return View("~/Views/MobileFitted/MasTest.cshtml");
        }

        [DealMobileUrl(IsWxAutoLogin = false)]
        public ActionResult MultiAttributeSelector()
        {
            return View("~/Views/MobileFitted/MultiAttributeSelector.cshtml");
        }

        /// <summary>
        /// 发票信息页。
        /// </summary>
        /// <param name="appIds">以,分隔的订单商品所在店铺的id</param>
        /// <returns></returns>
        [DealMobileUrl]
        public ActionResult InvoiceInfo(string appIds)
        {
            string[] appIdArr = appIds.Split(";；,，".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            List<Guid> appIdList = new List<Guid>();
            if (appIdArr.Any())
            {
                appIdList = appIdArr.ToList().ConvertAll(aid => new Guid(aid));
            }
            InvoiceSearchDTO invSearchDTO = new InvoiceSearchDTO();
            invSearchDTO.AppIds = appIdList;
            invSearchDTO.UserId = this.ContextDTO.LoginUserID;
            InvoiceFacade invFacade = new InvoiceFacade();
            ResultDTO<InvoiceSettingDTO> invResult = invFacade.GetInvoiceSetting(invSearchDTO);
            if (invResult.ResultCode == 0 && invResult.Data != null)
            {
                ViewBag.InvoiceSetting = invResult.Data;
            }
            else
            {
                ViewBag.InvoiceSetting = new InvoiceSettingDTO();
            }
            ViewBag.appId = Request["appId"].ToString();
            OrderFieldFacade ordersetfacde = new OrderFieldFacade();
            ViewBag.OrderSet = ordersetfacde.GetOrderSet(Guid.Parse(Request["appId"]));
            return View();
        }

        /// <summary>
        /// 发票信息页。 历史发票数据
        /// </summary>
        /// <returns></returns>
        public ActionResult InvoiceInfoList()
        {
            return View();
        }

        /// <summary>
        /// 获取发票历史数据
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <param name="category">发票类型 1:增值税专用发票,2:电子发票,4:增值税专用发票</param>
        /// <returns></returns>
        public ActionResult GetInvoiceInfoList(Guid appId, Guid userId, int category)
        {
            InvoiceFacade invFacade = new InvoiceFacade();
            ResultDTO<List<InvoiceInfoDTO>> invResult = invFacade.GetInvoiceInfoList(appId, userId, category);

            return Json(invResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取定制应用底部菜单
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult GetAppFittedBottomMenus(Guid appId, Guid userId)
        {
            var result = Jinher.AMP.BTP.TPS.BACSV.GetAppBottomMenus(appId, userId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 登陆页（为zph提供）
        /// </summary>
        /// <returns></returns>
        public ActionResult LoginCenter()
        {
            var callBackUrl = Request["backUrl"];
            if (Request.Cookies["CookieContextDTO"] != null && !string.IsNullOrEmpty(callBackUrl))
            {
                return Redirect(callBackUrl);
            }
            return View();
        }

        /// <summary>
        /// 生成在线支付的Url地址
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public ActionResult GetPayUrl(Jinher.AMP.BTP.Deploy.CustomDTO.PayOrderToFspDTO payOrderToFspDto)
        {
            CommodityOrderFacade facade = new CommodityOrderFacade();
            if (payOrderToFspDto != null)
                payOrderToFspDto.Scheme = Request.Url.Scheme;
            var result = facade.GetPayUrl(payOrderToFspDto);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 手机端公用错误页
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="message">内容</param>
        /// <returns></returns>
        public ActionResult MobileError(string title, string message)
        {
            ViewBag.Title = title;
            ViewBag.Message = message;
            return View();
        }

        public ActionResult GetProviceCityJsonData(Guid appId)
        {
            List<ProviceCityModel> data = new List<ProviceCityModel>();
            var result = SNSSV.Instance.GetAllDistrict(appId);
            if (result != null && result.IsSuccess)
            {
                var contentList = result.Content;
                if (contentList != null && contentList.Count > 0)
                {
                    foreach (var content in contentList)
                    {
                        ProviceCityModel model = new ProviceCityModel();
                        model.A = content.Code;
                        model.N = content.Name;
                        data.Add(model);
                    }
                }
                data = data.OrderBy(t => t.L).ThenBy(t => t.N).ToList();
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 检查App自提点设置
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public ActionResult CheckAppSelfTakeWay(Guid appId)
        {
            var result = 1;
            var resultBAC = BACBP.CheckAppSelfTake(appId);
            if (resultBAC)
            {
                var resultZPH = ZPHSV.Instance.GetAppSelfTakeWay(appId);
                result = resultZPH;
            }
            return Json(new { Code = 0, Data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAppSelfTakeStationDefault(
            Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationSearchDTO search)
        {
            AppSelfTakeStationFacade facade = new AppSelfTakeStationFacade();
            var result = facade.GetAppSelfTakeStationDefault(search);
            return Json(new { Code = 0, Data = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult UpdateZSH(string ExpOrderNo)
        {
            ResultDTO result = null;
            try
            {
                Guid AppId = Guid.Empty;
                Jinher.AMP.BTP.IBP.Facade.CommodityOrderFacade commodityOrderFacade = new Jinher.AMP.BTP.IBP.Facade.CommodityOrderFacade();
                OrderExpressRouteFacade orderExpressRoutefacade = new OrderExpressRouteFacade();
                ExpressTraceFacade expressTraceFacade = new ExpressTraceFacade();
                var orderExpressRoute = orderExpressRoutefacade.GetExpressRouteByExpOrderNo(ExpOrderNo);
                orderExpressRoute.State = 2;
                var CommodityOrder = commodityOrderFacade.GetCommodityOrderbyExpOrderNo(ExpOrderNo);
                if (CommodityOrder != null)
                {
                    AppId = CommodityOrder.AppId;
                }
                result = ZshwlBP.GetWuliu(orderExpressRoute, AppId);
            }
            catch (Exception ex)
            {
                result = new ResultDTO() { ResultCode = 1, Message = ex.Message, isSuccess = false };
                LogHelper.Debug(string.Format("物流异常信息:{0}", ex.Message));
            }

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 订单详情视图
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl]
        [WeixinOAuthOpenId]
        public ActionResult MyYJBDetails(Guid appId)
        {
            if (appId == Guid.Empty || appId != YJB.Deploy.CustomDTO.YJBConsts.YJAppId)
            {
                return View("MyYJBEmptyDetails");
            }
            return View();
            //Guid userId = getLoginUserId();
            //var result = YJBSV.GetUserYJB(userId);
            //if (result.IsSuccess)
            //{
            //    ViewBag.YJBInfo = result.Data;
            //    return View();
            //}
            //ViewBag.Message = result.Message;
            //return View("MobileError");
        }

        [HttpGet]
        public ActionResult GetMyYJB()
        {
            Guid userId = getLoginUserId();
            if (userId == Guid.Empty)
                return Json(new YJB.Deploy.CustomDTO.ResultDTO<YJB.Deploy.CustomDTO.UserYJBDTO> { Message = "用户不存在。" }, JsonRequestBehavior.AllowGet);
            var result = YJBSV.GetUserYJB(userId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 获取购物车数量
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="esAppId">电商馆id</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetShoppingCartNum(Guid userId, Guid esAppId)
        {
            Jinher.AMP.BTP.ISV.Facade.ShoppingCartFacade facade = new Jinher.AMP.BTP.ISV.Facade.ShoppingCartFacade();
            Jinher.AMP.BTP.Deploy.CustomDTO.NumResultSDTO model = facade.GetShoppingCartNum(userId, esAppId);
            return Json(new { Data = model.ShopCartNum }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult GetYJBJournal(int type, int pageIndex)
        {
            Guid userId = getLoginUserId();
            if (userId == Guid.Empty)
                return Json(new YJB.Deploy.CustomDTO.ResultDTO<YJB.Deploy.CustomDTO.UserYJBDTO> { Message = "用户不存在。" }, JsonRequestBehavior.AllowGet);
            var data = YJBSV.GetUserYJBJournal(new Jinher.AMP.YJB.Deploy.CustomDTO.OrderYJBInfoInputDTO
            {
                UserId = userId,
                Type = type,
                PageIndex = pageIndex,
                PageSize = 20
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult BindCouponInCommodityDetails(CouponCreateRequestDTO input)
        {
            return Json(CouponSV.Instance.BindCouponToUser(input));
        }

        [DealMobileUrl]
        [WeixinOAuthOpenId]
        public ActionResult MyCoupon()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetMyCoupon(Jinher.AMP.Coupon.Deploy.CustomDTO.ZSH.UserCouponInput input)
        {
            return Json(CouponSV.Instance.GetMyUsableCoupons(input));
        }


        /// <summary>
        /// 获取跨店铺满减券的商品金额合计，每选择一次跨店满减券，就会调用这个一下，这个结果为拆单准备。
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult getStoreCouponCommidtyTotalPrice(Jinher.AMP.Coupon.Deploy.CustomDTO.ZSH.UserCouponInput input, decimal StoreCouponValue)
        {
            decimal StoreCouponCommdityPrice = 0;
            int StoreCouponCommdityCount = 0;


            foreach (var item in input.Commodities)//遍历商品。
            {
                if (input.CouponType != CouponType.BeInCommon && !new Coupon.ISV.Facade.CouponFacade().IsUsableCoupon(item.AppId, item.CommodityId, input.CouponId, input.CouponType).IsSuccess)
                    continue;
                if (CacheHelper.MallApply.GetMallTypeListByEsAppId((Guid)input.EsAppId).Any(_ => _.Id == item.AppId && _.Type != 1))//如果这个商品所在的店铺是自营的
                {
                    // if (!item.UsedCoupon)//第三方的不用算，自营的已经过滤掉
                    var details = new ISV.Facade.CommodityFacade().GetCommodityDetails(item.CommodityId, item.AppId, Guid.Empty, "xxx");
                    if (details != null && details.PromotionTypeNew == ComPromotionStatusEnum.NoPromotion)
                    {
                        StoreCouponCommdityCount++;
                        StoreCouponCommdityPrice += item.Num * item.RealPrice;
                    }
                }
            }


            //取较小的值为 实际抵用的。
            StoreCouponValue = StoreCouponValue > StoreCouponCommdityPrice ? StoreCouponCommdityPrice : StoreCouponValue;
            List<storeCouponDto> li = new List<storeCouponDto>();

            int index = 0;

            foreach (var item in input.Commodities)//遍历商品。
            {
                if (input.CouponType != CouponType.BeInCommon && !new Coupon.ISV.Facade.CouponFacade().IsUsableCoupon(item.AppId, item.CommodityId, input.CouponId, input.CouponType).IsSuccess)
                    continue;

                if (CacheHelper.MallApply.GetMallTypeListByEsAppId((Guid)input.EsAppId).Any(_ => _.Id == item.AppId && _.Type != 1))//如果这个商品所在的店铺是自营的
                {
                    //if (!item.UsedCoupon)///第三方的不用算，自营的已经过滤掉
                    var details = new ISV.Facade.CommodityFacade().GetCommodityDetails(item.CommodityId, item.AppId, Guid.Empty, "xxx");
                    if (details != null && details.PromotionTypeNew == ComPromotionStatusEnum.NoPromotion)
                    {
                        index++;
                        if (index == StoreCouponCommdityCount)//最后一个
                            li.Add(new storeCouponDto { value = StoreCouponValue - li.Sum(t => t.value), appId = item.AppId, id = item.CommodityId });
                        else
                            li.Add(new storeCouponDto { value = decimal.Round(((item.RealPrice * item.Num) / StoreCouponCommdityPrice) * StoreCouponValue, 2), appId = item.AppId, id = item.CommodityId });
                    }
                }
            }
            return Json(new { StoreCouponCommdityPrice, StoreCouponCommdityCount, li });
        }

        private class storeCouponDto
        {
            public decimal value;
            public Guid appId;
            public Guid id;
        }






        [HttpPost]
        public ActionResult GetMyStoreCoupon(Jinher.AMP.Coupon.Deploy.CustomDTO.ZSH.UserCouponInput input)
        {
            var result = new List<Jinher.AMP.Coupon.Deploy.CustomDTO.ZSH.UserCouponOutput>();
            try
            {
                Jinher.AMP.Coupon.ISV.Facade.CouponFacade couponFacade = new Coupon.ISV.Facade.CouponFacade();
                couponFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                couponFacade.ContextDTO.LoginOrg = Guid.Empty;

                var coResult = couponFacade.GetStoreCoupon(input);
                if (coResult.IsSuccess)
                {
                    result = coResult.Data;
                }
                else
                {
                    LogHelper.Info(string.Format("CouponSV.GetMyUsableCoupons获取我的商品优惠券信息失败。 condition：{0}, result: {1}", JsonHelper.JsonSerializer(input), JsonHelper.JsonSerializer(coResult)));
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CouponSV.GetUserCouponsStoresByIds服务异常:获取应用信息异常。 "), ex);
            }

            //var re = CouponSV.Instance.GetStoreCoupon(input);
            return Json(result);
        }
        /// <summary>
        /// 京东订单售后服务流程
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult JdOrderServiceTrackInfo(string id)
        {
            var trackInfo = JDSV.GetServiceDetailInfo(id);
            return View(trackInfo.serviceTrackInfoDTOs.OrderByDescending(_ => _.createDate).ToList());
        }
        /// <summary>
        /// 获取用户最新的订单物流信息
        /// </summary>
        /// <param name="AppId"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetUserOrderExpress(Guid appId, Guid userId)
        {
            ResultDTO result = null;
            try
            {
                OrderExpressRouteFacade OrderExpress = new OrderExpressRouteFacade();
                result = OrderExpress.GetUserNewOrderExpress(appId, userId);
            }
            catch (Exception ex)
            {
                result = new ResultDTO() { ResultCode = 1, Message = ex.Message, isSuccess = false };
                LogHelper.Debug(string.Format("物流异常信息:{0}", ex.Message));
            }
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 移动端客服系统订单页面
        /// </summary>
        /// <returns></returns>
        public ActionResult CustomerServiceOrder()
        {
            return View();
        }

        public ActionResult GetCustomOrder(OrderQueryParamDTO orderQueryParamDTO)
        {
            //var appId = Guid.Parse("1375ad99-de3b-4e93-80d5-5b96e1588967");
            ////if (appId == null && Session["appId"] != null)
            ////{
            ////    appId = (Guid)Session["appId"];
            ////}
            ////Guid UserId = Jinher.JAP.BF.BE.Deploy.Base.ContextDTO.Current.LoginUserID;
            //var UserId = Guid.Parse("581bb23c-5447-4fa3-9eef-898d00054da0");
            //orderQueryParamDTO.EsAppId = appId;
            //orderQueryParamDTO.UserId = UserId;
            Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade facade = new ISV.Facade.CommodityOrderFacade();

            var result = facade.GetCustomCommodityOrderByUserIDNew(orderQueryParamDTO);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 交易成功页面
        /// </summary>
        /// <returns></returns>
        public ActionResult TradesSuccessfully()
        {
            try
            {
                Jinher.AMP.ZPH.ISV.Facade.ToRecommendFacade ToRemd = new ZPH.ISV.Facade.ToRecommendFacade();
                Jinher.AMP.ZPH.Deploy.MobileCDTO.YJBJToRecommendParam param = new ZPH.Deploy.MobileCDTO.YJBJToRecommendParam()
                {
                    pageOpt = Jinher.AMP.ZPH.Deploy.Enum.PageOperate.FirstOpen,
                    pageSize = 20,
                    rankNo = 0,
                    appid = "jinHe",
                    category = "",
                    sceneId = "evaPage",
                    itemId = Request.QueryString["commodityid"],
                    esappid = Request.QueryString["esappid"]
                };
                var result = ToRemd.GetHotRed(param);
                if (result != null)
                {
                    if (result.Data.Count % 2 != 0)//奇数删除最后一条
                        result.Data.RemoveAt(result.Data.Count - 1);
                    ViewBag.GDTJList = result.Data;
                    LogHelper.Info(string.Format("进入交易成功页面，参数：itemId:{4},esappid:{5} | 返回Data：{0},返回结果isSuccess:{1},Message:{2},Code:{3}", JsonHelper.JsonSerializer(result.Data), result.isSuccess.ToString(), result.Message, result.Code, Request.QueryString["commodityid"], Request.QueryString["esappid"]));
                }
                else
                    ViewBag.GDTJList = null;
                return View();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("进入交易成功页面发生错误，参数：itemId:{0},esappid:{1} ", Request.QueryString["commodityid"], Request.QueryString["esappid"]), ex);
                return View();
            }
        }

        /// <summary>
        /// 获取交易成功的更多推荐
        /// </summary>
        /// <param name="AppId"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetTradesSuccessfullyMore(string esappid, string userId)
        {
            Jinher.AMP.ZPH.ISV.Facade.ToRecommendFacade ToRemd = new ZPH.ISV.Facade.ToRecommendFacade();
            Jinher.AMP.ZPH.Deploy.MobileCDTO.YJBJToRecommendParam param = new ZPH.Deploy.MobileCDTO.YJBJToRecommendParam()
            {
                pageOpt = Jinher.AMP.ZPH.Deploy.Enum.PageOperate.FirstOpen,
                pageSize = 20,
                rankNo = 0,
                appid = "jinHe",
                category = "",
                sceneId = "evaPage",
                userId = userId,
                esappid = esappid
            };
            var result = ToRemd.GetHotRed(param);
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRefundOrderList(OrderQueryParamDTO orderQueryParamDTO)
        {
            Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade facade = new ISV.Facade.CommodityOrderFacade();
            var result = facade.GetRefundList(orderQueryParamDTO);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RefundList()
        {
            return View();
        }

        [DealMobileUrl]
        [ArgumentExceptionDeal(Title = "退款/退货申请")]
        public ActionResult RefundOrder2(string type, Guid shopId, string orderId, string pri, string state, string userId, string pay, string spendScoreMoney, string spendYJBMoney, string spendCouponMoney, string orderItemId)
        {
            decimal CurrPic = Convert.ToDecimal(pri);
            decimal SpendScoreMoney = Convert.ToDecimal(spendScoreMoney);
            decimal SpendCouponMoney = Convert.ToDecimal(spendCouponMoney);
            decimal SpendYJBMoney = Convert.ToDecimal(spendYJBMoney);
            decimal yjCouponPrice = 0;
            ViewBag.appId = shopId;
            ViewBag.orderId = orderId;
            bool canOnlyRefund = true;
            var orderItemList = new List<OrderListItemCDTO>();
            if (string.IsNullOrEmpty(orderItemId) || orderItemId == "00000000-0000-0000-0000-000000000000")
            {
                if (state == "1" || state == "13")
                {
                    ViewBag.pic = (CurrPic).ToString(CultureInfo.InvariantCulture);
                    //ViewBag.pic = (CurrPic + SpendScoreMoney + SpendYJBMoney - SpendCouponMoney).ToString(CultureInfo.InvariantCulture);
                }
                else if (state == "2" || state == "3")
                {
                    canOnlyRefund = false;
                    var commodityOrderFacade = new Jinher.AMP.BTP.IBP.Facade.CommodityOrderFacade();
                    var order = commodityOrderFacade.GetCommodityOrderInfo(new Guid(orderId));
                    LogHelper.Info("CurrPic " + CurrPic + " - order.Freight" + order.Freight);
                    ViewBag.pic = (CurrPic + SpendScoreMoney + SpendYJBMoney - SpendCouponMoney - order.Freight).ToString(CultureInfo.InvariantCulture);
                }
                var cc = new CommodityOrderFacade();
                var orderItems = cc.GetOrderItems(new Guid(orderId), new Guid(userId), shopId);
                orderItemList = orderItems.ShoppingCartItemSDTO;
            }
            else
            {
                //单品退款
                CommodityOrderFacade commodityOrderFacade = new CommodityOrderFacade();
                var orderItems = commodityOrderFacade.GetOrderItems(new Guid(orderId), new Guid(userId), shopId);
                var orderItem = orderItems.ShoppingCartItemSDTO.FirstOrDefault(t => t.Id == new Guid(orderItemId));
                if (orderItem != null)
                {
                    CurrPic = (orderItem.RealPrice * orderItem.CommodityNumber);
                    if (CurrPic == 0)
                    {
                        CurrPic = (orderItem.DiscountPrice * orderItem.CommodityNumber);
                    }

                    if (state == "1" || state == "13")
                    {
                        // 待发货时最高退款金额=该商品实际售价-该商品承担的优惠券金额+该商品的运费-该商品承担的改价运费金额-该商品承担的改价商品金额+关税-易捷抵用券
                        //ViewBag.pic = (CurrPic - orderItem.CouponPrice + orderItem.FreightPrice - orderItem.ChangeFreightPrice - orderItem.ChangeRealPrice + orderItem.Duty - orderItem.YJCouponPrice).ToString(CultureInfo.InvariantCulture);
                        ViewBag.pic = (CurrPic + orderItem.FreightPrice - orderItem.ChangeFreightPrice - orderItem.ChangeRealPrice + orderItem.Duty - orderItem.YJCouponPrice).ToString(CultureInfo.InvariantCulture);
                    }
                    else if (state == "2" || state == "3")
                    {
                        canOnlyRefund = false;
                        //已发货时默认退款金额=该商品实际售价-该商品承担的优惠券金额-该商品承担的改价商品金额—关税-易捷抵用券
                        //ViewBag.pic = (CurrPic - orderItem.CouponPrice - orderItem.ChangeRealPrice - orderItem.Duty - orderItem.YJCouponPrice).ToString(CultureInfo.InvariantCulture);
                        ViewBag.pic = (CurrPic - orderItem.ChangeRealPrice - orderItem.Duty - orderItem.YJCouponPrice).ToString(CultureInfo.InvariantCulture);
                    }
                    orderItemList = new List<OrderListItemCDTO> { orderItem };
                }
            }
            ViewBag.orderItemList = orderItemList;
            ViewBag.state = state;
            ViewBag.pay = pay;
            ViewBag.CanOnlyRefund = canOnlyRefund || !ThirdECommerceHelper.IsWangYiYanXuan(shopId);
            return View();
        }

        #region 易捷卡相关的方法
        /// <summary>
        /// 发送手机验证码
        /// </summary>
        /// <param name="cachekey">缓存中验证码的key</param>
        ///<param name="YJCId">YJCID</param>
        /// <returns></returns>
        public ActionResult GetYJCValidateCode(string cachekey, Guid YJCId)
        {
            ResultDTO dto = new ResultDTO();
            YJB.ISV.Facade.YJCardFacade yjcFacade = new YJB.ISV.Facade.YJCardFacade();
            var yjc = yjcFacade.GetMyYJCard(new YJB.Deploy.CustomDTO.QueryUserYJCardDTO
            {
                YJCId = YJCId
            });
            Jinher.AMP.PIP.ISV.Facade.AppSmsQueryFacade facade = new PIP.ISV.Facade.AppSmsQueryFacade();
            string vcode = CommonUtil.CreateValidateCode();//生成的验证码
            var result = facade.SendAliYunSms(CustomConfig.YJAppId, yjc.Data.BindPhone, "CBCRegister", vcode);
            RedisHelper.Set(cachekey, "1", TimeSpan.FromMinutes(1));//测试验证码都是1
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 校验手机验证码
        /// </summary>
        /// <returns></returns>
        public ActionResult CheckValidateCode(string cachekey, string Code)
        {
            ResultDTO dto = new ResultDTO();
            //dto.isSuccess=
            if (RedisHelper.Get(cachekey) != null && RedisHelper.Get(cachekey).ToString().Replace("\"","") == Code)
            {
                dto.Message = "验证通过";
                dto.isSuccess = true;
            }
            return Json(dto, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// //根据易捷卡id查询卡的信息
        /// </summary>
        /// <param name="IDs"></param>
        /// <returns></returns>
        public ActionResult GetYJCInfoByID(string IDs)
        {
            if (string.IsNullOrWhiteSpace(IDs))
            {
                return Json("参数为空！", JsonRequestBehavior.AllowGet);
            }
            ResultDTO<List<Jinher.AMP.YJB.Deploy.YJCardDTO>> result = new ResultDTO<List<Jinher.AMP.YJB.Deploy.YJCardDTO>>();
            List<Jinher.AMP.YJB.Deploy.YJCardDTO> list = new List<YJB.Deploy.YJCardDTO>();
            result.Data = list;
            Jinher.AMP.YJB.ISV.Facade.YJCardFacade fa = new YJB.ISV.Facade.YJCardFacade();

            foreach (string id in IDs.Split(','))
            {
                var yjc = fa.GetMyYJCard(new YJB.Deploy.CustomDTO.QueryUserYJCardDTO { YJCId = Guid.Parse(id) });
                result.Data.Add(yjc.Data);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        /// <summary>
        /// 保险页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Insurance()
        {
            IBP.Facade.InsuranceCompanyFacade insuranceCompanyFacade = new IBP.Facade.InsuranceCompanyFacade();
            var result = insuranceCompanyFacade.GetInsuranceCompany();
            ViewBag.Company = result.Data;
            return View();
        }

        [HttpPost]
        public JsonResult GetUserInfo(Guid userId)
        {
            var uf = new UserInfo();
            LogHelper.Debug("保险页面GotoInsurance参数，myUserId" + userId + "");
            var yjUserId = CBCSV.GetYJUserId(userId);
            var userInfo = CBCSV.GetUserNameAndCode(userId);
            var staffPhone = userInfo.Item2;
            //var staffCode = staffPhone;
            LogHelper.Debug("保险页面GotoInsurance参数，YJAppId" + Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId + ",userId" + userId + "staffPhone" + staffPhone + "");
            Jinher.AMP.BTP.ISV.Facade.YJEmployeeFacade facade = new BTP.ISV.Facade.YJEmployeeFacade();
            var YJStaff = facade.GetUserCodeByAcccount(Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId, userId, staffPhone);
            if (YJStaff != null && YJStaff.Data != null && YJStaff.Data != null && YJStaff.isSuccess && !string.IsNullOrEmpty(YJStaff.Data.UserCode))
            {
                //是易捷员工
                uf.UserPhone = staffPhone;
                uf.IsShareSign = 1;
                uf.yjUserId = yjUserId;
            }
            else
            {
                uf.UserPhone = staffPhone;
                uf.IsShareSign = 0;
                uf.yjUserId = yjUserId;
            }
            return Json(uf);
        }

        /// <summary>
        /// 返回用户信息
        /// </summary>
        public class UserInfo
        {
            /// <summary>
            /// 用户手机
            /// </summary>
            public string UserPhone { get; set; }

            /// <summary>
            /// 是否为员工
            /// </summary>
            public int IsShareSign { get; set; }
            /// <summary>
            /// 易捷UserId
            /// </summary>
            public string yjUserId { get; set; }
        }

        /// <summary>
        /// 参数类
        /// </summary>
        [DataContract]
        [Serializable]
        public class CreateOrderActionParam
        {
            /// <summary>
            /// 选择的易捷卡的id多张以逗号分隔
            /// </summary>
            public string YjcIds { get; set; }

            /// <summary>
            /// 用户id
            /// </summary>
            [DataMember]
            public Guid userId { get; set; }
            /// <summary>
            /// sessionId
            /// </summary>
            [DataMember]
            public string sessionId { get; set; }
            /// <summary>
            /// 应用id
            /// </summary>
            [DataMember]
            public Guid appId { get; set; }
            /// <summary>
            /// 收货地址Id
            /// </summary>
            [DataMember]
            public Guid addressId { get; set; }
            /// <summary>
            /// 是否显示代金券
            /// </summary>
            [DataMember]
            public bool isShowCoupon { get; set; }
            /// <summary>
            /// 以,分隔的商品Id串。
            /// </summary>
            [DataMember]
            public string commodityIds { get; set; }
            /// <summary>
            /// 下订单方式： 购物车结算（gouwuche） ;直接购买。
            /// </summary>
            [DataMember]
            public string coType { get; set; }
            /// <summary>
            /// 区域编码
            /// </summary>
            [DataMember]
            public string areaCode { get; set; }
            /// <summary>
            /// 电商馆id
            /// </summary>
            [DataMember]
            public Guid esAppId { get; set; }
            /// <summary>
            /// 是否需要显示积分。
            /// </summary>
            [DataMember]
            public bool isShowScore { get; set; }
            /// <summary>
            /// 以,分隔的商品所在店铺Id
            /// </summary>
            [DataMember]
            public string appIds { get; set; }
            /// <summary>
            /// 配送方式：0 app没有自提功能，1快递，2自提，3两者
            /// </summary>
            [DataMember]
            public int appSelfTakeWay { get; set; }
            /// <summary>
            /// Id
            /// </summary>
            [DataMember]
            public Guid appSelfTakeStationId { get; set; }
            /// <summary>
            /// 类型：1 按Id查；2 按EsAppId,UserId查
            /// </summary>
            [DataMember]
            public int searchTypeForAppSelfTake { get; set; }
            /// <summary>
            /// 商品列表
            /// </summary>
            [DataMember]
            public List<ComScoreCheckDTO> comList { get; set; }
            /// <summary>
            /// 优惠套装id
            /// </summary>
            [DataMember]
            public Guid setMealId { get; set; }
            /// <summary>
            /// 金采团购活动id
            /// </summary>
            [DataMember]
            public Guid jcActivityId { get; set; }
        }

        /// <summary>
        /// CreateOrder页面加载时所有要并行调用的接口。
        /// </summary>
        [DataContract]
        [Serializable]
        public class CreateOrderPageLoadAction : ResultDTO
        {
            #region 字段

            private List<dynamic> yjcList;
            /// <summary>
            /// 可用易捷卡数量
            /// </summary>
            private int yjcNum = 0;
            /// <summary>
            /// 当前用户金币余额
            /// </summary>
            private ulong _goldBalance = 0;
            /// <summary>
            /// 当前用户代金券张数。
            /// </summary>
            private int _couponCount = 0;
            /// <summary>
            /// 默认的地址信息。
            /// </summary>
            private AddressSDTO _addressInfo;
            /// <summary>
            /// 用户积分
            /// </summary>
            private OrderScoreCheckResultDTO _userScore;
            /// <summary>
            /// 订单中所有商品的自提属性。
            /// </summary>
            private ReturnInfo<List<Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySelfTakeListDTO>> _commoditySelfTakeList;
            private UserOrderCarDTO _userCrowdfunding;
            /// <summary>
            /// 自提点信息。
            /// </summary>
            private SelfTakeStationSearchResultDTO _selfTakeStation;
            private bool _isAllAppSupportCOD;
            private int _tradeType;
            private bool _isAllAppInZPH;
            private bool _isInvoice;
            #endregion
            #region 属性
            /// <summary>
            /// 易捷卡信息
            /// </summary>
            public List<dynamic> YjcList
            {
                get
                {
                    return yjcList;
                }

                set
                {
                    yjcList = value;
                }
            }

            /// <summary>
            /// 可用易捷卡数量
            /// </summary>
            public int YjcNum
            {
                get
                {
                    return yjcNum;
                }

                set
                {
                    yjcNum = value;
                }
            }

            /// <summary>
            /// 用户积分
            /// </summary>
            [DataMember]
            public OrderScoreCheckResultDTO UserScore
            {
                get { return _userScore; }
                set { _userScore = value; }
            }
            /// <summary>
            /// 当前用户金币余额
            /// </summary>
            [DataMember]
            public ulong GoldBalance
            {
                get { return _goldBalance; }
                set { _goldBalance = value; }
            }
            /// <summary>
            /// 当前用户代金券张数。
            /// </summary>
            [DataMember]
            public int CouponCount
            {
                get { return _couponCount; }
                set { _couponCount = value; }
            }
            /// <summary>
            /// 默认的地址信息。
            /// </summary>
            [DataMember]
            public AddressSDTO AddressInfo
            {
                get { return _addressInfo; }
                set { _addressInfo = value; }
            }
            /// <summary>
            /// 是不是打开发票开关
            /// </summary>
            [DataMember]
            public bool IsInvoice
            {
                get { return _isInvoice; }
                set { _isInvoice = value; }
            }
            /// <summary>
            /// 订单中所有商品的自提属性。
            /// </summary>
            [DataMember]
            public ReturnInfo<List<Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySelfTakeListDTO>> CommoditySelfTakeList
            {
                get { return _commoditySelfTakeList; }
                set { _commoditySelfTakeList = value; }
            }
            /// <summary>
            /// 用户众筹信息
            /// </summary>
            [DataMember]
            public UserOrderCarDTO UserCrowdfunding
            {
                get { return _userCrowdfunding; }
                set { _userCrowdfunding = value; }
            }
            /// <summary>
            /// 自提点信息。
            /// </summary>
            [DataMember]
            public SelfTakeStationSearchResultDTO SelfTakeStation
            {
                get { return _selfTakeStation; }
                set { _selfTakeStation = value; }
            }
            /// <summary>
            ///  是不是所有店铺app都支持“货到付款”。
            /// </summary>
            [DataMember]
            public bool IsAllAppSupportCOD
            {
                get { return _isAllAppSupportCOD; }
                set { _isAllAppSupportCOD = value; }
            }
            /// <summary>
            /// 交易类型：0:担保交易;1：非担保交易（直接到账）
            /// </summary>
            [DataMember]
            public int TradeType
            {
                get { return _tradeType; }
                set { _tradeType = value; }
            }
            /// <summary>
            /// 是不是订单中的所有app都是代运营app
            /// </summary>
            [DataMember]
            public bool IsAllAppInZPH
            {
                get { return _isAllAppInZPH; }
                set { _isAllAppInZPH = value; }
            }
            /// <summary>
            /// 配送方式：0 app没有自提功能，1快递，2自提，3两者
            /// </summary>
            [DataMember]
            public int AppSelfTakeWay { get; set; }
            /// <summary>
            /// 关税
            /// </summary>
            [DataMember]
            public CreateOrderDutyResultDTO Dutys { get; set; }
            /// <summary>
            /// App自提点信息
            /// </summary>
            [DataMember]
            public AppSelfTakeStationDefaultInfoDTO AppSelfTakeStationDefaultInfo { get; set; }
            /// <summary>
            /// 易捷币信息
            /// </summary>
            [DataMember]
            public YJB.Deploy.CustomDTO.OrderInsteadCashDTO YJBInfo { get; set; }
            /// <summary>
            /// 店铺商品可赠送油卡兑换券金额汇总
            /// </summary>
            [DataMember]
            public List<YJB.Deploy.CustomDTO.CommodityYouKaDTO> AppYouKa { get; set; }
            /// <summary>
            /// 店铺优惠套装
            /// </summary>
            [DataMember]
            public ZPH.Deploy.CustomDTO.SetMealActivityCDTO setMeal { get; set; }
            /// <summary>
            /// 金采团购活动 赠送油卡总额
            /// </summary>
            [DataMember]
            public decimal jcActivityYouKa { get; set; }
            /// <summary>
            /// 商品类型列表(0实物商品，1(虚拟商品)易捷卡密)
            /// </summary>
            [DataMember]
            public List<int> CommodityTypeList { get; set; }



            #endregion
            #region 方法
            /// <summary>
            ///  获取当前用户金币余额。
            /// </summary>
            /// <param name="userId">用户id</param>
            /// <param name="sessionId">sessionId</param>
            public void GetBalance(object state)
            {
                CreateOrderActionParam cp = (CreateOrderActionParam)state;
                string methodInfo = this.GetType() + ".GetBalance";
                this._goldBalance = PayModel.GetBalance(cp.userId, cp.sessionId, methodInfo);
            }
            /// <summary>
            /// 获取当前用户代金券张数。
            /// </summary>
            /// <param name="userId">当前用户id</param>
            /// <param name="invoker">调用方</param>
            /// <returns></returns>
            public void GetGoldCouponCount(object state)
            {
                CreateOrderActionParam cp = (CreateOrderActionParam)state;
                string methodInfo = this.GetType() + ".GetGoldCouponCount";
                this._couponCount = PayModel.GetGoldCouponCount(cp.userId, methodInfo);
            }
            /// <summary>
            /// 获取用户下订单时显示的收货地址。
            /// </summary>
            /// <param name="addressId">地址id</param>
            /// <param name="appId">应用id</param>
            /// <param name="userId">当前用户id</param>
            public void GetDeliveryAddressDefault(object state)
            {
                CreateOrderActionParam cp = (CreateOrderActionParam)state;
                string methodInfo = this.GetType() + ".GetDeliveryAddressDefault";
                if (cp.addressId != Guid.Empty)
                {
                    this._addressInfo = DeliveryAddressModel.GetDeliveryAddressByAddressId(cp.addressId, cp.appId,
                        methodInfo);
                }
                else
                {
                    List<AddressSDTO> addressList = DeliveryAddressModel.GetDeliveryAddressList(cp.userId, cp.appId, 1,
                        methodInfo);
                    if (addressList == null || addressList.Count == 0)
                    {
                        return;
                    }
                    this._addressInfo = addressList[0];
                }
            }
            /// <summary>
            /// 获取商品是否支持自提。
            /// </summary>
            /// <param name="state"></param>
            public void GetCommodityIsEnableSelfTakeList(object state)
            {
                CreateOrderActionParam cp = (CreateOrderActionParam)state;
                string methodInfo = this.GetType() + ".GetCommodityIsEnableSelfTakeList";
                this._commoditySelfTakeList = CommodityVMUI.GetCommodityIsEnableSelfTakeList(cp.commodityIds);
            }
            /// <summary>
            /// 用户众筹信息
            /// </summary>
            /// <param name="state"></param>
            public void GetUserCrowdfundingBuy(object state)
            {
                CreateOrderActionParam cp = (CreateOrderActionParam)state;
                string methodInfo = this.GetType() + ".GetUserCrowdfundingBuy";
                this._userCrowdfunding = CrowdfundingVM.GetUserCrowdfundingBuy(cp.appId, cp.userId, methodInfo);
            }
            /// <summary>
            /// 自提点信息
            /// </summary>
            /// <param name="state"></param>
            public void GetSelfTakeStation(object state)
            {
                CreateOrderActionParam cp = (CreateOrderActionParam)state;
                string methodInfo = this.GetType() + ".GetSelfTakeStation";

                int rowCount = 0;

                SelfTakeStationSearchDTO stsSearch = new SelfTakeStationSearchDTO();
                stsSearch.Code = cp.areaCode;
                stsSearch.pageIndex = 1;
                stsSearch.pageSize = 10;
                stsSearch.rowCount = 10;

                var result = SelfTakeStationVM.GetSelfTakeStation(stsSearch, out rowCount, methodInfo);
                if (result != null && result.Count > 0)
                {
                    this._selfTakeStation = result[0];
                }
            }
            /// <summary>
            /// 配送信息
            /// </summary>
            /// <param name="state"></param>
            public void GetAppSelfTakeWay(object state)
            {
                CreateOrderActionParam cp = (CreateOrderActionParam)state;
                string methodInfo = this.GetType() + ".GetAppSelfTakeWay";

                var result = 0;
                var resultBAC = BACBP.CheckAppSelfTake(cp.esAppId);
                if (resultBAC)
                {
                    var resultZPH = ZPHSV.Instance.GetAppSelfTakeWay(cp.esAppId);
                    result = resultZPH;
                }
                else
                {
                    result = 0;
                }
                this.AppSelfTakeWay = result;
            }
            /// <summary>
            /// App自提点信息
            /// </summary>
            /// <param name="state"></param>
            public void GetAppSelfTakeStationDefault(object state)
            {
                CreateOrderActionParam cp = (CreateOrderActionParam)state;
                string methodInfo = this.GetType() + ".GetAppSelfTakeStationDefault";

                Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationSearchDTO search = new AppSelfTakeStationSearchDTO();
                search.EsAppId = cp.esAppId;
                search.UserId = cp.userId;
                search.Id = cp.appSelfTakeStationId;
                search.SearchType = cp.searchTypeForAppSelfTake;

                AppSelfTakeStationFacade facade = new AppSelfTakeStationFacade();
                var result = facade.GetAppSelfTakeStationDefault(search);
                this.AppSelfTakeStationDefaultInfo = result;
            }
            /// <summary>
            /// 获取用户在当前应用中的积分。
            /// </summary>
            /// <param name="state"></param>
            public void GetUserScore(object state)
            {
                CreateOrderActionParam cp = (CreateOrderActionParam)state;

                OrderScoreCheckDTO pdto = new OrderScoreCheckDTO();
                pdto.UserId = cp.userId;
                pdto.EsAppId = cp.esAppId;
                pdto.Coms = cp.comList;

                Jinher.AMP.BTP.ISV.Facade.ScoreSettingFacade ssFacade =
                    new Jinher.AMP.BTP.ISV.Facade.ScoreSettingFacade();
                ResultDTO<OrderScoreCheckResultDTO> usResult = ssFacade.OrderScoreCheck(pdto);
                if (usResult.ResultCode != 0)
                {
                    return;
                }
                this._userScore = usResult.Data;
            }
            /// <summary>
            /// 是不是所有店铺app都支持“货到付款”。
            /// </summary>
            /// <param name="state"></param>
            public void GetIsAllAppSupportCOD(object state)
            {
                try
                {
                    CreateOrderActionParam cp = (CreateOrderActionParam)state;
                    if (string.IsNullOrWhiteSpace(cp.appIds))
                    {
                        _isAllAppSupportCOD = false;
                        return;
                    }
                    var key = "IsAllAppSupport:" + cp.appIds;
                    var supported = HttpRuntime.Cache.Get(key) as bool?;
                    if (supported.HasValue)
                    {
                        _isAllAppSupportCOD = supported.Value;
                    }
                    else
                    {
                        List<string> appids =
                            cp.appIds.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                        List<Guid> listAppIds = appids.ConvertAll(appId => new Guid(appId));
                        Jinher.AMP.BTP.ISV.Facade.PaymentsFacade pmFacade = new Jinher.AMP.BTP.ISV.Facade.PaymentsFacade();
                        ResultDTO<bool> result = pmFacade.IsAllAppSupportCOD(listAppIds);
                        supported = result.Data;
                        HttpRuntime.Cache.Add(key, result.Data, null, DateTime.Now.AddMinutes(10), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                        LogHelper.Debug("Mobile.IsAllAppSupportCOD from DB......");
                        _isAllAppSupportCOD = result.Data;
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error("GetIsAllAppSupportCOD异常，异常信息：", ex);
                }
            }
            /// <summary>
            /// 取关税
            /// </summary>
            /// <param name="state"></param>
            public void GetAllDuty(object state)
            {
                CreateOrderActionParam cp = (CreateOrderActionParam)state;
                ISV.Facade.CommodityFacade facade = new CommodityFacade();
                var commodityResult = facade.GetComListDuty(cp.comList);
                if (commodityResult != null && commodityResult.ResultCode == 0)
                {
                    this.Dutys = commodityResult.Data;
                    this.CommodityTypeList = new List<int>();
                    this.Dutys.List.ForEach(p => this.CommodityTypeList.AddRange(p.Coms.Select(x => x.Type).ToList()));
                    this.CommodityTypeList = this.CommodityTypeList.Distinct().ToList();
                }
            }

            /// <summary>
            /// 店铺商品可抵现易捷币汇总
            /// </summary>
            /// <param name="state"></param>
            public void GetAppYJB(object state)
            {
                var cp = (CreateOrderActionParam)state;
                var input = new YJB.Deploy.CustomDTO.OrderInsteadCashInputDTO
                {
                    UserId = cp.userId,
                    Commodities = cp.comList.Where(p => !p.IsPromotion)
                        .Select(p => new YJB.Deploy.CustomDTO.OrderInsteadCashInputCommodityDTO
                        {
                            AppId = p.AppId,
                            Id = p.CommodityId,
                            Price = p.RealPrice,
                            Number = p.Num,
                        }).ToList()
                };
                var yjbInfo = YJBHelper.GetCommodityCashPercent(cp.esAppId, input).YJBInfo;
                if (yjbInfo != null && yjbInfo.CommodityList != null && yjbInfo.CommodityList.Count > 0)
                {
                    yjbInfo.CommodityList = yjbInfo.CommodityList.GroupBy(p => p.AppId).Select(p => new YJB.Deploy.CustomDTO.CommodityInsteadCashDTO
                    {
                        AppId = p.Key,
                        InsteadCashAmount = p.Sum(x => x.InsteadCashAmount),
                        InsteadCashCount = p.Sum(x => x.InsteadCashCount)
                    }).ToList();
                }
                this.YJBInfo = yjbInfo;
            }
            /// <summary>
            /// 店铺商品可赠送油卡兑换券金额汇总
            /// </summary>
            /// <param name="state"></param>
            public void GetAppYouKa(object state)
            {
                var cp = (CreateOrderActionParam)state;
                var ykPercentList = YJBHelper.GetCommodityYouKaPercent(cp.esAppId
                    , cp.comList.Where(p => !p.IsPromotion).Select(p => p.CommodityId).ToList());
                ykPercentList.ForEach(p =>
                {
                    p.AppId = cp.comList.Where(x => x.CommodityId == p.CommodityId).Select(x => x.AppId).FirstOrDefault();
                });
                this.AppYouKa = ykPercentList.GroupBy(p => p.AppId).Select(p => new YJB.Deploy.CustomDTO.CommodityYouKaDTO
                {
                    AppId = p.Key,
                    YouKaPersent = p.Max(x => x.YouKaPersent),//只用于立即购买
                    GiveMoney = p.Sum(x => Math.Round(x.YouKaPersent * cp.comList.Where(z => z.CommodityId == x.CommodityId).Select(z => z.RealPrice * z.Num).FirstOrDefault() / 100, 2))
                }).ToList();
            }
            /// <summary>
            /// 店铺优惠套装汇总
            /// </summary>
            /// <param name="state"></param>
            public void GetAppSetMeal(object state)
            {
                var cp = (CreateOrderActionParam)state;
                //var setMealActivity = ZPHSV.Instance.GetSetMealActivitysById(cp.setMealId);
                var setMealActivity = CacheHelper.ZPH.GetSetMealActivitysById(cp.setMealId);
                this.setMeal = setMealActivity;
            }

            /// <summary>
            /// 金采团购活动数据
            /// </summary>
            /// <param name="state"></param>
            public void GetAppJcActivity(object state)
            {
                var cp = (CreateOrderActionParam)state;
                var jcActivityYouKa = ZPHSV.Instance.GetjcActivityYouKa(cp.jcActivityId, cp.comList);
                this.jcActivityYouKa = jcActivityYouKa;
            }
            /// <summary>
            /// 获取易捷卡数量
            /// </summary>
            /// <param name="obj"></param>
            public void GetYjcNum(object obj)
            {
                Jinher.AMP.YJB.ISV.Facade.YJCardFacade fac = new YJB.ISV.Facade.YJCardFacade();
                this.YjcNum = fac.GetYJCardNumber(new YJB.Deploy.CustomDTO.QueryUserYJCardDTO
                {
                    UserId = ContextDTO.Current.LoginUserID
                }).Data;
            }
            /// <summary>
            /// 获取易捷卡信息
            /// </summary>
            /// <param name="obj"></param>
            public void GetYjcInfos(object obj)
            {
                var cp = (CreateOrderActionParam)obj;
                if (!string.IsNullOrWhiteSpace(cp.YjcIds))
                {
                    Jinher.AMP.YJB.ISV.Facade.YJCardFacade fac = new YJB.ISV.Facade.YJCardFacade();
                    this.YjcList = new List<dynamic>();
                    foreach (string id in cp.YjcIds.Split(','))
                    {
                        var data = fac.GetMyYJCard(new YJB.Deploy.CustomDTO.QueryUserYJCardDTO { YJCId = Guid.Parse(id) });
                        this.YjcList.Add(new
                        {
                            Id = data.Data.Id,//ID
                            Balance = data.Data.Balance,//余额
                            CardNo = data.Data.Code,//卡号
                            Amount = data.Data.Cash,//面额
                            Phone = Regex.Replace(data.Data.BindPhone, "(\\d{3})\\d{4}(\\d{4})", "$1****$2")//绑定的手机号(隐藏中间几位)
                        });
                    }
                }
            }

            #endregion
        }

        public class ProviceCityModel
        {
            /// <summary>
            /// 编码
            /// </summary>
            public string A { get; set; }

            /// <summary>
            /// 等级省1，市2
            /// </summary>
            public int L
            {
                get
                {
                    int level = 2;
                    if (A.Substring(2, 2) == "00")
                    {
                        level = 2;
                    }
                    return level;
                }
            }

            /// <summary>
            /// 名称
            /// </summary>
            public string N { get; set; }

            /// <summary>
            /// 简拼
            /// </summary>
            public string S
            {
                get
                {
                    return ProvinceCityHelper.GetCityByAreaCode(A).SpellCode;
                    //return SpellDeal.GetSpellCode(N).ToLower();
                }
            }
        }

        public class SpellDeal
        {
            /// <summary> 
            /// 在指定的字符串列表CnStr中检索符合拼音索引字符串 
            /// </summary> 
            /// <param name="CnStr">汉字字符串</param> 
            /// <returns>相对应的汉语拼音首字母串</returns> 
            public static string GetSpellCode(string CnStr)
            {
                string strTemp = "";
                int iLen = CnStr.Length;
                int i = 0;

                for (i = 0; i <= iLen - 1; i++)
                {
                    strTemp += GetCharSpellCode(CnStr.Substring(i, 1));
                }

                return strTemp;
            }


            /// <summary> 
            /// 得到一个汉字的拼音第一个字母，如果是一个英文字母则直接返回大写字母 
            /// </summary> 
            /// <param name="CnChar">单个汉字</param> 
            /// <returns>单个大写字母</returns> 
            private static string GetCharSpellCode(string CnChar)
            {
                long iCnChar;

                byte[] ZW = System.Text.Encoding.Default.GetBytes(CnChar);

                //如果是字母，则直接返回 
                if (ZW.Length == 1)
                {
                    return CnChar.ToUpper();
                }
                else
                {
                    // get the array of byte from the single char 
                    int i1 = (short)(ZW[0]);
                    int i2 = (short)(ZW[1]);
                    iCnChar = i1 * 256 + i2;
                }

                //expresstion 
                //table of the constant list 
                // 'A'; //45217..45252 
                // 'B'; //45253..45760 
                // 'C'; //45761..46317 
                // 'D'; //46318..46825 
                // 'E'; //46826..47009 
                // 'F'; //47010..47296 
                // 'G'; //47297..47613 

                // 'H'; //47614..48118 
                // 'J'; //48119..49061 
                // 'K'; //49062..49323 
                // 'L'; //49324..49895 
                // 'M'; //49896..50370 
                // 'N'; //50371..50613 
                // 'O'; //50614..50621 
                // 'P'; //50622..50905 
                // 'Q'; //50906..51386 

                // 'R'; //51387..51445 
                // 'S'; //51446..52217 
                // 'T'; //52218..52697 
                //没有U,V 
                // 'W'; //52698..52979 
                // 'X'; //52980..53640 
                // 'Y'; //53689..54480 
                // 'Z'; //54481..55289 

                // iCnChar match the constant 
                if ((iCnChar >= 45217) && (iCnChar <= 45252))
                {
                    return "A";
                }
                else if ((iCnChar >= 45253) && (iCnChar <= 45760))
                {
                    return "B";
                }
                else if ((iCnChar >= 45761) && (iCnChar <= 46317))
                {
                    return "C";
                }
                else if ((iCnChar >= 46318) && (iCnChar <= 46825))
                {
                    return "D";
                }
                else if ((iCnChar >= 46826) && (iCnChar <= 47009))
                {
                    return "E";
                }
                else if ((iCnChar >= 47010) && (iCnChar <= 47296))
                {
                    return "F";
                }
                else if ((iCnChar >= 47297) && (iCnChar <= 47613))
                {
                    return "G";
                }
                else if ((iCnChar >= 47614) && (iCnChar <= 48118))
                {
                    return "H";
                }
                else if ((iCnChar >= 48119) && (iCnChar <= 49061))
                {
                    return "J";
                }
                else if ((iCnChar >= 49062) && (iCnChar <= 49323))
                {
                    return "K";
                }
                else if ((iCnChar >= 49324) && (iCnChar <= 49895))
                {
                    return "L";
                }
                else if ((iCnChar >= 49896) && (iCnChar <= 50370))
                {
                    return "M";
                }

                else if ((iCnChar >= 50371) && (iCnChar <= 50613))
                {
                    return "N";
                }
                else if ((iCnChar >= 50614) && (iCnChar <= 50621))
                {
                    return "O";
                }
                else if ((iCnChar >= 50622) && (iCnChar <= 50905))
                {
                    return "P";
                }
                else if ((iCnChar >= 50906) && (iCnChar <= 51386))
                {
                    return "Q";
                }
                else if ((iCnChar >= 51387) && (iCnChar <= 51445))
                {
                    return "R";
                }
                else if ((iCnChar >= 51446) && (iCnChar <= 52217))
                {
                    return "S";
                }
                else if ((iCnChar >= 52218) && (iCnChar <= 52697))
                {
                    return "T";
                }
                else if ((iCnChar >= 52698) && (iCnChar <= 52979))
                {
                    return "W";
                }
                else if ((iCnChar >= 52980) && (iCnChar <= 53640))
                {
                    return "X";
                }
                else if ((iCnChar >= 53689) && (iCnChar <= 54480))
                {
                    return "Y";
                }
                else if ((iCnChar >= 54481) && (iCnChar <= 55289))
                {
                    return "Z";
                }
                else return ("?");
            }
        }


        public class JdCache
        {
            public string JdporderId { get; set; }

            public string AppId { get; set; }
        }



        #region 苏宁-售后
        /// <summary>
        /// 苏宁---售后保存用户退款退货退单等操作
        /// </summary>
        public ActionResult SaveRefundSNOrderAfterSales(AddressInfo address, string type, string RefundExpCo = "", string RefundExpOrderNo = "",
            string appId = "", string state = "", string orderId = "",
            string pic = "", string money = "", string dec = "",
            string refundReason = "", string userId = "", string pay = "",
            string refundType = "", string orderItemId = "")
        {
            BTP.ISV.Facade.CommodityOrderAfterSalesFacade orderSV = new BTP.ISV.Facade.CommodityOrderAfterSalesFacade();


            ResultDTO result = new ResultDTO();

            SubmitOrderRefundDTO modelParam = new SubmitOrderRefundDTO();
            modelParam.commodityorderId = Guid.Parse(orderId);
            modelParam.Id = Guid.Parse(orderId);
            modelParam.RefundDesc = dec;
            modelParam.RefundExpCo = RefundExpCo;
            modelParam.RefundExpOrderNo = RefundExpOrderNo;
            modelParam.RefundMoney = decimal.Parse(money);
            modelParam.State = int.Parse(state);
            modelParam.RefundReason = refundReason;

            modelParam.RefundType = refundType == "1" ? 0 : 1;
            modelParam.OrderRefundImgs = pic;
            modelParam.OrderItemId = Guid.Parse(orderItemId);

            modelParam.Address = address;
            modelParam.IsSNOrder = true;
            result = orderSV.SubmitOrderRefundAfterSales(modelParam);
            return Json(result, JsonRequestBehavior.AllowGet);
        }




        /// <summary>
        /// 检查苏宁订单是否可以退款
        /// </summary>
        /// <param name="price">商品价格</param>
        /// <param name="skuId">苏宁skuId</param>
        /// <param name="snOrderId">苏宁订单Id</param>
        /// <param name="SnOrderItemId">苏宁子订单Id</param>
        /// <returns></returns>
        public ActionResult CheckSNRefundIsAvailable(string price, string skuId, string snOrderId, string SnOrderItemId)
        {
            //****测试数据
            //return Json(new ResultDTO { isSuccess = true, Message = "支持退货" }, JsonRequestBehavior.AllowGet);



            ResultDTO trtDto = new ResultDTO { isSuccess = false, Message = "不支持退货" };

            var customerExpects = SuningSV.SNJudgeOrderServiceType(new List<SNGetOrderServiceDTO>() { new SNGetOrderServiceDTO() { Price = price, SkuId = skuId } });
            //是否支持无理由退货(01-7天无理由退货；02-不支持退货)
            if (customerExpects.Any(_ => _.ReturnGoods == "01"))
            {

                int snOrderStatus = SNGetOrderStatus(snOrderId, SnOrderItemId);

                //订单行状态码： 1:审核中; 2:待发货; 3:待收货; 4:已完成; 5:已取消; 6:已退货; 7:待处理; 8：审核不通过，订单已取消; 9：待支付
                if (snOrderStatus == 2)
                {
                    trtDto = new ResultDTO { isSuccess = false, Message = "商品已出库，不能申请退款，可拒收~" };
                }
                else if (snOrderStatus == 3)
                {
                    trtDto = new ResultDTO { isSuccess = false, Message = "已发货不能申请退款，可拒收~" };
                }
                else
                {
                    //查询物流是否妥投

                    //获取苏宁物流信息
                    SNOrderLogistOutPutDTO outPut = SuningSV.SNGetOrderLogist(new SNOrderLogistInputDTO() { OrderId = snOrderId, OrderItemId = SnOrderItemId, SkuId = skuId });

                    if (outPut != null)
                    {

                        if (!string.IsNullOrWhiteSpace(outPut.ReceiveTime))
                        {

                            //获取用户确认收货时间
                            DateTime dt = DateTime.Parse(outPut.ReceiveTime);
                            //获取苏宁支持多少天退货
                            SNGetOrderServiceReturnDTO snGetOrderStatus = customerExpects.Where(p => p.SkuId == skuId).FirstOrDefault();
                            int snday = 0;

                            int.TryParse(snGetOrderStatus.NoReasonLimit, out snday);
                            //确认收货时间+支持退货天数>当前时间  在退货时间内
                            if (dt.AddDays(snday) > DateTime.Now)
                            {
                                trtDto = new ResultDTO { isSuccess = true, Message = "可以退款" };

                            }
                            else
                            {
                                trtDto = new ResultDTO { isSuccess = false, Message = "超过退款期限" };
                            }
                        }
                        else
                        {
                            trtDto = new ResultDTO { isSuccess = false, Message = "商品未妥投" };
                        }
                    }
                }




            }
            //trtDto = new ResultDTO { isSuccess = false, Message = "不支持退货" + JsonConvert.SerializeObject(customerExpects) };

            return Json(trtDto, JsonRequestBehavior.AllowGet);


        }

        /// <summary>
        /// 获取苏宁订单状态
        /// </summary>
        /// <param name="snOrderId"></param>
        /// <param name="snOrderItemId"></param>
        /// <returns></returns>
        private int SNGetOrderStatus(string snOrderId, string snOrderItemId)
        {
            //获取苏宁订单状态SNGetOrderStatus
            SNOrderStatusDTO orderStatus = SuningSV.SNGetOrderStatus(snOrderId);
            if (orderStatus != null)
            {
                SNOrderItemInfo itemStatus = orderStatus.OrderItemInfoList.Where(p => p.OrderItemId.Equals(snOrderItemId)).FirstOrDefault();

                if (itemStatus != null)
                {
                    return int.Parse(itemStatus.StatusName);
                }
            }
            return 999;
        }

        /// <summary>
        /// 判断苏宁商品是否支持7天无理由退货
        /// </summary>
        /// <param name="price"></param>
        /// <param name="skuId"></param>
        /// <returns></returns>
        public JsonResult SNJudgeIs7Return(string price, string skuId)
        {
            ResultDTO dto = new ResultDTO { isSuccess = false, Message = "不支持7天无理由退货" };
            var customerExpects = SuningSV.SNJudgeOrderServiceType(new List<SNGetOrderServiceDTO>() { new SNGetOrderServiceDTO() { Price = price, SkuId = skuId } });
            //是否支持无理由退货(01-7天无理由退货；02-不支持退货)
            if (customerExpects.Any(_ => _.ReturnGoods == "01"))
            {
                dto = new ResultDTO { isSuccess = true, Message = "可以退款" };
            }
            return Json(dto, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 退款详情页面
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl]
        public ActionResult SnFactoryDelivery()
        {
            return View();
        }
        #endregion
    }
}

