using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 厂家直营app相关查询
    /// </summary>
    [Serializable()]
    [DataContract]
    public class AppSetSearchDTO : SearchBase
    {
        /// <summary>
        /// 分类Id
        /// </summary>
        [DataMember]
        public Guid CategoryId { get; set; }
    }
}
