
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2015/7/28 11:43:17
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

    public class CommodityOrderExceptionAgent : BaseBpAgent<ICommodityOrderException>, ICommodityOrderException
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityOrderExceptionDTO>> GetOrderExceptionByAppId(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderExceptionParamDTO dto)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityOrderExceptionDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetOrderExceptionByAppId(dto);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateOrderException(Jinher.AMP.BTP.Deploy.CommodityOrderExceptionDTO dto)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateOrderException(dto);

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
