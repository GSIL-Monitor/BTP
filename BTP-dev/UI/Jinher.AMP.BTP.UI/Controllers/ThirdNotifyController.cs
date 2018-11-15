using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.CustomDTO.ThirdECommerce;
using Jinher.AMP.BTP.Deploy.CustomDTO.YX;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.BTP.TPS.Helper;
using Jinher.JAP.Common.Loging;
using Newtonsoft.Json;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public class ThirdNotifyController : Controller
    {
        /// <summary>
        /// 网易严选回调
        /// </summary>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult YanXuan()
        {
            try
            {
                var result = Json(new { code = 200, msg = "ok" }, JsonRequestBehavior.AllowGet);
                var method = Request["method"];
                var appKey = Request["appKey"];
                var sign = Request["sign"];
                var timestamp = Request["timestamp"];
                if (string.IsNullOrEmpty(method) || string.IsNullOrEmpty(appKey) || string.IsNullOrEmpty(sign) || string.IsNullOrEmpty(timestamp))
                    return result;
                var sbQuery = new StringBuilder();
                var sbForm = new StringBuilder();
                foreach (var key in Request.QueryString.AllKeys)
                {
                    sbQuery.AppendFormat("{0}={1}&", key, Request.QueryString[key]);
                }
                foreach (var key in Request.Form.AllKeys)
                {
                    sbQuery.AppendFormat("{0}={1}&", key, Request.Form[key]);
                }
                var request = sbQuery.ToString().TrimEnd('&');
                LogHelper.Info("ThirdNotifyController.YanXuan 网易严选回调，Request.QueryString: " + request);
                LogHelper.Info("ThirdNotifyController.YanXuan 网易严选回调，Request.Form: " + sbForm.ToString().TrimEnd('&'));
                //发送数据到mq
                var json = SerializationHelper.JsonSerialize(new { Request = request });
                if (!string.IsNullOrEmpty(method) && method.Contains("order"))
                {
                    RabbitMqHelper.Send(RabbitMqRoutingKey.YxOrderMsgReceived, RabbitMqExchange.Order, json);
                }
                else
                {
                    RabbitMqHelper.Send(RabbitMqRoutingKey.YxCommodityMsgReceived, RabbitMqExchange.Commodity, json);
                }
                // 订单包裹物流绑单回调
                if (method == "yanxuan.notification.order.delivered")
                {
                    YXOrderHelper.DeliverOrder(new YXSign
                    {
                        appKey = appKey,
                        method = method,
                        sign = sign,
                        timestamp = timestamp
                    }, Request["orderPackage"]);
                }
                // 订单异常回调（严选主动取消订单）
                else if (method == "yanxuan.notification.order.exceptional")
                {
                    YXOrderHelper.ExceptionalOrder(new YXSign
                    {
                        appKey = appKey,
                        method = method,
                        sign = sign,
                        timestamp = timestamp
                    }, Request["exceptionInfo"]);
                }
                // 取消订单回调
                else if (method == "yanxuan.callback.order.cancel")
                {
                    var jsonStr = Request["orderCancelResult"];
                    if (string.IsNullOrEmpty(jsonStr)) return result;
                    if (!YXSV.CheckSign(new YXSign
                    {
                        appKey = appKey,
                        method = method,
                        sign = sign,
                        timestamp = timestamp
                    },
                    new Dictionary<string, string> { { "orderCancelResult", jsonStr } })) return result;

                    YXOrderRefundHelper.CancelOrderCallback(JsonConvert.DeserializeObject<OrderCancelResultCallBack>(jsonStr));
                }
                // 退货地址回调（同意售后申请）
                else if (method == "yanxuan.notification.order.refund.address")
                {
                    var jsonStr = Request["refundAddress"];
                    if (string.IsNullOrEmpty(jsonStr)) return result;
                    if (!YXSV.CheckSign(new YXSign
                    {
                        appKey = appKey,
                        method = method,
                        sign = sign,
                        timestamp = timestamp
                    },
                    new Dictionary<string, string> { { "refundAddress", jsonStr } })) return result;

                    YXOrderRefundHelper.OrderRefundAddress(JsonConvert.DeserializeObject<RefundAddress>(jsonStr));
                }
                // 拒绝退货回调
                else if (method == "yanxuan.notification.order.refund.reject")
                {
                    var jsonStr = Request["rejectInfo"];
                    if (string.IsNullOrEmpty(jsonStr)) return result;
                    if (!YXSV.CheckSign(new YXSign
                    {
                        appKey = appKey,
                        method = method,
                        sign = sign,
                        timestamp = timestamp
                    },
                    new Dictionary<string, string> { { "rejectInfo", jsonStr } })) return result;

                    YXOrderRefundHelper.RejectOrderRefund(JsonConvert.DeserializeObject<RejectInfo>(jsonStr));
                }
                // 退货包裹确认收货回调
                else if (method == "yanxuan.notification.order.refund.express.confirm")
                {
                    var jsonStr = Request["expressConfirm"];
                    if (string.IsNullOrEmpty(jsonStr)) return result;
                    if (!YXSV.CheckSign(new YXSign
                    {
                        appKey = appKey,
                        method = method,
                        sign = sign,
                        timestamp = timestamp
                    },
                    new Dictionary<string, string> { { "expressConfirm", jsonStr } })) return result;

                    YXOrderRefundHelper.ConfirmOrderRefundExpress(JsonConvert.DeserializeObject<ExpressConfirm>(jsonStr));
                }
                // 取消退货回调(严选发起退货地址回调之后，如果超过14天没有收到渠道方绑定售后寄回物流单号，会取消相关的退货申请)
                else if (method == "yanxuan.notification.order.refund.system.cancel")
                {
                    var jsonStr = Request["systemCancel"];
                    if (string.IsNullOrEmpty(jsonStr)) return result;
                    if (!YXSV.CheckSign(new YXSign
                    {
                        appKey = appKey,
                        method = method,
                        sign = sign,
                        timestamp = timestamp
                    },
                    new Dictionary<string, string> { { "systemCancel", jsonStr } })) return result;

                    YXOrderRefundHelper.SystemCancelOrderRefund(JsonConvert.DeserializeObject<SystemCancel>(jsonStr));
                }
                // 退款结果回调(严选会将最终的可退款结果通知渠道，会出现部分sku退款或者同一sku多个数量但只退其中一部分的情况)
                else if (method == "yanxuan.notification.order.refund.result")
                {
                    var jsonStr = Request["refundResult"];
                    if (string.IsNullOrEmpty(jsonStr)) return result;
                    if (!YXSV.CheckSign(new YXSign
                    {
                        appKey = appKey,
                        method = method,
                        sign = sign,
                        timestamp = timestamp
                    },
                    new Dictionary<string, string> { { "refundResult", jsonStr } })) return result;

                    YXOrderRefundHelper.OrderRefundResult(JsonConvert.DeserializeObject<RefundResult>(jsonStr));
                }
                // 渠道SKU库存校准回调
                else if (method == "yanxuan.notification.inventory.count.check")
                {
                    var jsonStr = Request["skuCheck"];
                    if (string.IsNullOrEmpty(jsonStr)) return result;
                    if (!YXSV.CheckSign(new YXSign
                    {
                        appKey = appKey,
                        method = method,
                        sign = sign,
                        timestamp = timestamp
                    },
                    new Dictionary<string, string> { { "skuCheck", jsonStr } })) return result;

                    YXCommodityHelper.SkuStockCheck(JsonConvert.DeserializeObject<skuCheck>(jsonStr));
                }
                // 渠道SKU低库存预警通知
                else if (method == "yanxuan.notification.sku.alarm.close")
                {
                    var jsonStr = Request["closeAlarmSkus"];
                    if (string.IsNullOrEmpty(jsonStr)) return result;
                    if (!YXSV.CheckSign(new YXSign
                    {
                        appKey = appKey,
                        method = method,
                        sign = sign,
                        timestamp = timestamp
                    },
                    new Dictionary<string, string> { { "closeAlarmSkus", jsonStr } })) return result;

                    YXCommodityHelper.SkuStockAlarm(JsonConvert.DeserializeObject<List<SkuCloseAlarmVO>>(jsonStr));
                }
                // 渠道SKU再次开售通知
                else if (method == "yanxuan.notification.sku.reopen")
                {
                    var jsonStr = Request["reopenSkus"];
                    if (string.IsNullOrEmpty(jsonStr)) return result;
                    if (!YXSV.CheckSign(new YXSign
                    {
                        appKey = appKey,
                        method = method,
                        sign = sign,
                        timestamp = timestamp
                    },
                    new Dictionary<string, string> { { "reopenSkus", jsonStr } })) return result;

                    YXCommodityHelper.SkuStockReopen(JsonConvert.DeserializeObject<List<SkuReopenVO>>(jsonStr));
                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error("ThirdNotifyController.YanXuan 网易严选回调异常", ex);
                return Json(new { code = 500, msg = "系统异常" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 第三方电商回调
        /// </summary>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult ECommerce()
        {
            try
            {
                Guid appId;
                var appIdStr = Request["appId"];
                var method = Request["method"];
                var callerId = Request["callerid"];
                var time = Request["time"];
                var code = Request["code"];
                Guid.TryParse(appIdStr, out appId);
                if (string.IsNullOrEmpty(method)) return Json(new ThirdResponse { Code = 10001, Msg = "缺少参数method" }, JsonRequestBehavior.AllowGet);
                var result = ThirdECommerceHelper.CheckThirdECommerce(appId, callerId, time, code);
                if (!result.Successed) return Json(result);
                var sbQuery = new StringBuilder();
                var sbForm = new StringBuilder();
                foreach (var key in Request.QueryString.AllKeys)
                {
                    sbQuery.AppendFormat("{0}={1}&", key, Request.QueryString[key]);
                }
                foreach (var key in Request.Form.AllKeys)
                {
                    sbQuery.AppendFormat("{0}={1}&", key, Request.Form[key]);
                }
                LogHelper.Info("ThirdNotifyController.ECommerce 第三方电商回调，Request.QueryString: " + sbQuery.ToString().TrimEnd('&'));
                LogHelper.Info("ThirdNotifyController.ECommerce 第三方电商回调，Request.Form: " + sbForm.ToString().TrimEnd('&'));
                // 取消订单审核结果回调
                if (method == "thirdnotify.order.cancelcallback")
                {
                    return Json(ThirdECommerceOrderHelper.CancelOrderCallback(Request["orderCancelResult"]), JsonRequestBehavior.AllowGet);
                }
                // 商品发货信息回调
                else if (method == "thirdnotify.order.deliver")
                {
                    return Json(ThirdECommerceOrderHelper.DeliverOrderCallback(Request["orderPackage"]), JsonRequestBehavior.AllowGet);
                }
                // 物流追踪信息回调
                else if (method == "thirdnotify.order.expresstrace")
                {
                    return Json(ThirdECommerceOrderHelper.ExpressTraceCallback(Request["expressTrace"]), JsonRequestBehavior.AllowGet);
                }
                // 商品收货信息回调
                else if (method == "thirdnotify.order.confirm")
                {
                    return Json(ThirdECommerceOrderHelper.ConfirmOrderCallback(Request["orderPackage"]), JsonRequestBehavior.AllowGet);
                }
                // 同意售后回调
                else if (method == "thirdnotify.service.agree")
                {
                    return Json(ThirdECommerceServiceHelper.AgreeServiceCallback(Request["serviceAgreeResult"]), JsonRequestBehavior.AllowGet);
                }
                // 拒绝售后回调
                else if (method == "thirdnotify.service.reject")
                {
                    return Json(ThirdECommerceServiceHelper.RejectServiceCallback(Request["serviceRejectResult"]), JsonRequestBehavior.AllowGet);
                }
                // 退货物流签收回调
                else if (method == "thirdnotify.mail.receive")
                {
                    return Json(ThirdECommerceServiceHelper.ReceiveMailCallback(Request["mailReceiveResult"]), JsonRequestBehavior.AllowGet);
                }
                // 退货物流(超X天)未签收回调
                else if (method == "thirdnotify.mail.notreceive")
                {
                    return Json(ThirdECommerceServiceHelper.NotReceiveMailCallback(Request["mailNotReceiveResult"]), JsonRequestBehavior.AllowGet);
                }
                // 售后退货结果回调
                else if (method == "thirdnotify.service.refundresult")
                {
                    return Json(ThirdECommerceServiceHelper.RefundResultCallback(Request["serviceRefundResult"]), JsonRequestBehavior.AllowGet);
                }
                
                return Json(new ThirdResponse { Code = 200, Msg = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Error("ThirdNotifyController.YanXuan 网易严选回调异常", ex);
                return Json(new { code = 500, msg = "内部异常" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
