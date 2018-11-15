using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 到货提醒
    /// </summary>
    [Serializable()]
    [DataContract]
    public class NotificationsDTO
    {
        /// <summary>
        /// 到货提醒状态
        /// </summary>
        [DataMember]
        public int state { get; set; }
        /// <summary>
        /// 到货提醒内容
        /// </summary>
        [DataMember]
        public string Content { get; set; }

    }
}
