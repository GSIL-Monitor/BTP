using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Jinher.JAP.MVC.Filter.FilterProvider;
using Jinher.JAP.BaseApp.Portal.Implement;

namespace Jinher.AMP.BTP.UI
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            GlobalFilterProvider provider = new GlobalFilterProvider();
            provider.RegisterFilters(filters);
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
            routes.IgnoreRoute("{*service}", new { service = @".*\.svc(/.*)?" });
            routes.IgnoreRoute("{*service}", new { service = @".*\.ashx(/.*)?" });
            //RouteManager.RouteRegistration(routes);

            routes.MapRoute(
               "YYPayFinish", // Route name
               "payfinish/{appId}/{orderId}/{shopId}", // URL with parameters
               new { controller = "Mobile", action = "PayFinish", appId = UrlParameter.Optional, orderId = UrlParameter.Optional, shopId = UrlParameter.Optional }
              );

            routes.MapRoute(
                 "DiyGroup", // Route name
                 "diygroupdetail/{appId}/{diyGroupId}/{*shareId}", // URL with parameters
                 new { controller = "Mobile", action = "DiyGroupDetail", diyGroupId = UrlParameter.Optional, shareId = UrlParameter.Optional, appId = UrlParameter.Optional }
                );
            //餐饮支付回调
            routes.MapRoute(
                 "CyOrderPayBack", // Route name
                 "cyorderpayback/{appId}/", // URL with parameters
                 new { controller = "Mobile", action = "CYOrderPayBack", orderId = UrlParameter.Optional }
                );
            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "SetMobile", action = "Index", id = UrlParameter.Optional } // Parameter defaults
                , new string[] { "Jinher.AMP.BTP.UI.Controllers" }
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(decimal?), new DecimalModelBinder());
#if DEBUG
            //RouteDebug.RouteDebugger.RewriteRoutesForTesting(RouteTable.Routes);
#endif
        }
    }

    public class DecimalModelBinder : DefaultModelBinder
    {
        #region Implementation of IModelBinder

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult.AttemptedValue.Equals("N.aN") ||
                valueProviderResult.AttemptedValue.Equals("NaN") ||
                valueProviderResult.AttemptedValue.Equals("Infini.ty") ||
                valueProviderResult.AttemptedValue.Equals("Infinity") ||
                string.IsNullOrEmpty(valueProviderResult.AttemptedValue))
                return 0m;

            return Convert.ToDecimal(valueProviderResult.AttemptedValue);
        }

        #endregion
    }

}