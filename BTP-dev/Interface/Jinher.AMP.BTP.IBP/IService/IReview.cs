
/***************
功能描述: BTPIService
作    者: 
创建时间: 2014/3/26 14:08:05
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

    [ServiceContract]
    public interface IReview : ICommand
    {

        /// <summary>
        /// 获得商家所有评价
        /// </summary>
        /// <param name="appId">AppId</param>
        /// <param name="pageSize">每页显示数</param>
        /// <param name="pageIndex">当前页数</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetReviewList", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<ReviewVM> GetReviewList(Guid appId, int pageSize, int pageIndex, string startTime, string endTime, string commodityName, string content,out int rowCount);


        /// <summary>
        /// 获得商家下某个商品所有评价
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <param name="pageSize">每页显示数</param>
        /// <param name="pageIndex">当前页数</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetReplyListByCommodityId", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        CommodityReplyVM GetReplyByCommodityId(Guid commodityId, int pageSize, int pageIndex,out int rowCount);


        /// <summary>
        /// 回复评价
        /// </summary>
        /// <param name="userId">被回复人ID</param>
        /// <param name="reviewId">评价ID</param>
        /// <param name="commodityId">商品ID</param>
        /// <param name="replyerDetails">回复内容</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/RespondComment", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO RespondComment(Guid userId, Guid reviewId, Guid appId, string replyerDetails);

        /// <summary>
        /// 获得评价详细信息
        /// </summary>
        /// <param name="reviewId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetReviewById", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ReviewDTO GetReviewById(Guid reviewId);

        /// <summary>
        /// 获得最后回复人ID
        /// </summary>
        /// <param name="reviewId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SelectReplyUserId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Guid SelectReplyUserId(Guid reviewId);
    }
}
