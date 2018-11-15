using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    [Serializable]
    public class ComdtySearch4SelCDTO : SearchBase
    {
        /// <summary>
        /// 商品所在appId
        /// </summary>
        [DataMember]
        public Guid? AppId { get; set; }
        /// <summary>
        /// 应用名称（模糊查询）
        /// </summary>
        [DataMember]
        public string AppName { get; set; }
        /// <summary>
        /// 商品编码
        /// </summary>
        [DataMember]
        public string CommodityCode { get; set; }
        /// <summary>
        /// 商品Id
        /// </summary>   
        [DataMember]
        public Guid? CommodityId { get; set; }
        /// <summary>
        /// 商品名称（模糊查询）
        /// </summary>
        [DataMember]
        public string CommodityName { get; set; }
        /// <summary>
        /// 所属APPID
        /// </summary>
        [DataMember]
        public Guid belongTo { get; set; }







        /// <summary>
        /// 商品所属类别
        /// </summary>
        [DataMember]
        public string Categorys { get; set; }
        /// <summary>
        /// 商品最小毛利率
        /// </summary>
        [DataMember]
        public string MinInterestRate { get; set; }
        /// <summary>
        /// 商品最大毛利率
        /// </summary>
        [DataMember]
        public string MaxInterestRate { get; set; }
        /// <summary>
        /// 商品最小价格
        /// </summary>
        [DataMember]
        public string MinPrice { get; set; }
        /// <summary>
        /// 商品最大价格
        /// </summary>
        [DataMember]
        public string MaxPrice { get; set; }
    }
}
