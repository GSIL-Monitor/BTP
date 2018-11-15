
/***************
功能描述: BTP-OPTSV
作    者: 
创建时间: 2015/8/25 16:59:46
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
using System.ServiceModel.Activation;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class DeliveryAddressSV : BaseSv, IDeliveryAddress
    {

        /// <summary>
        /// 添加收货地址
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.DeliveryAddressSV.svc/GetAllCommodityOrder
        /// </para>
        /// </summary>
        /// <param name="addressDTO">地址实体</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveDeliveryAddress(Jinher.AMP.BTP.Deploy.CustomDTO.AddressSDTO addressDTO)
        {
            base.Do(false);
            return this.SaveDeliveryAddressExt(addressDTO);

        }
        /// <summary>
        /// 获取收货地址列表
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.DeliveryAddressSV.svc/GetDeliveryAddressList
        /// </para>
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.AddressSDTO> GetDeliveryAddressList(System.Guid userId, System.Guid appId, int IsDefault)
        {
            base.Do(false);
            return this.GetDeliveryAddressListExt(userId, appId, IsDefault);

        }
        /// <summary>
        /// 获取收货地址列表
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.DeliveryAddressSV.svc/GetDeliveryAddress
        /// </para>
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.AddressSDTO> GetDeliveryAddress(System.Guid userId, System.Guid appId)
        {
            base.Do(false);
            return this.GetDeliveryAddressExt(userId, appId);

        }
        /// <summary>
        /// 删除收货地址
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.DeliveryAddressSV.svc/DeleteDeliveryAddress
        /// </para>
        /// </summary>
        /// <param name="addressId">地址ID</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteDeliveryAddress(System.Guid addressId, System.Guid appId)
        {
            base.Do(false);
            return this.DeleteDeliveryAddressExt(addressId, appId);

        }
        /// <summary>
        /// 编辑收货地址 设置默认地址
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.DeliveryAddressSV.svc/UpdateDeliveryAddressIsDefault
        /// </para>
        /// </summary>
        /// <param name="addressId">地址ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateDeliveryAddressIsDefault(System.Guid addressId)
        {
            base.Do(false);
            return this.UpdateDeliveryAddressIsDefaultExt(addressId);

        }
        /// <summary>
        /// 编辑收货地址
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.DeliveryAddressSV.svc/UpdateDeliveryAddress
        /// </para>
        /// </summary>
        /// <param name="addressDTO">地址实体</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateDeliveryAddress(Jinher.AMP.BTP.Deploy.CustomDTO.AddressSDTO addressDTO, System.Guid appId)
        {
            base.Do(false);
            return this.UpdateDeliveryAddressExt(addressDTO, appId);

        }
        /// <summary>
        /// 收货地址详情
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.DeliveryAddressSV.svc/GetDeliveryAddressByAddressId
        /// </para>
        /// </summary>
        /// <param name="addressId">地址ID</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.AddressSDTO GetDeliveryAddressByAddressId(System.Guid addressId, System.Guid appId)
        {
            base.Do(false);
            return this.GetDeliveryAddressByAddressIdExt(addressId, appId);

        }
        /// <summary>
        /// 保存（添加或修改）收货地址
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.DeliveryAddressSV.svc/SaveDeliveryAddressNew
        /// </para>
        /// </summary>
        /// <param name="addressDTO">地址实体</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveDeliveryAddressNew(Jinher.AMP.BTP.Deploy.CustomDTO.AddressSDTO addressDTO)
        {
            base.Do(false);
            return this.SaveDeliveryAddressNewExt(addressDTO);

        }
    }
}
