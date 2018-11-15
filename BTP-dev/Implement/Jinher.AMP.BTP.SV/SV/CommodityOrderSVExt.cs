using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Jinher.AMP.App.Deploy.CustomDTO;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.BE.BELogic;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.Game.IBP.Facade;
using System.Text;
using Jinher.JAP.Cache;
using Jinher.AMP.FSP.ISV.Facade;
using Jinher.AMP.FSP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.CBC.Deploy.CustomDTO;
using System.Transactions;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.AMP.BTP.TPS;
using CommodityStockDTO = Jinher.AMP.BTP.Deploy.CommodityStockDTO;
using CouponNewDTO = Jinher.AMP.Coupon.Deploy.CustomDTO.CouponNewDTO;
using System.Web;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.TPS.Helper;
using Jinher.AMP.Coupon.Deploy.Enum;
using Newtonsoft.Json.Linq;
using Jinher.AMP.ZPH.Deploy.CustomDTO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Jinher.AMP.BTP.Common.Extensions;

namespace Jinher.AMP.BTP.SV
{
    /// <summary>
    /// 订单接口类
    /// </summary>
    public partial class CommodityOrderSV : BaseSv, ICommodityOrder
    {
        //private static Object submitOrderLock = new Object();
        //private static Object comfirmPayLock = new Object();
        private const string PickUpCodePre = "BTP_PickUpCode:";
        /// <summary>
        /// 生成订单
        /// </summary>
        /// <param name="orderSDTO">订单实体</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.OrderResultDTO SaveCommodityOrderExt(Jinher.AMP.BTP.Deploy.CustomDTO.OrderSDTO orderSDTO)
        {
            LogHelper.Info(string.Format("下订单SaveCommodityOrderExt：当前时间={0},orderSDTO={1}", DateTime.Now, JsonHelper.JsonSerializer(orderSDTO)), "BTP_Order");
            Tuple<Guid, decimal> appIdRealPrice = null;
            if (orderSDTO == null)
                return new OrderResultDTO { ResultCode = 1, Message = "参数不能为空" };

            if (string.IsNullOrEmpty(orderSDTO.ReceiptUserName) || string.IsNullOrEmpty(orderSDTO.ReceiptPhone))
            {
                var tuple = CBCSV.GetUserNameAndCode(orderSDTO.UserId);
                orderSDTO.ReceiptUserName = tuple.Item1;
                orderSDTO.ReceiptPhone = tuple.Item2;
            }
            if (orderSDTO.mealBoxFee.HasValue == false)
                orderSDTO.mealBoxFee = 0;
            if (orderSDTO.deliveryFeeDiscount.HasValue == false)
                orderSDTO.deliveryFeeDiscount = 0;

            //只有代运营的才可用优惠券与代金券
            if (CustomConfig.CouponFlag == 1)
            {
                if (orderSDTO.CouponId != Guid.Empty || !(string.IsNullOrWhiteSpace(orderSDTO.PaycouponCodes) || orderSDTO.PaycouponCodes == "null"))
                {
                    if (!Jinher.AMP.BTP.TPS.ZPHSV.Instance.CheckIsAppInZPH(orderSDTO.AppId))
                    {
                        return new OrderResultDTO { ResultCode = 1, Message = "只有代运营的商品才能使用优惠券与代金券" };
                    }
                }
            }
            Jinher.AMP.YJB.Deploy.CustomDTO.OrderInsteadCashDTO yjbInfo = null;
            if (orderSDTO.YJBPrice > 0)
            {
                orderSDTO.RealPrice -= orderSDTO.YJBPrice;
                var yjInfo = YJBHelper.GetCommodityCashPercent(orderSDTO.EsAppId, new Jinher.AMP.YJB.Deploy.CustomDTO.OrderInsteadCashInputDTO
                {
                    UserId = orderSDTO.UserId,
                    YJCouponIds = orderSDTO.YJCouponIds,
                    Commodities = orderSDTO.ShoppingCartItemSDTO.Select(s => new
                    Jinher.AMP.YJB.Deploy.CustomDTO.OrderInsteadCashInputCommodityDTO
                    {
                        AppId = orderSDTO.AppId,
                        Id = s.Id,
                        Price = s.RealPrice,
                        Number = s.CommodityNumber,
                    }).ToList()
                });
                yjbInfo = yjInfo.YJBInfo;
            }
            if (orderSDTO.YJCouponPrice > 0)
            {
                orderSDTO.RealPrice -= orderSDTO.YJCouponPrice;
            }

            if (orderSDTO.YjCardPrice > 0)
            {
                orderSDTO.RealPrice -= orderSDTO.YjCardPrice;
            }

            SplitOrderYjCard(orderSDTO);

            orderSDTO.MainOrderId = Guid.Empty;
            var result = SubmitOrderCommon(orderSDTO, ref appIdRealPrice, false, false, yjbInfo);

            var json = SerializationHelper.JsonSerialize(new { OrderId = (result != null ? result.OrderId : Guid.Empty), Request = orderSDTO, Response = result });
            RabbitMqHelper.Send(RabbitMqRoutingKey.OrderSingleCreateEnd, RabbitMqExchange.Order, json);
            //删除缓存
            JdOrderHelper.DeleteRedisJdPOrder(orderSDTO.UserId.ToString());
            return result;
        }

        #region 下订单
        private void saveOrderPickUp(CommodityOrder order, AppSelfTakeStation appSelfTakeStation, AppOrderPickUpInfoDTO pickUpInfo)
        {
            if (order == null || appSelfTakeStation == null || order.SelfTakeFlag != 1)
            {
                return;
            }
            var pickUpCodeLong = RedisHelper.Incr(RedisKeyConst.PickUpCode, new Random().Next(1, 100));
            string pickUpCode = pickUpCodeLong.ToString(CultureInfo.InvariantCulture);
            string codepath = string.Empty;
            bool genQRCodeError = false;
            try
            {
                //int i = 0, k = 1;
                //int n = k / i;              
                codepath = Jinher.AMP.BTP.TPS.BaseAppToolsSV.Instance.GenQRCode(PickUpCodePre + pickUpCode);
                if (string.IsNullOrEmpty(codepath))
                {
                    LogHelper.Error(string.Format("自提订单生成二维码失败。订单Id：{0} 入参：{1},出参：{2}", order.Id, PickUpCodePre + pickUpCode, codepath));
                    genQRCodeError = true;
                }
                else
                {
                    codepath = Jinher.AMP.BTP.Common.CustomConfig.CommonFileServerUrl + codepath;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("自提订单生成二维码异常。订单Id：{0} 入参：{1},出参：{2}", order.Id, PickUpCodePre + pickUpCode, codepath), ex);
                genQRCodeError = true;
            }
            try
            {
                //将错误的订单编号存到ErrorCommodityOrder表中
                ContextFactory.ReleaseContextSession();
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                if (genQRCodeError)
                {

                    ErrorCommodityOrder errorCommodityOrder = ErrorCommodityOrder.CreateErrorCommodityOrder();
                    errorCommodityOrder.ErrorOrderId = order.Id;
                    errorCommodityOrder.ResourceType = 0;
                    errorCommodityOrder.Source = 0;
                    errorCommodityOrder.AppId = order.AppId;
                    errorCommodityOrder.UserId = order.UserId;
                    errorCommodityOrder.OrderCode = order.Code;

                    errorCommodityOrder.EntityState = System.Data.EntityState.Added;

                    contextSession.SaveObject(errorCommodityOrder);
                }

                AppOrderPickUp appOrderPickUp = new AppOrderPickUp();
                appOrderPickUp.Id = order.Id;
                appOrderPickUp.SubTime = DateTime.Now;
                appOrderPickUp.ModifiedOn = DateTime.Now;
                appOrderPickUp.AppId = order.EsAppId.Value;
                appOrderPickUp.Name = pickUpInfo.Name;
                appOrderPickUp.Phone = pickUpInfo.Phone;
                appOrderPickUp.BookDate = pickUpInfo.BookDate;
                appOrderPickUp.BookStartTime = pickUpInfo.BookStartTime;
                appOrderPickUp.BookEndTime = pickUpInfo.BookEndTime;
                appOrderPickUp.PickUpCode = pickUpCode;
                appOrderPickUp.PickUpQrCodeUrl = codepath;
                appOrderPickUp.SelfTakeStationId = appSelfTakeStation.Id;
                appOrderPickUp.StsProvince = appSelfTakeStation.Province;
                appOrderPickUp.StsCity = appSelfTakeStation.City;
                appOrderPickUp.StsDistrict = appSelfTakeStation.District;
                appOrderPickUp.StsAddress = string.Format("{0} {1} {2} {3}", ProvinceCityHelper.GetAreaNameByCode(appSelfTakeStation.Province), ProvinceCityHelper.GetAreaNameByCode(appSelfTakeStation.City), ProvinceCityHelper.GetAreaNameByCode(appSelfTakeStation.District), appSelfTakeStation.Address);
                appOrderPickUp.StsPhone = appSelfTakeStation.Phone;
                appOrderPickUp.StsName = appSelfTakeStation.Name;
                //appOrderPickUp.PickUpTime = mmmmmm;
                //appOrderPickUp.PickUpManagerId = mmmmmm;
                appOrderPickUp.UserId = order.UserId;
                appOrderPickUp.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(appOrderPickUp);
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {

                LogHelper.Error(string.Format("saveOrderPickUp error:{0}", ex));
            }
        }


        /// <summary>
        /// 预校验是否可以参加活动
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="comproDict">活动列表</param>
        /// <param name="comNumDict">订单中商品汇总</param>
        /// <param name="orderResultDTO">返回值，如果校验成功返回null</param>
        /// <returns></returns>
        private bool preCheckPromotion(Guid userId, Dictionary<Guid, TodayPromotion> comproDict, Dictionary<Guid, int> comNumDict, out OrderResultDTO orderResultDTO)
        {
            orderResultDTO = null;
            //如果有活动抢占活动资源
            if (!comproDict.Any())
            {
                return true;
            }
            foreach (var t in comproDict.Values)
            {
                var comNumber = comNumDict[t.CommodityId];
                if (t.LimitBuyEach != -1)
                {
                    int sumLi = RedisHelper.GetHashValue<int>(RedisKeyConst.UserLimitPrefix + t.PromotionId + ":" + t.CommodityId, userId.ToString());
                    if (sumLi + comNumber > t.LimitBuyEach)
                    {
                        orderResultDTO = new OrderResultDTO { ResultCode = 1, Message = string.Format("该商品每人限购{0}件，您已超限", t.LimitBuyEach) };
                        return false;
                    }
                }
                if (t.LimitBuyTotal != -1)
                {
                    if (t.SurplusLimitBuyTotal + comNumber > t.LimitBuyTotal)
                    {
                        orderResultDTO = new OrderResultDTO { ResultCode = 1, Message = "抢光了~" };
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 校验是否可以参加活动
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="comproDict">活动列表</param>
        /// <param name="comNumDict">订单中商品汇总</param>
        /// <param name="orderResultDTO">返回值，如果校验成功返回null</param>
        /// <returns></returns>
        private bool checkPromotion(Guid userId, Dictionary<Guid, TodayPromotion> comproDict, Dictionary<Guid, int> comNumDict, out OrderResultDTO orderResultDTO)
        {
            orderResultDTO = null;
            //如果有活动抢占活动资源
            if (!comproDict.Any())
            {
                return true;
            }
            foreach (var t in comproDict.Values)
            {
                if (t.LimitBuyEach != -1)
                {
                    int sumLi = RedisHelper.GetHashValue<int>(RedisKeyConst.UserLimitPrefix + t.PromotionId + ":" + t.CommodityId, userId.ToString());
                    if (sumLi > t.LimitBuyEach)
                    {
                        orderResultDTO = new OrderResultDTO { ResultCode = 1, Message = string.Format("该商品每人限购{0}件，您已超限", t.LimitBuyEach) };
                        return false;
                    }
                }
                if (t.LimitBuyTotal != -1)
                {
                    if (t.SurplusLimitBuyTotal > t.LimitBuyTotal)
                    {
                        orderResultDTO = new OrderResultDTO { ResultCode = 1, Message = "抢光了~" };
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 回滚活动资源
        /// </summary>
        /// <param name="proComTuples"></param>
        /// <param name="userId"></param>
        private void rollbackPromotion(List<Tuple<string, string, int>> proComTuples, Guid userId, Guid orderId)
        {
            if (proComTuples == null || !proComTuples.Any())
            {
                return;
            }
            List<Tuple<string, string, int>> backProComTuples = proComTuples.Select(proComTuple => new Tuple<string, string, int>(proComTuple.Item1, proComTuple.Item2, -proComTuple.Item3)).ToList();
            var result = RedisHelper.ListHIncr(backProComTuples, userId);
            if (result == null || !result.Any())
            {
                return;
            }
            ContextFactory.ReleaseContextSession();
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            List<TodayPromotion> needRefreshCacheTodayPromotions = new List<TodayPromotion>();

            //用户限购信息。
            var ulQuery = UserLimited.ObjectSet().Where(_ => _.CommodityOrderId == orderId);
            if (ulQuery.Any())
            {
                foreach (UserLimited ul in ulQuery)
                {
                    ul.EntityState = EntityState.Deleted;
                }
            }

            var promotionIdList = proComTuples.Select(c => new Guid(c.Item1)).ToList();
            var comIdList = proComTuples.Select(c => new Guid(c.Item2)).ToList();
            var todayPromotionList = TodayPromotion.ObjectSet().Where(c => promotionIdList.Contains(c.PromotionId) && comIdList.Contains(c.CommodityId)).ToList();
            var promotionItemsList = PromotionItems.ObjectSet().Where(c => promotionIdList.Contains(c.PromotionId) && comIdList.Contains(c.CommodityId)).ToList();

            foreach (var proComTuple in proComTuples)
            {
                var promotionId = new Guid(proComTuple.Item1);
                var comId = new Guid(proComTuple.Item2);
                var surplusLimitBuyTotal = Convert.ToInt32(result.First(c => c.Item1 == proComTuple.Item1 && c.Item2 == proComTuple.Item2).Item3);
                surplusLimitBuyTotal = surplusLimitBuyTotal < 0 ? 0 : surplusLimitBuyTotal;
                var todayPromotion = todayPromotionList.FirstOrDefault(c => c.PromotionId == promotionId && c.CommodityId == comId);
                if (todayPromotion != null)
                {
                    todayPromotion.SurplusLimitBuyTotal = surplusLimitBuyTotal;
                    todayPromotion.EntityState = EntityState.Modified;
                    needRefreshCacheTodayPromotions.Add(todayPromotion);
                }

                var promotionItems = promotionItemsList.FirstOrDefault(c => c.PromotionId == promotionId && c.CommodityId == comId);
                if (promotionItems != null)
                {
                    promotionItems.SurplusLimitBuyTotal = surplusLimitBuyTotal;
                    promotionItems.EntityState = EntityState.Modified;
                }

            }
            contextSession.SaveChanges();
            if (needRefreshCacheTodayPromotions.Any())
            {
                needRefreshCacheTodayPromotions.ForEach(c => c.RefreshCache(EntityState.Modified));
            }
        }

        /// <summary>
        /// 回滚活动资源
        /// </summary>
        /// <param name="proComTuples"></param>
        /// <param name="userId"></param>
        private void rollbackPromotion(List<Tuple<string, string, int>> proComTuples, Guid userId)
        {
            if (proComTuples == null || !proComTuples.Any())
            {
                return;
            }
            List<Tuple<string, string, int>> backProComTuples = proComTuples.Select(proComTuple => new Tuple<string, string, int>(proComTuple.Item1, proComTuple.Item2, -proComTuple.Item3)).ToList();
            var result = RedisHelper.ListHIncr(backProComTuples, userId);
            if (result == null || !result.Any())
            {
                return;
            }
            ContextFactory.ReleaseContextSession();
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            List<TodayPromotion> needRefreshCacheTodayPromotions = new List<TodayPromotion>();

            var promotionIdList = proComTuples.Select(c => new Guid(c.Item1)).ToList();
            var comIdList = proComTuples.Select(c => new Guid(c.Item2)).ToList();
            var todayPromotionList = TodayPromotion.ObjectSet().Where(c => promotionIdList.Contains(c.PromotionId) && comIdList.Contains(c.CommodityId)).ToList();
            var promotionItemsList = PromotionItems.ObjectSet().Where(c => promotionIdList.Contains(c.PromotionId) && comIdList.Contains(c.CommodityId)).ToList();

            foreach (var proComTuple in proComTuples)
            {
                var promotionId = new Guid(proComTuple.Item1);
                var comId = new Guid(proComTuple.Item2);
                var surplusLimitBuyTotal = Convert.ToInt32(result.First(c => c.Item1 == proComTuple.Item1 && c.Item2 == proComTuple.Item2).Item3);
                surplusLimitBuyTotal = surplusLimitBuyTotal < 0 ? 0 : surplusLimitBuyTotal;
                var todayPromotion = todayPromotionList.FirstOrDefault(c => c.PromotionId == promotionId && c.CommodityId == comId);
                if (todayPromotion != null)
                {
                    todayPromotion.SurplusLimitBuyTotal = surplusLimitBuyTotal;
                    todayPromotion.EntityState = EntityState.Modified;
                    needRefreshCacheTodayPromotions.Add(todayPromotion);
                }

                var promotionItems = promotionItemsList.FirstOrDefault(c => c.PromotionId == promotionId && c.CommodityId == comId);
                if (promotionItems != null)
                {
                    promotionItems.SurplusLimitBuyTotal = surplusLimitBuyTotal;
                    promotionItems.EntityState = EntityState.Modified;
                }

            }
            contextSession.SaveChanges();
            if (needRefreshCacheTodayPromotions.Any())
            {
                needRefreshCacheTodayPromotions.ForEach(c => c.RefreshCache(EntityState.Modified));
            }
        }


        private InvoiceJounal GetInvoiceJounal(Guid invoiceId, Guid subId, int stateFrom, int stateTo)
        {
            InvoiceJounal ij = InvoiceJounal.CreateInvoiceJounal();
            ij.InvoiceId = invoiceId;
            ij.SubTime = DateTime.Now;
            ij.ModifiedOn = ij.SubTime;
            ij.SubId = subId;
            ij.StateFrom = stateFrom;
            ij.StateTo = stateTo;

            ContextFactory.CurrentThreadContext.SaveObject(ij);

            return ij;
        }
        /// <summary>
        /// 提交订单
        /// </summary>
        /// <param name="orderSDTO"> 订单，里面包含有商品，一次下单会有多个订单，这个函数在一个循环里面</param>
        /// <param name="appIdRealPrice"></param>
        /// <param name="isMixPay">是否混合支付</param>
        /// <param name="isMultiPay">是否多App支付</param>
        /// <param name="yjbInfo"></param> 
        /// <returns></returns>
        private OrderResultDTO SubmitOrderCommon(Jinher.AMP.BTP.Deploy.CustomDTO.OrderSDTO orderSDTO, ref Tuple<Guid, decimal> appIdRealPrice, bool isMixPay = false, bool isMultiPay = false, Jinher.AMP.YJB.Deploy.CustomDTO.OrderInsteadCashDTO yjbInfo = null)
        {
            orderSDTO.StoreCouponShopDivid = 0;
            foreach (var commdity in orderSDTO.ShoppingCartItemSDTO)
                orderSDTO.StoreCouponShopDivid += commdity.StoreCouponDivide;

            //orderSDTO.PayCouponValue += orderSDTO.StoreCouponShopDivid;
            //qgb

            OrderResultDTO result = new OrderResultDTO();
            //在线支付包括支付宝与U付时,要跳转到的支付页面
            string payUrl = string.Empty;

            //如果没有商品则不允许
            if (orderSDTO.ShoppingCartItemSDTO == null || orderSDTO.ShoppingCartItemSDTO.Count == 0)
            {
                return new OrderResultDTO { ResultCode = 1, Message = "订单商品不能为空" };
            }
            if (orderSDTO.ScorePrice > 0 && orderSDTO.AppId != orderSDTO.EsAppId)
            {
                return new OrderResultDTO { ResultCode = 1, Message = "订单不能使用积分抵现，请重新下订单" };
            }
            if (orderSDTO.UserId == Guid.Empty)
            {
                return new OrderResultDTO { ResultCode = 1, Message = "用户信息异常，请重新登录" };
            }
            if (orderSDTO.ShoppingCartItemSDTO.Select(p => p.Type).Distinct().Count() >= 2)
            {
                return new OrderResultDTO { ResultCode = 1, Message = "实物商品和虚拟商品不能一起购买，请分开下单购买" };
            }
            //校验下单商品数量
            foreach (var shoppingCartItemSdto in orderSDTO.ShoppingCartItemSDTO)
            {
                if (shoppingCartItemSdto.CommodityNumber <= 0)
                {
                    shoppingCartItemSdto.CommodityNumber = 1;
                }
                //预先生成好订单项Id,以解除后边订单项、资源消费的依赖关系。
                shoppingCartItemSdto.OrderItemId = Guid.NewGuid();

            }
            DateTime now = DateTime.Now;
            Random rd = new Random();
            //订单ID
            Guid orderId = Guid.NewGuid();
            //订单号
            string orderCode = Jinher.AMP.BTP.Common.Snowflake.NewId();
            //活动、商品、销量
            var proComTuples = new List<Tuple<string, string, int>>();

            #region 基础数据加载

            //商品id列表
            List<Guid> comIdList = orderSDTO.ShoppingCartItemSDTO.Select(x => x.Id).Distinct().ToList();

            //商品数量列表。
            Dictionary<Guid, int> comNumDict = new Dictionary<Guid, int>();

            #region 判断包装规格库存

            var CommoditySpecificationsList = CommoditySpecifications.ObjectSet()
                .Where(p => comIdList.Contains(p.CommodityId)).Select(p => new { p.CommodityId, p.Attribute }).ToList();

            if (orderSDTO.ShoppingCartItemSDTO.Count() > 0)
            {
                foreach (var item in orderSDTO.ShoppingCartItemSDTO)
                {
                    if (item.Specifications < 0)
                    {
                        return new OrderResultDTO { ResultCode = 1, Message = "商品规格选择有误，请重新下订单" };
                    }
                    else if (item.Specifications > 1)
                    {
                        if (!CommoditySpecificationsList.Any(p => p.CommodityId == item.Id && p.Attribute == item.Specifications))
                        {
                            return new OrderResultDTO { ResultCode = 1, Message = "商品规格选择有误，请重新下订单" };
                        }
                        //商品数量字典
                        if (comNumDict.ContainsKey(item.Id))
                        {
                            comNumDict[item.Id] += item.CommodityNumber / item.Specifications;
                        }
                        else comNumDict.Add(item.Id, item.CommodityNumber / item.Specifications);
                    }
                    else
                    {
                        if (CommoditySpecificationsList.Any(p => p.CommodityId == item.Id && p.Attribute != 1))
                        {
                            return new OrderResultDTO { ResultCode = 1, Message = "商品规格选择有误，请重新下订单" };
                        }
                        //商品数量字典
                        if (comNumDict.ContainsKey(item.Id))
                        {
                            comNumDict[item.Id] += item.CommodityNumber;
                        }
                        else comNumDict.Add(item.Id, item.CommodityNumber);
                    }
                }
            }
            #endregion

            //商品列表
            List<CommodityDTO> comList = null;

            #region 商品列表

            comList = Commodity.ObjectSet()
                .Where(c => comIdList.Contains(c.Id) && !c.IsDel && c.State == 0)
                .Select(m => new CommodityDTO()
                {
                    Id = m.Id,
                    Price = m.Price,
                    AppId = m.AppId,
                    Stock = m.Stock,
                    SaleAreas = m.SaleAreas,
                    Name = m.Name,
                    PicturesPath = m.PicturesPath,
                    CommodityType = m.CommodityType,
                    ScorePercent = m.ScorePercent,
                    Duty = m.Duty,
                    TaxRate = m.TaxRate,
                    InputRax = m.InputRax,
                    MarketPrice = m.MarketPrice,
                    CostPrice = m.CostPrice,
                    Barcode = m.Barcode,
                    No_Code = m.No_Code,
                    JDCode = m.JDCode,
                    Unit = m.Unit,
                    Type = m.Type,
                    YJCouponActivityId = m.YJCouponActivityId,
                    YJCouponType = m.YJCouponType,
                    ErQiCode = m.ErQiCode
                }).ToList();

            #endregion


            //商品库存列表
            List<CommodityStockDTO> commodityStockList = null;

            #region 商品库存列表

            commodityStockList = CommodityStock.ObjectSet().Where(c => comIdList.Contains(c.CommodityId)).Select(s => new CommodityStockDTO()
            {
                Id = s.Id,
                Price = s.Price,
                CommodityId = s.CommodityId,
                ComAttribute = s.ComAttribute,
                Stock = s.Stock,
                Duty = s.Duty,
                MarketPrice = s.MarketPrice,
                CostPrice = s.CostPrice,
                Barcode = s.Barcode,
                No_Code = s.No_Code,
                JDCode = s.JDCode,
                ErQiCode = s.ErQiCode
            }).ToList();

            #endregion

            //是否有(虚拟商品)易捷卡密
            var hasVirtualCommodity = comList.Any(p => p.Type == 1 && !string.IsNullOrEmpty(p.YJCouponActivityId) && !string.IsNullOrEmpty(p.YJCouponType));
            orderSDTO.OrderType = hasVirtualCommodity ? 3 : orderSDTO.OrderType;

            orderSDTO.ShoppingCartItemSDTO.ForEach(p =>
            {
                p.Type = comList.Where(x => x.Id == p.Id).Select(x => x.Type ?? 0).FirstOrDefault();
            });

            //校验提交订单商品是否非法
            if (!comList.Any() || comList.Select(c => c.AppId).Distinct().Count() > 1 || comList.Count != comNumDict.Count)
            {
                return new OrderResultDTO { ResultCode = 1, Message = "提交订单有误" };
            }
            Guid orderAppId = comList.First().AppId;
            //如果EsAppId为空，默认为AppId。
            if (orderSDTO.EsAppId == Guid.Empty)
            {
                orderSDTO.EsAppId = orderAppId;
            }

            //参加活动的商品id列表(c => c.Price != c.RealPrice是为了过滤没有优惠活动时下单，下单时刻刚好商品有优惠活动还以下单时的价格为准。)
            List<Guid> proCommodityIdList = orderSDTO.ShoppingCartItemSDTO.Where(c => Math.Round(c.Price, 2) != Math.Round(c.RealPrice, 2)).Select(x => x.Id).Distinct().ToList();  //参加活动的商品id集合

            //今日活动列表
            List<TodayPromotion> todayPromotionList = new List<TodayPromotion>();

            #region 活动信息

            //拼团活动订单只取拼团活动
            if (orderSDTO.PromotionType == 3)
            {
                todayPromotionList = TodayPromotion.ObjectSet().Where(t => comIdList.Contains(t.CommodityId) && t.EndTime > now && t.StartTime < now && t.PromotionType == 3).ToList();
                if (!todayPromotionList.Any())
                {
                    return new OrderResultDTO { ResultCode = 1, Message = "促销活动已结束，请重新下单" };
                }
                proCommodityIdList = todayPromotionList.Select(c => c.CommodityId).Distinct().ToList();
            }
            // 预售活动
            else if (orderSDTO.PromotionType == 5)
            {
                todayPromotionList = TodayPromotion.ObjectSet().Where(t => comIdList.Contains(t.CommodityId) && t.EndTime > now && t.StartTime < now && t.PromotionType == 5).ToList();
                if (!todayPromotionList.Any())
                {
                    return new OrderResultDTO { ResultCode = 1, Message = "预售活动已结束，请重新下单" };
                }
                proCommodityIdList = todayPromotionList.Select(c => c.CommodityId).Distinct().ToList();
            }
            else
            {
                //优惠套装不走活动判断
                if (!orderSDTO.IsSetMeal && !orderSDTO.IsJcActivity)
                {

                    var jcap = new GetJcActivityIdsParamDTO();
                    jcap.AppId = orderSDTO.EsAppId;
                    jcap.CommodityIds = new List<Guid>();
                    jcap.CommodityStockIds = new List<Guid>();

                    jcap.CommodityIds.AddRange(orderSDTO.ShoppingCartItemSDTO.Select(_ => _.Id));
                    IEnumerable<Guid> stockQuery = orderSDTO.ShoppingCartItemSDTO.Where(_ => _.CommodityStockId != null && _.CommodityStockId.HasValue).Select(_ => _.CommodityStockId.Value);
                    if (stockQuery != null && stockQuery.Any())
                    {
                        jcap.CommodityStockIds.AddRange(stockQuery);
                    }
                    List<JcActivityIdsDTO> jcar = ZPHSV.Instance.GetJcActivityIdsByCommodity(jcap);
                    var jcActivityIds = jcar.Select(a => a.ActivityId).Distinct().ToList();


                    #region

                    ////todo GetActivityIdByInfo 改为批量的
                    //foreach (var shoppingCartItemSdto in orderSDTO.ShoppingCartItemSDTO)
                    //{
                    //    SearchJCActivityCDTO searchJcActivityCdto = new SearchJCActivityCDTO();
                    //    searchJcActivityCdto.AppId = orderSDTO.EsAppId;
                    //    searchJcActivityCdto.Theme = shoppingCartItemSdto.Name;
                    //    var jcActivity = TPS.ZPHSV.Instance.GetActivityIdByInfo(searchJcActivityCdto);

                    //    LogHelper.Debug("金采团购活动jcActivity：" + JsonHelper.JsSerializer(jcActivity));
                    //    if (jcActivity != null && jcActivity.isSuccess)
                    //    {
                    //        jcActivityIds.Add(jcActivity.Data.id);
                    //    }
                    //}

                    #endregion

                    LogHelper.Debug("金采团购活动id集合：" + JsonHelper.JsSerializer(jcActivityIds));
                    if (jcActivityIds.Count > 1)
                    {
                        return new OrderResultDTO { ResultCode = 1, Message = "不支持多个金采团购活动商品同时下单，请重新下单" };
                    }
                    if (jcActivityIds.Count == 1 && jcActivityIds.FirstOrDefault() != Guid.Empty)
                    {
                        //给金采团购活动Id赋值
                        orderSDTO.JcActivityId = jcActivityIds.FirstOrDefault();
                        orderSDTO.IsJcActivity = true;
                        if (!ZPHSV.Instance.IsActivityAuthToUser(orderSDTO.JcActivityId, orderSDTO.UserId))
                        {
                            return new OrderResultDTO { ResultCode = 1, Message = "您不是授权用户" };
                        }
                    }
                    else
                    {
                        //商品活动字典（只取提交订单时有活动的商品活动）
                        todayPromotionList = TodayPromotion.ObjectSet().Where(t => proCommodityIdList.Contains(t.CommodityId) && t.EndTime > now && t.StartTime < now && t.PromotionType != 3).ToList();
                        if (!todayPromotionList.Any() && proCommodityIdList.Count > 0)
                        {
                            return new OrderResultDTO { ResultCode = 1, Message = "促销活动已结束，请重新下单" };
                        }
                    }
                }
            }

            #endregion

            //商品活动信息
            var comproDict = todayPromotionList.GroupBy(c => c.CommodityId).ToDictionary(x => x.Key, y => y.OrderByDescending(c => c.PromotionType).First());

            //会员折扣信息
            var vipPromotionDTO = AVMSV.GetVipIntensity(orderAppId, orderSDTO.UserId);

            //
            CateringSetting cateringSetting = null;
            if (orderSDTO.OrderType == 2)
            {
                cateringSetting = CateringSetting.ObjectSet().FirstOrDefault(c => c.AppId == orderAppId && !c.IsDel);
            }

            #endregion

            #region 提交订单数据校验


            #region 优惠信息验证

            if (orderSDTO.IsSetMeal && (orderSDTO.IsUseYouKa && orderSDTO.YJBPrice > 0 && orderSDTO.CouponValue > 0)
                 || (orderSDTO.IsUseYouKa && orderSDTO.YJBPrice > 0)
                 || (orderSDTO.IsUseYouKa && orderSDTO.CouponValue > 0)
                 || (orderSDTO.YJBPrice > 0 && orderSDTO.CouponValue > 0))
            {
                return new OrderResultDTO { ResultCode = 1, Message = "优惠券、易捷币抵现券、赠送油卡兑换券、优惠套装不能同时享受，请修改~" };
            }

            if ((comproDict.Count > 0 || vipPromotionDTO.DiscountPrice > 0) && (orderSDTO.IsUseYouKa || orderSDTO.YJBPrice > 0 || orderSDTO.CouponValue > 0 || orderSDTO.IsSetMeal))
            {
                return new OrderResultDTO { ResultCode = 1, Message = "活动商品不能使用优惠券、易捷币、油卡抵用券、优惠套装，请修改~" };
            }

            if (orderSDTO.EsAppId == YJB.Deploy.CustomDTO.YJBConsts.YJAppId)
            {
                if (orderSDTO.ShoppingCartItemSDTO.Any(_ => _.Presents != null && _.Presents.Count > 0))
                {
                    if (orderSDTO.IsUseYouKa || orderSDTO.YJBPrice > 0 || orderSDTO.CouponValue > 0)
                    {
                        return new OrderResultDTO { ResultCode = 1, Message = "参与赠品促销的商品不能使用优惠券、易捷币、油卡抵用券，请修改~" };
                    }
                }
            }

            #endregion


            #region 并行验证下订单要消耗的各种资源。

            ResourceVerifyParamDTO rvp = new ResourceVerifyParamDTO();
            rvp.CommodityList = comList;
            rvp.ComNumDict = comNumDict;
            rvp.ComproDict = comproDict;
            rvp.HasVirtualCommodity = hasVirtualCommodity;
            rvp.OrderCode = orderCode;
            rvp.OrderId = orderId;
            rvp.OrderInfo = orderSDTO;
            rvp.StockList = commodityStockList;
            rvp.VipPromotion = vipPromotionDTO;
            rvp.cateringSetting = cateringSetting;

            ResourceVerifyer resVerify = new ResourceVerifyer();
            List<ResultDTO> verifyResult = resVerify.Verify(rvp);
            IEnumerable<ResultDTO> vrError = verifyResult.Where(_ => _.ResultCode != 0);
            if (vrError.Any())
            {
                result.ResultCode = vrError.ElementAt(0).ResultCode;
                result.Message = vrError.ElementAt(0).Message;
                return result;
            }

            #endregion


            CouponNewDTO couponNewDTO = resVerify.CouponInfo;
            AppSelfTakeStation appSelfTakeStation = resVerify.AppSelfTakeStation;


            //运费计算
            decimal freight = decimal.Zero;

            #region  运费计算

            if (orderSDTO.SelfTakeFlag == 0)
            {
                if (orderSDTO.OrderType == 2)
                {
                    freight = cateringSetting.DeliveryFee;
                }
                else
                {
                    var coupons = new Dictionary<Guid, decimal>();
                    if (couponNewDTO != null)
                    {
                        coupons.Add(couponNewDTO.ShopId, couponNewDTO.Cash);
                    }
                    List<TemplateCountDTO> templateCountList = orderSDTO.ShoppingCartItemSDTO.Select(item => new TemplateCountDTO { CommodityId = item.Id, Count = item.CommodityNumber, Price = item.RealPrice }).ToList();
                    freight = new CommoditySV().CalFreightMultiAppsByTextExt(orderSDTO.Province, orderSDTO.SelfTakeFlag, templateCountList, coupons, yjbInfo, orderSDTO.YJCouponIds).Freight;
                    LogHelper.Debug(string.Format("记录订单运费1，orderSDTO.Id：{0},freight：{1}", orderId, freight));
                }
            }

            if (freight < 0)
            {
                return new OrderResultDTO { ResultCode = 1, Message = "运费异常，请重新下单" };
            }
            //校验订单总价
            LogHelper.Info(string.Format("下订单 OrderCode:" + orderCode + ", orderSDTO.Price :{0}, freight {1} ,orderSDTO.CouponValue {2}, orderSDTO.RealPrice:{3},orderSDTO.ScorePrice:{4}, orderSDTO.YJBPrice:{5},orderSDTO.Duty:{6},(orderSDTO.deliveryFeeDiscount ?? 0):{7}, orderSDTO.CouponValue {8}", orderSDTO.Price, freight, orderSDTO.CouponValue, orderSDTO.RealPrice, orderSDTO.ScorePrice, orderSDTO.YJBPrice, (orderSDTO.Duty ?? 0), (orderSDTO.deliveryFeeDiscount ?? 0), orderSDTO.YJCouponPrice));
            if (orderSDTO.Price + freight - (orderSDTO.YJCouponPrice) - orderSDTO.CouponValue - orderSDTO.ScorePrice - orderSDTO.YJBPrice - (orderSDTO.deliveryFeeDiscount ?? 0) - orderSDTO.YjCardPrice + (orderSDTO.Duty ?? 0) > orderSDTO.RealPrice)
            {
                return new OrderResultDTO { ResultCode = 1, Message = "商品价格发生变化，请重新下单" };
            }

            #endregion

            decimal realPrice = 0;

            #region realPrice计算

            if (orderSDTO.IsSetMeal && orderSDTO.SetMealId != Guid.Empty)
            {
                var setMealActivitys = ZPHSV.Instance.GetSetMealActivitysById(orderSDTO.SetMealId);
                realPrice = setMealActivitys.MealPrice;
            }
            else if (orderSDTO.IsJcActivity && orderSDTO.JcActivityId != Guid.Empty)
            {
                foreach (var item in orderSDTO.ShoppingCartItemSDTO)
                {
                    var jcPromotion = ZPHSV.Instance.GetItemsListByActivityId(orderSDTO.JcActivityId).Data.Where(t => t.ComdtyId == item.Id && t.ComdtyStockId == item.CommodityStockId).FirstOrDefault();
                    if (jcPromotion != null)
                    {
                        realPrice += item.CommodityNumber * jcPromotion.GroupPrice;
                    }
                }
            }
            else
            {
                foreach (var item in orderSDTO.ShoppingCartItemSDTO)
                {
                    var promotion = todayPromotionList.Where(_ => _.CommodityId == item.Id).FirstOrDefault();
                    if (item.CommodityStockId.HasValue && item.CommodityStockId != Guid.Empty && item.CommodityStockId != item.Id)
                    {
                        if (promotion != null)
                        {
                            var skuPromotion = ZPHSV.Instance.GetSkuActivityList((Guid)promotion.OutsideId).Where(t => t.IsJoin && t.CommodityId == item.Id && t.CommodityStockId == item.CommodityStockId).FirstOrDefault();
                            if (skuPromotion != null)
                            {
                                realPrice += item.CommodityNumber * skuPromotion.JoinPrice;
                            }
                            else
                            {
                                realPrice += item.CommodityNumber * promotion.DiscountPrice.Value;
                            }
                        }
                        else
                        {
                            realPrice += item.CommodityNumber * commodityStockList.FirstOrDefault(_ => _.Id == item.CommodityStockId).Price;
                        }
                    }
                    else
                    {
                        // 检查活动价
                        if (promotion != null)
                        {
                            realPrice += item.CommodityNumber * promotion.DiscountPrice.Value;
                        }
                        else
                        {
                            realPrice += item.CommodityNumber * comList.FirstOrDefault(_ => _.Id == item.Id).Price;
                        }
                    }
                }
            }
            if (orderSDTO.Price != realPrice)
            {
                LogHelper.Error("商品价格异常，请重新下单.orderSDTO.RealPrice:" + orderSDTO.RealPrice + "；realPrice" + realPrice +
                                "；orderSDTO.Price" + orderSDTO.Price + "；orderSDTO.ShoppingCartItemSDTO" + JsonHelper.JsonSerializer(orderSDTO.ShoppingCartItemSDTO));
                return new OrderResultDTO { ResultCode = 1, Message = "商品价格异常，请重新下单" };
            }

            #endregion


            #endregion

            #region 保存订单
            CommodityOrder commodityOrderDTO = null;
            List<OrderItem> orderItems = null;
            try
            {

                //消费资源之前，先生成一个失败订单，各步骤记录消费的资源，若有某一步消费失败，统一回滚资源。
                //CreatePreOrder(orderId, orderCode, orderSDTO);

                ContextSession contextSession = ContextFactory.CurrentThreadContext;


                // 更新京东订单ID
                JdOrderHelper.UpdateJdOrderId(contextSession, Guid.Empty.ToString(), orderId.ToString(), orderSDTO.AppId.ToString(), orderSDTO.UserId.ToString());
                // 苏宁订单提交
                List<SNOrderItem> SnOrderItemList = null;
                if (ThirdECommerceHelper.IsSuNingYiGou(orderSDTO.AppId))
                {
                    SnOrderItemList = SuningSV.suning_govbus_order_add(contextSession, orderSDTO, orderId);
                    if (SnOrderItemList == null)
                    {
                        return new OrderResultDTO { ResultCode = 1, Message = "商品已经售罄，请重新下单" };
                    }
                }


                //校验积分
                int scoreCost = resVerify.ScoreCost;
                int dxScore = (int)(scoreCost * orderSDTO.ScorePrice);


                #region 初始化订单、订单项信息。（临时解决方案：有时间要精简RollbackOrderResource参数，并逐个对比在此赋值后的各字段是否在此之后有变化）

                commodityOrderDTO = new CommodityOrder
                {
                    Freight = freight,
                    SelfTakeFlag = orderSDTO.SelfTakeFlag,
                    Id = orderId,
                    Name = "用户订单",
                    Code = orderCode,
                    SubId = orderSDTO.UserId,
                    UserId = orderSDTO.UserId,
                    State = 0,
                    Payment = orderSDTO.Payment,
                    Price = orderSDTO.Price,
                    ReceiptAddress = orderSDTO.ReceiptAddress ?? "",
                    ReceiptPhone = orderSDTO.ReceiptPhone ?? "",
                    ReceiptUserName = orderSDTO.ReceiptUserName ?? "",
                    City = orderSDTO.City ?? "",
                    Province = orderSDTO.Province ?? "",
                    District = orderSDTO.District ?? "",
                    Street = orderSDTO.Street ?? "",
                    AppId = orderAppId,
                    Details = orderSDTO.Details,
                    IsModifiedPrice = false,
                    RecipientsZipCode = orderSDTO.RecipientsZipCode,
                    SrcTagId = orderSDTO.SrcTagId,
                    SrcType = orderSDTO.SrcType,
                    CPSId = orderSDTO.CPSId,
                    OrderType = orderSDTO.OrderType,
                    ServiceId = orderSDTO.ServiceId,
                    RealPrice = orderSDTO.RealPrice,
                    EsAppId = orderSDTO.EsAppId,
                    PicturesPath = orderSDTO.PicturesPath,
                    //餐盒费
                    MealBoxFee = orderSDTO.mealBoxFee,
                    Duty = orderSDTO.Duty == null ? 0 : (decimal)orderSDTO.Duty,
                    FirstContent = orderSDTO.FirstContent,
                    SecondContent = orderSDTO.SecondContent,
                    ThirdContent = orderSDTO.ThirdContent,
                    SetMealId = orderSDTO.SetMealId,
                    InviterMobile = orderSDTO.InviterMobile,
                    EntityState = EntityState.Added
                };

                orderItems = new List<OrderItem>();
                //遍历商品
                foreach (var item in orderSDTO.ShoppingCartItemSDTO)
                {
                    OrderItem orderitem = new OrderItem
                    {
                        Number = item.CommodityNumber,
                        Code = orderCode,
                        Name = item.Name,
                        CommodityAttributes = item.SizeAndColorId,
                        CommodityStockId = item.CommodityStockId,
                        CommodityId = item.Id,
                        PicturesPath = item.Pic,
                        PromotionId = item.PromotionId,
                        Id = Guid.NewGuid(),
                        CommodityOrderId = orderId,
                        SubTime = now,
                        CurrentPrice = item.Price,
                        SubId = item.UserId,
                        Intensity = item.Intensity,
                        DiscountPrice = item.DiscountPrice,
                        RealPrice = item.RealPrice,
                        ScorePrice = item.ScorePrice,
                        AlreadyReview = false,
                        Duty = item.Duty,
                        TaxRate = item.TaxRate,
                        InputRax = item.InputRax,
                        CostPrice = item.CostPrice,
                        Barcode = item.BarCode,
                        No_Code = item.Code,
                        InnerCatetoryIds = item.InnerCatetoryIds,
                        Unit = item.Unit,
                        Type = item.Type,
                        YJCouponActivityId = comList.Where(x => x.Id == item.Id).Select(x => x.YJCouponActivityId).FirstOrDefault(),
                        YJCouponType = comList.Where(x => x.Id == item.Id).Select(x => x.YJCouponType).FirstOrDefault(),
                        ErQiCode = item.ErQiCode,
                        Specifications = item.Specifications,
                        AppId = item.AppId,
                        JDCode = item.JDCode,
                        YouKaPercent = orderSDTO.IsUseYouKa ? -1 : 0, //如果订单送油卡兑换包但未获取到送卡额度，暂时存-1，送卡时修正
                        EntityState = EntityState.Added
                    };

                    var com = comList.Where(_ => _.Id == orderitem.CommodityId).FirstOrDefault();
                    if (com != null)
                    {
                        orderitem.AppId = com.AppId;
                    }
                    orderItems.Add(orderitem);
                }

                #endregion



                #region 并行消费下订单资源

                ResourceSpendParamDTO rspDto = new ResourceSpendParamDTO();
                rspDto.OrderInfo = orderSDTO;
                rspDto.OrderId = orderId;
                rspDto.OrderCode = orderCode;
                rspDto.Score = dxScore;
                rspDto.CouponInfo = couponNewDTO;
                rspDto.ComNumDict = comNumDict;
                rspDto.ComproDict = comproDict;
                rspDto.ContextDTO = ContextDTO;
                rspDto.proCommodityIdList = proCommodityIdList;
                rspDto.todayPromotionList = todayPromotionList;

                ResourceSpender resSpender = new ResourceSpender();
                List<ResultDTO> spendResult = resSpender.Spend(rspDto);
                var spendError = spendResult.Where(x => x.ResultCode != 0);
                if (spendError.Any())
                {
                    //回滚资源。
                    RollbackOrderResource(commodityOrderDTO, orderItems);
                    //消费资源有错误，直接返回错误。
                    result.ResultCode = spendError.ElementAt(0).ResultCode;
                    result.Message = spendError.ElementAt(0).Message;
                    return result;
                }

                #endregion

                #region 删除购物车

                var shopCartItemIds = orderSDTO.ShoppingCartItemSDTO.Where(c => c.ShopCartItemId != Guid.Empty).Select(c => c.ShopCartItemId).ToList();
                if (shopCartItemIds.Any())
                {
                    var shopCarts = ShoppingCartItems.ObjectSet().Where(c => shopCartItemIds.Contains(c.Id)).ToList();
                    if (shopCarts.Any())
                    {
                        foreach (var shoppingCartItem in shopCarts)
                        {
                            shoppingCartItem.EntityState = EntityState.Deleted;
                        }
                    }
                }

                #endregion

                #region 订单项处理子流程


                #region 油卡兑换券
                var ykPercentDir = orderSDTO.IsUseYouKa ? YJBHelper.GetCommodityYouKaPercent(orderSDTO.EsAppId, orderSDTO.ShoppingCartItemSDTO.Select(p => p.Id).ToList()).ToDictionary(p => p.CommodityId, p => p.YouKaPersent)
                    : new Dictionary<Guid, decimal>();
                #endregion

                #region 订单商品金额拆分
                //适合优惠券抵用的所有商品之和
                decimal sumPrice = 0;
                //去除最后一个商品的优惠券抵现之和 
                decimal sumWipeLastPrice = 0;
                //去除最后一个商品的运费抵现之和 
                decimal sumFreightLastPrice = 0;
                //去除最后一个商品的易捷卡抵现之和 
                decimal sumYjcardLastPrice = 0;

                if (orderSDTO.CouponValue > 0 && orderSDTO.CouponId != Guid.Empty)
                {
                    switch (couponNewDTO.CouponType)
                    {
                        case CouponType.SpecifyGoods:
                            {
                                var goods = orderSDTO.ShoppingCartItemSDTO.Where(t => couponNewDTO.GoodList.Contains(t.Id));
                                sumPrice += goods.Sum(shoppingCartItemSdto => shoppingCartItemSdto.RealPrice * shoppingCartItemSdto.CommodityNumber);
                            }
                            break;
                        case CouponType.BeInCommon:
                            {
                                var goods = orderSDTO.ShoppingCartItemSDTO.Where(t => t.AppId == couponNewDTO.ShopId);
                                sumPrice += goods.Sum(shoppingCartItemSdto => shoppingCartItemSdto.RealPrice * shoppingCartItemSdto.CommodityNumber);
                            }
                            break;
                    }
                }
                #endregion

                #region 商品分类

                List<CategoryDTO> categoryList = new List<CategoryDTO>();
                var comCategoryIds = orderSDTO.ShoppingCartItemSDTO.Where(c => c.CategoryId.HasValue && c.CategoryId != Guid.Empty).Select(c => c.CategoryId.Value).Distinct().ToList();
                if (comCategoryIds.Any())
                {
                    categoryList = Category.ObjectSet().Where(c => comCategoryIds.Contains(c.Id)).Select(c => new CategoryDTO { Id = c.Id, Name = c.Name }).ToList();
                }

                #endregion



                //订单商品序号
                var index = 0;
                //指定商品所需参数
                var goodIndex = 0;
                var goodPrice = 0m;
                // 保存OrderItem，这次下单的所有订单
                orderItems = new List<OrderItem>();
                //遍历商品
                foreach (var item in orderSDTO.ShoppingCartItemSDTO)
                {

                    LogHelper.Info(string.Format("下订单 orderSDTO.ShoppingCartItemSDTO :{0}", JsonHelper.JsSerializer(orderSDTO.ShoppingCartItemSDTO)));
                    OrderItem orderitem = new OrderItem
                    {
                        Number = item.CommodityNumber,
                        Code = orderCode,
                        Name = item.Name,
                        CommodityAttributes = item.SizeAndColorId,
                        CommodityStockId = item.CommodityStockId,
                        CommodityId = item.Id,
                        PicturesPath = item.Pic,
                        PromotionId = item.PromotionId,
                        Id = item.OrderItemId,
                        CommodityOrderId = orderId,
                        SubTime = now,
                        CurrentPrice = item.Price,
                        SubId = item.UserId,
                        Intensity = item.Intensity,
                        DiscountPrice = item.DiscountPrice,
                        RealPrice = item.RealPrice,
                        ScorePrice = item.ScorePrice,
                        AlreadyReview = false,
                        Duty = item.Duty,
                        TaxRate = item.TaxRate,
                        InputRax = item.InputRax,
                        CostPrice = item.CostPrice,
                        Barcode = item.BarCode,
                        No_Code = item.Code,
                        InnerCatetoryIds = item.InnerCatetoryIds,
                        Unit = item.Unit,
                        Type = item.Type,
                        YJCouponActivityId = comList.Where(x => x.Id == item.Id).Select(x => x.YJCouponActivityId).FirstOrDefault(),
                        YJCouponType = comList.Where(x => x.Id == item.Id).Select(x => x.YJCouponType).FirstOrDefault(),
                        ErQiCode = item.ErQiCode,
                        Specifications = item.Specifications,
                        AppId = item.AppId,
                        JDCode = item.JDCode,
                        YouKaPercent = orderSDTO.IsUseYouKa ? -1 : 0, //如果订单送油卡兑换包但未获取到送卡额度，暂时存-1，送卡时修正
                        EntityState = EntityState.Added
                    };

                    var com = comList.Where(_ => _.Id == orderitem.CommodityId).FirstOrDefault();
                    if (com != null)
                    {
                        orderitem.AppId = com.AppId;
                    }

                    orderItems.Add(orderitem);
                    if (ykPercentDir.ContainsKey(item.Id))
                    {
                        orderitem.YouKaPercent = ykPercentDir[item.Id];
                    }

                    LogHelper.Debug("进入获取金彩金采团购活动，赠送油卡兑换券orderSDTO.IsJcActivity：" + orderSDTO.IsJcActivity + ",orderSDTO.JcActivityId :" + orderSDTO.JcActivityId);
                    //金采团购活动 赠送油卡兑换券
                    if (orderSDTO.IsJcActivity && orderSDTO.JcActivityId != Guid.Empty && orderSDTO.JcActivityId != null)
                    {
                        var jcPromotion = resSpender.JCActivityItemsList.FirstOrDefault(t => t.ComdtyId == item.Id && t.ComdtyStockId == item.CommodityStockId);
                        //var jcPromotion = ZPHSV.Instance.GetItemsListByActivityId(orderSDTO.JcActivityId).Data.FirstOrDefault(t => t.ComdtyId == item.Id && t.ComdtyStockId == item.CommodityStockId);
                        LogHelper.Debug("进入获取金彩金采团购活动，赠送油卡兑换券jcPromotion：" + JsonHelper.JsSerializer(jcPromotion));
                        if (jcPromotion != null)
                        {
                            orderitem.RealPrice = jcPromotion.GroupPrice;
                            orderitem.DiscountPrice = jcPromotion.GroupPrice;
                            orderitem.PromotionDesc = "金采支付";
                            //油卡兑换券 默认为0
                            orderitem.YouKaPercent = 0;
                            if (jcPromotion.GiftGardScale != 0)
                            {
                                orderitem.YouKaPercent = jcPromotion.GiftGardScale;
                            }
                        }
                    }

                    if (comproDict.ContainsKey(item.Id))
                    {
                        orderitem.PromotionId = comproDict[item.Id].PromotionId;
                        orderitem.PromotionType = comproDict[item.Id].PromotionType;
                        orderitem.PromotionDesc = Promotion.GetPromotionTypeDesc(comproDict[item.Id].PromotionType);
                        if (orderitem.DiscountPrice == 0)
                        {
                            //var skuActivityList = ZPHSV.Instance.GetSkuActivityList((Guid)comproDict[item.Id].OutsideId).Where(t => t.CommodityId == comproDict[item.Id].CommodityId);
                            var skuActivityList = resSpender.SkuActivityList.Where(x => x.OutSideActivityId == (Guid)comproDict[item.Id].OutsideId && x.CommodityId == comproDict[item.Id].CommodityId);
                            if (skuActivityList.Any())
                            {
                                var commodityStockId = item.CommodityStockId;
                                LogHelper.Debug(string.Format("下订单 commodityStockId :{0}", commodityStockId));
                                if (commodityStockId == null || !commodityStockId.HasValue || commodityStockId == Guid.Empty)
                                {
                                    commodityStockId = skuActivityList.ToList()[0].CommodityStockId;
                                }
                                LogHelper.Debug(string.Format("1下订单 commodityStockId :{0}", commodityStockId));
                                var skuActivity = skuActivityList.FirstOrDefault(t => t.CommodityStockId == commodityStockId);
                                if (skuActivity != null && skuActivity.IsJoin)
                                {
                                    orderitem.DiscountPrice = Convert.ToDecimal(skuActivity.JoinPrice);
                                    LogHelper.Debug(string.Format("1下订单 orderitem.DiscountPrice :{0}", orderitem.DiscountPrice));
                                }
                            }
                        }
                        // 预售活动设置订单发货时间
                        if (orderitem.PromotionType == 5)
                        {
                            //var presell = ZPHSV.Instance.GetAndCheckPresellInfoById(new Jinher.AMP.ZPH.Deploy.CustomDTO.CheckPresellInfoCDTO()
                            //{
                            //    comdtyId = orderitem.CommodityId,
                            //    id = comproDict[item.Id].OutsideId.Value
                            //});
                            var presell = resSpender.PresellInfoList.FirstOrDefault(x => x.id == comproDict[item.Id].OutsideId.Value);
                            if (presell != null)
                            {
                                orderitem.DeliveryDays = presell.DeliveryDays;
                                orderitem.DeliveryTime = presell.DeliveryTime;
                            }
                        }
                    }
                    else if (vipPromotionDTO.IsVip)
                    {
                        orderitem.VipLevelId = vipPromotionDTO.VipLevlelId;
                        orderitem.PromotionType = 200;
                        orderitem.PromotionDesc = vipPromotionDTO.VipLevelDesc;
                    }

                    if (item.CategoryId.HasValue && item.CategoryId != Guid.Empty)
                    {
                        var category = categoryList.FirstOrDefault(c => c.Id == item.CategoryId.Value);
                        if (category != null)
                        {
                            orderitem.ComCategoryId = item.CategoryId;
                            orderitem.ComCategoryName = category.Name;
                        }
                    }

                    #region 订单商品金额拆分
                    //优惠券 （商品承担的优惠券金额=（商品金额/参与优惠券使用贡献的所有商品金额之和）*优惠券金额；订单中最后一个商品承担的优惠券=优惠券金额-订单中其余商品承担的优惠券金额之和所有商品金额之和）
                    orderitem.CouponPrice = 0;
                    if (orderSDTO.CouponId != Guid.Empty && orderSDTO.CouponValue > 0)
                    {
                        //指定商品的优惠券特殊处理
                        if (couponNewDTO.CouponType == CouponType.BeInCommon)
                        {
                            if (index < orderSDTO.ShoppingCartItemSDTO.Count - 1)
                            {
                                orderitem.CouponPrice = Math.Round(((item.RealPrice * item.CommodityNumber) / sumPrice) * orderSDTO.CouponValue, 2, MidpointRounding.AwayFromZero);
                                sumWipeLastPrice += (decimal)orderitem.CouponPrice;
                            }
                            else
                            {
                                orderitem.CouponPrice = Math.Round(orderSDTO.CouponValue - sumWipeLastPrice, 2, MidpointRounding.AwayFromZero);
                            }
                        }
                        else
                        {
                            var goodIds = orderSDTO.ShoppingCartItemSDTO.Where(t => couponNewDTO.GoodList.Contains(t.Id)).Select(t => t.Id).Distinct();
                            if (goodIds.Count() == 1)
                            {
                                orderitem.CouponPrice = goodIds.First() == orderitem.CommodityId ? orderSDTO.CouponValue : 0;
                            }
                            else
                            {
                                if (goodIds.Contains(orderitem.CommodityId))
                                {
                                    if (goodIndex == goodIds.Count())
                                    {
                                        orderitem.CouponPrice = Math.Round(orderSDTO.CouponValue - goodPrice, 2, MidpointRounding.AwayFromZero);
                                    }
                                    else
                                    {
                                        orderitem.CouponPrice = Math.Round(((item.RealPrice * item.CommodityNumber) / sumPrice) * orderSDTO.CouponValue, 2, MidpointRounding.AwayFromZero);
                                        goodPrice += (decimal)orderitem.CouponPrice;
                                        goodIndex++;
                                    }
                                }
                                else
                                {
                                    orderitem.CouponPrice = 0;
                                }
                            }
                        }
                    }


                    orderitem.CouponPrice += item.StoreCouponDivide;//qgb

                    //if (CacheHelper.MallApply.GetMallTypeListByEsAppId(orderSDTO.EsAppId).Any(_ => _.Id == orderSDTO.AppId && _.Type != 1))//如果这个店铺是自营的
                    //{
                    //    if (StoreCouponCommdityCount == 1)//这个是最后一个商品
                    //    {
                    //        orderitem.CouponPrice -= StoreCouponRemainingPrice;
                    //    }
                    //    else if (StoreCouponCommdityCount > 1)
                    //    {
                    //        orderitem.CouponPrice -= Math.Round(((item.RealPrice * item.CommodityNumber) / orderSDTO.StoreCouponCommdityPrice) * orderSDTO.StoreCouponPrice, 2, MidpointRounding.AwayFromZero);
                    //        StoreCouponRemainingPrice -= Math.Round(((item.RealPrice * item.CommodityNumber) / orderSDTO.StoreCouponCommdityPrice) * orderSDTO.StoreCouponPrice, 2, MidpointRounding.AwayFromZero);
                    //    }
                    //    StoreCouponCommdityCount--;
                    //}


                    //运费 该商品的运费与计算商品运费时商品的运费一致 
                    if (freight > 0)
                    {
                        //TemplateCountDTO templateCountDto = new TemplateCountDTO()
                        //{
                        //    CommodityId = item.Id,
                        //    Count = item.CommodityNumber,
                        //    Price = item.RealPrice
                        //};
                        //templateCountList.Add(templateCountDto);

                        //var orderItemfreight = new CommoditySV().CalFreightMultiAppsByTextExt(orderSDTO.Province, orderSDTO.SelfTakeFlag, templateCountList).Freight;
                        //if (templateCountList.Count == 1)
                        //{
                        //    orderitem.FreightPrice = orderItemfreight;
                        //}
                        //else
                        //{
                        //    orderitem.FreightPrice = orderItemfreight - sumFreight;
                        //}
                        //sumFreight += (decimal)orderitem.FreightPrice;
                        //LogHelper.Debug(string.Format("1下订单 orderitem.FreightPrice :{0},订单项商品id：{1}", orderitem.FreightPrice, orderitem.CommodityId));

                        if (index < orderSDTO.ShoppingCartItemSDTO.Count - 1)
                        {
                            orderitem.FreightPrice = Math.Round(((item.RealPrice * item.CommodityNumber) / orderSDTO.Price) * freight, 2, MidpointRounding.AwayFromZero);
                            sumFreightLastPrice += (decimal)orderitem.FreightPrice;
                        }
                        else
                        {
                            orderitem.FreightPrice = Math.Round(freight - sumFreightLastPrice, 2, MidpointRounding.AwayFromZero);
                        }
                    }

                    //易捷币 商品使用的易捷币金额=下单时计算易捷币抵现时该商品抵用的额度 按订单中商品的顺序抵现（必须创建完订单 通过定时任务补充数据）
                    //易捷卡抵用金额拆分
                    if (orderSDTO.YjCardPrice > 0)
                    {
                        if (index < orderSDTO.ShoppingCartItemSDTO.Count - 1)
                        {
                            orderitem.YJCardPrice = Math.Round(((item.RealPrice * item.CommodityNumber) / orderSDTO.Price) * orderSDTO.YjCardPrice, 2, MidpointRounding.AwayFromZero);
                            sumYjcardLastPrice += (decimal)orderitem.YJCardPrice;
                        }
                        else
                        {
                            orderitem.YJCardPrice = Math.Round(orderSDTO.YjCardPrice - sumYjcardLastPrice, 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    index++;
                    #endregion

                    LogHelper.Debug(string.Format("1下订单 orderitem :{0}", JsonHelper.JsSerializer(orderitem)));

                    orderitem.State = 0;
                    contextSession.SaveObject(orderitem);

                    //保存方正与易捷关系数据
                    if (ThirdECommerceHelper.IsFangZheng((Guid)orderitem.AppId))
                        FangZhengSV.FangZheng_Order_Submit(contextSession, orderitem);
                    // 保存苏宁与易捷关系数据
                    SuningSV.suning_govbus_order_add(contextSession, orderitem, SnOrderItemList);

                    #region 添加赠品
                    // 查询赠品
                    if (item.Presents != null && item.Presents.Count > 0)
                    {
                        var tempCommodityStockId = orderitem.CommodityStockId ?? Guid.Empty;
                        var present = PresentPromotionCommodity.ObjectSet().Where(_ => _.CommodityId == orderitem.CommodityId &&
                                (_.CommoditySKUId == tempCommodityStockId || _.CommoditySKUId == orderitem.CommodityId))
                           .Join(PresentPromotion.ObjectSet().Where(_ => !_.IsEnd && _.BeginTime < now && now < _.EndTime),
                               pp => pp.PresentPromotionId,
                               ppc => ppc.Id,
                               (c, p) => new
                               {
                                   Commodity = c,
                                   PromotionId = p.Id,
                                   Limit = p.Limit,
                                   BeginTime = p.BeginTime,
                                   EndTime = p.EndTime
                               })
                                .FirstOrDefault();
                        if (present != null)
                        {
                            var limit = present.Limit ?? 1;
                            if (limit == 0) limit = 1;
                            {
                                limit = 1;
                            }
                            var gifts = PresentPromotionGift.ObjectSet().Where(_ => _.PresentPromotionId == present.PromotionId).ToList();
                            var giftCommodityIds = gifts.Select(_ => _.CommodityId).ToList();
                            var giftCommodityStockIds = gifts.Where(_ => _.CommoditySKUId != Guid.Empty).Select(_ => _.CommoditySKUId).ToList();
                            var giftCommodityStocks = CommodityStock.ObjectSet().Where(_ => giftCommodityStockIds.Contains(_.Id)).ToList();
                            // 查询商品商城品类
                            List<CommodityInnerCategory> giftCommodityInnerCategories = new List<CommodityInnerCategory>();
                            if (orderSDTO.AppId == Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId)
                            {
                                giftCommodityInnerCategories = CommodityInnerCategory.ObjectSet().Where(_ => _.AppId == Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId && giftCommodityIds.Contains(_.CommodityId)).ToList();
                            }
                            foreach (var presentDto in item.Presents)
                            {
                                var g = gifts.Where(_ => _.CommodityId == presentDto.CommodityId && _.CommoditySKUId == presentDto.CommodityStockId).FirstOrDefault();
                                if (g == null)
                                {
                                    return new OrderResultDTO { ResultCode = 1, Message = "赠品信息有误，请重新下单", CommodityId = g.CommodityId };
                                }
                                var needStock = orderitem.Number / limit * g.Number;
                                if (presentDto.Number > needStock)
                                {
                                    return new OrderResultDTO { ResultCode = 1, Message = "赠品库存不足，请重新下单", CommodityId = g.CommodityId };
                                }
                                needStock = presentDto.Number;
                                var tempCom = GetCommodity(item.AppId, g.CommodityId);
                                string tempSku = null;
                                Guid? tempSkuId = null;
                                if (g.CommoditySKUId != Guid.Empty)
                                {
                                    var giftCommodityStock = giftCommodityStocks.Find(_ => _.Id == g.CommoditySKUId);
                                    if (giftCommodityStock != null && giftCommodityStock.Stock >= needStock)
                                    {
                                        var skuObj = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(giftCommodityStock.ComAttribute);
                                        tempSku = string.Join(",", skuObj.Select(_ => _.SecondAttribute));
                                        tempSkuId = giftCommodityStock.Id;
                                    }
                                    else
                                    {
                                        return new OrderResultDTO { ResultCode = 1, Message = "赠品库存不足，请重新下单", CommodityId = g.CommodityId, CommodityStockId = g.CommoditySKUId };
                                    }
                                }
                                else
                                {
                                    if (tempCom.Stock < needStock)
                                    {
                                        return new OrderResultDTO { ResultCode = 1, Message = "赠品库存不足，请重新下单", CommodityId = g.CommodityId };
                                    }
                                }

                                var tempGiftCommodityInnerCategory = giftCommodityInnerCategories.Where(_ => _.CommodityId == g.CommodityId).ToList();
                                OrderItemPresent orderitemPresent = new OrderItemPresent
                                {
                                    Number = needStock,
                                    Code = orderCode,
                                    Name = tempCom.Name,
                                    CommodityAttributes = tempSku,
                                    CommodityStockId = tempSkuId,
                                    CommodityId = tempCom.Id,
                                    PicturesPath = tempCom.PicturesPath,
                                    //PromotionId = tempCom.PromotionId,
                                    Id = Guid.NewGuid(),
                                    CommodityOrderId = orderId,
                                    SubTime = now,
                                    CurrentPrice = tempCom.Price,
                                    SubId = item.UserId,
                                    //Intensity = tempCom.Intensity,
                                    //DiscountPrice = tempCom.DiscountPrice,
                                    RealPrice = 0,
                                    //ScorePrice = tempCom.ScorePrice,
                                    AlreadyReview = false,
                                    Duty = tempCom.Duty ?? 0,
                                    TaxRate = tempCom.TaxRate,
                                    InputRax = tempCom.InputRax,
                                    CostPrice = tempCom.CostPrice,
                                    Barcode = tempCom.Barcode,
                                    No_Code = tempCom.Code,
                                    InnerCatetoryIds = string.Join(",", tempGiftCommodityInnerCategory.Select(_ => _.CategoryId)),
                                    Unit = tempCom.Unit,
                                    Type = tempCom.Type,
                                    //YJCouponActivityId = comList.Where(x => x.Id == item.Id).Select(x => x.YJCouponActivityId).FirstOrDefault(),
                                    //YJCouponType = comList.Where(x => x.Id == item.Id).Select(x => x.YJCouponType).FirstOrDefault(),
                                    EntityState = EntityState.Added
                                };
                                orderitemPresent.OrderItemId = orderitem.Id;
                                contextSession.SaveObject(orderitemPresent);
                            }
                            orderitem.HasPresent = true;
                        }
                    }
                    #endregion
                }

                #endregion

                foreach (var commdity in orderSDTO.ShoppingCartItemSDTO)
                {

                    if (orderSDTO.RealPrice == 0)
                        break;

                    if (orderSDTO.RealPrice > commdity.StoreCouponDivide)
                    {
                        orderSDTO.RealPrice -= commdity.StoreCouponDivide;
                        orderSDTO.CouponValue += commdity.StoreCouponDivide;
                    }
                    else
                    {
                        orderSDTO.RealPrice = 0;
                        orderSDTO.CouponValue += commdity.StoreCouponDivide - orderSDTO.RealPrice;
                    }

                }//qgb

                #region MallApply

                string mallAppName = string.Empty;
                string supplierCode = string.Empty;
                string supplierName = string.Empty;
                short? supplierType = null;
                short? shipperType = null;
                short? mallAppType = null;
                var esAppIdList = new List<Guid> { CustomConfig.YJAppId };
                if (orderSDTO.EsAppId != Guid.Empty && orderSDTO.EsAppId != CustomConfig.YJAppId)
                {
                    esAppIdList.Add(orderSDTO.EsAppId);
                }
                var mallAppList = MallApply.ObjectSet()
                    .Where(p => esAppIdList.Contains(p.EsAppId) && p.AppId == orderSDTO.AppId && new int[] { 2, 4 }.Contains(p.State.Value))
                    .Select(p => new { p.AppName, p.Type, p.EsAppId }).ToList();
                if (mallAppList.Any(p => p.EsAppId == CustomConfig.YJAppId) && orderSDTO.EsAppId != CustomConfig.YJAppId)
                {
                    LogHelper.Error("易捷北京商家订单未生成到易捷北京商城：" + JsonHelper.JsonSerializer(orderSDTO));
                    orderSDTO.EsAppId = CustomConfig.YJAppId;
                }
                if (!mallAppList.Any() && orderSDTO.EsAppId == CustomConfig.YJAppId)
                {
                    return new OrderResultDTO { ResultCode = 1, Message = "提交订单有误" };
                }
                if (orderSDTO.EsAppId != Guid.Empty)
                {
                    var mallApp = MallApply.ObjectSet()
                        .Where(p => p.EsAppId == orderSDTO.EsAppId && p.AppId == orderAppId && new int[] { 2, 4 }.Contains(p.State.Value))
                          .Select(p => new { p.AppName, p.Type, p.EsAppId }).FirstOrDefault();
                    if (mallApp != null)
                    {
                        mallAppName = mallApp.AppName;
                        mallAppType = mallApp.Type;
                    }

                    var supplier = Supplier.ObjectSet()
                        .Where(p => p.EsAppId == orderSDTO.EsAppId && p.AppId == orderAppId && p.IsDel == false)
                        .Select(p => new { p.AppName, p.SupplierCode, p.SupplierName, p.SupplierType, p.ShipperType }).FirstOrDefault();
                    if (supplier != null)
                    {
                        supplierCode = supplier.SupplierCode;
                        supplierName = supplier.SupplierName;
                        supplierType = supplier.SupplierType;
                        shipperType = supplier.ShipperType;
                    }
                }

                #endregion

                commodityOrderDTO = new CommodityOrder
                {
                    Freight = freight,
                    SelfTakeFlag = orderSDTO.SelfTakeFlag,
                    Id = orderId,
                    Name = "用户订单",
                    Code = orderCode,
                    SubId = orderSDTO.UserId,
                    UserId = orderSDTO.UserId,
                    State = 0,
                    Payment = orderSDTO.Payment,
                    Price = orderSDTO.Price,
                    ReceiptAddress = orderSDTO.ReceiptAddress ?? "",
                    ReceiptPhone = orderSDTO.ReceiptPhone ?? "",
                    ReceiptUserName = orderSDTO.ReceiptUserName ?? "",
                    City = orderSDTO.City ?? "",
                    Province = orderSDTO.Province ?? "",
                    District = orderSDTO.District ?? "",
                    Street = orderSDTO.Street ?? "",
                    AppId = orderAppId,
                    Details = orderSDTO.Details,
                    IsModifiedPrice = false,
                    RecipientsZipCode = orderSDTO.RecipientsZipCode,
                    SrcTagId = orderSDTO.SrcTagId,
                    SrcType = orderSDTO.SrcType,
                    CPSId = orderSDTO.CPSId,
                    OrderType = orderSDTO.OrderType,
                    ServiceId = orderSDTO.ServiceId,
                    RealPrice = orderSDTO.RealPrice,
                    EsAppId = orderSDTO.EsAppId,
                    PicturesPath = orderSDTO.PicturesPath,
                    //餐盒费
                    MealBoxFee = orderSDTO.mealBoxFee,
                    Duty = orderSDTO.Duty == null ? 0 : (decimal)orderSDTO.Duty,
                    FirstContent = orderSDTO.FirstContent,
                    SecondContent = orderSDTO.SecondContent,
                    ThirdContent = orderSDTO.ThirdContent,
                    SetMealId = orderSDTO.SetMealId,
                    AppName = mallAppName,
                    SupplierCode = supplierCode,
                    SupplierName = supplierName,
                    SupplierType = supplierType,
                    ShipperType = shipperType,
                    AppType = mallAppType,
                    InviterMobile=orderSDTO.InviterMobile,
                    EntityState = EntityState.Added
                };

                //流水号，餐饮使用
                if (orderSDTO.OrderType == 2)
                {
                    commodityOrderDTO.Batch = CommodityOrder.GenerateOrderBatch(orderAppId);
                }

                if (orderSDTO.SpreadCode != null && orderSDTO.SpreadCode != Guid.Empty)
                {
                    commodityOrderDTO.SpreadCode = orderSDTO.SpreadCode;
                }

                #region 金彩金采团购活动 获取客户Id

                LogHelper.Debug("进入获取金彩金采团购活动orderSDTO.JcActivityId :" + orderSDTO.JcActivityId);
                if (orderSDTO.JcActivityId != null && orderSDTO.JcActivityId != Guid.Empty)
                {
                    commodityOrderDTO.JcActivityId = orderSDTO.JcActivityId;
                    commodityOrderDTO.Payment = 2001;//金采支付 
                    commodityOrderDTO.CustomerInfo = resSpender.ReturnValues["JczfCustomerId"];//xiexg
                }

                #endregion

                #region 拼团

                if (orderSDTO.PromotionType == 3)
                {
                    commodityOrderDTO.State = 17;

                    if (resSpender.ReturnValues["DiyGroupId"] != null)
                    {
                        result.DiyGroupId = resSpender.ReturnValues["DiyGroupId"];
                    }
                }

                #endregion

                #region 三级分销
                //判断用户是否分销商，如果不是：生成分销商信息，如果是：直接在订单上标注
                if (orderSDTO.DistributorId != Guid.Empty)
                {
                    commodityOrderDTO.DistributorId = orderSDTO.DistributorId;
                }
                else
                {
                    var disInfo = Distributor.ObjectSet().FirstOrDefault(t => t.UserId == orderSDTO.UserId && t.EsAppId == orderSDTO.EsAppId);
                    if (disInfo != null && disInfo.Id != Guid.Empty)
                    {
                        commodityOrderDTO.DistributorId = disInfo.ParentId;
                    }
                }

                #endregion

                #region 应用主、推广主分成计算

                //同时存在应用主、推广主，只给应用主分成
                if (orderSDTO.SrcAppId != Guid.Empty)
                {
                    commodityOrderDTO.SrcAppId = orderSDTO.SrcAppId;
                }
                else
                {
                    if (resSpender.ReturnValues["spreaderId"] != null)
                    {
                        result.DiyGroupId = resSpender.ReturnValues["spreaderId"];
                    }
                }

                #endregion

                #region 订单日志
                Journal journal = new Journal
                {
                    Id = Guid.NewGuid(),
                    Name = "用户创建订单",
                    Code = orderCode,
                    SubId = orderSDTO.UserId,
                    SubTime = now,
                    Details = "订单状态为" + commodityOrderDTO.State,
                    CommodityOrderId = orderId,
                    StateTo = commodityOrderDTO.State,
                    IsPush = false,
                    OrderType = orderSDTO.OrderType,
                    EntityState = System.Data.EntityState.Added
                };
                contextSession.SaveObject(journal);

                #endregion

                commodityOrderDTO.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(commodityOrderDTO);
                int rowCount = contextSession.SaveChanges();
                if (rowCount <= 0)
                {
                    //异步回滚资源。相当于未付款的订单被取消，但是此处CommodityOrder还没有成功。
                    result.ResultCode = (int)BTP.Deploy.Enum.ReturnCodeEnum.SubmitOrderFail;
                    result.Message = BTP.Deploy.Enum.ReturnCodeEnum.SubmitOrderFail.GetDescription();
                    RollbackOrderResource(commodityOrderDTO, orderItems);
                    return result;
                }

                if (todayPromotionList.Any())
                {
                    todayPromotionList.ForEach(c => c.RefreshCache(EntityState.Modified));
                }

                if (comproDict.Any())
                {
                    GlobalCacheWrapper.Add("G_OrderSecPro:" + commodityOrderDTO.AppId, commodityOrderDTO.Id.ToString(), "1", CacheTypeEnum.redisSS, "BTPCache");
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("订单提交到数据库服务器异常。orderSDTO：{0}，Ex：{1}", JsonHelper.JsonSerializer(orderSDTO), ex));
                RollbackOrderResource(commodityOrderDTO, orderItems);
                result.ResultCode = (int)BTP.Deploy.Enum.ReturnCodeEnum.SubmitOrderException;
                result.Message = BTP.Deploy.Enum.ReturnCodeEnum.SubmitOrderFail.GetDescription();
                return result;
            }

            //异步保存自提订单提货信息
            if (commodityOrderDTO.SelfTakeFlag == 1 && appSelfTakeStation != null)
            {
                System.Threading.ThreadPool.QueueUserWorkItem(a => saveOrderPickUp(commodityOrderDTO, appSelfTakeStation, orderSDTO.AppOrderPickUpInfo));
            }
            #endregion

            appIdRealPrice = new Tuple<Guid, decimal>(commodityOrderDTO.AppId, commodityOrderDTO.RealPrice ?? 0);


            if (commodityOrderDTO.RealPrice <= 0
                && (orderSDTO.MainOrderId == null || orderSDTO.MainOrderId == Guid.Empty))
            {
                NoThirdPartPay(commodityOrderDTO);
            }
            else if (!isMixPay && orderSDTO.Payment == 0 && orderSDTO.RealPrice > 0 && orderSDTO.GoldPrice + orderSDTO.PayCouponValue >= commodityOrderDTO.RealPrice)
            {
                #region 全金币支付 金币足够支付整个订单时才在此用金币支付，否则金币会在fsp里处理。
                ContextDTO contextDTO = this.ContextDTO;
                if (contextDTO == null)
                {
                    contextDTO = Jinher.AMP.BTP.Common.AuthorizeHelper.CoinInitAuthorizeInfo();
                }

                Jinher.AMP.App.Deploy.CustomDTO.AppIdOwnerIdTypeDTO appDTO = APPSV.Instance.GetAppOwnerInfo(commodityOrderDTO.AppId, contextDTO);

                if (appDTO == null || appDTO.OwnerId == Guid.Empty)
                {
                    result.ResultCode = 2;
                    result.Message = "获取收款人异常";
                    result.OrderCode = orderCode;
                    result.OrderId = orderId;
                    result.Freight = freight;
                    return result;
                }

                string payorComment = string.Format("电商{0}", orderSDTO.ShoppingCartItemSDTO[0].Name);
                string notifyUrl = string.Format("{0}PaymentNotify/Goldpay", CustomConfig.BtpDomain);
                PayOrderGoldDTO payOrderDTO = new PayOrderGoldDTO
                {
                    AppId = commodityOrderDTO.AppId,
                    BizId = commodityOrderDTO.Id,
                    CouponCodes = orderSDTO.PaycouponCodes,
                    CouponCount = (double)orderSDTO.PayCouponValue,
                    Gold = (long)(orderSDTO.GoldPrice * 1000),
                    TotalCount = (long)(commodityOrderDTO.RealPrice * 1000),
                    UsageId = CustomConfig.ShareGoldAccout.BTPGlodUsageId
                };
                payOrderDTO.PayeeComment = payOrderDTO.PayorComment = payorComment;
                payOrderDTO.Password = orderSDTO.PayPassword;
                payOrderDTO.NotifyUrl = notifyUrl;
                payOrderDTO.PayeeId = appDTO.OwnerId;
                LogHelper.Info(string.Format(@"全金币支付接口参数:payOrderDTO.AppId:{0},payOrderDTO.BizId:{1},payOrderDTO.CouponCodes:{2},payOrderDTO.CouponCount:{3},payOrderDTO.Gold:{4},
            payOrderDTO.TotalCount:{5},payOrderDTO.PayeeId:{6}", payOrderDTO.AppId, payOrderDTO.BizId, payOrderDTO.CouponCodes, payOrderDTO.CouponCount,
                payOrderDTO.Gold, payOrderDTO.TotalCount, payOrderDTO.PayeeId));
                try
                {
                    LogHelper.Info(string.Format("误入PayByPayeeIdBatch，payOrderDTO:{0},orderSDTO:{1},isMixPay:{2},commodityOrderDTO.RealPrice:{3}",
                        JsonHelper.JsonSerializer(payOrderDTO), JsonHelper.JsonSerializer(orderSDTO), isMixPay, commodityOrderDTO.RealPrice));

                    FSP.Deploy.CustomDTO.ReturnInfoDTO<long> goldResult = Jinher.AMP.BTP.TPS.FSPSV.Instance.PayByPayeeIdBatch(payOrderDTO);
                    if (goldResult.Code == 0)
                    {
                        result.ResultCode = 3;
                        result.Message = "金币支付成功";
                        result.OrderCode = orderCode;
                        result.OrderId = orderId;
                        result.Freight = freight;
                        return result;
                    }
                    else
                    {
                        result.ResultCode = 2;
                        result.Message = goldResult.Message;
                        result.OrderCode = orderCode;
                        result.OrderId = orderId;
                        result.Freight = freight;
                        LogHelper.Error(string.Format("全金币支付接口异常。orderId：{0}，Message：{1}", commodityOrderDTO.Id, goldResult.Message));
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    string errStack = ex.Message + ex.StackTrace;
                    while (ex.InnerException != null)
                    {
                        errStack += ex.InnerException.Message + ex.InnerException.StackTrace;
                        ex = ex.InnerException;
                    }
                    LogHelper.Error(string.Format("全金币支付接口异常。orderId：{0}，Message：{1}，StackTrace：{2}",
                        commodityOrderDTO.Id, ex.Message, errStack));
                    result.ResultCode = 2;
                    result.Message = "金币支付异常";
                    result.OrderCode = orderCode;
                    result.OrderId = orderId;
                    result.Freight = freight;
                    return result;
                }
                #endregion
            }
            else if (orderSDTO.Payment == 1)//货到付款
            {
                var hdfkResult = UpdateCommodityOrderExt(1, commodityOrderDTO.Id, commodityOrderDTO.UserId, commodityOrderDTO.AppId, 1, "", "");
                if (hdfkResult.ResultCode == 0)
                {
                    result.ResultCode = 3;
                    result.Message = "货到付款支付成功";
                    result.OrderCode = orderCode;
                    result.OrderId = orderId;
                    result.Freight = freight;
                    return result;
                }
                else
                {
                    result.ResultCode = 2;
                    result.Message = hdfkResult.Message;
                    result.OrderCode = orderCode;
                    result.OrderId = orderId;
                    result.Freight = freight;
                    LogHelper.Error(string.Format("货到付款支付接口异常。orderId：{0}，Message：{1}", commodityOrderDTO.Id, hdfkResult.Message));
                    return result;
                }
            }
            else if (!isMultiPay)
            {
                CreateOrderToFspDTO ceateOrderToFspDTO = new Deploy.CustomDTO.CreateOrderToFspDTO();
                ceateOrderToFspDTO.GoldCoupon = orderSDTO.PayCouponValue;
                ceateOrderToFspDTO.GoldCouponIds = orderSDTO.PaycouponCodes;
                ceateOrderToFspDTO.GoldPrice = orderSDTO.GoldPrice;
                ceateOrderToFspDTO.OrderId = commodityOrderDTO.Id;
                ceateOrderToFspDTO.RealPrice = commodityOrderDTO.RealPrice.Value;
                ceateOrderToFspDTO.Source = orderSDTO.Source;
                ceateOrderToFspDTO.SrcType = commodityOrderDTO.SrcType.Value;
                ceateOrderToFspDTO.WxOpenId = orderSDTO.WxOpenId;
                ceateOrderToFspDTO.FirstCommodityName = orderSDTO.ShoppingCartItemSDTO[0].Name;
                ceateOrderToFspDTO.AppId = orderSDTO.AppId;
                ceateOrderToFspDTO.EsAppId = orderSDTO.EsAppId;
                ceateOrderToFspDTO.ExpireTime = result.ExpireTime;
                ceateOrderToFspDTO.TradeType = orderSDTO.TradeType;
                ceateOrderToFspDTO.DiyGroupId = orderSDTO.DiyGroupId;
                ceateOrderToFspDTO.ShareId = orderSDTO.ShareId;
                ceateOrderToFspDTO.DealType = 0;
                ceateOrderToFspDTO.IsWeixinPay = orderSDTO.IsWeixinPay;
                ceateOrderToFspDTO.OrderType = orderSDTO.OrderType;
                ceateOrderToFspDTO.Scheme = orderSDTO.Scheme;
                ceateOrderToFspDTO.JcActivityId = orderSDTO.JcActivityId;

                if (orderSDTO.EsAppId != Guid.Empty)
                {
                    ceateOrderToFspDTO.TradeType = Jinher.AMP.BTP.TPS.FSPSV.GetTradeSettingInfo(orderSDTO.EsAppId);

                    if (orderSDTO.EsAppId == YJB.Deploy.CustomDTO.YJBConsts.YJAppId && ceateOrderToFspDTO.TradeType != 1)
                    {
                        LogHelper.Error("CommodityOrder.SubmitOrderCommoe 易捷商城支付类型错误，TradeType:" + ceateOrderToFspDTO.TradeType);
                    }
                }

                payUrl = OrderSV.GetCreateOrderFSPUrl(ceateOrderToFspDTO, this.ContextDTO);
            }

            result.PayUrl = payUrl;
            result.ResultCode = 0;
            result.Message = "Success";
            result.OrderCode = orderCode;
            result.OrderId = orderId;
            result.Freight = freight;
            return result;
        }

        /// <summary>
        /// 下订单完成对不需要第三放支付的订单进行处理
        /// </summary>
        /// <param name="commodityOrderDTO"></param>
        private void NoThirdPartPay(CommodityOrder commodityOrderDTO)
        {
            LogHelper.Debug(string.Format("进入NoThirdPartPay，订单Id:{0}", commodityOrderDTO.Id));
            var rdto = PayUpdateCommodityOrderExt(commodityOrderDTO.Id, commodityOrderDTO.UserId, commodityOrderDTO.AppId, 0, 0, 0, 0);
            if (rdto != null && rdto.ResultCode == 0)
            {
                #region 易捷卡密订单
                if (commodityOrderDTO.OrderType == 3)
                {
                    System.Threading.Tasks.Task.Factory.StartNew(() =>
                    {
                        const string message = "易捷卡密订单调用盈科接口生成卡信息:";
                        var rdto1 = new IBP.Facade.YJBJCardFacade().Create(commodityOrderDTO.Id);
                        if (rdto1.isSuccess) LogHelper.Info(message + rdto1.Message);
                        else LogHelper.Error(message + rdto1.Message);
                    });
                }
                #endregion
                #region 进销存订单
                if (commodityOrderDTO.AppType.HasValue && new List<short> { 2, 3 }.Contains(commodityOrderDTO.AppType.Value))
                {
                    new IBP.Facade.JdEclpOrderFacade().CreateOrder(commodityOrderDTO.Id, string.Empty);
                    new IBP.Facade.JdEclpOrderFacade().SendPayInfoToHaiXin(commodityOrderDTO.Id);
                }
                #endregion
                new IBP.Facade.CommodityOrderFacade().SendPayInfoToYKBDMq(commodityOrderDTO.Id);//盈科大数据mq
                YXOrderHelper.CreateOrder(commodityOrderDTO.Id);//网易严选订单
            }
        }


        /// <summary>
        /// 回退订单资源
        /// </summary>
        /// <param name="commodityOrder">订单信息</param>
        /// <param name="orderitemlist">订单项列表</param>
        /// <returns></returns>
        private ResultDTO RollbackOrderResource(CommodityOrder commodityOrder, List<OrderItem> orderitemlist)
        {
            ResultDTO result = new ResultDTO();

            List<Commodity> needRefreshCacheCommoditys = new List<Commodity>();
            List<TodayPromotion> needRefreshCacheTodayPromotions = new List<TodayPromotion>();

            //解冻金币
            if (commodityOrder.Payment != 1 && commodityOrder.RealPrice > 0)
            {
                FSPSV.Instance.UnFreezeGold(commodityOrder.Id);
            }
            //回退积分
            SignSV.CommodityOrderCancelSrore(ContextFactory.CurrentThreadContext, commodityOrder);

            // 回退易捷币
            YJBHelper.CancelOrder(ContextFactory.CurrentThreadContext, commodityOrder);

            //回退优惠券
            CouponSV.RefundCoupon(ContextFactory.CurrentThreadContext, commodityOrder);

            //撤回活动资源
            List<Tuple<string, string, int>> resTuple = orderitemlist.Select(oi => new Tuple<string, string, int>(oi.PromotionId.ToString(), oi.CommodityId.ToString(), oi.Number)).ToList();
            rollbackPromotion(resTuple, commodityOrder.UserId, commodityOrder.Id);

            //撤销易捷卡消费
            decimal yjCardPrice = (commodityOrder.YJCardPrice != null && commodityOrder.YJCardPrice.HasValue) ? commodityOrder.YJCardPrice.Value : 0;
            YJBHelper.RetreatYjc(commodityOrder.UserId, yjCardPrice, commodityOrder.Id, Guid.Empty);
            

            if (needRefreshCacheCommoditys.Any())
            {
                needRefreshCacheCommoditys.ForEach(c => c.RefreshCache(EntityState.Modified));
            }
            if (needRefreshCacheTodayPromotions.Any())
            {
                needRefreshCacheTodayPromotions.ForEach(c => c.RefreshCache(EntityState.Modified));
            }

            return result;
        }

        #endregion

        /// <summary>
        /// 生成订单（好运来下订单使用）
        /// </summary>
        /// <param name="orderSDTO">订单实体</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.OrderResultDTO SavePrizeCommodityOrderExt(Jinher.AMP.BTP.Deploy.CustomDTO.OrderSDTO orderSDTO)
        {
            OrderQueueDTO orderQueueDTO = new OrderQueueDTO();
            //在线支付包括支付宝与U付时,要跳转到的支付页面
            string payUrl = string.Empty;
            //如果没有商品则不允许
            if (orderSDTO.ShoppingCartItemSDTO == null || orderSDTO.ShoppingCartItemSDTO.Count == 0)
            {
                return new OrderResultDTO { ResultCode = 1, Message = "订单商品不能为空" };
            }
            var commodityDTO = orderSDTO.ShoppingCartItemSDTO[0];

            GenUserPrizeRecord prizeResultRecord = (from e in GenUserPrizeRecord.ObjectSet()
                                                    where e.CommodityId == commodityDTO.Id && e.UserId == orderSDTO.UserId && e.PromotionId == orderSDTO.SrcTagId
                                                    select e).FirstOrDefault();

            if (prizeResultRecord == null)
            {
                return new OrderResultDTO { ResultCode = 1, Message = "您没有权限购买此商品" };
            }
            if (prizeResultRecord.IsBuyed)
            {
                return new OrderResultDTO { ResultCode = 1, Message = "您已经购买过此商品，现在没有权限购买此商品" };
            }
            if (prizeResultRecord.Price != orderSDTO.Price)
            {
                LogHelper.Info(string.Format("好运来订单：prizeResultRecord.Price：{0},orderSDTO.Price:{1}", prizeResultRecord.Price, orderSDTO.Price));
                return new OrderResultDTO { ResultCode = 1, Message = "您不能以此价格购买此商品" };
            }
            ContextSession contextSession = ContextFactory.CurrentThreadContext;

            #region 删除曾经下过的未付款的订单
            var orderItems = (from o in OrderItem.ObjectSet()
                              join c in CommodityOrder.ObjectSet() on o.CommodityOrderId equals c.Id
                              where o.CommodityId == prizeResultRecord.CommodityId && c.SrcTagId == orderSDTO.SrcTagId
                              && c.UserId == orderSDTO.UserId
                              select c).Distinct().ToList();
            foreach (var orderItem in orderItems)
            {

                orderItem.State = 4;
                orderItem.EntityState = System.Data.EntityState.Modified;
                contextSession.SaveObject(orderItem);

            }
            #endregion
            OrderResultDTO result = new OrderResultDTO();
            //订单ID
            Guid ids;
            //订单号
            string code = "";
            try
            {
                DateTime date = DateTime.Now;
                Random rd = new Random();
                ids = Guid.NewGuid();
                code = date.Year.ToString() + date.Month.ToString("d2") + date.Day.ToString("d2");
                code += date.Hour.ToString("d2") + date.Minute.ToString("d2") + date.Second.ToString("d2");
                code += rd.Next(1000, 9999).ToString();
                // 更新京东订单ID
                JdOrderHelper.UpdateJdOrderId(contextSession, Guid.Empty.ToString(), ids.ToString(), orderSDTO.AppId.ToString(), orderSDTO.UserId.ToString());

                List<OrderListItemCDTO> orderItemsCache = new List<OrderListItemCDTO>();
                List<TemplateCountDTO> templateCountList = new List<TemplateCountDTO>();

                //遍历购买商品列表
                foreach (var item in orderSDTO.ShoppingCartItemSDTO)
                {
                    //运费计算使用
                    templateCountList.Add(new TemplateCountDTO { CommodityId = item.Id, Count = item.CommodityNumber, Price = item.Price });
                    //删除购物车
                    if (!string.IsNullOrEmpty(item.ShopCartItemId.ToString()))
                    {
                        Guid ShopCartItemId = new Guid(item.ShopCartItemId.ToString());
                        ShoppingCartItems shop = new ShoppingCartItems();
                        shop = ShoppingCartItems.ObjectSet().Where(n => n.Id == ShopCartItemId).FirstOrDefault();
                        if (shop != null)
                        {
                            shop.EntityState = System.Data.EntityState.Deleted;
                            contextSession.Delete(shop);
                        }
                    }

                    //添加订单商品
                    OrderItem orderitem = new OrderItem
                    {
                        Number = item.CommodityNumber,
                        Code = code,
                        Name = item.Name,
                        CommodityAttributes = item.SizeAndColorId,
                        CommodityId = item.Id,
                        PicturesPath = item.Pic,
                        PromotionId = item.PromotionId,
                        Id = Guid.NewGuid(),
                        CommodityOrderId = ids,
                        SubTime = date,
                        CurrentPrice = item.Price,
                        SubId = item.UserId,
                        Intensity = item.Intensity
                    };

                    orderitem.AlreadyReview = false;
                    orderitem.EntityState = System.Data.EntityState.Added;
                    contextSession.SaveObject(orderitem);

                    orderItemsCache.Add(new OrderListItemCDTO
                    {
                        Id = orderitem.Id,
                        OrderId = orderitem.CommodityOrderId,
                        Pic = orderitem.PicturesPath,
                        Name = orderitem.Name,
                        Price = orderitem.CurrentPrice,
                        CommodityNumber = orderitem.Number,
                        Size = orderitem.CommodityAttributes,
                        HasReview = orderitem.AlreadyReview,
                        Intensity = (decimal)orderitem.Intensity,
                        DiscountPrice = (decimal)(orderitem.DiscountPrice != null ? orderitem.DiscountPrice : -1),
                        CommodityId = orderitem.CommodityId
                    });
                }

                //运费计算
                decimal freightDec = decimal.Zero;
                if (orderSDTO.SelfTakeFlag == 0)
                    freightDec = new CommoditySV().CalFreightMultiAppsByTextExt(orderSDTO.Province, orderSDTO.SelfTakeFlag, templateCountList, null, null, null).Freight;

                #region 订单
                CommodityOrder commodityOrderDTO = new CommodityOrder();
                //运费
                commodityOrderDTO.Freight = freightDec;
                commodityOrderDTO.Id = ids;
                commodityOrderDTO.Name = "用户订单";
                commodityOrderDTO.Code = code;
                commodityOrderDTO.SubId = orderSDTO.UserId;
                commodityOrderDTO.UserId = orderSDTO.UserId;
                commodityOrderDTO.SubTime = date;
                commodityOrderDTO.State = 0;
                commodityOrderDTO.Payment = orderSDTO.Payment;
                commodityOrderDTO.RealPrice = (orderSDTO.Price + freightDec);//付款总价
                commodityOrderDTO.Price = orderSDTO.Price; //订单总价
                commodityOrderDTO.ReceiptAddress = orderSDTO.ReceiptAddress;
                commodityOrderDTO.ReceiptPhone = orderSDTO.ReceiptPhone;
                commodityOrderDTO.ReceiptUserName = orderSDTO.ReceiptUserName;
                commodityOrderDTO.City = orderSDTO.City;
                commodityOrderDTO.Province = orderSDTO.Province;
                commodityOrderDTO.District = orderSDTO.District;
                commodityOrderDTO.Street = orderSDTO.Street;
                commodityOrderDTO.AppId = orderSDTO.AppId;
                commodityOrderDTO.Details = orderSDTO.Details;
                commodityOrderDTO.IsModifiedPrice = true;
                commodityOrderDTO.ModifiedOn = date;
                commodityOrderDTO.RecipientsZipCode = orderSDTO.RecipientsZipCode;
                commodityOrderDTO.EntityState = System.Data.EntityState.Added;
                commodityOrderDTO.SrcTagId = orderSDTO.SrcTagId;
                commodityOrderDTO.SrcType = 1;//好运来
                commodityOrderDTO.PicturesPath = orderSDTO.PicturesPath;
                commodityOrderDTO.FirstContent = orderSDTO.FirstContent;
                commodityOrderDTO.SecondContent = orderSDTO.SecondContent;
                commodityOrderDTO.ThirdContent = orderSDTO.ThirdContent;
                commodityOrderDTO.InviterMobile = orderSDTO.InviterMobile;
                contextSession.SaveObject(commodityOrderDTO);
                #endregion

                prizeResultRecord.OrderId = commodityOrderDTO.Id;
                prizeResultRecord.EntityState = System.Data.EntityState.Modified;
                //prizeResultRecord.IsBuyed = true;
                contextSession.SaveObject(prizeResultRecord);

                #region 地址
                int address = DeliveryAddress.ObjectSet()
                    .Where(n => n.UserId == orderSDTO.UserId && n.AppId == orderSDTO.AppId)
                    .Count();
                //地址不存在，则添加新地址
                if (address == 0)
                {
                    DeliveryAddress deliveryAddressDTO = new DeliveryAddress();
                    deliveryAddressDTO.Id = Guid.NewGuid();
                    deliveryAddressDTO.Code = commodityOrderDTO.Code;
                    deliveryAddressDTO.Name = "用户地址";
                    deliveryAddressDTO.RecipientsAddress = orderSDTO.ReceiptAddress;
                    deliveryAddressDTO.RecipientsPhone = orderSDTO.ReceiptPhone;
                    deliveryAddressDTO.RecipientsUserName = orderSDTO.ReceiptUserName;
                    deliveryAddressDTO.City = orderSDTO.City;
                    deliveryAddressDTO.Province = orderSDTO.Province;
                    deliveryAddressDTO.District = orderSDTO.District;
                    deliveryAddressDTO.Street = orderSDTO.Street;
                    deliveryAddressDTO.UserId = orderSDTO.UserId;
                    deliveryAddressDTO.AppId = orderSDTO.AppId;
                    deliveryAddressDTO.SubTime = date;
                    deliveryAddressDTO.RecipientsZipCode = orderSDTO.RecipientsZipCode;
                    deliveryAddressDTO.EntityState = System.Data.EntityState.Added;
                    contextSession.SaveObject(deliveryAddressDTO);
                }
                #endregion

                #region 发票
                InvoiceDTO invDto = orderSDTO.InvoiceInfo;
                Invoice invoice = Invoice.CreateInvoice();
                invoice = invoice.FromEntityData(invDto);
                invoice.Id = Guid.NewGuid();
                invoice.EntityState = System.Data.EntityState.Added;
                invoice.SubTime = DateTime.Now;
                invoice.ModifiedOn = invoice.SubTime;
                invoice.State = 0;
                invoice.CommodityOrderId = commodityOrderDTO.Id;
                contextSession.SaveObject(invoice);
                #endregion

                try
                {
                    //订单日志
                    Journal journal = new Journal();
                    journal.Id = Guid.NewGuid();
                    journal.Name = "用户创建中奖订单";
                    journal.Code = code;
                    journal.SubId = orderSDTO.UserId;
                    journal.SubTime = date;
                    journal.Details = "订单状态为" + commodityOrderDTO.State;
                    journal.StateTo = commodityOrderDTO.State;
                    journal.IsPush = false;
                    journal.OrderType = commodityOrderDTO.OrderType;
                    journal.CommodityOrderId = ids;
                    journal.EntityState = System.Data.EntityState.Added;
                    contextSession.SaveObject(journal);
                    contextSession.SaveChanges();

                    OrderListCDTO commodityOrderSDTO = new OrderListCDTO()
                    {
                        CommodityOrderId = commodityOrderDTO.Id,
                        Price = commodityOrderDTO.Price,
                        AppId = commodityOrderDTO.AppId,
                        UserId = commodityOrderDTO.UserId,
                        State = commodityOrderDTO.State,
                        Freight = commodityOrderDTO.Freight,
                        OriginPrice = commodityOrderDTO.Price + commodityOrderDTO.Freight,
                        ShoppingCartItemSDTO = orderItemsCache
                    };

                    Dictionary<Guid, OrderListCDTO> dicCacheOrders = GlobalCacheWrapper.GetData("G_Order", commodityOrderDTO.UserId.ToString(), CacheTypeEnum.redisSS, "BTPCache") as Dictionary<Guid, OrderListCDTO>;
                    if (dicCacheOrders == null)
                    {
                        dicCacheOrders = new Dictionary<Guid, OrderListCDTO>();
                    }
                    if (!dicCacheOrders.ContainsKey(commodityOrderSDTO.CommodityOrderId))
                    {
                        dicCacheOrders.Add(commodityOrderSDTO.CommodityOrderId, commodityOrderSDTO);
                    }
                    GlobalCacheWrapper.Add("G_Order", commodityOrderDTO.UserId.ToString(), dicCacheOrders, CacheTypeEnum.redisSS, "BTPCache");
                    //删除缓存
                    JdOrderHelper.DeleteRedisJdPOrder(commodityOrderDTO.UserId.ToString());
                }
                catch (Exception ex)
                {
                    LogHelper.Error(string.Format("中奖订单提交到数据库服务器异常。orderSDTO：{0}", JsonHelper.JsonSerializer(orderSDTO)), ex);
                    result.ResultCode = 1;
                    result.Message = "提交订单异常";
                    return result;
                }

                #region 全金币支付
                if (orderSDTO.Payment == 0 && orderSDTO.GoldPrice >= commodityOrderDTO.RealPrice)
                {
                    ContextDTO contextDTO = this.ContextDTO;//Jinher.AMP.BTP.Common.AuthorizeHelper.CoinInitAuthorizeInfo();
                    if (contextDTO == null)
                    {
                        contextDTO = Jinher.AMP.BTP.Common.AuthorizeHelper.CoinInitAuthorizeInfo();
                    }
                    Jinher.AMP.App.Deploy.CustomDTO.AppIdOwnerIdTypeDTO appDTO = APPSV.Instance.GetAppOwnerInfo(commodityOrderDTO.AppId, contextDTO);

                    if (appDTO == null || appDTO.OwnerId == Guid.Empty)
                    {
                        result.ResultCode = 2;
                        result.Message = "获取收款人异常";
                        return result;
                    }

                    string payorComment = string.Format("电商{0}", orderSDTO.ShoppingCartItemSDTO[0].Name);
                    string notifyUrl = string.Format("{0}PaymentNotify/Goldpay", CustomConfig.BtpDomain);
                    try
                    {
                        FSP.Deploy.CustomDTO.ReturnInfoDTO<long> goldResult = Jinher.AMP.BTP.TPS.FSPSV.Instance.PayByPayeeId(commodityOrderDTO.Id, appDTO.OwnerId, (ulong)(commodityOrderDTO.RealPrice * 1000), payorComment, payorComment, orderSDTO.PayPassword, commodityOrderDTO.AppId, notifyUrl);
                        if (goldResult.Code == 0)
                        {
                            result.ResultCode = 3;
                            result.Message = "金币支付成功";
                            result.OrderCode = code;
                            result.OrderId = ids;
                            result.Freight = freightDec;
                            return result;
                        }
                        else
                        {
                            result.ResultCode = 2;
                            result.Message = goldResult.Message;
                            result.OrderCode = code;
                            result.OrderId = ids;
                            result.Freight = freightDec;
                            LogHelper.Error(string.Format("全金币支付接口异常。outTradeId{0}，payeeId{1}，gold{2}，payorComment{3}，payeeComment{4}，password{5}，appId{6}，notifyUrl{7}，Message：{8}", commodityOrderDTO.Id, appDTO.OwnerId, (ulong)(commodityOrderDTO.RealPrice * 1000), payorComment, payorComment, orderSDTO.PayPassword, commodityOrderDTO.AppId, notifyUrl, goldResult.Message));
                            return result;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(string.Format("全金币支付接口异常。outTradeId{0}，payeeId{1}，gold{2}，payorComment{3}，payeeComment{4}，password{5}，appId{6}，notifyUrl{7}", commodityOrderDTO.Id, appDTO.OwnerId, (ulong)(commodityOrderDTO.RealPrice * 1000), payorComment, payorComment, orderSDTO.PayPassword, commodityOrderDTO.AppId, notifyUrl), ex);
                        result.ResultCode = 2;
                        result.Message = "金币支付异常";
                        result.OrderCode = code;
                        result.OrderId = ids;
                        result.Freight = freightDec;
                        return result;
                    }
                }
                else if (orderSDTO.Payment == 1)//货到付款
                {
                    var hdfkResult = UpdateCommodityOrderExt(1, commodityOrderDTO.Id, commodityOrderDTO.UserId, commodityOrderDTO.AppId, 1, "", "");
                    if (hdfkResult.ResultCode == 0)
                    {
                        result.ResultCode = 3;
                        result.Message = "货到付款支付成功";
                        result.OrderCode = code;
                        result.OrderId = ids;
                        result.Freight = freightDec;
                        return result;
                    }
                    else
                    {
                        result.ResultCode = 2;
                        result.Message = hdfkResult.Message;
                        result.OrderCode = code;
                        result.OrderId = ids;
                        result.Freight = freightDec;
                        LogHelper.Error(string.Format("货到付款支付接口异常。state{0}，orderId{1}，userId{2}，appId{3}，payment{4}，goldpwd{5}，remessage{6}，Message：{7}", 1, commodityOrderDTO.Id, commodityOrderDTO.UserId, commodityOrderDTO.AppId, 1, "", "", hdfkResult.Message));
                        return result;
                    }
                }
                else
                {
                    CreateOrderToFspDTO ceateOrderToFspDTO = new Deploy.CustomDTO.CreateOrderToFspDTO();
                    ceateOrderToFspDTO.GoldCoupon = orderSDTO.PayCouponValue;
                    ceateOrderToFspDTO.GoldCouponIds = orderSDTO.PaycouponCodes;
                    ceateOrderToFspDTO.GoldPrice = orderSDTO.GoldPrice;
                    ceateOrderToFspDTO.OrderId = commodityOrderDTO.Id;
                    ceateOrderToFspDTO.RealPrice = commodityOrderDTO.RealPrice.Value;
                    ceateOrderToFspDTO.Source = orderSDTO.Source;
                    ceateOrderToFspDTO.SrcType = commodityOrderDTO.SrcType.Value;
                    ceateOrderToFspDTO.WxOpenId = orderSDTO.WxOpenId;
                    ceateOrderToFspDTO.FirstCommodityName = orderSDTO.ShoppingCartItemSDTO[0].Name;
                    ceateOrderToFspDTO.AppId = orderSDTO.AppId;
                    ceateOrderToFspDTO.EsAppId = orderSDTO.EsAppId;
                    ceateOrderToFspDTO.ExpireTime = result.ExpireTime;
                    ceateOrderToFspDTO.TradeType = orderSDTO.TradeType;
                    ceateOrderToFspDTO.DealType = 0;
                    ceateOrderToFspDTO.IsWeixinPay = orderSDTO.IsWeixinPay;
                    ceateOrderToFspDTO.OrderType = orderSDTO.OrderType;
                    payUrl = OrderSV.GetCreateOrderFSPUrl(ceateOrderToFspDTO, this.ContextDTO);
                }

                #endregion

                result.PayUrl = payUrl;
                result.ResultCode = 0;
                result.Message = "Success";
                result.OrderCode = code;
                result.OrderId = ids;
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("提交订单异常异常。orderSDTO：{0}", orderSDTO), ex);
                return new OrderResultDTO { ResultCode = 1, Message = "Error" };
            }

        }

        /// <summary>
        /// 先生成一个失败订单，各步骤记录消费的资源，若有某一步消费失败，统一回滚资源。
        /// </summary>
        /// <param name="orderId">订单Id</param>
        /// <param name="orderCode">订单编号</param>
        /// <param name="orderSDTO">订单其他信息</param>
        private void CreatePreOrder(Guid orderId, string orderCode, OrderSDTO orderSDTO)
        {
            //先生成一个失败订单，各步骤记录消费的资源，若有某一步消费失败，统一回滚资源。
            ErrorCommodityOrder ecoPre = ErrorCommodityOrder.CreateErrorCommodityOrder();
            ecoPre.ErrorOrderId = orderId;
            ecoPre.ResourceType = -100;
            ecoPre.Source = 0;
            ecoPre.State = 0;
            ecoPre.AppId = orderSDTO.EsAppId;
            ecoPre.UserId = orderSDTO.UserId;
            ecoPre.OrderCode = orderCode;
            ecoPre.CouponId = Guid.Empty;
            ecoPre.Score = 0;
            ecoPre.SubTime = DateTime.Now;
            ecoPre.ModifiedOn = ecoPre.SubTime;
            ContextFactory.CurrentThreadContext.SaveObject(ecoPre);
            ContextFactory.CurrentThreadContext.SaveChanges();
        }


        /// <summary>
        /// 订单状态修改
        /// </summary>
        /// <param name="state">订单状态:未付款=0，未发货=1，已发货=2，确认收货=3，删除=4</param>
        /// <param name="orderId">订单Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="appId">appId</param>
        /// <param name="payment">付款方式:金币=0，到付=1，支付宝=2</param>
        /// <returns></returns>
        [Obsolete("已过时，请调用UpdateCommodityOrderNewExt", false)]
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateCommodityOrderExt(int state, System.Guid orderId,
            System.Guid userId, System.Guid appId, int payment, string goldpwd, string remessage)
        {
            ResultDTO result = new ResultDTO();
            result.ResultCode = 0;
            result.Message = "Success";

            LogHelper.Info(string.Format("CommodityOrderSV:支付方式：{0},订单状态:{1},orderId:{2}", payment, state, orderId));
            if (orderId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数不能为空" };
            }
            if (!OrderSV.LockOrder(orderId))
            {
                return new ResultDTO { ResultCode = 110, Message = "操作失败" };
            }
            try
            {
                CommodityOrder commodityOrder = CommodityOrder.ObjectSet().FirstOrDefault(n => n.Id == orderId);
                if (commodityOrder == null)
                {
                    return new ResultDTO { ResultCode = 1, Message = "订单不存在" };
                }
                if (commodityOrder.State == 1 && state == 1)
                {
                    return new ResultDTO { ResultCode = 1, Message = "订单状态无法支付" };
                }

                if (userId == Guid.Empty)
                {
                    userId = commodityOrder.UserId;
                }
                appId = commodityOrder.AppId;

                var orderitemlist = OrderItem.ObjectSet().Where(n => n.CommodityOrderId == commodityOrder.Id).ToList();


                UpdateCommodityOrderParamDTO ucopDto = new UpdateCommodityOrderParamDTO();
                ucopDto.appId = appId;
                ucopDto.goldpwd = goldpwd;
                ucopDto.orderId = orderId;
                ucopDto.payment = payment;
                ucopDto.remessage = remessage;
                ucopDto.targetState = state;
                ucopDto.userId = userId;
                //未发货
                if (state == 1) //调用此接口付状态1统一改为付状态11（付款中）
                {
                    result = UpdateOrderStateTo1(ucopDto, commodityOrder, orderitemlist);
                }
                //确认收货
                else if (state == 3)
                {
                    result = UpdateOrderStateTo3(ucopDto, commodityOrder);
                }
                //取消订单
                else if (state == 4)
                {
                    result = UpdateOrderStateTo4(ucopDto, commodityOrder, orderitemlist);
                }
                else if (state == 5)
                {
                    result = UpdateOrderStateTo5(ucopDto, commodityOrder, orderitemlist);
                }
                else
                {
                    return new ResultDTO { ResultCode = 1, Message = "订单状态不存在" };
                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(
                    string.Format(
                        "订单状态修改服务异常。state{0}，orderId:{1}，userId:{2}，appId:{3}，payment:{4}，goldpwd:{5}，remessage:{6}",
                        state, orderId, userId, appId, payment, goldpwd, remessage), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(orderId);
            }
        }





        //job更新京东订单
        public ResultDTO UpdateJobCommodityOrderExt(DateTime time, System.Guid orderId, System.Guid userId, System.Guid appId, int payment, string goldpwd, string remessage)
        {
            ResultDTO result = new ResultDTO();
            result.ResultCode = 0;
            result.Message = "Success";
            LogHelper.Info(string.Format("CommodityOrderSV:支付方式：{0},orderId:{1}", payment, orderId));
            if (orderId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数不能为空" };
            }
            try
            {
                CommodityOrder commodityOrder = CommodityOrder.ObjectSet().FirstOrDefault(n => n.Id == orderId);
                if (commodityOrder == null)
                {
                    return new ResultDTO { ResultCode = 1, Message = "订单不存在" };
                }
                if (userId == Guid.Empty)
                {
                    userId = commodityOrder.UserId;
                }
                appId = commodityOrder.AppId;
                UpdateCommodityOrderParamDTO ucopDto = new UpdateCommodityOrderParamDTO();
                ucopDto.appId = appId;
                ucopDto.goldpwd = goldpwd;
                ucopDto.orderId = orderId;
                ucopDto.payment = payment;
                ucopDto.remessage = remessage;
                ucopDto.targetState = 3;
                ucopDto.userId = userId;
                result = UpdateJobOrderStateTo(ucopDto, commodityOrder, time);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("job更新京东订单异常信息:{0}", ex.Message), ex);
                return new ResultDTO { ResultCode = 1, Message = ex.Message };
            }

        }


        /// <summary>
        ///  job更新京东订单确认收货
        /// </summary>
        /// <param name="ucopDto"></param>
        /// <param name="commodityOrder">订单信息</param>
        /// <returns></returns>
        private ResultDTO UpdateJobOrderStateTo(UpdateCommodityOrderParamDTO ucopDto, CommodityOrder commodityOrder, DateTime time)
        {
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            ResultDTO result = new ResultDTO();
            var contextDTO = Jinher.AMP.BTP.Common.AuthorizeHelper.InitAuthorizeInfo();
            int newState = ucopDto.targetState;
            int targetState = 3;
            DateTime now = DateTime.Now;
            //订单日志
            Journal journal = CreateJournal(ucopDto, commodityOrder);
            journal.Name = "用户确认收货";
            contextSession.SaveObject(journal);
            if (commodityOrder.Payment != 1 && commodityOrder.OrderType != 1)
            {
                if (!CommodityOrderService.ObjectSet().Any(p => p.Id == commodityOrder.Id))
                {
                    CommodityOrderService commodityOrderService = new CommodityOrderService();
                    commodityOrderService.Id = commodityOrder.Id;
                    commodityOrderService.Name = commodityOrder.Name;
                    commodityOrderService.Code = commodityOrder.Code;
                    commodityOrderService.State = 3;
                    commodityOrderService.SubId = commodityOrder.SubId;
                    commodityOrderService.SelfTakeFlag = commodityOrder.SelfTakeFlag;
                    commodityOrderService.EntityState = System.Data.EntityState.Added;
                    contextSession.SaveObject(commodityOrderService);
                }
            }
            commodityOrder.State = targetState;
            commodityOrder.ConfirmTime = time;
            commodityOrder.ModifiedOn = now;

            // 更新结算单确认收货时间
            SettleAccountHelper.ConfirmOrder(contextSession, commodityOrder);

            //易捷北京自营商品申请开电子发票
            Guid eesAppId = new Guid(CustomConfig.InvoiceAppId);
            //易捷北京的自营或者门店自营
            MallApply mallApply = MallApply.ObjectSet().FirstOrDefault(t => t.EsAppId == eesAppId && t.AppId == commodityOrder.AppId && (t.Type == 0 || t.Type == 2 || t.Type == 3));
            if (mallApply != null)
            {
                var invoice = Invoice.ObjectSet().FirstOrDefault(t => t.CommodityOrderId == commodityOrder.Id);
                if (invoice != null && invoice.Category == 2)
                {
                    TPS.Invoic.InvoicManage invoicManage = new TPS.Invoic.InvoicManage();
                    invoicManage.CreateInvoic(contextSession, commodityOrder, 0);
                }
            }
            contextSession.SaveChanges();

            #region CPS通知
            //CPS通知
            if ((commodityOrder.SrcType == 33 || commodityOrder.SrcType == 34 || commodityOrder.SrcType == 36 || commodityOrder.SrcType == 39 || commodityOrder.SrcType == 40) && !string.IsNullOrEmpty(commodityOrder.CPSId) && commodityOrder.CPSId != "null")
            {
                LogHelper.Info("CPS通知");
                if (contextDTO == null)
                {
                    contextDTO = Jinher.AMP.BTP.Common.AuthorizeHelper.CoinInitAuthorizeInfo();
                }
                CPSCallBack(commodityOrder, contextDTO);
            }
            #endregion

            #region 发送消息
            System.Threading.ThreadPool.QueueUserWorkItem(
                a =>
                {
                    #region 确认收货消息
                    Guid esAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
                    AddMessage addmassage = new AddMessage();
                    addmassage.AddMessages(commodityOrder.Id.ToString(), commodityOrder.UserId.ToString(), esAppId, commodityOrder.Code, targetState, "", "order");
                    #endregion

                    #region 众销红包消息
                    if (commodityOrder.Commission > 0)
                    {
                        decimal commission = commodityOrder.Commission.ToMoney();
                        decimal tradeMoney = (commission * CustomConfig.SaleShare.BuyerPercent).ToMoney();  //商贸众销
                        if (tradeMoney > 0)
                        {
                            OrderShareMess tempShare = OrderShareMess.ObjectSet().FirstOrDefault(c => c.OrderId == commodityOrder.Id);
                            if (tempShare != null)
                            {
                                List<Guid> userIds = new List<Guid>();
                                SNS.Deploy.CustomDTO.ReturnInfoDTO<Guid> shareServiceResult = SNSSV.Instance.GetShareUserId(tempShare.ShareId);
                                if (shareServiceResult != null)
                                {
                                    if (shareServiceResult.Code == "0")
                                    {
                                        userIds.Add(shareServiceResult.Content);
                                        SendMessageToPayment(userIds, "affirm", tradeMoney.ToGold().ToString(), null, 0);
                                    }
                                    else
                                    {
                                        LogHelper.Error(string.Format("确认收货后发送消息失败 返回code为 1：\"根据分享Id获取分享人Id\" 不成功,分享Id={0}，返回结果={1}", tempShare.ShareId, JsonHelper.JsonSerializer(shareServiceResult)));
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                });
            #endregion

            YXOrderHelper.ConfirmReceivedOrder(commodityOrder.Id);

            return result;
        }




        /// <summary>
        /// 订单状态变为1（已支付，未发货）  |只有货到付款使用
        /// </summary>
        /// <param name="ucopDto"></param>
        /// <param name="commodityOrder">订单信息</param>
        /// <param name="orderitemlist"></param>
        /// <returns></returns>
        private ResultDTO UpdateOrderStateTo1(UpdateCommodityOrderParamDTO ucopDto, CommodityOrder commodityOrder, List<OrderItem> orderitemlist)
        {
            int newState = ucopDto.targetState;
            if (!OrderSV.CanChangeState(newState, commodityOrder, null, null, null))
            {
                return new ResultDTO() { ResultCode = 1, Message = "订单状态修改错误" };
            }

            var contextDTO = Jinher.AMP.BTP.Common.AuthorizeHelper.InitAuthorizeInfo();

            int targetState = 1;
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            ResultDTO result = new ResultDTO();
            DateTime now = DateTime.Now;
            List<Commodity> needRefreshCacheCommoditys = new List<Commodity>();

            Guid userId = ucopDto.userId;
            int payment = ucopDto.payment;
            Guid appId = ucopDto.appId;
            //订单日志
            Journal journal = CreateJournal(ucopDto, commodityOrder);

            //处理订单商品
            if (payment != 1)
            {
                return result;
            }

            #region 好运来订单支付后调用好运来接口
            if (payment == 1)
            {
                if (commodityOrder.State == 1
                    && commodityOrder.SrcType == 1 && commodityOrder.SrcTagId != null && commodityOrder.SrcTagId != Guid.Empty)//好运来
                {
                    GameSV.Instance.UpdateWinPlayerBuyed(commodityOrder.SrcTagId.Value, commodityOrder.UserId);
                }
            }
            #endregion

            List<int> allPayments = new PaySourceSV().GetAllPayments();
            if (allPayments.Contains(payment))
            {
                commodityOrder.Payment = payment;
            }
            else
            {
                return new ResultDTO { ResultCode = 1, Message = "支付方式不存在" };
            }

            #region

            var commodityIdList = orderitemlist.Select(a => a.CommodityId).ToList();
            var commodityList = Commodity.ObjectSet().Where(a => commodityIdList.Contains(a.Id) && a.IsDel == false && a.State == 0).ToList();

            #region 好运来权限校验
            if (commodityOrder.SrcType == 1 && commodityOrder.SrcTagId != null && commodityOrder.SrcTagId != Guid.Empty && commodityList.Count > 0)//好运来
            {
                Guid firstComId = commodityList[0].Id;
                GenUserPrizeRecord prizeResultRecord = (from e in GenUserPrizeRecord.ObjectSet()
                                                        where e.CommodityId == firstComId && e.UserId == userId && e.PromotionId == commodityOrder.SrcTagId
                                                        select e).FirstOrDefault();

                if (prizeResultRecord == null)
                {
                    LogHelper.Error("您没有权限购买此商品");
                    return new ResultDTO { ResultCode = 1, Message = "您没有权限购买此商品" };
                }
                if (prizeResultRecord.IsBuyed)
                {
                    LogHelper.Error("您已经购买过此商品，现在没有权限购买此商品");
                    return new ResultDTO { ResultCode = 1, Message = "您已经购买过此商品，现在没有权限购买此商品" };
                }
                if (prizeResultRecord.Price != commodityOrder.Price)
                {
                    LogHelper.Error("您不能以此价格购买此商品");
                    return new ResultDTO { ResultCode = 1, Message = "您不能以此价格购买此商品" };
                }
                prizeResultRecord.OrderId = commodityOrder.Id;
                prizeResultRecord.EntityState = System.Data.EntityState.Modified;
                prizeResultRecord.IsBuyed = true;
                contextSession.SaveObject(prizeResultRecord);
            }
            #endregion

            Jinher.AMP.App.Deploy.ApplicationDTO applicationDTO = GetAppIdInfo(commodityOrder.Id, commodityOrder.AppId);

            var commodityStockIds = orderitemlist.Where(c => c.CommodityStockId.HasValue).Select(a => a.CommodityStockId).Distinct().ToList();
            List<CommodityStock> commodityStockList = new List<CommodityStock>();
            if (commodityStockIds.Any())
                commodityStockList = CommodityStock.ObjectSet().Where(c => commodityStockIds.Contains(c.Id)).ToList();
            Dictionary<Guid, Commodity> littleStockComDict = new Dictionary<Guid, Commodity>();

            #region 减库存
            List<OrderItemPresent> presents = new List<OrderItemPresent>();
            foreach (var orderItem in orderitemlist)
            {
                //减库存
                Commodity com = commodityList.First(c => c.Id == orderItem.CommodityId);
                CommodityStock cStock = commodityStockList.FirstOrDefault(c => orderItem.CommodityStockId == c.Id);
                if (cStock != null)
                {
                    if (com.Stock <= 0 || cStock.Stock <= 0)
                    {
                        return new ResultDTO { ResultCode = 1, Message = "商品库存不足" };
                    }
                    com.Stock -= orderItem.Number;
                    com.Salesvolume += orderItem.Number;
                    com.ModifiedOn = now;
                    com.EntityState = System.Data.EntityState.Modified;

                    cStock.Stock -= orderItem.Number;
                    cStock.ModifiedOn = now;
                    cStock.EntityState = System.Data.EntityState.Modified;

                }
                else
                {
                    if (com.Stock <= 0)
                    {
                        return new ResultDTO { ResultCode = 1, Message = "商品库存不足" };
                    }
                    com.Stock -= orderItem.Number;
                    com.Salesvolume += orderItem.Number;
                    com.ModifiedOn = now;
                    com.EntityState = System.Data.EntityState.Modified;
                }
                if (com.Stock <= 1)
                {
                    if (!littleStockComDict.ContainsKey(com.Id))
                        littleStockComDict.Add(com.Id, com);
                }
                needRefreshCacheCommoditys.RemoveAll(c => c.Id == com.Id);
                needRefreshCacheCommoditys.Add(com);

                // 赠品库存
                if (orderItem.HasPresent.HasValue && orderItem.HasPresent.Value)
                {
                    presents.AddRange(OrderItemPresent.ObjectSet().Where(_ => _.OrderItemId == orderItem.Id).ToList());
                }
            }

            var presentCommodityIds = presents.Select(a => a.CommodityId).ToList();
            var presentCommodityList = Commodity.ObjectSet().Where(a => presentCommodityIds.Contains(a.Id) && !a.IsDel).ToList();
            var presentCommodityStockIds = presents.Where(c => c.CommodityStockId.HasValue).Select(a => a.CommodityStockId).Distinct().ToList();
            List<CommodityStock> presentCommodityStockList = new List<CommodityStock>();
            if (presentCommodityStockIds.Count > 0)
            {
                presentCommodityStockList = CommodityStock.ObjectSet().Where(c => commodityStockIds.Contains(c.Id)).ToList();
            }
            // 减去赠品库存
            OrderEventHelper.SubStock();
            foreach (var orderItem in presents)
            {
                //减库存
                Commodity com = presentCommodityList.First(c => c.Id == orderItem.CommodityId);
                if (orderItem.CommodityStockId.HasValue && orderItem.CommodityStockId != Guid.Empty)
                {
                    CommodityStock cStock = presentCommodityStockList.FirstOrDefault(c => orderItem.CommodityStockId == c.Id);
                    if (cStock != null)
                    {
                        if (com.Stock <= 0 || cStock.Stock <= 0)
                        {
                            return new ResultDTO { ResultCode = 1, Message = "商品库存不足" };
                        }
                        com.Stock -= orderItem.Number;
                        com.Salesvolume += orderItem.Number;
                        com.ModifiedOn = now;
                        com.EntityState = System.Data.EntityState.Modified;

                        cStock.Stock -= orderItem.Number;
                        cStock.ModifiedOn = now;
                        cStock.EntityState = System.Data.EntityState.Modified;
                        contextSession.SaveObject(cStock);
                    }
                }
                else
                {
                    if (com.Stock <= 0)
                    {
                        return new ResultDTO { ResultCode = 1, Message = "商品库存不足" };
                    }
                    com.Stock -= orderItem.Number;
                    com.Salesvolume += orderItem.Number;
                    com.ModifiedOn = now;
                    com.EntityState = System.Data.EntityState.Modified;
                    contextSession.SaveObject(com);
                }
                if (com.Stock <= 1)
                {
                    if (!littleStockComDict.ContainsKey(com.Id))
                        littleStockComDict.Add(com.Id, com);
                }
                needRefreshCacheCommoditys.RemoveAll(c => c.Id == com.Id);
                needRefreshCacheCommoditys.Add(com);
            }

            #endregion

            #region 库存紧张提醒
            if (littleStockComDict.Any())
            {
                if (applicationDTO != null && applicationDTO.OwnerId.HasValue && applicationDTO.OwnerId != Guid.Empty && applicationDTO.OwnerType == Jinher.AMP.App.Deploy.Enum.AppOwnerTypeEnum.Personal)
                {
                    Jinher.AMP.Info.Deploy.CustomDTO.MessageForAddDTO messageAdd =
                         new Info.Deploy.CustomDTO.MessageForAddDTO();

                    string _appName = APPSV.GetAppName(commodityOrder.AppId);
                    foreach (var com in littleStockComDict.Values.ToList())
                    {
                        //根据Appid判断是否为个人商家 如果为个人商家 发送 库存提醒消息
                        try
                        {

                            //推送消息
                            List<Guid> lGuid = new List<Guid>();
                            lGuid.Add(applicationDTO.OwnerId.Value);
                            messageAdd.PublishTime = DateTime.Now;
                            messageAdd.ReceiverUserId = lGuid;

                            messageAdd.SenderType = Info.Deploy.Enum.SenderType.System;
                            if (!string.IsNullOrWhiteSpace(com.No_Code))
                            {
                                messageAdd.Title = "【" + _appName + "】" + "【" + com.No_Code + "】【" + com.Name + "】商品数量紧张，请关注！";
                                messageAdd.Content = "【" + _appName + "】" + "【" + com.No_Code + "】【" + com.Name + "】商品数量紧张，请关注！";
                            }
                            else
                            {
                                messageAdd.Title = "【" + _appName + "】" + "【" + com.Name + "】商品数量紧张，请关注！";
                                messageAdd.Content = "【" + _appName + "】" + "【" + com.Name + "】商品数量紧张，请关注！";
                            }

                            messageAdd.ReceiverRange = Info.Deploy.Enum.ReceiverRange.SpecifiedUser;
                            Jinher.AMP.BTP.TPS.InfoSV.Instance.AddSystemMessage(messageAdd);
                        }
                        catch (Exception ex)
                        {

                            LogHelper.Error(string.Format("InfoManageSV服务异常:获取应用信息异常。orderId：{0}", commodityOrder.Id), ex);
                        }
                    }
                }
            }
            #endregion

            SendMessageToAppOwner(targetState, appId, commodityOrder, applicationDTO, contextDTO);

            if (commodityOrder.SelfTakeFlag == 1)
            {
                SendMessageToSelfTakeManagers(targetState, commodityOrder);
            }

            #region  发票

            var invOrder = (from inv in Invoice.ObjectSet()
                            where inv.CommodityOrderId == commodityOrder.Id
                            select inv).FirstOrDefault();
            if (invOrder != null && invOrder.State == 0)
            {
                invOrder.State = 1;
                invOrder.EntityState = EntityState.Modified;
                invOrder.ModifiedOn = now;
                GetInvoiceJounal(invOrder.Id, Guid.Empty, invOrder.State, 1);
            }

            #endregion

            #endregion

            commodityOrder.PaymentTime = now;

            journal.Name = "用户支付订单";
            contextSession.SaveObject(journal);

            commodityOrder.State = 1;
            List<int> secTranPayments = new PaySourceSV().GetSecuriedTransactionPaymentExt();
            if (secTranPayments.Contains(payment))
            {
                commodityOrder.State = 11;
                journal.StateTo = 11;
            }
            commodityOrder.ModifiedOn = now;
            commodityOrder.EntityState = System.Data.EntityState.Modified;


            contextSession.SaveChanges();

            if (needRefreshCacheCommoditys.Any())
            {
                needRefreshCacheCommoditys.ForEach(c => c.RefreshCache(EntityState.Modified));
            }

            //订单id
            string odid = commodityOrder.Id.ToString();
            string type = "order";
            //用户id
            string usid = commodityOrder.UserId.ToString();
            Guid esAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
            AddMessage addmassage = new AddMessage();
            addmassage.AddMessages(odid, usid, esAppId, commodityOrder.Code, targetState, "", type);

            return result;
        }


        /// <summary>
        ///  订单状态变为3（确认收货）
        /// </summary>
        /// <param name="ucopDto"></param>
        /// <param name="commodityOrder">订单信息</param>
        /// <returns></returns>
        private ResultDTO UpdateOrderStateTo3(UpdateCommodityOrderParamDTO ucopDto, CommodityOrder commodityOrder)
        {
            int newState = ucopDto.targetState;
            if (!OrderSV.CanChangeState(newState, commodityOrder, null, null, null))
            {
                return new ResultDTO() { ResultCode = 1, Message = "订单状态修改错误" };
            }

            var contextDTO = Jinher.AMP.BTP.Common.AuthorizeHelper.InitAuthorizeInfo();
            int targetState = 3;
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            ResultDTO result = new ResultDTO();
            DateTime now = DateTime.Now;

            //订单日志
            Journal journal = CreateJournal(ucopDto, commodityOrder);
            journal.Name = "用户确认收货";
            contextSession.SaveObject(journal);

            #region 金币担保确认收货
            List<int> secTranPayments = new PaySourceSV().GetSecuriedTransactionPaymentExt();
            if (secTranPayments.Contains(ucopDto.payment))
            {
                LogHelper.Info(string.Format("out:{0};sign:{1};userid:{2};password:{3};", commodityOrder.Id, CustomConfig.PaySing, commodityOrder.UserId, ucopDto.goldpwd));
                List<object> savelistUnused;

                Jinher.AMP.App.Deploy.CustomDTO.AppIdOwnerIdTypeDTO applicationDTO = APPSV.Instance.GetAppOwnerInfo(commodityOrder.AppId, contextDTO);
                if (applicationDTO == null || applicationDTO.OwnerId == Guid.Empty)
                {
                    return new ResultDTO { ResultCode = 1, Message = "确认收货失败" };
                }

                Jinher.AMP.FSP.Deploy.CustomDTO.ReturnInfoDTO goldPayresult = new FSP.Deploy.CustomDTO.ReturnInfoDTO();
                if (commodityOrder.RealPrice > 0)
                {
                    if (commodityOrder.OrderType != 1)
                    {
                        var confirmPayDTO = OrderSV.BuildConfirmPayDTO(contextSession, commodityOrder, out savelistUnused, applicationDTO, ucopDto.goldpwd);
                        LogHelper.Info(string.Format("订单确认收货UpdateCommodityOrderExt：订单id={0} , 支付DTO ={1}", commodityOrder.Id, JsonHelper.JsonSerializer(confirmPayDTO)), "BTP_Order");
                        //冻结
                        goldPayresult = Jinher.AMP.BTP.TPS.FSPSV.Instance.ConfirmPayFreeze(confirmPayDTO);
                    }
                    else
                    {
                        var confirmPayDTO = OrderSV.BuildConfirmPayDTO(contextSession, commodityOrder, out savelistUnused, applicationDTO, ucopDto.goldpwd);
                        LogHelper.Info(string.Format("订单确认收货UpdateCommodityOrderExt：订单id={0} , 支付DTO ={1}", commodityOrder.Id, JsonHelper.JsonSerializer(confirmPayDTO)), "BTP_Order");

                        goldPayresult = Jinher.AMP.BTP.TPS.FSPSV.Instance.ConfirmPay(confirmPayDTO);
                    }
                }
                //担保支付宝支付不成功
                if (goldPayresult.Code != 0 && commodityOrder.RealPrice > 0)
                {
                    if (goldPayresult.Code == -9)
                    {
                        return new ResultDTO { ResultCode = 1, Message = "金币支付密码错误" };
                    }
                    else
                    {
                        LogHelper.Error(string.Format("担保支付宝确认支付金币接口支付失败。code：{0}，Message：{1}", goldPayresult.Code, goldPayresult.Message));
                        return new ResultDTO { ResultCode = 1, Message = "担保支付宝支付失败" };
                    }
                }

            }
            #endregion

            if (commodityOrder.Payment != 1 && commodityOrder.OrderType != 1)
            {
                //排除后台有自动完成售后 还要插入数据 引起的主键唯一性错误的问题
                var comService = CommodityOrderService.FindByID(commodityOrder.Id);
                if (comService == null)
                {
                    CommodityOrderService commodityOrderService = new CommodityOrderService();
                    commodityOrderService.Id = commodityOrder.Id;
                    commodityOrderService.Name = commodityOrder.Name;
                    commodityOrderService.Code = commodityOrder.Code;
                    commodityOrderService.State = targetState;
                    commodityOrderService.SubId = commodityOrder.SubId;
                    commodityOrderService.SelfTakeFlag = commodityOrder.SelfTakeFlag;
                    commodityOrderService.EntityState = System.Data.EntityState.Added;
                    contextSession.SaveObject(commodityOrderService);
                }
            }
            commodityOrder.State = targetState;
            commodityOrder.ConfirmTime = now;
            commodityOrder.ModifiedOn = now;
            commodityOrder.EntityState = System.Data.EntityState.Modified;

            // 更新结算单确认收货时间
            SettleAccountHelper.ConfirmOrder(contextSession, commodityOrder);

            //易捷北京自营商品申请开电子发票
            Guid eesAppId = new Guid(CustomConfig.InvoiceAppId);
            //易捷北京的自营或者门店自营
            MallApply mallApply = MallApply.ObjectSet().FirstOrDefault(t => t.EsAppId == eesAppId && t.AppId == commodityOrder.AppId && (t.Type == 0 || t.Type == 2 || t.Type == 3));
            if (mallApply != null)
            {
                var invoice = Invoice.ObjectSet().FirstOrDefault(t => t.CommodityOrderId == commodityOrder.Id);
                if (invoice != null && invoice.Category == 2)
                {
                    TPS.Invoic.InvoicManage invoicManage = new TPS.Invoic.InvoicManage();
                    invoicManage.CreateInvoic(contextSession, commodityOrder, 0);
                }
            }

            contextSession.SaveChanges();

            #region CPS通知
            //CPS通知
            if ((commodityOrder.SrcType == 33 || commodityOrder.SrcType == 34 || commodityOrder.SrcType == 36 || commodityOrder.SrcType == 39 || commodityOrder.SrcType == 40) && !string.IsNullOrEmpty(commodityOrder.CPSId) && commodityOrder.CPSId != "null")
            {
                LogHelper.Info("CPS通知");
                if (contextDTO == null)
                {
                    contextDTO = Jinher.AMP.BTP.Common.AuthorizeHelper.CoinInitAuthorizeInfo();
                }
                CPSCallBack(commodityOrder, contextDTO);
            }
            #endregion

            #region 发送消息
            System.Threading.ThreadPool.QueueUserWorkItem(
                a =>
                {
                    #region 确认收货消息
                    Guid esAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
                    AddMessage addmassage = new AddMessage();
                    addmassage.AddMessages(commodityOrder.Id.ToString(), commodityOrder.UserId.ToString(), esAppId, commodityOrder.Code, targetState, "", "order");
                    #endregion

                    #region 众销红包消息
                    if (commodityOrder.Commission > 0)
                    {
                        decimal commission = commodityOrder.Commission.ToMoney();
                        decimal tradeMoney = (commission * CustomConfig.SaleShare.BuyerPercent).ToMoney();  //商贸众销
                        if (tradeMoney > 0)
                        {
                            OrderShareMess tempShare = OrderShareMess.ObjectSet().FirstOrDefault(c => c.OrderId == commodityOrder.Id);
                            if (tempShare != null)
                            {
                                List<Guid> userIds = new List<Guid>();
                                SNS.Deploy.CustomDTO.ReturnInfoDTO<Guid> shareServiceResult = SNSSV.Instance.GetShareUserId(tempShare.ShareId);
                                if (shareServiceResult != null)
                                {
                                    if (shareServiceResult.Code == "0")
                                    {
                                        userIds.Add(shareServiceResult.Content);
                                        SendMessageToPayment(userIds, "affirm", tradeMoney.ToGold().ToString(), null, 0);
                                    }
                                    else
                                    {
                                        LogHelper.Error(string.Format("确认收货后发送消息失败 返回code为 1：\"根据分享Id获取分享人Id\" 不成功,分享Id={0}，返回结果={1}", tempShare.ShareId, JsonHelper.JsonSerializer(shareServiceResult)));
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                });
            #endregion

            YXOrderHelper.ConfirmReceivedOrder(commodityOrder.Id);

            return result;
        }

        /// <summary>
        ///  订单状态变为4或5（商家取消订单=4，客户取消订单=5）
        /// </summary>
        /// <param name="targetState">目标状态</param>
        /// <param name="commodityOrder">订单信息</param>
        /// <param name="orderitemlist">订单项列表</param>
        /// <param name="needRefreshCacheCommoditys">需要刷新缓存的商品列表</param>
        /// <param name="needRefreshCacheTodayPromotions">需要刷新缓存的活动列表</param>
        /// <param name="journal">订单日志</param>
        /// <param name="contextDTO">用户信息上下文</param>
        /// <param name="now">当前时间</param>
        /// <param name="remessage"></param>
        /// <param name="appId">应用id</param>
        /// <returns></returns>
        private ResultDTO UpdateOrderStateTo4(UpdateCommodityOrderParamDTO ucopDto, CommodityOrder commodityOrder, List<OrderItem> orderitemlist)
        {
            int newState = ucopDto.targetState;
            if (!OrderSV.CanChangeState(newState, commodityOrder, null, null, null))
            {
                return new ResultDTO() { ResultCode = 1, Message = "订单状态修改错误" };
            }

            ContextDTO contextDTO = this.ContextDTO;
            if (contextDTO == null)
            {
                contextDTO = Jinher.AMP.BTP.Common.AuthorizeHelper.CoinInitAuthorizeInfo();
            }

            Guid userId = ucopDto.userId;
            int payment = ucopDto.payment;
            Guid appId = ucopDto.appId;
            int targetState = ucopDto.targetState;
            string remessage = ucopDto.remessage;

            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            ResultDTO result = new ResultDTO();
            List<Commodity> needRefreshCacheCommoditys = new List<Commodity>();
            List<TodayPromotion> needRefreshCacheTodayPromotions = new List<TodayPromotion>();
            DateTime now = DateTime.Now;

            //订单状态不是 不是待付款 支付方式不是
            if ((commodityOrder.State != 0 && commodityOrder.Payment != 1)
                || (commodityOrder.Payment == 1 && commodityOrder.State != 1))
            {
                return new ResultDTO { ResultCode = 1, Message = "订单状态无法取消" };
            }
            if (TPS.Finance.Instance.GetIsPay(commodityOrder.Id))
            {
                return new ResultDTO { ResultCode = 1, Message = "订单状态无法取消" };
            }
            //解冻金币
            if (commodityOrder.Payment != 1 && commodityOrder.RealPrice > 0)
            {
                UnFreezeGoldDTO unFreezeGoldDTO = new UnFreezeGoldDTO()
                {
                    BizId = commodityOrder.Id,
                    Sign = CustomConfig.PaySing
                };
                Jinher.AMP.FSP.Deploy.CustomDTO.ReturnInfoDTO fspResult = FSPSV.Instance.UnFreezeGold(unFreezeGoldDTO, "取消订单");
                if (fspResult.Code != 0)
                {
                    result.ResultCode = 1;
                    result.Message = fspResult.Message;
                    return result;
                }
            }
            //回退积分
            SignSV.CommodityOrderCancelSrore(ContextFactory.CurrentThreadContext, commodityOrder);

            // 回退易捷币
            Jinher.AMP.BTP.TPS.Helper.YJBHelper.CancelOrder(ContextFactory.CurrentThreadContext, commodityOrder);

            //加库存
            if (commodityOrder.State > 0 && commodityOrder.Payment == 1)
            {
                foreach (OrderItem items in orderitemlist)
                {
                    Guid comId = items.CommodityId;
                    Commodity com = Commodity.ObjectSet().Where(n => n.Id == comId).First();

                    // zgx-modify 回滚库存
                    if (items.CommodityStockId.HasValue && items.CommodityStockId.Value != Guid.Empty)
                    {
                        CommodityStock cStock = CommodityStock.ObjectSet().Where(n => n.Id == items.CommodityStockId.Value && n.CommodityId == comId).First();
                        cStock.EntityState = System.Data.EntityState.Modified;
                        cStock.Stock += items.Number;
                        contextSession.SaveObject(cStock);
                    }
                    com.EntityState = System.Data.EntityState.Modified;
                    com.Stock += items.Number;
                    contextSession.SaveObject(com);
                    needRefreshCacheCommoditys.Add(com);

                    // 赠品库存
                    OrderEventHelper.AddStock(items, needRefreshCacheCommoditys);
                }
            }
            //释放活动资源
            if (commodityOrder.State == 0 || commodityOrder.State == 1 && commodityOrder.Payment == 1)
            {
                List<Tuple<string, string, int>> proComTuples = new List<Tuple<string, string, int>>();
                List<Tuple<string, string, long, long>> proComBuyTuples = new List<Tuple<string, string, long, long>>();

                foreach (var orderItem in orderitemlist.Where(c => c.PromotionId.HasValue && c.PromotionId != Guid.Empty && (c.Intensity != 10 || c.DiscountPrice != -1)))
                {
                    proComTuples.Add(new Tuple<string, string, int>(orderItem.PromotionId.Value.ToString(), orderItem.CommodityId.ToString(), -orderItem.Number));
                }
                if (proComTuples.Any())
                {
                    proComBuyTuples = RedisHelper.ListHIncr(proComTuples, commodityOrder.UserId);
                    if (proComBuyTuples == null || !proComBuyTuples.Any())
                    {
                        return new ResultDTO { ResultCode = 1, Message = "操作失败" };
                    }
                }
                UserLimited.ObjectSet().Context.ExecuteStoreCommand("delete from UserLimited where CommodityOrderId='" + commodityOrder.Id + "'");
                foreach (OrderItem orderItem in orderitemlist)
                {
                    Guid comId = orderItem.CommodityId;
                    if (orderItem.Intensity != 10 || orderItem.DiscountPrice != -1)
                    {
                        var promotionId = orderItem.PromotionId.HasValue ? orderItem.PromotionId.Value : Guid.Empty;
                        if (RedisHelper.HashContainsEntry(RedisKeyConst.ProSaleCountPrefix + promotionId.ToString(), orderItem.CommodityId.ToString()))
                        {
                            int surplusLimitBuyTotal = RedisHelper.GetHashValue<int>(RedisKeyConst.ProSaleCountPrefix + orderItem.PromotionId.Value.ToString(), orderItem.CommodityId.ToString());
                            surplusLimitBuyTotal = surplusLimitBuyTotal < 0 ? 0 : surplusLimitBuyTotal;
                            TodayPromotion to = TodayPromotion.ObjectSet().FirstOrDefault(n => n.CommodityId == comId && n.PromotionId == orderItem.PromotionId && n.EndTime > now && n.StartTime < now);
                            if (to != null)
                            {
                                to.SurplusLimitBuyTotal = surplusLimitBuyTotal;
                                to.EntityState = System.Data.EntityState.Modified;
                                needRefreshCacheTodayPromotions.Add(to);
                            }
                            PromotionItems pti = PromotionItems.ObjectSet().FirstOrDefault(n => n.PromotionId == orderItem.PromotionId && n.CommodityId == comId);
                            if (pti != null)
                            {
                                pti.SurplusLimitBuyTotal = surplusLimitBuyTotal;
                                pti.EntityState = System.Data.EntityState.Modified;
                            }
                        }
                        else  //缓存中没有，直接改库
                        {
                            TodayPromotion to = TodayPromotion.ObjectSet().FirstOrDefault(n => n.CommodityId == comId && n.PromotionId == orderItem.PromotionId && n.EndTime > now && n.StartTime < now);
                            if (to != null)
                            {
                                to.SurplusLimitBuyTotal -= orderItem.Number;
                                to.SurplusLimitBuyTotal = to.SurplusLimitBuyTotal < 0 ? 0 : to.SurplusLimitBuyTotal;
                                to.EntityState = System.Data.EntityState.Modified;
                                needRefreshCacheTodayPromotions.Add(to);
                            }
                            PromotionItems pti = PromotionItems.ObjectSet().FirstOrDefault(n => n.PromotionId == orderItem.PromotionId && n.CommodityId == comId);
                            if (pti != null)
                            {
                                pti.SurplusLimitBuyTotal -= orderItem.Number;
                                pti.SurplusLimitBuyTotal = pti.SurplusLimitBuyTotal < 0 ? 0 : pti.SurplusLimitBuyTotal;
                                pti.EntityState = System.Data.EntityState.Modified;
                            }
                        }
                    }
                }
            }

            commodityOrder.ConfirmTime = now;
            LogHelper.Info("取消订单原因:" + remessage);
            //commodityOrder.CancelReason = remessage;
            commodityOrder.MessageToBuyer = remessage;
            //订单日志
            Journal journal = CreateJournal(ucopDto, commodityOrder);
            journal.Name = "用户取消订单";
            contextSession.SaveObject(journal);

            commodityOrder.State = targetState;
            commodityOrder.ModifiedOn = now;
            commodityOrder.EntityState = System.Data.EntityState.Modified;


            contextSession.SaveChanges();

            if (needRefreshCacheCommoditys.Any())
            {
                needRefreshCacheCommoditys.ForEach(c => c.RefreshCache(EntityState.Modified));
            }
            if (needRefreshCacheTodayPromotions.Any())
            {
                needRefreshCacheTodayPromotions.ForEach(c => c.RefreshCache(EntityState.Modified));
            }

            //订单id
            string odid = commodityOrder.Id.ToString();
            string type = "order";
            //用户id
            string usid = commodityOrder.UserId.ToString();
            Guid esAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
            AddMessage addmassage = new AddMessage();
            addmassage.AddMessages(odid, usid, esAppId, commodityOrder.Code, targetState, "", type);
            ////正品会发送消息
            //if (new ZPHSV().CheckIsAppInZPH(appId))
            //{
            //    addmassage.AddMessages(odid, usid, CustomConfig.ZPHAppId, commodityOrder.Code, state, "", type);
            //}


            return result;
        }

        /// <summary>
        ///  订单状态变为4或5（客户取消订单=5）
        /// </summary>
        /// <param name="targetState">目标状态</param>
        /// <param name="commodityOrder">订单信息</param>
        /// <param name="orderitemlist">订单项列表</param>
        /// <param name="needRefreshCacheCommoditys">需要刷新缓存的商品列表</param>
        /// <param name="needRefreshCacheTodayPromotions">需要刷新缓存的活动列表</param>
        /// <param name="journal">订单日志</param>
        /// <param name="contextDTO">用户信息上下文</param>
        /// <param name="now">当前时间</param>
        /// <param name="remessage"></param>
        /// <param name="appId">应用id</param>
        /// <returns></returns>
        private ResultDTO UpdateOrderStateTo5(UpdateCommodityOrderParamDTO ucopDto, CommodityOrder commodityOrder, List<OrderItem> orderitemlist)
        {
            int newState = ucopDto.targetState;
            if (!OrderSV.CanChangeState(newState, commodityOrder, null, null, null))
            {
                return new ResultDTO() { ResultCode = 1, Message = "订单状态修改错误" };
            }
            ContextDTO contextDTO = this.ContextDTO;
            if (contextDTO == null)
            {
                contextDTO = Jinher.AMP.BTP.Common.AuthorizeHelper.CoinInitAuthorizeInfo();
            }

            Guid userId = ucopDto.userId;
            int payment = ucopDto.payment;
            Guid appId = ucopDto.appId;
            int targetState = ucopDto.targetState;


            ResultDTO result = new ResultDTO();

            if (contextDTO == null)
            {
                contextDTO = Jinher.AMP.BTP.Common.AuthorizeHelper.CoinInitAuthorizeInfo();
            }
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            List<Commodity> needRefreshCacheCommoditys = new List<Commodity>();
            List<TodayPromotion> needRefreshCacheTodayPromotions = new List<TodayPromotion>();
            DateTime now = DateTime.Now;

            //订单状态不是 不是待付款 支付方式不是
            if ((commodityOrder.State != 0 && commodityOrder.Payment != 1)
                || (commodityOrder.Payment == 1 && commodityOrder.State != 1))
            {
                return new ResultDTO { ResultCode = 1, Message = "订单状态无法取消" };
            }
            if (TPS.Finance.Instance.GetIsPay(commodityOrder.Id))
            {
                return new ResultDTO { ResultCode = 1, Message = "订单状态无法取消" };
            }
            //解冻金币
            if (commodityOrder.Payment != 1 && commodityOrder.RealPrice > 0)
            {
                UnFreezeGoldDTO unFreezeGoldDTO = new UnFreezeGoldDTO()
                {
                    BizId = commodityOrder.Id,
                    Sign = CustomConfig.PaySing
                };
                Jinher.AMP.FSP.Deploy.CustomDTO.ReturnInfoDTO fspResult = FSPSV.Instance.UnFreezeGold(unFreezeGoldDTO, "取消订单");
                if (fspResult.Code != 0)
                {
                    result.ResultCode = 1;
                    result.Message = fspResult.Message;
                    return result;
                }
            }
            //回退积分
            SignSV.CommodityOrderCancelSrore(ContextFactory.CurrentThreadContext, commodityOrder);

            // 回退易捷币
            Jinher.AMP.BTP.TPS.Helper.YJBHelper.CancelOrder(ContextFactory.CurrentThreadContext, commodityOrder);

            //跨店优惠券要求使用同一券的所有订单都取消订单时将券退回给用户。
            //找出订单使用的优惠券id
            Guid couponId = (from opd in OrderPayDetail.ObjectSet()
                             where opd.OrderId == commodityOrder.Id && opd.ObjectType == 1
                             select opd.ObjectId).FirstOrDefault();
            //couponId为空表示没有使用优惠券
            if (couponId != Guid.Empty)
            {
                //使用同一跨店优惠券的所有订单
                var orderIds = (from opd in OrderPayDetail.ObjectSet()
                                where opd.ObjectId == couponId && opd.ObjectType == 1
                                select opd.OrderId).ToList();
                //已被用户取消的订单
                var couponOrderCount = (from co in CommodityOrder.ObjectSet()
                                        where orderIds.Contains(co.Id) && co.State == 5
                                        select co.Id).Count();
                //当前订单被取消，所有使用该跨店优惠券的订单都被取消了
                if (orderIds.Count() - 1 == couponOrderCount)
                {
                    //退回跨店优惠券
                    CouponSV.RetreatCoupon(couponId);
                    ////回退优惠券
                    //CouponSV.RefundCoupon(ContextFactory.CurrentThreadContext, commodityOrder);
                }
            }




            //加库存
            if (commodityOrder.State > 0 && commodityOrder.Payment == 1)
            {
                foreach (OrderItem items in orderitemlist)
                {
                    Guid comId = items.CommodityId;
                    Commodity com = Commodity.ObjectSet().Where(n => n.Id == comId).First();

                    // zgx-modify 回滚库存
                    if (items.CommodityStockId.HasValue && items.CommodityStockId.Value != Guid.Empty)
                    {
                        CommodityStock cStock = CommodityStock.ObjectSet().Where(n => n.Id == items.CommodityStockId.Value && n.CommodityId == comId).First();
                        cStock.EntityState = System.Data.EntityState.Modified;
                        cStock.Stock += items.Number;
                        contextSession.SaveObject(cStock);
                    }
                    com.EntityState = System.Data.EntityState.Modified;
                    com.Stock += items.Number;
                    contextSession.SaveObject(com);
                    needRefreshCacheCommoditys.Add(com);
                    // 赠品库存
                    OrderEventHelper.AddStock(items, needRefreshCacheCommoditys);
                }
            }
            if (commodityOrder.State == 0 || commodityOrder.State == 1 && commodityOrder.Payment == 1)  //释放活动资源
            {
                List<Tuple<string, string, int>> proComTuples = new List<Tuple<string, string, int>>();
                List<Tuple<string, string, long, long>> proComBuyTuples = new List<Tuple<string, string, long, long>>();

                foreach (var orderItem in orderitemlist.Where(c => c.PromotionId.HasValue && c.PromotionId != Guid.Empty && (c.Intensity != 10 || c.DiscountPrice != -1)))
                {
                    proComTuples.Add(new Tuple<string, string, int>(orderItem.PromotionId.Value.ToString(), orderItem.CommodityId.ToString(), -orderItem.Number));
                }
                if (proComTuples.Any())
                {
                    proComBuyTuples = RedisHelper.ListHIncr(proComTuples, commodityOrder.UserId);
                    if (proComBuyTuples == null || !proComBuyTuples.Any())
                    {
                        return new ResultDTO { ResultCode = 1, Message = "操作失败" };
                    }
                }
                UserLimited.ObjectSet().Context.ExecuteStoreCommand("delete from UserLimited where CommodityOrderId='" + commodityOrder.Id + "'");
                foreach (OrderItem orderItem in orderitemlist)
                {
                    Guid comId = orderItem.CommodityId;
                    if (orderItem.Intensity != 10 || orderItem.DiscountPrice != -1)
                    {
                        var promotionId = orderItem.PromotionId.HasValue ? orderItem.PromotionId.Value : Guid.Empty;
                        if (RedisHelper.HashContainsEntry(RedisKeyConst.ProSaleCountPrefix + promotionId.ToString(), orderItem.CommodityId.ToString()))
                        {
                            int surplusLimitBuyTotal = RedisHelper.GetHashValue<int>(RedisKeyConst.ProSaleCountPrefix + orderItem.PromotionId.Value.ToString(), orderItem.CommodityId.ToString());
                            surplusLimitBuyTotal = surplusLimitBuyTotal < 0 ? 0 : surplusLimitBuyTotal;
                            TodayPromotion to = TodayPromotion.ObjectSet().FirstOrDefault(n => n.CommodityId == comId && n.PromotionId == orderItem.PromotionId && n.EndTime > now && n.StartTime < now);
                            if (to != null)
                            {
                                to.SurplusLimitBuyTotal = surplusLimitBuyTotal;
                                to.EntityState = System.Data.EntityState.Modified;
                                needRefreshCacheTodayPromotions.Add(to);
                            }
                            PromotionItems pti = PromotionItems.ObjectSet().FirstOrDefault(n => n.PromotionId == orderItem.PromotionId && n.CommodityId == comId);
                            if (pti != null)
                            {
                                pti.SurplusLimitBuyTotal = surplusLimitBuyTotal;
                                pti.EntityState = System.Data.EntityState.Modified;
                            }
                        }
                        else  //缓存中没有，直接改库
                        {
                            TodayPromotion to = TodayPromotion.ObjectSet().FirstOrDefault(n => n.CommodityId == comId && n.PromotionId == orderItem.PromotionId && n.EndTime > now && n.StartTime < now);
                            if (to != null)
                            {
                                to.SurplusLimitBuyTotal -= orderItem.Number;
                                to.SurplusLimitBuyTotal = to.SurplusLimitBuyTotal < 0 ? 0 : to.SurplusLimitBuyTotal;
                                to.EntityState = System.Data.EntityState.Modified;
                                needRefreshCacheTodayPromotions.Add(to);
                            }
                            PromotionItems pti = PromotionItems.ObjectSet().FirstOrDefault(n => n.PromotionId == orderItem.PromotionId && n.CommodityId == comId);
                            if (pti != null)
                            {
                                pti.SurplusLimitBuyTotal -= orderItem.Number;
                                pti.SurplusLimitBuyTotal = pti.SurplusLimitBuyTotal < 0 ? 0 : pti.SurplusLimitBuyTotal;
                                pti.EntityState = System.Data.EntityState.Modified;
                            }
                        }
                    }
                }
            }

            commodityOrder.ConfirmTime = now;
            string remessage = ucopDto.remessage;
            short reasonCdoe;
            if (!short.TryParse(remessage, out reasonCdoe))
            {
                return new ResultDTO { ResultCode = 1, Message = "操作失败" };
            }
            remessage = TypeToStringHelper.CancleOrderReasonTypeToString(reasonCdoe);
            LogHelper.Info("取消订单原因:" + remessage);
            //commodityOrder.CancelReason = remessage;
            commodityOrder.MessageToBuyer = remessage;
            commodityOrder.CancelReasonCode = reasonCdoe;
            //订单日志
            Journal journal = CreateJournal(ucopDto, commodityOrder);
            journal.Name = "用户取消订单";
            contextSession.SaveObject(journal);

            commodityOrder.State = targetState;
            commodityOrder.ModifiedOn = now;
            commodityOrder.EntityState = System.Data.EntityState.Modified;


            contextSession.SaveChanges();

            if (needRefreshCacheCommoditys.Any())
            {
                needRefreshCacheCommoditys.ForEach(c => c.RefreshCache(EntityState.Modified));
            }
            if (needRefreshCacheTodayPromotions.Any())
            {
                needRefreshCacheTodayPromotions.ForEach(c => c.RefreshCache(EntityState.Modified));
            }

            //订单id
            string odid = commodityOrder.Id.ToString();
            string type = "order";
            //用户id
            string usid = commodityOrder.UserId.ToString();
            Guid esAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
            AddMessage addmassage = new AddMessage();
            addmassage.AddMessages(odid, usid, esAppId, commodityOrder.Code, targetState, "", type);
            ////正品会发送消息
            //if (new ZPHSV().CheckIsAppInZPH(appId))
            //{
            //    addmassage.AddMessages(odid, usid, CustomConfig.ZPHAppId, commodityOrder.Code, state, "", type);
            //}

            if (result.ResultCode != 0)
            {
                return result;
            }
            string _appName = APPSV.GetAppName(commodityOrder.AppId);
            string mobilemess = "【" + _appName + "】" + "编号为" + commodityOrder.Code + "的订单，由于" + remessage + "，客户取消了该订单，请知悉！";
            string tile = "【" + _appName + "】" + "编号为" + commodityOrder.Code + "的订单，由于" + remessage + "，客户取消了该订单，请知悉！";
            string content = "【" + _appName + "】" + "编号为" + commodityOrder.Code + "的订单，由于" + remessage + "，客户取消了该订单，请知悉！";
            SendMessageCommon(tile, content, mobilemess, 3, appId, commodityOrder, contextDTO);
            return result;
        }

        private Journal CreateJournal(UpdateCommodityOrderParamDTO ucopDto, CommodityOrder commodityOrder)
        {

            Guid userId = ucopDto.userId;
            int payment = ucopDto.payment;
            Guid appId = ucopDto.appId;
            int targetState = ucopDto.targetState;
            string remessage = ucopDto.remessage;
            int srcState = commodityOrder.State;

            //订单日志
            Journal journal = new Journal();
            journal.Id = Guid.NewGuid();
            journal.Code = commodityOrder.Code;
            journal.SubId = userId;
            journal.SubTime = DateTime.Now;
            journal.CommodityOrderId = commodityOrder.Id;

            journal.Details = "订单状态由" + srcState + "变为" + targetState;
            if (payment != null)
            {
                journal.Details += ",支付方式为:" + payment;
            }
            journal.StateFrom = srcState;
            journal.StateTo = targetState;
            journal.IsPush = false;
            journal.OrderType = commodityOrder.OrderType;

            journal.EntityState = System.Data.EntityState.Added;

            return journal;
        }


        private Jinher.AMP.App.Deploy.ApplicationDTO GetAppIdInfo(System.Guid orderId, System.Guid appId)
        {
            Jinher.AMP.App.Deploy.ApplicationDTO applicationDTO = null;
            ContextDTO contextDTO = AuthorizeHelper.InitAuthorizeInfo();
            try
            {
                Jinher.AMP.App.IBP.Facade.AppManagerFacade appManagerFacade = new App.IBP.Facade.AppManagerFacade();
                appManagerFacade.ContextDTO = contextDTO;
                applicationDTO = appManagerFacade.GetAppById(appId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("AppManagerSV服务异常:获取应用信息异常。orderId：{0}，appId：{1}", orderId, appId), ex);
            }
            return applicationDTO;
        }
        /// <summary>
        /// 金额转换成金币
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        private static long toGold(decimal price)
        {
            return (long)(100 * price) * 10;
        }

        /// <summary>
        /// 众筹订单支付
        /// </summary>
        /// <param name="contextSession"></param>
        /// <param name="now"></param>
        /// <param name="commodityOrder"></param>
        private void CrowdfundingPay(ContextSession contextSession, DateTime now, CommodityOrder commodityOrder)
        {
            if (commodityOrder == null)
            {
                return;
            }
            try
            {
                //众筹基本表
                var CrowdfundingQuery = Crowdfunding.ObjectSet().FirstOrDefault(q => q.AppId == commodityOrder.AppId && q.StartTime < now);

                if (CrowdfundingQuery == null)
                    return;

                //众筹股东表
                var UserCrowdfundingQuery = UserCrowdfunding.ObjectSet().FirstOrDefault(q => q.UserId == commodityOrder.UserId && q.AppId == commodityOrder.AppId);


                //  如果订单 改过价格，那么取改价后的价钱，如果没改过价格，就取不包含运费的价格
                decimal realPrice = commodityOrder.IsModifiedPrice
                                        ? commodityOrder.RealPrice.Value
                                        : commodityOrder.Price;

                //用户增加的股数
                long result = 0;
                //众筹中
                if (CrowdfundingQuery.State == 0)
                {
                    //众筹计数表
                    var CrowdfundingCountQuery =
                        CrowdfundingCount.ObjectSet().FirstOrDefault(q => q.CrowdfundingId == CrowdfundingQuery.Id);

                    if (UserCrowdfundingQuery == null)
                    {
                        //添加股东
                        UserCrowdfundingQuery = UserCrowdfunding.CreateUserCrowdfunding();
                        UserCrowdfundingQuery.UserId = commodityOrder.UserId;
                        UserCrowdfundingQuery.CrowdfundingId = CrowdfundingQuery.Id;
                        UserCrowdfundingQuery.AppId = commodityOrder.AppId;

                        try
                        {
                            List<UserNameAccountsDTO> userNamelist = CBCSV.Instance.GetUserNameAccountsByIds(new List<Guid>() { commodityOrder.UserId });

                            if (userNamelist.Any())
                            {
                                var user = userNamelist.First();
                                UserCrowdfundingQuery.UserName = user.userName;
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
                                        UserCrowdfundingQuery.UserCode = acc.Account;
                                    else
                                    {
                                        UserCrowdfundingQuery.UserCode = "--";
                                    }
                                }
                            }
                            else
                            {
                                UserCrowdfundingQuery.UserName = "--";
                                UserCrowdfundingQuery.UserCode = "--";
                            }
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error("CommodityOrderSV-CrowdfundingPay-CBC中的GetUserAccount异常", ex);
                            UserCrowdfundingQuery.UserName = "--";
                            UserCrowdfundingQuery.UserCode = "--";
                        }
                    }
                    else
                    {
                        //修改股东
                        UserCrowdfundingQuery.EntityState = System.Data.EntityState.Modified;
                    }
                    //购买活动总价格
                    decimal afterMoney = UserCrowdfundingQuery.Money + realPrice;
                    //用户购买价格所得的股数
                    long afterShareCnt = (long)(afterMoney / CrowdfundingQuery.PerShareMoney);
                    //购买活动总订单数
                    UserCrowdfundingQuery.OrderCount += 1;
                    //用户增加的股数
                    result = afterShareCnt - UserCrowdfundingQuery.CurrentShareCount;

                    //股东实际消费金额
                    UserCrowdfundingQuery.OrdersMoney += commodityOrder.RealPrice.Value;

                    //已获得股数大于总股数
                    if (CrowdfundingCountQuery.CurrentShareCount + result >= CrowdfundingCountQuery.ShareCount)
                    {
                        //剩余股数
                        var max = CrowdfundingCountQuery.ShareCount - CrowdfundingCountQuery.CurrentShareCount;
                        //众筹有效金额
                        commodityOrder.CrowdfundingPrice = max * CrowdfundingQuery.PerShareMoney -
                                                           (UserCrowdfundingQuery.Money -
                                                            UserCrowdfundingQuery.CurrentShareCount *
                                                            CrowdfundingQuery.PerShareMoney);
                        //用户能购买的股数
                        UserCrowdfundingQuery.CurrentShareCount += max;
                        //用户购买股数对应的金额
                        UserCrowdfundingQuery.Money += commodityOrder.CrowdfundingPrice;
                        //修改众筹状态为成功
                        CrowdfundingQuery.State = 1;
                        CrowdfundingQuery.EntityState = System.Data.EntityState.Modified;
                        contextSession.SaveObject(CrowdfundingQuery);
                        CrowdfundingCountQuery.CurrentShareCount = CrowdfundingCountQuery.ShareCount;
                    }
                    else
                    {
                        UserCrowdfundingQuery.Money += realPrice;
                        UserCrowdfundingQuery.CurrentShareCount =
                            (long)(UserCrowdfundingQuery.Money / CrowdfundingQuery.PerShareMoney);
                        CrowdfundingCountQuery.CurrentShareCount = CrowdfundingCountQuery.CurrentShareCount + result;
                        //众筹有效金额
                        commodityOrder.CrowdfundingPrice = realPrice;

                    }

                    contextSession.SaveObject(UserCrowdfundingQuery);
                    CrowdfundingCountQuery.EntityState = System.Data.EntityState.Modified;
                    contextSession.SaveObject(CrowdfundingCountQuery);
                    //众筹完成发消息
                    if (CrowdfundingQuery.State == 1)
                    {
                        CrowdfundingMessageDTO Message = new CrowdfundingMessageDTO();
                        Message.Now = DateTime.Now;
                        Message.StartTime = CrowdfundingQuery.StartTime;
                        Message.State = 1;
                        AddMessage addMessage = new AddMessage();
                        addMessage.SendMessage(CrowdfundingQuery.Id, CrowdfundingQuery.AppId,
                                               CrowdfundingQuery.StartTime, Message);
                    }

                    commodityOrder.IsCrowdfunding = 1;
                }
                else
                {
                    if (UserCrowdfundingQuery != null)
                    {
                        //众筹成功
                        //股东实际消费金额
                        UserCrowdfundingQuery.OrdersMoney += commodityOrder.RealPrice.Value;
                        //购买活动总订单数
                        UserCrowdfundingQuery.OrderCount += 1;
                        UserCrowdfundingQuery.EntityState = System.Data.EntityState.Modified;
                        contextSession.SaveObject(UserCrowdfundingQuery);
                    }
                    commodityOrder.IsCrowdfunding = 2;
                }

            }
            catch (Exception ex)
            {

                LogHelper.Error(string.Format("CommodityOrderSV-GetUserAccount。commodityOrder{0}", JsonHelper.JsonSerializer(commodityOrder)), ex);

            }
        }

        /// <summary>
        /// 支付成功后更新订单 金采团购活动使用
        /// </summary>
        /// <param name="orderId">订单Id</param>
        /// <returns></returns>
        public ResultDTO PayUpdateCommodityOrderForJcExt(Guid orderId)
        {
            LogHelper.Info(string.Format("开始PayUpdateCommodityOrderForJcExt,orderId:{0}", orderId));

            if (orderId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数不能为空" };
            }
            if (!OrderSV.LockOrder(orderId))
            {
                return new ResultDTO { ResultCode = 110, Message = "操作失败" };
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                StringBuilder orderErrorLog = new StringBuilder();
                List<Commodity> needRefreshCacheCommoditys = new List<Commodity>();

                var commodityOrder = CommodityOrder.ObjectSet().FirstOrDefault(p => p.Id == orderId);
                if (commodityOrder == null)
                    return new ResultDTO { ResultCode = 1, Message = "订单不存在" };
                if (commodityOrder.State != 0 && commodityOrder.State != 11 && commodityOrder.State != 4 && commodityOrder.State != 5 && commodityOrder.State != 6 && commodityOrder.State != 17)
                {
                    return new ResultDTO { ResultCode = 0, Message = "订单已支付" };
                }
                if (commodityOrder.Payment != 2001 || commodityOrder.JcActivityId == null || commodityOrder.JcActivityId == Guid.Empty)
                {
                    return new ResultDTO { ResultCode = 0, Message = "订单不是金采团购订单" };
                }
                int oldState = commodityOrder.State;

                var orderitemlist = OrderItem.ObjectSet().Where(n => n.CommodityOrderId == commodityOrder.Id).ToList();
                //处理订单商品
                var commodityIdList = orderitemlist.Select(a => a.CommodityId).Distinct().ToList();
                var commodityList = Commodity.ObjectSet().Where(a => commodityIdList.Contains(a.Id)).ToList();

                List<CommodityStock> commodityStockList = new List<CommodityStock>();
                var commodityStockIds = orderitemlist.Select(a => a.CommodityStockId).Distinct().ToList();
                if (commodityStockIds.Any())
                    commodityStockList =
                        CommodityStock.ObjectSet().Where(c => commodityStockIds.Contains(c.Id)).ToList();

                Dictionary<Guid, Commodity> littleStockComDict = new Dictionary<Guid, Commodity>();
                foreach (var orderItem in orderitemlist)
                {
                    //减库存 新逻辑  zgx-modify
                    Commodity com = commodityList.First(c => c.Id == orderItem.CommodityId);
                    CommodityStock cStock = null;
                    if (orderItem.CommodityStockId.HasValue && orderItem.CommodityStockId != Guid.Empty &&
                        orderItem.CommodityStockId != orderItem.CommodityId)
                        cStock = commodityStockList.First(c => c.Id == orderItem.CommodityStockId);
                    if (cStock != null)
                    {
                        if (com.Stock <= 0 || cStock.Stock <= 0)
                        {
                            orderErrorLog.Append(string.Format(" CommodityId:{0}商品库存不足", com.Id));
                            LogHelper.Error(string.Format("PayUpdateCommodityOrderSV:orderId:{0},商品库存不足", orderId));
                        }
                        com.Stock -= orderItem.Number;
                        com.Salesvolume += orderItem.Number;
                        com.ModifiedOn = DateTime.Now;
                        com.EntityState = System.Data.EntityState.Modified;

                        cStock.Stock -= orderItem.Number;
                        cStock.ModifiedOn = DateTime.Now;
                        cStock.EntityState = System.Data.EntityState.Modified;
                    }
                    else
                    {
                        if (com.Stock <= 0)
                        {
                            orderErrorLog.Append(string.Format(" CommodityId:{0}商品库存不足", com.Id));
                            LogHelper.Error(string.Format("PayUpdateCommodityOrderSV:orderId:{0},商品库存不足", orderId));
                        }
                        com.Stock -= orderItem.Number;
                        com.Salesvolume += orderItem.Number;
                        com.ModifiedOn = DateTime.Now;
                        com.EntityState = System.Data.EntityState.Modified;
                    }
                    if (com.Stock <= 1)
                    {
                        if (!littleStockComDict.ContainsKey(com.Id))
                            littleStockComDict.Add(com.Id, com);
                    }

                    needRefreshCacheCommoditys.RemoveAll(c => c.Id == com.Id);
                    needRefreshCacheCommoditys.Add(com);
                }

                //订单日志
                Journal journal = new Journal();
                journal.Id = Guid.NewGuid();
                journal.Code = commodityOrder.Code;
                journal.SubId = commodityOrder.UserId;
                journal.SubTime = DateTime.Now;
                journal.CommodityOrderId = orderId;
                journal.Name = "用户支付订单";
                journal.Details = string.Format("订单状态由{0}变为{1},支付方式为:{2},金采支付 ", oldState, 1,
                    "2001");
                if (orderErrorLog.Length > 0)
                {
                    journal.Details += orderErrorLog.ToString();
                }
                journal.StateFrom = oldState;
                journal.StateTo = 1;
                journal.IsPush = false;
                journal.OrderType = commodityOrder.OrderType;
                journal.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(journal);

                OrderEventHelper.OnOrderPaySuccess(commodityOrder);

                commodityOrder.State = 1;
                commodityOrder.PaymentTime = DateTime.Now;
                commodityOrder.ModifiedOn = DateTime.Now;
                commodityOrder.EntityState = System.Data.EntityState.Modified;
                contextSession.SaveObject(commodityOrder);

                //处理相关的金采活动的授权额度

                contextSession.SaveChanges();

                LogHelper.Info(string.Format("数据保存PayUpdateCommodityOrderExt,orderId:{0}", orderId));

                //刷新缓存商品缓存
                if (needRefreshCacheCommoditys.Any())
                {
                    needRefreshCacheCommoditys.ForEach(c => c.RefreshCache(EntityState.Modified));
                }
                LogHelper.Info(string.Format("刷新缓存PayUpdateCommodityOrderExt,orderId:{0}", orderId));

                //Guid EsAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
                //AddMessage addmassage = new AddMessage();
                //addmassage.AddMessages(orderId.ToString(), commodityOrder.UserId.ToString(), EsAppId, commodityOrder.Code, 1, "", "order");
                //LogHelper.Info(string.Format("消息发送PayUpdateCommodityOrderExt,orderId:{0}", orderId));

                return new ResultDTO { ResultCode = 0, Message = "Success" };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("订单状态修改服务异常。orderId{0}", orderId), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(orderId);
            }
        }

        /// <summary>
        /// 支付成功后更新订单
        /// </summary>
        /// <param name="orderId">订单Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="appId"></param>
        /// <param name="payment">支付方式</param>
        /// <param name="gold">订单中使用的金币个数</param>
        /// <param name="money">订单中使用钱支付部分金额</param>
        /// <param name="couponCount">订单中使用代金券支付部分金额</param>
        /// <returns></returns>
        public ResultDTO PayUpdateCommodityOrderExt(Guid orderId, Guid userId, Guid appId, int payment, ulong gold, decimal money, decimal couponCount)
        {
            LogHelper.Info(string.Format("开始PayUpdateCommodityOrderExt,orderId:{0},userId:{1},appId:{2},payment:{3},gold:{4},money:{5},couponCount:{6}", orderId, userId, appId, payment, gold, money, couponCount));

            if (orderId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数不能为空" };
            }
            //先解锁一下
            OrderSV.UnLockOrder(orderId);
            if (!OrderSV.LockOrder(orderId))
            {
                return new ResultDTO { ResultCode = 110, Message = "操作失败" };
            }
            try
            {
                List<Commodity> needRefreshCacheCommoditys = new List<Commodity>();
                List<TodayPromotion> needRefreshCacheTodayPromotions = new List<TodayPromotion>();

                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                AddMessage addmassage = new AddMessage();
                DateTime now = DateTime.Now;
                CommodityOrder commodityOrder = CommodityOrder.ObjectSet().FirstOrDefault(p => p.Id == orderId);
                if (commodityOrder == null)
                    return new ResultDTO { ResultCode = 1, Message = "订单不存在" };

                LogHelper.Info(string.Format("付款方式：paymnet:{0},commodityOrder.Payment:{1},订单状态:{2}", payment, commodityOrder.Payment, commodityOrder.State));
                if (commodityOrder.State != 0 && commodityOrder.State != 11 && commodityOrder.State != 4 && commodityOrder.State != 5 && commodityOrder.State != 6 && commodityOrder.State != 17)
                {
                    return new ResultDTO { ResultCode = 0, Message = "订单已支付" };
                }

                // 支付成功，解冻易捷币
                YJBHelper.PayOrder(commodityOrder);

                int oldState = commodityOrder.State;
                //订单id
                string odid = orderId.ToString();
                const string type = "order";
                //用户id
                string usid = commodityOrder.UserId.ToString();

                StringBuilder orderErrorLog = new StringBuilder();
                if (userId == Guid.Empty)
                {
                    userId = commodityOrder.UserId;
                }
                appId = commodityOrder.AppId;
                var orderitemlist = OrderItem.ObjectSet().Where(n => n.CommodityOrderId == commodityOrder.Id).ToList();

                //获取当时的积分比例 
                //易捷北京不走该逻辑
                int scoreCost = 0;
                if (CustomConfig.IsShareAsScore && commodityOrder.EsAppId != CustomConfig.YJAppId)
                {
                    if (!new ScoreSV().CheckOrderScoreCost(commodityOrder.AppId, out scoreCost, commodityOrder.SubTime))
                    {
                        return new ResultDTO { ResultCode = 120, Message = "操作失败，获取积分失败" };
                    }
                }


                JAP.BF.BE.Deploy.Base.ContextDTO contextDTO = ContextDTO ?? Jinher.AMP.BTP.Common.AuthorizeHelper.CoinInitAuthorizeInfo();

                #region 好运来订单支付后调用好运来接口
                if (commodityOrder.State == 1 && commodityOrder.SrcType == 1 && commodityOrder.SrcTagId != null && commodityOrder.SrcTagId != Guid.Empty)//好运来
                {
                    var LotteryResult = GameSV.Instance.UpdateWinPlayerBuyed(commodityOrder.SrcTagId.Value, commodityOrder.UserId);
                }
                #endregion



                //所有支付方式
                List<int> allPayTypes = new List<int>();
                List<PaySource> psList = PaySource.GetAllPaySources();
                if (psList != null && psList.Any())
                {
                    allPayTypes = psList.Select(ps => ps.Payment).Distinct().ToList();
                }
                if (allPayTypes.Contains(payment))
                {
                    commodityOrder.Payment = payment;
                }
                else
                {
                    return new ResultDTO { ResultCode = 1, Message = "支付方式不存在" };
                }

                LogHelper.Info(string.Format("付款方式PayUpdateCommodityOrderExt,orderId:{0}", orderId));

                //是否为拼团订单，拼团订单不发商品，并且订单状态改为16
                bool isDiyGroup = false;
                //拼团时，修改团订单状态，并修改参团人数
                var diyGroupInfo = (from dgOrder in DiyGroupOrder.ObjectSet()
                                    join dg in DiyGroup.ObjectSet()
                                        on dgOrder.DiyGroupId equals dg.Id
                                    where dgOrder.OrderId == commodityOrder.Id
                                    //&& dgOrder.State == 0
                                    select new
                                    {
                                        DiyGroupOrder = dgOrder,
                                        DiyGroup = dg
                                    }).FirstOrDefault();
                if (diyGroupInfo != null && diyGroupInfo.DiyGroup != null && diyGroupInfo.DiyGroupOrder != null)
                {
                    isDiyGroup = true;

                    var diyGroupOrder = diyGroupInfo.DiyGroupOrder;
                    var diyGroup = diyGroupInfo.DiyGroup;

                    diyGroupOrder.State = 1;
                    diyGroupOrder.ModifiedOn = DateTime.Now;
                    diyGroupOrder.EntityState = EntityState.Modified;

                    diyGroup.JoinNumber = diyGroup.JoinNumber + 1;
                    if (diyGroup.State == 0 || diyGroup.State == 1)
                    {
                        var proP = (from m in DiyGroup.ObjectSet()
                                    join p in Promotion.ObjectSet()
                                        on m.PromotionId equals p.Id
                                    join proItem in PromotionItems.ObjectSet()
                                        on m.PromotionId equals proItem.PromotionId
                                    where m.Id == diyGroup.Id
                                    select new
                                    {
                                        Promotion = p,
                                        PromotionItems = proItem
                                    }).FirstOrDefault();
                        var groupMinVolume = proP.Promotion.GroupMinVolume;
                        if (groupMinVolume <= diyGroup.JoinNumber)
                        {
                            diyGroup.State = 2;
                        }
                        else
                        {
                            diyGroup.State = 1;
                        }
                    }
                    diyGroup.ModifiedOn = DateTime.Now;
                    diyGroup.EntityState = EntityState.Modified;
                }

                LogHelper.Info(string.Format("拼团PayUpdateCommodityOrderExt,orderId:{0}", orderId));

                //处理订单商品
                var commodityIdList = orderitemlist.Select(a => a.CommodityId).Distinct().ToList();
                var commodityList = Commodity.ObjectSet()
                    .Where(a => commodityIdList.Contains(a.Id)).ToList();

                if (commodityOrder.SrcType == 1 && commodityOrder.SrcTagId != null && commodityOrder.SrcTagId != Guid.Empty && commodityList.Count > 0)//好运来
                {
                    Guid firstComId = commodityList[0].Id;
                    GenUserPrizeRecord prizeResultRecord = (from e in GenUserPrizeRecord.ObjectSet()
                                                            where e.CommodityId == firstComId && e.UserId == userId && e.PromotionId == commodityOrder.SrcTagId
                                                            select e).FirstOrDefault();

                    if (prizeResultRecord == null)
                    {
                        orderErrorLog.Append(" 您没有权限购买此商品");
                        LogHelper.Error(string.Format("PayUpdateCommodityOrderSV:orderId:{0},您没有权限购买此商品", orderId));
                        //return new ResultDTO { ResultCode = 1, Message = "您没有权限购买此商品" };
                    }
                    if (prizeResultRecord.IsBuyed)
                    {
                        orderErrorLog.Append(" 您已经购买过此商品，现在没有权限购买此商品");
                        LogHelper.Error(string.Format("PayUpdateCommodityOrderSV:orderId:{0},您已经购买过此商品，现在没有权限购买此商品", orderId));
                        //return new ResultDTO { ResultCode = 1, Message = "您已经购买过此商品，现在没有权限购买此商品" };
                    }
                    if (prizeResultRecord.Price != commodityOrder.Price)
                    {
                        orderErrorLog.Append(" 您不能以此价格购买此商品");
                        LogHelper.Error(string.Format("PayUpdateCommodityOrderSV:orderId:{0},您不能以此价格购买此商品", orderId));
                        //return new ResultDTO { ResultCode = 1, Message = "您不能以此价格购买此商品" };
                    }
                    prizeResultRecord.OrderId = commodityOrder.Id;
                    prizeResultRecord.EntityState = System.Data.EntityState.Modified;
                    prizeResultRecord.IsBuyed = true;
                    contextSession.SaveObject(prizeResultRecord);
                }

                LogHelper.Info(string.Format("商品PayUpdateCommodityOrderExt,orderId:{0}", orderId));

                Jinher.AMP.App.Deploy.ApplicationDTO applicationDTO = APPBP.Instance.GetAppById(appId);

                List<CommodityStock> commodityStockList = new List<CommodityStock>();
                var commodityStockIds = orderitemlist.Select(a => a.CommodityStockId).Distinct().ToList();
                if (commodityStockIds.Any())
                    commodityStockList = CommodityStock.ObjectSet().Where(c => commodityStockIds.Contains(c.Id)).ToList();

                Dictionary<Guid, Commodity> littleStockComDict = new Dictionary<Guid, Commodity>();
                foreach (var orderItem in orderitemlist)
                {
                    //减库存 新逻辑  zgx-modify
                    Commodity com = commodityList.First(c => c.Id == orderItem.CommodityId);
                    CommodityStock cStock = null;
                    if (orderItem.CommodityStockId.HasValue && orderItem.CommodityStockId != Guid.Empty && orderItem.CommodityStockId != orderItem.CommodityId)
                        cStock = commodityStockList.First(c => c.Id == orderItem.CommodityStockId);
                    if (cStock != null)
                    {
                        if (com.Stock <= 0 || cStock.Stock <= 0)
                        {
                            orderErrorLog.Append(string.Format(" CommodityId:{0}商品库存不足", com.Id));
                            LogHelper.Error(string.Format("PayUpdateCommodityOrderSV:orderId:{0},商品库存不足", orderId));
                        }
                        com.Stock -= orderItem.Number;
                        com.Salesvolume += orderItem.Number;
                        com.ModifiedOn = now;
                        com.EntityState = System.Data.EntityState.Modified;

                        cStock.Stock -= orderItem.Number;
                        cStock.ModifiedOn = now;
                        cStock.EntityState = System.Data.EntityState.Modified;
                    }
                    else
                    {
                        if (com.Stock <= 0)
                        {
                            orderErrorLog.Append(string.Format(" CommodityId:{0}商品库存不足", com.Id));
                            LogHelper.Error(string.Format("PayUpdateCommodityOrderSV:orderId:{0},商品库存不足", orderId));
                        }
                        com.Stock -= orderItem.Number;
                        com.Salesvolume += orderItem.Number;
                        com.ModifiedOn = now;
                        com.EntityState = System.Data.EntityState.Modified;
                    }
                    if (com.Stock <= 1)
                    {
                        if (!littleStockComDict.ContainsKey(com.Id))
                            littleStockComDict.Add(com.Id, com);
                    }

                    needRefreshCacheCommoditys.RemoveAll(c => c.Id == com.Id);
                    needRefreshCacheCommoditys.Add(com);
                }

                LogHelper.Info(string.Format("库存1 PayUpdateCommodityOrderExt,orderId:{0}", orderId));

                if (!isDiyGroup)
                {
                    if (littleStockComDict.Any())
                    {
                        if (applicationDTO != null && applicationDTO.OwnerId.HasValue && applicationDTO.OwnerId != Guid.Empty && applicationDTO.OwnerType == Jinher.AMP.App.Deploy.Enum.AppOwnerTypeEnum.Personal)
                        {
                            Jinher.AMP.Info.Deploy.CustomDTO.MessageForAddDTO messageAdd =
                                 new Info.Deploy.CustomDTO.MessageForAddDTO();
                            string _appName = APPSV.GetAppName(commodityOrder.AppId);
                            foreach (var com in littleStockComDict.Values.ToList())
                            {
                                //根据Appid判断是否为个人商家 如果为个人商家 发送 库存提醒消息
                                try
                                {

                                    //推送消息
                                    List<Guid> lGuid = new List<Guid>();
                                    lGuid.Add(applicationDTO.OwnerId.Value);
                                    messageAdd.PublishTime = DateTime.Now;
                                    messageAdd.ReceiverUserId = lGuid;

                                    messageAdd.SenderType = Info.Deploy.Enum.SenderType.System;
                                    if (!string.IsNullOrWhiteSpace(com.No_Code))
                                    {
                                        messageAdd.Title = "【" + _appName + "】" + "【" + com.No_Code + "】【" + com.Name + "】商品数量紧张，请关注！";
                                        messageAdd.Content = "【" + _appName + "】" + "【" + com.No_Code + "】【" + com.Name + "】商品数量紧张，请关注！";
                                    }
                                    else
                                    {
                                        messageAdd.Title = "【" + _appName + "】" + "【" + com.Name + "】商品数量紧张，请关注！";
                                        messageAdd.Content = "【" + _appName + "】" + "【" + com.Name + "】商品数量紧张，请关注！";
                                    }

                                    messageAdd.ReceiverRange = Info.Deploy.Enum.ReceiverRange.SpecifiedUser;
                                    Jinher.AMP.BTP.TPS.InfoSV.Instance.AddSystemMessage(messageAdd);
                                }
                                catch (Exception ex)
                                {

                                    LogHelper.Error(string.Format("InfoManageSV服务异常:获取应用信息异常。orderId：{0}", orderId), ex);
                                }
                            }
                        }
                    }

                    LogHelper.Info(string.Format("库存2 PayUpdateCommodityOrderExt,orderId:{0}", orderId));

                    //发送微信消息
                    new BTPMessageSV().SendPayWxMessage(commodityOrder);

                    SendMessageToAppOwner(1, appId, commodityOrder, applicationDTO, contextDTO);
                    if (commodityOrder.SelfTakeFlag == 1)
                    {
                        SendMessageToSelfTakeManagers(1, commodityOrder);
                    }
                    try
                    {
                        #region 发消息操作
                        decimal realPrice = commodityOrder.IsModifiedPrice ? commodityOrder.RealPrice.Value : commodityOrder.Price;

                        List<Guid> userIds = new List<Guid>();
                        if (commodityOrder.SrcType == 33 || commodityOrder.SrcType == 34)
                        {
                            OrderShareMess tempShare =
                                OrderShareMess.ObjectSet().FirstOrDefault(c => c.OrderId == commodityOrder.Id);
                            if (tempShare != null && !string.IsNullOrEmpty(tempShare.ShareId) &&
                                tempShare.ShareId.ToLower() != "null" && tempShare.ShareId.ToLower() != "undefined")
                            {
                                Jinher.AMP.SNS.Deploy.CustomDTO.ReturnInfoDTO<Guid> shareServiceResultMes = new Jinher.AMP.SNS.Deploy.CustomDTO.ReturnInfoDTO<Guid>();
                                var buyerPercent = CustomConfig.SaleShare.Commission * CustomConfig.SaleShare.BuyerPercent;
                                shareServiceResultMes = Jinher.AMP.BTP.TPS.SNSSV.Instance.GetShareUserId(tempShare.ShareId);
                                if (shareServiceResultMes != null)
                                {
                                    if (shareServiceResultMes.Code == "0")
                                    {
                                        userIds.Add(shareServiceResultMes.Content);
                                        SendMessageToPayment(userIds, "Payment", realPrice.ToString(), contextDTO, buyerPercent);
                                    }
                                    else
                                    {
                                        LogHelper.Error(string.Format("付款后发送消息失败 返回code为 1：\"根据分享Id获取分享人Id\" 不成功,分享Id={0}，返回结果={1}", tempShare.ShareId, JsonHelper.JsonSerializer(shareServiceResultMes)));
                                    }
                                }
                            }
                        }

                        if (commodityOrder.SrcAppId != null && commodityOrder.SrcAppId != Guid.Empty)
                        {
                            var result = APPSV.Instance.GetAppOwnerInfo(Guid.Parse(commodityOrder.SrcAppId.ToString()));
                            if (result.OwnerType == App.Deploy.Enum.AppOwnerTypeEnum.Org)
                            {
                                List<Guid> orgUserIds = Jinher.AMP.BTP.TPS.EBCSV.Instance.GetUserIdsByOrgIdAndCode(result.OwnerId, "ReceiveRed");

                                SendMessageToPayment(orgUserIds, "Payment", realPrice.ToString(), contextDTO, CustomConfig.ShareOwner.OwnerPercent);
                            }
                            else if (result.OwnerType == App.Deploy.Enum.AppOwnerTypeEnum.Personal)
                            {
                                userIds.Clear();
                                userIds.Add(result.OwnerId);
                                SendMessageToPayment(userIds, "Payment", realPrice.ToString(), contextDTO, CustomConfig.ShareOwner.OwnerPercent);
                            }
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(string.Format("SendMessageToPayment 发送消息服务异常:。orderId：{0}", commodityOrder.Id), ex);
                    }
                }

                LogHelper.Info(string.Format("库存PayUpdateCommodityOrderExt,orderId:{0}", orderId));

                commodityOrder.PaymentTime = now;

                commodityOrder.ModifiedOn = now;
                if (commodityOrder.OrderType == 2)
                {
                    commodityOrder.State = 18;
                }
                else if (!isDiyGroup)
                {
                    commodityOrder.State = 1;
                }
                else
                {
                    //拼团时，修改团订单状态为16
                    commodityOrder.State = 16;
                }

                commodityOrder.EntityState = System.Data.EntityState.Modified;
                if (gold != 0)
                {
                    commodityOrder.GoldPrice = gold * 0.001m;
                }
                if (commodityOrder.GoldPrice + money + couponCount != commodityOrder.RealPrice)
                {
                    LogHelper.Info(string.Format("混合支付金额不一致异常：金币抵用值:{0},在线支付值:{1},订单实收价:{2}", commodityOrder.GoldPrice, money, commodityOrder.RealPrice));
                }
                commodityOrder.GoldCoupon = couponCount;

                //订单日志
                Journal journal = new Journal();
                journal.Id = Guid.NewGuid();
                journal.Code = commodityOrder.Code;
                journal.SubId = userId;
                journal.SubTime = now;
                journal.CommodityOrderId = orderId;
                journal.Name = "用户支付订单";
                journal.Details = string.Format("订单状态由{0}变为{1},支付方式为:{2} ", oldState, commodityOrder.State, payment);
                if (orderErrorLog.Length > 0)
                {
                    journal.Details += orderErrorLog.ToString();
                }
                journal.StateFrom = oldState;
                journal.StateTo = commodityOrder.State;
                journal.IsPush = false;
                journal.OrderType = commodityOrder.OrderType;

                journal.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(journal);

                //担保交易
                IEnumerable<int> secTrans = allPayTypes.Where(p => p < 1000 && p != 1 && p != 2);

                #region  发票

                var invOrder = (from inv in Invoice.ObjectSet()
                                where inv.CommodityOrderId == commodityOrder.Id
                                select inv).FirstOrDefault();
                if (invOrder != null)
                {
                    invOrder.State = 1;
                    invOrder.EntityState = EntityState.Modified;
                    invOrder.ModifiedOn = now;
                    GetInvoiceJounal(invOrder.Id, Guid.Empty, invOrder.State, 1);
                }

                #endregion

                #region 众筹订单付款

                if (CustomConfig.CrowdfundingFlag && secTrans.Contains(payment) && commodityOrder.RealPrice > 0)
                {
                    CrowdfundingPay(contextSession, now, commodityOrder);
                }

                #endregion

                #region 取得订单可分金额以及分享信息

                decimal shareMoney = commodityOrder.IsModifiedPrice
                                   ? commodityOrder.RealPrice.Value
                                   : commodityOrder.RealPrice.Value - commodityOrder.Freight;
                string shareId = OrderShareMess.ObjectSet().Where(c => c.OrderId == commodityOrder.Id).Select(c => c.ShareId).FirstOrDefault();
                //取得分享人
                Guid shareUseId = Guid.Empty;
                if (!string.IsNullOrEmpty(shareId))
                {
                    SNS.Deploy.CustomDTO.ReturnInfoDTO<Guid> shareServiceResult = SNSSV.Instance.GetShareUserId(shareId);
                    if (shareServiceResult != null && shareServiceResult.Code == "0")
                    {

                        shareUseId = shareServiceResult.Content;
                    }
                    else
                    {
                        LogHelper.Error(string.Format("金币回调时，根据分享Id获取分享人Id\" 不成功,分享Id={0}，返回结果={1}", shareId, JsonHelper.JsonSerializer(shareServiceResult)));
                    }
                }

                #endregion


                #region 推广主

                bool isSpread = CalcOrderSpread(commodityOrder, orderitemlist, userId, contextSession, commodityList);

                #endregion

                #region 获取实时 抵用券和易捷币 用于佣金计算

                var yjbInfo = new YJB.Deploy.CustomDTO.OrderYJBInfoDTO();
                var yjCouponInfo = new YJB.Deploy.CustomDTO.OrderYJCouponInfoDTO();

                // 查询商品易捷币抵用数量
                var yjInfo = YJBSV.GetOrderItemYJInfo(commodityOrder.EsAppId.Value, commodityOrder.Id);
                if (yjInfo.IsSuccess)
                {
                    yjbInfo = yjInfo.Data.YJBInfo;
                    yjCouponInfo = yjInfo.Data.YJCouponInfo;
                }

                #endregion
                //是否渠道推广
                bool isSai = false;
                if (!CustomConfig.IsChannelShare)
                {
                    isSai = isSpread;
                }
                else if (!isSpread)
                {
                    #region 渠道推广

                    var isChannelShareFunction = BACBP.CheckChannel(commodityOrder.AppId);
                    var saiCode = ZPHSV.Instance.GetPromoCodeByShareId(commodityOrder.EsAppId.Value, shareId);
                    LogHelper.Info(string.Format("渠道推广：订单Id:{0} ,shareId:{1},shareMoney:{2}, isChannelShareFunction:{3} ,shareUseId:{4},saiCode:{5}  ", commodityOrder.Id, shareId, shareMoney, isChannelShareFunction, shareUseId, saiCode));
                    if (shareMoney > 0 && isChannelShareFunction && shareUseId != Guid.Empty && saiCode != Guid.Empty)
                    {
                        var channelSharePercent = AppExtension.ObjectSet()
                               .Where(t => t.Id == commodityOrder.EsAppId)
                               .Select(t => t.ChannelSharePercent)
                               .FirstOrDefault();
                        if (channelSharePercent > 0)
                        {
                            isSai = true;
                            var orderItemList = OrderItem.ObjectSet().Where(c => c.CommodityOrderId == commodityOrder.Id).ToList();
                            //渠道推广实发金额
                            decimal channelShareMoney = 0.0m;
                            //渠道推广应得金额
                            decimal channelShouldShareMoney = 0.0m;
                            var orderPayDetailsList = OrderPayDetail.ObjectSet().Where(c => c.OrderId == commodityOrder.Id).ToList();

                            //使用了优惠券或积分
                            if (orderPayDetailsList.Any())
                            {
                                var couponModel = orderPayDetailsList.Where(t => t.ObjectType == 1).FirstOrDefault();
                                var integrationAmount =
                                    orderPayDetailsList.Where(t => t.ObjectType == 2).Select(t => t.Amount).FirstOrDefault();
                                if (couponModel != null)
                                {
                                    //使用了优惠券
                                    if (couponModel.CouponType == 0)
                                    {
                                        //当优惠券为店铺优惠券时
                                        foreach (var orderItem in orderItemList)
                                        {
                                            if (yjbInfo != null && yjbInfo.Items != null)
                                            {
                                                var currentCommodityYjbInfo = yjbInfo.Items.FirstOrDefault(c => c.CommodityId == orderItem.CommodityId);
                                                if (currentCommodityYjbInfo != null && currentCommodityYjbInfo.InsteadCashAmount > 0)
                                                {
                                                    orderItem.YjbPrice = currentCommodityYjbInfo.InsteadCashAmount;
                                                }
                                            }

                                            decimal orderItemAmount = 0;
                                            if (orderItem.DiscountPrice.Value > 0)
                                            {
                                                orderItemAmount = orderItem.DiscountPrice.Value * orderItem.Number;
                                            }
                                            else
                                            {
                                                orderItemAmount = orderItem.RealPrice.Value * orderItem.Number;
                                            }

                                            if (orderItem.YjbPrice != null)
                                            {
                                                orderItemAmount = orderItemAmount - (decimal)orderItem.YjbPrice;
                                            }
                                            //if (orderItem.CouponPrice != null)
                                            //{
                                            //    orderItemAmount = orderItemAmount - (decimal)orderItem.CouponPrice;
                                            //}
                                            decimal moneyTmp = (orderItemAmount -
                                                                (orderItemAmount / commodityOrder.Price) *
                                                                couponModel.Amount -
                                                                (orderItemAmount / commodityOrder.Price) *
                                                                integrationAmount) * channelSharePercent.Value;

                                            OrderItemShare oiShare = OrderItemShare.CreateOrderItemShare();
                                            oiShare.Id = Guid.NewGuid();
                                            oiShare.SubTime = DateTime.Now;
                                            oiShare.ModifiedOn = DateTime.Now;
                                            oiShare.SubId = userId;
                                            oiShare.SharePrice = orderItemAmount;
                                            oiShare.Commission = getCommisionWithCost(moneyTmp, scoreCost);
                                            oiShare.ShouldCommission = moneyTmp.ToMoney();
                                            oiShare.SharePercent = channelSharePercent.Value;
                                            oiShare.OrderId = orderItem.CommodityOrderId;
                                            oiShare.OrderItemId = orderItem.Id;
                                            oiShare.PayeeType = 12;
                                            oiShare.PayeeId = shareUseId;
                                            oiShare.ShareKey = saiCode.ToString();
                                            oiShare.EntityState = EntityState.Added;
                                            contextSession.SaveObject(oiShare);

                                            channelShareMoney += oiShare.Commission;
                                            channelShouldShareMoney += oiShare.ShouldCommission;
                                        }
                                    }
                                    else if (couponModel.CouponType == 1)
                                    {
                                        //当优惠券为商品优惠券时
                                        var couponCommodityAccount =
                                            orderItemList.Where(
                                                t => couponModel.CommodityIds.Contains(t.CommodityId.ToString()))
                                                         .Sum(t => t.RealPrice.Value * t.Number);

                                        if (couponCommodityAccount > 0)
                                        {
                                            foreach (var orderItem in orderItemList)
                                            {
                                                if (yjbInfo != null && yjbInfo.Items != null)
                                                {
                                                    var currentCommodityYjbInfo = yjbInfo.Items.FirstOrDefault(c => c.CommodityId == orderItem.CommodityId);
                                                    if (currentCommodityYjbInfo != null && currentCommodityYjbInfo.InsteadCashAmount > 0)
                                                    {
                                                        orderItem.YjbPrice = currentCommodityYjbInfo.InsteadCashAmount;
                                                    }
                                                }

                                                decimal orderItemAmount = 0;
                                                if (orderItem.DiscountPrice.Value > 0)
                                                {
                                                    orderItemAmount = orderItem.DiscountPrice.Value * orderItem.Number;
                                                }
                                                else
                                                {
                                                    orderItemAmount = orderItem.RealPrice.Value * orderItem.Number;
                                                }

                                                if (orderItem.YjbPrice != null)
                                                {
                                                    orderItemAmount = orderItemAmount - (decimal)orderItem.YjbPrice;
                                                }
                                                //if (orderItem.CouponPrice != null)
                                                //{
                                                //    orderItemAmount = orderItemAmount - (decimal)orderItem.CouponPrice;
                                                //}
                                                decimal moneyTmp = 0m;
                                                if (couponModel.CommodityIds.Contains(orderItem.CommodityId.ToString()))
                                                {
                                                    moneyTmp = (orderItemAmount -
                                                                (orderItemAmount /
                                                                 couponCommodityAccount) *
                                                                couponModel.Amount -
                                                                (orderItemAmount / commodityOrder.Price) *
                                                                integrationAmount) * channelSharePercent.Value;
                                                }
                                                else
                                                {
                                                    moneyTmp = (orderItemAmount -
                                                                (orderItemAmount / commodityOrder.Price) *
                                                                integrationAmount) * channelSharePercent.Value;
                                                }

                                                OrderItemShare oiShare = OrderItemShare.CreateOrderItemShare();
                                                oiShare.Id = Guid.NewGuid();
                                                oiShare.SubTime = DateTime.Now;
                                                oiShare.ModifiedOn = DateTime.Now;
                                                oiShare.SubId = userId;
                                                oiShare.SharePrice = orderItemAmount;
                                                oiShare.Commission = getCommisionWithCost(moneyTmp, scoreCost);
                                                oiShare.ShouldCommission = moneyTmp.ToMoney();
                                                oiShare.SharePercent = channelSharePercent.Value;
                                                oiShare.OrderId = orderItem.CommodityOrderId;
                                                oiShare.OrderItemId = orderItem.Id;
                                                oiShare.PayeeType = 12;
                                                oiShare.PayeeId = shareUseId;
                                                oiShare.ShareKey = saiCode.ToString();
                                                oiShare.EntityState = EntityState.Added;
                                                contextSession.SaveObject(oiShare);

                                                channelShareMoney += oiShare.Commission;
                                                channelShouldShareMoney += oiShare.ShouldCommission;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    //没有使用优惠券
                                    foreach (var orderItem in orderItemList)
                                    {
                                        if (yjbInfo != null && yjbInfo.Items != null)
                                        {
                                            var currentCommodityYjbInfo = yjbInfo.Items.FirstOrDefault(c => c.CommodityId == orderItem.CommodityId);
                                            if (currentCommodityYjbInfo != null && currentCommodityYjbInfo.InsteadCashAmount > 0)
                                            {
                                                orderItem.YjbPrice = currentCommodityYjbInfo.InsteadCashAmount;
                                            }
                                        }

                                        decimal orderItemAmount = 0;
                                        if (orderItem.DiscountPrice.Value > 0)
                                        {
                                            orderItemAmount = orderItem.DiscountPrice.Value * orderItem.Number;
                                        }
                                        else
                                        {
                                            orderItemAmount = orderItem.RealPrice.Value * orderItem.Number;
                                        }

                                        if (orderItem.YjbPrice != null)
                                        {
                                            orderItemAmount = orderItemAmount - (decimal)orderItem.YjbPrice;
                                        }
                                        if (orderItem.CouponPrice != null)
                                        {
                                            orderItemAmount = orderItemAmount - (decimal)orderItem.CouponPrice;
                                        }
                                        decimal moneyTmp = (orderItemAmount -
                                                            (orderItemAmount / commodityOrder.Price) *
                                                            integrationAmount) * channelSharePercent.Value;

                                        OrderItemShare oiShare = OrderItemShare.CreateOrderItemShare();
                                        oiShare.Id = Guid.NewGuid();
                                        oiShare.SubTime = DateTime.Now;
                                        oiShare.ModifiedOn = DateTime.Now;
                                        oiShare.SubId = userId;
                                        oiShare.SharePrice = orderItemAmount;
                                        oiShare.Commission = getCommisionWithCost(moneyTmp, scoreCost);
                                        oiShare.ShouldCommission = moneyTmp.ToMoney();
                                        oiShare.SharePercent = channelSharePercent.Value;
                                        oiShare.OrderId = orderItem.CommodityOrderId;
                                        oiShare.OrderItemId = orderItem.Id;
                                        oiShare.PayeeType = 12;
                                        oiShare.PayeeId = shareUseId;
                                        oiShare.ShareKey = saiCode.ToString();
                                        oiShare.EntityState = EntityState.Added;
                                        contextSession.SaveObject(oiShare);

                                        channelShareMoney += oiShare.Commission;
                                        channelShouldShareMoney += oiShare.ShouldCommission;
                                    }
                                }
                            }
                            else
                            {
                                foreach (var orderItem in orderItemList)
                                {
                                    if (yjbInfo != null && yjbInfo.Items != null)
                                    {
                                        var currentCommodityYjbInfo = yjbInfo.Items.FirstOrDefault(c => c.CommodityId == orderItem.CommodityId);
                                        if (currentCommodityYjbInfo != null && currentCommodityYjbInfo.InsteadCashAmount > 0)
                                        {
                                            orderItem.YjbPrice = currentCommodityYjbInfo.InsteadCashAmount;
                                        }
                                    }

                                    decimal orderItemAmount = 0;
                                    if (orderItem.DiscountPrice.Value > 0)
                                    {
                                        orderItemAmount = orderItem.DiscountPrice.Value * orderItem.Number;
                                    }
                                    else
                                    {
                                        orderItemAmount = orderItem.RealPrice.Value * orderItem.Number;
                                    }

                                    if (orderItem.YjbPrice != null)
                                    {
                                        orderItemAmount = orderItemAmount - (decimal)orderItem.YjbPrice;
                                    }
                                    if (orderItem.CouponPrice != null)
                                    {
                                        orderItemAmount = orderItemAmount - (decimal)orderItem.CouponPrice;
                                    }
                                    decimal moneyTmp = orderItemAmount * channelSharePercent.Value;

                                    OrderItemShare oiShare = OrderItemShare.CreateOrderItemShare();
                                    oiShare.Id = Guid.NewGuid();
                                    oiShare.SubTime = DateTime.Now;
                                    oiShare.ModifiedOn = DateTime.Now;
                                    oiShare.SubId = userId;
                                    oiShare.SharePrice = orderItemAmount;
                                    oiShare.Commission = getCommisionWithCost(moneyTmp, scoreCost);
                                    oiShare.ShouldCommission = moneyTmp.ToMoney();
                                    oiShare.SharePercent = channelSharePercent.Value;
                                    oiShare.OrderId = orderItem.CommodityOrderId;
                                    oiShare.OrderItemId = orderItem.Id;
                                    oiShare.PayeeType = 12;
                                    oiShare.PayeeId = shareUseId;
                                    oiShare.ShareKey = saiCode.ToString();
                                    oiShare.EntityState = EntityState.Added;
                                    contextSession.SaveObject(oiShare);

                                    channelShareMoney += oiShare.Commission;
                                    channelShouldShareMoney += oiShare.ShouldCommission;
                                }
                            }

                            //渠道分享佣金
                            commodityOrder.ChannelShareMoney = channelShareMoney;

                            //保存渠道分成到 OrderShare 表
                            if (shareUseId != Guid.Empty)
                            {
                                decimal sharePriceTotal = orderitemlist.Sum(t => t.RealPrice.Value * t.Number);

                                OrderShare os = OrderShare.CreateOrderShare();
                                os.Id = Guid.NewGuid();
                                os.SubTime = DateTime.Now;
                                os.ModifiedOn = DateTime.Now;
                                os.SubId = userId;
                                os.SharePrice = sharePriceTotal;
                                os.Commission = channelShareMoney;
                                os.ShouldCommission = channelShouldShareMoney;
                                os.SharePercent = channelSharePercent.Value;
                                os.OrderId = commodityOrder.Id;
                                os.PayeeType = 12;
                                os.PayeeId = shareUseId;
                                os.ShareKey = saiCode.ToString();

                                os.EntityState = EntityState.Added;
                                contextSession.SaveObject(os);
                            }
                        }
                    }

                    #endregion
                }

                bool isDistribut = false;
                if (!isSai)
                {
                    //三级分销
                    CalcOrderDistribut(commodityOrder, orderitemlist, commodityList[0].AppId, userId, scoreCost, out isDistribut);
                }

                if (!isDistribut)
                {
                    #region 分成推广（众销）

                    var isSharePromotionFunction = BACBP.CheckSharePromotion(commodityOrder.AppId);
                    LogHelper.Info(string.Format("分成推广：订单Id:{0} ,shareUseId:{1},shareMoney:{2}, isSharePromotionFunction:{3} ,appId:{4},esAppId:{5}  ", commodityOrder.Id, shareUseId, shareMoney, isSharePromotionFunction, commodityOrder.AppId, commodityOrder.EsAppId));

                    //只有本应用下单才会走分成推广
                    if (shareMoney > 0 && isSharePromotionFunction && shareUseId != Guid.Empty)
                    {
                        var orderItemList = OrderItem.ObjectSet().Where(c => c.CommodityOrderId == commodityOrder.Id).ToList();
                        //众销推广实发金额
                        decimal commission = 0.0m;
                        //众销推广应发金额
                        decimal shouldCommission = 0.0m;
                        var orderPayDetailsList = OrderPayDetail.ObjectSet().Where(c => c.OrderId == commodityOrder.Id).ToList();
                        var appext = AppExtension.ObjectSet().FirstOrDefault(c => c.Id == commodityOrder.AppId);
                        //是否使用全局设置 
                        decimal globalAmount = 0m;
                        //商品设置列表 
                        List<Commodity> commodityShareList = null;
                        if (appext != null && appext.IsDividendAll == true && appext.SharePercent > 0m)
                        {
                            globalAmount = appext.SharePercent;
                        }
                        else
                        {
                            var comIdList = orderItemList.Select(c => c.CommodityId).Distinct().ToList();
                            commodityShareList = Commodity.ObjectSet().Where(c => c.SharePercent > 0 && comIdList.Contains(c.Id)).ToList();
                        }
                        //使用了优惠券或积分
                        if (orderPayDetailsList.Any())
                        {
                            var couponModel = orderPayDetailsList.FirstOrDefault(t => t.ObjectType == 1);
                            var integrationAmount =
                                orderPayDetailsList.Where(t => t.ObjectType == 2).Select(t => t.Amount).FirstOrDefault();
                            if (couponModel != null)
                            {
                                //使用了优惠券
                                if (couponModel.CouponType == 0)
                                {
                                    //当优惠券为店铺优惠券时
                                    foreach (var orderItem in orderItemList)
                                    {
                                        if (yjbInfo != null && yjbInfo.Items != null)
                                        {
                                            var currentCommodityYjbInfo = yjbInfo.Items.FirstOrDefault(c => c.CommodityId == orderItem.CommodityId);
                                            if (currentCommodityYjbInfo != null && currentCommodityYjbInfo.InsteadCashAmount > 0)
                                            {
                                                orderItem.YjbPrice = currentCommodityYjbInfo.InsteadCashAmount;
                                            }
                                        }

                                        decimal orderItemAmount = 0;
                                        if (orderItem.DiscountPrice.Value > 0)
                                        {
                                            orderItemAmount = orderItem.DiscountPrice.Value * orderItem.Number;
                                        }
                                        else
                                        {
                                            orderItemAmount = orderItem.RealPrice.Value * orderItem.Number;
                                        }

                                        if (orderItem.YjbPrice != null)
                                        {
                                            orderItemAmount = orderItemAmount - (decimal)orderItem.YjbPrice;
                                        }
                                        //if (orderItem.CouponPrice != null)
                                        //{
                                        //    orderItemAmount = orderItemAmount - (decimal)orderItem.CouponPrice;
                                        //}
                                        decimal sharePercent = globalAmount;
                                        if (sharePercent <= 0)
                                        {
                                            var sharePercentTmp =
                                                 commodityShareList.Where(t => t.Id == orderItem.CommodityId)
                                                                   .Select(t => t.SharePercent)
                                                                   .FirstOrDefault();
                                            sharePercent = sharePercentTmp.HasValue ? sharePercentTmp.Value : 0m;
                                        }
                                        decimal moneyTmp = (orderItemAmount -
                                                            (orderItemAmount / commodityOrder.Price) *
                                                            couponModel.Amount -
                                                            (orderItemAmount / commodityOrder.Price) *
                                                            integrationAmount) * sharePercent;

                                        OrderItemShare oiShare = OrderItemShare.CreateOrderItemShare();
                                        oiShare.Id = Guid.NewGuid();
                                        oiShare.SubTime = DateTime.Now;
                                        oiShare.ModifiedOn = DateTime.Now;
                                        oiShare.SubId = userId;
                                        oiShare.SharePrice = orderItemAmount;
                                        oiShare.Commission = getCommisionWithCost(moneyTmp, scoreCost);
                                        oiShare.ShouldCommission = moneyTmp.ToMoney();
                                        oiShare.SharePercent = sharePercent;
                                        oiShare.OrderId = orderItem.CommodityOrderId;
                                        oiShare.OrderItemId = orderItem.Id;
                                        oiShare.PayeeType = 3;
                                        oiShare.PayeeId = shareUseId;
                                        oiShare.ShareKey = shareId;
                                        oiShare.EntityState = EntityState.Added;
                                        contextSession.SaveObject(oiShare);

                                        commission += oiShare.Commission;
                                        shouldCommission += oiShare.ShouldCommission;
                                    }
                                }
                                else if (couponModel.CouponType == 1)
                                {
                                    //当优惠券为商品优惠券时
                                    var couponCommodityAccount =
                                        orderItemList.Where(
                                            t => couponModel.CommodityIds.Contains(t.CommodityId.ToString()))
                                                     .Sum(t => t.RealPrice.Value * t.Number);

                                    if (couponCommodityAccount > 0)
                                    {
                                        foreach (var orderItem in orderItemList)
                                        {
                                            if (yjbInfo != null && yjbInfo.Items != null)
                                            {
                                                var currentCommodityYjbInfo = yjbInfo.Items.FirstOrDefault(c => c.CommodityId == orderItem.CommodityId);
                                                if (currentCommodityYjbInfo != null && currentCommodityYjbInfo.InsteadCashAmount > 0)
                                                {
                                                    orderItem.YjbPrice = currentCommodityYjbInfo.InsteadCashAmount;
                                                }
                                            }

                                            decimal orderItemAmount = 0;
                                            if (orderItem.DiscountPrice.Value > 0)
                                            {
                                                orderItemAmount = orderItem.DiscountPrice.Value * orderItem.Number;
                                            }
                                            else
                                            {
                                                orderItemAmount = orderItem.RealPrice.Value * orderItem.Number;
                                            }

                                            if (orderItem.YjbPrice != null)
                                            {
                                                orderItemAmount = orderItemAmount - (decimal)orderItem.YjbPrice;
                                            }
                                            //if (orderItem.CouponPrice != null)
                                            //{
                                            //    orderItemAmount = orderItemAmount - (decimal)orderItem.CouponPrice;
                                            //}
                                            decimal moneyTmp = 0m;
                                            decimal sharePercent = globalAmount;
                                            if (sharePercent <= 0)
                                            {
                                                var sharePercentTmp =
                                                     commodityShareList.Where(t => t.Id == orderItem.CommodityId)
                                                                       .Select(t => t.SharePercent)
                                                                       .FirstOrDefault();
                                                sharePercent = sharePercentTmp.HasValue ? sharePercentTmp.Value : 0m;
                                            }
                                            if (couponModel.CommodityIds.Contains(orderItem.CommodityId.ToString()))
                                            {
                                                moneyTmp = (orderItemAmount -
                                                            (orderItemAmount /
                                                             couponCommodityAccount) *
                                                            couponModel.Amount -
                                                            (orderItemAmount / commodityOrder.Price) *
                                                            integrationAmount) * sharePercent;
                                            }
                                            else
                                            {
                                                moneyTmp = (orderItemAmount -
                                                            (orderItemAmount / commodityOrder.Price) *
                                                            integrationAmount) * sharePercent;
                                            }

                                            OrderItemShare oiShare = OrderItemShare.CreateOrderItemShare();
                                            oiShare.Id = Guid.NewGuid();
                                            oiShare.SubTime = DateTime.Now;
                                            oiShare.ModifiedOn = DateTime.Now;
                                            oiShare.SubId = userId;
                                            oiShare.SharePrice = orderItemAmount;
                                            oiShare.Commission = getCommisionWithCost(moneyTmp, scoreCost);
                                            oiShare.ShouldCommission = moneyTmp.ToMoney();
                                            oiShare.SharePercent = sharePercent;
                                            oiShare.OrderId = orderItem.CommodityOrderId;
                                            oiShare.OrderItemId = orderItem.Id;
                                            oiShare.PayeeType = 3;
                                            oiShare.PayeeId = shareUseId;
                                            oiShare.ShareKey = shareId;
                                            oiShare.EntityState = EntityState.Added;
                                            contextSession.SaveObject(oiShare);

                                            commission += oiShare.Commission;
                                            shouldCommission += oiShare.ShouldCommission;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //没有使用优惠券
                                foreach (var orderItem in orderItemList)
                                {
                                    if (yjbInfo != null && yjbInfo.Items != null)
                                    {
                                        var currentCommodityYjbInfo = yjbInfo.Items.FirstOrDefault(c => c.CommodityId == orderItem.CommodityId);
                                        if (currentCommodityYjbInfo != null && currentCommodityYjbInfo.InsteadCashAmount > 0)
                                        {
                                            orderItem.YjbPrice = currentCommodityYjbInfo.InsteadCashAmount;
                                        }
                                    }

                                    decimal orderItemAmount = 0;
                                    if (orderItem.DiscountPrice.Value > 0)
                                    {
                                        orderItemAmount = orderItem.DiscountPrice.Value * orderItem.Number;
                                    }
                                    else
                                    {
                                        orderItemAmount = orderItem.RealPrice.Value * orderItem.Number;
                                    }

                                    if (orderItem.YjbPrice != null)
                                    {
                                        orderItemAmount = orderItemAmount - (decimal)orderItem.YjbPrice;
                                    }
                                    if (orderItem.CouponPrice != null)
                                    {
                                        orderItemAmount = orderItemAmount - (decimal)orderItem.CouponPrice;
                                    }
                                    decimal sharePercent = globalAmount;
                                    if (sharePercent <= 0)
                                    {
                                        var sharePercentTmp =
                                             commodityShareList.Where(t => t.Id == orderItem.CommodityId)
                                                               .Select(t => t.SharePercent)
                                                               .FirstOrDefault();
                                        sharePercent = sharePercentTmp.HasValue ? sharePercentTmp.Value : 0m;
                                    }
                                    decimal moneyTmp = (orderItemAmount -
                                                        (orderItemAmount / commodityOrder.Price) *
                                                        integrationAmount) * sharePercent;

                                    OrderItemShare oiShare = OrderItemShare.CreateOrderItemShare();
                                    oiShare.Id = Guid.NewGuid();
                                    oiShare.SubTime = DateTime.Now;
                                    oiShare.ModifiedOn = DateTime.Now;
                                    oiShare.SubId = userId;
                                    oiShare.SharePrice = orderItemAmount;
                                    oiShare.Commission = getCommisionWithCost(moneyTmp, scoreCost);
                                    oiShare.ShouldCommission = moneyTmp.ToMoney();
                                    oiShare.SharePercent = sharePercent;
                                    oiShare.OrderId = orderItem.CommodityOrderId;
                                    oiShare.OrderItemId = orderItem.Id;
                                    oiShare.PayeeType = 3;
                                    oiShare.PayeeId = shareUseId;
                                    oiShare.ShareKey = shareId;
                                    oiShare.EntityState = EntityState.Added;
                                    contextSession.SaveObject(oiShare);

                                    commission += oiShare.Commission;
                                    shouldCommission += oiShare.ShouldCommission;
                                }
                            }
                        }
                        else
                        {
                            foreach (var orderItem in orderItemList)
                            {
                                if (yjbInfo != null && yjbInfo.Items != null)
                                {
                                    var currentCommodityYjbInfo = yjbInfo.Items.FirstOrDefault(c => c.CommodityId == orderItem.CommodityId);
                                    if (currentCommodityYjbInfo != null && currentCommodityYjbInfo.InsteadCashAmount > 0)
                                    {
                                        orderItem.YjbPrice = currentCommodityYjbInfo.InsteadCashAmount;
                                    }
                                }

                                decimal orderItemAmount = 0;
                                if (orderItem.DiscountPrice.Value > 0)
                                {
                                    orderItemAmount = orderItem.DiscountPrice.Value * orderItem.Number;
                                }
                                else
                                {
                                    orderItemAmount = orderItem.RealPrice.Value * orderItem.Number;
                                }

                                if (orderItem.YjbPrice != null)
                                {
                                    orderItemAmount = orderItemAmount - (decimal)orderItem.YjbPrice;
                                }
                                if (orderItem.CouponPrice != null)
                                {
                                    orderItemAmount = orderItemAmount - (decimal)orderItem.CouponPrice;
                                }
                                decimal sharePercent = globalAmount;
                                if (sharePercent <= 0)
                                {
                                    var sharePercentTmp =
                                         commodityShareList.Where(t => t.Id == orderItem.CommodityId)
                                                           .Select(t => t.SharePercent)
                                                           .FirstOrDefault();
                                    sharePercent = sharePercentTmp.HasValue ? sharePercentTmp.Value : 0m;
                                }
                                decimal moneyTmp = orderItemAmount * sharePercent;

                                OrderItemShare oiShare = OrderItemShare.CreateOrderItemShare();
                                oiShare.Id = Guid.NewGuid();
                                oiShare.SubTime = DateTime.Now;
                                oiShare.ModifiedOn = DateTime.Now;
                                oiShare.SubId = userId;
                                oiShare.SharePrice = orderItemAmount;
                                oiShare.Commission = getCommisionWithCost(moneyTmp, scoreCost);
                                oiShare.ShouldCommission = moneyTmp.ToMoney();
                                oiShare.SharePercent = sharePercent;
                                oiShare.OrderId = orderItem.CommodityOrderId;
                                oiShare.OrderItemId = orderItem.Id;
                                oiShare.PayeeType = 3;
                                oiShare.PayeeId = shareUseId;
                                oiShare.ShareKey = shareId;
                                oiShare.EntityState = EntityState.Added;
                                contextSession.SaveObject(oiShare);

                                commission += oiShare.Commission;
                                shouldCommission += oiShare.ShouldCommission;
                            }
                        }

                        //众销分享佣金
                        commodityOrder.Commission = commission;

                        //保存众销分成到 OrderShare 表
                        if (shareUseId != Guid.Empty)
                        {
                            decimal sharePriceTotal = orderitemlist.Sum(t => t.RealPrice.Value * t.Number);

                            OrderShare os = OrderShare.CreateOrderShare();
                            os.Id = Guid.NewGuid();
                            os.SubTime = DateTime.Now;
                            os.ModifiedOn = DateTime.Now;
                            os.SubId = userId;
                            os.SharePrice = sharePriceTotal;
                            os.Commission = commission;
                            os.ShouldCommission = shouldCommission;
                            os.SharePercent = 0;
                            os.OrderId = commodityOrder.Id;
                            os.PayeeType = 3;
                            os.PayeeId = shareUseId;
                            os.ShareKey = shareId;

                            os.EntityState = EntityState.Added;
                            contextSession.SaveObject(os);
                        }

                    }
                    #endregion
                }

                // 添加结算信息
                // 直接到账
                if (commodityOrder.Payment != 0)
                {
                    SettleAccountHelper.CreateSettleAccountDetails(contextSession, commodityOrder);
                }

                contextSession.SaveChanges();

                OrderEventHelper.OnOrderPaySuccess(commodityOrder);


                LogHelper.Info(string.Format("数据保存PayUpdateCommodityOrderExt,orderId:{0}", orderId));

                //刷新缓存商品缓存
                if (needRefreshCacheCommoditys.Any())
                {
                    needRefreshCacheCommoditys.ForEach(c => c.RefreshCache(EntityState.Modified));
                }
                //刷新优惠信息缓存
                if (needRefreshCacheTodayPromotions.Any())
                {
                    needRefreshCacheTodayPromotions.ForEach(c => c.RefreshCache(EntityState.Modified));
                }

                LogHelper.Info(string.Format("刷新缓存PayUpdateCommodityOrderExt,orderId:{0}", orderId));

                #region 好运来订单支付后调用好运来接口
                if (commodityOrder.SrcType == 1 && commodityOrder.SrcTagId != null && commodityOrder.SrcTagId != Guid.Empty)//好运来
                {
                    var lotteryResult = GameSV.Instance.UpdateWinPlayerBuyed(commodityOrder.SrcTagId.Value, commodityOrder.UserId);

                }
                #endregion

                Guid EsAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
                addmassage.AddMessages(odid, usid, EsAppId, commodityOrder.Code, 1, "", type);
                ////正品会发送消息
                //if (new ZPHSV().CheckIsAppInZPH(appId))
                //{
                //    addmassage.AddMessages(odid, usid, CustomConfig.ZPHAppId, commodityOrder.Code, 1, "", type);
                //}

                LogHelper.Info(string.Format("消息发送PayUpdateCommodityOrderExt,orderId:{0}", orderId));

                return new ResultDTO { ResultCode = 0, Message = "Success" };

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("订单状态修改服务异常。orderId{0}，userId{1}，appId{2}，payment{3}，gold{4}，money{5}，couponCount{6}", orderId, userId, appId, payment, gold, money, couponCount), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(orderId);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="commodityOrder">订单</param>
        /// <param name="orderItemList">订单项</param>
        /// <param name="userId"></param>
        private bool CalcOrderSpread(CommodityOrder commodityOrder, List<OrderItem> orderItemList, Guid userId, ContextSession contextSession, List<Commodity> coms)
        {
            if (!ZPHSV.Instance.CheckIsAppInZPH(commodityOrder.AppId))
                return false;
            if (!commodityOrder.SpreaderId.HasValue)
                return false;
            decimal globalPercent = 0.0m;
            //众销推广实发金额
            decimal commission = 0.0m;

            //众销推广应发金额
            var orderPayDetailsList = OrderPayDetail.ObjectSet().Where(c => c.OrderId == commodityOrder.Id).ToList();
            var appext = AppExtension.ObjectSet().FirstOrDefault(c => c.Id == commodityOrder.AppId);

            //商品设置列表 
            List<Commodity> commodityShareList = new List<Commodity>();
            if (appext != null && appext.IsDividendAll == true && appext.SharePercent > 0m)
            {
                globalPercent = appext.SharePercent;
            }
            else
            {
                var comIdList = orderItemList.Select(c => c.CommodityId).Distinct().ToList();
                commodityShareList = coms.Where(c => comIdList.Contains(c.Id) && c.SharePercent > 0).ToList();
            }
            decimal shouldCommission = 0.0m;
            if (orderPayDetailsList.Any())
            {
                var couponModel = orderPayDetailsList.FirstOrDefault(t => t.ObjectType == 1);
                var integrationAmount =
                    orderPayDetailsList.Where(t => t.ObjectType == 2).Select(t => t.Amount).FirstOrDefault();
                if (couponModel != null)
                {
                    //使用了优惠券
                    if (couponModel.CouponType == 0)
                    {
                        //当优惠券为店铺优惠券时
                        foreach (var orderItem in orderItemList)
                        {
                            decimal orderItemAmount = orderItem.RealPrice.Value * orderItem.Number;
                            decimal sharePercent = globalPercent;
                            if (sharePercent <= 0)
                            {
                                var sharePercentTmp =
                                     commodityShareList.Where(t => t.Id == orderItem.CommodityId)
                                                       .Select(c => c.SharePercent)
                                                       .FirstOrDefault();
                                sharePercent = sharePercentTmp.HasValue ? sharePercentTmp.Value : 0m;
                            }
                            decimal money = (orderItemAmount -
                                                (orderItemAmount / commodityOrder.Price) *
                                                couponModel.Amount -
                                                (orderItemAmount / commodityOrder.Price) *
                                                integrationAmount) * sharePercent;
                            money = money.ToMoney();
                            OrderItemShare oiShare = OrderItemShare.CreateOrderItemShare();
                            oiShare.Id = Guid.NewGuid();
                            oiShare.SubTime = DateTime.Now;
                            oiShare.ModifiedOn = DateTime.Now;
                            oiShare.SubId = userId;
                            oiShare.SharePrice = orderItemAmount;
                            oiShare.Commission = money;
                            oiShare.ShouldCommission = money;
                            oiShare.SharePercent = sharePercent;
                            oiShare.OrderId = orderItem.CommodityOrderId;
                            oiShare.OrderItemId = orderItem.Id;
                            oiShare.PayeeType = 5;
                            oiShare.PayeeId = commodityOrder.SpreaderId.Value;
                            oiShare.ShareKey = commodityOrder.SpreadCode == null ? string.Empty : commodityOrder.SpreadCode.ToString();
                            oiShare.EntityState = EntityState.Added;
                            contextSession.SaveObject(oiShare);

                            commission += oiShare.Commission;
                            shouldCommission += oiShare.ShouldCommission;
                        }
                    }
                    else if (couponModel.CouponType == 1)
                    {
                        //当优惠券为商品优惠券时
                        var couponCommodityAccount =
                            orderItemList.Where(
                                t => couponModel.CommodityIds.Contains(t.CommodityId.ToString()))
                                         .Sum(t => t.RealPrice.Value * t.Number);

                        if (couponCommodityAccount > 0)
                        {
                            foreach (var orderItem in orderItemList)
                            {
                                decimal orderItemAmount = orderItem.RealPrice.Value * orderItem.Number;
                                decimal moneyTmp = 0m;
                                decimal sharePercent = globalPercent;
                                if (sharePercent <= 0)
                                {
                                    var sharePercentTmp =
                                         commodityShareList.Where(t => t.Id == orderItem.CommodityId)
                                                           .Select(t => t.SharePercent)
                                                           .FirstOrDefault();
                                    sharePercent = sharePercentTmp.HasValue ? sharePercentTmp.Value : 0m;
                                }
                                if (couponModel.CommodityIds.Contains(orderItem.CommodityId.ToString()))
                                {
                                    moneyTmp = (orderItemAmount -
                                                (orderItemAmount /
                                                 couponCommodityAccount) *
                                                couponModel.Amount -
                                                (orderItemAmount / commodityOrder.Price) *
                                                integrationAmount) * sharePercent;
                                }
                                else
                                {
                                    moneyTmp = (orderItemAmount -
                                                (orderItemAmount / commodityOrder.Price) *
                                                integrationAmount) * sharePercent;
                                }
                                moneyTmp = moneyTmp.ToMoney();
                                OrderItemShare oiShare = OrderItemShare.CreateOrderItemShare();
                                oiShare.Id = Guid.NewGuid();
                                oiShare.SubTime = DateTime.Now;
                                oiShare.ModifiedOn = DateTime.Now;
                                oiShare.SubId = userId;
                                oiShare.SharePrice = orderItemAmount;
                                oiShare.Commission = moneyTmp;
                                oiShare.ShouldCommission = moneyTmp;
                                oiShare.SharePercent = sharePercent;
                                oiShare.OrderId = orderItem.CommodityOrderId;
                                oiShare.OrderItemId = orderItem.Id;
                                oiShare.PayeeType = 5;
                                oiShare.PayeeId = commodityOrder.SpreaderId.Value;
                                oiShare.ShareKey = commodityOrder.SpreadCode == null ? string.Empty : commodityOrder.SpreadCode.ToString();
                                oiShare.EntityState = EntityState.Added;
                                contextSession.SaveObject(oiShare);

                                commission += oiShare.Commission;
                                shouldCommission += oiShare.ShouldCommission;
                            }
                        }
                    }
                }
                else
                {
                    //没有使用优惠券
                    foreach (var orderItem in orderItemList)
                    {
                        decimal orderItemAmount = orderItem.RealPrice.Value * orderItem.Number;
                        decimal sharePercent = globalPercent;
                        if (sharePercent <= 0)
                        {
                            var sharePercentTmp =
                                 commodityShareList.Where(t => t.Id == orderItem.CommodityId)
                                                   .Select(t => t.SharePercent)
                                                   .FirstOrDefault();
                            sharePercent = sharePercentTmp.HasValue ? sharePercentTmp.Value : 0m;
                        }
                        decimal moneyTmp = (orderItemAmount -
                                            (orderItemAmount / commodityOrder.Price) *
                                            integrationAmount) * sharePercent;
                        moneyTmp = moneyTmp.ToMoney();
                        OrderItemShare oiShare = OrderItemShare.CreateOrderItemShare();
                        oiShare.Id = Guid.NewGuid();
                        oiShare.SubTime = DateTime.Now;
                        oiShare.ModifiedOn = DateTime.Now;
                        oiShare.SubId = userId;
                        oiShare.SharePrice = orderItemAmount;
                        oiShare.Commission = moneyTmp;
                        oiShare.ShouldCommission = moneyTmp;
                        oiShare.SharePercent = sharePercent;
                        oiShare.OrderId = orderItem.CommodityOrderId;
                        oiShare.OrderItemId = orderItem.Id;
                        oiShare.PayeeType = 5;
                        oiShare.PayeeId = commodityOrder.SpreaderId.Value;
                        oiShare.ShareKey = commodityOrder.SpreadCode == null ? string.Empty : commodityOrder.SpreadCode.ToString();
                        oiShare.EntityState = EntityState.Added;
                        contextSession.SaveObject(oiShare);

                        commission += oiShare.Commission;
                        shouldCommission += oiShare.ShouldCommission;
                    }
                }
            }
            else
            {
                foreach (var orderItem in orderItemList)
                {
                    decimal orderItemAmount = orderItem.RealPrice.Value * orderItem.Number;
                    decimal sharePercent = globalPercent;
                    if (sharePercent <= 0)
                    {
                        var sharePercentTmp =
                             commodityShareList.Where(t => t.Id == orderItem.CommodityId)
                                               .Select(t => t.SharePercent)
                                               .FirstOrDefault();
                        sharePercent = sharePercentTmp.HasValue ? sharePercentTmp.Value : 0m;
                    }
                    decimal moneyTmp = (orderItemAmount * sharePercent).ToMoney();
                    OrderItemShare oiShare = OrderItemShare.CreateOrderItemShare();
                    oiShare.Id = Guid.NewGuid();
                    oiShare.SubTime = DateTime.Now;
                    oiShare.ModifiedOn = DateTime.Now;
                    oiShare.SubId = userId;
                    oiShare.SharePrice = orderItemAmount;
                    oiShare.Commission = moneyTmp;
                    oiShare.ShouldCommission = moneyTmp;
                    oiShare.SharePercent = sharePercent;
                    oiShare.OrderId = orderItem.CommodityOrderId;
                    oiShare.OrderItemId = orderItem.Id;
                    oiShare.PayeeType = 5;
                    oiShare.PayeeId = commodityOrder.SpreaderId.Value;
                    oiShare.ShareKey = commodityOrder.SpreadCode == null ? string.Empty : commodityOrder.SpreadCode.ToString();
                    oiShare.EntityState = EntityState.Added;
                    contextSession.SaveObject(oiShare);

                    commission += oiShare.Commission;
                    shouldCommission += oiShare.ShouldCommission;
                }
            }

            //众销分享佣金
            commodityOrder.SpreadGold = commission.ToGold();

            //保存众销分成到 OrderShare 表
            if (commodityOrder.SpreadGold > 0)
            {
                decimal sharePriceTotal = orderItemList.Sum(t => t.RealPrice.Value * t.Number);

                OrderShare os = OrderShare.CreateOrderShare();
                os.Id = Guid.NewGuid();
                os.SubTime = DateTime.Now;
                os.ModifiedOn = DateTime.Now;
                os.SubId = userId;
                os.SharePrice = sharePriceTotal;
                os.Commission = commission;
                os.ShouldCommission = shouldCommission;
                os.SharePercent = 0;
                os.OrderId = commodityOrder.Id;
                os.PayeeType = 5;
                os.PayeeId = commodityOrder.SpreaderId.Value;
                os.ShareKey = commodityOrder.SpreadCode == null ? string.Empty : commodityOrder.SpreadCode.ToString();

                os.EntityState = EntityState.Added;
                contextSession.SaveObject(os);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 获取应得佣金（考虑积分汇率）
        /// </summary>
        /// <param name="money"></param>
        /// <param name="cost">积分汇率，只可能是10的幂（1，10，100....）</param>
        /// <returns></returns>
        private decimal getCommisionWithCost(decimal money, int cost)
        {
            //积分汇率太小，金额会有除不尽的情况，如：积分汇率1：1，分成应分成金额0.45，则实发佣金为0
            if (!CustomConfig.IsShareAsScore)
                return money.ToMoney();

            if (cost == 1)
            {
                return money.ToMoney(0);
            }
            else if (cost == 10)
            {
                return money.ToMoney(1);
            }
            return money.ToMoney();
        }

        /// <summary>
        /// 计算订单三级分销
        /// </summary>
        /// <param name="commodityOrder">订单信息</param>
        /// <param name="orderitemlist">订单项列表</param>
        /// <param name="cAppId">订单中商品所在店铺id</param>
        /// <param name="userId">当前用户id</param>
        /// <param name="scoreCost">积分汇率</param>
        /// <param name="isDistribut">订单是否参加了三级分销</param>
        private void CalcOrderDistribut(CommodityOrder commodityOrder, List<OrderItem> orderitemlist, Guid cAppId, Guid userId, int scoreCost, out bool isDistribut)
        {
            isDistribut = false;


            //计算订单、订单项三级分销金额
            if (commodityOrder.DistributorId == null || commodityOrder.DistributorId == Guid.Empty)
            {
                return;
            }
            ////若A供应商嵌入了B供应商的商品，通过A供应商的APP或者链接购买了B供应商的分销商品，此时B供应商的商品不参与三级分销。
            //if (commodityOrder.EsAppId != cAppId)
            //{
            //    return;
            //}

            //商品三级分销设置。
            var commodityIdList = orderitemlist.Select(a => a.CommodityId).Distinct().ToList();
            var cdList = (from cd in CommodityDistribution.ObjectSet()
                          where commodityIdList.Contains(cd.Id)
                          select cd).ToList();

            //订单中商品都没有参加三级分销，不用分销佣金。
            if (!cdList.Any())
            {
                return;
            }

            //三级分销功能是否生效。
            if (!commodityOrder.EsAppId.HasValue)
            {
                return;
            }
            bool b = BACBP.CheckAppDistribute(commodityOrder.EsAppId.Value);
            if (!b)
            {
                return;
            }
            isDistribut = true;

            ContextSession contextSession = ContextFactory.CurrentThreadContext;

            var distrib = (from distr in Distributor.ObjectSet()
                           where distr.Id == commodityOrder.DistributorId
                           select new { distr.Key, distr.EsAppId }).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(distrib.Key))
            {
                return;
            }
            //下单用户的上三级。
            List<string> didQuery = distrib.Key.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Reverse().Take(3).ToList();
            if (!didQuery.Any())
            {
                return;
            }
            List<Guid> didList = didQuery.ConvertAll<Guid>(d => new Guid(d));
            if (!didList.Any())
            {
                return;
            }

            //分销商关系错乱（A电商馆的分销商分销了B馆的商品）
            if (commodityOrder.EsAppId != distrib.EsAppId)
            {
                return;
            }

            var higherLevelDistributor = (from distr in Distributor.ObjectSet()
                                          where didList.Contains(distr.Id)
                                          orderby distr.Level descending
                                          select new { Id = distr.Id, UserId = distr.UserId }).ToList();

            decimal? couponRPTotal = 0;
            decimal cAmount = 0;
            decimal sAmount = 0;
            //订单支付详情列表。
            var orderPayDetailsList = OrderPayDetail.ObjectSet().Where(c => c.OrderId == commodityOrder.Id).ToList();
            var couponModel = orderPayDetailsList.Where(t => t.ObjectType == 1).FirstOrDefault();
            var integrationAmount = orderPayDetailsList.Where(t => t.ObjectType == 2).Select(t => t.Amount).FirstOrDefault();

            //订单实际支付总价
            decimal? priceSum = orderitemlist.Sum(oi => oi.RealPrice);

            //三级分销在app中的全局设置。
            var appDistribution = (from ae in AppExtension.ObjectSet()
                                   where ae.Id == commodityOrder.AppId
                                   select new { ae.DistributeL1Percent, ae.DistributeL2Percent, ae.DistributeL3Percent }).FirstOrDefault();

            List<OrderItemShare> oisList = new List<OrderItemShare>();

            foreach (OrderItem oi in orderitemlist)
            {
                decimal orderItemAmount = oi.RealPrice.Value * oi.Number;
                decimal? l1p = 0;
                decimal? l2p = 0;
                decimal? l3p = 0;
                var cdc = (from cd in cdList
                           where cd.Id == oi.CommodityId
                           select cd).FirstOrDefault();
                if (cdc == null)
                {
                    continue;
                }
                if (cdc.L1Percent == null && cdc.L2Percent == null && cdc.L3Percent == null)
                {
                    if (appDistribution == null)
                    {
                        continue;
                    }
                    if (appDistribution.DistributeL1Percent == null &&
                        appDistribution.DistributeL2Percent == null &&
                        appDistribution.DistributeL3Percent == null)
                    {
                        continue;
                    }
                    l1p = appDistribution.DistributeL1Percent;
                    l2p = appDistribution.DistributeL2Percent;
                    l3p = appDistribution.DistributeL3Percent;
                }
                else
                {
                    l1p = cdc.L1Percent;
                    l2p = cdc.L2Percent;
                    l3p = cdc.L3Percent;
                }

                //分成类型（同收款人角色） 参考OrderItemShare.PayeeType注释。
                int payeeType = 9;
                foreach (var hld in higherLevelDistributor)
                {
                    decimal? sharePercent = 0;
                    if (payeeType == 9)
                    {
                        sharePercent = l1p;
                    }
                    else if (payeeType == 10)
                    {
                        sharePercent = l2p;
                    }
                    else if (payeeType == 11)
                    {
                        sharePercent = l3p;
                    }
                    if (!sharePercent.HasValue || sharePercent < 0)
                    {
                        sharePercent = 0;
                    }

                    decimal sc = 0;

                    if (couponModel != null)
                    {
                        //使用了优惠券
                        if (couponModel.CouponType == 0)
                        {
                            sc = (orderItemAmount -
                    (orderItemAmount / commodityOrder.Price) *
                    couponModel.Amount -
                    (orderItemAmount / commodityOrder.Price) *
                    integrationAmount) * sharePercent.Value;

                        }
                        else if (couponModel.CouponType == 1)
                        {
                            //当优惠券为商品优惠券时
                            var couponCommodityAccount =
                                orderitemlist.Where(
                                    t => couponModel.CommodityIds.Contains(t.CommodityId.ToString()))
                                             .Sum(t => t.RealPrice.Value * t.Number);
                            if (couponCommodityAccount > 0)
                            {
                                if (couponModel.CommodityIds.Contains(oi.CommodityId.ToString()))
                                {
                                    sc = (orderItemAmount -
                                                (orderItemAmount /
                                                 couponCommodityAccount) *
                                                couponModel.Amount -
                                                (orderItemAmount / commodityOrder.Price) *
                                                integrationAmount) * sharePercent.Value;
                                }
                                else
                                {
                                    sc = (orderItemAmount -
                                                (orderItemAmount / commodityOrder.Price) *
                                                integrationAmount) * sharePercent.Value;
                                }
                            }
                        }
                        else
                        {
                            //
                        }
                    }
                    else
                    {
                        sc = (orderItemAmount -
                    (orderItemAmount / commodityOrder.Price) *
                    integrationAmount) * sharePercent.Value;
                    }

                    OrderItemShare oiShare = OrderItemShare.CreateOrderItemShare();
                    oiShare.Id = Guid.NewGuid();
                    oiShare.SubTime = DateTime.Now;
                    oiShare.ModifiedOn = DateTime.Now;
                    oiShare.SubId = userId;
                    oiShare.SharePrice = orderItemAmount;
                    oiShare.Commission = getCommisionWithCost(sc, scoreCost);
                    oiShare.ShouldCommission = sc.ToMoney();
                    oiShare.SharePercent = sharePercent.HasValue ? sharePercent.Value : 0;
                    oiShare.OrderId = oi.CommodityOrderId;
                    oiShare.OrderItemId = oi.Id;
                    oiShare.PayeeType = payeeType;
                    oiShare.PayeeId = hld.UserId;
                    oiShare.ShareKey = hld.Id.ToString();

                    oisList.Add(oiShare);
                    contextSession.SaveObject(oiShare);

                    payeeType++;
                }
            }

            if (!oisList.Any())
            {
                return;
            }

            int payeeType1 = 9;
            foreach (var hld in higherLevelDistributor)
            {
                decimal sharePriceTotal = oisList.Where(ois => ois.PayeeId == hld.UserId).Select(ois => ois.SharePrice).Sum();
                decimal commissionTotal = oisList.Where(ois => ois.PayeeId == hld.UserId).Select(ois => ois.Commission).Sum();
                decimal shouldCommissionTotal = oisList.Where(ois => ois.PayeeId == hld.UserId).Select(ois => ois.ShouldCommission).Sum();

                OrderShare os = OrderShare.CreateOrderShare();
                os.Id = Guid.NewGuid();
                os.SubTime = DateTime.Now;
                os.ModifiedOn = DateTime.Now;
                os.SubId = userId;
                os.SharePrice = sharePriceTotal;
                os.Commission = commissionTotal;
                os.ShouldCommission = shouldCommissionTotal;
                os.SharePercent = 0;
                os.OrderId = commodityOrder.Id;
                os.PayeeType = payeeType1;
                os.PayeeId = hld.UserId;
                os.ShareKey = hld.Id.ToString();

                contextSession.SaveObject(os);

                payeeType1++;
            }


            //订单项记录明细，订单记录总分成金额，但不用记录是谁分成
            //分销佣金
            commodityOrder.DistributeMoney = oisList.Select(ois => ois.Commission).Sum();

        }

        /// <summary>
        /// 发消息 （用户付款后消息；确认收货后发消息）
        /// </summary>
        /// <param name="userids">接受人userid集合</param>
        /// <param name="type">type： Payment 用户付款后消息； affirm 确认收货后发消息</param>
        /// <param name="number">数量</param>
        public static void SendMessageToPayment(List<Guid> userids, string type, string number, ContextDTO contextDTO, decimal percent)
        {
            try
            {
                if (userids == null || userids.Count == 0)
                {
                    LogHelper.Info("SendMessageToPayment发送消息人为空");
                }
                string messages = string.Empty;
                if (type == "Payment")
                {
                    if (percent <= decimal.Zero)
                        return;
                    messages = string.Format("您的好友通过您的分享购买{0}元商品，您将获得{1:P2}的众销红包（最终所得以实际发放时为准），待用户确认收货后就能领取了，记得持续关注哦~", number, percent);
                }
                else if (type == "affirm")
                {
                    messages = string.Format("您通过分享商品获得的{0}个金币的众销红包将于明天早上八点发放，记得来领取哦，过期作废哦~", number);
                }

                List<string> tempUserIds = new List<string>();
                if (userids.Count() > 0)
                {
                    foreach (Guid gd in userids)
                    {
                        tempUserIds.Add(gd.ToString());
                    }
                }
                LogHelper.Info(string.Format("SendMessageToPayment发送消息：{0}，消息人：{1} ", tempUserIds[0], messages));

                Jinher.AMP.SNS.Deploy.CustomDTO.ReturnInfoDTO<bool> _message = Jinher.AMP.BTP.TPS.SNSSV.Instance.PushSysMessageToUsers(tempUserIds, messages, "btp");
                LogHelper.Info("SendMessageToPayment发送消息状态 ： " + (_message.Code == "0" ? "成功" : "失败"));
                LogHelper.Info("SendMessageToPayment发送消息Content ： " + (_message.Content.ToString()));
            }
            catch (Exception ex)
            {
                LogHelper.Error("SendMessageToPayment发送消息异常。", ex);
            }
        }

        private static void SendMessageToAppOwner(int state, System.Guid appId, CommodityOrder commodityOrder, Jinher.AMP.App.Deploy.ApplicationDTO applicationDTO, ContextDTO contextDTO)
        {
            AddMessage addmassage = new AddMessage();
            if (commodityOrder != null && applicationDTO != null && applicationDTO.OwnerId != null && applicationDTO.OwnerId.Value != Guid.Empty)
            {
                try
                {
                    string appName = APPSV.GetAppName(commodityOrder.AppId);
                    string messages = string.Format("{0}有客户下单并支付啦，快快发货吧~订单编号：{1}", "【" + appName + "】", commodityOrder.Code);

                    List<Guid> lGuid = EBCSV.GetOrderMenuUsers(applicationDTO);
                    if (lGuid == null || !lGuid.Any())
                        return;
                    System.Text.StringBuilder strOrgUserIds = new System.Text.StringBuilder();
                    foreach (Guid orgUserId in lGuid)
                    {
                        strOrgUserIds.Append(orgUserId).Append(",");
                    }
                    strOrgUserIds.Remove(strOrgUserIds.Length - 1, 1);
                    addmassage.AddMessages(commodityOrder.Id.ToString(), strOrgUserIds.ToString(), appId, commodityOrder.Code, state, messages, "orderAppOwner");

                    //后台发消息 
                    //推送消息
                    Jinher.AMP.Info.Deploy.CustomDTO.MessageForAddDTO messageAdd = new Info.Deploy.CustomDTO.MessageForAddDTO
                    {
                        PublishTime = DateTime.Now,
                        ReceiverUserId = lGuid,
                        SenderType = Info.Deploy.Enum.SenderType.System,
                        Title = messages,
                        Content = messages,
                        ReceiverRange = Info.Deploy.Enum.ReceiverRange.SpecifiedUser
                    };

                    var retret = Jinher.AMP.BTP.TPS.InfoSV.Instance.AddSystemMessage(messageAdd);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(string.Format("推送消息异常,state:{0}, appId:{1},orderId:{2}", state, appId, commodityOrder.Id), ex);
                }

            }
        }

        /// <summary>
        /// 向自提点管理员发消息
        /// </summary>
        /// <param name="state"></param>
        /// <param name="commodityOrder"></param>
        private static void SendMessageToSelfTakeManagers(int state, CommodityOrder commodityOrder)
        {
            AddMessage addmassage = new AddMessage();
            if (commodityOrder == null || commodityOrder.SelfTakeFlag != 1)
                return;
            var managerIdList = (from orderPickUp in AppOrderPickUp.ObjectSet()
                                 join manager in AppStsManager.ObjectSet() on orderPickUp.SelfTakeStationId
                                     equals manager.SelfTakeStationId
                                 where orderPickUp.Id == commodityOrder.Id
                                 select manager.UserId).Distinct().ToList();
            if (!managerIdList.Any())
                return;
            string _appName = APPSV.GetAppName(commodityOrder.AppId);
            string messages = string.Format("{0}有客户下单并支付啦，快快发货吧~订单编号：{1}", "【" + _appName + "】", commodityOrder.Code);
            addmassage.AddMessages(commodityOrder.Id.ToString(), string.Join(",", managerIdList), CustomConfig.SelfTakeAppId, commodityOrder.Code, state, messages, "selfTakeManager");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="title">后台接收消息的标题</param>
        /// <param name="content">后台接收消息的内容</param>
        /// <param name="mobilemess">手机接收的消息</param>
        /// <param name="state">1.只给手机发送2.只给后台发送3.同时给手机和后台发消息</param>
        /// <param name="appId"></param>
        /// <param name="commodityOrder"></param>
        private static void SendMessageCommon(string title, string content, string mobilemess, int state, System.Guid appId, CommodityOrder commodityOrder, ContextDTO contextDTO)
        {
            if (commodityOrder == null)
            {
                LogHelper.Error("SendMessageCommon中传入参数commodityOrder不能为空");
            }
            Jinher.AMP.App.Deploy.ApplicationDTO applicationDTO = null;
            try
            {
                Jinher.AMP.App.IBP.Facade.AppManagerFacade appManagerFacade = new App.IBP.Facade.AppManagerFacade();
                if (contextDTO != null)
                {
                    appManagerFacade.ContextDTO = contextDTO;
                }
                applicationDTO = appManagerFacade.GetAppById(appId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SendMessageCommon AppManagerSV服务异常:获取应用信息异常:。title：{0}，content：{1}，mobilemess：{2}，state：{3}，appId：{4}，Id：{5}，code：{6}", title, content, mobilemess, state, appId, commodityOrder.Id, commodityOrder.Code), ex);
            }

            try
            {
                AddMessage addmassage = new AddMessage();
                if (applicationDTO != null && applicationDTO.OwnerId != null && applicationDTO.OwnerId.Value != Guid.Empty && commodityOrder != null)
                {
                    string messages = mobilemess;
                    List<Guid> LGuid = EBCSV.GetOrderMenuUsers(applicationDTO);
                    if (LGuid == null || !LGuid.Any())
                        return;
                    System.Text.StringBuilder strOrgUserIds = new System.Text.StringBuilder();
                    foreach (Guid orgUserId in LGuid)
                    {
                        strOrgUserIds.Append(orgUserId).Append(",");
                    }
                    strOrgUserIds.Remove(strOrgUserIds.Length - 1, 1);
                    addmassage.AddMessages(commodityOrder.Id.ToString(), strOrgUserIds.ToString(), appId, commodityOrder.Code, state, messages, "orderAppOwner");


                    //后台发消息 
                    Jinher.AMP.Info.Deploy.CustomDTO.MessageForAddDTO messageAdd = new Info.Deploy.CustomDTO.MessageForAddDTO();
                    //推送消息


                    messageAdd.PublishTime = DateTime.Now;
                    messageAdd.ReceiverUserId = LGuid;

                    //messageAdd.SenderOrgId = employeeDTO.EBCOrganizationId;
                    messageAdd.SenderType = Info.Deploy.Enum.SenderType.System;
                    messageAdd.Title = title;
                    messageAdd.Content = content;
                    //messageAdd.MessageType = "OrgDisableMessage";
                    messageAdd.ReceiverRange = Info.Deploy.Enum.ReceiverRange.SpecifiedUser;

                    var retret = Jinher.AMP.BTP.TPS.InfoSV.Instance.AddSystemMessage(messageAdd);
                    LogHelper.Info(string.Format("SendMessageCommon后台发送消息服务成功。messageAdd：{0}", JsonHelper.JsonSerializer(messageAdd)));
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SendMessageCommon发送消息服务异常:。title：{0}，content：{1}，mobilemess：{2}，state：{3}，appId：{4}，Id：{5}，code：{6}", title, content, mobilemess, state, appId, commodityOrder.Id, commodityOrder.Code), ex);
            }

        }

        internal void CPSCallBack(CommodityOrder commodityOrder, ContextDTO contextDTO)
        {
            try
            {
                ADM.Deploy.CustomDTO.CpsCostParamCDTO cpsDTO = new ADM.Deploy.CustomDTO.CpsCostParamCDTO();
                cpsDTO.CpsId = commodityOrder.CPSId;
                cpsDTO.CustomerId = commodityOrder.UserId;
                cpsDTO.TradingVolume = commodityOrder.RealPrice ?? decimal.Zero;
                cpsDTO.SecurityCode = commodityOrder.Code;

                ADM.Deploy.CustomDTO.CpsCostReturnCDTO cpsResult = Jinher.AMP.BTP.TPS.ADMSV.Instance.EffectivePromote(cpsDTO);

                if (!cpsResult.IsSuccess)
                {
                    LogHelper.Error(string.Format("订单状态修改服务异常:更新CPS通知失败。orderId：{0}", commodityOrder.Id));
                }
            }
            catch (Exception ex)
            {

                LogHelper.Error(string.Format("订单状态修改服务异常:更新CPS通知异常。orderId：{0}", commodityOrder.Id), ex);
            }
        }

        /// <summary>
        /// 查询订单详情
        /// </summary>
        /// <param name="commodityorderId">订单ID</param>
        /// <param name="userId">用户ID</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderSDTO GetOrderItemsExt(System.Guid commodityorderId,
            System.Guid userId, System.Guid appId)
        {
            try
            {
                //获取订单

                OrderItem od = new OrderItem();
                var commodityOrder = CommodityOrder.ObjectSet().FirstOrDefault(n => n.Id == commodityorderId);
                var orderRefund = OrderRefund.ObjectSet().FirstOrDefault(n => n.OrderId == commodityorderId && n.State != 2 && n.State != 3 && n.State != 4 && n.State != 13);
                var invoice = Invoice.ObjectSet().FirstOrDefault(n => n.CommodityOrderId == commodityorderId);

                if (commodityOrder == null)
                {
                    return null;
                }

                #region 有规格的订单总价的计算
                var orderitem = OrderItem.ObjectSet().Where(p => p.CommodityOrderId == commodityorderId).ToList();
                if (orderitem != null && orderitem.Count() > 0)
                {
                    decimal shijiPrice = 0;
                    foreach (var item in orderitem)
                    {
                        //if (item.Specifications != 0 && item.Specifications!=null)
                        //{
                        //    shijiPrice += (Convert.ToInt32(item.Specifications) * Convert.ToDecimal(item.RealPrice));
                        //}
                        //else
                        //{
                        //    shijiPrice += 1 * Convert.ToDecimal(item.RealPrice);
                        //}
                        shijiPrice += item.Number * Convert.ToDecimal(item.RealPrice);
                    }
                    //commodityOrder.RealPrice = shijiPrice;
                    commodityOrder.Price = shijiPrice;
                }
                #endregion

                int invtype = 0;
                string invtitle = "";
                string invcontent = "";
                InvoiceDTO invoiceDTO = null;
                if (invoice != null)
                {
                    invoiceDTO = invoice.ToEntityData();
                    invtype = invoice.InvoiceType;
                    invtitle = invoice.InvoiceTitle;
                    invcontent = invoice.InvoiceContent;
                }
                //获取物流信息表中的指定订单的子物流单集合。
                string SubExpressNos_ = "";
                var subnos = YXExpressDetailInfo.ObjectSet().FirstOrDefault((o => o.OrderId == commodityorderId));
                if (subnos != null) SubExpressNos_ = subnos.SubExpressNos;
                if (string.IsNullOrEmpty(SubExpressNos_)) SubExpressNos_ = "";

                //获取订单商品
                var commodityList = (from o in OrderItem.ObjectSet()
                                     where o.CommodityOrderId == commodityorderId
                                     select new OrderListItemCDTO
                                     {
                                         Id = o.Id,
                                         Pic = o.PicturesPath,
                                         Name = o.Name,
                                         Price = (decimal)((o.DiscountPrice != null && o.DiscountPrice > 0) ? o.DiscountPrice : o.CurrentPrice),
                                         CommodityNumber = o.Number,
                                         OrderId = o.CommodityOrderId,
                                         Size = o.CommodityAttributes,
                                         HasReview = o.AlreadyReview,
                                         Intensity = o.Intensity == null ? decimal.Zero : o.Intensity.Value,
                                         DiscountPrice = (decimal)(o.DiscountPrice == null ? -1 : o.DiscountPrice),
                                         RealPrice = o.RealPrice == null ? decimal.Zero : o.RealPrice.Value,
                                         CommodityId = o.CommodityId,
                                         ComCategoryName = o.ComCategoryName,
                                         Duty = o.Duty,
                                         _DeliveryTime = o.DeliveryTime,
                                         _DeliveryDays = o.DeliveryDays,
                                         Type = o.Type ?? 0,
                                         Specifications = o.Specifications ?? 0,
                                         CouponPrice = o.CouponPrice == null ? decimal.Zero : (decimal)o.CouponPrice,
                                         FreightPrice = o.FreightPrice == null ? decimal.Zero : (decimal)o.FreightPrice,
                                         YjbPrice = o.YjbPrice == null ? decimal.Zero : (decimal)o.YjbPrice,
                                         ChangeFreightPrice = o.ChangeFreightPrice == null ? decimal.Zero : (decimal)o.ChangeFreightPrice,
                                         ChangeRealPrice = o.ChangeRealPrice == null ? decimal.Zero : (decimal)o.ChangeRealPrice,
                                         State = o.State == null ? 0 : (int)o.State,
                                         YJCouponPrice = o.YJCouponPrice == null ? decimal.Zero : (decimal)o.YJCouponPrice
                                     }).ToList();
                // 获取赠品信息
                var presents = OrderItemPresent.ObjectSet().Where(_ => _.CommodityOrderId == commodityorderId).Select(o =>
                    new OrderListItemPresentDTO
                    {
                        Id = o.Id,
                        OrderItemId = o.OrderItemId,
                        Pic = o.PicturesPath,
                        Name = o.Name,
                        Price = (decimal)((o.DiscountPrice != null && o.DiscountPrice > 0) ? o.DiscountPrice : o.CurrentPrice),
                        CommodityNumber = o.Number,
                        OrderId = o.CommodityOrderId,
                        Size = o.CommodityAttributes,
                        HasReview = o.AlreadyReview,
                        Intensity = o.Intensity == null ? decimal.Zero : o.Intensity.Value,
                        DiscountPrice = (decimal)(o.DiscountPrice == null ? -1 : o.DiscountPrice),
                        RealPrice = o.RealPrice == null ? decimal.Zero : o.RealPrice.Value,
                        CommodityId = o.CommodityId,
                        ComCategoryName = o.ComCategoryName,
                        Duty = o.Duty,
                        Type = o.Type ?? 0
                    }).ToList();

                // 设置发货时间
                commodityList.ForEach(o =>
                {
                    if (o._DeliveryTime.HasValue)
                    {
                        o.DeliveryTime = o._DeliveryTime.Value.ToString("yyyy-MM-dd");
                    }
                    else if (o._DeliveryDays.HasValue)
                    {
                        o.DeliveryTime = "支付后" + o._DeliveryDays.Value + "天内";
                    }
                    // 设置赠品
                    o.Presents = presents.Where(_ => _.OrderItemId == o.Id).ToList();
                });
                var query = new CommodityOrderSDTO()
                {
                    AppId = commodityOrder.AppId,
                    EsAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId,
                    Code = commodityOrder.Code,
                    SubTime = commodityOrder.SubTime,
                    CommodityOrderId = commodityOrder.Id,
                    Price = commodityOrder.RealPrice == null ? decimal.Zero : commodityOrder.RealPrice.Value,
                    State = commodityOrder.State == 11 ? 1 : commodityOrder.State,
                    Details = commodityOrder.Details,
                    Payment = commodityOrder.Payment,
                    PaymentTime = commodityOrder.PaymentTime,
                    City = commodityOrder.City,
                    Province = commodityOrder.Province,
                    District = commodityOrder.District,
                    Street = commodityOrder.Street,
                    ShipmentsTime = commodityOrder.ShipmentsTime,
                    ReceiptUserName = commodityOrder.ReceiptUserName,
                    ReceiptAddress = commodityOrder.ReceiptAddress,
                    ReceiptPhone = commodityOrder.ReceiptPhone,
                    ShoppingCartItemSDTO = commodityList,
                    RecipientsZipCode = commodityOrder.RecipientsZipCode,
                    ShipExpCo = commodityOrder.ShipExpCo,
                    ExpOrderNo = commodityOrder.ExpOrderNo,
                    MessageToBuyer = commodityOrder.MessageToBuyer ?? "",
                    IsDelayConfirmTime = commodityOrder.IsDelayConfirmTime,
                    RefundMoney = orderRefund == null ? decimal.Zero : orderRefund.RefundMoney,
                    RefundDesc = orderRefund == null ? "" : orderRefund.RefundDesc,
                    Receiver = orderRefund == null ? "" : orderRefund.Receiver,
                    ReceiverAccount = orderRefund == null ? "" : orderRefund.ReceiverAccount,
                    RefundExpOrderNo = orderRefund == null ? "" : orderRefund.RefundExpOrderNo,
                    RefundExpCo = orderRefund == null ? "" : orderRefund.RefundExpCo,
                    Freight = commodityOrder.Freight,  //zgx-modify
                    IsModifiedPrice = commodityOrder.IsModifiedPrice,
                    OriginPrice = (commodityOrder.Price + commodityOrder.Freight),
                    IsDel = commodityOrder.IsDel,
                    RefundType = orderRefund == null ? 0 : orderRefund.RefundType,
                    InvoiceType = invtype,
                    InvoiceTitle = invtitle,
                    InvoiceContent = invcontent,
                    UserId = commodityOrder.UserId,
                    GoldPrice = commodityOrder.GoldPrice,
                    GoldCoupon = commodityOrder.GoldCoupon,
                    SelfTakeFlag = commodityOrder.SelfTakeFlag,
                    AgreeFlag = orderRefund == null ? -1 : orderRefund.AgreeFlag,
                    OrderRefundState = orderRefund == null ? -1 : orderRefund.State,
                    RefundScoreMoney = orderRefund == null ? 0 : orderRefund.RefundScoreMoney,
                    OrderType = commodityOrder.OrderType,
                    PicturesPath = commodityOrder.PicturesPath,
                    InvoiceDTO = invoiceDTO,
                    Batch = commodityOrder.Batch,
                    MealBoxFee = commodityOrder.MealBoxFee.HasValue ? commodityOrder.MealBoxFee.Value : 0,
                    Duty = commodityOrder.Duty,
                    YJCardPrice = commodityOrder.YJCardPrice == null ? 0 : Convert.ToDecimal(commodityOrder.YJCardPrice),
                    ModifiedOn = commodityOrder.ModifiedOn,
                    AppType = commodityOrder.AppType,
                    SubExpressNos = SubExpressNos_
                };
                if (query.IsDelayConfirmTime && query.ShipmentsTime != null)//如果已延长收货，则收货时间减三天，用于订单详情倒计时显示
                    query.ShipmentsTime = query.ShipmentsTime.Value.AddDays(-3);

                //订单未付到期关闭时间
                if (query.State == 0)//未支付
                {
                    var orderExpirePay = OrderExpirePay.ObjectSet().Where(o => o.OrderId == commodityorderId && o.State == 0).FirstOrDefault();
                    if (orderExpirePay != null)
                        query.ExpirePayTime = orderExpirePay.ExpirePayTime;
                }

                if (query.RefundMoney > 0)
                {
                    query.RefundMoney = OrderRefund.ObjectSet()
                        .Where(n => n.OrderId == commodityorderId && n.State != 2 && n.State != 3 && n.State != 4 && n.State != 13)
                        .Sum(n => n.RefundMoney + n.RefundScoreMoney);

                    query.RefundScoreMoney = OrderRefund.ObjectSet()
                        .Where(n => n.OrderId == commodityorderId && n.State != 2 && n.State != 3 && n.State != 4 && n.State != 13)
                        .Sum(n => n.RefundScoreMoney);
                }

                query.PaymentName = new PaySourceSV().GetPaymentNameExt(query.Payment);

                //取出订单的各项支付信息
                var opdList = OrderPayDetail.ObjectSet().Where(t => t.OrderId == commodityorderId).ToList();
                if (opdList.Any())
                {
                    //优惠券信息
                    var couponInfo = opdList.FirstOrDefault(t => t.ObjectType == 1);
                    if (couponInfo != null)
                    {
                        query.CouponComId = couponInfo.CommodityId == null ? Guid.Empty : couponInfo.CommodityId.Value;
                        query.CouponId = couponInfo.ObjectId;
                        query.CouponValue = couponInfo.Amount;
                    }
                    //积分抵现。
                    var scoreInfo = opdList.FirstOrDefault(t => t.ObjectType == 2);
                    if (scoreInfo != null)
                    {
                        query.ScorePrice = scoreInfo.Amount;
                    }

                    //配送费折扣信息费用
                    var deliveryFeeDiscount = opdList.FirstOrDefault(t => t.ObjectType == 3);
                    if (deliveryFeeDiscount != null)
                    {
                        query.DeliveryFeeDiscount = deliveryFeeDiscount.Amount;
                    }

                    //满免除运费。。
                    var freeAmount = opdList.FirstOrDefault(t => t.ObjectType == 4);
                    if (freeAmount != null)
                    {
                        query.FreeAmount = freeAmount.Amount;
                    }
                }

                // 易捷币抵现
                var yjbInfo = YJBSV.GetOrderInfo(commodityOrder.EsAppId, commodityorderId);
                if (yjbInfo.IsSuccess)
                {
                    if (yjbInfo.Data.YJBInfo != null)
                    {
                        query.YJBPrice = yjbInfo.Data.YJBInfo.InsteadCashAmount;
                        //query.Price -= query.YJBPrice ?? 0;
                    }
                    if (yjbInfo.Data.YJCouponInfo != null)
                    {
                        query.YJCouponPrice = yjbInfo.Data.YJCouponInfo.InsteadCashAmount;
                        //query.Price -= query.YJCouponPrice;
                    }
                }

                // 加上运费
                //query.Price = query.Price + query.Freight;

                //增加售后退款退货信息
                var orderRefundAfterSales = OrderRefundAfterSales.ObjectSet().Where(n => n.OrderId == commodityorderId).OrderByDescending(t => t.SubTime).FirstOrDefault();
                if (orderRefundAfterSales != null)
                {
                    query.RefundMoney = orderRefundAfterSales.RefundMoney;
                    query.RefundDesc = orderRefundAfterSales.RefundDesc;
                    query.RefundExpOrderNo = orderRefundAfterSales.RefundExpOrderNo;
                    query.RefundExpCo = orderRefundAfterSales.RefundExpCo;
                    query.OrderRefundAfterSalesState = orderRefundAfterSales.State;
                    query.RefundType = orderRefundAfterSales.RefundType;
                    query.RefundScoreMoney = orderRefundAfterSales.RefundScoreMoney;

                    decimal totRefundMoney = 0;
                    decimal totRefundScoreMoney = 0;
                    var orderItemIds = OrderRefundAfterSales.ObjectSet().Where(n => n.OrderId == commodityorderId).Select(t => t.OrderItemId).Distinct();

                    foreach (var orderItemId in orderItemIds)
                    {
                        var a = OrderRefundAfterSales.ObjectSet().Where(t => t.OrderItemId == orderItemId).OrderByDescending(t => t.SubTime).FirstOrDefault();
                        if (a != null)
                        {
                            totRefundMoney += a.RefundMoney + a.RefundScoreMoney;
                            totRefundScoreMoney += a.RefundScoreMoney;
                        }
                    }
                    if (totRefundMoney > 0)
                    {
                        query.RefundMoney = totRefundMoney;
                    }
                    if (totRefundScoreMoney > 0)
                    {
                        query.RefundScoreMoney = totRefundScoreMoney;
                    }
                }

                var commodityOrderService = CommodityOrderService.ObjectSet().Where(n => n.Id == commodityorderId).FirstOrDefault();
                if (commodityOrderService != null)
                {
                    query.StateAfterSales = commodityOrderService.State;
                }

                //自提
                if (commodityOrder.SelfTakeFlag == 1)
                {
                    var orderPickUp = AppOrderPickUp.ObjectSet().FirstOrDefault(c => c.Id == commodityOrder.Id);
                    if (orderPickUp != null)
                    {
                        query.SelfTakeName = orderPickUp.StsName;
                        query.PickUpPhone = orderPickUp.Phone;
                        query.SelfTakeAddress = orderPickUp.StsAddress.Replace(" ", "");
                        query.SelfTakePhone = orderPickUp.StsPhone;
                        query.PickUpTime = orderPickUp.BookDate;
                        query.PickUpStartTime = orderPickUp.BookStartTime;
                        query.PickUpEndTime = orderPickUp.BookEndTime;
                        query.PickUpName = orderPickUp.Name;
                        query.PickUpCode = orderPickUp.PickUpCode;
                        query.PickUpCodeUrl = orderPickUp.PickUpQrCodeUrl;
                    }
                }
                if (commodityOrder.SetMealId != null)
                {
                    var setMealActivitys = ZPHSV.Instance.GetSetMealActivitysById((Guid)commodityOrder.SetMealId);
                    query.SetMealActivity = setMealActivitys;
                }

                LogHelper.Debug(string.Format("commodityOrder.JcActivityId。commodityorderId：{0}" + ",commodityOrder.JcActivityId:{1}", commodityorderId, commodityOrder.JcActivityId));
                query.IsJcActivity = (commodityOrder.JcActivityId != null && commodityOrder.JcActivityId != Guid.Empty);

                //获取中石化的电子发票信息 IsRefund = false为没有申请退款 SMsgCode = "0000"为调用下载发票接口成功
                HTJSInvoice htjs = HTJSInvoice.ObjectSet().FirstOrDefault(t => t.SwNo == "jh" + query.Code && t.SMsgCode == "0000");
                if (htjs != null)
                {
                    query.SwNo = htjs.SwNo;
                }
                return query;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("查询订单详情异常。commodityorderId：{0}", commodityorderId), ex);

                return null;
            }
        }

        /// <summary>
        /// 查询分享订单详情页面
        /// </summary>
        /// <param name="commodityorderId">订单ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderShareDTO GetShareOrderItemsExt(System.Guid commodityorderId)
        {
            try
            {
                var orderIds = new List<Guid>() { commodityorderId };

                //判断订单是否是拆单
                var mainOrders = MainOrder.ObjectSet().Where(r => r.MainOrderId == commodityorderId).ToList();
                if (mainOrders != null && mainOrders.Count > 0)
                {
                    orderIds = mainOrders.Select(r => r.SubOrderId).Distinct().ToList();
                }

                //获取订单
                var commodityOrders = CommodityOrder.ObjectSet().Where(r => orderIds.Contains(r.Id)).ToList();
                if (commodityOrders == null || commodityOrders.Count == 0)
                {
                    return null;
                }

                var commodityOrder = commodityOrders.FirstOrDefault();

                //获取订单商品
                var commodityList = (from o in OrderItem.ObjectSet()
                                     join c in Commodity.ObjectSet() on o.CommodityId equals c.Id
                                     where orderIds.Contains(o.CommodityOrderId)
                                     select new OrderItemShareCDTO
                                     {
                                         Id = o.Id,
                                         Pic = o.PicturesPath,
                                         Name = o.Name,
                                         Price = o.CurrentPrice,
                                         CommodityNumber = o.Number,
                                         OrderId = o.CommodityOrderId,
                                         Size = o.CommodityAttributes,
                                         HasReview = o.AlreadyReview,
                                         Intensity = o.Intensity == null ? decimal.Zero : o.Intensity.Value,
                                         DiscountPrice = (decimal)(o.DiscountPrice == null ? -1 : o.DiscountPrice),
                                         RealPrice = o.RealPrice == null ? decimal.Zero : o.RealPrice.Value,
                                         CommodityId = o.CommodityId,
                                         ComCategoryName = o.ComCategoryName,
                                         Duty = o.Duty,
                                         MarketPrice = c.MarketPrice, //如果有属性的话，暂时不考虑，商品主表默认是属性的最小的那个价格
                                         CommodityPrice = c.Price, //如果有属性的话，暂时不考虑，商品主表默认是属性的最小的那个价格
                                         ShowOriPrice = c.MarketPrice,
                                         ShowRealPrice = c.Price
                                     }).ToList();


                if (commodityList != null && commodityList.Count > 0)
                {
                    var commodityIds = commodityList.Select(r => r.CommodityId).Distinct().ToList();
                    var todayPromotions = TodayPromotion.GetCurrentPromotionsWithPresell(commodityIds);
                    commodityList.ForEach(r =>
                    {
                        var todayPromotion = todayPromotions.FirstOrDefault(c => c.CommodityId == r.Id && c.PromotionType != 3);
                        if (todayPromotion != null)
                        {
                            var discountPrice = todayPromotion.DiscountPrice.HasValue ? todayPromotion.DiscountPrice.Value : -1;
                            r.ShowOriPrice = Commodity.GetShowOriPrice(r.CommodityPrice, r.MarketPrice, discountPrice, todayPromotion.Intensity);
                            r.ShowRealPrice = Commodity.GetShowRealPrice(r.CommodityPrice, discountPrice, todayPromotion.Intensity);
                        }
                    });
                    commodityList = commodityList.OrderBy(r => r.OrderId).ThenBy(r => r.Name).ToList();
                }
                var query = new CommodityOrderShareDTO()
                {
                    AppId = commodityOrder.AppId,
                    EsAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId,
                    ShareOrderItemDTO = commodityList,
                    UserId = commodityOrder.UserId
                };
                return query;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("查询订单详情异常。commodityorderId：{0}", commodityorderId), ex);

                return null;
            }
        }

        /// <summary>
        /// 查询用户订单数量 用esAppid区分
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="esAppId">馆Id</param>
        /// <returns></returns>
        public UserOrderCountDTO GetOrderCountExt(Guid userId, Guid esAppId)
        {
            var dto = getUserOrderCount(userId, esAppId);
            return new UserOrderCountDTO(userId, esAppId, dto);
        }

        /// <summary>
        /// 查询用户订单数量
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="esAppId">馆Id</param>
        /// <returns></returns>
        private OrderListCDTO getUserOrderCount(Guid userId, Guid esAppId)
        {
            //获取每个状态的订单数量
            Dictionary<int, int> q = new Dictionary<int, int>();

            var query = from c in CommodityOrder.ObjectSet()
                        join cs in CommodityOrderService.ObjectSet()
                             on c.Id equals cs.Id
                             into data1
                        from data in data1.DefaultIfEmpty()
                        where c.UserId == userId && c.State != 16 && c.State != 17 && c.IsDel != 1 && c.IsDel != 3
                        select new
                        {
                            EsAppId = c.EsAppId,
                            State = c.State,
                            StateAfterSales = data.State == null ? -1 : data.State
                        };

            if (esAppId != Guid.Empty)
                query = query.Where(t => t.EsAppId == esAppId);

            var result = query.ToList();

            List<int> stateAfterList = new List<int>() { 5, 10, 12 };

            //第三方订单未付款
            var querydsf = YJBDSFOrderInfo.ObjectSet().Where(d => d.OrderPayState == "待支付" && d.UserID == userId);
            var totalstate0_DSF = querydsf.Count();

            int totalState0 = result.Count(t => t.State == 0);
            int totalState1 = result.Count(t => t.State == 1 || t.State == 11);
            int totalState2 = result.Count(t => t.State == 2 || t.State == 13);
            int totalState3 = result.Count(t => t.State == 3 && (t.StateAfterSales == -1 || t.StateAfterSales == 3 || t.StateAfterSales == 13 || t.StateAfterSales == 15));
            int totalStateTui = result.Count(t => t.State == 8 || t.State == 9 || t.State == 10 || t.State == 12 || t.State == 14 || (t.State == 3 && stateAfterList.Contains(t.StateAfterSales)));

            //各种订单状态数量
            OrderListCDTO totalCommodityOrderSDTO = new OrderListCDTO();
            totalCommodityOrderSDTO.totalState0 = totalState0 + totalstate0_DSF;
            totalCommodityOrderSDTO.totalState1 = totalState1;
            totalCommodityOrderSDTO.totalState2 = totalState2;
            totalCommodityOrderSDTO.totalState3 = totalState3;
            totalCommodityOrderSDTO.totalStateTui = totalStateTui;

            return totalCommodityOrderSDTO;
        }

        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderListCDTO> GetCommodityOrderByStateExt(
               System.Guid userId, System.Guid appId, int state, int pageIndex, int pageSize)
        {
            try
            {
                List<OrderListCDTO> resultlist = new List<OrderListCDTO>();

                pageSize = pageSize == 0 ? 10 : pageSize;

                var totalCommodityOrderSDTO = getUserOrderCount(userId, appId);

                resultlist.Add(totalCommodityOrderSDTO);

                //获取该状态下的所有订单
                List<OrderListCDTO> commodityorderList = new List<OrderListCDTO>();
                var nonLastOrderIds = new Dictionary<Guid, OrderListCDTO>();
                if (appId != new Guid("00000000-0000-0000-0000-000000000000"))
                {
                    if (state == 1)
                    {
                        commodityorderList = CommodityOrder.ObjectSet()
                           .Where(n => n.AppId == appId && (n.State == 1 || n.State == 11) && n.UserId == userId && n.IsDel != 1 && n.IsDel != 3)
                           .OrderByDescending(n => n.ModifiedOn)
                           .Select(n => new OrderListCDTO
                           {
                               CommodityOrderId = n.Id,
                               Price = n.RealPrice == null ? 0 : n.RealPrice.Value,
                               IsModifiedPrice = n.IsModifiedPrice,
                               State = n.State,
                               Freight = n.Freight,
                               OriginPrice = n.Price + n.Freight,
                               SelfTakeFlag = n.SelfTakeFlag,
                               OrderType = n.OrderType
                           })
                           .Skip((pageIndex - 1) * pageSize)
                           .Take(pageSize).ToList();
                    }
                    else if (state == 3)  //交易成功 (售后为3 15)
                    {
                        List<int> listStateAfter = new List<int>() { 3, 15 };
                        commodityorderList = (from c in CommodityOrder.ObjectSet()
                                              join cs in CommodityOrderService.ObjectSet()
                                              on c.Id equals cs.Id
                                              into dataS
                                              from data in dataS.DefaultIfEmpty()
                                              where c.AppId == appId && (c.State == 3 && (data.State == null || listStateAfter.Contains(data.State))) && c.UserId == userId && c.IsDel != 1 && c.IsDel != 3
                                              orderby c.ModifiedOn descending

                                              select new OrderListCDTO
                                              {
                                                  CommodityOrderId = c.Id,
                                                  Price = c.RealPrice == null ? 0 : c.RealPrice.Value,
                                                  IsModifiedPrice = c.IsModifiedPrice,
                                                  State = c.State,
                                                  Freight = c.Freight,
                                                  OriginPrice = c.Price + c.Freight,
                                                  SelfTakeFlag = c.SelfTakeFlag,
                                                  StateAfterSales = data.State == null ? -1 : data.State,
                                                  OrderType = c.OrderType
                                              })
                          .Skip((pageIndex - 1) * pageSize)
                          .Take(pageSize).ToList();
                    }
                    else if (state == -1) //失败/退款
                    {
                        List<int> listState = CommodityOrder.GetOrderStateList(state);
                        List<int> listStateAfter = new List<int>() { 5, 7, 10, 12 };

                        commodityorderList = (from c in CommodityOrder.ObjectSet()
                                              join cs in CommodityOrderService.ObjectSet()
                                              on c.Id equals cs.Id
                                              into dataS
                                              from data in dataS.DefaultIfEmpty()
                                              where c.AppId == appId && (listState.Contains(c.State) || c.State == 3 && listStateAfter.Contains(data.State)) && c.UserId == userId && c.IsDel != 1 && c.IsDel != 3
                                              orderby c.ModifiedOn descending
                                              select new OrderListCDTO
                                              {
                                                  CommodityOrderId = c.Id,
                                                  Price = c.RealPrice == null ? 0 : c.RealPrice.Value,
                                                  IsModifiedPrice = c.IsModifiedPrice,
                                                  State = c.State,
                                                  Freight = c.Freight,
                                                  OriginPrice = c.Price + c.Freight,
                                                  SelfTakeFlag = c.SelfTakeFlag,
                                                  StateAfterSales = data.State == null ? -1 : data.State,
                                                  OrderType = c.OrderType
                                              })
                                              .Skip((pageIndex - 1) * pageSize)
                                              .Take(pageSize).ToList();
                    }
                    else
                    {
                        commodityorderList = CommodityOrder.ObjectSet()
                           .Where(n => n.AppId == appId && n.State == state && n.State != 16 && n.State != 17 && n.UserId == userId && n.IsDel != 1 && n.IsDel != 3)
                           .OrderByDescending(n => n.ModifiedOn)
                           .Select(n => new OrderListCDTO
                           {
                               CommodityOrderId = n.Id,
                               Price = n.RealPrice == null ? 0 : n.RealPrice.Value,
                               IsModifiedPrice = n.IsModifiedPrice,
                               State = n.State,
                               Freight = n.Freight,
                               OriginPrice = n.Price + n.Freight,
                               SelfTakeFlag = n.SelfTakeFlag,
                               OrderType = n.OrderType
                           })
                           .Skip((pageIndex - 1) * pageSize)
                           .Take(pageSize).ToList();
                        //if (state == 0)
                        //{
                        //    nonLastOrderIds = commodityorderList.Where(n => n.State == 0).Select(n => new { Id = n.CommodityOrderId, Order = n }).ToDictionary(x => x.Id, y => y.Order);
                        //    //HandleOrderState(nonLastOrderIds);
                        //}
                    }

                }
                else
                {

                    List<int> listState = CommodityOrder.GetOrderStateList(state);

                    if (state == 3)  //交易成功 (售后为3 15)
                    {
                        List<int> listStateAfter = new List<int>() { 3, 15 };
                        commodityorderList = (from c in CommodityOrder.ObjectSet()
                                              join cs in CommodityOrderService.ObjectSet()
                                              on c.Id equals cs.Id
                                              into dataS
                                              from data in dataS.DefaultIfEmpty()
                                              where (c.State == 3 && (data.State == null || listStateAfter.Contains(data.State))) && c.UserId == userId && c.IsDel != 1 && c.IsDel != 3
                                              orderby c.ModifiedOn descending

                                              select new OrderListCDTO
                                              {
                                                  CommodityOrderId = c.Id,
                                                  Price = c.RealPrice == null ? 0 : c.RealPrice.Value,
                                                  IsModifiedPrice = c.IsModifiedPrice,
                                                  State = c.State,
                                                  Freight = c.Freight,
                                                  OriginPrice = c.Price + c.Freight,
                                                  SelfTakeFlag = c.SelfTakeFlag,
                                                  StateAfterSales = data.State == null ? -1 : data.State,
                                                  OrderType = c.OrderType
                                              })
                          .Skip((pageIndex - 1) * pageSize)
                          .Take(pageSize).ToList();
                    }
                    else if (state == -1) //失败/退款
                    {
                        List<int> listStateAfter = new List<int>() { 5, 7, 10, 12 };

                        commodityorderList = (from c in CommodityOrder.ObjectSet()
                                              join cs in CommodityOrderService.ObjectSet()
                                              on c.Id equals cs.Id
                                              into dataS
                                              from data in dataS.DefaultIfEmpty()
                                              where (listState.Contains(c.State) || c.State == 3 && listStateAfter.Contains(data.State)) && c.UserId == userId && c.IsDel != 1 && c.IsDel != 3
                                              orderby c.ModifiedOn descending
                                              select new OrderListCDTO
                                              {
                                                  CommodityOrderId = c.Id,
                                                  Price = c.RealPrice == null ? 0 : c.RealPrice.Value,
                                                  IsModifiedPrice = c.IsModifiedPrice,
                                                  State = c.State,
                                                  Freight = c.Freight,
                                                  OriginPrice = c.Price + c.Freight,
                                                  SelfTakeFlag = c.SelfTakeFlag,
                                                  StateAfterSales = data.State == null ? -1 : data.State,
                                                  OrderType = c.OrderType
                                              })
                                              .Skip((pageIndex - 1) * pageSize)
                                              .Take(pageSize).ToList();
                    }
                    else
                    {

                        commodityorderList = CommodityOrder.ObjectSet()
                                              .Where(n => listState.Contains(n.State) && n.UserId == userId && n.IsDel != 1 && n.IsDel != 3)
                                              .OrderByDescending(n => n.ModifiedOn)
                                              .Select(n => new OrderListCDTO
                                              {
                                                  CommodityOrderId = n.Id,
                                                  Price = n.RealPrice == null ? 0 : n.RealPrice.Value,
                                                  IsModifiedPrice = n.IsModifiedPrice,
                                                  State = n.State,
                                                  Freight = n.Freight,
                                                  OriginPrice = n.Price + n.Freight,
                                                  SelfTakeFlag = n.SelfTakeFlag,
                                                  OrderType = n.OrderType
                                              })
                                              .Skip((pageIndex - 1) * pageSize)
                                              .Take(pageSize).ToList();
                    }
                }
                if (commodityorderList.Count() > 0)
                {
                    //查询订单列表的所有订单商品，并以订单id分组
                    List<Guid> commodityOrderIds = commodityorderList.Select(n => n.CommodityOrderId).ToList<Guid>();

                    //nonLastOrderIds = commodityorderList.Select(n => new { Id = n.CommodityOrderId, Order = n }).ToDictionary(x => x.Id, y => y.Order);

                    //var cacheOrders = GlobalCacheWrapper.GetData("G_Order", userId.ToString(), CacheTypeEnum.redisSS, "BTPCache") as Dictionary<Guid, OrderListCDTO>;

                    //if (cacheOrders != null)
                    //{
                    //    List<Guid> delOrderIds = new List<Guid>();
                    //    foreach (var cacheOrderKey in cacheOrders.Keys)
                    //    {
                    //        var cacheOrder = cacheOrders[cacheOrderKey];
                    //        if (commodityOrderIds.Contains(cacheOrder.CommodityOrderId))
                    //        {
                    //            delOrderIds.Add(cacheOrderKey);
                    //        }
                    //        else
                    //        {
                    //            resultlist.Add(cacheOrder);
                    //            nonLastOrderIds.Add(cacheOrder.CommodityOrderId, cacheOrder);
                    //        }
                    //    }
                    //    if (delOrderIds.Count > 0)
                    //    {
                    //        foreach (var delOrderId in delOrderIds)
                    //        {
                    //            cacheOrders.Remove(delOrderId);
                    //        }
                    //        GlobalCacheWrapper.Add("G_Order", userId.ToString(), cacheOrders, CacheTypeEnum.redisSS, "BTPCache");
                    //    }
                    //}
                    //HandleOrderState(nonLastOrderIds);
                    var commoditySDTOList = (from o in OrderItem.ObjectSet()
                                             where commodityOrderIds.Contains(o.CommodityOrderId)
                                             select new OrderListItemCDTO
                                             {
                                                 Id = o.Id,
                                                 OrderId = o.CommodityOrderId,
                                                 Pic = o.PicturesPath,
                                                 Name = o.Name,
                                                 Price = o.CurrentPrice,
                                                 CommodityNumber = o.Number,
                                                 Size = o.CommodityAttributes,
                                                 HasReview = o.AlreadyReview,
                                                 Intensity = (decimal)o.Intensity,
                                                 DiscountPrice = (decimal)(o.DiscountPrice != null ? o.DiscountPrice : -1),
                                                 CommodityId = o.CommodityId
                                             }).ToList();
                    Dictionary<Guid, List<OrderListItemCDTO>> csdtoList = commoditySDTOList
                        .GroupBy(c => c.OrderId, (key, group) => new { OrderId = key, CommodityList = group })
                        .ToDictionary(c => c.OrderId, c => c.CommodityList.ToList());

                    foreach (var commodityOrder in commodityorderList)
                    {
                        //订单信息
                        //OrderListCDTO commodityOrderSDTO = new OrderListCDTO()
                        //{
                        //    CommodityOrderId = commodityOrder.Id,
                        //    Price = (decimal)commodityOrder.RealPrice,
                        //    State = commodityOrder.State,
                        //    Freight = commodityOrder.Freight,
                        //    IsModifiedPrice = commodityOrder.IsModifiedPrice,
                        //    OriginPrice = commodityOrder.Price + commodityOrder.Freight

                        //};
                        //if (state != -1 && state != commodityOrder.State)
                        //{
                        //    continue;
                        //}
                        if (csdtoList.ContainsKey(commodityOrder.CommodityOrderId))
                        {
                            var commodityDTOList = csdtoList[commodityOrder.CommodityOrderId];
                            commodityOrder.ShoppingCartItemSDTO = commodityDTOList;
                        }

                        //转换订单状态
                        commodityOrder.State = ConvertOrderStateAfterSales(commodityOrder.StateAfterSales);

                        resultlist.Add(commodityOrder);
                    }
                }
                return resultlist;
            }
            catch (Exception ex)
            {

                LogHelper.Error(string.Format("根据交易状态获取订单异常。userId：{0}，appId：{1}，state：{2}", userId, appId, state), ex);

                return null;
            }
        }


        private void HandleOrderState(Dictionary<Guid, OrderListCDTO> nonLastOrderIds)
        {
            var nonLastOrderStates = CommodityOrder.ObjectSet()
                                        .Where(n => nonLastOrderIds.Keys.Contains(n.Id)).Select(n =>
                                            new { Id = n.Id, State = n.State, RealPrice = n.RealPrice == null ? 0 : n.RealPrice.Value, IsModifiedPrice = n.IsModifiedPrice })
                                            .ToDictionary(x => x.Id, y => new
                                            {
                                                State = y.State,
                                                RealPrice = y.RealPrice,
                                                IsModifiedPrice = y.IsModifiedPrice
                                            });
            foreach (var orderId in nonLastOrderIds.Keys)
            {
                if (nonLastOrderStates.ContainsKey(orderId))
                {
                    nonLastOrderIds[orderId].State = nonLastOrderStates[orderId].State;
                    nonLastOrderIds[orderId].Price = nonLastOrderStates[orderId].RealPrice;
                    nonLastOrderIds[orderId].IsModifiedPrice = nonLastOrderStates[orderId].IsModifiedPrice;
                }
            }
        }


        /// <summary>
        /// 获取用户所有订单
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="state">订单状态0：未付款|1:未发货|2:已发货|3:交易成功|-1：失败</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderListCDTO> GetCommodityOrderByUserIDExt(
            System.Guid userId, int pageIndex, int pageSize, int? state)
        {
            try
            {
                List<OrderListCDTO> resultlist = new List<OrderListCDTO>();

                pageSize = pageSize == 0 ? 10 : pageSize;

                //获取该状态下的所有订单
                List<int> stateList = CommodityOrder.GetOrderStateList(state);
                var query = from c in CommodityOrder.ObjectSet()
                            join cs in CommodityOrderService.ObjectSet()
                            on c.Id equals cs.Id
                            into data1
                            from data in data1.DefaultIfEmpty()
                            where c.State != 16 && c.State != 17
                            select new
                            {
                                commodityOrder = c,
                                StateAfterSales = data.State == null ? -1 : data.State
                            };


                if (state.HasValue && stateList != null && stateList.Any())
                {
                    //加入售后部分
                    if (state == -1)
                    {
                        List<int> stateAfterList = new List<int>() { 5, 7, 10, 12 };
                        query = query.Where(n => (stateList.Contains(n.commodityOrder.State) || (n.commodityOrder.State == 3 && stateAfterList.Contains(n.StateAfterSales))) && n.commodityOrder.UserId == userId && n.commodityOrder.IsDel != 1 && n.commodityOrder.IsDel != 3);
                    }
                    else
                    {
                        query = query.Where(n => stateList.Contains(n.commodityOrder.State) && n.commodityOrder.UserId == userId && n.commodityOrder.IsDel != 1 && n.commodityOrder.IsDel != 3);
                    }
                }
                else
                {
                    query = query.Where(n => n.commodityOrder.UserId == userId && n.commodityOrder.IsDel != 1 && n.commodityOrder.IsDel != 3);
                }
                var
                    commodityorderList = query.OrderByDescending(n => n.commodityOrder.SubTime)
                    .Select(n => new OrderListCDTO
                    {
                        CommodityOrderId = n.commodityOrder.Id,
                        Price = (decimal)n.commodityOrder.RealPrice,
                        AppId = n.commodityOrder.AppId,
                        UserId = n.commodityOrder.UserId,
                        State = n.commodityOrder.State,
                        Freight = n.commodityOrder.Freight,
                        IsModifiedPrice = n.commodityOrder.IsModifiedPrice,
                        OriginPrice = n.commodityOrder.Price + n.commodityOrder.Freight,
                        PayType = n.commodityOrder.Payment,
                        SelfTakeFlag = n.commodityOrder.SelfTakeFlag,
                        StateAfterSales = n.StateAfterSales,
                        OrderRefundState = -1,
                        OrderRefundAfterSalesState = -1,
                        OrderType = n.commodityOrder.OrderType
                    })
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize).ToList();

                //查询订单列表的所有订单商品，并以订单id分组
                List<Guid> commodityOrderIds = commodityorderList.Select(n => n.CommodityOrderId).ToList<Guid>();
                //var nonLastOrderIds = commodityorderList.Select(n => new { Id = n.CommodityOrderId, Order = n }).ToDictionary(x => x.Id, y => y.Order);

                //var cacheOrders = GlobalCacheWrapper.GetData("G_Order", userId.ToString(), CacheTypeEnum.redisSS, "BTPCache") as Dictionary<Guid, OrderListCDTO>;
                //if (cacheOrders != null)
                //{
                //    List<Guid> delOrderIds = new List<Guid>();
                //    foreach (var cacheOrderKey in cacheOrders.Keys)
                //    {
                //        var cacheOrder = cacheOrders[cacheOrderKey];
                //        if (commodityOrderIds.Contains(cacheOrder.CommodityOrderId))
                //        {
                //            delOrderIds.Add(cacheOrderKey);
                //        }
                //        else
                //        {
                //            resultlist.Add(cacheOrder);
                //            nonLastOrderIds.Add(cacheOrder.CommodityOrderId, cacheOrder);
                //        }
                //    }
                //    if (delOrderIds.Count > 0)
                //    {
                //        foreach (var delOrderId in delOrderIds)
                //        {
                //            cacheOrders.Remove(delOrderId);
                //        }
                //        GlobalCacheWrapper.Add("G_Order", userId.ToString(), cacheOrders, CacheTypeEnum.redisSS, "BTPCache");
                //    }
                //}

                //HandleOrderState(nonLastOrderIds);

                if (commodityorderList.Any())
                {


                    var commoditySDTOList = (from o in OrderItem.ObjectSet()
                                             where commodityOrderIds.Contains(o.CommodityOrderId)
                                             select new OrderListItemCDTO
                                             {
                                                 Id = o.Id,
                                                 OrderId = o.CommodityOrderId,
                                                 Pic = o.PicturesPath,
                                                 Name = o.Name,
                                                 Price = o.CurrentPrice,
                                                 CommodityNumber = o.Number,
                                                 Size = o.CommodityAttributes,
                                                 HasReview = o.AlreadyReview,
                                                 Intensity = (decimal)o.Intensity,
                                                 DiscountPrice = (decimal)(o.DiscountPrice != null ? o.DiscountPrice : -1),
                                                 CommodityId = o.CommodityId
                                             }).ToList();
                    Dictionary<Guid, List<OrderListItemCDTO>> csdtoList = commoditySDTOList
                        .GroupBy(c => c.OrderId, (key, group) => new { OrderId = key, CommodityList = group })
                        .ToDictionary(c => c.OrderId, c => c.CommodityList.ToList());

                    var listAppIds = (from co in commodityorderList select co.AppId).Distinct().ToList();
                    Dictionary<Guid, string> dictAppName = APPSV.GetAppNameListByIds(listAppIds);

                    var idList = commodityorderList.Select(t => t.CommodityOrderId).ToList();

                    //售中申请表
                    var middle = (from o in OrderRefund.ObjectSet()
                                  where idList.Contains(o.OrderId)
                                  select o).GroupBy(t => t.OrderId).ToDictionary(x => x.Key, y => y.OrderByDescending(t => t.SubTime).FirstOrDefault());
                    //售后申请表
                    var afterSales = (from o in OrderRefundAfterSales.ObjectSet()
                                      where idList.Contains(o.OrderId)
                                      select o).GroupBy(t => t.OrderId).ToDictionary(x => x.Key, y => y.OrderByDescending(t => t.SubTime).FirstOrDefault());



                    foreach (var commodityOrder in commodityorderList)
                    {
                        //订单信息
                        //OrderListCDTO commodityOrderSDTO = new OrderListCDTO()
                        //{
                        //    CommodityOrderId = commodityOrder.CommodityOrderId,
                        //    Price = commodityOrder.Price,
                        //    AppId = commodityOrder.AppId,
                        //    UserId = commodityOrder.UserId,
                        //    State = commodityOrder.State,
                        //    Freight = commodityOrder.Freight,
                        //    IsModifiedPrice = commodityOrder.IsModifiedPrice,
                        //    OriginPrice = commodityOrder.Price + commodityOrder.Freight
                        //};
                        if (csdtoList.ContainsKey(commodityOrder.CommodityOrderId))
                        {
                            var commodityDTOList = csdtoList[commodityOrder.CommodityOrderId];
                            commodityOrder.ShoppingCartItemSDTO = commodityDTOList;
                        }
                        if (dictAppName != null && dictAppName.Count > 0 && dictAppName.ContainsKey(commodityOrder.AppId))
                        {
                            var appNameDto = dictAppName[commodityOrder.AppId];
                            commodityOrder.AppName = appNameDto;
                        }

                        //加入售申请状态
                        if (commodityOrder.State == 3)
                        {
                            if (afterSales != null)
                            {
                                var refund = afterSales.Where(t => t.Key == commodityOrder.CommodityOrderId).Select(t => t.Value).FirstOrDefault();
                                if (refund != null)
                                {
                                    commodityOrder.OrderRefundAfterSalesState = refund.State;
                                }
                            }
                        }
                        else
                        {
                            if (middle != null)
                            {
                                var refund = middle.Where(t => t.Key == commodityOrder.CommodityOrderId).Select(t => t.Value).FirstOrDefault();
                                if (refund != null)
                                {
                                    commodityOrder.OrderRefundState = refund.State;
                                }
                            }
                        }
                        resultlist.Add(commodityOrder);
                    }
                }
                return resultlist;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取用户所有订单。userId：{0}", userId), ex);

                return null;
            }
        }

        /// <summary>
        /// 金币确认收货
        /// /// </summary>
        /// <param name="commodityOrderId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public ResultDTO ConfirmOrderExt(Guid commodityOrderId, string password)
        {
            if (commodityOrderId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数不能为空" };
            }
            if (!OrderSV.LockOrder(commodityOrderId))
            {
                return new ResultDTO { ResultCode = 110, Message = "操作失败" };
            }
            try
            {
                List<Commodity> needRefreshCacheCommoditys = new List<Commodity>();

                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                DateTime now = DateTime.Now;
                var order = CommodityOrder.ObjectSet().Where(n => n.Id == commodityOrderId).FirstOrDefault();

                if (order == null)
                {
                    return new ResultDTO { ResultCode = 1, Message = "订单处理中" };
                }
                int oldState = order.State;
                //订单状态不是待收货，或自提待收货
                if (order.State != 2 && order.State != 13 && !(order.State == 1 && order.SelfTakeFlag == 1))
                {
                    return new ResultDTO { ResultCode = 1, Message = "订单已支付过" };
                }
                //订单支付方式不是金币支付
                if (order.Payment > 0)
                {
                    return new ResultDTO { ResultCode = 1, Message = "订单支付方式不是金币" };
                }
                App.Deploy.CustomDTO.AppIdOwnerIdTypeDTO applicationDTO = APPSV.Instance.GetAppOwnerInfo(order.AppId);
                //金币支付

                List<object> saveListUnUsed;

                Jinher.AMP.FSP.Deploy.CustomDTO.ReturnInfoDTO goldPayresult = new FSP.Deploy.CustomDTO.ReturnInfoDTO();
                if (order.RealPrice > 0)
                {
                    if (order.OrderType != 1)
                    {
                        try
                        {
                            //冻结接口调用
                            var confirmPayDTO = OrderSV.BuildConfirmPayDTO(contextSession, order, out saveListUnUsed,
                                                                           applicationDTO, password);
                            LogHelper.Info(
                                string.Format(
                                    "订单确认收货:ConfirmOrderExt goldPayFacade.ConfirmPayFreeze：订单id={0} , 支付DTO ={1}",
                                    order.Id, JsonHelper.JsonSerializer(confirmPayDTO)), "BTP_Order");
                            goldPayresult = Jinher.AMP.BTP.TPS.FSPSV.Instance.ConfirmPayFreeze(confirmPayDTO);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error(string.Format("确认支付金币ConfirmPayFreeze接口支付失败。code：{0}，Message：{1}",
                                                          goldPayresult.Code, goldPayresult.Message + ex.ToString()));
                            return new ResultDTO { ResultCode = 1, Message = "金币支付异常" };
                        }
                    }
                    else
                    {
                        try
                        {
                            //冻结接口调用
                            var confirmPayDTO = OrderSV.BuildConfirmPayDTO(contextSession, order, out saveListUnUsed,
                                                                           applicationDTO, password);
                            LogHelper.Info(
                                string.Format("订单确认收货:ConfirmOrderExt goldPayFacade.ConfirmPay：订单id={0} , 支付DTO ={1}",
                                              order.Id, JsonHelper.JsonSerializer(confirmPayDTO)), "BTP_Order");
                            goldPayresult = Jinher.AMP.BTP.TPS.FSPSV.Instance.ConfirmPay(confirmPayDTO);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error(string.Format("确认支付金币ConfirmPay接口支付失败。code：{0}，Message：{1}",
                                                          goldPayresult.Code, goldPayresult.Message + ex.ToString()));
                            return new ResultDTO { ResultCode = 1, Message = "金币支付异常" };
                        }
                    }
                }
                //金币支付成功
                if (order.RealPrice <= 0 || goldPayresult.Code == 0)
                {
                    ////更新销量
                    //try
                    //{
                    //    var orderitemlist = OrderItem.ObjectSet().Where(n => n.CommodityOrderId == order.Id);
                    //    foreach (OrderItem items in orderitemlist)
                    //    {
                    //        Commodity com = Commodity.ObjectSet().Where(n => n.Id == items.CommodityId).First();
                    //        com.EntityState = System.Data.EntityState.Modified;
                    //        //加销量
                    //        com.Salesvolume += items.Number;
                    //        contextSession.SaveObject(com);
                    //        needRefreshCacheCommoditys.Add(com);
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    LogHelper.Error("确认支付更新库存异常。", ex);
                    //}

                    //更新订单状态
                    order.State = 3;
                    order.ConfirmTime = now;
                    order.ModifiedOn = now;
                    order.EntityState = System.Data.EntityState.Modified;
                    contextSession.SaveObject(order);

                    // 更新结算单确认收货时间
                    SettleAccountHelper.ConfirmOrder(contextSession, order);

                    if (order.Payment != 1 && order.OrderType != 1)
                    {
                        CommodityOrderService commodityOrderService = new CommodityOrderService();
                        commodityOrderService.Id = order.Id;
                        commodityOrderService.Name = order.Name;
                        commodityOrderService.Code = order.Code;
                        commodityOrderService.State = order.State;
                        commodityOrderService.SubId = order.SubId;
                        commodityOrderService.SelfTakeFlag = order.SelfTakeFlag;
                        commodityOrderService.EntityState = System.Data.EntityState.Added;
                        contextSession.SaveObject(commodityOrderService);
                    }

                    contextSession.SaveChanges();

                    if (needRefreshCacheCommoditys.Any())
                    {
                        needRefreshCacheCommoditys.ForEach(c => c.RefreshCache(EntityState.Modified));
                    }

                    #region CPS通知

                    LogHelper.Info(string.Format("CPS通知：来源类型:{0},CPSId：{1}", order.SrcType, order.CPSId));
                    if ((order.SrcType == 33 || order.SrcType == 34 || order.SrcType == 36 || order.SrcType == 39 ||
                         order.SrcType == 40) && !string.IsNullOrEmpty(order.CPSId) && order.CPSId != "null")
                    {
                        LogHelper.Info(string.Format("CPS通知：来源类型:{0},CPSId：{1}", order.SrcType, order.CPSId));
                        CPSCallBack(order, null);
                    }

                    #endregion

                    try
                    {
                        //保存日志
                        ContextSession contextSessionLog = ContextFactory.CurrentThreadContext;
                        //订单日志
                        Journal journal = new Journal();
                        journal.Id = Guid.NewGuid();
                        journal.Name = "用户确认收货";
                        journal.Code = order.Code;
                        journal.SubId = order.UserId;
                        journal.SubTime = now;
                        journal.Details = "订单状态由" + oldState + "变为" + order.State;
                        journal.StateFrom = oldState;
                        journal.StateTo = order.State;
                        journal.IsPush = false;
                        journal.OrderType = order.OrderType;
                        journal.CommodityOrderId = commodityOrderId;

                        journal.EntityState = System.Data.EntityState.Added;
                        contextSessionLog.SaveObject(journal);
                        contextSessionLog.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(string.Format("用户确认收货日志保存异常。commodityOrderId:{0}", commodityOrderId), ex);
                    }

                    //发送消息
                    System.Threading.ThreadPool.QueueUserWorkItem(
                        a =>
                        {
                            //添加消息                                              
                            AddMessage addmassage = new AddMessage();
                            string odid = commodityOrderId.ToString();
                            string usid = order.UserId.ToString();
                            string type = "order";

                            Guid EsAppId = order.EsAppId.HasValue ? order.EsAppId.Value : order.AppId;

                            addmassage.AddMessages(odid, usid, EsAppId, order.Code, order.State, "", type);

                        });


                    #region 发消息操作

                    if (order != null)
                    {
                        //TODO yjz 分成发红包消息规则需要修改  之前是计算分成金额，需要改为读取CommodityOrder.Commission字段

                        decimal realPrice = order.IsModifiedPrice ? order.RealPrice.Value : order.Price;
                        decimal total = realPrice * CustomConfig.SaleShare.Commission;
                        decimal commission = 0;
                        commission = (long)(total * 100) * 10;
                        string commossionGold = (1000 * commission).ToString("0");
                        List<Guid> userIds = new List<Guid>();
                        if (order.SrcType == 33 || order.SrcType == 34 && commission > decimal.Zero)
                        {
                            OrderShareMess tempShare =
                                OrderShareMess.ObjectSet().FirstOrDefault(c => c.OrderId == order.Id);
                            if (tempShare != null && !string.IsNullOrEmpty(tempShare.ShareId) &&
                                tempShare.ShareId.ToLower() != "null" && tempShare.ShareId.ToLower() != "undefined")
                            {
                                SNS.Deploy.CustomDTO.ReturnInfoDTO<Guid> shareServiceResult =
                                    Jinher.AMP.BTP.TPS.SNSSV.Instance.GetShareUserId(tempShare.ShareId);
                                if (shareServiceResult != null)
                                {
                                    if (shareServiceResult.Code == "0")
                                    {
                                        userIds.Add(shareServiceResult.Content);
                                        SendMessageToPayment(userIds, "affirm", commossionGold, null, 0);
                                    }
                                    else
                                    {
                                        LogHelper.Error(
                                            string.Format(
                                                "确认收货后发送消息失败 返回code为 1：\"根据分享Id获取分享人Id\" 不成功,分享Id={0}，返回结果={1}",
                                                tempShare.ShareId, JsonHelper.JsonSerializer(shareServiceResult)));
                                    }
                                }
                            }
                        }
                    }

                    #endregion

                    YXOrderHelper.ConfirmReceivedOrder(order.Id);

                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                }
                else
                {
                    if (goldPayresult.Code == -9)
                    {
                        return new ResultDTO { ResultCode = 1, Message = "金币支付密码错误" };
                    }
                    else
                    {
                        LogHelper.Error(string.Format("确认支付金币接口支付失败。code：{0}，Message：{1}", goldPayresult.Code,
                                                      goldPayresult.Message));
                        return new ResultDTO
                        {
                            ResultCode = 1,
                            Message = "金币支付失败," + goldPayresult.Code + goldPayresult.Message
                        };
                    }
                }
            }
            catch (Exception ex)
            {

                LogHelper.Error(
                    string.Format("确认支付服务异常。commodityOrderId:{0},password：{1}", commodityOrderId, password), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(commodityOrderId);
            }
        }
        /// <summary>
        /// 确认按最新价支付
        /// </summary>
        /// <param name="commodityOrderId"></param>
        /// <returns></returns>
        public NewResultDTO ConfirmPayPriceExt(Guid commodityOrderId, Guid userId)
        {
            if (commodityOrderId == Guid.Empty)
            {
                return new NewResultDTO { ResultCode = 1, Message = "参数不能为空" };
            }
            if (!OrderSV.LockOrder(commodityOrderId))
            {
                return new NewResultDTO { ResultCode = 1, Message = "操作失败" };
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                CommodityOrder commodityOrder = CommodityOrder.ObjectSet().FirstOrDefault(n => n.Id == commodityOrderId && n.State == 0);
                if (commodityOrder == null)
                {
                    return new NewResultDTO { ResultCode = 1, Message = "订单不存在,无法支付" };
                }

                //商品订单原价
                decimal oldtotalPrice = (decimal)commodityOrder.Price;

                var orderitemlist = OrderItem.ObjectSet().Where(n => n.CommodityOrderId == commodityOrderId).ToList();
                List<Guid> commodityIds = orderitemlist.Select(o => o.CommodityId).ToList();
                if (orderitemlist.Count == 0)
                {
                    return new NewResultDTO { ResultCode = 1, Message = "订单商品为空,无法支付" };
                }

                DateTime now = DateTime.Now;
                List<TodayPromotion> compromoList = new List<TodayPromotion>();
                DateTime? expireTime = null;

                var orderExpirePay = OrderExpirePay.ObjectSet().FirstOrDefault(c => c.OrderId == commodityOrderId);
                if (orderExpirePay != null)
                {
                    if (now >= orderExpirePay.ExpirePayTime)
                    {
                        noPayOrder(commodityOrder, now);

                        return new NewResultDTO { ResultCode = 6, Message = "超时未支付，交易关闭" };
                    }
                    expireTime = orderExpirePay.ExpirePayTime;
                }

                #region 处理促销商品的促销库存 以及每人的限购数
                //处理促销商品的促销库存 以及每人的限购数
                List<Guid> notVipCommodityIds = orderitemlist.Where(c => c.PromotionType != (int)ComPromotionStatusEnum.VipIntensity).Select(o => o.CommodityId).ToList();
                string isCanSecPro = GlobalCacheWrapper.GetData("G_OrderSecPro:" + commodityOrder.AppId, commodityOrder.Id.ToString(), CacheTypeEnum.redisSS, "BTPCache") as string;
                if (isCanSecPro != "1")
                {
                    //拼团流程不会有继续支付，，这里过滤掉拼团活动
                    compromoList = (from t in TodayPromotion.ObjectSet()
                                    where notVipCommodityIds.Contains(t.CommodityId) && t.EndTime > now && t.StartTime < now && t.PromotionType != 3
                                    select t).ToList().Distinct().ToList();
                    if (compromoList.Count > 0)
                    {
                        compromoList = updateOrderPromotion(compromoList, orderitemlist, commodityOrder);
                    }
                    #region 重新获取促销活动信息 2018-07-28 libinghui添加
                    else
                    {
                        var temp = (from p in PromotionItems.ObjectSet()
                                    join pro in Promotion.ObjectSet() on p.PromotionId equals pro.Id
                                    where notVipCommodityIds.Contains(p.CommodityId) && !pro.IsDel && pro.IsEnable &&
                                    pro.EndTime >= now && (pro.StartTime <= now || pro.PresellStartTime <= now) && pro.PromotionType != 3
                                    orderby pro.PromotionType descending
                                    select new
                                    {
                                        PromotionId = p.PromotionId,
                                        CommodityId = p.CommodityId,
                                        Intensity = (decimal)p.Intensity,
                                        StartTime = pro.StartTime,
                                        EndTime = pro.EndTime,
                                        DiscountPrice = (decimal)p.DiscountPrice,
                                        LimitBuyEach = p.LimitBuyEach,
                                        LimitBuyTotal = p.LimitBuyTotal,
                                        SurplusLimitBuyTotal = p.SurplusLimitBuyTotal,
                                        AppId = pro.AppId,
                                        ChannelId = pro.ChannelId,
                                        OutsideId = pro.OutsideId,
                                        PresellStartTime = pro.PresellStartTime,
                                        PresellEndTime = pro.PresellEndTime,
                                        PromotionType = pro.PromotionType,
                                        GroupMinVolume = pro.GroupMinVolume,
                                        ExpireSecond = pro.ExpireSecond,
                                        Description = pro.Description
                                    }).Distinct().ToList();
                        temp.ForEach(p =>
                        {
                            compromoList.Add(new TodayPromotion
                            {
                                PromotionId = p.PromotionId,
                                CommodityId = p.CommodityId,
                                Intensity = p.Intensity,
                                StartTime = p.StartTime,
                                EndTime = p.EndTime,
                                DiscountPrice = p.DiscountPrice,
                                LimitBuyEach = p.LimitBuyEach,
                                LimitBuyTotal = p.LimitBuyTotal,
                                SurplusLimitBuyTotal = p.SurplusLimitBuyTotal,
                                AppId = p.AppId,
                                ChannelId = p.ChannelId,
                                OutsideId = p.OutsideId,
                                PresellStartTime = p.PresellStartTime,
                                PresellEndTime = p.PresellEndTime,
                                PromotionType = p.PromotionType,
                                GroupMinVolume = p.GroupMinVolume,
                                ExpireSecond = p.ExpireSecond,
                                Description = p.Description
                            });
                        });
                        if (compromoList.Count > 0)
                        {
                            compromoList = updateOrderPromotion(compromoList, orderitemlist, commodityOrder);
                        }
                    }
                    #endregion
                }
                else if (isCanSecPro == "1")
                {
                    compromoList = (from t in TodayPromotion.ObjectSet()
                                    where notVipCommodityIds.Contains(t.CommodityId) && t.EndTime > now && t.StartTime < now
                                    select t).ToList().Distinct().ToList();

                    #region 重新获取促销活动信息 2018-07-28 libinghui添加
                    if (compromoList.Count == 0)
                    {
                        var temp = (from p in PromotionItems.ObjectSet()
                                    join pro in Promotion.ObjectSet() on p.PromotionId equals pro.Id
                                    where notVipCommodityIds.Contains(p.CommodityId) && !pro.IsDel && pro.IsEnable &&
                                    pro.EndTime >= now && (pro.StartTime <= now || pro.PresellStartTime <= now)
                                    orderby pro.PromotionType descending
                                    select new
                                    {
                                        PromotionId = p.PromotionId,
                                        CommodityId = p.CommodityId,
                                        Intensity = (decimal)p.Intensity,
                                        StartTime = pro.StartTime,
                                        EndTime = pro.EndTime,
                                        DiscountPrice = (decimal)p.DiscountPrice,
                                        LimitBuyEach = p.LimitBuyEach,
                                        LimitBuyTotal = p.LimitBuyTotal,
                                        SurplusLimitBuyTotal = p.SurplusLimitBuyTotal,
                                        AppId = pro.AppId,
                                        ChannelId = pro.ChannelId,
                                        OutsideId = pro.OutsideId,
                                        PresellStartTime = pro.PresellStartTime,
                                        PresellEndTime = pro.PresellEndTime,
                                        PromotionType = pro.PromotionType,
                                        GroupMinVolume = pro.GroupMinVolume,
                                        ExpireSecond = pro.ExpireSecond,
                                        Description = pro.Description
                                    }).Distinct().ToList();
                        temp.ForEach(p =>
                        {
                            compromoList.Add(new TodayPromotion
                            {
                                PromotionId = p.PromotionId,
                                CommodityId = p.CommodityId,
                                Intensity = p.Intensity,
                                StartTime = p.StartTime,
                                EndTime = p.EndTime,
                                DiscountPrice = p.DiscountPrice,
                                LimitBuyEach = p.LimitBuyEach,
                                LimitBuyTotal = p.LimitBuyTotal,
                                SurplusLimitBuyTotal = p.SurplusLimitBuyTotal,
                                AppId = p.AppId,
                                ChannelId = p.ChannelId,
                                OutsideId = p.OutsideId,
                                PresellStartTime = p.PresellStartTime,
                                PresellEndTime = p.PresellEndTime,
                                PromotionType = p.PromotionType,
                                GroupMinVolume = p.GroupMinVolume,
                                ExpireSecond = p.ExpireSecond,
                                Description = p.Description
                            });
                        });
                    }
                    #endregion
                }

                #endregion

                int ResultCode = 0;
                string message = "";
                decimal totalPrice = 0;
                decimal totalDuty = 0;
                //订单减免
                decimal reduction = 0.0m;

                //如果商家修改过价格，则不需要验证
                if (commodityOrder.IsModifiedPrice == true)
                {
                    //订单商品序号
                    var index = 0;
                    //总改价运费
                    decimal changeFreight = 0;
                    //总改价订单金额
                    decimal changeOrderPrice = 0;
                    //计算是否修改价格 （商品价格 + 运费 + 关税 - 优惠券抵现 - 易捷币抵现 - 积分抵现 - 实际支付金额）
                    var orderItems = OrderItem.ObjectSet().Where(t => t.CommodityOrderId == commodityOrder.Id);
                    var couponValue = orderItems.Sum(t => t.CouponPrice == null ? 0 : (decimal)t.CouponPrice);
                    var yJbPrice = orderItems.Sum(t => t.YjbPrice == null ? 0 : (decimal)t.YjbPrice);
                    var scorePrice = orderItems.Sum(t => t.ScorePrice);

                    decimal revisePrice = commodityOrder.Price + commodityOrder.Freight + commodityOrder.Duty - couponValue - yJbPrice - scorePrice - (decimal)commodityOrder.RealPrice;
                    LogHelper.Debug(string.Format("计算是否修改价格,commodityOrder.Price:{0},commodityOrder.Freight:{1},orderSDTO.Duty:{2},couponValue:{3},yJBPrice：{4},scorePrice:{5},commodityOrder.RealPrice:{6}",
                        commodityOrder.Price, commodityOrder.Freight, commodityOrder.Duty, couponValue, yJbPrice, scorePrice, commodityOrder.RealPrice));
                    if (revisePrice > 0)
                    {
                        if (commodityOrder.Freight >= revisePrice)
                        {
                            changeFreight = revisePrice;
                            changeOrderPrice = 0;
                        }
                        else
                        {
                            changeFreight = commodityOrder.Freight;
                            changeOrderPrice = revisePrice - commodityOrder.Freight;
                        }
                        LogHelper.Debug(string.Format("计算是否修改价格111,changeFreight:{0},changeOrderPrice:{1}", changeFreight, changeOrderPrice));
                        //去除最后一个商品的运费改价之和 
                        decimal sumWipeLastChangeFreight = 0;
                        //去除最后一个商品的订单商品改价之和 
                        decimal sumWipeLastChangeOrderPrice = 0;
                        //改价运费 商品承担的改价运费=商品金额/所有贡献运费的商品金额之和*改价运费与原运费差额
                        //商品承担的改价商品金额=商品金额/订单所有商品售价之和*改价商品金额与原商品金额差额
                        foreach (var orderItem in orderItems)
                        {
                            if (index < orderItems.Count() - 1)
                            {
                                orderItem.ChangeFreightPrice = Math.Round((((decimal)orderItem.RealPrice * orderItem.Number) / commodityOrder.Price) * changeFreight, 2, MidpointRounding.AwayFromZero);
                                sumWipeLastChangeFreight += (decimal)orderItem.ChangeFreightPrice;
                                orderItem.ChangeRealPrice = Math.Round((((decimal)orderItem.RealPrice * orderItem.Number) / commodityOrder.Price) * changeOrderPrice, 2, MidpointRounding.AwayFromZero);
                                sumWipeLastChangeOrderPrice += (decimal)orderItem.ChangeRealPrice;
                            }
                            else
                            {
                                orderItem.ChangeFreightPrice = changeFreight - sumWipeLastChangeFreight;
                                orderItem.ChangeRealPrice = changeOrderPrice - sumWipeLastChangeOrderPrice;
                            }
                            index++;
                            orderItem.ModifiedOn = DateTime.Now;
                            orderItem.EntityState = EntityState.Modified;
                            contextSession.SaveObject(orderItem);
                        }
                    }
                    contextSession.SaveChanges();

                    return new NewResultDTO { ResultCode = 0, Message = commodityOrder.RealPrice.ToString(), IsModifiedPrice = 1, ExpireTime = expireTime };
                }

                List<Jinher.AMP.BTP.Deploy.CustomDTO.TemplateCountDTO> templateCounts = new List<TemplateCountDTO>();

                //处理订单商品
                //包括OrderItem表的 CurrentPrice、Intensity、DiscountPrice、RealPrice、PromotionType、PromotionDesc
                var coms = Commodity.ObjectSet()
                                         .Where(n => commodityIds.Contains(n.Id) && n.IsDel == false && n.State == 0)
                                         .Select(n => new ShortCom() { Id = n.Id, Stock = n.Stock, Price = n.Price, Duty = n.Duty }).ToList();
                var comStockIds = orderitemlist.Where(c => c.CommodityStockId != null && c.CommodityStockId != Guid.Empty).Select(c => c.CommodityStockId).Distinct().ToList();
                var comStocks =
                    CommodityStock.ObjectSet()
                                  .Where(c => comStockIds.Contains(c.Id))
                                  .Select(n => new ShortCom { Id = n.Id, Stock = n.Stock, Price = n.Price, Duty = n.Duty })
                                  .ToList();
                foreach (OrderItem items in orderitemlist)
                {

                    bool isModified = false;
                    decimal oldCurrentPrice = items.CurrentPrice;
                    decimal newCurrentPrice = 0;

                    decimal oldIntensity = items.Intensity.Value;
                    decimal newIntensity = 10;

                    decimal oldDiscountPrice = items.DiscountPrice.Value;
                    decimal newDiscountPrice = -1;

                    int? oldPromotionType = items.PromotionType;
                    int? newPromotionType = null;

                    decimal oldRealPrice = items.RealPrice.Value;

                    decimal oldDuty = items.Duty;
                    decimal newDuty = 0;
                    //decimal newRealPrice = 0;

                    var com = coms.FirstOrDefault(c => c.Id == items.CommodityId);
                    if (com == null)
                    {
                        ResultCode = 1;
                        message = "商品已下架或不存在,无法支付";
                        break;
                    }
                    int commoditynum = 0;
                    orderitemlist.Where(a => a.CommodityId == items.CommodityId).ToList().ForEach(a => { commoditynum += a.Number; });
                    if (com.Stock < commoditynum)
                    {
                        ResultCode = 1;
                        message = "商品库存不足,无法支付";
                        break;
                    }
                    if (items.CommodityStockId.HasValue && items.CommodityStockId.Value != Guid.Empty)
                    {
                        com = comStocks.FirstOrDefault(c => items.CommodityStockId.Value == c.Id);
                        if (com == null)
                        {
                            ResultCode = 1;
                            message = "此款商品已下架或不存在,无法支付";
                            break;
                        }

                        int commodityStocknum = 0;
                        orderitemlist.Where(a => a.CommodityId == items.CommodityId && a.CommodityStockId == items.CommodityStockId).ToList().ForEach(a => { commodityStocknum += a.Number; });
                        if (com.Stock < commodityStocknum)
                        {
                            ResultCode = 1;
                            message = "商品库存不足,无法支付";
                            break;
                        }
                    }
                    else
                    {
                        //单属性商品
                        if (CommodityStock.ObjectSet().Count(c => c.CommodityId == items.CommodityId) > 1)
                        {
                            ResultCode = 1;
                            message = "此款商品已下架或不存在,无法支付";
                            break;
                        }

                    }

                    newCurrentPrice = com.Price;
                    //判断是否还存在 套装活动的情况
                    ZPH.Deploy.CustomDTO.SetMealActivityCDTO setMeal = ZPHSV.Instance.GetSetMealActivitysById((Guid)commodityOrder.SetMealId);
                    if (setMeal != null && setMeal.SetMealItemsCdtos != null && items.CommodityStockId.HasValue)
                    {
                        var item = setMeal.SetMealItemsCdtos.FirstOrDefault(t => t.CommodityId == items.CommodityId && t.ComdtyStockId == items.CommodityStockId);
                        if (item != null)
                        {
                            newCurrentPrice = item.OPrice - item.PreferentialPrice;
                        }
                    }
                    if (newCurrentPrice != oldCurrentPrice)
                    {
                        items.CurrentPrice = newCurrentPrice;
                        isModified = true;
                    }
                    newDuty = com.Duty ?? 0;
                    if (newDuty != oldDuty)
                    {
                        items.Duty = newDuty;
                        isModified = true;
                    }

                    if (items.PromotionType == (int)ComPromotionStatusEnum.VipIntensity)
                    {
                        newIntensity = items.Intensity.Value;
                        newDiscountPrice = items.DiscountPrice.Value;
                        newPromotionType = items.PromotionType;
                    }
                    else
                    {
                        var promotion = compromoList.FirstOrDefault(c => c.CommodityId == items.CommodityId);
                        if (promotion != null)
                        {
                            newIntensity = promotion.Intensity;
                            newDiscountPrice = promotion.DiscountPrice.Value;
                            newPromotionType = promotion.PromotionType;
                            if (newDiscountPrice == 0)
                            {
                                newDiscountPrice = (decimal)items.DiscountPrice;
                            }
                        }
                    }
                    if (newIntensity != oldIntensity)
                    {
                        items.Intensity = newIntensity;
                        isModified = true;
                    }
                    if (newDiscountPrice != oldDiscountPrice)
                    {
                        items.DiscountPrice = newDiscountPrice;
                        isModified = true;
                    }
                    if (newPromotionType != oldPromotionType)
                    {
                        items.PromotionType = newPromotionType;
                        items.PromotionDesc = Promotion.GetPromotionTypeDesc(newPromotionType ?? -1);
                        isModified = true;
                    }

                    decimal newRealPrice = (newDiscountPrice > -1)
                                        ? newDiscountPrice
                                        : decimal.Round((newCurrentPrice * newIntensity / 10), 2, MidpointRounding.AwayFromZero);
                    //newRealPrice = newUnitPrice * items.Number;
                    if (newRealPrice != oldRealPrice)
                    {
                        items.RealPrice = newRealPrice;
                        isModified = true;
                    }
                    //商品成交价格与当前商品购买价格不一致,需判断价格是否有效
                    if (isModified)
                    {
                        items.ModifiedOn = now;
                        items.EntityState = System.Data.EntityState.Modified;
                    }
                    //decimal itemAmount = ;
                    totalPrice += newRealPrice * items.Number;

                    totalDuty += newDuty * items.Number;
                    templateCounts.Add(new TemplateCountDTO() { CommodityId = items.CommodityId, Count = items.Number, Price = newRealPrice });
                }
                if (ResultCode == 1)
                {
                    return new NewResultDTO { ResultCode = 1, Message = message };
                }
                //实际优惠的金额。
                decimal tc = 0;
                //积分抵现金额
                decimal ts = 0;
                // 易捷币抵现金额
                decimal yjbReduction = 0;
                // 易捷抵现卷抵现金额
                decimal yjCouponReduction = 0;

                var orderPayDetails = OrderPayDetail.ObjectSet().Where(c => c.OrderId == commodityOrder.Id).ToList();

                var coupons = orderPayDetails.Where(c => c.ObjectType == 1).ToList();
                if (coupons != null && coupons.Any())
                {

                    //优惠券总金额。
                    decimal cTotalAmount = (from c in coupons select c.Amount).Sum();

                    //优惠券金额大于订单总金额，优惠券全抵。
                    if (cTotalAmount > totalPrice)
                    {
                        tc = totalPrice;
                    }
                    else
                    {
                        tc = cTotalAmount;
                    }

                    foreach (var coupon in coupons)
                    {
                        coupon.Amount = tc;
                    }
                }
                var scores = orderPayDetails.Where(c => c.ObjectType == 2).ToList();
                if (scores != null && scores.Any())
                {
                    //积分抵现金额。
                    ts = (from c in scores select c.Amount).Sum();
                }
                var yjInfo = YJBSV.GetOrderInfo(commodityOrder.EsAppId, commodityOrder.Id);
                if (yjInfo.IsSuccess)
                {
                    if (yjInfo.Data.YJBInfo != null) yjbReduction = yjInfo.Data.YJBInfo.InsteadCashAmount;
                    if (yjInfo.Data.YJCouponInfo != null) yjCouponReduction = yjInfo.Data.YJCouponInfo.InsteadCashAmount;
                }

                //减免综合
                reduction = tc + ts + yjbReduction + yjCouponReduction;

                FreighMultiAppResultDTO fr = new FreighMultiAppResultDTO();
                if (commodityOrder.OrderType != 3)
                {
                    if (commodityOrder.SelfTakeFlag == 0)
                    {
                        fr = new CommoditySV().CalFreightMultiAppsByTextExt(commodityOrder.Province, commodityOrder.SelfTakeFlag, templateCounts, null, null, null);
                        if (fr.ResultCode != 0)
                        {
                            ResultCode = 1;
                            message = fr.Message;
                        }
                        if (ResultCode == 1)
                        {
                            return new NewResultDTO { ResultCode = 1, Message = message };
                        }
                    }
                }

                decimal oldFreight = commodityOrder.Freight;
                decimal oldTotalDuty = commodityOrder.Duty;
                var newOrderRealPrice = totalPrice + fr.Freight - reduction + totalDuty;
                var result = new NewResultDTO { ResultCode = 0, Message = newOrderRealPrice.ToString(), Freight = fr.Freight, ExpireTime = expireTime, CouponAmount = tc, Duty = totalDuty };
                if (oldFreight != fr.Freight || commodityOrder.RealPrice != newOrderRealPrice || oldTotalDuty != totalDuty)
                {
                    //修改订单
                    commodityOrder.Freight = fr.Freight;
                    commodityOrder.Price = totalPrice;
                    commodityOrder.RealPrice = newOrderRealPrice;
                    commodityOrder.Duty = totalDuty;
                    //commodityOrder.IsModifiedPrice = true;
                    commodityOrder.ModifiedOn = now;
                    commodityOrder.EntityState = System.Data.EntityState.Modified;
                    //contextSession.SaveObject(commodityOrder);


                    //订单日志
                    Journal journal = new Journal();
                    journal.Id = Guid.NewGuid();
                    journal.Name = "用户修改订单实收款";
                    journal.Code = commodityOrder.Code;
                    journal.SubId = userId;
                    journal.SubTime = now;
                    journal.Details = "";
                    if (oldtotalPrice != totalPrice)
                    {
                        journal.Details += "订单实收款由" + oldtotalPrice + "变为" + totalPrice;
                    }
                    if (oldFreight != fr.Freight)
                    {
                        journal.Details += "订单运费由" + oldFreight + "变为" + fr.Freight;
                    }
                    if (oldTotalDuty != totalDuty)
                    {
                        journal.Details += "订单关税由" + oldTotalDuty + "变为" + totalDuty;
                    }
                    journal.CommodityOrderId = commodityOrderId;
                    journal.EntityState = System.Data.EntityState.Added;
                    contextSession.SaveObject(journal);

                    result.ResultCode = 2;

                }

                contextSession.SaveChanges();

                return result;

            }
            catch (Exception ex)
            {

                LogHelper.Error("确认支付前价格确认服务异常。", ex);
                return new NewResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(commodityOrderId);
            }
        }
        /// <summary>
        /// 重新确认订单活动家
        /// </summary>
        /// <param name="compromoList"></param>
        /// <param name="orderitemlist"></param>
        /// <param name="commodityOrder"></param>
        /// <returns></returns>
        private List<TodayPromotion> updateOrderPromotion(List<TodayPromotion> compromoList, List<OrderItem> orderitemlist, CommodityOrder commodityOrder)
        {
            List<TodayPromotion> needRefreshCacheTodayPromotions = new List<TodayPromotion>();
            List<TodayPromotion> result = new List<TodayPromotion>();
            if (orderitemlist == null || !orderitemlist.Any() || compromoList == null || !orderitemlist.Any() || commodityOrder == null)
                return result;
            try
            {
                var comproDict = compromoList.GroupBy(c => c.CommodityId)
                                             .ToDictionary(x => x.Key, y => y.OrderByDescending(c => c.PromotionType).First());
                //如果已经参加会员折扣就不改价了
                var comNumDict = orderitemlist.GroupBy(c => c.CommodityId).ToDictionary(x => x.Key, y => y.Sum(c => c.Number));
                OrderResultDTO orderResultDTO;
                List<Guid> proCommodityIdList = compromoList.Select(c => c.CommodityId).Distinct().ToList();
                DateTime now = DateTime.Now;
                if (!preCheckPromotion(commodityOrder.UserId, comproDict, comNumDict, out orderResultDTO))
                {
                    return result;
                }

                //活动、商品、销量
                List<Tuple<string, string, int>> proComTuples = new List<Tuple<string, string, int>>();
                foreach (var compro in comproDict)
                {
                    proComTuples.Add(new Tuple<string, string, int>(compro.Value.PromotionId.ToString(), compro.Key.ToString(),
                                                                    comNumDict[compro.Key]));
                }
                var proComBuyTuples = RedisHelper.ListHIncr(proComTuples, commodityOrder.UserId);
                if (proComBuyTuples == null || !proComBuyTuples.Any() || proComBuyTuples.Count != proComTuples.Count)
                {
                    return result;
                }
                foreach (var compro in comproDict)
                {
                    var tuple =
                        proComBuyTuples.First(
                            c => c.Item2 == compro.Key.ToString() && c.Item1 == compro.Value.PromotionId.ToString());
                    //获得真实资源：提交该订单后的卖出数量-本单该商品数量
                    compro.Value.SurplusLimitBuyTotal = Convert.ToInt32(tuple.Item3);

                    if (compro.Value.LimitBuyTotal > 0 && compro.Value.LimitBuyTotal - compro.Value.SurplusLimitBuyTotal < 0)
                    {
                        rollbackPromotion(proComTuples, commodityOrder.UserId);

                        return result;
                    }
                }

                if (!checkPromotion(commodityOrder.UserId, comproDict, comNumDict, out orderResultDTO))
                {
                    rollbackPromotion(proComTuples, commodityOrder.UserId);

                    return result;
                }
                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                var promotionIdList = compromoList.Select(c => c.PromotionId).Distinct().ToList();

                var promotionItemsList = PromotionItems.ObjectSet()
                          .Where(
                              t => promotionIdList.Contains(t.PromotionId) &&
                               proCommodityIdList.Contains(t.CommodityId))
                          .ToList();

                foreach (var todayp in compromoList)
                {
                    Guid comId = todayp.CommodityId;

                    needRefreshCacheTodayPromotions.Add(todayp);
                    todayp.EntityState = System.Data.EntityState.Modified;

                    PromotionItems pro = (from pr in promotionItemsList
                                          where pr.PromotionId == todayp.PromotionId && pr.CommodityId == todayp.CommodityId
                                          select pr).FirstOrDefault();
                    pro.EntityState = System.Data.EntityState.Modified;
                    pro.SurplusLimitBuyTotal = todayp.SurplusLimitBuyTotal;
                    UserLimited ul = new UserLimited
                    {
                        Id = Guid.NewGuid(),
                        UserId = commodityOrder.UserId,
                        PromotionId = todayp.PromotionId,
                        CommodityId = todayp.CommodityId,
                        Count = comNumDict[comId],
                        CreateTime = now,
                        CommodityOrderId = commodityOrder.Id,
                        EntityState = System.Data.EntityState.Added
                    };
                    contextSession.SaveObject(ul);

                }
                #region 处理未支付订单超时
                var firstOutPrmotion = comproDict.Values.FirstOrDefault(c => c.PromotionType != 0);
                if (firstOutPrmotion != null)
                {
                    var expireSeconds = PromotionSV.GetExpirePaySeconds((Guid)commodityOrder.EsAppId);
                    OrderExpirePay orderExpirePay = OrderExpirePay.CreateOrderExpirePay();
                    orderExpirePay.OrderId = commodityOrder.Id;
                    orderExpirePay.PromotionId = firstOutPrmotion.PromotionId;
                    orderExpirePay.State = 0;
                    orderExpirePay.ExpirePayTime = now.AddSeconds(expireSeconds);
                    contextSession.SaveObject(orderExpirePay);
                }
                #endregion

                contextSession.SaveChanges();
                needRefreshCacheTodayPromotions.ForEach(c => c.RefreshCache(EntityState.Modified));
                return compromoList;
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// 定时处理订单
        /// </summary>
        public void AutoDealOrderExt()
        {
            LogHelper.Info(string.Format("自动确认支付服务开始"));
            //处理订单状态为确认收货
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                DateTime now = DateTime.Now;
                //查询超过12天未确认收货的订单
                //DateTime lastday = now.AddDays(-9);
                //DateTime threelastday = now.AddDays(-12);
                double lastday = 9 * 24;
                double threelastday = 12 * 24;
                var orders = CommodityOrder.ObjectSet().Where(n => n.State == 2).ToList();
                LogHelper.Info(string.Format("自动确认支付服务处理订单数:{0}", orders.Count));
                List<int> secTranPayments = new PaySourceSV().GetSecuriedTransactionPaymentExt();
                if (orders.Count > 0)
                {
                    ContextDTO contextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();

                    foreach (CommodityOrder order in orders)
                    {
                        int oldState = order.State;
                        ContextFactory.ReleaseContextSession();
                        double middleSalesTimeOut = 0;
                        //获取退款信息
                        var refundInformation = OrderRefund.ObjectSet().Where(n => n.OrderId == order.Id).OrderByDescending(p => p.ModifiedOn);
                        //存在售后退款
                        if (refundInformation.Any())
                        {
                            double allRequestTime = 0;
                            //取最后一次发起的售后信息为当前用户售后处理结果
                            var firstRecode = refundInformation.FirstOrDefault();
                            if (firstRecode.State == 2 || firstRecode.State == 4 || firstRecode.State == 13)
                            {
                                foreach (OrderRefund orderRefund in refundInformation)
                                {
                                    if (orderRefund.RefuseTime.HasValue)
                                    {
                                        //获取申请拒绝时间和申请未收到货时间之和
                                        allRequestTime +=
                                            orderRefund.RefuseTime.Value.Subtract(orderRefund.SubTime).TotalHours;
                                    }
                                    if (orderRefund.NotReceiveTime.HasValue)
                                    {
                                        //申请未收到货时间之和
                                        allRequestTime +=
                                            orderRefund.NotReceiveTime.Value.Subtract(orderRefund.SubTime).TotalHours;
                                    }
                                }
                            }
                            if (order.ShipmentsTime != null)
                                middleSalesTimeOut = now.Subtract((DateTime)order.ShipmentsTime).TotalHours - allRequestTime;
                        }
                        else
                        {
                            if (order.ShipmentsTime != null)
                                middleSalesTimeOut = now.Subtract((DateTime)order.ShipmentsTime).TotalHours;
                        }
                        double confirmTime = (order.IsDelayConfirmTime ? threelastday : lastday);

                        //售中9天确认收货，延长3天后，12天确认收货
                        if (middleSalesTimeOut >= confirmTime)
                        {
                            //金币支付，
                            if (secTranPayments.Contains(order.Payment))
                            {
                                //AuthorizeHelper.InitAuthorizeInfo();

                                ConfirmPayDTO comConfirmPayDto = null;
                                //金币支付
                                try
                                {
                                    Jinher.AMP.FSP.Deploy.CustomDTO.ReturnInfoDTO goldPayresult =
                                        new Jinher.AMP.FSP.Deploy.CustomDTO.ReturnInfoDTO();

                                    List<object> saveList = new List<object>();

                                    Jinher.AMP.App.Deploy.CustomDTO.AppIdOwnerIdTypeDTO applicationDTO =
                                        APPSV.Instance.GetAppOwnerInfo(order.AppId, contextDTO);
                                    if (applicationDTO == null || applicationDTO.OwnerId == Guid.Empty)
                                        continue;

                                    if (order.RealPrice > 0)
                                    {
                                        if (order.OrderType != 1)
                                        {
                                            var confirmPayDTO = OrderSV.BuildConfirmPayDTO(contextSession, order, out saveList, applicationDTO, isSaveObject: false);
                                            LogHelper.Info(string.Format("订单确认收货:AutoDealOrderExt goldPayFacade.ConfirmPay：订单id={0} , 支付DTO ={1}", order.Id, JsonHelper.JsonSerializer(confirmPayDTO)), "BTP_Order");

                                            //冻结账户资金
                                            goldPayresult = Jinher.AMP.BTP.TPS.FSPSV.Instance.ConfirmPayFreeze(confirmPayDTO);
                                        }
                                        else
                                        {
                                            var confirmPayDTO = OrderSV.BuildConfirmPayDTO(contextSession, order, out saveList,
                                                                                  applicationDTO, isSaveObject: false);
                                            LogHelper.Info(
                                                string.Format(
                                                    "订单确认收货:AutoDealOrderExt goldPayFacade.ConfirmPay：订单id={0} , 支付DTO ={1}",
                                                    order.Id, JsonHelper.JsonSerializer(confirmPayDTO)), "BTP_Order");

                                            //确认付款
                                            goldPayresult = Jinher.AMP.BTP.TPS.FSPSV.Instance.ConfirmPay(confirmPayDTO);
                                        }
                                    }

                                    if (goldPayresult.Code != 0 && goldPayresult.Code != -8)
                                    {
                                        LogHelper.Error(string.Format("9天自动收货，确认支付金币接口支付失败,OrderId:{0},OrderCode:{1}。code：{2}，Message：{3}", order.Id, order.Code, goldPayresult.Code, goldPayresult.Message));
                                        continue; //金币支付失败
                                    }
                                    else
                                    {
                                        if (goldPayresult.Code == -8)
                                        {
                                            LogHelper.Error(string.Format("9天自动收货，确认支付金币接口支付失败,OrderId:{0},OrderCode:{1}。code：{2}，Message：{3}", order.Id, order.Code, goldPayresult.Code, goldPayresult.Message));
                                        }
                                        if (saveList != null && saveList.Any())
                                        {
                                            foreach (var o in saveList)
                                            {
                                                contextSession.SaveObject(o);
                                            }
                                        }
                                    }

                                }
                                catch (Exception ex)
                                {

                                    LogHelper.Error(
                                        string.Format("自动确认支付服务异常。入参:{0}", JsonHelper.JsonSerializer(comConfirmPayDto)),
                                        ex);
                                    continue; //金币支付失败
                                }
                            }



                            ////更新销量
                            //var orderitemlist = OrderItem.ObjectSet().Where(n => n.CommodityOrderId == order.Id);
                            //foreach (OrderItem items in orderitemlist)
                            //{
                            //    Commodity com = Commodity.ObjectSet().Where(n => n.Id == items.CommodityId).FirstOrDefault();
                            //    com.EntityState = System.Data.EntityState.Modified;
                            //    //加销量
                            //    com.Salesvolume += items.Number;
                            //    contextSession.SaveObject(com);
                            //}

                            //更新订单状态
                            order.State = 3;
                            order.ConfirmTime = now;
                            order.ModifiedOn = now;
                            order.EntityState = System.Data.EntityState.Modified;

                            // 更新结算单确认收货时间
                            SettleAccountHelper.ConfirmOrder(contextSession, order);

                            if (order.Payment != 1 && order.OrderType != 1)
                            {
                                CommodityOrderService commodityOrderService = new CommodityOrderService();
                                commodityOrderService.Id = order.Id;
                                commodityOrderService.Name = order.Name;
                                commodityOrderService.Code = order.Code;
                                commodityOrderService.State = order.State;
                                commodityOrderService.SubId = order.SubId;
                                commodityOrderService.SelfTakeFlag = order.SelfTakeFlag;
                                commodityOrderService.EntityState = System.Data.EntityState.Added;
                                contextSession.SaveObject(commodityOrderService);
                            }

                            #region CPS通知

                            //CPS通知
                            if ((order.SrcType == 33 || order.SrcType == 34 || order.SrcType == 36 ||
                                 order.SrcType == 39 ||
                                 order.SrcType == 40) && !string.IsNullOrEmpty(order.CPSId) && order.CPSId != "null")
                            {
                                CPSCallBack(order, contextDTO);
                            }

                            #endregion

                            //发送消息，异步执行
                            //System.Threading.ThreadPool.QueueUserWorkItem(
                            //    a =>
                            //    {
                            //订单日志
                            Journal journal = new Journal();
                            journal.Id = Guid.NewGuid();
                            journal.Name = "系统9天后自动确认收货";
                            journal.Code = order.Code;
                            journal.SubId = order.UserId;
                            journal.SubTime = now;
                            journal.Details = "订单状态由" + oldState + "变为" + order.State;
                            journal.StateFrom = oldState;
                            journal.StateTo = order.State;
                            journal.IsPush = false;
                            journal.OrderType = order.OrderType;
                            journal.CommodityOrderId = order.Id;

                            journal.EntityState = System.Data.EntityState.Added;
                            contextSession.SaveObject(journal);
                            //contextSession.SaveChanges();

                            //添加消息
                            AddMessage addmassage = new AddMessage();
                            string odid = order.Id.ToString();
                            string usid = order.UserId.ToString();
                            string type = "order";
                            Guid EsAppId = order.EsAppId.HasValue ? order.EsAppId.Value : order.AppId;
                            addmassage.AddMessages(odid, usid, EsAppId, order.Code, order.State, "", type);
                            ////正品会发送消息
                            //if (new ZPHSV().CheckIsAppInZPH(order.AppId))
                            //{
                            //    addmassage.AddMessages(odid, usid, CustomConfig.ZPHAppId, order.Code, order.State, "",
                            //                           type);
                            //}
                            //});
                        }
                    }
                }
                DateTime lastHour = now.AddHours(-25);//now.AddMinutes(-10);//now.AddHours(-25);
                var payingorders = CommodityOrder.ObjectSet().Where(n => n.State == 11 && n.PaymentTime <= lastHour).ToList();

                LogHelper.Info(string.Format("自动处理付款中为待付款的订单数:{0}", payingorders.Count));
                foreach (CommodityOrder payingorder in payingorders)
                {
                    payingorder.State = 0;
                    payingorder.EntityState = EntityState.Modified;
                    //添加消息
                    AddMessage addmassage = new AddMessage();
                    string odid = payingorder.Id.ToString();
                    string usid = payingorder.UserId.ToString();
                    string type = "payingorder";
                    Guid EsAppId = payingorder.EsAppId.HasValue ? payingorder.EsAppId.Value : payingorder.AppId;
                    addmassage.AddMessages(odid, usid, EsAppId, payingorder.Code, payingorder.State, "", type);
                    ////正品会发送消息
                    //if (new ZPHSV().CheckIsAppInZPH(payingorder.AppId))
                    //{
                    //    addmassage.AddMessages(odid, usid, CustomConfig.ZPHAppId, payingorder.Code, payingorder.State, "", type);
                    //}
                }
                if (contextSession.SaveChanges() > 0)
                {
                    orders.ForEach(order =>
                    {
                        YXOrderHelper.ConfirmReceivedOrder(order.Id);
                    });
                }
            }
            catch (Exception ex)
            {

                LogHelper.Error("自动确认支付服务异常。", ex);
            }

        }

        /// <summary>
        /// 队列提交订单
        /// </summary>
        /// <param name="orderSDTO"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.OrderResultDTO SubmitOrderExt(Jinher.AMP.BTP.Deploy.CustomDTO.OrderQueueDTO orderSDTO)
        {

            try
            {
                DateTime now = DateTime.Now;
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                //遍历购买商品列表
                if (orderSDTO.ShoppingCartItemSDTO != null)
                {
                    foreach (var item in orderSDTO.ShoppingCartItemSDTO)
                    {

                        //删除购物车
                        if (!string.IsNullOrEmpty(item.ShopCartItemId.ToString()))
                        {
                            Guid ShopCartItemId = new Guid(item.ShopCartItemId.ToString());
                            ShoppingCartItems shop = new ShoppingCartItems();
                            shop = ShoppingCartItems.ObjectSet().Where(n => n.Id == ShopCartItemId).FirstOrDefault();
                            if (shop != null)
                            {
                                shop.EntityState = System.Data.EntityState.Deleted;
                                contextSession.Delete(shop);
                            }
                        }
                        //LogHelper.Error("DiscountPrice:" + item.DiscountPrice);
                        //添加订单商品
                        OrderItem orderitem = new OrderItem
                        {
                            Number = item.CommodityNumber,
                            Code = orderSDTO.Code,
                            Name = item.Name,
                            CommodityAttributes = item.SizeAndColorId,
                            CommodityId = item.Id,
                            PicturesPath = item.Pic,

                            PromotionId = item.PromotionId,
                            Id = Guid.NewGuid(),
                            CommodityOrderId = orderSDTO.OrderId,
                            SubTime = now,
                            CurrentPrice = item.Price,
                            SubId = item.UserId,
                            Intensity = item.Intensity,
                            DiscountPrice = item.DiscountPrice,

                            RealPrice = item.DiscountPrice > -1 ? item.DiscountPrice : Math.Round((item.Price * item.Intensity / 10), 2, MidpointRounding.AwayFromZero)
                        };


                        orderitem.AlreadyReview = false;
                        orderitem.EntityState = System.Data.EntityState.Added;
                        contextSession.SaveObject(orderitem);

                    }
                }

                #region 订单
                CommodityOrder commodityOrderDTO = new CommodityOrder();

                commodityOrderDTO.Id = orderSDTO.OrderId;
                commodityOrderDTO.Name = "用户订单";
                commodityOrderDTO.Code = orderSDTO.Code;
                commodityOrderDTO.SubId = orderSDTO.UserId;
                commodityOrderDTO.UserId = orderSDTO.UserId;
                commodityOrderDTO.SubTime = now;
                commodityOrderDTO.State = 0;
                commodityOrderDTO.Payment = orderSDTO.Payment;
                commodityOrderDTO.RealPrice = orderSDTO.Price;//付款总价
                commodityOrderDTO.Price = orderSDTO.Price; //订单总价
                commodityOrderDTO.ReceiptAddress = orderSDTO.ReceiptAddress;
                commodityOrderDTO.ReceiptPhone = orderSDTO.ReceiptPhone;
                commodityOrderDTO.ReceiptUserName = orderSDTO.ReceiptUserName;
                commodityOrderDTO.City = orderSDTO.City;
                commodityOrderDTO.Province = orderSDTO.Province;
                commodityOrderDTO.District = orderSDTO.District;
                commodityOrderDTO.Street = orderSDTO.Street;
                commodityOrderDTO.AppId = orderSDTO.AppId;
                commodityOrderDTO.Details = orderSDTO.Details;
                commodityOrderDTO.IsModifiedPrice = false;
                commodityOrderDTO.ModifiedOn = now;
                commodityOrderDTO.RecipientsZipCode = orderSDTO.RecipientsZipCode;
                commodityOrderDTO.EntityState = System.Data.EntityState.Added;
                commodityOrderDTO.SrcType = orderSDTO.SrcType;
                commodityOrderDTO.SrcTagId = orderSDTO.SrcTagId;
                commodityOrderDTO.CPSId = orderSDTO.CPSId;
                contextSession.SaveObject(commodityOrderDTO);
                #endregion

                #region 地址
                int address = DeliveryAddress.ObjectSet()
                    .Where(n => n.UserId == orderSDTO.UserId && n.RecipientsAddress == commodityOrderDTO.ReceiptAddress)
                    .Count();
                //地址不存在，则添加新地址
                if (address == 0)
                {
                    DeliveryAddress deliveryAddressDTO = new DeliveryAddress();
                    deliveryAddressDTO.Id = Guid.NewGuid();
                    deliveryAddressDTO.Code = commodityOrderDTO.Code;
                    deliveryAddressDTO.Name = "用户地址";
                    deliveryAddressDTO.RecipientsAddress = orderSDTO.ReceiptAddress;
                    deliveryAddressDTO.RecipientsPhone = orderSDTO.ReceiptPhone;
                    deliveryAddressDTO.RecipientsUserName = orderSDTO.ReceiptUserName;
                    deliveryAddressDTO.City = orderSDTO.City;
                    deliveryAddressDTO.Province = orderSDTO.Province;
                    deliveryAddressDTO.District = orderSDTO.District;
                    deliveryAddressDTO.Street = orderSDTO.Street;
                    deliveryAddressDTO.UserId = orderSDTO.UserId;
                    deliveryAddressDTO.AppId = orderSDTO.AppId;
                    deliveryAddressDTO.SubTime = now;
                    deliveryAddressDTO.RecipientsZipCode = orderSDTO.RecipientsZipCode;
                    deliveryAddressDTO.EntityState = System.Data.EntityState.Added;
                    contextSession.SaveObject(deliveryAddressDTO);
                }
                #endregion


                //订单日志
                Journal journal = new Journal();
                journal.Id = Guid.NewGuid();
                journal.Name = "用户创建订单";
                journal.Code = orderSDTO.Code;
                journal.SubId = orderSDTO.UserId;
                journal.SubTime = now;
                journal.Details = "订单状态为" + commodityOrderDTO.State;
                //journal.StateFrom = ;                   
                journal.StateTo = commodityOrderDTO.State;
                journal.IsPush = false;
                journal.OrderType = commodityOrderDTO.OrderType;
                journal.CommodityOrderId = orderSDTO.OrderId;
                journal.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(journal);
                contextSession.SaveChanges();

                return new OrderResultDTO { ResultCode = 0, Message = "Success" };
            }
            catch (Exception ex)
            {

                LogHelper.Error(string.Format("添加订单服务异常。orderSDTO：{0}", orderSDTO), ex);
                return new OrderResultDTO { ResultCode = 1, Message = "Error" };
            }
        }

        public List<LotteryOrderInfoDTO> GetLotteryOrdersExt(Guid lotteryId)
        {
            if (lotteryId == Guid.Empty)
            {
                throw new ArgumentNullException("好运来活动Id为空异常");
            }
            try
            {



                //获取所有订单
                var commodityorderList = (from c in CommodityOrder.ObjectSet()
                                          join o in OrderItem.ObjectSet() on c.Id equals o.CommodityOrderId
                                          where c.SrcTagId == lotteryId && c.State > 0 && c.State != 4 && c.State != 16 && c.State != 17
                                          orderby c.SubTime descending
                                          select new LotteryOrderInfoDTO()
                                          {
                                              City = c.City,
                                              Code = c.Code,
                                              CommodityOrderId = c.Id,
                                              District = c.District,
                                              Province = c.Province,
                                              Street = c.Street,
                                              ReceiptAddress = c.ReceiptAddress,
                                              ReceiptPhone = c.ReceiptPhone,
                                              ReceiptUserName = c.ReceiptUserName,
                                              RecipientsZipCode = c.RecipientsZipCode,
                                              SubTime = c.SubTime,
                                              UserId = c.UserId,
                                              CommodityId = o.CommodityId,
                                              CommodityName = o.Name
                                          })
                    .ToList();

                return commodityorderList;
            }
            catch (Exception ex)
            {

                LogHelper.Error(string.Format("获取用户所有订单。lotteryId：{0}", lotteryId), ex);

                return new List<LotteryOrderInfoDTO>();
            }
        }

        public ResultDTO GetOrderStateByCodeExt(string orderCode)
        {
            ResultDTO result = new ResultDTO();
            if (string.IsNullOrEmpty(orderCode))
            {
                result.ResultCode = -1;
                result.Message = "订单号为空";
                return result;
            }
            var state = (from c in CommodityOrder.ObjectSet()

                         where c.Code == orderCode && c.State != 16 && c.State != 17
                         select c.State
                         ).FirstOrDefault();

            result.ResultCode = state;

            return result;
        }

        /// <summary>
        /// 通过emb服务器提交订单
        /// </summary>
        /// <param name="orderQueueDTO"></param>
        private static void SendOrderSubmitMsg(OrderQueueDTO orderQueueDTO)
        {
            Jinher.JAP.EMB.MB.PublishMessage eventMsg = new JAP.EMB.MB.PublishMessage();
            eventMsg.TopicURI = QueueBTP.EMBPublish.topicUri;
            eventMsg.SendData = orderQueueDTO;

            QueueBTP.EMBPublish.PublishMessage(eventMsg);
        }

        /// <summary>
        /// 单商品申请退款
        /// </summary>
        /// <param name="submitOrderRefundDTO"></param>
        /// <returns></returns>
        private ResultDTO SubmitOrderItemRefund(SubmitOrderRefundDTO submitOrderRefundDTO)
        {
            LogHelper.Debug("开始进入单商品申请退款方法SubmitOrderItemRefund，参数为submitOrderRefundDTO：" + JsonHelper.JsSerializer(submitOrderRefundDTO));
            //退积分金额
            decimal spendScoreMoney = 0;
            // 退易捷币
            decimal spendYJBMoney = 0;
            decimal spendAllYJBMoney = 0;
            //原订单状态
            int oldState = 0;
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var commodityOrder = CommodityOrder.ObjectSet().FirstOrDefault(n => n.Id == submitOrderRefundDTO.commodityorderId);
                if (commodityOrder == null)
                {
                    return new ResultDTO { ResultCode = 4, Message = "订单不存在" };
                }

                // 京东商品未发货禁止退款
                if (commodityOrder.State == 1 && JdJobHelper.JDAppIdList.Contains(commodityOrder.AppId))
                {
                    return new ResultDTO { ResultCode = 1, Message = "商品已出库，不能申请退款，可拒收~" };
                }

                if (ThirdECommerceHelper.IsWangYiYanXuan(commodityOrder.AppId) && commodityOrder.State == 1)
                {
                    return new ResultDTO { ResultCode = 1, Message = "未发货订单不支持单品退款~" };
                }
                if (commodityOrder.State == 11)
                {
                    return new ResultDTO { ResultCode = 5, Message = "钱尚未到达商家账户，处理中，请稍后重试~" };
                }
                //判断订单状态是否可以进行 退款操作
                if (commodityOrder.State != 1 && commodityOrder.State != 2 && commodityOrder.State != 13)
                {
                    return new ResultDTO { ResultCode = 2, Message = "订单状态无法申请退款" };
                }
                //判断订单状态是否 发生改变
                if (submitOrderRefundDTO.State != commodityOrder.State)
                {
                    return new ResultDTO { ResultCode = 3, Message = "订单状态已经改变" };
                }
                var orderItem = OrderItem.FindByID(submitOrderRefundDTO.OrderItemId);
                //退款金额
                //最大退款金额                   
                decimal maxMoney = ((decimal)orderItem.RealPrice * orderItem.Number) + orderItem.Duty;

                var m = (decimal)orderItem.RealPrice * orderItem.Number;
                if (m == 0)
                {
                    maxMoney = (decimal)(orderItem.DiscountPrice * orderItem.Number);
                }

                // 只有未发货时才可以退运费
                if (commodityOrder.State == 1 && orderItem.FreightPrice != null)
                {
                    maxMoney = maxMoney + (decimal)orderItem.FreightPrice;
                }
                if (orderItem.CouponPrice != null)
                {
                    maxMoney = maxMoney - (decimal)orderItem.CouponPrice;
                }
                if (orderItem.ChangeFreightPrice != null)
                {
                    maxMoney = maxMoney - (decimal)orderItem.ChangeFreightPrice;
                }
                if (orderItem.ChangeRealPrice != null)
                {
                    maxMoney = maxMoney - (decimal)orderItem.ChangeRealPrice;
                }
                if (submitOrderRefundDTO.RefundMoney > maxMoney)
                {
                    return new ResultDTO { ResultCode = 1, Message = "退款金额不能大于订单金额" };
                }
                // -1 表示全部退款
                if (submitOrderRefundDTO.RefundMoney == -1)
                {
                    submitOrderRefundDTO.RefundMoney = maxMoney;
                }
                OrderRefund orderRefund = new OrderRefund();
                orderRefund.Id = Guid.NewGuid();
                orderRefund.RefundReason = submitOrderRefundDTO.RefundReason;

                //订单项使用易捷抵用券抵用金额。
                decimal couponprice = 0;
                var useryjcoupon = YJBSV.GetUserYJCouponItemByOrderId(orderItem.Id);
                if (useryjcoupon.Data != null)
                {
                    foreach (var item in useryjcoupon.Data)
                    {
                        if (item != null)
                        {
                            couponprice += item.UseAmount;
                        }
                    }
                }

                #region

                //if (submitOrderRefundDTO.RefundMoney >= orderItem.RealPrice * orderItem.Number)
                //{//全额退
                //    if (commodityOrder.State == 1 || commodityOrder.State == 8)
                //    {//待发货状态，不可修改退款金额，直接传多少就是多少
                //        orderRefund.OrderRefundMoneyAndCoupun = submitOrderRefundDTO.RefundMoney + (orderItem.FreightPrice ?? 0);
                //        orderRefund.RefundMoney = (decimal)(orderItem.RealPrice * orderItem.Number) + (orderItem.FreightPrice ?? 0) - couponprice;

                //        orderRefund.RefundFreightPrice = (orderItem.FreightPrice ?? 0);
                //    }
                //    else
                //    {
                //        orderRefund.OrderRefundMoneyAndCoupun = (decimal)(orderItem.RealPrice * orderItem.Number);
                //        orderRefund.RefundMoney = (decimal)(orderItem.RealPrice * orderItem.Number) - couponprice;
                //    }
                //    orderRefund.RefundScoreMoney = submitOrderRefundDTO.RefundMoney - couponprice - (decimal)(orderItem.RealPrice * orderItem.Number) - spendYJBMoney;
                //    // 退易捷币金额
                //    orderRefund.RefundYJBMoney = spendYJBMoney;
                //}
                //else
                //{//部分退
                //    orderRefund.OrderRefundMoneyAndCoupun = submitOrderRefundDTO.RefundMoney;
                //    if (submitOrderRefundDTO.RefundMoney > orderItem.RealPrice.Value * orderItem.Number)
                //    {
                //        orderRefund.RefundMoney = orderItem.RealPrice.Value * orderItem.Number;
                //        orderItem.YJCouponPrice = submitOrderRefundDTO.RefundMoney - orderItem.RealPrice.Value * orderItem.Number;
                //    }
                //    else
                //    {
                //        orderRefund.RefundMoney = submitOrderRefundDTO.RefundMoney;
                //        orderItem.YJCouponPrice = 0;
                //    }
                //    orderRefund.RefundScoreMoney = 0;
                //}

                #endregion

                //计算实付金额
                decimal yjbprice = 0;
                var yjbresult = YJBSV.GetOrderItemYJBInfo(commodityOrder.EsAppId.Value, commodityOrder.Id);
                if (yjbresult.Data != null)
                {
                    yjbprice = yjbresult.Data.Items != null ? yjbresult.Data.Items[0].InsteadCashAmount : 0;
                }
                var CurrPic1 = orderItem.RealPrice * orderItem.Number;
                if (CurrPic1 == 0)
                {
                    CurrPic1 = (orderItem.DiscountPrice.Value * orderItem.Number);
                }

                var yjcPrice = orderItem.YJCardPrice ?? 0;

                var cashmoney = CurrPic1 - (orderItem.CouponPrice ?? 0) - (orderItem.ChangeRealPrice ?? 0) - orderItem.Duty - couponprice - yjbprice;
                //易捷卡可以抵运费。
                //cashmoney = cashmoney + orderItem.FreightPrice - yjcPrice;

                if (commodityOrder.State == 1 || commodityOrder.State == 8)
                {//待发货，全退
                    LogHelper.Info("SubmitOrderItemRefund待发货，全退OrderRefundMoneyAndCoupun:" + submitOrderRefundDTO.RefundMoney);
                    LogHelper.Info("SubmitOrderItemRefund待发货，全退RefundMoney:" + ((cashmoney ?? 0) + (orderItem.FreightPrice ?? 0)));
                    LogHelper.Info("SubmitOrderItemRefund待发货，全退RefundYJCouponMoney:" + couponprice);
                    LogHelper.Info("SubmitOrderItemRefund待发货，全退RefundYJBMoney:" + spendYJBMoney);
                    LogHelper.Info("SubmitOrderItemRefund待发货，全退RefundYJBMoney:" + spendYJBMoney);

                    orderRefund.OrderRefundMoneyAndCoupun = submitOrderRefundDTO.RefundMoney - yjcPrice;
                    orderRefund.RefundMoney = (cashmoney ?? 0) + (orderItem.FreightPrice ?? 0);
                    //commodityOrder.YJCouponPrice = couponprice;
                    orderRefund.RefundYJCouponMoney = couponprice;
                    // 退易捷币金额
                    orderRefund.RefundYJBMoney = spendYJBMoney;
                    //orderRefund.RefundScoreMoney = submitOrderRefundDTO.RefundMoney - couponprice - orderRefund.RefundMoney - spendYJBMoney;
                    orderRefund.RefundScoreMoney = 0;
                    orderRefund.RefundYJCardMoney = yjcPrice;
                }
                else
                {//已发货
                    if (submitOrderRefundDTO.RefundMoney >= (cashmoney ?? 0) + couponprice + yjcPrice)
                    {//全额退
                        LogHelper.Info("SubmitOrderItemRefund已发货，全退OrderRefundMoneyAndCoupun:" + submitOrderRefundDTO.RefundMoney);
                        LogHelper.Info("SubmitOrderItemRefund已发货，全退RefundMoney:" + (cashmoney ?? 0));
                        LogHelper.Info("SubmitOrderItemRefund已发货，全退RefundYJCouponMoney:" + couponprice);
                        LogHelper.Info("SubmitOrderItemRefund已发货，全退RefundYJBMoney:" + spendYJBMoney);
                        orderRefund.OrderRefundMoneyAndCoupun = submitOrderRefundDTO.RefundMoney - yjcPrice;
                        orderRefund.RefundMoney = (cashmoney ?? 0);
                        //commodityOrder.YJCouponPrice = couponprice;
                        orderRefund.RefundYJCouponMoney = couponprice;
                        // 退易捷币金额
                        orderRefund.RefundYJBMoney = spendYJBMoney;
                        //orderRefund.RefundScoreMoney = submitOrderRefundDTO.RefundMoney - couponprice - (cashmoney ?? 0) - spendYJBMoney;
                        orderRefund.RefundScoreMoney = 0;
                        orderRefund.RefundYJCardMoney = yjcPrice;
                    }
                    else
                    {//部分退
                        if (submitOrderRefundDTO.RefundMoney <= (cashmoney ?? 0))
                        {//只退现金
                            LogHelper.Info("SubmitOrderItemRefund部分退，只退现金RefundMoney:" + submitOrderRefundDTO.RefundMoney);
                            LogHelper.Info("SubmitOrderItemRefund部分退，只退现金OrderRefundMoneyAndCoupun:" + submitOrderRefundDTO.RefundMoney);
                            orderRefund.RefundMoney = submitOrderRefundDTO.RefundMoney;
                            orderRefund.OrderRefundMoneyAndCoupun = submitOrderRefundDTO.RefundMoney;
                        }
                        else if (submitOrderRefundDTO.RefundMoney < (cashmoney ?? 0) + yjcPrice)
                        {
                            //退现金+部分易捷卡
                            orderRefund.RefundMoney = (cashmoney ?? 0);
                            orderRefund.RefundYJCardMoney = submitOrderRefundDTO.RefundMoney - (cashmoney ?? 0);
                        }
                        else if (submitOrderRefundDTO.RefundMoney < (cashmoney ?? 0) + yjcPrice + couponprice)
                        {
                            //退现金+易捷卡+部分抵用券
                            LogHelper.Info("SubmitOrderItemRefund部分退，退现金+部分抵用券RefundMoney:" + (cashmoney ?? 0));
                            LogHelper.Info("SubmitOrderItemRefund部分退，只退现金OrderRefundMoneyAndCoupun:" + submitOrderRefundDTO.RefundMoney);
                            LogHelper.Info("SubmitOrderItemRefund部分退，只退现金RefundYJCouponMoney:" + (orderRefund.OrderRefundMoneyAndCoupun - orderRefund.RefundMoney));
                            orderRefund.RefundMoney = (cashmoney ?? 0);
                            orderRefund.RefundYJCardMoney = yjcPrice;
                            orderRefund.OrderRefundMoneyAndCoupun = submitOrderRefundDTO.RefundMoney - yjcPrice;
                            orderRefund.RefundYJCouponMoney = submitOrderRefundDTO.RefundMoney - orderRefund.RefundMoney - orderRefund.RefundYJCardMoney;
                        }
                        orderRefund.RefundScoreMoney = 0;
                    }
                }

                orderRefund.RefundDesc = submitOrderRefundDTO.RefundDesc;
                orderRefund.OrderRefundImgs = submitOrderRefundDTO.OrderRefundImgs;
                orderRefund.OrderId = submitOrderRefundDTO.commodityorderId;
                //仅退款
                orderRefund.RefundType = submitOrderRefundDTO.RefundType;
                //退款中
                orderRefund.State = 0;
                orderRefund.DataType = "1";
                orderRefund.IsDelayConfirmTimeAfterSales = false;

                decimal rePrice = 0;
                decimal couponPrice = orderItem.CouponPrice == null ? 0 : (decimal)orderItem.CouponPrice;
                decimal freightPrice = orderItem.FreightPrice == null ? 0 : (decimal)orderItem.FreightPrice;
                decimal changeFreightPrice = orderItem.ChangeFreightPrice == null ? 0 : (decimal)orderItem.ChangeFreightPrice;
                decimal changeRealPrice = orderItem.ChangeRealPrice == null ? 0 : (decimal)orderItem.ChangeRealPrice;
                if (commodityOrder.State == 1)
                {
                    rePrice = (((decimal)orderItem.RealPrice * orderItem.Number) - couponPrice + freightPrice - changeFreightPrice - changeRealPrice + orderItem.Duty);
                }
                else if (commodityOrder.State == 2 || commodityOrder.State == 23)
                {
                    rePrice = (((decimal)orderItem.RealPrice * orderItem.Number) - couponPrice - changeRealPrice - orderItem.Duty);
                    //最大退款金额
                    var maxRePrice = (orderItem.RealPrice * orderItem.Number) - couponPrice + freightPrice - changeFreightPrice - changeRealPrice - orderItem.Duty;
                    if (maxRePrice < rePrice)
                    {
                        return new ResultDTO { ResultCode = 1, Message = "退款金额不能大于最大退款金额" };
                    }
                }
                orderRefund.IsFullRefund = rePrice == submitOrderRefundDTO.RefundMoney;
                orderRefund.OrderItemId = submitOrderRefundDTO.OrderItemId;
                orderRefund.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(orderRefund);

                //保存订单项退款状态
                orderItem.State = 1;
                orderItem.ModifiedOn = DateTime.Now;
                orderItem.EntityState = EntityState.Modified;
                contextSession.SaveObject(orderItem);

                //获取订单其他详情项 判断是否都是已退款的状态 是的话，改变订单状态；不是的话，不改变订单状态
                LogHelper.Debug(String.Format("退款orderItem.Id：" + orderItem.Id + "submitOrderRefundDTO.commodityorderId:" + submitOrderRefundDTO.commodityorderId));
                var orderItemCount = OrderItem.ObjectSet().Count(t => t.Id != orderItem.Id && t.CommodityOrderId == submitOrderRefundDTO.commodityorderId && t.State == 0);
                LogHelper.Debug(String.Format("退款orderItemCount：" + orderItemCount));
                if (orderItemCount == 0)
                {
                    oldState = commodityOrder.State;
                    switch (commodityOrder.State)
                    {
                        //如果待发货 更改为待发货 退款中
                        case 1:
                            commodityOrder.State = 8;
                            break;
                        //如果已发货 更改为已发货 退款中
                        case 2:
                            commodityOrder.State = 9;
                            break;
                        //如果已发货 更改为已发货 退款中
                        case 13:
                            commodityOrder.State = 14;
                            break;
                    }
                    commodityOrder.RefundTime = DateTime.Now;
                    commodityOrder.IsRefund = true;
                    commodityOrder.ModifiedOn = DateTime.Now;
                    commodityOrder.EntityState = System.Data.EntityState.Modified;
                    contextSession.SaveObject(commodityOrder);
                }

                var eventResult = OrderEventHelper.OnOrderItemRefund(commodityOrder, orderItem, orderRefund);
                if (!eventResult.isSuccess)
                {
                    return new ResultDTO { ResultCode = 1, Message = eventResult.Message };
                }
                int reslult = contextSession.SaveChanges();
                if (reslult > 0)
                {
                    try
                    {
                        //订单日志
                        Journal journal = new Journal();
                        journal.Id = Guid.NewGuid();
                        journal.Name = "售中申请单商品退款订单";
                        journal.Code = commodityOrder.Code;
                        journal.SubId = commodityOrder.UserId;
                        journal.SubTime = DateTime.Now;
                        journal.CommodityOrderId = commodityOrder.Id;
                        journal.CommodityOrderItemId = submitOrderRefundDTO.OrderItemId;
                        journal.IsPush = false;
                        journal.OrderType = commodityOrder.OrderType;
                        if (orderItemCount == 0)
                        {
                            journal.Details = "订单状态由" + oldState + "变为" + commodityOrder.State;
                            journal.StateFrom = oldState;
                            journal.StateTo = commodityOrder.State;
                        }
                        else
                        {
                            journal.Details = "订单单商品退款，订单id：" + commodityOrder.Id + "订单商品项id：" + orderItem.Id;
                            journal.StateFrom = oldState;
                            journal.StateTo = 21;
                        }
                        journal.EntityState = System.Data.EntityState.Added;
                        contextSession.SaveObject(journal);
                        contextSession.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("售中申请退款订单记日志异常。", ex);
                    }

                    if (orderItemCount == 0)
                    {
                        //添加消息
                        BTPMessageSV addmassage = new BTPMessageSV();
                        CommodityOrderMessages messageModel = new CommodityOrderMessages();
                        messageModel.IsAuto = true;
                        messageModel.Id = commodityOrder.Id.ToString();
                        messageModel.UserIds = commodityOrder.UserId.ToString();
                        messageModel.AppId = commodityOrder.AppId;
                        messageModel.Code = commodityOrder.Code;
                        messageModel.State = commodityOrder.State;
                        messageModel.RefundType = orderRefund.RefundType;
                        messageModel.RefundMoney = orderRefund.RefundMoney + orderRefund.RefundScoreMoney;
                        messageModel.PayType = commodityOrder.Payment;
                        messageModel.orderRefundState = orderRefund.State;
                        //messageModel.oldOrderRefundAfterSalesState = 0;
                        messageModel.SelfTakeFlag = commodityOrder.SelfTakeFlag;
                        messageModel.EsAppId = commodityOrder.EsAppId.HasValue
                            ? commodityOrder.EsAppId.Value
                            : commodityOrder.AppId;
                        addmassage.AddMessagesCommodityOrder(messageModel);
                        return new ResultDTO { ResultCode = 0, Message = "Success" };
                    }
                }
                else
                {
                    LogHelper.Error(string.Format("申请退款服务异常。orderSDTO：{0}", submitOrderRefundDTO));
                    return new ResultDTO { ResultCode = 1, Message = "退款失败" };
                }
                return new ResultDTO { ResultCode = 0, Message = "Success" };
            }
            catch (Exception ex)
            {

                LogHelper.Error(string.Format("申请退款服务异常。submitOrderRefundDTO：{0}", submitOrderRefundDTO), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(submitOrderRefundDTO.commodityorderId);
            }
        }


        /// <summary>
        /// 申请退款
        /// </summary>
        /// <param name="submitOrderRefundDTO"></param>
        /// <returns></returns>
        public ResultDTO SubmitOrderRefundExt(SubmitOrderRefundDTO submitOrderRefundDTO)
        {
            if (submitOrderRefundDTO == null || submitOrderRefundDTO.commodityorderId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数错误" };
            }
            if (!OrderSV.LockOrder(submitOrderRefundDTO.commodityorderId))
            {
                return new ResultDTO { ResultCode = 110, Message = "操作失败" };
            }

            if (submitOrderRefundDTO.OrderItemId != Guid.Empty)
            {
                //单商品退款/退货
                return SubmitOrderItemRefund(submitOrderRefundDTO);
            }
            //退积分金额
            decimal spendScoreMoney = 0;
            // 退易捷币
            decimal spendYJBMoney = 0;
            decimal spendAllYJBMoney = 0;
            //原订单状态
            int oldState = 0;
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var commodityOrder = CommodityOrder.ObjectSet().FirstOrDefault(n => n.Id == submitOrderRefundDTO.commodityorderId);
                if (commodityOrder == null)
                {
                    return new ResultDTO { ResultCode = 4, Message = "订单不存在" };
                }
                if (ThirdECommerceHelper.IsWangYiYanXuan(commodityOrder.AppId) && commodityOrder.State == 2)
                {
                    return new ResultDTO { ResultCode = 1, Message = "该订单不支持整单退款~" };
                }

                // 京东商品未发货禁止退款
                if (commodityOrder.State == 1 && JdJobHelper.JDAppIdList.Contains(commodityOrder.AppId))
                {
                    return new ResultDTO { ResultCode = 1, Message = "商品已出库，不能申请退款，可拒收~" };
                }


                if (commodityOrder.State == 11)
                {
                    return new ResultDTO { ResultCode = 5, Message = "钱尚未到达商家账户，处理中，请稍后重试~" };
                }
                //判断订单状态是否可以进行 退款操作
                if (commodityOrder.State != 1 && commodityOrder.State != 2 && commodityOrder.State != 13)
                {
                    return new ResultDTO { ResultCode = 2, Message = "订单状态无法申请退款" };
                }
                //判断订单状态是否 发生改变
                if (submitOrderRefundDTO.State != commodityOrder.State)
                {
                    return new ResultDTO { ResultCode = 3, Message = "订单状态已经改变" };
                }


                //查询抵用券
                decimal couponprice = 0;
                decimal couponuseprice = 0;//抵用券使用金额
                var useryjcoupon = YJBSV.GetUserYJCouponByOrderId(commodityOrder.Id);
                if (useryjcoupon.Data != null)
                {
                    foreach (var item in useryjcoupon.Data)
                    {
                        if (item != null)
                        {
                            couponprice += item.Price;
                            couponuseprice += (item.UseAmount ?? 0);
                        }
                    }
                }

                //退款金额
                if (submitOrderRefundDTO.RefundMoney > commodityOrder.RealPrice)
                {
                    bool isOverPrice = true;
                    spendScoreMoney =
                        OrderPayDetail.ObjectSet()
                            .Where(t => t.OrderId == commodityOrder.Id && t.ObjectType == 2 && t.Amount > 0)
                            .Select(t => t.Amount)
                            .FirstOrDefault();
                    if (spendScoreMoney > 0)
                    {
                        if (submitOrderRefundDTO.RefundMoney <= commodityOrder.RealPrice + spendScoreMoney)
                        {
                            isOverPrice = false;
                        }
                    }


                    decimal realprice = 0;
                    if (commodityOrder.State == 1 || commodityOrder.State == 8)
                    {//待发货，退运费
                        realprice = commodityOrder.RealPrice.Value;
                    }
                    else
                    {//其他状态不退运费
                        realprice = commodityOrder.RealPrice.Value - commodityOrder.Freight;
                    }

                    //查询抵用券
                    if (submitOrderRefundDTO.RefundMoney >= realprice + couponprice)
                    {
                        if (couponprice > 0)
                        {
                            isOverPrice = false;
                        }
                    }


                    // 查询易捷币
                    if (submitOrderRefundDTO.RefundMoney > realprice + spendScoreMoney + couponprice)
                    {
                        var yjbInfo = YJBSV.GetOrderYJBInfo(commodityOrder.EsAppId.Value, commodityOrder.Id);
                        if (!yjbInfo.IsSuccess)
                        {
                            return new ResultDTO { ResultCode = 1, Message = yjbInfo.Message };
                        }
                        spendAllYJBMoney = yjbInfo.Data.InsteadCashAmount;
                        if (spendAllYJBMoney > 0)
                        {
                            if (submitOrderRefundDTO.RefundMoney <=
                               realprice + spendScoreMoney + spendAllYJBMoney + couponprice)
                            {
                                spendYJBMoney = submitOrderRefundDTO.RefundMoney - realprice -
                                                spendScoreMoney - couponprice;
                                isOverPrice = false;
                            }
                        }
                    }
                    else
                    {
                        isOverPrice = false;
                    }

                    if (isOverPrice)
                    {
                        return new ResultDTO { ResultCode = 1, Message = "退款金额不能大于订单金额" };
                    }
                }
                OrderRefund orderRefund = new OrderRefund();
                orderRefund.Id = Guid.NewGuid();
                orderRefund.RefundReason = submitOrderRefundDTO.RefundReason;

                #region

                //if (submitOrderRefundDTO.RefundMoney > commodityOrder.RealPrice + couponprice)
                //{
                //    commodityOrder.RealPrice = commodityOrder.RealPrice.HasValue
                //        ? commodityOrder.RealPrice.Value
                //        : 0;
                //    if (commodityOrder.State == 1 || commodityOrder.State == 8)
                //    {//待发货状态，不可修改退款金额，直接传多少就是多少
                //     orderRefund.OrderRefundMoneyAndCoupun = (submitOrderRefundDTO.RefundMoney - couponprice) > 0 ? (submitOrderRefundDTO.RefundMoney - couponprice) : submitOrderRefundDTO.RefundMoney;

                //        orderRefund.RefundMoney = commodityOrder.RealPrice.Value + spendYJBMoney;
                //    }
                //    else
                //    {
                //        orderRefund.OrderRefundMoneyAndCoupun = commodityOrder.RealPrice.Value + spendYJBMoney;
                //        orderRefund.RefundMoney = commodityOrder.RealPrice.Value + spendYJBMoney - commodityOrder.Freight;
                //    }
                //    orderRefund.RefundScoreMoney = submitOrderRefundDTO.RefundMoney - commodityOrder.RealPrice.Value - couponprice -
                //                                   spendYJBMoney;
                //    // 退易捷币金额
                //    orderRefund.RefundYJBMoney = spendYJBMoney;
                //}
                //else
                //{
                //    if (commodityOrder.State == 1 || commodityOrder.State == 8)
                //    {
                //        orderRefund.OrderRefundMoneyAndCoupun = submitOrderRefundDTO.RefundMoney;
                //        if (submitOrderRefundDTO.RefundMoney - commodityOrder.RealPrice.Value - commodityOrder.Freight > 0)
                //        {
                //            orderRefund.RefundMoney = submitOrderRefundDTO.RefundMoney - commodityOrder.RealPrice.Value - commodityOrder.Freight;
                //            commodityOrder.YJCouponPrice = submitOrderRefundDTO.RefundMoney - commodityOrder.RealPrice.Value;
                //        }
                //        else
                //        {
                //            orderRefund.RefundMoney = submitOrderRefundDTO.RefundMoney;
                //            commodityOrder.YJCouponPrice = 0;
                //        }
                //    }
                //    else
                //    {
                //        orderRefund.OrderRefundMoneyAndCoupun = submitOrderRefundDTO.RefundMoney;
                //        if (submitOrderRefundDTO.RefundMoney - commodityOrder.RealPrice.Value > 0)
                //        {
                //            orderRefund.RefundMoney = submitOrderRefundDTO.RefundMoney - commodityOrder.RealPrice.Value;
                //            commodityOrder.YJCouponPrice = submitOrderRefundDTO.RefundMoney - commodityOrder.RealPrice.Value;
                //        }
                //        else
                //        {
                //            orderRefund.RefundMoney = submitOrderRefundDTO.RefundMoney;
                //            commodityOrder.YJCouponPrice = 0;
                //        }
                //    }

                //}

                #endregion

                decimal yjcPrice = commodityOrder.YJCardPrice ?? 0;

                if (commodityOrder.State == 1 || commodityOrder.State == 8)
                {//待发货，全退
                    LogHelper.Info("SubmitOrderRefund待发货，全退OrderRefundMoneyAndCoupun:" + submitOrderRefundDTO.RefundMoney);
                    LogHelper.Info("SubmitOrderRefund待发货，全退RefundMoney:" + commodityOrder.RealPrice.Value);
                    LogHelper.Info("SubmitOrderRefund待发货，全退RefundYJCouponMoney:" + couponprice);
                    LogHelper.Info("SubmitOrderRefund待发货，全退RefundYJBMoney:" + spendYJBMoney);
                    orderRefund.OrderRefundMoneyAndCoupun = submitOrderRefundDTO.RefundMoney;
                    orderRefund.RefundMoney = commodityOrder.RealPrice.Value;
                    //commodityOrder.YJCouponPrice = couponprice;
                    orderRefund.RefundYJCouponMoney = couponprice;
                    //orderRefund.RefundScoreMoney = submitOrderRefundDTO.RefundMoney - commodityOrder.RealPrice.Value - couponprice -
                    //                              spendYJBMoney;
                    orderRefund.RefundScoreMoney = 0;
                    // 退易捷币金额
                    orderRefund.RefundYJBMoney = spendYJBMoney;
                    //退易捷卡金额
                    orderRefund.RefundYJCardMoney = yjcPrice;
                }
                else
                {
                    //已发货
                    //- commodityOrder.Freight 猜测是退款不能退运费
                    if (submitOrderRefundDTO.RefundMoney >= (commodityOrder.RealPrice - commodityOrder.Freight) + yjcPrice + couponuseprice)
                    {//全额退
                        LogHelper.Info("SubmitOrderRefund已发货，全退OrderRefundMoneyAndCoupun:" + submitOrderRefundDTO.RefundMoney);
                        LogHelper.Info("SubmitOrderRefund已发货，全退RefundMoney:" + (commodityOrder.RealPrice.Value - commodityOrder.Freight));
                        LogHelper.Info("SubmitOrderRefund已发货，全退RefundYJCouponMoney:" + couponprice);
                        LogHelper.Info("SubmitOrderRefund已发货，全退RefundYJBMoney:" + spendYJBMoney);
                        orderRefund.OrderRefundMoneyAndCoupun = submitOrderRefundDTO.RefundMoney;
                        orderRefund.RefundMoney = commodityOrder.RealPrice.Value - commodityOrder.Freight;
                        //commodityOrder.YJCouponPrice = couponprice;
                        orderRefund.RefundYJCouponMoney = couponprice;
                        //orderRefund.RefundScoreMoney = submitOrderRefundDTO.RefundMoney - commodityOrder.RealPrice.Value - couponprice -
                        //                           spendYJBMoney;
                        orderRefund.RefundScoreMoney = 0;
                        // 退易捷币金额
                        orderRefund.RefundYJBMoney = spendYJBMoney;

                        //todo 已发货不能退运费，要把抵运费的易捷卡排除掉。
                    }
                    else
                    {//部分退
                        if (submitOrderRefundDTO.RefundMoney <= (commodityOrder.RealPrice - commodityOrder.Freight))
                        {//只退现金
                            LogHelper.Info("SubmitOrderRefund部分退，只退现金RefundMoney:" + submitOrderRefundDTO.RefundMoney);
                            LogHelper.Info("SubmitOrderRefund部分退，只退现金OrderRefundMoneyAndCoupun:" + submitOrderRefundDTO.RefundMoney);
                            orderRefund.RefundMoney = submitOrderRefundDTO.RefundMoney;
                            orderRefund.OrderRefundMoneyAndCoupun = submitOrderRefundDTO.RefundMoney;
                        }
                        else if (submitOrderRefundDTO.RefundMoney < (commodityOrder.RealPrice - commodityOrder.Freight) + yjcPrice)
                        {
                            //退现金+部分易捷卡；
                            orderRefund.RefundMoney = commodityOrder.RealPrice.Value - commodityOrder.Freight;
                            orderRefund.RefundYJCardMoney = submitOrderRefundDTO.RefundMoney - orderRefund.RefundMoney;
                        }

                        else if (submitOrderRefundDTO.RefundMoney < (commodityOrder.RealPrice - commodityOrder.Freight) + couponuseprice + yjcPrice)
                        {
                            //退现金+易捷卡+部分抵用券
                            LogHelper.Info("SubmitOrderRefund部分退，只退现金RefundMoney:" + (commodityOrder.RealPrice.Value - commodityOrder.Freight));
                            LogHelper.Info("SubmitOrderRefund部分退，只退现金OrderRefundMoneyAndCoupun:" + submitOrderRefundDTO.RefundMoney);
                            LogHelper.Info("SubmitOrderRefund部分退，只退现金RefundYJCouponMoney:" + (orderRefund.OrderRefundMoneyAndCoupun - orderRefund.RefundMoney));
                            orderRefund.RefundMoney = commodityOrder.RealPrice.Value - commodityOrder.Freight;
                            orderRefund.RefundYJCardMoney = yjcPrice;
                            orderRefund.OrderRefundMoneyAndCoupun = submitOrderRefundDTO.RefundMoney;
                            //commodityOrder.YJCouponPrice = orderRefund.OrderRefundMoneyAndCoupun - orderRefund.RefundMoney;
                            orderRefund.RefundYJCouponMoney = submitOrderRefundDTO.RefundMoney - orderRefund.RefundMoney - orderRefund.RefundYJCardMoney;
                        }
                    }
                }
                orderRefund.RefundDesc = submitOrderRefundDTO.RefundDesc;
                orderRefund.OrderRefundImgs = submitOrderRefundDTO.OrderRefundImgs;
                orderRefund.OrderId = submitOrderRefundDTO.commodityorderId;
                //仅退款
                orderRefund.RefundType = submitOrderRefundDTO.RefundType;
                //退款中
                orderRefund.State = 0;
                orderRefund.DataType = "0";
                orderRefund.IsDelayConfirmTimeAfterSales = false;
                if (commodityOrder.RealPrice + spendScoreMoney + spendAllYJBMoney ==
                    submitOrderRefundDTO.RefundMoney)
                {
                    orderRefund.IsFullRefund = true;
                }
                else
                {
                    orderRefund.IsFullRefund = false;
                }
                orderRefund.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(orderRefund);
                oldState = commodityOrder.State;
                switch (commodityOrder.State)
                {
                    //如果待发货 更改为待发货 退款中
                    case 1:
                        commodityOrder.State = 8;
                        break;
                    //如果已发货 更改为已发货 退款中
                    case 2:
                        commodityOrder.State = 9;
                        break;
                    //如果已发货 更改为已发货 退款中
                    case 13:
                        commodityOrder.State = 14;
                        break;
                }
                commodityOrder.RefundTime = DateTime.Now;
                commodityOrder.IsRefund = true;
                commodityOrder.ModifiedOn = DateTime.Now;
                commodityOrder.EntityState = System.Data.EntityState.Modified;
                var eventResult = OrderEventHelper.OnOrderRefund(commodityOrder, oldState, orderRefund);
                if (!eventResult.isSuccess)
                {
                    return new ResultDTO { ResultCode = 1, Message = eventResult.Message };
                }

                int reslult = contextSession.SaveChanges();
                if (reslult > 0)
                {
                    try
                    {
                        //订单日志
                        Journal journal = new Journal();
                        journal.Id = Guid.NewGuid();
                        journal.Name = "售中申请退款订单";
                        journal.Code = commodityOrder.Code;
                        journal.SubId = commodityOrder.UserId;
                        journal.SubTime = DateTime.Now;
                        journal.Details = "订单状态由" + oldState + "变为" + commodityOrder.State;
                        journal.CommodityOrderId = commodityOrder.Id;
                        journal.StateFrom = oldState;
                        journal.StateTo = commodityOrder.State;
                        journal.IsPush = false;
                        journal.OrderType = commodityOrder.OrderType;

                        journal.EntityState = System.Data.EntityState.Added;
                        contextSession.SaveObject(journal);
                        contextSession.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("售中申请退款订单记日志异常。", ex);
                    }

                    //添加消息
                    BTPMessageSV addmassage = new BTPMessageSV();
                    CommodityOrderMessages messageModel = new CommodityOrderMessages();
                    messageModel.IsAuto = true;
                    messageModel.Id = commodityOrder.Id.ToString();
                    messageModel.UserIds = commodityOrder.UserId.ToString();
                    messageModel.AppId = commodityOrder.AppId;
                    messageModel.Code = commodityOrder.Code;
                    messageModel.State = commodityOrder.State;
                    messageModel.RefundType = orderRefund.RefundType;
                    messageModel.RefundMoney = orderRefund.RefundMoney + orderRefund.RefundScoreMoney;
                    messageModel.PayType = commodityOrder.Payment;
                    messageModel.orderRefundState = orderRefund.State;
                    //messageModel.oldOrderRefundAfterSalesState = 0;
                    messageModel.SelfTakeFlag = commodityOrder.SelfTakeFlag;
                    messageModel.EsAppId = commodityOrder.EsAppId.HasValue
                        ? commodityOrder.EsAppId.Value
                        : commodityOrder.AppId;
                    addmassage.AddMessagesCommodityOrder(messageModel);

                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                }
                else
                {
                    LogHelper.Error(string.Format("申请退款服务异常。orderSDTO：{0}", submitOrderRefundDTO));
                    return new ResultDTO { ResultCode = 1, Message = "退款失败" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("申请退款服务异常。submitOrderRefundDTO：{0}", submitOrderRefundDTO), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(submitOrderRefundDTO.commodityorderId);
            }
        }

        /// <summary>
        ///  查看退款申请
        /// </summary>
        /// <param name="commodityorderId"></param>
        /// <returns></returns>
        public SubmitOrderRefundDTO GetOrderRefundExt(Guid commodityorderId, Guid orderItemId)
        {
            try
            {
                var order = CommodityOrder.FindByID(commodityorderId);
                List<int> rState = new List<int> { 2, 3, 4, 13 };
                SubmitOrderRefundDTO orre;
                if (orderItemId != Guid.Empty)
                {
                    orre = (from o in OrderRefund.ObjectSet()
                            where o.OrderId == commodityorderId && !rState.Contains(o.State) && o.OrderItemId == orderItemId
                            select new SubmitOrderRefundDTO()
                            {
                                commodityorderId = o.OrderId,
                                RefundReason = o.RefundReason,
                                RefundMoney = o.RefundMoney + o.RefundYJBMoney,
                                RefundDesc = o.RefundDesc,
                                OrderRefundImgs = o.OrderRefundImgs,
                                State = o.State,
                                RefundType = o.RefundType,
                                RefuseTime = o.RefuseTime,
                                RefuseReason = o.RefuseReason,
                                RefundExpOrderTime = o.RefundExpOrderTime,
                                RefundExpCo = o.RefundExpCo,
                                RefundExpOrderNo = o.RefundExpOrderNo,
                                IsDelayConfirmTimeAfterSales =
                                    (o.IsDelayConfirmTimeAfterSales == true ? true : false),
                                RefundScoreMoney = o.RefundScoreMoney,
                                SubTime = o.SubTime
                            }).FirstOrDefault();

                    var orderItem = OrderItem.FindByID(orderItemId);
                    if (orre != null)
                    {
                        orre.Pic = orderItem.PicturesPath;
                        orre.Name = orderItem.Name;
                        orre.CommodityAttributes = orderItem.CommodityAttributes;
                        orre.Num = orderItem.Number;
                        orre.OrderItemState = orderItem.State == null ? 0 : (int)orderItem.State;
                    }
                    else
                    {
                        //整单退款
                        orre = (from o in OrderRefund.ObjectSet()
                                where o.OrderId == commodityorderId && !rState.Contains(o.State)
                                select new SubmitOrderRefundDTO()
                                {
                                    commodityorderId = o.OrderId,
                                    RefundReason = o.RefundReason,
                                    RefundMoney = o.RefundMoney + o.RefundYJBMoney,
                                    RefundDesc = o.RefundDesc,
                                    OrderRefundImgs = o.OrderRefundImgs,
                                    State = o.State,
                                    RefundType = o.RefundType,
                                    RefuseTime = o.RefuseTime,
                                    RefuseReason = o.RefuseReason,
                                    RefundExpOrderTime = o.RefundExpOrderTime,
                                    RefundExpCo = o.RefundExpCo,
                                    RefundExpOrderNo = o.RefundExpOrderNo,
                                    IsDelayConfirmTimeAfterSales =
                                        (o.IsDelayConfirmTimeAfterSales == true ? true : false),
                                    RefundScoreMoney = o.RefundScoreMoney,
                                    SubTime = o.SubTime
                                }).FirstOrDefault();
                    }
                }
                else
                {
                    orre = (from o in OrderRefund.ObjectSet()
                            where o.OrderId == commodityorderId && !rState.Contains(o.State)
                            select new SubmitOrderRefundDTO()
                            {
                                commodityorderId = o.OrderId,
                                RefundReason = o.RefundReason,
                                RefundMoney = o.RefundMoney + o.RefundYJBMoney,
                                RefundDesc = o.RefundDesc,
                                OrderRefundImgs = o.OrderRefundImgs,
                                State = o.State,
                                RefundType = o.RefundType,
                                RefuseTime = o.RefuseTime,
                                RefuseReason = o.RefuseReason,
                                RefundExpOrderTime = o.RefundExpOrderTime,
                                RefundExpCo = o.RefundExpCo,
                                RefundExpOrderNo = o.RefundExpOrderNo,
                                IsDelayConfirmTimeAfterSales =
                                    (o.IsDelayConfirmTimeAfterSales == true ? true : false),
                                RefundScoreMoney = o.RefundScoreMoney,
                                SubTime = o.SubTime
                            }).FirstOrDefault();
                }

                //读取售完退款/退货表
                if (orre == null)
                {
                    if (orderItemId != Guid.Empty)
                    {
                        var orderRefundAfterSales = OrderRefundAfterSales.ObjectSet().Where(t => t.OrderId == commodityorderId && t.OrderItemId == orderItemId).OrderByDescending(t => t.SubTime).FirstOrDefault();
                        if (orderRefundAfterSales != null)
                        {
                            SubmitOrderRefundDTO result = new SubmitOrderRefundDTO()
                            {
                                Id = orderRefundAfterSales.Id,
                                commodityorderId = orderRefundAfterSales.OrderId,
                                RefundReason = orderRefundAfterSales.RefundReason,
                                RefundMoney = orderRefundAfterSales.RefundMoney + orderRefundAfterSales.RefundScoreMoney + orderRefundAfterSales.RefundYJBMoney,
                                RefundDesc = orderRefundAfterSales.RefundDesc,
                                OrderRefundImgs = orderRefundAfterSales.OrderRefundImgs,
                                State = orderRefundAfterSales.State,
                                RefundExpCo = orderRefundAfterSales.RefundExpCo,
                                RefundExpOrderNo = orderRefundAfterSales.RefundExpOrderNo,
                                RefundType = orderRefundAfterSales.RefundType,
                                RefuseReason = orderRefundAfterSales.RefuseReason,
                                RefuseTime = orderRefundAfterSales.RefuseTime,
                                RefundExpOrderTime = orderRefundAfterSales.RefundExpOrderTime,
                                SubTime = orderRefundAfterSales.SubTime,
                                PickwareType = orderRefundAfterSales.PickwareType,
                                PickUpFreightMoney = orderRefundAfterSales.PickUpFreightMoney ?? 0,
                                Address = new AddressInfo
                                {
                                    customerContactName = orderRefundAfterSales.CustomerContactName,
                                    customerTel = orderRefundAfterSales.CustomerTel,
                                    pickwareAddress = orderRefundAfterSales.PickwareAddress
                                }
                            };
                            var orderItem = OrderItem.FindByID(orderItemId);
                            result.Pic = orderItem.PicturesPath;
                            result.Name = orderItem.Name;
                            result.CommodityAttributes = orderItem.CommodityAttributes;
                            result.Num = orderItem.Number;
                            result.OrderItemState = orderItem.State == null ? 0 : (int)orderItem.State;
                            result.IsThirdECommerce = ThirdECommerceHelper.IsWangYiYanXuan(order.AppId);
                            return result;
                        }
                        else
                        {
                            //整单退款
                            orderRefundAfterSales = OrderRefundAfterSales.ObjectSet().Where(t => t.OrderId == commodityorderId).OrderByDescending(t => t.SubTime).FirstOrDefault();
                            if (orderRefundAfterSales != null)
                            {
                                SubmitOrderRefundDTO result = new SubmitOrderRefundDTO()
                                {
                                    Id = orderRefundAfterSales.Id,
                                    commodityorderId = orderRefundAfterSales.OrderId,
                                    RefundReason = orderRefundAfterSales.RefundReason,
                                    RefundMoney = orderRefundAfterSales.RefundMoney + orderRefundAfterSales.RefundScoreMoney + orderRefundAfterSales.RefundYJBMoney,
                                    RefundDesc = orderRefundAfterSales.RefundDesc,
                                    OrderRefundImgs = orderRefundAfterSales.OrderRefundImgs,
                                    State = orderRefundAfterSales.State,
                                    RefundExpCo = orderRefundAfterSales.RefundExpCo,
                                    RefundExpOrderNo = orderRefundAfterSales.RefundExpOrderNo,
                                    RefundType = orderRefundAfterSales.RefundType,
                                    RefuseReason = orderRefundAfterSales.RefuseReason,
                                    RefuseTime = orderRefundAfterSales.RefuseTime,
                                    RefundExpOrderTime = orderRefundAfterSales.RefundExpOrderTime,
                                    SubTime = orderRefundAfterSales.SubTime
                                };
                                result.IsThirdECommerce = ThirdECommerceHelper.IsWangYiYanXuan(order.AppId);
                                return result;
                            }
                        }
                    }
                    else
                    {
                        var orderRefundAfterSales = OrderRefundAfterSales.ObjectSet().Where(t => t.OrderId == commodityorderId).OrderByDescending(t => t.SubTime).FirstOrDefault();
                        if (orderRefundAfterSales != null)
                        {
                            SubmitOrderRefundDTO result = new SubmitOrderRefundDTO()
                            {
                                Id = orderRefundAfterSales.Id,
                                commodityorderId = orderRefundAfterSales.OrderId,
                                RefundReason = orderRefundAfterSales.RefundReason,
                                RefundMoney = orderRefundAfterSales.RefundMoney + orderRefundAfterSales.RefundScoreMoney + orderRefundAfterSales.RefundYJBMoney,
                                RefundDesc = orderRefundAfterSales.RefundDesc,
                                OrderRefundImgs = orderRefundAfterSales.OrderRefundImgs,
                                State = orderRefundAfterSales.State,
                                RefundExpCo = orderRefundAfterSales.RefundExpCo,
                                RefundExpOrderNo = orderRefundAfterSales.RefundExpOrderNo,
                                RefundType = orderRefundAfterSales.RefundType,
                                RefuseReason = orderRefundAfterSales.RefuseReason,
                                RefuseTime = orderRefundAfterSales.RefuseTime,
                                RefundExpOrderTime = orderRefundAfterSales.RefundExpOrderTime,
                                SubTime = orderRefundAfterSales.SubTime
                            };
                            result.IsThirdECommerce = ThirdECommerceHelper.IsWangYiYanXuan(order.AppId);
                            return result;
                        }
                    }
                }
                if (ThirdECommerceHelper.IsWangYiYanXuan(order.AppId))
                {
                    if (orre.RefundType == 0)
                    {
                        // 禁止取消
                        orre.CanCancel = false;
                    }
                }
                return orre;

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("查看退款申请服务异常。commodityorderId：{0}", commodityorderId), ex);
                return new SubmitOrderRefundDTO();
            }

        }

        /// <summary>
        /// 撤销退款申请
        /// </summary>
        /// <param name="commodityorderId"></param>
        /// <param name="state">无用，只用来记error时的日志</param>
        /// <returns></returns>
        public ResultDTO CancelOrderRefundExt(Guid commodityorderId, int state)
        {
            if (commodityorderId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数不能为空" };
            }
            if (!OrderSV.LockOrder(commodityorderId))
            {
                return new ResultDTO { ResultCode = 110, Message = "操作失败" };
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var stateList = new List<int> { 8, 9, 10, 14 };
                var commodityOrderList = (from c in CommodityOrder.ObjectSet()
                                          join r in OrderRefund.ObjectSet() on c.Id equals r.OrderId
                                          where c.Id == commodityorderId && stateList.Contains(c.State) && (r.State == 0 || r.State == 10 || r.State == 11)
                                          select new
                                          {
                                              commodityOrder = c,
                                              orderRefund = r
                                          }).FirstOrDefault();
                if (commodityOrderList == null)
                {
                    return new ResultDTO { ResultCode = 2, Message = "找不到相应的订单" };
                }

                var commodityOrder = commodityOrderList.commodityOrder;
                var orderRefund = commodityOrderList.orderRefund;

                int oldState = commodityOrder.State;
                switch (commodityOrder.State)
                {
                    //如果待发货退款中 更改为待发货
                    case 8:
                        commodityOrder.State = 1;
                        break;
                    //如果已发货退款中 更改为已发货 
                    case 9:
                        commodityOrder.State = 2;
                        break;
                    //如果出库中退款中 更改为出库中
                    case 14:
                        commodityOrder.State = 13;
                        break;
                    //如果已发货退款中商家同意退款，商家未收到货，更改为已发货
                    case 10:
                        commodityOrder.State = 2;
                        break;
                }
                if (oldState == 10 && orderRefund.AgreeFlag == 0)
                {
                    commodityOrder.State = 13;
                }

                commodityOrder.IsRefund = false;
                commodityOrder.RefundTime = null;
                commodityOrder.ModifiedOn = DateTime.Now;
                commodityOrder.EntityState = System.Data.EntityState.Modified;

                orderRefund.State = 3;
                orderRefund.ModifiedOn = DateTime.Now;
                orderRefund.EntityState = System.Data.EntityState.Modified;

                var eventResult = OrderEventHelper.OnCancelOrderRefund(commodityOrder, orderRefund);
                if (!eventResult.isSuccess)
                {
                    eventResult.ResultCode = -1;
                    return eventResult;
                }

                int reslult = contextSession.SaveChanges();
                if (reslult > 0)
                {
                    BTPMessageSV addmassage = new BTPMessageSV();
                    //发送消息，异步执行
                    System.Threading.ThreadPool.QueueUserWorkItem(
                        a =>
                        {
                            CommodityOrderMessages messageModel = new CommodityOrderMessages();
                            messageModel.IsAuto = false;
                            messageModel.Id = commodityOrder.Id.ToString();
                            messageModel.UserIds = commodityOrder.UserId.ToString();
                            messageModel.AppId = commodityOrder.AppId;
                            messageModel.Code = commodityOrder.Code;
                            messageModel.State = commodityOrder.State;
                            messageModel.RefundType = orderRefund.RefundType;
                            messageModel.RefundMoney = orderRefund.RefundMoney + orderRefund.RefundScoreMoney;
                            messageModel.PayType = commodityOrder.Payment;
                            messageModel.orderRefundState = orderRefund.State;
                            messageModel.EsAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
                            addmassage.AddMessagesCommodityOrder(messageModel);
                        });

                    try
                    {
                        Journal journal = new Journal();
                        journal.Id = Guid.NewGuid();
                        journal.Name = "撤销退款/退货申请";
                        journal.Code = commodityOrder.Code;
                        journal.SubId = commodityOrder.UserId;
                        journal.SubTime = DateTime.Now;
                        journal.Details = "订单状态由" + oldState + "变为" + commodityOrder.State;
                        journal.StateFrom = oldState;
                        journal.StateTo = commodityOrder.State;
                        journal.IsPush = false;
                        journal.OrderType = commodityOrder.OrderType;
                        journal.CommodityOrderId = commodityOrder.Id;
                        journal.EntityState = System.Data.EntityState.Added;
                        contextSession.SaveObject(journal);
                        contextSession.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("撤销退款/退货申请记日志异常。", ex);
                    }
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                }
                else
                {
                    LogHelper.Error(string.Format("撤销退款申请。orderSDTO：{0}", commodityorderId.ToString() + "," + state));
                    return new ResultDTO { ResultCode = 1, Message = "退款失败" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("撤销退款申请。commodityorderId,state：{0}", commodityorderId.ToString() + "," + state.ToString()), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(commodityorderId);
            }
        }

        /// <summary>
        /// 撤销退款申请
        /// </summary>
        /// <param name="commodityorderId"></param>
        /// <param name="state">无用，只用来记error时的日志</param>
        /// <returns></returns>
        public ResultDTO CancelOrderItemRefundExt(Guid commodityorderId, int state, Guid orderItemId)
        {
            LogHelper.Debug("开始进入撤销退款申请方法CancelOrderItemRefundExt，参数为commodityorderId：" + commodityorderId + ",state:" + state + ",orderItemId:" + orderItemId);
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            if (commodityorderId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数不能为空" };
            }
            if (!OrderSV.LockOrder(commodityorderId))
            {
                return new ResultDTO { ResultCode = 110, Message = "操作失败" };
            }
            if (orderItemId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数不能为空" };
            }
            try
            {
                var orderItem = OrderItem.FindByID(orderItemId);
                orderItem.State = 0;
                orderItem.ModifiedOn = DateTime.Now;
                orderItem.EntityState = EntityState.Modified;
                contextSession.SaveObject(orderItem);

                var commodityOrder = CommodityOrder.FindByID(commodityorderId);
                var oldState = commodityOrder.State;
                var orderItemCount = OrderItem.ObjectSet().Count(t => t.Id != orderItemId && t.CommodityOrderId == commodityorderId && t.State == 0);

                var orderRefund = OrderRefund.ObjectSet().FirstOrDefault(t => t.OrderItemId == orderItemId);
                if (orderRefund != null)
                {
                    orderRefund.State = 3;
                    orderRefund.ModifiedOn = DateTime.Now;
                    orderRefund.EntityState = System.Data.EntityState.Modified;
                    contextSession.SaveObject(orderRefund);
                }
                else
                {
                    var orderRefundAfterSales = OrderRefundAfterSales.ObjectSet().OrderByDescending(_ => _.SubTime).FirstOrDefault(t => t.OrderItemId == orderItemId);
                    if (orderRefundAfterSales != null)
                    {
                        orderRefundAfterSales.State = 3;
                        orderRefundAfterSales.ModifiedOn = DateTime.Now;
                        orderRefundAfterSales.EntityState = System.Data.EntityState.Modified;
                        contextSession.SaveObject(orderRefundAfterSales);

                        // 京东退款
                        var jdCancelResult = JdOrderHelper.CancelRefund(contextSession, orderRefundAfterSales);
                        if (!jdCancelResult.isSuccess)
                        {
                            return jdCancelResult;
                        }
                    }
                }

                if (orderItemCount == 0)
                {
                    switch (commodityOrder.State)
                    {
                        //如果待发货退款中 更改为待发货
                        case 8:
                            commodityOrder.State = 1;
                            break;
                        //如果已发货退款中 更改为已发货 
                        case 9:
                            commodityOrder.State = 2;
                            break;
                        //如果出库中退款中 更改为出库中
                        case 14:
                            commodityOrder.State = 13;
                            break;
                        //如果已发货退款中商家同意退款，商家未收到货，更改为已发货
                        case 10:
                            commodityOrder.State = 2;
                            break;
                    }
                    if (orderRefund != null)
                    {
                        if (oldState == 10 && orderRefund.AgreeFlag == 0)
                        {
                            commodityOrder.State = 13;
                        }
                    }

                    commodityOrder.IsRefund = false;
                    commodityOrder.RefundTime = null;
                    commodityOrder.ModifiedOn = DateTime.Now;
                    commodityOrder.EntityState = System.Data.EntityState.Modified;
                    contextSession.SaveObject(commodityOrder);
                }

                int reslult = contextSession.SaveChanges();
                if (reslult > 0)
                {
                    try
                    {
                        Journal journal = new Journal();
                        journal.Id = Guid.NewGuid();
                        journal.Name = "撤销退款/退货申请";
                        journal.Code = commodityOrder.Code;
                        journal.SubId = commodityOrder.UserId;
                        journal.SubTime = DateTime.Now;
                        journal.IsPush = false;
                        journal.OrderType = commodityOrder.OrderType;
                        journal.CommodityOrderId = commodityOrder.Id;
                        if (orderItemCount == 0)
                        {
                            journal.Details = "订单状态由" + oldState + "变为" + commodityOrder.State;
                            journal.StateFrom = oldState;
                            journal.StateTo = commodityOrder.State;
                        }
                        else
                        {
                            journal.Details = "订单商品项进行退款，commodityOrderId：" + commodityOrder + "orderItemId:" + orderItemId;
                            journal.StateFrom = oldState;
                            journal.StateTo = commodityOrder.State;
                        }

                        journal.EntityState = System.Data.EntityState.Added;
                        contextSession.SaveObject(journal);
                        contextSession.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("撤销退款/退货申请记日志异常。", ex);
                    }
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                }
                else
                {
                    LogHelper.Error(string.Format("撤销退款申请。orderSDTO：{0}", commodityorderId.ToString() + "," + state));
                    return new ResultDTO { ResultCode = 1, Message = "退款失败" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("撤销退款申请。commodityorderId,state：{0}", commodityorderId.ToString() + "," + state.ToString()), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(commodityorderId);
            }
        }
        /// <summary>
        /// 退款物流信息提交
        /// </summary>
        /// <param name="commodityorderId"></param>
        /// <param name="RefundExpCo">退货物流公司</param>
        /// <param name="RefundExpOrderNo">退货单号</param>
        /// <returns></returns>
        public ResultDTO AddOrderRefundExpExt(Guid commodityorderId, string RefundExpCo, string RefundExpOrderNo, Guid orderItemId)
        {
            if (orderItemId != Guid.Empty)
            {
                var orderRefund = OrderRefund.ObjectSet().Where(t => t.OrderItemId == orderItemId);
                var orderRefundAfterSales = OrderRefundAfterSales.ObjectSet().Where(t => t.OrderItemId == orderItemId);
                if (orderRefund.Count() > 0 || orderRefundAfterSales.Count() > 0)
                {
                    return AddOrderItemRefundExp(commodityorderId, RefundExpCo, RefundExpOrderNo, orderItemId);
                }
            }
            if (commodityorderId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数不能为空" };
            }
            if (!OrderSV.LockOrder(commodityorderId))
            {
                return new ResultDTO { ResultCode = 110, Message = "操作失败" };
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var commodityOrderList = (from c in CommodityOrder.ObjectSet()
                                          join r in OrderRefund.ObjectSet() on c.Id equals r.OrderId
                                          where c.Id == commodityorderId && c.State == 10 && r.State == 10
                                          select new
                                          {
                                              commodityOrder = c,
                                              orderRefund = r
                                          }).FirstOrDefault();

                if (commodityOrderList == null)
                {
                    return new ResultDTO { ResultCode = 2, Message = "找不到相应的订单" };
                }

                var commodityOrder = commodityOrderList.commodityOrder;
                var orderRefund = commodityOrderList.orderRefund;

                orderRefund.State = 11;
                orderRefund.RefundExpCo = string.IsNullOrWhiteSpace(RefundExpCo) ? "" : RefundExpCo.Trim();
                orderRefund.RefundExpOrderNo = string.IsNullOrWhiteSpace(RefundExpOrderNo) ? "" : RefundExpOrderNo.Trim();
                orderRefund.RefundExpOrderNo = orderRefund.RefundExpOrderNo.Replace("+", "");
                orderRefund.RefundExpOrderTime = DateTime.Now;
                orderRefund.ModifiedOn = DateTime.Now;
                orderRefund.RefundExpOrderTime = DateTime.Now;
                orderRefund.EntityState = System.Data.EntityState.Modified;
                commodityOrder.ModifiedOn = DateTime.Now;
                commodityOrder.EntityState = System.Data.EntityState.Modified;

                int reslult = contextSession.SaveChanges();
                if (reslult > 0)
                {
                    BTPMessageSV addmassage = new BTPMessageSV();
                    //发送消息，异步执行
                    System.Threading.ThreadPool.QueueUserWorkItem(
                        a =>
                        {
                            CommodityOrderMessages messageModel = new CommodityOrderMessages();
                            messageModel.IsAuto = false;
                            messageModel.Id = commodityOrder.Id.ToString();
                            messageModel.UserIds = commodityOrder.UserId.ToString();
                            messageModel.AppId = commodityOrder.AppId;
                            messageModel.Code = commodityOrder.Code;
                            messageModel.State = commodityOrder.State;
                            messageModel.RefundType = orderRefund.RefundType;
                            messageModel.RefundMoney = orderRefund.RefundMoney + orderRefund.RefundScoreMoney;
                            messageModel.PayType = commodityOrder.Payment;
                            messageModel.orderRefundState = orderRefund.State;
                            messageModel.EsAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
                            addmassage.AddMessagesCommodityOrder(messageModel);
                        });


                    OrderExpressRoute oer = new OrderExpressRoute()
                    {
                        ShipExpCo = orderRefund.RefundExpCo,
                        ExpOrderNo = orderRefund.RefundExpOrderNo
                    };
                    OrderExpressRouteSV oerSv = new OrderExpressRouteSV();
                    oerSv.SubscribeOneOrderExpressExt(oer);

                    //添加退货物流跟踪信息
                    RefundExpressTraceDTO retd = new RefundExpressTraceDTO();
                    retd.OrderId = commodityorderId;
                    retd.RefundExpCo = orderRefund.RefundExpCo;
                    retd.RefundExpOrderNo = orderRefund.RefundExpOrderNo;
                    retd.UploadExpOrderTime = DateTime.Now;
                    UpdateRefundExpress(retd);

                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                }
                else
                {
                    return new ResultDTO { ResultCode = 1, Message = "退款失败" };
                }
            }
            catch (Exception ex)
            {

                LogHelper.Error(string.Format("退款物流信息提交服务异常。commodityorderId,RefundExpCo,RefundExpOrderNo：{0}", commodityorderId.ToString() + "," + RefundExpCo + "," + RefundExpOrderNo), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(commodityorderId);
            }
        }
        /// <summary>
        /// 退款物流信息提交 单商品
        /// </summary>
        /// <param name="commodityorderId"></param>
        /// <param name="RefundExpCo">退货物流公司</param>
        /// <param name="RefundExpOrderNo">退货单号</param>
        /// <returns></returns>
        private ResultDTO AddOrderItemRefundExp(Guid commodityorderId, string RefundExpCo, string RefundExpOrderNo, Guid orderItemId)
        {
            if (orderItemId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数不能为空" };
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var commodityOrder = CommodityOrder.FindByID(commodityorderId);
                var orderRefund = OrderRefund.ObjectSet().Where(t => t.OrderItemId == orderItemId).OrderByDescending(t => t.SubTime).FirstOrDefault();

                if (orderRefund != null)
                {
                    orderRefund.State = 11;
                    orderRefund.RefundExpCo = string.IsNullOrWhiteSpace(RefundExpCo) ? "" : RefundExpCo.Trim();
                    orderRefund.RefundExpOrderNo = string.IsNullOrWhiteSpace(RefundExpOrderNo)
                        ? ""
                        : RefundExpOrderNo.Trim();
                    orderRefund.RefundExpOrderNo = orderRefund.RefundExpOrderNo.Replace("+", "");
                    orderRefund.RefundExpOrderTime = DateTime.Now;
                    orderRefund.ModifiedOn = DateTime.Now;
                    orderRefund.RefundExpOrderTime = DateTime.Now;
                    orderRefund.EntityState = System.Data.EntityState.Modified;
                    contextSession.SaveObject(orderRefund);

                    var orderItem = OrderItem.FindByID(orderItemId);
                    orderItem.RefundExpCo = orderRefund.RefundExpCo;
                    orderItem.RefundExpOrderNo = orderRefund.RefundExpOrderNo;
                    orderItem.ModifiedOn = DateTime.Now;
                    orderItem.EntityState = EntityState.Modified;
                    contextSession.SaveObject(orderItem);

                    var eventResult = OrderEventHelper.OnOrderRefundOfferExpress(orderRefund, RefundExpCo, RefundExpOrderNo);
                    if (!eventResult.isSuccess)
                    {
                        return new ResultDTO { ResultCode = 1, Message = eventResult.Message };
                    }

                    int reslult = contextSession.SaveChanges();
                    if (reslult > 0)
                    {
                        OrderExpressRoute oer = new OrderExpressRoute()
                        {
                            ShipExpCo = orderRefund.RefundExpCo,
                            ExpOrderNo = orderRefund.RefundExpOrderNo
                        };
                        OrderExpressRouteSV oerSv = new OrderExpressRouteSV();
                        oerSv.SubscribeOneOrderExpressExt(oer);

                        //添加退货物流跟踪信息
                        RefundExpressTraceDTO retd = new RefundExpressTraceDTO();
                        retd.OrderId = commodityorderId;
                        retd.RefundExpCo = orderRefund.RefundExpCo;
                        retd.RefundExpOrderNo = orderRefund.RefundExpOrderNo;
                        retd.UploadExpOrderTime = DateTime.Now;
                        UpdateRefundExpress(retd);

                        return new ResultDTO { ResultCode = 0, Message = "Success" };
                    }
                    else
                    {
                        return new ResultDTO { ResultCode = 1, Message = "退款失败" };
                    }
                }
                else
                {
                    var orderRefundAfterSales = OrderRefundAfterSales.ObjectSet().Where(t => t.OrderItemId == orderItemId).OrderByDescending(t => t.SubTime).FirstOrDefault();
                    if (orderRefundAfterSales != null)
                    {
                        orderRefundAfterSales.State = 11;
                        orderRefundAfterSales.RefundExpCo = string.IsNullOrWhiteSpace(RefundExpCo) ? "" : RefundExpCo.Trim();
                        orderRefundAfterSales.RefundExpOrderNo = string.IsNullOrWhiteSpace(RefundExpOrderNo)
                            ? ""
                            : RefundExpOrderNo.Trim();
                        orderRefundAfterSales.RefundExpOrderNo = orderRefundAfterSales.RefundExpOrderNo.Replace("+", "");
                        orderRefundAfterSales.RefundExpOrderTime = DateTime.Now;
                        orderRefundAfterSales.ModifiedOn = DateTime.Now;
                        orderRefundAfterSales.RefundExpOrderTime = DateTime.Now;
                        orderRefundAfterSales.EntityState = System.Data.EntityState.Modified;
                        contextSession.SaveObject(orderRefundAfterSales);

                        var orderItem = OrderItem.FindByID(orderItemId);
                        orderItem.RefundExpCo = orderRefundAfterSales.RefundExpCo;
                        orderItem.RefundExpOrderNo = orderRefundAfterSales.RefundExpOrderNo;
                        orderItem.ModifiedOn = DateTime.Now;
                        orderItem.EntityState = EntityState.Modified;
                        contextSession.SaveObject(orderItem);

                        var eventResult = OrderEventHelper.OnOrderRefundOfferExpress(commodityOrder, orderRefundAfterSales, RefundExpCo, RefundExpOrderNo);
                        if (!eventResult.isSuccess)
                        {
                            return new ResultDTO { ResultCode = 1, Message = eventResult.Message };
                        }

                        int reslult = contextSession.SaveChanges();
                        if (reslult > 0)
                        {
                            if (orderRefundAfterSales.PickwareType.HasValue && orderRefundAfterSales.JDEclpOrderRefundAfterSalesId.HasValue
                                && orderRefundAfterSales.JDEclpOrderRefundAfterSalesId.Value != Guid.Empty) //进销存订单京东售后
                                new IBP.Facade.JdEclpOrderFacade().CreateJDEclpRefundAfterSales(commodityorderId, orderItemId, string.Empty);
                            OrderExpressRoute oer = new OrderExpressRoute()
                            {
                                ShipExpCo = orderRefundAfterSales.RefundExpCo,
                                ExpOrderNo = orderRefundAfterSales.RefundExpOrderNo
                            };
                            OrderExpressRouteSV oerSv = new OrderExpressRouteSV();
                            oerSv.SubscribeOneOrderExpressExt(oer);

                            //添加退货物流跟踪信息
                            RefundExpressTraceDTO retd = new RefundExpressTraceDTO();
                            retd.OrderId = commodityorderId;
                            retd.RefundExpCo = orderRefundAfterSales.RefundExpCo;
                            retd.RefundExpOrderNo = orderRefundAfterSales.RefundExpOrderNo;
                            retd.UploadExpOrderTime = DateTime.Now;
                            UpdateRefundExpress(retd);

                            return new ResultDTO { ResultCode = 0, Message = "Success" };
                        }
                        else
                        {
                            return new ResultDTO { ResultCode = 1, Message = "退款失败" };
                        }
                    }
                }
                return new ResultDTO { ResultCode = 1, Message = "退款失败" };

            }
            catch (Exception ex)
            {

                LogHelper.Error(string.Format("退款物流信息提交服务异常。commodityorderId,RefundExpCo,RefundExpOrderNo：{0}", commodityorderId.ToString() + "," + RefundExpCo + "," + RefundExpOrderNo), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(commodityorderId);
            }
        }

        /// <summary>
        /// 添加退货物流跟踪信息
        /// </summary>
        /// <param name="retd"></param>
        private void UpdateRefundExpress(RefundExpressTraceDTO retd)
        {
            RefundExpressTraceSV sv = new RefundExpressTraceSV();
            sv.UpdateRefundExpressExt(retd);
        }
        /// <summary>
        /// 延长收货时间 
        /// </summary>
        /// <param name="commodityorderId"></param>
        /// <returns></returns>
        public ResultDTO DelayConfirmTimeExt(Guid commodityorderId)
        {
            if (commodityorderId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数不能为空" };
            }
            if (!OrderSV.LockOrder(commodityorderId))
            {
                return new ResultDTO { ResultCode = 110, Message = "操作失败" };
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var commodityOrder = CommodityOrder.ObjectSet().Where(n => n.Id == commodityorderId).FirstOrDefault();
                if (commodityOrder.State != 2)
                {
                    return new ResultDTO { ResultCode = 2, Message = "该订单状态,不能进行延长收货时间操作" };
                }
                commodityOrder.IsDelayConfirmTime = true;
                commodityOrder.EntityState = System.Data.EntityState.Modified;
                contextSession.SaveObject(commodityOrder);
                int reslult = contextSession.SaveChanges();
                if (reslult > 0)
                {
                    BTPMessageSV addmassage = new BTPMessageSV();
                    //发送消息，异步执行
                    System.Threading.ThreadPool.QueueUserWorkItem(
                        a =>
                        {
                            CommodityOrderMessages messageModel = new CommodityOrderMessages();
                            messageModel.IsAuto = false;
                            messageModel.Id = commodityOrder.Id.ToString();
                            messageModel.UserIds = commodityOrder.UserId.ToString();
                            messageModel.AppId = commodityOrder.AppId;
                            messageModel.Code = commodityOrder.Code;
                            messageModel.State = commodityOrder.State;
                            // messageModel.RefundType = orderRefund.RefundType;
                            // messageModel.RefundMoney = orderRefund.RefundMoney;
                            messageModel.PayType = commodityOrder.Payment;
                            //messageModel.orderRefundState = orderRefund.State;
                            messageModel.IsBuyersDelayTime = true;
                            messageModel.EsAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
                            addmassage.AddMessagesCommodityOrder(messageModel);
                        });
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                }
                else
                {
                    return new ResultDTO { ResultCode = 1, Message = "延长收货时间失败" };
                }
            }
            catch (Exception ex)
            {

                LogHelper.Error(string.Format("延长收货时间服务异常。commodityorderId：{0}", commodityorderId.ToString()), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(commodityorderId);
            }

        }
        /// <summary>
        /// 提交订单后3天未付款，则交易状态变为“交易失败”（超时交易关闭state=6） @王超 修改为24小时未付款，则交易失败
        /// </summary>
        public void ThreeDayNoPayOrderExt()
        {
            LogHelper.Info(string.Format("处理三天未付款订单服务开始"));
            //处理订单状态为超时交易关闭
            try
            {

                DateTime now = DateTime.Now;
                //查询三天未付款的订单 包括直接到账的订单 
                //DateTime lastday = now.AddDays(-3);
                DateTime lastday = now.AddHours(-24);//@王超 此处修改为24小时
                var orders = CommodityOrder.ObjectSet().Where(n => (n.State == 0 || n.State == 17) && n.SubTime < lastday).ToList();


                int oldsate = 0;
                LogHelper.Info(string.Format("处理三天未付款订单服务处理订单数:{0}", orders.Count));
                if (orders.Count > 0)
                {
                    foreach (CommodityOrder order in orders)
                    {
                        noPayOrder(order, now);
                        OrderCancel(order);
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error("处理三天未付款订单服务异常。", ex);
            }
        }

        /// <summary>
        /// 京东取消按钮
        /// </summary>
        /// <param name="order"></param>
        public void OrderCancel(CommodityOrder order)
        {
            var orderId = order.Id.ToString().ToLower();
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            var orderitem = OrderItem.ObjectSet().Where(p => p.CommodityOrderId == order.Id).ToList();
            if (orderitem.Count() > 0)
            {
                foreach (var item in orderitem)
                {
                    //获取到商品CommodityId
                    var jdorderItem = JdOrderItem.ObjectSet().FirstOrDefault(p => p.TempId == item.CommodityId && p.State == 1 && p.CommodityOrderId == orderId);
                    if (jdorderItem != null)
                    {
                        //取消京东预占信息
                        bool falg = JdHelper.OrderCancel(jdorderItem.JdPorderId);
                        if (falg == true)
                        {
                            //删除 JdOrderItem
                            jdorderItem.EntityState = EntityState.Deleted;
                            contextSession.Delete(item);
                            //增加日志表操作的内容
                            JdJournal jdjournal = new JdJournal()
                            {
                                Id = Guid.NewGuid(),
                                JdPorderId = jdorderItem.JdPorderId,
                                TempId = jdorderItem.TempId,
                                JdOrderId = jdorderItem.JdOrderId,
                                MainOrderId = jdorderItem.MainOrderId,
                                CommodityOrderId = jdorderItem.CommodityOrderId,
                                Name = "京东确认取消订单",
                                Details = "48小时订单未付款删除JdOrderItem表中相应的内容",
                                SubTime = DateTime.Now
                            };
                            jdjournal.EntityState = EntityState.Added;
                            contextSession.SaveObject(jdjournal);
                            contextSession.SaveChanges();

                        }
                    }
                }

            }
        }

        /// <summary>
        /// 处理的退款处理订单 n天内未响应 交易状态变为 7 已退款
        /// </summary>
        public void AutoDaiRefundOrderExt()
        {
            LogHelper.Info(string.Format("处理10天内未响应订单服务开始"));
            //处理订单状态为已退款
            try
            {
                List<Commodity> needRefreshCacheCommoditys = new List<Commodity>();
                List<TodayPromotion> needRefreshCacheTodayPromotions = new List<TodayPromotion>();

                List<int> directArrivalPayments = new PaySourceSV().GetDirectArrivalPaymentExt();
                List<int> secTranPayments = new PaySourceSV().GetSecuriedTransactionPaymentExt();
                List<int> stwog = new PaySourceSV().GetSecTransWithoutGoldPaymentExt();

                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                DateTime now = DateTime.Now;
                //查询10天内 商家未处理的待发货退款订单
                DateTime lastday = now.AddDays(-10);
                //屏蔽直接到账的订单
                var ordersQuery = CommodityOrder.ObjectSet().Where(n => n.State == 8 && n.RefundTime < lastday);
                if (!CustomConfig.IsSystemDirectRefund)
                {
                    ordersQuery = ordersQuery.Where(c => !directArrivalPayments.Contains(c.Payment));
                }
                var orders = ordersQuery.ToList();

                int oldstate = 0;
                LogHelper.Info(string.Format("处理10天内 商家未处理的待发货退款订单服务处理订单数:{0}", orders.Count));
                if (orders.Count > 0)
                {
                    List<Guid> orderIds = orders.Select(t => t.Id).ToList();
                    List<int> refundState = new List<int> { 0, 10, 12 };
                    var orderRefunds = OrderRefund.ObjectSet().Where(t => orderIds.Contains(t.OrderId) && t.State == 0).ToList();
                    foreach (CommodityOrder order in orders)
                    {
                        //退款申请
                        OrderRefund orderRefund = orderRefunds.Where(t => t.OrderId == order.Id).FirstOrDefault();
                        decimal orRefundMoney = orderRefund.RefundMoney;
                        //之前的整单退款逻辑
                        if (orderRefund.OrderItemId == null || orderRefund.OrderItemId == Guid.Empty)
                        {
                            oldstate = order.State;
                            //加库存
                            var orderitemlist = OrderItem.ObjectSet().Where(n => n.CommodityOrderId == order.Id);
                            UserLimited.ObjectSet().Context.ExecuteStoreCommand("delete from UserLimited where CommodityOrderId='" + order.Id + "'");
                            foreach (OrderItem items in orderitemlist)
                            {
                                Guid comId = items.CommodityId;
                                Commodity com = Commodity.ObjectSet().Where(n => n.Id == comId).First();
                                com.EntityState = System.Data.EntityState.Modified;
                                com.Stock += items.Number;
                                contextSession.SaveObject(com);
                                needRefreshCacheCommoditys.Add(com);
                                // 赠品库存
                                OrderEventHelper.AddStock(items, needRefreshCacheCommoditys);

                                if (items.Intensity != 10 || items.DiscountPrice != -1)
                                {
                                    TodayPromotion to = TodayPromotion.ObjectSet().Where(n => n.CommodityId == comId && n.EndTime > now && n.StartTime < now).FirstOrDefault();
                                    if (to != null)
                                    {
                                        to.SurplusLimitBuyTotal = to.SurplusLimitBuyTotal - items.Number;
                                        to.EntityState = System.Data.EntityState.Modified;
                                        contextSession.SaveObject(to);
                                        needRefreshCacheTodayPromotions.Add(to);

                                        PromotionItems pti = PromotionItems.ObjectSet().Where(n => n.PromotionId == to.PromotionId && n.CommodityId == comId).FirstOrDefault();
                                        pti.SurplusLimitBuyTotal = pti.SurplusLimitBuyTotal - items.Number;
                                        pti.EntityState = System.Data.EntityState.Modified;
                                        contextSession.SaveObject(pti);
                                    }
                                }

                            }
                            var tradeType = PaySource.GetTradeType(order.Payment);
                            //如果是金币支付，调用金币退款接口
                            if (order.RealPrice > 0 && (tradeType == TradeTypeEnum.SecTrans && order.GoldPrice > 0 || tradeType == TradeTypeEnum.Direct && CustomConfig.IsSystemDirectRefund))
                            {
                                Jinher.AMP.App.Deploy.CustomDTO.AppIdOwnerIdTypeDTO applicationDTO = APPSV.Instance.GetAppOwnerInfo(order.AppId);
                                if (applicationDTO == null || applicationDTO.OwnerId == Guid.Empty)
                                    continue;

                                decimal coupon_price = 0;
                                var user_yjcoupon = YJBSV.GetUserYJCouponByOrderId(order.Id);
                                if (user_yjcoupon.Data != null)
                                {
                                    foreach (var item in user_yjcoupon.Data)
                                    {
                                        coupon_price += item.UseAmount ?? 0;
                                    }
                                }
                                decimal yjbprice = 0;
                                var yjbresult = YJBSV.GetOrderYJBInfo(order.EsAppId.Value, order.Id);
                                if (yjbresult.Data != null)
                                {
                                    yjbprice = yjbresult.Data.InsteadCashAmount;
                                }
                                //orRefundMoney = orRefundMoney - coupon_price - yjbprice;//退款金币去除抵用券金额
                                var cashmoney = order.RealPrice.Value - order.Freight;
                                //orRefundMoney = (orderRefund.OrderRefundMoneyAndCoupun ?? 0) > cashmoney ? cashmoney : (orderRefund.OrderRefundMoneyAndCoupun ?? 0);
                                if (orderRefund.OrderRefundMoneyAndCoupun == null)
                                {//老的退款数据
                                    orRefundMoney = orderRefund.RefundMoney > cashmoney ? cashmoney : orderRefund.RefundMoney;
                                }
                                else
                                {
                                    orRefundMoney = (orderRefund.OrderRefundMoneyAndCoupun ?? 0) > cashmoney ? cashmoney : (orderRefund.OrderRefundMoneyAndCoupun ?? 0);
                                }
                                var cancelPayDto = OrderSV.BuildCancelPayDTO(order, orRefundMoney, contextSession, applicationDTO);

                                //退款支付
                                Jinher.AMP.FSP.Deploy.CustomDTO.ReturnInfoDTO goldPayresult = Jinher.AMP.BTP.TPS.FSPSV.CancelPay(cancelPayDto, tradeType);
                                if (goldPayresult == null)
                                {
                                    continue;
                                }
                                if (goldPayresult.Code != 0)
                                {
                                    continue;
                                }
                                if (tradeType == TradeTypeEnum.Direct)
                                {
                                    if (goldPayresult.Message == "success")
                                    {
                                        order.State = 12;
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    if (goldPayresult.Message == "success")
                                    {
                                        order.State = 7;
                                    }
                                    else
                                    {
                                        order.State = 12;
                                    }
                                }
                                LogHelper.Info(string.Format("退款返回值：{0},Message:{1},order.State:{2},   CancelPayDTO:{3}", goldPayresult.Code, goldPayresult.Message, order.State, JsonHelper.JsonSerializer(cancelPayDto)));
                            }
                            else
                            {
                                //更新订单状态
                                order.State = 7;
                            }
                            //回退积分
                            SignSV.CommodityOrderRefundScore(contextSession, order, orderRefund);

                            // 回退易捷币
                            //Jinher.AMP.BTP.TPS.Helper.YJBHelper.OrderRefund(contextSession, order, orderRefund);
                            #region 回退易捷币和易捷抵用券
                            decimal couponprice = 0;
                            decimal couponmoney = 0;//抵用券使用总金额
                            //decimal totalcouprice = 0;//保存抵用券之和(当2个或者2个以上的抵用券相加的时候)
                            //decimal pretotalcouprice = 0;//保存上一个抵用券相加的价格(当2个或者2个以上的抵用券相加的时候)
                            bool isexistsplit = false;//是否有拆单，如果有的话，整单退款的时候，退的是抵用券的使用金额
                            var issplit = MainOrder.ObjectSet().Where(x => x.SubOrderId == order.Id).FirstOrDefault();
                            if (issplit != null)
                            {
                                isexistsplit = true;
                            }

                            var useryjcoupon = YJBSV.GetUserYJCouponByOrderId(order.Id);
                            if (useryjcoupon.Data != null && useryjcoupon.Data.Count > 0)
                            {
                                useryjcoupon.Data = useryjcoupon.Data.OrderBy(x => x.Price).ToList();
                                var refundmoney = (orderRefund.OrderRefundMoneyAndCoupun ?? 0);

                                foreach (var item in useryjcoupon.Data)
                                {
                                    couponmoney += item.UseAmount ?? 0;
                                }
                                if (refundmoney == couponmoney + orRefundMoney)
                                {//全额退
                                    for (int i = 0; i < useryjcoupon.Data.Count; i++)
                                    {
                                        if (useryjcoupon.Data[i] != null)
                                        {
                                            if (isexistsplit)
                                                couponprice = useryjcoupon.Data[i].UsePrice;
                                            else
                                                couponprice = useryjcoupon.Data[i].Price;
                                            if (i == 0)
                                            {

                                            }
                                            else
                                            {//易捷币不能循环退
                                                orderRefund.RefundYJBMoney = 0;
                                            }
                                            decimal coupon = couponprice;
                                            Jinher.AMP.BTP.TPS.Helper.YJBHelper.OrderRefund(contextSession, order, orderRefund, coupon, useryjcoupon.Data[i].Id);
                                        }
                                    }
                                }
                                else
                                {

                                    for (int i = 0; i < useryjcoupon.Data.Count; i++)
                                    {
                                        if (useryjcoupon.Data[i] != null)
                                        {
                                            if (isexistsplit)
                                                couponprice = useryjcoupon.Data[i].UsePrice;
                                            else
                                                couponprice = useryjcoupon.Data[i].Price;
                                            //totalcouprice += useryjcoupon.Data[i].Price;
                                            if (i == 0)
                                            {
                                                //pretotalcouprice = couponprice;
                                            }
                                            else
                                            {//易捷币不能循环退
                                                orderRefund.RefundYJBMoney = 0;
                                            }
                                            decimal coupon = 0;
                                            if (refundmoney - orRefundMoney > 0)
                                            {
                                                if (refundmoney - orRefundMoney - couponprice >= 0)
                                                {//退款金额大于等于（实际支付金额+抵用券金额），直接返回抵用券面值
                                                 //if (refundmoney - order.RealPrice - couponprice - orderRefund.RefundYJBMoney < 0)
                                                 //{//返还部分易捷币
                                                 //    orderRefund.RefundYJBMoney = orderRefund.RefundYJBMoney - (refundmoney + order.RealPrice.Value + couponprice);
                                                 //}
                                                    coupon = couponprice;
                                                }
                                                else
                                                {//否则，表示不是全额退款，返还部分抵用券，易捷币返还0
                                                    coupon = refundmoney - orRefundMoney;
                                                    orderRefund.RefundYJBMoney = 0;
                                                }
                                                Jinher.AMP.BTP.TPS.Helper.YJBHelper.OrderRefund(contextSession, order, orderRefund, coupon, useryjcoupon.Data[i].Id);
                                                refundmoney -= coupon;
                                                //pretotalcouprice = totalcouprice;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Jinher.AMP.BTP.TPS.Helper.YJBHelper.OrderRefund(contextSession, order, orderRefund, 0, Guid.Empty);
                            }

                            #endregion

                            // 更新结算项
                            Jinher.AMP.BTP.TPS.Helper.SettleAccountHelper.OrderRefund(contextSession, order, orderRefund);

                            order.ConfirmTime = now;
                            order.ModifiedOn = now;
                            order.EntityState = System.Data.EntityState.Modified;

                            orderRefund.ModifiedOn = DateTime.Now;
                            if (order.State == 7)
                            {
                                orderRefund.State = 1;
                            }
                            else if (order.State == 12)
                            {
                                orderRefund.State = 12;
                            }
                            orderRefund.EntityState = System.Data.EntityState.Modified;
                            contextSession.SaveChanges();

                            if (needRefreshCacheCommoditys.Any())
                                needRefreshCacheCommoditys.ForEach(c => c.RefreshCache(EntityState.Modified));
                            if (needRefreshCacheTodayPromotions.Any())
                                needRefreshCacheTodayPromotions.ForEach(c => c.RefreshCache(EntityState.Modified));


                            try
                            {
                                //订单日志
                                Journal journal = new Journal();
                                journal.Id = Guid.NewGuid();
                                journal.Name = "系统处理10天内商家未响应，自动达成同意退款申请协议订单";
                                journal.Code = order.Code;
                                journal.SubId = order.UserId;
                                journal.SubTime = now;
                                journal.Details = "订单状态由" + oldstate + "变为" + order.State;
                                journal.StateFrom = oldstate;
                                journal.StateTo = order.State;
                                journal.IsPush = false;
                                journal.OrderType = order.OrderType;
                                journal.CommodityOrderId = order.Id;
                                journal.EntityState = System.Data.EntityState.Added;
                                contextSession.SaveObject(journal);
                                contextSession.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                LogHelper.Error("系统处理10天内商家未响应，自动达成同意退款申请协议订单记日志异常。", ex);
                            }

                            //添加消息
                            AddMessage addmassage = new AddMessage();
                            string odid = order.Id.ToString();
                            string usid = order.UserId.ToString();
                            string type = "order";
                            Guid EsAppId = order.EsAppId.HasValue ? order.EsAppId.Value : order.AppId;
                            addmassage.AddMessages(odid, usid, EsAppId, order.Code, order.State, "", type);

                            //易捷北京自营商品申请开电子发票（红票）
                            Guid eesAppId = new Guid(CustomConfig.InvoiceAppId);
                            //易捷北京的自营或者门店自营
                            MallApply mallApply = MallApply.ObjectSet().FirstOrDefault(t => t.EsAppId == eesAppId && t.AppId == order.AppId && (t.Type == 0 || t.Type == 2 || t.Type == 3));
                            if (mallApply != null)
                            {
                                var invoice = Invoice.ObjectSet().FirstOrDefault(t => t.CommodityOrderId == order.Id);
                                if (invoice != null && invoice.Category == 2)
                                {
                                    TPS.Invoic.InvoicManage invoicManage = new TPS.Invoic.InvoicManage();
                                    invoicManage.CreateInvoic(contextSession, order, 1);
                                    contextSession.SaveChanges();
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error("系统处理10天内商家未响应，自动达成同意退款申请协议订单服务异常。", ex);
            }
        }

        /// <summary>
        /// 解冻参数获取
        /// </summary>
        private ConfirmPayDTO buildConfirmPayUnFreezeDTO(ContextSession contextSession, CommodityOrder commodityOrder, Jinher.AMP.App.Deploy.CustomDTO.AppIdOwnerIdTypeDTO applicationDTO, string password = null)
        {
            ConfirmPayDTO comConfirmPayDto = new ConfirmPayDTO
            {
                BizId = commodityOrder.Id,
                Password = password,
                Sign = CustomConfig.PaySing,
                UserId = commodityOrder.UserId,
                AppId = commodityOrder.AppId,
                PayeeId = applicationDTO.OwnerId
            };

            return comConfirmPayDto;
        }

        /// <summary>
        /// 处理10天内商家未响应，自动达成同意退款/退货申请协议订 交易状态变为 10 
        /// </summary>
        public void AutoYiRefundOrderExt()
        {
            LogHelper.Info(string.Format("10天内商家未响应，自动达成同意退款/退货申请协议订单服务开始"));
            //处理订单状态为商家同意退款 但是没有收到卖家发过来的货
            try
            {
                List<int> directArrivalPayments = new PaySourceSV().GetDirectArrivalPaymentExt();

                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                DateTime now = DateTime.Now;
                //查询10天内商家未响应，自动达成同意退款/退货申请协议订单
                DateTime lastday = now.AddDays(-10);
                var orderList = (from c in CommodityOrder.ObjectSet()
                                 join r in OrderRefund.ObjectSet()
                                  on c.Id equals r.OrderId
                                 where c.State == 9 && c.RefundTime < lastday && !directArrivalPayments.Contains(c.Payment) && r.State == 0 && r.RefundType == 1
                                 select new
                                 {
                                     commodityOrder = c,
                                     orderRefund = r
                                 }).ToList();
                if (!orderList.Any())
                    return;

                //要处理的订单
                var orders = orderList.Select(t => t.commodityOrder).ToList();
                //申请退款列表
                var orderRefundList = orderList.Select(t => t.orderRefund).ToList();

                int oldstate = 0;
                LogHelper.Info(string.Format("处理10天内商家未响应，自动达成同意退款/退货申请协议订单服务处理订单数:{0}", orders.Count));
                if (orders.Count > 0)
                {
                    foreach (CommodityOrder order in orders)
                    {
                        LogHelper.Info(string.Format("[售中]处理10天内商家未响应，自动达成同意退款/退货申请协议订单，订单Id:{0}", order.Id));

                        var orderRefund = orderRefundList.Where(t => t.OrderId == order.Id).FirstOrDefault();

                        oldstate = order.State;
                        //更新订单状态                       
                        order.State = 10;
                        order.AgreementTime = now;
                        //order.ConfirmTime = now;
                        order.ModifiedOn = now;
                        order.EntityState = System.Data.EntityState.Modified;

                        if (oldstate == 14)
                        {
                            orderRefund.AgreeFlag = 0;
                        }
                        else
                        {
                            orderRefund.AgreeFlag = 1;
                        }
                        orderRefund.State = 10;
                        orderRefund.RefuseTime = DateTime.Now;
                        orderRefund.ModifiedOn = DateTime.Now;
                        orderRefund.EntityState = System.Data.EntityState.Modified;

                        contextSession.SaveChanges();
                        //发送消息，异步执行
                        //System.Threading.ThreadPool.QueueUserWorkItem(
                        //    a =>
                        //    {
                        try
                        {
                            //订单日志
                            Journal journal = new Journal();
                            journal.Id = Guid.NewGuid();
                            journal.Name = "系统处理10天内商家未响应，自动达成同意退款/退货申请协议订单";
                            journal.Code = order.Code;
                            journal.SubId = order.UserId;
                            journal.SubTime = now;
                            journal.Details = "订单状态由" + oldstate + "变为10";
                            journal.StateFrom = oldstate;
                            journal.StateTo = 10;
                            journal.IsPush = false;
                            journal.OrderType = order.OrderType;
                            journal.CommodityOrderId = order.Id;
                            journal.EntityState = System.Data.EntityState.Added;
                            contextSession.SaveObject(journal);
                            contextSession.SaveChanges();

                            //添加消息
                            AddMessage addmassage = new AddMessage();
                            string odid = order.Id.ToString();
                            string usid = order.UserId.ToString();
                            string type = "order";
                            Guid EsAppId = order.EsAppId.HasValue ? order.EsAppId.Value : order.AppId;
                            addmassage.AddMessages(odid, usid, EsAppId, order.Code, order.State, "", type);
                            ////正品会发送消息
                            //if (new ZPHSV().CheckIsAppInZPH(order.AppId))
                            //{
                            //    addmassage.AddMessages(odid, usid, CustomConfig.ZPHAppId, order.Code, order.State, "", type);
                            //}

                        }

                        catch (Exception ex)
                        {
                            LogHelper.Error(string.Format("系统处理10天内商家未响应，自动达成同意退款/退货申请协议订单记日志异常。"), ex);
                        }
                        //});

                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error("系统处理10天内商家未响应，自动达成同意退款/退货申请协议订单服务异常。", ex);
            }
        }
        /// <summary>
        /// 删除订单
        /// </summary>
        /// <param name="commodityorderId"></param>
        /// <param name="IsDel"></param>
        /// <returns></returns>
        public ResultDTO DelOrderExt(Guid commodityorderId, int IsDel)
        {
            if (commodityorderId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数不能为空" };
            }
            if (IsDel != 1 && IsDel != 2)
            {
                return new ResultDTO { ResultCode = 1, Message = "订单删除状态不是有效值" };
            }
            if (!OrderSV.LockOrder(commodityorderId))
            {
                return new ResultDTO { ResultCode = 110, Message = "操作失败" };
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var com = CommodityOrder.ObjectSet().Where(n => n.Id == commodityorderId).FirstOrDefault();
                if (com == null)
                {
                    return new ResultDTO { ResultCode = 4, Message = "订单不存在" };
                }
                if (IsDel == 1 && com.IsDel == 1)
                {
                    return new ResultDTO { ResultCode = 2, Message = "此订单已经被卖家删除" };
                }
                if (IsDel == 2 && com.IsDel == 2)
                {
                    return new ResultDTO { ResultCode = 3, Message = "此订单已经被商家删除" };
                }
                if ((com.IsDel == 1 && IsDel == 2) || (com.IsDel == 2 && IsDel == 1))
                {
                    com.IsDel = 3;
                }
                else
                {
                    com.IsDel = IsDel;
                }
                com.EntityState = System.Data.EntityState.Modified;
                //contextSession.SaveObject(com);

                //订单日志
                Journal journal = new Journal();
                journal.Id = Guid.NewGuid();
                if (IsDel == 2)
                {
                    journal.Name = "商家删除订单";
                }
                else if (IsDel == 1)
                {
                    journal.Name = "客户删除订单";
                }
                else if (IsDel == 3)
                {
                    journal.Name = "客户商家同时删除订单";
                }
                journal.Code = com.Code;
                journal.SubId = this.ContextDTO.LoginUserID;
                journal.SubTime = DateTime.Now;
                journal.Details = "订单状态为" + com.State;
                journal.StateTo = com.State;
                journal.IsPush = false;
                journal.OrderType = com.OrderType;
                journal.CommodityOrderId = commodityorderId;
                journal.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(journal);

                int reslult = contextSession.SaveChanges();
                if (reslult > 0)
                {
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                }
                else
                {
                    return new ResultDTO { ResultCode = 1, Message = "删除订单失败" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("删除订单。commodityorderId：{0}", commodityorderId.ToString()), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(commodityorderId);
            }
        }
        /// <summary>
        /// 退款的时候 处理众筹相关的业务
        /// </summary>
        /// <param name="contextSession"></param>
        /// <param name="commodityOrder"></param>
        /// <param name="orRefundMoney"></param>
        /// <param name="now"></param>
        public long CancelCrowdfundingExt(ContextSession contextSession, CommodityOrder commodityOrder, decimal orRefundMoney, DateTime now)
        {
            long gold = 0;
            //众筹股东表
            var UserCrowdfundingQuery = UserCrowdfunding.ObjectSet().FirstOrDefault(q => q.UserId == commodityOrder.UserId && q.AppId == commodityOrder.AppId);
            //众筹成功
            if (commodityOrder.IsCrowdfunding == 2)
            {
                //不是众筹
                if (UserCrowdfundingQuery == null)
                    return gold;
                //众筹成功，且当前订单
                UserCrowdfundingQuery.OrdersMoney -= orRefundMoney;
                //全额退款，减去订单
                if (commodityOrder.IsModifiedPrice)
                {
                    //如果退款金额等于订单金额 相当于全额退款 订单数量减1
                    if (orRefundMoney == commodityOrder.RealPrice)
                    {
                        UserCrowdfundingQuery.OrderCount = UserCrowdfundingQuery.OrderCount - 1;
                    }
                }
                else
                {
                    //如果退款的金额大于 不计算运费的金额的时候 按不算运费的金额计算 相当于全额退款 订单数量减1
                    if (orRefundMoney >= commodityOrder.Price)
                    {

                        UserCrowdfundingQuery.OrderCount = UserCrowdfundingQuery.OrderCount - 1;
                    }
                }
                UserCrowdfundingQuery.EntityState = EntityState.Modified;
                contextSession.SaveObject(UserCrowdfundingQuery);
                return gold;
            }

            //众筹基本表
            var CrowdfundingQuery = Crowdfunding.ObjectSet().FirstOrDefault(q => q.AppId == commodityOrder.AppId && q.StartTime < now);
            if (CrowdfundingQuery != null)
            {
                //众筹计数表 
                var CrowdfundingCountQuery = CrowdfundingCount.ObjectSet().FirstOrDefault(q => q.CrowdfundingId == CrowdfundingQuery.Id);
                //众筹股东表

                decimal corRealPrice = 0;
                decimal fenRealPrice = 0;


                decimal realPrice = commodityOrder.IsModifiedPrice
                                        ? commodityOrder.RealPrice.Value
                                        : commodityOrder.Price;

                if (commodityOrder.IsModifiedPrice)
                {
                    corRealPrice = orRefundMoney;
                    //如果退款金额等于订单金额 相当于全额退款 订单数量减1
                    if (orRefundMoney == commodityOrder.RealPrice)
                    {
                        UserCrowdfundingQuery.OrderCount = UserCrowdfundingQuery.OrderCount - 1;
                    }
                    else
                    {
                        //订单金额-退款 剩余部分 计算分红
                        fenRealPrice = commodityOrder.RealPrice.Value - orRefundMoney;
                    }
                }
                else
                {
                    //如果退款的金额大于 不计算运费的金额的时候 按不算运费的金额计算 相当于全额退款 订单数量减1
                    if (orRefundMoney >= commodityOrder.Price)
                    {
                        corRealPrice = commodityOrder.Price;
                        UserCrowdfundingQuery.OrderCount = UserCrowdfundingQuery.OrderCount - 1;
                    }
                    else
                    {
                        //订单金额-退款 剩余部分 计算分红
                        fenRealPrice = commodityOrder.Price - orRefundMoney;
                        corRealPrice = orRefundMoney;
                    }
                }
                //解决众筹临界点订单退款问题，如果退款后众筹有效金额仍未改变，则不改变股东表信息
                decimal notCfMoney = realPrice - corRealPrice - commodityOrder.CrowdfundingPrice;
                if (notCfMoney < 0)
                {
                    var notCfMoneyless = -notCfMoney;
                    decimal afterMoney = UserCrowdfundingQuery.Money - notCfMoneyless;
                    long afterShareCnt = (long)(afterMoney / CrowdfundingQuery.PerShareMoney);
                    //用户减少的股数
                    long result = UserCrowdfundingQuery.CurrentShareCount - afterShareCnt;
                    if (result > 0)
                    {
                        // 如果众筹已经成功 变成进行中
                        if (CrowdfundingQuery.State == 1)
                        {
                            CrowdfundingQuery.State = 0;
                            CrowdfundingQuery.EntityState = System.Data.EntityState.Modified;
                            contextSession.SaveObject(CrowdfundingQuery);
                        }
                        CrowdfundingCountQuery.CurrentShareCount = CrowdfundingCountQuery.CurrentShareCount - result;
                        CrowdfundingCountQuery.EntityState = System.Data.EntityState.Modified;
                        contextSession.SaveObject(CrowdfundingCountQuery);
                    }
                    else
                    {
                        result = 0;
                    }

                    UserCrowdfundingQuery.Money = afterMoney;
                    UserCrowdfundingQuery.CurrentShareCount -= result;
                    UserCrowdfundingQuery.EntityState = System.Data.EntityState.Modified;



                    if (commodityOrder.CrowdfundingPrice - orRefundMoney < 0)
                    {
                        commodityOrder.CrowdfundingPrice = 0;
                    }
                    else
                    {
                        commodityOrder.CrowdfundingPrice = commodityOrder.CrowdfundingPrice - orRefundMoney;
                    }
                }
                UserCrowdfundingQuery.OrdersMoney -= corRealPrice;
                contextSession.SaveObject(UserCrowdfundingQuery);


                if (fenRealPrice > 0)
                {
                    //众筹股东每日统计
                    var yestorday = DateTime.Today.AddDays(-1);
                    var UserCrowdfundingDailyQuery = UserCrowdfundingDaily.ObjectSet().Where(q => q.AppId == commodityOrder.AppId && q.SettlementDate == yestorday).FirstOrDefault();

                    if (UserCrowdfundingDailyQuery != null)
                    {
                        gold = (long)(UserCrowdfundingDailyQuery.ShareCount * CrowdfundingQuery.DividendPercent * fenRealPrice * 1000);
                        if (gold > 0)
                        {
                            CfOrderDividend cfOrderDividend = CfOrderDividend.CreateCfOrderDividend();
                            cfOrderDividend.Gold = gold;
                            cfOrderDividend.AppId = commodityOrder.AppId;
                            cfOrderDividend.State = 0;
                            cfOrderDividend.CurrentShareCount = UserCrowdfundingDailyQuery.ShareCount;
                            cfOrderDividend.CommodityOrderId = commodityOrder.Id;
                            contextSession.SaveObject(cfOrderDividend);
                        }

                    }
                }
            }
            return gold;
        }


        /// <summary>
        /// 保存商品订单 --- 厂家直销，一次购买多个App
        /// </summary>
        /// <param name="orderSDTO"></param>
        /// <returns></returns>
        public SetOrderResultDTO SaveSetCommodityOrderExt(OrderSDTO orderSDTO)
        {
            LogHelper.Error("CommoditySVExt.SaveSetCommodityOrderExt NotImplementedException");
            throw new NotImplementedException();
        }

        /// <summary>
        /// 保存商品订单 --- 厂家直销，一次购买多个App
        /// </summary>
        /// <param name="orderList"></param>
        /// <returns></returns>
        public SetOrderResultDTO SaveSetCommodityOrderNewExt(List<OrderSDTO> orderList)
        {
            if (orderList == null || orderList.Count == 0)
            {
                return new SetOrderResultDTO { ResultCode = 1, Message = "订单商品不能为空" };
            }
            //只有代运营的才可用优惠券与代金券
            if (CustomConfig.CouponFlag == 1)
            {
                foreach (OrderSDTO orderSDTO in orderList)
                {
                    if (orderSDTO.CouponId != Guid.Empty || !(string.IsNullOrWhiteSpace(orderSDTO.PaycouponCodes) || orderSDTO.PaycouponCodes == "null"))
                    {
                        if (!Jinher.AMP.BTP.TPS.ZPHSV.Instance.CheckIsAppInZPH(orderSDTO.AppId))
                        {
                            return new SetOrderResultDTO { ResultCode = 1, Message = "只有代运营的商品才能使用优惠券与代金券" };
                        }
                    }
                }
            }
            //合并支付不能“积分抵现”
            int esAppIdCount = orderList.Select(o => o.EsAppId).Distinct().Count();
            int scount = (from o in orderList where o.ScorePrice > 0 select o).Count();
            if (esAppIdCount > 0 && scount > 0)
            {
                return new SetOrderResultDTO { ResultCode = 1, Message = "订单不能使用积分抵现，请重新下订单" };
            }

            LogHelper.Info(string.Format("下订单SaveCommodityOrderExt：当前时间:{0}, orderList={1}", DateTime.Now, JsonHelper.JsonSerializer(orderList)), "BTP_Order");
            string payUrl = string.Empty;
            var mainId = Guid.NewGuid();
            List<MyOrderResultDTO> listMy = new List<MyOrderResultDTO>();
            var ret = new SetOrderResultDTO { ResultCode = 0, Message = "下订单成功", MainOrderId = mainId, OrderInfo = listMy };

            //此变量用来进行原MorePayPrice操作的输入类
            List<MainOrdersDTO> mainOrdersDTOList = new List<MainOrdersDTO>();
            //多app支付
            bool isMultiPay = true;

            //金币、代金券每条数据上保存的都是整个订单所能使用的总金额，不用各订单sum汇总。
            var goldPrice = orderList[0].GoldPrice;
            var payCouponValue = orderList[0].PayCouponValue;
            var paycouponCodes = orderList[0].PaycouponCodes;
            //跨店满减 加上，最后跳转的时候，会把跨店满减的值减去。
            //qgb payCouponValue += orderList[0].StoreCouponCommdityPrice > orderList[0].StoreCouponPrice ? orderList[0].StoreCouponPrice : orderList[0].StoreCouponCommdityPrice;

            var payment = orderList[0].Payment;

            //foreach (var order in orderList)
            //{ 
            //    foreach (var commdity in order.ShoppingCartItemSDTO)
            //    {
            //        payCouponValue += commdity.StoreCouponDivide;
            //    }
            //}//qgb

            bool isMixPay = (goldPrice > 0 || payCouponValue > 0);

            if (string.IsNullOrEmpty(orderList[0].ReceiptUserName) || string.IsNullOrEmpty(orderList[0].ReceiptPhone))
            {
                var tuple = CBCSV.GetUserNameAndCode(orderList[0].UserId);
                foreach (var orderSdto in orderList)
                {
                    orderSdto.ReceiptUserName = tuple.Item1;
                    orderSdto.ReceiptPhone = tuple.Item2;
                }
            }

            List<PayItem> yjCardsList = orderList[0].YjCards;
            if (yjCardsList != null && yjCardsList.Any())
            {
                //调用易捷卡查询接口验证余额是否发生变化。
                ResourceVerifyer resVerifyer = new ResourceVerifyer();
                ResultDTO yjcResult = resVerifyer.ValidYjCardBalance(yjCardsList);
                if (yjcResult.ResultCode != 0)
                {
                    return new SetOrderResultDTO { ResultCode = yjcResult.ResultCode, Message = yjcResult.Message };
                }
            }

            List<string> errorMessList = new List<string>();
            //long hasUseYJBCount = 0;
            decimal hasUseYJBPrice = 0;
            decimal hasUseYJCouponPrice = 0;

            //已使用的易捷卡金额。
            decimal hasUseYJCardPrice = 0;

            var yjInfos = new Dictionary<Guid, YJB.Deploy.CustomDTO.CanInsteadCashDTO>();

            foreach (var order in orderList)
            {
                order.MainOrderId = mainId;
                if (/*order.YJBCount > 0 ||*/ order.YJBPrice > 0 || order.YJCouponPrice > 0)
                {
                    var yjInfo = YJBHelper.GetCommodityCashPercent(order.EsAppId, new Jinher.AMP.YJB.Deploy.CustomDTO.OrderInsteadCashInputDTO
                    {
                        UserId = order.UserId,
                        //AppId = orderSDTO.AppId,
                        YJCouponIds = order.YJCouponIds,
                        Commodities = order.ShoppingCartItemSDTO.Select(s => new
                        Jinher.AMP.YJB.Deploy.CustomDTO.OrderInsteadCashInputCommodityDTO
                        {
                            AppId = order.AppId,
                            Id = s.Id,
                            Price = s.RealPrice,
                            Number = s.CommodityNumber,
                        }).ToList()
                    });
                    yjInfos[order.AppId] = yjInfo;
                }
            }

            IEnumerable<Guid> oAppIds = orderList.Select(o => o.AppId).Distinct();
            Guid esAppId = orderList[0].EsAppId;
            var mallApplies = (from ma in MallApply.ObjectSet()
                               where ma.EsAppId == esAppId && oAppIds.Contains(ma.AppId)
                               select new { ma.AppId, ma.Type }).ToList();

            var selfAppIdList = mallApplies.Where(x => x.Type != 1).Select(y => y.AppId).Distinct();
            //自营商品总金额（含运费）。
            decimal totalSelfPrice = orderList.Where(_ => selfAppIdList.Contains(_.AppId)).Sum(x => x.RealPrice);

            //已经处理的自营订单数量。
            int a = 0;
            //已经处理的自营订单总金额（含运费）。
            decimal readyAmount = 0;

            //所有选中的易捷卡列表。
            List<PayItem> allCards = orderList[0].YjCards;

            //已处理的订单数量。
            int readyOrderCount = 0;
            foreach (var order in orderList)
            {

                // 调用购买单个App方法
                OrderResultDTO singleRet;
                Tuple<Guid, decimal> appIdRealPrice = null;

                // 调用购买单个App方法
                Jinher.AMP.YJB.Deploy.CustomDTO.OrderInsteadCashDTO yjbCheckResult = null;
                var yjInfo = yjInfos.ContainsKey(order.AppId) ? yjInfos[order.AppId] : null;
                if (yjInfo != null)
                {
                    if (/*order.YJBCount > 0 || */order.YJBPrice > 0)
                    {
                        yjbCheckResult = yjInfo.YJBInfo;
                        if (!yjbCheckResult.Enabled)
                        {
                            singleRet = new OrderResultDTO { ResultCode = 1, Message = "易捷币已禁用，请重新下单" };
                            goto CheckResult;
                        }

                        //order.YJBCount = Math.Min(order.YJBCount - hasUseYJBCount, yjbCheckResult.InsteadCashCount);
                        order.YJBPrice = Math.Min(order.YJBPrice - hasUseYJBPrice, yjbCheckResult.InsteadCashAmount);

                        //hasUseYJBCount += order.YJBCount;
                        hasUseYJBPrice += order.YJBPrice;
                        order.RealPrice -= order.YJBPrice;
                    }
                    else
                    {
                        order.YJBPrice = 0;
                    }

                    // 多APP，拆分易捷抵现劵
                    if (order.YJCouponPrice > 0)
                    {
                        var yjCouponInfo = yjInfo.YJCouponInfo;
                        if (yjCouponInfo != null && yjCouponInfo.YJCoupons.Count > 0)
                        {
                            var returnPrice = yjCouponInfo.YJCoupons.Sum(_ => _.Price);
                            var totalPrice = order.ShoppingCartItemSDTO.Sum(_ => _.RealPrice * _.CommodityNumber) - order.YJBPrice;
                            var usePrice = Math.Min(returnPrice, totalPrice);
                            order.YJCouponIds = yjCouponInfo.YJCoupons.Select(_ => _.Id).ToList();
                            order.YJCouponPrice = Math.Min(order.YJCouponPrice - hasUseYJCouponPrice, usePrice);
                            hasUseYJCouponPrice += order.YJCouponPrice;
                            order.RealPrice -= order.YJCouponPrice;
                        }
                        else
                        {
                            order.YJCouponIds = new List<Guid>();
                            order.YJCouponPrice = 0;
                        }
                    }
                }


                //易捷卡抵用金额，第三方店铺不可用易捷卡支付。
                if (order.YjCardPrice > 0
                    && order.YjCards != null && order.YjCards.Count > 0
                    && mallApplies != null && mallApplies.FirstOrDefault(x => x.AppId == order.AppId).Type != 1)
                {
                    if (a != selfAppIdList.Count() - 1)
                    {
                        //todo 若前台不是拆分好的数据，此处拆分要加上店铺类型的判断，第三方店铺不能用易捷卡。
                        order.YjCardPrice = decimal.Round(order.RealPrice / totalSelfPrice * order.YjCardPrice, 2);
                        readyAmount += order.YjCardPrice;
                    }
                    else
                    {
                        order.YjCardPrice = decimal.Round(totalSelfPrice - readyAmount, 2);
                    }
                    order.RealPrice -= order.YjCardPrice;

                    SplitOrderYjCard(order);

                    a++;
                }
                else
                {
                    order.YjCards = new List<PayItem>();
                    order.YjCardPrice = 0;
                }

                singleRet = this.SubmitOrderCommon(order, ref appIdRealPrice, isMixPay, isMultiPay, yjbCheckResult);
            CheckResult:
                {
                    readyOrderCount++;
                    //如果订单提交成功
                    if (singleRet.ResultCode == 0)
                    {
                        ContextSession contextSession = ContextFactory.CurrentThreadContext;
                        MainOrder mainModel = new MainOrder();
                        mainModel.Id = Guid.NewGuid();
                        mainModel.AppId = order.AppId;
                        mainModel.MainOrderId = mainId;
                        mainModel.SubOrderId = singleRet.OrderId;
                        mainModel.SubTime = DateTime.Now;
                        mainModel.EntityState = System.Data.EntityState.Added;
                        contextSession.SaveObject(mainModel);
                        contextSession.SaveChanges();
                        MyOrderResultDTO mo = new MyOrderResultDTO();
                        mo.Freight = singleRet.Freight;
                        mo.OrderCode = singleRet.OrderCode;
                        mo.OrderId = singleRet.OrderId;
                        mo.RealPrice = appIdRealPrice.Item2;
                        mo.AppId = appIdRealPrice.Item1;
                        listMy.Add(mo);


                        MainOrdersDTO model = new MainOrdersDTO();
                        model.UserId = order.UserId;
                        model.AppId = order.AppId;
                        model.OrderId = singleRet.OrderId;
                        model.RealPrice = order.RealPrice;
                        mainOrdersDTOList.Add(model);
                    }
                    else
                    {
                        if (singleRet.ResultCode == 2 && (ret.ResultCode == 1 || ret.ResultCode == 0))
                        {
                            ret.ResultCode = 2;
                        }
                        else if (singleRet.ResultCode == 3)
                        {
                            ret.ResultCode = 3;
                        }
                        else
                        {
                            ret.ResultCode = singleRet.ResultCode;
                            ret.Message = singleRet.Message;
                        }

                        if (!string.IsNullOrEmpty(singleRet.Message))
                        {
                            errorMessList.Add(singleRet.Message);
                        }
                        LogHelper.Error(string.Format("厂家直营多商品生成其中一个订单失败：AppId{0},UserId{1},Message:{2}", order.AppId, order.UserId, JsonConvert.SerializeObject(singleRet)));
                        //有一个错误，后边的子订单不在继续下单。
                        break;
                    }
                }
            }

            var json = SerializationHelper.JsonSerialize(new { MainOrderId = mainId, Request = orderList, Response = new { Success = listMy, Error = errorMessList } });
            RabbitMqHelper.Send(RabbitMqRoutingKey.OrderBatchCreateEnd, RabbitMqExchange.Order, json);

            //删除缓存
            JdOrderHelper.DeleteRedisJdPOrder(orderList.FirstOrDefault().UserId.ToString());



            List<Guid> subOrderIds = MainOrder.ObjectSet().Where(mo => mo.MainOrderId == mainId).Select(y => y.SubOrderId).Distinct().ToList();
            List<CommodityOrder> coQuery = null;
            List<OrderItem> oiQuery = null;
            if (subOrderIds != null && subOrderIds.Any())
            {
                coQuery = CommodityOrder.ObjectSet().Where(x => subOrderIds.Contains(x.Id)).ToList();
                oiQuery = OrderItem.ObjectSet().Where(y => subOrderIds.Contains(y.CommodityOrderId)).ToList();
            }

            if (ret.ResultCode != 0 && oiQuery != null && oiQuery != null)
            {
                #region 资源回滚

                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                foreach (CommodityOrder cox in coQuery)
                {
                    var oiInOneOrder = oiQuery.Where(oix => oix.CommodityOrderId == cox.Id).ToList();
                    RollbackOrderResource(cox, oiInOneOrder);

                    //todo  State = -1 未下单失败订单。
                    cox.State = -1;
                    cox.ModifiedOn = DateTime.Now;
                    cox.EntityState = EntityState.Modified;
                }
                int rollRowCount = contextSession.SaveChanges();

                #endregion

                ret.Message = string.Join("\r\n", errorMessList);
                return ret;
            }
            else if (ret.ResultCode == 0 && readyOrderCount == orderList.Count)
            {
                //最后一单对前面订单进行处理。
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                foreach (CommodityOrder cox in coQuery)
                {
                    if (cox.RealPrice <= 0)
                    {
                        NoThirdPartPay(cox);
                    }
                }
                int rollRowCount = contextSession.SaveChanges();
            }


            if (!listMy.Any())
                return new SetOrderResultDTO { ResultCode = 1, Message = string.Join("\r\n", errorMessList) };

            #region 全金币支付
            var realPrice = orderList.Sum(c => c.RealPrice);
            if (isMixPay && payment == 0 && (goldPrice + payCouponValue) >= realPrice)
            {


                ContextDTO contextDTO = this.ContextDTO;
                if (contextDTO == null)
                {
                    contextDTO = Jinher.AMP.BTP.Common.AuthorizeHelper.CoinInitAuthorizeInfo();
                }
                Jinher.AMP.App.Deploy.CustomDTO.AppIdOwnerIdTypeDTO appDTO = APPSV.Instance.GetAppOwnerInfo(orderList[0].AppId, contextDTO);

                if (appDTO == null || appDTO.OwnerId == Guid.Empty)
                {
                    ret.ResultCode = 2;
                    return ret;
                }

                var totalRealPrice = (from gp in orderList select gp.RealPrice).Sum();

                string payorComment = string.Format("{0}", orderList[0].ShoppingCartItemSDTO[0].Name);
                string notifyUrl = string.Format("{0}PaymentNotify/Goldpay", CustomConfig.BtpDomain);
                PayOrderGoldDTO payOrderDTO = new PayOrderGoldDTO();
                payOrderDTO.AppId = orderList[0].AppId;
                payOrderDTO.BizId = mainId;
                payOrderDTO.CouponCodes = orderList[0].PaycouponCodes;
                payOrderDTO.CouponCount = (double)orderList[0].PayCouponValue;
                payOrderDTO.Gold = (long)(orderList[0].GoldPrice * 1000);
                payOrderDTO.TotalCount = (long)(totalRealPrice * 1000);
                payOrderDTO.PayeeComment = payOrderDTO.PayorComment = payorComment;
                payOrderDTO.Password = orderList[0].PayPassword;
                payOrderDTO.NotifyUrl = notifyUrl;
                payOrderDTO.PayeeId = appDTO.OwnerId;

                List<Guid> appIds = listMy.Select(_ => _.AppId).Distinct().ToList();
                List<AppOwnerTypeDTO> appOwnerList = APPSV.Instance.GetAppOwnerTypeList(appIds);

                List<Jinher.AMP.FSP.Deploy.CustomDTO.SecuredTransDTO> listSecuredTrans = new List<FSP.Deploy.CustomDTO.SecuredTransDTO>();
                foreach (MyOrderResultDTO m in listMy)
                {
                    #region
                    //var result = APPSV.Instance.GetAppOwnerInfo(m.AppId, contextDTO);
                    //if (result != null)
                    //{
                    //    modelSecuredTrans.PayeeId = result.OwnerId;
                    //}
                    #endregion

                    Jinher.AMP.FSP.Deploy.CustomDTO.SecuredTransDTO modelSecuredTrans = new FSP.Deploy.CustomDTO.SecuredTransDTO();
                    var appOwner = appOwnerList.FirstOrDefault(_ => _.AppId == m.AppId);
                    if (appOwner != null)
                    {
                        modelSecuredTrans.PayeeId = appOwner.OwnerId;
                    }
                    modelSecuredTrans.BizId = m.OrderId;
                    modelSecuredTrans.Money = Convert.ToDouble(m.RealPrice);
                    modelSecuredTrans.AppId = m.AppId;
                    listSecuredTrans.Add(modelSecuredTrans);
                }


                var preDto = new FSP.Deploy.CustomDTO.PrePayBatchDTO();
                preDto.MainBizId = mainId;
                preDto.PayorId = orderList[0].UserId;
                preDto.NotifyUrl = notifyUrl;
                preDto.OrderList = listSecuredTrans;

                payOrderDTO.OrderList = preDto;
                try
                {
                    LogHelper.Info(string.Format("误入PayByPayeeIdBatch，payOrderDTO:{0},goldPrice:{1},payCouponValue:{2},realPrice:{3}",
                            JsonHelper.JsonSerializer(payOrderDTO), goldPrice, payCouponValue, realPrice));

                    FSP.Deploy.CustomDTO.ReturnInfoDTO<long> goldResult = Jinher.AMP.BTP.TPS.FSPSV.Instance.PayByPayeeIdBatch(payOrderDTO);
                    //commodityOrderDTO.Id, appDTO.OwnerId, (ulong)(commodityOrderDTO.RealPrice * 1000), payorComment, payorComment, orderSDTO.PayPassword, commodityOrderDTO.AppId, notifyUrl);
                    if (goldResult.Code == 0)
                    {
                        ret.ResultCode = 3;
                    }
                    else
                    {
                        ret.ResultCode = 2;
                        LogHelper.Error(string.Format("全金币支付接口异常。orderId：{0}，Message：{1}", payOrderDTO.BizId, goldResult.Message));
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(string.Format("全金币支付接口异常。orderId：{0}", payOrderDTO.BizId), ex);
                    ret.ResultCode = 2;
                }
            }
            else
            {
                List<Guid> appIds = mainOrdersDTOList.Select(_ => _.AppId).Distinct().ToList();
                List<AppOwnerTypeDTO> appOwnerList = APPSV.Instance.GetAppOwnerTypeList(appIds);

                //通知金币多app支付
                List<Jinher.AMP.FSP.Deploy.CustomDTO.SecuredTransDTO> listSecuredTrans = new List<FSP.Deploy.CustomDTO.SecuredTransDTO>();
                foreach (MainOrdersDTO m in mainOrdersDTOList)
                {
                    #region
                    //var result = APPSV.Instance.GetAppOwnerInfo(m.AppId, null);
                    //if (result != null)
                    //{
                    //    modelSecuredTrans.PayeeId = result.OwnerId;
                    //}
                    #endregion

                    Jinher.AMP.FSP.Deploy.CustomDTO.SecuredTransDTO modelSecuredTrans = new FSP.Deploy.CustomDTO.SecuredTransDTO();
                    var appOwner = appOwnerList.FirstOrDefault(_ => _.AppId == m.AppId);
                    if (appOwner != null)
                    {
                        modelSecuredTrans.PayeeId = appOwner.OwnerId;
                    }
                    modelSecuredTrans.BizId = m.OrderId;
                    modelSecuredTrans.Money = Convert.ToDouble(m.RealPrice);
                    modelSecuredTrans.AppId = m.AppId;
                    listSecuredTrans.Add(modelSecuredTrans);
                }
                if (listSecuredTrans.Count > 0)
                {
                    Jinher.AMP.FSP.Deploy.CustomDTO.PrePayBatchDTO preDto = new FSP.Deploy.CustomDTO.PrePayBatchDTO();
                    preDto.MainBizId = mainId;
                    preDto.PayorId = this.ContextDTO.LoginUserID;
                    preDto.NotifyUrl = CustomConfig.BtpDomain + "PaymentNotify/Goldpay";
                    preDto.OrderList = listSecuredTrans;

                    Jinher.AMP.FSP.Deploy.CustomDTO.ReturnInfoDTO<List<ChildTransactionStatusDTO>> resultGoldPay = null;
                    //担保交易
                    if (orderList[0].TradeType == 0)
                    {
                        LogHelper.Info(string.Format("批量支付预处理PrePayBatch。preDto：{0}", JsonHelper.JsonSerializer(preDto)));
                        resultGoldPay = Jinher.AMP.BTP.TPS.FSPSV.Instance.PrePayBatch(preDto);
                    } //直接到账
                    else if (orderList[0].TradeType == 1)
                    {
                        LogHelper.Info(string.Format("批量支付预处理PreDirectPayBatch。preDto：{0}", JsonHelper.JsonSerializer(preDto)));
                        resultGoldPay = Jinher.AMP.BTP.TPS.FSPSV.Instance.PreDirectPayBatch(preDto);
                    }

                    if (resultGoldPay == null)
                    {
                        LogHelper.Error(string.Format("批量支付预处理订单支付失败结果为Null"));
                    }
                    else if (resultGoldPay.Code == 0)
                    {
                        if (Convert.ToDecimal(resultGoldPay.ExtBag["TotalMoney"]) != 0)
                        {
                            LogHelper.Info(string.Format("批量支付预处理PrePayBatch成功。Moeny={0}，AppId={1}", Convert.ToDecimal(resultGoldPay.ExtBag["TotalMoney"]), listSecuredTrans[0].BizId));
                        }
                        else
                        {
                            LogHelper.Error(string.Format("批量支付预处理PrePayBatch失败ExtBag[\"TotalMoney\"]=0"));
                        }
                    }
                    else
                    {
                        LogHelper.Error(string.Format("批量支付预处理PrePayBatch失败Code!=0"));
                    }
                }

                CreateOrderToFspDTO ceateOrderToFspDTO = new Deploy.CustomDTO.CreateOrderToFspDTO();
                ceateOrderToFspDTO.GoldCoupon = payCouponValue;//money = RealPrice - GoldPrice - GoldCoupon;
                ceateOrderToFspDTO.GoldCouponIds = paycouponCodes;
                ceateOrderToFspDTO.GoldPrice = goldPrice;
                ceateOrderToFspDTO.OrderId = mainId;
                ceateOrderToFspDTO.RealPrice = realPrice;
                ceateOrderToFspDTO.Source = orderList[0].Source;
                ceateOrderToFspDTO.SrcType = orderList[0].SrcType;
                ceateOrderToFspDTO.WxOpenId = orderList[0].WxOpenId;
                ceateOrderToFspDTO.FirstCommodityName = orderList[0].ShoppingCartItemSDTO[0].Name;
                ceateOrderToFspDTO.AppId = orderList[0].AppId;
                ceateOrderToFspDTO.EsAppId = orderList[0].EsAppId;
                //ceateOrderToFspDTO.ExpireTime = orderList[0].ExpireTime;
                ceateOrderToFspDTO.TradeType = orderList[0].TradeType;
                ceateOrderToFspDTO.DealType = 0;
                ceateOrderToFspDTO.IsWeixinPay = orderList[0].IsWeixinPay;
                ceateOrderToFspDTO.OrderType = orderList[0].OrderType;
                ceateOrderToFspDTO.Scheme = orderList[0].Scheme;
                payUrl = OrderSV.GetCreateOrderFSPUrl(ceateOrderToFspDTO, this.ContextDTO);
            }

            #endregion
            ret.PayUrl = payUrl;
            return ret;

        }

        /// <summary>
        /// 拆分订单中易捷卡使用金额
        /// </summary>
        /// <param name="order">订单</param>
        private void SplitOrderYjCard(OrderSDTO order)
        {
            //所有选中的易捷卡列表。
            List<PayItem> allCards = order.YjCards;
            if (allCards == null || !allCards.Any())
            {
                return;
            }

            #region 拆分易捷卡列表

            order.YjCards = new List<PayItem>();
            var shenyuCards = allCards.Where(x => x.Balance > 0);

            //该订单已经使用订单抵扣的金额。
            decimal je = 0;

            List<PayItem> orderPayItems = new List<PayItem>();
            foreach (var c in shenyuCards)
            {
                PayItem pi = new PayItem();
                pi.FillWith(c);
                orderPayItems.Add(pi);

                if (je + c.Balance >= order.YjCardPrice)
                {
                    pi.Amount = order.YjCardPrice - je;

                    //卡剩余可用金额 = 卡剩余可用金额 - （需要用这张卡抵扣的金额）
                    c.Balance = c.Balance - (order.YjCardPrice - je);
                    //本张卡还有余额，就不必用下一张卡了。
                    break;
                }
                else
                {
                    pi.Amount = c.Balance;

                    je += c.Balance;
                    c.Balance = 0;
                }
            }

            order.YjCards = orderPayItems;

            #endregion
        }


        /// <summary>
        /// 商家批量删除订单
        /// </summary>
        /// <returns></returns>
        public ResultDTO DeleteOrdersExt(List<Guid> list)
        {
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            try
            {


                //事务处理
                using (TransactionScope transaction = new TransactionScope())
                {
                    foreach (var orderId in list)
                    {
                        var query = CommodityOrder.ObjectSet().Where(n => n.Id == orderId).FirstOrDefault();
                        if (query != null)
                        {
                            query.IsDel = 2;
                            query.EntityState = EntityState.Modified;
                            contextSession.SaveObject(query);
                            Journal journal = new Journal();
                            journal.Id = Guid.NewGuid();
                            journal.Name = "商家删除订单";
                            journal.Code = query.Code;
                            journal.SubId = this.ContextDTO.LoginUserID;
                            journal.SubTime = DateTime.Now;
                            journal.Details = "订单状态为" + query.State;
                            journal.StateTo = query.State;
                            journal.IsPush = false;
                            journal.OrderType = query.OrderType;
                            journal.CommodityOrderId = orderId;
                            journal.EntityState = System.Data.EntityState.Added;
                            contextSession.SaveObject(journal);
                        }
                    }

                    int reslult = contextSession.SaveChanges();
                    //最后决定是否回滚
                    transaction.Complete();
                    if (reslult > 0)
                    {
                        return new ResultDTO { ResultCode = 0, Message = "Success" };
                    }
                    else
                    {
                        return new ResultDTO { ResultCode = 1, Message = "删除订单失败" };
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CommodityOrderSV-DeleteOrdersExt商家删除订单。list：{0}", string.Join(",", list.ToArray())), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }


        }
        /// <summary>
        /// 根据订单主表ID获取子订单信息
        /// </summary>
        /// <param name="MainOrderId"></param>
        /// <returns></returns>
        public List<MainOrdersDTO> GetMianOrderListExt(Guid MainOrderId)
        {
            List<MainOrdersDTO> mainOrders = (from m in MainOrder.ObjectSet()
                                              join c in CommodityOrder.ObjectSet() on m.SubOrderId equals c.Id
                                              where m.MainOrderId == MainOrderId
                                              select new MainOrdersDTO
                                              {
                                                  UserId = c.UserId,
                                                  AppId = c.AppId,
                                                  OrderId = c.Id,
                                                  RealPrice = c.RealPrice.Value
                                              }).ToList();

            return mainOrders;
        }
        /// <summary>
        /// 删除订单集合表
        /// </summary>
        /// <param name="SubOrderId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteMainOrderExt(Guid SubOrderId)
        {

            ResultDTO result = new ResultDTO();
            try
            {
                MainOrder sci = new MainOrder();
                var mian = MainOrder.ObjectSet().Where(n => n.SubOrderId == SubOrderId).FirstOrDefault();
                if (mian != null)
                {
                    ContextSession contextSession = ContextFactory.CurrentThreadContext;
                    mian.EntityState = System.Data.EntityState.Deleted;
                    contextSession.Delete(mian);
                    contextSession.SaveChange();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("删除订单集合表服务异常。SubOrderId：{0}", SubOrderId), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        /// <summary>
        /// 处理超时未支付订单
        /// </summary>
        /// <param name="orderId">订单Id</param>
        /// <returns></returns>
        public ResultDTO ExpirePayOrderExt(Guid orderId)
        {
            LogHelper.Info(string.Format("处理活动超时未付款订单服务开始，orderId={0}", orderId));
            //处理订单状态为超时交易关闭
            try
            {
                DateTime now = DateTime.Now;
                //查询超时未付款的订单 

                var order = CommodityOrder.ObjectSet().FirstOrDefault(c => c.Id == orderId && c.State == 0);
                if (order == null)
                    return new NewResultDTO { ResultCode = 0, Message = "Success" };


                var expirePay = OrderExpirePay.ObjectSet().FirstOrDefault(c => c.OrderId == orderId);
                if (expirePay == null)
                    return new NewResultDTO { ResultCode = 1, Message = "订单未设置超时时间" };

                if (expirePay.ExpirePayTime > now.AddMinutes(2))
                    return new NewResultDTO { ResultCode = 1, Message = "未到超时时间，设置失败" };

                if (noPayOrder(order, now))
                {
                    ContextSession contextSession = ContextFactory.CurrentThreadContext;
                    expirePay.State = 1;
                    expirePay.EntityState = EntityState.Modified;
                    contextSession.SaveChanges();
                    return new ResultDTO { ResultCode = 0, Message = "Success" }; ;
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error("处理活动超时未付款订单服务异常。", ex);
            }
            return new ResultDTO { ResultCode = 1, Message = "Error" };
        }

        /// <summary>
        /// 订单实时传递给盈科，补发数据
        /// </summary>
        public void SendOrderInfoToYKBDMqExt()
        {
            try
            {
                DateTime taday = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                DateTime lastday = Convert.ToDateTime(taday.AddDays(-1).ToString("yyyy-MM-dd"));

                LogHelper.Info("补发订单数据到盈科 taday：" + taday.ToLongDateString());
                LogHelper.Info("补发订单数据到盈科 lastday：" + lastday.ToLongDateString());


                //补发订单数据
                var orders = CommodityOrder.ObjectSet().Where(t =>
                                t.PaymentTime > lastday && t.PaymentTime < taday &&
                                t.EsAppId == new Guid("8b4d3317-6562-4d51-bef1-0c05694ac3a6"));

                var ids = orders.Select(t => t.Id);
                foreach (var id in ids)
                {
                    new CommodityOrderFacade().SendPayInfoToYKBDMq(id);
                }
                LogHelper.Info("补发订单数据到盈科，订单数量为：" + ids.Count());

                //补发售中数据
                var refundOrders = from c in CommodityOrder.ObjectSet()
                                   join r in OrderRefund.ObjectSet() on c.Id equals r.OrderId
                                   where c.EsAppId == new Guid("8b4d3317-6562-4d51-bef1-0c05694ac3a6") && r.State == 1 && r.ModifiedOn > lastday && r.ModifiedOn < taday
                                   select r;

                foreach (var refundOrder in refundOrders)
                {
                    if (refundOrder.OrderItemId == null)
                    {
                        refundOrder.OrderItemId = Guid.Empty;
                    }
                    SendRefundInfoToYKBDMq(refundOrder.OrderId, (Guid)refundOrder.OrderItemId);
                }
                LogHelper.Info("补发售中退款订单数据到盈科，订单数量为：" + refundOrders.Count());

                //补发售后数据
                var refundAfterSalesOrders = from c in CommodityOrder.ObjectSet()
                                             join or in OrderRefundAfterSales.ObjectSet() on c.Id equals or.OrderId
                                             where c.EsAppId == new Guid("8b4d3317-6562-4d51-bef1-0c05694ac3a6") && or.State == 1 && or.ModifiedOn > lastday && or.ModifiedOn < taday
                                             select or;

                foreach (var refundAfterSalesOrder in refundAfterSalesOrders)
                {
                    if (refundAfterSalesOrder.OrderItemId == null)
                    {
                        refundAfterSalesOrder.OrderItemId = Guid.Empty;
                    }
                    SendASRefundInfoToYKBDMq(refundAfterSalesOrder.OrderId, (Guid)refundAfterSalesOrder.OrderItemId);
                }
                LogHelper.Info("补发售后退款订单数据到盈科，订单数量为：" + refundAfterSalesOrders.Count());

            }
            catch (Exception ex)
            {
                LogHelper.Error("SendOrderInfoToYKBDMq方法异常", ex);
            }
        }

        /// <summary>
        /// 发送订单售中退款实时数据到盈科大数据系统mq
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public void SendRefundInfoToYKBDMq(Guid orderId, Guid itemId)
        {
            LogHelper.Debug(string.Format("CommodityOrderBP.SendRefundInfoToYKBDMqExt发送订单售中退款实时数据到盈科大数据系统mq,入参:orderId={0}", orderId));
            if (orderId == Guid.Empty) return;
            Task.Factory.StartNew(() =>
            {
                var json = string.Empty;
                var mqjson = string.Empty;
                var bdJournal = default(YKBigDataMqJournalDTO);
                try
                {
                    var order = CommodityOrder.ObjectSet().FirstOrDefault(o => o.Id == orderId);
                    if (order == null || !order.PaymentTime.HasValue || order.EsAppId != YJB.Deploy.CustomDTO.YJBConsts.YJAppId) return;
                    var orderRefundList = OrderRefund.ObjectSet().Where(p => p.OrderId == orderId && p.State == 1);
                    if (itemId != Guid.Empty)
                    {
                        orderRefundList = orderRefundList.Where(t => t.OrderItemId == itemId);
                    }
                    var orderRefund = orderRefundList.FirstOrDefault();

                    if (orderRefund != null && orderRefund.RefundMoney <= 0) return;
                    bdJournal = new YKBigDataMqJournalDTO
                    {
                        Id = Guid.NewGuid(),
                        SubTime = DateTime.Now,
                        OrderId = order.Id,
                        OrderCode = order.Code,
                        OrderItemId = orderRefund.OrderItemId ?? Guid.Empty,
                        Source = "CommodityOrderBP.SendRefundInfoToYKBDMqExt",
                        Message = "售中退款数据",
                        Result = "发送成功"
                    };
                    var jsonObj = new
                    {
                        OrderId = order.Id,
                        OrderCode = order.Code,
                        OrderTradeTime = orderRefund.ModifiedOn.ToString("yyyyMMddHHmmss"),
                        OrderMoney = -orderRefund.RefundMoney
                    };
                    json = JsonConvert.SerializeObject(jsonObj);
                    bdJournal.Json = json;
                    if (!TPS.YJBJMQSV.SendToMq("bj_bd_order", json))
                    {
                        bdJournal.Result = "发送失败";
                        LogHelper.Error(string.Format("CommodityOrderBP.SendRefundInfoToYKBDMqExt发送订单售中退款实时数据到盈科大数据系统mq失败,入参:orderId={0}", orderId));
                    }

                    Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundDTO orderRefundDto = new Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundDTO();
                    orderRefundDto.Id = orderRefund.Id;
                    orderRefundDto.RefundType = orderRefund.RefundType;
                    orderRefundDto.RefundReason = orderRefund.RefundReason;
                    orderRefundDto.RefundMoney = orderRefund.RefundMoney;
                    orderRefundDto.RefundDesc = orderRefund.RefundDesc;
                    orderRefundDto.OrderId = orderRefund.OrderId;
                    orderRefundDto.State = orderRefund.State;
                    if (orderRefund.RefundExpCo != null)
                    {
                        orderRefundDto.RefundExpCo = orderRefund.RefundExpCo;
                    }
                    if (orderRefund.RefundExpOrderNo != null)
                    {
                        orderRefundDto.RefundExpOrderNo = orderRefund.RefundExpOrderNo;
                    }
                    orderRefundDto.OrderRefundImgs = orderRefund.OrderRefundImgs;
                    orderRefundDto.IsFullRefund = (bool)orderRefund.IsFullRefund ? 1 : 0;
                    orderRefundDto.strSubTime = orderRefund.SubTime.ToString("yyyyMMddHHmmss");
                    orderRefundDto.strModifiedOn = orderRefund.ModifiedOn.ToString("yyyyMMddHHmmss");
                    if (orderRefund.OrderItemId != null)
                    {
                        orderRefundDto.OrderItemId = orderRefund.OrderItemId;
                    }
                    orderRefundDto.DataType = orderRefund.DataType;
                    if (orderRefund.RefuseTime != null)
                    {
                        orderRefundDto.RefuseTime = orderRefund.RefuseTime;
                    }
                    if (orderRefund.RefundExpOrderTime != null)
                    {
                        orderRefundDto.RefundExpOrderTime = orderRefund.RefundExpOrderTime;
                    }
                    if (orderRefund.IsDelayConfirmTimeAfterSales != null)
                    {
                        orderRefundDto.IsDelayConfirmTimeAfterSales = orderRefund.IsDelayConfirmTimeAfterSales;
                    }
                    if (orderRefund.AgreeFlag != null)
                    {
                        orderRefundDto.AgreeFlag = orderRefund.AgreeFlag;
                    }
                    orderRefundDto.RefundScoreMoney = orderRefund.RefundScoreMoney;
                    orderRefundDto.RefundYJBMoney = orderRefund.RefundYJBMoney;
                    if (orderRefund.RefundFreightPrice != null)
                    {
                        orderRefundDto.RefundFreightPrice = orderRefund.RefundFreightPrice;
                    }
                    //bj_bw_refund1  售中退款
                    mqjson = JsonConvert.SerializeObject(orderRefundDto);
                    LogHelper.Info("bj_bw_refund1发送的json:" + mqjson);
                    if (!TPS.YJBJMQSV.SendToMq("bj_bw_refund1", mqjson))
                    {
                        bdJournal.Result = "新售中发送失败";
                        LogHelper.Error(string.Format("CommodityOrderBP.SendRefundInfoToYKBDMqExt发送售中退款订单数据到盈科大数据系统mq失败222222,入参:orderId={0}", orderId));
                    }
                }
                catch (Exception ex)
                {
                    bdJournal.Result = "发送异常";
                    bdJournal.Message = ex.ToString();
                    LogHelper.Error(string.Format("CommodityOrderBP.SendRefundInfoToYKBDMqExt发送订单售中退款实时数据到盈科大数据系统mq异常,入参:orderId={0}", orderId), ex);
                }
                finally
                {
                    if (bdJournal != null) new YKBigDataMqJournal().Create(bdJournal);
                }
            });
        }

        /// <summary>
        /// 发送订单售后退款实时数据到盈科大数据系统mq
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public void SendASRefundInfoToYKBDMq(Guid orderId, Guid itemId)
        {
            LogHelper.Debug(string.Format("CommodityOrderBP.SendASRefundInfoToYKBDMqExt发送订单售后退款实时数据到盈科大数据系统mq,入参:orderId={0}", orderId));
            if (orderId == Guid.Empty) return;
            Task.Factory.StartNew(() =>
            {
                var json = string.Empty;
                var mqjson = string.Empty;
                var bdJournal = default(YKBigDataMqJournalDTO);
                try
                {
                    var order = CommodityOrder.ObjectSet().FirstOrDefault(o => o.Id == orderId);
                    if (order == null || !order.PaymentTime.HasValue || order.EsAppId != YJB.Deploy.CustomDTO.YJBConsts.YJAppId) return;
                    var orderRefundList = OrderRefundAfterSales.ObjectSet().Where(p => p.OrderId == orderId && p.State == 1);
                    if (itemId != Guid.Empty)
                    {
                        orderRefundList = orderRefundList.Where(t => t.OrderItemId == itemId);
                    }
                    var orderRefund = orderRefundList.FirstOrDefault();

                    if (orderRefund != null && orderRefund.RefundMoney <= 0) return;
                    bdJournal = new YKBigDataMqJournalDTO
                    {
                        Id = Guid.NewGuid(),
                        SubTime = DateTime.Now,
                        OrderId = order.Id,
                        OrderCode = order.Code,
                        OrderItemId = orderRefund.OrderItemId ?? Guid.Empty,
                        Source = "CommodityOrderBP.SendASRefundInfoToYKBDMqExt",
                        Message = "售后退款数据",
                        Result = "发送成功"
                    };
                    var jsonObj = new
                    {
                        OrderId = order.Id,
                        OrderCode = order.Code,
                        OrderTradeTime = orderRefund.ModifiedOn.ToString("yyyyMMddHHmmss"),
                        OrderMoney = -orderRefund.RefundMoney
                    };
                    json = JsonConvert.SerializeObject(jsonObj);
                    bdJournal.Json = json;
                    if (!TPS.YJBJMQSV.SendToMq("bj_bd_order", json))
                    {
                        bdJournal.Result = "发送失败";
                        LogHelper.Error(string.Format("CommodityOrderBP.SendASRefundInfoToYKBDMqExt发送订单售后退款实时数据到盈科大数据系统mq失败,入参:orderId={0}", orderId));
                    }

                    Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundAfterSalesDTO orderRefundAfterSalesDto = new Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundAfterSalesDTO();
                    orderRefundAfterSalesDto.Id = orderRefund.Id;
                    orderRefundAfterSalesDto.RefundType = orderRefund.RefundType;
                    orderRefundAfterSalesDto.RefundReason = orderRefund.RefundReason;
                    orderRefundAfterSalesDto.RefundMoney = orderRefund.RefundMoney;
                    orderRefundAfterSalesDto.RefundDesc = orderRefund.RefundDesc;
                    orderRefundAfterSalesDto.OrderId = orderRefund.OrderId;
                    if (orderRefund.OrderItemId != null)
                    {
                        orderRefundAfterSalesDto.OrderItemId = orderRefund.OrderItemId;
                    }
                    orderRefundAfterSalesDto.State = orderRefund.State;
                    if (orderRefund.RefundExpCo != null)
                    {
                        orderRefundAfterSalesDto.RefundExpCo = orderRefund.RefundExpCo;
                    }
                    if (orderRefund.RefundExpOrderNo != null)
                    {
                        orderRefundAfterSalesDto.RefundExpOrderNo = orderRefund.RefundExpOrderNo;
                    }
                    orderRefundAfterSalesDto.OrderRefundImgs = orderRefund.OrderRefundImgs;
                    orderRefundAfterSalesDto.IsFullRefund = orderRefund.IsFullRefund;
                    orderRefundAfterSalesDto.DataType = orderRefund.DataType;
                    orderRefundAfterSalesDto.strSubTime = orderRefund.SubTime.ToString("yyyyMMddHHmmss");
                    orderRefundAfterSalesDto.strModifiedOn = orderRefund.ModifiedOn.ToString("yyyyMMddHHmmss");
                    if (orderRefund.RefuseTime != null)
                    {
                        orderRefundAfterSalesDto.RefuseTime = orderRefund.RefuseTime;
                    }
                    if (orderRefund.RefundExpOrderTime != null)
                    {
                        orderRefundAfterSalesDto.RefundExpOrderTime = orderRefund.RefundExpOrderTime;
                    }
                    orderRefundAfterSalesDto.RefundScoreMoney = orderRefund.RefundScoreMoney;
                    orderRefundAfterSalesDto.RefundYJBMoney = orderRefund.RefundYJBMoney;
                    orderRefundAfterSalesDto.RefundFreightPrice = orderRefund.RefundFreightPrice;

                    //bj_bw_refund2  售后退款
                    mqjson = JsonConvert.SerializeObject(orderRefundAfterSalesDto);
                    LogHelper.Info("bj_bw_refund2发送的json数据:" + mqjson);
                    if (!TPS.YJBJMQSV.SendToMq("bj_bw_refund2", mqjson))
                    {
                        bdJournal.Result = "新售后发送失败";
                        LogHelper.Error(string.Format("CommodityOrderBP.SendASRefundInfoToYKBDMqExt发送订单售后退款实时数据到盈科大数据系统mq失败222222,入参:orderId={0}", orderId));
                    }
                }
                catch (Exception ex)
                {
                    bdJournal.Result = "发送异常";
                    bdJournal.Message = ex.ToString();
                    LogHelper.Error(string.Format("CommodityOrderBP.SendASRefundInfoToYKBDMqExt发送订单售后退款实时数据到盈科大数据系统mq异常,入参:orderId={0}", orderId), ex);
                }
                finally
                {
                    if (bdJournal != null) new YKBigDataMqJournal().Create(bdJournal);
                }
            });
        }

        /// <summary>
        /// 批量处理超时未支付订单
        /// </summary>
        public void AutoExpirePayOrderExt()
        {
            //// 预售商品自动上架
            //PromotionHelper.Shelve();

            LogHelper.Info(string.Format("处理活动超时未付款订单服务开始"));
            //处理订单状态为超时交易关闭
            try
            {
                DateTime now = DateTime.Now;
                //订单超时给予5分钟缓冲时间，
                //DateTime lastTime = now.AddMinutes(-5);
                //查询超时未付款的订单 
                var orderExpirePayList = OrderExpirePay.ObjectSet().Where(c => c.ExpirePayTime < now && c.State == 0).ToList();

                if (!orderExpirePayList.Any())
                    return;

                List<Guid> orderIds = orderExpirePayList.Select(c => c.OrderId).ToList();
                var orders = CommodityOrder.ObjectSet().Where(c => (c.State == 0 || c.State == 6) && orderIds.Contains(c.Id)).ToList();
                var needUpdateExpirePayList = new List<OrderExpirePay>();
                foreach (var orderExpirePay in orderExpirePayList)
                {
                    var order = orders.FirstOrDefault(c => c.Id == orderExpirePay.OrderId);

                    if (order != null && order.State == 0)
                    {
                        if (noPayOrder(order, now))
                        {
                            needUpdateExpirePayList.Add(orderExpirePay);
                        }
                    }
                    else
                    {
                        needUpdateExpirePayList.Add(orderExpirePay);
                    }
                }

                if (needUpdateExpirePayList.Any())
                {
                    ContextSession contextSession = ContextFactory.CurrentThreadContext;
                    for (int i = 0; i < needUpdateExpirePayList.Count; i++)
                    {
                        needUpdateExpirePayList[i].State = 1;
                        needUpdateExpirePayList[i].EntityState = EntityState.Modified;
                    }
                    contextSession.SaveChanges();
                }
                LogHelper.Info("处理活动超时未付款订单服务完成");
            }
            catch (Exception ex)
            {
                LogHelper.Error("处理活动超时未付款订单服务异常。", ex);
            }
        }
        private bool noPayOrder(CommodityOrder order, DateTime now)
        {
            if (order == null)
            {
                return true;
            }
            //调用Finance接口判断 是否有真实的交易  
            if (TPS.Finance.Instance.GetIsPay(order.Id))
            {
                return true;
            }
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            List<TodayPromotion> needRefreshCacheTodayPromotions = new List<TodayPromotion>();
            int oldsate;
            if (order.Payment != 1 && order.RealPrice > 0)
            {
                LogHelper.Info("取消订单 解冻金币!");
                try
                {
                    UnFreezeGoldDTO unFreezeGoldDTO = new UnFreezeGoldDTO()
                    {
                        BizId = order.Id,
                        Sign = CustomConfig.PaySing
                    };
                    var unFreezeGoldResult = FSPSV.Instance.UnFreezeGold(unFreezeGoldDTO);
                    if (unFreezeGoldResult == null || unFreezeGoldResult.Code != 0)
                    {
                        return false;
                    }
                    LogHelper.Info("取消订单 解冻金币,message=" + unFreezeGoldResult.Message);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(string.Format("处理三天未付款订单服务异常,GoldPayFacade.UnFreezeGold异常。orderId：{0}", order.Id), ex);
                    return false;
                }
            }
            //回退积分
            bool refundScoreResult = SignSV.CommodityOrderCancelSrore(ContextFactory.CurrentThreadContext, order);
            if (!refundScoreResult)
            {
                return false;
            }

            // 回退易捷币
            var refundYJBResult = Jinher.AMP.BTP.TPS.Helper.YJBHelper.CancelOrder(ContextFactory.CurrentThreadContext, order);
            if (!refundYJBResult.IsSuccess)
            {
                return false;
            }

            //回退优惠券
            var flag = CouponSV.RefundCoupon(ContextFactory.CurrentThreadContext, order);
            if (!flag) return false;

            var orderitemlist = OrderItem.ObjectSet().Where(n => n.CommodityOrderId == order.Id);

            //加库存
            // OrderEventHelper.AddStock();
            UserLimited.ObjectSet().Context.ExecuteStoreCommand("delete from UserLimited where CommodityOrderId='" + order.Id + "'");
            foreach (OrderItem orderItem in orderitemlist)
            {
                Guid comId = orderItem.CommodityId;
                if (orderItem.Intensity != 10 || orderItem.DiscountPrice > 0)
                {
                    var promotionId = orderItem.PromotionId.HasValue ? orderItem.PromotionId.Value : Guid.Empty;
                    if (RedisHelper.HashContainsEntry(RedisKeyConst.ProSaleCountPrefix + promotionId.ToString(), orderItem.CommodityId.ToString()))
                    {
                        int surplusLimitBuyTotal = (int)RedisHelper.HashIncr(RedisKeyConst.ProSaleCountPrefix + orderItem.PromotionId.Value.ToString(), orderItem.CommodityId.ToString(), -orderItem.Number);
                        surplusLimitBuyTotal = surplusLimitBuyTotal < 0 ? 0 : surplusLimitBuyTotal;
                        TodayPromotion to = TodayPromotion.ObjectSet().FirstOrDefault(n => n.CommodityId == comId && n.PromotionId == orderItem.PromotionId && n.EndTime > now && n.StartTime < now);
                        if (to != null)
                        {
                            to.SurplusLimitBuyTotal = surplusLimitBuyTotal;
                            to.EntityState = System.Data.EntityState.Modified;
                            needRefreshCacheTodayPromotions.Add(to);
                        }
                        PromotionItems pti = PromotionItems.ObjectSet().FirstOrDefault(n => n.PromotionId == orderItem.PromotionId && n.CommodityId == comId);
                        if (pti != null)
                        {
                            pti.SurplusLimitBuyTotal = surplusLimitBuyTotal;
                            pti.EntityState = System.Data.EntityState.Modified;
                        }
                    }
                    else  //缓存中没有，直接改库
                    {
                        TodayPromotion to = TodayPromotion.ObjectSet().FirstOrDefault(n => n.CommodityId == comId && n.PromotionId == orderItem.PromotionId && n.EndTime > now && n.StartTime < now);
                        if (to != null)
                        {
                            to.SurplusLimitBuyTotal -= orderItem.Number;
                            to.EntityState = System.Data.EntityState.Modified;
                            needRefreshCacheTodayPromotions.Add(to);
                        }
                        PromotionItems pti = PromotionItems.ObjectSet().FirstOrDefault(n => n.PromotionId == orderItem.PromotionId && n.CommodityId == comId);
                        if (pti != null)
                        {
                            pti.SurplusLimitBuyTotal -= orderItem.Number;
                            pti.EntityState = System.Data.EntityState.Modified;
                        }
                    }
                    if (RedisHelper.HashContainsEntry(RedisKeyConst.UserLimitPrefix + promotionId.ToString() + ":" + orderItem.CommodityId.ToString(), order.UserId.ToString()))
                    {
                        RedisHelper.HashIncr(RedisKeyConst.UserLimitPrefix + promotionId.ToString() + ":" + orderItem.CommodityId.ToString(), order.UserId.ToString(), -orderItem.Number);
                    }

                }
            }


            oldsate = order.State;
            //更新订单状态
            order.State = 6;
            //order.RealPrice = 0;
            order.ConfirmTime = now;
            order.ModifiedOn = now;
            order.EntityState = System.Data.EntityState.Modified;
            contextSession.SaveObject(order);

            contextSession.SaveChanges();

            try
            {
                //订单日志
                Journal journal = new Journal();
                journal.Id = Guid.NewGuid();
                journal.Name = "系统处理三天未付款订单";
                journal.Code = order.Code;
                journal.SubId = order.UserId;
                journal.SubTime = now;
                journal.Details = "订单状态由" + oldsate + "变为" + order.State;
                journal.StateFrom = oldsate;
                journal.StateTo = order.State;
                journal.IsPush = false;
                journal.OrderType = order.OrderType;
                journal.CommodityOrderId = order.Id;

                journal.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(journal);
                contextSession.SaveChanges();
                if (needRefreshCacheTodayPromotions.Any())
                {
                    needRefreshCacheTodayPromotions.ForEach(c => c.RefreshCache(EntityState.Modified));
                }

                //添加消息
                AddMessage addmassage = new AddMessage();
                string odid = order.Id.ToString();
                string usid = order.UserId.ToString();
                string type = "order";
                Guid EsAppId = order.EsAppId.HasValue ? order.EsAppId.Value : order.AppId;
                addmassage.AddMessages(odid, usid, EsAppId, order.Code, order.State, "", type);
                ////正品会发送消息
                //if (new ZPHSV().CheckIsAppInZPH(order.AppId))
                //{
                //    addmassage.AddMessages(odid, usid, CustomConfig.ZPHAppId, order.Code, order.State, "", type);
                //}

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error("处理过期未付款订单记日志异常。", ex);
                return false;
            }
        }

        /// <summary>
        /// 每日计算订单分润异常
        /// </summary>
        public void CalcOrderExceptionExt()
        {

            int pageSize = 100;

            DateTime exTime = DateTime.Now;
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            DateTime minDate = CustomConfig.MinClearningOrderDate;
            LogHelper.Info("分润异常订单计算开始");
            int cnt = 0;
            try
            {
                AppSetSV appSetSv = new AppSetSV();
                var apps = appSetSv.GetAppSetExt(new Jinher.AMP.BTP.Deploy.CustomDTO.AppSetSearchDTO { PageIndex = 1, PageSize = int.MaxValue });
                if (apps != null && apps.AppList != null && apps.AppList.Any())
                {
                    foreach (var appInfo in apps.AppList)
                    {
                        int start = 0;
                        bool iscontinue = true;
                        while (iscontinue)
                        {
                            var orders = (from commodityOrder in CommodityOrder.ObjectSet()
                                          where
                                              commodityOrder.PaymentTime > minDate && commodityOrder.AppId == appInfo.AppId &&
                                              commodityOrder.State != 3 && commodityOrder.State != 4 && commodityOrder.State != 5 &&
                                              commodityOrder.State != 6 && commodityOrder.State != 7
                                          orderby commodityOrder.AppId, commodityOrder.SubTime descending
                                          select commodityOrder).Skip(start).Take(pageSize).ToList();

                            if (orders.Count < pageSize)
                                iscontinue = false;
                            start += pageSize;
                            foreach (var commodityOrder in orders)
                            {                //获取并缓存app名称

                                decimal clearingPrice;
                                var orderEx = OrderSV.BuildOrderException(commodityOrder, appInfo.AppName, exTime, 0, out clearingPrice, null, true);
                                if (orderEx != null)
                                {
                                    cnt++;
                                    contextSession.SaveObject(orderEx);
                                    contextSession.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("CommodityOrderSV.CalcOrderExceptionExt 异常", ex);
            }
            LogHelper.Info(string.Format("分润异常订单计算完成，共处理异常订单{0}单", cnt));

        }

        /// <summary>
        /// 获取订货商品清单信息
        /// </summary>
        /// <param name="managerId">管理员ID</param>
        /// <param name="pickUpCode">推广码</param>
        /// <returns>订单编号和商品明细</returns>
        public ResultDTO<CommodityOrderSDTO> GetOrderItemsByPickUpCodeExt(Guid managerId, string pickUpCode)
        {

            //光标自动定位到文本框，打开数字键盘，文本框中提示“请输入提货码”，当编辑时，提示语消失。
            //点击扫描发货，打开扫描页面。
            //提货码/扫描二维码校验成功后，显示该提货码对应订单编号、商品明细，对应订单状态变为交易成功。
            //若输入的提货码/二维码对应订单非该自提点对应的订单，则提示“该订单非该自提点订单。”；订单状态不变
            //若输入的提货码/二维码校验失败，提示“提货码错误，请重新输入！”
            //若输入的提货码/二维码对应的订单已经交易成功，再次输入该提货码时，显示商品明细，并标识“已提货”
            //若输入的提货码/二维码对应的订单在退款中，则打开页面显示订单编号，商品明细、并标识“退款中”
            //若输入的提货码/二维码对应的订单已经退款，则打开的页面显示订单编号、商品明细，并标识“已退款”
            //若输入的提货码/二维码对应的订单已经快递发货，则打开的页面显示订单编号、商品明细，并标识“已发货”
            //参考：GetOrderItemsExt  
            CommodityOrderSDTO resultInfo = new CommodityOrderSDTO();
            try
            {
                if (managerId == Guid.Empty || String.IsNullOrEmpty(pickUpCode))
                {
                    return new ResultDTO<CommodityOrderSDTO> { Data = resultInfo, ResultCode = -1, Message = "参数错误，自提点管理员Id或提货码为空。" };
                }
                var managerInfo = (from p in AppStsManager.ObjectSet()
                                   join r in AppSelfTakeStation.ObjectSet() on p.SelfTakeStationId equals r.Id
                                   where p.UserId == managerId && p.IsDel == false && r.IsDel == false
                                   select new
                                   {
                                       ManagerId = p.UserId,
                                       SelfTakeStationId = p.SelfTakeStationId
                                   }).Distinct();
                if (!managerInfo.Any())
                {
                    return new ResultDTO<CommodityOrderSDTO> { Data = resultInfo, ResultCode = -2, Message = "抱歉，您暂时没有权限查看此信息。" };
                }

                var orderPickUpInfo = AppOrderPickUp.ObjectSet().FirstOrDefault(p => p.PickUpCode == pickUpCode);
                if (orderPickUpInfo == null)
                {
                    return new ResultDTO<CommodityOrderSDTO> { Data = resultInfo, ResultCode = -4, Message = "提货码错误，请重新输入。" };
                }

                var order = CommodityOrder.ObjectSet().FirstOrDefault(c => c.Id == orderPickUpInfo.Id);

                if (!managerInfo.Any(p => p.SelfTakeStationId == orderPickUpInfo.SelfTakeStationId))
                {
                    return new ResultDTO<CommodityOrderSDTO> { Data = resultInfo, ResultCode = -3, Message = "该订单非该自提点订单。" };
                }

                resultInfo = GetOrderItemsExt(orderPickUpInfo.Id, Guid.NewGuid(), Guid.NewGuid());
                if (order.State == 2)
                    return new ResultDTO<CommodityOrderSDTO> { Data = resultInfo, ResultCode = -10, Message = "已发货" };

                if (order.State == 3)
                    return new ResultDTO<CommodityOrderSDTO> { Data = resultInfo, ResultCode = -5, Message = "已提货" };

                if (order.State == 8 || order.State == 9 || order.State == 10 || order.State == 12 || order.State == 14)
                    return new ResultDTO<CommodityOrderSDTO> { Data = resultInfo, ResultCode = -6, Message = "退款中" };

                if (order.State == 7)
                    return new ResultDTO<CommodityOrderSDTO> { Data = resultInfo, ResultCode = -7, Message = "已退款" };

                var contextSession = ContextFactory.CurrentThreadContext;

                if (confirmOrderByManager(order, contextSession))
                {
                    orderPickUpInfo.PickUpManagerId = managerId;
                    orderPickUpInfo.EntityState = EntityState.Modified;
                    contextSession.SaveChanges();
                }
                else
                {
                    return new ResultDTO<CommodityOrderSDTO> { Data = resultInfo, ResultCode = -8, Message = "确认收货失败。" };
                }

                return new ResultDTO<CommodityOrderSDTO> { Data = resultInfo, ResultCode = 0, Message = "" };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CommodityOrderSV.GetOrderItemsByPickUpCodeExt获取订货商品清单信息。managerId：{0}，ex:{1}", managerId, ex));
                return new ResultDTO<CommodityOrderSDTO> { Data = resultInfo, ResultCode = 1, Message = "获取订货商品清单信息异常。" };
            }
        }

        /// <summary>
        /// 获取自提点管理员待自提的订单信息
        /// </summary>
        /// <param name="userId">提货点管理员</param>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="pageSize">页面数量</param>
        /// <returns>获取自提点管理员待自提的订单信息</returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderListCDTO> GetOrderListByManagerIdExt(Guid userId, int pageIndex, int pageSize)
        {

            List<OrderListCDTO> resultlist = new List<OrderListCDTO>();
            try
            {
                if (userId == Guid.Empty)
                    return resultlist;
                var managerInfo = (from p in AppStsManager.ObjectSet()
                                   join r in AppSelfTakeStation.ObjectSet() on p.SelfTakeStationId equals r.Id
                                   where p.UserId == userId && p.IsDel == false && r.IsDel == false
                                   select new
                                   {
                                       UserId = p.UserId,
                                       SelfTakeStationId = p.SelfTakeStationId
                                   }).Distinct();
                if (!managerInfo.Any())
                {
                    LogHelper.Info(string.Format("抱歉，您暂时没有权限查看此信息，userId：{0}", userId));
                    return resultlist;
                }
                pageSize = pageSize == 0 ? 10 : pageSize;
                //获取该状态下的所有订单
                List<int> stateList = new List<int>() { 1 };

                var commodityorderList = (from r in managerInfo
                                          join t in AppOrderPickUp.ObjectSet() on r.SelfTakeStationId equals t.SelfTakeStationId
                                          join order in CommodityOrder.ObjectSet() on t.Id equals order.Id
                                          where order.State == 1
                                          orderby order.SubTime descending
                                          select
                                          new OrderListCDTO
                                          {
                                              CommodityOrderId = order.Id,
                                              Price = (decimal)order.RealPrice,
                                              AppId = order.AppId,
                                              UserId = order.UserId,
                                              State = order.State,
                                              Freight = order.Freight,
                                              IsModifiedPrice = order.IsModifiedPrice,
                                              OriginPrice = order.Price + order.Freight,
                                              PayType = order.Payment,
                                              SelfTakeFlag = order.SelfTakeFlag
                                          }).Skip((pageIndex - 1) * pageSize)
                                            .Take(pageSize);

                //查询订单列表的所有订单商品，并以订单id分组
                var commodityOrderIds = commodityorderList.Select(n => n.CommodityOrderId);
                if (commodityorderList.Any())
                {
                    var commoditySDTOList = (from o in OrderItem.ObjectSet()
                                             where commodityOrderIds.Contains(o.CommodityOrderId)
                                             select new OrderListItemCDTO
                                             {
                                                 Id = o.Id,
                                                 OrderId = o.CommodityOrderId,
                                                 Pic = o.PicturesPath,
                                                 Name = o.Name,
                                                 Price = o.CurrentPrice,
                                                 CommodityNumber = o.Number,
                                                 Size = o.CommodityAttributes,
                                                 HasReview = o.AlreadyReview,
                                                 Intensity = (decimal)o.Intensity,
                                                 DiscountPrice = o.DiscountPrice ?? -1,
                                                 CommodityId = o.CommodityId
                                             });
                    Dictionary<Guid, IEnumerable<OrderListItemCDTO>> csdtoList = commoditySDTOList
                        .GroupBy(c => c.OrderId, (key, group) => new { OrderId = key, CommodityList = group })
                        .ToDictionary(c => c.OrderId, c => c.CommodityList);

                    Dictionary<Guid, string> listApp = null;
                    try
                    {
                        var listAppIds = (from co in commodityorderList select co.AppId).Distinct().ToList();
                        listApp = APPSV.GetAppNameListByIds(listAppIds);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(string.Format("GetOrderListByManagerIdExt方法中调用APPSV.GetAppNameListByIds接口异常：{0}", ex));
                    }

                    foreach (var commodityOrder in commodityorderList)
                    {
                        if (csdtoList.ContainsKey(commodityOrder.CommodityOrderId))
                        {
                            var commodityDTOList = csdtoList[commodityOrder.CommodityOrderId];
                            commodityOrder.ShoppingCartItemSDTO = commodityDTOList.ToList();
                        }
                        if (listApp != null && listApp.Count > 0 && listApp.ContainsKey(commodityOrder.AppId))
                        {
                            var appNameDto = listApp[commodityOrder.AppId];
                            if (!String.IsNullOrEmpty(appNameDto))
                            {
                                commodityOrder.AppName = appNameDto;
                            }
                        }
                        resultlist.Add(commodityOrder);
                    }
                }
                return resultlist;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CommodityOrderSV.GetOrderListByManagerIdExt获取自提点管理员下所有订单。userId：{0}，ex:{1}", userId, ex));
                return resultlist;
            }
        }

        /// <summary>
        /// 确认订单收货管理
        /// </summary>
        /// <param name="order">订单信息</param>
        /// <param name="contextSession">上下文</param>
        /// <returns>true:成功，false:失败</returns>
        private bool confirmOrderByManager(CommodityOrder order, ContextSession contextSession)
        {
            ConfirmPayDTO comConfirmPayDto = null;
            DateTime now = DateTime.Now;
            var contextDTO = base.ContextDTO ?? AuthorizeHelper.InitAuthorizeInfo();
            var oldState = order.State;
            List<int> secTranPayments = new PaySourceSV().GetSecuriedTransactionPaymentExt();

            //金币支付，
            if (secTranPayments.Contains(order.Payment))
            {
                //金币支付
                try
                {
                    Jinher.AMP.FSP.Deploy.CustomDTO.ReturnInfoDTO goldPayresult = new Jinher.AMP.FSP.Deploy.CustomDTO.ReturnInfoDTO();

                    List<object> saveList = new List<object>();

                    Jinher.AMP.App.Deploy.CustomDTO.AppIdOwnerIdTypeDTO applicationDTO = APPSV.Instance.GetAppOwnerInfo(order.AppId, contextDTO);
                    if (applicationDTO == null || applicationDTO.OwnerId == Guid.Empty)
                        return false;
                    if (order.RealPrice > 0)
                    {
                        if (order.OrderType != 1)
                        {
                            comConfirmPayDto = OrderSV.BuildConfirmPayDTO(contextSession, order, out saveList, applicationDTO, isSaveObject: false);
                            LogHelper.Info(string.Format("订单确认收货confirmOrderByManager：订单id={0} , 支付DTO ={1}", order.Id, JsonHelper.JsonSerializer(comConfirmPayDto)), "BTP_Order");

                            //已经修改为冻结
                            goldPayresult = Jinher.AMP.BTP.TPS.FSPSV.Instance.ConfirmPayFreeze(comConfirmPayDto);
                        }
                        else
                        {
                            comConfirmPayDto = OrderSV.BuildConfirmPayDTO(contextSession, order, out saveList, applicationDTO, isSaveObject: false);
                            LogHelper.Info(string.Format("订单确认收货confirmOrderByManager：订单id={0} , 支付DTO ={1}", order.Id, JsonHelper.JsonSerializer(comConfirmPayDto)), "BTP_Order");

                            //已经修改为冻结
                            goldPayresult = Jinher.AMP.BTP.TPS.FSPSV.Instance.ConfirmPay(comConfirmPayDto);
                        }
                    }

                    if (goldPayresult.Code != 0 && goldPayresult.Code != -8)
                    {
                        LogHelper.Error(string.Format("运营工具确认收货，确认支付金币接口支付失败,OrderId:{0},OrderCode:{1}。code：{2}，Message：{3}", order.Id, order.Code, goldPayresult.Code, goldPayresult.Message));
                        return false;
                    }
                    else
                    {
                        if (goldPayresult.Code == -8)
                        {
                            LogHelper.Error(string.Format("运营工具确认收货，确认支付金币接口支付失败,OrderId:{0},OrderCode:{1}。code：{2}，Message：{3}", order.Id, order.Code, goldPayresult.Code, goldPayresult.Message));
                        }
                        if (saveList != null && saveList.Any())
                        {
                            foreach (var o in saveList)
                            {
                                contextSession.SaveObject(o);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    LogHelper.Error(string.Format("运营工具确认收货服务异常。 order : {0}", JsonHelper.JsonSerializer(order)), ex);
                    return false;
                }
            }
            //订单日志
            Journal journal = new Journal();
            journal.Id = Guid.NewGuid();
            journal.Name = "运营工具确认收货";
            journal.Code = order.Code;
            journal.SubId = order.UserId;
            journal.SubTime = now;
            journal.Details = "订单状态由" + oldState + "变为3";
            journal.StateFrom = oldState;
            journal.StateTo = 3;
            journal.IsPush = false;
            journal.OrderType = order.OrderType;
            journal.CommodityOrderId = order.Id;
            journal.EntityState = System.Data.EntityState.Added;
            contextSession.SaveObject(journal);

            //更新订单状态
            order.State = 3;
            order.ConfirmTime = now;
            order.ModifiedOn = now;
            order.EntityState = System.Data.EntityState.Modified;

            // 更新结算单确认收货时间
            SettleAccountHelper.ConfirmOrder(contextSession, order);

            if (order.Payment != 1 && order.OrderType != 1)
            {
                CommodityOrderService commodityOrderService = new CommodityOrderService();
                commodityOrderService.Id = order.Id;
                commodityOrderService.Name = order.Name;
                commodityOrderService.Code = order.Code;
                commodityOrderService.State = order.State;
                commodityOrderService.SubId = order.SubId;
                commodityOrderService.SelfTakeFlag = order.SelfTakeFlag;
                commodityOrderService.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(commodityOrderService);
            }


            #region CPS通知
            //CPS通知
            if ((order.SrcType == 33 || order.SrcType == 34 || order.SrcType == 36 || order.SrcType == 39 || order.SrcType == 40) && !string.IsNullOrEmpty(order.CPSId) && order.CPSId != "null")
            {
                CPSCallBack(order, contextDTO);
            }
            #endregion

            //添加消息
            AddMessage addmassage = new AddMessage();
            string odid = order.Id.ToString();
            string usid = order.UserId.ToString();
            string type = "order";
            Guid EsAppId = order.EsAppId.HasValue ? order.EsAppId.Value : order.AppId;
            addmassage.AddMessages(odid, usid, EsAppId, order.Code, order.State, "", type);
            ////正品会发送消息
            //if (new ZPHSV().CheckIsAppInZPH(order.AppId))
            //{
            //    addmassage.AddMessages(odid, usid, CustomConfig.ZPHAppId, order.Code, order.State, "", type);
            //}

            YXOrderHelper.ConfirmReceivedOrder(order.Id);
            //苏宁确认收货
            new SNOrderItemFacade().OrderConfirmReceived(order.Id);
            return true;
        }

        /// <summary>
        /// 售后订单状态转成售中的状态
        /// </summary>
        /// <param name="state">售后订单状态</param>
        /// <returns>售中订单状态</returns>
        private int ConvertOrderStateAfterSales(int state)
        {
            //确认收货=3，售后退款中=5,已退款=7，商家未收到货=10 ,金和处理退款中=12, 买家发货超时，商家未收到货=13，售后交易成功=15
            switch (state)
            {
                case 3:
                case 13:
                case 15:
                    return 3;   //交易成功
                case 5:
                    return 9;   //退款中
                case 10:
                    return 10;  //商家未收到货
                case 12:
                    return 12;  //金和处理退款中
                case 7:
                    return 7;
                default:
                    return 3; //交易成功
            }
        }
        /// <summary>
        /// 补生成二维码方法
        /// </summary>
        public void RepairePickUpCodeExt()
        {
            try
            {
                const int pageSize = 10;
                bool isNext = true;
                int errorCount = 0;
                DateTime now = DateTime.Now;
                while (isNext)
                {
                    var errorList = (from error in ErrorCommodityOrder.ObjectSet().Where(c => c.ResourceType == 0 && c.State == 0) select error).Take(pageSize).ToList();
                    if (!errorList.Any())
                        break;
                    if (errorList.Count < pageSize)
                        isNext = false;

                    ContextSession contextSession = ContextFactory.CurrentThreadContext;
                    var orderIdList = (from error in errorList select error.ErrorOrderId).ToList();

                    var orderPickUpList = (from pickUp in OrderPickUp.ObjectSet() where orderIdList.Contains(pickUp.OrderId) select pickUp).ToList();
                    if (orderPickUpList.Count != errorList.Count)
                    {
                        break;
                    }

                    foreach (var error in errorList)
                    {
                        var orderPickUp = orderPickUpList.First(c => c.OrderId == error.ErrorOrderId);
                        bool genQRCodeFlag = false;
                        string genPickUpCode = PickUpCodePre + orderPickUp.PickUpCode;
                        string codepath = string.Empty;
                        try
                        {
                            codepath = Jinher.AMP.BTP.TPS.BaseAppToolsSV.Instance.GenQRCode(genPickUpCode);
                            if (string.IsNullOrEmpty(codepath))
                            {
                                LogHelper.Error(string.Format("自提订单补生成二维码失败。订单Id：{0} 入参：{1},出参：{2}", orderPickUp.OrderId,
                                    genPickUpCode, codepath));
                                genQRCodeFlag = true;
                                errorCount++;
                            }
                            else
                            {
                                codepath = Jinher.AMP.BTP.Common.CustomConfig.CommonFileServerUrl + codepath;
                            }
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error(string.Format("自提订单补生成二维码异常。订单Id：{0} 入参：{1},出参：{2}", orderPickUp.OrderId, genPickUpCode, codepath), ex);
                            genQRCodeFlag = true;
                            errorCount++;
                        }
                        if (genQRCodeFlag)
                            continue;

                        orderPickUp.PickUpQrCodeUrl = codepath;
                        orderPickUp.EntityState = EntityState.Modified;
                        orderPickUp.ModifiedOn = now;
                        error.EntityState = EntityState.Modified;
                        error.ModifiedOn = now;
                        error.State = 1;
                    }
                    contextSession.SaveChange();
                    if (errorCount >= pageSize)
                        break;
                }
            }
            catch (Exception e)
            {
                LogHelper.Error("CommodityOrderSV.RepairePickUpCodeExt异常", e);
            }
        }
        /// <summary>
        /// 批量增加售后完成送积分
        /// </summary>
        /// <returns></returns>
        public bool AutoAddOrderScoreExt()
        {
            const int pageSize = 100;
            bool isNext = true;
            int errorCount = 0;
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            while (isNext)
            {
                // var orderList = CommodityOrder.ObjectSet().Where(c => c.State == 3 && c.ConfirmTime > CustomConfig.MinScoreOrderDate && c.ScoreState == 0).Take(10).ToList();
                var orderList = (from commodityOrder in CommodityOrder.ObjectSet()
                                 join commodityOrderService in CommodityOrderService.ObjectSet()
                                 on commodityOrder.Id equals commodityOrderService.Id
                                 where commodityOrder.State == 3 && commodityOrderService.State == 15 && commodityOrderService.EndTime > CustomConfig.MinScoreOrderDate && commodityOrder.ScoreState == 0
                                 select commodityOrder).Take(pageSize).ToList();

                if (!orderList.Any())
                    break;
                if (orderList.Count < pageSize)
                    isNext = false;

                foreach (var order in orderList)
                {
                    if (order.RealPrice == 0 || SignSV.Instance.GiveScoreBtp(order))
                    {
                        order.ScoreState = 1;
                        order.ModifiedOn = DateTime.Now;
                        order.EntityState = EntityState.Modified;
                    }
                    else
                    {
                        errorCount++;
                    }
                    contextSession.SaveChange();
                }
                if (errorCount >= pageSize)
                    break;
            }
            return true;
        }
        /// <summary>
        /// 售中买家7天未发货超时处理
        /// </summary>
        public void AutoRefundAndCommodityOrderExt()
        {
            LogHelper.Info(string.Format("售中买家7天未发货超时处理服务开始"));
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                int pageSize = 20;
                List<int> directArrivalPayments = new PaySourceSV().GetDirectArrivalPaymentExt();

                while (true)
                {
                    DateTime now = DateTime.Now;
                    DateTime lastday = now.AddDays(-7);
                    //DateTime lastday = now;
                    var orderAndRefundList = (from c in CommodityOrder.ObjectSet()
                                              join r in OrderRefund.ObjectSet() on c.Id equals r.OrderId
                                              where c.SelfTakeFlag == 0 && !directArrivalPayments.Contains(c.Payment) && (c.State == 10 && r.State == 10) && r.RefuseTime < lastday && r.RefundType == 1
                                              select new
                                              {
                                                  commodityOrder = c,
                                                  orderRefund = r
                                              }).Take(pageSize).ToList();
                    if (!orderAndRefundList.Any())
                    {
                        break;
                    }

                    //要处理的订单
                    var ordersList = orderAndRefundList.Select(t => t.commodityOrder).ToList();
                    //申请退款列表
                    var orderRefundList = orderAndRefundList.Select(t => t.orderRefund).ToList();

                    //要处理的订单ID列表
                    List<Guid> orderIds = ordersList.Select(t => t.Id).ToList();

                    foreach (CommodityOrder commodityOrder in ordersList)
                    {
                        //当前要处理的退款申请
                        var orderRefund = orderRefundList.Where(t => t.OrderId == commodityOrder.Id).FirstOrDefault();
                        int oldState = commodityOrder.State;
                        int oldOrderRefundState = orderRefund.State;
                        if (commodityOrder.Payment != 1 && orderRefund.RefundMoney > 0)
                        {
                            commodityOrder.State = 2;
                            orderRefund.State = 13;
                            orderRefund.NotReceiveTime = DateTime.Now;
                        }
                        else if (commodityOrder.Payment != 1 && orderRefund.RefundMoney == 0)
                        {
                            commodityOrder.State = 2;
                            orderRefund.State = 13;
                            orderRefund.NotReceiveTime = DateTime.Now;
                        }
                        else
                        {
                            LogHelper.Error(string.Format("此订单不能进行售后退款。commodityOrderId：{0}", commodityOrder.Id));
                            continue;
                        }
                        commodityOrder.IsRefund = false;
                        commodityOrder.RefundTime = null;
                        commodityOrder.ModifiedOn = DateTime.Now;
                        commodityOrder.EntityState = System.Data.EntityState.Modified;

                        orderRefund.ModifiedOn = DateTime.Now;
                        orderRefund.EntityState = System.Data.EntityState.Modified;

                        contextSession.SaveChanges();

                        try
                        {
                            //订单日志
                            Journal journal = new Journal();
                            journal.Id = Guid.NewGuid();
                            journal.Name = "系统处理售中买家7天未发货超时处理";
                            journal.Code = commodityOrder.Code;
                            journal.SubId = commodityOrder.UserId;
                            journal.SubTime = DateTime.Now;
                            journal.Details = "订单状态由" + oldState + "变为" + commodityOrder.State;
                            journal.StateFrom = oldState;
                            journal.StateTo = commodityOrder.State;
                            journal.IsPush = false;
                            journal.OrderType = commodityOrder.OrderType;
                            journal.CommodityOrderId = commodityOrder.Id;
                            journal.EntityState = System.Data.EntityState.Added;
                            contextSession.SaveObject(journal);
                            contextSession.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error(string.Format("系统处理售中买家7天未发货超时处理保存日志异常。"), ex);
                            continue;
                        }

                        //添加消息
                        BTPMessageSV addmassage = new BTPMessageSV();
                        CommodityOrderMessages messageModel = new CommodityOrderMessages();
                        messageModel.IsAuto = true;
                        messageModel.Id = commodityOrder.Id.ToString();
                        messageModel.UserIds = commodityOrder.UserId.ToString();
                        messageModel.AppId = commodityOrder.AppId;
                        messageModel.Code = commodityOrder.Code;
                        messageModel.State = commodityOrder.State;
                        messageModel.RefundType = orderRefund.RefundType;
                        messageModel.RefundMoney = orderRefund.RefundMoney + orderRefund.RefundScoreMoney;
                        messageModel.PayType = commodityOrder.Payment;
                        messageModel.orderRefundState = orderRefund.State;
                        messageModel.oldOrderRefundState = oldOrderRefundState;
                        messageModel.EsAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
                        addmassage.AddMessagesCommodityOrder(messageModel);

                        //易捷北京自营商品申请开电子发票（红票）
                        Guid eesAppId = new Guid(CustomConfig.InvoiceAppId);
                        //易捷北京的自营或者门店自营
                        MallApply mallApply = MallApply.ObjectSet().FirstOrDefault(t => t.EsAppId == eesAppId && t.AppId == commodityOrder.AppId && (t.Type == 0 || t.Type == 2 || t.Type == 3));
                        if (mallApply != null)
                        {
                            var invoice = Invoice.ObjectSet().FirstOrDefault(t => t.CommodityOrderId == commodityOrder.Id);
                            if (invoice != null && invoice.Category == 2)
                            {
                                TPS.Invoic.InvoicManage invoicManage = new TPS.Invoic.InvoicManage();
                                invoicManage.CreateInvoic(contextSession, commodityOrder, 0);
                                contextSession.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("系统处理售中买家7天未发货超时处理服务异常。", ex);
            }
        }

        /// <summary>
        /// 售中买家发货（物流信息）后9天（若有延长，则为12天），卖家自动确认收货。
        /// </summary>
        public void AutoDealOrderConfirmExt()
        {
            LogHelper.Info(string.Format("售中退款退货买家发货后9天确认收货开始...."));
            //处理订单状态为确认收货
            try
            {
                //所有阳关餐饮的app.
                Jinher.AMP.Store.ISV.Facade.StoreFacade storefacade = new Jinher.AMP.Store.ISV.Facade.StoreFacade();
                List<Guid> ygcyAppids = storefacade.GetAppIdList("1");


                List<int> directArrivalPayments = new PaySourceSV().GetDirectArrivalPaymentExt();
                List<int> secTranPayments = new PaySourceSV().GetSecuriedTransactionPaymentExt();

                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                DateTime now = DateTime.Now;
                //查询超过12天未确认收货的订单
                DateTime lastday = now.AddDays(-9);
                DateTime threelastday = now.AddDays(-12);
                //获取卖家已发货商品信息订单 
                var ordersQuery = (from p in OrderRefund.ObjectSet()
                                   join q in CommodityOrder.ObjectSet() on p.OrderId equals q.Id
                                   where
                                         p.State == 11 && q.SelfTakeFlag == 0 &&
                                       p.RefundExpOrderTime < (p.IsDelayConfirmTimeAfterSales == true ? threelastday : lastday)
                                       && !ygcyAppids.Contains(q.AppId)
                                   select new
                                   {
                                       orderRefund = p,
                                       commodityOrder = q
                                   }
                             );
                if (!CustomConfig.IsSystemDirectRefund)
                {
                    ordersQuery = ordersQuery.Where(q => !directArrivalPayments.Contains(q.commodityOrder.Payment));
                }
                var orders = ordersQuery.Distinct().ToList();
                LogHelper.Info(string.Format("售中自动确认支付服务处理订单数:{0}", orders.Count));

                if (orders.Count > 0)
                {
                    ContextDTO contextDto = AuthorizeHelper.CoinInitAuthorizeInfo();

                    foreach (var order in orders)
                    {
                        int oldState = order.commodityOrder.State;
                        var tradeType = PaySource.GetTradeType(order.commodityOrder.Payment);
                        if (tradeType == TradeTypeEnum.SecTrans && order.commodityOrder.GoldPrice > 0 || tradeType == TradeTypeEnum.Direct && CustomConfig.IsSystemDirectRefund)
                        {
                            Jinher.AMP.App.Deploy.CustomDTO.AppIdOwnerIdTypeDTO applicationDto = APPSV.Instance.GetAppOwnerInfo(order.commodityOrder.AppId, contextDto);
                            //退还金币
                            var goldPayresult = Jinher.AMP.BTP.TPS.FSPSV.CancelPay(OrderSV.BuildCancelPayDTO(order.commodityOrder, order.orderRefund.RefundMoney, contextSession, applicationDto), tradeType);
                            if (goldPayresult == null)
                            {
                                LogHelper.Error("售中9天确认收货退款失败。返回值为空");
                                continue;
                            }
                            if (goldPayresult.Code != 0)
                            {
                                LogHelper.Error(string.Format("售中9天确认收货退款失败。code：{0}，Message：{1}", goldPayresult.Code,
                                                              goldPayresult.Message));
                                continue;
                            }
                            if (tradeType == TradeTypeEnum.Direct)
                            {
                                if (goldPayresult.Message == "success")
                                {
                                    order.commodityOrder.State = 12;
                                    order.orderRefund.State = 12;
                                }
                                else
                                {
                                    LogHelper.Error(string.Format("售中9天确认收货退款失败。code：{0}，Message：{1}", goldPayresult.Code, goldPayresult.Message));
                                    continue;
                                }
                            }
                            else
                            {
                                if (goldPayresult.Code == 0 && goldPayresult.Message == "success")
                                {
                                    //支付成功后为已退款
                                    order.commodityOrder.State = 7;
                                    order.orderRefund.State = 1;
                                }
                            }
                        }
                        else
                        {
                            order.commodityOrder.State = 7;
                            order.orderRefund.State = 1;
                        }
                        //修改时间赋值
                        order.orderRefund.ModifiedOn = DateTime.Now;
                        order.orderRefund.EntityState = System.Data.EntityState.Modified;

                        //商品订单表修改时间赋值
                        order.commodityOrder.ConfirmTime = DateTime.Now;
                        order.commodityOrder.ModifiedOn = DateTime.Now;
                        order.commodityOrder.EntityState = System.Data.EntityState.Modified;

                        try
                        {
                            //订单日志
                            Journal journal = new Journal();
                            journal.Id = Guid.NewGuid();
                            journal.Name = "售中9天后卖家自动确认收货";
                            journal.Code = order.commodityOrder.Code;
                            journal.SubId = order.commodityOrder.UserId;
                            journal.SubTime = DateTime.Now;
                            journal.Details = "订单状态由" + oldState + "变为" + order.commodityOrder.State;
                            journal.StateFrom = oldState;
                            journal.StateTo = order.commodityOrder.State;
                            journal.IsPush = false;
                            journal.OrderType = order.commodityOrder.OrderType;
                            journal.CommodityOrderId = order.commodityOrder.Id;

                            journal.EntityState = System.Data.EntityState.Added;
                            contextSession.SaveObject(journal);
                            contextSession.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error("系统处理售中9天后卖家自动确认收货记日志异常。", ex);
                        }

                        //向客户端推送交易失败消息
                        AddMessage addmassage = new AddMessage();
                        string type = "order";
                        string messages = "";

                        switch (order.commodityOrder.State)
                        {
                            case 7:
                                messages = string.Format("您的订单{0}已完成退款，退款金额{1}元，请到付款账号中确认！", order.commodityOrder.Code, order.orderRefund.RefundMoney);
                                break;
                        }
                        Guid EsAppId = order.commodityOrder.EsAppId.HasValue ? order.commodityOrder.EsAppId.Value : order.commodityOrder.AppId;
                        addmassage.AddMessages(order.commodityOrder.Id.ToString(), order.commodityOrder.UserId.ToString(), EsAppId,
                            order.commodityOrder.Code, order.commodityOrder.State, messages, type);

                        //易捷北京自营商品申请开电子发票
                        Guid esAppId = new Guid(CustomConfig.InvoiceAppId);
                        //易捷北京的自营或者门店自营
                        MallApply mallApply = MallApply.ObjectSet().FirstOrDefault(t => t.EsAppId == esAppId && t.AppId == order.commodityOrder.AppId && (t.Type == 0 || t.Type == 2 || t.Type == 3));
                        if (mallApply != null)
                        {
                            var invoice = Invoice.ObjectSet().FirstOrDefault(t => t.CommodityOrderId == order.commodityOrder.Id);
                            if (invoice != null && invoice.Category == 2)
                            {
                                TPS.Invoic.InvoicManage invoicManage = new TPS.Invoic.InvoicManage();
                                invoicManage.CreateInvoic(contextSession, order.commodityOrder, 0);
                                contextSession.SaveChanges();
                            }
                        }
                        ////正品会发送消息
                        //if (new ZPHSV().CheckIsAppInZPH(order.commodityOrder.AppId))
                        //{
                        //    addmassage.AddMessages(order.commodityOrder.Id.ToString(), order.commodityOrder.UserId.ToString(), CustomConfig.ZPHAppId,
                        //    order.commodityOrder.Code, order.commodityOrder.State, messages, type);
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("售中9天卖家自动确认支付服务异常。", ex);
            }
        }

        /// <summary>
        ///处理售中仅退款的申请订单 10天内未响应 交易状态变为 7 已退款
        /// </summary>
        public void AutoOnlyRefundOrderExt()
        {
            LogHelper.Info(string.Format("处理售中10天内未响应的申请的仅退款的订单Job服务开始"));

            //处理订单状态为已退款
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                int pageSize = 20;
                List<int> directArrivalPayments = new PaySourceSV().GetDirectArrivalPaymentExt();
                List<int> stwog = new PaySourceSV().GetSecTransWithoutGoldPaymentExt();

                //所有阳关餐饮的app.
                Jinher.AMP.Store.ISV.Facade.StoreFacade storefacade = new Jinher.AMP.Store.ISV.Facade.StoreFacade();
                List<Guid> ygcyAppids = storefacade.GetAppIdList("1");


                while (true)
                {
                    DateTime now = DateTime.Now;
                    //售中查询10天内 商家未处理的申请退款的订单
                    DateTime lastday = now.AddDays(-10);
                    //DateTime lastday = now;
                    var middelQuery = (from c in CommodityOrder.ObjectSet()
                                       join r in OrderRefund.ObjectSet() on c.Id equals r.OrderId
                                       where c.SelfTakeFlag == 0 && (c.State == 9 || c.State == 14) && r.State == 0 && r.RefundType == 0 && r.SubTime < lastday
                                        && !ygcyAppids.Contains(c.AppId)
                                       select new
                                       {
                                           commodityOrder = c,
                                           orderRefund = r
                                       });

                    if (!CustomConfig.IsSystemDirectRefund)
                    {
                        middelQuery = middelQuery.Where(c => !directArrivalPayments.Contains(c.commodityOrder.Payment));
                    }
                    var middelList = middelQuery.Take(pageSize).ToList();
                    if (!middelList.Any())
                        break;

                    //要处理的售中订单
                    var commodityOrderList = middelList.Select(t => t.commodityOrder).ToList();
                    //申请退款列表
                    var orderRefundList = middelList.Select(t => t.orderRefund).ToList();
                    //要处理的订单ID列表
                    List<Guid> orderIds = commodityOrderList.Select(t => t.Id).ToList();

                    //售中旧订单状态
                    int oldState = 0;
                    //售中旧退款流水状态
                    int oldOrderRefundState = -1;

                    LogHelper.Info(string.Format("处理售中10天内 商家未处理的申请退款订单服务处理订单数:{0}", orderIds.Count));

                    foreach (CommodityOrder commodityOrder in commodityOrderList)
                    {
                        //ContextFactory.ReleaseContextSession();
                        LogHelper.Info(string.Format("[售中]处理售中10天内 商家未处理的申请退款订单服务处理订单，订单Id:{0}", commodityOrder.Id));
                        var orderRefunds = orderRefundList.Where(t => t.OrderId == commodityOrder.Id);
                        //整单退款或者是订单只有单商品的退款
                        if (orderRefunds.Count() == 1)
                        {
                            //当前要处理的售中退款申请
                            var orderRefund = orderRefundList.Where(t => t.OrderId == commodityOrder.Id).FirstOrDefault();
                            //订单状态
                            oldState = commodityOrder.State;
                            oldOrderRefundState = orderRefund.State;
                            var tradeType = PaySource.GetTradeType(commodityOrder.Payment);

                            var refundmoney = orderRefund.RefundMoney;
                            decimal coupon_price = 0;
                            var user_yjcoupon = YJBSV.GetUserYJCouponByOrderId(commodityOrder.Id);
                            if (user_yjcoupon.Data != null)
                            {
                                foreach (var item in user_yjcoupon.Data)
                                {
                                    coupon_price += item.UseAmount ?? 0;
                                }
                            }
                            decimal yjbprice = 0;
                            var yjbresult = YJBSV.GetOrderYJBInfo(commodityOrder.EsAppId.Value, commodityOrder.Id);
                            if (yjbresult.Data != null)
                            {
                                yjbprice = yjbresult.Data.InsteadCashAmount;
                            }
                            var cashmoney = commodityOrder.RealPrice.Value - commodityOrder.Freight;
                            //refundmoney = orderRefund.RefundMoney - coupon_price - yjbprice;//退款金币去除抵用券金额和易捷币
                            //refundmoney = (orderRefund.OrderRefundMoneyAndCoupun ?? 0) > cashmoney ? cashmoney : (orderRefund.OrderRefundMoneyAndCoupun ?? 0);
                            if (orderRefund.OrderRefundMoneyAndCoupun == null)
                            {//老的退款数据
                                refundmoney = orderRefund.RefundMoney > cashmoney ? cashmoney : orderRefund.RefundMoney;
                            }
                            else
                            {
                                refundmoney = (orderRefund.OrderRefundMoneyAndCoupun ?? 0) > cashmoney ? cashmoney : (orderRefund.OrderRefundMoneyAndCoupun ?? 0);
                            }
                            if (refundmoney > 0 && (tradeType == TradeTypeEnum.SecTrans && commodityOrder.GoldPrice > 0 || tradeType == TradeTypeEnum.Direct && CustomConfig.IsSystemDirectRefund))
                            {
                                if (orderRefund.OrderItemId != Guid.Empty && orderRefund.OrderItemId != null)
                                {
                                    var orderItem = OrderItem.FindByID((Guid)orderRefund.OrderItemId);
                                    if (orderItem != null)
                                    {
                                        decimal orRefundMoney = orderRefund.RefundMoney + orderRefund.RefundScoreMoney - orderItem.ScorePrice;
                                        if (orderItem.YjbPrice != null)
                                        {
                                            orRefundMoney = orRefundMoney - (decimal)orderItem.YjbPrice;
                                        }
                                        orderRefund.RefundMoney = orRefundMoney - orderItem.ScorePrice;
                                    }
                                }
                                Jinher.AMP.App.Deploy.CustomDTO.AppIdOwnerIdTypeDTO applicationDTO = APPSV.Instance.GetAppOwnerInfo(commodityOrder.AppId);
                                var goldPayresult = Jinher.AMP.BTP.TPS.FSPSV.CancelPay(OrderSV.BuildCancelPayDTO(commodityOrder, refundmoney, contextSession, applicationDTO), tradeType);
                                if (goldPayresult == null)
                                {
                                    LogHelper.Error(string.Format("售中退款失败。返回值为空，commodityOrderId：{0}", commodityOrder.Id));
                                    continue;
                                }
                                if (goldPayresult.Code != 0)
                                {
                                    LogHelper.Error(string.Format("售中退款失败。code：{0}，Message：{1}，commodityOrderId：{2}", goldPayresult.Code, goldPayresult.Message, commodityOrder.Id));
                                    continue;
                                }
                                if (tradeType == TradeTypeEnum.Direct)
                                {
                                    if (goldPayresult.Message == "success")
                                    {
                                        commodityOrder.State = 12;
                                        orderRefund.State = 12;
                                    }
                                    else
                                    {
                                        LogHelper.Error(string.Format("售中退款失败。code：{0}，Message：{1}，commodityOrderId：{2}", goldPayresult.Code, goldPayresult.Message, commodityOrder.Id));
                                        continue;
                                    }
                                }
                                else
                                {
                                    if (goldPayresult.Message == "success")
                                    {
                                        commodityOrder.State = 7;
                                        orderRefund.State = 1;
                                    }
                                    else
                                    {
                                        commodityOrder.State = 12;
                                        orderRefund.State = 12;
                                    }
                                }
                                LogHelper.Info(string.Format("售中退款返回值：{0},Message:{1},order.commodityOrderId:{2}", goldPayresult.Code, goldPayresult.Message, commodityOrder.Id));

                            }
                            else
                            {
                                commodityOrder.State = 7;
                                orderRefund.State = 1;
                            }
                            //回退积分
                            SignSV.CommodityOrderRefundScore(contextSession, commodityOrder, orderRefund);

                            // 回退易捷币
                            //Jinher.AMP.BTP.TPS.Helper.YJBHelper.OrderRefund(contextSession, commodityOrder, orderRefund);

                            #region 回退易捷币和易捷抵用券
                            decimal couponprice = 0;
                            decimal couponmoney = 0;//抵用券使用总金额
                            //decimal totalcouprice = 0;//保存抵用券之和(当2个或者2个以上的抵用券相加的时候)
                            //decimal pretotalcouprice = 0;//保存上一个抵用券相加的价格(当2个或者2个以上的抵用券相加的时候)
                            bool isexistsplit = false;//是否有拆单，如果有的话，整单退款的时候，退的是抵用券的使用金额
                            var issplit = MainOrder.ObjectSet().Where(x => x.SubOrderId == commodityOrder.Id).FirstOrDefault();
                            if (issplit != null)
                            {
                                isexistsplit = true;
                            }
                            var useryjcoupon = YJBSV.GetUserYJCouponByOrderId(commodityOrder.Id);
                            if (useryjcoupon.Data != null && useryjcoupon.Data.Count > 0)
                            {
                                useryjcoupon.Data = useryjcoupon.Data.OrderBy(x => x.Price).ToList();
                                var refundmoney1 = orderRefund.OrderRefundMoneyAndCoupun ?? 0;

                                foreach (var item in useryjcoupon.Data)
                                {
                                    couponmoney += item.UseAmount ?? 0;
                                }
                                if (refundmoney == couponmoney + refundmoney)
                                {//全额退
                                    for (int i = 0; i < useryjcoupon.Data.Count; i++)
                                    {
                                        if (useryjcoupon.Data[i] != null)
                                        {
                                            if (isexistsplit)
                                                couponprice = useryjcoupon.Data[i].UsePrice;
                                            else
                                                couponprice = useryjcoupon.Data[i].Price;
                                            if (i == 0)
                                            {

                                            }
                                            else
                                            {//易捷币不能循环退
                                                orderRefund.RefundYJBMoney = 0;
                                            }
                                            decimal coupon = couponprice;
                                            Jinher.AMP.BTP.TPS.Helper.YJBHelper.OrderRefund(ContextFactory.CurrentThreadContext, commodityOrder, orderRefund, coupon, useryjcoupon.Data[i].Id);
                                        }
                                    }
                                }
                                else
                                {//部分退款

                                    for (int i = 0; i < useryjcoupon.Data.Count; i++)
                                    {
                                        if (useryjcoupon.Data[i] != null)
                                        {
                                            if (isexistsplit)
                                                couponprice = useryjcoupon.Data[i].UsePrice;
                                            else
                                                couponprice = useryjcoupon.Data[i].Price;
                                            //totalcouprice += useryjcoupon.Data[i].Price;
                                            if (i == 0)
                                            {
                                                //pretotalcouprice = couponprice;
                                            }
                                            else
                                            {//易捷币不能循环退
                                                orderRefund.RefundYJBMoney = 0;
                                            }
                                            decimal coupon = 0;
                                            if (refundmoney1 - refundmoney > 0)
                                            {
                                                if (refundmoney1 - refundmoney - couponprice >= 0)
                                                {//退款金额大于等于（实际支付金额+抵用券金额），直接返回抵用券面值
                                                 //if (refundmoney1 - commodityOrder.RealPrice - totalcouprice - orderRefund.RefundYJBMoney < 0)
                                                 //{//返还部分易捷币
                                                 //    orderRefund.RefundYJBMoney = orderRefund.RefundYJBMoney - (refundmoney1 + commodityOrder.RealPrice.Value + totalcouprice);
                                                 //}
                                                    coupon = couponprice;
                                                }
                                                else
                                                {//否则，表示不是全额退款，返还部分抵用券，易捷币返还0
                                                    coupon = refundmoney1 - refundmoney;
                                                    orderRefund.RefundYJBMoney = 0;
                                                }
                                            }
                                            Jinher.AMP.BTP.TPS.Helper.YJBHelper.OrderRefund(contextSession, commodityOrder, orderRefund, coupon, useryjcoupon.Data[i].Id);
                                            refundmoney1 -= coupon;
                                            //pretotalcouprice = totalcouprice;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Jinher.AMP.BTP.TPS.Helper.YJBHelper.OrderRefund(ContextFactory.CurrentThreadContext, commodityOrder, orderRefund, 0, Guid.Empty);
                            }

                            #endregion

                            // 更新结算项
                            Jinher.AMP.BTP.TPS.Helper.SettleAccountHelper.OrderRefund(contextSession, commodityOrder, orderRefund);

                            commodityOrder.ConfirmTime = DateTime.Now;
                            commodityOrder.ModifiedOn = DateTime.Now;
                            commodityOrder.EntityState = System.Data.EntityState.Modified;

                            orderRefund.ModifiedOn = DateTime.Now;
                            orderRefund.EntityState = System.Data.EntityState.Modified;

                            contextSession.SaveChanges();

                            try
                            {
                                //订单日志
                                Journal journal = new Journal();
                                journal.Id = Guid.NewGuid();
                                journal.Name = "系统处理售中10天内商家未响应，自动达成同意退款申请协议订单";
                                journal.Code = commodityOrder.Code;
                                journal.SubId = commodityOrder.UserId;
                                journal.SubTime = DateTime.Now;
                                journal.Details = "订单状态由" + oldState + "变为" + commodityOrder.State;
                                journal.CommodityOrderId = commodityOrder.Id;
                                journal.StateFrom = oldState;
                                journal.StateTo = commodityOrder.State;
                                journal.IsPush = false;
                                journal.OrderType = commodityOrder.OrderType;

                                journal.EntityState = System.Data.EntityState.Added;
                                contextSession.SaveObject(journal);
                                contextSession.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                LogHelper.Error("系统处理售中10天内商家未响应，自动达成同意退款申请协议订单记日志异常。", ex);
                                continue;
                            }
                            //添加消息
                            BTPMessageSV addmassage = new BTPMessageSV();
                            AfterSalesMessages messageModel = new AfterSalesMessages();
                            messageModel.IsAuto = true;
                            messageModel.Id = commodityOrder.Id.ToString();
                            messageModel.UserIds = commodityOrder.UserId.ToString();
                            messageModel.AppId = commodityOrder.AppId;
                            messageModel.Code = commodityOrder.Code;
                            messageModel.State = commodityOrder.State;
                            messageModel.RefundType = orderRefund.RefundType;
                            messageModel.RefundMoney = orderRefund.RefundMoney + orderRefund.RefundScoreMoney;
                            messageModel.PayType = commodityOrder.Payment;
                            messageModel.orderRefundAfterSalesState = orderRefund.State;
                            messageModel.oldOrderRefundAfterSalesState = oldOrderRefundState;
                            messageModel.EsAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
                            addmassage.AddMessagesAfterSales(messageModel);

                            //易捷北京自营商品申请开电子发票 (红票)
                            Guid esAppId = new Guid(CustomConfig.InvoiceAppId);
                            //易捷北京的自营或者门店自营
                            MallApply mallApply = MallApply.ObjectSet().FirstOrDefault(t => t.EsAppId == esAppId && t.AppId == commodityOrder.AppId && (t.Type == 0 || t.Type == 2 || t.Type == 3));
                            if (mallApply != null)
                            {
                                var invoice = Invoice.ObjectSet().FirstOrDefault(t => t.CommodityOrderId == commodityOrder.Id);
                                if (invoice != null && invoice.Category == 2 && (bool)orderRefund.IsFullRefund)
                                {
                                    TPS.Invoic.InvoicManage invoicManage = new TPS.Invoic.InvoicManage();
                                    invoicManage.CreateInvoic(contextSession, commodityOrder, 1);
                                    contextSession.SaveChanges();
                                }
                            }
                        }
                        if (orderIds.Count < pageSize)
                        {
                            break;
                        }
                    }
                }
                LogHelper.Info("系统处理售中10天内商家未响应，自动达成同意退款申请协议Job处理成功");
            }
            catch (Exception ex)
            {
                LogHelper.Error("系统处理售中10天内商家未响应，自动达成同意退款申请协议订单Job服务异常。", ex);
            }
        }

        /// <summary>
        ///中石化电子发票 补发错误发票请求以及下载电子发票接口调用
        /// </summary>
        public void DownloadEInvoiceInfoExt()
        {
            LogHelper.Info(string.Format("中石化电子发票 补发错误发票请求以及下载电子发票接口调用 Job服务开始"));

            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            try
            {
                var invoicManage = new TPS.Invoic.InvoicManage();

                #region 20180511添加（补发在还没有拆分数据 就进行确认收货的没有开票订单）
                Guid yjAppId = new Guid(CustomConfig.InvoiceAppId);
                var temp = from co in CommodityOrder.ObjectSet()
                           join i in Invoice.ObjectSet() on co.Id equals i.CommodityOrderId
                           join m in MallApply.ObjectSet() on co.AppId equals m.AppId
                           orderby co.SubTime descending
                           where m.EsAppId == yjAppId && (m.Type == 0 || m.Type == 2 || m.Type == 3)
                                 && i.Category == 2 && co.State == 3
                           select co;

                //已开电子发票的订单id集合
                var htjsInvoiceOrderIds = HTJSInvoice.ObjectSet().Select(t => t.Id);
                temp = temp.Where(t => !htjsInvoiceOrderIds.Contains(t.Id));

                foreach (var commodityOrder in temp)
                {
                    invoicManage.CreateInvoic(contextSession, commodityOrder, 0);
                }
                #endregion

                //补发调用开票接口错误的数据
                var fhtjsInvoices = HTJSInvoice.ObjectSet().Where(t => t.FMsgCode != "0000");
                foreach (var fhtjsInvoice in fhtjsInvoices)
                {
                    CommodityOrder commodityOrder = CommodityOrder.FindByID(fhtjsInvoice.Id);
                    invoicManage.CreateInvoic(contextSession, commodityOrder, fhtjsInvoice.RefundType);
                }

                //下载电子发票
                var shtjsInvoices = HTJSInvoice.ObjectSet().Where(t => t.FMsgCode == "0000" && t.SMsgCode != "0000");
                foreach (var shtjsInvoice in shtjsInvoices)
                {
                    CommodityOrder commodityOrder = CommodityOrder.FindByID(shtjsInvoice.Id);
                    invoicManage.DownloadInvoic(commodityOrder, shtjsInvoice.RefundType, contextSession);
                }

                contextSession.SaveChange();
            }
            catch (Exception ex)
            {
                string s = string.Format("中石化电子发票 补发错误发票请求以及下载电子发票接口调用，异常信息：{0}", ex);
                LogHelper.Error(s);
            }

        }

        /// <summary>
        ///中石化电子发票 补发错误发票请求以及下载电子发票接口调用
        /// </summary>
        public void PrCreateInvoicExt()
        {
            LogHelper.Info(string.Format("中石化电子发票 补发错误发票请求以及下载电子发票接口调用 Job服务开始"));
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            try
            {
                //检索部分退款 应该开正常发票的订单
                var commodityOrderIds = HTJSInvoice.ObjectSet().Where(t => t.RefundType == 1).Select(t => t.Id);
                foreach (var commodityOrderId in commodityOrderIds)
                {
                    var orderItems = OrderItem.ObjectSet().Where(t => t.CommodityOrderId == commodityOrderId);
                    foreach (var orderItem in orderItems)
                    {
                        var yjbjOrderItemList = YJBJOrderItem.ObjectSet().Where(t => t.OrderId == commodityOrderId && t.CommodityId == orderItem.CommodityId);
                        var sumPaymoney = yjbjOrderItemList.Sum(t => t.PayMoney);
                        var sumRefundmoney = yjbjOrderItemList.Sum(t => t.RefundMoney);
                        if (sumPaymoney != sumRefundmoney)
                        {
                            var invoicManage = new TPS.Invoic.InvoicManage();
                            CommodityOrder commodityOrder = CommodityOrder.FindByID(commodityOrderId);
                            invoicManage.CreateInvoic(contextSession, commodityOrder, 2);
                        }
                    }
                }
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                string s = string.Format("中石化电子发票 补发错误发票请求以及下载电子发票接口调用，异常信息：{0}", ex);
                LogHelper.Error(s);
            }
        }

        /// <summary>
        /// 重新校验已完成订单的钱款去向。
        /// </summary>
        public void CheckFinishOrderExt()
        {
            LogHelper.Info(string.Format("重新校验已完成订单的钱款去向Job服务开始"));

            //处理订单状态为已退款
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                int pageSize = 20;

                while (true)
                {
                    //以订单为中心。
                    List<Guid> orderIds = (from op in OrderPayee.ObjectSet()
                                           where op.ReCheckFlag == 1
                                           select op.OrderId).Distinct().Take(pageSize).ToList();
                    if (!orderIds.Any())
                    {
                        return;
                    }
                    //订单钱款去向列表。
                    var orderPayeeListOri = (from op in OrderPayee.ObjectSet()
                                             where orderIds.Contains(op.OrderId)
                                             select op).ToList();
                    //订单列表
                    var commodityOrderList = (from co in CommodityOrder.ObjectSet()
                                              where orderIds.Contains(co.Id)
                                              select co).ToList();

                    //售中退款列表。
                    var orderRefundList = from oref in OrderRefund.ObjectSet()
                                          where oref.State == 1 && orderIds.Contains(oref.OrderId)
                                          select new { Id = oref.Id, OrderId = oref.OrderId, RefundMoney = oref.RefundMoney };
                    //订单售后列表
                    var coServiceList = (from cos in CommodityOrderService.ObjectSet()
                                         where orderIds.Contains(cos.Id)
                                         select cos).ToList();
                    //订单售后流水表中已退款流水信息。
                    var orasList = (from oras in OrderRefundAfterSales.ObjectSet()
                                    where oras.State == 1 && orderIds.Contains(oras.OrderId)
                                    select new { Id = oras.Id, OrderId = oras.OrderId, RefundMoney = oras.RefundMoney }).ToList();

                    ContextDTO contextDTO = Jinher.AMP.BTP.Common.AuthorizeHelper.CoinInitAuthorizeInfo();

                    StringBuilder sbResult = new StringBuilder("");
                    //找出相关订单，在售中，且为最终状态的（已退款；已退款、退货）
                    foreach (CommodityOrder co in commodityOrderList)
                    {
                        AppIdOwnerIdTypeDTO applicationDTO = APPSV.Instance.GetAppOwnerInfo(co.AppId, contextDTO);
                        var opOriList = orderPayeeListOri.Where(op => op.OrderId == co.Id);
                        if (co.State == 3)
                        {
                            var cosList = (from cos in coServiceList
                                           where cos.Id == co.Id
                                           select cos);
                            if (!cosList.Any())
                            {
                                continue;
                            }
                            var cosFirst = cosList.FirstOrDefault();
                            if (cosFirst.State != 7 && cosFirst.State != 15)
                            {
                                continue;
                            }
                            //售后已退款
                            if (cosFirst.State == 7)
                            {
                                var orasl = orasList.Where(oras => oras.OrderId == co.Id);
                                if (!orasl.Any())
                                {
                                    continue;
                                }
                                var orasFirst = orasl.FirstOrDefault();
                                //重算后的收款明细。
                                List<OrderPayee> orderPayeeListRe = OrderSV.BuildCancelOrderPayees(co, orasFirst.RefundMoney, contextSession, applicationDTO, cosFirst);
                                string msg = CheckOrderPayee(opOriList, orderPayeeListRe, co.Id);
                                sbResult.AppendLine(msg);
                            }
                            //售后交易成功
                            else if (cosFirst.State == 15)
                            {
                                List<Jinher.JAP.BF.BE.Base.BusinessObject> objList = new List<Jinher.JAP.BF.BE.Base.BusinessObject>();
                                List<OrderPayee> orderPayeeListRe = OrderSV.BuildConfirmOrderPayees(co, applicationDTO, true, out objList);
                                string msg = CheckOrderPayee(opOriList, orderPayeeListRe, co.Id);
                                sbResult.AppendLine(msg);
                            }

                        }
                        //售中已退款
                        else if (co.State == 7)
                        {
                            var orList = orderRefundList.Where(ore => ore.OrderId == co.Id);
                            if (!orList.Any())
                            {
                                continue;
                            }
                            var orFirst = orList.FirstOrDefault();
                            List<OrderPayee> orderPayeeListRe = OrderSV.BuildCancelOrderPayees(co, orFirst.RefundMoney, contextSession, applicationDTO, null);
                            string msg = CheckOrderPayee(opOriList, orderPayeeListRe, co.Id);
                            sbResult.AppendLine(msg);
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(sbResult.ToString()))
                    {
                        sbResult.Insert(0, "订单钱款去向异常纪录： ");
                        LogHelper.Error(sbResult.ToString());
                    }
                    //更新订单钱款去向纪录的状态。
                    foreach (var op in orderPayeeListOri)
                    {
                        op.ReCheckFlag = 2;
                        op.ModifiedOn = DateTime.Now;
                        op.EntityState = EntityState.Modified;
                    }
                    contextSession.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                string s = string.Format("重新校验已完成订单的钱款去向异常，异常信息：{0}", ex);
                LogHelper.Error(s);
            }

        }

        /// <summary>
        /// 服务订单状态变化发出通知.
        /// </summary>
        public void ServiceOrderStateChangedNotifyExt()
        {
            LogHelper.Info(string.Format("服务订单状态变化发出通知Job开始"));

            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                int pageSize = 20;
                List<Guid> exceptionOrderIds = new List<Guid>();

                Jinher.JAP.BF.BE.Deploy.Base.ContextDTO contextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();


                while (true)
                {
                    var journalQuery = (from j in Journal.ObjectSet()
                                        where j.IsPush == false && j.OrderType == 1
                                        && !exceptionOrderIds.Contains(j.CommodityOrderId)
                                        orderby j.SubTime ascending
                                        select j).Take(pageSize).ToList();


                    if (!journalQuery.Any())
                    {
                        return;
                    }
                    var oids = journalQuery.Select(j => j.CommodityOrderId).Distinct();
                    var coQuery = (from co in CommodityOrder.ObjectSet()
                                   where oids.Contains(co.Id)
                                   select new { co.Id, co.ServiceId }).ToList();

                    foreach (var jou in journalQuery)
                    {
                        var coList = coQuery.Where(co => co.Id == jou.CommodityOrderId);
                        Guid soId = Guid.Empty;
                        if (coList.Any())
                        {
                            soId = coList.First().ServiceId.HasValue ? coList.First().ServiceId.Value : Guid.Empty;
                        }
                        //通知服务订单业务平台。
                        var isSucc = LSPSV.Instance.SynchronizeOrderFromBTP(jou.CommodityOrderId, jou.StateTo, soId, contextDTO);
                        if (isSucc)
                        {
                            jou.IsPush = true;
                            jou.EntityState = EntityState.Modified;
                        }
                        else
                        {
                            exceptionOrderIds.Add(jou.CommodityOrderId);
                        }
                    }
                    contextSession.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ServiceOrderStateChangedNotifyExt异常，异常信息：{0}", ex));
            }
        }

        /// <summary>
        /// 校验某订单钱款去向
        /// </summary>
        /// <param name="orderPayeeListOri">订单原始钱款收款详情</param>
        /// <param name="orderPayeeListRe">重算订单钱款收款详情</param>
        /// <param name="orderId">订单Id</param>
        private string CheckOrderPayee(IEnumerable<OrderPayee> opOriList, IEnumerable<OrderPayee> orderPayeeListRe, Guid orderId)
        {
            StringBuilder resultMsg = new StringBuilder("");
            if (opOriList.Count() != orderPayeeListRe.Count())
            {
                //校验时收款人数和原始收款人数不一致。
                //订单钱款去向异常。
                resultMsg.Append("校验时收款人数和原始收款人数不一致，");
            }
            else
            {
                //逐个对比每个收款人所分钱款是否一致。
                foreach (var opRe in orderPayeeListRe)
                {
                    var opOris = opOriList.Where(opo => opo.PayeeId == opRe.PayeeId);
                    if (!opOris.Any())
                    {
                        resultMsg.AppendFormat("校验时收款人{0}在原纪录里未找到", opRe.PayeeId);
                    }
                }
                foreach (var opOri in opOriList)
                {
                    var opRecs = orderPayeeListRe.Where(opRe => opRe.PayeeId == opOri.PayeeId);
                    if (!opRecs.Any())
                    {
                        //校验时 某一收款人 未找到。 
                        resultMsg.AppendFormat("原收款人{0}在校验时未找到", opOri.PayeeId);
                        continue;
                    }
                    var opre = opRecs.FirstOrDefault();
                    if (opre.PayMoney != opOri.PayMoney)
                    {
                        //两次计算 分钱金额 不一致。
                        resultMsg.AppendFormat("收款人{0}在校验时收款金额和原收款金额不一致", opOri.PayeeId);
                    }
                    else if (opre.PayeeType != opOri.PayeeType)
                    {
                        //两次计算 同一收款人收钱角色 不一致。
                        resultMsg.AppendFormat("收款人{0}在校验时收款角色和原收款角色不一致", opOri.PayeeId);
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(resultMsg.ToString()))
            {
                string oinfo = string.Format("订单{0} ", orderId);
                resultMsg.Insert(0, oinfo);
            }
            return resultMsg.ToString();
        }

        /// <summary>
        /// 获取订单列表(包含第三方订单)
        /// </summary>
        /// <param name="oqpDTO">订单列表查询参数</param>
        /// <returns></returns>
        public List<OrderListCDTO> GetCommodityOrderByUserIDNewExt(OrderQueryParamDTO oqpDTO)
        {
            try
            {
                List<OrderListCDTO> resultlist = new List<OrderListCDTO>();
                if (oqpDTO == null || oqpDTO.UserId == Guid.Empty)
                    return resultlist;
                oqpDTO.PageIndex = oqpDTO.PageIndex == 0 ? 1 : oqpDTO.PageIndex;
                oqpDTO.PageSize = oqpDTO.PageSize == 0 ? 10 : oqpDTO.PageSize;

                //获取该状态下的所有订单
                List<int> stateList = CommodityOrder.GetOrderStateList(oqpDTO.State);
                var query = from c in CommodityOrder.ObjectSet()
                            join cs in CommodityOrderService.ObjectSet() on c.Id equals cs.Id
                            into data1
                            from data in data1.DefaultIfEmpty()
                            where c.State != 16 && c.State != 17 && c.State != -1
                            select new
                            {
                                commodityOrder = c,
                                StateAfterSales = data == null ? -1 : data.State
                            };


                if (oqpDTO.State.HasValue && stateList != null && stateList.Any())
                {
                    //加入售后部分
                    if (oqpDTO.State == -1)
                    {
                        List<int> stateAfterList = new List<int>() { 5, 7, 10, 12 };
                        query = query.Where(n => (stateList.Contains(n.commodityOrder.State) || (n.commodityOrder.State == 3 && stateAfterList.Contains(n.StateAfterSales))) && n.commodityOrder.UserId == oqpDTO.UserId && n.commodityOrder.IsDel != 1 && n.commodityOrder.IsDel != 3);
                    }
                    else
                    {
                        query = query.Where(n => stateList.Contains(n.commodityOrder.State) && n.commodityOrder.UserId == oqpDTO.UserId && n.commodityOrder.IsDel != 1 && n.commodityOrder.IsDel != 3);
                    }
                }
                else
                {
                    query = query.Where(n => n.commodityOrder.UserId == oqpDTO.UserId && n.commodityOrder.IsDel != 1 && n.commodityOrder.IsDel != 3);
                }
                if (oqpDTO.EsAppId != Guid.Empty)
                {
                    query = query.Where(n => n.commodityOrder.EsAppId == oqpDTO.EsAppId);
                }

                var commodityorderList1 = query.Select(n => new OrderListCDTO
                {
                    CommodityOrderId = n.commodityOrder.Id,
                    OrderCode = n.commodityOrder.Code,
                    SubTime = n.commodityOrder.SubTime,
                    Price = (decimal)n.commodityOrder.RealPrice,
                    AppId = n.commodityOrder.AppId,
                    UserId = n.commodityOrder.UserId,
                    State = n.commodityOrder.State,
                    Freight = n.commodityOrder.Freight,
                    IsModifiedPrice = n.commodityOrder.IsModifiedPrice,
                    OriginPrice = n.commodityOrder.Price + n.commodityOrder.Freight,
                    PayType = n.commodityOrder.Payment,
                    SelfTakeFlag = n.commodityOrder.SelfTakeFlag,
                    StateAfterSales = n.StateAfterSales,
                    OrderRefundState = -1,
                    OrderRefundAfterSalesState = -1,
                    OrderType = n.commodityOrder.OrderType,
                    PaymentTime = n.commodityOrder.PaymentTime,
                    Batch = n.commodityOrder.Batch,
                    ShipExpCo = n.commodityOrder.ShipExpCo,
                    ExpOrderNo = n.commodityOrder.ExpOrderNo,
                    AppType = n.commodityOrder.AppType == null ? -1 : n.commodityOrder.AppType.Value,
                    CommodityEntity = "",
                    DSFStateName = "",
                    ShipmentsTime = n.commodityOrder.ShipmentsTime,
                    IsThirdOrder = false,
                    AppName = ""
                });

                //查询第三方订单数据
                var dsforderlist = YJBDSFOrderInfo.ObjectSet().Where(d => d.UserID == oqpDTO.UserId);
                if (oqpDTO.State == 0)
                {
                    dsforderlist = dsforderlist.Where(x => x.OrderPayState == "待支付");
                }
                else if (oqpDTO.State != null)
                { //其他状态不在订单列表展示
                    dsforderlist = dsforderlist.Where(x => x.OrderPayState == "不展示");
                }
                var commodityorderList2 = from d in dsforderlist
                                          select new OrderListCDTO
                                          {
                                              CommodityOrderId = d.Id,
                                              OrderCode = d.OrderNo,
                                              SubTime = d.SubTime,
                                              Price = d.OrderPayMoney.Value,
                                              AppId = d.AppId,
                                              UserId = d.UserID.Value,
                                              State = d.Status,//第三方数据订单状态
                                              Freight = d.Freight,
                                              IsModifiedPrice = false,
                                              OriginPrice = d.OrderPayMoney.Value,
                                              PayType = -1,
                                              SelfTakeFlag = -1,
                                              StateAfterSales = -1,
                                              OrderRefundState = -1,
                                              OrderRefundAfterSalesState = -1,
                                              OrderType = -1,
                                              PaymentTime = d.OrderPayDate,
                                              Batch = "",
                                              ShipExpCo = "",
                                              ExpOrderNo = "",
                                              AppType = -1,
                                              CommodityEntity = d.Commodity,
                                              DSFStateName = d.OrderPayState,
                                              ShipmentsTime = null,
                                              IsThirdOrder = true,
                                              AppName = d.PlatformName
                                          };

                //var commodityorderList = commodityorderList1.Concat(commodityorderList2)
                //    .OrderByDescending(n => n.State == 8)//wangchao 按退款中、已退款、发布时间排序
                //    .ThenByDescending(n => n.State == 9)
                //    .ThenByDescending(n => n.State == 10)
                //    .ThenByDescending(n => n.State == 12)
                //    .ThenByDescending(n => n.State == 14)
                //    .ThenByDescending(n => n.State == 7)
                //    .ThenByDescending(n => n.SubTime)
                //    .Skip((oqpDTO.PageIndex - 1) * oqpDTO.PageSize)
                //    .Take(oqpDTO.PageSize).ToList();

                var commodityorderList3 = commodityorderList1.Concat(commodityorderList2);
                if (oqpDTO.State == 3) //待评价入口 筛除已评论的订单 
                {
                    #region 获取订单项的是否评论数据，最终得到已评论的订单ID集合，用于在待评论列表筛掉已评论的订单 by wangchao
                    //获取订单项是否已评论 
                    Jinher.AMP.SNS.IBP.Facade.ScoreFacade sc = new SNS.IBP.Facade.ScoreFacade();
                    var resultD = sc.getScoreStatusSByUser(oqpDTO.UserId.ToString());
                    //var resultD = Jinher.AMP.BTP.TPS.SNSSV.Instance.GetScoreStatusSByUser(oqpDTO.UserId.ToString());
                    LogHelper.Info(string.Format("获取订单项的是否评论数据1。resultD {0}", resultD != null ? 1 : 0));
                    if (resultD != null && resultD.Content != null && resultD.Content.Count > 0)
                    {
                        LogHelper.Info(string.Format("获取订单项的是否评论数据2。Data：{0}", JsonHelper.JsonSerializer(resultD.Content)));
                        var resultC = resultD.Content;
                        var oICList = new List<OrderItemComments>();
                        foreach (var item in resultC)
                        {
                            var m = new OrderItemComments();
                            m.OrderItemId = Guid.Parse(item.Split(',')[0]);
                            if (oICList.Count(p => p.OrderItemId == m.OrderItemId) == 0)
                                oICList.Add(m);
                        }
                        oICList = oICList.Distinct().ToList();
                        var orderItemIds = oICList.Select(p => p.OrderItemId).ToList();
                        var orderItemList = OrderItem.ObjectSet().Where(p => orderItemIds.Contains(p.Id)).ToList();
                        foreach (var itemi in oICList)
                        {
                            foreach (var itemj in orderItemList)
                            {
                                if (itemi.OrderItemId == itemj.Id)
                                    itemi.OrderId = itemj.CommodityOrderId;
                            }
                        }

                        var orderIds = oICList.Select(p => p.OrderId).Distinct().ToList();
                        var orderItemCount = OrderItem.ObjectSet().Where(p => orderIds.Contains(p.CommodityOrderId)).GroupBy(p => p.CommodityOrderId).Select(p => new { OrderId = p.Key, ItemCount = p.Count() }).ToList();
                        LogHelper.Info(string.Format("获取订单项的是否评论数据3。Data：{0}", orderItemCount.Count));
                        var orderItemCountD = oICList.GroupBy(p => p.OrderId).Select(p => new { OrderId = p.Key, ItemCount = p.Count() }).ToList();
                        LogHelper.Info(string.Format("获取订单项的是否评论数据4。Data：{0}", orderItemCountD.Count));
                        var orders = new List<Guid>();
                        foreach (var itemi in orderIds)
                        {
                            var m = orderItemCount.Where(p => p.OrderId == itemi).FirstOrDefault();
                            var c = orderItemCountD.Where(p => p.OrderId == itemi).FirstOrDefault();
                            if (m != null && c != null && m.ItemCount == c.ItemCount)
                                orders.Add(itemi);
                        }
                        LogHelper.Info(string.Format("获取订单项的是否评论数据5。Data：{0}", JsonHelper.JsonSerializer(orders)));
                        commodityorderList3 = commodityorderList3.Where(p => !orders.Contains(p.CommodityOrderId));
                    }
                    #endregion
                }
                var commodityorderList = new List<OrderListCDTO>();
                if (oqpDTO.State == -1)//如果是从退换货入口进入
                    commodityorderList = commodityorderList3
                   .OrderByDescending(n => n.State == 8)//wangchao 按退款中、已退款、发布时间排序
                   .ThenByDescending(n => n.State == 9)
                   .ThenByDescending(n => n.State == 10)
                   .ThenByDescending(n => n.State == 12)
                   .ThenByDescending(n => n.State == 14)
                   .ThenByDescending(n => n.State == 7)
                   .ThenByDescending(n => n.SubTime)
                   .Skip((oqpDTO.PageIndex - 1) * oqpDTO.PageSize)
                   .Take(oqpDTO.PageSize).ToList();
                else
                    commodityorderList = commodityorderList3
                    .OrderByDescending(n => n.SubTime)
                    .Skip((oqpDTO.PageIndex - 1) * oqpDTO.PageSize)
                    .Take(oqpDTO.PageSize).ToList();


                //查询订单列表的所有订单商品，并以订单id分组
                List<Guid> commodityOrderIds = commodityorderList.Select(n => n.CommodityOrderId).ToList<Guid>();

                if (commodityorderList.Any())
                {
                    var commoditySDTOList = (from o in OrderItem.ObjectSet()
                                             where commodityOrderIds.Contains(o.CommodityOrderId)
                                             select new OrderListItemCDTO
                                             {
                                                 Id = o.Id,
                                                 Number = o.Number,
                                                 OrderId = o.CommodityOrderId,
                                                 Pic = o.PicturesPath,
                                                 Name = o.Name,
                                                 Price = o.CurrentPrice,
                                                 CommodityNumber = o.Number,
                                                 Size = o.CommodityAttributes,
                                                 HasReview = o.AlreadyReview,
                                                 Intensity = (decimal)o.Intensity,
                                                 DiscountPrice = (decimal)(o.DiscountPrice != null ? o.DiscountPrice : -1),
                                                 CommodityId = o.CommodityId,
                                                 CommodityAttributes = o.CommodityAttributes,
                                                 Specifications = o.Specifications ?? 0,
                                                 State = o.State == null ? 0 : (int)o.State
                                             }).ToList();
                    Dictionary<Guid, List<OrderListItemCDTO>> csdtoList = commoditySDTOList
                        .GroupBy(c => c.OrderId, (key, group) => new { OrderId = key, CommodityList = group })
                        .ToDictionary(c => c.OrderId, c => c.CommodityList.ToList());

                    var listAppIds = (from co in commodityorderList select co.AppId).Distinct().ToList();
                    Dictionary<Guid, string> dictAppName = APPSV.GetAppNameListByIds(listAppIds);

                    var idList = commodityorderList.Select(t => t.CommodityOrderId).ToList();

                    //售中申请表
                    var middle = (from o in OrderRefund.ObjectSet()
                                  where idList.Contains(o.OrderId)
                                  select o).GroupBy(t => t.OrderId).ToDictionary(x => x.Key, y => y.OrderByDescending(t => t.SubTime).FirstOrDefault());
                    //售后申请表
                    var afterSales = (from o in OrderRefundAfterSales.ObjectSet()
                                      where idList.Contains(o.OrderId)
                                      select o).GroupBy(t => t.OrderId).ToDictionary(x => x.Key, y => y.OrderByDescending(t => t.SubTime).FirstOrDefault());



                    foreach (var commodityOrder in commodityorderList)
                    {
                        if (commodityOrder.IsThirdOrder)
                        {//第三方订单数据商品处理
                            List<OrderListItemCDTO> orderListItemCDTOs = new List<OrderListItemCDTO>();
                            var commoditylist = JsonHelper.JsonDeserialize<List<DSFCommodityDTO>>(commodityOrder.CommodityEntity);
                            foreach (var item in commoditylist)
                            {
                                OrderListItemCDTO orderListItemCDTO = new OrderListItemCDTO();
                                orderListItemCDTO.OrderId = commodityOrder.CommodityOrderId;
                                orderListItemCDTO.Id = Guid.NewGuid();
                                orderListItemCDTO.Name = item.Name;
                                orderListItemCDTO.Number = item.Num;
                                orderListItemCDTO.Price = item.Price;
                                orderListItemCDTO.Pic = item.Thumbnail;
                                orderListItemCDTO.CommodityNumber = item.Num;
                                orderListItemCDTOs.Add(orderListItemCDTO);
                            }
                            commodityOrder.ShoppingCartItemSDTO = orderListItemCDTOs;
                            var appitem = (from m in ThirdMerchant.ObjectSet()
                                           join s in ThirdMapStatus.ObjectSet() on m.Id equals s.AppId
                                           where s.AppId == commodityOrder.AppId && s.Status == commodityOrder.State
                                           select new
                                           {
                                               MerchantName = m.MerchantName,
                                               StatusName = s.StatusName,
                                               MobileUrl = m.MobileUrl
                                           }).FirstOrDefault();
                            if (commodityOrder.AppName == "易捷保险")
                                commodityOrder.AppName = "保险推荐";
                            else
                            {
                                if (appitem != null)
                                {
                                    commodityOrder.AppName = appitem.MerchantName;
                                    commodityOrder.DSFStateName = appitem.StatusName;
                                    commodityOrder.ThirdMobileUrl = appitem.MobileUrl;
                                }
                            }
                        }
                        else
                        {
                            // 易捷币抵现
                            var yjbInfo = YJBSV.GetOrderInfo(oqpDTO.EsAppId, commodityOrder.CommodityOrderId);
                            if (yjbInfo.IsSuccess)
                            {
                                if (yjbInfo.Data.YJBInfo != null)
                                    commodityOrder.YJBPrice = yjbInfo.Data.YJBInfo.InsteadCashAmount;
                            }

                            //取出订单的各项支付信息
                            var opdList = OrderPayDetail.ObjectSet().Where(t => t.OrderId == commodityOrder.CommodityOrderId).ToList();
                            //优惠券信息
                            var couponInfo = opdList.FirstOrDefault(t => t.ObjectType == 1);
                            if (couponInfo != null)
                            {
                                commodityOrder.CouponValue = couponInfo.Amount;
                            }
                            //积分抵现。
                            var scoreInfo = opdList.FirstOrDefault(t => t.ObjectType == 2);
                            if (scoreInfo != null)
                            {
                                commodityOrder.ScorePrice = scoreInfo.Amount;
                            }

                            if (csdtoList.ContainsKey(commodityOrder.CommodityOrderId))
                            {
                                var commodityDTOList = csdtoList[commodityOrder.CommodityOrderId];
                                commodityOrder.ShoppingCartItemSDTO = commodityDTOList;
                            }
                            if (dictAppName != null && dictAppName.Count > 0 && dictAppName.ContainsKey(commodityOrder.AppId))
                            {
                                var appNameDto = dictAppName[commodityOrder.AppId];
                                commodityOrder.AppName = appNameDto;
                            }

                            //加入售申请状态
                            if (commodityOrder.State == 3)
                            {
                                if (afterSales != null)
                                {
                                    var refund = afterSales.Where(t => t.Key == commodityOrder.CommodityOrderId).Select(t => t.Value).FirstOrDefault();
                                    if (refund != null)
                                    {
                                        commodityOrder.OrderRefundAfterSalesState = refund.State;
                                    }
                                }
                            }
                            else
                            {
                                if (middle != null)
                                {
                                    var refund = middle.Where(t => t.Key == commodityOrder.CommodityOrderId).Select(t => t.Value).FirstOrDefault();
                                    if (refund != null)
                                    {
                                        commodityOrder.OrderRefundState = refund.State;
                                    }
                                }
                            }
                        }
                        if (commodityOrder.ShoppingCartItemSDTO != null)
                            commodityOrder.ItemAllCount = commodityOrder.ShoppingCartItemSDTO.Select(p => p.CommodityNumber).Sum();
                        else
                            commodityOrder.ItemAllCount = 0;
                        resultlist.Add(commodityOrder);
                    }
                }
                #region 规格设置数据获取
                //if (resultlist != null && resultlist.Count() > 0)
                //{
                //    foreach (var item in resultlist)
                //    {
                //        decimal realprice = 0;
                //        foreach (var _item in item.ShoppingCartItemSDTO)
                //        {
                //            //if (_item.Specifications!=0&&_item.Specifications!=null)
                //            //{
                //            //    int Specifications = Convert.ToInt32(_item.Specifications ?? 0);
                //            //    int Number = (_item.Number * Specifications);
                //            //    decimal RealPrice = _item.Price;
                //            //    realprice += (Number * RealPrice);
                //            //}
                //            //else
                //            //{
                //            //    realprice +=(1 * _item.Price);
                //            //}

                //            realprice += (_item.Number * _item.Price);

                //        }
                //        item.Price = realprice;
                //    }
                //}
                #endregion
                return resultlist;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取用户所有订单。search：{0}", JsonHelper.JsonSerializer(oqpDTO)), ex);

                return null;
            }
        }

        /// <summary>
        /// 获取订单列表(不包含保险订单,现在客服系统用这个方法)
        /// </summary>
        /// <param name="oqpDTO">订单列表查询参数</param>
        /// <returns></returns>
        public List<OrderListCDTO> GetCustomCommodityOrderByUserIDNewExt(OrderQueryParamDTO oqpDTO)
        {
            try
            {
                List<OrderListCDTO> resultlist = new List<OrderListCDTO>();
                if (oqpDTO == null || oqpDTO.UserId == Guid.Empty)
                    return resultlist;
                oqpDTO.PageIndex = oqpDTO.PageIndex == 0 ? 1 : oqpDTO.PageIndex;
                oqpDTO.PageSize = oqpDTO.PageSize == 0 ? 10 : oqpDTO.PageSize;

                //获取该状态下的所有订单
                List<int> stateList = CommodityOrder.GetOrderStateList(oqpDTO.State);
                var query = from c in CommodityOrder.ObjectSet()
                            join cs in CommodityOrderService.ObjectSet()
                            on c.Id equals cs.Id
                            into data1
                            from data in data1.DefaultIfEmpty()
                            where c.State != 16 && c.State != 17
                            select new
                            {
                                commodityOrder = c,
                                StateAfterSales = data == null ? -1 : data.State
                            };


                if (oqpDTO.State.HasValue && stateList != null && stateList.Any())
                {
                    //加入售后部分
                    if (oqpDTO.State == -1)
                    {
                        List<int> stateAfterList = new List<int>() { 5, 7, 10, 12 };
                        query = query.Where(n => (stateList.Contains(n.commodityOrder.State) || (n.commodityOrder.State == 3 && stateAfterList.Contains(n.StateAfterSales))) && n.commodityOrder.UserId == oqpDTO.UserId && n.commodityOrder.IsDel != 1 && n.commodityOrder.IsDel != 3);
                    }
                    else
                    {
                        query = query.Where(n => stateList.Contains(n.commodityOrder.State) && n.commodityOrder.UserId == oqpDTO.UserId && n.commodityOrder.IsDel != 1 && n.commodityOrder.IsDel != 3);
                    }
                }
                else
                {
                    query = query.Where(n => n.commodityOrder.UserId == oqpDTO.UserId && n.commodityOrder.IsDel != 1 && n.commodityOrder.IsDel != 3);
                }
                if (oqpDTO.EsAppId != Guid.Empty)
                {
                    query = query.Where(n => n.commodityOrder.EsAppId == oqpDTO.EsAppId);

                }

                var commodityorderList1 = query.Select(n => new OrderListCDTO
                {
                    CommodityOrderId = n.commodityOrder.Id,
                    OrderCode = n.commodityOrder.Code,
                    SubTime = n.commodityOrder.SubTime,
                    Price = (decimal)n.commodityOrder.RealPrice,
                    AppId = n.commodityOrder.AppId,
                    UserId = n.commodityOrder.UserId,
                    State = n.commodityOrder.State,
                    Freight = n.commodityOrder.Freight,
                    IsModifiedPrice = n.commodityOrder.IsModifiedPrice,
                    OriginPrice = n.commodityOrder.Price + n.commodityOrder.Freight,
                    PayType = n.commodityOrder.Payment,
                    SelfTakeFlag = n.commodityOrder.SelfTakeFlag,
                    StateAfterSales = n.StateAfterSales,
                    OrderRefundState = -1,
                    OrderRefundAfterSalesState = -1,
                    OrderType = n.commodityOrder.OrderType,
                    PaymentTime = n.commodityOrder.PaymentTime,
                    Batch = n.commodityOrder.Batch,
                    ShipExpCo = n.commodityOrder.ShipExpCo,
                    ExpOrderNo = n.commodityOrder.ExpOrderNo,
                    AppType = n.commodityOrder.AppType == null ? -1 : n.commodityOrder.AppType.Value,
                    CommodityEntity = "",
                    DSFStateName = "",
                    ShipmentsTime = n.commodityOrder.ShipmentsTime
                });

                var commodityorderList3 = commodityorderList1;
                if (oqpDTO.State == 3) //待评价入口 筛除已评论的订单 
                {
                    #region 获取订单项的是否评论数据，最终得到已评论的订单ID集合，用于在待评论列表筛掉已评论的订单 by wangchao
                    //获取订单项是否已评论 
                    Jinher.AMP.SNS.IBP.Facade.ScoreFacade sc = new SNS.IBP.Facade.ScoreFacade();
                    var resultD = sc.getScoreStatusSByUser(oqpDTO.UserId.ToString());
                    //var resultD = Jinher.AMP.BTP.TPS.SNSSV.Instance.GetScoreStatusSByUser(oqpDTO.UserId.ToString());
                    LogHelper.Info(string.Format("获取订单项的是否评论数据1。resultD {0}", resultD != null ? 1 : 0));
                    if (resultD != null && resultD.Content != null && resultD.Content.Count > 0)
                    {
                        LogHelper.Info(string.Format("获取订单项的是否评论数据2。Data：{0}", JsonHelper.JsonSerializer(resultD.Content)));
                        var resultC = resultD.Content;
                        var oICList = new List<OrderItemComments>();
                        foreach (var item in resultC)
                        {
                            var m = new OrderItemComments();
                            m.OrderItemId = Guid.Parse(item.Split(',')[0]);
                            if (oICList.Count(p => p.OrderItemId == m.OrderItemId) == 0)
                                oICList.Add(m);
                        }
                        oICList = oICList.Distinct().ToList();
                        var orderItemIds = oICList.Select(p => p.OrderItemId).ToList();
                        var orderItemList = OrderItem.ObjectSet().Where(p => orderItemIds.Contains(p.Id)).ToList();
                        foreach (var itemi in oICList)
                        {
                            foreach (var itemj in orderItemList)
                            {
                                if (itemi.OrderItemId == itemj.Id)
                                    itemi.OrderId = itemj.CommodityOrderId;
                            }
                        }

                        var orderIds = oICList.Select(p => p.OrderId).Distinct().ToList();
                        var orderItemCount = OrderItem.ObjectSet().Where(p => orderIds.Contains(p.CommodityOrderId)).GroupBy(p => p.CommodityOrderId).Select(p => new { OrderId = p.Key, ItemCount = p.Count() }).ToList();
                        LogHelper.Info(string.Format("获取订单项的是否评论数据3。Data：{0}", orderItemCount.Count));
                        var orderItemCountD = oICList.GroupBy(p => p.OrderId).Select(p => new { OrderId = p.Key, ItemCount = p.Count() }).ToList();
                        LogHelper.Info(string.Format("获取订单项的是否评论数据4。Data：{0}", orderItemCountD.Count));
                        var orders = new List<Guid>();
                        foreach (var itemi in orderIds)
                        {
                            var m = orderItemCount.Where(p => p.OrderId == itemi).FirstOrDefault();
                            var c = orderItemCountD.Where(p => p.OrderId == itemi).FirstOrDefault();
                            if (m != null && c != null && m.ItemCount == c.ItemCount)
                                orders.Add(itemi);
                        }
                        LogHelper.Info(string.Format("获取订单项的是否评论数据5。Data：{0}", JsonHelper.JsonSerializer(orders)));
                        commodityorderList3 = commodityorderList3.Where(p => !orders.Contains(p.CommodityOrderId));
                    }
                    #endregion
                }
                var commodityorderList = new List<OrderListCDTO>();
                if (oqpDTO.State == -1)//如果是从退换货入口进入
                    commodityorderList = commodityorderList3
                   .OrderByDescending(n => n.State == 8)//wangchao 按退款中、已退款、发布时间排序
                   .ThenByDescending(n => n.State == 9)
                   .ThenByDescending(n => n.State == 10)
                   .ThenByDescending(n => n.State == 12)
                   .ThenByDescending(n => n.State == 14)
                   .ThenByDescending(n => n.State == 7)
                   .ThenByDescending(n => n.SubTime)
                   .Skip((oqpDTO.PageIndex - 1) * oqpDTO.PageSize)
                   .Take(oqpDTO.PageSize).ToList();
                else
                    commodityorderList = commodityorderList3
                    .OrderByDescending(n => n.SubTime)
                    .Skip((oqpDTO.PageIndex - 1) * oqpDTO.PageSize)
                    .Take(oqpDTO.PageSize).ToList();


                //查询订单列表的所有订单商品，并以订单id分组
                List<Guid> commodityOrderIds = commodityorderList.Select(n => n.CommodityOrderId).ToList<Guid>();

                if (commodityorderList.Any())
                {
                    var commoditySDTOList = (from o in OrderItem.ObjectSet()
                                             where commodityOrderIds.Contains(o.CommodityOrderId)
                                             select new OrderListItemCDTO
                                             {
                                                 Id = o.Id,
                                                 Number = o.Number,
                                                 OrderId = o.CommodityOrderId,
                                                 Pic = o.PicturesPath,
                                                 Name = o.Name,
                                                 Price = o.CurrentPrice,
                                                 CommodityNumber = o.Number,
                                                 Size = o.CommodityAttributes,
                                                 HasReview = o.AlreadyReview,
                                                 Intensity = (decimal)o.Intensity,
                                                 DiscountPrice = (decimal)(o.DiscountPrice != null ? o.DiscountPrice : -1),
                                                 CommodityId = o.CommodityId,
                                                 CommodityAttributes = o.CommodityAttributes,
                                                 Specifications = o.Specifications ?? 0,
                                                 State = o.State == null ? 0 : (int)o.State
                                             }).ToList();
                    Dictionary<Guid, List<OrderListItemCDTO>> csdtoList = commoditySDTOList
                        .GroupBy(c => c.OrderId, (key, group) => new { OrderId = key, CommodityList = group })
                        .ToDictionary(c => c.OrderId, c => c.CommodityList.ToList());

                    var listAppIds = (from co in commodityorderList select co.AppId).Distinct().ToList();
                    Dictionary<Guid, string> dictAppName = APPSV.GetAppNameListByIds(listAppIds);

                    var idList = commodityorderList.Select(t => t.CommodityOrderId).ToList();

                    //售中申请表
                    var middle = (from o in OrderRefund.ObjectSet()
                                  where idList.Contains(o.OrderId)
                                  select o).GroupBy(t => t.OrderId).ToDictionary(x => x.Key, y => y.OrderByDescending(t => t.SubTime).FirstOrDefault());
                    //售后申请表
                    var afterSales = (from o in OrderRefundAfterSales.ObjectSet()
                                      where idList.Contains(o.OrderId)
                                      select o).GroupBy(t => t.OrderId).ToDictionary(x => x.Key, y => y.OrderByDescending(t => t.SubTime).FirstOrDefault());



                    foreach (var commodityOrder in commodityorderList)
                    {
                        // 易捷币抵现
                        var yjbInfo = YJBSV.GetOrderInfo(oqpDTO.EsAppId, commodityOrder.CommodityOrderId);
                        if (yjbInfo.IsSuccess)
                        {
                            if (yjbInfo.Data.YJBInfo != null)
                                commodityOrder.YJBPrice = yjbInfo.Data.YJBInfo.InsteadCashAmount;
                        }

                        //取出订单的各项支付信息
                        var opdList = OrderPayDetail.ObjectSet().Where(t => t.OrderId == commodityOrder.CommodityOrderId).ToList();
                        //优惠券信息
                        var couponInfo = opdList.FirstOrDefault(t => t.ObjectType == 1);
                        if (couponInfo != null)
                        {
                            commodityOrder.CouponValue = couponInfo.Amount;
                        }
                        //积分抵现。
                        var scoreInfo = opdList.FirstOrDefault(t => t.ObjectType == 2);
                        if (scoreInfo != null)
                        {
                            commodityOrder.ScorePrice = scoreInfo.Amount;
                        }

                        if (csdtoList.ContainsKey(commodityOrder.CommodityOrderId))
                        {
                            var commodityDTOList = csdtoList[commodityOrder.CommodityOrderId];
                            commodityOrder.ShoppingCartItemSDTO = commodityDTOList;
                        }
                        if (dictAppName != null && dictAppName.Count > 0 && dictAppName.ContainsKey(commodityOrder.AppId))
                        {
                            var appNameDto = dictAppName[commodityOrder.AppId];
                            commodityOrder.AppName = appNameDto;
                        }

                        //加入售申请状态
                        if (commodityOrder.State == 3)
                        {
                            if (afterSales != null)
                            {
                                var refund = afterSales.Where(t => t.Key == commodityOrder.CommodityOrderId).Select(t => t.Value).FirstOrDefault();
                                if (refund != null)
                                {
                                    commodityOrder.OrderRefundAfterSalesState = refund.State;
                                }
                            }
                        }
                        else
                        {
                            if (middle != null)
                            {
                                var refund = middle.Where(t => t.Key == commodityOrder.CommodityOrderId).Select(t => t.Value).FirstOrDefault();
                                if (refund != null)
                                {
                                    commodityOrder.OrderRefundState = refund.State;
                                }
                            }
                        }

                        commodityOrder.ItemAllCount = commodityOrder.ShoppingCartItemSDTO.Select(p => p.CommodityNumber).Sum();
                        resultlist.Add(commodityOrder);
                    }
                }
                return resultlist;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取用户所有订单。search：{0}", JsonHelper.JsonSerializer(oqpDTO)), ex);

                return null;
            }
        }

        /// <summary>
        /// 订单状态修改
        /// <para>Service Url: http://testbtp.iuoooo.com/Jinher.AMP.BTP.SV.CommodityOrderSV.svc/UpdateCommodityOrderNew
        /// </para>
        /// </summary>
        /// <param name="ucopDto">参数</param>
        ///<returns></returns>
        public ResultDTO UpdateCommodityOrderNewExt(UpdateCommodityOrderParamDTO ucopDto)
        {
            ResultDTO result = new ResultDTO();
            result.ResultCode = 0;
            result.Message = "Success";

            if (ucopDto == null || ucopDto.orderId == Guid.Empty)
            {
                result.Message = "参数错误，参数不能为空！";
                result.ResultCode = 1;
                return result;
            }

            string json = JsonHelper.JsonSerializer<UpdateCommodityOrderParamDTO>(ucopDto);
            LogHelper.Info(string.Format("UpdateCommodityOrderNewExt参数：{0}", json));
            if (!OrderSV.LockOrder(ucopDto.orderId))
            {
                return new ResultDTO { ResultCode = 110, Message = "操作失败" };
            }
            try
            {
                CommodityOrder commodityOrder =
                    CommodityOrder.ObjectSet().FirstOrDefault(n => n.Id == ucopDto.orderId);
                if (commodityOrder == null)
                {
                    return new ResultDTO { ResultCode = 1, Message = "订单不存在" };
                }
                if (commodityOrder.State == 1 && ucopDto.targetState == 1)
                {
                    return new ResultDTO { ResultCode = 1, Message = "订单状态无法支付" };
                }

                //if (ucopDto.userId == Guid.Empty)
                //{
                //    ucopDto.userId = commodityOrder.UserId;
                //}
                //ucopDto.appId = commodityOrder.AppId;

                var orderitemlist =
                    OrderItem.ObjectSet().Where(n => n.CommodityOrderId == commodityOrder.Id).ToList();

                //未发货
                if (ucopDto.targetState == 1) //调用此接口付状态1统一改为付状态11（付款中）
                {
                    result = UpdateOrderStateTo1(ucopDto, commodityOrder, orderitemlist);
                }
                //确认收货
                else if (ucopDto.targetState == 3)
                {
                    result = UpdateOrderStateTo3(ucopDto, commodityOrder);
                }
                //取消订单
                else if (ucopDto.targetState == 4)
                {
                    result = UpdateOrderStateTo4(ucopDto, commodityOrder, orderitemlist);
                }
                else if (ucopDto.targetState == 5)
                {
                    result = UpdateOrderStateTo5(ucopDto, commodityOrder, orderitemlist);
                }
                else
                {
                    return new ResultDTO { ResultCode = 1, Message = "订单状态不存在" };
                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error("订单状态修改服务异常。异常信息：", ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(ucopDto.orderId);
            }
        }

        public ResultDTO<string> OrderSignExt(SignUrlDTO signUrlDTO)
        {
            ResultDTO<string> result = new ResultDTO<string>();
            if (signUrlDTO == null)
                return result;
            Guid orderId, payeeId, userId;
            decimal money, couponCount;
            long gold;
            if (!Guid.TryParse(signUrlDTO.outTradeId, out orderId) || orderId == Guid.Empty)
                return result;
            if (!Guid.TryParse(signUrlDTO.payeeId, out payeeId) || payeeId == Guid.Empty)
                return result;
            if (!Guid.TryParse(signUrlDTO.userId, out userId) || userId == Guid.Empty)
                return result;
            if (!decimal.TryParse(signUrlDTO.money, out money))
                return result;
            if (!decimal.TryParse(signUrlDTO.couponCount, out couponCount))
                return result;
            if (!long.TryParse(signUrlDTO.gold, out gold))
                return result;

            decimal dbRealPrice;
            Guid dbAppId;
            Guid dbUserId;
            var mainOrderList = GetMianOrderListExt(orderId);
            if (mainOrderList.Any())
            {
                dbRealPrice = mainOrderList.Sum(c => c.RealPrice);
                dbAppId = mainOrderList[0].AppId;
                dbUserId = mainOrderList[0].UserId;
            }
            else
            {
                var order = CommodityOrder.ObjectSet().FirstOrDefault(c => c.Id == orderId);
                if (order == null)
                    return result;
                dbRealPrice = order.RealPrice.Value;
                dbAppId = order.AppId;
                dbUserId = order.UserId;
            }

            if (dbRealPrice - money - couponCount - (gold / 1000.0m) != 0)
                return result;
            if (userId != dbUserId)
                return result;

            if (couponCount > 0 && string.IsNullOrEmpty(signUrlDTO.couponCodes))
                return result;
            if (couponCount == 0 && !string.IsNullOrEmpty(signUrlDTO.couponCodes))
                return result;

            Jinher.AMP.App.Deploy.CustomDTO.AppIdOwnerIdTypeDTO appDTO = APPSV.Instance.GetAppOwnerInfo(dbAppId, null);
            if (appDTO == null || appDTO.OwnerId != payeeId)
            {
                return result;
            }

            SortedDictionary<string, string> paraArray = new SortedDictionary<string, string>();
            paraArray.Add("outTradeId", signUrlDTO.outTradeId.ToUpper());
            paraArray.Add("payeeId", signUrlDTO.payeeId.ToUpper());
            paraArray.Add("money", signUrlDTO.money);
            paraArray.Add("userId", signUrlDTO.userId.ToUpper());
            paraArray.Add("gold", signUrlDTO.gold);
            paraArray.Add("couponCount", signUrlDTO.couponCount);
            if (signUrlDTO.couponCodes == null)
            {
                signUrlDTO.couponCodes = "";
            }
            paraArray.Add("couponCodes", signUrlDTO.couponCodes);

            // 获取参数签名
            string strParamSign = string.Empty;
            try
            {
                result.Data = SignHelper.GetSign(SignHelper.ConvertDictionaryToUrlParam(paraArray),
                                                 CustomConfig.PartnerPrivKey);
            }
            catch (Exception ex)
            {
                return result;
            }
            return result;
        }
        /// <summary>
        /// 生成在线支付的Url地址
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<string> GetPayUrlExt(Jinher.AMP.BTP.Deploy.CustomDTO.PayOrderToFspDTO payOrderToFspDto)
        {
            if (payOrderToFspDto == null || payOrderToFspDto.OrderId == Guid.Empty)
            {
                return new ResultDTO<string>() { ResultCode = 1, Message = "参数错误，不能为空" };
            }

            ResultDTO<string> result = new ResultDTO<string>();

            var commodityOrder = CommodityOrder.ObjectSet().Where(t => t.Id == payOrderToFspDto.OrderId).FirstOrDefault();

            if (commodityOrder == null)
            {
                return new ResultDTO<string>() { ResultCode = 1, Message = "找不到相应订单" };
            }

            CreateOrderToFspDTO ceateOrderToFspDTO = new Deploy.CustomDTO.CreateOrderToFspDTO();
            ceateOrderToFspDTO.GoldCoupon = payOrderToFspDto.GoldCoupon;
            ceateOrderToFspDTO.GoldCouponIds = payOrderToFspDto.GoldCouponIds;
            ceateOrderToFspDTO.GoldPrice = payOrderToFspDto.GoldPrice;
            ceateOrderToFspDTO.OrderId = commodityOrder.Id;
            ceateOrderToFspDTO.RealPrice = commodityOrder.RealPrice.Value;
            ceateOrderToFspDTO.Source = payOrderToFspDto.Source;
            ceateOrderToFspDTO.SrcType = payOrderToFspDto.SrcType;
            ceateOrderToFspDTO.WxOpenId = payOrderToFspDto.WxOpenId;
            ceateOrderToFspDTO.FirstCommodityName = payOrderToFspDto.FirstCommodityName;
            ceateOrderToFspDTO.AppId = commodityOrder.AppId;
            ceateOrderToFspDTO.EsAppId = payOrderToFspDto.EsAppId;
            ceateOrderToFspDTO.ExpireTime = payOrderToFspDto.ExpireTime;
            ceateOrderToFspDTO.TradeType = payOrderToFspDto.TradeType;
            if (commodityOrder.EsAppId.HasValue && commodityOrder.EsAppId.Value == CustomConfig.YJAppId)
            {
                ceateOrderToFspDTO.EsAppId = commodityOrder.EsAppId.Value;
                ceateOrderToFspDTO.TradeType = FSPSV.GetTradeSettingInfo(commodityOrder.EsAppId.Value);
            }
            //ceateOrderToFspDTO.DiyGroupId = diyGroupId;
            //ceateOrderToFspDTO.ShareId = shareId;
            ceateOrderToFspDTO.DealType = 1;
            ceateOrderToFspDTO.OrderType = commodityOrder.OrderType;
            ceateOrderToFspDTO.Scheme = payOrderToFspDto.Scheme;

            result.ResultCode = 0;
            result.Message = "Success";
            result.Data = OrderSV.GetCreateOrderFSPUrl(ceateOrderToFspDTO, this.ContextDTO);

            return result;
        }
        /// <summary>
        /// 获取订单详情校验数据
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ResultDTO<OrderCheckDTO> GetOrderCheckInfoExt(OrderQueryParamDTO search)
        {
            ResultDTO<OrderCheckDTO> result = new ResultDTO<OrderCheckDTO>() { Message = "Success" };
            if (search == null || search.OrderId == Guid.Empty)
                return new ResultDTO<OrderCheckDTO>() { ResultCode = 1, Message = "参数为空" };
            var order = CommodityOrder.ObjectSet()
                           .Where(c => c.Id == search.OrderId)
                           .Select(c => new OrderCheckDTO { Id = c.Id, UserId = c.UserId, AppId = c.AppId, Batch = c.Batch }).FirstOrDefault();
            if (order == null)
            {
                return new ResultDTO<OrderCheckDTO>() { ResultCode = 2, Message = "无此订单" };
            }
            result.Data = order;

            return result;
        }

        public ResultDTO UpdateOrderServiceTimeExt(OrderQueryParamDTO search)
        {
            if (search == null || search.OrderId == Guid.Empty)
                return new ResultDTO { Message = "参数为空" };
            var orderService = CommodityOrderService.ObjectSet().FirstOrDefault(c => c.Id == search.OrderId);
            if (orderService == null)
                return new ResultDTO { Message = "未找到售后订单" };
            if (orderService.State != 3)
                return new ResultDTO { Message = "订单状态有误" };
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            orderService.EntityState = EntityState.Modified;
            orderService.SubTime = orderService.SubTime.AddDays(-10);
            orderService.ModifiedOn = orderService.ModifiedOn.AddDays(-10);
            contextSession.SaveChanges();
            return new ResultDTO() { isSuccess = true, Message = "seccess" };
        }

        /// <summary>
        /// 判断订单是否为拆分订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public bool CheckIsMainOrderExt(Guid orderId)
        {
            return MainOrder.ObjectSet().Where(r => r.MainOrderId == orderId).Count() > 0;
        }

        private List<int> GetDirectArrivalPayment()
        {
            List<int> paymentList = new List<int>();
            try
            {
                var pQuery = (from p in PaySource.GetAllPaySources()
                              where p.TradeType == 1
                              select p.Payment).ToList();
                if (pQuery != null && pQuery.Any())
                {
                    paymentList.AddRange(pQuery);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("GetPaymentByTradeType异常,异常信息:", ex);
            }
            return paymentList;
        }


        /// <summary>
        /// 跟据id获取订单内容
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public List<CommodityOrderDTO> GetCommodityOrderExt(List<Guid> ids)
        {
            List<CommodityOrder> restult = null;
            if (ids.Count() > 0)
            {
                restult = CommodityOrder.ObjectSet().Where(p => ids.Contains(p.Id)).ToList();
            }
            List<CommodityOrderDTO> objlist = new List<CommodityOrderDTO>();
            if (restult.Count() > 0)
            {
                foreach (var item in restult)
                {
                    CommodityOrderDTO model = new CommodityOrderDTO();
                    model.Id = item.Id;
                    model.Name = item.Name;
                    model.Code = item.Code;
                    model.SubId = item.SubId;
                    model.SubTime = item.SubTime;
                    model.UserId = item.UserId;
                    model.AppId = item.AppId;
                    model.State = item.State;
                    model.PaymentTime = item.PaymentTime;
                    model.ConfirmTime = item.ConfirmTime;
                    model.ShipmentsTime = item.ShipmentsTime;
                    model.ReceiptUserName = item.ReceiptUserName;
                    model.ReceiptPhone = item.ReceiptPhone;
                    model.ReceiptAddress = item.ReceiptAddress;
                    model.Details = item.Details;
                    model.Payment = item.Payment;
                    model.MessageToBuyer = item.MessageToBuyer;
                    model.Province = item.Province;
                    model.City = item.City;
                    model.District = item.District;
                    model.Street = item.Street;
                    model.IsModifiedPrice = item.IsModifiedPrice;
                    model.ModifiedOn = item.ModifiedOn;
                    model.ModifiedBy = item.ModifiedBy;
                    model.ModifiedId = item.ModifiedId;
                    model.PaymentState = item.PaymentState;
                    model.Price = item.Price;
                    model.RealPrice = item.RealPrice;
                    objlist.Add(model);
                }
            }
            return objlist;
        }

        public void ProcessJdOrderExt()
        {

        }

        private CommodityDTO GetCommodity(Guid appId, Guid commodityId)
        {
            CommodityDTO com = Commodity.GetDTOFromCache(appId, commodityId);
            if (com == null)
            {
                var commodity = Commodity.ObjectSet().FirstOrDefault(n => n.Id == commodityId);
                if (commodity != null)
                {
                    com = commodity.ToEntityData();
                    Commodity.AddAppCommondityDTOCache(com);
                    LogHelper.Debug(string.Format("添加缓存结束AddAppCommondityDTOCache，商品id：{1}，结束时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), commodityId));
                }
            }

            if (com == null || com.IsDel) return null;
            return com;
        }

        /// <summary>
        /// 更新订单统计表 记录用户近一年的订单总数、金额总数以及最后的交易时间等字段
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RenewOrderStatisticsExt()
        {
            //string sql =
            //    "select co.UserId,co.AppId,sum(co.RealPrice) as 'sumRealPrice',count(co.Id) as 'orderCount',max(co.SubTime) as 'lastSubTime' " +
            //    "from CommodityOrder as co,CommodityOrderService as cose " +
            //    "where co.[State] = 3 and co.SubTime > (select dateAdd(yy,-1,getdate())) and cose.Code = co.code and cose.[State] = 15 " +
            //    "group by co.UserId,co.AppId order by sumRealPrice desc";
            LogHelper.Debug(String.Format("开始进入 RenewOrderStatisticsExt方法"));

            ResultDTO returnInfo = new ResultDTO() { isSuccess = false };
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                //统计之前 先清空相关历史统计数据
                var list = OrderStatistics.ObjectSet();
                foreach (var orderStatisticse in list)
                {
                    orderStatisticse.EntityState = EntityState.Deleted;
                    contextSession.SaveObject(orderStatisticse);
                }

                var lastYear = DateTime.Now.AddYears(-1);
                var orderStatisticsList = (from co in CommodityOrder.ObjectSet()
                                               //join cose in CommodityOrderService.ObjectSet() on co.Code equals cose.Code
                                           where (co.State == 3 || co.State == 1 || co.State == 2 || co.State == 13) && co.SubTime > lastYear
                                           //&& cose.State == 15
                                           group co by new { co.UserId, co.AppId }
                                               into g
                                           select new
                                           {
                                               userId = g.Select(t => t.UserId),
                                               appId = g.Select(t => t.AppId),
                                               sumRealPrice = g.Sum(t => t.RealPrice),
                                               orderCount = g.Count(t => t.Id != Guid.Empty),
                                               lastSubTime = g.Max(t => t.SubTime)
                                           }).OrderByDescending(t => t.sumRealPrice).ToList();

                foreach (var t in orderStatisticsList)
                {
                    OrderStatistics orderStatistics = OrderStatistics.CreateOrderStatistics();
                    orderStatistics.UserId = t.userId.FirstOrDefault();
                    orderStatistics.AppId = t.appId.FirstOrDefault();
                    orderStatistics.SumRealPrice = (decimal)t.sumRealPrice;
                    orderStatistics.OrderCount = t.orderCount;
                    orderStatistics.LastSubTime = t.lastSubTime;
                    orderStatistics.SubId = t.userId.FirstOrDefault();
                    orderStatistics.SubTime = DateTime.Now;
                    orderStatistics.ModifiedOn = DateTime.Now;
                    contextSession.SaveObject(orderStatistics);
                }

                var count = contextSession.SaveChange();
                if (count > -1)
                {
                    returnInfo.isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(String.Format("更新订单统计表 记录用户近一年的订单总数、金额总数以及最后的交易时间等字段 出现异常 ：" + ex.Message));
            }

            return returnInfo;
        }

        /// <summary>
        /// 获取符合条件的用户数据
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ResultDTO<List<OrderStatisticsSDTO>> GetOrderStatisticsExt(Jinher.AMP.Coupon.Deploy.CustomDTO.SearchOrderStatisticsExtDTO search)
        {
            ResultDTO<List<OrderStatisticsSDTO>> resultDto = new ResultDTO<List<OrderStatisticsSDTO>> { isSuccess = false };
            try
            {
                List<OrderStatisticsSDTO> orderStatistics = new List<OrderStatisticsSDTO>();
                var list = OrderStatistics.ObjectSet().Where(t => t.AppId == search.AppId);
                //订单总额区间
                if (search.MinLimit > 0 && search.MaxLimit > 0)
                {
                    list = list.Where(t => t.SumRealPrice >= search.MinLimit && t.SumRealPrice <= search.MaxLimit);
                }
                else
                {
                    if (search.MinLimit == 0 && search.MaxLimit > 0)
                    {
                        list = list.Where(t => t.SumRealPrice >= search.MinLimit && t.SumRealPrice <= search.MaxLimit);
                    }
                    if (search.MinLimit > 0 && search.MaxLimit == 0)
                    {
                        list = list.Where(t => t.SumRealPrice >= search.MinLimit);
                    }
                }
                //订单数量区间
                if (search.MinNumber > 0 && search.MaxNumber > 0)
                {
                    list = list.Where(t => t.OrderCount >= search.MinNumber && t.OrderCount <= search.MaxNumber);
                }
                else
                {
                    if (search.MinNumber == 0 && search.MaxNumber > 0)
                    {
                        list = list.Where(t => t.OrderCount >= search.MinNumber && t.OrderCount <= search.MaxNumber);
                    }
                    if (search.MinNumber > 0 && search.MaxNumber == 0)
                    {
                        list = list.Where(t => t.OrderCount >= search.MinNumber);
                    }
                }

                //订单最后一笔订单时间区间
                if (search.LastMaxTime != DateTime.MinValue && search.LastMinTime != DateTime.MinValue)
                {
                    list = list.Where(t => t.LastSubTime >= search.LastMinTime && t.LastSubTime <= search.LastMaxTime);
                }
                else
                {
                    if (search.LastMinTime == DateTime.MinValue && search.LastMaxTime != DateTime.MinValue)
                    {
                        list = list.Where(t => t.LastSubTime >= search.LastMinTime && t.LastSubTime <= search.LastMaxTime);
                    }
                    if (search.LastMinTime != DateTime.MinValue && search.LastMaxTime == DateTime.MinValue)
                    {
                        list = list.Where(t => t.LastSubTime >= search.LastMinTime);
                    }
                }

                foreach (var orderStatisticse in list)
                {
                    OrderStatisticsSDTO order = new OrderStatisticsSDTO
                    {
                        Id = orderStatisticse.Id,
                        SubId = orderStatisticse.SubId,
                        SubTime = orderStatisticse.SubTime,
                        ModifiedOn = orderStatisticse.ModifiedOn,
                        SumRealPrice = orderStatisticse.SumRealPrice,
                        AppId = orderStatisticse.AppId,
                        UserId = orderStatisticse.UserId,
                        LastSubTime = orderStatisticse.LastSubTime,
                        OrderCount = orderStatisticse.OrderCount
                    };
                    //获取用户账号
                    Jinher.AMP.CBC.ISV.Facade.UserFacade facade = new Jinher.AMP.CBC.ISV.Facade.UserFacade();
                    var userInfo = facade.GetUserInfoDTO(order.UserId);
                    order.Account = userInfo.Account;

                    orderStatistics.Add(order);
                }
                resultDto.Data = orderStatistics;
                resultDto.isSuccess = true;
                return resultDto;
            }
            catch (Exception ex)
            {
                LogHelper.Error(String.Format(" 获取符合条件的用户数据 出现异常 ：" + ex.Message));
            }
            resultDto.Data = new List<OrderStatisticsSDTO>();
            return resultDto;
        }

        /// <summary>
        /// 易捷币抵现订单，按照商品进行拆分
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateOrderItemYjbPriceExt()
        {
            LogHelper.Debug("进入易捷币抵现订单，按照商品进行拆分，开始时间：" + DateTime.Now);
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO resultDto = new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = false };
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var lastTime = DateTime.Now.AddDays(-2);
                //获取最近一分钟内的订单 因为定时任务每1分钟执行一次
                var commodityOrders = CommodityOrder.ObjectSet().Where(t => t.EsAppId == YJB.Deploy.CustomDTO.YJBConsts.YJAppId
                    && !(t.HasStatisYJInfo.HasValue && t.HasStatisYJInfo.Value)
                    && t.SubTime >= lastTime);
                LogHelper.Debug("进入易捷币抵现订单，按照商品进行拆分，商品ids：" + JsonHelper.JsSerializer(commodityOrders.Select(t => t.Id)));
                foreach (var commodityOrder in commodityOrders)
                {
                    // 查询商品易捷币抵用数量
                    var yjInfo = YJBSV.GetOrderItemYJInfo(commodityOrder.EsAppId.Value, commodityOrder.Id);
                    if (yjInfo.IsSuccess)
                    {
                        var yjbInfo = yjInfo.Data.YJBInfo;
                        if (yjbInfo == null)
                        {
                            LogHelper.Debug("进入易捷币抵现订单，按照商品进行拆分，使用易捷币抵现订单Id：" + commodityOrder.Id + "，Items: null");
                        }
                        else
                        {
                            LogHelper.Debug("进入易捷币抵现订单，按照商品进行拆分，使用易捷币抵现订单Id：" + commodityOrder.Id + "，Items:" + JsonHelper.JsSerializer(yjbInfo.Items));
                        }

                        var yjCouponInfo = yjInfo.Data.YJCouponInfo;
                        if (yjCouponInfo == null)
                        {
                            LogHelper.Debug("进入易捷抵现劵抵现订单，按照商品进行拆分，使用易捷抵现劵抵现订单Id：" + commodityOrder.Id + "，Items: null");
                        }
                        else
                        {
                            LogHelper.Debug("进入易捷抵现劵抵现订单，按照商品进行拆分，使用易捷抵现劵抵现订单Id：" + commodityOrder.Id + "，Items:" + JsonHelper.JsSerializer(yjCouponInfo.Items));
                        }

                        var orderItems = OrderItem.ObjectSet().Where(t => t.CommodityOrderId == commodityOrder.Id).ToList().OrderBy(_ => _.CommodityId).ThenByDescending(_ => _.Number).ToList();
                        var dirCommodityIndex = orderItems.Select(_ => _.CommodityId).Distinct().ToDictionary(_ => _, _ => 0);
                        foreach (var orderItem in orderItems)
                        {
                            if (yjbInfo != null && yjbInfo.Items != null)
                            {
                                var yjbInfoItems = yjbInfo.Items.Where(c => c.CommodityId == orderItem.CommodityId).ToList();
                                if (yjbInfoItems.Count > dirCommodityIndex[orderItem.CommodityId])
                                {
                                    var currentCommodityYjbInfo = yjbInfoItems[dirCommodityIndex[orderItem.CommodityId]];
                                    if (currentCommodityYjbInfo != null /*&& currentCommodityYjbInfo.IsMallYJB*/ && currentCommodityYjbInfo.InsteadCashAmount > 0)
                                    {
                                        orderItem.YjbPrice = currentCommodityYjbInfo.InsteadCashAmount;
                                    }
                                    dirCommodityIndex[orderItem.CommodityId]++;
                                }
                            }
                            if (yjCouponInfo != null && yjCouponInfo.Items != null)
                            {
                                if (yjCouponInfo.Items.FirstOrDefault().OrderItemId != Guid.Empty)
                                {
                                    orderItem.YJCouponPrice = yjCouponInfo.Items.Where(_ => _.OrderItemId == orderItem.Id).Sum(_ => _.InsteadCashAmount);
                                }
                                else
                                {
                                    orderItem.YJCouponPrice = yjCouponInfo.Items.Where(_ => _.CommodityId == orderItem.CommodityId).Sum(_ => _.InsteadCashAmount);
                                }
                            }
                            if (yjbInfo != null || yjCouponInfo != null)
                            {
                                orderItem.ModifiedOn = DateTime.Now;
                            }
                        }
                        commodityOrder.HasStatisYJInfo = true;
                    }
                }
                var count = contextSession.SaveChanges();
                if (count > -1)
                {
                    resultDto.isSuccess = true;
                }
                LogHelper.Debug("进入易捷币抵现订单，按照商品进行拆分，结束时间：" + DateTime.Now);
                return resultDto;
            }
            catch (Exception ex)
            {
                LogHelper.Error(String.Format("易捷币抵现订单，按照商品进行拆分：" + ex.Message), ex);
            }
            return resultDto;
        }

        /// <summary>
        /// 处理单品退款，OrderItem表State状态不正确的问题
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateOrderItemStateExt()
        {
            LogHelper.Debug("处理单品退款，OrderItem表State状态不正确的问题方法UpdateOrderItemState，开始时间：" + DateTime.Now);
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO resultDto = new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = false };
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                var temp = from oi in OrderItem.ObjectSet()
                           join or in OrderRefund.ObjectSet()
                           on oi.CommodityOrderId equals or.OrderId
                           orderby oi.SubTime descending
                           where or.OrderItemId != null && or.OrderItemId != Guid.Empty && or.State != 3 && or.State != 2
                           && oi.State == 0
                           select oi;
                LogHelper.Debug("处理单品退款，OrderItem表State状态不正确的问题方法，OrderItems：" + JsonHelper.JsSerializer(temp));

                foreach (var orderItem in temp)
                {
                    orderItem.State = 1;
                    orderItem.ModifiedOn = DateTime.Now;
                    orderItem.EntityState = EntityState.Modified;
                    contextSession.SaveObject(orderItem);
                }

                var count = contextSession.SaveChanges();
                if (count > -1)
                {
                    resultDto.isSuccess = true;
                }
                LogHelper.Debug("处理单品退款，OrderItem表State状态不正确的问题方法UpdateOrderItemState，结束时间：" + DateTime.Now);
                return resultDto;
            }
            catch (Exception ex)
            {
                LogHelper.Error(String.Format("处理单品退款，OrderItem表State状态不正确的问题：" + ex.Message));
            }
            return resultDto;
        }


        /// <summary>
        /// 根据订单项Id获取订单部分信息 封装给sns使用
        /// </summary>
        /// <param name="orderItemId">订单项id</param>
        /// <returns></returns>
        public CommodityOrderSDTO GetCommodityOrderSdtoByOrderItemIdExt(Guid orderItemId)
        {
            LogHelper.Debug("开始进入GetCommodityOrderSdtoByOrderItemIdExt方法，orderItemId：" + orderItemId);
            if (orderItemId == Guid.Empty)
            {
                return new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderSDTO();
            }
            Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderSDTO resultDto = new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderSDTO();
            try
            {
                var orderItem = OrderItem.FindByID(orderItemId);
                if (orderItem != null)
                {
                    resultDto = GetOrderItems(orderItem.CommodityOrderId, Guid.Empty, Guid.Empty);
                }
                LogHelper.Debug("开始进入GetCommodityOrderSdtoByOrderItemIdExt方法，resultDto：" + JsonHelper.JsSerializer(resultDto));
                return resultDto;
            }
            catch (Exception ex)
            {
                LogHelper.Error(String.Format("根据订单项Id获取订单部分信息 封装给sns使用方法异常：" + ex.Message));
            }
            return resultDto;
        }

        /// <summary>
        /// 发货提醒，发消息给商家
        /// </summary>
        /// <returns></returns>
        public bool ShipmentRemindExt(Guid commodityOrderId)
        {
            var commdityorder = CommodityOrder.ObjectSet().Where(p => p.Id == commodityOrderId).FirstOrDefault();
            if (commdityorder == null)
                LogHelper.Error(string.Format("InfoManageSV服务异常:获取订单信息异常。orderId：{0}", commodityOrderId));
            //根据Appid判断是否为个人商家 如果为个人商家 发送 库存提醒消息
            try
            {
                Jinher.AMP.Info.Deploy.CustomDTO.MessageForAddDTO messageAdd =
                         new Info.Deploy.CustomDTO.MessageForAddDTO();
                //推送消息
                List<Guid> lGuid = new List<Guid>();
                lGuid.Add(commdityorder.AppId);
                messageAdd.PublishTime = DateTime.Now;
                messageAdd.ReceiverUserId = lGuid;

                messageAdd.SenderType = Info.Deploy.Enum.SenderType.System;

                messageAdd.Title = "编号为" + commdityorder.Code + "的订单用户提醒您尽快发货，请知悉！";
                messageAdd.Content = "编号为" + commdityorder.Code + "的订单用户提醒您尽快发货，请知悉！";

                messageAdd.ReceiverRange = Info.Deploy.Enum.ReceiverRange.SpecifiedUser;
                Jinher.AMP.BTP.TPS.InfoSV.Instance.AddSystemMessage(messageAdd);
            }
            catch (Exception ex)
            {

                LogHelper.Error(string.Format("InfoManageSV服务异常:获取应用信息异常。orderId：{0}", commodityOrderId), ex);
            }
            return true;
        }

        /// <summary>
        /// 发货提醒，发消息给商家
        /// </summary>
        /// <returns></returns>
        public string GetOrderItemAttrIdExt(Guid orderItemId)
        {
            var orderItem = OrderItem.FindByID(orderItemId);
            if (orderItem != null)
            {
                return orderItem.CommodityAttributes;
            }
            return "";
        }

        /// <summary>
        /// 返回用户的退换货列表
        /// </summary>
        /// <param name="oqpDTO"></param>
        /// <returns></returns>
        public List<RefundOrderListDTO> GetRefundListExt(OrderQueryParamDTO oqpDTO)
        {
            try
            {
                //var statetList = new List<int> { 7, 8, 9, 10, 12, 14 };
                var query = CommodityOrder.ObjectSet()
                            .Where(p => p.UserId == oqpDTO.UserId)
                            .Where(p => p.State != 16 && p.State != 17)
                            .Where(p => p.IsDel != 1 && p.IsDel != 3);
                if (oqpDTO.EsAppId != Guid.Empty)
                    query = query.Where(p => p.EsAppId == oqpDTO.EsAppId);
                var soids = query.Select(p => p.Id).ToList();

                var refundOrders = OrderRefund.ObjectSet().Where(n => n.State != 2 && n.State != 3 && n.State != 4 && n.State != 13 && soids.Contains(n.OrderId))
                                   .Select(p => new OrderRefundTmp { State = p.State, OrderId = p.OrderId, OrderItemId = p.OrderItemId, SubTime = p.SubTime, RefundExpCo = p.RefundExpCo, RefundExpOrderNo = p.RefundExpOrderNo })
                                   .Union(OrderRefundAfterSales.ObjectSet().Where(n => n.State != 2 && n.State != 3 && n.State != 4 && n.State != 13 && soids.Contains(n.OrderId))
                                   .Select(p => new OrderRefundTmp { State = p.State, OrderId = p.OrderId, OrderItemId = p.OrderItemId, SubTime = p.SubTime, RefundExpCo = p.RefundExpCo, RefundExpOrderNo = p.RefundExpOrderNo })
                                   ).OrderByDescending(p => p.SubTime).Skip((oqpDTO.PageIndex - 1) * oqpDTO.PageSize).Take(oqpDTO.PageSize).ToList();

                //refundOrders = refundOrders.Union(OrderRefundAfterSales.ObjectSet().Where(n => n.State != 2 && n.State != 3 && n.State != 4 && n.State != 13 && soids.Contains(n.OrderId))
                //             .OrderByDescending(p => p.SubTime).Skip((oqpDTO.PageIndex - 1) * oqpDTO.PageSize).Take(oqpDTO.PageSize)
                //             .Select(p => new OrderRefundTmp { State = p.State, OrderId = p.OrderId, OrderItemId = p.OrderItemId, SubTime = p.SubTime, RefundExpCo = p.RefundExpCo, RefundExpOrderNo = p.RefundExpOrderNo }).ToList()
                //             ).ToList();

                var alloIds = refundOrders.Select(p => p.OrderId).Distinct().ToList();
                var orders = CommodityOrder.ObjectSet().Where(p => alloIds.Contains(p.Id)).ToList();

                //1、整单
                var fullRefund = refundOrders.Where(p => p.OrderItemId == null).ToList();
                var frIds = fullRefund.OrderByDescending(p => p.SubTime).Select(p => p.OrderId).Distinct().ToList();
                var fullOrderItems = OrderItem.ObjectSet().Where(p => frIds.Contains(p.CommodityOrderId)).ToList();



                //2、单品
                var noFullRefund = refundOrders.Where(p => p.OrderItemId != null).ToList();
                var nfrIds = noFullRefund.OrderByDescending(p => p.SubTime).Select(p => p.OrderItemId).Distinct().ToList();
                var nofullOrderItems = OrderItem.ObjectSet().Where(p => nfrIds.Contains(p.Id)).ToList();


                var result = (from a in fullRefund
                              join b in fullOrderItems on a.OrderId equals b.CommodityOrderId
                              orderby a.SubTime
                              select new RefundOrderListDTO
                              {
                                  SubTime = a.SubTime,
                                  OrderId = a.OrderId,
                                  RefundExpCo = a.RefundExpCo,
                                  RefundExpOrderNo = a.RefundExpOrderNo,
                                  ItemName = b.Name,
                                  ItemPrice = b.CurrentPrice,
                                  ItemPic = b.PicturesPath,
                                  ItemSize = b.CommodityAttributes,
                                  CommodityNumber = b.Number,
                                  ItemID = b.Id,
                                  ItemState = b.State,
                                  RefundState = a.State,
                                  OrderItemState = b.State
                              }).Union(
                                         from a in noFullRefund
                                         join b in nofullOrderItems on a.OrderItemId equals b.Id
                                         orderby a.SubTime
                                         select new RefundOrderListDTO
                                         {
                                             SubTime = a.SubTime,
                                             OrderId = a.OrderId,
                                             RefundExpCo = a.RefundExpCo,
                                             RefundExpOrderNo = a.RefundExpOrderNo,
                                             ItemName = b.Name,
                                             ItemPrice = b.CurrentPrice,
                                             ItemPic = b.PicturesPath,
                                             ItemSize = b.CommodityAttributes,
                                             CommodityNumber = b.Number,
                                             ItemID = b.Id,
                                             ItemState = b.State,
                                             RefundState = a.State,
                                             OrderItemState = b.State
                                         }
                                         ).OrderByDescending(p => p.SubTime).ToList();

                foreach (var item in result)
                {
                    var m = orders.Where(p => p.Id == item.OrderId).FirstOrDefault();
                    if (m != null)
                    {
                        item.AppId = m.AppId;
                        item.AppName = m.AppName;
                        item.Payment = m.Payment;
                        item.OrderState = m.State;
                    }
                    item.RefundShowState = item.RefundState == 1 ? "已退款" : "退款中";

                    ////售后退款退货
                    //var orderRefundAfterSales = OrderRefundAfterSales.ObjectSet().Where(n => n.OrderId == item.OrderId).OrderByDescending(t => t.SubTime).FirstOrDefault();
                    //if (orderRefundAfterSales != null)
                    //{
                    //    item.RefundExpOrderNo = orderRefundAfterSales.RefundExpOrderNo;
                    //    item.RefundExpCo = orderRefundAfterSales.RefundExpCo;
                    //}
                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("GetRefundListExt服务异常:返回用户的退换货列表。userId：{0}", oqpDTO.UserId), ex); return null;
            }
        }



        //补分享佣金 需要先补OrderShareMess表信息 从日志获取ShareId保存起来
        //OrderShareMess orderShareMessDTO = new OrderShareMess
        //{
        //    OrderId = _orderId,
        //    Id = Guid.NewGuid(),
        //    ShareId = orderSDTO.ShareId
        //};
        /// <summary>
        /// 补发分享佣金接口
        /// </summary>
        /// <param name="orderId">订单Id</param>
        public bool PushShareMoneyExt(Guid orderId)
        {
            var commodityOrder = CommodityOrder.FindByID(orderId);
            if (commodityOrder == null)
            {
                return false;
            }

            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            var yjbInfo = new YJB.Deploy.CustomDTO.OrderYJBInfoDTO();
            var yjCouponInfo = new YJB.Deploy.CustomDTO.OrderYJCouponInfoDTO();

            // 查询商品易捷币抵用数量
            var yjInfo = YJBSV.GetOrderItemYJInfo(commodityOrder.EsAppId.Value, commodityOrder.Id);
            if (yjInfo.IsSuccess)
            {
                yjbInfo = yjInfo.Data.YJBInfo;
                yjCouponInfo = yjInfo.Data.YJCouponInfo;
            }

            decimal shareMoney = commodityOrder.IsModifiedPrice
                   ? commodityOrder.RealPrice.Value
                   : commodityOrder.RealPrice.Value - commodityOrder.Freight;
            string shareId = OrderShareMess.ObjectSet().Where(c => c.OrderId == commodityOrder.Id).Select(c => c.ShareId).FirstOrDefault();
            //取得分享人
            Guid shareUseId = Guid.Empty;
            if (!string.IsNullOrEmpty(shareId))
            {
                SNS.Deploy.CustomDTO.ReturnInfoDTO<Guid> shareServiceResult = SNSSV.Instance.GetShareUserId(shareId);
                if (shareServiceResult != null && shareServiceResult.Code == "0")
                {

                    shareUseId = shareServiceResult.Content;
                }
            }
            LogHelper.Info("PushShareMoneyExt方法获取shareUseId：" + shareUseId);
            var orderitemlist = OrderItem.ObjectSet().Where(n => n.CommodityOrderId == commodityOrder.Id).ToList();
            var isSharePromotionFunction = BACBP.CheckSharePromotion(commodityOrder.AppId);

            LogHelper.Info("PushShareMoneyExt方法获取shareMoney：" + shareMoney + ",isSharePromotionFunction:" + isSharePromotionFunction);
            try
            {
                //只有本应用下单才会走分成推广
                if (shareMoney > 0 && isSharePromotionFunction && shareUseId != Guid.Empty)
                {
                    //众销推广实发金额
                    decimal commission = 0.0m;
                    //众销推广应发金额
                    decimal shouldCommission = 0.0m;
                    var orderPayDetailsList = OrderPayDetail.ObjectSet().Where(c => c.OrderId == commodityOrder.Id).ToList();
                    var appext = AppExtension.ObjectSet().FirstOrDefault(c => c.Id == commodityOrder.AppId);
                    //是否使用全局设置 
                    decimal globalAmount = 0m;
                    //商品设置列表 
                    List<Commodity> commodityShareList = null;
                    if (appext != null && appext.IsDividendAll == true && appext.SharePercent > 0m)
                    {
                        globalAmount = appext.SharePercent;
                    }
                    else
                    {
                        var comIdList = orderitemlist.Select(c => c.CommodityId).Distinct().ToList();
                        commodityShareList = Commodity.ObjectSet().Where(c => c.SharePercent > 0 && comIdList.Contains(c.Id)).ToList();
                    }
                    //使用了优惠券或积分
                    if (orderPayDetailsList.Any())
                    {
                        var couponModel = orderPayDetailsList.FirstOrDefault(t => t.ObjectType == 1);
                        var integrationAmount =
                            orderPayDetailsList.Where(t => t.ObjectType == 2).Select(t => t.Amount).FirstOrDefault();
                        if (couponModel != null)
                        {
                            //使用了优惠券
                            if (couponModel.CouponType == 0)
                            {
                                //当优惠券为店铺优惠券时
                                foreach (var orderItem in orderitemlist)
                                {
                                    if (yjbInfo != null && yjbInfo.Items != null)
                                    {
                                        var currentCommodityYjbInfo = yjbInfo.Items.FirstOrDefault(c => c.CommodityId == orderItem.CommodityId);
                                        if (currentCommodityYjbInfo != null && currentCommodityYjbInfo.InsteadCashAmount > 0)
                                        {
                                            orderItem.YjbPrice = currentCommodityYjbInfo.InsteadCashAmount;
                                        }
                                    }

                                    decimal orderItemAmount = 0;
                                    if (orderItem.DiscountPrice.Value > 0)
                                    {
                                        orderItemAmount = orderItem.DiscountPrice.Value * orderItem.Number;
                                    }
                                    else
                                    {
                                        orderItemAmount = orderItem.RealPrice.Value * orderItem.Number;
                                    }

                                    if (orderItem.YjbPrice != null)
                                    {
                                        orderItemAmount = orderItemAmount - (decimal)orderItem.YjbPrice;
                                    }
                                    decimal sharePercent = globalAmount;
                                    if (sharePercent <= 0)
                                    {
                                        var sharePercentTmp =
                                             commodityShareList.Where(t => t.Id == orderItem.CommodityId)
                                                               .Select(t => t.SharePercent)
                                                               .FirstOrDefault();
                                        sharePercent = sharePercentTmp.HasValue ? sharePercentTmp.Value : 0m;
                                    }
                                    decimal moneyTmp = (orderItemAmount -
                                                        (orderItemAmount / commodityOrder.Price) *
                                                        couponModel.Amount -
                                                        (orderItemAmount / commodityOrder.Price) *
                                                        integrationAmount) * sharePercent;

                                    OrderItemShare oiShare = OrderItemShare.CreateOrderItemShare();
                                    oiShare.Id = Guid.NewGuid();
                                    oiShare.SubTime = DateTime.Now;
                                    oiShare.ModifiedOn = DateTime.Now;
                                    oiShare.SubId = commodityOrder.UserId;
                                    oiShare.SharePrice = orderItemAmount;
                                    oiShare.Commission = getCommisionWithCost(moneyTmp, 0);
                                    oiShare.ShouldCommission = moneyTmp.ToMoney();
                                    oiShare.SharePercent = sharePercent;
                                    oiShare.OrderId = orderItem.CommodityOrderId;
                                    oiShare.OrderItemId = orderItem.Id;
                                    oiShare.PayeeType = 3;
                                    oiShare.PayeeId = shareUseId;
                                    oiShare.ShareKey = shareId;
                                    oiShare.EntityState = EntityState.Added;
                                    contextSession.SaveObject(oiShare);

                                    commission += oiShare.Commission;
                                    shouldCommission += oiShare.ShouldCommission;

                                    LogHelper.Info("PushShareMoneyExt方法获取OrderItemShare44：" + JsonHelper.JsonSerializer(oiShare.ToEntityData()));
                                }
                            }
                            else if (couponModel.CouponType == 1)
                            {
                                //当优惠券为商品优惠券时
                                var couponCommodityAccount =
                                    orderitemlist.Where(
                                        t => couponModel.CommodityIds.Contains(t.CommodityId.ToString()))
                                                 .Sum(t => t.RealPrice.Value * t.Number);

                                if (couponCommodityAccount > 0)
                                {
                                    foreach (var orderItem in orderitemlist)
                                    {
                                        if (yjbInfo != null && yjbInfo.Items != null)
                                        {
                                            var currentCommodityYjbInfo = yjbInfo.Items.FirstOrDefault(c => c.CommodityId == orderItem.CommodityId);
                                            if (currentCommodityYjbInfo != null && currentCommodityYjbInfo.InsteadCashAmount > 0)
                                            {
                                                orderItem.YjbPrice = currentCommodityYjbInfo.InsteadCashAmount;
                                            }
                                        }

                                        decimal orderItemAmount = 0;
                                        if (orderItem.DiscountPrice.Value > 0)
                                        {
                                            orderItemAmount = orderItem.DiscountPrice.Value * orderItem.Number;
                                        }
                                        else
                                        {
                                            orderItemAmount = orderItem.RealPrice.Value * orderItem.Number;
                                        }

                                        if (orderItem.YjbPrice != null)
                                        {
                                            orderItemAmount = orderItemAmount - (decimal)orderItem.YjbPrice;
                                        }
                                        decimal moneyTmp = 0m;
                                        decimal sharePercent = globalAmount;
                                        if (sharePercent <= 0)
                                        {
                                            var sharePercentTmp =
                                                 commodityShareList.Where(t => t.Id == orderItem.CommodityId)
                                                                   .Select(t => t.SharePercent)
                                                                   .FirstOrDefault();
                                            sharePercent = sharePercentTmp.HasValue ? sharePercentTmp.Value : 0m;
                                        }
                                        if (couponModel.CommodityIds.Contains(orderItem.CommodityId.ToString()))
                                        {
                                            moneyTmp = (orderItemAmount -
                                                        (orderItemAmount /
                                                         couponCommodityAccount) *
                                                        couponModel.Amount -
                                                        (orderItemAmount / commodityOrder.Price) *
                                                        integrationAmount) * sharePercent;
                                        }
                                        else
                                        {
                                            moneyTmp = (orderItemAmount -
                                                        (orderItemAmount / commodityOrder.Price) *
                                                        integrationAmount) * sharePercent;
                                        }

                                        OrderItemShare oiShare = OrderItemShare.CreateOrderItemShare();
                                        oiShare.Id = Guid.NewGuid();
                                        oiShare.SubTime = DateTime.Now;
                                        oiShare.ModifiedOn = DateTime.Now;
                                        oiShare.SubId = commodityOrder.UserId;
                                        oiShare.SharePrice = orderItemAmount;
                                        oiShare.Commission = getCommisionWithCost(moneyTmp, 0);
                                        oiShare.ShouldCommission = moneyTmp.ToMoney();
                                        oiShare.SharePercent = sharePercent;
                                        oiShare.OrderId = orderItem.CommodityOrderId;
                                        oiShare.OrderItemId = orderItem.Id;
                                        oiShare.PayeeType = 3;
                                        oiShare.PayeeId = shareUseId;
                                        oiShare.ShareKey = shareId;
                                        oiShare.EntityState = EntityState.Added;
                                        contextSession.SaveObject(oiShare);

                                        commission += oiShare.Commission;
                                        shouldCommission += oiShare.ShouldCommission;

                                        LogHelper.Info("PushShareMoneyExt方法获取OrderItemShare11：" + JsonHelper.JsonSerializer(oiShare.ToEntityData()));
                                    }
                                }
                            }
                        }
                        else
                        {
                            //没有使用优惠券
                            foreach (var orderItem in orderitemlist)
                            {
                                if (yjbInfo != null && yjbInfo.Items != null)
                                {
                                    var currentCommodityYjbInfo = yjbInfo.Items.FirstOrDefault(c => c.CommodityId == orderItem.CommodityId);
                                    if (currentCommodityYjbInfo != null && currentCommodityYjbInfo.InsteadCashAmount > 0)
                                    {
                                        orderItem.YjbPrice = currentCommodityYjbInfo.InsteadCashAmount;
                                    }
                                }

                                decimal orderItemAmount = 0;
                                if (orderItem.DiscountPrice.Value > 0)
                                {
                                    orderItemAmount = orderItem.DiscountPrice.Value * orderItem.Number;
                                }
                                else
                                {
                                    orderItemAmount = orderItem.RealPrice.Value * orderItem.Number;
                                }

                                if (orderItem.YjbPrice != null)
                                {
                                    orderItemAmount = orderItemAmount - (decimal)orderItem.YjbPrice;
                                }
                                if (orderItem.CouponPrice != null)
                                {
                                    orderItemAmount = orderItemAmount - (decimal)orderItem.CouponPrice;
                                }
                                decimal sharePercent = globalAmount;
                                if (sharePercent <= 0)
                                {
                                    var sharePercentTmp =
                                         commodityShareList.Where(t => t.Id == orderItem.CommodityId)
                                                           .Select(t => t.SharePercent)
                                                           .FirstOrDefault();
                                    sharePercent = sharePercentTmp.HasValue ? sharePercentTmp.Value : 0m;
                                }
                                decimal moneyTmp = (orderItemAmount -
                                                    (orderItemAmount / commodityOrder.Price) *
                                                    integrationAmount) * sharePercent;

                                OrderItemShare oiShare = OrderItemShare.CreateOrderItemShare();
                                oiShare.Id = Guid.NewGuid();
                                oiShare.SubTime = DateTime.Now;
                                oiShare.ModifiedOn = DateTime.Now;
                                oiShare.SubId = commodityOrder.UserId;
                                oiShare.SharePrice = orderItemAmount;
                                oiShare.Commission = getCommisionWithCost(moneyTmp, 0);
                                oiShare.ShouldCommission = moneyTmp.ToMoney();
                                oiShare.SharePercent = sharePercent;
                                oiShare.OrderId = orderItem.CommodityOrderId;
                                oiShare.OrderItemId = orderItem.Id;
                                oiShare.PayeeType = 3;
                                oiShare.PayeeId = shareUseId;
                                oiShare.ShareKey = shareId;
                                oiShare.EntityState = EntityState.Added;
                                contextSession.SaveObject(oiShare);

                                commission += oiShare.Commission;
                                shouldCommission += oiShare.ShouldCommission;

                                LogHelper.Info("PushShareMoneyExt方法获取OrderItemShare22：" + JsonHelper.JsonSerializer(oiShare.ToEntityData()));
                            }
                        }
                    }
                    else
                    {
                        foreach (var orderItem in orderitemlist)
                        {
                            if (yjbInfo != null && yjbInfo.Items != null)
                            {
                                var currentCommodityYjbInfo = yjbInfo.Items.FirstOrDefault(c => c.CommodityId == orderItem.CommodityId);
                                if (currentCommodityYjbInfo != null && currentCommodityYjbInfo.InsteadCashAmount > 0)
                                {
                                    orderItem.YjbPrice = currentCommodityYjbInfo.InsteadCashAmount;
                                }
                            }

                            decimal orderItemAmount = 0;
                            if (orderItem.DiscountPrice.Value > 0)
                            {
                                orderItemAmount = orderItem.DiscountPrice.Value * orderItem.Number;
                            }
                            else
                            {
                                orderItemAmount = orderItem.RealPrice.Value * orderItem.Number;
                            }

                            if (orderItem.YjbPrice != null)
                            {
                                orderItemAmount = orderItemAmount - (decimal)orderItem.YjbPrice;
                            }
                            if (orderItem.CouponPrice != null)
                            {
                                orderItemAmount = orderItemAmount - (decimal)orderItem.CouponPrice;
                            }
                            decimal sharePercent = globalAmount;
                            if (sharePercent <= 0)
                            {
                                var sharePercentTmp =
                                     commodityShareList.Where(t => t.Id == orderItem.CommodityId)
                                                       .Select(t => t.SharePercent)
                                                       .FirstOrDefault();
                                sharePercent = sharePercentTmp.HasValue ? sharePercentTmp.Value : 0m;
                            }
                            decimal moneyTmp = orderItemAmount * sharePercent;

                            OrderItemShare oiShare = OrderItemShare.CreateOrderItemShare();
                            oiShare.Id = Guid.NewGuid();
                            oiShare.SubTime = DateTime.Now;
                            oiShare.ModifiedOn = DateTime.Now;
                            oiShare.SubId = commodityOrder.UserId;
                            oiShare.SharePrice = orderItemAmount;
                            oiShare.Commission = getCommisionWithCost(moneyTmp, 0);
                            oiShare.ShouldCommission = moneyTmp.ToMoney();
                            oiShare.SharePercent = sharePercent;
                            oiShare.OrderId = orderItem.CommodityOrderId;
                            oiShare.OrderItemId = orderItem.Id;
                            oiShare.PayeeType = 3;
                            oiShare.PayeeId = shareUseId;
                            oiShare.ShareKey = shareId;
                            oiShare.EntityState = EntityState.Added;
                            contextSession.SaveObject(oiShare);

                            commission += oiShare.Commission;
                            shouldCommission += oiShare.ShouldCommission;

                            LogHelper.Info("PushShareMoneyExt方法获取OrderItemShare33：" + JsonHelper.JsonSerializer(oiShare.ToEntityData()));
                        }
                    }

                    //众销分享佣金
                    commodityOrder.Commission = commission;

                    //保存众销分成到 OrderShare 表
                    if (shareUseId != Guid.Empty)
                    {
                        decimal sharePriceTotal = orderitemlist.Sum(t => t.RealPrice.Value * t.Number);

                        OrderShare os = OrderShare.CreateOrderShare();
                        os.Id = Guid.NewGuid();
                        os.SubTime = DateTime.Now;
                        os.ModifiedOn = DateTime.Now;
                        os.SubId = commodityOrder.UserId;
                        os.SharePrice = sharePriceTotal;
                        os.Commission = commission;
                        os.ShouldCommission = shouldCommission;
                        os.SharePercent = 0;
                        os.OrderId = commodityOrder.Id;
                        os.PayeeType = 3;
                        os.PayeeId = shareUseId;
                        os.ShareKey = shareId;

                        os.EntityState = EntityState.Added;
                        contextSession.SaveObject(os);

                        LogHelper.Info("PushShareMoneyExt方法获取OrderShare：" + JsonHelper.JsonSerializer(os.ToEntityData()));
                    }

                    contextSession.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Info("PushShareMoneyExt方法发生异常：" + ex);
                return false;
            }
        }
    }

    class ShortCom
    {
        public Guid Id { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public decimal? Duty { get; set; }
    }
    class TodayPromoDTO
    {
        public int? SurplusLimitBuyTotal { get; set; }
        public int Number { get; set; }
        public Guid ComdityID { get; set; }
        public Guid UserID { get; set; }
        public int? LimitBuyEach { get; set; }
        public Guid PromotionId { get; set; }
        public Guid ID { get; set; }
        public int? LimitBuyTotal { get; set; }
        public decimal Intensity { get; set; }
        public decimal? DiscountPrice { get; set; }
    }

    class OrderItemComments
    {
        public Guid OrderItemId { get; set; }
        public Guid OrderId { get; set; }
    }

    class OrderRefundTmp
    {
        public int State { get; set; }
        public Guid OrderId { get; set; }
        public Guid? OrderItemId { get; set; }
        public DateTime SubTime { get; set; }
        public string RefundExpCo { get; set; }
        public string RefundExpOrderNo { get; set; }
    }
}
