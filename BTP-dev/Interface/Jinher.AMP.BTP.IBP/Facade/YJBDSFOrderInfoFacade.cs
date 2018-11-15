
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2018/7/31 16:47:24
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.IBP.IService;

namespace Jinher.AMP.BTP.IBP.Facade
{
    public class YJBDSFOrderInfoFacade : BaseFacade<IYJBDSFOrderInfo>
    {

        /// <summary>
        /// 根据订单号获取订单数据
        /// </summary>
        /// <param name="OrderNos"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderInfoAndCarRebateDTO>> GetDSFOrderInfoByOrderNos(System.Collections.Generic.List<string> OrderNos)
        {
            base.Do();
            return this.Command.GetDSFOrderInfoByOrderNos(OrderNos);
        }
        /// <summary>
        /// 获取保险返利数据
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CarInsurancePolymerizationDTO>> GetCarInsuranceRebate(Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceRebateSearchDTO arg)
        {
            base.Do();
            return this.Command.GetCarInsuranceRebate(arg);
        }
        /// <summary>
        /// 根据汇款单号获取返利数据
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceRebateDataDTO>> GetCarRebateByRemittanceNo(Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceRebateSearchDTO arg)
        {
            base.Do();
            return this.Command.GetCarRebateByRemittanceNo(arg);
        }
        /// <summary>
        /// 查询统计报表
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceReportDTO>> GetCarInsuranceReport(Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceReportSearchDTO arg)
        {
            base.Do();
            return this.Command.GetCarInsuranceReport(arg);
        }
        /// <summary>
        /// 更新返利状态
        /// </summary>
        /// <param name="OrderNO"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateCarRebateState(string OrderNO, int State)
        {
            base.Do();
            return this.Command.UpdateCarRebateState(OrderNO, State);
        }
    }
}