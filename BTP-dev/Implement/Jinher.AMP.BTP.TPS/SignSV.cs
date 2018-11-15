using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.CBC.Deploy.CustomDTO;
using Jinher.AMP.Sign.Deploy;
using Jinher.AMP.Sign.Deploy.CustomDTO;
using Jinher.AMP.Sign.Deploy.Enum;
using Jinher.AMP.Sign.IBP.Facade;
using Jinher.JAP.Cache;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;

namespace Jinher.AMP.BTP.TPS
{
    /*
     * 请注意：！！！！！！！！！！！！！！！
     * 外部引用请将代码写到TPS.XXXFacade中，自定义扩展请写到TPS.XXXSV、TPS.XXXBP中
     */

    public class SignSV : OutSideServiceBase<SignSVFacade>
    {

        /// <summary>
        /// 取消订单时回退积分
        /// </summary>
        /// <param name="contextSession"></param>
        /// <param name="commodityOrder"></param>
        /// <returns></returns>
        public static bool CommodityOrderCancelSrore(ContextSession contextSession, CommodityOrder commodityOrder)
        {
            if (!commodityOrder.EsAppId.HasValue)
            {
                return true;
            }
            //获取下订单时用的积分金额
            var sroreMoney = OrderPayDetail.ObjectSet().Where(t => t.OrderId == commodityOrder.Id && t.ObjectType == 2 && t.Amount > 0).Select(t => t.Amount).FirstOrDefault();
            if (sroreMoney <= 0)
            {
                return true;
            }
            //获取当时的积分汇率
            CostScoreInfoDTO scoreCostDto;
            bool isscsc = new ScoreSV().GetScoreCost(commodityOrder.EsAppId.Value, out scoreCostDto, commodityOrder.SubTime);
            if (!isscsc)
            {
                return true;
            }
            int srore = (int)(sroreMoney * scoreCostDto.Cost);
            bool refundScoreResult = Instance.RefundScore(commodityOrder.UserId, commodityOrder.EsAppId.Value, srore, commodityOrder.Id, commodityOrder.Code, scoreCostDto.ScoreType);
            if (refundScoreResult)
            {
                return refundScoreResult;
            }

            LogHelper.Error(string.Format("取消订单时回退积分失败。UserId：{0}，EsAppId：{1}，srore：{2}", commodityOrder.UserId, commodityOrder.EsAppId.Value, srore));
            ErrorCommodityOrder errorOrder = new ErrorCommodityOrder();
            errorOrder.Id = Guid.NewGuid();
            errorOrder.ErrorOrderId = commodityOrder.Id;
            errorOrder.ResourceType = 1;
            errorOrder.Source = commodityOrder.State;
            errorOrder.State = 0;
            errorOrder.AppId = commodityOrder.EsAppId.Value;
            errorOrder.UserId = commodityOrder.UserId;
            errorOrder.OrderCode = commodityOrder.Code;
            errorOrder.CouponId = Guid.Empty;
            errorOrder.Score = srore;
            errorOrder.SubTime = DateTime.Now;
            errorOrder.ModifiedOn = DateTime.Now;
            errorOrder.ScoreType = (int)scoreCostDto.ScoreType;
            errorOrder.EntityState = System.Data.EntityState.Added;
            contextSession.SaveObject(errorOrder);
            return refundScoreResult;
        }


        /// <summary>
        /// 售中退款时回退积分
        /// </summary>
        /// <param name="contextSession"></param>
        /// <param name="commodityOrder"></param>
        /// <param name="orderRefund"></param>
        /// <returns></returns>
        public static bool CommodityOrderRefundScore(ContextSession contextSession, CommodityOrder commodityOrder, OrderRefund orderRefund)
        {
            if (!commodityOrder.EsAppId.HasValue)
            {
                return true;
            }
            //使用了积分
            if (orderRefund.RefundScoreMoney > 0)
            {
                //获取当时的积分比例
                CostScoreInfoDTO scoreCostDto;
                if (new ScoreSV().GetScoreCost(commodityOrder.EsAppId.Value, out scoreCostDto, commodityOrder.SubTime))
                {
                    int srore = (int)(orderRefund.RefundScoreMoney * scoreCostDto.Cost);
                    bool refundScoreResult = SignSV.Instance.RefundScore(commodityOrder.UserId, commodityOrder.EsAppId.Value, srore, commodityOrder.Id, commodityOrder.Code, scoreCostDto.ScoreType);
                    if (!refundScoreResult)
                    {
                        LogHelper.Error(string.Format("售中退款时回退积分失败。UserId：{0}，EsAppId：{1}，srore：{2}", commodityOrder.UserId, commodityOrder.EsAppId.Value, srore));
                        ErrorCommodityOrder errorOrder = new ErrorCommodityOrder();
                        errorOrder.Id = Guid.NewGuid();
                        errorOrder.ErrorOrderId = commodityOrder.Id;
                        errorOrder.ResourceType = 1;
                        errorOrder.Source = 7;
                        errorOrder.State = 0;
                        errorOrder.AppId = commodityOrder.EsAppId.Value;
                        errorOrder.UserId = commodityOrder.UserId;
                        errorOrder.OrderCode = commodityOrder.Code;
                        errorOrder.CouponId = Guid.Empty;
                        errorOrder.Score = srore;
                        errorOrder.SubTime = DateTime.Now;
                        errorOrder.ModifiedOn = DateTime.Now;
                        errorOrder.ScoreType = (int)scoreCostDto.ScoreType;
                        errorOrder.EntityState = System.Data.EntityState.Added;
                        contextSession.SaveObject(errorOrder);
                    }
                    return refundScoreResult;
                }
            }
            return true;
        }

