using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Jinher.AMP.App.Deploy;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.BE.BELogic;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.CBC.Deploy.CustomDTO;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;
using Attribute = System.Attribute;
using Jinher.AMP.BTP.TPS.Helper;

namespace Jinher.AMP.BTP.SV.Order
{
    /// <summary>
    /// 订单处理基类
    /// </summary>
    public abstract class OrderDealBase
    {
        /// <summary>
        /// 校验
        /// </summary>
        /// <returns></returns>
        public abstract bool InitAndValid();
        internal abstract ResultDTO Do();
        internal abstract void SendMessage();
        /// <summary>
        /// 增加日志
        /// </summary>
        internal abstract void CreateJounal();
        /// <summary>
        /// 刷新缓存
        /// </summary>
        internal abstract void RefreshCache();

        /// <summary>
        /// 处理拼团
        /// </summary>
        /// <returns>是否拼团</returns>
        internal abstract bool DealDiyGroup();
        /// <summary>
        /// 处理众销
        /// </summary>
        /// <returns>是否众销</returns>
        internal abstract bool DealSharePromotion();
        /// <summary>
        /// 计算三级分销
        /// </summary>
        /// <returns>是否三级分销</returns>
        internal abstract bool DealDistribute();
        /// <summary>
        /// 计算渠道推广
        /// </summary>
        /// <returns>是否渠道推广</returns>
        internal abstract bool DealChannel();
        /// <summary>
        /// 处理好运来
        /// </summary>
        /// <returns>是否好运来</returns>
        internal abstract bool DealHyl();
        /// <summary>
        /// 处理众筹
        /// </summary>
        /// <returns>是否众筹</returns>
        internal abstract bool DealCrowdfunding();

        #region 公共属性
        /// <summary>
        /// 订单id
        /// </summary>
        public Guid OrderId { get; set; }
        /// <summary>
        /// 当前类处理业务名称
        /// </summary>
        public string BusinessName { get; set; }
        ///// <summary>
        ///// 是否存在错误
        ///// </summary>
        //public bool HasError { get; set; }

