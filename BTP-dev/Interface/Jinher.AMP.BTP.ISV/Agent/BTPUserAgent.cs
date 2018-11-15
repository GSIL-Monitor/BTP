
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2014/4/18 14:07:59
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

    public class BTPUserAgent : BaseBpAgent<IBTPUser>, IBTPUser
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateUser(Jinher.AMP.BTP.Deploy.CustomDTO.UserSDTO userDTO)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateUser(userDTO);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.UserSDTO GetUser(System.Guid userId, System.Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.UserSDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetUser(userId, appId);

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
        /// 待自提订单数量
        /// </summary>
        /// <param name="userId">自提点管理员</param>
        /// <returns>待自提订单数量</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<int> GetSelfTakeManager(Guid userId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<int> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetSelfTakeManager(userId);

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
        /// 获取会员信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.VipPromotionDTO> GetVipInfo(Guid userId, Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.VipPromotionDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetVipInfo(userId, appId);

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
