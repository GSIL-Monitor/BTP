using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 推广信息查询
    /// </summary>
    [Serializable()]
    [DataContract]
    public class SpreadInfoSearchDTO
    {
        /// <summary>
        /// 推广码
        /// </summary>
        [DataMember]
        public Guid SpreadCode { get; set; }
    }
}
