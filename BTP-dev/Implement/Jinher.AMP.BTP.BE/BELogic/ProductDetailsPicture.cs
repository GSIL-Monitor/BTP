
 
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
namespace Jinher.AMP.BTP.BE
{
	public  partial class ProductDetailsPicture
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
        public void Add(ProductDetailsPictureDTO productDetailsPictureDTO)
        {
            ProductDetailsPicture productDetailsPicture = new ProductDetailsPicture().FromEntityData(productDetailsPictureDTO);
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            productDetailsPicture.EntityState = System.Data.EntityState.Added;            
            contextSession.SaveObject(productDetailsPicture);
            contextSession.SaveChanges();
        }
        /// <summary>
        /// 修改操作
        /// </summary>
        public void Updates(ProductDetailsPictureDTO productDetailsPictureDTO)
        {
            ProductDetailsPicture productDetailsPicture = new ProductDetailsPicture().FromEntityData(productDetailsPictureDTO);
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            productDetailsPicture.EntityState = System.Data.EntityState.Modified;            
            contextSession.SaveObject(productDetailsPicture);
            contextSession.SaveChanges();
        }

        /// <summary>
        /// 删除操作
        /// </summary>
        public void Del(ProductDetailsPicture productDetailsPicture)
        {
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            productDetailsPicture.EntityState = System.Data.EntityState.Deleted;
            contextSession.Delete(productDetailsPicture);
            contextSession.SaveChanges();
        }

        /// <summary>
        /// 查询商品展示图片
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        public List<ProductDetailsPictureDTO> select(Guid commodityId) 
        {
            List<ProductDetailsPictureDTO> productDetailPicList = new List<ProductDetailsPictureDTO>();
            try
            {
                var productDetailsPictures = ProductDetailsPicture.ObjectSet().Where(n => n.CommodityId == commodityId).ToList();
                productDetailPicList = new ProductDetailsPicture().ToEntityDataList(productDetailsPictures);
            }
            catch (Exception ex)
            {
                LogHelper.Error("查询商品详情图片异常." + ex.Message);
            }
            return productDetailPicList;
        }
	}
}



