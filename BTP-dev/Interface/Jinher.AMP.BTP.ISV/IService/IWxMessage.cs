
/***************
功能描述: BTPIService
作    者: 
创建时间: 2014/3/24 13:19:55
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using Jinher.JAP.BF.IService.Interface;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.ISV.IService
{
    /// <summary>
    /// 商品分类接口
    /// </summary>
    [ServiceContract]
    public interface IWxMessage : ICommand
    {
        /// <summary>
        /// 处理微信消息推送
        /// <para>Service URL：
        /// <a href="http://btp.iuoooo.com/Jinher.AMP.BTP.SV.WxMessageSV.svc/DealMessage">
        /// http://btp.iuoooo.com/Jinher.AMP.BTP.SV.WxMessageSV.svc/DealMessage
        /// </a></para>
        /// </summary>
        /// <param name="message">微信消息</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DealMessage", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.WcpBusiRetDto DealMessage(Jinher.AMP.BTP.Deploy.CustomDTO.WcpBusiDto message);
    }
}
