
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2016/5/18 14:39:01
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.ISV.Facade
{
    public class DiyGroupFacade : BaseFacade<IDiyGroup>
    {

        /// <summary>
        /// 获取拼团详情
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupDetailDTO> GetDiyGroupDetail(Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupDetailSearchDTO search)
        {
            base.Do();
            return this.Command.GetDiyGroupDetail(search);
        }
        /// <summary>
        /// 处理超时未成团
        /// </summary>
        public void DealUnDiyGroupTimeout()
        {
            base.Do();
            this.Command.DealUnDiyGroupTimeout();
        }
        /// <summary>
        /// 处理 未成团退款
        /// </summary>
        public void DealUnDiyGroupRefund()
        {
            base.Do();
            this.Command.DealUnDiyGroupRefund();
        }
        /// <summary>
        /// 我的拼团订单列表
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupOrderListDTO> GetDiyGroupList(Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupSearchDTO search)
        {
            base.Do();
            return this.Command.GetDiyGroupList(search);
        }

        /// <summary>
        /// 自动确认成团 -- JOB调用
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO VoluntarilyConfirmDiyGroup()
        {
            base.Do();
            return this.Command.VoluntarilyConfirmDiyGroup();
        }

        /// <summary>
        /// 成团自动退款 -- JOB调用
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO VoluntarilyRefundDiyGroup()
        {
            base.Do();
            return this.Command.VoluntarilyRefundDiyGroup();
        }

        /// <summary>
        /// 检查拼团状态
        /// </summary>
        /// <param name="inputDTO"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<CheckDiyGroupOutputDTO> CheckDiyGroup(CheckDiyGroupInputDTO inputDTO)
        {
            base.Do();
            return this.Command.CheckDiyGroup(inputDTO);
        }
    }
}