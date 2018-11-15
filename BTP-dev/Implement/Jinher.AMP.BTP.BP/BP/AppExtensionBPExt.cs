
/***************
功能描述: BTP-setBP
作    者: 
创建时间: 2015/11/17 14:36:05
***************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO.DaMiWang;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.TPS;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class AppExtensionBP : BaseBP, IAppExtension
    {

        /// <summary>
        /// 更新应用扩展（店铺扩展）
        /// </summary>
        /// <param name="appExtDTO">应用扩展信息实体</param>
        /// <returns>操作结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateAppExtensionExt(Jinher.AMP.BTP.Deploy.AppExtensionDTO appExtDTO)
        {
            ResultDTO result = new ResultDTO();
            string sParamJson = "";
            try
            {
                if (appExtDTO == null)
                {
                    result.Message = "参数不能为空！";
                    result.ResultCode = 1;
                }
                if (appExtDTO.Id == Guid.Empty)
                {
                    result.Message = "参数错误，appId不能为空！";
                    result.ResultCode = 2;
                }
                sParamJson = JsonHelper.JsonSerializer<Jinher.AMP.BTP.Deploy.AppExtensionDTO>(appExtDTO);


                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                AppExtension appExt = (from ae in AppExtension.ObjectSet()
                                       where ae.Id == appExtDTO.Id
                                       select ae).FirstOrDefault();
                if (appExt == null)
                {
                    appExt = new AppExtension();
                    appExt.SubTime = DateTime.Now;
                    appExt.EntityState = System.Data.EntityState.Added;
                    contextSession.SaveObject(appExt);
                }
                appExt.Id = appExtDTO.Id;
                appExt.IsShowAddCart = appExtDTO.IsShowAddCart;
                appExt.ModifiedOn = DateTime.Now;

                contextSession.SaveChanges();

                return result;
            }
            catch (Exception ex)
            {
                string str = string.Format("AppExtensionBP.UpdateAppExtensionExt中发生异常，参数：{0},异常信息：{1}", sParamJson, ex);
                LogHelper.Error(str);
                return result;
            }
        }


        /// <summary>
        /// 获取特定app在电商中的扩展信息。
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.AppExtensionDTO> GetAppExtensionByAppIdExt(System.Guid appId)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.AppExtensionDTO> resultAppExt = new ResultDTO<Jinher.AMP.BTP.Deploy.AppExtensionDTO>();

            try
            {
                if (appId == Guid.Empty)
                {
                    resultAppExt.Message = "参数错误，appId不能为空！";
                    resultAppExt.ResultCode = 1;
                }
                var aeList = (from ae in AppExtension.ObjectSet()
                              where ae.Id == appId
                              select ae).ToList();
                if (aeList.Any())
                {
                    var aeFirst = aeList.FirstOrDefault();
                    Jinher.AMP.BTP.Deploy.AppExtensionDTO aeDto = aeFirst.ToEntityData();
                    resultAppExt.Data = aeDto;
                }
            }
            catch (Exception ex)
            {
                string str = string.Format("AppExtensionBP.GetAppExtensionByAppIdExt中发生异常，参数AppId：{0},异常信息：{1}", appId, ex);
                LogHelper.Error(str);

                resultAppExt.Message = "服务异常！";
                resultAppExt.ResultCode = 2;
            }
            return resultAppExt;
        }
        /// <summary>
        /// 获取app的积分抵现设置
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public AppScoreSettingDTO GetScoreSettingExt(Guid appId)
        {
            AppScoreSettingDTO result = new AppScoreSettingDTO();
            if (appId == Guid.Empty)
                return result;
            var appExtension = AppExtension.ObjectSet().FirstOrDefault(c => c.Id == appId);
            if (appExtension != null && appExtension.IsCashForScore)
                result.IsCashForScore = true;
            var scoreSetting = ScoreSetting.ObjectSet().Where(c => c.AppId == appId).OrderByDescending(c => c.SubTime).FirstOrDefault();
            if (scoreSetting != null && scoreSetting.ScoreCost > 0)
                result.ScoreCost = scoreSetting.ScoreCost.Value;
            return result;
        }

        /// <summary>
        /// 设置渠道佣金比例
        /// </summary>
        /// <param name="appExtension">佣金比例</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetDefaultChannelAccountExt(Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO appExtension)
        {
            ResultDTO result = new ResultDTO() { ResultCode = 1, Message = "参数不能为空" };
            if (appExtension == null || appExtension.Id == Guid.Empty)
            {
                return result;
            }
            var model = AppExtension.ObjectSet().Where(t => t.Id == appExtension.Id).FirstOrDefault();
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            if (model != null)
            {
                model.ChannelSharePercent = appExtension.ChannelDistributePercent;
                model.ModifiedOn = DateTime.Now;
                model.EntityState = System.Data.EntityState.Modified;
                contextSession.SaveChanges();
            }
            else
            {
                model = new AppExtension();
                model.Id = appExtension.Id;
                model.AppName = appExtension.AppName;
                model.SubTime = DateTime.Now;
                model.ModifiedOn = DateTime.Now;
                model.IsShowSearchMenu = false;
                model.IsShowAddCart = false;
                model.IsDividendAll = null;
                model.SharePercent = 0;
                model.DistributeL1Percent = appExtension.DistributeL1Percent;
                model.DistributeL2Percent = appExtension.DistributeL2Percent;
                model.DistributeL3Percent = appExtension.DistributeL3Percent;
                model.ChannelSharePercent = appExtension.ChannelDistributePercent;
                model.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(model);
            }

            contextSession.SaveChanges();
            result.ResultCode = 0;
            result.Message = "Success";
            return result;
        }
        /// <summary>
        /// 获取渠道默认佣金比例
        /// </summary>
        /// <param name="appId">AppId</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO GetDefaulChannelAccountExt(Guid appId)
        {
            if (appId == Guid.Empty)
            {
                return null;
            }
            var model = AppExtension.ObjectSet().Where(c => c.Id == appId).FirstOrDefault();
            if (model == null)
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var appName = APPSV.GetAppName(appId);
                model = new AppExtension();
                model.Id = appId;
                model.AppName = appName;
                model.SubTime = DateTime.Now;
                model.ModifiedOn = DateTime.Now;
                model.IsShowSearchMenu = false;
                model.IsShowAddCart = false;
                model.IsDividendAll = null;
                model.SharePercent = 0;
                model.DistributeL1Percent = null;
                model.DistributeL2Percent = null;
                model.DistributeL3Percent = null;
                model.IsCashForScore = false;
                model.ChannelSharePercent = null;
                model.EntityState=EntityState.Added;
                contextSession.SaveObject(model);
                contextSession.SaveChanges();
            }
            Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO result = new Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO()
            {
                Id = model.Id,
                AppName = model.AppName,
                SubTime = model.SubTime,
                ModifiedOn = model.ModifiedOn,
                IsShowSearchMenu = model.IsShowSearchMenu,
                IsShowAddCart = model.IsShowAddCart,
                IsDividendAll = model.IsDividendAll,
                SharePercent = model.SharePercent,
                DistributeL1Percent = model.DistributeL1Percent,
                DistributeL2Percent = model.DistributeL2Percent,
                DistributeL3Percent = model.DistributeL3Percent,
                IsCashForScore = model.IsCashForScore,
                ChannelDistributePercent = model.ChannelSharePercent
            };
            return result;
            
        }

        /// <summary>
        /// 获取应用统计信息
        /// </summary>
        /// <param name="appId">AppId</param>
        /// <returns>结果</returns>
        public AppStatisticsDTO GetAppStatisticsExt(System.Guid appId)
        {
            AppStatisticsDTO model = new AppStatisticsDTO();
            try
            {
                var order = CommodityOrder.ObjectSet().Where(p => p.EsAppId == appId && !new[] {0, 4, 5, 6, 11, 17, 19, 21}.Contains(p.State)).Select(p => new
                {
                    RealPrice = p.RealPrice ?? 0,
                }).ToList();
                var orderCount = order.Count();
                var totalMoney = (int) order.Sum(p => p.RealPrice)*100;
                var query = new ZPH.Deploy.CustomDTO.QueryPavilionAppParam
                {
                    Id = appId,
                    pageIndex = 1,
                    pageSize = int.MaxValue
                };
                var appIdList = ZPHSV.Instance.GetPavilionApp(query).Data.Select(t => t.appId).ToList();
                var commodityCount = Commodity.ObjectSet().Count(p => appIdList.Contains(p.AppId));

                model.msg_salesvolume = totalMoney.ToString();
                model.msg_ordernumber = orderCount.ToString();
                model.msg_productquantity = commodityCount.ToString();
            }
            catch (Exception ex)
            {
                LogHelper.Error("获取应用统计信息AppExtensionBP.GetAppStatisticsExt异常," + appId, ex);
                model = null;
            }
            return model;
        }
    }
}
