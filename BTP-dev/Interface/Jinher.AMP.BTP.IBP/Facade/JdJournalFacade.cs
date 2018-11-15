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
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.IBP.Facade
{
    public class JdJournalFacade : BaseFacade<IJdJournal>
    {


        /// <summary>
        /// 查询JdJournal信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>

        public List<Jinher.AMP.BTP.Deploy.JdJournalDTO> GetJdJournalList(Jinher.AMP.BTP.Deploy.JdJournalDTO search)
        {
            base.Do();
            return this.Command.GetJdJournalList(search);
        }


        /// <summary>
        /// 保存JdJournal信息
        /// </summary>
        /// <param name="VatInvoiceProof"></param>
        /// <returns></returns>
        public ResultDTO SaveJdJournal(Jinher.AMP.BTP.Deploy.JdJournalDTO model)
        {
            base.Do();
            return this.Command.SaveJdJournal(model);
        }

        /// <summary>
        /// 删除JdJournal
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO DeleteJdJournal(List<string> jdorders)
        {
            base.Do();
            return this.Command.DeleteJdJournal(jdorders);
        }

    }
}
