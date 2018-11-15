using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.AfterSales
{
    /// <summary>
    /// 订单详情
    /// </summary>
    [Serializable]
    [DataContract]
    public class SNAfterOrderDetailDTO
    {
        /// <summary>
        /// 下单企业账号的用户名
        /// </summary>
        [DataMember]
        public string AccountName { get; set; }
        /// <summary>
        /// 采购账号的企业名称
        /// </summary>
        [DataMember]
        public string CompanyName { get; set; }
        /// <summary>
        /// 订单创建时间（yyyy-MM-dd HH:mm:ss）
        /// </summary>
        [DataMember]
        public string CreateTime { get; set; }
        /// <summary>
        /// 订单总价
        /// </summary>
        [DataMember]
        public string OrderAmt { get; set; }
        /// <summary>
        /// 苏宁订单号（唯一）
        /// </summary>
        [DataMember]
        public string OrderId { get; set; }
        /// <summary>
        /// 订单子集集合
        /// </summary>
        [DataMember]
        public List<SNAfterOrderDetailListDTO> OrderItemList { get; set; }
        /// <summary>
        /// 	订单收货地址
        /// </summary>
        [DataMember]
        public string ReceiverAddress { get; set; }
        /// <summary>
        /// 收货人联系电话
        /// </summary>
        [DataMember]
        public string ReceiverTel { get; set; }
    }

}
