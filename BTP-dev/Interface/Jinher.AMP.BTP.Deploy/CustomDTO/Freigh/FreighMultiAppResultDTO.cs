using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 运费计算
    /// </summary>
    [Serializable()]
    [DataContract]
    public class FreighMultiAppResultDTO : ResultDTO
    {
        /// <summary>
        /// 返回总
        /// </summary>
        [DataMemberAttribute()]
        public List<AppFreight> AppFreight { get; set; }

        /// <summary>
        /// 返回总运费
        /// </summary>
        [DataMemberAttribute()]
        public decimal Freight { get; set; }
    }

    /// <summary>
    /// 商品App运费类
    /// </summary>
    [Serializable()]
    [DataContract]
    public class AppFreight
    {

        private Guid appId;
        /// <summary>
        /// AppId
        /// </summary>
        [DataMemberAttribute()]
        public Guid AppId
        {
            get { return appId; }
            set { appId = value; }
        }

        private decimal freight;
        /// <summary>
        /// 运费
        /// </summary>
        [DataMemberAttribute()]
        public decimal Freight
        {
            get { return freight; }
            set { freight = value; }
        }

        /// <summary>
        /// 运费描述
        /// </summary>
        [DataMember]
        public string FreeFreightStandard { get; set; }
    };
}
