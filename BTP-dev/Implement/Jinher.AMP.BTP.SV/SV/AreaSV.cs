
/***************
功能描述: BTP-setSV
作    者: 
创建时间: 2015/11/21 15:50:21
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;
using Jinher.AMP.BTP.Deploy.CustomDTO.ThirdECommerce;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class AreaSV : BaseSv, IArea
    {

        /// <summary>
        /// 获取一级地址
        /// </summary>
        /// <returns></returns>
        public ThirdResponse<Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDTO> GetProvince()
        {
            base.Do(false);
            return this.GetProvinceExt();
        }


        /// <summary>
        /// 获取二级地址
        /// </summary>
        /// <returns></returns>
        public ThirdResponse<Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDTO> GetCity(string Code)
        {
            base.Do(false);
            return this.GetCityExt(Code);
        }



        /// <summary>
        /// 获取三级地址
        /// </summary>
        /// <returns></returns>
        public ThirdResponse<Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDTO> GetCounty(string Code)
        {
            base.Do(false);
            return this.GetCountyExt(Code);
        }


        /// <summary>
        /// 获取四级地址
        /// </summary>
        /// <returns></returns>
        public ThirdResponse<Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDTO> GetTown(string Code)
        {
            base.Do(false);
            return this.GetTownExt(Code);
        }
      
    }
}