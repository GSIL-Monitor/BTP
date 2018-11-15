using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Specialized;
using Com.Alipay;
using Jinher.JAP.PL;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.UI.Models;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.Enum;
using System.Threading;
using System.Threading.Tasks;
using Jinher.AMP.BTP.TPS.Helper;
using Jinher.AMP.BTP.TPS;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public class PaymentNotifyController : Jinher.JAP.MVC.Controller.BaseController
    {
        /// <summary>
        /// 支付宝支付回调(支付宝直接到账)
        /// </summary>
        public void Alipay(string out_trade_no)
        {
            //SortedDictionary<string, string> sPara = GetRequestPost();

            //if (sPara.Count > 0)//判断是否有带返回参数
            //{
            //string out_trade_no = Request.Form["out_trade_no"];      //商户订单号
            //string trade_no = Request.Form["trade_no"];              //支付宝交易号
            //string trade_status = Request.Form["trade_status"];      //交易状态

            string[] orderids = out_trade_no.Split(',');
            //获取订单id
            Guid orderid;
            if (!Guid.TryParse(orderids[0], out orderid) || orderid == null)
            {
                Response.Write("fail");
                LogHelper.Error("支付宝支付回调失败：传入参数不正确out_trade_no：" + out_trade_no);
                return;
            }

            #region 根据订单id查询商户的支付宝签约信息，并更新到配置

            //查询订单信息
            Jinher.AMP.BTP.IBP.Facade.CommodityOrderFacade facade = new IBP.Facade.CommodityOrderFacade();
            facade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
            Deploy.PaymentsDTO payment = facade.GetPaymentByOrderId(orderid, "支付宝");
            if (payment == null)
            {
                Response.Write("fail");
                LogHelper.Error("支付宝支付回调失败：未查询到到订单。out_trade_no：" + out_trade_no);
                return;
            }
            Com.Alipay.Config.Partner = payment.AliPayPartnerId == null ? "" : payment.AliPayPartnerId;
            //Com.Alipay.Config.Private_key = payment.AliPayPrivateKey;//暂时没有用到

            #endregion

            //验证消息是否是支付宝所发的合法消息
            Notify aliNotify = new Notify();
            //bool verifyResult = aliNotify.Verify(sPara, Request.Form["notify_id"], Request.Form["sign"]);

            //if (verifyResult)//验证成功
            //{
            LogHelper.Error(string.Format("支付宝支付回调：订单out_trade_no：{0},trade_status:", out_trade_no, Request.Form["trade_status"]));
            #region 交易状态，此处不区分状态，一律视为成功
            if (Request.Form["trade_status"] == "TRADE_FINISHED" || Request.Form["trade_status"] == "TRADE_SUCCESS")
            {
                //注意：
                //该种交易状态只在两种情况下出现
                //1、开通了普通即时到账，买家付款成功后。
                //2、开通了高级即时到账，从该笔交易成功时间算起，过了签约时的可退款时限（如：三个月以内可退款、一年以内可退款等）后。
                #region 更新订单信息

                //更新订单状态
                Deploy.CustomDTO.ResultDTO result = UpdateOrderStatus(orderid, 2, 0, 0, 0);
                if (result == null || result.ResultCode != 0)
                {
                    Response.Write("fail");
                    LogHelper.Error(string.Format("支付宝支付回调失败：更新订单状态失败。out_trade_no:{0},ErrorMessage:{1}", out_trade_no, result.Message));
                    return;
                }

                #endregion

                Response.Write("success");  //请不要修改或删除
                LogHelper.Info("支付宝支付回调成功：out_trade_no" + out_trade_no);
                //}
                //else//验证失败
                //{
                //    Response.Write("fail");
                //    LogHelper.Error("支付宝支付回调失败：签名验证失败。out_trade_no：" + out_trade_no);
                //}
                //}
                //else
                //{
                //    Response.Write("无通知参数");
                //    LogHelper.Error("支付宝支付回调失败：没有参数传入");
                //}
            }
            else
            {
            }
            #endregion

            Response.Write("success");
        }

        /// <summary>
        /// 获取支付宝POST过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        [NonAction()]
        public SortedDictionary<string, string> GetRequestPost()
        {
            int i = 0;
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = Request.Form;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Request.Form[requestItem[i]]);
            }

            return sArray;
        }

        /// <summary>
        /// 金币支付回调
        /// </summary>
        public void Goldpay(string outTradeId, string sign, int payment, ulong gold, decimal money, decimal couponCount, int tradeType)
        {

            LogHelper.Info(string.Format("金币支付回调Goldpay。outTradeId：{0}，sign：{1}，payment：{2}，gold：{3}，money：{4}，couponCount：{5}，tradeType：{6}", outTradeId, sign, payment, gold, money, couponCount, tradeType));
            List<int> payments = new List<int>();
            if (tradeType == 0)
            {
                payments = PaySourceVM.GetSecuriedTransactionPayment();

            }
            else if (tradeType == 1)
            {
                payment = payment + 1000;
                payments = PaySourceVM.GetDirectArrivalPayment();

            }

            if (!payments.Contains(payment))
            {
                Response.Write("fail");
                LogHelper.Error("回调支付方式不存在outTradeId=" + outTradeId + ",payment=" + payment);
                return;
            }
            try
            {
                string strPayment = PaySourceVM.GetPaymentName(payment);
                string out_trade_no = outTradeId;
                if (!string.Equals(sign, Common.CustomConfig.GoldPayNotifySign, StringComparison.CurrentCultureIgnoreCase))
                {
                    Response.Write("fail");
                    LogHelper.Error("" + strPayment + "回调失败：签名验证失败,服务端签名(" + Common.CustomConfig.GoldPayNotifySign + "),请求签名(" + sign + ")");
                    return;
                }
                Guid orderId;
                if (!Guid.TryParse(out_trade_no, out orderId))
                {
                    Response.Write("fail");
                    LogHelper.Error("" + strPayment + "回调失败：订单号不正确outTradeId:" + out_trade_no);
                    return;
                }

                Deploy.CustomDTO.ResultDTO result = UpdateOrderStatus(orderId, payment, gold, money, couponCount);
                if (result == null || result.ResultCode != 0)
                {
                    Response.Write("fail");
                    LogHelper.Error(string.Format(strPayment + "支付回调失败：更新订单状态失败。out_trade_no:{0},ErrorMessage:{1}", out_trade_no, result.Message));
                    return;
                }
                else
                {
                    JdOrderHelper.UpdateJdorder(orderId);
                    CreateYJBJCard(orderId);
                    JinXiaoCunPay(orderId);  //进销存操作
                    new IBP.Facade.CommodityOrderFacade().SendPayInfoToYKBDMq(orderId);//盈科大数据mq
                    YXOrderHelper.CreateOrder(orderId);//网易严选订单
                    ThirdECommerceOrderHelper.CreateOrder(orderId);//标准接口接入第三方电商订单
                    SuningSV.suning_govbus_confirmorder_add(orderId, false);//苏宁确认预占订单
                    //FangZhengSV.FangZheng_Order_Confirm(orderId, false);//方正确认预占订单
                }

                Response.Write("success");
                LogHelper.Info("" + strPayment + "回调成功：outTradeId" + out_trade_no);
            }
            catch (Exception ex)
            {
                Response.Write("fail");
                LogHelper.Error(string.Format("金币支付回调失败Goldpay。outTradeId：{0}，sign：{1}，payment：{2}，gold：{3}，money：{4}，couponCount：{5}，tradeType：{6}", outTradeId, sign, payment, gold, money, couponCount, tradeType), ex);
            }
        }

        public ActionResult UpdateJdorders(string orderids)
        {
            foreach (var orderid in orderids.Split(','))
            {
                JdOrderHelper.UpdateJdorder(new Guid(orderid));
            }
            return Content("ok");
        }

        /// <summary>
        /// 修改订单状态为支付成功
        /// </summary>
        /// <param name="orderId">订单id</param>
        /// <param name="payment">付款方式:金币=0，到付=1，支付宝=2</param>
        /// <returns></returns>
        [NonAction()]
        public Deploy.CustomDTO.ResultDTO UpdateOrderStatus(Guid orderId, int payment, ulong gold, decimal money, decimal couponCount)
        {
            try
            {
                //匿名账号
                Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade facade = new ISV.Facade.CommodityOrderFacade();
                facade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                ResultDTO result = facade.PayUpdateCommodityOrder(orderId, Guid.Empty, Guid.Empty, payment, gold, money, couponCount);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("PaymentNotifyController中调用Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade.PayUpdateCommodityOrder接口异常orderId：{0}，payment：{1}，gold：{2}，money：{3}，couponCount：{4}", orderId, payment, gold, money, couponCount), ex);

                ResultDTO result = new ResultDTO();
                result.ResultCode = 1;
                result.Message = "接口异常，请稍后重试！";
                return result;
            }
        }

        public ActionResult UpdateCommodityOrderStatus(Guid orderId, int payment, ulong gold, decimal money, decimal couponCount)
        {
            var result = UpdateOrderStatus(orderId, payment, gold, money, couponCount);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public void GoldRefund()
        {
            if (Request.Url != null) LogHelper.Debug("调用GoldRefund开始:" + Request.Url.Query);
            //bool isValid = SignHelper.ValidSignRequest(this.Request);
            //if (!isValid)
            if (!string.Equals(this.Request.QueryString["sign"], Common.CustomConfig.GoldPayNotifySign, StringComparison.CurrentCultureIgnoreCase))
            {
                LogHelper.Error("调用GoldRefund方法失败,签名验证错误，参数可能被修改,签名:" + this.Request.QueryString["sign"]);
                Response.Write("fail");
            }
            else
            {
                try
                {
                    Guid orderId;
                    if (!Guid.TryParse(this.Request.QueryString["outTradeId"], out orderId))
                    {
                        Response.Write("fail");
                        LogHelper.Error("调用GoldRefund方法失败,订单号不正确outTradeId:" + this.Request.QueryString["outTradeId"]);
                        return;
                    }
                    Jinher.AMP.BTP.IBP.Facade.CommodityOrderFacade facade = new IBP.Facade.CommodityOrderFacade();
                    facade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                    var result = facade.CancelTheOrderCallBack(new CancelTheOrderDTO { OrderId = orderId, UserId = new Guid("00000000-0000-0000-0000-000000000000") });
                    if (result.ResultCode == 0)
                    {
                        JinXiaoCunRefund(orderId);
                        new IBP.Facade.CommodityOrderFacade().SendRefundInfoToYKBDMq(orderId);
                        Response.Write("success");
                    }
                    else
                    {
                        LogHelper.Error("调用GoldRefund方法失败:" + ",ResultCode:" + result.ResultCode + ",Message:" + result.Message);
                        Response.Write("fail");
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(string.Format("GoldRefund回调失败。orderId：{0}，state：{1}，message：{2}，userId：{3}", new Guid(this.Request.QueryString["outTradeId"]), 7, "", new Guid("00000000-0000-0000-0000-000000000000")), ex);
                    Response.Write("fail");
                }
            }
        }

        /// <summary>
        /// 餐饮订单商家取消订单全额退款回调方法，提供给FSP退款接口回调
        /// </summary>
        public void CancelPayRefund()
        {
            if (Request.Url != null) LogHelper.Debug("调用CancelPayRefund开始:" + Request.Url.Query);
            if (!string.Equals(this.Request.QueryString["sign"], Common.CustomConfig.GoldPayNotifySign, StringComparison.CurrentCultureIgnoreCase))
            {
                LogHelper.Error("调用CancelPayRefund方法失败,签名验证错误，参数可能被修改,签名:" + this.Request.QueryString["sign"]);
                Response.Write("fail");
            }
            else
            {
                try
                {
                    Jinher.AMP.BTP.IBP.Facade.CommodityOrderFacade facade = new IBP.Facade.CommodityOrderFacade();
                    facade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                    var result = facade.CancelTheOrderCallBack(new CancelTheOrderDTO { OrderId = new Guid(this.Request.QueryString["outTradeId"]), UserId = new Guid("00000000-0000-0000-0000-000000000000") });
                    if (result.ResultCode == 0)
                    {
                        Response.Write("success");
                    }
                    else
                    {
                        LogHelper.Error("调用CancelPayRefund方法失败:" + ",ResultCode:" + result.ResultCode + ",Message:" + result.Message);
                        Response.Write("fail");
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(string.Format("CancelPayRefund回调失败。orderId：{0}，state：{1}，message：{2}，userId：{3}", new Guid(this.Request.QueryString["outTradeId"]), 7, "", new Guid("00000000-0000-0000-0000-000000000000")), ex);
                    Response.Write("fail");
                }
            }
        }

        /// <summary>
        /// 支付URL加密处理
        /// </summary>
        /// <returns></returns>
        public ActionResult SignUrl(SignUrlDTO signUrlDTO)
        {

            ISV.Facade.CommodityOrderFacade facade = new ISV.Facade.CommodityOrderFacade();
            var result = facade.OrderSign(signUrlDTO);
            //LogHelper.Info(string.Format("URL加密参数：userId={0},payeeId={1},outTradeId={2},money={3},gold={4},sign={5},PartnerPrivKey={6},paraArray={7},couponCount={8},couponCodes={9}", signUrlDTO.userId, signUrlDTO.payeeId, signUrlDTO.outTradeId, signUrlDTO.money, signUrlDTO.gold, result.Data, CustomConfig.PartnerPrivKey, SignHelper.ConvertDictionaryToUrlParam(paraArray), signUrlDTO.couponCount, signUrlDTO.couponCodes));
            return Json(new { Result = result.Data }, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// 售后退款金币回调
        /// </summary>
        public void GoldRefundAfterSales()
        {
            if (!string.Equals(this.Request.QueryString["sign"], Common.CustomConfig.GoldPayNotifySign, StringComparison.CurrentCultureIgnoreCase))
            {
                LogHelper.Error("调用GoldRefundAfterSales方法失败,签名验证错误，参数可能被修改,签名:" + this.Request.QueryString["sign"]);
                Response.Write("fail");
            }
            else
            {
                try
                {
                    Guid orderId;
                    if (!Guid.TryParse(this.Request.QueryString["outTradeId"], out orderId))
                    {
                        Response.Write("fail");
                        LogHelper.Error("调用GoldRefundAfterSales方法失败,订单号不正确outTradeId:" + this.Request.QueryString["outTradeId"]);
                        return;
                    }
                    Jinher.AMP.BTP.IBP.Facade.CommodityOrderAfterSalesFacade facade = new IBP.Facade.CommodityOrderAfterSalesFacade();
                    facade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                    var result = facade.CancelTheOrderAfterSalesCallBack(new CancelTheOrderDTO { OrderId = orderId });
                    if (result.ResultCode == 0)
                    {
                        JinXiaoCunRefundAfterSales(orderId);
                        new IBP.Facade.CommodityOrderFacade().SendASRefundInfoToYKBDMq(orderId);
                        Response.Write("success");
                    }
                    else
                    {
                        Response.Write("fail");
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(string.Format("GoldRefund回调失败。orderId：{0}，state：{1}，message：{2}，userId：{3}", new Guid(this.Request.QueryString["outTradeId"]), 7, "", new Guid("00000000-0000-0000-0000-000000000000")), ex);
                    Response.Write("fail");
                }
            }
        }

        /// <summary>
        /// (虚拟商品)易捷卡密订单调用盈科接口生成卡信息
        /// </summary>
        /// <param name="orderId"></param>
        private void CreateYJBJCard(Guid orderId)
        {
            const string message = "易捷卡密订单调用盈科接口生成卡信息:";
            var result = new IBP.Facade.YJBJCardFacade().Create(orderId);
            var msg = message + result.Message + ",订单Id:" + orderId;
            if (result.isSuccess) LogHelper.Info(msg);
            else LogHelper.Error(msg);
        }

        /// <summary>
        /// 修改订单状态为支付成功 金采支付使用
        /// </summary>
        /// <returns></returns>
        public void UpdateOrderStatusForJc()
        {
            string strOrderId = Request.QueryString["orderId"];
            string strUserId = Request.QueryString["userId"];
            string strAppId = Request.QueryString["appId"];
            string strEsAppId = Request.QueryString["esAppId"];
            try
            {
                var orderId = new Guid(strOrderId);
                //匿名账号
                Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade facade = new ISV.Facade.CommodityOrderFacade();
                facade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                ResultDTO result = facade.PayUpdateCommodityOrderForJc(orderId);
                if (result.ResultCode == 0)
                {
                    //京东商品处理回调
                    JdOrderHelper.UpdateJdorder(orderId);
                    CreateYJBJCard(orderId);
                    JinXiaoCunPay(orderId);  //进销存操作
                    new IBP.Facade.CommodityOrderFacade().SendPayInfoToYKBDMq(orderId);//盈科大数据mq
                    YXOrderHelper.CreateOrder(orderId);//网易严选订单
                    ThirdECommerceOrderHelper.CreateOrder(orderId);//标准接口接入第三方电商订单
                    SuningSV.suning_govbus_confirmorder_add(orderId, false);//苏宁确认预占订单
                    //FangZhengSV.FangZheng_Order_Confirm(orderId, false);//方正确认预占订单
                    string uRl = ("/Mobile/PaySuccess?user=" + strUserId + "&orderId=" + strOrderId + "&shopId=" + strAppId + "&type=shuaxin&orderState=1&appId=" + strEsAppId + "&source=share");
                    Response.Redirect(uRl);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format(
                        "PaymentNotifyController中调用Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade.UpdateOrderStatus接口异常orderId：{0},userId:{1},appId:{2},esAppId:{3}",
                        strOrderId, strUserId, strAppId, strEsAppId), ex);
            }
        }

        /// <summary>
        /// 进销存订单支付成功后续操作
        /// </summary>
        /// <param name="orderId"></param>
        private void JinXiaoCunPay(Guid orderId)
        {
            new IBP.Facade.JdEclpOrderFacade().CreateOrder(orderId, string.Empty);
            new IBP.Facade.JdEclpOrderFacade().SendPayInfoToHaiXin(orderId);
        }

        /// <summary>
        /// 进销存订单售中退款成功后续操作
        /// </summary>
        /// <param name="orderId"></param>
        private void JinXiaoCunRefund(Guid orderId)
        {
            new IBP.Facade.JdEclpOrderFacade().SendRefundInfoToHaiXin(orderId);
        }

        /// <summary>
        /// 进销存订单售后退款成功后续操作
        /// </summary>
        /// <param name="orderId"></param>
        private void JinXiaoCunRefundAfterSales(Guid orderId)
        {
            new IBP.Facade.JdEclpOrderFacade().SendASSingleRefundInfoToHaiXin(orderId, Guid.Empty);
        }
    }
}
