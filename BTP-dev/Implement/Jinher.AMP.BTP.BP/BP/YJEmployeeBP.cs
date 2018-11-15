
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/9/12 18:50:36
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
using System.ServiceModel.Activation;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class YJEmployeeBP : BaseBP, IYJEmployee
    {

        /// <summary>
        /// 查询员工信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.YJEmployeeDTO>> GetYJEmployeeList(Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.YJEmployeeSearchDTO input)
        {
            base.Do(false);
            return this.GetYJEmployeeListExt(input);
        }
        /// <summary>
        /// 根据条件查询员工信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.YJEmployeeDTO>> GetYJEmployeeListBySearch(Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.YJEmployeeSearchDTO input)
        {
            base.Do(false);
            return this.GetYJEmployeeListExtBySearch(input);
        }
        /// <summary>
        /// 新建员工
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddYJEmployee(Jinher.AMP.BTP.Deploy.YJEmployeeDTO input)
        {
            base.Do(false);
            return this.AddYJEmployeeExt(input);
        }
        /// <summary>
        /// 编辑员工
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateYJEmployee(Jinher.AMP.BTP.Deploy.YJEmployeeDTO input)
        {
            base.Do(false);
            return this.UpdateYJEmployeeExt(input);
        }
        /// <summary>
        /// 获取员工详情
        /// </summary>
        public Jinher.AMP.BTP.Deploy.YJEmployeeDTO GetYJEmployeeInfo(System.Guid Id)
        {
            base.Do(false);
            return this.GetYJEmployeeInfoExt(Id);
        }
        /// <summary>
        /// 删除员工
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelYJEmployee(System.Guid Id)
        {
            base.Do(false);
            return this.DelYJEmployeeExt(Id);
        }
        /// <summary>
        /// 批量删除员工
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelYJEmployeeAll(System.Collections.Generic.List<System.Guid> Ids)
        {
            base.Do(false);
            return this.DelYJEmployeeAllExt(Ids);
        }
        /// <summary>
        /// 导出员工信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.YJEmployeeDTO>> ExportYJEmployeeList(Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.YJEmployeeSearchDTO input)
        {
            base.Do(false);
            return this.ExportYJEmployeeListExt(input);
        }
        /// <summary>
        /// 导出当次无效账户信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.YJEmployeeDTO>> ExportInvalidData(System.Collections.Generic.List<string> UserAccounts, System.Guid AppId)
        {
            base.Do(false);
            return this.ExportInvalidDataExt(UserAccounts, AppId);
        }
        /// <summary>
        /// 导出全部无效账户信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.YJEmployeeDTO>> ExportInvalidDataByAppid(System.Guid AppId)
        {
            base.Do(false);
            return this.ExportInvalidDataByAppidExt(AppId);
        }
        /// <summary>
        /// 导入员工信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.YJEmployeeSearchDTO> ImportYJEmployeeList(System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.YJEmployeeDTO> YJEmpList, System.Guid AppId)
        {
            base.Do(false);
            return this.ImportYJEmployeeListExt(YJEmpList, AppId);
        }
        /// <summary>
        /// 定时更新无效用户信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdataYJEmployeeInfo()
        {
            base.Do(false);
            return this.UpdataYJEmployeeInfoExt();
        }
        /// <summary>
        /// 获取易捷员工所属区域信息
        /// </summary>
        /// <param name="AppId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.YJEmployeeDTO> GetAreaInfo(System.Guid AppId)
        {
            base.Do(false);
            return this.GetAreaInfoExt(AppId);
        }
        /// <summary>
        /// 根据站编码修改站名称
        ///  </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdataStationNameByCode()
        {
            base.Do(false);
            return this.UpdataStationNameByCodeExt();
        }
        /// <summary>
        /// 根据员工账号更新员工编码
        ///  </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdataUserCodeByUserAccount()
        {
            base.Do(false);
            return this.UpdataUserCodeByUserAccountExt();
        }
    }
}