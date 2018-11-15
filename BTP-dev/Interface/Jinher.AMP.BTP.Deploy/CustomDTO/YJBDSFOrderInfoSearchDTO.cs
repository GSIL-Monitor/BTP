using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    public class YJBDSFOrderInfoSearchDTO : SearchBase
    {
        /// <summary>
        /// 订单号
        /// </summary>
        [DataMember]
        public String OrderNo { get; set; }
        /// <summary>
        /// 平台名称
        /// </summary>
        [DataMember]
        public string PlatformName { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        [DataMember]
        public string OrderPayState { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        [DataMember]
        public string StartDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        [DataMember]
        public string EndDate { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        [DataMember]
        public Guid UserID { get; set; }
    }
}
