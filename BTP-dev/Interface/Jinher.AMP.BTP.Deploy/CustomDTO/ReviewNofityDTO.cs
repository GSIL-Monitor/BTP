using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 评价成功异步通知参数实体。
    /// </summary>
    [Serializable()]
    [DataContract]
    public class ReviewNofityDTO
    {
        /// <summary>
        /// 订单项Id
        /// </summary>
        [DataMember]
        public Guid OrderItemId { get; set; }

        /// <summary>
        /// 商品id
        /// </summary>
        [DataMember]
        public Guid CommodityId { get; set; }
    }
}
