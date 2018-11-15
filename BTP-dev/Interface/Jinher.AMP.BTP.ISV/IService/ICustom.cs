using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.BF.IService.Interface;

namespace Jinher.AMP.BTP.ISV.IService
{
    /// <summary>
    /// 客服服务
    /// </summary>
    [ServiceContract]
    public interface ICustom : ICommand
    {
        /// <summary>
        /// 根据userid获取用户信息
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCustomInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<CBC.Deploy.CustomDTO.UserInfoWithAccountDTO> GetCustomInfo(Guid userId);

        /// <summary>
        /// 根据commodityId获取商品信息
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<CommodityInfoListDTO> GetCommodityInfo(Guid commodityId);

        /// <summary>
        /// 根据orderId获取订单信息
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityOrder", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<CommodityOrderDTO> GetCommodityOrder(Guid orderId);
        /// <summary>
        /// 商家的移动坐席数据
        /// </summary>
        /// <param name="pageSize">返回数据条数</param>
        /// <param name="pageIndex">返回数据条数</param>
        /// <param name="recordCount">返回总数据条数</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAppSceneContent", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<List<AppSceneUserDTO>> GetAppSceneContent(int pageIndex);
        /// <summary>
        ///获取易捷北京下所有店铺信息
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetYJAppInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<List<YJBJAppInfo>> GetYJAppInfo(int pageIndex);

        /// <summary>
        /// 获取消息未读数
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetNoInfoCount", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        int GetNoInfoCount(String guid);
    }
}
