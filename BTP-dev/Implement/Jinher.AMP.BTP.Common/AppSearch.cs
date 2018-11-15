using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Jinher.AMP.BTP.Common.Search
{
    /// <summary>
    /// 查询结果
    /// </summary>
    public class AppSearch
    {
        public HeadDetail Head { get; set; }
        public List<ParagraphDetail> Paragraph { get; set; }
    }

    public class ParagraphDetail
    {
        public ContentDetail Content { get; set; }

        public List<PackageDetail> JoinContent_0 { get; set; }

        private string _hitcount;
        public string hitcount
        {
            get
            {
                return _hitcount;
            }
            set
            {
                _hitcount = HttpUtility.UrlDecode(value, Encoding.GetEncoding("gb2312"));
            }
        }

        private string _parsecount;
        public string parsecount
        {
            get
            {
                return _parsecount;
            }
            set
            {
                _parsecount = HttpUtility.UrlDecode(value, Encoding.GetEncoding("gb2312"));
            }
        }

        private string _subtime;
        public string subtime
        {
            get
            {
                return _subtime;
            }
            set
            {
                _subtime = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Convert.ToDouble(value)).ToString("yyyy-MM-dd");
            }
        }

        private string _templateid;
        public string templateid
        {
            get
            {
                return _templateid;
            }
            set
            {
                _templateid = HttpUtility.UrlDecode(value, Encoding.GetEncoding("gb2312"));
            }
        }

        private string _recommandrank;
        public string recommandrank
        {
            get
            {
                return _recommandrank;
            }
            set
            {
                _recommandrank = HttpUtility.UrlDecode(value, Encoding.GetEncoding("gb2312"));
            }
        }

        private string _downloadcount;
        public string downloadcount
        {
            get
            {
                return _downloadcount;
            }
            set
            {
                _downloadcount = HttpUtility.UrlDecode(value, Encoding.GetEncoding("gb2312"));
            }
        }
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
    }

    public class HeadDetail
    {
        public SummaryDetail Summary { get; set; }
    }

    public class SummaryDetail
    {
        private string _resultCount;
        public string ResultCount
        {
            get
            {
                return _resultCount;
            }
            set
            {
                _resultCount = HttpUtility.UrlDecode(value, Encoding.GetEncoding("gb2312"));
            }
        }
        public PageDetail Page { get; set; }
    }

    public class PageDetail
    {
        private string _pageCount;
        public string PageCount
        {
            get
            {
                return _pageCount;
            }
            set
            {
                _pageCount = HttpUtility.UrlDecode(value, Encoding.GetEncoding("gb2312"));
            }
        }

        private string _pageIndex;
        public string PageIndex
        {
            get
            {
                return _pageIndex;
            }
            set
            {
                _pageIndex = HttpUtility.UrlDecode(value, Encoding.GetEncoding("gb2312"));
            }
        }

        private string _pageSize;
        public string PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = HttpUtility.UrlDecode(value, Encoding.GetEncoding("gb2312"));
            }
        }
    }

    public class PackageDetail
    {
        private string _code;
        public string code
        {
            get
            {
                return _code;
            }
            set
            {
                _code = HttpUtility.UrlDecode(value, Encoding.GetEncoding("gb2312")).Trim();
            }
        }

        private string _hosttype;
        public string hosttype
        {
            get
            {
                return _hosttype;
            }
            set
            {
                _hosttype = HttpUtility.UrlDecode(value, Encoding.GetEncoding("gb2312"));
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
                _id = HttpUtility.UrlDecode(value, Encoding.GetEncoding("gb2312")).Trim();
            }
        }

        private string _packageid;
        public string packageid
        {
            get
            {
                return _packageid;
            }
            set
            {
                _packageid = HttpUtility.UrlDecode(value, Encoding.GetEncoding("gb2312")).Trim();
            }
        }

        private string _publishurl;
        public string publishurl
        {
            get
            {
                return _publishurl;
            }
            set
            {
                _publishurl = HttpUtility.UrlDecode(value, Encoding.GetEncoding("gb2312")).Trim();
            }
        }

        private string _totalbytes;
        public string totalbytes
        {
            get
            {
                return _totalbytes;
            }
            set
            {
                _totalbytes = HttpUtility.UrlDecode(value, Encoding.GetEncoding("gb2312")).Trim();
            }
        }

        private string _versioncode;
        public string versioncode
        {
            get
            {
                return _versioncode;
            }
            set
            {
                _versioncode = HttpUtility.UrlDecode(value, Encoding.GetEncoding("gb2312"));
            }
        }

        private string _versionname;
        public string versionname
        {
            get
            {
                return _versionname;
            }
            set
            {
                _versionname = HttpUtility.UrlDecode(value, Encoding.GetEncoding("gb2312")).Trim();
            }
        }
    }

    /// <summary>
    /// 搜索热词返回结果
    /// </summary>
    public class HotWord
    {
        public HotWordHead Header { get; set; }
        public List<HotWordDeatal> JContent { get; set; }
        public List<string> HotWords
        {
            get
            {
                if (JContent != null && JContent.Count > 0)
                {
                    return JContent.Select(d => d.key).ToList();
                }
                else
                {
                    return new List<string>();
                }
            }
        }
    }

    public class HotWordHead
    {
        public string Cost { get; set; }
        public string project { get; set; }
    }
    public class HotWordDeatal
    {
        public string id { get; set; }
        private string _key;
        public string key { get { return _key; } set { _key = HttpUtility.UrlDecode(value, Encoding.GetEncoding("gb2312")); } }
        public string resultSize { get; set; }
    }
}
