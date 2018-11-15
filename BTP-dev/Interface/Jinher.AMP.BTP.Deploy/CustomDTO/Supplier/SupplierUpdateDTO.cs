using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 供用商DTO
    /// </summary>
    [Serializable]
    [DataContract]
    public class SupplierUpdateDTO
    {
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// 商城ID
        /// </summary>
        [DataMember]
        public Guid EsAppId { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        [DataMember]
        public List<Guid> AppIds { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        [DataMember]
        public string AppName { get; set; }

        /// <summary>
        /// 供用商名称
        /// </summary>
        [DataMember]
        public string SupplierName { get; set; }

        /// <summary>
        /// 供用商编码
        /// </summary>
        [DataMember]
        public string SupplierCode { get; set; }

        /// <summary>
        /// 供用商类型
        /// </summary>
        [DataMember]
        public short SupplierType { get; set; }

        /// <summary>
        /// 发货商类型
        /// </summary>
        [DataMember]
        public short ShipperType { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [DataMember]
        public Guid SubId { get; set; }
    }
}
