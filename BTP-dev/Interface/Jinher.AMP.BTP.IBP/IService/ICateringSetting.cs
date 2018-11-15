
/***************
功能描述: BTPIService
作    者: 
创建时间: 2016/12/3 14:06:15
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
    public interface ICateringSetting : ICommand
    {

        [WebInvoke(Method = "POST", UriTemplate = "/AddCateringSetting", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddCateringSetting(Deploy.CustomDTO.FCYSettingCDTO settingDTO);

        [WebInvoke(Method = "POST", UriTemplate = "/UpdateCateringSetting", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateCateringSetting(Deploy.CustomDTO.FCYSettingCDTO settingDTO);

        [WebInvoke(Method = "POST", UriTemplate = "/GetCateringSetting", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Deploy.CustomDTO.FCYSettingCDTO GetCateringSetting(Guid storeId);


        /// <summary>
        /// 获取餐饮门店设置
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCateringSettingByAppId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Deploy.CustomDTO.FCYSettingCDTO GetCateringSettingByAppId(Guid appId);



        /// <summary>
        /// 获取餐饮门店设置
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCateringSettingByStoreIds", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<Deploy.CustomDTO.FCYSettingCDTO> GetCateringSettingByStoreIds(List<Guid> storeIds);


    }
}
