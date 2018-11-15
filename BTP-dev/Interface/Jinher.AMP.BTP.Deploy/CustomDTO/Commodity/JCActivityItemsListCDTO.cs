using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 金采团购活动sku
    /// </summary>
    [Serializable]
    [DataContract]
    public class JCActivityItemsListCDTO:DBBase
    {
        /// <summary>
        /// 活动Id
        /// </summary>
        [DataMember]
        public Guid JCActivityId { get; set; }
        /// <summary>
        /// 商品id
        /// </summary>
        [DataMember]
        public Guid ComdtyId { get; set; }
        /// <summary>
        /// 商品库存id
        /// </summary>
        [DataMember]
        public Guid ComdtyStockId { get; set; }
        /// <summary>
        /// 油卡兑换券比例
        /// </summary>
        [DataMember]
        public decimal GiftGardScale { get; set; }

        /// <summary>
        /// 活动价格
        /// </summary>
        [DataMember]
        public decimal GroupPrice { get; set; }
    }
}
