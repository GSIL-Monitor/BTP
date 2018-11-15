
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2016/6/6 11:44:07
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.ISV.IService;

namespace Jinher.AMP.BTP.ISV.Facade
{
    public class InvoiceFacade : BaseFacade<IInvoice>
    {

        /// <summary>
        /// 获取订单
        /// </summary>
        /// <param name="search">发票设置必传参数，AppIds，UserId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.InvoiceSettingDTO> GetInvoiceSetting(Jinher.AMP.BTP.Deploy.CustomDTO.InvoiceSearchDTO search)
        {
            base.Do();
            return this.Command.GetInvoiceSetting(search);
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
            return this.Command.GetInvoiceInfoList(appId, userId, category);
        }
    }
}