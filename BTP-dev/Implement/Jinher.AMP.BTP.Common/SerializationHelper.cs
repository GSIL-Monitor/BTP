using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Jinher.JAP.Common.Loging;
using Newtonsoft.Json;

namespace Jinher.AMP.BTP.Common
{
    /// <summary>
    /// 序列化类
    /// </summary>
    public class SerializationHelper
    {
        /// <summary>
        /// JSON序列化
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>json字符串</returns>
        public static string JsonSerialize(Object obj)
        {
            try
            {
                return JsonConvert.SerializeObject(obj);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
            }
            return string.Empty;
        }

        /// <summary>
        /// JSON反序列化
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="jsonString">json字符串</param>
        /// <returns>对象</returns>
        public static T JsonDeserialize<T>(string jsonString)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(jsonString);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
            }
            return default(T);
        }

        /// <summary>
        /// JSON序列化(时间字段返回格式为/Date(1410019200000+0800))/)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string JsonSerializeObjectForTime(Object obj)
        {
            string jsonString = string.Empty;
            MemoryStream ms = new MemoryStream();
            try
            {
                DataContractJsonSerializer dcjs = new DataContractJsonSerializer(obj.GetType());
                dcjs.WriteObject(ms, obj);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
            }
            finally
            {
                ms.Close();
                ms.Dispose();
            }
            return jsonString;
        }

        /// <summary>
        /// JSON序列化(时间字段返回格式为/Date(1410019200000+0800))/)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string JsonSerializeForTime<T>(T t)
        {
            string jsonString = string.Empty;
            MemoryStream ms = new MemoryStream();
            try
            {
                DataContractJsonSerializer dcjs = new DataContractJsonSerializer(typeof(T));
                dcjs.WriteObject(ms, t);
                jsonString = Encoding.UTF8.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
            }
            finally
            {
                ms.Close();
                ms.Dispose();
            }
            return jsonString;
        }
    }
}