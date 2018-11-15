
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2014/4/16 17:22:17
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
    public class SecondAttributeFacade : BaseFacade<ISecondAttribute>
    {

        /// <summary>
        /// 添加尺寸/颜色
        /// </summary>
        /// <param name="attributeId">属性ID</param>
        /// <param name="name">二级属性名</param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddSecondAttribute(System.Guid attributeId, string name, System.Guid appid)
        {
            base.Do();
            return this.Command.AddSecondAttribute(attributeId, name, appid);
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
            return this.Command.GetAttributeBySellerID(sellerID, attributeid);
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
            return this.Command.DelSecondAttribute(secondAttributeId, appid);
        }
        /// <summary>
        /// 是否已有属性
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsExists(string name, Guid appid,Guid attId)
        {
            base.Do();
            return this.Command.IsExists(name, appid, attId);
        }

        /// <summary>
        /// 查询卖家所有已存属性
        /// </summary>
        /// <param name="sellerID">卖家ID</param>
        /// <param name="attributeid">属性ID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ColorAndSizeAttributeVM> GetAttributeByAppID(System.Guid appID)
        {
            base.Do();
            return this.Command.GetAttributeByAppID(appID);
        }

        /// <summary>
        /// 商品属性添加
        /// </summary>
        /// <param name="attributeId"></param>
        /// <param name="name"></param>
        /// <param name="appid"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddAttribute(System.Guid attributeId, string name, Guid appid)
        {
            base.Do();
            return this.Command.AddAttribute(attributeId, name, appid);
        }

        /// <summary>
        /// 商品属性编辑
        /// </summary>
        /// <param name="attributeId"></param>
        /// <param name="name"></param>
        /// <param name="appid"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateAttribute(System.Guid attributeId, string name, Guid appid)
        {
            base.Do();
            return this.Command.UpdateAttribute(attributeId, name, appid);
        }
    }
}