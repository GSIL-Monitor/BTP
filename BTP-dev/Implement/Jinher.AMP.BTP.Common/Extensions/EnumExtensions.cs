using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using System;

namespace Jinher.AMP.BTP.Common.Extensions
{
    /// <summary>
    /// 枚举帮助类
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// 从枚举中获取Description
        /// 说明：
        /// 单元测试-->通过
        /// </summary>
        /// <param name="enumName">需要获取枚举描述的枚举</param>
        /// <returns>描述内容</returns>
        public static string GetDescription(this Enum enumName)
        {
            string description = string.Empty;
            FieldInfo fieldInfo = enumName.GetType().GetField(enumName.ToString());
            DescriptionAttribute[] attributes = fieldInfo.GetDescriptAttr();
            if (attributes != null && attributes.Length > 0)
                description = attributes[0].Description;
            else
                description = enumName.ToString();
            return description;
        }
        /// <summary>
        /// 获取字段Description
        /// </summary>
        /// <param name="fieldInfo">FieldInfo</param>
        /// <returns>DescriptionAttribute[] </returns>
        public static DescriptionAttribute[] GetDescriptAttr(this FieldInfo fieldInfo)
        {
            if (fieldInfo != null)
            {
                return (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            }
            return null;
        }
        /// <summary>
        /// 根据Description获取枚举
        /// 说明：
        /// 单元测试-->通过
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="description">枚举描述</param>
        /// <returns>枚举</returns>
        public static T GetEnumName<T>(string description)
        {
            Type type = typeof(T);
            foreach (FieldInfo field in type.GetFields())
            {
                DescriptionAttribute[] curDesc = field.GetDescriptAttr();
                if (curDesc != null && curDesc.Length > 0)
                {
                    if (curDesc[0].Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }
            throw new ArgumentException(string.Format("{0} 未能找到对应的枚举.", description), "Description");
        }
        /// <summary>
        /// 将枚举转换为ArrayList
        /// 说明：
        /// 若不是枚举类型，则返回NULL
        /// 单元测试-->通过
        /// </summary>
        /// <param name="type">枚举类型</param>
        /// <returns>ArrayList</returns>
        public static ArrayList ToArrayList(this Type type)
        {
            if (type.IsEnum)
            {
                ArrayList array = new ArrayList();
                Array enumValues = Enum.GetValues(type);
                foreach (Enum value in enumValues)
                {
                    array.Add(new KeyValuePair<Enum, string>(value, GetDescription(value)));
                }
                return array;
            }
            return null;
        }

        /// <summary>
        /// 将枚举属性转换为list
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<EnumItem> ToItemList(this Type type)
        {
            List<EnumItem> items = new List<EnumItem>();
            if (!type.IsEnum)
            {
                return items;
            }
            Array enumValues = Enum.GetValues(type);
            foreach (Enum value in enumValues)
            {
                EnumItem item = new EnumItem();
                item.Name = value.ToString();
                item.Value = Convert.ToInt32(value);
                item.Description = GetDescription(value);
                items.Add(item);
            }
            return items;
        }
    }


    /// <summary>
    /// 每一个枚举属性的信息。
    /// </summary>
    public class EnumItem
    {
        public string Name { get; set; }
        public int Value { get; set; }
        public string Description { get; set; }
    }
}
