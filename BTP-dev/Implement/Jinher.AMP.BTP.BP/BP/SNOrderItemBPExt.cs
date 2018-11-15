using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.BP.Base;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.PL;
using Jinher.AMP.BTP.BE;
using System.Data;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.BTP.Deploy.CustomDTO.SN;
using Jinher.AMP.BTP.TPS.Helper;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.BP
{
    public partial class SNOrderItemBP : BaseBP, ISNOrderItem
    {
        public bool AddSNOrderItemExt(List<Deploy.SNOrderItemDTO> snOrderItem)
        {
            if (snOrderItem == null)
                return true;
            if (snOrderItem.Count == 0)
                return true;
            snOrderItem.ForEach(item =>
            {
                SNOrderItem.ObjectSet().AddObject(new SNOrderItem()
                {
                    Id = item.Id,
                    OrderId = item.OrderId,
                    OrderItemId = item.OrderItemId,
                    OrderCode = item.OrderCode,
                    CustomOrderId = item.CustomOrderId,
                    CustomOrderItemId = item.CustomOrderItemId,
                    CustomSkuId = item.CustomSkuId,
                    ExpressStatus = item.ExpressStatus,
                    Status = item.Status,
                    DeliveryType = item.DeliveryType,
                    SubTime = item.SubTime,
                    ModifiedOn = item.ModifiedOn,
                    EntityState = EntityState.Added
                });
            });
            return ContextFactory.CurrentThreadContext.SaveChanges() > 0;
        }

        public bool UpdSNOrderItemExt(Deploy.SNOrderItemDTO snOrderItem)
        {
            var EntityQuery = SNOrderItem.ObjectSet();
            if (snOrderItem.OrderId != null)
                EntityQuery.Where(w => w.OrderId == snOrderItem.OrderId);
            if (snOrderItem.OrderItemId != null)
                EntityQuery.Where(w => w.OrderItemId == snOrderItem.OrderItemId);
            if (snOrderItem.OrderCode != null)
                EntityQuery.Where(w => w.OrderCode == snOrderItem.OrderCode);


            if (snOrderItem.CustomOrderId != null)
                EntityQuery.Where(w => w.CustomOrderId == snOrderItem.CustomOrderId);
            if (snOrderItem.CustomOrderItemId != null)
                EntityQuery.Where(w => w.CustomOrderItemId == snOrderItem.CustomOrderItemId);
            if (snOrderItem.CustomSkuId != null)
                EntityQuery.Where(w => w.CustomSkuId == snOrderItem.CustomSkuId);
            var EntityModels = EntityQuery.ToList();
            if (EntityModels == null)
                return true;
            EntityModels.ForEach(item =>
            {
                item.Status = snOrderItem.Status;
                item.ModifiedOn = DateTime.Now;
                item.EntityState = EntityState.Modified;
                SNOrderItem.ObjectSet().Attach(item);
            });
            return ContextFactory.CurrentThreadContext.SaveChanges() > 0;
        }

        public bool ChangeOrderStatusForJobExt()
        {
            try
            {
                //更改订单状态
                var DataList = SuningSV.suning_govbus_message_get("11");
                DataList.ForEach(Item =>
                {
                    var EntityList = SNOrderItem.ObjectSet()
                                      .Where(w => w.CustomOrderId == Item.orderNo)
                                      .ToList();
                    if (EntityList != null)
                    {
                        EntityList.ForEach(ItemModel =>
                        {
                            ItemModel.Status = Convert.ToInt32(Item.status);
                            ItemModel.EntityState = EntityState.Modified;
                            SNOrderItem.ObjectSet().Attach(ItemModel);
                            //根据ID从消息池删除数据
                            if (ContextFactory.CurrentThreadContext.SaveChanges() >= 0)
                                SuningSV.suning_govbus_message_delete(Item.id);
                        });
                    }
                });
                //检查确认预占失败的订单
                SNOrderItem.ObjectSet().Where(w => w.Status < 3).ToList()
                .ForEach(Item =>
                {
                    SuningSV.suning_govbus_confirmorder_add(Item.OrderId, false);
                });
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("Job-SuningChangeOrderStatus：{0}", ex.Message));
                return false;
            }
        }

        public bool OrderConfirmReceivedExt(Guid OrderId)
        {
            var OrderItemList = new SNConfirmParamsDto();
            OrderItemList.OrderId = OrderId.ToString();
            OrderItemList.SkuConfirmList = SNOrderAfterSalesHelper
                                         .SNJudgeIsFactoryDeliveryByOrderId(OrderId)
                                         .Where(w => w.IsFactorySend == true)
                                         .Select(s => new SNConfirmItemParamsDto
                                         {
                                             ConfirmTime = DateTime.Now.ToString(),
                                             SkuId = s.SkuId
                                         }).ToList();
            if (OrderItemList.SkuConfirmList.Count == 0)
                return true;
            return SuningSV.suning_govbus_facproduct_confirm(OrderItemList);
        }
    }
}
