using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Common
{
    /// <summary>
    /// redis 帮助类
    /// Jinher.JAP.RedisCache.dll
    /// </summary>
    public class RedisHelperNew
    {
        /// <summary>
        /// 写入Redis
        /// </summary>
        /// <param name="hashId"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireHours"></param>
        /// <param name="redisName"></param>
        public static void RegionSet(string hashId, string key, object value, int? expireHours, string redisName)
        {
            Jinher.JAP.RedisCache.RedisHelper.RegionSet(hashId, key, value, expireHours, redisName);
        }
        /// <summary>
        /// 移除redis
        /// </summary>
        /// <param name="hashId"></param>
        /// <param name="key"></param>
        /// <param name="redisName"></param>
        /// <returns></returns>
        public static bool Remove(string hashId, string key, string redisName)
        {
            return Jinher.JAP.RedisCache.RedisHelper.Remove(hashId + "|_|" + key, redisName);
        }
        /// <summary>
        /// 获取redis
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashId"></param>
        /// <param name="key"></param>
        /// <param name="redisName"></param>
        /// <returns></returns>
        public static T RegionGet<T>(string hashId, string key, string redisName)
        {
            return Jinher.JAP.RedisCache.RedisHelper.RegionGet<T>(hashId, key, redisName);
        }

    }
}
