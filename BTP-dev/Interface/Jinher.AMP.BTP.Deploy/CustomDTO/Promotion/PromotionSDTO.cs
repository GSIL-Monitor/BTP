using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 促销展示
    /// </summary>
    [Serializable()]
    [DataContract]
    public class PromotionSDTO
    {
        /// <summary>
        /// 促销名称
        /// </summary>
        [DataMemberAttribute()]
        public string Name { get; set; }
        /// <summary>
        /// 促销ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid PromotionId { get; set; }
        /// <summary>
        /// 促销图片
        /// </summary>
        [DataMemberAttribute()]
        public string PicPath { get; set; }
        /// <summary>
        /// 促销开始时间---没有促销时显示当时时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 促销结束时间---没有促销时显示当时时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 促销折扣
        /// </summary>
        [DataMemberAttribute()]
        public decimal Intensity { get; set; }
        /// <summary>
        /// 是否可用
        /// </summary>
        [DataMemberAttribute()]
        public bool IsEnable { get; set; }

        /// <summary>
        /// 当前时间
        /// </summary>
        [DataMember]
        public DateTime CurrentTime { get; set; }

        /// <summary>
        /// 优惠价
        /// </summary>
        [DataMemberAttribute()]
        public decimal? DiscountPrice { get; set; }
    }
}
