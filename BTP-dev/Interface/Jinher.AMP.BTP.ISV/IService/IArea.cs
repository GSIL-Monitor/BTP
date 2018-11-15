
/***************
功能描述: BTPIService
作    者: 
创建时间: 2015/1/7 16:26:53
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using Jinher.JAP.BF.IService.Interface;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO.ThirdECommerce;

namespace Jinher.AMP.BTP.ISV.IService
{
    /// <summary>
    /// 获取四级地址信息
    /// </summary>
    [ServiceContract]
    public interface IArea : ICommand
    {
        /// <summary>
        /// 获取一级地址
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetProvince", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ThirdResponse<Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDTO> GetProvince();


        /// <summary>
        /// 获取二级地址
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCity", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ThirdResponse<Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDTO> GetCity(string Code);



        /// <summary>
        /// 获取三级地址
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCounty", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ThirdResponse<Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDTO> GetCounty(string Code);


        /// <summary>
        /// 获取四级地址
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetTown", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ThirdResponse<Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDTO> GetTown(string Code);
    }
}
