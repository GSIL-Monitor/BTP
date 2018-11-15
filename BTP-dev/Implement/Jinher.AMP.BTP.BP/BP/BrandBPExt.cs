
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/6/12 11:17:54
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
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class BrandBP : BaseBP, IBrand
    {

        /// <summary>
        /// 添加品牌
        /// </summary>
        /// <param name="brandWallDto">品牌实体</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddBrandExt(Jinher.AMP.BTP.Deploy.BrandwallDTO brandWallDto)
        {
            ResultDTO dto = null;
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var brand = Brandwall.CreateBrandwall();
                brand.AppId = brandWallDto.AppId;
                brand.Brandname = brandWallDto.Brandname;
                brand.BrandLogo = brandWallDto.BrandLogo;
                brand.Brandstatu = brandWallDto.Brandstatu;
                brand.SubId = brandWallDto.SubId;
                brand.SubTime = DateTime.Now;
                brand.ModifiedOn = brand.SubTime;
                brand.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(brand);
                contextSession.SaveChanges();
                dto = new ResultDTO { ResultCode = 0, Message = "保存成功", isSuccess = true };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("品牌信息保存异常。AddBrand：{0}", ex.Message));
                dto = new ResultDTO { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return dto;
        }
        /// <summary>
        /// 修改品牌
        /// </summary>
        /// <param name="brandWallDto">品牌实体</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateBrandExt(Jinher.AMP.BTP.Deploy.BrandwallDTO brandWallDto)
        {
            ResultDTO dto = null;
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var brand = Brandwall.ObjectSet().FirstOrDefault(p => p.Id == brandWallDto.Id && p.AppId == brandWallDto.AppId);
                if (brand != null)
                {
                    brand.Brandname = brandWallDto.Brandname;
                    brand.BrandLogo = brandWallDto.BrandLogo;
                    brand.Brandstatu = brandWallDto.Brandstatu;
                    brand.SubId = brandWallDto.SubId;
                    brand.ModifiedOn = DateTime.Now;
                    contextSession.SaveChanges();
                    dto = new ResultDTO() { ResultCode = 0, Message = "修改成功", isSuccess = true };
                }
                else
                {
                    dto = new ResultDTO() { ResultCode = 1, Message = "品牌信息不存在", isSuccess = false };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("品牌信息修改异常。UpdateBrand：{0}", ex.Message));
                dto = new ResultDTO() { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return dto;
        }
        /// <summary>
        /// 查询品牌
        /// </summary>
        /// <param name="brandName">品牌名称</param>
        /// <param name="status">品牌状态</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<List<Jinher.AMP.BTP.Deploy.BrandwallDTO>> GetBrandListExt(string brandName, int status, Guid appId)
        {
            ResultDTO<List<BrandwallDTO>> dto = null;
            try
            {
                var list = Brandwall.ObjectSet().AsQueryable();
                list = list.Where(b => b.AppId == appId);
                if (String.IsNullOrEmpty(brandName))
                {
                    list = list.Where(b => b.Brandname.Contains(brandName));
                }
                if (status > 0)
                {
                    list = list.Where(b => b.Brandstatu.Equals(status));
                }
                var listDto = (from n in list
                               select new BrandwallDTO
                               {
                                   Id = n.Id,
                                   Brandname = n.Brandname,
                                   BrandLogo = n.BrandLogo,
                                   Brandstatu = n.Brandstatu
                               }).OrderByDescending(p => p.ModifiedOn);
                dto = new ResultDTO<List<BrandwallDTO>>()
               {
                   Data = listDto.ToList(),
                   isSuccess = true,
                   Message = "Sucess",
                   ResultCode = 0
               };

                return dto;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("查询品牌异常。GetBrandList：{0}", ex.Message));
                dto = new ResultDTO<List<BrandwallDTO>>()
               {
                   Data = null,
                   isSuccess = false,
                   Message = "fail",
                   ResultCode = 1
               };
                return dto;
            }
        }
        /// <summary>
        /// 分页查询品牌
        /// </summary>
        /// <param name="brandName">品牌名称</param>
        /// <param name="status">品牌状态</param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<List<Jinher.AMP.BTP.Deploy.BrandwallDTO>> GetBrandPageListExt(string brandName, int status, int pageSize, int pageIndex, Guid appId)
        {
            if (pageIndex < 1 || pageSize < 1)
                return null;
            ResultDTO<List<BrandwallDTO>> dto = null;
            try
            {
                var list = Brandwall.ObjectSet().AsQueryable();
                list = list.Where(b => b.AppId == appId);
                if (!string.IsNullOrEmpty(brandName))
                {
                    list = list.Where(b => b.Brandname.Contains(brandName));
                }
                if (status > 0)
                {
                    list = list.Where(b => b.Brandstatu.Equals(status));
                }
                var listDto = (from n in list
                               select new BrandwallDTO
                               {
                                   Id = n.Id,
                                   Brandname = n.Brandname,
                                   BrandLogo = n.BrandLogo,
                                   Brandstatu = n.Brandstatu,
                                   ModifiedOn = n.ModifiedOn
                               }).OrderByDescending(p => p.ModifiedOn).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(); ;
                dto = new ResultDTO<List<BrandwallDTO>>()
                {
                    Data = listDto.ToList(),
                    isSuccess = true,
                    Message = "Sucess",
                    ResultCode = list.Count()
                };

                return dto;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("分页查询品牌异常。GetBrandPageList：{0}", ex.Message));
                dto = new ResultDTO<List<BrandwallDTO>>()
                {
                    Data = null,
                    isSuccess = false,
                    Message = "fail",
                    ResultCode = 1
                };
                return dto;
            }
        }
        /// <summary>
        /// 是否存在同名品牌
        /// </summary>
        /// <param name="brandName">品牌名称</param>
        /// <returns></returns>
        public bool CheckBrandExt(string brandName, out int rowCount, Guid appId)
        {
            bool flag = false;
            rowCount = 0;
            try
            {
                var brand = Brandwall.ObjectSet().Where(p => p.Brandname.Equals(brandName) && p.AppId == appId);
                if (brand != null)
                {
                    flag = true;
                    rowCount = brand.Count();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("检查是否存在同名品牌异常。CheckBrand：{0}", ex.Message));
                flag = false;
            }
            return flag;
        }
        /// <summary>
        /// 更新品牌状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateBrandStatusExt(Guid id, int status, Guid appId)
        {
            ResultDTO dto = null;
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var brand = Brandwall.ObjectSet().FirstOrDefault(p => p.Id == id && p.AppId == appId);
                if (brand != null)
                {
                    //status 1启用 2停用
                    if (status == 1)
                    {
                        brand.Brandstatu = 2;
                    }
                    else
                    {
                        brand.Brandstatu = 1;
                    }
                    brand.ModifiedOn = DateTime.Now;
                    contextSession.SaveChanges();
                    dto = new ResultDTO() { ResultCode = 0, Message = "修改成功", isSuccess = true };
                }
                else
                {
                    dto = new ResultDTO() { ResultCode = 1, Message = "品牌信息不存在", isSuccess = false };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("品牌信息状态修改异常。UpdateBrandStatus：{0}", ex.Message));
                dto = new ResultDTO() { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return dto;
        }

        /// <summary>
        /// 获取品牌详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.BrandwallDTO GetBrandExt(Guid id, Guid appId)
        {
            try
            {
                BrandwallDTO brand = null;
                Brandwall brandDto = Brandwall.ObjectSet().Where(p => p.Id == id && p.AppId == appId).FirstOrDefault();
                if (brandDto == null)
                {
                    return brand;
                }
                brand = new BrandwallDTO();
                brand.Id = brandDto.Id;
                brand.Brandname = brandDto.Brandname;
                brand.BrandLogo = brandDto.BrandLogo;
                brand.Brandstatu = brandDto.Brandstatu;
                brand.SubId = brandDto.SubId;
                brand.AppId = brandDto.AppId;
                brand.ModifiedOn = brandDto.ModifiedOn;
                return brand;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取品牌详情异常。brandId：{0}", id), ex);
                return null;
            }
        }
    }
}