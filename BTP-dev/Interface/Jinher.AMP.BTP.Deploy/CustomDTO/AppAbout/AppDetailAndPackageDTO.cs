using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    ///  应用信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class AppDetailAndPackageDTO
    {
        /// <summary>
        /// 应用下载量
        /// </summary>
        [DataMember]
        public int DownLoad { get; set; }
        /// <summary>
        /// 应用图标
        /// </summary>
        [DataMember]
        public string Icon { get; set; }
        /// <summary>
        /// 应用Id
        /// </summary> 
        [DataMember]
        public Guid Id { get; set; }
        /// <summary>
        ///  应用状态非法
        /// </summary>
        [DataMember]
        public int Illegal { get; set; }
        /// <summary>
        ///  应用名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 应用状态下架
        /// </summary> 
        [DataMember]
        public int OffShelves { get; set; }
        /// <summary>
        ///  应用二维码
        /// </summary> 
        [DataMember]
        public string QRCodeUrl { get; set; }

        /// <summary>
        ///  苹果Url
        /// </summary> 
        [DataMember]
        public string IosUrl { get; set; }

        /// <summary>
        ///  Android的Url
        /// </summary> 
        [DataMember]
        public string AndroidUrl { get; set; }
      
    }
}