        /// <summary>
        /// 售后退款时回退积分
        /// </summary>
        /// <param name="contextSession"></param>
        /// <param name="commodityOrder"></param>
        /// <param name="orderRefundAfterSales"></param>
        /// <returns></returns>
        public static bool CommodityOrderAfterSalesRefundScore(ContextSession contextSession, CommodityOrder commodityOrder, OrderRefundAfterSales orderRefundAfterSales)
        {
            if (!commodityOrder.EsAppId.HasValue)
            {
                return true;
            }
            //使用了积分
            if (orderRefundAfterSales.RefundScoreMoney > 0)
            {
                //获取当时的积分比例
                CostScoreInfoDTO scoreCostDto;
                if (new ScoreSV().GetScoreCost(commodityOrder.EsAppId.Value, out scoreCostDto, commodityOrder.SubTime))
                {
                    int srore = (int)(orderRefundAfterSales.RefundScoreMoney * scoreCostDto.Cost);
                    bool refundScoreResult = SignSV.Instance.RefundScore(commodityOrder.UserId, commodityOrder.EsAppId.Value, srore, commodityOrder.Id, commodityOrder.Code, scoreCostDto.ScoreType);
                    if (!refundScoreResult)
                    {
                        LogHelper.Error(string.Format("售中退款时回退积分失败。UserId：{0}，EsAppId：{1}，srore：{2}", commodityOrder.UserId, commodityOrder.EsAppId.Value, srore));
                        ErrorCommodityOrder errorOrder = new ErrorCommodityOrder();
                        errorOrder.Id = Guid.NewGuid();
                        errorOrder.ErrorOrderId = commodityOrder.Id;
                        errorOrder.ResourceType = 1;
                        errorOrder.Source = 107;
                        errorOrder.State = 0;
                        errorOrder.AppId = commodityOrder.EsAppId.Value;
                        errorOrder.UserId = commodityOrder.UserId;
                        errorOrder.OrderCode = commodityOrder.Code;
                        errorOrder.CouponId = Guid.Empty;
                        errorOrder.Score = srore;
                        errorOrder.SubTime = DateTime.Now;
                        errorOrder.ModifiedOn = DateTime.Now;
                        errorOrder.ScoreType = (int)scoreCostDto.ScoreType;
                        errorOrder.EntityState = System.Data.EntityState.Added;
                        contextSession.SaveObject(errorOrder);
                    }
                    return refundScoreResult;
                }
            }
            return true;
        }

