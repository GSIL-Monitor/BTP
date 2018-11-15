using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jinher.AMP.YJB.Deploy.Enums;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable()]
    [DataContract]
    public class YJBCarInsuranceRebateDTO
    {
        /// <summary>
        /// 返利日期
        /// </summary>
        [DataMember]
        public string RebateDate { get; set; }
        
        /// <summary>
        /// 汇款单号
        /// </summary>
        [DataMember]
        public string RemittanceNo { get; set; }
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
        /// 汇款单的订单列表
        /// </summary>
        [DataMember]
        public List<OrderInfoList> OrderInfoList { get; set; }
    }

    [Serializable()]
    [DataContract]
    public class OrderInfoList
    {
        /// <summary>
        /// 订单号
        /// </summary>
        [DataMember]
        public String OrderNo { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        [DataMember]
        public string PhoneNum { get; set; }
        /// <summary>
        /// 返利金额
        /// </summary>
        [DataMember]
        public decimal RebateMoney { get; set; }
        /// <summary>
        /// 保险金额
        /// </summary>
        [DataMember]
        public decimal InsuranceAmount { get; set; }
        /// <summary>
        /// 笔数
        /// </summary>
        [DataMember]
        public int RebateNum { get; set; }
        /// <summary>
        /// 爱豆汇款金额
        /// </summary>
        [DataMember]
        public decimal DouRemittanceAmount { get; set; }
        /// <summary>
        /// 保险公司汇款金额
        /// </summary>
        [DataMember]
        public decimal CompanyRemittanceAmount { get; set; }
        /// <summary>
        /// 商险保单金额
        /// </summary>
        [DataMember]
        public decimal BusinessInsuranceAmount { get; set; }
        /// <summary>
        /// 交强险保单金额
        /// </summary>
        [DataMember]
        public decimal StrongInsuranceAmount { get; set; }
        /// <summary>
        /// 保险公司标识
        /// </summary>
        [DataMember]
        public string InsuranceCompanyCode { get; set; }
        /// <summary>
        /// 车船税
        /// </summary>
        [DataMember]
        public decimal CarShipAmount { get; set; }
    }


    [Serializable()]
    [DataContract]
    public class YJBCarInsuranceRebateDataDTO
    {
        /// <summary>
        /// jqGrid的tr的Id
        /// </summary>
        [DataMember]
        public string Id { get; set; }
        /// <summary>
        /// 返利日期
        /// </summary>
        [DataMember]
        public DateTime RebateDate { get; set; }

        /// <summary>
        /// 汇款单号
        /// </summary>
        [DataMember]
        public string RemittanceNo { get; set; }
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
        /// 订单号
        /// </summary>
        [DataMember]
        public String OrderNo { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        [DataMember]
        public string PhoneNum { get; set; }
        /// <summary>
        /// 返利金额
        /// </summary>
        [DataMember]
        public decimal RebateMoney { get; set; }
        /// <summary>
        /// 保险金额
        /// </summary>
        [DataMember]
        public decimal InsuranceAmount { get; set; }
        /// <summary>
        /// 笔数
        /// </summary>
        [DataMember]
        public int RebateNum { get; set; }

        /// <summary>
        /// 爱豆汇款金额
        /// </summary>
        [DataMember]
        public decimal DouRemittanceAmount { get; set; }
        /// <summary>
        /// 爱豆返利金额
        /// </summary>
        [DataMember]
        public decimal? DouRebateMoney { get; set; }
        /// <summary>
        /// 爱豆返利比例
        /// </summary>
        [DataMember]
        public decimal? DouRebatePersent { get; set; }
        /// <summary>
        /// 保险公司汇款金额
        /// </summary>
        [DataMember]
        public decimal? CompanyRemittanceMoney { get; set; }
        /// <summary>
        /// 保险公司返利金额
        /// </summary>
        [DataMember]
        public decimal? CompanyRebateMoney { get; set; }
        /// <summary>
        /// 保险公司返利比例
        /// </summary>
        [DataMember]
        public decimal? CompanyRebatePersent { get; set; }
        /// <summary>
        /// 保险公司编码
        /// </summary>
        [DataMember]
        public string InsuranceCompanyCode { get; set; }
        /// <summary>
        /// 保险公司名称
        /// </summary>
        [DataMember]
        public string InsuranceCompanyName { get; set; }
        /// <summary>
        /// 商业险保单金额
        /// </summary>
        [DataMember]
        public decimal? BusinessInsuranceAmount { get; set; }
        /// <summary>
        /// 交强险保单金额
        /// </summary>
        [DataMember]
        public decimal? StrongInsuranceAmount { get; set; }
        /// <summary>
        /// 车船险保单金额
        /// </summary>
        [DataMember]
        public decimal? CarShipAmount { get; set; }
        /// <summary>
        /// 返利是否正确
        /// </summary>
        [DataMember]
        public int? IsCorrect { get; set; }
    }
}
