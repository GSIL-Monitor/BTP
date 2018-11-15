using System;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 上门取件地址信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class AddressInfo
    {
        /// <summary>
        /// 上门取件联系人
        /// </summary>
        [DataMember]
        public string customerContactName { get; set; }

        /// <summary>
        /// 上门取件联系电话
        /// </summary>
        [DataMember]
        public string customerTel { get; set; }

        /// <summary>
        /// 上门取件地址-省编码
        /// </summary>
        [DataMember]
        public int pickwareProvince { get; set; }

        /// <summary>
        /// 上门取件地址-市编码
        /// </summary>
        [DataMember]
        public int pickwareCity { get; set; }

        /// <summary>
        /// 上门取件地址-县编码
        /// </summary>
        [DataMember]
        public int pickwareCounty { get; set; }

        /// <summary>
        /// 上门取件地址-镇编码
        /// </summary>
        [DataMember]
        public int pickwareVillage { get; set; }

        /// <summary>
        /// 上门取件地址
        /// </summary>
        [DataMember]
        public string pickwareAddress { get; set; }

        /// <summary>
        /// 省市县镇字符串
        /// </summary>
        [DataMember]
        public string ProviceCityStr { get; set; }
    }
}
