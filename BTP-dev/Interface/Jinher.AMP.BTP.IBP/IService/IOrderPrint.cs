
/***************
功能描述: BTPIService
作    者: 
创建时间: 2017/2/16 15:24:29
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using Jinher.JAP.BF.IService.Interface;
using Jinher.AMP.BTP.Deploy;

namespace Jinher.AMP.BTP.IBP.IService
{

    [ServiceContract]
    public interface IOrderPrint : ICommand
    {
        [WebInvoke(Method = "POST", UriTemplate = "/GetPrintOrder", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<Jinher.AMP.BTP.Deploy.CustomDTO.PrintOrderDTO> GetPrintOrder(List<Guid> orderIds);

         /// <summary>
        /// 打印快递单更新保存数据
        /// </summary>
        /// <param name="orders"></param>
        [WebInvoke(Method = "POST", UriTemplate = "/SavePrintOrders", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SavePrintOrders(Jinher.AMP.BTP.Deploy.CustomDTO.UpdatePrintDTO orders);


        /// <summary>
        /// 打印发货单更新保存数据
        /// </summary>
        /// <param name="orders"></param>
        [WebInvoke(Method = "POST", UriTemplate = "/SavePrintInvoiceOrders", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SavePrintInvoiceOrders(Jinher.AMP.BTP.Deploy.CustomDTO.UpdatePrintDTO orders);

    }
}
