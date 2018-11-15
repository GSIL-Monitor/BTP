
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2014/4/12 15:03:46
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.BP.Agent.Base;

namespace Jinher.AMP.BTP.ISV.Agent
{

    public class ReviewAgent : BaseBpAgent<IReview>, IReview
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveReview(Jinher.AMP.BTP.Deploy.CustomDTO.ReviewSDTO reviewSDTO, System.Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SaveReview(reviewSDTO, appId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO ReplyReview(Jinher.AMP.BTP.Deploy.CustomDTO.ReplySDTO replySDTO, System.Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.ReplyReview(replySDTO, appId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ReviewSDTO> GetReviewByCommodityId(System.Guid commodityId, System.Guid appId, System.DateTime lastReviewTime)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ReviewSDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetReviewByCommodityId(commodityId, appId, lastReviewTime);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ReviewSDTO> GetReviewByUserId(System.Guid userId, System.Guid appId, System.DateTime lastReviewTime)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ReviewSDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetReviewByUserId(userId, appId, lastReviewTime);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.NumResultSDTO GetReviewNum(System.Guid commodityId, System.Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.NumResultSDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetReviewNum(commodityId, appId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateReview(System.Guid userId, System.Guid reviewId, string content)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateReview(userId, reviewId, content);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ReplySDTO> GetReplyByReviewId(System.Guid reviewId, System.Guid userId)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ReplySDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetReplyByReviewId(reviewId, userId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteReview(System.Guid reviewId, System.Guid userId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DeleteReview(reviewId, userId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        public Jinher.AMP.Apm.Deploy.CustomDTO.AppPackageCommentTotalDTO GetAppCommentDTOList(Guid appId, int pageIndex, int pageSize)
        {
            //定义返回值
            Jinher.AMP.Apm.Deploy.CustomDTO.AppPackageCommentTotalDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAppCommentDTOList(appId, pageIndex, pageSize);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        public List<ReviewOnlyDTO> GetReviewOnlyByCommodityId(ReviewSearchDTO search)
        {
            //定义返回值
            List<ReviewOnlyDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetReviewOnlyByCommodityId(search);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
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
            //定义返回值
            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.ReviewNofity(rnDto);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
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
            //定义返回值
            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.ReviewNofityComOnly(rnDto);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
    }
}
