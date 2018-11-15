
/***************
功能描述: BTPBP
作    者: LSH
创建时间: 2017/9/21 15:02:29
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class JdOrderItemBP : BaseBP, IJdOrderItem
    {
        /// <summary>
        /// 查询JdOrderItem信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<Jinher.AMP.BTP.Deploy.JdOrderItemDTO> GetJdOrderItemList(Jinher.AMP.BTP.Deploy.JdOrderItemDTO search)
        {
            base.Do(false);
            return this.GetJdOrderItemListExt(search);
        }


        /// <summary>
        /// 保存JdOrderItem信息
        /// </summary>
        /// <param name="VatInvoiceProof"></param>
        /// <returns></returns>
        public ResultDTO SaveJdOrderItem(Jinher.AMP.BTP.Deploy.JdOrderItemDTO model)
        {
            base.Do(false);
            return this.SaveJdOrderItemExt(model);
        }


        /// <summary>
        /// 修改JdOrderItem
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO UpdateJdOrderItem(Jinher.AMP.BTP.Deploy.JdOrderItemDTO model)
        {
            base.Do(false);
            return this.UpdateJdOrderItemExt(model);
        }


        /// <summary>
        /// 删除JdOrderItem
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO DeleteJdOrderItem(List<string> jdorders)
        {
            base.Do(false);
            return this.DeleteJdOrderItemExt(jdorders);
        }


        /// <summary>
        /// 查询JdOrderItem信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<Jinher.AMP.BTP.Deploy.JdOrderItemDTO> GetList(List<string> jdporders)
        {
            base.Do(false);
            return this.GetListExt(jdporders);
        }

        /// <summary>
        /// 查询JdOrderItem信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<Jinher.AMP.BTP.Deploy.JdOrderItemDTO> GetJdOrderIdList(List<string> jdorders)
        {
            base.Do();
            return this.GetJdOrderIdListExt(jdorders);
        }

        /// <summary>
        /// 根据订单Id查询JdOrderItem信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<Jinher.AMP.BTP.Deploy.JdOrderItemDTO> GetJdOrderItemLists(List<Guid> TempIds)
        {
            base.Do(false);
            return this.GetJdOrderItemListsExt(TempIds);
        }

    }
}
