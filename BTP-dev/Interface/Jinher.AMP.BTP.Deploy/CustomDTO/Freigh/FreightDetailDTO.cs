using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 运费详细
    /// </summary>
    [Serializable()]
    [DataContract]
    public class FreightDetailDTO : ResultDTO
    {
        /// <summary>
        /// 运费详细列表
        /// </summary>
        [DataMemberAttribute()]
        public List<FreightDetail> FreightList { get; set; }
    }
    [Serializable]
    [DataContract]
    public class FreightDetail
    {
        /// <summary>
        /// 运送到
        /// </summary>
        [DataMember]
        public string FreightTo { get; set; }
        /// <summary>
        /// 运费
        /// </summary>
        [DataMember]
        public decimal Freitht { get; set; }
    }
}
