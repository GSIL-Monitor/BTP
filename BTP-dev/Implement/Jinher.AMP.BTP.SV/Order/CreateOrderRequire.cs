using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jinher.AMP.App.Deploy;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.BE.BELogic;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.CBC.Deploy.CustomDTO;
using Jinher.AMP.ZPH.Deploy.CustomDTO;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;
using Attribute = System.Attribute;
using Jinher.AMP.BTP.Deploy.Enum;

namespace Jinher.AMP.BTP.SV.Order
{
    /// <summary>
    /// 订单需要
    /// </summary>
    public class CreateOrderRequire
    {
        private const string ShopCartBuyKey = "gouwuche";
        private static readonly List<string> FunctionCodes = new List<string>() { FunctionCodeConst.AppSelfTake, FunctionCodeConst.MyIntegral };

        /// <summary>
        /// 确认订单条件
        /// </summary>
        private Jinher.AMP.BTP.Deploy.CustomDTO.CreateOrderSearchDTO _condition;

        private List<Action> actionList = new List<Action>();
        private CreateOrderNeedDTO dto = new CreateOrderNeedDTO();
        private List<Guid> _appIds = new List<Guid>();
        private string _errorMessage = "";
        private bool _hasError = false;
        private Dictionary<string, bool> _funcionUsedDict = new Dictionary<string, bool>();

        //TODO

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        public CreateOrderRequire(CreateOrderSearchDTO condition)
        {
            _condition = condition;
        }
        /// <summary>
        /// 生成
        /// </summary>
        public void CreateRequires()
        {
            _hasError = initAndVilad();
            if (_hasError)
                return;
            _hasError = create();
        }
        /// <summary>
        /// 获取结果
        /// </summary>
        /// <returns></returns>
        public CreateOrderNeedDTO GetRequires()
        {
            return dto;
        }

        #region 私有方法
        private bool initAndVilad()
        {
            if (_condition == null || _condition.userId == Guid.Empty || _condition.esAppId == Guid.Empty || _condition.coms == null || !_condition.coms.Any())
            {
                _errorMessage = "参数有误，请重新加载";
                return false;
            }
            if (!initComs())
            {
                _errorMessage = "校验订单商品出错";
                return false;
            }
            initFunctionCodes();

            return true;
        }

        private bool initFunctionCodes()
        {
            _funcionUsedDict = BACSV.GetFunctionUsed(_condition.esAppId, FunctionCodes);

            //TODO  积分、自提、
            return true;
        }
        private bool initTradeType()
        {
            //是否直接到账
            int tradeType = Jinher.AMP.BTP.TPS.FSPSV.GetTradeSettingInfo(_condition.esAppId);
            if (tradeType == -1)
                return false;
            dto.TradeType = tradeType;
            return true;
        }
        private bool create()
        {
            var result = false;
            if (!initTradeType())
                return result;

            if (dto.TradeType == 0)
            {
                var balanceAction = new Action(GetBalance);
                actionList.Add(balanceAction);

                dto.IsAllAppInZPH = ZPHSV.Instance.CheckAllAppInZPH(_appIds);
                if (dto.IsAllAppInZPH)
                {
                    actionList.Add(getGoldCouponCount);
                }
            }

            actionList.Add(getDeliveryAddressDefault);


            actionList.Add(GetIsAllAppSupportCOD);

            return StartTasks(actionList);
        }

        /// <summary>
        /// 开始执行多个任务
        /// </summary>
        /// <param name="actionList">要执行的方法和对应的参数列表。</param>
        private bool StartTasks(List<Action> actionList)
        {
            //所有任务列表。
            List<Task> taskList = new List<Task>();
            TaskFactory taskFactory = new TaskFactory();
            foreach (var act in actionList)
            {
                Task task = taskFactory.StartNew(act);
                taskList.Add(task);
            }
            //等待所有任务完成返回。
            return Task.WaitAll(taskList.ToArray(), CustomConfig.RequestTimeOut);
        }

