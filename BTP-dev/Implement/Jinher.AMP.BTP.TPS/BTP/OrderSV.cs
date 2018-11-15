using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Jinher.AMP.App.Deploy.CustomDTO;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.BE.BELogic;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.FSP.Deploy.CustomDTO;
using Jinher.JAP.BF.BE.Base;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.BF.BE.Deploy.Base;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;
using Jinher.AMP.BTP.TPS.Helper;

namespace Jinher.AMP.BTP.TPS
{
    /// <summary>
    /// 订单公用方法类
    /// </summary>
    public class OrderSV
    {
        /// <summary>
        /// 生成确认收货model
        /// </summary>
        /// <param name="contextSession">上下文</param>
        /// <param name="commodityOrder">订单</param>
        /// <param name="saveList">保存列表</param>
        /// <param name="applicationDTO">应用信息</param>
        /// <param name="isAfterSales"></param>
        /// <param name="password">支付密码</param>
        /// <param name="isSaveObject">是否保存对象</param>
        /// <returns></returns>
        private static ConfirmPayDTO BuildConfirmPayDTOInternal(ContextSession contextSession, CommodityOrder commodityOrder, out List<object> saveList, Jinher.AMP.App.Deploy.CustomDTO.AppIdOwnerIdTypeDTO applicationDTO, bool isAfterSales, string password = null, bool isSaveObject = true)
        {
            long realPriceGold = commodityOrder.RealPrice.Value.ToGold();

            saveList = new List<object>();

            List<BusinessObject> needSaveEntities;
            var list = BuildConfirmOrderPayees(commodityOrder, applicationDTO, isAfterSales, out needSaveEntities);
            if (list == null || !list.Any())
                return null;
            var saler = list.FirstOrDefault(c => c.PayeeType == 1);
            if (saler == null)
                return null;

            ConfirmPayDTO comConfirmPayDto = new ConfirmPayDTO
            {
                BizId = commodityOrder.Id,
                Password = password,
                PayGold = saler.PayMoney.ToGold(),
                Sign = CustomConfig.PaySing,
                UserId = commodityOrder.UserId,
                AppId = commodityOrder.AppId,
                PayeeId = applicationDTO.OwnerId,
                PayeesList = new List<PayeeInfoDTO>()
            };
            //完成售后才真正打款||服务订单没有售后，确认收货即打款
            if (isAfterSales || commodityOrder.OrderType == 1)
            {
                if (isSaveObject)
                {
                    contextSession.SaveObject(saler);
                }
                else
                {
                    saveList.Add(saler);
                }
            }

            foreach (var orderPayee in list.Where(c => c.PayeeType != 1).ToList())
            {

                comConfirmPayDto.PayeesList.Add(new PayeeInfoDTO
                {
                    PayeeId = orderPayee.PayeeId,
                    Description = orderPayee.Description,
                    IsJHSelfUseAccount = orderPayee.IsJHSelfUseAccount,
                    PayeeGold = orderPayee.PayMoney.ToGold(),
                    IsVoucherBuyGold = orderPayee.IsVoucherBuyGold
                });
                //完成售后才真正打款||服务订单没有售后，确认收货即打款
                if (isAfterSales || commodityOrder.OrderType == 1)
                {
                    if (isSaveObject)
                    {
                        contextSession.SaveObject(orderPayee);
                    }
                    else
                    {
                        saveList.Add(orderPayee);
                    }
                }
            }
            if (needSaveEntities != null && needSaveEntities.Any())
            {
                foreach (var businessObject in needSaveEntities)
                {
                    if (isSaveObject)
                    {
                        contextSession.SaveObject(businessObject);
                    }
                    else
                    {
                        saveList.Add(businessObject);
                    }
                }
            }

            return comConfirmPayDto;
        }

        /// <summary>
        /// 确认收货->生成订单打款记录
        /// </summary>
        /// <param name="commodityOrder"></param>
        /// <param name="applicationDTO"></param>
        /// <param name="isAfterSales"></param>
        /// <param name="needSaveEntities"></param>
        /// <returns></returns>
        public static List<OrderPayee> BuildConfirmOrderPayees(CommodityOrder commodityOrder, Jinher.AMP.App.Deploy.CustomDTO.AppIdOwnerIdTypeDTO applicationDTO, bool isAfterSales, out List<BusinessObject> needSaveEntities)
        {
            // 判断是否为担保交易
            if (isAfterSales)
            {
                if (SettleAccountHelper.CheckPayType(commodityOrder))
                {
                    // 判断是否入驻
                    var mall = SettleAccountHelper.GetMallApply(commodityOrder);
                    if (mall != null && (mall.Type == 1 || mall.Type == 2))
                    {
                        return BuildConfirmOrderSettlePayees(mall, commodityOrder, applicationDTO, isAfterSales, out needSaveEntities);
                    }
                }
            }
            List<OrderPayee> result = new List<OrderPayee>();
            DateTime now = DateTime.Now;
            decimal shareAmount = decimal.Zero;
            needSaveEntities = new List<BusinessObject>();

            //商家打款
            //处理单商品退款的情况
            decimal rePrice = 0;
            var sum = OrderItem.ObjectSet()
                .Where(t => t.CommodityOrderId == commodityOrder.Id && t.State != 0 && t.State != 2 && t.State != 3)
                .Sum(t => t.RealPrice * t.Number);
            if (sum != null)
                rePrice = (decimal)sum;

            OrderPayee orderPayeeSeller = OrderPayee.CreateOrderPayee(commodityOrder.Id, applicationDTO.OwnerId, false, false, commodityOrder.RealPrice.Value - rePrice, 1, "商家收款");

            decimal realPrice = commodityOrder.IsModifiedPrice ? commodityOrder.RealPrice.Value : commodityOrder.RealPrice.Value - commodityOrder.Freight;
            if (realPrice <= 0)
            {
                result.Add(orderPayeeSeller);
                return result;
            }
            var isZPHApp = ZPHSV.Instance.CheckIsAppInZPH(commodityOrder.AppId);
            LogHelper.Debug(string.Format("订单分润 订单Id：{0} , 是否需要订单分润{1}", commodityOrder.Id, commodityOrder.PaymentTime >= CustomConfig.MinClearningOrderDate && isZPHApp));

            decimal payeeTradeMoney = decimal.Zero;//商贸众销
            decimal jinherShareMoney = decimal.Zero;//金和众销

            #region 渠道

            if (commodityOrder.ChannelShareMoney > 0)
            {
                if (!CustomConfig.IsShareAsScore)
                {
                    //TODO 渠道暂不支持非积分，需要时，请加上
                    throw new Exception("渠道暂不支持非积分");
                }
                else
                {
                    orderPayeeSeller.PayMoney -= commodityOrder.ChannelShareMoney;
                    Guid payeeIdTmp =
                        OrderShare.ObjectSet()
                                  .Where(t => t.OrderId == commodityOrder.Id && t.PayeeType == 12)
                                  .Select(t => t.PayeeId)
                                  .FirstOrDefault();
                    OrderPayee channelPayeeSeller = OrderPayee.CreateOrderPayee(commodityOrder.Id, payeeIdTmp, false, false, commodityOrder.ChannelShareMoney, 12, "渠道推广");
                    result.Add(channelPayeeSeller);
                }
            }

            #endregion

            #region 推广主

            if (commodityOrder.SpreadGold > 0)
            {
                decimal spreadMoney = commodityOrder.SpreadGold.ToMoney();
                orderPayeeSeller.PayMoney -= spreadMoney;
                Guid payeeIdTmp =
                    OrderShare.ObjectSet()
                              .Where(t => t.OrderId == commodityOrder.Id && t.PayeeType == 5)
                              .Select(t => t.PayeeId)
                              .FirstOrDefault();
                OrderPayee channelPayeeSeller = OrderPayee.CreateOrderPayee(commodityOrder.Id, payeeIdTmp, false, false, spreadMoney, 5, "推广主");
                result.Add(channelPayeeSeller);
            }

            #endregion

            #region 众销 (目前应用主分成按照众销处理)
            //33为分享订单，34为分享商品   众销处理
            if (!CustomConfig.IsShareAsScore && commodityOrder.Commission > 0)
            {
                //众销分红
                decimal commission = commodityOrder.Commission.ToMoney();
                shareAmount += commission;
                decimal tradeMoney = (commission * CustomConfig.SaleShare.BuyerPercent).ToMoney();  //商贸众销
                decimal jinherMoney = (commission - tradeMoney);  //金和众销

                //商贸众销
                payeeTradeMoney += tradeMoney;
                if (payeeTradeMoney > 0)
                {
                    int shareDivCount = ShareDividend.ObjectSet().Count(x => x.CommodityOrderId == commodityOrder.Id && x.ShareType == 0);
                    if (shareDivCount == 0)
                    {
                        var shareDividend = new ShareDividend
                        {
                            Id = Guid.NewGuid(),
                            Money = commission,
                            SettlementDate = now,
                            AppId = commodityOrder.AppId,
                            ModifiedOn = now,
                            State = 0,
                            CommodityOrderId = commodityOrder.Id,
                            SharerMoney = tradeMoney.ToGold(),
                            ShareType = 0,
                            EntityState = EntityState.Added
                        };
                        //售中不保存分成表
                        if (isAfterSales || commodityOrder.OrderType == 1)
                            needSaveEntities.Add(shareDividend);
                    }
                }
                //金和众销
                jinherShareMoney += jinherMoney;
            }

            //代运营的订单才参加 应用主分成、推广主分成
            if (isZPHApp)
            {

                //同时存在应用主、推广主只给应用主分成---已作废
                if (commodityOrder.SrcAppId.HasValue && commodityOrder.SrcAppId != Guid.Empty)
                {
                    #region 应用主   --作废
                    //应用主分红
                    //decimal ownerShare = (realPrice * CustomConfig.ShareOwner.OwnerPercent).ToMoney();
                    ////商贸众销（应用主分红暂时放如商贸众销帐号）
                    //if (ownerShare > 0)
                    //{
                    //    shareAmount += ownerShare;
                    //    payeeTradeMoney += ownerShare;

                    //    int shareDivCount = ShareDividend.ObjectSet().Count(x => x.CommodityOrderId == commodityOrder.Id && x.ShareType == 1);
                    //    if (shareDivCount == 0)
                    //    {
                    //        var ownerDividend = new ShareDividend
                    //        {
                    //            Id = Guid.NewGuid(),
                    //            Money = ownerShare,
                    //            SettlementDate = now,
                    //            AppId = commodityOrder.SrcAppId.Value,
                    //            ModifiedOn = now,
                    //            State = 0,
                    //            CommodityOrderId = commodityOrder.Id,
                    //            SharerMoney = ownerShare.ToGold(),
                    //            ShareType = 1,
                    //            EntityState = EntityState.Added
                    //        };
                    //        //售中不保存分成表
                    //        if (isAfterSales || commodityOrder.OrderType == 1)
                    //            needSaveEntities.Add(ownerDividend);
                    //    }
                    //    commodityOrder.OwnerShare = ownerShare;
                    //}

                    #endregion
                }
            }

            if (payeeTradeMoney > 0)
            {
                result.Add(OrderPayee.CreateOrderPayee(commodityOrder.Id, CustomConfig.ShareGoldAccout.BTPShareGoldAccount, true, CustomConfig.ShareGoldAccout.IsBtpVoucherBuyGold, payeeTradeMoney, 3, "商贸众销"));
            }
            if (jinherShareMoney > 0)
            {
                result.Add(OrderPayee.CreateOrderPayee(commodityOrder.Id, CustomConfig.ShareGoldAccout.JHShareGoldAccount, true, CustomConfig.ShareGoldAccout.IsJhVoucherBuyGold, jinherShareMoney, 2, "金和众销"));
            }

            #endregion

            #region 众筹
            //众筹处理
            if (commodityOrder.IsCrowdfunding != 0)
            {
                var crowdfunding = Crowdfunding.ObjectSet().FirstOrDefault(q => q.AppId == commodityOrder.AppId && q.StartTime < now);
                if (crowdfunding != null)
                {
                    var yestorday = DateTime.Today.AddDays(-1);
                    //众筹股东每日统计
                    var crowdfundingDailyQuery = CrowdfundingDaily.ObjectSet().FirstOrDefault(q => q.CrowdfundingId == crowdfunding.Id && q.SettlementDate == yestorday);
                    if (crowdfundingDailyQuery != null)
                    {

                        decimal tradeCrowdfunding = (crowdfundingDailyQuery.CurrentShareCount * crowdfunding.DividendPercent * realPrice).ToMoney();
                        shareAmount += tradeCrowdfunding;

                        if (tradeCrowdfunding > 0)
                        {
                            CfOrderDividend cfOrderDividend = CfOrderDividend.CreateCfOrderDividend();
                            cfOrderDividend.Gold = tradeCrowdfunding.ToGold();
                            cfOrderDividend.AppId = commodityOrder.AppId;
                            cfOrderDividend.State = 0;
                            cfOrderDividend.CurrentShareCount = crowdfundingDailyQuery.CurrentShareCount;
                            cfOrderDividend.CommodityOrderId = commodityOrder.Id;
                            needSaveEntities.Add(cfOrderDividend);

                            var cfcnt = CrowdfundingCount.ObjectSet().FirstOrDefault(c => c.AppId == commodityOrder.AppId);
                            if (cfcnt != null)
                            {
                                cfcnt.TotalDividend += tradeCrowdfunding.ToGold();
                                needSaveEntities.Add(cfcnt);
                            }

                            result.Add(OrderPayee.CreateOrderPayee(commodityOrder.Id, CustomConfig.CrowdfundingAccount.BTPCrowdfundingAcount, true, CustomConfig.CrowdfundingAccount.IsBtpVoucherBuyGold, tradeCrowdfunding, 4, "商贸众筹"));

                            orderPayeeSeller.PayMoney -= tradeCrowdfunding;
                        }
                    }
                }
            }
            #endregion

            orderPayeeSeller.PayMoney -= shareAmount;

            #region 三级分销
            //
            //只有非代运营app才参加三级分销   20160307 from wangfang  
            if (!CustomConfig.IsShareAsScore)
            {
                //三级分销类型列表
                List<int> distributorPayeeType = new List<int>() { 9, 10, 11 };
                var osQuery = (from os in OrderShare.ObjectSet()
                               where os.OrderId == commodityOrder.Id && os.Commission > 0 && distributorPayeeType.Contains(os.PayeeType)
                               select os).ToList();
                if (osQuery.Any())
                {
                    foreach (OrderShare os in osQuery)
                    {
                        var desc = "";
                        if (os.PayeeType == 9)
                        {
                            desc = "一级分销";
                        }
                        else if (os.PayeeType == 10)
                        {
                            desc = "二级分销";
                        }
                        else if (os.PayeeType == 11)
                        {
                            desc = "三级分销";
                        }

                        OrderPayee op = OrderPayee.CreateOrderPayee();
                        op.Id = Guid.NewGuid();
                        op.SubTime = DateTime.Now;
                        op.OrderId = os.OrderId;
                        op.PayeeId = os.PayeeId;
                        op.PayeeType = os.PayeeType;
                        op.Description = desc;
                        op.IsJHSelfUseAccount = false;
                        op.IsVoucherBuyGold = false;
                        op.PayMoney = os.Commission;
                        op.ModifiedOn = DateTime.Now;
                        op.ReCheckFlag = 1;

                        result.Add(op);
                    }

                    orderPayeeSeller.PayMoney -= commodityOrder.DistributeMoney;
                }
            }
            #endregion

            #region 分润
            if (commodityOrder.PaymentTime >= CustomConfig.MinClearningOrderDate && isZPHApp)
            {
                decimal clearingPrice;
                var orderEx = BuildOrderException(commodityOrder, APPSV.GetAppName(commodityOrder.AppId), now, shareAmount, out clearingPrice, null);
                if (orderEx != null)
                {
                    needSaveEntities.Add(orderEx);

                    switch (orderEx.ExceptionType)
                    {
                        case 0:
                            //未设置结算价，把钱打到金和账户
                            result.Add(OrderPayee.CreateOrderPayee(commodityOrder.Id, CustomConfig.JinherBtpAccount.BtpGoldAccount, true, CustomConfig.JinherBtpAccount.IsVoucherBuyGold, orderPayeeSeller.PayMoney, 7, "商贸分润"));

                            orderPayeeSeller.PayMoney = 0;
                            break;
                        case 1:
                        case 2:
                            break;
                    }

                }
                else
                {
                    clearingPrice = clearingPrice.ToMoney();
                    var jinher = orderPayeeSeller.PayMoney - clearingPrice;
                    if (jinher > 0)
                        result.Add(OrderPayee.CreateOrderPayee(commodityOrder.Id, CustomConfig.JinherBtpAccount.BtpGoldAccount, true, CustomConfig.JinherBtpAccount.IsVoucherBuyGold, jinher, 7, "商贸分润"));

                    orderPayeeSeller.PayMoney = clearingPrice;
                }
            }
            #endregion

            result.Add(orderPayeeSeller);
            return result;
        }

