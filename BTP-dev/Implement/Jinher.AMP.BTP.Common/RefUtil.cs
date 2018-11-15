using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.Common
{
    /// <summary>
    /// 反射帮助类。
    /// </summary>
    public static class RefUtil
    {

        /// <summary>
        /// 用源实体的中属性填充目标实体中同名属性。(Guid、Enum、可空类型都顺利赋值)
        /// </summary>
        /// <param name="target">目标实体</param>
        /// <param name="src">源实体</param>
        public static object FillWith(this object target, object src)
        {
            try
            {
                if (target == null || src == null)
                {
                    return target;
                }

                Type srcType = src.GetType();
                Type targetType = target.GetType();
                PropertyInfo[] targetPinfos = targetType.GetProperties();
                if (targetPinfos == null || targetPinfos.Length == 0)
                {
                    return target;
                }
                foreach (PropertyInfo tp in targetPinfos)
                {

                    //只读属性不能赋值。
                    if (!tp.CanWrite)
                    {
                        continue;
                    }
                    Type tppt = tp.PropertyType;
                    //可空类型，将值转换成空类型的基础类型。
                    tppt = tppt.IsNullable() && tppt != typeof(string) ? Nullable.GetUnderlyingType(tppt) : tp.PropertyType;
                    PropertyInfo srcPinfo = srcType.GetProperty(tp.Name);
                    if (srcPinfo == null)
                    {
                        continue;
                    }
                    try
                    {
                        object obj = srcPinfo.GetValue(src, null);
                        if (obj == null)
                        {
                            continue;
                        }
                        object targetVal = Convert.ChangeType(obj, tppt);
                        tp.SetValue(target, targetVal, null);
                    }
                    catch { }
                }

                return target;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("FillWith异常：{0}", ex));
            }
            return null;
        }


        private static object ChangeType(object value, Type type)
        {
            if (value == null && type.IsGenericType)
            {
                return Activator.CreateInstance(type);
            }
            if (value == null)
            {
                return null;
            }
            if (type == value.GetType())
            {
                return value;
            }
            if (type.IsEnum)
            {
                if (value is string)
                {
                    return Enum.Parse(type, value as string);
                }
                else
                {
                    return Enum.ToObject(type, value);
                }
            }
            if (!type.IsInterface && type.IsGenericType)
            {
                Type innerType = type.GetGenericArguments()[0];
                object innerValue = ChangeType(value, innerType);
                return Activator.CreateInstance(type, new object[] { innerValue });
            }
            if (value is string && type == typeof(Guid))
            {
                return new Guid(value as string);
            }
            if (value is string && type == typeof(Version))
            {
                return new Version(value as string);
            }
            if (!(value is IConvertible))
            {
                return value;
            }
            return Convert.ChangeType(value, type);
        }

    }
}
