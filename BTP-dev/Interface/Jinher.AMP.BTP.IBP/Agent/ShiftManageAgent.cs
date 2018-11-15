
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2017/1/10 13:42:06
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

    public class ShiftManageAgent : BaseBpAgent<IShiftManage>, IShiftManage
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO ShiftExchange(Jinher.AMP.BTP.Deploy.CustomDTO.ShiftLogDTO dto)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.ShiftExchange(dto);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.FShiftLogCDTO GetLastShiftInfo(System.Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.FShiftLogCDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetLastShiftInfo(appId);

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
