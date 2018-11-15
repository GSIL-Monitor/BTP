
/***************
功能描述: BTPSV
作    者: 
创建时间: 2014/12/29 15:19:26
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.CBC.Deploy.CustomDTO;
using Jinher.AMP.SNS.ISV.Facade;
using Jinher.JAP.PL;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.BaseApp.MessageCenter.Deploy.CustomDTO;
using System.Runtime.Serialization.Json;
using System.IO;
using Jinher.JAP.BaseApp.MessageCenter.ISV.Facade;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.Finance.Deploy.CustomDTO;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using System.Data;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.Cache;
using ReturnInfoDTO = Jinher.AMP.Finance.Deploy.CustomDTO.ReturnInfoDTO;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    public partial class CrowdfundingSV : BaseSv, ICrowdfunding
    {

        /// <summary>
        /// 获取众筹
        /// </summary>
        /// <param name="appId">appId</param>
        public Jinher.AMP.BTP.Deploy.CrowdfundingDTO GetCrowdfundingByAppIdExt(System.Guid appId)
        {

            CrowdfundingDTO crowdfundingDTO = new CrowdfundingDTO();
            try
            {

                var query = Crowdfunding.ObjectSet().Where(q => q.AppId == appId).FirstOrDefault();
                if (query != null)
                {
                    crowdfundingDTO = query.ToEntityData();
                }
                else
                {
                    crowdfundingDTO = null;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取众筹CrowdfundingSV-GetCrowdfundingByAppIdExt。appId：{0}", appId), ex);
                return null;
            }

            return crowdfundingDTO;


        }

        /// <summary>
        /// 更多众筹项目
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数量</param>
        public System.Collections.Generic.List<MoreCrowdfundingDTO> GetMoreCrowdfundingsExt(System.Guid userId, int pageIndex, int pageSize)
        {

            System.Collections.Generic.List<MoreCrowdfundingDTO> moreCrowdfundingDTOlist = new List<MoreCrowdfundingDTO>();
            var queryList = (from a in Crowdfunding.ObjectSet()
                             join c in CrowdfundingCount.ObjectSet()
                             on a.Id equals c.CrowdfundingId
                             join b in UserCrowdfunding.ObjectSet()
                             on a.Id equals b.CrowdfundingId into Temp

                             from d in Temp.DefaultIfEmpty()
                             join e in CrowdfundingStatistics.ObjectSet() on d.UserId equals e.UserId
                             where d.UserId == userId && a.StartTime < DateTime.Now && a.State == 0
                             orderby d.CurrentShareCount descending
                             select new
                             {
                                 a.AppId,
                                 d.Money,
                                 a.ShareCount,
                                 PerShareMoney = a.PerShareMoney,
                                 UserShareCount = d.CurrentShareCount,
                                 UserMoney = d.Money,
                                 CurrentShareCount = c.CurrentShareCount,
                                 e.Total,
                                 UserBuyFlag = d == null
                             }

                         ).ToList();
            if (queryList.Any())
            {
                List<App.Deploy.CustomDTO.AppIdNameIconDTO> list = APPSV.Instance.GetAppListByIdsInfo(queryList.Select(c => c.AppId).ToList());
                foreach (var item in queryList)
                {
                    var appInfo = list.FirstOrDefault(c => c.AppId == item.AppId);
                    if (appInfo == null)
                        continue;

                    MoreCrowdfundingDTO modelItem = new MoreCrowdfundingDTO
                {
                    AppId = item.AppId,
                    AppLogoUrl = appInfo.AppIcon,
                    AppName = appInfo.AppName,
                    ShareCountRemain = item.ShareCount - item.CurrentShareCount,
                    PerShareMoney = item.PerShareMoney,
                    UserShareCount = item.UserShareCount,
                    NextMoney = (item.UserShareCount + 1) * item.PerShareMoney - item.UserMoney,
                    UserTotalDividend = item.Total,
                    UserBuyFlag = item.UserBuyFlag

                };


                }
            }

            return moreCrowdfundingDTOlist;



        }
        /// <summary>
        /// 领取单个分红
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="dividendId">分红Id</param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DrawDividendExt(System.Guid userId, System.Guid dividendId)
        {

            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result = new ResultDTO();
            try
            {
                var query = CfDividend.ObjectSet().Where(q => q.Id == dividendId).FirstOrDefault();

                if (query == null)
                {
                    result.ResultCode = 1;
                    result.Message = "无众筹分红可领";
                    return result;
                }
                else if (query.State == 2)
                {
                    result.ResultCode = 1;
                    result.Message = "众筹分红已过期";
                    return result;
                }
                else if (query.State == 1)
                {
                    result.ResultCode = 1;
                    result.Message = "众筹分红已领";
                    return result;
                }
                else
                {
                    MultiPayeeTradeByPasswordArg arg = new MultiPayeeTradeByPasswordArg();
                    arg.PayeeComments = new List<string>();
                    arg.PayorComments = new List<string>();

                    arg.AppId = query.AppId;
                    arg.PayorId = CustomConfig.CrowdfundingAccount.BTPCrowdfundingAcount;
                    arg.UsageId = CustomConfig.CrowdfundingAccount.BTPCrowdfundingUsageId;
                    arg.Golds = new List<long> { query.Gold };
                    arg.Payees = new List<Tuple<Guid, bool>>();
                    arg.Payees.Add(new Tuple<Guid, bool>(query.UserId, true));
                    arg.BizSystem = "BTP";
                    arg.BizId = query.Id;
                    arg.BizType = "BTP_CrowdfundingDividend";

                    arg.PayorComments.Add("电商众筹分红支出");
                    arg.PayeeComments.Add("电商众筹分红收益");
                    arg.PayorPassword = CustomConfig.ShareGoldAccout.BTPShareAccountPwd;
                    
                    ReturnInfoDTO gReturnDTO = new ReturnInfoDTO();

                    try
                    {
                        gReturnDTO = Jinher.AMP.BTP.TPS.Finance.Instance.MultiPayeeTradeByPassword(arg);
                        if (gReturnDTO.IsSuccess)
                        {
                            query.State = 1;
                            query.EntityState = EntityState.Modified;
                            contextSession.SaveObject(query);
                            int num = contextSession.SaveChanges();
                            if (num > 0)
                            {
                                result.ResultCode = 0;
                                result.Message = "领取众筹分红成功";
                                return result;
                            }
                            else
                            {
                                result.ResultCode = 1;
                                result.Message = "领取众筹分红失败";
                                return result;
                            }
                        }
                        else
                        {
                            result.ResultCode = 1;
                            result.Message = "众筹分红支付失败";

                            LogHelper.Error(string.Format("调用金币MultiPayeeTradeByPassword方法失败,code:{0},错误消息:{1},参数dividendId:{2}", gReturnDTO.Code, gReturnDTO.Info, dividendId));

                            return result;

                        }
                    }

                    catch (Exception ex)
                    {

                        LogHelper.Error(string.Format("获取我的众筹分红CrowdfundingSV-DrawDividendExt-MultiPayeeTrade,arg:{0}", JsonHelper.JsonSerializer(arg)), ex);
                        result.ResultCode = 1;
                        result.Message = "调用金币接口失败";
                        return result;
                    }


                }

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取我的众筹分红CrowdfundingSV-DrawDividendExt。userId：{0}。dividendId：{1}", userId, dividendId), ex);
                result.ResultCode = 1;
                result.Message = ex.Message;
                return result;

            }

        }
        /// <summary>
        /// 领取用户所有分红
        /// </summary>
        /// <param name="userId">用户Id</param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DrawUserDividendsExt(System.Guid userId)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result = new ResultDTO();

            var list = CfDividend.ObjectSet().Where(q => q.UserId == userId).ToList();
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    result = DrawDividendExt(userId, item.Id);
                }
            }
            else
            {
                result.ResultCode = 1;
                result.Message = "无众筹分红可领";
                return result;
            }

            return result;

        }

        /// <summary>
        /// 获取宣传语
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public string GetCrowdfundingSloganExt(Guid appId)
        {
            var result = (from c in Crowdfunding.ObjectSet()
                          where c.AppId == appId
                          select c.Slogan
                 ).FirstOrDefault();
            return result;
        }

        /// <summary>
        /// 获取重筹详细说明
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public string GetCrowdfundingDescExt(Guid appId)
        {

            var result = (from c in Crowdfunding.ObjectSet()
                          where c.AppId == appId
                          select c.Description
                 ).FirstOrDefault();
            return result;
        }


        /// <summary>
        /// 下订单或购物车获取众筹信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.UserOrderCarDTO GetUserCrowdfundingBuyExt(Guid appId, Guid userId)
        {

            UserOrderCarDTO userCrowdfundingDTO = new UserOrderCarDTO();
            try
            {

                //查询众筹状态为进行中的应用
                var CrowdfundingData = Crowdfunding.ObjectSet().FirstOrDefault(e => e.State == 0 && e.AppId == appId && e.StartTime < DateTime.Now);
                if (CrowdfundingData != null)
                {
                    //用户是否是股东
                    var UserCrowdfundingData = UserCrowdfunding.ObjectSet().FirstOrDefault(e => e.CrowdfundingId == CrowdfundingData.Id && e.UserId == userId && e.AppId == appId);
                    if (UserCrowdfundingData != null)
                    {
                        userCrowdfundingDTO.Money = UserCrowdfundingData.Money;
                        userCrowdfundingDTO.CurrentShareCount = UserCrowdfundingData.CurrentShareCount;

                    }
                    userCrowdfundingDTO.PerShareMoney = CrowdfundingData.PerShareMoney;
                    var crowdfundingCount = CrowdfundingCount.ObjectSet().FirstOrDefault(e => e.CrowdfundingId == CrowdfundingData.Id);
                    userCrowdfundingDTO.ShareCountRemain = CrowdfundingData.ShareCount - crowdfundingCount.CurrentShareCount;
                    userCrowdfundingDTO.IsActiveCrowdfunding = true;

                }
                else
                {
                    userCrowdfundingDTO.IsActiveCrowdfunding = false;
                }

            }
            catch (Exception ex)
            {

                LogHelper.Error(string.Format("下订单或购物车获取众筹信息CrowdfundingSV-GetUserCrowdfundingBuyExt。appId：{0}。userId：{1}", appId, userId), ex);
                return null;

            }

            return userCrowdfundingDTO;

        }

        /// <summary>
        /// 获取用户众筹汇总信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public CrowdfundingStatisticsDTO GetUserCrowdfundingStatisticsExt(Guid userId)
        {
            CrowdfundingStatisticsDTO crowdfundingStatisticsDTO = new CrowdfundingStatisticsDTO();

            try
            {
                var query = CrowdfundingStatistics.ObjectSet().Where(q => q.UserId == userId).FirstOrDefault();
                if (query != null)
                {
                    crowdfundingStatisticsDTO = query.ToEntityData();
                }
                else
                {
                    crowdfundingStatisticsDTO = null;
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取用户众筹汇总信息CrowdfundingSV-GetCrowdfundingStatisticsExt。userId：{0}", userId), ex);
                return null;

            }

            return crowdfundingStatisticsDTO;

        }

        public ResultDTO UpdateUserNameExt(Tuple<Guid, string> userIdName)
        {
            ResultDTO result = new ResultDTO();

            var userCrowds = (from u in UserCrowdfunding.ObjectSet()
                              where u.UserId == userIdName.Item1
                              select u).ToList();

            ContextSession session = ContextFactory.CurrentThreadContext;

            foreach (var userCrowd in userCrowds)
            {
                userCrowd.UserName = userIdName.Item2;
                userCrowd.EntityState = EntityState.Modified;
            }

            session.SaveChanges();

            result.ResultCode = 0;

            return result;

        }

        public ResultDTO UpdateAppNameExt(Tuple<Guid, string> appIdName)
        {
            ResultDTO result = new ResultDTO();

            var crowds = (from u in Crowdfunding.ObjectSet()
                          where u.AppId == appIdName.Item1
                          select u).ToList();

            ContextSession session = ContextFactory.CurrentThreadContext;

            foreach (var crowd in crowds)
            {
                crowd.AppName = appIdName.Item2;
                crowd.EntityState = EntityState.Modified;
            }


            var userCrowds = (from u in UserCrowdfundingDaily.ObjectSet()
                              where u.AppId == appIdName.Item1
                              select u).ToList();

            foreach (var userCrowd in userCrowds)
            {
                userCrowd.AppName = appIdName.Item2;
                userCrowd.EntityState = EntityState.Modified;
            }

            session.SaveChanges();

            result.ResultCode = 0;

            return result;
        }


        /// <summary>
        /// 获取众筹状态
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public ResultDTO GetCrowdfundingStateExt(Guid appId)
        {

            ResultDTO result = new ResultDTO { ResultCode = -1, Message = "不存在此记录" };


            var query = Crowdfunding.ObjectSet().Where(q => q.AppId == appId && q.StartTime < DateTime.Now).FirstOrDefault();

            if (query != null)
            {

                if (query.State == 0)
                {
                    result.ResultCode = 0;
                    result.Message = "活动众筹";
                    return result;
                }
                if (query.State == 1)
                {
                    result.ResultCode = 1;
                    result.Message = "已成功众筹";
                    return result;
                }
            }



            return result;


        }

        /// <summary>
        /// 获得众筹分红更新条数
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public CfDividendMoreDTO IsCfDividendMoreExt(Guid userId)
        {

            CfDividendMoreDTO result = new CfDividendMoreDTO();
            var userTime = GlobalCacheWrapper.GetData("G_CfDividendDTime", userId.ToString(), CacheTypeEnum.redisSS, "BTPCache") as DateTime?;
            if (!userTime.HasValue)
                userTime = DateTime.MinValue;
            var count = CfDividend.ObjectSet().Where(c => c.SubTime > userTime.Value).Count();

            result.Count = count;

            GlobalCacheWrapper.Add("G_CfDividendDTime", userId.ToString(), DateTime.Now, CacheTypeEnum.redisSS, "BTPCache");

            return result;


        }

        private const string UCfCalcPrefix = "G_UCfCalc:";

        /// <summary>
        /// 众筹每日统计、众筹股东每日统计，更新CrowdfundingDaily、UserCrowdfundingDaily表
        /// </summary>
        /// <param name="calcDate">被统计日期</param>
        /// <returns></returns>
        public bool CalcUserCrowdfundingDailyExt(DateTime calcDate)
        {
            const int pageSize = 100;
            int start = 0;
            bool isContinue = true;
            DateTime startTime = calcDate.Date;
            DateTime endTime = startTime.AddDays(1);
            DateTime yestorday = startTime.AddDays(-1);
            try
            {
                LogHelper.Info(string.Format("众筹股东计算开始，计算日期:{0}", startTime.ToString("yyyy-MM-dd")));

                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                while (true)
                {
                    //分批次查询众筹基本表
                    var cfs = Crowdfunding.ObjectSet().Where(c => c.StartTime < endTime).OrderBy(c => c.SubTime).Skip(start).Take(pageSize).ToList();
                    if (cfs.Count < pageSize)
                        isContinue = false;
                    start += pageSize;
                    foreach (var crowdfunding in cfs)
                    {
                        //用户股东每次取数量
                        int ucPageSize = 500;
                        int ucStart = 0;
                        bool isUcContinue = true;
                        long currentShareCount = 0;

                        //众筹每日信息
                        CrowdfundingDaily crowdfundingDaily = CrowdfundingDaily.ObjectSet()
                                               .FirstOrDefault(
                                                   c => c.CrowdfundingId == crowdfunding.Id && c.SettlementDate == startTime);
                        if (crowdfundingDaily == null)
                        {
                            crowdfundingDaily = CrowdfundingDaily.CreateCrowdfundingDaily();
                            crowdfundingDaily.CrowdfundingId = crowdfunding.Id;
                            crowdfundingDaily.DividendPercent = crowdfunding.DividendPercent;
                            crowdfundingDaily.SettlementDate = startTime;
                            crowdfundingDaily.EntityState = EntityState.Added;
                        }
                        else
                        {
                            crowdfundingDaily.EntityState = EntityState.Modified;
                        }

                        //计算股东每日信息
                        while (true)
                        {
                            var userCrowdfundings = UserCrowdfunding.ObjectSet().Where(c => c.AppId == crowdfunding.AppId).OrderBy(c => c.SubTime).Skip(ucStart).Take(ucPageSize).ToList();
                            if (userCrowdfundings.Count < ucPageSize)
                                isUcContinue = false;
                            ucStart += ucPageSize;

                            foreach (var userCrowdfunding in userCrowdfundings)
                            {

                                UserCrowdfundingDaily currentUcDaily =
           UserCrowdfundingDaily.ObjectSet()
                                .FirstOrDefault(
                                    c =>
                                    c.UserId == userCrowdfunding.UserId &&
                                    c.AppId == crowdfunding.AppId &&
                                    c.SettlementDate == startTime);
                                if (currentUcDaily == null)
                                {
                                    currentUcDaily = UserCrowdfundingDaily.CreateUserCrowdfundingDaily();
                                    currentUcDaily.UserId = userCrowdfunding.UserId;
                                    currentUcDaily.AppName = crowdfunding.AppName;
                                    currentUcDaily.AppId = crowdfunding.AppId;
                                    currentUcDaily.SettlementDate = startTime;
                                    currentUcDaily.EntityState = EntityState.Added;
                                }
                                else
                                {
                                    currentUcDaily.EntityState = EntityState.Modified;
                                }

                                var crowdfundingPriceList =
                                    CommodityOrder.ObjectSet()
                                                  .Where(
                                                      c =>
                                                      c.AppId == crowdfunding.AppId &&
                                                      c.UserId == userCrowdfunding.UserId &&
                                                      c.IsCrowdfunding == 1 &&
                                                      (c.PaymentTime < endTime || (c.State == 7 && c.ConfirmTime < endTime))
                                                      )
                                                  .Select(c => c.CrowdfundingPrice);
                                if (crowdfundingPriceList.Any())
                                {
                                    currentUcDaily.Money = crowdfundingPriceList.Sum(c => c);
                                    currentUcDaily.ShareCount = (long)(currentUcDaily.Money / crowdfunding.PerShareMoney);
                                }
                                contextSession.SaveObject(currentUcDaily);
                                currentShareCount += currentUcDaily.ShareCount;
                            }
                            if (!isUcContinue)
                                break;
                        }
                        crowdfundingDaily.CurrentShareCount = currentShareCount;
                        contextSession.SaveObject(crowdfundingDaily);
                    }
                    contextSession.SaveChanges();

                    if (!isContinue)
                        break;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("众筹每日统计、众筹股东每日统计：CrowdfundingSV-CalcUserCrowdfundingDaily,参数calcDate:{0}", calcDate.ToString("yyyy-MM-dd")), ex);
                return false;
            }
            LogHelper.Info(string.Format("众筹股东计算完成，计算日期:{0}", startTime.ToString("yyyy-MM-dd")));
            return true;
        }

        /// <summary>
        /// 每日计算众筹分红，更新CfOrderDividendDetail、CfDividend表
        /// </summary>
        /// <returns></returns>
        public bool CalcCfDividendExt()
        {
            try
            {
                const int pageSize = 500;
                int start = 0;
                bool isContinue = true;
                Guid lastAppId = Guid.Empty;
                DateTime lastSubTime = DateTime.MinValue;
                //long dividend = 0;
                LogHelper.Info("众筹分红计算开始");
                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                //每个众筹、每天|每个股东的分红
                Dictionary<Guid, CfDividend> cfDividendDict = new Dictionary<Guid, CfDividend>();

                //每个众筹|每个股东的分红金币
                Dictionary<Guid, long> dividendsDict = new Dictionary<Guid, long>();

                string appName = null;

               
                while (true)
                {
                    var cfOrderDividends = CfOrderDividend.ObjectSet()
                                       .Where(c => c.State == 0 && c.SubTime < DateTime.Today).OrderBy(c => c.AppId).ThenBy(c => c.SubTime)
                                       .Skip(start)
                                       .Take(pageSize)
                                       .ToList();
                    if (!cfOrderDividends.Any())
                        break;
                    if (cfOrderDividends.Count < pageSize)
                        isContinue = false;
                    start += pageSize;
                    //每个众筹，不同日期的股东列表
                    Dictionary<Guid, Dictionary<DateTime, List<UserCrowdfundingDailyDTO>>> dict = new Dictionary<Guid, Dictionary<DateTime, List<UserCrowdfundingDailyDTO>>>();
                    for (int i = 0; i < cfOrderDividends.Count; i++)
                    {
                        var cfOrderDividend = cfOrderDividends[i];
                        if (cfOrderDividend.Gold <= 0 || cfOrderDividend.CurrentShareCount == 0)
                            continue;
                        //订单日期
                        var orderDate = cfOrderDividend.SubTime.Date;
                        //核算分红使用的股东日期
                        var orderCalcDate = orderDate.AddDays(-1);

                        if (appName == null)
                        {
                            //List<App.Deploy.CustomDTO.AppIdNameIconDTO> list = appManage.GetAppListByIds(new List<Guid> { cfOrderDividend.AppId });
                            Dictionary<Guid, string> list = Jinher.AMP.BTP.TPS.APPSV.GetAppNameListByIds(new List<Guid> { cfOrderDividend.AppId });
                            if (list != null && list.Any())
                                appName = list[cfOrderDividend.AppId];
                        }

                        //首次循环，为临时变量赋初值
                        if (lastAppId == Guid.Empty)
                        {
                            lastAppId = cfOrderDividend.AppId;
                            lastSubTime = orderDate;
                        }

                        //每股分账金币数：先计算再取整
                        decimal perGold = 1.0m * cfOrderDividend.Gold / cfOrderDividend.CurrentShareCount;
                        long buyerGold = 0;
                        if (!dict.ContainsKey(cfOrderDividend.AppId))
                            dict.Add(cfOrderDividend.AppId, new Dictionary<DateTime, List<UserCrowdfundingDailyDTO>>());
                        List<UserCrowdfundingDailyDTO> ucDailies;
                        if (!dict[cfOrderDividend.AppId].ContainsKey(orderDate))
                        {
                            ucDailies =
                                UserCrowdfundingDaily.ObjectSet()
                                 .Where(c => c.AppId == cfOrderDividend.AppId && c.SettlementDate == orderCalcDate).OrderBy(c => c.UserId)
                                 .Select(c => new UserCrowdfundingDailyDTO { ShareCount = c.ShareCount, UserId = c.UserId })
                                 .ToList();
                            dict[cfOrderDividend.AppId].Add(orderDate, ucDailies);
                        }
                        else
                        {
                            ucDailies = dict[cfOrderDividend.AppId][orderDate];
                        }

                        foreach (var userCrowdfundingDailyDto in ucDailies)
                        {
                            //计算每个股东针对每一单获得分红，如果不足一个金币，不预分配
                            var orderUserDailyGold = (long)(perGold * userCrowdfundingDailyDto.ShareCount);
                            if (orderUserDailyGold > 0)
                            {
                                CfOrderDividendDetail model = CfOrderDividendDetail.CreateCfOrderDividendDetail();
                                model.UserId = userCrowdfundingDailyDto.UserId;
                                model.AppId = cfOrderDividend.AppId;
                                model.OrderDividendId = cfOrderDividend.Id;
                                model.SettlementDate = orderDate;
                                model.Gold = orderUserDailyGold;
                                model.ShareCount = userCrowdfundingDailyDto.ShareCount;
                                contextSession.SaveObject(model);
                                buyerGold += model.Gold;

                                //换应用，换人，换日期 cfDividend保存
                                if (lastAppId != cfOrderDividend.AppId || lastSubTime != orderDate)
                                {
                                    foreach (var cfd in cfDividendDict.Values)
                                    {
                                        contextSession.SaveObject(cfd);
                                        GlobalCacheWrapper.Add(UCfCalcPrefix + cfd.UserId, cfd.Id.ToString(), cfd.ToEntityData(), "BTPCache");
                                    }

                                    //上一个app已统计完成，则将统计结果存入股东表
                                    if (lastAppId != cfOrderDividend.AppId)
                                    {

                                        Dictionary<Guid, string> list = Jinher.AMP.BTP.TPS.APPSV.GetAppNameListByIds(new List<Guid> { cfOrderDividend.AppId });//appManage.GetAppListByIds(new List<Guid> { cfOrderDividend.AppId });
                                        if (list != null && list.Any())
                                            appName = list[cfOrderDividend.AppId];
                                        else
                                        {
                                            appName = "";
                                        }

                                        foreach (var kv in dividendsDict)
                                        {
                                            UserCrowdfunding uc =
                                                UserCrowdfunding.ObjectSet()
                                                                .FirstOrDefault(
                                                                    c => c.AppId == lastAppId && c.UserId == kv.Key);
                                            if (uc != null)
                                            {
                                                uc.TotalDividend += kv.Value;
                                                uc.EntityState = EntityState.Modified;
                                                contextSession.SaveObject(uc);
                                            }
                                        }
                                        dividendsDict.Clear();
                                    }

                                    //重新为这两个变量赋值
                                    lastAppId = cfOrderDividend.AppId;
                                    lastSubTime = orderDate;
                                    cfDividendDict.Clear();
                                }

                                if (!cfDividendDict.ContainsKey(userCrowdfundingDailyDto.UserId))
                                {

                                    var cfDividend = CfDividend.CreateCfDividend();
                                    cfDividend.UserId = userCrowdfundingDailyDto.UserId;
                                    cfDividend.AppId = cfOrderDividend.AppId;
                                    cfDividend.AppName = appName;
                                    cfDividend.State = 0;
                                    cfDividend.SettlementDate = orderDate;
                                    cfDividend.ShareCount = userCrowdfundingDailyDto.ShareCount;

                                    cfDividendDict.Add(userCrowdfundingDailyDto.UserId, cfDividend);
                                }
                                cfDividendDict[userCrowdfundingDailyDto.UserId].Gold += orderUserDailyGold;

                                //每个众筹|每个股东的分红金币
                                if (!dividendsDict.ContainsKey(userCrowdfundingDailyDto.UserId))
                                    dividendsDict.Add(userCrowdfundingDailyDto.UserId, 0);
                                dividendsDict[userCrowdfundingDailyDto.UserId] += orderUserDailyGold;

                            }

                        }



                        //更新分账状态
                        cfOrderDividend.State = 1;
                        contextSession.SaveObject(cfOrderDividend);

                        #region 分红剩余金额，分到金和众筹账户
                        long jinherGold = cfOrderDividend.Gold - buyerGold;
                        if (jinherGold > 0)
                        {

                            MultiPayeeTradeByPasswordArg arg = new MultiPayeeTradeByPasswordArg();
                            arg.PayeeComments = new List<string>();
                            arg.PayorComments = new List<string>();

                            arg.AppId = cfOrderDividend.AppId;
                            arg.PayorId = CustomConfig.CrowdfundingAccount.BTPCrowdfundingAcount;
                            arg.UsageId = CustomConfig.CrowdfundingAccount.BTPCrowdfundingUsageId;
                            arg.Golds = new List<long> { jinherGold };
                            arg.Payees = new List<Tuple<Guid, bool>>();
                            arg.Payees.Add(new Tuple<Guid, bool>(CustomConfig.CrowdfundingAccount.JhCrowdfundingGoldAccount, true));
                            arg.BizSystem = "BTP";
                            arg.BizId = cfOrderDividend.Id;
                            arg.BizType = "BTP_CrowdfundingDividend_Auto";

                            arg.PayorComments.Add("电商众筹分红支出");
                            arg.PayeeComments.Add("金和众筹分红沉淀");
                            arg.PayorPassword = CustomConfig.CrowdfundingAccount.BTPCrowdfundingPwd;
                           try
                            {
                                ReturnInfoDTO gReturnDTO = new ReturnInfoDTO();
                                gReturnDTO = Jinher.AMP.BTP.TPS.Finance.Instance.MultiPayeeTradeByPassword(arg);
                                if (!gReturnDTO.IsSuccess)
                                {
                                    JAP.Common.Loging.LogHelper.Error(string.Format("众筹每日统计、众筹股东每日统计：CrowdfundingSV.CalcCfDividendExt错误,\r\n Finance.ISV.Facade.GoldDealerFacade.MultiPayeeTradeByPassword错误：错误码:{0},错误信息:{1}", gReturnDTO.Code, gReturnDTO.Info));
                                    isContinue = false;
                                }
                            }

                            catch (Exception ex)
                            {
                                LogHelper.Error(string.Format("众筹每日统计、众筹股东每日统计：CrowdfundingSV.CalcCfDividendExt错误,\r\n Finance.ISV.Facade.GoldDealerFacade.MultiPayeeTradeByPassword异常。arg：{0}", arg), ex);
                                isContinue = false;
                            }
                        }
                        #endregion
                    }
                    contextSession.SaveChanges();
                    if (!isContinue)
                        break;
                }

                foreach (var kv in dividendsDict)
                {
                    UserCrowdfunding uc =
                        UserCrowdfunding.ObjectSet()
                                        .FirstOrDefault(
                                            c => c.AppId == lastAppId && c.UserId == kv.Key);
                    if (uc != null)
                    {
                        uc.TotalDividend += kv.Value;
                        uc.EntityState = EntityState.Modified;
                        contextSession.SaveObject(uc);
                    }
                }

                foreach (var cfd in cfDividendDict.Values)
                {
                    contextSession.SaveObject(cfd);
                    GlobalCacheWrapper.Add(UCfCalcPrefix + cfd.UserId, cfd.Id.ToString(), cfd.ToEntityData(), "BTPCache");
                }
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error("众筹每日统计、众筹股东每日统计：CrowdfundingSV-CalcUserCrowdfundingDaily错误", ex);
                return false;
            }
            LogHelper.Info("众筹分红计算完成");
            return true;
        }
        /// <summary>
        /// 众筹汇总统计，更新CrowdfundingStatistics表，更新UserCrowdfunding表TotalDividend
        /// </summary>
        /// <returns></returns>
        public bool CalcCfStatisticsExt()
        {
            const int pageSize = 500;
            int start = 0;
            bool isContinue = true;
            DateTime settlementDate = DateTime.Today.AddDays(-1);
            DateTime nextDate = DateTime.Today;
            DateTime minDate = DateTime.Today.AddDays(-7);
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            try
            {
                LogHelper.Info("众筹统计计算开始");

                #region 更新历史数据
                while (true)
                {
                    var cfStatistics = CrowdfundingStatistics.ObjectSet().Where(c => c.SettlementDate < settlementDate).OrderBy(c => c.SubTime).Skip(start).Take(pageSize).ToList();
                    if (!cfStatistics.Any())
                        break;
                    if (cfStatistics.Count < pageSize)
                        isContinue = false;
                    start += pageSize;
                    Dictionary<DateTime, List<CfDividendDTO>> dictDate = new Dictionary<DateTime, List<CfDividendDTO>>();
                    Dictionary<Guid, List<CfDividendDTO>> dictApp = new Dictionary<Guid, List<CfDividendDTO>>();
                    foreach (var crowdfundingStatistics in cfStatistics)
                    {

                        var dict = GlobalCacheWrapper.GetAllData<CfDividendDTO>(UCfCalcPrefix + crowdfundingStatistics.UserId, CacheTypeEnum.redisSS, "BTPCache");
                        if (dict.Any())
                        {

                            dictDate = dict.Values.GroupBy(c => c.SettlementDate,
                                                                 (key, group) => new { SettlementDate = key, CfDividendDTOs = group })
                                                        .ToDictionary(c => c.SettlementDate, c => c.CfDividendDTOs.ToList());
                            dictApp = dict.Values.GroupBy(c => c.AppId, (key, group) => new { AppId = key, CfDividendDTOs = group })
                                .ToDictionary(c => c.AppId, c => c.CfDividendDTOs.ToList());

                            var sum = dict.Values.Sum(c => c.Gold);
                            crowdfundingStatistics.Total += sum;
                            crowdfundingStatistics.UnReceive += sum;
                        }

                        long oldOne = crowdfundingStatistics.LastOneDay;
                        long oldTwo = crowdfundingStatistics.LastTwoDay;
                        long oldThree = crowdfundingStatistics.LastThreeDay;
                        long oldFour = crowdfundingStatistics.LastFourDay;
                        long oldFive = crowdfundingStatistics.LastFiveDay;
                        long oldSix = crowdfundingStatistics.LastSixDay;

                        crowdfundingStatistics.LastOneDay = dictDate.ContainsKey(DateTime.Today.AddDays(-1)) ? dictDate[DateTime.Today.AddDays(-1)].Sum(c => c.Gold) : 0;
                        crowdfundingStatistics.LastTwoDay = dictDate.ContainsKey(DateTime.Today.AddDays(-2)) ? dictDate[DateTime.Today.AddDays(-2)].Sum(c => c.Gold) : oldOne;
                        crowdfundingStatistics.LastThreeDay = dictDate.ContainsKey(DateTime.Today.AddDays(-3)) ? dictDate[DateTime.Today.AddDays(-3)].Sum(c => c.Gold) : oldTwo;
                        crowdfundingStatistics.LastFourDay = dictDate.ContainsKey(DateTime.Today.AddDays(-4)) ? dictDate[DateTime.Today.AddDays(-4)].Sum(c => c.Gold) : oldThree;
                        crowdfundingStatistics.LastFiveDay = dictDate.ContainsKey(DateTime.Today.AddDays(-5)) ? dictDate[DateTime.Today.AddDays(-5)].Sum(c => c.Gold) : oldFour;
                        crowdfundingStatistics.LastSixDay = dictDate.ContainsKey(DateTime.Today.AddDays(-6)) ? dictDate[DateTime.Today.AddDays(-6)].Sum(c => c.Gold) : oldFive;
                        crowdfundingStatistics.LastSevenDay = dictDate.ContainsKey(DateTime.Today.AddDays(-7)) ? dictDate[DateTime.Today.AddDays(-7)].Sum(c => c.Gold) : oldSix;

                        var userDivdidendWeeks =
                            CfDividend.ObjectSet()
                                      .Where(c => c.UserId == crowdfundingStatistics.UserId && c.SettlementDate >= minDate)
                                      .Select(
                                          c =>
                                          new
                                          {
                                              c.Gold,
                                              c.ShareCount,
                                              c.State
                                          })
                                      .ToList();

                        var listCurrentShareCount =
                            UserCrowdfunding.ObjectSet()
                                            .Where(c => c.UserId == crowdfundingStatistics.UserId)
                                            .Select(c => c.CurrentShareCount);
                        if (listCurrentShareCount.Any())
                        {
                            crowdfundingStatistics.CrowdfundingCount = listCurrentShareCount.Sum(c => c);
                        }


                        crowdfundingStatistics.Week = userDivdidendWeeks.Sum(c => c.Gold);
                        crowdfundingStatistics.UnReceiveWeek = userDivdidendWeeks.Where(c => c.State == 0).Sum(c => c.Gold);
                        crowdfundingStatistics.SettlementDate = settlementDate;
                        crowdfundingStatistics.EntityState = EntityState.Modified;

                        contextSession.SaveObject(crowdfundingStatistics);
                    }

                    contextSession.SaveChanges();

                    foreach (var crowdfundingStatistics in cfStatistics)
                    {
                        GlobalCacheWrapper.RemoveCache(UCfCalcPrefix + crowdfundingStatistics.UserId, "BTPCache",
                                                       CacheTypeEnum.redisSS);
                    }

                    if (!isContinue)
                        break;
                }
                #endregion

                #region 昨日新增股东
                start = 0;
                while (true)
                {
                    var userCrowdfundings = (from uc in UserCrowdfunding.ObjectSet()
                                             join cs in CrowdfundingStatistics.ObjectSet() on uc.UserId equals cs.UserId
                                                 into
                                                 coms
                                             from com in coms.DefaultIfEmpty()
                                             where uc.SubTime >= settlementDate && uc.SubTime < nextDate && uc.CurrentShareCount > 0 && com == null
                                             select uc).ToList();

                    if (!userCrowdfundings.Any())
                        break;
                    if (userCrowdfundings.Count < pageSize)
                        isContinue = false;
                    start += pageSize;

                    foreach (var userCrowdfunding in userCrowdfundings)
                    {
                        CrowdfundingStatistics model = CrowdfundingStatistics.CreateCrowdfundingStatistics();
                        model.UserId = userCrowdfunding.UserId;
                        var dict = GlobalCacheWrapper.GetAllData<CfDividendDTO>(UCfCalcPrefix + userCrowdfunding.UserId, CacheTypeEnum.redisSS, "BTPCache");
                        if (dict.Any())
                        {
                            model.Total = dict.Values.Sum(c => c.Gold);
                            model.UnReceive = model.Total;
                            model.Week = model.Total;
                            model.UnReceiveWeek = model.Total;
                            model.LastOneDay = model.Total;

                        }

                        model.SettlementDate = settlementDate;
                        model.CrowdfundingCount = userCrowdfunding.CurrentShareCount;
                        model.EntityState = EntityState.Added;
                        contextSession.SaveObject(model);

                        var dictApp = dict.Values.GroupBy(c => c.AppId, (key, group) => new { AppId = key, CfDividendDTOs = group })
                            .ToDictionary(c => c.AppId, c => c.CfDividendDTOs.ToList());
                    }

                    contextSession.SaveChanges();

                    //删除缓存
                    foreach (var userCrowdfunding in userCrowdfundings)
                    {
                        GlobalCacheWrapper.RemoveCache(UCfCalcPrefix + userCrowdfunding.UserId, "BTPCache",
                                                       CacheTypeEnum.redisSS);
                    }
                    if (!isContinue)
                        break;
                }


                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("众筹汇总统计发生错误：CrowdfundingSV-CalcCfStatistics,被统计日:{0}", settlementDate.ToString("yyyy-MM-dd")), ex);
                return false;
            }
            LogHelper.Info("众筹统计计算完成");
            return true;
        }

        public bool ChangeDateExt(Guid appId, int day)
        {
            Crowdfunding crowdfunding = Crowdfunding.ObjectSet().FirstOrDefault(c => c.AppId == appId);
            if (crowdfunding != null)
            {
                day = -day;
                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                CommodityOrder.ObjectSet().Context.ExecuteStoreCommand(string.Format(@"UPDATE dbo.CommodityOrder SET SubTime= DATEADD(day,{0},SubTime),PaymentTime=DATEADD(day,{0},PaymentTime),ConfirmTime=DATEADD(day,{0},ConfirmTime),ShipmentsTime=DATEADD(day,{0},ShipmentsTime),RefundTime=DATEADD(day,{0},RefundTime) WHERE AppId='{1}'", day, appId));
                CfOrderDividend.ObjectSet().Context.ExecuteStoreCommand(string.Format("UPDATE dbo.CfOrderDividend SET SubTime=DATEADD(DAY,{0},SubTime) WHERE AppId='{1}'", day, appId));
                CfOrderDividendDetail.ObjectSet().Context.ExecuteStoreCommand(string.Format("UPDATE dbo.CfOrderDividendDetail set  SettlementDate=DATEADD(DAY,{0},SettlementDate) WHERE AppId='{1}'", day, appId));
                CfDividend.ObjectSet().Context.ExecuteStoreCommand(string.Format("UPDATE dbo.CfDividend set  SettlementDate=DATEADD(DAY,{0},SettlementDate) WHERE AppId='{1}'", day, appId));

                UserRedEnvelope.ObjectSet().Context.ExecuteStoreCommand(string.Format("UPDATE  UserRedEnvelope SET   SubTime=DATEADD(DAY,{0},SubTime),DueDate=DATEADD(DAY,{0},DueDate)   WHERE AppId='{1}' AND RedEnvelopeType=1 and state=0", day, appId));
                crowdfunding.StartTime = crowdfunding.StartTime.AddDays(day);
                crowdfunding.EntityState = EntityState.Modified;
                contextSession.SaveObject(crowdfunding);
                contextSession.SaveChanges();
            }
            return true;

        }

        public bool DelCfExt(Guid appId)
        {
            Crowdfunding crowdfunding = Crowdfunding.ObjectSet().FirstOrDefault(c => c.AppId == appId);
            LogHelper.Info(string.Format("删除众筹开始，appId：{0}", appId));
            if (crowdfunding != null)
            {
                try
                {
                    UserRedEnvelope.ObjectSet().Context.ExecuteStoreCommand(string.Format("DELETE FROM dbo.UserRedEnvelope WHERE RedEnvelopeType=1 AND AppId='{0}'", appId));
                    CfDividend.ObjectSet().Context.ExecuteStoreCommand(string.Format("DELETE FROM dbo.CfDividend WHERE  AppId='{0}'", appId));
                    CfOrderDividendDetail.ObjectSet().Context.ExecuteStoreCommand(string.Format("DELETE FROM dbo.CfOrderDividendDetail WHERE  AppId='{0}'", appId));
                    CfOrderDividend.ObjectSet().Context.ExecuteStoreCommand(string.Format("DELETE FROM dbo.CfOrderDividend WHERE  AppId='{0}'", appId));
                    CrowdfundingDaily.ObjectSet().Context.ExecuteStoreCommand(string.Format("DELETE FROM dbo.CrowdfundingDaily WHERE  CrowdfundingId='{0}'", crowdfunding.Id));
                    UserCrowdfundingDaily.ObjectSet().Context.ExecuteStoreCommand(string.Format("DELETE FROM dbo.UserCrowdfundingDaily WHERE  AppId='{0}'", appId));
                    UserCrowdfunding.ObjectSet().Context.ExecuteStoreCommand(string.Format("DELETE FROM dbo.UserCrowdfunding WHERE  AppId='{0}'", appId));
                    CrowdfundingCount.ObjectSet().Context.ExecuteStoreCommand(string.Format("DELETE FROM dbo.CrowdfundingCount WHERE  AppId='{0}'", appId));
                    Crowdfunding.ObjectSet().Context.ExecuteStoreCommand(string.Format("DELETE FROM dbo.Crowdfunding WHERE  AppId='{0}'", appId));
                    CommodityOrder.ObjectSet().Context.ExecuteStoreCommand(string.Format("UPDATE dbo.CommodityOrder  SET IsCrowdfunding=0 WHERE AppId='{0}'", appId));
                }
                catch (Exception ex)
                {

                    LogHelper.Error(string.Format("删除众筹错误。appId：{0}", appId), ex);
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 向前修改众筹时间，重新计算股东
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public bool ChangeCfStartTimeEarlierExt(Guid appId)
        {
            try
            {
                LogHelper.Info(string.Format("更改众筹开始时间开始，appId:{0}个", appId));
                Crowdfunding crowdfunding = Crowdfunding.ObjectSet().FirstOrDefault(c => c.AppId == appId);
                if (crowdfunding != null)
                {
                    //众筹当前募得股数
                    long currentShareCount = 0;
                    ContextSession contextSession = ContextFactory.CurrentThreadContext;

                    //获得符合条件的所有订单
                    var orders = CommodityOrder.ObjectSet().Where(c => c.AppId == appId && c.PaymentTime >= crowdfunding.StartTime && c.IsCrowdfunding == 0 && c.Payment != 1 && c.State != 0 && c.State != 11).ToList();
                    foreach (var commodityOrder in orders)
                    {
                        var realPrice = commodityOrder.IsModifiedPrice
                                            ? commodityOrder.RealPrice.Value
                                            : commodityOrder.Price;
                        decimal refundMoney = 0m;
                        //订单中的有效众筹金额
                        decimal cfPrice = realPrice;

                        //退费
                        if (commodityOrder.State == 7)
                        {
                            var refund = OrderRefund.ObjectSet().Where(t => t.OrderId == commodityOrder.Id && t.State == 1).FirstOrDefault();
                            if (refund != null)
                            {
                                refundMoney = refund.RefundMoney;
                                if (realPrice > refund.RefundMoney)
                                {
                                    cfPrice = realPrice - refund.RefundMoney;
                                }
                                else
                                {
                                    cfPrice = 0m;
                                }
                            }
                        }

                        commodityOrder.IsCrowdfunding = 1;
                        commodityOrder.CrowdfundingPrice = cfPrice;
                        commodityOrder.EntityState = EntityState.Modified;
                        LogHelper.Info(string.Format("更改众筹开始时间，处理订单:{0}，订单状态：{1}，有效众筹金额：{2}", commodityOrder.Id, commodityOrder.State, cfPrice));
                        Guid? newUCId = null;
                        if (cfPrice > 0)
                        {
                            UserCrowdfunding userCrowdfunding = UserCrowdfunding.ObjectSet().FirstOrDefault(c => c.UserId == commodityOrder.UserId && c.AppId == appId);
                            if (userCrowdfunding == null)
                            {
                                //添加股东
                                userCrowdfunding = UserCrowdfunding.CreateUserCrowdfunding();
                                userCrowdfunding.UserId = commodityOrder.UserId;
                                userCrowdfunding.CrowdfundingId = crowdfunding.Id;
                                userCrowdfunding.AppId = commodityOrder.AppId;
                                newUCId = userCrowdfunding.Id;
                                try
                                {
                                    List<UserNameAccountsDTO> userNamelist = CBCSV.Instance.GetUserNameAccountsByIds(new List<Guid> { commodityOrder.UserId });

                                    if (userNamelist.Any())
                                    {
                                        var user = userNamelist.First();
                                        userCrowdfunding.UserName = user.userName;
                                        if (user.Accounts != null && user.Accounts.Any())
                                        {
                                            //取手机号，如果手机号为空取 邮箱， 还为空，随便取
                                            var acc = user.Accounts.FirstOrDefault(c => c.AccountType == CBC.Deploy.Enum.AccountSrcEnum.System && !string.IsNullOrEmpty(c.Account) && !c.Account.Contains('@'));
                                            if (acc == null)
                                            {
                                                acc = user.Accounts.FirstOrDefault(c => c.AccountType == CBC.Deploy.Enum.AccountSrcEnum.System && !string.IsNullOrEmpty(c.Account) && c.Account.Contains('@'));
                                            }
                                            else
                                            {
                                                acc = user.Accounts.FirstOrDefault(c => !string.IsNullOrEmpty(c.Account));
                                            }

                                            if (acc != null)
                                                userCrowdfunding.UserCode = acc.Account;
                                            else
                                            {
                                                userCrowdfunding.UserCode = "--";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        userCrowdfunding.UserName = "--";
                                        userCrowdfunding.UserCode = "--";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogHelper.Error(string.Format("CrowdfundingSV-ChangeCfStartTimeEarlier-CBC中的GetUserAccount异常。UserId：{0}", commodityOrder.UserId), ex);
                                    userCrowdfunding.UserName = "--";
                                    userCrowdfunding.UserCode = "--";
                                }
                            }
                            else
                            {
                                //修改股东
                                userCrowdfunding.EntityState = System.Data.EntityState.Modified;
                            }

                            //购买活动总价格
                            decimal afterMoney = userCrowdfunding.Money + cfPrice;
                            //用户购买价格所得的股数
                            long afterShareCnt = (long)(afterMoney / crowdfunding.PerShareMoney);
                            //购买活动总订单数
                            userCrowdfunding.OrderCount += 1;
                            //用户增加的股数
                            var shareCountAdd = afterShareCnt - userCrowdfunding.CurrentShareCount;

                            //股东实际消费金额
                            userCrowdfunding.OrdersMoney += realPrice - refundMoney;
                            userCrowdfunding.Money += cfPrice;
                            userCrowdfunding.CurrentShareCount = (long)(userCrowdfunding.Money / crowdfunding.PerShareMoney);
                            contextSession.SaveObject(userCrowdfunding);


                            currentShareCount = currentShareCount + shareCountAdd;
                        }
                        contextSession.SaveChanges();
                        if (newUCId.HasValue && newUCId != Guid.Empty)
                        {
                            CommodityOrder.ObjectSet().Context.ExecuteStoreCommand(string.Format("UPDATE dbo.UserCrowdfunding  SET SubTime='{0}' WHERE Id='{1}'", commodityOrder.PaymentTime, newUCId.Value));
                        }
                    }
                    LogHelper.Info(string.Format("更改众筹开始时间完成，共处理订单{0}个", orders.Count));
                    if (currentShareCount > 0)
                    {
                        var crowdfundingCount = CrowdfundingCount.ObjectSet().FirstOrDefault(c => c.AppId == appId);
                        crowdfundingCount.CurrentShareCount += currentShareCount;
                        crowdfundingCount.EntityState = EntityState.Modified;
                        contextSession.SaveChanges();
                    }
                    CalcUserCrowdfundingDailyExt(DateTime.Today.AddDays(-1));
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CrowdfundingSV-ChangeCfStartTimeEarlier ,更改众筹开始时间异常。appId：{0}", appId), ex);
                return false;
            }

            return true;

        }
        public List<UserOrderCarDTO> GetUserCrowdfundingBuyerExt(List<Guid> appIds, Guid userId)
        {
            List<UserOrderCarDTO> uocList = new List<UserOrderCarDTO>();
            try
            {

                //查询众筹状态为进行中的应用
                var cfdList = from e in Crowdfunding.ObjectSet()
                              where e.State == 0 && appIds.Contains(e.AppId) && e.StartTime < DateTime.Now
                              select e;
                if (cfdList == null || cfdList.Count() == 0)
                {
                    return uocList;
                }




                var cfdIds = from cf in cfdList select cf.Id;
                //用户是否是股东
                var ucdList = from e in UserCrowdfunding.ObjectSet()
                              where cfdIds.Contains(e.CrowdfundingId) && e.UserId == userId
                              select new { e.CrowdfundingId, e.Money, e.CurrentShareCount };
                var ccList = from e in CrowdfundingCount.ObjectSet()
                             where cfdIds.Contains(e.CrowdfundingId)
                             select new { e.CrowdfundingId, e.CurrentShareCount };

                foreach (var cfd in cfdList)
                {
                    var userCrowd = ucdList == null ? null : ucdList.Where(ucd => ucd.CrowdfundingId == cfd.Id).FirstOrDefault();
                    var cfc = ccList == null ? null : ccList.Where(cc => cc.CrowdfundingId == cfd.Id).FirstOrDefault();

                    UserOrderCarDTO uoc = new UserOrderCarDTO();
                    uoc.PerShareMoney = cfd.PerShareMoney;
                    uoc.AppId = cfd.AppId;
                    uoc.IsActiveCrowdfunding = true;

                    if (userCrowd != null)
                    {
                        uoc.Money = userCrowd.Money;
                        uoc.CurrentShareCount = userCrowd.CurrentShareCount;
                    }
                    if (cfc != null)
                    {
                        uoc.ShareCountRemain = cfd.ShareCount - cfc.CurrentShareCount;
                    }
                    uocList.Add(uoc);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("方法GetUserCrowdfundingBuyerExt异常。appIds：{0}。userId：{1}", appIds, userId), ex);
                return null;
            }

            return uocList;
        }
    }
}
