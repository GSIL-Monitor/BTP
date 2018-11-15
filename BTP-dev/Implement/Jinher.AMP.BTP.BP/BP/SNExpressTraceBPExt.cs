using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;
using Jinher.AMP.BTP.BE;
using System.Data;
using Jinher.JAP.PL;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.BTP.Deploy.CustomDTO.SN;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.Common;
using System.Data.Objects;
using Jinher.JAP.Common.Loging;
using System.Globalization;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.BP
{
    public partial class SNExpressTraceBP : BaseBP, ISNExpressTrace
    {
        /// <summary>
        /// 获取苏宁物流信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderItemId"></param>
        /// <returns></returns>
        public List<Deploy.SNExpressTraceDTO> GetExpressTraceExt(string orderId, string orderItemId)
        {
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            List<SNExpressTraceDTO> SNExpressList = new List<SNExpressTraceDTO>();
            var OrderItemGuid = Guid.Parse(orderItemId);
            var EntityQuery = SNOrderItem.ObjectSet().Where(w => w.OrderItemId == OrderItemGuid).FirstOrDefault();
            LogHelper.Info(string.Format("苏宁物流3"));
            if (EntityQuery != null)
            {
                SuningSV.suning_govbus_orderlogistnew_get(new SNQueryParamsDto
                {
                    OrderId = EntityQuery.CustomOrderId,
                    OrderItemIds = new List<SNQueryItemParamsDto>
                            {
                                new SNQueryItemParamsDto
                                {
                                    SkuId=EntityQuery.CustomSkuId,
                                    OrderItemId=EntityQuery.CustomOrderItemId
                                }
                            }
                })
                .ForEach(ItemPackage =>
                {
                    ItemPackage.OrderLogistics.ForEach(LogisticsItem =>
                    {
                        SNExpressTraceDTO SNExpress = new SNExpressTraceDTO();
                        SNExpress.Id = Guid.NewGuid();
                        SNExpress.OrderId = ItemPackage.OrderId;
                        SNExpress.PackageId = ItemPackage.PackageId;
                        SNExpress.OperateState = LogisticsItem.OperateState;
                        SNExpress.OperateTime = string.IsNullOrWhiteSpace(LogisticsItem.OperateTime) ? DateTime.Now : DateTime.ParseExact(LogisticsItem.OperateTime, "yyyyMMddHHmmss", CultureInfo.CurrentCulture);
                        SNExpressList.Add(SNExpress);

                    });
                });
            }
            LogHelper.Info(string.Format("苏宁物流4"));
            return SNExpressList.Count > 0 ? SNExpressList : null;
        }

        public bool ChangeLogistStatusForJobExt()
        {
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            SuningSV.suning_govbus_message_get("12")
            .ForEach(Item =>
            {
                try
                {
                    var EntityQuery = SNOrderItem.ObjectSet()
                                      .Where(w => w.CustomOrderId == Item.orderNo)
                                      .Where(w => w.CustomOrderItemId == Item.orderItemNo)
                                      .FirstOrDefault();
                    if (EntityQuery != null)
                    {
                        EntityQuery.ExpressStatus = Convert.ToInt32(Item.status);
                        //根据订单获得物流信息
                        SuningSV.suning_govbus_orderlogistnew_get(new SNQueryParamsDto
                        {
                            OrderId = EntityQuery.CustomOrderId,
                            OrderItemIds = new List<SNQueryItemParamsDto>
                            {
                                new SNQueryItemParamsDto
                                {
                                    SkuId=EntityQuery.CustomSkuId,
                                    OrderItemId=EntityQuery.CustomOrderItemId
                                }
                            }
                        })
                        .ForEach(ItemPackage =>
                        {
                            ItemPackage.OrderItemIds.ForEach(OrderItem =>
                            {
                                var EntityModel = SNPackageTrace.ObjectSet()
                                                .Where(w => w.OrderId == ItemPackage.OrderId)
                                                .Where(w => w.OrderItemId == OrderItem.OrderItemId)
                                                .Where(w => w.SkuId == OrderItem.SkuId)
                                                .Where(w => w.PackageId == ItemPackage.PackageId)
                                                .FirstOrDefault();
                                if (EntityModel == null)
                                {
                                    contextSession.SaveObject(new SNPackageTrace
                                    {
                                        Id = Guid.NewGuid(),
                                        CommodityOrderId = EntityQuery.OrderId,
                                        OrderId = ItemPackage.OrderId,
                                        OrderItemId = OrderItem.OrderItemId,
                                        SkuId = OrderItem.SkuId,
                                        PackageId = ItemPackage.PackageId,
                                        ReceiveTime = string.IsNullOrWhiteSpace(ItemPackage.ReceiveTime) ? DateTime.Now : DateTime.ParseExact(ItemPackage.ReceiveTime, "yyyyMMddHHmmss", CultureInfo.CurrentCulture),
                                        ShippingTime = string.IsNullOrWhiteSpace(ItemPackage.ShippingTime) ? DateTime.Now : DateTime.ParseExact(ItemPackage.ShippingTime, "yyyyMMddHHmmss", CultureInfo.CurrentCulture),
                                        EntityState = EntityState.Added
                                    });
                                }
                                else
                                {
                                    EntityModel.ReceiveTime = string.IsNullOrWhiteSpace(ItemPackage.ReceiveTime) ? DateTime.Now : DateTime.ParseExact(ItemPackage.ReceiveTime, "yyyyMMddHHmmss", CultureInfo.CurrentCulture);
                                    EntityModel.ShippingTime = string.IsNullOrWhiteSpace(ItemPackage.ShippingTime) ? DateTime.Now : DateTime.ParseExact(ItemPackage.ShippingTime, "yyyyMMddHHmmss", CultureInfo.CurrentCulture);
                                    EntityModel.EntityState = EntityState.Modified;
                                    contextSession.SaveObject(EntityModel);
                                }
                            });
                            ItemPackage.OrderLogistics.ForEach(LogisticsItem =>
                            {
                                if (SNExpressTrace.ObjectSet().Where(w => w.OrderId == ItemPackage.OrderId && w.PackageId == ItemPackage.PackageId).FirstOrDefault() == null)
                                {
                                    contextSession.SaveObject(new SNExpressTrace
                                    {
                                        Id = Guid.NewGuid(),
                                        OrderId = ItemPackage.OrderId,
                                        PackageId = ItemPackage.PackageId,
                                        OperateState = LogisticsItem.OperateState,
                                        OperateTime = string.IsNullOrWhiteSpace(LogisticsItem.OperateTime) ? DateTime.Now : DateTime.ParseExact(LogisticsItem.OperateTime, "yyyyMMddHHmmss", CultureInfo.CurrentCulture),
                                        EntityState = EntityState.Added
                                    });
                                }
                            });
                        });
                        //根据ID从消息池删除数据
                        if (contextSession.SaveChanges() >= 0)
                        {
                            //根据订单ID改订单状态
                            SuningSV.suning_govbus_rejection_changestatus(Item.status, EntityQuery);
                            SuningSV.suning_govbus_message_delete(Item.id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(string.Format("Job-SuningChangeLogistStatus：{0}", ex));
                }
            });
            return true;
        }

    }
}
