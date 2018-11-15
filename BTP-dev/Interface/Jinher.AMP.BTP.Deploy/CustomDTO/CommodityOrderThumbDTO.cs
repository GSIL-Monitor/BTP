using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 订单缩略信息
    /// </summary>
    [Serializable()]
    [DataContract]
    public class CommodityOrderThumbDTO
    {
        /// <summary>
        /// 订单id
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        [DataMember]
        public string Code { get; set; }


        /// <summary>
        /// 电商平台id（电商馆id）
        /// </summary>
        [DataMember]
        public Guid? EsAppId { get; set; }


        /// <summary>
        /// 提交订单时间
        /// </summary>
        [DataMember]
        public DateTime SubTime { get; set; }

        /// <summary>
        /// 买家
        /// </summary>
        [DataMember]
        public Guid UserId { get; set; }

        /// <summary>
        /// 订单状态（必填）：未支付=0，未发货=1，已发货=2，确认收货=3，商家取消订单=4，客户取消订单=5，超时交易关闭=6，已退款=7，待发货退款中=8，已发货退款中=9,已发货退款中商家同意退款，商家未收到货=10,付款中=11,金和处理退款中=12,出库中=13，出库中退款中=14，待审核=16，待付款待审核=17，餐饮订单待处理=18，餐饮订单已处理=19
        /// </summary>
        [DataMember]
        public int State { get; set; }
    }
}
