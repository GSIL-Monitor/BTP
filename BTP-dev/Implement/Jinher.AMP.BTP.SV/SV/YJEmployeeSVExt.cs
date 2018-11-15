
/***************
功能描述: BTPSV
作    者: 
创建时间: 2018/4/27 10:26:05
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.TPS;
using System.Data;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee;

namespace Jinher.AMP.BTP.SV
{
    /// <summary>
    /// 
    /// </summary>
    public partial class YJEmployeeSV : BaseSv, IYJEmployee
    {
        /// <summary>
        /// 定时更新无效用户信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdataYJEmployeeInfoExt()
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                //取出表中多有的无效员工
                var InvalidYJEmployeeList = YJEmployee.ObjectSet().Where(p => p.UserId == Guid.Empty && p.IsDel == 0).ToList();
                foreach (var item in InvalidYJEmployeeList)
                {
                    Guid UserId = CBCSV.GetUserIdByAccount(item.UserAccount);
                    if (UserId != Guid.Empty)
                    {
                        item.UserId = UserId;
                        item.ModifiedOn = DateTime.Now;
                        item.EntityState = EntityState.Modified;
                        contextSession.SaveObject(item);
                    }

                }
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJEmployeeBP.UpdataYJEmployeeInfoExt 异常", ex);
                return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = false, Message = "更新失败" };
            }
            return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = true, Message = "更新成功" };
        }
        /// <summary>
        /// 获取员工code
        ///  </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.YJEmployeeCodeDTO> GetUserCodeByAcccountExt(System.Guid AppId, System.Guid UserId, string UserAccount)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.YJEmployeeCodeDTO> result = new Deploy.CustomDTO.ResultDTO<YJEmployeeCodeDTO>() { isSuccess = false, ResultCode = 1 };
            try
            {
                if (AppId == Guid.Empty || string.IsNullOrEmpty(UserAccount) || UserId == Guid.Empty)
                {
                    return result;
                }

                var query = (from yj in YJEmployee.ObjectSet()
                             where yj.AppId == AppId && yj.UserId == UserId && yj.UserAccount == UserAccount
                             select new YJEmployeeCodeDTO
                             {
                                 UserCode = yj.UserAccount
                             }).FirstOrDefault();
                LogHelper.Info("GetUserCodeAcccount" + JsonHelper.JsonSerializer(query));
                if (query != null)
                {
                    YJEmployeeCodeDTO YJEmployeeCode = new YJEmployeeCodeDTO();
                    YJEmployeeCode.UserCode = query.UserCode;
                    result.Data = YJEmployeeCode;
                    result.isSuccess = true;
                    result.ResultCode = 0;
                    result.Message = "获取成功";
                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJEmployeeBP.GetUserCodeByAcccount 异常", ex);
                return result;
            }
        }
    }
}
