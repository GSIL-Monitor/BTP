using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 更新商品分类
    /// </summary>
    [Serializable()]
    [DataContract]
    public class UCategoryVM
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid CommodityId { get; set; }

        /// <summary>
        /// 分类ID串
        /// </summary>
        [DataMemberAttribute()]
        public string ComCateIds { get; set; }



    }
}
