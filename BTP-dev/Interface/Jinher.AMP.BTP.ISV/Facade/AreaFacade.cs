
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2018/1/7 10:39:27
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.CustomDTO.ThirdECommerce;

namespace Jinher.AMP.BTP.ISV.Facade
{
    public class AreaFacade : BaseFacade<IArea>
    {

        /// <summary>
        /// 获取一级地址
        /// </summary>
        /// <returns></returns>
        public ThirdResponse<Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDTO> GetProvince()
        {
            base.Do();
            return this.Command.GetProvince();
        }


        /// <summary>
        /// 获取二级地址
        /// </summary>
        /// <returns></returns>
        public ThirdResponse<Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDTO> GetCity(string Code)
        {
            base.Do();
            return this.Command.GetCity(Code);
        }



        /// <summary>
        /// 获取三级地址
        /// </summary>
        /// <returns></returns>
        public ThirdResponse<Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDTO> GetCounty(string Code)
        {
            base.Do();
            return this.Command.GetCounty(Code);
        }


        /// <summary>
        /// 获取四级地址
        /// </summary>
        /// <returns></returns>
        public ThirdResponse<Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDTO> GetTown(string Code)
        {
            base.Do();
            return this.Command.GetTown(Code);
        }
    }
}