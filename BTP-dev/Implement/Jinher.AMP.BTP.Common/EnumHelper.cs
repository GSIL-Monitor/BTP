using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Jinher.AMP.BTP.Common
{
    /// <summary>
    /// 枚举辅助类
    /// </summary>
    public  class EnumHelper
    {
        /// <summary>
        /// 根据枚举值，获取枚举名称
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public  string GetName<T>(T value)
        {
            return System.Enum.GetName(typeof(T), value);
        }

        /// <summary>
        /// 获取单个枚举的描述信息
        /// </summary>
        /// <param name="targetObj"></param>
        /// <returns></returns>
        public  String GetDescription(Enum targetObj)
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
        /// 根据枚举值，获取枚举名称
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public  string GetName<T>(int value)
        {
            return System.Enum.GetName(typeof(T), value);
        }

        /// <summary>
        /// 根据枚举值，获取枚举的描述信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public  string GetDepict<T>(int value)
        {
            var enumType = typeof(T);
            var enumItem = (T)Enum.ToObject(enumType, value);

            return GetDepict(enumItem);
        }

        /// <summary>
        /// 根据枚举值，获取枚举的描述信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetDepict<T>(T value)
        {
            var enumType = typeof(T);
            var enumName = value.ToString();

            DescriptionAttribute attr = null;

            FieldInfo fieldInfo = enumType.GetField(enumName);
            if (fieldInfo != null)
            {
                attr = Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute), false)
                    as DescriptionAttribute;
            }

            var depict = attr == null ? enumName : attr.Description;
            return depict;
        }

        /// <summary>
        /// 获取枚举所有值的KeyValuePair<int, string>列表 key是枚举值，value是枚举名称
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public  List<KeyValuePair<int, string>> GetEnumEnglishDepicts<T>()
        {
            return GetEnumItems<T>().Select(x => new KeyValuePair<int, string>(x.Value, x.Name)).ToList();
        }

        /// <summary>
        /// 获取枚举所有值的KeyValuePair(int, string)列表 key是枚举值，value是中文描述
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public  List<KeyValuePair<int, string>> GetEnumChineseDepicts<T>()
        {
            return GetEnumItems<T>().Select(x => new KeyValuePair<int, string>(x.Value, x.Depict)).ToList();
        }

        /// <summary>
        /// 获取枚举所有值的KeyValuePair(int, string)列表 key是枚举值，value是中文描述
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="removeValue">如果有特殊的值需要移除掉，则赋值</param>
        /// <param name="addDefaultValue">为了dropdownlist显示，可以选择添加一个 "--全部--"</param>
        /// <returns></returns>
        public  List<KeyValuePair<int, string>> GetEnumChineseDepicts<T>(int? removeValue, bool addDefaultValue)
        {
            var enums = GetEnumChineseDepicts<T>();
            if (removeValue.HasValue)
            {
                if (enums.Exists(x => x.Key == removeValue.Value))
                {
                    enums.Remove(enums.FirstOrDefault(x => x.Key == removeValue.Value));
                }
            }
            if (addDefaultValue)
            {
                enums.Insert(0, new KeyValuePair<int, string>(-1, "--全部--"));
            }
            return enums;
        }

        private  List<EnumItem> GetEnumItems<T>()
        {
            var result = new List<EnumItem>();

            var enumType = typeof(T);
            if (!enumType.IsEnum)
                throw new Exception("本方法只适用于枚举类型！");

            foreach (var t in (T[])System.Enum.GetValues(enumType))
            {
                var value = (int)(object)t;
                var name = t.ToString();

                DescriptionAttribute attr = null;
                FieldInfo fieldInfo = enumType.GetField(name);
                if (fieldInfo != null)
                {
                    attr = Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute), false)
                        as DescriptionAttribute;
                }

                var depict = attr == null ? name : attr.Description;
                result.Add(new EnumItem(name, value, depict));
            }
            return result;
        }

        /// <summary>
        /// 内部类
        /// </summary>
        public class EnumItem
        {
            public string Name { get; set; }
            public int Value { get; set; }
            public string Depict { get; set; }

            public EnumItem(string name, int value, string depict)
            {
                Name = name;
                Value = value;
                Depict = depict;
            }
        }
    }
}
