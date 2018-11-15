using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    public class JdSkuStateDto
    {
        public string Sku { get; set; }

        /// <summary>
        /// 1为上架，0为下架
        /// </summary>
        public int State { get; set; }
    }
}
