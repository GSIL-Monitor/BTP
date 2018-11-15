using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable()]
    [DataContract]
    public class OrderRefundDTO
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
        public int IsFullRefund { get; set; }
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
        /// SubTime。
        /// </summary>
        [DataMemberAttribute()]
        public string strSubTime { get; set; }
        /// <summary>
        /// ModifiedOn。
        /// </summary>
        [DataMemberAttribute()]
        public DateTime ModifiedOn { get; set; }
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
        /// 买家是否延迟收货时间，1:延长，0：未延长
        /// </summary>
        [DataMemberAttribute()]
        public bool? IsDelayConfirmTimeAfterSales { get; set; }

        /// <summary>
        /// 0:出库中达成协议，1：已发货达成协议
        /// </summary>
        [DataMemberAttribute()]
        public int? AgreeFlag { get; set; }

        /// <summary>
        /// 买家是否延迟收货时间，1:延长，0：未延长
        /// </summary>
        [DataMemberAttribute()]
        public decimal? RefundFreightPrice { get; set; }

        /// <summary>
        /// 退还积分抵现金额
        /// </summary>
        [DataMemberAttribute()]
        public decimal RefundScoreMoney { get; set; }

        /// <summary>
        /// 商家备注
        /// </summary>
        [DataMemberAttribute()]
        public string SalerRemark { get; set; }

        /// <summary>
        /// 退还易捷币抵现金额
        /// </summary>
        [DataMemberAttribute()]
        public decimal RefundYJBMoney { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMemberAttribute()]
        public string Name { get; set; }

        /// <summary>
        /// 商品图片
        /// </summary>
        [DataMemberAttribute()]
        public string Pic { get; set; }

        /// <summary>
        /// 商品单价
        /// </summary>
        [DataMemberAttribute()]
        public decimal Price { get; set; }

        /// <summary>
        /// 商品数量    
        /// </summary>
        [DataMemberAttribute()]
        public int Num { get; set; }

        /// <summary>
        /// 是否为京东订单    
        /// </summary>
        [DataMember]
        public bool IsJdOrder { get; set; }

        /// <summary>
        /// 进销存京东售后服务单id
        /// </summary>
        [DataMemberAttribute()]
        public Guid? JDEclpOrderRefundAfterSalesId { get; set; }

        /// <summary>
        /// 是否进销存京东订单
        /// </summary>
        [DataMemberAttribute]
        public bool IsJdEclpOrder { get; set; }

        /// <summary>
        /// 取件类型：1、上门取件；2、客户发货
        /// </summary>
        [DataMemberAttribute]
        public int? PickwareType { get; set; }

        /// <summary>
        /// 取件服务费
        /// </summary>
        [DataMemberAttribute]
        public decimal PickUpFreightMoney { get; set; }

        /// <summary>
        /// 上门取件地址信息
        /// </summary>
        [DataMemberAttribute]
        public AddressInfo Address { get; set; }

        /// <summary>
        /// 进销存京东售后服务单信息
        /// </summary>
        [DataMemberAttribute()]
        public JDEclpOrderRefundAfterSalesExtraDTO JdEclpServiceInfo { get; set; }

        /// <summary>
        /// 残品寄回运费
        /// </summary>
        [DataMemberAttribute]
        public decimal SendBackFreightMoney { get; set; }

        #endregion
    }

    /// <summary>
    /// 进销存京东售后服务单扩展
    /// </summary>
    [Serializable()]
    [DataContract]
    public class JDEclpOrderRefundAfterSalesExtraDTO : JDEclpOrderRefundAfterSalesDTO
    {
        /// <summary>
        /// 进销存-京东售后服务单商品验收信息
        /// </summary>
        [DataMemberAttribute]
        public List<JDEclpOrderRefundAfterSalesItemDTO> ItemList;
    }
}
