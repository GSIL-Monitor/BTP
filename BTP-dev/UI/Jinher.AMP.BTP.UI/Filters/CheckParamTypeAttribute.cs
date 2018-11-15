using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Jinher.AMP.BTP.UI.Filters
{
    /// <summary>
    /// 检查Guid类型的参数不能为00000000-0000-0000-0000-000000000000的过滤器
    /// </summary>
    public class CheckParamTypeAttribute : ActionFilterAttribute
    {
        private string _errorMessage = "地址错误，请检查后重试~";

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }


        private bool _isCheckGuid = false;

        /// <summary>
        /// 是否检查guid类型的参数。
        /// </summary>
        public bool IsCheckGuid
        {
            get { return _isCheckGuid; }
            set { _isCheckGuid = value; }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        { 
            ParameterDescriptor[] actionParameters = filterContext.ActionDescriptor.GetParameters();
            foreach (var p in actionParameters)
            {
                if (p.ParameterType == typeof(Guid) && IsCheckGuid)
                {
                    object obj = filterContext.ActionParameters[p.ParameterName];
                    if(obj == null)
                    {
                        continue;
                    }
                    Guid guidValue = Guid.Empty;
                    Guid.TryParse(obj.ToString(), out guidValue);
                    if (guidValue == Guid.Empty)
                    {
                        ViewResult vr = new ViewResult();
                        vr.ViewName = "~/Views/Distribute/MobileError.cshtml";
                        vr.ViewData["Message"] = _errorMessage;
                        filterContext.Result = vr;
                    }
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}