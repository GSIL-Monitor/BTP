using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.CustomDTO.YingKe;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;

namespace Jinher.AMP.BTP.BP
{
    /// <summary>
    /// 
    /// </summary>
    public partial class YJBJCardBP : BaseBP, IYJBJCard
    {
        #region 盈科接口相关

        private const string MapTmp = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
        private const string MapKey = "lC0FeA3K1g752GtSPqYaocTyEmBUhJwpRZVOf9dsNzuj4WQkiLxrbDvnXM8IH6";

        ///<summary>
        ///换位加密解密
        ///<param name="data">原数据</param>
        ///<param name="map">密钥map</param>
        ///<returns>加密后数据</returns>
        /// </summary>
        private String EncodeADecode(String data, string key, string tmp)
        {
            String result = "";
            try
            {
                Dictionary<Object, Object> map = getMap(key, tmp);
                byte[] bData = System.Text.Encoding.UTF8.GetBytes(data);
                byte[] nData = new byte[bData.Length];
                for (int i = 0; i < bData.Length; i++)
                {
                    if (map.Keys.Contains(bData[i]))
                    {
                        Byte b = (Byte)map[bData[i]];
                        nData[i] = b;
                    }
                    else
                    {
                        nData[i] = bData[i];
                    }
                }
                result = System.Text.Encoding.UTF8.GetString(nData);
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("换位加密解密异常" + ex.Message + ex.StackTrace));
            }
            return result;
        }

