
/***************
功能描述: BTPBP
作    者: 
创建时间: 2014/4/16 17:22:18
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
    public partial class SecondAttributeBP : BaseBP, ISecondAttribute
    {

        /// <summary>
        /// 添加尺寸/颜色
        /// </summary>
        /// <param name="attributeId">属性ID</param>
        /// <param name="name">二级属性名</param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddSecondAttribute(System.Guid attributeId, string name, System.Guid appid)
        {
            base.Do();
            return this.AddSecondAttributeExt(attributeId, name, appid);
        }
        /// <summary>
        /// 查询卖家所有已存在尺寸/颜色
        /// </summary>
        /// <param name="sellerID">卖家ID</param>
        /// <param name="attributeid">属性ID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ColorAndSizeAttributeVM> GetAttributeBySellerID(System.Guid sellerID, System.Guid attributeid)
        {
            base.Do();
            return this.GetAttributeBySellerIDExt(sellerID, attributeid);
        }


         /// <summary>
        /// 查询卖家所有已存属性
        /// </summary>
        /// <param name="sellerID">卖家ID</param>
        /// <param name="attributeid">属性ID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ColorAndSizeAttributeVM> GetAttributeByAppID(System.Guid appID)
        {
            base.Do(false);
            return this.GetAttributeByAppIDExt(appID);
        }

         /// <summary>
        /// 商品属性添加
        /// </summary>
        /// <param name="attributeId"></param>
        /// <param name="name"></param>
        /// <param name="appid"></param>
        /// <returns></returns>
        public ResultDTO AddAttribute(System.Guid attributeId, string name, Guid appid)
        {
            base.Do();
            return this.AddAttributeExt( attributeId,  name,  appid);
        }

        /// <summary>
        /// 商品属性编辑
        /// </summary>
        /// <param name="attributeId"></param>
        /// <param name="name"></param>
        /// <param name="appid"></param>
        /// <returns></returns>
        public ResultDTO UpdateAttribute(System.Guid attributeId, string name, Guid appid)
        {
            base.Do();
            return this.UpdateAttributeExt(attributeId, name, appid);
        }

        /// <summary>
        /// 属性删除
        /// </summary>
        /// <param name="secondAttributeId">次级属性ID</param>
        /// <param name="appid"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelSecondAttribute(System.Guid secondAttributeId, System.Guid appid)
        {
            base.Do();
            return this.DelSecondAttributeExt(secondAttributeId, appid);
        }
        /// <summary>
        /// 是否已有属性
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsExists(string name, Guid appid ,Guid attId)
        {
            base.Do();
            return this.IsExistsExt(name, appid, attId);
        }
    }
}