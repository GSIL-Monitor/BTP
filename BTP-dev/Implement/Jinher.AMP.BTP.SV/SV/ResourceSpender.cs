using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.TPS;
using Jinher.JAP.PL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.Coupon.Deploy.CustomDTO;
using Jinher.AMP.BTP.TPS.Helper;
using Jinher.AMP.BTP.Deploy.Enum;
using System.Data;
using System.Globalization;
using Jinher.AMP.ZPH.Deploy.CustomDTO;
using Newtonsoft.Json;
using Jinher.AMP.BTP.Common.Extensions;

namespace Jinher.AMP.BTP.SV
{
    /// <summary>
    /// 资源消费
    /// </summary>
    public class ResourceSpender
    {
        #region 私有属性

        private OrderSDTO _orderSDTO;
        private Guid _orderId;
        private string _orderCode;



        /// <summary>
        /// 资源消费结果
        /// </summary>
        private List<ResultDTO> _result = new List<ResultDTO>();

        /// <summary>
        /// 消费资源完成时对外返回的结果
        /// </summary>
        private Dictionary<string, Guid> _returnValues = new Dictionary<string, Guid>();

        private DateTime? _expireTime = null;


        /// <summary>
        /// 金采团购活动相关商品相关信息
        /// </summary>
        private List<ZPH.Deploy.CustomDTO.JCActivityItemsListCDTO> _JCActivityItemsList = new List<ZPH.Deploy.CustomDTO.JCActivityItemsListCDTO>();

        /// <summary>
        /// 活动sku属性集合
        /// </summary>
        private List<ZPH.Deploy.CustomDTO.SkuActivityCDTO> _skuActivityList = new List<ZPH.Deploy.CustomDTO.SkuActivityCDTO>();
        /// <summary>
        /// 预售信息
        /// </summary>
        private List<PresellComdtyInfoCDTO> _presellInfoList = new List<PresellComdtyInfoCDTO>();

        /// <summary>
        /// 消费成功的资源列表
        /// </summary>
        private List<ResourceTypeEnum> _consumeSuccessResources = new List<ResourceTypeEnum>();

        #endregion

        #region 共有属性

        /// <summary>
        /// 消费资源完成时对外返回的结果
        /// </summary>
        public Dictionary<string, Guid> ReturnValues
        {
            get
            {
                return _returnValues;
            }
        }

        /// <summary>
        /// 未付款订单过期时间
        /// </summary>
        public DateTime? ExpireTime
        {
            get
            {
                return _expireTime;
            }

        }

        /// <summary>
        /// 资源消费结果
        /// </summary>
        public List<ResultDTO> Result
        {
            get
            {
                return _result;
            }
        }

        /// <summary>
        /// 金采团购活动相关商品相关信息
        /// </summary>
        public List<ZPH.Deploy.CustomDTO.JCActivityItemsListCDTO> JCActivityItemsList
        {
            get
            {
                return _JCActivityItemsList;
            }
        }

        /// <summary>
        /// 活动sku属性集合
        /// </summary>
        public List<ZPH.Deploy.CustomDTO.SkuActivityCDTO> SkuActivityList
        {
            get
            {
                return _skuActivityList;
            }
        }

        /// <summary>
        /// 预售信息
        /// </summary>
        public List<PresellComdtyInfoCDTO> PresellInfoList
        {
            get
            {
                return _presellInfoList;
            }
        }

        #endregion


        /// <summary>
        /// 开始消费资源
        /// </summary>
        /// <param name="dto"></param>
        public List<ResultDTO> Spend(ResourceSpendParamDTO dto)
        {
            _orderSDTO = dto.OrderInfo;
            _orderId = dto.OrderId;
            _orderCode = dto.OrderCode;

            //并行任务列表
            List<Tuple<Action<object>, object>> actionList = new List<Tuple<Action<object>, object>>();

            //积分
            var ssAction = new Tuple<Action<object>, object>(SpendScore, dto);
            actionList.Add(ssAction);

            //优惠券
            var couponAction = new Tuple<Action<object>, object>(SpendCoupon, dto);
            actionList.Add(couponAction);

            //易捷币、易捷抵用券
            var yjbAction = new Tuple<Action<object>, object>(SpendYJB, dto);
            actionList.Add(yjbAction);

            //拼团
            var dgAction = new Tuple<Action<object>, object>(DealDiyGroup, dto);
            actionList.Add(dgAction);

            //推广主分成
            var usAction = new Tuple<Action<object>, object>(DealUserSpreader, dto);
            actionList.Add(usAction);

            //发票
            var invAction = new Tuple<Action<object>, object>(DealInvoice, dto);
            actionList.Add(invAction);

            //订单分享
            var osAction = new Tuple<Action<object>, object>(DealOrderShare, dto);
            actionList.Add(osAction);

            //促销、限购
            var promAction = new Tuple<Action<object>, object>(DealPromotion, dto);
            actionList.Add(promAction);

            //未支付订单超时
            var uooAction = new Tuple<Action<object>, object>(DealUnpaidOrderOvertime, dto);
            actionList.Add(uooAction);


            //餐饮配送优惠
            var dfdAction = new Tuple<Action<object>, object>(DealDeliveryFeeDiscount, dto);
            actionList.Add(dfdAction);

            //获取金采支付客户信息
            var jczfcAction = new Tuple<Action<object>, object>(PreGetJczfCustomerInfo, dto);
            actionList.Add(jczfcAction);

            //金采团购活动相关商品相关信息
            var ilaAction = new Tuple<Action<object>, object>(PreGetItemsListByActivityId, dto);
            actionList.Add(ilaAction);

            //活动sku属性集合
            var skualAction = new Tuple<Action<object>, object>(PreGetSkuActivityListBatch, dto);
            actionList.Add(skualAction);

            //预售信息
            var piiAction = new Tuple<Action<object>, object>(PreGetPresellInfoByIds, dto);
            actionList.Add(piiAction);

            //批量消费易捷卡。
            var cyjAction = new Tuple<Action<object>, object>(ConsumeYjcBatch, dto);
            actionList.Add(cyjAction);


            MultiWorkTask mwt = new MultiWorkTask();
            mwt.Start(actionList);

            if (_result.Any())
            {
                string rStr = JsonConvert.SerializeObject(_result);
                string x = "ResourceSpender.Spend 订单id:{0},订单号：{1},结果：{2}";
                x = string.Format(x, _orderId, _orderCode, rStr);
                LogHelper.Debug(x);
            }

            return _result;
        }





