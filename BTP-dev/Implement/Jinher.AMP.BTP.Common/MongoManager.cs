using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using MongoDB.Driver;

namespace Jinher.AMP.BTP.Common
{
    /// <summary>
    /// MongoDB工具类
    /// </summary>
    public static class MongoCache
    {
        /// <summary>
        /// 保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="colName"></param>
        /// <param name="obj"></param>
        public static void Save<T>(string colName, T obj)
        {
            var mongo = MongoManager.getDB();
            var collection = mongo.GetCollection(colName);
            collection.Save(typeof(T), obj);

        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="colName"></param>
        /// <param name="list"></param>
        public static void Save<T>(string colName, IEnumerable<T> list)
        {
            var enumerable = list as T[] ?? list.ToArray();
            if (list != null && enumerable.Any())
            {
                var mongo = MongoManager.getDB();
                var collection = mongo.GetCollection(colName);
                foreach (var item in enumerable)
                {
                    collection.Save(item);
                }

            }
        }

        /// <summary>
        /// 根据文档名称获取文档
        /// </summary>
        /// <typeparam name="T">文档类型</typeparam>
        /// <param name="colName">文档名称</param>
        /// <returns>文档所有数据</returns>
        public static List<T> Get<T>(string colName)
        {
            return Get<T>(colName, null, null);
        }
        /// <summary>
        /// 根据文档名称、查询条件获取文档
        /// </summary>
        /// <typeparam name="T">文档类型</typeparam>
        /// <param name="colName">文档名称</param>
        /// <param name="query">查询条件</param>
        /// <returns>文档符合条件所有数据</returns>
        public static List<T> Get<T>(string colName, IMongoQuery query)
        {
            return Get<T>(colName, query, null);
        }
        /// <summary>
        /// 根据文档名称、查询条件获取文档指定列的数据
        /// </summary>
        /// <typeparam name="T">文档类型</typeparam>
        /// <param name="colName">文档名称</param>
        /// <param name="query">查询条件</param>
        /// <param name="fields">列列表</param>
        /// <returns>文档符合条件所有数据</returns>
        public static List<T> Get<T>(string colName, IMongoQuery query, IMongoFields fields)
        {
            var mongo = MongoManager.getDB();
            var collection = mongo.GetCollection<T>(colName);
            return collection.FindAs<T>(query).SetFields(fields).ToList();
        }

        /// <summary>
        /// 根据文档名称更新指定内容
        /// </summary>
        /// <param name="colName">文档名称</param>
        /// <param name="query">查询条件</param>
        /// <param name="update">更新内容</param>
        /// <typeparam name="T">文档类型</typeparam>
        /// <returns></returns>
        public static bool Update<T>(string colName, IMongoQuery query, IMongoUpdate update)
        {
            var mongo = MongoManager.getDB();
            var collection = mongo.GetCollection<T>(colName);

            var result = collection.Update(query, update);
            return result.Ok;
        }


        /// <summary>
        /// 根据文档名称更新指定内容
        /// </summary>
        /// <param name="colName">文档名称</param>
        /// <param name="query">查询条件</param>
        /// <param name="update">更新内容</param>
        /// <param name="updateFlags">更新类型(upsert：如果不存在插入)</param>
        /// <typeparam name="T"></typeparam>
        public static void Update<T>(string colName, IMongoQuery query, IMongoUpdate update, UpdateFlags updateFlags)
        {
            var mongo = MongoManager.getDB();
            var collection = mongo.GetCollection<T>(colName);
            collection.Update(query, update, updateFlags);
        }

    }



    /// <summary>
    /// 
    /// </summary>
    public class MongoManager
    {
        private static string dbName;
        private static MongoServer mongo;

        private MongoManager()
        {

        }

        /**
         * 根据名称获取DB，相当于是连接
         * 
         * @param dbName
         * @return
         */
        public static MongoDatabase getDB()
        {
            if (mongo == null)
            {
                // 初始化
                init();
            }
            //MongoServerInstanceType.
            return mongo.GetDatabase(dbName);
        }

        /**
         * 初始化连接池，设置参数。
         */
        private static void init()
        {
            var connectString = ConfigurationManager.ConnectionStrings["MongoServerSettings"].ConnectionString;
            MongoUrl cnn = new MongoUrl(connectString);
            mongo = new MongoServer(cnn.ToServerSettings());
            dbName = cnn.DatabaseName;
        }
    }

    /// <summary>
    /// Mongo相关常量
    /// </summary>
    public class MongoKeyConst
    {
        public const string CollectionName = "Store";
    }
}
