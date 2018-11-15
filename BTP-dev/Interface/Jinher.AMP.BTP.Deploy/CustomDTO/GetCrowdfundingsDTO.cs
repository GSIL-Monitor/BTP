using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable]
    [DataContract]
    public class GetCrowdfundingsDTO
    {
       

        /// <summary>
        /// 众筹列表
        /// </summary>
        [DataMember]
        public System.Collections.Generic.List<CrowdfundingFullDTO> List { get; set; }
        /// <summary>
        /// 总数
        /// </summary>
        [DataMember]
        public int Total { get; set; }
    }
}
