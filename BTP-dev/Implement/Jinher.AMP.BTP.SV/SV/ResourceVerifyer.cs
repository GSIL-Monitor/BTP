using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.BTP.TPS.Helper;
using Jinher.JAP.Common.Loging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common.Extensions;

namespace Jinher.AMP.BTP.SV
{
    /// <summary>
    /// 下订单资源校验
    /// </summary>
    public class ResourceVerifyer
    {
        #region 私有字段

        private OrderSDTO _orderSDTO;
        private Guid _orderId;
        private string _orderCode;
        private List<Deploy.CommodityDTO> _commodityList;


        /// <summary>
        /// 资源消费结果
        /// </summary>
        private List<ResultDTO> _result = new List<ResultDTO>();

        /// <summary>
        /// 优惠券信息
        /// </summary>
        private Coupon.Deploy.CustomDTO.CouponNewDTO _couponInfo;


        /// <summary>
        /// 自提点
        /// </summary>
        private AppSelfTakeStation _appSelfTakeStation = null;

        /// <summary>
        /// 订单目的地-省
        /// </summary>
        private Area _freightToProvince = null;

        /// <summary>
        /// 订单目的地-市
        /// </summary>
        private Area _freightToCity = null;



        /// <summary>
        /// 积分兑换比例
        /// </summary>
        private int _scoreCost = 0;

        #endregion

        #region 共有属性

        /// <summary>
        /// 积分兑换比例
        /// </summary>
        public int ScoreCost
        {
            get
            {
                return _scoreCost;
            }
        }


        /// <summary>
        /// 优惠券信息
        /// </summary>
        public Coupon.Deploy.CustomDTO.CouponNewDTO CouponInfo
        {
            get
            {
                return _couponInfo;
            }
        }

        /// <summary>
        /// 自提点
        /// </summary>
        public AppSelfTakeStation AppSelfTakeStation{ get {return _appSelfTakeStation;} }

        /// <summary>
        /// 订单目的地-省
        /// </summary>
        public Area FreightToProvince { get { return _freightToProvince; } }

        /// <summary>
        /// 订单目的地-市
        /// </summary>
        public Area FreightToCity { get { return _freightToCity; } }

        #endregion

