/**************************************************************************************************
 * 作    者：xujun         创始时间：2015-04-27                                              *
 * 修 改 人：                   修改时间：                                                        *
 * 描    述：  公共转换实现类，包括JSON与对象的相互转换，对象间的转换，业务数据的转换等等         *
 **************************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Jinher.AMP.BTP.Common
{
    /// <summary>
    /// 公共转换实现类，包括JSON与对象的相互转换，对象间的转换，业务数据的转换等等
    /// </summary>
    /// <remarks>
    /// 模块编号：mes_om_common_class_util
    /// 作    者：jinyu.qiao
    /// 创建时间：2015-04-27
    /// 修改编号：1         “初始为1每次修改加1”
    /// 描    述：
    /// </remarks>
    public static class CommonUtil
    {
        /// <summary>
        /// 两个实体相互赋值使用，不剔除相关创建人六个字段的方法
        /// add by jinyu.qiao：2015-04-27 
        /// </summary>
        /// <typeparam name="TC">当前使用实体类型</typeparam>
        /// <typeparam name="TO">扩展实体类型</typeparam>
        /// <param name="useObject">当前使用实体</param>
        /// <param name="dataObject">扩展实体</param>
        /// <returns></returns>
        public static TC ReadObjectExchange<TC, TO>(TC useObject, TO dataObject)
        {
            PropertyInfo[] newPropertys = useObject.GetType().GetProperties();
            PropertyInfo[] oldPropertys = dataObject.GetType().GetProperties();
            foreach (PropertyInfo newProperty in newPropertys)
            {
                string newPName = newProperty.Name; //获取到要新类型赋值的属性名称；
                //在循环old实体，找到对应的该属性，把值赋予新的类型即可；
                foreach (PropertyInfo oldProperty in oldPropertys)
                {
                    if (newPName == oldProperty.Name && newProperty.CanWrite)
                    {
                        newProperty.SetValue(useObject, oldProperty.GetValue(dataObject, null), null);
                        break;
                    }
                }

            }
            return useObject;
        }

        /// <summary>
        /// 两个实体相互赋值使用(创建人、创建时间等字段不进行拷贝)
        /// add by jinyu.qiao：2015-04-27 
        /// </summary>
        /// <typeparam name="TC">当前使用实体类型</typeparam>
        /// <typeparam name="TO">扩展实体类型</typeparam>
        /// <param name="useObject">当前使用实体</param>
        /// <param name="dataObject">扩展实体</param>
        /// <returns></returns>
        public static TC ObjectExchange<TC, TO>(TC useObject, TO dataObject)
        {
            PropertyInfo[] newPropertys = useObject.GetType().GetProperties();
            PropertyInfo[] oldPropertys = dataObject.GetType().GetProperties();
            foreach (PropertyInfo newProperty in newPropertys)
            {
                string newPName = newProperty.Name; //获取到要新类型赋值的属性名称；
                //在循环old实体，找到对应的该属性，把值赋予新的类型即可；
                foreach (PropertyInfo oldProperty in oldPropertys)
                {
                    if (newPName.ToLower() == oldProperty.Name.ToLower())
                    {
                        if (oldProperty.Name.ToLower() == "createdby" ||
                            oldProperty.Name.ToLower() == "createdon" ||
                            oldProperty.Name.ToLower() == "createdusername" ||
                            oldProperty.Name.ToLower() == "modifiedby" ||
                            oldProperty.Name.ToLower() == "modifiedon" ||
                            oldProperty.Name.ToLower() == "modifiedusername")
                        {
                            continue;
                        }
                        newProperty.SetValue(useObject, oldProperty.GetValue(dataObject, null), null);
                        break;
                    }
                }

            }
            return useObject;
        }

        /// <summary>
        /// object对象转换成json字符串
        /// add by jinyu.qiao：2015-04-27 
        /// </summary>
        /// <param name="ob">对象</param>
        /// <returns></returns>
        public static string ObjToJson(Object ob)
        {
            var json = new DataContractJsonSerializer(ob.GetType());
            string szJson;
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    json.WriteObject(stream, ob);
                    szJson = Encoding.UTF8.GetString(stream.ToArray());
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
            return szJson;
        }

        /// <summary>
        /// object对象转换成json字符串
        /// add by jinyu.qiao：2015-04-27 
        /// </summary>
        /// <param name="ob">对象</param>
        /// <returns></returns>
        public static string ObjConvertToJson(Object ob)
        {
            var timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
            //这里使用自定义日期格式，默认是ISO8601格式
            string szJson = JsonConvert.SerializeObject(ob, Formatting.Indented, timeConverter);
            return szJson;
        }

        /// <summary>
        /// json字符串转换成object对象
        /// add by jinyu.qiao：2015-04-27 
        /// </summary>
        /// <param name="jsonString">json字符串 </param>
        /// <returns></returns>
        public static T JsonConvertToObj<T>(string jsonString)
        {
            var obj = JsonConvert.DeserializeObject<T>(jsonString);
            return obj;
        }

        /// <summary>
        /// JSON串转换成对象
        /// add by jinyu.qiao：2015-04-27 
        /// </summary>
        /// <param name="jsonString">jsonString</param>
        /// <returns>T对象</returns>
        public static T JsonToObj<T>(string jsonString)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(ms);
            }
        }

        /// <summary>
        /// 把IList转换成JSon串
        /// add by jinyu.qiao：2015-04-27 
        /// </summary>
        /// <returns></returns>
        public static string ListToJson<T>(IList<T> dataList) where T : class
        {
            if (dataList == null || dataList.Count == 0)
            {
                return string.Empty;
            }
            //定义数据类型
            Type type = typeof(T);
            const BindingFlags bf =
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
            //反射标识 　　 
            PropertyInfo[] proInfo = type.GetProperties(bf);
            //获取T的属性 　　 
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.Formatting = Formatting.Indented;
                foreach (PropertyInfo info in proInfo)
                //遍历对象属性 　　　　
                {
                    jsonWriter.WriteStartObject();
                    jsonWriter.WritePropertyName(info.Name);
                    jsonWriter.WriteStartArray();
                    foreach (T item in dataList)
                    //遍历每个对象 　　　　　　
                    {
                        object value = info.GetValue(item, null);
                        if (value != null)
                            jsonWriter.WriteValue(value.ToString());
                    }
                    jsonWriter.WriteEndArray();
                    jsonWriter.WriteEndObject();
                }
            }
            sw.Close();
            return sb.ToString();
        }

        /// <summary>
        /// 将正常的日期转换成unix日期时间戳格式
        /// add by jinyu.qiao：2015-04-27 
        /// </summary>
        /// <param name="dateTime">正常日期转换成的字符串格式如：yyyy-MM-dd HH:mm:ss</param>
        /// <returns>unix时间</returns>
        public static string ConvertToUnix(DateTime dateTime)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            DateTime dtNow = dateTime;
            TimeSpan toNow = dtNow.Subtract(dtStart);
            string timeStamp = Convert.ToString(toNow.Ticks);
            timeStamp = timeStamp.Substring(0, timeStamp.Length - 7);
            return timeStamp;
        }

        /// <summary>
        /// 将unix日期时间戳格式换成正常的日期转
        /// add by jinyu.qiao：2015-04-27 
        /// </summary>
        /// <param name="unitTime">unit格式时间</param>
        /// <returns>datetime时间</returns>
        public static DateTime ConvertToDateTime(string unitTime)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(unitTime + "0000000");
            var toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);

        }

        /// <summary>
        /// 获取单个枚举的描述信息
        /// add by jinyu.qiao：2015-04-27 
        /// </summary>
        /// <param name="targetObj"></param>
        /// <returns></returns>
        public static String GetDescription(Enum targetObj)
        {
            Type type = targetObj.GetType();
            MemberInfo[] memInfo = type.GetMember(targetObj.ToString());
            if (memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }
            return targetObj.ToString();

        }

        /// <summary>
        /// 获取整个枚举的集合
        /// add by jinyu.qiao：2015-04-27 
        /// </summary>
        /// <param name="targetObj"></param>
        /// <returns></returns>
        public static List<DictionaryEntry> GetDesCollection(Enum targetObj)
        {
            List<DictionaryEntry> list = new List<DictionaryEntry>();
            Dictionary<int, string> dic = GetDictionary(targetObj.GetType());
            if (dic != null)
            {
                foreach (KeyValuePair<int, string> keyValuePair in dic)
                {
                    var dictionaryEntry = new DictionaryEntry();
                    dictionaryEntry.Key = keyValuePair.Key;
                    dictionaryEntry.Value = keyValuePair.Value;
                    list.Add(dictionaryEntry);
                }
            }
            return list;
        }

        /// <summary>
        /// 根据枚举值获取描述
        /// add by jinyu.qiao：2015-04-27 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumDes<T>(int value)
        {
            Dictionary<int, string> dic;
            dic = GetDictionary(typeof(T));
            foreach (var key in dic.Keys)
            {
                if (key == value)
                    return dic[key];
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取枚举值、描述列表
        /// add by jinyu.qiao：2015-04-27 
        /// </summary>
        /// <param name="enumType">枚举类型</param>                
        /// <returns>
        /// 返回枚举值、描述列表
        /// </returns>
        private static Dictionary<int, string> GetDictionary(Type enumType)
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            foreach (int enumValue in Enum.GetValues(enumType))
            {
                dic.Add(enumValue, string.Empty);
                FieldInfo fieldinfo = enumType.GetField(Enum.GetName(enumType, enumValue));
                if (fieldinfo == null)
                {
                    return null;
                }
                Object[] objs = fieldinfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (objs.Length != 0)
                {
                    DescriptionAttribute da = (DescriptionAttribute)objs[0];
                    dic[enumValue] = da.Description;
                }
            }
            return dic;
        }

        /// <summary>
        /// 把枚举描述和值规则拼接字符串
        /// add by jinyu.qiao：2015-04-27 
        /// </summary>
        /// <typeparam name="T">枚举</typeparam>
        /// <returns>枚举值,枚举描述;枚举值,枚举描述;枚举值,枚举描述</returns>
        public static IList<DictionaryEntry> GetEnumList<T>()
        {
            List<DictionaryEntry> list = new List<DictionaryEntry>();
            Dictionary<int, string> dic = GetDictionary(typeof(T));
            DictionaryEntry entity;
            foreach (var key in dic.Keys)
            {
                entity = new DictionaryEntry();
                entity.Key = key;
                entity.Value = dic[key];
                list.Add(entity);
            }
            return list;
        }

        /// <summary>
        /// 字符串转金额类型
        /// </summary>
        /// <param name="str">12位数  以分为单位，右靠齐，前补0</param>
        /// <returns></returns>
        public static decimal ConvertToDecimal(string str)
        {
            string str1 = str.TrimStart('0');//去掉左边的0
            if (str1.Length >= 3)
            {
                string str4 = str1.Insert(str1.Length - 2, ".");
                return decimal.Parse(str4);
            }
            else
            {
                string str2 = str1.PadLeft(3, '0');
                string str3 = str2.Insert(str2.Length - 2, ".");
                return decimal.Parse(str3);
            }
        }

        /// <summary>
        /// 生成4位随机数
        /// </summary>
        public static string CreateValidateCode()
        {
            string vc = "";
            Random rNum = new Random();//随机生成类
            int num1 = rNum.Next(0, 9);//返回指定范围内的随机数
            int num2 = rNum.Next(0, 9);
            int num3 = rNum.Next(0, 9);
            int num4 = rNum.Next(0, 9);
            int[] nums = new int[4] { num1, num2, num3, num4 };
            for (int i = 0; i < nums.Length; i++)//循环添加四个随机生成数
            {
                vc += nums[i].ToString();
            }
            return vc;
        }
    }
}
