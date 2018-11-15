using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.BE;
using System.Net;
using Jinher.JAP.Common.Loging;
using System.Configuration;
using Jinher.JAP.PL;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.CustomDTO.YX;

namespace Jinher.AMP.BTP.TPS.Helper
{
    /// <summary>
    /// 订单操作事件
    /// </summary>
    public static class OrderEventHelper
    {
        static readonly string msgUrl;

        static OrderEventHelper()
        {
            var cofMsgUrl = ConfigurationManager.AppSettings["MsgUrl"];
            if (!string.IsNullOrEmpty(cofMsgUrl))
            {
                // 正式 http://msgcenterpublisher.iuoooo.com:8072/
                msgUrl = cofMsgUrl;
            }
            else
            {
                // 测试
                msgUrl = "http://offlinemsgcenter.iuoooo.com:8072/";
            }
        }

        /// <summary>
        /// 订单支付成功时调用
        /// </summary>
        /// <param name="order"></param>
        public static void OnOrderPaySuccess(CommodityOrder order)
        {
            LogHelper.Info("进入OrderEventHelper.OnOrderPaySuccess-" + order.Code + "，OrderId:" + order.Id + "，OrderState:" + order.State);

            // 拼图支付成功时不触发
            if (order.State != 1)
            {
                return;
            }

            // 发送消息
            var requestPar = "{\"project\":\"btp\",\"topic\":\"btporder-0x0001\",\"data\":\"";
            var data = "{\"orderId\": \"" + order.Id + "\",\"userId\": \"" + order.UserId + "\",\"appId\": \"" + order.AppId + "\"}";
            requestPar += System.Web.HttpUtility.UrlEncode(data, Encoding.GetEncoding("gbk")) + "\"}";
            var requestUrl = msgUrl + "?" + System.Web.HttpUtility.UrlEncode(requestPar, Encoding.GetEncoding("gbk"));
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        LogHelper.Info("OrderEventHelper.OnOrderPaySuccess 发送消息成功， RequestUrl: " + requestUrl);
                    }
                    else
                    {
                        LogHelper.Error("OrderEventHelper.OnOrderPaySuccess 发送消息失败， RequestUrl: " + requestUrl + " \r\nResponseCode: " + response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("OrderEventHelper.OnOrderPaySuccess 发送消息异常， RequestUrl: " + requestUrl, ex);
            }


            switch (ThirdECommerceHelper.GetAppThirdECommerceType(order.AppId))
            {
                case Deploy.Enum.ThirdECommerceTypeEnum.JingDongDaKeHu:
                    // 更新京东订单
                    JdOrderHelper.UpdateJdorder(order.Id);
                    break;
                case Deploy.Enum.ThirdECommerceTypeEnum.SuNingYiGou:
                    // 确认苏宁预占
                    SuningSV.suning_govbus_confirmorder_add(order.Id, false);
                    break;
                case Deploy.Enum.ThirdECommerceTypeEnum.FangZheng:
                    // 方正电商订单
                    FangZhengSV.FangZheng_Order_Confirm(order.Id, false);
                    break;
            }
        }

        /// <summary>
        /// 支付成功减库存时调用
        /// </summary> 
        public static void SubStock()
        {

        }

        /// <summary>
        /// 取消订单加库存时调用
        /// </summary>
        /// <param name="orderItem"></param>
        /// <param name="needRefreshCacheCommoditys"></param> 
        public static void AddStock(OrderItem orderItem, List<Commodity> needRefreshCacheCommoditys)
        {
            var contextSession = ContextFactory.CurrentThreadContext;
            var presents = OrderItemPresent.ObjectSet().Where(_ => _.OrderItemId == orderItem.Id).ToList();
            var presentCommodityIds = presents.Select(a => a.CommodityId).ToList();
            var presentCommodityList = Commodity.ObjectSet().Where(a => presentCommodityIds.Contains(a.Id) && !a.IsDel).ToList();
            var presentCommodityStockIds = presents.Where(c => c.CommodityStockId.HasValue).Select(a => a.CommodityStockId).Distinct().ToList();
            List<CommodityStock> presentCommodityStockList = new List<CommodityStock>();
            if (presentCommodityStockIds.Count > 0)
            {
                presentCommodityStockList = CommodityStock.ObjectSet().Where(c => presentCommodityIds.Contains(c.Id)).ToList();
            }
            foreach (var present in presents)
            {
                Commodity com = presentCommodityList.First(c => c.Id == orderItem.CommodityId);
                if (present.CommodityStockId.HasValue && present.CommodityStockId.Value != Guid.Empty)
                {
                    CommodityStock cStock = presentCommodityStockList.FirstOrDefault(c => orderItem.CommodityStockId == c.Id);
                    cStock.EntityState = System.Data.EntityState.Modified;
                    cStock.Stock += present.Number;
                    contextSession.SaveObject(cStock);
                }
                com.EntityState = System.Data.EntityState.Modified;
                com.Stock += present.Number;
                contextSession.SaveObject(com);
                needRefreshCacheCommoditys.Add(com);
            }
        }

