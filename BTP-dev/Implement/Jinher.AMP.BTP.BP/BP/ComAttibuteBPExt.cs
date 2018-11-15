
/***************
功能描述: BTPBP
作    者: 
创建时间: 2014/3/25 15:13:02
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
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class ComAttibuteBP : BaseBP, IComAttibute
    {

        /// <summary>
        /// 添加商品颜色/尺寸
        /// </summary>
        /// <param name="secondAttributeIds">尺寸、颜色ID</param>
        /// <param name="commodityId">商品ID</param>
        /// <param name="attributeId">一级属性ID</param>
        public void AddComAttibuteExt(System.Collections.Generic.List<System.Guid> secondAttributeIds, System.Guid commodityId, System.Guid attributeId)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 获取商品颜色/尺寸
        /// </summary>
        /// <param name="appId">appid</param>
        /// <param name="commodityId">商品ID</param>
        /// <param name="attributeId">属性ID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<ColorAndSizeAttributeVM> GetColorOrSizeByAppIdExt(System.Guid appId, System.Guid commodityId, System.Guid attributeId)
        {
            var query = from n in ComAttibute.ObjectSet()
                        join m in Jinher.AMP.BTP.BE.Attribute.ObjectSet() on n.AttributeId equals m.Id
                        join b in SecondAttribute.ObjectSet() on n.SecondAttributeId equals b.Id
                        where m.AppId == appId && n.CommodityId == commodityId && n.AttributeId == attributeId
                        select new ColorAndSizeAttributeVM
                        {
                            AttributeId = n.AttributeId,
                            SecondAttributeId = n.SecondAttributeId,
                            AttributeName = m.Name,
                            SecondAttributeName = b.Name,
                            AppId = m.AppId,
                            CommodityId = n.CommodityId
                        };

            return query.ToList();
        }

        /// <summary>
        /// 获取商家颜色/尺寸
        /// </summary>
        /// <param name="appId">appid</param>
        /// <returns></returns>
        public System.Collections.Generic.List<SecondAttributeDTO> GetSecondAttributeExt(System.Guid appId)
        {
            var query = SecondAttribute.ObjectSet().Where(n => (n.AppId == appId && n.IsDel != true));

            var result = from s in query
                         select new SecondAttributeDTO
                         {
                             AppId = s.AppId,
                             AttributeId = s.AttributeId,
                             Code = s.Code,
                             Id = s.Id,
                             IsDel = s.IsDel,
                             Name = s.Name
                         };
            return result.ToList();
        }
    }
}