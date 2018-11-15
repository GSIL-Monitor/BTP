using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 分销统计结果
    /// </summary>
    [Serializable]
    [DataContract]
    public class DistributorProfitsResultDTO
    {
        /// <summary>
        /// 统计信息列表
        /// </summary>
        [DataMember]
        public List<DistributorProfitsInfoDTO> DistributorProfitsInfoList { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        [DataMember]
        public int Count { get; set; }
    }
}
