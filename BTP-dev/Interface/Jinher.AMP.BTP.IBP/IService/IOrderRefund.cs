
/***************
功能描述: BTPIService
作    者: 
创建时间: 2018/10/10 14:32:43
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

namespace Jinher.AMP.BTP.IBP.IService
{

    [ServiceContract]
    public interface IOrderRefund : ICommand
    {
        [WebInvoke(Method = "POST", UriTemplate = "/GetOrderRefund", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundCompareDTO>> GetOrderRefund(OrderRefundSearchDTO search);
    }
}
