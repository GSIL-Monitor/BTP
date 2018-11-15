using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.Enum
{
    /// <summary>
    /// 行为记录枚举
    /// </summary>
    [DataContract]
    public enum BehaviorTypeEnum
    {
        /// <summary>
        /// 浏览商品
        /// </summary>
        [EnumMember]
        CommodityDetail = 1,
        /// <summary>
        /// 收藏商品
        /// </summary>
        [EnumMember]
        Collection = 2,
        /// <summary>
        /// 删除收藏
        /// </summary>
        [EnumMember]
        DeleteCollection = 3,
        /// <summary>
        /// 添加购物车
        /// </summary>
        [EnumMember]
        AddShoppingCart = 4,
        /// <summary>
        /// 删除购物车商品
        /// </summary>
        [EnumMember]
        DeleteShoppingCart = 5,
        /// <summary>
        /// 确认支付
        /// </summary>
        [EnumMember]
        ConfirmPay = 6





    }
}
