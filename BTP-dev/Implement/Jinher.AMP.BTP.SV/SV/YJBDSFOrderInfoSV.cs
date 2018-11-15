
/***************
功能描述: BTPSV
作    者: 
创建时间: 2018/7/29 10:59:59
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

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class YJBDSFOrderInfoSV : BaseSv, IYJBDSFOrderInfo
    {

        /// <summary>
        /// 获取第三方订单
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public Jinher.AMP.YJB.Deploy.CustomDTO.ListResultDTO<Jinher.AMP.BTP.Deploy.YJBDSFOrderInfoDTO> GetDSFOrderInfo(Jinher.AMP.BTP.Deploy.CustomDTO.YJBDSFOrderInfoSearchDTO arg)
        {
            base.Do(false);
            return this.GetDSFOrderInfoExt(arg);

        }
        /// <summary>
        /// 插入第三方订单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Jinher.AMP.YJB.Deploy.CustomDTO.ResultDTO InsertTODSFOrderInfo(Jinher.AMP.BTP.Deploy.CustomDTO.YJBDSFOrderInformationDTO model)
        {
            base.Do(false);
            return this.InsertTODSFOrderInfoExt(model);

        }
        /// <summary>
        /// 获取汇款单
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public Jinher.AMP.YJB.Deploy.CustomDTO.ListResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CarInsurancePolymerizationDTO> GetCarInsuranceRebate(Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceRebateSearchDTO arg)
        {
            base.Do(false);
            return this.GetCarInsuranceRebateExt(arg);

        }
        /// <summary>
        /// 插入汇款单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Jinher.AMP.YJB.Deploy.CustomDTO.ResultDTO InsertTOCarInsuranceRebate(Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceRebateDTO model)
        {
            base.Do(false);
            return this.InsertTOCarInsuranceRebateExt(model);

        }
        /// <summary>
        /// 获取保险统计报表
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public Jinher.AMP.YJB.Deploy.CustomDTO.ListResultDTO<Jinher.AMP.BTP.Deploy.YJBCarInsuranceReportDTO> GetCarInsuranceReport(Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceReportSearchDTO arg)
        {
            base.Do(false);
            return this.GetCarInsuranceReportExt(arg);

        }
        /// <summary>
        /// 插入保险统计报表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Jinher.AMP.YJB.Deploy.CustomDTO.ResultDTO InsertTOCarInsuranceReport(Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceReportDTO model)
        {
            base.Do(false);
            return this.InsertTOCarInsuranceReportExt(model);

        }
    }
}