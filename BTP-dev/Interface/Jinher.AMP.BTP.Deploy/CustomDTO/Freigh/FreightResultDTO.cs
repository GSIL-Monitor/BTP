using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 运费计算
    /// </summary>
    [Serializable()]
    [DataContract]
    public class FreightResultDTO:ResultDTO
    {
        /// <summary>
        /// 运费：应用id,运费
        /// </summary>
        [DataMemberAttribute()]
        public List<Tuple<Guid, decimal>> Freights { get; set; }
        /// <summary>
        /// 返回运费（改动后的版本返回总运费）
        /// </summary>
        [DataMemberAttribute()]
        public decimal Freight{ get; set; }
    }

     
}
