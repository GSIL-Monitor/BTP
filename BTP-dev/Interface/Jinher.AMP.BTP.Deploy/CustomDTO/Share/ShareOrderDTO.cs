using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.Share
{
    public class ShareOrderDTO
    {

        [DataMember]
        public Guid OrderId { get; set; }


        /// <summary>
        /// 分享标题：订单中第一个商品名称
        /// </summary>
        [DataMember]
        public string ShareTitle { get; set; }

        /// <summary>
        /// 第一个商品缩略图
        /// </summary>
        [DataMember]
        public string ShareImgUrl { get; set; }

    }
}
