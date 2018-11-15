
/***************
功能描述: BTPSV
作    者: 
创建时间: 2018/6/15 16:43:09
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.SV
{
    /// <summary>
    /// 
    /// </summary>
    public partial class CategoryAdvertiseSV : BaseSv, ICategoryAdvertise
    {
        public ResultDTO<CategoryAdvertiseDTO> getBrandWallSpecialByCateIDExt(System.Guid CategoryID)
        {
            var catogry = CategoryAdvertise.ObjectSet().FirstOrDefault(_ => _.CategoryId == CategoryID);
            var catDto = catogry.ToEntityData();
            var resdto = new ResultDTO<CategoryAdvertiseDTO>()
            {
                isSuccess = true,
                Data = catDto,
                Message = "Sucess",
                ResultCode = 0
            };

            return resdto;
        }
    }
}