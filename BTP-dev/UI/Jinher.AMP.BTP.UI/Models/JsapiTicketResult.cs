using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jinher.AMP.BTP.UI.Models
{
    /// <summary>
    /// Jsapi_Ticket返回结果。
    /// </summary>
    public class JsapiTicketResult
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }
        public string ticket { get; set; }
        public string expires_in { get; set; }
    }
}