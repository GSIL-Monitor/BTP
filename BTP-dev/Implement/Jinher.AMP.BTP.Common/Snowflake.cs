using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Configuration;

namespace Jinher.AMP.BTP.Common
{
    public class Snowflake
    {
        static Snowflake()
        {
            var _machineId = ConfigurationManager.AppSettings["MachineId"];
            if (!string.IsNullOrEmpty(_machineId))
            {
                machineId = int.Parse(_machineId);
            }
        }

        private static readonly DateTime BaseDateTime = new DateTime(2017, 11, 23, 0, 0, 0);

        private static int sequence = 0;

        private const int sequenceBits = 11;
        private static int sequenceMask = -1 ^ (-1 << sequenceBits);

        private const int maxMachineIdBits = 4;
        private static int machineId = 0;
        private const int machineIdShift = sequenceBits;


        private static long lastTimestamp = 0;
        private static object syncRoot = new object();

        private static long GetTimestamp()
        {
            return (long)(DateTime.Now - BaseDateTime).TotalSeconds;
        }

        private static DateTime GetDateTime(long timeStamp)
        {
            return BaseDateTime.AddSeconds(timeStamp);
        }

        private static long GetNextTimestamp(long lastTimestamp)
        {
            long timestamp = GetTimestamp();
            if (timestamp <= lastTimestamp)
            {
                Thread.Sleep(100);
                return GetNextTimestamp(lastTimestamp);
            }
            return timestamp;
        }

        public static string NewId()
        {
            long timestamp;
            lock (syncRoot)
            {
                timestamp = GetTimestamp();
                if (timestamp == lastTimestamp)
                {
                    sequence = (sequence + 1) & sequenceMask;
                    if (sequence == 0)
                    {
                        timestamp = GetNextTimestamp(lastTimestamp);
                    }
                }
                else
                {
                    sequence = 0;
                }
                if (timestamp < lastTimestamp)
                {
                    throw new Exception("时间戳比上一次生成ID时时间戳还小，故异常");
                }
                lastTimestamp = timestamp;
                return GetDateTime(timestamp).ToString("yyyyMMddHHmmss") + ((machineId << machineIdShift) | sequence).ToString("D4");
            }
        }
    }
}
