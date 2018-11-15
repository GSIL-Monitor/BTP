using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.Deploy;
using Newtonsoft.Json.Linq;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.JAP.Cache;

namespace Jinher.AMP.BTP.TPS.Helper
{

    public class JdCache
    {
        public string JdporderId { get; set; }

        public string AppId { get; set; }
    }

    public static class JdOrderHelper
    {
        // 更新京东订单
        public static void UpdateJdorder(Guid orderId)
        {
            try
            {
                //如果更新成功 京东下单
                Jinher.AMP.BTP.IBP.Facade.CommodityOrderFacade commodityorderfacade = new Jinher.AMP.BTP.IBP.Facade.CommodityOrderFacade();
                commodityorderfacade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                Jinher.AMP.BTP.IBP.Facade.JdOrderItemFacade jdorderitemfacade = new Jinher.AMP.BTP.IBP.Facade.JdOrderItemFacade();
                jdorderitemfacade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                Jinher.AMP.BTP.IBP.Facade.JdJournalFacade jdjournalfacade = new Jinher.AMP.BTP.IBP.Facade.JdJournalFacade();
                jdjournalfacade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                var mainOrder = commodityorderfacade.GetMainOrderInfo(orderId);
                var order = commodityorderfacade.GetCommodityOrder(orderId, Guid.Empty);
                if (mainOrder == null || order == null || !order.PaymentTime.HasValue)
                {
                    LogHelper.Info("订单未支付orderId=" + orderId);
                    return;
                }
                JdOrderItemDTO model = new JdOrderItemDTO();
                model.State = 1;
                model.CommodityOrderId = orderId.ToString();
                LogHelper.Info("jdorderitem是否为空数据单号:" + model.CommodityOrderId + "");
                //获取最早的京东数据模型
                var jdorderitem = jdorderitemfacade.GetJdOrderItemList(model).ToList().FirstOrDefault();
                if (jdorderitem != null)
                {
                    //京东确认下单
                    bool flag = JdHelper.confirmOrder(jdorderitem.JdPorderId);
                    if (flag == true)
                    {
                        jdorderitem.StateContent = "确认预占";
                        jdorderitem.MainOrderId = mainOrder.MainOrderId.ToString();
                        var res = jdorderitemfacade.UpdateJdOrderItem(jdorderitem);
                        LogHelper.Info("京东确认下单res:" + res + "");
                        if (res.isSuccess == true)
                        {
                            JdJournalDTO jdjournaldto = new JdJournalDTO()
                            {
                                Id = Guid.NewGuid(),
                                JdPorderId = jdorderitem.JdPorderId,
                                TempId = jdorderitem.TempId,
                                JdOrderId = Guid.Empty.ToString(),
                                MainOrderId = mainOrder.MainOrderId.ToString(),
                                CommodityOrderId = jdorderitem.CommodityOrderId,
                                Name = "京东确认预占库存订单接口",
                                Details = "京东确认预占库存订单接口",
                                SubTime = DateTime.Now
                            };
                            //添加明细记录
                            jdjournalfacade.SaveJdJournal(jdjournaldto);
                        }

                    }

                }
                else
                {
                    LogHelper.Info("jdorderitemjdorderitemjdorderitemjdorderitemjdorderitem为空");
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("OrderEventHelper.UpdateJdorder 异常", ex);
            }
        }

        public static void DeleteRedisJdPOrder(string userId)
        {
            //数据获取完成，则删除redis
            //#region 原dll
            //GlobalCacheWrapper.Remove(RedisKeyConst.UserOrder_JdPOrderIdList, userId, CacheTypeEnum.redisSS, "BTPCache");
            //var jsonaa = GlobalCacheWrapper.GetData(RedisKeyConst.UserOrder_JdPOrderIdList, userId, CacheTypeEnum.redisSS, "BTPCache");// RedisHelper.GetHashValue<string>(RedisKeyConst.UserOrder_JdPOrderIdList, userId);
            //LogHelper.Info("【京东订单--删除后查询】关联JdOrderItem表--->userId--->[" + userId + "],缓存Json值--->【" + jsonaa + "】");
            //#endregion

            //数据获取完成，则删除redis

            #region 新dll
            //新dll
            RedisHelperNew.Remove(RedisKeyConst.UserOrder_JdPOrderIdList, userId, "BTPCache");
            string jsonaa = RedisHelperNew.RegionGet<string>(RedisKeyConst.UserOrder_JdPOrderIdList, userId, "BTPCache");
            LogHelper.Info("【京东订单--删除后查询】关联JdOrderItem表--->userId--->[" + userId + "],缓存Json值--->【" + jsonaa + "】");
            #endregion

        }

        public static ResultDTO UpdateJdOrderId(ContextSession contextSession, string MainOrderId, string CommodityOrderId, string appId, string userId)
        {
            ResultDTO dto = new ResultDTO { isSuccess = true };
            try
            {
               string jsonRedis = RedisHelperNew.RegionGet<string>(RedisKeyConst.UserOrder_JdPOrderIdList, userId, "BTPCache");

                if (!string.IsNullOrEmpty(jsonRedis))
                {
                    string json = jsonRedis.ToString();
                    LogHelper.Info("【京东订单】关联JdOrderItem表--->CommodityOrderId=[" + CommodityOrderId + "],appId--->[" + appId + "],userId--->[" + userId + "],缓存Json值--->【" + jsonRedis + "】");
                    List<JdCache> arr = JsonHelper.JsonDeserialize<List<JdCache>>(json);
                    if (arr != null)
                    {
                        if (arr.Count() > 0)
                        {

                            foreach (var item in arr)
                            {
                                if (item.AppId == appId)
                                {
                                    var jdorderitem = JdOrderItem.ObjectSet().Where(p => p.JdPorderId == item.JdporderId && p.State == 1).FirstOrDefault();
                                    if (jdorderitem != null)
                                    {
                                        jdorderitem.MainOrderId = MainOrderId;
                                        jdorderitem.CommodityOrderId = CommodityOrderId;
                                        jdorderitem.State = 1;
                                        jdorderitem.ModifiedOn = DateTime.Now;
                                        jdorderitem.EntityState = EntityState.Modified;
                                        contextSession.SaveObject(jdorderitem);
                                    }
                                }
                            }
                           
                        }
                    }
                }
                else {
                    LogHelper.Info("【京东订单】关联JdOrderItem表--->CommodityOrderId=[" + CommodityOrderId + "],appId--->[" + appId + "],userId--->[" + userId + "],缓存Json值--->【" + jsonRedis + "】");
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("JdOrderItem信息保存异常。JdOrderItem：{0}", ex.Message));
                dto = new ResultDTO() { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return dto;
        }

        public static ResultDTO SubmitRefund(ContextSession contextSession, CommodityOrder order, OrderItem orderItem, OrderRefundAfterSales refund, AddressInfo address)
        {
            if (address == null)
            {
                return new ResultDTO { isSuccess = false, Message = "取件地址不能为空。" };
            }
            var strCommodityOrderId = orderItem.CommodityOrderId.ToString();
            var jdorderitem = JdOrderItem.ObjectSet()
                .Where(_ => _.CommodityOrderId == strCommodityOrderId && _.TempId == orderItem.CommodityId).FirstOrDefault();
            if (jdorderitem == null)
            {
                return new ResultDTO { isSuccess = false, ResultCode = 1, Message = "该订单不是京东订单。" };
            }
            AfterSaleDto dto = new AfterSaleDto();
            dto.jdOrderId = jdorderitem.JdOrderId;
            dto.customerExpect = 10;
            dto.questionDesc = refund.RefundDesc;
            dto.questionPic = refund.OrderRefundImgs;
            dto.asCustomerDto = new AfterSaleCustomerDto()
            {
                jdOrderId = jdorderitem.JdOrderId,
                customerContactName = address.customerContactName,
                customerTel = address.customerTel
            };
            dto.asPickwareDto = new AfterSalePickwareDto()
            {
                pickwareType = 4,
                pickwareProvince = address.pickwareProvince,
                pickwareCity = address.pickwareCity,
                pickwareCounty = address.pickwareCounty,
                pickwareVillage = address.pickwareVillage,
                pickwareAddress = address.pickwareAddress
            };
            dto.asReturnwareDto = new AfterSaleReturnwareDto
            {
                returnwareType = 10,
                returnwareCity = address.pickwareCity,
                returnwareCounty = address.pickwareCounty,
                returnwareProvince = address.pickwareProvince,
                returnwareVillage = address.pickwareVillage,
                returnwareAddress = address.pickwareAddress
            };
            dto.asDetailDto = new AfterSaleDetailDto()
            {
                skuId = jdorderitem.CommoditySkuId,
                skuNum = orderItem.Number
            };
            var result = JDSV.CreateAfsApply(dto);
            if (result.isSuccess)
            {
                // 保存到 JdOrderRefundAfterSales
                var jdOrderRefundAfterSales = JdOrderRefundAfterSales.CreateJdOrderRefundAfterSales();
                jdOrderRefundAfterSales.AppId = order.AppId;
                jdOrderRefundAfterSales.OrderRefundAfterSalesId = refund.Id;
                jdOrderRefundAfterSales.OrderId = orderItem.CommodityOrderId;
                jdOrderRefundAfterSales.OrderItemId = orderItem.Id;
                jdOrderRefundAfterSales.JdOrderId = jdorderitem.JdOrderId;
                jdOrderRefundAfterSales.CommodityId = orderItem.CommodityId;
                jdOrderRefundAfterSales.CommodityNum = orderItem.Number;
                jdOrderRefundAfterSales.SkuId = jdorderitem.CommoditySkuId;
                jdOrderRefundAfterSales.Cancel = 1;
                jdOrderRefundAfterSales.PickwareType = 4;
                jdOrderRefundAfterSales.CustomerContactName = address.customerContactName;
                jdOrderRefundAfterSales.CustomerTel = address.customerTel;
                jdOrderRefundAfterSales.PickwareAddress = address.ProviceCityStr + address.pickwareAddress;
                jdOrderRefundAfterSales.AfsServiceStep = 10;
                jdOrderRefundAfterSales.AfsServiceStepName = "申请阶段";
                contextSession.SaveObject(jdOrderRefundAfterSales);
            }
            else
            {
                bool isWriteLog = false;
                switch (result.ResultCode)
                {
                    case -1: // 系统异常
                        break;
                    //京东异常
                    case 2000:
                        result.Message = "提交失败，请重试~";
                        break;
                    case 6000:
                        result.Message = "网络异常，请稍后重试~";
                        break;
                    case 6001:
                    case 6002:
                    case 6003:
                    case 6004:
                    case 6006:
                    case 6007:
                    case 6011:
                    case 6012:
                        isWriteLog = true;
                        break;
                    case 6005:
                        result.Message = "收货后才能申请退货~";
                        break;
                    case 6008:
                        result.Message = "收货后才能申请退货~";
                        break;
                    case 6009:
                        result.Message = "该商品不支持售后~";
                        break;
                    case 6010:
                        result.Message = "退货申请商品数量超过订单商品数量~";
                        isWriteLog = true;
                        break;
                    case 6013:
                        result.Message = "售后申请审核未通过，如有异议，请联系客服处理~";
                        break;
                    default:
                        break;
                }
                if (isWriteLog)
                {
                    var log = new Jinher.AMP.BTP.Deploy.JdlogsDTO
                    {
                        Id = Guid.NewGuid(),
                        Content = "【" + order.Code + "】中的" + orderItem.Name + "商品【" + jdorderitem.CommoditySkuId + "】，提交售后申请失败，失败原因：【" + result.ResultCode + ":" + result.Message + "】"
                    };
                    JdlogsFacade facade = new JdlogsFacade();
                    facade.SaveJdlogs(log);
                }
            }
            return result;
        }

        public static ResultDTO CancelRefund(ContextSession contextSession, OrderRefundAfterSales refund)
        {
            var jdOrderRefundAfterSales = JdOrderRefundAfterSales.ObjectSet().Where(_ => _.OrderRefundAfterSalesId == refund.Id).FirstOrDefault();
            if (jdOrderRefundAfterSales == null)
            {
                return new ResultDTO { isSuccess = true, Message = "非京东订单，跳过。" };
            }
            if (string.IsNullOrEmpty(jdOrderRefundAfterSales.AfsServiceId))
            {
                return new ResultDTO { isSuccess = false, Message = "未获取到京东退款服务单号。", ResultCode = -1 };
            }
            ResultDTO result = null;
            if (jdOrderRefundAfterSales.CommodityNum.HasValue && jdOrderRefundAfterSales.CommodityNum.Value > 1 && !string.IsNullOrWhiteSpace(jdOrderRefundAfterSales.AfsServiceIds))
            {
                result = JDSV.AuditMultipulCancel(jdOrderRefundAfterSales.AfsServiceId.Split(',').ToList(), "用户取消");
            }
            else
            {
                result = JDSV.AuditCancel(jdOrderRefundAfterSales.AfsServiceId, "用户取消");
            }
            if (result.isSuccess)
            {
                jdOrderRefundAfterSales.Cancel = 0;
                contextSession.SaveObject(jdOrderRefundAfterSales);
            }
            return result;
        }

        public static void GetJdRefundInfo(SubmitOrderRefundDTO refund)
        {
            var jdOrderRefundAfterSales = JdOrderRefundAfterSales.ObjectSet().Where(_ => _.OrderRefundAfterSalesId == refund.Id).FirstOrDefault();
            if (jdOrderRefundAfterSales == null)
            {
                return;
            }
            refund.JdOrderRefundInfo = new JdOrderRefundDto
            {
                ServiceId = jdOrderRefundAfterSales.AfsServiceId,
                Cancel = jdOrderRefundAfterSales.Cancel,
                CustomerContactName = jdOrderRefundAfterSales.CustomerContactName,
                CustomerTel = jdOrderRefundAfterSales.CustomerTel,
                PickwareAddress = jdOrderRefundAfterSales.PickwareAddress,
                PickwareType = jdOrderRefundAfterSales.PickwareType
            };
        }

        /// <summary>
        /// 金采支付 京东订单补发job
        /// </summary>
        //[Obsolete("没有符合SupplierCode=43266570的订单")]
        public static void SynchroJdForJC()
        {
            //return;
            try
            {
                //var orders = CommodityOrder.ObjectSet().Where(_ => _.State == 1 && _.Payment == 2001 && _.SupplierCode == "43266570").ToList();
                var orders = CommodityOrder.ObjectSet().Where(_ => _.State == 1 && _.SupplierCode == "43266570").ToList();
                foreach (var order in orders)
                {
                    //如果更新成功 京东下单
                    CommodityOrderFacade commodityorderfacade = new CommodityOrderFacade { ContextDTO = AuthorizeHelper.InitAuthorizeInfo() };
                    JdOrderItemFacade jdorderitemfacade = new JdOrderItemFacade { ContextDTO = AuthorizeHelper.InitAuthorizeInfo() };
                    JdJournalFacade jdjournalfacade = new JdJournalFacade { ContextDTO = AuthorizeHelper.InitAuthorizeInfo() };

                    var commodityorder = commodityorderfacade.GetMainOrderInfo(order.Id);
                    JdOrderItemDTO model = new JdOrderItemDTO
                    {
                        State = 1,
                        CommodityOrderId = order.Id.ToString()
                    };

                    //获取最早的京东数据模型
                    var jdorderitem = jdorderitemfacade.GetJdOrderItemList(model).ToList().FirstOrDefault();
                    if (jdorderitem != null)
                    {
                        //京东确认下单
                        bool flag = JdHelper.confirmOrder(jdorderitem.JdPorderId);
                        if (flag)
                        {
                            jdorderitem.StateContent = "确认预占";
                            jdorderitem.MainOrderId = commodityorder.MainOrderId.ToString();
                            var res = jdorderitemfacade.UpdateJdOrderItem(jdorderitem);
                            if (res.isSuccess)
                            {
                                JdJournalDTO jdjournaldto = new JdJournalDTO()
                                {
                                    Id = Guid.NewGuid(),
                                    JdPorderId = jdorderitem.JdPorderId,
                                    TempId = jdorderitem.TempId,
                                    JdOrderId = Guid.Empty.ToString(),
                                    MainOrderId = commodityorder.MainOrderId.ToString(),
                                    CommodityOrderId = jdorderitem.CommodityOrderId,
                                    Name = "京东确认预占库存订单接口",
                                    Details = "京东确认预占库存订单接口",
                                    SubTime = DateTime.Now
                                };
                                //添加明细记录
                                jdjournalfacade.SaveJdJournal(jdjournaldto);
                            }
                        }
                    }
                    else
                    {
                        LogHelper.Info("SynchroJdForJC方法，jdorderitem为空");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("SynchroJdForJC方法异常", ex);
            }
        }

        public static string AutoRefundByItemId(Guid commodityOrderItemId, decimal refundMoney = 0)
        {
            try
            {
                LogHelper.Info("手动退款，Begin...................." + commodityOrderItemId);
                CommodityOrderFacade commodityOrderFacade = new CommodityOrderFacade();
                var orderSV = new BTP.ISV.Facade.CommodityOrderFacade();
                Jinher.AMP.BTP.IBP.Facade.CommodityOrderAfterSalesFacade cf = new Jinher.AMP.BTP.IBP.Facade.CommodityOrderAfterSalesFacade();
                orderSV.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();

                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var orderItem = OrderItem.ObjectSet().Where(_ => _.Id == commodityOrderItemId).FirstOrDefault();
                if (orderItem == null)
                {
                    LogHelper.Error("手动退款失败，未找到订单Item, 提交申请：CommodityOrderItemId=" + commodityOrderItemId);
                }
                SubmitOrderRefundDTO modelParam = new SubmitOrderRefundDTO();
                modelParam.Id = modelParam.commodityorderId = orderItem.CommodityOrderId;
                modelParam.RefundDesc = "手动退款";
                modelParam.RefundMoney = refundMoney;
                modelParam.State = 1;
                modelParam.RefundReason = "其他";
                // 仅退款
                modelParam.RefundType = 0;
                modelParam.OrderRefundImgs = "";
                modelParam.OrderItemId = commodityOrderItemId;

                LogHelper.Info("手动退款，提交申请：CommodityOrderId=" + modelParam.commodityorderId + "CommodityOrderItemId=" + modelParam.OrderItemId);

                var result = orderSV.SubmitOrderRefund(modelParam);
                if (result.ResultCode != 0)
                {
                    LogHelper.Error("手动退款失败，提交申请：CommodityOrderItemId=" + modelParam.OrderItemId + ", Message=" + result.Message);
                    throw new Exception("退款失败");
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("手动退款异常", ex);
                throw ex;
            }
            LogHelper.Info("手动退款，End...................." + commodityOrderItemId);
            return "ok";
        }

        public static string RepairJdOrder(string jdPorderId)
        {
            var commodityorderfacade = new Jinher.AMP.BTP.IBP.Facade.CommodityOrderFacade();
            commodityorderfacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
            var jdorderitemfacade = new Jinher.AMP.BTP.IBP.Facade.JdOrderItemFacade();
            jdorderitemfacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
            var jdjournalfacade = new Jinher.AMP.BTP.IBP.Facade.JdJournalFacade();
            jdjournalfacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();

            //开始拆单
            JdOrderItemDTO model = new JdOrderItemDTO();
            model.JdPorderId = jdPorderId;
            model.State = 1;//
            //获取jdorderitem表中的数据
            var jdorderitem = jdorderitemfacade.GetJdOrderItemList(model).FirstOrDefault();
            if (jdorderitem != null)
            {
                var getFullOrderInfo = commodityorderfacade.GetFullOrderInfoById(jdorderitem.CommodityOrderId);
                bool flag = JdHelper.IsjdOrder(jdPorderId);
                if (flag == true)
                {
                    #region 父订单拆单行为
                    var selectJdOrder2 = JdHelper.selectJdOrder2(jdPorderId);
                    if (!string.IsNullOrWhiteSpace(selectJdOrder2))
                    {
                        List<string> list = new List<string>();
                        list.Add(jdPorderId);
                        //删除已有的父类订单
                        var delresult = jdorderitemfacade.DeleteJdOrderItem(list);
                        if (delresult.isSuccess == true)
                        {
                            JArray objson = JArray.Parse(selectJdOrder2);
                            foreach (var item in objson)
                            {
                                JArray objsku = JArray.Parse(item["sku"].ToString());
                                foreach (var ZiJdOrder in objsku)
                                {
                                    #region 子订单包含包含多个商品的情况
                                    string skuId = ZiJdOrder["skuId"].ToString();
                                    Guid TempId = Guid.Empty;
                                    Guid commodityOrderItemId = Guid.Empty;
                                    if (getFullOrderInfo.OrderItems != null && getFullOrderInfo.OrderItems.Count() > 0)
                                    {
                                        var commoditydto = getFullOrderInfo.OrderItems.Where(p => p.JdCode == skuId).FirstOrDefault();
                                        if (commoditydto != null)
                                        {
                                            commodityOrderItemId = commoditydto.Id;
                                            TempId = commoditydto.CommodityId;
                                        }
                                    }
                                    int State = Convert.ToInt32(JdEnum.BCF);
                                    string StateContent = new EnumHelper().GetDescription(JdEnum.BCF);
                                    string JdOrderId = item["jdOrderId"].ToString();
                                    string Name = "京东下订单" + StateContent;
                                    string Details = "订单状态由" + jdorderitem.State + "变成" + State;
                                    if (item["orderState"].ToString() == "0")
                                    {
                                        State = Convert.ToInt32(JdEnum.BCF);
                                        StateContent = new EnumHelper().GetDescription(JdEnum.BCF);
                                        Name = "京东下订单" + StateContent;
                                        Details = "订单状态由" + jdorderitem.State + "变成" + State;
                                    }
                                    JdOrderItemDTO jdorderitemdto = new JdOrderItemDTO()
                                    {
                                        Id = Guid.NewGuid(),
                                        JdPorderId = jdPorderId,
                                        TempId = TempId,
                                        JdOrderId = JdOrderId,
                                        MainOrderId = jdorderitem.MainOrderId,
                                        CommodityOrderId = jdorderitem.CommodityOrderId,
                                        State = State,
                                        StateContent = StateContent,
                                        SubTime = DateTime.Now,
                                        ModifiedOn = DateTime.Now,
                                        CommoditySkuId = skuId,
                                        CommodityOrderItemId = commodityOrderItemId
                                    };
                                    var restult = jdorderitemfacade.SaveJdOrderItem(jdorderitemdto);
                                    if (restult.isSuccess == true)
                                    {
                                        JdJournalDTO ex = new JdJournalDTO()
                                        {
                                            JdPorderId = jdPorderId,
                                            TempId = TempId,
                                            JdOrderId = JdOrderId,
                                            MainOrderId = jdorderitem.MainOrderId,
                                            CommodityOrderId = jdorderitem.CommodityOrderId,
                                            Name = Name,
                                            Details = StateContent
                                        };
                                        var res = jdjournalfacade.GetJdJournalList(ex);
                                        if (res.Count() == 0)
                                        {
                                            JdJournalDTO jdjournaldto = new JdJournalDTO()
                                            {
                                                Id = Guid.NewGuid(),
                                                JdPorderId = jdPorderId,
                                                TempId = TempId,
                                                JdOrderId = JdOrderId,
                                                MainOrderId = jdorderitem.MainOrderId,
                                                CommodityOrderId = jdorderitem.CommodityOrderId,
                                                Name = Name,
                                                Details = StateContent,
                                                SubTime = DateTime.Now
                                            };
                                            jdjournalfacade.SaveJdJournal(jdjournaldto);
                                        }

                                    }
                                    #endregion
                                }

                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region 不拆单行为
                    var selectJdOrder1 = JdHelper.selectJdOrder1(jdPorderId);
                    if (!string.IsNullOrWhiteSpace(selectJdOrder1))
                    {
                        JObject objs = JObject.Parse(selectJdOrder1);
                        JArray objson = JArray.Parse(objs["sku"].ToString());
                        foreach (var ZiJdOrder in objson)
                        {
                            #region 子订单包含包含多个商品的情况
                            string skuId = ZiJdOrder["skuId"].ToString();
                            Guid TempId = Guid.Empty;
                            Guid commodityOrderItemId = Guid.Empty;
                            if (getFullOrderInfo.OrderItems != null && getFullOrderInfo.OrderItems.Count() > 0)
                            {
                                var commoditydto = getFullOrderInfo.OrderItems.Where(p => p.JdCode == skuId).FirstOrDefault();
                                if (commoditydto != null)
                                {
                                    commodityOrderItemId = commoditydto.Id;
                                    TempId = commoditydto.CommodityId;
                                }
                            }
                            int State = Convert.ToInt32(JdEnum.BCF);
                            string StateContent = new EnumHelper().GetDescription(JdEnum.BCF);
                            string JdOrderId = objs["jdOrderId"].ToString();
                            string Name = "京东下订单" + StateContent;
                            string Details = "订单状态由" + jdorderitem.State + "变成" + State;
                            jdorderitem.TempId = TempId;
                            jdorderitem.JdOrderId = JdOrderId;
                            jdorderitem.State = State;
                            jdorderitem.StateContent = StateContent;
                            jdorderitem.CommoditySkuId = skuId;
                            jdorderitem.CommodityOrderItemId = commodityOrderItemId;
                            var restult = jdorderitemfacade.UpdateJdOrderItem(jdorderitem);
                            if (restult.isSuccess == true)
                            {
                                JdJournalDTO ex = new JdJournalDTO()
                                {
                                    JdPorderId = jdPorderId,
                                    TempId = TempId,
                                    JdOrderId = JdOrderId,
                                    MainOrderId = jdorderitem.MainOrderId,
                                    CommodityOrderId = jdorderitem.CommodityOrderId,
                                    Name = Name,
                                    Details = StateContent
                                };
                                var res = jdjournalfacade.GetJdJournalList(ex);
                                if (res.Count() == 0)
                                {
                                    JdJournalDTO jdjournaldto = new JdJournalDTO()
                                    {
                                        Id = Guid.NewGuid(),
                                        JdPorderId = jdPorderId,
                                        TempId = TempId,
                                        JdOrderId = JdOrderId,
                                        MainOrderId = jdorderitem.MainOrderId,
                                        CommodityOrderId = jdorderitem.CommodityOrderId,
                                        Name = Name,
                                        Details = StateContent,
                                        SubTime = DateTime.Now
                                    };
                                    jdjournalfacade.SaveJdJournal(jdjournaldto);
                                }

                            }
                            #endregion
                        }
                    }
                    #endregion
                }

                #region 更新订单信息状态
                Jinher.AMP.BTP.Deploy.CommodityOrderDTO commodity = new Jinher.AMP.BTP.Deploy.CommodityOrderDTO();
                commodity.Id = Guid.Parse(jdorderitem.CommodityOrderId);
                commodity.State = 2;
                commodity.ShipmentsTime = DateTime.Now;
                //更新订单状态
                commodityorderfacade.UpdateCommodityOrder(commodity);
                #endregion

                return "ok";
            }
            return "no found";
        }
    }
}
