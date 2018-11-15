
/***************
功能描述: BTPIService
作    者: 
创建时间: 2014/3/19 9:53:46
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
    public interface IDeliveryAddress : ICommand
    {

        /// <summary>
        /// 添加收货地址
        /// </summary>
        /// <param name="deliveryAddressDTO">收货地址实体</param>
        [WebInvoke(Method = "POST", UriTemplate = "/AddDeliveryAddress", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void AddDeliveryAddress(DeliveryAddressDTO deliveryAddressDTO);

        /// <summary>
        /// 删除收货地址
        /// </summary>
        /// <param name="id">地址ID</param>
        [WebInvoke(Method = "POST", UriTemplate = "/DeleteDeliveryAddress", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void DeleteDeliveryAddress(Guid id);

        /// <summary>
        /// 修改收货地址
        /// </summary>
        /// <param name="commodityDTO">地址实体</param>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateDeliveryAddress", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void UpdateDeliveryAddress(Jinher.AMP.BTP.Deploy.DeliveryAddressDTO commodityDTO);

        /// <summary>
        /// 查询用户收货地址
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllDeliveryAddress", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<DeliveryAddressDTO> GetAllDeliveryAddress(Guid userId);

        /// <summary>
        /// 查询一条收货地址
        /// </summary>
        /// <param name="id">地址ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetDeliveryAddress", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        DeliveryAddressDTO GetDeliveryAddress(Guid id);
    }
}
