using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 供用商DTO
    /// </summary>
    [Serializable]
    [DataContract]
    public class SupplierListDTO
    {
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        [DataMember]
        public string[] AppIds { get; set; }

        public string _AppIds { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        [DataMember]
        public string AppNames { get; set; }

        /// <summary>
        /// 供用商名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 供用商编码
        /// </summary>
        [DataMember]
        public string Code { get; set; }

        /// <summary>
        /// 供用商类型
        /// </summary>
        [DataMember]
        public int Type { get; set; }

        /// <summary>
        /// 发货商
        /// </summary>
        [DataMember]
        public int ShipperType { get; set; }

    }
}
