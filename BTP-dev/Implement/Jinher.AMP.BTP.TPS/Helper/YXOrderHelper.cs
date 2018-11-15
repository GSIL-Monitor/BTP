using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.BE.BELogic;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO.YX;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;
using Newtonsoft.Json;

namespace Jinher.AMP.BTP.TPS.Helper
{
    /// <summary>
    /// 网易严选订单帮助类
    /// </summary>
    public static class YXOrderHelper
    {
        private static string GetPaymentName(int payment)
        {
            switch (payment)
            {
                case 1003:
                    return "支付宝";
                case 1005:
                    return "银联";
                case 1006:
                    return "微信";
                case 2001:
                    return "金采";
                default:
                    return "其他";
            }
        }

        /// <summary>
        /// 更新物流轨迹信息 zbs:注意这里的expressNo参数有可能是主包裹也可能是子包裹，也即子包裹和主包裹的物流轨迹信息都存一个表。还要返回信息供页面显示。
        /// </summary>
        private static DeliveryInfo SaveYXExpressTrace(Guid orderId, string orderCode, string packageId, string expressNo, string expressCompany)
        {
            //20180706byzbs
            //严选回调物流订单。并把这保存到YXExpressTrace这个表中。
            var deliveryInfo = YXSV.GetExpressOrder(orderId.ToString().ToLower(), packageId, expressNo, expressCompany);
            if (deliveryInfo != null)
            {
                if (deliveryInfo.content != null && deliveryInfo.content.Count > 0)
                {
                    YXExpressTrace.ObjectSet().Where(trace => trace.OrderId == orderId && trace.PackageId == packageId
                        && trace.ExpressCompany == expressCompany && trace.ExpressNo == expressNo)
                        .ToList().ForEach(trace =>
                        {
                            trace.EntityState = EntityState.Deleted;
                            ContextFactory.CurrentThreadContext.Delete(trace);
                        });
                    var list = new List<YXExpressTrace>();
                    deliveryInfo.content.ForEach(trace =>
                    {
                        var time = DateTime.Parse(trace.time);
                        if (!list.Any(p => p.Time == time && p.Desc == trace.desc))
                        {
                            var yxExpressTrace = new YXExpressTrace
                            {
                                Id = Guid.NewGuid(),
                                OrderId = orderId,
                                OrderCode = orderCode,
                                PackageId = packageId,
                                ExpressCompany = deliveryInfo.company,
                                ExpressNo = deliveryInfo.number,
                                Time = time,
                                Desc = trace.desc,
                                EntityState = EntityState.Added
                            };
                            ContextFactory.CurrentThreadContext.SaveObject(yxExpressTrace);
                            list.Add(yxExpressTrace);
                        }
                    });
                    deliveryInfo.content = list.OrderByDescending(p => p.Time).Select(p => new DeliveryDetailInfo
                    {
                        desc = p.Desc,
                        time = p.Time.ToString("yyyy-MM-dd HH:mm:ss")
                    }).ToList();
                }
            }
            return deliveryInfo;
        }

        /// <summary>
        /// 网易严选保存错误日志
        /// </summary>
        /// <param name="dto"></param>
        public static void SaveErrorLog(YXOrderErrorLogDTO dto)
        {
            var param = string.Empty;
            try
            {
                param = JsonConvert.SerializeObject(dto);
                var yxOrderErrorLog = new YXOrderErrorLog
                {
                    Id = Guid.NewGuid(),
                    Content = dto.Content,
                    AppId = dto.AppId,
                    OrderId = dto.OrderId,
                    OrderCode = dto.OrderCode,
                    AppName = dto.AppName,
                    CommodityNames = dto.CommodityNames,
                    Json = dto.Json,
                    EntityState = System.Data.EntityState.Added
                };
                var jdlogs = new Jdlogs
                {
                    Id = Guid.NewGuid(),
                    Content = dto.Content,
                    Remark = string.Empty,
                    AppId = dto.AppId,
                    ThirdECommerceType = (int)Deploy.Enum.ThirdECommerceTypeEnum.WangYiYanXuan,
                    EntityState = System.Data.EntityState.Added
                };
                if (!string.IsNullOrEmpty(dto.Json) && dto.Json.StartsWith("{"))
                {
                    var yxResult = JsonConvert.DeserializeObject<YXResult>(dto.Json);
                    if (yxResult != null)
                    {
                        yxOrderErrorLog.Content = yxResult.Msg;
                        jdlogs.Content = string.Format("{0}App中{1}等商品异常信息:{2}", dto.AppName, dto.CommodityNames, yxResult.Msg);
                    }
                }
                ContextFactory.CurrentThreadContext.SaveObject(yxOrderErrorLog);
                if (!dto.Isdisable) ContextFactory.CurrentThreadContext.SaveObject(jdlogs);
                int count = ContextFactory.CurrentThreadContext.SaveChanges();
                if (count == 0)
                {
                    LogHelper.Error(string.Format("YXOrderHelper.SaveErrorLog网易严选保存错误日志失败,入参:{0}", param));
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("YXOrderHelper.SaveErrorLog网易严选保存错误日志异常,入参:{0}", param), ex);
            }
        }