        /// <summary>
        /// 售后完成三级分销打积分
        /// </summary>
        /// <param name="contextSession"></param>
        /// <param name="commodityOrder"></param>
        /// <param name="commodityOrderService"></param>
        /// <returns></returns>
        public static bool DistributeSaleGiveScore(ContextSession contextSession, CommodityOrder commodityOrder, CommodityOrderService commodityOrderService)
        {
            if (!commodityOrder.EsAppId.HasValue)
            {
                return true;
            }
            List<int> payeeTypeList = new List<int>() { 9, 10, 11 };
            //取得三级分销应分的积分记录
            var osQuery =
                OrderShare.ObjectSet()
                          .Where(
                              t =>
                              t.OrderId == commodityOrder.Id && t.Commission > 0 && payeeTypeList.Contains(t.PayeeType))
                          .ToList();
            //使用了积分
            if (osQuery.Any())
            {
                //获取当时的积分比例
                CostScoreInfoDTO scoreCostDto;
                if (new ScoreSV().GetScoreCost(commodityOrder.EsAppId.Value, out scoreCostDto, commodityOrder.SubTime))
                {
                    bool isAllSuccess = true;
                    foreach (var orderShare in osQuery)
                    {
                        int srore = (int)(orderShare.Commission * scoreCostDto.Cost);
                        bool refundScoreResult = SignSV.Instance.GiveScoreBtpShare(orderShare.PayeeId, commodityOrder.AppId, srore, commodityOrder.Id, commodityOrder.Code, Jinher.AMP.Sign.Deploy.Enum.ScoringType.BtpDistributeSale, orderShare.Commission, scoreCostDto.Cost, scoreCostDto.ScoreType);
                        if (!refundScoreResult)
                        {
                            isAllSuccess = false;
                            LogHelper.Error(string.Format("售售后完成三级分销打积分失败。UserId：{0}，AppId：{1}，srore：{2}，OrderId：{3}", orderShare.PayeeId, commodityOrder.AppId, srore, commodityOrder.Id));
                            ErrorCommodityOrder errorOrder = new ErrorCommodityOrder();
                            errorOrder.Id = Guid.NewGuid();
                            errorOrder.ErrorOrderId = commodityOrder.Id;
                            if (orderShare.PayeeType == 9)
                            {
                                errorOrder.ResourceType = 2;
                            }
                            else if (orderShare.PayeeType == 10)
                            {
                                errorOrder.ResourceType = 3;
                            }
                            else if (orderShare.PayeeType == 11)
                            {
                                errorOrder.ResourceType = 4;
                            }
                            errorOrder.Source = 3;
                            errorOrder.State = 0;
                            errorOrder.AppId = commodityOrder.AppId;
                            errorOrder.UserId = orderShare.PayeeId;
                            errorOrder.OrderCode = commodityOrder.Code;
                            errorOrder.CouponId = Guid.Empty;
                            errorOrder.Score = srore;
                            errorOrder.SubTime = DateTime.Now;
                            errorOrder.ModifiedOn = DateTime.Now;
                            errorOrder.ScoreType = (int)scoreCostDto.ScoreType;
                            errorOrder.EntityState = System.Data.EntityState.Added;
                            contextSession.SaveObject(errorOrder);
                        }
                    }

                    return isAllSuccess;
                }
            }
            return true;
        }

        /// <summary>
        /// 售后完成众销打积分
        /// </summary>
        /// <param name="contextSession"></param>
        /// <param name="commodityOrder"></param>
        /// <param name="commodityOrderService"></param>
        /// <returns></returns>
        public static bool ShareSaleGiveScore(ContextSession contextSession, CommodityOrder commodityOrder, CommodityOrderService commodityOrderService)
        {
            if (!commodityOrder.EsAppId.HasValue)
            {
                return true;
            }
            //使用了积分
            if (commodityOrder.Commission > 0)
            {
                //取商货众销的收款人
                var shareModel =
                    OrderShare.ObjectSet()
                              .Where(t => t.OrderId == commodityOrder.Id && t.PayeeType == 3).FirstOrDefault();

                //没有收款人
                if (shareModel == null || shareModel.PayeeId == Guid.Empty)
                {
                    LogHelper.Error(string.Format("售后完成众销打积，找不到收款人。OrderId:{0}", commodityOrder.Id));
                    return false;
                }
                //获取当时的积分比例
                CostScoreInfoDTO scoreCostDto;
                if (new ScoreSV().GetScoreCost(commodityOrder.EsAppId.Value, out scoreCostDto, commodityOrder.SubTime))
                {
                    int srore = (int)(commodityOrder.Commission * scoreCostDto.Cost);
                    bool refundScoreResult = Instance.GiveScoreBtpShare(shareModel.PayeeId, commodityOrder.AppId, srore, commodityOrder.Id, commodityOrder.Code, ScoringType.BtpShareSale, commodityOrder.Commission, scoreCostDto.Cost, scoreCostDto.ScoreType);
                    if (!refundScoreResult)
                    {
                        LogHelper.Error(string.Format("售后完成众销打积分失败。UserId：{0}，AppId：{1}，srore：{2}，OrderId：{3}", shareModel.PayeeId, commodityOrder.AppId, srore, commodityOrder.Id));
                        ErrorCommodityOrder errorOrder = new ErrorCommodityOrder();
                        errorOrder.Id = Guid.NewGuid();
                        errorOrder.ErrorOrderId = commodityOrder.Id;
                        errorOrder.ResourceType = 5;
                        errorOrder.Source = 3;
                        errorOrder.State = 0;
                        errorOrder.AppId = commodityOrder.AppId;
                        errorOrder.UserId = shareModel.PayeeId;
                        errorOrder.OrderCode = commodityOrder.Code;
                        errorOrder.CouponId = Guid.Empty;
                        errorOrder.Score = srore;
                        errorOrder.SubTime = DateTime.Now;
                        errorOrder.ModifiedOn = DateTime.Now;
                        errorOrder.ScoreType = (int)scoreCostDto.ScoreType;
                        errorOrder.EntityState = System.Data.EntityState.Added;
                        contextSession.SaveObject(errorOrder);
                    }
                    return refundScoreResult;
                }
            }
            return true;
        }

