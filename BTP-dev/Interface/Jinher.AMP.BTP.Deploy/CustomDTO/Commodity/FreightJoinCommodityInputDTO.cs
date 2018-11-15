using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 运费模板关联商品
    /// </summary>
    public class FreightTemplateAssociationCommodityInputDTO
    {
        /// <summary>
        /// 
        /// </summary>
        public FreightTemplateAssociationCommodityInputDTO()
        {
            CommodityIds = new List<Guid>();
        }

        /// <summary>
        /// 获取或设置 应用编号
        /// </summary>
        public Guid AppId { get; set; }

        /// <summary>
        /// 获取或设置 运费模板编号
        /// </summary>
        public Guid TemplateId { get; set; }

        /// <summary>
        /// 获取或设置 商品编号列表
        /// </summary>
        public List<Guid> CommodityIds { get; set; }
    }
}
