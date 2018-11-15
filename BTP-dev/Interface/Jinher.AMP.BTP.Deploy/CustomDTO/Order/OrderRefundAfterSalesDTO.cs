using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable()]
    [DataContract]
    public class OrderRefundAfterSalesDTO
    {
        #region 简单类型属性

        /// <summary>
        /// ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid Id { get; set; }
        /// <summary>
        /// 退款类型：仅退款=0，退货退款=1
        /// </summary>
        [DataMemberAttribute()]
        public int RefundType { get; set; }
        /// <summary>
        /// 退款原因
        /// </summary>
        [DataMemberAttribute()]
        public string RefundReason { get; set; }
        /// <summary>
        /// 退款金额
        /// </summary>
        [DataMemberAttribute()]
        public decimal RefundMoney { get; set; }
        /// <summary>
        /// 退款详细说明
        /// </summary>
        [DataMemberAttribute()]
        public string RefundDesc { get; set; }
        /// <summary>
        /// 订单ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid OrderId { get; set; }
        /// <summary>
        /// 状态：退款中=0，已退款=1，已拒绝=2，已撤销=3，售后退款中商家同意退款，商家未收到货=10 ,金和处理退款中=12,买家发货超时，商家未收到货=13
        /// </summary>
        [DataMemberAttribute()]
        public int State { get; set; }
        /// <summary>
        /// 收款人账号
        /// </summary>
        [DataMemberAttribute()]
        public string ReceiverAccount { get; set; }
        /// <summary>
        /// 收款人姓名
        /// </summary>
        [DataMemberAttribute()]
        public string Receiver { get; set; }
        /// <summary>
        /// 退货物流公司
        /// </summary>
        [DataMemberAttribute()]
        public string RefundExpCo { get; set; }
        /// <summary>
        /// 退货物流单号
        /// </summary>
        [DataMemberAttribute()]
        public string RefundExpOrderNo { get; set; }
        /// <summary>
        /// 退款图片地址
        /// </summary>
        [DataMemberAttribute()]
        public string OrderRefundImgs { get; set; }
        /// <summary>
        /// 订单ID=0,商品ID=1
        /// </summary>
        [DataMemberAttribute()]
        public string DataType { get; set; }
        /// <summary>
        /// 订单项Id
        /// </summary>
        [DataMemberAttribute()]
        public Guid? OrderItemId { get; set; }
        /// <summary>
        /// 拒绝时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? RefuseTime { get; set; }
        /// <summary>
        /// IsFullRefund。
        /// </summary>
        [DataMemberAttribute()]
        public int? IsFullRefund { get; set; }
        /// <summary>
        /// 拒绝原因
        /// </summary>
        [DataMemberAttribute()]
        public string RefuseReason { get; set; }
        /// <summary>
        /// NotReceiveTime。
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? NotReceiveTime { get; set; }
        /// <summary>
        /// SubTime。
        /// </summary>
        [DataMemberAttribute()]
        public DateTime SubTime { get; set; }
        /// <summary>
        /// ModifiedOn。
        /// </summary>
        [DataMemberAttribute()]
        public DateTime ModifiedOn { get; set; }

        /// <summary>
        /// SubTime。
        /// </summary>
        [DataMemberAttribute()]
        public string strSubTime { get; set; }
        /// <summary>
        /// ModifiedOn。
        /// </summary>
        [DataMemberAttribute()]
        public string strModifiedOn { get; set; }
        /// <summary>
        /// 买家退货时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? RefundExpOrderTime { get; set; }

        /// <summary>
        /// 退还积分抵现金额
        /// </summary>
        [DataMemberAttribute()]
        public decimal RefundScoreMoney { get; set; }

        /// <summary>
        /// 易捷币退款金额
        /// </summary>
        [DataMemberAttribute()]
        public decimal RefundYJBMoney { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        public decimal? RefundFreightPrice { get; set; }

        /// <summary>
        /// 京东退款信息
        /// </summary>
        [DataMember]
        public JdOrderRefundDto JdOrderRefundInfo { get; set; }
        #endregion
    }
}
