using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// AppSet排序
    /// </summary>
    [Serializable]
    [DataContract]
    public class AppSetSortDTO
    {
        /// <summary>
        /// 排序Id、序号
        /// </summary>
        [DataMember]
        public List<Deploy.CustomDTO.SetCommodityOrderDTO> DtoList { get; set; }

        /// <summary>
        /// 所属AppId
        /// </summary>
        [DataMember]
        public Guid CategoryId { get; set; }
    }
}
