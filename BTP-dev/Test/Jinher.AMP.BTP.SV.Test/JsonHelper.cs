﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;

/// <summary>
/// JSON序列化和反序列化辅助类
/// </summary>
public class JsonHelper
{
    /// <summary>
    /// JSON序列化
    /// </summary>
    public static string JsonSerializer<T>(T t)
    {
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
        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
        MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
        T obj = (T)ser.ReadObject(ms);
        return obj;
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
}