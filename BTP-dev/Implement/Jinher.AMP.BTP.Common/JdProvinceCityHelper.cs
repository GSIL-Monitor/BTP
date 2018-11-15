using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Jinher.AMP.BTP.Common
{
    public class JdProvinceCityHelper
    {
        public const string CountryCode = "000000";
        private static readonly string FilePath = AppDomain.CurrentDomain.BaseDirectory + @"Configuration\CustomConfig\Jinher.AMP.BTP\JdProvinceCity.xml";
        private static _ProvinceCityCollection _provinceCity;

        static JdProvinceCityHelper()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(_ProvinceCityCollection));
                FileStream stream = new FileStream(FilePath, FileMode.Open);
                _provinceCity = (_ProvinceCityCollection)serializer.Deserialize(stream);
                stream.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 按区域编码获取区域名称.
        /// </summary>
        /// <param name="code">区域编码</param>
        /// <returns>区域名称</returns>
        public static string GetAreaNameByCode(string code)
        {
            var result = string.Empty;
            if (_provinceCity == null || _provinceCity._Areas == null || !_provinceCity._Areas.Any())
            {
                return result;
            }
            var area = _provinceCity._Areas.FirstOrDefault(c => c.AreaCode == code);
            if (area != null)
            {
                return area.Name;
            }
            return result;
        }

        /// <summary>
        /// 按区域名称获取区域编码.
        /// </summary>
        /// <param name="name">区域名称</param>
        /// <param name="level">级别：-1 不用level过滤 1省 2市 3 区县</param>
        /// <returns>区域编码</returns>
        public static string GetAreaCodeByName(string name, int level)
        {
            var result = string.Empty;
            if (_provinceCity == null || _provinceCity._Areas == null || !_provinceCity._Areas.Any())
            {
                return result;
            }

            var areas = _provinceCity._Areas.Where(c => c.Name.Contains(name));
            if (areas == null || (!areas.Any()))
            {
                return result;
            }
            if (level != -1)
            {
                areas = areas.Where(c => c.Level == level);
            }
            if (areas == null || (!areas.Any()))
            {
                return result;
            }
            Area areaF = areas.FirstOrDefault();
            return areaF.AreaCode;
        }

        /// <summary>
        /// 根据城市名称获取城市编码
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetCityCodeByCityName(string name)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;
            if (new List<string> { "北京市", "上海市", "天津市", "重庆市" }.Contains(name))
            {
                return GetAreaCodeByName(name, 1);
            }
            return GetAreaCodeByName(name, 2);
        }
    }

    [XmlRoot("ProvinceCities")]
    public class _ProvinceCityCollection
    {
        [XmlArray("Areas"), XmlArrayItem("Area")]
        public Area[] _Areas { get; set; }
    }
    [XmlRoot("ProvinceCities")]
    public class _Area
    {
        [XmlAttribute("code")]
        public string AreaCode { get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("spellCode")]
        public string SpellCode { get; set; }
        [XmlAttribute("level")]
        public int Level { get; set; }

        public string Code
        {
            get { return AreaCode; }
            set { AreaCode = value; }
        }
    }
}