        private static List<OrderPayee> BuildConfirmOrderSettlePayees(MallApply mall, CommodityOrder commodityOrder, Jinher.AMP.App.Deploy.CustomDTO.AppIdOwnerIdTypeDTO applicationDTO, bool isAfterSales, out List<BusinessObject> needSaveEntities)
        {
            List<OrderPayee> result = new List<OrderPayee>();
            DateTime now = DateTime.Now;
            decimal shareAmount = decimal.Zero;
            needSaveEntities = new List<BusinessObject>();

            LogHelper.Info("OrderSV.BuildConfirmOrderSettlePayees 生产结算打款记录，OrderId:" + commodityOrder.Id + "---------------------Begin");
            var settleDetails = SettleAccountHelper.CreateSettleAccountDetails(null, commodityOrder, mall);
            if (settleDetails == null)
            {
                // 未设置结算价，不结算。
                LogHelper.Info("OrderSV.BuildConfirmOrderSettlePayees 生产结算打款记录，OrderId:" + commodityOrder.Id + "未设置结算价");
                return null;
            }

            //商家打款
            OrderPayee orderPayeeSeller = OrderPayee.CreateOrderPayee(commodityOrder.Id, applicationDTO.OwnerId, false, false, settleDetails.SellerAmount, 1, "商家收款");
            LogHelper.Info("OrderSV.BuildConfirmOrderSettlePayees 生产结算打款记录，OrderId:" + commodityOrder.Id + "商家分润：" + orderPayeeSeller.PayMoney);

            var mallApplicationDTO = APPSV.Instance.GetAppOwnerInfo(commodityOrder.EsAppId.Value, AuthorizeHelper.CoinInitAuthorizeInfo());
            OrderPayee orderPayeeMall = OrderPayee.CreateOrderPayee(commodityOrder.Id, mallApplicationDTO.OwnerId, false, false, settleDetails.PromotionAmount, 30, "商城分润");
            result.Add(orderPayeeMall);
            LogHelper.Info("OrderSV.BuildConfirmOrderSettlePayees 生产结算打款记录，OrderId:" + commodityOrder.Id + "商城分润：" + orderPayeeMall.PayMoney);

            decimal realPrice = commodityOrder.IsModifiedPrice ? commodityOrder.RealPrice.Value : commodityOrder.RealPrice.Value - commodityOrder.Freight;
            if (realPrice <= 0)
            {
                result.Add(orderPayeeSeller);
                return result;
            }
            var isZPHApp = ZPHSV.Instance.CheckIsAppInZPH(commodityOrder.AppId);
            LogHelper.Debug(string.Format("订单分润 订单Id：{0} , 是否需要订单分润{1}", commodityOrder.Id, commodityOrder.PaymentTime >= CustomConfig.MinClearningOrderDate && isZPHApp));

            decimal payeeTradeMoney = decimal.Zero;//商贸众销
            decimal jinherShareMoney = decimal.Zero;//金和众销

            #region 渠道

            if (commodityOrder.ChannelShareMoney > 0)
            {
                if (!CustomConfig.IsShareAsScore)
                {
                    //TODO 渠道暂不支持非积分，需要时，请加上
                    throw new Exception("渠道暂不支持非积分");
                }
                else
                {

                    Guid payeeIdTmp =
                        OrderShare.ObjectSet()
                                  .Where(t => t.OrderId == commodityOrder.Id && t.PayeeType == 12)
                                  .Select(t => t.PayeeId)
                                  .FirstOrDefault();
                    OrderPayee channelPayeeSeller = OrderPayee.CreateOrderPayee(commodityOrder.Id, payeeIdTmp, false, false, commodityOrder.ChannelShareMoney, 12, "渠道推广");
                    result.Add(channelPayeeSeller);

                    //orderPayeeSeller.PayMoney -= commodityOrder.ChannelShareMoney;
                    LogHelper.Info("OrderSV.BuildConfirmOrderSettlePayees 生产结算打款记录，OrderId:" + commodityOrder.Id + "渠道分润：" + commodityOrder.ChannelShareMoney);
                }
            }

            #endregion

            #region 推广主

            if (commodityOrder.SpreadGold > 0)
            {
                decimal spreadMoney = commodityOrder.SpreadGold.ToMoney();
                Guid payeeIdTmp =
                    OrderShare.ObjectSet()
                              .Where(t => t.OrderId == commodityOrder.Id && t.PayeeType == 5)
                              .Select(t => t.PayeeId)
                              .FirstOrDefault();
                OrderPayee channelPayeeSeller = OrderPayee.CreateOrderPayee(commodityOrder.Id, payeeIdTmp, false, false, spreadMoney, 5, "推广主");
                result.Add(channelPayeeSeller);

                //orderPayeeSeller.PayMoney -= spreadMoney;
                LogHelper.Info("OrderSV.BuildConfirmOrderSettlePayees 生产结算打款记录，OrderId:" + commodityOrder.Id + "推广主分润：" + spreadMoney);
            }

            #endregion

            #region 众销 (目前应用主分成按照众销处理)
            //33为分享订单，34为分享商品   众销处理
            if (!CustomConfig.IsShareAsScore && commodityOrder.Commission > 0)
            {
                //众销分红
                decimal commission = commodityOrder.Commission.ToMoney();
                shareAmount += commission;
                decimal tradeMoney = (commission * CustomConfig.SaleShare.BuyerPercent).ToMoney();  //商贸众销
                decimal jinherMoney = (commission - tradeMoney);  //金和众销

                //商贸众销
                payeeTradeMoney += tradeMoney;
                if (payeeTradeMoney > 0)
                {
                    int shareDivCount = ShareDividend.ObjectSet().Count(x => x.CommodityOrderId == commodityOrder.Id && x.ShareType == 0);
                    if (shareDivCount == 0)
                    {
                        var shareDividend = new ShareDividend
                        {
                            Id = Guid.NewGuid(),
                            Money = commission,
                            SettlementDate = now,
                            AppId = commodityOrder.AppId,
                            ModifiedOn = now,
                            State = 0,
                            CommodityOrderId = commodityOrder.Id,
                            SharerMoney = tradeMoney.ToGold(),
                            ShareType = 0,
                            EntityState = EntityState.Added
                        };
                        //售中不保存分成表
                        if (isAfterSales || commodityOrder.OrderType == 1)
                            needSaveEntities.Add(shareDividend);
                    }
                }
                //金和众销
                jinherShareMoney += jinherMoney;
            }

            //代运营的订单才参加 应用主分成、推广主分成
            if (isZPHApp)
            {

                //同时存在应用主、推广主只给应用主分成---已作废
                if (commodityOrder.SrcAppId.HasValue && commodityOrder.SrcAppId != Guid.Empty)
                {
                    #region 应用主   --作废
                    //应用主分红
                    //decimal ownerShare = (realPrice * CustomConfig.ShareOwner.OwnerPercent).ToMoney();
                    ////商贸众销（应用主分红暂时放如商贸众销帐号）
                    //if (ownerShare > 0)
                    //{
                    //    shareAmount += ownerShare;
                    //    payeeTradeMoney += ownerShare;

                    //    int shareDivCount = ShareDividend.ObjectSet().Count(x => x.CommodityOrderId == commodityOrder.Id && x.ShareType == 1);
                    //    if (shareDivCount == 0)
                    //    {
                    //        var ownerDividend = new ShareDividend
                    //        {
                    //            Id = Guid.NewGuid(),
                    //            Money = ownerShare,
                    //            SettlementDate = now,
                    //            AppId = commodityOrder.SrcAppId.Value,
                    //            ModifiedOn = now,
                    //            State = 0,
                    //            CommodityOrderId = commodityOrder.Id,
                    //            SharerMoney = ownerShare.ToGold(),
                    //            ShareType = 1,
                    //            EntityState = EntityState.Added
                    //        };
                    //        //售中不保存分成表
                    //        if (isAfterSales || commodityOrder.OrderType == 1)
                    //            needSaveEntities.Add(ownerDividend);
                    //    }
                    //    commodityOrder.OwnerShare = ownerShare;
                    //}

                    #endregion
                }
            }

            if (payeeTradeMoney > 0)
            {
                result.Add(OrderPayee.CreateOrderPayee(commodityOrder.Id, CustomConfig.ShareGoldAccout.BTPShareGoldAccount, true, CustomConfig.ShareGoldAccout.IsBtpVoucherBuyGold, payeeTradeMoney, 3, "商贸众销"));
            }
            if (jinherShareMoney > 0)
            {
                result.Add(OrderPayee.CreateOrderPayee(commodityOrder.Id, CustomConfig.ShareGoldAccout.JHShareGoldAccount, true, CustomConfig.ShareGoldAccout.IsJhVoucherBuyGold, jinherShareMoney, 2, "金和众销"));
            }

            #endregion

            #region 众筹
            //众筹处理
            if (commodityOrder.IsCrowdfunding != 0)
            {
                var crowdfunding = Crowdfunding.ObjectSet().FirstOrDefault(q => q.AppId == commodityOrder.AppId && q.StartTime < now);
                if (crowdfunding != null)
                {
                    var yestorday = DateTime.Today.AddDays(-1);
                    //众筹股东每日统计
                    var crowdfundingDailyQuery = CrowdfundingDaily.ObjectSet().FirstOrDefault(q => q.CrowdfundingId == crowdfunding.Id && q.SettlementDate == yestorday);
                    if (crowdfundingDailyQuery != null)
                    {

                        decimal tradeCrowdfunding = (crowdfundingDailyQuery.CurrentShareCount * crowdfunding.DividendPercent * realPrice).ToMoney();
                        shareAmount += tradeCrowdfunding;

                        if (tradeCrowdfunding > 0)
                        {
                            CfOrderDividend cfOrderDividend = CfOrderDividend.CreateCfOrderDividend();
                            cfOrderDividend.Gold = tradeCrowdfunding.ToGold();
                            cfOrderDividend.AppId = commodityOrder.AppId;
                            cfOrderDividend.State = 0;
                            cfOrderDividend.CurrentShareCount = crowdfundingDailyQuery.CurrentShareCount;
                            cfOrderDividend.CommodityOrderId = commodityOrder.Id;
                            needSaveEntities.Add(cfOrderDividend);

                            var cfcnt = CrowdfundingCount.ObjectSet().FirstOrDefault(c => c.AppId == commodityOrder.AppId);
                            if (cfcnt != null)
                            {
                                cfcnt.TotalDividend += tradeCrowdfunding.ToGold();
                                needSaveEntities.Add(cfcnt);
                            }

                            result.Add(OrderPayee.CreateOrderPayee(commodityOrder.Id, CustomConfig.CrowdfundingAccount.BTPCrowdfundingAcount, true, CustomConfig.CrowdfundingAccount.IsBtpVoucherBuyGold, tradeCrowdfunding, 4, "商贸众筹"));

                            orderPayeeSeller.PayMoney -= tradeCrowdfunding;
                        }
                    }
                }
            }
            #endregion

            orderPayeeSeller.PayMoney -= shareAmount;

            #region 三级分销
            //
            //只有非代运营app才参加三级分销   20160307 from wangfang  
            if (!CustomConfig.IsShareAsScore)
            {
                //三级分销类型列表
                List<int> distributorPayeeType = new List<int>() { 9, 10, 11 };
                var osQuery = (from os in OrderShare.ObjectSet()
                               where os.OrderId == commodityOrder.Id && os.Commission > 0 && distributorPayeeType.Contains(os.PayeeType)
                               select os).ToList();
                if (osQuery.Any())
                {
                    foreach (OrderShare os in osQuery)
                    {
                        var desc = "";
                        if (os.PayeeType == 9)
                        {
                            desc = "一级分销";
                        }
                        else if (os.PayeeType == 10)
                        {
                            desc = "二级分销";
                        }
                        else if (os.PayeeType == 11)
                        {
                            desc = "三级分销";
                        }

                        OrderPayee op = OrderPayee.CreateOrderPayee();
                        op.Id = Guid.NewGuid();
                        op.SubTime = DateTime.Now;
                        op.OrderId = os.OrderId;
                        op.PayeeId = os.PayeeId;
                        op.PayeeType = os.PayeeType;
                        op.Description = desc;
                        op.IsJHSelfUseAccount = false;
                        op.IsVoucherBuyGold = false;
                        op.PayMoney = os.Commission;
                        op.ModifiedOn = DateTime.Now;
                        op.ReCheckFlag = 1;

                        result.Add(op);
                    }

                    //orderPayeeSeller.PayMoney -= commodityOrder.DistributeMoney;
                    LogHelper.Info("OrderSV.BuildConfirmOrderSettlePayees 生产结算打款记录，OrderId:" + commodityOrder.Id + "三级分销分润：" + commodityOrder.DistributeMoney);
                }
            }
            #endregion

            result.Add(orderPayeeSeller);

            LogHelper.Info("OrderSV.BuildConfirmOrderSettlePayees 生产结算打款记录---------------------End");
            return result;
        }

