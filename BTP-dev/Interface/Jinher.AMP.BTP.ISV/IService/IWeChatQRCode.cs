
/***************
功能描述: BTPIService
作    者: 
创建时间: 2016/12/10 15:32:49
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using Jinher.JAP.BF.IService.Interface;
using Jinher.AMP.BTP.Deploy.CustomDTO.WeChat;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.ISV.IService
{

    [ServiceContract]
    public interface IWeChatQRCode : ICommand
    {
        /// <summary>
        /// 永久二维码请求
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/CreateForeverQrcode", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<string> CreateForeverQrcode(ForeverQrcodeDTO param);

        /// <summary>
        /// 临时二维码请求
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/CreateTempQrcode", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<string> CreateTempQrcode(TempQrcodeDTO param);

        /// <summary>
        /// AccessToken请求
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAccessToken", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<string> GetAccessToken(string appId, string appSecret);

        /// <summary>
        /// 发送消息
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/SendMsg", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<string> SendMsg(SendMsgDTO param);

        /// <summary>
        /// 发送消息
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/Repaire", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO Repaire();
    }
}
