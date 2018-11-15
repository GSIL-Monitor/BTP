
/***************
功能描述: BTPBP
作    者: 
创建时间: 2017/2/16 16:21:19
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.BTP.BE.BELogic;
using Jinher.JAP.Common.Loging;
using System.Data;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 订单打印服务
    /// </summary>
    public partial class OrderPrintBP : BaseBP, IOrderPrint
    {
        /// <summary>
        /// 获取打印的订单数据数据
        /// </summary>
        /// <param name="orderIds"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.PrintOrderDTO> GetPrintOrderExt(System.Collections.Generic.List<System.Guid> orderIds)
        {
            var printOrders = new List<PrintOrderDTO>();
            if (orderIds == null || orderIds.Count == 0)
                return printOrders;

            List<ImportOrderDTO> orders;
            Dictionary<Guid, List<ImportOrderItemDTO>> dicOrderItem;
            new CommodityOrderBP().GetImportOrders(orderIds, out orders, out dicOrderItem);

            if (orders == null || orders.Count == 0) return printOrders;

            orders.ForEach(r =>
            {
                printOrders.Add(new PrintOrderDTO()
                {
                    Orders = r,
                    OrderItems = dicOrderItem.ContainsKey(r.CommodityOrderId) ? dicOrderItem[r.CommodityOrderId] : new List<ImportOrderItemDTO>()
                });
            });
            return printOrders;
        }

        #region 保存打印快递单
        
        /// <summary>
        /// 打印快递单更新保存数据
        /// </summary>
        /// <param name="orders"></param>
        public ResultDTO SavePrintOrdersExt(UpdatePrintDTO orders)
        {
            #region 需求整理
            //订单状态（必填）：未支付=0，未发货=1，
            //已发货=2，确认收货=3，
            //商家取消订单=4，客户取消订单=5，
            //超时交易关闭=6，已退款=7，
            //待发货退款中=8，已发货退款中=9,
            //已发货退款中商家同意退款，商家未收到货=10,付款中=11,
            //金和处理退款中=12,出库中=13，
            //出库中退款中=14，待审核=16，
            //待付款待审核=17，餐饮订单待处理=18，
            //餐饮订单已处理=19

            //判断是否更新订单状态。
            //更新订单物流信息
            //记录物流信息
            //更新打印次数。
            //记录打印日志
            //CancelTheOrderExt
            //ShipUpdataOrderExt
            //GetOrderStateList
            //OrderSV.CanChangeState
            //OrderSV.LockOrder

            //待发货、出库中、待发货退款中   
            //    打印（自动发货）       更新物流信息，如果有退款拒绝退款
            //其他状态都不更新订单洗物流信息 也不更新订单状态，无论选择不选择自动发货
            #endregion
            try
            {
                if (orders == null || orders.Orders == null || orders.Orders.Count == 0)
                    return new ResultDTO() { ResultCode = 1, Message = "参数错误，参数不能为空！" };

                var tempOrderIds = orders.Orders.Select(r => r.OrderId).ToList();
                var newOrders = (from n in CommodityOrder.ObjectSet()
                                  join o in tempOrderIds on n.Id equals o
                                  select n).ToList();


                //var newOrders = (from o in orders.Orders
                //                 join n in tnewOrders on o.OrderId equals n.Id
                //                 select new
                //                  {
                //                      OldOrder = o,
                //                      NewOrder = n
                //                  }).ToList();

                if (newOrders == null || newOrders.Count == 0)
                    return new ResultDTO() { ResultCode = 1, Message = "数据错误" };

                var canToState2 = new List<int>() { 1, 8, 13 };
                var updateStates = newOrders.Where(r => orders.AutoSend && canToState2.Contains(r.State)).Select(r => r).ToList();

                //记录打印日志，延迟提交库
                RecordPrintLog(newOrders, orders);

                if (updateStates.Count > 0)
                {
                    return BatchUpdateOrderStateTo2(updateStates, orders, UpdateOrderStateTo2CallBack);
                }

                if (ContextFactory.CurrentThreadContext.SaveChanges() > 0)
                    return new ResultDTO() { ResultCode = 0, Message = "保存成功" };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("打印快递单更新保存数据。UpdatePrintOrderInfoExt：{0}", JsonHelper.JsonSerializer(orders)), ex);
            }
            return new ResultDTO() { ResultCode = 1, Message = "保存失败" };
        }

        /// <summary>
        /// 记录打印日志，延迟提交库
        /// </summary>
        /// <param name="commodityOrders"></param>
        /// <param name="orders"></param>
        public void RecordPrintLog(List<CommodityOrder> commodityOrders, UpdatePrintDTO orders)
        {
            DateTime now = DateTime.Now;
            var orderPrintLog = OrderPrintLog.CreateOrderPrintLog();
            orderPrintLog.AppId = orders.AppId;
            orderPrintLog.PrintType = orders.PrintType;
            orderPrintLog.PrintUserId = orders.UserId;
            ContextFactory.CurrentThreadContext.SaveObject(orderPrintLog);

            //记录打印次数
            commodityOrders.ForEach(r =>
            {
                if (orderPrintLog.PrintType == 1)
                    r.InvoicePrintCount = r.InvoicePrintCount.HasValue ? r.InvoicePrintCount.Value + 1 : 1;
                if (orderPrintLog.PrintType == 0)
                    r.ExpressPrintCount = r.ExpressPrintCount.HasValue ? r.ExpressPrintCount.Value + 1 : 1;
                r.EntityState = EntityState.Modified;
                r.ModifiedOn = now;
                ContextFactory.CurrentThreadContext.SaveObject(r);

                var log = OrderPrintDetailLog.CreateOrderPrintDetailLog();
                if (orderPrintLog.PrintType == 0)
                {
                    log.ExpressNo = orders.Orders.Find(o => o.OrderId == r.Id).ExpressOrder;
                }
                log.OrderId = r.Id;
                log.PrintId = orderPrintLog.Id;
                ContextFactory.CurrentThreadContext.SaveObject(log);
            });

        }

        /// <summary>
        /// 批量更新订单自动发货
        /// </summary>
        /// <param name="commodityOrders"></param>
        /// <param name="orders"></param>
        /// <param name="updateAfter"></param>
        /// <returns></returns>
        public ResultDTO BatchUpdateOrderStateTo2(List<CommodityOrder> commodityOrders, UpdatePrintDTO orders, Action<List<CommodityOrder>, UpdatePrintDTO> updateAfter)
        {
            try
            {
                var result = new ResultDTO() { ResultCode = 0 };
                commodityOrders.ForEach(r =>
                {
                    UpdateCommodityOrderParamDTO ucopDto = new UpdateCommodityOrderParamDTO()
                    {
                        orderId = r.Id,
                        targetState = 2,
                        remessage = "",
                        userId = orders.UserId
                    };
                    var _result = UpdateOrderStateTo2(ucopDto, r, orders);
                    if (_result.ResultCode != 0) result = _result;
                });

                if (result.ResultCode != 0) return result;

                var savecount = ContextFactory.CurrentThreadContext.SaveChanges();

                ///保存成功之后异步回调
                if (savecount > 0 && updateAfter != null)
                {
                    System.Threading.Tasks.Task.Factory.StartNew(() =>
                    {
                        updateAfter(commodityOrders, orders);
                    });
                }
                if (savecount > 0)
                    return new ResultDTO() { ResultCode = 0, Message = "保存成功" };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("批量更新订单自动发货。BatchUpdateOrderStateTo2：{0}", JsonHelper.JsonSerializer(orders)), ex);
            }
            return new ResultDTO() { ResultCode = 1, Message = "保存失败" };
        }

        /// <summary>
        ///  打印订单状态变为2（已发货）
        /// </summary>
        /// <param name="ucopDto">参数</param>
        /// <param name="commodityOrder">订单信息</param> 
        /// <returns></returns>
        private ResultDTO UpdateOrderStateTo2(UpdateCommodityOrderParamDTO ucopDto, CommodityOrder commodityOrder, UpdatePrintDTO orders)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                DateTime now = DateTime.Now;
                int oldState = commodityOrder.State;
                int newState = ucopDto.targetState;

                #region 退款
                var orderRefund = new CommodityOrderBP().getOrderRefund(ucopDto.orderId);
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

                #region 订单
                string shipExpCo = orders.ShipName;
                string expOrderNo = orders.Orders.Find(r => r.OrderId == ucopDto.orderId).ExpressOrder;
                commodityOrder.ShipExpCo = string.IsNullOrWhiteSpace(shipExpCo) ? "" : shipExpCo.Trim();
                commodityOrder.ExpOrderNo = string.IsNullOrWhiteSpace(expOrderNo) ? "" : expOrderNo.Trim();
                commodityOrder.ExpOrderNo = commodityOrder.ExpOrderNo.Replace("+", "");

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

                #region 保存物流子表
                //保存物流子表
                OrderShipping orderShipping = OrderShipping.CreateOrderShipping();
                orderShipping.OrderId = ucopDto.orderId;
                orderShipping.ShipExpCo = commodityOrder.ShipExpCo;
                orderShipping.ExpOrderNo = commodityOrder.ExpOrderNo;
                contextSession.SaveObject(orderShipping);
                #endregion

                #region 订单日志
                Journal journal = Journal.CreateJournal(ucopDto, commodityOrder, oldState, "商家已发货");
                contextSession.SaveObject(journal);
                #endregion

                return new ResultDTO() { ResultCode = 0, Message = "设置一个订单成功" };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("打印订单状态变为2（已发货）。UpdateOrderStateTo2：{0}", JsonHelper.JsonSerializer(ucopDto)), ex);
            }
            return new ResultDTO() { ResultCode = 1, Message = "设置一个订单失败" };
        }

        /// <summary>
        /// 更新订单成功回调函数
        /// </summary>
        /// <param name="commodityOrder"></param>
        public void UpdateOrderStateTo2CallBack(List<CommodityOrder> commodityOrders, UpdatePrintDTO orders)
        {
            try
            {
                #region 异步发送消息
                System.Threading.ThreadPool.QueueUserWorkItem(
                    a =>
                    {
                        commodityOrders.ForEach(commodityOrder =>
                        {
                            try
                            {
                                AddMessage addmassage = new AddMessage();
                                string type = "order";
                                Guid esAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
                                addmassage.AddMessages(commodityOrder.Id.ToString(), commodityOrder.UserId.ToString(), esAppId, commodityOrder.Code, commodityOrder.State, "", type);
                            }
                            catch (Exception ex)
                            {

                            }
                        });
                    });
                #endregion

                #region 向第三方“快递鸟”发送物流订阅请求
                List<OrderExpressRoute> oers = new List<OrderExpressRoute>();

                orders.Orders.ForEach(r =>
                {
                    if (!string.IsNullOrEmpty(r.ExpressOrder) && !oers.Exists(o=>o.ExpOrderNo==r.ExpressOrder))
                    {
                        //TODO dzc 向第三方“快递鸟”发送物流订阅请求。  
                        oers.Add(new OrderExpressRoute()
                        {
                            ShipExpCo = orders.ShipName,
                            ExpOrderNo = r.ExpressOrder
                        });
                    }
                });
                OrderExpressRouteBP oerBP = new OrderExpressRouteBP();
                oerBP.BatchSubscribeOneOrderExpressExt(oers);
                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("更新打印订单成功回调函数。UpdateOrderStateTo2CallBack：{0}", JsonHelper.JsonSerializer(orders)), ex);
            }
        }

        #endregion

        #region 保存打印发货单
        /// <summary>
        /// 保存打印发货单
        /// </summary>
        /// <param name="orders"></param>
        public ResultDTO SavePrintInvoiceOrdersExt(UpdatePrintDTO orders)
        {
            
            try
            {
                if (orders == null || orders.Orders == null || orders.Orders.Count == 0)
                    return new ResultDTO() { ResultCode = 1, Message = "参数错误，参数不能为空！" };

                var tempOrderIds = orders.Orders.Select(r => r.OrderId).ToList();
                var newOrders = (from n in CommodityOrder.ObjectSet()
                                 join o in tempOrderIds on n.Id equals o
                                 select n).ToList();


                if (newOrders == null || newOrders.Count == 0)
                    return new ResultDTO() { ResultCode = 1, Message = "数据错误" };

                //记录打印日志，延迟提交库
                RecordPrintLog(newOrders, orders);

                if (ContextFactory.CurrentThreadContext.SaveChanges() > 0)
                    return new ResultDTO() { ResultCode = 0, Message = "保存成功" };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("保存打印发货单。SavePrintInvoiceOrdersExt：{0}", JsonHelper.JsonSerializer(orders)), ex);
            }
            return new ResultDTO() { ResultCode = 1, Message = "保存失败" };
        }
        #endregion
    }
}