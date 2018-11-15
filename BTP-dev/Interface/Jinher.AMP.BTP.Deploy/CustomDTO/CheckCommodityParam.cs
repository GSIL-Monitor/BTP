using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 校验商品接口参数
    /// </summary>
    [Serializable()]
    [DataContract]
    public class CheckCommodityParam
    {
        /// <summary>
        /// 当前用户id
        /// </summary>
        [DataMember]
        public Guid UserID { get; set; }

        /// <summary>
        /// 商品属性id 列表
        /// </summary>
        [DataMember]
        public List<CommodityIdAndStockId> CommodityIdsList { get; set; }

        /// <summary>
        /// 拼团活动id
        /// </summary>
        [DataMember]
        public Guid DiygId { get; set; }

        /// <summary>
        /// 活动类型  => 0，普通活动;1，秒杀;2，预售; 3，拼团 ; -1：表示0、1、2;
        /// </summary>
        [DataMember]
        public int PromotionType { get; set; }

        /// <summary>
        /// 金采团购活动
        /// </summary>
        [DataMember]
        public Guid JcActivityId { get; set; }
    }
}
