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
    public class ShareRedEnvelopeAgent : BaseBpAgent<IShareRedEnvelope>, IShareRedEnvelope
    {

        /// <summary>
        /// 佣金结算
        /// </summary>
        public void SettleCommossion()
        {


            try
            {
                //调用代理方法
                base.Channel.SettleCommossion();

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

        }

        /// <summary>
        /// 发送红包
        /// </summary>
        public void SendRedEnvelope()
        {


            try
            {
                //调用代理方法
                base.Channel.SendRedEnvelope();

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

        }


        /// <summary>
        /// 发送红包
        /// </summary>
        public void HandleInValidRedEnvelope()
        {


            try
            {
                base.Channel.HandleInValidRedEnvelope();

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

        }
        /// <summary>
        /// 处理众筹过期红包
        /// </summary>
        public void HandleCfInValidRedEnvelope()
        {

            try
            {
                base.Channel.HandleCfInValidRedEnvelope();

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
        }

        /// <summary>
        /// 获取红包
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.UserRedEnvelopeDTO GetRedEnvelope(Guid redEnvelopeId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.UserRedEnvelopeDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetRedEnvelope(redEnvelopeId);

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
        /// 领取红包
        /// </summary>
        /// <param name="userRedEnvelopeId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DrawRedEnvelope(Guid userRedEnvelopeId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DrawRedEnvelope(userRedEnvelopeId);

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
        /// 获取我的红包
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.UserRedEnvelopeDTO> GetMyRedEnvelope(Guid userId, int type, int pageIndex, int pageSize)
        {
            //定义返回值
            List<Jinher.AMP.BTP.Deploy.CustomDTO.UserRedEnvelopeDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetMyRedEnvelope(userId, type, pageIndex, pageSize);

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
        /// 获取我的红包
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.UserRedEnvelopeDTO> GetMyOrgRedEnvelope(Guid userId, int type, int pageIndex, int pageSize)
        {
            //定义返回值
            List<Jinher.AMP.BTP.Deploy.CustomDTO.UserRedEnvelopeDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetMyOrgRedEnvelope(userId, type, pageIndex, pageSize);

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

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UseRuleDescription(Jinher.AMP.BTP.Deploy.CustomDTO.RuleDescriptionDTO ruleDescriptionDTO)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UseRuleDescription(ruleDescriptionDTO);

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

        public Jinher.AMP.BTP.Deploy.CustomDTO.RuleDescriptionDTO GetRuleDescription(Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.RuleDescriptionDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetRuleDescription(appId);

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
        /// 发送红包
        /// </summary>
        public void SendCfRedEnvelope()
        {


            try
            {
                //调用代理方法
                base.Channel.SendCfRedEnvelope();

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

        }


        public ShareListResult GetShareList(int pageSize, int pageIndex)
        {
            //定义返回值
            ShareListResult result;

            try
            {
                //调用代理方法
                result = base.Channel.GetShareList(pageSize, pageIndex);

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
