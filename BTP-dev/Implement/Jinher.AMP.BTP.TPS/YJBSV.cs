using System;
using System.Collections.Generic;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.YJB.Deploy.CustomDTO;
using Jinher.AMP.YJB.Deploy.CustomDTO.YJCard;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Common.Extensions;


namespace Jinher.AMP.BTP.TPS
{
    /*
 * 请注意：！！！！！！！！！！！！！！！
 * 外部引用请将代码写到TPS.XXXFacade中，自定义扩展请写到TPS.XXXSV、TPS.XXXBP中
 */
    /// <summary>
    /// 易捷币相关接口
    /// </summary>
    public class YJBSV : OutSideServiceBase<YJBFacade>
    {
        /// <summary>
        /// 获取商品的抵现比例信息
        /// </summary>
        public static OrderInsteadCashDTO GetCommodityCashPercentWithoutUser(List<OrderInsteadCashInputCommodityDTO> commodities)
        {
            if (commodities == null || commodities.Count == 0)
            {
                return new OrderInsteadCashDTO { Enabled = false };
            }
            var result = Instance.GetCommodityCashPercentWithoutUser(commodities);
            if (result == null)
            {
                return new OrderInsteadCashDTO { Enabled = false };
            }
            return result;
        }

        /// <summary>
        /// 查询用户可用的商品的抵现比例
        /// </summary>
        public static CanInsteadCashDTO GetCommodityCashPercent(OrderInsteadCashInputDTO input)
        {
            var emptyResult = new CanInsteadCashDTO { YJBInfo = new OrderInsteadCashDTO { Enabled = false }, YJCouponInfo = new YJCouponCanInsteadCashDTO { YJCoupons = new List<MUserCouponDto>() } };
            if (input == null || input.Commodities == null || input.Commodities.Count == 0)
            {
                return emptyResult;
            }
            var result = Instance.GetCommodityCashPercent(input);
            LogHelper.Info("YJBSV.GetCommodityCashPercent，输入：" + JsonHelper.JsonSerializer(input) + "，输出：" + JsonHelper.JsonSerializer(result));
            if (result == null)
            {
                return emptyResult;
            }
            return result;
        }

        /// <summary>
        /// 查询商品的抵现比例
        /// </summary>
        public static Jinher.AMP.YJB.Deploy.CustomDTO.ListResultDTO<CommodityCashOutput> GetCommodityCashPercent(CommodityCashInput input)
        {
            return Instance.GetCommodityCashPercent(input);
        }

        /// <summary>
        /// 创建定单时，记录易捷币消费流水
        /// </summary>
        public static ResultDTO CreateOrderJournal(CreateOrderInputDTO input)
        {
            // 默认冻结先冻结易捷币
            input.Freezed = true;
            return Instance.CreateOrderJournal(input);
        }

        /// <summary>
        ///  支付成功时，取消易捷币冻结
        /// </summary>
        public static ResultDTO PayOrderJournal(Jinher.AMP.BTP.BE.CommodityOrder order)
        {
            return Instance.PayOrderJournal(order.Id);
        }

        /// <summary>
        ///  取消订单，回退记录易捷币消费流水
        /// </summary>
        public static ResultDTO CancelOrderJournal(Guid orderId)
        {
            return Instance.CancelOrderJournal(orderId);
        }

        /// <summary>
        ///  退货时，记录易捷币返还流水
        /// </summary>
        public static ResultDTO RefundOrderJournal(Guid orderId, decimal refundMoney, decimal commodityPrice, string UserMobile, Guid UserId, Guid useryucouponid)
        {
            return Instance.RefundOrderJournal(orderId, refundMoney, commodityPrice, UserMobile, UserId, useryucouponid);
        }

        /// <summary>
        ///  单品退货时，记录易捷币返还流水
        /// </summary>
        public static ResultDTO RefundOrderItemJournal(Guid orderId, Guid commodityId, decimal refundMoney, Guid orderItemId, decimal commodityPrice, string UserMobile, Guid UserId, Guid useryucouponid)
        {
            return Instance.RefundOrderItemJournal(orderId, commodityId, refundMoney, orderItemId, commodityPrice, UserMobile, UserId, useryucouponid);
        }

        /// <summary>
        ///  全部退货时，记录易捷币返还流水
        /// </summary>
        public static ResultDTO RefundAllOrderJournal(Guid orderId)
        {
            return Instance.RefundAllOrderJournal(orderId);
        }
        /// <summary>
        /// 退款时，把易捷卡消费的资金退回到卡里
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="Refund"></param>
        /// <returns></returns>
        public static ResultDTO UpdateCardCash(Guid UserId, Decimal Refund)
        {
            return Instance.UpdateCardCash(UserId, Refund);
        }

        /// <summary>
        /// 退回易捷卡（消费退货）
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public static ResultDTO RetreatYjc(Guid UserId, Decimal Refund, Guid orderId, Guid orderItemId)
        {
            return Instance.RetreatYjc(UserId, Refund, orderId, orderItemId);
        }
        /// <summary>
        /// 根据卡绑定人获取易捷卡面值
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public static decimal GetYJCardAcount(Guid UserId)
        {
            return Instance.GetYJCardAcount(UserId);
        }

        /// <summary>
        ///  查询订单的易捷币抵用信息
        /// </summary>
        public static ResultDTO<OrderYJBInfoDTO> GetOrderYJBInfo(Guid? esAppId, Guid orderId)
        {
            if (!esAppId.HasValue || esAppId != Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId)
            {
                return ResultDTO<OrderYJBInfoDTO>.Error("Disabled", "未启用易捷币");
            }
            var result = Instance.GetOrderYJBInfo(orderId);
            if (!result.IsSuccess)
            {
                if (result.Code == "OrderNotFound")
                {
                    return ResultDTO<OrderYJBInfoDTO>.Success(new OrderYJBInfoDTO { InsteadCashAmount = 0, InsteadCashCount = 0 });
                }
            }
            return result;
        }

