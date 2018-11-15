using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;
using Jinher.AMP.BTP.BE;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.Enum;

namespace Jinher.AMP.BTP.BP
{
    /// <summary>
    /// 苏宁--售后
    /// </summary>
    public partial class SNAfterSaleBP : BaseBP, ISNAfterSale
    {


        //#region 部分退货

        ///// <summary>
        ///// 部分商品退货
        ///// </summary>
        ///// <param name="reqDto">实体数据</param>
        ///// <returns></returns>
        //public Deploy.CustomDTO.ResultDTO SnReturnPartOrderExt(Deploy.CustomDTO.AfterSales.SNReturnPartOrderDTO reqDto, SNFactoryDeliveryEnum ty)
        //{

        //    Deploy.CustomDTO.ResultDTO retDto = new Deploy.CustomDTO.ResultDTO() { ResultCode = 0, isSuccess = false, Message = "退货申请失败" };
        //    //根据苏宁接口处理自己业务
        //    //switch (ty)
        //    //{
        //    //    case SNFactoryDeliveryEnum.FactoryDelivery:
        //    //        break;
        //    //    case SNFactoryDeliveryEnum.NonFactoryDelivery:
        //    //        break;
        //    //    default:
        //    //        break;
        //    //}

        //    ////调用苏宁接口
        //    //retDto = TPS.SuningSV.ReturnPartOrder(reqDto);

        //    return retDto;
        //}
        ///// <summary>
        ///// 部分退货---厂送
        ///// </summary>
        ///// <param name="reqDto"></param>
        ///// <returns></returns>
        //private Deploy.CustomDTO.ResultDTO ReturnPartFactoryDelivery(Deploy.CustomDTO.AfterSales.SNReturnPartOrderDTO reqDto)
        //{
        //    return null;
        //}
        //#endregion
        ///// <summary>
        ///// 整单退货接口申请
        ///// </summary>
        ///// <param name="reqDto"></param>
        ///// <returns></returns>
        //public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SnApplyRejectedExt(Deploy.CustomDTO.AfterSales.SNApplyRejectedDTO reqDto, SNFactoryDeliveryEnum ty)
        //{

        //    //根据苏宁接口处理自己业务
        //    //switch (ty)
        //    //{
        //    //    case SNFactoryDeliveryEnum.FactoryDelivery:
        //    //        break;
        //    //    case SNFactoryDeliveryEnum.NonFactoryDelivery:
        //    //        break;
        //    //    default:
        //    //        break;
        //    //}

        //    //调用苏宁接口
        //    //Deploy.CustomDTO.AfterSales.SNApplyRejectedReturnDTO dto = Jinher.AMP.BTP.TPS.SuningSV.ApplyRejected(reqDto);
        //    //根据苏宁接口处理自己业务
        //    return null;
        //}

        /// <summary>
        /// 查询苏宁订单详情
        /// </summary>
        /// <returns></returns>
        public List<Deploy.SNOrderItemDTO> GetSNOrderItemListExt(Deploy.SNOrderItemDTO snOrderItem)
        {
            List<Jinher.AMP.BTP.Deploy.SNOrderItemDTO> SNOrderItemlist = new List<Jinher.AMP.BTP.Deploy.SNOrderItemDTO>();
            try
            {
                if (!snOrderItem.OrderId.Equals(Guid.Empty) && snOrderItem.OrderItemId.Equals(Guid.Empty))
                {
                    var Query = SNOrderItem.ObjectSet().Where(w => w.OrderId == snOrderItem.OrderId).ToList();
                    ExecuteQuery(SNOrderItemlist, Query);
                }
                else if (snOrderItem.OrderId.Equals(Guid.Empty) && !snOrderItem.OrderItemId.Equals(Guid.Empty))
                {
                    var Query = SNOrderItem.ObjectSet().Where(w => w.OrderItemId == snOrderItem.OrderItemId).ToList();
                    ExecuteQuery(SNOrderItemlist, Query);
                }
                else if (!snOrderItem.OrderId.Equals(Guid.Empty) && !snOrderItem.OrderItemId.Equals(Guid.Empty))
                {
                    var Query = SNOrderItem.ObjectSet()
                        .Where(w => w.OrderItemId == snOrderItem.OrderItemId && w.OrderId == snOrderItem.OrderId)
                        .ToList();
                    ExecuteQuery(SNOrderItemlist, Query);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("SNAfterSaleBP.GetSNOrderItemListExt【苏宁-售后】查询苏宁订单详情", ex);
            }
            return SNOrderItemlist;
        }

        private static void ExecuteQuery(List<Deploy.SNOrderItemDTO> SNOrderItemlist, List<SNOrderItem> Query)
        {
            Query.ForEach(p =>
            {
                Deploy.SNOrderItemDTO model = new Deploy.SNOrderItemDTO();
                model = CommonUtil.ReadObjectExchange(model, p);
                SNOrderItemlist.Add(model);
            });
        }
    }
}
