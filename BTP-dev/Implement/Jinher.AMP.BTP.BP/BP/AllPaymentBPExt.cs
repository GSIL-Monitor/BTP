
/***************
功能描述: BTPBP
作    者: 
创建时间: 2014/4/8 16:23:10
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
    public partial class AllPaymentBP : BaseBP, IAllPayment
    {

        /// <summary>
        /// 由ID得到支付名称
        /// </summary>
        /// <param name="allPaymentId">支付ID</param>
        /// <returns></returns>
        public string GetNameByIdExt(System.Guid allPaymentId)
        {
            string payName = AllPayment.ObjectSet().Where(n => n.Id == allPaymentId).Select(n => n.PaymentName).FirstOrDefault();
            return payName;
        }
    }
}