        /// <summary>
        ///  查询订单的易捷抵用信息
        /// </summary>
        public static ResultDTO<YJOrderInfoDTO> GetOrderInfo(Guid? esAppId, Guid orderId)
        {
            if (!esAppId.HasValue || esAppId != Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId)
            {
                return ResultDTO<YJOrderInfoDTO>.Error("Disabled", "未启用易捷币");
            }
            var result = Instance.GetOrderInfo(orderId);
            if (!result.IsSuccess)
            {
                if (result.Code == "OrderNotFound")
                {
                    return ResultDTO<YJOrderInfoDTO>.Success(new YJOrderInfoDTO { YJBInfo = new OrderYJBInfoDTO { InsteadCashAmount = 0, InsteadCashCount = 0 } });
                }
            }
            return result;
        }

        /// <summary>
        ///  查询订单的易捷币抵用信息
        /// </summary>
        public static ResultDTO<List<OrderYJBInfoDTO>> GetOrderYJBInfoes(List<Guid> orderIds)
        {
            return Instance.GetOrderYJBInfoes(orderIds);
        }

        /// <summary>
        ///  查询订单的易捷币抵用信息
        /// </summary>
        public static YJOrderInfoDTO GetOrderInfo(Guid orderId)
        {
            var result = Instance.GetOrderInfo(orderId);
            if (result.IsSuccess)
            {
                return result.Data;
            }
            return null;
        }

        /// <summary>
        ///  查询订单的易捷币抵用信息
        /// </summary>
        public static ResultDTO<List<YJOrderInfoDTO>> GetOrderInfoes(List<Guid> orderIds)
        {
            return Instance.GetOrderInfoes(orderIds);
        }

        /// <summary>
        ///  查询订单项的易捷币抵用信息
        /// </summary>
        public static ResultDTO<OrderItemYJBInfoDTO> GetOrderItemYJBInfo(Guid appId, Guid orderId)
        {
            LogHelper.Debug("查询订单项的易捷币抵用信息，GetOrderItemYJBInfo，appId：" + appId + ",YJBConsts.YJAppId：" + YJBConsts.YJAppId);

            if (appId != YJB.Deploy.CustomDTO.YJBConsts.YJAppId)
            {
                return new ResultDTO<OrderItemYJBInfoDTO> { IsSuccess = false, Data = null };
            }
            return Instance.GetOrderItemYJBInfo(orderId);
        }


        /// <summary>
        ///  获取用户的易捷币信息
        /// </summary>
        public static ResultDTO<UserYJBDTO> GetUserYJB(Guid userId)
        {
            return Instance.GetUserYJB(userId);
        }

        /// <summary>
        ///  获取用户的易捷币流水信息
        /// </summary>
        public static ListResultDTO<UserYJBJournalDTO> GetUserYJBJournal(OrderYJBInfoInputDTO input)
        {
            return Instance.GetUserYJBJournal(input);
        }

        /// <summary>
        /// 获取商品的油卡兑换券额度
        /// </summary>
        public static List<CommodityYouKaDTO> GetCommodityYouKaPercent(GetYouKaPersentDTO input)
        {
            return Instance.GetCommodityYouKaPercent(input);
        }

        /// <summary>
        ///  获取易捷抵用券信息
        /// </summary>
        public static YJCouponInfoDto GetYJCouponInfo(Guid userId)
        {
            var result = Instance.GetYJCouponInfo(userId);
            if (result.IsSuccess) return result.Data;
            return null;
        }
        /// <summary>
        ///  根据类型获取易捷抵用券信息
        /// </summary>
        public static List<YJCouponInfoDto> GetYJCouponInfo()
        {
            var result = Instance.GetYJCouponInfoByType();
            if (result.IsSuccess) return result.Data;
            return null;
        }

        /// <summary>
        ///  获取易捷抵用券信息
        /// </summary>
        public static YJCouponSettingDto GetYJCouponSetting(Guid appId)
        {
            var result = Instance.GetYJCouponSetting(appId);
            if (result.IsSuccess) return result.Data;
            return new YJCouponSettingDto { CanCombinabled = false, CanMultipuled = false };
        }


        /// <summary>
        /// 获取我的易捷抵现劵信息
        /// </summary>
        public static List<YJCouponWithCommodityInfo> GetMyYJCouponInfoWithApp(YJCouponWithCommodityInfoInput input)
        {
            input.EsAppId = YJBConsts.YJAppId;
            var result = Instance.GetMyYJCouponInfoWithApp(input);
            if (result.IsSuccess) return result.Data;
            return null;
        }

        /// <summary>
        ///  查询订单项的易捷-抵用信息
        /// </summary>
        public static ResultDTO<OrderItemYJInfoDTO> GetOrderItemYJInfo(Guid appId, Guid orderId)
        {
            LogHelper.Debug("查询订单项的易捷币抵用信息，GetOrderItemYJInfo，appId：" + appId + ",YJBConsts.YJAppId：" + YJBConsts.YJAppId);

            if (appId != YJB.Deploy.CustomDTO.YJBConsts.YJAppId && appId != new Guid("1375ad99-de3b-4e93-80d5-5b96e1588967"))
            {
                return new ResultDTO<OrderItemYJInfoDTO> { IsSuccess = false, Data = null };
            }
            return Instance.GetOrderItemYJInfo(orderId);
        }
        /// <summary>
        /// 获取严选商品定时改价商品ID
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>       
        public static ResultDTO<List<Jinher.AMP.YJB.Deploy.PriceChangeDetailDTO>> GetYXChangeComInfo(List<Guid> appId)
        {
            return Instance.GetYXChangeComInfo(appId);
        }

