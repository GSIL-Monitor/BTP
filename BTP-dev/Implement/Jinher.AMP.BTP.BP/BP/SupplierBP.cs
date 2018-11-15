
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/1/4 17:29:31
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
    public partial class SupplierBP : BaseBP, ISupplier
    {

        /// <summary>
        /// 获取供应商数据
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.SupplierListDTO>> GetSuppliers(Jinher.AMP.BTP.Deploy.CustomDTO.SupplierSearchDTO searchDto)
        {
            base.Do();
            return this.GetSuppliersExt(searchDto);
        }
        /// <summary>
        /// 添加供应商
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddSupplier(Jinher.AMP.BTP.Deploy.CustomDTO.SupplierUpdateDTO dto)
        {
            base.Do();
            return this.AddSupplierExt(dto);
        }
        /// <summary>
        /// 修改供应商
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateSupplier(Jinher.AMP.BTP.Deploy.CustomDTO.SupplierUpdateDTO dto)
        {
            base.Do();
            return this.UpdateSupplierExt(dto);
        }
        /// <summary>
        /// 删除供应商
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteSupplier(System.Guid id)
        {
            base.Do();
            return this.DeleteSupplierExt(id);
        }
        /// <summary>
        /// 检查供应商编码
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CheckSupplerCode(string code)
        {
            base.Do();
            return this.CheckSupplerCodeExt(code);
        }
        /// <summary>
        /// 获取商城下所有的app信息
        /// </summary>
        /// <param name="esAppId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.SupplierListDTO> GetSupplierApps(System.Guid esAppId)
        {
            base.Do();
            return this.GetSupplierAppsExt(esAppId);
        }
    }
}