        #region 资源消费和任务处理方法

        /// <summary>
        /// 消费积分
        /// </summary>
        /// <param name="state">使用多少积分抵用订单金额</param>
        /// <returns></returns>
        private void SpendScore(object state)
        {
            ResultDTO result = new ResultDTO();
            if (_orderSDTO.ScorePrice <= 0)
            {
                return;
            }

            ResourceSpendParamDTO dto = (ResourceSpendParamDTO)state;
            int score = dto.Score;

            var spendResult = SignSV.Instance.SpendScore(_orderSDTO.UserId, _orderSDTO.EsAppId, score, _orderId, _orderCode);
            if (!spendResult)
            {
                result.ResultCode = (int)ReturnCodeEnum.SpendScoreFail;
                result.Message = ReturnCodeEnum.SpendScoreFail.GetDescription();
                _result.Add(result);
                return;
            }

            //绑定订单与积分的数据
            OrderPayDetail orderPayDetail = new OrderPayDetail();
            orderPayDetail.Id = Guid.NewGuid();
            orderPayDetail.OrderId = _orderId;
            orderPayDetail.CommodityId = Guid.Empty;
            orderPayDetail.ObjectType = 2;
            orderPayDetail.Amount = _orderSDTO.ScorePrice;
            orderPayDetail.ObjectId = Guid.Empty;
            orderPayDetail.UseType = 0;
            orderPayDetail.EntityState = System.Data.EntityState.Added;
            ContextFactory.CurrentThreadContext.SaveObject(orderPayDetail);
            ContextFactory.CurrentThreadContext.SaveChanges();
            _consumeSuccessResources.Add(ResourceTypeEnum.Score);
        }

        /// <summary>
        /// 消费优惠券
        /// </summary>
        /// <param name="state"></param>
        /// <returns>是否成功消费优惠券</returns>
        private void SpendCoupon(object state)
        {
            ResultDTO result = new ResultDTO();
            if (_orderSDTO.CouponId == Guid.Empty && _orderSDTO.StoreCouponShopDivid <= 0)
            {
                //优惠券、跨店满减券在订单中都没有使用到。
                return;
            }

            ResourceSpendParamDTO dto = (ResourceSpendParamDTO)state;
            CouponNewDTO couponNewDTO = dto.CouponInfo;

            //消费优惠券
            var querySpendCoupon = new SpendCouponRequestDTO
            {
                OrderID = _orderId,
                ShopID = _orderSDTO.AppId,
                UserID = _orderSDTO.UserId,
                CouponIds = new List<Guid> { _orderSDTO.CouponId == Guid.Empty ? _orderSDTO.StoreCouponId : _orderSDTO.CouponId },
                EsAppId = _orderSDTO.EsAppId,
                CouponTemplateType = _orderSDTO.CouponId == Guid.Empty ? Coupon.Deploy.Enum.CouponTemplateType.StoreCoupon : Coupon.Deploy.Enum.CouponTemplateType.Coupon
            };
            try
            {
                var couponResult = CouponSV.Instance.SpendCoupon(querySpendCoupon);

                if (couponResult == null)
                {
                    LogHelper.Error(string.Format("CommodityOrderSV.spendCoupon优惠券消费错误。spendParams：{0}，Message：{1}", JsonHelper.JsonSerializer(querySpendCoupon), "结果返回Null"));

                    result.ResultCode = (int)ReturnCodeEnum.SpendCouponFail;
                    result.Message = ReturnCodeEnum.SpendCouponFail.GetDescription();
                    _result.Add(result);
                    return;
                }
                else if (!couponResult.IsSuccess)
                {
                    LogHelper.Error(string.Format("CommodityOrderSV.spendCoupon优惠券消费错误。spendParams：{0}，Message：{1}", JsonHelper.JsonSerializer(querySpendCoupon), couponResult.Info));
                    result.ResultCode = (int)ReturnCodeEnum.SpendCouponFail;
                    result.Message = couponResult.Info;
                    _result.Add(result);
                    return;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("优惠券消费异常。spendParams：{0}", JsonHelper.JsonSerializer(querySpendCoupon)), ex);
                result.ResultCode = (int)ReturnCodeEnum.SpendCouponFail;
                result.Message = ReturnCodeEnum.SpendCouponFail.GetDescription();
                _result.Add(result);
                return;
            }

            StringBuilder commodityIds = new StringBuilder();
            string commId = "";
            if (couponNewDTO.GoodList != null && couponNewDTO.GoodList.Count > 0)
            {
                commId = string.Join(",", couponNewDTO.GoodList);
            }
            //绑定订单与优惠券的数据
            OrderPayDetail orderPayDetail = new OrderPayDetail();
            orderPayDetail.Id = Guid.NewGuid();
            orderPayDetail.OrderId = _orderId;
            orderPayDetail.CommodityId = _orderSDTO.CouponComId != Guid.Empty ? (Guid?)_orderSDTO.CouponComId : null;
            orderPayDetail.ObjectType = 1;
            orderPayDetail.CommodityIds = commId != String.Empty ? commId : null;
            orderPayDetail.CouponType = (int)couponNewDTO.CouponType;
            //   commodityOrderDTO.RealPrice = commodityOrderDTO.RealPrice == null ? 0 : commodityOrderDTO.RealPrice.Value;
            orderPayDetail.Amount = _orderSDTO.StoreCouponShopDivid > 0 ? _orderSDTO.StoreCouponShopDivid : _orderSDTO.CouponValue;
            orderPayDetail.ObjectId = _orderSDTO.StoreCouponShopDivid > 0 ? _orderSDTO.StoreCouponId : _orderSDTO.CouponId;
            orderPayDetail.UseType = (int)couponNewDTO.UseType;
            orderPayDetail.EntityState = System.Data.EntityState.Added;

            ContextFactory.CurrentThreadContext.SaveObject(orderPayDetail);
            ContextFactory.CurrentThreadContext.SaveChanges();
            _consumeSuccessResources.Add(ResourceTypeEnum.Coupon);

        }

