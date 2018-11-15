using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy.CustomDTO.AfterSales;
using Jinher.AMP.BTP.Deploy.Enum;

namespace Jinher.AMP.BTP.IBP.Facade
{
    /// <summary>
    /// 苏宁--售后
    /// </summary>
    public class SNAfterSaleFacade : BaseFacade<ISNAfterSale>
    {
        ///// <summary>
        ///// 苏宁--单品申请售后
        ///// </summary>
        ///// <param name="reqDto"></param>
        ///// <returns></returns>
        //public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SnReturnPartOrder(SNReturnPartOrderDTO reqDto, SNFactoryDeliveryEnum ty)
        //{
        //    base.Do();
        //    return this.Command.SnReturnPartOrder(reqDto,ty);
        //}
        ///// <summary>
        ///// 苏宁--整单申请售后
        ///// </summary>
        ///// <param name="reqDto"></param>
        ///// <returns></returns>
        //public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SnApplyRejected(SNApplyRejectedDTO reqDto, SNFactoryDeliveryEnum ty)
        //{
        //    base.Do();
        //    return this.Command.SnApplyRejected(reqDto,ty);
        //}
        /// <summary>
        /// 获取苏宁订单详情
        /// </summary>
        /// <param name="reqDto"></param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.SNOrderItemDTO> GetSNOrderItemList(Jinher.AMP.BTP.Deploy.SNOrderItemDTO reqDto)
        {
            base.Do();
            return this.Command.GetSNOrderItemList(reqDto);
        }
    }
}
