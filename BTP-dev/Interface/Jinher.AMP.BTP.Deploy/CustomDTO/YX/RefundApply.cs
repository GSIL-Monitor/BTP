using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.YX
{
    /// <summary>
    /// 售后申请返回信息
    /// </summary>
    public class RefundApply
    {
        /// <summary>
        /// 申请单Id	
        /// </summary>
        public string applyId { get; set; }

        /// <summary>
        /// 订单号	
        /// </summary>
        public string orderId { get; set; }

        /// <summary>
        /// 申请售后的sku信息
        /// </summary>
        public List<ApplySku> applySkuList { get; set; }

        /// <summary>
        /// 退货类型(0	待定|1	无理由，需用户支付邮费|2	质量问题，严选支付邮费，发顺丰到付--可选，不一定有该项)
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
        public long? createTime { get; set; }

        /// <summary>
        /// 更新时间(单位毫秒，可选，不一定有该项)
        /// </summary>
        public long? updateTime { get; set; }
    }
}
