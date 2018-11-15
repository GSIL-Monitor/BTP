
/***************
功能描述: BTPSV
作    者: 
创建时间: 2015/11/18 19:46:35
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
using Jinher.JAP.Cache;
using Jinher.AMP.BTP.Common;
using Newtonsoft.Json.Linq;
using Jinher.AMP.BTP.Deploy.CustomDTO.ThirdECommerce;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    public partial class AreaSV : BaseSv, IArea
    {

        /// <summary>
        /// 获取一级地址
        /// </summary>
        /// <returns></returns>
        public ThirdResponse<Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDTO> GetProvinceExt()
        {
            ThirdResponse<Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDTO> res = new ThirdResponse<Deploy.CustomDTO.JD.AreaDTO>();
            try
            {
                Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDTO  Areadto = new Deploy.CustomDTO.JD.AreaDTO();
                List<Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDto> objlist = new List<Deploy.CustomDTO.JD.AreaDto>();
                string objstr = JdHelper.GetProvince();
                if (!string.IsNullOrEmpty(objstr))
                {
                    JObject obj = JObject.Parse(objstr);
                    if (obj.Count > 0)
                    {
                        foreach (var item in obj)
                        {
                            Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDto model = new Deploy.CustomDTO.JD.AreaDto();
                            model.Name = item.Key;
                            model.Code = item.Value.ToString();
                            objlist.Add(model);
                        }
                    }
                    Areadto.Count = objlist.Count();
                    Areadto.Data = objlist;

                    res.Code = 200;
                    res.Result = Areadto;
                    res.Msg = "查询成功!";
                }
                else
                {
                    res.Code = 200;
                    res.Result = new Deploy.CustomDTO.JD.AreaDTO();
                    res.Msg = "查询成功!";
                }
            }
            catch (Exception ex)
            {
                res.Code = 200;
                res.Result = new Deploy.CustomDTO.JD.AreaDTO();
                res.Msg = ex.Message;
            }
            return res;
        }


        /// <summary>
        /// 获取二级地址
        /// </summary>
        /// <returns></returns>
        public ThirdResponse<Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDTO> GetCityExt(string Code)
        {
            ThirdResponse<Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDTO> res = new ThirdResponse<Deploy.CustomDTO.JD.AreaDTO>();
            try
            {
                Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDTO Areadto = new Deploy.CustomDTO.JD.AreaDTO();
                List<Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDto> objlist = new List<Deploy.CustomDTO.JD.AreaDto>();
                string objstr = JdHelper.GetCity(Code);
                if (!string.IsNullOrEmpty(objstr))
                {
                    JObject obj = JObject.Parse(objstr);
                    if (obj.Count > 0)
                    {
                        foreach (var item in obj)
                        {
                            Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDto model = new Deploy.CustomDTO.JD.AreaDto();
                            model.Name = item.Key;
                            model.Code = item.Value.ToString();
                            objlist.Add(model);
                        }
                    }
                    Areadto.Count = objlist.Count();
                    Areadto.Data = objlist;

                    res.Code = 200;
                    res.Result = Areadto;
                    res.Msg = "查询成功!";
                }
                else
                {
                    res.Code = 200;
                    res.Result = Areadto;
                    res.Msg = "查询成功!";
                }
            }
            catch (Exception ex)
            {
                res.Code = 200;
                res.Result = new Deploy.CustomDTO.JD.AreaDTO();
                res.Msg = ex.Message;
            }
            return res;
        }



        /// <summary>
        /// 获取三级地址
        /// </summary>
        /// <returns></returns>
        public ThirdResponse<Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDTO> GetCountyExt(string Code)
        {
            ThirdResponse<Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDTO> res = new ThirdResponse<Deploy.CustomDTO.JD.AreaDTO>();
            try
            {
                Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDTO Areadto = new Deploy.CustomDTO.JD.AreaDTO();
                List<Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDto> objlist = new List<Deploy.CustomDTO.JD.AreaDto>();
                string objstr = JdHelper.GetCounty(Code);
                if (!string.IsNullOrEmpty(objstr))
                {
                    JObject obj = JObject.Parse(objstr);
                    if (obj.Count > 0)
                    {
                        foreach (var item in obj)
                        {
                            Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDto model = new Deploy.CustomDTO.JD.AreaDto();
                            model.Name = item.Key;
                            model.Code = item.Value.ToString();
                            objlist.Add(model);
                        }
                    }
                    Areadto.Count = objlist.Count();
                    Areadto.Data = objlist;

                    res.Code = 200;
                    res.Result = Areadto;
                    res.Msg = "查询成功!";
                }
                else
                {
                    res.Code = 200;
                    res.Result = Areadto;
                    res.Msg = "查询成功!";
                }
            }
            catch (Exception ex)
            {
                res.Code = 200;
                res.Result = new Deploy.CustomDTO.JD.AreaDTO();
                res.Msg = ex.Message;
            }
            return res;
        }


        /// <summary>
        /// 获取四级地址
        /// </summary>
        /// <returns></returns>
        public ThirdResponse<Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDTO> GetTownExt(string Code)
        {
            ThirdResponse<Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDTO> res = new ThirdResponse<Deploy.CustomDTO.JD.AreaDTO>();
            try
            {
                Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDTO Areadto = new Deploy.CustomDTO.JD.AreaDTO();
                List<Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDto> objlist = new List<Deploy.CustomDTO.JD.AreaDto>();
                string objstr = JdHelper.GetTown(Code);
                if (!string.IsNullOrEmpty(objstr))
                {
                    JObject obj = JObject.Parse(objstr);
                    if (obj.Count > 0)
                    {
                        foreach (var item in obj)
                        {
                            Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDto model = new Deploy.CustomDTO.JD.AreaDto();
                            model.Name = item.Key;
                            model.Code = item.Value.ToString();
                            objlist.Add(model);
                        }
                    }
                    Areadto.Count = objlist.Count();
                    Areadto.Data = objlist;

                    res.Code = 200;
                    res.Result = Areadto;
                    res.Msg = "查询成功!";
                }
                else
                {
                    res.Code = 200;
                    res.Result = Areadto;
                    res.Msg = "查询成功!";
                }
            }
            catch (Exception ex)
            {
                res.Code = 200;
                res.Result = new Deploy.CustomDTO.JD.AreaDTO();
                res.Msg = ex.Message;
            }
            return res;
        }
    }
}
