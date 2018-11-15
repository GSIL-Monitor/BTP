
/***************
功能描述: BTPSV
作    者: 
创建时间: 2016/2/17 10:08:56
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.AMP.BTP.TPS;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Common;
using CommodityStockDTO = Jinher.AMP.BTP.Deploy.CommodityStockDTO;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    public partial class ScoreSettingSV : BaseSv, IScoreSetting
    {

        /// <summary>
        /// 获取特定app在电商中的当前生效的扩展信息。
        /// </summary>
        /// <param name="appId">应用id</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.ScoreSettingDTO> GetScoreSettingByAppIdExt(System.Guid appId)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.ScoreSettingDTO> resultAppExt = new ResultDTO<ScoreSettingDTO>();

            try
            {
                if (appId == Guid.Empty)
                {
                    resultAppExt.Message = "参数错误，appId不能为空！";
                    resultAppExt.ResultCode = 1;
                }
                var aeList = (from ae in ScoreSetting.ObjectSet()
                              where ae.AppId == appId
                              orderby ae.SubTime descending
                              select ae).ToList();
                if (aeList.Any())
                {
                    var aeFirst = aeList.FirstOrDefault();
                    ScoreSettingDTO aeDto = aeFirst.ToEntityData();
                    resultAppExt.Data = aeDto;
                }
            }
            catch (Exception ex)
            {
                string str = string.Format("ScoreSettingSV.GetScoreSettingByAppIdExt中发生异常，参数AppId：{0},异常信息：{1}", appId, ex);
                LogHelper.Error(str);

                resultAppExt.Message = "服务异常！";
                resultAppExt.ResultCode = 2;
            }
            return resultAppExt;
        }

        /// <summary>
        /// 获取特定app在电商中的当前生效的积分扩展信息。
        /// </summary> 
        /// <param name="paramDto">查询参数</param>
        /// <returns></returns>
        public ResultDTO<UserScoreDTO> GetUserScoreInAppExt(Param2DTO paramDto)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.UserScoreDTO> result = new ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.UserScoreDTO>();
            UserScoreDTO usDto = new UserScoreDTO();
            result.Data = usDto;

            try
            {
                if (paramDto == null)
                {
                    result.Message = "参数错误，参数不能为空！";
                    result.ResultCode = 1;
                    return result;
                }
                if (paramDto.UserId == Guid.Empty)
                {
                    result.Message = "参数错误，appId不能为空！";
                    result.ResultCode = 1;
                    return result;
                }
                if (paramDto.AppId == Guid.Empty)
                {
                    result.Message = "参数错误，appId不能为空！";
                    result.ResultCode = 1;
                    return result;
                }
                var appIsCashForScore = (from ae in AppExtension.ObjectSet()
                                         where ae.Id == paramDto.AppId
                                         select ae.IsCashForScore).FirstOrDefault();
                if (appIsCashForScore == null || appIsCashForScore == false)
                {
                    usDto.IsCashForScore = false;
                    return result;
                }
                usDto.IsCashForScore = appIsCashForScore;

                var ssFirst = (from ss in ScoreSetting.ObjectSet()
                               where ss.AppId == paramDto.AppId
                               orderby ss.SubTime descending
                               select ss.ScoreCost).FirstOrDefault();
                if (!ssFirst.HasValue)
                {
                    return result;
                }
                usDto.ScoreCost = ssFirst.Value;
                int score = Jinher.AMP.BTP.TPS.SignSV.Instance.GiveUserScore(paramDto.UserId, paramDto.AppId);
                usDto.Score = score;
                usDto.Money = DecimalExt.ToMoney((decimal)score / usDto.ScoreCost);
            }
            catch (Exception ex)
            {
                string str = string.Format("ScoreSettingSV.GetUserScoreInAppExt中发生异常，参数AppId：{0},异常信息：{1}", paramDto.AppId, ex);
                LogHelper.Error(str);

                result.Message = "服务异常！";
                result.ResultCode = 2;
            }
            return result;
        }

        public ResultDTO<OrderScoreCheckResultDTO> OrderScoreCheckExt(OrderScoreCheckDTO paramDto)
        {
            ResultDTO<OrderScoreCheckResultDTO> result = new ResultDTO<OrderScoreCheckResultDTO>() { Message = "Success", Data = new OrderScoreCheckResultDTO() };
            //OrderScoreCheckResultDTO resultData = new OrderScoreCheckResultDTO();
            //resultData.IsCashForScore = true;
            //resultData.Score = 120000;
            //resultData.ScoreCost = 100;
            //resultData.ScoreType = ScoreTypeEnum.Self;
            //var price = (paramDto.Coms[0].RealPrice * paramDto.Coms[0].Num / 10.0m).ToMoney();
            //resultData.Money = price;
            //resultData.List = new List<AppScoreDTO>(){new AppScoreDTO()
            //    {
            //        AppId = paramDto.EsAppId,
            //        Money = price
            //    }};
            //result.Data = resultData;
            //return result;
            DateTime now = DateTime.Now;
            if (paramDto == null || paramDto.EsAppId == Guid.Empty || paramDto.Coms == null || !paramDto.Coms.Any())
                return new ResultDTO<OrderScoreCheckResultDTO>() { ResultCode = 1, Message = "参数有误" };
            var comIds = paramDto.Coms.Select(c => c.CommodityId).Distinct().ToList();
            var coms = Commodity.ObjectSet().Where(c => comIds.Contains(c.Id)).Select(m => new CommodityDTO { Id = m.Id, Price = m.Price, AppId = m.AppId, ScorePercent = m.ScorePercent }).ToList();
            var appIds = coms.Select(c => c.AppId).ToList();
            appIds.Add(paramDto.EsAppId);
            appIds = appIds.Distinct().ToList();

            var appExts = AppExtension.ObjectSet()
                            .Where(c => appIds.Contains(c.Id) && c.IsCashForScore && c.IsScoreAll != null)
                            .Select(c => new Deploy.AppExtensionDTO() { Id = c.Id, IsCashForScore = c.IsCashForScore, IsScoreAll = c.IsScoreAll, ScorePercent = c.ScorePercent })
                            .ToList();
            Dictionary<Guid, Deploy.AppExtensionDTO> appExtDict = appExts.ToDictionary(x => x.Id, y => y);


            //获取支持积分抵现所有app,（不支持积分抵现、未设置积分抵现比例均视为不支持积分抵现）
            var cashForScoreAppIds = appExts.Select(c => c.Id).ToList();

            //如果所有的app都没有启用积分抵现，直接返回
            if (!cashForScoreAppIds.Any())
                return result;

            List<Deploy.CommodityStockDTO> comStocks = getComStocks(paramDto);

            #region 获取可参加积分抵现商品
            List<ComScoreCheckReDTO> canScoreComList = new List<ComScoreCheckReDTO>();
            foreach (var requestCom in paramDto.Coms)
            {

                var com = coms.First(c => c.Id == requestCom.CommodityId);
                if (cashForScoreAppIds.All(c => c != com.AppId))
                    continue;
                Deploy.CommodityStockDTO comStock = null;
                if (requestCom.CommodityStockId != null && requestCom.CommodityStockId != Guid.Empty)
                {
                    comStock = comStocks.FirstOrDefault(c => c.Id == requestCom.CommodityStockId);
                }
                if (getOriPrice(com, comStock) == requestCom.RealPrice)
                {
                    var scorePercent = appExtDict[com.AppId].IsScoreAll == true ? (decimal?)appExtDict[com.AppId].ScorePercent : com.ScorePercent;
                    if (!scorePercent.HasValue || scorePercent <= 0)
                        continue;
                    var item = ComScoreCheckReDTO.FromRequest(requestCom, com.AppId);
                    item.CanScoreMoney = (requestCom.RealPrice * requestCom.Num * scorePercent.Value).ToMoney();
                    if (item.CanScoreMoney <= 0)
                        continue;
                    canScoreComList.Add(item);
                }
            }
            //只有不参加活动的商品才可以使用积分抵现
            if (!canScoreComList.Any())
                return result;
            #endregion

            var checkResult = SignSV.Instance.QueryAllScores(appIds, paramDto.UserId);
            if (checkResult == null || checkResult.ScoreDetailList == null || !checkResult.ScoreDetailList.Any())
                return new ResultDTO<OrderScoreCheckResultDTO>() { ResultCode = 1, Message = "积分校验失败" };

            //校验是否在本应用下单（非通用积分商城中下单不允许使用）
            bool isInSelfApp = coms.All(c => c.AppId == paramDto.EsAppId);

            int cost;  //积分汇率
            int scoreCount;  //积分数量

            //是否都是通用积分
            bool isAllCurrency = checkResult.ScoreDetailList.All(returnUnitiveScoreDTO => returnUnitiveScoreDTO.IsUnitive);
            if (isAllCurrency)
            {
                //汇率无效
                if (checkResult.ExchangeRate <= 0)
                    return result;
                cost = checkResult.ExchangeRate;
                scoreCount = checkResult.UnitiveScore;
            }
            //非通用积分只有在本app下才可以使用
            else if (isInSelfApp)
            {
                var scoreSetting = (from ss in ScoreSetting.ObjectSet()
                                    where ss.AppId == paramDto.EsAppId && ss.SubTime <= now
                                    orderby ss.SubTime descending
                                    select new { ss.ScoreCost }).FirstOrDefault();
                if (scoreSetting == null)
                    return result;

                cost = scoreSetting.ScoreCost.Value;
                var appInfo = checkResult.ScoreDetailList.FirstOrDefault(c => c.AppId == paramDto.EsAppId);
                if (appInfo == null)
                {
                    return result;
                }
                scoreCount = appInfo.AvailableScore;
            }
            else
            {
                return result;
            }
            result.Data.IsCashForScore = true;
            result.Data.Score = scoreCount;
            result.Data.ScoreType = isAllCurrency ? ScoreTypeEnum.Currency : ScoreTypeEnum.Self;
            result.Data.ScoreCost = cost;

            //用户积分折现金额，
            decimal userScoreMoney = (scoreCount * 1.0m / cost).ToMoney();

            //总可参与积分抵现的总金额
            decimal canScoreMoney = canScoreComList.Sum(c => c.CanScoreMoney);

            if (userScoreMoney > canScoreMoney)
                userScoreMoney = canScoreMoney;


            var dict = canScoreComList.GroupBy(c => c.AppId).ToDictionary(x => x.Key, y => y.ToList());
            foreach (var kv in dict)
            {
                var appExt = appExts.FirstOrDefault(c => c.Id == kv.Key);
                if (appExt == null)
                    continue;
                AppScoreDTO appScoreDTO = new AppScoreDTO() { AppId = kv.Key };
                foreach (var com in kv.Value)
                {
                    com.Money = getScoreMoney(userScoreMoney * com.CanScoreMoney / canScoreMoney, cost);
                    if (com.Money <= 0)
                        continue;
                    appScoreDTO.Money += com.Money;
                    appScoreDTO.Coms.Add(com);
                }
                if (appScoreDTO.Money <= 0)
                    continue;
                result.Data.List.Add(appScoreDTO);
                result.Data.Money += appScoreDTO.Money;
            }
            #region 订单减免->订单减免后余额小于积分抵现金额

            decimal money = 0.0m;
            foreach (var appScoreDTO in result.Data.List)
            {

                var reducation = paramDto.Reductions.FirstOrDefault(c => c.AppId == appScoreDTO.AppId);
                if (reducation == null || reducation.Reduction <= 0) //不存在订单减免，不需要处理
                {
                    money += appScoreDTO.Money;
                    continue;
                }


                var appComIds = coms.Where(c => c.AppId == appScoreDTO.AppId).Select(c => c.Id).ToList();
                var appTotalPrice = paramDto.Coms.Where(c => appComIds.Contains(c.CommodityId)).Sum(c => c.RealPrice * c.Num);
                var appPrice = appTotalPrice - reducation.Reduction;

                if (appPrice > appScoreDTO.Money)  //订单订单减免后金额足够支付，不需要处理
                {
                    money += appScoreDTO.Money;
                    continue;
                }

                var percent = appPrice / appTotalPrice;
                appScoreDTO.Money = 0;
                for (int i = 0; i < appScoreDTO.Coms.Count - 1; i++)
                {
                    appScoreDTO.Coms[i].Money = (appScoreDTO.Coms[i].Money * percent).ToMoney();
                    appScoreDTO.Money += appScoreDTO.Coms[i].Money;
                }
                //处理由于四舍五入造成除不尽的问题，剩余数据都给最后一条订单项
                appScoreDTO.Coms[appScoreDTO.Coms.Count - 1].Money = appPrice - appScoreDTO.Money;
                appScoreDTO.Money += appScoreDTO.Coms[appScoreDTO.Coms.Count - 1].Money;
                money += appScoreDTO.Money;
            }
            result.Data.Money = money;
            #endregion


            return result;
        }
        private static decimal getScoreMoney(decimal money, int cost)
        {
            if (cost < 100)
            {
                money = (decimal)((int)(money * cost)) / cost;

            }
            return money.ToMoney();
        }
        /// <summary>
        /// 获得商品原价
        /// </summary>
        /// <param name="com"></param>
        /// <param name="stock"></param>
        /// <returns></returns>
        private decimal getOriPrice(CommodityDTO com, Deploy.CommodityStockDTO stock)
        {
            decimal result = 0.0m;
            if (stock != null)
            {
                result = stock.Price;
            }
            else
            {
                result = com.Price;
            }
            return result;

        }
        /// <summary>
        /// 获取多属性商品
        /// </summary>
        /// <param name="coms"></param>
        /// <returns></returns>
        private List<Guid> getMultComIds(List<ComScoreCheckDTO> coms)
        {
            Dictionary<Guid, List<Tuple<string, string>>> dict = new Dictionary<Guid, List<Tuple<string, string>>>();
            List<Guid> result = new List<Guid>();
            if (coms != null && coms.Any())
            {
                foreach (var comScoreCheckDTO in coms)
                {
                    if (string.IsNullOrEmpty(comScoreCheckDTO.ColorAndSize))
                        continue;
                    comScoreCheckDTO.ColorAndSize = comScoreCheckDTO.ColorAndSize.Replace("null", "").Replace("nil", "").Replace("undefined", "").Replace("(null)", "").Replace("，", ",");
                    var arr = comScoreCheckDTO.ColorAndSize.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    if (arr.Length == 2)
                        result.Add(comScoreCheckDTO.CommodityId);
                }
            }
            return result;
        }
        private List<CommodityStockDTO> getComStocks(OrderScoreCheckDTO paramDto)
        {
            List<CommodityStockDTO> result = new List<CommodityStockDTO>();
            Dictionary<Guid, Tuple<string, string>> dict = new Dictionary<Guid, Tuple<string, string>>();
            List<Guid> comIds = new List<Guid>();
            if (paramDto == null || paramDto.Coms == null || !paramDto.Coms.Any())
                return result;

            foreach (var comScoreCheckDTO in paramDto.Coms)
            {
                if (string.IsNullOrEmpty(comScoreCheckDTO.ColorAndSize))
                    continue;
                comScoreCheckDTO.ColorAndSize = comScoreCheckDTO.ColorAndSize.Replace("null", "").Replace("nil", "").Replace("undefined", "").Replace("(null)", "").Replace("，", ",");
                var arr = comScoreCheckDTO.ColorAndSize.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Length == 2)
                {
                    comIds.Add(comScoreCheckDTO.CommodityId);
                    if (!dict.ContainsKey(comScoreCheckDTO.ItemId))
                        dict.Add(comScoreCheckDTO.ItemId, new Tuple<string, string>(arr[0], arr[1]));
                }
            }
            comIds = comIds.Distinct().ToList();
            List<CommodityStockDTO> temp = CommodityStock.ObjectSet().Where(c => comIds.Contains(c.CommodityId)).Select(m => new CommodityStockDTO
                  {
                      Id = m.Id,
                      Price = m.Price,
                      Stock = m.Stock,
                      CommodityId = m.CommodityId,
                      ComAttribute = m.ComAttribute
                  }).ToList();
            foreach (var comScoreCheckDTO in paramDto.Coms)
            {
                if (comIds.All(c => c != comScoreCheckDTO.CommodityId))
                {
                    comScoreCheckDTO.CommodityStockId = Guid.Empty;
                    continue;
                }
                var comStockDto = getComStock(temp, comScoreCheckDTO.CommodityId, dict[comScoreCheckDTO.ItemId].Item1, dict[comScoreCheckDTO.ItemId].Item2);
                if (comStockDto != null)
                {
                    comScoreCheckDTO.CommodityStockId = comStockDto.Id;
                    result.Add(comStockDto);
                }
            }
            return result;

        }
        private CommodityStockDTO getComStock(List<CommodityStockDTO> stocks, Guid comId, string attr1, string attr2)
        {
            CommodityStockDTO result = null;
            foreach (var commodityStockDTO in stocks.Where(c => c.CommodityId == comId))
            {
                var comAttrs = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(commodityStockDTO.ComAttribute);
                if (comAttrs != null && comAttrs.Count == 2 && comAttrs.Any(c => c.SecondAttribute == attr1) && comAttrs.Any(c => c.SecondAttribute == attr2))
                {
                    result = commodityStockDTO;
                }
            }
            return result;
        }
    }
}