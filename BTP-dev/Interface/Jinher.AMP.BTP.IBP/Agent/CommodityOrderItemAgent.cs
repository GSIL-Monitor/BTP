
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2018/7/6 13:45:05
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

    public class CommodityOrderItemAgent : BaseBpAgent<ICommodityOrderItem>, ICommodityOrderItem
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.Commodity.CommodityAndOrderItemDTO>> GetCommodityOrderItemByUserId(string UserId, int PageSize, int PageIndex)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.Commodity.CommodityAndOrderItemDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityOrderItemByUserId(UserId, PageSize, PageIndex);

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

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.Commodity.CommodityAndOrderItemDTO>> GetCommodityOrderItemByOrderId(Guid orderId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.Commodity.CommodityAndOrderItemDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityOrderItemByOrderId(orderId);

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
