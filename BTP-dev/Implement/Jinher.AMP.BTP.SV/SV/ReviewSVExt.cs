using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using Jinher.AMP.BTP.BE;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.TPS;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 评价接口类
    /// </summary>
    public partial class ReviewSV : BaseSv, IReview
    {

        /// <summary>
        /// 添加评价
        /// </summary>
        /// <param name="reviewSDTO">评价实体</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveReviewExt
            (Jinher.AMP.BTP.Deploy.CustomDTO.ReviewSDTO reviewSDTO, System.Guid appId)
        {
            try
            {
                if (reviewSDTO == null || reviewSDTO.UserId == Guid.Empty || reviewSDTO.OrderItemId == Guid.Empty)
                    return new ResultDTO { ResultCode = 1, Message = "参数错误！" };

                if (string.IsNullOrWhiteSpace(reviewSDTO.Details))
                {
                    return new ResultDTO { ResultCode = 1, Message = "参数错误，评价内容不能为空！" };
                }
                if (Review.ObjectSet().Any(c => c.UserId == reviewSDTO.UserId && c.OrderItemId == reviewSDTO.OrderItemId))
                {
                    return new ResultDTO { ResultCode = 1, Message = "已评价！" };
                }

                var orderitem = OrderItem.ObjectSet().FirstOrDefault(n => n.Id == reviewSDTO.OrderItemId);
                if (orderitem != null)
                {
                    ContextSession contextSession = ContextFactory.CurrentThreadContext;

                    Review review = new Review();
                    review.Id = Guid.NewGuid();
                    review.EntityState = System.Data.EntityState.Added;
                    review.Name = "评价";
                    review.UserId = reviewSDTO.UserId;
                    review.UserName = reviewSDTO.Name ?? string.Empty;
                    review.UserHeader = reviewSDTO.UserHead;
                    review.Content = reviewSDTO.Details;
                    review.CommodityId = orderitem.CommodityId;
                    review.AppId = appId;
                    review.OrderItemId = orderitem.Id;
                    review.CommodityName = orderitem.Name;
                    review.CommodityPicture = orderitem.PicturesPath;
                    review.CommodityAttributes = orderitem.CommodityAttributes;
                    contextSession.SaveObject(review);

                    orderitem.AlreadyReview = true;
                    contextSession.SaveObject(orderitem);

                    //增加商品评价数
                    Commodity com = Commodity.ObjectSet().FirstOrDefault(n => n.Id == orderitem.CommodityId);
                    if (com != null)
                    {
                        com.TotalReview += 1;
                        contextSession.SaveObject(com);
                    }

                    contextSession.SaveChanges();

                    if (com != null)
                        com.RefreshCache(EntityState.Modified);
                    var esAppId = CommodityOrder.ObjectSet().Where(t => t.Id == orderitem.CommodityOrderId).Select(t => t.EsAppId).FirstOrDefault();
                    if (esAppId.HasValue)
                    {
                        bool giveResult = SignSV.Instance.GiveScoreBtpComment(review.UserId, esAppId.Value, reviewSDTO.SourceUrl, orderitem.CommodityOrderId, orderitem.Code, review.Id);
                        if (!giveResult)
                        {
                            return new ResultDTO { ResultCode = 1, Message = "评价加积分错误" };
                        }
                    }
                }
                else
                {
                    return new ResultDTO { ResultCode = 1, Message = "Error" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("添加评价服务异常。reviewSDTO：{0}，appId：{1}", JsonHelper.JsonSerializer(reviewSDTO), appId), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        /// <summary>
        /// 回复评价
        /// </summary>
        /// <param name="replySDTO">评价实体</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO ReplyReviewExt(Jinher.AMP.BTP.Deploy.CustomDTO.ReplySDTO replySDTO, System.Guid appId)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                int aa = Review.ObjectSet().Where(n => n.Id == replySDTO.ReviewId).Count();
                //判断评价是否存在
                if (aa == 0)
                {
                    return new ResultDTO { ResultCode = 1, Message = "评价ID错误" };
                }
                //判断商家是否回复
                int count = Reply.ObjectSet().Where(n => n.ReviewId == replySDTO.ReviewId).Count();
                if (count % 2 == 0)
                {
                    return new ResultDTO { ResultCode = 1, Message = "请等待商家回复后再回复" };
                }
                Reply reply = new Reply();
                reply.Id = Guid.NewGuid();
                reply.Name = "回复";
                reply.ReplyerId = replySDTO.ReplyerId;
                reply.UserName = replySDTO.ReplyerName;
                reply.UserHeader = replySDTO.ReplyerHead;
                reply.ReplyDetails = replySDTO.Details;
                reply.ReviewId = replySDTO.ReviewId;
                reply.PreUserId = replySDTO.PreId;
                reply.SubTime = DateTime.Now;
                reply.SubId = replySDTO.ReplyerId;
                reply.Type = 1;
                reply.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(reply);
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("回复服务异常。replySDTO：{0}，appId：{1}，", JsonHelper.JsonSerializer(replySDTO), appId), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        /// <summary>
        /// 根据商品ID查询商品评价
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <param name="appId">appId</param>
        /// <param name="lastReviewTime">本页最后评价时间</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ReviewSDTO> GetReviewByCommodityIdExt
            (System.Guid commodityId, System.Guid appId, System.DateTime lastReviewTime)
        {
            Reply r = new Reply();
            string min = "1970-01-01 08:00:00";
            DateTime mintime = Convert.ToDateTime(min);
            if (lastReviewTime.Equals(null) || lastReviewTime.Equals(DateTime.MinValue) || lastReviewTime.Equals(mintime))
            {
                lastReviewTime = DateTime.Now;
            }

            var query = (from data in Review.ObjectSet().
                             Where(n => n.CommodityId == commodityId && n.SubTime < lastReviewTime)
                             .OrderByDescending(n => n.SubTime).Take(10)
                         select data).ToList();

            var reviewid = query.Select(n => n.Id).ToList();

            var replaylist = Reply.ObjectSet().Where(n => reviewid.Contains(n.ReviewId)).ToList();

            List<ReviewSDTO> reviewlist = new List<ReviewSDTO>();
            foreach (var data in query)
            {
                ReviewSDTO reviewSDTO = new ReviewSDTO();
                reviewSDTO.ReviewId = data.Id;
                reviewSDTO.UserId = data.UserId;
                reviewSDTO.OrderItemId = data.CommodityId;
                reviewSDTO.Name = ConvertAnonymous(data.UserName);
                reviewSDTO.UserHead = data.UserHeader;
                reviewSDTO.Details = data.Content;
                reviewSDTO.SubTime = data.SubTime;
                reviewSDTO.ShowTime = ConvertPublishTime(data.SubTime);
                reviewSDTO.Size = data.CommodityAttributes;
                reviewSDTO.Replays = (from n in replaylist.Where(n => n.ReviewId == data.Id)
                                      select new ReplySDTO
                                      {
                                          ReplyerName = ConvertAnonymous(n.UserName),
                                          ReplyerHead = n.UserHeader,
                                          Details = n.ReplyDetails,
                                          ReviewId = n.Id,
                                          PreId = n.PreUserId,
                                          SubTime = n.SubTime,
                                          ShowTime = ConvertPublishTime(n.SubTime)
                                      }).OrderBy(n => n.SubTime).ToList();

                reviewlist.Add(reviewSDTO);
            }
            return reviewlist;
        }

        /// <summary>
        /// 根据用户ID查询商品评价
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="appId">appId</param>
        /// <param name="lastReviewTime">本页最后评价时间</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ReviewSDTO> GetReviewByUserIdExt
            (System.Guid userId, System.Guid appId, System.DateTime lastReviewTime)
        {

            string min = "1970-01-01 08:00:00";
            DateTime mintime = Convert.ToDateTime(min);
            if (lastReviewTime.Equals(null) || lastReviewTime.Equals(DateTime.MinValue) || lastReviewTime.Equals(mintime))
            {
                lastReviewTime = DateTime.Now;
            }

            var query = (from r in Review.ObjectSet()
                         where r.UserId == userId && r.AppId == appId && r.SubTime < lastReviewTime
                         orderby r.SubTime descending
                         select r).Take(10).ToList();

            //获取所有评价id
            var reviewid = query.Select(n => n.Id).ToList();

            var replaylist = Reply.ObjectSet().Where(n => reviewid.Contains(n.ReviewId)).ToList();

            List<ReviewSDTO> reviewlist = new List<ReviewSDTO>();
            foreach (var review in query)
            {
                ReviewSDTO reviewSDTO = new ReviewSDTO();
                reviewSDTO.ReviewId = review.Id;
                reviewSDTO.UserId = review.UserId;
                reviewSDTO.Name = review.UserName;
                reviewSDTO.UserHead = review.UserHeader;
                reviewSDTO.Details = review.Content;
                reviewSDTO.SubTime = review.SubTime;
                reviewSDTO.ShowTime = ConvertPublishTime(review.SubTime);
                reviewSDTO.CommodityName = review.CommodityName;
                reviewSDTO.CommodityPicture = review.CommodityPicture;
                reviewSDTO.Size = review.CommodityAttributes;
                reviewSDTO.Replays = (from n in replaylist.Where(n => n.ReviewId == review.Id)
                                      select new ReplySDTO
                                      {
                                          ReplyerName = n.UserName,
                                          ReplyerHead = n.UserHeader,
                                          Details = n.ReplyDetails,
                                          ReviewId = n.Id,
                                          PreId = n.PreUserId,
                                          SubTime = n.SubTime,
                                          ShowTime = ConvertPublishTime(n.SubTime)
                                      }).OrderBy(n => n.SubTime).ToList();

                reviewlist.Add(reviewSDTO);
            }


            return reviewlist;
        }

        /// <summary>
        /// 商品评价数量
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.NumResultSDTO GetReviewNumExt(System.Guid commodityId, System.Guid appId)
        {
            NumResultSDTO num = new NumResultSDTO();
            num.TotalReview = Commodity.ObjectSet().Where(n => n.Id == commodityId).Select(n => n.TotalReview).FirstOrDefault();
            return num;
        }

        /// <summary>
        /// 修改评价
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="reviewId">评价ID</param>
        /// <param name="content">评价内容</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateReviewExt(System.Guid userId, System.Guid reviewId, string content)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                Review review = Review.ObjectSet().Where(n => n.Id == reviewId && n.UserId == userId).FirstOrDefault();
                review.Content = content;

                contextSession.SaveObject(review);
                contextSession.SaveChanges();

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("修改评价服务异常。userId：{0}，reviewId：{1}，content：{2}，", userId, reviewId, content), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        /// <summary>
        /// 根据评价得到回复列表
        /// </summary>
        /// <param name="reviewId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ReplySDTO> GetReplyByReviewIdExt
            (System.Guid reviewId, System.Guid userId)
        {
            var query = from data in Reply.ObjectSet()
                        where data.ReviewId == reviewId
                        orderby data.SubTime
                        select new ReplySDTO
                        {
                            Details = data.ReplyDetails,
                            PreId = data.PreUserId,
                            ReplyerId = data.ReplyerId,
                            SubTime = data.SubTime,
                            ReviewId = data.ReviewId
                        };
            return query.ToList();
        }

        /// <summary>
        /// 删除评价
        /// </summary>
        /// <param name="reviewId">评价ID</param>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteReviewExt(System.Guid reviewId, System.Guid userId)
        {
            try
            {

                List<Reply> replaylist = Reply.ObjectSet().Where(n => n.ReviewId == reviewId).ToList();

                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                foreach (var item in replaylist)
                {
                    item.EntityState = System.Data.EntityState.Deleted;
                    contextSession.Delete(item);
                }

                Review review = Review.ObjectSet().Where(n => n.Id == reviewId && n.UserId == userId).FirstOrDefault();
                review.EntityState = System.Data.EntityState.Deleted;

                //更新商品评价数
                Commodity commodity = Commodity.ObjectSet().Where(a => a.Id == review.CommodityId).FirstOrDefault();
                if (commodity != null)
                {
                    commodity.TotalReview -= 1;
                }

                //删除回复
                //Reply.ObjectSet().Context.ExecuteStoreCommand("delete from Reply where ReviewId={0}", reviewId);

                //删除评价
                contextSession.Delete(review);

                contextSession.SaveChange();
                if (commodity != null)
                    commodity.RefreshCache(EntityState.Modified);

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("修改评价服务异常。reviewId：{0}，userId：{1}，", reviewId, userId), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        /// <summary>
        /// 获取App相关评价
        /// </summary>
        /// <param name="appId">应用Id</param>
        /// <param name="pageIndex">页号</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="total">总数</param>
        /// <returns></returns>
        public Jinher.AMP.Apm.Deploy.CustomDTO.AppPackageCommentTotalDTO GetAppCommentDTOListExt(Guid appId, int pageIndex, int pageSize)
        {
            Jinher.AMP.Apm.Deploy.CustomDTO.AppPackageCommentTotalDTO result = new Apm.Deploy.CustomDTO.AppPackageCommentTotalDTO();

            if (appId == Guid.Empty)
            {
                return result;
            }


            var query = (from r in Review.ObjectSet()
                         where r.AppId == appId
                         orderby r.SubTime descending
                         select new Jinher.AMP.Apm.Deploy.CustomDTO.AppPackageCommentDTO
                         {
                             //Commenter = r.UserName == "null" ? "" : r.UserName,
                             Commenter = r.UserName == "null" ? "" : r.UserName,
                             CommentSource = r.CommodityName,
                             //CommentSourceUrl
                             Description = r.Content,
                             HeaderIcon = r.UserHeader == "null" ? "" : r.UserHeader,
                             SubTime = r.SubTime
                         }
                         );
            result.Total = query.Count();

            result.Comments = query.Skip((pageIndex - 1) * pageSize)
                         .Take(pageSize).ToList();
            foreach (var appPackageCommentDTO in result.Comments)
            {
                appPackageCommentDTO.Commenter = ConvertAnonymous(appPackageCommentDTO.Commenter);
            }

            return result;
        }

        /// <summary>
        /// 时间格式转换
        /// </summary>
        /// <param name="subTime"></param>
        /// <returns></returns>
        public string ConvertPublishTime(DateTime subTime)
        {
            string retStr = string.Empty;
            DateTime day = subTime.Date;

            if (day == DateTime.Today)
            {
                retStr = subTime.ToString("HH:mm");
            }
            else if (day.Year == DateTime.Now.Year)
            {
                retStr = subTime.ToString("M-d") + subTime.ToString(" HH:mm");
            }
            else
            {
                retStr = subTime.ToString("yyyy-M-d HH:mm");
            }

            return retStr;
        }
        /// <summary>
        /// 把昵称或用户名转换为 匿名的m**
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public string ConvertAnonymous(string Name)
        {
            if (Name.Length > 0)
            {

                string Ni = "";
                for (int i = 1; i < Name.Length; i++)
                {
                    Ni += "*";
                }
                return Name.Substring(0, 1) + Ni;
            }
            else
            {
                return Name;
            }
        }

        /// <summary>
        /// 根据商品ID查询商品评价
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ReviewOnlyDTO> GetReviewOnlyByCommodityIdExt(ReviewSearchDTO search)
        {
            if (search == null || search.PageSize <= 0)
                return new List<ReviewOnlyDTO>();
            try
            {

                var query = Review.ObjectSet().
                                   Where(n => n.CommodityId == search.CommodityId);

                DateTime lastReviewTime = DateTime.Today.AddDays(2);
                if (search.LastReviewId.HasValue)
                {
                    var lastReviewId = search.LastReviewId.Value;
                    var lastReview = Review.ObjectSet().Where(c => c.Id == lastReviewId).Select(c => c.SubTime).FirstOrDefault();
                    if (lastReview != DateTime.MinValue)
                        lastReviewTime = lastReview;
                }
                else if (search.LastReviewTime.HasValue)
                {
                    lastReviewTime = search.LastReviewTime.Value;
                }

                var reviewList =
                    query.Where(n => n.SubTime < lastReviewTime).OrderByDescending(n => n.SubTime).Select(data => new ReviewOnlyDTO
                    {
                        ReviewId = data.Id,
                        UserId = data.UserId,
                        OrderItemId = data.CommodityId,
                        Name = data.UserName,
                        Details = data.Content,
                        SubTime = data.SubTime,
                        Size = data.CommodityAttributes,
                    })
                         .Take(search.PageSize)
                         .ToList();
                if (reviewList.Any())
                {
                    foreach (var reviewOnlyDTO in reviewList)
                    {
                        reviewOnlyDTO.Name = ConvertAnonymous(reviewOnlyDTO.Name);
                    }
                }
                return reviewList;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ReviewSV.GetReviewOnlyByCommodityIdExt: search:{0}", JsonHelper.JsonSerializer(search)), ex);
                return new List<ReviewOnlyDTO>();
            }
        }

        /// <summary>
        /// 评价成功后的异常通知。
        /// <para>Service Url: http://devbtp.iuoooo.com/Jinher.AMP.BTP.SV.ReviewSV.svc/ReviewNofity
        /// </para>
        /// </summary>
        /// <param name="reviewSDTO">评价实体</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public ResultDTO ReviewNofityExt(ReviewNofityDTO rnDto)
        {
            ResultDTO result = new ResultDTO();
            try
            {
                if (rnDto == null)
                {
                    result.ResultCode = 1;
                    result.Message = "参数错误，参数不能为空！";
                    return result;
                }
                if (rnDto.CommodityId == Guid.Empty)
                {
                    result.ResultCode = 2;
                    result.Message = "参数错误，商品id不能为空！";
                    return result;
                }
                if (rnDto.OrderItemId == Guid.Empty)
                {
                    result.ResultCode = 3;
                    result.Message = "参数错误，订单项Id不能为空！";
                    return result;
                }



                var orderItem = OrderItem.ObjectSet().FirstOrDefault(n => n.Id == rnDto.OrderItemId);
                if (orderItem == null)
                {
                    result.ResultCode = 4;
                    result.Message = "未找到要评价的订单项！";
                    return result;
                }
                if (orderItem.AlreadyReview)
                {
                    result.ResultCode = 0;
                    result.Message = "订单已评价过了！";
                    return result;
                }
                orderItem.AlreadyReview = true;

                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                //增加商品评价数
                Commodity com = Commodity.ObjectSet().FirstOrDefault(n => n.Id == orderItem.CommodityId);
                if (com == null)
                {
                    result.ResultCode = 4;
                    result.Message = "未找到要评价的商品！";
                    return result;
                }
                com.TotalReview += 1;
                com.ModifiedOn = DateTime.Now;
                contextSession.SaveChanges();

                com.RefreshCache(EntityState.Modified);


            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ReviewSV.ReviewNofityExt:{0}", JsonHelper.JsonSerializer(rnDto)), ex);

                result.ResultCode = -1;
                result.Message = "服务异常，请稍后重试！";
            }
            return result;
        }

        public ResultDTO ReviewNofityComOnlyExt(ReviewNofityDTO rnDto)
        {
            ResultDTO result = new ResultDTO();
            try
            {
                if (rnDto == null)
                {
                    result.ResultCode = 1;
                    result.Message = "参数错误，参数不能为空！";
                    return result;
                }
                if (rnDto.CommodityId == Guid.Empty)
                {
                    result.ResultCode = 2;
                    result.Message = "参数错误，商品id不能为空！";
                    return result;
                }

                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                //增加商品评价数
                Commodity com = Commodity.ObjectSet().FirstOrDefault(n => n.Id == rnDto.CommodityId);
                if (com == null)
                {
                    result.ResultCode = 4;
                    result.Message = "未找到要评价的商品！";
                    return result;
                }
                com.TotalReview += 1;
                com.ModifiedOn = DateTime.Now;
                contextSession.SaveChanges();

                com.RefreshCache(EntityState.Modified);


            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ReviewSV.ReviewNofityComOnlyExt:{0}", JsonHelper.JsonSerializer(rnDto)), ex);

                result.ResultCode = -1;
                result.Message = "服务异常，请稍后重试！";
            }
            return result;
        }
    }
}
