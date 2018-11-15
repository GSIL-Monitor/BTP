
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2015/10/23 11:01:26
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Agent.Base;

namespace Jinher.AMP.BTP.IBP.Agent
{

    public class CommodityOrderAfterSalesAgent : BaseBpAgent<ICommodityOrderAfterSales>, ICommodityOrderAfterSales
    {
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
        public Jinher.AMP.BTP.Deploy.CustomDTO.SubmitOrderRefundDTO GetOrderRefundAfterSales(System.Guid commodityorderId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.SubmitOrderRefundDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetOrderRefundAfterSales(commodityorderId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.SubmitOrderRefundDTO GetOrderItemRefundAfterSales(Guid commodityorderId, Guid orderItemId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.SubmitOrderRefundDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetOrderItemRefundAfterSales(commodityorderId, orderItemId);

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
        /// 申请列表
        /// </summary>
        /// <param name="refundInfoDTO"></param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundAfterSalesDTO> GetRefundInfoAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.RefundInfoDTO refundInfoDTO)
        {
            //定义返回值
            List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundAfterSalesDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetRefundInfoAfterSales(refundInfoDTO);

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
        /// 售后延长收货时间
        /// </summary>
        /// <param name="commodityorderId">订单号</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelayConfirmTimeAfterSales(Guid commodityorderId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DelayConfirmTimeAfterSales(commodityorderId);

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
        /// 拒绝收货
        /// </summary>
        /// <param name="cancelTheOrderDTO"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RefuseRefundOrderSellerAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.RefuseRefundOrderSellerAfterSales(cancelTheOrderDTO);

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

        public ResultDTO CancelTheOrderAfterSalesCallBack(CancelTheOrderDTO cancelTheOrderDTO)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.CancelTheOrderAfterSalesCallBack(cancelTheOrderDTO);

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
        /// 售中直接到账退款
        /// </summary>
        /// <param name="orderRefundDto">退款信息</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DirectPayRefundAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundDTO orderRefundDto)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DirectPayRefundAfterSales(orderRefundDto);

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
    }
}
