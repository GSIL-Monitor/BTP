using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jinher.AMP.BTP.Deploy;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.UI.Models
{
    [Serializable()]
    [DataContract]
    public class StoreVM : StoreDTO
    {
        [DataMember]
        public string Setting { get; set; }
    }
    [Serializable()]
    [DataContract]
    public class StoreSettingVM
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string AppId { get; set; }
        [DataMember]
        public string StoreId { get; set; }
        [DataMember]
        public double DeliveryRange { get; set; }
        [DataMember]

        public decimal DeliveryAmount { get; set; }
        [DataMember]
        public decimal DeliveryFee { get; set; }
        [DataMember]
        public List<WorkTimeVM> WorkTimes { get; set; }

         [DataMember]
        public string DeliveryFeeStartT { get; set; }

         [DataMember]
         public string DeliveryFeeEndT { get; set; }

         [DataMember]
         public double DeliveryFeeDiscount { get; set; }

         [DataMember]
         public decimal FreeAmount { get; set; }

    }
    [Serializable()]
    [DataContract]
    public class WorkTimeVM
    {
        [DataMember]
        public TimeVM stime { get; set; }
        [DataMember]
        public TimeVM etime { get; set; }
    }

    [Serializable()]
    [DataContract]
    public class TimeVM
    {
        [DataMember]
        public string hour { get; set; }
        [DataMember]
        public string min { get; set; }
    }
}