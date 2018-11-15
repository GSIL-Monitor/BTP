using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 分销订单结果
    /// </summary>
    [Serializable]
    [DataContract]
    public class CommodityOrderDistributionResultDTO
    {
        /// <summary>
        /// 分销订单列表
        /// </summary>
        [DataMember]
        public List<CommodityOrderDistributionInfoDTO> CommodityOrderDistributionInfoList { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        [DataMember]
        public int Count { get; set; }
    }
}
