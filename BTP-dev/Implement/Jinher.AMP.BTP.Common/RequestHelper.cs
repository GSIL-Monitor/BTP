using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.Common
{
    /// <summary>
    /// web请求信息
    /// </summary>
    [Serializable()]
    [DataContract]
    public class RequestDTO<T>
    {
        /// <summary>
        /// 接口地址
        /// </summary>
        [DataMember]
        public string ServiceUrl { get; set; }

        /// <summary>
        /// 请求数据
        /// </summary>
        [DataMember]
        public T RequestData { get; set; }

        /// <summary>
        /// 是否含有时间类型字段
        /// </summary>
        [DataMember]
        public bool HasDateTime { get; set; }

        /// <summary>
        /// 请求数据对象名称
        /// </summary>
        [DataMember]
        public string RequestDataName { get; set; }

        /// <summary>
        /// 请求数据json字符串
        /// </summary>
        public string RequestDataJsonStr
        {
            get
            {
                if (typeof(T) == typeof(string)) return RequestData.ToString();
                var json = this.HasDateTime ? SerializationHelper.JsonSerializeForTime(this.RequestData) : SerializationHelper.JsonSerialize(this.RequestData);
                return json;
            }
        }

        /// <summary>
        /// 请求数据类型
        /// </summary>
        public string ContentType { get { return "application/json; charset=utf-8"; } }

        /// <summary>
        /// 请求数据编码类型
        /// </summary>
        public Encoding RequestDataEncoding { get { return Encoding.UTF8; } }

        /// <summary>
        /// 返回数据编码类型
        /// </summary>
        public Encoding ResponseDataEncoding { get { return Encoding.UTF8; } }

        /// <summary>
        /// 上下文
        /// </summary>
        [DataMember]
        public ContextDTO Context { get; set; }

        /// <summary>
        /// 单点登录cookie字符串(包含上下文中userid和sessionId)
        /// </summary>
        public string CookieContextStr
        {
            get
            {
                var context = this.Context;
                if (context == null) return string.Empty;
                var cookie = SerializationHelper.JsonSerialize(new { userId = context.LoginUserID, sessionId = context.SessionID, changeOrg = "" });
                cookie = HttpUtility.UrlEncode(cookie);
                if (string.IsNullOrEmpty(cookie)) return cookie;
                cookie = cookie.Replace("%7b%22", "%7B%22").Replace("%22%7d", "%22%7D").Replace("%22%3a%22", "%22%3A%22").Replace("%22%2c%22", "%22%2C%22");//Portal.Common的Cookie处理有bug
                cookie = "CookieContextDTO=" + cookie;
                return cookie;
            }
        }

        /// <summary>
        /// 上下文json的base64格式字符串
        /// </summary>
        public string ContextJsonBase64Str
        {
            get
            {
                var context = this.Context;
                if (context == null) return string.Empty;
                var contextJsonStr = SerializationHelper.JsonSerializeForTime(context);
                if (string.IsNullOrEmpty(contextJsonStr)) return string.Empty;
                var arr = RequestDataEncoding.GetBytes(contextJsonStr);
                var base64Str = Convert.ToBase64String(arr);
                return base64Str;
            }
        }

        /// <summary>
        /// 应用修改时间
        /// </summary>
        [DataMember]
        public bool IsCookie { get; set; }
    }

    /// <summary>
    /// 请求信息处理帮助类
    /// </summary>
    public class RequestHelper
    {
        /// <summary>
        /// 构造Web请求
        /// </summary>
        /// <typeparam name="T1">返回值泛型</typeparam>
        /// <typeparam name="T2">请求数据泛型</typeparam>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static T1 CreateRequest<T1, T2>(RequestDTO<T2> arg)
        {
            LogHelper.Debug("构造Web请求CreateRequest，入参：" + SerializationHelper.JsonSerialize(arg));
            WebRequest request = null;
            try
            {
                request = WebRequest.Create(arg.ServiceUrl);
                request.Method = "POST";
                request.ContentType = arg.ContentType;
                if (arg.Context != null)
                {
                    if (arg.IsCookie)
                    {
                        if (arg.CookieContextStr.Length > 0) request.Headers.Add("Cookie", arg.CookieContextStr);
                    }
                    else
                    {
                        if (arg.ContextJsonBase64Str.Length > 0) request.Headers.Add("ApplicationContext", arg.ContextJsonBase64Str);
                    }
                }
                var json = arg.RequestDataJsonStr;
                if (!string.IsNullOrEmpty(arg.RequestDataName))
                {
                    json = "{\"" + arg.RequestDataName + "\":" + json + "}";
                }
                LogHelper.Debug("构造Web请求CreateRequest，json：" + json);
                var bArr = arg.RequestDataEncoding.GetBytes(json);
                var postStream = request.GetRequestStream();
                postStream.Write(bArr, 0, bArr.Length);
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        if (stream != null)
                        {
                            using (StreamReader reader = new StreamReader(stream, arg.ResponseDataEncoding))
                            {
                                var retStr = reader.ReadToEnd();
                                LogHelper.Debug("构造Web请求CreateRequest，retStr:" + retStr);
                                if (!string.IsNullOrEmpty(retStr))
                                {
                                    T1 t = SerializationHelper.JsonDeserialize<T1>(retStr);
                                    if (typeof(T1).IsClass && t == null)
                                    {
                                        LogHelper.Error("构造Web请求CreateRequest失败,t=null");
                                    }
                                    return t;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("构造Web请求CreateRequest异常", ex);
            }
            finally
            {
                if (request != null) request.Abort();
            }
            return default(T1);
        }
    }
}