using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable()]
    [DataContract()]
    public class CommodityZPHResultDTO
    {
        /// <summary>
        /// 商品列表
        /// </summary>
        [DataMember()]
        public List<CommoditySDTO> CommodityList { get; set; }

        /// <summary>
        /// 商品总数
        /// </summary>
        [DataMember()]
        public int TotalCount { get; set; }
    }
}
