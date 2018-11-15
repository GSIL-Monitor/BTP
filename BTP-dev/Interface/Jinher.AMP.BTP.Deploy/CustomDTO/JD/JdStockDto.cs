using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    public class JdStockDto
    {
        public string Area { get; set; }

        public string Desc { get; set; }

        public string Sku { get; set; }

        /// <summary>
        /// 33 有货 现货-下单立即发货 39 有货 在途-正在内部配货，预计2~6天到达本仓库 40 有货 可配货-下单后从有货仓库配货 36 预订 34 无货
        /// </summary>
        public int State { get; set; }

        public bool HaveStock { get { return (State == 33 || State == 40); } }

        /// <summary>
        /// 易捷北京库存
        /// </summary>
        public int yjStock { get { return HaveStock ? 999 : 0; } }
    }
}
