using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.Enum
{
    /// <summary>
    /// 付款方式
    /// </summary>
    [DataContract]
    public enum PaymentEnum
    {
        /// <summary>
        /// 暂无
        /// </summary>
        [EnumMemberAttribute]
        empty = -1,
        /// <summary>
        /// 金币
        /// </summary>
        [EnumMemberAttribute]
        gold = 0,
        /// <summary>
        /// 货到付款
        /// </summary>
        [EnumMemberAttribute]
        cash = 1,
        /// <summary>
        /// 支付宝
        /// </summary>
        [EnumMemberAttribute]
        alipay = 2,
        /// <summary>
        /// 支付宝担保
        /// </summary>
        [EnumMemberAttribute]
        alipayGuarantee = 3,
        /// <summary>
        /// U付
        /// </summary>
        [EnumMemberAttribute]
        upay = 4


    }
}
