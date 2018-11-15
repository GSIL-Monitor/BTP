
/***************
功能描述: BTPIService
作    者: 
创建时间: 2014/3/20 19:37:04
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
    /// 评价接口
    /// </summary>
    [ServiceContract]
    public interface IReview : ICommand
    {
        /// <summary>
        /// 添加评价
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.ReviewSV.svc/SaveReview
        /// </para>
        /// </summary>
        /// <param name="reviewSDTO">评价实体</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveReview", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SaveReview(ReviewSDTO reviewSDTO, Guid appId);


        /// <summary>
        /// 回复评价
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.ReviewSV.svc/ReplyReview
        /// </para>
        /// </summary>
        /// <param name="replySDTO">评价实体</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/ReplyReview", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO ReplyReview(ReplySDTO replySDTO, Guid appId);

        /// <summary>
        /// 根据商品ID查询商品评价
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.ReviewSV.svc/GetReviewByCommodityId
        /// </para>
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <param name="appId">appId</param>
        /// <param name="lastReviewTime">本页最后评价时间</param>       
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetReviewByCommodityId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<ReviewSDTO> GetReviewByCommodityId(Guid commodityId, Guid appId, DateTime lastReviewTime);

        /// <summary>
        /// 根据用户ID查询商品评价
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.ReviewSV.svc/GetReviewByUserId
        /// </para>
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="appId">appId</param>
        /// <param name="lastReviewTime">本页最后评价时间</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetReviewByUserId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<ReviewSDTO> GetReviewByUserId(Guid userId, Guid appId, DateTime lastReviewTime);

        /// <summary>
        /// 商品评价数量
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.ReviewSV.svc/GetReviewNum
        /// </para>
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetReviewNum", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        NumResultSDTO GetReviewNum(Guid commodityId, Guid appId);

        /// <summary>
        /// 修改评价
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.ReviewSV.svc/UpdateReview
        /// </para>
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="reviewId">评价ID</param>
        /// <param name="content">评价内容</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateReview", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateReview(Guid userId, Guid reviewId, string content);

        /// <summary>
        /// 根据评价得到回复列表
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.ReviewSV.svc/GetReplyByReviewId
        /// </para>
        /// </summary>
        /// <param name="ReviewId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetReplyByReviewId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<ReplySDTO> GetReplyByReviewId(Guid reviewId, Guid userId);
        /// <summary>
        /// 删除评价
        /// </summary>
        /// <param name="reviewId">评价ID</param>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DeleteReview", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DeleteReview(Guid reviewId, Guid userId);

        /// <summary>
        /// 获取App相关评价
        /// </summary>
        /// <param name="appId">应用Id</param>
        /// <param name="pageIndex">页号</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="total">总数</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAppCommentDTOList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.Apm.Deploy.CustomDTO.AppPackageCommentTotalDTO GetAppCommentDTOList(Guid appId, int pageIndex, int pageSize);

        /// <summary>
        /// 根据商品ID查询商品评价
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetReviewOnlyByCommodityId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ReviewOnlyDTO> GetReviewOnlyByCommodityId(
            ReviewSearchDTO search);


        /// <summary>
        /// 评价成功后的异常通知。
        /// <para>Service Url: http://devbtp.iuoooo.com/Jinher.AMP.BTP.SV.ReviewSV.svc/ReviewNofity
        /// </para>
        /// </summary>
        /// <param name="reviewSDTO">评价实体</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/ReviewNofity", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO ReviewNofity(ReviewNofityDTO rnDto);

        /// <summary>
        /// 评价成功后的通知更新商品评价数
        /// <para>Service Url: http://devbtp.iuoooo.com/Jinher.AMP.BTP.SV.ReviewSV.svc/ReviewNofityComOnly
        /// </para>
        /// </summary>
        /// <param name="rnDto"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/ReviewNofityComOnly", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO ReviewNofityComOnly(ReviewNofityDTO rnDto);
    }
}
