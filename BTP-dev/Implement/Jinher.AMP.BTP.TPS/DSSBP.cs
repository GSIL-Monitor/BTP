using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.DSS.IBP.Facade;


namespace Jinher.AMP.BTP.TPS
{
    /*
     * 请注意：！！！！！！！！！！！！！！！
     * 外部引用请将代码写到TPS.XXXFacade中，自定义扩展请写到TPS.XXXSV、TPS.XXXBP中
     */

    public class DSSBP : OutSideServiceBase<DSSBPFacade>
    {

    }

    public class DSSBPFacade : OutSideFacadeBase
    {
        /// <summary>
        /// 分销商列表信息
        /// </summary>
        /// <param name="manageVM"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ResultDTO<ManageDTO> ManageInfo(ManageVM manageVM)
        {
            ResultDTO<ManageDTO> result = null;
            try
            {
                var url = string.Format("{0}/Jinher.AMP.DSS.BP.DistributorBP.svc/ManageInfo", CustomConfig.DSSUrl);
                var resultStr = BaseRequest.CreateRequest(url, manageVM, true, "a");
                result = JsonHelper.JsonDeserialize<ResultDTO<ManageDTO>>(resultStr);
                //DistributorFacade distributorFacade = new DistributorFacade();
                //distributorFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                //result = distributorFacade.ManageInfo(manageVM);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("DSSBP.ManageInfo分销商列表信息服务异常。manageVM：{0}", JsonHelper.JsonSerializer(manageVM)), ex);
            }
            return result;
        }
        /// <summary>
        /// 我的分销-概况
        /// </summary>
        /// <param name="distributorProfitsSearchDTO"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ResultDTO<DistributorProfitsResultDTO> GetDistributorProfits(DistributorProfitsSearchDTO distributorProfitsSearchDTO)
        {
            ResultDTO<DistributorProfitsResultDTO> result = null;
            try
            {
                var url = string.Format("{0}/Jinher.AMP.DSS.BP.DistributorBP.svc/GetDistributorProfits", CustomConfig.DSSUrl);
                var resultStr = BaseRequest.CreateRequest(url, distributorProfitsSearchDTO, true, "a");
                result = JsonHelper.JsonDeserialize<ResultDTO<DistributorProfitsResultDTO>>(resultStr);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("DSSBP.GetDistributorProfits我的分销-概况服务异常。distributorProfitsSearchDTO：{0}", JsonHelper.JsonSerializer(distributorProfitsSearchDTO)), ex);
            }
            return result;
        }

        /// <summary>
        /// 我的分销-列表
        /// </summary>
        /// <param name="distributorProfitsSearchDTO"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ResultDTO<DistributorProfitsResultDTO> GetDistributorList(DistributorProfitsSearchDTO distributorProfitsSearchDTO)
        {
            ResultDTO<DistributorProfitsResultDTO> result = null;
            try
            {
                var url = string.Format("{0}/Jinher.AMP.DSS.BP.DistributorBP.svc/GetDistributorList", CustomConfig.DSSUrl);
                var resultStr = BaseRequest.CreateRequest(url, distributorProfitsSearchDTO, true, "a");
                result = JsonHelper.JsonDeserialize<ResultDTO<DistributorProfitsResultDTO>>(resultStr);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("DSSBP.GetDistributorList我的分销-列表服务异常。distributorProfitsSearchDTO：{0}", JsonHelper.JsonSerializer(distributorProfitsSearchDTO)), ex);
            }
            return result;
        }
    }



}