        /// <summary>
        /// 校验严选售价
        /// </summary>
        /// <param name="skuPriceDir"></param>
        /// <returns></returns>
        public static bool CheckPrice(Dictionary<string, decimal> skuPriceDir)
        {
            if (skuPriceDir == null || skuPriceDir.Count == 0) return false;
            foreach (var item in skuPriceDir)
            {
                var skuInfo = YXSV.GetSkuInfo(item.Key);
                if (skuInfo != null && skuInfo.skuList != null && skuInfo.skuList.Any(p => p.id == item.Key))
                {
                    var yxPrice = skuInfo.skuList.Where(p => p.id == item.Key).Select(p => p.channelPrice).FirstOrDefault();
                    if (item.Value < 0 || yxPrice < 0 || item.Value != yxPrice)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 创建网易严选订单
        /// </summary>
        /// <param name="orderId">金和订单id</param>
        /// <param name="isExists">严选订单是否已存在</param>
        public static void CreateOrder(Guid orderId, bool isExists = false)
        {
            string param = string.Format("orderId={0}", orderId);
            LogHelper.Debug(string.Format("YXOrderHelper.CreateOrder创建网易严选订单,入参:{0}", param));
            if (orderId == Guid.Empty) return;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    #region 判断是否网易严选订单及获取订单信息
                    var order = CommodityOrder.ObjectSet().FirstOrDefault(o => o.Id == orderId);
                    if (order == null || !order.PaymentTime.HasValue || order.State != 1 || order.EsAppId != YJB.Deploy.CustomDTO.YJBConsts.YJAppId
                        || !ThirdECommerceHelper.IsWangYiYanXuan(order.AppId)) return;
                    var orderItemList = OrderItem.ObjectSet()
                        .Where(p => p.CommodityOrderId == order.Id && p.JDCode != null && p.JDCode != string.Empty)
                        .Select(p => new
                        {
                            p.Id,
                            p.CommodityId,
                            CommodityStockId = p.CommodityStockId ?? Guid.Empty,
                            p.JDCode,
                            p.Name,
                            p.Number,
                            RealPrice = p.RealPrice ?? 0,
                            p.CurrentPrice,
                            DiscountMoney = (p.CouponPrice ?? 0) + (p.YjbPrice ?? 0) + (p.ChangeRealPrice ?? 0) + (p.ChangeFreightPrice ?? 0)
                        }).ToList();
                    if (orderItemList.Count == 0) return;
                    var money = orderItemList.Sum(p => p.RealPrice * p.Number - p.DiscountMoney);
                    if (money <= 0)
                    {
                        LogHelper.Error(string.Format("YXOrderHelper.CreateOrder创建网易严选订单失败，商品总金额不能小于等于0,入参:{0}", param));
                    }
                    if (YXOrder.ObjectSet().Any(p => p.OrderId == order.Id)) return;
                    if (order.PaymentTime > DateTime.Parse("2018-09-27") && !CheckPrice(orderItemList.Select(p => new { p.JDCode, p.CurrentPrice }).Distinct().ToDictionary(p => p.JDCode, p => p.CurrentPrice)))
                    {
                        LogHelper.Error(string.Format("YXOrderHelper.CreateOrder创建网易严选订单失败，商品价格与严选价格不一致,入参:{0}", param));
                        return;
                    }
                    var errorSkuIds = new List<string> { "1240022" };
                    if (orderItemList.Any(p => errorSkuIds.Contains(p.JDCode)))
                    {
                        LogHelper.Error(string.Format("YXOrderHelper.CreateOrder创建网易严选订单失败，27日前支付的订单包含非法商品,入参:{0}", param));
                        return;
                    }
                    #endregion
                    #region 调用严选渠道下单接口
                    var jsonStr = string.Empty;
                    OrderOut orderOut = null;
                    if (!isExists)
                    {
                        orderOut = YXSV.CreatePaidOrder(new OrderVO
                        {
                            orderId = order.Id.ToString().ToLower(),
                            submitTime = order.SubTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            payTime = order.PaymentTime.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                            buyerAccount = order.ReceiptPhone,
                            receiverName = order.ReceiptUserName,
                            receiverMobile = order.ReceiptPhone,
                            receiverPhone = order.ReceiptPhone,
                            receiverProvinceName = order.Province,
                            receiverCityName = order.City,
                            receiverDistrictName = order.District,
                            receiverAddressDetail = order.ReceiptAddress,
                            realPrice = orderItemList.Sum(p => p.RealPrice * p.Number - p.DiscountMoney) + order.Freight,
                            expFee = order.Freight,
                            payMethod = GetPaymentName(order.Payment),
                            //invoiceTitle = string.Empty,
                            //invoiceAmount = string.Empty,
                            orderSkus = orderItemList.Select(p => new OrderSkuVO
                            {
                                skuId = p.JDCode,
                                productName = p.Name,
                                saleCount = p.Number,
                                originPrice = p.RealPrice,
                                subtotalAmount = p.RealPrice * p.Number - p.DiscountMoney,
                                couponTotalAmount = p.DiscountMoney,
                                activityTotalAmount = 0
                            }).ToList()
                        }, ref jsonStr);
                        if (orderOut == null || orderOut.orderId != order.Id.ToString().ToLower())
                        {
                            SaveErrorLog(new YXOrderErrorLogDTO
                            {
                                Content = jsonStr,
                                AppId = order.AppId,
                                OrderId = order.Id,
                                OrderCode = order.Code,
                                AppName = order.AppName,
                                CommodityNames = string.Join("、", orderItemList.Select(x => x.Name).ToList()),
                                Json = jsonStr
                            });
                            return;
                        }
                    }
                    else
                    {
                        orderOut = YXSV.GetPaidOrder(orderId.ToString(), ref jsonStr);
                    }
                    #endregion
                    #region 保存YXOrder
                    var yxOrder = new YXOrder
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        OrderCode = order.Code,
                        OrderState = (int)orderOut.orderStatus,
                        OrderStateName = new EnumHelper().GetDescription(orderOut.orderStatus),
                        OrderSubTime = order.SubTime,
                        PayTime = order.PaymentTime.Value,
                        SubId = order.SubId,
                        AppId = order.AppId,
                        EsAppId = order.EsAppId.Value,
                        AppName = order.AppName,
                        AppType = order.AppType,
                        SupplierName = order.SupplierName,
                        SupplierCode = order.SupplierCode,
                        SupplierType = order.SupplierType,
                        ShipperType = order.ShipperType,
                        EntityState = System.Data.EntityState.Added
                    };
                    ContextFactory.CurrentThreadContext.SaveObject(yxOrder);
                    #endregion
                    #region 保存YXOrderJournal
                    var yxOrderJournal = new YXOrderJournal
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        OrderCode = order.Code,
                        SubTime = DateTime.Now,
                        SubId = order.SubId,
                        Name = new EnumHelper().GetDescription(orderOut.orderStatus),
                        Details = new EnumHelper().GetDescription(orderOut.orderStatus),
                        StateFrom = -1,
                        StateTo = (int)orderOut.orderStatus,
                        Json = jsonStr,
                        EntityState = System.Data.EntityState.Added
                    };
                    ContextFactory.CurrentThreadContext.SaveObject(yxOrderJournal);
                    #endregion
                    #region 保存YXOrderPackage、YXOrderPackageJournal、YXOrderSku、YXExpressDetailInfo、YXExpressDetailInfoSku
                    if (orderOut.orderPackages != null && orderOut.orderPackages.Count > 0)
                    {
                        var packageIdList = YXOrderPackage.ObjectSet().Where(yop => yop.OrderId == order.Id).Select(yop => yop.PackageId).ToList();
                        orderOut.orderPackages.ForEach(orderPackage =>
                        {
                            #region YXOrderPackage
                            DateTime expCreateTime, confirmTime;
                            DateTime.TryParse(orderPackage.expCreateTime, out expCreateTime);
                            DateTime.TryParse(orderPackage.confirmTime, out confirmTime);
                            var expCreateTimeNull = expCreateTime == DateTime.MinValue ? default(DateTime?) : expCreateTime;
                            var confirmTimeNull = confirmTime == DateTime.MinValue ? default(DateTime?) : confirmTime;
                            if (!packageIdList.Contains(orderPackage.packageId))
                            {
                                var yxOrderPackage = new YXOrderPackage
                                {
                                    Id = Guid.NewGuid(),
                                    OrderId = order.Id,
                                    OrderCode = order.Code,
                                    PackageId = orderPackage.packageId,
                                    ExpressCompany = orderPackage.expressCompany ?? string.Empty,
                                    ExpressNo = orderPackage.expressNo ?? string.Empty,
                                    ExpCreateTime = expCreateTimeNull,
                                    ConfirmTime = confirmTimeNull,
                                    PackageState = (int)orderPackage.packageStatus,
                                    PackageStateName = new EnumHelper().GetDescription(orderPackage.packageStatus),
                                    EntityState = System.Data.EntityState.Added
                                };
                                ContextFactory.CurrentThreadContext.SaveObject(yxOrderPackage);
                            }
                            #endregion
                            #region YXOrderPackageJournal
                            var yxOrderPackageJournal = new YXOrderPackageJournal
                            {
                                Id = Guid.NewGuid(),
                                OrderId = order.Id,
                                OrderCode = order.Code,
                                PackageId = orderPackage.packageId,
                                ExpCreateTime = expCreateTimeNull,
                                ConfirmTime = confirmTimeNull,
                                Name = new EnumHelper().GetDescription(orderPackage.packageStatus),
                                Details = new EnumHelper().GetDescription(orderPackage.packageStatus),
                                StateFrom = -1,
                                StateTo = (int)orderPackage.packageStatus,
                                Json = string.Empty,
                                EntityState = System.Data.EntityState.Added
                            };
                            ContextFactory.CurrentThreadContext.SaveObject(yxOrderPackageJournal);
                            #endregion
                            #region YXOrderSku
                            if (orderPackage.orderSkus != null && orderPackage.orderSkus.Count > 0)
                            {
                                var skuIdList = YXOrderSku.ObjectSet().Where(yos => yos.OrderId == order.Id && yos.PackageId == orderPackage.packageId).Select(yos => yos.SkuId).ToList();
                                orderPackage.orderSkus.ForEach(orderSku =>
                                {
                                    if (!skuIdList.Contains(orderSku.skuId))
                                    {
                                        var orderItemId = Guid.Empty;
                                        var commodityId = Guid.Empty;
                                        var commodityStockId = Guid.Empty;
                                        var orderItem = orderItemList.Where(p => p.JDCode == orderSku.skuId).FirstOrDefault();
                                        if (orderItem != null)
                                        {
                                            orderItemId = orderItem.Id;
                                            commodityId = orderItem.CommodityId;
                                            commodityStockId = orderItem.CommodityStockId;
                                        }
                                        var yxOrderSku = new YXOrderSku
                                        {
                                            Id = Guid.NewGuid(),
                                            OrderId = order.Id,
                                            OrderCode = order.Code,
                                            OrderItemId = orderItemId,
                                            CommodityId = commodityId,
                                            CommodityStockId = commodityStockId,
                                            PackageId = orderPackage.packageId,
                                            SkuId = orderSku.skuId,
                                            ProductName = orderSku.productName,
                                            SaleCount = orderSku.saleCount,
                                            OriginPrice = orderSku.originPrice,
                                            SubtotalAmount = orderSku.subtotalAmount,
                                            CouponTotalAmount = orderSku.couponTotalAmount,
                                            ActivityTotalAmount = orderSku.activityTotalAmount,
                                            EntityState = System.Data.EntityState.Added
                                        };
                                        ContextFactory.CurrentThreadContext.SaveObject(yxOrderSku);
                                    }
                                });
                            }
                            #endregion
                            #region YXExpressDetailInfo
                            if (orderPackage.expressDetailInfos != null && orderPackage.expressDetailInfos.Count > 0)
                            {
                                var expressNoList = YXExpressDetailInfo.ObjectSet().Where(yedi => yedi.OrderId == order.Id && yedi.PackageId == orderPackage.packageId).Select(yedi => yedi.ExpressNo).ToList();
                                orderPackage.expressDetailInfos.ForEach(expressDetailInfo =>
                                {
                                    if (!expressNoList.Contains(expressDetailInfo.expressNo))
                                    {
                                        var yxExpressDetailInfo = new YXExpressDetailInfo
                                        {
                                            Id = Guid.NewGuid(),
                                            OrderId = order.Id,
                                            OrderCode = order.Code,
                                            PackageId = orderPackage.packageId,
                                            ExpressCompany = expressDetailInfo.expressCompany,
                                            ExpressNo = expressDetailInfo.expressNo,
                                            EntityState = System.Data.EntityState.Added
                                        };
                                        ContextFactory.CurrentThreadContext.SaveObject(yxExpressDetailInfo);
                                        #region YXExpressDetailInfoSku
                                        if (expressDetailInfo.skus != null && expressDetailInfo.skus.Count > 0)
                                        {
                                            var eSkuIdList = YXExpressDetailInfoSku.ObjectSet().Where(yedis => yedis.OrderId == order.Id && yedis.PackageId == orderPackage.packageId).Select(yedis => yedis.SkuId).ToList();
                                            expressDetailInfo.skus.ForEach(sku =>
                                            {
                                                if (!eSkuIdList.Contains(sku.skuId))
                                                {
                                                    var orderItemId = Guid.Empty;
                                                    var commodityId = Guid.Empty;
                                                    var commodityStockId = Guid.Empty;
                                                    var orderItem = orderItemList.Where(p => p.JDCode == sku.skuId).FirstOrDefault();
                                                    if (orderItem != null)
                                                    {
                                                        orderItemId = orderItem.Id;
                                                        commodityId = orderItem.CommodityId;
                                                        commodityStockId = orderItem.CommodityStockId;
                                                    }
                                                    var yxExpressDetailInfoSku = new YXExpressDetailInfoSku
                                                    {
                                                        Id = Guid.NewGuid(),
                                                        OrderId = order.Id,
                                                        OrderCode = order.Code,
                                                        OrderItemId = orderItemId,
                                                        CommodityId = commodityId,
                                                        CommodityStockId = commodityStockId,
                                                        PackageId = orderPackage.packageId,
                                                        SkuId = sku.skuId,
                                                        ProductName = sku.productName,
                                                        SaleCount = sku.saleCount,
                                                        OriginPrice = sku.originPrice,
                                                        SubtotalAmount = sku.subtotalAmount,
                                                        CouponTotalAmount = sku.couponTotalAmount,
                                                        ActivityTotalAmount = sku.activityTotalAmount,
                                                        YXExpressDetailInfoId = yxExpressDetailInfo.Id,
                                                        ExpressCompany = yxExpressDetailInfo.ExpressCompany,
                                                        ExpressNo = yxExpressDetailInfo.ExpressNo,
                                                        EntityState = System.Data.EntityState.Added
                                                    };
                                                    ContextFactory.CurrentThreadContext.SaveObject(yxExpressDetailInfoSku);
                                                }
                                            });
                                        }
                                        #endregion
                                    }
                                });
                            }
                            #endregion
                        });
                    }
                    #endregion
                    int count = ContextFactory.CurrentThreadContext.SaveChanges();
                    if (count > 0)
                    {
                        #region 订单所有包裹发货后更新订单状态到已发货
                        if (!YXOrderPackage.ObjectSet().Any(p => p.OrderId == orderId && !p.ExpCreateTime.HasValue))
                        {
                            var commodityOrder = new CommodityOrderDTO();
                            commodityOrder.Id = orderId;
                            commodityOrder.State = 2;
                            commodityOrder.ShipmentsTime = DateTime.Now;
                            var facade = new CommodityOrderFacade();
                            facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                            facade.UpdateCommodityOrder(commodityOrder);
                        }
                        #endregion
                    }
                    else
                    {
                        LogHelper.Error(string.Format("YXOrderHelper.CreateOrder创建网易严选订单失败,入参:{0}", param));
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(string.Format("YXOrderHelper.CreateOrder创建网易严选订单异常,入参:{0}", param), ex);
                }
            });
        }

