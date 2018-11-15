
/***************
功能描述: BTPIService
作    者: 
创建时间: 2014/3/19 11:25:47
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
    /// 服务项设置
    /// </summary>
    [ServiceContract]
    public interface IServiceSettings : ICommand
    {
        /// <summary>
        /// 查询所有的服务项设置信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetALLServiceSettingsList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<Jinher.AMP.BTP.Deploy.ServiceSettingsDTO> GetALLServiceSettingsList(Jinher.AMP.BTP.Deploy.ServiceSettingsDTO model);


        /// <summary>
        /// 根据ids集合获取所有的的内容
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetServiceSettingsList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<Jinher.AMP.BTP.Deploy.ServiceSettingsDTO> GetServiceSettingsList(List<Guid> ids);



        /// <summary>
        /// 保存ServiceSettings信息
        /// </summary>
        /// <param name="VatInvoiceProof"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveServiceSettings", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SaveServiceSettings(Jinher.AMP.BTP.Deploy.ServiceSettingsDTO model);


        /// <summary>
        /// 根据id修改ServiceSettings设置信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateServiceSettings", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateServiceSettings(Jinher.AMP.BTP.Deploy.ServiceSettingsDTO model);


        /// <summary>
        /// 根据id删除ServiceSettings设置信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DeleteServiceSettings", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DeleteServiceSettings(Guid id);

        /// <summary>
        /// 根据AppId获取实体的内容
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetServiceSettings", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.ServiceSettingsDTO GetServiceSettings(Guid AppId);


     
    }
}
