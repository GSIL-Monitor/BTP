using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 按运费模板搜索商品
    /// </summary>
    public partial class SearchCommodityByFreightTemplateInputDTO : SearchBase
    {
        /// <summary>
        /// 获取或设置 应用编号
        /// </summary>
        public Guid AppId { get; set; }

        /// <summary>
        /// 获取或设置 运费模板编号
        /// </summary>
        public Guid TemplateId { get; set; }

        /// <summary>
        /// 获取或设置 是否显示已关联的商品
        /// </summary>
        public bool ShowAssociated { get; set; }

        /// <summary>
        /// 获取或设置 商品名称
        /// </summary>
        public string CommodityName { get; set; }

        /// <summary>
        /// 获取或设置 是否关联活动商品
        /// </summary>
        public bool JoinPromotion { get; set; }
    }
}