        /// <summary>
        ///  获取当前用户金币余额。
        /// </summary>
        private void GetBalance()
        {
            dto.GoldBalance = FSPSV.Instance.GetBalance(_condition.userId);
        }
        /// <summary>
        /// 获取当前用户代金券张数。
        /// </summary>
        /// <returns></returns>
        private void getGoldCouponCount()
        {
            dto.CouponCount = Jinher.AMP.BTP.TPS.PromotionSV.Instance.GetUsersVoucherCount(_condition.userId);
        }
        /// <summary>
        /// 获取用户下订单时显示的收货地址。
        /// </summary>
        private void getDeliveryAddressDefault()
        {
            DeliveryAddressSV sv = new DeliveryAddressSV();
            if (_condition.addressId != Guid.Empty)
            {
                dto.ShipmentInfo.DefaultAddressInfo = sv.GetDeliveryAddressByAddressIdExt(_condition.addressId, _condition.esAppId);
            }
            else
            {
                var addressList = sv.GetDeliveryAddressListExt(_condition.userId, _condition.esAppId, 1);
                if (addressList == null || addressList.Count == 0)
                {
                    return;
                }
                dto.ShipmentInfo.DefaultAddressInfo = addressList[0];
            }
        }
        /// <summary>
        /// App自提点信息
        /// </summary>
        private void getAppSelfTakeStationDefault()
        {
            //AppSelfTakeStationSV sv = new AppSelfTakeStationSV();
            //dto.AppSelfTakeStationDefaultInfo = sv.GetAppSelfTakeStationDefaultExt(new AppSelfTakeStationSearchDTO()
            //{
            //    EsAppId = _condition.esAppId,
            //    UserId = _condition.userId,
            //    Id = _condition.appSelfTakeStationId,
            //    SearchType = _condition.searchTypeForAppSelfTake
            //});
        }
        /// <summary>
        /// 获取用户在当前应用中的积分。
        /// </summary>
        public void initUserScore()
        {
            Param2DTO pdto = new Param2DTO();
            pdto.UserId = _condition.userId;
            pdto.AppId = _condition.esAppId;

            ScoreSettingSV sv = new ScoreSettingSV();
            ResultDTO<UserScoreDTO> usResult = sv.GetUserScoreInAppExt(pdto);
            if (usResult.ResultCode != 0)
            {
                return;
            }
            dto.UserScore = usResult.Data;
        }
        /// <summary>
        /// 是不是所有店铺app都支持“货到付款”。
        /// </summary>
        public void GetIsAllAppSupportCOD()
        {
            try
            {
                Jinher.AMP.BTP.ISV.Facade.PaymentsFacade pmFacade = new Jinher.AMP.BTP.ISV.Facade.PaymentsFacade();
                dto.IsAllAppSupportCOD = pmFacade.IsAllAppSupportCOD(_appIds).Data;
            }
            catch (Exception ex)
            {
                LogHelper.Error("GetIsAllAppSupportCOD异常，异常信息：", ex);
            }
        }
        #endregion

