
/***************
功能描述: BTP-OPTProxy
作    者: 
创建时间: 2015/7/17 18:19:53
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
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.ISV.Agent
{

    public class UserSpreaderAgent : BaseBpAgent<IUserSpreader>, IUserSpreader
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveSpreaderAndBuyerWxRel(Jinher.AMP.BTP.Deploy.CustomDTO.SpreaderAndBuyerWxDTO sbwxDto)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SaveSpreaderAndBuyerWxRel(sbwxDto);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateOrderSpreader(Jinher.AMP.BTP.Deploy.CustomDTO.SpreaderAndBuyerWxDTO sbwxDto)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateOrderSpreader(sbwxDto);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateToSpreader(SpreaderAndBuyerWxDTO spreaderDto)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateToSpreader(spreaderDto);

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
        /// 绑定关系
        /// </summary>
        /// <param name="userSpreaderBindDTO">参数只传SpreadCode、UserID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveUserSpreaderCode(Jinher.AMP.BTP.Deploy.CustomDTO.UserSpreaderBindDTO userSpreaderBindDTO)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SaveUserSpreaderCode(userSpreaderBindDTO);

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
