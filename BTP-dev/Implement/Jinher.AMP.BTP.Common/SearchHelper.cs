using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using Jinher.AMP.BTP.Common.Search;

namespace Jinher.AMP.BTP.Common
{
    /// <summary>
    /// 应用搜索帮助类
    /// </summary>
    public static class SearchHelper
    {
        /// <summary>
        /// 应用分页搜索
        /// </summary>
        /// <param name="appName">应用名称搜索字符串</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns></returns>
        public static AppSearch GetTemplateSearchResult(string appName, int pageIndex, int pageSize)
        {
            var searchServiceUrl = CustomConfig.SearchServiceUrl;
            if (string.IsNullOrEmpty(searchServiceUrl))
            {
                searchServiceUrl = "http://183.56.132.225:10000/";
            }
            var url = "";
            if (!string.IsNullOrEmpty(appName))
            {
                url = searchServiceUrl + "?key=name,," + appName + "&filt_type=templateid,l4m4&max_doc_len=1024&pagesize=" + pageSize + "&page=" + pageIndex + "&sort_type=sort_subtime_desc";
            }
            else
            {
                url = searchServiceUrl + "?key=tidalias,,4&max_doc_len=1024&pagesize=" + pageSize + "&page=" + pageIndex + "&sort_type=sort_subtime_desc";
            }
            try
            {
                var myRequest = (HttpWebRequest)WebRequest.Create(url);
                myRequest.Method = "Get";
                myRequest.ContentType = "application/json";
                myRequest.MaximumAutomaticRedirections = 1;
                myRequest.AllowAutoRedirect = true;
                myRequest.Timeout = 1000 * 1800;
                var myResponse = (HttpWebResponse)myRequest.GetResponse();
                var responseStream = myResponse.GetResponseStream();
                if (responseStream != null)
                {
                    var reader = new StreamReader(responseStream, Encoding.GetEncoding("GBK"));
                    var retJson = reader.ReadToEnd();
                    reader.Close();
                    myResponse.Close();

                    var serializer = new DataContractJsonSerializer(typeof(AppSearch));
                    AppSearch appsearch;
                    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(retJson)))
                    {
                        appsearch = (AppSearch)serializer.ReadObject(ms);
                        ms.Close();
                    }
                    return appsearch;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
