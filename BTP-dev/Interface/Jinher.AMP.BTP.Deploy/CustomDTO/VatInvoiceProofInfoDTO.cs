using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 增票资质
    /// </summary>
    [Serializable]
    [DataContract]
    public class VatInvoiceProofInfoDTO
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }
        /// <summary>
        /// 单位名称
        /// </summary>
        [DataMember]
        public string CompanyName { get; set; }
        /// <summary>
        /// 纳税人识别码
        /// </summary>
        [DataMember]
        public string IdentifyNo { get; set; }
        /// <summary>
        /// 注册地址
        /// </summary>
        [DataMember]
        public string Address { get; set; }
        /// <summary>
        /// 注册电话
        /// </summary>
        [DataMember]
        public string Phone { get; set; }
        /// <summary>
        /// 开户银行
        /// </summary>
        [DataMember]
        public string BankName { get; set; }
        /// <summary>
        /// 银行账户
        /// </summary>
        [DataMember]
        public string BankCode { get; set; }
        /// <summary>
        /// 营业执照复印件地址
        /// </summary>
        [DataMember]
        public string BusinessLicence { get; set; }
        /// <summary>
        /// 税务登记复印件地址
        /// </summary>
        [DataMember]
        public string TaxRegistration { get; set; }
        /// <summary>
        /// 一般纳税人资格认证复印件地址
        /// </summary>
        [DataMember]
        public string PersonalProof { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        [DataMember]
        public DateTime SubTime { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        [DataMember]
        public DateTime ModifiedOn { get; set; }
    }
}
