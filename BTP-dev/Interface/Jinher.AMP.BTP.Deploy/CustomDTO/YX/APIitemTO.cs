using System.Collections.Generic;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.YX
{
    public class APIitemTO
    {
        /// <summary>
        /// 商品（SPU）的标识
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 商品下的sku列表
        /// </summary>
        public List<APIskuTO> skuList { get; set; }
    }

    public class APIskuTO
    {
        /// <summary>
        /// Sku的标识（skuId）
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 渠道售价
        /// </summary>
        public decimal channelPrice { get; set; }
    }
}
