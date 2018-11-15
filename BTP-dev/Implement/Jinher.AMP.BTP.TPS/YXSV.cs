using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.CustomDTO.YX;
using Jinher.JAP.Common.Loging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Jinher.AMP.BTP.TPS
{
    public class OrderedContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return base.CreateProperties(type, memberSerialization).OrderBy(p => p.PropertyName).ToList();
        }
    }

    /// <summary>
    /// 严选接口
    /// </summary>
    public static class YXSV
    {
        static readonly string ApiUrl;
        static readonly string ApiCallBackUrl;
        static readonly string AppKey;
        static readonly string AppSecret;
        public static readonly bool IsProduction = false;

        static YXSV()
        {
            var isProduction = ConfigurationManager.AppSettings["Production"];
            if (string.IsNullOrEmpty(isProduction))
            {
                IsProduction = false;
            }
            else
            {
                IsProduction = isProduction == "1";
            }
            if (IsProduction)
            {
                ApiUrl = "http://openapi.you.163.com/channel/api.json";
                AppKey = "2a40b2fa2ce947fe92a62caba0e4e2b9";
                AppSecret = "dbbd6e8cacfc4d28892dd97bf5e615ed";
            }
            else
            {
                ApiUrl = "http://openapi-test.you.163.com/channel/api.json";
                ApiCallBackUrl = "http://openapi-test.you.163.com/mock/api/v1/";
                AppKey = "d38f2b18e86442e8ac1fe2df0d318eae";
                AppSecret = "8029e27e67884f5296d55cdba2558edd";
            }
        }

        /// <summary>
        /// 全量获取SPU(商品ID查询接口yanxuan.item.id.batch.get)
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllSPU()
        {
            try
            {
                string postData = GenerateParameter("yanxuan.item.id.batch.get", new Dictionary<string, string> { });
                string jsonstr = WebRequestHelper.SendPostInfo(ApiUrl, postData);
                LogHelper.Info("YXSV.GetAllSPU 全量获取SPU，Request: " + postData + "，Response: " + jsonstr);

                var resullt = JsonConvert.DeserializeObject<YXResult<List<string>>>(jsonstr);
                if (resullt.Code == "200")
                {
                    return resullt.Result;
                }
                else
                {
                    LogHelper.Error("YXSV.GetAllSPU 全量获取SPU 失败，Request: " + postData + "，Response: " + resullt.Code + "-" + resullt.Msg);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXSV.GetAllSPU 全量获取SPU 异常", ex);
            }
            return new List<string>();
        }

        /// <summary>
        /// 批量查询严选商品库存(商品SKU库存查询接口yanxuan.inventory.count.get)
        /// </summary>
        /// <returns></returns>
        public static List<StockDTO> GetStockNum(List<string> skuIds)
        {
            try
            {
                string postData = GenerateParameter("yanxuan.inventory.count.get", new Dictionary<string, string> { { "skuIds", JsonConvert.SerializeObject(skuIds) } });
                string jsonstr = WebRequestHelper.SendPostInfo(ApiUrl, postData);
                LogHelper.Info("YXSV.GetStockNum 获取严选库存，Request: " + postData + "，Response: " + jsonstr);

                var resullt = JsonConvert.DeserializeObject<YXResult<string>>(jsonstr);
                if (resullt.Code == "200" && !string.IsNullOrEmpty(resullt.Result))
                {
                    return JsonConvert.DeserializeObject<List<StockDTO>>(resullt.Result);
                }
                else if (resullt.Code == "20201" && resullt.Msg != null)
                {
                    resullt.Msg.Replace("未选品", "").Replace("sku:", "").Split(',').ToList().ForEach(p =>
                    {
                        skuIds.Remove(p);
                    });
                    return GetStockNum(skuIds);
                }
                else
                {
                    LogHelper.Error("YXSV.GetStockNum 批量查询严选商品库存 失败，Request: " + postData + "，Response: " + resullt.Code + "-" + resullt.Msg);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXSV.GetStockNum 批量查询严选库存异常", ex);
            }
            return new List<StockDTO>();
        }

        /// <summary>
        /// 批量查询严选商品详情(商品信息查询接口yanxuan.item.batch.get)
        /// </summary>
        /// <returns></returns>
        public static List<YXComDetailDTO> GetComDetailList(List<string> skuId)
        {
            try
            {
                string skuIds = string.Join(",", skuId.ToArray());
                string postData = GenerateParameter("yanxuan.item.batch.get", new Dictionary<string, string> { { "itemIds", skuIds } });
                string jsonstr = WebRequestHelper.SendPostInfo(ApiUrl, postData);
                LogHelper.Info("YXSV.GetComDetailList 获取商品详情，Request: " + postData);

                var resullt = JsonConvert.DeserializeObject<YXResult<List<YXComDetailDTO>>>(jsonstr);

                if (resullt.Code == "200")
                {
                    return resullt.Result;
                }
                else
                {
                    LogHelper.Error("YXSV.GetComDetailList 批量查询严选商品详情 失败，Request: " + postData + "，Response: " + resullt.Code + "-" + resullt.Msg);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXSV.GetComDetailList 批量查询严选商品详情异常", ex);
            }
            return new List<YXComDetailDTO>();
        }
        /// <summary>
        /// 渠道下单接口(yanxuan.order.paid.create)
        /// </summary>
        public static OrderOut CreatePaidOrder(OrderVO order, ref string jsonstr)
        {
            try
            {
                var settings = new JsonSerializerSettings()
                {
                    ContractResolver = new OrderedContractResolver()
                };
                var orderJson = JsonConvert.SerializeObject(order, Formatting.Indented, settings);
                orderJson = orderJson.Replace("\r\n", "");
                string postData = GenerateParameter("yanxuan.order.paid.create", new Dictionary<string, string> { { "order", orderJson } });
                jsonstr = WebRequestHelper.SendPostInfo(ApiUrl, postData);
                LogHelper.Info("YXSV.CreatePaidOrder 渠道下单，Request: " + postData + "，Response: " + jsonstr);
                var result = JsonConvert.DeserializeObject<YXResult<string>>(jsonstr);
                if (result.Successed && !string.IsNullOrEmpty(result.Result))
                {
                    return JsonConvert.DeserializeObject<OrderOut>(result.Result);
                }
                else
                {
                    LogHelper.Error("YXSV.CreatePaidOrder 渠道下单失败，Request: " + postData + "，Response: " + jsonstr);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXSV.CreatePaidOrder 渠道下单异常，Request: " + JsonConvert.SerializeObject(order), ex);
            }
            return null;
        }

        /// <summary>
        /// 渠道取消订单接口(yanxuan.order.paid.cancel)
        /// </summary>
        /// <param name="orderId">订单号</param>
        public static ResultDTO<OrderCancelResult> CancelPaidOrder(string orderId, ref string jsonstr)
        {
            if (string.IsNullOrEmpty(orderId)) return new ResultDTO<OrderCancelResult> { Message = "参数有误" };
            orderId = orderId.ToLower();
            try
            {
                string postData = GenerateParameter("yanxuan.order.paid.cancel", new Dictionary<string, string> { { "orderId", orderId } });
                jsonstr = WebRequestHelper.SendPostInfo(ApiUrl, postData);
                LogHelper.Info("YXSV.CancelPaidOrder 渠道取消订单，Request: " + postData + "，Response: " + jsonstr);
                var result = JsonConvert.DeserializeObject<YXResult<string>>(jsonstr);
                if (result.Successed)
                {
                    return new ResultDTO<OrderCancelResult> { isSuccess = true, Data = JsonConvert.DeserializeObject<OrderCancelResult>(result.Result) };
                }
                else
                {
                    LogHelper.Error("YXSV.CancelPaidOrder 渠道取消订单失败，Request: " + postData + "，Response: " + jsonstr);
                    return new ResultDTO<OrderCancelResult> { isSuccess = false, ResultCode = int.Parse(result.Code), Message = result.Msg };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXSV.CancelPaidOrder 渠道取消订单异常，Request: " + orderId, ex);
                return new ResultDTO<OrderCancelResult> { isSuccess = false, Message = ex.Message };
            }
        }

        /// <summary>
        /// 渠道订单确认收货接口(yanxuan.order.received.confirm)
        /// </summary>
        /// <param name="orderId">订单号</param>
        /// <param name="packageId">订单包裹号</param>
        /// <param name="confirmTime">签收时间</param>
        public static ResultDTO<string> ConfirmReceivedOrder(string orderId, string packageId, DateTime confirmTime)
        {
            if (string.IsNullOrEmpty(orderId) || string.IsNullOrEmpty(packageId)) return new ResultDTO<string> { Message = "参数有误" };
            orderId = orderId.ToLower();
            try
            {
                string postData = GenerateParameter("yanxuan.order.received.confirm", new Dictionary<string, string> { 
                    { "orderId", orderId }, { "packageId", packageId }, { "confirmTime", confirmTime.ToString("yyyy-MM-dd HH:mm:ss") } 
                });
                string jsonstr = WebRequestHelper.SendPostInfo(ApiUrl, postData);
                LogHelper.Info("YXSV.ConfirmReceivedOrder 渠道订单确认收货，Request: " + postData + "，Response: " + jsonstr);
                var resultDto = new ResultDTO<string> { Data = jsonstr };
                var result = JsonConvert.DeserializeObject<YXResult>(jsonstr);
                if (result.Successed)
                {
                    resultDto.isSuccess = true;
                    return resultDto;
                }
                else
                {
                    LogHelper.Error("YXSV.ConfirmReceivedOrder 渠道订单确认收货失败，Request: " + postData + "，Response: " + jsonstr);
                    resultDto.Message = result.Msg;
                    return resultDto;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXSV.ConfirmReceivedOrder 渠道订单确认收货异常，Request: " + orderId + "_" + packageId + "_" + confirmTime, ex);
                return new ResultDTO<string> { Message = ex.Message };
            }
        }

        /// <summary>
        /// 渠道订单信息查询接口(yanxuan.order.paid.get)
        /// </summary>
        public static OrderOut GetPaidOrder(string orderId, ref string jsonstr)
        {
            if (string.IsNullOrEmpty(orderId)) return null;
            orderId = orderId.ToLower();
            try
            {
                string postData = GenerateParameter("yanxuan.order.paid.get", new Dictionary<string, string> { { "orderId", orderId } });
                jsonstr = WebRequestHelper.SendPostInfo(ApiUrl, postData);
                LogHelper.Info("YXSV.GetPaidOrder 渠道订单信息查询，Request: " + postData + "，Response: " + jsonstr);
                var result = JsonConvert.DeserializeObject<YXResult<string>>(jsonstr);
                if (result.Successed && !string.IsNullOrEmpty(result.Result))
                {
                    return JsonConvert.DeserializeObject<OrderOut>(result.Result);
                }
                else
                {
                    LogHelper.Error("YXSV.GetPaidOrder 渠道订单信息查询失败，Request: " + postData + "，Response: " + jsonstr);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXSV.GetPaidOrder 渠道订单信息查询异常，Request: " + orderId, ex);
            }
            return null;
        }

        /// <summary>
        /// 获取物流轨迹信息接口(yanxuan.order.express.get)
        /// </summary>
        /// <param name="orderId">订单号</param>
        /// <param name="packageId">包裹号</param>
        /// <param name="expressNo">物流单号</param>
        /// <param name="expressCom">物流公司名称 zbs</param>
        /// <returns></returns>
        public static DeliveryInfo GetExpressOrder(string orderId, string packageId, string expressNo, string expressCom)
        {
            if (string.IsNullOrEmpty(orderId) || string.IsNullOrEmpty(packageId)
                || string.IsNullOrEmpty(expressNo) || string.IsNullOrEmpty(expressCom)) return null;
            orderId = orderId.ToLower();
            try
            {
                string postData = GenerateParameter("yanxuan.order.express.get", new Dictionary<string, string> { 
                    { "orderId", orderId }, { "packageId", packageId }, { "expressNo", expressNo }, { "expressCom", expressCom } 
                });
                string jsonstr = WebRequestHelper.SendPostInfo(ApiUrl, postData);
                LogHelper.Info("YXSV.GetExpressOrder 获取物流轨迹信息，Request: " + postData + "，Response: " + jsonstr);
                var result = JsonConvert.DeserializeObject<YXResult<string>>(jsonstr);
                if (result.Successed && !string.IsNullOrEmpty(result.Result))
                {
                    return JsonConvert.DeserializeObject<DeliveryInfo>(result.Result);
                }
                else
                {
                    LogHelper.Error("YXSV.GetExpressOrder 获取物流轨迹信息，Request: " + postData + "，Response: " + jsonstr);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXSV.GetExpressOrder 获取物流轨迹信息，Request: " + orderId + "_" + packageId + "_" + expressNo + "_" + expressCom, ex);
            }
            return null;
        }

        /// <summary>
        /// 渠道发起售后服务请求(yanxuan.order.refund.apply)
        /// </summary>
        public static ResultDTO<OrderRefundApplyResponse> ApplyRefundOrder(ApplyInfo info)
        {
            try
            {
                string postData = GenerateParameter("yanxuan.order.refund.apply", new Dictionary<string, string> { { "applyInfo", JsonConvert.SerializeObject(info) } });
                string jsonstr = WebRequestHelper.SendPostInfo(ApiUrl, postData);
                LogHelper.Info("YXSV.ApplyRefundOrder 渠道发起售后服务请求，Request: " + postData + "，Response: " + jsonstr);
                var result = JsonConvert.DeserializeObject<YXResult<string>>(jsonstr);
                if (result.Successed)
                {
                    return new ResultDTO<OrderRefundApplyResponse> { isSuccess = true, Data = JsonConvert.DeserializeObject<OrderRefundApplyResponse>(result.Result) };
                }
                else
                {
                    LogHelper.Error("YXSV.ApplyRefundOrder 渠道发起售后服务请求失败，Request: " + postData + "，Response: " + jsonstr);
                    return new ResultDTO<OrderRefundApplyResponse> { isSuccess = false, ResultCode = 1, Message = result.Msg };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXSV.ApplyRefundOrder 渠道发起售后服务请求异常，Request: " + JsonConvert.SerializeObject(info), ex);
                return new ResultDTO<OrderRefundApplyResponse> { isSuccess = false, ResultCode = 1, Message = "请求失败，请稍后重试。" };
            }

        }

        /// <summary>
        /// 渠道取消售后服务请求(yanxuan.order.refund.cancel)
        /// </summary>
        public static ResultDTO CancelRefundOrder(string applyId)
        {
            try
            {
                string postData = GenerateParameter("yanxuan.order.refund.cancel", new Dictionary<string, string> { { "applyId", applyId } });
                string jsonstr = WebRequestHelper.SendPostInfo(ApiUrl, postData);
                LogHelper.Info("YXSV.CancelRefundOrder 渠道取消售后服务请求，Request: " + postData + "，Response: " + jsonstr);
                var result = JsonConvert.DeserializeObject<YXResult>(jsonstr);
                if (result.Successed)
                {
                    return new ResultDTO { isSuccess = true };
                }
                else
                {
                    LogHelper.Error("YXSV.CancelRefundOrder 渠道取消售后服务请求失败，Request: " + postData + "，Response: " + jsonstr);
                    return new ResultDTO { isSuccess = false, Message = result.Msg };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXSV.CancelRefundOrder 渠道取消售后服务请求异常，Request: " + applyId, ex);
                return new ResultDTO { isSuccess = false, Message = ex.Message };
            }
        }

        /// <summary>
        /// 渠道绑定售后寄回物流单号(yanxuan.order.refund.offer.express)
        /// </summary>
        public static ResultDTO ExpressOfferRefundOrder(string applyId, List<ExpressInfo> expressInfo)
        {
            try
            {
                expressInfo.ForEach(_ => _.trackingCompany = ConvertExpressName(_.trackingCompany));
                string postData = GenerateParameter("yanxuan.order.refund.offer.express", new Dictionary<string, string> { { "applyId", applyId }, { "expressInfo", JsonConvert.SerializeObject(expressInfo) } });
                string jsonstr = WebRequestHelper.SendPostInfo(ApiUrl, postData);
                LogHelper.Info("YXSV.ExpressOfferRefundOrder 渠道绑定售后寄回物流单号，Request: " + postData + "，Response: " + jsonstr);
                var result = JsonConvert.DeserializeObject<YXResult>(jsonstr);
                if (result.Successed)
                {
                    return new ResultDTO { isSuccess = true };
                }
                else
                {
                    LogHelper.Error("YXSV.ExpressOfferRefundOrder 渠道绑定售后寄回物流单号请求失败，Request: " + postData + "，Response: " + jsonstr);
                    return new ResultDTO { isSuccess = false, Message = result.Msg };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXSV.ExpressOfferRefundOrder 渠道绑定售后寄回物流单号请求异常，Request: " + JsonConvert.SerializeObject(expressInfo), ex);
                return new ResultDTO { isSuccess = false, Message = ex.Message };
            }
        }

        /// <summary>
        /// 渠道查询售后申请详情(yanxuan.order.refund.detail.get)
        /// </summary>
        public static RefundApply GetDetailRefundOrder(string applyId)
        {
            try
            {
                string postData = GenerateParameter("yanxuan.order.refund.detail.get", new Dictionary<string, string> { { "applyId", applyId } });
                string jsonstr = WebRequestHelper.SendPostInfo(ApiUrl, postData);
                LogHelper.Info("YXSV.GetDetailRefundOrder 渠道查询售后申请详情，Request: " + postData + "，Response: " + jsonstr);
                var result = JsonConvert.DeserializeObject<YXResult<string>>(jsonstr);
                if (result.Successed)
                {
                    return JsonConvert.DeserializeObject<RefundApply>(result.Result);
                }
                else
                {
                    LogHelper.Error("YXSV.GetDetailRefundOrder 渠道查询售后申请详情请求失败，Request: " + postData + "，Response: " + jsonstr);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXSV.GetDetailRefundOrder 渠道查询售后申请详情请求异常，Request: " + applyId, ex);
            }
            return null;
        }

        static string GenerateParameter(string method, Dictionary<string, string> paras)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("appKey", AppKey);
            dic.Add("method", method);
            dic.Add("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            paras.ToList().ForEach(x =>
            {
                dic.Add(x.Key, string.IsNullOrEmpty(x.Value) ? x.Value : x.Value.Replace("+", " ").Replace("%", " "));
            });
            dic = dic.OrderBy(_ => _.Key).ToDictionary(_ => _.Key, _ => _.Value);

            //参数值拼接的字符串收尾添加appSecret值
            var waitSignStr = AppSecret + string.Join("", dic.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value))) + AppSecret;
            LogHelper.Debug("YXSV.GenerateParameter 计算sign前 waitSignStr=" + waitSignStr);

            dic.Add("sign", MD5Helper.GetMD5(waitSignStr, Encoding.UTF8).ToUpper());

            return string.Join("&", dic.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));
        }

        static string GetSign(string method, string timestamp, Dictionary<string, string> paras)
        {
            var dic = new SortedDictionary<string, string>();
            dic.Add("appKey", AppKey);
            dic.Add("method", method);
            dic.Add("timestamp", timestamp);
            paras.ToList().ForEach(x => dic.Add(x.Key, x.Value));

            //参数值拼接的字符串收尾添加appSecret值
            var waitSignStr = AppSecret + string.Join("", dic.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value))) + AppSecret;
            LogHelper.Debug("YXSV.GenerateParameter 计算sign前 waitSignStr=" + waitSignStr);

            return MD5Helper.GetMD5(waitSignStr, Encoding.UTF8).ToUpper();
        }

        /// <summary>
        /// 校验回调参数
        /// </summary>
        /// <param name="sign"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public static bool CheckSign(YXSign sign, Dictionary<string, string> paras)
        {
            if (sign == null || string.IsNullOrEmpty(sign.method) || string.IsNullOrEmpty(sign.appKey)
                || string.IsNullOrEmpty(sign.timestamp) || string.IsNullOrEmpty(sign.sign)) return false;
            LogHelper.Info("YXSV.CheckSign 校验回调参数，Request: " + JsonConvert.SerializeObject(sign));
            if (sign.appKey != AppKey) return false;
            var signStr = GetSign(sign.method, sign.timestamp, paras);
            if (sign.sign != signStr) return false;
            return true;
        }

        /// <summary>
        /// 渠道自助注册回调
        /// </summary>
        /// <param name="methods">多个方法名用英文逗号分隔，覆盖原来所有方法</param>
        /// <returns></returns>
        public static bool RegisterCallbackMethod(string methods)
        {
            if (string.IsNullOrEmpty(methods)) return false;
            try
            {
                string postData = GenerateParameter("yanxuan.callback.method.register", new Dictionary<string, string> { { "methods", methods } });
                string jsonstr = WebRequestHelper.SendPostInfo(ApiUrl, postData);
                LogHelper.Info("YXSV.RegisterCallbackMethod 注册回调方法，Request: " + postData + "，Response: " + jsonstr);
                var result = JsonConvert.DeserializeObject<YXResult<string>>(jsonstr);
                if (result.Successed)
                {
                    return true;
                }
                else
                {
                    LogHelper.Error("YXSV.RegisterCallbackMethod 注册回调方法失败，Request: " + postData + "，Response: " + jsonstr);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXSV.RegisterCallbackMethod 注册回调方法异常，Request: " + methods, ex);
            }
            return false;
        }

        /// <summary>
        /// 获取渠道已注册的回调方法名
        /// </summary>
        /// <returns></returns>
        public static List<string> GetCallbackMethodList()
        {
            try
            {
                string postData = GenerateParameter("yanxuan.callback.method.list", new Dictionary<string, string>());
                string jsonstr = WebRequestHelper.SendPostInfo(ApiUrl, postData);
                LogHelper.Info("YXSV.GetCallbackMethodList 获取已注册回调方法，Request: " + postData + "，Response: " + jsonstr);
                var result = JsonConvert.DeserializeObject<YXResult<string>>(jsonstr);
                if (result.Successed && !string.IsNullOrEmpty(result.Result))
                {
                    return JsonConvert.DeserializeObject<List<string>>(result.Result);
                }
                else
                {
                    LogHelper.Error("YXSV.GetCallbackMethodList 获取已注册回调方法失败，Request: " + postData + "，Response: " + jsonstr);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXSV.GetCallbackMethodList 获取已注册回调方法异常", ex);
            }
            return new List<string>();
        }

        private static string ConvertExpressName(string trackingCompany)
        {
            switch (trackingCompany)
            {
                case "顺丰速运":
                    return "顺丰";
                case "申通快递":
                    return "申通";
                case "圆通速递":
                    return "圆通";
                case "EMS快递":
                    return "EMS";
                case "中通速递":
                    return "中通";
                case "韵达快运":
                    return "韵达";
                case "天天快递":
                    return "天天";
                // 不支持
                case "中邮物流":
                    return "中邮物流";
                case "汇通快运":
                    return "百世汇通";
                case "宅急送":
                    return "宅急送";
                case "快捷快递":
                    return "快捷";
                case "德邦物流":
                    return "德邦";
                case "优速物流":
                    return "优速";
                case "全峰快递":
                    return "全峰";
                // 不支持
                case "信丰物流":
                    return "信丰物流";
                case "国通快递":
                    return "国通";
                // 不支持
                case "港中能达物流":
                    return "港中能达物流";
                // 不支持
                case "联昊通物流":
                    return "联昊通物流";
                // 不支持
                case "全日通快递":
                    return "全日通快递";
                // 不支持
                case "佳吉物流":
                    return "佳吉物流";
                case "速尔物流":
                    return "速尔";
                case "天地华宇":
                    return "天地华宇";
                // 不支持
                case "佳怡物流":
                    return "佳怡物流";
                // 不支持
                case "如风达":
                    return "如风达";
                case "新邦物流":
                    return "新邦物流";
                // 不支持
                case "安捷快递":
                    return "";
                // 不支持
                case "UPS":
                    return "UPS";
                // 不支持
                case "龙邦物流":
                    return "龙邦物流";
                // 不支持
                case "联邦快递（国内）":
                    return "联邦快递（国内）";
                // 不支持
                case "DHL":
                    return "DHL";
                // 不支持
                case "全一快递":
                    return "全一快递";
                // 不支持
                case "TNT":
                    return "TNT";
                // 不支持
                case "运通快递":
                    return "运通快递";
                case "京东快递":
                    return "京东快递";
                case "其他":
                    return "其他";
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// 查询SKU对应商品信息
        /// </summary>
        /// <param name="skuId"></param>
        /// <returns></returns>
        public static APIitemTO GetSkuInfo(string skuId)
        {
            try
            {
                string postData = GenerateParameter("yanxuan.item.sku.get", new Dictionary<string, string> { { "skuId", skuId } });
                string jsonstr = WebRequestHelper.SendPostInfo(ApiUrl, postData);
                LogHelper.Info("YXSV.GetSkuInfo 查询SKU对应商品信息，Request: " + postData + "，Response: " + jsonstr);
                var result = JsonConvert.DeserializeObject<YXResult<APIitemTO>>(jsonstr);
                if (result.Code == "200")
                {
                    return result.Result;
                }
                else
                {
                    LogHelper.Error("YXSV.GetSkuInfo 查询SKU对应商品信息 失败，Request: " + postData + "，Response: " + result.Code + "-" + result.Msg);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXSV.GetSkuInfo 查询SKU对应商品信息异常", ex);
            }
            return null;
        }

        #region 只用于测试环境，触发严选回调

        /// <summary>
        /// 触发包裹物流绑单回调
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static bool CallbackDeliveryOrder(string orderId)
        {
            if (string.IsNullOrEmpty(orderId)) return false;
            orderId = orderId.ToLower();
            try
            {
                string postData = GenerateParameter("yanxuan.notification.order.delivered", new Dictionary<string, string> { { "orderId", orderId } });
                string jsonstr = WebRequestHelper.SendPostInfo(ApiCallBackUrl + "callbackDeliveryOrder", postData);
                LogHelper.Info("YXSV.CallbackDeliveryOrder 触发包裹物流绑单回调，Request: " + postData + "，Response: " + jsonstr);
                var result = JsonConvert.DeserializeObject<YXResult<string>>(jsonstr);
                if (result.Successed)
                {
                    return true;
                }
                else
                {
                    LogHelper.Error("YXSV.CallbackDeliveryOrder 触发包裹物流绑单回调失败，Request: " + postData + "，Response: " + jsonstr);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXSV.CallbackDeliveryOrder 触发包裹物流绑单回调异常，Request: " + orderId, ex);
            }
            return false;
        }

        /// <summary>
        /// 触发异常取消订单回调
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static bool CallbackCancelOrder(string orderId)
        {
            if (string.IsNullOrEmpty(orderId)) return false;
            orderId = orderId.ToLower();
            try
            {
                string postData = GenerateParameter("yanxuan.notification.order.exceptional", new Dictionary<string, string> { { "orderId", orderId } });
                string jsonstr = WebRequestHelper.SendPostInfo(ApiCallBackUrl + "callbackCancelOrder", postData);
                LogHelper.Info("YXSV.CallbackCancelOrder 触发异常取消订单回调，Request: " + postData + "，Response: " + jsonstr);
                var result = JsonConvert.DeserializeObject<YXResult<string>>(jsonstr);
                if (result.Successed)
                {
                    return true;
                }
                else
                {
                    LogHelper.Error("YXSV.CallbackCancelOrder 触发异常取消订单回调失败，Request: " + postData + "，Response: " + jsonstr);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXSV.CallbackCancelOrder 触发异常取消订单回调异常，Request: " + orderId, ex);
            }
            return false;
        }

        /// <summary>
        /// 触发审核取消订单回调
        /// </summary>      
        /// <param name="applyId">申请单Id</param>
        /// <param name="cancel">是否同意取消</param>
        /// <returns></returns>
        public static bool CallbackAuditCancelOrder(string applyId, bool cancel)
        {
            string postData = GenerateParameter("/callbackAuditCancelOrder", new Dictionary<string, string> { { "applyId", applyId }, { "cancel", cancel.ToString() } });
            try
            {
                string jsonstr = WebRequestHelper.SendPostInfo(ApiCallBackUrl + "callbackAuditCancelOrder", postData);
                LogHelper.Info("YXSV.CallbackAuditCancelOrder 触发退货包裹确认收货后退款结果回调，Request: " + postData + "，Response: " + jsonstr);
                var result = JsonConvert.DeserializeObject<YXResult<string>>(jsonstr);
                if (result.Successed)
                {
                    return true;
                }
                else
                {
                    LogHelper.Error("YXSV.CallbackAuditCancelOrder 触发退货包裹确认收货后退款结果回调 失败，Request: " + postData + "，Response: " + jsonstr);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXSV.CallbackAuditCancelOrder 触发退货包裹确认收货后退款结果回调 异常，Request: " + postData, ex);
            }
            return false;
        }

        /// <summary>
        /// 触发退货地址回调
        /// </summary>      
        /// <param name="applyId">申请单Id</param>
        /// <param name="type">1：通过，无理由退货，2：通过，质量问题退货</param>
        /// <returns></returns>
        public static bool CallbackRefundAddress(string applyId, int type)
        {
            string postData = GenerateParameter("/callbackRefundAddress", new Dictionary<string, string> { { "applyId", applyId }, { "type", type.ToString() } });
            try
            {
                string jsonstr = WebRequestHelper.SendPostInfo(ApiCallBackUrl + "callbackRefundAddress", postData);
                LogHelper.Info("YXSV.CallbackRefundAddress 触发退货地址回调，Request: " + postData + "，Response: " + jsonstr);
                var result = JsonConvert.DeserializeObject<YXResult<string>>(jsonstr);
                if (result.Successed)
                {
                    return true;
                }
                else
                {
                    LogHelper.Error("YXSV.CallbackRefundAddress 触发退货地址回调 失败，Request: " + postData + "，Response: " + jsonstr);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXSV.CallbackRefundAddress 触发退货地址回调 异常，Request: " + postData, ex);
            }
            return false;
        }

        /// <summary>
        /// 触发严选拒绝退货回调
        /// </summary>      
        /// <param name="applyId">申请单Id</param>
        /// <returns></returns>
        public static bool CallbackRefundReject(string applyId)
        {
            string postData = GenerateParameter("/callbackRefundReject", new Dictionary<string, string> { { "applyId", applyId } });
            try
            {
                string jsonstr = WebRequestHelper.SendPostInfo(ApiCallBackUrl + "callbackRefundReject", postData);
                LogHelper.Info("YXSV.CallbackRefundReject 触发严选拒绝退货回调，Request: " + postData + "，Response: " + jsonstr);
                var result = JsonConvert.DeserializeObject<YXResult<string>>(jsonstr);
                if (result.Successed)
                {
                    return true;
                }
                else
                {
                    LogHelper.Error("YXSV.CallbackRefundReject 触发严选拒绝退货回调 失败，Request: " + postData + "，Response: " + jsonstr);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXSV.CallbackRefundReject 触发严选拒绝退货回调 异常，Request: " + postData, ex);
            }
            return false;
        }

        /// <summary>
        /// 触发退货包裹确认收货回调
        /// </summary>      
        /// <param name="applyId">申请单Id</param>
        /// <returns></returns>
        public static bool CallbackRefundExpressConfirm(string applyId)
        {
            string postData = GenerateParameter("/callbackRefundExpressConfirm", new Dictionary<string, string> { { "applyId", applyId } });
            try
            {
                string jsonstr = WebRequestHelper.SendPostInfo(ApiCallBackUrl + "callbackRefundExpressConfirm", postData);
                LogHelper.Info("YXSV.CallbackRefundExpressConfirm 触发退货包裹确认收货回调，Request: " + postData + "，Response: " + jsonstr);
                var result = JsonConvert.DeserializeObject<YXResult<string>>(jsonstr);
                if (result.Successed)
                {
                    return true;
                }
                else
                {
                    LogHelper.Error("YXSV.CallbackRefundExpressConfirm 触发退货包裹确认收货回调 失败，Request: " + postData + "，Response: " + jsonstr);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXSV.CallbackRefundExpressConfirm 触发退货包裹确认收货回调 异常，Request: " + postData, ex);
            }
            return false;
        }

        /// <summary>
        /// 触发严选系统取消退货回调
        /// </summary>      
        /// <param name="applyId">申请单Id</param>
        /// <returns></returns>
        public static bool CallbackRefundSystemCancel(string applyId)
        {
            string postData = GenerateParameter("/callbackRefundSystemCancel", new Dictionary<string, string> { { "applyId", applyId } });
            try
            {
                string jsonstr = WebRequestHelper.SendPostInfo(ApiCallBackUrl + "callbackRefundSystemCancel", postData);
                LogHelper.Info("YXSV.CallbackRefundSystemCancel 触发严选系统取消退货回调，Request: " + postData + "，Response: " + jsonstr);
                var result = JsonConvert.DeserializeObject<YXResult<string>>(jsonstr);
                if (result.Successed)
                {
                    return true;
                }
                else
                {
                    LogHelper.Error("YXSV.CallbackRefundSystemCancel 触发严选系统取消退货回调 失败，Request: " + postData + "，Response: " + jsonstr);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXSV.CallbackRefundSystemCancel 触发严选系统取消退货回调 异常，Request: " + postData, ex);
            }
            return false;
        }

        /// <summary>
        /// 触发直接退款结果回调
        /// </summary>      
        /// <param name="applyId">申请单Id</param>
        /// <returns></returns>
        public static bool CallbackRefundResultDirectly(string applyId)
        {
            string postData = GenerateParameter("/callbackRefundResultDirectly", new Dictionary<string, string> { { "applyId", applyId } });
            try
            {
                string jsonstr = WebRequestHelper.SendPostInfo(ApiCallBackUrl + "callbackRefundResultDirectly", postData);
                LogHelper.Info("YXSV.CallbackRefundResultDirectly 触发直接退款结果回调，Request: " + postData + "，Response: " + jsonstr);
                var result = JsonConvert.DeserializeObject<YXResult<string>>(jsonstr);
                if (result.Successed)
                {
                    return true;
                }
                else
                {
                    LogHelper.Error("YXSV.CallbackRefundResultDirectly 触发直接退款结果回调 失败，Request: " + postData + "，Response: " + jsonstr);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXSV.CallbackRefundResultDirectly 触发直接退款结果回调 异常，Request: " + postData, ex);
            }
            return false;
        }

        /// <summary>
        /// 触发退货包裹确认收货后退款结果回调
        /// </summary>      
        /// <param name="applyId">申请单Id</param>
        /// <param name="allApproved">是否该申请单下所有sku都审核同意退款，默认为true</param>
        /// <returns></returns>
        public static bool CallbackRefundResultWithExpress(string applyId, bool allApproved)
        {
            string postData = GenerateParameter("/callbackRefundResultWithExpress", new Dictionary<string, string> { { "applyId", applyId }, { "allApproved", allApproved.ToString().ToLower() } });
            try
            {
                string jsonstr = WebRequestHelper.SendPostInfo(ApiCallBackUrl + "callbackRefundResultWithExpress", postData);
                LogHelper.Info("YXSV.CallbackRefundResultWithExpress 触发退货包裹确认收货后退款结果回调，Request: " + postData + "，Response: " + jsonstr);
                var result = JsonConvert.DeserializeObject<YXResult<string>>(jsonstr);
                if (result.Successed)
                {
                    return true;
                }
                else
                {
                    LogHelper.Error("YXSV.CallbackRefundResultWithExpress 触发退货包裹确认收货后退款结果回调 失败，Request: " + postData + "，Response: " + jsonstr);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXSV.CallbackRefundResultWithExpress 触发退货包裹确认收货后退款结果回调 异常，Request: " + postData, ex);
            }
            return false;
        }

        #endregion
    }
}
