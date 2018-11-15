
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/11/6 11:11:14
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
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class InsuranceCompanyBP : BaseBP, IInsuranceCompany
    {

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<List<Jinher.AMP.BTP.Deploy.CustomDTO.InsuranceCompanyDTO>> GetInsuranceCompanyExt()
        {
            ResultDTO<List<InsuranceCompanyDTO>> Result = new ResultDTO<List<InsuranceCompanyDTO>> { isSuccess = true, Message = "获取成功" };
            try
            {
                var Company = InsuranceCompany.ObjectSet().Where(p => p.Status == 1).Select(o => new InsuranceCompanyDTO
                {
                    InsuranceCompanyCode = o.InsuranceCompanyCode,
                    Name = o.Name,
                    PicUrl = o.PicUrl
                }).ToList();
                if (!Company.Any())
                {
                    Result.Message = "未获取到任何数据";
                    return Result;
                }
                else
                {
                    Result.Data = Company;
                }
            }
            catch (Exception ex)
            {
                Result.ResultCode = -1;
                Result.isSuccess = false;
                Result.Message = string.Format("按快递单号获取快递路由信息异常，异常信息：{0}", ex);
                return Result;
            }
            return Result;
        }
    }
}