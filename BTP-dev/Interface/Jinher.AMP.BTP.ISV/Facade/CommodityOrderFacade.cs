
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2018/8/3 14:56:59
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.ISV.IService;

namespace Jinher.AMP.BTP.ISV.Facade
{
    public class CommodityOrderFacade : BaseFacade<ICommodityOrder>
    {

        public Jinher.AMP.BTP.Deploy.CustomDTO.OrderResultDTO SaveCommodityOrder(Jinher.AMP.BTP.Deploy.CustomDTO.OrderSDTO orderSDTO)
        {
            base.Do();
            return this.Command.SaveCommodityOrder(orderSDTO);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.OrderResultDTO SavePrizeCommodityOrder(Jinher.AMP.BTP.Deploy.CustomDTO.OrderSDTO orderSDTO)
        {
            base.Do();
            return this.Command.SavePrizeCommodityOrder(orderSDTO);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateCommodityOrder(int state, System.Guid orderId, System.Guid userId, System.Guid appId, int payment, string goldpwd, string remessage)
        {
            base.Do();
            return this.Command.UpdateCommodityOrder(state, orderId, userId, appId, payment, goldpwd, remessage);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderSDTO GetOrderItems(System.Guid commodityorderId, System.Guid userId, System.Guid appId)
        {
            base.Do();
            return this.Command.GetOrderItems(commodityorderId, userId, appId);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderShareDTO GetShareOrderItems(System.Guid commodityorderId)
        {
            base.Do();
            return this.Command.GetShareOrderItems(commodityorderId);
        }
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderListCDTO> GetCommodityOrderByState(System.Guid userId, System.Guid appId, int state, int pageIndex, int pageSize)
        {
            base.Do();
            return this.Command.GetCommodityOrderByState(userId, appId, state, pageIndex, pageSize);
        }
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderListCDTO> GetCommodityOrderByUserID(System.Guid userId, int pageIndex, int pageSize, int? state)
        {
            base.Do();
            return this.Command.GetCommodityOrderByUserID(userId, pageIndex, pageSize, state);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO ConfirmOrder(System.Guid commodityOrderId, string password)
        {
            base.Do();
            return this.Command.ConfirmOrder(commodityOrderId, password);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.NewResultDTO ConfirmPayPrice(System.Guid commodityOrderId, System.Guid userId)
        {
            base.Do();
            return this.Command.ConfirmPayPrice(commodityOrderId, userId);
        }
        public void AutoDealOrder()
        {
            base.Do();
            this.Command.AutoDealOrder();
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.OrderResultDTO SubmitOrder(Jinher.AMP.BTP.Deploy.CustomDTO.OrderQueueDTO orderSDTO)
        {
            base.Do();
            return this.Command.SubmitOrder(orderSDTO);
        }
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.LotteryOrderInfoDTO> GetLotteryOrders(System.Guid lotteryId)
        {
            base.Do();
            return this.Command.GetLotteryOrders(lotteryId);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO GetOrderStateByCode(string orderCode)
        {
            base.Do();
            return this.Command.GetOrderStateByCode(orderCode);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SubmitOrderRefund(Jinher.AMP.BTP.Deploy.CustomDTO.SubmitOrderRefundDTO submitOrderRefundDTO)
        {
            base.Do();
            return this.Command.SubmitOrderRefund(submitOrderRefundDTO);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.SubmitOrderRefundDTO GetOrderRefund(System.Guid commodityorderId, System.Guid orderItemId)
        {
            base.Do();
            return this.Command.GetOrderRefund(commodityorderId, orderItemId);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CancelOrderRefund(System.Guid commodityorderId, int state)
        {
            base.Do();
            return this.Command.CancelOrderRefund(commodityorderId, state);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CancelOrderItemRefund(System.Guid commodityorderId, int state, System.Guid orderItemId)
        {
            base.Do();
            return this.Command.CancelOrderItemRefund(commodityorderId, state, orderItemId);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddOrderRefundExp(System.Guid commodityorderId, string RefundExpCo, string RefundExpOrderNo, System.Guid orderItemId)
        {
            base.Do();
            return this.Command.AddOrderRefundExp(commodityorderId, RefundExpCo, RefundExpOrderNo, orderItemId);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelayConfirmTime(System.Guid commodityorderId)
        {
            base.Do();
            return this.Command.DelayConfirmTime(commodityorderId);
        }
        public void ThreeDayNoPayOrder()
        {
            base.Do();
            this.Command.ThreeDayNoPayOrder();
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO PayUpdateCommodityOrder(System.Guid orderId, System.Guid userId, System.Guid appId, int payment, ulong gold, decimal money, decimal couponCount)
        {
            base.Do();
            return this.Command.PayUpdateCommodityOrder(orderId, userId, appId, payment, gold, money, couponCount);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO PayUpdateCommodityOrderForJc(System.Guid orderId)
        {
            base.Do();
            return this.Command.PayUpdateCommodityOrderForJc(orderId);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelOrder(System.Guid commodityorderId, int IsDel)
        {
            base.Do();
            return this.Command.DelOrder(commodityorderId, IsDel);
        }
        public void AutoDaiRefundOrder()
        {
            base.Do();
            this.Command.AutoDaiRefundOrder();
        }
        public void AutoYiRefundOrder()
        {
            base.Do();
            this.Command.AutoYiRefundOrder();
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.SetOrderResultDTO SaveSetCommodityOrder(Jinher.AMP.BTP.Deploy.CustomDTO.OrderSDTO orderSDTO)
        {
            base.Do();
            return this.Command.SaveSetCommodityOrder(orderSDTO);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteOrders(System.Collections.Generic.List<System.Guid> list)
        {
            base.Do();
            return this.Command.DeleteOrders(list);
        }
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.MainOrdersDTO> GetMianOrderList(System.Guid MainOrderId)
        {
            base.Do();
            return this.Command.GetMianOrderList(MainOrderId);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteMainOrder(System.Guid SubOrderId)
        {
            base.Do();
            return this.Command.DeleteMainOrder(SubOrderId);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO ExpirePayOrder(System.Guid orderId)
        {
            base.Do();
            return this.Command.ExpirePayOrder(orderId);
        }
        public void AutoExpirePayOrder()
        {
            base.Do();
            this.Command.AutoExpirePayOrder();
        }
        public void SendOrderInfoToYKBDMq()
        {
            base.Do();
            this.Command.SendOrderInfoToYKBDMq();
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.SetOrderResultDTO SaveSetCommodityOrderNew(System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderSDTO> orderList)
        {
            base.Do();
            return this.Command.SaveSetCommodityOrderNew(orderList);
        }
        public void CalcOrderException()
        {
            base.Do();
            this.Command.CalcOrderException();
        }
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderListCDTO> GetOrderListByManagerId(System.Guid userId, int pageIndex, int pageSize)
        {
            base.Do();
            return this.Command.GetOrderListByManagerId(userId, pageIndex, pageSize);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderSDTO> GetOrderItemsByPickUpCode(System.Guid managerId, string pickUpCode)
        {
            base.Do();
            return this.Command.GetOrderItemsByPickUpCode(managerId, pickUpCode);
        }
        public void RepairePickUpCode()
        {
            base.Do();
            this.Command.RepairePickUpCode();
        }
        public bool AutoAddOrderScore()
        {
            base.Do();
            return this.Command.AutoAddOrderScore();
        }
        public void AutoRefundAndCommodityOrder()
        {
            base.Do();
            this.Command.AutoRefundAndCommodityOrder();
        }
        public void AutoDealOrderConfirm()
        {
            base.Do();
            this.Command.AutoDealOrderConfirm();
        }
        public void AutoOnlyRefundOrder()
        {
            base.Do();
            this.Command.AutoOnlyRefundOrder();
        }
        public void CheckFinishOrder()
        {
            base.Do();
            this.Command.CheckFinishOrder();
        }
        public void DownloadEInvoiceInfo()
        {
            base.Do();
            this.Command.DownloadEInvoiceInfo();
        }
        public void PrCreateInvoic()
        {
            base.Do();
            this.Command.PrCreateInvoic();
        }
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderListCDTO> GetCommodityOrderByUserIDNew(Jinher.AMP.BTP.Deploy.CustomDTO.OrderQueryParamDTO oqpDTO)
        {
            base.Do();
            return this.Command.GetCommodityOrderByUserIDNew(oqpDTO);
        }

        public List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderListCDTO> GetCustomCommodityOrderByUserIDNew(Jinher.AMP.BTP.Deploy.CustomDTO.OrderQueryParamDTO oqpDTO)
        {
            base.Do();
            return this.Command.GetCustomCommodityOrderByUserIDNew(oqpDTO);
        }

        public Jinher.AMP.BTP.Deploy.CustomDTO.UserOrderCountDTO GetOrderCount(System.Guid userId, System.Guid esAppId)
        {
            base.Do();
            return this.Command.GetOrderCount(userId, esAppId);
        }
        public void ServiceOrderStateChangedNotify()
        {
            base.Do();
            this.Command.ServiceOrderStateChangedNotify();
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateCommodityOrderNew(Jinher.AMP.BTP.Deploy.CustomDTO.UpdateCommodityOrderParamDTO ucopDto)
        {
            base.Do();
            return this.Command.UpdateCommodityOrderNew(ucopDto);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<string> OrderSign(Jinher.AMP.BTP.Deploy.CustomDTO.SignUrlDTO signUrlDTO)
        {
            base.Do();
            return this.Command.OrderSign(signUrlDTO);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<string> GetPayUrl(Jinher.AMP.BTP.Deploy.CustomDTO.PayOrderToFspDTO payOrderToFspDto)
        {
            base.Do();
            return this.Command.GetPayUrl(payOrderToFspDto);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.OrderCheckDTO> GetOrderCheckInfo(Jinher.AMP.BTP.Deploy.CustomDTO.OrderQueryParamDTO search)
        {
            base.Do();
            return this.Command.GetOrderCheckInfo(search);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateOrderServiceTime(Jinher.AMP.BTP.Deploy.CustomDTO.OrderQueryParamDTO search)
        {
            base.Do();
            return this.Command.UpdateOrderServiceTime(search);
        }
        public bool CheckIsMainOrder(System.Guid orderId)
        {
            base.Do();
            return this.Command.CheckIsMainOrder(orderId);
        }
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityOrderDTO> GetCommodityOrder(System.Collections.Generic.List<System.Guid> ids)
        {
            base.Do();
            return this.Command.GetCommodityOrder(ids);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateJobCommodityOrder(System.DateTime time, System.Guid orderId, System.Guid userId, System.Guid appId, int payment, string goldpwd, string remessage)
        {
            base.Do();
            return this.Command.UpdateJobCommodityOrder(time, orderId, userId, appId, payment, goldpwd, remessage);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RenewOrderStatistics()
        {
            base.Do();
            return this.Command.RenewOrderStatistics();
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderStatisticsSDTO>> GetOrderStatistics(Jinher.AMP.Coupon.Deploy.CustomDTO.SearchOrderStatisticsExtDTO search)
        {
            base.Do();
            return this.Command.GetOrderStatistics(search);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateOrderItemYjbPrice()
        {
            base.Do();
            return this.Command.UpdateOrderItemYjbPrice();
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateOrderItemState()
        {
            base.Do();
            return this.Command.UpdateOrderItemState();
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderSDTO GetCommodityOrderSdtoByOrderItemId(System.Guid orderItemId)
        {
            base.Do();
            return this.Command.GetCommodityOrderSdtoByOrderItemId(orderItemId);
        }
        public string GetOrderItemAttrId(System.Guid orderItemId)
        {
            base.Do();
            return this.Command.GetOrderItemAttrId(orderItemId);
        }
        public bool ShipmentRemind(System.Guid commodityOrderId)
        {
            base.Do();
            return this.Command.ShipmentRemind(commodityOrderId);
        }
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.RefundOrderListDTO> GetRefundList(Jinher.AMP.BTP.Deploy.CustomDTO.OrderQueryParamDTO oqpDTO)
        {
            base.Do();
            return this.Command.GetRefundList(oqpDTO);
        }

        public bool PushShareMoney(Guid orderId)
        {
            base.Do();
            return this.Command.PushShareMoney(orderId);
        }
    }
}