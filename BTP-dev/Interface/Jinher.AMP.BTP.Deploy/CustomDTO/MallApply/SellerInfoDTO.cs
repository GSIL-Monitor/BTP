using System;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 商家信息DTO
    /// </summary>
    [DataContract]
    [Serializable]
    public class SellerInfoDTO
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }

        /// <summary>
        /// 商城ID
        /// </summary>
        [DataMember]
        public Guid EsAppId { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        [DataMember]
        public string AppName { get; set; }

        /// <summary>
        /// 商城名称
        /// </summary>
        [DataMember]
        public string EsAppName { get; set; }

        /// <summary>
        /// 商家类型
        /// </summary>
        [DataMember]
        public string Type { get; set; }
    }
}
