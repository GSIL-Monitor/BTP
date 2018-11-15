
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2018/1/4 17:29:30
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.IBP.IService;

namespace Jinher.AMP.BTP.IBP.Facade
{
    public class SupplierFacade : BaseFacade<ISupplier>
    {

        /// <summary>
        /// 获取供应商数据
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.SupplierListDTO>> GetSuppliers(Jinher.AMP.BTP.Deploy.CustomDTO.SupplierSearchDTO searchDto)
        {
            base.Do();
            return this.Command.GetSuppliers(searchDto);
        }
        /// <summary>
        /// 添加供应商
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddSupplier(Jinher.AMP.BTP.Deploy.CustomDTO.SupplierUpdateDTO dto)
        {
            base.Do();
            return this.Command.AddSupplier(dto);
        }
        /// <summary>
        /// 修改供应商
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateSupplier(Jinher.AMP.BTP.Deploy.CustomDTO.SupplierUpdateDTO dto)
        {
            base.Do();
            return this.Command.UpdateSupplier(dto);
        }
        /// <summary>
        /// 删除供应商
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteSupplier(System.Guid id)
        {
            base.Do();
            return this.Command.DeleteSupplier(id);
        }
        /// <summary>
        /// 检查供应商编码
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CheckSupplerCode(string code)
        {
            base.Do();
            return this.Command.CheckSupplerCode(code);
        }
        /// <summary>
        /// 获取商城下所有的app信息
        /// </summary>
        /// <param name="esAppId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.SupplierListDTO> GetSupplierApps(System.Guid esAppId)
        {
            base.Do();
            return this.Command.GetSupplierApps(esAppId);
        }
    }
}