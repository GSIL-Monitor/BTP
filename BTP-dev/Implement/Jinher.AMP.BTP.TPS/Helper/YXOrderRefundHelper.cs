using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.CustomDTO.YX;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;
using Newtonsoft.Json;

namespace Jinher.AMP.BTP.TPS.Helper
{
    /// <summary>
    /// 网易严选订单售后帮助类
    /// </summary>
    public static class YXOrderRefundHelper
    {
        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="order"></param>
        public static ResultDTO CancelPaidOrder(CommodityOrder order)
        {
            var jsonStr = string.Empty;
            var result = YXSV.CancelPaidOrder(order.Id.ToString(), ref jsonStr);
            #region 保存网易严选订单日志
            Task.Factory.StartNew(() =>
            {
                var journal = new YXOrderJournal
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    OrderCode = order.Code,
                    SubTime = DateTime.Now,
                    SubId = order.SubId,
                    Name = "渠道取消订单申请",
                    Details = result.Message,
                    Json = jsonStr,
                    EntityState = System.Data.EntityState.Added
                };
                ContextFactory.CurrentThreadContext.SaveObject(journal);
                ContextFactory.CurrentThreadContext.SaveChanges();
            });
            #endregion
            if (result.isSuccess)
            {
                if (result.Data.cancelStatus == 0)
                {
                    return new ResultDTO { isSuccess = false, ResultCode = 1, Message = result.Data.rejectReason };
                }
                if (result.Data.cancelStatus == 2)
                {
                    // 等待人工审核(审核时间一般为24小时内)，对于人工审核的订单取消申请，严选工作人员审核完成后，通过调用渠道方提供的回调接口告知取消结果。
                    return new ResultDTO { isSuccess = true, Message = "请等待审核。", ResultCode = 2 };
                }
                if (result.Data.cancelStatus == 1)
                {
                    // 手动触发取消订单成功回调
                    Timer timer = null;
                    timer = new Timer(_ =>
                    {
                        CancelOrderCallback(new OrderCancelResultCallBack { orderId = order.Id.ToString().ToLower(), cancelStatus = 1 });
                        timer.Dispose();
                    }, null, 2000, System.Threading.Timeout.Infinite);
                    //Task.Factory.StartNew(() => Thread.Sleep(10000)).ContinueWith(_ => CancelOrderCallback(new OrderCancelResultCallBack { orderId = order.Id, cancelStatus = 1 }));
                    return new ResultDTO { isSuccess = true, Message = result.Data.rejectReason, ResultCode = 0 };
                }
            }
            // 订单不存在
            else if (result.ResultCode == 20108)
            {
                return new ResultDTO { isSuccess = false, Message = "订单异常，请联系客服~", ResultCode = -1 };
            }
            return new ResultDTO { isSuccess = false, Message = "操作失败，请稍后重试。", ResultCode = -1 };
        }

