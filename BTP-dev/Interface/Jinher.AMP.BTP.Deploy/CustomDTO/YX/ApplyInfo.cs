using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.YX
{
    /// <summary>
    /// 售后申请请求信息
    /// </summary>
    public class ApplyInfo
    {
        /// <summary>
        /// 售后申请Id	
        /// </summary>
        public string requestId { get; set; }

        /// <summary>
        /// 订单号	
        /// </summary>
        public string orderId { get; set; }

        /// <summary>
        /// 售后申请人	
        /// </summary>
        public ApplyUser applyUser { get; set; }

        /// <summary>
        /// 申请售后的sku信息	
        /// </summary>
        public ApplySku applySku { get; set; }
    }

    /// <summary>
    /// 售后申请人
    /// </summary>
    public class ApplyUser
    {
        /// <summary>
        /// 退货联系人
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 退货联系人电话
        /// </summary>
        public string mobile { get; set; }
    }

    /// <summary>
    /// 申请售后的sku信息
    /// </summary>
    public class ApplySku
    {
        /// <summary>
        /// sku发货时得到的包裹号
        /// </summary>
        public string packageId { get; set; }

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
    }

    /// <summary>
    /// 退换货原因
    /// </summary>
    public class ApplySkuReason
    {
        /// <summary>
        /// 退换货原因(指定文案：无理由、质量问题，最大128位)
        /// </summary>
        public string reason { get; set; }

        /// <summary>
        /// 退换货详细原因(最大512位)
        /// </summary>
        public string reasonDesc { get; set; }
    }

    /// <summary>
    /// 申请售后的图片信息，严选会做转存处理，不支持链接内容的动态更新
    /// </summary>
    public class ApplyPic
    {
        /// <summary>
        /// 图片url(必须支持GET请求获取图片,最长255位)
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 图片文件名(必须带文件类型后缀)
        /// </summary>
        public string fileName { get; set; }
    }
}
