using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable]
    [DataContract]
    public class ColorAndSizeAttributeVM
    {
        /// <summary>
        /// 一级属性ID
        /// </summary>
        [DataMember]
        public Guid AttributeId { get; set; }
        /// <summary>
        /// 二级属性ID
        /// </summary>
        [DataMember]
        public Guid SecondAttributeId { get; set; }
        /// <summary>
        /// 一级属性名
        /// </summary>
        [DataMember]
        public string AttributeName { get; set; }
        /// <summary>
        /// 二级属性名
        /// </summary>
        [DataMember]
        public string SecondAttributeName { get; set; }
        /// <summary>
        /// appid
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMember]
        public Guid CommodityId { get; set; }
    }
}
