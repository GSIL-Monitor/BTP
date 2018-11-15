using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Jinher.JAP.Cache.Configuration;
using Jinher.JAP.Common.Loging;
using ServiceStack.Redis;
using System.Configuration;

namespace Jinher.AMP.BTP.Common
{
    public static class RedisHelper
    {
        /// <summary>
        /// 添加到哈希表中
        /// </summary>
        /// <param name="hashId">哈希表ID</param>
        /// <param name="key">哈希表Key</param>
        /// <param name="value">值</param>
        /// <param name="updateIfExists">如果key存在是否更新</param>
        public static bool AddHash(string hashId, string key, string value, bool updateIfExists = true)
        {
            bool result = false;
            try
            {
                using (IRedisClient redis = RedisManager.GetClient())
                {
                    if (updateIfExists)
                    {
                        LogHelper.Debug("添加到哈希表中AddHash-SetEntryInHash");
                        result = redis.SetEntryInHash(hashId, key, value);
                    }
                    else
                    {
                        LogHelper.Debug("添加到哈希表中AddHash-SetEntryInHashIfNotExists");
                        result = redis.SetEntryInHashIfNotExists(hashId, key, value);
                    }
                    LogHelper.Debug("添加到哈希表中AddHash，hashId：" + hashId + ",key=" + key + ",value=" + value + ",updateIfExists=" + updateIfExists + ",result=" + result);
                    redis.Dispose();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("添加到哈希表中AddHash异常", ex);
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hashId"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetHash(string hashId)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            using (IRedisClient redis = RedisManager.GetClient())
            {
                dict = redis.GetAllEntriesFromHash(hashId);

                redis.Dispose();
            }
            return dict;
        }

        /// <summary>
        /// 添加到哈希表中
        /// </summary>
        /// <param name="hashId">哈希表ID</param>
        /// <param name="key">哈希表Key</param>
        /// <param name="value">值</param>
        /// <param name="updateIfExists">如果key存在是否更新</param>
        public static bool AddHash<T>(string hashId, string key, T value, bool updateIfExists = true)
        {
            using (IRedisClient redis = RedisManager.GetClient())
            {
                return AddHash(hashId, key, JsonHelper.JsonSerializer<T>(value));
            }
        }

        /// <summary>
        /// 获取整个hash的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashId"></param>
        /// <returns></returns>
        public static List<T> GetHashValues<T>(string hashId)
        {

            using (IRedisClient redis = RedisManager.GetClient())
            {
                List<T> result = new List<T>();

                List<string> list = redis.GetHashValues(hashId);

                if (list != null && list.Count > 0)
                {
                    foreach (var temp in list)
                    {
                        if (!string.IsNullOrEmpty(temp))
                        {
                            result.Add(JsonHelper.JsonDeserialize<T>(temp));
                        }
                    }
                }
                return result;
            }
        }
        /// <summary>
        /// 获取整个hash的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashId"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetHashValue<T>(string hashId, string key)
        {
            if (string.IsNullOrEmpty(hashId) || string.IsNullOrEmpty(key))
                return default(T);
            using (IRedisClient redis = RedisManager.GetClient())
            {
                string temp = redis.GetValueFromHash(hashId, key);
                if (!string.IsNullOrEmpty(temp))
                {
                    return JsonHelper.JsonDeserialize<T>(temp);
                }

                return default(T);
            }
        }
        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        /// <param name="hashId"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool RemoveHash(string hashId, string key)
        {
            using (IRedisClient redis = RedisManager.GetClient())
            {
                return redis.RemoveEntryFromHash(hashId, key);
            }
        }
        /// <summary>
        /// 删除hash表
        /// </summary>
        /// <param name="hashId"></param>
        /// <returns></returns>
        public static bool RemoveHash(string hashId)
        {
            using (IRedisClient redis = RedisManager.GetClient())
            {
                return redis.Remove(hashId);
            }
        }

        /// <summary>
        /// 判断某个数据是否已经被缓存
        /// </summary>
        /// <param name="hashId"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool ExistHash(string hashId, string key)
        {
            using (IRedisClient redis = RedisManager.GetClient())
            {
                return redis.HashContainsEntry(hashId, key);
            }
        }
        /// <summary>
        /// 返回Hash中所有的Keys 
        /// </summary>
        /// <param name="hashId"></param>
        public static List<string> GetHashKeys(string hashId)
        {
            using (IRedisClient redis = RedisManager.GetClient())
            {
                return redis.GetHashKeys(hashId);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<string> SearchKeys(string key)
        {
            using (IRedisClient redis = RedisManager.GetClient())
            {
                return redis.SearchKeys(key);
            }
        }
        public static object Get(string key)
        {
            using (IRedisClient redis = RedisManager.GetClient())
            {
                return redis.GetValue(key);
            }
        }

        public static void Set<T>(string key, T value)
        {
            using (IRedisClient redis = RedisManager.GetClient())
            {
                redis.Set(key, value);
            }
        }
        public static void Set<T>(string key, T value, TimeSpan expiresIn)
        {
            using (IRedisClient redis = RedisManager.GetClient())
            {
                redis.Set(key, value, expiresIn);
            }
        }

        public static T Get<T>(string key)
        {
            using (IRedisClient redis = RedisManager.GetClient())
            {
                return redis.Get<T>(key);
            }
        }
        public static List<string> SearcheKeys(string key)
        {
            using (IRedisClient redis = RedisManager.GetClient())
            {
                return redis.SearchKeys(key);
            }
        }
        public static long Incr(string key)
        {
            return Incr(key, 1);
        }
        public static long Incr(string key, long amount)
        {
            using (IRedisClient redis = RedisManager.GetClient())
            {
                return redis.IncrementValueBy(key, amount);
            }
        }
        public static T BlockingPopItemFromList<T>(string listId, TimeSpan? timSpan)
        {
            var str = BlockingPopItemFromList(listId, timSpan);
            return JsonHelper.JsonDeserialize<T>(str);
        }
        public static string BlockingPopItemFromList(string listId, TimeSpan? timSpan)
        {
            string result = null;
            using (IRedisClient redis = RedisManager.GetClient())
            {
                result = redis.BlockingPopItemFromList(listId, timSpan);
                redis.Dispose();
            }
            return result;
        }
        public static string BlockingPopItemFromList(string listId)
        {
            return BlockingPopItemFromList(listId, null);
        }
        public static T BlockingPopItemFromList<T>(string listId)
        {
            return BlockingPopItemFromList<T>(listId, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tuples">
        /// <para>Item1：活动Id</para>
        /// <para>Item2：商品Id</para>
        /// <para>Item3：购买数量</para>
        /// </param>
        /// <param name="userId">下订单用户</param>
        /// <returns>
        /// <para>Item1：活动Id</para>
        /// <para>Item2：商品Id</para>
        /// <para>Item3：购买数量</para>
        /// <para>Item4：本次购买后，当前用户购买数量</para>
        /// </returns>
        public static List<Tuple<string, string, long, long>> ListHIncr(List<Tuple<string, string, int>> tuples, Guid userId)
        {
            List<Tuple<string, string, long, long>> result = new List<Tuple<string, string, long, long>>();
            if (tuples != null && tuples.Any())
            {
                using (IRedisClient redis = RedisManager.GetClient())
                {
                    List<long> sussessList = new List<long>();
                    List<Exception> errList = new List<Exception>();

                    List<long> sussessUlList = new List<long>();
                    List<Exception> errUlList = new List<Exception>();
                    List<long> sussessPUlList = new List<long>();
                    List<Exception> errPUlList = new List<Exception>();
                    Dictionary<string, int> dictPro = new Dictionary<string, int>();
                    int cnt = 0;
                    using (var pineline = redis.CreatePipeline())
                    {
                        for (int i = 0; i < tuples.Count; i++)
                        {
                            //pineline.QueueCommand 第二、三个参数是action回调，此处必须在for里声明cur,或者学习js闭包做法：
                            //m=>new Action<int>((j) =>
                            //   {
                            //        sussessList[j] = m;
                            //   })(cur)
                            int cur = i;
                            sussessList.Add(0);
                            sussessUlList.Add(0);
                            errList.Add(null);
                            errUlList.Add(null);
                            Tuple<string, string, int> tmpTuple = tuples[cur];
                            var promotionId = tmpTuple.Item1;  //活动Id
                            var proCommodityId = tmpTuple.Item2;  //商品Id
                            var orderCommodityCount = tmpTuple.Item3;  //购买数量 

                            pineline.QueueCommand(c => c.IncrementValueInHash(RedisKeyConst.ProSaleCountPrefix + promotionId, proCommodityId, orderCommodityCount), m => sussessList[cur] = m, e => errList[cur] = e);
                            pineline.QueueCommand(c => c.IncrementValueInHash(RedisKeyConst.UserLimitPrefix + promotionId + ":" + proCommodityId, userId.ToString(), orderCommodityCount), m => sussessUlList[cur] = m, e => errUlList[cur] = e);
                        }
                        dictPro = tuples.GroupBy(c => c.Item1).ToDictionary(x => x.Key, y => y.Sum(m => m.Item3));
                        cnt = 0;
                        foreach (var item in dictPro)
                        {
                            var xy = cnt;
                            sussessPUlList.Add(0);
                            errPUlList.Add(null);

                            pineline.QueueCommand(c => c.IncrementValueInHash(RedisKeyConst.UserPromotionLimitPrefix + item.Key, userId.ToString(), item.Value), m => sussessPUlList[xy] = m, e => errPUlList[xy] = e);
                            cnt++;
                        }
                        pineline.Flush();
                    }
                    bool hasError = errList.Count(c => c != null) > 0 || errUlList.Count(c => c != null) > 0 || errPUlList.Count(c => c != null) > 0;
                    if (hasError)
                    {
                        for (int i = 0; i < tuples.Count; i++)
                        {
                            if (errList[i] == null)
                            {
                                Tuple<string, string, int> tmpTuple = tuples[i];
                                var promotionId = tmpTuple.Item1;  //活动Id
                                var proCommodityId = tmpTuple.Item2;  //商品Id
                                var orderCommodityCount = tmpTuple.Item3;  //购买数量
                                redis.IncrementValueInHash(RedisKeyConst.ProSaleCountPrefix + promotionId, proCommodityId, -orderCommodityCount);
                            }

                            if (errUlList[i] == null)
                            {
                                Tuple<string, string, int> tmpTuple = tuples[i];
                                var promotionId = tmpTuple.Item1;  //活动Id
                                var proCommodityId = tmpTuple.Item2;  //商品Id
                                var orderCommodityCount = tmpTuple.Item3;  //购买数量
                                redis.IncrementValueInHash(RedisKeyConst.UserLimitPrefix + promotionId + ":" + proCommodityId, userId.ToString(), -orderCommodityCount);
                            }

                        }
                        cnt = 0;
                        foreach (var item in dictPro)
                        {
                            if (errPUlList[cnt] == null)
                            {
                                redis.IncrementValueInHash(RedisKeyConst.UserPromotionLimitPrefix + item.Key, userId.ToString(), -item.Value);
                            }
                            cnt++;
                        }
                        return result;
                    }
                    for (int i = 0; i < sussessList.Count; i++)
                    {
                        result.Add(new Tuple<string, string, long, long>(tuples[i].Item1, tuples[i].Item2, sussessList[i], sussessUlList[i]));
                    }
                }
            }
            return result;
        }

        public static bool HashContainsEntry(string hashId, string key)
        {
            using (IRedisClient redis = RedisManager.GetClient())
            {
                return redis.HashContainsEntry(hashId, key);
            }
        }
        /// <summary>
        /// 批量获取hash的key-values
        /// </summary>
        /// <param name="hashId"></param>
        /// <param name="keyList"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetHashInfoList(string hashId, List<string> keyList)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (keyList == null || !keyList.Any())
                return result;

            using (IRedisClient redis = RedisManager.GetClient())
            {
                var resultList = redis.GetValuesFromHash(hashId, keyList.ToArray());
                if (resultList == null || resultList.Count != keyList.Count)
                    return result;
                for (int i = 0; i < keyList.Count; i++)
                {
                    if (!result.ContainsKey(keyList[i]))
                        result.Add(keyList[i], resultList[i]);
                }

            }
            return result;
        }
        /// <summary>
        /// 批量获取hash的key-values
        /// </summary>
        /// <param name="hashId"></param>
        /// <param name="keyList"></param>
        /// <returns></returns>
        public static Dictionary<string, T> GetHashInfoList<T>(string hashId, List<string> keyList)
        {
            Dictionary<string, T> result = new Dictionary<string, T>();
            if (keyList == null || !keyList.Any())
                return result;

            using (IRedisClient redis = RedisManager.GetClient())
            {
                var resultList = redis.GetValuesFromHash(hashId, keyList.ToArray());
                if (resultList == null || resultList.Count != keyList.Count)
                    return result;
                for (int i = 0; i < keyList.Count; i++)
                {
                    if (!result.ContainsKey(keyList[i]))
                        result.Add(keyList[i], JsonHelper.JsonDeserialize<T>(resultList[i]));
                }
            }
            return result;
        }
        /// <summary>
        /// 批量设置hash的 key-values
        /// </summary>
        /// <param name="hashId"></param>
        /// <param name="keyValuePairs"></param>
        public static void SetRangeInHash(string hashId, IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            var valuePairs = keyValuePairs as KeyValuePair<string, string>[] ?? keyValuePairs.ToArray();
            if (!valuePairs.Any())
                return;
            using (IRedisClient redis = RedisManager.GetClient())
            {
                redis.SetRangeInHash(hashId, valuePairs);
            }
        }
        /// <summary>
        /// 批量设置hash的 key-values
        /// </summary>
        /// <param name="hashId"></param>
        /// <param name="keyValuePairs"></param>
        public static void SetRangeInHash<T>(string hashId, IEnumerable<KeyValuePair<string, T>> keyValuePairs)
        {
            if (!keyValuePairs.Any())
                return;
            List<KeyValuePair<string, string>> valuePairs = new List<KeyValuePair<string, string>>();
            foreach (var item in keyValuePairs)
            {
                valuePairs.Add(new KeyValuePair<string, string>(item.Key, JsonHelper.JsonSerializer(item.Value)));
            }
            using (IRedisClient redis = RedisManager.GetClient())
            {
                redis.SetRangeInHash(hashId, valuePairs);
            }
        }
        public static void Remove(string key)
        {
            if (string.IsNullOrEmpty(key))
                return;
            using (IRedisClient redis = RedisManager.GetClient())
            {
                redis.Remove(key);
            }
        }

        /// <summary>
        /// 列表追加
        /// </summary>
        /// <param name="listId">列表Id</param>
        /// <param name="value"></param>
        public static bool AddItemToList<T>(string listId, T value)
        {
            var str = JsonHelper.JsonSerializer(value);
            return AddItemToList(listId, str);
        }

        /// <summary>
        /// 列表追加
        /// </summary>
        /// <param name="listId">列表Id</param>
        /// <param name="value"></param>
        public static bool AddItemToList(string listId, string value)
        {
            using (IRedisClient redis = RedisManager.GetClient())
            {
                redis.AddItemToList(listId, value);
                redis.Dispose();
            }
            return true;
        }
        /// <summary>
        /// 列表插入
        /// </summary>
        /// <param name="listId">列表Id</param>
        /// <param name="value"></param>
        public static bool PrependItemToList<T>(string listId, T value)
        {
            var str = JsonHelper.JsonSerializer(value);
            return PrependItemToList(listId, str);
        }

        /// <summary>
        /// 列表插入
        /// </summary>
        /// <param name="listId">列表Id</param>
        /// <param name="value"></param>
        public static bool PrependItemToList(string listId, string value)
        {
            using (IRedisClient redis = RedisManager.GetClient())
            {
                redis.PrependItemToList(listId, value);
                redis.Dispose();
            }
            return true;
        }
        public static long HashIncr(string hashId, string key)
        {
            return HashIncr(hashId, key, 1);
        }
        public static long HashIncr(string hashId, string key, long amount)
        {
            using (IRedisClient redis = RedisManager.GetClient())
            {
                return (long)redis.IncrementValueInHash(hashId, key, amount);

            }
        }

    }

    /// <summary>
    /// MemCache管理操作类
    /// </summary>
    public sealed class RedisManager
    {

        private static IRedisClientsManager redis;

        /// <summary>
        /// 静态构造方法，初始化链接池管理对象
        /// </summary>
        static RedisManager()
        {
            CreateManager();
        }


        /// <summary>
        /// 创建链接池管理对象
        /// </summary>
        private static void CreateManager()
        {
            var settings = RedisCacheSection.GetRedisCacheSettings();
            if (settings == null || !settings.Any())
                return;
            RedisCacheSetting redisCacheSetting = settings.FirstOrDefault((RedisCacheSetting c) => c.Key == "BTPCache");
            if (redisCacheSetting == null)
                return;
            string[] writeList = new[] { redisCacheSetting.Host };
            string[] readList = new[] { redisCacheSetting.Host };
            int num = string.IsNullOrEmpty(ConfigurationManager.AppSettings["RedisPoolSize"]) ? 60 : System.Convert.ToInt32(ConfigurationManager.AppSettings["RedisPoolSize"]);
            int value = string.IsNullOrEmpty(ConfigurationManager.AppSettings["PoolTimeOutSecond"]) ? 20 : System.Convert.ToInt32(ConfigurationManager.AppSettings["PoolTimeOutSecond"]);

            RedisClientManagerConfig redisClientManagerConfig = new RedisClientManagerConfig();
            redisClientManagerConfig.MaxWritePoolSize = num;
            redisClientManagerConfig.MaxReadPoolSize = num;
            redisClientManagerConfig.AutoStart = true;
            //redis = new BasicRedisClientManager(readList, writeList);
            redis = new PooledRedisClientManager(readList, writeList, redisClientManagerConfig)
            {
                PoolTimeout = 2000,
                ConnectTimeout = 500
            };
        }



        /// <summary>
        /// 客户端缓存操作对象
        /// </summary>
        public static IRedisClient GetClient()
        {
            if (redis == null)
            {
                CreateManager();
            }

            return redis.GetClient();
        }
    }
    /// <summary>
    /// redis相关key
    /// 框架要求key必须以"G_"开头
    /// </summary>
    public class RedisKeyConst
    {
        /// <summary>
        /// app的应用主类型
        /// <para>数据类型：Hash</para>
        /// <para>HashId：G_AppOwnerType</para>
        /// <para>key：AppId</para>
        /// <para>value： <see cref="Jinher.AMP.App.Deploy.CustomDTO.AppIdOwnerIdTypeDTO"/></para>
        /// </summary>
        public const string AppOwnerType = "G_AppOwnerType";
        /// <summary>
        /// app相关信息
        /// <para>数据类型：Hash</para>
        /// <para>HashId：G_AppNameIcon</para>
        /// <para>key：AppId</para>
        /// <para>value： <see cref="Jinher.AMP.App.Deploy.CustomDTO.AppIdNameIconDTO"/></para>
        /// </summary>
        public const string AppNameIcon = "G_AppNameIcon";
        /// <summary>
        /// app相关信息
        /// <para>数据类型：Hash</para>
        /// <para>HashId：G_AppInfo</para>
        /// <para>key：AppId</para>
        /// <para>value： <see cref="Jinher.AMP.App.Deploy.CustomDTO.ApplicationDTO"/></para>
        /// </summary>
        public const string AppInfo = "G_AppInfo";
        /// <summary>
        /// app相关信息
        /// <para>数据类型：Hash</para>
        /// <para>HashId：G_AppInZPH</para>
        /// <para>key：AppId</para>
        /// <para>value： 0(不在正品会中)；1(在正品会中)</para>
        /// </summary>
        public const string AppInZPH = "G_AppInZPH";
        /// <summary>
        /// <para>活动商品用户购买数量</para>
        /// <para>数据类型：Hash</para>
        /// <para>HashId组成：  G_UL:活动Id:商品Id</para>
        /// <para>key：用户Id</para>
        /// <para>value：int</para>
        /// </summary>
        public const string UserLimitPrefix = "G_UL:";
        /// <summary>
        /// <para>活动商品销量</para>
        /// <para>数据类型：Hash</para>
        /// <para>HashId组成：  G_UL:活动Id</para>
        /// <para>key：商品Id</para>
        /// <para>value：int</para>
        /// </summary>
        public const string ProSaleCountPrefix = "G_ProSaleCount:";

        /// <summary>
        /// <para>活动用户购买数量</para>
        /// <para>数据类型：Hash</para>
        /// <para>HashId组成：  G_PUL:活动Id</para>
        /// <para>key：用户Id</para>
        /// <para>value：int</para>
        /// </summary>
        public const string UserPromotionLimitPrefix = "G_PUL:";

        /// <summary>
        /// <para>行为记录js网址</para>
        /// <para>数据类型：string</para>
        /// <para>key：  G_BehaviorRecordUrl</para>
        /// <para>value：string</para>
        /// </summary>
        public const string BehaviorRecordUrl = "G_BehaviorRecordUrl";
        /// <summary>
        /// <para>提货码</para>
        /// <para>数据类型：string</para>
        /// <para>key：  G_PickUpCode</para>
        /// <para>value：int</para>
        /// </summary>
        public const string PickUpCode = "G_PickUpCode";
        /// <summary>
        /// <para>队列Id</para>
        /// <para>数据类型：string</para>
        /// <para>key：  G_QuequeId</para>
        /// <para>value：int</para>
        /// </summary>
        public const string QueTaskId = "G_QueTaskId";
        /// <summary>
        /// <para>队列异常计数器</para>
        /// <para>数据类型：Hash</para>
        /// <para>HashId组成：  G_QuequeErrCount</para>
        /// <para>key：消息队列Id:任务Id</para>
        /// <para>value：int</para>
        /// </summary>
        public const string QuequeErrCount = "G_QuequeErrCount";
        /// <summary>
        /// <para>订单锁</para>
        /// <para>数据类型：Hash</para>
        /// <para>HashId组成：  G_QuequeErrCount</para>
        /// <para>key：订单Id</para>
        /// <para>value：int，目前固定为1</para>
        /// </summary>
        public const string OrderLock = "G_OrderLock";
        /// <summary>
        /// <para>订单流水号</para>
        /// <para>数据类型：Hash</para>
        /// <para>HashId组成：  G_OrderBatch:yyyyMMdd</para>
        /// <para>key：appId</para>
        /// <para>value：序号</para>
        /// </summary>
        public const string OrderBatch = "G_OrderBatch";

        /// <summary>
        /// <para>IP地址对应的省份</para>
        /// </summary>
        public const string IpRegion = "G_IpRegion";

        /// <summary>
        /// <para>按京东eclp系统标准导出订单上次导出时间</para>
        /// <para>数据类型：string</para>
        /// <para>key：  G_ExportOrderForJDLastTime</para>
        /// <para>value：DateTime</para>
        /// </summary>
        public const string ExportOrderForJDLastTime = "G_ExportOrderForJDLastTime";

        /// <summary>
        /// <para>用户下京东订单后，返回京东订单父Id 存储</para>
        /// <para>数据类型：Hash</para>
        /// <para>HashId:   G_UserOrderJdPOrderIdList</para>
        /// <para>key：用户Id</para>
        /// <para>value：下单后返回的京东父Id</para>
        /// </summary>
        public const string UserOrder_JdPOrderIdList = "G_UserOrderJdPOrderIdList";
    }

}

