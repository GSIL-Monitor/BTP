using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Jinher.AMP.BTP.Common
{
    /// <summary>
    /// Xml序列化与反序列化
    /// </summary>
    public class XmlUtil
    {
        #region 反序列化

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="xml">XML字符串</param>
        /// <returns></returns>
        public static T Deserialize<T>(string xml) where T : class
        {
            try
            {
                using (StringReader sr = new StringReader(xml))
                {
                    XmlSerializer xmldes = new XmlSerializer(typeof(T));
                    return xmldes.Deserialize(sr) as T;
                }
            }
            catch (Exception e)
            {

                return default(T);
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="stream"></param>
        /// 
        /// 
        /// <returns></returns>
        public static T Deserialize<T>(Stream stream) where T : class
        {
            XmlSerializer xmldes = new XmlSerializer(typeof(T));
            return xmldes.Deserialize(stream) as T;
        }
        #endregion

        #region 序列化

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static string Serializer<T>(T obj)
        {
            MemoryStream Stream = new MemoryStream();
            var type = typeof(T);
            XmlSerializer xml = new XmlSerializer(type);
            try
            {
                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                //序列化对象
                xml.Serialize(Stream, obj, ns);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            Stream.Position = 0;
            StreamReader sr = new StreamReader(Stream);
            string str = sr.ReadToEnd();

            str = str.Replace("<" + type.Name + ">", "<xml>").Replace("</" + type.Name + ">", "</xml>");
            sr.Dispose();
            Stream.Dispose();

            return str;
        }

        #endregion
    }
}
