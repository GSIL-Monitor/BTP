extern alias snsdk;

using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using snsdk::suning_api_sdk;
using Jinher.AMP.BTP.Deploy.CustomDTO.SN;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using System.Threading.Tasks;
using Jinher.AMP.BTP.TPS.Helper;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Deploy.CustomDTO.SN;
using snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest;
using snsdk.suning_api_sdk.BizResponse.CustomGovbusResponse;
using Jinher.AMP.BTP.Deploy.CustomDTO.AfterSales;
using Jinher.JAP.PL;
using Newtonsoft.Json;
using Jinher.AMP.BTP.Deploy.CustomDTO.ThirdECommerce;
using Jinher.AMP.BTP.Deploy.Enum;

namespace Jinher.AMP.BTP.TPS
{
    public partial class SuningSV
    {
        /// <summary>
        /// 服务基础URL
        /// </summary>
        private static string URLBASE { get; set; }
        /// <summary>
        /// 服务AppKey
        /// </summary>
        private static string APPKEY { get; set; }
        /// <summary>
        /// 服务AppSecret
        /// </summary>
        private static string APPSECRET { get; set; }
        /// <summary>
        /// 获取请求数据
        /// </summary>
        public static snsdk.suning_api_sdk.ISuningClient SuningClient
        {
            get
            {
                URLBASE = CustomConfig.Suning_UrlBase;
                APPKEY = CustomConfig.Suning_AppKey;
                APPSECRET = CustomConfig.Suning_AppSecret;
                snsdk.suning_api_sdk.Logger.SuningLogger.IsLogDebug = true;
                return new snsdk.suning_api_sdk.DefaultSuningClient(URLBASE, APPKEY, APPSECRET);
            }
        }

