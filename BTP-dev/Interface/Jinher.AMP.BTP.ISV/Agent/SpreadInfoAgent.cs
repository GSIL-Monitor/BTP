
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2015/8/27 15:47:12
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

    public class SpreadInfoAgent : BaseBpAgent<ISpreadInfo>, ISpreadInfo
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveToSpreadInfo(Jinher.AMP.BTP.Deploy.SpreadInfoDTO spreadInfo)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SaveToSpreadInfo(spreadInfo);

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
        /// 查询推广主信息
        /// </summary>
        /// <param name="spreadInfoSearchDTO"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.SpreadInfoResultDTO GetSpreadInfo(Jinher.AMP.BTP.Deploy.CustomDTO.SpreadInfoSearchDTO spreadInfoSearchDTO)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.SpreadInfoResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetSpreadInfo(spreadInfoSearchDTO);

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
