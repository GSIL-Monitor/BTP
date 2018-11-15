using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Jinher.AMP.BTP.Common
{
    public class ProvinceCityHelper
    {
        public const string CountryCode = "000000";
        private static readonly string FilePath = AppDomain.CurrentDomain.BaseDirectory + @"Configuration\CustomConfig\Jinher.AMP.BTP\ProvinceCity.xml";
        private static ProvinceCityCollection _provinceCity;

        static ProvinceCityHelper()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ProvinceCityCollection));
                FileStream stream = new FileStream(FilePath, FileMode.Open);
                _provinceCity = (ProvinceCityCollection)serializer.Deserialize(stream);
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
            if (_provinceCity == null || _provinceCity.Areas == null || !_provinceCity.Areas.Any())
            {
                return result;
            }
            var area = _provinceCity.Areas.FirstOrDefault(c => c.AreaCode == code);
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
            if (_provinceCity == null || _provinceCity.Areas == null || !_provinceCity.Areas.Any())
            {
                return result;
            }

            var areas = _provinceCity.Areas.Where(c => c.Name.Contains(name));
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
        public static string GetAreaNamesByCodeList(List<string> codeList, string separator)
        {
            string result = string.Empty;
            if (_provinceCity == null || _provinceCity.Areas == null || !_provinceCity.Areas.Any())
            {
                return result;
            }
            var areas = _provinceCity.Areas.Where(c => codeList.Contains(c.AreaCode)).ToList();
            if (areas.Any())
            {
                for (int i = 0; i < codeList.Count; i++)
                {
                    var area = areas.FirstOrDefault(c => c.AreaCode == codeList[i]);
                    if (area != null)
                    {
                        result = result + area.Name + separator;
                    }
                }
                if (!string.IsNullOrEmpty(result))
                {
                    return result.Substring(0, result.Length - separator.Length);
                }
            }
            return result;
        }

        public static string GetProvinceCodeByName(string name)
        {
            var result = string.Empty;
            if (_provinceCity == null || _provinceCity.Areas == null || !_provinceCity.Areas.Any() || string.IsNullOrWhiteSpace(name))
            {
                return result;
            }
            var area = _provinceCity.Areas.FirstOrDefault(c => c.Level == 1 && c.Name.StartsWith(name));
            if (area != null)
            {
                return area.AreaCode;
            }
            return result;
        }
        public static List<Area> GetAllProvince()
        {
            List<Area> result = new List<Area>();
            if (_provinceCity == null || _provinceCity.Areas == null || !_provinceCity.Areas.Any())
            {
                return result;
            }
            var area = _provinceCity.Areas.Where(c => c.Level == 1).ToList();
            return area;
        }
        public static List<Area> GetDirectlyCityList()
        {
            var directlyCityList = new List<string> { "110000", "120000", "310000", "500000" };
            return _provinceCity.Areas.Where(c => directlyCityList.Contains(c.AreaCode)).ToList();
        }

        public static Area GetProvinceByAreaCode(string code)
        {
            var result = string.Empty;
            if (_provinceCity == null || _provinceCity.Areas == null || !_provinceCity.Areas.Any() || string.IsNullOrWhiteSpace(code) || code.Length != 6)
            {
                return null;
            }
            var area = _provinceCity.Areas.FirstOrDefault(c => c.Level == 1 && c.AreaCode == code.Substring(0, 2) + "0000");
            if (area != null)
            {
                return area;
            }
            return null;
        }
        public static Area GetCityByAreaCode(string code)
        {
            var result = string.Empty;
            if (_provinceCity == null || _provinceCity.Areas == null || !_provinceCity.Areas.Any() || string.IsNullOrWhiteSpace(code) || code.Length != 6)
            {
                return null;
            }
            var area = _provinceCity.Areas.FirstOrDefault(c => c.AreaCode == code.Substring(0, 4) + "00");
            if (area != null)
            {
                return area;
            }
            return null;
        }
        public static Area GetAreaByName(string name)
        {
            if (_provinceCity == null || _provinceCity.Areas == null || !_provinceCity.Areas.Any() || string.IsNullOrWhiteSpace(name))
            {
                return null;
            }
            var area = _provinceCity.Areas.FirstOrDefault(c => c.Name.StartsWith(name));
            if (area != null)
            {
                return area;
            }
            return null;
        }

        /// <summary>
        /// 是否是全国
        /// </summary>
        /// <param name="code">区域编码</param>
        /// <returns></returns>
        public static bool IsTheWholeCountry(string code)
        {
            if (string.IsNullOrWhiteSpace(code) || code == CountryCode)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 获取省份下的城市
        /// </summary>
        /// <param name="provinceCode"></param>
        /// <returns></returns>
        public static List<Area> GetProvinceCities(string provinceCode)
        {
            if (_provinceCity == null || _provinceCity.Areas == null || !_provinceCity.Areas.Any() || string.IsNullOrWhiteSpace(provinceCode) || provinceCode.Length != 6)
            {
                return null;
            }
            var area = _provinceCity.Areas.Where(c => c.Level == 2 && c.AreaCode.StartsWith(provinceCode.Substring(0, 2))).ToList();
            return area;
        }
        /// <summary>
        /// 获取城市下所有区县
        /// </summary>
        /// <param name="cityCode"></param>
        /// <returns></returns>
        public static List<Area> GetCityDistricts(string cityCode)
        {
            if (_provinceCity == null || _provinceCity.Areas == null || !_provinceCity.Areas.Any() || string.IsNullOrWhiteSpace(cityCode) || cityCode.Length != 6)
            {
                return null;
            }
            var area = _provinceCity.Areas.Where(c => c.Level == 3 && c.AreaCode.StartsWith(cityCode.Substring(0, 4))).ToList();
            if (area.Count == 0)
            {
                area = _provinceCity.Areas.Where(c => c.Level == 3 && c.AreaCode.StartsWith(cityCode.Substring(0, 2))).ToList();
            }
            return area;
        }
    }
    [XmlRoot("ProvinceCities")]
    public class ProvinceCityCollection
    {
        [XmlArray("Areas"), XmlArrayItem("Area")]
        public Area[] Areas { get; set; }
    }
    [XmlRoot("ProvinceCities")]
    public class Area
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
