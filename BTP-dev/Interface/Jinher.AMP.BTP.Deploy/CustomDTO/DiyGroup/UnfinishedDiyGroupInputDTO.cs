using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 未完成的拼团 InputDTO
    /// </summary>
    public class UnfinishedDiyGroupInputDTO
    {
        /// <summary>
        /// 获取或设置 商品编号
        /// </summary>
        public Guid CommodityId { get; set; }

        /// <summary>
        /// 获取或设置 外部促销编号
        /// </summary>
        public Guid OutsidePromoId { get; set; }

        /// <summary>
        /// 获取或设置 应用编号
        /// </summary>
        public Guid AppId { get; set; }
    }
}
