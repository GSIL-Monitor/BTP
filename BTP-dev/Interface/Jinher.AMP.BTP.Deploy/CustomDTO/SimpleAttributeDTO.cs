using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 商品属性DTO
    /// </summary>
    [DataContract]
    public class SimpleAttributeDTO
    {
        /// <summary>
        /// 属性名
        /// </summary>
        [DataMember]
        public string AttrName
        {
            get;
            set;
        }

        /// <summary>
        /// 属性值
        /// </summary>
        [DataMember]
        public string AttrValue
        {
            get;
            set;
        }
    }
}
