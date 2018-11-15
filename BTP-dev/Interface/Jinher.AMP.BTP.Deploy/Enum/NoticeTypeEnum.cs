using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.Enum
{
    /// <summary>
    /// 商品使用的活动类型
    /// </summary>
    [Serializable]
    [DataContract]
    public enum NoticeTypeEnum
    {
        /// <summary>
        /// 到货提醒
        /// </summary>
        [EnumMember]
        到货提醒 = 0,
        /// <summary>
        /// 已设置到货提醒
        /// </summary>
        [EnumMember]
        已设置到货提醒 = 1,
        /// <summary>
        /// 取消到货提醒
        /// </summary>
        [EnumMember]
        取消到货提醒 = 2,
        /// <summary>
        /// 接受短信通知
        /// </summary>
        [EnumMember]
        接受短信通知 = 3,        
        /// <summary>
        /// 取消短信到货提醒
        /// </summary>
        [EnumMember]
        已取消短信到货提醒 = 4,
    }
}
