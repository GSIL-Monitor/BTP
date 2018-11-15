using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jinher.AMP.YJB.Deploy.Enums;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    public class YJBCarInsuranceRebateSearchDTO : SearchBase
    {
        /// <summary>
        /// 订单号
        /// </summary>
        [DataMember]
        public String OrderNo { get; set; }
        /// <summary>
        /// 返利开始日期
        /// </summary>
        [DataMember]
        public string StartRebateDate { get; set; }
        /// <summary>
        /// 返利结束日期
        /// </summary>
        [DataMember]
        public string EndRebateDate { get; set; }
        /// <summary>
        /// 汇款单号
        /// </summary>
        [DataMember]
        public string RemittanceNo { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        [DataMember]
        public string PhoneNum { get; set; }
        /// <summary>
        /// 车险返利状态
        /// </summary>
        [DataMember]
        public CarInsuranceStateEnum RebateState { get; set; }
    }
}
