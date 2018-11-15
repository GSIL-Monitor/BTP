using System;
using System.Data;
using System.Linq;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.CustomDTO.ThirdECommerce;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;
using Newtonsoft.Json;

namespace Jinher.AMP.BTP.TPS.Helper
{
    /// <summary>
    /// 第三方电商(非京东大客户和网易严选)标准接口接入-售后服务帮助类
    /// </summary>
    public class ThirdECommerceServiceHelper
    {
        /// <summary>
        /// 第三方电商发起售后服务申请
        /// </summary>
        /// <returns></returns>
        public static ResultDTO CreateService(CommodityOrder order, OrderItem orderItem, OrderRefundAfterSales refund)
        {
            string param = string.Format("orderId={0}&refundId={1}", order.Id, refund.Id);
            LogHelper.Debug(string.Format("ThirdECommerceServiceHelper.CreateService第三方电商发起售后服务申请,入参:{0}", param));
            var result = new ResultDTO { Message = "操作失败，请稍后重试", ResultCode = -1 };
            try
            {
                #region 判断是否第三方电商订单及获取订单信息
                var orderSku = ThirdECOrderPackageSku.ObjectSet().FirstOrDefault(o => o.OrderId == order.Id && o.OrderItemId == orderItem.Id);
                if (orderSku == null) return result;
                var thirdECommerce = ThirdECommerceHelper.GetThirdECommerce(order.AppId);
                if (thirdECommerce == null || string.IsNullOrEmpty(thirdECommerce.OpenApiKey)
                    || string.IsNullOrEmpty(thirdECommerce.OpenApiCallerId)
                    || string.IsNullOrEmpty(thirdECommerce.ServiceCreateUrl)) return result;
                if (ThirdECService.ObjectSet().Any(p => p.OrderRefundAfterSalesId == refund.Id)) return result;
                #endregion
                #region 调用第三方发起售后服务申请接口
                var jsonStr = string.Empty;
                var response = ThirdECommerceSV.CreateService(new ThirdApiInfo
                {
                    Apikey = thirdECommerce.OpenApiKey,
                    CallerId = thirdECommerce.OpenApiCallerId,
                    ApiUrl = thirdECommerce.ServiceCreateUrl
                }, new ThirdServiceCreate
                {
                    ServiceId = refund.Id.ToString().ToLower(),
                    OrderId = order.Id.ToString().ToLower(),
                    CustomerName = order.ReceiptUserName,
                    CustomerPhone = order.ReceiptPhone,
                    SkuId = orderSku.SkuId,
                    Number = orderItem.Number,
                    RefundReason = refund.RefundReason,
                    RefundDesc = refund.RefundDesc,
                    RefundImgs = refund.OrderRefundImgs
                }, ref jsonStr);
                #endregion
                var isSuccess = response.Successed && response.Result != null;
                #region 保存ThirdECService
                if (isSuccess)
                {
                    var stateName = string.Empty;
                    var stateDesc = string.Empty;
                    if (response.Result.ServiceStatus == 0)
                    {
                        stateName = "不支持售后";
                        result = new ResultDTO { Message = response.Result.RejectReason, ResultCode = 1 };
                    }
                    else if (response.Result.ServiceStatus == 1)
                    {
                        stateName = "待审核";
                        result = new ResultDTO { isSuccess = true, Message = "请等待审核", ResultCode = 2 };
                    }
                    stateDesc = response.Result.RejectReason;
                    var service = new ThirdECService
                    {
                        Id = Guid.NewGuid(),
                        OrderId = orderSku.OrderId,
                        OrderCode = orderSku.OrderCode,
                        OrderItemId = orderSku.OrderItemId,
                        OrderRefundAfterSalesId = refund.Id,
                        SubId = order.SubId,
                        SkuId = orderSku.SkuId,
                        Number = orderItem.Number,
                        StateName = stateName,
                        StateDesc = stateDesc,
                        EntityState = EntityState.Added
                    };
                    ContextFactory.CurrentThreadContext.SaveObject(service);
                }
                else
                {
                    ContextFactory.ReleaseContextSession();
                }
                #endregion
                #region 保存ThirdECServiceJournal
                var journal = new ThirdECServiceJournal
                {
                    Id = Guid.NewGuid(),
                    OrderId = orderSku.OrderId,
                    OrderCode = orderSku.OrderCode,
                    OrderItemId = orderSku.OrderItemId,
                    OrderRefundAfterSalesId = refund.Id,
                    Name = "发起售后服务申请",
                    Details = response.Msg,
                    Json = jsonStr,
                    EntityState = EntityState.Added
                };
                ContextFactory.CurrentThreadContext.SaveObject(journal);
                #endregion
                if (!isSuccess) //失败则只保存日志
                {
                    int count = ContextFactory.CurrentThreadContext.SaveChanges();
                    if (count == 0) LogHelper.Error(string.Format("ThirdECommerceServiceHelper.CreateService第三方电商发起售后服务申请数据保存失败,入参:{0}", param));
                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ThirdECommerceServiceHelper.CreateService第三方电商发起售后服务申请异常,入参:{0}", param), ex);
                return result;
            }
        }

        /// <summary>
        /// 第三方电商取消售后服务申请
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="orderId"></param>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        public static ResultDTO CancelService(Guid appId, Guid orderId, Guid serviceId)
        {
            string param = string.Format("appId={0}&orderId={1}&serviceId={2}", appId, orderId, serviceId);
            LogHelper.Debug(string.Format("ThirdECommerceServiceHelper.CancelService第三方电商取消售后服务申请,入参:{0}", param));
            var result = new ResultDTO { Message = "操作失败，请稍后重试" };
            try
            {
                #region 判断是否第三方电商售后服务单及获取售后服务单信息
                var service = ThirdECService.ObjectSet().FirstOrDefault(o => o.OrderRefundAfterSalesId == serviceId);
                if (service == null) return result;
                var thirdECommerce = ThirdECommerceHelper.GetThirdECommerce(appId);
                if (thirdECommerce == null || string.IsNullOrEmpty(thirdECommerce.OpenApiKey)
                    || string.IsNullOrEmpty(thirdECommerce.OpenApiCallerId)
                    || string.IsNullOrEmpty(thirdECommerce.ServiceCancelUrl)) return result;
                if (service.UserCancelTime.HasValue) return result;
                #endregion
                #region 调用第三方取消售后服务申请接口
                var jsonStr = string.Empty;
                var response = ThirdECommerceSV.CancelService(new ThirdApiInfo
                {
                    Apikey = thirdECommerce.OpenApiKey,
                    CallerId = thirdECommerce.OpenApiCallerId,
                    ApiUrl = thirdECommerce.ServiceCancelUrl
                }, serviceId.ToString(), ref jsonStr);
                #endregion
                var isSuccess = response.Successed;
                #region 修改ThirdECService
                if (isSuccess)
                {
                    service.UserCancelTime = DateTime.Now;
                    service.StateName = "取消售后服务申请成功";
                    result = new ResultDTO { isSuccess = true };
                }
                else
                {
                    ContextFactory.ReleaseContextSession();
                    service.StateName = "取消售后服务申请失败";
                    result = new ResultDTO { Message = response.Msg ?? "不允许取消" };
                }
                service.StateDesc = response.Msg;
                #endregion
                #region 保存ThirdECServiceJournal
                var journal = new ThirdECServiceJournal
                {
                    Id = Guid.NewGuid(),
                    OrderId = service.OrderId,
                    OrderCode = service.OrderCode,
                    OrderItemId = service.OrderItemId,
                    OrderRefundAfterSalesId = service.OrderRefundAfterSalesId,
                    Name = "取消售后服务申请",
                    Details = response.Msg,
                    Json = jsonStr,
                    EntityState = EntityState.Added
                };
                ContextFactory.CurrentThreadContext.SaveObject(journal);
                #endregion
                if (!isSuccess) //失败则只保存日志
                {
                    int count = ContextFactory.CurrentThreadContext.SaveChanges();
                    if (count == 0) LogHelper.Error(string.Format("ThirdECommerceServiceHelper.CreateService第三方电商取消售后服务申请数据保存失败,入参:{0}", param));
                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ThirdECommerceServiceHelper.CreateService第三方电商取消售后服务申请异常,入参:{0}", param), ex);
                return result;
            }
        }

        #region 回调

        /// <summary>
        /// 第三方电商允许售后结果回调
        /// </summary>
        /// <param name="resultJsonStr"></param>
        public static ThirdResponse AgreeServiceCallback(string resultJsonStr)
        {
            LogHelper.Debug("ThirdECommerceServiceHelper.AgreeServiceCallback第三方电商允许售后结果回调，Input:" + resultJsonStr);
            if (string.IsNullOrEmpty(resultJsonStr)) return new ThirdResponse { Code = 20100, Msg = "缺少参数serviceAgreeResult" };
            var result = new ThirdResponse { Code = 200, Msg = "ok" };
            try
            {
                var serviceResult = JsonConvert.DeserializeObject<ThirdServiceAgreeResult>(resultJsonStr);
                if (serviceResult == null) return new ThirdResponse { Code = 20101, Msg = "非法参数serviceAgreeResult" };
                Guid orderId, serviceId;
                DateTime agreeTime;
                Guid.TryParse(serviceResult.OrderId, out orderId);
                Guid.TryParse(serviceResult.ServiceId, out serviceId);
                DateTime.TryParse(serviceResult.AgreeTime, out agreeTime);
                if (orderId == Guid.Empty) return new ThirdResponse { Code = 20102, Msg = "非法参数OrderId" };
                if (serviceId == Guid.Empty) return new ThirdResponse { Code = 20103, Msg = "非法参数ServiceId" };
                if (agreeTime == DateTime.MinValue) return new ThirdResponse { Code = 20104, Msg = "非法参数AgreeTime" };
                if (serviceResult.Address == null) return new ThirdResponse { Code = 20105, Msg = "非法参数Address" };
                if (string.IsNullOrEmpty(serviceResult.Address.Name)) return new ThirdResponse { Code = 20106, Msg = "缺少参数Address.Name" };
                if (string.IsNullOrEmpty(serviceResult.Address.Phone)) return new ThirdResponse { Code = 20107, Msg = "缺少参数Address.Phone" };
                //if (string.IsNullOrEmpty(serviceResult.Address.ProvinceName)) return new ThirdResponse { Code = 20108, Msg = "缺少参数Address.ProvinceName" };
                //if (string.IsNullOrEmpty(serviceResult.Address.CityName)) return new ThirdResponse { Code = 20109, Msg = "缺少参数Address.CityName" };
                //if (string.IsNullOrEmpty(serviceResult.Address.CountyName)) return new ThirdResponse { Code = 20110, Msg = "缺少参数Address.CountyName" };
                //if (string.IsNullOrEmpty(serviceResult.Address.TownName)) return new ThirdResponse { Code = 20111, Msg = "缺少参数Address.TownName" };
                //if (string.IsNullOrEmpty(serviceResult.Address.AddressDetail)) return new ThirdResponse { Code = 20112, Msg = "缺少参数Address.AddressDetail" };
                if (string.IsNullOrEmpty(serviceResult.Address.FullAddress)) return new ThirdResponse { Code = 20113, Msg = "缺少参数Address.FullAddress" };
                #region 判断是否第三方电商售后服务单及获取售后服务单信息
                var service = ThirdECService.ObjectSet().FirstOrDefault(o => o.OrderId == orderId && o.OrderRefundAfterSalesId == serviceId);
                if (service == null) return new ThirdResponse { Code = 20114, Msg = "未找到此服务单" };
                var orderRefund = OrderRefundAfterSales.ObjectSet().FirstOrDefault(p => p.Id == service.OrderRefundAfterSalesId);
                if (service == null) return new ThirdResponse { Code = 20115, Msg = "未找到此服务单" };
                if (service.AgreeApplyTime.HasValue) return result;
                #endregion
                #region 退款处理
                var refundResult = OrderHelper.ApproveOrderRefundAfterSales(orderId, orderRefund.OrderItemId ?? Guid.Empty);
                if (refundResult.ResultCode == 0)
                {
                    service.AgreeApplyTime = agreeTime;
                    service.StateName = "允许售后";
                    service.StateDesc = "售后服务申请审核结果回调：允许售后";
                    orderRefund.RefundReceiveFullAddress = serviceResult.Address.FullAddress;
                    orderRefund.RefundReceiveMobile = serviceResult.Address.Phone;
                    orderRefund.RefundReceiveName = serviceResult.Address.Name;
                }
                else result = new ThirdResponse { Code = 20116, Msg = "内部异常" };
                #endregion
                #region 保存ThirdECOrderJournal
                var journal = new ThirdECServiceJournal
                {
                    Id = Guid.NewGuid(),
                    OrderId = service.OrderId,
                    OrderCode = service.OrderCode,
                    OrderItemId = service.OrderItemId,
                    OrderRefundAfterSalesId = service.OrderRefundAfterSalesId,
                    Name = "售后服务申请审核结果回调：允许售后",
                    Details = refundResult.ResultCode == 0 ? "售后服务申请审核结果回调：允许售后" : refundResult.Message,
                    Json = resultJsonStr,
                    EntityState = EntityState.Added
                };
                ContextFactory.CurrentThreadContext.SaveObject(journal);
                #endregion
                int count = ContextFactory.CurrentThreadContext.SaveChanges();
                if (count == 0)
                {
                    LogHelper.Error(string.Format("ThirdECommerceServiceHelper.AgreeServiceCallback第三方电商允许售后结果回调数据保存失败,入参:{0}", resultJsonStr));
                    result = new ThirdResponse { Code = 20117, Msg = "内部异常" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("ThirdECommerceServiceHelper.AgreeServiceCallback第三方电商允许售后结果回调异常，Input:" + resultJsonStr, ex);
                result = new ThirdResponse { Code = 20118, Msg = "内部异常" };
            }
            return result;
        }

        /// <summary>
        /// 第三方电商拒绝售后结果回调
        /// </summary>
        /// <param name="resultJsonStr"></param>
        /// <returns></returns>
        public static ThirdResponse RejectServiceCallback(string resultJsonStr)
        {
            LogHelper.Debug("ThirdECommerceServiceHelper.RejectServiceCallback第三方电商拒绝售后结果回调，Input:" + resultJsonStr);
            if (string.IsNullOrEmpty(resultJsonStr)) return new ThirdResponse { Code = 20200, Msg = "缺少参数serviceRejectResult" };
            var result = new ThirdResponse { Code = 200, Msg = "ok" };
            try
            {
                var serviceResult = JsonConvert.DeserializeObject<ThirdServiceRejectResult>(resultJsonStr);
                if (serviceResult == null) return new ThirdResponse { Code = 20201, Msg = "非法参数serviceRejectResult" };
                Guid orderId, serviceId;
                DateTime rejectTime;
                Guid.TryParse(serviceResult.OrderId, out orderId);
                Guid.TryParse(serviceResult.ServiceId, out serviceId);
                DateTime.TryParse(serviceResult.RejectTime, out rejectTime);
                if (orderId == Guid.Empty) return new ThirdResponse { Code = 20202, Msg = "非法参数OrderId" };
                if (serviceId == Guid.Empty) return new ThirdResponse { Code = 20203, Msg = "非法参数ServiceId" };
                if (rejectTime == DateTime.MinValue) return new ThirdResponse { Code = 20204, Msg = "非法参数RejectTime" };
                if (string.IsNullOrEmpty(serviceResult.RejectReason)) return new ThirdResponse { Code = 20205, Msg = "缺少参数RejectReason" };
                #region 判断是否第三方电商售后服务单及获取售后服务单信息
                var service = ThirdECService.ObjectSet().FirstOrDefault(o => o.OrderId == orderId && o.OrderRefundAfterSalesId == serviceId);
                if (service == null) return new ThirdResponse { Code = 20206, Msg = "未找到此服务单" };
                var orderRefund = OrderRefundAfterSales.ObjectSet().FirstOrDefault(p => p.Id == service.OrderRefundAfterSalesId);
                if (service == null) return new ThirdResponse { Code = 20207, Msg = "未找到此服务单" };
                if (service.RejectApplyTime.HasValue) return result;
                #endregion
                #region 退款处理
                var refundResult = OrderHelper.RejectOrderRefundAfterSales(orderId, orderRefund.OrderItemId ?? Guid.Empty, serviceResult.RejectReason);
                if (refundResult.ResultCode == 0)
                {
                    service.RejectApplyTime = rejectTime;
                    service.StateName = "拒绝售后";
                    service.StateDesc = "售后服务申请审核结果回调：拒绝售后";
                }
                else result = new ThirdResponse { Code = 20208, Msg = "内部异常" };
                #endregion
                #region 保存ThirdECOrderJournal
                var journal = new ThirdECServiceJournal
                {
                    Id = Guid.NewGuid(),
                    OrderId = service.OrderId,
                    OrderCode = service.OrderCode,
                    OrderItemId = service.OrderItemId,
                    OrderRefundAfterSalesId = service.OrderRefundAfterSalesId,
                    Name = "售后服务申请审核结果回调：拒绝售后",
                    Details = refundResult.ResultCode == 0 ? "售后服务申请审核结果回调：拒绝售后" : refundResult.Message,
                    Json = resultJsonStr,
                    EntityState = EntityState.Added
                };
                ContextFactory.CurrentThreadContext.SaveObject(journal);
                #endregion
                int count = ContextFactory.CurrentThreadContext.SaveChanges();
                if (count == 0)
                {
                    LogHelper.Error(string.Format("ThirdECommerceServiceHelper.RejectServiceCallback第三方电商拒绝售后结果回调数据保存失败,入参:{0}", resultJsonStr));
                    result = new ThirdResponse { Code = 20209, Msg = "内部异常" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("ThirdECommerceServiceHelper.RejectServiceCallback第三方电商拒绝售后结果回调异常，Input:" + resultJsonStr, ex);
                result = new ThirdResponse { Code = 20210, Msg = "内部异常" };
            }
            return result;
        }

        /// <summary>
        /// 第三方电商售后服务退货物流签收回调
        /// </summary>
        /// <param name="resultJsonStr"></param>
        /// <returns></returns>
        public static ThirdResponse ReceiveMailCallback(string resultJsonStr)
        {
            LogHelper.Debug("ThirdECommerceServiceHelper.ReceiveMailCallback第三方电商售后服务退货物流签收回调，Input:" + resultJsonStr);
            if (string.IsNullOrEmpty(resultJsonStr)) return new ThirdResponse { Code = 20300, Msg = "缺少参数mailReceiveResult" };
            var result = new ThirdResponse { Code = 200, Msg = "ok" };
            try
            {
                var mailResult = JsonConvert.DeserializeObject<ThirdMailReceiveResult>(resultJsonStr);
                if (mailResult == null) return new ThirdResponse { Code = 20301, Msg = "非法参数mailReceiveResult" };
                Guid orderId, serviceId;
                DateTime receiveTime;
                Guid.TryParse(mailResult.OrderId, out orderId);
                Guid.TryParse(mailResult.ServiceId, out serviceId);
                DateTime.TryParse(mailResult.ReceiveTime, out receiveTime);
                if (orderId == Guid.Empty) return new ThirdResponse { Code = 20302, Msg = "非法参数OrderId" };
                if (orderId == Guid.Empty) return new ThirdResponse { Code = 20303, Msg = "非法参数ServiceId" };
                if (receiveTime == DateTime.MinValue) return new ThirdResponse { Code = 20304, Msg = "非法参数ReceiveTime" };
                #region 判断是否第三方电商售后服务单及获取售后服务单信息
                var service = ThirdECService.ObjectSet().FirstOrDefault(o => o.OrderId == orderId && o.OrderRefundAfterSalesId == serviceId);
                if (service == null) return new ThirdResponse { Code = 20305, Msg = "未找到此服务单" };
                if (service.MailReceiveTime.HasValue) return result;
                service.MailReceiveTime = receiveTime;
                service.StateName = "退货物流签收";
                service.StateDesc = "退货物流签收";
                #endregion
                #region 保存ThirdECOrderJournal
                var journal = new ThirdECServiceJournal
                {
                    Id = Guid.NewGuid(),
                    OrderId = service.OrderId,
                    OrderCode = service.OrderCode,
                    OrderItemId = service.OrderItemId,
                    OrderRefundAfterSalesId = service.OrderRefundAfterSalesId,
                    Name = "退货物流签收",
                    Details = "退货物流签收",
                    Json = resultJsonStr,
                    EntityState = EntityState.Added
                };
                ContextFactory.CurrentThreadContext.SaveObject(journal);
                #endregion
                int count = ContextFactory.CurrentThreadContext.SaveChanges();
                if (count == 0)
                {
                    LogHelper.Error(string.Format("ThirdECommerceServiceHelper.ReceiveMailCallback第三方电商售后服务退货物流签收回调数据保存失败,入参:{0}", resultJsonStr));
                    result = new ThirdResponse { Code = 20206, Msg = "内部异常" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("ThirdECommerceServiceHelper.ReceiveMailCallback第三方电商售后服务退货物流签收回调异常，Input:" + resultJsonStr, ex);
                result = new ThirdResponse { Code = 20207, Msg = "内部异常" };
            }
            return result;
        }

        /// <summary>
        /// 第三方电商售后服务超过X天未收到退货物流回调
        /// </summary>
        /// <param name="resultJsonStr"></param>
        /// <returns></returns>
        public static ThirdResponse NotReceiveMailCallback(string resultJsonStr)
        {
            LogHelper.Debug("ThirdECommerceServiceHelper.NotReceiveMailCallback第三方电商售后服务超过X天未收到退货物流回调，Input:" + resultJsonStr);
            if (string.IsNullOrEmpty(resultJsonStr)) return new ThirdResponse { Code = 20400, Msg = "缺少参数mailNotReceiveResult" };
            var result = new ThirdResponse { Code = 200, Msg = "ok" };
            try
            {
                var mailResult = JsonConvert.DeserializeObject<ThirdMailNotReceiveResult>(resultJsonStr);
                if (mailResult == null) return new ThirdResponse { Code = 20401, Msg = "非法参数mailNotReceiveResult" };
                Guid orderId, serviceId;
                DateTime notReceiveTime;
                Guid.TryParse(mailResult.OrderId, out orderId);
                Guid.TryParse(mailResult.ServiceId, out serviceId);
                DateTime.TryParse(mailResult.NotifyTime, out notReceiveTime);
                if (orderId == Guid.Empty) return new ThirdResponse { Code = 20402, Msg = "非法参数OrderId" };
                if (orderId == Guid.Empty) return new ThirdResponse { Code = 20403, Msg = "非法参数ServiceId" };
                if (notReceiveTime == DateTime.MinValue) return new ThirdResponse { Code = 20404, Msg = "非法参数ReceiveTime" };
                if (mailResult.Days <= 0) return new ThirdResponse { Code = 20405, Msg = "非法参数Days" };
                #region 判断是否第三方电商售后服务单及获取售后服务单信息
                var service = ThirdECService.ObjectSet().FirstOrDefault(o => o.OrderId == orderId && o.OrderRefundAfterSalesId == serviceId);
                if (service == null) return new ThirdResponse { Code = 20406, Msg = "未找到此服务单" };
                if (service.MailNotReceiveTime.HasValue) return result;
                service.MailNotReceiveTime = notReceiveTime;
                service.StateName = "超过" + mailResult.Days + "天未收到退货物流";
                service.StateDesc = service.StateDesc;
                #endregion
                #region 保存ThirdECOrderJournal
                var journal = new ThirdECServiceJournal
                {
                    Id = Guid.NewGuid(),
                    OrderId = service.OrderId,
                    OrderCode = service.OrderCode,
                    OrderItemId = service.OrderItemId,
                    OrderRefundAfterSalesId = service.OrderRefundAfterSalesId,
                    Name = service.StateDesc,
                    Details = service.StateDesc,
                    Json = resultJsonStr,
                    EntityState = EntityState.Added
                };
                ContextFactory.CurrentThreadContext.SaveObject(journal);
                #endregion
                int count = ContextFactory.CurrentThreadContext.SaveChanges();
                if (count == 0)
                {
                    LogHelper.Error(string.Format("ThirdECommerceServiceHelper.NotReceiveMailCallback第三方电商售后服务超过X天未收到退货物流回调数据保存失败,入参:{0}", resultJsonStr));
                    result = new ThirdResponse { Code = 20207, Msg = "内部异常" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("ThirdECommerceServiceHelper.NotReceiveMailCallback第三方电商售后服务超过X天未收到退货物流回调异常，Input:" + resultJsonStr, ex);
                result = new ThirdResponse { Code = 20208, Msg = "内部异常" };
            }
            return result;
        }

        /// <summary>
        /// 第三方电商售后服务退货结果回调
        /// </summary>
        /// <param name="resultJsonStr"></param>
        /// <returns></returns>
        public static ThirdResponse RefundResultCallback(string resultJsonStr)
        {
            LogHelper.Debug("ThirdECommerceServiceHelper.RefundResultCallback第三方电商售后服务退货结果回调，Input:" + resultJsonStr);
            if (string.IsNullOrEmpty(resultJsonStr)) return new ThirdResponse { Code = 20500, Msg = "缺少参数serviceRefundResult" };
            var result = new ThirdResponse { Code = 200, Msg = "ok" };
            try
            {
                var serviceResult = JsonConvert.DeserializeObject<ThirdServiceRefundResult>(resultJsonStr);
                if (serviceResult == null) return new ThirdResponse { Code = 20501, Msg = "非法参数serviceRejectResult" };
                Guid orderId, serviceId;
                DateTime auditTime;
                Guid.TryParse(serviceResult.OrderId, out orderId);
                Guid.TryParse(serviceResult.ServiceId, out serviceId);
                DateTime.TryParse(serviceResult.AuditTime, out auditTime);
                if (orderId == Guid.Empty) return new ThirdResponse { Code = 20502, Msg = "非法参数OrderId" };
                if (serviceId == Guid.Empty) return new ThirdResponse { Code = 20503, Msg = "非法参数ServiceId" };
                if (string.IsNullOrEmpty(serviceResult.SkuId)) return new ThirdResponse { Code = 20504, Msg = "非法参数SkuId" };
                if (auditTime == DateTime.MinValue) return new ThirdResponse { Code = 20505, Msg = "非法参数AuditTime" };
                if (serviceResult.RefundStatus < 0 || serviceResult.RefundStatus > 1) return new ThirdResponse { Code = 20506, Msg = "非法参数RefundStatus" };
                if (serviceResult.RefundStatus == 1 && string.IsNullOrEmpty(serviceResult.RejectReason)) return new ThirdResponse { Code = 20507, Msg = "缺少参数RejectReason" };
                #region 判断是否第三方电商售后服务单及获取售后服务单信息
                var service = ThirdECService.ObjectSet().FirstOrDefault(o => o.OrderId == orderId
                    && o.OrderRefundAfterSalesId == serviceId && o.SkuId == serviceResult.SkuId);
                if (service == null) return new ThirdResponse { Code = 20508, Msg = "未找到此服务单" };
                var orderRefund = OrderRefundAfterSales.ObjectSet().FirstOrDefault(p => p.Id == service.OrderRefundAfterSalesId);
                if (service == null) return new ThirdResponse { Code = 20509, Msg = "未找到此服务单" };
                if (service.AgreeRefundTime.HasValue || service.RejectRefundTime.HasValue) return result;
                #endregion
                #region 退款处理
                var errorMessage = string.Empty;
                if (serviceResult.RefundStatus == 0)
                {
                    var refundResult = OrderHelper.ApproveOrderRefundAfterSales(orderRefund.OrderId, orderRefund.OrderItemId ?? Guid.Empty);
                    if (refundResult.ResultCode == 0)
                    {
                        service.AgreeRefundTime = auditTime;
                        service.StateName = "允许退货";
                        service.StateDesc = "售后服务退货结果回调：允许退货";
                    }
                    else
                    {
                        errorMessage = refundResult.Message;
                        result = new ThirdResponse { Code = 20510, Msg = "内部异常" };
                    }
                }
                else
                {
                    var refundResult = OrderHelper.RejectOrderRefundAfterSales(orderId, orderRefund.OrderItemId ?? Guid.Empty, serviceResult.RejectReason);
                    if (refundResult.ResultCode == 0)
                    {
                        service.RejectRefundTime = auditTime;
                        service.StateName = "拒绝退货";
                        service.StateDesc = "售后服务退货结果回调：拒绝退货," + serviceResult.RejectReason;
                    }
                    else
                    {
                        errorMessage = refundResult.Message;
                        result = new ThirdResponse { Code = 20511, Msg = "内部异常" };
                    }
                }
                #endregion
                #region 保存ThirdECOrderJournal
                var journal = new ThirdECServiceJournal
                {
                    Id = Guid.NewGuid(),
                    OrderId = service.OrderId,
                    OrderCode = service.OrderCode,
                    OrderItemId = service.OrderItemId,
                    OrderRefundAfterSalesId = service.OrderRefundAfterSalesId,
                    Name = serviceResult.RefundStatus == 0 ? "允许退货" : "拒绝退货",
                    Details = string.IsNullOrEmpty(errorMessage) ? service.StateDesc : errorMessage,
                    Json = resultJsonStr,
                    EntityState = EntityState.Added
                };
                ContextFactory.CurrentThreadContext.SaveObject(journal);
                #endregion
                int count = ContextFactory.CurrentThreadContext.SaveChanges();
                if (count == 0)
                {
                    LogHelper.Error(string.Format("ThirdECommerceServiceHelper.RefundResultCallback第三方电商售后服务退货结果回调数据保存失败,入参:{0}", resultJsonStr));
                    result = new ThirdResponse { Code = 20512, Msg = "内部异常" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("ThirdECommerceServiceHelper.RefundResultCallback第三方电商售后服务退货结果回调异常，Input:" + resultJsonStr, ex);
                result = new ThirdResponse { Code = 20513, Msg = "内部异常" };
            }
            return result;
        }

        #endregion
    }
}