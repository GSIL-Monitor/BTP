using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.Enum
{
    /// <summary>
    /// 排序：1升序；2降序
    /// </summary>
    [DataContract]
    [Serializable]
    public enum OrderType
    {
        /// <summary>
        /// 升序
        /// </summary>
        [EnumMember]
        ASC = 1,
        /// <summary>
        /// 降序
        /// </summary>
        [EnumMember]
        DESC = 2        
    }
}
