using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable]
    [DataContract]
    public class ParamDTO
    {
        /// <summary>
        /// 当前页数
        /// </summary>
        [DataMember]
        public int CurrentPageIndex { get; set; }
        /// <summary>
        /// 每页显示数
        /// </summary>
        [DataMember]
        public int PageSize { get; set; }

    }
}

