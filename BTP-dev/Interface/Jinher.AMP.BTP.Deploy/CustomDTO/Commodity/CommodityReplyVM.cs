using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{

    [Serializable()]
    [DataContract]
    public class CommodityReplyVM
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid CommodityId { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMemberAttribute()]
        public string CommodityName { get; set; }
        /// <summary>
        /// 商品图片
        /// </summary>
        [DataMemberAttribute()]
        public string CommodityPicture { get; set; }
        

        /// <summary>
        /// 回复集合
        /// </summary>
        [DataMemberAttribute()]
        public List<ReviewVM> ReviewList { get; set; }
    }
}
