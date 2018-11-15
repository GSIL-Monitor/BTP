using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.TPS.Helper;
using System.Net;
using System.IO;
using Jinher.JAP.Common.Loging;
using Newtonsoft.Json;
using Jinher.JAP.PL;
using Jinher.AMP.BTP.Common;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using System.Data;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.Deploy.CustomDTO.ThirdECommerce;
using System.Threading.Tasks;

namespace Jinher.AMP.BTP.TPS
{
    public class FangZhengSV
    {
        public struct FangZhengData
        {
            /// <summary>
            /// 请求Url地址
            /// </summary>
            public string UrlBase { get; set; }
            /// <summary>
            /// 方正门店
            /// </summary>
            public string ShopId { get; set; }
            /// <summary>
            /// 方正密码
            /// </summary>
            public string Password { get; set; }
        }

        public static FangZhengData GetFangZhengInfo
        {
            get
            {
                var _UrlBase = CustomConfig.FangZheng_UrlBase;
                var _ShopId = CustomConfig.FangZheng_ShopId;
                var _Password = CustomConfig.FangZheng_Password;
                return new FangZhengData
                {
                    UrlBase = _UrlBase,
                    ShopId = _ShopId,
                    Password = _Password
                };
            }
        }
        /// <summary>
        /// 方正订单提交异常补救
        /// </summary>
        public static void FangZheng_Order_ForJobs()
        {
            try
            {
                var SubmitDate = DateTime.Now.AddHours(-1);
                var FangZhengAppIds = CustomConfig.FZAppIdList;
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                //对提交失败的订单重新提交
                FangZhengOrder.ObjectSet().Where(w => w.SubTime < SubmitDate)
                .Where(w => w.OrderStatus == 400 || w.OrderStatus == 900 || w.OrderStatus == 0)
                .Select(s => s.OrderId).Distinct().ToList()
                .ForEach(Item =>
                {
                    if (CommodityOrder.ObjectSet().Where(w => w.Id == Item && w.PaymentTime.HasValue && w.State == 1).Count() > 0)
                    {
                        FangZheng_Order_Confirm(Item);
                    }
                });
                //获得方正订单产品的 SN码 
                #region MyRegion
                //var StartTime = DateTime.Parse("2018-09-18");
                //CommodityOrder.ObjectSet().Where(w => w.SubTime > StartTime && w.ExpOrderNo == null &&
                //            w.PaymentTime.HasValue && FangZhengAppIds.Contains(w.AppId))
                //.ToList().ForEach(Item =>
                //{
                //    var ExpName = string.Empty;
                //    var ExoCode = string.Empty;
                //    var SnDir = new Dictionary<string, string>();

                //    var Result = FangZheng_Logistics_Status(Item.Id);
                //    if (Result["code"].ToString() == "200")
                //    {
                //        var ExpInfo = Result["data"]["logistics"].Children();
                //        ExpInfo.ToList().ForEach(model =>
                //        {
                //            if (!string.IsNullOrWhiteSpace(model["logisticsCompany"].ToString()))
                //                ExpName = model["logisticsCompany"].ToString();
                //            if (!string.IsNullOrWhiteSpace(model["logisticsOrderId"].ToString()))
                //                ExoCode = model["logisticsOrderId"].ToString();
                //        });
                //        ExpInfo["snInfo"].ToList().ForEach(model =>
                //        {
                //            var code = model.First["productCode"].ToString();
                //            var sn = string.Join(",", model.First["sn"]);
                //            SnDir.Add(code, sn);
                //        });
                //        Item.ShipExpCo = ExpName;
                //        Item.ExpOrderNo = ExoCode;
                //        OrderItem.ObjectSet().Where(w => w.CommodityOrderId == Item.Id)
                //        .ToList().ForEach(ItemChild =>
                //        {
                //            if (string.IsNullOrWhiteSpace(ItemChild.SNCode))
                //            {
                //                var SnStr = SnDir[ItemChild.JDCode];
                //                ItemChild.SNCode = SnStr;
                //            }
                //        });
                //        contextSession.SaveChange();
                //    }
                //}); 
                #endregion
                var StartTime = DateTime.Parse("2018-09-18");
                var orderItems = (from a in OrderItem.ObjectSet()
                                  join b in CommodityOrder.ObjectSet() on a.CommodityOrderId equals b.Id
                                  where a.SubTime > StartTime && b.SubTime > StartTime &&
                                        (a.SNCode == null || b.ExpOrderNo == null) &&
                                         b.PaymentTime.HasValue && FangZhengAppIds.Contains(b.AppId)
                                  select new { orderItem = a, commodityOrder = b }).ToList();
                var orderIds = orderItems.Select(p => p.orderItem.CommodityOrderId).Distinct().ToList();
                orderIds.ForEach(p =>
                {
                    try
                    {
                        var IsDelivery = false;
                        var snDir = new Dictionary<string, string>();
                        var Result = FangZheng_Logistics_Status(p);
                        if (Result["code"].ToString() == "200")
                        {
                            Result["data"]["logistics"].Children()["snInfo"]
                            .ToList().ForEach(model =>
                            {
                                var code = model.First["productCode"].ToString();
                                var sn = string.Join(",", model.First["sn"]);
                                snDir.Add(code, sn);
                            });
                            orderItems.Where(x => x.orderItem.CommodityOrderId == p)
                            .ToList().ForEach(x =>
                            {
                                if (snDir.ContainsKey(x.orderItem.JDCode))
                                {
                                    if (string.IsNullOrWhiteSpace(x.orderItem.SNCode))
                                        x.orderItem.SNCode = snDir[x.orderItem.JDCode];
                                    if (string.IsNullOrWhiteSpace(x.commodityOrder.ExpOrderNo))
                                    {
                                        Result["data"]["logistics"].Children()
                                        .ToList().ForEach(Item =>
                                        {
                                            if (!string.IsNullOrWhiteSpace(Item["logisticsCompany"].ToString()))
                                                x.commodityOrder.ShipExpCo = Item["logisticsCompany"].ToString();
                                            if (!string.IsNullOrWhiteSpace(Item["logisticsOrderId"].ToString()))
                                                x.commodityOrder.ExpOrderNo = Item["logisticsOrderId"].ToString();
                                        });
                                        IsDelivery = true;
                                    }
                                }
                            });
                            if (contextSession.SaveChange() >= 0)
                            {
                                //更改状态为已经发货
                                if (IsDelivery)
                                {
                                    var commodityorderfacade = new CommodityOrderFacade()
                                    {
                                        ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo()
                                    };
                                    var result = commodityorderfacade.UpdateCommodityOrder(
                                    new Jinher.AMP.BTP.Deploy.CommodityOrderDTO()
                                    {
                                        Id = p,
                                        State = 2,
                                        ShipmentsTime = DateTime.Now
                                    });
                                    if (result.ResultCode != 0)
                                    {
                                        LogHelper.Error("方正订单状态更改失败：OrderId" + p);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("方正电商接口请求异常：" + ex);
                    }
                });
            }
            catch (Exception ex)
            {
                LogHelper.Error("方正电商Job-Exception：" + ex);
            }
        }

        /// <summary>
        /// 保存方正订单与易捷数据关系
        /// </summary>
        public static void FangZheng_Order_Submit(ContextSession contextSession, OrderItem orderItem)
        {
            contextSession.SaveObject(new FangZhengOrder
            {
                Id = Guid.NewGuid(),
                OrderId = orderItem.CommodityOrderId,
                OrderItemId = orderItem.Id,
                SubTime = DateTime.Now,
                OrderStatus = 0,
                ModifiedOn = DateTime.Now,
                EntityState = EntityState.Added
            });
        }

        /// <summary>
        /// 获得方正电商物流状态数据
        /// </summary>
        public static JObject FangZheng_Logistics_Status(Guid OrderId)
        {
            var RequestParams = JsonConvert.SerializeObject(new
            {
                sign = GetMd5(GetFangZhengInfo.Password + TimeHelper.GetTimestampM() + GetFangZhengInfo.ShopId),
                shopId = GetFangZhengInfo.ShopId,
                reqTime = TimeHelper.GetTimestampM(),
                data = new
                {
                    orderId = OrderId.ToString()
                }
            });
            return SendRequest(GetFangZhengInfo.UrlBase + "/querylogistics", RequestParams);
        }

        /// <summary>
        /// 获得方正物流信息数据
        /// </summary>
        public static ThirdOrderPackageExpress FangZheng_Logistics_InfoList(Guid OrderId)
        {
            try
            {
                var OrderData = CommodityOrder.ObjectSet().Where(w => w.Id == OrderId).FirstOrDefault();
                if (string.IsNullOrWhiteSpace(OrderData.ExpOrderNo))
                    return null;
                var Logistics = FangZheng_Logistics_Data(OrderData.ExpOrderNo);
                var LogisticsList = (from data in Logistics["result"]["list"].Children()
                                     select new ThirdExpressTrace
                                     {
                                         Time = data["time"].ToString(),
                                         Desc = data["status"].ToString()
                                     }).ToList();
                return new ThirdOrderPackageExpress
                {
                    OrderId = OrderId.ToString(),
                    ExpressCompany = OrderData.ShipExpCo,
                    ExpressNo = OrderData.ExpOrderNo,
                    ExpressTraceList = LogisticsList
                };
            }
            catch (Exception ex)
            {
                var LogInfo = new Jinher.AMP.BTP.Deploy.JdlogsDTO
                {
                    Id = Guid.NewGuid(),
                    Content = "获得物流信息Exception：" + ex.Message,
                    SubTime = DateTime.Now,
                    ThirdECommerceType = (int)ThirdECommerceTypeEnum.FangZheng
                };
                new JdlogsFacade().SaveJdlogs(LogInfo);
                return null;
            }
        }
        /// <summary>
        /// 获得方正物流包裹信息
        /// </summary>
        public static List<ThirdOrderItemExpress> FangZheng_Logistics_Package(Guid OrderId)
        {
            var ReturnList = new List<ThirdOrderItemExpress>();
            FangZhengOrder.ObjectSet().Where(w => w.OrderId == OrderId)
            .ToList().ForEach(Item =>
            {
                var MyOrder = CommodityOrder.ObjectSet()
                    .Where(w => w.Id == Item.OrderId)
                    .FirstOrDefault();
                if (MyOrder != null)
                {
                    ReturnList.Add(new ThirdOrderItemExpress
                    {
                        OrderItemId = Item.OrderItemId,
                        ExpressNo = MyOrder.ExpOrderNo
                    });
                }
            });
            return ReturnList;
        }

        /// <summary>
        /// 提交订单数据接口到方正电商
        /// </summary>
        public static void FangZheng_Order_Confirm(Guid MyOrderId, bool IsMainOrder)
        {
            if (IsMainOrder)
            {
                MainOrder.ObjectSet().Where(r => r.MainOrderId == MyOrderId).ToList()
                .ForEach(Item =>
                {
                    FangZheng_Order_Confirm(Item.SubOrderId);
                });
            }
            else
            {
                FangZheng_Order_Confirm(MyOrderId);
            }
        }
        private static string FangZheng_Order_Confirm(string Name, CommodityOrder OrderInfo)
        {
            string ReutnValue = string.Empty;
            if (",北京市,天津市,上海市,重庆市,".IndexOf(OrderInfo.Province.Trim()) > 0)
            {
                switch (Name)
                {
                    case "省": ReutnValue = OrderInfo.Province; break;
                    case "市": ReutnValue = OrderInfo.Province; break;
                    case "区": ReutnValue = OrderInfo.City; break;
                }
            }
            else
            {
                switch (Name)
                {
                    case "省": ReutnValue = OrderInfo.Province; break;
                    case "市": ReutnValue = OrderInfo.City; break;
                    case "区": ReutnValue = OrderInfo.District; break;
                }
            }
            return ReutnValue;
        }
        private static void FangZheng_Order_Confirm(Guid MyOrderId)
        {
            Task.Factory.StartNew(() =>
            {
                var LogMessage = string.Empty;
                CommodityOrder MyOrder = null;
                try
                {
                    if (FangZhengOrder.ObjectSet().Where(w => w.OrderId == MyOrderId && w.OrderStatus == 200).Count() > 0)
                        return;
                    MyOrder = CommodityOrder.ObjectSet().Where(w => w.Id == MyOrderId)
                                            .Where(w => w.PaymentTime.HasValue)
                                            .Where(w => w.State == 1)
                                            .FirstOrDefault();
                    if (MyOrder != null)
                    {
                        Decimal OrderSumPrice = 0;
                        var CommodityList = new List<object>();
                        OrderItem.ObjectSet().Where(w => w.CommodityOrderId == MyOrderId)
                        .ToList().ForEach(Item =>
                        {
                            if (Item.AppId != null)
                            {
                                var AppId = (Guid)Item.AppId;
                                if (ThirdECommerceHelper.IsFangZheng(AppId))
                                {
                                    if (Item.CostPrice == null)
                                        throw new Exception("商品[" + Item.Name + "]价格为空！");
                                    CommodityList.Add(new
                                    {
                                        orderId = Item.CommodityOrderId,
                                        orderEntryId = Item.Id,
                                        productMapType = 1,
                                        productCode = Item.JDCode,
                                        productName = Item.Name,
                                        productNum = Item.Number,
                                        basePrice = Item.CostPrice,
                                        realPrice = Item.CostPrice,
                                        mallDiscountFee = 0,
                                        discountFee = 0
                                    });
                                    OrderSumPrice += Item.Number * (decimal)Item.CostPrice;
                                }
                            }
                            else
                            {
                                throw new Exception("订单中商品AppId为空！");
                            }
                        });
                        if (CommodityList.Count > 0 && OrderSumPrice > 0)
                        {
                            var RequestTime = TimeHelper.GetTimestampM();
                            var RequestParams = JsonConvert.SerializeObject(new
                            {
                                sign = GetMd5(GetFangZhengInfo.Password + RequestTime + GetFangZhengInfo.ShopId),
                                shopId = GetFangZhengInfo.ShopId,
                                reqTime = RequestTime,
                                data = new
                                {
                                    orderId = MyOrder.Id,
                                    orderStatus = 1,
                                    payTotalPrice = OrderSumPrice,
                                    totalPrice = OrderSumPrice,
                                    mallDiscountFee = 0,
                                    discountFee = 0,
                                    orderCreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                    remark = MyOrder.Details,
                                    orderEntryList = CommodityList,
                                    hasInvoice = true,//是否要发票
                                    invoiceTitle = "中国石化销售有限公司北京石油分公司",//发票抬头
                                    invoiceTitleType = 1,//0：个人,1：企业
                                    taxpayerNumber = "911100008011253056",//企业识别号                            
                                    invoiceEmail = "yijieds@126.com",//用户收发票的邮箱
                                    receiverinfo = new
                                    {
                                        receiverPhone = "",
                                        receiverName = MyOrder.ReceiptUserName,
                                        receiverMobile = MyOrder.ReceiptPhone,
                                        receiverProvince = FangZheng_Order_Confirm("省", MyOrder),
                                        receiverCity = FangZheng_Order_Confirm("市", MyOrder),
                                        receiverDistrct = FangZheng_Order_Confirm("区", MyOrder),
                                        receiverCounty = "",
                                        receiverAddress = MyOrder.ReceiptAddress,
                                        receiverZip = MyOrder.RecipientsZipCode
                                    }
                                }
                            });
                            var ResponseData = SendRequest(GetFangZhengInfo.UrlBase + "/submitorder", RequestParams);
                            ContextSession contextSession = ContextFactory.CurrentThreadContext;
                            FangZhengOrder.ObjectSet().Where(w => w.OrderId == MyOrderId)
                            .ToList().ForEach(Item =>
                            {
                                int Code = Convert.ToInt32(ResponseData["code"]);
                                if (Code != 200)
                                    LogMessage = Code + "-" + ResponseData["msg"].ToString();
                                Item.OrderStatus = Code;
                                Item.ModifiedOn = DateTime.Now;
                                Item.EntityState = EntityState.Modified;
                                contextSession.SaveObject(Item);
                            });
                            contextSession.SaveChange();
                        }
                        else
                        {
                            throw new Exception("订单商品总价为0元！");
                        }
                    }
                    else
                    {
                        throw new Exception("未查到易捷订单信息");
                    }
                }
                catch (Exception ex)
                {
                    LogMessage = ex.Message;
                    LogHelper.Error("方正电商订单提交接口Exception：" + ex);
                }
                finally
                {
                    if (!string.IsNullOrWhiteSpace(LogMessage))
                    {
                        var LogInfo = new Jinher.AMP.BTP.Deploy.JdlogsDTO
                        {
                            Id = Guid.NewGuid(),
                            Content = ",Exception：" + LogMessage,
                            SubTime = DateTime.Now,
                            AppId = MyOrder.AppId,
                            ThirdECommerceType = (int)ThirdECommerceTypeEnum.FangZheng
                        };
                        if (MyOrder == null)
                            LogInfo.Content = "订单主键：[" + MyOrderId + "]" + LogInfo.Content;
                        else
                            LogInfo.Content = "订单Code：[" + MyOrder.Code + "]" + LogInfo.Content;
                        new JdlogsFacade().SaveJdlogs(LogInfo);
                    }
                }
            });
        }

        private static JObject FangZheng_Logistics_Data(string ExpOrderNo)
        {
            string AppKey = CustomConfig.zshappkey;
            string Url = "http://api.jisuapi.com/express/query?appkey={0}&type={1}&number={2}";
            return SendRequest(string.Format(Url, AppKey, "auto", ExpOrderNo));
        }

        private static string GetMd5(string data)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            MD5 md5 = MD5.Create();
            byte[] retVal = md5.ComputeHash(buffer);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }

        private static JObject SendRequest(string Url)
        {
            var request = WebRequest.Create(Url);
            request.Method = "GET";
            request.Timeout = 1000 * 20;
            request.ContentType = "application/json;charset=utf-8";
            LogHelper.Info("方正电商订单提交接口RequestParams：" + Url);
            using (WebResponse response = request.GetResponse())
            {
                LogHelper.Info("方正电商订单提交接口WebResponse");
                using (Stream stream = response.GetResponseStream())
                {
                    LogHelper.Info("方正电商订单提交接口GetResponseStream");
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        var retStr = reader.ReadToEnd();
                        LogHelper.Info("方正电商订单提交接口GetResponse：" + retStr);
                        return (JObject)JsonConvert.DeserializeObject(retStr);
                    }
                }
            }
        }

        private static JObject SendRequest(string Url, string Params)
        {
            var request = WebRequest.Create(Url);
            request.Method = "POST";
            request.Timeout = 1000 * 20;
            request.ContentType = "application/json;charset=utf-8";
            var bArr = Encoding.UTF8.GetBytes(Params);
            var postStream = request.GetRequestStream();
            postStream.Write(bArr, 0, bArr.Length);
            LogHelper.Info("方正电商订单提交接口RequestParams：" + Params);
            using (WebResponse response = request.GetResponse())
            {
                LogHelper.Info("方正电商订单提交接口WebResponse");
                using (Stream stream = response.GetResponseStream())
                {
                    LogHelper.Info("方正电商订单提交接口GetResponseStream");
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        var retStr = reader.ReadToEnd();
                        LogHelper.Info("方正电商订单提交接口GetResponse：" + retStr);
                        return (JObject)JsonConvert.DeserializeObject(retStr);
                    }
                }
            }
        }


    }
}
