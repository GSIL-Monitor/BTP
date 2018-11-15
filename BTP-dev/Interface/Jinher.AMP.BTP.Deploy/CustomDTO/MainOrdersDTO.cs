using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;


namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 返回结果 --- 厂家直销
    /// </summary>
    [Serializable]
    [DataContract]
    public class MainOrdersDTO
    {
        /// <summary>
        /// 付款人id
        /// </summary>
        [DataMemberAttribute]
        public Guid UserId { get; set; }
        /// <summary>
        /// 应用id
        /// </summary>
        [DataMemberAttribute]
        public Guid AppId { get; set; }
        /// <summary>
        /// 订单id
        /// </summary>
        [DataMemberAttribute]
        public Guid OrderId { get; set; }
        /// <summary>
        /// 实际价格
        /// </summary>
        [DataMemberAttribute]
        public decimal RealPrice { get; set; }

    }
}
