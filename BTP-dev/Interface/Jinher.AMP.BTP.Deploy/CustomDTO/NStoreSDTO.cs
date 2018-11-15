using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 首次门店列表
    /// </summary>
    [Serializable()]
    [DataContract]
   public class NStoreSDTO
    {
        /// <summary>
        /// 门店列表
        /// </summary>
        [DataMemberAttribute()]
        public List<StoreSDTO> Stroes { get; set; }
        /// <summary>
        /// 省份LIST
        /// </summary>
        [DataMemberAttribute()]
        public List<string> Proviences { get; set; }
    }
}
