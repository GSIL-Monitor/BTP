using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 保险公司表
    /// </summary>
    [Serializable()]
    [DataContract]
    public class InsuranceCompanyDTO
    {
        /// <summary>
        /// 保险公司名
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 保险公司图片
        /// </summary>
        [DataMember]
        public string PicUrl { get; set; }
        /// <summary>
        /// 保险公司编码
        /// </summary>
        [DataMember]
        public string InsuranceCompanyCode { get; set; }
    }
}
