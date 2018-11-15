using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace Jinher.AMP.BTP.Deploy.Enum
{
    /// <summary>
    /// 回款类型  电汇=0,支票=1,内部挂账=2,现金=3,其他=4
    /// </summary>
    [DataContract]
    public enum RefundTypeEnum
    {
        [EnumMember]
        电汇=0,
        [EnumMember]
        支票=1,
        [EnumMember]
        内部挂账=2,
        [EnumMember]
        现金 = 3,
        [EnumMember]
        其他 = 4
    }
}
