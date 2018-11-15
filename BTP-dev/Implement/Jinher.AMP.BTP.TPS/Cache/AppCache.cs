using System;
using Jinher.AMP.App.Deploy.CustomDTO;
using Jinher.AMP.BTP.Common;

namespace Jinher.AMP.BTP.TPS.Cache
{
    /// <summary>
    /// CBC Cache Helper
    /// </summary>
    public class AppCache
    {
        /// <summary>
        /// 获取应用名称、图标
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public AppIdNameIconDTO GetAppNameIcon(Guid appId)
        {
            var key = "AppNameIcon:" + appId.ToString();
            var data = RedisHelper.Get<AppIdNameIconDTO>(key);
            if (data == null)
            {
                data = APPSV.Instance.GetAppDetailById(appId);
                RedisHelper.Set(key, data, TimeSpan.FromHours(1));
            }
            return data;
        }
    }
}