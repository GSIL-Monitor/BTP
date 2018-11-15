﻿
/***************
功能描述: BTPBP
作    者: 
创建时间: 2015/7/27 14:02:33
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

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class SettlingAccountBP : BaseBP, ISettlingAccount
    {

        /// <summary>
        /// 获取当前商品结算列表
        /// </summary>
        /// <param name="search">商品结算价检索类</param>
        /// <param name="rowCount">记录数</param>
        /// <returns>结果</returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.SettlingAccountVM> GetNowSettlingAccount(Jinher.AMP.BTP.Deploy.CustomDTO.SettlingAccountSearchDTO search, out int rowCount)
        {
            base.Do();
            return this.GetNowSettlingAccountExt(search, out rowCount);
        }
        /// <summary>
        /// 添加商品的厂家结算价
        /// </summary>
        /// <param name="settlingAccountDTO">结算价实体</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveSettlingAccount(Jinher.AMP.BTP.Deploy.SettlingAccountDTO settlingAccountDTO)
        {
            base.Do();
            return this.SaveSettlingAccountExt(settlingAccountDTO);
        }
        /// <summary>
        /// 获取当前商品厂家结算价设置的历史列表
        /// </summary>
        /// <param name="search">商品结算价修改历史检索类</param>
        /// <param name="rowCount">记录数</param>
        /// <returns>结果</returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.SettlingAccountDTO> GetHistorySettlingAccount(Jinher.AMP.BTP.Deploy.CustomDTO.SettlingAccountHistorySearchDTO search, out int rowCount)
        {
            base.Do();
            return this.GetHistorySettlingAccountExt(search, out rowCount);
        }
        /// <summary>
        /// 删除厂家结算记录
        /// </summary>
        /// <param name="ids">id列表</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteSettlingAccountById(List<Guid> ids)
        {
            base.Do();
            return this.DeleteSettlingAccountByIdExt(ids);
        }
    }
}