using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Jinher.AMP.BTP.BE;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;
using Jinher.AMP.FSP.Deploy.CustomDTO;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.AMP.BTP.Common;

namespace Jinher.AMP.BTP.TPS.Helper
{
    /// <summary>
    /// 结算单帮助类
    /// </summary>
    public static class SettleAccountHelper
    {
        public static MallApply GetMallApply(CommodityOrder commodityOrder)
        {
            // 查询场馆信息
            return MallApply.ObjectSet().Where(m => m.AppId == commodityOrder.AppId && m.EsAppId == commodityOrder.EsAppId && (m.State.Value == 2 || m.State.Value == 4)).FirstOrDefault();
        }

        /// <summary>
        /// 根据订单生成待结算项 
        /// </summary>
        public static SettleAccountsDetails CreateSettleAccountDetails(ContextSession contextSession, CommodityOrder commodityOrder, int? orderServiceState = null)
        {
            // 查询场馆信息
            var mall = GetMallApply(commodityOrder);
            if (mall == null)
            {
                LogHelper.Info("生成结算项失败，商城中未找到该APP，订单ID：" + commodityOrder.Id);
                return null;
            }
            return CreateSettleAccountDetails(contextSession, commodityOrder, mall, orderServiceState);
        }

        /// <summary>
        /// 根据订单生成待结算项 
        /// </summary>
        public static SettleAccountsDetails CreateSettleAccountDetails(ContextSession contextSession, CommodityOrder commodityOrder, MallApply mall, int? orderServiceState = null)
        {
            //LogHelper.Info("开始生成结算项，订单ID：" + commodityOrder.Id);

            var currentDate = DateTime.Now;

            // 1:商家，2：金和众销（给金和分的钱），3：商贸众销（给分享者分的钱），4：商贸众筹，5：推广主分成，6：应用主分成，7金和分润，8买家,9一级分销,10二级分销,11三级分销,12渠道推广,20：一级代理，21：二级代理
            //var payeeType = new List<int> { 3, 5, 9, 10, 11, 20, 21 };
            //var orderitemlist = OrderItem.ObjectSet().Where(n => n.CommodityOrderId == commodityOrder.Id).
            //    GroupJoin(OrderItemShare.ObjectSet().Where(s => payeeType.Contains(s.PayeeType)), o => o.Id, s => s.OrderItemId, (o, s) =>
            //        new { o.Id, o.Name, o.Number, o.RealPrice, o.CommodityId, o.ComCategoryId, Commission = s.Sum(_ => (decimal?)_.Commission) }).ToList();

            // 暂不计算推广佣金
            var orderitemlist = OrderItem.ObjectSet().Where(n => n.CommodityOrderId == commodityOrder.Id).Select(o =>
                    new { o.Id, o.Name, o.Number, o.RealPrice, o.CommodityId, o.ComCategoryId, o.CommodityAttributes }).ToList();
            bool isSetSettleAmount = true;
            List<SettleAccountsOrderItem> items = new List<SettleAccountsOrderItem>();
            var payTime = commodityOrder.PaymentTime.Value;

            foreach (var orderItem in orderitemlist)
            {
                // 结算单订单项
                SettleAccountsOrderItem saItem = new SettleAccountsOrderItem();
                saItem.Id = saItem.OrderItemId = orderItem.Id;
                saItem.ModifiedOn = saItem.SubTime = currentDate;
                saItem.OrderId = commodityOrder.Id;
                saItem.OrderItemRefundAmount = 0; // 暂无单品退货
                saItem.OrderItemPromotionCommissionAmount = 0M; // orderItem.Commission ?? 0M;
                saItem.OrderItemName = orderItem.Name;
                saItem.OrderItemNumber = orderItem.Number;
                saItem.OrderItemPrice = orderItem.RealPrice.Value;
                // 自营他配
                if (mall.Type == 0)
                {
                    var commoditySettleAmount = CommoditySettleAmount.ObjectSet()
                        .Where(c => c.CommodityId == orderItem.CommodityId && c.EffectiveTime < payTime)
                        .OrderByDescending(c => c.SubTime)
                        .ThenByDescending(c => c.EffectiveTime)
                        .FirstOrDefault();
                    if (commoditySettleAmount == null)
                    {
                        // 未设置结算价，暂不结算
                        //LogHelper.Info("开始生成结算项，订单ID：" + commodityOrder.Id + "，自营商家，未设置结算价，商品ID：" + orderItem.CommodityId);
                        return null;
                    }
                    //LogHelper.Info("开始生成结算项，订单ID：" + commodityOrder.Id + "，自营商家，商品ID：" + orderItem.CommodityId + "，商品属性：" + orderItem.CommodityAttributes + "，结算价：" + commoditySettleAmount.CommodityAttrJson);

                    var saAttrs = JsonHelper.JsonDeserialize<List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAttributePrice>>(commoditySettleAmount.CommodityAttrJson);
                    var attrs = (orderItem.CommodityAttributes ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (attrs.Length == 0)
                    {
                        // 无属性
                        saItem.SettleAmount = saAttrs[0].SettlePrice.Value;
                    }
                    else if (attrs.Length == 1)
                    {
                        // 单属性
                        var settlePrice = saAttrs.Where(a => a.AttributeValue == attrs[0]).FirstOrDefault();
                        if (settlePrice == null)
                        {
                            LogHelper.Error("SettleAccountHelper.CreateSettleAccountDetails 生成结算单失败，结算价设置异常，商品ID：" + orderItem.CommodityId + "，商品属性：" + orderItem.CommodityAttributes + "，结算价ID：" + commoditySettleAmount.Id + "，结算价属性：" + commoditySettleAmount.CommodityAttrJson);
                            return null;
                        }
                        saItem.SettleAmount = settlePrice.SettlePrice.Value;
                    }
                    else if (attrs.Length == 2)
                    {
                        // 双属性
                        var settlePrice = saAttrs.Where(a =>
                                (a.AttributeValue == attrs[0] && a.SecAttributeValue == attrs[1]) ||
                                (a.AttributeValue == attrs[1] && a.SecAttributeValue == attrs[0]))
                            .FirstOrDefault();
                        if (settlePrice == null)
                        {
                            LogHelper.Error("SettleAccountHelper.CreateSettleAccountDetails 生成结算单失败，结算价设置异常，商品ID：" + orderItem.CommodityId + "，商品属性：" + orderItem.CommodityAttributes + "，结算价ID：" + commoditySettleAmount.Id + "，结算价属性：" + commoditySettleAmount.CommodityAttrJson);
                            return null;
                        }
                        saItem.SettleAmount = settlePrice.SettlePrice.Value;
                    }
                    else
                    {
                        LogHelper.Error("SettleAccountHelper.CreateSettleAccountDetails 生成结算单失败，商品属性异常，商品ID：" + orderItem.CommodityId + "，商品属性：" + orderItem.CommodityAttributes);
                        return null;
                    }
                    //orderItem.com
                }
                // 第三方
                else if (mall.Type == 1)
                {
                    // 查询商品易捷币抵用数量
                    var yjbInfo = YJBSV.GetOrderItemYJBInfo(commodityOrder.EsAppId.Value, commodityOrder.Id);
                    if (!yjbInfo.IsSuccess)
                    {
                        saItem.OrderItemYJBAmount = 0;
                    }
                    else
                    {
                        var currentCommodityYJBInfo = yjbInfo.Data.Items.Where(c => c.CommodityId == orderItem.CommodityId).FirstOrDefault();
                        if (currentCommodityYJBInfo != null && currentCommodityYJBInfo.IsMallYJB)
                        {
                            saItem.OrderItemYJBAmount = currentCommodityYJBInfo.InsteadCashAmount;
                        }
                    }

                    // 计算实际成交价



                    #region 计算商品佣金比例
                    // 获取佣金比例：按结算日期获取有效的（启用日期小于等于结算日期的最近一次设置的佣金比例）佣金比例【基础、类目、商品】；
                    saItem.BaseCommission = BaseCommission.ObjectSet().Join(MallApply.ObjectSet(),
                        b => b.MallApplyId, m => m.Id, (b, m) => new { b.Commission, b.EffectiveTime, b.SubTime, m.AppId, m.EsAppId }).
                        Where(t => t.EsAppId == commodityOrder.EsAppId && t.AppId == commodityOrder.AppId && t.EffectiveTime < payTime).
                        OrderByDescending(t => t.SubTime).Select(t => (decimal?)t.Commission).FirstOrDefault();

                    // 查询当前商品的类目
                    List<Guid> categoryIds = new List<Guid>();
                    var tempCategories = CommodityCategory.ObjectSet().Where(c => c.AppId == commodityOrder.EsAppId && c.CommodityId == orderItem.CommodityId).Join(Category.ObjectSet(), cc => cc.CategoryId, c => c.Id, (cc, c) => new { c.Id, c.CurrentLevel, c.ParentId, cc.IsDel }).ToList();
                    foreach (var item in tempCategories)
                    {
                        if (item.IsDel.HasValue && item.IsDel.Value)
                        {
                            continue;
                        }
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
                    #endregion

                    // 商城佣金计算公式：商品销售价*佣金比例【商品佣金优先，类目佣金其次、基础佣金最后，三选一计算】* 购买数量；
                    saItem.PromotionAmount = Math.Truncate(
                        (saItem.CommodityCommission.HasValue ? saItem.CommodityCommission.Value :
                        (saItem.CategoryCommission.HasValue ? saItem.CategoryCommission.Value :
                        (saItem.BaseCommission.HasValue ? saItem.BaseCommission.Value : 0)
                    )) * saItem.OrderItemPrice * saItem.OrderItemNumber) / 100;
                }

                saItem.EntityState = EntityState.Added;
                items.Add(saItem);
            }

            // 结算单订单详情
            SettleAccountsDetails sad = new SettleAccountsDetails();
            sad.Id = sad.OrderId = commodityOrder.Id;
            sad.EntityState = EntityState.Added;
            sad.ModifiedOn = sad.SubTime = currentDate;
            sad.IsSettled = false;
            sad.AppId = commodityOrder.AppId;
            sad.EsAppId = commodityOrder.EsAppId.Value;
            sad.OrderCode = commodityOrder.Code;
            sad.OrderSubTime = payTime;
            sad.OrderAmount = commodityOrder.Price;
            sad.OrderRealAmount = commodityOrder.RealPrice.Value;
            sad.OrderFreight = commodityOrder.Freight;
            sad.IsSetSettleAmount = isSetSettleAmount;

            // 导入订单时，记录退款金额
            if (commodityOrder.State == 7 || orderServiceState == 7)
            {
                var refund = OrderRefundAfterSales.ObjectSet().Where(o => o.OrderId == commodityOrder.Id).FirstOrDefault();
                // 全额退款 不计入结算单
                if (refund.IsFullRefund == 1)
                {
                    sad.IsSettled = true;
                    if (contextSession != null)
                    {
                        foreach (var item in items)
                        {
                            contextSession.SaveObject(item);
                        }
                        contextSession.SaveObject(sad);
                    }
                    return sad;
                }
                else
                {
                    sad.OrderRefundAmount = refund.RefundMoney;
                }
            }
            else
            {
                sad.OrderRefundAmount = 0; // 暂无单品退货
            }

            //// 计算商城优惠券金额
            //var orderP ayDetailId = OrderPayDetail.ObjectSet().Where(c => c.OrderId == commodityOrder.Id && c.ObjectType == 1).Select(c => c.ObjectId).FirstOrDefault();
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

            // 计算商家结算金额

            decimal spreadMoney = 0;
            if (commodityOrder.SpreadGold > 0)
            {
                spreadMoney = commodityOrder.SpreadGold.ToMoney();
            }
            sad.OrderSpreadAmount = spreadMoney;

            // 1.第三方
            if (mall.Type == 1)
            {
                sad.OrderYJBAmount = items.Sum(i => i.OrderItemYJBAmount);

                // 商城佣金
                sad.PromotionAmount = items.Sum(i => i.PromotionAmount);

                // 优先级：1推广主、2三级分销、 3众销
                //sad.OrderPromotionCommissionAmount = commodityOrder.SpreadGold > 0 ? commodityOrder.SpreadGold.ToMoney() :
                //    commodityOrder.DistributeMoney > 0 ? commodityOrder.DistributeMoney : commodityOrder.Commission;
                // 老版本，已弃 商家结算金额 = 订单总额 + 商城优惠券总金额 - 退款总金额 - 推广佣金总额 - 商城佣金总额
                // 版本1：商家结算金额 =  实收款 +  商城易捷币抵用金额 - 退款金额  - 商城佣金
                // 版本2：商家结算金额 =  实收款 +  商城易捷币抵用金额 - 退款金额  - 商城佣金 - 推广佣金总额
                // 商家结算金额
                sad.SellerAmount = Math.Truncate((sad.OrderRealAmount + sad.OrderYJBAmount - sad.OrderRefundAmount - sad.PromotionAmount - sad.OrderSpreadAmount.Value) * 100) / 100;
            }
            // 0.自营他配
            else if (mall.Type == 0)
            {
                // 商品结算价
                decimal totalsSettlePrice = 0;
                foreach (var item in items)
                {
                    totalsSettlePrice += item.SettleAmount * item.OrderItemNumber;
                }
                sad.SettleAmount = totalsSettlePrice;
                // 商家结算金额 = 商品结算价*数量 + 运费 - 退款金额
                sad.SellerAmount = Math.Truncate((sad.SettleAmount + sad.OrderFreight - sad.OrderRefundAmount) * 100) / 100;
                // 商城佣金
                sad.PromotionAmount = Math.Truncate((sad.OrderRealAmount - sad.SellerAmount - sad.OrderSpreadAmount.Value) * 100) / 100;
            }
            // 2.自营自配自采
            // 3.自营自配统采
            if (contextSession != null)
            {
                foreach (var item in items)
                {
                    contextSession.SaveObject(item);
                }
                contextSession.SaveObject(sad);
            }
            return sad;
        }

        /// <summary>
        /// 担保交易（金币）支付，生成结算单
        /// </summary>
        public static SettleAccounts CreateSettleAccount(ContextSession contextSession, CommodityOrder commodityOrder)
        {
            // 未加入场馆的不生成结算单
            if (!commodityOrder.EsAppId.HasValue) return null;

            if (!CheckPayType(commodityOrder))
            {
                return null;
            }

            var parSources = PaySource.GetPaymentByTradeType(0);

            if (commodityOrder.Payment != 0)
            {
                LogHelper.Warn("担保交易（金币）支付，生成结算单失败，该订单：[" + commodityOrder.Id + "]非金币支付。");
                return null;
            }

            // 查询场馆信息
            var mall = MallApply.ObjectSet().Where(m => m.AppId == commodityOrder.AppId && m.EsAppId == commodityOrder.EsAppId && (m.State.Value == 2 || m.State.Value == 4)).FirstOrDefault();
            if (mall == null)
            {
                LogHelper.Info("生成结算项失败，商城中未找到该APP，订单ID：" + commodityOrder.Id);
                return null;
            }
            return CreateSettleAccount(contextSession, commodityOrder, mall);
        }


        /// <summary>
        /// 担保交易（金币）支付，生成结算单
        /// </summary>
        public static SettleAccounts CreateSettleAccount(ContextSession contextSession, CommodityOrder commodityOrder, MallApply mall)
        {
            if (!CheckPayType(commodityOrder))
            {
                return null;
            }
            if (mall == null)
            {
                LogHelper.Info("生成结算项失败，商城中未找到该APP，订单ID：" + commodityOrder.Id);
                return null;
            }
            var sad = CreateSettleAccountDetails(contextSession, commodityOrder, mall);
            if (sad == null) return null;
            LogHelper.Info("担保交易（金币）支付，生成结算单，OrderId:" + commodityOrder.Id);
            // 结算单
            SettleAccounts sa = new SettleAccounts();
            sad.SAId = sa.Id = Guid.NewGuid();
            sa.UserId = Guid.Empty; // 系统生成结算单，UserId为空
            sa.AmountDate = sa.ModifiedOn = sa.SubTime = DateTime.Now;
            sa.AppId = commodityOrder.AppId;
            sa.AppName = APPSV.GetAppName(sa.AppId);
            sa.EsAppId = commodityOrder.EsAppId.Value;
            sa.SellerType = mall.Type;
            var pInfo = ZPHSV.Instance.GetAppPavilionInfo(new Jinher.AMP.ZPH.Deploy.CustomDTO.QueryAppPavilionParam { id = sa.EsAppId });
            if (string.IsNullOrEmpty(pInfo.pavilionName))
            {
                sa.EsAppName = "未找到";
                LogHelper.Error("OrderSV.CreateSettleAccount 生成结算单异常：未能从 ZPHSV.Instance.GetAppPavilionInfo 获取到场管名称。OrderId:" + commodityOrder.Id);
            }
            sa.EsAppName = pInfo.pavilionName;
            sa.OrderAmount = sad.OrderAmount;
            sa.OrderRealAmount = sad.OrderRealAmount;
            sa.CouponAmount = sad.OrderCouponAmount;
            sa.RefundAmount = sad.OrderRefundAmount;
            sa.PromotionCommissionAmount = sad.OrderPromotionCommissionAmount;
            sa.PromotionAmount = sad.PromotionAmount;
            sa.SellerAmount = sad.SellerAmount;
            sa.OrderYJBAmount = sad.OrderYJBAmount;
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
            sad.IsSettled = true;
            sad.SAId = sa.Id;

            contextSession.SaveObject(sa);
            return sa;
        }

        /// <summary>
        /// 取消订单时，删除结算详情
        /// </summary>
        public static void CancelOrder(ContextSession contextSession, CommodityOrder commodityOrder)
        {
            try
            {
                LogHelper.Info("结算单记录退款信息，全部退款，订单ID：" + commodityOrder.Id);
                var sad = SettleAccountsDetails.ObjectSet().Where(s => s.OrderId == commodityOrder.Id).FirstOrDefault();
                if (sad != null)
                {
                    foreach (var saoi in SettleAccountsOrderItem.ObjectSet().Where(s => s.OrderId == sad.Id).ToList())
                    {
                        contextSession.Delete(saoi);
                    }
                    contextSession.Delete(sad);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("结算单记录退款信息，全部退款，订单ID：" + commodityOrder.Id + "，错误：", ex);
            }
        }

        /// <summary>
        /// 售前退货时，记录退款金额
        /// </summary>
        public static void OrderRefund(ContextSession contextSession, CommodityOrder commodityOrder, OrderRefund orderRefund)
        {
            if (orderRefund.IsFullRefund.HasValue && orderRefund.IsFullRefund.Value)
            {
                CancelOrder(contextSession, commodityOrder);
            }
            else
            {
                LogHelper.Info("结算单记录售前退款信息，订单ID：" + commodityOrder.Id + "，退款：" + orderRefund.RefundMoney);
                try
                {
                    var sad = SettleAccountsDetails.FindByID(commodityOrder.Id);
                    if (sad != null)
                    {
                        if (sad.OrderRefundAmount == 0)
                        {
                            sad.OrderRefundAmount = orderRefund.RefundMoney;
                            sad.SellerAmount -= orderRefund.RefundMoney;
                        }
                        sad.EntityState = EntityState.Modified;
                        contextSession.SaveObject(sad);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error("结算单记录售前退款信息，订单ID：" + commodityOrder.Id + "，退款：" + orderRefund.RefundMoney + "，异常：", ex);
                }
            }
        }

        /// <summary>
        /// 确认收货时，记录确认时间
        /// </summary>
        public static void ConfirmOrder(ContextSession contextSession, CommodityOrder commodityOrder)
        {
            try
            {
                LogHelper.Info("结算单记录确认收货，订单ID：" + commodityOrder.Id);
                var sad = SettleAccountsDetails.FindByID(commodityOrder.Id);
                if (sad != null)
                {
                    sad.OrderConfirmTime = commodityOrder.ConfirmTime.Value;
                    sad.EntityState = EntityState.Modified;
                    contextSession.SaveObject(sad);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("结算单记录确认收货，订单ID：" + commodityOrder.Id + "，错误：", ex);
            }
        }

        /// <summary>
        /// 售后退货时，记录退款金额
        /// </summary>
        public static void OrderRefund(ContextSession contextSession, CommodityOrder commodityOrder, OrderRefundAfterSales orderRefundAfterSales)
        {
            if (orderRefundAfterSales.IsFullRefund.HasValue && orderRefundAfterSales.IsFullRefund == 1)
            {
                CancelOrder(contextSession, commodityOrder);
            }
            else
            {
                LogHelper.Info("结算单记录售后退款信息，订单ID：" + commodityOrder.Id + "，退款：" + orderRefundAfterSales.RefundMoney);
                try
                {
                    var sad = SettleAccountsDetails.FindByID(commodityOrder.Id);
                    if (sad != null)
                    {
                        if (sad.OrderRefundAmount == 0)
                        {
                            sad.OrderRefundAmount = orderRefundAfterSales.RefundMoney;
                            sad.SellerAmount -= orderRefundAfterSales.RefundMoney;
                        }
                        sad.EntityState = EntityState.Modified;
                        contextSession.SaveObject(sad);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error("结算单记录售后退款信息，订单ID：" + commodityOrder.Id + "，退款：" + orderRefundAfterSales.RefundMoney + "，异常：", ex);
                }
            }
        }

        /// <summary>
        /// 售后完成时，记录售后时间
        /// </summary>
        public static void AfterSalesEndOrder(ContextSession contextSession, CommodityOrderService commodityOrderService)
        {
            try
            {
                LogHelper.Info("结算单记录售后完成，订单ID：" + commodityOrderService.Id);
                var sad = SettleAccountsDetails.FindByID(commodityOrderService.Id);
                if (sad != null)
                {
                    sad.AfterSalesEndTime = commodityOrderService.EndTime.Value;
                    sad.EntityState = EntityState.Modified;
                    contextSession.SaveObject(sad);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("结算单记录售后完成，订单ID：" + commodityOrderService.Id + "，错误：", ex);
            }
        }

        /// <summary>
        /// 导入自营商家未设置结算价时的结算订单
        /// </summary>
        public static void ImportNotSettleOrder()
        {
            LogHelper.Info("SettleAccountHelper.ImportNotSettleOrder 开始导入未设置结算价的订单");
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                // 查询商城下APP
                var mallQuery = MallApply.ObjectSet().Where(m => m.Type == 0 && (m.State.Value == 2 || m.State.Value == 4));
                var startDate = new DateTime(2017, 9, 1);
                var hadAddOrderQuery = SettleAccountsDetails.ObjectSet().AsQueryable();
                foreach (var mall in mallQuery.ToList())
                {
                    var orderQuery = CommodityOrder.ObjectSet().Where(o =>
                        o.AppId == mall.AppId
                        && o.EsAppId == mall.EsAppId
                        && o.PaymentTime > startDate
                        && o.Payment != 0);
                    var orders = orderQuery.Join(CommodityOrderService.ObjectSet()
                        .Where(s => (s.State == 3 || s.State == 7 || s.State == 15)), o => o.Id, s => s.Id, (o, s) => new { s.State, o })
                        .Where(so => !hadAddOrderQuery.Where(h => h.EsAppId == mall.EsAppId && h.AppId == mall.AppId && h.OrderId == so.o.Id)
                        .Any(h => h.Id == so.o.Id))
                        .ToList();
                    if (orders.Count > 0)
                    {
                        // 生成结算项
                        foreach (var so in orders)
                        {
                            SettleAccountHelper.CreateSettleAccountDetails(contextSession, so.o, mall, so.State);
                        }
                    }
                }
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SettleAccountHelper.ImportNotSettleOrder 异常", ex));
            }
            LogHelper.Info("SettleAccountHelper.ImportNotSettleOrder 结束导入未设置结算价的订单");

            ImportNotSettleGoldOrder();
        }

        /// <summary>
        /// 导入自营商家未设置结算价时的金币支付订单
        /// </summary>
        public static void ImportNotSettleGoldOrder()
        {
            LogHelper.Info("SettleAccountHelper.ImportNotSettleGoldOrder 开始导入未设置结算价的金币支付订单");
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                // 查询商城下APP
                var mallQuery = MallApply.ObjectSet().Where(m => m.Type == 0 && (m.State.Value == 2 || m.State.Value == 4));
                //var startDate = new DateTime(2017, 9, 1);
                var hadAddOrderQuery = SettleAccountsDetails.ObjectSet().AsQueryable();
                ContextDTO contextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                // 生成结算项
                List<object> saveList = new List<object>();
                foreach (var mall in mallQuery.ToList())
                {
                    var orderQuery = CommodityOrder.ObjectSet().Where(o =>
                        o.AppId == mall.AppId
                        && o.EsAppId == mall.EsAppId
                        && o.PaymentTime > mall.SubTime
                        && o.Payment == 0);
                    var orders = orderQuery.Join(CommodityOrderService.ObjectSet()
                        .Where(s => (s.State == 15)), o => o.Id, s => s.Id, (o, s) => o)
                        .Where(o => !hadAddOrderQuery.Where(h => h.EsAppId == mall.EsAppId && h.AppId == mall.AppId && h.OrderId == o.Id)
                        .Any(h => h.Id == o.Id))
                        .ToList();
                    if (orders.Count > 0)
                    {
                        foreach (var o in orders)
                        {
                            LogHelper.Info("ImportNotSettleGoldOrder,导入自营商家未设置结算价时的金币支付订单，开始生成结算单，OrderId：" + o.Id);
                            var sa = SettleAccountHelper.CreateSettleAccount(contextSession, o, mall);
                            if (sa != null)
                            {
                                Jinher.AMP.App.Deploy.CustomDTO.AppIdOwnerIdTypeDTO applicationDTO = APPSV.Instance.GetAppOwnerInfo(o.AppId, contextDTO);
                                // 打款
                                var confirmDto = OrderSV.BuildConfirmPayDTOAfterSales(contextSession, o, out saveList, applicationDTO, isSaveObject: false);
                                LogHelper.Info("导入自营商家未设置结算价时的金币支付订单，开始打款，OrderId:" + o.Id + "ConfirmPayDTO: " + JsonHelper.JsonSerializer(confirmDto));
                                var goldPayResult = Jinher.AMP.BTP.TPS.FSPSV.Instance.ConfirmPay(confirmDto);
                                if (goldPayResult.Code != 0)
                                {
                                    // 打款失败
                                    sa.IsPaySuccess = false;
                                    LogHelper.Info("导入自营商家未设置结算价时的金币支付订单，结束打款，失败，OrderId:" + o.Id);

                                    var errorInfo = JsonHelper.JsonSerializer(goldPayResult);
                                    LogHelper.Error("OrderSV.CreateSettleAccount 生成结算单，打款异常，OrderId: " + o.Id + " ReturnInfoDTO: " + errorInfo);
                                    SettleAccountsException exception = new SettleAccountsException();
                                    exception.Id = sa.Id;
                                    exception.OrderId = o.Id;
                                    exception.OrderCode = o.Code;
                                    exception.OrderRealPrice = o.RealPrice;
                                    exception.ClearingPrice = sa.SellerAmount;
                                    exception.ExceptionInfo = errorInfo;
                                    exception.AppId = sa.AppId;
                                    exception.AppName = sa.AppName;
                                    exception.EntityState = EntityState.Added;
                                    contextSession.SaveObject(exception);
                                }
                                else
                                {
                                    // 打款成功
                                    sa.IsPaySuccess = true;
                                    LogHelper.Info("导入自营商家未设置结算价时的金币支付订单，结束打款，成功，OrderId:" + o.Id);
                                }
                                LogHelper.Info("ImportNotSettleGoldOrder,导入自营商家未设置结算价时的金币支付订单，结束生成结算单，OrderId：" + o.Id);
                            }
                        }
                    }
                }
                if (saveList != null && saveList.Any())
                {
                    foreach (var o in saveList)
                    {
                        contextSession.SaveObject(o);
                    }
                }
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SettleAccountHelper.ImportNotSettleGoldOrder 异常", ex));
            }
            LogHelper.Info("SettleAccountHelper.ImportNotSettleGoldOrder 结束导入未设置结算价的金币支付订单");
        }

        public static bool CheckPayType(CommodityOrder commodityOrder)
        {
            var paySources = PaySource.GetPaymentByTradeType(0);
            if (paySources.Contains(commodityOrder.Payment))
            {
                return true;
            }
            LogHelper.Warn("担保交易（金币）支付，生成结算单失败，该订单：[" + commodityOrder.Id + "]非担保交易。");
            return false;
        }
    }
}
