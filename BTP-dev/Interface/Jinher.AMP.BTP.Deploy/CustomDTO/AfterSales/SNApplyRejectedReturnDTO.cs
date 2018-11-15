using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.AfterSales
{
    /// <summary>
    /// 整单退返回
    /// </summary>
    [Serializable]
    [DataContract]
    public class SNApplyRejectedReturnDTO
    {
        /// <summary>
        /// 订单Id
        /// </summary>
        [DataMember]
        public string OrderId { get; set; }
        /// <summary>
        /// 返回集合
        /// </summary>
        [DataMember]
        public List<SNApplyRejectedReturnListDTO> InfoList { get; set; }
    }
    /// <summary>
    /// 整单退返回集合
    /// </summary>
    [Serializable]
    [DataContract]
    public class SNApplyRejectedReturnListDTO
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
        /// 	不能取消的原因
        /// </summary>
        [DataMember]
        public string UnableReason { get; set; }
    }
}