        internal ContextSession ContextSession { get; set; }

        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime Now { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ContextDTO ContextDTO { get; set; }
        /// <summary>
        /// 旧订单状态
        /// </summary>
        public int OldState { get; set; }
        /// <summary>
        /// 新订单状态
        /// </summary>
        public int NewState { get; set; }
        /// <summary>
        /// 返回结果
        /// </summary>
        public ResultDTO Result { get; set; }

        internal List<Commodity> NeedRefreshCacheCommoditys { get; set; }
        internal List<TodayPromotion> NeedRefreshCacheTodayPromotions { get; set; }
        /// <summary>
        /// 处理人
        /// </summary>
        public Guid Operator { get; set; }

        /// <summary>
        /// 订单
        /// </summary>
        internal CommodityOrder Order { get; set; }
        /// <summary>
        /// 订单项
        /// </summary>
        internal List<OrderItem> OrderItemList { get; set; }
        /// <summary>
        /// 订单商品
        /// </summary>
        public List<Commodity> CommodityList { get; set; }

        public List<CommodityStock> CommodityStockList { get; set; }

        #endregion

        /// <summary>
        /// 订单处理基类
        /// </summary>
        /// <param name="orderId"></param>
        protected OrderDealBase(Guid orderId)
        {
            NeedRefreshCacheCommoditys = new List<Commodity>();
            NeedRefreshCacheTodayPromotions = new List<TodayPromotion>();
            Result = new ResultDTO { ResultCode = 0, Message = "Success" };
            Now = DateTime.Now;
            OrderId = orderId;
            ContextDTO = Jinher.AMP.BTP.Common.AuthorizeHelper.CoinInitAuthorizeInfo();
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        public ResultDTO Execute()
        {
            if (!OrderSV.LockOrder(Order.Id))
            {
                return new ResultDTO { ResultCode = 110, Message = "操作失败" };
            }
            if (!InitAndValid())
                return new ResultDTO() { ResultCode = 1, Message = "参数有误" };

            try
            {
                ContextSession = ContextFactory.CurrentThreadContext;

                if (DealChannel())
                {

                }
                else if (DealDistribute())
                {

                }
                else if (DealSharePromotion())
                {

                }

                var result = Do();
                if (result != null && result.ResultCode == 0)
                {
                    //try
                    //{
                    //    SendMessage();
                    //}
                    //catch (Exception )
                    //{

                    //}
                    ContextSession.SaveChanges();
                    RefreshCache();

                    OrderEventHelper.OnOrderPaySuccess(Order);
                }
            }
            catch (Exception e)
            {

            }

            OrderSV.UnLockOrder(Order.Id);
            return Result;
        }

        private string _logMessage;
        public string LogMessage
        {
            get
            {
                if (string.IsNullOrEmpty(_logMessage))
                {
                    //TODO 显示参数
                }
                return _logMessage;
            }
        }

        private bool hasError()
        {
            if (Result != null && Result.ResultCode != 0)
                return true;
            return false;
        }
    }

    public class OrderPayCallBack : OrderDealBase
    {
        private int _payment;
        private ulong _gold;
        private decimal _money;
        private decimal _couponCount;


        //是否为拼团订单，拼团订单不发商品，并且订单状态改为16
        bool _isDiyGroup = false;

        private string _shareId;
        private decimal _shareMoney;
        private Guid _shareUseId;


        public OrderPayCallBack(Guid orderId, Guid userId, int payment, ulong gold, decimal money, decimal couponCount)
            : base(orderId)
        {
            BusinessName = "订单支付回调";
            Operator = userId;
            _payment = payment;
            _gold = gold;
            _money = money;
            _couponCount = couponCount;
        }
        public override bool InitAndValid()
        {
            if (OrderId == Guid.Empty)
            {
                Result.ResultCode = 1;
                Result.Message = "参数无效";
                return false;
            }
            Order = CommodityOrder.ObjectSet().FirstOrDefault(c => c.Id == OrderId);
            if (Order == null)
            {
                Result.ResultCode = 1;
                Result.Message = "参数无效";
                return false;
            }

            OldState = Order.State;
            if (Order.State != 0 && Order.State != 11 && Order.State != 4 && Order.State != 5 && Order.State != 6 && Order.State != 17)
            {
                Result.ResultCode = 1;
                Result.Message = "订单状态无法支付";
                return false;
            }
            //所有支付方式
            List<int> allPayTypes = new List<int>();
            List<PaySource> psList = PaySource.GetAllPaySources();
            if (psList != null && psList.Any())
            {
                allPayTypes = psList.Select(ps => ps.Payment).Distinct().ToList();
            }
            if (allPayTypes.Contains(_payment))
            {
                Order.Payment = _payment;
            }
            else
            {
                Result.ResultCode = 1;
                Result.Message = "支付方式不存在";
                return false;
            }

            OrderItemList = OrderItem.ObjectSet().Where(c => c.CommodityOrderId == OrderId).ToList();
            var comIds = base.OrderItemList.Select(c => c.CommodityId).Distinct().ToList();
            base.CommodityList = Commodity.ObjectSet().Where(c => comIds.Contains(c.Id)).ToList();

            CommodityStockList = new List<CommodityStock>();
            var commodityStockIds = OrderItemList.Select(a => a.CommodityStockId).Distinct().ToList();
            if (commodityStockIds.Any())
                CommodityStockList = CommodityStock.ObjectSet().Where(c => commodityStockIds.Contains(c.Id)).ToList();


            Operator = Operator == Guid.Empty ? Order.UserId : Operator;
            getShareInfo();
            return true;
        }


        /// <summary>
        /// 支付成功后更新订单
        /// </summary>
        /// <returns></returns>
        internal override ResultDTO Do()
        {
            Dictionary<Guid, Commodity> littleStockComDict = new Dictionary<Guid, Commodity>();
            foreach (var orderItem in OrderItemList)
            {
                //减库存 新逻辑  zgx-modify
                Commodity com = CommodityList.First(c => c.Id == orderItem.CommodityId);
                CommodityStock cStock = null;
                if (orderItem.CommodityStockId.HasValue && orderItem.CommodityStockId != Guid.Empty)
                    cStock = CommodityStockList.First(c => c.Id == orderItem.CommodityStockId);
                if (cStock != null)
                {
                    if (com.Stock <= 0 || cStock.Stock <= 0)
                    {
                        LogHelper.Error(string.Format("PayUpdateCommodityOrderSV:_orderId:{0},商品库存不足,商品id:{1}", Order.Id, com.Id));
                    }
                    com.Stock -= orderItem.Number;
                    com.Salesvolume += orderItem.Number;
                    com.ModifiedOn = Now;
                    com.EntityState = System.Data.EntityState.Modified;

                    cStock.Stock -= orderItem.Number;
                    cStock.ModifiedOn = Now;
                    cStock.EntityState = System.Data.EntityState.Modified;
                }
                else
                {
                    if (com.Stock <= 0)
                    {
                        LogHelper.Error(string.Format("PayUpdateCommodityOrderSV:_orderId:{0},商品库存不足,商品id:{1}", Order.Id, com.Id));
                    }
                    com.Stock -= orderItem.Number;
                    com.Salesvolume += orderItem.Number;
                    com.ModifiedOn = Now;
                    com.EntityState = System.Data.EntityState.Modified;
                }
                NeedRefreshCacheCommoditys.RemoveAll(c => c.Id == com.Id);
                NeedRefreshCacheCommoditys.Add(com);
            }

            Order.PaymentTime = Now;
            Order.ModifiedOn = Now;
            if (!_isDiyGroup)
            {
                Order.State = 1;
            }
            else
            {
                //拼团时，修改团订单状态为16
                Order.State = 16;
            }

            Order.EntityState = System.Data.EntityState.Modified;
            if (_gold != 0)
            {
                Order.GoldPrice = _gold * 0.001m;
            }
            if (Order.GoldPrice + _money + _couponCount != Order.RealPrice)
            {
                LogHelper.Info(string.Format("混合支付金额不一致异常：金币抵用值:{0},在线支付值:{1},订单实收价:{2}", Order.GoldPrice, _money, Order.RealPrice));
            }
            Order.GoldCoupon = _couponCount;



            //担保交易
            IEnumerable<int> secTrans = PaySource.GetAllPaySources().Select(c => c.Payment).Where(p => p < 1000 && p != 1 && p != 2);

            //保存发票以及发票历史记录
            dealInvoice();

            #region 众筹订单付款

            if (CustomConfig.CrowdfundingFlag && secTrans.Contains(_payment) && Order.RealPrice > 0)
            {
                CrowdfundingPay();
            }

            #endregion

            #region 取得订单可分金额以及分享信息

            _shareMoney = Order.IsModifiedPrice
                               ? Order.RealPrice.Value
                               : Order.RealPrice.Value - Order.Freight;
            _shareId = OrderShareMess.ObjectSet().Where(c => c.OrderId == Order.Id).Select(c => c.ShareId).FirstOrDefault();
            SNS.Deploy.CustomDTO.ReturnInfoDTO<Guid> shareServiceResult = SNSSV.Instance.GetShareUserId(_shareId);
            if (shareServiceResult != null)
            {
                if (shareServiceResult.Code == "0")
                {
                    _shareUseId = shareServiceResult.Content;
                }
                else
                {
                    LogHelper.Error(string.Format("金币回调时，根据分享Id获取分享人Id\" 不成功,分享Id={0}，返回结果={1}", _shareId, JsonHelper.JsonSerializer(shareServiceResult)));
                }
            }
            #endregion


            //刷新缓存商品缓存
            if (NeedRefreshCacheCommoditys.Any())
            {
                NeedRefreshCacheCommoditys.ForEach(c => c.RefreshCache(EntityState.Modified));
            }
            //刷新优惠信息缓存
            if (NeedRefreshCacheTodayPromotions.Any())
            {
                NeedRefreshCacheTodayPromotions.ForEach(c => c.RefreshCache(EntityState.Modified));
            }

            AddMessage addmassage = new AddMessage();
            addmassage.AddMessages(Order.Id.ToString(), Order.UserId.ToString(), Order.EsAppId.Value, Order.Code, 1, "", "order");


            return new ResultDTO { ResultCode = 0, Message = "Success" };


        }

        internal override void SendMessage()
        {
            Jinher.AMP.App.Deploy.ApplicationDTO applicationDTO = APPBP.Instance.GetAppById(Order.AppId); ;
            SendLittleStockComMessage(applicationDTO);
            SendMessageToAppOwner(applicationDTO);
            SendMessageToSelfTakeManagers(applicationDTO);

            try
            {
                decimal realPrice = Order.IsModifiedPrice ? Order.RealPrice.Value : Order.Price;
                if (Order.SrcType == 33 || Order.SrcType == 34)
                {
                    SendShareUserMessage();
                }

                if (Order.SrcAppId != null && Order.SrcAppId != Guid.Empty)
                {
                    SendAppOwerMessage(realPrice, Order.SrcAppId.Value);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SendMessageToPayment 发送消息服务异常:。_orderId：{0}", Order.Id), ex);
            }




            SendMessageToSelfTakeManagers(applicationDTO);
        }

        internal override void CreateJounal()
        {
            //订单日志
            Journal journal = new Journal();
            journal.Id = Guid.NewGuid();
            journal.Code = Order.Code;
            journal.SubId = Operator;
            journal.SubTime = Now;
            journal.CommodityOrderId = Order.Id;
            journal.Name = "用户支付订单";
            journal.Details = string.Format("订单状态由{0}变为{1},支付方式为:{2} ", OldState, Order.State, _payment);

            journal.StateFrom = OldState;
            journal.StateTo = Order.State;
            journal.IsPush = false;
            journal.OrderType = Order.OrderType;

            journal.EntityState = System.Data.EntityState.Added;
            ContextSession.SaveObject(journal);
        }

        internal override void RefreshCache()
        {
            throw new NotImplementedException();
        }

        internal override bool DealDiyGroup()
        {
            bool result = false;

            //拼团时，修改团订单状态，并修改参团人数
            var diyGroupInfo = (from dgOrder in DiyGroupOrder.ObjectSet()
                                join dg in DiyGroup.ObjectSet()
                                    on dgOrder.DiyGroupId equals dg.Id
                                where dgOrder.OrderId == Order.Id && dgOrder.State == 0
                                select new
                                {
                                    DiyGroupOrder = dgOrder,
                                    DiyGroup = dg
                                }).FirstOrDefault();
            if (diyGroupInfo != null && diyGroupInfo.DiyGroup != null && diyGroupInfo.DiyGroupOrder != null)
            {
                result = true;

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
            return result;
        }

        internal override bool DealSharePromotion()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 记录发票信息以及发票历史记录表
        /// </summary>
        private void dealInvoice()
        {
            var invOrder = (from inv in Invoice.ObjectSet()
                            where inv.CommodityOrderId == Order.Id
                            select inv).FirstOrDefault();
            if (invOrder != null)
            {
                invOrder.State = 1;
                invOrder.EntityState = EntityState.Modified;
                invOrder.ModifiedOn = Now;
                var invoiceJounal = new InvoiceJounal(invOrder.Id, Guid.Empty, invOrder.State, 1);
                ContextSession.SaveObject(invoiceJounal);
            }
        }

        /// <summary>
        /// 渠道
        /// </summary>
        /// <param name="isSai"></param>
        private void DealChannel(out bool isSai)
        {
            isSai = false;
            var isChannelShareFunction = BACBP.CheckChannel(Order.AppId);
            LogHelper.Info(string.Format("金币回调：订单Id:{0} ,SrcType:{1},shareMoney:{2}, isChannelShareFunction:{3} ,appId:{4},esAppId:{5}  ", Order.Id, Order.SrcType, _shareMoney, isChannelShareFunction, Order.AppId, Order.EsAppId));
            var saiCode = ZPHSV.Instance.GetPromoCodeByShareId(Order.EsAppId.Value, _shareId);
            if ((Order.SrcType == 33 || Order.SrcType == 34)
                && _shareMoney > 0 && isChannelShareFunction && (!string.IsNullOrWhiteSpace(_shareId)) && saiCode != Guid.Empty)
            {
                var channelSharePercent = AppExtension.ObjectSet()
                       .Where(t => t.Id == Order.EsAppId)
                       .Select(t => t.ChannelSharePercent)
                       .FirstOrDefault();
                if (channelSharePercent > 0)
                {
                    isSai = true;
                    var orderItemList = OrderItem.ObjectSet().Where(c => c.CommodityOrderId == Order.Id).ToList();
                    //渠道推广金额
                    decimal commission = 0.0m;

                    var orderPayDetailsList = OrderPayDetail.ObjectSet().Where(c => c.OrderId == Order.Id).ToList();

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
                                    decimal moneyTmp = (orderItem.RealPrice.Value - (orderItem.RealPrice.Value / Order.Price) * couponModel.Amount - (orderItem.RealPrice.Value / Order.Price) * integrationAmount) * channelSharePercent.Value;

                                    OrderItemShare oiShare = OrderItemShare.CreateOrderItemShare();
                                    oiShare.Id = Guid.NewGuid();
                                    oiShare.SubTime = DateTime.Now;
                                    oiShare.ModifiedOn = DateTime.Now;
                                    oiShare.SubId = Operator;
                                    oiShare.SharePrice = orderItem.RealPrice.HasValue
                                                             ? orderItem.RealPrice.Value
                                                             : 0;
                                    oiShare.Commission = moneyTmp.ToMoney();
                                    oiShare.SharePercent = channelSharePercent.Value;
                                    oiShare.OrderId = orderItem.CommodityOrderId;
                                    oiShare.OrderItemId = orderItem.Id;
                                    oiShare.PayeeType = 12;
                                    oiShare.PayeeId = _shareUseId;
                                    oiShare.ShareKey = saiCode.ToString();
                                    oiShare.EntityState = EntityState.Added;
                                    ContextSession.SaveObject(oiShare);

                                    commission += orderItem.ShareMoney;
                                }
                            }
                            else if (couponModel.CouponType == 1)
                            {
                                //当优惠券为商品优惠券时
                                var couponCommodityAccount =
                                    orderItemList.Where(
                                        t => couponModel.CommodityIds.Contains(t.CommodityId.ToString()))
                                                 .Sum(t => t.RealPrice.Value);

                                if (couponCommodityAccount > 0)
                                {
                                    foreach (var orderItem in orderItemList)
                                    {
                                        decimal moneyTmp = 0m;
                                        if (couponModel.CommodityIds.Contains(orderItem.CommodityId.ToString()))
                                        {
                                            moneyTmp = (orderItem.RealPrice.Value -
                                                        (orderItem.RealPrice.Value /
                                                         couponCommodityAccount) *
                                                        couponModel.Amount -
                                                        (orderItem.RealPrice.Value / Order.Price) *
                                                        integrationAmount) * channelSharePercent.Value;
                                        }
                                        else
                                        {
                                            moneyTmp = (orderItem.RealPrice.Value -
                                                        (orderItem.RealPrice.Value / Order.Price) *
                                                        integrationAmount) * channelSharePercent.Value;
                                        }

                                        OrderItemShare oiShare = OrderItemShare.CreateOrderItemShare();
                                        oiShare.Id = Guid.NewGuid();
                                        oiShare.SubTime = DateTime.Now;
                                        oiShare.ModifiedOn = DateTime.Now;
                                        oiShare.SubId = Operator;
                                        oiShare.SharePrice = orderItem.RealPrice.HasValue
                                                                 ? orderItem.RealPrice.Value
                                                                 : 0;
                                        oiShare.Commission = moneyTmp.ToMoney();
                                        oiShare.SharePercent = channelSharePercent.Value;
                                        oiShare.OrderId = orderItem.CommodityOrderId;
                                        oiShare.OrderItemId = orderItem.Id;
                                        oiShare.PayeeType = 12;
                                        oiShare.PayeeId = _shareUseId;
                                        oiShare.ShareKey = saiCode.ToString();
                                        oiShare.EntityState = EntityState.Added;
                                        ContextSession.SaveObject(oiShare);

                                        commission += orderItem.ShareMoney;
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
                            //没有使用优惠券
                            foreach (var orderItem in orderItemList)
                            {
                                decimal moneyTmp = (orderItem.RealPrice.Value -
                                                    (orderItem.RealPrice.Value / Order.Price) *
                                                    integrationAmount) * channelSharePercent.Value;

                                OrderItemShare oiShare = OrderItemShare.CreateOrderItemShare();
                                oiShare.Id = Guid.NewGuid();
                                oiShare.SubTime = DateTime.Now;
                                oiShare.ModifiedOn = DateTime.Now;
                                oiShare.SubId = Operator;
                                oiShare.SharePrice = orderItem.RealPrice.HasValue ? orderItem.RealPrice.Value : 0;
                                oiShare.Commission = moneyTmp.ToMoney();
                                oiShare.SharePercent = channelSharePercent.Value;
                                oiShare.OrderId = orderItem.CommodityOrderId;
                                oiShare.OrderItemId = orderItem.Id;
                                oiShare.PayeeType = 12;
                                oiShare.PayeeId = _shareUseId;
                                oiShare.ShareKey = saiCode.ToString();
                                oiShare.EntityState = EntityState.Added;
                                ContextSession.SaveObject(oiShare);

                                commission += orderItem.ShareMoney;
                            }
                        }
                    }
                    else
                    {
                        foreach (var orderItem in orderItemList)
                        {
                            decimal moneyTmp = orderItem.RealPrice.Value * channelSharePercent.Value;

                            OrderItemShare oiShare = OrderItemShare.CreateOrderItemShare();
                            oiShare.Id = Guid.NewGuid();
                            oiShare.SubTime = DateTime.Now;
                            oiShare.ModifiedOn = DateTime.Now;
                            oiShare.SubId = Operator;
                            oiShare.SharePrice = orderItem.RealPrice.HasValue ? orderItem.RealPrice.Value : 0;
                            oiShare.Commission = moneyTmp.ToMoney();
                            oiShare.SharePercent = channelSharePercent.Value;
                            oiShare.OrderId = orderItem.CommodityOrderId;
                            oiShare.OrderItemId = orderItem.Id;
                            oiShare.PayeeType = 12;
                            oiShare.PayeeId = _shareUseId;
                            oiShare.ShareKey = saiCode.ToString();
                            oiShare.EntityState = EntityState.Added;
                            ContextSession.SaveObject(oiShare);

                            commission += orderItem.ShareMoney;
                        }
                    }

                    //渠道分享佣金
                    Order.ChannelShareMoney = commission;

                    //保存渠道分成到 OrderShare 表
                    if (_shareUseId != Guid.Empty)
                    {
                        decimal sharePriceTotal = OrderItemList.Sum(t => t.RealPrice.Value);

                        OrderShare os = OrderShare.CreateOrderShare();
                        os.Id = Guid.NewGuid();
                        os.SubTime = DateTime.Now;
                        os.ModifiedOn = DateTime.Now;
                        os.SubId = Operator;
                        os.SharePrice = sharePriceTotal;
                        os.Commission = Order.ChannelShareMoney;
                        os.SharePercent = channelSharePercent.Value;
                        os.OrderId = Order.Id;
                        os.PayeeType = 12;
                        os.PayeeId = _shareUseId;
                        os.ShareKey = saiCode.ToString();

                        os.EntityState = EntityState.Added;
                        ContextSession.SaveObject(os);
                    }
                }
            }
        }

        /// <summary>
        /// 众销
        /// </summary>
        private void SharePromotion()
        {
            var isSharePromotionFunction = BACBP.CheckSharePromotion(Order.AppId);
            LogHelper.Info(string.Format("金币回调：订单Id:{0} ,SrcType:{1},shareMoney:{2}, isSharePromotionFunction:{3} ,appId:{4},esAppId:{5}  ", Order.Id, Order.SrcType, _shareMoney, isSharePromotionFunction, Order.AppId, Order.EsAppId));

            //只有本应用下单才会走分成推广
            if ((Order.SrcType == 33 || Order.SrcType == 34)
                && _shareMoney > 0 && isSharePromotionFunction && Order.AppId == Order.EsAppId
                && (!string.IsNullOrWhiteSpace(_shareId)))
            {
                var orderItemList = OrderItem.ObjectSet().Where(c => c.CommodityOrderId == Order.Id).ToList();
                //众销推广金额
                decimal commission = 0.0m;

                var orderPayDetailsList = OrderPayDetail.ObjectSet().Where(c => c.OrderId == Order.Id).ToList();
                var appext = AppExtension.ObjectSet().FirstOrDefault(c => c.Id == Order.AppId);
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
                                decimal sharePercent = globalAmount;
                                if (sharePercent <= 0)
                                {
                                    var sharePercentTmp =
                                         commodityShareList.Where(t => t.Id == orderItem.CommodityId)
                                                           .Select(t => t.SharePercent)
                                                           .FirstOrDefault();
                                    sharePercent = sharePercentTmp.HasValue ? sharePercentTmp.Value : 0m;
                                }
                                decimal moneyTmp = (orderItem.RealPrice.Value -
                                                    (orderItem.RealPrice.Value / Order.Price) *
                                                    couponModel.Amount -
                                                    (orderItem.RealPrice.Value / Order.Price) *
                                                    integrationAmount) * sharePercent;

                                OrderItemShare oiShare = OrderItemShare.CreateOrderItemShare();
                                oiShare.Id = Guid.NewGuid();
                                oiShare.SubTime = DateTime.Now;
                                oiShare.ModifiedOn = DateTime.Now;
                                oiShare.SubId = Operator;
                                oiShare.SharePrice = orderItem.RealPrice.HasValue
                                                         ? orderItem.RealPrice.Value
                                                         : 0;
                                oiShare.Commission = moneyTmp.ToMoney();
                                oiShare.SharePercent = sharePercent;
                                oiShare.OrderId = orderItem.CommodityOrderId;
                                oiShare.OrderItemId = orderItem.Id;
                                oiShare.PayeeType = 3;
                                oiShare.PayeeId = _shareUseId;
                                oiShare.ShareKey = _shareId;
                                oiShare.EntityState = EntityState.Added;
                                ContextSession.SaveObject(oiShare);

                                commission += orderItem.ShareMoney;
                            }
                        }
                        else if (couponModel.CouponType == 1)
                        {
                            //当优惠券为商品优惠券时
                            var couponCommodityAccount =
                                orderItemList.Where(
                                    t => couponModel.CommodityIds.Contains(t.CommodityId.ToString()))
                                             .Sum(t => t.RealPrice.Value);

                            if (couponCommodityAccount > 0)
                            {
                                foreach (var orderItem in orderItemList)
                                {
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
                                        moneyTmp = (orderItem.RealPrice.Value -
                                                    (orderItem.RealPrice.Value /
                                                     couponCommodityAccount) *
                                                    couponModel.Amount -
                                                    (orderItem.RealPrice.Value / Order.Price) *
                                                    integrationAmount) * sharePercent;
                                    }
                                    else
                                    {
                                        moneyTmp = (orderItem.RealPrice.Value -
                                                    (orderItem.RealPrice.Value / Order.Price) *
                                                    integrationAmount) * sharePercent;
                                    }

                                    OrderItemShare oiShare = OrderItemShare.CreateOrderItemShare();
                                    oiShare.Id = Guid.NewGuid();
                                    oiShare.SubTime = DateTime.Now;
                                    oiShare.ModifiedOn = DateTime.Now;
                                    oiShare.SubId = Operator;
                                    oiShare.SharePrice = orderItem.RealPrice.HasValue
                                                             ? orderItem.RealPrice.Value
                                                             : 0;
                                    oiShare.Commission = moneyTmp.ToMoney();
                                    oiShare.SharePercent = sharePercent;
                                    oiShare.OrderId = orderItem.CommodityOrderId;
                                    oiShare.OrderItemId = orderItem.Id;
                                    oiShare.PayeeType = 3;
                                    oiShare.PayeeId = _shareUseId;
                                    oiShare.ShareKey = _shareId;
                                    oiShare.EntityState = EntityState.Added;
                                    ContextSession.SaveObject(oiShare);

                                    commission += orderItem.ShareMoney;
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
                        //没有使用优惠券
                        foreach (var orderItem in orderItemList)
                        {
                            decimal sharePercent = globalAmount;
                            if (sharePercent <= 0)
                            {
                                var sharePercentTmp =
                                     commodityShareList.Where(t => t.Id == orderItem.CommodityId)
                                                       .Select(t => t.SharePercent)
                                                       .FirstOrDefault();
                                sharePercent = sharePercentTmp.HasValue ? sharePercentTmp.Value : 0m;
                            }
                            decimal moneyTmp = (orderItem.RealPrice.Value -
                                                (orderItem.RealPrice.Value / Order.Price) *
                                                integrationAmount) * sharePercent;

                            OrderItemShare oiShare = OrderItemShare.CreateOrderItemShare();
                            oiShare.Id = Guid.NewGuid();
                            oiShare.SubTime = DateTime.Now;
                            oiShare.ModifiedOn = DateTime.Now;
                            oiShare.SubId = Operator;
                            oiShare.SharePrice = orderItem.RealPrice.HasValue ? orderItem.RealPrice.Value : 0;
                            oiShare.Commission = moneyTmp.ToMoney();
                            oiShare.SharePercent = sharePercent;
                            oiShare.OrderId = orderItem.CommodityOrderId;
                            oiShare.OrderItemId = orderItem.Id;
                            oiShare.PayeeType = 3;
                            oiShare.PayeeId = _shareUseId;
                            oiShare.ShareKey = _shareId;
                            oiShare.EntityState = EntityState.Added;
                            ContextSession.SaveObject(oiShare);

                            commission += orderItem.ShareMoney;
                        }
                    }
                }
                else
                {
                    foreach (var orderItem in orderItemList)
                    {
                        decimal sharePercent = globalAmount;
                        if (sharePercent <= 0)
                        {
                            var sharePercentTmp =
                                 commodityShareList.Where(t => t.Id == orderItem.CommodityId)
                                                   .Select(t => t.SharePercent)
                                                   .FirstOrDefault();
                            sharePercent = sharePercentTmp.HasValue ? sharePercentTmp.Value : 0m;
                        }
                        decimal moneyTmp = orderItem.RealPrice.Value * sharePercent;

                        OrderItemShare oiShare = OrderItemShare.CreateOrderItemShare();
                        oiShare.Id = Guid.NewGuid();
                        oiShare.SubTime = DateTime.Now;
                        oiShare.ModifiedOn = DateTime.Now;
                        oiShare.SubId = Operator;
                        oiShare.SharePrice = orderItem.RealPrice.HasValue ? orderItem.RealPrice.Value : 0;
                        oiShare.Commission = moneyTmp.ToMoney();
                        oiShare.SharePercent = sharePercent;
                        oiShare.OrderId = orderItem.CommodityOrderId;
                        oiShare.OrderItemId = orderItem.Id;
                        oiShare.PayeeType = 3;
                        oiShare.PayeeId = _shareUseId;
                        oiShare.ShareKey = _shareId;
                        oiShare.EntityState = EntityState.Added;
                        ContextSession.SaveObject(oiShare);

                        commission += orderItem.ShareMoney;
                    }
                }

                //众销分享佣金
                Order.Commission = commission;

                //保存众销分成到 OrderShare 表
                if (_shareUseId != Guid.Empty)
                {
                    decimal sharePriceTotal = OrderItemList.Sum(t => t.RealPrice.Value);

                    OrderShare os = OrderShare.CreateOrderShare();
                    os.Id = Guid.NewGuid();
                    os.SubTime = DateTime.Now;
                    os.ModifiedOn = DateTime.Now;
                    os.SubId = Operator;
                    os.SharePrice = sharePriceTotal;
                    os.Commission = commission;
                    os.SharePercent = 0;
                    os.OrderId = Order.Id;
                    os.PayeeType = 3;
                    os.PayeeId = _shareUseId;
                    os.ShareKey = _shareId;

                    os.EntityState = EntityState.Added;
                    ContextSession.SaveObject(os);
                }

            }
        }

        #region 消息相关

        /// <summary>
        /// 发送商品数量紧张消息
        /// </summary>
        /// <param name="applicationDTO"></param>
        /// <param name="littleStockComDict"></param>
        private void SendLittleStockComMessage(Jinher.AMP.App.Deploy.ApplicationDTO applicationDTO)
        {
            var littleStockCom = CommodityList.Where(c => c.Stock <= 1).ToList();

            if (applicationDTO != null && applicationDTO.OwnerId.HasValue && applicationDTO.OwnerId != Guid.Empty && applicationDTO.OwnerType == Jinher.AMP.App.Deploy.Enum.AppOwnerTypeEnum.Personal)
            {
                Jinher.AMP.Info.Deploy.CustomDTO.MessageForAddDTO messageAdd =
                     new Info.Deploy.CustomDTO.MessageForAddDTO();

                foreach (var com in littleStockCom)
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
                            messageAdd.Title = "【" + applicationDTO.Name + "】" + "【" + com.No_Code + "】【" + com.Name + "】商品数量紧张，请关注！";
                            messageAdd.Content = "【" + applicationDTO.Name + "】" + "【" + com.No_Code + "】【" + com.Name + "】商品数量紧张，请关注！";
                        }
                        else
                        {
                            messageAdd.Title = "【" + applicationDTO.Name + "】" + "【" + com.Name + "】商品数量紧张，请关注！";
                            messageAdd.Content = "【" + applicationDTO.Name + "】" + "【" + com.Name + "】商品数量紧张，请关注！";
                        }

                        messageAdd.ReceiverRange = Info.Deploy.Enum.ReceiverRange.SpecifiedUser;
                        Jinher.AMP.BTP.TPS.InfoSV.Instance.AddSystemMessage(messageAdd);
                    }
                    catch (Exception ex)
                    {

                        LogHelper.Error(string.Format("InfoManageSV服务异常:获取应用信息异常。_orderId：{0}", Order.Id), ex);
                    }
                }
            }
        }

        /// <summary>
        /// 给享者发送消息
        /// </summary>
        private void SendShareUserMessage()
        {
            if (_orderShareInfoDTO != null && _orderShareInfoDTO.IsShare)
            {
                decimal realPrice = Order.IsModifiedPrice ? Order.RealPrice.Value : Order.Price;
                var buyerPercent = CustomConfig.SaleShare.Commission * CustomConfig.SaleShare.BuyerPercent;
                SendMessageToPayment(new List<Guid> { _orderShareInfoDTO.ShareUser }, "Payment", realPrice.ToString(), ContextDTO, buyerPercent);
            }


            List<Guid> userIds = new List<Guid>();
            OrderShareMess tempShare =
                               OrderShareMess.ObjectSet().FirstOrDefault(c => c.OrderId == Order.Id);
            if (tempShare != null && !string.IsNullOrEmpty(tempShare.ShareId) &&
                tempShare.ShareId.ToLower() != "null" && tempShare.ShareId.ToLower() != "undefined")
            {

            }
        }

        /// <summary>
        /// 给应用主发送消息
        /// </summary>
        /// <param name="realPrice"></param>
        /// <param name="srcAppId"></param>
        private void SendAppOwerMessage(decimal realPrice, Guid srcAppId)
        {
            List<Guid> userIds = new List<Guid>();
            var result = APPSV.Instance.GetAppOwnerInfo(Guid.Parse(srcAppId.ToString()));
            if (result.OwnerType == App.Deploy.Enum.AppOwnerTypeEnum.Org)
            {
                List<Guid> orgUserIds = Jinher.AMP.BTP.TPS.EBCSV.Instance.GetUserIdsByOrgIdAndCode(result.OwnerId, "ReceiveRed");

                SendMessageToPayment(orgUserIds, "Payment", realPrice.ToString(), ContextDTO, CustomConfig.ShareOwner.OwnerPercent);
            }
            else if (result.OwnerType == App.Deploy.Enum.AppOwnerTypeEnum.Personal)
            {
                userIds.Clear();
                userIds.Add(result.OwnerId);
                SendMessageToPayment(userIds, "Payment", realPrice.ToString(), ContextDTO, CustomConfig.ShareOwner.OwnerPercent);
            }
        }

        /// <summary>
        /// 发消息 （用户付款后消息；确认收货后发消息）
        /// </summary>
        /// <param name="userids">接受人userid集合</param>
        /// <param name="type">type： Payment 用户付款后消息； affirm 确认收货后发消息</param>
        /// <param name="number">数量</param>
        private void SendMessageToPayment(List<Guid> userids, string type, string number, ContextDTO contextDTO, decimal percent)
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

        /// <summary>
        /// 众筹订单支付
        /// </summary>
        private void CrowdfundingPay()
        {
            if (Order == null)
            {
                return;
            }
            try
            {
                //众筹基本表
                var CrowdfundingQuery = Crowdfunding.ObjectSet().FirstOrDefault(q => q.AppId == Order.AppId && q.StartTime < Now);

                if (CrowdfundingQuery == null)
                    return;

                //众筹股东表
                var UserCrowdfundingQuery = UserCrowdfunding.ObjectSet().FirstOrDefault(q => q.UserId == Order.UserId && q.AppId == Order.AppId);


                //  如果订单 改过价格，那么取改价后的价钱，如果没改过价格，就取不包含运费的价格
                decimal realPrice = Order.IsModifiedPrice
                                        ? Order.RealPrice.Value
                                        : Order.Price;

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
                        UserCrowdfundingQuery.UserId = Order.UserId;
                        UserCrowdfundingQuery.CrowdfundingId = CrowdfundingQuery.Id;
                        UserCrowdfundingQuery.AppId = Order.AppId;

                        try
                        {
                            List<UserNameAccountsDTO> userNamelist = CBCSV.Instance.GetUserNameAccountsByIds(new List<Guid>() { Order.UserId });

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
                    UserCrowdfundingQuery.OrdersMoney += Order.RealPrice.Value;

                    //已获得股数大于总股数
                    if (CrowdfundingCountQuery.CurrentShareCount + result >= CrowdfundingCountQuery.ShareCount)
                    {
                        //剩余股数
                        var max = CrowdfundingCountQuery.ShareCount - CrowdfundingCountQuery.CurrentShareCount;
                        //众筹有效金额
                        Order.CrowdfundingPrice = max * CrowdfundingQuery.PerShareMoney -
                                                           (UserCrowdfundingQuery.Money -
                                                            UserCrowdfundingQuery.CurrentShareCount *
                                                            CrowdfundingQuery.PerShareMoney);
                        //用户能购买的股数
                        UserCrowdfundingQuery.CurrentShareCount += max;
                        //用户购买股数对应的金额
                        UserCrowdfundingQuery.Money += Order.CrowdfundingPrice;
                        //修改众筹状态为成功
                        CrowdfundingQuery.State = 1;
                        CrowdfundingQuery.EntityState = System.Data.EntityState.Modified;
                        ContextSession.SaveObject(CrowdfundingQuery);
                        CrowdfundingCountQuery.CurrentShareCount = CrowdfundingCountQuery.ShareCount;
                    }
                    else
                    {
                        UserCrowdfundingQuery.Money += realPrice;
                        UserCrowdfundingQuery.CurrentShareCount =
                            (long)(UserCrowdfundingQuery.Money / CrowdfundingQuery.PerShareMoney);
                        CrowdfundingCountQuery.CurrentShareCount = CrowdfundingCountQuery.CurrentShareCount + result;
                        //众筹有效金额
                        Order.CrowdfundingPrice = realPrice;

                    }

                    ContextSession.SaveObject(UserCrowdfundingQuery);
                    CrowdfundingCountQuery.EntityState = System.Data.EntityState.Modified;
                    ContextSession.SaveObject(CrowdfundingCountQuery);
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

                    Order.IsCrowdfunding = 1;
                }
                else
                {
                    if (UserCrowdfundingQuery != null)
                    {
                        //众筹成功
                        //股东实际消费金额
                        UserCrowdfundingQuery.OrdersMoney += Order.RealPrice.Value;
                        //购买活动总订单数
                        UserCrowdfundingQuery.OrderCount += 1;
                        UserCrowdfundingQuery.EntityState = System.Data.EntityState.Modified;
                        ContextSession.SaveObject(UserCrowdfundingQuery);
                    }
                    Order.IsCrowdfunding = 2;
                }

            }
            catch (Exception ex)
            {

                LogHelper.Error(string.Format("CommodityOrderSV-GetUserAccount。Order{0}", JsonHelper.JsonSerializer(Order)), ex);

            }
        }





        /// <summary>
        /// 向自提点管理员发消息
        /// </summary>
        /// <param name="state"></param>
        /// <param name="commodityOrder"></param>
        private void SendMessageToSelfTakeManagers(Jinher.AMP.App.Deploy.ApplicationDTO applicationDTO)
        {
            if (Order.SelfTakeFlag != 1)
                return;
            AddMessage addmassage = new AddMessage();
            var managerIdList = (from orderPickUp in OrderPickUp.ObjectSet()
                                 join manager in SelfTakeStationManager.ObjectSet() on orderPickUp.SelfTakeStationId
                                     equals manager.SelfTakeStationId
                                 where orderPickUp.OrderId == OrderId
                                 select manager.UserId).Distinct().ToList();
            if (!managerIdList.Any())
                return;
            string messages = string.Format("{0}有客户下单并支付啦，快快发货吧~订单编号：{1}", "【" + applicationDTO.Name + "】", Order.Code);
            addmassage.AddMessages(Order.Id.ToString(), string.Join(",", managerIdList), CustomConfig.SelfTakeAppId, Order.Code, Order.State, messages, "selfTakeManager");
        }

        private void SendMessageToAppOwner(Jinher.AMP.App.Deploy.ApplicationDTO applicationDTO)
        {
            //拼团订单等业务支付成功时，订单状态并不是代发货状态，所以不能发送消息
            if (Order.State != 1)
                return;

            AddMessage addmassage = new AddMessage();
            if (applicationDTO != null && applicationDTO.OwnerId != null && applicationDTO.OwnerId.Value != Guid.Empty)
            {
                List<Guid> LGuid = new List<Guid>();
                string messages = string.Format("{0}有客户下单并支付啦，快快发货吧~订单编号：{1}", "【" + applicationDTO.Name + "】", Order.Code);

                LGuid = EBCSV.GetOrderMenuUsers(applicationDTO);
                if (LGuid == null || !LGuid.Any())
                    return;
                System.Text.StringBuilder strOrgUserIds = new System.Text.StringBuilder();
                foreach (Guid orgUserId in LGuid)
                {
                    strOrgUserIds.Append(orgUserId).Append(",");
                }
                strOrgUserIds.Remove(strOrgUserIds.Length - 1, 1);
                addmassage.AddMessages(Order.Id.ToString(), strOrgUserIds.ToString(), Order.AppId, Order.Code, Order.State, messages, "orderAppOwner");

                //后台发消息 
                //推送消息
                Jinher.AMP.Info.Deploy.CustomDTO.MessageForAddDTO messageAdd = new Info.Deploy.CustomDTO.MessageForAddDTO
                {
                    PublishTime = DateTime.Now,
                    ReceiverUserId = LGuid,
                    SenderType = Info.Deploy.Enum.SenderType.System,
                    Title = messages,
                    Content = messages,
                    ReceiverRange = Info.Deploy.Enum.ReceiverRange.SpecifiedUser
                };
                try
                {
                    var retret = Jinher.AMP.BTP.TPS.InfoSV.Instance.AddSystemMessage(messageAdd);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(string.Format("推送消息异常 ,messageAdd:{0}", messageAdd), ex);
                }

            }
        }

        #endregion

        /// <summary>
        /// 计算订单三级分销
        /// </summary>
        /// <returns></returns>
        internal override bool DealDistribute()
        {
            bool isDistribut = false;


            //计算订单、订单项三级分销金额
            if (Order.DistributorId == null || Order.DistributorId == Guid.Empty)
            {
                return isDistribut;
            }
            //若A供应商嵌入了B供应商的商品，通过A供应商的APP或者链接购买了B供应商的分销商品，此时B供应商的商品不参与三级分销。
            if (Order.EsAppId != Order.AppId)
            {
                return isDistribut;
            }

            //商品三级分销设置。
            var commodityIdList = OrderItemList.Select(a => a.CommodityId).Distinct().ToList();
            var cdList = (from cd in CommodityDistribution.ObjectSet()
                          where commodityIdList.Contains(cd.Id)
                          select cd).ToList();

            //订单中商品都没有参加三级分销，不用分销佣金。
            if (!cdList.Any())
            {
                return isDistribut;
            }

            //三级分销功能是否生效。
            if (!Order.EsAppId.HasValue)
            {
                return isDistribut;
            }
            bool b = BACBP.CheckAppDistribute(Order.EsAppId.Value);
            if (!b)
            {
                return isDistribut;
            }
            isDistribut = true;

            var distrib = (from distr in Distributor.ObjectSet()
                           where distr.Id == Order.DistributorId
                           select new { distr.Key, distr.EsAppId }).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(distrib.Key))
            {
                return isDistribut;
            }
            //下单用户的上三级。
            List<string> didQuery = distrib.Key.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Reverse().Take(3).ToList();
            if (!didQuery.Any())
            {
                return isDistribut;
            }
            List<Guid> didList = didQuery.ConvertAll<Guid>(d => new Guid(d));
            if (!didList.Any())
            {
                return isDistribut;
            }

            //分销商关系错乱（A电商馆的分销商分销了B馆的商品）
            if (Order.EsAppId != distrib.EsAppId)
            {
                return isDistribut;
            }

            var higherLevelDistributor = (from distr in Distributor.ObjectSet()
                                          where didList.Contains(distr.Id)
                                          orderby distr.Level descending
                                          select new { Id = distr.Id, UserId = distr.UserId }).ToList();

            decimal? couponRPTotal = 0;
            decimal cAmount = 0;
            decimal sAmount = 0;
            //订单支付详情列表。
            var orderPayDetailsList = OrderPayDetail.ObjectSet().Where(c => c.OrderId == Order.Id).ToList();
            var couponModel = orderPayDetailsList.Where(t => t.ObjectType == 1).FirstOrDefault();
            var integrationAmount = orderPayDetailsList.Where(t => t.ObjectType == 2).Select(t => t.Amount).FirstOrDefault();

            //订单实际支付总价
            decimal? priceSum = OrderItemList.Sum(oi => oi.RealPrice);

            //三级分销在app中的全局设置。
            var appDistribution = (from ae in AppExtension.ObjectSet()
                                   where ae.Id == Order.AppId
                                   select new { ae.DistributeL1Percent, ae.DistributeL2Percent, ae.DistributeL3Percent }).FirstOrDefault();

            List<OrderItemShare> oisList = new List<OrderItemShare>();

            foreach (OrderItem oi in OrderItemList)
            {
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
                            sc = (oi.RealPrice.Value -
                    (oi.RealPrice.Value / Order.Price) *
                    couponModel.Amount -
                    (oi.RealPrice.Value / Order.Price) *
                    integrationAmount) * sharePercent.Value;

                        }
                        else if (couponModel.CouponType == 1)
                        {
                            //当优惠券为商品优惠券时
                            var couponCommodityAccount =
                                OrderItemList.Where(
                                    t => couponModel.CommodityIds.Contains(t.CommodityId.ToString()))
                                             .Sum(t => t.RealPrice.Value);
                            if (couponCommodityAccount > 0)
                            {
                                if (couponModel.CommodityIds.Contains(oi.CommodityId.ToString()))
                                {
                                    sc = (oi.RealPrice.Value -
                                                (oi.RealPrice.Value /
                                                 couponCommodityAccount) *
                                                couponModel.Amount -
                                                (oi.RealPrice.Value / Order.Price) *
                                                integrationAmount) * sharePercent.Value;
                                }
                                else
                                {
                                    sc = (oi.RealPrice.Value -
                                                (oi.RealPrice.Value / Order.Price) *
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
                        sc = (oi.RealPrice.Value -
                    (oi.RealPrice.Value / Order.Price) *
                    integrationAmount) * sharePercent.Value;
                    }

                    OrderItemShare oiShare = OrderItemShare.CreateOrderItemShare();
                    oiShare.Id = Guid.NewGuid();
                    oiShare.SubTime = DateTime.Now;
                    oiShare.ModifiedOn = DateTime.Now;
                    oiShare.SubId = Operator;
                    oiShare.SharePrice = oi.RealPrice.HasValue ? oi.RealPrice.Value : 0;
                    oiShare.Commission = sc;
                    oiShare.SharePercent = sharePercent.HasValue ? sharePercent.Value : 0;
                    oiShare.OrderId = oi.CommodityOrderId;
                    oiShare.OrderItemId = oi.Id;
                    oiShare.PayeeType = payeeType;
                    oiShare.PayeeId = hld.UserId;
                    oiShare.ShareKey = hld.Id.ToString();

                    oisList.Add(oiShare);
                    ContextSession.SaveObject(oiShare);

                    payeeType++;
                }
            }

            if (!oisList.Any())
            {
                return isDistribut;
            }

            int payeeType1 = 9;
            foreach (var hld in higherLevelDistributor)
            {
                decimal sharePriceTotal = oisList.Where(ois => ois.PayeeId == hld.UserId).Select(ois => ois.SharePrice).Sum();
                decimal commissionTotal = oisList.Where(ois => ois.PayeeId == hld.UserId).Select(ois => ois.Commission).Sum();

                OrderShare os = OrderShare.CreateOrderShare();
                os.Id = Guid.NewGuid();
                os.SubTime = DateTime.Now;
                os.ModifiedOn = DateTime.Now;
                os.SubId = Operator;
                os.SharePrice = sharePriceTotal;
                os.Commission = commissionTotal;
                os.SharePercent = 0;
                os.OrderId = Order.Id;
                os.PayeeType = payeeType1;
                os.PayeeId = hld.UserId;
                os.ShareKey = hld.Id.ToString();

                payeeType1++;
            }

            //订单项记录明细，订单记录总分成金额，但不用记录是谁分成
            //分销佣金
            Order.DistributeMoney = oisList.Select(ois => ois.Commission).Sum();
            return true;
        }

        internal override bool DealChannel()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 好运来业务处理
        /// </summary>
        internal override bool DealHyl()
        {
            bool result = false;
            if (Order.SrcType == 1 && Order.SrcTagId != null && Order.SrcTagId != Guid.Empty && CommodityList.Count > 0)//好运来
            {
                result = true;
                if (Order.SrcType == 1 && Order.SrcTagId != null && Order.SrcTagId != Guid.Empty)//好运来
                {
                    GameSV.Instance.UpdateWinPlayerBuyed(Order.SrcTagId.Value, Order.UserId);
                }

                Guid firstComId = CommodityList[0].Id;
                GenUserPrizeRecord prizeResultRecord = (from e in GenUserPrizeRecord.ObjectSet()
                                                        where e.CommodityId == firstComId && e.UserId == Operator && e.PromotionId == Order.SrcTagId
                                                        select e).FirstOrDefault();

                if (prizeResultRecord == null)
                {
                    LogHelper.Error(string.Format("PayUpdateCommodityOrderSV:_orderId:{0},您没有权限购买此商品", Order.Id));
                }
                if (prizeResultRecord.IsBuyed)
                {
                    LogHelper.Error(string.Format("PayUpdateCommodityOrderSV:_orderId:{0},您已经购买过此商品，现在没有权限购买此商品", Order.Id));
                }
                if (prizeResultRecord.Price != Order.Price)
                {
                    LogHelper.Error(string.Format("PayUpdateCommodityOrderSV:_orderId:{0},您不能以此价格购买此商品", Order.Id));
                }
                prizeResultRecord.OrderId = Order.Id;
                prizeResultRecord.EntityState = System.Data.EntityState.Modified;
                prizeResultRecord.IsBuyed = true;
                ContextSession.SaveObject(prizeResultRecord);
            }
            return result;
        }

        internal override bool DealCrowdfunding()
        {
            throw new NotImplementedException();
        }

        private OrderShareInfoDTO _orderShareInfoDTO;
        private OrderShareInfoDTO getShareInfo()
        {
            if (_orderShareInfoDTO != null)
                return _orderShareInfoDTO;
            _orderShareInfoDTO = new OrderShareInfoDTO();
            string shareId = OrderShareMess.ObjectSet().Where(c => c.OrderId == OrderId).Select(c => c.ShareId).FirstOrDefault();
            if (shareId.IsNullVauleFromWeb())
                return _orderShareInfoDTO;
            _orderShareInfoDTO.ShareId = shareId;

            SNS.Deploy.CustomDTO.ReturnInfoDTO<Guid> shareServiceResult = SNSSV.Instance.GetShareUserId(shareId);

            if (shareServiceResult != null && shareServiceResult.Code == "0")
            {
                _orderShareInfoDTO.ShareUser = shareServiceResult.Content;
                _orderShareInfoDTO.IsShare = true;
            }

            return _orderShareInfoDTO;
        }
    }


}