        /// <summary>
        /// 下订单资源校验入口
        /// </summary>
        /// <param name="rvp"></param>
        public List<ResultDTO> Verify(ResourceVerifyParamDTO rvp)
        {
            try
            {
                if (rvp == null)
                {
                    return _result;
                }
                _orderSDTO = rvp.OrderInfo;
                _orderId = rvp.OrderId;
                _orderCode = rvp.OrderCode;
                _commodityList = rvp.CommodityList;


                List<Tuple<Action<object>, object>> actionList = new List<Tuple<Action<object>, object>>();

                //jdcode
                var jdcAction = new Tuple<Action<object>, object>(CheckJDCodeExists, rvp);
                actionList.Add(jdcAction);

                //易捷币、易捷抵用券
                var yjbAction = new Tuple<Action<object>, object>(CheckYJCoupon, rvp);
                actionList.Add(yjbAction);

                //发票
                var invoiceAction = new Tuple<Action<object>, object>(CheckInvoice, rvp);
                actionList.Add(invoiceAction);

                //自提点
                var stsAction = new Tuple<Action<object>, object>(CheckSelfTakeStation, rvp);
                actionList.Add(stsAction);

                //订单商品
                var ocAction = new Tuple<Action<object>, object>(CheckOrderCommodity, rvp);
                actionList.Add(ocAction);

                //销售区域
                var saAction = new Tuple<Action<object>, object>(CheckSaleArea, rvp);
                actionList.Add(saAction);

                //优惠券
                var couponAction = new Tuple<Action<object>, object>(CheckOrderCoupon, rvp);
                actionList.Add(couponAction);

                //积分
                var scoreAction = new Tuple<Action<object>, object>(CheckOrderScore, rvp);
                actionList.Add(scoreAction);

                //拼团
                var diyGroupAction = new Tuple<Action<object>, object>(CheckDiyGroup, rvp);
                actionList.Add(diyGroupAction);

                //服务订单
                var soAction = new Tuple<Action<object>, object>(CheckServiceOrder, rvp);
                actionList.Add(soAction);

                //配送优惠信息
                var dfdAction = new Tuple<Action<object>, object>(CheckDeliveryFeeDiscount, rvp);
                actionList.Add(dfdAction);

                //活动资源
                var pcrAction = new Tuple<Action<object>, object>(PreCheckPromotion, rvp);
                actionList.Add(pcrAction);


                //验证易捷卡余额
                var yjcbAction = new Tuple<Action<object>, object>(ValidYjCards, rvp);
                actionList.Add(yjcbAction);

                MultiWorkTask mwt = new MultiWorkTask();
                mwt.Start(actionList);

                if (_result.Any())
                {
                    string rstr = JsonConvert.SerializeObject(_result);
                    string x = string.Format("ResourceVerify.Verify =>订单：{0}，{1}的效验结果：{2}", _orderId, _orderCode, rstr);
                    LogHelper.Debug(x);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("ResourceVerify.Verify异常，异常信息：", ex);
            }

            return _result;
        }


        #region 校验方法

        /// <summary>
        /// 效验商品的jdcode
        /// </summary>
        /// <param name="state"></param>
        private void CheckJDCodeExists(object state)
        {
            try
            {
                ResultDTO result = new ResultDTO();

                ResourceVerifyParamDTO rvp = (ResourceVerifyParamDTO)state;
                var comList = rvp.CommodityList;
                var stockList = rvp.StockList;

                List<string> jdResult = ThirdECommerceHelper.CheckJDCodeExists(_orderSDTO.AppId, comList, stockList);
                if (jdResult.Count > 0)
                {
                    result.ResultCode = (int)ReturnCodeEnum.CommoditySold;
                    result.Message = ReturnCodeEnum.CommoditySold.GetDescription();
                    _result.Add(result);
                    return;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("ResourceVerify.CheckJDCodeExists异常，异常信息：", ex);
            }
        }

        /// <summary>
        /// 校验易捷抵用券
        /// </summary>
        /// <param name="state"></param>
        private void CheckYJCoupon(object state)
        {
            try
            {
                // 易捷抵现券
                if (_orderSDTO.YJCouponPrice <= 0)
                {
                    return;
                }
                ResultDTO result = new ResultDTO();

                var yjCouponSetting = YJBSV.GetYJCouponSetting(_orderSDTO.EsAppId);
                if (!yjCouponSetting.CanMultipuled && _orderSDTO.YJCouponIds.Count > 1)
                {
                    result.ResultCode = (int)ReturnCodeEnum.YJCouponCannotMultiple;
                    result.Message = ReturnCodeEnum.YJCouponCannotMultiple.GetDescription();
                    _result.Add(result);
                    return;
                }
                if (!yjCouponSetting.CanCombinabled)
                {
                    if (_orderSDTO.IsSetMeal || _orderSDTO.IsUseYouKa
                        || _orderSDTO.YJBPrice > 0 || _orderSDTO.CouponValue > 0)
                    {
                        result.ResultCode = (int)ReturnCodeEnum.CannotEnjoyMultiplePreferentialActivities;
                        result.Message = ReturnCodeEnum.CannotEnjoyMultiplePreferentialActivities.GetDescription();
                        _result.Add(result);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("ResourceVerify.CheckYJCoupon异常，异常信息：", ex);
            }
        }

        /// <summary>
        /// 校验发票
        /// </summary>
        private void CheckInvoice(object state)
        {
            try
            {
                if (!CheckInvoice(_orderSDTO))
                {
                    ResultDTO result = new ResultDTO();
                    result.ResultCode = (int)ReturnCodeEnum.InvoiceWrong;
                    result.Message = ReturnCodeEnum.InvoiceWrong.GetDescription();
                    _result.Add(result);
                    return;
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error("ResourceVerify.CheckInvoice异常，异常信息：", ex);
            }
        }

        /// <summary>
        /// 校验自提点
        /// </summary>
        /// <param name="state"></param>
        private void CheckSelfTakeStation(object state)
        {
            OrderSDTO orderSDTO = _orderSDTO;
            ResourceVerifyParamDTO rvp = (ResourceVerifyParamDTO)state;
            bool hasVirtualCommodity = rvp.HasVirtualCommodity;


            try
            {
                if (orderSDTO.SelfTakeFlag != 1)
                {
                    return;
                }

                ResultDTO result = new ResultDTO();

                if (orderSDTO.AppOrderPickUpInfo == null || orderSDTO.AppOrderPickUpInfo.SelfTakeStationId == Guid.Empty)
                {
                    //自提订单没有自提信息。
                    result.ResultCode = (int)ReturnCodeEnum.OrderNoPickUpInfo;
                    result.Message = ReturnCodeEnum.OrderNoPickUpInfo.GetDescription();
                    _result.Add(result);
                    return;
                }

                if (orderSDTO.AppOrderPickUpInfo.BookStartTime >= orderSDTO.AppOrderPickUpInfo.BookEndTime)
                {
                    result.ResultCode = (int)ReturnCodeEnum.PickupDateError;
                    result.Message = ReturnCodeEnum.PickupDateError.GetDescription();
                    _result.Add(result);
                    return;
                }

                if (string.IsNullOrWhiteSpace(orderSDTO.AppOrderPickUpInfo.Name))
                {
                    result.ResultCode = (int)ReturnCodeEnum.PleaseFillInPicker;
                    result.Message = ReturnCodeEnum.PleaseFillInPicker.GetDescription();
                    _result.Add(result);
                    return;
                }


                if (!orderSDTO.AppOrderPickUpInfo.BookDate.HasValue)
                {
                    result.ResultCode = (int)ReturnCodeEnum.PleaseSelectPickDate;
                    result.Message = ReturnCodeEnum.PleaseSelectPickDate.GetDescription();
                    _result.Add(result);
                    return;
                }

                _appSelfTakeStation = AppSelfTakeStation.ObjectSet().Where(t => t.Id == orderSDTO.AppOrderPickUpInfo.SelfTakeStationId && !t.IsDel).FirstOrDefault();
                if (_appSelfTakeStation == null)
                {
                    result.ResultCode = (int)ReturnCodeEnum.SelfTakeStationClosed;
                    result.Message = ReturnCodeEnum.SelfTakeStationClosed.GetDescription();
                    _result.Add(result);
                    return;
                }
                var _now = DateTime.Now;
                var _nowZero = DateTime.Parse(_now.ToString("yyyy-MM-dd"));
                var startDate = _nowZero.AddDays(_appSelfTakeStation.DelayDay);
                var endDate = _nowZero.AddDays(_appSelfTakeStation.DelayDay + _appSelfTakeStation.MaxBookDay);


                var bookDate = orderSDTO.AppOrderPickUpInfo.BookDate.Value;
                if (!(DateTime.Compare(bookDate, startDate) >= 0 && DateTime.Compare(bookDate, endDate) <= 0))
                {
                    result.ResultCode = (int)ReturnCodeEnum.PickupDateInvalid;
                    result.Message = ReturnCodeEnum.PickupDateInvalid.GetDescription();
                    _result.Add(result);
                    return;
                }

                var appStsOfficeTime = AppStsOfficeTime.ObjectSet().Where(t => t.SelfTakeStationId == _appSelfTakeStation.Id).ToList();
                if (appStsOfficeTime.Any())
                {
                    var weekDay = orderSDTO.AppOrderPickUpInfo.BookDate.Value.DayOfWeek;
                    int week = (int)AppSelfTakeSV.ConvertToStationWeek(weekDay);
                    var weekListSet = appStsOfficeTime.Where(t => (t.WeekDays & week) == week).ToList();
                    if (weekListSet.Any())
                    {
                        //有此日期，然后查是否有此时间段
                        var timeSet = weekListSet.Where(
                             t =>
                             t.StartTime <= orderSDTO.AppOrderPickUpInfo.BookStartTime.Value &&
                             orderSDTO.AppOrderPickUpInfo.BookStartTime.Value < t.EndTime &&
                             t.StartTime < orderSDTO.AppOrderPickUpInfo.BookEndTime.Value &&
                             orderSDTO.AppOrderPickUpInfo.BookEndTime.Value <= t.EndTime).FirstOrDefault();
                        if (timeSet == null)
                        {
                            result.ResultCode = (int)ReturnCodeEnum.PickupDateInvalid;
                            result.Message = ReturnCodeEnum.PickupDateInvalid.GetDescription();
                            _result.Add(result);
                            return;
                        }
                    }
                    else
                    {
                        result.ResultCode = (int)ReturnCodeEnum.PickupDateInvalid;
                        result.Message = ReturnCodeEnum.PickupDateInvalid.GetDescription();
                        _result.Add(result);
                        return;
                    }
                }


                _freightToProvince = ProvinceCityHelper.GetProvinceByAreaCode(_appSelfTakeStation.Province);
                _freightToCity = ProvinceCityHelper.GetCityByAreaCode(_appSelfTakeStation.City);

                if ((_freightToProvince == null || _freightToCity == null) && !hasVirtualCommodity)
                {
                    result.ResultCode = (int)ReturnCodeEnum.NoCargoArea;
                    result.Message = ReturnCodeEnum.NoCargoArea.GetDescription();
                    _result.Add(result);
                    return;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("ResourceVerify.CheckSelfTakeStation异常，异常信息：", ex);
            }

        }

        /// <summary>
        /// 检查订单商品
        /// </summary>
        /// <param name="state"></param>
        private void CheckOrderCommodity(object state)
        {
            try
            {
                ResourceVerifyParamDTO rvp = (ResourceVerifyParamDTO)state;
                bool b = checkOrderCommodity(_orderSDTO, rvp.CommodityList, rvp.StockList, rvp.ComproDict, rvp.VipPromotion);
                if (!b)
                {
                    ResultDTO result = new ResultDTO();
                    result.ResultCode = (int)ReturnCodeEnum.PurchaseQuantityOverflowLimit;
                    result.Message = ReturnCodeEnum.PurchaseQuantityOverflowLimit.GetDescription();
                    _result.Add(result);
                    return;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("ResourceVerify.CheckOrderCommodity异常，异常信息：", ex);
            }
        }

        /// <summary>
        /// 校验商品销售区域，自提订单使用
        /// </summary>
        ///<param name="state"></param>
        /// <returns></returns>
        private void CheckSaleArea(object state)
        {
            try
            {
                ResourceVerifyParamDTO rvp = (ResourceVerifyParamDTO)state;
                if (rvp.HasVirtualCommodity)
                {
                    return;
                }

                ResultDTO result = new ResultDTO();

                OrderSDTO orderSDTO = _orderSDTO;

                List<Deploy.CommodityDTO> comList = rvp.CommodityList;

                Area province = ProvinceCityHelper.GetAreaByName(orderSDTO.Province);
                Area city = ProvinceCityHelper.GetAreaByName(orderSDTO.City);
                //todo 如果有自提业务，province、city可能是自提点的省市。

                if (comList == null || !comList.Any() || province == null || city == null)
                {
                    result.ResultCode = (int)ReturnCodeEnum.NoCargoArea;
                    result.Message = ReturnCodeEnum.NoCargoArea.GetDescription();
                    _result.Add(result);
                    return;
                }

                string message = string.Empty;
                bool hasError = false;
                foreach (var commodityDTO in comList)
                {
                    if (string.IsNullOrEmpty(commodityDTO.SaleAreas)
                        || commodityDTO.SaleAreas == ProvinceCityHelper.CountryCode)
                    {
                        continue;
                    }

                    var saleAreaArray = commodityDTO.SaleAreas.Replace("，", ",").Replace(";", ",").Replace("；", ",").Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    if (!saleAreaArray.Any())
                    {
                        continue;
                    }
                    if (saleAreaArray.Contains(province.AreaCode) || saleAreaArray.Contains(city.AreaCode))
                    {
                        continue;
                    }
                    message += commodityDTO.Name + "，";
                    hasError = true;
                }
                if (hasError)
                {
                    result.ResultCode = (int)ReturnCodeEnum.NoCargoArea;
                    result.Message = message.Substring(0, message.Length - 1) + " 在" + city.Name + "无货";
                    _result.Add(result);
                    return;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("ResourceVerify.CheckSaleArea异常，异常信息：", ex);
            }
        }

        /// <summary>
        /// 校验订单优惠券是否有效
        /// </summary>
        ///<param name="state">参数</param>
        /// <returns></returns>
        private void CheckOrderCoupon(object state)
        {
            ResultDTO result = new ResultDTO();

            OrderSDTO orderSDTO = _orderSDTO;

            try
            {
                decimal orderTotalPrice = orderSDTO.Price;

                if (orderSDTO.CouponValue == 0 && orderSDTO.CouponId == Guid.Empty
                    && orderSDTO.StoreCouponId == Guid.Empty && orderSDTO.StoreCouponShopDivid == 0)
                {
                    //如果没有优惠券，直接返回校验通过。
                    return;
                }

                var queryCoupon = new Coupon.Deploy.CustomDTO.ListCouponNewRequestDTO
                {
                    UserId = orderSDTO.UserId,
                    CouponIds = new List<Guid> { orderSDTO.CouponId == Guid.Empty ? orderSDTO.StoreCouponId : orderSDTO.CouponId }
                };


                var resultCoupon = Jinher.AMP.BTP.TPS.CouponSV.Instance.GetUserCouponsByIds(queryCoupon);//
                if (resultCoupon == null || resultCoupon.Code != 0 || resultCoupon.Data == null || (!resultCoupon.Data.Any()))
                {
                    result.ResultCode = resultCoupon.Code;
                    result.Message = resultCoupon.Info;
                    _result.Add(result);
                    return;
                }


                _couponInfo = resultCoupon.Data[0];
                //有数据，并且为绑定和在使用的
                if (_couponInfo == null || _couponInfo.CouponTemplateState != Coupon.Deploy.Enum.CouponTemplateState.Use)
                {
                    AddMessage(ReturnCodeEnum.CouponStateError);
                    return;
                }
                else if (orderSDTO.CouponId != Guid.Empty && _couponInfo.CouponState != Coupon.Deploy.Enum.CouponState.Bind)
                {
                    //单店优惠券
                    AddMessage(ReturnCodeEnum.CouponStateError);
                    return;
                }
                else if (orderSDTO.StoreCouponId != Guid.Empty
                    && _couponInfo.CouponState != Coupon.Deploy.Enum.CouponState.Bind
                    && _couponInfo.CouponState != Coupon.Deploy.Enum.CouponState.Used)
                {
                    //跨店优惠券，第一个订单消费时改为“已使用”，第二个订单效验时为“已使用”
                    AddMessage(ReturnCodeEnum.CouponStateError);
                    return;
                }
                //优惠券的面值大于订单总价，优惠的实际金额不能大于订单总价
                if (_couponInfo.Cash > orderTotalPrice)
                {
                    //单店优惠券
                    if (orderSDTO.CouponId != Guid.Empty && orderSDTO.CouponValue != orderTotalPrice)
                    {
                        AddMessage(ReturnCodeEnum.CouponExceedTotalPrice);
                        return;
                    }
                    ////跨店优惠券
                    //else if (orderSDTO.StoreCouponId != Guid.Empty && orderSDTO.StoreCouponShopDivid != orderTotalPrice)
                    //{
                    //    result.ResultCode = (int)ReturnCodeEnum.CouponExceedTotalPrice;
                    //    result.Message = ReturnCodeEnum.CouponExceedTotalPrice.GetDescription();
                    //    _result.Add(result);
                    //    return;
                    //}
                }

                //这个订单的优惠券的值 大于 优惠券的面值
                if (orderSDTO.CouponValue > _couponInfo.Cash || orderSDTO.StoreCouponShopDivid > _couponInfo.Cash)
                {
                    AddMessage(ReturnCodeEnum.CouponPirceExceedCash);
                    return;
                }
                //指定商品的
                if (_couponInfo.GoodList != null && _couponInfo.GoodList.Count > 0 && _couponInfo.CouponType == Coupon.Deploy.Enum.CouponType.SpecifyGoods)
                {
                    var comAmountList = (from com in orderSDTO.ShoppingCartItemSDTO
                                         join couPonCom in _couponInfo.GoodList on com.Id equals couPonCom
                                         select com.Amount).ToList();

                    if (comAmountList.Any())
                    {
                        if (_couponInfo.LimitCondition <= 0)
                        {
                            //result = true;
                        }
                        else if (comAmountList.Sum() >= _couponInfo.LimitCondition)
                        {
                            //result = true;
                        }
                    }
                }
                //商店通用的
                else if (_couponInfo.CouponType == Coupon.Deploy.Enum.CouponType.BeInCommon)
                {
                    if (_couponInfo.LimitCondition == 0)
                    {
                        //result = true;
                    }
                    else if (orderTotalPrice >= _couponInfo.LimitCondition)
                    {
                        //result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CommodityOrderSV.checkOrderCoupon 优惠券接口异常。userId：{0}，CouponIds：{1}，ex:{2}", orderSDTO.UserId, orderSDTO.CouponId, ex));
                AddMessage(ReturnCodeEnum.CheckCouponException);
                return;
            }
        }

        /// <summary>
        /// 校验下单积分使用是否有效, 校验范围：
        /// <para>1应用是否启用积分抵现功能</para>
        /// <para>2是否设置商品抵用比例</para>
        /// <para>3订单可用积分抵现金额是否超过商品设置</para>
        /// <para>4各订单项中积分抵现总金额是否与订单总的积分抵现金额一致</para>
        /// </summary> 
        /// <param name="state">参数</param>
        /// <returns></returns>
        private void CheckOrderScore(object state)
        {
            try
            {

                ResourceVerifyParamDTO rvp = (ResourceVerifyParamDTO)state;

                OrderSDTO orderSDTO = _orderSDTO;
                List<Deploy.CommodityDTO> coms = rvp.CommodityList;

                //不需要积分，返回true
                if (orderSDTO.ScorePrice <= 0 || orderSDTO.ScoreType == ScoreTypeEnum.None)
                {
                    return;
                }

                ResultDTO result = new ResultDTO();
                //应用是否启用积分抵现
                var appExt = (from ae in AppExtension.ObjectSet()
                              where ae.Id == orderSDTO.EsAppId
                              select new Deploy.AppExtensionDTO()
                              {
                                  IsCashForScore = ae.IsCashForScore,
                                  IsScoreAll = ae.IsScoreAll,
                                  ScorePercent = ae.ScorePercent

                              }).FirstOrDefault();
                if (appExt == null || !appExt.IsCashForScore || appExt.IsScoreAll == null)
                {
                    result.ResultCode = (int)ReturnCodeEnum.NoScoreSetting;
                    result.Message = ReturnCodeEnum.NoScoreSetting.GetDescription();
                    _result.Add(result);
                    return;
                }

                if (!new ScoreSV().GetScoreCost(orderSDTO.AppId, out _scoreCost, orderSDTO.ScoreType == ScoreTypeEnum.Currency))
                {
                    result.ResultCode = (int)ReturnCodeEnum.GetScoreCostFail;
                    result.Message = ReturnCodeEnum.GetScoreCostFail.GetDescription();
                    _result.Add(result);
                    return;
                }


                decimal scoreMoney = 0.0m;
                decimal itemsScorePrice = 0.0m;
                foreach (var cart in orderSDTO.ShoppingCartItemSDTO)
                {
                    decimal scorePercent;
                    if (appExt.IsScoreAll == true)
                    {
                        scorePercent = appExt.ScorePercent;
                    }
                    else
                    {
                        var com = coms.FirstOrDefault(c => c.Id == cart.Id);
                        if (com == null)
                        {
                            throw new ArgumentException("数据异常");
                        }
                        if (!com.ScorePercent.HasValue)
                        {
                            continue;
                        }
                        scorePercent = com.ScorePercent.Value;
                    }
                    var itemScorePrice = (cart.RealPrice * cart.CommodityNumber * scorePercent).ToMoney();
                    if (itemScorePrice < cart.ScorePrice)
                    {
                        result.ResultCode = (int)ReturnCodeEnum.ScoreRealAmountNotEqualOrder;
                        result.Message = ReturnCodeEnum.ScoreRealAmountNotEqualOrder.GetDescription();
                        _result.Add(result);
                        return;
                    }
                    scoreMoney += itemScorePrice;
                    itemsScorePrice += cart.ScorePrice;
                }
                if (scoreMoney < orderSDTO.ScorePrice || itemsScorePrice != orderSDTO.ScorePrice)
                {
                    result.ResultCode = (int)ReturnCodeEnum.ScoreRealAmountNotEqualOrder;
                    result.Message = ReturnCodeEnum.ScoreRealAmountNotEqualOrder.GetDescription();
                    _result.Add(result);
                    return;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("ResourceVerify.CheckOrderScore异常，异常信息：", ex);
            }
        }


        /// <summary>
        /// 校验拼团
        /// </summary>
        /// <param name="state">参数</param>
        private void CheckDiyGroup(object state)
        {
            try
            {
                ResourceVerifyParamDTO rvpDto = (ResourceVerifyParamDTO)state;
                OrderSDTO orderSDTO = _orderSDTO;

                ResultDTO result = new ResultDTO();

                //拼团校验
                if (orderSDTO.PromotionType != 3 || !orderSDTO.DiyGroupId.HasValue || orderSDTO.DiyGroupId == Guid.Empty)
                {
                    return;
                }

                var diyGroup = DiyGroup.ObjectSet().FirstOrDefault(c => c.Id == orderSDTO.DiyGroupId.Value);
                if (diyGroup == null)
                {
                    result.ResultCode = (int)ReturnCodeEnum.DiyGroupNotExists;
                    result.Message = ReturnCodeEnum.DiyGroupNotExists.GetDescription();
                    _result.Add(result);
                    return;
                }
                var aa = DiyGroupOrder.ObjectSet().FirstOrDefault(c => c.DiyGroupId == orderSDTO.DiyGroupId.Value && c.SubId == orderSDTO.UserId && c.State == 1);
                if (aa != null)
                {
                    result.ResultCode = (int)ReturnCodeEnum.JoinDiyGroupSuccess;
                    result.Message = ReturnCodeEnum.JoinDiyGroupSuccess.GetDescription();
                    _result.Add(result);
                    return;
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error("ResourceVerify.CheckDiyGroup异常，异常信息：", ex);
            }
        }

        /// <summary>
        /// 校验服务订单。
        /// </summary>
        ///<param name="state"></param>
        /// <returns></returns>
        private void CheckServiceOrder(object state)
        {
            try
            {
                OrderSDTO orderSDTO = _orderSDTO;
                List<Deploy.CommodityDTO> comList = _commodityList;

                ResultDTO result = new ResultDTO();
                //非服务订单直接返回。
                if (!orderSDTO.ServiceId.HasValue || orderSDTO.ServiceId == Guid.Empty)
                {
                    return;
                }
                //服务订单一次只能购买一个服务。
                if (orderSDTO.ShoppingCartItemSDTO.Count != 1)
                {
                    result.ResultCode = (int)ReturnCodeEnum.ServiceOrderOnlyOneItemAllowed;
                    result.Message = ReturnCodeEnum.ServiceOrderOnlyOneItemAllowed.GetDescription();
                    _result.Add(result);
                    return;
                }
                //虚拟商品和实体商品不能同时下单。
                int entityCount = comList.Where(c => c.CommodityType == 0).Count();
                if (entityCount > 0)
                {
                    result.ResultCode = (int)ReturnCodeEnum.PhysicalAndVirualInOneOrder;
                    result.Message = ReturnCodeEnum.PhysicalAndVirualInOneOrder.GetDescription();
                    _result.Add(result);
                    return;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("ResourceVerify.CheckServiceOrder异常，异常信息：", ex);
            }
        }

        /// <summary>
        /// 校验配送优惠信息
        /// </summary>
        /// <param name="state"></param>
        private void CheckDeliveryFeeDiscount(object state)
        {
            try
            {
                if (_orderSDTO.deliveryFeeDiscount <= 0 || _orderSDTO.SelfTakeFlag != 0 || _orderSDTO.OrderType != 2)
                {
                    return;
                }

                ResourceVerifyParamDTO rvpDto = (ResourceVerifyParamDTO)state;

                ResultDTO result = new ResultDTO();

                decimal disFreight = GetDiscountFreight(rvpDto.cateringSetting, _orderSDTO);
                if (disFreight != _orderSDTO.deliveryFeeDiscount)
                {
                    //配送费优惠信息有误
                    result.ResultCode = (int)ReturnCodeEnum.DeliveryFeeDiscountFail;
                    result.Message = ReturnCodeEnum.DeliveryFeeDiscountFail.GetDescription();
                    _result.Add(result);
                    return;
                }


                if (rvpDto.cateringSetting.DeliveryAmount > _orderSDTO.Price)
                {
                    //未达到起送金额
                    result.ResultCode = (int)ReturnCodeEnum.NotReachedStartingAmount;
                    result.Message = ReturnCodeEnum.NotReachedStartingAmount.GetDescription();
                    _result.Add(result);
                    return;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("ResourceVerify.CheckDeliveryFeeDiscount异常，异常信息：", ex);
            }
        }



        /// <summary>
        /// 预校验是否可以参加活动
        /// </summary>
        /// <param name="state">参数</param>
        private void PreCheckPromotion(object state)
        {
            ResourceVerifyParamDTO rvpDto = (ResourceVerifyParamDTO)state;

            ResultDTO result = new ResultDTO();

            Guid userId;
            //活动列表
            Dictionary<Guid, TodayPromotion> comproDict = rvpDto.ComproDict;
            //订单中商品汇总
            Dictionary<Guid, int> comNumDict = rvpDto.ComNumDict;

            OrderSDTO orderSDTO = rvpDto.OrderInfo;

            //如果有活动抢占活动资源
            if (!comproDict.Any())
            {
                return;
            }
            foreach (var t in comproDict.Values)
            {
                var comNumber = comNumDict[t.CommodityId];
                if (t.LimitBuyEach != -1)
                {
                    int sumLi = RedisHelper.GetHashValue<int>(RedisKeyConst.UserLimitPrefix + t.PromotionId + ":" + t.CommodityId, orderSDTO.UserId.ToString());
                    if (sumLi + comNumber > t.LimitBuyEach)
                    {
                        string msg = string.Format(ReturnCodeEnum.CommodityBuyLimit.GetDescription(), t.LimitBuyEach);
                        AddMessage((int)ReturnCodeEnum.CommodityBuyLimit, msg);
                        return;
                    }
                }
                if (t.LimitBuyTotal != -1)
                {
                    if (t.SurplusLimitBuyTotal + comNumber > t.LimitBuyTotal)
                    {
                        AddMessage(ReturnCodeEnum.QiangGuang);
                        return;
                    }
                }
            }
        }



        private void ValidYjCards(object state)
        {
            ResourceVerifyParamDTO dto = (ResourceVerifyParamDTO)state;

            //单订单提交在此验证易捷卡的余额，多店铺合并下单在调用SubmitOrderCommon的方法中验证易捷卡。
            if (_orderSDTO.MainOrderId != Guid.Empty)
            {
                return;
            }
            List<PayItem> yjCardsList = _orderSDTO.YjCards;
            if (yjCardsList == null || (!yjCardsList.Any()))
            {
                return;
            }

            //调用易捷卡查询接口验证余额是否发生变化。
            ResultDTO yjcResult = ValidYjCardBalance(yjCardsList);
            if (yjcResult.ResultCode == 0)
            {
                return;
            }
            AddMessage(yjcResult);
        }

        #endregion

        #region 私有方法

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

        /// <summary>
        /// 校验订单发票
        /// </summary>
        /// <param name="orderSDTO"></param>
        /// <returns></returns>
        private bool CheckInvoice(OrderSDTO orderSDTO)
        {

            if (orderSDTO.InvoiceInfo == null)
            {
                return true;
            }

            switch (orderSDTO.InvoiceInfo.Category)
            {
                case 1:
                    break;
                case 2:
                    if (string.IsNullOrEmpty(orderSDTO.InvoiceInfo.ReceiptPhone))
                        return false;
                    break;
                case 3:
                    return true;
                    break;
                case 4:
                    var vatInvoiceProof = VatInvoiceProof.ObjectSet().FirstOrDefault(c => c.Id == orderSDTO.UserId);
                    if (vatInvoiceProof == null)
                        return false;
                    orderSDTO.InvoiceInfo.InvoiceTitle = vatInvoiceProof.CompanyName;
                    orderSDTO.InvoiceInfo.InvoiceType = 2;
                    break;
                default:
                    return false;
            }

            switch (orderSDTO.InvoiceInfo.InvoiceType)
            {
                case 1:
                    orderSDTO.InvoiceInfo.InvoiceTitle = "个人";
                    break;
                case 2:
                    if (string.IsNullOrEmpty(orderSDTO.InvoiceInfo.InvoiceTitle))
                        return false;
                    break;
                default:
                    return false;
            }

            return true;
        }


        /// <summary>
        /// 校验下订单商品价格、库存
        /// </summary>
        /// <param name="orderSDTO">提交订单model</param>
        /// <param name="comList">商品列表</param>
        /// <param name="commodityStockList">商品库存列表</param>
        /// <param name="comproDict">活动列表</param>
        /// <returns></returns>
        private bool checkOrderCommodity(OrderSDTO orderSDTO, List<Deploy.CommodityDTO> comList, List<Deploy.CommodityStockDTO> commodityStockList, Dictionary<Guid, TodayPromotion> comproDict, VipPromotionDTO vipPromotionDTO)
        {
            foreach (var shoppingCartItemSdto in orderSDTO.ShoppingCartItemSDTO)
            {
                shoppingCartItemSdto.SizeAndColorId = Commodity.RepairAttrs(shoppingCartItemSdto.SizeAndColorId);
            }

            var orderComCountDict = orderSDTO.ShoppingCartItemSDTO.GroupBy(c => c.Id).ToDictionary(x => x.Key, y => y.Sum(c => c.CommodityNumber));
            foreach (var kv in orderComCountDict)
            {
                var com = comList.First(c => c.Id == kv.Key);
                if (com.Stock < kv.Value)
                    return false;
            }

            //校验商品原价 库存
            foreach (var item in orderSDTO.ShoppingCartItemSDTO)
            {
                var oPrice = item.Price;
                #region 校验原价、库存
                var commodity = comList.First(c => c.Id == item.Id);
                item.Price = commodity.Price;
                item.Pic = commodity.PicturesPath;
                item.Name = commodity.Name;
                item.Duty = commodity.Duty ?? 0;

                item.TaxRate = commodity.TaxRate;
                item.InputRax = commodity.InputRax;
                item.MarkPrice = commodity.MarketPrice;
                item.CostPrice = commodity.CostPrice;
                item.BarCode = commodity.Barcode;
                item.Code = commodity.No_Code;
                item.JDCode = commodity.JDCode;
                item.Unit = commodity.Unit;
                item.ErQiCode = commodity.ErQiCode;

                // 查询商品商城品类Ids
                var ids = CommodityInnerCategory.ObjectSet().Where(_ => _.AppId == Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId && _.CommodityId == item.Id).Select(_ => _.CategoryId).ToList();
                if (ids.Count > 0) item.InnerCatetoryIds = string.Join(",", ids);

                if (comList.First(c => c.Id == item.Id).Stock < item.CommodityNumber)
                {
                    return false;
                }
                if (item.CommodityStockId.HasValue && item.CommodityStockId != Guid.Empty)
                {
                    var itemStock = commodityStockList.FirstOrDefault(c => c.Id == item.CommodityStockId);
                    if (itemStock != null)
                    {
                        if (itemStock.Stock < item.CommodityNumber)
                        {
                            return false;
                        }
                        item.Price = itemStock.Price;
                        item.Duty = itemStock.Duty ?? 0;

                        item.CostPrice = itemStock.CostPrice;
                        item.BarCode = itemStock.Barcode;
                        item.Code = itemStock.No_Code;
                        item.JDCode = itemStock.JDCode;
                        item.ErQiCode = itemStock.ErQiCode;
                    }
                    else
                    {
                        //在下单过程中商品被删除的情况。 =>商品已删除，请重新选择商品下单！
                        return false;
                    }
                }
                else if (!string.IsNullOrEmpty(item.SizeAndColorId))
                {
                    var queryStock = commodityStockList.Where(n => n.CommodityId == item.Id).ToList();
                    if (queryStock.Count > 0)
                    {
                        List<string> nameStr = new List<string>();
                        foreach (var s in item.SizeAndColorId.Split(','))
                        {
                            if (!string.IsNullOrEmpty(s))
                            {
                                nameStr.Add(s);
                            }
                        }
                        // 双属性
                        if (nameStr.Count == 2 && !string.IsNullOrEmpty(nameStr[0]) && !string.IsNullOrEmpty(nameStr[1]))
                        {
                            foreach (var itemStock in queryStock)
                            {
                                List<ComAttributeDTO> attr =
                                    JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(itemStock.ComAttribute);
                                if ((attr[0].SecondAttribute.ToLower() == nameStr[0].ToLower() && attr[1].SecondAttribute.ToLower() == nameStr[1].ToLower())
                                    ||
                                    (attr[0].SecondAttribute.ToLower() == nameStr[1].ToLower() && attr[1].SecondAttribute.ToLower() == nameStr[0].ToLower())
                                    )
                                {
                                    item.CommodityStockId = itemStock.Id;
                                    item.Price = itemStock.Price;
                                    item.Duty = itemStock.Duty ?? 0;

                                    item.CostPrice = itemStock.CostPrice;
                                    item.BarCode = itemStock.Barcode;
                                    item.Code = itemStock.No_Code;
                                    item.JDCode = itemStock.JDCode;

                                    if (itemStock.Stock < item.CommodityNumber)
                                    {
                                        return false;
                                    }
                                    break;
                                }
                            }
                        }
                        // 单属性
                        else if (nameStr.Count == 1 && !string.IsNullOrEmpty(nameStr[0]))
                        {
                            foreach (var itemStock in queryStock)
                            {
                                List<ComAttributeDTO> attr = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(itemStock.ComAttribute);
                                if (attr.Count == 1 && attr[0].SecondAttribute.ToLower() == nameStr[0].ToLower())
                                {
                                    item.CommodityStockId = itemStock.Id;
                                    item.Price = itemStock.Price;
                                    item.Duty = itemStock.Duty ?? 0;

                                    item.CostPrice = itemStock.CostPrice;
                                    item.BarCode = itemStock.Barcode;
                                    item.Code = itemStock.No_Code;
                                    item.JDCode = itemStock.JDCode;

                                    if (itemStock.Stock < item.CommodityNumber)
                                    {
                                        return false;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
                #endregion

                bool isPromotion;
                #region 校验活动价

                decimal intensity = 10;
                decimal? discountPrice = -1;
                if (comproDict.ContainsKey(item.Id))
                {
                    if (comproDict[item.Id].DiscountPrice > -1)
                    {
                        intensity = 10;
                        discountPrice = comproDict[item.Id].DiscountPrice;
                    }
                    else
                    {
                        intensity = comproDict[item.Id].Intensity;
                        discountPrice = -1;
                    }

                    //判断是否有商品sku参与活动
                    if (comproDict[item.Id].OutsideId != null)
                    {
                        var skuActivityList = ZPHSV.Instance.GetSkuActivityList((Guid)comproDict[item.Id].OutsideId);
                        var skuActivity = skuActivityList.FirstOrDefault(t => t.CommodityId == item.Id && t.CommodityStockId == item.CommodityStockId && t.IsJoin);
                        if (skuActivity != null)
                        {
                            discountPrice = skuActivity.JoinPrice;
                        }
                    }
                }
                isPromotion = (intensity > 0 && intensity < 10) || discountPrice > 0;
                item.Intensity = intensity;
                item.DiscountPrice = Convert.ToDecimal(discountPrice.Value);

                var itemRealPrice = (item.DiscountPrice > -1)
                                        ? item.DiscountPrice
                                        : decimal.Round((item.Price * item.Intensity / 10), 2, MidpointRounding.AwayFromZero);
                item.RealPrice = itemRealPrice;
                item.Amount = decimal.Round(item.RealPrice * item.CommodityNumber, 2, MidpointRounding.AwayFromZero);

                #endregion

                #region 校验会员价
                //不参加活动的商品才会参加会员价
                if (!isPromotion && vipPromotionDTO.IsVip)
                {
                    item.Intensity = vipPromotionDTO.Intensity;
                    item.DiscountPrice = vipPromotionDTO.DiscountPrice;
                    item.RealPrice = decimal.Round((item.Price * item.Intensity / 10), 2, MidpointRounding.AwayFromZero);
                    item.Amount = decimal.Round(item.RealPrice * item.CommodityNumber, 2, MidpointRounding.AwayFromZero);
                }
                #endregion
                if (orderSDTO.IsSetMeal)
                {
                    item.Price = oPrice;
                    item.RealPrice = oPrice;
                }
            }
            return true;
        }


        /// <summary>
        /// 餐饮订单运费
        /// </summary>
        /// <param name="cateringSetting"></param>
        /// <param name="orderSDTO"></param>
        /// <returns></returns>
        private decimal GetDiscountFreight(CateringSetting cateringSetting, BTP.Deploy.CustomDTO.OrderSDTO orderSDTO)
        {
            decimal freight = decimal.Zero;
            LogHelper.Debug(string.Format("cateringSetting.Id={0},DeliveryFee={1}", cateringSetting.Id, cateringSetting.DeliveryFee));
            //TODO 目前没有校验配送范围、是否达到起送金额、门店是否正确、优惠券使用是否合法
            if (cateringSetting.FreeAmount != null && cateringSetting.FreeAmount > 0)
            {
                if (orderSDTO.Price >= cateringSetting.FreeAmount)
                {
                    freight = -cateringSetting.DeliveryFee;
                }
                else
                {
                    freight = GetDeliveryFeeDiscountFreight(cateringSetting);
                }
            }
            else
            {
                freight = GetDeliveryFeeDiscountFreight(cateringSetting);
            }

            return freight;
        }

        decimal GetDeliveryFeeDiscountFreight(CateringSetting cateringSetting)
        {
            decimal freight = decimal.Zero;
            if (cateringSetting.DeliveryFeeDiscount != null && cateringSetting.DeliveryFeeDiscount > 0 && cateringSetting.DeliveryFeeStartT != null && cateringSetting.DeliveryFeeStartT < DateTime.Now && DateTime.Now < cateringSetting.DeliveryFeeEndT)
            {
                freight = cateringSetting.DeliveryFee * (decimal)cateringSetting.DeliveryFeeDiscount;
            }

            return freight;
        }

        #endregion


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

    }

    /// <summary>
    /// 下订单资源效验参数
    /// </summary>
    public class ResourceVerifyParamDTO : ResourceParamDTO
    {
        /// <summary>
        ///商品列表
        /// </summary>
        public List<Deploy.CommodityDTO> CommodityList { get; set; }

        /// <summary>
        /// 商品库存列表
        /// </summary>
        public List<Deploy.CommodityStockDTO> StockList { get; set; }

        /// <summary>
        /// 订单中是否包含虚拟商品
        /// </summary>
        public bool HasVirtualCommodity { get; set; }


        /// <summary>
        /// 会员优惠信息
        /// </summary>
        public VipPromotionDTO VipPromotion { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public CateringSetting cateringSetting { get; set; }

         
    }


}