        /// <summary>
        /// 计算订单分润异常，如果没有异常返回null
        /// </summary>
        /// <param name="commodityOrder">订单实体</param>
        /// <param name="appName">应用名称</param>
        /// <param name="exTime">统计时间</param>
        /// <param name="shareMoney">订单需要分成金额</param>
        /// <param name="clearingPrice">应用主分成金额</param>
        /// <param name="orderService">售后表</param>
        /// <param name="isNeedCalcShare">是否需要计算订单分成（如果传如true则本方法会重新计算shareMoney，否则使用传入shareMoney字段）</param>
        /// <returns></returns>
        public static CommodityOrderException BuildOrderException(CommodityOrder commodityOrder, string appName, DateTime exTime, decimal shareMoney, out decimal clearingPrice, CommodityOrderService orderService, bool isNeedCalcShare = false)
        {
            CommodityOrderException result = null;
            decimal orderRealPrice = commodityOrder.RealPrice.Value;
            List<int> rState = new List<int> { 2, 3, 4, 13 };
            clearingPrice = 0.0m;
            try
            {
                var orderTime = commodityOrder.PaymentTime.HasValue ? commodityOrder.PaymentTime.Value : commodityOrder.SubTime;
                bool hasException = false;
                int exceptionType = 0;
                string message = "";

                var orderItems = OrderItem.ObjectSet().Where(c => c.CommodityOrderId == commodityOrder.Id).ToList();
                var comIds = orderItems.Select(c => c.CommodityId).ToList();
                var dict = SettlingAccount.ObjectSet()
                                          .Where(c => comIds.Contains(c.CommodityId) && c.EffectiveTime <= orderTime)
                                          .GroupBy(c => c.CommodityId)
                                          .ToDictionary(c => c.Key, c => c.OrderByDescending(m => m.EffectiveTime).First());
                //校验商家结算价是否存在
                foreach (var orderItem in orderItems)
                {
                    if (!dict.ContainsKey(orderItem.CommodityId))
                    {
                        hasException = true;
                        exceptionType = 0;
                        message += "订单商品\"" + orderItem.Name + "\"未设置结算价\r\n";
                        continue;
                    }
                    clearingPrice += Math.Round(dict[orderItem.CommodityId].ManufacturerClearingPrice * orderItem.Number, 2);
                }
                //厂家结算价=设置的厂家结算金额+运费-优惠券-部分退款金额
                //订单总价-分销-商家=金和
                if (!hasException)
                {
                    var orderPayDetails = OrderPayDetail.ObjectSet().Where(c => c.OrderId == commodityOrder.Id && c.ObjectType == 1 && c.UseType == 0).ToList();
                    //运费金额给商家，订单改价除外
                    if (!commodityOrder.IsModifiedPrice)
                        clearingPrice += commodityOrder.Freight;
                    //优惠券减免金额商家承担
                    var couponAmount = 0.0m;
                    var coupons = orderPayDetails.Where(c => c.ObjectType == 1 && c.UseType == 0).Select(c => c.Amount).ToList();

                    if (coupons.Any())
                        couponAmount = coupons.Sum();
                    clearingPrice -= couponAmount;
                    if (clearingPrice < 0)
                        clearingPrice = 0;
                    LogHelper.Debug(string.Format("BuildOrderException:  计算优惠券 订单Id：{0}  ,优惠券减免金额: {1} ,减优惠券之前的结算价为 :{2}", commodityOrder.Id, couponAmount, clearingPrice));


                    //TODO yjz 目前积分未处理分润逻辑，因为目前不会有此种业务出现
                    bool isRefund = false;

                    //售中退款处理(退款不计算分成)

                    var refundStateList = CommodityOrder.GetOrderStateList(-1);
                    if (refundStateList.Any() && refundStateList.Contains(commodityOrder.State))
                    {

                        var orderRefund = OrderRefund.ObjectSet().FirstOrDefault(c => c.OrderId == commodityOrder.Id && !rState.Contains(c.State));
                        if (orderRefund != null)
                        {
                            clearingPrice -= orderRefund.RefundMoney;
                            orderRealPrice -= orderRefund.RefundMoney;
                            if (clearingPrice < 0)
                                clearingPrice = 0;
                            isRefund = true;
                        }
                    }
                    //售后退款(退款不计算分成)
                    else if (commodityOrder.State == 3 && orderService != null && new List<int> { 5, 7, 10, 12 }.Contains(orderService.State))
                    {
                        var orderRefund = OrderRefundAfterSales.ObjectSet().FirstOrDefault(c => c.OrderId == commodityOrder.Id && !rState.Contains(c.State));
                        if (orderRefund != null)
                        {
                            clearingPrice -= orderRefund.RefundMoney;
                            orderRealPrice -= orderRefund.RefundMoney;
                            if (clearingPrice < 0)
                                clearingPrice = 0;
                            isRefund = true;
                        }
                    }
                    LogHelper.Debug(string.Format("BuildOrderException:  计算退款 订单Id：{0}  ,clearingPrice: {1} ,orderRealPrice :{2} ,是否退款：{3}", commodityOrder.Id, clearingPrice, orderRealPrice, isRefund));
                    if (clearingPrice <= orderRealPrice)
                    {

                        //退款不计算分成
                        if (isRefund)
                            return null;

                        //订单参与分成金额
                        var shareRealPrice = commodityOrder.IsModifiedPrice ? commodityOrder.RealPrice : commodityOrder.RealPrice.Value - commodityOrder.Freight;

                        #region 计算分成金额
                        if (isNeedCalcShare)
                        {
                            shareMoney = 0.0m;
                            //众销
                            shareMoney += commodityOrder.Commission;
                            //分销
                            shareMoney += commodityOrder.DistributeMoney;

                            if (commodityOrder.SrcAppId.HasValue && commodityOrder.SrcAppId != Guid.Empty)
                            {
                                decimal ownerShare = ((long)(shareRealPrice * CustomConfig.ShareOwner.OwnerPercent * 100)) / 100.0m;
                                shareMoney += ownerShare;
                            }
                            else if (commodityOrder.SpreaderId.HasValue && commodityOrder.SpreaderId != Guid.Empty)
                            {
                                decimal spreadShare = ((long)(shareRealPrice * CustomConfig.SpreaderAccount.SpreaderPercent * 100)) / 100.0m;
                                shareMoney += spreadShare;
                            }
                            //TODO 众筹已取消，这里就不计算了
                        }
                        #endregion

                        LogHelper.Debug(string.Format("BuildOrderException:  异常类型 2 订单Id：{0}  ,clearingPrice: {1} ,orderRealPrice :{2}", commodityOrder.Id, clearingPrice, orderRealPrice));
                        //比较 分成金额+结算价是否小于等于订单总价，如果不是，结算价异常
                        if (clearingPrice + shareMoney > commodityOrder.RealPrice)
                        {
                            hasException = true;
                            exceptionType = 2;
                            message += string.Format("订单分成金额不足，结算价为:￥{0}，订单总价为：￥{1}，需要分成金额为：￥{2}\r\n", clearingPrice, commodityOrder.RealPrice, shareMoney);
                        }
                    }
                    //订单金额不够给厂家结算
                    else
                    {
                        LogHelper.Debug(string.Format("BuildOrderException:  异常类型 :{3}, 订单Id：{0}  ,clearingPrice: {1} ,orderRealPrice :{2}", commodityOrder.Id, clearingPrice, orderRealPrice, exceptionType));
                        hasException = true;
                        exceptionType = 1;
                        message += string.Format("结算价大于订单金额，结算价为:￥{0}，订单总价为：￥{1}\r\n", clearingPrice, commodityOrder.RealPrice);
                    }
                }

                if (hasException)
                {
                    result = CommodityOrderException.CreateCommodityOrderException();
                    result.OrderId = commodityOrder.Id;
                    result.OrderCode = commodityOrder.Code;
                    result.OrderRealPrice = commodityOrder.RealPrice;
                    result.ClearingPrice = exceptionType == 0 ? null : (decimal?)clearingPrice;
                    result.ExceptionType = exceptionType;
                    result.ExceptionReason = message;
                    result.AppId = commodityOrder.AppId;
                    result.AppName = appName;
                    result.ExceptionTime = exTime;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CommodityOrderSV.buildOrderException异常。commodityOrder：{0}，appName：{1}，exTime：{2}，shareMoney：{3}，clearingPrice：{4}，false：{5}", commodityOrder, appName, exTime, shareMoney, clearingPrice, false), ex);
                result = null;
            }
            return result;

        }

        /// <summary>
        /// 退款退货->生成订单打款记录
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orRefundMoney"></param>
        /// <param name="contextSession"></param>
        /// <param name="applicationDTO"></param>
        /// <param name="orderService"></param>
        /// <returns></returns>
        public static List<OrderPayee> BuildCancelOrderPayeesForOrderItem(CommodityOrder order, decimal orRefundMoney, ContextSession contextSession, AppIdOwnerIdTypeDTO applicationDTO, OrderItem orderItem, CommodityOrderService orderService = null)
        {
            //DateTime now = DateTime.Now;
            List<OrderPayee> result = new List<OrderPayee>();
            //decimal tradeMoney = 0;
            //decimal sellerPayeeMoney = decimal.Zero; //部分订单收益金额
            //decimal jinherPayeeMoney = decimal.Zero; //电商分润金额
            //decimal shareAmount = decimal.Zero; //分成总金额

            //退款给买家
            result.Add(OrderPayee.CreateOrderPayee(order.Id, order.UserId, false, false, orRefundMoney, 8, "退款给买家"));

            ////处理众筹相关业务
            //if (order.IsCrowdfunding != 0)
            //{
            //    tradeMoney = CancelCrowdfunding(contextSession, order, orRefundMoney, now);
            //    shareAmount += tradeMoney;
            //}

            ////商贸众筹
            //if (tradeMoney > 0)
            //{
            //    result.Add(OrderPayee.CreateOrderPayee(order.Id, CustomConfig.CrowdfundingAccount.BTPCrowdfundingAcount, true, CustomConfig.CrowdfundingAccount.IsBtpVoucherBuyGold, tradeMoney, 4, "商贸众筹"));
            //}

            //sellerPayeeMoney = orderItem.RealPrice.Value - orRefundMoney - shareAmount;

            //#region 分润
            //if (order.PaymentTime >= CustomConfig.MinClearningOrderDate && sellerPayeeMoney > 0 && ZPHSV.Instance.CheckIsAppInZPH(order.AppId))
            //{
            //    decimal clearingPrice;
            //    var orderEx = BuildOrderException(order, APPSV.GetAppName(order.AppId), now, shareAmount, out clearingPrice, orderService);
            //    if (orderEx != null)
            //    {
            //        contextSession.SaveObject(orderEx);

            //        switch (orderEx.ExceptionType)
            //        {
            //            case 0:
            //                jinherPayeeMoney = sellerPayeeMoney;
            //                sellerPayeeMoney = 0;
            //                break;
            //            case 1:
            //            case 2:
            //                break;
            //        }
            //    }
            //    else
            //    {
            //        var jinherAmountMoney = sellerPayeeMoney - clearingPrice;
            //        if (jinherAmountMoney > 0)
            //        {
            //            jinherPayeeMoney = jinherAmountMoney;
            //            sellerPayeeMoney = clearingPrice;
            //        }
            //    }
            //}
            //#endregion

            //if (sellerPayeeMoney > 0)
            //{
            //    result.Add(OrderPayee.CreateOrderPayee(order.Id, applicationDTO.OwnerId, false, false, sellerPayeeMoney, 1, "部分订单收益"));

            //}
            //if (jinherPayeeMoney > 0)
            //{
            //    result.Add(OrderPayee.CreateOrderPayee(order.Id, CustomConfig.JinherBtpAccount.BtpGoldAccount, true, CustomConfig.JinherBtpAccount.IsVoucherBuyGold, jinherPayeeMoney, 7, "电商分润"));
            //}
            return result;
        }

        /// <summary>
        /// 退款退货->生成订单打款记录
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orRefundMoney"></param>
        /// <param name="contextSession"></param>
        /// <param name="applicationDTO"></param>
        /// <param name="orderService"></param>
        /// <returns></returns>
        public static List<OrderPayee> BuildCancelOrderPayees(CommodityOrder order, decimal orRefundMoney, ContextSession contextSession, AppIdOwnerIdTypeDTO applicationDTO, CommodityOrderService orderService = null)
        {
            DateTime now = DateTime.Now;
            List<OrderPayee> result = new List<OrderPayee>();
            decimal tradeMoney = 0;
            decimal sellerPayeeMoney = decimal.Zero; //部分订单收益金额
            decimal jinherPayeeMoney = decimal.Zero; //电商分润金额
            decimal shareAmount = decimal.Zero; //分成总金额

            //退款给买家
            result.Add(OrderPayee.CreateOrderPayee(order.Id, order.UserId, false, false, orRefundMoney, 8, "退款给买家"));

            //处理众筹相关业务
            if (order.IsCrowdfunding != 0)
            {
                tradeMoney = CancelCrowdfunding(contextSession, order, orRefundMoney, now);
                shareAmount += tradeMoney;
            }

            //商贸众筹
            if (tradeMoney > 0)
            {
                result.Add(OrderPayee.CreateOrderPayee(order.Id, CustomConfig.CrowdfundingAccount.BTPCrowdfundingAcount, true, CustomConfig.CrowdfundingAccount.IsBtpVoucherBuyGold, tradeMoney, 4, "商贸众筹"));
            }

            sellerPayeeMoney = order.RealPrice.Value - orRefundMoney - shareAmount;

            #region 分润
            if (order.PaymentTime >= CustomConfig.MinClearningOrderDate && sellerPayeeMoney > 0 && ZPHSV.Instance.CheckIsAppInZPH(order.AppId))
            {
                decimal clearingPrice;
                var orderEx = BuildOrderException(order, APPSV.GetAppName(order.AppId), now, shareAmount, out clearingPrice, orderService);
                if (orderEx != null)
                {
                    contextSession.SaveObject(orderEx);

                    switch (orderEx.ExceptionType)
                    {
                        case 0:
                            jinherPayeeMoney = sellerPayeeMoney;
                            sellerPayeeMoney = 0;
                            break;
                        case 1:
                        case 2:
                            break;
                    }
                }
                else
                {
                    var jinherAmountMoney = sellerPayeeMoney - clearingPrice;
                    if (jinherAmountMoney > 0)
                    {
                        jinherPayeeMoney = jinherAmountMoney;
                        sellerPayeeMoney = clearingPrice;
                    }
                }
            }
            #endregion

            if (sellerPayeeMoney > 0)
            {
                result.Add(OrderPayee.CreateOrderPayee(order.Id, applicationDTO.OwnerId, false, false, sellerPayeeMoney, 1, "部分订单收益"));

            }
            if (jinherPayeeMoney > 0)
            {
                result.Add(OrderPayee.CreateOrderPayee(order.Id, CustomConfig.JinherBtpAccount.BtpGoldAccount, true, CustomConfig.JinherBtpAccount.IsVoucherBuyGold, jinherPayeeMoney, 7, "电商分润"));
            }
            return result;
        }
        /// <summary>
        /// 退款的时候 处理众筹相关的业务
        /// </summary>
        /// <param name="contextSession"></param>
        /// <param name="commodityOrder"></param>
        /// <param name="orRefundMoney"></param>
        /// <param name="now"></param>
        public static decimal CancelCrowdfunding(ContextSession contextSession, CommodityOrder commodityOrder, decimal orRefundMoney, DateTime now)
        {
            decimal money = 0;
            //众筹股东表
            var userCrowdfundingQuery = UserCrowdfunding.ObjectSet().FirstOrDefault(q => q.UserId == commodityOrder.UserId && q.AppId == commodityOrder.AppId);
            //众筹成功
            if (commodityOrder.IsCrowdfunding == 2)
            {
                //不是众筹
                if (userCrowdfundingQuery == null)
                    return money;
                //众筹成功，且当前订单
                userCrowdfundingQuery.OrdersMoney -= orRefundMoney;
                //全额退款，减去订单
                if (commodityOrder.IsModifiedPrice)
                {
                    //如果退款金额等于订单金额 相当于全额退款 订单数量减1
                    if (orRefundMoney == commodityOrder.RealPrice)
                    {
                        userCrowdfundingQuery.OrderCount = userCrowdfundingQuery.OrderCount - 1;
                    }
                }
                else
                {
                    //如果退款的金额大于 不计算运费的金额的时候 按不算运费的金额计算 相当于全额退款 订单数量减1
                    if (orRefundMoney >= commodityOrder.RealPrice - commodityOrder.Freight)
                    {

                        userCrowdfundingQuery.OrderCount = userCrowdfundingQuery.OrderCount - 1;
                    }
                }
                userCrowdfundingQuery.EntityState = EntityState.Modified;
                contextSession.SaveObject(userCrowdfundingQuery);
                return money;
            }

            //众筹基本表
            var crowdfundingQuery = Crowdfunding.ObjectSet().FirstOrDefault(q => q.AppId == commodityOrder.AppId && q.StartTime < now);
            if (crowdfundingQuery != null)
            {
                //众筹计数表 
                var crowdfundingCountQuery = CrowdfundingCount.ObjectSet().FirstOrDefault(q => q.CrowdfundingId == crowdfundingQuery.Id);
                //众筹股东表

                decimal corRealPrice;
                decimal fenRealPrice = 0;


                decimal realPrice = commodityOrder.IsModifiedPrice
                                        ? commodityOrder.RealPrice.Value
                                        : commodityOrder.RealPrice.Value - commodityOrder.Freight;

                if (commodityOrder.IsModifiedPrice)
                {
                    corRealPrice = orRefundMoney;
                    //如果退款金额等于订单金额 相当于全额退款 订单数量减1
                    if (orRefundMoney == commodityOrder.RealPrice)
                    {
                        if (userCrowdfundingQuery != null)
                            userCrowdfundingQuery.OrderCount = userCrowdfundingQuery.OrderCount - 1;
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
                        userCrowdfundingQuery.OrderCount = userCrowdfundingQuery.OrderCount - 1;
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
                    var cfMoneyless = -notCfMoney;
                    decimal afterMoney = userCrowdfundingQuery.Money - cfMoneyless;
                    long afterShareCnt = (long)(afterMoney / crowdfundingQuery.PerShareMoney);
                    //用户减少的股数
                    long result = userCrowdfundingQuery.CurrentShareCount - afterShareCnt;
                    if (result > 0)
                    {
                        // 如果众筹已经成功 变成进行中
                        if (crowdfundingQuery.State == 1)
                        {
                            crowdfundingQuery.State = 0;
                            crowdfundingQuery.EntityState = EntityState.Modified;
                            contextSession.SaveObject(crowdfundingQuery);
                        }
                        crowdfundingCountQuery.CurrentShareCount = crowdfundingCountQuery.CurrentShareCount - result;
                        crowdfundingCountQuery.EntityState = EntityState.Modified;
                        contextSession.SaveObject(crowdfundingCountQuery);
                    }
                    else
                    {
                        result = 0;
                    }

                    userCrowdfundingQuery.Money = afterMoney;
                    userCrowdfundingQuery.CurrentShareCount -= result;
                    userCrowdfundingQuery.EntityState = EntityState.Modified;



                    if (commodityOrder.CrowdfundingPrice - orRefundMoney < 0)
                    {
                        commodityOrder.CrowdfundingPrice = 0;
                    }
                    else
                    {
                        commodityOrder.CrowdfundingPrice = commodityOrder.CrowdfundingPrice - orRefundMoney;
                    }
                }
                if (userCrowdfundingQuery != null)
                {
                    userCrowdfundingQuery.OrdersMoney -= corRealPrice;
                    contextSession.SaveObject(userCrowdfundingQuery);
                }


                if (fenRealPrice > 0)
                {
                    //众筹股东每日统计
                    var yestorday = DateTime.Today.AddDays(-1);
                    var userCrowdfundingDailyQuery = UserCrowdfundingDaily.ObjectSet().FirstOrDefault(q => q.AppId == commodityOrder.AppId && q.SettlementDate == yestorday);

                    if (userCrowdfundingDailyQuery != null)
                    {

                        var gold = (userCrowdfundingDailyQuery.ShareCount * crowdfundingQuery.DividendPercent * fenRealPrice).ToGold();
                        money = gold.ToMoney();
                        if (money > 0)
                        {
                            CfOrderDividend cfOrderDividend = CfOrderDividend.CreateCfOrderDividend();
                            cfOrderDividend.Gold = gold;
                            cfOrderDividend.AppId = commodityOrder.AppId;
                            cfOrderDividend.State = 0;
                            cfOrderDividend.CurrentShareCount = userCrowdfundingDailyQuery.ShareCount;
                            cfOrderDividend.CommodityOrderId = commodityOrder.Id;
                            contextSession.SaveObject(cfOrderDividend);
                        }
                    }
                }
            }
            return money;
        }

        /// <summary>
        /// 餐饮订单商家取消订单全额退款支付处理参数
        /// </summary>
        /// <param name="order">订单ID</param>
        /// <param name="orRefundMoney">退款金额</param>
        /// <param name="contextSession">上下文</param>
        /// <param name="applicationDTO">应用DTO</param>
        /// <param name="isafterSales">是否售后</param>
        /// <param name="orderService"></param>
        /// <returns>CancelPayDTO</returns>
        private static CancelPayDTO BuildCYCancelPayDTOInternal(CommodityOrder order, decimal orRefundMoney,
                                                     ContextSession contextSession,
                                                      AppIdOwnerIdTypeDTO applicationDTO, bool isafterSales, CommodityOrderService orderService = null)
        {
            string refundNotifyUrl = CustomConfig.BtpDomain + "PaymentNotify/CancelPayRefund";

            CancelPayDTO cancelPayDTO = BuildCancelPayDTOInternal(order, orRefundMoney, contextSession, applicationDTO, isafterSales);
            cancelPayDTO.RefundNotifyUrl = refundNotifyUrl;

            return cancelPayDTO;
        }

        /// <summary>
        /// 餐饮订单，整单、部分退款支付处理参数
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orRefundMoney"></param>
        /// <param name="contextSession"></param>
        /// <param name="applicationDTO"></param>
        /// <param name="isafterSales"></param>
        /// <param name="orderService"></param>
        /// <returns></returns>
        private static CancelPayDTO BuildCYOrderPayDTOInternal(CommodityOrder order, decimal orRefundMoney, ContextSession contextSession,
            AppIdOwnerIdTypeDTO applicationDTO, bool isafterSales, CommodityOrderService orderService = null)
        {
            string refundNotifyUrl = CustomConfig.BtpDomain + "PaymentNotify/CancelPayOrderRefund";

            CancelPayDTO cancelPayDTO = BuildCancelPayDTOInternal(order, orRefundMoney, contextSession, applicationDTO, isafterSales);
            cancelPayDTO.RefundNotifyUrl = refundNotifyUrl;

            return cancelPayDTO;
        }

        /// <summary>
        /// 金币退款支付处理参数
        /// </summary>
        /// <param name="order">订单ID</param>
        /// <param name="orRefundMoney">退款金额</param>
        /// <param name="contextSession">上下文</param>
        /// <param name="applicationDTO">应用DTO</param>
        /// <param name="isafterSales">是否售后</param>
        /// <param name="orderService"></param>
        /// <returns>CancelPayDTO</returns>
        private static CancelPayDTO BuildCancelPayDTOInternal(CommodityOrder order, decimal orRefundMoney,
                                                     ContextSession contextSession,
                                                      AppIdOwnerIdTypeDTO applicationDTO, bool isafterSales, CommodityOrderService orderService = null)
        {
            long total = (long)(order.RealPrice * 1000);
            long refundGold = (long)(orRefundMoney * 1000);
            string refundNotifyUrl = CustomConfig.BtpDomain + (isafterSales ? "PaymentNotify/GoldRefundAfterSales" : "PaymentNotify/GoldRefund");

            var payeeList = BuildCancelOrderPayees(order, orRefundMoney, contextSession, applicationDTO, orderService);
            if (payeeList == null || !payeeList.Any())
                return null;
            var buyer = payeeList.FirstOrDefault(c => c.PayeeType == 8);
            if (buyer == null || buyer.PayMoney <= 0)
                return null;
            CancelPayDTO cancelPayDTO = new CancelPayDTO
            {
                BizId = order.Id,
                RefundGold = buyer.PayMoney.ToGold(),
                Sign = CustomConfig.PaySing,
                RefundNotifyUrl = refundNotifyUrl,
                PayeesList = new List<PayeeInfoDTO>()
            };
            contextSession.SaveObject(buyer);
            foreach (var orderPayee in payeeList.Where(c => c.PayeeType != 8).ToList())
            {
                cancelPayDTO.PayeesList.Add(new PayeeInfoDTO
                {
                    PayeeId = orderPayee.PayeeId,
                    Description = orderPayee.Description,
                    IsJHSelfUseAccount = orderPayee.IsJHSelfUseAccount,
                    PayeeGold = orderPayee.PayMoney.ToGold(),
                    IsVoucherBuyGold = orderPayee.IsVoucherBuyGold
                });
                contextSession.SaveObject(orderPayee);
            }
            return cancelPayDTO;
        }

        /// <summary>
        /// 金币退款支付处理参数
        /// </summary>
        /// <param name="order">订单ID</param>
        /// <param name="orRefundMoney">退款金额</param>
        /// <param name="contextSession">上下文</param>
        /// <param name="applicationDTO">应用DTO</param>
        /// <param name="isafterSales">是否售后</param>
        /// <param name="orderService"></param>
        /// <returns>CancelPayDTO</returns>
        private static CancelPayDTO BuildCancelPayDtoInternalForOrderItem(CommodityOrder order, decimal orRefundMoney,
                                                     ContextSession contextSession,
                                                      AppIdOwnerIdTypeDTO applicationDTO, OrderItem orderItem, bool isafterSales, CommodityOrderService orderService = null)
        {
            long total = (long)(order.RealPrice * 1000);
            long refundGold = (long)(orRefundMoney * 1000);
            string refundNotifyUrl = CustomConfig.BtpDomain + (isafterSales ? "PaymentNotify/GoldRefundAfterSales" : "PaymentNotify/GoldRefund");

            //var payeeList = BuildCancelOrderPayeesForOrderItem(order, orRefundMoney, contextSession, applicationDTO, orderItem, orderService);
            var payeeList = BuildCancelOrderPayeesForOrderItem(order, orRefundMoney, contextSession, applicationDTO, orderItem, orderService);
            if (payeeList == null || !payeeList.Any())
                return null;
            var buyer = payeeList.FirstOrDefault(c => c.PayeeType == 8);
            if (buyer == null || buyer.PayMoney <= 0)
                return null;
            CancelPayDTO cancelPayDTO = new CancelPayDTO
            {
                BizId = order.Id,
                RefundGold = buyer.PayMoney.ToGold(),
                Sign = CustomConfig.PaySing,
                RefundNotifyUrl = refundNotifyUrl,
                PayeesList = new List<PayeeInfoDTO>()
            };
            contextSession.SaveObject(buyer);
            foreach (var orderPayee in payeeList.Where(c => c.PayeeType != 8).ToList())
            {
                cancelPayDTO.PayeesList.Add(new PayeeInfoDTO
                {
                    PayeeId = orderPayee.PayeeId,
                    Description = orderPayee.Description,
                    IsJHSelfUseAccount = orderPayee.IsJHSelfUseAccount,
                    PayeeGold = orderPayee.PayMoney.ToGold(),
                    IsVoucherBuyGold = orderPayee.IsVoucherBuyGold
                });
                contextSession.SaveObject(orderPayee);
            }
            return cancelPayDTO;
        }
        /// <summary>
        /// 生成确认收货model(售中)
        /// </summary>
        /// <param name="contextSession">上下文</param>
        /// <param name="commodityOrder">订单</param>
        /// <param name="saveList">保存列表</param>
        /// <param name="applicationDTO">应用信息</param>
        /// <param name="password">支付密码</param>
        /// <param name="isSaveObject">是否保存对象</param>
        /// <returns></returns>
        public static ConfirmPayDTO BuildConfirmPayDTO(ContextSession contextSession, CommodityOrder commodityOrder, out List<object> saveList, AppIdOwnerIdTypeDTO applicationDTO, string password = null, bool isSaveObject = true)
        {
            return BuildConfirmPayDTOInternal(contextSession, commodityOrder, out saveList, applicationDTO, false, password, isSaveObject);
        }

        /// <summary>
        /// 生成确认收货model(售后)
        /// </summary>
        /// <param name="contextSession">上下文</param>
        /// <param name="commodityOrder">订单</param>
        /// <param name="saveList">保存列表</param>
        /// <param name="applicationDTO">应用信息</param>
        /// <param name="password">支付密码</param>
        /// <param name="isSaveObject">是否保存对象</param>
        /// <returns></returns>
        public static ConfirmPayDTO BuildConfirmPayDTOAfterSales(ContextSession contextSession, CommodityOrder commodityOrder, out List<object> saveList, AppIdOwnerIdTypeDTO applicationDTO, string password = null, bool isSaveObject = true)
        {
            return BuildConfirmPayDTOInternal(contextSession, commodityOrder, out saveList, applicationDTO, true, password, isSaveObject);
        }

        /// <summary>
        /// 餐饮订单商家取消订单全额退款
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orRefundMoney"></param>
        /// <param name="contextSession"></param>
        /// <param name="applicationDTO"></param>
        /// <returns></returns>
        public static CancelPayDTO BuildCYCancelPayDTO(CommodityOrder order, decimal orRefundMoney, ContextSession contextSession, AppIdOwnerIdTypeDTO applicationDTO)
        {
            return BuildCYCancelPayDTOInternal(order, orRefundMoney, contextSession, applicationDTO, false);
        }

        /// <summary>
        /// 金币退款支付处理参数(售中)
        /// </summary>
        /// <param name="order">订单ID</param>
        /// <param name="orRefundMoney">退款金额</param>
        /// <param name="contextSession">上下文</param>
        /// <param name="applicationDTO">应用DTO</param>
        /// <returns>CancelPayDTO</returns>
        public static CancelPayDTO BuildCancelPayDTO(CommodityOrder order, decimal orRefundMoney, ContextSession contextSession, AppIdOwnerIdTypeDTO applicationDTO)
        {
            return BuildCancelPayDTOInternal(order, orRefundMoney, contextSession, applicationDTO, false);
        }

        /// <summary>
        /// 金币退款支付处理参数(售中) 单品退款
        /// </summary>
        /// <param name="order">订单ID</param>
        /// <param name="orRefundMoney">退款金额</param>
        /// <param name="contextSession">上下文</param>
        /// <param name="applicationDTO">应用DTO</param>
        /// <returns>CancelPayDTO</returns>
        public static CancelPayDTO BuildCancelPayDTOForOrderItem(CommodityOrder order, decimal orRefundMoney, ContextSession contextSession, AppIdOwnerIdTypeDTO applicationDTO, OrderItem orderItem)
        {
            return BuildCancelPayDtoInternalForOrderItem(order, orRefundMoney, contextSession, applicationDTO, orderItem, false);
        }

        /// <summary>
        /// 金币退款支付处理参数(售后)
        /// </summary>
        /// <param name="order">订单ID</param>
        /// <param name="orRefundMoney">退款金额</param>
        /// <param name="contextSession">上下文</param>
        /// <param name="applicationDTO">应用DTO</param>
        /// <param name="orderService"></param>
        /// <returns>CancelPayDTO</returns>
        public static CancelPayDTO BuildCancelPayDTOAfterSales(CommodityOrder order, decimal orRefundMoney, ContextSession contextSession, AppIdOwnerIdTypeDTO applicationDTO, CommodityOrderService orderService)
        {
            return BuildCancelPayDTOInternal(order, orRefundMoney, contextSession, applicationDTO, true, orderService);
        }

        /// <summary>
        /// 单app下订单页获取在线支付地址
        /// </summary>
        /// <param name="createOrderToFspDTO"></param>
        /// <param name="contextDTO"></param>
        /// <returns></returns>
        public static string GetCreateOrderFSPUrl(CreateOrderToFspDTO createOrderToFspDTO, ContextDTO contextDTO)
        {

            LogHelper.Debug(string.Format("请求参数1:{0}", JsonHelper.JsonSerializer<CreateOrderToFspDTO>(createOrderToFspDTO)));
            LogHelper.Debug(string.Format("请求参数2:{0}", JsonHelper.JsonSerializer<ContextDTO>(contextDTO)));
            const string payOnlineUrl = "/Pay/PayOnline?";
            const string directPayUrl = "/Pay/PayOnlineByDirectPay?";
            const string wxPayOnlineUrl = "/Pay/WeiXinPayOnline?";
            const string wxDirectPayUrl = "/Pay/WeiXinPayOnlineByDirectPay?";
            string result = string.Empty;

            LogHelper.Info(string.Format("下订单页获取在线支付地址。createOrderToFspDTO：{0}", JsonHelper.JsonSerializer(createOrderToFspDTO)));

            try
            {
                decimal money = createOrderToFspDTO.RealPrice - createOrderToFspDTO.GoldPrice - createOrderToFspDTO.GoldCoupon;
                string bizSysCode = Convert.ToString(createOrderToFspDTO.SrcType, 16);
                if (bizSysCode.Length == 1)
                {
                    bizSysCode = "0x000" + bizSysCode;
                }
                else
                {
                    bizSysCode = "0x00" + bizSysCode;
                }

                //应用主ID
                Guid payeeId;
                if (createOrderToFspDTO.TradeType == 0)
                {
                    payeeId = APPSV.Instance.GetAppOwnerInfo(createOrderToFspDTO.AppId).OwnerId;
                }
                else if (createOrderToFspDTO.TradeType == 1)
                {
                    payeeId = APPSV.Instance.GetAppOwnerInfo(createOrderToFspDTO.EsAppId).OwnerId;
                }
                else
                {
                    payeeId = APPSV.Instance.GetAppOwnerInfo(createOrderToFspDTO.AppId).OwnerId;
                }
                string expireTime = createOrderToFspDTO.ExpireTime.HasValue ? ConvertJsonDateToDateString(createOrderToFspDTO.ExpireTime.Value) : "null";

                int gold = (int)(createOrderToFspDTO.GoldPrice * 1000);
                string callbackUrl = "";
                if (createOrderToFspDTO.DealType == 0)
                {
                    if (createOrderToFspDTO.DiyGroupId.HasValue && createOrderToFspDTO.DiyGroupId.Value != Guid.Empty)
                    {


                        callbackUrl = CustomConfig.BtpDomain + "diygroupdetail/" +
                                      createOrderToFspDTO.EsAppId + "/" +
                                      createOrderToFspDTO.DiyGroupId.Value;
                        if (!string.IsNullOrWhiteSpace(createOrderToFspDTO.ShareId))
                        {
                            callbackUrl += "/" + createOrderToFspDTO.ShareId;
                        }

                    }
                    else if (createOrderToFspDTO.OrderType == 2)
                    {
                        callbackUrl = string.Format("{0}{1}{2}/", CustomConfig.BtpDomain, "cyorderpayback/", createOrderToFspDTO.EsAppId);
                    }
                    else
                    {
                        callbackUrl = CustomConfig.BtpDomain + "Mobile/PaySuccess";
                    }
                    if (createOrderToFspDTO.Scheme == "https")
                    {
                        callbackUrl = callbackUrl.Replace("http:", "https:");
                    }
                    callbackUrl = HttpUtility.UrlEncode(callbackUrl);
                }
                string couponCodes = createOrderToFspDTO.GoldCouponIds;
                if (string.IsNullOrWhiteSpace(createOrderToFspDTO.GoldCouponIds) || createOrderToFspDTO.GoldCouponIds == "null")
                {
                    couponCodes = "";
                }
                //createOrderToFspDTO.IsWeixinPay = true;
                //以下是组织支付跳转串
                StringBuilder payUrlSB = new StringBuilder();

                payUrlSB.Append(createOrderToFspDTO.Scheme == "https" ? CustomConfig.FSPUrl.Replace("http:", "https:") : CustomConfig.FSPUrl);

                if (createOrderToFspDTO.TradeType == 0)
                {
                    if (createOrderToFspDTO.IsWeixinPay == false)
                    {
                        payUrlSB.Append(payOnlineUrl);
                    }
                    else if (createOrderToFspDTO.IsWeixinPay == true)
                    {
                        payUrlSB.Append(wxPayOnlineUrl);
                    }
                }
                else if (createOrderToFspDTO.TradeType == 1)
                {
                    if (createOrderToFspDTO.IsWeixinPay == false)
                    {
                        payUrlSB.Append(directPayUrl);
                    }
                    else if (createOrderToFspDTO.IsWeixinPay)
                    {
                        var setting = FSPSV.Instance.GetTradeSettingInfoFsp(createOrderToFspDTO.EsAppId);
                        if (setting.IsSetWeixinPay)
                        {
                            payUrlSB.Append(wxDirectPayUrl);
                        }
                        else
                        {
                            payUrlSB.Append(wxPayOnlineUrl);
                        }
                    }
                }
                else
                {
                    payUrlSB.Append(payOnlineUrl);
                }
                payUrlSB.Append("usageId=6F77755B-409D-4446-B3D2-3F5050539869&ChangeOrg=00000000-0000-0000-0000-000000000000");
                payUrlSB.Append("&userId=" + contextDTO.LoginUserID);
                payUrlSB.Append("&esappId=" + createOrderToFspDTO.EsAppId);
                payUrlSB.Append("&sessionId=" + contextDTO.SessionID);
                payUrlSB.Append("&outTradeId=" + createOrderToFspDTO.OrderId);
                payUrlSB.Append("&money=" + money.ToMoney().ToString());
                payUrlSB.Append("&subject=" + string.Format("【{0}】{1}", APPSV.GetAppName(createOrderToFspDTO.AppId), createOrderToFspDTO.FirstCommodityName));
                payUrlSB.Append("&appId=" + createOrderToFspDTO.AppId);
                payUrlSB.Append("&bizSysCode=" + bizSysCode);
                payUrlSB.Append("&isMixedPayment=true");
                payUrlSB.Append("&notifyUrl=" + HttpUtility.UrlEncode(CustomConfig.BtpDomain + "PaymentNotify/Goldpay"));
                payUrlSB.Append("&payeeId=" + payeeId);
                payUrlSB.Append("&gold=" + gold);
                payUrlSB.Append("&couponCount=" + createOrderToFspDTO.GoldCoupon);
                payUrlSB.Append("&couponCodes=" + couponCodes);
                payUrlSB.Append("&source=" + createOrderToFspDTO.Source);
                if (createOrderToFspDTO.TradeType == 1)
                {
                    payUrlSB.Append("&wxOpenId=" + createOrderToFspDTO.WxOpenId);
                }
                if (createOrderToFspDTO.JcActivityId != null && createOrderToFspDTO.JcActivityId != Guid.Empty)
                {
                    payUrlSB.Append("&jcActivityId=" + createOrderToFspDTO.JcActivityId);
                }
                //下订单时需要callbackUrl
                if (createOrderToFspDTO.DealType == 0)
                {
                    payUrlSB.Append("&callbackUrl=" + callbackUrl);
                }

                string payUrl = payUrlSB.ToString().Replace("#", "");

                //以下是签名
                SortedDictionary<string, string> paraArray = new SortedDictionary<string, string>();
                paraArray.Add("outTradeId", createOrderToFspDTO.OrderId.ToString().ToUpper());
                paraArray.Add("payeeId", payeeId.ToString().ToUpper());
                paraArray.Add("money", money.ToMoney().ToString());
                paraArray.Add("userId", contextDTO.LoginUserID.ToString().ToUpper());
                paraArray.Add("gold", gold.ToString());
                paraArray.Add("couponCount", createOrderToFspDTO.GoldCoupon.ToString());

                if (createOrderToFspDTO.GoldCouponIds == null || createOrderToFspDTO.GoldCouponIds == "null")
                {
                    createOrderToFspDTO.GoldCouponIds = "";
                }
                paraArray.Add("couponCodes", createOrderToFspDTO.GoldCouponIds.ToString());

                LogHelper.Info(string.Format("进行签名。paraArray：{0}", JsonHelper.JsonSerializer(paraArray)));
                // 获取参数签名
                string signResult = string.Empty;
                try
                {
                    signResult = SignHelper.GetSign(SignHelper.ConvertDictionaryToUrlParam(paraArray),
                                                     CustomConfig.PartnerPrivKey);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(string.Format("签名异常。paraArray：{0}", JsonHelper.JsonSerializer(paraArray)), ex);
                }

                payUrl = payUrl + "&sign=" + signResult + "&expiretime=" + expireTime;

                //string shutUrl = ShortUrlSV.Instance.GenShortUrl(payUrl);

                //if (!string.IsNullOrWhiteSpace(shutUrl) && createOrderToFspDTO.IsWeixinPay == false)
                //{
                //    result = shutUrl;
                //}
                //else
                //{
                //    result = payUrl;
                //}
                //LogHelper.Debug(string.Format("下订单页获取在线支付地址。resultUrl：{0}", result));
                return payUrl;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("下订单页获取在线支付地址GetCreateOrderFSPUrl异常。createOrderToFspDTO：{0}", JsonHelper.JsonSerializer(createOrderToFspDTO)), ex);
            }
            return result;
        }

        /// <summary>
        /// 将DateTime序列化的时间到格式/Date(1304931520336+0800)/
        /// </summary>
        public static string ConvertJsonDateToDateString(DateTime input)
        {
            string result = string.Empty;

            DateTime d1 = new DateTime(1970, 1, 1);
            DateTime d2 = input.ToUniversalTime();
            TimeSpan ts = new TimeSpan(d2.Ticks - d1.Ticks);
            long data = (long)ts.TotalMilliseconds;
            result = "\\/Date(" + data + ")\\/";
            return result;
        }

        /// <summary>
        /// 拼团退款
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static bool RefundOrderDiyGroup(Guid orderId)
        {
            LogHelper.Debug("拼团退款.orderId:" + orderId);
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            CommodityOrder order = CommodityOrder.ObjectSet().FirstOrDefault(c => c.Id == orderId);
            if (order == null)
                return false;
            int oldstate = order.State;
            //如果已退款直接返回成功
            if (oldstate == 7 || oldstate == 12)
                return true;
            if (oldstate != 16)
                return false;

            List<int> secTranPayments = PaySource.GetPaymentByTradeType(0);
            List<Commodity> needRefreshCacheCommoditys = new List<Commodity>();
            List<TodayPromotion> needRefreshCacheTodayPromotions = new List<TodayPromotion>();
            DateTime now = DateTime.Now;
            decimal orRefundMoney = order.RealPrice.Value;

            #region 生成退款记录
            decimal refundScoreMoney = 0.0m;
            var orderPayDetails = OrderPayDetail.ObjectSet().Where(c => c.OrderId == order.Id).Select(c => c.Amount).ToList();
            if (orderPayDetails.Any())
                refundScoreMoney = orderPayDetails.Sum();

            OrderRefund orderRefund = OrderRefund.ObjectSet().Where(p => p.OrderId == orderId).FirstOrDefault();
            if (orderRefund == null)
            {
                orderRefund = new OrderRefund
                {
                    Id = Guid.NewGuid(),
                    RefundReason = "其他",
                    RefundMoney = order.RealPrice.Value,
                    RefundScoreMoney = refundScoreMoney,
                    RefundDesc = "未成团，退款",
                    OrderId = order.Id,
                    RefundType = 0,
                    State = 0,
                    DataType = "0",
                    IsDelayConfirmTimeAfterSales = false,
                    IsFullRefund = true,
                    EntityState = System.Data.EntityState.Added
                };
                contextSession.SaveObject(orderRefund);
            }
            #endregion

            //加库存
            var orderitemlist = OrderItem.ObjectSet().Where(n => n.CommodityOrderId == order.Id);
            UserLimited.ObjectSet().Context.ExecuteStoreCommand("delete from UserLimited where CommodityOrderId='" + order.Id + "'");
            foreach (OrderItem items in orderitemlist)
            {
                Guid comId = items.CommodityId;
                Commodity com = Commodity.ObjectSet().First(n => n.Id == comId);
                com.EntityState = System.Data.EntityState.Modified;
                com.Stock += items.Number;
                contextSession.SaveObject(com);

                needRefreshCacheCommoditys.Add(com);

                if (items.Intensity != 10 || items.DiscountPrice != -1)
                {
                    TodayPromotion to = TodayPromotion.ObjectSet().FirstOrDefault(n => n.CommodityId == comId && n.EndTime > now && n.StartTime < now);
                    if (to != null)
                    {
                        to.SurplusLimitBuyTotal = to.SurplusLimitBuyTotal - items.Number;
                        to.EntityState = System.Data.EntityState.Modified;
                        contextSession.SaveObject(to);
                        needRefreshCacheTodayPromotions.Add(to);

                        PromotionItems pti = PromotionItems.ObjectSet().FirstOrDefault(n => n.PromotionId == to.PromotionId && n.CommodityId == comId);
                        pti.SurplusLimitBuyTotal = pti.SurplusLimitBuyTotal - items.Number;
                        pti.EntityState = System.Data.EntityState.Modified;
                        contextSession.SaveObject(pti);
                    }
                }
            }
            var tradeType = PaySource.GetTradeType(order.Payment);
            if (order.RealPrice == 0)
            {
                order.State = 7;
            }
            //担保交易，直接到账支持退款的情况
            else if (tradeType == TradeTypeEnum.SecTrans && order.GoldPrice > 0 || tradeType == TradeTypeEnum.Direct && CustomConfig.IsSystemDirectRefund)
            {
                Jinher.AMP.App.Deploy.CustomDTO.AppIdOwnerIdTypeDTO applicationDTO = APPSV.Instance.GetAppOwnerInfo(order.AppId);
                if (applicationDTO == null || applicationDTO.OwnerId == Guid.Empty)
                    return false;

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
                var cashmoney = order.RealPrice.Value;
                //var cashmoney = order.RealPrice.Value - order.Freight;
                //orRefundMoney = orRefundMoney - coupon_price - yjbprice;//退款金币去除抵用券金额
                //orRefundMoney = (orderRefund.OrderRefundMoneyAndCoupun ?? 0) > cashmoney ? cashmoney : (orderRefund.OrderRefundMoneyAndCoupun ?? 0);
                if (orderRefund.OrderRefundMoneyAndCoupun == null)
                {//老的退款数据
                    orRefundMoney = orderRefund.RefundMoney > cashmoney ? cashmoney : orderRefund.RefundMoney;
                }
                else
                {
                    orRefundMoney = (orderRefund.OrderRefundMoneyAndCoupun ?? 0) > cashmoney ? cashmoney : (orderRefund.OrderRefundMoneyAndCoupun ?? 0);
                }
                if (orRefundMoney > 0)
                {
                    var cancelPayDto = OrderSV.BuildCancelPayDTO(order, orRefundMoney, contextSession, applicationDTO);
                    //退款支付
                    var cancelPayresult = FSPSV.CancelPay(cancelPayDto, tradeType);
                    if (cancelPayresult == null || cancelPayresult.Code != 0)
                    {
                        return false;
                    }
                    if (order.Payment == 0)
                    {
                        order.State = 7;
                    }
                    else if (PaySource.GetSecTransWithoutGoldPayment().Contains(order.Payment))
                    {
                        order.State = 12;
                    }
                    if (tradeType == TradeTypeEnum.Direct)
                    {
                        if (cancelPayresult.Code == 0 && cancelPayresult.Message == "success")
                        {
                            order.State = 12;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (cancelPayresult.Code == 0 && cancelPayresult.Message == "success")
                        {
                            order.State = 7;
                        }
                    }
                }
            }
            else
            {
                //订单金额等于0或者直接到账不启用系统退款情况下
                order.State = 7;
            }

            // 回退积分
            SignSV.CommodityOrderRefundScore(contextSession, order, orderRefund);

            // 回退易捷币
            //Jinher.AMP.BTP.TPS.Helper.YJBHelper.OrderRefund(contextSession, order, orderRefund);

            #region 回退易捷币和易捷抵用券
            decimal couponprice = 0;
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
                var refundmoney = orderRefund.OrderRefundMoneyAndCoupun ?? 0;
                for (int i = 0; i < useryjcoupon.Data.Count; i++)
                {
                    if (useryjcoupon.Data[i] != null)
                    {
                        if (isexistsplit)
                            couponprice = useryjcoupon.Data[i].UsePrice;
                        else
                            couponprice = useryjcoupon.Data[i].Price;
                        //totalcouprice += useryjcoupon.Data[i].Price;
                        decimal coupon = 0;
                        if (i == 0)
                        {
                            //pretotalcouprice = couponprice;
                        }
                        else
                        {//易捷币不能循环退
                            orderRefund.RefundYJBMoney = 0;
                        }
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
                        }
                        Jinher.AMP.BTP.TPS.Helper.YJBHelper.OrderRefund(contextSession, order, orderRefund, coupon, useryjcoupon.Data[i].Id);
                        refundmoney -= coupon;
                        //pretotalcouprice = totalcouprice;
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
                journal.Name = "拼团未成团，退款";
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
                LogHelper.Error("系统处理48小时内商家未响应，自动达成同意退款申请协议订单记日志异常。", ex);
            }

            #region 异步发送消息
            if (order.State == 7)
            {

                System.Threading.ThreadPool.QueueUserWorkItem(
                    a =>
                    {

                        AddMessage addmassage = new AddMessage();
                        string type = "commodityOrder";
                        string tipayment = PaySource.GetPaymentName(order.Payment);
                        var messages = string.Format("您的订单{0}已完成退款，退款金额{1}元，请到{2}账号中确认！", order.Code, orRefundMoney, tipayment);
                        Guid esAppId = order.EsAppId.HasValue ? order.EsAppId.Value : order.AppId;
                        addmassage.AddMessages(order.Id.ToString(), order.UserId.ToString(), esAppId, order.Code, order.State, messages, type);
                    });
            }
            #endregion
            return true;
        }
        /// <summary>
        /// 锁订单，解决订单处理的并发问题，已加锁订单不允许操作
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static bool LockOrder(Guid orderId)
        {
            return RedisHelper.AddHash(RedisKeyConst.OrderLock, orderId.ToString(), "1", false);
        }
        /// <summary>
        /// 订单解锁，与锁订单成对使用
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static bool UnLockOrder(Guid orderId)
        {
            RedisHelper.RemoveHash(RedisKeyConst.OrderLock, orderId.ToString());
            return true;
        }

        #region 订单状态是否能更新

        /// <summary>
        /// 订单已存时，判断订单状态是否可以变为目标状态
        /// </summary>
        /// <param name="isStateAfterSales">是否是售后。false：售中；true：售后</param>
        /// <param name="newState">订单目标状态</param>
        /// <param name="oldState">售中订单状态：未支付=0，未发货=1，已发货=2，确认收货=3，商家取消订单=4，客户取消订单=5，超时交易关闭=6，已退款=7，待发货退款中=8，已发货退款中=9,已发货退款中商家同意退款，商家未收到货=10,付款中=11,金和处理退款中=12,出库中=13，出库中退款中=14，待审核=16，待付款待审核=17
        ///  售后订单状态：确认收货=3，售后退款中=5，已退款=7，商家未收到货=10，金和处理退款中=12，买家发货超时，商家未收到货=13，售后交易成功=15
        /// </param>
        /// <param name="selfTakeFlag">自提标志，false：非自提，true：自提</param>
        /// <param name="payment">付款方式:金币=0，货到付款=1，...</param>
        /// <param name="isRefund">是否退款：否=0，是=1</param>
        /// <param name="refundState">状态：退款中=0，已退款=1，已拒绝申请=2，卖家已拒绝=4,已撤销=3，售后退款中商家同意退款，商家未收到货=10 , 买家已发货=11,金和处理退款中=12,买家发货超时，商家未收到货=13</param>
        /// <param name="refundType">退款类型：仅退款=0，退货退款=1</param>
        /// <param name="agreeFlag">0:出库中达成协议，1：已发货达成协议</param>
        /// <returns></returns>
        public static bool CanChangeState(bool isStateAfterSales, int newState, int oldState, bool selfTakeFlag, int payment, bool isRefund, int refundState, int refundType, int? agreeFlag)
        {
            bool result = false;
            List<int> stateList = new List<int>() { };

            if (!isStateAfterSales)
            {
                switch (newState)
                {
                    case 0: //未支付
                        stateList = new List<int>() { 11 };
                        if (stateList.Contains(oldState))
                        {
                            result = true;
                        }
                        break;
                    case 1: //未发货
                        if (isRefund)
                        {
                            stateList = new List<int>() { 8 };
                            if (stateList.Contains(oldState))
                            {
                                result = true;
                            }
                        }
                        else
                        {
                            stateList = new List<int>() { 0, 11, 16 };
                            if (stateList.Contains(oldState))
                            {
                                result = true;
                            }
                        }
                        break;
                    case 2: //已发货
                        if (isRefund)
                        {
                            //注：出库中退款时，商家可以在后台直接改成已发货
                            if (refundType == 0) //仅退款
                            {
                                stateList = new List<int>() { 8, 9, 14 };
                            }
                            else if (refundType == 1)//退货退款
                            {
                                stateList = new List<int>() { 8, 9, 14 };
                                //10的特殊处理，已发货退款中，商家同意后
                                if (oldState == 10 && agreeFlag.HasValue && agreeFlag.Value == 1)
                                {
                                    stateList.Add(10);
                                }
                            }
                            if (stateList.Contains(oldState))
                            {
                                result = true;
                            }
                        }
                        else
                        {
                            stateList = new List<int>() { 1, 13 };
                            if (stateList.Contains(oldState))
                            {
                                result = true;
                            }
                        }
                        break;
                    case 3: //确认收货
                        if (selfTakeFlag) //自提
                        {
                            stateList = new List<int>() { 1, 2, 13 };
                        }
                        else
                        {
                            stateList = new List<int>() { 2, 13 };
                        }
                        if (stateList.Contains(oldState))
                        {
                            result = true;
                        }
                        break;
                    case 4: //商家取消订单
                        if (payment == 1) //货到付款
                        {
                            stateList = new List<int>() { 0, 1, 2 };
                        }
                        else
                        {
                            stateList = new List<int>() { 0 };
                        }

                        if (stateList.Contains(oldState))
                        {
                            result = true;
                        }
                        break;
                    case 5: //客户取消订单
                        if (payment == 1) //货到付款
                        {
                            stateList = new List<int>() { 0, 1 };
                        }
                        else
                        {
                            stateList = new List<int>() { 0 };
                        }

                        if (stateList.Contains(oldState))
                        {
                            result = true;
                        }
                        break;
                    case 6: //超时交易关闭
                        stateList = new List<int>() { 0 };
                        if (stateList.Contains(oldState))
                        {
                            result = true;
                        }
                        break;
                    case 7: //已退款
                        if (isRefund)
                        {
                            if (refundType == 0) //仅退款
                            {
                                stateList = new List<int>() { 8, 9, 12, 14 };
                            }
                            else if (refundType == 1)//退货退款
                            {
                                //10的特殊处理，只有买家已发货时才可以改变
                                if (oldState == 10 && refundState == 11)
                                {
                                    stateList = new List<int>() { 10, 12 };
                                }
                            }
                            if (stateList.Contains(oldState))
                            {
                                result = true;
                            }
                        }
                        break;
                    case 8: //待发货退款中
                        stateList = new List<int>() { 1 };
                        if (stateList.Contains(oldState))
                        {
                            result = true;
                        }
                        break;
                    case 9: //已发货退款中
                        stateList = new List<int>() { 2 };
                        if (stateList.Contains(oldState))
                        {
                            result = true;
                        }
                        break;
                    case 10: //已发货退款中商家同意退款，商家未收到货
                        if (isRefund)
                        {
                            if (refundType == 1) //退货退款
                            {
                                stateList = new List<int>() { 9, 14 };
                                if (stateList.Contains(oldState))
                                {
                                    result = true;
                                }
                            }
                        }
                        break;
                    case 11: //付款中
                        stateList = new List<int>() { 0 };
                        if (stateList.Contains(oldState))
                        {
                            result = true;
                        }
                        break;
                    case 12: //金和处理退款中
                        if (isRefund)
                        {
                            if (refundType == 0) //仅退款
                            {
                                stateList = new List<int>() { 8, 9, 14 };
                            }
                            else if (refundType == 1) //退货退款
                            {
                                stateList = new List<int>() { 10, 14 };
                            }
                        }
                        break;
                    case 13: //出库中
                        if (isRefund)
                        {
                            //有退款时
                            if (refundType == 0) //仅退款
                            {
                                stateList = new List<int>() { 14 };
                            }
                            else if (refundType == 1)//退货退款
                            {
                                stateList = new List<int>() { 14 };
                                //10的特殊处理，出库中退款中，商家同意后
                                if (oldState == 10 && agreeFlag.HasValue && agreeFlag.Value == 0)
                                {
                                    stateList.Add(10);
                                }
                            }
                            if (stateList.Contains(oldState))
                            {
                                result = true;
                            }
                        }
                        else
                        {
                            stateList = new List<int>() { 1 };
                            if (stateList.Contains(oldState))
                            {
                                result = true;
                            }
                        }
                        break;
                    case 14: //出库中退款中
                        stateList = new List<int>() { 13 };
                        if (stateList.Contains(oldState))
                        {
                            result = true;
                        }
                        break;
                    case 16: //待审核
                        stateList = new List<int>() { 17 };
                        if (stateList.Contains(oldState))
                        {
                            result = true;
                        }
                        break;
                    case 17: //待付款待审核，只有下订单时才能是这种状态
                        result = false;
                        break;
                    case 20: //商家退款
                        stateList = new List<int>() { 18, 19 };
                        if (stateList.Contains(oldState))
                        {
                            result = true;
                        }
                        break;
                    default:
                        result = false;
                        break;
                }
            }
            else
            {
                switch (newState)
                {
                    case 3: //确认收货
                        if (refundType == 0) //仅退款
                        {
                            stateList = new List<int>() { 5 };
                        }
                        else if (refundType == 1) //退货退款
                        {
                            stateList = new List<int>() { 5, 10 };
                        }
                        if (stateList.Contains(oldState))
                        {
                            result = true;
                        }
                        break;
                    case 5: //售后退款中
                        stateList = new List<int>() { 3 };
                        if (stateList.Contains(oldState))
                        {
                            result = true;
                        }
                        break;
                    case 7: //已退款
                        if (refundType == 0) //仅退款
                        {
                            stateList = new List<int>() { 5, 12 };
                        }
                        else if (refundType == 1) //退货退款
                        {
                            stateList = new List<int>() { 10, 12 };
                        }
                        if (stateList.Contains(oldState))
                        {
                            result = true;
                        }
                        break;
                    case 10: //商家未收到货
                        if (refundType == 1)
                        {
                            stateList = new List<int>() { 5 };
                        }
                        if (stateList.Contains(oldState))
                        {
                            result = true;
                        }
                        break;
                    case 12: //金和处理退款中
                        if (refundType == 0) //仅退款
                        {
                            stateList = new List<int>() { 5 };
                        }
                        else if (refundType == 1) //退货退款
                        {
                            stateList = new List<int>() { 10 };
                        }
                        if (stateList.Contains(oldState))
                        {
                            result = true;
                        }
                        break;
                    case 13: //买家发货超时，商家未收到货 现已无用
                        result = false;
                        break; ;
                    case 15: //售后交易成功
                        stateList = new List<int>() { 3 };
                        if (stateList.Contains(oldState))
                        {
                            result = true;
                        }
                        break;
                    default:
                        result = false;
                        break;
                }
            }
            return result;
        }

        public static bool CanChangeState(int newState, CommodityOrder commodityOrder, OrderRefund orderRefund, CommodityOrderService commodityOrderService, OrderRefundAfterSales orderRefundAfterSales)
        {
            try
            {
                bool isStateAfterSales = false;
                int oldState = -1;
                bool selfTakeFlag = commodityOrder.SelfTakeFlag == 1 ? true : false;
                int payment = commodityOrder.Payment;
                bool isRefund = false;
                int refundState = -1;
                int refundType = -1;
                int? agreeFlag = -1;

                if (commodityOrder.State == 3 && (commodityOrder.Payment != 1 || commodityOrder.OrderType == 0))
                {
                    isStateAfterSales = true;
                }

                if (isStateAfterSales)
                {
                    oldState = commodityOrderService.State;
                    List<int> stateList = new List<int>() { 5, 7, 10, 12 };
                    if (stateList.Contains(oldState))
                    {
                        isRefund = true;
                        refundState = orderRefundAfterSales.State;
                        refundType = orderRefundAfterSales.RefundType;
                    }
                }
                else
                {
                    oldState = commodityOrder.State;
                    List<int> stateList = new List<int>() { 7, 8, 9, 10, 12, 14 };
                    if (stateList.Contains(oldState))
                    {
                        isRefund = true;
                        refundState = orderRefund.State;
                        refundType = orderRefund.RefundType;
                        agreeFlag = orderRefund.AgreeFlag;
                    }
                }
                bool result = CanChangeState(isStateAfterSales, newState, oldState, selfTakeFlag, payment, isRefund, refundState, refundType, agreeFlag);
                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion


        /// <summary>
        /// 担保交易（金币）支付，生成结算单
        /// </summary>
        /// <param name="commodityOrder">订单</param>
        /// <returns></returns>
        public static void CreateSettleAccount(ContextSession contextSession, CommodityOrder commodityOrder)
        {
            // 未加入场馆的不生成结算单
            if (!commodityOrder.EsAppId.HasValue) return;
            if (!ZPHSV.Instance.IsAppPavilion(commodityOrder.EsAppId.Value))
            {
                return;
            }

            var sad = CreateSettleAccountCore(contextSession, commodityOrder);

            // 结算单
            SettleAccounts sa = new SettleAccounts();
            sad.SAId = sa.Id = Guid.NewGuid();
            sa.UserId = Guid.Empty; // 系统生成结算单，UserId为空
            sa.AmountDate = sa.ModifiedOn = sa.SubTime = DateTime.Now;
            sa.AppId = commodityOrder.AppId;
            sa.AppName = APPSV.GetAppName(sa.AppId);
            sa.EsAppId = commodityOrder.EsAppId.Value;
            //sa.EsAppName = APPSV.GetAppName(sa.EsAppId);
            var pInfo = ZPHSV.Instance.GetAppPavilionInfo(new Jinher.AMP.ZPH.Deploy.CustomDTO.QueryAppPavilionParam { id = sa.EsAppId });
            if (string.IsNullOrEmpty(pInfo.pavilionName))
            {
                sa.EsAppName = "未找到";
                LogHelper.Error("OrderSV.CreateSettleAccount 生成结算单异常：未能从 ZPHSV.Instance.GetAppPavilionInfo 获取到场管名称。");
            }
            sa.EsAppName = pInfo.pavilionName;
            //sa.OrderAmount = commodityOrder.RealPrice.Value;
            sa.OrderAmount = commodityOrder.Price;
            sa.CouponAmount = sad.OrderCouponAmount;
            sa.RefundAmount = sad.OrderRefundAmount;
            sa.PromotionCommissionAmount = sad.OrderPromotionCommissionAmount;
            sa.PromotionAmount = sad.PromotionAmount;
            sa.SellerAmount = sad.SellerAmount;
            sa.IsAmount = true;
            sa.SettleStatue = true;
            Random rd = new Random();
            sa.Code = DateTime.Now.ToString("yyyyMMddHHmmss") + rd.Next(9999).ToString("D4");
            if (sad.IsMallCoupon)
            {
                // 当商城佣金小于商城优惠券金额时，则商城不分佣金，表示当前订单结算异常，结算结果为：结算异常；
                if (sad.PromotionAmount < sad.OrderCouponAmount)
                {
                    sa.PromotionAmount = 0;
                    sa.SellerAmount += sa.PromotionAmount;
                    sa.SettleStatue = false;
                }
            }
            else
            {
                // 当商家的结算金额小于0时，则先结算推广佣金，而后结算商城佣金，表示当前订单结算异常，结算结果为：结算异常；
                if (sa.SellerAmount < 0)
                {
                    sa.SettleStatue = false;
                }
            }


            sa.State = new Deploy.SettleAccountsVO() { Value = 3 };
            sa.EntityState = EntityState.Added;
            contextSession.SaveObject(sa);
        }

        /// <summary>
        /// 生成结算单
        /// </summary>
        /// <param name="commodityOrder">订单</param>
        /// <returns></returns>
        public static void CreateSettleAccount(ContextSession contextSession, List<CommodityOrder> commodityOrders, Guid userId, DateTime amountDate, bool isTemp)
        {
            if (commodityOrders.Count == 0)
            {
                return;
            }
            var firestOrder = commodityOrders[0];
            var sads = new List<SettleAccountsDetails>();
            foreach (var commodityOrder in commodityOrders)
            {
                sads.Add(CreateSettleAccountCore(contextSession, commodityOrder));
            }
            // 结算单
            SettleAccounts sa = new SettleAccounts();
            sa.Id = Guid.NewGuid();
            sa.UserId = userId;
            sa.ModifiedOn = sa.SubTime = DateTime.Now;
            sa.AmountDate = amountDate;
            sa.AppId = firestOrder.AppId;
            sa.AppName = APPSV.GetAppName(sa.AppId);
            sa.EsAppId = firestOrder.EsAppId.Value;
            sa.EsAppName = ZPHSV.Instance.GetAppPavilionInfo(new Jinher.AMP.ZPH.Deploy.CustomDTO.QueryAppPavilionParam { id = sa.EsAppId }).pavilionName;
            sa.OrderAmount = sads.Sum(s => s.OrderAmount);
            sa.OrderRealAmount = sads.Sum(s => s.OrderRealAmount);
            sa.CouponAmount = sads.Sum(s => s.OrderCouponAmount);
            sa.RefundAmount = sads.Sum(s => s.OrderRefundAmount);
            sa.PromotionCommissionAmount = sads.Sum(s => s.OrderPromotionCommissionAmount);
            sa.PromotionAmount = sads.Sum(s => s.PromotionAmount);
            sa.SellerAmount = sads.Sum(s => s.SellerAmount);
            sa.IsAmount = false;
            sa.SettleStatue = true;
            Random rd = new Random();
            sa.Code = DateTime.Now.ToString("yyyyMMddHHmmss") + rd.Next(9999).ToString("D4");
            if (isTemp)
            {
                sa.State = new Deploy.SettleAccountsVO() { Value = 0 };
            }
            else
            {
                sa.State = new Deploy.SettleAccountsVO() { Value = 1 };
            }
            sa.EntityState = EntityState.Added;

            foreach (var sad in sads)
            {
                sad.SAId = sa.Id;
            }
            contextSession.SaveObject(sa);
        }

        private static SettleAccountsDetails CreateSettleAccountCore(ContextSession contextSession, CommodityOrder commodityOrder)
        {
            var currentDate = DateTime.Now;
            // 1:商家，2：金和众销（给金和分的钱），3：商贸众销（给分享者分的钱），4：商贸众筹，5：推广主分成，6：应用主分成，7金和分润，8买家,9一级分销,10二级分销,11三级分销,12渠道推广,20：一级代理，21：二级代理
            var payeeType = new List<int> { 3, 5, 9, 10, 11, 20, 21 };
            var orderitemlist = OrderItem.ObjectSet().Where(n => n.CommodityOrderId == commodityOrder.Id).
                GroupJoin(OrderItemShare.ObjectSet().Where(s => payeeType.Contains(s.PayeeType)), o => o.Id, s => s.OrderItemId, (o, s) =>
                    new { o.Id, o.Name, o.Number, o.RealPrice, o.CommodityId, o.ComCategoryId, Commission = s.Sum(_ => (decimal?)_.Commission) }).ToList();
            List<SettleAccountsOrderItem> items = new List<SettleAccountsOrderItem>();
            var payTime = commodityOrder.PaymentTime.Value;
            foreach (var orderItem in orderitemlist)
            {
                // 结算单订单项
                SettleAccountsOrderItem saItem = new SettleAccountsOrderItem();
                saItem.Id = saItem.OrderItemId = orderItem.Id;
                // saItem.UserId = Guid.Empty; // 系统生成结算单，UserId为空
                saItem.ModifiedOn = saItem.SubTime = currentDate;
                saItem.OrderId = commodityOrder.Id;
                saItem.OrderItemRefundAmount = 0; // 暂无单品退货
                saItem.OrderItemPromotionCommissionAmount = orderItem.Commission ?? 0M;
                saItem.OrderItemName = orderItem.Name;
                saItem.OrderItemNumber = orderItem.Number;
                saItem.OrderItemPrice = orderItem.RealPrice.Value;

                // 获取佣金比例：按结算日期获取有效的（启用日期小于等于结算日期的最近一次设置的佣金比例）佣金比例【基础、类目、商品】；
                saItem.BaseCommission = BaseCommission.ObjectSet().Join(MallApply.ObjectSet(),
                    b => b.MallApplyId, m => m.Id, (b, m) => new { b.Commission, b.EffectiveTime, b.SubTime, m.AppId, m.EsAppId }).
                    Where(t => t.EsAppId == commodityOrder.EsAppId && t.AppId == commodityOrder.AppId && t.EffectiveTime < payTime).
                    OrderByDescending(t => t.SubTime).Select(t => (decimal?)t.Commission).FirstOrDefault();

                // 查询当前商品的类目
                List<Guid> categoryIds = new List<Guid>();
                var tempCategories = CommodityCategory.ObjectSet().Where(c => c.CommodityId == orderItem.CommodityId).Join(Category.ObjectSet(), cc => cc.CategoryId, c => c.Id, (cc, c) => new { c.Id, c.CurrentLevel, c.ParentId }).ToList();
                foreach (var item in tempCategories)
                {
                    if (item.CurrentLevel == 3 || item.CurrentLevel == 4 || item.CurrentLevel == 5)
                    {
                        var lv2 = Category.FindByID(item.ParentId.Value);
                        if (item.CurrentLevel == 3)
                        {
                            categoryIds.Add(lv2.ParentId.Value);
                            continue;
                        }
                        var lv3 = Category.FindByID(lv2.ParentId.Value);
                        if (item.CurrentLevel == 4)
                        {
                            categoryIds.Add(lv3.ParentId.Value);
                            continue;
                        }
                        var lv4 = Category.FindByID(lv3.ParentId.Value);
                        categoryIds.Add(lv4.ParentId.Value);
                    }
                    else if (item.CurrentLevel == 2)
                    {
                        categoryIds.Add(item.ParentId.Value);
                    }
                    else if (item.CurrentLevel == 1)
                    {
                        categoryIds.Add(item.Id);
                    }
                }

                var commissions = CategoryCommission.ObjectSet().
                            Join(MallApply.ObjectSet(), b => b.MallApplyId, m => m.Id,
                            (b, m) => new { b.CategoryId, b.Commission, b.EffectiveTime, b.SubTime, m.AppId, m.EsAppId }).
                        Where(t =>
                            t.EsAppId == commodityOrder.EsAppId &&
                            t.AppId == commodityOrder.AppId &&
                            categoryIds.Contains(t.CategoryId) &&
                            t.EffectiveTime < payTime).
                        GroupBy(t => t.CategoryId).
                        Select(group => group.OrderByDescending(g => g.SubTime).Select(g => g.Commission).FirstOrDefault()).
                        ToList();
                if (commissions.Count > 0)
                {
                    saItem.CategoryCommission = commissions.Min();
                }
                saItem.CommodityCommission = CommodityCommission.ObjectSet().Join(MallApply.ObjectSet(),
                    b => b.MallApplyId, m => m.Id, (b, m) => new { b.CommodityId, b.Commission, b.EffectiveTime, b.SubTime, m.AppId, m.EsAppId }).
                    Where(t => t.EsAppId == commodityOrder.EsAppId && t.AppId == commodityOrder.AppId &&
                        t.CommodityId == orderItem.CommodityId && t.EffectiveTime < payTime).
                    OrderByDescending(t => t.SubTime).Select(t => (decimal?)t.Commission).FirstOrDefault();

                // 商城佣金计算公式：商品销售价*佣金比例【商品佣金优先，类目佣金其次、基础佣金最后，三选一计算】* 购买数量；
                saItem.PromotionAmount = Math.Truncate(
                    (saItem.CommodityCommission.HasValue ? saItem.CommodityCommission.Value :
                    (saItem.CategoryCommission.HasValue ? saItem.CategoryCommission.Value :
                    (saItem.BaseCommission.HasValue ? saItem.BaseCommission.Value : 0)
                    )) * saItem.OrderItemPrice * saItem.OrderItemNumber) / 100;
                saItem.EntityState = EntityState.Added;
                items.Add(saItem);
                contextSession.SaveObject(saItem);
            }

            // 结算单订单详情
            SettleAccountsDetails sad = new SettleAccountsDetails();
            sad.Id = sad.OrderId = commodityOrder.Id;
            sad.ModifiedOn = sad.SubTime = currentDate;
            sad.AppId = commodityOrder.AppId;
            sad.OrderCode = commodityOrder.Code;
            sad.OrderSubTime = commodityOrder.SubTime;
            sad.OrderRealAmount = commodityOrder.RealPrice.Value;
            sad.OrderAmount = commodityOrder.Price;

            //// 计算商城优惠券金额
            //var orderPayDetailId = OrderPayDetail.ObjectSet().Where(c => c.OrderId == commodityOrder.Id && c.ObjectType == 1).Select(c => c.ObjectId).FirstOrDefault();
            //if (orderPayDetailId != Guid.Empty)
            //{
            //    var request = new Coupon.Deploy.CustomDTO.ListCouponNewRequestDTO
            //    {
            //        CouponIds = new List<Guid> { orderPayDetailId },
            //        UserId = commodityOrder.UserId
            //    };
            //    var coupons = CouponSV.Instance.GetUserCouponsByIds(request);
            //    if (coupons.IsSuccess)
            //    {
            //        var firstCoupon = coupons.Data[0];

            //        sad.OrderCouponAmount = firstCoupon.Cash;
            //        // 判断是否为电商馆
            //        sad.IsMallCoupon = ZPHSV.Instance.IsAppPavilion(firstCoupon.ShopId);
            //    }
            //    else
            //    {
            //        LogHelper.Error(string.Format("CouponSV.GetUserCouponsByIds返回结果不成功，入参 CouponIds: {0} UserId: {1}，出参 Code: {2} Info: {3}。", orderPayDetailId, commodityOrder.UserId, coupons.Code, coupons.Info));
            //    }
            //}

            if (commodityOrder.State == 7)
            {
                var refund = OrderRefundAfterSales.ObjectSet().Where(o => o.OrderId == commodityOrder.Id).FirstOrDefault();
                sad.OrderRefundAmount = refund.RefundMoney;
            }
            else
            {
                sad.OrderRefundAmount = 0; // 暂无单品退货
            }
            // 优先级：1推广主、2三级分销、 3众销
            sad.OrderPromotionCommissionAmount = commodityOrder.SpreadGold > 0 ? commodityOrder.SpreadGold.ToMoney() :
                commodityOrder.DistributeMoney > 0 ? commodityOrder.DistributeMoney : commodityOrder.Commission;
            //sad.OrderPromotionCommissionAmount = Math.Truncate(sad.OrderPromotionCommissionAmount * 100) / 100;
            sad.PromotionAmount = items.Sum(i => i.PromotionAmount);
            // 商家结算金额 = 订单总额 + 商城优惠券总金额 - 退款总金额 - 推广佣金总额 - 商城佣金总额
            sad.SellerAmount = Math.Truncate((sad.OrderRealAmount + sad.OrderCouponAmount - sad.OrderRefundAmount - sad.OrderPromotionCommissionAmount - sad.PromotionAmount) * 100) / 100;
            sad.EntityState = EntityState.Added;
            contextSession.SaveObject(sad);
            return sad;
        }
    }
}
