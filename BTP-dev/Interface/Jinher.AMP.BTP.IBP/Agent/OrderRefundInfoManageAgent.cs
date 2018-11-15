
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2018/7/11 16:12:24
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

    public class OrderRefundInfoManageAgent : BaseBpAgent<IOrderRefundInfoManage>, IOrderRefundInfoManage
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddRefundComdtyInfo(Jinher.AMP.BTP.Deploy.CustomDTO.BOrderRefundInfoCDTO cdto)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.AddRefundComdtyInfo(cdto);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.FOrderRefundInfoCDTO GetOrderRefundInfo(System.Guid orderId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.FOrderRefundInfoCDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetOrderRefundInfo(orderId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundDTO GetOrderRefundInfoByItemId(System.Guid orderItemId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetOrderRefundInfoByItemId(orderItemId);

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