        /// <summary>
        /// 发起售后申请（售前-不支持）
        /// </summary>
        /// <returns></returns>
        public static ResultDTO ApplyRefundOrder(CommodityOrder order, OrderItem orderItem, OrderRefund refund)
        {
            string skuId = null;
            if (orderItem.CommodityStockId.HasValue && orderItem.CommodityStockId != Guid.Empty && orderItem.CommodityStockId != orderItem.CommodityId)
            {
                var comStock = CommodityStock.FindByID(orderItem.CommodityStockId.Value);
                if (comStock == null)
                {
                    return new ResultDTO { isSuccess = false, Message = "商品不存在。", ResultCode = -1 };
                }
                else
                {
                    skuId = comStock.JDCode;
                }
            }
            else
            {
                var commodity = Commodity.FindByID(orderItem.CommodityId);
                if (commodity == null)
                {
                    return new ResultDTO { isSuccess = false, Message = "商品不存在。", ResultCode = -1 };
                }
                else
                {
                    skuId = commodity.JDCode;
                }
            }
            if (string.IsNullOrEmpty(skuId))
            {
                return new ResultDTO { isSuccess = false, Message = "商品SKU编码不存在。", ResultCode = -1 };
            }
            var orderSku = YXOrderSku.ObjectSet().Where(_ => _.OrderId == order.Id && _.SkuId == skuId).FirstOrDefault();
            if (orderSku == null)
            {
                return new ResultDTO { isSuccess = false, Message = "订单SKU存在。", ResultCode = -1 };
            }
            var requestData = new ApplyInfo
            {
                orderId = order.Id.ToString(),
                requestId = refund.Id.ToString(),
                applyUser = new ApplyUser { name = order.ReceiptUserName, mobile = order.ReceiptPhone },
                applySku = new ApplySku
                {
                    packageId = orderSku.PackageId,
                    skuId = orderSku.SkuId,
                    count = orderSku.SaleCount,
                    originalPrice = orderSku.OriginPrice,
                    subtotalPrice = orderSku.SubtotalAmount,
                    applySkuReason = new ApplySkuReason { reason = refund.RefundReason, reasonDesc = refund.RefundDesc },
                    applyPicList = new List<ApplyPic> { }
                }
            };
            if (!string.IsNullOrEmpty(refund.OrderRefundImgs))
            {
                foreach (var img in refund.OrderRefundImgs.Split(','))
                {
                    requestData.applySku.applyPicList.Add(new ApplyPic { fileName = System.IO.Path.GetFileName(img), url = img });
                }
            }
            var result = YXSV.ApplyRefundOrder(requestData);
            if (!result.isSuccess)
            {
                return result;
            }
            refund.ApplyId = result.Data.applyId;
            return new ResultDTO { isSuccess = true };
        }

        /// <summary>
        /// 发起售后申请（售后）
        /// </summary>
        /// <returns></returns>
        public static ResultDTO ApplyRefundOrderAfterSales(CommodityOrder order, OrderItem orderItem, OrderRefundAfterSales refund)
        {
            string skuId = null;
            if (orderItem.CommodityStockId.HasValue && orderItem.CommodityStockId != Guid.Empty && orderItem.CommodityStockId != orderItem.CommodityId)
            {
                var comStock = CommodityStock.FindByID(orderItem.CommodityStockId.Value);
                if (comStock == null)
                {
                    return new ResultDTO { isSuccess = false, Message = "商品不存在。", ResultCode = -1 };
                }
                else
                {
                    skuId = comStock.JDCode;
                }
            }
            else
            {
                var commodity = Commodity.FindByID(orderItem.CommodityId);
                if (commodity == null)
                {
                    return new ResultDTO { isSuccess = false, Message = "商品不存在。", ResultCode = -1 };
                }
                else
                {
                    skuId = commodity.JDCode;
                }
            }
            if (string.IsNullOrEmpty(skuId))
            {
                return new ResultDTO { isSuccess = false, Message = "商品SKU编码不存在。", ResultCode = -1 };
            }
            var orderSku = YXOrderSku.ObjectSet().Where(_ => _.OrderId == order.Id && _.SkuId == skuId).FirstOrDefault();
            if (orderSku == null)
            {
                return new ResultDTO { isSuccess = false, Message = "订单SKU存在。", ResultCode = -1 };
            }
            var requestData = new ApplyInfo
            {
                orderId = order.Id.ToString(),
                requestId = "s" + refund.Id.ToString(),
                applyUser = new ApplyUser { name = order.ReceiptUserName, mobile = order.ReceiptPhone },
                applySku = new ApplySku
                {
                    packageId = orderSku.PackageId,
                    skuId = orderSku.SkuId,
                    count = orderSku.SaleCount,
                    originalPrice = orderSku.OriginPrice,
                    subtotalPrice = orderSku.SubtotalAmount,
                    applySkuReason = new ApplySkuReason { reason = refund.RefundReason, reasonDesc = refund.RefundDesc },
                    applyPicList = new List<ApplyPic> { }
                }
            };
            if (!string.IsNullOrEmpty(refund.OrderRefundImgs))
            {
                foreach (var img in refund.OrderRefundImgs.Split(','))
                {
                    requestData.applySku.applyPicList.Add(new ApplyPic { fileName = System.IO.Path.GetFileName(img), url = img });
                }
            }
            var result = YXSV.ApplyRefundOrder(requestData);
            if (!result.isSuccess)
            {
                return result;
            }
            refund.ApplyId = result.Data.applyId;
            return new ResultDTO { isSuccess = true };
        }

