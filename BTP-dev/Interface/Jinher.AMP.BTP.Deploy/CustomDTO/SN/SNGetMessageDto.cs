using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.SN
{
    public class SNGetMessageDto
    {
        /// <summary>
        /// 商品编码
        /// </summary>
        public string cmmdtyCode { get; set; }
        /// <summary>
        /// 消息id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 订单行号
        /// </summary>
        public string orderItemNo { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string orderNo { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 消息类型
        /// 10-商品池 上架、下架 
        /// 11-订单 实时创建、预占成功、确认预占、取消预占、异常订单取消　 
        /// 12-物流 商品出库、商品妥投、商品拒收、商品退货 
        /// 13-目录 添加、修改、删除录
        /// </summary>
        public string type { get; set; }
    }
}
