using System;
using System.Web.Mvc;
using Jinher.AMP.BTP.Deploy.CustomDTO.DaMiWang;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Common;
using System.Collections.Generic;

namespace Jinher.AMP.BTP.UI.Controllers
{
    /// <summary>
    /// 对外接口
    /// </summary>
    public class OpenApiController : Controller
    {
        /// <summary>
        /// 应用统计信息（目前用于大米网获取“寒地黑土”数据）
        /// </summary>
        /// <returns></returns>
        //public JsonResult Statistics()
        //{
        //    var msg_type = Request["msg_type"];
        //    var msg_code = Request["msg_code"];
        //    var msg_client_id = Request["msg_client_id"];
        //    var msg_time = Request["msg_time"];
        //    LogHelper.Info(string.Format("OpenApiController.Statistics大米网获取数据输入：msg_type={0}&msg_code={1}&msg_client_id={2}&msg_time={3}", msg_type, msg_code, msg_client_id, msg_time));
        //    var dmwIpList = Common.CustomConfig.DMWIPList;
        //    var btpIp = Common.CustomConfig.BTPIP;
        //    var result = new AppStatisticsDTO
        //    {
        //        msg_type = "2",
        //        msg_code = "02",
        //        msg_client_id = btpIp,
        //        msg_time = DateTime.Now.ToString("yyyyMMddHHmmss"),
        //        sign = btpIp,
        //        msg_salesvolume = "0",
        //        msg_browser = "0",
        //        msg_membership = "0",
        //        msg_membershipgrowth = "0",
        //        msg_ordernumber = "0",
        //        msg_productquantity = "0"
        //    };
        //    if (!Common.IPHelper.CheckCurrentIP(dmwIpList))
        //    {
        //        LogHelper.Error("OpenApiController.Statistics大米网获取数据失败，ip无效");
        //        return Json(result, JsonRequestBehavior.AllowGet);
        //    }
        //    Guid appId = Guid.Parse("3a77b453-f06b-43e0-9f92-e258cc326c44");
        //    var dto = new IBP.Facade.AppExtensionFacade().GetAppStatistics(appId);
        //    if (dto == null)
        //    {
        //        dto = new AppStatisticsDTO {msg_salesvolume = "0", msg_ordernumber = "0", msg_productquantity = "0"};
        //        LogHelper.Error("OpenApiController.Statistics大米网获取数据失败，未获取到btp统计信息");
        //    }
        //    var dssDto = TPS.DSSSV.Instance.GetAppUserPV(DateTime.Now.ToString("yyyyMMdd"), appId);
        //    if (dssDto == null)
        //    {
        //        dssDto = new DSS.Deploy.CustomDTO.AppUserPVInfo();
        //        LogHelper.Error("OpenApiController.Statistics大米网获取数据失败，未获取到dss统计信息");
        //    }
        //    result.msg_code = "01";
        //    result.msg_salesvolume = dto.msg_salesvolume;
        //    result.msg_ordernumber = dto.msg_ordernumber;
        //    result.msg_productquantity = dto.msg_productquantity;
        //    result.msg_browser = dssDto.pv.ToString();
        //    result.msg_membership = dssDto.userNum.ToString();
        //    result.msg_membershipgrowth = dssDto.newUserNum.ToString();
        //    LogHelper.Info("OpenApiController.Statistics大米网获取数据输出：" + JsonHelper.JsSerializer(result));
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}


        public JsonResult Statistics()
        {
            string Appids = CustomConfig.TJAppIds;
            List<Guid> Appidlist = new List<Guid>();
            if (!string.IsNullOrEmpty(Appids))
            {
                var arr = Appids.Split(new char[] { ',' });
                foreach (var item in arr)
                {
                    Appidlist.Add(Guid.Parse(item));
                }
            }
            var msg_type = Request["msg_type"];
            var msg_code = Request["msg_code"];
            var msg_client_id = Request["msg_client_id"];
            var msg_time = Request["msg_time"];
            LogHelper.Info(string.Format("OpenApiController.Statistics大米网获取数据输入：msg_type={0}&msg_code={1}&msg_client_id={2}&msg_time={3}", msg_type, msg_code, msg_client_id, msg_time));
            var dmwIpList = Common.CustomConfig.DMWIPList;
            var btpIp = Common.CustomConfig.BTPIP;
            var result = new AppStatisticsDTO
            {
                msg_type = "2",
                msg_code = "02",
                msg_client_id = btpIp,
                msg_time = DateTime.Now.ToString("yyyyMMddHHmmss"),
                sign = btpIp,
                msg_salesvolume = "0",
                msg_browser = "0",
                msg_membership = "0",
                msg_membershipgrowth = "0",
                msg_ordernumber = "0",
                msg_productquantity = "0"
            };
            if (!Common.IPHelper.CheckCurrentIP(dmwIpList))
            {
                LogHelper.Error("OpenApiController.Statistics大米网获取数据失败，ip无效");
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            double msg_salesvolume = 0;
            double msg_ordernumber = 0;
            double msg_productquantity = 0;
            int msg_browser = 0;
            int msg_membership = 0;
            int msg_membershipgrowth = 0;
            foreach (var item in Appidlist)
            {
                var dto = new IBP.Facade.AppExtensionFacade().GetAppStatistics(item);
                if (dto == null)
                {
                    dto = new AppStatisticsDTO { msg_salesvolume = "0", msg_ordernumber = "0", msg_productquantity = "0" };
                    LogHelper.Error("OpenApiController.Statistics大米网获取数据失败，未获取到btp统计信息");
                }
                var dssDto = TPS.DSSSV.Instance.GetAppUserPV(DateTime.Now.ToString("yyyyMMdd"), item);
                if (dssDto == null)
                {
                    dssDto = new DSS.Deploy.CustomDTO.AppUserPVInfo();
                    LogHelper.Error("OpenApiController.Statistics大米网获取数据失败，未获取到dss统计信息");
                }
                msg_salesvolume += Convert.ToDouble(dto.msg_salesvolume);
                msg_ordernumber += Convert.ToDouble(dto.msg_ordernumber);
                msg_productquantity += Convert.ToDouble(dto.msg_productquantity);
                msg_browser += dssDto.pv;
                msg_membership += dssDto.userNum;
                msg_membershipgrowth += dssDto.newUserNum;
            }
            result.msg_code = "01";
            result.msg_salesvolume = msg_salesvolume.ToString();
            result.msg_ordernumber = msg_ordernumber.ToString();
            result.msg_productquantity = msg_productquantity.ToString();
            result.msg_browser = msg_browser.ToString();
            result.msg_membership = msg_membership.ToString();
            result.msg_membershipgrowth = msg_membershipgrowth.ToString();
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        
    }
}