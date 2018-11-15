using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable()]
    [DataContract]
     public class ServiceCommodityAndCategoryDTO
    {
        /// <summary>
        /// AppID
        /// </summary>
        [DataMemberAttribute()]
        public Guid AppId { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid CommodityId { get; set; }
        /// <summary>
        /// 虚拟商品名称
        /// </summary>
        [DataMemberAttribute()]
        public String Name { get; set; }
        /// <summary>
        /// 商品编号字符
        /// </summary>
        [DataMemberAttribute()]
        public String No_Code { get; set; }
        /// <summary>
        /// 商品编号数字
        /// </summary>
        [DataMemberAttribute()]
        public int No_Number { get; set; }
        /// <summary>
        /// 商品单价
        /// </summary>
        [DataMemberAttribute()]
        public decimal Price { get; set; }
        /// <summary>
        /// 市场价
        /// </summary>
        [DataMemberAttribute()]
        public decimal? MarketPrice { get; set; }
       
        /// <summary>
        /// 商品属性
        /// </summary>
        [DataMemberAttribute()]
        public List<ComAttibuteDTO> comAttributes { get; set; }
        /// <summary>
        /// 组合属性列表  
        /// </summary>
        [DataMemberAttribute()]
        public List<CommodityStockDTO> ComAttributes { get; set; }
        /// <summary>
        /// 商品图片路径
        /// </summary>
        [DataMemberAttribute()]
        public string PicturesPath { get; set; }
        /// <summary>
        /// 销售地区
        /// </summary>
        [DataMemberAttribute()]
        public string SaleAreas { get; set; }
        /// <summary>
        /// 图片列表地址串
        /// </summary>
        [DataMemberAttribute()]
        public List<string> Picturelist { get; set; }
        /// <summary>
        /// 商品库存
        /// </summary>
        [DataMemberAttribute()]
        public int Stock { get; set; }
        /// <summary>
        /// 商品类型:0实体商品；1虚拟商品
        /// </summary>
        [DataMember]
        public int CommodityType { get; set; }
        /// <summary>
        /// 商品描述
        /// </summary>
        [DataMemberAttribute()]
        public string Description { get; set; }

        /// <summary>
        /// 有效属性组合
        /// </summary>
        [DataMemberAttribute()]
        public List<ServiceSecondAttr> ActiveSecondAtributes { get; set; }
    }
    [Serializable()]
    [DataContract]
    public class ServiceSecondAttr
    {
        /// <summary>
        /// 沟通方式
        /// </summary>
        [DataMemberAttribute()]
        public Guid CommunicateAttribute { get; set; }
        /// <summary>
        /// 计费单位
        /// </summary>
        [DataMemberAttribute()]
        public Guid BillingAttribute { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        [DataMemberAttribute()]
        public decimal Price { get; set; }
    }
    
}