        /// <summary>
        /// 消费易捷币、易捷抵用券
        /// </summary>
        /// <param name="state"></param>
        private void SpendYJB(object state)
        {
            if ((_orderSDTO.YJCouponIds == null || _orderSDTO.YJCouponIds.Count <= 0)
                && _orderSDTO.YJBPrice <= 0)
            {
                return;
            }
            ResultDTO result = new ResultDTO();

            IEnumerable<Guid> cids = _orderSDTO.ShoppingCartItemSDTO.Select(s => s.Id);
            var coicList = _orderSDTO.ShoppingCartItemSDTO.Select(s => new
               YJB.Deploy.CustomDTO.CreateOrderInputCommodityDTO
            {
                Id = s.Id,
                AppId = _orderSDTO.AppId,
                OrderItemId = s.OrderItemId,
                Name = s.Name,
                ImgUrl = s.Pic,
                Price = s.RealPrice,
                Number = s.CommodityNumber
            }).ToList();
            var createOrderInput = new YJB.Deploy.CustomDTO.CreateOrderInputDTO
            {
                UserId = _orderSDTO.UserId,
                //UserName = ContextDTO.LoginUserName,
                AppId = _orderSDTO.AppId,
                Commodities = coicList,
                OrderId = _orderId,
                //InsteadCashAmount = orderSDTO.YJBPrice,
                //InsteadCashCount = (int)orderSDTO.YJBCount
            };
            // 消费易捷抵用券
            if (_orderSDTO.YJCouponIds != null && _orderSDTO.YJCouponIds.Count > 0)
            {
                createOrderInput.YJCounponIds = _orderSDTO.YJCouponIds;
                createOrderInput.YJCounponUseAmount = _orderSDTO.YJCouponPrice;
            }
            // 消费易捷币
            if (_orderSDTO.YJBPrice > 0)
            {
                createOrderInput.InsteadCashAmount = _orderSDTO.YJBPrice;
            }

            var yjbResult = YJBHelper.CreateOrder(_orderSDTO.EsAppId, createOrderInput);
            LogHelper.Info("YJBHelper.CreateOrder " + JsonHelper.JsonSerializer(createOrderInput));
            if (!yjbResult.IsSuccess)
            {
                // 消费易捷抵用券
                if (_orderSDTO.YJCouponIds != null && _orderSDTO.YJCouponIds.Count > 0)
                {
                    result.ResultCode = (int)ReturnCodeEnum.SpendYJCouponFail;
                    result.Message = ReturnCodeEnum.SpendYJCouponFail.GetDescription();
                    _result.Add(result);
                }
                else if (_orderSDTO.YJBPrice > 0)
                {
                    //消费易捷币失败，请重新下单
                    result.ResultCode = (int)ReturnCodeEnum.SpendYJBFail;
                    result.Message = ReturnCodeEnum.SpendYJBFail.GetDescription();
                    _result.Add(result);
                }
                return;
            }


            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            //易捷币
            if (_orderSDTO.YJBPrice > 0)
            {
                OrderPayDetail orderPayDetail = new OrderPayDetail();
                orderPayDetail.Id = Guid.NewGuid();
                orderPayDetail.OrderId = _orderId;
                orderPayDetail.CommodityId = null;
                orderPayDetail.ObjectType = 10;
                orderPayDetail.CommodityIds = string.Join(",", cids);
                orderPayDetail.CouponType = 0;
                orderPayDetail.Amount = _orderSDTO.YJBPrice;
                orderPayDetail.ObjectId = Guid.Empty;
                orderPayDetail.UseType = 0;
                orderPayDetail.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(orderPayDetail);
            }

            //易捷抵用券
            if (_orderSDTO.YJCouponIds != null && _orderSDTO.YJCouponIds.Count > 0)
            {
                OrderPayDetail orderPayDetail = new OrderPayDetail();
                orderPayDetail.Id = Guid.NewGuid();
                orderPayDetail.OrderId = _orderId;
                orderPayDetail.CommodityId = null;
                orderPayDetail.ObjectType = 15;
                orderPayDetail.CommodityIds = string.Join(",", cids);
                orderPayDetail.CouponType = 0;
                orderPayDetail.Amount = _orderSDTO.YJCouponPrice;
                orderPayDetail.ObjectIds = string.Join(",", _orderSDTO.YJCouponIds);
                orderPayDetail.ObjectId = Guid.Empty;
                orderPayDetail.UseType = 0;
                orderPayDetail.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(orderPayDetail);
            }
            contextSession.SaveChanges();
            _consumeSuccessResources.Add(ResourceTypeEnum.YJB);
        }

