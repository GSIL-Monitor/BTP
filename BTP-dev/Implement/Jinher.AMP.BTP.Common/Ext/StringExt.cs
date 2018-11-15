using System.Reflection;
using System.Text.RegularExpressions;

namespace System
{
    /// <summary>
    /// decimal类型扩展类
    /// </summary>
    public static class StringExt
    {
        /// <summary>
        /// 将金币转换成人民币（并保留两位小数）
        /// </summary>
        /// <param name="url"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string UrlAddParam(this string url, string key, string value)
        {
            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(key))
            {
                return url;
            }
            string newParam = string.Format("{0}={1}", key, value);
            Regex reg = new Regex(string.Format("(?<=\\?|\\&){0}=[^\\&]*", key), RegexOptions.IgnoreCase);
            if (reg.IsMatch(url))
            {
                url = reg.Replace(url, newParam);
            }
            else
            {
                if (url.Contains("?"))
                {
                    url += "&" + newParam;
                }
                else
                {
                    url += "?" + newParam;
                }
            }
            return url;
        }

        /// <summary>
        /// 转换成支持金币的金额（并保留三位小数）
        /// </summary>
        /// <param name="url"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string UrlDelParam(this string url, string key)
        {
            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(key))
            {
                return url;
            }
            Regex reg = new Regex(string.Format("((?<=\\?)|\\&){0}=[^\\&]*", key), RegexOptions.IgnoreCase);
            url = reg.Replace(url, "");
            url = url.TrimEnd('?');
            return url;
        }
        /// <summary>
        /// 转换成支持金币的金额（并保留三位小数）
        /// </summary>
        /// <returns></returns>
        public static bool IsNullVauleFromWeb(this string str)
        {
            if (str == null)
                return true;
            var trimStr = str.Trim().ToLower();
            if (str == string.Empty || str == "null" || str == "undefined" || str == "nan")
                return true;
            return false;
        }
        /// <summary>
        /// 转换成支持金币的金额（并保留三位小数）
        /// </summary>
        /// <returns></returns>
        public static string RemoveNullStr(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            return str.Replace("null", "").Replace("undefinded", "").Replace("(null)", "").Replace("nil", "");
        }
        /// <summary>
        /// 格式化字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Format(this string str, object obj)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            if (obj == null)
                return str;

            Regex regex = new Regex("{[^}]*}", RegexOptions.IgnoreCase);
            var matches = regex.Matches(str);
            if (matches.Count > 0)
            {
                Type type = obj.GetType();

                foreach (var item in matches)
                {
                    string matchStr = item.ToString();
                    var property = type.GetProperty(matchStr.Replace("{", "").Replace("}", ""));
                    if (property != null)
                    {
                        var value = property.GetValue(obj, null);
                        str = str.Replace(matchStr, value != null ? value.ToString() : string.Empty);
                    }
                }
            }
            return str;
        }
    }
}
