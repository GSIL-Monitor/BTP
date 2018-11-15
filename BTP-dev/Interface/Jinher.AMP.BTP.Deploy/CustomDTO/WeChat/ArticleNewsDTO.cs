using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 图文消息
    /// </summary>
    [Serializable()]
    [DataContract]
    public class ArticleNewsDTO : NewsSendBaseDTO
    {
        public ArticleNewsDTO()
        {
            news = new ArticleNewsBodyDTO();
        }
        /// <summary>
        /// 消息体
        /// </summary>
        [DataMember]
        public ArticleNewsBodyDTO news { get; set; }
    }
    /// <summary>
    /// 消息体
    /// </summary>
    [Serializable()]
    [DataContract]
    public class ArticleNewsBodyDTO
    {
        public ArticleNewsBodyDTO()
        {
            articles = new List<ArticleDTO>();
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public List<ArticleDTO> articles { get; set; }
    }

    /// <summary>
    /// 图文消息体
    /// </summary>
    [Serializable()]
    [DataContract]
    public class ArticleDTO
    {
        /// <summary>
        /// 图文消息/视频消息/音乐消息的标题
        /// </summary>
        [DataMember]
        public string title { get; set; }
        /// <summary>
        /// 图文消息/视频消息/音乐消息的描述
        /// </summary>
        [DataMember]
        public string description { get; set; }
        /// <summary>
        /// 图文消息被点击后跳转的链接
        /// </summary>
        [DataMember]
        public string url { get; set; }
        /// <summary>
        /// 图文消息的图片链接，支持JPG、PNG格式，较好的效果为大图640*320，小图80*80
        /// </summary>
        [DataMember]
        public string picurl { get; set; }
    }
}