        /// <summary>
        /// 处理拼团任务
        /// </summary>
        /// <param name="state">参数</param>
        private void DealDiyGroup(object state)
        {
            try
            {
                ResourceSpendParamDTO dto = (ResourceSpendParamDTO)state;
                var todayPromotionList = dto.todayPromotionList;
                var contextDTO = dto.ContextDTO;

                OrderSDTO orderSDTO = _orderSDTO;

                if (orderSDTO.PromotionType != 3)
                {
                    return;
                }
                //Guid orderAppId = comList.First().AppId
                Guid orderAppId = orderSDTO.AppId;
                Guid diyGroupId = Guid.Empty;

                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                int role = 0;
                if (orderSDTO.DiyGroupId.HasValue && orderSDTO.DiyGroupId != Guid.Empty)
                {
                    diyGroupId = orderSDTO.DiyGroupId.HasValue ? orderSDTO.DiyGroupId.Value : Guid.Empty;
                    //参团
                    role = 1;
                }
                else
                {


                    DateTime now = DateTime.Now;
                    //创建团
                    DiyGroup diyGroup = DiyGroup.CreateDiyGroup();
                    diyGroup.Code = ((ulong)JAP.Common.Crc64.ComputeAsAsciiGuid(diyGroup.Id)).ToString(CultureInfo.InvariantCulture);
                    diyGroup.SubId = orderSDTO.UserId;
                    diyGroup.AppId = orderAppId;
                    diyGroup.Name = "";
                    diyGroup.CommodityId = todayPromotionList[0].CommodityId;
                    diyGroup.PromotionId = todayPromotionList[0].PromotionId;
                    diyGroup.ExpireTime = now.AddSeconds(todayPromotionList[0].ExpireSecond ?? 0);
                    diyGroup.EsAppId = orderSDTO.EsAppId;
                    diyGroup.EntityState = EntityState.Added;
                    contextSession.SaveObject(diyGroup);

                    orderSDTO.DiyGroupId = diyGroup.Id;
                    role = 0;

                    diyGroupId = diyGroup.Id;
                }

                _returnValues.Add("DiyGroupId", diyGroupId);
                //创建拼团订单
                DiyGroupOrder diyGroupOrder = DiyGroupOrder.CreateDiyGroupOrder();
                diyGroupOrder.SubId = orderSDTO.UserId;
                diyGroupOrder.AppId = orderAppId;
                diyGroupOrder.OrderId = _orderId;
                diyGroupOrder.OrderCode = _orderCode;
                diyGroupOrder.Role = role;
                diyGroupOrder.DiyGroupId = orderSDTO.DiyGroupId.Value;
                diyGroupOrder.SubCode = contextDTO.LoginUserCode;
                //待付款
                diyGroupOrder.State = 0;
                diyGroupOrder.EntityState = EntityState.Added;
                contextSession.SaveObject(diyGroupOrder);

                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                ResultDTO result = new ResultDTO();
                result.ResultCode = (int)ReturnCodeEnum.JoinOrCreateDiyGroupException;
                result.Message = ReturnCodeEnum.JoinOrCreateDiyGroupException.GetDescription();
                _result.Add(result);

                LogHelper.Error("ResourceSpender.DealDiyGroup异常，异常信息：", ex);
            }
        }

        /// <summary>
        /// 处理推广主分成
        /// </summary>
        /// <param name="state"></param>
        private void DealUserSpreader(object state)
        {
            OrderSDTO orderSDTO = _orderSDTO;
            try
            {
                ResourceSpendParamDTO dto = (ResourceSpendParamDTO)state;



                Guid spreaderId = Guid.Empty;

                //客户已被推广，则取得其推广者的ID；否则若是第一次推广，则根据推广码获取推广者ID；
                var tempUserSpreader = (from userSpreader in UserSpreader.ObjectSet()
                                        join spreadInfo in SpreadInfo.ObjectSet() on userSpreader.SpreaderId equals spreadInfo.SpreadId
                                        where !userSpreader.IsDel && spreadInfo.IsDel == 0
                                        && userSpreader.UserId == orderSDTO.UserId
                                        select userSpreader).FirstOrDefault();
                if (tempUserSpreader != null)
                {
                    spreaderId = tempUserSpreader.SpreaderId;
                }
                else
                {
                    if (orderSDTO.SpreadCode.HasValue && orderSDTO.SpreadCode.Value != Guid.Empty)
                    {

                        var spreadinfo = SpreadInfo.ObjectSet().FirstOrDefault(c => c.IsDel == 0 && c.SpreadCode == orderSDTO.SpreadCode.Value);
                        if (spreadinfo != null)
                        {
                            spreaderId = spreadinfo.SpreadId;

                            ContextSession contextSession = ContextFactory.CurrentThreadContext;
                            UserSpreader userSpreader = UserSpreader.CreateUserSpreader();
                            userSpreader.UserId = orderSDTO.UserId;
                            userSpreader.SpreaderId = spreadinfo.SpreadId;
                            userSpreader.SpreadCode = orderSDTO.SpreadCode.Value;
                            userSpreader.IsDel = false;
                            userSpreader.CreateOrderId = _orderId;
                            contextSession.SaveObject(userSpreader);
                            contextSession.SaveChanges();
                        }

                    }
                }
                _returnValues.Add("spreaderId", spreaderId);

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("根据推广码获取推广者ID异常。spreadCode：{0}，Ex：{1}", orderSDTO.SpreadCode, ex));
            }
        }

