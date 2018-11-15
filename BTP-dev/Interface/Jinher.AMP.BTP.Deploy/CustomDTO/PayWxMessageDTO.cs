using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable]
    [DataContract]
    public class PayWxMessageDTO
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string touser { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string msgtype { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public WxNew news { get; set; }
        public PayWxMessageDTO()
        {
            news = new WxNew();
        }


    }
    [Serializable]
    [DataContract]
    public class WxNew
    {
        public WxNew()
        {
            articles= new List<WxArticles>();
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public List<WxArticles> articles { get; set; }
    }

    [Serializable]
    [DataContract]
    public class  WxArticles
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string url { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string picurl { get; set; }
    }

}
