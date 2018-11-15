
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2015/10/23 11:03:47
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.BP.Agent.Base;

namespace Jinher.AMP.BTP.ISV.Agent
{

    public class CommodityOrderAfterSalesAgent : BaseBpAgent<ICommodityOrderAfterSales>, ICommodityOrderAfterSales
    {
        public void AutoDaiRefundOrderAfterSales()
        {

            try
            {
                //调用代理方法
                base.Channel.AutoDaiRefundOrderAfterSales();

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SubmitOrderRefundAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.SubmitOrderRefundDTO submitOrderRefundDTO)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SubmitOrderRefundAfterSales(submitOrderRefundDTO);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CancelOrderRefundAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.CancelOrderRefundDTO cancelOrderRefundDTO)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.CancelOrderRefundAfterSales(cancelOrderRefundDTO);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.SubmitOrderRefundDTO GetOrderRefundAfterSales(System.Guid commodityorderId, Guid orderItemId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.SubmitOrderRefundDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetOrderRefundAfterSales(commodityorderId, orderItemId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddOrderRefundExpAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.AddOrderRefundExpDTO addOrderRefundExpDTO)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.AddOrderRefundExpAfterSales(addOrderRefundExpDTO);

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
        public void AutoYiRefundOrderAfterSales()
        {

            try
            {
                //调用代理方法
                base.Channel.AutoYiRefundOrderAfterSales();

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
        public void AutoRefundAndCommodityOrderAfterSales()
        {

            try
            {
                //调用代理方法
                base.Channel.AutoRefundAndCommodityOrderAfterSales();

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
        public void AutoDealOrderAfterSales()
        {
            try
            {
                //调用代理方法
                base.Channel.AutoDealOrderAfterSales();

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
        ///  售后同意退款/退货申请
        /// </summary>
        /// <param name="cancelTheOrderDTO"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CancelTheOrderAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.CancelTheOrderAfterSales(cancelTheOrderDTO);

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
        /// 售后拒绝退款/退货申请
        /// </summary>
        /// <param name="cancelTheOrderDTO"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RefuseRefundOrderAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.RefuseRefundOrderAfterSales(cancelTheOrderDTO);

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
        /// 拒绝原因
        /// </summary>
        /// <param name="refuseDTO"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<int> DealRefuseBusinessAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.RefuseDTO refuseDTO)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<int> result;

            try
            {
                //调用代理方法
                result = base.Channel.DealRefuseBusinessAfterSales(refuseDTO);

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
        /// 获取自提点售后待处理和已处理的自提的订单信息
        /// </summary>
        /// <param name="userId">提货点管理员</param>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="pageSize">页面数量</param>
        /// <param name="state">0：待处理，1：已处理</param>
        /// <returns>自提点售后待处理和已处理的自提的订单信息</returns>
        public List<Deploy.CustomDTO.OrderListCDTO> GetSelfTakeOrderListAfterSales(Guid userId, int pageIndex, int pageSize, string state)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderListCDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetSelfTakeOrderListAfterSales(userId, pageIndex, pageSize, state);
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
        /// 售后自提订单数量
        /// </summary>
        /// <param name="userId">自提点管理员</param>
        /// <returns>售后自提订单数量</returns>
        public Deploy.CustomDTO.ResultDTO<int> GetSelfTakeManagerAfterSales(Guid userId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<int> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetSelfTakeManagerAfterSales(userId);

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
        /// 买家发货（物流信息）后9天（若有延长，则为12天），卖家自动确认收货。
        /// </summary>
        public void AutoDealOrderConfirmAfterSales()
        {

            try
            {
                //调用代理方法
                base.Channel.AutoDealOrderConfirmAfterSales();

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
