using System;
using Jinher.AMP.BTP.Common;

namespace Jinher.AMP.BTP.TPS.Cache
{
    /// <summary>
    /// LVP Cache Helper
    /// </summary>
    public class LVPCache
    {
        /// <summary>
        ///  获取爱尔目直播地址(新)
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetEquipmentUrlNew(Guid appId, Guid userId)
        {
            var key = "EquipmentUrlNew:AI:" + appId.ToString();
            var data = RedisHelper.Get<string>(key);
            if (data == null)
            {
                data = LVPSV.GetEquipmentUrlNew(appId, userId);
                if (string.IsNullOrEmpty(data))
                {
                    data = string.Empty;
                }
                RedisHelper.Set(key, data, TimeSpan.FromHours(1));
            }
            return data;
        }
    }
}