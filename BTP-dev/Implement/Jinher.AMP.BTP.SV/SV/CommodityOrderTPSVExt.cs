using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.BTP.TPS.Helper;
using Jinher.AMP.FSP.Deploy.CustomDTO;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Jinher.AMP.BTP.Common.Extensions;

namespace Jinher.AMP.BTP.SV
{
    public partial class CommodityOrderSV : BaseSv,ISV.IService.ICommodityOrder
    { 
        /// <summary>
        /// 保存一个京东订单
        /// </summary>
        /// <param name="item">一个订单</param>
        /// <param name="comList">订单项对应的商品</param>
        private ResultDTO SaveOneJdOrder(OrderSDTO item, List<Commodity> comList)
        {
            ResultDTO<List<CommoditySummaryDTO>> result = new ResultDTO<List<CommoditySummaryDTO>>();

            if (item == null)
            {
                result.ResultCode = (int)ReturnCodeEnum.ParamEmpty;
                result.Message = ReturnCodeEnum.ParamEmpty.GetDescription();
                return result;
            }
            if (item.ShoppingCartItemSDTO == null || !item.ShoppingCartItemSDTO.Any()
                || comList == null || !comList.Any())
            {
                result.ResultCode = (int)ReturnCodeEnum.ParamEmpty;
                result.Message = ReturnCodeEnum.ParamEmpty.GetDescription();
                return result;
            }
            //易捷北京所有的AppId
            string Appids = CustomConfig.AppIds;
            LogHelper.Info(string.Format("Appids{0}", Appids));
            List<string> Appidlist = null;
            if (!string.IsNullOrEmpty(Appids))
            {
                Appidlist = Appids.Split(new char[] { ',' }).ToList();
            }

            if (!Appidlist.Contains(comList[0].AppId.ToString().ToUpper()))
            {
                //todo 返回非京东app.
                result.ResultCode = (int)ReturnCodeEnum.NotJdShop;
                result.Message = ReturnCodeEnum.NotJdShop.GetDescription();
                return result;
            }
            LogHelper.Info(string.Format("Appidlist的数量{0}", Appidlist.Count()));

            string orderPriceSnap = null;
            string sku = null;

            //订单项里的商品都是同一店铺的，店铺名称相同。
            string appName = APPSV.GetAppName(comList[0].AppId);

            List<CommoditySummaryDTO> errorCommodities = new List<CommoditySummaryDTO>();

            orderPriceSnap = null;
            sku = null;

            ContextSession contextSession = ContextFactory.CurrentThreadContext;

            var scis = item.ShoppingCartItemSDTO;

            foreach (var _item in scis)
            {
                Commodity commodity = comList.FirstOrDefault(_ => _.Id == _item.Id);

                LogHelper.Info(string.Format("京东日志:商品Id:{0}，JDCode：{1}，AppId：{2}", commodity.Id, commodity.JDCode, commodity.AppId));

                //京东店铺的商品没有JDCode，返回错误。
                if (string.IsNullOrWhiteSpace(commodity.JDCode))
                {
                    #region

                    Jdlogs model = new Jdlogs();
                    model.Id = Guid.NewGuid();
                    model.Content = (appName + "App中" + commodity.Name + "商品的备注编码不存在，请尽快补充填写~");
                    model.Remark = string.Empty;
                    model.AppId = commodity.AppId;
                    model.ModifiedOn = DateTime.Now;
                    model.SubTime = DateTime.Now;
                    model.Isdisable = false;

                    model.EntityState = EntityState.Added;
                    contextSession.SaveObject(model);


                    bool falg = EmailHelper.SendEmail("京东错误日志", model.Content, "yijieds@126.com");

                    var errorCommodity = new CommoditySummaryDTO();
                    errorCommodity.Id = commodity.Id;
                    errorCommodity.Name = commodity.Name;
                    errorCommodity.PicturesPath = commodity.PicturesPath;
                    errorCommodity.Price = _item.Price;
                    errorCommodity.Sku = _item.SizeAndColorId;
                    errorCommodity.ShopCartItemId = _item.ShopCartItemId;
                    errorCommodities.Add(errorCommodity);

                    result.ResultCode = (int)ReturnCodeEnum.CommoditySold;
                    result.Message = ReturnCodeEnum.CommoditySold.GetDescription();
                    result.Data = errorCommodities;
                    return result;

                    #endregion
                }

                orderPriceSnap += "{'price':" + commodity.CostPrice + ",'skuId':" + commodity.JDCode + "},";
                sku += "{'skuId':" + commodity.JDCode + ", 'num':" + _item.CommodityNumber + ",'bNeedAnnex':true, 'bNeedGift':false},";
                LogHelper.Info(string.Format("京东日志2:{0}:{1}", orderPriceSnap, sku));
            }

            LogHelper.Info(string.Format("京东日志3:{0}:{1}", orderPriceSnap, sku));
            if (string.IsNullOrEmpty(orderPriceSnap) || string.IsNullOrEmpty(sku))
            {
                //没有商品要去京东下单。
                result.ResultCode = (int)ReturnCodeEnum.NoCommodityNeedJdOrder;
                result.Message = ReturnCodeEnum.NoCommodityNeedJdOrder.GetDescription();
                return result;
            }

            orderPriceSnap = orderPriceSnap.Remove(orderPriceSnap.Length - 1, 1);
            sku = sku.Remove(sku.Length - 1, 1);
            orderPriceSnap = "[" + orderPriceSnap + "]";
            sku = "[" + sku + "]";
            string thirdOrder = Guid.NewGuid().ToString();
            if (string.IsNullOrEmpty(item.StreetCode))
            {
                item.StreetCode = "0";
            }
            //获取京东编号
            ResultDTO jdResult = JdHelper.GetJDOrderNew(thirdOrder, orderPriceSnap, sku, item.ReceiptUserName, item.ReceiptAddress, item.ReceiptPhone, "yijieds@126.com", item.ProvinceCode, item.CityCode, item.DistrictCode, item.StreetCode);
            LogHelper.Info(string.Format("京东日志4:{0}:{1}", orderPriceSnap, sku));

            //正常下单，保存订单项关系。
            if (jdResult.ResultCode == 0)
            {
                #region 京东下单情况

                JdOrderItem jdorderitemdto = new JdOrderItem()
                {
                    Id = Guid.NewGuid(),
                    //todo jdporderId????
                    //JdPorderId = jdporderId,
                    TempId = Guid.Parse(thirdOrder),
                    JdOrderId = Guid.Empty.ToString(),
                    MainOrderId = Guid.Empty.ToString(),
                    CommodityOrderId = Guid.Empty.ToString(),
                    State = Convert.ToInt32(JdEnum.YZ),
                    StateContent = new EnumHelper().GetDescription(JdEnum.YZ),
                    SubTime = DateTime.Now,
                    ModifiedOn = DateTime.Now
                };

                //todo SaveJdOrderItem(jdorderitemdto);

                JdJournal jdjournaldto = new JdJournal()
                {
                    Id = Guid.NewGuid(),
                    //todo jdporderId
                    //JdPorderId = jdporderId,
                    TempId = Guid.Parse(thirdOrder),
                    JdOrderId = Guid.Empty.ToString(),
                    MainOrderId = Guid.Empty.ToString(),
                    CommodityOrderId = Guid.Empty.ToString(),
                    Name = "京东统一下单接口",
                    Details = "初始状态为" + Convert.ToInt32(JdEnum.YZ),
                    SubTime = DateTime.Now
                };

                //todo SaveJdJournal(jdjournaldto);

                #endregion
            }
            else
            {
                #region 记录京东日志

                int resultCode = jdResult.ResultCode;
                string jdlog = jdResult.Message;
                if (resultCode == 3017) //账户异常情况特殊
                {
                    #region

                    EmailHelper.SendEmail("京东错误日志", "您的京东账户余额不足,请充值!", "yijieds@126.com");

                    Jinher.AMP.BTP.Deploy.JdlogsDTO model = new Jinher.AMP.BTP.Deploy.JdlogsDTO();
                    model.Id = Guid.NewGuid();
                    model.Content = "您的京东账户余额不足,请充值!";
                    model.Remark = string.Empty;
                    model.AppId = Guid.Empty;
                    model.ModifiedOn = DateTime.Now;
                    model.SubTime = DateTime.Now;
                    model.Isdisable = false;
                    //SaveJdlogs(model);

                    foreach (var itemlog in scis)
                    {
                        var errorCommodity = new CommoditySummaryDTO();
                        errorCommodity.Id = itemlog.Id;
                        errorCommodity.Name = itemlog.Name;
                        errorCommodity.PicturesPath = itemlog.Pic;
                        errorCommodity.Price = itemlog.Price;
                        errorCommodity.Sku = itemlog.SizeAndColorId;
                        errorCommodity.ShopCartItemId = itemlog.ShopCartItemId;
                        errorCommodities.Add(errorCommodity);
                    }

                    #endregion
                }
                else
                {
                    #region

                    if (!string.IsNullOrEmpty(jdResult.Message))
                    {
                        string num = null;
                        var matches = Regex.Matches(jdResult.Message, @"(\d+)");
                        int count = 0;
                        foreach (Match match in matches)
                        {
                            if (count == 0)
                            {
                                num = match.Value;
                            }
                            count++;
                        }
                        foreach (var itemlog in scis)
                        {
                            Commodity commodity = comList.FirstOrDefault(_ => _.Id == itemlog.Id);
                            if (commodity.JDCode != num.ToString())
                            {
                                continue;
                            }

                            var errorCommodity = new CommoditySummaryDTO();
                            errorCommodity.Id = commodity.Id;
                            errorCommodity.Name = commodity.Name;
                            errorCommodity.PicturesPath = commodity.PicturesPath;
                            errorCommodity.Price = itemlog.Price;
                            errorCommodity.Sku = itemlog.SizeAndColorId;
                            errorCommodity.ShopCartItemId = itemlog.ShopCartItemId;
                            errorCommodities.Add(errorCommodity);

                            string content = null;
                            content += (APPSV.GetAppName(commodity.AppId) + "App中" + itemlog.Name) + "商品[" + commodity.JDCode + "]";
                            if (resultCode == 2004)
                            {
                                content += "京东商品池中不存在";
                            }
                            else if (resultCode == 3019)
                            {
                                string str = jdlog;
                                if (!string.IsNullOrEmpty(str))
                                {
                                    content += "价格错误,";
                                    int num1 = str.IndexOf('[');
                                    int num2 = str.IndexOf(']');
                                    string strjdprice = str.Substring(num1 + 1, (num2 - num1 - 1));
                                    string[] arr = strjdprice.Split(new char[] { '=' });
                                    content += "京东价" + arr[1] + "元," + "易捷价" + commodity.CostPrice + "元";
                                }

                            }
                            else if (resultCode == 3008)
                            {
                                content += "已售馨";
                            }
                            else
                            {
                                content += "异常信息:" + jdlog;
                            }

                            EmailHelper.SendEmail("京东错误日志", content, "yijieds@126.com");

                            Jinher.AMP.BTP.Deploy.JdlogsDTO model = new Jinher.AMP.BTP.Deploy.JdlogsDTO();
                            model.Id = Guid.NewGuid();
                            model.Content = content;
                            model.Remark = string.Empty;
                            model.AppId = itemlog.AppId;
                            model.ModifiedOn = DateTime.Now;
                            model.SubTime = DateTime.Now;
                            model.Isdisable = false;
                            //SaveJdlogs(model);
                        }

                    }

                    #endregion
                }

                #endregion

                #region 获取京东订单单号失败的情况
               
                //////京东确认取消订单
                ////bool flag = JdHelper.OrderCancel(jdorderid.JdporderId);
                ////if (flag == true)
                ////{
                ////    List<string> jdorder = new List<string>();
                ////    jdorder.Add(jdorderid.JdporderId);
                ////    //删除京东对应订单
                ////    var res = jdorderitemfacade.DeleteJdOrderItem(jdorder);
                ////    if (res.isSuccess == true)
                ////    {
                ////        JdJournal jdjournaldto = new JdJournal()
                ////        {
                ////            Id = Guid.NewGuid(),
                ////            JdPorderId = jdporderId,
                ////            TempId = Guid.Parse(thirdOrder),
                ////            JdOrderId = Guid.Empty.ToString(),
                ////            MainOrderId = Guid.Empty.ToString(),
                ////            CommodityOrderId = Guid.Empty.ToString(),
                ////            Name = "京东确认取消订单",
                ////            Details = "删除JdOrderItem表中相应的内容",
                ////            SubTime = DateTime.Now
                ////        };
                ////        //SaveJdJournal(jdjournaldto);
                ////    }
                ////}
                 

                #endregion

                //LogHelper.Error("商品已售馨,请选择其他商品,Jdlogs：" + jdlog + " resultCode:" + resultCode);
                //return Json(new OrderResultDTO { ResultCode = 2, Message = "商品已售馨,请选择其他商品", ErrorCommodities = errorCommodities }, JsonRequestBehavior.AllowGet);
            } 
            return result;

        }


