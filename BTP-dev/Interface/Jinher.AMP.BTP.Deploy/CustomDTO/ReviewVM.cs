using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable]
    [DataContract]
    public class ReviewVM
    {
        /// <summary>
        /// 评价ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid ReviewId { get; set; }
        /// <summary>
        /// 评价数
        /// </summary>
        [DataMemberAttribute()]
        public int ReviewNum { get; set; }
        // <summary>
        /// 用户ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid UserId { get; set; }
        /// <summary>
        /// APPID
        /// </summary>
        [DataMemberAttribute()]
        public Guid AppId { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid CommodityId { get; set; }
        /// <summary>
        /// 商品图片
        /// </summary>
        [DataMemberAttribute()]
        public string CommodityPicture { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMemberAttribute()]
        public string CommodityName { get; set; }
        /// <summary>
        /// 评价人昵称
        /// </summary>
        [DataMemberAttribute()]
        public string ReviewUserName { get; set; }
        /// <summary>
        /// 评价内容
        /// </summary>
        [DataMemberAttribute()]
        public string Details { get; set; }
        /// <summary>
        /// 评价时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime SubTime { get; set; }
        /// <summary>
        /// 回复数量
        /// </summary>
        [DataMemberAttribute()]
        public int Number { get; set; }
        /// <summary>
        /// 回复列表
        /// </summary>
        [DataMemberAttribute()]
        public List<ReplyVM> ReplyList { get; set; }
        /// <summary>
        /// 是否回复
        /// </summary>
        [DataMemberAttribute()]
        public bool IsReply { get; set; }
    }
}
