
/***************
功能描述: BTPBP
作    者: LSH
创建时间: 2017/9/21 15:02:29
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
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class SpecificationsBP : BaseBP, ISpecifications
    {
        /// <summary>
        /// 查询商品规格
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<Jinher.AMP.BTP.Deploy.SpecificationsDTO> GetSpecificationsList(Jinher.AMP.BTP.Deploy.SpecificationsDTO search)
        {
            this.Do();
            return this.GetSpecificationsListExt(search);
        }

        /// <summary>
        /// 保存商品规格
        /// </summary>
        /// <param name="VatInvoiceProof"></param>
        /// <returns></returns>
        public ResultDTO SaveSpecifications(Jinher.AMP.BTP.Deploy.SpecificationsDTO model)
        {
            this.Do();
            return this.SaveSpecificationsExt(model);
        }


        /// <summary>
        /// 根据id删除商品规格信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO Del(Guid id)
        {
            this.Do();
            return this.DelExt(id);
        }
    }
}