        /// <summary>
        /// 整单退款时调用
        /// </summary>
        /// <param name="order"></param>
        /// <param name="oldState"></param>
        /// <param name="refund"></param>
        public static ResultDTO OnOrderRefund(CommodityOrder order, int oldState, OrderRefund refund)
        {
            // 网易严选和其他第三方电商只有在未发货时支持整单退款
            if (oldState == 1)
            {
                return ThirdECommerceHelper.CancelOrder(order);
            }
            return new ResultDTO { isSuccess = true };
        }

        /// <summary>
        /// 取消整单退款时调用
        /// </summary>
        /// <param name="order"></param>
        /// <param name="refund"></param>
        public static ResultDTO OnCancelOrderRefund(CommodityOrder order, OrderRefund refund)
        {
            // 判断严选订单
            if (ThirdECommerceHelper.IsWangYiYanXuan(order.AppId))
            {
                return new ResultDTO { ResultCode = 1, Message = "不支持整单取消退款。" };
            }
            return new ResultDTO { isSuccess = true };
        }

        /// <summary>
        /// 单品退款时调用（售前）
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderItem"></param>
        /// <param name="refund"></param>
        public static ResultDTO OnOrderItemRefund(CommodityOrder order, OrderItem orderItem, OrderRefund refund)
        {
            // 判断严选订单
            if (ThirdECommerceHelper.IsWangYiYanXuan(order.AppId))
            {
                return new ResultDTO { isSuccess = false, ResultCode = 1, Message = "确认收货后才能申请售后~" };
            }
            return new ResultDTO { isSuccess = true };
        }

        /// <summary>
        /// 取消单品退款时调用（售前）
        /// </summary>
        public static ResultDTO OnCancelOrderItemRefund(CommodityOrder order, OrderItem orderItem, OrderRefund refund)
        {
            return new ResultDTO { isSuccess = true };
        }

        /// <summary>
        /// 整单退款时调用（售后）
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderItrefundem"></param>
        public static ResultDTO OnOrderRefundAfterSales(CommodityOrder order, OrderRefundAfterSales refund)
        {
            // 判断严选订单
            if (ThirdECommerceHelper.IsWangYiYanXuan(order.AppId))
            {
                return new ResultDTO { isSuccess = false, ResultCode = 1, Message = "该订单不支持整单退款，请选择单品退款~" };
            }
            return new ResultDTO { isSuccess = true };
        }

        /// <summary>
        /// 取消整单退款时调用（售后）
        /// </summary>
        public static ResultDTO OnCancelOrderRefundAfterSales(CommodityOrder order, OrderRefundAfterSales refund)
        {
            // 判断严选订单
            if (ThirdECommerceHelper.IsWangYiYanXuan(order.AppId))
            {

            }
            return new ResultDTO { isSuccess = true };
        }

        /// <summary>
        /// 单品退款时调用（售后）
        /// </summary>
        public static ResultDTO OnOrderItemRefundAfterSales(CommodityOrder order, OrderItem orderItem, OrderRefundAfterSales refund)
        {
            return ThirdECommerceHelper.CreateService(order, orderItem, refund);
        }

        /// <summary>
        /// 取消单品退款时调用（售后）
        /// </summary>
        public static ResultDTO OnCancelOrderItemRefundAfterSales(CommodityOrder order, OrderItem orderItem, OrderRefundAfterSales refund)
        {
            return ThirdECommerceHelper.CancelService(order, orderItem, refund);
        }

        /// <summary>
        /// 添加退货物流信息时调用
        /// </summary>
        public static ResultDTO OnOrderRefundOfferExpress(OrderRefund refund, string trackingCompany, string trackingNum)
        {
            // 判断严选订单
            if (ThirdECommerceHelper.IsWangYiYanXuan(refund.OrderId))
            {
                return new ResultDTO { isSuccess = false, Message = "不支持" };
                //return YXSV.ExpressOfferRefundOrder(refund.ApplyId, new List<ExpressInfo> { new ExpressInfo { trackingCompany = trackingCompany, trackingNum = trackingNum } });
            }
            return new ResultDTO { isSuccess = true };
        }

        /// <summary>
        /// 添加退货物流信息时调用（售后）
        /// </summary>
        public static ResultDTO OnOrderRefundOfferExpress(CommodityOrder order, OrderRefundAfterSales refund, string trackingCompany, string trackingNum)
        {
            // 判断严选订单
            if (ThirdECommerceHelper.IsWangYiYanXuan(order.AppId))
            {
                return YXSV.ExpressOfferRefundOrder(refund.ApplyId, new List<ExpressInfo> { new ExpressInfo { trackingCompany = trackingCompany, trackingNum = trackingNum } });
            }
            return new ResultDTO { isSuccess = true };
        }
    }
}
