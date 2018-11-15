using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 完整模板
    /// </summary>
    [Serializable()]
    [DataContract]
    public class OrderPrintTemplate
    {
        /// <summary>
        /// 模板
        /// </summary>
        [DataMember]
        public ExpressOrderTemplateDTO Template { get; set; }

        /// <summary>
        /// 模板属性
        /// </summary>
        [DataMember]
        public List<ExpressOrderTemplatePropertyDTO> Property { get; set; }
    }
}
