

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Jinher.JAP.Metadata;
using Jinher.JAP.Metadata.Description;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.BF.BE.Base;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.Common.Exception;
using Jinher.JAP.Common.Exception.ComExpDefine;
using Jinher.JAP.Common;
using Jinher.JAP.PL;
using Jinher.AMP.BTP.Deploy.CustomDTO;
namespace Jinher.AMP.BTP.BE
{
    public partial class Reply
    {
        #region 基类抽象方法重载

        public override void BusinessRuleValidate()
        {
        }
        #endregion
        #region 基类虚方法重写
        public override void SetDefaultValue()
        {
            base.SetDefaultValue();
        }
        #endregion

        /// <summary>
        /// 根据商品ID得到所有评价回复
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        public List<ReviewVM> GetReplyListByCommodityId(Guid commodityId, int pageIndex, int pageSize)
        {
            Reply reply = new Reply();
            IEnumerable<ReviewVM> query = from data in Review.ObjectSet()
                                          where data.CommodityId == commodityId
                                          select new ReviewVM
                                          {
                                              SubTime = data.SubTime,
                                              Details = data.Content,
                                              ReviewId = data.Id,
                                              UserId = data.UserId,
                                              AppId = data.AppId,
                                              ReviewUserName=data.UserName==null?"":data.UserName
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
                                 }).OrderBy(a=>a.ReplyerSubTime).ToList().ToList();
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
            return query.ToList();
        }


    }
}



