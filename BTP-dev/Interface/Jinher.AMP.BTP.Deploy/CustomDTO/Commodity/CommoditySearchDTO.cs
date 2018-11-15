using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 商品检索类
    /// </summary>
    [Serializable]
    [DataContract]
    public class CommoditySearchDTO
    {
        [DataMember]
        public System.Guid appId { get; set; }

        [DataMember]
        public int pageSize { get; set; }

        [DataMember]
        public int pageIndex { get; set; }

        /// <summary>
        /// 数据总数
        /// </summary>
        [DataMember]
        public int rowCount { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string commodityName { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        [DataMember]
        public string commodityCode { get; set; }

        /// <summary>
        /// 类目名称
        /// </summary>
        [DataMember]
        public string commodityCategory { get; set; }

        [DataMember]
        public string sSalesvolume { get; set; }

        [DataMember]
        public string eSalesvolume { get; set; }

        [DataMember]
        public string sPrice { get; set; }

        [DataMember]
        public string ePrice { get; set; }

        /// <summary>
        /// 商品列表
        /// </summary>
        [DataMember]
        public List<Guid> commodityIdList { get; set; }
        /// <summary>
        /// 商品Id
        /// </summary>
        [DataMember]
        public Guid CommodityId { get; set; }

        public CommoditySearchDTO()
        {
            pageSize = 50;
            pageIndex = 1;
            appId = new Guid();
        }
        /// <summary>
        /// 是否按照传入顺序输出
        /// </summary>
        [DataMember]
        public bool IsDefaultOrder { get; set; }

        /// <summary>
        /// 地区编码
        /// </summary>
        [DataMember]
        public string AreaCode { get; set; }




        /// <summary>
        /// 商品所属类别
        /// </summary>
        [DataMember]
        public string CategorysIdList { get; set; }
        /// <summary>
        /// 商品最小毛利率
        /// </summary>
        [DataMember]
        public string MinInterestRate { get; set; }
        /// <summary>
        /// 商品最大毛利率
        /// </summary>
        [DataMember]
        public string MaxInterestRate { get; set; }
        /// <summary>
        /// 商品最小价格
        /// </summary>
        [DataMember]
        public string MinPrice { get; set; }
        /// <summary>
        /// 商品最大价格
        /// </summary>
        [DataMember]
        public string MaxPrice { get; set; }
    }
}
