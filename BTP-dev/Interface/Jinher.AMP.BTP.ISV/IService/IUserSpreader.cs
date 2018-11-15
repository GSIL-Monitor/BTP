using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel;
using System.ServiceModel.Web;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.ISV.IService
{
    [ServiceContract]
    public interface IUserSpreader : ICommand
    {

        /// <summary>
        /// 保存买家微信和推广者（推广码）之间的关系。
        /// <para>Service Url: http://devbtp.iuoooo.com/Jinher.AMP.BTP.SV.UserSpreaderSV.svc/SaveSpreaderAndBuyerWxRel
        /// </para>
        /// </summary>
        /// <param name="sbwxDto">参数只传SpreadCode、WxOpenId</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveSpreaderAndBuyerWxRel", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SaveSpreaderAndBuyerWxRel(SpreaderAndBuyerWxDTO sbwxDto);

        /// <summary>
        /// 更新订单推广者信息。
        /// <para>Service Url: http://devbtp.iuoooo.com/Jinher.AMP.BTP.SV.UserSpreaderSV.svc/UpdateOrderSpreader
        /// </para>
        /// </summary>
        /// <param name="sbwxDto">参数只传WxOpenId、BuyerId、OrderId</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateOrderSpreader", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateOrderSpreader(SpreaderAndBuyerWxDTO sbwxDto);

        /// <summary>
        /// 更新用户为推广主
        /// </summary>
        /// <param name="spreaderDto">推广者dto</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateToSpreader", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateToSpreader(SpreaderAndBuyerWxDTO spreaderDto);

        /// <summary>
        /// 绑定关系
        /// </summary>
        /// <param name="userSpreaderBindDTO">参数只传SpreadCode、UserID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveUserSpreaderCode", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveUserSpreaderCode(Jinher.AMP.BTP.Deploy.CustomDTO.UserSpreaderBindDTO userSpreaderBindDTO);
    }
}
