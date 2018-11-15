
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2014/4/9 18:40:06
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Agent.Base;

namespace Jinher.AMP.BTP.IBP.Agent
{

    public class ReviewAgent : BaseBpAgent<IReview>, IReview
    {
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ReviewVM> GetReviewList(System.Guid appId, int pageSize, int pageIndex, string startTime, string endTime, string commodityName, string content,out int rowCount)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ReviewVM> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetReviewList(appId, pageSize, pageIndex,startTime,endTime,commodityName,content,out rowCount);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommodityReplyVM GetReplyByCommodityId(System.Guid commodityId, int pageSize, int pageIndex, out int rowCount)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.CommodityReplyVM result;

            try
            {
                //调用代理方法
                result = base.Channel.GetReplyByCommodityId(commodityId, pageSize, pageIndex,out rowCount);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RespondComment(System.Guid userId, System.Guid reviewId, System.Guid appId, string replyerDetails)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.RespondComment(userId, reviewId, appId, replyerDetails);

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
        public Jinher.AMP.BTP.Deploy.ReviewDTO GetReviewById(System.Guid reviewId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.ReviewDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetReviewById(reviewId);

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
        public System.Guid SelectReplyUserId(System.Guid reviewId)
        {
            //定义返回值
            System.Guid result;

            try
            {
                //调用代理方法
                result = base.Channel.SelectReplyUserId(reviewId);

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
