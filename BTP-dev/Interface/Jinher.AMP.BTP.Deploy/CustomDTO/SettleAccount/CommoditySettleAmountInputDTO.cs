using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 商品结算价设置DTO
    /// </summary>
    [Serializable]
    [DataContract]
    public class CommoditySettleAmountInputDTO
    {
        [DataMember]
        public Guid EsAppId { get; set; }

        [DataMember]
        public Guid AppId { get; set; }

        [DataMember]
        public Guid UserId { get; set; }

        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        /// 生效时间
        /// </summary>
        [DataMember]
        public DateTime? EffectiveTime { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMember]
        public Guid CommodityId { get; set; }

        [DataMember]
        public string AttributeName { get; set; }

        [DataMember]
        public string SecAttributeName { get; set; }

        [DataMember]
        public List<CommodityAttributePrice> Items { get; set; }
    }
}
