using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.YX
{
    /// <summary>
    /// 退款结果信息
    /// </summary>
    public class RefundResult
    {
        /// <summary>
        /// 申请单Id
        /// </summary>
        public string applyId { get; set; }

        /// <summary>
        /// 订单Id
        /// </summary>
        public string orderId { get; set; }

        /// <summary>
        /// 退款的商品信息
        /// </summary>
        public List<RefundSku> refundSkuList { get; set; }
    }

    /// <summary>
    /// 申请售后的sku信息
    /// </summary>
    public class RefundSku
    {
        /// <summary>
        /// 申请售后的skuId
        /// </summary>
        public string skuId { get; set; }

        /// <summary>
        /// 申请售后的sku数量	
        /// </summary>
        public int count { get; set; }

        /// <summary>
        /// 退换货原因	
        /// </summary>
        public ApplySkuReason applySkuReason { get; set; }

        /// <summary>
        /// 申请售后的图片信息	
        /// </summary>
        public List<ApplyPic> applyPicList { get; set; }

        /// <summary>
        /// sku单价
        /// </summary>
        public decimal originalPrice { get; set; }

        /// <summary>
        /// 实付金额小计
        /// </summary>
        public decimal subtotalPrice { get; set; }

        /// <summary>
        /// 此sku的售后信息
        /// </summary>
        public List<OperateSku> operateSkus { get; set; }

        /// <summary>
        /// 退货地址
        /// </summary>
        public RefundResultSkuAddress returnStoreHouse { get; set; }
    }

    /// <summary>
    /// 退换货原因
    /// </summary>
    public class RefundResultSkuAddress
    {
        /// <summary>
        /// 退换货原因	
        /// </summary>
        public string reason { get; set; }

        /// <summary>
        /// 退换货详细原因
        /// </summary>
        public string reasonDesc { get; set; }
    }
}
