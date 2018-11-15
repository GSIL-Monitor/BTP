using System;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.Common
{
    /// <summary>
    /// Web请求处理
    /// </summary>
    public static class BaseRequest
    {
        /// <summary>
        /// 构造Web请求
        /// </summary>
        /// <param name="serviceUrl">接口地址</param>
        /// <param name="requestData">请求数据(json字符串)</param>
        /// <param name="isNeedAuth">是否需要登录验证</param>
        /// <returns></returns>
        public static string CreateRequest(string serviceUrl, string requestData, bool isNeedAuth)
        {
            requestData = string.IsNullOrEmpty(requestData) ? "whatever" : requestData;
            string retStr = string.Empty;
            BaseWebRequest request = new BaseWebRequest();
            HttpContentDTO httpDto = new HttpContentDTO()
            {
                ServerUrl = serviceUrl,
                RequestData = Encoding.GetEncoding("UTF-8").GetBytes(requestData)
            };
            if (isNeedAuth)
            {
                retStr = request.BaseHttpWebRequestWithAuth(httpDto);
            }
            else
            {
                retStr = request.BaseHttpWebRequest(httpDto);
            }
            return retStr;
        }

        /// <summary>
        /// 构造Web请求
        /// </summary>
        /// <param name="serviceUrl">接口地址</param>
        /// <param name="requestData">请求数据(json字符串)</param>
        /// <param name="isNeedAuth">是否需要登录验证</param>
        /// <returns></returns>
        public static T CreateRequest<T>(string serviceUrl, string requestData, bool isNeedAuth)
        {
            string resultStr = string.Empty;
            T result = default(T);
            try
            {
                resultStr = CreateRequest(serviceUrl, requestData, isNeedAuth);
                if (!string.IsNullOrEmpty(resultStr))
                    result = JsonHelper.JsonDeserialize<T>(resultStr);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("BaseRequest.CreateRequest,serviceUrl:{0},requestData:{1},isNeedAuth:{2},result:{3}", serviceUrl, requestData, isNeedAuth, resultStr),ex);
            }
            return result;
        }

        /// <summary>
        /// 构造Web请求
        /// </summary>
        /// <param name="serviceUrl">接口地址</param>
        /// <param name="requestData">请求数据</param>
        /// <param name="isNeedAuth">是否需要登录验证</param>
        /// <param name="isNeedAuth">请求数据对象名称</param>
        /// <returns></returns>
        public static string CreateRequest(string serviceUrl, object requestData, bool isNeedAuth, string requestDataName = "")
        {
            string json = JsonHelper.JsonSerializer(requestData);
            if (requestDataName.Length > 0)
            {
                json = "{\"" + requestDataName + "\":" + json + "}";
            }
            return CreateRequest(serviceUrl, json, isNeedAuth);
        }

        /// <summary>
        /// 构造Web请求
        /// </summary>
        /// <typeparam name="T">请求数据泛型</typeparam>
        /// <param name="serviceUrl">接口地址</param>
        /// <param name="requestData">请求数据</param>
        /// <param name="isNeedAuth">是否需要登录验证</param>
        /// <param name="isNeedAuth">请求数据对象名称</param>
        /// <returns></returns>
        public static string CreateRequest<T>(string serviceUrl, T requestData, bool isNeedAuth, string requestDataName = "")
        {
            string json = string.Empty;
            if (requestData.GetType() == "string".GetType())
            {
                json = requestData.ToString();
            }
            else
            {
                json = JsonHelper.JsonSerializer<T>(requestData);
            }
            if (requestDataName.Length > 0)
            {
                json = "{\"" + requestDataName + "\":" + json + "}";
            }
            return CreateRequest(serviceUrl, json, isNeedAuth);
        }
    }
}
