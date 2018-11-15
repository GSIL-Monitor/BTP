using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{

    /// <summary>
    /// 拼团搜索入参
    /// </summary>
    [Serializable]
    [DataContract]
    public class DiyGroupSearchDTO : SearchBase
    {
        /// <summary>
        /// 应用Id
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }
        /// <summary>
        /// EsAppId
        /// </summary>
        [DataMember]
        public Guid EsAppId { get; set; }
        /// <summary>
        /// 拼团状态 空为不限
        /// </summary>
        [DataMember]
        public string State { get; set; }
        /// <summary>
        /// 拼团商品（模糊匹配）
        /// </summary>
        [DataMember]
        public string ComNameSub { get; set; }
        /// <summary>
        /// 团Id
        /// </summary>
        [DataMember]
        public Guid DiyGoupId { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        [DataMember]
        public Guid UserId { get; set; }
    }
}
