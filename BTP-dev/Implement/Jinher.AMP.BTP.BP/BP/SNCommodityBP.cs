
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/10/28 10:51:51
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
    public partial class SNCommodityBP : BaseBP, ISNCommodity
    {

        /// <summary>
        /// 新建商品信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddSNCommodity(Jinher.AMP.BTP.Deploy.JdCommodityDTO input)
        {
            base.Do(false);
            return this.AddSNCommodityExt(input);
        }
        /// <summary>
        /// 导入苏宁商品信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> ImportSNCommodityData(System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.JdCommodityDTO> JdComList, System.Guid AppId)
        {
            base.Do(false);
            return this.ImportSNCommodityDataExt(JdComList, AppId);
        }
        /// <summary>
        /// 自动同步苏宁商品信息
        /// </summary>
        /// <param name="AppId"></param>
        /// <param name="Ids"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> AutoSyncSNCommodityInfo(System.Guid AppId, System.Collections.Generic.List<System.Guid> Ids)
        {
            base.Do(false);
            return this.AutoSyncSNCommodityInfoExt(AppId, Ids);
        }
        /// <summary>
        /// 同步商品列表图片
        ///  </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateComPic()
        {
            base.Do(false);
            return this.UpdateComPicExt();
        }
        /// <summary>
        /// 导出苏宁进货价差异商品列表
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.Commodity.SNComCostDiffDTO> GetSNDiffCostPrice()
        {
            base.Do(false);
            return this.GetSNDiffCostPriceExt();
        }
        /// <summary>
        /// 全量同步苏宁进货价
        ///  </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SynSNCostPrice()
        {
            base.Do(false);
            return this.SynSNCostPriceExt();
        }
        /// <summary>
        /// 苏宁对账
        ///  </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SynSNBill()
        {
            base.Do(false);
            return this.SynSNBillExt();
        }
        /// <summary>
        /// 苏宁对账2
        ///  </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SynSNBill2()
        {
            base.Do(false);
            return this.SynSNBill2Ext();
        }
    }
}