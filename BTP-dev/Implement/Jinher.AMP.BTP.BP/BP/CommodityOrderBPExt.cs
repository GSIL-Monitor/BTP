
/***************
功能描述: BTPBP
作    者: 
创建时间: 2014/3/19 16:55:03
***************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Jinher.AMP.App.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.AMP.BTP.TPS;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.FSP.ISV.Facade;
using Jinher.AMP.FSP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.BE.BELogic;
using System.IO;
using NPOI.XWPF.UserModel;
using NPOI.OpenXmlFormats.Wordprocessing;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using Jinher.AMP.BTP.TPS;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Jinher.AMP.BTP.TPS.Helper;

namespace Jinher.AMP.BTP.BP
{
    /// <summary>
    /// 
    /// </summary>
    public partial class CommodityOrderBP : BaseBP, ICommodityOrder
    {

        /// <summary>
        /// 获得订单详细信息
        /// </summary>
        /// <param name="id">商品订单ID</param>
        /// <returns></returns>
        public CommodityOrderVM GetCommodityOrderExt(System.Guid id, System.Guid appId)
        {
            try
            {
                OrderItem ot = new OrderItem();
                IEnumerable<CommodityOrderVM> queryOrder = from data in CommodityOrder.ObjectSet()
                                                           join opu in AppOrderPickUp.ObjectSet()
                                                           on data.Id equals opu.Id into opuOrderPickUps
                                                           from opuOrderPickUp in opuOrderPickUps.DefaultIfEmpty()
                                                           where data.Id == id && data.State != 16 && data.State != 17
                                                           select new CommodityOrderVM
                                                           {
                                                               AppId = data.AppId,
                                                               UserId = data.UserId,
                                                               ReceiptUserName = data.ReceiptUserName,
                                                               ReceiptAddress = data.ReceiptAddress,
                                                               ReceiptPhone = data.ReceiptPhone,
                                                               CommodityOrderId = data.Id,
                                                               CurrentPrice = data.RealPrice,
                                                               State = data.State,
                                                               CommodityOrderCode = data.Code,//订单编号
                                                               SubTime = data.SubTime,//下单时间
                                                               ConfirmTime = data.ConfirmTime,
                                                               PaymentTime = data.PaymentTime,
                                                               ShipmentsTime = data.ShipmentsTime,
                                                               Payment = data.Payment,
                                                               Province = data.Province,
                                                               City = data.City,
                                                               District = data.District,
                                                               Street = data.Street,
                                                               Details = data.Details,
                                                               RecipientsZipCode = data.RecipientsZipCode,
                                                               ShipExpCo = data.ShipExpCo,
                                                               ExpOrderNo = data.ExpOrderNo,
                                                               GoldCoupon = data.GoldCoupon,
                                                               GoldPrice = data.GoldPrice,
                                                               Price = data.Price,
                                                               Freight = data.Freight,
                                                               CouponValue = 0,
                                                               SelfTakeFlag = data.SelfTakeFlag,
                                                               OwnerShare = data.OwnerShare,
                                                               SelfTakeProvinceCode = opuOrderPickUp.StsProvince,
                                                               SelfTakeCityCode = opuOrderPickUp.StsCity,
                                                               SelfTakeDistrictCode = opuOrderPickUp.StsDistrict,
                                                               SelfTakeAddress = opuOrderPickUp.StsAddress,
                                                               OrderType = data.OrderType,
                                                               EsAppId = data.EsAppId,
                                                               ChannelShareMoney = data.ChannelShareMoney,
                                                               SelfTakeStationId = opuOrderPickUp.Id,
                                                               PickUpName = opuOrderPickUp.Name,
                                                               PickUpPhone = opuOrderPickUp.Phone,
                                                               PickUpBookDate = opuOrderPickUp.PickUpTime,
                                                               PickUpBookStartTime = opuOrderPickUp.BookStartTime,
                                                               PickUpBookEndTime = opuOrderPickUp.BookEndTime,
                                                               Batch = data.Batch,
                                                               Duty = data.Duty,
                                                               SetMealId = data.SetMealId == null ? Guid.Empty : (Guid)data.SetMealId,
                                                               JcActivityId = data.JcActivityId == null ? Guid.Empty : (Guid)data.JcActivityId

                                                           };

                CommodityOrderVM order = queryOrder.OrderByDescending(n => n.SubTime).FirstOrDefault();
                if (order != null)
                {
                    //发票附值
                    var invo = Invoice.ObjectSet().Where(p => p.CommodityOrderId == id).FirstOrDefault();
                    if (invo != null && invo.InvoiceType != 0)
                    {
                        order.InvoiceInfo = new InvoiceDTO();
                        order.InvoiceInfo.Id = invo.Id;
                        order.InvoiceInfo.CommodityOrderId = invo.CommodityOrderId;
                        order.InvoiceInfo.InvoiceTitle = invo.InvoiceTitle;
                        order.InvoiceInfo.InvoiceContent = invo.InvoiceContent;
                        order.InvoiceInfo.InvoiceType = invo.InvoiceType;
                        order.InvoiceInfo.SubTime = invo.SubTime;
                        order.InvoiceInfo.ModifiedOn = invo.ModifiedOn;
                        order.InvoiceInfo.ReceiptPhone = invo.ReceiptPhone;
                        order.InvoiceInfo.ReceiptEmail = invo.ReceiptEmail;
                        order.InvoiceInfo.State = invo.State;
                        order.InvoiceInfo.Remark = invo.Remark;
                        order.InvoiceInfo.Category = invo.Category;
                        order.InvoiceInfo.SubId = invo.SubId;
                        order.InvoiceInfo.Code = invo.Code;
                    }
                    //自提相关信息
                    if (order.SelfTakeStationId.HasValue && order.SelfTakeStationId.Value != Guid.Empty)
                    {
                        //自提点省市附值
                        if (!string.IsNullOrWhiteSpace(order.SelfTakeProvinceCode))
                            order.SelfTakeProvinceName = ProvinceCityHelper.GetAreaNameByCode(order.SelfTakeProvinceCode);
                        if (!string.IsNullOrWhiteSpace(order.SelfTakeCityCode))
                            order.SelfTakeCityName = ProvinceCityHelper.GetAreaNameByCode(order.SelfTakeCityCode);
                        if (!string.IsNullOrWhiteSpace(order.SelfTakeDistrictCode))
                            order.SelfTakeDistrictName = ProvinceCityHelper.GetAreaNameByCode(order.SelfTakeDistrictCode);
                    }

                    //优惠券与花费积分抵现金额 CouponValue SpendScoreCost
                    var orderPayDetail = OrderPayDetail.ObjectSet().Where(t => t.OrderId == order.CommodityOrderId).ToList();
                    if (orderPayDetail.Count > 0)
                    {
                        var couponValue = orderPayDetail.Where(t => t.OrderId == order.CommodityOrderId && t.ObjectType == 1).Select(t => t.Amount).FirstOrDefault();
                        order.CouponValue = couponValue;
                        var spendScoreMoney = orderPayDetail.Where(t => t.OrderId == order.CommodityOrderId && t.ObjectType == 2).Select(t => t.Amount).FirstOrDefault();
                        order.SpendScoreMoney = spendScoreMoney;
                    }

                    // 查询易捷抵现金额 
                    var yjInfo = YJBSV.GetOrderInfo(order.EsAppId, order.CommodityOrderId);
                    if (yjInfo.IsSuccess)
                    {
                        if (yjInfo.Data.YJBInfo != null)
                        {
                            order.SpendYJBMoney = yjInfo.Data.YJBInfo.InsteadCashAmount;
                            order.RefundYJBMoney = yjInfo.Data.YJBInfo.RefundCashAmount;
                        }
                        if (yjInfo.Data.YJCouponInfo != null)
                        {
                            order.SpendYJCouponMoney = yjInfo.Data.YJCouponInfo.InsteadCashAmount;
                        }
                    }

                    CommodityCategory cc = new CommodityCategory();
                    var orderItems = (from data in OrderItem.ObjectSet()
                                          //join data1 in Commodity.ObjectSet() on data.CommodityId equals data1.Id
                                      where data.CommodityOrderId == order.CommodityOrderId
                                      select new
                                      {
                                          Id = data.Id,
                                          CommodityOrderId = data.CommodityOrderId,
                                          CommodityId = data.CommodityId,
                                          CommodityIdName = data.Name,
                                          PicturesPath = data.PicturesPath,
                                          Price = data.CurrentPrice,//取订单商品列表中的价格
                                          Number = data.Number,
                                          CommodityAttributes = data.CommodityAttributes,
                                          CategoryName = data.CategoryNames,
                                          RealPrice = data.RealPrice == 0 ? data.DiscountPrice : data.RealPrice,
                                          PromotionDesc = data.PromotionDesc,
                                          Duty = data.Duty,
                                          SnCode = data.SNCode,
                                          JdCode = data.JDCode
                                      }).ToList();
                    List<OrderItemsVM> orderItemslist = (from data in orderItems
                                                         select new OrderItemsVM
                                                         {
                                                             Id = data.Id,
                                                             CommodityOrderId = data.CommodityOrderId,
                                                             CommodityId = data.CommodityId,
                                                             CommodityIdName = data.CommodityIdName,
                                                             PicturesPath = data.PicturesPath,
                                                             Price = data.Price,//取订单商品列表中的价格
                                                             Number = data.Number,
                                                             SizeAndColorId = data.CommodityAttributes,
                                                             CommodityCategorys =
                                                             data.CategoryName == null ? new List<string>() : data.CategoryName.Split(',').ToList(),
                                                             RealPrice = data.RealPrice,
                                                             PromotionDesc = data.PromotionDesc,
                                                             Duty = data.Duty,
                                                             JdfhTime = DateTime.Now,
                                                             JdorderId = null,
                                                             SNCode = data.SnCode,
                                                             JdCode = data.JdCode
                                                         }).ToList();
                    order.OrderItems = orderItemslist;

                    // 获取赠品信息
                    var presents = OrderItemPresent.ObjectSet().Where(_ => _.CommodityOrderId == order.CommodityOrderId).Select(data =>
                        new
                        {
                            OrderItemId = data.OrderItemId,
                            CommodityOrderId = data.CommodityOrderId,
                            CommodityId = data.CommodityId,
                            CommodityIdName = data.Name,
                            PicturesPath = data.PicturesPath,
                            Price = data.CurrentPrice,//取订单商品列表中的价格
                            Number = data.Number,
                            CommodityAttributes = data.CommodityAttributes,
                            CategoryName = data.CategoryNames,
                            RealPrice = data.RealPrice,
                            PromotionDesc = data.PromotionDesc,
                            Duty = data.Duty
                        }
                    ).ToList();

                    foreach (var item in orderItemslist)
                    {
                        string commodityorderid = item.CommodityOrderId.ToString();
                        var jdorderItem = JdOrderItem.ObjectSet().Where(p => p.CommodityOrderId == commodityorderid).ToList();
                        //对JdOrderId进行去重处理
                        jdorderItem = jdorderItem.GroupBy(p => p.JdOrderId).Select(p => p.OrderByDescending(t => t.SubTime).FirstOrDefault()).ToList();
                        if (jdorderItem.Count() > 0)
                        {
                            foreach (var _item in jdorderItem)
                            {
                                var selectJdOrder = JdHelper.selectJdOrder1(_item.JdOrderId);
                                if (!string.IsNullOrEmpty(selectJdOrder))
                                {
                                    JObject objwlgs = JObject.Parse(selectJdOrder);
                                    if (objwlgs != null && objwlgs["sku"] != null)
                                    {
                                        JArray objson = JArray.Parse(Convert.ToString(objwlgs["sku"]));
                                        foreach (var ZiJdOrder in objson)
                                        {
                                            var commodity = Commodity.ObjectSet().FirstOrDefault(p => p.Id == item.CommodityId);
                                            if (commodity != null)
                                            {
                                                if (commodity.JDCode == ZiJdOrder["skuId"].ToString())
                                                {
                                                    item.JdorderId = _item.JdOrderId;
                                                    item.JdfhTime = _item.ModifiedOn;
                                                }
                                            }

                                        }
                                    }
                                }
                            }
                        }

                        #region 赠品信息
                        var currentPresents = presents.Where(_ => _.OrderItemId == item.Id).ToList();
                        if (currentPresents.Count > 0)
                        {
                            item.Presents = new List<OrderItemPresentsVM>();
                            foreach (var p in currentPresents)
                            {
                                item.Presents.Add(new OrderItemPresentsVM
                                {
                                    CommodityName = p.CommodityIdName,
                                    Number = p.Number,
                                    PicturesPath = p.PicturesPath,
                                    Price = p.Price,
                                    RealPrice = p.RealPrice ?? 0,
                                    SizeAndColorId = p.CommodityAttributes,
                                });
                            }
                        }
                        #endregion
                    }
                    if (order.State != 0 && order.State != 1 && order.State != 8 && order.State != 4 && order.State != 5 &&
                        order.State != 6)
                    {
                        var orgShip = OrderShipping.ObjectSet().Where(c => c.OrderId == id).OrderBy(c => c.SubTime).FirstOrDefault();
                        if (orgShip != null)
                        {
                            order.OrgShipExpCo = orgShip.ShipExpCo;
                            order.OrgExpOrderNo = orgShip.ExpOrderNo;
                        }
                        var orgSip = (from data in OrderShipping.ObjectSet()
                                      where data.OrderId == id
                                      select data).Count();
                        order.Wlsl = orgSip;
                    }
                    var tuple = CBCSV.GetUserNameAndCode(order.UserId);
                    order.Uname = tuple.Item1;
                    order.Ucode = tuple.Item2;

                    #region 设置规格信息
                    var orderItem = OrderItem.ObjectSet().FirstOrDefault(p => p.CommodityOrderId == order.CommodityOrderId);
                    if (orderItem != null)
                    {
                        order.Specifications = orderItem.Specifications ?? 0;
                    }
                    #endregion

                    //获取中石化的电子发票信息 IsRefund = false为没有申请退款
                    HTJSInvoice htjs = HTJSInvoice.ObjectSet().FirstOrDefault(t =>
                                    (t.SwNo == "jh" + order.CommodityOrderCode || t.SwNo == "tk" + order.CommodityOrderCode || t.SwNo == "pr" + order.CommodityOrderCode) && t.FMsgCode == "0000");
                    if (htjs != null)
                    {
                        order.SwNo = htjs.SwNo;
                        order.IsRefund = htjs.RefundType == 1;
                    }
                }
                return order;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("订单详情异常。id：{0}", id), ex);
                return null;
            }
        }

        /// <summary>
        /// 获得商家所有订单
        /// <para>tips: 2018-04-13 张剑 返回值中增加返油卡兑换券的信息</para>
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderListDTO GetAllCommodityOrderByAppIdExt(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderSearchDTO search)
        {
            var result = new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderListDTO();
            if (search == null)
            {
                result.Data = new List<CommodityOrderVM>();
                return result;
            }

            OrderItem ot = new OrderItem();

            IQueryable<CommodityOrderVM> query = null;

            var youkaMoneyQuery = from oi in OrderItem.ObjectSet()
                                  group oi by oi.CommodityOrderId into oiGroup
                                  select new
                                  {
                                      Key = oiGroup.Key,
                                      ReturnYoukaMoney = oiGroup.Sum(selector => Math.Round((selector.RealPrice ?? 0) * selector.Number * (selector.YouKaPercent ?? 0) / 100, 2))
                                  };

            if (search.AppId == Guid.Empty && search.AppIds != null)
            {
                query = from data in CommodityOrder.ObjectSet()
                        join cf in CfOrderDividend.ObjectSet()
                        on data.Id equals cf.CommodityOrderId into cfOrderDividends
                        from cfOrderDividend in cfOrderDividends.DefaultIfEmpty()
                        join dataS in CommodityOrderService.ObjectSet()
                        on data.Id equals dataS.Id
                        into tempS
                        from tbS in tempS.DefaultIfEmpty()
                        join opu in AppOrderPickUp.ObjectSet()
                        on data.Id equals opu.Id into opuOrderPickUps
                        from opuOrderPickUp in opuOrderPickUps.DefaultIfEmpty()
                        join oi in youkaMoneyQuery on data.Id equals oi.Key
                        where search.AppIds.Contains(data.AppId) && data.State != 16 && data.State != 17 && data.State != -1 && data.IsDel != 2 && data.IsDel != 3
                        select new CommodityOrderVM
                        {
                            Id = data.Id,
                            AppId = data.AppId,
                            UserId = data.UserId,
                            CommodityOrderId = data.Id,
                            CurrentPrice = data.RealPrice,
                            Details = data.Details,
                            State = data.State,
                            CommodityOrderCode = data.Code,
                            SubTime = data.SubTime,
                            ReceiptUserName = data.ReceiptUserName,
                            Payment = data.Payment,
                            PaymentTime = data.PaymentTime,
                            ShipmentsTime = data.ShipmentsTime,
                            ConfirmTime = data.ConfirmTime,
                            ModifiedOn = data.ModifiedOn,
                            Price = data.Price,
                            MessageToBuyer = data.MessageToBuyer,
                            IsModifiedPrice = data.IsModifiedPrice,
                            ReceiptUserId = data.SubId,
                            ReceiptPhone = data.ReceiptPhone,
                            ReceiptAddress = data.ReceiptAddress,
                            Province = data.Province,
                            District = data.District,
                            City = data.City,
                            Street = data.Street,
                            RefundExpCo = "",
                            RefundTime = data.RefundTime,
                            AgreementTime = data.AgreementTime,
                            Freight = data.Freight,
                            Commission = data.Commission,
                            SellersRemark = data.SellersRemark,
                            CfDividend = cfOrderDividend.Gold / 1000.0m,
                            IsCrowdfunding = data.IsCrowdfunding != 0,
                            SelfTakeFlag = data.SelfTakeFlag,
                            SpreadGold = data.SpreadGold,
                            RefundMoney = 0,
                            RefundType = 0,
                            RefundExpOrderNo = "",
                            IsDelayConfirmTime = data.IsDelayConfirmTime,
                            IsDelayConfirmTimeAfterSales = tbS.IsDelayConfirmTimeAfterSales == null ? false : tbS.IsDelayConfirmTimeAfterSales,
                            OwnerShare = data.OwnerShare,
                            SelfTakeProvinceCode = opuOrderPickUp.StsProvince,
                            SelfTakeCityCode = opuOrderPickUp.StsCity,
                            SelfTakeDistrictCode = opuOrderPickUp.StsDistrict,
                            SelfTakeAddress = opuOrderPickUp.StsAddress,
                            StateRefund = -1,
                            StateAfterSales = tbS.State == null ? -1 : tbS.State,
                            StateRefundAfterSales = -1,
                            EndTime = tbS.EndTime == null ? null : tbS.EndTime,
                            ModifiedOnServiceAfterSales = tbS.ModifiedOn == null ? DateTime.Now : tbS.ModifiedOn,
                            OrderType = data.OrderType,
                            DistributorId = data.DistributorId,
                            DistributeMoney = data.DistributeMoney,
                            EsAppId = data.EsAppId,
                            PicturesPath = data.PicturesPath,
                            ChannelShareMoney = data.ChannelShareMoney,
                            Batch = data.Batch,
                            GoldCoupon = data.GoldCoupon,
                            GoldPrice = data.GoldPrice,
                            ExpressPrintCount = data.ExpressPrintCount,
                            InvoicePrintCount = data.InvoicePrintCount,
                            CancelReasonCode = data.CancelReasonCode,
                            SetMealId = data.SetMealId == null ? Guid.Empty : (Guid)data.SetMealId,
                            Specifications = 0,
                            ReturnYoukaMoney = oi.ReturnYoukaMoney,
                            YJCardPrice = data.YJCardPrice
                        };
            }
            if (search.AppId != Guid.Empty && search.AppIds == null)
            {
                query = from data in CommodityOrder.ObjectSet()
                        join cf in CfOrderDividend.ObjectSet()
                        on data.Id equals cf.CommodityOrderId into cfOrderDividends
                        from cfOrderDividend in cfOrderDividends.DefaultIfEmpty()
                        join dataS in CommodityOrderService.ObjectSet()
                        on data.Id equals dataS.Id
                        into tempS
                        from tbS in tempS.DefaultIfEmpty()
                        join opu in AppOrderPickUp.ObjectSet()
                        on data.Id equals opu.Id into opuOrderPickUps
                        from opuOrderPickUp in opuOrderPickUps.DefaultIfEmpty()
                        join oi in youkaMoneyQuery on data.Id equals oi.Key
                        where data.AppId == search.AppId && data.State != 16 && data.State != 17 && data.IsDel != 2 && data.IsDel != 3
                        select new CommodityOrderVM
                        {
                            Id = data.Id,
                            AppId = search.AppId,
                            UserId = data.UserId,
                            CommodityOrderId = data.Id,
                            CurrentPrice = data.RealPrice,
                            Details = data.Details,
                            State = data.State,
                            CommodityOrderCode = data.Code,
                            SubTime = data.SubTime,
                            ReceiptUserName = data.ReceiptUserName,
                            Payment = data.Payment,
                            PaymentTime = data.PaymentTime,
                            ShipmentsTime = data.ShipmentsTime,
                            ConfirmTime = data.ConfirmTime,
                            ModifiedOn = data.ModifiedOn,
                            Price = data.Price,
                            MessageToBuyer = data.MessageToBuyer,
                            IsModifiedPrice = data.IsModifiedPrice,
                            ReceiptUserId = data.SubId,
                            ReceiptPhone = data.ReceiptPhone,
                            ReceiptAddress = data.ReceiptAddress,
                            Province = data.Province,
                            District = data.District,
                            City = data.City,
                            Street = data.Street,
                            RefundExpCo = "",
                            RefundTime = data.RefundTime,
                            AgreementTime = data.AgreementTime,
                            Freight = data.Freight,
                            Commission = data.Commission,
                            SellersRemark = data.SellersRemark,
                            CfDividend = cfOrderDividend.Gold / 1000.0m,
                            IsCrowdfunding = data.IsCrowdfunding != 0,
                            SelfTakeFlag = data.SelfTakeFlag,
                            SpreadGold = data.SpreadGold,
                            RefundMoney = 0,
                            RefundType = 0,
                            RefundExpOrderNo = "",
                            IsDelayConfirmTime = data.IsDelayConfirmTime,
                            IsDelayConfirmTimeAfterSales = tbS.IsDelayConfirmTimeAfterSales == null ? false : tbS.IsDelayConfirmTimeAfterSales,
                            OwnerShare = data.OwnerShare,
                            SelfTakeProvinceCode = opuOrderPickUp.StsProvince,
                            SelfTakeCityCode = opuOrderPickUp.StsCity,
                            SelfTakeDistrictCode = opuOrderPickUp.StsDistrict,
                            SelfTakeAddress = opuOrderPickUp.StsAddress,
                            StateRefund = -1,
                            StateAfterSales = tbS.State == null ? -1 : tbS.State,
                            StateRefundAfterSales = -1,
                            EndTime = tbS.EndTime == null ? null : tbS.EndTime,
                            ModifiedOnServiceAfterSales = tbS.ModifiedOn == null ? DateTime.Now : tbS.ModifiedOn,
                            OrderType = data.OrderType,
                            DistributorId = data.DistributorId,
                            DistributeMoney = data.DistributeMoney,
                            EsAppId = data.EsAppId,
                            PicturesPath = data.PicturesPath,
                            ChannelShareMoney = data.ChannelShareMoney,
                            Batch = data.Batch,
                            GoldCoupon = data.GoldCoupon,
                            GoldPrice = data.GoldPrice,
                            ExpressPrintCount = data.ExpressPrintCount,
                            InvoicePrintCount = data.InvoicePrintCount,
                            CancelReasonCode = data.CancelReasonCode,
                            SetMealId = data.SetMealId == null ? Guid.Empty : (Guid)data.SetMealId,
                            ReturnYoukaMoney = oi.ReturnYoukaMoney,
                            YJCardPrice = data.YJCardPrice
                        };
            }





            if (search.PriceLow != "-1" && search.PriceLow != null && search.PriceLow != "")
            {
                decimal _priceLow = decimal.Parse(search.PriceLow);
                query = query.Where(n => n.CurrentPrice >= _priceLow);
                //countquery = countquery.Where(n => n.RealPrice >= _priceLow);
            }
            if (search.PriceHight != "-1" && search.PriceHight != null && search.PriceHight != "")
            {
                decimal _priceHight = decimal.Parse(search.PriceHight);
                query = query.Where(n => n.CurrentPrice <= _priceHight);
                //countquery = countquery.Where(n => n.RealPrice <= _priceHight);
            }
            if (search.SeacrhContent != null && !string.IsNullOrEmpty(search.SeacrhContent))
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(search.SeacrhContent, "^[0-9]+$"))
                {
                    query = query.Where(n => n.CommodityOrderCode.Contains(search.SeacrhContent) || n.ReceiptPhone.Contains(search.SeacrhContent));
                }
                else
                {
                    query = query.Where(n => n.ReceiptUserName.Contains(search.SeacrhContent));
                }
            }
            if (search.DayCount != null && search.DayCount != "0" && search.DayCount != "")
            {
                switch (int.Parse(search.DayCount))
                {
                    case 0:
                        break;
                    default:
                        int _dayCount = int.Parse(search.DayCount);
                        DateTime date = DateTime.Now.AddDays(-_dayCount);//减去多少天
                        query = query.Where(n => n.SubTime.Value >= date);
                        //countquery = countquery.Where(n => n.SubTime >= date);
                        break;
                }

            }
            if (search.StartOrderTime.HasValue)
            {
                if (!search.EndOrderTime.HasValue)
                {
                    search.EndOrderTime = DateTime.Now;
                }
                DateTime start = search.StartOrderTime.Value.Date;
                DateTime end = search.EndOrderTime.Value.Date.AddDays(1);
                query = query.Where(n => n.SubTime >= start && n.SubTime < end);
            }
            if (search.State != null)
            {
                if (search.State.Contains(","))
                {
                    if (search.State == "8,9,10,12,14")   //退款中
                    {
                        var allCommodityOrderIds = query.Select(t => t.Id).Distinct();
                        var allquery = query;
                        List<int> beforeState = new List<int>() { 8, 9, 10, 12, 14 };
                        List<int> afterState = new List<int>() { 5, 10, 12 };
                        query = query.Where(n => beforeState.Contains(n.State) || afterState.Contains(n.StateAfterSales));

                        //获取单商品退款中 但是订单不是退款中的订单
                        var calCommodityOrderIds = query.Select(t => t.Id).Distinct();
                        var query1 = OrderItem.ObjectSet()
                                .Where(t => t.State == 1 && allCommodityOrderIds.Contains(t.CommodityOrderId) && !calCommodityOrderIds.Contains(t.CommodityOrderId))
                                .Select(t => t.CommodityOrderId)
                                .Distinct();

                        query = query.Concat(allquery.Where(t => query1.Contains(t.Id)));
                    }
                    else
                    {
                        int[] arrystate = Array.ConvertAll<string, int>(search.State.Split(','), s => int.Parse(s));

                        //等发货且自提
                        if (arrystate.Contains(1) && arrystate.Contains(99))
                        {
                            int[] exceptTmp = new int[] { 99 };
                            int[] arrystateTmp = arrystate.Except(exceptTmp).ToArray();
                            query = query.Where(a => arrystateTmp.Contains(a.State));
                        }
                        else if (arrystate.Contains(1))
                        {
                            if (arrystate.Contains(11))
                            {
                                int[] exceptTmp = new int[] { 1, 11 };
                                int[] arrystateTmp = arrystate.Except(exceptTmp).ToArray();
                                query = query.Where(a => arrystateTmp.Contains(a.State) || ((a.State == 1 || a.State == 11) && a.SelfTakeFlag == 0));
                            }
                            else
                            {
                                int[] exceptTmp = new int[] { 1 };
                                int[] arrystateTmp = arrystate.Except(exceptTmp).ToArray();
                                query = query.Where(a => arrystateTmp.Contains(a.State) || (a.State == 1 && a.SelfTakeFlag == 0));
                            }
                        }
                        else if (arrystate.Contains(99))
                        {
                            int[] exceptTmp = new int[] { 99 };
                            int[] arrystateTmp = arrystate.Except(exceptTmp).ToArray();
                            query = query.Where(a => arrystateTmp.Contains(a.State) || ((a.State == 1 || a.State == 11) && a.SelfTakeFlag == 1));
                        }
                        else
                        {
                            query = query.Where(a => arrystate.Contains(a.State));
                        }
                    }
                }
                else
                {
                    if (search.State != "-1" && search.State != null && search.State != "")
                    {
                        int _state = int.Parse(search.State);
                        //待发货的
                        if (_state == 1)
                        {
                            query = query.Where(n => n.State == _state && n.SelfTakeFlag == 0);
                        }
                        //待自提的
                        else if (_state == 99)
                        {
                            query = query.Where(n => (n.State == 1 || n.State == 11) && n.SelfTakeFlag == 1);
                        }
                        else if (search.State == "3") //交易成功
                        {
                            query = query.Where(n => n.State == 3 && (n.StateAfterSales == 3 || n.StateAfterSales == 15 || n.StateAfterSales == -1));
                        }
                        else if (search.State == "7")
                        {
                            var allCommodityOrderIds = query.Select(t => t.Id).Distinct();
                            var allquery = query;
                            query = query.Where(n => n.State == 7 || n.StateAfterSales == 7);

                            //获取单商品退款中 但是订单不是退款中的订单
                            var calCommodityOrderIds = query.Select(t => t.Id).Distinct();
                            var query1 = OrderItem.ObjectSet()
                                    .Where(t => t.State == 2 && allCommodityOrderIds.Contains(t.CommodityOrderId) && !calCommodityOrderIds.Contains(t.CommodityOrderId))
                                    .Select(t => t.CommodityOrderId)
                                    .Distinct();

                            query = query.Concat(allquery.Where(t => query1.Contains(t.Id)));
                        }
                        else
                        {
                            query = query.Where(n => n.State == _state);
                        }
                        //countquery = countquery.Where(n => n.State == _state);
                    }
                }
            }
            if (search.Payment != "-1" && search.Payment != null && search.Payment != "")
            {
                query = query.Where(n => n.State > 0);
                if (search.Payment.Contains(","))
                {
                    List<string> pmStrList = search.Payment.Split(",，".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                    List<int> pmIntList = pmStrList.ConvertAll(strPm => Convert.ToInt32(strPm));
                    query = query.Where(pm => pmIntList.Contains(pm.Payment));
                }
                else
                {
                    int _payment = int.Parse(search.Payment);
                    query = query.Where(n => n.Payment == _payment);// && n.State != 0 && n.State != 4);
                }

                //countquery = countquery.Where(n => n.Payment == _payment && n.State != 0 && n.State != 4);
            }
            if (search.EsAppId.HasValue && search.EsAppId.Value != Guid.Empty)
            {
                var directArrival = PaySource.GetPaymentByTradeType(1);
                if (search.EsAppId.Value == search.AppId)
                {
                    query = query.Where(n => n.EsAppId == search.EsAppId.Value || (n.EsAppId != search.EsAppId.Value && !directArrival.Contains(n.Payment)));
                }
                else
                {
                    query = query.Where(n => n.EsAppId == search.EsAppId.Value && !directArrival.Contains(n.Payment));// && n.State != 0 && n.State != 4);
                }
            }

            if (search.OrderSourceId.HasValue && search.OrderSourceId.Value != Guid.Empty)
            {
                query = query.Where(n => n.EsAppId == search.OrderSourceId.Value);
            }

            if (search.LastPayTime.HasValue)
            {
                query = query.Where(c => c.PaymentTime > search.LastPayTime);
            }
            query = query.Distinct();
            result.Count = query.Count();

            //return query.OrderByDescending(n => n.SubTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            var searchResult = query.OrderByDescending(n => n.ModifiedOn).ThenByDescending(n => n.SubTime).Skip((search.PageIndex - 1) * search.PageSize).Take(search.PageSize).ToList();

            //自提点省市附值
            foreach (var item in searchResult)
            {
                if (!string.IsNullOrWhiteSpace(item.SelfTakeProvinceCode))
                    item.SelfTakeProvinceName = ProvinceCityHelper.GetAreaNameByCode(item.SelfTakeProvinceCode);
                if (!string.IsNullOrWhiteSpace(item.SelfTakeCityCode))
                    item.SelfTakeCityName = ProvinceCityHelper.GetAreaNameByCode(item.SelfTakeCityCode);
                if (!string.IsNullOrWhiteSpace(item.SelfTakeDistrictCode))
                    item.SelfTakeDistrictName = ProvinceCityHelper.GetAreaNameByCode(item.SelfTakeDistrictCode);
            }

            //此操作与下面相同，性能太差，不再采用
            //foreach (var item in result)
            //{
            //    item.OrderItems = ot.SelectOrderItemsByOrderId(item.CommodityOrderId, appId);
            //}

            //订单ID
            var idList = searchResult.Select(t => t.CommodityOrderId).ToList();

            //取出售中退款信息
            var middle = (from o in OrderRefund.ObjectSet()
                          where idList.Contains(o.OrderId) && (o.OrderItemId == null || o.OrderItemId == Guid.Empty)
                          select new OrderRefundTmp
                          {
                              OrderId = o.OrderId,
                              RefundMoney = o.RefundMoney,
                              RefundType = o.RefundType,
                              SubTime = o.SubTime,
                              RefundExpOrderNo = o.RefundExpOrderNo,
                              RefundExpCo = o.RefundExpCo,
                              StateRefund = o.State,
                              RefuseTime = o.RefuseTime,
                              RefundScoreMoney = o.RefundScoreMoney,
                              OrderItemId = Guid.Empty,
                              RefundYJBMoney = o.RefundYJBMoney
                          }).GroupBy(t => t.OrderId).ToDictionary(x => x.Key, y => y.OrderByDescending(t => t.SubTime).FirstOrDefault());

            //售中订单ID
            var idMiddleList = middle.Select(t => t.Value.OrderId).ToList();
            if (idMiddleList.Count > 0)
            {
                foreach (var item in searchResult)
                {
                    var modelMiddle = middle.Where(t => t.Value.OrderId == item.CommodityOrderId).FirstOrDefault();
                    if (modelMiddle.Value != null)
                    {
                        item.RefundMoney = modelMiddle.Value.RefundMoney;
                        item.RefundType = modelMiddle.Value.RefundType;
                        item.RefundExpOrderNo = modelMiddle.Value.RefundExpOrderNo;
                        item.RefundExpCo = modelMiddle.Value.RefundExpCo;
                        item.StateRefund = modelMiddle.Value.StateRefund;
                        item.RefundScoreMoney = modelMiddle.Value.RefundScoreMoney;
                        item.RefundYJBMoney = modelMiddle.Value.RefundYJBMoney;
                        item.OrderItemId = modelMiddle.Value.OrderItemId;
                    }
                }
            }

            //取出售后退款信息
            var afterSales = (from o in OrderRefundAfterSales.ObjectSet()
                              where idList.Contains(o.OrderId) && (o.OrderItemId == null || o.OrderItemId == Guid.Empty)
                              select new OrderRefundTmp
                              {
                                  OrderId = o.OrderId,
                                  RefundMoney = o.RefundMoney,
                                  RefundType = o.RefundType,
                                  SubTime = o.SubTime,
                                  RefundExpOrderNo = o.RefundExpOrderNo,
                                  RefundExpCo = o.RefundExpCo,
                                  StateRefundAfterSales = o.State,
                                  RefuseTime = o.RefuseTime,
                                  RefundScoreMoney = o.RefundScoreMoney,
                                  OrderItemId = Guid.Empty,
                                  RefundYJBMoney = o.RefundYJBMoney
                              }).GroupBy(t => t.OrderId).ToDictionary(x => x.Key, y => y.OrderByDescending(t => t.SubTime).FirstOrDefault());

            //售后订单ID
            var idAfterSalesList = afterSales.Select(t => t.Value.OrderId).ToList();
            if (idAfterSalesList.Count > 0)
            {
                foreach (var item in searchResult)
                {
                    //可售后
                    if (item.State == 3)
                    {
                        var modelAfterSales = afterSales.Where(t => t.Value.OrderId == item.CommodityOrderId).FirstOrDefault();
                        if (modelAfterSales.Value != null)
                        {
                            item.RefundMoney = modelAfterSales.Value.RefundMoney;
                            item.RefundType = modelAfterSales.Value.RefundType;
                            item.RefundExpOrderNo = modelAfterSales.Value.RefundExpOrderNo;
                            item.RefundExpCo = modelAfterSales.Value.RefundExpCo;
                            item.StateRefundAfterSales = modelAfterSales.Value.StateRefundAfterSales;
                            item.RefundTime = modelAfterSales.Value.SubTime;
                            item.AgreementTime = modelAfterSales.Value.RefuseTime;
                            item.RefundScoreMoney = modelAfterSales.Value.RefundScoreMoney;
                            item.RefundYJBMoney = modelAfterSales.Value.RefundYJBMoney;
                            item.OrderItemId = modelAfterSales.Value.OrderItemId;
                        }
                        else
                        {
                            item.StateRefundAfterSales = -1;
                        }
                    }
                    else
                    {
                        item.StateAfterSales = -1;
                        item.StateRefundAfterSales = -1;
                    }
                }
            }

            foreach (var item in searchResult)
            {
                var norderItemIds = OrderRefund.ObjectSet().Where(t => t.OrderId == item.CommodityOrderId).Select(t => t.OrderItemId).Distinct();
                if (norderItemIds.Count() > 0 && norderItemIds.First() != null)
                {
                    decimal totRefundMoney = 0;
                    decimal totRefundScoreMoney = 0;
                    decimal totRefundYJBMoney = 0;
                    foreach (var orderItemId in norderItemIds)
                    {
                        var a = OrderRefund.ObjectSet().Where(t => t.OrderItemId == orderItemId).OrderByDescending(t => t.SubTime).FirstOrDefault();
                        if (a != null)
                        {
                            totRefundMoney += a.RefundMoney;
                            totRefundScoreMoney += a.RefundScoreMoney;
                            totRefundYJBMoney += a.RefundYJBMoney;
                        }
                    }
                    item.RefundMoney = totRefundMoney;
                    item.RefundScoreMoney = totRefundScoreMoney;
                    item.RefundYJBMoney = totRefundYJBMoney;
                    item.OrderItemId = (Guid)norderItemIds.First();
                }
                else
                {
                    norderItemIds = OrderRefundAfterSales.ObjectSet().Where(t => t.OrderId == item.CommodityOrderId).Select(t => t.OrderItemId).Distinct();
                    if (norderItemIds.Count() > 0 && norderItemIds.First() != null)
                    {
                        decimal totRefundMoney = 0;
                        decimal totRefundScoreMoney = 0;
                        decimal totRefundYJBMoney = 0;
                        foreach (var orderItemId in norderItemIds)
                        {
                            var a = OrderRefundAfterSales.ObjectSet().Where(t => t.OrderItemId == orderItemId).OrderByDescending(t => t.SubTime).FirstOrDefault();
                            if (a != null)
                            {
                                totRefundMoney += a.RefundMoney;
                                totRefundScoreMoney += a.RefundScoreMoney;
                                totRefundYJBMoney += a.RefundYJBMoney;
                            }
                        }
                        item.RefundMoney = totRefundMoney;
                        item.RefundScoreMoney = totRefundScoreMoney;
                        item.RefundYJBMoney = totRefundYJBMoney;
                        item.OrderItemId = (Guid)norderItemIds.First();
                    }
                }
            }

            //花费积分抵现金额 SpendScoreCost
            var orderPayDetail = OrderPayDetail.ObjectSet().Where(t => idList.Contains(t.OrderId)).ToList();
            if (orderPayDetail.Count > 0)
            {
                foreach (var item in searchResult)
                {
                    var spendScoreMoney = orderPayDetail.Where(t => t.OrderId == item.CommodityOrderId && t.ObjectType == 2).Select(t => t.Amount).FirstOrDefault();
                    item.SpendScoreMoney = spendScoreMoney;
                    var couponMoney = orderPayDetail.Where(t => t.OrderId == item.CommodityOrderId && t.ObjectType == 1).Select(t => t.Amount).FirstOrDefault();
                    item.CouponValue = couponMoney;
                }
            }

            // 查询易捷币抵现金额
            var orderIds = searchResult.Where(s => s.EsAppId == YJB.Deploy.CustomDTO.YJBConsts.YJAppId).Select(t => t.CommodityOrderId).Distinct().ToList();
            if (orderIds.Count > 0)
            {
                var yjInfoes = YJBSV.GetOrderInfoes(orderIds);
                if (yjInfoes.IsSuccess)
                {
                    foreach (var item in searchResult)
                    {
                        var yjInfo = yjInfoes.Data.Find(y => y.OrderId == item.CommodityOrderId);
                        if (yjInfo != null)
                        {
                            if (yjInfo.YJBInfo != null) item.SpendYJBMoney = yjInfo.YJBInfo.InsteadCashAmount;
                            //item.RefundYJBMoney = yjbInfo.RefundCashAmount;
                            if (yjInfo.YJCouponInfo != null) item.SpendYJCouponMoney = yjInfo.YJCouponInfo.InsteadCashAmount;
                        }
                    }
                }
            }


            #region 查询订单商品信息(优化)

            //构建订单id数组
            List<Guid> commodityOrderIdList = new List<Guid>();
            for (int i = 0; i < searchResult.Count; i++)
            {
                commodityOrderIdList.Add(searchResult[i].CommodityOrderId);
            }

            CommodityCategory cc = new CommodityCategory();

            //取出所有订单的所有商品
            var orderItems = (from data in OrderItem.ObjectSet()
                              join data1 in Commodity.ObjectSet() on data.CommodityId equals data1.Id
                              where commodityOrderIdList.Contains(data.CommodityOrderId)
                              select new
                              {
                                  Id = data.Id,
                                  CommodityOrderId = data.CommodityOrderId,
                                  CommodityId = data.CommodityId,
                                  CommodityIdName = data.Name,
                                  PicturesPath = data.PicturesPath,
                                  Price = data.CurrentPrice,//取订单商品列表中的价格
                                  Number = data.Number,
                                  CommodityAttributes = data.CommodityAttributes,
                                  CategoryName = data.CategoryNames,
                                  RealPrice = data.RealPrice,
                                  IsEnableSelfTake = data1.IsEnableSelfTake,
                                  ComCategoryName = data.ComCategoryName,
                                  data.State,
                                  data.RefundExpCo,
                                  data.RefundExpOrderNo,
                                  data.ScorePrice,
                                  data.YjbPrice,
                                  data.FreightPrice,
                                  data.CouponPrice
                              }).ToList();

            List<OrderItemsVM> orderItemsVMList = (from data in orderItems
                                                   select new OrderItemsVM
                                                   {
                                                       Id = data.Id,
                                                       CommodityOrderId = data.CommodityOrderId,
                                                       CommodityId = data.CommodityId,
                                                       CommodityIdName = data.CommodityIdName,
                                                       PicturesPath = data.PicturesPath,
                                                       Price = data.Price,//取订单商品列表中的价格
                                                       RealPrice = data.RealPrice,
                                                       Number = data.Number,
                                                       SizeAndColorId = data.CommodityAttributes,
                                                       CommodityCategorys =
                                                       data.CategoryName == null ? new List<string>() : data.CategoryName.Split(',').ToList(),
                                                       IsEnableSelfTake = data.IsEnableSelfTake,
                                                       ComCategoryName = data.ComCategoryName,
                                                       JdorderId = null,
                                                       State = data.State == null ? 0 : (int)data.State,
                                                       RefundExpCo = data.RefundExpCo,
                                                       RefundExpOrderNo = data.RefundExpOrderNo,
                                                       ScorePrice = data.ScorePrice,
                                                       YjbPrice = data.YjbPrice == null ? 0 : (decimal)data.YjbPrice,
                                                       FreightPrice = data.FreightPrice == null ? 0 : (decimal)data.FreightPrice,
                                                       CouponPrice = data.CouponPrice == null ? 0 : (decimal)data.CouponPrice
                                                   }).ToList();

            // 获取赠品信息
            var presents = OrderItemPresent.ObjectSet().Where(_ => commodityOrderIdList.Contains(_.CommodityOrderId)).Select(data =>
                new
                {
                    OrderItemId = data.OrderItemId,
                    CommodityOrderId = data.CommodityOrderId,
                    CommodityId = data.CommodityId,
                    CommodityIdName = data.Name,
                    PicturesPath = data.PicturesPath,
                    Price = data.CurrentPrice,//取订单商品列表中的价格
                    Number = data.Number,
                    CommodityAttributes = data.CommodityAttributes,
                    CategoryName = data.CategoryNames,
                    RealPrice = data.RealPrice,
                    ComCategoryName = data.ComCategoryName
                }
            ).ToList();


            var tempOrderItemIds = orderItems.Select(_ => _.Id).ToList();
            var jdOrderItems = JdOrderItem.ObjectSet().Where(_ => tempOrderItemIds.Contains(_.CommodityOrderItemId.Value)).ToList();
            foreach (var item in orderItemsVMList)
            {
                var jdOrderItem = jdOrderItems.FirstOrDefault(_ => _.CommodityOrderItemId == item.Id);
                if (jdOrderItem != null)
                {
                    item.JdState = jdOrderItem.State;
                    item.JdorderId = jdOrderItem.JdOrderId;
                }
                //string commodityorderid = item.CommodityOrderId.ToString();
                //var jdorderItem = JdOrderItem.ObjectSet().Where(p => p.CommodityOrderId == commodityorderid).ToList();
                //if (jdorderItem.Count() > 0)
                //{
                //    foreach (var _item in jdorderItem)
                //    {
                //        if (item.CommodityId == _item.TempId)
                //        {
                //            item.JdorderId = _item.JdOrderId;
                //        }
                //    }
                //}
                #region 赠品信息
                var currentPresents = presents.Where(_ => _.OrderItemId == item.Id).ToList();
                if (currentPresents.Count > 0)
                {
                    item.Presents = new List<OrderItemPresentsVM>();
                    foreach (var p in currentPresents)
                    {
                        item.Presents.Add(new OrderItemPresentsVM
                        {
                            ComCategoryName = p.ComCategoryName,
                            CommodityName = p.CommodityIdName,
                            Number = p.Number,
                            PicturesPath = p.PicturesPath,
                            Price = p.Price,
                            RealPrice = p.RealPrice,
                            SizeAndColorId = p.CommodityAttributes,
                        });
                    }
                }
                #endregion

                OrderRefund orderRefund = OrderRefund.ObjectSet().Where(t => t.OrderItemId == item.Id).OrderByDescending(t => t.SubTime).FirstOrDefault();
                if (orderRefund != null)
                {
                    item.RefundOrderItemId = orderRefund.OrderItemId == null ? Guid.Empty : (Guid)orderRefund.OrderItemId;
                    item.StateRefund = orderRefund.State;
                    item.RefundType = orderRefund.RefundType;
                }
                else
                {
                    var orderRefundAfterSales = OrderRefundAfterSales.ObjectSet().Where(t => t.OrderItemId == item.Id).OrderByDescending(t => t.SubTime).FirstOrDefault();
                    if (orderRefundAfterSales != null)
                    {
                        item.RefundOrderItemId = orderRefundAfterSales.OrderItemId == null ? Guid.Empty : (Guid)orderRefundAfterSales.OrderItemId;
                        item.StateAfterSales = orderRefundAfterSales.State;
                        item.RefundType = orderRefundAfterSales.RefundType;
                    }
                }
            }
            Collection collect = new Collection();

            //遍历订单
            foreach (CommodityOrderVM v in searchResult)
            {
                List<OrderItemsVM> orderItemslist = new List<OrderItemsVM>();
                //遍历订单中的商品，获取每个商品对应的颜色、尺寸属性
                foreach (OrderItemsVM model in orderItemsVMList)
                {
                    if (model.CommodityOrderId == v.CommodityOrderId)
                    {
                        orderItemslist.Add(model);
                    }
                }
                v.OrderItems = orderItemslist;
            }

            #endregion

            // 转换取消订单类型
            foreach (var item in searchResult)
            {
                if (item.CancelReasonCode.HasValue)
                {
                    item.MessageToBuyer = TypeToStringHelper.CancleOrderReasonTypeToString(item.CancelReasonCode);
                }
            }

            //获取规格设置信息
            foreach (var item in searchResult)
            {
                var orderItem = OrderItem.ObjectSet().FirstOrDefault(p => p.CommodityOrderId == item.CommodityOrderId);
                if (orderItem != null)
                {
                    item.Specifications = orderItem.Specifications ?? 0;
                }
            }

            result.Data = searchResult;
            return result;
        }


        /// <summary>
        /// 获得商家所有订单（获取电商馆下所有订单）
        /// <para>tips: 2018-04-13 张剑 添加优惠活动筛选项</para>
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderListDTO> GetAllCommodityOrderByEsAppIdExt(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderSearchDTO search)
        {
            bool isEsAppId = false;
            LogHelper.Info(String.Format("GetAllCommodityOrderByEsAppIdExt：查询参数: {0}", JsonConvert.SerializeObject(search)));
            var result = new ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderListDTO>();
            result.Data = new CommodityOrderListDTO();
            if (search == null)
            {
                result.Data.Data = new List<CommodityOrderVM>();
                return result;
            }

            //Jinher.AMP.ZPH.Deploy.CustomDTO.QueryPavilionAppParam queryPavilionAppParam = new Jinher.AMP.ZPH.Deploy.CustomDTO.QueryPavilionAppParam
            //{
            //    Id = search.AppId,
            //    pageIndex = 1,
            //    pageSize = Int32.MaxValue
            //};
            //TPS.ZPHSVFacade zph = new TPS.ZPHSVFacade();

            var applist = MallApply.ObjectSet().Where(_ => _.State.Value == 2 && _.EsAppId == search.AppId).Select(_ => _.AppId);  //zph.GetPavilionApp(queryPavilionAppParam).Data.Select(t => t.appId);

            if (applist.Count() > 0)
            {
                //如果是馆 
                search.EsAppId = search.AppId;
                isEsAppId = true;
                LogHelper.Info(String.Format("GetAllCommodityOrderByEsAppIdExt：EsAppId: {0}", search.AppId));
            }
            


            var oquery = (search.Marketing == 1 || search.Marketing == 2) ?
                          (from data in CommodityOrder.ObjectSet().Where(x => (x.AppId == search.AppId || applist.Contains(x.AppId)) && x.State != 16 && x.State != 17)
                               //join cf in CfOrderDividend.ObjectSet()
                               //on data.Id equals cf.CommodityOrderId
                               //into cfOrderDividends
                               //from cfOrderDividend in cfOrderDividends.DefaultIfEmpty()
                           join dataS in CommodityOrderService.ObjectSet()
                           on data.Id equals dataS.Id
                           into tempS
                           from tbS in tempS.DefaultIfEmpty()
                               //join opu in AppOrderPickUp.ObjectSet()
                               //on data.Id equals opu.Id into opuOrderPickUps
                               //from opuOrderPickUp in opuOrderPickUps.DefaultIfEmpty()

                           join oi in
                              (from oi in OrderItem.ObjectSet().Where(x => search.Marketing == 1 || search.Marketing == 2)
                               group oi by oi.CommodityOrderId into oiGroup
                               select new
                               {
                                   Key = oiGroup.Key,
                                   ReturnYoukaMoney = oiGroup.Sum(selector => Math.Round((selector.RealPrice ?? 0) * selector.Number * (selector.YouKaPercent ?? 0) / 100, 2)),
                                   YJBMoney = oiGroup.Sum(selector => selector.YjbPrice),
                                   YJCouponPrice = oiGroup.Sum(s => s.YJCouponPrice)
                               }) on data.Id equals oi.Key

                           //where (applist.Contains(data.AppId) || data.AppId == search.AppId) && data.State != 16 && data.State != 17

                           //join oi in
                           //    (from oi in OrderItem.ObjectSet()
                           //     group oi by oi.CommodityOrderId into oiGroup
                           //     select new
                           //     {
                           //         Key = oiGroup.Key,
                           //         ReturnYoukaMoney = oiGroup.Sum(selector => Math.Round((selector.RealPrice ?? 0) * selector.Number * (selector.YouKaPercent ?? 0) / 100, 2)),
                           //         YJBMoney = oiGroup.Sum(selector => selector.YjbPrice)
                           //     }) on data.Id equals oi.Key
                           //where
                           //   (applist.Contains(data.AppId) || data.AppId == search.AppId)
                           //   && data.State != 16 && data.State != 17
                           //   && (search.Marketing == 1 ? oi.YJBMoney > 0 : (search.Marketing == 2 ? oi.ReturnYoukaMoney > 0 : true))

                           select new CommodityOrderVM
                           {
                               AppId = data.AppId,
                               UserId = data.UserId,
                               CommodityOrderId = data.Id,
                               CurrentPrice = data.RealPrice,
                               Details = data.Details,
                               State = data.State,
                               CommodityOrderCode = data.Code,
                               SubTime = data.SubTime,
                               ReceiptUserName = data.ReceiptUserName,
                               Payment = data.Payment,
                               PaymentTime = data.PaymentTime,
                               ShipmentsTime = data.ShipmentsTime,
                               ConfirmTime = data.ConfirmTime,
                               ModifiedOn = data.ModifiedOn,
                               Price = data.Price,
                               MessageToBuyer = data.MessageToBuyer,
                               IsModifiedPrice = data.IsModifiedPrice,
                               ReceiptPhone = data.ReceiptPhone,
                               ReceiptAddress = data.ReceiptAddress,
                               Province = data.Province,
                               District = data.District,
                               City = data.City,
                               Street = data.Street,
                               RefundExpCo = "",
                               RefundTime = data.RefundTime,
                               AgreementTime = data.AgreementTime,
                               Freight = data.Freight,
                               Commission = data.Commission,
                               SellersRemark = data.SellersRemark,
                               //CfDividend = cfOrderDividend.Gold / 1000.0m,
                               IsCrowdfunding = data.IsCrowdfunding != 0,
                               SelfTakeFlag = data.SelfTakeFlag,
                               SpreadGold = data.SpreadGold,
                               RefundMoney = 0,
                               RefundType = 0,
                               RefundExpOrderNo = "",
                               IsDelayConfirmTime = data.IsDelayConfirmTime,
                               IsDelayConfirmTimeAfterSales = tbS.IsDelayConfirmTimeAfterSales == null ? false : tbS.IsDelayConfirmTimeAfterSales,
                               OwnerShare = data.OwnerShare,
                               //SelfTakeProvinceCode = opuOrderPickUp.StsProvince,
                               //SelfTakeCityCode = opuOrderPickUp.StsCity,
                               //SelfTakeDistrictCode = opuOrderPickUp.StsDistrict,
                               //SelfTakeAddress = opuOrderPickUp.StsAddress,
                               StateRefund = -1,
                               StateAfterSales = tbS.State == null ? -1 : tbS.State,
                               StateRefundAfterSales = -1,
                               EndTime = tbS.EndTime == null ? null : tbS.EndTime,
                               ModifiedOnServiceAfterSales = tbS.ModifiedOn == null ? DateTime.Now : tbS.ModifiedOn,
                               OrderType = data.OrderType,
                               DistributorId = data.DistributorId,
                               DistributeMoney = data.DistributeMoney,
                               EsAppId = data.EsAppId,
                               PicturesPath = data.PicturesPath,
                               ChannelShareMoney = data.ChannelShareMoney,
                               Batch = data.Batch,
                               GoldCoupon = data.GoldCoupon,
                               GoldPrice = data.GoldPrice,
                               ExpressPrintCount = data.ExpressPrintCount,
                               InvoicePrintCount = data.InvoicePrintCount,
                               SetMealId = data.SetMealId == null ? Guid.Empty : (Guid)data.SetMealId,
                               Specifications = 0,
                               AppName = data.AppName,
                               ReturnYoukaMoney = oi.ReturnYoukaMoney,
                               SpendYJBMoney = oi.YJBMoney ?? 0,
                               SpendYJCouponMoney = oi.YJCouponPrice ?? 0,
                               JcActivityId = data.JcActivityId == null ? Guid.Empty : (Guid)data.JcActivityId,
                               CustomerInfo = data.CustomerInfo == null ? Guid.Empty : (Guid)data.CustomerInfo
                           })

                : (from data in CommodityOrder.ObjectSet().Where(x => ((x.AppId == search.AppId || applist.Contains(x.AppId))) && x.State != 16 && x.State != 17)
                       //join cf in CfOrderDividend.ObjectSet()
                       //on data.Id equals cf.CommodityOrderId
                       //into cfOrderDividends
                       //from cfOrderDividend in cfOrderDividends.DefaultIfEmpty()
                   join dataS in CommodityOrderService.ObjectSet()
                   on data.Id equals dataS.Id
                   into tempS
                   from tbS in tempS.DefaultIfEmpty()
                       //join opu in AppOrderPickUp.ObjectSet()
                       //on data.Id equals opu.Id into opuOrderPickUps
                       //from opuOrderPickUp in opuOrderPickUps.DefaultIfEmpty()

                       //where (applist.Contains(data.AppId) || data.AppId == search.AppId) && data.State != 16 && data.State != 17

                       //join oi in
                       //    (from oi in OrderItem.ObjectSet()
                       //     group oi by oi.CommodityOrderId into oiGroup
                       //     select new
                       //     {
                       //         Key = oiGroup.Key,
                       //         ReturnYoukaMoney = oiGroup.Sum(selector => Math.Round((selector.RealPrice ?? 0) * selector.Number * (selector.YouKaPercent ?? 0) / 100, 2)),
                       //         YJBMoney = oiGroup.Sum(selector => selector.YjbPrice)
                       //     }) on data.Id equals oi.Key
                       //where
                       //   (applist.Contains(data.AppId) || data.AppId == search.AppId)
                       //   && data.State != 16 && data.State != 17
                       //   && (search.Marketing == 1 ? oi.YJBMoney > 0 : (search.Marketing == 2 ? oi.ReturnYoukaMoney > 0 : true))

                   select new CommodityOrderVM
                   {
                       AppId = data.AppId,
                       UserId = data.UserId,
                       CommodityOrderId = data.Id,
                       CurrentPrice = data.RealPrice,
                       Details = data.Details,
                       State = data.State,
                       CommodityOrderCode = data.Code,
                       SubTime = data.SubTime,
                       ReceiptUserName = data.ReceiptUserName,
                       Payment = data.Payment,
                       PaymentTime = data.PaymentTime,
                       ShipmentsTime = data.ShipmentsTime,
                       ConfirmTime = data.ConfirmTime,
                       ModifiedOn = data.ModifiedOn,
                       Price = data.Price,
                       MessageToBuyer = data.MessageToBuyer,
                       IsModifiedPrice = data.IsModifiedPrice,
                       ReceiptPhone = data.ReceiptPhone,
                       ReceiptAddress = data.ReceiptAddress,
                       Province = data.Province,
                       District = data.District,
                       City = data.City,
                       Street = data.Street,
                       RefundExpCo = "",
                       RefundTime = data.RefundTime,
                       AgreementTime = data.AgreementTime,
                       Freight = data.Freight,
                       Commission = data.Commission,
                       SellersRemark = data.SellersRemark,
                       //CfDividend = cfOrderDividend.Gold / 1000.0m,
                       IsCrowdfunding = data.IsCrowdfunding != 0,
                       SelfTakeFlag = data.SelfTakeFlag,
                       SpreadGold = data.SpreadGold,
                       RefundMoney = 0,
                       RefundType = 0,
                       RefundExpOrderNo = "",
                       IsDelayConfirmTime = data.IsDelayConfirmTime,
                       IsDelayConfirmTimeAfterSales = tbS.IsDelayConfirmTimeAfterSales == null ? false : tbS.IsDelayConfirmTimeAfterSales,
                       OwnerShare = data.OwnerShare,
                       //SelfTakeProvinceCode = opuOrderPickUp.StsProvince,
                       //SelfTakeCityCode = opuOrderPickUp.StsCity,
                       //SelfTakeDistrictCode = opuOrderPickUp.StsDistrict,
                       //SelfTakeAddress = opuOrderPickUp.StsAddress,
                       StateRefund = -1,
                       StateAfterSales = tbS.State == null ? -1 : tbS.State,
                       StateRefundAfterSales = -1,
                       EndTime = tbS.EndTime == null ? null : tbS.EndTime,
                       ModifiedOnServiceAfterSales = tbS.ModifiedOn == null ? DateTime.Now : tbS.ModifiedOn,
                       OrderType = data.OrderType,
                       DistributorId = data.DistributorId,
                       DistributeMoney = data.DistributeMoney,
                       EsAppId = data.EsAppId,
                       PicturesPath = data.PicturesPath,
                       ChannelShareMoney = data.ChannelShareMoney,
                       Batch = data.Batch,
                       GoldCoupon = data.GoldCoupon,
                       GoldPrice = data.GoldPrice,
                       ExpressPrintCount = data.ExpressPrintCount,
                       InvoicePrintCount = data.InvoicePrintCount,
                       SetMealId = data.SetMealId == null ? Guid.Empty : (Guid)data.SetMealId,
                       Specifications = 0,
                       AppName = data.AppName,
                       //ReturnYoukaMoney = oi.ReturnYoukaMoney,
                       //SpendYJBMoney = oi.YJBMoney ?? 0,
                       //SpendYJCouponMoney = oi.YJCouponPrice ?? 0,
                       JcActivityId = data.JcActivityId == null ? Guid.Empty : (Guid)data.JcActivityId,
                       CustomerInfo = data.CustomerInfo == null ? Guid.Empty : (Guid)data.CustomerInfo
                   });



            if (search.Marketing == 1)
            {
                oquery = oquery.Where(n => n.SpendYJBMoney > 0);
            }
            if (search.Marketing == 2)
            {
                oquery = oquery.Where(n => n.ReturnYoukaMoney > 0);
            }
            if (search.PriceLow != "-1" && search.PriceLow != null && search.PriceLow != "")
            {
                decimal _priceLow = decimal.Parse(search.PriceLow);
                oquery = oquery.Where(n => n.CurrentPrice >= _priceLow);
            }
            if (search.PriceHight != "-1" && search.PriceHight != null && search.PriceHight != "")
            {
                decimal _priceHight = decimal.Parse(search.PriceHight);
                oquery = oquery.Where(n => n.CurrentPrice <= _priceHight);
            }
            if (search.SeacrhContent != null && !string.IsNullOrEmpty(search.SeacrhContent))
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(search.SeacrhContent, "^[0-9]+$"))
                {
                    oquery = oquery.Where(n => n.CommodityOrderCode.Contains(search.SeacrhContent) || n.ReceiptPhone.Contains(search.SeacrhContent));
                }
                else
                {
                    oquery = oquery.Where(n => n.ReceiptUserName.Contains(search.SeacrhContent) || n.AppName.Contains(search.SeacrhContent));
                }
            }

            if (!string.IsNullOrEmpty(search.RegisterPhone))
            {
                Guid userId = CBCSV.GetUserIdByAccount(search.RegisterPhone);
                oquery = oquery.Where(n => n.UserId == userId);
            }

            if (search.UserId != null && search.UserId != Guid.Empty)
            {
                oquery = oquery.Where(n => n.UserId == search.UserId);
            }
            if (search.DayCount != null && search.DayCount != "0" && search.DayCount != "")
            {
                switch (int.Parse(search.DayCount))
                {
                    case 0:
                        break;
                    default:
                        int _dayCount = int.Parse(search.DayCount);
                        DateTime date = DateTime.Now.AddDays(-_dayCount);//减去多少天
                        oquery = oquery.Where(n => n.SubTime.Value >= date);
                        //countquery = countquery.Where(n => n.SubTime >= date);
                        break;
                }

            }
            if (search.StartOrderTime.HasValue)
            {
                if (!search.EndOrderTime.HasValue)
                {
                    search.EndOrderTime = DateTime.Now;
                }
                DateTime start = search.StartOrderTime.Value.Date;
                DateTime end = search.EndOrderTime.Value.Date.AddDays(1);
                oquery = oquery.Where(n => n.SubTime >= start && n.SubTime < end);
            }
            if (search.State != null)
            {
                if (search.State.Contains(","))
                {
                    if (search.State == "8,9,10,12,14")   //退款中
                    {
                        List<int> beforeState = new List<int>() { 8, 9, 10, 12, 14 };
                        List<int> afterState = new List<int>() { 5, 10, 12 };
                        oquery = oquery.Where(n => beforeState.Contains(n.State) || afterState.Contains(n.StateAfterSales));
                    }
                    else
                    {
                        int[] arrystate = Array.ConvertAll<string, int>(search.State.Split(','), s => int.Parse(s));

                        //等发货且自提
                        if (arrystate.Contains(1) && arrystate.Contains(99))
                        {
                            int[] exceptTmp = new int[] { 99 };
                            int[] arrystateTmp = arrystate.Except(exceptTmp).ToArray();
                            oquery = oquery.Where(a => arrystateTmp.Contains(a.State));
                        }
                        else if (arrystate.Contains(1))
                        {
                            if (arrystate.Contains(11))
                            {
                                int[] exceptTmp = new int[] { 1, 11 };
                                int[] arrystateTmp = arrystate.Except(exceptTmp).ToArray();
                                oquery = oquery.Where(a => arrystateTmp.Contains(a.State) || ((a.State == 1 || a.State == 11) && a.SelfTakeFlag == 0));
                            }
                            else
                            {
                                int[] exceptTmp = new int[] { 1 };
                                int[] arrystateTmp = arrystate.Except(exceptTmp).ToArray();
                                oquery = oquery.Where(a => arrystateTmp.Contains(a.State) || (a.State == 1 && a.SelfTakeFlag == 0));
                            }
                        }
                        else if (arrystate.Contains(99))
                        {
                            int[] exceptTmp = new int[] { 99 };
                            int[] arrystateTmp = arrystate.Except(exceptTmp).ToArray();
                            oquery = oquery.Where(a => arrystateTmp.Contains(a.State) || ((a.State == 1 || a.State == 11) && a.SelfTakeFlag == 1));
                        }
                        else
                        {
                            oquery = oquery.Where(a => arrystate.Contains(a.State));
                        }
                    }
                }
                else
                {
                    if (search.State != "-1" && search.State != null && search.State != "")
                    {
                        int _state = int.Parse(search.State);
                        //待发货的
                        if (_state == 1)
                        {
                            oquery = oquery.Where(n => n.State == _state && n.SelfTakeFlag == 0);
                        }
                        //待自提的
                        else if (_state == 99)
                        {
                            oquery = oquery.Where(n => (n.State == 1 || n.State == 11) && n.SelfTakeFlag == 1);
                        }
                        else if (search.State == "3") //交易成功
                        {
                            oquery = oquery.Where(n => n.State == 3 && (n.StateAfterSales == 3 || n.StateAfterSales == 15 || n.StateAfterSales == -1));
                        }
                        else if (search.State == "7")
                        {
                            oquery = oquery.Where(n => n.State == 7 || n.StateAfterSales == 7);
                        }
                        else
                        {
                            oquery = oquery.Where(n => n.State == _state);
                        }
                        //countquery = countquery.Where(n => n.State == _state);
                    }
                }
            }
            if (search.Payment != "-1" && search.Payment != null && search.Payment != "")
            {
                oquery = oquery.Where(n => n.State > 0);
                if (search.Payment.Contains(","))
                {
                    List<string> pmStrList = search.Payment.Split(",，".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                    List<int> pmIntList = pmStrList.ConvertAll(strPm => Convert.ToInt32(strPm));
                    oquery = oquery.Where(pm => pmIntList.Contains(pm.Payment));
                }
                else
                {
                    int _payment = int.Parse(search.Payment);
                    oquery = oquery.Where(n => n.Payment == _payment);// && n.State != 0 && n.State != 4);
                }

                //countquery = countquery.Where(n => n.Payment == _payment && n.State != 0 && n.State != 4);
            }
            if (search.EsAppId.HasValue && search.EsAppId.Value != Guid.Empty)
            {
                var directArrival = PaySource.GetPaymentByTradeType(1);
                if (search.EsAppId.Value == search.AppId)
                {
                    oquery = oquery.Where(n => n.EsAppId == search.EsAppId.Value || (n.EsAppId != search.EsAppId.Value && !directArrival.Contains(n.Payment)));
                }
                else
                {
                    oquery = oquery.Where(n => n.EsAppId == search.EsAppId.Value && !directArrival.Contains(n.Payment));// && n.State != 0 && n.State != 4);
                }

                if (isEsAppId)
                {
                    oquery = oquery.Where(n => n.EsAppId == search.EsAppId.Value);// && n.State != 0 && n.State != 4);
                }
            }

            if (search.OrderSourceId.HasValue && search.OrderSourceId.Value != Guid.Empty)
            {
                oquery = oquery.Where(n => n.EsAppId == search.OrderSourceId.Value);
            }

            if (search.LastPayTime.HasValue)
            {
                oquery = oquery.Where(c => c.PaymentTime > search.LastPayTime);
            }


            #region Old
            //List<CommodityOrderVM> query = new List<CommodityOrderVM>();
            //foreach (var commodityOrderVm in oquery)
            //{
            //    commodityOrderVm.AppName = Jinher.AMP.BTP.TPS.APPSV.GetAppName(commodityOrderVm.AppId);
            //    query.Add(commodityOrderVm);
            //}
            //if (search.PriceLow != "-1" && search.PriceLow != null && search.PriceLow != "")
            //{
            //    decimal _priceLow = decimal.Parse(search.PriceLow);
            //    query = query.Where(n => n.CurrentPrice >= _priceLow).ToList();
            //    //countquery = countquery.Where(n => n.RealPrice >= _priceLow);
            //}
            //if (search.PriceHight != "-1" && search.PriceHight != null && search.PriceHight != "")
            //{
            //    decimal _priceHight = decimal.Parse(search.PriceHight);
            //    query = query.Where(n => n.CurrentPrice <= _priceHight).ToList();
            //    //countquery = countquery.Where(n => n.RealPrice <= _priceHight);
            //}
            //if (search.SeacrhContent != null && !string.IsNullOrEmpty(search.SeacrhContent))
            //{
            //    if (System.Text.RegularExpressions.Regex.IsMatch(search.SeacrhContent, "^[0-9]+$"))
            //    {
            //        query = query.Where(n => n.CommodityOrderCode.Contains(search.SeacrhContent) || n.ReceiptPhone.Contains(search.SeacrhContent)).ToList();
            //    }
            //    else
            //    {
            //        query = query.Where(n => n.ReceiptUserName.Contains(search.SeacrhContent) || n.AppName.Contains(search.SeacrhContent)).ToList();
            //    }
            //}
            //if (search.DayCount != null && search.DayCount != "0" && search.DayCount != "")
            //{
            //    switch (int.Parse(search.DayCount))
            //    {
            //        case 0:
            //            break;
            //        default:
            //            int _dayCount = int.Parse(search.DayCount);
            //            DateTime date = DateTime.Now.AddDays(-_dayCount);//减去多少天
            //            query = query.Where(n => n.SubTime.Value >= date).ToList();
            //            //countquery = countquery.Where(n => n.SubTime >= date);
            //            break;
            //    }

            //}
            //if (search.StartOrderTime.HasValue)
            //{
            //    if (!search.EndOrderTime.HasValue)
            //    {
            //        search.EndOrderTime = DateTime.Now;
            //    }
            //    DateTime start = search.StartOrderTime.Value.Date;
            //    DateTime end = search.EndOrderTime.Value.Date.AddDays(1);
            //    query = query.Where(n => n.SubTime >= start && n.SubTime < end).ToList();
            //}
            //if (search.State != null)
            //{
            //    if (search.State.Contains(","))
            //    {
            //        if (search.State == "8,9,10,12,14")   //退款中
            //        {
            //            List<int> beforeState = new List<int>() { 8, 9, 10, 12, 14 };
            //            List<int> afterState = new List<int>() { 5, 10, 12 };
            //            query = query.Where(n => beforeState.Contains(n.State) || afterState.Contains(n.StateAfterSales)).ToList();
            //        }
            //        else
            //        {
            //            int[] arrystate = Array.ConvertAll<string, int>(search.State.Split(','), s => int.Parse(s));

            //            //等发货且自提
            //            if (arrystate.Contains(1) && arrystate.Contains(99))
            //            {
            //                int[] exceptTmp = new int[] { 99 };
            //                int[] arrystateTmp = arrystate.Except(exceptTmp).ToArray();
            //                query = query.Where(a => arrystateTmp.Contains(a.State)).ToList();
            //            }
            //            else if (arrystate.Contains(1))
            //            {
            //                if (arrystate.Contains(11))
            //                {
            //                    int[] exceptTmp = new int[] { 1, 11 };
            //                    int[] arrystateTmp = arrystate.Except(exceptTmp).ToArray();
            //                    query = query.Where(a => arrystateTmp.Contains(a.State) || ((a.State == 1 || a.State == 11) && a.SelfTakeFlag == 0)).ToList();
            //                }
            //                else
            //                {
            //                    int[] exceptTmp = new int[] { 1 };
            //                    int[] arrystateTmp = arrystate.Except(exceptTmp).ToArray();
            //                    query = query.Where(a => arrystateTmp.Contains(a.State) || (a.State == 1 && a.SelfTakeFlag == 0)).ToList();
            //                }
            //            }
            //            else if (arrystate.Contains(99))
            //            {
            //                int[] exceptTmp = new int[] { 99 };
            //                int[] arrystateTmp = arrystate.Except(exceptTmp).ToArray();
            //                query = query.Where(a => arrystateTmp.Contains(a.State) || ((a.State == 1 || a.State == 11) && a.SelfTakeFlag == 1)).ToList();
            //            }
            //            else
            //            {
            //                query = query.Where(a => arrystate.Contains(a.State)).ToList();
            //            }
            //        }
            //    }
            //    else
            //    {
            //        if (search.State != "-1" && search.State != null && search.State != "")
            //        {
            //            int _state = int.Parse(search.State);
            //            //待发货的
            //            if (_state == 1)
            //            {
            //                query = query.Where(n => n.State == _state && n.SelfTakeFlag == 0).ToList();
            //            }
            //            //待自提的
            //            else if (_state == 99)
            //            {
            //                query = query.Where(n => (n.State == 1 || n.State == 11) && n.SelfTakeFlag == 1).ToList();
            //            }
            //            else if (search.State == "3") //交易成功
            //            {
            //                query = query.Where(n => n.State == 3 && (n.StateAfterSales == 3 || n.StateAfterSales == 15 || n.StateAfterSales == -1)).ToList();
            //            }
            //            else if (search.State == "7")
            //            {
            //                query = query.Where(n => n.State == 7 || n.StateAfterSales == 7).ToList();
            //            }
            //            else
            //            {
            //                query = query.Where(n => n.State == _state).ToList();
            //            }
            //            //countquery = countquery.Where(n => n.State == _state);
            //        }
            //    }
            //}
            //if (search.Payment != "-1" && search.Payment != null && search.Payment != "")
            //{
            //    query = query.Where(n => n.State > 0).ToList();
            //    if (search.Payment.Contains(","))
            //    {
            //        List<string> pmStrList = search.Payment.Split(",，".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
            //        List<int> pmIntList = pmStrList.ConvertAll(strPm => Convert.ToInt32(strPm));
            //        query = query.Where(pm => pmIntList.Contains(pm.Payment)).ToList();
            //    }
            //    else
            //    {
            //        int _payment = int.Parse(search.Payment);
            //        query = query.Where(n => n.Payment == _payment).ToList();// && n.State != 0 && n.State != 4);
            //    }

            //    //countquery = countquery.Where(n => n.Payment == _payment && n.State != 0 && n.State != 4);
            //}
            //if (search.EsAppId.HasValue && search.EsAppId.Value != Guid.Empty)
            //{
            //    var directArrival = PaySource.GetPaymentByTradeType(1);
            //    if (search.EsAppId.Value == search.AppId)
            //    {
            //        query = query.Where(n => n.EsAppId == search.EsAppId.Value || (n.EsAppId != search.EsAppId.Value && !directArrival.Contains(n.Payment))).ToList();
            //    }
            //    else
            //    {
            //        query = query.Where(n => n.EsAppId == search.EsAppId.Value && !directArrival.Contains(n.Payment)).ToList();// && n.State != 0 && n.State != 4);
            //    }

            //    if (isEsAppId)
            //    {
            //        query = query.Where(n => n.EsAppId == search.EsAppId.Value).ToList();// && n.State != 0 && n.State != 4);
            //    }
            //}

            //if (search.OrderSourceId.HasValue && search.OrderSourceId.Value != Guid.Empty)
            //{
            //    query = query.Where(n => n.EsAppId == search.OrderSourceId.Value).ToList();
            //}

            //if (search.LastPayTime.HasValue)
            //{
            //    query = query.Where(c => c.PaymentTime > search.LastPayTime).ToList();
            //} 
            #endregion


            //oquery = oquery.Distinct();
            result.Data.Count = oquery.Count();

            result.Message = string.Join(",", oquery.Select(t => t.CommodityOrderId).ToList());

            var searchResult = oquery.OrderByDescending(t => t.ModifiedOn).ThenByDescending(t => t.SubTime).Skip((search.PageIndex - 1) * search.PageSize).Take(search.PageSize).ToList();


            //订单ID
            var idList = searchResult.Select(t => t.CommodityOrderId).ToList();

            //获取所有检索的orderids
            //var strOrderIds = query.Aggregate("", (current, commodityOrderVm) => current + (commodityOrderVm.CommodityOrderId + ","));
            //if (!string.IsNullOrEmpty(strOrderIds))
            //{
            //    result.Message = strOrderIds.Substring(0, strOrderIds.Length - 1);
            //}

            var golds = (from u in CfOrderDividend.ObjectSet().Where(x => idList.Contains(x.CommodityOrderId))
                         select new { Id = u.CommodityOrderId, Gold = u.Gold, }).ToList();


            var opuOrderPickUp = (from u in AppOrderPickUp.ObjectSet().Where(x => idList.Contains(x.Id))
                                  select new { Id = u.Id, StsProvince = u.StsProvince, StsCity = u.StsCity, StsDistrict = u.StsDistrict, StsAddress = u.StsAddress })
                      .ToList();

            //var commodityOrderService = (from c in CommodityOrderService.ObjectSet().Where(x => idList.Contains(x.Id))
            //                             select new
            //                             {
            //                                 Id = c.Id,
            //                                 IsDelayConfirmTimeAfterSales = c.IsDelayConfirmTimeAfterSales,
            //                                 State = c.State,
            //                                 EndTime = c.EndTime,
            //                                 ModifiedOn = c.ModifiedOn
            //                             }).ToList();
            //自提点省市附值
            foreach (var item in searchResult)
            {
                item.CfDividend = golds.Where(x => x.Id == item.CommodityOrderId).Select(x => x.Gold).FirstOrDefault() / 1000.0m;

                item.SelfTakeProvinceCode = opuOrderPickUp.Where(x => x.Id == item.CommodityOrderId).Select(x => x.StsProvince).FirstOrDefault();
                item.SelfTakeCityCode = opuOrderPickUp.Where(x => x.Id == item.CommodityOrderId).Select(x => x.StsCity).FirstOrDefault();
                item.SelfTakeDistrictCode = opuOrderPickUp.Where(x => x.Id == item.CommodityOrderId).Select(x => x.StsDistrict).FirstOrDefault();
                item.SelfTakeAddress = opuOrderPickUp.Where(x => x.Id == item.CommodityOrderId).Select(x => x.StsAddress).FirstOrDefault();

                //var tbs = commodityOrderService.Where(x => x.Id == item.CommodityOrderId).FirstOrDefault();

                //if (tbs == null)
                //{
                //    item.IsDelayConfirmTimeAfterSales = false;
                //    item.StateAfterSales = 0;
                //    item.EndTime = null;
                //    item.ModifiedOnServiceAfterSales = DateTime.Now;
                //}
                //else
                //{
                //    item.IsDelayConfirmTimeAfterSales = tbs.IsDelayConfirmTimeAfterSales;
                //    item.StateAfterSales = tbs.State;
                //    item.EndTime = tbs.EndTime == null ? null : tbs.EndTime;
                //    item.ModifiedOnServiceAfterSales = tbs.ModifiedOn == null ? DateTime.Now : tbs.ModifiedOn;
                //}

                if (!string.IsNullOrWhiteSpace(item.SelfTakeProvinceCode))
                    item.SelfTakeProvinceName = ProvinceCityHelper.GetAreaNameByCode(item.SelfTakeProvinceCode);
                if (!string.IsNullOrWhiteSpace(item.SelfTakeCityCode))
                    item.SelfTakeCityName = ProvinceCityHelper.GetAreaNameByCode(item.SelfTakeCityCode);
                if (!string.IsNullOrWhiteSpace(item.SelfTakeDistrictCode))
                    item.SelfTakeDistrictName = ProvinceCityHelper.GetAreaNameByCode(item.SelfTakeDistrictCode);
            }

            if (!(search.Marketing == 1 || search.Marketing == 2))
            {
                var orderItem = (from oi in OrderItem.ObjectSet().Where(x => idList.Contains(x.CommodityOrderId))
                                 group oi by oi.CommodityOrderId into oiGroup
                                 select new
                                 {
                                     Key = oiGroup.Key,
                                     ReturnYoukaMoney = oiGroup.Sum(selector => Math.Round((selector.RealPrice ?? 0) * selector.Number * (selector.YouKaPercent ?? 0) / 100, 2)),
                                     YJBMoney = oiGroup.Sum(selector => selector.YjbPrice),
                                     YJCouponPrice = oiGroup.Sum(s => s.YJCouponPrice)
                                 }).ToList();

                foreach (var item in searchResult)
                {
                    item.ReturnYoukaMoney = orderItem.Where(x => x.Key == item.CommodityOrderId).Select(x => x.ReturnYoukaMoney).FirstOrDefault();
                    item.SpendYJBMoney = orderItem.Where(x => x.Key == item.CommodityOrderId).Select(x => x.YJBMoney).FirstOrDefault().GetValueOrDefault();
                    item.SpendYJCouponMoney = orderItem.Where(x => x.Key == item.CommodityOrderId).Select(x => x.YJCouponPrice).FirstOrDefault().GetValueOrDefault();
                }
            }


            //金彩支付分期回款xiexg
            #region

            if (search.Isjczf != null && (bool)search.Isjczf)
            {

                // IEnumerable<Guid> idJcActivitys = searchResult.Select(o => o.JcActivityId).Distinct().ToList();
                IEnumerable<Guid> CustomerInfos = searchResult.Where(o => o.CustomerInfo != null).Select(o => (Guid)o.CustomerInfo).Distinct().ToList();

                Jinher.AMP.ZPH.ISV.Facade.FJCPayFacade SD = new Jinher.AMP.ZPH.ISV.Facade.FJCPayFacade();
                // List<Guid> L = (List<Guid>)idJcActivitys;
                List<Guid> L = (List<Guid>)CustomerInfos;
                var resutltcus = SD.GetJcCustomersByActivityIds(L);
                var idUser = searchResult.Select(t => t.UserId).ToList();
                var refund = from obj in CommodityOrderRefund.ObjectSet()
                             where idList.Contains(obj.CommodityOrderId)

                             select new CommodityOrderRefundDTO
                             {
                                 Id = obj.Id,
                                 SubId = obj.SubId,
                                 RefundDate = obj.RefundDate,
                                 RefundAmount = obj.RefundAmount,
                                 Remark = obj.Remark,
                                 RefundType = obj.RefundType,
                                 CommodityOrderId = obj.CommodityOrderId
                             };
                foreach (var item in searchResult)
                {
                    var model = refund.Where(o => o.CommodityOrderId == item.CommodityOrderId).OrderByDescending(o => o.RefundDate).ToList().FirstOrDefault();
                    if (model != null)
                    {
                        item.JCZFRefundAmount = refund.Where(o => o.CommodityOrderId == item.CommodityOrderId).ToList().Sum(o => o.RefundAmount);
                        item.JCZFRefundDate = model.RefundDate;
                        item.JCZFRefundType = model.RefundType;
                    }
                    if (resutltcus != null)
                    {
                        var userModel = resutltcus.Data.Where(o => o.id == item.CustomerInfo).ToList().FirstOrDefault();
                        if (userModel != null)
                        {
                            item.CustomerCode = userModel.Code;
                            item.CustomerName = userModel.Name;
                        }
                    }


                }

            }

            #endregion

            //取出售中退款信息
            var middle = (from o in OrderRefund.ObjectSet()
                          where idList.Contains(o.OrderId) && (o.OrderItemId == null || o.OrderItemId == Guid.Empty)
                          select new OrderRefundTmp
                          {
                              OrderId = o.OrderId,
                              RefundMoney = o.RefundMoney,
                              RefundType = o.RefundType,
                              SubTime = o.SubTime,
                              RefundExpOrderNo = o.RefundExpOrderNo,
                              RefundExpCo = o.RefundExpCo,
                              StateRefund = o.State,
                              RefuseTime = o.RefuseTime,
                              RefundScoreMoney = o.RefundScoreMoney,
                              OrderItemId = Guid.Empty,
                              RefundYJBMoney = o.RefundYJBMoney
                          }).GroupBy(t => t.OrderId).ToDictionary(x => x.Key, y => y.OrderByDescending(t => t.SubTime).FirstOrDefault());

            //售中订单ID
            var idMiddleList = middle.Select(t => t.Value.OrderId).ToList();
            if (idMiddleList.Count > 0)
            {
                foreach (var item in searchResult)
                {
                    var modelMiddle = middle.Where(t => t.Value.OrderId == item.CommodityOrderId).FirstOrDefault();
                    if (modelMiddle.Value != null)
                    {
                        item.RefundMoney = modelMiddle.Value.RefundMoney;
                        item.RefundType = modelMiddle.Value.RefundType;
                        item.RefundExpOrderNo = modelMiddle.Value.RefundExpOrderNo;
                        item.RefundExpCo = modelMiddle.Value.RefundExpCo;
                        item.StateRefund = modelMiddle.Value.StateRefund;
                        item.RefundScoreMoney = modelMiddle.Value.RefundScoreMoney;
                        item.RefundYJBMoney = modelMiddle.Value.RefundYJBMoney;
                        item.OrderItemId = modelMiddle.Value.OrderItemId;
                    }
                }
            }

            //取出售后退款信息
            var afterSales = (from o in OrderRefundAfterSales.ObjectSet()
                              where idList.Contains(o.OrderId) && (o.OrderItemId == null || o.OrderItemId == Guid.Empty)
                              select new OrderRefundTmp
                              {
                                  OrderId = o.OrderId,
                                  RefundMoney = o.RefundMoney,
                                  RefundType = o.RefundType,
                                  SubTime = o.SubTime,
                                  RefundExpOrderNo = o.RefundExpOrderNo,
                                  RefundExpCo = o.RefundExpCo,
                                  StateRefundAfterSales = o.State,
                                  RefuseTime = o.RefuseTime,
                                  RefundScoreMoney = o.RefundScoreMoney,
                                  OrderItemId = Guid.Empty,
                                  RefundYJBMoney = o.RefundYJBMoney
                              }).GroupBy(t => t.OrderId).ToDictionary(x => x.Key, y => y.OrderByDescending(t => t.SubTime).FirstOrDefault());

            //售后订单ID
            var idAfterSalesList = afterSales.Select(t => t.Value.OrderId).ToList();
            if (idAfterSalesList.Count > 0)
            {
                foreach (var item in searchResult)
                {
                    //可售后
                    if (item.State == 3)
                    {
                        var modelAfterSales = afterSales.Where(t => t.Value.OrderId == item.CommodityOrderId).FirstOrDefault();
                        if (modelAfterSales.Value != null)
                        {
                            item.RefundMoney = modelAfterSales.Value.RefundMoney;
                            item.RefundType = modelAfterSales.Value.RefundType;
                            item.RefundExpOrderNo = modelAfterSales.Value.RefundExpOrderNo;
                            item.RefundExpCo = modelAfterSales.Value.RefundExpCo;
                            item.StateRefundAfterSales = modelAfterSales.Value.StateRefundAfterSales;
                            item.RefundTime = modelAfterSales.Value.SubTime;
                            item.AgreementTime = modelAfterSales.Value.RefuseTime;
                            item.RefundScoreMoney = modelAfterSales.Value.RefundScoreMoney;
                            item.RefundYJBMoney = modelAfterSales.Value.RefundYJBMoney;
                            item.OrderItemId = modelAfterSales.Value.OrderItemId;
                        }
                        else
                        {
                            item.StateRefundAfterSales = -1;
                        }
                    }
                    else
                    {
                        item.StateAfterSales = -1;
                        item.StateRefundAfterSales = -1;
                    }
                }
            }

            foreach (var item in searchResult)
            {
                var norderItemIds = OrderRefund.ObjectSet().Where(t => t.OrderId == item.CommodityOrderId && t.OrderItemId != Guid.Empty).Select(t => t.OrderItemId).Distinct();

                if (norderItemIds.Count() > 0 && norderItemIds.First() != null)
                {
                    decimal totRefundMoney = 0;
                    decimal totRefundScoreMoney = 0;
                    decimal totRefundYJBMoney = 0;
                    foreach (var orderItemId in norderItemIds)
                    {
                        var a = OrderRefund.ObjectSet().Where(t => t.OrderItemId == orderItemId).OrderByDescending(t => t.SubTime).FirstOrDefault();
                        LogHelper.Info(string.Format("订单id:{0}", orderItemId));
                        if (a != null)
                        {
                            totRefundMoney += a.RefundMoney;
                            totRefundScoreMoney += a.RefundScoreMoney;
                            totRefundYJBMoney += a.RefundYJBMoney;
                        }
                    }
                    item.RefundMoney = totRefundMoney;
                    item.RefundScoreMoney = totRefundScoreMoney;
                    item.RefundYJBMoney = totRefundYJBMoney;
                    item.OrderItemId = (Guid)norderItemIds.First();

                }
                else
                {
                    norderItemIds = OrderRefundAfterSales.ObjectSet().Where(t => t.OrderId == item.CommodityOrderId).Select(t => t.OrderItemId).Distinct();
                    if (norderItemIds.Count() > 0 && norderItemIds.First() != null)
                    {
                        decimal totRefundMoney = 0;
                        decimal totRefundScoreMoney = 0;
                        decimal totRefundYJBMoney = 0;
                        foreach (var orderItemId in norderItemIds)
                        {
                            var a = OrderRefundAfterSales.ObjectSet().Where(t => t.OrderItemId == orderItemId).OrderByDescending(t => t.SubTime).FirstOrDefault();
                            if (a != null)
                            {
                                totRefundMoney += a.RefundMoney;
                                totRefundScoreMoney += a.RefundScoreMoney;
                                totRefundYJBMoney += a.RefundYJBMoney;
                            }
                        }
                        item.RefundMoney = totRefundMoney;
                        item.RefundScoreMoney = totRefundScoreMoney;
                        item.RefundYJBMoney = totRefundYJBMoney;
                        item.OrderItemId = (Guid)norderItemIds.First();
                    }
                }
            }
            //花费积分抵现金额 SpendScoreCost
            var orderPayDetail = OrderPayDetail.ObjectSet().Where(t => idList.Contains(t.OrderId)).ToList();
            if (orderPayDetail.Count > 0)
            {
                foreach (var item in searchResult)
                {
                    var spendScoreMoney = orderPayDetail.Where(t => t.OrderId == item.CommodityOrderId && t.ObjectType == 2).Select(t => t.Amount).FirstOrDefault();
                    item.SpendScoreMoney = spendScoreMoney;
                    var couponMoney = orderPayDetail.Where(t => t.OrderId == item.CommodityOrderId && t.ObjectType == 1).Select(t => t.Amount).FirstOrDefault();
                    item.CouponValue = couponMoney;
                }
            }

            //// 查询易捷币抵现金额
            //var orderIds = searchResult.Where(s => s.EsAppId == YJB.Deploy.CustomDTO.YJBConsts.YJAppId).Select(t => t.CommodityOrderId).Distinct().ToList();
            //if (orderIds.Count > 0)
            //{
            //    var yjInfoes = YJBSV.GetOrderInfoes(orderIds);

            //    if (yjInfoes.IsSuccess)
            //    {
            //        foreach (var item in searchResult)
            //        {
            //            var yjInfo = yjInfoes.Data.Find(y => y.OrderId == item.CommodityOrderId);
            //            if (yjInfo != null)
            //            {
            //                if (yjInfo.YJBInfo != null)
            //                {
            //                    item.SpendYJBMoney = yjInfo.YJBInfo.InsteadCashAmount;
            //                    item.RefundYJBMoney = yjInfo.YJBInfo.RefundCashAmount;
            //                }
            //                if (yjInfo.YJCouponInfo != null) item.SpendYJCouponMoney = yjInfo.YJCouponInfo.InsteadCashAmount;
            //            }
            //        }
            //    }
            //}


            #region 查询订单商品信息(优化)

            //构建订单id数组
            List<Guid> commodityOrderIdList = new List<Guid>();
            for (int i = 0; i < searchResult.Count; i++)
            {
                commodityOrderIdList.Add(searchResult[i].CommodityOrderId);
            }

            CommodityCategory cc = new CommodityCategory();

            //取出所有订单的所有商品
            var orderItems = (from data in OrderItem.ObjectSet()
                                  //join data1 in Commodity.ObjectSet() on data.CommodityId equals data1.Id
                              where commodityOrderIdList.Contains(data.CommodityOrderId)
                              select new
                              {
                                  Id = data.Id,
                                  CommodityOrderId = data.CommodityOrderId,
                                  CommodityId = data.CommodityId,
                                  CommodityIdName = data.Name,
                                  PicturesPath = data.PicturesPath,
                                  Price = data.CurrentPrice,//取订单商品列表中的价格
                                  Number = data.Number,
                                  CommodityAttributes = data.CommodityAttributes,
                                  CategoryName = data.CategoryNames,
                                  RealPrice = data.RealPrice,
                                  //IsEnableSelfTake = data1.IsEnableSelfTake,
                                  ComCategoryName = data.ComCategoryName,
                                  data.State,
                                  data.RefundExpCo,
                                  data.RefundExpOrderNo,
                                  data.ScorePrice,
                                  data.YjbPrice,
                                  data.FreightPrice,
                                  data.CouponPrice
                              }).ToList();

            var commodityIds = orderItems.Select(s => s.CommodityId).ToList();

            var commoditys = (from data in Commodity.ObjectSet()
                              where commodityIds.Contains(data.Id)
                              select new { Id = data.Id, IsEnableSelfTake = data.IsEnableSelfTake }).ToList();


            List<OrderItemsVM> orderItemsVMList = (from data in orderItems
                                                   select new OrderItemsVM
                                                   {
                                                       Id = data.Id,
                                                       CommodityOrderId = data.CommodityOrderId,
                                                       CommodityId = data.CommodityId,
                                                       CommodityIdName = data.CommodityIdName,
                                                       PicturesPath = data.PicturesPath,
                                                       Price = data.Price,//取订单商品列表中的价格
                                                       RealPrice = data.RealPrice,
                                                       Number = data.Number,
                                                       SizeAndColorId = data.CommodityAttributes,
                                                       CommodityCategorys =
                                                       data.CategoryName == null ? new List<string>() : data.CategoryName.Split(',').ToList(),
                                                       IsEnableSelfTake = commoditys.Where(x => x.Id == data.CommodityId).Select(s => s.IsEnableSelfTake).FirstOrDefault(),//data.IsEnableSelfTake,
                                                       ComCategoryName = data.ComCategoryName,
                                                       JdorderId = null,
                                                       State = data.State == null ? 0 : (int)data.State,
                                                       RefundExpCo = data.RefundExpCo,
                                                       RefundExpOrderNo = data.RefundExpOrderNo,
                                                       ScorePrice = data.ScorePrice,
                                                       YjbPrice = data.YjbPrice == null ? 0 : (decimal)data.YjbPrice,
                                                       FreightPrice = data.FreightPrice == null ? 0 : (decimal)data.FreightPrice,
                                                       CouponPrice = data.CouponPrice == null ? 0 : (decimal)data.CouponPrice
                                                   }).ToList();

            // 获取赠品信息
            var presents = OrderItemPresent.ObjectSet().Where(_ => commodityOrderIdList.Contains(_.CommodityOrderId)).Select(data =>
                new
                {
                    OrderItemId = data.OrderItemId,
                    CommodityOrderId = data.CommodityOrderId,
                    CommodityId = data.CommodityId,
                    CommodityIdName = data.Name,
                    PicturesPath = data.PicturesPath,
                    Price = data.CurrentPrice,//取订单商品列表中的价格
                    Number = data.Number,
                    CommodityAttributes = data.CommodityAttributes,
                    CategoryName = data.CategoryNames,
                    RealPrice = data.RealPrice,
                    ComCategoryName = data.ComCategoryName
                }
            ).ToList();

            var tempOrderItemIds = orderItems.Select(_ => _.Id).ToList();
            var jdOrderItems = JdOrderItem.ObjectSet().Where(_ => tempOrderItemIds.Contains(_.CommodityOrderItemId.Value)).ToList();
            foreach (var item in orderItemsVMList)
            {
                var jdOrderItem = jdOrderItems.FirstOrDefault(_ => _.CommodityOrderItemId == item.Id);
                if (jdOrderItem != null)
                {
                    item.JdState = jdOrderItem.State;
                    item.JdorderId = jdOrderItem.JdOrderId;
                }
                //string commodityorderid = item.CommodityOrderId.ToString();
                //var jdorderItem = JdOrderItem.ObjectSet().Where(p => p.CommodityOrderId == commodityorderid).ToList();
                //if (jdorderItem.Count() > 0)
                //{
                //    foreach (var _item in jdorderItem)
                //    {
                //        if (item.CommodityId == _item.TempId)
                //        {
                //            item.JdorderId = _item.JdOrderId;
                //        }
                //    }
                //}

                #region 赠品信息
                var currentPresents = presents.Where(_ => _.OrderItemId == item.Id).ToList();
                if (currentPresents.Count > 0)
                {
                    item.Presents = new List<OrderItemPresentsVM>();
                    foreach (var p in currentPresents)
                    {
                        item.Presents.Add(new OrderItemPresentsVM
                        {
                            ComCategoryName = p.ComCategoryName,
                            CommodityName = p.CommodityIdName,
                            Number = p.Number,
                            PicturesPath = p.PicturesPath,
                            Price = p.Price,
                            RealPrice = p.RealPrice,
                            SizeAndColorId = p.CommodityAttributes,
                        });
                    }
                }
                #endregion

                OrderRefund orderRefund = OrderRefund.ObjectSet().Where(t => t.OrderItemId == item.Id).OrderByDescending(t => t.SubTime).FirstOrDefault();
                if (orderRefund != null)
                {
                    item.RefundOrderItemId = orderRefund.OrderItemId == null ? Guid.Empty : (Guid)orderRefund.OrderItemId;
                    item.StateRefund = orderRefund.State;
                    item.RefundType = orderRefund.RefundType;
                }
                else
                {
                    var orderRefundAfterSales = OrderRefundAfterSales.ObjectSet().Where(t => t.OrderItemId == item.Id).OrderByDescending(t => t.SubTime).FirstOrDefault();
                    if (orderRefundAfterSales != null)
                    {
                        item.RefundOrderItemId = orderRefundAfterSales.OrderItemId == null ? Guid.Empty : (Guid)orderRefundAfterSales.OrderItemId;
                        item.StateAfterSales = orderRefundAfterSales.State;
                        item.RefundType = orderRefundAfterSales.RefundType;
                    }
                }
            }
            Collection collect = new Collection();

            //遍历订单
            foreach (CommodityOrderVM v in searchResult)
            {
                List<OrderItemsVM> orderItemslist = new List<OrderItemsVM>();
                //遍历订单中的商品，获取每个商品对应的颜色、尺寸属性
                foreach (OrderItemsVM model in orderItemsVMList)
                {
                    if (model.CommodityOrderId == v.CommodityOrderId)
                    {
                        orderItemslist.Add(model);
                    }
                }
                v.OrderItems = orderItemslist;
            }

            #endregion

            // 转换取消订单类型
            foreach (var item in searchResult)
            {
                if (item.CancelReasonCode.HasValue)
                {
                    item.MessageToBuyer = TypeToStringHelper.CancleOrderReasonTypeToString(item.CancelReasonCode);
                }
                // 查询厂商类型
                var mallApply = MallApply.ObjectSet().FirstOrDefault(t => t.EsAppId == item.EsAppId && t.AppId == item.AppId);
                if (mallApply != null)
                {
                    item.AppType = mallApply.GetTypeString();
                }
            }

            //获取规格设置信息
            foreach (var item in searchResult)
            {
                var orderItem = OrderItem.ObjectSet().FirstOrDefault(p => p.CommodityOrderId == item.CommodityOrderId);
                if (orderItem != null)
                {
                    item.Specifications = orderItem.Specifications ?? 0;
                }
            }

            result.Data.Data = searchResult;
            return result;
        }

        /// <summary>
        /// 订单状态变化 处理售后
        /// </summary>
        /// <param name="commodityOrder">订单信息</param>
        private void CancelTheOrderDealAfterSales(CommodityOrder commodityOrder)
        {
            Guid orderId = commodityOrder.Id;
            DateTime now = DateTime.Now;
            //金币回调处理售后状态从金和处理中->已退款
            CommodityOrderService commodityOrderService = CommodityOrderService.ObjectSet().FirstOrDefault(p => p.Id == orderId);
            if (commodityOrderService != null && commodityOrderService.State == 12)
            {
                commodityOrderService.State = 7;
                commodityOrderService.ModifiedOn = now;
                commodityOrderService.EntityState = System.Data.EntityState.Modified;

                var orderRefundAfterSales = OrderRefundAfterSales.ObjectSet().FirstOrDefault(p => p.OrderId == orderId && p.State == 12);
                if (orderRefundAfterSales != null)
                {
                    orderRefundAfterSales.State = 1;
                    orderRefundAfterSales.ModifiedOn = now;
                    orderRefundAfterSales.EntityState = System.Data.EntityState.Modified;
                }
                LogHelper.Info(string.Format("金币回调执行：outTradeId：{0}，sign:{1}，messages：{2},Payment:{3}", commodityOrder.Id, CustomConfig.PaySing, "CommodityOrderService State金和处理中12->已退款7", commodityOrder.Payment));
                //回退积分
                SignSV.CommodityOrderAfterSalesRefundScore(ContextFactory.CurrentThreadContext, commodityOrder, orderRefundAfterSales);
                // 回退易捷币
                // Jinher.AMP.BTP.TPS.Helper.YJBHelper.OrderAfterSalesRefund(ContextFactory.CurrentThreadContext, commodityOrder, orderRefundAfterSales);

                #region 回退易捷币和易捷抵用券
                decimal couponprice = 0;
                decimal couponmoney = 0;//抵用券使用总金额
                //decimal totalcouprice = 0;//保存抵用券之和(当2个或者2个以上的抵用券相加的时候)
                bool isexistsplit = false;//是否有拆单，如果有的话，整单退款的时候，退的是抵用券的使用金额
                var issplit = MainOrder.ObjectSet().Where(x => x.SubOrderId == commodityOrder.Id).FirstOrDefault();
                if (issplit != null)
                {
                    isexistsplit = true;
                }
                var useryjcoupon = YJBSV.GetUserYJCouponByOrderId(commodityOrder.Id);
                decimal cashmonye = commodityOrder.RealPrice.Value;
                if (useryjcoupon.Data != null && useryjcoupon.Data.Count > 0)
                {
                    useryjcoupon.Data = useryjcoupon.Data.OrderBy(x => x.Price).ToList();
                    var refundmoney = (orderRefundAfterSales.OrderRefundMoneyAndCoupun ?? 0);
                    foreach (var item in useryjcoupon.Data)
                    {
                        couponmoney += item.UseAmount ?? 0;
                    }
                    if (refundmoney == couponmoney + refundmoney)
                    {//全额退
                        for (int i = 0; i < useryjcoupon.Data.Count; i++)
                        {
                            if (useryjcoupon.Data[i] != null)
                            {
                                if (isexistsplit)
                                    couponprice = useryjcoupon.Data[i].UsePrice;
                                else
                                    couponprice = useryjcoupon.Data[i].Price;
                                if (i == 0)
                                {

                                }
                                else
                                {//易捷币不能循环退
                                    orderRefundAfterSales.RefundYJBMoney = 0;
                                }
                                decimal coupon = couponprice;
                                Jinher.AMP.BTP.TPS.Helper.YJBHelper.OrderAfterSalesRefund(ContextFactory.CurrentThreadContext, commodityOrder, orderRefundAfterSales, coupon, useryjcoupon.Data[i].Id);
                            }
                        }
                    }
                    else
                    {//部分退款
                        for (int i = 0; i < useryjcoupon.Data.Count; i++)
                        {
                            if (useryjcoupon.Data[i] != null)
                            {
                                if (isexistsplit)
                                    couponprice = useryjcoupon.Data[i].UsePrice;
                                else
                                    couponprice = useryjcoupon.Data[i].Price;
                                //totalcouprice += useryjcoupon.Data[i].Price;
                                if (i == 0)
                                {

                                }
                                else
                                {//易捷币不能循环退
                                    orderRefundAfterSales.RefundYJBMoney = 0;
                                }
                                decimal coupon = 0;
                                if (refundmoney - cashmonye > 0)
                                {
                                    if (refundmoney - cashmonye - couponprice >= 0)
                                    {//退款金额大于等于（实际支付金额+抵用券金额），直接返回抵用券面值
                                     //if (refundmoney - cashmonye - couponprice - orderRefundAfterSales.RefundYJBMoney < 0)
                                     //{//返还部分易捷币
                                     //    orderRefundAfterSales.RefundYJBMoney = orderRefundAfterSales.RefundYJBMoney - (refundmoney + cashmonye.Value + couponprice);
                                     //}
                                        coupon = couponprice;
                                    }
                                    else
                                    {//否则，表示不是全额退款，返还部分抵用券，易捷币返还0
                                        coupon = refundmoney - cashmonye;
                                        orderRefundAfterSales.RefundYJBMoney = 0;
                                    }
                                }
                                Jinher.AMP.BTP.TPS.Helper.YJBHelper.OrderAfterSalesRefund(ContextFactory.CurrentThreadContext, commodityOrder, orderRefundAfterSales, coupon, useryjcoupon.Data[i].Id);
                                refundmoney -= coupon;
                            }
                        }
                    }
                }
                else
                {
                    Jinher.AMP.BTP.TPS.Helper.YJBHelper.OrderAfterSalesRefund(ContextFactory.CurrentThreadContext, commodityOrder, orderRefundAfterSales, 0, Guid.Empty);
                }
                #endregion
            }
        }

        /// <summary>
        /// 创建订单日志
        /// </summary>
        /// <param name="commodityOrder">订单信息</param>
        /// <param name="userId">用户id</param>
        /// <param name="state">目标状态</param>
        /// <param name="oldState">源状态</param>
        /// <returns>订单日志</returns>
        private Journal CreateJournal(UpdateCommodityOrderParamDTO ucopDto, CommodityOrder commodityOrder, int oldState)
        {
            //订单日志
            Journal journal = new Journal();
            journal.Id = Guid.NewGuid();
            journal.Code = commodityOrder.Code;
            journal.SubId = ucopDto.userId;
            journal.SubTime = DateTime.Now;
            journal.CommodityOrderId = commodityOrder.Id;
            journal.Details = "订单状态由" + oldState + "变为" + ucopDto.targetState;
            journal.StateFrom = oldState;
            journal.StateTo = ucopDto.targetState;
            journal.IsPush = false;
            journal.OrderType = commodityOrder.OrderType;
            journal.EntityState = System.Data.EntityState.Added;

            return journal;

        }

        /// <summary>
        ///  订单状态变为2（已发货）
        /// </summary>
        /// <param name="ucopDto">参数</param>
        /// <param name="commodityOrder">订单信息</param> 
        /// <returns></returns>
        private ResultDTO UpdateOrderStateTo2(UpdateCommodityOrderParamDTO ucopDto, CommodityOrder commodityOrder)
        {
            ResultDTO result = new ResultDTO();
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            DateTime now = DateTime.Now;
            int oldState = commodityOrder.State;
            int newState = ucopDto.targetState;

            #region 退款
            var orderRefund = getOrderRefund(ucopDto.orderId);
            if (orderRefund != null)
            {
                orderRefund.State = 2;
                orderRefund.ModifiedOn = now;
            }
            #endregion

            if (!OrderSV.CanChangeState(newState, commodityOrder, orderRefund, null, null))
            {
                return new ResultDTO() { ResultCode = 1, Message = "订单状态修改错误" };
            }

            List<Commodity> needRefreshCacheCommoditys = new List<Commodity>();

            #region 库存减
            //0=>2 表示付款成功，库存减
            if (oldState == 0)
            {
                var orderitemlist = OrderItem.ObjectSet().Where(n => n.CommodityOrderId == commodityOrder.Id);
                var commodityIds = orderitemlist.Select(c => c.CommodityId).Distinct().ToList();
                var coms = Commodity.ObjectSet().Where(c => commodityIds.Contains(c.Id)).ToList();
                var hotCommodities = HotCommodity.ObjectSet().Where(c => commodityIds.Contains(c.CommodityId)).ToList();
                foreach (OrderItem orderItem in orderitemlist)
                {
                    Commodity com = coms.FirstOrDefault(c => c.Id == orderItem.CommodityId);
                    if (com == null)
                        continue;
                    com.EntityState = System.Data.EntityState.Modified;
                    com.Stock -= orderItem.Number;
                    com.ModifiedOn = now;
                    needRefreshCacheCommoditys.Add(com);

                    HotCommodity hotCommodity = hotCommodities.FirstOrDefault(c => c.CommodityId == orderItem.CommodityId);
                    if (hotCommodity != null)
                    {
                        hotCommodity.ModifiedOn = now;
                        hotCommodity.Stock = com.Stock;
                        hotCommodity.EntityState = EntityState.Modified;
                    }
                }
            }
            #endregion

            #region 订单
            commodityOrder.State = ucopDto.targetState;
            //更新发货时间
            commodityOrder.ShipmentsTime = now;
            if (commodityOrder.IsRefund == true)
            {
                commodityOrder.IsRefund = false;
            }
            commodityOrder.EntityState = EntityState.Modified;
            commodityOrder.ModifiedOn = now;
            #endregion



            #region 订单日志
            Journal journal = Journal.CreateJournal(ucopDto, commodityOrder, oldState, "商家已发货");
            contextSession.SaveObject(journal);
            #endregion

            contextSession.SaveChanges();

            #region 刷新缓存
            if (needRefreshCacheCommoditys.Any())
            {
                foreach (var needRefreshCacheCommodity in needRefreshCacheCommoditys)
                {
                    needRefreshCacheCommodity.RefreshCache(EntityState.Modified);
                }
            }
            #endregion

            #region 异步发送消息
            System.Threading.ThreadPool.QueueUserWorkItem(
                a =>
                {
                    AddMessage addmassage = new AddMessage();
                    string type = "order";
                    Guid esAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
                    addmassage.AddMessages(commodityOrder.Id.ToString(), commodityOrder.UserId.ToString(), esAppId, commodityOrder.Code, commodityOrder.State, "", type);
                });
            #endregion

            return result;
        }

        /// <summary>
        /// 商家取消订单
        /// </summary>
        /// <param name="ucopDto">参数</param>
        /// <param name="commodityOrder">订单信息</param>
        /// <returns></returns>
        private ResultDTO UpdateOrderStateTo4(UpdateCommodityOrderParamDTO ucopDto, CommodityOrder commodityOrder)
        {
            ResultDTO result = new ResultDTO();

            List<Commodity> needRefreshCacheCommoditys = new List<Commodity>();
            List<TodayPromotion> needRefreshCacheTodayPromotions = new List<TodayPromotion>();
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            DateTime now = DateTime.Now;
            int oldState = commodityOrder.State;
            int newState = ucopDto.targetState;
            if (!OrderSV.CanChangeState(newState, commodityOrder, null, null, null))
            {
                return new ResultDTO() { ResultCode = 1, Message = "订单状态修改错误" };
            }

            #region 解冻金币
            if (commodityOrder.State == 0 && commodityOrder.Payment != 1 && commodityOrder.RealPrice > 0)
            {
                UnFreezeGoldDTO unFreezeGoldDTO = new UnFreezeGoldDTO()
                {
                    BizId = commodityOrder.Id,
                    Sign = CustomConfig.PaySing
                };
                try
                {
                    var unFreezeGoldResult = Jinher.AMP.BTP.TPS.FSPSV.Instance.UnFreezeGold(unFreezeGoldDTO);
                    if (unFreezeGoldResult == null || unFreezeGoldResult.Code != 0)
                    {
                        return new ResultDTO { ResultCode = 1, Message = "订单状态无法取消" };
                    }
                    LogHelper.Info("取消订单 解冻金币,message=" + unFreezeGoldResult.Message);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(string.Format("订单状态修改服务异常,unFreezeGoldDTO：{0}", JsonHelper.JsonSerializer(unFreezeGoldDTO)), ex);
                    return new ResultDTO { ResultCode = 1, Message = "Error" };
                }
            }

            #endregion

            #region 加库存
            var orderitemlist = OrderItem.ObjectSet().Where(n => n.CommodityOrderId == commodityOrder.Id);
            if (commodityOrder.State > 0)
            {
                var commodityIds = orderitemlist.Select(c => c.CommodityId).Distinct().ToList();
                var coms = Commodity.ObjectSet().Where(n => commodityIds.Contains(n.Id)).ToList();
                foreach (OrderItem orderItem in orderitemlist)
                {
                    Commodity com = coms.FirstOrDefault(c => c.Id == orderItem.CommodityId);
                    if (com != null)
                    {
                        com.EntityState = EntityState.Modified;
                        com.Stock += orderItem.Number;
                        needRefreshCacheCommoditys.Add(com);
                    }
                }
            }
            #endregion

            #region 回退活动资源
            UserLimited.ObjectSet().Context.ExecuteStoreCommand("delete from UserLimited where CommodityOrderId='" + commodityOrder.Id + "'");
            foreach (OrderItem orderItem in orderitemlist)
            {
                Guid comId = orderItem.CommodityId;
                if (orderItem.Intensity != 10 || orderItem.DiscountPrice > 0)
                {
                    var promotionId = orderItem.PromotionId.HasValue ? orderItem.PromotionId.Value : Guid.Empty;
                    if (RedisHelper.HashContainsEntry(RedisKeyConst.ProSaleCountPrefix + promotionId.ToString(), orderItem.CommodityId.ToString()))
                    {
                        int surplusLimitBuyTotal = RedisHelper.GetHashValue<int>(RedisKeyConst.ProSaleCountPrefix + orderItem.PromotionId.Value.ToString(), orderItem.CommodityId.ToString());
                        surplusLimitBuyTotal = surplusLimitBuyTotal < 0 ? 0 : surplusLimitBuyTotal;
                        TodayPromotion to = TodayPromotion.ObjectSet().FirstOrDefault(n => n.CommodityId == comId && n.PromotionId == orderItem.PromotionId && n.EndTime > now && n.StartTime < now);
                        if (to != null)
                        {
                            to.SurplusLimitBuyTotal = surplusLimitBuyTotal;
                            to.EntityState = System.Data.EntityState.Modified;
                            needRefreshCacheTodayPromotions.Add(to);
                        }
                        PromotionItems pti = PromotionItems.ObjectSet().FirstOrDefault(n => n.PromotionId == orderItem.PromotionId && n.CommodityId == comId);
                        if (pti != null)
                        {
                            pti.SurplusLimitBuyTotal = surplusLimitBuyTotal;
                            pti.EntityState = System.Data.EntityState.Modified;
                        }
                    }
                    else  //缓存中没有，直接改库
                    {
                        TodayPromotion to = TodayPromotion.GetCurrentPromotion(comId);
                        if (to != null)
                        {

                            to.SurplusLimitBuyTotal -= orderItem.Number;
                            to.SurplusLimitBuyTotal = to.SurplusLimitBuyTotal < 0 ? 0 : to.SurplusLimitBuyTotal;
                            to.EntityState = System.Data.EntityState.Modified;
                            needRefreshCacheTodayPromotions.Add(to);
                        }
                        PromotionItems pti = PromotionItems.ObjectSet().FirstOrDefault(n => n.PromotionId == orderItem.PromotionId && n.CommodityId == comId);
                        if (pti != null)
                        {
                            pti.SurplusLimitBuyTotal -= orderItem.Number;
                            pti.SurplusLimitBuyTotal = pti.SurplusLimitBuyTotal < 0 ? 0 : pti.SurplusLimitBuyTotal;
                            pti.EntityState = System.Data.EntityState.Modified;
                        }
                    }
                }
            }
            #endregion
            #region 回退积分
            SignSV.CommodityOrderCancelSrore(ContextFactory.CurrentThreadContext, commodityOrder);
            #endregion

            // 回退易捷币
            Jinher.AMP.BTP.TPS.Helper.YJBHelper.CancelOrder(ContextFactory.CurrentThreadContext, commodityOrder);

            //回退优惠券
            CouponSV.RefundCoupon(ContextFactory.CurrentThreadContext, commodityOrder);

            //给易捷卡退款
            Jinher.AMP.BTP.TPS.Helper.YJBHelper.RetreatYjc(commodityOrder.UserId, commodityOrder.YJCardPrice == null ? 0 : Convert.ToDecimal(commodityOrder.YJCardPrice), ucopDto.orderId, ucopDto.orderItemId);


            #region 订单
            commodityOrder.State = ucopDto.targetState;
            commodityOrder.ConfirmTime = now;
            commodityOrder.ModifiedOn = now;
            commodityOrder.EntityState = EntityState.Modified;
            #endregion

            #region 订单日志
            Journal journal = Journal.CreateJournal(ucopDto, commodityOrder, oldState, "取消订单");
            contextSession.SaveObject(journal);
            #endregion

            contextSession.SaveChanges();

            #region 刷新缓存
            if (needRefreshCacheCommoditys.Any())
            {
                foreach (var needRefreshCacheCommodity in needRefreshCacheCommoditys)
                {
                    needRefreshCacheCommodity.RefreshCache(EntityState.Modified);
                }
            }
            if (needRefreshCacheTodayPromotions.Any())
            {
                needRefreshCacheTodayPromotions.ForEach(c => c.RefreshCache(EntityState.Modified));
            }
            #endregion

            #region 异步发送消息
            System.Threading.ThreadPool.QueueUserWorkItem(
                a =>
                {
                    AddMessage addmassage = new AddMessage();
                    string type = "order";
                    string messages = string.Format("您的订单号：{0}的订单由于“{1}”,商家已取消订单,请关注！", commodityOrder.Code, ucopDto.remessage);
                    Guid esAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
                    addmassage.AddMessages(commodityOrder.Id.ToString(), commodityOrder.UserId.ToString(), esAppId, commodityOrder.Code, commodityOrder.State, messages, type);
                });
            #endregion

            return result;
        }

        /// <summary>
        /// 更新订单状态（目标状态：已发货退款中商家同意退款，商家未收到货=10）
        /// </summary>
        /// <param name="ucopDto">参数</param>
        /// <param name="commodityOrder">订单信息</param>
        /// <returns></returns>
        private ResultDTO UpdateOrderStateTo10(UpdateCommodityOrderParamDTO ucopDto, CommodityOrder commodityOrder)
        {
            if (ucopDto.orderItemId != Guid.Empty)
            {
                return UpdateOrderItemStateTo10(ucopDto, commodityOrder);
            }
            ResultDTO result = new ResultDTO();
            DateTime now = DateTime.Now;
            int oldState = commodityOrder.State;
            int newState = ucopDto.targetState;

            var orderRefund = getOrderRefund(commodityOrder.Id);
            if (orderRefund == null)
            {
                LogHelper.Error(string.Format("CommodityOrderBP.UpdateOrderStateTo10未找到退款信息,OrderId:{0}, oldState:{1}", commodityOrder.Id, oldState));
                return new ResultDTO { ResultCode = 1, Message = "退款信息有误" };
            }

            if (!OrderSV.CanChangeState(newState, commodityOrder, orderRefund, null, null))
            {
                return new ResultDTO() { ResultCode = 1, Message = "订单状态修改错误" };
            }
            ContextSession contextSession = ContextFactory.CurrentThreadContext;

            //达成协议退款/退货协议
            commodityOrder.AgreementTime = now;
            commodityOrder.State = ucopDto.targetState;
            commodityOrder.ModifiedOn = now;
            commodityOrder.EntityState = EntityState.Modified;


            //对原状态为出库中退款中的特殊处理
            if (oldState == 14)
                orderRefund.AgreeFlag = 0;
            else
                orderRefund.AgreeFlag = 1;
            orderRefund.RefuseTime = DateTime.Now;
            orderRefund.State = 10;
            orderRefund.ModifiedOn = now;

            if (ucopDto.RejectFreightMoney > 0)
            {
                orderRefund.RejectFreightMoney = ucopDto.RejectFreightMoney;
            }

            CancelTheOrderDealAfterSales(commodityOrder);

            //订单日志
            Journal journal = Journal.CreateJournal(ucopDto, commodityOrder, oldState, "达成退款/退货协议");
            contextSession.SaveObject(journal);

            contextSession.SaveChanges();

            if (ucopDto.targetState == 21)
            {
                //添加退货物流跟踪信息
                RefundExpressTraceBP bp = new RefundExpressTraceBP();
                Jinher.AMP.BTP.Deploy.RefundExpressTraceDTO retd = new Deploy.RefundExpressTraceDTO();
                retd.OrderId = ucopDto.orderId;
                retd.AgreeRefundTime = DateTime.Now;
                bp.AddRefundExpressTraceExt(retd);
            }
            #region 异步发送消息
            System.Threading.ThreadPool.QueueUserWorkItem(
                a =>
                {
                    AddMessage addmassage = new AddMessage();
                    string type = "order";
                    string messages = string.Format("您的订单{0}商家已同意退款/退货，请在7天内发货！", commodityOrder.Code);
                    Guid esAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
                    addmassage.AddMessages(commodityOrder.Id.ToString(), commodityOrder.UserId.ToString(), esAppId, commodityOrder.Code, commodityOrder.State, messages, type);
                });
            #endregion

            return result;
        }


        /// <summary>
        /// 更新订单状态（目标状态：已发货退款中商家同意退款，商家未收到货=10） 单商品退款/退货
        /// </summary>
        /// <param name="ucopDto">参数</param>
        /// <param name="commodityOrder">订单信息</param>
        /// <returns></returns>
        private ResultDTO UpdateOrderItemStateTo10(UpdateCommodityOrderParamDTO ucopDto, CommodityOrder commodityOrder)
        {
            ResultDTO result = new ResultDTO();
            DateTime now = DateTime.Now;
            int oldState = commodityOrder.State;

            var orderItem = OrderItem.FindByID(ucopDto.orderItemId);
            var orderRefund = OrderRefund.ObjectSet().Where(t => t.OrderItemId == ucopDto.orderItemId).OrderByDescending(t => t.SubTime).FirstOrDefault();
            if (orderRefund == null)
            {
                LogHelper.Error(string.Format("CommodityOrderBP.UpdateOrderItemStateTo10未找到退款信息,ucopDto.orderItemId:{0}", ucopDto.orderItemId));
                return new ResultDTO { ResultCode = 1, Message = "退款信息有误" };
            }
            //达成协议的订单 进行退款
            if (orderItem.State == 3 || orderRefund.RefundType == 0)
            {
                return UpdateOrderItemStateTo7(ucopDto, commodityOrder);
            }
            ContextSession contextSession = ContextFactory.CurrentThreadContext;

            //对原状态为出库中退款中的特殊处理
            orderRefund.AgreeFlag = oldState == 14 ? 0 : 1;
            orderRefund.RefuseTime = DateTime.Now;
            orderRefund.State = 10;
            orderRefund.ModifiedOn = now;
            orderRefund.EntityState = EntityState.Modified;
            contextSession.SaveObject(orderRefund);

            orderItem.State = 3;
            orderItem.ModifiedOn = DateTime.Now;
            orderItem.EntityState = EntityState.Modified;
            contextSession.SaveObject(orderItem);

            //订单日志
            Journal journal = Journal.CreateJournal(ucopDto, commodityOrder, oldState, "达成退款/退货协议");
            contextSession.SaveObject(journal);
            contextSession.SaveChanges();

            if (ucopDto.targetState == 21)
            {
                //添加退货物流跟踪信息
                RefundExpressTraceBP bp = new RefundExpressTraceBP();
                Jinher.AMP.BTP.Deploy.RefundExpressTraceDTO retd = new Deploy.RefundExpressTraceDTO();
                retd.OrderId = ucopDto.orderId;
                retd.AgreeRefundTime = DateTime.Now;
                retd.OrderItemId = ucopDto.orderItemId;
                bp.AddRefundExpressTraceExt(retd);
            }
            #region 异步发送消息
            System.Threading.ThreadPool.QueueUserWorkItem(
                a =>
                {
                    AddMessage addmassage = new AddMessage();
                    string type = "order";
                    string messages = string.Format("您的订单{0}商家已同意退款/退货，请在7天内发货！", commodityOrder.Code);
                    Guid esAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
                    addmassage.AddMessages(commodityOrder.Id.ToString(), commodityOrder.UserId.ToString(), esAppId, commodityOrder.Code, commodityOrder.State, messages, type);
                });
            #endregion

            return result;
        }


        /// <summary>
        ///  更新订单状态（目标状态：出库中=13）
        /// </summary>
        /// <param name="ucopDto">参数</param>
        /// <param name="commodityOrder">订单信息</param>
        /// <returns></returns>
        private ResultDTO UpdateOrderStateTo13(UpdateCommodityOrderParamDTO ucopDto, CommodityOrder commodityOrder)
        {
            ResultDTO result = new ResultDTO();

            DateTime now = DateTime.Now;
            ContextSession contextSession = ContextFactory.CurrentThreadContext;

            int oldState = commodityOrder.State;
            int newState = ucopDto.targetState;
            if (!OrderSV.CanChangeState(newState, commodityOrder, null, null, null))
            {
                return new ResultDTO() { ResultCode = 1, Message = "订单状态修改错误" };
            }

            #region 订单
            commodityOrder.State = ucopDto.targetState;
            //更新出库时间
            commodityOrder.ShipmentsTime = now;
            commodityOrder.EntityState = EntityState.Modified;
            commodityOrder.ModifiedOn = now;
            #endregion

            #region 日志
            Journal journal = Journal.CreateJournal(ucopDto, commodityOrder, oldState, "商家商品已出库");
            contextSession.SaveObject(journal);
            #endregion

            contextSession.SaveChanges();

            #region 异步发送消息
            System.Threading.ThreadPool.QueueUserWorkItem(
                a =>
                {
                    AddMessage addmassage = new AddMessage();
                    string type = "order";
                    string messages = string.Format("您的{0}订单正在出库，即将安排发货，请关注~", commodityOrder.Code);
                    Guid esAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
                    addmassage.AddMessages(commodityOrder.Id.ToString(), commodityOrder.UserId.ToString(), esAppId, commodityOrder.Code, commodityOrder.State, messages, type);
                });
            #endregion

            return result;
        }


        /// <summary>
        ///  更新订单状态（目标状态：确认收货=3）
        /// </summary>
        /// <param name="ucopDto">参数</param>
        /// <param name="commodityOrder">订单信息</param>
        /// <param name="orderRefund">退款信息</param> 
        /// <returns></returns>
        private ResultDTO UpdateOrderStateTo3(UpdateCommodityOrderParamDTO ucopDto, CommodityOrder commodityOrder)
        {
            ResultDTO result = new ResultDTO();
            DateTime now = DateTime.Now;
            List<Commodity> needRefreshCacheCommoditys = new List<Commodity>();
            int oldState = commodityOrder.State;
            int newState = ucopDto.targetState;

            #region 退款
            var orderRefund = getOrderRefund(commodityOrder.Id);
            if (orderRefund != null)
            {
                orderRefund.State = 2;
                orderRefund.ModifiedOn = now;
                orderRefund.EntityState = EntityState.Modified;
            }
            #endregion

            if (!OrderSV.CanChangeState(newState, commodityOrder, orderRefund, null, null))
            {
                return new ResultDTO() { ResultCode = 1, Message = "订单状态修改错误" };
            }

            ContextSession contextSession = ContextFactory.CurrentThreadContext;

            #region 销量加
            var orderitemlist = OrderItem.ObjectSet().Where(n => n.CommodityOrderId == commodityOrder.Id);
            var commodityIds = orderitemlist.Select(c => c.CommodityId).Distinct().ToList();
            var coms = Commodity.ObjectSet().Where(c => commodityIds.Contains(c.Id)).ToList();
            foreach (OrderItem items in orderitemlist)
            {
                Guid comId = items.CommodityId;
                Commodity com = coms.FirstOrDefault(n => n.Id == comId);
                if (com == null)
                    continue;
                com.EntityState = System.Data.EntityState.Modified;
                com.Salesvolume += items.Number;
                com.ModifiedOn = now;
                needRefreshCacheCommoditys.Add(com);
            }
            #endregion

            #region 订单
            //更新确认收货时间
            commodityOrder.ConfirmTime = now;
            commodityOrder.State = ucopDto.targetState;
            commodityOrder.ModifiedOn = now;
            commodityOrder.EntityState = System.Data.EntityState.Modified;
            #endregion

            #region 订单日志
            Journal journal = Journal.CreateJournal(ucopDto, commodityOrder, oldState, "商家确认收货");
            contextSession.SaveObject(journal);
            #endregion

            contextSession.SaveChanges();

            #region 刷新缓存
            if (needRefreshCacheCommoditys.Any())
            {
                foreach (var needRefreshCacheCommodity in needRefreshCacheCommoditys)
                {
                    needRefreshCacheCommodity.RefreshCache(EntityState.Modified);
                }
            }
            #endregion

            return result;
        }
        /// <summary>
        /// 更新订单状态（目标状态：已发货退款中商家同意退款，商家未收到货=10）
        /// </summary>
        /// <param name="ucopDto">参数</param>
        /// <param name="commodityOrder">订单信息</param>
        /// <returns></returns>
        private ResultDTO UpdateOrderStateTo19(UpdateCommodityOrderParamDTO ucopDto, CommodityOrder commodityOrder)
        {
            ResultDTO result = new ResultDTO();
            DateTime now = DateTime.Now;
            int oldState = commodityOrder.State;
            if (oldState != 18)
            {
                return new ResultDTO() { ResultCode = 1, Message = "订单状态修改错误" };
            }
            ContextSession contextSession = ContextFactory.CurrentThreadContext;

            commodityOrder.State = ucopDto.targetState;
            commodityOrder.ModifiedOn = now;
            commodityOrder.EntityState = EntityState.Modified;
            //订单日志
            Journal journal = Journal.CreateJournal(ucopDto, commodityOrder, oldState, "处理订单");
            contextSession.SaveObject(journal);

            contextSession.SaveChanges();
            return result;
        }

        /// <summary>
        /// 更新订单状态（目标状态：商家取消订单退款=7）
        /// </summary>
        /// <param name="ucopDto">参数</param>
        /// <param name="commodityOrder">订单信息</param>
        /// <returns></returns>
        private ResultDTO UpdateOrderStateTo20(UpdateCommodityOrderParamDTO ucopDto, CommodityOrder commodityOrder)
        {
            ResultDTO result = new ResultDTO();
            DateTime now = DateTime.Now;
            int oldState = commodityOrder.State;
            int newState = ucopDto.targetState;
            if (oldState != 18)
            {
                return new ResultDTO() { ResultCode = 1, Message = "订单状态修改错误" };
            }

            OrderRefund orderRefund = GetCYOrderRefund(commodityOrder.Id);

            if (orderRefund == null)
            {
                orderRefund = new OrderRefund();
                orderRefund.Id = Guid.NewGuid();
                orderRefund.DataType = "0";
                orderRefund.EntityState = EntityState.Added;
                orderRefund.IsFullRefund = true;
                orderRefund.ModifiedId = ucopDto.userId;
                orderRefund.ModifiedOn = DateTime.Now;
                orderRefund.OrderId = ucopDto.orderId;
                orderRefund.Receiver = ucopDto.Receiver;
                orderRefund.ReceiverAccount = ucopDto.ReceiverAccount;
                orderRefund.RefundDesc = ucopDto.remessage;
                orderRefund.RefundMoney = commodityOrder.RealPrice.HasValue ? commodityOrder.RealPrice.Value : 0;
                orderRefund.RefundReason = ucopDto.remessage;
                orderRefund.RefundType = 0;
                orderRefund.State = ucopDto.targetState;
                orderRefund.SubId = ucopDto.userId;
                orderRefund.SubTime = DateTime.Now;
            }

            ContextSession contextSession = ContextFactory.CurrentThreadContext;

            try
            {
                ReturnInfoDTO goldPayresult = new ReturnInfoDTO();
                decimal orRefundMoney = orderRefund.RefundMoney;
                if (orRefundMoney > 0)
                {
                    LogHelper.Info("订单：" + commodityOrder.Id + ",退款金额:" + orRefundMoney);

                    ContextDTO contextDTO = Jinher.AMP.BTP.Common.AuthorizeHelper.CoinInitAuthorizeInfo();

                    Jinher.AMP.App.Deploy.CustomDTO.AppIdOwnerIdTypeDTO applicationDTO =
                        APPSV.Instance.GetAppOwnerInfo(commodityOrder.AppId, contextDTO);
                    if (applicationDTO == null || applicationDTO.OwnerId == Guid.Empty)
                        return new ResultDTO { ResultCode = 1, Message = "Error" };

                    var cancelPayDto = OrderSV.BuildCancelPayDTO(commodityOrder, orRefundMoney, contextSession, applicationDTO);

                    //退款                    
                    goldPayresult = Jinher.AMP.BTP.TPS.FSPSV.CancelPay(cancelPayDto, BTP.Deploy.Enum.TradeTypeEnum.Direct);

                    if (goldPayresult.Code != 0)
                    {
                        LogHelper.Error(
                            string.Format(
                                "CancelTheOrderExt退款失败。outTradeId：{0}，sign:{1}，messages：{2},Payment:{3}",
                                commodityOrder.Id, CustomConfig.PaySing, goldPayresult.Message,
                                commodityOrder.Payment));
                        return new ResultDTO { ResultCode = 1, Message = goldPayresult.Message };
                    }
                    List<int> stwog = new PaySourceBP().GetSecTransWithoutGoldPaymentExt();
                    if (commodityOrder.Payment == 0 || goldPayresult.Message == "success")
                    {
                        orderRefund.State = 21;  // 餐饮订单商家取消订单，全额退款记录状态，已退款
                        commodityOrder.State = 21;  // 餐饮订单状态，已退款
                        result.Message = "退款成功";
                        result.ResultCode = 0;
                    }
                    else if (commodityOrder.Payment == 1003 /* 支付宝直接到账 */ || goldPayresult.Message == "waiting")
                    {
                        orderRefund.State = 20;  // 餐饮订单商家取消订单，全额退款记录状态，退款中
                        commodityOrder.State = 20;  // 餐饮订单状态，退款中
                        result.Message = "退款中";
                        result.ResultCode = 0;
                    }
                    LogHelper.Info(
                        string.Format("直接到账退款返回值：{0},Message:{1},commodityOrder.State:{2},   CancelPayDTO:{3}",
                                      goldPayresult.Code, goldPayresult.Message, commodityOrder.State,
                                      JsonHelper.JsonSerializer(cancelPayDto)));

                    contextSession.SaveObject(orderRefund);

                    //订单日志
                    Journal journal = Journal.CreateJournal(ucopDto, commodityOrder, oldState, "处理订单");
                    contextSession.SaveObject(journal);

                    result.isSuccess = contextSession.SaveChanges() > 0;
                    return result;
                }
                else
                {
                    LogHelper.Error(
                        string.Format(
                            "CancelTheOrderExt直接到账退款失败，退款金额应该大于0。outTradeId：{0}，sign:{1}，messages：{2},Payment:{3}",
                            commodityOrder.Id, CustomConfig.PaySing, goldPayresult.Message,
                            commodityOrder.Payment));
                    return new ResultDTO { isSuccess = false, ResultCode = 1, Message = "退款失败" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(
                    string.Format("调用直接到账退款接口失败。outTradeId：{0},Payment:{1}", commodityOrder.Id,
                                  commodityOrder.Payment), ex);
                return new ResultDTO { isSuccess = false, ResultCode = 1, Message = "Error" };
            }
        }

        /// <summary>
        /// 退款的时候 处理众筹相关的业务
        /// </summary>
        /// <param name="contextSession"></param>
        /// <param name="commodityOrder"></param>
        /// <param name="orRefundMoney"></param>
        /// <param name="now"></param>
        public long CancelCrowdfundingExt(ContextSession contextSession, CommodityOrder commodityOrder, decimal orRefundMoney, DateTime now)
        {
            long gold = 0;
            if (!OrderSV.LockOrder(commodityOrder.Id))
            {
                return gold;
            }
            try
            {
                //众筹股东表
                var UserCrowdfundingQuery =
                    UserCrowdfunding.ObjectSet()
                                    .FirstOrDefault(
                                        q => q.UserId == commodityOrder.UserId && q.AppId == commodityOrder.AppId);
                //众筹成功
                if (commodityOrder.IsCrowdfunding == 2)
                {
                    //不是众筹
                    if (UserCrowdfundingQuery == null)
                        return gold;
                    //众筹成功，且当前订单
                    UserCrowdfundingQuery.OrdersMoney -= orRefundMoney;
                    //全额退款，减去订单
                    if (commodityOrder.IsModifiedPrice)
                    {
                        //如果退款金额等于订单金额 相当于全额退款 订单数量减1
                        if (orRefundMoney == commodityOrder.RealPrice)
                        {
                            UserCrowdfundingQuery.OrderCount = UserCrowdfundingQuery.OrderCount - 1;
                        }
                    }
                    else
                    {
                        //如果退款的金额大于 不计算运费的金额的时候 按不算运费的金额计算 相当于全额退款 订单数量减1
                        if (orRefundMoney >= commodityOrder.Price)
                        {

                            UserCrowdfundingQuery.OrderCount = UserCrowdfundingQuery.OrderCount - 1;
                        }
                    }
                    UserCrowdfundingQuery.EntityState = EntityState.Modified;
                    contextSession.SaveObject(UserCrowdfundingQuery);
                    return gold;
                }

                //众筹基本表
                var CrowdfundingQuery =
                    Crowdfunding.ObjectSet().FirstOrDefault(q => q.AppId == commodityOrder.AppId && q.StartTime < now);
                if (CrowdfundingQuery != null)
                {
                    //众筹计数表 
                    var CrowdfundingCountQuery =
                        CrowdfundingCount.ObjectSet().FirstOrDefault(q => q.CrowdfundingId == CrowdfundingQuery.Id);
                    //众筹股东表

                    decimal corRealPrice = 0;
                    decimal fenRealPrice = 0;


                    decimal realPrice = commodityOrder.IsModifiedPrice
                                            ? commodityOrder.RealPrice.Value
                                            : commodityOrder.Price;

                    if (commodityOrder.IsModifiedPrice)
                    {
                        corRealPrice = orRefundMoney;
                        //如果退款金额等于订单金额 相当于全额退款 订单数量减1
                        if (orRefundMoney == commodityOrder.RealPrice)
                        {
                            UserCrowdfundingQuery.OrderCount = UserCrowdfundingQuery.OrderCount - 1;
                        }
                        else
                        {
                            //订单金额-退款 剩余部分 计算分红
                            fenRealPrice = commodityOrder.RealPrice.Value - orRefundMoney;
                        }
                    }
                    else
                    {
                        //如果退款的金额大于 不计算运费的金额的时候 按不算运费的金额计算 相当于全额退款 订单数量减1
                        if (orRefundMoney >= commodityOrder.Price)
                        {
                            corRealPrice = commodityOrder.Price;
                            UserCrowdfundingQuery.OrderCount = UserCrowdfundingQuery.OrderCount - 1;
                        }
                        else
                        {
                            //订单金额-退款 剩余部分 计算分红
                            fenRealPrice = commodityOrder.Price - orRefundMoney;
                            corRealPrice = orRefundMoney;
                        }
                    }
                    //解决众筹临界点订单退款问题，如果退款后众筹有效金额仍未改变，则不改变股东表信息
                    decimal notCfMoney = realPrice - corRealPrice - commodityOrder.CrowdfundingPrice;
                    if (notCfMoney < 0)
                    {
                        var cfMoneyless = -notCfMoney;
                        decimal afterMoney = UserCrowdfundingQuery.Money - cfMoneyless;
                        long afterShareCnt = (long)(afterMoney / CrowdfundingQuery.PerShareMoney);
                        //用户减少的股数
                        long result = UserCrowdfundingQuery.CurrentShareCount - afterShareCnt;
                        if (result > 0)
                        {
                            // 如果众筹已经成功 变成进行中
                            if (CrowdfundingQuery.State == 1)
                            {
                                CrowdfundingQuery.State = 0;
                                CrowdfundingQuery.EntityState = System.Data.EntityState.Modified;
                                contextSession.SaveObject(CrowdfundingQuery);
                            }
                            CrowdfundingCountQuery.CurrentShareCount = CrowdfundingCountQuery.CurrentShareCount - result;
                            CrowdfundingCountQuery.EntityState = System.Data.EntityState.Modified;
                            contextSession.SaveObject(CrowdfundingCountQuery);
                        }
                        else
                        {
                            result = 0;
                        }

                        UserCrowdfundingQuery.Money = afterMoney;
                        UserCrowdfundingQuery.CurrentShareCount -= result;
                        UserCrowdfundingQuery.EntityState = System.Data.EntityState.Modified;



                        if (commodityOrder.CrowdfundingPrice - orRefundMoney < 0)
                        {
                            commodityOrder.CrowdfundingPrice = 0;
                        }
                        else
                        {
                            commodityOrder.CrowdfundingPrice = commodityOrder.CrowdfundingPrice - orRefundMoney;
                        }
                    }
                    UserCrowdfundingQuery.OrdersMoney -= corRealPrice;
                    contextSession.SaveObject(UserCrowdfundingQuery);


                    if (fenRealPrice > 0)
                    {
                        //众筹股东每日统计
                        var yestorday = DateTime.Today.AddDays(-1);
                        var UserCrowdfundingDailyQuery =
                            UserCrowdfundingDaily.ObjectSet()
                                                 .Where(
                                                     q =>
                                                     q.AppId == commodityOrder.AppId && q.SettlementDate == yestorday)
                                                 .FirstOrDefault();

                        if (UserCrowdfundingDailyQuery != null)
                        {
                            gold =
                                (long)
                                (UserCrowdfundingDailyQuery.ShareCount * CrowdfundingQuery.DividendPercent * fenRealPrice *
                                 100) * 10;
                            if (gold > 0)
                            {
                                CfOrderDividend cfOrderDividend = CfOrderDividend.CreateCfOrderDividend();
                                cfOrderDividend.Gold = gold;
                                cfOrderDividend.AppId = commodityOrder.AppId;
                                cfOrderDividend.State = 0;
                                cfOrderDividend.CurrentShareCount = UserCrowdfundingDailyQuery.ShareCount;
                                cfOrderDividend.CommodityOrderId = commodityOrder.Id;
                                contextSession.SaveObject(cfOrderDividend);
                            }

                        }
                    }
                }
                return gold;
            }
            catch (Exception ex)
            {
                return gold;
            }
            finally
            {
                OrderSV.UnLockOrder(commodityOrder.Id);
            }
        }

        /// <summary>
        /// 修改订单实收总价
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <param name="price">实收总价</param>
        /// <returns></returns>
        public ResultDTO UpdateOrderPriceExt(Guid orderId, decimal price, Guid userId)
        {
            if (orderId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数错误" };
            }
            if (!OrderSV.LockOrder(orderId))
            {
                return new ResultDTO { ResultCode = 110, Message = "操作失败" };
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                DateTime now = DateTime.Now;
                CommodityOrder commodityOrder = CommodityOrder.ObjectSet().Where(n => n.Id == orderId && n.State == 0).FirstOrDefault();
                if (commodityOrder == null)
                {
                    return new ResultDTO { ResultCode = 1, Message = "不存在此类型订单" };
                }
                if (price != commodityOrder.RealPrice)
                {
                    commodityOrder.RealPrice = price;
                    commodityOrder.IsModifiedPrice = true;
                    commodityOrder.ModifiedOn = now;
                    commodityOrder.EntityState = System.Data.EntityState.Modified;
                    contextSession.SaveObject(commodityOrder);
                    contextSession.SaveChanges();

                    //保存日志
                    try
                    {
                        //订单日志
                        Journal journal = new Journal();
                        journal.Id = Guid.NewGuid();
                        journal.Name = "商家修改订单实收款";
                        journal.Code = commodityOrder.Code;
                        journal.SubTime = now;
                        journal.SubId = userId;
                        journal.Details = "订单实收款由" + commodityOrder.RealPrice + "变为" + price;
                        journal.CommodityOrderId = orderId;
                        journal.StateFrom = commodityOrder.State;
                        journal.StateTo = commodityOrder.State;
                        journal.IsPush = true;
                        journal.OrderType = commodityOrder.OrderType;

                        journal.EntityState = System.Data.EntityState.Added;
                        contextSession.SaveObject(journal);
                        contextSession.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(string.Format("商家修改订单实收款CommodityOrderBP.UpdateOrderPriceExt,记日志异常。orderId：{0}，price：{1}，userId)：{2}", orderId, price, userId), ex);
                    }
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                }
                else
                {
                    return new ResultDTO { ResultCode = 1, Message = "修改价格与原价一致" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("修改订单服务异常。orderId：{0}，price：{1}，userId：{2}", orderId, price, userId), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(orderId);
            }
        }

        public PaymentsDTO GetPaymentByOrderIdExt(Guid orderId, string paymentName)
        {
            Guid appId = CommodityOrder.ObjectSet()
                .Where(n => n.Id == orderId).Select(n => n.AppId).FirstOrDefault();
            if (appId == Guid.Empty)
            {
                return null;
            }

            //根据appid查询支付设置信息
            Payments payment = Payments.ObjectSet().
                Where(a => a.AppId == appId && a.PaymentName == paymentName).FirstOrDefault();
            if (payment == null)
            {
                return null;
            }

            return new PaymentsDTO
            {
                AliPayPartnerId = payment.AliPayPartnerId,
                AliPayPrivateKey = payment.AliPayPrivateKey,
                AliPayPublicKey = payment.AliPayPublicKey,
                AliPaySeller = payment.AliPaySeller,
            };
        }

        /// <summary>
        /// 确认订单支付(支付宝回调)
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="payment"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO ConfirmPaymentExt(Guid orderId, int payment)
        {
            if (!OrderSV.LockOrder(orderId))
            {
                return new ResultDTO { ResultCode = 110, Message = "操作失败" };
            }
            try
            {
                //ContextSession session = ContextFactory.CurrentThreadContext;
                //CommodityOrder order = CommodityOrder.ObjectSet().Where(a => a.Id == orderId).FirstOrDefault();
                //if (order != null)
                //{
                //    order.PaymentState = true;
                //    session.SaveObject(order);
                //    session.SaveChanges();
                //}
                //return new ResultDTO { ResultCode = 0, Message = "Success" };

                List<TodayPromotion> needRefreshCacheTodayPromotions = new List<TodayPromotion>();
                #region 修改订单状态
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                AddMessage addmassage = new AddMessage();
                DateTime now = DateTime.Now;

                //付款成功，状态改为1(未发货)
                int state = 1;

                //订单id
                string odid = orderId.ToString();
                string type = "order";
                CommodityOrder commodityOrder = CommodityOrder.ObjectSet()
                    .Where(n => n.Id == orderId).FirstOrDefault();

                if (commodityOrder != null)
                {
                    int oldState = commodityOrder.State;
                    //未支付
                    if (commodityOrder.State != 0)
                    {
                        return new ResultDTO { ResultCode = 1, Message = "订单状态无法支付" };
                    }
                    List<int> directArrivalPayments = new PaySourceBP().GetDirectArrivalPaymentExt();
                    if (!directArrivalPayments.Contains(payment))
                    {
                        commodityOrder.Payment = payment;
                    }
                    else
                    {
                        return new ResultDTO { ResultCode = 1, Message = "支付方式不存在" };
                    }
                    var orderitemlist = OrderItem.ObjectSet().Where(n => n.CommodityOrderId == commodityOrder.Id).ToList();

                    //处理订单商品
                    var commodityIdList = orderitemlist.Select(a => a.CommodityId).Distinct().ToList();
                    var commodityList = Commodity.ObjectSet().Where(a => commodityIdList.Contains(a.Id) && a.IsDel == false && a.State == 0).ToList();
                    #region 处理促销商品的促销库存 以及每人的限购数
                    //处理促销商品的促销库存 以及每人的限购数

                    //var compromoList = (from o in orderitemlist
                    //                    join t in TodayPromotion.ObjectSet()
                    //                      on o.CommodityId equals t.CommodityId
                    //                    join c in commodityList
                    //                      on o.CommodityId equals c.Id
                    //                    where t.EndTime > now && t.StartTime < now && o.Intensity != null && (o.Intensity != 10 || o.DiscountPrice != -1)
                    //                    select new
                    //                    {
                    //                        SurplusLimitBuyTotal = t.SurplusLimitBuyTotal,
                    //                        Number = o.Number,
                    //                        ComdityID = o.CommodityId,
                    //                        UserID = o.SubId,
                    //                        LimitBuyEach = t.LimitBuyEach,
                    //                        PromotionId = t.PromotionId,
                    //                        ID = t.Id,
                    //                        LimitBuyTotal = t.LimitBuyTotal,
                    //                        Stock = c.Stock
                    //                    }).ToList();
                    //if (compromoList != null && compromoList.Count() > 0)
                    //{
                    //    foreach (var t in compromoList)
                    //    {

                    //        if (t.LimitBuyEach != -1)
                    //        {
                    //            int sumLi = 0;
                    //            var su = UserLimited.ObjectSet().Where(p => p.UserId == t.UserID && p.PromotionId == t.PromotionId && p.CommodityId == t.ComdityID).Select(s => s.Count).ToList();

                    //            foreach (var i in su)
                    //            {
                    //                sumLi += i;
                    //            }
                    //            if (sumLi + t.Number > t.LimitBuyEach)
                    //            {
                    //                return new ResultDTO { ResultCode = 1, Message = "您已经达到商品的限购数量" };
                    //            }
                    //            else
                    //            {
                    //                UserLimited ul = new UserLimited();
                    //                ul.Id = Guid.NewGuid();
                    //                ul.UserId = t.UserID;
                    //                ul.PromotionId = (Guid)t.PromotionId;
                    //                ul.CommodityId = t.ComdityID;
                    //                ul.Count = t.Number;
                    //                ul.CreateTime = now;
                    //                ul.CommodityOrderId = orderId;
                    //                ul.EntityState = System.Data.EntityState.Added;
                    //                contextSession.SaveObject(ul);
                    //            }

                    //        }
                    //        if (t.LimitBuyTotal != -1)
                    //        {
                    //            if (t.SurplusLimitBuyTotal + t.Number > t.LimitBuyTotal)
                    //            {
                    //                return new ResultDTO { ResultCode = 1, Message = "促销商品库存不足" };
                    //            }
                    //        }
                    //        else
                    //        {
                    //            if (t.SurplusLimitBuyTotal + t.Number > t.Stock)
                    //            {
                    //                return new ResultDTO { ResultCode = 1, Message = "促销商品库存不足" };
                    //            }
                    //        }
                    //        TodayPromotion todayp = (from to in TodayPromotion.ObjectSet()
                    //                                 where to.Id == t.ID
                    //                                 select to).FirstOrDefault();
                    //        todayp.EntityState = System.Data.EntityState.Modified;
                    //        todayp.SurplusLimitBuyTotal = todayp.SurplusLimitBuyTotal + t.Number;
                    //        contextSession.SaveObject(todayp);
                    //        needRefreshCacheTodayPromotions.Add(todayp);
                    //        PromotionItems pro = (from pr in PromotionItems.ObjectSet()
                    //                              where pr.PromotionId == t.PromotionId && pr.CommodityId == t.ComdityID
                    //                              select pr).FirstOrDefault();
                    //        pro.EntityState = System.Data.EntityState.Modified;
                    //        pro.SurplusLimitBuyTotal = pro.SurplusLimitBuyTotal + t.Number;
                    //        contextSession.SaveObject(pro);

                    //    }
                    //}
                    #endregion

                    List<HotCommodity> hotCommodities = new List<HotCommodity>();
                    if (commodityList.Any())
                    {
                        var ids = commodityList.Select(m => m.Id).ToList();
                        hotCommodities =
                            HotCommodity.ObjectSet().Where(c => ids.Contains(c.Id)).Distinct().ToList();
                    }
                    commodityList.ForEach(
                        com =>
                        {
                            com.EntityState = System.Data.EntityState.Modified;
                            //减库存
                            com.Stock -= orderitemlist.Where(a => a.CommodityId == com.Id).FirstOrDefault().Number;
                            contextSession.SaveObject(com);

                            // 修改热销商品信息
                            HotCommodity hotCommodity = hotCommodities.FirstOrDefault(c => c.CommodityId == com.Id);
                            if (hotCommodity != null)
                            {
                                hotCommodity.EntityState = EntityState.Modified;
                                hotCommodity.Stock = com.Stock;
                                contextSession.SaveObject(hotCommodity);
                            }

                        });

                    commodityOrder.PaymentTime = now;
                    commodityOrder.ModifiedOn = now;
                    commodityOrder.State = state;
                    commodityOrder.EntityState = System.Data.EntityState.Modified;
                    contextSession.SaveObject(commodityOrder);

                    //提交修改
                    contextSession.SaveChanges();

                    //刷新商品缓存
                    if (commodityList.Any())
                    {
                        foreach (var commodity in commodityList)
                        {
                            commodity.RefreshCache(EntityState.Modified);
                        }
                    }
                    //刷新今日优惠缓存
                    if (needRefreshCacheTodayPromotions.Any())
                    {
                        needRefreshCacheTodayPromotions.ForEach(c => c.RefreshCache(EntityState.Modified));
                    }

                    //订单日志
                    Journal journal = new Journal();
                    journal.Id = Guid.NewGuid();
                    journal.Code = commodityOrder.Code;
                    journal.SubTime = now;
                    journal.CommodityOrderId = orderId;
                    journal.Name = "用户支付订单";
                    journal.Details = "订单状态由" + oldState + "变为" + commodityOrder.State + ",支付方式为:" + payment;
                    journal.StateFrom = oldState;
                    journal.StateTo = commodityOrder.State;
                    journal.IsPush = false;
                    journal.OrderType = commodityOrder.OrderType;
                    journal.EntityState = System.Data.EntityState.Added;
                    contextSession.SaveObject(journal);
                    contextSession.SaveChanges();

                    //发送消息
                    System.Threading.ThreadPool.QueueUserWorkItem(
                            a =>
                            {
                                Guid EsAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
                                addmassage.AddMessages(odid, commodityOrder.UserId.ToString(), EsAppId, commodityOrder.Code, state, "", type);
                                ////正品会发送消息
                                //if (new ZPHSV().CheckIsAppInZPH(commodityOrder.AppId))
                                //{
                                //    addmassage.AddMessages(odid, commodityOrder.UserId.ToString(), CustomConfig.ZPHAppId, commodityOrder.Code, state, "", type);
                                //}
                            });


                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                }
                else
                {
                    return new ResultDTO { ResultCode = 1, Message = "订单不存在" };
                }
                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("订单状态修改服务异常。orderId：{0}，payment：{1}", orderId, payment), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(orderId);
            }
        }

        /// <summary>
        /// 获取查询的所有订单号id
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<Guid> GetEsOrderIdsExt(ExportParamDTO param)
        {
            Jinher.AMP.ZPH.Deploy.CustomDTO.QueryPavilionAppParam queryPavilionAppParam = new Jinher.AMP.ZPH.Deploy.CustomDTO.QueryPavilionAppParam
            {
                Id = param.AppId,
                pageIndex = 1,
                pageSize = Int32.MaxValue
            };
            TPS.ZPHSVFacade zph = new TPS.ZPHSVFacade();
            var applist = zph.GetPavilionApp(queryPavilionAppParam).Data.Select(t => t.appId);

            List<CommodityOrder> orderList = new List<CommodityOrder>();
            if (param.State.Contains(","))
            {
                int[] arrystate = Array.ConvertAll<string, int>(param.State.Split(','), s => int.Parse(s));

                if (param.State == "1,11") //待发货 付款时间
                {
                    orderList = CommodityOrder.ObjectSet().Where(a => (applist.Contains(a.AppId) || a.AppId == param.AppId) && a.PaymentTime > param.StartDate && a.PaymentTime < param.EndDate && arrystate.Contains(a.State) && a.SelfTakeFlag == 0).OrderByDescending(a => a.PaymentTime).ToList();
                }

                else if (param.State == "8,9,10,12,14") //退款中 退款时间
                {
                    orderList = (from data1 in CommodityOrder.ObjectSet()
                                 join dataS in CommodityOrderService.ObjectSet()
                                             on data1.Id equals dataS.Id
                                 into tempS
                                 from tbS in tempS.DefaultIfEmpty()
                                 where (applist.Contains(data1.AppId) || data1.AppId == param.AppId) && ((data1.RefundTime > param.StartDate && data1.RefundTime < param.EndDate && arrystate.Contains(data1.State)) || (data1.State == 3 && tbS.SubTime > param.StartDate && tbS.SubTime < param.EndDate && (tbS.State == 5 || tbS.State == 10 || tbS.State == 12)))
                                 orderby data1.ModifiedOn descending
                                 select data1).ToList();
                    //orderList = CommodityOrder.ObjectSet().Where(a => a.AppId == param.AppId && a.RefundTime > param.StartDate && a.RefundTime < param.EndDate && arrystate.Contains(a.State)).OrderByDescending(a => a.RefundTime).ToList();
                }

                else if (param.State == "4,5,6")//交易失败 失败时间
                {
                    orderList = CommodityOrder.ObjectSet().Where(a => (applist.Contains(a.AppId) || a.AppId == param.AppId) && a.ConfirmTime > param.StartDate && a.ConfirmTime < param.EndDate && arrystate.Contains(a.State)).OrderByDescending(a => a.ConfirmTime).ToList();
                }
            }
            else if (param.State == "99") //待自提 付款时间
            {
                orderList = CommodityOrder.ObjectSet().Where(a => (applist.Contains(a.AppId) || a.AppId == param.AppId) && a.PaymentTime > param.StartDate && a.PaymentTime < param.EndDate && (a.State == 1 || a.State == 11) && a.SelfTakeFlag == 1).OrderByDescending(a => a.PaymentTime).ToList();
            }
            else
            {
                if (!string.IsNullOrEmpty(param.State))
                {
                    int _state = int.Parse(param.State);
                    if (param.State == "0")  // 待付款  提交时间
                    {
                        orderList = CommodityOrder.ObjectSet().Where(a => (applist.Contains(a.AppId) || a.AppId == param.AppId) && a.SubTime > param.StartDate && a.SubTime < param.EndDate && a.State == _state).OrderByDescending(a => a.SubTime).ToList();
                    }
                    else if (param.State == "7") //已退款 成功时间
                    {
                        orderList = (from data1 in CommodityOrder.ObjectSet()
                                     join dataS in CommodityOrderService.ObjectSet()
                                                on data1.Id equals dataS.Id
                                                into tempS
                                     from tbS in tempS.DefaultIfEmpty()
                                     where (applist.Contains(data1.AppId) || data1.AppId == param.AppId) && ((data1.ConfirmTime > param.StartDate && data1.ConfirmTime < param.EndDate && data1.State == 7) || (data1.State == 3 && tbS.State == 7 && tbS.State == 7 && tbS.ModifiedOn > param.StartDate && tbS.ModifiedOn < param.EndDate))
                                     orderby data1.ModifiedOn descending
                                     select data1).ToList();
                        //orderList = CommodityOrder.ObjectSet().Where(a => a.AppId == param.AppId && a.ConfirmTime > param.StartDate && a.ConfirmTime < param.EndDate && a.State == _state).OrderByDescending(a => a.AgreementTime).ToList();
                    }
                    else if (param.State == "3")  //交易成功 成交时间
                    {
                        orderList = (from data1 in CommodityOrder.ObjectSet()
                                     join dataS in CommodityOrderService.ObjectSet()
                                                  on data1.Id equals dataS.Id
                                                  into tempS
                                     from tbS in tempS.DefaultIfEmpty()
                                     where (applist.Contains(data1.AppId) || data1.AppId == param.AppId) && (data1.State == 3 && (tbS.State == null || tbS.State == 3 || tbS.State == 15) && data1.ConfirmTime > param.StartDate && data1.ConfirmTime < param.EndDate)
                                     orderby data1.ModifiedOn descending
                                     select data1).ToList();
                        //orderList = CommodityOrder.ObjectSet().Where(a => a.AppId == param.AppId && a.ConfirmTime > param.StartDate && a.ConfirmTime < param.EndDate && a.State == _state).OrderByDescending(a => a.ConfirmTime).ToList();
                    }
                    else if (param.State == "2")  //已发货 发货时间
                    {
                        orderList = CommodityOrder.ObjectSet().Where(a => (applist.Contains(a.AppId) || a.AppId == param.AppId) && a.ShipmentsTime > param.StartDate && a.ShipmentsTime < param.EndDate && a.State == _state).OrderByDescending(a => a.ShipmentsTime).ToList();
                    }
                    else if (param.State == "13")  //出库中 发货时间
                    {
                        orderList = CommodityOrder.ObjectSet().Where(a => (applist.Contains(a.AppId) || a.AppId == param.AppId) && a.ShipmentsTime > param.StartDate && a.ShipmentsTime < param.EndDate && a.State == _state).OrderByDescending(a => a.ShipmentsTime).ToList();
                    }
                    else if (param.State == "100") //售后完毕
                    {
                        orderList = (from data1 in CommodityOrder.ObjectSet()
                                     join refund in OrderRefund.ObjectSet() on data1.Id equals refund.OrderId
                                      into tempR
                                     from tbR in tempR.DefaultIfEmpty()
                                     join dataS in CommodityOrderService.ObjectSet()
                                                  on data1.Id equals dataS.Id
                                                  into tempS
                                     from tbS in tempS.DefaultIfEmpty()
                                     join dataRS in OrderRefundAfterSales.ObjectSet()
                                                  on data1.Id equals dataRS.OrderId
                                                  into tempRS
                                     from tbRS in tempRS.DefaultIfEmpty()
                                     where (applist.Contains(data1.AppId) || data1.AppId == param.AppId) && ((data1.State == 7 && tbR.State == 1 && tbR.IsFullRefund == false && data1.ConfirmTime > param.StartDate && data1.ConfirmTime < param.EndDate) || (tbS.State == 15 && tbS.EndTime > param.StartDate && tbS.EndTime < param.EndDate) || (tbS.State == 7 && tbRS.State == 1 && tbRS.IsFullRefund == 0 && tbS.ModifiedOn > param.StartDate && tbS.ModifiedOn < param.EndDate))
                                     orderby data1.ModifiedOn descending
                                     select data1).ToList();
                        //orderList = CommodityOrder.ObjectSet().Where(a => a.AppId == param.AppId && a.ConfirmTime > param.StartDate && a.ConfirmTime < param.EndDate && a.State == 3).OrderByDescending(a => a.ConfirmTime).ToList();
                    }
                }
                else
                {
                    orderList = CommodityOrder.ObjectSet().Where(a => (applist.Contains(a.AppId) || a.AppId == param.AppId) && a.State != 16 && a.State != 17 && a.SubTime < param.EndDate && a.SubTime > param.StartDate).OrderByDescending(a => a.SubTime).ToList();
                }
            }
            List<Guid> orderIds = new List<Guid>();
            foreach (CommodityOrder orderEntity in orderList)
            {
                orderIds.Add(orderEntity.Id);
            }

            return orderIds;
        }

        /// <summary>
        /// 获取查询的所有订单号id
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<Guid> GetOrderIdsExt(ExportParamDTO param)
        {
            IQueryable<CommodityOrder> orderList = null;
            if (param.State.Contains(","))
            {
                int[] arrystate = Array.ConvertAll<string, int>(param.State.Split(','), s => int.Parse(s));

                if (param.State == "1,11") //待发货 付款时间
                {
                    orderList = CommodityOrder.ObjectSet().Where(a => a.AppId == param.AppId && a.PaymentTime > param.StartDate && a.PaymentTime < param.EndDate && arrystate.Contains(a.State) && a.SelfTakeFlag == 0).OrderByDescending(a => a.PaymentTime);
                }

                else if (param.State == "8,9,10,12,14") //退款中 退款时间
                {
                    orderList = (from data1 in CommodityOrder.ObjectSet()
                                 join dataS in CommodityOrderService.ObjectSet()
                                             on data1.Id equals dataS.Id
                                 into tempS
                                 from tbS in tempS.DefaultIfEmpty()
                                 where data1.AppId == param.AppId && ((data1.RefundTime > param.StartDate && data1.RefundTime < param.EndDate && arrystate.Contains(data1.State)) || (data1.State == 3 && tbS.SubTime > param.StartDate && tbS.SubTime < param.EndDate && (tbS.State == 5 || tbS.State == 10 || tbS.State == 12)))
                                 orderby data1.ModifiedOn descending
                                 select data1);
                    //orderList = CommodityOrder.ObjectSet().Where(a => a.AppId == param.AppId && a.RefundTime > param.StartDate && a.RefundTime < param.EndDate && arrystate.Contains(a.State)).OrderByDescending(a => a.RefundTime).ToList();
                }

                else if (param.State == "4,5,6")//交易失败 失败时间
                {
                    orderList = CommodityOrder.ObjectSet().Where(a => a.AppId == param.AppId && a.ConfirmTime > param.StartDate && a.ConfirmTime < param.EndDate && arrystate.Contains(a.State)).OrderByDescending(a => a.ConfirmTime);
                }
            }
            else if (param.State == "99") //待自提 付款时间
            {
                orderList = CommodityOrder.ObjectSet().Where(a => a.AppId == param.AppId && a.PaymentTime > param.StartDate && a.PaymentTime < param.EndDate && (a.State == 1 || a.State == 11) && a.SelfTakeFlag == 1).OrderByDescending(a => a.PaymentTime);
            }
            else
            {
                if (!string.IsNullOrEmpty(param.State))
                {
                    int _state = int.Parse(param.State);
                    if (param.State == "0")  // 待付款  提交时间
                    {
                        orderList = CommodityOrder.ObjectSet().Where(a => a.AppId == param.AppId && a.SubTime > param.StartDate && a.SubTime < param.EndDate && a.State == _state).OrderByDescending(a => a.SubTime);
                    }
                    else if (param.State == "7") //已退款 成功时间
                    {
                        orderList = (from data1 in CommodityOrder.ObjectSet()
                                     join dataS in CommodityOrderService.ObjectSet()
                                                on data1.Id equals dataS.Id
                                                into tempS
                                     from tbS in tempS.DefaultIfEmpty()
                                     where data1.AppId == param.AppId && ((data1.ConfirmTime > param.StartDate && data1.ConfirmTime < param.EndDate && data1.State == 7) || (data1.State == 3 && tbS.State == 7 && tbS.State == 7 && tbS.ModifiedOn > param.StartDate && tbS.ModifiedOn < param.EndDate))
                                     orderby data1.ModifiedOn descending
                                     select data1);
                        //orderList = CommodityOrder.ObjectSet().Where(a => a.AppId == param.AppId && a.ConfirmTime > param.StartDate && a.ConfirmTime < param.EndDate && a.State == _state).OrderByDescending(a => a.AgreementTime).ToList();
                    }
                    else if (param.State == "3")  //交易成功 成交时间
                    {
                        orderList = (from data1 in CommodityOrder.ObjectSet()
                                     join dataS in CommodityOrderService.ObjectSet()
                                                  on data1.Id equals dataS.Id
                                                  into tempS
                                     from tbS in tempS.DefaultIfEmpty()
                                     where data1.AppId == param.AppId && (data1.State == 3 && (tbS.State == null || tbS.State == 3 || tbS.State == 15) && data1.ConfirmTime > param.StartDate && data1.ConfirmTime < param.EndDate)
                                     orderby data1.ModifiedOn descending
                                     select data1);
                        //orderList = CommodityOrder.ObjectSet().Where(a => a.AppId == param.AppId && a.ConfirmTime > param.StartDate && a.ConfirmTime < param.EndDate && a.State == _state).OrderByDescending(a => a.ConfirmTime).ToList();
                    }
                    else if (param.State == "2")  //已发货 发货时间
                    {
                        orderList = CommodityOrder.ObjectSet().Where(a => a.AppId == param.AppId && a.ShipmentsTime > param.StartDate && a.ShipmentsTime < param.EndDate && a.State == _state).OrderByDescending(a => a.ShipmentsTime);
                    }
                    else if (param.State == "13")  //出库中 发货时间
                    {
                        orderList = CommodityOrder.ObjectSet().Where(a => a.AppId == param.AppId && a.ShipmentsTime > param.StartDate && a.ShipmentsTime < param.EndDate && a.State == _state).OrderByDescending(a => a.ShipmentsTime);
                    }
                    else if (param.State == "100") //售后完毕
                    {
                        orderList = (from data1 in CommodityOrder.ObjectSet()
                                     join refund in OrderRefund.ObjectSet() on data1.Id equals refund.OrderId
                                      into tempR
                                     from tbR in tempR.DefaultIfEmpty()
                                     join dataS in CommodityOrderService.ObjectSet()
                                                  on data1.Id equals dataS.Id
                                                  into tempS
                                     from tbS in tempS.DefaultIfEmpty()
                                     join dataRS in OrderRefundAfterSales.ObjectSet()
                                                  on data1.Id equals dataRS.OrderId
                                                  into tempRS
                                     from tbRS in tempRS.DefaultIfEmpty()
                                     where data1.AppId == param.AppId && ((data1.State == 7 && tbR.State == 1 && tbR.IsFullRefund == false && data1.ConfirmTime > param.StartDate && data1.ConfirmTime < param.EndDate) || (tbS.State == 15 && tbS.EndTime > param.StartDate && tbS.EndTime < param.EndDate) || (tbS.State == 7 && tbRS.State == 1 && tbRS.IsFullRefund == 0 && tbS.ModifiedOn > param.StartDate && tbS.ModifiedOn < param.EndDate))
                                     orderby data1.ModifiedOn descending
                                     select data1);
                        //orderList = CommodityOrder.ObjectSet().Where(a => a.AppId == param.AppId && a.ConfirmTime > param.StartDate && a.ConfirmTime < param.EndDate && a.State == 3).OrderByDescending(a => a.ConfirmTime).ToList();
                    }
                }
                else
                {
                    orderList = CommodityOrder.ObjectSet().Where(a => a.AppId == param.AppId && a.State != 16 && a.State != 17 && a.SubTime < param.EndDate && a.SubTime > param.StartDate).OrderByDescending(a => a.SubTime);
                }
            }
            //List<Guid> orderIds = new List<Guid>();
            //foreach (CommodityOrder orderEntity in orderList)
            //{
            //    orderIds.Add(orderEntity.Id);
            //}
            List<Guid> oidsList = new List<Guid>();
            if (orderList != null)
            {
                oidsList = orderList.Select(a => a.Id).ToList();
            }
            return oidsList;
        }



        /// <summary>
        /// 获取查询的所有阳光餐饮的订单号id
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<Guid> GetTotalOrderIdsExt(ExportParamDTO param)
        {
            List<CommodityOrder> orderList = new List<CommodityOrder>();
            if (param.State.Contains(","))
            {
                int[] arrystate = Array.ConvertAll<string, int>(param.State.Split(','), s => int.Parse(s));

                if (param.State == "1,11") //待发货 付款时间
                {
                    orderList = CommodityOrder.ObjectSet().Where(a => param.AppIds.Contains(a.AppId) && a.PaymentTime > param.StartDate && a.PaymentTime < param.EndDate && arrystate.Contains(a.State) && a.SelfTakeFlag == 0).OrderByDescending(a => a.PaymentTime).ToList();
                }

                else if (param.State == "8,9,10,12,14") //退款中 退款时间
                {
                    orderList = (from data1 in CommodityOrder.ObjectSet()
                                 join dataS in CommodityOrderService.ObjectSet()
                                             on data1.Id equals dataS.Id
                                 into tempS
                                 from tbS in tempS.DefaultIfEmpty()
                                 where param.AppIds.Contains(data1.AppId) && ((data1.RefundTime > param.StartDate && data1.RefundTime < param.EndDate && arrystate.Contains(data1.State)) || (data1.State == 3 && tbS.SubTime > param.StartDate && tbS.SubTime < param.EndDate && (tbS.State == 5 || tbS.State == 10 || tbS.State == 12)))
                                 orderby data1.ModifiedOn descending
                                 select data1).ToList();
                    //orderList = CommodityOrder.ObjectSet().Where(a => a.AppId == param.AppId && a.RefundTime > param.StartDate && a.RefundTime < param.EndDate && arrystate.Contains(a.State)).OrderByDescending(a => a.RefundTime).ToList();
                }

                else if (param.State == "4,5,6")//交易失败 失败时间
                {
                    orderList = CommodityOrder.ObjectSet().Where(a => param.AppIds.Contains(a.AppId) && a.ConfirmTime > param.StartDate && a.ConfirmTime < param.EndDate && arrystate.Contains(a.State)).OrderByDescending(a => a.ConfirmTime).ToList();
                }
            }
            else if (param.State == "99") //待自提 付款时间
            {
                orderList = CommodityOrder.ObjectSet().Where(a => param.AppIds.Contains(a.AppId) && a.PaymentTime > param.StartDate && a.PaymentTime < param.EndDate && (a.State == 1 || a.State == 11) && a.SelfTakeFlag == 1).OrderByDescending(a => a.PaymentTime).ToList();
            }
            else
            {
                if (!string.IsNullOrEmpty(param.State))
                {
                    int _state = int.Parse(param.State);
                    if (param.State == "0")  // 待付款  提交时间
                    {
                        orderList = CommodityOrder.ObjectSet().Where(a => a.AppId == param.AppId && a.SubTime > param.StartDate && a.SubTime < param.EndDate && a.State == _state).OrderByDescending(a => a.SubTime).ToList();
                    }
                    else if (param.State == "7") //已退款 成功时间
                    {
                        orderList = (from data1 in CommodityOrder.ObjectSet()
                                     join dataS in CommodityOrderService.ObjectSet()
                                                on data1.Id equals dataS.Id
                                                into tempS
                                     from tbS in tempS.DefaultIfEmpty()
                                     where param.AppIds.Contains(data1.AppId) && ((data1.ConfirmTime > param.StartDate && data1.ConfirmTime < param.EndDate && data1.State == 7) || (data1.State == 3 && tbS.State == 7 && tbS.State == 7 && tbS.ModifiedOn > param.StartDate && tbS.ModifiedOn < param.EndDate))
                                     orderby data1.ModifiedOn descending
                                     select data1).ToList();
                        //orderList = CommodityOrder.ObjectSet().Where(a => a.AppId == param.AppId && a.ConfirmTime > param.StartDate && a.ConfirmTime < param.EndDate && a.State == _state).OrderByDescending(a => a.AgreementTime).ToList();
                    }
                    else if (param.State == "3")  //交易成功 成交时间
                    {
                        orderList = (from data1 in CommodityOrder.ObjectSet()
                                     join dataS in CommodityOrderService.ObjectSet()
                                                  on data1.Id equals dataS.Id
                                                  into tempS
                                     from tbS in tempS.DefaultIfEmpty()
                                     where param.AppIds.Contains(data1.AppId) && (data1.State == 3 && (tbS.State == null || tbS.State == 3 || tbS.State == 15) && data1.ConfirmTime > param.StartDate && data1.ConfirmTime < param.EndDate)
                                     orderby data1.ModifiedOn descending
                                     select data1).ToList();
                        //orderList = CommodityOrder.ObjectSet().Where(a => a.AppId == param.AppId && a.ConfirmTime > param.StartDate && a.ConfirmTime < param.EndDate && a.State == _state).OrderByDescending(a => a.ConfirmTime).ToList();
                    }
                    else if (param.State == "2")  //已发货 发货时间
                    {
                        orderList = CommodityOrder.ObjectSet().Where(a => param.AppIds.Contains(a.AppId) && a.ShipmentsTime > param.StartDate && a.ShipmentsTime < param.EndDate && a.State == _state).OrderByDescending(a => a.ShipmentsTime).ToList();
                    }
                    else if (param.State == "13")  //出库中 发货时间
                    {
                        orderList = CommodityOrder.ObjectSet().Where(a => param.AppIds.Contains(a.AppId) && a.ShipmentsTime > param.StartDate && a.ShipmentsTime < param.EndDate && a.State == _state).OrderByDescending(a => a.ShipmentsTime).ToList();
                    }
                    else if (param.State == "100") //售后完毕
                    {
                        orderList = (from data1 in CommodityOrder.ObjectSet()
                                     join refund in OrderRefund.ObjectSet() on data1.Id equals refund.OrderId
                                      into tempR
                                     from tbR in tempR.DefaultIfEmpty()
                                     join dataS in CommodityOrderService.ObjectSet()
                                                  on data1.Id equals dataS.Id
                                                  into tempS
                                     from tbS in tempS.DefaultIfEmpty()
                                     join dataRS in OrderRefundAfterSales.ObjectSet()
                                                  on data1.Id equals dataRS.OrderId
                                                  into tempRS
                                     from tbRS in tempRS.DefaultIfEmpty()
                                     where param.AppIds.Contains(data1.AppId) && ((data1.State == 7 && tbR.State == 1 && tbR.IsFullRefund == false && data1.ConfirmTime > param.StartDate && data1.ConfirmTime < param.EndDate) || (tbS.State == 15 && tbS.EndTime > param.StartDate && tbS.EndTime < param.EndDate) || (tbS.State == 7 && tbRS.State == 1 && tbRS.IsFullRefund == 0 && tbS.ModifiedOn > param.StartDate && tbS.ModifiedOn < param.EndDate))
                                     orderby data1.ModifiedOn descending
                                     select data1).ToList();
                        //orderList = CommodityOrder.ObjectSet().Where(a => a.AppId == param.AppId && a.ConfirmTime > param.StartDate && a.ConfirmTime < param.EndDate && a.State == 3).OrderByDescending(a => a.ConfirmTime).ToList();
                    }
                }
                else
                {
                    orderList = CommodityOrder.ObjectSet().Where(a => a.AppId == param.AppId && a.State != 16 && a.State != 17 && a.SubTime < param.EndDate && a.SubTime > param.StartDate).OrderByDescending(a => a.SubTime).ToList();
                }
            }
            List<Guid> orderIds = new List<Guid>();
            foreach (CommodityOrder orderEntity in orderList)
            {
                orderIds.Add(orderEntity.Id);
            }
            return orderIds;
        }

        /// <summary>
        /// 订单导出(excel)
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<ExportResultDTO> ExportResultExt(ExportParamDTO param)
        {
            List<ExportResultDTO> returnList = new List<ExportResultDTO>();
            if (param._orderIds == null)
            {
                PaySourceBP paySourceBP = new PaySourceBP();
                List<Guid> orderIds = new List<Guid>();
                if (!string.IsNullOrEmpty(param.orderIds))
                {
                    foreach (string strOrderId in param.orderIds.Split(','))
                    {

                        Guid guidOrderId = Guid.Empty;
                        if (Guid.TryParse(strOrderId, out guidOrderId))
                        {
                            orderIds.Add(guidOrderId);
                        }
                    }
                }
                else
                {
                    orderIds = GetOrderIdsExt(param);
                }
                List<ImportOrderDTO> orders;
                Dictionary<Guid, List<ImportOrderItemDTO>> dicOrderItem;
                GetImportOrders(orderIds, out orders, out dicOrderItem);
                List<YXExpressDetailInfoSku> YXExpressList = new List<YXExpressDetailInfoSku>();
                if (ThirdECommerceHelper.IsWangYiYanXuan(param.AppId))
                {
                    YXExpressList = YXExpressDetailInfoSku.ObjectSet().Where(p => orderIds.Contains(p.OrderId)).ToList();
                    LogHelper.Info(string.Format("获取严选物流信息条数:{0}", YXExpressList.Count));
                }
                //xiexg
                var refund = from obj in CommodityOrderRefund.ObjectSet()
                             where orderIds.Contains(obj.CommodityOrderId)
                             select new { id = obj.CommodityOrderId, money = obj.RefundAmount };

                foreach (var orderId in orderIds)
                {
                    var _order = orders.FirstOrDefault(c => c.CommodityOrderId == orderId);
                    if (_order == null)
                        continue;
                    string SNOrderCode = string.Empty;
                    if (ThirdECommerceHelper.IsSuNingYiGou(_order.AppId))
                    {
                        var orderInfo = SNOrderItem.ObjectSet().FirstOrDefault(w => w.OrderId == _order.CommodityOrderId);
                        if (orderInfo != null)
                        {
                            SNOrderCode = ",苏宁订单编号：" + orderInfo.CustomOrderId;
                        }
                    }

                    ExportResultDTO model = new ExportResultDTO();
                    model.CommodityOrderId = _order.CommodityOrderId;
                    model.Code = _order.Code + SNOrderCode;
                    model.OrdersTime = _order.SubTime;
                    model.Payer = _order.ReceiptUserName;
                    model.Postcode = _order.RecipientsZipCode;
                    model.PaymentTime = _order.PaymentTime;
                    model.remark = _order.Details;
                    model.SellersRemark = _order.SellersRemark;
                    model.Phone = _order.ReceiptPhone;
                    model.ShippingAddress = string.Format("{0}{1}{2}{3}", _order.Province, _order.City, _order.District, _order.ReceiptAddress);
                    model.PracticalPayment = _order.RealPrice;
                    model.RefundMoney = _order.RefundMoney;
                    model.Products = new List<ProductList>();
                    var singleItem = dicOrderItem.Where(c => c.Key == _order.CommodityOrderId).FirstOrDefault();
                    if (singleItem.Value != null && singleItem.Value.Count() > 0)
                    {
                        foreach (ImportOrderItemDTO product in singleItem.Value)
                        {
                            //model.PracticalPayment = product.Price;
                            model.BuyNumber = product.Number;
                            model.ProductName = product.CommodityName;

                            ProductList _product = new ProductList();
                            _product.PracticalPayment = product.Number * product.Price;
                            _product.BuyNumber = product.Number;
                            _product.ProductName = product.CommodityName;
                            _product.ProductPric = product.Price;
                            _product.ManufacturerClearingPrice = product.ManufacturerClearingPrice;
                            _product.CostPrice = product.CostPrice;
                            model.Products.Add(_product);
                        }
                    }
                    model.PaymentType = paySourceBP.GetPaymentName(_order.Payment).Replace("直接到账", "").Replace("担保交易", "");
                    model.CrowdfundingPrice = _order.CrowdfundingPrice;
                    model.Commission = _order.Commission;
                    model.Freight = _order.Freight;
                    model.GoldCoupon = _order.GoldCoupon;
                    model.CouponValue = _order.CouponValue;
                    model.GoldPrice = _order.GoldPrice;
                    model.SpreadGold = _order.SpreadGold;
                    model.OwnerShare = _order.OwnerShare;
                    model.State = _order.State;
                    model.SelfTakeFlag = _order.SelfTakeFlag;
                    model.StateAfterSales = _order.StateAfterSales;
                    model.SelfTakeAddress = _order.SelfTakeAddress;
                    model.DistributeMoney = _order.DistributeMoney;
                    model.SpendScoreMoney = _order.SpendScoreMoney;
                    model.RefundScoreMoney = _order.RefundScoreMoney;
                    model.EsAppId = _order.EsAppId;
                    model.ChannelShareMoney = _order.ChannelShareMoney;
                    if (ThirdECommerceHelper.IsWangYiYanXuan(param.AppId))
                    {
                        var YXExpressInfo = YXExpressList.FirstOrDefault(p => p.OrderId == orderId);
                        if (YXExpressInfo != null)
                        {
                            model.ExpOrderNo = YXExpressInfo.ExpressNo;
                            model.ShipExpCo = YXExpressInfo.ExpressCompany;
                        }
                    }
                    else
                    {
                        model.ExpOrderNo = _order.ExpOrderNo;
                        model.ShipExpCo = _order.ShipExpCo;
                    }
                    model.EsAppName = APPSV.GetAppName((Guid)_order.EsAppId);
                    model.AppName = APPSV.GetAppName((Guid)_order.AppId);
                    model.AppType = _order.AppType;
                    model.SpendYJBMoney = _order.SpendYJBMoney;
                    var tuple = CBCSV.GetUserNameAndCode(_order.UserId);
                    model.Uname = tuple.Item1;
                    model.Ucode = tuple.Item2;
                    model.FirstContent = _order.FirstContent;
                    model.SecondContent = _order.SecondContent;
                    model.ThirdContent = _order.ThirdContent;
                    model.JczfAmonut = refund.Where(o => o.id == orderId).ToList().Sum(o => o.money);//xiexg
                    returnList.Add(model);
                }
            }
            else
            {
                returnList = _ExportResultExt(param);
            }

            return returnList;
        }

        private List<List<Guid>> GroupBigListSmaller(List<Guid> list, int smallListMaxLength)
        {
            var result = new List<List<Guid>>();
            for (int i = 0; i < list.Count; i += smallListMaxLength)
            {
                result.Add(list.Skip(i).Take(smallListMaxLength).ToList());
            }
            return result;
        }
        /// <summary>
        /// 订单导出(excel)
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<ExportResultDTO> ExportResult1Ext(ExportParamDTO param)
        {
            List<ExportResultDTO> returnList = new List<ExportResultDTO>();
            List<Guid> orderIdsAll = param._orderIds;

            List<List<Guid>> orderIdsList = null;
            if (orderIdsAll.Count > 100)
            {
                orderIdsList = GroupBigListSmaller(orderIdsAll, 100);
            }
            else
            {
                orderIdsList = new List<List<Guid>>();
                orderIdsList.Add(orderIdsAll);
            }

            foreach (var orderIds in orderIdsList)
            {
                var commodityorder = CommodityOrder.ObjectSet().Where(p => orderIds.Contains(p.Id)).ToList();
                var searchorderIds = commodityorder.Select(x => x.Id).ToList();
                var orderItemsAll = OrderItem.ObjectSet().Where(t => searchorderIds.Contains(t.CommodityOrderId))
                                    .Select(item => new { item.CommodityOrderId, item.Name }).ToList();

                foreach (var item in commodityorder)
                {
                    ExportResultDTO model = new ExportResultDTO();
                    model.Code = item.Code;
                    model.AppType = item.AppType != null ? MallTypeHelper.GetMallTypeString((short)item.AppType) : "未知类型";
                    model.OrdersTime = item.SubTime;
                    model.Price = item.Price;
                    model.PracticalPayment = item.RealPrice;
                    model.Products = new List<ProductList>();
                    var orderItems = orderItemsAll.Where(x => x.CommodityOrderId == item.Id);
                    foreach (var orderItem in orderItems)
                    {
                        ProductList product = new ProductList { ProductName = orderItem.Name };
                        model.Products.Add(product);
                    }
                    returnList.Add(model);
                }
            }
            return returnList;
        }

        /// <summary>
        /// 订单导出(excel)(方法优化)
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<ExportResultDTO> _ExportResultExt(ExportParamDTO param)
        {
            List<ExportResultDTO> returnList = new List<ExportResultDTO>();
            PaySourceBP paySourceBP = new PaySourceBP();
            LogHelper.Debug(string.Format("存在orderis的数量{0}", param._orderIds.Count()));
            List<Guid> orderIdsAll = param._orderIds;

            List<List<Guid>> orderIdsList = null;
            if (orderIdsAll.Count > 100)
            {
                orderIdsList = GroupBigListSmaller(orderIdsAll, 100);
            }
            else
            {
                orderIdsList = new List<List<Guid>>();
                orderIdsList.Add(orderIdsAll);
            }

            List<ImportOrderDTO> orders;
            Dictionary<Guid, List<ImportOrderItemDTO>> dicOrderItem;

            foreach (var orderIds in orderIdsList)
            {
                var commodityorder = CommodityOrder.ObjectSet().Where(p => orderIds.Contains(p.Id)).ToList();

                GetImportOrders(orderIds, out orders, out dicOrderItem);
                //xiexg
                var refund = from obj in CommodityOrderRefund.ObjectSet()
                             where orderIds.Contains(obj.CommodityOrderId)
                             select new { id = obj.CommodityOrderId, money = obj.RefundAmount };
                foreach (var item in commodityorder)
                {
                    var _order = orders.FirstOrDefault(c => c.CommodityOrderId == item.Id);
                    if (_order == null)
                    {
                        continue;
                    }

                    ExportResultDTO model = new ExportResultDTO();
                    model.CommodityOrderId = item.Id;
                    model.Code = item.Code;
                    model.OrdersTime = item.SubTime;
                    model.Payer = item.ReceiptUserName;
                    model.PaymentTime = item.PaymentTime;
                    model.Phone = item.ReceiptPhone;
                    model.ShippingAddress = string.Format("{0}{1}{2}{3}", item.Province, item.City, item.District, item.ReceiptAddress);
                    model.PracticalPayment = item.RealPrice;
                    model.RefundMoney = _order.RefundMoney;
                    model.Products = new List<ProductList>();
                    var singleItem = dicOrderItem.Where(c => c.Key == _order.CommodityOrderId).FirstOrDefault();
                    if (singleItem.Value != null && singleItem.Value.Count() > 0)
                    {
                        foreach (ImportOrderItemDTO product in singleItem.Value)
                        {
                            model.BuyNumber = product.Number;
                            model.ProductName = product.CommodityName;
                            ProductList _product = new ProductList();
                            _product.PracticalPayment = product.Number * product.Price;
                            _product.BuyNumber = product.Number;
                            _product.ProductName = product.CommodityName;
                            _product.ProductPric = product.Price;
                            _product.ManufacturerClearingPrice = product.ManufacturerClearingPrice;
                            _product.CostPrice = product.CostPrice;
                            model.Products.Add(_product);
                        }
                    }
                    model.PaymentType = paySourceBP.GetPaymentName(_order.Payment).Replace("直接到账", "").Replace("担保交易", "");
                    model.Freight = _order.Freight;
                    model.GoldCoupon = _order.GoldCoupon;
                    model.CouponValue = _order.CouponValue;
                    model.State = _order.State;
                    model.SelfTakeFlag = _order.SelfTakeFlag;
                    model.StateAfterSales = _order.StateAfterSales;
                    model.RefundScoreMoney = _order.RefundScoreMoney;
                    model.ChannelShareMoney = _order.ChannelShareMoney;
                    model.AppName = item.AppName;
                    model.AppType = _order.AppType;
                    model.SpendYJBMoney = _order.SpendYJBMoney;
                    model.JczfAmonut = refund.Where(o => o.id == item.Id).ToList().Sum(o => o.money);//xiexg
                    returnList.Add(model);
                }
            }
            return returnList;
        }


        private string GetPayContentByPayState(int type)
        {
            switch (type)
            {
                case (int)Jinher.AMP.BTP.Deploy.Enum.PaymentEnum.alipay:
                    return "支付宝(直接到账)";
                case (int)Jinher.AMP.BTP.Deploy.Enum.PaymentEnum.alipayGuarantee:
                    return "支付宝(担保交易)";
                case (int)Jinher.AMP.BTP.Deploy.Enum.PaymentEnum.upay:
                    return "U付";
                case (int)Jinher.AMP.BTP.Deploy.Enum.PaymentEnum.cash:
                    return "货到付款";
                case (int)Jinher.AMP.BTP.Deploy.Enum.PaymentEnum.gold:
                    return "金币";
                default:
                    return "暂无";
            }
        }

        /// <summary>
        /// 订单导出
        /// </summary>
        /// <param name="orderIds">订单Ids</param>
        /// <returns></returns>
        public ResultDTO ImportOrderExt(List<Guid> orderIds)
        {
            if (orderIds == null || orderIds.Count == 0)
            {
                return new ResultDTO { ResultCode = 1, Message = "没有需要导出的订单" };
            }
            List<ImportOrderDTO> orders;
            Dictionary<Guid, List<ImportOrderItemDTO>> dicOrderItem;
            GetImportOrders(orderIds, out orders, out dicOrderItem);

            if (orderIds == null || orderIds.Count == 0)
            {
                return new ResultDTO { ResultCode = 1, Message = "没有需要导出的订单" };
            }

            string importPath = AppDomain.CurrentDomain.BaseDirectory + "\\importOrder\\";
            if (!Directory.Exists(importPath))
            {
                Directory.CreateDirectory(importPath);
            }
            try
            {
                foreach (string delfile in Directory.GetFiles(importPath))
                {
                    File.Delete(delfile);
                }
            }
            catch (Exception innex)
            {
                LogHelper.Error(string.Format("订单导出服务异常。importPath：{0}", importPath), innex);
            }
            string fileName = string.Format("{0}订单导出记录{1}-{2}.doc", Guid.NewGuid(), orders[0].Code, orders[orders.Count - 1].Code);
            //string fileName = string.Format("订单导出记录{0}.doc", Guid.NewGuid().ToString());
            string produceCardPath = string.Format("{0}{1}", importPath, fileName);
            object objproduceCardPath = (object)produceCardPath;

            try
            {
                XWPFDocument doc = new XWPFDocument();
            }
            catch (Exception innex)
            {
                LogHelper.Error("订单导出服务异常。", innex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            try
            {
                XWPFDocument doc = new XWPFDocument();
                using (FileStream fs = new FileStream(produceCardPath, FileMode.Create, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.GetEncoding("utf-8"));

                    int countRow = 1;
                    foreach (ImportOrderDTO order in orders)
                    {
                        if (dicOrderItem.ContainsKey(order.CommodityOrderId))
                        {
                            string SNOrderCode = string.Empty;
                            if (ThirdECommerceHelper.IsSuNingYiGou(order.AppId))
                            {
                                var orderInfo = SNOrderItem.ObjectSet().Where(w => w.OrderId == order.CommodityOrderId)
                                            .FirstOrDefault();
                                if (orderInfo != null)
                                {
                                    SNOrderCode = orderInfo.CustomOrderId;
                                }
                            }
                            var curOrderItems = dicOrderItem[order.CommodityOrderId];
                            WriteNPOIOrderItem(doc, order, curOrderItems, SNOrderCode);
                            if (countRow < orders.Count)
                            {
                                doc.CreateParagraph();
                            }
                            countRow++;
                        }
                    }
                    //设置页边距
                    doc.Document.body.sectPr = new CT_SectPr();
                    doc.Document.body.sectPr.pgMar = new CT_PageMar();
                    doc.Document.body.sectPr.pgMar.left = 200;
                    doc.Document.body.sectPr.pgMar.right = 200;

                    //文件保存                 
                    doc.Write(fs);
                    fs.Close();
                }
                return new ResultDTO { ResultCode = 0, Message = produceCardPath };
            }
            catch (Exception ex)
            {
                try
                {
                    //文件保存
                    XWPFDocument doc = new XWPFDocument();
                    doc.CreateParagraph();
                    using (FileStream sw = File.Create(produceCardPath))
                    {
                        doc.Write(sw);
                    }
                    return new ResultDTO { ResultCode = 0, Message = produceCardPath };
                }
                catch (Exception innex1)
                {
                    try
                    {
                        //文件保存
                        XWPFDocument doc = new XWPFDocument();
                        doc.CreateParagraph();
                        using (FileStream sw = File.Create(produceCardPath))
                        {
                            doc.Write(sw);
                        }
                        return new ResultDTO { ResultCode = 0, Message = produceCardPath };
                    }
                    catch (Exception innex)
                    {
                        LogHelper.Error("订单导出服务异常。", innex);
                        return new ResultDTO { ResultCode = 1, Message = "Error" };
                    }
                }
            }
        }

        public void GetImportOrders(List<Guid> orderIds, out List<ImportOrderDTO> orders, out Dictionary<Guid, List<ImportOrderItemDTO>> dicOrderItem)
        {
            List<ImportOrderDTO> norders = new List<ImportOrderDTO>();
            dicOrderItem = new Dictionary<Guid, List<ImportOrderItemDTO>>();

            string strOrderIds = string.Join(",", orderIds.ToArray());

            var queryOrder = (from data in CommodityOrder.ObjectSet()
                                  //join data1 in Invoice.ObjectSet() on data.Id equals data1.CommodityOrderId
                                  //into os
                                  //from s in os.DefaultIfEmpty()
                                  //join cos in CommodityOrderService.ObjectSet() on data.Id equals cos.Id
                                  //into commodityOrderServiceS
                                  //from dataS in commodityOrderServiceS.DefaultIfEmpty()

                                  //join opu in AppOrderPickUp.ObjectSet()
                                  //on data.Id equals opu.Id into opuOrderPickUps
                                  //from opuOrderPickUp in opuOrderPickUps.DefaultIfEmpty()
                              where orderIds.Contains(data.Id) && data.State != 16 && data.State != 17
                              orderby data.ModifiedOn descending, data.SubTime descending
                              select new ImportOrderDTO
                              {
                                  CommodityOrderId = data.Id,
                                  RealPrice = data.RealPrice,
                                  Code = data.Code,
                                  SubTime = data.SubTime,
                                  ReceiptUserName = data.ReceiptUserName,
                                  Payment = data.Payment,
                                  PaymentTime = data.PaymentTime,
                                  ReceiptPhone = data.ReceiptPhone,
                                  ReceiptAddress = data.ReceiptAddress,
                                  Details = data.Details,
                                  RecipientsZipCode = data.RecipientsZipCode,
                                  Province = data.Province,
                                  City = data.City,
                                  District = data.District,
                                  Street = data.Street,
                                  //InvoiceType = s.InvoiceType == null ? 0 : s.InvoiceType,
                                  //InvoiceTitle = s.InvoiceTitle == null ? "" : s.InvoiceTitle,
                                  InvoiceType = 0,
                                  InvoiceTitle = "",
                                  SellersRemark = data.SellersRemark,
                                  CrowdfundingPrice = data.CrowdfundingPrice,
                                  Commission = data.Commission,
                                  Freight = data.Freight,
                                  GoldCoupon = data.GoldCoupon,
                                  CouponValue = 0,
                                  GoldPrice = data.GoldPrice,
                                  SpreadGold = data.SpreadGold,
                                  OwnerShare = data.OwnerShare,
                                  State = data.State,
                                  //SelfTakeAddress = opuOrderPickUp.StsAddress,
                                  //RefundMoney = orderRefund.RefundMoney,
                                  SelfTakeFlag = data.SelfTakeFlag,
                                  //StateAfterSales = dataS.State == null ? -1 : dataS.State,
                                  OrderTime = data.PaymentTime.HasValue ? data.PaymentTime.Value : data.SubTime,
                                  DistributeMoney = data.DistributeMoney,
                                  EsAppId = data.EsAppId,
                                  ChannelShareMoney = data.ChannelShareMoney,
                                  ExpOrderNo = data.ExpOrderNo,
                                  ShipExpCo = data.ShipExpCo,
                                  UserId = data.UserId,
                                  AppId = data.AppId,
                                  FirstContent = data.FirstContent,
                                  SecondContent = data.SecondContent,
                                  ThirdContent = data.ThirdContent
                              }).ToList();

            if (queryOrder.Count() > 0)
            {
                var invoiceList = (from data in Invoice.ObjectSet()
                                   where orderIds.Contains(data.CommodityOrderId)
                                   select new { CommodityOrderId = data.CommodityOrderId, InvoiceType = data.InvoiceType, InvoiceTitle = data.InvoiceTitle }).ToList();

                var commodityOrderServiceList = (from data in CommodityOrderService.ObjectSet()
                                                 where orderIds.Contains(data.Id)
                                                 select new { Id = data.Id, State = data.State }).ToList();

                var appOrderPickUpList = (from data in AppOrderPickUp.ObjectSet()
                                          where orderIds.Contains(data.Id)
                                          select new { Id = data.Id, StsAddress = data.StsAddress }).ToList();

                foreach (var order in queryOrder)
                {
                    var invoice = invoiceList.Where(x => x.CommodityOrderId == order.CommodityOrderId).FirstOrDefault();
                    if (invoice != null)
                    {
                        order.InvoiceType = invoice.InvoiceType;
                        order.InvoiceTitle = invoice.InvoiceTitle == null ? "" : invoice.InvoiceTitle;
                    }

                    order.StateAfterSales = commodityOrderServiceList.Where(x => x.Id == order.CommodityOrderId).Select(x => x.State).FirstOrDefault();
                    order.SelfTakeAddress = appOrderPickUpList.Where(x => x.Id == order.CommodityOrderId).Select(x => x.StsAddress).FirstOrDefault();
                }


                var esAppIds = queryOrder.FirstOrDefault().EsAppId;
                var appIds = queryOrder.Select(_ => _.AppId).Distinct().ToList();
                var tempOrderIds = queryOrder.Select(_ => _.CommodityOrderId).ToList();
                var yjbInfos = YJBSV.GetOrderYJBInfoes(tempOrderIds);
                var mallApplys = MallApply.ObjectSet().Where(t => t.EsAppId == esAppIds && appIds.Contains(t.AppId)).ToList();

                foreach (var importOrderDto in queryOrder)
                {
                    // 查询易捷币抵现金额 
                    //var yjbInfo = YJBSV.GetOrderYJBInfo(importOrderDto.EsAppId, importOrderDto.CommodityOrderId);
                    //if (yjbInfo.IsSuccess)
                    //{
                    //    importOrderDto.SpendYJBMoney = yjbInfo.Data.InsteadCashAmount;
                    //}
                    if (yjbInfos.IsSuccess)
                    {
                        var yjbInfo = yjbInfos.Data.Where(_ => _.OrderId == importOrderDto.CommodityOrderId).FirstOrDefault();
                        if (yjbInfo != null)
                        {
                            importOrderDto.SpendYJBMoney = yjbInfo.InsteadCashAmount;
                        }
                    }

                    // 查询厂商类型
                    //var mallApply = MallApply.ObjectSet().FirstOrDefault(t => t.EsAppId == importOrderDto.EsAppId && t.AppId == importOrderDto.AppId);
                    var mallApply = mallApplys.Where(_ => _.AppId == importOrderDto.AppId).FirstOrDefault();
                    if (mallApply != null)
                    {
                        importOrderDto.AppType = mallApply.GetTypeString();
                    }

                    norders.Add(importOrderDto);
                }
            }

            orders = norders;

            if (orderIds == null || orderIds.Count == 0)
            {
                return;
            }

            //售中已退款
            var ordersMiddle = orders.Where(t => t.State == 7).ToList();
            //售中已退款的订单ID
            var idsMiddle = ordersMiddle.Select(t => t.CommodityOrderId).ToList();
            if (idsMiddle != null && idsMiddle.Count > 0)
            {
                var middle = (from o in OrderRefund.ObjectSet()
                              where idsMiddle.Contains(o.OrderId) && o.State == 1
                              select new OrderRefundTmp
                              {
                                  OrderId = o.OrderId,
                                  RefundMoney = o.RefundMoney,
                                  RefundScoreMoney = o.RefundScoreMoney
                              }).ToList();
                //售后订单ID
                var idMiddleList = middle.Select(t => t.OrderId).ToList();
                if (idMiddleList.Count > 0)
                {
                    foreach (var item in ordersMiddle)
                    {
                        var modelMiddle = middle.Where(t => t.OrderId == item.CommodityOrderId).FirstOrDefault();
                        if (modelMiddle != null)
                        {
                            item.RefundMoney = modelMiddle.RefundMoney;
                            item.RefundScoreMoney = modelMiddle.RefundScoreMoney;
                        }
                    }
                }
            }


            //售后已退款的订单
            var ordersAfater = orders.Where(t => t.State == 3 && t.StateAfterSales == 7).ToList();
            //售后已退款的订单ID
            var idsAfater = ordersAfater.Select(t => t.CommodityOrderId).ToList();

            if (idsAfater != null && idsAfater.Count > 0)
            {
                //选出退款金额
                //取出售后退款信息
                var afterSales = (from o in OrderRefundAfterSales.ObjectSet()
                                  where idsAfater.Contains(o.OrderId) && o.State == 1
                                  select new OrderRefundTmp
                                  {
                                      OrderId = o.OrderId,
                                      RefundMoney = o.RefundMoney,
                                      RefundScoreMoney = o.RefundScoreMoney
                                  }).ToList();

                //售后订单ID
                var idAfterSalesList = afterSales.Select(t => t.OrderId).ToList();
                if (idAfterSalesList.Count > 0)
                {
                    foreach (var item in ordersAfater)
                    {
                        var modelAfterSales = afterSales.Where(t => t.OrderId == item.CommodityOrderId).FirstOrDefault();
                        if (modelAfterSales != null)
                        {
                            item.RefundMoney = modelAfterSales.RefundMoney;
                            item.RefundScoreMoney = modelAfterSales.RefundScoreMoney;
                        }
                    }
                }
            }

            //优惠券与花费积分抵现金额 CouponValue SpendScoreCost
            var orderPayDetail = OrderPayDetail.ObjectSet().Where(t => orderIds.Contains(t.OrderId)).ToList();
            if (orderPayDetail.Count > 0)
            {
                foreach (var item in orders)
                {
                    var couponValue = orderPayDetail.Where(t => t.OrderId == item.CommodityOrderId && t.ObjectType == 1).Select(t => t.Amount).FirstOrDefault();
                    item.CouponValue = couponValue;
                    var spendScoreMoney = orderPayDetail.Where(t => t.OrderId == item.CommodityOrderId && t.ObjectType == 2).Select(t => t.Amount).FirstOrDefault();
                    item.SpendScoreMoney = spendScoreMoney;
                }
            }

            var orderItems = (from data in OrderItem.ObjectSet()
                              where orderIds.Contains(data.CommodityOrderId)
                              select new ImportOrderItemDTO
                              {
                                  CommodityOrderId = data.CommodityOrderId,
                                  CommodityName = data.Name,
                                  //CommodityCode = data1.No_Code,
                                  Price = data.CurrentPrice,//取订单商品列表中的价格
                                  Number = data.Number,
                                  CommodityAttributes = data.CommodityAttributes,
                                  CommodityId = data.CommodityId,
                                  ManufacturerClearingPrice = -1,
                                  CostPrice = data.CostPrice ?? -1
                              }).ToList();

            var commdityIds = orderItems.Select(x => x.CommodityId).ToList();

            var commditys = (from data in Commodity.ObjectSet()
                             where commdityIds.Contains(data.Id)
                             select new
                             {
                                 Id = data.Id,
                                 CommodityCode = data.No_Code,
                             }).ToList();

            foreach (var item in orderItems)
            {
                item.CommodityCode = commditys.Where(x => x.Id == item.CommodityId).Select(x => x.CommodityCode).FirstOrDefault();
            }

            //厂家结算价         
            var comIds = orderItems.Select(c => c.CommodityId).ToList();
            var dict = SettlingAccount.ObjectSet().Where(c => comIds.Contains(c.CommodityId)).ToList();
            if (dict != null && dict.Count > 0)
            {
                foreach (ImportOrderItemDTO imOrderItem in orderItems)
                {
                    imOrderItem.OrderTime = orders.Where(t => t.CommodityOrderId == imOrderItem.CommodityOrderId).Select(t => t.OrderTime).FirstOrDefault();
                    var thisSetting = dict.Where(t => t.CommodityId == imOrderItem.CommodityId && t.EffectiveTime <= imOrderItem.OrderTime).OrderByDescending(c => c.EffectiveTime).FirstOrDefault();
                    if (thisSetting != null)
                    {
                        imOrderItem.ManufacturerClearingPrice = thisSetting.ManufacturerClearingPrice;
                    }
                }
            }

            var appNames = APPSV.GetAppNameListByIds(orders.Where(_ => _.EsAppId.HasValue).Select(t => t.EsAppId.Value).Distinct().ToList());
            if (appNames.Count > 0)
            {
                foreach (ImportOrderItemDTO imOrderItem in orderItems)
                {
                    var esAppId = orders.Where(t => t.CommodityOrderId == imOrderItem.CommodityOrderId).Select(t => t.EsAppId).FirstOrDefault();
                    if (esAppId.HasValue)
                    {
                        imOrderItem.EsAppName = appNames[esAppId.Value];
                    }
                    //imOrderItem.EsAppName = APPSV.GetAppName((Guid)esAppId);

                    if (string.IsNullOrEmpty(imOrderItem.CommodityAttributes))
                    {
                        continue;
                    }
                    var comAtrts = imOrderItem.CommodityAttributes.Split(',');
                    if (comAtrts.Length == 0)
                    {
                        continue;
                    }
                    string strComAttr = string.Empty;
                    foreach (string comAttr in comAtrts)
                    {
                        if (comAttr == "null")
                        {
                            continue;
                        }
                        if (strComAttr != string.Empty)
                        {
                            strComAttr = string.Format("{0} {1}", strComAttr, comAttr);
                        }
                        else
                        {
                            strComAttr = comAttr;
                        }
                    }
                    if (strComAttr != string.Empty)
                    {
                        imOrderItem.CommodityName = string.Format("{0}({1})", imOrderItem.CommodityName, strComAttr);
                    }
                }
            }

            dicOrderItem = orderItems.GroupBy(c => c.CommodityOrderId, (key, group) => new { CommodityOrderId = key, OrderItem = group }).ToDictionary(c => c.CommodityOrderId, c => c.OrderItem.ToList());
        }

        /// <summary>
        ///    查看退款申请
        /// </summary>
        /// <param name="commodityorderId"></param>
        /// <returns></returns>
        public SubmitOrderRefundDTO GetOrderRefundExt(Guid commodityorderId)
        {
            try
            {
                var orre = (from o in OrderRefund.ObjectSet()
                            where o.OrderId == commodityorderId
                            orderby o.SubTime descending
                            select new SubmitOrderRefundDTO()
                            {
                                commodityorderId = o.OrderId,
                                RefundReason = o.RefundReason,
                                RefundMoney = o.RefundMoney,
                                RefundDesc = o.RefundDesc,
                                OrderRefundImgs = o.OrderRefundImgs,
                                State = o.State,
                                RefundType = o.RefundType,
                                RefuseTime = o.RefuseTime,
                                RefuseReason = o.RefuseReason,
                                RefundExpCo = o.RefundExpCo,
                                RefundExpOrderNo = o.RefundExpOrderNo,
                                RefundExpOrderTime = o.RefundExpOrderTime,
                                IsDelayConfirmTimeAfterSales =
                                    (o.IsDelayConfirmTimeAfterSales == true ? true : false),
                                SubTime = o.SubTime,
                                NotReceiveTime = o.NotReceiveTime,
                                RefundScoreMoney = o.RefundScoreMoney,
                                SalerRemark = o.SalerRemark,
                                RefundCouponPirce = o.RefundCouponPirce
                            }).FirstOrDefault();



                CommodityOrder commodityOrder = CommodityOrder.ObjectSet().FirstOrDefault(p => p.Id == commodityorderId);
                OrderItem orderItem = OrderItem.ObjectSet().FirstOrDefault(p => p.CommodityOrderId == commodityorderId);
                if (orderItem != null)
                {
                    orre.Specifications = orderItem.Specifications ?? 0;
                }
                orre.IsThirdECommerce = ThirdECommerceHelper.IsThirdECommerceOrder(commodityOrder.AppId, commodityOrder.Id);
                return orre;

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("查看退款申请服务异常。commodityorderId：{0}", commodityorderId), ex);
                return new SubmitOrderRefundDTO();
            }

        }

        /// <summary>
        ///    查看退款申请
        /// </summary>
        /// <param name="commodityorderId"></param>
        /// <returns></returns>
        public SubmitOrderRefundDTO GetOrderItemRefundExt(Guid commodityorderId, Guid orderItemId)
        {
            LogHelper.Debug("进入查看退款详情页面，commodityorderId：" + commodityorderId + "；orderItemId:" + orderItemId);
            try
            {
                var commodityOrder = CommodityOrder.FindByID(commodityorderId);

                var orre = new SubmitOrderRefundDTO();
                if (commodityOrder.State != 3)
                {
                    orre = (from o in OrderRefund.ObjectSet()
                            where o.OrderId == commodityorderId && o.OrderItemId == orderItemId
                            orderby o.SubTime descending
                            select new SubmitOrderRefundDTO()
                            {
                                commodityorderId = o.OrderId,
                                RefundReason = o.RefundReason,
                                RefundMoney = o.RefundMoney,
                                RefundDesc = o.RefundDesc,
                                OrderRefundImgs = o.OrderRefundImgs,
                                State = o.State,
                                RefundType = o.RefundType,
                                RefuseTime = o.RefuseTime,
                                RefuseReason = o.RefuseReason,
                                RefundExpCo = o.RefundExpCo,
                                RefundExpOrderNo = o.RefundExpOrderNo,
                                RefundExpOrderTime = o.RefundExpOrderTime,
                                IsDelayConfirmTimeAfterSales =
                                    (o.IsDelayConfirmTimeAfterSales == true ? true : false),
                                SubTime = o.SubTime,
                                NotReceiveTime = o.NotReceiveTime,
                                RefundScoreMoney = o.RefundScoreMoney,
                                SalerRemark = o.SalerRemark,
                                RefundCouponPirce = o.RefundCouponPirce
                            }).FirstOrDefault();
                    if (orre != null)
                    {
                        var orderItem = OrderItem.FindByID(orderItemId);
                        orre.Name = orderItem.Name;
                        orre.Num = orderItem.Number;
                        orre.Pic = orderItem.PicturesPath;
                        orre.CommodityAttributes = orderItem.CommodityAttributes;
                        orre.Price = (decimal)orderItem.RealPrice;
                        orre.CouponPrice = orderItem.CouponPrice == null ? 0 : (decimal)orderItem.CouponPrice;
                        orre.FreightPrice = orderItem.FreightPrice == null ? 0 : (decimal)orderItem.FreightPrice;
                        orre.ChangeFreightPrice = orderItem.ChangeFreightPrice == null ? 0 : (decimal)orderItem.ChangeFreightPrice;
                        orre.ChangeRealPrice = orderItem.ChangeRealPrice == null ? 0 : (decimal)orderItem.ChangeRealPrice;
                        orre.Specifications = orderItem.Specifications ?? 0;
                    }
                    else
                    {
                        orre = (from o in OrderRefund.ObjectSet()
                                where o.OrderId == commodityorderId
                                orderby o.SubTime descending
                                select new SubmitOrderRefundDTO()
                                {
                                    commodityorderId = o.OrderId,
                                    RefundReason = o.RefundReason,
                                    RefundMoney = o.RefundMoney,
                                    RefundDesc = o.RefundDesc,
                                    OrderRefundImgs = o.OrderRefundImgs,
                                    State = o.State,
                                    RefundType = o.RefundType,
                                    RefuseTime = o.RefuseTime,
                                    RefuseReason = o.RefuseReason,
                                    RefundExpCo = o.RefundExpCo,
                                    RefundExpOrderNo = o.RefundExpOrderNo,
                                    RefundExpOrderTime = o.RefundExpOrderTime,
                                    IsDelayConfirmTimeAfterSales =
                                        (o.IsDelayConfirmTimeAfterSales == true ? true : false),
                                    SubTime = o.SubTime,
                                    NotReceiveTime = o.NotReceiveTime,
                                    RefundScoreMoney = o.RefundScoreMoney,
                                    SalerRemark = o.SalerRemark,
                                    RefundCouponPirce = o.RefundCouponPirce
                                }).FirstOrDefault();
                    }
                }
                else
                {
                    orre = (from o in OrderRefundAfterSales.ObjectSet()
                            where o.OrderId == commodityorderId && o.OrderItemId == orderItemId
                            orderby o.SubTime descending
                            select new SubmitOrderRefundDTO()
                            {
                                commodityorderId = o.OrderId,
                                RefundReason = o.RefundReason,
                                RefundMoney = o.RefundMoney,
                                RefundDesc = o.RefundDesc,
                                OrderRefundImgs = o.OrderRefundImgs,
                                State = o.State,
                                RefundType = o.RefundType,
                                RefuseTime = o.RefuseTime,
                                RefuseReason = o.RefuseReason,
                                RefundExpCo = o.RefundExpCo,
                                RefundExpOrderNo = o.RefundExpOrderNo,
                                RefundExpOrderTime = o.RefundExpOrderTime,
                                SubTime = o.SubTime,
                                NotReceiveTime = o.NotReceiveTime,
                                RefundScoreMoney = o.RefundScoreMoney,
                                SalerRemark = o.SalerRemark,
                                RefundCouponPirce = o.RefundCouponPirce
                            }).FirstOrDefault();
                    if (orre != null)
                    {
                        var orderItem = OrderItem.FindByID(orderItemId);
                        orre.Name = orderItem.Name;
                        orre.Num = orderItem.Number;
                        orre.Pic = orderItem.PicturesPath;
                        orre.CommodityAttributes = orderItem.CommodityAttributes;
                        orre.Price = (decimal)orderItem.RealPrice;
                        orre.CouponPrice = orderItem.CouponPrice == null ? 0 : (decimal)orderItem.CouponPrice;
                        orre.FreightPrice = orderItem.FreightPrice == null ? 0 : (decimal)orderItem.FreightPrice;
                        orre.ChangeFreightPrice = orderItem.ChangeFreightPrice == null ? 0 : (decimal)orderItem.ChangeFreightPrice;
                        orre.ChangeRealPrice = orderItem.ChangeRealPrice == null ? 0 : (decimal)orderItem.ChangeRealPrice;
                        orre.Specifications = orderItem.Specifications ?? 0;
                    }
                    else
                    {
                        orre = (from o in OrderRefundAfterSales.ObjectSet()
                                where o.OrderId == commodityorderId
                                orderby o.SubTime descending
                                select new SubmitOrderRefundDTO()
                                {
                                    commodityorderId = o.OrderId,
                                    RefundReason = o.RefundReason,
                                    RefundMoney = o.RefundMoney,
                                    RefundDesc = o.RefundDesc,
                                    OrderRefundImgs = o.OrderRefundImgs,
                                    State = o.State,
                                    RefundType = o.RefundType,
                                    RefuseTime = o.RefuseTime,
                                    RefuseReason = o.RefuseReason,
                                    RefundExpCo = o.RefundExpCo,
                                    RefundExpOrderNo = o.RefundExpOrderNo,
                                    RefundExpOrderTime = o.RefundExpOrderTime,
                                    SubTime = o.SubTime,
                                    NotReceiveTime = o.NotReceiveTime,
                                    RefundScoreMoney = o.RefundScoreMoney,
                                    SalerRemark = o.SalerRemark,
                                    RefundCouponPirce = o.RefundCouponPirce
                                }).FirstOrDefault();
                    }
                }
                orre.IsThirdECommerce = ThirdECommerceHelper.IsThirdECommerceOrder(commodityOrder.AppId, commodityOrder.Id);
                return orre;

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("查看退款申请服务异常。commodityorderId：{0}", commodityorderId), ex);
                return new SubmitOrderRefundDTO();
            }

        }

        public ResultDTO ShipUpdataOrderExt(Guid commodityOrderId, string shipExpCo, string expOrderNo)
        {
            if (!OrderSV.LockOrder(commodityOrderId))
            {
                return new ResultDTO { ResultCode = 110, Message = "操作失败" };
            }
            try
            {
                List<int> stateList = new List<int>() { 1, 8, 9, 10, 13, 14 };
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                DateTime now = DateTime.Now;
                CommodityOrder commodityOrder = CommodityOrder.ObjectSet().Where(n => n.Id == commodityOrderId && stateList.Contains(n.State)).FirstOrDefault();
                if (commodityOrder == null)
                {
                    return new ResultDTO { ResultCode = 1, Message = "找不到要处理的订单" };
                }
                int oldState = commodityOrder.State;

                if (commodityOrder.State == 8 || commodityOrder.State == 14)
                {
                    //删除退款申请表里的数据 
                    var orreList = OrderRefund.ObjectSet().Where(n => n.OrderId == commodityOrderId).ToList();
                    if (orreList != null && orreList.Count > 0)
                    {
                        foreach (var orre in orreList)
                        {
                            orre.EntityState = System.Data.EntityState.Deleted;
                            contextSession.Delete(orre);
                        }
                    }
                }
                commodityOrder.State = 2;
                commodityOrder.ShipExpCo = string.IsNullOrWhiteSpace(shipExpCo) ? "" : shipExpCo.Trim();
                commodityOrder.ExpOrderNo = string.IsNullOrWhiteSpace(expOrderNo) ? "" : expOrderNo.Trim();
                commodityOrder.ExpOrderNo = commodityOrder.ExpOrderNo.Replace("+", "");

                //付款成功：库存减
                //交易成功：销量加
                //交易失败：库存加
                AddMessage addmassage = new AddMessage();
                string type = "order";


                //更新发货时间
                commodityOrder.ShipmentsTime = now;

                //发送消息，异步执行
                System.Threading.ThreadPool.QueueUserWorkItem(
                    a =>
                    {
                        Guid EsAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
                        addmassage.AddMessages(commodityOrderId.ToString(), commodityOrder.UserId.ToString(), EsAppId,
                    commodityOrder.Code, commodityOrder.State, "", type);
                        //    //正品会发送消息
                        //    if (new ZPHSV().CheckIsAppInZPH(commodityOrder.AppId))
                        //    {
                        //        addmassage.AddMessages(commodityOrderId.ToString(), commodityOrder.UserId.ToString(), CustomConfig.ZPHAppId,
                        //commodityOrder.Code, commodityOrder.State, "", type);
                        //    }
                    });
                commodityOrder.ModifiedOn = now;
                //contextSession.SaveObject(commodityOrder);

                //保存物流子表
                OrderShipping orderShipping = OrderShipping.CreateOrderShipping();
                orderShipping.OrderId = commodityOrderId;
                orderShipping.ShipExpCo = commodityOrder.ShipExpCo;
                orderShipping.ExpOrderNo = commodityOrder.ExpOrderNo;
                contextSession.SaveObject(orderShipping);
                contextSession.SaveChanges();

                //订单日志
                Journal journal = new Journal();
                journal.Id = Guid.NewGuid();
                journal.Name = "商家已发货";
                journal.Code = commodityOrder.Code;
                journal.SubTime = DateTime.Now;
                journal.SubId = commodityOrder.UserId;
                journal.Details = "订单状态由" + oldState + "变为" + commodityOrder.State;
                journal.CommodityOrderId = commodityOrderId;
                journal.StateFrom = oldState;
                journal.StateTo = commodityOrder.State;
                journal.IsPush = false;
                journal.OrderType = commodityOrder.OrderType;

                journal.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(journal);
                contextSession.SaveChanges();

                if (!string.IsNullOrEmpty(commodityOrder.ExpOrderNo))
                {
                    //TODO dzc 向第三方“快递鸟”发送物流订阅请求。  
                    OrderExpressRoute oer = new OrderExpressRoute()
                    {
                        ShipExpCo = commodityOrder.ShipExpCo,
                        ExpOrderNo = commodityOrder.ExpOrderNo
                    };
                    OrderExpressRouteBP oerBP = new OrderExpressRouteBP();
                    oerBP.SubscribeOneOrderExpressExt(oer);
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("修改订单服务异常。commodityOrderId：{0}，shipExpCo：{1}，expOrderNo：{2}", commodityOrderId, shipExpCo, expOrderNo), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(commodityOrderId);
            }

            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        public List<Guid> GetCommodityIdsByOrderIdExt(Guid orderId)
        {
            List<Guid> commodityIds = (from o in OrderItem.ObjectSet()
                                       where o.CommodityOrderId == orderId
                                       select o.CommodityId).ToList();

            return commodityIds;
        }

        /// <summary>
        /// 支付宝直接到账退款
        /// </summary>
        /// <param name="commodityorderId"></param>
        /// <param name="ReceiverAccount"></param>
        /// <param name="Receiver"></param>
        /// <param name="RefundMoney"></param>
        /// <returns></returns>
        public ResultDTO AlipayZhiTuiExt(Guid commodityorderId, string ReceiverAccount, string Receiver, decimal RefundMoney)
        {
            if (!OrderSV.LockOrder(commodityorderId))
            {
                return new ResultDTO { ResultCode = 110, Message = "操作失败" };
            }
            try
            {
                //原订单状态
                int oldState = 0;
                List<Commodity> needRefreshCacheCommoditys = new List<Commodity>();
                List<TodayPromotion> needRefreshCacheTodayPromotions = new List<TodayPromotion>();
                DateTime now = DateTime.Now;
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var commodityOrder = CommodityOrder.ObjectSet().Where(n => n.Id == commodityorderId).FirstOrDefault();
                if (commodityOrder == null)
                {
                    return new ResultDTO { ResultCode = 1, Message = "没有相应的订单" };
                }
                oldState = commodityOrder.State;
                if (commodityOrder.Payment != 2)
                {
                    return new ResultDTO { ResultCode = 1, Message = "订单支付方式不是支付宝直接到账" };
                }
                if (commodityOrder.State != 8 && commodityOrder.State != 9 && commodityOrder.State != 10)
                {
                    return new ResultDTO { ResultCode = 2, Message = "订单状态无法退款" };
                }
                List<int> rState = new List<int> { 2, 3, 4, 13 };
                var orre = OrderRefund.ObjectSet().Where(n => n.OrderId == commodityorderId && !rState.Contains(n.State)).FirstOrDefault();

                var orderitemlist = OrderItem.ObjectSet().Where(n => n.CommodityOrderId == commodityOrder.Id).ToList();
                UserLimited.ObjectSet().Context.ExecuteStoreCommand("delete from UserLimited where CommodityOrderId='" + commodityOrder.Id + "'");

                List<HotCommodity> hotCommodities = new List<HotCommodity>();
                if (orderitemlist.Any())
                {
                    var ids = orderitemlist.Select(c => c.CommodityId).ToList();
                    hotCommodities =
                        HotCommodity.ObjectSet().Where(c => ids.Contains(c.CommodityId)).Distinct().ToList();

                }

                foreach (OrderItem items in orderitemlist)
                {
                    Guid comId = items.CommodityId;
                    Commodity com = Commodity.ObjectSet().Where(n => n.Id == comId).First();
                    com.EntityState = System.Data.EntityState.Modified;
                    com.Stock += items.Number;
                    contextSession.SaveObject(com);
                    needRefreshCacheCommoditys.Add(com);
                    HotCommodity hotCommodity = hotCommodities.FirstOrDefault(c => c.CommodityId == comId);
                    if (hotCommodity != null)
                    {
                        hotCommodity.EntityState = EntityState.Modified;
                        hotCommodity.Stock = com.Stock;
                        contextSession.SaveObject(hotCommodity);
                    }

                    if (items.Intensity != 10 || items.DiscountPrice != -1)
                    {
                        TodayPromotion to = TodayPromotion.GetCurrentPromotion(comId);
                        if (to != null)
                        {
                            to.SurplusLimitBuyTotal = to.SurplusLimitBuyTotal - items.Number;
                            to.EntityState = System.Data.EntityState.Modified;
                            contextSession.SaveObject(to);
                            needRefreshCacheTodayPromotions.Add(to);
                            PromotionItems pti = PromotionItems.ObjectSet().Where(n => n.PromotionId == to.PromotionId && n.CommodityId == comId).FirstOrDefault();
                            pti.SurplusLimitBuyTotal = pti.SurplusLimitBuyTotal - items.Number;
                            pti.EntityState = System.Data.EntityState.Modified;
                            contextSession.SaveObject(pti);
                        }
                    }

                }
                commodityOrder.State = 7;
                commodityOrder.EntityState = System.Data.EntityState.Modified;
                contextSession.SaveObject(commodityOrder);

                orre.State = 1;
                orre.ReceiverAccount = ReceiverAccount;
                orre.Receiver = Receiver;
                orre.RefundMoney = RefundMoney;
                orre.EntityState = System.Data.EntityState.Modified;
                contextSession.SaveObject(orre);

                int reslult = contextSession.SaveChanges();
                if (reslult > 0)
                {

                    if (needRefreshCacheCommoditys.Any())
                    {
                        needRefreshCacheCommoditys.ForEach(c => c.RefreshCache(EntityState.Modified));
                    }
                    if (needRefreshCacheTodayPromotions.Any())
                    {
                        needRefreshCacheTodayPromotions.ForEach(c => c.RefreshCache(EntityState.Modified));
                    }

                    try
                    {
                        //订单日志
                        Journal journal = new Journal();
                        journal.Id = Guid.NewGuid();
                        journal.Name = "支付宝直接到账退款";
                        journal.Code = commodityOrder.Code;
                        journal.SubTime = DateTime.Now;
                        journal.SubId = commodityOrder.UserId;
                        journal.Details = "订单状态由" + oldState + "变为" + commodityOrder.State;
                        journal.CommodityOrderId = commodityOrder.Id;
                        journal.StateFrom = oldState;
                        journal.StateTo = commodityOrder.State;
                        journal.IsPush = false;
                        journal.OrderType = commodityOrder.OrderType;

                        journal.EntityState = System.Data.EntityState.Added;
                        contextSession.SaveObject(journal);
                        contextSession.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("支付宝直接到账退款订单记日志异常。", ex);
                    }

                    AddMessage addmassage = new AddMessage();
                    string type = "order";
                    //发送消息，异步执行
                    System.Threading.ThreadPool.QueueUserWorkItem(
                        a =>
                        {
                            //向客户端推送交易失败消息
                            string messages = string.Format("您的订单{0}已完成退款，退款金额{1}元，请确认！", commodityOrder.Code, RefundMoney);
                            Guid EsAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
                            addmassage.AddMessages(commodityorderId.ToString(), commodityOrder.UserId.ToString(), EsAppId,
                                commodityOrder.Code, commodityOrder.State, messages, type);
                            ////正品会发送消息
                            //if (new ZPHSV().CheckIsAppInZPH(commodityOrder.AppId))
                            //{
                            //    addmassage.AddMessages(commodityorderId.ToString(), commodityOrder.UserId.ToString(), CustomConfig.ZPHAppId,
                            //       commodityOrder.Code, commodityOrder.State, messages, type);
                            //}
                        });
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                }
                else
                {
                    return new ResultDTO { ResultCode = 3, Message = "退款失败" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("支付宝直接到账退款服务异常。。commodityorderId：{0}，ReceiverAccount：{1}，Receiver：{2}，RefundMoney：{3}", commodityorderId, ReceiverAccount, Receiver, RefundMoney), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(commodityorderId);
            }
        }

        public List<QryOrderTradeMoneyDTO> QryAppOrderTradeInfoExt(string appName)
        {

            List<QryOrderTradeMoneyDTO> results = new List<QryOrderTradeMoneyDTO>();
            try
            {

                List<AppSearchResultDTO> appSearchDTOs = AppBTPSearch.GetSearchResult(appName);
                if (appSearchDTOs == null || !appSearchDTOs.Any())
                    return results;
                List<Guid> appIds = appSearchDTOs.Select(x => x.AppId).ToList();
                LogHelper.Info(string.Format("QryAppOrderTradeInfoBP搜索到应用数：{0}", appSearchDTOs.Count));
                if (appIds == null || appIds.Count == 0)
                {
                    return results;
                }
                List<int> secTranPayments = new PaySourceBP().GetSecuriedTransactionPaymentExt();
                var qryOrder = (from e in CommodityOrder.ObjectSet()
                                join c in CfOrderDividend.ObjectSet() on e.Id equals c.CommodityOrderId into coms
                                from b in coms.DefaultIfEmpty()
                                where appIds.Contains(e.AppId) && e.State == 3
                                && secTranPayments.Contains(e.Payment)
                                group new { e.RealPrice, e.Commission, b.Gold } by e.AppId into g
                                select new { key = g.Key, TradeMoney = g.Sum(x => x.RealPrice - x.Commission - ((x.Gold == null ? 0 : x.Gold) * 0.1m / 100)) })
                                 .ToDictionary(c => c.key, c => c.TradeMoney);

                var refundOrder = (from e in CommodityOrder.ObjectSet()
                                   join c in CfOrderDividend.ObjectSet() on e.Id equals c.CommodityOrderId into coms
                                   from b in coms.DefaultIfEmpty()
                                   join t in OrderRefund.ObjectSet() on e.Id equals t.OrderId into reos
                                   from r in reos.DefaultIfEmpty()
                                   where appIds.Contains(e.AppId) && (e.State == 7 && r.State == 1 || e.State == 12 && r.State == 12)
                                   && secTranPayments.Contains(e.Payment)
                                   group new { e.RealPrice, e.Commission, b.Gold, r.RefundMoney } by e.AppId into g
                                   select new { key = g.Key, TradeMoney = g.Sum(x => x.RealPrice - x.Commission - ((x.Gold == null ? 0 : x.Gold) * 0.1m / 100) - (x.RefundMoney == null ? 0 : x.RefundMoney)) })
                                 .ToDictionary(c => c.key, c => c.TradeMoney);


                var payqryOrder = (from e in CommodityOrder.ObjectSet()
                                   where appIds.Contains(e.AppId) && (e.State == 1 || e.State == 2 || e.State == 8 || e.State == 9 || e.State == 11)
                                   && secTranPayments.Contains(e.Payment)
                                   group new { e.RealPrice } by e.AppId into g
                                   select new { key = g.Key, TradeMoney = g.Sum(x => x.RealPrice) })
                                 .ToDictionary(c => c.key, c => c.TradeMoney);

                Dictionary<Guid, List<QryOrderTradeMoneyDTO>> cbcUserIds = new Dictionary<Guid, List<QryOrderTradeMoneyDTO>>();
                Dictionary<Guid, List<QryOrderTradeMoneyDTO>> ebcUserIds = new Dictionary<Guid, List<QryOrderTradeMoneyDTO>>();
                foreach (AppSearchResultDTO searchDTO in appSearchDTOs)
                {

                    QryOrderTradeMoneyDTO result = new QryOrderTradeMoneyDTO();
                    result.AppId = searchDTO.AppId;
                    result.AppName = searchDTO.AppName;
                    if (qryOrder.ContainsKey(searchDTO.AppId))
                    {
                        result.TradeMoney = qryOrder[searchDTO.AppId] ?? 0;
                    }
                    if (refundOrder.ContainsKey(searchDTO.AppId))
                    {
                        result.TradeMoney += refundOrder[searchDTO.AppId] ?? 0;
                    }
                    result.TradeMoney = Math.Round(result.TradeMoney, 3);
                    if (payqryOrder.ContainsKey(searchDTO.AppId))
                    {
                        result.PayTradeMoney = payqryOrder[searchDTO.AppId] ?? 0;
                    }
                    result.PayTradeMoney = Math.Round(result.PayTradeMoney, 3);
                    result.UserId = searchDTO.OwnerId;
                    if (searchDTO.OwnerType == "0")
                    {
                        if (!cbcUserIds.ContainsKey(searchDTO.OwnerId))
                        {
                            List<QryOrderTradeMoneyDTO> cbcOrderDTOs = new List<QryOrderTradeMoneyDTO>();
                            cbcOrderDTOs.Add(result);
                            cbcUserIds.Add(searchDTO.OwnerId, cbcOrderDTOs);
                        }
                        else
                        {
                            cbcUserIds[searchDTO.OwnerId].Add(result);
                        }
                    }
                    else
                    {
                        if (!ebcUserIds.ContainsKey(searchDTO.OwnerId))
                        {
                            List<QryOrderTradeMoneyDTO> ebcOrderDTOs = new List<QryOrderTradeMoneyDTO>();
                            ebcOrderDTOs.Add(result);
                            ebcUserIds.Add(searchDTO.OwnerId, ebcOrderDTOs);
                        }
                        else
                        {
                            ebcUserIds[searchDTO.OwnerId].Add(result);
                        }
                    }

                    results.Add(result);

                }

                if (cbcUserIds.Count > 0)
                {
                    var cbcUserInfos = CBCSV.Instance.GetUserNameByIds(cbcUserIds.Keys.ToList());
                    foreach (var cbcUserInfo in cbcUserInfos)
                    {
                        if (cbcUserIds.ContainsKey(cbcUserInfo.UserId))
                        {
                            foreach (var cbcUserOrder in cbcUserIds[cbcUserInfo.UserId])
                            {
                                cbcUserOrder.UserName = cbcUserInfo.UserName;
                            }
                        }
                    }
                }
                if (ebcUserIds.Count > 0)
                {
                    var ebcUserInfos = Jinher.AMP.BTP.TPS.EBCSV.Instance.GetOrgNameIconByIdList(ebcUserIds.Keys.ToList());

                    foreach (var ebcUserInfo in ebcUserInfos)
                    {
                        if (ebcUserIds.ContainsKey(ebcUserInfo.Id))
                        {
                            foreach (var ebcUserOrder in ebcUserIds[ebcUserInfo.Id])
                            {
                                ebcUserOrder.UserName = ebcUserInfo.Name;
                            }
                        }
                    }
                }

                LogHelper.Info(string.Format("QryAppOrderTradeInfoBP搜索到应用数：{0}", results.Count));
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("QryAppOrderTradeInfoBP异常。appName：{0}", appName), ex);
            }
            return results;
        }


        /// <summary>
        /// 修改卖家备注信息
        /// </summary>
        /// <param name="commodityOrderId">订单编号</param>
        /// <param name="SellersRemark">备注信息</param>
        /// <returns></returns>
        public ResultDTO UpdateSellersRemarkExt(Guid commodityOrderId, string SellersRemark)
        {
            if (!OrderSV.LockOrder(commodityOrderId))
            {
                return new ResultDTO { ResultCode = 110, Message = "操作失败" };
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                CommodityOrder commodityOrder = CommodityOrder.ObjectSet().Where(n => n.Id == commodityOrderId).FirstOrDefault();
                if (commodityOrder != null)
                {
                    commodityOrder.SellersRemark = SellersRemark;
                    commodityOrder.EntityState = EntityState.Modified;
                    contextSession.SaveObject(commodityOrder);
                    contextSession.SaveChanges();
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                }
                else
                {
                    return new ResultDTO { ResultCode = 1, Message = "不存在此订单" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("修改订单卖家备注异常。commodityOrderId：{0}，SellersRemark：{1}", commodityOrderId, SellersRemark), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(commodityOrderId);
            }
        }
        /// <summary>
        /// 修改物流信息（只修改信息，不修改订单状态）
        /// </summary>
        /// <param name="commodityOrderId"></param>
        /// <param name="shipExpCo"></param>
        /// <param name="expOrderNo"></param>
        /// <returns></returns>
        public ResultShipDTO ChgOrderShipExt(Guid commodityOrderId, string shipExpCo, string expOrderNo)
        {

            ResultShipDTO result = new ResultShipDTO { ResultCode = 0, Message = "Success" };
            if (!OrderSV.LockOrder(commodityOrderId))
            {
                return new ResultShipDTO { ResultCode = 110, Message = "操作失败" };
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                CommodityOrder commodityOrder = CommodityOrder.ObjectSet().Where(n => n.Id == commodityOrderId).FirstOrDefault();
                if (commodityOrder == null)
                    return new ResultShipDTO { ResultCode = 1, Message = "Error" };

                commodityOrder.ShipExpCo = string.IsNullOrWhiteSpace(shipExpCo) ? "" : shipExpCo.Trim();
                commodityOrder.ExpOrderNo = string.IsNullOrWhiteSpace(expOrderNo) ? "" : expOrderNo.Trim();
                commodityOrder.ExpOrderNo = commodityOrder.ExpOrderNo.Replace("+", "");
                commodityOrder.EntityState = EntityState.Modified;

                OrderShipping orderShipping = OrderShipping.CreateOrderShipping();
                orderShipping.OrderId = commodityOrderId;
                orderShipping.ShipExpCo = commodityOrder.ShipExpCo;
                orderShipping.ExpOrderNo = commodityOrder.ExpOrderNo;
                contextSession.SaveObject(orderShipping);

                contextSession.SaveChanges();

                result.OrderShippingExt = new OrderShippingExtDTO()
                {
                    OrderId = commodityOrderId,
                    ShipExpCo = shipExpCo,
                    ExpOrderNo = expOrderNo
                };
                var orgShip = OrderShipping.ObjectSet().Where(c => c.OrderId == commodityOrderId).OrderBy(c => c.SubTime).FirstOrDefault();
                if (orgShip != null)
                {
                    result.OrderShippingExt.OrgShipExpCo = orgShip.ShipExpCo;
                    result.OrderShippingExt.OrgExpOrderNo = orgShip.ExpOrderNo;
                }


                //TODO dzc 向第三方“快递鸟”发送物流订阅请求。  
                OrderExpressRoute oer = new OrderExpressRoute()
                {
                    ShipExpCo = commodityOrder.ShipExpCo,
                    ExpOrderNo = commodityOrder.ExpOrderNo
                };
                OrderExpressRouteBP oerBP = new OrderExpressRouteBP();
                oerBP.SubscribeOneOrderExpressExt(oer);

                return result;

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CommodityOrderBP.ChgOrderShipExt 异常。commodityOrderId：{0}，shipExpCo：{1}，expOrderNo：{2}", commodityOrderId, shipExpCo, expOrderNo), ex);
                return new ResultShipDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(commodityOrderId);
            }
        }

        private void WriteNPOIOrderItem(XWPFDocument doc, ImportOrderDTO order, List<ImportOrderItemDTO> curOrderItems, string SnOrderCode)
        {
            int itemTotal = (from e in curOrderItems
                             select e.Number).Sum();
            XWPFParagraph paragraph;
            XWPFRun run;

            CT_Tbl ctTbl;
            CT_TblGrid tbGrid;
            CT_TblPr tblPr;

            CT_Tc ctTc;
            CT_TcPr tcPr;

            CT_R ctR;
            CT_RPr ctRpr;
            CT_HpsMeasure sz;


            XWPFTable table;
            XWPFTable table2;

            XWPFTableRow row;
            XWPFTableCell cell;
            XWPFTableCell cell_1;
            XWPFTableCell cell_2;

            #region 文字

            table = doc.CreateTable(8, 2);
            table.SetInsideHBorder(XWPFTable.XWPFBorderType.NONE, -1, 0, null);
            table.SetInsideVBorder(XWPFTable.XWPFBorderType.NONE, -1, 0, null);
            table.SetTopBorder(XWPFTable.XWPFBorderType.NONE, -1, 0, null);
            table.SetRightBorder(XWPFTable.XWPFBorderType.NONE, -1, 0, null);
            table.SetBottomBorder(XWPFTable.XWPFBorderType.NONE, -1, 0, null);
            table.SetLeftBorder(XWPFTable.XWPFBorderType.NONE, -1, 0, null);
            // table.Width = 11800;
            table.SetColumnWidth(0, 8000);
            table.SetColumnWidth(1, 3800);
            table.SetCellMargins(40, 40, 40, 40);

            row = table.GetRow(0);
            row.MergeCells(0, 1);
            cell = row.GetCell(0);

            ctTc = cell.GetCTTc();
            ctTc.tcPr = new CT_TcPr();
            ctTc.tcPr.gridSpan = new CT_DecimalNumber();
            ctTc.tcPr.gridSpan.val = "2";
            ctTc.tcPr.tcW = new CT_TblWidth();
            ctTc.tcPr.tcW.type = ST_TblWidth.dxa;
            ctTc.tcPr.tcW.w = "11800";

            paragraph = cell.Paragraphs[0];
            paragraph.Alignment = ParagraphAlignment.CENTER;
            run = paragraph.CreateRun();
            run.FontFamily = "宋体";
            run.SetBold(true);
            run.FontSize = 12;
            run.SetText("订单");

            row = table.GetRow(1);

            cell_1 = row.GetCell(0);
            ctTc = cell_1.GetCTTc();
            ctTc.tcPr = new CT_TcPr();
            ctTc.tcPr.tcW = new CT_TblWidth();
            ctTc.tcPr.tcW.type = ST_TblWidth.dxa;
            ctTc.tcPr.tcW.w = "8000";
            ctR = ctTc.GetPList()[0].AddNewR();
            ctRpr = ctR.AddNewRPr();
            sz = new CT_HpsMeasure();
            sz.val = (ulong.Parse("24"));
            ctRpr.sz = sz;
            ctR.AddNewT().Value = string.Format("订单号：{0}{1}", order.Code, (string.IsNullOrWhiteSpace(SnOrderCode) ? "" : ",苏宁订单编号：" + SnOrderCode));


            cell_2 = row.GetCell(1);
            ctTc = cell_2.GetCTTc();
            ctTc.tcPr = new CT_TcPr();
            ctTc.tcPr.tcW = new CT_TblWidth();
            ctTc.tcPr.tcW.type = ST_TblWidth.dxa;
            ctTc.tcPr.tcW.w = "3800";
            ctR = ctTc.GetPList()[0].AddNewR();
            ctRpr = ctR.AddNewRPr();
            sz = new CT_HpsMeasure();
            sz.val = (ulong.Parse("24"));
            ctRpr.sz = sz;
            ctR.AddNewT().Value = "下单时间：" + order.SubTime.ToString();

            string payType = new PaySourceBP().GetPaymentName(order.Payment);
            payType = "付款方式：" + payType;

            row = table.GetRow(2);

            cell_1 = row.GetCell(0);
            ctTc = cell_1.GetCTTc();
            ctTc.tcPr = new CT_TcPr();
            ctTc.tcPr.tcW = new CT_TblWidth();
            ctTc.tcPr.tcW.type = ST_TblWidth.dxa;
            ctTc.tcPr.tcW.w = "8000";
            ctR = ctTc.GetPList()[0].AddNewR();
            ctRpr = ctR.AddNewRPr();
            sz = new CT_HpsMeasure();
            sz.val = (ulong.Parse("24"));
            ctRpr.sz = sz;
            ctR.AddNewT().Value = payType;

            cell_2 = row.GetCell(1);
            ctTc = cell_2.GetCTTc();
            ctTc.tcPr = new CT_TcPr();
            ctTc.tcPr.tcW = new CT_TblWidth();
            ctTc.tcPr.tcW.type = ST_TblWidth.dxa;
            ctTc.tcPr.tcW.w = "3800";
            ctR = ctTc.GetPList()[0].AddNewR();
            ctRpr = ctR.AddNewRPr();
            sz = new CT_HpsMeasure();
            sz.val = (ulong.Parse("24"));
            ctRpr.sz = sz;
            ctR.AddNewT().Value = "付款时间：" + order.PaymentTime.ToString();


            row = table.GetRow(3);

            cell_1 = row.GetCell(0);
            ctTc = cell_1.GetCTTc();
            ctTc.tcPr = new CT_TcPr();
            ctTc.tcPr.tcW = new CT_TblWidth();
            ctTc.tcPr.tcW.type = ST_TblWidth.dxa;
            ctTc.tcPr.tcW.w = "8000";
            ctR = ctTc.GetPList()[0].AddNewR();
            ctRpr = ctR.AddNewRPr();
            sz = new CT_HpsMeasure();
            sz.val = (ulong.Parse("24"));
            ctRpr.sz = sz;
            ctR.AddNewT().Value = "收货人：" + order.ReceiptUserName;


            cell_2 = row.GetCell(1);
            ctTc = cell_2.GetCTTc();
            ctTc.tcPr = new CT_TcPr();
            ctTc.tcPr.tcW = new CT_TblWidth();
            ctTc.tcPr.tcW.type = ST_TblWidth.dxa;
            ctTc.tcPr.tcW.w = "3800";
            ctR = ctTc.GetPList()[0].AddNewR();
            ctRpr = ctR.AddNewRPr();
            sz = new CT_HpsMeasure();
            sz.val = (ulong.Parse("24"));
            ctRpr.sz = sz;
            ctR.AddNewT().Value = "手机号：" + order.ReceiptPhone;


            row = table.GetRow(4);

            cell_1 = row.GetCell(0);
            ctTc = cell_1.GetCTTc();
            ctTc.tcPr = new CT_TcPr();
            ctTc.tcPr.tcW = new CT_TblWidth();
            ctTc.tcPr.tcW.type = ST_TblWidth.dxa;
            ctTc.tcPr.tcW.w = "8000";
            ctR = ctTc.GetPList()[0].AddNewR();
            ctRpr = ctR.AddNewRPr();
            sz = new CT_HpsMeasure();
            sz.val = (ulong.Parse("24"));
            ctRpr.sz = sz;
            ctR.AddNewT().Value = string.Format("收货地址：{0}{1}{2}{3}{4}", order.Province, order.City, order.District, order.Street, order.ReceiptAddress);

            cell_2 = row.GetCell(1);
            ctTc = cell_2.GetCTTc();
            ctTc.tcPr = new CT_TcPr();
            ctTc.tcPr.tcW = new CT_TblWidth();
            ctTc.tcPr.tcW.type = ST_TblWidth.dxa;
            ctTc.tcPr.tcW.w = "3800";
            ctR = ctTc.GetPList()[0].AddNewR();
            ctRpr = ctR.AddNewRPr();
            sz = new CT_HpsMeasure();
            sz.val = (ulong.Parse("24"));
            ctRpr.sz = sz;
            ctR.AddNewT().Value = "邮编：" + order.RecipientsZipCode;

            #region 添加物流信息 zgx-by add 20170213
            row = table.GetRow(5);

            cell_1 = row.GetCell(0);
            ctTc = cell_1.GetCTTc();
            ctTc.tcPr = new CT_TcPr();
            ctTc.tcPr.tcW = new CT_TblWidth();
            ctTc.tcPr.tcW.type = ST_TblWidth.dxa;
            ctTc.tcPr.tcW.w = "8000";
            ctR = ctTc.GetPList()[0].AddNewR();
            ctRpr = ctR.AddNewRPr();
            sz = new CT_HpsMeasure();
            sz.val = (ulong.Parse("24"));
            ctRpr.sz = sz;
            ctR.AddNewT().Value = "物流公司：" + order.ShipExpCo;

            cell_2 = row.GetCell(1);
            ctTc = cell_2.GetCTTc();
            ctTc.tcPr = new CT_TcPr();
            ctTc.tcPr.tcW = new CT_TblWidth();
            ctTc.tcPr.tcW.type = ST_TblWidth.dxa;
            ctTc.tcPr.tcW.w = "3800";
            ctR = ctTc.GetPList()[0].AddNewR();
            ctRpr = ctR.AddNewRPr();
            sz = new CT_HpsMeasure();
            sz.val = (ulong.Parse("24"));
            ctRpr.sz = sz;
            ctR.AddNewT().Value = "快递单号：" + order.ExpOrderNo;
            #endregion


            row = table.GetRow(6);

            cell_1 = row.GetCell(0);
            ctTc = cell_1.GetCTTc();
            ctTc.tcPr = new CT_TcPr();
            ctTc.tcPr.tcW = new CT_TblWidth();
            ctTc.tcPr.tcW.type = ST_TblWidth.dxa;
            ctTc.tcPr.tcW.w = "8000";
            ctR = ctTc.GetPList()[0].AddNewR();
            ctRpr = ctR.AddNewRPr();
            sz = new CT_HpsMeasure();
            sz.val = (ulong.Parse("24"));
            ctRpr.sz = sz;
            ctR.AddNewT().Value = "买家备注：" + order.Details;

            cell_2 = row.GetCell(1);
            ctTc = cell_2.GetCTTc();
            ctTc.tcPr = new CT_TcPr();
            ctTc.tcPr.tcW = new CT_TblWidth();
            ctTc.tcPr.tcW.type = ST_TblWidth.dxa;
            ctTc.tcPr.tcW.w = "3800";
            ctR = ctTc.GetPList()[0].AddNewR();
            ctRpr = ctR.AddNewRPr();
            sz = new CT_HpsMeasure();
            sz.val = (ulong.Parse("24"));
            ctRpr.sz = sz;
            ctR.AddNewT().Value = "";

            row = table.GetRow(7);

            cell_1 = row.GetCell(0);
            ctTc = cell_1.GetCTTc();
            ctTc.tcPr = new CT_TcPr();
            ctTc.tcPr.tcW = new CT_TblWidth();
            ctTc.tcPr.tcW.type = ST_TblWidth.dxa;
            ctTc.tcPr.tcW.w = "8000";
            ctR = ctTc.GetPList()[0].AddNewR();
            ctRpr = ctR.AddNewRPr();
            sz = new CT_HpsMeasure();
            sz.val = (ulong.Parse("24"));
            ctRpr.sz = sz;
            ctR.AddNewT().Value = "商家备注：" + System.Web.HttpContext.Current.Server.UrlDecode(order.SellersRemark);

            cell_2 = row.GetCell(1);
            ctTc = cell_2.GetCTTc();
            ctTc.tcPr = new CT_TcPr();
            ctTc.tcPr.tcW = new CT_TblWidth();
            ctTc.tcPr.tcW.type = ST_TblWidth.dxa;
            ctTc.tcPr.tcW.w = "3800";
            ctR = ctTc.GetPList()[0].AddNewR();
            ctRpr = ctR.AddNewRPr();
            sz = new CT_HpsMeasure();
            sz.val = (ulong.Parse("24"));
            ctRpr.sz = sz;
            ctR.AddNewT().Value = "";

            if (order.InvoiceType != 0)
            {
                row = table.CreateRow();

                cell_1 = row.GetCell(0);
                ctTc = cell_1.GetCTTc();
                ctTc.tcPr = new CT_TcPr();
                ctTc.tcPr.tcW = new CT_TblWidth();
                ctTc.tcPr.tcW.type = ST_TblWidth.dxa;
                ctTc.tcPr.tcW.w = "8000";
                ctR = ctTc.GetPList()[0].AddNewR();
                ctRpr = ctR.AddNewRPr();
                sz = new CT_HpsMeasure();
                sz.val = (ulong.Parse("24"));
                ctRpr.sz = sz;
                ctR.AddNewT().Value = "发票抬头：" + order.InvoiceTitle;

                cell_2 = row.CreateCell();
                ctTc = cell_2.GetCTTc();
                ctTc.tcPr = new CT_TcPr();
                ctTc.tcPr.tcW = new CT_TblWidth();
                ctTc.tcPr.tcW.type = ST_TblWidth.dxa;
                ctTc.tcPr.tcW.w = "3800";
                ctR = ctTc.GetPList()[0].AddNewR();
                ctRpr = ctR.AddNewRPr();
                sz = new CT_HpsMeasure();
                sz.val = (ulong.Parse("24"));
                ctRpr.sz = sz;
                ctR.AddNewT().Value = "发票内容：明细";
            }

            #region 添加物流信息 zgx-by add 20170213
            /*
            row = table.InsertNewTableRow(5);
            cell_1 = row.GetCell(0);
            ctTc = cell_1.GetCTTc();
            ctTc.tcPr = new CT_TcPr();
            ctTc.tcPr.tcW = new CT_TblWidth();
            ctTc.tcPr.tcW.type = ST_TblWidth.dxa;
            ctTc.tcPr.tcW.w = "8000";
            ctR = ctTc.GetPList()[0].AddNewR();
            ctRpr = ctR.AddNewRPr();
            sz = new CT_HpsMeasure();
            sz.val = (ulong.Parse("24"));
            ctRpr.sz = sz;
            ctR.AddNewT().Value = "物流公司：" + order.ShipExpCo;

            cell_2 = row.CreateCell();
            ctTc = cell_2.GetCTTc();
            ctTc.tcPr = new CT_TcPr();
            ctTc.tcPr.tcW = new CT_TblWidth();
            ctTc.tcPr.tcW.type = ST_TblWidth.dxa;
            ctTc.tcPr.tcW.w = "3800";
            ctR = ctTc.GetPList()[0].AddNewR();
            ctRpr = ctR.AddNewRPr();
            sz = new CT_HpsMeasure();
            sz.val = (ulong.Parse("24"));
            ctRpr.sz = sz;
            ctR.AddNewT().Value = "快递单号:" + order.ExpOrderNo;
             */
            #endregion

            #endregion

            doc.CreateParagraph();

            #region

            table2 = doc.CreateTable(2, 8);
            table2.SetInsideHBorder(XWPFTable.XWPFBorderType.SINGLE, 4, 0, "#000");
            table2.SetInsideVBorder(XWPFTable.XWPFBorderType.SINGLE, 4, 0, "#000");
            table2.SetTopBorder(XWPFTable.XWPFBorderType.SINGLE, 4, 0, "#000");
            table2.SetRightBorder(XWPFTable.XWPFBorderType.SINGLE, 4, 0, "#000");
            table2.SetBottomBorder(XWPFTable.XWPFBorderType.SINGLE, 4, 0, "#000");
            table2.SetLeftBorder(XWPFTable.XWPFBorderType.SINGLE, 4, 0, "#000");
            //table2.Width = 11327;
            table2.SetColumnWidth(0, 800);
            table2.SetColumnWidth(1, 1887);
            table2.SetColumnWidth(2, 4640);
            table2.SetColumnWidth(3, 1600);
            table2.SetColumnWidth(4, 800);
            table2.SetColumnWidth(5, 1595);
            table2.SetColumnWidth(6, 1600);
            table2.SetCellMargins(40, 90, 40, 90);

            row = table2.GetRow(0);
            row.MergeCells(0, 7);
            cell = row.GetCell(0);

            ctTc = cell.GetCTTc();
            ctTc.tcPr = new CT_TcPr();
            ctTc.tcPr.gridSpan = new CT_DecimalNumber();
            ctTc.tcPr.gridSpan.val = "8";
            ctTc.tcPr.tcW = new CT_TblWidth();
            ctTc.tcPr.tcW.type = ST_TblWidth.dxa;
            ctTc.tcPr.tcW.w = "12922";

            paragraph = cell.Paragraphs[0];
            paragraph.Alignment = ParagraphAlignment.CENTER;
            run = paragraph.CreateRun();
            run.FontFamily = "宋体";
            run.FontSize = 12;
            run.SetText(string.Format("商品总数：{0}    实收款：{1}元", itemTotal, order.RealPrice));

            List<string> table2Titles = new List<string> { "序号", "商品编号", "商品名称", "单价（元）", "数量", "金额（元）", "订单来源" };
            row = table2.GetRow(1);
            for (int i = 0; i < table2Titles.Count; i++)
            {
                cell = row.GetCell(i);
                ctTc = cell.GetCTTc();
                ctTc.tcPr = new CT_TcPr();
                ctTc.tcPr.tcW = new CT_TblWidth();
                ctTc.tcPr.tcW.type = ST_TblWidth.dxa;

                ctR = ctTc.GetPList()[0].AddNewR();
                ctRpr = ctR.AddNewRPr();
                sz = new CT_HpsMeasure();
                sz.val = (ulong.Parse("24"));
                ctRpr.sz = sz;
                ctR.AddNewT().Value = table2Titles[i];

                switch (i)
                {
                    case 0:
                        ctTc.tcPr.tcW.w = "800";
                        break;
                    case 1:
                        ctTc.tcPr.tcW.w = "1887";
                        break;
                    case 2:
                        ctTc.tcPr.tcW.w = "4640";
                        break;
                    case 3:
                        ctTc.tcPr.tcW.w = "1600";
                        break;
                    case 4:
                        ctTc.tcPr.tcW.w = "800";
                        break;
                    case 5:
                        ctTc.tcPr.tcW.w = "1595";
                        break;
                    case 6:
                        ctTc.tcPr.tcW.w = "1600";
                        break;
                    default:
                        break;
                }
            }

            int curItemNo = 1;

            foreach (var orderItem in curOrderItems)
            {
                row = table2.CreateRow();
                row.RemoveCell(0);
                //0
                cell = row.CreateCell();
                ctTc = cell.GetCTTc();
                ctTc.tcPr = new CT_TcPr();
                ctTc.tcPr.tcW = new CT_TblWidth();
                ctTc.tcPr.tcW.type = ST_TblWidth.dxa;
                ctTc.tcPr.tcW.w = "800";
                ctR = ctTc.GetPList()[0].AddNewR();
                ctRpr = ctR.AddNewRPr();
                sz = new CT_HpsMeasure();
                sz.val = (ulong.Parse("24"));
                ctRpr.sz = sz;
                ctR.AddNewT().Value = (curItemNo).ToString();

                //1
                cell = row.CreateCell();
                ctTc = cell.GetCTTc();
                ctTc.tcPr = new CT_TcPr();
                ctTc.tcPr.tcW = new CT_TblWidth();
                ctTc.tcPr.tcW.type = ST_TblWidth.dxa;
                ctTc.tcPr.tcW.w = "1887";
                ctR = ctTc.GetPList()[0].AddNewR();
                ctRpr = ctR.AddNewRPr();
                sz = new CT_HpsMeasure();
                sz.val = (ulong.Parse("24"));
                ctRpr.sz = sz;
                ctR.AddNewT().Value = orderItem.CommodityCode;

                //2
                cell = row.CreateCell();
                ctTc = cell.GetCTTc();
                ctTc.tcPr = new CT_TcPr();
                ctTc.tcPr.tcW = new CT_TblWidth();
                ctTc.tcPr.tcW.type = ST_TblWidth.dxa;
                ctTc.tcPr.tcW.w = "4640";
                ctR = ctTc.GetPList()[0].AddNewR();
                ctRpr = ctR.AddNewRPr();
                sz = new CT_HpsMeasure();
                sz.val = (ulong.Parse("24"));
                ctRpr.sz = sz;
                ctR.AddNewT().Value = orderItem.CommodityName;

                //3
                cell = row.CreateCell();
                ctTc = cell.GetCTTc();
                ctTc.tcPr = new CT_TcPr();
                ctTc.tcPr.tcW = new CT_TblWidth();
                ctTc.tcPr.tcW.type = ST_TblWidth.dxa;
                ctTc.tcPr.tcW.w = "1600";
                ctR = ctTc.GetPList()[0].AddNewR();
                ctRpr = ctR.AddNewRPr();
                sz = new CT_HpsMeasure();
                sz.val = (ulong.Parse("24"));
                ctRpr.sz = sz;
                ctR.AddNewT().Value = orderItem.Price.ToString();

                //4
                cell = row.CreateCell();
                ctTc = cell.GetCTTc();
                ctTc.tcPr = new CT_TcPr();
                ctTc.tcPr.tcW = new CT_TblWidth();
                ctTc.tcPr.tcW.type = ST_TblWidth.dxa;
                ctTc.tcPr.tcW.w = "800";
                ctR = ctTc.GetPList()[0].AddNewR();
                ctRpr = ctR.AddNewRPr();
                sz = new CT_HpsMeasure();
                sz.val = (ulong.Parse("24"));
                ctRpr.sz = sz;
                ctR.AddNewT().Value = orderItem.Number.ToString();

                //5
                cell = row.CreateCell();
                ctTc = cell.GetCTTc();
                ctTc.tcPr = new CT_TcPr();
                ctTc.tcPr.tcW = new CT_TblWidth();
                ctTc.tcPr.tcW.type = ST_TblWidth.dxa;
                ctTc.tcPr.tcW.w = "1595";
                ctR = ctTc.GetPList()[0].AddNewR();
                ctRpr = ctR.AddNewRPr();
                sz = new CT_HpsMeasure();
                sz.val = (ulong.Parse("24"));
                ctRpr.sz = sz;
                ctR.AddNewT().Value = (orderItem.Number * orderItem.Price).ToString();

                //6
                cell = row.CreateCell();
                ctTc = cell.GetCTTc();
                ctTc.tcPr = new CT_TcPr();
                ctTc.tcPr.tcW = new CT_TblWidth();
                ctTc.tcPr.tcW.type = ST_TblWidth.dxa;
                ctTc.tcPr.tcW.w = "1600";
                ctR = ctTc.GetPList()[0].AddNewR();
                ctRpr = ctR.AddNewRPr();
                sz = new CT_HpsMeasure();
                sz.val = (ulong.Parse("24"));
                ctRpr.sz = sz;
                ctR.AddNewT().Value = orderItem.EsAppName;

                curItemNo++;
            }
            #endregion

            doc.CreateParagraph();
        }

        /// <summary>
        /// 根据对账订单Id列表取电商订单对账信息
        /// </summary>
        /// <param name="mainOrderIds">对账订单Id列表</param>
        /// <returns>电商订单对账信息</returns>
        public List<CommodityOrderCheckAccount> GetMainOrdersPayExt(string mainOrderIds)
        {
            List<CommodityOrderCheckAccount> result = new List<CommodityOrderCheckAccount>();
            List<CommodityOrderCheckAccount> resultTmp = new List<CommodityOrderCheckAccount>();
            mainOrderIds = Regex.Replace(mainOrderIds, "\\s*", "");
            if (string.IsNullOrWhiteSpace(mainOrderIds))
                return result;
            var strIds = mainOrderIds.Replace('；', ';').Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

            //订单ID列表
            List<Guid> mainOrderIdList = new List<Guid>();
            Guid tmpId;

            foreach (var strId in strIds)
            {
                if (Guid.TryParse(strId, out tmpId))
                {
                    if (!mainOrderIdList.Contains(tmpId))
                    {
                        mainOrderIdList.Add(tmpId);
                    }
                }
            }
            if (!mainOrderIdList.Any())
                return result;

            //根据主订单ID取出拆分出的订单的IDs
            var dict = MainOrder.ObjectSet()
                      .Where(c => mainOrderIdList.Contains(c.MainOrderId))
                      .GroupBy(c => c.MainOrderId, (key, group) => new { MainOrderId = key, MainOrderList = group })
                      .ToDictionary(a => a.MainOrderId, a => a.MainOrderList.Select(c => c.SubOrderId));

            //没有主订单的订单们，即没有拆分过的订单
            List<Guid> orderIdList = mainOrderIdList.Except(dict.Keys.ToList()).ToList();
            foreach (var dicTmp in dict)
            {
                orderIdList.AddRange(dicTmp.Value);
            }

            List<int> states = new List<int> { 1, 2, 3, 7, 8, 9, 10, 12, 13, 14 };

            //取出对账信息
            var resultOrder = (from c in CommodityOrder.ObjectSet()
                               join m in MainOrder.ObjectSet() on c.Id equals m.SubOrderId
                               into data2
                               from mOrder in data2.DefaultIfEmpty()
                               where orderIdList.Contains(c.Id) && states.Contains(c.State)
                               select new CommodityOrderCheckAccount
                               {
                                   AccountId = mOrder.MainOrderId == null ? c.Id : mOrder.MainOrderId,
                                   O2OId = c.Id,
                                   AppId = c.AppId,
                                   AppName = "",
                                   PaymentTime = c.PaymentTime,
                                   RealPrice = c.RealPrice == null ? 0 : c.RealPrice.Value,
                                   RefundPrice = 0

                               }).ToList();
            //TODO 订单ID
            var orderIds = resultOrder.Select(t => t.O2OId).Distinct().ToList();
            //取出退款信息
            List<int> rState = new List<int> { 2, 3, 4, 13 };
            var refundList = OrderRefund.ObjectSet().Where(t => orderIds.Contains(t.OrderId) && !rState.Contains(t.State)).ToList();
            //var refundList = OrderRefund.ObjectSet().Where(t => orderIds.Contains(t.OrderId)).GroupBy(t => t.OrderId).ToDictionary(x => x.Key, y => y.OrderByDescending(t => t.SubTime).FirstOrDefault());
            if (refundList.Count > 0)
            {
                foreach (var refundItem in refundList)
                {
                    var orderItem = resultOrder.Where(t => t.O2OId == refundItem.OrderId).ToList();
                    if (orderItem != null && orderItem.Count > 0)
                    {
                        foreach (var item in orderItem)
                        {
                            item.RefundPrice = refundItem.RefundMoney;
                        }
                    }
                }
            }

            List<Guid> appIds = (from it in resultOrder select it.AppId).Distinct().ToList();
            try
            {
                Dictionary<Guid, string> listApps = APPSV.GetAppNameListByIds(appIds);
                foreach (var item in resultOrder)
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
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("修改订单卖家备注异常。appIds：{0}", appIds), ex);
            }

            //排序          
            for (int i = 0; i < strIds.Length; i++)
            {
                var tmpAddId = new Guid(strIds[i]);
                var tempList = resultOrder.Where(t => t.AccountId == tmpAddId).ToList();
                foreach (var item in tempList)
                {
                    var CommodityOrderCheckAccount = new CommodityOrderCheckAccount()
                    {
                        AccountId = item.AccountId,
                        AccountIdString = strIds[i],
                        AppId = item.AppId,
                        AppName = item.AppName,
                        O2OId = item.O2OId,
                        PaymentTime = item.PaymentTime,
                        RealPrice = item.RealPrice,
                        RefundPrice = item.RefundPrice
                    };
                    resultTmp.Add(CommodityOrderCheckAccount);
                }
            }

            //取属于代运营电商的订单
            List<Guid> o2oAppIdList = new List<Guid>();
            var appIdsTmp = resultTmp.Select(t => t.AppId).Distinct().ToList();
            foreach (Guid id in appIdsTmp)
            {
                if (ZPHSV.Instance.CheckIsAppInZPH(id))
                {
                    o2oAppIdList.Add(id);
                }
            }
            result = resultTmp.Where(t => o2oAppIdList.Contains(t.AppId)).ToList();
            return result;
        }
        /// <summary>
        /// 售后拒绝退款/退货申请
        /// </summary>
        /// <param name="cancelTheOrderDTO"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RefuseRefundOrderExt(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO)
        {
            if (cancelTheOrderDTO.OrderItemId != Guid.Empty)
            {
                return RefuseRefundOrderItem(cancelTheOrderDTO);
            }
            if (cancelTheOrderDTO == null)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数不能为空" };
            }
            if (!OrderSV.LockOrder(cancelTheOrderDTO.OrderId))
            {
                return new ResultDTO { ResultCode = 110, Message = "操作失败" };
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var stateList = new List<int> { 8, 9, 10, 14 };
                var commodityOrderList = (from c in CommodityOrder.ObjectSet()
                                          join r in OrderRefund.ObjectSet() on c.Id equals r.OrderId
                                          where c.Id == cancelTheOrderDTO.OrderId && stateList.Contains(c.State) && r.State == 0
                                          select new
                                          {
                                              commodityOrder = c,
                                              orderRefund = r
                                          }).FirstOrDefault();
                if (commodityOrderList == null)
                {
                    return new ResultDTO { ResultCode = 1, Message = "找不到相应的订单" };
                }
                CommodityOrder commodityOrder = commodityOrderList.commodityOrder;
                OrderRefund orderRefund = commodityOrderList.orderRefund;

                if (commodityOrder == null || orderRefund == null)
                {
                    return new ResultDTO { ResultCode = 1, Message = "找不到相应的订单" };
                }

                int oldState = commodityOrder.State;
                int oldOrderRefundState = orderRefund.State;

                switch (commodityOrder.State)
                {
                    //如果待发货退款中 更改为待发货
                    case 8:
                        commodityOrder.State = 1;
                        break;
                    //如果已发货退款中 更改为已发货 
                    case 9:
                        commodityOrder.State = 2;
                        break;
                    //如果出库中退款中 更改为出库中
                    case 14:
                        commodityOrder.State = 13;
                        break;
                    //如果已发货退款中商家同意退款，商家未收到货，更改为已发货
                    case 10:
                        commodityOrder.State = 2;
                        break;
                }
                commodityOrder.IsRefund = false;
                commodityOrder.RefundTime = null;
                commodityOrder.ModifiedOn = DateTime.Now;
                commodityOrder.EntityState = System.Data.EntityState.Modified;

                orderRefund.State = 2;
                orderRefund.RefuseTime = DateTime.Now;
                orderRefund.RefuseReason = cancelTheOrderDTO.RefuseReason;
                orderRefund.ModifiedOn = DateTime.Now;
                orderRefund.EntityState = System.Data.EntityState.Modified;

                //if (cancelTheOrderDTO.RejectFreightMoney> 0)  //拒收运费
                //{
                //    orderRefund.RejectFreightMoney = cancelTheOrderDTO.RejectFreightMoney;
                //}

                int result = contextSession.SaveChanges();

                if (result > 0)
                {
                    BTPMessageSV addmassage = new BTPMessageSV();
                    //发送消息，异步执行
                    System.Threading.ThreadPool.QueueUserWorkItem(
                        a =>
                        {
                            CommodityOrderMessages messageModel = new CommodityOrderMessages();
                            messageModel.IsAuto = false;
                            messageModel.Id = commodityOrder.Id.ToString();
                            messageModel.UserIds = commodityOrder.UserId.ToString();
                            messageModel.AppId = commodityOrder.AppId;
                            messageModel.Code = commodityOrder.Code;
                            messageModel.State = commodityOrder.State;
                            messageModel.RefundType = orderRefund.RefundType;
                            messageModel.RefundMoney = orderRefund.RefundMoney;
                            messageModel.PayType = commodityOrder.Payment;
                            messageModel.orderRefundState = orderRefund.State;
                            messageModel.oldOrderRefundState = oldOrderRefundState;
                            messageModel.RefuseReason = orderRefund.RefuseReason;
                            messageModel.EsAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
                            addmassage.AddMessagesCommodityOrder(messageModel);
                        });
                    try
                    {
                        Journal journal = new Journal();
                        journal.Id = Guid.NewGuid();
                        if (orderRefund.RefundType == 0)
                        {
                            journal.Name = "售中退款拒绝退款请";
                        }
                        else if (orderRefund.RefundType == 1)
                        {
                            journal.Name = "售中退款拒绝退款/退货申请";
                        }
                        journal.Code = commodityOrder.Code;
                        journal.SubId = commodityOrder.UserId;
                        journal.SubTime = DateTime.Now;
                        journal.Details = "订单状态由" + oldState + "变为" + commodityOrder.State;
                        journal.CommodityOrderId = commodityOrder.Id;
                        journal.StateFrom = oldState;
                        journal.StateTo = commodityOrder.State;
                        journal.IsPush = false;
                        journal.OrderType = commodityOrder.OrderType;

                        journal.EntityState = System.Data.EntityState.Added;
                        contextSession.SaveObject(journal);
                        contextSession.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("拒绝退款/退货申请记日志异常。", ex);
                    }

                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                }
                return new ResultDTO { ResultCode = 1, Message = "拒绝退款/退货申请时，保存失败" };

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("拒绝退款/退货申请。cancelOrderRefundDTO：{0}", JsonHelper.JsonSerializer(cancelTheOrderDTO)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(cancelTheOrderDTO.OrderId);
            }
        }

        /// <summary>
        /// 售中拒绝退款/退货申请 单商品
        /// </summary>
        /// <param name="cancelTheOrderDTO"></param>
        /// <returns></returns>
        private Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RefuseRefundOrderItem(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO)
        {
            LogHelper.Debug("开始进入单商品售中拒绝退款/退货申请方法RefuseRefundOrderItem，参数为cancelTheOrderDTO：" + JsonHelper.JsSerializer(cancelTheOrderDTO));
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                CommodityOrder commodityOrder = CommodityOrder.FindByID(cancelTheOrderDTO.OrderId);
                OrderItem orderItem = OrderItem.FindByID(cancelTheOrderDTO.OrderItemId);
                OrderRefund orderRefund = OrderRefund.ObjectSet().Where(t => t.OrderItemId == cancelTheOrderDTO.OrderItemId).OrderByDescending(t => t.SubTime).FirstOrDefault();

                if (commodityOrder == null || orderRefund == null || orderItem == null)
                {
                    return new ResultDTO { ResultCode = 1, Message = "找不到相应的订单" };
                }

                orderItem.State = 0;
                orderItem.ModifiedOn = DateTime.Now;
                orderItem.EntityState = EntityState.Modified;
                contextSession.SaveObject(orderItem);

                orderRefund.State = 2;
                orderRefund.RefuseTime = DateTime.Now;
                orderRefund.RefuseReason = cancelTheOrderDTO.RefuseReason;
                orderRefund.ModifiedOn = DateTime.Now;
                orderRefund.EntityState = EntityState.Modified;
                contextSession.SaveObject(orderRefund);

                int oldOrderState = commodityOrder.State;
                int oldOrderRefundState = orderRefund.State;
                switch (commodityOrder.State)
                {
                    //如果待发货退款中 更改为待发货
                    case 8:
                        commodityOrder.State = 1;
                        break;
                    //如果已发货退款中 更改为已发货 
                    case 9:
                        commodityOrder.State = 2;
                        break;
                    //如果出库中退款中 更改为出库中
                    case 14:
                        commodityOrder.State = 13;
                        break;
                    //如果已发货退款中商家同意退款，商家未收到货，更改为已发货
                    case 10:
                        commodityOrder.State = 2;
                        break;
                }
                commodityOrder.IsRefund = false;
                commodityOrder.RefundTime = null;
                commodityOrder.ModifiedOn = DateTime.Now;
                commodityOrder.EntityState = System.Data.EntityState.Modified;
                contextSession.SaveObject(commodityOrder);

                int result = contextSession.SaveChanges();

                if (result > 0)
                {
                    BTPMessageSV addmassage = new BTPMessageSV();
                    //发送消息，异步执行
                    System.Threading.ThreadPool.QueueUserWorkItem(
                        a =>
                        {
                            CommodityOrderMessages messageModel = new CommodityOrderMessages();
                            messageModel.IsAuto = false;
                            messageModel.Id = commodityOrder.Id.ToString();
                            messageModel.UserIds = commodityOrder.UserId.ToString();
                            messageModel.AppId = commodityOrder.AppId;
                            messageModel.Code = commodityOrder.Code;
                            messageModel.State = commodityOrder.State;
                            messageModel.RefundType = orderRefund.RefundType;
                            messageModel.RefundMoney = orderRefund.RefundMoney;
                            messageModel.PayType = commodityOrder.Payment;
                            messageModel.orderRefundState = orderRefund.State;
                            messageModel.oldOrderRefundState = oldOrderRefundState;
                            messageModel.RefuseReason = orderRefund.RefuseReason;
                            messageModel.EsAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
                            addmassage.AddMessagesCommodityOrder(messageModel);
                        });
                    try
                    {
                        Journal journal = new Journal();
                        journal.Id = Guid.NewGuid();
                        if (orderRefund.RefundType == 0)
                        {
                            journal.Name = "售中退款拒绝退款请";
                        }
                        else if (orderRefund.RefundType == 1)
                        {
                            journal.Name = "售中退款拒绝退款/退货申请";
                        }
                        journal.Code = commodityOrder.Code;
                        journal.SubId = commodityOrder.UserId;
                        journal.SubTime = DateTime.Now;
                        journal.Details = "订单售后状态由" + oldOrderState + "变为" + commodityOrder.State;
                        journal.CommodityOrderId = commodityOrder.Id;
                        journal.CommodityOrderItemId = cancelTheOrderDTO.OrderItemId;
                        journal.StateFrom = oldOrderState;
                        journal.StateTo = commodityOrder.State;
                        journal.IsPush = false;
                        journal.OrderType = commodityOrder.OrderType;

                        journal.EntityState = System.Data.EntityState.Added;
                        contextSession.SaveObject(journal);
                        contextSession.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("拒绝退款/退货申请记日志异常。", ex);
                    }

                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                }
                return new ResultDTO { ResultCode = 1, Message = "拒绝退款/退货申请时，保存失败" };

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("拒绝退款/退货申请。cancelOrderRefundDTO：{0}", JsonHelper.JsonSerializer(cancelTheOrderDTO)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(cancelTheOrderDTO.OrderId);
            }
        }

        /// <summary>
        /// 拒绝收货
        /// </summary>
        /// <param name="cancelTheOrderDTO"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RefuseRefundOrderSellerExt(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO)
        {
            if (cancelTheOrderDTO == null)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数不能为空" };
            }
            if (cancelTheOrderDTO.OrderItemId != Guid.Empty)
            {
                return RefuseRefundOrderItemSeller(cancelTheOrderDTO);
            }
            if (!OrderSV.LockOrder(cancelTheOrderDTO.OrderId))
            {
                return new ResultDTO { ResultCode = 110, Message = "操作失败" };
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var stateList = new List<int> { 10 };
                var commodityOrderList = (from c in CommodityOrder.ObjectSet()
                                          join r in OrderRefund.ObjectSet() on c.Id equals r.OrderId
                                          where c.Id == cancelTheOrderDTO.OrderId && stateList.Contains(c.State) && r.State == 11
                                          select new
                                          {
                                              commodityOrder = c,
                                              orderRefund = r
                                          }).FirstOrDefault();
                if (commodityOrderList == null)
                {
                    return new ResultDTO { ResultCode = 1, Message = "找不到相应的订单" };
                }

                CommodityOrder commodityOrder = commodityOrderList.commodityOrder;
                OrderRefund orderRefund = commodityOrderList.orderRefund;

                if (commodityOrder == null || orderRefund == null)
                {
                    return new ResultDTO { ResultCode = 1, Message = "找不到相应的订单" };
                }

                int oldState = commodityOrder.State;
                int oldOrderRefundState = orderRefund.State;

                if (orderRefund.AgreeFlag == 0)
                {
                    commodityOrder.State = 13;
                }
                else
                {
                    commodityOrder.State = 2;
                }
                commodityOrder.IsRefund = false;
                commodityOrder.RefundTime = null;
                commodityOrder.ModifiedOn = DateTime.Now;
                commodityOrder.EntityState = System.Data.EntityState.Modified;

                orderRefund.State = 4;
                orderRefund.RefuseTime = DateTime.Now;
                orderRefund.RefuseReason = cancelTheOrderDTO.RefuseReason;
                orderRefund.ModifiedOn = DateTime.Now;
                orderRefund.EntityState = System.Data.EntityState.Modified;

                int result = contextSession.SaveChanges();

                if (result > 0)
                {
                    BTPMessageSV addmassage = new BTPMessageSV();
                    //发送消息，异步执行
                    System.Threading.ThreadPool.QueueUserWorkItem(
                        a =>
                        {
                            CommodityOrderMessages messageModel = new CommodityOrderMessages();
                            messageModel.IsAuto = false;
                            messageModel.Id = commodityOrder.Id.ToString();
                            messageModel.UserIds = commodityOrder.UserId.ToString();
                            messageModel.AppId = commodityOrder.AppId;
                            messageModel.Code = commodityOrder.Code;
                            messageModel.State = commodityOrder.State;
                            messageModel.RefundType = orderRefund.RefundType;
                            messageModel.RefundMoney = orderRefund.RefundMoney;
                            messageModel.PayType = commodityOrder.Payment;
                            messageModel.orderRefundState = orderRefund.State;
                            messageModel.oldOrderRefundState = oldOrderRefundState;
                            messageModel.RefuseReason = orderRefund.RefuseReason;
                            messageModel.EsAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
                            addmassage.AddMessagesCommodityOrder(messageModel);
                        });
                    try
                    {
                        Journal journal = new Journal();
                        journal.Id = Guid.NewGuid();
                        journal.Name = "售中退款拒绝收货";
                        journal.Code = commodityOrder.Code;
                        journal.SubId = commodityOrder.UserId;
                        journal.SubTime = DateTime.Now;
                        journal.Details = "订单状态由" + oldState + "变为" + commodityOrder.State;
                        journal.CommodityOrderId = commodityOrder.Id;
                        journal.StateFrom = oldState;
                        journal.StateTo = commodityOrder.State;
                        journal.IsPush = false;
                        journal.OrderType = commodityOrder.OrderType;

                        journal.EntityState = System.Data.EntityState.Added;
                        contextSession.SaveObject(journal);
                        contextSession.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("拒绝收货记日志异常。", ex);
                    }

                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                }
                return new ResultDTO { ResultCode = 1, Message = "拒绝收货，保存失败" };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("拒绝收货。cancelOrderRefundDTO：{0}", JsonHelper.JsonSerializer(cancelTheOrderDTO)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(cancelTheOrderDTO.OrderId);
            }
        }

        /// <summary>
        /// 拒绝收货 单商品
        /// </summary>
        /// <param name="cancelTheOrderDTO"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RefuseRefundOrderItemSeller(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                CommodityOrder commodityOrder = CommodityOrder.FindByID(cancelTheOrderDTO.OrderId);
                OrderRefund orderRefund = OrderRefund.ObjectSet().Where(t => t.OrderItemId == cancelTheOrderDTO.OrderItemId).OrderByDescending(t => t.SubTime).FirstOrDefault();
                OrderItem orderItem = OrderItem.FindByID(cancelTheOrderDTO.OrderItemId);

                if (commodityOrder == null || orderRefund == null || orderItem == null)
                {
                    return new ResultDTO { ResultCode = 1, Message = "找不到相应的订单" };
                }

                int oldState = commodityOrder.State;
                int oldOrderRefundState = orderRefund.State;

                commodityOrder.State = 2;
                commodityOrder.ModifiedOn = DateTime.Now;
                commodityOrder.EntityState = System.Data.EntityState.Modified;
                contextSession.SaveObject(commodityOrder);

                orderRefund.State = 4;
                orderRefund.RefuseTime = DateTime.Now;
                orderRefund.RefuseReason = cancelTheOrderDTO.RefuseReason;
                orderRefund.RefundExpCo = "";
                orderRefund.RefundExpOrderNo = "";
                orderRefund.ModifiedOn = DateTime.Now;
                orderRefund.EntityState = System.Data.EntityState.Modified;
                contextSession.SaveObject(orderRefund);

                //拒绝收货
                orderItem.State = 5;
                orderItem.ModifiedOn = DateTime.Now;
                orderItem.EntityState = EntityState.Modified;
                contextSession.SaveObject(orderItem);

                int result = contextSession.SaveChanges();
                if (result > 0)
                {
                    BTPMessageSV addmassage = new BTPMessageSV();
                    //发送消息，异步执行
                    System.Threading.ThreadPool.QueueUserWorkItem(
                        a =>
                        {
                            CommodityOrderMessages messageModel = new CommodityOrderMessages();
                            messageModel.IsAuto = false;
                            messageModel.Id = commodityOrder.Id.ToString();
                            messageModel.UserIds = commodityOrder.UserId.ToString();
                            messageModel.AppId = commodityOrder.AppId;
                            messageModel.Code = commodityOrder.Code;
                            messageModel.State = commodityOrder.State;
                            messageModel.RefundType = orderRefund.RefundType;
                            messageModel.RefundMoney = orderRefund.RefundMoney;
                            messageModel.PayType = commodityOrder.Payment;
                            messageModel.orderRefundState = orderRefund.State;
                            messageModel.oldOrderRefundState = oldOrderRefundState;
                            messageModel.RefuseReason = orderRefund.RefuseReason;
                            messageModel.EsAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
                            addmassage.AddMessagesCommodityOrder(messageModel);
                        });
                    try
                    {
                        Journal journal = new Journal();
                        journal.Id = Guid.NewGuid();
                        journal.Name = "售中退款拒绝收货";
                        journal.Code = commodityOrder.Code;
                        journal.SubId = commodityOrder.UserId;
                        journal.SubTime = DateTime.Now;
                        journal.Details = "订单状态由" + oldState + "变为" + commodityOrder.State;
                        journal.CommodityOrderId = commodityOrder.Id;
                        journal.CommodityOrderItemId = cancelTheOrderDTO.OrderItemId;
                        journal.StateFrom = oldState;
                        journal.StateTo = commodityOrder.State;
                        journal.IsPush = false;
                        journal.OrderType = commodityOrder.OrderType;

                        journal.EntityState = System.Data.EntityState.Added;
                        contextSession.SaveObject(journal);
                        contextSession.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("拒绝收货记日志异常。", ex);
                    }

                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                }
                return new ResultDTO { ResultCode = 1, Message = "拒绝收货，保存失败" };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("拒绝收货。cancelOrderRefundDTO：{0}", JsonHelper.JsonSerializer(cancelTheOrderDTO)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(cancelTheOrderDTO.OrderId);
            }
        }

        /// <summary>
        /// 售中卖家延长收货时间
        /// </summary>
        /// <param name="commodityorderId">订单号</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelayConfirmTimeExt(Guid commodityorderId)
        {
            if (commodityorderId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数不能为空" };
            }
            if (!OrderSV.LockOrder(commodityorderId))
            {
                return new ResultDTO { ResultCode = 110, Message = "操作失败" };
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                var orderAndRefund = (from c in CommodityOrder.ObjectSet()
                                      join r in OrderRefund.ObjectSet() on c.Id equals r.OrderId
                                      where c.Id == commodityorderId && (c.State == 10 && r.State == 11)
                                      select new
                                      {
                                          commodityOrder = c,
                                          orderRefund = r
                                      }).FirstOrDefault();
                if (orderAndRefund == null)
                {
                    return new ResultDTO { ResultCode = 1, Message = "找不到相应的订单" };
                }

                CommodityOrder commodityOrder = orderAndRefund.commodityOrder;
                OrderRefund orderRefund = orderAndRefund.orderRefund;

                if (orderRefund == null || commodityOrder == null)
                {
                    return new ResultDTO { ResultCode = 1, Message = "找不到相应的订单" };
                }

                commodityOrder.ModifiedOn = DateTime.Now;
                commodityOrder.EntityState = System.Data.EntityState.Modified;

                orderRefund.IsDelayConfirmTimeAfterSales = true;
                orderRefund.ModifiedOn = DateTime.Now;
                orderRefund.EntityState = System.Data.EntityState.Modified;

                int reslult = contextSession.SaveChanges();
                if (reslult > 0)
                {
                    BTPMessageSV addmassage = new BTPMessageSV();
                    //发送消息，异步执行
                    System.Threading.ThreadPool.QueueUserWorkItem(
                        a =>
                        {
                            CommodityOrderMessages messageModel = new CommodityOrderMessages();
                            messageModel.IsAuto = false;
                            messageModel.Id = commodityOrder.Id.ToString();
                            messageModel.UserIds = commodityOrder.UserId.ToString();
                            messageModel.AppId = commodityOrder.AppId;
                            messageModel.Code = commodityOrder.Code;
                            messageModel.State = commodityOrder.State;
                            // messageModel.RefundType = orderRefund.RefundType;
                            // messageModel.RefundMoney = orderRefund.RefundMoney;
                            messageModel.PayType = commodityOrder.Payment;
                            //messageModel.orderRefundState = orderRefund.State;
                            messageModel.IsSellerDelayTime = true;
                            messageModel.EsAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
                            addmassage.AddMessagesCommodityOrder(messageModel);
                        });
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                }
                else
                {
                    return new ResultDTO { ResultCode = 1, Message = "延长收货时间失败" };
                }
            }
            catch (Exception ex)
            {

                LogHelper.Error(string.Format("售中卖家延长收货时间服务异常。commodityorderId：{0}", commodityorderId.ToString()), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(commodityorderId);
            }
        }

        /// <summary>
        /// 申请列表
        /// </summary>
        /// <param name="refundInfoDTO"></param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundDTO> GetRefundInfoExt(Jinher.AMP.BTP.Deploy.CustomDTO.RefundInfoDTO refundInfoDTO)
        {
            var result = new List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundDTO>();
            if (refundInfoDTO == null)
            {
                return result;
            }
            var orderMiddleList = new List<Deploy.CustomDTO.OrderRefundDTO>();
            var orderAfterList = new List<Deploy.CustomDTO.OrderRefundDTO>();

            if (refundInfoDTO.CommodityOrderItemId != Guid.Empty)
            {
                orderMiddleList = (from refund in OrderRefund.ObjectSet()
                                   join orderItem in OrderItem.ObjectSet() on refund.OrderItemId equals orderItem.Id
                                   where
                                       refund.OrderId == refundInfoDTO.CommodityOrderId &&
                                       refund.OrderItemId == refundInfoDTO.CommodityOrderItemId
                                   orderby refund.SubTime descending
                                   select new Deploy.CustomDTO.OrderRefundDTO
                                   {
                                       Id = refund.Id,
                                       RefundType = refund.RefundType,
                                       RefundReason = refund.RefundReason,
                                       RefundMoney = refund.RefundMoney,
                                       RefundDesc = refund.RefundDesc,
                                       OrderId = refund.OrderId,
                                       State = refund.State,
                                       ReceiverAccount = refund.ReceiverAccount,
                                       Receiver = refund.Receiver,
                                       RefundExpCo = refund.RefundExpCo,
                                       RefundExpOrderNo = refund.RefundExpOrderNo,
                                       OrderRefundImgs = refund.OrderRefundImgs,
                                       DataType = refund.DataType,
                                       OrderItemId = refund.OrderItemId,
                                       RefuseTime = refund.RefuseTime,
                                       IsFullRefund = refund.IsFullRefund == null ? -1 : refund.IsFullRefund.Value ? 1 : 0,
                                       RefuseReason = refund.RefuseReason,
                                       NotReceiveTime = refund.NotReceiveTime,
                                       SubTime = refund.SubTime,
                                       ModifiedOn = refund.ModifiedOn,
                                       RefundExpOrderTime = refund.RefundExpOrderTime,
                                       RefundScoreMoney = refund.RefundScoreMoney,
                                       SalerRemark = refund.SalerRemark,
                                       RefundYJBMoney = refund.RefundYJBMoney,
                                       Name = orderItem.Name,
                                       Pic = orderItem.PicturesPath,
                                       Price = (decimal)orderItem.RealPrice,
                                       Num = orderItem.Number
                                   }).ToList();
            }
            else
            {
                orderMiddleList = (from refund in OrderRefund.ObjectSet()
                                   where refund.OrderId == refundInfoDTO.CommodityOrderId
                                   orderby refund.SubTime descending
                                   select new Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundDTO
                                   {
                                       Id = refund.Id,
                                       RefundType = refund.RefundType,
                                       RefundReason = refund.RefundReason,
                                       RefundMoney = refund.RefundMoney,
                                       RefundDesc = refund.RefundDesc,
                                       OrderId = refund.OrderId,
                                       State = refund.State,
                                       ReceiverAccount = refund.ReceiverAccount,
                                       Receiver = refund.Receiver,
                                       RefundExpCo = refund.RefundExpCo,
                                       RefundExpOrderNo = refund.RefundExpOrderNo,
                                       OrderRefundImgs = refund.OrderRefundImgs,
                                       DataType = refund.DataType,
                                       OrderItemId = refund.OrderItemId,
                                       RefuseTime = refund.RefuseTime,
                                       IsFullRefund = refund.IsFullRefund == null ? -1 : refund.IsFullRefund.Value ? 1 : 0,
                                       RefuseReason = refund.RefuseReason,
                                       NotReceiveTime = refund.NotReceiveTime,
                                       SubTime = refund.SubTime,
                                       ModifiedOn = refund.ModifiedOn,
                                       RefundExpOrderTime = refund.RefundExpOrderTime,
                                       RefundScoreMoney = refund.RefundScoreMoney,
                                       SalerRemark = refund.SalerRemark,
                                       RefundYJBMoney = refund.RefundYJBMoney
                                   }).ToList();
            }

            if (refundInfoDTO.CommodityOrderItemId != Guid.Empty)
            {
                orderAfterList = (from refund in OrderRefundAfterSales.ObjectSet()
                                  join orderItem in OrderItem.ObjectSet() on refund.OrderItemId equals orderItem.Id
                                  where
                                      refund.OrderId == refundInfoDTO.CommodityOrderId &&
                                      refund.OrderItemId == refundInfoDTO.CommodityOrderItemId
                                  orderby refund.SubTime descending
                                  select new Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundDTO
                                  {
                                      Id = refund.Id,
                                      RefundType = refund.RefundType,
                                      RefundReason = refund.RefundReason,
                                      RefundMoney = refund.RefundMoney,
                                      RefundDesc = refund.RefundDesc,
                                      OrderId = refund.OrderId,
                                      State = refund.State,
                                      ReceiverAccount = refund.ReceiverAccount,
                                      Receiver = refund.Receiver,
                                      RefundExpCo = refund.RefundExpCo,
                                      RefundExpOrderNo = refund.RefundExpOrderNo,
                                      OrderRefundImgs = refund.OrderRefundImgs,
                                      DataType = refund.DataType,
                                      OrderItemId = refund.OrderItemId,
                                      RefuseTime = refund.RefuseTime,
                                      IsFullRefund = refund.IsFullRefund == null ? -1 : refund.IsFullRefund.Value,
                                      RefuseReason = refund.RefuseReason,
                                      NotReceiveTime = refund.NotReceiveTime,
                                      SubTime = refund.SubTime,
                                      ModifiedOn = refund.ModifiedOn,
                                      RefundExpOrderTime = refund.RefundExpOrderTime,
                                      RefundScoreMoney = refund.RefundScoreMoney,
                                      SalerRemark = refund.SalerRemark,
                                      RefundYJBMoney = refund.RefundYJBMoney,
                                      Name = orderItem.Name,
                                      Pic = orderItem.PicturesPath,
                                      Price = (decimal)orderItem.RealPrice,
                                      Num = orderItem.Number,
                                      JDEclpOrderRefundAfterSalesId = refund.JDEclpOrderRefundAfterSalesId,
                                      PickwareType = refund.PickwareType,
                                      PickUpFreightMoney = refund.PickUpFreightMoney ?? 0,
                                      SendBackFreightMoney = refund.SendBackFreightMoney ?? 0,
                                      Address = new AddressInfo
                                      {
                                          customerContactName = refund.CustomerContactName,
                                          customerTel = refund.CustomerTel,
                                          pickwareAddress = refund.PickwareAddress
                                      }
                                  }).ToList();
                orderAfterList.ForEach(p =>
                {
                    if (p.JDEclpOrderRefundAfterSalesId.HasValue && p.JDEclpOrderRefundAfterSalesId != Guid.Empty)
                    {
                        p.IsJdEclpOrder = true;
                        var jdEclpService = JDEclpOrderRefundAfterSales.ObjectSet().Where(x => x.Id == p.JDEclpOrderRefundAfterSalesId.Value).FirstOrDefault();
                        if (jdEclpService != null)
                        {
                            p.JdEclpServiceInfo = new JDEclpOrderRefundAfterSalesExtraDTO
                            {
                                EclpServicesState = jdEclpService.EclpServicesState,
                                ItemList = JDEclpOrderRefundAfterSalesItem.ObjectSet()
                                    .Where(x => x.JDEclpOrderRefundAfterSalesId == jdEclpService.Id)
                                    .ToList().Select(x => x.ToEntityData()).ToList()
                            };
                        }
                    }
                });
            }
            else
            {
                orderAfterList = (from refund in OrderRefundAfterSales.ObjectSet()
                                  where refund.OrderId == refundInfoDTO.CommodityOrderId
                                  orderby refund.SubTime descending
                                  select new Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundDTO
                                  {
                                      Id = refund.Id,
                                      RefundType = refund.RefundType,
                                      RefundReason = refund.RefundReason,
                                      RefundMoney = refund.RefundMoney,
                                      RefundDesc = refund.RefundDesc,
                                      OrderId = refund.OrderId,
                                      State = refund.State,
                                      ReceiverAccount = refund.ReceiverAccount,
                                      Receiver = refund.Receiver,
                                      RefundExpCo = refund.RefundExpCo,
                                      RefundExpOrderNo = refund.RefundExpOrderNo,
                                      OrderRefundImgs = refund.OrderRefundImgs,
                                      DataType = refund.DataType,
                                      OrderItemId = refund.OrderItemId,
                                      RefuseTime = refund.RefuseTime,
                                      IsFullRefund = refund.IsFullRefund == null ? -1 : refund.IsFullRefund.Value,
                                      RefuseReason = refund.RefuseReason,
                                      NotReceiveTime = refund.NotReceiveTime,
                                      SubTime = refund.SubTime,
                                      ModifiedOn = refund.ModifiedOn,
                                      RefundExpOrderTime = refund.RefundExpOrderTime,
                                      RefundScoreMoney = refund.RefundScoreMoney,
                                      SalerRemark = refund.SalerRemark,
                                      RefundYJBMoney = refund.RefundYJBMoney
                                  }).ToList();
            }
            var orderList = orderMiddleList.Union(orderAfterList);
            result = orderList.OrderByDescending(t => t.SubTime)
                    .Skip((refundInfoDTO.PageIndex - 1) * refundInfoDTO.PageSize)
                    .Take(refundInfoDTO.PageSize)
                    .ToList();

            return result;
        }

        /// <summary>
        /// 获取订单相关信息（订单，售后， 退款，分润设置，钱款去向，订单项）
        /// </summary>
        /// <param name="orderIdStr">商品订单ID或订单编号</param>
        /// <returns></returns>
        public OrderFullInfo GetFullOrderInfoByIdExt(string orderIdStr)
        {
            OrderFullInfo ofi = new OrderFullInfo();

            if (string.IsNullOrWhiteSpace(orderIdStr))
            {
                return ofi;
            }
            orderIdStr = orderIdStr.Trim();

            Guid orderId = Guid.Empty;
            Guid.TryParse(orderIdStr, out orderId);

            //订单信息

            CommodityOrder cOrder = null;
            if (orderId == Guid.Empty)
            {
                cOrder = (from o in CommodityOrder.ObjectSet()
                          where o.Code == orderIdStr && o.State != 16 && o.State != 17
                          select o).FirstOrDefault();
            }
            else
            {
                cOrder = (from o in CommodityOrder.ObjectSet()
                          where o.Id == orderId && o.State != 16 && o.State != 17
                          select o).FirstOrDefault();
            }
            if (cOrder == null)
                return ofi;

            orderId = cOrder.Id;



            var co = new CommodityOrderVM();
            co.FillWith(cOrder);
            co.CommodityOrderId = cOrder.Id;
            co.CommodityOrderCode = cOrder.Code;
            co.SubTime = cOrder.SubTime;
            co.PaymentTime = cOrder.PaymentTime;


            var orderPayDetails = OrderPayDetail.ObjectSet().Where(c => c.OrderId == orderId).ToList();
            if (orderPayDetails.Any())
            {
                //1、优惠券
                var couponDetail = orderPayDetails.FirstOrDefault(c => c.ObjectType == 1);
                if (couponDetail != null)
                    co.CouponValue = couponDetail.Amount;

                //2、积分抵现
                var scoreDetail = orderPayDetails.FirstOrDefault(c => c.ObjectType == 2);
                if (scoreDetail != null)
                    co.SpendScoreMoney = scoreDetail.Amount;

                //3、运费折扣优惠
                var freightIntensityDetail = orderPayDetails.FirstOrDefault(c => c.ObjectType == 3);
                if (freightIntensityDetail != null)
                    co.FreightDiscount = freightIntensityDetail.Amount;

                //4、满减免运费
                var freightFreeDetail = orderPayDetails.FirstOrDefault(c => c.ObjectType == 4);
                if (freightFreeDetail != null)
                    co.FreightDiscount = freightFreeDetail.Amount;
            }


            ofi.OrderVM = co;





            //售后
            var coas = (from oas in CommodityOrderService.ObjectSet()
                        where oas.Id == orderId
                        select oas).FirstOrDefault();
            if (coas != null)
            {
                var coServiceDTO = new CommodityOrderServiceDTO();
                coServiceDTO.FillWith(coas);
                ofi.CoServiceDTO = coServiceDTO;
            }

            //订单项
            List<Guid> cids = new List<Guid>();
            var oiQuery = (from oi in OrderItem.ObjectSet()
                           where oi.CommodityOrderId == orderId
                           select oi).ToList();
            if (oiQuery.Any())
            {

                ofi.OrderItems = oiQuery.ConvertAll<OrderItemsVM>(ConvertOrderItemBE2VM);
                foreach (var item in ofi.OrderItems)
                {
                    var commodity = Commodity.ObjectSet().FirstOrDefault(p => p.Id == item.CommodityId);
                    if (commodity != null)
                    {
                        item.JdCode = commodity.JDCode;
                    }
                }
                cids.AddRange(oiQuery.Select(oi => oi.CommodityId));
            }

            //售中退款
            var orQuery = (from or in OrderRefund.ObjectSet()
                           where or.OrderId == orderId
                           select or).ToList();
            if (orQuery.Any())
            {
                ofi.OrList = orQuery.ConvertAll<Jinher.AMP.BTP.Deploy.OrderRefundDTO>(or => or.ToEntityData());
            }

            //售后退款
            var orasQuery = (from oras in OrderRefundAfterSales.ObjectSet()
                             where oras.OrderId == orderId
                             select oras).ToList();
            if (orasQuery.Any())
            {
                ofi.OrasList = orasQuery.ConvertAll<Jinher.AMP.BTP.Deploy.OrderRefundAfterSalesDTO>(oras => oras.ToEntityData());
            }

            //分润设置
            if (cids.Any())
            {
                var saQuery = (from sa in SettlingAccount.ObjectSet()
                               where cids.Contains(sa.CommodityId)
                               select sa).ToList();
                if (saQuery.Any())
                {
                    ofi.SaList = saQuery.ConvertAll<SettlingAccountDTO>(sa => sa.ToEntityData());
                }
            }

            //钱款去向
            var opQuery = (from op in OrderPayee.ObjectSet()
                           where op.OrderId == orderId
                           select op).ToList();
            if (opQuery.Any())
            {
                ofi.OrderPayeeList = opQuery.ConvertAll(op => op.ToEntityData());
            }

            return ofi;
        }


        private OrderItemsVM ConvertOrderItemBE2VM(OrderItem oi)
        {
            OrderItemsVM oiVm = new OrderItemsVM();
            oiVm.FillWith(oi);
            return oiVm;
        }
        /// <summary>
        /// 查询分销订单
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderDistributionResultDTO GetDistributeOrderListExt(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderDistributionSearchDTO search)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderDistributionResultDTO result = new CommodityOrderDistributionResultDTO();

            if (search == null)
            {
                return result;
            }

            //按销量查询
            if (search.SearchType == 1)
            {
                var query = from order in CommodityOrder.ObjectSet()
                            join orderAfterSales in CommodityOrderService.ObjectSet() on order.Id equals orderAfterSales.Id
                            join orderShare in OrderShare.ObjectSet() on order.Id equals orderShare.OrderId
                            where orderAfterSales.State == 15 && orderShare.PayeeType == 9
                            select new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderDistributionInfoDTO
                            {
                                AppId = order.AppId,
                                Code = order.Code,
                                DistributeMoney = order.DistributeMoney,
                                EndTime = orderAfterSales.EndTime == null ? DateTime.MinValue : orderAfterSales.EndTime.Value,
                                Id = order.Id,
                                Price = order.RealPrice == null ? 0 : order.RealPrice.Value,
                                DistributorId = order.DistributorId == null ? Guid.Empty : order.DistributorId.Value
                            };

                if (search.DistributorId != Guid.Empty)
                {
                    query = query.Where(t => t.DistributorId == search.DistributorId);
                }
                //订单编号模糊匹配
                if (!string.IsNullOrWhiteSpace(search.OrderCode))
                {
                    query = query.Where(t => t.Code.Contains(search.OrderCode));
                }
                if (search.FinishTimeEnd != null)
                {
                    query = query.Where(t => t.EndTime <= search.FinishTimeEnd);
                }
                if (search.FinishTimeStart != null)
                {
                    query = query.Where(t => t.EndTime >= search.FinishTimeStart);
                }
                // query = query.Where(t => t.DistributeMoney > 0);
                var count = query.Count();
                var resultData = query.OrderByDescending(t => t.EndTime).Skip((search.PageIndex - 1) * search.PageSize).Take(search.PageSize).ToList();
                result.Count = count;
                result.CommodityOrderDistributionInfoList = resultData;
            }
            //按佣金查询
            else if (search.SearchType == 2)
            {
                var distributorId = search.DistributorId.ToString();
                var payeeTypeArr = new int[] { 9, 10, 11 };
                var orderShareList = OrderShare.ObjectSet().Where(t => t.ShareKey == distributorId && payeeTypeArr.Contains(t.PayeeType)).Select(t => new OrderShareDTO { OrderId = t.OrderId, ShareKey = t.ShareKey });
                if (orderShareList.Any())
                {
                    var query = from order in CommodityOrder.ObjectSet()
                                join orderAfterSales in CommodityOrderService.ObjectSet() on order.Id equals orderAfterSales.Id
                                join orderShare in orderShareList on order.Id equals orderShare.OrderId
                                where orderAfterSales.State == 15
                                select new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderDistributionInfoDTO
                                {
                                    AppId = order.AppId,
                                    Code = order.Code,
                                    DistributeMoney = order.DistributeMoney,
                                    EndTime = orderAfterSales.EndTime.Value,
                                    Id = order.Id,
                                    Price = order.RealPrice.Value,
                                    DistributorIdStr = orderShare.ShareKey
                                };

                    if (search.DistributorId != Guid.Empty)
                    {
                        query = query.Where(t => t.DistributorIdStr == distributorId);
                    }
                    if (!string.IsNullOrWhiteSpace(search.OrderCode))
                    {
                        query = query.Where(t => t.Code == search.OrderCode);
                    }
                    if (search.FinishTimeEnd != null)
                    {
                        query = query.Where(t => t.EndTime <= search.FinishTimeEnd);
                    }
                    if (search.FinishTimeStart != null)
                    {
                        query = query.Where(t => t.EndTime >= search.FinishTimeStart);
                    }
                    query = query.OrderByDescending(c => c.EndTime);
                    // query = query.Where(t => t.DistributeMoney > 0);
                    var count = query.Count();
                    var resultData = query.Skip((search.PageIndex - 1) * search.PageSize).Take(search.PageSize).ToList();
                    resultData.ForEach(c => c.DistributorId = new Guid(c.DistributorIdStr));
                    result.Count = count;
                    result.CommodityOrderDistributionInfoList = resultData;
                }
            }
            else
            {
                //待定
            }
            return result;
        }


        private ResultDTO CancelTheOrderCallBackExt(CancelTheOrderDTO cancelTheOrderDTO)
        {
            if (cancelTheOrderDTO == null || cancelTheOrderDTO.OrderId == Guid.Empty)
                return new ResultDTO { ResultCode = 1, Message = "参数为空" };
            if (!OrderSV.LockOrder(cancelTheOrderDTO.OrderId))
            {
                return new ResultDTO { ResultCode = 110, Message = "操作失败" };
            }
            try
            {
                CommodityOrder commodityOrder =
                    CommodityOrder.ObjectSet().FirstOrDefault(n => n.Id == cancelTheOrderDTO.OrderId);
                if (commodityOrder == null)
                    return new ResultDTO { ResultCode = 1, Message = "订单无效" };
                if (commodityOrder.State == 7)
                    return new ResultDTO { ResultCode = 0, Message = "Success" };

                DateTime now = DateTime.Now;
                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                if (commodityOrder.State == 12)
                {
                    List<int> rState = new List<int> { 2, 3, 4, 13 };
                    OrderRefund orderRefund = OrderRefund.ObjectSet().FirstOrDefault(n => n.OrderId == cancelTheOrderDTO.OrderId && !rState.Contains(n.State));
                    if (orderRefund != null)
                    {
                        LogHelper.Info(string.Format("CommodityOrderBP.CancelTheOrderCallBackExt.1:OrderId={0}&OrderRefundId={1}", orderRefund.OrderId, orderRefund.Id));
                        orderRefund.ModifiedOn = now;
                        orderRefund.State = 1;
                        //整单退票走之前的逻辑
                        if (orderRefund.OrderItemId == null || orderRefund.OrderItemId == Guid.Empty)
                        {
                            Journal journal = Journal.CreateJournal();
                            journal.Id = Guid.NewGuid();
                            journal.Name = "取消订单金币回调";
                            journal.Code = commodityOrder.Code;
                            journal.SubId = cancelTheOrderDTO.UserId;
                            journal.SubTime = DateTime.Now;
                            journal.Details = "订单状态由12变为7";
                            journal.CommodityOrderId = cancelTheOrderDTO.OrderId;
                            journal.StateFrom = 12;
                            journal.StateTo = 7;
                            journal.IsPush = false;
                            journal.OrderType = commodityOrder.OrderType;

                            journal.EntityState = System.Data.EntityState.Added;
                            contextSession.SaveObject(journal);

                            commodityOrder.State = 7;
                            commodityOrder.ModifiedOn = now;
                        }
                    }
                }
                else if (commodityOrder.State == 20)
                {
                    List<int> rState = new List<int> { 2, 3, 4, 13, 20 };
                    OrderRefund orderRefund =
                        OrderRefund.ObjectSet()
                                   .FirstOrDefault(
                                       n => n.OrderId == cancelTheOrderDTO.OrderId && !rState.Contains(n.State));

                    if (orderRefund != null)
                    {
                        orderRefund.ModifiedOn = now;
                        orderRefund.State = 21;
                    }

                    Journal journal = Journal.CreateJournal();
                    journal.Id = Guid.NewGuid();
                    journal.Name = "取消订单全额退款回调";
                    journal.Code = commodityOrder.Code;
                    journal.SubId = cancelTheOrderDTO.UserId;
                    journal.SubTime = DateTime.Now;
                    journal.Details = "订单状态由20变为21";
                    journal.CommodityOrderId = cancelTheOrderDTO.OrderId;
                    journal.StateFrom = 20;
                    journal.StateTo = 21;
                    journal.IsPush = false;
                    journal.OrderType = commodityOrder.OrderType;

                    journal.EntityState = System.Data.EntityState.Added;
                    contextSession.SaveObject(journal);

                    commodityOrder.State = 21;
                    commodityOrder.ModifiedOn = now;
                }

                //直接到账 单商品退款
                var orderItemIds = OrderRefund.ObjectSet().Where(n => n.OrderId == cancelTheOrderDTO.OrderId).Select(t => t.OrderItemId).Distinct();
                if (orderItemIds.Any() && orderItemIds.First() != null)
                {
                    foreach (var orderItemId in orderItemIds)
                    {
                        var orderRefund = OrderRefund.ObjectSet().Where(n => n.OrderItemId == orderItemId).OrderByDescending(t => t.SubTime).FirstOrDefault();
                        if (orderRefund != null)
                        {
                            LogHelper.Info(string.Format("CommodityOrderBP.CancelTheOrderCallBackExt.2:OrderId={0}&OrderRefundId={1}", orderRefund.OrderId, orderRefund.Id));
                            orderRefund.State = 1;
                            orderRefund.ModifiedOn = now;
                        }
                    }
                }

                contextSession.SaveChanges();
                return new ResultDTO { ResultCode = 0, Message = "Success" };
            }
            catch (Exception ex)
            {
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(cancelTheOrderDTO.OrderId);
            }
        }


        /// <summary>
        /// 获取订单来源
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderMoneyToDTO GetOrderSourceExt(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderMoneyToSearch search)
        {
            CommodityOrderMoneyToDTO result = new CommodityOrderMoneyToDTO();
            List<Guid?> appList = new List<Guid?>();
            if (search.AppId == Guid.Empty && search.AppIds != null)
            {
                appList = CommodityOrder.ObjectSet().Where(t => search.AppIds.Contains(t.AppId)).Select(t => t.EsAppId).Distinct().ToList();
                int count = 0;
                foreach (var item in search.AppIds)
                {
                    appList.Remove(item);
                    appList.Insert(count, item);
                    count++;

                }
            }
            if (search.AppId != Guid.Empty && search.AppIds == null)
            {
                appList = CommodityOrder.ObjectSet().Where(t => t.AppId == search.AppId).Select(t => t.EsAppId).Distinct().ToList();
                appList.Remove(search.AppId);
                appList.Insert(0, search.AppId);
            }
            if (search.AppId == Guid.Empty && search.AppIds == null)
            {
                return result;
            }
            return GetOrderSourceAppName(result, appList);
        }



        private static CommodityOrderMoneyToDTO GetOrderSourceAppName(CommodityOrderMoneyToDTO result, List<Guid?> appList)
        {
            var appSearch = new List<Guid>();
            foreach (Guid? appid in appList)
            {
                if (appid.HasValue)
                {
                    appSearch.Add(appid.Value);
                }
            }
            var appIdNames = APPSV.GetAppNameListByIds(appSearch);
            if (appIdNames != null && appIdNames.Count > 0)
            {
                result.Count = appIdNames.Count;
                result.Data = new List<CommodityOrderMoneyToModelDTO>();
                foreach (KeyValuePair<Guid, string> appTmp in appIdNames)
                {
                    CommodityOrderMoneyToModelDTO model = new CommodityOrderMoneyToModelDTO()
                    {
                        EsAppId = appTmp.Key,
                        EsAppName = appTmp.Value
                    };
                    result.Data.Add(model);
                }
            }

            return result;
        }

        /// <summary>
        /// 获取app的钱款去向
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderMoneyToDTO GetCommodityOrderMoneyToExt(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderMoneyToSearch search)
        {
            CommodityOrderMoneyToDTO result = new CommodityOrderMoneyToDTO();
            List<int> directArrivalPayments = new PaySourceBP().GetDirectArrivalPaymentExt();
            List<Guid?> appList = new List<Guid?>();
            if (search.AppId == Guid.Empty && search.AppIds != null)
            {
                appList = CommodityOrder.ObjectSet().Where(t => search.AppIds.Contains(t.AppId) && directArrivalPayments.Contains(t.Payment)).Select(t => t.EsAppId).Distinct().ToList();
                int count = 0;
                foreach (var item in search.AppIds)
                {
                    appList.Remove(item);
                    appList.Insert(count, item);
                    count++;

                }
            }
            if (search.AppId != Guid.Empty && search.AppIds == null)
            {
                appList = CommodityOrder.ObjectSet().Where(t => t.AppId == search.AppId && directArrivalPayments.Contains(t.Payment)).Select(t => t.EsAppId).Distinct().ToList();
                appList.Remove(search.AppId);
                appList.Insert(0, search.AppId);
            }
            if (search.AppId == Guid.Empty && search.AppIds == null)
            {
                return result;
            }
            var appSearch = new List<Guid>();
            foreach (Guid? appid in appList)
            {
                if (appid.HasValue)
                {
                    appSearch.Add(appid.Value);
                }
            }
            var appIdNames = APPSV.GetAppNameListByIds(appSearch);
            if (appIdNames != null && appIdNames.Count > 0)
            {
                result.Count = appIdNames.Count;
                result.Data = new List<CommodityOrderMoneyToModelDTO>();
                foreach (KeyValuePair<Guid, string> appTmp in appIdNames)
                {
                    CommodityOrderMoneyToModelDTO model = new CommodityOrderMoneyToModelDTO()
                    {
                        EsAppId = appTmp.Key,
                        EsAppName = appTmp.Value
                    };
                    result.Data.Add(model);
                }
            }

            return result;
        }

        /// <summary>
        /// 获取订单有效的退款
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public OrderRefund getOrderRefund(Guid orderId)
        {
            List<int> rState = new List<int> { 2, 3, 4, 13 };
            return OrderRefund.ObjectSet().Where(n => n.OrderId == orderId && !rState.Contains(n.State)).FirstOrDefault();
        }

        /// <summary>
        /// 获取餐饮订单有效的退款
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        private OrderRefund GetCYOrderRefund(Guid orderId)
        {
            List<int> rState = new List<int> { 2, 3, 4, 13, 20 };
            return OrderRefund.ObjectSet().Where(n => n.OrderId == orderId && !rState.Contains(n.State)).FirstOrDefault();
        }

        /// <summary>
        ///  修改订单
        /// </summary>
        ///<param name="ucopDto">参数实体</param>
        /// <returns></returns>
        public ResultDTO CancelTheOrderExt(UpdateCommodityOrderParamDTO ucopDto)
        {
            ResultDTO result = new ResultDTO { ResultCode = 0, Message = "Success" };
            if (ucopDto == null || ucopDto.orderId == Guid.Empty)
            {
                result.ResultCode = 1;
                result.Message = "参数错误，参数不能为空！";
                return result;
            }
            string s = JsonHelper.JsonSerializer<UpdateCommodityOrderParamDTO>(ucopDto);
            LogHelper.Info(string.Format("CancelTheOrderExt参数：{0}", s));
            OrderSV.UnLockOrder(ucopDto.orderId);
            if (!OrderSV.LockOrder(ucopDto.orderId))
            {
                return new ResultDTO { ResultCode = 110, Message = "操作失败" };
            }
            try
            {
                CommodityOrder commodityOrder = CommodityOrder.ObjectSet().Where(n => n.Id == ucopDto.orderId).FirstOrDefault();

                commodityOrder.MessageToBuyer = ucopDto.remessage;
                commodityOrder.ModifiedOn = DateTime.Now;
                commodityOrder.EntityState = System.Data.EntityState.Modified;

                LogHelper.Debug($"lzh-CancelTheOrderExt2:{JsonHelper.JsonSerializer(ucopDto)}");
                int state = ucopDto.targetState;
                //已发货
                if (state == 2)
                {
                    result = UpdateOrderStateTo2(ucopDto, commodityOrder);
                }
                else if (state == 13)
                {
                    result = UpdateOrderStateTo13(ucopDto, commodityOrder);
                }
                //确认收货
                else if (state == 3)
                {
                    result = UpdateOrderStateTo3(ucopDto, commodityOrder);
                }
                //取消订单
                else if (state == 4)
                {
                    result = UpdateOrderStateTo4(ucopDto, commodityOrder);
                }
                else if (state == 7)
                {
                    // 确认整单退货
                    result = UpdateOrderStateTo7(ucopDto, commodityOrder);
                }
                else if (state == 10)
                {
                    result = UpdateOrderStateTo10(ucopDto, commodityOrder);
                }
                else if (state == 19)
                {
                    result = UpdateOrderStateTo19(ucopDto, commodityOrder);
                }
                // 餐饮订单商家取消订单
                else if (state == 20)
                {
                    result = UpdateOrderStateTo20(ucopDto, commodityOrder);
                }
                //订单单品退款
                else if (state == 21)
                {
                    //LogHelper.Debug($"lzh-CancelTheOrderExt4:{JsonHelper.JsonSerializer(commodityOrder)}");
                    if (commodityOrder.State == 2 || commodityOrder.State == 9)
                    {
                        result = UpdateOrderStateTo10(ucopDto, commodityOrder);
                    }
                    else
                    {
                        result = UpdateOrderItemStateTo7(ucopDto, commodityOrder);
                    }
                }
                else
                {
                    result.ResultCode = 1;
                    result.Message = "订单状态不存在";
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error("CancelTheOrderExt异常,异常信息：", ex);
                result.ResultCode = 1;
                result.Message = "Error";
            }
            finally
            {
                OrderSV.UnLockOrder(ucopDto.orderId);
            }
            return result;
        }

        /// <summary>
        ///  批量修改订单状态为出库中
        /// </summary>
        ///<param name="commodityOrderIds"></param>
        /// <returns></returns>
        public ResultDTO BatchUpdateCommodityOrderExt(string commodityOrderIds)
        {
            ResultDTO result = new ResultDTO { ResultCode = 0, Message = "Success" };
            if (string.IsNullOrEmpty(commodityOrderIds))
            {
                result.ResultCode = 1;
                result.Message = "参数错误，参数不能为空！";
                return result;
            }
            try
            {
                bool isExist = false;
                var ids = commodityOrderIds.Split(',');
                foreach (var id in ids)
                {
                    if (string.IsNullOrEmpty(id))
                    {
                        continue;
                    }
                    Guid orderId = new Guid(id);
                    CommodityOrder commodityOrder = CommodityOrder.FindByID(orderId);

                    if (commodityOrder.State == 1)
                    {
                        isExist = true;
                        commodityOrder.MessageToBuyer = "";
                        commodityOrder.ModifiedOn = DateTime.Now;
                        commodityOrder.EntityState = System.Data.EntityState.Modified;

                        UpdateCommodityOrderParamDTO dto = new UpdateCommodityOrderParamDTO
                        {
                            orderId = orderId,
                            targetState = 13,
                            remessage = "",
                            userId = commodityOrder.SubId
                        };
                        //已发货
                        result = UpdateOrderStateTo2(dto, commodityOrder);
                    }
                }

                if (!isExist)
                {
                    result.ResultCode = 1;
                    result.Message = "请选择待发货状态的订单";
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("BatchUpdateCommodityOrderExt异常,异常信息：", ex);
                result.ResultCode = 1;
                result.Message = "Error";
            }
            return result;
        }


        private ResultDTO UpdateOrderStateTo7(UpdateCommodityOrderParamDTO ucopDto, CommodityOrder commodityOrder)
        {
            ResultDTO result = new ResultDTO();

            List<Commodity> needRefreshCacheCommoditys = new List<Commodity>();
            List<TodayPromotion> needRefreshCacheTodayPromotions = new List<TodayPromotion>();
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            DateTime now = DateTime.Now;
            int oldState = commodityOrder.State;
            int newState = ucopDto.targetState;

            #region 餐饮订单退款处理

            if (commodityOrder.OrderType == 2)
            {
                return UpdateCYOrderStateTo7(ucopDto, commodityOrder);
            }

            #endregion

            var orderRefund = getOrderRefund(commodityOrder.Id);
            if (orderRefund == null)
                return new ResultDTO { ResultCode = 1, Message = "当前订单状态错误,请重试" };
            if (!OrderSV.CanChangeState(newState, commodityOrder, orderRefund, null, null))
            {
                return new ResultDTO() { ResultCode = 1, Message = "订单状态修改错误" };
            }

            if (ucopDto.RejectFreightMoney > 0 && ucopDto.RejectFreightMoney >= orderRefund.RefundMoney) return new ResultDTO() { ResultCode = 1, Message = "拒收运费金额须小于退款金额" };
            decimal orRefundMoney = orderRefund.RefundMoney - ucopDto.RejectFreightMoney;

            if (oldState != 12)
            {
                #region 加库存

                //只有未发货的订单退款才会加库存
                if (oldState == 8)
                {
                    var orderitemlist = OrderItem.ObjectSet().Where(n => n.CommodityOrderId == commodityOrder.Id);
                    var commodityIds = orderitemlist.Select(c => c.CommodityId).Distinct().ToList();
                    var coms = Commodity.ObjectSet().Where(c => commodityIds.Contains(c.Id)).ToList();
                    foreach (OrderItem items in orderitemlist)
                    {
                        Guid comId = items.CommodityId;
                        Commodity com = coms.FirstOrDefault(n => n.Id == comId);
                        if (com == null)
                            continue;
                        com.Stock += items.Number;
                        com.ModifiedOn = now;
                        com.EntityState = System.Data.EntityState.Modified;
                        if (items.CommodityStockId.HasValue && items.CommodityStockId.Value != Guid.Empty && items.CommodityStockId != com.Id)
                        {
                            var cs = CommodityStock.FindByID(items.CommodityStockId.Value);
                            cs.Stock += items.Number;
                            cs.ModifiedOn = now;
                            cs.EntityState = System.Data.EntityState.Modified;
                        }
                        needRefreshCacheCommoditys.Add(com);
                    }
                }

                #endregion

                #region 退款
                var tradeType = PaySource.GetTradeType(commodityOrder.Payment);
                if (commodityOrder.RealPrice > 0 && (tradeType == TradeTypeEnum.SecTrans || tradeType == TradeTypeEnum.Direct && CustomConfig.IsSystemDirectRefund))
                {
                    ReturnInfoDTO cancelPayResult = new ReturnInfoDTO();
                    orRefundMoney = orderRefund.RefundMoney - ucopDto.RejectFreightMoney;
                    LogHelper.Info(string.Format("UpdateOrderStateTo7，开始退款：OrderId={0}&OrderRefundId={1}&orRefundMoney={2}", commodityOrder.Id, orderRefund.Id, orRefundMoney));
                    decimal coupon_price = 0;//抵用券使用金额
                    var user_yjcoupon = YJBSV.GetUserYJCouponByOrderId(commodityOrder.Id);
                    if (user_yjcoupon != null && user_yjcoupon.Data != null)
                    {
                        foreach (var item in user_yjcoupon.Data)
                        {
                            coupon_price += item.UseAmount ?? 0;
                        }
                    }
                    decimal yjbprice = 0;
                    var yjbresult = YJBSV.GetOrderYJBInfo(commodityOrder.EsAppId.Value, commodityOrder.Id);
                    if (yjbresult.Data != null)
                    {
                        yjbprice = yjbresult.Data.InsteadCashAmount;
                    }
                    decimal cashmoney = 0;
                    if (commodityOrder.State != 1 && commodityOrder.State != 8)
                    {//非待发货状态不退运费
                        cashmoney = commodityOrder.RealPrice.Value - commodityOrder.Freight;
                    }
                    else
                    {
                        cashmoney = commodityOrder.RealPrice.Value;
                    }
                    //orRefundMoney = orRefundMoney -  coupon_price - yjbprice;//退金币时候去掉抵用券金额
                    if (orderRefund.OrderRefundMoneyAndCoupun == null)
                    {//老的退款数据
                        orRefundMoney = orderRefund.RefundMoney > cashmoney ? cashmoney : orderRefund.RefundMoney;
                    }
                    else
                    {
                        orRefundMoney = (orderRefund.OrderRefundMoneyAndCoupun ?? 0) > cashmoney ? cashmoney : (orderRefund.OrderRefundMoneyAndCoupun ?? 0);
                    }
                    if (orRefundMoney > 0)
                    {
                        Jinher.AMP.App.Deploy.CustomDTO.AppIdOwnerIdTypeDTO applicationDTO = APPSV.Instance.GetAppOwnerInfo(commodityOrder.AppId);
                        if (applicationDTO == null || applicationDTO.OwnerId == Guid.Empty)
                            return new ResultDTO { ResultCode = 1, Message = "Error" };

                        var cancelPayDto = OrderSV.BuildCancelPayDTO(commodityOrder, orRefundMoney, contextSession, applicationDTO);

                        //退款
                        cancelPayResult = Jinher.AMP.BTP.TPS.FSPSV.CancelPay(cancelPayDto, tradeType);
                        LogHelper.Info("UpdateOrderStateTo7，FSPSV.CancelPay，输入：" + JsonHelper.JsonSerializer(cancelPayDto) + "，输出：" + JsonHelper.JsonSerializer(cancelPayResult));
                        if (cancelPayResult == null)
                        {
                            return new ResultDTO { ResultCode = 1, Message = "退款失败" };
                        }
                        if (cancelPayResult.Code != 0)
                        {
                            return new ResultDTO { ResultCode = 1, Message = cancelPayResult == null ? "error" : cancelPayResult.Message };
                        }
                        List<int> stwog = new PaySourceBP().GetSecTransWithoutGoldPaymentExt();
                        if (tradeType == TradeTypeEnum.Direct)
                        {
                            if (cancelPayResult.Message == "success")
                            {
                                commodityOrder.State = 12;
                                orderRefund.State = 12;
                                if (ucopDto.RejectFreightMoney > 0) // 新添加拒退运费
                                {
                                    orderRefund.RejectFreightMoney = ucopDto.RejectFreightMoney;
                                }

                            }
                            else
                            {
                                return new ResultDTO { ResultCode = 1, Message = cancelPayResult.Message };
                            }
                        }
                        else
                        {
                            if (cancelPayResult.Message == "success")
                            {
                                commodityOrder.State = 7;
                                orderRefund.State = 1;
                                if (ucopDto.RejectFreightMoney > 0) // 新添加拒退运费
                                {
                                    orderRefund.RejectFreightMoney = ucopDto.RejectFreightMoney;
                                }
                            }
                            else
                            {
                                commodityOrder.State = 12;
                                orderRefund.State = 12;
                            }
                        }
                        orderRefund.ModifiedOn = DateTime.Now;
                    }
                    else
                    {
                        commodityOrder.State = 7;
                        orderRefund.State = 1;
                        orderRefund.ModifiedOn = DateTime.Now;
                    }
                }
                else
                {
                    commodityOrder.State = 7;
                    orderRefund.State = 1;
                    orderRefund.ModifiedOn = DateTime.Now;
                }

                #endregion
            }
            else
            {
                //金和处理退款中，只需要改状态
                commodityOrder.State = 7;
            }
            #region 订单
            commodityOrder.ConfirmTime = now;
            commodityOrder.ModifiedOn = now;
            #endregion

            #region 订单日志
            Journal journal = Journal.CreateJournal(ucopDto, commodityOrder, oldState, "商品已退款");
            contextSession.SaveObject(journal);
            #endregion

            // 回退积分
            SignSV.CommodityOrderRefundScore(ContextFactory.CurrentThreadContext, commodityOrder, orderRefund);

            if (orderRefund.RefundYJCardMoney != null && orderRefund.RefundYJCardMoney.HasValue && orderRefund.RefundYJCardMoney > 0)
            {
                //给易捷卡退款
                Jinher.AMP.BTP.TPS.Helper.YJBHelper.RetreatYjc(commodityOrder.UserId, orderRefund.RefundYJCardMoney == null ? 0 : Convert.ToDecimal(orderRefund.RefundYJCardMoney), ucopDto.orderId, ucopDto.orderItemId);
            }


            #region 回退易捷币和易捷抵用券
            decimal couponprice = 0;//抵用券面额
            decimal couponmoney = 0;//抵用券使用总金额
            //decimal totalcouprice = 0;//保存抵用券之和(当2个或者2个以上的抵用券相加的时候)
            bool isexistsplit = false;//是否有拆单，如果有的话，整单退款的时候，退的是抵用券的使用金额
            var issplit = MainOrder.ObjectSet().Where(x => x.SubOrderId == commodityOrder.Id).FirstOrDefault();
            if (issplit != null)
            {
                isexistsplit = true;
            }
            var useryjcoupon = YJBSV.GetUserYJCouponByOrderId(commodityOrder.Id);
            if (useryjcoupon != null && useryjcoupon.Data != null && useryjcoupon.Data.Count > 0)
            {
                var refundmoney = (orderRefund.OrderRefundMoneyAndCoupun ?? 0);
                useryjcoupon.Data = useryjcoupon.Data.OrderBy(x => x.Price).ToList();
                foreach (var item in useryjcoupon.Data)
                {
                    couponmoney += item.UseAmount ?? 0;
                }
                if (refundmoney == couponmoney + orRefundMoney)
                {//全额退
                    for (int i = 0; i < useryjcoupon.Data.Count; i++)
                    {
                        if (useryjcoupon.Data[i] != null)
                        {
                            if (isexistsplit)
                                couponprice = useryjcoupon.Data[i].UsePrice;
                            else
                                couponprice = useryjcoupon.Data[i].Price;
                            if (i == 0)
                            {

                            }
                            else
                            {//易捷币不能循环退
                                orderRefund.RefundYJBMoney = 0;
                            }
                            decimal coupon = couponprice;
                            Jinher.AMP.BTP.TPS.Helper.YJBHelper.OrderRefund(ContextFactory.CurrentThreadContext, commodityOrder, orderRefund, coupon, useryjcoupon.Data[i].Id);
                        }
                    }
                }
                else
                {//部分退款
                    for (int i = 0; i < useryjcoupon.Data.Count; i++)
                    {
                        if (useryjcoupon.Data[i] != null)
                        {
                            if (isexistsplit)
                                couponprice = useryjcoupon.Data[i].UsePrice;
                            else
                                couponprice = useryjcoupon.Data[i].Price;
                            //totalcouprice += useryjcoupon.Data[i].Price;
                            if (i == 0)
                            {

                            }
                            else
                            {//易捷币不能循环退
                                orderRefund.RefundYJBMoney = 0;
                            }
                            decimal coupon = 0;
                            if (refundmoney - orRefundMoney > 0)
                            {
                                if (refundmoney - orRefundMoney - couponprice >= 0)
                                {//退款金额大于等于（实际支付金额+抵用券金额），直接返回抵用券面值
                                 //if (refundmoney - commodityOrder.RealPrice - couponprice - orderRefund.RefundYJBMoney < 0)
                                 //{//返还部分易捷币
                                 //    orderRefund.RefundYJBMoney = orderRefund.RefundYJBMoney - (refundmoney + commodityOrder.RealPrice.Value + couponprice);
                                 //}
                                    coupon = couponprice;
                                }
                                else
                                {//否则，表示不是全额退款，返还部分抵用券，易捷币返还0
                                    coupon = refundmoney - orRefundMoney;
                                    orderRefund.RefundYJBMoney = 0;
                                }
                            }
                            Jinher.AMP.BTP.TPS.Helper.YJBHelper.OrderRefund(ContextFactory.CurrentThreadContext, commodityOrder, orderRefund, coupon, useryjcoupon.Data[i].Id);
                            refundmoney -= coupon;
                            //pretotalcouprice = totalcouprice;
                        }
                    }
                }
            }
            else
            {
                Jinher.AMP.BTP.TPS.Helper.YJBHelper.OrderRefund(ContextFactory.CurrentThreadContext, commodityOrder, orderRefund, 0, Guid.Empty);
            }
            #endregion

            // 更新结算项
            Jinher.AMP.BTP.TPS.Helper.SettleAccountHelper.OrderRefund(contextSession, commodityOrder, orderRefund);

            contextSession.SaveChanges();

            //更新退货物流信息
            if (orderRefund.RefundType == 1)
            {
                RefundExpressTraceBP bp = new RefundExpressTraceBP();
                Jinher.AMP.BTP.Deploy.RefundExpressTraceDTO retd = new Deploy.RefundExpressTraceDTO();
                retd.OrderId = ucopDto.orderId;
                bp.UpdateRefundExpressTraceExt(retd, commodityOrder.AppId);
            }

            #region 异步发送消息
            if (commodityOrder.State == 7)
            {

                System.Threading.ThreadPool.QueueUserWorkItem(
                    a =>
                    {

                        AddMessage addmassage = new AddMessage();
                        string type = "order";
                        string tipayment = new PaySourceBP().GetPaymentName(commodityOrder.Payment);
                        var messages = string.Format("您的订单{0}已完成退款，退款金额{1}元，请到{2}账号中确认！", commodityOrder.Code, orRefundMoney, tipayment);
                        Guid esAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
                        addmassage.AddMessages(commodityOrder.Id.ToString(), commodityOrder.UserId.ToString(), esAppId, commodityOrder.Code, commodityOrder.State, messages, type);
                    });
            }
            #endregion
            return result;
        }

        /// <summary>
        /// 订单单品退款
        /// </summary>
        /// <param name="ucopDto"></param>
        /// <param name="commodityOrder"></param>
        /// <returns></returns>
        private ResultDTO UpdateOrderItemStateTo7(UpdateCommodityOrderParamDTO ucopDto, CommodityOrder commodityOrder)
        {
            LogHelper.Debug(String.Format("进入单商品退款接口UpdateOrderItemStateTo7，参数ucopDto：{0}，commodityOrder：{1}", JsonHelper.JsSerializer(ucopDto), JsonHelper.JsSerializer(commodityOrder)));

            ResultDTO result = new ResultDTO();

            List<Commodity> needRefreshCacheCommoditys = new List<Commodity>();
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            DateTime now = DateTime.Now;
            int oldState = ucopDto.targetState;

            var orderItem = OrderItem.FindByID(ucopDto.orderItemId);
            if (orderItem.State != 1 && orderItem.State != 3)
            {
                if (orderItem.State == 0)
                {
                    return new ResultDTO { ResultCode = 1, Message = "客户已撤销退款申请" };
                }
                return new ResultDTO { ResultCode = 1, Message = "当前订单项状态错误,请重试" };
            }

            var orderRefund = OrderRefund.ObjectSet().Where(t => t.OrderItemId == ucopDto.orderItemId).OrderByDescending(t => t.SubTime).FirstOrDefault();
            //decimal orRefundMoney = orderRefund.RefundMoney + orderRefund.RefundScoreMoney - orderItem.ScorePrice;
            //if (orderItem.YjbPrice != null)
            //{
            //    orRefundMoney = orRefundMoney - (decimal)orderItem.YjbPrice;
            //}
            decimal coupon_price = 0;
            var user_yjcoupon = YJBSV.GetUserYJCouponItemByOrderId(orderItem.Id);
            if (user_yjcoupon.Data != null)
            {
                foreach (var item in user_yjcoupon.Data)
                {
                    coupon_price += item.UseAmount;
                }
            }
            decimal yjbprice = 0;
            var yjbresult = YJBSV.GetOrderItemYJBInfo(commodityOrder.EsAppId.Value, commodityOrder.Id);
            if (yjbresult.Data != null)
            {
                yjbprice = yjbresult.Data.Items != null ? yjbresult.Data.Items[0].InsteadCashAmount : 0;
            }


            decimal yjcPrice = orderItem.YJCardPrice ?? 0;

            //猜测refund_money为退款总金额。
            decimal refund_money = 0;
            if (orderRefund.OrderRefundMoneyAndCoupun == null)
            {//老的退款数据
                refund_money = orderRefund.RefundMoney + (orderItem.FreightPrice ?? 0);
            }
            else
            {
                //refund_money = (orderRefund.OrderRefundMoneyAndCoupun ?? 0) + (orderItem.FreightPrice ?? 0);
                refund_money = (orderRefund.OrderRefundMoneyAndCoupun ?? 0) + (orderItem.FreightPrice ?? 0) + yjcPrice;
            }
            var CurrPic1 = orderItem.RealPrice * orderItem.Number + (orderItem.FreightPrice ?? 0);
            if (CurrPic1 == 0)
            {
                CurrPic1 = (orderItem.DiscountPrice.Value * orderItem.Number);
            }
            decimal cashmoney = 0;
            var cashmoneyx = CurrPic1 - (orderItem.CouponPrice ?? 0) - (orderItem.ChangeRealPrice ?? 0) - orderItem.Duty - coupon_price - yjbprice - yjcPrice;
            cashmoney = cashmoneyx == null || !cashmoneyx.HasValue ? 0 : cashmoneyx.Value;

            if (commodityOrder.State != 1 && commodityOrder.State != 8)
            {//待发货和待发货退款中的时候，退运费，其他状态下不退运费
                cashmoney -= (orderItem.FreightPrice ?? 0);
                refund_money -= (orderItem.FreightPrice ?? 0);
            }

            decimal orRefundMoney = refund_money > cashmoney ? cashmoney : refund_money;

            //orRefundMoney = orRefundMoney - orderItem.ScorePrice;

            #region 加库存
            //只有未发货的订单单商品退款才会加库存
            if (oldState == 21)
            {
                Commodity com = Commodity.ObjectSet().FirstOrDefault(n => n.Id == orderItem.CommodityId);
                if (com != null)
                {
                    com.Stock += orderItem.Number;
                    com.ModifiedOn = now;
                    com.EntityState = System.Data.EntityState.Modified;
                    if (orderItem.CommodityStockId.HasValue && orderItem.CommodityStockId.Value != Guid.Empty && orderItem.CommodityStockId != com.Id)
                    {
                        var cs = CommodityStock.FindByID(orderItem.CommodityStockId.Value);
                        cs.Stock += orderItem.Number;
                        cs.ModifiedOn = now;
                        cs.EntityState = System.Data.EntityState.Modified;
                    }
                    needRefreshCacheCommoditys.Add(com);
                }
            }

            #endregion

            #region 退款
            var tradeType = PaySource.GetTradeType(commodityOrder.Payment);
            if (orderItem.RealPrice > 0 && (tradeType == TradeTypeEnum.SecTrans || tradeType == TradeTypeEnum.Direct && CustomConfig.IsSystemDirectRefund))
            {
                ReturnInfoDTO cancelPayResult = new ReturnInfoDTO();
                LogHelper.Info("UpdateOrderItemStateTo7，开始退款：" + orRefundMoney);

                if (orRefundMoney > 0)
                {
                    Jinher.AMP.App.Deploy.CustomDTO.AppIdOwnerIdTypeDTO applicationDTO = APPSV.Instance.GetAppOwnerInfo(commodityOrder.AppId);
                    if (applicationDTO == null || applicationDTO.OwnerId == Guid.Empty)
                        return new ResultDTO { ResultCode = 1, Message = "Error" };

                    var cancelPayDto = OrderSV.BuildCancelPayDTOForOrderItem(commodityOrder, orRefundMoney, contextSession, applicationDTO, orderItem);

                    //退款
                    cancelPayResult = Jinher.AMP.BTP.TPS.FSPSV.CancelPay(cancelPayDto, tradeType);
                    LogHelper.Info("UpdateOrderItemStateTo7，FSPSV.CancelPay，输入：" + JsonHelper.JsonSerializer(cancelPayDto) + "，输出：" + JsonHelper.JsonSerializer(cancelPayResult));
                    if (cancelPayResult == null)
                    {
                        return new ResultDTO { ResultCode = 1, Message = "退款失败" };
                    }
                    if (cancelPayResult.Code != 0)
                    {
                        return new ResultDTO { ResultCode = 1, Message = cancelPayResult == null ? "error" : cancelPayResult.Message };
                    }
                    if (tradeType == TradeTypeEnum.Direct)
                    {
                        if (cancelPayResult.Message == "success")
                        {
                            orderItem.State = 2;
                            orderRefund.State = 1;
                        }
                        else
                        {
                            return new ResultDTO { ResultCode = 1, Message = cancelPayResult.Message };
                        }
                    }
                    else
                    {
                        if (cancelPayResult.Message == "success")
                        {
                            orderItem.State = 2;
                            orderRefund.State = 1;
                        }
                        else
                        {
                            orderRefund.State = 12;
                        }
                    }
                    orderRefund.ModifiedOn = DateTime.Now;
                    orderRefund.EntityState = EntityState.Modified;
                }
                else
                {
                    //LogHelper.Error(string.Format("CancelTheOrderExt退款失败，退款金额应该大于0。outTradeId：{0}，sign:{1}，messages：{2},Payment:{3}", commodityOrder.Id, CustomConfig.PaySing, cancelPayResult.Message, commodityOrder.Payment));
                    //return new ResultDTO { ResultCode = 1, Message = "退款失败" };
                    orderItem.State = 2;
                    orderRefund.State = 1;
                }
            }
            else
            {
                orderItem.State = 2;
            }
            #endregion

            //退款总金额
            var totRefund = cashmoney + (orderRefund.RefundYJCardMoney.HasValue ? orderRefund.RefundYJCardMoney.Value : 0) + orderRefund.RefundScoreMoney + orderRefund.RefundYJBMoney;
            LogHelper.Info($"退款运费refundFreightPrice:totRefund{totRefund} = refund_money{refund_money} + orderRefund.RefundScoreMoney{orderRefund.RefundScoreMoney} + orderRefund.RefundYJBMoney{orderRefund.RefundYJBMoney}");

            //之前退款积分为订单的 现在修改为订单商品项的
            orderRefund.RefundScoreMoney = orderItem.ScorePrice;
            // 回退积分
            SignSV.CommodityOrderRefundScore(ContextFactory.CurrentThreadContext, commodityOrder, orderRefund);

            //退回易捷卡。
            if (orderRefund.RefundYJCardMoney != null && orderRefund.RefundYJCardMoney.HasValue && orderRefund.RefundYJCardMoney > 0)
            {
                Jinher.AMP.BTP.TPS.Helper.YJBHelper.RetreatYjc(commodityOrder.UserId, orderRefund.RefundYJCardMoney.Value, ucopDto.orderId, ucopDto.orderItemId);
            }
            //之前退款易捷币为订单的 现在修改为订单商品项的
            if (orderItem.YjbPrice != null)
            {
                orderRefund.RefundYJBMoney = (decimal)orderItem.YjbPrice;
            }

            orderRefund.RefundYJBMoney = yjbprice;//单品易捷币退款金额为0，在这里处理一下
            if (orderRefund.RefundMoney - cashmoney - coupon_price - orderRefund.RefundYJBMoney <= 0)
            {//返还部分易捷币
                orderRefund.RefundYJBMoney = (orderRefund.OrderRefundMoneyAndCoupun ?? 0) - cashmoney - coupon_price;
            }

            decimal couponprice = 0;
            //decimal totalcouprice = 0;//保存抵用券之和(当2个或者2个以上的抵用券相加的时候)
            //decimal pretotalcouprice = 0;//保存上一个抵用券相加的价格(当2个或者2个以上的抵用券相加的时候)
            var refundmoney = (orderRefund.OrderRefundMoneyAndCoupun ?? 0);
            var useryjcoupon = YJBSV.GetUserYJCouponItemByOrderId(orderItem.Id);
            if (useryjcoupon.Data != null)
            {
                useryjcoupon.Data = useryjcoupon.Data.OrderBy(x => x.UseAmount).ToList();
                for (int i = 0; i < useryjcoupon.Data.Count; i++)
                {
                    if (useryjcoupon.Data[i] != null)
                    {
                        couponprice = useryjcoupon.Data[i].UseAmount;
                        decimal coupon = 0;
                        //totalcouprice += useryjcoupon.Data[i].UseAmount;
                        if (i == 0)
                        {
                            //pretotalcouprice = couponprice;
                        }
                        else
                        {//易捷币不能循环退
                            orderRefund.RefundYJBMoney = 0;
                        }
                        if (refundmoney - orRefundMoney > 0)
                        {
                            if (refundmoney - orRefundMoney - couponprice >= 0)
                            {//退款金额大于等于（实际支付金额+抵用券金额），直接返回抵用券面值
                                //if (orefundmoney - cashmoney - couponprice - orderRefund.RefundYJBMoney < 0)
                                //{//返还部分易捷币
                                //    orderRefund.RefundYJBMoney = orderRefund.RefundYJBMoney - (orefundmoney + cashmoney + couponprice);
                                //}
                                coupon = couponprice;
                            }
                            else
                            {//否则，表示不是全额退款，返还部分抵用券，易捷币返还0
                                coupon = refundmoney - orRefundMoney;
                                orderRefund.RefundYJBMoney = 0;
                            }
                        }

                        // 回退易捷币和抵用券 单品退款
                        Jinher.AMP.BTP.TPS.Helper.YJBHelper.OrderItemRefund(ContextFactory.CurrentThreadContext, commodityOrder, orderRefund, orderItem.CommodityId, orderItem.Id, coupon, useryjcoupon.Data[i].UserYJCouponId);
                        refundmoney -= coupon;
                        //pretotalcouprice = totalcouprice;
                    }
                }
            }


            // 更新结算项
            Jinher.AMP.BTP.TPS.Helper.SettleAccountHelper.OrderRefund(contextSession, commodityOrder, orderRefund);

            //记录商品退款详情到退款表
            var CurrPic = (orderItem.RealPrice * orderItem.Number);
            if (CurrPic == 0)
            {
                CurrPic = (orderItem.DiscountPrice * orderItem.Number);
            }
            //商品单项退款运费
            decimal refundFreightPrice = 0;
            if (commodityOrder.State == 1 || commodityOrder.State == 8 || commodityOrder.State == 13 || commodityOrder.State == 14)
            {
                LogHelper.Info($"退款运费refundFreightPrice:totRefund{totRefund} - (decimal)CurrPic{(decimal)CurrPic} - orderItem.Duty{orderItem.Duty}={refundFreightPrice}");
                refundFreightPrice = totRefund - (decimal)CurrPic - orderItem.Duty;
                if (orderItem.CouponPrice != null && orderItem.CouponPrice > 0)
                {
                    LogHelper.Info($"退款运费refundFreightPrice:refundFreightPrice{refundFreightPrice} += (decimal)orderItem.CouponPrice{(decimal)orderItem.CouponPrice}");
                    refundFreightPrice += (decimal)orderItem.CouponPrice;
                }
                if (orderItem.ChangeRealPrice != null && orderItem.ChangeRealPrice > 0)
                {
                    LogHelper.Info($"退款运费refundFreightPrice:refundFreightPrice{refundFreightPrice} += (decimal)orderItem.ChangeRealPrice{(decimal)orderItem.ChangeRealPrice}");
                    refundFreightPrice += (decimal)orderItem.ChangeRealPrice;
                }
            }
            else if (commodityOrder.State == 2 || commodityOrder.State == 9)
            {
                LogHelper.Info($"退款运费refundFreightPrice:refundFreightPrice{refundFreightPrice} = totRefund{totRefund} - (decimal)CurrPic{(decimal)CurrPic} + orderItem.Duty{orderItem.Duty}");
                refundFreightPrice = totRefund - (decimal)CurrPic + orderItem.Duty;
                if (orderItem.CouponPrice != null && orderItem.CouponPrice > 0)
                {
                    LogHelper.Info($"退款运费refundFreightPrice:refundFreightPrice{refundFreightPrice} += (decimal)orderItem.CouponPrice{(decimal)orderItem.CouponPrice}");
                    refundFreightPrice += (decimal)orderItem.CouponPrice;
                }
                if (orderItem.ChangeRealPrice != null && orderItem.ChangeRealPrice > 0)
                {
                    LogHelper.Info($"退款运费refundFreightPrice:refundFreightPrice{refundFreightPrice} += (decimal)orderItem.ChangeRealPrice{(decimal)orderItem.ChangeRealPrice}");
                    refundFreightPrice += (decimal)orderItem.ChangeRealPrice;
                }
            }
            // 易捷抵用劵
            //if (orderItem.YJCouponPrice.HasValue && orderItem.YJCouponPrice > 0)
            //{
            //    refundFreightPrice += orderItem.YJCouponPrice.Value;
            //}

            orderRefund.RefundFreightPrice = refundFreightPrice;

            //orderRefund.RefundMoney = totRefund;
            orderRefund.RefundMoney = refundmoney;


            //订单项全部退款 修改订单状态
            var orderItems = OrderItem.ObjectSet().Where(t => t.CommodityOrderId == commodityOrder.Id && t.Id != ucopDto.orderItemId && t.State != 2);
            if (!orderItems.Any() && orderItem.State == 2)
            {
                commodityOrder.State = 7;
                commodityOrder.ModifiedOn = DateTime.Now;
                contextSession.SaveObject(commodityOrder);
            }

            Journal journal = Journal.CreateJournal(ucopDto, commodityOrder, oldState, "商品已退款");
            contextSession.SaveObject(journal);

            contextSession.SaveChanges();

            //更新退货物流信息
            if (orderRefund.RefundType == 1)
            {
                RefundExpressTraceBP bp = new RefundExpressTraceBP();
                Jinher.AMP.BTP.Deploy.RefundExpressTraceDTO retd = new Deploy.RefundExpressTraceDTO();
                retd.OrderId = ucopDto.orderId;
                retd.OrderItemId = ucopDto.orderItemId;
                bp.UpdateRefundExpressTraceExt(retd, commodityOrder.AppId);
            }
            return result;
        }

        private ResultDTO UpdateCYOrderStateTo7(UpdateCommodityOrderParamDTO ucopDto, CommodityOrder commodityOrder)
        {
            ResultDTO result = new ResultDTO();

            List<Commodity> needRefreshCacheCommoditys = new List<Commodity>();
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            DateTime now = DateTime.Now;
            int oldState = commodityOrder.State;
            int newState = ucopDto.targetState;

            var orderRefund = getOrderRefund(commodityOrder.Id);

            if (orderRefund == null)
            {
                orderRefund = new OrderRefund();
                orderRefund.Id = Guid.NewGuid();
                orderRefund.DataType = "0";
                orderRefund.EntityState = EntityState.Added;
                orderRefund.IsFullRefund = ucopDto.CYRefundType == 0 ? true : false;
                orderRefund.ModifiedId = ucopDto.userId;
                orderRefund.ModifiedOn = DateTime.Now;
                orderRefund.OrderId = ucopDto.orderId;
                orderRefund.Receiver = ucopDto.Receiver;
                orderRefund.ReceiverAccount = ucopDto.ReceiverAccount;
                orderRefund.RefundDesc = ucopDto.remessage;

                switch (ucopDto.CYRefundType)
                {
                    case 0:
                        orderRefund.RefundMoney = commodityOrder.RealPrice.HasValue ? commodityOrder.RealPrice.Value : 0;
                        break;
                    case 1:
                        var commoditys = (from c in Commodity.ObjectSet()
                                          where ucopDto.OrderItemIds.Contains(c.Id)
                                          select c).ToList();
                        foreach (var item in commoditys)
                        {
                            orderRefund.RefundMoney += item.Price;
                        }
                        break;
                }

                orderRefund.RefundReason = ucopDto.remessage;
                orderRefund.RefundType = 0;
                orderRefund.State = ucopDto.targetState;
                orderRefund.SubId = ucopDto.userId;
                orderRefund.SubTime = DateTime.Now;
            }

            decimal orRefundMoney = orderRefund.RefundMoney;

            if (oldState != 12)
            {
                #region 退款

                var tradeType = PaySource.GetTradeType(commodityOrder.Payment);
                if (commodityOrder.RealPrice > 0 && (tradeType == TradeTypeEnum.SecTrans || tradeType == TradeTypeEnum.Direct && CustomConfig.IsSystemDirectRefund))
                {
                    ReturnInfoDTO cancelPayResult = new ReturnInfoDTO();
                    orRefundMoney = orderRefund.RefundMoney;
                    if (orRefundMoney > 0)
                    {
                        Jinher.AMP.App.Deploy.CustomDTO.AppIdOwnerIdTypeDTO applicationDTO = APPSV.Instance.GetAppOwnerInfo(commodityOrder.AppId);
                        if (applicationDTO == null || applicationDTO.OwnerId == Guid.Empty)
                            return new ResultDTO { ResultCode = 1, Message = "Error" };

                        var cancelPayDto = OrderSV.BuildCancelPayDTO(commodityOrder, orRefundMoney, contextSession, applicationDTO);

                        //退款
                        cancelPayResult = Jinher.AMP.BTP.TPS.FSPSV.CancelPay(cancelPayDto, tradeType);
                        if (cancelPayResult == null)
                        {
                            return new ResultDTO { ResultCode = 1, Message = "退款失败" };
                        }
                        if (cancelPayResult.Code != 0)
                        {
                            return new ResultDTO { ResultCode = 1, Message = cancelPayResult == null ? "error" : cancelPayResult.Message };
                        }
                        List<int> stwog = new PaySourceBP().GetSecTransWithoutGoldPaymentExt();
                        if (tradeType == TradeTypeEnum.Direct)
                        {
                            if (cancelPayResult.Message == "success")
                            {
                                commodityOrder.State = 12;
                                orderRefund.State = 12;
                            }
                            else
                            {
                                return new ResultDTO { ResultCode = 1, Message = cancelPayResult.Message };
                            }
                        }
                        else
                        {
                            if (cancelPayResult.Message == "success")
                            {
                                commodityOrder.State = 7;
                                orderRefund.State = 1;
                            }
                            else
                            {
                                commodityOrder.State = 12;
                                orderRefund.State = 12;
                            }
                        }
                        orderRefund.ModifiedOn = DateTime.Now;
                        orderRefund.EntityState = EntityState.Modified;

                        contextSession.SaveObject(orderRefund);
                        contextSession.SaveObject(commodityOrder);

                        result.isSuccess = contextSession.SaveChanges() > 0;
                        result.Message = result.isSuccess ? "退款成功" : "退款失败";
                        result.ResultCode = result.isSuccess ? 0 : -1;
                    }
                    else
                    {
                        LogHelper.Error(string.Format("CancelTheOrderExt退款失败，退款金额应该大于0。outTradeId：{0}，sign:{1}，messages：{2},Payment:{3}", commodityOrder.Id, CustomConfig.PaySing, cancelPayResult.Message, commodityOrder.Payment));
                        return new ResultDTO { ResultCode = 1, Message = "退款失败" };
                    }
                }
                else
                {
                    commodityOrder.State = 7;
                }

                #endregion
            }

            return result;
        }

        /// <summary>
        /// 售中直接到账退款
        /// </summary>
        /// <param name="orderRefundDto">退款信息</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DirectPayRefundExt(Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundDTO orderRefundDto)
        {
            if (orderRefundDto == null || orderRefundDto.OrderId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数不能为空" };
            }
            if (!OrderSV.LockOrder(orderRefundDto.OrderId))
            {
                return new ResultDTO { ResultCode = 110, Message = "操作失败" };
            }
            try
            {
                List<Commodity> needRefreshCacheCommoditys = new List<Commodity>();
                List<TodayPromotion> needRefreshCacheTodayPromotions = new List<TodayPromotion>();
                DateTime now = DateTime.Now;
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                List<int> enableState = new List<int>() { 8, 9, 14 };

                var commityList = (from c in CommodityOrder.ObjectSet()
                                   join r in OrderRefund.ObjectSet()
                                    on c.Id equals r.OrderId
                                   where c.Id == orderRefundDto.OrderId && enableState.Contains(c.State) && r.State == 0
                                   select new
                                   {
                                       CommodityOrder = c,
                                       OrderRefund = r
                                   }).FirstOrDefault();
                if (commityList == null || commityList.CommodityOrder == null || commityList.OrderRefund == null)
                {
                    return new ResultDTO { ResultCode = 1, Message = "找不到相应的订单" };
                }

                var commodityOrder = commityList.CommodityOrder;
                var orderRefund = commityList.OrderRefund;
                List<int> directArrivalPayments = new PaySourceBP().GetDirectArrivalPaymentExt();
                if (!directArrivalPayments.Contains(commodityOrder.Payment))
                {
                    return new ResultDTO { ResultCode = 1, Message = "订单支付方式不是直接到账" };
                }
                var orderitemlist = OrderItem.ObjectSet().Where(n => n.CommodityOrderId == commodityOrder.Id).ToList();
                UserLimited.ObjectSet().Context.ExecuteStoreCommand("delete from UserLimited where CommodityOrderId='" + commodityOrder.Id + "'");

                List<HotCommodity> hotCommodities = new List<HotCommodity>();
                if (orderitemlist.Any())
                {
                    var ids = orderitemlist.Select(c => c.CommodityId).ToList();
                    hotCommodities =
                        HotCommodity.ObjectSet().Where(c => ids.Contains(c.CommodityId)).Distinct().ToList();

                }

                foreach (OrderItem items in orderitemlist)
                {
                    Guid comId = items.CommodityId;
                    Commodity com = Commodity.ObjectSet().Where(n => n.Id == comId).First();
                    com.EntityState = System.Data.EntityState.Modified;
                    com.Stock += items.Number;
                    contextSession.SaveObject(com);
                    needRefreshCacheCommoditys.Add(com);
                    HotCommodity hotCommodity = hotCommodities.FirstOrDefault(c => c.CommodityId == comId);
                    if (hotCommodity != null)
                    {
                        hotCommodity.EntityState = EntityState.Modified;
                        hotCommodity.Stock = com.Stock;
                        contextSession.SaveObject(hotCommodity);
                    }

                    if (items.Intensity != 10 || items.DiscountPrice != -1)
                    {
                        TodayPromotion to = TodayPromotion.GetCurrentPromotion(comId);
                        if (to != null)
                        {
                            to.SurplusLimitBuyTotal = to.SurplusLimitBuyTotal - items.Number;
                            to.EntityState = System.Data.EntityState.Modified;
                            contextSession.SaveObject(to);
                            needRefreshCacheTodayPromotions.Add(to);
                            PromotionItems pti = PromotionItems.ObjectSet().Where(n => n.PromotionId == to.PromotionId && n.CommodityId == comId).FirstOrDefault();
                            pti.SurplusLimitBuyTotal = pti.SurplusLimitBuyTotal - items.Number;
                            pti.EntityState = System.Data.EntityState.Modified;
                            contextSession.SaveObject(pti);
                        }
                    }

                }

                commodityOrder.State = 7;
                commodityOrder.ConfirmTime = DateTime.Now;
                commodityOrder.ModifiedOn = DateTime.Now;
                commodityOrder.EntityState = System.Data.EntityState.Modified;

                orderRefund.State = 1;
                orderRefund.ReceiverAccount = orderRefundDto.ReceiverAccount;
                orderRefund.Receiver = orderRefundDto.Receiver;
                orderRefund.RefundMoney = orderRefundDto.RefundMoney;
                orderRefund.SalerRemark = orderRefundDto.SalerRemark;
                orderRefund.ModifiedOn = DateTime.Now;
                orderRefund.EntityState = System.Data.EntityState.Modified;

                int reslult = contextSession.SaveChanges();
                if (reslult > 0)
                {

                    if (needRefreshCacheCommoditys.Any())
                    {
                        needRefreshCacheCommoditys.ForEach(c => c.RefreshCache(EntityState.Modified));
                    }
                    if (needRefreshCacheTodayPromotions.Any())
                    {
                        needRefreshCacheTodayPromotions.ForEach(c => c.RefreshCache(EntityState.Modified));
                    }

                    AddMessage addmassage = new AddMessage();
                    string type = "order";
                    //发送消息，异步执行
                    System.Threading.ThreadPool.QueueUserWorkItem(
                        a =>
                        {
                            //向客户端推送交易失败消息
                            string messages = string.Format("您的订单{0}已完成退款，退款金额{1}元，请确认！", commodityOrder.Code, orderRefundDto.RefundMoney);
                            Guid EsAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
                            addmassage.AddMessages(orderRefundDto.OrderId.ToString(), commodityOrder.UserId.ToString(), EsAppId,
                                commodityOrder.Code, commodityOrder.State, messages, type);
                            ////正品会发送消息
                            //if (new ZPHSV().CheckIsAppInZPH(commodityOrder.AppId))
                            //{
                            //    addmassage.AddMessages(commodityorderId.ToString(), commodityOrder.UserId.ToString(), CustomConfig.ZPHAppId,
                            //       commodityOrder.Code, commodityOrder.State, messages, type);
                            //}
                        });
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                }
                else
                {
                    return new ResultDTO { ResultCode = 3, Message = "退款失败" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("直接到账退款服务异常。orderRefundDto：{0}", JsonHelper.JsonSerializer(orderRefundDto)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(orderRefundDto.OrderId);
            }
        }
        /// <summary>
        /// 获取新增待处理订单数量
        /// </summary>
        /// <param name="appId">店铺id</param>
        /// <param name="lastPayTime">最后支付时间</param>
        /// <returns></returns>
        public int GetNewCyUntreatedCountExt(Guid appId, DateTime lastPayTime)
        {
            return CommodityOrder.ObjectSet().Count(c => c.AppId == appId && c.State == 18 && c.PaymentTime > lastPayTime);
        }

        /// <summary>
        /// 分享订单获取相关的的数据
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.Share.ShareOrderDTO GetShareOrderInfoByOrderIdExt(Guid orderId)
        {

            //判断订单是否是拆单
            var mainOrder = MainOrder.ObjectSet().Where(r => r.MainOrderId == orderId).OrderBy(r => r.ModifiedOn).FirstOrDefault();
            if (mainOrder != null)
            {
                orderId = mainOrder.SubOrderId;
            }

            //获取订单商品
            var commodityList = (from o in OrderItem.ObjectSet()
                                 where o.CommodityOrderId == orderId
                                 select o).ToList();

            if (commodityList == null || commodityList.Count == 0) return null;

            var item = commodityList.OrderBy(r => r.ModifiedOn).FirstOrDefault();
            return new Deploy.CustomDTO.Share.ShareOrderDTO()
            {
                OrderId = orderId,
                ShareImgUrl = item.PicturesPath,
                ShareTitle = item.Name
            };
        }

        /// <summary>
        /// 获得订单信息
        /// </summary>
        /// <param name="id">商品订单ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CommodityOrderDTO GetCommodityOrderInfoExt(System.Guid id)
        {
            var entity = CommodityOrder.FindByID(id);
            if (entity != null)
            {
                return entity.ToEntityData();
            }
            return null;
        }



        /// <summary>
        /// 获得主订单信息
        /// </summary>
        /// <param name="id">商品订单ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.MainOrderDTO GetMainOrderInfoExt(Guid suborderId)
        {
            MainOrderDTO model = new MainOrderDTO();
            var mainOrder = MainOrder.ObjectSet().FirstOrDefault(t => t.SubOrderId == suborderId);
            if (mainOrder != null)
            {
                model = CommonUtil.ReadObjectExchange(model, mainOrder);
            }
            return model;
        }




        /// <summary>
        /// 修改订单信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO UpdateCommodityOrderExt(Jinher.AMP.BTP.Deploy.CommodityOrderDTO model)
        {
            ResultDTO dto = null;
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var commodityorder = CommodityOrder.ObjectSet().FirstOrDefault(p => p.Id == model.Id);
                if (commodityorder != null)
                {
                    commodityorder.State = model.State;
                    commodityorder.ShipmentsTime = model.ShipmentsTime;
                    contextSession.SaveChanges();
                    dto = new ResultDTO() { ResultCode = 0, Message = "修改成功", isSuccess = true };
                }
                else
                {
                    dto = new ResultDTO() { ResultCode = 1, Message = "该信息不存在", isSuccess = false };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CommodityOrder信息保存异常。CommodityOrder：{0}", ex.Message), ex);
                dto = new ResultDTO() { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return dto;
        }


        /// <summary>
        /// 根据ExpOrderNo获取订单信息
        /// </summary>
        /// <param name="id">商品订单ID</param>
        /// <returns></returns>
        public CommodityOrderDTO GetCommodityOrderbyExpOrderNoExt(string ExpOrderNo)
        {
            Jinher.AMP.BTP.Deploy.CommodityOrderDTO exEntity = new Jinher.AMP.BTP.Deploy.CommodityOrderDTO();
            var model = CommodityOrder.ObjectSet().FirstOrDefault(p => p.ExpOrderNo == ExpOrderNo);
            if (model != null)
            {
                exEntity = CommonUtil.ReadObjectExchange(exEntity, model);
            }
            return exEntity;

        }

        /// <summary>
        /// 进销存系统对接临时方案-按京东eclp系统标准导出订单
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<ExportOrderForJDDTO> ExportOrderForJDExt(Guid appId, DateTime startTime, DateTime endTime)
        {
            LogHelper.Debug("CommodityOrderBP.ExportOrderForJDExt,appId=" + appId + ",startTime=" + startTime + ",endTime=" + endTime);
            try
            {
                var orderList1 = (from o in CommodityOrder.ObjectSet()
                                  join i in OrderItem.ObjectSet() on o.Id equals i.CommodityOrderId
                                  join m in MallApply.ObjectSet() on new { a = o.EsAppId ?? Guid.Empty, b = o.AppId } equals new { a = m.EsAppId, b = m.AppId } //o.EsAppId equals m.EsAppId
                                  join c in Commodity.ObjectSet() on i.CommodityId equals c.Id
                                  where o.AppId == appId && m.AppId == appId && o.State == 1 && o.PaymentTime.HasValue && o.PaymentTime.Value >= startTime && o.PaymentTime.Value < endTime
                                      && o.EsAppId == YJB.Deploy.CustomDTO.YJBConsts.YJAppId && m.EsAppId == YJB.Deploy.CustomDTO.YJBConsts.YJAppId && new List<int> { 2, 3 }.Contains(m.Type)
                                      && c.JDCode != null && c.JDCode != "" && (!i.CommodityStockId.HasValue || i.CommodityStockId.Value == Guid.Empty || i.CommodityStockId.Value == i.CommodityId)
                                  select new ExportOrderForJDDTO
                                  {
                                      OrderId = o.Id,
                                      OrderCode = o.Code,
                                      ReceiptUserName = o.ReceiptUserName,
                                      ReceiptPhone = o.ReceiptPhone,
                                      ReceiptAddress = o.Province + o.City + o.District + o.Street + o.ReceiptAddress,
                                      Numbers = string.Empty,
                                      Prices = string.Empty,
                                      JdCodes = string.Empty,
                                      ErQiCodes = string.Empty
                                  }).Distinct().ToList();
                LogHelper.Debug("CommodityOrderBP.ExportOrderForJDExt,orderList1.Count:" + orderList1.Count);
                var orderList2 = (from o in CommodityOrder.ObjectSet()
                                  join i in OrderItem.ObjectSet() on o.Id equals i.CommodityOrderId
                                  join m in MallApply.ObjectSet() on new { a = o.EsAppId ?? Guid.Empty, b = o.AppId } equals new { a = m.EsAppId, b = m.AppId } //o.EsAppId equals m.EsAppId
                                  join s in CommodityStock.ObjectSet() on i.CommodityStockId equals s.Id
                                  where o.AppId == appId && m.AppId == appId && o.State == 1 && o.PaymentTime.HasValue && o.PaymentTime.Value >= startTime && o.PaymentTime.Value < endTime
                                      && o.EsAppId == YJB.Deploy.CustomDTO.YJBConsts.YJAppId && m.EsAppId == YJB.Deploy.CustomDTO.YJBConsts.YJAppId && new List<int> { 2, 3 }.Contains(m.Type)
                                      && s.JDCode != null && s.JDCode != "" && i.CommodityStockId.HasValue && i.CommodityStockId.Value != Guid.Empty && i.CommodityStockId.Value != i.CommodityId
                                  select new ExportOrderForJDDTO
                                  {
                                      OrderId = o.Id,
                                      OrderCode = o.Code,
                                      ReceiptUserName = o.ReceiptUserName,
                                      ReceiptPhone = o.ReceiptPhone,
                                      ReceiptAddress = o.Province + o.City + o.District + o.Street + o.ReceiptAddress,
                                      Numbers = string.Empty,
                                      Prices = string.Empty,
                                      JdCodes = string.Empty,
                                      ErQiCodes = string.Empty
                                  }).Distinct().ToList();
                LogHelper.Debug("CommodityOrderBP.ExportOrderForJDExt,orderList2.Count:" + orderList2.Count);
                var dir = new Dictionary<Guid, ExportOrderForJDDTO>();
                orderList1.Concat(orderList2).ToList().ForEach(p =>
                {
                    if (!dir.ContainsKey(p.OrderId))
                    {
                        dir.Add(p.OrderId, p);
                    }
                });
                var orderList = dir.Values.ToList();
                LogHelper.Debug("CommodityOrderBP.ExportOrderForJDExt,orderList.Count:" + orderList.Count);
                var orderIdList = orderList.Select(p => p.OrderId).ToList();
                var itemList = OrderItem.ObjectSet().Where(p => orderIdList.Contains(p.CommodityOrderId)).Select(p => new
                {
                    p.CommodityOrderId,
                    p.CommodityId,
                    CommodityStockId = p.CommodityStockId ?? Guid.Empty,
                    p.Number,
                    RealPrice = p.RealPrice ?? 0
                }).ToList();
                var commodityIdList = itemList.Where(p => p.CommodityStockId == Guid.Empty || p.CommodityId == p.CommodityStockId).Select(p => p.CommodityId).ToList();
                var commodityStockIdList = itemList.Where(p => p.CommodityStockId != Guid.Empty && p.CommodityId != p.CommodityStockId).Select(p => p.CommodityStockId).ToList();
                var commodityList = Commodity.ObjectSet().Where(p => commodityIdList.Contains(p.Id)).Select(p => new
                {
                    p.Id,
                    p.JDCode,
                    p.ErQiCode
                }).ToList();
                var commodityStockList = CommodityStock.ObjectSet().Where(p => commodityStockIdList.Contains(p.Id)).Select(p => new
                {
                    p.Id,
                    p.JDCode,
                    p.ErQiCode
                }).ToList();
                orderList.ForEach(p =>
                {
                    var items = itemList.Where(x => x.CommodityOrderId == p.OrderId).ToList();
                    items.ForEach(x =>
                    {
                        p.Numbers += x.Number + ",";
                        p.Prices += x.RealPrice + ",";
                        if (x.CommodityStockId == Guid.Empty || x.CommodityStockId == x.CommodityId)
                        {
                            var commodity = commodityList.Where(g => g.Id == x.CommodityId).FirstOrDefault();
                            if (commodity != null)
                            {
                                p.JdCodes += commodity.JDCode + ",";
                                p.ErQiCodes += commodity.ErQiCode + ",";
                            }
                        }
                        else
                        {
                            var commodityStock = commodityStockList.Where(g => g.Id == x.CommodityStockId).FirstOrDefault();
                            if (commodityStock != null)
                            {
                                p.JdCodes += commodityStock.JDCode + ",";
                                p.ErQiCodes += commodityStock.ErQiCode + ",";
                            }
                        }
                    });
                    p.Prices = p.Prices.TrimEnd(',');
                    p.Numbers = p.Numbers.TrimEnd(',');
                    p.JdCodes = p.JdCodes.TrimEnd(',');
                    p.ErQiCodes = p.ErQiCodes.TrimEnd(',');
                });
                return orderList;
            }
            catch (Exception ex)
            {
                LogHelper.Error("CommodityOrderBP.ExportOrderForJDExt异常", ex);
            }
            return new List<ExportOrderForJDDTO>();
        }

        /// <summary>
        /// 运费计算方法，或SV里调整，需要跟着去调整。
        /// </summary>
        /// <param name="FreightTo"></param>
        /// <param name="tem"></param>
        /// <param name="templateId"></param>
        /// <returns></returns>
        private decimal CalOneFreightExt(string FreightTo, Jinher.AMP.BTP.Deploy.CustomDTO.TemplateCountDTO tem, Guid templateId)
        {
            if (string.IsNullOrWhiteSpace(FreightTo))
            {
                return 0;
            }

            decimal dec = 0;
            FreightTemplate ft = FreightTemplate.ObjectSet().Where(s => s.Id == templateId).FirstOrDefault();
            if (ft != null)
            {
                //不包邮
                if (ft.IsFreeExp == false)
                {
                    List<FreightTemplateDetail> ftdList = FreightTemplateDetail.ObjectSet().Where(s => s.FreightTemplateId == ft.Id).ToList();
                    bool IsHaveFreightTo = false;

                    #region 读取运费详情数据
                    if (ftdList != null && ftdList.Count > 0)
                    {
                        foreach (FreightTemplateDetail detail in ftdList)
                        {
                            if (!string.IsNullOrEmpty(detail.DestinationCodes))
                            {
                                string[] freighttoList = detail.DestinationCodes.Replace("，", ",").Replace(";", ",").Replace("；", ",").Split(',');
                                bool isContain = false;
                                //改为包含关系
                                if (freighttoList != null && freighttoList.Length > 0)
                                {
                                    foreach (string fTo in freighttoList)
                                    {
                                        if (fTo.Contains(FreightTo))
                                        {
                                            isContain = true;
                                            break;
                                        }
                                    }
                                }
                                if (isContain)
                                {
                                    IsHaveFreightTo = true;
                                    if (detail.FirstCount >= tem.Count)
                                    {
                                        dec = dec + detail.FirstCountPrice;
                                    }
                                    else
                                    {
                                        dec = dec + detail.FirstCountPrice;
                                        decimal cou = (tem.Count - detail.FirstCount) / detail.NextCount;
                                        dec = dec + cou * detail.NextCountPrice;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    #endregion

                    #region 读取默认数据
                    if (IsHaveFreightTo == false)
                    {
                        if (ft.FirstCount >= tem.Count)
                        {
                            dec = dec + ft.FirstCountPrice;
                        }
                        else
                        {
                            dec = dec + ft.FirstCountPrice;
                            decimal cou = (tem.Count - ft.FirstCount) / ft.NextCount;
                            dec = dec + cou * ft.NextCountPrice;
                        }
                    }
                    #endregion
                }
            }
            return dec;
        }

        /// <summary>
        /// 发送订单支付实时数据到盈科大数据系统mq
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public void SendPayInfoToYKBDMqExt(Guid orderId)
        {
            LogHelper.Debug(string.Format("CommodityOrderBP.SendPayInfoToYKBDMqExt发送订单支付实时数据到盈科大数据系统mq,入参:orderId={0}", orderId));
            if (orderId == Guid.Empty) return;
            Task.Factory.StartNew(() =>
            {
                var json = string.Empty;
                var mqjson = string.Empty;
                var bdJournal = default(YKBigDataMqJournalDTO);
                try
                {
                    var order = CommodityOrder.ObjectSet().FirstOrDefault(o => o.Id == orderId);
                    if (order == null || !order.PaymentTime.HasValue || order.EsAppId != YJB.Deploy.CustomDTO.YJBConsts.YJAppId) return;
                    bdJournal = new YKBigDataMqJournalDTO
                    {
                        Id = Guid.NewGuid(),
                        SubTime = DateTime.Now,
                        OrderId = order.Id,
                        OrderCode = order.Code,
                        Source = "CommodityOrderBP.SendPayInfoToYKBDMqExt",
                        Message = "支付数据",
                        Result = "发送成功"
                    };
                    var jsonObj = new
                    {
                        OrderId = order.Id,
                        OrderCode = order.Code,
                        OrderTradeTime = order.PaymentTime.Value.ToString("yyyyMMddHHmmss"),
                        OrderMoney = order.Price,

                        OrderRealPrice = order.RealPrice
                    };
                    json = JsonConvert.SerializeObject(jsonObj);
                    bdJournal.Json = json;
                    if (!TPS.YJBJMQSV.SendToMq("bj_bd_order", json))
                    {
                        bdJournal.Result = "发送失败";
                        LogHelper.Error(string.Format("CommodityOrderBP.SendPayInfoToYKBDMqExt发送订单支付实时数据到盈科大数据系统mq失败,入参:orderId={0}", orderId));
                    }

                    //bj_bw_neworder 订单主表及明细表
                    CommodityOrderVM commodityOrder = new CommodityOrderVM()
                    {
                        Id = order.Id,
                        Name = order.Name,
                        Code = order.Code,
                        SubTime = order.SubTime,
                        SubId = order.SubId,
                        UserId = order.UserId,
                        AppId = order.AppId,
                        Price = order.Price,
                        State = order.State,
                        PaymentTime = order.PaymentTime,
                        RealPrice = order.RealPrice,
                        ReceiptUserName = order.ReceiptUserName,
                        ReceiptPhone = order.ReceiptPhone,
                        ReceiptAddress = order.ReceiptAddress,
                        Payment = order.Payment,
                        Province = order.Province,
                        City = order.City,
                        District = order.District,
                        IsModifiedPrice = order.IsModifiedPrice,
                        ModifiedOn = order.ModifiedOn,
                        PaymentState = order.PaymentState,
                        SrcType = order.SrcType,
                        SrcTagId = order.SrcTagId,
                        IsDel = order.IsDel,
                        CancelReason = order.CancelReason,
                        IsDelayConfirmTime = order.IsDelayConfirmTime,
                        Freight = order.Freight,
                        Commission = order.Commission,
                        Crowdfunding = order.IsCrowdfunding,//需要说明一下
                        CrowdfundingPrice = order.CrowdfundingPrice,
                        GoldPrice = order.GoldPrice,
                        GoldCoupon = order.GoldCoupon,
                        SelfTakeFlag = order.SelfTakeFlag,
                        SpreadGold = order.SpreadGold,
                        OwnerShare = order.OwnerShare,
                        ScoreState = order.ScoreState,
                        EsAppId = order.EsAppId,
                        DistributeMoney = order.DistributeMoney,
                        OrderType = order.OrderType,
                        ChannelShareMoney = order.ChannelShareMoney,
                        Duty = order.Duty,
                        Street = order.Street,
                        SetMealId = (Guid)order.SetMealId,
                        AppName = order.AppName,
                        SupplierName = order.SupplierName,
                        SupplierCode = order.SupplierCode
                    };

                    var orderItemsVMs = new List<OrderItemsVM>();
                    var orderItems = OrderItem.ObjectSet().Where(t => t.CommodityOrderId == orderId).ToList();
                    foreach (var orderItem in orderItems)
                    {
                        OrderItemsVM orderItemsVM = new OrderItemsVM();
                        orderItemsVM.Id = orderItem.Id;
                        orderItemsVM.Name = orderItem.Name;
                        orderItemsVM.Code = orderItem.Code;
                        orderItemsVM.SubTime = orderItem.SubTime;
                        orderItemsVM.SubId = orderItem.SubId;
                        orderItemsVM.Number = orderItem.Number;
                        orderItemsVM.CurrentPrice = orderItem.CurrentPrice;
                        orderItemsVM.PicturesPath = orderItem.PicturesPath;
                        orderItemsVM.CommodityOrderId = orderItem.CommodityOrderId;
                        orderItemsVM.CommodityId = orderItem.CommodityId;
                        orderItemsVM.PromotionId = orderItem.PromotionId;
                        orderItemsVM.CommodityAttributes = orderItem.CommodityAttributes;
                        orderItemsVM.RealPrice = orderItem.RealPrice;
                        orderItemsVM.Intensity = orderItem.Intensity;
                        orderItemsVM.AlreadyReview = orderItem.AlreadyReview;
                        orderItemsVM.DiscountPrice = orderItem.DiscountPrice;
                        orderItemsVM.CommodityStockId = orderItem.CommodityStockId;
                        orderItemsVM.ModifiedOn = orderItem.ModifiedOn;
                        orderItemsVM.ShareMoney = orderItem.ShareMoney;
                        orderItemsVM.ScorePrice = orderItem.ScorePrice;
                        orderItemsVM.Duty = orderItem.Duty;
                        orderItemsVM.CostPrice = orderItem.CostPrice;
                        orderItemsVM.Barcode = orderItem.Barcode;
                        orderItemsVM.TaxRate = orderItem.TaxRate;
                        orderItemsVM.InputRax = orderItem.InputRax;
                        orderItemsVM.No_Code = orderItem.No_Code;
                        orderItemsVM.Unit = orderItem.Unit;
                        orderItemsVM.Type = orderItem.Type;
                        orderItemsVM.PromotionDesc = orderItem.PromotionDesc;
                        if (orderItem.PromotionType != null)
                        {
                            orderItemsVM.PromotionType = orderItem.PromotionType;
                        }
                        if (orderItem.CouponPrice != null)
                        {
                            orderItemsVM.CouponPrice = (decimal)orderItem.CouponPrice;
                        }
                        if (orderItem.FreightPrice != null)
                        {
                            orderItemsVM.FreightPrice = (decimal)orderItem.FreightPrice;
                        }
                        if (orderItem.YouKaPercent != null)
                        {
                            orderItemsVM.YouKaPercent = (decimal)orderItem.YouKaPercent;
                        }
                        if (orderItem.YjbPrice != null)
                        {
                            orderItemsVM.YjbPrice = (decimal)orderItem.YjbPrice;
                        }
                        if (orderItem.YJCouponPrice != null)
                        {
                            orderItemsVM.YJCouponPrice = (decimal)orderItem.YJCouponPrice;
                        }
                        orderItemsVM.State = (int)orderItem.State;
                        orderItemsVM.Specifications = orderItem.Specifications;
                        orderItemsVM.AppId = (Guid)orderItem.AppId;
                        orderItemsVM.JdCode = orderItem.JDCode;
                        orderItemsVMs.Add(orderItemsVM);
                    }
                    commodityOrder.OrderItems = orderItemsVMs;

                    mqjson = JsonConvert.SerializeObject(commodityOrder);
                    if (!TPS.YJBJMQSV.SendToMq("bj_bw_neworder", mqjson))
                    {
                        bdJournal.Result = "新实时发送失败";
                        LogHelper.Error(string.Format("CommodityOrderBP.SendPayInfoToYKBDMqExt发送订单支付实时数据到盈科大数据系统mq失败222222,入参:orderId={0}", orderId));
                    }
                }
                catch (Exception ex)
                {
                    bdJournal.Result = "发送异常";
                    bdJournal.Message = ex.ToString();
                    LogHelper.Error(string.Format("JdEclpOrderBP.SendPayInfoToYKBDMqExt发送订单支付实时数据到盈科大数据系统mq异常,入参:orderId={0}", orderId), ex);
                }
                finally
                {
                    if (bdJournal != null) new YKBigDataMqJournal().Create(bdJournal);
                }
            });
        }

        /// <summary>
        /// 发送订单售中退款实时数据到盈科大数据系统mq
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public void SendRefundInfoToYKBDMqExt(Guid orderId)
        {
            LogHelper.Debug(string.Format("CommodityOrderBP.SendRefundInfoToYKBDMqExt发送订单售中退款实时数据到盈科大数据系统mq,入参:orderId={0}", orderId));
            if (orderId == Guid.Empty) return;
            Task.Factory.StartNew(() =>
            {
                var json = string.Empty;
                var mqjson = string.Empty;
                var bdJournal = default(YKBigDataMqJournalDTO);
                try
                {
                    var order = CommodityOrder.ObjectSet().FirstOrDefault(o => o.Id == orderId);
                    if (order == null || !order.PaymentTime.HasValue || order.EsAppId != YJB.Deploy.CustomDTO.YJBConsts.YJAppId) return;
                    var orderRefund = OrderRefund.ObjectSet()
                            .Where(p => p.OrderId == orderId && p.State == 1)
                            .OrderByDescending(t => t.ModifiedOn)
                            .FirstOrDefault();
                    if (orderRefund != null && orderRefund.RefundMoney <= 0) return;
                    bdJournal = new YKBigDataMqJournalDTO
                    {
                        Id = Guid.NewGuid(),
                        SubTime = DateTime.Now,
                        OrderId = order.Id,
                        OrderCode = order.Code,
                        OrderItemId = orderRefund.OrderItemId ?? Guid.Empty,
                        Source = "CommodityOrderBP.SendRefundInfoToYKBDMqExt",
                        Message = "售中退款数据",
                        Result = "发送成功"
                    };
                    var jsonObj = new
                    {
                        OrderId = order.Id,
                        OrderCode = order.Code,
                        OrderTradeTime = orderRefund.ModifiedOn.ToString("yyyyMMddHHmmss"),
                        OrderMoney = -orderRefund.RefundMoney
                    };
                    json = JsonConvert.SerializeObject(jsonObj);
                    bdJournal.Json = json;
                    if (!TPS.YJBJMQSV.SendToMq("bj_bd_order", json))
                    {
                        bdJournal.Result = "发送失败";
                        LogHelper.Error(string.Format("CommodityOrderBP.SendRefundInfoToYKBDMqExt发送订单售中退款实时数据到盈科大数据系统mq失败,入参:orderId={0}", orderId));
                    }

                    Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundDTO orderRefundDto = new Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundDTO();
                    orderRefundDto.Id = orderRefund.Id;
                    orderRefundDto.RefundType = orderRefund.RefundType;
                    orderRefundDto.RefundReason = orderRefund.RefundReason;
                    orderRefundDto.RefundMoney = orderRefund.RefundMoney;
                    orderRefundDto.RefundDesc = orderRefund.RefundDesc;
                    orderRefundDto.OrderId = orderRefund.OrderId;
                    orderRefundDto.State = orderRefund.State;
                    if (orderRefund.RefundExpCo != null)
                    {
                        orderRefundDto.RefundExpCo = orderRefund.RefundExpCo;
                    }
                    if (orderRefund.RefundExpOrderNo != null)
                    {
                        orderRefundDto.RefundExpOrderNo = orderRefund.RefundExpOrderNo;
                    }
                    orderRefundDto.OrderRefundImgs = orderRefund.OrderRefundImgs;
                    orderRefundDto.IsFullRefund = (bool)orderRefund.IsFullRefund ? 1 : 0;
                    orderRefundDto.strSubTime = orderRefund.SubTime.ToString("yyyyMMddHHmmss");
                    orderRefundDto.strModifiedOn = orderRefund.ModifiedOn.ToString("yyyyMMddHHmmss");
                    if (orderRefund.OrderItemId != null)
                    {
                        orderRefundDto.OrderItemId = orderRefund.OrderItemId;
                    }
                    orderRefundDto.DataType = orderRefund.DataType;
                    if (orderRefund.RefuseTime != null)
                    {
                        orderRefundDto.RefuseTime = orderRefund.RefuseTime;
                    }
                    if (orderRefund.RefundExpOrderTime != null)
                    {
                        orderRefundDto.RefundExpOrderTime = orderRefund.RefundExpOrderTime;
                    }
                    if (orderRefund.IsDelayConfirmTimeAfterSales != null)
                    {
                        orderRefundDto.IsDelayConfirmTimeAfterSales = orderRefund.IsDelayConfirmTimeAfterSales;
                    }
                    if (orderRefund.AgreeFlag != null)
                    {
                        orderRefundDto.AgreeFlag = orderRefund.AgreeFlag;
                    }
                    orderRefundDto.RefundScoreMoney = orderRefund.RefundScoreMoney;
                    orderRefundDto.RefundYJBMoney = orderRefund.RefundYJBMoney;
                    if (orderRefund.RefundFreightPrice != null)
                    {
                        orderRefundDto.RefundFreightPrice = orderRefund.RefundFreightPrice;
                    }
                    //bj_bw_refund1  售中退款
                    mqjson = JsonConvert.SerializeObject(orderRefundDto);
                    LogHelper.Info("bj_bw_refund1发送的json:" + mqjson);
                    if (!TPS.YJBJMQSV.SendToMq("bj_bw_refund1", mqjson))
                    {
                        bdJournal.Result = "新售中发送失败";
                        LogHelper.Error(string.Format("CommodityOrderBP.SendRefundInfoToYKBDMqExt发送售中退款订单数据到盈科大数据系统mq失败222222,入参:orderId={0}", orderId));
                    }
                }
                catch (Exception ex)
                {
                    bdJournal.Result = "发送异常";
                    bdJournal.Message = ex.ToString();
                    LogHelper.Error(string.Format("CommodityOrderBP.SendRefundInfoToYKBDMqExt发送订单售中退款实时数据到盈科大数据系统mq异常,入参:orderId={0}", orderId), ex);
                }
                finally
                {
                    if (bdJournal != null) new YKBigDataMqJournal().Create(bdJournal);
                }
            });
        }

        /// <summary>
        /// 发送订单售后退款实时数据到盈科大数据系统mq
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public void SendASRefundInfoToYKBDMqExt(Guid orderId)
        {
            LogHelper.Debug(string.Format("CommodityOrderBP.SendASRefundInfoToYKBDMqExt发送订单售后退款实时数据到盈科大数据系统mq,入参:orderId={0}", orderId));
            if (orderId == Guid.Empty) return;
            Task.Factory.StartNew(() =>
            {
                var json = string.Empty;
                var mqjson = string.Empty;
                var bdJournal = default(YKBigDataMqJournalDTO);
                try
                {
                    var order = CommodityOrder.ObjectSet().FirstOrDefault(o => o.Id == orderId);
                    if (order == null || !order.PaymentTime.HasValue || order.EsAppId != YJB.Deploy.CustomDTO.YJBConsts.YJAppId) return;
                    var orderRefund = OrderRefundAfterSales.ObjectSet()
                            .Where(p => p.OrderId == orderId && p.State == 1)
                            .OrderByDescending(t => t.ModifiedOn)
                            .FirstOrDefault();
                    if (orderRefund != null && orderRefund.RefundMoney <= 0) return;
                    bdJournal = new YKBigDataMqJournalDTO
                    {
                        Id = Guid.NewGuid(),
                        SubTime = DateTime.Now,
                        OrderId = order.Id,
                        OrderCode = order.Code,
                        OrderItemId = orderRefund.OrderItemId ?? Guid.Empty,
                        Source = "CommodityOrderBP.SendASRefundInfoToYKBDMqExt",
                        Message = "售后退款数据",
                        Result = "发送成功"
                    };
                    var jsonObj = new
                    {
                        OrderId = order.Id,
                        OrderCode = order.Code,
                        OrderTradeTime = orderRefund.ModifiedOn.ToString("yyyyMMddHHmmss"),
                        OrderMoney = -orderRefund.RefundMoney
                    };
                    json = JsonConvert.SerializeObject(jsonObj);
                    bdJournal.Json = json;
                    if (!TPS.YJBJMQSV.SendToMq("bj_bd_order", json))
                    {
                        bdJournal.Result = "发送失败";
                        LogHelper.Error(string.Format("CommodityOrderBP.SendASRefundInfoToYKBDMqExt发送订单售后退款实时数据到盈科大数据系统mq失败,入参:orderId={0}", orderId));
                    }

                    Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundAfterSalesDTO orderRefundAfterSalesDto = new Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundAfterSalesDTO();
                    orderRefundAfterSalesDto.Id = orderRefund.Id;
                    orderRefundAfterSalesDto.RefundType = orderRefund.RefundType;
                    orderRefundAfterSalesDto.RefundReason = orderRefund.RefundReason;
                    orderRefundAfterSalesDto.RefundMoney = orderRefund.RefundMoney;
                    orderRefundAfterSalesDto.RefundDesc = orderRefund.RefundDesc;
                    orderRefundAfterSalesDto.OrderId = orderRefund.OrderId;
                    if (orderRefund.OrderItemId != null)
                    {
                        orderRefundAfterSalesDto.OrderItemId = orderRefund.OrderItemId;
                    }
                    orderRefundAfterSalesDto.State = orderRefund.State;
                    if (orderRefund.RefundExpCo != null)
                    {
                        orderRefundAfterSalesDto.RefundExpCo = orderRefund.RefundExpCo;
                    }
                    if (orderRefund.RefundExpOrderNo != null)
                    {
                        orderRefundAfterSalesDto.RefundExpOrderNo = orderRefund.RefundExpOrderNo;
                    }
                    orderRefundAfterSalesDto.OrderRefundImgs = orderRefund.OrderRefundImgs;
                    orderRefundAfterSalesDto.IsFullRefund = orderRefund.IsFullRefund;
                    orderRefundAfterSalesDto.DataType = orderRefund.DataType;
                    orderRefundAfterSalesDto.strSubTime = orderRefund.SubTime.ToString("yyyyMMddHHmmss");
                    orderRefundAfterSalesDto.strModifiedOn = orderRefund.ModifiedOn.ToString("yyyyMMddHHmmss");
                    if (orderRefund.RefuseTime != null)
                    {
                        orderRefundAfterSalesDto.RefuseTime = orderRefund.RefuseTime;
                    }
                    if (orderRefund.RefundExpOrderTime != null)
                    {
                        orderRefundAfterSalesDto.RefundExpOrderTime = orderRefund.RefundExpOrderTime;
                    }
                    orderRefundAfterSalesDto.RefundScoreMoney = orderRefund.RefundScoreMoney;
                    orderRefundAfterSalesDto.RefundYJBMoney = orderRefund.RefundYJBMoney;
                    orderRefundAfterSalesDto.RefundFreightPrice = orderRefund.RefundFreightPrice;

                    //bj_bw_refund2  售后退款
                    mqjson = JsonConvert.SerializeObject(orderRefundAfterSalesDto);
                    LogHelper.Info("bj_bw_refund2发送的json数据:" + mqjson);
                    if (!TPS.YJBJMQSV.SendToMq("bj_bw_refund2", mqjson))
                    {
                        bdJournal.Result = "新售后发送失败";
                        LogHelper.Error(string.Format("CommodityOrderBP.SendASRefundInfoToYKBDMqExt发送订单售后退款实时数据到盈科大数据系统mq失败222222,入参:orderId={0}", orderId));
                    }
                }
                catch (Exception ex)
                {
                    bdJournal.Result = "发送异常";
                    bdJournal.Message = ex.ToString();
                    LogHelper.Error(string.Format("CommodityOrderBP.SendASRefundInfoToYKBDMqExt发送订单售后退款实时数据到盈科大数据系统mq异常,入参:orderId={0}", orderId), ex);
                }
                finally
                {
                    if (bdJournal != null) new YKBigDataMqJournal().Create(bdJournal);
                }
            });
        }
    }

    /// <summary>
    /// 售后
    /// </summary>
    class OrderRefundTmp
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// 订单号详情项
        /// </summary>
        public Guid OrderItemId { get; set; }

        /// <summary>
        /// 售后订单状态
        /// </summary>
        public int OrderStateAfterSales { get; set; }

        /// <summary>
        /// 退款金额
        /// </summary>
        public decimal RefundMoney { get; set; }

        /// <summary>
        /// 退款类型：仅退款=0，退货退款=1
        /// </summary>
        public int RefundType { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        public DateTime SubTime { get; set; }

        /// <summary>
        /// 退货物流单号
        /// </summary>
        public string RefundExpOrderNo { get; set; }
        /// <summary>
        /// 退货物流公司
        /// </summary>
        public string RefundExpCo { get; set; }

        /// <summary>
        /// 售后申请状态
        /// </summary>
        public int StateRefundAfterSales { get; set; }

        /// <summary>
        /// 售中申请状态
        /// </summary>
        public int StateRefund { get; set; }

        /// <summary>
        /// 售后拒绝时间/达成协议时间
        /// </summary>
        public DateTime? RefuseTime { get; set; }

        /// <summary>
        /// 退还积分抵现金额
        /// </summary>
        public decimal RefundScoreMoney { get; set; }

        /// <summary>
        /// 退还易捷币抵现金额
        /// </summary>
        public decimal RefundYJBMoney { get; set; }
    }
}
