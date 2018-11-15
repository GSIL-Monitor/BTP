using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.CustomDTO.ThirdECommerce;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;
using Newtonsoft.Json;

namespace Jinher.AMP.BTP.TPS.Helper
{
    /// <summary>
    /// 第三方电商(非京东大客户和网易严选)标准接口接入-订单帮助类
    /// </summary>
    public class ThirdECommerceOrderHelper
    {
        /// <summary>
        /// 第三方电商保存错误日志
        /// </summary>
        /// <param name="dto"></param>
        public static void SaveErrorLog(ThirdECOrderErrorLogDTO dto)
        {
            var param = string.Empty;
            try
            {
                param = JsonConvert.SerializeObject(dto);
                var orderErrorLog = new ThirdECOrderErrorLog
                {
                    Id = Guid.NewGuid(),
                    Content = dto.Content,
                    AppId = dto.AppId,
                    OrderId = dto.OrderId,
                    OrderCode = dto.OrderCode,
                    AppName = dto.AppName,
                    CommodityNames = dto.CommodityNames,
                    Json = dto.Json,
                    EntityState = EntityState.Added
                };
                var jdlogs = new Jdlogs
                {
                    Id = Guid.NewGuid(),
                    Content = dto.Content,
                    Remark = string.Empty,
                    AppId = dto.AppId,
                    ThirdECommerceType = (int)Deploy.Enum.ThirdECommerceTypeEnum.ByBiaoZhunJieKou,
                    EntityState = EntityState.Added
                };
                if (!string.IsNullOrEmpty(dto.Json) && dto.Json.StartsWith("{"))
                {
                    var thirdResult = JsonConvert.DeserializeObject<ThirdResponse>(dto.Json);
                    if (thirdResult != null)
                    {
                        orderErrorLog.Content = thirdResult.Msg;
                        jdlogs.Content = string.Format("{0}App中{1}等商品异常信息:{2}", dto.AppName, dto.CommodityNames, thirdResult.Msg);
                    }
                }
                ContextFactory.CurrentThreadContext.SaveObject(orderErrorLog);
                if (!dto.Isdisable) ContextFactory.CurrentThreadContext.SaveObject(jdlogs);
                int count = ContextFactory.CurrentThreadContext.SaveChanges();
                if (count == 0)
                {
                    LogHelper.Error(string.Format("ThirdECommerceOrderHelper.SaveErrorLog第三方电商保存错误日志失败,入参:{0}", param));
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ThirdECommerceOrderHelper.SaveErrorLog第三方电商保存错误日志异常,入参:{0}", param), ex);
            }
        }

        /// <summary>
        /// 创建第三方电商订单
        /// </summary>
        /// <param name="orderId"></param>
        public static void CreateOrder(Guid orderId)
        {
            string param = string.Format("orderId={0}", orderId);
            LogHelper.Debug(string.Format("ThirdECOrderHelper.CreateOrder创建第三方电商订单,入参:{0}", param));
            if (orderId == Guid.Empty) return;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    #region 判断是否第三方电商订单及获取订单信息
                    var order = CommodityOrder.ObjectSet().FirstOrDefault(o => o.Id == orderId);
                    if (order == null || !order.PaymentTime.HasValue || order.State != 1 || order.EsAppId != YJB.Deploy.CustomDTO.YJBConsts.YJAppId) return;
                    var thirdECommerce = ThirdECommerceHelper.GetThirdECommerce(order.AppId);
                    if (thirdECommerce == null || string.IsNullOrEmpty(thirdECommerce.OpenApiKey) || string.IsNullOrEmpty(thirdECommerce.OpenApiCallerId) || string.IsNullOrEmpty(thirdECommerce.OrderCreateUrl)) return;
                    var orderItemList = OrderItem.ObjectSet()
                        .Where(p => p.CommodityOrderId == order.Id && p.JDCode != null && p.JDCode != string.Empty)
                        .Select(p => new
                        {
                            p.JDCode,
                            p.Name,
                            p.Number,
                            CostPrice = p.CostPrice ?? 0
                        }).ToList();
                    if (orderItemList.Count == 0) return;
                    if (ThirdECOrder.ObjectSet().Any(p => p.OrderId == order.Id)) return;
                    #endregion
                    #region 调用第三方电商订单提交接口
                    var jsonStr = string.Empty;
                    var response = ThirdECommerceSV.CreateOrder(new ThirdApiInfo
                    {
                        Apikey = thirdECommerce.OpenApiKey,
                        CallerId = thirdECommerce.OpenApiCallerId,
                        ApiUrl = thirdECommerce.OrderCreateUrl
                    }, new ThirdOrderCreate
                    {
                        OrderId = order.Id.ToString().ToLower(),
                        SubTime = order.SubTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        PayTime = order.PaymentTime.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                        Address = new ThirdAddress
                        {
                            Name = order.ReceiptUserName,
                            Phone = order.ReceiptPhone,
                            ProvinceName = order.Province,
                            CityName = order.City,
                            CountyName = order.District,
                            TownName = order.Street,
                            AddressDetail = order.ReceiptAddress,
                            FullAddress = order.Province + order.City + order.District + order.Street + order.ReceiptAddress,
                            ZipCode = order.RecipientsZipCode
                        },
                        OrderSkus = orderItemList.Select(p => new ThirdOrderCreateSku
                        {
                            SkuId = p.JDCode,
                            Name = p.Name,
                            Number = p.Number,
                            Price = p.CostPrice
                        }).ToList()
                    }, ref jsonStr);
                    #endregion
                    #region 保存ThirdECOrderJournal
                    var thirdOrderJournal = new ThirdECOrderJournal
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        OrderCode = order.Code,
                        SubTime = DateTime.Now,
                        SubId = order.SubId,
                        Name = "提交订单",
                        Details = response.Msg,
                        Json = jsonStr,
                        EntityState = EntityState.Added
                    };
                    ContextFactory.CurrentThreadContext.SaveObject(thirdOrderJournal);
                    #endregion
                    #region 保存错误日志
                    if (!response.Successed)
                    {
                        SaveErrorLog(new ThirdECOrderErrorLogDTO
                        {
                            Content = response.Msg,
                            AppId = order.AppId,
                            OrderId = order.Id,
                            OrderCode = order.Code,
                            AppName = order.AppName,
                            CommodityNames = string.Join("、", orderItemList.Select(x => x.Name).ToList()),
                            Json = jsonStr
                        });
                        return;
                    }
                    #endregion
                    #region 保存ThirdECOrder
                    var thirdOrder = new ThirdECOrder
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        OrderCode = order.Code,
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
                        StateName = "下单成功",
                        EntityState = EntityState.Added
                    };
                    ContextFactory.CurrentThreadContext.SaveObject(thirdOrder);
                    #endregion
                    int count = ContextFactory.CurrentThreadContext.SaveChanges();
                    if (count == 0) LogHelper.Error(string.Format("ThirdECOrderHelper.CreateOrder创建第三方电商订单失败,入参:{0}", param));
                }
                catch (Exception ex)
                {
                    LogHelper.Error(string.Format("ThirdECOrderHelper.CreateOrder创建第三方电商订单异常,入参:{0}", param), ex);
                }
            });
        }

        /// <summary>
        /// 取消第三方电商订单
        /// </summary>
        /// <param name="orderId"></param>
        public static ResultDTO CancelOrder(Guid orderId)
        {
            string param = string.Format("orderId={0}", orderId);
            LogHelper.Debug(string.Format("ThirdECOrderHelper.CancelOrder取消第三方电商订单,入参:{0}", param));
            if (orderId == Guid.Empty) return new ResultDTO { Message = "参数有误" };
            var result = new ResultDTO { Message = "操作失败，请稍后重试", ResultCode = -1 };
            try
            {
                #region 判断是否第三方电商订单及获取订单信息
                var order = ThirdECOrder.ObjectSet().FirstOrDefault(o => o.OrderId == orderId);
                if (order == null) return result;
                var thirdECommerce = ThirdECommerceHelper.GetThirdECommerce(order.AppId);
                if (thirdECommerce == null || string.IsNullOrEmpty(thirdECommerce.OpenApiKey)
                    || string.IsNullOrEmpty(thirdECommerce.OpenApiCallerId)
                    || string.IsNullOrEmpty(thirdECommerce.OrderCancelUrl)) return result;
                if (order.CancelResultTime.HasValue) return result;
                #endregion
                order.CancelApplyTime = DateTime.Now;
                #region 调用第三方电商订单取消接口
                var jsonStr = string.Empty;
                var response = ThirdECommerceSV.CancelOrder(new ThirdApiInfo
                {
                    Apikey = thirdECommerce.OpenApiKey,
                    CallerId = thirdECommerce.OpenApiCallerId,
                    ApiUrl = thirdECommerce.OrderCancelUrl
                }, orderId.ToString(), ref jsonStr);
                #endregion
                var isSuccess = response.Successed && response.Result != null;
                #region 修改ThirdECOrder
                if (isSuccess)
                {
                    order.CancelResultTime = DateTime.Now;
                    if (response.Result.CancelStatus == 0)
                    {
                        order.StateName = "不允许取消";
                        result = new ResultDTO { Message = response.Result.RejectReason, ResultCode = 1 };
                    }
                    else if (response.Result.CancelStatus == 1)
                    {
                        order.StateName = "允许取消";
                        // 模拟订单取消审核结果回调
                        Timer timer = null;
                        timer = new Timer(_ =>
                        {
                            CancelOrderCallback(JsonConvert.SerializeObject(new { OrderId = order.Id.ToString().ToLower(), CancelStatus = 1 }));
                            timer.Dispose();
                        }, null, 2000, System.Threading.Timeout.Infinite);
                        result = new ResultDTO { isSuccess = true, Message = response.Result.RejectReason, ResultCode = 0 };
                    }
                    else if (response.Result.CancelStatus == 2)
                    {
                        order.StateName = "待审核";
                        result = new ResultDTO { isSuccess = true, Message = "请等待审核", ResultCode = 2 };
                    }
                    order.StateDesc = response.Result.RejectReason;
                }
                else
                {
                    ContextFactory.ReleaseContextSession();
                    order.StateName = "取消订单申请失败";
                    order.StateDesc = response.Msg;
                }
                #endregion
                #region 保存ThirdECOrderJournal
                var thirdOrderJournal = new ThirdECOrderJournal
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.OrderId,
                    OrderCode = order.OrderCode,
                    SubTime = DateTime.Now,
                    SubId = order.SubId,
                    Name = "取消订单申请",
                    Details = response.Msg,
                    Json = jsonStr,
                    EntityState = EntityState.Added
                };
                ContextFactory.CurrentThreadContext.SaveObject(thirdOrderJournal);
                #endregion
                if (!isSuccess) //失败则只保存日志
                {
                    int count = ContextFactory.CurrentThreadContext.SaveChanges();
                    if (count == 0) LogHelper.Error(string.Format("ThirdECOrderHelper.CancelOrder取消第三方电商订单数据保存失败,入参:{0}", param));
                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ThirdECOrderHelper.CancelOrder取消第三方电商订单异常,入参:{0}", param), ex);
                return result;
            }
        }

        /// <summary>
        /// 判断是否第三方电商订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static bool IsThirdECOrder(Guid orderId)
        {
            string param = string.Format("orderId={0}", orderId);
            LogHelper.Debug(string.Format("ThirdECommerceOrderHelper.IsThirdECOrder判断是否第三方电商订单,入参:{0}", param));
            if (orderId == Guid.Empty) return false;
            try
            {
                return ThirdECOrder.ObjectSet().Any(p => p.OrderId == orderId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ThirdECommerceOrderHelper.IsThirdECOrder判断是否第三方电商订单异常,入参:{0}", param), ex);
            }
            return false;
        }

        /// <summary>
        /// 第三方电商获取订单包裹商品物流信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static List<ThirdECOrderPackageSkuDTO> GetThirdECOrderPackageSkuList(Guid orderId)
        {
            if (orderId == Guid.Empty) return new List<ThirdECOrderPackageSkuDTO>();
            try
            {
                return ThirdECOrderPackageSku.ObjectSet()
                    .Where(p => p.OrderId == orderId)
                    .ToList()
                    .Select(p => p.ToEntityData())
                    .ToList();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ThirdECommerceOrderHelper.GetThirdECOrderPackageSkuList第三方电商获取订单包裹商品物流信息异常,入参:orderId={0}", orderId), ex);
            }
            return new List<ThirdECOrderPackageSkuDTO>();
        }

        /// <summary>
        /// 第三方电商获取订单项物流轨迹信息
        /// </summary>
        /// <param name="orderItemId"></param>
        /// <returns></returns>
        public static ThirdOrderPackageExpress GetExpressInfo(Guid orderItemId)
        {
            if (orderItemId == Guid.Empty) return null;
            try
            {
                var expressSku = ThirdECOrderPackageSku.ObjectSet().Where(p => p.OrderItemId == orderItemId).FirstOrDefault();
                if (expressSku == null) return null;
                var express = new ThirdOrderPackageExpress { ExpressCompany = expressSku.ExpressCompany, ExpressNo = expressSku.ExpressNo, ExpressTraceList = new List<ThirdExpressTrace>() };
                ThirdECExpressTrace.ObjectSet().Where(p => p.OrderId == expressSku.OrderId && p.PackageId == expressSku.PackageId
                    && p.ExpressCompany == expressSku.ExpressCompany && p.ExpressNo == expressSku.ExpressNo).ToList().ForEach(p =>
                    {
                        express.ExpressTraceList.Add(new ThirdExpressTrace
                        {
                            Desc = p.Desc,
                            Time = p.Time.ToString("yyyy-MM-dd HH:mm:ss")
                        });
                    });
                return express;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ThirdECommerceOrderHelper.GetExpressInfo第三方电商获取订单项物流轨迹信息异常,入参:orderItemId={0}", orderItemId), ex);
            }
            return null;
        }

        #region 回调

        /// <summary>
        /// 第三方电商订单取消审核结果回调
        /// </summary>
        /// <param name="result"></param>
        public static ThirdResponse CancelOrderCallback(string resultJsonStr)
        {
            LogHelper.Debug("ThirdECommerceOrderHelper.CancelOrderCallback第三方电商订单取消审核结果回调，Input:" + resultJsonStr);
            if (string.IsNullOrEmpty(resultJsonStr)) return new ThirdResponse { Code = 10200, Msg = "缺少参数orderCancelResult" };
            var result = new ThirdResponse { Code = 200, Msg = "ok" };
            try
            {
                var cancelResult = JsonConvert.DeserializeObject<ThirdOrderCancelResultCallBack>(resultJsonStr);
                if (cancelResult == null) return new ThirdResponse { Code = 10201, Msg = "非法参数orderCancelResult" };
                Guid orderId;
                DateTime auditTime;
                Guid.TryParse(cancelResult.OrderId, out orderId);
                DateTime.TryParse(cancelResult.AuditTime, out auditTime);
                if (orderId == Guid.Empty) return new ThirdResponse { Code = 10202, Msg = "非法参数OrderId" };
                if (!new List<int> { 0, 1 }.Contains(cancelResult.CancelStatus)) return new ThirdResponse { Code = 10203, Msg = "非法参数CancelStatus" };
                if (cancelResult.CancelStatus == 0 && string.IsNullOrEmpty(cancelResult.RejectReason)) return new ThirdResponse { Code = 10204, Msg = "非法参数RejectReason" };
                if (auditTime == DateTime.MinValue) return new ThirdResponse { Code = 10205, Msg = "非法参数CancelStatus" };
                #region 判断是否第三方电商订单及获取订单信息
                var order = ThirdECOrder.ObjectSet().FirstOrDefault(o => o.OrderId == orderId);
                if (order == null) return new ThirdResponse { Code = 10206, Msg = "未找到此订单" };
                if (order.CancelCallBackTime.HasValue) return result;
                #endregion
                #region 处理退款
                var errorMessage = string.Empty;
                OrderSV.UnLockOrder(orderId);
                if (cancelResult.CancelStatus == 0)
                {
                    var optResult = OrderHelper.RejectOrderRefund(orderId, cancelResult.RejectReason);
                    if (optResult.ResultCode != 0)
                    {
                        errorMessage = optResult.Message;
                        LogHelper.Error("ThirdECommerceOrderHelper.CancelOrderCallback 取消订单失败，RejectOrderRefund返回：" + JsonConvert.SerializeObject(optResult));
                        result = new ThirdResponse { Code = 10207, Msg = "内部异常" };
                    }
                    else
                    {
                        order.StateName = "不允许取消";
                        order.CancelCallBackTime = auditTime;
                        order.StateDesc = "订单取消审核结果回调:不允许取消," + cancelResult.RejectReason;
                    }
                }
                else
                {
                    order.StateName = "允许取消";
                    var optResult = OrderHelper.ApproveCancelOrder(orderId);
                    if (optResult.ResultCode != 0)
                    {
                        errorMessage = optResult.Message;
                        LogHelper.Error("ThirdECommerceOrderHelper.CancelOrderCallback 取消订单失败，ApproveCancelOrder返回：" + JsonConvert.SerializeObject(optResult));
                        result = new ThirdResponse { Code = 10208, Msg = "内部异常" };
                    }
                    else
                    {
                        order.StateName = "允许取消";
                        order.CancelCallBackTime = auditTime;
                        order.StateDesc = "订单取消审核结果回调:允许取消";
                    }
                }
                #endregion
                #region 保存ThirdECOrderJournal
                var thirdOrderJournal = new ThirdECOrderJournal
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.OrderId,
                    OrderCode = order.OrderCode,
                    SubTime = DateTime.Now,
                    SubId = order.SubId,
                    Name = "取消订单审核结果回调",
                    Details = result.Successed ? order.StateDesc : errorMessage,
                    Json = resultJsonStr,
                    EntityState = EntityState.Added
                };
                ContextFactory.CurrentThreadContext.SaveObject(thirdOrderJournal);
                #endregion
                int count = ContextFactory.CurrentThreadContext.SaveChanges();
                if (count == 0)
                {
                    LogHelper.Error(string.Format("ThirdECommerceOrderHelper.CancelOrderCallback第三方电商订单取消审核结果回调数据保存失败,入参:{0}", resultJsonStr));
                    return new ThirdResponse { Code = 10209, Msg = "内部异常" };
                }
            }
            catch (JsonReaderException ex)
            {
                LogHelper.Error("ThirdECommerceOrderHelper.CancelOrderCallback第三方电商订单取消审核结果回调反序列化异常，Input:" + resultJsonStr, ex);
                return new ThirdResponse<ThirdOrderCancelResult> { Msg = "反序列化异常" };
            }
            catch (Exception ex)
            {
                LogHelper.Error("ThirdECommerceOrderHelper.CancelOrderCallback第三方电商订单取消审核结果回调异常，Input:" + resultJsonStr, ex);
                return new ThirdResponse { Code = 10210, Msg = "内部异常" };
            }
            return result;
        }

        /// <summary>
        /// 第三方电商商品发货信息回调
        /// </summary>
        /// <param name="result"></param>
        public static ThirdResponse DeliverOrderCallback(string packageJsonStr)
        {
            LogHelper.Debug("ThirdECommerceOrderHelper.DeliverOrderCallback第三方电商商品发货信息回调，Input:" + packageJsonStr);
            if (string.IsNullOrEmpty(packageJsonStr)) return new ThirdResponse { Code = 10300, Msg = "缺少参数orderPackage" };
            try
            {
                var packageResult = JsonConvert.DeserializeObject<ThirdOrderPackage>(packageJsonStr);
                if (packageResult == null) return new ThirdResponse { Code = 10301, Msg = "非法参数orderPackage" };
                Guid orderId;
                Guid.TryParse(packageResult.OrderId, out orderId);
                DateTime expCreateTime;
                DateTime.TryParse(packageResult.ExpCreateTime, out expCreateTime);
                if (orderId == Guid.Empty) return new ThirdResponse { Code = 10302, Msg = "非法参数OrderId" };
                if (string.IsNullOrEmpty(packageResult.ExpressCompany)) return new ThirdResponse { Code = 10303, Msg = "非法参数ExpressCompany" };
                if (string.IsNullOrEmpty(packageResult.ExpressNo)) return new ThirdResponse { Code = 10304, Msg = "非法参数ExpressNo" };
                if (packageResult.SkuIdList == null || packageResult.SkuIdList.Count == 0) return new ThirdResponse { Code = 10305, Msg = "非法参数SkuIdList" };
                if (expCreateTime == DateTime.MinValue) return new ThirdResponse { Code = 10306, Msg = "非法参数ExpCreateTime" };
                #region 判断是否第三方电商订单及获取订单信息
                var order = ThirdECOrder.ObjectSet().FirstOrDefault(o => o.OrderId == orderId);
                if (order == null) return new ThirdResponse { Code = 10307, Msg = "未找到此订单" };
                var orderItemList = OrderItem.ObjectSet()
                    .Where(p => p.CommodityOrderId == orderId)
                    .Select(p => new
                    {
                        SkuId = p.JDCode,
                        OrderItemId = p.Id,
                        p.CommodityId,
                        CommodityStockId = p.CommodityStockId ?? Guid.Empty,
                    }).ToList();
                if (orderItemList.Count == 0) return new ThirdResponse { Code = 10308, Msg = "未找到订单sku" };
                #endregion
                #region ThirdECOrderPackage和ThirdECOrderPackageSku
                var package = ThirdECOrderPackage.ObjectSet().FirstOrDefault(p => p.OrderId == orderId
                    && p.ExpressCompany == packageResult.ExpressCompany && p.ExpressNo == packageResult.ExpressNo);
                if (package == null)
                {
                    package = new ThirdECOrderPackage
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.OrderId,
                        OrderCode = order.OrderCode,
                        ExpressCompany = packageResult.ExpressCompany,
                        ExpressNo = packageResult.ExpressNo,
                        ExpCreateTime = DateTime.Parse(packageResult.ExpCreateTime),
                        EntityState = EntityState.Added
                    };
                    ContextFactory.CurrentThreadContext.SaveObject(package);
                    packageResult.SkuIdList.ForEach(skuId =>
                    {
                        var orderItem = orderItemList.FirstOrDefault(p => p.SkuId == skuId);
                        if (orderItem == null) return;
                        var packageSku = new ThirdECOrderPackageSku
                        {
                            Id = Guid.NewGuid(),
                            OrderId = order.OrderId,
                            OrderCode = order.OrderCode,
                            OrderItemId = orderItem.OrderItemId,
                            CommodityId = orderItem.CommodityId,
                            CommodityStockId = orderItem.CommodityStockId,
                            PackageId = package.Id,
                            SkuId = skuId,
                            ExpressCompany = packageResult.ExpressCompany,
                            ExpressNo = packageResult.ExpressNo,
                            EntityState = EntityState.Added
                        };
                        ContextFactory.CurrentThreadContext.SaveObject(packageSku);
                    });
                }
                #endregion
                #region ThirdECOrderJournal
                var packageJournal = new ThirdECOrderJournal
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.OrderId,
                    OrderCode = order.OrderCode,
                    Name = "订单商品发货",
                    Details = packageResult.ExpressCompany + "|" + packageResult.ExpressNo,
                    Json = packageJsonStr,
                    EntityState = EntityState.Added
                };
                ContextFactory.CurrentThreadContext.SaveObject(packageJournal);
                #endregion
                int count = ContextFactory.CurrentThreadContext.SaveChanges();
                if (count > 0)
                {
                    #region 订单所有商品发货后更新订单状态到已发货
                    if (ThirdECOrderPackageSku.ObjectSet().Count(p => p.OrderId == orderId) >= orderItemList.Select(x => x.SkuId).Distinct().Count())
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
                    LogHelper.Error(string.Format("ThirdECommerceOrderHelper.DeliverOrderCallback第三方电商商品发货信息回调失败,入参:{0}", packageJsonStr));
                    return new ThirdResponse { Code = 10309, Msg = "内部异常" };
                }
            }
            catch (JsonReaderException ex)
            {
                LogHelper.Error("ThirdECommerceOrderHelper.DeliverOrderCallback第三方电商商品发货信息回调反序列化异常，Input:" + packageJsonStr, ex);
                return new ThirdResponse<ThirdOrderCancelResult> { Msg = "反序列化异常" };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ThirdECommerceOrderHelper.DeliverOrderCallback第三方电商商品发货信息回调异常,入参:{0}", packageJsonStr), ex);
                return new ThirdResponse { Code = 10310, Msg = "内部异常" };
            }
            return new ThirdResponse { Code = 200, Msg = "ok" };
        }

        /// <summary>
        /// 第三方电商物流追踪信息回调
        /// </summary>
        /// <param name="expressJsonStr"></param>
        /// <returns></returns>
        public static ThirdResponse ExpressTraceCallback(string expressJsonStr)
        {
            LogHelper.Debug("ThirdECommerceOrderHelper.ExpressTraceCallback第三方电商物流追踪信息回调，Input:" + expressJsonStr);
            if (string.IsNullOrEmpty(expressJsonStr)) return new ThirdResponse { Code = 10400, Msg = "缺少参数expressTrace" };
            try
            {
                var expressResult = JsonConvert.DeserializeObject<ThirdOrderPackageExpress>(expressJsonStr);
                if (expressResult == null) return new ThirdResponse { Code = 10401, Msg = "非法参数expressTrace" };
                Guid orderId;
                Guid.TryParse(expressResult.OrderId, out orderId);
                if (orderId == Guid.Empty) return new ThirdResponse { Code = 10402, Msg = "非法参数OrderId" };
                if (string.IsNullOrEmpty(expressResult.ExpressCompany)) return new ThirdResponse { Code = 10403, Msg = "非法参数ExpressCompany" };
                if (string.IsNullOrEmpty(expressResult.ExpressNo)) return new ThirdResponse { Code = 10404, Msg = "非法参数ExpressNo" };
                if (expressResult.ExpressTraceList == null || expressResult.ExpressTraceList.Count == 0) return new ThirdResponse { Code = 10405, Msg = "非法参数ExpressTraceList" };
                #region 判断是否第三方电商订单及获取订单信息
                var package = ThirdECOrderPackage.ObjectSet().FirstOrDefault(p => p.OrderId == orderId
                    && p.ExpressCompany == expressResult.ExpressCompany && p.ExpressNo == expressResult.ExpressNo);
                if (package == null) return new ThirdResponse { Code = 10406, Msg = "未找到此包裹" };
                #endregion
                #region ThirdECExpressTrace
                var descFlag = false;
                var timeFlag = false;
                expressResult.ExpressTraceList.ForEach(trace =>
                {
                    DateTime traceTime;
                    DateTime.TryParse(trace.Time, out traceTime);
                    if (string.IsNullOrEmpty(trace.Desc)) { descFlag = false; return; }
                    else if (traceTime == DateTime.MinValue) { timeFlag = false; return; }
                    var express = ThirdECExpressTrace.ObjectSet().FirstOrDefault(p => p.OrderId == orderId
                        && p.ExpressCompany == expressResult.ExpressCompany && p.ExpressNo == expressResult.ExpressNo
                        && p.Desc == trace.Desc && p.Time == traceTime);
                    if (express == null)
                    {
                        express = new ThirdECExpressTrace
                        {
                            Id = Guid.NewGuid(),
                            OrderId = package.OrderId,
                            OrderCode = package.OrderCode,
                            PackageId = package.Id,
                            ExpressCompany = package.ExpressCompany,
                            ExpressNo = package.ExpressNo,
                            Time = traceTime,
                            Desc = trace.Desc,
                            EntityState = EntityState.Added
                        };
                        ContextFactory.CurrentThreadContext.SaveObject(express);
                    }
                });
                if (descFlag) return new ThirdResponse { Code = 10407, Msg = "非法参数ExpressTraceList" };
                if (timeFlag) return new ThirdResponse { Code = 10408, Msg = "非法参数ExpressTraceList" };
                #endregion
                int count = ContextFactory.CurrentThreadContext.SaveChanges();
                if (count == 0)
                {
                    LogHelper.Error(string.Format("ThirdECommerceOrderHelper.ExpressTraceCallback第三方电商物流追踪信息回调失败,入参:{0}", expressJsonStr));
                    return new ThirdResponse { Code = 10409, Msg = "内部异常" };
                }
            }
            catch (JsonReaderException ex)
            {
                LogHelper.Error("ThirdECommerceOrderHelper.ExpressTraceCallback第三方电商物流追踪信息回调反序列化异常，Input:" + expressJsonStr, ex);
                return new ThirdResponse<ThirdOrderCancelResult> { Msg = "反序列化异常" };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ThirdECommerceOrderHelper.ExpressTraceCallback第三方电商物流追踪信息回调异常,入参:{0}", expressJsonStr), ex);
                return new ThirdResponse { Code = 10410, Msg = "内部异常" };
            }
            return new ThirdResponse { Code = 200, Msg = "ok" };
        }

        /// <summary>
        /// 第三方电商商品收货信息回调
        /// </summary>
        /// <param name="packageJsonStr"></param>
        /// <returns></returns>
        public static ThirdResponse ConfirmOrderCallback(string packageJsonStr)
        {
            LogHelper.Debug("ThirdECommerceOrderHelper.ConfirmOrderCallback第三方电商商品收货信息回调，Input:" + packageJsonStr);
            if (string.IsNullOrEmpty(packageJsonStr)) return new ThirdResponse { Code = 10500, Msg = "缺少参数orderPackage" };
            try
            {
                var packageResult = JsonConvert.DeserializeObject<ThirdOrderPackage>(packageJsonStr);
                if (packageResult == null) return new ThirdResponse { Code = 10501, Msg = "非法参数orderPackage" };
                Guid orderId;
                Guid.TryParse(packageResult.OrderId, out orderId);
                DateTime confirmTime;
                DateTime.TryParse(packageResult.ConfirmTime, out confirmTime);
                if (orderId == Guid.Empty) return new ThirdResponse { Code = 10502, Msg = "非法参数OrderId" };
                if (string.IsNullOrEmpty(packageResult.ExpressCompany)) return new ThirdResponse { Code = 10503, Msg = "非法参数ExpressCompany" };
                if (string.IsNullOrEmpty(packageResult.ExpressNo)) return new ThirdResponse { Code = 10504, Msg = "非法参数ExpressNo" };
                if (confirmTime == DateTime.MinValue) return new ThirdResponse { Code = 10505, Msg = "非法参数ConfirmTime" };
                #region 判断是否第三方电商订单及获取订单信息
                var order = ThirdECOrder.ObjectSet().FirstOrDefault(o => o.OrderId == orderId);
                if (order == null) return new ThirdResponse { Code = 10506, Msg = "未找到此订单" };
                #endregion
                #region ThirdECOrderPackage
                var package = ThirdECOrderPackage.ObjectSet().FirstOrDefault(p => p.OrderId == orderId
                    && p.ExpressCompany == packageResult.ExpressCompany && p.ExpressNo == packageResult.ExpressNo);
                if (package != null)
                {
                    package.ConfirmTime = confirmTime;
                }
                #endregion
                #region ThirdECOrderJournal
                var packageJournal = new ThirdECOrderJournal
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.OrderId,
                    OrderCode = order.OrderCode,
                    Name = "订单商品收货",
                    Details = packageResult.ExpressCompany + "|" + packageResult.ExpressNo,
                    Json = packageJsonStr,
                    EntityState = EntityState.Added
                };
                ContextFactory.CurrentThreadContext.SaveObject(packageJournal);
                #endregion
                int count = ContextFactory.CurrentThreadContext.SaveChanges();
                if (count > 0)
                {
                    #region 订单所有商品收货后更新订单状态到已收货

                    #endregion
                }
                else
                {
                    LogHelper.Error(string.Format("ThirdECommerceOrderHelper.ConfirmOrderCallback第三方电商商品收货信息回调失败,入参:{0}", packageJsonStr));
                    return new ThirdResponse { Code = 10507, Msg = "内部异常" };
                }
            }
            catch (JsonReaderException ex)
            {
                LogHelper.Error("ThirdECommerceOrderHelper.ConfirmOrderCallback第三方电商商品收货信息回调反序列化异常，Input:" + packageJsonStr, ex);
                return new ThirdResponse<ThirdOrderCancelResult> { Msg = "反序列化异常" };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ThirdECommerceOrderHelper.ConfirmOrderCallback第三方电商商品收货信息回调异常,入参:{0}", packageJsonStr), ex);
                return new ThirdResponse { Code = 10508, Msg = "内部异常" };
            }
            return new ThirdResponse { Code = 200, Msg = "ok" };
        }

        #endregion
    }
}