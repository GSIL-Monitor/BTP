using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.CustomDTO.JdEclp;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.AMP.BTP.TPS;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Jinher.AMP.BTP.BP
{
    /// <summary>
    /// 进销存
    /// </summary>
    public partial class JdEclpOrderBP : BaseBP, IJdEclpOrder
    {
        private int GetZFCode(int payment)
        {
            switch (payment)
            {
                case 0:
                    return 707;
                case 1003:
                    return 705;
                case 1005:
                    return 61;
                case 1006:
                    return 702;
                case 2001:
                    return 706;
            }
            return -1;
        }

        private string GetZFName(int payment)
        {
            switch (payment)
            {
                case 0:
                    return "抵用券";
                case 1003:
                    return "支付宝";
                case 1005:
                    return "银联";
                case 1006:
                    return "微信";
                case 2001:
                    return "金采";
                default:
                    return "其他";
            }
        }

        /// <summary>
        /// 创建京东订单
        /// </summary>
        /// <param name="orderId">金和订单id</param>
        /// <param name="eclpOrderNo">京东订单编号,京东接口失败补录数据用</param>
        public void CreateOrderExt(Guid orderId, string eclpOrderNo)
        {
            eclpOrderNo = eclpOrderNo == null ? string.Empty : eclpOrderNo;
            string param = string.Format("orderId={0},eclpOrderNo={1}", orderId, eclpOrderNo);
            LogHelper.Debug(string.Format("JdEclpOrderBP.CreateOrderExt创建进销存京东订单,入参:{0}", param));

            try
            {
                LogHelper.Debug(string.Format("JdEclpOrderBP.CreateOrderExt创建进销存京东订单堆栈:{0}", new StackTrace(true)));
            }
            catch (Exception ex)
            {

            }

            if (orderId == Guid.Empty) return;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    #region 判断是否京东订单及获取订单信息
                    var order = CommodityOrder.ObjectSet().FirstOrDefault(o => o.Id == orderId);
                    if (order == null || !order.PaymentTime.HasValue || order.State != 1 || order.EsAppId != YJB.Deploy.CustomDTO.YJBConsts.YJAppId
                        || !order.AppType.HasValue || !new List<short> { 2, 3 }.Contains(order.AppType.Value)) return;
                    var itemList = OrderItem.ObjectSet()
                        .Where(p => p.CommodityOrderId == orderId && p.ErQiCode != null && p.ErQiCode != string.Empty)
                        .Select(p => new
                        {
                            p.Number,
                            p.ErQiCode
                        }).ToList();
                    if (itemList.Count == 0) return;
                    var jdOrder = JDEclpOrder.ObjectSet().Where(p => p.OrderId == orderId).FirstOrDefault();
                    if (jdOrder != null && jdOrder.EclpOrderState > 2000) return;
                    #endregion
                    #region 调用京东销售出库单接口
                    var erqiCodeList = itemList.Select(p => p.ErQiCode).ToList();
                    var numberList = itemList.Select(p => p.Number).ToList();
                    var jdResult = JdJosHelper.AddOrder(order.Id, order.ReceiptUserName, order.ReceiptPhone, order.Province, order.City, order.District, order.Street, order.ReceiptAddress, erqiCodeList, numberList);
                    if (jdResult.IsResultSuccess)
                    {
                        eclpOrderNo = jdResult.jingdong_eclp_order_addOrder_responce.eclpSoNo;
                    }
                    else if (!string.IsNullOrEmpty(eclpOrderNo))
                    {
                        jdResult.IsInterfaceSuccess = true;
                        jdResult.IsResultSuccess = true;
                    }
                    #endregion
                    #region 保存京东订单
                    var isNewOrder = false;
                    if (jdOrder == null)
                    {
                        isNewOrder = true;
                        jdOrder = new JDEclpOrder
                        {
                            Id = Guid.NewGuid(),
                            OrderId = order.Id,
                            OrderCode = order.Code,
                            EclpOrderNo = eclpOrderNo,
                            EclpOrderState = -1,
                            EclpOrderStateName = string.Empty,
                            OrderSubTime = order.SubTime,
                            PayTime = order.PaymentTime.Value,
                            SubId = order.SubId,
                            AppId = order.AppId,
                            EsAppId = order.EsAppId.Value,
                            AppName = order.AppName,
                            AppType = order.AppType,
                            SupplierName = order.SupplierName,
                            SupplierCode = order.SupplierCode,
                            SupplierType = order.SupplierType,
                            ShipperType = order.ShipperType,
                            EntityState = System.Data.EntityState.Added
                        };
                    }
                    jdOrder.EclpOrderNo = eclpOrderNo;
                    if (jdResult.IsResultSuccess) jdOrder.EclpOrderState = (int)JDEclpOrderStateEnum.AddEclpOrderResultSuccess;
                    else if (jdResult.IsInterfaceSuccess) jdOrder.EclpOrderState = (int)JDEclpOrderStateEnum.AddEclpOrderResultFail;
                    else jdOrder.EclpOrderState = (int)JDEclpOrderStateEnum.AddEclpOrderInterfaceFail;
                    jdOrder.EclpOrderStateName = new EnumHelper().GetDescription((JDEclpOrderStateEnum)jdOrder.EclpOrderState);
                    if (isNewOrder) ContextFactory.CurrentThreadContext.SaveObject(jdOrder);
                    #endregion
                    #region 保存京东订单日志
                    var jdJournal1 = new JDEclpOrderJournal
                    {
                        Id = Guid.NewGuid(),
                        OrderId = jdOrder.OrderId,
                        OrderCode = jdOrder.OrderCode,
                        EclpOrderNo = jdOrder.EclpOrderNo,
                        SubTime = DateTime.Now,
                        SubId = jdOrder.SubId,
                        Name = jdOrder.EclpOrderStateName,
                        Details = jdResult.Message,
                        StateFrom = -1,
                        StateTo = jdOrder.EclpOrderState,
                        EntityState = System.Data.EntityState.Added
                    };
                    ContextFactory.CurrentThreadContext.SaveObject(jdJournal1);
                    #endregion
                    int count = ContextFactory.CurrentThreadContext.SaveChanges();
                    if (count > 0)
                    {
                        #region 调整订单状态
                        if (jdResult.IsResultSuccess)
                        {
                            var expOrderNo = JdJosHelper.queryOrderPacks(eclpOrderNo);
                            var orderResult = new CommodityOrderBP().ShipUpdataOrderExt(orderId, "京东快递", expOrderNo);
                            if (orderResult.ResultCode != 0)//调整订单状态失败
                            {
                                LogHelper.Debug(string.Format("JdEclpOrderBP.CreateOrderExt调整订单状态失败:{0},入参:{1}", orderResult.Message ?? string.Empty, param));
                                var oldState = jdOrder.EclpOrderState;
                                jdOrder = JDEclpOrder.ObjectSet().FirstOrDefault(p => p.OrderId == jdOrder.OrderId);
                                jdOrder.EclpOrderState = (int)JDEclpOrderStateEnum.UpdateOrderStateFail;
                                jdOrder.EclpOrderStateName = new EnumHelper().GetDescription((JDEclpOrderStateEnum)jdOrder.EclpOrderState);
                                var jdJournal2 = new JDEclpOrderJournal
                                {
                                    Id = Guid.NewGuid(),
                                    OrderId = jdOrder.OrderId,
                                    OrderCode = jdOrder.OrderCode,
                                    EclpOrderNo = jdOrder.EclpOrderNo,
                                    SubTime = DateTime.Now,
                                    SubId = jdOrder.SubId,
                                    Name = jdOrder.EclpOrderStateName,
                                    Details = jdOrder.EclpOrderStateName,
                                    StateFrom = oldState,
                                    StateTo = jdOrder.EclpOrderState,
                                    EntityState = System.Data.EntityState.Added
                                };
                                ContextFactory.CurrentThreadContext.SaveObject(jdJournal2);
                                count = ContextFactory.CurrentThreadContext.SaveChanges();
                                if (count == 0) LogHelper.Error(string.Format("JdEclpOrderBP.CreateOrderExt保存日志失败,入参:{0}", param));
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        LogHelper.Error(string.Format("JdEclpOrderBP.CreateOrderExt创建进销存京东订单失败,入参:{0}", param));
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(string.Format("JdEclpOrderBP.CreateOrderExt创建进销存京东订单异常,入参:{0}", param), ex);
                }
            });
        }

        /// <summary>
        /// 发送支付信息到海信
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public void SendPayInfoToHaiXinExt(Guid orderId)
        {
            LogHelper.Debug(string.Format("JdEclpOrderBP.SendPayInfoToHaiXinExt发送支付数据到“海信”系统,入参:orderId={0}", orderId));
            if (orderId == Guid.Empty) return;
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(5 * 60 * 1000);
                var json = string.Empty;
                var hxJournal = default(HaiXinMqJournalDTO);
                try
                {
                    #region 判断是否京东订单及获取订单信息
                    var order = CommodityOrder.ObjectSet().FirstOrDefault(o => o.Id == orderId);
                    if (order == null || !order.PaymentTime.HasValue || order.EsAppId != YJB.Deploy.CustomDTO.YJBConsts.YJAppId
                        || !order.AppType.HasValue || !new List<short> { 2, 3 }.Contains(order.AppType.Value)) return;
                    var orderItemList = OrderItem.ObjectSet()
                        .Where(p => p.CommodityOrderId == orderId && p.ErQiCode != null && p.ErQiCode != string.Empty)
                        .Select(p => new
                        {
                            OrderItemId = p.Id,
                            OrderId = p.CommodityOrderId,
                            p.ErQiCode,
                            p.Name,
                            p.Barcode,
                            p.Number,
                            RealPrice = p.RealPrice.Value,
                            PayMoney = p.Number * (p.RealPrice ?? 0) - (p.CouponPrice ?? 0) - (p.YjbPrice ?? 0) - (p.ChangeRealPrice ?? 0),
                            DiscountMoney = (p.CouponPrice ?? 0) + (p.YjbPrice ?? 0) + (p.ChangeRealPrice ?? 0) + (p.ChangeFreightPrice ?? 0),
                            YJCouponMoney = p.YJCouponPrice ?? 0,
                            TaxRate = p.TaxRate ?? 0
                        }).ToList();
                    if (orderItemList.Count == 0) return;
                    var itemIdNoDir = new Dictionary<Guid, int>();
                    for (int i = 0; i < orderItemList.Count; i++)
                    {
                        itemIdNoDir.Add(orderItemList[i].OrderItemId, i);
                    }
                    #endregion
                    hxJournal = new HaiXinMqJournalDTO
                    {
                        Id = Guid.NewGuid(),
                        SubTime = DateTime.Now,
                        OrderId = order.Id,
                        OrderCode = order.Code,
                        Source = "JdEclpOrderBP.SendPayInfoToHaiXinExt",
                        Message = "支付数据",
                        Result = "发送成功"
                    };
                    #region 海信数据对象
                    var totalNumber = orderItemList.Sum(p => p.Number);
                    var tatalPayMoney = orderItemList.Sum(p => p.PayMoney);
                    var tatalDiscountMoney = orderItemList.Sum(p => p.DiscountMoney);
                    var tatalTaxMoney = orderItemList.Sum(p => Math.Round(p.PayMoney / (1 + p.TaxRate / 100) * (p.TaxRate / 100), 2));
                    var totalYJCouponMoney = orderItemList.Sum(p => p.YJCouponMoney);
                    var jsonObj = new
                    {
                        BILLNO = order.Code,
                        LRDATE = order.SubTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        USERID = "10000000061",
                        USERCODE = "110101",
                        USERNAME = "梁莉",
                        WLSTATUS = 1,
                        BILLTYPE = 0,
                        XSBILLNO = order.Code,
                        ORGCODE = "32550000",
                        ORGNAME = "北京石油分公司",
                        CKCODE = "JD01",
                        CKNAME = "正常仓",
                        ETPCODE = UserCodeForHaiXin.GetUserCode(order.UserId),
                        ETPNAME = "易捷北京",
                        ETPLINKNAME = (!string.IsNullOrEmpty(order.ReceiptUserName) && order.ReceiptUserName.Length > 10) ? order.ReceiptUserName.Substring(0, 10) : order.ReceiptUserName,
                        ETPTEL = order.ReceiptPhone,
                        ADDRESS = order.Province + order.City + order.District + order.Street + order.ReceiptAddress,
                        PFCOUNT = totalNumber,
                        FCNT = totalNumber,
                        PFTOTAL = tatalPayMoney,
                        YHTOTAL = tatalDiscountMoney,
                        SSTOTAL = tatalPayMoney,
                        XTAXTOTAL = tatalTaxMoney,
                        SKTOTAL = tatalPayMoney,
                        YFYSTOTAL = order.Freight,
                        YFSKTOTAL = order.Freight,
                        REMARK = string.Empty,
                        DATASOURCE = 2,
                        XSDETAIL = orderItemList.Select(p => new
                        {
                            BILLNO = order.Code,
                            SERIALNO = itemIdNoDir[p.OrderItemId] + 1,
                            PLUCODE = p.ErQiCode,
                            PLUNAME = p.Name,
                            BARCODE = p.Barcode,
                            PFCOUNT = p.Number,
                            PFPRICE = p.RealPrice,
                            YHTOTAL = p.DiscountMoney,
                            HSYSTOTAL = p.PayMoney,
                            SSTOTAL = p.PayMoney,
                            XTAXRATE = p.TaxRate,
                            XTAXTOTAL = Math.Round(p.PayMoney / (1 + p.TaxRate / 100) * (p.TaxRate / 100), 2),
                            REMARK = string.Empty,
                            FCNT = p.Number
                        }).ToList(),
                        XSDETAILPAY = (order.Payment != 0 && totalYJCouponMoney > 0) ?
                            new List<int> { 1, 2 }.Select(p => new
                            {
                                BILLNO = order.Code,
                                SERIALNO = p,
                                ZFCODE = p == 1 ? GetZFCode(order.Payment) : GetZFCode(0),
                                ZFNAME = p == 1 ? GetZFName(order.Payment) : GetZFName(0),
                                ZFNO = string.Empty,
                                SSTOTAL = p == 1 ? (tatalPayMoney - totalYJCouponMoney) : totalYJCouponMoney
                            }).ToList()
                        :
                            new List<int> { 1 }.Select(p => new
                            {
                                BILLNO = order.Code,
                                SERIALNO = p,
                                ZFCODE = GetZFCode(order.Payment),
                                ZFNAME = GetZFName(order.Payment),
                                ZFNO = string.Empty,
                                SSTOTAL = tatalPayMoney
                            }).ToList()
                    };
                    #endregion
                    json = JsonConvert.SerializeObject(jsonObj);
                    hxJournal.Json = json;
                    if (!TPS.YJBJMQSV.SendToMq("32550000_Jh_pfxs", json))
                    {
                        hxJournal.Result = "发送失败";
                        LogHelper.Error(string.Format("JdEclpOrderBP.SendPayInfoToHaiXinExt发送支付数据到“海信”系统失败,入参:orderId={0}", orderId));
                    }
                }
                catch (Exception ex)
                {
                    hxJournal.Result = "发送异常";
                    hxJournal.Message = ex.ToString();
                    LogHelper.Error(string.Format("JdEclpOrderBP.SendPayInfoToHaiXinExt发送支付数据到“海信”系统异常,入参:orderId={0}", orderId), ex);

                }
                finally
                {
                    if (hxJournal != null) new HaiXinMqJournal().Create(hxJournal);
                }
            });
        }

        /// <summary>
        /// 发送售中整单退款(拒收)信息到海信
        /// </summary>
        /// <param name="orderId"></param>
        public void SendRefundInfoToHaiXinExt(Guid orderId)
        {
            LogHelper.Debug(string.Format("JdEclpOrderBP.SendRefundInfoToHaiXinExt发送拒收退款数据到“海信”系统,入参:orderId={0}", orderId));
            if (orderId == Guid.Empty) return;
            Task.Factory.StartNew(() =>
            {
                var json = string.Empty;
                var hxJournal = default(HaiXinMqJournalDTO);
                try
                {
                    #region 判断是否京东订单及获取订单信息
                    var order = CommodityOrder.ObjectSet().FirstOrDefault(o => o.Id == orderId);
                    if (order == null || !order.PaymentTime.HasValue || order.EsAppId != YJB.Deploy.CustomDTO.YJBConsts.YJAppId
                        || !order.AppType.HasValue || !new List<short> { 2, 3 }.Contains(order.AppType.Value)) return;
                    var orderRefund = OrderRefund.ObjectSet().Where(p => p.OrderId == orderId && p.State == 1).FirstOrDefault();
                    if (orderRefund == null || (orderRefund.RefundMoney <= 0 && (orderRefund.RefundYJCouponMoney ?? 0) <= 0)) return;
                    var orderItemList = OrderItem.ObjectSet()
                        .Where(p => p.CommodityOrderId == orderId && p.ErQiCode != null && p.ErQiCode != string.Empty && p.RealPrice.HasValue)
                        .Select(p => new
                        {
                            OrderItemId = p.Id,
                            OrderId = p.CommodityOrderId,
                            p.ErQiCode,
                            p.Name,
                            p.Barcode,
                            p.Number,
                            RealPrice = p.RealPrice.Value,
                            PayMoney = p.Number * (p.RealPrice ?? 0) - (p.CouponPrice ?? 0) - (p.YjbPrice ?? 0) - (p.ChangeRealPrice ?? 0),
                            DiscountMoney = (p.CouponPrice ?? 0) + (p.YjbPrice ?? 0) + (p.ChangeRealPrice ?? 0) + (p.ChangeFreightPrice ?? 0),
                            TaxRate = p.TaxRate ?? 0
                        }).ToList();
                    if (orderItemList.Count == 0) return;
                    var itemIdNoDir = new Dictionary<Guid, int>();
                    for (int i = 0; i < orderItemList.Count; i++)
                    {
                        itemIdNoDir.Add(orderItemList[i].OrderItemId, i);
                    }
                    #endregion
                    hxJournal = new HaiXinMqJournalDTO
                    {
                        Id = Guid.NewGuid(),
                        SubTime = DateTime.Now,
                        OrderId = order.Id,
                        OrderCode = order.Code,
                        Source = "JdEclpOrderBP.SendRefundInfoToHaiXinExt",
                        Message = "拒收退款数据",
                        Result = "发送成功"
                    };
                    #region 拆分退款
                    var itemRefundMoneyDir = new Dictionary<Guid, Tuple<decimal, decimal>>();
                    var refundTotalDir = new Dictionary<Guid, decimal>(); //订单各项累计退款
                    var coIdListForRefund = orderItemList.Select(p => new { p.OrderId, p.OrderItemId }).ToList(); //订单id和订单项id关联记录
                    var orderRefundMoney = orderRefund.RefundMoney;//订单退款金额
                    var commodityPayMoney = orderItemList.Sum(x => x.PayMoney);//订单项实付金额合计
                    var commodityRefundMoney = orderRefundMoney > commodityPayMoney ? commodityPayMoney : orderRefundMoney;//订单项退款金额合计
                    orderItemList.ForEach(p =>
    {
        var refundMoney = default(decimal); //订单项退款金额
        var orderItemCount = coIdListForRefund.Count(x => x.OrderId == p.OrderId); //订单(剩余)订单项数量
        if (orderItemCount == 1)
        {
            if (refundTotalDir.ContainsKey(p.OrderId)) //订单中最后一种订单项退款金额 = 订单项退款金额合计-订单其余订单项累计退款金额
                refundMoney = commodityRefundMoney - refundTotalDir[p.OrderId];
            else //订单只包含一个订单项
                refundMoney = commodityRefundMoney;
        }
        else
        {
            //百分比计算退款金额，即 订单项退款金额=订单项退款金额合计*订单项实付金额/订单项实付金额合计
            refundMoney = Math.Round(commodityRefundMoney * p.PayMoney / commodityPayMoney);
            //每计算一次订单项退款金额则清理掉已计算的订单项，并将此订单项退款金额分别累计到对应的Dir
            coIdListForRefund.Remove(new { p.OrderId, p.OrderItemId });
            if (refundTotalDir.ContainsKey(p.OrderId))
                refundTotalDir[p.OrderId] += refundMoney;
            else
                refundTotalDir.Add(p.OrderId, refundMoney);
        }
        var taxMoney = -Math.Round(refundMoney / (1 + p.TaxRate / 100) * (p.TaxRate / 100), 2);
        itemRefundMoneyDir.Add(p.OrderItemId, new Tuple<decimal, decimal>(refundMoney, taxMoney));
    });
                    #endregion
                    #region 海信数据对象
                    var totalNumber = orderItemList.Sum(p => p.Number);
                    var tatalRefundMoney = orderRefund.RefundMoney;
                    var tatalRefundFreightMoney = tatalRefundMoney - commodityRefundMoney;
                    var tatalDiscountMoney = orderItemList.Sum(p => p.DiscountMoney);
                    var tatalTaxMoney = itemRefundMoneyDir.Values.Select(p => p.Item2).Sum();
                    var totalYJCouponRefundMoney = orderRefund.RefundYJCouponMoney ?? 0;
                    var jsonObj = new
                    {
                        BILLNO = orderRefund.Id,
                        LRDATE = orderRefund.ModifiedOn.ToString("yyyy-MM-dd HH:mm:ss"),
                        USERID = "10000000061",
                        USERCODE = "110101",
                        USERNAME = "梁莉",
                        WLSTATUS = 1,
                        BILLTYPE = 1,
                        XSBILLNO = order.Code,
                        ORGCODE = "32550000",
                        ORGNAME = "北京石油分公司",
                        CKCODE = "JD01",
                        CKNAME = "正常仓",
                        ETPCODE = UserCodeForHaiXin.GetUserCode(order.UserId),
                        ETPNAME = "易捷北京",
                        ETPLINKNAME = (!string.IsNullOrEmpty(order.ReceiptUserName) && order.ReceiptUserName.Length > 10) ? order.ReceiptUserName.Substring(0, 10) : order.ReceiptUserName,
                        ETPTEL = order.ReceiptPhone,
                        ADDRESS = order.Province + order.City + order.District + order.Street + order.ReceiptAddress,
                        PFCOUNT = totalNumber,
                        FCNT = totalNumber,
                        PFTOTAL = tatalRefundMoney,
                        YHTOTAL = tatalDiscountMoney,
                        SSTOTAL = tatalRefundMoney,
                        XTAXTOTAL = tatalTaxMoney,
                        SKTOTAL = tatalRefundMoney,
                        YFYSTOTAL = tatalRefundFreightMoney,
                        YFSKTOTAL = tatalRefundFreightMoney,
                        REMARK = string.Empty,
                        DATASOURCE = 2,
                        XSDETAIL = orderItemList.Select(p => new
                        {
                            BILLNO = orderRefund.Id,
                            SERIALNO = itemIdNoDir[p.OrderItemId] + 1,
                            PLUCODE = p.ErQiCode,
                            PLUNAME = p.Name,
                            BARCODE = p.Barcode,
                            PFCOUNT = p.Number,
                            PFPRICE = p.RealPrice,
                            YHTOTAL = p.DiscountMoney,
                            HSYSTOTAL = itemRefundMoneyDir[p.OrderItemId].Item1,
                            SSTOTAL = itemRefundMoneyDir[p.OrderItemId].Item1,
                            XTAXRATE = p.TaxRate,
                            XTAXTOTAL = itemRefundMoneyDir[p.OrderItemId].Item2,
                            REMARK = string.Empty,
                            FCNT = p.Number
                        }).ToList(),
                        XSDETAILPAY = tatalRefundMoney > 0 && totalYJCouponRefundMoney > 0 ?
                            new List<int> { 1, 2 }.Select(p => new
                            {
                                BILLNO = orderRefund.Id,
                                SERIALNO = p,
                                ZFCODE = p == 1 ? GetZFCode(order.Payment) : GetZFCode(0),
                                ZFNAME = p == 1 ? GetZFName(order.Payment) : GetZFName(0),
                                ZFNO = string.Empty,
                                SSTOTAL = p == 1 ? tatalRefundMoney : totalYJCouponRefundMoney
                            }).ToList()
                        :
                            new List<int> { 1 }.Select(p => new
                            {
                                BILLNO = orderRefund.Id,
                                SERIALNO = p,
                                ZFCODE = GetZFCode(order.Payment),
                                ZFNAME = GetZFName(order.Payment),
                                ZFNO = string.Empty,
                                SSTOTAL = tatalRefundMoney
                            }).ToList()
                    };
                    #endregion
                    json = JsonConvert.SerializeObject(jsonObj);
                    hxJournal.Json = json;
                    if (!TPS.YJBJMQSV.SendToMq("32550000_Jh_pfxs", json))
                    {
                        hxJournal.Result = "发送失败";
                        LogHelper.Error(string.Format("JdEclpOrderBP.SendRefundInfoToHaiXinExt发送拒收退款数据到“海信”系统失败,入参:orderId={0}", orderId));
                    }
                }
                catch (Exception ex)
                {
                    hxJournal.Result = "发送异常";
                    hxJournal.Message = ex.ToString();
                    LogHelper.Error(string.Format("JdEclpOrderBP.SendRefundInfoToHaiXinExt发送拒收退款数据到“海信”系统异常,入参:orderId={0}", orderId), ex);
                }
                finally
                {
                    if (hxJournal != null) new HaiXinMqJournal().Create(hxJournal);
                }
            });
        }

        /// <summary>
        /// 发送售中单品退款信息到海信
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderItemId"></param>
        public void SendSingleRefundInfoToHaiXinExt(Guid orderId, Guid orderItemId)
        {

        }

        /// <summary>   
        /// 发送售后整单退款信息到海信
        /// </summary>
        /// <param name="orderId"></param>
        public void SendASRefundInfoToHaiXinExt(Guid orderId)
        {

        }

        /// <summary>
        /// 发送售后单品退款信息到海信
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderItemId"></param>
        public void SendASSingleRefundInfoToHaiXinExt(Guid orderId, Guid orderItemId)
        {
            LogHelper.Debug(string.Format("JdEclpOrderBP.SendRefundInfoToHaiXinExt发送售后单品退款数据到“海信”系统,入参:orderId={0}", orderId));
            if (orderId == Guid.Empty) return;
            Task.Factory.StartNew(() =>
            {
                var json = string.Empty;
                var hxJournal = default(HaiXinMqJournalDTO);
                try
                {
                    #region 判断是否京东订单及获取订单信息
                    var order = CommodityOrder.ObjectSet().FirstOrDefault(o => o.Id == orderId);
                    if (order == null || !order.PaymentTime.HasValue || order.EsAppId != YJB.Deploy.CustomDTO.YJBConsts.YJAppId
                        || !order.AppType.HasValue || !new List<short> { 2, 3 }.Contains(order.AppType.Value)) return;
                    var orderRefund = OrderRefundAfterSales.ObjectSet()
                        .Where(p => p.OrderId == orderId && p.State == 1 && p.RefundMoney > 0 && p.OrderItemId.HasValue && p.OrderItemId.Value != Guid.Empty)
                        .OrderByDescending(p => p.SubTime).FirstOrDefault();
                    if (orderRefund == null) return;
                    orderItemId = orderRefund.OrderItemId.Value;
                    var orderItemList = OrderItem.ObjectSet()
                        .Where(p => p.CommodityOrderId == orderId && p.Id == orderItemId && p.ErQiCode != null && p.ErQiCode != string.Empty && p.RealPrice.HasValue)
                        .Select(p => new
                        {
                            p.ErQiCode,
                            p.Name,
                            p.Barcode,
                            p.Number,
                            RealPrice = p.RealPrice.Value,
                            DiscountMoney = (p.CouponPrice ?? 0) + (p.YjbPrice ?? 0) + (p.ChangeRealPrice ?? 0) + (p.ChangeFreightPrice ?? 0),
                            TaxRate = p.TaxRate ?? 0
                        }).ToList();
                    if (orderItemList.Count == 0) return;
                    #endregion
                    hxJournal = new HaiXinMqJournalDTO
                    {
                        Id = Guid.NewGuid(),
                        SubTime = DateTime.Now,
                        OrderId = order.Id,
                        OrderItemId = orderItemId,
                        OrderCode = order.Code,
                        Source = "JdEclpOrderBP.SendASSingleRefundInfoToHaiXinExt",
                        Message = "售后单品退款数据",
                        Result = "发送成功"
                    };
                    #region 海信数据对象
                    var totalNumber = orderItemList.Sum(p => p.Number);
                    var tatalRefundMoney = orderRefund.RefundMoney;
                    var tatalRefundFreightMoney = (orderRefund.RefundFreightPrice ?? 0) + (orderRefund.RefundChangeFreightPrice ?? 0);
                    var tatalDiscountMoney = orderItemList.Sum(p => p.DiscountMoney);
                    var tatalTaxMoney = Math.Round((tatalRefundMoney - tatalRefundFreightMoney) / (1 + orderItemList.Max(p => p.TaxRate) / 100) * (orderItemList.Max(p => p.TaxRate) / 100), 2);
                    var totalYJCouponRefundMoney = orderRefund.RefundYJCouponMoney ?? 0;
                    var jsonObj = new
                    {
                        BILLNO = orderRefund.Id,
                        LRDATE = orderRefund.ModifiedOn.ToString("yyyy-MM-dd HH:mm:ss"),
                        USERID = "10000000061",
                        USERCODE = "110101",
                        USERNAME = "梁莉",
                        WLSTATUS = 1,
                        BILLTYPE = 1,
                        XSBILLNO = order.Code,
                        ORGCODE = "32550000",
                        ORGNAME = "北京石油分公司",
                        CKCODE = "JD01",
                        CKNAME = "正常仓",
                        ETPCODE = UserCodeForHaiXin.GetUserCode(order.UserId),
                        ETPNAME = "易捷北京",
                        ETPLINKNAME = (!string.IsNullOrEmpty(order.ReceiptUserName) && order.ReceiptUserName.Length > 10) ? order.ReceiptUserName.Substring(0, 10) : order.ReceiptUserName,
                        ETPTEL = order.ReceiptPhone,
                        ADDRESS = order.Province + order.City + order.District + order.Street + order.ReceiptAddress,
                        PFCOUNT = totalNumber,
                        FCNT = totalNumber,
                        PFTOTAL = tatalRefundMoney,
                        YHTOTAL = tatalDiscountMoney,
                        SSTOTAL = tatalRefundMoney,
                        XTAXTOTAL = tatalTaxMoney,
                        SKTOTAL = tatalRefundMoney,
                        YFYSTOTAL = tatalRefundFreightMoney,
                        YFSKTOTAL = tatalRefundFreightMoney,
                        REMARK = string.Empty,
                        DATASOURCE = 2,
                        XSDETAIL = orderItemList.Select(p => new
                        {
                            BILLNO = orderRefund.Id,
                            SERIALNO = 1,
                            PLUCODE = p.ErQiCode,
                            PLUNAME = p.Name,
                            BARCODE = p.Barcode,
                            PFCOUNT = p.Number,
                            PFPRICE = p.RealPrice,
                            YHTOTAL = p.DiscountMoney,
                            HSYSTOTAL = tatalRefundMoney,
                            SSTOTAL = tatalRefundMoney,
                            XTAXRATE = p.TaxRate,
                            XTAXTOTAL = tatalTaxMoney,
                            REMARK = string.Empty,
                            FCNT = p.Number
                        }).ToList(),
                        XSDETAILPAY = tatalRefundMoney > 0 && totalYJCouponRefundMoney > 0 ?
                            new List<int> { 1, 2 }.Select(p => new
                            {
                                BILLNO = orderRefund.Id,
                                SERIALNO = p,
                                ZFCODE = p == 1 ? GetZFCode(order.Payment) : GetZFCode(0),
                                ZFNAME = p == 1 ? GetZFName(order.Payment) : GetZFName(0),
                                ZFNO = string.Empty,
                                SSTOTAL = p == 1 ? tatalRefundMoney : totalYJCouponRefundMoney
                            }).ToList()
                        :
                            new List<int> { 1 }.Select(p => new
                            {
                                BILLNO = orderRefund.Id,
                                SERIALNO = p,
                                ZFCODE = GetZFCode(order.Payment),
                                ZFNAME = GetZFName(order.Payment),
                                ZFNO = string.Empty,
                                SSTOTAL = tatalRefundMoney
                            }).ToList()
                    };
                    #endregion
                    json = JsonConvert.SerializeObject(jsonObj);
                    hxJournal.Json = json;
                    if (!TPS.YJBJMQSV.SendToMq("32550000_Jh_pfxs", json))
                    {
                        hxJournal.Result = "发送失败";
                        LogHelper.Error(string.Format("JdEclpOrderBP.SendRefundInfoToHaiXinExt发送售后单品退款数据到“海信”系统失败,入参:orderId={0}", orderId));
                    }
                }
                catch (Exception ex)
                {
                    hxJournal.Result = "发送异常";
                    hxJournal.Message = ex.ToString();
                    LogHelper.Error(string.Format("JdEclpOrderBP.SendRefundInfoToHaiXinExt发送售后单品退款数据到“海信”系统异常,入参:orderId={0}", orderId), ex);
                }
                finally
                {
                    if (hxJournal != null) new HaiXinMqJournal().Create(hxJournal);
                }
            });
        }

        /// <summary>
        /// 获取京东订单信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public JDEclpOrderDTO GetOrderInfoExt(Guid orderId)
        {
            if (orderId == Guid.Empty) return null;
            try
            {
                var order = JDEclpOrder.ObjectSet().FirstOrDefault(p => p.OrderId == orderId);
                if (order == null) return null;
                return order.ToEntityData();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("JdEclpOrderBP.GetOrderInfoExt获取进销存京东订单信息异常,入参:orderId={0}", orderId), ex);
            }
            return null;
        }

        /// <summary>
        /// 创建进销存京东订单售后信息
        /// </summary>
        /// <param name="orderId">金和订单id</param>
        /// <param name="orderItemId">金和订单项id</param>
        /// <param name="servicesNo">京东服务单编号,京东接口失败补录数据用</param>
        public void CreateJDEclpRefundAfterSalesExt(Guid orderId, Guid orderItemId, string servicesNo)
        {
            servicesNo = servicesNo == null ? string.Empty : servicesNo;
            string param = string.Format("orderId={0},orderItemId={1},servicesNo={2}", orderId, orderItemId, servicesNo);
            LogHelper.Debug(string.Format("JdEclpOrderBP.CreateJDEclpRefundAfterSalesExt创建进销存服务单,入参:{0}", param));
            if (orderId == Guid.Empty || orderItemId == Guid.Empty) return;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    #region 判断是否京东售后及获取售后信息
                    var jdOrder = JDEclpOrder.ObjectSet().Where(p => p.OrderId == orderId).FirstOrDefault();
                    if (jdOrder == null && jdOrder.EclpOrderState != (int)JDEclpOrderStateEnum.EclpOrderState10034) return;
                    var order = CommodityOrder.ObjectSet().Where(p => p.Id == orderId).Select(p => new
                    {
                        p.ReceiptUserName,
                        p.ReceiptPhone,
                        p.Province,
                        p.City,
                        p.District,
                        p.Street,
                        p.ReceiptAddress
                    }).FirstOrDefault();
                    if (order == null)
                    {
                        LogHelper.Error(string.Format("JdEclpOrderBP.CreateJDEclpRefundAfterSalesExt进销存服务单，未找到订单，入参:{0}", param));
                        return;
                    }
                    var orderRefund = OrderRefundAfterSales.ObjectSet()
                        .Where(p => p.OrderId == orderId && p.OrderItemId == orderItemId && new List<int> { 10, 11 }.Contains(p.State) && p.RefundType == 1
                            && p.RefundMoney > 0 && p.PickwareType.HasValue && p.AuditUserId.HasValue)
                        .OrderByDescending(p => p.SubTime).FirstOrDefault();
                    if (orderRefund == null)
                    {
                        LogHelper.Error(string.Format("JdEclpOrderBP.CreateJDEclpRefundAfterSalesExt进销存服务单，未找到售后，入参:{0}", param));
                        return;
                    }
                    var itemList = OrderItem.ObjectSet()
                        .Where(p => p.CommodityOrderId == orderId && p.ErQiCode != null && p.ErQiCode != string.Empty)
                        .Select(p => new
                        {
                            p.Number,
                            p.ErQiCode
                        }).ToList();
                    if (itemList.Count == 0)
                    {
                        LogHelper.Error(string.Format("JdEclpOrderBP.CreateJDEclpRefundAfterSalesExt进销存服务单，未找到订单项，入参:{0}", param));
                        return;
                    }
                    #endregion
                    #region 调用京东售后服务单接口
                    var jdEclpOrderRefundAfterSalesId = Guid.NewGuid();
                    var receiptUserName = order.ReceiptUserName;
                    var receiptPhone = order.ReceiptPhone;
                    var pickupAddress = string.Empty;
                    var refundExpOrderNo = string.Empty;
                    if (orderRefund.PickwareType.Value == 1)
                    {
                        receiptUserName = orderRefund.CustomerContactName;
                        receiptPhone = orderRefund.CustomerTel;
                        pickupAddress = orderRefund.PickwareAddress;
                    }
                    else if (orderRefund.PickwareType.Value == 2)
                    {
                        refundExpOrderNo = orderRefund.RefundExpOrderNo;
                    }
                    var consigneeAddress = order.Province + order.City + order.District + order.Street + order.ReceiptAddress;
                    var auditUserName = string.IsNullOrEmpty(orderRefund.AuditUserName) ? orderRefund.AuditUserId.ToString() : orderRefund.AuditUserName;
                    var erqiCodeList = itemList.Select(p => p.ErQiCode).ToList();
                    var numberList = itemList.Select(p => p.Number).ToList();
                    var jdResult = JdJosHelper.CreateServiceOrder(jdEclpOrderRefundAfterSalesId, jdOrder.EclpOrderNo, receiptUserName,
                        receiptPhone, consigneeAddress, orderRefund.AuditUserId.Value.ToString(), auditUserName, orderRefund.ModifiedOn, orderRefund.RefundDesc,
                        orderRefund.RefundReason, orderRefund.SalerRemark, erqiCodeList, numberList, pickupAddress, refundExpOrderNo);
                    if (jdResult.IsResultSuccess)
                    {
                        servicesNo = jdResult.jingdong_eclp_afs_createServiceOrder_response.servicesResult.serivcesNo;
                    }
                    else if (!string.IsNullOrEmpty(servicesNo))
                    {
                        jdResult.IsInterfaceSuccess = true;
                        jdResult.IsResultSuccess = true;
                    }
                    #endregion
                    #region 保存京东售后服务单
                    var isNewService = false;
                    var jdService = JDEclpOrderRefundAfterSales.ObjectSet().Where(p => p.OrderId == orderId && p.OrderItemId == orderItemId && p.EclpServicesNo == servicesNo).FirstOrDefault();
                    if (jdService == null)
                    {
                        isNewService = true;
                        jdService = new JDEclpOrderRefundAfterSales
                        {
                            Id = Guid.NewGuid(),
                            OrderId = jdOrder.OrderId,
                            OrderCode = jdOrder.OrderCode,
                            OrderRefundAfterSalesId = orderRefund.Id,
                            EclpOrderNo = jdOrder.EclpOrderNo,
                            EclpServicesNo = servicesNo,
                            EclpServicesState = -1,
                            EclpServicesStateName = string.Empty,
                            SubId = orderRefund.AuditUserId.Value,
                            SubTime = DateTime.Now,
                            OrderItemId = orderItemId,
                            PickwareType = orderRefund.PickwareType.Value,
                            EntityState = System.Data.EntityState.Added
                        };
                        if (jdService.PickwareType == 1)
                        {
                            jdService.CustomerContactName = receiptUserName;
                            jdService.CustomerTel = receiptPhone;
                            jdService.PickwareAddress = pickupAddress;
                        }
                    }
                    jdService.EclpServicesNo = servicesNo;
                    if (jdResult.IsResultSuccess) jdService.EclpServicesState = (int)JDEclpServicesStateEnum.AddEclpServiceResultSuccess;
                    else if (jdResult.IsInterfaceSuccess) jdService.EclpServicesState = (int)JDEclpServicesStateEnum.AddEclpServiceResultFail;
                    else jdService.EclpServicesState = (int)JDEclpServicesStateEnum.AddEclpServiceInterfaceFail;
                    jdService.EclpServicesStateName = new EnumHelper().GetDescription((JDEclpServicesStateEnum)jdService.EclpServicesState);
                    if (isNewService) ContextFactory.CurrentThreadContext.SaveObject(jdService);
                    #endregion
                    #region 修正售后退款表中JDEclpOrderRefundAfterSalesId字段
                    orderRefund.JDEclpOrderRefundAfterSalesId = jdService.Id;
                    #endregion
                    #region 保存京东售后服务单日志
                    var jdJournal1 = new JDEclpOrderRefundAfterSalesJournal
                    {
                        Id = Guid.NewGuid(),
                        OrderId = jdOrder.OrderId,
                        OrderCode = jdOrder.OrderCode,
                        OrderItemId = orderItemId,
                        OrderRefundAfterSalesId = orderRefund.Id,
                        EclpOrderNo = jdOrder.EclpOrderNo,
                        EclpServicesNo = jdService.EclpServicesNo,
                        SubId = jdService.SubId,
                        SubTime = DateTime.Now,
                        Name = jdService.EclpServicesStateName,
                        Details = jdResult.Message,
                        StateFrom = -1,
                        StateTo = jdService.EclpServicesState,
                        JDEclpOrderRefundAfterSalesId = jdService.Id,
                        EntityState = System.Data.EntityState.Added
                    };
                    ContextFactory.CurrentThreadContext.SaveObject(jdJournal1);
                    #endregion
                    int count = ContextFactory.CurrentThreadContext.SaveChanges();
                    if (count == 0) LogHelper.Error(string.Format("JdEclpOrderBP.CreateJDEclpRefundAfterSalesExt创建进销存服务单失败,入参:{0}", param));
                }
                catch (Exception ex)
                {
                    LogHelper.Error(string.Format("JdEclpOrderBP.CreateJDEclpRefundAfterSalesExt创建进销存服务单异常,入参:{0}", param), ex);
                }
            });
        }

        /// <summary>
        /// 判断是否进销存京东订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public bool ISEclpOrderExt(Guid orderId)
        {
            try
            {
                return CommodityOrder.ObjectSet().Any(o => o.Id == orderId && (o.AppType == 2 || o.AppType == 3)) && JDEclpOrder.ObjectSet().Any(o => o.OrderId == orderId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("判断是否进销存京东订单JdEclpOrderBP.ISEclpOrderExt异常orderId={0}", orderId), ex);
            }
            return false;
        }

        /// <summary>
        /// 获取JDECLP订单的信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderItemId"></param>
        /// <returns></returns>
        public JDEclpOrderRefundAfterSalesDTO GetJdEclpOrderRefundAfterSaleExt(Guid orderId, Guid orderItemId)
        {
            var dto = JDEclpOrderRefundAfterSales.ObjectSet().FirstOrDefault(o => o.OrderId == orderId && o.OrderItemId == orderItemId);
            if (dto != null)
            {
                return dto.ToEntityData();
            }
            return null;
        }

        /// <summary>
        /// 进销存-京东商品库存同步日志
        /// </summary>
        /// <param name="jdto"></param>
        /// <returns></returns>
        ResultDTO<List<JDStockJournalDTO>> GetJDStockJourneyListExt(JourneyDTO jdto)
        {
            var result = new ResultDTO<List<JDStockJournalDTO>>();
            var query = from data in JDStockJournal.ObjectSet()
                        select new JDStockJournalDTO()
                        {
                            AppId = data.AppId,
                            Id = data.Id,
                            SubTime = data.SubTime,
                            CommodityId = data.CommodityId,
                            CommodityStockId = data.CommodityStockId,
                            CommodityErQiCode = data.CommodityErQiCode,
                            CommodityNewStock = data.CommodityNewStock,
                            CommodityOldStock = data.CommodityOldStock,
                            CommodityStockErQiCode = data.CommodityStockErQiCode,
                            CommodityStockNewStock = data.CommodityStockNewStock,
                            CommodityStockOldStock = data.CommodityStockOldStock,
                            Code = data.Code,
                            Json = data.Json
                        };

            if (jdto.dtBegTime != null && jdto.dtBegTime != DateTime.MinValue)
            {
                query = query.Where(o => o.SubTime > jdto.dtBegTime);
            }

            if (jdto.dtEndTime != null && jdto.dtEndTime != DateTime.MaxValue)
            {
                query = query.Where(o => o.SubTime > jdto.dtBegTime);
            }

            query = query.OrderBy(o => o.SubTime).Skip((jdto.PageIndex - 1) * jdto.PageSize).Take(jdto.PageSize).OrderByDescending(o => o.SubTime); ;

            result.Data = query.ToList();
            return result;
        }

        /// <summary>
        /// 进销存-京东订单日志
        /// </summary>
        /// <param name="jdto"></param>
        /// <returns></returns>
        ResultDTO<List<JDEclpJourneyExtendDTO>> GetJDEclpOrderJournalListExt(JourneyDTO jdto)
        {
            var result = new ResultDTO<List<Deploy.CustomDTO.JdEclp.JDEclpJourneyExtendDTO>>();
            var query = from data in JDEclpOrderJournal.ObjectSet()
                        join m in JDEclpOrder.ObjectSet() on data.OrderId equals m.OrderId
                        select new JDEclpJourneyExtendDTO()
                        {
                            Id = data.Id,
                            SubTime = data.SubTime,
                            Json = data.Json,
                            Details = data.Details,
                            EclpOrderNo = data.EclpOrderNo,
                            Name = data.Name,
                            OrderCode = data.OrderCode,
                            OrderId = data.Id,
                            StateFrom = data.StateFrom,
                            StateTo = data.StateTo,
                            SubId = data.SubId,
                            AppName = m.AppName,
                            Code = data.Code,
                            SupplierName = m.SupplierName,
                            AppType = m.AppType
                        };

            if (jdto.dtBegTime != null && jdto.dtBegTime != DateTime.MinValue)
            {
                query = query.Where(o => o.SubTime > jdto.dtBegTime);
            }

            if (jdto.dtEndTime != null && jdto.dtEndTime != DateTime.MaxValue)
            {
                query = query.Where(o => o.SubTime > jdto.dtBegTime);
            }

            if (jdto.OrderID != null && jdto.OrderID != Guid.Empty)
            {
                query = query.Where(o => o.OrderId == jdto.OrderID);
            }

            if (!String.IsNullOrEmpty(jdto.OrderCode))
            {
                query = query.Where(o => o.OrderCode == jdto.OrderCode);
            }

            if (!String.IsNullOrEmpty(jdto.EclpOrderNo))
            {
                query = query.Where(o => o.EclpOrderNo == jdto.EclpOrderNo);
            }

            query = query.OrderBy(o => o.SubTime).Skip((jdto.PageIndex - 1) * jdto.PageSize).Take(jdto.PageSize).OrderByDescending(o => o.SubTime);
            result.Data = query.ToList();

            //foreach (var item in result.Data)
            //{
            //    switch (item.AppType)
            //    {
            //        case "0": item.AppType = "自营他配"; break;
            //        case "1": item.AppType = "第三方"; break;
            //        case "2": item.AppType = "自营自配自采"; break;
            //        case "3": item.AppType = "自营自配统采"; break;
            //        default: item.AppType = ""; break;
            //    }
            //}
            return result;
        }

        /// <summary>
        /// 进销存-京东服务单日志
        /// </summary>
        /// <param name="jdto"></param>
        /// <returns></returns>
        ResultDTO<List<JDEclpOrderRefundAfterSalesJournalDTO>> GetJDEclpOrderRefundAfterSalesJournalListExt(JourneyDTO jdto)
        {
            var result = new ResultDTO<List<JDEclpOrderRefundAfterSalesJournalDTO>>();
            var query = from data in JDEclpOrderRefundAfterSalesJournal.ObjectSet()
                        select new JDEclpOrderRefundAfterSalesJournalDTO()
                        {
                            Id = data.Id,
                            SubTime = data.SubTime,
                            Json = data.Json,
                            Details = data.Details,
                            EclpOrderNo = data.EclpOrderNo,
                            Name = data.Name,
                            OrderCode = data.OrderCode,
                            OrderId = data.Id,
                            StateFrom = data.StateFrom,
                            StateTo = data.StateTo,
                            SubId = data.SubId,
                            Code = data.Code,
                            OrderRefundAfterSalesId = data.OrderRefundAfterSalesId,
                            JDEclpOrderRefundAfterSalesId = data.JDEclpOrderRefundAfterSalesId,
                            EclpServicesNo = data.EclpServicesNo,
                            WarehouseName = data.WarehouseName,
                            WarehouseNo = data.WarehouseNo
                        };

            if (jdto.dtBegTime != null && jdto.dtBegTime != DateTime.MinValue)
            {
                query = query.Where(o => o.SubTime > jdto.dtBegTime);
            }

            if (jdto.dtEndTime != null && jdto.dtEndTime != DateTime.MaxValue)
            {
                query = query.Where(o => o.SubTime > jdto.dtBegTime);
            }

            query = query.OrderBy(o => o.SubTime).Skip((jdto.PageIndex - 1) * jdto.PageSize).Take(jdto.PageSize).OrderByDescending(O => O.SubTime);

            result.Data = query.ToList();
            return result;
        }

        /// <summary>
        /// 进销存-获取订单物流单号
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public ResultDTO<string> GetExpOrderNoExt(Guid orderId)
        {
            if (orderId == Guid.Empty) return new ResultDTO<string> { Message = "orderId有误" };
            var param = string.Format("orderId={0}", orderId);
            LogHelper.Debug(string.Format("JdEclpOrderBP.GetExpOrderNoExt获取进销存订单物流单号,入参:{0}", param));
            try
            {
                var order = CommodityOrder.ObjectSet().Where(p => p.Id == orderId).FirstOrDefault();
                if (order == null) return new ResultDTO<string> { Message = "未找到订单" };
                if (!string.IsNullOrEmpty(order.ExpOrderNo)) return new ResultDTO<string> { isSuccess = true, Data = order.ExpOrderNo };
                var eclpOrderNo = JDEclpOrder.ObjectSet().Where(p => p.OrderId == orderId).Select(p => p.EclpOrderNo).FirstOrDefault();
                if (string.IsNullOrEmpty(eclpOrderNo)) return new ResultDTO<string> { Message = "非进销存订单" };
                if (order.State == 1) order.State = 2;
                order.ShipmentsTime = DateTime.Now;
                order.ShipExpCo = "京东快递";
                order.ExpOrderNo = JdJosHelper.queryOrderPacks(eclpOrderNo);
                if (!string.IsNullOrEmpty(order.ExpOrderNo))
                {
                    var orderShipping = OrderShipping.ObjectSet().Where(p => p.OrderId == orderId).FirstOrDefault();
                    if (orderShipping == null)
                    {
                        orderShipping = OrderShipping.CreateOrderShipping();
                        orderShipping.OrderId = orderId;
                        orderShipping.ShipExpCo = order.ShipExpCo;
                        orderShipping.ExpOrderNo = order.ExpOrderNo;
                        ContextFactory.CurrentThreadContext.SaveObject(orderShipping);
                    }
                    else
                    {
                        orderShipping.ExpOrderNo = order.ExpOrderNo;
                    }
                    var orderExpressRoute = new OrderExpressRoute()
                    {
                        ShipExpCo = order.ShipExpCo,
                        ExpOrderNo = order.ExpOrderNo
                    };
                    new OrderExpressRouteBP().SubscribeOneOrderExpressExt(orderExpressRoute);
                    return new ResultDTO<string> { isSuccess = true, Data = order.ExpOrderNo };
                }
                return new ResultDTO<string> { Message = "京东接口未返回物流单号" };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("JdEclpOrderBP.GetExpOrderNoExt获取进销存订单物流单号异常,入参:{0}", param), ex);
                return new ResultDTO<string> { Message = ex.ToString() };
            }
        }

        /// <summary>
        /// 重新发送支付信息到海信
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public void RetranPayInfoToHaiXinExt(DateTime startTime, DateTime endTime)
        {
            LogHelper.Debug(string.Format("JdEclpOrderBP.RetranPayInfoToHaiXinExt“海信”系统,入参:startTime={0} endTime= {1}", startTime, endTime));
            List<HaiXinMqJournal> _lis = HaiXinMqJournal.ObjectSet().Where(p =>
            p.Source.Equals("JdEclpOrderBP.SendPayInfoToHaiXinExt") &&
            new string[] { "发送失败", "发送异常" }.Contains(p.Result) &&
            (p.SubTime >= startTime && p.SubTime <= endTime)).ToList();


            if (_lis != null && _lis.Count > 0)
            {
                foreach (var item in _lis)
                {
                    var json = string.Empty;
                    try
                    {
                        if (!string.IsNullOrEmpty(item.Json))
                        {
                            #region 判断是否京东订单及获取订单信息
                            var order = CommodityOrder.ObjectSet().FirstOrDefault(o => o.Id == item.OrderId);

                            var orderItemList = OrderItem.ObjectSet()
                                .Where(p => p.CommodityOrderId == item.OrderId && p.ErQiCode != null && p.ErQiCode != string.Empty)
                                .Select(p => new
                                {
                                    OrderItemId = p.Id,
                                    OrderId = p.CommodityOrderId,
                                    p.ErQiCode,
                                    p.Name,
                                    p.Barcode,
                                    p.Number,
                                    RealPrice = p.RealPrice.Value,
                                    PayMoney = p.Number * (p.RealPrice ?? 0) - (p.CouponPrice ?? 0) - (p.YjbPrice ?? 0) - (p.ChangeRealPrice ?? 0),
                                    DiscountMoney = (p.CouponPrice ?? 0) + (p.YjbPrice ?? 0) + (p.ChangeRealPrice ?? 0) + (p.ChangeFreightPrice ?? 0),
                                    TaxRate = p.TaxRate ?? 0
                                }).ToList();
                            if (orderItemList.Count == 0) return;
                            var itemIdNoDir = new Dictionary<Guid, int>();
                            for (int i = 0; i < orderItemList.Count; i++)
                            {
                                itemIdNoDir.Add(orderItemList[i].OrderItemId, i);
                            }
                            #endregion

                            #region 海信数据对象
                            var totalNumber = orderItemList.Sum(p => p.Number);
                            var tatalPayMoney = orderItemList.Sum(p => p.PayMoney);
                            var tatalDiscountMoney = orderItemList.Sum(p => p.DiscountMoney);
                            var tatalTaxMoney = orderItemList.Sum(p => Math.Round(p.PayMoney / (1 + p.TaxRate / 100) * (p.TaxRate / 100), 2));
                            var jsonObj = new
                            {
                                BILLNO = order.Code,
                                LRDATE = order.SubTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                USERID = "10000000061",
                                USERCODE = "110101",
                                USERNAME = "梁莉",
                                WLSTATUS = 1,
                                BILLTYPE = 0,
                                XSBILLNO = order.Code,
                                ORGCODE = "32550000",
                                ORGNAME = "北京石油分公司",
                                CKCODE = "JD01",
                                CKNAME = "正常仓",
                                ETPCODE = UserCodeForHaiXin.GetUserCode(order.UserId),
                                ETPNAME = "易捷北京",
                                ETPLINKNAME = (!string.IsNullOrEmpty(order.ReceiptUserName) && order.ReceiptUserName.Length > 10) ? order.ReceiptUserName.Substring(0, 10) : order.ReceiptUserName,
                                ETPTEL = order.ReceiptPhone,
                                ADDRESS = order.Province + order.City + order.District + order.Street + order.ReceiptAddress,
                                PFCOUNT = totalNumber,
                                FCNT = totalNumber,
                                PFTOTAL = tatalPayMoney,
                                YHTOTAL = tatalDiscountMoney,
                                SSTOTAL = tatalPayMoney,
                                XTAXTOTAL = tatalTaxMoney,
                                SKTOTAL = tatalPayMoney,
                                YFYSTOTAL = order.Freight,
                                YFSKTOTAL = order.Freight,
                                REMARK = string.Empty,
                                DATASOURCE = 2,
                                XSDETAIL = orderItemList.Select(p => new
                                {
                                    BILLNO = order.Code,
                                    SERIALNO = itemIdNoDir[p.OrderItemId] + 1,
                                    PLUCODE = p.ErQiCode,
                                    PLUNAME = p.Name,
                                    BARCODE = p.Barcode,
                                    PFCOUNT = p.Number,
                                    PFPRICE = p.RealPrice,
                                    YHTOTAL = p.DiscountMoney,
                                    HSYSTOTAL = p.PayMoney,
                                    SSTOTAL = p.PayMoney,
                                    XTAXRATE = p.TaxRate,
                                    XTAXTOTAL = Math.Round(p.PayMoney / (1 + p.TaxRate / 100) * (p.TaxRate / 100), 2),
                                    REMARK = string.Empty,
                                    FCNT = p.Number
                                }).ToList(),
                                XSDETAILPAY = new List<int> { 0 }.Select(p => new
                                {
                                    BILLNO = order.Code,
                                    SERIALNO = 1,
                                    ZFCODE = order.Payment,
                                    ZFNAME = GetZFName(order.Payment),
                                    ZFNO = string.Empty,
                                    SSTOTAL = tatalPayMoney
                                }).ToList()
                            };
                            #endregion
                            json = JsonConvert.SerializeObject(jsonObj);

                            if (!TPS.YJBJMQSV.SendToMq("32550000_Jh_pfxs", json))
                                item.Result = "发送成功";
                        }
                        else
                        {
                            if (TPS.YJBJMQSV.SendToMq("32550000_Jh_pfxs", item.Json))
                                item.Result = "发送成功";
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    finally
                    {
                        item.Update();
                    }
                }
            }
        }

        /// <summary>
        /// 重新发送售中整单退款信息到海信
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        public void RetranRefundInfoToHaiXinExt(DateTime startTime, DateTime endTime)
        {
            LogHelper.Debug(string.Format("JdEclpOrderBP.RetranRefundInfoToHaiXinExtExt“海信”系统,入参:startTime={0} endTime= {1}", startTime, endTime));
            List<HaiXinMqJournal> _lis = HaiXinMqJournal.ObjectSet().Where(p =>
            p.Source.Equals("JdEclpOrderBP.SendRefundInfoToHaiXinExt") &&
            new string[] { "发送失败", "发送异常" }.Contains(p.Result) &&
            (p.SubTime >= startTime && p.SubTime <= endTime)).ToList();


            if (_lis != null && _lis.Count > 0)
            {
                foreach (var item in _lis)
                {
                    var json = string.Empty;
                    try
                    {
                        if (!string.IsNullOrEmpty(item.Json))
                        {
                            #region 判断是否京东订单及获取订单信息
                            var order = CommodityOrder.ObjectSet().FirstOrDefault(o => o.Id == item.OrderId);

                            var orderRefund = OrderRefund.ObjectSet().Where(p => p.OrderId == item.OrderId && p.State == 1).FirstOrDefault();
                            if (orderRefund != null && orderRefund.RefundMoney <= 0) return;
                            var orderItemList = OrderItem.ObjectSet()
                                .Where(p => p.CommodityOrderId == item.OrderId && p.ErQiCode != null && p.ErQiCode != string.Empty && p.RealPrice.HasValue)
                                .Select(p => new
                                {
                                    OrderItemId = p.Id,
                                    OrderId = p.CommodityOrderId,
                                    p.ErQiCode,
                                    p.Name,
                                    p.Barcode,
                                    p.Number,
                                    RealPrice = p.RealPrice.Value,
                                    PayMoney = p.Number * (p.RealPrice ?? 0) - (p.CouponPrice ?? 0) - (p.YjbPrice ?? 0) - (p.ChangeRealPrice ?? 0),
                                    DiscountMoney = (p.CouponPrice ?? 0) + (p.YjbPrice ?? 0) + (p.ChangeRealPrice ?? 0) + (p.ChangeFreightPrice ?? 0),
                                    TaxRate = p.TaxRate ?? 0
                                }).ToList();
                            if (orderItemList.Count == 0) return;
                            var itemIdNoDir = new Dictionary<Guid, int>();
                            for (int i = 0; i < orderItemList.Count; i++)
                            {
                                itemIdNoDir.Add(orderItemList[i].OrderItemId, i);
                            }
                            #endregion
                            #region 拆分退款
                            var itemRefundMoneyDir = new Dictionary<Guid, Tuple<decimal, decimal>>();
                            var refundTotalDir = new Dictionary<Guid, decimal>(); //订单各项累计退款
                            var coIdListForRefund = orderItemList.Select(p => new { p.OrderId, p.OrderItemId }).ToList(); //订单id和订单项id关联记录
                            var orderRefundMoney = orderRefund.RefundMoney;//订单退款金额
                            var commodityPayMoney = orderItemList.Sum(x => x.PayMoney);//订单项实付金额合计
                            var commodityRefundMoney = orderRefundMoney > commodityPayMoney ? commodityPayMoney : orderRefundMoney;//订单项退款金额合计
                            orderItemList.ForEach(p =>
                            {
                                var refundMoney = default(decimal); //订单项退款金额
                                var orderItemCount = coIdListForRefund.Count(x => x.OrderId == p.OrderId); //订单(剩余)订单项数量
                                if (orderItemCount == 1)
                                {
                                    if (refundTotalDir.ContainsKey(p.OrderId)) //订单中最后一种订单项退款金额 = 订单项退款金额合计-订单其余订单项累计退款金额
                                        refundMoney = commodityRefundMoney - refundTotalDir[p.OrderId];
                                    else //订单只包含一个订单项
                                        refundMoney = commodityRefundMoney;
                                }
                                else
                                {
                                    //百分比计算退款金额，即 订单项退款金额=订单项退款金额合计*订单项实付金额/订单项实付金额合计
                                    refundMoney = Math.Round(commodityRefundMoney * p.PayMoney / commodityPayMoney);
                                    //每计算一次订单项退款金额则清理掉已计算的订单项，并将此订单项退款金额分别累计到对应的Dir
                                    coIdListForRefund.Remove(new { p.OrderId, p.OrderItemId });
                                    if (refundTotalDir.ContainsKey(p.OrderId))
                                        refundTotalDir[p.OrderId] += refundMoney;
                                    else
                                        refundTotalDir.Add(p.OrderId, refundMoney);
                                }
                                var taxMoney = -Math.Round(refundMoney / (1 + p.TaxRate / 100) * (p.TaxRate / 100), 2);
                                itemRefundMoneyDir.Add(p.OrderItemId, new Tuple<decimal, decimal>(refundMoney, taxMoney));
                            });
                            #endregion
                            #region 海信数据对象
                            var totalNumber = orderItemList.Sum(p => p.Number);
                            var tatalRefundMoney = orderRefund.RefundMoney;
                            var tatalRefundFreightMoney = tatalRefundMoney - commodityRefundMoney;
                            var tatalDiscountMoney = orderItemList.Sum(p => p.DiscountMoney);
                            var tatalTaxMoney = itemRefundMoneyDir.Values.Select(p => p.Item2).Sum();
                            var jsonObj = new
                            {
                                BILLNO = orderRefund.Id,
                                LRDATE = orderRefund.ModifiedOn.ToString("yyyy-MM-dd HH:mm:ss"),
                                USERID = "10000000061",
                                USERCODE = "110101",
                                USERNAME = "梁莉",
                                WLSTATUS = 1,
                                BILLTYPE = 1,
                                XSBILLNO = order.Code,
                                ORGCODE = "32550000",
                                ORGNAME = "北京石油分公司",
                                CKCODE = "JD01",
                                CKNAME = "正常仓",
                                ETPCODE = UserCodeForHaiXin.GetUserCode(order.UserId),
                                ETPNAME = "易捷北京",
                                ETPLINKNAME = (!string.IsNullOrEmpty(order.ReceiptUserName) && order.ReceiptUserName.Length > 10) ? order.ReceiptUserName.Substring(0, 10) : order.ReceiptUserName,
                                ETPTEL = order.ReceiptPhone,
                                ADDRESS = order.Province + order.City + order.District + order.Street + order.ReceiptAddress,
                                PFCOUNT = totalNumber,
                                FCNT = totalNumber,
                                PFTOTAL = tatalRefundMoney,
                                YHTOTAL = tatalDiscountMoney,
                                SSTOTAL = tatalRefundMoney,
                                XTAXTOTAL = tatalTaxMoney,
                                SKTOTAL = tatalRefundMoney,
                                YFYSTOTAL = tatalRefundFreightMoney,
                                YFSKTOTAL = tatalRefundFreightMoney,
                                REMARK = string.Empty,
                                DATASOURCE = 2,
                                XSDETAIL = orderItemList.Select(p => new
                                {
                                    BILLNO = orderRefund.Id,
                                    SERIALNO = itemIdNoDir[p.OrderItemId] + 1,
                                    PLUCODE = p.ErQiCode,
                                    PLUNAME = p.Name,
                                    BARCODE = p.Barcode,
                                    PFCOUNT = p.Number,
                                    PFPRICE = p.RealPrice,
                                    YHTOTAL = p.DiscountMoney,
                                    HSYSTOTAL = itemRefundMoneyDir[p.OrderItemId].Item1,
                                    SSTOTAL = itemRefundMoneyDir[p.OrderItemId].Item1,
                                    XTAXRATE = p.TaxRate,
                                    XTAXTOTAL = itemRefundMoneyDir[p.OrderItemId].Item2,
                                    REMARK = string.Empty,
                                    FCNT = p.Number
                                }).ToList(),
                                XSDETAILPAY = new List<int> { 0 }.Select(p => new
                                {
                                    BILLNO = orderRefund.Id,
                                    SERIALNO = 1,
                                    ZFCODE = order.Payment,
                                    ZFNAME = GetZFName(order.Payment),
                                    ZFNO = string.Empty,
                                    SSTOTAL = tatalRefundMoney
                                }).ToList()
                            };
                            #endregion
                            json = JsonConvert.SerializeObject(jsonObj);

                            if (!TPS.YJBJMQSV.SendToMq("32550000_Jh_pfxs", json))
                                item.Result = "发送成功";
                        }
                        else
                        {
                            if (TPS.YJBJMQSV.SendToMq("32550000_Jh_pfxs", item.Json))
                                item.Result = "发送成功";
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    finally
                    {
                        item.Update();
                    }
                }
            }
        }
        /// <summary>
        /// 重新发送售后单品退款信息到海信
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        public void RetranASSingleRefundInfoToHaiXinExt(DateTime startTime, DateTime endTime)
        {

            LogHelper.Debug(string.Format("JdEclpOrderBP.RetranASSingleRefundInfoToHaiXinExt“海信”系统,入参:startTime={0} endTime= {1}", startTime, endTime));
            List<HaiXinMqJournal> _lis = HaiXinMqJournal.ObjectSet().Where(p =>
            p.Source.Equals("JdEclpOrderBP.SendASSingleRefundInfoToHaiXinExt") &&
            new string[] { "发送失败", "发送异常" }.Contains(p.Result) &&
            (p.SubTime >= startTime && p.SubTime <= endTime)).ToList();

            if (_lis != null && _lis.Count > 0)
            {
                foreach (var item in _lis)
                {
                    var json = string.Empty;
                    try
                    {
                        if (!string.IsNullOrEmpty(item.Json))
                        {
                            #region 判断是否京东订单及获取订单信息
                            var order = CommodityOrder.ObjectSet().FirstOrDefault(o => o.Id == item.OrderId);
                            if (order == null || !order.PaymentTime.HasValue || order.EsAppId != YJB.Deploy.CustomDTO.YJBConsts.YJAppId
                                || !order.AppType.HasValue || !new List<short> { 2, 3 }.Contains(order.AppType.Value)) return;
                            var orderRefund = OrderRefundAfterSales.ObjectSet()
                                .Where(p => p.OrderId == item.OrderId && p.State == 1 && p.RefundMoney > 0 && p.OrderItemId.HasValue && p.OrderItemId.Value != Guid.Empty)
                                .OrderByDescending(p => p.SubTime).FirstOrDefault();
                            if (orderRefund == null) return;
                            item.OrderItemId = orderRefund.OrderItemId.Value;
                            var orderItemList = OrderItem.ObjectSet()
                                .Where(p => p.CommodityOrderId == item.OrderId && p.Id == item.OrderItemId && p.ErQiCode != null && p.ErQiCode != string.Empty && p.RealPrice.HasValue)
                                .Select(p => new
                                {
                                    p.ErQiCode,
                                    p.Name,
                                    p.Barcode,
                                    p.Number,
                                    RealPrice = p.RealPrice.Value,
                                    DiscountMoney = (p.CouponPrice ?? 0) + (p.YjbPrice ?? 0) + (p.ChangeRealPrice ?? 0) + (p.ChangeFreightPrice ?? 0),
                                    TaxRate = p.TaxRate ?? 0
                                }).ToList();
                            if (orderItemList.Count == 0) return;
                            #endregion

                            #region 海信数据对象
                            var totalNumber = orderItemList.Sum(p => p.Number);
                            var tatalRefundMoney = orderRefund.RefundMoney;
                            var tatalRefundFreightMoney = (orderRefund.RefundFreightPrice ?? 0) + (orderRefund.RefundChangeFreightPrice ?? 0);
                            var tatalDiscountMoney = orderItemList.Sum(p => p.DiscountMoney);
                            var tatalTaxMoney = Math.Round((tatalRefundMoney - tatalRefundFreightMoney) / (1 + orderItemList.Max(p => p.TaxRate) / 100) * (orderItemList.Max(p => p.TaxRate) / 100), 2);
                            var jsonObj = new
                            {
                                BILLNO = orderRefund.Id,
                                LRDATE = orderRefund.ModifiedOn.ToString("yyyy-MM-dd HH:mm:ss"),
                                USERID = "10000000061",
                                USERCODE = "110101",
                                USERNAME = "梁莉",
                                WLSTATUS = 1,
                                BILLTYPE = 1,
                                XSBILLNO = order.Code,
                                ORGCODE = "32550000",
                                ORGNAME = "北京石油分公司",
                                CKCODE = "JD01",
                                CKNAME = "正常仓",
                                ETPCODE = UserCodeForHaiXin.GetUserCode(order.UserId),
                                ETPNAME = "易捷北京",
                                ETPLINKNAME = (!string.IsNullOrEmpty(order.ReceiptUserName) && order.ReceiptUserName.Length > 10) ? order.ReceiptUserName.Substring(0, 10) : order.ReceiptUserName,
                                ETPTEL = order.ReceiptPhone,
                                ADDRESS = order.Province + order.City + order.District + order.Street + order.ReceiptAddress,
                                PFCOUNT = totalNumber,
                                FCNT = totalNumber,
                                PFTOTAL = tatalRefundMoney,
                                YHTOTAL = tatalDiscountMoney,
                                SSTOTAL = tatalRefundMoney,
                                XTAXTOTAL = tatalTaxMoney,
                                SKTOTAL = tatalRefundMoney,
                                YFYSTOTAL = tatalRefundFreightMoney,
                                YFSKTOTAL = tatalRefundFreightMoney,
                                REMARK = string.Empty,
                                DATASOURCE = 2,
                                XSDETAIL = orderItemList.Select(p => new
                                {
                                    BILLNO = orderRefund.Id,
                                    SERIALNO = 1,
                                    PLUCODE = p.ErQiCode,
                                    PLUNAME = p.Name,
                                    BARCODE = p.Barcode,
                                    PFCOUNT = p.Number,
                                    PFPRICE = p.RealPrice,
                                    YHTOTAL = p.DiscountMoney,
                                    HSYSTOTAL = tatalRefundMoney,
                                    SSTOTAL = tatalRefundMoney,
                                    XTAXRATE = p.TaxRate,
                                    XTAXTOTAL = tatalTaxMoney,
                                    REMARK = string.Empty,
                                    FCNT = p.Number
                                }).ToList(),
                                XSDETAILPAY = new List<int> { 0 }.Select(p => new
                                {
                                    BILLNO = orderRefund.Id,
                                    SERIALNO = 1,
                                    ZFCODE = order.Payment,
                                    ZFNAME = GetZFName(order.Payment),
                                    ZFNO = string.Empty,
                                    SSTOTAL = tatalRefundMoney
                                }).ToList()
                            };
                            #endregion
                            json = JsonConvert.SerializeObject(jsonObj);
                            if (!TPS.YJBJMQSV.SendToMq("32550000_Jh_pfxs", json))
                                item.Result = "发送成功";
                        }
                        else
                        {
                            if (TPS.YJBJMQSV.SendToMq("32550000_Jh_pfxs", item.Json))
                                item.Result = "发送成功";
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    finally
                    {
                        item.Update();
                    }
                }
            }
        }
    }
}
