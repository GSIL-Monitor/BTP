using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable()]
    [DataContract]
    public class CommodityPictureCDTO
    {
        /// <summary>
        /// 商品图片
        /// </summary>
        [DataMemberAttribute()]
        public string PicturesPath { get; set; }
           /// <summary>
        /// 排序
        /// </summary>
        [DataMemberAttribute()]
        public int Sort { get; set; }
    }
}
