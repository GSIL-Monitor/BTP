using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 分销商
    /// </summary>
    [Serializable()]
    [DataContract]
   public  class ManageDTO
    {
        /// <summary>
        /// 一级分销商总数
        /// </summary>
        [DataMemberAttribute()]
        public int? Count { get; set; }
        /// <summary>
        /// 父类名称
        /// </summary>
        [DataMemberAttribute()]
        public string ParentName { get; set; }
        /// <summary>
        /// 父类昵称
        /// </summary>
        [DataMemberAttribute()]
        public string ParentCode { get; set; }
        /// <summary>
        /// 分销商信息
        /// </summary>
        [DataMemberAttribute()]
        public List<ManagerSDTO> Manager { get; set; }

    }
}
