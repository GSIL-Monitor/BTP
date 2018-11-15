
/***************
功能描述: BTPIService
作    者: 
创建时间: 2018/7/6 11:21:32
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
    public interface ICommodityOrderItem : ICommand
    {

        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityOrderItemByUserId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<List<Jinher.AMP.BTP.Deploy.CustomDTO.Commodity.CommodityAndOrderItemDTO>> GetCommodityOrderItemByUserId(string UserId, int PageSize, int PageIndex);


        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityOrderItemByOrderId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]

        ResultDTO<List<Jinher.AMP.BTP.Deploy.CustomDTO.Commodity.CommodityAndOrderItemDTO>> GetCommodityOrderItemByOrderId(Guid orderId);

    }
}
