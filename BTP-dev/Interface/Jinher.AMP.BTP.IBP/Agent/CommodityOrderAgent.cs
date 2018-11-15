
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2018/3/13 17:26:07
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Agent.Base;

namespace Jinher.AMP.BTP.IBP.Agent
{

    public class CommodityOrderAgent : BaseBpAgent<ICommodityOrder>, ICommodityOrder
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderVM GetCommodityOrder(System.Guid id, System.Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderVM result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityOrder(id, appId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderListDTO GetAllCommodityOrderByAppId(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderSearchDTO search)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderListDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAllCommodityOrderByAppId(search);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderListDTO> GetAllCommodityOrderByEsAppId(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderSearchDTO search)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderListDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAllCommodityOrderByEsAppId(search);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateOrderPrice(System.Guid orderId, decimal price, System.Guid userId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateOrderPrice(orderId, price, userId);

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
        public Jinher.AMP.BTP.Deploy.PaymentsDTO GetPaymentByOrderId(System.Guid orderId, string paymentName)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.PaymentsDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetPaymentByOrderId(orderId, paymentName);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO ConfirmPayment(System.Guid orderId, int payment)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.ConfirmPayment(orderId, payment);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO ImportOrder(System.Collections.Generic.List<System.Guid> orderIds)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.ImportOrder(orderIds);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.SubmitOrderRefundDTO GetOrderRefund(System.Guid commodityorderId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.SubmitOrderRefundDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetOrderRefund(commodityorderId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.SubmitOrderRefundDTO GetOrderItemRefund(System.Guid commodityorderId, System.Guid orderItemId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.SubmitOrderRefundDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetOrderItemRefund(commodityorderId, orderItemId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO ShipUpdataOrder(System.Guid commodityOrderId, string shipExpCo, string expOrderNo)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.ShipUpdataOrder(commodityOrderId, shipExpCo, expOrderNo);

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
        public System.Collections.Generic.List<System.Guid> GetCommodityIdsByOrderId(System.Guid orderId)
        {
            //定义返回值
            System.Collections.Generic.List<System.Guid> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityIdsByOrderId(orderId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AlipayZhiTui(System.Guid commodityorderId, string ReceiverAccount, string Receiver, decimal RefundMoney)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.AlipayZhiTui(commodityorderId, ReceiverAccount, Receiver, RefundMoney);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.QryOrderTradeMoneyDTO> QryAppOrderTradeInfo(string appName)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.QryOrderTradeMoneyDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.QryAppOrderTradeInfo(appName);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ExportResultDTO> ExportResult(Jinher.AMP.BTP.Deploy.CustomDTO.ExportParamDTO param)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ExportResultDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.ExportResult(param);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ExportResultDTO> ExportResult1(Jinher.AMP.BTP.Deploy.CustomDTO.ExportParamDTO param)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ExportResultDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.ExportResult1(param);

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
        public System.Collections.Generic.List<System.Guid> GetEsOrderIds(Jinher.AMP.BTP.Deploy.CustomDTO.ExportParamDTO param)
        {
            //定义返回值
            System.Collections.Generic.List<System.Guid> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetEsOrderIds(param);

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
        public System.Collections.Generic.List<System.Guid> GetOrderIds(Jinher.AMP.BTP.Deploy.CustomDTO.ExportParamDTO param)
        {
            //定义返回值
            System.Collections.Generic.List<System.Guid> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetOrderIds(param);

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
        /// 获取查询的所有阳光餐饮的订单号id
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<Guid> GetTotalOrderIds(Jinher.AMP.BTP.Deploy.CustomDTO.ExportParamDTO param)
        {
            //定义返回值
            List<Guid> result;
            try
            {
                //调用代理方法
                result = base.Channel.GetTotalOrderIds(param);

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



        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateSellersRemark(System.Guid commodityOrderId, string SellersRemark)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateSellersRemark(commodityOrderId, SellersRemark);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultShipDTO ChgOrderShip(System.Guid commodityOrderId, string shipExpCo, string expOrderNo)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultShipDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.ChgOrderShip(commodityOrderId, shipExpCo, expOrderNo);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderCheckAccount> GetMainOrdersPay(string mainOrderIds)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderCheckAccount> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetMainOrdersPay(mainOrderIds);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RefuseRefundOrder(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.RefuseRefundOrder(cancelTheOrderDTO);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RefuseRefundOrderSeller(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.RefuseRefundOrderSeller(cancelTheOrderDTO);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelayConfirmTime(System.Guid commodityorderId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundDTO> GetRefundInfo(Jinher.AMP.BTP.Deploy.CustomDTO.RefundInfoDTO refundInfoDTO)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetRefundInfo(refundInfoDTO);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderDistributionResultDTO GetDistributeOrderList(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderDistributionSearchDTO search)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderDistributionResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetDistributeOrderList(search);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.OrderFullInfo GetFullOrderInfoById(string orderId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.OrderFullInfo result;

            try
            {
                //调用代理方法
                result = base.Channel.GetFullOrderInfoById(orderId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CancelTheOrderCallBack(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.CancelTheOrderCallBack(cancelTheOrderDTO);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderMoneyToDTO GetOrderSource(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderMoneyToSearch search)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderMoneyToDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetOrderSource(search);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderMoneyToDTO GetCommodityOrderMoneyTo(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderMoneyToSearch search)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderMoneyToDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityOrderMoneyTo(search);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CancelTheOrder(Jinher.AMP.BTP.Deploy.CustomDTO.UpdateCommodityOrderParamDTO ucopDto)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.CancelTheOrder(ucopDto);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO BatchUpdateCommodityOrder(string commodityOrderIds)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.BatchUpdateCommodityOrder(commodityOrderIds);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DirectPayRefund(Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundDTO orderRefundDto)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DirectPayRefund(orderRefundDto);

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
        public int GetNewCyUntreatedCount(System.Guid appId, System.DateTime lastPayTime)
        {
            //定义返回值
            int result;

            try
            {
                //调用代理方法
                result = base.Channel.GetNewCyUntreatedCount(appId, lastPayTime);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.Share.ShareOrderDTO GetShareOrderInfoByOrderId(System.Guid orderId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.Share.ShareOrderDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetShareOrderInfoByOrderId(orderId);

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
        public Jinher.AMP.BTP.Deploy.CommodityOrderDTO GetCommodityOrderInfo(System.Guid id)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CommodityOrderDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityOrderInfo(id);

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
        public Jinher.AMP.BTP.Deploy.MainOrderDTO GetMainOrderInfo(System.Guid suborderId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.MainOrderDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetMainOrderInfo(suborderId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateCommodityOrder(Jinher.AMP.BTP.Deploy.CommodityOrderDTO model)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateCommodityOrder(model);

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
        public Jinher.AMP.BTP.Deploy.CommodityOrderDTO GetCommodityOrderbyExpOrderNo(string ExpOrderNo)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CommodityOrderDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityOrderbyExpOrderNo(ExpOrderNo);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ExportOrderForJDDTO> ExportOrderForJD(System.Guid appId, System.DateTime startTime, System.DateTime endTime)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ExportOrderForJDDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.ExportOrderForJD(appId, startTime, endTime);

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


        public decimal CalOneFreight(string FreightTo, Deploy.CustomDTO.TemplateCountDTO tem, Guid templateId)
        {
            decimal result;

            try
            {
                //调用代理方法
                result = base.Channel.CalOneFreight(FreightTo,tem,templateId);
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

        public void SendPayInfoToYKBDMq(Guid orderId)
        {
            try
            {
                //调用代理方法
                base.Channel.SendPayInfoToYKBDMq(orderId);

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

        public void SendRefundInfoToYKBDMq(Guid orderId)
        {
            try
            {
                //调用代理方法
                base.Channel.SendRefundInfoToYKBDMq(orderId);

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

        public void SendASRefundInfoToYKBDMq(Guid orderId)
        {
            try
            {
                //调用代理方法
                base.Channel.SendASRefundInfoToYKBDMq(orderId);

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
    }
}
