using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using Jinher.JAP.BF.IService.Interface;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.Deploy.CustomDTO.AfterSales;

namespace Jinher.AMP.BTP.IBP.IService
{
    /// <summary>
    /// 苏宁售后
    /// </summary>
    [ServiceContract]
    public interface ISNAfterSale : ICommand
    {
        ///// <summary>
        ///// 苏宁--单品申请售后
        ///// </summary>
        ///// <param name="reqDto"></param>
        ///// <param name="ty">订单状态  --厂送 非厂送 </param>
        ///// <returns></returns>
        //[WebInvoke(Method = "POST", UriTemplate = "/SnReturnPartOrder", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        //[OperationContract]
        //Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SnReturnPartOrder(SNReturnPartOrderDTO reqDto, SNFactoryDeliveryEnum ty);
        ///// <summary>
        ///// 苏宁--整单申请售后
        ///// </summary>
        ///// <param name="reqDto">--厂送 非厂送</param>
        ///// <returns></returns>
        //[WebInvoke(Method = "POST", UriTemplate = "/SnApplyRejected", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        //[OperationContract]
        //Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SnApplyRejected(SNApplyRejectedDTO reqDto, SNFactoryDeliveryEnum ty);


        /// <summary>
        /// 苏宁--获取订单数据接口
        /// </summary>
        /// <param name="reqDto"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetSNOrderItemList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<Jinher.AMP.BTP.Deploy.SNOrderItemDTO> GetSNOrderItemList(Jinher.AMP.BTP.Deploy.SNOrderItemDTO reqDto);
    }
}
