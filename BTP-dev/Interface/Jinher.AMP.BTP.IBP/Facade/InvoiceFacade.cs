
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2016/5/29 11:37:04
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.IBP.Facade
{
    public class InvoiceFacade : BaseFacade<IInvoice>
    {
        /// <summary>
        /// 查询发票信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.InvoiceResultDTO> GetInvoiceInfoList(Jinher.AMP.BTP.Deploy.CustomDTO.InvoiceSearchDTO search)
        {
            base.Do();
            return this.Command.GetInvoiceInfoList(search);
        }
        /// <summary>
        /// 保存增值税发票资质信息
        /// </summary>
        /// <param name="VatInvoiceProof"></param>
        /// <returns></returns>
        public ResultDTO SaveVatInvoiceProof(Jinher.AMP.BTP.Deploy.CustomDTO.VatInvoiceProofInfoDTO VatInvoiceP)
        {
            base.Do();
            return this.Command.SaveVatInvoiceProof(VatInvoiceP);
        }
        /// <summary>
        /// 设置发票类型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetInvoiceCategory(Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO model)
        {
            base.Do();
            return this.Command.SetInvoiceCategory(model);
        }

        /// <summary>
        /// 修改发票
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateInvoice(List<Jinher.AMP.BTP.Deploy.CustomDTO.InvoiceUpdateDTO> list)
        {
            base.Do();
            return this.Command.UpdateInvoice(list);
        }
        /// <summary>
        /// 获取全局设置的发票类型
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO> GetInvoiceCategory(Guid appId)
        {
            base.Do();
            return this.Command.GetInvoiceCategory(appId);
        }
        /// <summary>
        /// 显示增值税发票信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.VatInvoiceProofInfoDTO> ShowVatInvoiceProof(Guid userId)
        {
            base.Do();
            return this.Command.ShowVatInvoiceProof(userId);
        }

        /// <summary>
        /// 显示增值税发票信息 金采支付专用
        /// </summary>
        /// <param name="jcActivityId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.VatInvoiceProofInfoDTO> ShowVatInvoiceProofII(Guid jcActivityId)
        {
            base.Do();
            return this.Command.ShowVatInvoiceProofII(jcActivityId);
        }

        /// <summary>
        /// 获取导出的Excel数据
        /// </summary>
        public List<InvoiceExportDTO> GetInvoiceExport(InvoiceExportDTO search)
        {
            base.Do();
            return this.Command.GetInvoiceExport(search);
        }

        /// <summary>
        /// 获取导出的电子发票的详细数据
        /// </summary>
        public List<ElectronicInvoiceDTO> GetInvoiceExportDetail(Jinher.AMP.BTP.Deploy.CustomDTO.InvoiceSearchDTO search)
        {
            base.Do();
            return this.Command.GetInvoiceExportDetail(search);
        }


        
    }
}