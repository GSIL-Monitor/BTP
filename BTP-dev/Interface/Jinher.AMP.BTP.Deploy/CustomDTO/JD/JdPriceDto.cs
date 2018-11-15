using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 协议价价格
    /// </summary>
    public class JdPriceDto
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public string SkuId { get; set; }

        /// <summary>
        /// 协议价格
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// 京东价格
        /// </summary>
        public decimal JdPrice { get; set; }
    }
}
