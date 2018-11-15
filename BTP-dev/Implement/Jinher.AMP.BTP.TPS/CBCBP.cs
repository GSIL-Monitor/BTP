using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.CBC.Deploy;
using Jinher.AMP.CBC.IBP.Facade;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.CBC.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.TPS
{

    /*
     * 请注意：！！！！！！！！！！！！！！！
     * 外部引用请将代码写到TPS.XXXFacade中，自定义扩展请写到TPS.XXXSV、TPS.XXXBP中
     */

    public class CBCBP : OutSideServiceBase<CBCBPFacade>
    {

    }

    public class CBCBPFacade : OutSideFacadeBase
    {
        /// <summary>
        /// 获取省列表
        /// </summary>
        /// <param name="countryCode"></param>
        /// <returns></returns>
        public List<Area> GeProvinceByCountryCode(string countryCode = "1001")
        {
            var result = new List<Area>();
            result = ProvinceCityHelper.GetAllProvince();
            //try
            //{
            //    Jinher.AMP.CBC.IBP.Facade.ProvinceFacade facade = new ProvinceFacade();
            //    facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
            //    result = facade.GeProvinceByCountryCode(countryCode);
            //}
            //catch (Exception ex)
            //{
            //    LogHelper.Error("调用CBCBP.GeProvinceByCountryCode异常", ex);
            //}

            return result ?? new List<Area>();
        }
        /// <summary>
        /// 根据省编码获取城市列表
        /// </summary>
        /// <param name="provinceCode"></param>
        /// <returns></returns>
        public List<Area> GetCityByProvinceCode(string provinceCode)
        {
            return ProvinceCityHelper.GetProvinceCities(provinceCode);
            //var result = new List<CityDTO>();
            //if (string.IsNullOrEmpty(provinceCode))
            //    return result;
            //try
            //{
            //    Jinher.AMP.CBC.IBP.Facade.CityFacade facade = new CityFacade();
            //    facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
            //    result = facade.GetCityByProvinceCode(provinceCode);
            //}
            //catch (Exception ex)
            //{
            //    LogHelper.Error("调用CBCBP.GetCityByProvinceCode异常", ex);
            //}
            //return result ?? new List<CityDTO>();
        }
        /// <summary>
        /// 根据城市code获取区县列表
        /// </summary>
        /// <param name="cityCode"></param>
        /// <returns></returns>
        public List<Area> GetCountyByCityCode(string cityCode)
        {
            return ProvinceCityHelper.GetCityDistricts(cityCode);

            //var result = new List<CountyDTO>();
            //if (string.IsNullOrEmpty(cityCode))
            //    return new List<CountyDTO>();
            //try
            //{
            //    Jinher.AMP.CBC.IBP.Facade.CountyFacade facade = new CountyFacade();
            //    facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
            //    result = facade.GetCountyByCityCode(cityCode);
            //}
            //catch (Exception ex)
            //{
            //    LogHelper.Error("调用CBCBP.GetCountyByCityCode异常", ex);
            //}
            //return result ?? new List<CountyDTO>();
        }

        /// <summary>
        /// 获取多个省份的名称
        /// </summary>
        /// <param name="codes">省份代码列表</param>
        /// <returns></returns>
        public IList<string> GetProvincesNameByCode(IList<string> codes)
        {
            var provinces = ProvinceCityHelper.GetAllProvince();

            return provinces.Where(predicate => codes.Contains(predicate.Code)).Select(selector => selector.Name).ToList();
        }

        /// <summary>
        /// 获取多个省份的名称
        /// </summary>
        /// <param name="codes">省份代码（半角逗号分隔的省份代码字符串）</param>
        /// <returns></returns>
        public IList<string> GetProvincesNameByCode(string codes)
        {
            if (string.IsNullOrEmpty(codes))
            {
                throw new ArgumentNullException();
            }

            return GetProvincesNameByCode(codes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList());
        }
    }


}
