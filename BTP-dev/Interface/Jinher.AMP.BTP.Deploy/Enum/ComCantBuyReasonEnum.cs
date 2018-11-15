using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.Enum
{
    /// <summary>
    /// 排序字段
    /// </summary>
    [DataContract]
    public enum ComCantBuyReasonEnum
    {

        /// <summary>
        /// 没有错（默认值）
        /// </summary>
        [EnumMemberAttribute]
        NoError = 0,

        /// <summary>
        /// 价格发生变化
        /// </summary>
        [EnumMemberAttribute]
        NoCom = 1,
        /// <summary>
        /// 商品已下架
        /// </summary>
        [EnumMemberAttribute]
        Deled = 2,
        /// <summary>
        /// 商品已下架
        /// </summary>
        [EnumMemberAttribute]
        State = 4,
        /// <summary>
        /// 价格发生变化
        /// </summary>
        [EnumMemberAttribute]
        Price = 8,
        /// <summary>
        /// 库存不足
        /// </summary>
        [EnumMemberAttribute]
        Stock = 16,
        /// <summary>
        /// 活动已失效   
        /// </summary>
        [EnumMemberAttribute]
        NoPromotion = 32,
        /// <summary>
        /// 活动未开始
        /// </summary>
        [EnumMemberAttribute]
        PromotionNotStart = 32,
        /// <summary>
        /// 活动已结束
        /// </summary>
        [EnumMemberAttribute]
        PromotionEnded = 64,
        /// <summary>
        /// VIP身份有误
        /// </summary>
        [EnumMemberAttribute]
        VipError = 128,

    }
}
