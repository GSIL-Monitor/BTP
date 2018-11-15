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
    public class AppSetSearch2DTO : AppSetSearchDTO
    {
        /// <summary>
        /// 所属AppId
        /// </summary>
        [DataMember]
        public Guid belongTo { get; set; }
    }
}
