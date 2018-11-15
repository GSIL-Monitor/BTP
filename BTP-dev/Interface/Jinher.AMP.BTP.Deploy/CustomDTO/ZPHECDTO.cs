using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 电商馆
    /// </summary>
    [Serializable]
    [DataContract]
    public class ZPHECDTO
    {
        /// <summary>
        /// 所在区域
        /// </summary>
        [DataMember]
        public Guid changeOrg { get; set; }
        /// <summary>
        /// 所在市
        /// </summary>
        [DataMember]
        public string cityCode { get; set; }
        /// <summary>
        /// 标识Id
        /// </summary>
        [DataMember]
        public Guid id { get; set; }
        /// <summary>
        /// IW号
        /// </summary>
        [DataMember]
        public string iwAccount { get; set; }
        /// <summary>
        /// 所在省
        /// </summary>
        [DataMember]
        public string provinceCode { get; set; }
        /// <summary>
        /// 总代名称
        /// </summary>
        [DataMember]
        public string proxyName { get; set; }
    }
}
