using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Jinher.AMP.BTP.Deploy.Enum;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 微小店信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class DistributionMicroShopDTO
    {
        /// <summary>
        /// 微小店id
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DateTime SubTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DateTime ModifiedOn { get; set; }

        /// <summary>
        /// 电商id
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        [DataMember]
        public Guid UserId { get; set; }

        /// <summary>
        /// 用户logo
        /// </summary>
        [DataMember]
        public string Logo { get; set; }

        /// <summary>
        /// 二维码地址
        /// </summary>
        [DataMember]
        public string QRCodeUrl { get; set; }

        /// <summary>
        /// 微店名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 短地址
        /// </summary>
        [DataMember]
        public string Url { get; set; }

        /// <summary>
        /// 微店类型
        /// </summary>
        [DataMember]
        public MicroshopTypeEnum Type { get; set; }

        /// <summary>
        /// 分销商id
        /// </summary>
        [DataMember]
        public Guid Key { get; set; }

        /// <summary>
        /// 上架商品集合
        /// </summary>
        [DataMember]
        public List<CommodityVM> UpCommodityDtos { get; set; }

        /// <summary>
        /// 下架商品集合
        /// </summary>
        [DataMember]
        public List<CommodityVM> DownCommodityDtos { get; set; }
    }
}
