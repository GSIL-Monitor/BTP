using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 商品选择列表
    /// </summary>
    [DataContract]
    [Serializable]
    public class ComdtyList4SelCDTO
    {        
        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }
        
        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 商品图片
        /// </summary> 
        [DataMember]
        public string Pic { get; set; }
          
        /// <summary>
        /// 商品价格
        /// </summary>
        [DataMember]
        public decimal Price { get; set; }
        /// <summary>
        /// AppId
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }
        /// <summary>
        /// 应用名称
        /// </summary>
        [DataMember]
        public string AppName { get; set; }        
        /// <summary>
        /// 库存
        /// </summary>
        [DataMember]
        public int Stock { get; set; }
        /// <summary>
        /// 是否支持自提:0不支持，1支持。
        /// </summary>
        [DataMember]
        public int IsEnableSelfTake { get; set; }
        /// <summary>
        /// 市场价
        /// </summary>
        [DataMember]
        public decimal MarketPrice { get; set; }
        /// <summary>
        /// 商品状态：上架=0，未上架=1
        /// </summary>
        [DataMember]
        public int State { get; set; }

        /// <summary>
        /// 已上架分类
        /// </summary>
        [DataMember]
        public string CommodityCategory { get; set; }
    }
}