        /// <summary>
        /// 获取售后信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static Jinher.AMP.BTP.Deploy.OrderRefundAfterSalesDTO GetOrderRefundAfterSalesInfo(Guid orderId)
        {
            var info = OrderRefundAfterSales.ObjectSet().Where(_ => _.OrderId == orderId).OrderByDescending(_ => _.SubTime).FirstOrDefault();
            if (info == null) return null;
            return info.ToEntityData();
        }

        #region 回调
        /// <summary>
        /// 订单取消回调
        /// </summary>
        /// <param name="result"></param>
        public static void CancelOrderCallback(OrderCancelResultCallBack result)
        {
            try
            {
                Guid orderId;
                Guid.TryParse(result.orderId, out orderId);
                LogHelper.Debug("进入 YXOrderHelper.CancelOrderCallback，Input:" + JsonConvert.SerializeObject(result));
                OrderSV.UnLockOrder(orderId);
                if (result.cancelStatus == 0)
                {
                    var optResult = OrderHelper.RejectOrderRefund(orderId, result.rejectReason);
                    if (optResult.ResultCode != 0)
                    {
                        LogHelper.Error("YXOrderHelper.CancelOrderCallback 取消订单失败，RejectOrderRefund返回：" + JsonConvert.SerializeObject(optResult));
                    }
                }
                else if (result.cancelStatus == 1)
                {
                    var optResult = OrderHelper.ApproveCancelOrder(orderId);
                    if (optResult.ResultCode != 0)
                    {
                        LogHelper.Error("YXOrderHelper.CancelOrderCallback 取消订单失败，ApproveCancelOrder返回：" + JsonConvert.SerializeObject(optResult));
                    }
                }
                else
                {
                    LogHelper.Error("YXOrderHelper.CancelOrderCallback 取消订单失败，严选回调为待客服审核：" + JsonConvert.SerializeObject(result));
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXOrderHelper.CancelOrderCallback 异常，Input:" + JsonConvert.SerializeObject(result), ex);
            }
        }

        /// <summary>
        /// 退货地址回调
        /// </summary>
        /// <param name="result"></param>
        public static void OrderRefundAddress(RefundAddress result)
        {
            try
            {
                Guid orderId;
                Guid.TryParse(result.orderId, out orderId);
                LogHelper.Debug("进入 YXOrderHelper.OrderRefundAddress，Input:" + JsonConvert.SerializeObject(result));
                var orderRefund = OrderRefundAfterSales.ObjectSet().Where(_ => _.ApplyId == result.applyId).FirstOrDefault();
                if (orderRefund == null)
                {
                    LogHelper.Error("YXOrderHelper.OrderRefundAddress 失败，未找到退款记录，Input:" + JsonConvert.SerializeObject(result));
                }
                var refundResult = OrderHelper.ApproveOrderRefundAfterSales(orderId, orderRefund.OrderItemId ?? Guid.Empty);
                if (refundResult.ResultCode != 0)
                {
                    // 失败
                    LogHelper.Error("YXOrderHelper.OrderRefundAddress 失败，" + refundResult.Message + "，Input:" + JsonConvert.SerializeObject(result));
                }
                else
                {
                    ContextSession contextSession = ContextFactory.CurrentThreadContext;
                    orderRefund.RefundReturnType = result.type;
                    orderRefund.RefundReceiveFullAddress = result.returnAddr.fullAddress;
                    orderRefund.RefundReceiveMobile = result.returnAddr.mobile;
                    orderRefund.RefundReceiveName = result.returnAddr.name;
                    //orderRefund.EntityState = System.Data.EntityState.Added;
                    contextSession.SaveObject(orderRefund);
                    contextSession.SaveChanges();
                }

                //if (result.applyId.StartsWith("s"))
                //{
                //    // 售后
                //    var orderRefund = OrderRefundAfterSales.FindByID(new Guid(result.applyId.Substring(1)));
                //    if (orderRefund == null)
                //    {
                //        LogHelper.Error("YXOrderHelper.OrderRefundAddress 失败，未找到退款记录，Input:" + JsonConvert.SerializeObject(result));
                //    }
                //    var refundResult = OrderHelper.ApproveOrderRefund(orderId, orderRefund.OrderItemId ?? Guid.Empty);
                //    if (refundResult.ResultCode != 0)
                //    {
                //        // 失败
                //        LogHelper.Error("YXOrderHelper.OrderRefundAddress 失败，" + refundResult.Message + "，Input:" + JsonConvert.SerializeObject(result));
                //    }
                //}
                //else
                //{
                //    //售前
                //    var orderRefund = OrderRefund.FindByID(new Guid(result.applyId));
                //    if (orderRefund == null)
                //    {
                //        LogHelper.Error("YXOrderHelper.OrderRefundAddress 失败，未找到退款记录，Input:" + JsonConvert.SerializeObject(result));
                //    }
                //    var refundResult = OrderHelper.ApproveOrderRefundAfterSales(orderId, orderRefund.OrderItemId ?? Guid.Empty);
                //    if (refundResult.ResultCode != 0)
                //    {
                //        // 失败
                //        LogHelper.Error("YXOrderHelper.OrderRefundAddress 失败，" + refundResult.Message + "，Input:" + JsonConvert.SerializeObject(result));
                //    }
                //}
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXOrderHelper.OrderRefundAddress 异常，Input:" + JsonConvert.SerializeObject(result), ex);
            }
        }

        /// <summary>
        /// 拒绝退货回调
        /// </summary>
        /// <param name="result"></param>
        public static void RejectOrderRefund(RejectInfo result)
        {
            try
            {
                Guid orderId;
                Guid.TryParse(result.orderId, out orderId);
                LogHelper.Debug("进入 YXOrderHelper.RejectOrderRefund，Input:" + JsonConvert.SerializeObject(result));

                var orderRefund = OrderRefundAfterSales.ObjectSet().Where(_ => _.ApplyId == result.applyId).FirstOrDefault();
                if (orderRefund == null)
                {
                    LogHelper.Error("YXOrderHelper.RejectOrderRefund 失败，未找到退款记录，Input:" + JsonConvert.SerializeObject(result));
                }
                var refundResult = OrderHelper.RejectOrderRefundAfterSales(orderId, orderRefund.OrderItemId ?? Guid.Empty, result.rejectReason);
                if (refundResult.ResultCode != 0)
                {
                    // 失败
                    LogHelper.Error("YXOrderHelper.RejectOrderRefund 失败，" + refundResult.Message + "，Input:" + JsonConvert.SerializeObject(result));
                }

                //if (result.applyId.StartsWith("s"))
                //{
                //    // 售后
                //    var orderRefund = OrderRefundAfterSales.FindByID(new Guid(result.applyId.Substring(1)));
                //    if (orderRefund == null)
                //    {
                //        LogHelper.Error("YXOrderHelper.RejectOrderRefund 失败，未找到退款记录，Input:" + JsonConvert.SerializeObject(result));
                //    }
                //    var refundResult = OrderHelper.RejectOrderRefund(orderId, orderRefund.OrderItemId ?? Guid.Empty, result.rejectReason);
                //    if (refundResult.ResultCode != 0)
                //    {
                //        // 失败
                //        LogHelper.Error("YXOrderHelper.RejectOrderRefund 失败，" + refundResult.Message + "，Input:" + JsonConvert.SerializeObject(result));
                //    }
                //}
                //else
                //{
                //    //售前
                //    var orderRefund = OrderRefund.FindByID(new Guid(result.applyId));
                //    if (orderRefund == null)
                //    {
                //        LogHelper.Error("YXOrderHelper.RejectOrderRefund 失败，未找到退款记录，Input:" + JsonConvert.SerializeObject(result));
                //    }
                //    var refundResult = OrderHelper.RejectOrderRefundAfterSales(orderId, orderRefund.OrderItemId ?? Guid.Empty, result.rejectReason);
                //    if (refundResult.ResultCode != 0)
                //    {
                //        // 失败
                //        LogHelper.Error("YXOrderHelper.RejectOrderRefund 失败，" + refundResult.Message + "，Input:" + JsonConvert.SerializeObject(result));
                //    }
                //}
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXOrderHelper.RejectOrderRefund 异常，Input:" + JsonConvert.SerializeObject(result), ex);
            }
        }

        /// <summary>
        /// 退货包裹确认收货回调
        /// </summary>
        /// <param name="result"></param>
        public static void ConfirmOrderRefundExpress(ExpressConfirm result)
        {
            try
            {
                LogHelper.Debug("进入 YXOrderHelper.ConfirmOrderRefundExpress，Input:" + JsonConvert.SerializeObject(result));

                // 金和系统暂不支持确认收到包裹，暂时注释
                //if (result.applyId.StartsWith("s"))
                //{
                //    // 售后
                //    var orderRefund = OrderRefundAfterSales.FindByID(new Guid(result.applyId.Substring(1)));
                //    if (orderRefund == null)
                //    {
                //        LogHelper.Error("YXOrderHelper.ConfirmOrderRefundExpress 失败，未找到退款记录，Input:" + JsonConvert.SerializeObject(result));
                //    }
                //    var refundResult = OrderHelper.ApproveOrderRefund(orderRefund.OrderId, orderRefund.OrderItemId ?? Guid.Empty);
                //    if (refundResult.ResultCode != 0)
                //    {
                //        // 失败
                //        LogHelper.Error("YXOrderHelper.ConfirmOrderRefundExpress 失败，" + refundResult.Message + "，Input:" + JsonConvert.SerializeObject(result));
                //    }
                //}
                //else
                //{
                //    //售前
                //    var orderRefund = OrderRefund.FindByID(new Guid(result.applyId));
                //    if (orderRefund == null)
                //    {
                //        LogHelper.Error("YXOrderHelper.ConfirmOrderRefundExpress 失败，未找到退款记录，Input:" + JsonConvert.SerializeObject(result));
                //    }
                //    var refundResult = OrderHelper.ApproveOrderRefundAfterSales(orderRefund.OrderId, orderRefund.OrderItemId ?? Guid.Empty);
                //    if (refundResult.ResultCode != 0)
                //    {
                //        // 失败
                //        LogHelper.Error("YXOrderHelper.ConfirmOrderRefundExpress 失败，" + refundResult.Message + "，Input:" + JsonConvert.SerializeObject(result));
                //    }
                //}
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXOrderHelper.ConfirmOrderRefundExpress 异常，Input:" + JsonConvert.SerializeObject(result), ex);
            }
        }

        /// <summary>
        /// 严选系统取消退货回调
        /// </summary>
        /// <param name="result"></param>
        public static void SystemCancelOrderRefund(SystemCancel result)
        {
            try
            {
                Guid orderId;
                Guid.TryParse(result.orderId, out orderId);
                LogHelper.Debug("进入 YXOrderHelper.SystemCancelOrderRefund，Input:" + JsonConvert.SerializeObject(result));

                var orderRefund = OrderRefundAfterSales.ObjectSet().Where(_ => _.ApplyId == result.applyId).FirstOrDefault();
                if (orderRefund == null)
                {
                    LogHelper.Error("YXOrderHelper.SystemCancelOrderRefund 失败，未找到退款记录，Input:" + JsonConvert.SerializeObject(result));
                }
                var refundResult = OrderHelper.RejectOrderRefundAfterSales(orderId, orderRefund.OrderItemId ?? Guid.Empty, result.errorMsg);
                if (refundResult.ResultCode != 0)
                {
                    // 失败
                    LogHelper.Error("YXOrderHelper.SystemCancelOrderRefund 失败，" + refundResult.Message + "，Input:" + JsonConvert.SerializeObject(result));
                }

                //if (result.applyId.StartsWith("s"))
                //{
                //    // 售后
                //    var orderRefund = OrderRefundAfterSales.FindByID(new Guid(result.applyId.Substring(1)));
                //    if (orderRefund == null)
                //    {
                //        LogHelper.Error("YXOrderHelper.SystemCancelOrderRefund 失败，未找到退款记录，Input:" + JsonConvert.SerializeObject(result));
                //    }
                //    var refundResult = OrderHelper.RejectOrderRefund(orderId, orderRefund.OrderItemId ?? Guid.Empty, result.errorMsg);
                //    if (refundResult.ResultCode != 0)
                //    {
                //        // 失败
                //        LogHelper.Error("YXOrderHelper.SystemCancelOrderRefund 失败，" + refundResult.Message + "，Input:" + JsonConvert.SerializeObject(result));
                //    }
                //}
                //else
                //{
                //    //售前
                //    var orderRefund = OrderRefund.FindByID(new Guid(result.applyId));
                //    if (orderRefund == null)
                //    {
                //        LogHelper.Error("YXOrderHelper.v 失败，未找到退款记录，Input:" + JsonConvert.SerializeObject(result));
                //    }
                //    var refundResult = OrderHelper.RejectOrderRefundAfterSales(orderId, orderRefund.OrderItemId ?? Guid.Empty, result.errorMsg);
                //    if (refundResult.ResultCode != 0)
                //    {
                //        // 失败
                //        LogHelper.Error("YXOrderHelper.SystemCancelOrderRefund 失败，" + refundResult.Message + "，Input:" + JsonConvert.SerializeObject(result));
                //    }
                //}
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXOrderHelper.SystemCancelOrderRefund 异常，Input:" + JsonConvert.SerializeObject(result), ex);
            }
        }

        /// <summary>
        /// 退款结果回调
        /// </summary>
        /// <param name="result"></param>
        public static void OrderRefundResult(RefundResult result)
        {
            try
            {
                Guid orderId;
                Guid.TryParse(result.orderId, out orderId);
                LogHelper.Debug("进入 YXOrderHelper.OrderRefundResult，Input:" + JsonConvert.SerializeObject(result));

                var orderRefund = OrderRefundAfterSales.ObjectSet().Where(_ => _.ApplyId == result.applyId).FirstOrDefault();
                if (orderRefund == null)
                {
                    LogHelper.Error("YXOrderHelper.OrderRefundResult 失败，未找到退款记录，Input:" + JsonConvert.SerializeObject(result));
                }
                var orderItem = OrderItem.FindByID(orderRefund.OrderItemId.Value);
                string skuId;
                if (orderItem.CommodityStockId == null || orderItem.CommodityStockId.Value == Guid.Empty || orderItem.CommodityStockId == orderItem.CommodityId)
                {
                    var commodity = Commodity.FindByID(orderItem.CommodityId);
                    skuId = commodity.JDCode;
                }
                else
                {
                    var comStock = CommodityStock.FindByID(orderItem.CommodityStockId.Value);
                    skuId = comStock.JDCode;
                }

                var skuResult = result.refundSkuList.Where(_ => _.skuId == skuId).FirstOrDefault();
                var skuOperateResult = skuResult.operateSkus.Where(_ => _.skuId == skuId).FirstOrDefault();

                if (skuOperateResult.status == OrderRefundApplySkuOperateStatusEnum.审核通过)
                {
                    var refundResult = OrderHelper.ApproveOrderRefundAfterSales(orderRefund.OrderId, orderRefund.OrderItemId ?? Guid.Empty);
                    if (refundResult.ResultCode != 0)
                    {
                        // 失败
                        LogHelper.Error("YXOrderHelper.OrderRefundResult 失败，" + refundResult.Message + "，Input:" + JsonConvert.SerializeObject(result));
                    }
                }
                else if (skuOperateResult.status == OrderRefundApplySkuOperateStatusEnum.已拒绝)
                {
                    var refundResult = OrderHelper.RejectOrderRefundAfterSales(orderId, orderRefund.OrderItemId ?? Guid.Empty, skuOperateResult.reason);
                    if (refundResult.ResultCode != 0)
                    {
                        // 失败
                        LogHelper.Error("YXOrderHelper.RejectOrderRefund 失败，" + refundResult.Message + "，Input:" + JsonConvert.SerializeObject(result));
                    }
                }
                else
                {
                    LogHelper.Error("YXOrderHelper.RejectOrderRefund -> OrderId: " + orderId + ", ApplyId: " + result.applyId + ", 忽略的状态：" + skuOperateResult.status.ToString());
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXOrderHelper.OrderRefundResult 异常，Input:" + JsonConvert.SerializeObject(result), ex);
            }
        }
        #endregion
    }
}
