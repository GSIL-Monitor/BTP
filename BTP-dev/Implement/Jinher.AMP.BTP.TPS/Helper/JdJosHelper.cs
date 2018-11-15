using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.CustomDTO.JdJos;
using Jinher.JAP.Common.Loging;
using Newtonsoft.Json;

namespace Jinher.AMP.BTP.TPS
{
    /// <summary>
    /// 京东jos系统API调用
    /// </summary>
    public class JdJosHelper
    {
        public const string APP_KEY = "app_key";
        public const string METHOD = "method";
        public const string TIMESTAMP = "timestamp";
        public const string VERSION = "v";
        public const string SIGN = "sign";
        public const string ACCESS_TOKEN = "access_token";
        public const string PARAM_JSON = "360buy_param_json";

        /// <summary>
        /// 给Jd请求签名。
        /// </summary>
        /// <param name="parameters">所有字符型的Jd请求参数</param>
        /// <param name="secret">签名密钥</param>
        /// <param name="qhs">是否前后都加密钥进行签名</param>
        /// <returns>签名</returns>
        private static string SignJdRequest(IDictionary<string, string> parameters, string secret, bool qhs)
        {
            // 第一步：把字典按Key的字母顺序排序
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parameters);
            IEnumerator<KeyValuePair<string, string>> dem = sortedParams.GetEnumerator();

            // 第二步：把所有参数名和参数值串在一起
            StringBuilder query = new StringBuilder(secret);
            while (dem.MoveNext())
            {
                string key = dem.Current.Key;
                string value = dem.Current.Value;
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                {
                    query.Append(key).Append(value);
                }
            }
            if (qhs)
            {
                query.Append(secret);
            }
            // 第三步：使用MD5加密
            MD5 md5 = MD5.Create();
            byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(query.ToString()));

