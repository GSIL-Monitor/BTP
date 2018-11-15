using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.Enum
{
    /// <summary>
    /// 订单状态
    /// </summary>
    [DataContract]
    public enum OrderStatusEnum
    {
        /// <summary>
        /// 未支付
        /// </summary>
        [EnumMemberAttribute]
        nopayment = 0,
        /// <summary>
        /// 未发货
        /// </summary>
        [EnumMemberAttribute]
        nodelivery = 1,
        /// <summary>
        /// 已发货
        /// </summary>
        [EnumMemberAttribute]
        shipped = 2,
        /// <summary>
        /// 确认收货
        /// </summary>
        [EnumMemberAttribute]
        confirm = 3,
        /// <summary>
        /// 已取消
        /// </summary>
        [EnumMemberAttribute]
        delete = 4,

        /// <summary>
        /// 未处理
        /// </summary>
        [EnumMemberAttribute]
        CYUntreated = 18,

        /// <summary>
        /// 已处理
        /// </summary>
        [EnumMemberAttribute]
        CYProcessed = 19
    }
}
