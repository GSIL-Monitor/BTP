using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.JAP.MVC.Controller;
using Jinher.AMP.Portal.Common;
using Jinher.JAP.MVC.UIJquery.DataGrid;
using Jinher.AMP.BTP.Deploy;

namespace Jinher.AMP.BTP.UI.Controllers
{
    /// <summary>
    /// 单点登录
    /// </summary>
    public class LoginController : BaseController
    {
        /// <summary>
        /// 单点登录方法
        /// </summary>
        /// <returns></returns>
        public ActionResult SSOIndex()
        {
            //单点登录验证
            SingleSignOn singleSignOn = new SingleSignOn();
            SSOResult res = singleSignOn.Do(Request);
            return res.ActionResult;
        }

        public ActionResult Test()
        {
            return View();
        }

        /// <summary>
        /// 收集错误信息
        /// </summary>
        public void GatherErrorInfo()
        {
            int count = 0;
            string countStr = Request["c"];
            if (countStr != null) int.TryParse(countStr, out count);
            if (count <= 0) return;
            string mess = string.Empty;
            for (int i = 0; i < count; i++)
            {
                mess += "|" + Request["m" + i];
            }
            JAP.Common.Loging.LogHelper.Error(mess);
        }
    }
}
