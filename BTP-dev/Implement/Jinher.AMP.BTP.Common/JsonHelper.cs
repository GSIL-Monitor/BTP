using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Web.Script.Serialization;

/// <summary>
/// JSON序列化和反序列化辅助类
/// </summary>
public class JsonHelper
{
    /// <summary>
    /// js序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    /// <returns></returns>
    public static string JsSerializer<T>(T t)
    {
        JavaScriptSerializer js = new JavaScriptSerializer();
        return js.Serialize(t);
    }

    /// <summary>
    /// JSON序列化
    /// </summary>
    public static string JsonSerializer<T>(T t)
    {
        GetPropertyInfoArray<T>(t);
        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
        MemoryStream ms = new MemoryStream();
        ser.WriteObject(ms, t);
        string jsonString = Encoding.UTF8.GetString(ms.ToArray());
        ms.Close();
        return jsonString;
    }

    /// <summary>
    /// JSON反序列化
    /// </summary>
    public static T JsonDeserialize<T>(string jsonString)
    {
        if (string.IsNullOrWhiteSpace(jsonString) || jsonString == "null")
        {
            return default(T);
        }
        try
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            T obj = (T)ser.ReadObject(ms);
            GetPropertyInfoArray2<T>(obj);
            return obj;
        }
        catch
        {
            return default(T);
        }
    }

    internal static readonly long DatetimeMinTimeTicks = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;
    public static string SerializeDateTime(DateTime datetime)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(@"""/Date(");
        sb.Append((datetime.Ticks - DatetimeMinTimeTicks) / 10000);
        sb.Append(@")/""");
        return sb.ToString();
    }

    #region reflect

    private static DateTime dt = DateTime.MinValue.AddDays(1);
    private static DateTime dtNull;
    private static string KeyName = "Key";
    private static string ValueName = "Value";
    private static Type DictionaryKeyType;
    private static Type DictionaryValueType;

    private static PropertyInfo[] GetPropertyInfoArray<T>(T t)
    {
        if (t == null)
            return null;
        PropertyInfo[] props = null;

        Type type = t.GetType(); //获取类型

        //泛型支持
        if (type.IsGenericType)
        {
            if (t.GetType().Name == typeof(Dictionary<,>).Name)            //Dictionary
            {
                foreach (object o in (t as IEnumerable))
                {
                    Type typeKeyValue = o.GetType();
                    IList<Type> genericArguments = typeKeyValue.GetGenericArguments();
                    Type keyType = genericArguments[0];
                    Type valueType = genericArguments[1];
                    MemberInfo[] members = typeKeyValue.GetMember(KeyName, BindingFlags.Instance | BindingFlags.Public);
                    MemberInfo member = members.Single();

                    MemberInfo[] members2 = typeKeyValue.GetMember(ValueName, BindingFlags.Instance | BindingFlags.Public);
                    MemberInfo member2 = members2.Single();

                    GetPropertyInfoArray(member2.ReflectedType.GetProperty("Value").GetValue(o, null));
                }
            }
            else if (t.GetType().Name == typeof(KeyValuePair<,>).Name)    //KeyValuePair
            {
                Type typeKeyValue = t.GetType();
                IList<Type> genericArguments = typeKeyValue.GetGenericArguments();
                Type keyType = genericArguments[0];
                Type valueType = genericArguments[1];
                MemberInfo[] members = typeKeyValue.GetMember(KeyName, BindingFlags.Instance | BindingFlags.Public);
                MemberInfo member = members.Single();

                MemberInfo[] members2 = typeKeyValue.GetMember(ValueName, BindingFlags.Instance | BindingFlags.Public);
                MemberInfo member2 = members2.Single();

                GetPropertyInfoArray(member2.ReflectedType.GetProperty("Value").GetValue(t, null));
            }
            else if (t.GetType().Name == typeof(List<>).Name)           //List
            {
                foreach (object o in (t as IEnumerable))
                {
                    if (o != null)
                    {
                        GetPropertyInfoArray(o);
                    }
                }
            }
        }
        else
        {
            System.Reflection.PropertyInfo[] propertyInfoList = type.GetProperties();
            if (propertyInfoList.Length >= 1)
            {
                foreach (System.Reflection.PropertyInfo propertyInfo in propertyInfoList)
                {
                    if (propertyInfo.PropertyType.FullName == "System.String")
                    {
                        continue;
                    }
                    else if (propertyInfo.PropertyType.BaseType.Name == "Object")
                    {
                        object tmp = propertyInfo.GetValue(t, null);
                        if (tmp != null)
                        {
                            GetPropertyInfoArray(tmp);
                        }
                    }
                    else if (propertyInfo.PropertyType.FullName == "System.DateTime")
                    {
                        DateTime value_Old = (DateTime)propertyInfo.GetValue(t, null); //获取属性值
                        //为空时附值
                        if (value_Old == dtNull)
                        {
                            propertyInfo.SetValue(t, dt, null); //给对应属性赋值
                        }
                    }
                }
            }
        }

        return props;
    }

    private static PropertyInfo[] GetPropertyInfoArray2<T>(T t)
    {
        if (t == null)
            return null;
        PropertyInfo[] props = null;
        Type type = t.GetType(); //获取类型

        //泛型支持
        if (type.IsGenericType)
        {
            if (t.GetType().Name == typeof(Dictionary<,>).Name)            //Dictionary
            {
                foreach (object o in (t as IEnumerable))
                {
                    Type typeKeyValue = o.GetType();
                    IList<Type> genericArguments = typeKeyValue.GetGenericArguments();
                    Type keyType = genericArguments[0];
                    Type valueType = genericArguments[1];
                    MemberInfo[] members = typeKeyValue.GetMember(KeyName, BindingFlags.Instance | BindingFlags.Public);
                    MemberInfo member = members.Single();

                    MemberInfo[] members2 = typeKeyValue.GetMember(ValueName, BindingFlags.Instance | BindingFlags.Public);
                    MemberInfo member2 = members2.Single();

                    GetPropertyInfoArray2(member2.ReflectedType.GetProperty("Value").GetValue(o, null));
                }
            }
            else if (t.GetType().Name == typeof(KeyValuePair<,>).Name)    //KeyValuePair
            {
                Type typeKeyValue = t.GetType();
                IList<Type> genericArguments = typeKeyValue.GetGenericArguments();
                Type keyType = genericArguments[0];
                Type valueType = genericArguments[1];
                MemberInfo[] members = typeKeyValue.GetMember(KeyName, BindingFlags.Instance | BindingFlags.Public);
                MemberInfo member = members.Single();

                MemberInfo[] members2 = typeKeyValue.GetMember(ValueName, BindingFlags.Instance | BindingFlags.Public);
                MemberInfo member2 = members2.Single();

                GetPropertyInfoArray2(member2.ReflectedType.GetProperty("Value").GetValue(t, null));
            }
            else if (t.GetType().Name == typeof(List<>).Name)           //List
            {
                foreach (object o in (t as IEnumerable))
                {
                    if (o != null)
                    {
                        GetPropertyInfoArray2(o);
                    }
                }
            }
        }
        else
        {
            System.Reflection.PropertyInfo[] propertyInfoList = type.GetProperties();
            if (propertyInfoList.Length >= 1)
            {
                foreach (System.Reflection.PropertyInfo propertyInfo in propertyInfoList)
                {
                    if (propertyInfo.PropertyType.FullName == "System.String")
                    {
                        continue;
                    }
                    else if (propertyInfo.PropertyType.BaseType.Name == "Object")
                    {
                        object tmp = propertyInfo.GetValue(t, null);
                        if (tmp != null)
                        {
                            GetPropertyInfoArray2(tmp);
                        }
                    }
                    else if (propertyInfo.PropertyType.FullName == "System.DateTime")
                    {
                        DateTime value_Old = (DateTime)propertyInfo.GetValue(t, null); //获取属性值
                        //为空时附值
                        if (value_Old == dt)
                        {
                            propertyInfo.SetValue(t, dtNull, null); //给对应属性赋值
                        }
                    }
                }
            }
        }
        return props;
    }

    #endregion
}
