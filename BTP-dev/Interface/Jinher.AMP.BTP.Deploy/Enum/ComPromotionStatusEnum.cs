using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.Enum
{
    /// <summary>
    /// 商品使用的活动类型
    /// </summary>
    [Serializable]
    [DataContract]
    public enum ComPromotionStatusEnum
    {
        /// <summary>
        /// 普通活动
        /// </summary>
        [EnumMember]
        CommonPromotion = 0,
        /// <summary>
        /// 秒杀
        /// </summary>
        [EnumMember]
        Seckill = 1,
        /// <summary>
        /// 预约
        /// </summary>
        [EnumMember]
        Presell = 2,
        /// <summary>
        /// 拼团
        /// </summary>
        [EnumMember]
        DiyGroup = 3,
        /// <summary>
        /// 预售
        /// </summary>
        [EnumMember]
        DirectPresell = 5,
        /// <summary>
        /// 会员折扣
        /// </summary>
        [EnumMember]
        VipIntensity = 200,
        /// <summary>
        /// 无优惠
        /// </summary>
        [EnumMember]
        NoPromotion = 9999
    }
}
