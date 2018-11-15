using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.JD
{
    public class JdMessageDto
    {
        [DataMember]
        public bool success { get; set; }

        [DataMember]
        public string resultMessage { get; set; }

        [DataMember]
        public string resultCode { get; set; }

        [DataMember]
        public List<JdMessagefirst> result { get; set; }

    }

    public class JdMessagefirst
    {

        [DataMember]
        public string id { get; set; }

        [DataMember]
        public JdMessageSecond result { get; set; }

        [DataMember]
        public DateTime time { get; set; }

        [DataMember]
        public int type { get; set; }

    }

    public class JdMessageSecond
    {

        [DataMember]
        public int state { get; set; }

        [DataMember]
        public string orderId { get; set; }

    }

    public class Suborders
    {
        [DataMember]
        public string pOrder { get; set; }

        [DataMember]
        public int orderState { get; set; }

        [DataMember]
        public string jdOrderId { get; set; }

        [DataMember]
        public decimal freight { get; set; }

        [DataMember]
        public int state { get; set; }

        [DataMember]
        public int submitState { get; set; }

        [DataMember]
        public decimal orderPrice { get; set; }

        [DataMember]
        public decimal orderNakedPrice { get; set; }

        [DataMember]
        public List<goodssku> sku { get; set; }

        [DataMember]
        public int type { get; set; }

        [DataMember]
        public decimal orderTaxPrice { get; set; }

    }

    public class goodssku
    {

        [DataMember]
        public string category { get; set; }

        [DataMember]
        public int num { get; set; }

        [DataMember]
        public decimal price { get; set; }

        [DataMember]
        public decimal tax { get; set; }

        [DataMember]
        public int oid { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public decimal taxPrice { get; set; }

        [DataMember]
        public string skuId { get; set; }

        [DataMember]
        public decimal nakedPrice { get; set; }

        [DataMember]
        public int type { get; set; }

    }
}
