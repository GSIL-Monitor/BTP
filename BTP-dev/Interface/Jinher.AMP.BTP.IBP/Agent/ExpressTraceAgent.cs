
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2015/12/31 18:23:45
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
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy;

namespace Jinher.AMP.BTP.IBP.Agent
{
    public class ExpressTraceAgent : BaseBpAgent<IExpressTrace>,IExpressTrace
    {


        /// <summary>
        /// 查询物流详细信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<ExpressTraceDTO> GetExpressTraceList(ExpressTraceDTO search)
        {
            //定义返回值
            List<ExpressTraceDTO> result;
            try
            {
                //调用代理方法
                result = base.Channel.GetExpressTraceList(search);

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
        /// 保存物流详细信息
        /// </summary>
        /// <param name="VatInvoiceProof"></param>
        /// <returns></returns>
        public ResultDTO SaveExpressTraceList(List<ExpressTraceDTO> list)
        {
            //定义返回值
            ResultDTO result;
            try
            {
                //调用代理方法
                result = base.Channel.SaveExpressTraceList(list);

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
        /// 根据ExpRouteId删除物流详细信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO DelExpressTrace(Guid ExpRouteId)
        {
            //定义返回值
            ResultDTO result;
            try
            {
                //调用代理方法
                result = base.Channel.DelExpressTrace(ExpRouteId);

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
