using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.EBC.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.CustomDTO.WeChat;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.EBC.ISV.Facade;

namespace Jinher.AMP.BTP.TPS
{
    /// <summary>
    /// 微信二维码
    /// </summary>
    public class WeChatQrCodeSV
    {
        private const string CreateQrCodeUrl = "https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token={0}";

        /// <summary>
        /// 永久二维码请求
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<string> CreateForeverQrcode(Jinher.AMP.BTP.Deploy.CustomDTO.WeChat.ForeverQrcodeDTO param)
        {
            //获取token
            var tokenInfo = GetToken(param);
            return CreateForeverQrcode(param, tokenInfo);
        }
        /// <summary>
        /// 永久二维码请求
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<string> CreateForeverQrcode(ForeverQrcodeDTO param, Deploy.CustomDTO.ResultDTO<string> tokenInfo)
        {
            var ret = new Deploy.CustomDTO.ResultDTO<string>();

            //获取token失败
            if (tokenInfo == null)
            {
                ret.Message = "获取微信access_token失败";
                return ret;
            }
            if (!tokenInfo.isSuccess)
            {
                ret.Message = tokenInfo.Message;
                return ret;
            }
            var token = tokenInfo.Data;
            var url = string.Format(CreateQrCodeUrl, token);
            var postJson = param.GetPostJson;
            var respText = WebRequestHelper.SendWebRequest(url, postJson, "Post", "application/json");
            var respInfo = JsonToDict(respText);
            if (respInfo.isSuccess)
            {
                var respDict = respInfo.Data;
                if (respDict.ContainsKey("ticket"))
                {
                    LogHelper.Info(string.Format("调用GetForeverQrcode()成功，参数access_token:{0}, 返回:{1}",
                        token, respText));
                    ret.Data = respDict["ticket"].ToString(); //成功
                    ret.isSuccess = true;
                }
                else
                    ret.Message = "返回的数据中没有ticket项，返回数据是：" + respText;
            }
            else
                ret.Message = respInfo.Message; //返回错误详情
            return ret;
        }

        /// <summary>
        /// 临时二维码请求
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<string> CreateTempQrcode(Jinher.AMP.BTP.Deploy.CustomDTO.WeChat.TempQrcodeDTO param)
        {
            //获取token
            var tokenInfo = GetToken(param);
            return CreateTempQrcode(param, tokenInfo);
        }
        /// <summary>
        /// 临时二维码请求
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<string> CreateTempQrcode(Jinher.AMP.BTP.Deploy.CustomDTO.WeChat.TempQrcodeDTO param, Deploy.CustomDTO.ResultDTO<string> tokenInfo)
        {
            var ret = new Deploy.CustomDTO.ResultDTO<string> { ResultCode = 0, isSuccess = false };

            if (tokenInfo == null)
            {
                ret.Message = "获取微信access_token失败";
                return ret;
            }
            //获取token失败
            if (!tokenInfo.isSuccess)
            {
                ret.Message = tokenInfo.Message;
                return ret;
            }

            var token = tokenInfo.Data;
            var url = string.Format(CreateQrCodeUrl, token);
            var postJson = param.GetPostJson;


            var respText = WebRequestHelper.SendWebRequest(url, postJson, "Post", "application/json");
            var respInfo = JsonToDict(respText);

            if (respInfo.isSuccess)
            {
                var respDict = respInfo.Data;
                if (respDict.ContainsKey("ticket"))
                {
                    LogHelper.Info(string.Format("调用GetTempQrcode成功，参数access_token:{0}, 返回:{1}",
                        token, respText));
                    ret.Data = respDict["ticket"].ToString(); //成功
                    ret.isSuccess = true;
                }
                else
                    ret.Message = "返回的数据中没有ticket项，返回数据是：" + respText;
            }
            else
                ret.Message = respInfo.Message; //返回错误详情

            return ret;
        }
        /// <summary>
        /// 获取应用access_token
        /// </summary>
        /// <param name="certif"></param>
        /// <returns></returns>
        public Deploy.CustomDTO.ResultDTO<string> GetToken(AccessCertifDTO certif)
        {
            if (certif == null || certif.JhAppId == Guid.Empty)
                return new ResultDTO<string>();
            return GetToken(certif.JhAppId);
        }
        /// <summary>
        /// 获取应用access_token
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Deploy.CustomDTO.ResultDTO<string> GetToken(Guid appId)
        {
            Deploy.CustomDTO.ResultDTO<string> result = new ResultDTO<string>();
            var facadeResult = WCPSV.Instance.GetWXAccessToken(appId);
            if (facadeResult == null)
            {
                result.Data = "获取微信access_token失败";
                return result;
            }
            if (facadeResult.IsSuccess)
            {
                result.isSuccess = true;
            }
            result.Data = facadeResult.Message;
            return result;
        }

        /// <summary>
        /// 将JSON字符串转换为Dictionary
        /// </summary>
        public Deploy.CustomDTO.ResultDTO<Dictionary<string, object>> JsonToDict(string jsonData)
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


    }

}
