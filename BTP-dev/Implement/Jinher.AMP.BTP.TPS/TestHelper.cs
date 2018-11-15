using System;
using System.Data;
using System.Linq;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.ZPH.ISV.Facade;
using Jinher.JAP.PL;

namespace Jinher.AMP.BTP.TPS
{
    public class TestHelper
    {
        public static DataTable ImportOrder()
        {
            var startDate = new DateTime(2018, 4, 10);
            var endDate = new DateTime(2018, 5, 10);
            var orderItems = OrderItem.ObjectSet().Where(_ => _.SubTime > startDate && _.SubTime < endDate && _.YJCouponPrice > 0).ToList();
            var orderIds = orderItems.Select(_ => _.CommodityOrderId).ToList();

            var orders = CommodityOrder.ObjectSet().Where(_ => orderIds.Contains(_.Id) && _.State != 5).OrderBy(_ => _.SubTime).ToList();
            DataTable dt = new DataTable();
            dt.Columns.Add("APP名称", typeof(string));
            dt.Columns.Add("订单编号", typeof(string));
            dt.Columns.Add("订单金额", typeof(decimal));
            dt.Columns.Add("支付金额", typeof(decimal));
            dt.Columns.Add("运费金额  ", typeof(decimal));
            dt.Columns.Add("抵用券金额  ", typeof(decimal));
            foreach (var d in orders)
            {
                var yjcouponPrice = orderItems.Where(_ => _.CommodityOrderId == d.Id).Sum(_ => _.YJCouponPrice);

                var payPrice = d.Price - yjcouponPrice;

                if (d.RealPrice > (payPrice < 0 ? 0 : payPrice) + d.Freight)
                {
                    dt.Rows.Add(d.AppName, d.Code, d.Price, d.RealPrice, d.Freight, yjcouponPrice);
                }
            }
            return dt;
        }

        public static string ImportMallApp(Guid appId)
        {
            if (appId == Guid.Empty)
                return "APPID为空";
            var appInfo = new APPSVFacade().GetAppOwnerInfo(appId);
            // 获取场馆下所有APP
            var apps = Jinher.AMP.BTP.TPS.ZPHSV.Instance.GetPavilionApp(new ZPH.Deploy.CustomDTO.QueryPavilionAppParam()
            {
                Id = appId,
                pageIndex = 1,
                pageSize = int.MaxValue
            }).Data;
            if (apps.Count == 0)
            {
                return "场馆下无APP";
            }
            var pavilionInfo = Jinher.AMP.BTP.TPS.ZPHSV.Instance.GetAppPavilionInfo(new ZPH.Deploy.CustomDTO.QueryAppPavilionParam { id = appId });
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            foreach (var app in apps)
            {
                var existCount = MallApply.ObjectSet().Where(m => m.AppId == app.appId && m.EsAppId == appId).Count();
                if (existCount == 0)
                {
                    MallApply entity = new MallApply();
                    entity.Id = Guid.NewGuid();
                    entity.EsAppId = appId;
                    entity.EsAppName = pavilionInfo.pavilionName;
                    entity.AppId = app.appId;
                    entity.AppName = app.appName;
                    entity.SubTime = DateTime.Now;
                    entity.ModifiedOn = DateTime.Now;
                    entity.UserId = appInfo.OwnerId;
                    entity.State = new ApplyStateVO { Value = 2 };
                    entity.Type = 1;
                    entity.EntityState = EntityState.Added;
                    contextSession.SaveObject(entity);
                }
            }
            try
            {
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "ok";
        }

        //添加app
        public static string AddPavilionApp(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO model)
        {
            ResultDTO dto = null;
            try
            {
                PavilionAppFacade facade = new PavilionAppFacade();
                if (string.IsNullOrWhiteSpace(model.AppId.ToString()) && (!model.AppId.ToString().Contains("00000000-0000-0000-0000-000000000000")))
                {
                    throw new Exception("appId不能为空");
                }
                if (string.IsNullOrWhiteSpace(model.EsAppId.ToString()) && (!model.EsAppId.ToString().Contains("00000000-0000-0000-0000-000000000000")))
                {
                    throw new Exception("esappId不能为空");
                }
                Jinher.AMP.ZPH.Deploy.CustomDTO.PavilionAppCDTO pavilionapp = new Jinher.AMP.ZPH.Deploy.CustomDTO.PavilionAppCDTO();
                pavilionapp.id = Guid.NewGuid();
                pavilionapp.appId = model.AppId;
                pavilionapp.appName = model.AppName;
                pavilionapp.belongTo = model.EsAppId;
                var result = APPSV.GetAppNameIcon(model.AppId);
                if (!string.IsNullOrEmpty(result.AppIcon))
                {
                    pavilionapp.appIcon = result.AppIcon;
                }
                else
                {
                    pavilionapp.appIcon = "找不到";
                }
                if (!string.IsNullOrEmpty(result.OwnerId.ToString()) && (!result.OwnerId.ToString().Contains("00000000-0000-0000-0000-000000000000")))
                {
                    pavilionapp.appAccount = result.OwnerId.ToString();
                }
                else
                {
                    pavilionapp.appAccount = Guid.NewGuid().ToString();
                }
                pavilionapp.appCreateOn = result.CreateDate;
                pavilionapp.subId = model.UserId;
                pavilionapp.subTime = DateTime.Now;
                pavilionapp.modifiedOn = DateTime.Now;
                dto = facade.SavePavilionApp(pavilionapp);
            }
            catch (Exception ex)
            {
                dto = new ResultDTO { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return dto.Message;
        }


        //删除app
        public static string delPavilionApp(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO model)
        {
            ResultDTO dto = null;
            try
            {
                PavilionAppFacade facade = new PavilionAppFacade();
                if (string.IsNullOrWhiteSpace(model.AppId.ToString()) && (!model.AppId.ToString().Contains("00000000-0000-0000-0000-000000000000")))
                {
                    throw new Exception("appId不能为空");
                }
                if (string.IsNullOrWhiteSpace(model.EsAppId.ToString()) && (!model.EsAppId.ToString().Contains("00000000-0000-0000-0000-000000000000")))
                {
                    throw new Exception("esappId不能为空");
                }
                var pavilionapp = new Jinher.AMP.ZPH.Deploy.CustomDTO.PavilionAppCDTO();
                pavilionapp.appId = model.AppId;
                pavilionapp.belongTo = model.EsAppId;
                dto = facade.DelPavilionApp(pavilionapp);
            }
            catch (Exception ex)
            {
                dto = new ResultDTO { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return dto.Message;
        }
    }
}
