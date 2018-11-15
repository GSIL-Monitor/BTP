extern alias snsdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using snsdk::suning_api_sdk.BizRequest.CustomGovbusRequest;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.CustomDTO.SN;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.BE;
using System.Threading.Tasks;
using Jinher.AMP.BTP.TPS.Helper;
using Jinher.AMP.BTP.IBP.Facade;
namespace Jinher.AMP.BTP.TPS
{
    /// <summary>
    /// 订单流程步骤
    /// 1:创建订单
    /// 2:支付
    /// 3:支付成功-预占库存
    /// 4:支付失败-取消预占
    /// 5:未支付取消订单
    /// </summary>
    public partial class SuningSV
    {
        /// <summary> 
        /// 创建订单接口
        /// </summary>
        public static void suning_govbus_order_add(OrderSDTO OrderInfo, Guid OrderId)
        {
            Task.Factory.StartNew(() =>
            {
                var ProductList = new List<snsdk.suning_api_sdk.Models.CustomGovbusModel.OrderAddSkuReq>();
                OrderInfo.ShoppingCartItemSDTO.ForEach(ProductItem =>
                {
                    if (ThirdECommerceHelper.IsSuNingYiGou(ProductItem.AppId))
                    {
                        ProductList.Add(new snsdk.suning_api_sdk.Models.CustomGovbusModel.OrderAddSkuReq
                        {
                            num = ProductItem.CommodityNumber.ToString(),
                            skuId = ProductItem.Code,
                            unitPrice = ProductItem.CostPrice.ToString()
                        });
                    }
                });
                if (ProductList.Count > 0)
                {
                    var request = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.OrderAddRequest();
                    request.sku = ProductList;
                    request.address = OrderInfo.ReceiptAddress;
                    request.amount = OrderInfo.Price.ToString("F2");
                    request.cityId = OrderInfo.CityCode;
                    request.countyId = OrderInfo.DistrictCode;
                    request.email = string.Empty;
                    request.mobile = OrderInfo.ReceiptPhone;
                    request.orderType = "1";
                    request.provinceId = OrderInfo.ProvinceCode;
                    request.receiverName = OrderInfo.ReceiptUserName;
                    request.remark = OrderInfo.Details;
                    request.servFee = "5.00";
                    request.taxNo = string.Empty;
                    request.telephone = string.Empty;
                    request.townId = OrderInfo.StreetCode;
                    request.tradeNo = string.Empty;
                    request.zip = OrderInfo.RecipientsZipCode;
                    request.invoiceState = "0";
                    if (OrderInfo.InvoiceInfo != null)
                    {
                        request.invoiceState = OrderInfo.InvoiceInfo.InvoiceType > 0 ? "1" : "0";
                        if (OrderInfo.InvoiceInfo.InvoiceType > 0)
                        {
                            request.invoiceContent = OrderInfo.InvoiceInfo.InvoiceContent;
                            request.invoiceTitle = OrderInfo.InvoiceInfo.InvoiceTitle;
                            request.invoiceType = "6";
                        }
                    }
                    request.payment = "08";
                    var response = SuningClient.Execute(request);
                    if (response.respError != null)
                    {
                        string Message = response.respError.error_msg + "===" + response.respError.error_msg;
                        LogHelper.Error(string.Format("苏宁接口[suning.govbus.order.add]：{0}", Message));
                    }
                    else
                    {
                        var SNOrderItemList = new List<SNOrderItemDTO>();
                        OrderItem.ObjectSet().Where(w => w.CommodityOrderId == OrderId)
                        .ToList()
                        .ForEach(Item =>
                        {
                            if (Item.AppId != null)
                            {
                                if (ThirdECommerceHelper.IsSuNingYiGou((Guid)Item.AppId))
                                {
                                    var Procdurt = Commodity.ObjectSet()
                                        .Where(w => w.Id == Item.CommodityId)
                                        .FirstOrDefault();
                                    if (Procdurt != null)
                                    {
                                        var time = DateTime.Now;
                                        var SuningProcudurt = response.skus.Where(w => w.skuId == Procdurt.JDCode)
                                                                .FirstOrDefault();
                                        SNOrderItemList.Add(new SNOrderItemDTO
                                        {
                                            Id = Guid.NewGuid(),
                                            OrderId = OrderId,
                                            OrderItemId = Item.Id,
                                            OrderCode = Procdurt.Code,
                                            CustomOrderId = response.orderId,
                                            CustomOrderItemId = SuningProcudurt.orderItemId,
                                            CustomSkuId = SuningProcudurt.skuId,
                                            Status = 0,
                                            ExpressStatus = 0,
                                            ModifiedOn = time,
                                            DeliveryType = 0,
                                            SubTime = time
                                        });
                                    }
                                }
                            }
                        });
                        if (SNOrderItemList.Count > 0)
                            new SNOrderItemFacade().AddSNOrderItem(SNOrderItemList);
                    }
                }
            });
        }

