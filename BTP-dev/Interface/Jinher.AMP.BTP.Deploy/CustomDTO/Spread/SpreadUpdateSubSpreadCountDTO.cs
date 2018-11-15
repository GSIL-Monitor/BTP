using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 修改子代理数量
    /// </summary>
    [Serializable]
    [DataContract]
    public class SpreadUpdateSubSpreadCountDTO
    {
        /// <summary>
        /// 推广主ID
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// 子代理数量。
        /// </summary>
        [DataMember]
        public int Count { get; set; }
    }
}
