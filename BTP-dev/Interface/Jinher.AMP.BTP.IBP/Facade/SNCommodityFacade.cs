
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2018/10/29 11:00:34
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
    public class SNCommodityFacade : BaseFacade<ISNCommodity>
    {

        /// <summary>
        /// 新建商品信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddSNCommodity(Jinher.AMP.BTP.Deploy.JdCommodityDTO input)
        {
            base.Do();
            return this.Command.AddSNCommodity(input);
        }
        /// <summary>
        /// 导入苏宁商品信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> ImportSNCommodityData(System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.JdCommodityDTO> JdComList, System.Guid AppId)
        {
            base.Do();
            return this.Command.ImportSNCommodityData(JdComList, AppId);
        }
        /// <summary>
        /// 自动同步苏宁商品信息
        /// </summary>
        /// <param name="AppId"></param>
        /// <param name="Ids"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> AutoSyncSNCommodityInfo(System.Guid AppId, System.Collections.Generic.List<System.Guid> Ids)
        {
            base.Do();
            return this.Command.AutoSyncSNCommodityInfo(AppId, Ids);
        }
        /// <summary>
        /// 同步商品列表图片
        ///  </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateComPic()
        {
            base.Do();
            return this.Command.UpdateComPic();
        }
        /// <summary>
        /// 导出苏宁进货价差异商品列表
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.Commodity.SNComCostDiffDTO> GetSNDiffCostPrice()
        {
            base.Do();
            return this.Command.GetSNDiffCostPrice();
        }
        /// <summary>
        /// 全量同步苏宁进货价
        ///  </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SynSNCostPrice()
        {
            base.Do();
            return this.Command.SynSNCostPrice();
        }
        /// <summary>
        /// 苏宁对账1
        ///  </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SynSNBill()
        {
            base.Do();
            return this.Command.SynSNBill();
        }
        /// <summary>
        /// 苏宁对账2
        ///  </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SynSNBill2()
        {
            base.Do();
            return this.Command.SynSNBill2();
        }
    }
}