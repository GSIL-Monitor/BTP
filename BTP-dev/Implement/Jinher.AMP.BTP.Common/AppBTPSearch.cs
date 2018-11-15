using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Json;

namespace Jinher.AMP.BTP.Common
{
    public class AppBTPSearch
    {
        public static List<AppSearchResultDTO> GetSearchResult(string key)
        {

            if (string.IsNullOrEmpty(key))
            {
                return null;
            }
            List<AppSearchResultDTO> searchResults = new List<AppSearchResultDTO>();

            var searchServiceUrl = CustomConfig.SearchServiceUrl;
            if (string.IsNullOrEmpty(searchServiceUrl))
            {
                searchServiceUrl = "http://183.56.132.225:10000/";
            }
            var urlFormate = searchServiceUrl +
                //格式：{0}:%E9%87%91%E5%92%8C  {1}:&pagesize=10  {2}:&page=1 {3}:&sort_by_subtime_asc {4}&debug=yes{5}&filt_type=illegal,l0m0;offshelves,l0m0 
                         "?key=name,, {0}&filt_type=templateid,l4m4&max_doc_len=1024";
            key = HttpUtility.UrlEncode(key, Encoding.GetEncoding("gb2312"));


            var myRequest =
                (HttpWebRequest)
                    WebRequest.Create(string.Format(urlFormate, key));
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

                var serializer = new DataContractJsonSerializer(typeof(AppSearchResult));
                AppSearchResult appsearch;
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(retJson)))
                {
                    appsearch = (AppSearchResult)serializer.ReadObject(ms);
                    ms.Close();
                }
                if (appsearch != null && appsearch.Paragraph != null)
                {
                    foreach (var x in appsearch.Paragraph)
                    {
                        AppSearchResultDTO result = new AppSearchResultDTO();
                        Guid appId = Guid.Empty;
                        Guid.TryParse(x.Content.id, out appId);
                        result.AppId = appId;
                        result.AppName = x.Content.name;
                        Guid ownerId = Guid.Empty;
                        Guid.TryParse(x.Content.ownerid, out ownerId);
                        result.OwnerId = ownerId;
                        result.OwnerType = x.ownertypeid;

                        searchResults.Add(result);
                    }
                }
            }

            return searchResults;
        }
    }

    /// <summary>
    /// 查询结果
    /// </summary>
    public class AppSearchResult
    {

        public List<ParagraphDetail> Paragraph { get; set; }
    }

    public class ParagraphDetail
    {
        public ContentDetail Content { get; set; }

        public string ownertypeid { get; set; }
    }
    public class ContentDetail
    {
        private string _description;
        public string description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = HttpUtility.UrlDecode(value, Encoding.GetEncoding("gb2312"));
            }
        }

        private string _icon;
        public string icon
        {
            get
            {
                return _icon;
            }
            set
            {
                _icon = HttpUtility.UrlDecode(value, Encoding.GetEncoding("gb2312"));
            }
        }

        private string _id;
        public string id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = HttpUtility.UrlDecode(value, Encoding.GetEncoding("gb2312"));
            }
        }

        private string _name;
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = HttpUtility.UrlDecode(value, Encoding.GetEncoding("gb2312"));
            }
        }

        private string _ownerid;

        public string ownerid
        {
            get
            {
                return _ownerid;
            }
            set
            {
                _ownerid = HttpUtility.UrlDecode(value, Encoding.GetEncoding("gb2312"));
            }
        }

        private string _owneridtype;

        public string owneridtype
        {
            get
            {
                return _owneridtype;
            }
            set
            {
                _owneridtype = HttpUtility.UrlDecode(value, Encoding.GetEncoding("gb2312"));
            }
        }

    }
}
