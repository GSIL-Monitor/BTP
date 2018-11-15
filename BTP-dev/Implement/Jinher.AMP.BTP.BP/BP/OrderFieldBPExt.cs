
/***************
功能描述: BTPBP
作    者: 
创建时间: 2016/5/29 11:37:09
***************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.AMP.BTP.TPS;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.Common.Loging;
using NPOI.SS.Formula.Functions;
using AppExtensionDTO = Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.Common;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class OrderFieldBP : BaseBP, IOrderField
    {
        /// <summary>
        /// 获取订单设置信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.OrderFieldDTO GetOrderSetExt(Guid AppId)
        {
            OrderFieldDTO model = new OrderFieldDTO();
            OrderField orderfiled = null;
            if (AppId!=Guid.Empty)
            {
                orderfiled = OrderField.ObjectSet().FirstOrDefault(p => p.AppId == AppId);
            }
            if (orderfiled!=null)
            {
                model.Id = orderfiled.Id;
                model.AppId = orderfiled.AppId;
                model.FirstField = orderfiled.FirstField;
                model.SecondField = orderfiled.SecondField;
                model.ThirdField = orderfiled.ThirdField;
                model.SubId = orderfiled.SubId;
                model.SubTime = orderfiled.SubTime;
                model.ModifiedOn = orderfiled.ModifiedOn;
            }
            return model;
        }
    }
}