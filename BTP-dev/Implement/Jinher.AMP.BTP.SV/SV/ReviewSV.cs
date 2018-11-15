
/***************
功能描述: BTPSV
作    者: 
创建时间: 2014/4/12 15:03:45
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class ReviewSV : BaseSv, IReview
    {

        /// <summary>
        /// 添加评价
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.ReviewSV.svc/SaveReview
        /// </para>
        /// </summary>
        /// <param name="reviewSDTO">评价实体</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveReview(Jinher.AMP.BTP.Deploy.CustomDTO.ReviewSDTO reviewSDTO, System.Guid appId)
        {
            base.Do();
            return this.SaveReviewExt(reviewSDTO, appId);

        }
        /// <summary>
        /// 回复评价
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.ReviewSV.svc/ReplyReview
        /// </para>
        /// </summary>
        /// <param name="replySDTO">评价实体</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO ReplyReview(Jinher.AMP.BTP.Deploy.CustomDTO.ReplySDTO replySDTO, System.Guid appId)
        {
            base.Do();
            return this.ReplyReviewExt(replySDTO, appId);

        }
        /// <summary>
        /// 根据商品ID查询商品评价
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.ReviewSV.svc/GetReviewByCommodityId
        /// </para>
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <param name="appId">appId</param>
        /// <param name="lastReviewTime">本页最后评价时间</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ReviewSDTO> GetReviewByCommodityId(System.Guid commodityId, System.Guid appId, System.DateTime lastReviewTime)
        {
            base.Do(false);
            return this.GetReviewByCommodityIdExt(commodityId, appId, lastReviewTime);

        }
        /// <summary>
        /// 根据用户ID查询商品评价
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.ReviewSV.svc/GetReviewByUserId
        /// </para>
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="appId">appId</param>
        /// <param name="lastReviewTime">本页最后评价时间</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ReviewSDTO> GetReviewByUserId(System.Guid userId, System.Guid appId, System.DateTime lastReviewTime)
        {
            base.Do();
            return this.GetReviewByUserIdExt(userId, appId, lastReviewTime);

        }
        /// <summary>
        /// 商品评价数量
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.ReviewSV.svc/GetReviewNum
        /// </para>
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.NumResultSDTO GetReviewNum(System.Guid commodityId, System.Guid appId)
        {
            base.Do();
            return this.GetReviewNumExt(commodityId, appId);

        }
        /// <summary>
        /// 修改评价
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.ReviewSV.svc/UpdateReview
        /// </para>
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="reviewId">评价ID</param>
        /// <param name="content">评价内容</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateReview(System.Guid userId, System.Guid reviewId, string content)
        {
            base.Do();
            return this.UpdateReviewExt(userId, reviewId, content);

        }
        /// <summary>
        /// 根据评价得到回复列表
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.ReviewSV.svc/GetReplyByReviewId
        /// </para>
        /// </summary>
        /// <param name="ReviewId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ReplySDTO> GetReplyByReviewId(System.Guid reviewId, System.Guid userId)
        {
            base.Do();
            return this.GetReplyByReviewIdExt(reviewId, userId);

        }
        /// <summary>
        /// 删除评价
        /// </summary>
        /// <param name="reviewId">评价ID</param>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteReview(System.Guid reviewId, System.Guid userId)
        {
            base.Do();
            return this.DeleteReviewExt(reviewId, userId);

        }

        public Jinher.AMP.Apm.Deploy.CustomDTO.AppPackageCommentTotalDTO GetAppCommentDTOList(Guid appId, int pageIndex, int pageSize)
        {
            base.Do(false);
            return this.GetAppCommentDTOListExt(appId, pageIndex, pageSize);
        }

        /// <summary>
        /// 根据商品ID查询商品评价
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ReviewOnlyDTO> GetReviewOnlyByCommodityId(ReviewSearchDTO search)
        {
            base.Do(false);
            return this.GetReviewOnlyByCommodityIdExt(search);
        }


        /// <summary>
        /// 评价成功后的异常通知。
        /// <para>Service Url: http://devbtp.iuoooo.com/Jinher.AMP.BTP.SV.ReviewSV.svc/ReviewNofity
        /// </para>
        /// </summary>
        /// <param name="reviewSDTO">评价实体</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public ResultDTO ReviewNofity(ReviewNofityDTO rnDto)
        {
            base.Do(false);
            return this.ReviewNofityExt(rnDto);
        }
        /// <summary>
        /// 评价成功后的通知更新商品评价数
        /// <para>Service Url: http://devbtp.iuoooo.com/Jinher.AMP.BTP.SV.ReviewSV.svc/ReviewNofityComOnly
        /// </para>
        /// </summary>
        /// <param name="rnDto"></param>
        /// <returns></returns>
        public ResultDTO ReviewNofityComOnly(ReviewNofityDTO rnDto)
        {
            base.Do(false);
            return this.ReviewNofityComOnlyExt(rnDto);
        }
    }
}