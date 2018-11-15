using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 评价列表
    /// </summary>
    [Serializable()]
    [DataContract]
    public class ReviewOnlyDTO
    {
        /// <summary>
        /// 评价ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid ReviewId { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid UserId { get; set; }

        /// <summary>
        /// 订单中商品的OrderItemId，取订单详情信息中商品信息中的Id属性值
        /// </summary>
        [DataMemberAttribute()]
        public Guid OrderItemId { get; set; }

        /// <summary>
        /// 商品属性
        /// </summary>
        [DataMemberAttribute()]
        public string Size { get; set; }
        
        /// <summary>
        /// 评价人昵称
        /// </summary>
        [DataMemberAttribute()]
        public string Name { get; set; }

        /// <summary>
        /// 评价内容
        /// </summary>
        [DataMemberAttribute()]
        public string Details { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? SubTime { get; set; }
 

       

    }
}
