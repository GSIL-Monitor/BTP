
/***************
功能描述: BTPBP
作    者:
创建时间: 2018/6/14 15:25:05
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

namespace Jinher.AMP.BTP.BP
{
    /// <summary>
    /// 
    /// </summary>
    public partial class CategoryAdvertiseBP : BaseBP, ICategoryAdvertise
    {
        /// <summary>
        /// 添加品类广告
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CreateCategoryAdvertiseExt(Jinher.AMP.BTP.Deploy.CategoryAdvertiseDTO entity)
        {
            var ifCreate = CategoryAdvertise.ObjectSet().Where(o => o.PushTime <= entity.PushTime && o.PutTime >= entity.PutTime && o.CategoryId == entity.CategoryId).Count();

            if (ifCreate > 0)
            {
                return new ResultDTO { ResultCode = 1, Message = "同一分类下同一时间段只能有一个分类广告", isSuccess = false };
            }

            try
            {
                var model = CategoryAdvertise.CreateCategoryAdvertise();
                model.AdvertiseImg = entity.AdvertiseImg;
                model.AdvertiseMedia = entity.AdvertiseMedia;
                model.AdvertiseName = entity.AdvertiseName;
                model.AdvertiseType = entity.AdvertiseType;
                model.CategoryId = entity.CategoryId;
                model.FreeUrl = entity.FreeUrl;
                model.PushTime = entity.PushTime;
                model.PutTime = entity.PutTime;
                model.spreadEnum = entity.spreadEnum;
                //model.State = entity.State;
                if (entity.PutTime <= DateTime.Now && entity.PushTime >= DateTime.Now)
                {
                    entity.State = 1;
                }
                else if (entity.PushTime < DateTime.Now)
                {
                    entity.State = 2;
                }
                else if (entity.PutTime > DateTime.Now)
                {
                    entity.State = 0;
                }

                model.AdverID = entity.AdverID;
                model.UserService = entity.UserService;

                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                contextSession.SaveObject(model);
                contextSession.SaveChanges();

                return new ResultDTO { ResultCode = 0, Message = "", isSuccess = true };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("添加添加品牌分类关系发生错误。参数：{0}", JsonHelper.JsonSerializer(entity)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error", isSuccess = false };
            }
        }
        /// <summary>
        ///  搜索获取品类广告信息
        /// </summary>
        /// <param name="advertiseName"></param>
        /// <param name="state"></param>
        /// <param name="CategoryId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.IList<Jinher.AMP.BTP.Deploy.CategoryAdvertiseDTO>> CateGoryAdvertiseListExt(string advertiseName, int state, Guid CategoryId, int pageIndex, int pageSize, out int rowCount)
        {
            try
            {
                var list = CategoryAdvertise.ObjectSet().Where(_ => _.CategoryId == CategoryId);
                if (!String.IsNullOrEmpty(advertiseName))
                {
                    list = list.Where(_ => _.AdvertiseName.Contains(advertiseName));
                }

                if (state >= 0)
                {
                    switch (state)
                    {
                        case 0: list = list.Where(o => o.PutTime > DateTime.Now); break;
                        case 1: list = list.Where(o => o.PutTime < DateTime.Now && o.PushTime > DateTime.Now); break;
                        case 2: list = list.Where(o => o.PushTime < DateTime.Now); break;
                    }

                    list = list.Where(_ => _.State == state);
                }

                rowCount = list.Count();

                list = list.OrderBy(o => o.SubTime).Skip((pageIndex - 1) * pageSize).Take(pageSize);

                var listDto = from n in list
                              select new CategoryAdvertiseDTO
                              {
                                  Id = n.Id,
                                  AdvertiseImg = n.AdvertiseImg,
                                  AdvertiseMedia = n.AdvertiseMedia,
                                  AdvertiseName = n.AdvertiseName,
                                  AdvertiseType = n.AdvertiseType,
                                  CategoryId = n.CategoryId,
                                  FreeUrl = n.FreeUrl,
                                  PushTime = n.PushTime,
                                  PutTime = n.PutTime,
                                  spreadEnum = n.spreadEnum,
                                  State = n.State
                              };

                var dto = new ResultDTO<IList<CategoryAdvertiseDTO>>()
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
                var dto = new ResultDTO<IList<CategoryAdvertiseDTO>>()
                {
                    Data = null,
                    isSuccess = false,
                    Message = "fail",
                    ResultCode = 1
                };

                rowCount = 0;
                return dto;
            }

            return null;

            //throw new NotImplementedException();
        }

        /// <summary>
        /// 删除品牌
        /// </summary>
        /// <param name="advertiseId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteCategoryAdvertiseExt(System.Guid advertiseId)
        {
            var entity = CategoryAdvertise.ObjectSet().First(o => o.Id == advertiseId);
            if (entity != null)
            {
                entity.EntityState = System.Data.EntityState.Deleted;
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                contextSession.Delete(entity);
                contextSession.SaveChanges();
                return new ResultDTO()
                {
                    isSuccess = true,
                    Message = "sucess",
                    ResultCode = 0
                };
            }

            return new ResultDTO()
            {
                isSuccess = true,
                Message = "sucess",
                ResultCode = 0
            };
        }

        /// <summary>
        /// 编辑品牌
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO EditCategoryAdvertiseExt(Jinher.AMP.BTP.Deploy.CategoryAdvertiseDTO entity)
        {
            try
            {
                entity.EntityState = System.Data.EntityState.Modified;
                var model = CategoryAdvertise.FromDTO(entity);
                model.EntityState = System.Data.EntityState.Modified;
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                contextSession.Update(model);
                var result =  contextSession.SaveChanges();
                
                return new ResultDTO()
                {
                    isSuccess = true,
                    Message = "sucess",
                    ResultCode = 0
                };
            }
            catch (Exception ex)
            {
                LogHelper.Info(string.Format("更新品类广告 CategoryAdvertiseBP.EditCategoryAdvertise, cancelTheOrderDTO:{0} ,{1}", JsonHelper.JsonSerializer(entity), ex), "BTP_CategoryAdvertise");
                return new ResultDTO()
                {
                    isSuccess = true,
                    Message = ex.Message,
                    ResultCode = 0
                };
            }
        }

        /// <summary>
        /// 获取品牌分类
        /// </summary>
        /// <param name="advertiseId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CategoryAdvertiseDTO> GetCategoryAdvertiseExt(System.Guid advertiseId)
        {
            try
            {
                var entity = CategoryAdvertise.ObjectSet().FirstOrDefault(o => o.Id == advertiseId);
                if (entity != null && entity.Id != Guid.Empty)
                {
                    return new ResultDTO<Jinher.AMP.BTP.Deploy.CategoryAdvertiseDTO>()
                    {
                        isSuccess = true,
                        Message = "Sucess",
                        Data = entity.ToEntityData()
                    };
                }
                else
                {
                    return new ResultDTO<Jinher.AMP.BTP.Deploy.CategoryAdvertiseDTO>()
                    {
                        isSuccess = true,
                        Message = "Sucess",
                        Data = null
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Info(string.Format("获取品类广告 CategoryAdvertiseBP.GetCategoryAdvertise,advertiseId :{0},{1}", JsonHelper.JsonSerializer(advertiseId),ex), "BTP_CategoryAdvertise");
                return new ResultDTO<Jinher.AMP.BTP.Deploy.CategoryAdvertiseDTO>()
                    {
                        isSuccess = false,
                        Message = "fail",
                        Data = null
                    };
            }
        }
    }
}