        /// <summary>
        /// 处理发票
        /// </summary>
        /// <param name="state"></param>
        private void DealInvoice(object state)
        {
            OrderSDTO orderSDTO = _orderSDTO;
            ResourceSpendParamDTO dto = (ResourceSpendParamDTO)state;


            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                LogHelper.Debug("获取读取金采支付的发票数据，orderSDTO.JcActivityId：" + orderSDTO.JcActivityId);
                if (orderSDTO.JcActivityId != null && orderSDTO.JcActivityId != Guid.Empty)
                {
                    //金采支付订单 读取自己的发票数据
                    var returnInfo = TPS.ZPHSV.Instance.GetJCInvoiceByActivityId(orderSDTO.JcActivityId);
                    LogHelper.Debug("获取读取金采支付的发票数据，returnInfo：" + JsonHelper.JsSerializer(returnInfo));
                    if (!returnInfo.isSuccess)
                    {
                        return;
                    }
                    Invoice invoice = Invoice.CreateInvoice();
                    invoice.CommodityOrderId = _orderId;
                    invoice.InvoiceContent = "明细";
                    if (returnInfo.Data.Head == 0)
                    {
                        invoice.InvoiceType = 1;
                    }
                    else if (returnInfo.Data.Head == 1)
                    {
                        invoice.InvoiceType = 2;
                    }

                    //invoice.Category 1增值税普通发票 2电子发票 4增值税专用发票
                    if (returnInfo.Data.Type == 0)
                    {
                        invoice.Category = 2;
                    }
                    else if (returnInfo.Data.Type == 1)
                    {
                        invoice.Category = 4;
                    }
                    else if (returnInfo.Data.Type == 2)
                    {
                        invoice.Category = 1;
                    }
                    invoice.SubId = orderSDTO.UserId;
                    invoice.Code = returnInfo.Data.IdentifyNumber;
                    invoice.SubTime = DateTime.Now;
                    invoice.ModifiedOn = invoice.SubTime;
                    invoice.ReceiptPhone = returnInfo.Data.AcceptUserTel;
                    invoice.ReceiptEmail = returnInfo.Data.AcceptUserMail;
                    invoice.State = 0;
                    invoice.InvoiceTitle = returnInfo.Data.HeadValue;

                    invoice.Remark = "";
                    contextSession.SaveObject(invoice);

                    InvoiceJounal ij = GetInvoiceJounal(invoice.Id, orderSDTO.UserId, 0, 0);
                    contextSession.SaveObject(ij);

                }
                else
                {
                    if (orderSDTO.InvoiceInfo != null && orderSDTO.InvoiceInfo.InvoiceType != 0)
                    {
                        Deploy.InvoiceDTO invDto = orderSDTO.InvoiceInfo;
                        Invoice invoice = Invoice.CreateInvoice();
                        invoice = invoice.FromEntityData(invDto);
                        invoice.Id = Guid.NewGuid();
                        invoice.EntityState = System.Data.EntityState.Added;
                        invoice.SubTime = DateTime.Now;
                        invoice.ModifiedOn = invoice.SubTime;
                        invoice.State = 0;
                        invoice.CommodityOrderId = _orderId;
                        invoice.Code = orderSDTO.InvoiceInfo.Code;
                        contextSession.SaveObject(invoice);

                        InvoiceJounal ij = GetInvoiceJounal(invoice.Id, orderSDTO.UserId, 0, 0);
                        contextSession.SaveObject(ij);
                    }
                }

                contextSession.SaveChanges();

            }
            catch (Exception ex)
            {
                //todo 发票处理异常
                LogHelper.Error("ResourceSpender.DealInvoice异常，异常信息：", ex);
            }
        }


        /// <summary>
        /// 处理订单分享
        /// </summary>
        /// <param name="state"></param>
        private void DealOrderShare(object state)
        {
            OrderSDTO orderSDTO = _orderSDTO;

            if (orderSDTO.SrcType != 33 && orderSDTO.SrcType != 34)
            {
                return;
            }
            if (string.IsNullOrEmpty(orderSDTO.ShareId) || orderSDTO.ShareId == "undefined" ||
                orderSDTO.ShareId == "null")
            {
                return;
            }
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            OrderShareMess orderShareMessDTO = new OrderShareMess
            {
                OrderId = _orderId,
                Id = Guid.NewGuid(),
                ShareId = orderSDTO.ShareId,
                EntityState = System.Data.EntityState.Added
            };
            contextSession.SaveObject(orderShareMessDTO);
            contextSession.SaveChanges();
        }

        /// <summary>
        /// 促销活动、限购信息更新
        /// </summary>
        /// <param name="state"></param>
        private void DealPromotion(object state)
        {
            OrderSDTO orderSDTO = _orderSDTO;
            ResourceSpendParamDTO dto = (ResourceSpendParamDTO)state;
            Dictionary<Guid, TodayPromotion> comproDict = dto.ComproDict;
            List<Guid> proCommodityIdList = dto.proCommodityIdList;
            Dictionary<Guid, int> comNumDict = dto.ComNumDict;

            try
            {
                if (comproDict.Count <= 0)
                {
                    return;
                }

                #region 活动资源校验和消费

                //活动、商品、销量
                var proComTuples = new List<Tuple<string, string, int>>();

                foreach (var compro in comproDict)
                {
                    proComTuples.Add(new Tuple<string, string, int>(compro.Value.PromotionId.ToString(), compro.Key.ToString(), comNumDict[compro.Key]));
                }
                //RedisHelper.ListHIncr实际消费活动资源的方法
                var proComBuyTuples = RedisHelper.ListHIncr(proComTuples, orderSDTO.UserId);

                if (proComBuyTuples == null || !proComBuyTuples.Any() || proComBuyTuples.Count != proComTuples.Count)
                {
                    AddMessage(ReturnCodeEnum.GetPromotionDataError);
                    return;

                }
                foreach (var compro in comproDict)
                {
                    var tuple = proComBuyTuples.First(c => c.Item2 == compro.Key.ToString() && c.Item1 == compro.Value.PromotionId.ToString());
                    ////获得真实资源：提交该订单后的卖出数量-本单该商品数量
                    compro.Value.SurplusLimitBuyTotal = Convert.ToInt32(tuple.Item3);
                    if (compro.Value.LimitBuyTotal > 0 && compro.Value.LimitBuyTotal - Convert.ToInt32(tuple.Item3) < 0)
                    {
                        rollbackPromotionCacheOnly(proComTuples, orderSDTO.UserId);
                        AddMessage(ReturnCodeEnum.QiangGuang);
                        return;
                    }
                }

                if (!checkPromotion(orderSDTO.UserId, comproDict, comNumDict))
                {
                    rollbackPromotionCacheOnly(proComTuples, orderSDTO.UserId);
                    return;
                }

                #endregion



                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                var promotionIdList = comproDict.Values.Select(c => c.PromotionId).Distinct().ToList();
                var promotionItemsList = PromotionItems.ObjectSet()
                            .Where(
                                t => promotionIdList.Contains(t.PromotionId) &&
                                proCommodityIdList.Contains(t.CommodityId))
                            .ToList();

                foreach (var promotion in comproDict)
                {
                    Guid comId = promotion.Key;
                    var todayp = promotion.Value;

                    //获得真实资源：提交该订单后的卖出数量-本单该商品数量
                    var tuple = proComBuyTuples.First(c => c.Item2 == promotion.Key.ToString() && c.Item1 == todayp.PromotionId.ToString());
                    todayp.SurplusLimitBuyTotal = Convert.ToInt32(tuple.Item3);
                    todayp.EntityState = System.Data.EntityState.Modified;

                    PromotionItems pro = (from pr in promotionItemsList
                                          where pr.PromotionId == todayp.PromotionId && pr.CommodityId == todayp.CommodityId
                                          select pr).FirstOrDefault();
                    pro.EntityState = System.Data.EntityState.Modified;
                    pro.SurplusLimitBuyTotal = todayp.SurplusLimitBuyTotal;
                    UserLimited ul = new UserLimited
                    {
                        Id = Guid.NewGuid(),
                        UserId = orderSDTO.UserId,
                        PromotionId = todayp.PromotionId,
                        CommodityId = todayp.CommodityId,
                        Count = comNumDict[comId],
                        CreateTime = DateTime.Now,
                        CommodityOrderId = _orderId,
                        EntityState = System.Data.EntityState.Added
                    };
                    contextSession.SaveObject(ul);
                }
                contextSession.SaveChanges();
                _consumeSuccessResources.Add(ResourceTypeEnum.Promotion);
            }
            catch (Exception ex)
            {
                LogHelper.Error("ResourceSpender.DealPromotion异常，异常信息：", ex);
            }
        }

        /// <summary>
        /// 处理未支付订单超时
        /// </summary>
        /// <param name="state"></param>
        private void DealUnpaidOrderOvertime(object state)
        {
            try
            {
                OrderSDTO orderSDTO = _orderSDTO;
                ResourceSpendParamDTO dto = (ResourceSpendParamDTO)state;
                Dictionary<Guid, TodayPromotion> comproDict = dto.ComproDict;
                List<Guid> proCommodityIdList = dto.proCommodityIdList;
                Dictionary<Guid, int> comNumDict = dto.ComNumDict;

                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                long expireSeconds = 0;
                bool needSaveExpirePay = false;
                Guid? promotionId = null;
                var firstOutPrmotion = comproDict.Values.FirstOrDefault(c => c.PromotionType != 0 && c.PromotionType != 3);
                //秒杀、预售订单单独设置超时时间
                if (firstOutPrmotion != null)
                {

                    expireSeconds = PromotionSV.GetExpirePaySeconds(orderSDTO.EsAppId);
                    needSaveExpirePay = true;
                    promotionId = firstOutPrmotion.PromotionId;
                }
                else if (orderSDTO.SelfTakeFlag == 1) //自提订单30分钟未支付，超时
                {
                    expireSeconds = CustomConfig.SelfTakeOrderExpirePaySecond;
                    needSaveExpirePay = true;
                }
                if (needSaveExpirePay)
                {
                    OrderExpirePay orderExpirePay = OrderExpirePay.CreateOrderExpirePay();
                    orderExpirePay.OrderId = _orderId;
                    orderExpirePay.PromotionId = promotionId;
                    orderExpirePay.State = 0;
                    orderExpirePay.ExpirePayTime = DateTime.Now.AddSeconds(expireSeconds);
                    contextSession.SaveObject(orderExpirePay);

                    _expireTime = orderExpirePay.ExpirePayTime;
                }
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error("ResourceSpender.DealUnpaidOrderOvertime异常，异常信息：", ex);
            }
        }

        /// <summary>
        /// 餐饮配送优惠
        /// </summary>
        /// <param name="state"></param>
        private void DealDeliveryFeeDiscount(object state)
        {
            try
            {

                OrderSDTO orderSDTO = _orderSDTO;
                ResourceSpendParamDTO dto = (ResourceSpendParamDTO)state;

                if (orderSDTO.OrderType != 2 || orderSDTO.deliveryFeeDiscount <= 0)
                {
                    return;
                }
                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                OrderPayDetail orderPayDetail = new OrderPayDetail();
                orderPayDetail.Id = Guid.NewGuid();
                orderPayDetail.OrderId = _orderId;
                orderPayDetail.CommodityId = Guid.Empty;
                orderPayDetail.ObjectType = 3;
                orderPayDetail.Amount = (decimal)orderSDTO.deliveryFeeDiscount;

                if (orderPayDetail.Amount < 0)
                    orderPayDetail.Amount = orderPayDetail.Amount * -1;

                orderPayDetail.ObjectId = Guid.Empty;
                orderPayDetail.UseType = 0;
                orderPayDetail.EntityState = System.Data.EntityState.Added;

               
                contextSession.SaveObject(orderPayDetail);
                contextSession.SaveChanges();

            }
            catch (Exception ex)
            {
                LogHelper.Error("ResourceSpender.DealDeliveryFeeDiscount异常，异常信息：", ex);
            }
        }

        /// <summary>
        /// 获取金采支付客户信息
        /// </summary>
        /// <param name="state"></param>
        private void PreGetJczfCustomerInfo(object state)
        {
            try
            {
                OrderSDTO orderSDTO = _orderSDTO;
                if (orderSDTO.JcActivityId == null || orderSDTO.JcActivityId == Guid.Empty)
                {
                    return;
                }
                Jinher.AMP.ZPH.ISV.Facade.FJCPayFacade SD = new Jinher.AMP.ZPH.ISV.Facade.FJCPayFacade();
                var JCZFRes = SD.GetJcCustomerByActivityId(orderSDTO.JcActivityId);
                if (JCZFRes != null)
                {
                    _returnValues.Add("JczfCustomerId", JCZFRes.Data.id);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("ResourceSpender.GetJczfCustomerInfo异常，异常信息：", ex);
            }
        }


        /// <summary>
        /// 根据金采团购活动Id获取商品相关信息
        /// </summary>
        /// <param name="state"></param>
        private void PreGetItemsListByActivityId(object state)
        {
            try
            {
                OrderSDTO orderSDTO = _orderSDTO;
                if (!orderSDTO.IsJcActivity
                    || orderSDTO.JcActivityId == null || orderSDTO.JcActivityId == Guid.Empty)
                {
                    return;
                }
                var jcPromotion = ZPHSV.Instance.GetItemsListByActivityId(orderSDTO.JcActivityId);
                _JCActivityItemsList = new List<ZPH.Deploy.CustomDTO.JCActivityItemsListCDTO>();
                if (jcPromotion.Code == 0
                    && jcPromotion.Data != null && jcPromotion.Data.Any())
                {
                    _JCActivityItemsList = jcPromotion.Data;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("ResourceSpender.GetJczfCustomerInfo异常，异常信息：", ex);
            }
        }



        /// <summary>
        /// 预加载活动sku属性集合
        /// </summary>
        /// <param name="state"></param>
        private void PreGetSkuActivityListBatch(object state)
        {
            try
            {
                OrderSDTO orderSDTO = _orderSDTO;
                ResourceSpendParamDTO dto = (ResourceSpendParamDTO)state;

                if (!orderSDTO.IsJcActivity
                    || orderSDTO.JcActivityId == null || orderSDTO.JcActivityId == Guid.Empty)
                {
                    return;
                }
                IEnumerable<Guid> activityIds = dto.todayPromotionList.Select(x => x.OutsideId.Value).Distinct();
                if (activityIds == null || !activityIds.Any())
                {
                    return;
                }
                _skuActivityList = new List<ZPH.Deploy.CustomDTO.SkuActivityCDTO>();
                var jcPromotion = ZPHSV.Instance.GetSkuActivityListBatch(activityIds.ToList());
                if (jcPromotion != null && jcPromotion.Any())
                {
                    _skuActivityList = jcPromotion;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("ResourceSpender.PreGetSkuActivityListBatch异常，异常信息：", ex);
            }
        }


        /// <summary>
        /// 预加载预售信息
        /// </summary>
        /// <param name="state"></param>
        private void PreGetPresellInfoByIds(object state)
        {
            try
            {
                OrderSDTO orderSDTO = _orderSDTO;
                ResourceSpendParamDTO dto = (ResourceSpendParamDTO)state;

                if (!orderSDTO.IsJcActivity
                    || orderSDTO.JcActivityId == null || orderSDTO.JcActivityId == Guid.Empty)
                {
                    return;
                }
                IEnumerable<Guid> activityIds = dto.todayPromotionList.Select(x => x.OutsideId.Value).Distinct();
                if (activityIds == null || !activityIds.Any())
                {
                    return;
                }
                _presellInfoList = new List<PresellComdtyInfoCDTO>();
                var presellList = ZPHSV.Instance.GetPresellInfoByIds(activityIds.ToList());
                if (presellList != null && presellList.Any())
                {
                    _presellInfoList = presellList;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("ResourceSpender.PreGetSkuActivityListBatch异常，异常信息：", ex);
            }
        }

        /// <summary>
        /// 消费易捷卡
        /// </summary>
        private void ConsumeYjcBatch(object state)
        {
            OrderSDTO orderSDTO = _orderSDTO;
            if (orderSDTO.YjCardPrice <= 0 || orderSDTO.YjCards == null || !orderSDTO.YjCards.Any())
            {
                return;
            }
            //todo 调用易捷卡接口消费易捷卡。
            YJB.Deploy.CustomDTO.ConsumeYjcBatchParamDTO cybp = new YJB.Deploy.CustomDTO.ConsumeYjcBatchParamDTO();
            cybp.OrderId = _orderId;
            cybp.UserId = orderSDTO.UserId;
            cybp.PayItems = orderSDTO.YjCards.ConvertAll(x => (YJB.Deploy.CustomDTO.PayItem)new YJB.Deploy.CustomDTO.PayItem().FillWith(x));
            YJB.Deploy.CustomDTO.ResultDTO yjcResult = YJBSV.ConsumeYjcBatch(cybp);


            if (yjcResult.Code != "0")
            {
                int xcode = -100;
                int.TryParse(yjcResult.Code, out xcode);
                AddMessage(xcode, yjcResult.Message);
            }
        }


        #endregion

        #region 其他私有方法

        /// <summary>
        /// 添加错误消息
        /// </summary>
        /// <param name="rc"></param>
        private void AddMessage(ReturnCodeEnum rc)
        {
            int code = (int)rc;
            string message = rc.GetDescription();
            AddMessage(code, message);
        }

        /// <summary>
        /// 添加错误消息
        /// </summary>
        /// <param name="code">消息编码</param>
        /// <param name="message">消息内容</param>
        private void AddMessage(int code, string message)
        {
            ResultDTO result = new ResultDTO();
            result.ResultCode = code;
            result.Message = message;
            _result.Add(result);
        }

        /// <summary>
        /// 添加错误消息
        /// </summary>
        /// <param name="result"></param>
        private void AddMessage(ResultDTO result)
        {
            _result.Add(result);
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
            return ij;
        }




        /// <summary>
        /// 校验是否可以参加活动
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="comproDict">活动列表</param>
        /// <param name="comNumDict">订单中商品汇总</param>
        /// <param name="orderResultDTO">返回值，如果校验成功返回null</param>
        /// <returns></returns>
        private bool checkPromotion(Guid userId, Dictionary<Guid, TodayPromotion> comproDict, Dictionary<Guid, int> comNumDict)
        {
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
                        string msg = string.Format(ReturnCodeEnum.CommodityBuyLimit.GetDescription(), t.LimitBuyEach);
                        AddMessage((int)ReturnCodeEnum.CommodityBuyLimit, msg);
                        return false;
                    }
                }
                if (t.LimitBuyTotal != -1)
                {
                    if (t.SurplusLimitBuyTotal > t.LimitBuyTotal)
                    {
                        AddMessage(ReturnCodeEnum.QiangGuang);
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

        /// <summary>
        /// 回滚缓存中的活动资源
        /// </summary>
        /// <param name="proComTuples"></param>
        /// <param name="userId"></param>
        private void rollbackPromotionCacheOnly(List<Tuple<string, string, int>> proComTuples, Guid userId)
        {
            if (proComTuples == null || !proComTuples.Any())
            {
                return;
            }
            List<Tuple<string, string, int>> backProComTuples = proComTuples.Select(proComTuple => new Tuple<string, string, int>(proComTuple.Item1, proComTuple.Item2, -proComTuple.Item3)).ToList();
            var result = RedisHelper.ListHIncr(backProComTuples, userId);
        }


        /// <summary>
        /// 验证易捷卡余额
        /// </summary>
        /// <param name="yjcList"></param>
        public ResultDTO ValidYjCardBalance(List<PayItem> yjcList)
        {
            ResultDTO result = new ResultDTO();
            //查询所有卡片的余额，和现有余额对比，若卡的实际余额小于向用户展示的余额（充值类卡片也可能大于,但不影响支付），则不够支付当前订单，需要重新选择。
            //todo 掉YJB接口查询易捷卡余额
            YJBFacade YJBFacade = new YJBFacade();
            var list = new List<YJB.Deploy.CustomDTO.PayItem>();
            yjcList.ForEach(a =>
            {
                list.Add(new YJB.Deploy.CustomDTO.PayItem { Id = a.Id, Amount = a.Amount, Balance = a.Balance });
            });
            var piResult = YJBFacade.YJCBalanceBatch(list);
            foreach (var pi in yjcList)
            {
                var piTarget = piResult.Data.FirstOrDefault(x => x.cardNo == pi.CardNo);
                if (piTarget == null)
                {
                    //todo易捷卡服务异常，请稍后重试！
                    result.ResultCode = 1;
                    result.Message = "易捷卡服务异常，请稍后重试！";
                    LogHelper.Error("易捷卡服务异常");
                    return result;
                }
                if (Jinher.AMP.BTP.Common.CommonUtil.ConvertToDecimal(piTarget.balance) < pi.Balance)
                {
                    //todo易捷卡余额发生变化，请重新下单。
                    result.ResultCode = 1;
                    result.Message = "易捷卡余额发生变化，请重新下单！";
                    LogHelper.Error("易捷卡余额发生变化");
                    return result;
                }
            }
            return result;
        }



        #endregion
    }

    /// <summary>
    /// 资源类型
    /// </summary>
    public enum ResourceTypeEnum : byte
    {
        /// <summary>
        /// 
        /// </summary>
        None = 0,

        /// <summary>
        /// 积分
        /// </summary>
        Score = 10,

        /// <summary>
        /// 优惠券
        /// </summary>
        Coupon = 20,

        /// <summary>
        /// 活动资源
        /// </summary>
        Promotion = 30,

        /// <summary>
        /// 易捷币、易捷抵用券
        /// </summary>
        YJB = 40
    }


    /// <summary>
    /// 资源消费参数
    /// </summary>
    public class ResourceParamDTO
    {

        /// <summary>
        /// 订单信息
        /// </summary>
        public OrderSDTO OrderInfo { get; set; }

        /// <summary>
        /// 订单id
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// 订单编码
        /// </summary>
        public string OrderCode { get; set; }


        /// <summary>
        /// 商品活动列表
        /// </summary>
        public Dictionary<Guid, TodayPromotion> ComproDict { get; set; }


        /// <summary>
        /// 商品数量
        /// </summary>
        public Dictionary<Guid, int> ComNumDict { get; set; }



    }

    /// <summary>
    /// 资源消费参数
    /// </summary>
    public class ResourceSpendParamDTO : ResourceParamDTO
    {
        /// <summary>
        /// 积分
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// 优惠券信息
        /// </summary>
        public CouponNewDTO CouponInfo { get; set; }

        /// <summary>
        /// 今日活动
        /// </summary>
        public List<TodayPromotion> todayPromotionList { get; set; }

        /// <summary>
        /// 当前登录用户上下文
        /// </summary>
        public JAP.BF.BE.Deploy.Base.ContextDTO ContextDTO { get; set; }

        /// <summary>
        /// 参加活动的商品id列表
        /// </summary>
        public List<Guid> proCommodityIdList { get; set; }



    }
}
