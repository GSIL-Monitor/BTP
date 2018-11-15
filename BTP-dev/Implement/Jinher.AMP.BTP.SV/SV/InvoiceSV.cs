
/***************
功能描述: BTPSV
作    者: 
创建时间: 2016/6/6 11:44:08
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class InvoiceSV : BaseSv, IInvoice
    {

        /// <summary>
        /// 获取订单
        /// </summary>
        /// <param name="search">发票设置必传参数，AppIds，UserId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.InvoiceSettingDTO> GetInvoiceSetting(Jinher.AMP.BTP.Deploy.CustomDTO.InvoiceSearchDTO search)
        {
            base.Do();
            return this.GetInvoiceSettingExt(search);

        }


        /// <summary>
        /// 获取发票历史数据
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <param name="category">发票类型 1:增值税专用发票,2:电子发票,4:增值税专用发票</param>
        /// <returns></returns>
        public Deploy.CustomDTO.ResultDTO<List<InvoiceInfoDTO>> GetInvoiceInfoList(Guid appId, Guid userId, int category)
        {
            base.Do();
            return this.GetInvoiceInfoListExt(appId, userId, category);

        }
    }
}