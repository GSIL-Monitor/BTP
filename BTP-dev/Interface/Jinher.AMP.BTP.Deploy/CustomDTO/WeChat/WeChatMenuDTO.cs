using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.WeChat
{
    /// <summary>
    /// 微信菜单
    /// </summary>
    [DataContract]
    [Serializable]
    public class WeChatMenuDTO
    {
        /// <summary>
        /// 菜单
        /// </summary>
        [DataMember]
        public List<WeChatMenuInfo> button { get; set; }
    }
    /// <summary>
    /// 一级菜单
    /// </summary>
    [DataContract]
    [Serializable]
    public class WeChatMenuInfo
    {
        /// <summary>
        /// 一级菜单
        /// </summary>
        [DataMember]
        public string name { get; set; }
        /// <summary>
        /// 二级菜单
        /// </summary>
        [DataMember]
        public List<WeChatSubMenuInfo> sub_button { get; set; }
    }

    /// <summary>
    /// 二级菜单
    /// </summary>
    [DataContract]
    [Serializable]
    public class WeChatSubMenuInfo
    {
        /// <summary>
        /// 类型:view/click
        /// </summary>
        [DataMember]
        public WeChatMenuType type { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [DataMember]
        public string name { get; set; }
        /// <summary>
        /// 跳转地址
        /// </summary>
        [DataMember]
        public string url { get; set; }
    }

    /// <summary>
    /// 菜单的响应动作类型
    /// </summary>
    [DataContract]
    [Serializable]
    public enum WeChatMenuType
    {
        /// <summary>
        /// 跳转URL
        /// </summary>
        [EnumMember]
        view = 0,
        /// <summary>
        /// 点击推事件
        /// </summary>
        [EnumMember]
        click = 1
    }
}
