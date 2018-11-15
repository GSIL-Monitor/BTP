using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Jinher.AMP.BTP.Common
{
    /// <summary>
    /// 微信推广主
    /// </summary>
    public class WeChatSpreaderSection : ConfigurationSection
    {
        /// <summary>
        /// 微信二维码appid
        /// </summary>   
        [ConfigurationProperty("appId", IsRequired = false)]
        public Guid AppId
        {
            get
            {
                string appIdStr = this["appId"].ToString();
                Guid result;
                if (Guid.TryParse(appIdStr, out result))
                {
                    return result;
                }
                return Guid.Empty;
            }
        }

        /// <summary>
        /// 消息标题
        /// </summary>   
        [ConfigurationProperty("messageTitle", IsRequired = false)]
        public string MessageTitle
        {
            get
            {
                return this["messageTitle"].ToString();
            }
        }

        /// <summary>
        /// 消息描述
        /// </summary>   
        [ConfigurationProperty("messageDesc", IsRequired = false)]
        public string MessageDesc
        {
            get
            {
                return this["messageDesc"].ToString();
            }
        }

        /// <summary>
        /// 消息图片
        /// </summary>   
        [ConfigurationProperty("messagePic", IsRequired = false)]
        public string MessagePic
        {
            get
            {
                return this["messagePic"].ToString();
            }
        }

    }

}
