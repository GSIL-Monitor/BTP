
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2018/7/29 10:59:58
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.ISV.IService;

namespace Jinher.AMP.BTP.ISV.Facade
{
    public class YJBDSFOrderInfoFacade : BaseFacade<IYJBDSFOrderInfo>
    {

        /// <summary>
        /// 获取第三方订单
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public Jinher.AMP.YJB.Deploy.CustomDTO.ListResultDTO<Jinher.AMP.BTP.Deploy.YJBDSFOrderInfoDTO> GetDSFOrderInfo(Jinher.AMP.BTP.Deploy.CustomDTO.YJBDSFOrderInfoSearchDTO arg)
        {
            base.Do();
            return this.Command.GetDSFOrderInfo(arg);
        }
        /// <summary>
        /// 插入第三方订单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Jinher.AMP.YJB.Deploy.CustomDTO.ResultDTO InsertTODSFOrderInfo(Jinher.AMP.BTP.Deploy.CustomDTO.YJBDSFOrderInformationDTO model)
        {
            base.Do();
            return this.Command.InsertTODSFOrderInfo(model);
        }
        /// <summary>
        /// 获取汇款单
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public Jinher.AMP.YJB.Deploy.CustomDTO.ListResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CarInsurancePolymerizationDTO> GetCarInsuranceRebate(Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceRebateSearchDTO arg)
        {
            base.Do();
            return this.Command.GetCarInsuranceRebate(arg);
        }
        /// <summary>
        /// 插入汇款单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Jinher.AMP.YJB.Deploy.CustomDTO.ResultDTO InsertTOCarInsuranceRebate(Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceRebateDTO model)
        {
            base.Do();
            return this.Command.InsertTOCarInsuranceRebate(model);
        }
        /// <summary>
        /// 获取保险统计报表
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public Jinher.AMP.YJB.Deploy.CustomDTO.ListResultDTO<Jinher.AMP.BTP.Deploy.YJBCarInsuranceReportDTO> GetCarInsuranceReport(Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceReportSearchDTO arg)
        {
            base.Do();
            return this.Command.GetCarInsuranceReport(arg);
        }
        /// <summary>
        /// 插入保险统计报表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Jinher.AMP.YJB.Deploy.CustomDTO.ResultDTO InsertTOCarInsuranceReport(Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceReportDTO model)
        {
            base.Do();
            return this.Command.InsertTOCarInsuranceReport(model);
        }
    }
}