        /// <summary>
        /// 售后完成渠道打积分
        /// </summary>
        /// <param name="contextSession"></param>
        /// <param name="commodityOrder"></param>
        /// <param name="commodityOrderService"></param>
        /// <returns></returns>
        public static bool ChannelShareSaleGiveScore(ContextSession contextSession, CommodityOrder commodityOrder, CommodityOrderService commodityOrderService)
        {
            if (!commodityOrder.EsAppId.HasValue)
            {
                return true;
            }
            //使用了积分
            if (commodityOrder.ChannelShareMoney > 0)
            {
                //取商货众销的收款人
                var shareModel = OrderShare.ObjectSet().FirstOrDefault(t => t.OrderId == commodityOrder.Id && t.PayeeType == 12);

                //没有收款人
                if (shareModel == null || shareModel.PayeeId == Guid.Empty)
                {
                    LogHelper.Error(string.Format("售后完成渠道打积，找不到收款人。OrderId:{0}", commodityOrder.Id));
                    return false;
                }
                //获取当时的积分比例
                CostScoreInfoDTO scoreCostDto;
                if (new ScoreSV().GetScoreCost(commodityOrder.EsAppId.Value, out scoreCostDto, commodityOrder.SubTime))
                {
                    int srore = (int)(commodityOrder.ChannelShareMoney * scoreCostDto.Cost);
                    bool refundScoreResult = Instance.GiveScoreBtpShare(shareModel.PayeeId, commodityOrder.EsAppId.Value, srore, commodityOrder.Id, commodityOrder.Code, ScoringType.BtpChannelBrokerage, commodityOrder.ChannelShareMoney, scoreCostDto.Cost, scoreCostDto.ScoreType);
                    if (!refundScoreResult)
                    {
                        LogHelper.Error(string.Format("售后完成渠道打积分失败。UserId：{0}，AppId：{1}，srore：{2}，OrderId：{3}", shareModel.PayeeId, commodityOrder.AppId, srore, commodityOrder.Id));
                        ErrorCommodityOrder errorOrder = new ErrorCommodityOrder
                        {
                            Id = Guid.NewGuid(),
                            ErrorOrderId = commodityOrder.Id,
                            ResourceType = 6,
                            Source = 3,
                            State = 0,
                            AppId = commodityOrder.EsAppId.Value,
                            UserId = shareModel.PayeeId,
                            OrderCode = commodityOrder.Code,
                            CouponId = Guid.Empty,
                            Score = srore,
                            SubTime = DateTime.Now,
                            ModifiedOn = DateTime.Now,
                            ScoreType = (int)scoreCostDto.ScoreType,
                            EntityState = System.Data.EntityState.Added
                        };
                        contextSession.SaveObject(errorOrder);
                    }
                    return refundScoreResult;
                }
            }
            return true;
        }


    }

