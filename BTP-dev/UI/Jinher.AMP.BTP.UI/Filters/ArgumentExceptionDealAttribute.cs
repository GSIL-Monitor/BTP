using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Jinher.AMP.BTP.UI.Filters
{
    /// <summary>
    /// 处理没有必须参数的错误url.
    /// </summary>
    public class ArgumentExceptionDealAttribute : HandleErrorAttribute
    {
        private string _errorMessage = "地址错误，请检查后重试~";
        private string _title = "错误页";

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception.GetType() == typeof(System.ArgumentException))
            {
                ViewResult vr = new ViewResult();
                vr.ViewName = "~/Views/Mobile/MobileError.cshtml";
                vr.ViewBag.Message = _errorMessage;
                vr.ViewBag.Tilte = _title;
                filterContext.Result = vr;
                filterContext.ExceptionHandled = true;
            }
            //base.OnException(filterContext);
        }
    }
}