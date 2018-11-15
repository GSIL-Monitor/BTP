
/***************
功能描述: BTPBP
作    者: 
创建时间: 2016/5/29 11:37:09
***************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.AMP.BTP.TPS;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.Common.Loging;
using NPOI.SS.Formula.Functions;
using AppExtensionDTO = Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.Common;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class SpecificationsBP : BaseBP, ISpecifications
    {
        


        /// <summary>
        /// 查询商品规格
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<Jinher.AMP.BTP.Deploy.SpecificationsDTO> GetSpecificationsListExt(Jinher.AMP.BTP.Deploy.SpecificationsDTO search)
        {
            List<Jinher.AMP.BTP.Deploy.SpecificationsDTO> objlist = new List<Jinher.AMP.BTP.Deploy.SpecificationsDTO>();
            var Specificationslist = Specifications.ObjectSet().Where(p => p.IsDel == false).AsQueryable();
            if (search.AppId != Guid.Empty)
            {
                Specificationslist = Specificationslist.Where(p => p.AppId == search.AppId);
            }
            if (search.Attribute!=0)
            {
                Specificationslist = Specificationslist.Where(p => p.Attribute == search.Attribute);
            }
            if (Specificationslist.Count() > 0)
            {
                foreach (var item in Specificationslist.OrderByDescending(p => p.SubTime).ToList())
                {
                    Jinher.AMP.BTP.Deploy.SpecificationsDTO entity = new Jinher.AMP.BTP.Deploy.SpecificationsDTO();
                    entity = CommonUtil.ReadObjectExchange(entity, item);
                    objlist.Add(entity);
                }
            }
            return objlist;
        }


        /// <summary>
        /// 保存商品规格
        /// </summary>
        /// <param name="VatInvoiceProof"></param>
        /// <returns></returns>
        public ResultDTO SaveSpecificationsExt(Jinher.AMP.BTP.Deploy.SpecificationsDTO model)
        {
            ResultDTO dto = null;
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                Specifications entity = new Specifications();
                entity = CommonUtil.ReadObjectExchange(entity, model);
                entity.EntityState = EntityState.Added;
                contextSession.SaveObject(entity);
                contextSession.SaveChanges();
                dto = new ResultDTO { ResultCode = 0, Message = "保存成功", isSuccess = true };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("保存异常。Specifications：{0}", ex.Message));
                dto = new ResultDTO { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return dto;
        }


        /// <summary>
        /// 根据id删除商品规格信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO DelExt(Guid id)
        {
            ResultDTO dto = null;
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var specifications = Specifications.ObjectSet().FirstOrDefault(p=>p.Id==id);
                specifications.EntityState = EntityState.Deleted;
                contextSession.Delete(specifications);
                contextSession.SaveChanges();
                dto = new ResultDTO { ResultCode = 0, Message = "删除成功", isSuccess = true };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("删除异常。Specifications：{0}", ex.Message));
                dto = new ResultDTO { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return dto;
        }
    }
}