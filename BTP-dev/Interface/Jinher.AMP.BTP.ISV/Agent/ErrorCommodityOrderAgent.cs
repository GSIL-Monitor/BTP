
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2016/2/18 15:42:41
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

    public class ErrorCommodityOrderAgent : BaseBpAgent<IErrorCommodityOrder>, IErrorCommodityOrder
    {
        /// <summary>
        /// Job自动处理取消订单时回退积分
        /// </summary>
        public void AutoDealOrderCancelSrore()
        {


            try
            {
                //调用代理方法
                base.Channel.AutoDealOrderCancelSrore();

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
        ///  Job自动处理售中退款时回退积分
        /// </summary>
        public void AutoDealOrderRefundScore()
        {


            try
            {
                //调用代理方法
                base.Channel.AutoDealOrderRefundScore();

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
        ///  Job自动处理售后退款时回退积分
        /// </summary>
        public void AutoDealOrderAfterSalesRefundScore()
        {


            try
            {
                //调用代理方法
                base.Channel.AutoDealOrderAfterSalesRefundScore();

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
        /// Job自动处理回退积分
        /// </summary>
        public void AutoRefundScore()
        {


            try
            {
                //调用代理方法
                base.Channel.AutoRefundScore();

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
