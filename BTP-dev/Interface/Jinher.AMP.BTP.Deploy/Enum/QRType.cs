using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.Enum
{
    /// <summary>
    /// 类型
    /// </summary>
    [DataContract]
    [Serializable]
    public enum QrType
    {
        /// <summary>
        /// 门店
        /// </summary>
        [EnumMember]
        Store = 0,
        /// <summary>
        /// 桌号
        /// </summary>
        [EnumMember]
        Table = 1,
        /// <summary>
        /// 服务员
        /// </summary>
        [EnumMember]
        Waiter = 2,
        /// <summary>
        /// 推广主
        /// </summary>
        [EnumMember]
        SpreadManager = 3
    }
}
