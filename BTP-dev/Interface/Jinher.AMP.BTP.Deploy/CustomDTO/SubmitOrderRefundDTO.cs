using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jinher.AMP.BTP.Deploy.CustomDTO.AfterSales;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 退款
    /// </summary>
    [Serializable()]
    [DataContract]
    public class SubmitOrderRefundDTO
    {
        /// <summary>
        /// 退款ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid Id { get; set; }

        /// <summary>
        /// 退款原因
        /// </summary>
        [DataMemberAttribute()]
        public string RefundReason { get; set; }
        /// <summary>
        /// 退款金额(退款现金金额+退款抵用券金额)
        /// </summary>
        [DataMemberAttribute()]
        public decimal RefundMoney { get; set; }
        /// <summary>
        /// 退款抵用券金额
        /// </summary>
        [DataMember]
        public decimal? RefundCouponPirce { get; set; }
        /// <summary>
        /// 退款详细说明
        /// </summary>
        [DataMemberAttribute()]
        public string RefundDesc { get; set; }

        /// <summary>
        /// 退货上传凭证图片多个，以逗号隔开
        /// </summary>
        [DataMemberAttribute()]
        public string OrderRefundImgs { get; set; }
        /// <summary>
        /// 订单ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid commodityorderId { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        [DataMemberAttribute()]
        public int State { get; set; }

        /// <summary>
        /// 物流公司 
        /// </summary>
        [DataMemberAttribute()]
        public string RefundExpCo { get; set; }
        /// <summary>
        /// 物流单号
        /// </summary>
        [DataMemberAttribute()]
        public string RefundExpOrderNo { get; set; }
        /// <summary>
        /// 退款类型：仅退款=0，退货退款=1
        /// </summary>
        [DataMemberAttribute()]
        public int RefundType { get; set; }
        /// <summary>
        /// 拒绝原因
        /// </summary>
        [DataMemberAttribute()]
        public string RefuseReason { get; set; }
        /// <summary>
        /// 拒绝时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? RefuseTime { get; set; }
        /// <summary>
        /// 买家退货时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? RefundExpOrderTime { get; set; }
        /// <summary>
        /// 买家是否延迟收货时间，1:延长，0：未延长
        /// </summary>
        [DataMemberAttribute()]
        public bool IsDelayConfirmTimeAfterSales { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime SubTime { get; set; }

        /// <summary>
        /// 买家发货时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? NotReceiveTime { get; set; }

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
        /// 订单详情ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid OrderItemId { get; set; }

        /// <summary>
        /// 商品图片
        /// </summary>
        [DataMemberAttribute()]
        public string Pic { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMemberAttribute()]
        public string Name { get; set; }

        /// <summary>
        /// 商品属性
        /// </summary>
        [DataMemberAttribute()]
        public string CommodityAttributes { get; set; }

        /// <summary>
        /// 商品数量
        /// </summary>
        [DataMemberAttribute()]
        public int Num { get; set; }

        /// <summary>
        /// 商品价格
        /// </summary>
        [DataMemberAttribute()]
        public decimal Price { get; set; }

        /// <summary>
        /// 优惠券拆分
        /// </summary>
        [DataMemberAttribute()]
        public decimal CouponPrice { get; set; }

        /// <summary>
        /// 运费拆分
        /// </summary>
        [DataMemberAttribute()]
        public decimal FreightPrice { get; set; }

        /// <summary>
        /// 运费改价
        /// </summary>
        [DataMemberAttribute()]
        public decimal ChangeFreightPrice { get; set; }

        /// <summary>
        /// 商品改价
        /// </summary>
        [DataMemberAttribute()]
        public decimal ChangeRealPrice { get; set; }

        /// <summary>
        /// 订单项状态
        /// </summary>
        [DataMemberAttribute()]
        public int OrderItemState { get; set; }

        /// <summary>
        /// 上门取件地址信息
        /// </summary>
        [DataMember]
        public AddressInfo Address { get; set; }

        /// <summary>
        /// 京东退款信息
        /// </summary>
        [DataMember]
        public JdOrderRefundDto JdOrderRefundInfo { get; set; }

        /// <summary>
        /// 规格信息
        /// 是否进销存京东订单
        /// </summary>
        [DataMember]
        public bool IsJdEclpOrder { get; set; }

        /// <summary>
        /// 客户拒收后收取的运费
        /// </summary>
        [DataMember]
        public decimal RejectFreightMoney { get; set; }

        /// <summary>
        /// 取件类型：1、上门取件；2、客户发货
        /// </summary>
        [DataMember]
        public int? PickwareType { get; set; }

        /// <summary>
        /// 取件服务费
        /// </summary>
        [DataMember]
        public int? Specifications { get; set; }
        public decimal PickUpFreightMoney { get; set; }

        /// <summary>
        /// 是否为第三方电商（不允许手动同意/拒绝退款）
        /// </summary>
        public bool IsThirdECommerce { get; set; }

        /// <summary>
        /// 是否可以取消
        /// </summary>
        public bool CanCancel { get { return _canCancel; } set { _canCancel = value; } }
        private bool _canCancel = true;
        /// <summary>
        /// 是否是苏宁订单
        /// </summary>
        [DataMember]
        public bool IsSNOrder { get; set; }

        /// <summary>
        /// 是否是苏宁订单
        /// </summary>
        [DataMember]
        public bool IsJDOrder { get; set; }

        /// <summary>
        /// 苏宁退款信息
        /// </summary>
        [DataMember]
        public SNOrderRefundDto SnOrderRefundInfo { get; set; }
    }

    [Serializable]
    [DataContract]
    public class JdOrderRefundDto
    {
        public string ServiceId { get; set; }

        /// <summary>
        /// 是否可取消(0代表否，1代表是)
        /// </summary>
        [DataMember]
        public int Cancel { get; set; }

        /// <summary>
        /// 取件方式(必填 4 上门取件 7 客户送货 40客户发货)
        /// </summary>
        [DataMember]
        public int PickwareType { get; set; }

        [DataMember]
        public string CustomerContactName { get; set; }

        [DataMember]
        public string CustomerTel { get; set; }

        [DataMember]
        public string PickwareAddress { get; set; }

        [DataMember]
        public string PickwarePovinceCode { get; set; }

        [DataMember]
        public string PickwareCityCode { get; set; }

    }
}
