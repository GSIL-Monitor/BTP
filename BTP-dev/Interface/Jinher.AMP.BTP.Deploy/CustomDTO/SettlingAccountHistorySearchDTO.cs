using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 商品结算价修改历史检索类
    /// </summary>
    [Serializable()]
    [DataContract()]
    public class SettlingAccountHistorySearchDTO
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMember()]
        public Guid CommodityId { get; set; }


        [DataMember()]
        public int PageSize { get; set; }

        [DataMember()]
        public int PageIndex { get; set; }

        /// <summary>
        /// 数据总数
        /// </summary>
        [DataMember()]
        public int RowCount { get; set; }
    }
}
