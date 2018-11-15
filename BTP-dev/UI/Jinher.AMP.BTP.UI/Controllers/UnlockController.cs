using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Mvc;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public class UnlockController : Controller
    {
        //
        // GET: /Unlock/

        public ActionResult Index()
        {
            string retStr = "{\"status\":1,\"data\":\"wxg9Udq6BTb91ONLGa2vGa1v2Oor2FRX50gihdcrmTX6UdAWmYlVtV5zRgjs97O9OUhV50gihsG9\nUsqtJT6VtOen50gihdcfmyqZBTLiEyqN101M1dZ6JKlM5xHOhd6QmTmbmTLkBTiQEDnkhvJ65v9n\n5rRDt3EDGFf65TP6EOPWGF2nG0bVmdED5aRx2FfnGF1nGa9ftYXNJ3b41ViVhdcOB3AxmDciJDPV\ntVgoYKf6asoXhOAxc9q0GxgH\n\"}";
            var result = JsonHelper.JsonDeserialize<YingKeResultDTO>(retStr);
            return View();
        }

        [HttpPost]
        public ActionResult DoUnlock(string unlockinput) 
        {
            var input = Guid.Parse(unlockinput);
            var isresult = TPS.OrderSV.UnLockOrder(input);
            return Content("解锁成功！" + isresult);
        }

    }

    [Serializable]
    [DataContract]
    public class YingKeResultDTO
    {
        /// <summary>
        /// 盈科接口返回原始字符串
        /// </summary>
        [DataMember]
        public string msg { get; set; }
    }
}
