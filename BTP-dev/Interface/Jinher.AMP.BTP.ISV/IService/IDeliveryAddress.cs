
/***************
功能描述: BTPIService
作    者: 
创建时间: 2014/3/24 13:47:25
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
    /// 收货地址接口
    /// </summary>
    [ServiceContract]
    public interface IDeliveryAddress : ICommand
    {

        /// <summary>
        /// 添加收货地址
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.DeliveryAddressSV.svc/GetAllCommodityOrder
        /// </para>
        /// </summary>
        /// <param name="addressDTO">地址实体</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveDeliveryAddress", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SaveDeliveryAddress(AddressSDTO addressDTO);

        /// <summary>
        /// 获取收货地址列表
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.DeliveryAddressSV.svc/GetDeliveryAddressList
        /// </para>
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetDeliveryAddressList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<AddressSDTO> GetDeliveryAddressList(Guid userId, Guid appId, int IsDefault);

        /// <summary>
        /// 获取收货地址列表
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.DeliveryAddressSV.svc/GetDeliveryAddress
        /// </para>
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetDeliveryAddress", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<AddressSDTO> GetDeliveryAddress(Guid userId, Guid appId);

        /// <summary>
        /// 删除收货地址
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.DeliveryAddressSV.svc/DeleteDeliveryAddress
        /// </para>
        /// </summary>
        /// <param name="addressId">地址ID</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DeleteDeliveryAddress", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DeleteDeliveryAddress(Guid addressId, Guid appId);

        /// <summary>
        /// 编辑收货地址 设置默认地址
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.DeliveryAddressSV.svc/UpdateDeliveryAddressIsDefault
        /// </para>
        /// </summary>
        /// <param name="addressId">地址ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateDeliveryAddressIsDefault", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateDeliveryAddressIsDefault(Guid addressId);

        /// <summary>
        /// 编辑收货地址
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.DeliveryAddressSV.svc/UpdateDeliveryAddress
        /// </para>
        /// </summary>
        /// <param name="addressDTO">地址实体</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateDeliveryAddress", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateDeliveryAddress(AddressSDTO addressDTO, Guid appId);

        /// <summary>
        /// 收货地址详情
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.DeliveryAddressSV.svc/GetDeliveryAddressByAddressId
        /// </para>
        /// </summary>
        /// <param name="addressId">地址ID</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetDeliveryAddressByAddressId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        AddressSDTO GetDeliveryAddressByAddressId(Guid addressId, Guid appId);


        /// <summary>
        /// 保存（添加或修改）收货地址
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.DeliveryAddressSV.svc/SaveDeliveryAddressNew
        /// </para>
        /// </summary>
        /// <param name="addressDTO">地址实体</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveDeliveryAddressNew", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SaveDeliveryAddressNew(AddressSDTO addressDTO);
    }
}
