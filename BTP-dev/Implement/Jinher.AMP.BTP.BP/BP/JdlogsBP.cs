
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
    public partial class JdlogsBP : BaseBP, IJdlogs
    {
        /// <summary>
        /// 查询所有的京东日志信息
        /// </summary>
        /// <param name="search">查询类</param>
        public List<Jinher.AMP.BTP.Deploy.JdlogsDTO> GetALLJdlogsList(Jinher.AMP.BTP.Deploy.CustomDTO.JdlogsDTO model)
        {
            base.Do();
            return this.GetALLJdlogsListExt(model);
        }


        /// <summary>
        /// 根据Id获取京东的日志内容
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.JdlogsDTO GetJdlogs(Guid Id)
        {
            base.Do();
            return this.GetJdlogsExt(Id);
        }



        /// <summary>
        /// 保存京东日志信息
        /// </summary>
        /// <param name="VatInvoiceProof"></param>
        /// <returns></returns>
        public ResultDTO SaveJdlogs(Jinher.AMP.BTP.Deploy.JdlogsDTO model)
        {
            base.Do(false);
            return this.SaveJdlogsExt(model);
        }


        /// <summary>
        ///修改京东日志信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO UpdateJdlogs(Jinher.AMP.BTP.Deploy.JdlogsDTO model)
        {
            base.Do();
            return this.UpdateJdlogsExt(model);
        }


        /// <summary>
        /// 根据id删除京东日志信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO DeleteJdlogs(Guid id)
        {
            base.Do();
            return this.DeleteJdlogsExt(id);
        }

    }
}