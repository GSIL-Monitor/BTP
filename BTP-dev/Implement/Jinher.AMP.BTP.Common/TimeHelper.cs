using System;

namespace Jinher.AMP.BTP.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class TimeHelper
    {
        /// <summary>
        /// 时间戳-秒
        /// </summary>
        /// <returns></returns>
        public static long GetTimestampS()
        {
            DateTime utcStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long timesapn = (long)(DateTime.UtcNow - utcStart).TotalSeconds;
            return timesapn;
        }

        /// <summary>
        /// 时间戳-毫秒
        /// </summary>
        /// <returns></returns>
        public static long GetTimestampM()
        {
            DateTime utcStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long timesapn = (long)(DateTime.UtcNow - utcStart).TotalMilliseconds;
            return timesapn;
        }
    }
}
