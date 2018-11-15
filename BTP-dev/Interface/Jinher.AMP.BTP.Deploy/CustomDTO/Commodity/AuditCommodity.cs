using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.Commodity
{
    [Serializable]
    [DataContract]
    public class AuditCommodity
    {
        /// <summary>
        ///
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }
        /// <summary>
        ///  用户名称
        /// </summary>
        [DataMember]
        public string UserName { get; set; }
    }
}
