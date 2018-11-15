
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/7/31 16:47:25
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class YJBDSFOrderInfoBP : BaseBP, IYJBDSFOrderInfo
    {

        /// <summary>
        /// 根据订单号获取订单数据
        /// </summary>
        /// <param name="OrderNos"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderInfoAndCarRebateDTO>> GetDSFOrderInfoByOrderNos(System.Collections.Generic.List<string> OrderNos)
        {
            base.Do(false);
            return this.GetDSFOrderInfoByOrderNosExt(OrderNos);
        }
        /// <summary>
        /// 获取保险返利数据
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CarInsurancePolymerizationDTO>> GetCarInsuranceRebate(Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceRebateSearchDTO arg)
        {
            base.Do(false);
            return this.GetCarInsuranceRebateExt(arg);
        }
        /// <summary>
        /// 根据汇款单号获取返利数据
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceRebateDataDTO>> GetCarRebateByRemittanceNo(Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceRebateSearchDTO arg)
        {
            base.Do(false);
            return this.GetCarRebateByRemittanceNoExt(arg);
        }
        /// <summary>
        /// 查询统计报表
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceReportDTO>> GetCarInsuranceReport(Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceReportSearchDTO arg)
        {
            base.Do(false);
            return this.GetCarInsuranceReportExt(arg);
        }
        /// <summary>
        /// 更新返利状态
        /// </summary>
        /// <param name="OrderNO"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateCarRebateState(string OrderNO, int State)
        {
            base.Do(false);
            return this.UpdateCarRebateStateExt(OrderNO, State);
        }
    }
}