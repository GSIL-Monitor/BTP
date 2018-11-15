using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 未完成的拼团 OutputDTO
    /// </summary>
    [Serializable, DataContract]
    public partial class UnfinishedDiyGroupOutputDTO
    {
        /// <summary>
        /// 获取或设置 拼团编号
        /// </summary>
        [DataMember]
        public Guid GroupId { get; set; }

        /// <summary>
        /// 获取或设置 团长昵称
        /// </summary>
        [DataMember]
        public string OwnerName { get; set; }

        /// <summary>
        /// 获取或设置 团长头像
        /// </summary>
        [DataMember]
        public string OwnerIcon { get; set; }

        /// <summary>
        /// 获取或设置 成团缺少的人数
        /// </summary>
        [DataMember]
        public int LackMember { get; set; }

        /// <summary>
        /// 获取或设置 拼团到期时间
        /// </summary>
        [DataMember]
        public DateTime ExpireTime { get; set; }

        /// <summary>
        /// 获取或设置 剩余拼团时间
        /// <para>tips: 用于倒计时</para>
        /// </summary>
        [DataMember]
        public int SpareSeconds
        {
            get
            {
                return (int)ExpireTime.Subtract(DateTime.Now).TotalSeconds;
            }
            set { } 
        }
    }
}
