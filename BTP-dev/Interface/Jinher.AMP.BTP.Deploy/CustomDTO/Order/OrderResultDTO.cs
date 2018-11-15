using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 订单返回结果
    /// </summary>
    [Serializable()]
    [DataContract]
    public class OrderResultDTO : ResultDTO
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid OrderId { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        [DataMemberAttribute()]
        public string OrderCode { get; set; }
        /// <summary>
        /// 运费
        /// </summary>
        [DataMemberAttribute()]
        public decimal Freight { get; set; }
        /// <summary>
        /// 未支付超时时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? ExpireTime { get; set; }
        /// <summary>
        /// 在线支付包括支付宝与U付时,要跳转到的支付页面
        /// </summary>
        [DataMemberAttribute()]
        public string PayUrl { get; set; }

        /// <summary>
        /// 拼团ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid? DiyGroupId { get; set; }

        public Guid? CommodityId { get; set; }
        public Guid CommodityStockId { get; set; }

        /// <summary>
        /// 售罄的商品
        /// </summary>
        public List<CommoditySummaryDTO> ErrorCommodities { get; set; }
    }
}
