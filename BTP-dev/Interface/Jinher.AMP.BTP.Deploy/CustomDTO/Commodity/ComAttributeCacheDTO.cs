using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 商品属性(缓存使用)
    /// </summary>
    [Serializable]
    [DataContract]
    public class ComAttributeCacheDTO
    {
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 代码
        /// </summary>
        [DataMember]
        public string Code { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        [DataMember]
        public DateTime SubTime { get; set; }

        /// <summary>
        /// 属性种类
        /// </summary>
        [DataMember]
        public string AttributeName { get; set; }

        /// <summary>
        /// 次级属性名称
        /// </summary>
        [DataMember]
        public string SecondAttributeName { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMember]
        public Guid CommodityId { get; set; }

        /// <summary>
        /// 属性ID
        /// </summary>
        [DataMember]
        public Guid AttributeId { get; set; }

        /// <summary>
        /// 次级属性ID
        /// </summary>
        [DataMember]
        public Guid SecondAttributeId { get; set; }
    }
}
