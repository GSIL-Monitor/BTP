using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable]
    [DataContract]
    public class PromotionCookieDTO
    {
        /// <summary>
        /// 促销图片本地路径（缩略图）
        /// </summary>
        [DataMember]
        public string Picture { get; set; }

        /// <summary>
        /// 促销图片名称
        /// </summary>
        [DataMember]
        public string PicName { get; set; }

        /// <summary>
        /// 手机里显示的图片
        /// </summary>
        [DataMember]
        public string picID { get; set; }

        /// <summary>
        /// 促销名称
        /// </summary>
        [DataMember]
        public string PromotionName { get; set; }

        /// <summary>
        /// 促销开始时间
        /// </summary>
        [DataMember]
        public string StartTime { get; set; }

        /// <summary>
        /// 促销结束时间
        /// </summary>
        [DataMember]
        public string EndTime { get; set; }

        /// <summary>
        /// 促销商品折扣
        /// </summary>
        [DataMember]
        public string IntenSity { get; set; }

        /// <summary>
        /// 促销所选商品Code
        /// </summary>
        [DataMember]
        public string CommodityIds { get; set; }
        /// <summary>
        /// 活动类型 0普通活动,1秒杀活动
        /// </summary>
        [DataMember]
        public string PromotionType { get; set; }
    }
}
