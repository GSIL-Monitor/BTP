using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 通用查询参数。
    /// </summary>
    [Serializable]
    [DataContract]
    public class Param2DTO
    {
        /// <summary>
        /// 应用id
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }
        /// <summary>
        ///用户id
        /// </summary>
        [DataMember]
        public Guid UserId { get; set; }

    }
}

