using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Text;
using System.Net;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.BTP.UI.Models;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.UI.Util
{
    /// <summary>
    /// 手机端公共参数
    /// </summary>
    public class MobileLoginCookie
    {
        /// <summary>
        /// 公共参数
        /// </summary>
        private const string Key = "CookieContextDTO";


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static CookieContextDto GetCookie()
        {
            CookieContextDto result = null;
            HttpCookie cookie = HttpContext.Current.Request.Cookies[Key];
            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            {

                JavaScriptSerializer js = new JavaScriptSerializer();
                var cookieValue = HttpUtility.UrlDecode(cookie.Value);
                if (cookieValue == null)
                    return null;
                var obj = js.Deserialize<CookieContextDto>(cookieValue);
                return obj;
            }
            return null;
        }
        public static Guid GetLoginUserId()
        {
            var dto = GetCookie();
            if (dto != null && dto.IsLogin())
                return dto.userId ?? Guid.Empty;
            return Guid.Empty;

        }
        public static bool HasLoginCookie()
        {
            var dto = GetCookie();
            if (dto != null && dto.IsLogin())
                return true;
            return false;
        }

    }
    public class CookieContextDto
    {
        public Guid? userId { get; set; }
        public string sessionId { get; set; }
        public Guid? changeOrg { get; set; }
        public bool IsLogin()
        {
            if (Guid.Empty != userId && changeOrg.HasValue && changeOrg != Guid.Empty && !string.IsNullOrWhiteSpace(sessionId))
                return true;
            return false;
        }
    }
}