            // 第四步：把二进制转化为大写的十六进制
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                result.Append(bytes[i].ToString("X2"));
            }

            return result.ToString();
        }

        /// <summary>
        /// API执行公共方法
        /// </summary>
        /// <param name="method">接口名称</param>
        /// <param name="paramJson">接口参数json串</param>
        /// <returns></returns>
        private static BaseResponse SendRequest(string method, string paramJson)
        {
            try
            {
                var now = DateTime.Now;
                var paramDir = new Dictionary<string, string>();
                paramDir.Add(METHOD, method);
                paramDir.Add(VERSION, "2.0");
                paramDir.Add(APP_KEY, CustomConfig.JdJosAppKey);
                paramDir.Add(TIMESTAMP, now.ToString("yyyy-MM-dd HH:mm:ss"));
                paramDir.Add(ACCESS_TOKEN, CustomConfig.JdJosAccessToken);
                paramDir.Add(PARAM_JSON, paramJson);
                paramDir.Add(SIGN, SignJdRequest(paramDir, CustomConfig.JdJosAppSecret, true));
                var paramStr = string.Empty;
                paramDir.ForEach((k, v) =>
                {
                    paramStr += k + "=" + v + "&";
                });
                paramStr = paramStr.TrimEnd('&');
                var request = WebRequest.Create("https://api.jd.com/routerjson");
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                LogHelper.Info("JdJosHelper.SendRequest.paramStr:" + paramStr);
                var bArr = Encoding.UTF8.GetBytes(paramStr);
                var postStream = request.GetRequestStream();
                postStream.Write(bArr, 0, bArr.Length);
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        if (stream == null)
                        {
                            LogHelper.Debug("JdJosHelper.SendRequest:stream=null");
                            return new BaseResponse { IsInterfaceSuccess = false, Message = "京东接口返回流为空stream=null" };
                        }
                        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            var retStr = reader.ReadToEnd();
                            LogHelper.Info("JdJosHelper.SendRequest.retStr:" + retStr);
                            if (string.IsNullOrEmpty(retStr)) return new BaseResponse { IsInterfaceSuccess = false, Message = "京东接口返回值为空" };
                            return new BaseResponse { IsInterfaceSuccess = true, Message = retStr };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("JdJosHelper.SendRequest异常", ex);
                return new BaseResponse { IsInterfaceSuccess = false, Message = ex.ToString() };
            }
        }

        private static AuthResponse OathRequest(string Interface, String queryParam)
        {
            var request = WebRequest.Create("https://oauth.jd.com/oauth/" + Interface + "?");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            var bArr = Encoding.UTF8.GetBytes(queryParam);
            var postStream = request.GetRequestStream();

            using (WebResponse response = request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    if (stream == null)
                    {
                        LogHelper.Debug("京东授权接口请求:stream=null");
                        return new AuthResponse { IsInterfaceSuccess = false, Message = "京东接口返回流为空stream=null" };
                    }

                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        var retStr = reader.ReadToEnd();
                        LogHelper.Debug("京东授权接口请求  https://oauth.jd.com/oauth/" + Interface + "?" + retStr);
                        if (string.IsNullOrEmpty(retStr)) return new AuthResponse { IsInterfaceSuccess = false, Message = "京东接口返回值为空" };

                        var data = JsonConvert.DeserializeObject(retStr) as AccessToken;

                        if (data != null)
                        {
                            return new AuthResponse() { code = data.code, Message = retStr, IsInterfaceSuccess = true, IsResultSuccess = true, access_token = data.access_token };
                        }
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// 销售出库单下发
        /// </summary>
        /// <param name="orderId">订单Id</param>
        /// <param name="receiptUserName">收货人姓名</param>
        /// <param name="receiptPhone">收货人电话</param>
        /// <param name="province">省</param>
        /// <param name="city">市</param>
        /// <param name="district">县（区）</param>
        /// <param name="street">镇（街道）</param>
        /// <param name="receiptAddress">详细地址</param>
        /// <param name="erQiCodeList">二期编码列表</param>
        /// <param name="numberList">商品数量列表</param>
        /// <returns></returns>
        public static AddOrderResponseDTO AddOrder(Guid orderId, string receiptUserName, string receiptPhone, string province, string city
            , string district, string street, string receiptAddress, List<string> erQiCodeList, List<int> numberList)
        {
            if (!string.IsNullOrEmpty(receiptUserName) && receiptUserName.Length >= 10)
            {
                receiptUserName = receiptUserName.Substring(0, 9);
            }
            var json = JsonConvert.SerializeObject(new
            {
                isvUUID = orderId.ToString(),
                isvSource = "ISV0020000000176",
                shopNo = "ESP0020000026240",
                bdOwnerNo = "010K164734",
                departmentNo = "EBU4418046541248",
                warehouseNo = "110008714",
                shipperNo = "CYS0000010",
                salePlatformSource = 6,
                consigneeName = receiptUserName,
                consigneeMobile = receiptPhone,
                consigneeAddress = (province + city + district + street + receiptAddress).Replace("&", "").Replace("🔟", ""),
                orderMark = "00000000000000000000000000000000000000000000000000",
                //supplierNo = "EMS4418046545704",
                //goodsNo = string.Join(",", erQiCodeList),
                isvGoodsNo = string.Join(",", erQiCodeList),
                quantity = string.Join(",", numberList)
            });
            var dto = SendRequest("jingdong.eclp.order.addOrder", json);
            if (dto.IsInterfaceSuccess)
            {
                var result = JsonConvert.DeserializeObject<AddOrderResponseDTO>(dto.Message);
                result.IsInterfaceSuccess = dto.IsInterfaceSuccess;
                result.IsResultSuccess = result.jingdong_eclp_order_addOrder_responce != null && !string.IsNullOrEmpty(result.jingdong_eclp_order_addOrder_responce.eclpSoNo);
                result.Message = dto.Message;
                return result;
            }
            return new AddOrderResponseDTO { IsInterfaceSuccess = dto.IsInterfaceSuccess, Message = dto.Message };
        }

        /// <summary>
        /// 销售出库单包裹数据查询
        /// </summary>
        /// <param name="eclpOrderNo"></param>
        /// <returns></returns>
        public static string queryOrderPacks(string eclpOrderNo)
        {
            var json = JsonConvert.SerializeObject(new
            {
                eclpSoNo = eclpOrderNo,
            });
            var dto = SendRequest("jingdong.eclp.order.queryOrderPacks", json);
            if (dto.IsInterfaceSuccess)
            {
                var result = JsonConvert.DeserializeObject<QueryOrderPacksResponseDTO>(dto.Message);
                result.IsInterfaceSuccess = dto.IsInterfaceSuccess;
                result.IsResultSuccess = result.jingdong_eclp_order_queryOrderPacks_responce != null
                    && result.jingdong_eclp_order_queryOrderPacks_responce.queryorderpacks_result != null
                    && result.jingdong_eclp_order_queryOrderPacks_responce.queryorderpacks_result.Count > 0
                    && !string.IsNullOrEmpty(result.jingdong_eclp_order_queryOrderPacks_responce.queryorderpacks_result[0].wayBill);
                result.Message = dto.Message;
                if (result.IsResultSuccess) return result.jingdong_eclp_order_queryOrderPacks_responce.queryorderpacks_result[0].wayBill;
            }
            return string.Empty;
        }

        /// <summary>
        /// 查询物流跟踪消息
        /// </summary>
        /// <param name="eclpOrderNo"></param>
        /// <returns></returns>
        public static string GetTrackMessagePlusByOrder(string eclpOrderNo)
        {
            var json = JsonConvert.SerializeObject(new
            {
                customerCode = "010K164734",
                bizCode = eclpOrderNo,
                type = "10"
            });
            var dto = SendRequest("jingdong.eclp.order.getTrackMessagePlusByOrder", json);
            if (dto.IsInterfaceSuccess)
            {
                var result = JsonConvert.DeserializeObject<GetTrackMessagePlusByOrderResponseDTO>(dto.Message);
                result.IsInterfaceSuccess = dto.IsInterfaceSuccess;
                result.IsResultSuccess = result.jingdong_eclp_order_getTrackMessagePlusByOrder_responce != null
                    && result.jingdong_eclp_order_getTrackMessagePlusByOrder_responce.getTrackMessagePlusByOrder_result != null
                    && result.jingdong_eclp_order_getTrackMessagePlusByOrder_responce.getTrackMessagePlusByOrder_result.resultData != null
                    && result.jingdong_eclp_order_getTrackMessagePlusByOrder_responce.getTrackMessagePlusByOrder_result.resultData.Count > 0
                    && result.jingdong_eclp_order_getTrackMessagePlusByOrder_responce.getTrackMessagePlusByOrder_result.resultData[0] != null
                    && !string.IsNullOrEmpty(result.jingdong_eclp_order_getTrackMessagePlusByOrder_responce.getTrackMessagePlusByOrder_result.resultData[0].waybillCode);
                result.Message = dto.Message;
                if (result.IsResultSuccess) return result.jingdong_eclp_order_getTrackMessagePlusByOrder_responce.getTrackMessagePlusByOrder_result.resultData[0].waybillCode;
            }
            return string.Empty;
        }

        /// <summary>
        /// 售后服务单
        /// </summary>
        /// <param name="jdEclpOrderRefundAfterSalesId">进销存京东售后服务单id</param>
        /// <param name="eclpOrderNo">京东订单编号</param>
        /// <param name="receiptUserName">收货人或取件人姓名</param>
        /// <param name="receiptPhone">收货人或取件人电话</param>
        /// <param name="consigneeAddress">收货人地址</param>
        /// <param name="opUserId">同意退款操作人Id</param>
        /// <param name="opUserName">同意退款操作人名称</param>
        /// <param name="opTime">同意退款操作时间</param>
        /// <param name="refundDesc">退款详细说明</param>
        /// <param name="refundReason">退款原因</param>
        /// <param name="salerRemark">商家备注</param>
        /// <param name="erQiCodeList">二期编码列表</param>
        /// <param name="numberList">商品数量列表</param>
        /// <param name="numberList">取件地址</param>
        /// <returns></returns>
        public static CreateServiceOrderResponseDTO CreateServiceOrder(Guid jdEclpOrderRefundAfterSalesId, string eclpOrderNo, string receiptUserName
            , string receiptPhone, string consigneeAddress, string opUserId, string opUserName
            , DateTime opTime, string refundDesc, string refundReason, string salerRemark, List<string> erQiCodeList, List<int> numberList
            , string pickupAddress = "", string refundExpOrderNo = "")
        {
            var json = JsonConvert.SerializeObject(new
            {
                isvUUId = jdEclpOrderRefundAfterSalesId.ToString(),
                isvSource = "ISV0020000000176",
                shopNo = "ESP0020000026240",
                departmentNo = "EBU4418046541248",
                shipperNo = "CYS0000010",
                eclpOrderId = eclpOrderNo,
                salePlatformSource = 6,
                customerName = receiptUserName,
                customerTel = receiptPhone,
                consigneeAddress = consigneeAddress,
                pickupAddress = pickupAddress,
                operatorId = opUserId,
                operatorName = opUserName,
                operateTime = opTime.ToString("yyyy-MM-dd HH:mm:ss"),
                questionDesc = refundDesc,
                applyReason = refundReason,
                amsAuditComment = string.IsNullOrEmpty(salerRemark) ? "同意" : salerRemark,
                waybill = refundExpOrderNo,
                pickwaretype = string.IsNullOrEmpty(pickupAddress) ? 2 : 1,
                isvGoodsNo = string.Join(",", erQiCodeList),
                quantity = string.Join(",", numberList),
                attachmentDetails = string.Join(",", erQiCodeList.Select(p => "退货").ToList()),
                wareType = string.Join(",", numberList.Select(p => 10).ToList())
            });
            var retStr = string.Empty;
            var dto = SendRequest("jingdong.eclp.afs.createServiceOrder", json);
            if (dto.IsInterfaceSuccess)
            {
                var result = JsonConvert.DeserializeObject<CreateServiceOrderResponseDTO>(dto.Message);
                result.IsInterfaceSuccess = dto.IsInterfaceSuccess;
                result.IsResultSuccess = result.jingdong_eclp_afs_createServiceOrder_response != null
                    && result.jingdong_eclp_afs_createServiceOrder_response.servicesResult != null
                    && !string.IsNullOrEmpty(result.jingdong_eclp_afs_createServiceOrder_response.servicesResult.serivcesNo);
                result.Message = dto.Message;
                return result;
            }
            return new CreateServiceOrderResponseDTO { IsInterfaceSuccess = dto.IsInterfaceSuccess, Message = dto.Message };
        }

        public static AuthResponse GetJdAccess_Code(String client_id, String redirect_uri, string state, string scope, string view)
        {
            var strb = "response_type=code&client_id=" + client_id + "&redirect_uri=" + redirect_uri + "&state=" + state + "&scope=" + scope + "&view=" + view;
            var dto = OathRequest("authorize", strb);
            return null;
        }

        public static AuthResponse GetJdAccess_Token(String code, string redirect_uri, string client_id, string client_secret, String state = "")
        {
            var strb = "response_type=code&client_id=YOUR_CLIENT_ID&redirect_uri=YOUR_REGISTERED_REDIRECT_URI&state=YOUR_CUSTOM_CODE";
            return OathRequest("authorize", strb);
        }

        private class AccessToken
        {
            public string access_token { get; set; }
            public string code { get; set; }
            public string expires_in { get; set; }
            public string refresh_token { get; set; }
            public string scope { get; set; }
            public string time { get; set; }
            public string token_type { get; set; }
            public string uid { get; set; }
            public string user_nick { get; set; }
        }
    }
}