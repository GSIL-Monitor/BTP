using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.Commodity
{
    [Serializable]
    [DataContract]
    public class SNComCostDiffDTO
    {
        /// <summary>
        ///商品id
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }
        /// <summary>
        ///商品名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        ///备注编码
        /// </summary>
        [DataMember]
        public string JdCode { get; set; }
        /// <summary>
        ///苏宁最新价格
        /// </summary>
        [DataMember]
        public decimal SNCostprice { get; set; }
        /// <summary>
        ///商品表进货价
        /// </summary>
        [DataMember]
        public decimal? CostPrice { get; set; }
        /// <summary>
        ///商品状态
        /// </summary>
        [DataMember]
        public int state { get; set; }  
    }
}
