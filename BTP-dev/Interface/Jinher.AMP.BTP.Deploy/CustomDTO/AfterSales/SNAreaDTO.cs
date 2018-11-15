using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.AfterSales
{
    /// <summary>
    /// 区域实体
    /// </summary>
    [Serializable]
    [DataContract]
    public class SNAreaDTO
    {
        /// <summary>
        /// 区域编码
        /// </summary>
        [DataMember]
        public string A { get; set; }
        /// <summary>
        /// 区域级别
        /// </summary>
        [DataMember]
        public string L { get; set; }
        /// <summary>
        /// 区域名称
        /// </summary>
        [DataMember]
        public string N { get; set; }
        /// <summary>
        /// 父Id
        /// </summary>
        [DataMember]
        public string P { get; set; }
    }
}
