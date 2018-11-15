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
    public class SelfTakeStationSearchDTO
    {
        /// <summary>
        /// 地区代码
        /// </summary>
        [DataMember()]
        public string Code { get; set; }

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
        /// 城市总代组织Id
        /// </summary>
        [DataMember()]
        public Guid CityOwnerId { get; set; }
        /// <summary>
        /// 店铺ID
        /// </summary>
        [DataMember()]
        public Guid appId { get; set; }
        /// <summary>
        /// 搜索内容
        /// </summary>
        [DataMember()]
        public string searchContent { get; set; }
    }
}
