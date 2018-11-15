using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.Enum
{
    /// <summary>
    /// 电商BTP的APP消息的二级类型
    /// </summary>
    [DataContract]
    public enum MobileMsgSecondTypeEnum
    {
        /// <summary>
        /// 增值税普通发票
        /// </summary>
        [EnumMemberAttribute]
        RedPackets = 3,
        /// <summary>
        /// 电子发票
        /// </summary>
        [EnumMemberAttribute]
        PreSale ,
        /// <summary>
        /// 售后业务
        /// </summary>
        [EnumMemberAttribute]
        AfterSalesService ,
        /// <summary>
        /// 分销商审核
        /// </summary>
        [EnumMemberAttribute]
        DistributionAudit ,
    }
}