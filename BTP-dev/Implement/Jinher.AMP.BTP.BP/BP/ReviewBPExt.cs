
/***************
功能描述: BTPBP
作    者: 
创建时间: 2014/4/7 12:16:02
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.BE.BELogic;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class ReviewBP : BaseBP, IReview
    {

        /// <summary>
        /// 获得商家所有评价
        /// </summary>
        /// <param name="appId">AppId</param>
        /// <param name="pageSize">每页显示数</param>
        /// <param name="pageIndex">当前页数</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ReviewVM> GetReviewListExt(System.Guid appId, int pageSize, int pageIndex, string startTime, string endTime, string commodityName, string content, out int rowCount)
        {
            Reply reply = new Reply();
            IQueryable<ReviewVM> query = from data in Review.ObjectSet().Where(n => n.AppId == appId)
                                         join data1 in Commodity.ObjectSet().Where(n => n.AppId == appId && n.IsDel == false) on data.CommodityId equals data1.Id
                                         select new ReviewVM
                                         {
                                             ReviewId = data.Id,
                                             AppId = data.AppId,
                                             ReviewUserName = data.UserName == null ? "" : data.UserName,
                                             Details = data.Content,
                                             CommodityId = data1.Id,
                                             CommodityName = data1.Name,
                                             CommodityPicture = data1.PicturesPath,
                                             SubTime = data.SubTime,
                                             UserId = data.UserId,
                                             ReviewNum = data1.TotalReview
                                         };

            if (!string.IsNullOrEmpty(startTime))
            {
                DateTime st = DateTime.Parse(startTime);
                query = query.Where(n => n.SubTime > st);
            }
            if (!string.IsNullOrEmpty(endTime))
            {
                DateTime end = DateTime.Parse(endTime);
                query = query.Where(n => n.SubTime < end);
            }
            if (!string.IsNullOrEmpty(commodityName))
            {
                query = query.Where(n => n.CommodityName.Contains(commodityName));
            }
            if (!string.IsNullOrEmpty(content))
            {
                //获取回复中含有关键字的评价id
                var matchedReply = from rep in Reply.ObjectSet()
                                   join rev in Review.ObjectSet() on rep.ReviewId equals rev.Id
                                   where rep.ReplyDetails.Contains(content)
                                   orderby rev.SubTime descending
                                   select rep.ReviewId;

                //查询评价或回复中有关键字的评价
                query = query.Where(n => n.Details.Contains(content) || matchedReply.Contains(n.ReviewId));
            }
            rowCount = query.Count();
            var list = query.OrderByDescending(n => n.SubTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            //获取所有评价id
            List<Guid> reviewIds = query.Select(n => n.ReviewId).ToList();

            var replyQuery = Reply.ObjectSet().Where(n => reviewIds.Contains(n.ReviewId)).OrderByDescending(n => n.SubTime);
            List<Reply> replylist = replyQuery.ToList();
            foreach (var item in list)
            {
                item.ReplyList = (from n in replylist.Where(n => n.ReviewId == item.ReviewId)
                                  select new ReplyVM
                                  {
                                      ReplyerDetails = n.ReplyDetails,
                                      ReplyerSubTime = n.SubTime,
                                      ReplyerUserName = n.UserName,
                                      ReviewContent = item.Details,
                                      ReviewId = item.ReviewId,
                                      ReviewTime = item.SubTime,
                                      ReviewUserName = item.ReviewUserName
                                  }).OrderBy(a => a.ReplyerSubTime).ToList();
                //item.IsReply = replylist.Where(n => n.ReviewId == item.ReviewId).OrderByDescending(n => n.SubTime).Select(n => n.PreUserId).FirstOrDefault() == this.ContextDTO.LoginUserID ? false : true;
                //根据条数进行判断商家是否可以回复
                int cou = replylist.Where(n => n.ReviewId == item.ReviewId).Count();
                if (cou == 0)
                { //表示商家没有回复过
                    item.IsReply = false;
                }
                else
                {
                    item.IsReply = cou % 2 == 0 ? false : true;
                }

            }
            return list;
        }
        /// <summary>
        /// 获得商家下某个商品所有评价
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <param name="pageSize">每页显示数</param>
        /// <param name="pageIndex">当前页数</param>
        /// <returns></returns>
        public CommodityReplyVM GetReplyByCommodityIdExt(System.Guid commodityId, int pageSize, int pageIndex, out int rowCount)
        {
            Reply reply = new Reply();
            var commodity = Commodity.ObjectSet().Where(n => n.Id == commodityId).FirstOrDefault();
            var query1 = new CommodityReplyVM
                         {
                             CommodityPicture = commodity.PicturesPath,
                             CommodityName = commodity.Name,
                             CommodityId = commodity.Id,
                             //ReplyList = reply.GetReplyListByCommodityId(commodityId),
                         };
            //query1.ReviewList = reply.GetReplyListByCommodityId(commodityId, pageIndex, pageSize);

            IEnumerable<ReviewVM> query = from data in Review.ObjectSet()
                                          where data.CommodityId == commodityId
                                          select new ReviewVM
                                          {
                                              SubTime = data.SubTime,
                                              Details = data.Content,
                                              ReviewId = data.Id,
                                              UserId = data.UserId,
                                              AppId = data.AppId,
                                              ReviewUserName = data.UserName == null ? "" : data.UserName
                                          };
            query = query.OrderByDescending(n => n.SubTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            List<Guid> reviewIds = query.Select(n => n.ReviewId).ToList();
            List<Reply> replylist = Reply.ObjectSet().Where(n => reviewIds.Contains(n.ReviewId)).OrderByDescending(n => n.SubTime).ToList();
            foreach (var item in query)
            {
                item.ReplyList = (from n in replylist.Where(n => n.ReviewId == item.ReviewId)
                                  select new ReplyVM
                                  {
                                      ReplyerDetails = n.ReplyDetails,
                                      ReplyerSubTime = n.SubTime,
                                      ReplyerUserName = n.UserName,
                                      ReviewContent = item.Details,
                                      ReviewId = item.ReviewId,
                                      ReviewTime = item.SubTime,
                                      ReviewUserName = item.ReviewUserName
                                  }).OrderBy(a => a.ReplyerSubTime).ToList().ToList();
                int cou = replylist.Where(n => n.ReviewId == item.ReviewId).Count();
                if (cou == 0)
                { //表示商家没有回复过
                    item.IsReply = false;
                }
                else
                {
                    item.IsReply = cou % 2 == 0 ? false : true;
                }
            }
            query1.ReviewList = query.ToList();

            rowCount = query1.ReviewList.Count();
            return query1;
        }
        /// <summary>
        /// 回复评价
        /// </summary>
        /// <param name="userId">被回复人ID</param>
        /// <param name="reviewId">评价ID</param>
        /// <param name="commodityId">商品ID</param>
        /// <param name="replyerDetails">回复内容</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RespondCommentExt(System.Guid userId, System.Guid reviewId, System.Guid appId, string replyerDetails)
        {
            try
            {
                int aa = Review.ObjectSet().Where(n => n.Id == reviewId).Count();
                //判断评价是否存在
                if (aa == 0)
                {
                    return new ResultDTO { ResultCode = 1, Message = "评价ID错误" };
                }

                var esAppId = (from r in Review.ObjectSet()
                               join m in OrderItem.ObjectSet()
                                    on r.OrderItemId equals m.Id
                               join o in CommodityOrder.ObjectSet()
                                    on m.CommodityOrderId equals o.Id
                               where r.Id == reviewId
                               select o.EsAppId).FirstOrDefault();

                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                Reply reply = new Reply();
                reply.Id = Guid.NewGuid();
                reply.Name = "回复";
                reply.SubTime = DateTime.Now;
                reply.PreUserId = userId;
                reply.ReviewId = reviewId;
                reply.UserName = "商家";
                reply.ReplyDetails = replyerDetails;
                reply.ReplyerId = appId;
                reply.Type = 0;
                reply.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(reply);
                int result = contextSession.SaveChanges();
                if (result > 0)
                {
                    AddMessage addmassage = new AddMessage();
                    string type = "review";

                    //发送消息，异步执行
                    System.Threading.ThreadPool.QueueUserWorkItem(
                        a =>
                        {
                            var EsAppId = esAppId.HasValue ? esAppId.Value : appId;
                            addmassage.AddMessages(reply.Id.ToString(), userId.ToString(), EsAppId, "", 0, "", type);
                            ////正品会发送消息
                            //if (new ZPHSV().CheckIsAppInZPH(appId))
                            //{
                            //    addmassage.AddMessages(reply.Id.ToString(), userId.ToString(), CustomConfig.ZPHAppId, "", 0, "", type);
                            //}
                        });

                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("回复评价服务异常。userId：{0}，reviewId：{1}，appId：{2}，replyerDetails：{3}", userId, reviewId, appId, replyerDetails), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        /// <summary>
        /// 获得评价详细信息
        /// </summary>
        /// <param name="reviewId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.ReviewDTO GetReviewByIdExt(System.Guid reviewId)
        {
            Review review = Review.ObjectSet().Where(n => n.Id == reviewId).FirstOrDefault();
            return review.ToEntityData();
        }


        /// <summary>
        /// 获得最后回复人ID
        /// </summary>
        /// <param name="reviewId"></param>
        /// <returns></returns>
        public System.Guid SelectReplyUserIdExt(System.Guid reviewId)
        {
            Guid replyerId = Reply.ObjectSet().Where(n => n.Id == reviewId).OrderByDescending(n => n.SubTime).Select(n => n.ReplyerId).FirstOrDefault();
            return replyerId;
        }


    }
}
