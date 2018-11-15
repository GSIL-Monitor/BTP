using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// App自提点查询结果 BP
    /// </summary>
    [Serializable]
    [DataContract]
    public class AppSelfTakeStationSearchResultSDTO
    {
        /// <summary>
        /// 记录数
        /// </summary>
        [DataMember]
        public int Count { get; set; }
        /// <summary>
        /// App自提点信息列表
        /// </summary>
        [DataMember]
        public List<AppSelfTakeStationSDTO> Data { get; set; }
    }
}
