using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 二维码类型
    /// </summary>
    [Serializable]
    [DataContract]
    public class QrTypeDTO
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int Type { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        public string Description { get; set; }
    }
}
