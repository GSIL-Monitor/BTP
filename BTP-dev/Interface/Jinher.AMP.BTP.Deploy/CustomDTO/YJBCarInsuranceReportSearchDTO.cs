using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jinher.AMP.YJB.Deploy.Enums;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    public class YJBCarInsuranceReportSearchDTO : SearchBase
    {
        /// <summary>
        /// 订单号
        /// </summary>
        [DataMember]
        public string OrderNo { get; set; }
        /// <summary>
        /// 会员手机号
        /// </summary>
        [DataMember]
        public string MemberPhone { get; set; }
        /// <summary>
        /// 客户手机号
        /// </summary>
        [DataMember]
        public string CustomPhone { get; set; }
        /// <summary>
        /// 保单开始时间
        /// </summary>
        [DataMember]
        public string StartInsuranceTime { get; set; }
        /// <summary>
        /// 保单结束时间
        /// </summary>
        [DataMember]
        public string EndInsuranceTime { get; set; }
        /// <summary>
        /// 客户返利支付状态
        /// </summary>
        [DataMember]
        public CarInsuranceStateEnum RebateState { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        [DataMember]
        public DateTime? BeginTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        [DataMember]
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 保险公司名称
        /// </summary>
        [DataMember]
        public List<string> CompanyCode { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public string Status { get; set; }
    }

}