        /// <summary>
        /// 网易严选订单异常回调（严选主动取消订单）
        /// </summary>
        /// <param name="sign"></param>
        /// <param name="exceptionInfoJsonStr"></param>
        public static void ExceptionalOrder(YXSign sign, string exceptionInfoJsonStr)
        {
            if (string.IsNullOrEmpty(exceptionInfoJsonStr)) return;
            if (!YXSV.CheckSign(sign, new Dictionary<string, string> { { "exceptionInfo", exceptionInfoJsonStr } })) return;
            LogHelper.Info("YXOrderHelper.ExceptionalOrder网易严选订单异常回调,入参:" + exceptionInfoJsonStr);
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var exceptionInfo = JsonConvert.DeserializeObject<ExceptionInfo>(exceptionInfoJsonStr);
                    if (exceptionInfo == null || exceptionInfo.orderId == Guid.Empty) return;
                    var yxOrder = YXOrder.ObjectSet().Where(p => p.OrderId == exceptionInfo.orderId).FirstOrDefault();
                    if (yxOrder == null) return;
                    #region 保存YXOrderJournal
                    var yxOrderJournal = new YXOrderJournal
                    {
                        Id = Guid.NewGuid(),
                        OrderId = yxOrder.OrderId,
                        OrderCode = yxOrder.OrderCode,
                        SubTime = DateTime.Now,
                        Name = "严选取消订单",
                        Details = "严选取消订单",
                        StateFrom = -1,
                        StateTo = (int)OrderStatusEnum.KF_CANCEL,
                        EntityState = System.Data.EntityState.Added
                    };
                    ContextFactory.CurrentThreadContext.SaveObject(yxOrderJournal);
                    #endregion
                    int count = ContextFactory.CurrentThreadContext.SaveChanges();
                    if (count == 0)
                        LogHelper.Error(string.Format("YXOrderHelper.DeliverOrder网易严选订单异常回调失败,入参:{0}", exceptionInfoJsonStr));
                    var message = string.Format("由于数据同步问题，您的订单{0}购买的商品目前处于无货状态，您可申请退款流程，如有不便敬请谅解！", yxOrder.OrderCode);
                    new AddMessage().AddMessages(yxOrder.OrderId.ToString(), yxOrder.SubId.ToString(), yxOrder.EsAppId
                        , yxOrder.OrderCode, null, message, "yxCancelOrder");
                }
                catch (Exception ex)
                {
                    LogHelper.Error(string.Format("YXOrderHelper.DeliverOrder网易严选订单异常回调异常,入参:{0}", exceptionInfoJsonStr), ex);
                }
            });
        }

        /// <summary>
        /// 网易严选订单包裹物流绑单回调:并保存到本地的三个表中。
        /// </summary>
        /// <param name="sign"></param>
        /// <param name="packageJsonStr"></param>
        /// <param name="isCheckSign"></param>
        public static void DeliverOrder(YXSign sign, string packageJsonStr, bool isCheckSign = false)
        {
            if (string.IsNullOrEmpty(packageJsonStr)) return;
            if (!isCheckSign)
            {
                if (!YXSV.CheckSign(sign, new Dictionary<string, string> { { "orderPackage", packageJsonStr } })) return;
            }
            LogHelper.Info("YXOrderHelper.DeliverOrder网易严选订单包裹物流绑单回调,入参:" + packageJsonStr);
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var package = JsonConvert.DeserializeObject<OrderPackageNotify>(packageJsonStr);
                    if (package == null || package.orderId == Guid.Empty || string.IsNullOrEmpty(package.packageId)
                        || package.expressDetailInfos == null || package.expressDetailInfos.Count == 0) return;
                    var expCreateTime = DateTime.Parse("1970-01-01").AddMilliseconds(package.expCreateTime).AddHours(8);
                    #region 更新YXOrderPackage
                    var orderPackage = YXOrderPackage.ObjectSet().Where(p => p.OrderId == package.orderId
                        && p.PackageId == package.packageId).FirstOrDefault();
                    if (orderPackage == null) return;
                    var packageOldState = orderPackage.PackageState;
                    orderPackage.ExpCreateTime = expCreateTime;
                    orderPackage.PackageState = (int)PackageStatus.START_DELIVERY;
                    orderPackage.PackageStateName = new EnumHelper().GetDescription(PackageStatus.START_DELIVERY);
                    if (string.IsNullOrEmpty(orderPackage.ExpressNo)) orderPackage.ExpressNo = string.Join(",", package.expressDetailInfos.Select(p => p.expressNo).ToList());
                    if (string.IsNullOrEmpty(orderPackage.ExpressCompany)) orderPackage.ExpressCompany = string.Join(",", package.expressDetailInfos.Select(p => p.expressCompany).ToList());

                    //把子单号也写入 by zbs 20180703
                    var expressDtlInfos = package.expressDetailInfos.Select(p => p.subExpressNos);
                    if (string.IsNullOrEmpty(orderPackage.SubExpressNos)) orderPackage.SubExpressNos = string.Join(",", expressDtlInfos);
                    orderPackage.EntityState = EntityState.Modified;

                    #endregion
                    #region 保存YXOrderPackageJournal
                    var yxOrderPackageJournal = new YXOrderPackageJournal
                    {
                        Id = Guid.NewGuid(),
                        OrderId = orderPackage.OrderId,
                        OrderCode = orderPackage.OrderCode,
                        PackageId = orderPackage.PackageId,
                        ExpCreateTime = expCreateTime,
                        Name = orderPackage.PackageStateName,
                        Details = orderPackage.PackageStateName,
                        StateFrom = packageOldState,
                        StateTo = orderPackage.PackageState,
                        Json = packageJsonStr,
                        EntityState = System.Data.EntityState.Added
                    };
                    ContextFactory.CurrentThreadContext.SaveObject(yxOrderPackageJournal);
                    #endregion
                    #region 保存YXExpressDetailInfo和YXExpressDetailInfoSku
                    var expressNoList = YXExpressDetailInfo.ObjectSet().Where(yedi => yedi.OrderId == package.orderId && yedi.PackageId == orderPackage.PackageId).Select(yedi => yedi.ExpressNo).ToList();
                    package.expressDetailInfos.ForEach(expressDetailInfo =>
                    {
                        if (!expressNoList.Contains(expressDetailInfo.expressNo))
                        {
                            var yxExpressDetailInfo = new YXExpressDetailInfo
                            {
                                Id = Guid.NewGuid(),
                                OrderId = orderPackage.OrderId,
                                OrderCode = orderPackage.OrderCode,
                                PackageId = orderPackage.PackageId,
                                ExpressCompany = expressDetailInfo.expressCompany,
                                ExpressNo = expressDetailInfo.expressNo,
                                EntityState = System.Data.EntityState.Added,
                                SubExpressNos = orderPackage.SubExpressNos
                            };
                            ContextFactory.CurrentThreadContext.SaveObject(yxExpressDetailInfo);
                            #region YXExpressDetailInfoSku
                            if (expressDetailInfo.skus != null && expressDetailInfo.skus.Count > 0)
                            {
                                var orderItemList = YXOrderSku.ObjectSet().Where(yos => yos.OrderId == package.orderId && yos.PackageId == package.packageId)
                                    .Select(p => new
                                    {
                                        p.SkuId,
                                        p.OrderItemId,
                                        p.CommodityId,
                                        p.CommodityStockId
                                    });
                                var eSkuIdList = YXExpressDetailInfoSku.ObjectSet().Where(yedis => yedis.OrderId == package.orderId && yedis.PackageId == package.packageId).Select(yedis => yedis.SkuId).ToList();
                                expressDetailInfo.skus.ForEach(sku =>
                                {
                                    if (!eSkuIdList.Contains(sku.skuId))
                                    {
                                        var orderItemId = Guid.Empty;
                                        var commodityId = Guid.Empty;
                                        var commodityStockId = Guid.Empty;
                                        var orderItem = orderItemList.Where(p => p.SkuId == sku.skuId).FirstOrDefault();
                                        if (orderItem != null)
                                        {
                                            orderItemId = orderItem.OrderItemId;
                                            commodityId = orderItem.CommodityId;
                                            commodityStockId = orderItem.CommodityStockId;
                                        }
                                        var yxExpressDetailInfoSku = new YXExpressDetailInfoSku
                                        {
                                            Id = Guid.NewGuid(),
                                            OrderId = orderPackage.OrderId,
                                            OrderCode = orderPackage.OrderCode,
                                            OrderItemId = orderItemId,
                                            CommodityId = commodityId,
                                            CommodityStockId = commodityStockId,
                                            PackageId = orderPackage.PackageId,
                                            SkuId = sku.skuId,
                                            ProductName = sku.productName,
                                            SaleCount = sku.saleCount,
                                            OriginPrice = sku.originPrice,
                                            SubtotalAmount = sku.subtotalAmount,
                                            CouponTotalAmount = sku.couponTotalAmount,
                                            ActivityTotalAmount = sku.activityTotalAmount,
                                            YXExpressDetailInfoId = yxExpressDetailInfo.Id,
                                            ExpressCompany = yxExpressDetailInfo.ExpressCompany,
                                            ExpressNo = yxExpressDetailInfo.ExpressNo,
                                            EntityState = System.Data.EntityState.Added,
                                            SubExpressNos = yxExpressDetailInfo.SubExpressNos
                                        };
                                        ContextFactory.CurrentThreadContext.SaveObject(yxExpressDetailInfoSku);
                                    }
                                });
                            }
                            #endregion
                        }
                    });
                    #endregion
                    int count = ContextFactory.CurrentThreadContext.SaveChanges();
                    if (count > 0)
                    {
                        #region 订单所有包裹发货后更新订单状态到已发货
                        if (!YXOrderPackage.ObjectSet().Any(p => p.OrderId == package.orderId && !p.ExpCreateTime.HasValue))
                        {
                            var commodityOrder = new CommodityOrderDTO();
                            commodityOrder.Id = package.orderId;
                            commodityOrder.State = 2;
                            commodityOrder.ShipmentsTime = DateTime.Now;
                            var facade = new CommodityOrderFacade();
                            facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                            facade.UpdateCommodityOrder(commodityOrder);
                        }
                        #endregion
                    }
                    else
                    {
                        LogHelper.Error(string.Format("YXOrderHelper.DeliverOrder网易严选订单包裹物流绑单回调失败,入参:{0}", packageJsonStr));
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(string.Format("YXOrderHelper.DeliverOrder网易严选订单包裹物流绑单回调异常,入参:{0}", packageJsonStr), ex);
                }
            });
        }

        /// <summary>
        /// 网易严选获取订单包裹商品物流信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static List<YXExpressDetailInfoSkuDTO> GetYXExpressDetailInfoSkuList(Guid orderId)
        {
            if (orderId == Guid.Empty) return new List<YXExpressDetailInfoSkuDTO>();
            try
            {
                return YXExpressDetailInfoSku.ObjectSet()
                    .Where(p => p.OrderId == orderId)
                    .ToList()
                    .Select(p => p.ToEntityData())
                    .ToList();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("YXOrderHelper.GetYXExpressDetailInfoSkuList网易严选获取订单包裹商品物流信息异常,入参:orderId={0}", orderId), ex);
            }
            return new List<YXExpressDetailInfoSkuDTO>();
        }

        /// <summary>
        /// 网易严选获取订单包裹信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static List<YXOrderPackageDTO> GetYXOrderPackageList(Guid orderId)
        {
            if (orderId == Guid.Empty) return new List<YXOrderPackageDTO>();
            try
            {
                return YXOrderPackage.ObjectSet()
                    .Where(p => p.OrderId == orderId)
                    .ToList()
                    .Select(p => p.ToEntityData())
                    .ToList();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("YXOrderHelper.GetYXExpressDetailInfoSkuList网易严选获取订单包裹信息异常,入参:orderId={0}", orderId), ex);
            }
            return new List<YXOrderPackageDTO>();
        }

        /// <summary>
        /// 网易严选获取订单项物流轨迹信息
        /// </summary>
        /// <param name="orderItemId"></param>
        /// <returns>DeliveryInfo:回调严选得到信息就放在这个参数中，返回的信息保存到数据库还要显示在客户端视图上。</returns>
        public static DeliveryInfo GetExpressInfo(Guid orderItemId)
        {
            if (orderItemId == Guid.Empty) return null;
            try
            {
                var yxExpressSku = YXExpressDetailInfoSku.ObjectSet().Where(p => p.OrderItemId == orderItemId).FirstOrDefault();
                if (yxExpressSku == null) return null;
                
                var deliveryInfo = YXSV.GetExpressOrder(yxExpressSku.OrderId.ToString().ToLower(), yxExpressSku.PackageId, yxExpressSku.ExpressNo, yxExpressSku.ExpressCompany);
                if (deliveryInfo != null)
                {
                    if (deliveryInfo.content != null && deliveryInfo.content.Count > 0)
                    {                       
                        var list = new List<YXExpressTrace>();
                        deliveryInfo.content.ForEach(trace =>
                        {
                            var time = DateTime.Parse(trace.time);
                            if (!list.Any(p => p.Time == time && p.Desc == trace.desc))
                            {
                                var yxExpressTrace = new YXExpressTrace
                                {                                    
                                    OrderId = yxExpressSku.OrderId,
                                    OrderCode = yxExpressSku.OrderCode,
                                    PackageId = yxExpressSku.PackageId,
                                    ExpressCompany = deliveryInfo.company,
                                    ExpressNo = deliveryInfo.number,
                                    Time = time,
                                    Desc = trace.desc                                   
                                };                               
                                list.Add(yxExpressTrace);
                            }
                        });
                        deliveryInfo.content = list.OrderByDescending(p => p.Time).Select(p => new DeliveryDetailInfo
                        {
                            desc = p.Desc,
                            time = p.Time.ToString("yyyy-MM-dd HH:mm:ss")
                        }).ToList();
                    }
                } 
                return deliveryInfo;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("YXOrderHelper.GetExpressInfo网易严选获取订单项物流轨迹信息异常,入参:orderItemId={0}", orderItemId), ex);
            }
            return null;
        }
        /// <summary>
        /// 网易严选获取订单项物流子订单 轨迹信息
        /// </summary>
        /// <param name="orderItemId"></param>
        /// <returns>DeliveryInfo:回调严选得到信息就放在这个参数中，返回的信息保存到数据库还要显示在视图上。</returns>
        public static DeliveryInfo GetExpressInfoSub(Guid orderItemId, string subExpress)
        {
            if (orderItemId == Guid.Empty) return null;
            try
            {
                var yxExpressSku = YXExpressDetailInfoSku.ObjectSet().Where(p => p.OrderItemId == orderItemId).FirstOrDefault();
                if (yxExpressSku == null) return null;
                if (YXOrderPackage.ObjectSet().Any(p => p.OrderId == yxExpressSku.OrderId && p.PackageId == yxExpressSku.PackageId && p.ConfirmTime.HasValue))
                {//确定序列是否包含任何符合包裹ID和订单ID条件元素。即是否存在。如果存在进入：
                    var deliveryInfo = new DeliveryInfo { company = yxExpressSku.ExpressCompany, number = yxExpressSku.ExpressNo, content = new List<DeliveryDetailInfo>() };
                    YXExpressTrace.ObjectSet()
                        .Where(p => p.OrderId == yxExpressSku.OrderId && p.PackageId == yxExpressSku.PackageId
                            && p.ExpressCompany == yxExpressSku.ExpressCompany && p.ExpressNo == subExpress)
                        .ToList().OrderByDescending(p => p.Time).ToList().ForEach(p =>
                        {
                            //var time = p.Time.ToString("yyyy-MM-dd HH:mm:ss");
                            //if (!deliveryInfo.content.Any(x => x.time == time && x.desc == p.Desc))
                            //{
                            deliveryInfo.content.Add(new DeliveryDetailInfo
                            {
                                desc = p.Desc,
                                time = p.Time.ToString("yyyy-MM-dd HH:mm:ss")
                            });
                            //}
                        });
                    return deliveryInfo;
                }
                else
                {//如果没有保存相关的主单物流跟踪信息,就下来保存这些信息[新增记录]。
                    //var deliveryInfo = SaveYXExpressTrace(yxExpressSku.OrderId, yxExpressSku.OrderCode, yxExpressSku.PackageId, yxExpressSku.SubExpressNos, yxExpressSku.ExpressCompany);

                    var deliveryInfo = SaveYXExpressTrace(yxExpressSku.OrderId, yxExpressSku.OrderCode, yxExpressSku.PackageId, subExpress, yxExpressSku.ExpressCompany);
                    int count = ContextFactory.CurrentThreadContext.SaveChanges();
                    if (count == 0) LogHelper.Error(string.Format("YXOrderHelper.GetExpressInfo网易严选获取订单项物流轨迹信息失败,入参:orderItemId={0}", orderItemId));
                    return deliveryInfo;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("YXOrderHelper.GetExpressInfoSub网易严选获取订单项物流子包裹轨迹信息异常,入参:orderItemId={0}", orderItemId), ex);
            }
            return null;
        }

        /// <summary>
        /// 网易严选确认收货
        /// </summary>
        /// <param name="orderId"></param>
        public static void ConfirmReceivedOrder(Guid orderId)
        {
            string param = string.Format("orderId={0}", orderId);
            LogHelper.Debug(string.Format("YXOrderHelper.ConfirmReceivedOrder网易严选确认收货,入参:{0}", param));
            if (orderId == Guid.Empty) return;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var time = DateTime.Now;
                    var yxOrder = YXOrder.ObjectSet().Where(p => p.OrderId == orderId).FirstOrDefault();
                    var yxExpressSkuList = YXExpressDetailInfoSku.ObjectSet().Where(p => p.OrderId == orderId).ToList();
                    if (yxOrder == null) return;
                    YXOrderPackage.ObjectSet().Where(p => p.OrderId == orderId && !p.ConfirmTime.HasValue).ToList().ForEach(orderPackage =>
                    {
                        var result = YXSV.ConfirmReceivedOrder(orderPackage.OrderId.ToString().ToLower(), orderPackage.PackageId, time);
                        if (result.isSuccess)
                        {
                            var packageOldState = orderPackage.PackageState;
                            orderPackage.ConfirmTime = time;
                            orderPackage.PackageState = (int)PackageStatus.WAITING_COMMENT;
                            orderPackage.PackageStateName = new EnumHelper().GetDescription(PackageStatus.WAITING_COMMENT);
                            #region YXOrderPackageJournal
                            var yxOrderPackageJournal = new YXOrderPackageJournal
                            {
                                Id = Guid.NewGuid(),
                                OrderId = orderPackage.OrderId,
                                OrderCode = orderPackage.OrderCode,
                                PackageId = orderPackage.PackageId,
                                ExpCreateTime = orderPackage.ExpCreateTime,
                                ConfirmTime = orderPackage.ConfirmTime,
                                Name = orderPackage.PackageStateName,
                                Details = orderPackage.PackageStateName,
                                StateFrom = packageOldState,
                                StateTo = orderPackage.PackageState,
                                Json = result.Data,
                                EntityState = System.Data.EntityState.Added
                            };
                            ContextFactory.CurrentThreadContext.SaveObject(yxOrderPackageJournal);
                            #endregion
                            #region YXExpressTrace
                            YXExpressDetailInfo.ObjectSet().Where(expressDetail => expressDetail.OrderId == orderPackage.OrderId
                                && expressDetail.PackageId == orderPackage.PackageId).ToList().ForEach(expressDetail =>
                                {
                                    SaveYXExpressTrace(expressDetail.OrderId, expressDetail.OrderCode, expressDetail.PackageId, expressDetail.ExpressNo, expressDetail.ExpressCompany);
                                });
                            #endregion
                        }
                        else
                        {
                            #region YXOrderErrorLog
                            var commodityNameList = yxExpressSkuList.Where(sku => sku.OrderId == orderPackage.OrderId
                                && sku.PackageId == orderPackage.PackageId).Select(sku => sku.ProductName).ToList();
                            var commodityNames = string.Join("、", commodityNameList);
                            var yxOrderErrorLog = new YXOrderErrorLog
                            {
                                Id = Guid.NewGuid(),
                                Content = string.Format("订单{0}的包裹{1}中商品{2}确认收货失败", orderPackage.OrderCode, orderPackage.PackageId, commodityNames),
                                AppId = yxOrder.AppId,
                                OrderId = orderPackage.OrderId,
                                OrderCode = orderPackage.OrderCode,
                                AppName = yxOrder.AppName,
                                Json = result.Data,
                                CommodityNames = commodityNames,
                                EntityState = System.Data.EntityState.Added
                            };
                            ContextFactory.CurrentThreadContext.SaveObject(yxOrderErrorLog);
                            #endregion
                        }
                    });
                    int count = ContextFactory.CurrentThreadContext.SaveChanges();
                    if (count == 0)
                    {
                        LogHelper.Error(string.Format("YXOrderHelper.ConfirmReceivedOrder网易严选确认收货失败,入参:{0}", param));
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(string.Format("YXOrderHelper.ConfirmReceivedOrder网易严选确认收货异常,入参:{0}", param), ex);
                }
            });
        }

        /// <summary>
        /// 定时获取未确认收货订单物流轨迹信息
        /// </summary>
        public static void GetExpressOrderForJob()
        {
            try
            {
                YXOrderPackage.ObjectSet().Where(p => !p.ConfirmTime.HasValue).ToList().ForEach(orderPackage =>
                {
                    YXExpressDetailInfo.ObjectSet().Where(expressDetail => expressDetail.OrderId == orderPackage.OrderId
                        && expressDetail.PackageId == orderPackage.PackageId).ToList().ForEach(expressDetail =>
                        {
                            SaveYXExpressTrace(expressDetail.OrderId, expressDetail.OrderCode, expressDetail.PackageId, expressDetail.ExpressNo, expressDetail.ExpressCompany);
                        });
                });
                int count = ContextFactory.CurrentThreadContext.SaveChanges();
                if (count == 0)
                {
                    LogHelper.Error("YXOrderHelper.GetExpressOrderForJob定时获取未确认收货订单物流轨迹信息失败");
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXOrderHelper.GetExpressOrderForJob定时获取未确认收货订单物流轨迹信息异常", ex);
            }
        }

        /// <summary>
        /// 判断是否网易严选订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static bool IsYXOrder(Guid orderId)
        {
            string param = string.Format("orderId={0}", orderId);
            LogHelper.Debug(string.Format("YXOrderHelper.IsYXOrder判断是否网易严选订单,入参:{0}", param));
            if (orderId == Guid.Empty) return false;
            try
            {
                return YXOrder.ObjectSet().Any(p => p.OrderId == orderId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("YXOrderHelper.IsYXOrder判断是否网易严选订单异常,入参:{0}", param), ex);
            }
            return false;
        }
    }
}
