using System;
using System.Collections.Generic;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Configuration;

namespace Jinher.AMP.BTP.TPS
{
    /// <summary>
    /// 京东接口
    /// </summary>
    public static class JDSV
    {
        static readonly string UrlBase = "https://bizapi.jd.com/api/";

        static readonly bool IsProduction = false;
        static JDSV()
        {
            var isProduction = ConfigurationManager.AppSettings["Production"];
            if (string.IsNullOrEmpty(isProduction))
            {
                IsProduction = false;
            }
            IsProduction = isProduction == "1";
        }


        /// <summary>
        /// 批量查询协议价价格
        /// </summary>
        /// <returns></returns>
        public static List<JdPriceDto> GetPrice(List<string> skuId)
        {
            try
            {
                string token = JdHelper.GetToken();
                string postData = "token=" + token + "&sku=" + string.Join(",", skuId);
                string jsonstr = WebRequestHelper.SendPostInfo(UrlBase + "price/getPrice", postData);

                //LogHelper.Info("JDSV.GetPrice 批量查询协议价价格，Request: " + postData + "，Response: " + jsonstr);

                var resullt = Newtonsoft.Json.JsonConvert.DeserializeObject<JdResultDto<List<JdPriceDto>>>(jsonstr);
                if (resullt.Success)
                {
                    // {"success":true,"resultMessage":"5090522不在您的商品池中，价格为null或者小于0时，为暂无报价","resultCode":"0000","result":[{"price":105.84,"skuId":5090521,"jdPrice":108.00}]}
                    // {"success":true,"resultMessage":"价格为null或者小于0时，为暂无报价","resultCode":"0000","result":[{"price":105.84,"skuId":5090521,"jdPrice":108.00}]}

                    //if (resullt.ResultMessage != "价格为null或者小于0时，为暂无报价")
                    //{
                    //    var facade = new Jinher.AMP.BTP.IBP.Facade.JdlogsFacade();
                    //    facade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                    //    var log = new Jinher.AMP.BTP.Deploy.JdlogsDTO
                    //    {
                    //        Id = Guid.NewGuid(),
                    //        Content = "【价格同步】" + resullt.ResultMessage
                    //    };
                    //    facade.SaveJdlogs(log);
                    //}
                    return resullt.Result;
                }
                else
                {
                    LogHelper.Error("JDSV.GetPrice 批量查询协议价价格 失败，Request: " + postData + "，Response: " + resullt.ResultCode + "-" + resullt.ResultMessage);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("JDSV.GetPrice 批量查询协议价价格异常", ex);
            }
            return new List<JdPriceDto>();
        }
        /// <summary>
        /// 批量获取商品图片信息
        /// </summary>
        /// <returns></returns>
        public static List<JdComPicturesDto> GetComPictures(List<string> skuId)
        {
            try
            {
                string token = JdHelper.GetToken();
                string postData = "token=" + token + "&sku=" + string.Join(",", skuId);
                string jsonstr = WebRequestHelper.SendPostInfo(UrlBase + "product/skuImage", postData);

                LogHelper.Info("JDSV.GetComPictures 批量获取京东商品图片，Request: " + postData + "，Response: " + jsonstr);

                var result = new List<JdComPicturesDto>();

                var jsonObj = JObject.Parse(jsonstr);
                if (jsonObj["success"].ToObject<bool>())
                {
                    foreach (var sku in skuId)
                    {
                        var skuObj = jsonObj["result"][sku].ToObject<List<JdComPicturesDto>>();
                        if (skuObj != null)
                        {
                            result.AddRange(skuObj);
                        }
                    }
                    return result;
                }
                else
                {
                    //LogHelper.Error("JDSV.GetComPictures 批量获取京东商品图片 失败，Request: " + postData + "，Response: " + resullt.ResultCode + "-" + resullt.ResultMessage);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("JDSV.GetComPictures 批量获取京东商品图片", ex);
            }
            return new List<JdComPicturesDto>();
        }
        /// <summary>
        /// 获取京东商品详情
        /// </summary>
        /// <returns></returns>
        public static JdComDetailDto GetJdDetail(string sku)
        {
            try
            {
                string token = JdHelper.GetToken();
                string postData = "token=" + token + "&sku=" + sku + "&isShow=" + true;
                string jsonstr = WebRequestHelper.SendPostInfo(UrlBase + "product/getDetail", postData);
                var resullt = Newtonsoft.Json.JsonConvert.DeserializeObject<JdResultDto<JdComDetailDto>>(jsonstr);
                LogHelper.Info("JDSV.GetJdDetail 批量查询京东商品详情,，Request: " + postData + "，Response: " + resullt.Success);
                if (resullt.Success)
                {
                    return resullt.Result;
                }
                else
                {
                    LogHelper.Error("JDSV.GetJdDetail获取京东商品详情失败，Request: " + postData + "，Response: " + resullt.ResultCode + "-" + resullt.ResultMessage);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("JDSV.GetJdDetail 获取京东商品详情异常", ex);
            }
            return new JdComDetailDto();
        }

        /// <summary>
        /// 校验某订单中某商品是否可以提交售后服务
        /// </summary>
        /// <returns></returns>
        public static bool GetAvailableNumberComp(string jdOrderId, string skuId)
        {
            try
            {
                string token = JdHelper.GetToken();
                string postData = "token=" + token + "&param={\"jdOrderId\":" + jdOrderId + ",\"skuId\":" + skuId + "}";
                string jsonstr = WebRequestHelper.SendPostInfo(UrlBase + "afterSale/getAvailableNumberComp", postData);
                var json = JObject.Parse(jsonstr);
                return json["success"].ToObject<bool>();
            }
            catch (Exception ex)
            {
                LogHelper.Error("JDSV.GetAvailableNumberComp 异常", ex);
                return false;
            }
        }

        /// <summary>
        /// 根据订单号、商品编号查询支持的服务类型
        /// </summary>
        /// <param name="jdOrderId"></param>
        /// <param name="skuId"></param>
        /// <returns></returns>
        public static List<JdComponentExport> GetCustomerExpectComp(string jdOrderId, string skuId)
        {
            try
            {
                string token = JdHelper.GetToken();
                string postData = "token=" + token + "&param={\"jdOrderId\":" + jdOrderId + ",\"skuId\":" + skuId + "}";
                string jsonstr = WebRequestHelper.SendPostInfo(UrlBase + "afterSale/getCustomerExpectComp", postData);
                var resullt = Newtonsoft.Json.JsonConvert.DeserializeObject<JdResultDto<List<JdComponentExport>>>(jsonstr);
                if (resullt.Success)
                {
                    return resullt.Result;
                }
                else
                {
                    LogHelper.Error("JDSV.GetCustomerExpectComp 查询支持的服务类型 失败，Request: " + postData + "，Response: " + resullt.ResultCode + "-" + resullt.ResultMessage);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("JDSV.GetCustomerExpectComp 查询支持的服务类型 异常", ex);
            }

            return new List<JdComponentExport>();
        }

        /// <summary>
        /// 根据订单号、商品编号查询支持的商品返回京东方式
        /// </summary>
        /// <param name="jdOrderId"></param>
        /// <param name="skuId"></param>
        /// <returns></returns>
        public static List<JdComponentExport> GetWareReturnJdComp(string jdOrderId, string skuId)
        {
            try
            {
                string token = JdHelper.GetToken();
                string postData = "token=" + token + "&param={\"jdOrderId\":" + jdOrderId + ",\"skuId\":" + skuId + "}";
                string jsonstr = WebRequestHelper.SendPostInfo(UrlBase + "afterSale/getWareReturnJdComp", postData);
                var resullt = Newtonsoft.Json.JsonConvert.DeserializeObject<JdResultDto<List<JdComponentExport>>>(jsonstr);
                if (resullt.Success)
                {
                    return resullt.Result;
                }
                else
                {
                    LogHelper.Error("JDSV.GetWareReturnJdComp 查询支持的商品返回京东方式 失败，Request: " + postData + "，Response: " + resullt.ResultCode + "-" + resullt.ResultMessage);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("JDSV.GetWareReturnJdComp 查询支持的商品返回京东方式 异常", ex);
            }
            return new List<JdComponentExport>();
        }

        /// <summary>
        /// 根据客户账号和订单号分页查询服务单概要信息
        /// </summary>
        /// <param name="jdOrderId"></param>
        /// <param name="skuId"></param>
        /// <returns></returns>
        public static List<AfsServicebyCustomerPin> GetServiceListPage(string jdOrderId)
        {
            try
            {
                string token = JdHelper.GetToken();
                string postData = "token=" + token + "&param={\"jdOrderId\":" + jdOrderId + ", \"pageIndex\":1, \"pageSize\":10}";
                string jsonstr = WebRequestHelper.SendPostInfo(UrlBase + "afterSale/getServiceListPage", postData);
                var resullt = Newtonsoft.Json.JsonConvert.DeserializeObject<JdResultDto<AfsServicebyCustomerPinPage>>(jsonstr);
                if (resullt.Success)
                {
                    return resullt.Result.serviceInfoList;
                }
                else
                {
                    LogHelper.Error("JDSV.GetServiceListPage 查询服务单概要信息 失败，Request: " + postData + "，Response: " + resullt.ResultCode + "-" + resullt.ResultMessage);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("JDSV.GetServiceListPage 查询服务单概要信息 异常", ex);
            }
            return null;
        }

        /// <summary>
        /// 根据服务单号查询服务单明细信息
        /// </summary>
        /// <param name="afsServiceId"></param>
        /// <returns></returns>
        public static CompatibleServiceDetailDTO GetServiceDetailInfo(string afsServiceId)
        {
            try
            {
                string token = JdHelper.GetToken();
                string postData = "token=" + token + "&param={\"afsServiceId\":" + afsServiceId + ", \"appendInfoSteps\":[1,2,3,4,5]}";
                string jsonstr = WebRequestHelper.SendPostInfo(UrlBase + "afterSale/getServiceDetailInfo", postData);
                var resullt = Newtonsoft.Json.JsonConvert.DeserializeObject<JdResultDto<CompatibleServiceDetailDTO>>(jsonstr);
                if (resullt.Success)
                {
                    return resullt.Result;
                }
                else
                {
                    LogHelper.Error("JDSV.GetServiceDetailInfo 查询服务单明细信息 失败，Request: " + postData + "，Response: " + resullt.ResultCode + "-" + resullt.ResultMessage);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("JDSV.GetServiceDetailInfo 查询服务单明细信息 异常", ex);
            }
            return null;
        }


        /// <summary>
        /// 服务单保存申请
        /// </summary>
        /// <param name="afsServiceId"></param>
        /// <returns></returns>
        public static ResultDTO CreateAfsApply(AfterSaleDto dto)
        {
            try
            {
                string token = JdHelper.GetToken();
                var par = JsonHelper.JsonSerializer(dto);
                string postData = "token=" + token + "&param=" + par;
                string jsonstr = WebRequestHelper.SendPostInfo(UrlBase + "afterSale/createAfsApply", postData);
                var resullt = Newtonsoft.Json.JsonConvert.DeserializeObject<JdResultDto>(jsonstr);
                if (resullt.Success)
                {
                    LogHelper.Info("JDSV.CreateAfsApply 服务单保存申请 成功，Request: " + postData + "，Response: " + jsonstr);
                    return new ResultDTO { isSuccess = true };
                }
                else
                {
                    LogHelper.Error("JDSV.CreateAfsApply 服务单保存申请 失败，Request: " + postData + "，Response: " + resullt.ResultCode + "-" + resullt.ResultMessage);
                    return new ResultDTO { isSuccess = false, ResultCode = resullt.ResultCode ?? -1, Message = resullt.ResultMessage };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("JDSV.CreateAfsApply 服务单保存申请 异常", ex);
                return new ResultDTO { isSuccess = false, Message = ex.Message, ResultCode = -1 };
            }
        }

        /// <summary>
        /// 取消服务单
        /// </summary>
        /// <param name="afsServiceId"></param>
        /// <returns></returns>
        public static ResultDTO AuditCancel(string serviceId, string approveNotes)
        {
            try
            {
                string token = JdHelper.GetToken();
                string postData = "token=" + token + "&param={\"serviceIdList\":[\"" + serviceId + "\"],\"approveNotes\":\"" + approveNotes + "\"}";
                string jsonstr = WebRequestHelper.SendPostInfo(UrlBase + "afterSale/auditCancel", postData);
                var resullt = Newtonsoft.Json.JsonConvert.DeserializeObject<JdResultDto>(jsonstr);
                if (resullt.Success)
                {
                    return new ResultDTO { isSuccess = true };
                }
                else
                {
                    LogHelper.Error("JDSV.AuditCancel 取消服务单 失败，Request: " + postData + "，Response: " + resullt.ResultCode + "-" + resullt.ResultMessage);
                    return new ResultDTO { isSuccess = false, Message = resullt.ResultMessage, ResultCode = -1 };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("JDSV.AuditCancel 取消服务单 异常", ex);
                return new ResultDTO { isSuccess = false, Message = ex.Message, ResultCode = -1 };
            }
        }

        /// <summary>
        /// 取消服务单
        /// </summary>
        /// <param name="afsServiceId"></param>
        /// <returns></returns>
        public static ResultDTO AuditMultipulCancel(List<string> serviceIds, string approveNotes)
        {
            try
            {
                string token = JdHelper.GetToken();
                var serviceIdstr = string.Join("\",\"", serviceIds);
                string postData = "token=" + token + "&param={\"serviceIdList\":[\"" + serviceIdstr + "\"],\"approveNotes\":\"" + approveNotes + "\"}";
                string jsonstr = WebRequestHelper.SendPostInfo(UrlBase + "afterSale/auditCancel", postData);
                var resullt = Newtonsoft.Json.JsonConvert.DeserializeObject<JdResultDto>(jsonstr);
                if (resullt.Success)
                {
                    return new ResultDTO { isSuccess = true };
                }
                else
                {
                    LogHelper.Error("JDSV.AuditCancel 取消服务单 失败，Request: " + postData + "，Response: " + resullt.ResultCode + "-" + resullt.ResultMessage);
                    return new ResultDTO { isSuccess = false, Message = resullt.ResultMessage, ResultCode = -1 };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("JDSV.AuditCancel 取消服务单 异常", ex);
                return new ResultDTO { isSuccess = false, Message = ex.Message, ResultCode = -1 };
            }
        }

        /// <summary>
        /// 获取推送消息
        /// </summary>
        /// <param name="type">消息类型（2-商品价格变更 3-库存变化 4-商品上下架变更）</param>
        /// <returns></returns>
        public static List<JdPriceMessageDto> GetMessage(string type)
        {
            try
            {
                string token = JdHelper.GetToken();
                string postData = "token=" + token + "&type=" + type;
                string jsonstr = WebRequestHelper.SendPostInfo(UrlBase + "message/get", postData);
                //发送数据到mq
                var json = SerializationHelper.JsonSerialize(new { MsgType = type, Request = postData, Response = jsonstr });
                RabbitMqHelper.Send(RabbitMqRoutingKey.JdCommodityMsgReceived, RabbitMqExchange.Commodity, json);
                var resullt = Newtonsoft.Json.JsonConvert.DeserializeObject<JdResultDto<List<JdPriceMessageDto>>>(jsonstr);
                if (resullt.Success)
                {
                    return resullt.Result;
                }
                else
                {
                    LogHelper.Error("JDSV.GetMessage 获取推送消息 失败，Request: " + postData + "，Response: " + resullt.ResultCode + "-" + resullt.ResultMessage);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("JDSV.GetMessage 获取推送消息 异常", ex);
            }
            return null;
        }

        /// <summary>
        /// 删除推送消息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static ResultDTO DelMessage(List<string> ids)
        {
            if (!IsProduction) return new ResultDTO { isSuccess = true };
            try
            {
                string token = JdHelper.GetToken();
                string postData = "token=" + token + "&id=" + string.Join(",", ids);
                string jsonstr = WebRequestHelper.SendPostInfo(UrlBase + "message/del", postData);
                var resullt = Newtonsoft.Json.JsonConvert.DeserializeObject<JdResultDto>(jsonstr);
                if (resullt.Success)
                {
                    LogHelper.Info("JDSV.DelMessage 删除推送消息 成功，Request: " + postData + "，Response: " + resullt.ResultMessage);
                    return new ResultDTO { isSuccess = true };
                }
                else
                {
                    LogHelper.Error("JDSV.DelMessage 删除推送消息 失败，Request: " + postData + "，Response: " + resullt.ResultCode + "-" + resullt.ResultMessage);
                    return new ResultDTO { isSuccess = false, Message = resullt.ResultMessage, ResultCode = -1 };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("JDSV.DelMessage 删除推送消息 异常", ex);
                return new ResultDTO { isSuccess = false, Message = ex.Message, ResultCode = -1 };
            }
        }

        /// <summary>
        /// 获取库存信息
        /// </summary>
        /// <param name="type">消息类型（2代表商品价格变更）</param>
        /// <returns></returns>
        public static List<JdStockDto> GetStockById(List<string> skus, string area)
        {
            var result = new List<JdStockDto>();
            try
            {
                skus = skus.Where(_ => _.Length > 5 && _.Length < 9).ToList();
                string token = JdHelper.GetToken();
                string postData = "token=" + token + "&sku=" + string.Join(",", skus) + "&area=" + area;
                string jsonstr = WebRequestHelper.SendPostInfo(UrlBase + "stock/getStockById", postData);
                var resullt = Newtonsoft.Json.JsonConvert.DeserializeObject<JdResultDto<string>>(jsonstr);
                if (resullt.Success)
                {
                    result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<JdStockDto>>(resullt.Result);
                }
                else
                {
                    LogHelper.Error("JDSV.GetStockById 获取库存信息 失败，Request: " + postData + "，Response: " + resullt.ResultCode + "-" + resullt.ResultMessage);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("JDSV.GetStockById 获取库存信息 异常", ex);
            }
            return result;
        }

        /// <summary>
        /// 获取上下架状态
        /// </summary>
        /// <param name="type">消息类型（2代表商品价格变更）</param>
        /// <returns></returns>
        public static List<JdSkuStateDto> GetSkuState(List<string> skus)
        {
            try
            {
                skus = skus.Where(_ => _.Length > 5 && _.Length < 9).ToList();
                string token = JdHelper.GetToken();
                string postData = "token=" + token + "&sku=" + string.Join(",", skus);
                string jsonstr = WebRequestHelper.SendPostInfo(UrlBase + "product/skuState", postData);
                var resullt = Newtonsoft.Json.JsonConvert.DeserializeObject<JdResultDto<List<JdSkuStateDto>>>(jsonstr);
                if (resullt.Success)
                {
                    return resullt.Result;
                }
                else
                {
                    LogHelper.Error("JDSV.GetSkuState 获取上下架状态 失败，Request: " + postData + "，Response: " + resullt.ResultCode + "-" + resullt.ResultMessage);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("JDSV.GetSkuState 获取上下架状态 异常", ex);
            }
            return new List<JdSkuStateDto>();
        }
    }
}