        ///// <summary>
        /////  订单状态变为4或5（客户取消订单=5）
        ///// </summary>
        ///// <param name="targetState">目标状态</param>
        ///// <param name="commodityOrder">订单信息</param>
        ///// <param name="orderitemlist">订单项列表</param>
        ///// <param name="needRefreshCacheCommoditys">需要刷新缓存的商品列表</param>
        ///// <param name="needRefreshCacheTodayPromotions">需要刷新缓存的活动列表</param>
        ///// <param name="journal">订单日志</param>
        ///// <param name="contextDTO">用户信息上下文</param>
        ///// <param name="now">当前时间</param>
        ///// <param name="remessage"></param>
        ///// <param name="appId">应用id</param>
        ///// <returns></returns>
        //private ResultDTO UpdateOrderStateTo5(UpdateCommodityOrderParamDTO ucopDto, CommodityOrder commodityOrder, List<OrderItem> orderitemlist)
        //{
        //    int newState = ucopDto.targetState;
        //    if (!OrderSV.CanChangeState(newState, commodityOrder, null, null, null))
        //    {
        //        return new ResultDTO() { ResultCode = 1, Message = "订单状态修改错误" };
        //    }
        //    ContextDTO contextDTO = this.ContextDTO;
        //    if (contextDTO == null)
        //    {
        //        contextDTO = Jinher.AMP.BTP.Common.AuthorizeHelper.CoinInitAuthorizeInfo();
        //    }

