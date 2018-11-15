/***************
功能描述: BTPFacade
作    者: 
创建时间: 2017/9/21 15:02:27
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.IBP.Facade
{   
    public class JdOrderItemFacade : BaseFacade<IJdOrderItem>
    {
        /// <summary>
        /// 查询JdOrderItem信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<Jinher.AMP.BTP.Deploy.JdOrderItemDTO> GetJdOrderItemList(Jinher.AMP.BTP.Deploy.JdOrderItemDTO search)
        {
            base.Do();
            return this.Command.GetJdOrderItemList(search);
        }


        /// <summary>
        /// 保存JdOrderItem信息
        /// </summary>
        /// <param name="VatInvoiceProof"></param>
        /// <returns></returns>
        public ResultDTO SaveJdOrderItem(Jinher.AMP.BTP.Deploy.JdOrderItemDTO model)
        {
            base.Do();
            return this.Command.SaveJdOrderItem(model);
        }


        /// <summary>
        /// 修改JdOrderItem
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO UpdateJdOrderItem(Jinher.AMP.BTP.Deploy.JdOrderItemDTO model)
        {
            base.Do();
            return this.Command.UpdateJdOrderItem(model);
        }


        /// <summary>
        /// 删除JdOrderItem
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO DeleteJdOrderItem(List<string> jdorders)
        {
            base.Do();
            return this.Command.DeleteJdOrderItem(jdorders);
        }


        /// <summary>
        /// 查询JdOrderItem信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<Jinher.AMP.BTP.Deploy.JdOrderItemDTO> GetList(List<string> jdporders)
        {
            base.Do();
            return this.Command.GetList(jdporders);
        }

        /// <summary>
        /// 查询JdOrderItem信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<Jinher.AMP.BTP.Deploy.JdOrderItemDTO> GetJdOrderIdList(List<string> jdorders)
        {
            base.Do();
            return this.Command.GetJdOrderIdList(jdorders);
        }


        /// <summary>
        /// 根据订单Id查询JdOrderItem信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<Jinher.AMP.BTP.Deploy.JdOrderItemDTO> GetJdOrderItemLists(List<Guid> TempIds)
        {
            base.Do();
            return this.Command.GetJdOrderItemLists(TempIds);
        }
    }
}