        /// <summary>
        /// 确认预占库存订单接口
        /// </summary>
        public static bool suning_govbus_confirmorder_add(Guid OrderId)
        {
            try
            {
                var relation = SNOrderItem.ObjectSet().Where(w => w.OrderId == OrderId).FirstOrDefault();
                if (relation == null)
                    return false;
                var request = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.ConfirmOrderAddRequest();
                request.orderId = relation.CustomOrderId;
                var response = SuningClient.Execute(request);
                if (response.respError == null)
                    return response.campSuccess == "Y";
                else
                {
                    string Message = response.respError.error_msg + "===" + response.respError.error_msg;
                    LogHelper.Error(string.Format("苏宁接口[suning.govbus.confirmorder.add]：{0}", Message));
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("苏宁接口[suning.govbus.confirmorder.add]Exception：" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 取消预占库存订单接口
        /// </summary>
        public static bool suning_govbus_rejectorder_delete(Guid OrderId)
        {
            try
            {
                var relation = SNOrderItem.ObjectSet().Where(w => w.OrderId == OrderId).FirstOrDefault();
                if (relation == null)
                    return false;
                var request = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.RejectOrderDeleteRequest();
                request.orderId = relation.CustomOrderId;
                var response = SuningClient.Execute(request);
                if (response.respError == null)
                    return response.unCampSuccess == "Y";
                else
                {
                    string Message = response.respError.error_msg + "===" + response.respError.error_msg;
                    LogHelper.Error(string.Format("苏宁接口[suning.govbus.confirmorder.add]：{0}", Message));
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("苏宁接口[suning.govbus.confirmorder.add]Exception：" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 未支付订单取消接口
        /// </summary>
        public static void suning_govbus_unpaidorder_cancel(string OrderId)
        {
            var request = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.UnpaidorderCancelRequest();
            request.orderId = OrderId;
            var response = SuningClient.Execute(request);
        }

        /// <summary>
        /// 获取订单状态接口
        /// </summary>
        /// <param name="OrderId"></param>
        public static void suning_govbus_orderstatus_get(string OrderId)
        {
            var request = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.OrderStatusGetRequest();
            request.orderId = OrderId;
            var response = SuningClient.Execute(request);
        }
        /// <summary>
        /// 厂送商品确认收货接口
        /// </summary>
        /// <param name="ConfirmParams"></param>
        /// <returns></returns>
        public static bool suning_govbus_facproduct_confirm(SNConfirmParamsDto ConfirmParams)
        {
            try
            {
                var request = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.FacProductConfirmRequest();
                request.orderId = ConfirmParams.OrderId;
                request.skuConfirmList = ConfirmParams.SkuConfirmList.Select(s =>
                new snsdk.suning_api_sdk.Models.CustomGovbusModel.FacProductConfirmSkuConfirmListReq
                {
                    skuId = s.SkuId,
                    confirmTime = s.ConfirmTime
                }).ToList();
                return SuningClient.Execute(request).apiIsSuccess == "Y";
            }
            catch (Exception ex)
            {
                LogHelper.Error("苏宁接口[suning.govbus.facproduct.confirm]：" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 获取订单物流详情
        /// </summary>
        public static List<SNOrderLogisticsDto> suning_govbus_orderlogistnew_get(SNQueryParamsDto list)
        {
            var request = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.OrderlogistnewGetRequest();
            request.orderId = list.OrderId;
            if (list.OrderItemIds != null)
            {
                if (list.OrderItemIds.Count > 0)
                {
                    request.orderItemIds = list.OrderItemIds.Select(Item =>
                    new snsdk.suning_api_sdk.Models.CustomGovbusModel.OrderlogistnewGetOrderItemIdsReq
                    {
                        orderItemId = Item.OrderItemId,
                        skuId = Item.SkuId
                    }).ToList();
                }
            }
            var response = SuningClient.Execute(request);
            if (response.respError != null)
                return null;
            return response.packageIds.Select(s => new SNOrderLogisticsDto
            {
                IsPackage = response.isPackage,
                OrderId = response.orderId,
                PackageId = s.packageId,
                ReceiveTime = s.receiveTime,
                ShippingTime = s.shippingTime,
                OrderItemIds = s.orderItemIds.Select(i => new OrderItemIds
                {
                    OrderItemId = i.orderItemId,
                    SkuId = i.skuId
                }).ToList(),
                OrderLogistics = s.orderLogistics.Select(j => new OrderLogistics
                {
                    OperateState = j.operateState,
                    OperateTime = j.operateTime
                }).ToList()
            }).ToList();
        }
    }
}
