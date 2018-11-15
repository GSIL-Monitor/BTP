using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.EBC.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.EBC.ISV.Facade;

namespace Jinher.AMP.BTP.TPS
{
    /// <summary>
    /// 积分相关
    /// </summary>
    [BTPAopLog]
    public class ScoreSV
    {
        /// <summary>
        /// 获取当前积分汇率
        /// </summary>
        /// <param name="appId">应用id</param>
        /// <param name="cost">积分汇率</param>
        /// <returns></returns>
        public bool GetScoreCost(Guid appId, out CostScoreInfoDTO cost)
        {
            return GetScoreCost(appId, out cost, DateTime.Now);
        }

        /// <summary>
        /// 获取某时间点积分汇率
        /// </summary>
        /// <param name="appId">应用id</param>
        /// <param name="resultDto"></param>
        /// <param name="time">时间点</param>
        /// <returns></returns>
        public bool GetScoreCost(Guid appId, out CostScoreInfoDTO resultDto, DateTime time)
        {
            resultDto = new CostScoreInfoDTO();
            var signResult = SignSV.Instance.GetCurrencyScoreCost(appId, time);
            if (signResult == null)
                return false;
            if (signResult.IsUnitiveScore)
            {
                resultDto.Cost = signResult.ExchangeRate;
                resultDto.ScoreType = ScoreTypeEnum.Currency;
            }
            else
            {
                var scoreSetting = (from ss in ScoreSetting.ObjectSet()
                                    where ss.AppId == appId && ss.SubTime <= time
                                    orderby ss.SubTime descending
                                    select new { Id = ss.Id, ScoreCost = ss.ScoreCost }).FirstOrDefault();
                if (scoreSetting == null || scoreSetting.ScoreCost == null)
                    return false;
                resultDto.Cost = scoreSetting.ScoreCost.Value;
                resultDto.ScoreType = ScoreTypeEnum.Self;
            }

            return true;
        }

        /// <summary>
        /// 获取某时间点积分汇率
        /// </summary>
        /// <param name="appId">应用id</param>
        /// <param name="cost"></param>
        /// <param name="time">时间点</param>
        /// <returns>校验是否成功</returns>
        public bool CheckOrderScoreCost(Guid appId, out int cost, DateTime time)
        {
            cost = 0;
            var signResult = SignSV.Instance.GetCurrencyScoreCost(appId, time);
            if (signResult == null)
                return false;
            if (signResult.IsUnitiveScore)
            {
                cost = signResult.ExchangeRate;
            }
            else
            {
                var scoreSetting = (from ss in ScoreSetting.ObjectSet()
                                    where ss.AppId == appId && ss.SubTime <= time
                                    orderby ss.SubTime descending
                                    select new { Id = ss.Id, ScoreCost = ss.ScoreCost }).FirstOrDefault();
                if (scoreSetting == null || scoreSetting.ScoreCost == null)
                    return true;
                cost = scoreSetting.ScoreCost.Value;
            }

            return true;
        }
        /// <summary>
        /// 获取某时间点积分汇率
        /// </summary>
        /// <param name="appId">应用id</param>
        /// <param name="cost">积分汇率</param>
        /// <param name="isCurrency">是否通用积分</param>
        /// <returns></returns>
        public bool GetScoreCost(Guid appId, out int cost, bool isCurrency)
        {
            cost = 0;

            if (isCurrency)
            {
                var signResult = SignSV.Instance.GetCurrencyScoreCost(appId, DateTime.Now);
                if (signResult == null || !signResult.IsUnitiveScore)
                    return false;
                cost = signResult.ExchangeRate;
            }
            else
            {
                var scoreSetting = (from ss in ScoreSetting.ObjectSet()
                                    where ss.AppId == appId
                                    orderby ss.SubTime descending
                                    select new { Id = ss.Id, ScoreCost = ss.ScoreCost }).FirstOrDefault();
                if (scoreSetting == null || scoreSetting.ScoreCost == null)
                    return false;
                cost = scoreSetting.ScoreCost.Value;
            }
            return true;
        }


    }

}
