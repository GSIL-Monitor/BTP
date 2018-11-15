using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 推广APP
    /// </summary>
    [Serializable]
    [DataContract]
    public class SpreadAppDTO
    {
        /// <summary>
        /// AppID
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// App名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }
    }
}
