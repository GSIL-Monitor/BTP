using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.SN
{
    /// <summary>
    /// 物流变更信息
    /// </summary>
    public class SNExpressChangeDto
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// 订单行号
        /// </summary>
        public string OrderItemNo { get; set; }
        /// <summary>
        /// ---------
        /// </summary>
        public string CmmdtyCode { get; set; }
        /// <summary>
        /// 1.商品出库
        /// 2.商品妥投 
        /// 3.商品拒收
        /// 4.商品退货
        /// </summary>
        public string Status { get; set; }
    }

    /// <summary>
    /// 订单信息
    /// </summary>
    public class SNOrdersChangeDto
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// 1.实时创建成功 
        /// 2.预占成功
        /// 3.确认预占
        /// 4.取消预占
        /// 5.异常订单取消 
        /// </summary>
        public string Status { get; set; }
    }
}
