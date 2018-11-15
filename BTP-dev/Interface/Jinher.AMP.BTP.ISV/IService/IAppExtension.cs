using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel;
using System.ServiceModel.Web;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy;

namespace Jinher.AMP.BTP.ISV.IService
{
    /// <summary>
    /// 应用扩展（店铺扩展）
    /// </summary>
    [ServiceContract]
    public interface IAppExtension : ICommand
    {
        /// <summary>
        /// 查询电商应用级信息
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetBTPAppInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Deploy.CustomDTO.AppExtDTO GetBTPAppInfo(Deploy.CustomDTO.AppSearchDTO search);

        /// <summary>
        /// 获取特定app在电商中的扩展信息。
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAppExtensionByAppId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<Jinher.AMP.BTP.Deploy.AppExtensionDTO> GetAppExtensionByAppId(Guid appId);

        /// <summary>
        /// 获取特定app下载所需数据
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAppDownLoadInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.AppDownloadDTO> GetAppDownLoadInfo(Guid appId);
       
    }
}
