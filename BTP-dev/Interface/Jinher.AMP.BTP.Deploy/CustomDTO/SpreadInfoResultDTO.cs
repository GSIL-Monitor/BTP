using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 推广信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class SpreadInfoResultDTO
    {
        /// <summary>
        /// 推广者ID。
        /// </summary>
        [DataMember]
        public System.Guid SpreadId { get; set; }

        /// <summary>
        /// 推广地址。
        /// </summary>
        [DataMember]
        public string SpreadUrl { get; set; }

        /// <summary>
        /// 推广码。
        /// </summary>
        [DataMember]
        public System.Guid SpreadCode { get; set; }

        /// <summary>
        /// 描述。
        /// </summary>
        [DataMember]
        public string SpreadDesc { get; set; }

        /// <summary>
        /// 推广类型 0：推广主，1：电商馆，2：总代，3企业
        /// </summary>
        [DataMember]
        public int SpreadType { get; set; }

        /// <summary>
        /// 是否删除。
        /// </summary>
        [DataMember]
        public int IsDel { get; set; }

        /// <summary>
        /// 所属馆AppId
        /// </summary>
        [DataMember]
        public System.Guid AppId { get; set; }
    }
}
