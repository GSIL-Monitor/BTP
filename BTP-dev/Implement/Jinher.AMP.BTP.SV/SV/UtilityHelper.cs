using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Json;

namespace Jinher.AMP.BTP.SV
{
    /// <summary>
    /// 工具类
    /// </summary>
    public class UtilityHelper
    {

        #region json序列化
        /// <summary>
        /// 将object类型的对象序列化为json字符串。
        /// </summary>
        /// <typeparam name="T">要序列化的对象的类型</typeparam>
        /// <param name="t">要序列化的对象</param>
        /// <returns>序列化后的字符串</returns>
        public static string ToJson<T>(T t)
        {
            DataContractJsonSerializer ds = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();
            ds.WriteObject(ms, t);

            string strReturn = Encoding.UTF8.GetString(ms.ToArray());
            ms.Close();
            return strReturn;
        }

        /// <summary>
        /// 将json字符串反序列化为对象.
        /// </summary>
        /// <typeparam name="T">目标对象的类型</typeparam>
        /// <param name="strJson">要反序列化的json字符串.</param>
        /// <returns>反序列化后的对象</returns>
        public static T FromJson<T>(string strJson) where T : class
        {
            DataContractJsonSerializer ds = new DataContractJsonSerializer(typeof(T));
            //string s = Regex.Replace(strJson, @"\\{1,}'", @"\\'");
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(strJson));

            return ds.ReadObject(ms) as T;
        }
        #endregion
    }
}
