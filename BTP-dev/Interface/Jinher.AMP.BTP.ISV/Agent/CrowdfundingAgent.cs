
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2014/12/29 15:19:28
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
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy;

namespace Jinher.AMP.BTP.ISV.Agent
{

    public class CrowdfundingAgent : BaseBpAgent<ICrowdfunding>, ICrowdfunding
    {
        public Jinher.AMP.BTP.Deploy.CrowdfundingDTO GetCrowdfundingByAppId(System.Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CrowdfundingDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCrowdfundingByAppId(appId);

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
        public System.Collections.Generic.List<MoreCrowdfundingDTO> GetMoreCrowdfundings(System.Guid userId, int pageIndex, int pageSize)
        {
            //定义返回值
            System.Collections.Generic.List<MoreCrowdfundingDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetMoreCrowdfundings(userId, pageIndex, pageSize);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DrawDividend(System.Guid userId, System.Guid dividendId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DrawDividend(userId, dividendId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DrawUserDividends(System.Guid userId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DrawUserDividends(userId);

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

        public string GetCrowdfundingSlogan(Guid appId)
        {
            //定义返回值
            string result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCrowdfundingSlogan(appId);

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

        public string GetCrowdfundingDesc(Guid appId)
        {
            //定义返回值
            string result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCrowdfundingDesc(appId);

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
        /// 下订单或购物车获取众筹信息
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.UserOrderCarDTO GetUserCrowdfundingBuy(Guid appId, Guid userId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.UserOrderCarDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetUserCrowdfundingBuy(appId, userId);

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
        /// 获取用户众筹汇总信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CrowdfundingStatisticsDTO GetUserCrowdfundingStatistics(Guid userId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CrowdfundingStatisticsDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetUserCrowdfundingStatistics(userId);

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

        public ResultDTO UpdateUserName(Tuple<Guid, string> userIdName)
        {
            //定义返回值
            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateUserName(userIdName);

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

        public ResultDTO UpdateAppName(Tuple<Guid, string> appIdName)
        {
            //定义返回值
            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateAppName(appIdName);

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
        /// 获得众筹分红更新条数
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public CfDividendMoreDTO IsCfDividendMore(Guid userId)
        {
            //定义返回值
            CfDividendMoreDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.IsCfDividendMore(userId);

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
        ///  获取众筹状态
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public ResultDTO GetCrowdfundingState(Guid appId)
        {
            //定义返回值
            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCrowdfundingState(appId);

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
        ///  众筹每日统计、众筹股东每日统计，更新CrowdfundingDaily、UserCrowdfundingDaily表
        /// </summary>
        /// <param name="calcDate"></param>
        /// <returns></returns>
        public bool CalcUserCrowdfundingDaily(DateTime calcDate)
        {
            //定义返回值
            bool result;

            try
            {
                //调用代理方法
                result = base.Channel.CalcUserCrowdfundingDaily(calcDate);

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
        /// 每日计算众筹分红，更新CfOrderDividendDetail、CfDividend表
        /// </summary>
        /// <returns></returns>
        public bool CalcCfDividend()
        {
            //定义返回值
            bool result;

            try
            {
                //调用代理方法
                result = base.Channel.CalcCfDividend();

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
        /// 众筹汇总统计，更新CrowdfundingStatistics表
        /// </summary>
        /// <returns></returns>
        public bool CalcCfStatistics()
        {
            //定义返回值
            bool result;

            try
            {
                //调用代理方法
                result = base.Channel.CalcCfStatistics();

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

        public bool ChangeDate(Guid appId, int day)
        {
            //定义返回值
            bool result;

            try
            {
                //调用代理方法
                result = base.Channel.ChangeDate(appId, day);

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

        public bool DelCf(Guid appId)
        {
            //定义返回值
            bool result;

            try
            {
                //调用代理方法
                result = base.Channel.DelCf(appId);

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
        /// 向前修改众筹时间，重新计算股东
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public bool ChangeCfStartTimeEarlier(Guid appId)
        {
            //定义返回值
            bool result;

            try
            {
                //调用代理方法
                result = base.Channel.ChangeCfStartTimeEarlier(appId);

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


        public List<UserOrderCarDTO> GetUserCrowdfundingBuyer(List<Guid> appIds, Guid userId)
        {
            //定义返回值
            List<UserOrderCarDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetUserCrowdfundingBuyer(appIds, userId);

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