        //    Guid userId = ucopDto.userId;
        //    int payment = ucopDto.payment;
        //    Guid appId = ucopDto.appId;
        //    int targetState = ucopDto.targetState;


        //    ResultDTO result = new ResultDTO();

        //    if (contextDTO == null)
        //    {
        //        contextDTO = Jinher.AMP.BTP.Common.AuthorizeHelper.CoinInitAuthorizeInfo();
        //    }
        //    ContextSession contextSession = ContextFactory.CurrentThreadContext;
        //    List<Commodity> needRefreshCacheCommoditys = new List<Commodity>();
        //    List<TodayPromotion> needRefreshCacheTodayPromotions = new List<TodayPromotion>();
        //    DateTime now = DateTime.Now;

        //    //解冻金币
        //    if (commodityOrder.Payment != 1 && commodityOrder.RealPrice > 0)
        //    {
        //        UnFreezeGoldDTO unFreezeGoldDTO = new UnFreezeGoldDTO()
        //        {
        //            BizId = commodityOrder.Id,
        //            Sign = CustomConfig.PaySing
        //        };
        //        Jinher.AMP.FSP.Deploy.CustomDTO.ReturnInfoDTO fspResult = FSPSV.Instance.UnFreezeGold(unFreezeGoldDTO, "取消订单");
        //        if (fspResult.Code != 0)
        //        {
        //            result.ResultCode = 1;
        //            result.Message = fspResult.Message;
        //            return result;
        //        }
        //    }
        //    //回退积分
        //    SignSV.CommodityOrderCancelSrore(ContextFactory.CurrentThreadContext, commodityOrder);

