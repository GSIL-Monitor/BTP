using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{

    /// <summary>
    /// 应用相关接口搜索dto
    /// </summary>
    [Serializable]
    [DataContract]
    public class AppSearchDTO 
    {
        /// <summary>
        /// 应用Id
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }
    }
}
