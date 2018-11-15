using System.Web.Mvc;

namespace Jinher.AMP.BTP.UI.Areas.WeChatQRCode
{
    public class WeChatQRCodeAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "WeChatQRCode";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "WeChatQRCode_default",
                "WeChatQRCode/{controller}/{action}/{id}",
                new { AreaName = "WeChatQRCode", action = "Index", id = UrlParameter.Optional },
                new string[] { "Jinher.AMP.BTP.UI.Areas.WeChatQRCode.Controllers" }
            );
        }
    }
}
