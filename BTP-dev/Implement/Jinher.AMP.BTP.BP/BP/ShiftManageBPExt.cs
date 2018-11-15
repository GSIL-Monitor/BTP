
/***************
功能描述: BTPBP
作    者: 
创建时间: 2017/1/10 13:42:04
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

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class ShiftManageBP : BaseBP, IShiftManage
    {

        /// <summary>
        /// 交班
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO ShiftExchangeExt(Jinher.AMP.BTP.Deploy.CustomDTO.ShiftLogDTO dto)
        {
            Deploy.CustomDTO.ResultDTO ret = new Deploy.CustomDTO.ResultDTO() { isSuccess = false, ResultCode = 1, Message = "添加失败！" };

            if (dto == null)
            {
                ret.Message = "参数不能为空！";
                return ret;
            }
            BE.ShiftTimeLog shift = new ShiftTimeLog();
            shift.Id = dto.id;
            shift.UserId = dto.userId;
            shift.ShiftTime = dto.shiftTime;
            shift.SubId = dto.subId;
            shift.SubTime = dto.subTime;
            shift.ModifiedOn = dto.modifiedOn;
            shift.StoreId = dto.storeId;
            shift.AppId = dto.appId;
            shift.EntityState = System.Data.EntityState.Added;
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                contextSession.SaveObject(shift);
                int change = contextSession.SaveChanges();
                ret.isSuccess = change > 0;
                if (ret.isSuccess)
                    ret.Message = "添加成功！";

                return ret;
            }
            catch(Exception ex)
            {
                ret.Message = ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 获取交班信息
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.FShiftLogCDTO GetLastShiftInfoExt(System.Guid appId)
        {
            Deploy.CustomDTO.FShiftLogCDTO shiftInfo = (from shift in BE.ShiftTimeLog.ObjectSet()
                                                        where shift.AppId == appId
                                                        orderby shift.SubTime descending
                                                        select new
                                                            Deploy.CustomDTO.FShiftLogCDTO
                                                            {
                                                                userId = shift.UserId,
                                                                shiftTime = shift.ShiftTime,
                                                                storeId = shift.StoreId
                                                            }).FirstOrDefault();

            return shiftInfo == null ? new Deploy.CustomDTO.FShiftLogCDTO() : shiftInfo;
        }
    }
}