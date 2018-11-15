
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2017/9/21 15:02:27
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
    public class MallApplyFacade : BaseFacade<IMallApply>
    {

        /// <summary>
        /// 查询商城信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO> GetMallApplyInfoList(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO search)
        {
            base.Do();
            return this.Command.GetMallApplyInfoList(search);
        }
        /// <summary>
        /// 保存商城信息
        /// </summary>
        /// <param name="VatInvoiceProof"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveMallApply(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO model)
        {
            base.Do();
            return this.Command.SaveMallApply(model);
        }
        /// <summary>
        /// 修改商城信息状态
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateMallApply(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO model)
        {
            base.Do();
            return this.Command.UpdateMallApply(model);
        }
        /// <summary>
        /// 根据id获取商城信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO GetMallApply(System.Guid id)
        {
            base.Do();
            return this.Command.GetMallApply(id);
        }
        /// <summary>
        /// 验证是否存在入驻商家
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO IsHaveMallApply(System.Guid esAppId, System.Guid appId)
        {
            base.Do();
            return this.Command.IsHaveMallApply(esAppId, appId);
        }
        /// <summary>
        /// 获取商城下入住的APP
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.AppInfoDTO> GetMallApps(System.Guid esAppId)
        {
            base.Do();
            return this.Command.GetMallApps(esAppId);
        }
        /// <summary>
        /// 给盈科同步指定商城数据
        /// </summary>
        /// <returns></returns>
        public void GetMallAppsForJob(System.Guid esAppId)
        {
            base.Do();
            this.Command.GetMallAppsForJob(esAppId);
        }

    }
}