        public static ListResultDTO<YJB.Deploy.YJBDSFOrderInfoDTO> GetDSFOrderInfo(YJBDSFOrderInfoSearchDTO input)
        {
            return Instance.GetDSFOrderInfo(input);
        }
        /// <summary>
        /// 定时改价商品信息
        /// </summary>       
        public static ResultDTO<List<Jinher.AMP.YJB.Deploy.PriceChangeDetailDTO>> GetAllChangeInfo(System.Collections.Generic.List<System.Guid> Appids, string StarTime, string EndTime)
        {
            return Instance.GetAllChangeInfo(Appids, StarTime, EndTime);
        }


        public static ResultDTO<List<YJB.Deploy.CustomDTO.UserYJCouponJounalDTO>> GetUserYJCouponByOrderId(Guid orderId)
        {
            return Instance.GetUserYJCouponByOrderId(orderId);
        }

        public static ResultDTO<List<YJB.Deploy.YJCouponItemJournalDTO>> GetUserYJCouponItemByOrderId(Guid orderItemId)
        {
            return Instance.GetUserYJCouponItemByOrderId(orderItemId);
        }

        /// <summary>
        /// 下订单消费易捷卡
        /// </summary>
        /// <returns></returns>
        public static ResultDTO ConsumeYjcBatch(ConsumeYjcBatchParamDTO dto)
        {
            return Instance.ConsumeYjcBatch(dto);
        }

        /// <summary>
        /// 获取积分及账务信息
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<Jinher.AMP.YJB.Deploy.CustomDTO.MyCapitalDTO> GetMyCapitals(System.Guid appId, System.Guid userId)
        {
            return Instance.GetMyCapitals(appId, userId);
        }    

        /// <summary>
        /// 获取积分信息
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<Jinher.AMP.YJB.Deploy.CustomDTO.MyCapitalDTO> GetMyAllScore(System.Guid appId, System.Guid userId)
        {
            return Instance.GetAllMyScore(appId, userId);
        }
    }

    public class YJBFacade : OutSideFacadeBase
    {
        /// <summary>
        /// 获取商品的抵现比例信息
        /// </summary>
        [BTPAopLogMethod]
        public OrderInsteadCashDTO GetCommodityCashPercentWithoutUser(List<OrderInsteadCashInputCommodityDTO> commodities)
        {
            ResultDTO<OrderInsteadCashDTO> result = null;
            try
            {
                Jinher.AMP.YJB.ISV.Facade.YJBInfoFacade facade = new YJB.ISV.Facade.YJBInfoFacade();
                //facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                result = facade.GetCommodityCashPercentWithoutUser(commodities);
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.GetCommodityCashPercentWithoutUser服务异常，获取商品的抵现比例信息异常。 输入：" + JsonHelper.JsonSerializer(commodities), ex);
            }
            if (result != null && result.IsSuccess)
            {
                return result.Data;
            }
            else
            {
                LogHelper.Error("YJBSV.GetCommodityCashPercentWithoutUser服务失败，获取商品的抵现比例信息失败。 输入：" + JsonHelper.JsonSerializer(commodities) + "，返回：" + JsonHelper.JsonSerializer(result));
                return null;
            }
        }

        /// <summary>
        /// 获取商品的抵现比例信息
        /// </summary>
        [BTPAopLogMethod]
        public CanInsteadCashDTO GetCommodityCashPercent(OrderInsteadCashInputDTO input)
        {
            ResultDTO<CanInsteadCashDTO> result = null;
            try
            {
                Jinher.AMP.YJB.ISV.Facade.YJBInfoFacade facade = new YJB.ISV.Facade.YJBInfoFacade();
                //facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                result = facade.GetCommodityCashPercent(input);
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.GetCommodityCashPercent服务异常，获取商品的抵现比例信息异常。 输入：" + JsonHelper.JsonSerializer(input), ex);
            }
            if (result != null && result.IsSuccess)
            {
                return result.Data;
            }
            else
            {
                LogHelper.Error("YJBSV.GetCommodityCashPercent服务失败，获取商品的抵现比例信息失败。 输入：" + JsonHelper.JsonSerializer(input) + "，返回：" + JsonHelper.JsonSerializer(result));
                return null;
            }
        }

        /// <summary>
        /// 获取商品的抵现比例信息
        /// </summary>
        [BTPAopLogMethod]
        public Jinher.AMP.YJB.Deploy.CustomDTO.ListResultDTO<CommodityCashOutput> GetCommodityCashPercent(CommodityCashInput input)
        {
            Jinher.AMP.YJB.Deploy.CustomDTO.ListResultDTO<CommodityCashOutput> result = null;
            try
            {
                var facade = new YJB.ISV.Facade.CommodityCashPercentFacade();
                //facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                result = facade.GetCommodityPercent(input);
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.GetCommodityCashPercent服务异常，获取商品的抵现比例信息异常。 输入：" + JsonHelper.JsonSerializer(input), ex);
            }
            if (result != null && result.IsSuccess)
            {
                return result;
            }
            else
            {
                LogHelper.Error("YJBSV.GetCommodityCashPercent服务失败，获取商品的抵现比例信息失败。 输入：" + JsonHelper.JsonSerializer(input) + "，返回：" + JsonHelper.JsonSerializer(result));
                return null;
            }
        }

