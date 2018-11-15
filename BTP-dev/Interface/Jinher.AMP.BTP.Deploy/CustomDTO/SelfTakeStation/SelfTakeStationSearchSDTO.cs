using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 自提点信息查询类
    /// </summary>
    [Serializable]
    [DataContract]
    public class SelfTakeStationSearchSDTO
    {
        /// <summary>
        /// 所属总代
        /// </summary>
        [DataMember()]
        public Guid CityOwnerId { get; set; }
        /// <summary>
        /// 自提点名称
        /// </summary>
        [DataMember()]
        public string Name { get; set; }

        [DataMember()]
        public int pageSize { get; set; }

        [DataMember()]
        public int pageIndex { get; set; }

        /// <summary>
        /// 数据总数
        /// </summary>
        [DataMember()]
        public int rowCount { get; set; }

        /// <summary>
        /// 类型：总代=0，电商馆=1
        /// </summary>
        [DataMember()]
        public int? SelfTakeStationType { get; set; }

        /// <summary>
        /// 电商馆appId
        /// </summary>
        [DataMember()]
        public Guid AppId { get; set; }
    }
}