    public class SignSVFacade : OutSideFacadeBase
    {
        /// <summary>
        /// 售后完成加积分
        /// </summary>
        /// <param name="commodityOrder"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public bool GiveScoreBtp(CommodityOrder commodityOrder)
        {
            #region
            //bool result = false;
            //if (!commodityOrder.EsAppId.HasValue)
            //{
            //    return result;
            //}
            //string url = string.Format("{0}btp.iuoooo.com/Mobile/ShareMyOrderDetail?appId={1}&type=tuwen&source=share&orderId={2}&SrcType=0", Common.CustomConfig.UrlPrefix, commodityOrder.AppId, commodityOrder.Id);

            ////获取用户账号，那边显示用
            //var jsonr = CBCSV.GetUserNameAndCode(commodityOrder.UserId);
            //var userCode = jsonr.Item2;
            //var dto = new GiveScoreDTO
            //{
            //    Account = userCode,
            //    AppId = commodityOrder.EsAppId.Value,
            //    BizId = commodityOrder.Id,
            //    BizName = "电商",
            //    BiztId = Guid.Empty,
            //    BizType = BizType.CustomBtp,
            //    BizUrl = url,
            //    ScoreType = ScoringType.ShareOrder,
            //    Scores = Convert.ToDouble(commodityOrder.RealPrice),
            //    SubId = commodityOrder.UserId,
            //    UserId = commodityOrder.UserId
            //};
            //try
            //{
            //    Jinher.AMP.Sign.IBP.Facade.GiveRewardsFacade facade = new GiveRewardsFacade();
            //    facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
            //    LogHelper.Info(string.Format("售后完成加积分GiveScoreBtp。dto:{0}", JsonHelper.JsonSerializer(dto)));
            //    var giveScoreBtpResult = facade.GiveScoreBtp(dto);

            //    if (giveScoreBtpResult == null || !giveScoreBtpResult.IsSuccess)
            //    {
            //        LogHelper.Error(string.Format("售后完成加积分失败。dto:{0}", JsonHelper.JsonSerializer(dto)));
            //        return result;
            //    }
            //    result = true;
            //}
            //catch (Exception ex)
            //{
            //    LogHelper.Error(string.Format("售后完成加积分异常。订单入参:{0}", JsonHelper.JsonSerializer(dto)), ex);
            //}
            //return result;
            #endregion

            #region 售后完成加积分(20170615积分比例版本)
            bool result = false;
            if (!commodityOrder.EsAppId.HasValue)
            {
                return false;
            }
            try
            {
                Jinher.AMP.Sign.IBP.Facade.GiveRewardsFacade facade = new GiveRewardsFacade
                {
                    ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo()
                };
                //订单商品id集合
                var commodityIds = OrderItem.ObjectSet().Where(t => t.CommodityOrderId == commodityOrder.Id).Select(t => t.CommodityId);
                //订单金额积分比例额度
                decimal sumRefundScoreMoney = 0;
                foreach (var commodityId in commodityIds)
                {
                    var commodity = Commodity.FindByID(commodityId);
                    if (commodity.ScoreScale != null && commodity.ScoreScale > 0)
                    {
                        sumRefundScoreMoney += (decimal)(commodity.ScoreScale * commodity.Price * (decimal)0.01);
                        LogHelper.Info(string.Format("获取商品积分比例commodity.Id:{0},RefundScoreMoney:{1}", commodity.Id, (decimal)(commodity.ScoreScale * commodity.Price * (decimal)0.01)));
                    }
                    else
                    {
                        //获取店铺积分比例   
                        Sign.Deploy.CustomDTO.ReturnInfoDTO<ScoreSettingsDTO> sInfoDto = facade.GetScoreSetting(commodityOrder.EsAppId.Value, ScoringType.Product);
                        LogHelper.Info(string.Format("获取店铺积分比例GetScoreSetting:{0},commodityOrder.EsAppId:{1}", JsonHelper.JsonSerializer(sInfoDto), commodityOrder.EsAppId.Value));
                        if (sInfoDto.IsSuccess && sInfoDto.Data != null)
                        {
                            sumRefundScoreMoney += (decimal)(sInfoDto.Data.GiveScores * (double)commodity.Price);
                        }
                    }
                }

                LogHelper.Info(string.Format("订单金额积分比例额度sumRefundScoreMoney:{0}", sumRefundScoreMoney));

                //只有当积分金额>0时 调用保存积分接口
                if (sumRefundScoreMoney > 0)
                {
                    var sumSrore = 0;
                    //获取当时的积分汇率
                    CostScoreInfoDTO scoreCostDto;
                    if (new ScoreSV().GetScoreCost(commodityOrder.EsAppId.Value, out scoreCostDto, commodityOrder.SubTime))
                    {
                        sumSrore = (int)(sumRefundScoreMoney * scoreCostDto.Cost);
                    }

                    string url = string.Format("{0}btp.iuoooo.com/Mobile/ShareMyOrderDetail?appId={1}&type=tuwen&source=share&orderId={2}&SrcType=0", Common.CustomConfig.UrlPrefix, commodityOrder.AppId, commodityOrder.Id);
                    //获取用户账号，那边显示用
                    var jsonr = CBCSV.GetUserNameAndCode(commodityOrder.UserId);
                    var userCode = jsonr.Item2;
                    var dto = new GiveScoreDTO
                    {
                        Account = userCode,
                        AppId = commodityOrder.EsAppId.Value,
                        BizId = commodityOrder.Id,
                        BizName = "电商",
                        BiztId = Guid.Empty,
                        BizType = BizType.CustomBtp,
                        BizUrl = url,
                        ScoreType = ScoringType.Product,
                        Scores = sumSrore,
                        SubId = commodityOrder.UserId,
                        UserId = commodityOrder.UserId
                    };

                    LogHelper.Info(string.Format("售后完成加积分GiveScoreBtp。dto:{0},添加积分sumSrore:{1}", JsonHelper.JsonSerializer(dto), sumSrore));
                    var giveScoreBtpResult = facade.GiveScoreBtpNew(dto);
                    if (giveScoreBtpResult == null || !giveScoreBtpResult.IsSuccess)
                    {
                        LogHelper.Error(string.Format("售后完成加积分失败。dto:{0},giveScoreBtpResult：{1}", JsonHelper.JsonSerializer(dto), JsonHelper.JsonSerializer(giveScoreBtpResult)));
                        return false;
                    }
                    result = true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("售后完成加积分异常。"), ex);
            }
            return result;
            #endregion
        }

