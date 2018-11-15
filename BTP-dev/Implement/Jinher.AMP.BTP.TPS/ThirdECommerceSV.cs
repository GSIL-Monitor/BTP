using System;
using System.Collections.Generic;
using System.Linq;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.CustomDTO.ThirdECommerce;
using Jinher.JAP.Common.Loging;
using Newtonsoft.Json;

namespace Jinher.AMP.BTP.TPS
{
    /// <summary>
    /// 第三方电商(非京东大客户和网易严选)标准接口接入
    /// </summary>
    public static class ThirdECommerceSV
    {
        /// <summary>
        /// 生成请求参数
        /// </summary>
        /// <param name="apiInfo"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        private static string GenerateParameter(ThirdApiInfo apiInfo, Dictionary<string, string> paras)
        {
            var time = TimeHelper.GetTimestampS().ToString();
            var code = OAPISV.GetCode(apiInfo.Apikey, apiInfo.CallerId, time);
            var dic = new Dictionary<string, string>();
            dic.Add("callerId", apiInfo.CallerId);
            dic.Add("time", time);
            dic.Add("code", code);
            paras.ToList().ForEach(x => dic.Add(x.Key, x.Value));
            return string.Join("&", dic.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));
        }

        /// <summary>
        /// 第三方电商标准接口提交订单
        /// </summary>
        /// <param name="order"></param>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public static ThirdResponse CreateOrder(ThirdApiInfo apiInfo, ThirdOrderCreate order, ref string jsonStr)
        {
            try
            {
                order.OrderId = order.OrderId.ToLower();
                var orderJson = JsonConvert.SerializeObject(order);
                var postData = GenerateParameter(apiInfo, new Dictionary<string, string> { { "order", orderJson } });
                jsonStr = WebRequestHelper.SendPostInfo(apiInfo.ApiUrl, postData);
                LogHelper.Info("ThirdECommerceSV.CreateOrder 第三方电商标准接口提交订单，Request: " + postData + "，Response: " + jsonStr);
                if (string.IsNullOrEmpty(jsonStr)) return new ThirdResponse { Msg = "接口Response为空" };
                if (!jsonStr.StartsWith("{")) return new ThirdResponse { Msg = jsonStr };
                var response = JsonConvert.DeserializeObject<ThirdResponse>(jsonStr);
                if (response == null) return new ThirdResponse { Msg = "反序列化失败" };
                return response;
            }
            catch (JsonReaderException ex)
            {
                LogHelper.Error("ThirdECommerceSV.CreateOrder 第三方电商标准接口提交订单反序列化异常，Request: " + JsonConvert.SerializeObject(order), ex);
                return new ThirdResponse { Msg = "反序列化异常" };
            }
            catch (Exception ex)
            {
                LogHelper.Error("ThirdECommerceSV.CreateOrder 第三方电商标准接口提交订单异常，Request: " + JsonConvert.SerializeObject(order), ex);
                return new ThirdResponse { Msg = ex.ToString() };
            }
        }

        /// <summary>
        /// 第三方电商标准接口取消订单
        /// </summary>
        /// <param name="apiInfo"></param>
        /// <param name="orderId"></param>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public static ThirdResponse<ThirdOrderCancelResult> CancelOrder(ThirdApiInfo apiInfo, string orderId, ref string jsonStr)
        {
            if (string.IsNullOrEmpty(orderId)) return new ThirdResponse<ThirdOrderCancelResult> { Msg = "参数有误" };
            orderId = orderId.ToLower();
            try
            {
                var postData = GenerateParameter(apiInfo, new Dictionary<string, string> { { "orderId", orderId } });
                jsonStr = WebRequestHelper.SendPostInfo(apiInfo.ApiUrl, postData);
                LogHelper.Info("ThirdECommerceSV.CancelOrder 第三方电商标准接口取消订单，Request: " + postData + "，Response: " + jsonStr);
                if (string.IsNullOrEmpty(jsonStr)) return new ThirdResponse<ThirdOrderCancelResult> { Msg = "接口Response为空" };
                if (!jsonStr.StartsWith("{")) return new ThirdResponse<ThirdOrderCancelResult> { Msg = jsonStr };
                var response = JsonConvert.DeserializeObject<ThirdResponse<string>>(jsonStr);
                if (response == null) return new ThirdResponse<ThirdOrderCancelResult> { Msg = "反序列化失败" };
                if (response.Successed && !string.IsNullOrEmpty(response.Result))
                {
                    return new ThirdResponse<ThirdOrderCancelResult> { Code = response.Code, Msg = response.Msg, Result = JsonConvert.DeserializeObject<ThirdOrderCancelResult>(response.Result) };
                }
                else
                {
                    LogHelper.Error("ThirdECommerceSV.CancelOrder 第三方电商标准接口取消订单失败，Request: " + postData + "，Response: " + jsonStr);
                    return new ThirdResponse<ThirdOrderCancelResult> { Code = response.Code, Msg = response.Msg };
                }
            }
            catch (JsonReaderException ex)
            {
                LogHelper.Error("ThirdECommerceSV.CancelOrder 第三方电商标准接口取消订单反序列化异常，orderId: " + orderId, ex);
                return new ThirdResponse<ThirdOrderCancelResult> { Msg = "反序列化异常" };
            }
            catch (Exception ex)
            {
                LogHelper.Error("ThirdECommerceSV.CancelOrder 第三方电商标准接口取消订单异常，orderId: " + orderId, ex);
                return new ThirdResponse<ThirdOrderCancelResult> { Msg = ex.Message };
            }
        }

