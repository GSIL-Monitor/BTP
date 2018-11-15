using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.Enum
{
    /// <summary>
    /// 购物车商品状态
    /// </summary>
    [DataContract]
    public enum ShopCartStateEnum
    {

        /// <summary>
        /// 正常
        /// </summary>
        [EnumMemberAttribute]
        OK = 0,
        /// <summary>
        /// 库存为0
        /// </summary>
        [EnumMemberAttribute]
        Stock = 1,
        /// <summary>
        /// 下架商品
        /// </summary>
        [EnumMemberAttribute]
        OffSale = 2,
        /// <summary>
        /// 已删除
        /// </summary>
        [EnumMemberAttribute]
        Del = 3,
        /// <summary>
        /// 属性变动
        /// </summary>
        [EnumMemberAttribute]
        Attribute = 4,
        /// <summary>
        /// 其他错误
        /// </summary>
        [EnumMemberAttribute]
        Others = 9999
    }
}
