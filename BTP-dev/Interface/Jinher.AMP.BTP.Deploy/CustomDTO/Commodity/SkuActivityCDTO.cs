using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 活动sku
    /// </summary>
    [Serializable]
    [DataContract]
    public class SkuActivityCDTO:DBBase
    {
        /// <summary>
        /// 活动Id
        /// </summary>
        [DataMember]
        public Guid OutSideActivityId { get; set; }
        /// <summary>
        /// 活动类型 1、秒杀 2、预售 3、拼团 4、打折促销
        /// </summary> 
        [DataMember]
        public int OutSideActivityType { get; set; }
        /// <summary>
        /// 商品id
        /// </summary>
        [DataMember]
        public Guid CommodityId { get; set; }
        /// <summary>
        /// 商品库存id
        /// </summary>
        [DataMember]
        public Guid CommodityStockId { get; set; }
        /// <summary>
        /// 是否参与活动
        /// </summary>
        [DataMember]
        public bool IsJoin { get; set; }

        /// <summary>
        /// 参与价格
        /// </summary>
        [DataMember]
        public decimal JoinPrice { get; set; }
    }
}
