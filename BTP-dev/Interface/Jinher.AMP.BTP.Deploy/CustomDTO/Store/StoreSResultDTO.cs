using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 门店查询结果
    /// </summary>
    [Serializable()]
    [DataContract]
    public class StoreSResultDTO
    {
        /// <summary>
        /// 门店查询结果
        /// </summary>
        public StoreSResultDTO()
        {
            Count = 0;
            Stroes = new List<StoreSDTO>();
        }

        /// <summary>
        /// 门店列表
        /// </summary>
        [DataMember]
        public List<StoreSDTO> Stroes { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        [DataMember]
        public long Count { get; set; }
    }
}
