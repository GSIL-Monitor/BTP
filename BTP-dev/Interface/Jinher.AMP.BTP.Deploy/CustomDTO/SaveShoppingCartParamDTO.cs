using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable()]
    [DataContract]
    public class SaveShoppingCartParamDTO
    {
        /// <summary>
        /// 商品ID    
        /// </summary>
        [DataMember]
        public Guid CommodityId { get; set; }

        /// <summary>
        ///商品数量
        /// </summary>
        [DataMember]
        public int CommodityNumber { get; set; }

        /// <summary>
        /// 尺寸颜色
        /// </summary>
        [DataMember]
        public string SizeAndColorId { get; set; }

        /// <summary>
        /// 当前用户Id
        /// </summary>
        [DataMember]
        public Guid UserId { get; set; }

        /// <summary>
        /// 发布商品的店铺的Id
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }

        /// <summary>
        /// 商品在哪个馆被加入到购物车的。
        /// </summary>
        [DataMember]
        public Guid EsAppId { get; set; }
        /// <summary>
        /// 商品库存id
        /// </summary>
        [DataMember]
        public Guid? CommodityStockId { get; set; }

        /// <summary>
        /// 金采团购活动Id
        /// </summary>
        [DataMember]
        public Guid? JcActivityId { get; set; }

        /// <summary>
        /// 规格设置
        /// </summary>
        [DataMember]
        public int? Specifications { get; set; }

    }
}
