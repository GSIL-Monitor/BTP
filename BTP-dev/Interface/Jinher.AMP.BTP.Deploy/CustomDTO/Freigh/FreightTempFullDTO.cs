using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 保存运费模板参数实体。
    /// </summary>
    [Serializable()]
    [DataContract]
    public class FreightTempFullDTO
    {
        /// <summary>
        /// 运费模板信息
        /// </summary>
        [DataMemberAttribute()]
        public FreightTemplateDTO Freight { get; set; }

        /// <summary>
        /// 指定地区运费信息
        /// </summary>
        [DataMemberAttribute()]
        public List<FreightTemplateDetailDTO> DetailList { get; set; }

        /// <summary>
        /// 部分地区包邮信息。
        /// </summary>
        [DataMemberAttribute()]
        public List<FreightPartialFreeDTO> PartialFreeList { get; set; }
    }
}
