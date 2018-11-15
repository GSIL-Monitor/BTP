using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 订阅结果
    /// </summary>
    [DataContract]
    public class KdSubscribeResultDTO
    {
        /// <summary>
        /// 电商用户ID
        /// </summary>
        [DataMember]
        public string EBusinessID { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        [DataMember]
        public string UpdateTime { get; set; }

        /// <summary>
        /// 成功与否
        /// </summary>
        [DataMember]
        public bool Success { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Reason { get; set; }
    }
}
