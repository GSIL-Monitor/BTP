
/***************
功能描述: BTPBP
作    者: 
创建时间: 2017-08-31 15:25:02
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
using System.Data;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class ExpressBP : BaseBP, IExpress
    {
        /// <summary>
        /// 所有
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.ExpressCodeDTO>> GetSystemExt()
        {
            ResultDTO<List<ExpressCodeDTO>> result = new ResultDTO<List<ExpressCodeDTO>>() { isSuccess = true };

            try
            {
                result.Data = (from t in ExpressCode.ObjectSet()
                               orderby t.ExpCode
                               select new ExpressCodeDTO()
                               {
                                   ExpCode = t.ExpCode,
                                   ExpCompanyName = t.ExpCompanyName
                               }).ToList();
            }
            catch (Exception ex)
            {
                result.isSuccess = false;
                result.Message = "查询失败";
                LogHelper.Error("查询快递异常", ex);
            }

            return result;
        }

        /// <summary>
        /// 收藏
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.ExpressCodeDTO>> GetAllExt(System.Guid appId)
        {
            ResultDTO<List<ExpressCodeDTO>> result = new ResultDTO<List<ExpressCodeDTO>>() { isSuccess = true };

            try
            {
                result.Data = (from t in ExpressCollection.ObjectSet()
                               where t.AppId == appId
                               join e in ExpressCode.ObjectSet() on t.ExpCode equals e.ExpCode
                               orderby e.ExpCode
                               select new ExpressCodeDTO()
                               {
                                   ExpCode = e.ExpCode,
                                   ExpCompanyName = e.ExpCompanyName
                               }).ToList();
            }
            catch (Exception ex)
            {
                result.isSuccess = false;
                result.Message = "查询失败";
                LogHelper.Error("查询快递异常", ex);
            }

            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.ExpressCodeDTO> SaveExt(Jinher.AMP.BTP.Deploy.ExpressCodeDTO dto)
        {
            ResultDTO<ExpressCodeDTO> result = new ResultDTO<ExpressCodeDTO>() { isSuccess = true };
            if (dto == null || string.IsNullOrEmpty(dto.ExpCompanyName) || string.IsNullOrEmpty(dto.ExpCode))
            {
                result.isSuccess = false;
                result.Message = "参数错误";
                return result;
            }

            try
            {
                dto.ExpCompanyName = dto.ExpCompanyName.Replace(" ", "");

                var model = ExpressCode.ObjectSet().Where(d => (d.ExpCompanyName == dto.ExpCompanyName || d.ExpCode == dto.ExpCode) && d.Id != dto.Id).FirstOrDefault();
                if (model != null)
                {
                    result.isSuccess = false;
                    result.Message = "重复名称或者编码";
                }
                else
                {
                    if (dto.Id != Guid.Empty)
                    {
                        model = ExpressCode.ObjectSet().Where(d => d.Id == dto.Id).FirstOrDefault();
                        if (model != null)
                        {
                            model.ExpCompanyName = dto.ExpCompanyName;
                            model.ExpCode = dto.ExpCode;
                            model.EntityState = EntityState.Modified;
                            ContextFactory.CurrentThreadContext.SaveChanges();
                        }
                        else
                        {
                            result.isSuccess = false;
                            result.Message = "数据不存在";
                        }
                    }
                    else
                    {
                        dto.Id = Guid.NewGuid();
                        model.ExpCompanyName = dto.ExpCompanyName;
                        model.ExpCode = dto.ExpCode;
                        model.EntityState = EntityState.Added;
                        ContextFactory.CurrentThreadContext.SaveObject(model);
                        ContextFactory.CurrentThreadContext.SaveChanges();
                    }
                }

                result.Data = dto;
            }
            catch (Exception ex)
            {
                result.isSuccess = false;
                result.Message = "保存异常";
                LogHelper.Error("保存快递异常", ex);
            }

            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RemoveExt(Jinher.AMP.BTP.Deploy.ExpressCodeDTO dto)
        {
            ResultDTO result = new ResultDTO() { isSuccess = true };
            if (dto == null || dto.Id == Guid.Empty)
            {
                result.isSuccess = false;
                result.Message = "参数不能为空";
                return result;
            }

            try
            {
                var id = dto.Id;
                var model = ExpressCode.ObjectSet().Where(s => s.Id == id).FirstOrDefault();
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
                result.Message = "删除失败";
                LogHelper.Error("删除快递异常", ex);
            }

            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveUsedExt(System.Guid appId, System.Collections.Generic.List<string> expressCodeList)
        {
            ResultDTO result = new ResultDTO() { isSuccess = true };
            if (expressCodeList == null || appId == Guid.Empty)
            {
                result.isSuccess = false;
                result.Message = "参数不能为空";
                return result;
            }

            try
            {
                var oldList = ExpressCollection.ObjectSet().Where(e => e.AppId == appId).ToList();
                foreach (var item in oldList)
                {
                    item.EntityState = EntityState.Deleted;
                    ContextFactory.CurrentThreadContext.SaveObject(item);
                }

                foreach (var code in expressCodeList)
                {
                    var item = new ExpressCollection();
                    item.Id = Guid.NewGuid();
                    item.AppId = appId;
                    item.ExpCode = code;
                    item.EntityState = EntityState.Added;
                    ContextFactory.CurrentThreadContext.SaveObject(item);
                }

                ContextFactory.CurrentThreadContext.SaveChanges();
            }
            catch (Exception ex)
            {
                result.isSuccess = false;
                result.Message = "保存失败";
                LogHelper.Error("保存常用快递异常", ex);
            }

            return result;
        }

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<string>> GetUsedExt(System.Guid appId)
        {
            ResultDTO<List<string>> result = new ResultDTO<List<string>>() { isSuccess = true };
            if (appId == Guid.Empty)
            {
                result.isSuccess = false;
                result.Message = "参数不能为空";
                return result;
            }

            try
            {
                result.Data = ExpressCollection.ObjectSet().Where(e => e.AppId == appId).Select(e => e.ExpCode).ToList();
            }
            catch (Exception ex)
            {
                result.isSuccess = false;
                result.Message = "查询失败";
                LogHelper.Error("查询常用快递异常", ex);
            }

            return result;
        }
    }
}