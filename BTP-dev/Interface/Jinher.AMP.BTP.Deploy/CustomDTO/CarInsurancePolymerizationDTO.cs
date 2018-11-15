using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [DataContract, Serializable]
    public class CarInsurancePolymerizationDTO
    {
        /// <summary>
        /// 表格tr的ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 汇款日期
        /// </summary>
        [DataMember]
        public DateTime? RebateDate { get; set; }
        /// <summary>
        /// 笔数
        /// </summary>
        [DataMember]
        public int? RebateNum { get; set; }
        /// <summary>
        /// 爱豆汇款总金额
        /// </summary>
        [DataMember]
        public decimal? AfterTaxMoney { get; set; }
        /// <summary>
        /// 返利总金额
        /// </summary>
        [DataMember]
        public decimal? RebateMoney { get; set; }
        /// <summary>
        /// 汇款单号
        /// </summary>
        [DataMember]
        public string RemittanceNo { get; set; }
        /// <summary>
        /// 审核标志
        /// </summary>
        [DataMember]
        public int AuditFlag { get; set; }
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
        /// 保单总金额
        /// </summary>
        [DataMember]
        public decimal? InsuranceAmount { get; set; }
        /// <summary>
        /// 返利是否正确
        /// </summary>
        [DataMember]
        public int? IsCorrect { get; set; }
    }
}