        /// <summary>
        /// 获取用户在应用下的积分
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="appId">应用id</param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public int GiveUserScore(Guid userId, Guid appId)
        {
            int result = 0;
            if (appId == Guid.Empty || userId == Guid.Empty)
            {
                return result;
            }
            try
            {
                Jinher.AMP.Sign.IBP.Facade.GiveRewardsFacade grFacade = new GiveRewardsFacade();
                grFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                Jinher.AMP.Sign.Deploy.CustomDTO.ReturnInfoDTO<ReturnScoreDTO> signResult = grFacade.QueryCurrentScores(appId, userId);
                //如果用户没有积分，接口返回的IsSuccess是false ，所以我们只判断返回值是不是空
                if (signResult == null || signResult.Data == null)
                {
                    LogHelper.Error(string.Format("获取用户在应用下的积分失败。userId:{0},appId:{1},返回结果：{2}", userId, appId, JsonHelper.JsonSerializer(signResult)));
                    return result;
                }
                result = signResult.Data.AvailableScore;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取用户在应用下的积分异常。userId:{0},appId:{1}", userId, appId), ex);
            }
            return result;
        }
        /// <summary>
        /// 消费积分(支持通用积分)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="appId"></param>
        /// <param name="score"></param>
        /// <param name="commodityOrderId"></param>
        /// <param name="commodityOrderCode"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public bool SpendScore(Guid userId, Guid appId, int score, Guid commodityOrderId, string commodityOrderCode)
        {
            bool result = false;
            try
            {



                //获取用户账号，那边显示用
                var jsonr = CBCSV.GetUserNameAndCode(userId);
                var userCode = jsonr.Item2;
                ConsumeScoreDTO consumeScoreDTO = new ConsumeScoreDTO()
                {
                    ConsumeAccount = userCode,
                    AppId = appId,
                    UserId = userId,
                    BizId = commodityOrderId,
                    BizName = commodityOrderCode,
                    ConsumeScore = score,
                    ConsumeTime = DateTime.Now
                };
                Jinher.AMP.Sign.IBP.Facade.GiveRewardsFacade grFacade = new GiveRewardsFacade();
                grFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                Jinher.AMP.Sign.Deploy.CustomDTO.ReturnInfoDTO<int> signResult = grFacade.ConsumeScoreBtp(consumeScoreDTO);
                if (!signResult.IsSuccess)
                {
                    LogHelper.Error(string.Format("消费积分失败。consumeScoreDTO：{0}", JsonHelper.JsonSerializer(consumeScoreDTO)));
                    return result;
                }
                result = true;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("消费积分异常。。userId：{0}，appId：{1}，score：{2}，commodityOrderId：{3}，commodityOrderCode：{4}", userId, appId, score, commodityOrderId, commodityOrderCode), ex);
            }
            return result;
        }
        /// <summary>
        /// 回退积分
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="appId"></param>
        /// <param name="srore"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public bool RefundScore(Guid userId, Guid appId, int srore, Guid commodityOrderId, string commodityOrderCode, ScoreTypeEnum type)
        {
            bool result = false;
            try
            {
                //获取用户账号，那边显示用
                var jsonr = CBCSV.GetUserNameAndCode(userId);
                var userCode = jsonr.Item2;
                GiveScoreDTO giveScoreDTO = new GiveScoreDTO()
                {
                    Account = userCode,
                    AppId = appId,
                    UserId = userId,
                    BizId = commodityOrderId,
                    BizName = commodityOrderCode,
                    BizType = Jinher.AMP.Sign.Deploy.Enum.BizType.CustomBtp,
                    Scores = srore,
                    SubId = userId
                };
                Jinher.AMP.Sign.IBP.Facade.GiveRewardsFacade grFacade = new GiveRewardsFacade();
                grFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                LogHelper.Info(string.Format("回退积分GiveScoreBtpBackNew。giveScoreDTO：{0}", JsonHelper.JsonSerializer(giveScoreDTO)));
                Jinher.AMP.Sign.Deploy.CustomDTO.ReturnInfoDTO signResult = grFacade.GiveScoreBtpBackNew(giveScoreDTO, DateTime.Now, type == ScoreTypeEnum.Currency);
                if (!signResult.IsSuccess)
                {
                    LogHelper.Error(string.Format("回退积分失败。giveScoreDTO：{0}", JsonHelper.JsonSerializer(giveScoreDTO)));
                    return result;
                }
                result = true;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("回退积分异常。userId：{0}，appId：{1}，srore：{2}，commodityOrderId：{3}，commodityOrderCode：{4}，type：{5}", userId, appId, srore, commodityOrderId, commodityOrderCode, type), ex);
            }
            return result;
        }

        /// <summary>
        /// 评价送积分
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="appId">esAppId</param>
        /// <param name="bizUrl"></param>
        /// <param name="commodityOrderId"></param>
        /// <param name="commodityOrderCode"></param>
        /// <param name="reviewId"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public bool GiveScoreBtpComment(Guid userId, Guid appId, string bizUrl, Guid commodityOrderId, string commodityOrderCode, Guid reviewId)
        {
            bool result = false;
            try
            {
                //获取用户账号，那边显示用
                var jsonr = CBCSV.GetUserNameAndCode(userId);
                var userCode = jsonr.Item2;
                GiveScoreDTO giveScoreDTO = new GiveScoreDTO()
                {
                    Account = userCode,
                    AppId = appId,
                    UserId = userId,
                    BizId = commodityOrderId,
                    BiztId = reviewId,
                    BizName = commodityOrderCode,
                    BizType = Jinher.AMP.Sign.Deploy.Enum.BizType.CustomBtp,
                    BizUrl = bizUrl,
                    ScoreType = Jinher.AMP.Sign.Deploy.Enum.ScoringType.Comment
                };
                Jinher.AMP.Sign.IBP.Facade.GiveRewardsFacade grFacade = new GiveRewardsFacade();
                grFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                LogHelper.Info(string.Format("评价送积分GiveScoreBtpComment。giveScoreDTO：{0}", JsonHelper.JsonSerializer(giveScoreDTO)));
                Jinher.AMP.Sign.Deploy.CustomDTO.ReturnInfoDTO signResult = grFacade.GiveScoreBtpComment(giveScoreDTO);
                if (!signResult.IsSuccess)
                {
                    LogHelper.Error(string.Format("评价送积分失败。giveScoreDTO：{0}", JsonHelper.JsonSerializer(giveScoreDTO)));
                    return result;
                }
                result = true;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("评价送积分异常。userId：{0}，appId：{1}，bizUrl：{2}，commodityOrderId：{3}，commodityOrderCode：{4}，reviewId：{5}", userId, appId, bizUrl, commodityOrderId, commodityOrderCode, reviewId), ex);
            }
            return result;
        }

        /// <summary>
        /// 众销、分销，以及渠道发放积分
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="appId"></param>
        /// <param name="srore"></param>
        /// <param name="commodityOrderId"></param>
        /// <param name="commodityOrderCode"></param>
        /// <param name="scoringType">积分类型(1签到 2评价 3分享 4发布内容,5赠送积分,6购买商品送积分 7.分享商品 8.分享订单 9.分享图文 10.分享多媒体 11.下载应用 12.分享应用,13.C6OA系统赠送积分 14.商品退货退积分 15.电商三级分销 16.电商-众销 17.电商-渠道佣金)</param>
        /// <param name="money">佣金金额</param>
        /// <param name="scale">佣金兑换比例</param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public bool GiveScoreBtpShare(Guid userId, Guid appId, int srore, Guid commodityOrderId, string commodityOrderCode, Jinher.AMP.Sign.Deploy.Enum.ScoringType scoringType, decimal money, int scale, ScoreTypeEnum scoreType)
        {
            bool result = false;
            string scoringTypeName = string.Empty;
            //设置积分显示名称
            string bizName = commodityOrderCode;
            if (scoringType == Jinher.AMP.Sign.Deploy.Enum.ScoringType.BtpDistributeSale)
            {
                scoringTypeName = "分销";
                bizName = "分销佣金" + money + "元（1元=" + scale + "积分）";
            }
            else if (scoringType == Jinher.AMP.Sign.Deploy.Enum.ScoringType.BtpShareSale)
            {
                scoringTypeName = "众销";
                bizName = "众销佣金" + money + "元（1元=" + scale + "积分）";
            }
            else if (scoringType == Jinher.AMP.Sign.Deploy.Enum.ScoringType.BtpChannelBrokerage)
            {
                scoringTypeName = "渠道";
                bizName = "渠道佣金" + money + "元（1元=" + scale + "积分）";
            }
            try
            {
                //获取用户账号，那边显示用
                var jsonr = CBCSV.GetUserNameAndCode(userId);
                var userCode = jsonr.Item2;
                GiveScoreDTO giveScoreDTO = new GiveScoreDTO()
                {
                    Account = userCode,
                    AppId = appId,
                    UserId = userId,
                    BizId = commodityOrderId,
                    BizName = bizName,
                    BizType = Jinher.AMP.Sign.Deploy.Enum.BizType.CustomBtp,
                    Scores = srore,
                    SubId = userId,
                    ScoreType = scoringType
                };
                Jinher.AMP.Sign.IBP.Facade.GiveRewardsFacade grFacade = new GiveRewardsFacade();
                grFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                LogHelper.Info(scoringTypeName + string.Format("发放积分GiveScoreBtpShare。giveScoreDTO：{0}；scoreType：{1}", JsonHelper.JsonSerializer(giveScoreDTO), scoreType));
                Jinher.AMP.Sign.Deploy.CustomDTO.ReturnInfoDTO signResult = grFacade.GiveScoreBtpShareNew(giveScoreDTO, DateTime.Now, scoreType == ScoreTypeEnum.Currency);
                if (!signResult.IsSuccess)
                {
                    LogHelper.Error(scoringTypeName + string.Format("发放积分失败。giveScoreDTO：{0}", JsonHelper.JsonSerializer(giveScoreDTO)));
                    return result;
                }
                result = true;
            }
            catch (Exception ex)
            {
                LogHelper.Error(scoringTypeName + string.Format("发放积分异常。userId：{0}，appId：{1}，srore：{2}，commodityOrderId：{3}，commodityOrderCode：{4}，scoringType：{5}，money：{6}，scale：{7}", userId, appId, srore, commodityOrderId, commodityOrderCode, (int)scoringType, money, scale), ex);
            }
            return result;
        }
        #region 通用积分
        /// <summary>
        /// 获取某个时间点通用积分汇率
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnExchangeRateInfoDTO GetCurrencyScoreCost(Guid appId, DateTime time)
        {
            ReturnExchangeRateInfoDTO result = null;
            try
            {
                Jinher.AMP.Sign.IBP.Facade.GiveRewardsFacade facade = new GiveRewardsFacade();
                facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                var searchResult = facade.QueryExchangeRate(appId, time);
                if (searchResult != null && searchResult.IsSuccess && searchResult.Data != null)
                {
                    LogHelper.Info(string.Format(
                            "获取某个时间点通用积分汇率GetCurrencyScoreCost。searchResult.Data.ExchangeRate：{0};searchResult.Data.IsUnitiveScore：{1}",
                            searchResult.Data.ExchangeRate, searchResult.Data.IsUnitiveScore));
                    result = searchResult.Data;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取通用积分汇率异常。appId：{0}，time：{1}", appId, time), ex);
            }
            return result;
        }



        /// <summary>
        /// 校验用户积分
        /// </summary>
        /// <param name="appIds">appid集合</param>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnUnitiveInfoDTO QueryAllScores(List<Guid> appIds, Guid userId)
        {
            ReturnUnitiveInfoDTO result = new ReturnUnitiveInfoDTO();
            try
            {

                Jinher.AMP.Sign.IBP.Facade.GiveRewardsFacade facade = new GiveRewardsFacade();
                facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                var searchResult = facade.QueryAllScores(appIds, userId);
                if (searchResult == null || !searchResult.IsSuccess || searchResult.Data == null)
                {
                    return result;
                }
                return searchResult.Data;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取通用积分汇率异常。appIds：{0}，userId：{1}", string.Join(",", appIds), userId), ex);
            }
            return result;
        }
        #endregion
    }



}
