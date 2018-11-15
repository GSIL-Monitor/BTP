
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2016/2/15 11:37:53
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

    public class DistributorAgent : BaseBpAgent<IDistributor>, IDistributor
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Guid> SaveDistributorRelation(Jinher.AMP.BTP.Deploy.CustomDTO.DistributorUserRelationDTO distributor)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Guid> result;

            try
            {
                //调用代理方法
                result = base.Channel.SaveDistributorRelation(distributor);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.DistributorProfitsResultDTO GetDistributorProfits(Jinher.AMP.BTP.Deploy.CustomDTO.DistributorProfitsSearchDTO search)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.DistributorProfitsResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetDistributorProfits(search);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateDistributorUserInfo(Jinher.AMP.BTP.Deploy.CustomDTO.UserSDTO uinfo)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateDistributorUserInfo(uinfo);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CheckDistributorUserInfo(Jinher.AMP.BTP.Deploy.CustomDTO.UserSDTO cuinfo)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.CheckDistributorUserInfo(cuinfo);

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
        /// 获取分销商信息
        /// </summary>
        /// <param name="distributorUserRelationDTO"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.DistributorInfoDTO GetDistributorInfo(Jinher.AMP.BTP.Deploy.CustomDTO.DistributorUserRelationDTO distributorUserRelationDTO)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.DistributorInfoDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetDistributorInfo(distributorUserRelationDTO);

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
        /// 获取分销商佣金入账信息
        /// </summary>
        /// <param name="distributeMoneySearch"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.DistributorMoneyResultDTO GetDistributorMoneyInfo(Jinher.AMP.BTP.Deploy.CustomDTO.DistributeMoneySearch distributeMoneySearch)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.DistributorMoneyResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetDistributorMoneyInfo(distributeMoneySearch);

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

        public UserDistributionCheckResultDTO UserDistributionCheck(DistributionSearchDTO search)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.UserDistributionCheckResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UserDistributionCheck(search);

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

        public DistributionIdentitySetFullDTO GetApplySet(DistributionSearchDTO search)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.DistributionIdentitySetFullDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetApplySet(search);

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

        public DistributApplyFullDTO GetApply(DistributionSearchDTO search)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.DistributApplyFullDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetApply(search);

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

        public ResultDTO SaveApply(DistributApplyFullDTO dto)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SaveApply(dto);

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

        public ResultDTO SaveMicroshop()
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SaveMicroshop();

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
