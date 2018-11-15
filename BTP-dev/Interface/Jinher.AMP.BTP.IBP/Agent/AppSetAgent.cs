
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2015-1-9 11:21:58
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Agent.Base;

namespace Jinher.AMP.BTP.IBP.Agent
{

    public class AppSetAgent : BaseBpAgent<IAppSet>, IAppSet
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.AppSetAppGridDTO GetAllCommodityApp(int pageIndex, int pageSize, string appNameForQry, int addToAppSetStatus)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.AppSetAppGridDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAllCommodityApp(pageIndex, pageSize, appNameForQry, addToAppSetStatus);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddAppToAppSet(List<Tuple<Guid, string, string, DateTime>> appInfoList, System.Guid appSetId, int appSetType)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.AddAppToAppSet(appInfoList, appSetId, appSetType);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelAppFromAppSet(System.Collections.Generic.List<System.Guid> appIdList, System.Guid appSetId, int appSetType)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DelAppFromAppSet(appIdList, appSetId, appSetType);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.AppSetCategoryDTO> GetCategoryListForTree()
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.AppSetCategoryDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCategoryListForTree();

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.AppSetCategoryDTO> GetCategoryListForTree(Guid appId)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.AppSetCategoryDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCategoryListForTree(appId);

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
        public Tuple<Guid, int, string> AddCategory(string name, Guid parentId, string picturesPath)
        {
            //定义返回值
            Tuple<Guid, int, string> result;

            try
            {
                //调用代理方法
                result = base.Channel.AddCategory(name, parentId, picturesPath);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelCategory(System.Guid id)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DelCategory(id);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateCategory(System.Guid id, string name,string picturesPath)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateCategory(id, name, picturesPath);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO ChangeCategorySort(System.Guid categoryId, System.Guid targetCategoryId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.ChangeCategorySort(categoryId, targetCategoryId);

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
        public int GetCommodityCountInCategory(System.Guid categoryId)
        {
            //定义返回值
            int result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityCountInCategory(categoryId);

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
        public int GetCommodityCountInCategory2(System.Guid categoryId)
        {
            //定义返回值
            int result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityCountInCategory2(categoryId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.AppSetCommodityGridDTO GetCommodityInCategory(System.Guid categoryId, int pageIndex, int pageSize, string appNameForQry, string commodityNameForQry)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.AppSetCommodityGridDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityInCategory(categoryId, pageIndex, pageSize, appNameForQry, commodityNameForQry);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.AppSetCommodityGrid2DTO GetCommodityInCategory2(Guid belongTo, System.Guid categoryId, int pageIndex, int pageSize, string appNameForQry, string commodityNameForQry)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.AppSetCommodityGrid2DTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityInCategory2(belongTo, categoryId, pageIndex, pageSize, appNameForQry, commodityNameForQry);

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
       
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddCommodityToCategory(System.Collections.Generic.List<System.Guid> commodityIdList, System.Collections.Generic.List<System.Guid> categoryIdList)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.AddCommodityToCategory(commodityIdList, categoryIdList);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddCommodityToCategory2(System.Collections.Generic.List<System.Guid> commodityIdList, System.Collections.Generic.List<System.Guid> categoryIdList)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.AddCommodityToCategory2(commodityIdList, categoryIdList);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO ReOrderCommodityInCategory(System.Guid categoryId, System.Collections.Generic.List<System.Guid> commodityIdList, System.Collections.Generic.List<int> commoditySortList)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.ReOrderCommodityInCategory(categoryId, commodityIdList, commoditySortList);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelCommodityFromCategory(System.Collections.Generic.List<System.Guid> commodityIdList, System.Guid categoryId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DelCommodityFromCategory(commodityIdList, categoryId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelCommodityFromCategory2(System.Collections.Generic.List<System.Guid> commodityIdList, System.Guid categoryId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DelCommodityFromCategory2(commodityIdList, categoryId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO ChangeCommodityOrderInCategory(Guid categoryId, Guid commodityId, int direction)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.ChangeCommodityOrderInCategory(categoryId, commodityId, direction);

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

        public ResultDTO TopCommodityOrderInCategory(Guid categoryId, Guid commodityId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.TopCommodityOrderInCategory(categoryId, commodityId);

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
            return result; ;
        }
        public ResultDTO SetSetCommodityOrder(AppSetSortDTO appSetSortDTO)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SetSetCommodityOrder(appSetSortDTO);

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
            return result; ;
        }
        public ResultDTO SetSetCommodityOrder2(AppSetSortDTO appSetSortDTO)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SetSetCommodityOrder2(appSetSortDTO);

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
            return result; ;
        }
    }
}