        //    // 回退易捷币
        //    Jinher.AMP.BTP.TPS.Helper.YJBHelper.CancelOrder(ContextFactory.CurrentThreadContext, commodityOrder);

        //    //回退优惠券
        //    CouponSV.RefundCoupon(ContextFactory.CurrentThreadContext, commodityOrder);

        //    //加库存
        //    if (commodityOrder.State > 0 && commodityOrder.Payment == 1)
        //    {
        //        foreach (OrderItem items in orderitemlist)
        //        {
        //            Guid comId = items.CommodityId;
        //            Commodity com = Commodity.ObjectSet().Where(n => n.Id == comId).First();

        //            // zgx-modify 回滚库存
        //            if (items.CommodityStockId.HasValue && items.CommodityStockId.Value != Guid.Empty)
        //            {
        //                CommodityStock cStock = CommodityStock.ObjectSet().Where(n => n.Id == items.CommodityStockId.Value && n.CommodityId == comId).First();
        //                cStock.EntityState = System.Data.EntityState.Modified;
        //                cStock.Stock += items.Number;
        //                contextSession.SaveObject(cStock);
        //            }
        //            com.EntityState = System.Data.EntityState.Modified;
        //            com.Stock += items.Number;
        //            contextSession.SaveObject(com);
        //            needRefreshCacheCommoditys.Add(com);
        //            // 赠品库存
        //            OrderEventHelper.AddStock(items, needRefreshCacheCommoditys);
        //        }
        //    }
        //    if (commodityOrder.State == 0 || commodityOrder.State == 1 && commodityOrder.Payment == 1)  //释放活动资源
        //    {
        //        List<Tuple<string, string, int>> proComTuples = new List<Tuple<string, string, int>>();
        //        List<Tuple<string, string, long, long>> proComBuyTuples = new List<Tuple<string, string, long, long>>();

