
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Jinher.JAP.Metadata;
using Jinher.JAP.Metadata.Description;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.BF.BE.Base;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.Common.Exception;
using Jinher.JAP.Common.Exception.ComExpDefine;
using Jinher.JAP.Common;
using Jinher.JAP.PL;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Deploy.CustomDTO;
namespace Jinher.AMP.BTP.BE
{
	public  partial class ShoppingCartItems
	{
		#region 基类抽象方法重载
			         
		public override void BusinessRuleValidate()				
		{
					}
				#endregion 
		#region 基类虚方法重写
		public override void SetDefaultValue()
		{
		    base.SetDefaultValue();
		}     
		#endregion 		

        /// <summary>
        /// 添加操作
        /// </summary>
        /// <param name="shoppingCartItemsDTO">购物车实体</param>
        public void Add(ShoppingCartItemsDTO shoppingCartItemsDTO)
        {
            ShoppingCartItems shoppingCartItems = new ShoppingCartItems().FromEntityData(shoppingCartItemsDTO);
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            shoppingCartItems.EntityState = System.Data.EntityState.Added;            
            contextSession.SaveObject(shoppingCartItems);
            contextSession.SaveChanges();
        }
        /// <summary>
        /// 修改操作
        /// </summary>
        /// <param name="shoppingCartItemsDTO">购物车实体</param>
        public void Updates(ShoppingCartItemsDTO shoppingCartItemsDTO)
        {
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            shoppingCartItemsDTO.EntityState = System.Data.EntityState.Modified;
            ShoppingCartItems commodity = new ShoppingCartItems().FromEntityData(shoppingCartItemsDTO);
            contextSession.SaveObject(commodity);
            contextSession.SaveChanges();
        }

        /// <summary>
        /// 删除操作
        /// </summary>
        /// <param name="shoppingCartItems">购物车BE</param>
        public void Del(ShoppingCartItems shoppingCartItems) 
        {
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            shoppingCartItems.EntityState = System.Data.EntityState.Deleted;
            contextSession.Delete(shoppingCartItems);
            contextSession.SaveChange();
        }
	}
}



