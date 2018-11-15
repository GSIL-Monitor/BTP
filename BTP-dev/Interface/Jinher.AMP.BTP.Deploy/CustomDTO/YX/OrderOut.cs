using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.YX
{
    /// <summary>
    /// 严选系统中订单信息
    /// </summary>
    public class OrderOut : OrderVO
    {
        /// <summary>
        /// 订单状态(PAYED)
        /// </summary>
        public OrderStatusEnum orderStatus { get; set; }

        /// <summary>
        /// 订单包裹
        /// </summary>
        public List<OrderPackage> orderPackages { get; set; }
    }

    /// <summary>
    /// 订单状态
    /// </summary>
    [DataContract]
    public enum OrderStatusEnum
    {
        /// <summary>
        /// 已付款
        /// </summary>
        [Description("已付款")]
        PAYED,
        /// <summary>
        /// 客服取消
        /// </summary>
        [Description("客服取消")]
        KF_CANCEL,
        /// <summary>
        /// 取消待审核
        /// </summary>
        [Description("取消待审核")]
        CANCELLING
    }

    /// <summary>
    /// 订单包裹
    /// </summary>
    public class OrderPackage
    {
        /// <summary>
        /// 包裹号
        /// </summary>
        public string packageId { get; set; }

        /// <summary>
        /// 物流公司名称(绑单之后才有，多物流公司之间使用英文竖线|隔开，如顺丰|圆通，物流公司列表)
        /// </summary>
        public string expressCompany { get; set; }

        /// <summary>
        /// (运单号)绑单之后才有， 不同物流公司的运单号使用英文竖线|隔开，相同物流公司的运单号使用,隔开如 sf123,sf456|yt123,yt456
        /// </summary>
        public string expressNo { get; set; }

        /// <summary>
        /// 发货时间(绑单之后才有，2016-05-23 13:59:59)
        /// </summary>
        public string expCreateTime { get; set; }

        /// <summary>
        /// 签收时间(确认收货之后才有，2016-05-24 13:59:59)
        /// </summary>
        public string confirmTime { get; set; }

        /// <summary>
        /// 包裹状态
        /// </summary>
        public PackageStatus packageStatus { get; set; }

        /// <summary>
        /// 订单SKU
        /// </summary>
        public List<OrderSkuVO> orderSkus { get; set; }

        /// <summary>
        /// 物流详细信息(绑单之后才有)
        /// </summary>
        public List<ExpressDetailInfo> expressDetailInfos { get; set; }

    }

    /// <summary>
    /// 包裹状态
    /// </summary>
    [DataContract]
    public enum PackageStatus
    {
        /// <summary>
        /// 等待发货
        /// </summary>
        [Description("等待发货")]
        WAITING_DELIVERY,
        /// <summary>
        /// 已发货(等待收货)
        /// </summary>
        [Description("已发货(等待收货)")]
        START_DELIVERY,
        /// <summary>
        /// 已收货(等待评论)
        /// </summary>
        [Description("已收货(等待评论)")]
        WAITING_COMMENT
    }

    /// <summary>
    /// 渠道订单回调的包裹详情信息
    /// </summary>
    public class ExpressDetailInfo
    {
        /// <summary>
        /// 物流公司名称
        /// </summary>
        public string expressCompany { get; set; }

        /// <summary>
        /// 物流单号
        /// </summary>
        public string expressNo { get; set; }

        /// <summary>
        /// 子物流单列表
        /// </summary>
        public List<string> subExpressNos { get; set; }

        /// <summary>
        /// 物流包裹包含SKU信息
        /// </summary>
        public List<OrderSkuVO> skus { get; set; }
    }
}
