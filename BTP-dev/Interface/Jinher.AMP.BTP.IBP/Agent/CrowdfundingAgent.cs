
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2014/12/29 15:18:48
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Agent.Base;

namespace Jinher.AMP.BTP.IBP.Agent
{

    public class CrowdfundingAgent : BaseBpAgent<ICrowdfunding>, ICrowdfunding
    {
        public ResultDTO AddCrowdfunding(Jinher.AMP.BTP.Deploy.CrowdfundingDTO crowdfundingDTO)
        {
            //定义返回值
            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.AddCrowdfunding(crowdfundingDTO);

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
        public ResultDTO UpdateCrowdfunding(Jinher.AMP.BTP.Deploy.CrowdfundingDTO crowdfundingDTO)
        {
            //定义返回值
            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateCrowdfunding(crowdfundingDTO);

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
        public Jinher.AMP.BTP.Deploy.CrowdfundingDTO GetCrowdfunding(System.Guid id)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CrowdfundingDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCrowdfunding(id);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.GetCrowdfundingsDTO GetCrowdfundings(string appName, int cfState, int pageIndex, int pageSize)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.GetCrowdfundingsDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCrowdfundings(appName, cfState, pageIndex, pageSize);

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
        public GetUserCrowdfundingsDTO GetUserCrowdfundings(System.Guid crowdfundingId, string userName, string userCode, int pageIndex, int pageSize)
        {
            //定义返回值
            GetUserCrowdfundingsDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetUserCrowdfundings(crowdfundingId, userName, userCode, pageIndex, pageSize);

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

        public CommodityOrderVMDTO GetUserCrowdfundingOrders(System.Guid crowdfundingId, System.Guid userId, int pageIndex, int pageSize)
        {
            //定义返回值
            CommodityOrderVMDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetUserCrowdfundingOrders(crowdfundingId, userId, pageIndex, pageSize);

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
        /// 根据appId找appName
        /// </summary>
        /// <returns></returns>
        public AppNameDTO GetAppNameByAppId(Guid appId)
        {
            //定义返回值
            AppNameDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAppNameByAppId(appId);

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
