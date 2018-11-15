
/***************
功能描述: BTPSV
作    者: 
创建时间: 2015/8/27 15:47:09
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.AMP.BTP.TPS;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using System.Data;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    public partial class SpreadInfoSV : BaseSv, ISpreadInfo
    {


        /// <summary>
        /// 保存推广主信息
        /// </summary>
        /// <param name="spreadInfo">推广主信息</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveToSpreadInfoExt(Jinher.AMP.BTP.Deploy.SpreadInfoDTO spreadInfo)
        {
            ResultDTO result = new ResultDTO { Message = "Success" };
            if (spreadInfo == null || spreadInfo.SpreadId == Guid.Empty || spreadInfo.SpreadCode == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "Spreadid为空或推广码为空" };
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                SpreadSV.Instance.BuildSaveSpreadInfo(spreadInfo, true);
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SpreadInfoSV.SaveToSpreadInfoExt异常：spreaderId={0},spreaderCode={1}, Exception={2}", spreadInfo.SpreadId, spreadInfo.SpreadCode, ex));
                return result;
            }
            return result;
        }
        /// <summary>
        /// 查询推广主信息
        /// </summary>
        /// <param name="spreadInfoSearchDTO"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.SpreadInfoResultDTO GetSpreadInfoExt(Jinher.AMP.BTP.Deploy.CustomDTO.SpreadInfoSearchDTO spreadInfoSearchDTO)
        {
            if (spreadInfoSearchDTO == null || spreadInfoSearchDTO.SpreadCode == Guid.Empty)
            {
                return null;
            }
            var spreadInfo = SpreadInfo.ObjectSet().Where(t => t.SpreadCode == spreadInfoSearchDTO.SpreadCode && t.IsDel == 0).FirstOrDefault();
            if (spreadInfo == null)
            {
                return null;
            }
            Jinher.AMP.BTP.Deploy.CustomDTO.SpreadInfoResultDTO result = new SpreadInfoResultDTO();

            result.SpreadId = spreadInfo.SpreadId;
            result.SpreadUrl = spreadInfo.SpreadUrl;
            result.SpreadCode = spreadInfo.SpreadCode;
            result.SpreadDesc = spreadInfo.SpreadDesc;
            result.SpreadType = spreadInfo.SpreadType;
            result.IsDel = spreadInfo.IsDel;
            result.AppId = spreadInfo.AppId;

            return result;
        }
    }
}