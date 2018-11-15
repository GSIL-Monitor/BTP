
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2016/10/28 18:04:26
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

    public class CreateOrderAgent : BaseBpAgent<ICreateOrder>, ICreateOrder
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CreateOrderNeedDTO> GetCreateOrderInfo(Jinher.AMP.BTP.Deploy.CustomDTO.CreateOrderSearchDTO search)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CreateOrderNeedDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCreateOrderInfo(search);

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
