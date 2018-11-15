using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jinher.AMP.BTP.UI.Commons
{
   /// <summary>
    /// 商品分类帮助类
    /// </summary>
    public class CategoryHelper
    {
        /// <summary>
        /// 获取分类列表
        /// </summary>
        /// <param name="appId">商家Id   null 为获取所有</param>
        /// <returns></returns>
        public static List<Jinher.AMP.BTP.Deploy.CustomDTO.CategorySDTO> GetCategoryLevelOne(string appId)
        {
            //获取分类******
            Guid apId = Guid.Parse("1375ad99-de3b-4e93-80d5-5b96e1588967");
            if (!string.IsNullOrWhiteSpace(appId))
            {
                apId = Guid.Parse(appId);
            }
            IBP.Facade.CategoryFacade caty = new IBP.Facade.CategoryFacade();

            return caty.GetCategoryL1(apId);
        }
    }
}