        //        foreach (var orderItem in orderitemlist.Where(c => c.PromotionId.HasValue && c.PromotionId != Guid.Empty && (c.Intensity != 10 || c.DiscountPrice != -1)))
        //        {
        //            proComTuples.Add(new Tuple<string, string, int>(orderItem.PromotionId.Value.ToString(), orderItem.CommodityId.ToString(), -orderItem.Number));
        //        }
        //        if (proComTuples.Any())
        //        {
        //            proComBuyTuples = RedisHelper.ListHIncr(proComTuples, commodityOrder.UserId);
        //            if (proComBuyTuples == null || !proComBuyTuples.Any())
        //            {
        //                return new ResultDTO { ResultCode = 1, Message = "操作失败" };
        //            }
        //        }
        //        UserLimited.ObjectSet().Context.ExecuteStoreCommand("delete from UserLimited where CommodityOrderId='" + commodityOrder.Id + "'");
        //        foreach (OrderItem orderItem in orderitemlist)
        //        {
        //            Guid comId = orderItem.CommodityId;
        //            if (orderItem.Intensity != 10 || orderItem.DiscountPrice != -1)
        //            {
        //                var promotionId = orderItem.PromotionId.HasValue ? orderItem.PromotionId.Value : Guid.Empty;
        //                if (RedisHelper.HashContainsEntry(RedisKeyConst.ProSaleCountPrefix + promotionId.ToString(), orderItem.CommodityId.ToString()))
        //                {
        //                    int surplusLimitBuyTotal = RedisHelper.GetHashValue<int>(RedisKeyConst.ProSaleCountPrefix + orderItem.PromotionId.Value.ToString(), orderItem.CommodityId.ToString());
        //                    surplusLimitBuyTotal = surplusLimitBuyTotal < 0 ? 0 : surplusLimitBuyTotal;
        //                    TodayPromotion to = TodayPromotion.ObjectSet().FirstOrDefault(n => n.CommodityId == comId && n.PromotionId == orderItem.PromotionId && n.EndTime > now && n.StartTime < now);
        //                    if (to != null)
        //                    {
        //                        to.SurplusLimitBuyTotal = surplusLimitBuyTotal;
        //                        to.EntityState = System.Data.EntityState.Modified;
        //                        needRefreshCacheTodayPromotions.Add(to);
        //                    }
        //                    PromotionItems pti = PromotionItems.ObjectSet().FirstOrDefault(n => n.PromotionId == orderItem.PromotionId && n.CommodityId == comId);
        //                    if (pti != null)
        //                    {
        //                        pti.SurplusLimitBuyTotal = surplusLimitBuyTotal;
        //                        pti.EntityState = System.Data.EntityState.Modified;
        //                    }
        //                }
        //                else  //缓存中没有，直接改库
        //                {
        //                    TodayPromotion to = TodayPromotion.ObjectSet().FirstOrDefault(n => n.CommodityId == comId && n.PromotionId == orderItem.PromotionId && n.EndTime > now && n.StartTime < now);
        //                    if (to != null)
        //                    {
        //                        to.SurplusLimitBuyTotal -= orderItem.Number;
        //                        to.SurplusLimitBuyTotal = to.SurplusLimitBuyTotal < 0 ? 0 : to.SurplusLimitBuyTotal;
        //                        to.EntityState = System.Data.EntityState.Modified;
        //                        needRefreshCacheTodayPromotions.Add(to);
        //                    }
        //                    PromotionItems pti = PromotionItems.ObjectSet().FirstOrDefault(n => n.PromotionId == orderItem.PromotionId && n.CommodityId == comId);
        //                    if (pti != null)
        //                    {
        //                        pti.SurplusLimitBuyTotal -= orderItem.Number;
        //                        pti.SurplusLimitBuyTotal = pti.SurplusLimitBuyTotal < 0 ? 0 : pti.SurplusLimitBuyTotal;
        //                        pti.EntityState = System.Data.EntityState.Modified;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    commodityOrder.ConfirmTime = now;
        //    string remessage = ucopDto.remessage;
        //    short reasonCdoe;
        //    if (!short.TryParse(remessage, out reasonCdoe))
        //    {
        //        return new ResultDTO { ResultCode = 1, Message = "操作失败" };
        //    }
        //    remessage = TypeToStringHelper.CancleOrderReasonTypeToString(reasonCdoe);
        //    LogHelper.Info("取消订单原因:" + remessage);
        //    //commodityOrder.CancelReason = remessage;
        //    commodityOrder.MessageToBuyer = remessage;
        //    commodityOrder.CancelReasonCode = reasonCdoe;
        //    //订单日志
        //    Journal journal = CreateJournal(ucopDto, commodityOrder);
        //    journal.Name = "用户取消订单";
        //    contextSession.SaveObject(journal);

