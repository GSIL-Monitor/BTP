using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.Enum
{
    /// <summary>
    /// 排序字段：0排序号、1销量、2价格
    /// </summary>
    [DataContract]
    [Serializable]
    public enum FieldSort4Mobile
    {
        /// <summary>
        /// 综合
        /// </summary>
        [EnumMember]
        Default=0,
        /// <summary>
        /// 价格
        /// </summary>
        [EnumMember]
        Price=2,
        /// <summary>
        /// 销量
        /// </summary>
        [EnumMember]
        Sales=1,

        /// <summary>
        /// 赠卡
        /// </summary>
        [EnumMember]
        YouKaPercent=3
    }
}
