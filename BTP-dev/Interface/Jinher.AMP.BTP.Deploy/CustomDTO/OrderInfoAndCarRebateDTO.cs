using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jinher.AMP.YJB.Deploy.Enums;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 第三方订单表和返利表联合数据表
    /// </summary>
    public class OrderInfoAndCarRebateDTO
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
        /// 订单日期
        /// </summary>
        [DataMember]
        public DateTime OrderPayDate { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        [DataMember]
        public decimal OrderPayMoney { get; set; }
        /// <summary>
        /// 商品列表
        /// </summary>
        [DataMember]
        public string Commodity { get; set; }

        /// <summary>
        /// 返利日期
        /// </summary>
        [DataMember]
        public DateTime RebateDate { get; set; }
        /// <summary>
        /// 笔数
        /// </summary>
        [DataMember]
        public int RebateNum { get; set; }
        /// <summary>
        /// 返利金额
        /// </summary>
        [DataMember]
        public decimal RebateMoney { get; set; }
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
        /// <summary>
        /// 审核日期
        /// </summary>
        [DataMember]
        public DateTime AuditDate { get; set; }
        /// <summary>
        /// 保险金额
        /// </summary>
        [DataMember]
        public decimal InsuranceAmount { get; set; }

    }
}
