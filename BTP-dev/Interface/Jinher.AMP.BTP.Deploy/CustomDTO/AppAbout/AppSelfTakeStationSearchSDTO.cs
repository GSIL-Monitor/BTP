using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// App自提点查询 BP
    /// </summary>
    [Serializable]
    [DataContract]
    public class AppSelfTakeStationSearchSDTO : SearchBase
    {
        /// <summary>
        /// 自提点名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 省编码
        /// </summary>
        [DataMember]
        public string Province { get; set; }
        /// <summary>
        /// 城市编码
        /// </summary>
        [DataMember]
        public string City { get; set; }
        /// <summary>
        /// 区县编码
        /// </summary>
        [DataMember]
        public string District { get; set; }
    }
}
