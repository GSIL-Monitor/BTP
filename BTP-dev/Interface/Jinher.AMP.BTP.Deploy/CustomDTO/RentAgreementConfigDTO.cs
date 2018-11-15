using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 协议配置类
    /// </summary>
    [Serializable]
    [DataContract]
    public class RentAgreementConfigDTO
    {
        /// <summary>
        /// 商品Id
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        [DataMember]
        public string Name { get; set; }
    }
}
