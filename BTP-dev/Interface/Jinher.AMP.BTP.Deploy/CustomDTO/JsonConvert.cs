using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.BE.Deploy.Base;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy
{
    [Serializable()]
    [DataContract]
    public class JsonResult<T>
    {
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public bool isSuccess { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public List<T> DATA { get; set; }

    }
    [Serializable()]
    [DataContract]
    public class ReturnJsonResult
    {
        public static JsonResult<T> GetJsonResult<T>(string Code, string Message, bool isSuccess,List<T> DATA)
        {
            JsonResult<T> jsonResult = new JsonResult<T>();
            jsonResult.Code = Code;
            jsonResult.Message = Message;
            jsonResult.isSuccess = isSuccess;
            jsonResult.DATA = DATA;
            return jsonResult;
        }
    }

    public class JCCustomer
    { 
      public Guid Id{get;set;}
      public Guid SubId{get;set;}
      public DateTime SubTime{get;set;}
      public DateTime ModifiedOn{get;set;}
      public string Code{get;set;}
      public string Name{get;set;}
      public string LoginName{get;set;}
      public int Property{get;set;}
      public decimal CreditLink{get;set;}
      public bool IsDel{get;set;}
      public Guid AppId{get;set;}
      public int AccountPeriod{get;set;}
      public string LinkMan{get;set;}
      public string Linkway{get;set;}
      public bool IsFreeze{get;set;}
      public Guid JCInvoiceId{get;set;}
      public Guid JcActivityId { get; set; }
    }
}
