using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.ExportDTO
{
    /// <summary>
    /// 保险汇款单导出实体
    /// </summary>
    public class YJBCarInsuranceRebateDTO
    {
        public string RebateDate { get; set; }
        public string RebateNum { get; set; }
        public string AfterTaxMoney { get; set; }
        public string RebateMoney { get; set; }
        public string RemittanceNo { get; set; }
        public string InsuranceCompanyName { get; set; }
        public string CompanyRemittanceMoney { get; set; }
        public string DouRebatePersent { get; set; }
        public string CompanyRebatePersent { get; set; }
        public string DouRebateMoney { get; set; }
        public string CompanyRebateMoney { get; set; }
        public string BusinessInsuranceAmount { get; set; }
        public string StrongInsuranceAmount { get; set; }
        public string CarShipAmount { get; set; }
        public string InsuranceAmount { get; set; }

    }

    /// <summary>
    /// 保险汇款单详情导出实体
    /// </summary>
    public class YJBCarInsuranceRebateDetailDTO
    {
        public string RebateDate { get; set; }
        public string AuditDate { get; set; }
        public string OrderNo { get; set; }
        public string RebateNum { get; set; }
        public string InsuranceAmount { get; set; }
        public string RebateMoney { get; set; }
        public string PhoneNum { get; set; }
        public string RemittanceNo { get; set; }
        public string RebateState { get; set; }
        public string DouRemittanceAmount { get; set; }

        public string InsuranceCompanyName { get; set; }
        public string CompanyRemittanceMoney { get; set; }
        public string DouRebatePersent { get; set; }
        public string CompanyRebatePersent { get; set; }
        public string DouRebateMoney { get; set; }
        public string CompanyRebateMoney { get; set; }
        public string BusinessInsuranceAmount { get; set; }
        public string StrongInsuranceAmount { get; set; }
        public string CarShipAmount { get; set; }
    }
    /// <summary>
    /// 保险统计报表导出Excel实体
    /// </summary>
    public class YJBCarInsuranceReportExportDTO
    {

        public string State { get; set; }
        public string InsuranceCompanyName { get; set; }
        public string CompanyRebateMoney { get; set; }
        public string CustomAmount { get; set; }
        public string InsuranceRebateMoney { get; set; }
        public string OrderNo { get; set; }
        public string MemberPhone { get; set; }
        public string CustomPhone { get; set; }
        public string InsuranceAmount { get; set; }
        public string StrongInsuranceAmount { get; set; }
        public string BusinessAmount { get; set; }
        public string StrongInsuranceOrderId { get; set; }
        public string StrongInsuranceStartTime { get; set; }
        public string BusinessOrderId { get; set; }
        public string BusinessStartTime { get; set; }
        public string PlateNumber { get; set; }
        public string ChassisNumber { get; set; }
        public string EngineNumber { get; set; }
        public string CarTypeName { get; set; }
        public string RegisterTime { get; set; }
        public string IsTransfer { get; set; }
        public string CarOwnerName { get; set; }
        public string CarOwnerIdType { get; set; }
        public string CarOwnerId { get; set; }
        public string CarOwnerPhone { get; set; }
        public string CarOwnerAddress { get; set; }
        public string PolicyHolderName { get; set; }
        public string PolicyHolderIdType { get; set; }
        public string PolicyHolderId { get; set; }
        public string PolicyHolderPhone { get; set; }
        public string PolicyHolderAddress { get; set; }
        public string InsuranceTime { get; set; }
        public string RecommendName { get; set; }
        public string RecommendAmount { get; set; }
        public string SinopecAmount { get; set; }
        public string RebateState { get; set; }
        public string StrongInsurance_SI { get; set; }
        public string StrongInsurance_Car { get; set; }
        public string Business_Car { get; set; }
        public string Business_Three { get; set; }
        public string Business_Driver { get; set; }
        public string Business_Passenger { get; set; }
        public string Business_AllCar { get; set; }
        public string Business_Glass { get; set; }
        public string Business_Body { get; set; }
        public string Business_Engine { get; set; }
        public string Business_Natural { get; set; }
        public string Business_Garage { get; set; }
        public string Business_Third { get; set; }
        public string Business_Spirit { get; set; }
        public string NoDeductibles_Car { get; set; }
        public string NoDeductibles_Three { get; set; }
        public string NoDeductibles_Driver { get; set; }
        public string NoDeductibles_Passenger { get; set; }
        public string NoDeductibles_AllCar { get; set; }
        public string NoDeductibles_Body { get; set; }
        public string NoDeductibles_Engine { get; set; }
        public string NoDeductibles_Natural { get; set; }
        public string NoDeductibles__Spirit { get; set; }
    }
}
