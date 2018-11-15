/***************
功能描述: 
作    者: 
创建时间: 2016/5/29 11:37:11
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

    public class OrderFieldAgent : BaseBpAgent<IOrderField>, IOrderField
    {
        /// <summary>
        /// 获取订单内容
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.OrderFieldDTO GetOrderSet(Guid AppId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.OrderFieldDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetOrderSet(AppId);

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
