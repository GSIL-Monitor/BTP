using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee
{
    /// <summary>
    /// 易捷员工搜索DTO
    /// </summary>
    [Serializable]
    [DataContract]
    public class YJEmployeeCodeDTO
    {        
        /// <summary>
        /// 员工编码
        /// </summary>
        [DataMember]
        public string UserCode { get; set; }        
    }   
}
