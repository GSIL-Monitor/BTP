
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2018/7/26 14:46:20
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

    public class YJEmployeeAgent : BaseBpAgent<IYJEmployee>, IYJEmployee
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdataYJEmployeeInfo()
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdataYJEmployeeInfo();

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.YJEmployeeCodeDTO> GetUserCodeByAcccount(System.Guid AppId, System.Guid UserId, string UserAccount)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.YJEmployeeCodeDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetUserCodeByAcccount(AppId, UserId, UserAccount);

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
