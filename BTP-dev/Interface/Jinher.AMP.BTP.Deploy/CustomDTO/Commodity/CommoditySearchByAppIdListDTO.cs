using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 商品检索类
    /// </summary>
    [Serializable()]
    [DataContract()]
    public class CommoditySearchByAppIdListDTO
    {
        /// <summary>
        /// appId的列表
        /// </summary>
        [DataMember()]
        public List<System.Guid> appIdList { get; set; }

        [DataMember()]
        public int pageSize { get; set; }

        [DataMember()]
        public int pageIndex { get; set; }

        /// <summary>
        /// 数据总数
        /// </summary>
        [DataMember()]
        public int rowCount { get; set; }
    }
}
