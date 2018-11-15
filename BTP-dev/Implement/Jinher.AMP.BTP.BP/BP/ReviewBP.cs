
/***************
功能描述: BTPBP
作    者: 
创建时间: 2014/4/9 18:40:05
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class ReviewBP : BaseBP, IReview
    {

        /// <summary>
        /// 获得商家所有评价
        /// </summary>
        /// <param name="appId">AppId</param>
        /// <param name="pageSize">每页显示数</param>
        /// <param name="pageIndex">当前页数</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ReviewVM> GetReviewList(System.Guid appId, int pageSize, int pageIndex, string startTime, string endTime, string commodityName, string content,out int rowCount)
        {
            base.Do();
            return this.GetReviewListExt(appId, pageSize, pageIndex,startTime,endTime,commodityName,content,out rowCount);
        }
        /// <summary>
        /// 获得商家下某个商品所有评价
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <param name="pageSize">每页显示数</param>
        /// <param name="pageIndex">当前页数</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommodityReplyVM GetReplyByCommodityId(System.Guid commodityId, int pageSize, int pageIndex, out int rowCount)
        {
            base.Do();
            return this.GetReplyByCommodityIdExt(commodityId, pageSize, pageIndex,out rowCount);
        }
        /// <summary>
        /// 回复评价
        /// </summary>
        /// <param name="userId">被回复人ID</param>
        /// <param name="reviewId">评价ID</param>
        /// <param name="commodityId">商品ID</param>
        /// <param name="replyerDetails">回复内容</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RespondComment(System.Guid userId, System.Guid reviewId, System.Guid appId, string replyerDetails)
        {
            base.Do();
            return this.RespondCommentExt(userId, reviewId, appId, replyerDetails);
        }
        /// <summary>
        /// 获得评价详细信息
        /// </summary>
        /// <param name="reviewId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.ReviewDTO GetReviewById(System.Guid reviewId)
        {
            base.Do();
            return this.GetReviewByIdExt(reviewId);
        }
        /// <summary>
        /// 获得最后回复人ID
        /// </summary>
        /// <param name="reviewId"></param>
        /// <returns></returns>
        public System.Guid SelectReplyUserId(System.Guid reviewId)
        {
            base.Do();
            return this.SelectReplyUserIdExt(reviewId);
        }
    }
}