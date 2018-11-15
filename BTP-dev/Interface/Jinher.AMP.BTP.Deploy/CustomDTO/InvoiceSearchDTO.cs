using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 发票信息查询类
    /// </summary>
    [Serializable]
    [DataContract]
    public class InvoiceSearchDTO : SearchBase
    {
        /// <summary>
        /// 发票类型
        /// </summary>
        [DataMember]
        public Int32 Category { get; set; }
        /// <summary>
        /// 发票状态：0:待付款，1:待开票,2:已开票,3:已发出,4:已作废
        /// </summary>
        [DataMember]
        public Int32 State { get; set; }
        /// <summary>
        /// AppId
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }
        /// <summary>
        /// Appid列表
        /// </summary>
        [DataMember]
        public List<Guid> AppIds { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        [DataMember]
        public Guid UserId { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        [DataMember]
        public string CommodityOrderState { get; set; }

        /// <summary>
        /// 查询内容
        /// </summary>
        [DataMember]
        public string SeacrhContent { get; set; }


    }
}
