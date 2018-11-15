using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.BE
{
    public  partial class OrderRefundInfo
    {
        #region 基类抽象方法重载
			         
		public override void BusinessRuleValidate()				
		{
					}
				#endregion 
		#region 基类虚方法重写
		public override void SetDefaultValue()
		{
		    base.SetDefaultValue();
		}     
		#endregion 		
    }
}
