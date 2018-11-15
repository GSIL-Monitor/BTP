
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2017/6/8 18:52:50
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Agent.Base;
using Jinher.AMP.BTP.Deploy.CustomDTO.JdEclp;

namespace Jinher.AMP.BTP.IBP.Agent
{

    public class CommodityAgent : BaseBpAgent<ICommodity>, ICommodity
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveCommodity(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO commodityAndCategoryDTO)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SaveCommodity(commodityAndCategoryDTO);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateCommodity(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO commodityAndCategoryDTO)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateCommodity(commodityAndCategoryDTO);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityVM> GetAllCommodityBySellerIDBySalesvolume(Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySearchDTO commoditySearch, out int rowCount)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityVM> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAllCommodityBySellerIDBySalesvolume(commoditySearch, out rowCount);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityVM> GetAllNoOnSaleCommodityBySellerIDBySalesvolume(Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySearchDTO commoditySearch, out int rowCount)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityVM> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAllNoOnSaleCommodityBySellerIDBySalesvolume(commoditySearch, out rowCount);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO GetCommodity(System.Guid id, System.Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodity(id, appId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteGetCommodity(System.Guid id)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DeleteGetCommodity(id);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteCommoditys(System.Collections.Generic.List<System.Guid> ids)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DeleteCommoditys(ids);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO OffShelves(System.Collections.Generic.List<System.Guid> ids)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.OffShelves(ids);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO Shelves(System.Collections.Generic.List<System.Guid> ids)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.Shelves(ids);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdatePrice(System.Guid id, decimal price)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdatePrice(id, price);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateStock(System.Guid id, int stock)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateStock(id, stock);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateSalesvolume(System.Guid id, int salesvolume)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateSalesvolume(id, salesvolume);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateName(System.Guid id, string name)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateName(id, name);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CategoryDTO> GetCategoryBycommodityId(System.Guid commodityId)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CategoryDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCategoryBycommodityId(commodityId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateCategoryBycommodityId(Jinher.AMP.BTP.Deploy.CustomDTO.UCategoryVM uCategoryVM)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateCategoryBycommodityId(uCategoryVM);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityDTO> GetCommodityByCategoryId(System.Guid appId, System.Guid categoryId)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityByCategoryId(appId, categoryId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityPromVM> GetCommodityVM(Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySearchDTO commoditySearch, System.Guid promotionId)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityPromVM> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityVM(commoditySearch, promotionId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public bool IsExists(string code, System.Guid appId)
        {
            //定义返回值
            bool result;

            try
            {
                //调用代理方法
                result = base.Channel.IsExists(code, appId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public int GetCommodityNum(System.Guid appId, bool isDel, int state)
        {
            //定义返回值
            int result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityNum(appId, isDel, state);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public System.Collections.Generic.List<string> GetCommodityCodes(System.Guid appid, System.Collections.Generic.List<System.Guid> commodityIds)
        {
            //定义返回值
            System.Collections.Generic.List<string> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityCodes(appid, commodityIds);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public string GetLastCommodityCode(System.Guid appId)
        {
            //定义返回值
            string result;

            try
            {
                //调用代理方法
                result = base.Channel.GetLastCommodityCode(appId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveCommoditySortValue(System.Collections.Generic.List<System.Guid> comIds)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SaveCommoditySortValue(comIds);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetCommodityFirst(System.Guid comId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SetCommodityFirst(comId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityPromVM> RelationCommodityList(System.Guid comId, Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySearchDTO search)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityPromVM> result;

            try
            {
                //调用代理方法
                result = base.Channel.RelationCommodityList(comId, search);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateMarketPrice(System.Guid id, decimal? marketPrice)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateMarketPrice(id, marketPrice);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateCostPrice(System.Guid id, decimal? costPrice)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateCostPrice(id, costPrice);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateCommodityListSaleAreas(System.Collections.Generic.List<System.Guid> ids, string saleAreas)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateCommodityListSaleAreas(ids, saleAreas);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityDividendListDTO> GetCommodityByName(Jinher.AMP.BTP.Deploy.CustomDTO.GetCommodityByNameParam pdto)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityDividendListDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityByName(pdto);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveCommoditySharePercent(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityShareInfoDTO dto)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SaveCommoditySharePercent(dto);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityShareInfoDTO> GetCommoditySharePercentByAppId(System.Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityShareInfoDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommoditySharePercentByAppId(appId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetCommodityDistribution(System.Collections.Generic.List<System.Guid> commodityIdList, bool isDistribute)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SetCommodityDistribution(commodityIdList, isDistribute);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetDistributionAccount(System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityDistributionDTO> commodityDistributionList)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SetDistributionAccount(commodityDistributionList);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetDefaulDistributionAccount(Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO appExtension)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SetDefaulDistributionAccount(appExtension);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO GetDefaulDistributionAccount(System.Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetDefaulDistributionAccount(appId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveUpCommoditySortValue(System.Guid appId, System.Collections.Generic.List<System.Guid> comIds, System.Guid id)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SaveUpCommoditySortValue(appId, comIds, id);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveDownCommoditySortValue(System.Guid appId, System.Collections.Generic.List<System.Guid> comIds, System.Guid id)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SaveDownCommoditySortValue(appId, comIds, id);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveCommodityScorePercent(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityScoreDTO dto)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SaveCommodityScorePercent(dto);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityScoreDTO> GetCommodityScorePercentByAppId(System.Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityScoreDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityScorePercentByAppId(appId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CateringComdtyXDataDTO GetCommodityBoxInfo(System.Guid commodityId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CateringComdtyXDataDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityBoxInfo(commodityId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityShareInfoDTO> GetCommoditySpreadPercentByAppId(System.Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityShareInfoDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommoditySpreadPercentByAppId(appId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveCommoditySpreadPercent(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityShareInfoDTO dto)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SaveCommoditySpreadPercent(dto);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public ResultDTO<List<CommodityTaxRateCDTO>> GetSingleCommodityCode(string name,double taxrate, int pageIndex, int pageSize)
        {
            //定义返回值
            ResultDTO<List<CommodityTaxRateCDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetSingleCommodityCode(name,-1, pageIndex, pageSize);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }



        /// <summary>
        /// 获取Commodity
        /// </summary>
        /// <param name="CommodityId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CommodityDTO GetCommodityDetail(Guid CommodityId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CommodityDTO result;
            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityDetail(CommodityId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }


        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetCommodityRefoundFreightTemp(System.Guid CommodityId, System.Guid TempId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SetCommodityRefoundFreightTemp(CommodityId, TempId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndTemplateDTO>> GetCommodityFreightTemplate(System.Guid AppId, string GoodName, string State, string FreightID, int pageIndex, int pageSize)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndTemplateDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityFreightTemplate(AppId, GoodName, State, FreightID, pageIndex, pageSize);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
    }
}
