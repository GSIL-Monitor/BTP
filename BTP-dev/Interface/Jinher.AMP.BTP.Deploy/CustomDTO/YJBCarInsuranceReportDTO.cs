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
    public class YJBCarInsuranceReportDTO
    {
        /// <summary>
        /// 保险公司标识
        /// </summary>
        [DataMember]
        public string InsuranceCompanyCode { get; set; }
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
        /// 保单金额
        /// </summary>
        [DataMember]
        public decimal InsuranceAmount { get; set; }

        /// <summary>
        /// 保单时间
        /// </summary>
        [DataMember]
        public string InsuranceTime { get; set; }
        /// <summary>
        /// 保单状态（报价成功，报价失败，待核保，核保失败，核保成功，支付成功，已完成）
        /// </summary>
        [DataMember]
        public string State { get; set; }
        /// <summary>
        /// 推荐员工姓名
        /// </summary>
        [DataMember]
        public string RecommendName { get; set; }
        /// <summary>
        /// 员工奖励金额
        /// </summary>
        [DataMember]
        public string RecommendAmount { get; set; }
        /// <summary>
        /// 客户返利金额
        /// </summary>
        [DataMember]
        public decimal CustomAmount { get; set; }
        /// <summary>
        /// 石化佣金
        /// </summary>
        [DataMember]
        public decimal SinopecAmount { get; set; }
        /// <summary>
        /// 客户返利支付状态
        /// </summary>
        [DataMember]
        public CarInsuranceStateEnum RebateState { get; set; }
        [DataMember]
        public Guid SubId { get; set; }
        [DataMember]
        public DateTime SubTime { get; set; }
        [DataMember]
        public DateTime ModifiedOn { get; set; }
        /// <summary>
        /// 交强险金额
        /// </summary>
        [DataMember]
        public decimal StrongInsuranceAmount { get; set; }
        /// <summary>
        /// 商业险金额
        /// </summary>
        [DataMember]
        public decimal BusinessAmount { get; set; }
        /// <summary>
        /// 商业险不计免赔总金额
        /// </summary>
        [DataMember]
        public decimal BusinessFreeAmount { get; set; }
        /// <summary>
        /// 交强险单号
        /// </summary>
        [DataMember]
        public string StrongInsuranceOrderId { get; set; }
        /// <summary>
        /// 交强险开始日期
        /// </summary>
        [DataMember]
        public string StrongInsuranceStartTime { get; set; }
        /// <summary>
        /// 交强险结束日期
        /// </summary>
        [DataMember]
        public string StrongInsuranceEndTime { get; set; }
        /// <summary>
        /// 商业险单号
        /// </summary>
        [DataMember]
        public string BusinessOrderId { get; set; }
        /// <summary>
        /// 商业险开始日期
        /// </summary>
        [DataMember]
        public string BusinessStartTime { get; set; }
        /// <summary>
        /// 商业险结束日期
        /// </summary>
        [DataMember]
        public string BusinessEndTime { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        [DataMember]
        public string PlateNumber { get; set; }
        /// <summary>
        /// 车架号
        /// </summary>
        [DataMember]
        public string ChassisNumber { get; set; }
        /// <summary>
        /// 发动机号
        /// </summary>
        [DataMember]
        public string EngineNumber { get; set; }
        /// <summary>
        /// 车型名称
        /// </summary>
        [DataMember]
        public string CarTypeName { get; set; }
        /// <summary>
        /// 注册日期
        /// </summary>
        [DataMember]
        public string RegisterTime { get; set; }
        /// <summary>
        /// 是否过户车
        /// </summary>
        [DataMember]
        public string IsTransfer { get; set; }
        /// <summary>
        /// 车主姓名
        /// </summary>
        [DataMember]
        public string CarOwnerName { get; set; }
        /// <summary>
        /// 车主证件类型
        /// </summary>
        [DataMember]
        public string CarOwnerIdType { get; set; }
        /// <summary>
        /// 车主证件号
        /// </summary>
        [DataMember]
        public string CarOwnerId { get; set; }
        /// <summary>
        /// 车主地址
        /// </summary>
        [DataMember]
        public string CarOwnerAddress { get; set; }
        /// <summary>
        /// 车主手机号
        /// </summary>
        [DataMember]
        public string CarOwnerPhone { get; set; }
        /// <summary>
        /// 投保人姓名
        /// </summary>
        [DataMember]
        public string PolicyHolderName { get; set; }
        /// <summary>
        /// 投保人证件类型
        /// </summary>
        [DataMember]
        public string PolicyHolderIdType { get; set; }
        /// <summary>
        /// 投保人证件号
        /// </summary>
        [DataMember]
        public string PolicyHolderId { get; set; }
        /// <summary>
        /// 投保人手机号
        /// </summary>
        [DataMember]
        public string PolicyHolderPhone { get; set; }
        /// <summary>
        /// 投保人地址
        /// </summary>
        [DataMember]
        public string PolicyHolderAddress { get; set; }
        /// <summary>
        /// 交强险（交强险）
        /// </summary>
        [DataMember]
        public string StrongInsurance_SI { get; set; }
        /// <summary>
        /// 交强险（车船税）
        /// </summary>
        [DataMember]
        public string StrongInsurance_Car { get; set; }
        /// <summary>
        /// 商业险（车损险）
        /// </summary>
        [DataMember]
        public string Business_Car { get; set; }
        /// <summary>
        /// 商业险（三责险）
        /// </summary>
        [DataMember]
        public string Business_Three { get; set; }
        /// <summary>
        /// 商业险（司机责任险）
        /// </summary>
        [DataMember]
        public string Business_Driver { get; set; }
        /// <summary>
        /// 商业险（乘客责任险）
        /// </summary>
        [DataMember]
        public string Business_Passenger { get; set; }
        /// <summary>
        /// 商业险（全车盗抢险）
        /// </summary>
        [DataMember]
        public string Business_AllCar { get; set; }
        /// <summary>
        /// 商业险（玻璃破碎险）
        /// </summary>
        [DataMember]
        public string Business_Glass { get; set; }
        /// <summary>
        /// 商业险（车身划痕险）
        /// </summary>
        [DataMember]
        public string Business_Body { get; set; }
        /// <summary>
        /// 商业险（发动机损失险）
        /// </summary>
        [DataMember]
        public string Business_Engine { get; set; }
        /// <summary>
        /// 商业险（自然损失险）
        /// </summary>
        [DataMember]
        public string Business_Natural { get; set; }
        /// <summary>
        /// 商业险（准修厂特约）
        /// </summary>
        [DataMember]
        public string Business_Garage { get; set; }
        /// <summary>
        /// 商业险（第三方特约）
        /// </summary>
        [DataMember]
        public string Business_Third { get; set; }
        /// <summary>
        /// 商业险（精神损害险）
        /// </summary>
        [DataMember]
        public string Business_Spirit { get; set; }
        /// <summary>
        /// 不计免赔（车损险）
        /// </summary>
        [DataMember]
        public string NoDeductibles_Car { get; set; }
        /// <summary>
        /// 不计免赔（三责险）
        /// </summary>
        [DataMember]
        public string NoDeductibles_Three { get; set; }
        /// <summary>
        /// 不计免赔（司机责任险）
        /// </summary>
        [DataMember]
        public string NoDeductibles_Driver { get; set; }
        /// <summary>
        /// 不计免赔（乘客责任险）
        /// </summary>
        [DataMember]
        public string NoDeductibles_Passenger { get; set; }
        /// <summary>
        /// 不计免赔（全车盗抢险）
        /// </summary>
        [DataMember]
        public string NoDeductibles_AllCar { get; set; }
        /// <summary>
        /// 不计免赔（车身划痕险）
        /// </summary>
        [DataMember]
        public string NoDeductibles_Body { get; set; }
        /// <summary>
        /// 不计免赔（发动机损失险）
        /// </summary>
        [DataMember]
        public string NoDeductibles_Engine { get; set; }
        /// <summary>
        /// 不计免赔（自然损失险）
        /// </summary>
        [DataMember]
        public string NoDeductibles_Natural { get; set; }
        /// <summary>
        /// 不计免赔（精神损害险）
        /// </summary>
        [DataMember]
        public string NoDeductibles__Spirit { get; set; }
        /// <summary>
        /// 保险公司名称
        /// </summary>
        [DataMember]
        public string InsuranceCompanyName { get; set; }
        /// <summary>
        /// 保险公司返利金额
        /// </summary>
        [DataMember]
        public decimal CompanyRebateMoney { get; set; }
        /// <summary>
        /// 返利总金额
        /// </summary>
        [DataMember]
        public decimal InsuranceRebateMoney { get; set; }
    }
}
