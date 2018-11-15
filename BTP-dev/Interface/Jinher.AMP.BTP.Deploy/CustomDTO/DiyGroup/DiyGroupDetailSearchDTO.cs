using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;


namespace Jinher.AMP.BTP.Deploy.CustomDTO
{

    /// <summary>
    /// 拼团详情查询类
    /// </summary>
    [Serializable]
    [DataContract]
    public class DiyGroupDetailSearchDTO
    {
        /// <summary>
        /// 团Id
        /// </summary>
        [DataMember]
        public Guid DiyGoupId { get; set; }
    }
}
