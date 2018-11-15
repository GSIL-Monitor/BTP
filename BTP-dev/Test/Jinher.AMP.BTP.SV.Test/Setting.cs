using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.News.SV.Test
{
    public  class Setting
    {
        /// <summary>
        /// 要调用的resuful地址
        /// </summary>
        public static readonly string RestfulUrl;
        public static readonly string LoginUrl;
        public static readonly string UserId;
        public static readonly string AppId;

        static Setting()
        {
            var appSetting = System.Configuration.ConfigurationManager.AppSettings;
            RestfulUrl = appSetting["RestfulUrl"];
            LoginUrl = appSetting["LoginUrl"];
            UserId = appSetting["UserId"];
            AppId = appSetting["AppId"]; 
        }
    }
}