        //    commodityOrder.State = targetState;
        //    commodityOrder.ModifiedOn = now;
        //    commodityOrder.EntityState = System.Data.EntityState.Modified;


        //    contextSession.SaveChanges();

        //    if (needRefreshCacheCommoditys.Any())
        //    {
        //        needRefreshCacheCommoditys.ForEach(c => c.RefreshCache(EntityState.Modified));
        //    }
        //    if (needRefreshCacheTodayPromotions.Any())
        //    {
        //        needRefreshCacheTodayPromotions.ForEach(c => c.RefreshCache(EntityState.Modified));
        //    }

        //    //订单id
        //    string odid = commodityOrder.Id.ToString();
        //    string type = "order";
        //    //用户id
        //    string usid = commodityOrder.UserId.ToString();
        //    Guid esAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
        //    AddMessage addmassage = new AddMessage();
        //    addmassage.AddMessages(odid, usid, esAppId, commodityOrder.Code, targetState, "", type);
        //    ////正品会发送消息
        //    //if (new ZPHSV().CheckIsAppInZPH(appId))
        //    //{
        //    //    addmassage.AddMessages(odid, usid, CustomConfig.ZPHAppId, commodityOrder.Code, state, "", type);
        //    //}

        //    if (result.ResultCode != 0)
        //    {
        //        return result;
        //    }
        //    string _appName = APPSV.GetAppName(commodityOrder.AppId);
        //    string mobilemess = "【" + _appName + "】" + "编号为" + commodityOrder.Code + "的订单，由于" + remessage + "，客户取消了该订单，请知悉！";
        //    string tile = "【" + _appName + "】" + "编号为" + commodityOrder.Code + "的订单，由于" + remessage + "，客户取消了该订单，请知悉！";
        //    string content = "【" + _appName + "】" + "编号为" + commodityOrder.Code + "的订单，由于" + remessage + "，客户取消了该订单，请知悉！";
        //    SendMessageCommon(tile, content, mobilemess, 3, appId, commodityOrder, contextDTO);
        //    return result;
        //}


    }

    public class JdCache
    {
        public string JdporderId { get; set; }

        public string AppId { get; set; }
    }
}
