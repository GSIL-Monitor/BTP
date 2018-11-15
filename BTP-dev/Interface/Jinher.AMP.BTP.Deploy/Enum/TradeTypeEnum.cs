using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.Enum
{
    /// <summary>
    /// 交易类型
    /// </summary>
    [DataContract]
    public enum TradeTypeEnum
    {
        /// <summary>
        /// 担保交易
        /// </summary>
        [EnumMember]
        SecTrans = 0,
        /// <summary>
        /// 直接到账
        /// </summary>
        [EnumMember]
        Direct = 1,
        /// <summary>
        /// 货到付款
        /// </summary>
        [EnumMember]
        Hdfk = 2,

        /// <summary>
        /// 异常
        /// </summary>
        [EnumMember]
        None = -1,

    }
}
