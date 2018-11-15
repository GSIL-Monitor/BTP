
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2014/4/3 14:32:49
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.BF.BP.Agent.Base;
using Jinher.JAP.BF.IService.Interface;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy;

namespace Jinher.AMP.BTP.ISV.Agent
{

    public class CommodityOrderAgent : BaseBpAgent<ICommodityOrder>, ICommodityOrder
    {

        public Jinher.AMP.BTP.Deploy.CustomDTO.OrderResultDTO SaveCommodityOrder(Jinher.AMP.BTP.Deploy.CustomDTO.OrderSDTO orderSDTO)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.OrderResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SaveCommodityOrder(orderSDTO);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.OrderResultDTO SavePrizeCommodityOrder(Jinher.AMP.BTP.Deploy.CustomDTO.OrderSDTO orderSDTO)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.OrderResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SavePrizeCommodityOrder(orderSDTO);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateCommodityOrder(int state, System.Guid orderId, System.Guid userId, System.Guid appId, int payment, string goldpwd, string remessage)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateCommodityOrder(state, orderId, userId, appId, payment, goldpwd, remessage);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        public Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderSDTO GetOrderItems(System.Guid commodityorderId, System.Guid userId, System.Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderSDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetOrderItems(commodityorderId, userId, appId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        public Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderShareDTO GetShareOrderItems(System.Guid commodityorderId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderShareDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetShareOrderItems(commodityorderId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }


        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderListCDTO> GetCommodityOrderByState(System.Guid userId, System.Guid appId, int state, int pageIndex, int pageSize)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderListCDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityOrderByState(userId, appId, state, pageIndex, pageSize);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        /// <summary>
        /// 获取用户所有订单
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="state">订单状态0：未付款|1:未发货|2:已发货|3:交易成功|-1：失败</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderListCDTO> GetCommodityOrderByUserID(System.Guid userId, int pageIndex, int pageSize, int? state)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderListCDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityOrderByUserID(userId, pageIndex, pageSize, state);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO ConfirmOrder(System.Guid commodityOrderId, string password)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.ConfirmOrder(commodityOrderId, password);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        public Jinher.AMP.BTP.Deploy.CustomDTO.NewResultDTO ConfirmPayPrice(System.Guid commodityOrderId, System.Guid userId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.NewResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.ConfirmPayPrice(commodityOrderId, userId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        public void AutoDealOrder()
        {
            try
            {
                //调用代理方法
                base.Channel.AutoDealOrder();

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
        }



        public Jinher.AMP.BTP.Deploy.CustomDTO.OrderResultDTO SubmitOrder(Jinher.AMP.BTP.Deploy.CustomDTO.OrderQueueDTO orderSDTO)
        {
            try
            {
                //调用代理方法
                return base.Channel.SubmitOrder(orderSDTO);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果

        }

        public List<LotteryOrderInfoDTO> GetLotteryOrders(Guid lotteryId)
        {
            try
            {
                //调用代理方法
                return base.Channel.GetLotteryOrders(lotteryId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果

        }

        public ResultDTO GetOrderStateByCode(string orderCode)
        {
            try
            {
                //调用代理方法
                return base.Channel.GetOrderStateByCode(orderCode);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果

        }

        public ResultDTO SubmitOrderRefund(SubmitOrderRefundDTO submitOrderRefundDTO)
        {
            //定义返回值

            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SubmitOrderRefund(submitOrderRefundDTO);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        ///    查看退款申请
        /// </summary>
        /// <param name="commodityorderId"></param>
        /// <returns></returns>
        public SubmitOrderRefundDTO GetOrderRefund(Guid commodityorderId, Guid orderItemId)
        {
            //定义返回值

            SubmitOrderRefundDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetOrderRefund(commodityorderId, orderItemId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        /// 撤销退款申请
        /// </summary>
        /// <param name="commodityorderId"></param>
        /// <returns></returns>
        public ResultDTO CancelOrderRefund(Guid commodityorderId, int state)
        {
            //定义返回值

            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.CancelOrderRefund(commodityorderId, state);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        /// 撤销退款申请
        /// </summary>
        /// <param name="commodityorderId"></param>
        /// <returns></returns>
        public ResultDTO CancelOrderItemRefund(Guid commodityorderId, int state, Guid orderItemId)
        {
            //定义返回值

            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.CancelOrderItemRefund(commodityorderId, state, orderItemId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        /// 退款物流信息提交
        /// </summary>
        /// <param name="commodityorderId"></param>
        /// <param name="RefundExpCo">退货物流公司</param>
        /// <param name="RefundExpOrderNo">退货单号</param>
        /// <returns></returns>
        public ResultDTO AddOrderRefundExp(Guid commodityorderId, string RefundExpCo, string RefundExpOrderNo, Guid orderItemId)
        {
            //定义返回值

            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.AddOrderRefundExp(commodityorderId, RefundExpCo, RefundExpOrderNo, orderItemId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        /// 延长收货时间 
        /// </summary>
        /// <param name="commodityorderId"></param>
        /// <returns></returns>
        public ResultDTO DelayConfirmTime(Guid commodityorderId)
        {
            //定义返回值

            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DelayConfirmTime(commodityorderId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        public void ThreeDayNoPayOrder()
        {
            try
            {
                //调用代理方法
                base.Channel.ThreeDayNoPayOrder();

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
        }
        public ResultDTO PayUpdateCommodityOrder(Guid orderId, Guid userId, Guid appId, int payment, ulong gold, decimal money, decimal couponCount)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.PayUpdateCommodityOrder(orderId, userId, appId, payment, gold, money, couponCount);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public ResultDTO PayUpdateCommodityOrderForJc(Guid orderId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.PayUpdateCommodityOrderForJc(orderId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public ResultDTO DelOrder(Guid commodityorderId, int IsDel)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DelOrder(commodityorderId, IsDel);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        public void AutoDaiRefundOrder()
        {
            try
            {
                //调用代理方法
                base.Channel.AutoDaiRefundOrder();

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
        }

        public void AutoYiRefundOrder()
        {
            try
            {
                //调用代理方法
                base.Channel.AutoYiRefundOrder();

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.SetOrderResultDTO SaveSetCommodityOrder(OrderSDTO orderSDTO)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.SetOrderResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SaveSetCommodityOrder(orderSDTO);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        /// <summary>
        /// 商家批量删除订单
        /// </summary>
        /// <returns></returns>
        public ResultDTO DeleteOrders(List<Guid> list)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DeleteOrders(list);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public List<MainOrdersDTO> GetMianOrderList(Guid MainOrderId)
        {
            //定义返回值
            List<Jinher.AMP.BTP.Deploy.CustomDTO.MainOrdersDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetMianOrderList(MainOrderId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteMainOrder(Guid SubOrderId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DeleteMainOrder(SubOrderId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        public ResultDTO ExpirePayOrder(Guid orderId)
        {
            try
            {
                //调用代理方法
                return base.Channel.ExpirePayOrder(orderId);
            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
        }

        public void AutoExpirePayOrder()
        {
            try
            {
                //调用代理方法
                base.Channel.AutoExpirePayOrder();
            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
        }

        public void SendOrderInfoToYKBDMq()
        {
            try
            {
                //调用代理方法
                base.Channel.SendOrderInfoToYKBDMq();
            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
        }

        public SetOrderResultDTO SaveSetCommodityOrderNew(List<OrderSDTO> orderList)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.SetOrderResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SaveSetCommodityOrderNew(orderList);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        public void CalcOrderException()
        {
            try
            {
                //调用代理方法
                base.Channel.CalcOrderException();
            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
        }

        /// <summary>
        /// 获取提货点管理员所管理的订单信息
        /// </summary>
        /// <param name="userId">提货点管理员</param>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="pageSize">页面数量</param>
        /// <returns>提货点管理员所管理的订单信息</returns>
        public List<OrderListCDTO> GetOrderListByManagerId(Guid userId, int pageIndex, int pageSize)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderListCDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetOrderListByManagerId(userId, pageIndex, pageSize);
            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }
            return result;
        }

        /// <summary>
        /// 获取订货商品清单信息
        /// </summary>
        /// <param name="managerId">管理员ID</param>
        /// <param name="pickUpCode">推广码</param>
        /// <returns>订单编号和商品明细</returns>
        public ResultDTO<CommodityOrderSDTO> GetOrderItemsByPickUpCode(Guid managerId, string pickUpCode)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<CommodityOrderSDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetOrderItemsByPickUpCode(managerId, pickUpCode);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public void RepairePickUpCode()
        {
            try
            {
                //调用代理方法
                base.Channel.RepairePickUpCode();
            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
        }
        /// <summary>
        /// 批量增加售后完成送积分
        /// </summary>
        /// <returns></returns>
        public bool AutoAddOrderScore()
        {

            bool result;
            try
            {
                //调用代理方法
                result = base.Channel.AutoAddOrderScore();

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;

        }


        /// <summary>
        /// 售中买家7天未发货超时处理
        /// </summary>
        public void AutoRefundAndCommodityOrder()
        {


            try
            {
                //调用代理方法
                base.Channel.AutoRefundAndCommodityOrder();

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }
        }

        /// <summary>
        /// 买家发货（物流信息）后9天（若有延长，则为12天），卖家自动确认收货。
        /// </summary>
        public void AutoDealOrderConfirm()
        {
            try
            {
                //调用代理方法
                base.Channel.AutoDealOrderConfirm();

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }
        }

        /// <summary>
        ///处理售中仅退款的申请订单 5天内未响应 交易状态变为 7 已退款
        /// </summary>
        public void AutoOnlyRefundOrder()
        {


            try
            {
                //调用代理方法
                base.Channel.AutoOnlyRefundOrder();

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }
        }
        /// <summary>
        /// 获取订单列表(包含保险订单)
        /// </summary>
        /// <param name="oqpDTO">订单列表查询参数</param>
        /// <returns></returns>
        public List<OrderListCDTO> GetCommodityOrderByUserIDNew(OrderQueryParamDTO oqpDTO)
        {
            //定义返回值
            List<OrderListCDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityOrderByUserIDNew(oqpDTO);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        /// 获取订单列表(不包含保险订单)
        /// </summary>
        /// <param name="oqpDTO">订单列表查询参数</param>
        /// <returns></returns>
        public List<OrderListCDTO> GetCustomCommodityOrderByUserIDNew(OrderQueryParamDTO oqpDTO)
        {
            //定义返回值
            List<OrderListCDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCustomCommodityOrderByUserIDNew(oqpDTO);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        /// 重新校验已完成订单的钱款去向
        /// </summary>
        /// <returns></returns>
        public void CheckFinishOrder()
        {

            try
            {
                //调用代理方法
                base.Channel.CheckFinishOrder();

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果 

        }

        /// <summary>
        /// 中石化电子发票 补发错误发票请求以及下载电子发票接口调用
        /// </summary>
        /// <returns></returns>
        public void DownloadEInvoiceInfo()
        {

            try
            {
                //调用代理方法
                base.Channel.DownloadEInvoiceInfo();

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果 

        }

        /// <summary>
        /// 中石化电子发票 部分退款商品 退完全款后 继续开正常发票
        /// </summary>
        /// <returns></returns>
        public void PrCreateInvoic()
        {

            try
            {
                //调用代理方法
                base.Channel.PrCreateInvoic();

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果 

        }
        /// <summary>
        /// 查询用户订单数量
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="esAppId"></param>
        /// <returns></returns>
        public UserOrderCountDTO GetOrderCount(Guid userId, Guid esAppId)
        {
            //定义返回值
            UserOrderCountDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetOrderCount(userId, esAppId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }


        /// <summary>
        /// 服务订单状态变化发出通知.
        /// </summary>
        public void ServiceOrderStateChangedNotify()
        {


            try
            {
                //调用代理方法
                base.Channel.ServiceOrderStateChangedNotify();

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }
        }
        public ResultDTO<string> OrderSign(SignUrlDTO signUrlDTO)
        {
            //定义返回值
            ResultDTO<string> result;

            try
            {
                //调用代理方法
                result = base.Channel.OrderSign(signUrlDTO);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }


        /// <summary>
        /// 订单状态修改
        /// <para>Service Url: http://testbtp.iuoooo.com/Jinher.AMP.BTP.SV.CommodityOrderSV.svc/UpdateCommodityOrderNew
        /// </para>
        /// </summary>
        /// <param name="ucopDto">参数</param>
        /// <returns></returns>
        public ResultDTO UpdateCommodityOrderNew(UpdateCommodityOrderParamDTO ucopDto)
        {
            //定义返回值
            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateCommodityOrderNew(ucopDto);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        /// 生成在线支付的Url地址
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<string> GetPayUrl(Jinher.AMP.BTP.Deploy.CustomDTO.PayOrderToFspDTO payOrderToFspDto)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<string> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetPayUrl(payOrderToFspDto);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        public ResultDTO<OrderCheckDTO> GetOrderCheckInfo(OrderQueryParamDTO search)
        {
            //定义返回值
            ResultDTO<OrderCheckDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetOrderCheckInfo(search);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        public ResultDTO UpdateOrderServiceTime(OrderQueryParamDTO search)
        {
            //定义返回值
            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateOrderServiceTime(search);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        /// 判断订单是否为拆分订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public bool CheckIsMainOrder(Guid orderId)
        {
            //定义返回值
            bool result;

            try
            {
                //调用代理方法
                result = base.Channel.CheckIsMainOrder(orderId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }



        /// <summary>
        /// 跟据id获取订单内容
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public List<CommodityOrderDTO> GetCommodityOrder(List<Guid> ids)
        {
            //定义返回值
            List<CommodityOrderDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityOrder(ids);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }


        /// <summary>
        /// job更新京东订单内容
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public ResultDTO UpdateJobCommodityOrder(DateTime time, System.Guid orderId, System.Guid userId, System.Guid appId, int payment, string goldpwd, string remessage)
        {
            //定义返回值
            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateJobCommodityOrder(time, orderId, userId, appId, payment, goldpwd, remessage);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        /// <summary>
        /// 更新订单统计表 记录用户近一年的订单总数、金额总数以及最后的交易时间等字段
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RenewOrderStatistics()
        {
            //定义返回值
            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.RenewOrderStatistics();

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        /// 获取符合条件的用户数据
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ResultDTO<List<OrderStatisticsSDTO>> GetOrderStatistics(Jinher.AMP.Coupon.Deploy.CustomDTO.SearchOrderStatisticsExtDTO search)
        {
            //定义返回值
            ResultDTO<List<OrderStatisticsSDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetOrderStatistics(search);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        /// 易捷币抵现订单，按照商品进行拆分
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateOrderItemYjbPrice()
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateOrderItemYjbPrice();

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        /// 处理单品退款，OrderItem表State状态不正确的问题
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateOrderItemState()
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateOrderItemState();

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        /// 根据订单项Id获取订单部分信息 封装给sns使用
        /// </summary>
        /// <param name="orderItemId">订单项id</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderSDTO GetCommodityOrderSdtoByOrderItemId(Guid orderItemId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderSDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityOrderSdtoByOrderItemId(orderItemId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        /// 根据订单项Id获取订单部分信息 封装给sns使用
        /// </summary>
        /// <param name="orderItemId">订单项id</param>
        /// <returns></returns>
        public string GetOrderItemAttrId(Guid orderItemId)
        {
            //定义返回值
            string result;

            try
            {
                //调用代理方法
                result = base.Channel.GetOrderItemAttrId(orderItemId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        public bool ShipmentRemind(Guid commodityOrderId)
        {
            var result = false;
            try
            {
                //调用代理方法
                result = base.Channel.ShipmentRemind(commodityOrderId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        public List<RefundOrderListDTO> GetRefundList(OrderQueryParamDTO oqpDTO)
        {
            List<RefundOrderListDTO> result;
            try
            {
                //调用代理方法
                result = base.Channel.GetRefundList(oqpDTO);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }


        public bool PushShareMoney(Guid orderId)
        {
            bool result;
            try
            {
                //调用代理方法
                result = base.Channel.PushShareMoney(orderId);
            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }
            //返回结果
            return result;
        }
    }
}