        ///<summary>
        ///<param name="key">密钥</param>
        ///<param name="tmp">原字符</param>
        /// </summary>
        private Dictionary<Object, Object> getMap(string key, string tmp)
        {
            Dictionary<Object, Object> map = new Dictionary<Object, Object>();
            try
            {
                byte[] bKey = System.Text.Encoding.UTF8.GetBytes(key);
                byte[] bTmp = System.Text.Encoding.UTF8.GetBytes(tmp);
                int len = bKey.Length;
                if (bKey.Length != bTmp.Length)
                {
                    Jinher.JAP.Common.Loging.LogHelper.Warn("密钥和原字符长度不一致！");
                }
                for (int i = 0; i < len; i++)
                {
                    map.Add(bKey[i], bTmp[i]);
                }
                if (map.Count != len)
                {
                    Jinher.JAP.Common.Loging.LogHelper.Warn("密钥内有重复字符！");
                }
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("换位加密解密map生成异常" + ex.Message + ex.StackTrace));
            }
            return map;
        }

        //返回数据解密
        private string GetDecData(string str)
        {
            string desStr = string.Empty;
            //decode
            str = str.Replace("\\n", "");

            desStr = EncodeADecode(str, MapKey, MapTmp);

            Jinher.JAP.Common.Loging.LogHelper.Debug("desStr:" + desStr);
            byte[] bufData = Convert.FromBase64String(desStr);

            //base64decode
            desStr = System.Text.Encoding.GetEncoding("UTF-8").GetString(bufData);
            return desStr;
        }

        //发送数据加密
        private string GetEncData(string str)
        {
            string encStr = string.Empty;
            //base64encode
            byte[] bufData = System.Text.Encoding.UTF8.GetBytes(str);
            encStr = Convert.ToBase64String(bufData);
            //encode
            encStr = EncodeADecode(encStr, MapTmp, MapKey);
            //urlencode
            //encStr = HttpUtility.UrlEncode(encStr, System.Text.Encoding.GetEncoding("UTF-8"));
            return encStr;
        }

        /// <summary>
        /// 调用盈科生成电子券接口
        /// </summary>
        /// <param name="yjCouponActivityId"></param>
        /// <param name="yjCouponType"></param>
        /// <param name="commodityNum"></param>
        /// <param name="yjUserId"></param>
        /// <returns></returns>
        private List<YJBJCard> CreateYingKeCoupon(string yjCouponActivityId, string yjCouponType, int commodityNum, string yjUserId)
        {
            var list = new List<YJBJCard>();
            var param = string.Format("yjCouponActivityId={0}&yjCouponType={1}&commodityNum={2}&yjUserId={3}", yjCouponActivityId, yjCouponType, commodityNum, yjUserId);
            try
            {
                var buCode = CustomConfig.YingKeBuCode;
                var secretKey = CustomConfig.YingKeSecretKey;
                var timeStamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
                var sign = string.Format("&buCode={0}&secretKey={1}&timeStamp={2}", buCode, secretKey, timeStamp);
                LogHelper.Info(string.Format("YJBJCardBP.CreateYingKeCoupon.param:{0}", sign));
                var data = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    swtid = yjCouponActivityId,
                    userid = yjUserId,
                    num = commodityNum.ToString(CultureInfo.InvariantCulture),
                    reserve1 = yjCouponType
                });
                LogHelper.Info(string.Format("YJBJCardBP.CreateYingKeCoupon.data加密前:{0}", data));
                data = GetEncData(data);
                LogHelper.Info(string.Format("YJBJCardBP.CreateYingKeCoupon.data加密后:{0}", data));
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    buCode,
                    data,
                    timeStamp,
                    sysSign = MD5Helper.GetMD5(sign, Encoding.UTF8)
                });
                LogHelper.Info(string.Format("YJBJCardBP.CreateYingKeCoupon.json:{0}", json));
                var request = WebRequest.Create(CustomConfig.YingKeGetCouponUrl);
                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";
                var bArr = Encoding.UTF8.GetBytes(json);
                request.GetRequestStream().Write(bArr, 0, bArr.Length);
                using (var response = request.GetResponse())
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        using (StreamReader reader = new StreamReader(responseStream, Encoding.UTF8))
                        {
                            var retStr = reader.ReadToEnd();
                            LogHelper.Info(string.Format("YJBJCardBP.CreateYingKeCoupon.retStr解密前:{0}", retStr));
                            if (!string.IsNullOrEmpty(retStr))
                            {
                                var result = JsonHelper.JsonDeserialize<YingKeResultDTO>(retStr);
                                if (result != null && !string.IsNullOrEmpty(result.msg))
                                {
                                    if (result.IsSuccess)
                                    {
                                        var msg = GetDecData(result.msg);
                                        LogHelper.Info(string.Format("YJBJCardBP.CreateYingKeCoupon.msg解密后:{0}", msg));
                                        var couponList = JsonHelper.JsonDeserialize<List<YingKeCouponDTO>>(msg);
                                        if (couponList != null && couponList.Count > 0)
                                        {
                                            #region 接口成功

                                            LogHelper.Info(string.Format("YJBJCardBP.CreateYingKeCoupon成功，输入：{0},输出{1}", param, retStr));
                                            couponList.ForEach(p =>
                                            {
                                                var yingkeNoAndPwdArr = p.rechargePwd.Split(',');
                                                list.Add(new YJBJCard
                                                {
                                                    CardName = p.presentName,
                                                    CardNo = yingkeNoAndPwdArr[0],
                                                    CheckCode = yingkeNoAndPwdArr.Length > 1 ? yingkeNoAndPwdArr[1] : string.Empty,
                                                    CouponUrl = p.preDetailPath,
                                                    EndTime = DateTime.Parse(p.endTime),
                                                    Status = 2,
                                                    Message = "成功",
                                                    EntityState = EntityState.Added
                                                });
                                            });
                                            return list;

                                            #endregion
                                        }
                                    }
                                    else
                                    {
                                        #region 接口失败-盈科错误

                                        list.Add(new YJBJCard
                                        {
                                            CardName = string.Empty,
                                            CardNo = string.Empty,
                                            CheckCode = string.Empty,
                                            CouponUrl = string.Empty,
                                            EndTime = DateTime.MaxValue,
                                            Status = 1,
                                            Message = result.msg,
                                            EntityState = EntityState.Added
                                        });
                                        return list;

                                        #endregion
                                    }
                                }
                            }
                            LogHelper.Error(string.Format("YJBJCardBP.CreateYingKeCoupon失败，输入：{0},输出{1}", param, retStr));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("YJBJCardBP.CreateYingKeCoupon异常，输入：{0}", param), ex);
            }
            return list;
        }

        #endregion

        public ResultDTO CreateExt(Guid orderId)
        {

            try
            {
                var order = CommodityOrder.ObjectSet().FirstOrDefault(p => p.Id == orderId);
                if (order == null) return new ResultDTO { isSuccess = false, Message = "未找到订单" };
                if (order.EsAppId != YJB.Deploy.CustomDTO.YJBConsts.YJAppId) return new ResultDTO { isSuccess = false, Message = "非易捷北京订单" };
                if (order.OrderType != 3) return new ResultDTO { isSuccess = false, Message = "非易捷卡密订单" };
                if (order.State == 7) return new ResultDTO { isSuccess = false, Message = "订单已退款" };
                if (YJBJCard.ObjectSet().Any(p => p.OrderId == orderId && p.Status == 2)) return new ResultDTO { isSuccess = false, Message = "已获取卡信息" };
                var yjUserId = TPS.CBCSV.GetYJUserId(order.UserId);
                if (string.IsNullOrEmpty(yjUserId)) return new ResultDTO { isSuccess = false, Message = "非易捷北京会员用户" };
                var isNoPhoneAccount = yjUserId == "NoPhoneAccount";
                //var yjUserId = "77fca49d080d4bcba3eca52e715e37f6";
                var orderItemList = OrderItem.ObjectSet()
                    .Where(p => p.CommodityOrderId == orderId && p.Type == 1).Select(p => new { p.Id, p.CommodityId, p.Number, p.YJCouponActivityId, p.YJCouponType, }).ToList()
                    .Where(p => !string.IsNullOrEmpty(p.YJCouponActivityId) && !string.IsNullOrEmpty(p.YJCouponType)).ToList();
                if (orderItemList.Count == 0) return new ResultDTO { isSuccess = false, Message = "未找到易捷卡密订单项" };
                orderItemList.ForEach(p =>
                {
                    var list = isNoPhoneAccount ? new List<YJBJCard>() : CreateYingKeCoupon(p.YJCouponActivityId, p.YJCouponType, p.Number, yjUserId);
                    if (list.Count == 0)//接口调用失败后再调用一次
                    {
                        list = isNoPhoneAccount ? new List<YJBJCard>() : CreateYingKeCoupon(p.YJCouponActivityId, p.YJCouponType, p.Number, yjUserId);
                        #region 接口二次调用失败

                        if (list.Count == 0)
                            list.Add(new YJBJCard
                            {
                                CardName = string.Empty,
                                CardNo = string.Empty,
                                CheckCode = string.Empty,
                                CouponUrl = string.Empty,
                                EndTime = DateTime.MaxValue,
                                Status = isNoPhoneAccount ? 1 : 0,
                                Message = isNoPhoneAccount ? "用户未绑定手机号" : "失败",
                                EntityState = EntityState.Added
                            });

                        #endregion
                    }
                    list.ForEach(x =>
                    {
                        x.Id = Guid.NewGuid();
                        x.UserId = order.UserId;
                        x.AppId = order.AppId;
                        x.EsAppId = (Guid)order.EsAppId;
                        x.OrderId = order.Id;
                        x.OrderCode = order.Code;
                        x.OrderItemId = p.Id;
                        x.CommodityId = p.CommodityId;
                        x.SubTime = DateTime.Now;
                        ContextFactory.CurrentThreadContext.SaveObject(x);
                    });
                });
                DateTime now = DateTime.Now;
                #region CommodityOrderService

                var commodityOrderService = CommodityOrderService.FindByID(order.Id);
                if (commodityOrderService == null)
                {
                    commodityOrderService = new CommodityOrderService
                    {
                        Id = order.Id,
                        Name = order.Name,
                        Code = order.Code,
                        State = 3,
                        SubId = order.UserId,
                        SubTime = now,
                        ModifiedOn = now,
                        EntityState = EntityState.Added
                    };
                    ContextFactory.CurrentThreadContext.SaveObject(commodityOrderService);
                }

                #endregion
                #region Journal

                var journal = new Journal
                {
                    Id = Guid.NewGuid(),
                    Name = "易捷卡密成功获取卡信息",
                    Code = order.Code,
                    SubId = order.UserId,
                    SubTime = now,
                    Details = "订单状态由" + order.State + "变为3",
                    StateFrom = order.State,
                    StateTo = 3,
                    IsPush = false,
                    OrderType = order.OrderType,
                    CommodityOrderId = order.Id,
                    EntityState = EntityState.Added
                };
                ContextFactory.CurrentThreadContext.SaveObject(journal);

                #endregion
                #region CommodityOrder

                order.State = 3;
                order.ModifiedOn = now;

                #endregion
                int count = ContextFactory.CurrentThreadContext.SaveChanges();
                return count == 0
                    ? new ResultDTO { isSuccess = false, Message = "数据库保存0行" }
                    : new ResultDTO { isSuccess = true, Message = "成功" };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("YJBJCardBP.CreateExt异常，orderId={0}", orderId), ex);
                return new ResultDTO { isSuccess = false, Message = "异常" };
            }
        }

        public List<YJBJCardDTO> GetExt(Guid orderId)
        {
            return YJBJCard.ObjectSet().Where(p => p.OrderId == orderId).ToList().Select(p => p.ToEntityData()).ToList();
        }
    }
}