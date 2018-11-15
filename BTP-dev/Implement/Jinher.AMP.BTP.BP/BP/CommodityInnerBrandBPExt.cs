
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/6/23 16:12:20
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
using Jinher.JAP.Common.Loging;
using System.Data;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class CommodityInnerBrandBP : BaseBP, ICommodityInnerBrand
    {

        /// <summary>
        /// 添加商品品牌
        /// </summary>
        /// <param name="brandWallDto">商品品牌实体</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddComInnerBrandExt(Jinher.AMP.BTP.Deploy.CommodityInnerBrandDTO innerBrandDto)
        {
            ResultDTO dto = null;
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var combrand = CommodityInnerBrand.CreateCommodityInnerBrand();
                combrand.AppId = innerBrandDto.AppId;
                combrand.BrandId = innerBrandDto.BrandId;
                combrand.CommodityId = innerBrandDto.CommodityId;
                combrand.Name = innerBrandDto.Name;
                combrand.AppId = innerBrandDto.AppId;
                combrand.SubId = innerBrandDto.SubId;
                combrand.SubTime = DateTime.Now;
                combrand.ModifiedOn = innerBrandDto.SubTime;
                combrand.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(combrand);
                contextSession.SaveChanges();
                dto = new ResultDTO { ResultCode = 0, Message = "保存成功", isSuccess = true };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("商品品牌信息保存异常。AddComInnerBrandExt：{0}", ex.Message));
                dto = new ResultDTO { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return dto;
        }
        /// <summary>
        /// 根据商品Id查询商品品牌
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CommodityInnerBrandDTO GetComInnerBrandExt(System.Guid commodityId)
        {
            try
            {
                CommodityInnerBrandDTO combrand = null;
                CommodityInnerBrand combrandDto = CommodityInnerBrand.ObjectSet().Where(p => p.CommodityId == commodityId).FirstOrDefault();
                if (combrandDto == null)
                {
                    return combrand;
                }
                combrand = new CommodityInnerBrandDTO();
                combrand.Id = combrandDto.Id;
                combrand.Name = combrandDto.Name;
                combrand.CommodityId = combrandDto.CommodityId;
                combrand.AppId = combrandDto.AppId;
                return combrand;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取商品品牌详情异常。commodityId：{0}", commodityId), ex);
                return null;
            }
        }
        /// <summary>
        /// 删除商品品牌
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelComInnerBrandExt(System.Guid commodityId)
        {
            ResultDTO result = new ResultDTO() { isSuccess = true };
            try
            {

                var model = CommodityInnerBrand.ObjectSet().Where(s => s.CommodityId == commodityId).FirstOrDefault();
                if (model != null)
                {
                    model.EntityState = EntityState.Deleted;
                    ContextFactory.CurrentThreadContext.SaveObject(model);
                    ContextFactory.CurrentThreadContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                result.isSuccess = false;
                result.Message = "删除商品品牌失败";
                LogHelper.Error(string.Format("删除商品品牌失败：DelComInnerBrandExt:{0}", commodityId), ex);
            }

            return result;
        }
    }
}