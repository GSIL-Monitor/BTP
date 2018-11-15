using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 商品属性列表
    /// </summary>
    [Serializable()]
    [DataContract]
    public class ComAttributeSDTO
    {
        /// <summary>
        /// 属性
        /// </summary>
        [DataMemberAttribute()]
        public string Attribute { get; set; }
        /// <summary>
        /// 次级属性
        /// </summary>
        [DataMemberAttribute()]
        public string SecondAttribute { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid CommodityId { get; set; }
        /// <summary>
        /// 属性ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid AttributeId { get; set; }
        /// <summary>
        /// 次级属性ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid SecondAttributeId { get; set; }
    }
}
