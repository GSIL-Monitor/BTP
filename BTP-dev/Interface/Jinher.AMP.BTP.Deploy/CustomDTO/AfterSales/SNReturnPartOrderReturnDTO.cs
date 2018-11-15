using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.AfterSales
{
    /// <summary>
    /// 部分退款返回
    /// </summary>
    [Serializable]
    [DataContract]
    public class SNReturnPartOrderReturnDTO
    {
        /// <summary>
        /// 订单Id
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 成功状态 	true-成功；false-失败
        /// </summary>
        public bool IsSuccess { get; set; }    
        /// <summary>
        /// 返回集合
        /// </summary>
        public List<SNReturnPartOrderReturnListDTO> InfoList { get; set; }
    }
    /// <summary>
    /// 整单退返回集合
    /// </summary>
    [Serializable]
    [DataContract]
    public class SNReturnPartOrderReturnListDTO
    {
        /// <summary>
        /// 商品编码
        /// </summary>
        [DataMember]
        public string SkuId { get; set; }
        /// <summary>
        /// 处理结果1：成功，0：失败
        /// </summary>
        [DataMember]
        public string Status { get; set; }
        /// <summary>
        /// 	单行描述   该退货申请提交成功，待客服人员处理
        /// </summary>
        [DataMember]
        public string UnableReason { get; set; }
    }
}
