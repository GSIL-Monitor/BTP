using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    ///  应用信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class CreateOrderToFspDTO
    {
        /// <summary>
        /// 要在线支付的钱
        /// </summary>
        [DataMember]
        public Decimal RealPrice { get; set; }

        /// <summary>
        /// 第一个商品名称
        /// </summary>
        [DataMember]
        public string FirstCommodityName { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        [DataMember]
        public int SrcType { get; set; }

        /// <summary>
        /// EsAppId
        /// </summary>
        [DataMember]
        public Guid EsAppId { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        [DataMember]
        public Guid OrderId { get; set; }

        /// <summary>
        /// AppId
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }

        /// <summary>
        /// 金币付款金额
        /// </summary>
        [DataMember]
        public decimal GoldPrice { get; set; }

        /// <summary>
        /// 代金券金额
        /// </summary>
        [DataMember]
        public decimal GoldCoupon { get; set; }

        /// <summary>
        /// 代金券ID串
        /// </summary>
        [DataMember]
        public string GoldCouponIds { get; set; }

        /// <summary>
        /// Source
        /// </summary>
        [DataMember]
        public string Source { get; set; }

        /// <summary>
        /// WxOpenId
        /// </summary>
        [DataMember]
        public string WxOpenId { get; set; }

        /// <summary>
        /// 未支付超时时间
        /// </summary>
        [DataMember]
        public DateTime? ExpireTime { get; set; }

        /// <summary>
        /// 0：担保；1：直接到账
        /// </summary>
        [DataMember]
        public int TradeType { get; set; }

        /// <summary>
        /// 拼团Id
        /// </summary>
        [DataMember]
        public Guid? DiyGroupId { get; set; }

        /// <summary>
        /// 分享Id √
        /// </summary>
        [DataMemberAttribute()]
        public string ShareId { get; set; }

        /// <summary>
        /// 区分获取支付地址的前提  0:下订单；1 继续支付
        /// </summary>
        [DataMemberAttribute()]
        public int DealType { get; set; }

        /// <summary>
        /// 是否微信支付
        /// </summary>
        [DataMember]
        public bool IsWeixinPay { get; set; }

        /// <summary>
        /// 订单类型
        /// </summary>
        [DataMember]
        public int OrderType { get; set; }

        /// <summary>
        /// 请求协议
        /// </summary>
        [DataMember]
        public string Scheme { get; set; }

        /// <summary>
        /// 金采活动Id
        /// </summary>
        [DataMember]
        public Guid? JcActivityId { get; set; }
    }
}
