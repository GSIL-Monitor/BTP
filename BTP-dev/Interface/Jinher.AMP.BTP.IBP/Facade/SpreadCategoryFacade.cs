
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2017/5/31 18:12:26
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
    public class SpreadCategoryFacade : BaseFacade<ISpreadCategory>
    {

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO Add(Jinher.AMP.BTP.Deploy.SpreadCategoryDTO dto)
        {
            base.Do();
            return this.Command.Add(dto);
        }
        /// <summary>
        ///  删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO Delete(System.Guid id)
        {
            base.Do();
            return this.Command.Delete(id);
        }
        /// <summary>
        /// 获取推广主类型
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.SpreadCategoryDTO>> GetSpreadCategoryList(Jinher.AMP.BTP.Deploy.CustomDTO.SpreadSearchDTO search)
        {
            base.Do();
            return this.Command.GetSpreadCategoryList(search);
        }
    }
}