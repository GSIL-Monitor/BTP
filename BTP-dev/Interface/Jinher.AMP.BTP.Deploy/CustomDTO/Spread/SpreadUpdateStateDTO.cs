using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 二维码查询
    /// </summary>
    [Serializable]
    [DataContract]
    public class SpreadUpdateStateDTO
    {
        /// <summary>
        /// 公众号名称
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// 没有元数据文档可用。
        /// </summary>
        [DataMember]
        public bool IsDel { get; set; }
    }
}
