using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 商品结算价检索类
    /// </summary>
    [Serializable()]
    [DataContract()]
    public class SettlingAccountSearchDTO
    {
        /// <summary>
        /// AppId
        /// </summary>
        [DataMember()]
        public Guid appId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember()]
        public string commodityName { get; set; }

        /// <summary>
        /// 类目名称
        /// </summary>
        [DataMember()]
        public string commodityCategory { get; set; }


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
