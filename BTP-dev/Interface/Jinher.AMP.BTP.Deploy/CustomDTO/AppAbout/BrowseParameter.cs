using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    [DataContract]
    public class BrowseParameter:SearchBase
    {
        [DataMember]
        public Guid userId { get; set; }
        [DataMember]
        public Guid appId { get; set; }
    }
}