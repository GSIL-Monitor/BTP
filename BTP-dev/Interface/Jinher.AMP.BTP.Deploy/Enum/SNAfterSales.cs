using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.Enum
{
    /// <summary>
    /// 苏宁厂送状态接口
    /// </summary>
    public enum SNFactoryDeliveryEnum
    {
       
        /// <summary>
        /// 1 上门取件-非厂送-自营
        /// </summary>
        NonFactoryDelivery = 1,
        /// <summary>
        /// 2快递寄回-厂送
        /// </summary>
        FactoryDelivery = 2,
        /// <summary>
        /// 获取失败
        /// </summary>
        Error = 3

    }
}
