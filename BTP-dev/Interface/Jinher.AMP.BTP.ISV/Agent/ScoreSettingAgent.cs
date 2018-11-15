
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2016/2/17 11:01:19
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

    public class ScoreSettingAgent : BaseBpAgent<IScoreSetting>, IScoreSetting
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.ScoreSettingDTO> GetScoreSettingByAppId(System.Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.ScoreSettingDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetScoreSettingByAppId(appId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.UserScoreDTO> GetUserScoreInApp(Jinher.AMP.BTP.Deploy.CustomDTO.Param2DTO paramDto)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.UserScoreDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetUserScoreInApp(paramDto);

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

        public ResultDTO<OrderScoreCheckResultDTO> OrderScoreCheck(OrderScoreCheckDTO paramDto)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.OrderScoreCheckResultDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.OrderScoreCheck(paramDto);

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
