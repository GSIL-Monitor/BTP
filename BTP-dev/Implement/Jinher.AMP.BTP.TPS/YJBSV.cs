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
 * ��ע�⣺������������������������������
 * �ⲿ�����뽫����д��TPS.XXXFacade�У��Զ�����չ��д��TPS.XXXSV��TPS.XXXBP��
 */
    /// <summary>
    /// �׽ݱ���ؽӿ�
    /// </summary>
    public class YJBSV : OutSideServiceBase<YJBFacade>
    {
        /// <summary>
        /// ��ȡ��Ʒ�ĵ��ֱ�����Ϣ
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
        /// ��ѯ�û����õ���Ʒ�ĵ��ֱ���
        /// </summary>
        public static CanInsteadCashDTO GetCommodityCashPercent(OrderInsteadCashInputDTO input)
        {
            var emptyResult = new CanInsteadCashDTO { YJBInfo = new OrderInsteadCashDTO { Enabled = false }, YJCouponInfo = new YJCouponCanInsteadCashDTO { YJCoupons = new List<MUserCouponDto>() } };
            if (input == null || input.Commodities == null || input.Commodities.Count == 0)
            {
                return emptyResult;
            }
            var result = Instance.GetCommodityCashPercent(input);
            LogHelper.Info("YJBSV.GetCommodityCashPercent�����룺" + JsonHelper.JsonSerializer(input) + "�������" + JsonHelper.JsonSerializer(result));
            if (result == null)
            {
                return emptyResult;
            }
            return result;
        }

        /// <summary>
        /// ��ѯ��Ʒ�ĵ��ֱ���
        /// </summary>
        public static Jinher.AMP.YJB.Deploy.CustomDTO.ListResultDTO<CommodityCashOutput> GetCommodityCashPercent(CommodityCashInput input)
        {
            return Instance.GetCommodityCashPercent(input);
        }

        /// <summary>
        /// ��������ʱ����¼�׽ݱ�������ˮ
        /// </summary>
        public static ResultDTO CreateOrderJournal(CreateOrderInputDTO input)
        {
            // Ĭ�϶����ȶ����׽ݱ�
            input.Freezed = true;
            return Instance.CreateOrderJournal(input);
        }

        /// <summary>
        ///  ֧���ɹ�ʱ��ȡ���׽ݱҶ���
        /// </summary>
        public static ResultDTO PayOrderJournal(Jinher.AMP.BTP.BE.CommodityOrder order)
        {
            return Instance.PayOrderJournal(order.Id);
        }

        /// <summary>
        ///  ȡ�����������˼�¼�׽ݱ�������ˮ
        /// </summary>
        public static ResultDTO CancelOrderJournal(Guid orderId)
        {
            return Instance.CancelOrderJournal(orderId);
        }

        /// <summary>
        ///  �˻�ʱ����¼�׽ݱҷ�����ˮ
        /// </summary>
        public static ResultDTO RefundOrderJournal(Guid orderId, decimal refundMoney, decimal commodityPrice, string UserMobile, Guid UserId, Guid useryucouponid)
        {
            return Instance.RefundOrderJournal(orderId, refundMoney, commodityPrice, UserMobile, UserId, useryucouponid);
        }

        /// <summary>
        ///  ��Ʒ�˻�ʱ����¼�׽ݱҷ�����ˮ
        /// </summary>
        public static ResultDTO RefundOrderItemJournal(Guid orderId, Guid commodityId, decimal refundMoney, Guid orderItemId, decimal commodityPrice, string UserMobile, Guid UserId, Guid useryucouponid)
        {
            return Instance.RefundOrderItemJournal(orderId, commodityId, refundMoney, orderItemId, commodityPrice, UserMobile, UserId, useryucouponid);
        }

        /// <summary>
        ///  ȫ���˻�ʱ����¼�׽ݱҷ�����ˮ
        /// </summary>
        public static ResultDTO RefundAllOrderJournal(Guid orderId)
        {
            return Instance.RefundAllOrderJournal(orderId);
        }
        /// <summary>
        /// �˿�ʱ�����׽ݿ����ѵ��ʽ��˻ص�����
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="Refund"></param>
        /// <returns></returns>
        public static ResultDTO UpdateCardCash(Guid UserId, Decimal Refund)
        {
            return Instance.UpdateCardCash(UserId, Refund);
        }

        /// <summary>
        /// �˻��׽ݿ��������˻���
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public static ResultDTO RetreatYjc(Guid UserId, Decimal Refund, Guid orderId, Guid orderItemId)
        {
            return Instance.RetreatYjc(UserId, Refund, orderId, orderItemId);
        }
        /// <summary>
        /// ���ݿ����˻�ȡ�׽ݿ���ֵ
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public static decimal GetYJCardAcount(Guid UserId)
        {
            return Instance.GetYJCardAcount(UserId);
        }

        /// <summary>
        ///  ��ѯ�������׽ݱҵ�����Ϣ
        /// </summary>
        public static ResultDTO<OrderYJBInfoDTO> GetOrderYJBInfo(Guid? esAppId, Guid orderId)
        {
            if (!esAppId.HasValue || esAppId != Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId)
            {
                return ResultDTO<OrderYJBInfoDTO>.Error("Disabled", "δ�����׽ݱ�");
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
        ///  ��ѯ�������׽ݵ�����Ϣ
        /// </summary>
        public static ResultDTO<YJOrderInfoDTO> GetOrderInfo(Guid? esAppId, Guid orderId)
        {
            if (!esAppId.HasValue || esAppId != Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId)
            {
                return ResultDTO<YJOrderInfoDTO>.Error("Disabled", "δ�����׽ݱ�");
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
        ///  ��ѯ�������׽ݱҵ�����Ϣ
        /// </summary>
        public static ResultDTO<List<OrderYJBInfoDTO>> GetOrderYJBInfoes(List<Guid> orderIds)
        {
            return Instance.GetOrderYJBInfoes(orderIds);
        }

        /// <summary>
        ///  ��ѯ�������׽ݱҵ�����Ϣ
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
        ///  ��ѯ�������׽ݱҵ�����Ϣ
        /// </summary>
        public static ResultDTO<List<YJOrderInfoDTO>> GetOrderInfoes(List<Guid> orderIds)
        {
            return Instance.GetOrderInfoes(orderIds);
        }

        /// <summary>
        ///  ��ѯ��������׽ݱҵ�����Ϣ
        /// </summary>
        public static ResultDTO<OrderItemYJBInfoDTO> GetOrderItemYJBInfo(Guid appId, Guid orderId)
        {
            LogHelper.Debug("��ѯ��������׽ݱҵ�����Ϣ��GetOrderItemYJBInfo��appId��" + appId + ",YJBConsts.YJAppId��" + YJBConsts.YJAppId);

            if (appId != YJB.Deploy.CustomDTO.YJBConsts.YJAppId)
            {
                return new ResultDTO<OrderItemYJBInfoDTO> { IsSuccess = false, Data = null };
            }
            return Instance.GetOrderItemYJBInfo(orderId);
        }


        /// <summary>
        ///  ��ȡ�û����׽ݱ���Ϣ
        /// </summary>
        public static ResultDTO<UserYJBDTO> GetUserYJB(Guid userId)
        {
            return Instance.GetUserYJB(userId);
        }

        /// <summary>
        ///  ��ȡ�û����׽ݱ���ˮ��Ϣ
        /// </summary>
        public static ListResultDTO<UserYJBJournalDTO> GetUserYJBJournal(OrderYJBInfoInputDTO input)
        {
            return Instance.GetUserYJBJournal(input);
        }

        /// <summary>
        /// ��ȡ��Ʒ���Ϳ��һ�ȯ���
        /// </summary>
        public static List<CommodityYouKaDTO> GetCommodityYouKaPercent(GetYouKaPersentDTO input)
        {
            return Instance.GetCommodityYouKaPercent(input);
        }

        /// <summary>
        ///  ��ȡ�׽ݵ���ȯ��Ϣ
        /// </summary>
        public static YJCouponInfoDto GetYJCouponInfo(Guid userId)
        {
            var result = Instance.GetYJCouponInfo(userId);
            if (result.IsSuccess) return result.Data;
            return null;
        }
        /// <summary>
        ///  �������ͻ�ȡ�׽ݵ���ȯ��Ϣ
        /// </summary>
        public static List<YJCouponInfoDto> GetYJCouponInfo()
        {
            var result = Instance.GetYJCouponInfoByType();
            if (result.IsSuccess) return result.Data;
            return null;
        }

        /// <summary>
        ///  ��ȡ�׽ݵ���ȯ��Ϣ
        /// </summary>
        public static YJCouponSettingDto GetYJCouponSetting(Guid appId)
        {
            var result = Instance.GetYJCouponSetting(appId);
            if (result.IsSuccess) return result.Data;
            return new YJCouponSettingDto { CanCombinabled = false, CanMultipuled = false };
        }


        /// <summary>
        /// ��ȡ�ҵ��׽ݵ��ք���Ϣ
        /// </summary>
        public static List<YJCouponWithCommodityInfo> GetMyYJCouponInfoWithApp(YJCouponWithCommodityInfoInput input)
        {
            input.EsAppId = YJBConsts.YJAppId;
            var result = Instance.GetMyYJCouponInfoWithApp(input);
            if (result.IsSuccess) return result.Data;
            return null;
        }

        /// <summary>
        ///  ��ѯ��������׽�-������Ϣ
        /// </summary>
        public static ResultDTO<OrderItemYJInfoDTO> GetOrderItemYJInfo(Guid appId, Guid orderId)
        {
            LogHelper.Debug("��ѯ��������׽ݱҵ�����Ϣ��GetOrderItemYJInfo��appId��" + appId + ",YJBConsts.YJAppId��" + YJBConsts.YJAppId);

            if (appId != YJB.Deploy.CustomDTO.YJBConsts.YJAppId && appId != new Guid("1375ad99-de3b-4e93-80d5-5b96e1588967"))
            {
                return new ResultDTO<OrderItemYJInfoDTO> { IsSuccess = false, Data = null };
            }
            return Instance.GetOrderItemYJInfo(orderId);
        }
        /// <summary>
        /// ��ȡ��ѡ��Ʒ��ʱ�ļ���ƷID
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
        /// ��ʱ�ļ���Ʒ��Ϣ
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
        /// �¶��������׽ݿ�
        /// </summary>
        /// <returns></returns>
        public static ResultDTO ConsumeYjcBatch(ConsumeYjcBatchParamDTO dto)
        {
            return Instance.ConsumeYjcBatch(dto);
        }

        /// <summary>
        /// ��ȡ���ּ�������Ϣ
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<Jinher.AMP.YJB.Deploy.CustomDTO.MyCapitalDTO> GetMyCapitals(System.Guid appId, System.Guid userId)
        {
            return Instance.GetMyCapitals(appId, userId);
        }    

        /// <summary>
        /// ��ȡ������Ϣ
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
        /// ��ȡ��Ʒ�ĵ��ֱ�����Ϣ
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
                LogHelper.Error("YJBSV.GetCommodityCashPercentWithoutUser�����쳣����ȡ��Ʒ�ĵ��ֱ�����Ϣ�쳣�� ���룺" + JsonHelper.JsonSerializer(commodities), ex);
            }
            if (result != null && result.IsSuccess)
            {
                return result.Data;
            }
            else
            {
                LogHelper.Error("YJBSV.GetCommodityCashPercentWithoutUser����ʧ�ܣ���ȡ��Ʒ�ĵ��ֱ�����Ϣʧ�ܡ� ���룺" + JsonHelper.JsonSerializer(commodities) + "�����أ�" + JsonHelper.JsonSerializer(result));
                return null;
            }
        }

        /// <summary>
        /// ��ȡ��Ʒ�ĵ��ֱ�����Ϣ
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
                LogHelper.Error("YJBSV.GetCommodityCashPercent�����쳣����ȡ��Ʒ�ĵ��ֱ�����Ϣ�쳣�� ���룺" + JsonHelper.JsonSerializer(input), ex);
            }
            if (result != null && result.IsSuccess)
            {
                return result.Data;
            }
            else
            {
                LogHelper.Error("YJBSV.GetCommodityCashPercent����ʧ�ܣ���ȡ��Ʒ�ĵ��ֱ�����Ϣʧ�ܡ� ���룺" + JsonHelper.JsonSerializer(input) + "�����أ�" + JsonHelper.JsonSerializer(result));
                return null;
            }
        }

        /// <summary>
        /// ��ȡ��Ʒ�ĵ��ֱ�����Ϣ
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
                LogHelper.Error("YJBSV.GetCommodityCashPercent�����쳣����ȡ��Ʒ�ĵ��ֱ�����Ϣ�쳣�� ���룺" + JsonHelper.JsonSerializer(input), ex);
            }
            if (result != null && result.IsSuccess)
            {
                return result;
            }
            else
            {
                LogHelper.Error("YJBSV.GetCommodityCashPercent����ʧ�ܣ���ȡ��Ʒ�ĵ��ֱ�����Ϣʧ�ܡ� ���룺" + JsonHelper.JsonSerializer(input) + "�����أ�" + JsonHelper.JsonSerializer(result));
                return null;
            }
        }

        /// <summary>
        /// ��������ʱ����¼�׽ݱ�������ˮ
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
                    LogHelper.Error("YJBSV.CreateOrderJournal����ʧ�ܣ���¼�׽ݱ�������ˮʧ�ܡ� ���룺" + JsonHelper.JsonSerializer(input) + " �������" + result.Message);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.CreateOrderJournal�����쳣����¼�׽ݱ�������ˮ�쳣�� ���룺" + JsonHelper.JsonSerializer(input), ex);
                result = ResultDTO.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// ֧���ɹ�ʱ��ȡ���׽ݱҶ���
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
                    LogHelper.Error("YJBSV.PayOrderJournal����ʧ�ܣ�ȡ���׽ݱҶ���ʧ�ܡ� ���룺OrderId=" + orderId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.PayOrderJournal�����쳣��ȡ���׽ݱҶ����쳣�� ���룺OrderId=" + orderId, ex);
                result = ResultDTO.Error(ex);
            }
            return result;
        }

        /// <summary>
        ///  ȡ�����������˼�¼�׽ݱ�������ˮ
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
                //        LogHelper.Error("YJBSV.CancelOrderJournal����ʧ�ܣ����˼�¼�׽ݱ�������ˮʧ�ܡ� ���룺OrderId=" + orderId + "�������" + result.Message);
                //}
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.CancelOrderJournal�����쳣�����˼�¼�׽ݱ�������ˮ�쳣�� ���룺OrderId=" + orderId, ex);
                result = ResultDTO.Error(ex);
            }
            return result;
        }

        /// <summary>
        ///  �˻�ʱ����¼�׽ݱҷ�����ˮ
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
                        LogHelper.Error("YJBSV.RefundOrderJournal����ʧ�ܣ���¼�׽ݱҷ�����ˮʧ�ܡ� ���룺OrderId=" + orderId + "�������" + result.Message);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.RefundOrderJournal�����쳣����¼�׽ݱҷ�����ˮ�쳣�� ���룺OrderId=" + orderId, ex);
                result = ResultDTO.Error(ex);
            }
            return result;
        }
        /// <summary>
        /// �˿�ʱ�����׽ݿ��˿�
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
                    LogHelper.Error("YJBSV.UpdateCardCash����ʧ�ܣ��˻��׽ݿ�������ʧ�ܡ� ���룺UserId=" + UserId + "�������" + result.Message);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.UpdateCardCash�����쳣����¼�׽ݱҷ�����ˮ�쳣�� ���룺UserId=" + UserId, ex);
                result = ResultDTO.Error(ex);
                throw;
            }
            return result;
        }
        /// <summary>
        /// �˻��׽ݿ��������˻���
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
                    LogHelper.Error("YJBSV.RetreatYjc���˻��׽ݿ�������ʧ�ܡ� ���룺UserId=" + UserId + "�������" + result.Message);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.UpdateCardCash�����쳣����¼�׽ݱҷ�����ˮ�쳣�� ���룺UserId=" + UserId, ex);
                result = ResultDTO.Error(ex);
                throw;
            }
            return result;
        }

        /// <summary>
        /// ���ݿ����˻�ȡ�׽ݿ���ֵ
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
                    LogHelper.Error("YJBSV.GetYJCardAcount���˻��׽ݿ�������ʧ�ܡ� ���룺UserId=" + UserId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.GetYJCardAcount�����쳣����¼�׽ݱҷ�����ˮ�쳣�� ���룺UserId=" + UserId, ex);
                throw;
            }
            return result;
        }

        /// <summary>
        ///  �˻�ʱ����¼�׽ݱҷ�����ˮ
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
                        LogHelper.Error("YJBSV.RefundAllOrderJournal����ʧ�ܣ���¼�׽ݱҷ�����ˮʧ�ܡ� ���룺OrderId=" + orderId + "�������" + result.Message);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.RefundAllOrderJournal�����쳣����¼�׽ݱҷ�����ˮ�쳣�� ���룺OrderId=" + orderId, ex);
                result = ResultDTO.Error(ex);
            }
            return result;
        }

        /// <summary>
        ///  ��Ʒ�˻�ʱ����¼�׽ݱҷ�����ˮ
        /// </summary>
        [BTPAopLogMethod]
        public ResultDTO RefundOrderItemJournal(Guid orderId, Guid commodityId, decimal refundMoney, Guid orderItemId, decimal commodityPrice, string UserMobile, Guid UserId, Guid useryjcouponid)
        {
            LogHelper.Debug("��Ʒ�˻�ʱ����¼�׽ݱҷ�����ˮ:orderId :" + orderId + ",commodityId:" + commodityId + ",refundMoney:" + refundMoney + ",orderItemId:" + orderItemId + ",commodityPrice :" + commodityPrice + ",UserMobile:" + UserMobile + ",UserId:" + UserId);
            ResultDTO result;
            try
            {
                var facade = new YJB.ISV.Facade.YJBJournalFacade();
                //facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                result = facade.RefundOrderItemJournal(new CancelOrderItemDTO { AppKey = YJBConsts.YJAppKey, OrderId = orderId, CommodityId = commodityId, RefundMoney = refundMoney, OrderItemId = orderItemId, CommodityPrice = commodityPrice, UserYJCouponId = useryjcouponid, userMobile = UserMobile, userID = UserId });
                LogHelper.Debug("��Ʒ�˻�ʱ����¼�׽ݱҷ�����ˮ:result :" + JsonHelper.JsSerializer(result));
                if (!result.IsSuccess)
                {
                    if (result.Code != "OrderNotFound")
                        LogHelper.Error("YJBSV.RefundOrderItemJournal����ʧ�ܣ���¼�׽ݱҷ�����ˮʧ�ܡ� ���룺OrderId=" + orderId + " CommodityID=" + commodityId + "�������" + result.Message);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.RefundOrderItemJournal�����쳣����¼�׽ݱҷ�����ˮ�쳣�� ���룺OrderId=" + orderId + " CommodityID=" + commodityId, ex);
                result = ResultDTO.Error(ex);
            }
            return result;
        }


        /// <summary>
        ///  ��ѯ�������׽ݵ�����Ϣ
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
                        LogHelper.Error("YJBSV.GetOrderInfo����ʧ�ܣ���ѯ�������׽ݵ�����Ϣʧ�ܡ� ���룺OrderId=" + orderId + "�������" + result.Message);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.GetOrderInfo�����쳣����ѯ�������׽ݵ�����Ϣ�쳣�� ���룺OrderId=" + orderId, ex);
                result = ResultDTO<YJOrderInfoDTO>.Error(ex);
            }
            return result;
        }

        /// <summary>
        ///  ��ѯ�������׽ݱҵ�����Ϣ
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
                        LogHelper.Error("YJBSV.GetOrderYJBInfo����ʧ�ܣ���ѯ�������׽ݱҵ�����Ϣʧ�ܡ� ���룺OrderId=" + orderId + "�������" + result.Message);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.GetOrderYJBInfo�����쳣����ѯ�������׽ݱҵ�����Ϣ�쳣�� ���룺OrderId=" + orderId, ex);
                result = ResultDTO<OrderYJBInfoDTO>.Error(ex);
            }
            return result;
        }

        /// <summary>
        ///  ��ѯ����������׽ݵ�����Ϣ
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
                        LogHelper.Error("YJBSV.GetOrderInfoes����ʧ�ܣ���ѯ�������׽ݵ�����Ϣʧ�ܡ� ���룺OrderIds=" + JsonHelper.JsonSerializer(orderIds) + "�������" + result.Message);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.GetOrderInfoes�����쳣����ѯ�������׽ݵ�����Ϣ�쳣�� ���룺OrderIds=" + JsonHelper.JsonSerializer(orderIds), ex);
                result = ResultDTO<List<YJOrderInfoDTO>>.Error(ex);
            }
            return result;
        }

        /// <summary>
        ///  ��ѯ����������׽ݱҵ�����Ϣ
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
                        LogHelper.Error("YJBSV.GetOrderYJBInfoes����ʧ�ܣ���ѯ�������׽ݱҵ�����Ϣʧ�ܡ� ���룺OrderIds=" + JsonHelper.JsonSerializer(orderIds) + "�������" + result.Message);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.GetOrderYJBInfoes�����쳣����ѯ�������׽ݱҵ�����Ϣ�쳣�� ���룺OrderIds=" + JsonHelper.JsonSerializer(orderIds), ex);
                result = ResultDTO<List<OrderYJBInfoDTO>>.Error(ex);
            }
            return result;
        }


        /// <summary>
        ///  ��ѯ��������׽ݱҵ�����Ϣ
        /// </summary>
        [BTPAopLogMethod]
        public ResultDTO<OrderItemYJBInfoDTO> GetOrderItemYJBInfo(Guid orderId)
        {
            LogHelper.Debug("YJBSV.GetOrderItemYJBInfo����ʼ�����룺OrderId=" + orderId);
            ResultDTO<OrderItemYJBInfoDTO> result;
            try
            {
                var facade = new YJB.ISV.Facade.YJBJournalFacade();
                //facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                result = facade.GetOrderItemYJBInfo(orderId);
                if (!result.IsSuccess)
                {
                    if (result.Code != "OrderNotFound")
                        LogHelper.Error("YJBSV.GetOrderItemYJBInfo����ʧ�ܣ���ѯ�������׽ݱҵ�����Ϣʧ�ܡ� ���룺OrderId=" + orderId + "�������" + result.Message);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.OrderItemYJBInfoDTO�����쳣����ѯ�������׽ݱҵ�����Ϣ�쳣�� ���룺OrderId=" + orderId, ex);
                result = ResultDTO<OrderItemYJBInfoDTO>.Error(ex);
            }
            return result;
        }

        /// <summary>
        ///  ��ȡ�û����׽ݱ���Ϣ
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
                    LogHelper.Error("YJBSV.GetUserYJB����ʧ�ܣ���ȡ�û����׽ݱ���Ϣʧ�ܡ� ���룺UserId=" + userId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.GetUserYJB�����쳣����ȡ�û����׽ݱ���Ϣ�쳣�� ���룺UserId=" + userId, ex);
                result = ResultDTO<UserYJBDTO>.Error(ex);
            }
            return result;
        }

        /// <summary>
        ///  ��ȡ�û����׽ݱ���ˮ��Ϣ
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
                    LogHelper.Error("YJBSV.GetUserYJBJournal����ʧ�ܣ���ȡ�û����׽ݱ���ˮ��Ϣʧ�ܡ� ���룺" + JsonHelper.JsonSerializer(input));
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.GetUserYJBJournal�����쳣����ȡ�û����׽ݱ���ˮ��Ϣ�쳣�� ���룺" + JsonHelper.JsonSerializer(input), ex);
                result = ListResultDTO<UserYJBJournalDTO>.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// ��ȡ��Ʒ���Ϳ��һ�ȯ���
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
                LogHelper.Error("YJBSV.GetCommodityYouKaPersent�����쳣����ȡ��Ʒ���Ϳ��һ�ȯ����쳣�� ���룺" + JsonHelper.JsonSerializer(input), ex);
            }
            return result;
        }

        /// <summary>
        ///  ��ȡ�׽ݵ���ȯ��Ϣ
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
                    LogHelper.Error("YJBSV.GetUserYJB����ʧ�ܣ���ȡ�û����׽ݱ���Ϣʧ�ܡ� ���룺couponId=" + couponId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.GetUserYJB�����쳣����ȡ�û����׽ݱ���Ϣ�쳣�� ���룺couponId=" + couponId, ex);
                result = ResultDTO<YJCouponInfoDto>.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// �������ͻ�ȡ�׽ݵ���ȯ��Ϣ
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
                    LogHelper.Error("YJBSV.GetYJCouponInfoByType����ʧ�ܣ���ȡ�û����׽ݱ���Ϣʧ�ܡ�");
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.GetYJCouponInfoByType�����쳣����ȡ�û����׽ݱ���Ϣ�쳣��", ex);
                result = ResultDTO<List<YJCouponInfoDto>>.Error(ex);
            }
            return result;
        }


        /// <summary>
        ///  ��ȡ�׽ݵ���ȯ����
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
                    LogHelper.Error("YJBSV.GetYJCouponSetting ��ȡ�׽ݵ���ȯ���÷���ʧ�ܡ� ���룺appId=" + appId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.GetYJCouponSetting ��ȡ�׽ݵ���ȯ���÷����쳣�� ���룺appId=" + appId, ex);
                result = ResultDTO<YJCouponSettingDto>.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// ��ȡ�ҵ��׽ݵ��ք���Ϣ
        /// </summary>
        [BTPAopLogMethod]
        public ResultDTO<List<YJCouponWithCommodityInfo>> GetMyYJCouponInfoWithApp(YJCouponWithCommodityInfoInput input)
        {
            ResultDTO<List<YJCouponWithCommodityInfo>> result;
            try
            {
                //LogHelper.Error("YJBSV.GetMyYJCouponInfoWithApp, ��ȡ�ҵ��׽ݵ��ք���Ϣ�� ���룺input=" + JsonHelper.JsonSerializer(input));
                var facade = new YJB.ISV.Facade.YJBInfoFacade();
                //facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                result = facade.GetMyYJCouponInfoWithApp(input);
                if (!result.IsSuccess)
                {
                    LogHelper.Error("YJBSV.GetMyYJCouponInfoWithApp����ʧ�ܣ���ȡ�ҵ��׽ݵ��ք���Ϣʧ�ܡ� ���룺input=" + JsonHelper.JsonSerializer(input));
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.GetMyYJCouponInfoWithApp�����쳣����ȡ�ҵ��׽ݵ��ք���Ϣ�쳣�� ���룺input=" + JsonHelper.JsonSerializer(input), ex);
                result = ResultDTO<List<YJCouponWithCommodityInfo>>.Error(ex);
            }
            return result;
        }

        /// <summary>
        ///  ��ѯ��������׽�-������Ϣ
        /// </summary>
        [BTPAopLogMethod]
        public ResultDTO<OrderItemYJInfoDTO> GetOrderItemYJInfo(Guid orderId)
        {
            LogHelper.Debug("YJBSV.GetOrderItemYJInfo����ʼ�����룺OrderId=" + orderId);
            ResultDTO<OrderItemYJInfoDTO> result;
            try
            {
                var facade = new YJB.ISV.Facade.YJBJournalFacade();
                //facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                result = facade.GetOrderItemYJInfo(orderId);
                if (!result.IsSuccess)
                {
                    if (result.Code != "OrderNotFound")
                        LogHelper.Error("YJBSV.GetOrderItemYJInfo����ʧ�ܣ���ѯ�������׽�-������Ϣʧ�ܡ� ���룺OrderId=" + orderId + "�������" + result.Message);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.GetOrderItemYJInfo�����쳣����ѯ�������׽�-������Ϣ�쳣�� ���룺OrderId=" + orderId, ex);
                result = ResultDTO<OrderItemYJInfoDTO>.Error(ex);
            }
            return result;
        }
        /// <summary>
        /// ��ȡ��ѡ��Ʒ��ʱ�ļ���ƷID
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
                LogHelper.Error("YJBSV.GetYXChangeComInfo�����쳣����ȡ�û����׽ݱ���Ϣ�쳣�� ���룺appids=" + JsonHelper.JsonSerializer(appId), ex);
                return new ResultDTO<List<Jinher.AMP.YJB.Deploy.PriceChangeDetailDTO>>();
            }
        }
        /// <summary>
        /// ��ȡ��ʱ�ļ���Ʒ��Ϣ
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
                LogHelper.Info(string.Format("��ȡ����ʱ�ļ���Ʒ����:{0}", result.Data.Count));
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJBSV.GetAllChangeInfoExt��ȡ��ʱ�ļ���Ʒ��Ϣ", ex);
                return new ResultDTO<List<Jinher.AMP.YJB.Deploy.PriceChangeDetailDTO>>();
            }
        }
        /// <summary>
        /// ��ȡ��������������
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
                LogHelper.Error("YJBSV.GetDSFOrderInfo����ȡ�������������ݷ����쳣�� ���룺" + JsonHelper.JsonSerializer(input), ex);
            }
            if (result != null && result.IsSuccess)
            {
                return result;
            }
            else
            {
                LogHelper.Error("YJBSV.GetDSFOrderInfo����ʧ�ܣ���ȡ��������������ʧ�ܡ� ���룺" + JsonHelper.JsonSerializer(input) + "�����أ�" + JsonHelper.JsonSerializer(result));
                return null;
            }
        }

        /// <summary>
        /// ��ȡ�û�����ȡ�׽ݵ���ȯ
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
                LogHelper.Error("YJBSV.GetUserYJCouponByOrderId����ȡ�û�����ȡ�׽ݵ���ȯʧ�ܡ� ���룺" + JsonHelper.JsonSerializer(orderId), ex);
            }
            if (result != null)
            {
                return result;
            }
            else
            {
                LogHelper.Error("YJBSV.GetUserYJCouponByOrderId����ʧ�ܣ���ȡ�û�����ȡ�׽ݵ���ȯʧ�ܡ� ���룺" + JsonHelper.JsonSerializer(orderId) + "�����أ�" + JsonHelper.JsonSerializer(result));
                return null;
            }
        }

        /// <summary>
        /// ��ȡ�û�����ȡ�׽ݵ���ȯ(��Ʒ)
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
                LogHelper.Error("YJBSV.GetUserYJCouponItemByOrderId����ȡ�û�����ȡ�׽ݵ���ȯ(��Ʒ)ʧ�ܡ� ���룺" + JsonHelper.JsonSerializer(orderItemId), ex);
            }
            if (result != null)
            {
                return result;
            }
            else
            {
                LogHelper.Error("YJBSV.GetUserYJCouponItemByOrderId����ʧ�ܣ���ȡ�û�����ȡ�׽ݵ���ȯ(��Ʒ)ʧ�ܡ� ���룺" + JsonHelper.JsonSerializer(orderItemId) + "�����أ�" + JsonHelper.JsonSerializer(result));
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
                LogHelper.Error("YJBSV.GetUserYJCouponItemByOrderId����ȡ�û�����ȡ�׽ݵ���ȯ(��Ʒ)ʧ�ܡ� ���룺" + JsonHelper.JsonSerializer(yjcList), ex);
            }
            if (result != null)
            {
                return result;
            }
            else
            {
                LogHelper.Error("YJBSV.GetUserYJCouponItemByOrderId����ʧ�ܣ���ȡ�û�����ȡ�׽ݵ���ȯ(��Ʒ)ʧ�ܡ� ���룺" + JsonHelper.JsonSerializer(yjcList) + "�����أ�" + JsonHelper.JsonSerializer(result));
                return null;
            }
        }

        /// <summary>
        /// �¶��������׽ݿ�
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
                LogHelper.Error("YJBSV.ConsumeYjcBatch���¶��������׽ݿ�(��Ʒ)ʧ�ܡ� ���룺" + JsonHelper.JsonSerializer(pString), ex);
            }
            return result;
        }

        /// <summary>
        /// ��ȡ������Ϣ
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
                LogHelper.Error("YJBSV.GetMyCapitals����ȡ������Ϣʧ�ܡ� ���룺appId:" + appId + ",userid:" + userId, ex);
            }
            return null;
        }

        /// <summary>
        /// ��ȡ������Ϣ
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
                LogHelper.Error("YJBSV.GetMyCapitals����ȡ������Ϣʧ�ܡ� ���룺appId:" + appId + ",userid:" + userId, ex);
            }
            return null;
        }


        ///// <summary>
        ///// �����û����׽ݿ����
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
        //        LogHelper.Error("YJBSV.ConsumeYjcBatch�������û����׽ݿ����ʧ�ܡ� ���룺" + JsonHelper.JsonSerializer(pString), ex);
        //    }
        //    return result;
        //}

    }
}
