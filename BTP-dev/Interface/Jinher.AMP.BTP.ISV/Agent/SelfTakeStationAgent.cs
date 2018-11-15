
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2014/4/12 15:03:46
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.BP.Agent.Base;

namespace Jinher.AMP.BTP.ISV.Agent
{
    public class SelfTakeStationAgent : BaseBpAgent<ISelfTakeStation>, ISelfTakeStation
    {
        /// <summary>
        /// 查询自提点
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public SelfTakeStationResultDTO GetSelfTakeStation(Jinher.AMP.BTP.Deploy.CustomDTO.SelfTakeStationSearchDTO search)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.SelfTakeStationResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetSelfTakeStation(search);

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

        public ResultDTO DeleteCityOwner(SelfTakeStationSearchDTO search)
        {
            //定义返回值
            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DeleteCityOwner(search);

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
