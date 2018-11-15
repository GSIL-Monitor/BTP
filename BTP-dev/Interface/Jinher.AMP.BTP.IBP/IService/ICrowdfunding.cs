using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.BF.IService.Interface;

namespace Jinher.AMP.BTP.IBP.IService
{
    /// <summary>
    /// 众筹相关接口
    /// </summary>
    [ServiceContract]
    public interface ICrowdfunding : ICommand
    {

        /// <summary>
        /// 添加众筹
        /// </summary>
        /// <param name="crowdfundingDTO">众筹实体</param>
        [WebInvoke(Method = "POST", UriTemplate = "/AddCrowdfunding", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO AddCrowdfunding(CrowdfundingDTO crowdfundingDTO);
        /// <summary>
        /// 更新众筹
        /// </summary>
        /// <param name="crowdfundingDTO">众筹实体</param>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateCrowdfunding", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateCrowdfunding(CrowdfundingDTO crowdfundingDTO);

        /// <summary>
        /// 获取众筹
        /// </summary>
        /// <param name="id">众筹Id</param>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCrowdfunding", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        CrowdfundingDTO GetCrowdfunding(Guid id);

        /// <summary>
        /// 获取众筹列表
        /// </summary>
        /// <param name="appName">app名称</param>
        /// <param name="cfState">众筹状态</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数量</param>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCrowdfundings", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
       GetCrowdfundingsDTO GetCrowdfundings(String appName, int cfState, int pageIndex, int pageSize);
        /// <summary>
        /// 获取众筹股东列表
        /// </summary>
        /// <param name="crowdfundingId">众筹Id</param>
        /// <param name="userName">用户姓名</param>
        /// <param name="userCode">用户账号</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数量</param>
        [WebInvoke(Method = "POST", UriTemplate = "/GetUserCrowdfundings", BodyStyle = WebMessageBodyStyle.WrappedRequest,
            ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        GetUserCrowdfundingsDTO GetUserCrowdfundings(Guid crowdfundingId, String userName, string userCode, int pageIndex, int pageSize);
        /// <summary>
        /// 众筹股东订单列表
        /// </summary>
        /// <param name="crowdfundingId">众筹Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数量</param>
        [WebInvoke(Method = "POST", UriTemplate = "/GetUserCrowdfundingOrders", BodyStyle = WebMessageBodyStyle.WrappedRequest,
            ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        CommodityOrderVMDTO GetUserCrowdfundingOrders(Guid crowdfundingId, Guid userId, int pageIndex, int pageSize);



          /// <summary>
        /// 根据appId找appName
        /// </summary>
        /// <returns></returns>

        [WebInvoke(Method = "POST", UriTemplate = "/GetAppNameByAppId", BodyStyle = WebMessageBodyStyle.WrappedRequest,
           ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        AppNameDTO GetAppNameByAppId(Guid appId);

    }
}
