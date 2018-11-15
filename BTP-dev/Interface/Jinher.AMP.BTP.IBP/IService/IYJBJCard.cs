using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.BF.IService.Interface;

namespace Jinher.AMP.BTP.IBP.IService
{
    [ServiceContract]
    public interface IYJBJCard : ICommand
    {
        [WebInvoke(Method = "POST", UriTemplate = "/Create", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO Create(Guid orderId);

        [WebInvoke(Method = "POST", UriTemplate = "/Get", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<YJBJCardDTO> Get(Guid orderId);
    }
}