        public bool initComs()
        {
            bool hasError = false;

            var selecid = _condition.coms.Select(c => c.commodityId).Distinct().ToList();
            var cStockid = _condition.coms.Where(c => c.commodityStockId != Guid.Empty).Select(c => c.commodityStockId).Distinct().ToList();
            var promotionIds = _condition.coms.Select(c => c.promotionId != Guid.Empty).Distinct().ToList();
            var commodityList = (
                                 from c in Commodity.ObjectSet()
                                 join s in CommodityStock.ObjectSet()
                                 on c.Id equals s.CommodityId into os
                                 from ss in os.DefaultIfEmpty()
                                 where c.CommodityType == 0 && selecid.Contains(c.Id) && (ss == null || cStockid.Contains(ss.Id))
                                 select new CheckCommodityDTO
                                 {
                                     Id = c.Id,
                                     Price = ss != null ? ss.Price : c.Price,
                                     State = c.IsDel ? 3 : c.State,
                                     Stock = ss != null ? ss.Stock : c.Stock,
                                     Intensity = 10,
                                     DiscountPrice = -1,
                                     OPrice = c.Price,
                                     LimitBuyEach = -1,
                                     LimitBuyTotal = -1,
                                     SurplusLimitBuyTotal = 0,
                                     CommodityStockId = ss != null ? ss.Id : Guid.Empty,
                                     IsEnableSelfTake = c.IsEnableSelfTake,
                                     AppId = c.AppId
                                 }).ToList();
            var vipAppIds = (from condition in _condition.coms
                             join com in commodityList on condition.commodityId equals com.Id
                             where condition.promotionId == Guid.Empty
                             select com.AppId
                            ).Distinct().ToList();
            var vipDict = AVMSV.GetVipIntensities(vipAppIds, _condition.userId);
            List<TodayPromotionDTO> promotionDic = new List<TodayPromotionDTO>();
            //商品在每日促销表里集合 
            if (promotionIds.Any())
            {
                promotionDic = TodayPromotion.ObjectSet().Where(a => selecid.Contains(a.CommodityId)).
                   Select(a => new TodayPromotionDTO
                   {
                       CommodityId = a.CommodityId,
                       Intensity = a.Intensity,
                       DiscountPrice = a.DiscountPrice,
                       LimitBuyTotal = a.LimitBuyTotal ?? -1,
                       SurplusLimitBuyTotal = a.SurplusLimitBuyTotal,
                       LimitBuyEach = a.LimitBuyEach,
                       PromotionId = a.PromotionId,
                       PromotionType = a.PromotionType,
                       OutsideId = a.OutsideId,
                       GroupMinVolume = a.GroupMinVolume,
                       ExpireSecond = a.ExpireSecond,
                       StartTime = a.StartTime,
                       EndTime = a.EndTime
                   }).Distinct().ToList();
            }
            foreach (var createOrderCom in _condition.coms)
            {
                createOrderCom.checkResult = new CreateOrderComCheckResult();
                var com = commodityList.FirstOrDefault(c => c.Id == createOrderCom.commodityId && c.CommodityStockId == createOrderCom.commodityStockId);

                if (!checkComCanBuy(com, createOrderCom))
                {
                    hasError = true;
                    continue;
                }

                var realPrice = com.Price;
                TodayPromotionDTO promotion;
                //有活动参加活动
                if (createOrderCom.promotionId != Guid.Empty)
                {
                    promotion = promotionDic.FirstOrDefault(c => c.CommodityId == createOrderCom.commodityId && c.PromotionId == createOrderCom.promotionId);

                    if (!checkCanBuyByPromotion(promotion, createOrderCom))
                    {
                        hasError = true;
                        continue;
                    }

                    if (promotion != null)
                    {
                        realPrice = (promotion.DiscountPrice > -1) ? promotion.DiscountPrice.Value : decimal.Round((com.Price * promotion.Intensity / 10), 2, MidpointRounding.AwayFromZero);
                    }
                }
                //没有活动有vip机制，采用vip体系
                else if (vipDict.ContainsKey(com.AppId) && vipDict[com.AppId] != null && vipDict[com.AppId].IsVip)
                {
                    realPrice = (vipDict[com.AppId].DiscountPrice > -1) ? vipDict[com.AppId].DiscountPrice : decimal.Round((com.Price * vipDict[com.AppId].Intensity / 10), 2, MidpointRounding.AwayFromZero);
                }
                if (createOrderCom.realPrice != realPrice)
                {
                    createOrderCom.checkResult.FillData(true, false, 0, ComCantBuyReasonEnum.Price, "商品价格发生变化");
                    hasError = true;

                }
            }
            return !hasError;
        }

        private bool checkCanBuyByPromotion(TodayPromotionDTO promotion, CreateOrderCom createOrderCom)
        {
            if (promotion == null)
            {
                createOrderCom.checkResult.FillData(true, false, 0, ComCantBuyReasonEnum.NoPromotion, "活动已失效");
                return false;
            }
            if (promotion.StartTime > DateTime.Now)
            {
                createOrderCom.checkResult.FillData(true, false, 0, ComCantBuyReasonEnum.PromotionNotStart,
                                                    "活动即将开始");
                return false;
            }
            if (promotion.EndTime < DateTime.Now)
            {
                createOrderCom.checkResult.FillData(true, false, 0, ComCantBuyReasonEnum.PromotionEnded, "活动已结束");
                return false;
            }
            if (!PromotionCheck.CheckResource(_condition.userId, promotion, createOrderCom.number))
            {
                createOrderCom.checkResult.FillData(true, false, 0, ComCantBuyReasonEnum.Stock, "商品库存不足");
                return false;
            }

            return true;
        }

        private bool checkComCanBuy(CheckCommodityDTO com, CreateOrderCom createOrderCom)
        {
            if (com == null)
            {
                createOrderCom.checkResult.FillData(true, false, 0, ComCantBuyReasonEnum.NoCom, "无此商品");
                return false;
            }
            if (com.State == 3)
            {
                createOrderCom.checkResult.FillData(true, false, 0, ComCantBuyReasonEnum.Deled, "商品已失效");
                return false;
            }
            if (com.State == 1)
            {
                createOrderCom.checkResult.FillData(true, false, 0, ComCantBuyReasonEnum.State, "商品已下架");
                return false;
            }

            if (com.Stock < createOrderCom.number)
            {
                createOrderCom.checkResult.FillData(true, false, com.Stock, ComCantBuyReasonEnum.Stock, "商品库存不足");
                return false;
            }
            return true;
        }
    }


}
