
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2018/6/29 17:00:27
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
    public class CategoryAdvertiseFacade : BaseFacade<ICategoryAdvertise>
    {

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CreateCategoryAdvertise(Jinher.AMP.BTP.Deploy.CategoryAdvertiseDTO entity)
        {
            base.Do();
            return this.Command.CreateCategoryAdvertise(entity);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.IList<Jinher.AMP.BTP.Deploy.CategoryAdvertiseDTO>> CateGoryAdvertiseList(string advertiseName, int state, System.Guid CategoryId, int pageIndex, int pageSize, out int rowCount)
        {
            base.Do();
            return this.Command.CateGoryAdvertiseList(advertiseName, state, CategoryId, pageIndex, pageSize, out rowCount);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteCategoryAdvertise(System.Guid advertiseId)
        {
            base.Do();
            return this.Command.DeleteCategoryAdvertise(advertiseId);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO EditCategoryAdvertise(Jinher.AMP.BTP.Deploy.CategoryAdvertiseDTO entity)
        {
            base.Do();
            return this.Command.EditCategoryAdvertise(entity);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CategoryAdvertiseDTO> GetCategoryAdvertise(System.Guid advertiseId)
        {
            base.Do();
            return this.Command.GetCategoryAdvertise(advertiseId);
        }
    }
}