using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable()]
    [DataContract]
    public class ManageNumDTO
    {
        /// <summary>
        /// 分销商总数
        /// </summary>
        [DataMemberAttribute()]
        public int Count { get; set; }
        /// <summary>
        /// 分销商层级
        /// </summary>
        [DataMemberAttribute()]
        public int MaxLevel { get; set; }
    }
}
