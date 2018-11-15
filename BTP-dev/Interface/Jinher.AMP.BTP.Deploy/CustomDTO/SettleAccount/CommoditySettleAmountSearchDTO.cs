using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 商品结算列表搜索DTO
    /// </summary>
    [Serializable]
    [DataContract]
    public class CommoditySettleAmountSearchDTO : SearchBase
    {
        [DataMember]
        public Guid AppId { get; set; }

        [DataMember]
        public Guid EsAppId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 是否已设置结算
        /// </summary>
        [DataMember]
        public bool? HasSetted { get; set; }
    }
}
