/***************
功能描述: BTPIService
作    者:  LSH
创建时间: 2017/9/21 10:52:36
***************/

using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.BF.IService.Interface;

namespace Jinher.AMP.BTP.IBP.IService
{
    [ServiceContract]
    public interface IPresentPromotion : ICommand
    {
        /// <summary>
        /// 查询活动
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/GetPromotions", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<ListResult<PresentPromotionSearchResultDTO>> GetPromotions(PresentPromotionSearchDTO input);

        /// <summary>
        /// 结束活动
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/EndPromotion", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO EndPromotion(Guid id);

        /// <summary>
        /// 删除活动
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/DeletePromotion", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DeletePromotion(Guid id);

        /// <summary>
        /// 获取活动详细信息
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/GetPromotionDetails", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<PresentPromotionCreateDTO> GetPromotionDetails(Guid id);

        /// <summary>
        /// 发布活动
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/CreatePromotion", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO CreatePromotion(PresentPromotionCreateDTO input);

        /// <summary>
        /// 更新活动
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdatePromotion", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdatePromotion(PresentPromotionCreateDTO input);

        /// <summary>
        /// 查询商品
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodities", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<ListResult<PresentPromotionCommoditySearchResultDTO>> GetCommodities(PresentPromotionCommoditySearchDTO input);
    }
}
