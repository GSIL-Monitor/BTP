
/***************
功能描述: BTPIService
作    者: 
创建时间: 2014/3/20 19:31:22
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
    /// 订单接口
    /// </summary>
    [ServiceContract]
    public interface ICreateOrder : ICommand
    {
        /// <summary>
        /// 获取拼团详情
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCreateOrderInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<CreateOrderNeedDTO> GetCreateOrderInfo(CreateOrderSearchDTO search);
    }
}