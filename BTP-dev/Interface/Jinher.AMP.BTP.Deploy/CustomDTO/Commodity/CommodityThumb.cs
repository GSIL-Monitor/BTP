using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 商品缩略信息
    /// </summary>
    [Serializable()]
    [DataContract]
    public class CommodityThumb
    {
        /// <summary>
        /// 发布商品的店铺的appid
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }

        /// <summary>
        /// 商品是否参加拼团活动
        /// </summary>
        [DataMember]
        public bool IsDiyGroup { get; set; }
        /// <summary>
        /// 商品图片
        /// </summary>
        [DataMember]
        public string PicturesPath { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }
    }
}
