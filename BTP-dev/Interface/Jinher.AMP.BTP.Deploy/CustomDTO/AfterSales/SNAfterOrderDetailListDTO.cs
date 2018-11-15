using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.AfterSales
{
    /// <summary>
    /// 订单子集
    /// </summary>
    [Serializable]
    [DataContract]
    public class SNAfterOrderDetailListDTO
    {
        /// <summary>
        /// 商品的品牌名称
        /// </summary>
        [DataMember]
        public string BrandName { get; set; }
        /// <summary>
        /// 商品编码
        /// </summary>
        [DataMember]
        public string CommdtyCode { get; set; }
        /// <summary>
        /// 	商品名称
        /// </summary>
        [DataMember]
        public string CommdtyName { get; set; }
        /// <summary>
        /// 希望送达时间（yyyy-MM-dd HH:mm:ss）
        /// </summary>
        [DataMember]
        public string HopeArriveTime { get; set; }
        /// <summary>
        /// 苏宁订单行号
        /// </summary>
        [DataMember]
        public string OrderItemId { get; set; }
        /// <summary>
        /// 商品总金额=商品数量*商品单价（含运费分摊）
        /// </summary>
        [DataMember]
        public string SkuAmt { get; set; }
        /// <summary>
        /// 商品的购买数量
        /// </summary>
        [DataMember]
        public string SkuNum { get; set; }
        /// <summary>
        /// 	商品单价
        /// </summary>
        [DataMember]
        public string UnitPrice { get; set; }
    }
}
