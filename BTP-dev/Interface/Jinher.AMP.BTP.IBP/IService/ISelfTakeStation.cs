
/***************
功能描述: BTPIService
作    者: 
创建时间: 2014/3/25 14:56:38
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
    /// <summary>
    /// 自提点
    /// </summary>
    [ServiceContract]
    public interface ISelfTakeStation : ICommand
    {
        /// <summary>
        /// 添加自提点
        /// </summary>
        /// <param name="selfTakeStationDTO">自提点实体</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveSelfTakeStation", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveSelfTakeStation(Jinher.AMP.BTP.Deploy.CustomDTO.SelfTakeStationAndManagerDTO selfTakeStationDTO);

        /// <summary>
        /// 修改自提点
        /// </summary>
        /// <param name="selfTakeStationDTO">自提点实体</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateSelfTakeStation", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateSelfTakeStation(Jinher.AMP.BTP.Deploy.CustomDTO.SelfTakeStationAndManagerDTO selfTakeStationDTO);

        /// <summary>
        /// 删除多个自提点
        /// </summary>
        /// <param name="ids">自提点ID集合</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DeleteSelfTakeStations", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteSelfTakeStations(System.Collections.Generic.List<System.Guid> ids);

        /// <summary>
        /// 查询自提点信息
        /// </summary>
        /// <param name="id">自提点ID</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllSelfTakeStation", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.SelfTakeStationResult> GetAllSelfTakeStation(Jinher.AMP.BTP.Deploy.CustomDTO.SelfTakeStationSearchSDTO selfTakeStationSearch, out int rowCount);


        /// <summary>
        /// 获取自提点信息
        /// </summary>
        /// <param name="id">自提点ID</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetSelfTakeStationById", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.SelfTakeStationResult GetSelfTakeStationById(System.Guid id);

        /// <summary>
        /// 查询负责人是否已存在
        /// </summary>
        /// <param name="userId">负责人IU平台用户ID</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CheckSelfTakeStationManagerByUserId", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CheckSelfTakeStationManagerByUserId(System.Guid userId);

        /// <summary>
        /// 删除负责人信息
        /// </summary>
        /// <param name="ids">id列表</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DeleteSelfTakeStationManagerById", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteSelfTakeStationManagerById(System.Collections.Generic.List<System.Guid> ids);
        /// <summary>
        /// 获取所有自提点
        /// </summary>
        /// <param name="AppId">卖家id</param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllAppSelfTakeStation", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<AppSelfTakeStationResultDTO> GetAllAppSelfTakeStation(Guid appId, int pageSize, int pageIndex, out  int rowCount);
        /// <summary>
        /// 删除自提点
        /// </summary>
        /// <param name="id">自提点ID</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DeleteAppSelfTakeStation", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteAppSelfTakeStation(Guid id);
        /// <summary>
        /// 根据条件查询所有自提点
        /// </summary>
        /// <param name="AppId">卖家ID</param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowCount"></param>
        /// <param name="Name"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="district"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllAppSelfTakeStationByWhere", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationResultDTO> GetAllAppSelfTakeStationByWhere(Guid appId, int pageSize, int pageIndex, out int rowCount, string Name, string province, string city, string district);
    }
}
