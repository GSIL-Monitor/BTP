using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable]
    [DataContract]
    public class CancelTheOrderDTO
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid OrderId { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [DataMemberAttribute()]
        public int State { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        [DataMemberAttribute()]
        public string Message { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid UserId { get; set; }
        /// <summary>
        /// 拒绝原因
        /// </summary>
        [DataMemberAttribute()]
        public string RefuseReason { get; set; }

        /// <summary>
        /// 订单项id   
        /// </summary>
        [DataMemberAttribute()]
        public Guid OrderItemId { get; set; }

        /// <summary>
        /// 上门取件运费
        /// </summary>
        public decimal PickUpFreightMoney { get; set; }

        /// <summary>
        /// 残品寄回运费
        /// </summary>
        public decimal SendBackFreightMoney { get; set; }
    }
}
