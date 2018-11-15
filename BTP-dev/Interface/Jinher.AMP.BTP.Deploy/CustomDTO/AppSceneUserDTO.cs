using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 商家的移动坐席数据
    /// </summary>
    [Serializable]
    [DataContract]
    public class AppSceneUserDTO
    {
        /// <summary>
        /// 店铺ID
        /// </summary>
        [DataMember]
        public Guid appId { get; set; }
        /// <summary>
        /// 场景ID
        /// </summary>
        [DataMember]
        public Guid? SceneId { get; set; }
        /// <summary>
        /// 场景名称
        /// </summary>
        [DataMember]
        public string SceneName { get; set; }  
        /// <summary>
        /// 用户Id
        /// </summary>
        [DataMember]
        public Guid UserId { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        [DataMember]
        public string UserName { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        [DataMember]
        public string Account { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        [DataMember]
        public DateTime? Birthday { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        [DataMember]
        public string HeadIcon { get; set; }
    }
    /// <summary>
    /// 易捷北京下所有店铺信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class YJBJAppInfo
    {
        /// <summary>
        /// 店铺Id
        /// </summary>
        [DataMember]
        public Guid appId { get; set; }
        /// <summary>
        /// 店铺名称
        /// </summary>
        [DataMember]
        public string AppName { get; set; }
        /// <summary>
        /// 店铺状态 0入驻  1 非入驻
        /// </summary>
        [DataMember]
        public int state { get; set; }
        /// <summary>
        /// 商家类型 自营他配；第三方；自营自配自采；自营自配统采
        /// </summary>
        [DataMember]
        public string type { get; set; }  
    }
}
