using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 检查拼团输出DTO
    /// </summary>
    [Serializable, DataContract]
    public class CheckDiyGroupOutputDTO
    {
        /// <summary>
        /// 获取或设置 当前拼团是否已经完成
        /// </summary>
        [DataMember]
        public bool IsCompleted { get; set; }
    }
}
