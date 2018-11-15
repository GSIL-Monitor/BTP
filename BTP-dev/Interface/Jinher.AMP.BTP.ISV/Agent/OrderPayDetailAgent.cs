/***************
功能描述: BTPProxy
作    者: 
创建时间: 2016/6/6 11:44:09
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
    public class OrderPayDetailAgent : BaseBpAgent<IOrderPayDetail>, IOrderPayDetail
    {
        public List<Jinher.AMP.BTP.Deploy.OrderPayDetailDTO> GetOrderPayDetailList(Guid objectid)
        {
            //定义返回值
            List<Jinher.AMP.BTP.Deploy.OrderPayDetailDTO> result;
            try
            {
                //调用代理方法
                result = base.Channel.GetOrderPayDetailList(objectid);

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
