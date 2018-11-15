using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 订阅物流信息.
    /// </summary>
    [DataContract]
    public class KdSubscribeDTO
    {
        /// <summary>
        /// 物流公司简称
        /// </summary>
        [DataMember]
        public string Code { get; set; }

        /// <summary>
        /// 单号集合
        /// </summary>
        [DataMember]
        public List<SubItem> Item { get; set; }
    }

    /// <summary>
    /// 要订阅的单号信息
    /// </summary>
    [DataContract]
    public class SubItem
    {
        /// <summary>
        /// 物流单号
        /// </summary>
        [DataMember]
        public string No { get; set; }

        /// <summary>
        /// 备注信息(推送信息时会随之返回)
        /// </summary>
        [DataMember]
        public string Bk { get; set; }
    }
}
