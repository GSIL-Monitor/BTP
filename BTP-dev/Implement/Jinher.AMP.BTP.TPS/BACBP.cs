using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BAC.Deploy.BEDTO;
using Jinher.AMP.BAC.ISV.Facade;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.TPS
{
    /*
     * 请注意：！！！！！！！！！！！！！！！
     * 外部引用请将代码写到TPS.XXXFacade中，自定义扩展请写到TPS.XXXSV、TPS.XXXBP中
     */

    public class BACBP : OutSideServiceBase<BACBPFacade>
    {
        /// <summary>
        /// 校验app是否选用商品分类功能
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static bool CheckCommodityCategory(Guid appId)
        {
            return Instance.CheckFunctionExists(appId, FunctionCodeConst.CategoryManageFunctionCode);
        }

        /// <summary>
        /// 校验app是否选用三级分销功能项
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static bool CheckAppDistribute(Guid appId)
        {
            return Instance.CheckFunctionExists(appId, FunctionCodeConst.ThreeLeveDistributionSale);
        }

        /// <summary>
        /// 校验app是否选用分成推广功能项
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static bool CheckAppShare(Guid appId)
        {
            return Instance.CheckFunctionExists(appId, FunctionCodeConst.SharePromotion);
        }

        /// <summary>
        /// 校验是否包含积分功能
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static bool CheckMyIntegral(Guid appId)
        {
            return Instance.CheckFunctionExists(appId, FunctionCodeConst.MyIntegral);
        }
        /// <summary>
        /// 校验app是否启用分成推广功能
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static bool CheckSharePromotion(Guid appId)
        {
            return Instance.CheckFunctionExists(appId, FunctionCodeConst.SharePromotion);
        }
        /// <summary>
        /// 校验app是否启用点餐列表功能
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static bool CheckFoodOderList(Guid appId)
        {
            return Instance.CheckFunctionExists(appId, FunctionCodeConst.FoodOderList);
        }

        /// <summary>
        /// 校验app是否启用“商品评价”功能
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static bool CheckCommodityReview(Guid appId)
        {
            return Instance.CheckFunctionExists(appId, FunctionCodeConst.CommodityReviewFunctionCode);
        }
        /// <summary>
        /// 校验app是否启用"上传视频"功能
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static bool CheckCommodityVideo(Guid appId)
        {
            return Instance.CheckFunctionExists(appId, FunctionCodeConst.CommodityVideo);
        }

        /// <summary>
        /// 校验app是否有电商基础功能
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static bool CheckTradeBasic(Guid appId)
        {
            return Instance.CheckFunctionExists(appId, FunctionCodeConst.TradeBasicFunctionCode);
        }
        /// <summary>
        /// 校验app是否有渠道推广功能
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static bool CheckChannel(Guid appId)
        {
            return Instance.CheckFunctionExists(appId, FunctionCodeConst.ChannelPromotion);
        }
        /// <summary>
        /// 校验app是否有自提点功能
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static bool CheckAppSelfTake(Guid appId)
        {
            return Instance.CheckFunctionExists(appId, FunctionCodeConst.AppSelfTake);
        }
    }

    public class BACBPFacade : OutSideFacadeBase
    {
        /// <summary>
        /// 检查定制应用{appId}是否选择了{functionCode}功能项。
        /// </summary>
        /// <param name="appId">应用id</param>
        /// <param name="functionCode">功能项编码</param>
        /// <returns>{functionCode}功能项是否存在</returns>
        [BTPAopLogMethod]
        public bool CheckFunctionExists(Guid appId, string functionCode)
        {
            bool isExist = false;
            try
            {
                //    CreateTemplateAPKFacade facade = new CreateTemplateAPKFacade();
                //    facade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                //    isExist = facade.Instance.CheckFunctionExists(appId, functionCode);
                AppDataServiceFacade facade = new AppDataServiceFacade();
                facade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                isExist = facade.CheckFunctionExists(appId, functionCode);
            }
            catch (Exception ex)
            {
                LogHelper.Error("BACBP.CheckFunctionExists异常，异常信息", ex);
            }
            return isExist;
        }
    }
}
