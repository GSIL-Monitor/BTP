
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2014/4/3 15:37:10
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
    public class ComAttibuteFacade : BaseFacade<IComAttibute>
    {

        /// <summary>
        /// 添加商品颜色/尺寸
        /// </summary>
        /// <param name="secondAttributeIds">尺寸、颜色ID</param>
        /// <param name="commodityId">商品ID</param>
        /// <param name="attributeId">一级属性ID</param>
        public void AddComAttibute(System.Collections.Generic.List<System.Guid> secondAttributeIds, System.Guid commodityId, System.Guid attributeId)
        {
            base.Do();
            this.Command.AddComAttibute(secondAttributeIds, commodityId, attributeId);
        }
        /// <summary>
        /// 获取商品颜色/尺寸
        /// </summary>
        /// <param name="appId">appid</param>
        /// <param name="commodityId">商品ID</param>
        /// <param name="attributeId">属性ID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ColorAndSizeAttributeVM> GetColorOrSizeByAppId(System.Guid appId, System.Guid commodityId, System.Guid attributeId)
        {
            base.Do();
            return this.Command.GetColorOrSizeByAppId(appId, commodityId, attributeId);
        }
        /// <summary>
        /// 获取商家颜色/尺寸
        /// </summary>
        /// <param name="appId">appid</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.SecondAttributeDTO> GetSecondAttribute(System.Guid appId)
        {
            base.Do();
            return this.Command.GetSecondAttribute(appId);
        }
    }
}