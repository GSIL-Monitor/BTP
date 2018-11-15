
/***************
功能描述: BTPSV
作    者: 
创建时间: 2016/12/10 15:46:23
***************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.AMP.BTP.TPS;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.Common.TypeDefine;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.Common.Loging;
using System.Net;
using System.IO;
using Jinher.AMP.BTP.Deploy.CustomDTO.WeChat;
using System.Web.Script.Serialization;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    public partial class WeChatQRCodeSV : BaseSv, IWeChatQRCode
    {

        /// <summary>
        /// 永久二维码请求
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<string> CreateForeverQrcodeExt(Jinher.AMP.BTP.Deploy.CustomDTO.WeChat.ForeverQrcodeDTO param)
        {
            TPS.WeChatQrCodeSV worker = new WeChatQrCodeSV();
            return worker.CreateForeverQrcode(param);
        }
        /// <summary>
        /// 临时二维码请求
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<string> CreateTempQrcodeExt(Jinher.AMP.BTP.Deploy.CustomDTO.WeChat.TempQrcodeDTO param)
        {
            TPS.WeChatQrCodeSV worker = new WeChatQrCodeSV();
            return worker.CreateTempQrcode(param);
        }

        /// <summary>
        /// AccessToken请求
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<string> GetAccessTokenExt(string appId, string appSecret)
        {
            return
                GetTokenFromMemory(new AccessCertifDTO
                {
                    UseDeveloperId = true,
                    AccessToken = "",
                    AppId = appId,
                    AppSecret = appSecret
                });
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<string> SendMsgExt(Jinher.AMP.BTP.Deploy.CustomDTO.WeChat.SendMsgDTO param)
        {
            var ret = new Deploy.CustomDTO.ResultDTO<string> { ResultCode = 0, isSuccess = false };

            var tokenInfo = GetTokenFromMemory(param);
            if (!tokenInfo.isSuccess)
                return tokenInfo;

            var token = tokenInfo.Data;

            var url = "https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token={0}";
            url = string.Format(url, token);
            var postJson = param.GetPostJson;

            try
            {
                var respText = SendWebRequest(url, postJson, "Post", "application/json");
                var respInfo = JsonToDict(respText);

                if (respInfo.isSuccess)
                {
                    var respDict = respInfo.Data;
                    if (respDict.ContainsKey("errmsg"))
                    {
                        var errmsg = respDict["errmsg"].ToString();
                        if (errmsg == "ok")
                        {
                            LogHelper.Info(string.Format("调用SendMsg成功，参数content:{0}", param.Content));
                            ret.isSuccess = true;
                        }
                        else
                        {
                            ret.Message = errmsg;
                            ret.ResultCode = Convert.ToInt32(respDict["errcode"]);
                        }
                    }
                    else
                        ret.Message = "返回的数据中没有errmsg项，返回数据是：" + respText;
                }
                else
                    ret.Message = respInfo.Message; //返回错误详情
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("调用SendMsg异常，参数content：{0}", param.Content), ex);
                ret.Message = ex.Message;
            }
            return ret;
        }

        #region private method

        private Deploy.CustomDTO.ResultDTO<string> GetTokenFromMemory(AccessCertifDTO certif)
        {
            if (certif == null)
                return new ResultDTO<string>();
            if (certif.IsAppWeChatSetting)
            {
                return GetTokenFromMemoryByAppId(certif);
            }
            else
            {
                return GetTokenFromMemorySelf(certif);
            }
        }
        private Deploy.CustomDTO.ResultDTO<string> GetTokenFromMemoryByAppId(AccessCertifDTO certif)
        {
            Deploy.CustomDTO.ResultDTO<string> result = new ResultDTO<string>();
            if (certif == null || certif.JhAppId == Guid.Empty)
                return result;
            var facadeResult = WCPSV.Instance.GetWXAccessToken(certif.JhAppId);
            if (facadeResult != null && facadeResult.IsSuccess)
            {
                result.isSuccess = true;
                result.Data = facadeResult.Message;
            }
            return result;
        }
        /// <summary>
        /// AccessToken请求
        /// </summary>
        private Deploy.CustomDTO.ResultDTO<string> GetTokenFromMemorySelf(AccessCertifDTO certif)
        {
            var ret = new Deploy.CustomDTO.ResultDTO<string> { ResultCode = 0, isSuccess = false };
            var tokenStr = string.Empty;

            try
            {
                //直接从参数DTO中读取Token
                if (!certif.UseDeveloperId)
                {
                    tokenStr = certif.AccessToken;
                    LogHelper.Info(string.Format(
                        "GetTokenFromMemory 参数accessCertif中已经包含AccessToken={0}", tokenStr));
                    ret.Data = tokenStr;
                    ret.isSuccess = true;
                }
                else
                {
                    var appId = certif.AppId;
                    var appSecret = certif.AppSecret;

                    //从缓存中读取到token
                    if (WeChatMemoryCache.TryGetToken(appId, appSecret, ref tokenStr))
                    {
                        LogHelper.Info(string.Format(
                            "GetTokenFromMemory 从缓存中获取AccessToken={0}", tokenStr));
                        ret.Data = tokenStr;
                        ret.isSuccess = true;
                    }
                    else //未能从缓存中读取到token
                    {
                        //调用微信接口获取token
                        ret = GetTokenFromWx(appId, appSecret);
                        if (ret.isSuccess)
                        {
                            WeChatMemoryCache.StorageToken(appId, appSecret, ret.Data);
                            LogHelper.Info(string.Format(
                                "GetTokenFromMemory 调用微信服务获取AccessToken={0}", tokenStr));
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(
                    string.Format("调用GetToken异常{0},参数UseDeveloperId:{1},AppId:{2},AppSecret:{3},AccessToken:{4}",
                        ex.Message, certif.UseDeveloperId, certif.AppId, certif.AppSecret, certif.AccessToken), ex);
                ret.Message = ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 获取AccessToken
        /// </summary>
        /// <returns>AccessToken</returns>
        private Deploy.CustomDTO.ResultDTO<string> GetTokenFromWx(string appId, string appSecret)
        {
            var ret = new Deploy.CustomDTO.ResultDTO<string> { ResultCode = 0, isSuccess = false };
            var url = string.Format(
                "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}",
                appId, appSecret);

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                var respText = "";
                using (Stream respStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(respStream, Encoding.Default);
                    respText = reader.ReadToEnd();
                }
                var respInfo = JsonToDict(respText);
                if (respInfo.isSuccess)
                {
                    var respDict = respInfo.Data;
                    if (respDict.ContainsKey("access_token"))
                    {
                        ret.Data = respDict["access_token"].ToString(); //成功
                        ret.isSuccess = true;
                    }
                    else
                        ret.Message = respText; //返回错误详情
                }
                else
                    ret.Message = respInfo.Message;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("调用GetTokenFromWx异常，参数appId:{0}, appSecret:{1}", appId, appSecret), ex);
                ret.Message = ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 将JSON字符串转换为Dictionary
        /// </summary>
        Deploy.CustomDTO.ResultDTO<Dictionary<string, object>> JsonToDict(string jsonData)
        {
            var returns = new Deploy.CustomDTO.ResultDTO<Dictionary<string, object>> { ResultCode = 0, isSuccess = false };
            var jss = new JavaScriptSerializer();
            try
            {
                var dictData = jss.Deserialize<Dictionary<string, object>>(jsonData);
                if (dictData.Count > 0)
                {
                    returns.Data = dictData;
                    returns.isSuccess = true;
                }
                else
                {
                    returns.Message = "调用JsonToDict失败，没有得到任何结果。参数jsonDate:" + jsonData;
                }
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("调用JsonToDict异常，参数jsonData：{0}", jsonData), ex);
                returns.Message = ex.Message;
            }
            return returns;
        }
        /// <summary>
        /// 发送WebRequest请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="strData"></param>
        /// <param name="reqMethod"></param>
        /// <param name="reqContentType"></param>
        /// <returns></returns>
        string SendWebRequest(string url, string strData, string reqMethod, string reqContentType)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = reqMethod;
            request.ContentType = reqContentType;

            Stream newStream = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(strData))
                {
                    UTF8Encoding encoding = new UTF8Encoding();
                    byte[] data = encoding.GetBytes(strData);

                    request.ContentLength = data.Length;
                    // 发送数据
                    newStream = request.GetRequestStream();
                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                }
            }
            catch (Exception ex)
            {
                JAP.Common.Loging.LogHelper.Error("Error_LogKey:WeChatQRCodeSV::SendPostInfo::发送数据");
            }
            finally
            {
                if (newStream != null)
                {
                    newStream.Close();
                }
            }

            // 获得返回数据
            string strResponse = string.Empty;
            WebResponse webResponse = null;
            StreamReader responseStream = null;
            try
            {
                webResponse = request.GetResponse();
                responseStream = new StreamReader(webResponse.GetResponseStream(), System.Text.Encoding.UTF8);
                strResponse = responseStream.ReadToEnd();
                webResponse.Close();
                responseStream.Close();
            }
            catch (Exception ex)
            {
                JAP.Common.Loging.LogHelper.Error("Error_LogKey:WeChatQRCodeSV::SendPostInfo::GetResponse");
                strResponse = "请求失败，请重试！";
            }
            finally
            {
                if (webResponse != null)
                {
                    webResponse.Close();
                }
                if (responseStream != null)
                {
                    responseStream.Close();
                }
            }
            return strResponse;
        }
        #endregion

        private ResultDTO RepaireExt()
        {
            ResultDTO result = new ResultDTO() { isSuccess = true };

            return result;
        }
    }



    /// <summary>
    /// 微信数据的Memory缓存
    /// </summary>
    public class WeChatMemoryCache
    {
        private const int ValidSeconds = 7000; //安全起见，保存时间要略小于微信的规定。所以选择：7200 - 200
        private const int RemoveValidHours = 3;
        private static readonly Dictionary<string, LimitedKv> WebChatTokens = new Dictionary<string, LimitedKv>();
        private static int _minute = -1;

        #region public method

        /// <summary>
        /// 尝试从缓存中读取token
        /// </summary>
        public static bool TryGetToken(string appId, string appSecret, ref string token)
        {
            var now = DateTime.Now;

            //清理过期的token数据
            RemoveHistoryTokens(now);

            var key = TokensKey(appId, appSecret);
            if (WebChatTokens.ContainsKey(key))
            {
                var kvObj = WebChatTokens[key];
                if (kvObj.IsValid(now, ValidSeconds))
                {
                    token = kvObj.Value;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 将token缓存到Memory
        /// </summary>
        public static void StorageToken(string appId, string appSecret, string token)
        {
            var key = TokensKey(appId, appSecret);
            var kvObj = new LimitedKv(key, token);
            WebChatTokens[key] = kvObj;
        }

        #endregion

        #region private method

        /// <summary>
        /// 获取token时的Key
        /// </summary>
        private static string TokensKey(string appId, string appSecret)
        {
            return appId + "---" + appSecret;
        }

        /// <summary>
        /// 定期清理过期数据
        /// </summary>
        private static void RemoveHistoryTokens(DateTime now)
        {
            if (_minute != now.Minute && _minute >= 0)
            {
                _minute = now.Minute;
                foreach (var k in WebChatTokens.Keys)
                {
                    if (WebChatTokens[k].CanRemove(now, RemoveValidHours))
                        WebChatTokens.Remove(k);
                }
            }
        }

        #endregion

        #region 内部类，用于存储KV

        /// <summary>
        /// 一个有有效期的K-V类
        /// </summary>
        public class LimitedKv
        {
            public string Key { get; set; }
            public string Value { get; set; }
            public DateTime UpdateTime { get; set; }

            public LimitedKv(string key, string value)
                : this(key, value, DateTime.Now)
            {
            }

            public LimitedKv(string key, string value, DateTime updaTime)
            {
                Key = key;
                Value = value;
                UpdateTime = DateTime.Now;
            }

            /// <summary>
            /// 是否有效
            /// </summary>
            public bool IsValid(DateTime now, int validSeconds)
            {
                return (now - UpdateTime).Seconds < validSeconds;
            }

            /// <summary>
            /// 是否可以清除
            /// </summary>
            public bool CanRemove(DateTime now, int removeValidHours)
            {
                return (now - UpdateTime).Hours > removeValidHours;
            }
        }
        #endregion
    }
}