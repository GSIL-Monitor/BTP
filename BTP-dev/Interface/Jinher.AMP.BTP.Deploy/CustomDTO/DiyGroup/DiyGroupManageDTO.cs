using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{

    /// <summary>
    /// 拼团管理输出dto
    /// </summary>
    [Serializable]
    [DataContract]
    public class DiyGroupManageDTO
    {
        /// <summary>
        /// 总数
        /// </summary>
        [DataMember]
        public int Count { get; set; }
        /// <summary>
        /// 数据列表
        /// </summary>
        [DataMember]
        public List< DiyGroupManageVM> Data { get; set; }
    }
}