        /// <summary>
        /// 苏宁消息池接口
        /// </summary>
        /// <param name="msgType">
        /// 消息类型
        /// 10-商品池 上架、下架、添加、删除、修改 
        /// 11-订单 实时创建、预占成功、确认预占、取消预占、异常订单取消　
        /// 12-物流 商品出库、商品妥投、商品拒收、商品退货 
        /// 13-目录 添加、修改、删除
        /// </param>
        /// <returns></returns>
        public static List<SNGetMessageDto> suning_govbus_message_get(string msgType)
        {
            try
            {
                var request = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.MessageGetRequest();
                request.type = msgType;
                var response = SuningClient.Execute(request);
                //发送数据到mq
                var json = SerializationHelper.JsonSerialize(new { MsgType = msgType, Request = request, Response = response });
                if (msgType == "10")
                {
                    RabbitMqHelper.Send(RabbitMqRoutingKey.SnCommodityMsgReceived, RabbitMqExchange.Commodity, json);
                }
                else if (msgType == "11" || msgType == "12")
                {
                    RabbitMqHelper.Send(RabbitMqRoutingKey.SnOrderMsgReceived, RabbitMqExchange.Order, json);
                }
                LogHelper.Info(string.Format("SuningSV.suning_govbus_message_get 苏宁消息池接口,入参:{0},出参:{1}", msgType, SerializationHelper.JsonSerialize(response)));
                if (response.respError == null)
                {
                    if (response.resultInfo != null)
                    {
                        return response.resultInfo.Select(s => new SNGetMessageDto
                        {
                            id = s.id,
                            orderItemNo = s.orderItemNo,
                            orderNo = s.orderNo,
                            cmmdtyCode = s.cmmdtyCode,
                            status = s.status,
                            type = s.type
                        }).ToList();
                    }
                }
                else
                {
                    LogHelper.Error("苏宁接口[suning_govbus_message_get]：" + response.respError);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("苏宁接口[suning_govbus_message_get]：" + ex);
            }
            return null;
        }

        /// <summary>
        /// 通过此接口可根据消息id删除具体消息。 
        /// </summary>
        /// <param name="Id">消息id</param>
        /// <returns></returns>
        public static bool suning_govbus_message_delete(string Id)
        {
            try
            {
                var request = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.MessageDeleteRequest();
                request.id = Id;
                var response = SuningClient.Execute(request);
                if (response.respError == null)
                    return response.isDelSuccess == "Y" ? true : false;
            }
            catch (Exception ex)
            {
                LogHelper.Error("苏宁接口[suning_govbus_message_delete]：" + ex);
            }
            return false;
        }

        #region 刘宝刚
        /// <summary> 
        /// 创建订单接口
        /// </summary>
        private static string suning_govbus_order_add(string Name, OrderSDTO OrderInfo)
        {
            string ReutnValue = string.Empty;
            if (",1,2,3,4,".IndexOf(OrderInfo.ProvinceCode) > 0)
            {
                switch (Name)
                {
                    case "省": ReutnValue = "0" + OrderInfo.ProvinceCode; break;
                    case "市": ReutnValue = OrderInfo.ProvinceCode; break;
                    case "区": ReutnValue = OrderInfo.CityCode; break;
                }
            }
            else
            {
                switch (Name)
                {
                    case "省": ReutnValue = OrderInfo.ProvinceCode; break;
                    case "市": ReutnValue = OrderInfo.CityCode; break;
                    case "区": ReutnValue = OrderInfo.DistrictCode; break;
                }
            }
            return ReutnValue;
        }

        public static void suning_govbus_order_add(Guid OrderId)
        {
            string uuid = Guid.NewGuid().ToString();
            if (SNOrderItem.ObjectSet().Where(w => w.OrderId == OrderId).Count() == 0)
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var ProductList = new List<snsdk.suning_api_sdk.Models.CustomGovbusModel.OrderAddSkuReq>();
                OrderItem.ObjectSet().Where(w => w.CommodityOrderId == OrderId)
                .ToList().ForEach(ProductItem =>
                {
                    var appid = (Guid)ProductItem.AppId;
                    if (ThirdECommerceHelper.IsSuNingYiGou(appid))
                    {
                        if (SNOrderItem.ObjectSet().Where(w => w.OrderId == OrderId && w.OrderItemId == ProductItem.Id).Count() == 0)
                        {
                            ProductList.Add(new snsdk.suning_api_sdk.Models.CustomGovbusModel.OrderAddSkuReq
                            {
                                num = ProductItem.Number.ToString(),
                                skuId = ProductItem.JDCode,
                                unitPrice = ProductItem.CostPrice.ToString()
                            });
                        }
                    }
                });
                if (ProductList.Count == 0)
                {
                    throw new Exception("手动-没有苏宁易购商品！");
                }
                var OrderInfo = CommodityOrder.ObjectSet().Where(w => w.Id == OrderId && w.PaymentTime.HasValue).FirstOrDefault();
                if (OrderInfo != null)
                {
                    var SalesList = SNOrderAfterSalesHelper.GetCityId();
                    var ProvinceCode = SalesList.Where(w => w.N == OrderInfo.Province && w.L == "1").FirstOrDefault();
                    var CityCode = SalesList.Where(w => w.N == OrderInfo.City && w.P == ProvinceCode.A).FirstOrDefault();
                    var DistrictCode = SalesList.Where(w => w.N == OrderInfo.District && w.P == CityCode.A).FirstOrDefault();
                    string provinceId_cityId_countyId = string.Empty;
                    if (",北京市,上海市,天津市,重庆市,".IndexOf(OrderInfo.Province) > 0)
                    {
                        provinceId_cityId_countyId = "0" + ProvinceCode.A + "," + ProvinceCode.A + "," + CityCode.A;
                    }
                    else
                    {
                        provinceId_cityId_countyId = ProvinceCode.A + "," + CityCode.A + "," + DistrictCode.A;
                    }

                    var SumPrice = ProductList.Sum(s => Convert.ToDecimal(s.num) * Convert.ToDecimal(s.unitPrice));
                    var request = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.OrderAddRequest();
                    request.sku = ProductList;
                    request.address = OrderInfo.ReceiptAddress;
                    request.provinceId = provinceId_cityId_countyId.Split(',')[0];
                    request.cityId = provinceId_cityId_countyId.Split(',')[1];
                    request.countyId = provinceId_cityId_countyId.Split(',')[2];
                    request.companyCustNo = "";
                    request.amount = SumPrice.ToString("F2");
                    request.email = string.Empty;
                    request.mobile = OrderInfo.ReceiptPhone;
                    request.orderType = "1";
                    request.receiverName = OrderInfo.ReceiptUserName;
                    request.remark = OrderInfo.Details;
                    request.servFee = SumPrice > 86 ? "0.00" : "5.00";
                    request.taxNo = string.Empty;
                    request.telephone = string.Empty;
                    request.townId = OrderInfo.RecipientsZipCode;
                    request.tradeNo = uuid;
                    request.zip = OrderInfo.RecipientsZipCode;
                    request.invoiceState = "0";
                    request.invoiceState = "1";
                    request.invoiceContent = "明细";
                    request.invoiceTitle = "中国石化销售有限公司北京石油分公司";
                    request.invoiceType = "6";
                    request.payment = "08";
                    var response = SuningClient.Execute(request);
                    //发送数据到mq
                    RabbitMqHelper.Send(RabbitMqRoutingKey.SnOrderPreCreateEnd, RabbitMqExchange.Order
                        , string.Format("orderId={0}&request={1}&response={2}", OrderId, request.ToJson(), response.ToJson()));
                    if (response.respError != null)
                    {
                        string Message = response.respError.error_code + "===" + response.respError.error_msg;
                        LogHelper.Error(string.Format("手动-苏宁接口[suning.govbus.order.add]：{0}", Message));
                        LogHelper.Error("手动-苏宁接口请求参数：" + JsonConvert.SerializeObject(request));
                        throw new Exception(Message);
                    }
                    else
                    {
                        LogHelper.Info("手动-苏宁接口响应参数：" + JsonConvert.SerializeObject(response));
                        var OrderTime = DateTime.Now;
                        response.skus.ForEach(Item =>
                        {
                            var Commodity = OrderItem.ObjectSet()
                                                .Where(w => w.CommodityOrderId == OrderId)
                                                .Where(w => w.JDCode == Item.skuId)
                                                .FirstOrDefault();
                            contextSession.SaveObject(new SNOrderItem
                            {
                                Id = Guid.NewGuid(),
                                OrderId = OrderId,
                                OrderItemId = Commodity.Id,
                                OrderCode = Commodity.No_Code,
                                CustomOrderId = response.orderId,
                                CustomOrderItemId = Item.orderItemId,
                                CustomSkuId = Item.skuId,
                                Status = 0,
                                ExpressStatus = 0,
                                ModifiedOn = OrderTime,
                                DeliveryType = 0,
                                SubTime = OrderTime,
                                EntityState = EntityState.Added
                            });
                        });
                        contextSession.SaveObject(new SNOrder
                        {
                            Id = Guid.NewGuid(),
                            OrderId = OrderId,
                            CustomOrderId = response.orderId,
                            RequestFee = Convert.ToDecimal(request.servFee),
                            ResponseFee = Convert.ToDecimal(response.freight),
                            SubTime = OrderTime,
                            ModifiedOn = OrderTime,
                            EntityState = EntityState.Added
                        });
                        contextSession.SaveChange();
                        //确认预占库存
                        var Order = CommodityOrder.ObjectSet().Where(w => w.Id == OrderId && w.PaymentTime.HasValue).FirstOrDefault();
                        if (Order != null)
                        {
                            //已经支付的才可以确认预占
                            var relation = SNOrderItem.ObjectSet().Where(w => w.OrderId == OrderId).FirstOrDefault();
                            if (relation != null)
                            {
                                var SNRequest = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.ConfirmOrderAddRequest();
                                SNRequest.orderId = relation.CustomOrderId;
                                var SNResponse = SuningClient.Execute(SNRequest);
                                LogHelper.Info(string.Format("手动-苏宁接口[suning.govbus.confirmorder.add]:入参：{0},出参:{1}", SerializationHelper.JsonSerialize(SNRequest), SerializationHelper.JsonSerialize(SNResponse)));
                                if (SNResponse.respError != null)
                                {
                                    string Message = SNResponse.respError.error_code + "===" + SNResponse.respError.error_msg;
                                    throw new Exception(string.Format("手动-苏宁接口[suning.govbus.confirmorder.add]：{0}", Message));
                                }
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception("手动-此订单未付款！");
                }
            }
            else
            {
                throw new Exception("手动-此订单已经补过数据！");
            }
            throw new Exception("新ID：" + uuid + "====原ID：" + OrderId);
        }

        public static List<SNOrderItem> suning_govbus_order_add(ContextSession contextSession, OrderSDTO OrderInfo, Guid OrderId)
        {
            //苏宁商品校验商品编码是否为空  
            OrderInfo.ShoppingCartItemSDTO.ForEach(ProductItem =>
            {
                if (string.IsNullOrWhiteSpace(ProductItem.JDCode))
                {
                    LogHelper.Error("苏宁接口[suning.govbus.order.add]：商品skuid为空");
                    AddSnlogs("苏宁接口异常：商品skuid为空；订单ID" + OrderId);
                    throw new Exception("苏宁接口[suning.govbus.order.add]商品skuid为空");
                }
            });
            var ProductList = new List<snsdk.suning_api_sdk.Models.CustomGovbusModel.OrderAddSkuReq>();
            OrderInfo.ShoppingCartItemSDTO.ForEach(ProductItem =>
            {
                ProductList.Add(new snsdk.suning_api_sdk.Models.CustomGovbusModel.OrderAddSkuReq
                {
                    num = ProductItem.CommodityNumber.ToString(),
                    skuId = ProductItem.JDCode,
                    unitPrice = ProductItem.CostPrice.ToString()
                });
            });
            if (ProductList.Count == 0)
                return null;
            var SumPrice = ProductList.Sum(s => Convert.ToDecimal(s.num) * Convert.ToDecimal(s.unitPrice));
            var request = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.OrderAddRequest();
            request.sku = ProductList;
            request.address = OrderInfo.ReceiptAddress;
            request.provinceId = suning_govbus_order_add("省", OrderInfo);
            request.cityId = suning_govbus_order_add("市", OrderInfo);
            request.countyId = suning_govbus_order_add("区", OrderInfo);
            request.companyCustNo = "";
            request.amount = SumPrice.ToString("F2");
            request.email = string.Empty;
            request.mobile = OrderInfo.ReceiptPhone;
            request.orderType = "1";
            request.receiverName = OrderInfo.ReceiptUserName;
            request.remark = OrderInfo.Details;
            request.servFee = SumPrice > 86 ? "0.00" : "5.00";
            request.taxNo = string.Empty;
            request.telephone = string.Empty;
            request.townId = OrderInfo.StreetCode;
            request.tradeNo = OrderId.ToString();
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
            //发送数据到mq
            var json = SerializationHelper.JsonSerialize(new { OrderId, Request = request, Response = response });
            RabbitMqHelper.Send(RabbitMqRoutingKey.SnOrderPreCreateEnd, RabbitMqExchange.Order, json);
            if (response.respError != null)
            {
                string Message = response.respError.error_code + "===" + response.respError.error_msg;
                LogHelper.Error(string.Format("苏宁接口[suning.govbus.order.add]：{0}", Message));
                AddSnlogs(string.Format("苏宁接口调用错误：{0}；订单ID：{1}", Message, OrderId));
                LogHelper.Error("苏宁接口请求参数：" + JsonConvert.SerializeObject(request));
                return null;
            }
            else
            {
                LogHelper.Info("苏宁接口响应参数：" + JsonConvert.SerializeObject(response));
                var OrderTime = DateTime.Now;
                var SNOrderItemList = new List<SNOrderItem>();
                response.skus.ForEach(Item =>
                {
                    var CommodityCode = OrderInfo.ShoppingCartItemSDTO
                                        .Where(w => w.JDCode == Item.skuId)
                                        .FirstOrDefault()
                                        .Code;
                    SNOrderItemList.Add(new SNOrderItem
                    {
                        Id = Guid.NewGuid(),
                        OrderId = OrderId,
                        OrderCode = CommodityCode,
                        CustomOrderId = response.orderId,
                        CustomOrderItemId = Item.orderItemId,
                        CustomSkuId = Item.skuId,
                        Status = 0,
                        ExpressStatus = 0,
                        ModifiedOn = OrderTime,
                        DeliveryType = 0,
                        SubTime = OrderTime
                    });
                });
                contextSession.SaveObject(new SNOrder
                {
                    Id = Guid.NewGuid(),
                    OrderId = OrderId,
                    CustomOrderId = response.orderId,
                    RequestFee = Convert.ToDecimal(request.servFee),
                    ResponseFee = Convert.ToDecimal(response.freight),
                    SubTime = OrderTime,
                    ModifiedOn = OrderTime,
                    EntityState = EntityState.Added
                });
                LogHelper.Info("调用苏宁返回参数：" + JsonConvert.SerializeObject(SNOrderItemList));
                return SNOrderItemList;
            }
        }
        public static void suning_govbus_order_add(ContextSession contextSession, OrderItem orderItem, List<SNOrderItem> snOrderItem)
        {
            if (snOrderItem != null)
            {
                if (snOrderItem.Count > 0)
                {
                    var EntityModel = snOrderItem.Where(w => w.OrderId == orderItem.CommodityOrderId)
                                       .Where(w => w.CustomSkuId == orderItem.JDCode)
                                       .FirstOrDefault();
                    if (EntityModel != null)
                    {
                        EntityModel.OrderItemId = orderItem.Id;
                        EntityModel.EntityState = EntityState.Added;
                        contextSession.SaveObject(EntityModel);
                        LogHelper.Info("保存数据[suning_govbus_order_add]：" + JsonConvert.SerializeObject(EntityModel.ToEntityData()));
                    }
                }
            }
        }
        /// <summary>
        /// 确认预占库存订单接口
        /// </summary>
        public static void suning_govbus_confirmorder_add(Guid OrderId, bool IsMainOrder)
        {
            try
            {
                var request = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.ConfirmOrderAddRequest();
                if (IsMainOrder)
                {
                    MainOrder.ObjectSet().Where(r => r.MainOrderId == OrderId).ToList()
                    .ForEach(Item =>
                    {
                        var Order = CommodityOrder.ObjectSet().Where(w => w.Id == Item.SubOrderId && w.PaymentTime.HasValue && w.State == 1).FirstOrDefault();
                        if (Order != null)
                        {
                            //已经支付的才可以确认预占
                            var relation = SNOrderItem.ObjectSet().Where(w => w.OrderId == Item.SubOrderId).FirstOrDefault();
                            if (relation != null)
                            {
                                request.orderId = relation.CustomOrderId;
                                var response = SuningClient.Execute(request);
                                LogHelper.Info(string.Format("苏宁接口[suning.govbus.confirmorder.add]:入参：{0},出参:{1}", SerializationHelper.JsonSerialize(request), SerializationHelper.JsonSerialize(response)));
                                if (response.respError != null)
                                {
                                    string Message = response.respError.error_code + "===" + response.respError.error_msg;
                                    LogHelper.Error(string.Format("苏宁接口[suning.govbus.confirmorder.add]：{0}", Message));
                                    AddSnlogs(string.Format("苏宁接口调用错误：{0}；订单ID：{1}", Message, OrderId));
                                }
                            }
                        }
                    });
                }
                else
                {
                    var Order = CommodityOrder.ObjectSet().Where(w => w.Id == OrderId && w.PaymentTime.HasValue && w.State == 1).FirstOrDefault();
                    if (Order != null)
                    {
                        //已经支付的才可以确认预占
                        var relation = SNOrderItem.ObjectSet().Where(w => w.OrderId == OrderId).FirstOrDefault();
                        if (relation != null)
                        {
                            request.orderId = relation.CustomOrderId;
                            var response = SuningClient.Execute(request);
                            LogHelper.Info(string.Format("苏宁接口[suning.govbus.confirmorder.add]:入参：{0},出参:{1}", SerializationHelper.JsonSerialize(request), SerializationHelper.JsonSerialize(response)));
                            if (response.respError != null)
                            {
                                string Message = response.respError.error_code + "===" + response.respError.error_msg;
                                LogHelper.Error(string.Format("苏宁接口[suning.govbus.confirmorder.add]：{0}", Message));
                                AddSnlogs(string.Format("苏宁接口调用错误：{0}；订单ID：{1}", Message, OrderId));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("苏宁接口[suning.govbus.confirmorder.add]Exception：" + ex);
                AddSnlogs("苏宁接口Exception：" + ex.Message);
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
                    string Message = response.respError.error_code + "===" + response.respError.error_msg;
                    LogHelper.Error(string.Format("苏宁接口[suning_govbus_rejectorder_delete]：{0}", Message));
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("苏宁接口[suning_govbus_rejectorder_delete]Exception：" + ex);
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
        public static void suning_govbus_orderstatus_get(string OrderId)
        {
            var request = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.OrderStatusGetRequest();
            request.orderId = OrderId;
            var response = SuningClient.Execute(request);
        }
        /// <summary>
        /// 厂送商品确认收货接口
        /// </summary>
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
                LogHelper.Error("苏宁接口[suning.govbus.facproduct.confirm]：" + ex);
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
            LogHelper.Error("苏宁接口[suning_govbus_orderlogistnew_get]：" + response.ToJson());
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
        /// <summary>
        /// 根据物流状态改订单状态
        /// </summary>
        public static void suning_govbus_rejection_changestatus(string Status, SNOrderItem EntityQuery)
        {
            int ReStatus = Convert.ToInt32(Status);
            int OrderItemCount = SNOrderItem.ObjectSet().Where(w => w.OrderId == EntityQuery.OrderId).Count();
            switch (Status)
            {
                case "1"://商品出库-已发货
                    {
                        if (SNOrderItem.ObjectSet().Where(w => w.OrderId == EntityQuery.OrderId
                                        && w.ExpressStatus >= ReStatus).Count() == OrderItemCount)
                        {
                            var commodityorderfacade = new CommodityOrderFacade()
                            {
                                ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo()
                            };
                            var result = commodityorderfacade.UpdateCommodityOrder(
                                new Jinher.AMP.BTP.Deploy.CommodityOrderDTO()
                                {
                                    Id = EntityQuery.OrderId,
                                    State = 2,
                                    ShipmentsTime = DateTime.Now
                                });
                            if (result.ResultCode != 0)
                            {
                                LogHelper.Error("苏宁-发货失败:" + result.Message + ",入参:" + EntityQuery.OrderId);
                            }
                        }
                    }; break;
                case "2"://商品妥投-已收货
                    {
                        if (SNOrderItem.ObjectSet().Where(w => w.OrderId == EntityQuery.OrderId
                                        && w.ExpressStatus >= ReStatus).Count() == OrderItemCount)
                        {
                            var order = CommodityOrder.ObjectSet().FirstOrDefault(p => p.Id == EntityQuery.OrderId);
                            if (order == null)
                            {
                                LogHelper.Error("苏宁-妥投后自动确认收货:未找到订单,入参:" + EntityQuery.OrderId);
                                return;
                            }
                            var facade = new Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade();
                            facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                            var result = facade.UpdateCommodityOrder(3, EntityQuery.OrderId, order.SubId, order.AppId, order.Payment, string.Empty, string.Empty);
                            if (result.ResultCode != 0)
                            {
                                LogHelper.Error("苏宁-妥投后自动确认收货失败:" + result.Message + ",入参:" + EntityQuery.OrderId);
                            }
                        }
                    }; break;
                case "3"://商品拒收-拒收货
                    {
                        var orderItem = OrderItem.ObjectSet().Where(w => w.Id == EntityQuery.OrderItemId).FirstOrDefault();
                        if (orderItem == null)
                            throw new Exception("未找到订单项ID为：" + EntityQuery.OrderItemId);
                        else
                        {
                            // 检查是否已申请退款
                            if (!OrderRefundAfterSales.ObjectSet().Any(w => w.OrderId == EntityQuery.OrderId && w.OrderItemId == EntityQuery.OrderItemId))
                            {
                                SubmitOrderRefundDTO modelParam = new SubmitOrderRefundDTO();
                                modelParam.Id = modelParam.commodityorderId = EntityQuery.OrderId;
                                modelParam.RefundDesc = "苏宁商品拒收自动退款";
                                var CurrPic = orderItem.RealPrice * orderItem.Number;
                                if (CurrPic == 0)
                                {
                                    CurrPic = (orderItem.DiscountPrice * orderItem.Number);
                                }
                                //已发货时默认退款金额=该商品实际售价-该商品承担的优惠券金额-该商品承担的改价商品金额-关税-易捷抵用券金额
                                modelParam.RefundMoney = CurrPic.Value - (orderItem.CouponPrice ?? 0) - (orderItem.ChangeRealPrice ?? 0) - orderItem.Duty - (orderItem.YJCouponPrice ?? 0);
                                if (modelParam.RefundMoney <= 0)
                                {
                                    LogHelper.Error(@"苏宁商品拒收自动退款失败，退款金额为零, 提交申请：CommodityOrderItemId="
                                                    + EntityQuery.OrderItemId + "CurrPic: " + CurrPic.Value + "CouponPrice: "
                                                    + (orderItem.CouponPrice ?? 0) + "ChangeRealPrice: " + (orderItem.ChangeRealPrice ?? 0)
                                                    + "Duty: " + orderItem.Duty + "YJCouponPrice: " + orderItem.YJCouponPrice
                                                    + "CommodityOrderId=" + modelParam.commodityorderId + "CommodityOrderItemId=" + modelParam.OrderItemId);
                                }
                                modelParam.State = 3;
                                modelParam.RefundReason = "其他";
                                modelParam.RefundType = 0;// 仅退款
                                modelParam.OrderRefundImgs = "";
                                modelParam.OrderItemId = EntityQuery.OrderItemId;
                                var orderSV = new ISV.Facade.CommodityOrderAfterSalesFacade
                                {
                                    ContextDTO = AuthorizeHelper.InitAuthorizeInfo()
                                };
                                var result = orderSV.SubmitOrderRefundAfterSales(modelParam);
                                if (result.ResultCode != 0)
                                {
                                    LogHelper.Error("苏宁商品拒收自动退款失败，提交申请：CommodityOrderItemId=" + modelParam.OrderItemId + ", Message=" + result.Message);
                                    if (result.ResultCode == 110)
                                    {
                                        OrderSV.UnLockOrder(modelParam.commodityorderId);
                                    }
                                }
                                try
                                {
                                    // 同意退款
                                    CancelTheOrderDTO model = new CancelTheOrderDTO
                                    {
                                        OrderId = modelParam.Id,
                                        OrderItemId = modelParam.OrderItemId,
                                        State = 21,
                                        Message = "自动同意",
                                        UserId = Guid.Empty
                                    };
                                    var orderAfterSalesFacade = new CommodityOrderAfterSalesFacade
                                    {
                                        ContextDTO = AuthorizeHelper.InitAuthorizeInfo()
                                    };
                                    var cancelResult = orderAfterSalesFacade.CancelTheOrderAfterSales(model);
                                    if (cancelResult.ResultCode == 1)
                                    {
                                        LogHelper.Error("苏宁商品拒收自动退款失败，同意退款：CommodityOrderItemId=" + modelParam.OrderItemId + ", Message=" + result.Message);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception("苏宁商品拒收自动退款失败，同意退款：CommodityOrderItemId=" + modelParam.OrderItemId, ex);
                                }
                            }
                            else
                            {
                                LogHelper.Error("苏宁订单ID为：" + EntityQuery.OrderId + "已申请退款!");
                            }
                        }
                    }; break;
                case "4"://商品退货-售后
                    {
                        Jinher.AMP.BTP.TPS.Helper.OrderHelper
                        .ApproveOrderRefundAfterSales(EntityQuery.OrderId, EntityQuery.OrderItemId);
                    }; break;
            }
        }
        /// <summary>
        /// 获得苏宁物流包裹信息
        /// </summary>
        public static List<ThirdOrderItemExpress> suning_govbus_rejection_getsnpackage(Guid OrderId)
        {
            var ReturnList = new List<ThirdOrderItemExpress>();
            var _SNPackageTrace = SNPackageTrace.ObjectSet()
                .Where(w => w.CommodityOrderId == OrderId).ToList();
            SNOrderItem.ObjectSet().Where(w => w.OrderId == OrderId)
            .ToList().ForEach(Item =>
            {
                var QueryEntity = _SNPackageTrace.Where(w => w.CommodityOrderId == Item.OrderId)
                                .Where(w => w.OrderId == Item.CustomOrderId)
                                .Where(w => w.OrderItemId == Item.CustomOrderItemId)
                                .Where(w => w.SkuId == Item.CustomSkuId)
                                .FirstOrDefault();
                if (QueryEntity != null)
                {
                    ReturnList.Add(new ThirdOrderItemExpress
                    {
                        OrderItemId = Item.OrderItemId,
                        ExpressNo = QueryEntity.OrderItemId + "-" + QueryEntity.PackageId
                    });
                }
            });
            return ReturnList;
        }
        /// <summary>
        /// 手动执行根据苏宁物流状态更改易捷订单状态
        /// </summary>
        /// <param name="Status">苏宁物流状态</param>
        /// <param name="OrderId">易捷订单ID</param>
        /// <param name="OrderItemId">易捷订单行ID</param>
        public static void suning_govbus_rejection_handmode(int Status, Guid OrderId, Guid OrderItemId, ref string Message)
        {
            try
            {
                int ReStatus = Convert.ToInt32(Status);
                int OrderItemCount = SNOrderItem.ObjectSet().Where(w => w.OrderId == OrderId).Count();
                switch (Status)
                {
                    case 1://商品出库-已发货
                        {
                            if (SNOrderItem.ObjectSet().Where(w => w.OrderId == OrderId
                                        && w.ExpressStatus >= ReStatus).Count() == OrderItemCount)
                            {
                                var commodityorderfacade = new CommodityOrderFacade()
                                {
                                    ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo()
                                };
                                var result = commodityorderfacade.UpdateCommodityOrder(
                                    new Jinher.AMP.BTP.Deploy.CommodityOrderDTO()
                                    {
                                        Id = OrderId,
                                        State = 2,
                                        ShipmentsTime = DateTime.Now
                                    });
                                if (result.ResultCode != 0)
                                {
                                    LogHelper.Error("苏宁-发货失败:" + result.Message + ",入参:" + OrderId);
                                    AddSnlogs(string.Format("苏宁-发货失败:{0}；订单ID:{1}", result.Message, OrderId));
                                }
                            }
                        }; break;
                    case 2://商品妥投-已收货
                        {
                            if (SNOrderItem.ObjectSet().Where(w => w.OrderId == OrderId
                                        && w.ExpressStatus >= ReStatus).Count() == OrderItemCount)
                            {
                                var order = CommodityOrder.ObjectSet().FirstOrDefault(p => p.Id == OrderId);
                                if (order == null)
                                {
                                    LogHelper.Error("苏宁-妥投后自动确认收货:未找到订单,入参:" + OrderId);
                                    AddSnlogs("苏宁-妥投后自动确认收货:未找到订单,订单号:" + OrderId);
                                    return;
                                }
                                var facade = new Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade();
                                facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                                var result = facade.UpdateCommodityOrder(3, OrderId, order.SubId, order.AppId, order.Payment, string.Empty, string.Empty);
                                if (result.ResultCode != 0)
                                {
                                    LogHelper.Error("苏宁-妥投后自动确认收货失败:" + result.Message + ",入参:" + OrderId);
                                    AddSnlogs(string.Format("苏宁-妥投后自动确认收货失败:{0}；订单ID:{1}", result.Message, OrderId));
                                }
                            }
                        }; break;
                    case 3://商品拒收-拒收货
                        {
                            var orderItem = OrderItem.ObjectSet().Where(w => w.Id == OrderItemId).FirstOrDefault();
                            if (orderItem == null)
                            {
                                AddSnlogs("商品拒收-未找到订单项ID为：" + OrderItemId);
                                throw new Exception("未找到订单项ID为：" + OrderItemId);
                            }
                            else
                            {
                                // 检查是否已申请退款
                                if (!OrderRefundAfterSales.ObjectSet().Any(w => w.OrderId == OrderId && w.OrderItemId == OrderItemId))
                                {
                                    SubmitOrderRefundDTO modelParam = new SubmitOrderRefundDTO();
                                    modelParam.Id = modelParam.commodityorderId = OrderId;
                                    modelParam.RefundDesc = "苏宁商品拒收自动退款";
                                    var CurrPic = orderItem.RealPrice * orderItem.Number;
                                    if (CurrPic == 0)
                                    {
                                        CurrPic = (orderItem.DiscountPrice * orderItem.Number);
                                    }
                                    //已发货时默认退款金额=该商品实际售价-该商品承担的优惠券金额-该商品承担的改价商品金额-关税-易捷抵用券金额
                                    modelParam.RefundMoney = CurrPic.Value - (orderItem.CouponPrice ?? 0) - (orderItem.ChangeRealPrice ?? 0) - orderItem.Duty - (orderItem.YJCouponPrice ?? 0);
                                    if (modelParam.RefundMoney <= 0)
                                    {
                                        LogHelper.Error(@"苏宁商品拒收自动退款失败，退款金额为零, 提交申请：CommodityOrderItemId="
                                                        + OrderItemId + "CurrPic: " + CurrPic.Value + "CouponPrice: "
                                                        + (orderItem.CouponPrice ?? 0) + "ChangeRealPrice: " + (orderItem.ChangeRealPrice ?? 0)
                                                        + "Duty: " + orderItem.Duty + "YJCouponPrice: " + orderItem.YJCouponPrice
                                                        + "CommodityOrderId=" + modelParam.commodityorderId + "CommodityOrderItemId=" + modelParam.OrderItemId);
                                        AddSnlogs("苏宁商品拒收自动退款失败，退款金额为零；订单行ID为:" + OrderItemId);
                                    }
                                    modelParam.State = 3;
                                    modelParam.RefundReason = "其他";
                                    modelParam.RefundType = 0;// 仅退款
                                    modelParam.OrderRefundImgs = "";
                                    modelParam.OrderItemId = OrderItemId;
                                    var orderSV = new ISV.Facade.CommodityOrderAfterSalesFacade
                                    {
                                        ContextDTO = AuthorizeHelper.InitAuthorizeInfo()
                                    };
                                    var result = orderSV.SubmitOrderRefundAfterSales(modelParam);
                                    if (result.ResultCode != 0)
                                    {
                                        LogHelper.Error("苏宁商品拒收自动退款失败，提交申请：CommodityOrderItemId=" + modelParam.OrderItemId + ", Message=" + result.Message);
                                        AddSnlogs(string.Format("苏宁商品拒收自动退款失败:{0}；订单行ID:{1}", result.Message, OrderId));
                                        if (result.ResultCode == 110)
                                        {
                                            OrderSV.UnLockOrder(modelParam.commodityorderId);
                                        }
                                    }
                                    try
                                    {
                                        // 同意退款
                                        CancelTheOrderDTO model = new CancelTheOrderDTO
                                        {
                                            OrderId = modelParam.Id,
                                            OrderItemId = modelParam.OrderItemId,
                                            State = 21,
                                            Message = "自动同意",
                                            UserId = Guid.Empty
                                        };
                                        var orderAfterSalesFacade = new CommodityOrderAfterSalesFacade
                                        {
                                            ContextDTO = AuthorizeHelper.InitAuthorizeInfo()
                                        };
                                        var cancelResult = orderAfterSalesFacade.CancelTheOrderAfterSales(model);
                                        if (cancelResult.ResultCode == 1)
                                        {
                                            LogHelper.Error("苏宁商品拒收自动退款失败，同意退款：CommodityOrderItemId=" + modelParam.OrderItemId + ", Message=" + result.Message);
                                            AddSnlogs(string.Format("苏宁商品拒收自动退款失败:{0}；订单行ID:{1}", result.Message, OrderItemId));
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        AddSnlogs(string.Format("苏宁商品拒收自动退款失败:{0}；订单行ID:{1}", ex.Message, OrderItemId));
                                        throw new Exception("苏宁商品拒收自动退款失败，同意退款：CommodityOrderItemId=" + modelParam.OrderItemId, ex);
                                    }
                                }
                                else
                                {
                                    LogHelper.Error("苏宁订单ID为：" + OrderId + "已申请退款!");
                                    AddSnlogs("苏宁订单ID为：" + OrderId + "已申请退款!");
                                }
                            }
                        }; break;
                    case 4://商品退货-售后
                        {
                            Jinher.AMP.BTP.TPS.Helper.OrderHelper.ApproveOrderRefundAfterSales(OrderId, OrderItemId);
                        }; break;
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                LogHelper.Error("手动执行根据苏宁物流状态更改易捷订单状态Exception：" + ex);
                AddSnlogs(string.Format("手动执行根据苏宁物流状态更改易捷订单状态Exception:{0}；订单ID:{1}", ex.Message, OrderId));
            }
            finally
            {
                LogHelper.Error("手动执行根据苏宁物流状态更改易捷订单状态Successful：Status=" + Status + ",OrderId= " + OrderId + ",OrderItemId=" + OrderItemId);
            }
        }

        /// <summary>
        /// 按时间手动执行根据苏宁物流状态更改易捷订单状态
        /// </summary>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        public static void suning_govbus_rejection_batch(DateTime beginDate, DateTime endDate)
        {
            var list = (from s in SNOrderItem.ObjectSet()
                        join c in CommodityOrder.ObjectSet() on s.OrderId equals c.Id
                        where s.SubTime > beginDate && s.SubTime < endDate && s.ExpressStatus > 0 && c.State == 1
                        select new { OrderId = c.Id, s.ExpressStatus }).ToList();
            list.ForEach(p =>
            {
                var info = string.Empty;
                if (p.ExpressStatus == 1)
                {
                    suning_govbus_rejection_handmode(1, p.OrderId, Guid.Empty, ref info);
                }
                else if (p.ExpressStatus == 2)
                {
                    suning_govbus_rejection_handmode(1, p.OrderId, Guid.Empty, ref info);
                    suning_govbus_rejection_handmode(2, p.OrderId, Guid.Empty, ref info);
                }
            });
        }

        #endregion

        #region 刘核心
        /// <summary>
        /// suning.govbus.category.get 获取商品目录接口 
        /// </summary>
        /// <returns></returns>
        public static List<string> GetCategory()
        {
            List<string> list = new List<string>();
            try
            {
                var request = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.CategoryGetRequest();
                var result = SuningClient.Execute(request);
                LogHelper.Info(string.Format("SuningSV.GetCategory 批量获取苏宁Category,入参:{0},出参:{1}", SerializationHelper.JsonSerialize(request), SerializationHelper.JsonSerialize(result.respError)));
                if (result.resultInfo != null)
                {
                    foreach (var item in result.resultInfo)
                    {
                        list.Add(item.categoryId);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("SuningSV.GetCategory 获取Category 异常", ex);
            }
            return list;
        }
        /// <summary>
        /// suning.govbus.prodpool.query 获取商品池 
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public static List<string> GetProdPool(string categoryId)
        {
            List<string> list = new List<string>();
            try
            {
                var request = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.ProdPoolQueryRequest();
                request.categoryId = categoryId;
                var result = SuningClient.Execute(request);
                LogHelper.Info(string.Format("SuningSV.GetProdPool 获取苏宁商品池,入参:{0},出参:{1}", SerializationHelper.JsonSerialize(request), SerializationHelper.JsonSerialize(result.respError)));
                if (result.resultInfo != null)
                {
                    foreach (var item in result.resultInfo)
                    {
                        list.Add(item.skuId);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("SuningSV.GetProdPool 获取商品池 异常", ex);
            }
            return list;
        }
        /// <summary>
        /// suning.govbus.proddetail.get 获取商品详情接口
        /// </summary>
        /// <param name="skuId"></param>
        /// <returns></returns>
        public static SNComDetailDto GetSNDetail(string skuId)
        {
            SNComDetailDto SnDetail = new SNComDetailDto();
            try
            {

                var request = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.ProdDetailGetRequest();
                request.skuId = skuId;
                var result = SuningClient.Execute(request);
                LogHelper.Info(string.Format("SuningSV.GetSNDetail 获取苏宁商品详情,入参:{0},出参:{1}", SerializationHelper.JsonSerialize(request), SerializationHelper.JsonSerialize(result)));
                if (!string.IsNullOrEmpty(result.name))
                {
                    SnDetail.brand = result.brand;
                    SnDetail.category = result.category;
                    SnDetail.image = result.image;
                    SnDetail.introduction = result.introduction;
                    SnDetail.packlisting = result.packlisting;
                    SnDetail.model = result.model;
                    SnDetail.name = result.name;
                    SnDetail.productArea = result.productArea;
                    SnDetail.saleUnit = result.saleUnit;
                    SnDetail.skuId = result.skuId;
                    SnDetail.state = result.state;
                    SnDetail.upc = result.upc;
                    SnDetail.weight = result.weight;
                    List<ProdParams> pros = new List<ProdParams>();
                    List<Param> pa = new List<Param>();
                    if (result.prodParams != null)
                    {
                        foreach (var item in result.prodParams)
                        {
                            ProdParams pro = new ProdParams();
                            pro.desc = item.desc;
                            pro.param = new List<Param>();
                            if (item.param != null)
                            {
                                foreach (var model in item.param)
                                {
                                    Param p = new Param();
                                    p.coreFlag = model.coreFlag;
                                    p.snparameterCode = model.snparameterCode;
                                    p.snparameterdesc = model.snparameterdesc;
                                    p.snparametersCode = model.snparametersCode;
                                    p.snparametersDesc = model.snparametersDesc;
                                    p.snparameterSequence = model.snparameterSequence;
                                    p.snparameterVal = model.snparameterVal;
                                    p.snsequence = model.snsequence;
                                    pro.param.Add(p);
                                }
                            }
                            pros.Add(pro);
                        }
                        SnDetail.prodParams = pros;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("SuningSV.GetSNDetail 获取苏宁商品详情 异常", ex);
            }
            return SnDetail;
        }
        /// <summary>
        /// suning.govbus.prodimage.query 获取商品图片接口 
        /// </summary>
        /// <param name="skuId"></param>
        /// <returns></returns>
        public static List<SNComPicturesDto> GetComPictures(List<string> skuId)
        {
            List<SNComPicturesDto> list = new List<SNComPicturesDto>();
            try
            {
                var request = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.ProdImageQueryRequest();
                request.skuIds = new List<snsdk.suning_api_sdk.Models.CustomGovbusModel.ProdImageQuerySkuIdsReq>();
                foreach (var item in skuId)
                {
                    var sku = new snsdk.suning_api_sdk.Models.CustomGovbusModel.ProdImageQuerySkuIdsReq();
                    sku.skuId = item;
                    request.skuIds.Add(sku);
                }
                var result = SuningClient.Execute(request);
                LogHelper.Info(string.Format("SuningSV.GetComPictures 批量获取苏宁图片,入参:{0},出参:{1}", SerializationHelper.JsonSerialize(request), SerializationHelper.JsonSerialize(result)));
                if (result.resultInfo != null)
                {
                    foreach (var item in result.resultInfo)
                    {
                        if (item.urls != null)
                        {
                            foreach (var model in item.urls)
                            {
                                SNComPicturesDto picture = new SNComPicturesDto();
                                picture.skuId = item.skuId;
                                picture.path = model.path;
                                picture.primary = model.primary;
                                list.Add(picture);
                            }
                        }
                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("SuningSV.GetComPictures 批量获取苏宁图片异常", ex);
            }
            return list;
        }
        /// <summary>
        /// suning.govbus.price.query 批量查询商品价格接口
        /// </summary>
        /// <param name="skuId"></param>
        /// <returns></returns>
        public static List<SNPriceDto> GetPrice(List<string> skuId)
        {
            List<SNPriceDto> list = new List<SNPriceDto>();
            try
            {
                var request = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.PriceQueryRequest();
                //全国统一价 传取价的城市北京
                request.cityId = CustomConfig.Suning_CityId;
                request.skus = new List<snsdk.suning_api_sdk.Models.CustomGovbusModel.PriceQuerySkusReq>();
                foreach (var item in skuId)
                {
                    var sku = new snsdk.suning_api_sdk.Models.CustomGovbusModel.PriceQuerySkusReq();
                    sku.skuId = item;
                    request.skus.Add(sku);
                }
                var result = SuningClient.Execute(request);
                LogHelper.Info(string.Format("SuningSV.GetPrice 批量获取苏宁价格,入参:{0},出参:{1}", SerializationHelper.JsonSerialize(request), SerializationHelper.JsonSerialize(result)));
                if (result.skus != null)
                {
                    foreach (var item in result.skus)
                    {
                        SNPriceDto price = new SNPriceDto();
                        price.skuId = item.skuId;
                        price.price = item.price;
                        price.snPrice = item.snPrice;
                        price.discountRate = item.discountRate;
                        price.tax = item.tax;
                        price.taxprice = item.taxprice;
                        price.nakedprice = item.nakedprice;
                        list.Add(price);
                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("SuningSV.GetPrice 批量获取苏宁价格异常,入参:" + SerializationHelper.JsonSerialize(skuId), ex);
            }

            return list;
        }
        /// <summary>
        /// suning.govbus.prodextend.get 查询商品扩展信息接口 
        /// 通过此接口可获取商品是否支持开增票及是否支持无理由退货
        /// 1、商品是否支持开增票，可通过接口“suning.govbus.prodextend.get”获取；
        /// 2、商品是否支持无理由退货，可通过接口“suning.govbus.prodextend.get”获取；
        /// </summary>
        /// <param name="skuId"></param>
        /// <returns></returns>
        public static List<SNComExtendDto> GetComExtend(List<SNPriceDto> prices)
        {
            List<SNComExtendDto> list = new List<SNComExtendDto>();
            try
            {
                var request = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.ProdextendGetRequest();
                request.skus = new List<snsdk.suning_api_sdk.Models.CustomGovbusModel.ProdextendGetSkusReq>();
                foreach (var item in prices)
                {
                    var price = new snsdk.suning_api_sdk.Models.CustomGovbusModel.ProdextendGetSkusReq();
                    price.price = item.price;
                    price.skuId = item.skuId;
                    request.skus.Add(price);
                }
                var result = SuningClient.Execute(request);
                LogHelper.Info(string.Format("SuningSV.GetComExtend 批量获取苏宁商品是否支持开增票及是否支持无理由退货,入参:{0},出参:{1}", SerializationHelper.JsonSerialize(request), SerializationHelper.JsonSerialize(result)));
                if (result.resultInfo != null)
                {
                    foreach (var item in result.resultInfo)
                    {
                        var model = new SNComExtendDto();
                        model.increaseTicket = item.increaseTicket;
                        model.noReasonLimit = item.noReasonLimit;
                        model.noReasonTip = item.noReasonTip;
                        model.returnGoods = item.returnGoods;
                        model.skuId = item.skuId;
                        list.Add(model);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("SuningSV.GetComExtend 批量获取苏宁商品是否支持开增票及是否支持无理由退货异常", ex);
            }
            return list;
        }
        /// <summary>
        /// suning.govbus.batchprodsalestatus.get 商品上下架状态查询接口
        /// </summary>
        /// <param name="skus"></param>
        /// <returns></returns>
        public static List<SNSkuStateDto> GetSkuState(List<string> skus)
        {
            List<SNSkuStateDto> list = new List<SNSkuStateDto>();
            try
            {
                var request = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.BatchProdSaleStatusGetRequest();
                request.skuIds = new List<snsdk.suning_api_sdk.Models.CustomGovbusModel.BatchProdSaleStatusGetSkuIdsReq>();
                foreach (var item in skus)
                {
                    var state = new snsdk.suning_api_sdk.Models.CustomGovbusModel.BatchProdSaleStatusGetSkuIdsReq();
                    state.skuId = item;
                    request.skuIds.Add(state);
                }
                var result = SuningClient.Execute(request);
                LogHelper.Info(string.Format("SuningSV.GetSkuState 批量获取苏宁商品上下架状态,入参:{0},出参:{1}", SerializationHelper.JsonSerialize(request), SerializationHelper.JsonSerialize(result)));
                if (result.onShelvesList != null)
                {
                    foreach (var item in result.onShelvesList)
                    {
                        var skuState = new SNSkuStateDto();
                        skuState.skuId = item.skuId;
                        skuState.state = item.state;
                        list.Add(skuState);
                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("SuningSV.GetSkuState 批量获取苏宁商品上下架状态异常", ex);
                AddSnlogs("批量获取苏宁商品上下架状态异常" + ex.Message);
            }
            return list;
        }
        /// <summary>
        /// suning.govbus.pricemessage.query 价格变动消息查询接口 
        /// </summary>
        /// <returns></returns>
        public static List<SNPriceMessageDto> GetPriceMessage()
        {
            List<SNPriceMessageDto> list = new List<SNPriceMessageDto>();
            try
            {
                var request = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.PricemessageQueryRequest();
                var result = SuningClient.Execute(request);
                //发送数据到mq
                var json = SerializationHelper.JsonSerialize(new { Request = request, Response = result });
                RabbitMqHelper.Send(RabbitMqRoutingKey.SnPriceMsgReceived, RabbitMqExchange.Commodity, json);
                LogHelper.Info(string.Format("SuningSV.GetPriceMessage 苏宁商品价格变动消息查询,入参:{0},出参:{1}", SerializationHelper.JsonSerialize(request), SerializationHelper.JsonSerialize(result)));
                if (result.result != null)
                {
                    foreach (var item in result.result)
                    {
                        var priceMsg = new SNPriceMessageDto();
                        priceMsg.cmmdtyCode = item.cmmdtyCode;
                        priceMsg.cityId = item.cityId;
                        priceMsg.time = Convert.ToDateTime(item.time);
                        list.Add(priceMsg);
                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("SuningSV.GetPriceMessage 苏宁商品价格变动消息查询异常", ex);
            }
            return list;
        }
        /// <summary>
        /// suning.govbus.inventory.get 单个商品的精准库存查询接口
        /// </summary>
        /// <returns></returns>
        public static bool GetSNInventory(string cityId, string countryId, string num, string skuId)
        {
            bool flag = false;
            try
            {
                var request = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.InventoryGetRequest();
                request.cityId = cityId;
                request.countyId = countryId;
                request.skuIds = new List<snsdk.suning_api_sdk.Models.CustomGovbusModel.InventoryGetSkuIdsReq>();
                var inven = new snsdk.suning_api_sdk.Models.CustomGovbusModel.InventoryGetSkuIdsReq();
                inven.num = num;
                inven.skuId = skuId;
                request.skuIds.Add(inven);
                var result = SuningClient.Execute(request);
                LogHelper.Info(string.Format("SuningSV.GetSNInventory 苏宁易购单个商品库存查询,入参:{0},出参:{1}", SerializationHelper.JsonSerialize(request), SerializationHelper.JsonSerialize(result)));
                if (result.resultInfo != null)
                {
                    if (result.resultInfo.state == "00")
                    {
                        flag = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("SuningSV.GetSNInventory 苏宁易购单个商品库存查询异常", ex);
                flag = false;
            }
            return flag;
        }
        /// <summary>
        /// suning.govbus.mprodstock.query 多商品的库存批量查询接口
        /// </summary>
        /// <returns></returns>
        public static bool GetSNMprodstock(string cityId, string skuId)
        {
            bool flag = false;
            try
            {
                var request = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.MpStockQueryRequest();
                request.cityId = cityId;
                request.skuIds = new List<snsdk.suning_api_sdk.Models.CustomGovbusModel.MpStockQuerySkuIdsReq>();
                var inven = new snsdk.suning_api_sdk.Models.CustomGovbusModel.MpStockQuerySkuIdsReq();
                inven.skuId = skuId;
                request.skuIds.Add(inven);
                var result = SuningClient.Execute(request);
                LogHelper.Info(string.Format("SuningSV.GetStock 苏宁易购商品批量库存查询,入参:{0},出参:{1}", SerializationHelper.JsonSerialize(request), SerializationHelper.JsonSerialize(result)));
                if (result.resultInfo != null)
                {
                    foreach (var item in result.resultInfo)
                    {
                        if (item.state == "0")
                        {
                            flag = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("SuningSV.GetStock 苏宁易购商品批量库存查询异常", ex);
                flag = false;
            }
            return flag;
        }
        #endregion

        #region 高兴
        /// <summary>
        /// 部分商品退货
        /// http://open.suning.com/ospos/apipage/toApiMethodDetailMenu.do?interCode=suning.govbus.returnpartorder.add
        /// </summary>
        /// <param name="reqDto">实体数据</param>
        /// <returns></returns>
        public static ResultDTO ReturnPartOrder(SNReturnPartOrderDTO reqDto)
        {
            //****测试注释
            //return new ResultDTO { isSuccess = true };
            ResultDTO ret = new ResultDTO();

            try
            {
                #region 将系统中自有的实体转换为苏宁自己的实体传入
                List<snsdk.suning_api_sdk.Models.CustomGovbusModel.ReturnpartorderAddSkusReq> skusReqList = new List<snsdk.suning_api_sdk.Models.CustomGovbusModel.ReturnpartorderAddSkusReq>();

                reqDto.SkusList.ForEach(p =>
                {
                    skusReqList.Add(new snsdk.suning_api_sdk.Models.CustomGovbusModel.ReturnpartorderAddSkusReq() { skuId = p.SkuId, num = p.Num });
                });

                ReturnpartorderAddRequest request = new ReturnpartorderAddRequest()
                {
                    orderId = reqDto.OrderId,
                    skus = skusReqList
                };
                #endregion


                ReturnpartorderAddResponse response = SuningClient.Execute(request);
                #region 构造返回实体
                if (response != null)
                {
                    if (response.respError != null)
                    {
                        string Message = response.respError.error_code + "===" + response.respError.error_msg;
                        LogHelper.Error(string.Format("苏宁售后[suning.govbus.returnpartorder.add]：{0}", Message));
                        AddSnlogs(string.Format("苏宁售后接口调用错误：{0}；订单ID：{1}", Message, reqDto.OrderId));
                    }
                    LogHelper.Info(string.Format("【苏宁-售后】SuningSV.ReturnPartOrder部分商品退货,入参:{0},出参:{1}", SerializationHelper.JsonSerialize(request), response.ToJson()));
                    bool ty = false;

                    if (response.infoList.Any())
                    {
                        var dto = response.infoList.FirstOrDefault();
                        if (dto.status.Equals("1"))
                        {
                            return new ResultDTO { isSuccess = true };
                        }
                        else
                        {
                            return new ResultDTO { isSuccess = false, ResultCode = 1, Message = dto.unableReason };
                        }
                    }


                    //if (response.isSuccess.Equals("Y"))
                    //{
                    //    return new ResultDTO { isSuccess = true };
                    //}
                    //else
                    //{
                    //    return new ResultDTO { isSuccess = false, ResultCode = 1, Message = "退货失败" };
                    //}

                    //List<SNReturnPartOrderReturnListDTO> listRet = new List<SNReturnPartOrderReturnListDTO>();

                    //response.infoList.ForEach(p =>
                    //{
                    //    listRet.Add(new SNReturnPartOrderReturnListDTO() { SkuId = p.skuId, Status = p.status, UnableReason = p.unableReason });
                    //});
                    //ret = new SNReturnPartOrderReturnDTO() { OrderId = response.orderId, InfoList = listRet, IsSuccess = response.isSuccess.Equals("Y") ? true : false };
                }
                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.Error("SuningSV.ReturnPartOrder 【苏宁-售后】部分商品退货", ex);
                AddSnlogs(string.Format("【苏宁-售后】部分商品退货异常:{0}；订单ID:{1}", ex.Message, reqDto.OrderId));
            }
            return ret;
        }


        /// <summary>
        /// 整单退货接口申请
        /// http://open.suning.com/ospos/apipage/toApiMethodDetailMenu.do?interCode=suning.govbus.applyrejected.add
        /// </summary>
        /// <param name="reqDto"></param>
        /// <returns></returns>
        public static SNApplyRejectedReturnDTO ApplyRejected(SNApplyRejectedDTO reqDto)
        {
            SNApplyRejectedReturnDTO ret = null;
            try
            {

                #region 将系统中自有的实体转换为苏宁自己的实体传入
                List<snsdk.suning_api_sdk.Models.CustomGovbusModel.ApplyRejectedAddSkusReq> listApplyRejList = new List<snsdk.suning_api_sdk.Models.CustomGovbusModel.ApplyRejectedAddSkusReq>();

                reqDto.SkusList.ForEach(p =>
                {
                    listApplyRejList.Add(new snsdk.suning_api_sdk.Models.CustomGovbusModel.ApplyRejectedAddSkusReq() { skuId = p.SkuId });
                });
                ApplyRejectedAddRequest request = new ApplyRejectedAddRequest()
                {
                    orderId = reqDto.OrderId,
                    skus = listApplyRejList
                };

                #endregion

                ApplyRejectedAddResponse response = SuningClient.Execute(request);

                #region 构造返回实体
                if (response != null)
                {
                    if (response.respError != null)
                    {
                        string Message = response.respError.error_code + "===" + response.respError.error_msg;
                        LogHelper.Error(string.Format("苏宁售后[suning.govbus.applyrejected.add]：{0}", Message));
                        AddSnlogs(string.Format("苏宁售后接口调用错误：{0}，订单ID：{1}", Message, reqDto.OrderId));
                    }
                    LogHelper.Info(string.Format("【苏宁-售后】SuningSV.ApplyRejected整单退货接口申请,入参:{0},出参:{1}", SerializationHelper.JsonSerialize(request), response.ToJson()));
                    if (response.infoList.Any())
                    {
                        List<SNApplyRejectedReturnListDTO> listRet = new List<SNApplyRejectedReturnListDTO>();

                        response.infoList.ForEach(p =>
                        {
                            listRet.Add(new SNApplyRejectedReturnListDTO() { SkuId = p.skuId, Status = p.status, UnableReason = p.unableReason });
                        });

                        ret = new SNApplyRejectedReturnDTO() { OrderId = response.orderId, InfoList = listRet };
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.Error("SuningSV.ApplyRejected 【苏宁-售后】整单退货接口申请", ex);
                AddSnlogs(string.Format("【苏宁 - 售后】整单退货接口申请异常:{0}；订单ID:{1}", ex.Message, reqDto.OrderId));
            }
            return ret;
        }

        /// <summary>
        /// 添加操作日志
        /// </summary>
        /// <param name="msg">日志消息</param>
        public static void AddSnlogs(string msg)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var LogInfo = new Jinher.AMP.BTP.Deploy.JdlogsDTO
                    {
                        Id = Guid.NewGuid(),
                        Content = msg,
                        SubTime = DateTime.Now,
                        ThirdECommerceType = (int)ThirdECommerceTypeEnum.SuNingYiGou
                    };
                    new JdlogsFacade().SaveJdlogs(LogInfo);
                }
                catch (Exception ex)
                {
                    LogHelper.Error("记录日志错误-AddSonlogs", ex);
                }
            });

        }


        /// <summary>
        /// 判断商品是否厂送
        /// https://open.suning.com/ospos/apipage/toApiMethodDetailMenu.do?interCode=suning.govbus.judgefacproduct.get
        /// </summary>
        /// <param name="factoryDeliverDto"></param>
        /// <returns></returns>
        public static SNFactoryDeliveryReturnDTO JudgeIsFactoryDelivery(SNFactoryDeliveryDTO factoryDeliverDto)
        {
            SNFactoryDeliveryReturnDTO ret = null;

            try
            {


                #region 将系统中自有的实体转换为苏宁自己的实体传入
                List<snsdk.suning_api_sdk.Models.CustomGovbusModel.JudgefacproductGetSkuIdsReq> listApplyRejList = new List<snsdk.suning_api_sdk.Models.CustomGovbusModel.JudgefacproductGetSkuIdsReq>();

                factoryDeliverDto.SkuIds.ForEach(p =>
                {
                    listApplyRejList.Add(new snsdk.suning_api_sdk.Models.CustomGovbusModel.JudgefacproductGetSkuIdsReq() { skuId = p.SkuId });
                });

                JudgefacproductGetRequest request = new JudgefacproductGetRequest()
                {
                    cityId = factoryDeliverDto.CityId,
                    skuIds = listApplyRejList
                };
                #endregion
                JudgefacproductGetResponse response = SuningClient.Execute(request);

                #region 构造返回实体

                if (response != null)
                {
                    if (response.respError != null)
                    {
                        string Message = response.respError.error_code + "===" + response.respError.error_msg;
                        LogHelper.Error(string.Format("苏宁售后[suning.govbus.judgefacproduct.get]：{0}", Message));
                    }
                    LogHelper.Info(string.Format("【苏宁-售后】SuningSV.JudgeIsFactoryDelivery判断商品是否厂送,入参:{0},出参:{1}", SerializationHelper.JsonSerialize(request), response.ToJson()));

                    if (response.results.Any())
                    {
                        List<SNFactoryDeliveryReturnListDTO> listRet = new List<SNFactoryDeliveryReturnListDTO>();

                        response.results.ForEach(p =>
                        {
                            listRet.Add(new SNFactoryDeliveryReturnListDTO() { IsFactorySend = p.isFactorySend.Equals("01") ? true : false, SkuId = p.skuId });
                        });

                        ret = new SNFactoryDeliveryReturnDTO() { IsSuccess = true ? true : false, ResultsList = listRet };
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.Error("SuningSV.JudgeIsFactoryDelivery 【苏宁-售后】判断商品是否厂送", ex);
            }
            return ret;
        }
        /// <summary>
        /// 获取订单状态
        /// </summary>
        /// <param name="snOrderId"></param>
        /// <returns></returns>
        public static SNOrderStatusDTO SNGetOrderStatus(string snOrderId)
        {

            SNOrderStatusDTO orderstatus = null;
            try
            {
                var request = new OrderStatusGetRequest()
                {
                    orderId = snOrderId
                };
                OrderStatusGetResponse response = SuningClient.Execute(request);
                if (response != null)
                {
                    if (response.respError != null)
                    {
                        string Message = response.respError.error_code + "===" + response.respError.error_msg;
                        LogHelper.Error(string.Format("苏宁售后[suning.govbus.orderstatus.get]：{0}", Message));
                    }

                    LogHelper.Info(string.Format("【苏宁-售后】SuningSV.SNGetOrderStatus获取订单状态,入参:{0},出参:{1}", SerializationHelper.JsonSerialize(request), response.ToJson()));
                    if (response.orderItemInfoList.Any())
                    {
                        List<SNOrderItemInfo> snOrderItemInfoList = new List<SNOrderItemInfo>();
                        response.orderItemInfoList.ForEach(p =>
                        {
                            snOrderItemInfoList.Add(new SNOrderItemInfo()
                            {
                                OrderItemId = p.orderItemId,
                                SkuId = p.skuId,
                                StatusName = p.statusName
                            });
                        });
                        orderstatus = new SNOrderStatusDTO() { OrderId = response.orderId, OrderItemInfoList = snOrderItemInfoList, OrderStatus = response.orderStatus };
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("SuningSV.SNGetOrderStatus 【苏宁-售后】获取订单状态", ex);
            }
            return orderstatus;
        }

        /// <summary>
        /// 判断商品是否支持退款
        /// </summary>
        /// <param name="orderServerDto"></param>
        public static List<SNGetOrderServiceReturnDTO> SNJudgeOrderServiceType(List<SNGetOrderServiceDTO> orderServerDto)
        {
            List<SNGetOrderServiceReturnDTO> retDto = new List<SNGetOrderServiceReturnDTO>();

            try
            {
                List<snsdk.suning_api_sdk.Models.CustomGovbusModel.ProdextendGetSkusReq> list = new List<snsdk.suning_api_sdk.Models.CustomGovbusModel.ProdextendGetSkusReq>();
                orderServerDto.ForEach(p =>
                {
                    list.Add(new snsdk.suning_api_sdk.Models.CustomGovbusModel.ProdextendGetSkusReq()
                    {
                        price = double.Parse(p.Price).ToString("0.00"),
                        skuId = p.SkuId
                    });
                });

                var request = new ProdextendGetRequest()
                {
                    skus = list
                };
                var response = SuningClient.Execute(request);
                if (response != null)
                {
                    if (response.respError != null)
                    {
                        string Message = response.respError.error_code + "===" + response.respError.error_msg;
                        LogHelper.Error(string.Format("苏宁售后[suning.govbus.prodextend.get ]：{0}", Message));
                    }

                    LogHelper.Info(string.Format("【苏宁-售后】SuningSV.SNJudgeOrderServiceType 判断商品是否支持退款,入参:{0},出参:{1}", SerializationHelper.JsonSerialize(request), response.ToJson()));
                    if (response.resultInfo.Any())
                    {
                        foreach (var p in response.resultInfo)
                        {


                            SNGetOrderServiceReturnDTO dt = new SNGetOrderServiceReturnDTO();

                            dt.ReturnGoods = p.returnGoods;
                            dt.SkuId = p.skuId;
                            dt.IncreaseTicket = p.increaseTicket;
                            dt.NoReasonTip = p.noReasonTip;
                            dt.NoReasonLimit = p.noReasonLimit;

                            retDto.Add(dt);
                        };

                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("SuningSV.SNJudgeOrderServiceType 【苏宁-售后】判断商品是否支持退款", ex);
            }
            return retDto;
        }

        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="snOrderId"></param>
        /// <returns></returns>
        public static SNAfterOrderDetailDTO SNGetOrderDetailById(string snOrderId)
        {
            #region //****测试注释   测试写死默认返回值
            //List<SNAfterOrderDetailListDTO> orderDetail1 = new List<SNAfterOrderDetailListDTO>();
            //SNAfterOrderDetailDTO retDto1 = new SNAfterOrderDetailDTO();

            //orderDetail1.Add(new SNAfterOrderDetailListDTO()
            //{
            //    // 商品的品牌名称
            //    BrandName = "商品的品牌名称",

            //    // 商品编码
            //    CommdtyCode = "商品编码",
            //    // 	商品名称
            //    CommdtyName = "商品名称",
            //    // 希望送达时间（yyyy-MM-dd HH:mm:ss）
            //    HopeArriveTime = "2018-08-02 12:12:12",
            //    // 苏宁订单行号
            //    OrderItemId = "1111111111",
            //    // 商品总金额=商品数量*商品单价（含运费分摊）
            //    SkuAmt = "1",
            //    // 商品的购买数量
            //    SkuNum = "1",
            //    // 	商品单价
            //    UnitPrice = "1"
            //});

            //retDto1 = new SNAfterOrderDetailDTO()
            //{

            //    AccountName = "下单企业账号的用户名",

            //    CompanyName = "采购账号的企业名称",

            //    CreateTime = "2016-07-18 18:00:00",

            //    OrderAmt = "12",

            //    OrderId = "1111111111",

            //    OrderItemList = orderDetail1,

            //    ReceiverAddress = "想不到送货地址呀",

            //    ReceiverTel = "1231231231"
            //};
            //return retDto1;
            #endregion



            SNAfterOrderDetailDTO retDto = null;
            try
            {
                var request = new OrderDetailGetRequest()
                {
                    orderId = snOrderId
                };

                var response = SuningClient.Execute(request);
                if (response != null)
                {
                    if (response.respError != null)
                    {
                        string Message = response.respError.error_code + "===" + response.respError.error_msg;
                        LogHelper.Error(string.Format("苏宁售后[suning.govbus.orderdetail.get]：{0}", Message));
                    }
                    LogHelper.Info(string.Format("【苏宁-售后】SuningSV.SNGetOrderDetailById 获取订单详情,入参:{0},出参:{1}", SerializationHelper.JsonSerialize(request), response.ToJson()));
                    if (response.orderItemList.Any())
                    {

                        List<SNAfterOrderDetailListDTO> orderDetail = new List<SNAfterOrderDetailListDTO>();

                        response.orderItemList.ForEach(p =>
                        {
                            orderDetail.Add(new SNAfterOrderDetailListDTO()
                            {
                                // 商品的品牌名称
                                BrandName = p.brandName,

                                // 商品编码
                                CommdtyCode = p.commdtyCode,
                                // 	商品名称
                                CommdtyName = p.commdtyName,
                                // 希望送达时间（yyyy-MM-dd HH:mm:ss）
                                HopeArriveTime = p.hopeArriveTime,
                                // 苏宁订单行号
                                OrderItemId = p.orderItemId,
                                // 商品总金额=商品数量*商品单价（含运费分摊）
                                SkuAmt = p.skuAmt,
                                // 商品的购买数量
                                SkuNum = p.skuNum,
                                // 	商品单价
                                UnitPrice = p.unitPrice
                            });
                        });

                        retDto = new SNAfterOrderDetailDTO()
                        {

                            AccountName = response.accountName,

                            CompanyName = response.companyName,

                            CreateTime = response.createTime,

                            OrderAmt = response.orderAmt,

                            OrderId = response.orderId,

                            OrderItemList = orderDetail,

                            ReceiverAddress = response.receiverAddress,

                            ReceiverTel = response.receiverTel
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("SuningSV.SNGetOrderDetailById 【苏宁-售后】获取订单详情", ex);
            }
            return retDto;
        }


        /// <summary>
        /// 获取物流信息
        /// </summary>
        /// <param name="orderLogist"></param>
        /// <returns></returns>
        public static SNOrderLogistOutPutDTO SNGetOrderLogist(SNOrderLogistInputDTO orderLogist)
        {
            SNOrderLogistOutPutDTO retDto = new SNOrderLogistOutPutDTO();
            try
            {
                var request = new OrderLogistGetRequest()
                {
                    orderId = orderLogist.OrderId,
                    orderItemId = orderLogist.OrderItemId,
                    skuId = orderLogist.SkuId
                };
                var response = SuningClient.Execute(request);
                if (response != null)
                {
                    if (response.respError != null)
                    {
                        string Message = response.respError.error_code + "===" + response.respError.error_msg;
                        LogHelper.Error(string.Format("苏宁售后[suning.govbus.orderlogist.get ]：{0}", Message));
                    }
                    LogHelper.Info(string.Format("【苏宁-售后】SuningSV.SNGetOrderLogist 获取物流信息,入参:{0},出参:{1}", SerializationHelper.JsonSerialize(request), response.ToJson()));
                    if (response.orderLogisticStatus.Any())
                    {
                        List<SNOrderLogistStatusResDTO> OrderLogisticStatusList = new List<SNOrderLogistStatusResDTO>();

                        foreach (var p in response.orderLogisticStatus)
                        {
                            SNOrderLogistStatusResDTO dto = new SNOrderLogistStatusResDTO();

                            dto.OperateState = p.operateState;
                            dto.OperateTime = p.operateTime == "" ? "" : DateTime.ParseExact(p.operateTime, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture).ToString();
                            OrderLogisticStatusList.Add(dto);
                        }

                        retDto = new SNOrderLogistOutPutDTO()
                        {
                            OrderId = response.orderId,
                            OrderItemId = response.orderItemId,
                            SkuId = response.skuId,
                            ShippingTime = string.IsNullOrWhiteSpace(response.shippingTime) ? "" : DateTime.ParseExact(response.shippingTime, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture).ToString(),
                            ReceiveTime = string.IsNullOrWhiteSpace(response.receiveTime) ? "" : DateTime.ParseExact(response.receiveTime, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture).ToString(),
                            OrderLogisticStatus = OrderLogisticStatusList
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("SuningSV.SNGetOrderLogist 【苏宁-售后】获取物流信息", ex);
            }
            return retDto;

        }
        #endregion

    }
}
