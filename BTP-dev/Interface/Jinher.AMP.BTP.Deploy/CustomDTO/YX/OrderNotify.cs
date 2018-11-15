using System.Collections.Generic;
using System;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.YX
{
    /// <summary>
    /// 订单包裹物流绑单回调
    /// </summary>
    public class OrderDelivered : YXSign
    {
        /// <summary>
        /// 订单SKU
        /// </summary>
        public List<OrderPackageNotify> orderPackage { get; set; }
    }

    /// <summary>
    /// 订单包裹物流绑单回调
    /// </summary>
    public class OrderPackageNotify
    {
        /// <summary>
        /// 订单号(最大128位)
        /// </summary>
        public Guid orderId { get; set; }

        /// <summary>
        /// 包裹号
        /// </summary>
        public string packageId { get; set; }

        /// <summary>
        /// 发货时间(unix时间戳，单位毫秒)
        /// </summary>
        public long expCreateTime { get; set; }

        /// <summary>
        /// 物流详细信息(绑单之后才有)
        /// </summary>
        public List<ExpressDetailInfo> expressDetailInfos { get; set; }
    }
}
