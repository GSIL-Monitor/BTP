
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2014/12/29 15:19:21
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.ISV.Facade
{
    public class CrowdfundingFacade : BaseFacade<ICrowdfunding>
    {

        /// <summary>
        /// 获取众筹
        /// </summary>
        /// <param name="appId">appId</param>
        public Jinher.AMP.BTP.Deploy.CrowdfundingDTO GetCrowdfundingByAppId(System.Guid appId)
        {
            base.Do();
            return this.Command.GetCrowdfundingByAppId(appId);
        }
        /// <summary>
        /// 更多众筹项目
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数量</param>
        public System.Collections.Generic.List<MoreCrowdfundingDTO> GetMoreCrowdfundings(System.Guid userId, int pageIndex, int pageSize)
        {
            base.Do();
            return this.Command.GetMoreCrowdfundings(userId, pageIndex, pageSize);
        }
        /// <summary>
        /// 领取单个分红
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="dividendId">分红Id</param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DrawDividend(System.Guid userId, System.Guid dividendId)
        {
            base.Do();
            return this.Command.DrawDividend(userId, dividendId);
        }
        /// <summary>
        /// 领取用户所有分红
        /// </summary>
        /// <param name="userId">用户Id</param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DrawUserDividends(System.Guid userId)
        {
            base.Do();
            return this.Command.DrawUserDividends(userId);
        }

        public string GetCrowdfundingSlogan(Guid appId)
        {
            base.Do();
            return this.Command.GetCrowdfundingSlogan(appId);
        }

        public string GetCrowdfundingDesc(Guid appId)
        {
            base.Do();
            return this.Command.GetCrowdfundingDesc(appId);
        }

        /// <summary>
        /// 下订单或购物车获取众筹信息
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.UserOrderCarDTO GetUserCrowdfundingBuy(Guid appId, Guid userId)
        {
            base.Do();
            return this.Command.GetUserCrowdfundingBuy(appId, userId);
        }


        /// <summary>
        /// 获取用户众筹汇总信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public CrowdfundingStatisticsDTO GetUserCrowdfundingStatistics(Guid userId)
        {
            base.Do();
            return this.Command.GetUserCrowdfundingStatistics(userId);
        }

        public ResultDTO UpdateUserName(Tuple<Guid, string> userIdName)
        {
            base.Do();
            return this.Command.UpdateUserName(userIdName);
        }

        public ResultDTO UpdateAppName(Tuple<Guid, string> appIdName)
        {
            base.Do();
            return this.Command.UpdateAppName(appIdName);
        }

        /// <summary>
        /// 获得众筹分红更新条数
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public CfDividendMoreDTO IsCfDividendMore(Guid userId)
        {
            base.Do();
            return this.Command.IsCfDividendMore(userId);

        }



        /// <summary>
        ///  获取众筹状态
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public ResultDTO GetCrowdfundingState(Guid appId)
        {
            base.Do();
            return this.Command.GetCrowdfundingState(appId);
        }

        /// <summary>
        /// 众筹每日统计、众筹股东每日统计，更新CrowdfundingDaily、UserCrowdfundingDaily表
        /// </summary>
        /// <param name="calcDate">被统计日期</param>
        /// <returns></returns>
        public bool CalcUserCrowdfundingDaily(DateTime calcDate)
        {
            base.Do();
            return this.Command.CalcUserCrowdfundingDaily(calcDate);
        }

        /// <summary>
        /// 每日计算众筹分红，更新CfOrderDividendDetail、CfDividend表
        /// </summary>
        /// <returns></returns>
        public bool CalcCfDividend()
        {
            base.Do();
            return this.Command.CalcCfDividend();
        }
        /// <summary>
        /// 众筹汇总统计，更新CrowdfundingStatistics表
        /// </summary>
        /// <returns></returns>
        public bool CalcCfStatistics()
        {
            base.Do();
            return this.Command.CalcCfStatistics();
        }

        /// <summary>
        /// 众筹测试方法
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public bool ChangeDate(Guid appId, int day)
        {
            base.Do();
            return this.Command.ChangeDate(appId, day);
        }

        /// <summary>
        /// 众筹测试方法
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public bool DelCf(Guid appId)
        {
            base.Do();
            return this.Command.DelCf(appId);
        }

        /// <summary>
        /// 向前修改众筹时间，重新计算股东
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public bool ChangeCfStartTimeEarlier(Guid appId)
        {
            base.Do();
            return this.Command.ChangeCfStartTimeEarlier(appId);
        }
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.UserOrderCarDTO> GetUserCrowdfundingBuyer(List<Guid> appIds, Guid userId)
        {
            base.Do();
            return this.Command.GetUserCrowdfundingBuyer(appIds, userId);
        }
    }
}