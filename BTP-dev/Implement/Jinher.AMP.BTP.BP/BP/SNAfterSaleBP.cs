using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;
using Jinher.AMP.BTP.Deploy.Enum;

namespace Jinher.AMP.BTP.BP
{
    /// <summary>
    /// 苏宁-售后
    /// </summary>
    public partial class SNAfterSaleBP : BaseBP, ISNAfterSale
    {
        ///// <summary>
        ///// 部分商品退货
        ///// </summary>
        ///// <param name="reqDto">实体数据</param>
        ///// <returns></returns>
        //public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SnReturnPartOrder(Deploy.CustomDTO.AfterSales.SNReturnPartOrderDTO reqDto, SNFactoryDeliveryEnum ty)
        //{
        //    base.Do(false);
        //    return this.SnReturnPartOrderExt(reqDto,ty);
        //}
        ///// <summary>
        ///// 整单退货接口申请
        ///// </summary>
        ///// <param name="reqDto"></param>
        ///// <returns></returns>
        //public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SnApplyRejected(Deploy.CustomDTO.AfterSales.SNApplyRejectedDTO reqDto, SNFactoryDeliveryEnum ty)
        //{
        //    base.Do(false);
        //    return this.SnApplyRejectedExt(reqDto,ty);
        //}
        /// <summary>
        /// 整单退货接口申请
        /// </summary>
        /// <param name="reqDto"></param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.SNOrderItemDTO> GetSNOrderItemList(Jinher.AMP.BTP.Deploy.SNOrderItemDTO reqDto)
        {
            base.Do(false);
            return this.GetSNOrderItemListExt(reqDto);
        }
    }
}
