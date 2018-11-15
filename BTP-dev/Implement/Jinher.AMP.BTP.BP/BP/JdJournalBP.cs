
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
    public partial class JdJournalBP : BaseBP, IJdJournal
    {
        /// <summary>
        /// 查询JdJournal信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>

        public List<Jinher.AMP.BTP.Deploy.JdJournalDTO> GetJdJournalList(Jinher.AMP.BTP.Deploy.JdJournalDTO search)
        {
            base.Do(false);
            return this.GetJdJournalListExt(search);
        }

        /// <summary>
        /// 保存JdJournal信息
        /// </summary>
        /// <param name="VatInvoiceProof"></param>
        /// <returns></returns>
        public ResultDTO SaveJdJournal(Jinher.AMP.BTP.Deploy.JdJournalDTO model)
        {
            base.Do(false);
            return this.SaveJdJournalExt(model);
        }

        /// <summary>
        /// 删除JdJournal
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO DeleteJdJournal(List<string> jdorders)
        {
            base.Do(false);
            return this.DeleteJdJournalExt(jdorders);
        }
    }
}