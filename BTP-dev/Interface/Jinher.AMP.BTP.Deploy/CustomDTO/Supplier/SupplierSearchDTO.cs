using System;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 供用商管搜索DTO
    /// </summary>
    [Serializable]
    [DataContract]
    public class SupplierSearchDTO : SearchBase
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [DataMember]
        public Guid UserId { get; set; }

        /// <summary>
        /// 商城ID
        /// </summary>
        [DataMember]
        public Guid EsAppId { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }

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
        public int? Type { get; set; }
    }
}
