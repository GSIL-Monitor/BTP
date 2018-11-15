using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.YX
{
    /// <summary>
    /// 售后申请返回信息
    /// </summary>
    public class OrderRefundApplyResponse
    {
        /// <summary>
        /// 售后申请Id	
        /// </summary>
        public string applyId { get; set; }

        /// <summary>
        /// 订单号	
        /// </summary>
        public string orderId { get; set; }

        /// <summary>
        /// 申请售后的sku信息
        /// </summary>
        public List<OrderRefundApplySkuResponse> applySkuList { get; set; }

        /// <summary>
        /// 退货类型(0	待定|1	无理由，需用户支付邮费|2	质量问题，严选支付邮费，发顺丰到付 --可选，不一定有该项)
        /// </summary>
        public int? returnType { get; set; }

        /// <summary>
        /// 申请单状态(可选，不一定有该项)
        /// </summary>
        public OrderRefundApplyResponseStatusEnum status { get; set; }

        /// <summary>
        /// 退货物流信息(可选，不一定有该项)
        /// </summary>
        public List<ExpressInfo> expressInfoList { get; set; }

        /// <summary>
        /// 客服审核不通过原因(可选，不一定有该项)
        /// </summary>
        public string denyReason { get; set; }

        /// <summary>
        /// 创建时间(单位毫秒，可选，不一定有该项)
        /// </summary>
        public DateTime? createTime { get; set; }

        /// <summary>
        /// 更新时间(单位毫秒，可选，不一定有该项)
        /// </summary>
        public DateTime? updateTime { get; set; }
    }

    /// <summary>
    /// 申请售后的sku信息
    /// </summary>
    public class OrderRefundApplySkuResponse
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
        /// 申请售后的图片信息(可选，不一定有该项)
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
        /// 此sku的售后信息(可选，不一定有该项)
        /// </summary>
        public List<OperateSku> operateSkus { get; set; }
    }

    /// <summary>
    /// 此sku的售后信息
    /// </summary>
    public class OperateSku
    {
        /// <summary>
        /// skuId
        /// </summary>
        public string skuId { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public OrderRefundApplySkuOperateStatusEnum status { get; set; }

        /// <summary>
        /// 审核不通过原因
        /// </summary>
        public string reason { get; set; }

        /// <summary>
        /// 异常类型
        /// </summary>
        public OrderRefundApplySkuOperateExceptionTypeEnum exceptionType { get; set; }

        /// <summary>
        /// 商品质检信息(组合装对应多个质检信息)
        /// </summary>
        public List<OrderRefundApplySkuQuality> qualityList { get; set; }
    }

    /// <summary>
    /// 商品质检信息
    /// </summary>
    public class OrderRefundApplySkuQuality
    {
        /// <summary>
        /// skuId
        /// </summary>
        public string skuId { get; set; }

        /// <summary>
        /// 次品类型
        /// </summary>
        public string defectiveType { get; set; }

        /// <summary>
        /// 处理方式
        /// </summary>
        public OrderRefundApplySkuQualityTreatmentMethodEnum treatmentMethod { get; set; }

        /// <summary>
        /// 权责问题	
        /// </summary>
        public string responsibilityDesc { get; set; }

        /// <summary>
        /// 图片(可选，不一定有该项)
        /// </summary>
        public List<string> picList { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public string specDesc { get; set; }
    }

    /// <summary>
    /// 退货地址详情信息
    /// </summary>
    public class OrderRefundApplySkuAddress
    {
        /// <summary>
        /// 省份名称	
        /// </summary>
        public string provinceName { get; set; }

        /// <summary>
        /// 城市名称	
        /// </summary>
        public string cityName { get; set; }

        /// <summary>
        /// 区域名称	
        /// </summary>
        public string districtName { get; set; }

        /// <summary>
        /// 具体街道地址	
        /// </summary>
        public string address { get; set; }

        /// <summary>
        /// 完整地址	
        /// </summary>
        public string fullAddress { get; set; }

        /// <summary>
        /// 邮政编码	
        /// </summary>
        public string zipCode { get; set; }

        /// <summary>
        /// 收件人姓名	
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 收件人手机	
        /// </summary>
        public string mobile { get; set; }
    }

    /// <summary>
    /// 退货物流信息
    /// </summary>
    public class ExpressInfo
    {
        /// <summary>
        /// 物流公司		
        /// </summary>
        public string trackingCompany { get; set; }

        /// <summary>
        /// 物流单号		
        /// </summary>
        public string trackingNum { get; set; }
    }
}