        /// <summary>
        /// 创建定单时，记录易捷币消费流水
        /// </summary>
        [BTPAopLogMethod]
        public ResultDTO CreateOrderJournal(CreateOrderInputDTO input)
        {
            ResultDTO result;
            try
            {
                var facade = new YJB.ISV.Facade.YJBJournalFacade();
                //facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                input.AppKey = YJBConsts.YJAppKey;
                result = facade.CreateOrderJournal(input);
                if (!result.IsSuccess)
                {
                    LogHelper.Error("YJBSV.CreateOrderJournal服务失败，记录易捷币消费流水失败。 输入：" + JsonHelper.JsonSerializer(input) + " ，输出：" + result.Message);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.CreateOrderJournal服务异常，记录易捷币消费流水异常。 输入：" + JsonHelper.JsonSerializer(input), ex);
                result = ResultDTO.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// 支付成功时，取消易捷币冻结
        /// </summary>
        [BTPAopLogMethod]
        public ResultDTO PayOrderJournal(Guid orderId)
        {
            ResultDTO result;
            try
            {
                var facade = new YJB.ISV.Facade.YJBJournalFacade();
                //facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                result = facade.PayOrderJournal(new CancelOrderDTO { AppKey = YJBConsts.YJAppKey, OrderId = orderId });
                if (!result.IsSuccess)
                {
                    LogHelper.Error("YJBSV.PayOrderJournal服务失败，取消易捷币冻结失败。 输入：OrderId=" + orderId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.PayOrderJournal服务异常，取消易捷币冻结异常。 输入：OrderId=" + orderId, ex);
                result = ResultDTO.Error(ex);
            }
            return result;
        }

        /// <summary>
        ///  取消订单，回退记录易捷币消费流水
        /// </summary>
        [BTPAopLogMethod]
        public ResultDTO CancelOrderJournal(Guid orderId)
        {
            ResultDTO result = new ResultDTO();
            try
            {
                //var facade = new YJB.ISV.Facade.YJBJournalFacade();
                //facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                //result = facade.CancelOrderJournal(new CancelOrderDTO { AppKey = YJBConsts.YJAppKey, OrderId = orderId });
                //if (!result.IsSuccess)
                //{
                //    if (result.Code != "OrderNotFound")
                //        LogHelper.Error("YJBSV.CancelOrderJournal服务失败，回退记录易捷币消费流水失败。 输入：OrderId=" + orderId + "，输出：" + result.Message);
                //}
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.CancelOrderJournal服务异常，回退记录易捷币消费流水异常。 输入：OrderId=" + orderId, ex);
                result = ResultDTO.Error(ex);
            }
            return result;
        }

        /// <summary>
        ///  退货时，记录易捷币返还流水
        /// </summary>
        [BTPAopLogMethod]
        public ResultDTO RefundOrderJournal(Guid orderId, decimal refundMoney, decimal commodityPrice, string UserMobile, Guid UserId, Guid useryucouponid)
        {
            ResultDTO result;
            try
            {
                var facade = new YJB.ISV.Facade.YJBJournalFacade();
                //facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                result = facade.RefundOrderJournal(new CancelOrderDTO { AppKey = YJBConsts.YJAppKey, OrderId = orderId, CommodityPrice = commodityPrice, RefundMoney = refundMoney, UserYJCouponId = useryucouponid, userMobile = UserMobile, userID = UserId });
                if (!result.IsSuccess)
                {
                    if (result.Code != "OrderNotFound")
                        LogHelper.Error("YJBSV.RefundOrderJournal服务失败，记录易捷币返还流水失败。 输入：OrderId=" + orderId + "，输出：" + result.Message);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.RefundOrderJournal服务异常，记录易捷币返还流水异常。 输入：OrderId=" + orderId, ex);
                result = ResultDTO.Error(ex);
            }
            return result;
        }
        /// <summary>
        /// 退款时，给易捷卡退款
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="Refund"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ResultDTO UpdateCardCash(Guid UserId, Decimal Refund)
        {
            ResultDTO result;
            try
            {
                var facade = new YJB.ISV.Facade.YJCardFacade();
                result = facade.UpdateCardCash(new QueryYJCardDTO { UserId = UserId, Refund = Refund });
                if (!result.IsSuccess)
                {
                    LogHelper.Error("YJBSV.UpdateCardCash服务失败，退回易捷卡的消费失败。 输入：UserId=" + UserId + "，输出：" + result.Message);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.UpdateCardCash服务异常，记录易捷币返还流水异常。 输入：UserId=" + UserId, ex);
                result = ResultDTO.Error(ex);
                throw;
            }
            return result;
        }
        /// <summary>
        /// 退回易捷卡（消费退货）
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ResultDTO RetreatYjc(Guid UserId, Decimal Refund,Guid orderId,Guid orderItemId)
        {
            ResultDTO result = new ResultDTO();
            try
            {
                if (Refund <= 0)
                {
                    return result;
                }

                var facade = new YJB.ISV.Facade.YJCardFacade();
                result = facade.RetreatYjc(new QueryYJCardDTO { UserId = UserId, Refund = Refund, OrderId = orderId, OrderItemId = orderItemId });
                if (result.Code != "0")
                {
                    LogHelper.Error("YJBSV.RetreatYjc，退回易捷卡的消费失败。 输入：UserId=" + UserId + "，输出：" + result.Message);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.UpdateCardCash服务异常，记录易捷币返还流水异常。 输入：UserId=" + UserId, ex);
                result = ResultDTO.Error(ex);
                throw;
            }
            return result;
        }

        /// <summary>
        /// 根据卡绑定人获取易捷卡面值
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public decimal GetYJCardAcount(Guid UserId)
        {
            decimal result;
            try
            {
                var facade = new YJB.ISV.Facade.YJCardFacade();
                result = facade.GetYJCardAcount(UserId);
                if (result <= 0)
                {
                    LogHelper.Error("YJBSV.GetYJCardAcount，退回易捷卡的消费失败。 输入：UserId=" + UserId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.GetYJCardAcount服务异常，记录易捷币返还流水异常。 输入：UserId=" + UserId, ex);
                throw;
            }
            return result;
        }

        /// <summary>
        ///  退货时，记录易捷币返还流水
        /// </summary>
        [BTPAopLogMethod]
        public ResultDTO RefundAllOrderJournal(Guid orderId)
        {
            ResultDTO result;
            try
            {
                var facade = new YJB.ISV.Facade.YJBJournalFacade();
                //facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                result = facade.RefundAllOrderJournal(new CancelOrderDTO { AppKey = YJBConsts.YJAppKey, OrderId = orderId });
                if (!result.IsSuccess)
                {
                    if (result.Code != "OrderNotFound")
                        LogHelper.Error("YJBSV.RefundAllOrderJournal服务失败，记录易捷币返还流水失败。 输入：OrderId=" + orderId + "，输出：" + result.Message);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.RefundAllOrderJournal服务异常，记录易捷币返还流水异常。 输入：OrderId=" + orderId, ex);
                result = ResultDTO.Error(ex);
            }
            return result;
        }

        /// <summary>
        ///  单品退货时，记录易捷币返还流水
        /// </summary>
        [BTPAopLogMethod]
        public ResultDTO RefundOrderItemJournal(Guid orderId, Guid commodityId, decimal refundMoney, Guid orderItemId, decimal commodityPrice, string UserMobile, Guid UserId, Guid useryjcouponid)
        {
            LogHelper.Debug("单品退货时，记录易捷币返还流水:orderId :" + orderId + ",commodityId:" + commodityId + ",refundMoney:" + refundMoney + ",orderItemId:" + orderItemId + ",commodityPrice :" + commodityPrice + ",UserMobile:" + UserMobile + ",UserId:" + UserId);
            ResultDTO result;
            try
            {
                var facade = new YJB.ISV.Facade.YJBJournalFacade();
                //facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                result = facade.RefundOrderItemJournal(new CancelOrderItemDTO { AppKey = YJBConsts.YJAppKey, OrderId = orderId, CommodityId = commodityId, RefundMoney = refundMoney, OrderItemId = orderItemId, CommodityPrice = commodityPrice, UserYJCouponId = useryjcouponid, userMobile = UserMobile, userID = UserId });
                LogHelper.Debug("单品退货时，记录易捷币返还流水:result :" + JsonHelper.JsSerializer(result));
                if (!result.IsSuccess)
                {
                    if (result.Code != "OrderNotFound")
                        LogHelper.Error("YJBSV.RefundOrderItemJournal服务失败，记录易捷币返还流水失败。 输入：OrderId=" + orderId + " CommodityID=" + commodityId + "，输出：" + result.Message);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.RefundOrderItemJournal服务异常，记录易捷币返还流水异常。 输入：OrderId=" + orderId + " CommodityID=" + commodityId, ex);
                result = ResultDTO.Error(ex);
            }
            return result;
        }


        /// <summary>
        ///  查询订单的易捷抵用信息
        /// </summary>
        [BTPAopLogMethod]
        public ResultDTO<YJOrderInfoDTO> GetOrderInfo(Guid orderId)
        {
            ResultDTO<YJOrderInfoDTO> result;
            try
            {
                var facade = new YJB.ISV.Facade.YJBJournalFacade();
                //facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                result = facade.GetOrderInfo(orderId);
                if (!result.IsSuccess)
                {
                    if (result.Code != "OrderNotFound")
                        LogHelper.Error("YJBSV.GetOrderInfo服务失败，查询订单的易捷抵用信息失败。 输入：OrderId=" + orderId + "，输出：" + result.Message);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.GetOrderInfo服务异常，查询订单的易捷抵用信息异常。 输入：OrderId=" + orderId, ex);
                result = ResultDTO<YJOrderInfoDTO>.Error(ex);
            }
            return result;
        }

        /// <summary>
        ///  查询订单的易捷币抵用信息
        /// </summary>
        [BTPAopLogMethod]
        public ResultDTO<OrderYJBInfoDTO> GetOrderYJBInfo(Guid orderId)
        {
            ResultDTO<OrderYJBInfoDTO> result;
            try
            {
                var facade = new YJB.ISV.Facade.YJBJournalFacade();
                //facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                result = facade.GetOrderYJBInfo(orderId);
                if (!result.IsSuccess)
                {
                    if (result.Code != "OrderNotFound")
                        LogHelper.Error("YJBSV.GetOrderYJBInfo服务失败，查询订单的易捷币抵用信息失败。 输入：OrderId=" + orderId + "，输出：" + result.Message);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.GetOrderYJBInfo服务异常，查询订单的易捷币抵用信息异常。 输入：OrderId=" + orderId, ex);
                result = ResultDTO<OrderYJBInfoDTO>.Error(ex);
            }
            return result;
        }

        /// <summary>
        ///  查询多个订单的易捷抵用信息
        /// </summary>
        [BTPAopLogMethod]
        public ResultDTO<List<YJOrderInfoDTO>> GetOrderInfoes(List<Guid> orderIds)
        {
            ResultDTO<List<YJOrderInfoDTO>> result;
            try
            {
                var facade = new YJB.ISV.Facade.YJBJournalFacade();
                //facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                result = facade.GetOrderInfoes(orderIds);
                if (!result.IsSuccess)
                {
                    if (result.Code != "OrderNotFound")
                        LogHelper.Error("YJBSV.GetOrderInfoes服务失败，查询订单的易捷抵用信息失败。 输入：OrderIds=" + JsonHelper.JsonSerializer(orderIds) + "，输出：" + result.Message);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.GetOrderInfoes服务异常，查询订单的易捷抵用信息异常。 输入：OrderIds=" + JsonHelper.JsonSerializer(orderIds), ex);
                result = ResultDTO<List<YJOrderInfoDTO>>.Error(ex);
            }
            return result;
        }

        /// <summary>
        ///  查询多个订单的易捷币抵用信息
        /// </summary>
        [BTPAopLogMethod]
        public ResultDTO<List<OrderYJBInfoDTO>> GetOrderYJBInfoes(List<Guid> orderIds)
        {
            ResultDTO<List<OrderYJBInfoDTO>> result;
            try
            {
                var facade = new YJB.ISV.Facade.YJBJournalFacade();
                //facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                result = facade.GetOrderYJBInfoes(orderIds);
                if (!result.IsSuccess)
                {
                    if (result.Code != "OrderNotFound")
                        LogHelper.Error("YJBSV.GetOrderYJBInfoes服务失败，查询订单的易捷币抵用信息失败。 输入：OrderIds=" + JsonHelper.JsonSerializer(orderIds) + "，输出：" + result.Message);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.GetOrderYJBInfoes服务异常，查询订单的易捷币抵用信息异常。 输入：OrderIds=" + JsonHelper.JsonSerializer(orderIds), ex);
                result = ResultDTO<List<OrderYJBInfoDTO>>.Error(ex);
            }
            return result;
        }


        /// <summary>
        ///  查询订单项的易捷币抵用信息
        /// </summary>
        [BTPAopLogMethod]
        public ResultDTO<OrderItemYJBInfoDTO> GetOrderItemYJBInfo(Guid orderId)
        {
            LogHelper.Debug("YJBSV.GetOrderItemYJBInfo服务开始，输入：OrderId=" + orderId);
            ResultDTO<OrderItemYJBInfoDTO> result;
            try
            {
                var facade = new YJB.ISV.Facade.YJBJournalFacade();
                //facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                result = facade.GetOrderItemYJBInfo(orderId);
                if (!result.IsSuccess)
                {
                    if (result.Code != "OrderNotFound")
                        LogHelper.Error("YJBSV.GetOrderItemYJBInfo服务失败，查询订单的易捷币抵用信息失败。 输入：OrderId=" + orderId + "，输出：" + result.Message);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.OrderItemYJBInfoDTO服务异常，查询订单的易捷币抵用信息异常。 输入：OrderId=" + orderId, ex);
                result = ResultDTO<OrderItemYJBInfoDTO>.Error(ex);
            }
            return result;
        }

        /// <summary>
        ///  获取用户的易捷币信息
        /// </summary>
        [BTPAopLogMethod]
        public ResultDTO<UserYJBDTO> GetUserYJB(Guid userId)
        {
            ResultDTO<UserYJBDTO> result;
            try
            {
                var facade = new YJB.ISV.Facade.YJBInfoFacade();
                //facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                result = facade.GetUserYJB(userId);
                if (!result.IsSuccess)
                {
                    LogHelper.Error("YJBSV.GetUserYJB服务失败，获取用户的易捷币信息失败。 输入：UserId=" + userId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.GetUserYJB服务异常，获取用户的易捷币信息异常。 输入：UserId=" + userId, ex);
                result = ResultDTO<UserYJBDTO>.Error(ex);
            }
            return result;
        }

        /// <summary>
        ///  获取用户的易捷币流水信息
        /// </summary>
        [BTPAopLogMethod]
        public ListResultDTO<UserYJBJournalDTO> GetUserYJBJournal(OrderYJBInfoInputDTO input)
        {
            ListResultDTO<UserYJBJournalDTO> result;
            try
            {
                var facade = new YJB.ISV.Facade.YJBInfoFacade();
                //facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                result = facade.GetUserYJBJournal(input);
                if (!result.IsSuccess)
                {
                    LogHelper.Error("YJBSV.GetUserYJBJournal服务失败，获取用户的易捷币流水信息失败。 输入：" + JsonHelper.JsonSerializer(input));
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.GetUserYJBJournal服务异常，获取用户的易捷币流水信息异常。 输入：" + JsonHelper.JsonSerializer(input), ex);
                result = ListResultDTO<UserYJBJournalDTO>.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// 获取商品的油卡兑换券额度
        /// </summary>
        [BTPAopLogMethod]
        public List<CommodityYouKaDTO> GetCommodityYouKaPercent(GetYouKaPersentDTO input)
        {
            List<CommodityYouKaDTO> result = null;
            try
            {
                Jinher.AMP.YJB.ISV.Facade.CouponFacade facade = new YJB.ISV.Facade.CouponFacade();
                //facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                result = facade.GetCommodityYouKaPersent(input);
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.GetCommodityYouKaPersent服务异常，获取商品的油卡兑换券额度异常。 输入：" + JsonHelper.JsonSerializer(input), ex);
            }
            return result;
        }

        /// <summary>
        ///  获取易捷抵用券信息
        /// </summary>
        [BTPAopLogMethod]
        public ResultDTO<YJCouponInfoDto> GetYJCouponInfo(Guid couponId)
        {
            ResultDTO<YJCouponInfoDto> result;
            try
            {
                var facade = new YJB.ISV.Facade.YJBInfoFacade();
                //facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                result = facade.GetYJCouponInfo(new YJCouponInfoInput { Id = couponId });
                if (!result.IsSuccess)
                {
                    LogHelper.Error("YJBSV.GetUserYJB服务失败，获取用户的易捷币信息失败。 输入：couponId=" + couponId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.GetUserYJB服务异常，获取用户的易捷币信息异常。 输入：couponId=" + couponId, ex);
                result = ResultDTO<YJCouponInfoDto>.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// 根据类型获取易捷抵用券信息
        /// </summary>
        [BTPAopLogMethod]
        public ResultDTO<List<YJCouponInfoDto>> GetYJCouponInfoByType()
        {
            ResultDTO<List<YJCouponInfoDto>> result;
            try
            {
                var facade = new YJB.ISV.Facade.YJBInfoFacade();
                //facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                result = facade.GetYJCouponInfoByType(0);
                if (!result.IsSuccess)
                {
                    LogHelper.Error("YJBSV.GetYJCouponInfoByType服务失败，获取用户的易捷币信息失败。");
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.GetYJCouponInfoByType服务异常，获取用户的易捷币信息异常。", ex);
                result = ResultDTO<List<YJCouponInfoDto>>.Error(ex);
            }
            return result;
        }


        /// <summary>
        ///  获取易捷抵用券设置
        /// </summary>
        [BTPAopLogMethod]
        public ResultDTO<YJCouponSettingDto> GetYJCouponSetting(Guid appId)
        {
            ResultDTO<YJCouponSettingDto> result;
            try
            {
                var facade = new YJB.ISV.Facade.YJBInfoFacade();
                //facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                result = facade.GetYJCouponSetting(appId);
                if (!result.IsSuccess)
                {
                    LogHelper.Error("YJBSV.GetYJCouponSetting 获取易捷抵用券设置服务失败。 输入：appId=" + appId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.GetYJCouponSetting 获取易捷抵用券设置服务异常。 输入：appId=" + appId, ex);
                result = ResultDTO<YJCouponSettingDto>.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// 获取我的易捷抵现劵信息
        /// </summary>
        [BTPAopLogMethod]
        public ResultDTO<List<YJCouponWithCommodityInfo>> GetMyYJCouponInfoWithApp(YJCouponWithCommodityInfoInput input)
        {
            ResultDTO<List<YJCouponWithCommodityInfo>> result;
            try
            {
                //LogHelper.Error("YJBSV.GetMyYJCouponInfoWithApp, 获取我的易捷抵现劵信息。 输入：input=" + JsonHelper.JsonSerializer(input));
                var facade = new YJB.ISV.Facade.YJBInfoFacade();
                //facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                result = facade.GetMyYJCouponInfoWithApp(input);
                if (!result.IsSuccess)
                {
                    LogHelper.Error("YJBSV.GetMyYJCouponInfoWithApp服务失败，获取我的易捷抵现劵信息失败。 输入：input=" + JsonHelper.JsonSerializer(input));
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.GetMyYJCouponInfoWithApp服务异常，获取我的易捷抵现劵信息异常。 输入：input=" + JsonHelper.JsonSerializer(input), ex);
                result = ResultDTO<List<YJCouponWithCommodityInfo>>.Error(ex);
            }
            return result;
        }

        /// <summary>
        ///  查询订单项的易捷-抵用信息
        /// </summary>
        [BTPAopLogMethod]
        public ResultDTO<OrderItemYJInfoDTO> GetOrderItemYJInfo(Guid orderId)
        {
            LogHelper.Debug("YJBSV.GetOrderItemYJInfo服务开始，输入：OrderId=" + orderId);
            ResultDTO<OrderItemYJInfoDTO> result;
            try
            {
                var facade = new YJB.ISV.Facade.YJBJournalFacade();
                //facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                result = facade.GetOrderItemYJInfo(orderId);
                if (!result.IsSuccess)
                {
                    if (result.Code != "OrderNotFound")
                        LogHelper.Error("YJBSV.GetOrderItemYJInfo服务失败，查询订单的易捷-抵用信息失败。 输入：OrderId=" + orderId + "，输出：" + result.Message);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.GetOrderItemYJInfo服务异常，查询订单的易捷-抵用信息异常。 输入：OrderId=" + orderId, ex);
                result = ResultDTO<OrderItemYJInfoDTO>.Error(ex);
            }
            return result;
        }
        /// <summary>
        /// 获取严选商品定时改价商品ID
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ResultDTO<List<Jinher.AMP.YJB.Deploy.PriceChangeDetailDTO>> GetYXChangeComInfo(List<Guid> appId)
        {
            try
            {
                Jinher.AMP.YJB.ISV.Facade.ChangePriceFacade Facade = new YJB.ISV.Facade.ChangePriceFacade();
                return Facade.GetYXChangeComInfo(appId);
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.GetYXChangeComInfo服务异常，获取用户的易捷币信息异常。 输入：appids=" + JsonHelper.JsonSerializer(appId), ex);
                return new ResultDTO<List<Jinher.AMP.YJB.Deploy.PriceChangeDetailDTO>>();
            }
        }
        /// <summary>
        /// 获取定时改价商品信息
        /// </summary>
        /// <param name="Appids"></param>
        /// <param name="StarTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ResultDTO<List<Jinher.AMP.YJB.Deploy.PriceChangeDetailDTO>> GetAllChangeInfo(System.Collections.Generic.List<System.Guid> Appids, string StarTime, string EndTime)
        {
            try
            {
                Jinher.AMP.YJB.ISV.Facade.ChangePriceFacade Facade = new YJB.ISV.Facade.ChangePriceFacade();
                Facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                var result = Facade.GetAllChangeInfo(Appids, StarTime, EndTime);
                LogHelper.Info(string.Format("获取到定时改价商品条数:{0}", result.Data.Count));
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.GetAllChangeInfoExt获取定时改价商品信息", ex);
                return new ResultDTO<List<Jinher.AMP.YJB.Deploy.PriceChangeDetailDTO>>();
            }
        }
        /// <summary>
        /// 获取第三方订单数据
        /// </summary>
        [BTPAopLogMethod]
        public ListResultDTO<YJB.Deploy.YJBDSFOrderInfoDTO> GetDSFOrderInfo(YJBDSFOrderInfoSearchDTO input)
        {
            ListResultDTO<YJB.Deploy.YJBDSFOrderInfoDTO> result = null;
            try
            {
                var facade = new YJB.ISV.Facade.YJBDSFOrderInfoFacade();
                //facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                result = facade.GetDSFOrderInfo(input);
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.GetDSFOrderInfo，获取第三方订单数据服务异常。 输入：" + JsonHelper.JsonSerializer(input), ex);
            }
            if (result != null && result.IsSuccess)
            {
                return result;
            }
            else
            {
                LogHelper.Error("YJBSV.GetDSFOrderInfo服务失败，获取第三方订单数据失败。 输入：" + JsonHelper.JsonSerializer(input) + "，返回：" + JsonHelper.JsonSerializer(result));
                return null;
            }
        }

        /// <summary>
        /// 获取用户已领取易捷抵用券
        /// </summary>
        [BTPAopLogMethod]
        public ResultDTO<List<YJB.Deploy.CustomDTO.UserYJCouponJounalDTO>> GetUserYJCouponByOrderId(Guid orderId)
        {
            ResultDTO<List<YJB.Deploy.CustomDTO.UserYJCouponJounalDTO>> result = null;
            try
            {
                var facade = new YJB.ISV.Facade.UserYJCouponFacade();
                //facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                result = facade.GetUserYJCouponByOrderId(orderId);

            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.GetUserYJCouponByOrderId，获取用户已领取易捷抵用券失败。 输入：" + JsonHelper.JsonSerializer(orderId), ex);
            }
            if (result != null)
            {
                return result;
            }
            else
            {
                LogHelper.Error("YJBSV.GetUserYJCouponByOrderId服务失败，获取用户已领取易捷抵用券失败。 输入：" + JsonHelper.JsonSerializer(orderId) + "，返回：" + JsonHelper.JsonSerializer(result));
                return null;
            }
        }

        /// <summary>
        /// 获取用户已领取易捷抵用券(单品)
        /// </summary>
        [BTPAopLogMethod]
        public ResultDTO<List<YJB.Deploy.YJCouponItemJournalDTO>> GetUserYJCouponItemByOrderId(Guid orderItemId)
        {
            ResultDTO<List<YJB.Deploy.YJCouponItemJournalDTO>> result = null;
            try
            {
                var facade = new YJB.ISV.Facade.UserYJCouponFacade();
                //facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                result = facade.GetUserYJCouponItemByOrderId(orderItemId);
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.GetUserYJCouponItemByOrderId，获取用户已领取易捷抵用券(单品)失败。 输入：" + JsonHelper.JsonSerializer(orderItemId), ex);
            }
            if (result != null)
            {
                return result;
            }
            else
            {
                LogHelper.Error("YJBSV.GetUserYJCouponItemByOrderId服务失败，获取用户已领取易捷抵用券(单品)失败。 输入：" + JsonHelper.JsonSerializer(orderItemId) + "，返回：" + JsonHelper.JsonSerializer(result));
                return null;
            }
        }



        /// <summary>
        /// 
        /// </summary>
        [BTPAopLogMethod]
        public ResultDTO<List<Jinher.AMP.YJB.Deploy.CustomDTO.YJCBalanceResponseDTO>> YJCBalanceBatch(List<PayItem> yjcList)
        {
            ResultDTO<List<Jinher.AMP.YJB.Deploy.CustomDTO.YJCBalanceResponseDTO>> result = null;
            try
            {
                var facade = new Jinher.AMP.YJB.ISV.Facade.YJCardFacade();
                result = facade.YJCBalanceBatch(yjcList);
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.GetUserYJCouponItemByOrderId，获取用户已领取易捷抵用券(单品)失败。 输入：" + JsonHelper.JsonSerializer(yjcList), ex);
            }
            if (result != null)
            {
                return result;
            }
            else
            {
                LogHelper.Error("YJBSV.GetUserYJCouponItemByOrderId服务失败，获取用户已领取易捷抵用券(单品)失败。 输入：" + JsonHelper.JsonSerializer(yjcList) + "，返回：" + JsonHelper.JsonSerializer(result));
                return null;
            }
        }

        /// <summary>
        /// 下订单消费易捷卡
        /// </summary>
        /// <returns></returns>
        public ResultDTO ConsumeYjcBatch(ConsumeYjcBatchParamDTO dto)
        {
            var result = new ResultDTO();
            string pString = "";
            try
            {
                pString = JsonHelper.JsonSerializer(dto);
                var facade = new Jinher.AMP.YJB.ISV.Facade.YJCardFacade();
                result = facade.ConsumeYjcBatch(dto);
                return result;
            }
            catch (Exception ex)
            {
                result.Code = ((int)ReturnCodeEnum.ServiceException).ToString();
                result.Message = ReturnCodeEnum.ServiceException.GetDescription();
                LogHelper.Error("YJBSV.ConsumeYjcBatch，下订单消费易捷卡(单品)失败。 输入：" + JsonHelper.JsonSerializer(pString), ex);
            }
            return result;
        }

        /// <summary>
        /// 获取财务信息
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [BTPAopLogMethod]        
        public List<Jinher.AMP.YJB.Deploy.CustomDTO.MyCapitalDTO> GetMyCapitals(System.Guid appId, System.Guid userId)
        {
            var result = new List<Jinher.AMP.YJB.Deploy.CustomDTO.MyCapitalDTO>();
            try
            {
                var myFacade = new YJB.ISV.Facade.MyCapitalFacade();
                return myFacade.GetMyCapitals(appId, userId);
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.GetMyCapitals，获取财务信息失败。 输入：appId:" + appId + ",userid:" + userId, ex);
            }
            return null;
        }

        /// <summary>
        /// 获取积分信息
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Jinher.AMP.YJB.Deploy.CustomDTO.MyCapitalDTO> GetAllMyScore(System.Guid appId, System.Guid userId)
        {
            var result = new List<Jinher.AMP.YJB.Deploy.CustomDTO.MyCapitalDTO>();
            try
            {
                var myFacade = new YJB.ISV.Facade.MyCapitalFacade();
                return myFacade.GetMyAllScore(appId, userId);
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.GetMyCapitals，获取积分信息失败。 输入：appId:" + appId + ",userid:" + userId, ex);
            }
            return null;
        }


        ///// <summary>
        ///// 更新用户的易捷卡余额
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <returns></returns>
        //public ResultDTO UpdateYJCardBalance(Jinher.AMP.YJB.Deploy.CustomDTO.QueryUserYJCardDTO dto)
        //{
        //    var result = new ResultDTO();
        //    string pString = "";
        //    try
        //    {
        //        pString = JsonHelper.JsonSerializer(dto);
        //        var facade = new Jinher.AMP.YJB.ISV.Facade.YJCardFacade();
        //        result = facade.UpdateYJCardBalance(dto);
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Code = ((int)ReturnCodeEnum.ServiceException).ToString();
        //        result.Message = ReturnCodeEnum.ServiceException.GetDescription();
        //        LogHelper.Error("YJBSV.ConsumeYjcBatch，更新用户的易捷卡余额失败。 输入：" + JsonHelper.JsonSerializer(pString), ex);
        //    }
        //    return result;
        //}

    }
}