        /// <summary>
        /// 第三方电商标准接口提交售后服务申请
        /// </summary>
        /// <param name="apiInfo"></param>
        /// <param name="service"></param>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public static ThirdResponse<ThirdServiceCreateResult> CreateService(ThirdApiInfo apiInfo, ThirdServiceCreate service, ref string jsonStr)
        {
            try
            {
                service.OrderId = service.OrderId.ToLower();
                service.ServiceId = service.ServiceId.ToLower();
                var serviceJson = JsonConvert.SerializeObject(service);
                var postData = GenerateParameter(apiInfo, new Dictionary<string, string> { { "service", serviceJson } });
                jsonStr = WebRequestHelper.SendPostInfo(apiInfo.ApiUrl, postData);
                LogHelper.Info("ThirdECommerceSV.CreateService 第三方电商标准接口提交售后服务申请，Request: " + postData + "，Response: " + jsonStr);
                if (string.IsNullOrEmpty(jsonStr)) return new ThirdResponse<ThirdServiceCreateResult> { Msg = "接口Response为空" };
                if (!jsonStr.StartsWith("{")) return new ThirdResponse<ThirdServiceCreateResult> { Msg = jsonStr };
                var response = JsonConvert.DeserializeObject<ThirdResponse<string>>(jsonStr);
                if (response == null) return new ThirdResponse<ThirdServiceCreateResult> { Msg = "反序列化失败" };
                if (response.Successed && !string.IsNullOrEmpty(response.Result))
                {
                    return new ThirdResponse<ThirdServiceCreateResult> { Code = response.Code, Msg = response.Msg, Result = JsonConvert.DeserializeObject<ThirdServiceCreateResult>(response.Result) };
                }
                else
                {
                    LogHelper.Error("ThirdECommerceSV.CreateService 第三方电商标准接口提交售后服务申请失败，Request: " + postData + "，Response: " + jsonStr);
                    return new ThirdResponse<ThirdServiceCreateResult> { Code = response.Code, Msg = response.Msg };
                }
            }
            catch (JsonReaderException ex)
            {
                LogHelper.Error("ThirdECommerceSV.CreateService 第三方电商标准接口提交售后服务申请反序列化异常，Request: " + JsonConvert.SerializeObject(service), ex);
                return new ThirdResponse<ThirdServiceCreateResult> { Msg = "反序列化异常" };
            }
            catch (Exception ex)
            {
                LogHelper.Error("ThirdECommerceSV.CreateService 第三方电商标准接口提交售后服务申请异常，Request: " + JsonConvert.SerializeObject(service), ex);
                return new ThirdResponse<ThirdServiceCreateResult> { Msg = ex.Message };
            }
        }

        /// <summary>
        /// 第三方电商标准接口取消售后服务申请
        /// </summary>
        /// <param name="apiInfo"></param>
        /// <param name="serviceId"></param>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public static ThirdResponse CancelService(ThirdApiInfo apiInfo, string serviceId, ref string jsonStr)
        {
            if (string.IsNullOrEmpty(serviceId)) return new ThirdResponse { Msg = "参数有误" };
            serviceId = serviceId.ToLower();
            try
            {
                var postData = GenerateParameter(apiInfo, new Dictionary<string, string> { { "serviceId", serviceId } });
                jsonStr = WebRequestHelper.SendPostInfo(apiInfo.ApiUrl, postData);
                LogHelper.Info("ThirdECommerceSV.CancelService 第三方电商标准接口取消售后服务申请，Request: " + postData + "，Response: " + jsonStr);
                if (string.IsNullOrEmpty(jsonStr)) return new ThirdResponse { Msg = "接口Response为空" };
                if (!jsonStr.StartsWith("{")) return new ThirdResponse { Msg = jsonStr };
                var response = JsonConvert.DeserializeObject<ThirdResponse>(jsonStr);
                if (response == null) return new ThirdResponse { Msg = "反序列化失败" };
                return response;
            }
            catch (JsonReaderException ex)
            {
                LogHelper.Error("ThirdECommerceSV.CancelService 第三方电商标准接口取消售后服务申请反序列化异常，serviceId: " + serviceId, ex);
                return new ThirdResponse { Msg = "反序列化异常" };
            }
            catch (Exception ex)
            {
                LogHelper.Error("ThirdECommerceSV.CancelService 第三方电商标准接口取消售后服务申请异常，serviceId: " + serviceId, ex);
                return new ThirdResponse { Msg = ex.Message };
            }
        }
    }
}
