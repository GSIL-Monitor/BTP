
/***************
功能描述: BTPSV
作    者: 
创建时间: 2014/12/29 15:19:24
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using System.Diagnostics;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class CrowdfundingSV : BaseSv, ICrowdfunding
    {

        /// <summary>
        /// 获取众筹
        /// </summary>
        /// <param name="appId">appId</param>
        public Jinher.AMP.BTP.Deploy.CrowdfundingDTO GetCrowdfundingByAppId(System.Guid appId)
        {
            base.Do();
            return this.GetCrowdfundingByAppIdExt(appId);

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
            return this.GetMoreCrowdfundingsExt(userId, pageIndex, pageSize);

        }
        /// <summary>
        /// 领取单个分红
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="dividendId">分红Id</param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DrawDividend(System.Guid userId, System.Guid dividendId)
        {
            base.Do();
            return this.DrawDividendExt(userId, dividendId);

        }
        /// <summary>
        /// 领取用户所有分红
        /// </summary>
        /// <param name="userId">用户Id</param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DrawUserDividends(System.Guid userId)
        {
            base.Do();
            return this.DrawUserDividendsExt(userId);

        }

        public string GetCrowdfundingSlogan(Guid appId)
        {
            base.Do(false);
            return this.GetCrowdfundingSloganExt(appId);

        }

        public string GetCrowdfundingDesc(Guid appId)
        {
            base.Do(false);
            return this.GetCrowdfundingDescExt(appId);

        }

        /// <summary>
        /// 下订单或购物车获取众筹信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.UserOrderCarDTO GetUserCrowdfundingBuy(Guid appId, Guid userId)
        {
            base.Do(false);
            return this.GetUserCrowdfundingBuyExt(appId, userId);
        }


        /// <summary>
        /// 获取用户众筹汇总信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public CrowdfundingStatisticsDTO GetUserCrowdfundingStatistics(Guid userId)
        {
            base.Do(false);
            return this.GetUserCrowdfundingStatisticsExt(userId);
        }

        public ResultDTO UpdateUserName(Tuple<Guid, string> userIdName)
        {
            base.Do(false);
            return this.UpdateUserNameExt(userIdName);
        }

        public ResultDTO UpdateAppName(Tuple<Guid, string> appIdName)
        {
            base.Do(false);
            return this.UpdateAppNameExt(appIdName);
        }

        /// <summary>
        /// 获得众筹分红更新条数
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public CfDividendMoreDTO IsCfDividendMore(Guid userId)
        {
            base.Do(false);
            return this.IsCfDividendMoreExt(userId);
        }

        /// <summary>
        ///  获取众筹状态
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public ResultDTO GetCrowdfundingState(Guid appId)
        {
            base.Do(false);
            return this.GetCrowdfundingStateExt(appId);
        }

        /// <summary>
        /// 众筹每日统计、众筹股东每日统计，更新CrowdfundingDaily、UserCrowdfundingDaily表
        /// </summary>
        /// <param name="calcDate">被统计日期</param>
        /// <returns></returns>
        public bool CalcUserCrowdfundingDaily(DateTime calcDate)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            return this.CalcUserCrowdfundingDailyExt(calcDate);
            timer.Stop();
            LogHelper.Info(string.Format("CrowdfundingSV.CalcUserCrowdfundingDaily：耗时：{0}。", timer.ElapsedMilliseconds));
        }

        /// <summary>
        /// 每日计算众筹分红，更新CfOrderDividendDetail、CfDividend表
        /// </summary>
        /// <returns></returns>
        public bool CalcCfDividend()
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            return this.CalcCfDividendExt();
            timer.Stop();
            LogHelper.Info(string.Format("CrowdfundingSV.CalcCfDividendExt：耗时：{0}。", timer.ElapsedMilliseconds));
        }
        /// <summary>
        /// 众筹汇总统计，更新CrowdfundingStatistics表
        /// </summary>
        /// <returns></returns>
        public bool CalcCfStatistics()
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            return this.CalcCfStatisticsExt();
            timer.Stop();
            LogHelper.Info(string.Format("CrowdfundingSV.CalcCfStatistics：耗时：{0}。", timer.ElapsedMilliseconds));
        }


        public bool ChangeDate(Guid appId, int day)
        {
            base.Do(false);
            return this.ChangeDateExt(appId, day);
        }
        public bool DelCf(Guid appId)
        {
            base.Do(false);
            return this.DelCfExt(appId);
        }
        /// <summary>
        /// 向前修改众筹时间，重新计算股东
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public bool ChangeCfStartTimeEarlier(Guid appId)
        {
            base.Do(false);
            return this.ChangeCfStartTimeEarlierExt(appId);
        }
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.UserOrderCarDTO> GetUserCrowdfundingBuyer(List<Guid> appIds, Guid userId)
        {
            base.Do(false);
            return this.GetUserCrowdfundingBuyerExt(appIds, userId);
        }


    }
}
